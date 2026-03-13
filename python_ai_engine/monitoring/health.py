"""
Health check response builder.
Collects status from all subsystems.
"""

import platform
import time
from typing import Optional


class HealthCollector:
    """Collects health info from all subsystems."""

    def __init__(self, version: str = '1.0.0'):
        self.version = version
        self._start_time = time.time()
        self._worker_pool = None
        self._scheduler = None
        self._model_registry = None
        self._model_cache = None

    def set_worker_pool(self, pool):
        self._worker_pool = pool

    def set_scheduler(self, scheduler):
        self._scheduler = scheduler

    def set_model_registry(self, registry):
        self._model_registry = registry

    def set_model_cache(self, cache):
        self._model_cache = cache

    def collect(self) -> dict:
        """Build the full health response."""
        result = {
            'version': self.version,
            'uptime_sec': round(time.time() - self._start_time, 1),
            'state': 'running',
            'python_version': platform.python_version(),
            'platform': platform.platform(),
        }

        # Workers
        if self._worker_pool:
            result['workers'] = self._worker_pool.get_stats()
        else:
            result['workers'] = {}

        # Queues
        if self._scheduler:
            result['queues'] = self._scheduler.get_queue_stats()
        else:
            result['queues'] = {}

        # Memory
        result['memory'] = self._get_memory_info()

        # Models
        if self._model_registry:
            result['models'] = {
                'loaded': self._model_registry.list_loaded_models()
            }
        if self._model_cache:
            result['cache'] = self._model_cache.get_stats()

        return result

    def _get_memory_info(self) -> dict:
        try:
            import psutil
            proc = psutil.Process()
            mem = proc.memory_info()
            return {
                'rss_mb': round(mem.rss / (1024 * 1024), 1),
                'vms_mb': round(mem.vms / (1024 * 1024), 1),
            }
        except ImportError:
            return {'rss_mb': -1, 'note': 'psutil not installed'}
        except Exception:
            return {'rss_mb': -1}
