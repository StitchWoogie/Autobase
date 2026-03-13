"""
Model hot reload and rollback (Phase 4).
Watches model directory for changes and reloads without engine restart.
"""

import asyncio
import logging
import os
import time
from typing import Dict, Optional

from .registry import ModelRegistry, ModelInfo
from .cache import ModelCache

logger = logging.getLogger(__name__)


class ModelHotReloader:
    """
    Watches the model registry directory for changes.
    Reloads models when metadata.json or model files change.
    Supports rollback to previous version on failure.
    """

    def __init__(self, registry: ModelRegistry, cache: ModelCache,
                 check_interval_sec: float = 30.0):
        self._registry = registry
        self._cache = cache
        self._check_interval = check_interval_sec
        self._file_mtimes: Dict[str, float] = {}
        self._running = False
        self._task: Optional[asyncio.Task] = None
        self._loaders = {}  # framework -> BaseModelLoader

    def set_loaders(self, loaders: dict):
        """Set the framework loaders dict."""
        self._loaders = loaders

    async def start(self):
        """Start the hot reload watcher."""
        self._running = True
        self._snapshot_mtimes()
        self._task = asyncio.create_task(self._watch_loop())
        logger.info("Model hot reloader started (interval: %.0fs)", self._check_interval)

    async def stop(self):
        """Stop the watcher."""
        self._running = False
        if self._task:
            self._task.cancel()
            try:
                await self._task
            except asyncio.CancelledError:
                pass
        logger.info("Model hot reloader stopped")

    async def _watch_loop(self):
        while self._running:
            try:
                await asyncio.sleep(self._check_interval)
                changed = self._detect_changes()
                if changed:
                    logger.info("Model changes detected: %s", changed)
                    await self._handle_changes(changed)
            except asyncio.CancelledError:
                break
            except Exception as e:
                logger.error("Hot reload watch error: %s", e)

    def _snapshot_mtimes(self):
        """Take snapshot of all metadata.json mtimes."""
        self._file_mtimes.clear()
        if not self._registry.registry_path.exists():
            return
        for model_dir in self._registry.registry_path.iterdir():
            if not model_dir.is_dir():
                continue
            for ver_dir in model_dir.iterdir():
                if not ver_dir.is_dir():
                    continue
                meta = ver_dir / 'metadata.json'
                if meta.exists():
                    self._file_mtimes[str(meta)] = os.path.getmtime(str(meta))

    def _detect_changes(self) -> list:
        """Detect changed model metadata files."""
        changed = []
        if not self._registry.registry_path.exists():
            return changed
        for model_dir in self._registry.registry_path.iterdir():
            if not model_dir.is_dir():
                continue
            for ver_dir in model_dir.iterdir():
                if not ver_dir.is_dir():
                    continue
                meta = ver_dir / 'metadata.json'
                if not meta.exists():
                    continue
                key = str(meta)
                mtime = os.path.getmtime(key)
                old_mtime = self._file_mtimes.get(key)
                if old_mtime is None or mtime > old_mtime:
                    changed.append({
                        'model_name': model_dir.name,
                        'version': ver_dir.name,
                        'meta_path': key,
                        'is_new': old_mtime is None,
                    })
                    self._file_mtimes[key] = mtime
        return changed

    async def _handle_changes(self, changes: list):
        """Handle detected model changes."""
        self._registry.scan()
        for change in changes:
            name = change['model_name']
            version = change['version']
            info = self._registry.get_model(name, version)
            if info and info.enabled:
                await self._reload_model(info)

    async def _reload_model(self, info: ModelInfo):
        """Reload a single model. Falls back to rollback on failure."""
        key = info.full_name
        logger.info("Hot reloading model: %s", key)
        try:
            loader = self._loaders.get(info.framework)
            if not loader:
                logger.warning("No loader for framework: %s", info.framework)
                return
            model, size_mb = loader.load(info.model_path)
            loader.warmup(model)
            self._cache.put(key, model, info.framework, size_mb)
            self._registry.set_active_version(info.name, info.version)
            logger.info("Model hot reloaded successfully: %s", key)
        except Exception as e:
            logger.error("Hot reload failed for %s: %s", key, e)
            await self._rollback(info.name)

    async def _rollback(self, model_name: str):
        """Rollback to the previous version."""
        prev_ver = self._registry.get_previous_version(model_name)
        if prev_ver:
            logger.warning("Rolling back %s to version %s", model_name, prev_ver)
            self._registry.set_active_version(model_name, prev_ver)
        else:
            logger.error("No previous version to rollback for %s", model_name)

    def force_reload(self, model_name: str) -> bool:
        """Force reload a specific model (synchronous)."""
        info = self._registry.get_model(model_name)
        if not info:
            return False
        loader = self._loaders.get(info.framework)
        if not loader:
            return False
        try:
            model, size_mb = loader.load(info.model_path)
            loader.warmup(model)
            self._cache.put(info.full_name, model, info.framework, size_mb)
            return True
        except Exception as e:
            logger.error("Force reload failed: %s", e)
            return False
