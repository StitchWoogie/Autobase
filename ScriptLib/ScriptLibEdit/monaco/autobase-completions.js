// Autobase Script IntelliSense Completion Provider for Monaco Editor

(function () {
    'use strict';

    // External data populated from .NET side
    window.autobaseCompletionData = {
        externalFunctions: [],
        externalClasses: [],
        snippets: [],
        tags: [],       // [{name, type, description}] - populated by MonacoEditorBridge
        methods: []     // [{name, returnType, signature, description}] - populated by MonacoEditorBridge
    };

    // PCL-style script keywords (procedural only)
    var keywords = [
        'return', 'if', 'else', 'for', 'while',
        'break', 'continue', 'const'
    ];

    var typeKeywords = [
        'bool', 'sbyte', 'byte', 'char', 'short', 'ushort',
        'int', 'uint', 'long', 'ulong', 'float', 'double',
        'string', 'object'
    ];

    var constants = ['true', 'false', 'null'];

    // Tag member completions (after $Tag.)
    var tagMembers = [
        { name: 'value', description: 'Tag value' },
        { name: 'tag', description: 'Tag identifier' },
        { name: 'name', description: 'Display name' },
        { name: 'des', description: 'Description' },
        { name: 'unit', description: 'Unit' },
        { name: 'desON', description: 'ON state description' },
        { name: 'desOFF', description: 'OFF state description' },
        { name: 'port', description: 'Port number' },
        { name: 'station', description: 'PLC station' },
        { name: 'address', description: 'Address' },
        { name: 'extra1', description: 'Extra field 1' },
        { name: 'extra2', description: 'Extra field 2' }
    ];

    // Built-in snippets
    var defaultSnippets = [
        {
            label: 'if',
            kind: monaco.languages.CompletionItemKind.Snippet,
            insertText: 'if (${1:condition})\n{\n\t${2}\n}',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: 'if statement'
        },
        {
            label: 'ifelse',
            kind: monaco.languages.CompletionItemKind.Snippet,
            insertText: 'if (${1:condition})\n{\n\t${2}\n}\nelse\n{\n\t${3}\n}',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: 'if-else statement'
        },
        {
            label: 'for',
            kind: monaco.languages.CompletionItemKind.Snippet,
            insertText: 'for (int ${1:i} = 0; ${1:i} < ${2:count}; ${1:i}++)\n{\n\t${3}\n}',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: 'for loop'
        },
        {
            label: 'while',
            kind: monaco.languages.CompletionItemKind.Snippet,
            insertText: 'while (${1:condition})\n{\n\t${2}\n}',
            insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
            documentation: 'while loop'
        }
    ];

    // Helper: get text before cursor on current line
    function getTextBeforeCursor(model, position) {
        return model.getValueInRange({
            startLineNumber: position.lineNumber,
            startColumn: 1,
            endLineNumber: position.lineNumber,
            endColumn: position.column
        });
    }

    monaco.languages.registerCompletionItemProvider('autobase-script', {
        triggerCharacters: ['.', '$', '@'],

        provideCompletionItems: function (model, position) {
            var textBefore = getTextBeforeCursor(model, position);
            var word = model.getWordUntilPosition(position);
            var range = {
                startLineNumber: position.lineNumber,
                endLineNumber: position.lineNumber,
                startColumn: word.startColumn,
                endColumn: word.endColumn
            };

            var data = window.autobaseCompletionData;
            var suggestions = [];

            // --- $Tag. member access ---
            var tagMemberMatch = textBefore.match(/\$[\w.]*\.(\w*)$/);
            if (tagMemberMatch) {
                tagMembers.forEach(function (m) {
                    suggestions.push({
                        label: m.name,
                        kind: monaco.languages.CompletionItemKind.Property,
                        insertText: m.name,
                        documentation: m.description,
                        range: range
                    });
                });
                return { suggestions: suggestions };
            }

            // --- $ tag completion ---
            var tagMatch = textBefore.match(/\$(\w*)$/);
            if (tagMatch) {
                var tagRange = {
                    startLineNumber: position.lineNumber,
                    endLineNumber: position.lineNumber,
                    startColumn: position.column - tagMatch[1].length,
                    endColumn: position.column
                };

                if (data && data.tags) {
                    data.tags.forEach(function (tag) {
                        suggestions.push({
                            label: tag.name,
                            kind: monaco.languages.CompletionItemKind.Variable,
                            insertText: tag.name,
                            documentation: tag.description || '',
                            detail: tag.type || '',
                            range: tagRange
                        });
                    });
                }
                return { suggestions: suggestions };
            }

            // --- @ method completion ---
            var methodMatch = textBefore.match(/@([\w.]*)$/);
            if (methodMatch) {
                var methodRange = {
                    startLineNumber: position.lineNumber,
                    endLineNumber: position.lineNumber,
                    startColumn: position.column - methodMatch[1].length,
                    endColumn: position.column
                };

                if (data && data.methods) {
                    data.methods.forEach(function (m) {
                        suggestions.push({
                            label: m.name,
                            kind: monaco.languages.CompletionItemKind.Method,
                            insertText: m.name,
                            documentation: m.description || '',
                            detail: m.returnType ? (m.returnType + ' ' + m.signature) : m.signature,
                            range: methodRange
                        });
                    });
                }
                return { suggestions: suggestions };
            }

            // --- Normal completions ---

            // Keywords
            keywords.forEach(function (kw) {
                suggestions.push({
                    label: kw,
                    kind: monaco.languages.CompletionItemKind.Keyword,
                    insertText: kw,
                    range: range
                });
            });

            // Type keywords
            typeKeywords.forEach(function (t) {
                suggestions.push({
                    label: t,
                    kind: monaco.languages.CompletionItemKind.TypeParameter,
                    insertText: t,
                    range: range
                });
            });

            // Constants
            constants.forEach(function (c) {
                suggestions.push({
                    label: c,
                    kind: monaco.languages.CompletionItemKind.Constant,
                    insertText: c,
                    range: range
                });
            });

            // Snippets
            defaultSnippets.forEach(function (sn) {
                suggestions.push({
                    label: sn.label,
                    kind: sn.kind,
                    insertText: sn.insertText,
                    insertTextRules: sn.insertTextRules,
                    documentation: sn.documentation,
                    range: range
                });
            });

            // External functions from .NET
            if (data && data.externalFunctions) {
                data.externalFunctions.forEach(function (fn) {
                    suggestions.push({
                        label: fn.name,
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: fn.insertText || fn.name,
                        insertTextRules: fn.isSnippet ? monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet : undefined,
                        documentation: fn.description || '',
                        detail: fn.signature || '',
                        range: range
                    });
                });
            }

            if (data && data.externalClasses) {
                data.externalClasses.forEach(function (cls) {
                    suggestions.push({
                        label: cls.name,
                        kind: monaco.languages.CompletionItemKind.Class,
                        insertText: cls.name,
                        documentation: cls.description || '',
                        range: range
                    });
                });
            }

            // User-provided snippets from .NET
            if (data && data.snippets) {
                data.snippets.forEach(function (sn) {
                    suggestions.push({
                        label: sn.label,
                        kind: monaco.languages.CompletionItemKind.Snippet,
                        insertText: sn.insertText,
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        documentation: sn.documentation || '',
                        range: range
                    });
                });
            }

            return { suggestions: suggestions };
        }
    });
})();
