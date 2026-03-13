"""
Script execution service (Phase 3 + Phase 5 SCADA integration).
Runs user Python scripts in sandbox with tag snapshot support.
"""

import logging

from sandbox.runner import SandboxRunner
from sandbox.policy import SandboxPolicy

logger = logging.getLogger(__name__)

_sandbox_runner: SandboxRunner = None


def initialize(policy: SandboxPolicy = None, audit_logger=None):
    """Initialize the script service with sandbox policy."""
    global _sandbox_runner
    _sandbox_runner = SandboxRunner(policy=policy, audit_logger=audit_logger)
    logger.info("Script service initialized")


async def handle_script_execute(payload: dict) -> dict:
    """
    script.execute — execute user Python script.

    Input: {
        code: str,                       # Python code
        payload: {                       # User data + SCADA snapshots
            _tag_snapshot: {tag: value},  # Pre-read tag values (C# injects)
            _history_snapshot: {tag: [values]},  # Pre-read history (C# injects)
            ... (user custom data)
        },
        context: {
            user: str,
            permissions: [str],          # e.g. ["script.execute", "script.tag_write"]
        }
    }

    Output: {
        status: 'ok' | 'error',
        result: any,                     # emit_result() value
        output: [str],                   # print() outputs
        logs: [{level, message, time}],  # log_info/warning/error
        _pending_writes: [{tag, value}], # tag_write() calls (C# applies)
        duration_ms: float,
    }
    """
    payload = payload or {}
    code = payload.get('code', '')
    script_payload = payload.get('payload', {})
    context = payload.get('context', {})

    if not code:
        return {'error': 'VALIDATION_ERROR: No code provided', 'status': 'error'}

    if _sandbox_runner is None:
        return {'error': 'ENGINE_UNAVAILABLE: Script service not initialized', 'status': 'error'}

    result = await _sandbox_runner.execute(code, script_payload, context)
    return result


def register(router):
    router.register('script/execute', handle_script_execute)
