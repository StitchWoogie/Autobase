"""
Script sandbox restriction policy.
Defines what user scripts can and cannot do.
"""

from dataclasses import dataclass, field
from typing import List, Set


@dataclass
class SandboxPolicy:
    """Security policy for user script execution."""

    # Time/resource limits
    timeout_sec: float = 5.0
    max_memory_mb: int = 256
    max_output_kb: int = 1024

    # Module whitelist
    allowed_modules: List[str] = field(default_factory=lambda: [
        'math', 'statistics', 'datetime', 'json', 'collections',
        'itertools', 'functools', 'operator', 'decimal', 'fractions',
        'random', 'string', 're', 'time', 'copy',
        'numpy', 'pandas', 'scipy.stats',
    ])

    # Blocked builtins
    blocked_builtins: List[str] = field(default_factory=lambda: [
        'exec', 'eval', 'compile', '__import__',
        'open', 'input', 'breakpoint',
        'exit', 'quit', 'help',
        'globals', 'locals', 'vars', 'dir',
        'getattr', 'setattr', 'delattr',
        'type', 'super', 'classmethod', 'staticmethod',
        'property', 'memoryview',
    ])

    # Access restrictions
    network_access: bool = False
    file_access: bool = False
    subprocess_access: bool = False

    def get_safe_builtins(self) -> dict:
        """Return builtins dict with dangerous functions removed."""
        import builtins
        safe = {}
        blocked = set(self.blocked_builtins)
        for name in dir(builtins):
            if name.startswith('_') and name != '__name__':
                continue
            if name in blocked:
                continue
            safe[name] = getattr(builtins, name)
        # Add safe replacements
        safe['print'] = self._safe_print
        return safe

    def _safe_print(self, *args, **kwargs):
        """Print replacement that captures output."""
        pass  # Overridden by runner

    def is_module_allowed(self, module_name: str) -> bool:
        """Check if a module import is allowed."""
        for allowed in self.allowed_modules:
            if module_name == allowed or module_name.startswith(allowed + '.'):
                return True
        return False
