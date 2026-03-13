"""
Large payload handler (Phase 4).
Payloads >= 64KB use file reference instead of inline socket transmission.
"""

import json
import logging
import os
import tempfile
import time
import uuid
from pathlib import Path
from typing import Optional, Tuple

logger = logging.getLogger(__name__)

THRESHOLD_KB = 64
SHARED_DIR = None


def initialize(shared_dir: str = None):
    """Initialize the large payload handler with a shared directory."""
    global SHARED_DIR
    SHARED_DIR = shared_dir or os.path.join(tempfile.gettempdir(), 'python_ai_shared')
    os.makedirs(SHARED_DIR, exist_ok=True)
    logger.info("Large payload handler initialized: %s", SHARED_DIR)


def should_use_file(payload: dict) -> bool:
    """Check if payload exceeds inline threshold."""
    try:
        size = len(json.dumps(payload, default=str).encode('utf-8'))
        return size >= THRESHOLD_KB * 1024
    except Exception:
        return False


def save_to_file(data: dict, prefix: str = 'payload') -> str:
    """Save data to a temporary file and return the path."""
    if not SHARED_DIR:
        initialize()
    filename = f"{prefix}_{uuid.uuid4().hex[:8]}_{int(time.time())}.json"
    path = os.path.join(SHARED_DIR, filename)
    with open(path, 'w', encoding='utf-8') as f:
        json.dump(data, f, ensure_ascii=False, default=str)
    logger.debug("Large payload saved: %s", path)
    return path


def load_from_file(path: str) -> Optional[dict]:
    """Load data from a file reference."""
    if not os.path.exists(path):
        logger.warning("Payload file not found: %s", path)
        return None
    try:
        with open(path, 'r', encoding='utf-8') as f:
            return json.load(f)
    except Exception as e:
        logger.error("Failed to load payload from %s: %s", path, e)
        return None


def resolve_payload(payload: dict) -> dict:
    """
    Resolve payload — if it contains a file reference, load from file.
    Supports: payloadRef (JSON file) and imageRef (binary file).
    """
    if payload is None:
        return {}

    # Check for payload file reference
    ref = payload.get('payloadRef')
    if ref:
        loaded = load_from_file(ref)
        if loaded:
            return loaded
        logger.warning("Could not resolve payloadRef: %s", ref)

    return payload


def cleanup_old_files(max_age_sec: int = 3600):
    """Remove old temporary payload files."""
    if not SHARED_DIR or not os.path.exists(SHARED_DIR):
        return
    now = time.time()
    count = 0
    for f in os.listdir(SHARED_DIR):
        path = os.path.join(SHARED_DIR, f)
        if os.path.isfile(path):
            age = now - os.path.getmtime(path)
            if age > max_age_sec:
                try:
                    os.remove(path)
                    count += 1
                except Exception:
                    pass
    if count:
        logger.info("Cleaned up %d old payload files", count)
