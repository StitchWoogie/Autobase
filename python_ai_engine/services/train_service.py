"""
Training services: start, status, cancel (Phase 4).
Manages model training jobs.
"""

import asyncio
import logging
import time
import uuid
from enum import Enum
from typing import Dict, Optional

logger = logging.getLogger(__name__)


class TrainStatus(str, Enum):
    PENDING = 'pending'
    RUNNING = 'running'
    COMPLETED = 'completed'
    FAILED = 'failed'
    CANCELLED = 'cancelled'


class TrainJob:
    """A model training job."""

    def __init__(self, job_id: str, model_name: str, config: dict):
        self.job_id = job_id
        self.model_name = model_name
        self.config = config
        self.status = TrainStatus.PENDING
        self.progress = 0.0
        self.start_time = None
        self.end_time = None
        self.error = None
        self.result = None
        self._task: Optional[asyncio.Task] = None
        self._cancelled = False

    def to_dict(self) -> dict:
        d = {
            'jobId': self.job_id,
            'modelName': self.model_name,
            'status': self.status.value,
            'progress': round(self.progress, 1),
        }
        if self.start_time:
            d['startTime'] = self.start_time
            d['elapsedSec'] = round(time.time() - self.start_time, 1)
        if self.end_time:
            d['endTime'] = self.end_time
        if self.error:
            d['error'] = self.error
        if self.result:
            d['result'] = self.result
        return d


class TrainManager:
    """Manages training jobs."""

    def __init__(self, max_concurrent: int = 1):
        self._jobs: Dict[str, TrainJob] = {}
        self._max_concurrent = max_concurrent
        self._active_count = 0

    async def start_job(self, model_name: str, config: dict) -> dict:
        """Start a new training job."""
        if self._active_count >= self._max_concurrent:
            return {
                'error': f'Max concurrent training ({self._max_concurrent}) reached',
                'retryable': True,
            }

        job_id = str(uuid.uuid4())[:8]
        job = TrainJob(job_id, model_name, config)
        self._jobs[job_id] = job

        # Start training task
        job._task = asyncio.create_task(self._run_training(job))
        self._active_count += 1

        logger.info("Training started: %s for model %s", job_id, model_name)
        return job.to_dict()

    async def get_status(self, job_id: str = None) -> dict:
        """Get status of a specific job or all jobs."""
        if job_id:
            job = self._jobs.get(job_id)
            if job:
                return job.to_dict()
            return {'error': f'Job not found: {job_id}'}

        return {
            'active': self._active_count,
            'total': len(self._jobs),
            'jobs': [j.to_dict() for j in self._jobs.values()],
        }

    async def cancel_job(self, job_id: str) -> dict:
        """Cancel a running training job."""
        job = self._jobs.get(job_id)
        if not job:
            return {'error': f'Job not found: {job_id}'}
        if job.status != TrainStatus.RUNNING:
            return {'error': f'Job is not running: {job.status.value}'}

        job._cancelled = True
        if job._task:
            job._task.cancel()
        job.status = TrainStatus.CANCELLED
        job.end_time = time.time()
        self._active_count = max(0, self._active_count - 1)

        logger.info("Training cancelled: %s", job_id)
        return job.to_dict()

    async def _run_training(self, job: TrainJob):
        """Execute the training process (placeholder for real training)."""
        job.status = TrainStatus.RUNNING
        job.start_time = time.time()

        try:
            # Simulate training epochs
            epochs = job.config.get('epochs', 10)
            for epoch in range(epochs):
                if job._cancelled:
                    break
                await asyncio.sleep(1.0)  # Simulated work
                job.progress = ((epoch + 1) / epochs) * 100.0
                logger.debug("Training %s: epoch %d/%d", job.job_id, epoch + 1, epochs)

            if not job._cancelled:
                job.status = TrainStatus.COMPLETED
                job.result = {
                    'epochs_completed': epochs,
                    'final_loss': 0.05,  # Placeholder
                    'model_path': f'models/{job.model_name}/trained/',
                }
                logger.info("Training completed: %s", job.job_id)

        except asyncio.CancelledError:
            job.status = TrainStatus.CANCELLED
            logger.info("Training task cancelled: %s", job.job_id)
        except Exception as e:
            job.status = TrainStatus.FAILED
            job.error = str(e)
            logger.error("Training failed: %s - %s", job.job_id, e)
        finally:
            job.end_time = time.time()
            self._active_count = max(0, self._active_count - 1)


# Module-level singleton
_train_manager = TrainManager()


async def handle_train_start(payload: dict) -> dict:
    """train.start — start a training job."""
    payload = payload or {}
    model_name = payload.get('model', 'unnamed')
    config = payload.get('config', {})
    return await _train_manager.start_job(model_name, config)


async def handle_train_status(payload: dict) -> dict:
    """train.status — get training job status."""
    payload = payload or {}
    job_id = payload.get('jobId')
    return await _train_manager.get_status(job_id)


async def handle_train_cancel(payload: dict) -> dict:
    """train.cancel — cancel a training job."""
    payload = payload or {}
    job_id = payload.get('jobId', '')
    return await _train_manager.cancel_job(job_id)


def register(router):
    router.register('train/start', handle_train_start)
    router.register('train/status', handle_train_status)
    router.register('train/cancel', handle_train_cancel)
