"""
Request metrics collection.
Tracks request count, latency, error rate per service.
"""

import threading
import time
from collections import defaultdict
from typing import Dict


class ServiceMetrics:
    __slots__ = ('total', 'success', 'errors', 'total_duration_ms',
                 'min_ms', 'max_ms', 'last_error', 'last_call_time')

    def __init__(self):
        self.total = 0
        self.success = 0
        self.errors = 0
        self.total_duration_ms = 0.0
        self.min_ms = float('inf')
        self.max_ms = 0.0
        self.last_error = None
        self.last_call_time = 0.0

    def record(self, duration_ms: float, is_success: bool, error: str = None):
        self.total += 1
        self.total_duration_ms += duration_ms
        self.last_call_time = time.time()
        if duration_ms < self.min_ms:
            self.min_ms = duration_ms
        if duration_ms > self.max_ms:
            self.max_ms = duration_ms
        if is_success:
            self.success += 1
        else:
            self.errors += 1
            self.last_error = error

    @property
    def avg_ms(self) -> float:
        return self.total_duration_ms / self.total if self.total > 0 else 0.0

    @property
    def error_rate(self) -> float:
        return self.errors / self.total if self.total > 0 else 0.0

    def to_dict(self) -> dict:
        return {
            'total': self.total,
            'success': self.success,
            'errors': self.errors,
            'error_rate': round(self.error_rate, 3),
            'avg_ms': round(self.avg_ms, 1),
            'min_ms': round(self.min_ms, 1) if self.min_ms != float('inf') else 0,
            'max_ms': round(self.max_ms, 1),
            'last_error': self.last_error,
        }


class MetricsCollector:
    """Thread-safe metrics collector for all services."""

    def __init__(self):
        self._metrics: Dict[str, ServiceMetrics] = defaultdict(ServiceMetrics)
        self._lock = threading.Lock()
        self._start_time = time.time()

    def record(self, service: str, duration_ms: float, is_success: bool, error: str = None):
        with self._lock:
            self._metrics[service].record(duration_ms, is_success, error)

    def get_service_metrics(self, service: str) -> dict:
        with self._lock:
            if service in self._metrics:
                return self._metrics[service].to_dict()
            return {}

    def get_all_metrics(self) -> dict:
        with self._lock:
            services = {k: v.to_dict() for k, v in self._metrics.items()}
            total_requests = sum(m.total for m in self._metrics.values())
            total_errors = sum(m.errors for m in self._metrics.values())
            return {
                'uptime_sec': round(time.time() - self._start_time, 1),
                'total_requests': total_requests,
                'total_errors': total_errors,
                'services': services,
            }

    def reset(self):
        with self._lock:
            self._metrics.clear()
            self._start_time = time.time()
