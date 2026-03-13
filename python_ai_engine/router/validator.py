"""
Payload validation.
Schema-based validation for service requests.
"""

import json
import logging
import os
from typing import Dict, Optional, Tuple

logger = logging.getLogger(__name__)


class SchemaEntry:
    """Minimal schema definition for a service."""

    def __init__(self, required_fields: list = None, field_types: dict = None,
                 max_payload_kb: int = 1024):
        self.required_fields = required_fields or []
        self.field_types = field_types or {}
        self.max_payload_kb = max_payload_kb


class Validator:
    """
    Validates incoming request payloads.
    Supports both inline schemas and JSON schema files.
    """

    def __init__(self, schemas_dir: str = None, default_max_kb: int = 1024):
        self._schemas: Dict[str, SchemaEntry] = {}
        self._schemas_dir = schemas_dir
        self._default_max_kb = default_max_kb
        self._json_schemas: Dict[str, dict] = {}
        if schemas_dir:
            self._load_json_schemas(schemas_dir)

    def register_schema(self, service: str, schema: SchemaEntry):
        self._schemas[service] = schema

    def validate(self, service: str, payload: dict) -> Tuple[bool, Optional[str]]:
        """
        Validate a request payload.
        Returns (is_valid, error_message).
        """
        if payload is None:
            payload = {}

        # Check payload size (approximate)
        try:
            size_kb = len(json.dumps(payload, default=str)) / 1024
            max_kb = self._default_max_kb
            schema = self._schemas.get(service)
            if schema:
                max_kb = schema.max_payload_kb
            if size_kb > max_kb:
                return False, f"Payload too large: {size_kb:.0f}KB > {max_kb}KB limit"
        except Exception:
            pass

        # Schema validation
        schema = self._schemas.get(service)
        if schema:
            for field in schema.required_fields:
                if field not in payload:
                    return False, f"Missing required field: '{field}'"

            for field, expected_type in schema.field_types.items():
                if field in payload:
                    val = payload[field]
                    if not isinstance(val, expected_type):
                        return False, (
                            f"Field '{field}' type mismatch: "
                            f"expected {expected_type.__name__}, "
                            f"got {type(val).__name__}"
                        )

        return True, None

    def _load_json_schemas(self, schemas_dir: str):
        """Load JSON schema files from directory."""
        if not os.path.isdir(schemas_dir):
            return
        for filename in os.listdir(schemas_dir):
            if not filename.endswith('.json'):
                continue
            try:
                path = os.path.join(schemas_dir, filename)
                with open(path, 'r', encoding='utf-8') as f:
                    self._json_schemas[filename] = json.load(f)
                logger.debug("Loaded schema: %s", filename)
            except Exception as e:
                logger.warning("Failed to load schema %s: %s", filename, e)
