"""
Python AI Engine entry point (Phase 1~4 full).
Wires up all subsystems and starts the asyncio TCP gateway.
"""

import asyncio
import argparse
import logging
import os
import signal
import sys

from config import DEFAULT_HOST, DEFAULT_PORT, EngineConfig
from gateway.server import GatewayServer
from gateway import large_payload
from router.router import Router
from router.validator import Validator, SchemaEntry
from router.auth import AuthChecker
from monitoring.health import HealthCollector
from monitoring.metrics import MetricsCollector
from monitoring.resource_guard import ResourceGuard
from logs.audit_logger import AuditLogger
from logs.structured_logger import setup_structured_logging
from models.registry import ModelRegistry
from models.cache import ModelCache
from models.hot_reload import ModelHotReloader
from workers.worker_pool import WorkerPool
from plugins.loader import PluginLoader
from plugins.interface import PluginContext
from sandbox.policy import SandboxPolicy

# Services
from services import (
    system_service,
    predict_service,
    analysis_service,
    vision_service,
    train_service,
    script_service,
    codegen_service,
    layout_service,
)


def build_validator(cfg: EngineConfig) -> Validator:
    """Build the request validator with known schemas."""
    schemas_dir = os.path.join(os.path.dirname(__file__), 'schemas')
    validator = Validator(schemas_dir=schemas_dir, default_max_kb=cfg.payload_max_kb)

    # Register known schemas
    validator.register_schema('predict/power', SchemaEntry(
        required_fields=[],
        field_types={'current_kw': (int, float), 'history': list},
    ))
    validator.register_schema('analysis/trend', SchemaEntry(
        required_fields=['values'],
        field_types={'values': list},
    ))
    validator.register_schema('analysis/correlation', SchemaEntry(
        required_fields=['values_x', 'values_y'],
        field_types={'values_x': list, 'values_y': list},
    ))
    validator.register_schema('script/execute', SchemaEntry(
        required_fields=['code'],
        field_types={'code': str},
    ))
    validator.register_schema('codegen/generate', SchemaEntry(
        required_fields=[],
        field_types={'prompt': str, 'template': str, 'tag': str},
    ))
    validator.register_schema('codegen/validate', SchemaEntry(
        required_fields=['code'],
        field_types={'code': str},
    ))
    validator.register_schema('layout/suggest', SchemaEntry(
        required_fields=['objects'],
        field_types={'objects': list},
    ))
    validator.register_schema('layout/optimize', SchemaEntry(
        required_fields=['positions'],
        field_types={'positions': list},
    ))
    return validator


async def main(host: str, port: int, config_path: str = None):
    logger = logging.getLogger(__name__)

    # Load config
    cfg = EngineConfig.load(config_path) if config_path else EngineConfig()
    cfg.host = host
    cfg.port = port

    # --- Subsystems ---

    # Metrics
    metrics = MetricsCollector()

    # Audit logger
    audit_path = os.path.join(os.path.dirname(__file__), cfg.audit_path)
    audit = AuditLogger(audit_path=audit_path, enabled=cfg.audit_enabled)

    # Validator & Auth (Phase 5: strict_mode from config, default=True)
    validator = build_validator(cfg)
    auth = AuthChecker(strict_mode=cfg.auth_strict_mode)

    # Router (enhanced)
    router = Router(validator=validator, auth=auth, metrics=metrics, audit_logger=audit)

    # Model Registry & Cache
    models_path = os.path.join(os.path.dirname(__file__), cfg.models_path)
    model_registry = ModelRegistry(models_path)
    model_registry.scan()
    model_cache = ModelCache(
        max_count=cfg.cache_max_count,
        max_memory_mb=cfg.cache_max_memory_mb,
    )

    # Worker Pool
    worker_pool = WorkerPool(
        realtime_workers=cfg.realtime_workers,
        batch_workers=cfg.batch_workers,
        script_workers=cfg.script_workers,
    )

    # Health Collector
    health = HealthCollector(version='1.0.0')
    health.set_model_registry(model_registry)
    health.set_model_cache(model_cache)
    health.set_worker_pool(worker_pool)

    # --- Register all services ---

    # System services
    shutdown_event = asyncio.Event()
    system_service.set_shutdown_event(shutdown_event)
    system_service.register(router)

    # Prediction services (Phase 5: model registry + cache injection)
    predict_service.register(router, model_registry=model_registry, model_cache=model_cache)

    # Analysis services (Phase 2)
    analysis_service.register(router)

    # Vision services (Phase 4)
    vision_service.register(router)

    # Training services (Phase 4)
    train_service.register(router)

    # Codegen services (code generation, validation, fix)
    codegen_service.register(router)

    # Layout services (placement, optimization, validation)
    layout_service.register(router)

    # Script sandbox service (Phase 3)
    sandbox_policy = SandboxPolicy(
        timeout_sec=cfg.script_timeout_sec,
        max_memory_mb=cfg.script_memory_mb,
        max_output_kb=cfg.output_max_kb,
    )
    script_service.initialize(policy=sandbox_policy, audit_logger=audit)
    script_service.register(router)

    # Enhanced system services (metrics, models, health)
    async def handle_system_metrics(payload):
        return metrics.get_all_metrics()

    async def handle_system_models(payload):
        return {
            'registry': model_registry.list_models(),
            'cache': model_cache.get_stats(),
        }

    async def handle_system_health(payload):
        return health.collect()

    router.register('system/metrics', handle_system_metrics)
    router.register('system/models', handle_system_models)
    router.register('system/health', handle_system_health)

    # Register handlers with worker pool
    for svc_name in router.list_services():
        pool_type = 'realtime'
        if svc_name.startswith('analysis/') or svc_name.startswith('train/'):
            pool_type = 'batch'
        elif svc_name.startswith('script/'):
            pool_type = 'script'

    # --- Plugins (Phase 3) ---
    plugins_path = os.path.join(os.path.dirname(__file__), cfg.plugins_path)
    plugin_context = PluginContext(
        engine_version='1.0.0',
        config={},
        model_registry=model_registry,
        audit_logger=audit,
    )
    plugin_loader = PluginLoader(plugins_path, plugin_context)
    if cfg.auto_load_plugins:
        loaded = plugin_loader.scan_and_load()
        # Register plugin handlers
        for plugin_info in plugin_loader.get_loaded_plugins():
            for svc in plugin_info.get('services', []):
                loaded_plugin = plugin_loader.get_handler(svc)
                if loaded_plugin:
                    async def make_plugin_handler(lp, s):
                        from plugins.interface import ServiceRequest
                        async def handler(payload):
                            req = ServiceRequest(service=s, payload=payload or {})
                            resp = await lp.instance.execute(req)
                            if resp.is_success:
                                return resp.result
                            raise RuntimeError(resp.error)
                        return handler
                    handler = await make_plugin_handler(loaded_plugin, svc)
                    router.register(svc, handler)

    # --- Large payload handler (Phase 4) ---
    shared_dir = cfg.shared_dir or os.path.join(os.path.dirname(__file__), 'shared')
    large_payload.initialize(shared_dir)

    # --- Model hot reload (Phase 4) ---
    hot_reloader = None
    if cfg.hot_reload_enabled:
        hot_reloader = ModelHotReloader(
            model_registry, model_cache,
            check_interval_sec=cfg.hot_reload_interval_sec,
        )
        await hot_reloader.start()

    # --- Resource guard ---
    resource_guard = ResourceGuard(
        max_memory_mb=cfg.max_memory_mb,
        check_interval_sec=cfg.resource_check_interval_sec,
    )
    await resource_guard.start()

    # --- Start worker pool ---
    await worker_pool.start()

    logger.info("Registered services: %s", router.list_services())
    logger.info("Models: %s", model_registry.list_loaded_models())

    # --- Start Gateway Server ---
    server = GatewayServer(host=host, port=port, router=router)
    await server.start()

    # Signal handlers (Unix)
    if sys.platform != 'win32':
        loop = asyncio.get_event_loop()
        for sig in (signal.SIGTERM, signal.SIGINT):
            loop.add_signal_handler(sig, shutdown_event.set)

    logger.info("Python AI Engine ready (Phase 4). Waiting for connections...")

    # --- Wait for shutdown ---
    await shutdown_event.wait()

    # --- Cleanup ---
    logger.info("Shutting down...")
    await server.stop()
    await resource_guard.stop()
    if hot_reloader:
        await hot_reloader.stop()
    await worker_pool.stop()
    plugin_loader.unload_all()
    model_cache.clear()
    audit.close()
    large_payload.cleanup_old_files()

    logger.info("Python AI Engine stopped.")


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Python AI Engine for SCADA')
    parser.add_argument('--host', default=DEFAULT_HOST, help='Bind host')
    parser.add_argument('--port', type=int, default=DEFAULT_PORT, help='Bind port')
    parser.add_argument('--config', default=None, help='Config file path')
    parser.add_argument('--log-level', default='INFO', help='Log level')
    parser.add_argument('--log-format', default='text',
                        choices=['text', 'json'], help='Log format')
    args = parser.parse_args()

    if args.log_format == 'json':
        setup_structured_logging(level=args.log_level)
    else:
        logging.basicConfig(
            level=getattr(logging, args.log_level.upper(), logging.INFO),
            format='%(asctime)s [%(levelname)s] %(name)s: %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S'
        )

    try:
        asyncio.run(main(args.host, args.port, args.config))
    except KeyboardInterrupt:
        pass
