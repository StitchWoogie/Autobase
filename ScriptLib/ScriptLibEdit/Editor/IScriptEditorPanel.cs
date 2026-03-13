using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ScriptLibEdit.Editor
{
    /// <summary>
    /// TextArea와 MonacoEditorBridge가 공유하는 스크립트 편집기 인터페이스.
    /// UserControlScriptEditor가 이 인터페이스를 통해 편집기와 통신한다.
    /// </summary>
    public interface IScriptEditorPanel
    {
        // --- Properties ---
        string sFilename { get; set; }
        bool bChangeFlag { get; set; }
        bool bInsertMode { get; }
        bool bDisplayBreakPointZone { get; set; }
        int ViewCursorX { get; }
        int ViewCursorY { get; }
        List<StringBuilder> arrayString { get; }

        // --- File Operations ---
        void LoadFromFile(string filename);
        void LoadFromString(string source);
        bool Save(string filename);
        string GetSourceString();

        // --- Clipboard ---
        void Copy();
        void Paste();
        void EditCut();
        void SelectAll();

        // --- Undo/Redo ---
        void Undo();
        void Redo();
        bool IsPosibleUndo();
        bool IsPosibleRedo();
        bool IsPosibleCopy();
        bool IsPosiblePaste();

        // --- Edit ---
        void KeyDownDelete();

        // --- Font/Config ---
        void SetFont(Font font);
        void OnConfigurationChanged();

        // --- Navigation ---
        void GotoViewCursor(int x, int y);
        void GotoBreakPoint(int x, int y);
        void GotoPosition(int position);
        void ScrollToCursor();

        // --- Selection ---
        string GetSelectedText();
        int GetCursorPosition();
        void SetSelection(int start, int length);

        // --- Debug ---
        void SetDebugBreakPointByCursor();
        bool IsDebugStarted();
        void DebugStop();
        void DebugStepInto();
        void DebugStepOver();
        void DebugStepContinue();

        // --- Layout ---
        void OnSize();
        void OnTimer();
    }
}
