"""
Worker pool management.
Manages pools of workers for different service types.
"""

import asyncio
import logging
import time
from concurrent.futures import ProcessPoolExecutor
from typing import Any, Callable, Dict, List, Optional

logger = logging.getLogger(__name__)


def _run_in_process(handler_func, service: str, payload: dict, context: dict):
    """Top-level function for ProcessPoolExecutor (must be picklable)."""
    import asyncio
    loop = asyncio.new_event_loop()
    try:
        return loop.run_until_complete(handler_func(payload))
    finally:
        loop.close()


class WorkerPool:
    """
    Manages worker pools for Realtime, Batch, and Script workloads.
    Uses ProcessPoolExecutor for CPU-intensive tasks,
    falls back to asyncio for lightweight tasks.
    """

    def __init__(self, realtime_workers: int = 2, batch_workers: int = 1,
                 script_workers: int = 1):
        self._rt_count = realtime_workers
        self._batch_count = batch_workers
        self._script_count = script_workers

        self._rt_pool: Optional[ProcessPoolExecutor] = None
        self._batch_pool: Optional[ProcessPoolExecutor] = None

        # Service -> handler mapping (async callables)
        self._handlers: Dict[str, Callable] = {}
        # Service -> pool type mapping
        self._service_pool: Dict[str, str] = {}

        self._active_tasks = {'realtime': 0, 'batch': 0, 'script': 0}
        self._total_executed = {'realtime': 0, 'batch': 0, 'script': 0}
        self._lock = asyncio.Lock()

    def register_handler(self, service: str, handler: Callable, pool_type: str = 'realtime'):
        """Register a service handler and assign to a pool type."""
        self._handlers[service] = handler
        self._service_pool[service] = pool_type

    async def start(self):
        """Start worker pools."""
        # For Phase 1-3, use asyncio directly instead of ProcessPoolExecutor
        # ProcessPoolExecutor requires picklable functions which limits flexibility
        logger.info("Worker pool started: RT=%d, Batch=%d, Script=%d",
                     self._rt_count, self._batch_count, self._script_count)

    async def stop(self):
        """Stop worker pools."""
        if self._rt_pool:
            self._rt_pool.shutdown(wait=False)
        if self._batch_pool:
            self._batch_pool.shutdown(wait=False)
        logger.info("Worker pools stopped")

    async def execute(self, service: str, payload: dict, context: dict = None) -> dict:
        """Execute a service request using the appropriate handler."""
        handler = self._handlers.get(service)
        if handler is None:
            raise ValueError(f"No handler registered for service: {service}")

        pool_type = self._service_pool.get(service, 'realtime')

        async with self._lock:
            self._active_tasks[pool_type] = self._active_tasks.get(pool_type, 0) + 1

        try:
            result = await handler(payload)
            async with self._lock:
                self._total_executed[pool_type] = self._total_executed.get(pool_type, 0) + 1
            return result
        finally:
            async with self._lock:
                self._active_tasks[pool_type] = max(0, self._active_tasks.get(pool_type, 0) - 1)

    def has_handler(self, service: str) -> bool:
        return service in self._handlers

    def get_stats(self) -> dict:
        return {
            'realtime': {
                'active': self._active_tasks.get('realtime', 0),
                'total': self._rt_count,
                'executed': self._total_executed.get('realtime', 0),
            },
            'batch': {
                'active': self._active_tasks.get('batch', 0),
                'total': self._batch_count,
                'executed': self._total_executed.get('batch', 0),
            },
            'script': {
                'active': self._active_tasks.get('script', 0),
                'total': self._script_count,
                'executed': self._total_executed.get('script', 0),
            },
        }
