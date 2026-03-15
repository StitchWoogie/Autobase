"""
Code generation services: generate, explain, fix, validate.
Provides AI-assisted code authoring for Autobase Script (C#-like DSL).

Strategies:
1. Template-based generation: pattern matching + code templates
2. Rule-based fixes: common error patterns → auto-corrections
3. Context-aware suggestions: tag/method context → relevant code
"""

import logging
import re
import time
from typing import Dict, List, Optional, Tuple

logger = logging.getLogger(__name__)

# ---------------------------------------------------------------------------
# Tag type → typical code pattern templates
# ---------------------------------------------------------------------------

_TAG_TYPE_TEMPLATES: Dict[str, List[dict]] = {
    'AI': [
        {
            'name': 'read_and_check_limit',
            'description': '아날로그 값 읽기 + 상/하한 체크',
            'template': (
                'double {var} = ${tag}.value;\n'
                'if ({var} > ${tag}.hihi)\n'
                '{{\n'
                '\t@SetAlarm("{tag}_HI");\n'
                '}}\n'
                'else if ({var} < ${tag}.lolo)\n'
                '{{\n'
                '\t@SetAlarm("{tag}_LO");\n'
                '}}'
            ),
        },
        {
            'name': 'trend_logging',
            'description': '아날로그 값 DB 기록',
            'template': (
                'double {var} = ${tag}.value;\n'
                '@LogToDatabase("{tag}", {var}, DateTime.Now);'
            ),
        },
        {
            'name': 'moving_average',
            'description': '이동 평균 계산',
            'template': (
                'double {var} = ${tag}.value;\n'
                '// 이동 평균 (최근 N개 샘플)\n'
                'const int N = 10;\n'
                'double sum = 0;\n'
                'for (int i = 0; i < N; i++)\n'
                '{{\n'
                '\tsum += ${tag}.value;\n'
                '}}\n'
                'double avg = sum / N;'
            ),
        },
    ],
    'AO': [
        {
            'name': 'setpoint_with_clamp',
            'description': '설정값 쓰기 (범위 제한)',
            'template': (
                'double setpoint = {value};\n'
                'if (setpoint > ${tag}.hihi) setpoint = ${tag}.hihi;\n'
                'if (setpoint < ${tag}.lolo) setpoint = ${tag}.lolo;\n'
                '${tag}.value = setpoint;'
            ),
        },
    ],
    'DI': [
        {
            'name': 'state_check',
            'description': '디지털 입력 상태 확인',
            'template': (
                'if (${tag}.value == 1)\n'
                '{{\n'
                '\t// ${tag}.desON 상태\n'
                '\t{cursor}\n'
                '}}\n'
                'else\n'
                '{{\n'
                '\t// ${tag}.desOFF 상태\n'
                '}}'
            ),
        },
    ],
    'DO': [
        {
            'name': 'toggle',
            'description': '디지털 출력 토글',
            'template': (
                'if (${tag}.value == 0)\n'
                '\t${tag}.value = 1;\n'
                'else\n'
                '\t${tag}.value = 0;'
            ),
        },
        {
            'name': 'interlock_control',
            'description': '인터록 조건부 제어',
            'template': (
                '// 인터록 조건 확인 후 출력\n'
                'if ({interlock_condition})\n'
                '{{\n'
                '\t${tag}.value = 1;\n'
                '\t@LogEvent("{tag} ON - interlock OK");\n'
                '}}\n'
                'else\n'
                '{{\n'
                '\t${tag}.value = 0;\n'
                '\t@SetAlarm("{tag}_INTERLOCK");\n'
                '}}'
            ),
        },
    ],
}

# ---------------------------------------------------------------------------
# Common SCADA code pattern templates (tag-type independent)
# ---------------------------------------------------------------------------

_PATTERN_TEMPLATES: Dict[str, dict] = {
    'alarm_check': {
        'keywords': ['alarm', 'alm', '알람', '경보', 'alert'],
        'template': (
            '// 알람 체크\n'
            'double val = ${tag}.value;\n'
            'double hi = ${tag}.hihi;\n'
            'double lo = ${tag}.lolo;\n'
            '\n'
            'if (val > hi)\n'
            '{{\n'
            '\t@SetAlarm("{tag}_HIHI");\n'
            '\t@LogEvent("{tag} 상한 초과: " + val);\n'
            '}}\n'
            'else if (val < lo)\n'
            '{{\n'
            '\t@SetAlarm("{tag}_LOLO");\n'
            '\t@LogEvent("{tag} 하한 미달: " + val);\n'
            '}}'
        ),
    },
    'pid_control': {
        'keywords': ['pid', 'PID', '제어', 'control', 'setpoint'],
        'template': (
            '// PID 제어\n'
            'double pv = ${pv_tag}.value;     // Process Variable\n'
            'double sp = ${sp_tag}.value;     // Setpoint\n'
            'double error = sp - pv;\n'
            '\n'
            'double Kp = {kp};  // 비례 이득\n'
            'double Ki = {ki};  // 적분 이득\n'
            'double Kd = {kd};  // 미분 이득\n'
            '\n'
            'double output = Kp * error;\n'
            '${output_tag}.value = output;'
        ),
    },
    'data_logging': {
        'keywords': ['log', '기록', '저장', 'save', 'database', 'db', '로그'],
        'template': (
            '// 데이터 기록\n'
            'double val = ${tag}.value;\n'
            '@LogToDatabase("{tag}", val, DateTime.Now);\n'
            '@LogEvent("{tag} 기록: " + val);'
        ),
    },
    'timer_action': {
        'keywords': ['timer', '타이머', '주기', 'interval', 'delay', '대기'],
        'template': (
            '// 주기적 동작\n'
            'int elapsed = @GetElapsedSec();\n'
            'if (elapsed % {interval_sec} == 0)\n'
            '{{\n'
            '\t// {interval_sec}초마다 실행\n'
            '\t{cursor}\n'
            '}}'
        ),
    },
    'batch_read': {
        'keywords': ['batch', '일괄', '여러', 'multiple', 'all', '전체'],
        'template': (
            '// 일괄 태그 읽기\n'
            'double[] values = new double[{count}];\n'
            '{read_lines}\n'
            '\n'
            '// 평균 계산\n'
            'double sum = 0;\n'
            'for (int i = 0; i < {count}; i++) sum += values[i];\n'
            'double avg = sum / {count};'
        ),
    },
}

# ---------------------------------------------------------------------------
# Common error patterns and auto-fix rules
# ---------------------------------------------------------------------------

_ERROR_FIX_RULES: List[dict] = [
    {
        'pattern': r'missing\s*;|세미콜론|semicolon',
        'fix_type': 'add_semicolon',
        'description': '세미콜론 추가',
    },
    {
        'pattern': r'undefined\s+identifier|정의되지\s*않은|unknown\s+tag',
        'fix_type': 'suggest_similar_tag',
        'description': '유사 태그명 제안',
    },
    {
        'pattern': r'type\s+mismatch|타입\s*불일치|cannot\s+convert',
        'fix_type': 'add_cast',
        'description': '타입 캐스팅 추가',
    },
    {
        'pattern': r'unmatched\s+bracket|괄호|brace|paren',
        'fix_type': 'fix_brackets',
        'description': '괄호 균형 수정',
    },
]

# ---------------------------------------------------------------------------
# Autobase script syntax validation rules
# ---------------------------------------------------------------------------

_VALIDATION_RULES = [
    {
        'id': 'missing_semicolon',
        'pattern': re.compile(
            r'^(?!\s*//)(?!.*[{};,]\s*$)(?!.*^\s*$)(?!.*\b(?:if|else|for|while|do)\b.*$)'
            r'(?!.*\{\s*$)(?!.*\}\s*$).+$',
            re.MULTILINE
        ),
        'message': '세미콜론(;)이 누락되었을 수 있습니다',
        'severity': 'warning',
    },
    {
        'id': 'unknown_tag_ref',
        'check': 'tag_reference',
        'message': '존재하지 않는 태그 참조: {tag}',
        'severity': 'error',
    },
    {
        'id': 'bitshift_vs_compare',
        'pattern': re.compile(r'(?<!=)>>(?!=)'),
        'message': "비교 연산자 '>'를 의도하셨나요? ('>>'는 비트 시프트 연산자)",
        'severity': 'warning',
    },
    {
        'id': 'assignment_in_condition',
        'pattern': re.compile(r'\b(?:if|while)\s*\([^)]*(?<!=)=(?!=)[^)]*\)'),
        'message': "비교 연산자 '=='를 의도하셨나요? ('='는 대입 연산자)",
        'severity': 'warning',
    },
    {
        'id': 'tag_without_member',
        'pattern': re.compile(r'\$\w+(?!\.\w)(?!\s*\.)'),
        'message': '태그 멤버(.value 등)를 지정하세요',
        'severity': 'info',
    },
]


# ---------------------------------------------------------------------------
# Helper functions
# ---------------------------------------------------------------------------

def _to_var_name(tag_name: str) -> str:
    """Convert tag name like 'Reactor1.Temperature' to variable name 'temperature'."""
    parts = tag_name.replace('.', '_').split('_')
    last = parts[-1] if parts else tag_name
    return last[0].lower() + last[1:] if last else 'val'


def _find_similar_tags(unknown_tag: str, known_tags: List[str], max_results: int = 3) -> List[str]:
    """Find tags similar to the unknown one using simple edit distance."""
    if not known_tags:
        return []

    unknown_lower = unknown_tag.lower()
    scored = []
    for tag in known_tags:
        tag_lower = tag.lower()
        # Simple similarity: common prefix length + substring match
        score = 0
        # Common prefix
        for i in range(min(len(unknown_lower), len(tag_lower))):
            if unknown_lower[i] == tag_lower[i]:
                score += 2
            else:
                break
        # Substring match
        if unknown_lower in tag_lower or tag_lower in unknown_lower:
            score += 10
        # Same length bonus
        if abs(len(unknown_tag) - len(tag)) <= 2:
            score += 3
        if score > 0:
            scored.append((tag, score))

    scored.sort(key=lambda x: -x[1])
    return [t[0] for t in scored[:max_results]]


def _match_intent(prompt: str) -> Tuple[Optional[str], dict]:
    """Match a natural language prompt to a code pattern template."""
    prompt_lower = prompt.lower()

    for pattern_name, pattern_info in _PATTERN_TEMPLATES.items():
        for keyword in pattern_info['keywords']:
            if keyword.lower() in prompt_lower:
                return pattern_name, pattern_info

    return None, {}


def _extract_tags_from_prompt(prompt: str, available_tags: List[str]) -> List[str]:
    """Extract tag names mentioned in the prompt."""
    found = []
    for tag in available_tags:
        if tag.lower() in prompt.lower() or tag in prompt:
            found.append(tag)
    # Also check $Tag pattern
    tag_refs = re.findall(r'\$(\w[\w.]*)', prompt)
    for ref in tag_refs:
        if ref not in found:
            found.append(ref)
    return found


def _generate_from_template(template: str, tag: str, extra_vars: dict = None) -> str:
    """Fill a template with tag name and variable names."""
    var_name = _to_var_name(tag)
    result = template.replace('{tag}', tag).replace('{var}', var_name)
    result = result.replace('{cursor}', '// TODO: 여기에 코드 작성')
    if extra_vars:
        for k, v in extra_vars.items():
            result = result.replace('{' + k + '}', str(v))
    return result


def _validate_code(code: str, known_tags: List[str] = None) -> List[dict]:
    """Validate Autobase script code and return diagnostics."""
    diagnostics = []
    lines = code.split('\n')

    for i, line in enumerate(lines):
        stripped = line.strip()
        if not stripped or stripped.startswith('//') or stripped.startswith('/*'):
            continue

        # Check tag references
        if known_tags is not None:
            tag_refs = re.findall(r'\$(\w+)', line)
            for ref in tag_refs:
                if ref not in known_tags:
                    similar = _find_similar_tags(ref, known_tags, 3)
                    diagnostics.append({
                        'line': i + 1,
                        'column': line.index('$' + ref) + 1,
                        'severity': 'error',
                        'message': f"존재하지 않는 태그: ${ref}",
                        'rule': 'unknown_tag_ref',
                        'suggestions': ['$' + s for s in similar] if similar else [],
                    })

        # Check common patterns
        for rule in _VALIDATION_RULES:
            if rule.get('check') == 'tag_reference':
                continue  # Already handled above
            pattern = rule.get('pattern')
            if pattern and pattern.search(line):
                diagnostics.append({
                    'line': i + 1,
                    'column': 1,
                    'severity': rule['severity'],
                    'message': rule['message'],
                    'rule': rule['id'],
                })

    # Check bracket balance
    open_count = code.count('{') + code.count('(')
    close_count = code.count('}') + code.count(')')
    if open_count != close_count:
        diagnostics.append({
            'line': len(lines),
            'column': 1,
            'severity': 'error',
            'message': f'괄호 불균형: 열기 {open_count}개, 닫기 {close_count}개',
            'rule': 'bracket_mismatch',
        })

    return diagnostics


# ---------------------------------------------------------------------------
# Service handlers
# ---------------------------------------------------------------------------

async def handle_codegen_generate(payload: dict) -> dict:
    """
    codegen/generate — Generate Autobase script code from natural language or template.

    Input: {
        prompt: str,                    # Natural language description (Korean/English)
        context: {
            available_tags: [{name, type, description}],  # Tags in scope
            available_methods: [str],   # Methods in scope (@Method names)
            existing_code: str,         # Current editor content (optional)
            cursor_line: int,           # Current cursor line (optional)
        },
        template: str,                  # Specific template name (optional, overrides prompt)
        tag: str,                       # Primary tag name (optional)
    }

    Output: {
        code: str,                      # Generated code
        explanation: str,               # Explanation of what the code does
        tags_used: [str],               # Tags referenced in generated code
        template_used: str,             # Template name used (if any)
        confidence: float,              # 0.0~1.0
    }
    """
    payload = payload or {}
    start_time = time.monotonic()

    prompt = payload.get('prompt', '')
    context = payload.get('context', {})
    template_name = payload.get('template', '')
    primary_tag = payload.get('tag', '')

    available_tags = context.get('available_tags', [])
    tag_names = [t['name'] if isinstance(t, dict) else t for t in available_tags]
    tag_types = {}
    for t in available_tags:
        if isinstance(t, dict):
            tag_types[t['name']] = t.get('type', 'AI')

    # Strategy 1: Explicit template requested
    if template_name and primary_tag:
        tag_type = tag_types.get(primary_tag, 'AI')
        templates = _TAG_TYPE_TEMPLATES.get(tag_type, [])
        for tmpl in templates:
            if tmpl['name'] == template_name:
                code = _generate_from_template(tmpl['template'], primary_tag)
                duration_ms = (time.monotonic() - start_time) * 1000
                return {
                    'code': code,
                    'explanation': tmpl['description'],
                    'tags_used': [primary_tag],
                    'template_used': template_name,
                    'confidence': 0.95,
                    'generation_ms': round(duration_ms, 1),
                }

    # Strategy 2: Pattern match from natural language prompt
    if prompt:
        # Extract tags from prompt
        mentioned_tags = _extract_tags_from_prompt(prompt, tag_names)
        tag = primary_tag or (mentioned_tags[0] if mentioned_tags else '')

        # Match intent
        pattern_name, pattern_info = _match_intent(prompt)

        if pattern_name and pattern_info and tag:
            extra_vars = {}
            # Fill pattern-specific variables with defaults
            if pattern_name == 'pid_control':
                extra_vars = {'kp': '1.0', 'ki': '0.1', 'kd': '0.01',
                              'pv_tag': tag, 'sp_tag': tag, 'output_tag': tag}
                if len(mentioned_tags) >= 2:
                    extra_vars['sp_tag'] = mentioned_tags[1]
                if len(mentioned_tags) >= 3:
                    extra_vars['output_tag'] = mentioned_tags[2]
            elif pattern_name == 'timer_action':
                # Extract number from prompt
                nums = re.findall(r'(\d+)\s*(?:초|sec|s)', prompt)
                extra_vars['interval_sec'] = nums[0] if nums else '60'
            elif pattern_name == 'batch_read':
                extra_vars['count'] = str(len(mentioned_tags) or 4)
                read_lines = '\n'.join(
                    f'values[{i}] = ${t}.value;'
                    for i, t in enumerate(mentioned_tags)
                )
                extra_vars['read_lines'] = read_lines or 'values[0] = $Tag1.value;'

            code = _generate_from_template(pattern_info['template'], tag, extra_vars)
            duration_ms = (time.monotonic() - start_time) * 1000
            return {
                'code': code,
                'explanation': f'{pattern_name} 패턴: {prompt}',
                'tags_used': mentioned_tags or [tag],
                'template_used': pattern_name,
                'confidence': 0.80,
                'generation_ms': round(duration_ms, 1),
            }

        # Strategy 3: Tag-type based default template
        if tag:
            tag_type = tag_types.get(tag, 'AI')
            templates = _TAG_TYPE_TEMPLATES.get(tag_type, [])
            if templates:
                tmpl = templates[0]  # Use first (most common) template
                code = _generate_from_template(tmpl['template'], tag)
                duration_ms = (time.monotonic() - start_time) * 1000
                return {
                    'code': code,
                    'explanation': f'{tag_type} 태그 기본 패턴: {tmpl["description"]}',
                    'tags_used': [tag],
                    'template_used': tmpl['name'],
                    'confidence': 0.60,
                    'generation_ms': round(duration_ms, 1),
                }

    # Fallback: return comment skeleton
    duration_ms = (time.monotonic() - start_time) * 1000
    code = f'// {prompt}\n// TODO: 구현 필요\n'
    return {
        'code': code,
        'explanation': '자동 생성에 실패했습니다. 프롬프트를 더 구체적으로 작성해주세요.',
        'tags_used': [],
        'template_used': None,
        'confidence': 0.0,
        'generation_ms': round(duration_ms, 1),
    }


async def handle_codegen_explain(payload: dict) -> dict:
    """
    codegen/explain — Explain what Autobase script code does.

    Input: {
        code: str,                      # Code to explain
        language: str,                  # Language (default: autobase-script)
    }

    Output: {
        explanation: str,               # Human-readable explanation (Korean)
        tags_used: [str],               # Tags referenced
        methods_used: [str],            # Methods called
        complexity: str,                # simple | moderate | complex
    }
    """
    payload = payload or {}
    code = payload.get('code', '')

    if not code.strip():
        return {
            'explanation': '코드가 비어있습니다.',
            'tags_used': [],
            'methods_used': [],
            'complexity': 'simple',
        }

    # Extract tags
    tags_used = list(set(re.findall(r'\$(\w[\w.]*)', code)))

    # Extract methods
    methods_used = list(set(re.findall(r'@(\w+)', code)))

    # Analyze complexity
    lines = [l.strip() for l in code.split('\n') if l.strip() and not l.strip().startswith('//')]
    line_count = len(lines)

    has_loops = bool(re.search(r'\b(for|while|do)\b', code))
    has_conditions = bool(re.search(r'\b(if|else|switch)\b', code))
    nesting_depth = max(l.count('{') for l in code.split('\n')) if code else 0

    if line_count <= 5 and not has_loops:
        complexity = 'simple'
    elif line_count <= 20 and nesting_depth <= 2:
        complexity = 'moderate'
    else:
        complexity = 'complex'

    # Build explanation
    parts = []
    if tags_used:
        parts.append(f"태그 {len(tags_used)}개 사용: {', '.join('$' + t for t in tags_used[:5])}")
    if methods_used:
        parts.append(f"메서드 {len(methods_used)}개 호출: {', '.join('@' + m for m in methods_used[:5])}")
    if has_conditions:
        parts.append("조건 분기 포함")
    if has_loops:
        parts.append("반복문 포함")
    parts.append(f"코드 {line_count}줄, 복잡도: {complexity}")

    explanation = '. '.join(parts) + '.'

    return {
        'explanation': explanation,
        'tags_used': tags_used,
        'methods_used': methods_used,
        'complexity': complexity,
        'line_count': line_count,
    }


async def handle_codegen_fix(payload: dict) -> dict:
    """
    codegen/fix — Suggest fixes for code errors.

    Input: {
        code: str,                      # Code with errors
        errors: [{line, message}],      # Error messages from compiler/validator
        available_tags: [str],          # Known tags for suggestion
    }

    Output: {
        fixes: [{
            line: int,
            original: str,             # Original line
            fixed: str,                # Suggested fix
            description: str,          # What was changed
            confidence: float,
        }],
    }
    """
    payload = payload or {}
    code = payload.get('code', '')
    errors = payload.get('errors', [])
    known_tags = payload.get('available_tags', [])

    if not code or not errors:
        return {'fixes': []}

    lines = code.split('\n')
    fixes = []

    for error in errors:
        line_num = error.get('line', 0)
        message = error.get('message', '')

        if line_num < 1 or line_num > len(lines):
            continue

        original_line = lines[line_num - 1]

        for rule in _ERROR_FIX_RULES:
            if not re.search(rule['pattern'], message, re.IGNORECASE):
                continue

            fix_type = rule['fix_type']
            fixed_line = original_line
            confidence = 0.7
            description = rule['description']

            if fix_type == 'add_semicolon':
                stripped = original_line.rstrip()
                if stripped and stripped[-1] not in (';', '{', '}', ',', '('):
                    fixed_line = stripped + ';'
                    confidence = 0.9

            elif fix_type == 'suggest_similar_tag':
                tag_refs = re.findall(r'\$(\w+)', original_line)
                for ref in tag_refs:
                    if ref not in known_tags:
                        similar = _find_similar_tags(ref, known_tags, 1)
                        if similar:
                            fixed_line = original_line.replace('$' + ref, '$' + similar[0])
                            description = f"${ref} → ${similar[0]} 태그명 수정"
                            confidence = 0.75

            elif fix_type == 'add_cast':
                # Simple: wrap in (double) or (int) cast
                fixed_line = original_line  # Keep as-is, just suggest
                description = '타입 캐스팅을 추가해보세요: (double)값 또는 (int)값'
                confidence = 0.5

            elif fix_type == 'fix_brackets':
                open_b = original_line.count('{') + original_line.count('(')
                close_b = original_line.count('}') + original_line.count(')')
                if open_b > close_b:
                    fixed_line = original_line + ')' * (open_b - close_b)
                    description = '닫는 괄호 추가'
                    confidence = 0.6

            if fixed_line != original_line:
                fixes.append({
                    'line': line_num,
                    'original': original_line,
                    'fixed': fixed_line,
                    'description': description,
                    'confidence': confidence,
                })
            break  # One fix per error

    return {'fixes': fixes}


async def handle_codegen_validate(payload: dict) -> dict:
    """
    codegen/validate — Validate Autobase script code syntax and references.

    Input: {
        code: str,                      # Code to validate
        available_tags: [str],          # Known tag names
    }

    Output: {
        valid: bool,
        diagnostics: [{
            line: int,
            column: int,
            severity: str,              # error | warning | info
            message: str,
            rule: str,
            suggestions: [str],
        }],
        error_count: int,
        warning_count: int,
    }
    """
    payload = payload or {}
    code = payload.get('code', '')
    known_tags = payload.get('available_tags', [])

    if not code.strip():
        return {'valid': True, 'diagnostics': [], 'error_count': 0, 'warning_count': 0}

    diagnostics = _validate_code(code, known_tags if known_tags else None)

    error_count = sum(1 for d in diagnostics if d['severity'] == 'error')
    warning_count = sum(1 for d in diagnostics if d['severity'] == 'warning')

    return {
        'valid': error_count == 0,
        'diagnostics': diagnostics,
        'error_count': error_count,
        'warning_count': warning_count,
    }


async def handle_codegen_templates(payload: dict) -> dict:
    """
    codegen/templates — List available code templates for a tag type.

    Input: {
        tag_type: str,                  # AI, AO, DI, DO, ST
        tag_name: str,                  # Optional: tag name for preview
    }

    Output: {
        templates: [{name, description, preview}],
        patterns: [{name, keywords, description}],
    }
    """
    payload = payload or {}
    tag_type = payload.get('tag_type', 'AI')
    tag_name = payload.get('tag_name', 'Tag1')

    # Type-specific templates
    type_templates = _TAG_TYPE_TEMPLATES.get(tag_type, [])
    templates = []
    for tmpl in type_templates:
        preview = _generate_from_template(tmpl['template'], tag_name)
        templates.append({
            'name': tmpl['name'],
            'description': tmpl['description'],
            'preview': preview,
        })

    # General patterns
    patterns = []
    for name, info in _PATTERN_TEMPLATES.items():
        patterns.append({
            'name': name,
            'keywords': info['keywords'],
            'description': info['keywords'][0],
        })

    return {
        'templates': templates,
        'patterns': patterns,
        'tag_type': tag_type,
    }


# ---------------------------------------------------------------------------
# Registration
# ---------------------------------------------------------------------------

def register(router):
    """Register codegen services."""
    router.register('codegen/generate', handle_codegen_generate)
    router.register('codegen/explain', handle_codegen_explain)
    router.register('codegen/fix', handle_codegen_fix)
    router.register('codegen/validate', handle_codegen_validate)
    router.register('codegen/templates', handle_codegen_templates)
    logger.info("Codegen service registered (5 endpoints)")
