"""
Vision services: detect, ocr (Phase 4).
"""

import base64
import logging
import os
import time
from typing import Optional

logger = logging.getLogger(__name__)


async def handle_vision_detect(payload: dict) -> dict:
    """
    vision.detect — object detection.
    Input: {image: base64_str} or {imageRef: file_path}
    Output: {detections: [{class, confidence, bbox}]}
    """
    payload = payload or {}
    image_data = _resolve_image(payload)

    if image_data is None:
        return {'error': 'No image provided', 'detections': []}

    try:
        detections = await _run_detection(image_data, payload)
        return {
            'detections': detections,
            'count': len(detections),
            'model': payload.get('model', 'default'),
        }
    except ImportError as e:
        return {
            'detections': [],
            'error': f'Vision dependency not installed: {e}',
            'note': 'Install: pip install ultralytics opencv-python',
        }
    except Exception as e:
        logger.error("Vision detect error: %s", e)
        return {'detections': [], 'error': str(e)}


async def handle_vision_ocr(payload: dict) -> dict:
    """
    vision.ocr — text recognition.
    Input: {image: base64_str} or {imageRef: file_path}
    Output: {texts: [{text, confidence, bbox}]}
    """
    payload = payload or {}
    image_data = _resolve_image(payload)

    if image_data is None:
        return {'error': 'No image provided', 'texts': []}

    try:
        texts = await _run_ocr(image_data, payload)
        return {
            'texts': texts,
            'count': len(texts),
        }
    except ImportError as e:
        return {
            'texts': [],
            'error': f'OCR dependency not installed: {e}',
            'note': 'Install: pip install easyocr',
        }
    except Exception as e:
        logger.error("Vision OCR error: %s", e)
        return {'texts': [], 'error': str(e)}


def _resolve_image(payload: dict) -> Optional[bytes]:
    """Resolve image from base64 or file reference."""
    # Base64 inline
    image_b64 = payload.get('image')
    if image_b64:
        try:
            return base64.b64decode(image_b64)
        except Exception:
            return None

    # File reference (large payload strategy)
    image_ref = payload.get('imageRef')
    if image_ref and os.path.exists(image_ref):
        with open(image_ref, 'rb') as f:
            return f.read()

    return None


async def _run_detection(image_data: bytes, payload: dict) -> list:
    """Run YOLO detection. Falls back to stub if ultralytics not available."""
    try:
        from ultralytics import YOLO
        import numpy as np
        import cv2

        model_path = payload.get('model_path', 'yolov8n.pt')
        conf_threshold = payload.get('confidence', 0.5)

        # Decode image
        nparr = np.frombuffer(image_data, np.uint8)
        img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

        model = YOLO(model_path)
        results = model(img, conf=conf_threshold)

        detections = []
        for r in results:
            for box in r.boxes:
                detections.append({
                    'class': r.names[int(box.cls[0])],
                    'confidence': round(float(box.conf[0]), 3),
                    'bbox': [round(float(v), 1) for v in box.xyxy[0].tolist()],
                })
        return detections

    except ImportError:
        raise
    except Exception as e:
        logger.error("YOLO detection error: %s", e)
        raise


async def _run_ocr(image_data: bytes, payload: dict) -> list:
    """Run OCR. Falls back to stub if easyocr not available."""
    try:
        import easyocr
        import numpy as np
        import cv2

        lang = payload.get('lang', ['en'])
        if isinstance(lang, str):
            lang = [lang]

        nparr = np.frombuffer(image_data, np.uint8)
        img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

        reader = easyocr.Reader(lang)
        results = reader.readtext(img)

        texts = []
        for (bbox, text, conf) in results:
            texts.append({
                'text': text,
                'confidence': round(float(conf), 3),
                'bbox': [[int(p[0]), int(p[1])] for p in bbox],
            })
        return texts

    except ImportError:
        raise
    except Exception as e:
        logger.error("OCR error: %s", e)
        raise


def register(router):
    router.register('vision/detect', handle_vision_detect)
    router.register('vision/ocr', handle_vision_ocr)
