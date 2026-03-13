"""
Script sandbox runner.
Executes user Python scripts in an isolated environment.
Uses run_in_executor with restricted namespace + safe builtins for isolation.

Security layers:
1. Safe builtins: dangerous builtins (exec, eval, open, etc.) removed
2. Import whitelist: only allowed modules can be imported
3. Code validation: dangerous attribute access patterns blocked
4. Resource limiter: timeout + memory cap via threading monitor
5. Restricted globals: only SCADA API functions injected
"""

import asyncio
import logging
import re
import time
import traceback

from .api_bridge import ScadaApi
from .policy import SandboxPolicy
from .resource_limiter import ResourceLimiter

logger = logging.getLogger(__name__)

# Maximum allowed script code size (bytes)
MAX_CODE_SIZE = 256 * 1024  # 256 KB

# Patterns that indicate sandbox escape attempts
_DANGEROUS_PATTERNS = [
    re.compile(r'__subclasses__'),
    re.compile(r'__globals__'),
    re.compile(r'__code__'),
    re.compile(r'__class__'),
    re.compile(r'__bases__'),
    re.compile(r'__mro__'),
    re.compile(r'__qualname__'),
    re.compile(r'__reduce__'),
    re.compile(r'__import__'),
    re.compile(r'\bos\s*\.\s*system'),
    re.compile(r'\bos\s*\.\s*popen'),
    re.compile(r'\bsubprocess'),
    re.compile(r'\bctypes\b'),
    re.compile(r'\bsys\s*\.\s*modules'),
    re.compile(r'\bsys\s*\.\s*path'),
]


def _validate_code(code: str) -> str:
    """
    Validate user code for dangerous patterns before execution.
    Returns error message if dangerous pattern found, None if safe.
    """
    if len(code.encode('utf-8', errors='replace')) > MAX_CODE_SIZE:
        return 'SCRIPT_TOO_LARGE: max %d KB' % (MAX_CODE_SIZE // 1024)

    for pattern in _DANGEROUS_PATTERNS:
        match = pattern.search(code)
        if match:
            return 'BLOCKED_PATTERN: "%s" is not allowed in sandbox scripts' % match.group()

    return None


class SandboxRunner:
    """
    Runs user scripts in isolated restricted namespace.

    Execution flow:
    1. Validate code (size + pattern check)
    2. Extract SCADA snapshots from payload
    3. Build ScadaApi with tag_snapshot + history_snapshot
    4. Execute in thread pool with restricted globals
    5. Collect results (output, logs, pending_writes)
    """

    def __init__(self, policy: SandboxPolicy = None, audit_logger=None):
        self._policy = policy or SandboxPolicy()
        self._audit_logger = audit_logger

    async def execute(self, code: str, payload: dict = None,
                      context: dict = None) -> dict:
        """
        Execute user script code safely.
        Returns result dict with script output.
        """
        context = context or {}
        payload = payload or {}

        # Pre-execution code validation
        validation_error = _validate_code(code)
        if validation_error:
            return {
                'status': 'error',
                'error': validation_error,
                'duration_ms': 0,
            }

        tag_write_allowed = 'script.tag_write' in context.get('permissions', [])

        # Extract preloaded snapshots from payload
        tag_snapshot = payload.pop('_tag_snapshot', {})
        history_snapshot = payload.pop('_history_snapshot', {})

        # Build SCADA API with snapshots
        api = ScadaApi(
            request_id=context.get('request_id', ''),
            user=context.get('user', ''),
            tag_write_allowed=tag_write_allowed,
            audit_logger=self._audit_logger,
            tag_snapshot=tag_snapshot,
            history_snapshot=history_snapshot,
        )

        # Inject payload into API namespace
        namespace = api.get_namespace()
        if payload:
            namespace['payload'] = payload

        start_time = time.monotonic()

        try:
            # Execute in thread with restricted globals
            loop = asyncio.get_event_loop()
            result = await asyncio.wait_for(
                loop.run_in_executor(
                    None,
                    _execute_restricted,
                    code, self._policy, namespace
                ),
                timeout=self._policy.timeout_sec + 2  # Extra buffer
            )

            duration_ms = (time.monotonic() - start_time) * 1000

            if result.get('status') == 'error':
                return {
                    'status': 'error',
                    'error': result.get('error', 'Unknown error'),
                    'traceback': result.get('traceback'),
                    'duration_ms': round(duration_ms, 1),
                    'logs': api.get_execution_result().get('logs', []),
                }

            exec_result = api.get_execution_result()
            response = {
                'status': 'ok',
                'result': exec_result.get('result'),
                'output': exec_result.get('output', []),
                'logs': exec_result.get('logs', []),
                'duration_ms': round(duration_ms, 1),
            }
            # Include pending tag writes
            pending = exec_result.get('_pending_writes')
            if pending:
                response['_pending_writes'] = pending
            return response

        except asyncio.TimeoutError:
            duration_ms = (time.monotonic() - start_time) * 1000
            return {
                'status': 'error',
                'error': 'Script timeout (%ss)' % self._policy.timeout_sec,
                'duration_ms': round(duration_ms, 1),
            }
        except Exception as e:
            duration_ms = (time.monotonic() - start_time) * 1000
            return {
                'status': 'error',
                'error': str(e),
                'duration_ms': round(duration_ms, 1),
            }


def _execute_restricted(code: str, policy: SandboxPolicy, namespace: dict) -> dict:
    """
    Execute user code in a restricted namespace within run_in_executor.

    Security measures:
    - Safe builtins only (exec/eval/open/compile removed)
    - Import whitelist via safe_import wrapper
    - Isolated globals (no access to outer scope)
    - ResourceLimiter (timeout + memory monitoring)
    """
    try:
        safe_builtins = policy.get_safe_builtins()

        original_import = __import__

        def safe_import(name, *args, **kwargs):
            if not policy.is_module_allowed(name):
                raise ImportError("Module '%s' is not allowed" % name)
            return original_import(name, *args, **kwargs)

        safe_builtins['__import__'] = safe_import

        # Isolated globals — no reference to outer scope
        script_globals = {
            '__builtins__': safe_builtins,
            '__name__': '__sandbox__',
        }

        # Only inject safe SCADA API callables from namespace
        _ALLOWED_NAMESPACE_KEYS = (
            'tag_read', 'tag_write', 'history_query',
            'log_info', 'log_warning', 'log_error',
            'emit_result', 'print', 'payload',
        )
        for key in _ALLOWED_NAMESPACE_KEYS:
            if key in namespace:
                script_globals[key] = namespace[key]

        # Resource limiter (timeout + memory monitor thread)
        limiter = ResourceLimiter(
            timeout_sec=policy.timeout_sec,
            max_memory_mb=policy.max_memory_mb
        )
        limiter.start()

        try:
            compiled = compile(code, '<user_script>', 'exec')
            exec(compiled, script_globals)
        finally:
            limiter.stop()

        return {'status': 'ok'}

    except Exception as e:
        return {
            'status': 'error',
            'error': str(e),
            'traceback': traceback.format_exc(),
        }
