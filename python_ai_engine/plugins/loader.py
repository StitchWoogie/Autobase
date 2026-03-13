"""
Plugin loader.
Scans plugin directory, loads manifests, instantiates plugins.
"""

import importlib.util
import logging
import os
import sys
from pathlib import Path
from typing import Dict, List, Optional

from .interface import PluginBase, PluginContext
from .manifest import PluginManifest

logger = logging.getLogger(__name__)


class LoadedPlugin:
    """Container for a loaded plugin instance."""

    def __init__(self, manifest: PluginManifest, instance: PluginBase):
        self.manifest = manifest
        self.instance = instance
        self.active = True
        self.error_count = 0

    def deactivate(self, reason: str = ''):
        self.active = False
        logger.warning("Plugin deactivated: %s (%s)", self.manifest.name, reason)


class PluginLoader:
    """
    Loads plugins from plugins/ directory.

    Plugin structure:
        plugins/
        ├── predict_power/
        │   ├── manifest.json
        │   └── entry.py
        └── trend_analysis/
            ├── manifest.json
            └── entry.py
    """

    def __init__(self, plugins_dir: str, context: PluginContext):
        self._plugins_dir = Path(plugins_dir)
        self._context = context
        self._loaded: Dict[str, LoadedPlugin] = {}  # name -> LoadedPlugin
        self._service_map: Dict[str, LoadedPlugin] = {}  # service -> LoadedPlugin

    def scan_and_load(self) -> int:
        """Scan plugins directory and load all valid plugins. Returns count loaded."""
        if not self._plugins_dir.exists():
            logger.info("No plugins directory: %s", self._plugins_dir)
            return 0

        count = 0
        for plugin_dir in sorted(self._plugins_dir.iterdir()):
            if not plugin_dir.is_dir():
                continue
            manifest_path = plugin_dir / 'manifest.json'
            if not manifest_path.exists():
                continue
            try:
                if self._load_plugin(str(plugin_dir), str(manifest_path)):
                    count += 1
            except Exception as e:
                logger.error("Failed to load plugin from %s: %s", plugin_dir, e)

        logger.info("Loaded %d plugins", count)
        return count

    def _load_plugin(self, plugin_dir: str, manifest_path: str) -> bool:
        """Load a single plugin."""
        manifest = PluginManifest.from_file(manifest_path)
        if manifest is None:
            return False

        valid, error = manifest.validate()
        if not valid:
            logger.error("Invalid manifest in %s: %s", plugin_dir, error)
            return False

        manifest.plugin_dir = plugin_dir

        # Load the entry module
        entry_path = os.path.join(plugin_dir, manifest.entry)
        if not os.path.exists(entry_path):
            logger.error("Entry file not found: %s", entry_path)
            return False

        try:
            spec = importlib.util.spec_from_file_location(
                f"plugin_{manifest.name}", entry_path)
            module = importlib.util.module_from_spec(spec)
            sys.modules[spec.name] = module
            spec.loader.exec_module(module)

            # Get plugin class
            plugin_cls = getattr(module, manifest.plugin_class)
            if not issubclass(plugin_cls, PluginBase):
                logger.error("%s does not extend PluginBase", manifest.plugin_class)
                return False

            # Instantiate and initialize
            instance = plugin_cls()
            instance.initialize(self._context)

            loaded = LoadedPlugin(manifest, instance)
            self._loaded[manifest.name] = loaded

            # Map services
            for svc in manifest.services:
                self._service_map[svc] = loaded
                logger.info("Plugin '%s' registered service: %s", manifest.name, svc)

            return True

        except Exception as e:
            logger.error("Plugin load error [%s]: %s", manifest.name, e)
            return False

    def get_handler(self, service: str):
        """Get the plugin handler for a service, or None."""
        loaded = self._service_map.get(service)
        if loaded and loaded.active:
            return loaded
        return None

    def get_loaded_plugins(self) -> List[dict]:
        """Get info about all loaded plugins."""
        return [
            {
                'name': p.manifest.name,
                'version': p.manifest.version,
                'services': p.manifest.services,
                'active': p.active,
                'errors': p.error_count,
            }
            for p in self._loaded.values()
        ]

    def unload_all(self):
        """Dispose all plugins."""
        for name, loaded in self._loaded.items():
            try:
                loaded.instance.dispose()
            except Exception as e:
                logger.error("Plugin dispose error [%s]: %s", name, e)
        self._loaded.clear()
        self._service_map.clear()
