"""
LRU Model Cache.
Loaded model instances cached in memory with eviction policy.
"""

import logging
import threading
import time
from collections import OrderedDict
from typing import Any, Optional

logger = logging.getLogger(__name__)


class CacheEntry:
    __slots__ = ('key', 'model', 'framework', 'loaded_at', 'last_used', 'use_count', 'size_mb')

    def __init__(self, key: str, model: Any, framework: str, size_mb: float = 0.0):
        self.key = key
        self.model = model
        self.framework = framework
        self.loaded_at = time.time()
        self.last_used = time.time()
        self.use_count = 0
        self.size_mb = size_mb

    def touch(self):
        self.last_used = time.time()
        self.use_count += 1


class ModelCache:
    """
    LRU model cache with max count and memory limit.
    Thread-safe.
    """

    def __init__(self, max_count: int = 10, max_memory_mb: float = 2048.0):
        self._cache: OrderedDict[str, CacheEntry] = OrderedDict()
        self._lock = threading.Lock()
        self._max_count = max_count
        self._max_memory_mb = max_memory_mb
        self._total_memory_mb = 0.0

    def get(self, key: str) -> Optional[Any]:
        """Get a cached model. Returns None if not cached."""
        with self._lock:
            entry = self._cache.get(key)
            if entry is None:
                return None
            entry.touch()
            self._cache.move_to_end(key)
            return entry.model

    def put(self, key: str, model: Any, framework: str, size_mb: float = 0.0):
        """Put a model into cache, evicting LRU if necessary."""
        with self._lock:
            if key in self._cache:
                old = self._cache.pop(key)
                self._total_memory_mb -= old.size_mb
                self._unload_model(old)

            # Evict until we have room
            while len(self._cache) >= self._max_count:
                self._evict_lru()
            while self._total_memory_mb + size_mb > self._max_memory_mb and self._cache:
                self._evict_lru()

            entry = CacheEntry(key, model, framework, size_mb)
            self._cache[key] = entry
            self._total_memory_mb += size_mb
            logger.info("Model cached: %s (%.1fMB, total: %.1fMB)",
                        key, size_mb, self._total_memory_mb)

    def remove(self, key: str) -> bool:
        """Remove a specific model from cache."""
        with self._lock:
            entry = self._cache.pop(key, None)
            if entry:
                self._total_memory_mb -= entry.size_mb
                self._unload_model(entry)
                logger.info("Model removed from cache: %s", key)
                return True
            return False

    def contains(self, key: str) -> bool:
        with self._lock:
            return key in self._cache

    def clear(self):
        """Clear all cached models."""
        with self._lock:
            for entry in self._cache.values():
                self._unload_model(entry)
            self._cache.clear()
            self._total_memory_mb = 0.0
            logger.info("Model cache cleared")

    def get_stats(self) -> dict:
        with self._lock:
            entries = []
            for key, entry in self._cache.items():
                entries.append({
                    'key': key,
                    'framework': entry.framework,
                    'size_mb': entry.size_mb,
                    'use_count': entry.use_count,
                    'loaded_at': entry.loaded_at,
                    'last_used': entry.last_used,
                })
            return {
                'count': len(self._cache),
                'max_count': self._max_count,
                'total_memory_mb': round(self._total_memory_mb, 1),
                'max_memory_mb': self._max_memory_mb,
                'entries': entries,
            }

    def _evict_lru(self):
        """Evict the least recently used entry. Must hold lock."""
        if not self._cache:
            return
        key, entry = self._cache.popitem(last=False)
        self._total_memory_mb -= entry.size_mb
        self._unload_model(entry)
        logger.info("Model evicted (LRU): %s", key)

    def _unload_model(self, entry: CacheEntry):
        """Clean up a model instance."""
        try:
            if hasattr(entry.model, 'close'):
                entry.model.close()
            elif hasattr(entry.model, 'dispose'):
                entry.model.dispose()
        except Exception as e:
            logger.warning("Error unloading model %s: %s", entry.key, e)
        entry.model = None
