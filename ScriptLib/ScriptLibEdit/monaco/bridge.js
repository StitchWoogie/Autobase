// .NET <-> Monaco Editor Bridge
// Handles communication between WebView2 host and Monaco Editor instance

(function () {
    'use strict';

    var editor = null;
    var breakpointDecorations = [];
    var debugLineDecoration = [];
    var breakpointLines = new Set();
    var isInsertMode = true;
    var pendingContentOnReady = null;

    // --- Editor Initialization ---

    function initEditor() {
        editor = monaco.editor.create(document.getElementById('editor'), {
            language: 'autobase-script',
            theme: 'autobase-theme',
            automaticLayout: true,
            glyphMargin: true,
            minimap: { enabled: true },
            folding: true,
            lineNumbers: 'on',
            renderWhitespace: 'none',
            wordWrap: 'off',
            scrollBeyondLastLine: false,
            fontSize: 14,
            fontFamily: 'DotumChe, Consolas, "Courier New", monospace',
            insertSpaces: true,
            tabSize: 4,
            cursorBlinking: 'blink',
            cursorStyle: 'line',
            mouseWheelZoom: true,
            bracketPairColorization: { enabled: true },
            guides: {
                bracketPairs: true,
                indentation: true
            },
            suggest: {
                showKeywords: true,
                showSnippets: true
            },
            quickSuggestions: {
                other: true,
                comments: false,
                strings: false
            }
        });

        // --- Event Handlers ---

        // Cursor position changed
        editor.onDidChangeCursorPosition(function (e) {
            postMessage({
                type: 'cursorChanged',
                line: e.position.lineNumber - 1,    // 0-based for .NET
                column: e.position.column - 1        // 0-based for .NET
            });
        });

        // Content changed
        var contentChangeTimer = null;
        editor.onDidChangeModelContent(function () {
            if (contentChangeTimer) clearTimeout(contentChangeTimer);
            contentChangeTimer = setTimeout(function () {
                var model = editor.getModel();
                postMessage({
                    type: 'contentChanged',
                    isDirty: true,
                    canUndo: (model._undoStack && model._undoStack.getElements().length > 0) || true,
                    canRedo: false,
                    hasSelection: !editor.getSelection().isEmpty()
                });
            }, 50);
        });

        // Selection changed
        editor.onDidChangeCursorSelection(function (e) {
            postMessage({
                type: 'selectionChanged',
                hasSelection: !e.selection.isEmpty()
            });
        });

        // Mouse down on glyph margin -> toggle breakpoint
        editor.onMouseDown(function (e) {
            if (e.target.type === monaco.editor.MouseTargetType.GUTTER_GLYPH_MARGIN ||
                e.target.type === monaco.editor.MouseTargetType.GUTTER_LINE_NUMBERS) {
                if (e.target.position) {
                    var line = e.target.position.lineNumber - 1; // 0-based
                    toggleBreakpointVisual(line);
                    postMessage({
                        type: 'breakpointToggled',
                        line: line
                    });
                }
            }
        });

        // Overwrite mode toggle (Insert key)
        editor.onKeyDown(function (e) {
            if (e.keyCode === monaco.KeyCode.Insert) {
                isInsertMode = !isInsertMode;
                editor.updateOptions({
                    cursorStyle: isInsertMode ? 'line' : 'block'
                });
                postMessage({
                    type: 'insertModeChanged',
                    isInsert: isInsertMode
                });
            }
        });

        // Load pending content if set before editor was ready
        if (pendingContentOnReady !== null) {
            editor.setValue(pendingContentOnReady);
            pendingContentOnReady = null;
        }

        // Notify .NET that editor is ready
        postMessage({ type: 'editorReady' });
    }

    // --- Helper Functions ---

    function postMessage(msg) {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(JSON.stringify(msg));
        }
    }

    function toggleBreakpointVisual(line0based) {
        if (breakpointLines.has(line0based)) {
            breakpointLines.delete(line0based);
        } else {
            breakpointLines.add(line0based);
        }
        updateBreakpointDecorations();
    }

    function updateBreakpointDecorations() {
        var decorations = [];
        breakpointLines.forEach(function (line0) {
            decorations.push({
                range: new monaco.Range(line0 + 1, 1, line0 + 1, 1),
                options: {
                    isWholeLine: true,
                    glyphMarginClassName: 'breakpoint-glyph',
                    glyphMarginHoverMessage: { value: 'Breakpoint' }
                }
            });
        });
        breakpointDecorations = editor.deltaDecorations(breakpointDecorations, decorations);
    }

    // --- Bridge API (called from .NET via ExecuteScriptAsync) ---

    window.bridge = {
        // Set editor content
        setContent: function (text) {
            if (!editor) {
                pendingContentOnReady = text;
                return;
            }
            editor.setValue(text);
            // Reset dirty state after loading
            postMessage({
                type: 'contentChanged',
                isDirty: false,
                canUndo: false,
                canRedo: false,
                hasSelection: false
            });
        },

        // Get editor content (async via message)
        getContent: function (requestId) {
            if (!editor) {
                postMessage({ type: 'contentResponse', requestId: requestId, content: '' });
                return;
            }
            postMessage({
                type: 'contentResponse',
                requestId: requestId,
                content: editor.getValue()
            });
        },

        // Get selected text (async via message)
        getSelectedText: function (requestId) {
            if (!editor) {
                postMessage({ type: 'selectedTextResponse', requestId: requestId, text: '' });
                return;
            }
            var selection = editor.getSelection();
            var text = editor.getModel().getValueInRange(selection);
            postMessage({
                type: 'selectedTextResponse',
                requestId: requestId,
                text: text
            });
        },

        // Get cursor position as absolute character offset (async via message)
        getCursorPosition: function (requestId) {
            if (!editor) {
                postMessage({ type: 'cursorPositionResponse', requestId: requestId, position: 0 });
                return;
            }
            var pos = editor.getPosition();
            var model = editor.getModel();
            var offset = model.getOffsetAt(pos);
            postMessage({
                type: 'cursorPositionResponse',
                requestId: requestId,
                position: offset
            });
        },

        // Set breakpoints (array of 0-based line numbers)
        setBreakpoints: function (lines) {
            breakpointLines.clear();
            if (lines && lines.length) {
                lines.forEach(function (l) { breakpointLines.add(l); });
            }
            updateBreakpointDecorations();
        },

        // Set debug current line highlight (-1 to clear)
        setDebugLine: function (line0based) {
            if (line0based < 0) {
                debugLineDecoration = editor.deltaDecorations(debugLineDecoration, []);
                return;
            }
            debugLineDecoration = editor.deltaDecorations(debugLineDecoration, [{
                range: new monaco.Range(line0based + 1, 1, line0based + 1, 1),
                options: {
                    isWholeLine: true,
                    className: 'debug-line-highlight',
                    glyphMarginClassName: 'debug-arrow-glyph'
                }
            }]);
            // Also reveal the debug line
            editor.revealLineInCenter(line0based + 1);
        },

        // Navigate to position (0-based line, 0-based column)
        gotoPosition: function (line0, col0) {
            if (!editor) return;
            var pos = { lineNumber: line0 + 1, column: col0 + 1 };
            editor.setPosition(pos);
            editor.revealPositionInCenter(pos);
            editor.focus();
        },

        // Navigate to absolute character offset
        gotoOffset: function (offset) {
            if (!editor) return;
            var model = editor.getModel();
            var pos = model.getPositionAt(offset);
            editor.setPosition(pos);
            editor.revealPositionInCenter(pos);
            editor.focus();
        },

        // Set selection by absolute offset and length
        setSelection: function (startOffset, length) {
            if (!editor) return;
            var model = editor.getModel();
            var startPos = model.getPositionAt(startOffset);
            var endPos = model.getPositionAt(startOffset + length);
            editor.setSelection(new monaco.Range(
                startPos.lineNumber, startPos.column,
                endPos.lineNumber, endPos.column
            ));
            editor.revealRangeInCenter(editor.getSelection());
            editor.focus();
        },

        // Set font
        setFont: function (fontFamily, fontSize) {
            if (!editor) return;
            editor.updateOptions({
                fontFamily: fontFamily || 'DotumChe, Consolas, "Courier New", monospace',
                fontSize: fontSize || 14
            });
        },

        // Insert text at current cursor position (used by .NET Paste and InsertText)
        insertText: function (text) {
            if (!editor) return;
            var selection = editor.getSelection();
            var op = { range: selection, text: text, forceMoveMarkers: true };
            editor.executeEdits('bridge', [op]);
        },

        // Editor commands
        copy: function () { if (editor) editor.trigger('bridge', 'editor.action.clipboardCopyAction'); },
        paste: function (text) {
            if (!editor) return;
            if (text !== undefined && text !== null) {
                // Direct text insertion from .NET (bypasses browser clipboard)
                var selection = editor.getSelection();
                var op = { range: selection, text: text, forceMoveMarkers: true };
                editor.executeEdits('bridge', [op]);
            } else {
                editor.trigger('bridge', 'editor.action.clipboardPasteAction');
            }
        },
        cut: function () { if (editor) editor.trigger('bridge', 'editor.action.clipboardCutAction'); },
        selectAll: function () { if (editor) editor.trigger('bridge', 'editor.action.selectAll'); },
        undo: function () { if (editor) editor.trigger('bridge', 'undo'); },
        redo: function () { if (editor) editor.trigger('bridge', 'redo'); },
        deleteSelection: function () { if (editor) editor.trigger('bridge', 'deleteRight'); },

        // Find/Replace
        openFind: function () { if (editor) editor.trigger('bridge', 'actions.find'); },
        openReplace: function () { if (editor) editor.trigger('bridge', 'editor.action.startFindReplaceAction'); },

        // Read-only mode
        setReadOnly: function (readOnly) {
            if (!editor) return;
            editor.updateOptions({ readOnly: readOnly });
        },

        // Show/hide glyph margin (breakpoint zone)
        setGlyphMargin: function (show) {
            if (!editor) return;
            editor.updateOptions({ glyphMargin: show });
        },

        // Set IntelliSense completion data from .NET
        setCompletionData: function (data) {
            if (data) {
                window.autobaseCompletionData = data;
            }
        },

        // Force layout recalculation (called from .NET on resize)
        layout: function () {
            if (editor) editor.layout();
        },

        // Focus editor
        focus: function () {
            if (editor) editor.focus();
        },

        // Scroll to cursor
        scrollToCursor: function () {
            if (!editor) return;
            editor.revealPositionInCenter(editor.getPosition());
        },

        // Get editor state info (sync return for ExecuteScriptAsync)
        getEditorState: function () {
            if (!editor) return JSON.stringify({ canUndo: false, canRedo: false, hasSelection: false, isInsert: true });
            var model = editor.getModel();
            return JSON.stringify({
                canUndo: true, // Monaco always has undo support
                canRedo: true,
                hasSelection: !editor.getSelection().isEmpty(),
                isInsert: isInsertMode
            });
        }
    };

    // --- Startup ---
    window.initMonacoEditor = initEditor;
})();
