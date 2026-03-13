"""
SCADA SDK API bridge for sandbox.
Provides safe, limited API for user scripts to interact with SCADA.

Preload mode (Phase 5):
  C# reads tag values and injects them as _tag_snapshot / _history_snapshot
  in the payload. ScadaApi reads from these snapshots.
  tag_write() does NOT write immediately — stores pending writes.
  C# applies pending_writes after script execution.
"""

import logging
import time
from typing import Any, Callable, Dict, List, Optional

logger = logging.getLogger(__name__)


class ScadaApi:
    """
    SCADA SDK API available inside user scripts.

    Available functions:
        tag_read(tag_name) -> value
        tag_write(tag_name, value) -> bool
        history_query(tag_name, start, end) -> list
        log_info(msg), log_warning(msg), log_error(msg)
        emit_result(data)
    """

    def __init__(self, request_id: str = '', user: str = '',
                 tag_write_allowed: bool = False,
                 on_tag_read: Callable = None,
                 on_tag_write: Callable = None,
                 on_history_query: Callable = None,
                 audit_logger=None,
                 tag_snapshot: dict = None,
                 history_snapshot: dict = None):
        self._request_id = request_id
        self._user = user
        self._tag_write_allowed = tag_write_allowed
        self._on_tag_read = on_tag_read
        self._on_tag_write = on_tag_write
        self._on_history_query = on_history_query
        self._audit_logger = audit_logger
        self._result = None
        self._logs: List[dict] = []
        self._output: List[str] = []
        self._output_size = 0
        self._max_output_kb = 1024

        # Preload snapshots (Phase 5)
        self._tag_snapshot: dict = tag_snapshot or {}
        self._history_snapshot: dict = history_snapshot or {}
        self._pending_writes: List[dict] = []

    def tag_read(self, tag_name: str) -> Any:
        """Read a tag value from SCADA (snapshot or callback)."""
        # 1. Try snapshot first (preload mode)
        if tag_name in self._tag_snapshot:
            return self._tag_snapshot[tag_name]

        # 2. Try callback (reverse IPC mode, future)
        if self._on_tag_read:
            return self._on_tag_read(tag_name)

        logger.debug("tag_read(%s) - not in snapshot, no handler", tag_name)
        return None

    def tag_write(self, tag_name: str, value: Any) -> bool:
        """
        Write a value to a SCADA tag.
        Does NOT write immediately — stores as pending write.
        C# applies pending_writes after script completes.
        Requires script.tag_write permission.
        """
        if not self._tag_write_allowed:
            raise PermissionError("tag_write is not allowed for this script")

        if self._audit_logger:
            self._audit_logger.log_tag_write(
                self._request_id, tag_name, value,
                user=self._user, script_name='user_script')

        # Store as pending write (C# will apply)
        self._pending_writes.append({
            'tag': tag_name,
            'value': value,
        })

        # Also update snapshot so subsequent reads see the new value
        self._tag_snapshot[tag_name] = value

        return True

    def history_query(self, tag_name: str, start_time: str, end_time: str) -> list:
        """Query historical data for a tag (snapshot or callback)."""
        # 1. Try snapshot first (preload mode)
        key = tag_name
        if key in self._history_snapshot:
            return self._history_snapshot[key]

        # 2. Try callback (reverse IPC mode, future)
        if self._on_history_query:
            return self._on_history_query(tag_name, start_time, end_time)

        return []

    def log_info(self, msg: str):
        self._logs.append({'level': 'INFO', 'message': str(msg), 'time': time.time()})

    def log_warning(self, msg: str):
        self._logs.append({'level': 'WARNING', 'message': str(msg), 'time': time.time()})

    def log_error(self, msg: str):
        self._logs.append({'level': 'ERROR', 'message': str(msg), 'time': time.time()})

    def emit_result(self, data):
        """Set the script result. Only the last call is used."""
        self._result = data

    def safe_print(self, *args, **kwargs):
        """Captured print output."""
        text = ' '.join(str(a) for a in args)
        self._output_size += len(text)
        if self._output_size > self._max_output_kb * 1024:
            raise RuntimeError("Output size limit exceeded")
        self._output.append(text)

    def get_execution_result(self) -> dict:
        """Get the final result of script execution."""
        result = {
            'result': self._result,
            'logs': self._logs,
            'output': self._output,
        }
        # Include pending writes if any
        if self._pending_writes:
            result['_pending_writes'] = self._pending_writes
        return result

    def get_namespace(self) -> dict:
        """Return the namespace dict to inject into script globals."""
        return {
            'tag_read': self.tag_read,
            'tag_write': self.tag_write,
            'history_query': self.history_query,
            'log_info': self.log_info,
            'log_warning': self.log_warning,
            'log_error': self.log_error,
            'emit_result': self.emit_result,
            'print': self.safe_print,
        }
