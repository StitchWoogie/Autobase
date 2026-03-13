"""
Model Registry.
모델 디렉토리 스캔, metadata.json 파싱, 버전 관리.
"""

import json
import logging
import os
from pathlib import Path
from typing import Dict, List, Optional

logger = logging.getLogger(__name__)


class ModelInfo:
    """Single model version metadata."""

    __slots__ = (
        'name', 'version', 'framework', 'path', 'model_dir',
        'input_schema', 'output_schema', 'created', 'metrics', 'enabled',
    )

    def __init__(self, name: str, version: str, framework: str,
                 path: str, model_dir: str, **kwargs):
        self.name = name
        self.version = version
        self.framework = framework
        self.path = path
        self.model_dir = model_dir
        self.input_schema = kwargs.get('inputSchema')
        self.output_schema = kwargs.get('outputSchema')
        self.created = kwargs.get('created')
        self.metrics = kwargs.get('metrics', {})
        self.enabled = kwargs.get('enabled', True)

    @property
    def full_name(self) -> str:
        return f"{self.name}:{self.version}"

    @property
    def model_path(self) -> str:
        return os.path.join(self.model_dir, self.path)

    def to_dict(self) -> dict:
        return {
            'name': self.name,
            'version': self.version,
            'framework': self.framework,
            'path': self.model_path,
            'created': self.created,
            'metrics': self.metrics,
            'enabled': self.enabled,
        }


class ModelRegistry:
    """
    Model registry — scans model directory, tracks versions.

    Directory structure:
        models/
        ├── power_predictor/
        │   ├── 1.0.0/
        │   │   ├── model.onnx
        │   │   └── metadata.json
        │   └── 1.1.0/
        │       ├── model.onnx
        │       └── metadata.json
        └── anomaly_detector/
            └── 1.0.0/
                └── metadata.json
    """

    def __init__(self, registry_path: str):
        self.registry_path = Path(registry_path)
        self._models: Dict[str, Dict[str, ModelInfo]] = {}  # name -> {version -> ModelInfo}
        self._active_versions: Dict[str, str] = {}  # name -> active version

    def scan(self):
        """Scan the registry directory for models."""
        if not self.registry_path.exists():
            logger.warning("Model registry path does not exist: %s", self.registry_path)
            return

        self._models.clear()
        self._active_versions.clear()

        for model_dir in sorted(self.registry_path.iterdir()):
            if not model_dir.is_dir():
                continue
            model_name = model_dir.name
            self._models[model_name] = {}

            versions = []
            for ver_dir in sorted(model_dir.iterdir()):
                if not ver_dir.is_dir():
                    continue
                meta_path = ver_dir / 'metadata.json'
                if not meta_path.exists():
                    continue
                try:
                    with open(meta_path, 'r', encoding='utf-8') as f:
                        meta = json.load(f)
                    info = ModelInfo(
                        name=meta.get('name', model_name),
                        version=meta.get('version', ver_dir.name),
                        framework=meta.get('framework', 'unknown'),
                        path=meta.get('path', ''),
                        model_dir=str(ver_dir),
                        **{k: v for k, v in meta.items()
                           if k not in ('name', 'version', 'framework', 'path')}
                    )
                    if info.enabled:
                        self._models[model_name][info.version] = info
                        versions.append(info.version)
                except Exception as e:
                    logger.error("Failed to load metadata %s: %s", meta_path, e)

            # Set active version (latest by default)
            if versions:
                self._active_versions[model_name] = sorted(versions)[-1]

        total = sum(len(v) for v in self._models.values())
        logger.info("Registry scanned: %d models, %d versions", len(self._models), total)

    def get_model(self, name: str, version: str = None) -> Optional[ModelInfo]:
        """Get model info. If version is None, returns active version."""
        versions = self._models.get(name)
        if not versions:
            return None
        if version:
            return versions.get(version)
        active_ver = self._active_versions.get(name)
        if active_ver:
            return versions.get(active_ver)
        return None

    def set_active_version(self, name: str, version: str) -> bool:
        """Set the active version for a model."""
        versions = self._models.get(name)
        if not versions or version not in versions:
            return False
        old = self._active_versions.get(name)
        self._active_versions[name] = version
        logger.info("Model %s active version: %s -> %s", name, old, version)
        return True

    def get_previous_version(self, name: str) -> Optional[str]:
        """Get the version before the current active one (for rollback)."""
        versions = self._models.get(name)
        if not versions:
            return None
        sorted_versions = sorted(versions.keys())
        active = self._active_versions.get(name)
        if active and active in sorted_versions:
            idx = sorted_versions.index(active)
            if idx > 0:
                return sorted_versions[idx - 1]
        return None

    def list_models(self) -> List[dict]:
        """List all models with their active versions."""
        result = []
        for name, versions in self._models.items():
            active = self._active_versions.get(name)
            result.append({
                'name': name,
                'activeVersion': active,
                'versions': sorted(versions.keys()),
                'versionCount': len(versions),
            })
        return result

    def list_loaded_models(self) -> List[str]:
        """List active model full names."""
        result = []
        for name, ver in self._active_versions.items():
            result.append(f"{name}:{ver}")
        return result

    def register_model(self, info: ModelInfo):
        """Manually register a model (for dynamically loaded models)."""
        if info.name not in self._models:
            self._models[info.name] = {}
        self._models[info.name][info.version] = info
        if info.name not in self._active_versions:
            self._active_versions[info.name] = info.version
        logger.info("Model registered: %s", info.full_name)
