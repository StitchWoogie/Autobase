"""
Base worker interface.
All workers run as subprocess for GIL avoidance and isolation.
"""

from abc import ABC, abstractmethod
from typing import Any, Dict, Optional


class BaseWorker(ABC):
    """
    Abstract worker base.
    Workers execute service requests in separate processes.
    """

    def __init__(self, worker_id: str, worker_type: str):
        self.worker_id = worker_id
        self.worker_type = worker_type
        self._initialized = False

    @abstractmethod
    async def initialize(self):
        """Initialize worker resources (model loading, etc.)."""
        ...

    @abstractmethod
    async def execute(self, service: str, payload: dict, context: dict = None) -> dict:
        """Execute a service request and return result dict."""
        ...

    @abstractmethod
    async def shutdown(self):
        """Clean up worker resources."""
        ...

    @property
    def is_ready(self) -> bool:
        return self._initialized

    def get_status(self) -> dict:
        return {
            'worker_id': self.worker_id,
            'worker_type': self.worker_type,
            'ready': self._initialized,
        }
