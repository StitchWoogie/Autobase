"""
Base model loader interface.
"""

from abc import ABC, abstractmethod
from typing import Any, Tuple


class BaseModelLoader(ABC):
    """Abstract base for model loaders."""

    @abstractmethod
    def load(self, model_path: str, **kwargs) -> Tuple[Any, float]:
        """
        Load a model from disk.
        Returns (model_instance, size_mb).
        """
        ...

    @abstractmethod
    def predict(self, model: Any, input_data: dict) -> dict:
        """Run inference on loaded model."""
        ...

    @abstractmethod
    def get_framework_name(self) -> str:
        """Return framework identifier (e.g., 'onnx', 'sklearn')."""
        ...

    def warmup(self, model: Any):
        """Optional warm-up inference to eliminate first-call latency."""
        pass
