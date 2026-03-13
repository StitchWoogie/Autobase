"""
Service router (Phase 2 enhanced).
Routes requests with validation, auth, metrics, audit.
"""

import logging
import time
import traceback
from typing import Optional

from .validator import Validator
from .auth import AuthChecker

logger = logging.getLogger(__name__)


class Router:
    """
    Enhanced service router.
    Pipeline: validate → auth → execute → metrics → audit.
    """

    def __init__(self, validator: Validator = None, auth: AuthChecker = None,
                 metrics=None, audit_logger=None):
        self._handlers = {}
        self._validator = validator or Validator()
        self._auth = auth or AuthChecker()
        self._metrics = metrics
        self._audit_logger = audit_logger

    def register(self, route: str, handler):
        """Register an async handler for a service route."""
        self._handlers[route] = handler
        logger.debug("Registered service: %s", route)

    async def route(self, msg: dict) -> dict:
        """Route a request through the full pipeline."""
        request_id = msg.get('Id', '')
        service = msg.get('Service', '')
        payload = msg.get('Payload') or {}
        context = msg.get('context', {})
        start_time = time.monotonic()

        # 1. Service lookup
        handler = self._handlers.get(service)
        if handler is None:
            self._record_metrics(service, 0, False, 'SERVICE_NOT_FOUND')
            logger.warning("Unknown service: %s", service)
            return self._error_response(request_id, f'SERVICE_NOT_FOUND: {service}')

        # 2. Payload validation
        valid, err = self._validator.validate(service, payload)
        if not valid:
            self._record_metrics(service, 0, False, 'VALIDATION_ERROR')
            return self._error_response(request_id, f'VALIDATION_ERROR: {err}')

        # 3. Auth check
        allowed, reason = self._auth.check(service, context)
        if not allowed:
            self._record_metrics(service, 0, False, 'PERMISSION_DENIED')
            if self._audit_logger:
                self._audit_logger.log_permission_denied(
                    service, context.get('user', ''), reason or '')
            return self._error_response(request_id, reason or 'PERMISSION_DENIED')

        # 4. Execute
        try:
            result = await handler(payload)
            duration_ms = (time.monotonic() - start_time) * 1000
            self._record_metrics(service, duration_ms, True)
            self._record_audit(request_id, service, 'ok', duration_ms, context)

            return {
                'Id': request_id,
                'Ok': True,
                'Result': result,
                'durationMs': round(duration_ms, 1),
            }

        except Exception as e:
            duration_ms = (time.monotonic() - start_time) * 1000
            error_msg = f'WORKER_ERROR: {str(e)}'
            self._record_metrics(service, duration_ms, False, error_msg)
            self._record_audit(request_id, service, 'error', duration_ms, context, str(e))
            logger.error("Handler error for %s: %s", service, traceback.format_exc())
            return self._error_response(request_id, error_msg, duration_ms)

    def list_services(self) -> list:
        """Return list of registered service names."""
        return sorted(self._handlers.keys())

    def has_service(self, service: str) -> bool:
        return service in self._handlers

    def _error_response(self, request_id: str, error: str,
                        duration_ms: float = 0) -> dict:
        resp = {'Id': request_id, 'Ok': False, 'Error': error}
        if duration_ms > 0:
            resp['durationMs'] = round(duration_ms, 1)
        return resp

    def _record_metrics(self, service: str, duration_ms: float,
                        is_success: bool, error: str = None):
        if self._metrics:
            try:
                self._metrics.record(service, duration_ms, is_success, error)
            except Exception:
                pass

    def _record_audit(self, request_id: str, service: str, status: str,
                      duration_ms: float, context: dict, error: str = None):
        if self._audit_logger:
            try:
                self._audit_logger.log_request(
                    request_id, service, status, duration_ms,
                    user=context.get('user'),
                    station=context.get('station'),
                    error_code=error,
                )
            except Exception:
                pass
