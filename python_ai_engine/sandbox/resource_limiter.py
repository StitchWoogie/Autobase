"""
Resource limiter for sandboxed script execution.
Enforces time, memory, and output limits.
"""

import logging
import signal
import sys
import threading
from typing import Optional

logger = logging.getLogger(__name__)


class ResourceLimiter:
    """
    Applies resource limits to script execution.
    Uses threading timer for timeout on Windows (no SIGALRM).
    """

    def __init__(self, timeout_sec: float = 5.0, max_memory_mb: int = 256,
                 max_output_kb: int = 1024):
        self._timeout_sec = timeout_sec
        self._max_memory_mb = max_memory_mb
        self._max_output_kb = max_output_kb
        self._timer: Optional[threading.Timer] = None
        self._timed_out = False

    def start(self):
        """Start the resource limiter (timeout timer)."""
        self._timed_out = False
        self._timer = threading.Timer(self._timeout_sec, self._on_timeout)
        self._timer.daemon = True
        self._timer.start()

    def stop(self):
        """Stop the resource limiter."""
        if self._timer:
            self._timer.cancel()
            self._timer = None

    def _on_timeout(self):
        """Called when timeout expires."""
        self._timed_out = True
        logger.warning("Script timeout after %.1fs", self._timeout_sec)

    @property
    def is_timed_out(self) -> bool:
        return self._timed_out

    def check_memory(self) -> bool:
        """Check if memory usage is within limits. Returns True if OK."""
        try:
            import psutil
            proc = psutil.Process()
            rss_mb = proc.memory_info().rss / (1024 * 1024)
            return rss_mb <= self._max_memory_mb
        except ImportError:
            return True
        except Exception:
            return True

    def get_limits(self) -> dict:
        return {
            'timeout_sec': self._timeout_sec,
            'max_memory_mb': self._max_memory_mb,
            'max_output_kb': self._max_output_kb,
        }
