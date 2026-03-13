"""
Python AI Engine configuration (Phase 1~4 full).
"""

import json
import logging
import os
from dataclasses import dataclass, field
from typing import List

logger = logging.getLogger(__name__)

# Defaults
DEFAULT_HOST = '127.0.0.1'
DEFAULT_PORT = 5678
MAX_MESSAGE_SIZE = 10_000_000  # 10MB


@dataclass
class EngineConfig:
    """Full engine configuration."""

    # Engine
    enabled: bool = True
    host: str = DEFAULT_HOST
    port: int = DEFAULT_PORT
    protocol_version: str = '1.0'

    # Workers
    realtime_workers: int = 2
    batch_workers: int = 1
    script_workers: int = 1

    # Queues
    realtime_queue_size: int = 100
    batch_queue_size: int = 50
    script_queue_size: int = 20

    # Script limits
    script_timeout_sec: float = 5.0
    script_memory_mb: int = 256
    payload_max_kb: int = 1024
    large_payload_threshold_kb: int = 64
    output_max_kb: int = 1024

    # Models
    models_path: str = 'models/'
    preload_models: List[str] = field(default_factory=list)
    cache_max_count: int = 10
    cache_max_memory_mb: float = 2048.0
    warmup_on_load: bool = True
    hot_reload_enabled: bool = True
    hot_reload_interval_sec: float = 30.0

    # Plugins
    plugins_path: str = 'plugins/'
    auto_load_plugins: bool = True

    # Logging
    log_level: str = 'INFO'
    log_format: str = 'json'
    audit_enabled: bool = True
    audit_path: str = 'logs/audit.log'
    log_file: str = ''

    # Auth
    auth_strict_mode: bool = True  # True=deny when no permissions, False=allow (dev mode)

    # Resource guard
    max_memory_mb: float = 2048.0
    resource_check_interval_sec: float = 10.0

    # Shared directory for large payloads
    shared_dir: str = ''

    @staticmethod
    def load(config_path: str) -> 'EngineConfig':
        """Load config from JSON file."""
        if not os.path.exists(config_path):
            logger.info("No config file found, using defaults: %s", config_path)
            return EngineConfig()

        try:
            with open(config_path, 'r', encoding='utf-8') as f:
                data = json.load(f)

            cfg = EngineConfig()

            # Engine section
            engine = data.get('engine', {})
            cfg.enabled = engine.get('enabled', cfg.enabled)
            cfg.host = engine.get('host', cfg.host)
            cfg.port = engine.get('port', cfg.port)
            cfg.protocol_version = engine.get('protocolVersion', cfg.protocol_version)

            # Workers
            workers = data.get('workers', {})
            cfg.realtime_workers = workers.get('realtime', {}).get('count', cfg.realtime_workers)
            cfg.batch_workers = workers.get('batch', {}).get('count', cfg.batch_workers)
            cfg.script_workers = workers.get('script', {}).get('count', cfg.script_workers)

            # Queues
            queues = data.get('queues', {})
            cfg.realtime_queue_size = queues.get('realtime', {}).get('maxSize', cfg.realtime_queue_size)
            cfg.batch_queue_size = queues.get('batch', {}).get('maxSize', cfg.batch_queue_size)
            cfg.script_queue_size = queues.get('script', {}).get('maxSize', cfg.script_queue_size)

            # Limits
            limits = data.get('limits', {})
            timeout_ms = limits.get('scriptTimeoutMs')
            if timeout_ms:
                cfg.script_timeout_sec = timeout_ms / 1000.0
            cfg.script_memory_mb = limits.get('scriptMemoryMb', cfg.script_memory_mb)
            cfg.payload_max_kb = limits.get('payloadMaxKb', cfg.payload_max_kb)
            cfg.large_payload_threshold_kb = limits.get('largePayloadThresholdKb', cfg.large_payload_threshold_kb)
            cfg.output_max_kb = limits.get('outputMaxKb', cfg.output_max_kb)

            # Models
            models = data.get('models', {})
            cfg.models_path = models.get('registryPath', cfg.models_path)
            cfg.preload_models = models.get('preload', cfg.preload_models)
            cfg.cache_max_count = models.get('cacheMaxCount', cfg.cache_max_count)
            cfg.cache_max_memory_mb = models.get('cacheMaxMemoryMb', cfg.cache_max_memory_mb)
            cfg.warmup_on_load = models.get('warmupOnLoad', cfg.warmup_on_load)
            cfg.hot_reload_enabled = models.get('hotReloadEnabled', cfg.hot_reload_enabled)
            cfg.hot_reload_interval_sec = models.get('hotReloadIntervalSec', cfg.hot_reload_interval_sec)

            # Plugins
            plugins = data.get('plugins', {})
            cfg.plugins_path = plugins.get('pluginPath', cfg.plugins_path)
            cfg.auto_load_plugins = plugins.get('autoLoad', cfg.auto_load_plugins)

            # Auth
            auth_cfg = data.get('auth', {})
            cfg.auth_strict_mode = auth_cfg.get('strictMode', cfg.auth_strict_mode)

            # Logging
            log_cfg = data.get('logging', {})
            cfg.log_level = log_cfg.get('level', cfg.log_level)
            cfg.log_format = log_cfg.get('format', cfg.log_format)
            cfg.audit_enabled = log_cfg.get('auditEnabled', cfg.audit_enabled)
            cfg.audit_path = log_cfg.get('auditPath', cfg.audit_path)
            cfg.log_file = log_cfg.get('file', cfg.log_file)

            # Resource guard
            resource = data.get('resource', {})
            cfg.max_memory_mb = resource.get('maxMemoryMb', cfg.max_memory_mb)
            cfg.resource_check_interval_sec = resource.get('checkIntervalSec', cfg.resource_check_interval_sec)

            # Shared directory
            cfg.shared_dir = data.get('sharedDir', cfg.shared_dir)

            logger.info("Config loaded from %s (auth_strict_mode=%s)", config_path, cfg.auth_strict_mode)
            return cfg

        except Exception as e:
            logger.error("Failed to load config %s: %s", config_path, e)
            return EngineConfig()
