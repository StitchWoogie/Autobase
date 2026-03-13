using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScriptLibRun;

namespace ScriptLibEdit.Editor
{
    partial class TextArea : Control
    {
        // 이 이벤트가 먼저 발생하고 OnKeyPress 가 나중에 발생한다.
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (WmKeyDown(keyData))
            {
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        // cursor/page  키 등 특수 키는 들어오지 않는다.
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == '}')
            {
                AddOneChar(e.KeyChar);

                ArrangeCode(nCursorX - 1, nCursorY);

                MakeProperty();
                Invalidate();
            }
            else
            {
                AddOneChar(e.KeyChar);
            }

            e.Handled = true;   // 이것을 추가하니 한글다음에 스페이스나 . 같은 영문자를 누르면 한글이 두번씩 이벤트가 발생하는 문제가 해결된다.
        }

        /// <summary>
        /// OnKeyPress에 잡히지 않는 키만 처리한다.
        /// </summary>
        /// <param name="m"></param>
        bool WmKeyDown(Keys key)
        {
            if (key == (Keys.Left | Keys.Shift))
            {
                if (!bSelectionStart)
                {
                    bSelectionStart = true;
                    nSelectionX1 = CursorX;
                    nSelectionY1 = CursorY;
                }

                KeyDownLeft();

                nSelectionX2 = CursorX;
                nSelectionY2 = CursorY;

                Invalidate();
            }
            else if (key == (Keys.Right | Keys.Shift))
            {
                if (!bSelectionStart)
                {
                    bSelectionStart = true;
                    nSelectionX1 = CursorX;
                    nSelectionY1 = CursorY;
                }

                KeyDownRight();

                nSelectionX2 = CursorX;
                nSelectionY2 = CursorY;

                Invalidate();
            }
            else if (key == (Keys.Up | Keys.Shift))
            {
                if (!bSelectionStart)
                {
                    bSelectionStart = true;
                    nSelectionX1 = CursorX;
                    nSelectionY1 = CursorY;
                }

                KeyDownUp();

                nSelectionX2 = CursorX;
                nSelectionY2 = CursorY;

                Invalidate();
            }
            else if (key == (Keys.Down | Keys.Shift))
            {
                if (!bSelectionStart)
                {
                    bSelectionStart = true;
                    nSelectionX1 = CursorX;
                    nSelectionY1 = CursorY;
                }

                KeyDownDown();

                nSelectionX2 = CursorX;
                nSelectionY2 = CursorY;

                Invalidate();
            }
            else if (key == (Keys.Home | Keys.Shift))
            {
                if (!bSelectionStart)
                {
                    bSelectionStart = true;
                    nSelectionX1 = CursorX;
                    nSelectionY1 = CursorY;
                }

                KeyDownHome();

                nSelectionX2 = CursorX;
                nSelectionY2 = CursorY;

                Invalidate();
            }
            else if (key == (Keys.End | Keys.Shift))
            {
                if (!bSelectionStart)
                {
                    bSelectionStart = true;
                    nSelectionX1 = CursorX;
                    nSelectionY1 = CursorY;
                }

                KeyDownEnd();

                nSelectionX2 = CursorX;
                nSelectionY2 = CursorY;

                Invalidate();
            }
            else if (key == (Keys.PageUp | Keys.Shift))
            {
                if (!bSelectionStart)
                {
                    bSelectionStart = true;
                    nSelectionX1 = CursorX;
                    nSelectionY1 = CursorY;
                }

                KeyDownPageUp();

                nSelectionX2 = CursorX;
                nSelectionY2 = CursorY;

                Invalidate();
            }
            else if (key == (Keys.PageDown | Keys.Shift))
            {
                if (!bSelectionStart)
                {
                    bSelectionStart = true;
                    nSelectionX1 = CursorX;
                    nSelectionY1 = CursorY;
                }

                KeyDownPageDown();

                nSelectionX2 = nCursorX;
                nSelectionY2 = nCursorY;

                Invalidate();
            }

            else if (key == Keys.Left)
            {
                CancelSelection();

                KeyDownLeft();
            }
            else if (key == Keys.Right)
            {
                CancelSelection();

                KeyDownRight();
            }
            else if (key == Keys.Up)
            {
                CancelSelection();

                KeyDownUp();
            }
            else if (key == Keys.Down)
            {
                CancelSelection();

                KeyDownDown();
            }
            else if (key == Keys.Home)
            {
                CancelSelection();

                KeyDownHome();
            }
            else if (key == Keys.End)
            {
                CancelSelection();

                KeyDownEnd();
            }
            else if (key == Keys.PageUp)
            {
                CancelSelection();

                KeyDownPageUp();
            }
            else if (key == Keys.PageDown)
            {
                CancelSelection();

                KeyDownPageDown();
            }
            else if (key == Keys.Delete)
            {
                KeyDownDelete();
            }
            else if (key == Keys.Back)
            {
                KeyDownBackspace();
            }
            else if (key == (Keys.Z | Keys.Control))
            {
                Undo();
            }
            else if (key == (Keys.Y | Keys.Control))
            {
                Redo();
            }
            else if (key == (Keys.C | Keys.Control))
            {
                Copy();
            }
            else if (key == (Keys.V | Keys.Control))
            {
                Paste();
            }
            else if (key == (Keys.A | Keys.Control))
            {
                SelectAll();
            }
            else if (key == (Keys.X | Keys.Control))
            {
                EditCut();
            }
            else if (key == Keys.Insert)
            {
                CursorOff();
                bInsertMode = !bInsertMode;
                parentEditor.SendEvent(EnumEditorEventType.InsertModeChanged);
                CursorOn();
            }
            else if (key == (Keys.Home | Keys.Control))
            {
                KeyDownCtrlHome();
            }
            else if (key == (Keys.End | Keys.Control))
            {
                KeyDownCtrlEnd();
            }
            else if (key == Keys.Tab)
            {
                KeyDownTab();
            }
            else if (key == (Keys.Tab | Keys.Shift))
            {
                KeyDownShiftTab();
            }
            else if (key == Keys.Enter)
            {
                KeyDownEnter();
            }
            else if (key == Keys.Escape)
            {
                KeyDownEscape();
            }
            else
            {
                return false;
            }

            return true;
        }

        void KeyDownEscape()
        {
            if (FormIntelliSense.formThis != null)
            {
                FormIntelliSense.formThis.Close();
            }
        }

        public void OnlyKeyDownEnter()
        {
            string newlines;

            StringBuilder s = GetOneLineString(CursorY);

            newlines = s.ToString();
            newlines = newlines.Substring(CursorX);
            s.Remove(CursorX, s.Length - CursorX);

            StringBuilder s2 = new StringBuilder();
            s2.Append(newlines);

            int start_position = 0;

            if (ConfigScriptEditor.bAutomaticallyFormat)
            {
                start_position = SeekFitCharStartPosition(CursorY + 1, newlines.Length == 0 ? ' ' : newlines[0]);
                for (int i = 0; i < start_position; i++)
                {
                    s2.Insert(0, ' ');
                }
            }

            arrayString.Insert(CursorY + 1, s2);
            CursorX = start_position;
            CursorY++;

            ReCalcScrollVert();
            ReCalcScrollHorizon();
            EnsureVisible();

            MakeProperty();

            Invalidate();

            CursorResetBlink();
        }

        public void KeyDownEnter()
        {
            StringBuilder s = GetOneLineString(CursorY);

            UndoSave_AddReturn(s.ToString());

            OnlyKeyDownEnter();
        }

        void KeyDownRight()
        {
            nSaveCursorX = -1;
            if (CursorY >= arrayString.Count)
            {
                return;
            }
            StringBuilder s = arrayString[CursorY];
            if (CursorX >= GetCursorRightEnd(s))   // 커서가 줄의 끝에 있을 때
            {
                if (CursorY < arrayString.Count - 1)
                {
                    CursorOff();
                    CursorY++;
                    CursorX = 0;
                    CursorOn();
                }
                else
                {
                    return;
                }
            }
            else
            {
                CursorOff();
                CursorX++;
                CursorOn();
            }
            EnsureVisible();
        }

        void KeyDownUp()
        {
            int vx, x;

            if (CursorY <= 0) return;

            CursorOff();

            if (nSaveCursorX == -1)
            {
                vx = GetViewCursorX(CursorX, CursorY);
                nSaveCursorX = vx;
                x = vx;
            }
            else
            {
                x = nSaveCursorX;
            }

            x = GetRealCursorX(x, CursorY - 1);

            StringBuilder s = GetOneLineString(CursorY - 1);

            if (x > s.Length)
            {
                x = s.Length;
            }

            CursorX = x;
            CursorY--;
            CursorOn();
            EnsureVisible();
        }

        void KeyDownDown()
        {
            int vx, x;

            if (CursorY >= arrayString.Count - 1) return;

            CursorOff();

            if (nSaveCursorX == -1)
            {
                vx = GetViewCursorX(CursorX, CursorY);
                nSaveCursorX = vx;
                x = vx;
            }
            else
            {
                x = nSaveCursorX;
            }

            x = GetRealCursorX(x, CursorY + 1);
            StringBuilder s = GetOneLineString(CursorY + 1);

            if (x > s.Length)
            {
                x = s.Length;
            }

            CursorX = x;
            CursorY++;
            CursorOn();
            EnsureVisible();
        }

        void KeyDownHome()
        {
            nSaveCursorX = -1;
            if (CursorX <= 0) return;
            CursorOff();

            // 빈칸이 아닌 처음으로 간다.
            StringBuilder s = GetOneLineString(nCursorY);
            int x = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ' || s[i] == '\t')
                {
                    continue;
                }
                x = i;
                break;
            }

            if (x == CursorX)    // 이미 글자의 맨앞에 있으면 줄의 맨처음으로 보낸다. Home키를 처음 누르면 문장의 맨앞 다음에는 줄의 맨앞
                CursorX = 0;
            else
                CursorX = x;

            EnsureVisible();
            CursorOn();
        }

        void KeyDownEnd()
        {
            nSaveCursorX = -1;
            CursorOff();
            StringBuilder s = arrayString[CursorY];
            CursorX = s.Length;
            EnsureVisible();
            CursorOn();
        }

        void KeyDownCtrlHome()
        {
            nSaveCursorX = -1;

            if (CursorX <= 0 && CursorY <= 0) return;

            CursorOff();
            CursorX = 0;
            CursorY = 0;

            EnsureVisible();
            CursorOn();
        }

        void KeyDownCtrlEnd()
        {
            nSaveCursorX = -1;
            CursorOff();
            StringBuilder s = arrayString[arrayString.Count - 1];

            CursorY = arrayString.Count - 1;
            CursorX = GetCursorRightEnd(s);

            EnsureVisible();
            CursorOn();
        }

        void KeyDownPageUp()
        {
            int vx, x;

            if (CursorY <= 0) return;

            CursorOff();

            if (nSaveCursorX == -1)
            {
                vx = GetViewCursorX(CursorX, CursorY);
                nSaveCursorX = vx;
                x = vx;
            }
            else
            {
                x = nSaveCursorX;
            }

            if (nPageY <= 0)
            {
                CursorY = 0;
            }
            else
            {
                int y = nPageY - nPageLimitY;
                if (y < 0)
                    y = 0;

                CursorY -= (nPageY - y);
                PageY = y;
            }

            x = GetRealCursorX(x, CursorY);

            StringBuilder s = GetOneLineString(CursorY);

            if (x >= s.Length)
            {
                x = s.Length - 1;
            }

            CursorX = x;

            CursorOn();
            EnsureVisible();

            Invalidate();
        }

        void KeyDownPageDown()
        {
            int vx, x;

            if (CursorY >= arrayString.Count - 1) return;

            CursorOff();

            if (nSaveCursorX == -1)
            {
                vx = GetViewCursorX(CursorX, CursorY);
                nSaveCursorX = vx;
                x = vx;
            }
            else
            {
                x = nSaveCursorX;
            }

            if (nPageY + nPageLimitY >= arrayString.Count)
            {
                CursorY = arrayString.Count - 1;
            }
            else
            {
                int y = nPageY + nPageLimitY;
                if (y >= arrayString.Count)
                    y = arrayString.Count - 1;

                CursorY += (y - nPageY);
                PageY = y;

                if (CursorY >= arrayString.Count)
                    CursorY = arrayString.Count - 1;
            }

            x = GetRealCursorX(x, CursorY);

            StringBuilder s = GetOneLineString(CursorY);

            if (x >= s.Length)
            {
                x = s.Length - 1;
            }

            CursorX = x;

            CursorOn();
            EnsureVisible();

            Invalidate();
        }

        public void KeyDownDelete()
        {
            CursorOff();

            if (IsSelected())
            {
                DeleteSelection();
            }
            else
            {
                StringBuilder s = GetOneLineString(nCursorY);

                //if (s.Length == 0) return;

                // 줄의 맨끝에 있을 때
                if (nCursorX >= GetCursorRightEnd(s))
                {
                    if (nCursorY >= arrayString.Count - 1)
                    {
                        if (nCursorX < s.Length)
                        { // \n 이 있는 경우만 삭제한다.
                            s = GetOneLineString(nCursorY);
                            s.Remove(nCursorX, 1);
                        }
                    }
                    else
                    {
                        string line1 = s.ToString();
                        StringBuilder s2 = GetOneLineString(nCursorY + 1);
                        string line2 = s2.ToString();

                        if (nCursorX < s.Length) // \n 이 있는 경우만 삭제한다.
                            s.Remove(nCursorX, 1);

                        s.Append(s2.ToString());

                        string result = s.ToString();

                        UndoSave_AddTwoLine(line1, line2, result, true, nCursorX, nCursorY);

                        arrayString.RemoveAt(nCursorY + 1);

                        ReCalcScrollVert();
                        ReCalcScrollHorizon();
                    }
                }
                else
                {
                    s = GetOneLineString(nCursorY);
                    UndoSave_DeleteOneChar(s[nCursorX], false);
                    s.Remove(nCursorX, 1);
                }

                CursorOn();
                MakeProperty();
                Invalidate();
            }
        }

        public void KeyDownBackspace()
        {
            StringBuilder s;

            if (IsSelected())
            {
                DeleteSelection();
            }
            else
            {
                if (CursorX <= 0 && CursorY <= 0) return;

                if (CursorX <= 0)
                {
                    s = GetOneLineString(CursorY);
                    string line1 = s.ToString();    // 커서가 있는 라인

                    StringBuilder s2 = GetOneLineString(CursorY - 1);
                    string line2 = s2.ToString();   // 커서 위의 라인

                    if (s2.Length == 0) // 커서위의 라인이 비어있으면 위의 라인을 삭제한다.
                    {
                        CursorY--;
                        UndoSave_DeleteOneLine();
                        arrayString.RemoveAt(nCursorY);
                    }
                    else
                    {   
                        // 위 라인의 맨뒤에 커서에 있는 문자열을 붙여준다. (즉 글자는 삭제된것이 없고 개행문자만 삭제된 것이다.)
                        int cx = s2.Length;

                        s2.Append(s.ToString());
                        string result = s2.ToString();

                        arrayString.RemoveAt(CursorY);

                        UndoSave_AddTwoLine(line1, line2, result, false, cx, nCursorY - 1);

                        CursorX = cx;
                        CursorY--;
                    }

                    ReCalcScrollVert();
                    ReCalcScrollHorizon();
                    EnsureVisible();
                }
                else
                {
                    s = GetOneLineString(CursorY);
                    UndoSave_DeleteOneChar(s[nCursorX - 1], true);
                    s.Remove(CursorX - 1, 1);
                    CursorX--;
                    if (CursorX < nPageX)
                        EnsureVisible();
                }

                MakeProperty();
                Invalidate();
            }

            CursorResetBlink();
        }

        public void KeyDownTab()
        {
            StringBuilder s = GetOneLineString(nCursorY);

            int viewx = GetViewCursorX(nCursorX, nCursorY);

            int tab = viewx % ScriptLibMain.nTabSpace;
            int space;

            if (tab == 0)
            {
                space = ScriptLibMain.nTabSpace;
            }
            else
            {
                space = ScriptLibMain.nTabSpace - tab;
            }

            string insert_string = "";
            for (int i = 0; i < space; i++)
            {
                insert_string += " ";
            }

            UndoSave_Paste(insert_string);

            OnlyPaste(insert_string);

            CursorResetBlink();
        }

        public void KeyDownShiftTab()
        {
            if (nCursorX == 0) return;  // 더 이상 갈 곳이 없다.

            StringBuilder s = GetOneLineString(nCursorY);

            // 왼쪽의 글자가 빈 공간일 때만 Shift+Tab이 가능하다. 일단 탭이 들어가면 하지 않는다. 복잡해서 일단 보류
            for (int i = 0; i < nCursorX; i++)
            {
                if (s[i] != ' ') return;
            }

            int tab = nCursorX % ScriptLibMain.nTabSpace;
            int space;

            if (tab == 0)
            {
                space = ScriptLibMain.nTabSpace;
            }
            else
            {
                space = tab;
            }

            string delete_string = "";
            int start_pos = nCursorX - space;
            for (int i = 0, pos = start_pos; i < space; i++, pos++)
            {
                delete_string += s[pos];
            }

            UndoSave_DeleteString(delete_string);

            s.Remove(start_pos, space);

            CursorX -= space;

            MakeProperty();
            EnsureVisible();
            Invalidate();
        }

        void KeyDownLeft()
        {
            StringBuilder s;

            nSaveCursorX = -1;
            if (CursorX <= 0 && CursorY <= 0) return;  // 제일처음으로 왔다.

            CursorOff();
            if (CursorX <= 0)
            {
                CursorY--;
                s = arrayString[CursorY];
                CursorX = GetCursorRightEnd(s);
            }
            else
            {
                CursorX--;
            }
            CursorOn();
            EnsureVisible();
        }
    }
}
