"""
System services: ping, status, shutdown.
"""

import time
import platform
import asyncio

_start_time = time.time()
_shutdown_event: asyncio.Event = None


def set_shutdown_event(event: asyncio.Event):
    global _shutdown_event
    _shutdown_event = event


# 지원되는 메시지 버전 목록 (C# 측과 협상에 사용)
SUPPORTED_MESSAGE_VERSIONS = [1]


async def handle_ping(payload):
    """system/ping - health check + message version negotiation."""
    return {
        'pong': True,
        'uptime_sec': round(time.time() - _start_time, 1),
        'supportedMessageVersions': SUPPORTED_MESSAGE_VERSIONS,
    }


async def handle_status(payload):
    """system/status - detailed engine status."""
    return {
        'uptime_sec': round(time.time() - _start_time, 1),
        'python_version': platform.python_version(),
        'platform': platform.platform(),
        'state': 'running'
    }


async def handle_shutdown(payload):
    """system/shutdown - graceful shutdown."""
    if _shutdown_event:
        _shutdown_event.set()
    return {'shutting_down': True}


def register(router):
    router.register('system/ping', handle_ping)
    router.register('system/status', handle_status)
    router.register('system/shutdown', handle_shutdown)
