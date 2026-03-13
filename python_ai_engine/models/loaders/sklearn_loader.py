"""
scikit-learn model loader (joblib/pickle).
"""

import logging
import os
from typing import Any, Tuple

from .base_loader import BaseModelLoader

logger = logging.getLogger(__name__)


class SklearnModelLoader(BaseModelLoader):
    """Load and run sklearn models via joblib."""

    def load(self, model_path: str, **kwargs) -> Tuple[Any, float]:
        try:
            import joblib
        except ImportError:
            raise ImportError("joblib is required: pip install joblib")

        model = joblib.load(model_path)
        size_mb = os.path.getsize(model_path) / (1024 * 1024)
        logger.info("sklearn model loaded: %s (%.1fMB)", model_path, size_mb)
        return model, size_mb

    def predict(self, model: Any, input_data: dict) -> dict:
        import numpy as np

        features = input_data.get('features') or input_data.get('X')
        if features is None:
            raise ValueError("Input must contain 'features' or 'X' key")

        X = np.array(features)
        if X.ndim == 1:
            X = X.reshape(1, -1)

        predictions = model.predict(X).tolist()
        result = {'predictions': predictions}

        if hasattr(model, 'predict_proba'):
            try:
                proba = model.predict_proba(X).tolist()
                result['probabilities'] = proba
            except Exception:
                pass

        return result

    def get_framework_name(self) -> str:
        return 'sklearn'
