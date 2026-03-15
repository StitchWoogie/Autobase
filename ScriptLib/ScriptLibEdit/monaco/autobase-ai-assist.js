// Autobase AI Assist for Monaco Editor
// Provides: AI code generation (Ctrl+I), real-time diagnostics,
// signature help, hover documentation, and quick fixes.

(function () {
    'use strict';

    // =========================================================================
    // AI Assist State
    // =========================================================================

    var pendingRequests = {};
    var diagnosticTimer = null;
    var lastValidatedContent = '';

    window.autobaseAiAssist = {
        // Called from C# with generated code
        onGenerateResult: function (requestId, code, explanation) {
            var cb = pendingRequests[requestId];
            if (cb) {
                delete pendingRequests[requestId];
                cb({ code: code, explanation: explanation });
            }
        },
        // Called from C# with validation diagnostics
        onValidateResult: function (requestId, diagnosticsJson) {
            var cb = pendingRequests[requestId];
            if (cb) {
                delete pendingRequests[requestId];
                try {
                    var diagnostics = JSON.parse(diagnosticsJson);
                    cb(diagnostics);
                } catch (e) {
                    cb([]);
                }
            }
        }
    };

    // =========================================================================
    // Helpers
    // =========================================================================

    function generateId() {
        return 'ai_' + Date.now() + '_' + Math.random().toString(36).substr(2, 6);
    }

    function postMessage(msg) {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(JSON.stringify(msg));
        }
    }

    function getEditor() {
        return monaco.editor.getEditors()[0] || null;
    }

    function getTextBeforeCursor(model, position) {
        return model.getValueInRange({
            startLineNumber: position.lineNumber,
            startColumn: 1,
            endLineNumber: position.lineNumber,
            endColumn: position.column
        });
    }

    // =========================================================================
    // 1. Signature Help Provider — parameter hints on '(' and ','
    // =========================================================================

    var methodSignatures = {
        'SetAlarm': {
            label: '@SetAlarm(alarmId)',
            documentation: '알람을 발생시킵니다.',
            parameters: [
                { label: 'alarmId', documentation: '**string** — 알람 식별자' }
            ]
        },
        'LogToDatabase': {
            label: '@LogToDatabase(tagName, value, timestamp)',
            documentation: '태그 값을 데이터베이스에 기록합니다.',
            parameters: [
                { label: 'tagName', documentation: '**string** — 태그 이름' },
                { label: 'value', documentation: '**double** — 기록할 값' },
                { label: 'timestamp', documentation: '**DateTime** — 타임스탬프' }
            ]
        },
        'LogEvent': {
            label: '@LogEvent(message)',
            documentation: '이벤트 메시지를 기록합니다.',
            parameters: [
                { label: 'message', documentation: '**string** — 이벤트 메시지' }
            ]
        },
        'SetTagValue': {
            label: '@SetTagValue(tagName, value)',
            documentation: '태그 값을 설정합니다.',
            parameters: [
                { label: 'tagName', documentation: '**string** — 태그 이름' },
                { label: 'value', documentation: '**double** — 설정 값' }
            ]
        },
        'GetElapsedSec': {
            label: '@GetElapsedSec()',
            documentation: '프로그램 시작 이후 경과 시간(초)을 반환합니다.',
            parameters: []
        },
        'PythonAiCall': {
            label: '@PythonAiCall(service, payloadJson)',
            documentation: 'Python AI Engine 서비스를 호출합니다.',
            parameters: [
                { label: 'service', documentation: '**string** — 서비스 경로 (예: "predict/power")' },
                { label: 'payloadJson', documentation: '**string** — JSON 형식 페이로드' }
            ]
        }
    };

    // Add signatures from completion data (methods populated by C#)
    var _signaturesEnriched = false;

    function enrichSignatures() {
        var data = window.autobaseCompletionData;
        if (!data || !data.methods) return;
        // Re-enrich every time to catch late-loaded methods
        data.methods.forEach(function (m) {
            if (methodSignatures[m.name]) return;

            var params = [];
            // Use structured params array if available (new format from C#)
            if (m.params && m.params.length > 0) {
                m.params.forEach(function (p) {
                    var dirPrefix = '';
                    if (p.direction === 'out') dirPrefix = 'out ';
                    else if (p.direction === 'ref') dirPrefix = 'ref ';
                    else if (p.direction === 'params') dirPrefix = 'params ';

                    params.push({
                        label: p.name,
                        documentation: '**' + dirPrefix + p.type + '** \u2014 `' + p.name + '`'
                    });
                });
            } else if (m.signature) {
                // Fallback: parse "methodName(type1 param1, type2 param2)" format
                var match = m.signature.match(/\(([^)]*)\)/);
                if (match && match[1]) {
                    match[1].split(',').forEach(function (p) {
                        var trimmed = p.trim();
                        var parts = trimmed.split(/\s+/);
                        if (parts.length >= 2) {
                            params.push({
                                label: parts[parts.length - 1],
                                documentation: '**' + parts.slice(0, -1).join(' ') + '** \u2014 `' + parts[parts.length - 1] + '`'
                            });
                        } else if (trimmed) {
                            params.push({ label: trimmed, documentation: '' });
                        }
                    });
                }
            }

            var retStr = (m.returnType && m.returnType !== 'void') ? ('  \u2192 ' + m.returnType) : '';
            methodSignatures[m.name] = {
                label: '@' + (m.signature || m.name + '()'),
                documentation: (m.description || '') + retStr,
                parameters: params
            };
        });
    }

    // Parse user-defined functions from the script source code
    function parseUserFunctions(model) {
        var code = model.getValue();
        var funcPattern = /(?:void|int|double|float|string|bool|object)\s+(\w+)\s*\(([^)]*)\)/g;
        var match;
        while ((match = funcPattern.exec(code)) !== null) {
            var funcName = match[1];
            if (methodSignatures['__user_' + funcName]) continue;

            var retType = code.substring(match.index, match.index + match[0].indexOf(funcName)).trim();
            var params = [];
            if (match[2].trim()) {
                match[2].split(',').forEach(function (p) {
                    var trimmed = p.trim();
                    var parts = trimmed.split(/\s+/);
                    if (parts.length >= 2) {
                        params.push({
                            label: parts[parts.length - 1],
                            documentation: '**' + parts.slice(0, -1).join(' ') + '** \u2014 `' + parts[parts.length - 1] + '`'
                        });
                    } else if (trimmed) {
                        params.push({ label: trimmed, documentation: '' });
                    }
                });
            }

            var sigLabel = funcName + '(' + params.map(function (p) { return p.label; }).join(', ') + ')';
            var retStr = (retType && retType !== 'void') ? ('  \u2192 ' + retType) : '';
            methodSignatures['__user_' + funcName] = {
                label: sigLabel,
                documentation: '\uc0ac\uc6a9\uc790 \uc815\uc758 \ud568\uc218' + retStr,
                parameters: params
            };
        }
    }

    // Find the active function call at the cursor position, handling nested parentheses
    function findActiveCall(text) {
        var depth = 0;
        var commaCount = 0;
        var funcEnd = -1;

        // Walk backwards from the end
        for (var i = text.length - 1; i >= 0; i--) {
            var ch = text.charAt(i);
            if (ch === ')') {
                depth++;
            } else if (ch === '(') {
                if (depth > 0) {
                    depth--;
                } else {
                    funcEnd = i;
                    break;
                }
            } else if (ch === ',' && depth === 0) {
                commaCount++;
            }
        }

        if (funcEnd < 0) return null;

        // Extract function name before '('
        var before = text.substring(0, funcEnd);
        // Try @Method pattern first
        var atMatch = before.match(/@(\w[\w.]*)$/);
        if (atMatch) {
            return { name: atMatch[1], commaCount: commaCount, isBuiltin: true };
        }
        // Try user-defined function name
        var fnMatch = before.match(/(\w+)\s*$/);
        if (fnMatch) {
            return { name: fnMatch[1], commaCount: commaCount, isBuiltin: false };
        }

        return null;
    }

    monaco.languages.registerSignatureHelpProvider('autobase-script', {
        signatureHelpTriggerCharacters: ['(', ','],
        signatureHelpRetriggerCharacters: [','],

        provideSignatureHelp: function (model, position) {
            enrichSignatures();
            parseUserFunctions(model);

            // Build text from line start to cursor, handling multi-line calls
            var textBefore = '';
            for (var ln = 1; ln <= position.lineNumber; ln++) {
                var lineContent = model.getLineContent(ln);
                if (ln === position.lineNumber) {
                    textBefore += lineContent.substring(0, position.column - 1);
                } else {
                    textBefore += lineContent + ' ';
                }
            }

            var callInfo = findActiveCall(textBefore);
            if (!callInfo) return null;

            // Look up signature
            var sig = null;
            if (callInfo.isBuiltin) {
                sig = methodSignatures[callInfo.name];
            } else {
                // Try user-defined function
                sig = methodSignatures['__user_' + callInfo.name];
            }
            if (!sig) return null;

            return {
                value: {
                    signatures: [{
                        label: sig.label,
                        documentation: { value: sig.documentation },
                        parameters: sig.parameters.map(function (p) {
                            return {
                                label: p.label,
                                documentation: { value: p.documentation }
                            };
                        })
                    }],
                    activeSignature: 0,
                    activeParameter: Math.min(callInfo.commaCount, Math.max(sig.parameters.length - 1, 0))
                },
                dispose: function () { }
            };
        }
    });

    // =========================================================================
    // 2. Hover Provider — tag/method documentation on hover
    // =========================================================================

    // Tag member documentation
    var tagMemberDocs = {
        'value': { type: 'double/int', desc: '태그 현재 값' },
        'tag': { type: 'string', desc: '태그 식별자' },
        'name': { type: 'string', desc: '태그 표시 이름' },
        'des': { type: 'string', desc: '태그 설명' },
        'unit': { type: 'string', desc: '단위 (℃, MPa, % 등)' },
        'hihi': { type: 'double', desc: '상한 경보값 (High-High)' },
        'lolo': { type: 'double', desc: '하한 경보값 (Low-Low)' },
        'desON': { type: 'string', desc: 'ON 상태 설명' },
        'desOFF': { type: 'string', desc: 'OFF 상태 설명' },
        'port': { type: 'int', desc: '포트 번호' },
        'station': { type: 'int', desc: 'PLC 스테이션 번호' },
        'address': { type: 'string', desc: '주소' },
        'extra1': { type: 'string', desc: '확장 필드 1' },
        'extra2': { type: 'string', desc: '확장 필드 2' }
    };

    monaco.languages.registerHoverProvider('autobase-script', {
        provideHover: function (model, position) {
            var wordInfo = model.getWordAtPosition(position);
            if (!wordInfo) return null;

            var word = wordInfo.word;
            var lineContent = model.getLineContent(position.lineNumber);
            var textBefore = lineContent.substring(0, wordInfo.startColumn - 1);

            var range = {
                startLineNumber: position.lineNumber,
                startColumn: wordInfo.startColumn,
                endLineNumber: position.lineNumber,
                endColumn: wordInfo.endColumn
            };

            // Tag member hover: $Tag.member
            if (textBefore.match(/\$\w+\.$/)) {
                var memberDoc = tagMemberDocs[word];
                if (memberDoc) {
                    return {
                        range: range,
                        contents: [
                            { value: '**태그 멤버**: `.' + word + '`' },
                            { value: '타입: `' + memberDoc.type + '`\n\n' + memberDoc.desc }
                        ]
                    };
                }
            }

            // Tag hover: $TagName
            if (textBefore.match(/\$$/) || (textBefore.match(/\$\w*$/) && !textBefore.match(/\$\w+\./))) {
                var data = window.autobaseCompletionData;
                if (data && data.tags) {
                    var tagInfo = data.tags.find(function (t) { return t.name === word; });
                    if (tagInfo) {
                        var contents = [
                            { value: '**태그**: `$' + tagInfo.name + '`' },
                            { value: '타입: `' + (tagInfo.type || 'Unknown') + '`\n\n' + (tagInfo.description || '') }
                        ];
                        contents.push({ value: '멤버: `.value` `.tag` `.name` `.des` `.unit` `.hihi` `.lolo`' });
                        return { range: range, contents: contents };
                    }
                }
            }

            // Method hover: @MethodName
            if (textBefore.match(/@$/)) {
                enrichSignatures();
                var sig = methodSignatures[word];
                if (sig) {
                    return {
                        range: range,
                        contents: [
                            { value: '**메서드**: `' + sig.label + '`' },
                            { value: sig.documentation }
                        ]
                    };
                }
            }

            return null;
        }
    });

    // =========================================================================
    // 3. Real-time Diagnostics — validate on content change
    // =========================================================================

    function setupDiagnostics(editor) {
        editor.onDidChangeModelContent(function () {
            if (diagnosticTimer) clearTimeout(diagnosticTimer);
            diagnosticTimer = setTimeout(function () {
                runLocalDiagnostics(editor);
            }, 800);  // 800ms debounce
        });
    }

    function runLocalDiagnostics(editor) {
        var model = editor.getModel();
        if (!model) return;

        var code = model.getValue();
        if (code === lastValidatedContent) return;
        lastValidatedContent = code;

        var markers = [];
        var lines = code.split('\n');
        var knownTags = {};

        // Build tag set from completion data
        var data = window.autobaseCompletionData;
        if (data && data.tags) {
            data.tags.forEach(function (t) { knownTags[t.name] = true; });
        }

        for (var i = 0; i < lines.length; i++) {
            var line = lines[i];
            var trimmed = line.trim();

            // Skip empty lines and comments
            if (!trimmed || trimmed.indexOf('//') === 0 || trimmed.indexOf('/*') === 0) continue;

            // Check unknown tag references (only if we have tag data)
            if (Object.keys(knownTags).length > 0) {
                var tagPattern = /\$(\w+)/g;
                var tagMatch;
                while ((tagMatch = tagPattern.exec(line)) !== null) {
                    if (!knownTags[tagMatch[1]]) {
                        markers.push({
                            severity: monaco.MarkerSeverity.Error,
                            message: '존재하지 않는 태그: $' + tagMatch[1],
                            startLineNumber: i + 1,
                            startColumn: tagMatch.index + 1,
                            endLineNumber: i + 1,
                            endColumn: tagMatch.index + tagMatch[0].length + 1
                        });
                    }
                }
            }

            // Check assignment in condition
            var condMatch = trimmed.match(/^(if|while)\s*\((.+)\)/);
            if (condMatch) {
                var cond = condMatch[2];
                if (cond.indexOf('=') !== -1 && cond.indexOf('==') === -1 &&
                    cond.indexOf('!=') === -1 && cond.indexOf('>=') === -1 &&
                    cond.indexOf('<=') === -1) {
                    markers.push({
                        severity: monaco.MarkerSeverity.Warning,
                        message: "'=='(비교)를 의도하셨나요? ('='는 대입 연산자)",
                        startLineNumber: i + 1,
                        startColumn: line.indexOf('=') + 1,
                        endLineNumber: i + 1,
                        endColumn: line.indexOf('=') + 2
                    });
                }
            }

            // Check bit shift vs comparison
            var shiftIdx = line.indexOf('>>');
            if (shiftIdx !== -1 && line.indexOf('>>=') === -1) {
                markers.push({
                    severity: monaco.MarkerSeverity.Warning,
                    message: "비교 연산자 '>'를 의도하셨나요? ('>>'는 비트 시프트)",
                    startLineNumber: i + 1,
                    startColumn: shiftIdx + 1,
                    endLineNumber: i + 1,
                    endColumn: shiftIdx + 3
                });
            }
        }

        // Check bracket balance
        var openBraces = (code.match(/\{/g) || []).length;
        var closeBraces = (code.match(/\}/g) || []).length;
        if (openBraces !== closeBraces) {
            markers.push({
                severity: monaco.MarkerSeverity.Error,
                message: '중괄호 불균형: 열기 ' + openBraces + '개, 닫기 ' + closeBraces + '개',
                startLineNumber: lines.length,
                startColumn: 1,
                endLineNumber: lines.length,
                endColumn: (lines[lines.length - 1] || '').length + 1
            });
        }

        monaco.editor.setModelMarkers(model, 'autobase-diagnostics', markers);
    }

    // =========================================================================
    // 4. Code Action Provider — Quick Fix suggestions
    // =========================================================================

    monaco.languages.registerCodeActionProvider('autobase-script', {
        provideCodeActions: function (model, range, context) {
            var actions = [];
            var markers = context.markers || [];

            markers.forEach(function (marker) {
                // Unknown tag → suggest similar tags
                if (marker.message && marker.message.indexOf('존재하지 않는 태그') !== -1) {
                    var tagMatch = marker.message.match(/\$(\w+)/);
                    if (tagMatch) {
                        var unknownTag = tagMatch[1];
                        var data = window.autobaseCompletionData;
                        if (data && data.tags) {
                            // Find similar tags
                            var similar = data.tags.filter(function (t) {
                                return t.name.toLowerCase().indexOf(unknownTag.toLowerCase().substring(0, 3)) !== -1;
                            }).slice(0, 3);

                            similar.forEach(function (t) {
                                actions.push({
                                    title: '$' + unknownTag + ' → $' + t.name + ' 로 변경',
                                    kind: 'quickfix',
                                    diagnostics: [marker],
                                    edit: {
                                        edits: [{
                                            resource: model.uri,
                                            textEdit: {
                                                range: {
                                                    startLineNumber: marker.startLineNumber,
                                                    startColumn: marker.startColumn,
                                                    endLineNumber: marker.endLineNumber,
                                                    endColumn: marker.endColumn
                                                },
                                                text: '$' + t.name
                                            },
                                            versionId: model.getVersionId()
                                        }]
                                    }
                                });
                            });
                        }
                    }
                }

                // Assignment in condition → suggest ==
                if (marker.message && marker.message.indexOf("'=='") !== -1) {
                    var line = model.getLineContent(marker.startLineNumber);
                    var fixedLine = line.substring(0, marker.startColumn - 1) + '==' +
                        line.substring(marker.endColumn - 1);
                    actions.push({
                        title: "'=' → '==' 비교 연산자로 변경",
                        kind: 'quickfix',
                        diagnostics: [marker],
                        edit: {
                            edits: [{
                                resource: model.uri,
                                textEdit: {
                                    range: {
                                        startLineNumber: marker.startLineNumber,
                                        startColumn: marker.startColumn,
                                        endLineNumber: marker.endLineNumber,
                                        endColumn: marker.endColumn
                                    },
                                    text: '=='
                                },
                                versionId: model.getVersionId()
                            }]
                        }
                    });
                }
            });

            return { actions: actions, dispose: function () { } };
        }
    });

    // =========================================================================
    // 5. AI Code Generation Command (Ctrl+I)
    // =========================================================================

    function registerAiGenerateCommand(editor) {
        editor.addAction({
            id: 'autobase.aiGenerate',
            label: 'AI 코드 생성 (Ctrl+I)',
            keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyCode.KeyI],
            contextMenuGroupId: 'ai',
            contextMenuOrder: 1,

            run: function (ed) {
                // Show input dialog using Monaco's quickInput
                var inputWidget = document.getElementById('ai-input-widget');
                if (inputWidget) {
                    inputWidget.style.display = 'flex';
                    var inputField = document.getElementById('ai-input-field');
                    if (inputField) {
                        inputField.value = '';
                        inputField.focus();
                    }
                    return;
                }

                // Create input widget overlay
                createAiInputWidget(ed);
            }
        });

        // Also add explain action
        editor.addAction({
            id: 'autobase.aiExplain',
            label: 'AI 코드 설명',
            contextMenuGroupId: 'ai',
            contextMenuOrder: 2,
            precondition: 'editorHasSelection',

            run: function (ed) {
                var selection = ed.getSelection();
                var text = ed.getModel().getValueInRange(selection);
                if (!text) return;

                // Send explain request to C#
                postMessage({
                    type: 'aiAssistRequest',
                    requestId: generateId(),
                    text: '다음 코드를 설명해주세요:\n' + text,
                    content: ed.getValue()
                });
            }
        });
    }

    function createAiInputWidget(editor) {
        var container = document.createElement('div');
        container.id = 'ai-input-widget';
        container.style.cssText = 'display:flex;position:fixed;top:0;left:0;right:0;z-index:10000;' +
            'background:#2d2d2d;border-bottom:2px solid #007acc;padding:8px 12px;align-items:center;gap:8px;';

        var label = document.createElement('span');
        label.textContent = 'AI';
        label.style.cssText = 'color:#007acc;font-weight:bold;font-size:13px;';

        var input = document.createElement('input');
        input.id = 'ai-input-field';
        input.type = 'text';
        input.placeholder = '무엇을 구현하시겠습니까? (예: 온도 상한 초과 시 알람 발생)';
        input.style.cssText = 'flex:1;background:#1e1e1e;color:#d4d4d4;border:1px solid #3c3c3c;' +
            'padding:6px 10px;font-size:13px;font-family:inherit;outline:none;border-radius:3px;';

        var btnGenerate = document.createElement('button');
        btnGenerate.textContent = '생성';
        btnGenerate.style.cssText = 'background:#007acc;color:white;border:none;padding:6px 16px;' +
            'cursor:pointer;font-size:13px;border-radius:3px;';

        var btnCancel = document.createElement('button');
        btnCancel.textContent = '취소';
        btnCancel.style.cssText = 'background:#3c3c3c;color:#d4d4d4;border:none;padding:6px 12px;' +
            'cursor:pointer;font-size:13px;border-radius:3px;';

        function closeWidget() {
            container.style.display = 'none';
            editor.focus();
        }

        function submitRequest() {
            var prompt = input.value.trim();
            if (!prompt) return;

            var requestId = generateId();
            pendingRequests[requestId] = function (result) {
                if (result && result.code) {
                    // Insert generated code at cursor
                    var position = editor.getPosition();
                    var selection = editor.getSelection();
                    editor.executeEdits('ai-assist', [{
                        range: selection,
                        text: result.code,
                        forceMoveMarkers: true
                    }]);
                }
                closeWidget();
            };

            // Show loading state
            btnGenerate.textContent = '생성 중...';
            btnGenerate.disabled = true;

            postMessage({
                type: 'aiAssistRequest',
                requestId: requestId,
                text: prompt,
                content: editor.getValue()
            });

            // Timeout fallback
            setTimeout(function () {
                if (pendingRequests[requestId]) {
                    delete pendingRequests[requestId];
                    btnGenerate.textContent = '생성';
                    btnGenerate.disabled = false;
                }
            }, 10000);
        }

        input.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') submitRequest();
            if (e.key === 'Escape') closeWidget();
        });
        btnGenerate.addEventListener('click', submitRequest);
        btnCancel.addEventListener('click', closeWidget);

        container.appendChild(label);
        container.appendChild(input);
        container.appendChild(btnGenerate);
        container.appendChild(btnCancel);
        document.body.appendChild(container);
        input.focus();
    }

    // =========================================================================
    // 6. SCADA-specific Snippets (extended)
    // =========================================================================

    var scadaSnippets = [
        {
            label: 'alarm-check',
            kind: monaco.languages.CompletionItemKind.Snippet,
            insertText: '// 알람 체크\ndouble ${1:val} = \\$${2:Tag}.value;\nif (${1:val} > \\$${2:Tag}.hihi)\n{\n\t@SetAlarm("${2:Tag}_HI");\n}\nelse if (${1:val} < \\$${2:Tag}.lolo)\n{\n\t@SetAlarm("${2:Tag}_LO");\n}',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: '아날로그 태그의 상/하한 알람 체크',
            detail: 'SCADA 알람 패턴'
        },
        {
            label: 'toggle-do',
            kind: monaco.languages.CompletionItemKind.Snippet,
            insertText: '// 디지털 출력 토글\nif (\\$${1:Tag}.value == 0)\n\t\\$${1:Tag}.value = 1;\nelse\n\t\\$${1:Tag}.value = 0;',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: '디지털 출력 토글 (ON/OFF)',
            detail: 'SCADA 제어 패턴'
        },
        {
            label: 'log-db',
            kind: monaco.languages.CompletionItemKind.Snippet,
            insertText: '// DB 기록\n@LogToDatabase("${1:Tag}", \\$${1:Tag}.value, DateTime.Now);',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: '태그 값을 데이터베이스에 기록',
            detail: 'SCADA 기록 패턴'
        },
        {
            label: 'interlock',
            kind: monaco.languages.CompletionItemKind.Snippet,
            insertText: '// 인터록 제어\nif (\\$${1:InterlockTag}.value == 1)\n{\n\t\\$${2:OutputTag}.value = ${3:1};\n\t@LogEvent("${2:OutputTag} 출력 - 인터록 OK");\n}\nelse\n{\n\t\\$${2:OutputTag}.value = 0;\n\t@SetAlarm("${2:OutputTag}_INTERLOCK");\n}',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: '인터록 조건 확인 후 출력 제어',
            detail: 'SCADA 인터록 패턴'
        },
        {
            label: 'python-ai',
            kind: monaco.languages.CompletionItemKind.Snippet,
            insertText: '// Python AI Engine 호출\nstring result = @PythonAiCall("${1:predict/power}", "{\\"current_kw\\": " + \\$${2:Tag}.value + "}");\n@LogEvent("AI 결과: " + result);',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: 'Python AI Engine 서비스 호출',
            detail: 'AI Engine 연동'
        }
    ];

    // Register SCADA snippets as additional completion items
    var originalProvider = null;

    monaco.languages.registerCompletionItemProvider('autobase-script', {
        triggerCharacters: [],
        provideCompletionItems: function (model, position) {
            var word = model.getWordUntilPosition(position);
            var range = {
                startLineNumber: position.lineNumber,
                endLineNumber: position.lineNumber,
                startColumn: word.startColumn,
                endColumn: word.endColumn
            };

            var suggestions = scadaSnippets.map(function (sn) {
                return {
                    label: sn.label,
                    kind: sn.kind,
                    insertText: sn.insertText,
                    insertTextRules: sn.insertTextRules,
                    documentation: sn.documentation,
                    detail: sn.detail,
                    range: range
                };
            });

            return { suggestions: suggestions };
        }
    });

    // =========================================================================
    // 7. Initialize on editor ready
    // =========================================================================

    var origInit = window.initMonacoEditor;
    window.initMonacoEditor = function () {
        // Call original init
        if (origInit) origInit();

        // Setup AI features on the editor
        var editor = getEditor();
        if (editor) {
            setupDiagnostics(editor);
            registerAiGenerateCommand(editor);

            // Run initial diagnostics
            setTimeout(function () { runLocalDiagnostics(editor); }, 1000);
        }
    };
})();
