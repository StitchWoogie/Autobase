"""
ONNX Runtime model loader.
"""

import logging
import os
from typing import Any, Tuple

import numpy as np

from .base_loader import BaseModelLoader

logger = logging.getLogger(__name__)


class OnnxModelLoader(BaseModelLoader):
    """Load and run ONNX models via onnxruntime."""

    def load(self, model_path: str, **kwargs) -> Tuple[Any, float]:
        try:
            import onnxruntime as ort
        except ImportError:
            raise ImportError("onnxruntime is required: pip install onnxruntime")

        opts = ort.SessionOptions()
        opts.graph_optimization_level = ort.GraphOptimizationLevel.ORT_ENABLE_ALL
        opts.intra_op_num_threads = kwargs.get('threads', 2)

        session = ort.InferenceSession(model_path, sess_options=opts)
        size_mb = os.path.getsize(model_path) / (1024 * 1024)
        logger.info("ONNX model loaded: %s (%.1fMB)", model_path, size_mb)
        return session, size_mb

    def predict(self, model: Any, input_data: dict) -> dict:
        input_names = [inp.name for inp in model.get_inputs()]
        feeds = {}
        for name in input_names:
            if name in input_data:
                val = input_data[name]
                if not isinstance(val, np.ndarray):
                    val = np.array(val, dtype=np.float32)
                feeds[name] = val

        outputs = model.run(None, feeds)
        output_names = [out.name for out in model.get_outputs()]
        result = {}
        for name, val in zip(output_names, outputs):
            if isinstance(val, np.ndarray):
                result[name] = val.tolist()
            else:
                result[name] = val
        return result

    def get_framework_name(self) -> str:
        return 'onnx'

    def warmup(self, model: Any):
        """Run a dummy inference to warm up."""
        try:
            inputs = model.get_inputs()
            feeds = {}
            for inp in inputs:
                shape = [d if isinstance(d, int) and d > 0 else 1 for d in inp.shape]
                feeds[inp.name] = np.zeros(shape, dtype=np.float32)
            model.run(None, feeds)
            logger.debug("ONNX warmup completed")
        except Exception as e:
            logger.warning("ONNX warmup failed: %s", e)
