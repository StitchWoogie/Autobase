"""
Backpressure policy manager.
Monitors queue levels and emits events.
"""

import logging
from typing import Callable, Dict, Optional

from .job_queue import JobQueue

logger = logging.getLogger(__name__)


class BackpressurePolicy:
    """
    Monitors queue utilization and triggers events.

    Thresholds:
      < 70%  -> normal
      70~90% -> warning (queue.backpressure event)
      > 90%  -> low priority rejected
      100%   -> normal+ rejected, high only
    """

    WARNING_THRESHOLD = 0.70
    REJECT_LOW_THRESHOLD = 0.90

    def __init__(self):
        self._queues: Dict[str, JobQueue] = {}
        self._on_backpressure: Optional[Callable] = None
        self._warned: Dict[str, bool] = {}

    def register_queue(self, queue: JobQueue):
        self._queues[queue.name] = queue
        self._warned[queue.name] = False

    def set_callback(self, on_backpressure: Callable):
        """Set callback for backpressure events: fn(queue_name, usage_ratio)."""
        self._on_backpressure = on_backpressure

    def check_all(self):
        """Check all queues and trigger events if needed."""
        for name, queue in self._queues.items():
            ratio = queue.usage_ratio
            if ratio >= self.WARNING_THRESHOLD:
                if not self._warned.get(name):
                    self._warned[name] = True
                    logger.warning("Backpressure: queue '%s' at %.0f%%", name, ratio * 100)
                    if self._on_backpressure:
                        try:
                            self._on_backpressure(name, ratio)
                        except Exception:
                            pass
            else:
                self._warned[name] = False

    def get_status(self) -> dict:
        return {
            name: {
                'usage': round(q.usage_ratio, 2),
                'warned': self._warned.get(name, False),
            }
            for name, q in self._queues.items()
        }
