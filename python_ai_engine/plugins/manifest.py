"""
Plugin manifest parser.
Reads and validates manifest.json files.
"""

import json
import logging
from dataclasses import dataclass, field
from typing import List, Optional

logger = logging.getLogger(__name__)


@dataclass
class PluginManifest:
    """Parsed plugin manifest."""
    name: str
    version: str
    services: List[str]
    entry: str
    plugin_class: str
    permissions: List[str] = field(default_factory=list)
    worker_pool: str = 'realtime'
    timeout_ms: int = 3000
    preload_models: List[str] = field(default_factory=list)
    sandbox: str = 'restricted'
    description: str = ''
    author: str = ''
    plugin_dir: str = ''

    @staticmethod
    def from_file(manifest_path: str) -> Optional['PluginManifest']:
        """Load manifest from JSON file."""
        try:
            with open(manifest_path, 'r', encoding='utf-8') as f:
                data = json.load(f)
            return PluginManifest.from_dict(data)
        except Exception as e:
            logger.error("Failed to load manifest %s: %s", manifest_path, e)
            return None

    @staticmethod
    def from_dict(data: dict) -> 'PluginManifest':
        """Create manifest from dict."""
        return PluginManifest(
            name=data['name'],
            version=data.get('version', '0.0.1'),
            services=data.get('services', []),
            entry=data.get('entry', 'entry.py'),
            plugin_class=data.get('class', ''),
            permissions=data.get('permissions', []),
            worker_pool=data.get('workerPool', 'realtime'),
            timeout_ms=data.get('timeoutMs', 3000),
            preload_models=data.get('preloadModels', []),
            sandbox=data.get('sandbox', 'restricted'),
            description=data.get('description', ''),
            author=data.get('author', ''),
        )

    def validate(self) -> tuple:
        """Validate manifest. Returns (is_valid, error_message)."""
        if not self.name:
            return False, "Plugin name is required"
        if not self.services:
            return False, "At least one service must be defined"
        if not self.entry:
            return False, "Entry file is required"
        if not self.plugin_class:
            return False, "Plugin class name is required"
        if self.worker_pool not in ('realtime', 'batch', 'script'):
            return False, f"Invalid workerPool: {self.worker_pool}"
        return True, None
