using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScriptLibEdit.Editor
{
    

    partial class TextArea : Control
    {
        List<UndoItem> arrayUndo = new List<UndoItem>();
        int nUndoPos = 0;

        void UndoSave_AddPublic(UndoItem item)
        {
            if (!bChangeFlag)
            {
                bChangeFlag = true; // Undo가 있다는 것은 파일이 변경되었다는 뜻.
                parentEditor.SendEvent(EnumEditorEventType.SourceModified);
            }

            item.x = nCursorX;
            item.y = nCursorY;

            if (nUndoPos < arrayUndo.Count)
            {
                // 현재위치 이후의 Undo는 모두 삭제한다.
                arrayUndo.RemoveRange(nUndoPos, arrayUndo.Count - nUndoPos);
            }

            arrayUndo.Add(item);

            nUndoPos = arrayUndo.Count;
        }

        void UndoSave_InsertChar(int x, int y, char ch)
        {
            UndoItem_InsertChar item = new UndoItem_InsertChar();

            item.ch = ch;

            UndoSave_AddPublic(item);
        }

        void UndoSave_ReplaceChar(int x, int y, char old_ch, char new_ch)
        {
            UndoItem_ReplaceChar item = new UndoItem_ReplaceChar();

            item.old_ch = old_ch;
            item.new_ch = new_ch;

            UndoSave_AddPublic(item);
        }

        void UndoSave_AddReturn(string olds)
        {
            UndoItem_AddReturn item = new UndoItem_AddReturn();

            item.old_string = olds;

            UndoSave_AddPublic(item);
        }

        void UndoSave_DeleteSelection(int x1, int y1, int x2, int y2)
        {
            UndoItem_DeleteSelection item = new UndoItem_DeleteSelection();

            item.x1 = x1;
            item.y1 = y1;
            item.x2 = x2;
            item.y2 = y2;

            for (int pos = y1; pos <= y2; pos++)
            {
                item.arrayString.Add(GetOneLineString(pos).ToString());
            }

            UndoSave_AddPublic(item);
        }

        void UndoSave_Paste(string paste_string)
        {
            UndoItem_Paste item = new UndoItem_Paste();

            StringBuilder s = GetOneLineString(nCursorY);

            item.paste_string = paste_string;
            item.old_string = s.ToString();
            item.count_newline = 0;

            for (int i = 0; i < paste_string.Length; i++)
            {
                if (paste_string[i] == '\n')
                {
                    item.count_newline++;
                }
            }

            UndoSave_AddPublic(item);
        }

        void UndoSave_DeleteOneLine()
        {
            UndoItem_DeleteOneLine item = new UndoItem_DeleteOneLine();

            StringBuilder s = GetOneLineString(nCursorY);

            item.old_string = s.ToString();

            UndoSave_AddPublic(item);
        }

        void UndoSave_DeleteOneChar(char ch, bool backspace)
        {
            UndoItem_DeleteOneChar item = new UndoItem_DeleteOneChar();

            item.ch = ch;
            item.bBackSpace = backspace;

            UndoSave_AddPublic(item);
        }

        void UndoSave_AddTwoLine(string line1, string line2, string result, bool bline1, int rx, int ry)
        {
            UndoItem_AddTwoLine item = new UndoItem_AddTwoLine();

            item.line1 = line1;
            item.line2 = line2;
            item.result = result;
            item.bLine1 = bline1;
            item.resultx = rx;
            item.resulty = ry;

            UndoSave_AddPublic(item);
        }

        void UndoSave_DeleteString(string delete_string)
        {
            UndoItem_DeleteString item = new UndoItem_DeleteString();

            item.delete_string = delete_string;

            UndoSave_AddPublic(item);
        }

        void UndoSave_ReplaceStrings(int starty, List<string> strings)
        {
            UndoItem_ReplaceStrings item = new UndoItem_ReplaceStrings();

            item.start_y = starty;
            item.strings = strings;

            UndoSave_AddPublic(item);
        }

        public bool IsPosibleUndo()
        {
            if (arrayUndo.Count == 0) return false;

            if (nUndoPos <= 0) return false;

            return true;
        }

        public void Undo()
        {
            if (!IsPosibleUndo()) return;

            nUndoPos--;

            UndoItem item = arrayUndo[nUndoPos];

            if (item.GetType() == typeof(UndoItem_InsertChar))
            {
                UndoItem_InsertChar ui = (UndoItem_InsertChar)item;
                StringBuilder s = GetOneLineString(item.y);
                s.Remove(item.x, 1);
                CursorX = item.x;
                CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_ReplaceChar))
            {
                UndoItem_ReplaceChar ui = (UndoItem_ReplaceChar)item;
                StringBuilder s = GetOneLineString(item.y);
                s[item.x] = ui.old_ch;
                CursorX = item.x;
                CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_AddReturn))
            {
                UndoItem_AddReturn ui = (UndoItem_AddReturn)item;
                arrayString.RemoveAt(ui.y + 1);
                StringBuilder s = GetOneLineString(item.y);
                s.Remove(0, s.Length);
                s.Append(ui.old_string);
                CursorX = item.x;
                CursorY = item.y;

                ReCalcScrollVert();
                ReCalcScrollHorizon();
                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_DeleteSelection))
            {
                UndoItem_DeleteSelection ui = (UndoItem_DeleteSelection)item;

                // 선택 삭제는 해당되는 줄은 모두 보관되어 있으므로. (삭제된 줄수-1) 만큼 줄을 ui.y1에 추가하고
                // 해당되는 줄을 보관된 줄로 교체하면 된다. 2016-12-12
                StringBuilder s;
                for (int y = ui.y1; y < ui.y2; y++)
                {
                    arrayString.Insert(ui.y1, new StringBuilder());
                }

                for (int y = ui.y1, i = 0; y <= ui.y2; y++, i++)
                {
                    s = GetOneLineString(y);
                    s.Remove(0, s.Length);
                    s.Append(ui.arrayString[i]);
                }
                    /*
                    StringBuilder s;
                    for (int y = ui.y1, i = 0; y <= ui.y2; y++, i++)
                    {
                        if (y == ui.y2 || y == ui.y1)
                        {
                            s = GetOneLineString(y);
                            s.Remove(0, s.Length);
                            s.Append(ui.arrayString[i]);
                        }
                        else
                        {
                            s = new StringBuilder();
                            s.Append(ui.arrayString[i]);
                            arrayString.Insert(y, s);
                        }
                    }*/

                    CursorX = item.x;
                CursorY = item.y;

                bSelectionStart = true;
                nSelectionX1 = ui.x1;
                nSelectionY1 = ui.y1;
                nSelectionX2 = ui.x2;
                nSelectionY2 = ui.y2;

                ReCalcScrollVert();
                ReCalcScrollHorizon();
                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_Paste))
            {
                UndoItem_Paste ui = (UndoItem_Paste)item;

                StringBuilder s;

                for (int i = 0; i < ui.count_newline; i++)
                {
                    arrayString.RemoveAt(ui.y + 1);
                }

                s = GetOneLineString(ui.y);
                s.Remove(0, s.Length);
                s.Append(ui.old_string);

                CursorX = item.x;
                CursorY = item.y;

                ReCalcScrollVert();
                ReCalcScrollHorizon();
                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_DeleteOneLine))
            {
                UndoItem_DeleteOneLine ui = (UndoItem_DeleteOneLine)item;

                StringBuilder s = new StringBuilder();

                s.Append(ui.old_string);

                arrayString.Insert(ui.y, s);
                
                CursorX = item.x;
                CursorY = item.y;

                ReCalcScrollVert();
                ReCalcScrollHorizon();
                EnsureVisible();
                Invalidate();
            }

            else if (item.GetType() == typeof(UndoItem_DeleteOneChar))
            {
                UndoItem_DeleteOneChar ui = (UndoItem_DeleteOneChar)item;

                StringBuilder s = GetOneLineString(ui.y);

                if (ui.bBackSpace)
                {
                    s.Insert(ui.x - 1, ui.ch);
                }
                else
                {
                    s.Insert(ui.x, ui.ch);
                    
                }

                CursorX = item.x;
                CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }

            else if (item.GetType() == typeof(UndoItem_AddTwoLine))
            {
                UndoItem_AddTwoLine ui = (UndoItem_AddTwoLine)item;

                if (ui.bLine1)
                {
                    StringBuilder s = GetOneLineString(item.y);
                    s.Remove(0, s.Length);
                    s.Append(ui.line1);
                    s = new StringBuilder();
                    s.Append(ui.line2);
                    arrayString.Insert(item.y + 1, s);
                }
                else
                {
                    StringBuilder s = GetOneLineString(item.y-1);
                    s.Remove(0, s.Length);
                    s.Append(ui.line2);
                    s = new StringBuilder();
                    s.Append(ui.line1);
                    arrayString.Insert(item.y, s);
                }
                
                CursorX = item.x;
                CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }

            else if (item.GetType() == typeof(UndoItem_DeleteString))
            {
                UndoItem_DeleteString ui = (UndoItem_DeleteString)item;

                StringBuilder s = GetOneLineString(item.y);
                s.Insert(item.x - ui.delete_string.Length, ui.delete_string);

                CursorX = item.x;
                CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }

            else if (item.GetType() == typeof(UndoItem_ReplaceStrings))
            {
                UndoItem_ReplaceStrings ui = (UndoItem_ReplaceStrings)item;

                for (int i = 0, y=ui.start_y; i < ui.strings.Count; i++, y++)
                {
                    string temp = ui.strings[i];
                    StringBuilder s = GetOneLineString(y);
                    ui.strings[i] = s.ToString();
                    s.Remove(0, s.Length);
                    s.Append(temp);
                }

                CursorX = item.x;
                CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }

            MakeProperty();

            // 더이상 Undo할 목록이 없다는 것은 소소가 원형으로 복구되었다는 뜻
            if (nUndoPos <= 0)
            {
                bChangeFlag = false;
                parentEditor.SendEvent(EnumEditorEventType.SourceModified);
            }
        }

        public bool IsPosibleRedo()
        {
            if (arrayUndo.Count == 0) return false;

            if (nUndoPos >= arrayUndo.Count) return false;

            return true;
        }

        public void Redo()
        {
            if (!IsPosibleRedo()) return;

            UndoItem item = arrayUndo[nUndoPos];

            if (item.GetType() == typeof(UndoItem_InsertChar))
            {
                UndoItem_InsertChar ui = (UndoItem_InsertChar)item;
                StringBuilder s = GetOneLineString(item.y);
                s.Insert(item.x, (char)ui.ch);
                CursorX = item.x+1;
                CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_ReplaceChar))
            {
                UndoItem_ReplaceChar ui = (UndoItem_ReplaceChar)item;
                StringBuilder s = GetOneLineString(item.y);
                s[item.x] = ui.new_ch;
                CursorX = item.x+1;
                CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_AddReturn))
            {
                UndoItem_AddReturn ui = (UndoItem_AddReturn)item;

                CursorX = ui.x;
                CursorY = ui.y;

                OnlyKeyDownEnter();
            }
            else if (item.GetType() == typeof(UndoItem_DeleteSelection))
            {
                UndoItem_DeleteSelection ui = (UndoItem_DeleteSelection)item;

                OnlyDeleteSelection(ui.x1, ui.y1, ui.x2, ui.y2);
            }
            else if (item.GetType() == typeof(UndoItem_Paste))
            {
                UndoItem_Paste ui = (UndoItem_Paste)item;

                CursorX = ui.x;
                CursorY = ui.y;

                OnlyPaste(ui.paste_string);
            }
            else if (item.GetType() == typeof(UndoItem_DeleteOneLine))
            {
                UndoItem_DeleteOneLine ui = (UndoItem_DeleteOneLine)item;

                CursorX = ui.x;
                CursorY = ui.y;

                OnlyDeleteOneLine();
            }
            else if (item.GetType() == typeof(UndoItem_DeleteOneChar))
            {
                UndoItem_DeleteOneChar ui = (UndoItem_DeleteOneChar)item;

                if (ui.bBackSpace)
                {
                    StringBuilder s = GetOneLineString(ui.y);
                    s.Remove(ui.x - 1, 1);
                    CursorX = ui.x - 1;
                }
                else
                {
                    StringBuilder s = GetOneLineString(ui.y);
                    s.Remove(ui.x, 1);
                    CursorX = ui.x;
                }

                CursorY = ui.y;

                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_AddTwoLine))
            {
                UndoItem_AddTwoLine ui = (UndoItem_AddTwoLine)item;

                if (ui.bLine1)
                {
                    StringBuilder s = GetOneLineString(ui.y);
                    s.Remove(0, s.Length);
                    s.Append(ui.result);
                    arrayString.RemoveAt(ui.y + 1);
                }
                else
                {
                    StringBuilder s = GetOneLineString(ui.y);
                    s.Remove(0, s.Length);
                    s.Append(ui.result);
                    arrayString.RemoveAt(ui.y - 1);
                }

                CursorX = ui.resultx;
                CursorY = ui.resulty;

                ReCalcScrollVert();
                ReCalcScrollHorizon();
                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_DeleteString))
            {
                UndoItem_DeleteString ui = (UndoItem_DeleteString)item;

                StringBuilder s = GetOneLineString(item.y);

                s.Remove(item.x - ui.delete_string.Length, ui.delete_string.Length);

                CursorX = item.x - ui.delete_string.Length;
                CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }
            else if (item.GetType() == typeof(UndoItem_ReplaceStrings))
            {
                UndoItem_ReplaceStrings ui = (UndoItem_ReplaceStrings)item;

                for (int i = 0, y = ui.start_y; i < ui.strings.Count; i++, y++)
                {
                    string temp = ui.strings[i];
                    StringBuilder s = GetOneLineString(y);
                    ui.strings[i] = s.ToString();
                    s.Remove(0, s.Length);
                    s.Append(temp);
                }

                //CursorX = item.x;
                //CursorY = item.y;

                EnsureVisible();
                Invalidate();
            }


            MakeProperty();

            nUndoPos++;

            // Redo는 소스가 변경되었다는 이야기
            bChangeFlag = true;
            parentEditor.SendEvent(EnumEditorEventType.SourceModified);
        }
    }

    class UndoItem
    {
        public int x;
        public int y;
    }

    class UndoItem_InsertChar : UndoItem
    {
        public char ch;
    }

    class UndoItem_ReplaceChar : UndoItem
    {
        public char old_ch;
        public char new_ch;
    }

    class UndoItem_AddReturn : UndoItem
    {
        public string old_string;   // 커서가 있는 문자열의 소스
    }

    class UndoItem_DeleteSelection : UndoItem
    {
        public int x1;
        public int y1;
        public int x2;
        public int y2;

        public List<string> arrayString = new List<string>();
    }

    class UndoItem_Paste : UndoItem
    {
        public string old_string;    // 커서가 있는 문자열의 소스
        public string paste_string;
        public int count_newline;    // 개행문자의 개수
    }

    class UndoItem_DeleteOneLine : UndoItem
    {
        public string old_string;    // 커서가 있는 문자열의 소스
    }

    class UndoItem_DeleteOneChar : UndoItem
    {
        public bool bBackSpace; // true이면 커서앞의 글자를 삭제한다.
        public char ch;
    }

    class UndoItem_ReplaceStrings : UndoItem
    {
        public int start_y;
        public List<string> strings;
    }

    // 두라인을 한라인으로 합쳤다.
    class UndoItem_AddTwoLine : UndoItem
    {
        public string line1;
        public string line2;
        public string result;
        public bool bLine1;     // true = 아래줄을 위줄에 붙인다. false = 위줄을 아래줄에 붙인다.
        public int resultx;     // 붙이고 난다음의 커서위치
        public int resulty;     // 붙이고 난다음의 커서위치
    }

    class UndoItem_DeleteString : UndoItem
    {
        public string delete_string;
    }
}
