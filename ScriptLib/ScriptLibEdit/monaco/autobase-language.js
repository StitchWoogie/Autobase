// Autobase Script Language Definition for Monaco Editor
// C#-like DSL for SCADA automation scripting

(function () {
    'use strict';

    monaco.languages.register({ id: 'autobase-script' });

    monaco.languages.setLanguageConfiguration('autobase-script', {
        comments: {
            lineComment: '//',
            blockComment: ['/*', '*/']
        },
        brackets: [
            ['{', '}'],
            ['[', ']'],
            ['(', ')']
        ],
        autoClosingPairs: [
            { open: '{', close: '}' },
            { open: '[', close: ']' },
            { open: '(', close: ')' },
            { open: '"', close: '"', notIn: ['string'] },
            { open: "'", close: "'", notIn: ['string', 'comment'] }
        ],
        surroundingPairs: [
            { open: '{', close: '}' },
            { open: '[', close: ']' },
            { open: '(', close: ')' },
            { open: '"', close: '"' },
            { open: "'", close: "'" }
        ],
        folding: {
            markers: {
                start: /^\s*\/\/\s*#region\b/,
                end: /^\s*\/\/\s*#endregion\b/
            }
        },
        indentationRules: {
            increaseIndentPattern: /^.*\{[^}"']*$/,
            decreaseIndentPattern: /^\s*\}/
        },
        onEnterRules: [
            {
                beforeText: /^\s*\/\*\*(?!\/)([^*]|\*(?!\/))*$/,
                afterText: /^\s*\*\/$/,
                action: { indentAction: monaco.languages.IndentAction.IndentOutdent, appendText: ' * ' }
            },
            {
                beforeText: /^\s*\/\*\*(?!\/)([^*]|\*(?!\/))*$/,
                action: { indentAction: monaco.languages.IndentAction.None, appendText: ' * ' }
            },
            {
                beforeText: /^(\t|[ ])*[ ]\*([ ]([^*]|\*(?!\/))*)?$/,
                action: { indentAction: monaco.languages.IndentAction.None, appendText: '* ' }
            },
            {
                beforeText: /^(\t|[ ])*[ ]\*\/\s*$/,
                action: { indentAction: monaco.languages.IndentAction.None, removeText: 1 }
            }
        ]
    });

    monaco.languages.setMonarchTokensProvider('autobase-script', {
        keywords: [
            'using', 'namespace', 'class', 'struct', 'enum', 'partial',
            'public', 'static', 'protected', 'private', 'override', 'virtual',
            'new', 'extern', 'return', 'if', 'else', 'for', 'while',
            'break', 'continue', 'get', 'set', 'in', 'out', 'ref',
            'params', 'const', 'readonly', 'void'
        ],

        typeKeywords: [
            'bool', 'sbyte', 'byte', 'char', 'short', 'ushort',
            'int', 'uint', 'long', 'ulong', 'float', 'double',
            'string', 'object'
        ],

        constants: ['true', 'false', 'null'],

        operators: [
            '=', '>', '<', '!', '~', '?', ':',
            '==', '<=', '>=', '!=', '&&', '||',
            '++', '--', '+', '-', '*', '/', '&',
            '|', '^', '%', '<<', '>>', '+=', '-=',
            '*=', '/=', '&=', '|=', '^=', '%=',
            '<<=', '>>='
        ],

        symbols: /[=><!~?:&|+\-*\/\^%]+/,

        escapes: /\\(?:[abfnrtv\\"']|x[0-9A-Fa-f]{1,4}|u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8})/,

        tokenizer: {
            root: [
                // Tag references ($TagName, $Group.TagName.member)
                [/\$[\w.]+/, 'variable.predefined'],

                // Method references (@MethodName, @Group.Method)
                [/@@[\w.]+/, 'annotation'],

                // Identifiers and keywords
                [/[a-zA-Z_]\w*/, {
                    cases: {
                        '@typeKeywords': 'type',
                        '@keywords': 'keyword',
                        '@constants': 'constant',
                        '@default': 'identifier'
                    }
                }],

                // Whitespace
                { include: '@whitespace' },

                // Delimiters and operators
                [/[{}()\[\]]/, '@brackets'],
                [/@symbols/, {
                    cases: {
                        '@operators': 'operator',
                        '@default': ''
                    }
                }],

                // Numbers
                [/\d*\.\d+([eE][\-+]?\d+)?[fFdDmM]?/, 'number.float'],
                [/0[xX][0-9a-fA-F]+[lLuU]*/, 'number.hex'],
                [/\d+[lLuUfFdDmM]?/, 'number'],

                // Delimiter
                [/[;,.]/, 'delimiter'],

                // Strings
                [/"([^"\\]|\\.)*$/, 'string.invalid'],  // non-terminated string
                [/"/, { token: 'string.quote', bracket: '@open', next: '@string' }],

                // Characters
                [/'[^\\']'/, 'string'],
                [/(')(@escapes)(')/, ['string', 'string.escape', 'string']],
                [/'/, 'string.invalid']
            ],

            comment: [
                [/[^\/*]+/, 'comment'],
                [/\/\*/, 'comment', '@push'],
                ['\\*/', 'comment', '@pop'],
                [/[\/*]/, 'comment']
            ],

            string: [
                [/[^\\"]+/, 'string'],
                [/@escapes/, 'string.escape'],
                [/\\./, 'string.escape.invalid'],
                [/"/, { token: 'string.quote', bracket: '@close', next: '@pop' }]
            ],

            whitespace: [
                [/[ \t\r\n]+/, 'white'],
                [/\/\*/, 'comment', '@comment'],
                [/\/\/.*$/, 'comment']
            ]
        }
    });
})();
