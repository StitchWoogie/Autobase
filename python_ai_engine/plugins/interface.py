"""
Plugin interface (PluginBase).
All plugins must inherit from PluginBase and implement required methods.
"""

from abc import ABC, abstractmethod
from typing import Any, Dict, Optional


class PluginContext:
    """Context provided to plugins during initialization."""

    def __init__(self, engine_version: str, config: dict,
                 model_registry=None, audit_logger=None):
        self.engine_version = engine_version
        self.config = config
        self.model_registry = model_registry
        self.audit_logger = audit_logger


class ServiceRequest:
    """Incoming service request for a plugin."""

    __slots__ = ('service', 'payload', 'context', 'request_id', 'timeout_ms')

    def __init__(self, service: str, payload: dict, context: dict = None,
                 request_id: str = '', timeout_ms: int = 3000):
        self.service = service
        self.payload = payload or {}
        self.context = context or {}
        self.request_id = request_id
        self.timeout_ms = timeout_ms


class ServiceResponse:
    """Response from a plugin service execution."""

    def __init__(self, result: dict = None, error: str = None):
        self.result = result
        self.error = error
        self.is_success = error is None

    @staticmethod
    def ok(result: dict) -> 'ServiceResponse':
        return ServiceResponse(result=result)

    @staticmethod
    def fail(error: str) -> 'ServiceResponse':
        return ServiceResponse(error=error)


class PluginBase(ABC):
    """
    Plugin base class.
    Plugins provide service implementations loaded dynamically.
    """

    @abstractmethod
    def initialize(self, context: PluginContext) -> None:
        """Initialize plugin with context. Called once at load time."""
        ...

    @abstractmethod
    async def execute(self, request: ServiceRequest) -> ServiceResponse:
        """Execute a service request."""
        ...

    @abstractmethod
    def dispose(self) -> None:
        """Clean up plugin resources."""
        ...

    def get_info(self) -> dict:
        """Return plugin info for status queries."""
        return {
            'class': self.__class__.__name__,
        }
