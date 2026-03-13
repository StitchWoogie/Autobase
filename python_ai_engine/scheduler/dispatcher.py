"""
Job dispatcher — routes jobs from queues to workers.
"""

import asyncio
import logging
import time
from typing import Dict, Optional

from .job_queue import Job, JobQueue

logger = logging.getLogger(__name__)


class Dispatcher:
    """
    Dispatches jobs from named queues to worker pool.
    Runs one consumer task per queue.
    """

    def __init__(self):
        self._queues: Dict[str, JobQueue] = {}
        self._worker_pool = None
        self._tasks = []
        self._running = False

    def set_worker_pool(self, pool):
        self._worker_pool = pool

    def register_queue(self, queue: JobQueue):
        self._queues[queue.name] = queue

    async def start(self):
        self._running = True
        for name, queue in self._queues.items():
            task = asyncio.create_task(self._consume_loop(name, queue))
            self._tasks.append(task)
        logger.info("Dispatcher started for queues: %s", list(self._queues.keys()))

    async def stop(self):
        self._running = False
        for task in self._tasks:
            task.cancel()
        for task in self._tasks:
            try:
                await task
            except asyncio.CancelledError:
                pass
        self._tasks.clear()
        logger.info("Dispatcher stopped")

    async def _consume_loop(self, queue_name: str, queue: JobQueue):
        while self._running:
            try:
                job = await queue.dequeue(timeout=1.0)
                if job is None:
                    continue
                asyncio.create_task(self._execute_job(queue_name, job))
            except asyncio.CancelledError:
                break
            except Exception as e:
                logger.error("Dispatcher consume error [%s]: %s", queue_name, e)
                await asyncio.sleep(0.5)

    async def _execute_job(self, queue_name: str, job: Job):
        """Execute a single job via worker pool."""
        start = time.monotonic()
        try:
            if self._worker_pool is None:
                raise RuntimeError("No worker pool configured")

            result = await asyncio.wait_for(
                self._worker_pool.execute(job.service, job.payload, job.context),
                timeout=job.timeout_ms / 1000.0
            )

            duration_ms = (time.monotonic() - start) * 1000
            if job.future and not job.future.done():
                job.future.set_result({
                    'Ok': True,
                    'Result': result,
                    'durationMs': round(duration_ms, 1),
                })

        except asyncio.TimeoutError:
            duration_ms = (time.monotonic() - start) * 1000
            if job.future and not job.future.done():
                job.future.set_result({
                    'Ok': False,
                    'Error': f'TIMEOUT: {job.service} exceeded {job.timeout_ms}ms',
                    'durationMs': round(duration_ms, 1),
                })

        except Exception as e:
            duration_ms = (time.monotonic() - start) * 1000
            if job.future and not job.future.done():
                job.future.set_result({
                    'Ok': False,
                    'Error': f'WORKER_ERROR: {str(e)}',
                    'durationMs': round(duration_ms, 1),
                })
