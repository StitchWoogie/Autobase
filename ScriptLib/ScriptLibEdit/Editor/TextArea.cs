using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using NetTools;
using System.Runtime.InteropServices;
using ScriptLibRun;
using ScriptLibEdit.Debugger;
using ScriptLibRun.Debugger;

namespace ScriptLibEdit.Editor
{
    enum EnumCharProperty : byte
    {
        None,           // 아무것도 아니다.
        ColorSystem,    // for if else class ...
        ColorString,    // "  "
        ColorOneChar,   // ' '
        ColorSkip,      //  // 이거나 /*  */
        OpenCurlyBracket,   // {
        CloseCurlyBracket,   // }
        OpenParenthesis,   // (
        CloseParenthesis,   // )
        OpenSqureBracket,   // [
        CloseSqureBracket,   // ]
    }

    public partial class TextArea : Control, IScriptEditorPanel
    {
        // IScriptEditorPanel 명시적 구현 (필드를 프로퍼티로 노출)
        string IScriptEditorPanel.sFilename { get { return sFilename; } set { sFilename = value; } }
        bool IScriptEditorPanel.bChangeFlag { get { return bChangeFlag; } set { bChangeFlag = value; } }
        bool IScriptEditorPanel.bInsertMode { get { return bInsertMode; } }
        bool IScriptEditorPanel.bDisplayBreakPointZone { get { return bDisplayBreakPointZone; } set { bDisplayBreakPointZone = value; } }
        List<StringBuilder> IScriptEditorPanel.arrayString { get { return arrayString; } }

        public List<StringBuilder> arrayString = new List<StringBuilder>();
        List<EnumCharProperty[]> arrayProperty = new List<EnumCharProperty[]>();

        int nCursorX = 0, nCursorY = 0;
        public int nPageX = 0, nPageY = 0;
        int nCharX = 8, nCharY = 16;
        int nPageLimitX, nPageLimitY;
        public bool bChangeFlag = false;

        public bool bDisplayBreakPointZone = false; // 왼쪽에 있는 BreakPoint 회색영역을 그린다.
        

        int nStartTextX = 0;                        // 글자가 시작하는 X위치
        public string sFilename;

        UserControlScriptEditor parentEditor;

        public TextArea(UserControlScriptEditor parent_editor)
        {
            parentEditor = parent_editor;

            ResizeRedraw = true;

            /*
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);*/

            if (bDisplayBreakPointZone)
                nStartTextX = 20;

            MakeProperty();

            CalcCharSize();

            if(ConfigScriptEditor.bDrawingByMemory)
                DoubleBuffered = true;
        }

        sbyte[] startChar = new sbyte[256];
                
        void CalcCharSize()
        {
            nCharY = (int)this.Font.Height+1;
            nCharX = ((nCharY+1)/2);

            Graphics g = CreateGraphics();
            string buf;
            SizeF size;
            int start;
            double center = nCharX / 2.0;
                        
            for (int i = 0; i < 256; i++)
            {
                buf = String.Format("{0}", (char)i);
                size = g.MeasureString(buf, this.Font);

                start = (int)(center-(size.Width)/2.0);

                
                if (start < -1)
                    start = -1;

                startChar[i] = (sbyte)start;
            }
        }

        public int ViewCursorX
        {
            get
            {
                return GetViewCursorX(nCursorX, nCursorY);
            }
        }

        public int ViewCursorY
        {
            get
            {
                return nCursorY;
            }
        }

        int CursorX
        {
            get
            {
                return nCursorX;
            }
            set
            {
                if (nCursorX != value)
                {
                    nCursorX = value;
                    parentEditor.SendEvent(EnumEditorEventType.CursorPositionChanged);
                }
            }
        }

        int CursorY
        {
            get
            {
                return nCursorY;
            }
            set
            {
                if (nCursorY != value)
                {
                    nCursorY = value;
                    parentEditor.SendEvent(EnumEditorEventType.CursorPositionChanged);
                }
            }
        }

        public int PageX
        {
            set
            {
                if (nPageX != value)
                {
                    nPageX = value;
                    parentEditor.hScrollBar1.Value = nPageX;
                }
            }
        }

        public int PageY
        {
            set
            {
                if (nPageY != value)
                {
                    nPageY = value;
                    parentEditor.vScrollBar1.Value = nPageY;
                }
            }
        }

        StringBuilder GetOneLineString(int cursory)
        {
            StringBuilder s;
            if (cursory >= arrayString.Count)
            {
                s = new StringBuilder();
                arrayString.Add(s);
            }
            else
            {
                s = arrayString[cursory];
            }

            return s;
        }

        bool IsSelected()
        {
            if (!bSelectionStart) return false;
            if (nSelectionX1 == nSelectionX2 && nSelectionY1 == nSelectionY2) return false;

            return true;
        }

        void GetArrangedSelectionZone(out int sx1, out int sy1, out int sx2, out int sy2)
        {
            sx1 = nSelectionX1;
            sy1 = nSelectionY1;

            sx2 = nSelectionX2;
            sy2 = nSelectionY2;

            // 정렬한 후
            if (sy1 > sy2)
            {
                Tools.Temp(ref sx1, ref sx2);
                Tools.Temp(ref sy1, ref sy2);
            }
            else if (sy1 == sy2)
            {
                if (sx1 > sx2)
                {
                    Tools.Temp(ref sx1, ref sx2);
                }
            }
        }

        // 선택된 부분을 삭제만 한다. Redo에서도 이 루틴을 같이 사용한다.
        void OnlyDeleteSelection(int sx1, int sy1, int sx2, int sy2)
        {
            // y1에 남아 있는 문자열과 y2에 남아 있는 문자열을 찾고  중간의 열을 삭제하고, 다시 합친다.
            StringBuilder s = GetOneLineString(sy1);
            string s1 = s.ToString(0, sx1);
            s = GetOneLineString(sy2);
            string s2 = s.ToString(sx2, s.Length - sx2);

            for (int i = sy1 + 1; i <= sy2; i++)
            {
                arrayString.RemoveAt(sy1 + 1);
            }

            s = GetOneLineString(sy1);
            s.Remove(0, s.Length);
            s.Append(s1 + s2);

            CancelSelection();

            CursorX = sx1;
            CursorY = sy1;

            ReCalcScrollVert();
            ReCalcScrollHorizon();
            EnsureVisible();
            MakeProperty();
            Invalidate();
        }

        void DeleteSelection()
        {
            if (!IsSelected()) return;

            int sx1, sy1, sx2, sy2;

            GetArrangedSelectionZone(out sx1, out sy1, out sx2, out sy2);

            UndoSave_DeleteSelection(sx1, sy1, sx2, sy2);

            OnlyDeleteSelection(sx1, sy1, sx2, sy2);
        }

        EnumCharProperty SetPropertySkip()
        {
            return  EnumCharProperty.ColorSkip;
        }

        EnumCharProperty SetPropertyString()
        {
            return EnumCharProperty.ColorString;
        }

        EnumCharProperty SetPropertySystem()
        {
            return EnumCharProperty.ColorSystem;
        }

        EnumCharProperty SetPropertyOneChar()
        {
            return EnumCharProperty.ColorOneChar;
        }

        void CheckSystemWord(StringBuilder s, ref int word_start_pos, int x, ref EnumCharProperty[] property)
        {
            if (word_start_pos == -1) return;

            string word = s.ToString(word_start_pos, x - word_start_pos);

            if (word == "using" || word == "namespace" ||
                word == "class" || word == "struct" || word == "enum" || word == "partial" ||
                word == "public" || word == "static" || word == "protected" || word == "private" || word == "override" || word == "virtual" ||
                word == "new" ||
                word == "extern" || 
                
                // elseif 도 사용할 수 있으나 틀린 문법이므로 색상을 표시하지는 않는다.
                word == "return" || word == "if" || word == "else" || word == "for" || word == "while" || word == "break" || word == "continue" ||

                word == "get" || word == "set" ||
                word == "in" || word == "out" || word == "ref" || word == "params" ||

                word == "const" || word == "readonly" ||
                word == "false" || word == "true" ||
                word == "null" ||
                word == "void" ||
                word == "bool" || word == "sbyte" || word == "byte" ||
                word == "char" || word == "short" || word == "ushort" ||
                word == "int" || word == "uint" ||
                word == "long" || word == "ulong" ||
                word == "float" || word == "double" || 
                word == "string" || word == "object")
            {
                for (int i = word_start_pos; i < x; i++)
                {
                    property[i] = SetPropertySystem();
                    i.ToString();
                }
            }

            word_start_pos = -1;
            
        }

        void MakeProperty()
        {
            StringBuilder s;
            EnumCharProperty[] property;

            char ch;
            int count_curlybracket = 0;     // {}
            int count_parenthesis = 0;      // ()
            int count_squarebracket = 0;    // [] 
            bool quotationmart = false;     // ""

            bool reverse_slash = false;
            bool apostrophe = false;
            bool slash_start = false;
            bool block_skip_start = false; // Skip /* 블럭이 시작되었다.
            bool star_start = false;    // *
            int word_start_pos = -1;

            for (int y = 0; y < arrayString.Count; y++)
            {
                s = arrayString[y];

                if (y >= arrayProperty.Count)
                {
                    property = new EnumCharProperty[s.Length];
                    arrayProperty.Add(property);
                }
                else
                {
                    property = arrayProperty[y];
                    if (property.Length < s.Length) // 특성이 문자열보다 작으면 다시 할당해야 한다.
                    {
                        property = new EnumCharProperty[s.Length];
                        arrayProperty[y] = property;
                    }
                }

                quotationmart = false;  // "는 줄이 끝나면 해제된다.
                apostrophe = false; // ' 는 줄이 끝나면 해제된다.
                slash_start = false;
                word_start_pos = -1;

                for (int x = 0; x < s.Length; x++)
                {
                    ch = s[x];
                    property[x] = 0;

                    if (block_skip_start)
                    {
                        property[x] = SetPropertySkip();
                        if (ch == '*')
                        {
                            star_start = true;
                        }
                        else if (ch == '/')
                        {
                            if (star_start)
                            {
                                star_start = false;
                                block_skip_start = false;
                            }
                        }
                        else
                        {
                            star_start = false;
                        }
                    }
                    else if (quotationmart)  // " 일때는 "로 끝날때까지 계속해서 진행한다.
                    {
                        property[x] = SetPropertyString();

                        if (ch == '\\')
                        {
                            reverse_slash = !reverse_slash;
                        }
                        else if (ch == '"')
                        {
                            if (reverse_slash)
                            {
                                // " " 문장 안에 있는 \" 문장이다
                                reverse_slash = false;
                            }
                            else
                            {
                                reverse_slash = false;
                                quotationmart = false;
                            }
                        }
                        else
                        {
                            // 그외 문자일 경우 \ 다음에 있는 문자가 나오면 reverse_slash 는 해제된다.
                            if (reverse_slash)
                            {
                                reverse_slash = false;
                            }
                        }
                    }
                    else if (apostrophe)  // ' 일때는 '로 끝날때까지 계속해서 진행한다.
                    {
                        property[x] = SetPropertyOneChar();

                        if (ch == '\\')
                        {
                            reverse_slash = !reverse_slash;
                        }
                        else if (ch == '\'')
                        {
                            if (reverse_slash)
                            {
                                // ' ' 문장 안에 있는 \' 문장이다
                                reverse_slash = false;
                            }
                            else
                            {
                                reverse_slash = false;
                                apostrophe = false;
                            }
                        }
                        else
                        {
                            // 그외 문자일 경우 \ 다음에 있는 문자가 나오면 reverse_slash 는 해제된다.
                            if (reverse_slash)
                            {
                                reverse_slash = false;
                            }
                        }
                    }
                    else if (slash_start)
                    {
                        if (ch == '/')  // line skip
                        {
                            slash_start = false;

                            for (int i = x - 1; i < s.Length; i++)
                            {
                                property[i] = SetPropertySkip();
                            }
                            
                            break; // line을 벗어난다.
                        }
                        else if (ch == '*')  // block skip
                        {
                            slash_start = false;
                            block_skip_start = true;

                            property[x] = SetPropertySkip();
                            property[x - 1] = SetPropertySkip();
                        }
                        else
                        {
                            slash_start = false;
                        }
                    }
                    else if (ch == '{')
                    {
                        CheckSystemWord(s, ref word_start_pos, x, ref property);
                        property[x] = EnumCharProperty.OpenCurlyBracket;
                        count_curlybracket++;
                    }
                    else if (ch == '}')
                    {
                        property[x] = EnumCharProperty.CloseCurlyBracket;
                        count_curlybracket--;
                    }
                    else if (ch == '"')
                    {
                        quotationmart = true;
                        property[x] = SetPropertyString();
                    }
                    else if (ch == '\'')
                    {
                        apostrophe = true;
                        property[x] = SetPropertyOneChar();
                    }
                    else if (ch == '/')
                    {
                        slash_start = true;
                    }
                    else if (ch == '(')
                    {
                        CheckSystemWord(s, ref word_start_pos, x, ref property);
                        property[x] = EnumCharProperty.OpenParenthesis;
                        count_parenthesis++;
                    }
                    else if (ch == ')')
                    {
                        // (int) 와 같은 cast 인 경우 () 안의 문자도 시스템인가를 검사한다. 2016-5-4
                        CheckSystemWord(s, ref word_start_pos, x, ref property);

                        property[x] = EnumCharProperty.CloseParenthesis;
                        count_parenthesis--;
                    }
                    else if (ch == '[')
                    {
                        CheckSystemWord(s, ref word_start_pos, x, ref property);
                        property[x] = EnumCharProperty.OpenSqureBracket;
                        count_squarebracket++;

                    }
                    else if (ch == ']')
                    {
                        property[x] = EnumCharProperty.CloseSqureBracket;
                        count_squarebracket--;
                    }
                    else
                    {
                        if (ch == ' ' || ch == '\n' || ch == '\t'|| ch == ';')
                        {
                            CheckSystemWord(s, ref word_start_pos, x, ref property);
                        }
                        else
                        {
                            if (word_start_pos == -1)
                            {
                                word_start_pos = x;
                            }
                        }
                    }
                }
                CheckSystemWord(s, ref word_start_pos, s.Length, ref property);
            }
        }

        void AddOneChar(char ch)
        {
            bool insert_mode = bInsertMode;

            if (bInsertMode)
            {
                DeleteSelection();  // 선택된 것이 있으면 모두 지운다.
            }
            else
            {
                if (IsSelected())
                {
                    DeleteSelection();
                    insert_mode = true; // 선택된 것이 있을 때는 순간적으로 삽입모드가 된다.
                }
            }

            StringBuilder s;

            s = GetOneLineString(CursorY);

            if (bInsertMode)
            {
                UndoSave_InsertChar(CursorX, CursorY, ch);
                s.Insert(CursorX, ch);
                CursorX++;
            }
            else
            {
                if (nCursorX < s.Length)
                {
                    UndoSave_ReplaceChar(CursorX, CursorY, s[nCursorX], ch);
                    s[nCursorX] = ch;
                    CursorX++;

                }
                else
                {
                    UndoSave_InsertChar(CursorX, CursorY, ch);
                    s.Insert(CursorX, ch);
                    CursorX++;
                }
            }

            CursorResetBlink();
            EnsureVisible();

            MakeProperty();
            Invalidate();
            ReCalcScrollHorizon(); //20250204 PSU 추가

            //IntelliSenseGo();
        }

        // 줄의 크기가 바뀌었으므로 Vertical을 새로 조정한다.
        void ReCalcScrollVert()
        {
            parentEditor.vScrollBar1.Maximum = parentEditor.vScrollBar1.LargeChange - 1 + arrayString.Count;
        }

        //줄의 길이가 바뀌었으므로 horizontal을 새로 조정한다. 250204 PSU 추가
        void ReCalcScrollHorizon()
        {
            int maxLength = 0;
            foreach (StringBuilder sb in arrayString)
            {
                // StringBuilder를 string으로 변환한 후 길이를 구합니다.
                int length = sb.ToString().Length;

                if (length > maxLength)
                {
                    maxLength = length;
                }
            }

            // 스크롤바의 최대값을 가장 긴 길이로 설정합니다.
            parentEditor.hScrollBar1.Maximum = Math.Max(maxLength, parentEditor.hScrollBar1.LargeChange);
        }

        // 현재 커서 위치가 화면안에 보이도록 한다.
        void EnsureVisible()
        {
            if (CursorY >= nPageY + nPageLimitY)
            {
                PageY = CursorY - (nPageLimitY - 1);
                Invalidate();
            }
            if (CursorY < nPageY)
            {
                PageY = CursorY;
                Invalidate();
            }

            int vx = GetViewCursorX(CursorX, CursorY);

            if (vx >= nPageX + nPageLimitX)
            {
                PageX = vx - (nPageLimitX - 1);
                Invalidate();
            }
            if (vx < nPageX)
            {
                PageX = vx;
                Invalidate();
            }
        }

        void SeekOpenCurlyBracket(int sx, int sy, out int ox, out int oy)
        {
            EnumCharProperty[] property;
            int count = 0; 
            
            for (int y = sy; y >= 0; y--)
            {
                property = arrayProperty[y];
                int x;

                if (y == sy)
                {
                    x = sx-1; // 커서 위치의 문자는 제외하고 앞에서 부터 한다. 
                }
                else
                {
                    x = property.Length - 1;
                }

                for (; x >= 0; x--)
                {
                    if (property[x] ==  EnumCharProperty.OpenCurlyBracket)
                    {
                        count++;

                        if (count == 1)
                        {
                            ox = x;
                            oy = y;
                            return;
                        }
                    }
                    else if (property[x] == EnumCharProperty.CloseCurlyBracket)
                    {
                        count--;
                    }
                }
            }

            ox = -1;
            oy = -1;
        }

        void SeekCloseCurlyBracket(int sx, int sy, out int ox, out int oy)
        {
            EnumCharProperty[] property;
            int count = 0;

            for (int y = sy; y < arrayProperty.Count; y++)
            {
                property = arrayProperty[y];
                int x;

                if (y == sy)
                {
                    x = sx; // 커서 위치의 문자는 제외하고 앞에서 부터 한다. 
                }
                else
                {
                    x = 0;
                }

                for (; x < property.Length; x++)
                {
                    if (property[x] == EnumCharProperty.OpenCurlyBracket)
                    {
                        count++;
                    }
                    else if (property[x] == EnumCharProperty.CloseCurlyBracket)
                    {
                        count--;

                        if (count == -1)
                        {
                            ox = x;
                            oy = y;
                            return;
                        }
                    }
                }
            }

            ox = -1;
            oy = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        void ArrangeCode(int sx, int sy)
        {
            if (!ConfigScriptEditor.bAutomaticallyFormat) return;

            int ox, oy;
            int cx, cy;

            SeekOpenCurlyBracket(sx, sy, out ox, out oy);
            if (ox == -1 || oy == -1)
            {
                return;
            }
            SeekCloseCurlyBracket(sx, sy, out cx, out cy);

            if (cx == -1 || cy == -1)
            {
                return;
            }

            List<string> undo = new List<string>();
            
            for (int y = oy; y <= cy; y++)
            {
                undo.Add(GetOneLineString(y).ToString());
            }

            UndoSave_ReplaceStrings(oy, undo);

            StringBuilder s;

            for (int y = oy; y <= cy; y++)
            {
                s = GetOneLineString(y);
                char start_char;
                int start_pos;

                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == ' ' || s[i] == '\t') continue;
                    start_char = s[i];
                    start_pos = i;
                    goto ok_seeked_start_char;
                }
                continue;  // 한줄이 전부 빈공간 이므로 정렬할 필요가 없다.
            ok_seeked_start_char:

                int start_position = SeekFitCharStartPosition(y, start_char);

                s.Remove(0, start_pos);
                for (int i = 0; i < start_position; i++)
                {
                    s.Insert(0, ' ');
                }
            }

            // 정렬한 후 커서가 오버되면 위치를 잡아준다.
            s = GetOneLineString(nCursorY);
            if (nCursorX > GetCursorRightEnd(s))
            CursorX = GetCursorRightEnd(s);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TextArea
            // 
            this.Font = new System.Drawing.Font("DotumChe", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.ResumeLayout(false);

        }

        // 한글 같은 두개짜리 문자인가를 검사한다.
        bool IsTwoWidthChar(char ch)
        {
            if (ch > 255) return true;

            return false;
        }

        bool IsPosSelected(int x, int y)
        {
            if (nSelectionX1 == nSelectionX2 && nSelectionY1 == nSelectionY2) return false;

            if (nSelectionY1 < nSelectionY2) 
            {
                if(y == nSelectionY1) {
                    return (x >= nSelectionX1);
                }
                else if (y > nSelectionY1 && y < nSelectionY2)
                {
                    return true;
                }
                else if (y == nSelectionY2)
                {
                    return (x < nSelectionX2);
                }
                else
                {
                    return false;
                }
            }
            else if (nSelectionY1 == nSelectionY2)
            {
                if (y == nSelectionY1)
                {
                    if (nSelectionX1 < nSelectionX2)
                    {
                        return (x >= nSelectionX1 && x < nSelectionX2);
                    }
                    else
                    {
                        return (x >= nSelectionX2 && x < nSelectionX1);
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (y == nSelectionY2)
                {
                    return (x >= nSelectionX2);
                }
                else if (y > nSelectionY2 && y < nSelectionY1)
                {
                    return true;
                }
                else if (y == nSelectionY1)
                {
                    return (x < nSelectionX1);
                }
                else
                {
                    return false;
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        public struct RECT
        {
            public Int32 Left;
            public Int32 Top;
            public Int32 Right;
            public Int32 Bottom;
        } 

        protected override void OnPaint(PaintEventArgs e)
        {
            int x = 0;
            int y = 0;
            //Font font = new Font("돋움체", 10);
            Font font = this.Font;

            Brush brush_black = new SolidBrush(Color.Black);
            Brush brush_white = new SolidBrush(Color.White);
            Brush brush_skip = new SolidBrush(Color.Green);
            Brush brush_string = new SolidBrush(Color.FromArgb(163, 21, 21));
            Brush brush_system = new SolidBrush(Color.Blue);
            Brush brush;
            bool selected;
            bool two_width;
            Brush brush_select = new SolidBrush(Color.FromArgb(51, 153, 255));
            StringBuilder s;
            string buf;
            EnumCharProperty[] property;
            char ch;
            int char_count; // 한글=2, 영문=1, Tab은 상황에 따라서 다르다.
            int char_hap = 0;   // 한글/영문/탭을 계산한 현재까지의 글자수

            Graphics g = e.Graphics;

            if (bDisplayBreakPointZone)
            {
                g.FillRectangle(Brushes.LightGray, 0, 0, nStartTextX, ClientRectangle.Height);

                for (int i = 0; i < arrayBreakPoints.Count; i++)
                {
                    y = (arrayBreakPoints[i] - nPageY) * nCharY;

                    g.FillEllipse(Brushes.Black, nStartTextX / 2 - 5, y + nCharY / 2 - 5, 9, 9);
                }
            }

            g.FillRectangle(Brushes.White, nStartTextX, 0, ClientRectangle.Width - nStartTextX, ClientRectangle.Height);

            y = 0;

            for (int i = nPageY; i < arrayString.Count; i++, y += nCharY)
            {
                if (y > e.ClipRectangle.Bottom) break;  // 속도를 빠르게 하기 위해서

                s = arrayString[i];

                if (i >= arrayProperty.Count) break; // 속성 over


                if (bBreaking)
                {
                    if (nBreakPos == i)
                    {
                        Rectangle r = new Rectangle(nStartTextX, y, ClientRectangle.Width - nStartTextX, nCharY);
                        g.FillRectangle(Brushes.Yellow, r);
                    }
                }

                property = arrayProperty[i];

                x = -(nPageX * nCharX) + nStartTextX;   // 한글이 있을 수 있으므로 처음 문자부터 그려야 한다.

                char_hap = 0;
                for (int j = 0; j < s.Length; j++)
                {
                    ch = s[j];

                    selected = IsPosSelected(j, i);
                    two_width = IsTwoWidthChar(ch);

                    if (ch == '\t')
                    {
                        char_count = ScriptLibMain.nTabSpace - (char_hap % ScriptLibMain.nTabSpace);
                    }
                    else if (two_width)
                    {
                        char_count = 2;
                    }
                    else
                    {
                        char_count = 1;
                    }

                    if (selected)
                    {
                        g.FillRectangle(brush_select, x, y, nCharX * char_count, nCharY);
                    }

                    if (ch == '\n')
                    {
                        g.FillRectangle(Brushes.LightBlue, x, y, nCharX - 1, nCharY - 1);
                    }
                    else if (ch == '\t')
                    {
                        // 아무것도 그리지 않는다.
                    }
                    else
                    {
                        if (property[j] == EnumCharProperty.ColorSkip)
                            brush = brush_skip;
                        else if (property[j] == EnumCharProperty.ColorString)
                            brush = brush_string;
                        else if (property[j] == EnumCharProperty.ColorOneChar)
                            brush = brush_string;
                        else if (property[j] == EnumCharProperty.ColorSystem)
                            brush = brush_system;
                        else
                            brush = brush_black;

                        buf = String.Format("{0}", s[j]);

                        if (s[j] < 256)
                            SafeException.SafeDrawString(g, buf, font, selected ? brush_white : brush, x + startChar[s[j]], y);
                        else
                            SafeException.SafeDrawString(g, buf, font, selected ? brush_white : brush, x, y);
                    }

                    char_hap += char_count;
                    x += nCharX * char_count;
                }

                if (ConfigScriptEditor.bDisplayLineEndingGlyph)
                {
                    buf = String.Format("{0}", (char)0x23CE);
                    SafeException.SafeDrawString(g, buf, font, Brushes.LightGray, x, y);
                }
            }

            DrawCursor(g);
        }

        // 주어진 버퍼의 위치에서 한글/탭 문자가 적용된 보이는 커서를 계산한다.
        int GetViewCursorX(int x, StringBuilder s)
        {
            int vx = 0;
            char ch;

            for (int i = 0; i < s.Length && i < x; i++)
            {
                ch = s[i];

                if (ch == '\t')
                {
                    vx += ScriptLibMain.nTabSpace - (vx % ScriptLibMain.nTabSpace);
                }
                else if (IsTwoWidthChar(ch))
                    vx += 2;
                else
                    vx += 1;
            }

            return vx;
        }

        // 주어진 버퍼의 커서에서 한글문자가 적용된 보이는 커서를 계산한다.
        int GetViewCursorX(int x, int y)
        {
            StringBuilder s = GetOneLineString(y);
            return GetViewCursorX(x, s);
        }

        // 주어진 커서의 위치에 해당되는 버퍼의 위치를 계산한다.
        int GetRealCursorX(int vx, StringBuilder s)
        {
            int x = 0;
            int i;
            for (i = 0; i < s.Length && x < vx; i++)
            {
                if (s[i] == '\t')
                {
                    x += ScriptLibMain.nTabSpace - (x % ScriptLibMain.nTabSpace);
                }
                else if (IsTwoWidthChar(s[i]))
                    x += 2;
                else
                    x += 1;
            }

            return i;
        }

        // 주어진 커서의 위치에 해당되는 버퍼의 위치를 계산한다.
        int GetRealCursorX(int x, int y)
        {
            StringBuilder s = GetOneLineString(y);
            return GetRealCursorX(x, s);
        }

        bool bCursorStatus = false;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InvertRect(IntPtr hDC, ref RECT lprc);

        /* 컴퓨터 전체좌표에서 반전을 그리는 도구이다.
        void MyDrawReversibleRectangle(RECT r)
        {
            Point p1 = new Point(r.Left, r.Top);
            Point p2 = new Point(r.Right, r.Bottom);

            Rectangle rc = new Rectangle();

            // Convert the points to screen coordinates.
            p1 = PointToScreen(p1);
            p2 = PointToScreen(p2);
            // Normalize the rectangle.
            if (p1.X < p2.X)
            {
                rc.X = p1.X;
                rc.Width = p2.X - p1.X;
            }
            else
            {
                rc.X = p2.X;
                rc.Width = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                rc.Y = p1.Y;
                rc.Height = p2.Y - p1.Y;
            }
            else
            {
                rc.Y = p2.Y;
                rc.Height = p1.Y - p2.Y;
            }
            // Draw the reversible frame.
            ControlPaint.DrawReversibleFrame(rc,
                            Color.White, FrameStyle.Thick);
        }*/

        void DrawCursor(Graphics g)
        {
            if (!bCursorStatus) return;

            if (!bInsertMode)
            {
                int vx = GetViewCursorX(CursorX, CursorY);

                int x = (vx - nPageX) * nCharX+nStartTextX;
                int y = (CursorY - nPageY) * nCharY;
                
                RECT r = new RECT();
                r.Left = x;
                r.Top = y;
                r.Right = x + nCharX+1;
                r.Bottom = y + nCharY+1;
                
                InvertRect(g.GetHdc(), ref r);
                g.ReleaseHdc();     // Release를 꼭해야한다.  DoubleBufferd사용시 사용중 문제가 발생한다.
            }
            else
            {
                int vx = GetViewCursorX(CursorX, CursorY);

                int x = (vx - nPageX) * nCharX + nStartTextX;
                int y = (CursorY - nPageY) * nCharY;

                
                RECT r = new RECT();

                r.Left = x;
                r.Top = y;
                r.Right = x + 1;
                r.Bottom = y + nCharY+1;

                InvertRect(g.GetHdc(), ref r);
                g.ReleaseHdc(); // Release를 꼭해야한다.  DoubleBufferd사용시 사용중 문제가 발생한다.
            }
        }

        void InvalidateCursor()
        {
            int vx = GetViewCursorX(CursorX, CursorY);

            int x = (vx - nPageX) * nCharX+nStartTextX;
            int y = (CursorY - nPageY) * nCharY;

            Rectangle r = new Rectangle();
            r.X = x;
            r.Y = y;
            r.Width = nCharX*2;
            r.Height = nCharY+1;
            Invalidate(r);
        }

        void CursorOff()
        {
            if (bCursorStatus == false) return;

            bCursorStatus = false;

            InvalidateCursor();
        }

        void CursorOn()
        {
            if (bCursorStatus == true) return;

            bCursorStatus = true;
            timeoutCursor.Reset();

            InvalidateCursor();
        }

        // 어떤 동작을 한 후 커서를 보이도록 하면 커서의 위치를 바로 알 수 있어 동작이 빠르게 보인다.
        void CursorResetBlink()
        {
            bCursorStatus = true;
            timeoutCursor.Reset();
            InvalidateCursor();
        }

        TimeOutMiliSecClass timeoutCursor = new TimeOutMiliSecClass();

        public void OnTimer()
        {
            if (timeoutCursor.IsTimeOut(500))
            {
                timeoutCursor.Reset();
                bCursorStatus = !bCursorStatus;

                InvalidateCursor();
            }
        }

        int nSaveCursorX = -1;   // 커서를 아래 위로 움직일 때 각줄의 크기가 다르므로 위치를 보관해 놓고 될 수 있으면 그 위치로 갈 수 있도록 한다.

        // 오른쪽 끝의 커서 위치를 찾는다.
        int GetCursorRightEnd(StringBuilder s)
        {
            return s.Length;
            /*
            if (s.Length == 0)
            {
                return 0;
            }
            if (s[s.Length - 1] == '\n')
            {
                return s.Length - 1;
            }
            else
            {
                return s.Length;
            }*/
        }

        /// <summary>
        /// 글자가 시작되면 좋을 위치를 찾는다.
        /// 주로 Enter를 쳐서 다음으로 개행했거나 } 를 입력하여 코드를 자동으로 정리할 때 호출되는 함수이다.
        /// </summary>
        /// <param name="row_pos"></param>
        /// <param name="cursor_char"></param>
        /// <returns></returns>
        int SeekFitCharStartPosition(int row_pos, char cursor_char)
        {
            char ch;
            int count_curlybracket = 0;     // {}
            //int count_parenthesis = 0;      // ()
            //int count_squarebracket = 0;    // [] 
            bool quotationmart = false;     // ""
            //bool block_start = false;
            bool reverse_slash = false;
            bool apostrophe = false;
            bool slash_start = false;
            bool block_skip_start = false; // Skip /* 블럭이 시작되었다.
            StringBuilder s;
            bool star_start = false;    // *
            bool if_else_start = false; // if_else 가 있고 다음이 { 로 시작되지 않으면 다음 줄은 한 탭 밀어야 한다.
            int word_pos = -1;  // 워드가 시작된 위치
            
            for(int y = 0; y < row_pos; y++) {
                s = GetOneLineString(y);

                quotationmart = false;  // "는 줄이 끝나면 해제된다.
                apostrophe = false; // ' 는 줄이 끝나면 해제된다.
                slash_start = false;
                word_pos = -1;// 줄이 바뀌면 word 도 다시 시작한다.

                for(int x = 0; x < s.Length; x++) {
                    ch = s[x];

                    if (block_skip_start)
                    {
                        if (ch == '*')
                        {
                            star_start = true;
                        }
                        else if (ch == '/')
                        {
                            if (star_start)
                            {
                                star_start = false;
                                block_skip_start = false;
                            }
                        }
                        else
                        {
                            star_start = false;
                        }
                    }
                    else if (quotationmart)  // " 일때는 "로 끝날때까지 계속해서 진행한다.
                    {
                        if (ch == '\\')
                        {
                            reverse_slash = !reverse_slash;
                        }
                        else if (ch == '"')
                        {
                            if (reverse_slash)
                            {
                                // " " 문장 안에 있는 \" 문장이다
                                reverse_slash = false;
                            }
                            else
                            {
                                quotationmart = false;
                            }
                        }
                        else
                        {
                            // 그외 문자일 경우 \ 다음에 있는 문자가 나오면 reverse_slash 는 해제된다.
                            if (reverse_slash)
                            {
                                reverse_slash = false;
                            }
                        }
                    }
                    else if (apostrophe)  // ' 일때는 '로 끝날때까지 계속해서 진행한다.
                    {
                        if (ch == '\\')
                        {
                            reverse_slash = !reverse_slash;
                        }
                        else if (ch == '\'')
                        {
                            if (reverse_slash)
                            {
                                // ' ' 문장 안에 있는 \' 문장이다
                                reverse_slash = false;
                            }
                            else
                            {
                                apostrophe = false;
                            }
                        }
                    }
                    else if (slash_start)
                    {
                        if (ch == '/')  // line skip
                        {
                            slash_start = false;
                            break; // line을 벗어난다.
                        }
                        else if (ch == '*')  // block skip
                        {
                            slash_start = false;
                            block_skip_start = true;
                        }
                        else
                        {
                            slash_start = false;
                        }
                    }
                    else if (ch == '}')
                    {
                        count_curlybracket--;
                    }
                    else if (ch == '{')
                    {
                        if_else_start = false;  // { 가 시작되면 if else 기본 탭은 사라진다.
                        count_curlybracket++;
                    }
                    else if (ch == '"')
                    {
                        quotationmart = true;
                    }
                    else if (ch == '\'')
                    {
                        apostrophe = true;
                    }
                    else if (ch == '/')
                    {
                        slash_start = true;
                    }
                    else
                    {
                        if (y == row_pos - 1)   // 마지막 줄 일때만 if else 를 검사하여 다음줄을 탭 할것인지를 결정한다.
                        {
                            if (ch == ' ' || ch == '\t' || ch == '(')
                            {
                                if (word_pos != -1)
                                {
                                    string word = s.ToString(word_pos, x - word_pos);
                                    word_pos = -1;
                                    if (word == "if" || word == "else")
                                    {
                                        if_else_start = true;
                                    }
                                }
                            }
                            else
                            {
                                if (word_pos == -1)
                                {
                                    word_pos = x;
                                }
                            }
                        }
                    }
                }

                if (word_pos != -1)
                {
                    string word = s.ToString(word_pos, s.Length-word_pos);
                    word_pos = -1;
                    if (word == "if" || word == "else")
                    {
                        if_else_start = true;
                    }
                }
            }

            if (cursor_char == '{')     // 
            {
                if_else_start = false;
            }
            if (cursor_char == '}')
            {
                count_curlybracket--;
            }

            if (if_else_start)
                count_curlybracket++;

            if (count_curlybracket < 0)
                count_curlybracket = 0; 

            return count_curlybracket*ScriptLibMain.nTabSpace;
        }

        public bool bInsertMode = true;

        public void OnSize()
        {
            nPageLimitX = ClientRectangle.Right / nCharX;
            nPageLimitY = ClientRectangle.Bottom / nCharY;

            ReCalcScrollHorizon(); // 20250407 PSU 추가 스크립트 길게 작성 후 다시 로드 시 재계산.
            ReCalcScrollVert(); //20250407 PSU 추가
        }

        public void LoadFromFile(string filename)
        {
            sFilename = filename;

            if (!File.Exists(filename))
            {
                MessageBox.Show(filename, "File not found");
                return;
            }

            arrayString.Clear();

            TextReader reader = new StreamReader(filename);

            string one_line;
            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                StringBuilder s = new StringBuilder();
                s.Append(one_line);

                arrayString.Add(s);
            }

            reader.Close();

            arrayBreakPoints = ScriptLibMain.LoadBreakPoints(filename);

            MakeProperty();

            this.Invalidate();
        }
        
        // 이 메소드는 샘플파일을 만들 때 주로 사용한다.
        public void LoadFromString(string source)
        {
            source = source.Replace("\r", "");      // \r은 미리 제거한다.

            arrayString.Clear();    // 그냥 붙여넣기를 하니 첫줄은 비어있고 다음줄에 들어간다.

            int start = 0;

            StringBuilder s;
            int i;
                        
            for (i = 0; i < source.Length; i++)
            {
                if (source[i] == '\n')
                {
                    s = new StringBuilder();
                    s.Append(source.Substring(start, i-start));
                    arrayString.Add(s);
                    start = i+1;
                }
            }

            if (start < i)
            {
                s = new StringBuilder();
                s.Append(source.Substring(start, i - start));
                arrayString.Add(s);
            }

            MakeProperty();

            this.Invalidate();
        }

        public bool Save(string filename)
        {
            TextWriter writer = new StreamWriter(filename);

            for (int i = 0; i < arrayString.Count; i++)
            {
                writer.WriteLine(arrayString[i].ToString());
            }

            writer.Close();

            SaveBreakPoint(filename);

            return true;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            const int WHEEL_DELTA = 120;

            int linesPerClick = Math.Max(SystemInformation.MouseWheelScrollLines, 1);

            // accumulate the delta to support high-resolution mice
            int delta = e.Delta/WHEEL_DELTA;

            if (delta < 0)
            {
                if (nPageY >= arrayString.Count-1) return;

                int y = nPageY + Math.Abs(delta) * linesPerClick;

                if (y >= arrayString.Count)
                    y = arrayString.Count - 1;

                PageY = y;
            }
            else
            {
                if (nPageY <= 0) return;
                int y = nPageY - Math.Abs(delta) * linesPerClick;

                if (y < 0)
                    y = 0;

                PageY = y;
            }

            Invalidate();

            //base.OnMouseWheel(e);
        }

        bool bCaptureFlag = false;
        bool bSelectionStart;
        int nSelectionX1, nSelectionY1;
        int nSelectionX2, nSelectionY2;

        void CancelSelection()
        {
            bSelectionStart = false;
            nSelectionX1 = nSelectionX2;
            nSelectionY1 = nSelectionY2;

            Invalidate();
        }

        void GetCursorPositionFromMouse(MouseEventArgs e, out int cx, out int cy)
        {
            int posx = 0;
            int posy = 0;
            int y;

            if (e.Y < 0)
            {
                cx = 0;
                cy = 0;
                return;
            }

            for (y = 0, posy = nPageY; posy < arrayString.Count; posy++, y += nCharY)
            {
                if (e.Y >= y && e.Y < y + nCharY)
                {
                    goto yes_match_y; 
                }
            }
            posy = arrayString.Count - 1;

            if (posy < 0) posy = 0; // 윈도우가 뜨자 마자 마우스를 클릭하면 -1이 된다.  016-11-22

        yes_match_y:
            cy = posy;
            StringBuilder s = GetOneLineString(posy);

            int x = -(nPageX * nCharX)+nStartTextX;   // 한글이 있을 수 있으므로 처음 문자부터 찾아야 한다.
            posx = 0;

            if (e.X < 0)
            {
                posx = 0;
                goto yes_match_x;
            }

            int char_hap = 0;
            int char_count;

            for (posx = 0; posx < s.Length; posx++)
            {
                if (s[posx] == '\t')
                {
                    char_count = ScriptLibMain.nTabSpace - (char_hap % ScriptLibMain.nTabSpace);
                }
                else if (IsTwoWidthChar(s[posx]))
                    char_count = 2;
                else
                    char_count = 1;

                if (e.X >= x && e.X < x + nCharX * char_count)
                    goto yes_match_x;

                x += nCharX * char_count;
                char_hap += char_count;

            }

            posx = GetCursorRightEnd(s);
            //posx = s.Length - 1;

        yes_match_x:
            cx = posx;
        }

        bool CheckMouseBreakZone(MouseEventArgs e)
        {
            if (e.X >= nStartTextX) return false;

            int posy = 0;
            int y;

            for (y = 0, posy = nPageY; posy < arrayString.Count; posy++, y += nCharY)
            {
                if (e.Y >= y && e.Y < y + nCharY)
                {
                    SetDebugBreakPoint(posy);
                    return true;
                }
            }

            return true;
        }
                
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //base.OnMouseDown(e);
            this.Focus();

            if (bCaptureFlag) return;

            if (CheckMouseBreakZone(e)) return;

            CancelSelection();

            CursorOff();
            int x, y;
            GetCursorPositionFromMouse(e, out x, out y);
            CursorX = x;
            CursorY = y;
            CursorOn();

            this.Capture = true;
            bCaptureFlag = true;
            bSelectionStart = true;
            nSelectionX1 = nCursorX;
            nSelectionY1 = nCursorY;
            nSelectionX2 = nCursorX;
            nSelectionY2 = nCursorY;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!bCaptureFlag) return;

            int cx, cy;

            GetCursorPositionFromMouse(e, out cx, out cy);

            if (cx == nSelectionX2 && cy == nSelectionY2) return;

            nSelectionX2 = cx;
            nSelectionY2 = cy;
            CursorX = cx;
            CursorY = cy;

            Invalidate();
            //base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!bCaptureFlag) return;
            this.Capture = false;
            bCaptureFlag = false;
            //base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            StringBuilder s = GetOneLineString(nCursorY);

            if(s.Length == 0)   return;

            int start_x = nCursorX;
            if(start_x >= s.Length) {
                start_x = s.Length-1;
            }

            char start_char = s[start_x];
            if(start_char == ' ' || start_char == '\t') return;
            if (start_char == '(' || start_char == ')')
            {
                bSelectionStart = true;
                nSelectionX1 = start_x;
                nSelectionY1 = nCursorY;
                nSelectionX2 = start_x+1;
                nSelectionY2 = nCursorY;
                Invalidate();
                return;
            }

            int x1, x2;
            int i;
            char[] stop_chars = new char[] { ' ', '\t', '(', ')', ';'};

            for (x2 = start_x+1; x2 < s.Length; x2++)
            {
                for (i = 0; i < stop_chars.Length; i++)
                {
                    if (s[x2] == stop_chars[i]) goto end_x2;
                }
            }
            end_x2:

            for (x1 = start_x - 1; x1 >= 0; x1--)
            {
                for (i = 0; i < stop_chars.Length; i++)
                {
                    if (s[x1] == stop_chars[i]) goto end_x1;
                }
            }
            end_x1:

            bSelectionStart = true;
            nSelectionX1 = x1+1;
            nSelectionY1 = nCursorY;
            nSelectionX2 = x2;
            nSelectionY2 = nCursorY;
            Invalidate();
        }

        public void GotoViewCursor(int x, int y)
        {
            x = GetRealCursorX(x, y);

            GotoCursor(x, y);
        }

        

        void GotoCursor(int x, int y)
        {
            CursorOff();
            if (y >= arrayString.Count)
                y = arrayString.Count - 1;

            if (y <= 0) y = 0;
            StringBuilder s = GetOneLineString(y);
            int last_x = GetCursorRightEnd(s);
            if (x > last_x)
                x = last_x;

            CursorX = x;
            CursorY = y;
            EnsureVisible();
            CursorOn();
        }

        public bool IsPosibleCopy()
        {
            // 아무것도 선택되어 있지 않으면 해당 줄을 복사한다.
            return true;
        }

        public void Copy()
        {
            if (!IsPosibleCopy()) return;

            if (IsSelected())
            {
                int sx1, sy1, sx2, sy2;
                StringBuilder text = new StringBuilder();
                StringBuilder s;

                GetArrangedSelectionZone(out sx1, out sy1, out sx2, out sy2);

                if (sy1 == sy2)
                {
                    s = GetOneLineString(sy1);
                    text.Append(s.ToString(sx1, sx2 - sx1));
                }
                else
                {
                    for (int y = sy1; y <= sy2; y++)
                    {
                        s = GetOneLineString(y);
                        if (y == sy1)
                        {
                            text.Append(s.ToString(sx1, s.Length - sx1));
                        }
                        else if (y == sy2)
                        {
                            text.Append("\n");
                            text.Append(s.ToString(0, sx2));
                        }
                        else
                        {
                            text.Append("\n");
                            text.Append(s.ToString());
                        }
                    }
                }
                Clipboard.SetText(text.ToString());
            }
            else
            {
                // 선택을 하지 않고 복사를 하면 한줄 전체가 복사된다. 
                // Visual Studio에서는 이것을 붙여넣기 하면 커서 위치에서가 아니라 현재 위치를 아래로 밀고 현재위치에 한줄이 생기는 구조이다.
                // 조금 다르다.
                StringBuilder s = GetOneLineString(nCursorY);
                string text = s.ToString()+"\n";
                Clipboard.SetText(text);
            }
        }

        public bool IsPosiblePaste()
        {
            return Clipboard.ContainsText();
        }

        // Undo에서도 이 함수를 사용한다.
        void OnlyPaste(string source)
        {
            int start = 0;

            StringBuilder s;
            int i;
            string buf;

            for (i = 0; i < source.Length; i++)
            {
                if (source[i] == '\n')
                {
                    s = GetOneLineString(nCursorY);
                    buf = s.ToString(nCursorX, s.Length - nCursorX);
                    s.Remove(nCursorX, s.Length - nCursorX);
                    s.Insert(nCursorX, source.Substring(start, i - start));

                    s = new StringBuilder();
                    s.Append(buf);
                    arrayString.Insert(CursorY + 1, s); // 앞에서 넘어온 커서뒤의 문자열을 다음줄로 넘긴다.
                    start = i + 1;
                    CursorY++;
                    CursorX = 0;
                }
            }

            if (start < i)
            {
                s = GetOneLineString(nCursorY);
                buf = source.Substring(start, i - start);
                s.Insert(nCursorX, buf);
                CursorX += buf.Length;
            }

            ReCalcScrollVert();
            ReCalcScrollHorizon();
            EnsureVisible();
            MakeProperty();
            Invalidate();
        }

        public void Paste()
        {
            if (!IsPosiblePaste()) return;

            DeleteSelection();

            string source = Clipboard.GetText();
            source = source.Replace("\r\n", "\n");  // 계산하기 좋도록 \r문자를 제거한다.

            UndoSave_Paste(source);

            OnlyPaste(source);
        }
           
        public void SelectAll()
        {
            bSelectionStart = true;
            nSelectionX1 = 0;
            nSelectionY1 = 0;

            nSelectionY2 = arrayString.Count - 1;
            StringBuilder s = GetOneLineString(nSelectionY2);

            nSelectionX2 = GetCursorRightEnd(s);

            Invalidate();
        }

        void OnlyDeleteOneLine()
        {
            arrayString.RemoveAt(nCursorY);

            CursorX = 0;

            ReCalcScrollVert();
            ReCalcScrollHorizon();
            EnsureVisible();
            MakeProperty();
            Invalidate();
        }

        public void EditCut()
        {
            Copy(); // 먼저 복사를 한 다음

            if (IsSelected())
            {
                DeleteSelection();
            }
            else
            {
                UndoSave_DeleteOneLine();

                OnlyDeleteOneLine();
            }
        }

        public void SetFont(Font font)
        {
            this.Font = font;
            CalcCharSize();
        }

        // 외부에서 환경설정이 바뀌었을 때 혹시 바꿀것이 있으면 바꾸어준다.
        public void OnConfigurationChanged()
        {
            this.DoubleBuffered = ConfigScriptEditor.bDrawingByMemory;
            Invalidate();
        }


        // 선택된 텍스트 가져오기 (기존 IsSelected() 메서드 활용) 250731 PSU
        public string GetSelectedText()
        {
            if (!IsSelected()) return "";

            int sx1, sy1, sx2, sy2;
            GetArrangedSelectionZone(out sx1, out sy1, out sx2, out sy2);

            var text = new StringBuilder();

            if (sy1 == sy2)
            {
                // 같은 줄에서 선택
                StringBuilder s = GetOneLineString(sy1);
                text.Append(s.ToString(sx1, sx2 - sx1));
            }
            else
            {
                // 여러 줄에 걸쳐 선택
                for (int y = sy1; y <= sy2; y++)
                {
                    StringBuilder s = GetOneLineString(y);
                    if (y == sy1)
                    {
                        text.Append(s.ToString(sx1, s.Length - sx1));
                    }
                    else if (y == sy2)
                    {
                        text.Append("\n");
                        text.Append(s.ToString(0, sx2));
                    }
                    else
                    {
                        text.Append("\n");
                        text.Append(s.ToString());
                    }
                }
            }

            return text.ToString();
        }

        // 전체 텍스트에서의 커서 위치 (인덱스) 계산 250731 PSU
        public int GetCursorPosition()
        {
            int position = 0;

            // 현재 줄 이전까지의 모든 문자 수 계산 (개행 문자 포함)
            for (int i = 0; i < nCursorY && i < arrayString.Count; i++)
            {
                position += arrayString[i].Length;
                if (i < arrayString.Count - 1) position++; // 개행 문자
            }

            // 현재 줄에서의 위치 추가
            if (nCursorY < arrayString.Count)
            {
                position += Math.Min(nCursorX, arrayString[nCursorY].Length);
            }

            return position;
        }

        // 전체 텍스트 인덱스를 줄/열 좌표로 변환
        public void PositionToLineColumn(int position, out int line, out int column)
        {
            line = 0;
            column = 0;
            int currentPos = 0;

            for (int i = 0; i < arrayString.Count; i++)
            {
                int lineLength = arrayString[i].Length;

                if (currentPos + lineLength >= position)
                {
                    line = i;
                    column = position - currentPos;
                    return;
                }

                currentPos += lineLength;
                if (i < arrayString.Count - 1) currentPos++; // 개행 문자
            }

            // 텍스트 끝을 벗어난 경우
            if (arrayString.Count > 0)
            {
                line = arrayString.Count - 1;
                column = arrayString[line].Length;
            }
        }

        // 텍스트 선택 (기존 선택 시스템 활용)
        public void SetSelection(int start, int length)
        {
            if (length <= 0)
            {
                CancelSelection();
                GotoPosition(start);
                return;
            }

            // 시작 위치로 이동
            PositionToLineColumn(start, out int startLine, out int startColumn);

            // 끝 위치 계산
            PositionToLineColumn(start + length, out int endLine, out int endColumn);

            // 기존 선택 시스템 사용
            bSelectionStart = true;
            nSelectionX1 = startColumn;
            nSelectionY1 = startLine;
            nSelectionX2 = endColumn;
            nSelectionY2 = endLine;

            // 커서를 선택 영역 끝으로 이동
            CursorX = endColumn;
            CursorY = endLine;

            // 선택 영역이 보이도록 스크롤
            ScrollToCursor();

            // 화면 갱신
            Invalidate();
        }

        // 지정된 위치로 커서 이동
        public void GotoPosition(int position)
        {
            PositionToLineColumn(position, out int line, out int column);

            // 기존 GotoCursor 메서드 활용
            GotoCursor(column, line);
        }

        // 커서가 보이도록 스크롤 조정 (기존 EnsureVisible 메서드 활용)
        public void ScrollToCursor()
        {
            EnsureVisible();
        }

        // 선택 영역이 있는지 확인 (기존 IsSelected 메서드 활용)
        public bool HasSelection()
        {
            return IsSelected();
        }

        // 선택 영역 해제 (기존 CancelSelection 메서드 활용)
        public void ClearSelection()
        {
            CancelSelection();
        }


        // IScriptEditorPanel 구현: 전체 소스를 문자열로 반환
        public string GetSourceString()
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < arrayString.Count; i++)
            {
                if (i != 0) s.Append('\n');
                s.Append(arrayString[i]);
            }
            return s.ToString();
        }
    }
}