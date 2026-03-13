"""
Prediction services (Phase 5: Real model inference + statistics fallback).

Three-tier prediction strategy:
1. Registry model: ONNX / sklearn / custom model from models/ directory
2. Statistics-based: OLS linear regression with r-squared confidence
3. Fallback: current value passthrough with confidence=0.0
"""

import logging
import time
from typing import Optional

logger = logging.getLogger(__name__)

# Module-level references (set by register())
_model_registry = None
_model_cache = None
_loaders = {}  # framework_name -> loader module


def _init_loaders():
    """Dynamically discover available model framework loaders."""
    global _loaders

    # ONNX Runtime
    try:
        import onnxruntime
        _loaders['onnx'] = onnxruntime
        logger.info("ONNX Runtime available (version %s)", onnxruntime.__version__)
    except ImportError:
        logger.debug("ONNX Runtime not installed — ONNX models will be skipped")

    # scikit-learn (joblib)
    try:
        import joblib
        import sklearn
        _loaders['sklearn'] = joblib
        logger.info("scikit-learn available (version %s)", sklearn.__version__)
    except ImportError:
        logger.debug("scikit-learn not installed — sklearn models will be skipped")


def _load_model(model_name: str):
    """
    Load a model from registry + cache.
    Returns (model_obj, framework) or (None, None).
    """
    if _model_registry is None:
        return None, None

    # Check registry for model info
    model_info = _model_registry.get_model(model_name)
    if model_info is None:
        return None, None

    framework = model_info.get('framework', 'unknown')
    model_path = model_info.get('path', '')

    # Check cache first
    if _model_cache is not None:
        cached = _model_cache.get(model_name)
        if cached is not None:
            return cached, framework

    # Load based on framework
    model_obj = None

    if framework == 'onnx' and 'onnx' in _loaders:
        try:
            ort = _loaders['onnx']
            sess = ort.InferenceSession(model_path)
            model_obj = sess
            logger.info("Loaded ONNX model: %s", model_name)
        except Exception as e:
            logger.error("Failed to load ONNX model '%s': %s", model_name, e)

    elif framework == 'sklearn' and 'sklearn' in _loaders:
        try:
            joblib = _loaders['sklearn']
            model_obj = joblib.load(model_path)
            logger.info("Loaded sklearn model: %s", model_name)
        except Exception as e:
            logger.error("Failed to load sklearn model '%s': %s", model_name, e)

    elif framework == 'custom':
        # Custom models should provide a predict() method
        try:
            import importlib.util
            spec = importlib.util.spec_from_file_location(model_name, model_path)
            if spec and spec.loader:
                mod = importlib.util.module_from_spec(spec)
                spec.loader.exec_module(mod)
                if hasattr(mod, 'load_model'):
                    model_obj = mod.load_model()
                    logger.info("Loaded custom model: %s", model_name)
        except Exception as e:
            logger.error("Failed to load custom model '%s': %s", model_name, e)

    # Cache the loaded model
    if model_obj is not None and _model_cache is not None:
        _model_cache.put(model_name, model_obj)

    return model_obj, framework


def _predict_with_model(model_obj, framework: str, features: list) -> Optional[float]:
    """Run inference on a loaded model. Returns predicted value or None."""
    try:
        if framework == 'onnx':
            import numpy as np
            input_name = model_obj.get_inputs()[0].name
            input_data = np.array([features], dtype=np.float32)
            outputs = model_obj.run(None, {input_name: input_data})
            result = outputs[0]
            if hasattr(result, 'item'):
                return float(result.item())
            return float(result[0])

        elif framework == 'sklearn':
            import numpy as np
            input_data = np.array([features])
            result = model_obj.predict(input_data)
            return float(result[0])

        elif framework == 'custom':
            if hasattr(model_obj, 'predict'):
                result = model_obj.predict(features)
                return float(result)

    except Exception as e:
        logger.warning("Model inference failed (%s): %s", framework, e)

    return None


def _predict_statistics(current_kw: float, history: list) -> dict:
    """
    Statistics-based prediction using OLS linear regression.
    Returns {predicted_kw, confidence, method}.
    """
    if not history or len(history) < 3:
        return None

    try:
        n = len(history)
        x = list(range(n))
        y = list(history)

        # OLS: y = a + b*x
        x_mean = sum(x) / n
        y_mean = sum(y) / n

        ss_xy = sum((x[i] - x_mean) * (y[i] - y_mean) for i in range(n))
        ss_xx = sum((x[i] - x_mean) ** 2 for i in range(n))

        if ss_xx == 0:
            return None

        b = ss_xy / ss_xx
        a = y_mean - b * x_mean

        # Predict next point (x = n)
        predicted = a + b * n

        # R-squared for confidence
        ss_yy = sum((y[i] - y_mean) ** 2 for i in range(n))
        if ss_yy == 0:
            r_squared = 1.0
        else:
            ss_res = sum((y[i] - (a + b * x[i])) ** 2 for i in range(n))
            r_squared = max(0.0, 1.0 - ss_res / ss_yy)

        return {
            'predicted_kw': round(predicted, 2),
            'confidence': round(r_squared, 4),
            'method': 'statistics',
            'detail': 'OLS linear regression (n=%d, R2=%.4f)' % (n, r_squared),
        }

    except Exception as e:
        logger.warning("Statistics prediction failed: %s", e)
        return None


async def handle_predict_power(payload):
    """
    predict.power — power load prediction.

    Three-tier strategy:
    1. Registry model (ONNX/sklearn/custom) if available
    2. Statistics-based OLS linear regression if history provided
    3. Fallback: current value passthrough

    Input: {
        current_kw: float,        # Current power (kW)
        history: [float],         # Historical values (optional)
        model_name: str,          # Model name in registry (optional, default: "power_predict")
        horizon_sec: int,         # Prediction horizon in seconds (optional, default: 60)
    }

    Output: {
        predicted_kw: float,
        confidence: float,        # 0.0~1.0 (0=no confidence, 1=perfect)
        method: str,              # 'model' | 'statistics' | 'fallback'
        model: str,               # Model name or method description
        horizon_sec: int,
    }
    """
    payload = payload or {}
    current_kw = payload.get('current_kw', 0.0)
    history = payload.get('history', [])
    model_name = payload.get('model_name', 'power_predict')
    horizon_sec = payload.get('horizon_sec', 60)

    start_time = time.monotonic()

    # Tier 1: Registry model inference
    model_obj, framework = _load_model(model_name)
    if model_obj is not None:
        features = [current_kw]
        if history:
            features.extend(history[-10:])  # Use last 10 history points as features

        predicted = _predict_with_model(model_obj, framework, features)
        if predicted is not None:
            duration_ms = (time.monotonic() - start_time) * 1000
            return {
                'predicted_kw': round(predicted, 2),
                'confidence': 0.9,
                'method': 'model',
                'model': '%s (%s)' % (model_name, framework),
                'horizon_sec': horizon_sec,
                'inference_ms': round(duration_ms, 1),
            }

    # Tier 2: Statistics-based prediction (needs history)
    stats_result = _predict_statistics(current_kw, history)
    if stats_result is not None:
        stats_result['horizon_sec'] = horizon_sec
        stats_result['model'] = 'ols_linear'
        duration_ms = (time.monotonic() - start_time) * 1000
        stats_result['inference_ms'] = round(duration_ms, 1)
        return stats_result

    # Tier 3: Fallback — return current value with zero confidence
    duration_ms = (time.monotonic() - start_time) * 1000
    return {
        'predicted_kw': round(current_kw, 2),
        'confidence': 0.0,
        'method': 'fallback',
        'model': 'passthrough (no model or history)',
        'horizon_sec': horizon_sec,
        'inference_ms': round(duration_ms, 1),
    }


def register(router, model_registry=None, model_cache=None):
    """Register prediction services. Optionally inject model registry and cache."""
    global _model_registry, _model_cache

    if model_registry is not None:
        _model_registry = model_registry
    if model_cache is not None:
        _model_cache = model_cache

    _init_loaders()

    router.register('predict/power', handle_predict_power)
    logger.info("Prediction service registered (loaders: %s)",
                list(_loaders.keys()) if _loaders else 'none')
