"""
Resource guard — monitors memory/CPU and triggers warnings/restarts.
"""

import asyncio
import logging
import os
import time
from typing import Callable, Optional

logger = logging.getLogger(__name__)


class ResourceGuard:
    """
    Monitors engine resource usage.
    Emits warnings at thresholds and can trigger restarts.
    """

    def __init__(self, max_memory_mb: float = 2048.0,
                 check_interval_sec: float = 10.0,
                 warning_threshold: float = 0.9,
                 critical_threshold: float = 0.95):
        self._max_memory_mb = max_memory_mb
        self._check_interval = check_interval_sec
        self._warning_threshold = warning_threshold
        self._critical_threshold = critical_threshold
        self._running = False
        self._task: Optional[asyncio.Task] = None
        self._on_warning: Optional[Callable] = None
        self._on_critical: Optional[Callable] = None
        self._warned = False

    def set_callbacks(self, on_warning: Callable = None, on_critical: Callable = None):
        self._on_warning = on_warning
        self._on_critical = on_critical

    async def start(self):
        self._running = True
        self._task = asyncio.create_task(self._monitor_loop())
        logger.info("Resource guard started (limit: %.0fMB)", self._max_memory_mb)

    async def stop(self):
        self._running = False
        if self._task:
            self._task.cancel()
            try:
                await self._task
            except asyncio.CancelledError:
                pass

    async def _monitor_loop(self):
        while self._running:
            try:
                await asyncio.sleep(self._check_interval)
                usage = self._check_memory()
                if usage is None:
                    continue

                ratio = usage / self._max_memory_mb
                if ratio >= self._critical_threshold:
                    logger.critical(
                        "CRITICAL: Memory %.0fMB / %.0fMB (%.0f%%)",
                        usage, self._max_memory_mb, ratio * 100)
                    if self._on_critical:
                        await self._on_critical(usage, self._max_memory_mb)
                elif ratio >= self._warning_threshold:
                    if not self._warned:
                        logger.warning(
                            "WARNING: Memory %.0fMB / %.0fMB (%.0f%%)",
                            usage, self._max_memory_mb, ratio * 100)
                        if self._on_warning:
                            await self._on_warning(usage, self._max_memory_mb)
                        self._warned = True
                else:
                    self._warned = False

            except asyncio.CancelledError:
                break
            except Exception as e:
                logger.error("Resource guard error: %s", e)

    def _check_memory(self) -> Optional[float]:
        """Get current process RSS in MB."""
        try:
            import psutil
            proc = psutil.Process()
            return proc.memory_info().rss / (1024 * 1024)
        except ImportError:
            return None
        except Exception:
            return None

    def get_status(self) -> dict:
        usage = self._check_memory()
        return {
            'current_mb': round(usage, 1) if usage else -1,
            'limit_mb': self._max_memory_mb,
            'usage_ratio': round(usage / self._max_memory_mb, 3) if usage else -1,
            'warning_threshold': self._warning_threshold,
            'critical_threshold': self._critical_threshold,
        }
