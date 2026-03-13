// Autobase Script Editor Theme for Monaco
// Matches the original TextArea color scheme

(function () {
    'use strict';

    monaco.editor.defineTheme('autobase-theme', {
        base: 'vs',
        inherit: true,
        rules: [
            { token: 'comment', foreground: '008000' },            // Color.Green
            { token: 'string', foreground: 'A31515' },             // Color.FromArgb(163, 21, 21)
            { token: 'string.quote', foreground: 'A31515' },
            { token: 'string.escape', foreground: 'A31515' },
            { token: 'string.invalid', foreground: 'A31515' },
            { token: 'keyword', foreground: '0000FF', fontStyle: '' },   // Color.Blue
            { token: 'type', foreground: '0000FF' },               // Type keywords also blue
            { token: 'constant', foreground: '0000FF' },           // true/false/null
            { token: 'number', foreground: '000000' },
            { token: 'number.float', foreground: '000000' },
            { token: 'number.hex', foreground: '000000' },
            { token: 'operator', foreground: '000000' },
            { token: 'delimiter', foreground: '000000' },
            { token: 'variable.predefined', foreground: '800080' }, // $Tag - purple
            { token: 'annotation', foreground: 'B22222' },          // @Method - dark red
            { token: 'identifier', foreground: '000000' },
            { token: '', foreground: '000000' }
        ],
        colors: {
            'editor.background': '#FFFFFF',
            'editor.foreground': '#000000',
            'editor.selectionBackground': '#3399FF80',
            'editor.lineHighlightBackground': '#00000008',
            'editorLineNumber.foreground': '#000000',
            'editorLineNumber.activeForeground': '#000000',
            'editorGutter.background': '#F0F0F0',
            'editorBracketMatch.background': '#ADD6FF80',
            'editorBracketMatch.border': '#0000FF40'
        }
    });
})();
