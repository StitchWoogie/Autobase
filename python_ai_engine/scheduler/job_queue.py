"""
Priority job queue with backpressure support.
"""

import asyncio
import logging
import time
from dataclasses import dataclass, field
from enum import IntEnum
from typing import Any, Dict, Optional

logger = logging.getLogger(__name__)


class Priority(IntEnum):
    HIGH = 1
    NORMAL = 5
    LOW = 9


@dataclass(order=True)
class Job:
    priority: int
    timestamp: float = field(compare=True)
    request_id: str = field(compare=False)
    service: str = field(compare=False)
    payload: Any = field(compare=False, default=None)
    timeout_ms: int = field(compare=False, default=3000)
    context: dict = field(compare=False, default_factory=dict)
    future: asyncio.Future = field(compare=False, default=None, repr=False)


class JobQueue:
    """
    Named priority queue with backpressure policy.
    """

    def __init__(self, name: str, max_size: int = 100):
        self.name = name
        self.max_size = max_size
        self._queue: asyncio.PriorityQueue = asyncio.PriorityQueue(maxsize=max_size)
        self._total_enqueued = 0
        self._total_rejected = 0

    @property
    def size(self) -> int:
        return self._queue.qsize()

    @property
    def usage_ratio(self) -> float:
        return self.size / self.max_size if self.max_size > 0 else 0.0

    def can_accept(self, priority: int) -> bool:
        """Check if queue can accept a job with given priority."""
        ratio = self.usage_ratio
        if ratio >= 1.0:
            return priority <= Priority.HIGH
        if ratio >= 0.9:
            return priority <= Priority.NORMAL
        return True

    async def enqueue(self, job: Job) -> bool:
        """Enqueue a job. Returns False if rejected by backpressure."""
        if not self.can_accept(job.priority):
            self._total_rejected += 1
            logger.warning("Queue '%s' backpressure: rejected %s (usage: %.0f%%)",
                           self.name, job.service, self.usage_ratio * 100)
            return False
        try:
            self._queue.put_nowait(job)
            self._total_enqueued += 1
            return True
        except asyncio.QueueFull:
            self._total_rejected += 1
            return False

    async def dequeue(self, timeout: float = None) -> Optional[Job]:
        """Get next job from queue."""
        try:
            if timeout:
                return await asyncio.wait_for(self._queue.get(), timeout)
            return await self._queue.get()
        except asyncio.TimeoutError:
            return None

    def get_stats(self) -> dict:
        return {
            'name': self.name,
            'size': self.size,
            'maxSize': self.max_size,
            'usage': round(self.usage_ratio, 2),
            'totalEnqueued': self._total_enqueued,
            'totalRejected': self._total_rejected,
        }
