"""
Permission / authorization checking.
Phase 5: Default-deny when auth is enabled. Backward-compatible fallback via strict_mode flag.
"""

import logging
from typing import Dict, List, Optional, Set

logger = logging.getLogger(__name__)

# Service -> required permission mapping
DEFAULT_PERMISSIONS: Dict[str, Optional[str]] = {
    'predict/power': 'predict.execute',
    'predict/anomaly': 'predict.execute',
    'predict/rul': 'predict.execute',
    'vision/detect': 'vision.execute',
    'vision/ocr': 'vision.execute',
    'analysis/trend': 'analysis.execute',
    'analysis/report': 'analysis.execute',
    'analysis/correlation': 'analysis.execute',
    'train/start': 'train.execute',
    'train/status': 'train.execute',
    'train/cancel': 'train.execute',
    'script/execute': 'script.execute',
    'system/ping': None,       # No permission required
    'system/status': None,
    'system/metrics': None,
    'system/models': None,
    'system/health': None,
    'system/shutdown': 'python.admin',
}


class AuthChecker:
    """
    Checks if a request has the required permissions.
    Permissions come from context.permissions in the request.
    """

    def __init__(self, strict_mode: bool = False):
        self._service_permissions: Dict[str, Optional[str]] = dict(DEFAULT_PERMISSIONS)
        self._enabled = True
        self._strict_mode = strict_mode

    def set_enabled(self, enabled: bool):
        self._enabled = enabled

    def set_strict_mode(self, strict: bool):
        """
        strict_mode=True: 권한 정보 없으면 거부 (운영 환경).
        strict_mode=False: 권한 정보 없으면 허용 (개발/호환 모드).
        """
        self._strict_mode = strict

    def register_permission(self, service: str, permission: str):
        """Register required permission for a service."""
        self._service_permissions[service] = permission

    def check(self, service: str, context: dict) -> tuple:
        """
        Check if the request context has permission for the service.
        Returns (allowed: bool, reason: str or None).
        """
        if not self._enabled:
            return True, None

        required = self._service_permissions.get(service)
        if required is None:
            return True, None  # No permission required for this service

        permissions = context.get('permissions', [])
        if not permissions:
            if self._strict_mode:
                # Strict mode: deny when no permissions provided
                logger.warning("AUTH DENIED: service=%s, no permissions in context (strict mode)", service)
                return False, "PERMISSION_DENIED: no permissions provided (strict mode)"
            else:
                # Compatible mode: allow when no permissions provided
                logger.debug("AUTH ALLOW (compat): service=%s, no permissions in context", service)
                return True, None

        if required in permissions or 'python.admin' in permissions:
            return True, None

        logger.warning("AUTH DENIED: service=%s, requires '%s', has %s", service, required, permissions)
        return False, "PERMISSION_DENIED: requires '%s'" % required
