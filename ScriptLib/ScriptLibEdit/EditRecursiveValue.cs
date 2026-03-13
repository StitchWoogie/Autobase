using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;
using System.Windows.Forms;

namespace ScriptLibEdit
{
    public class EditRecursiveValue : RecursiveValue
    {
        public EditRecursiveValue(ScriptLibMemberMethod parent, CommandPublic parent_block)
            : base(parent, parent_block)
        {

        }

        /// <summary>
        /// " "로 둘러쌓인 원시 문자열에서 \를 제거하여 진짜 문자열로 만든다.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        string ConvertStringFromSourceString(string p)
        {
            // += 을 StringBuilder로 변경함

            int total_size = p.Length;
            StringBuilder sb = new StringBuilder();

            int i;
            bool flag_slash = false;

            for (i = 1; i <= total_size - 2; i++)
            {
                if (flag_slash)
                {
                    // escape 문자는 ', ", \, 0, a, b, f, n, r, t, u, U, x, v  의 14개가 있다.

                    if (p[i] == '\'') sb.Append('\'');
                    else if (p[i] == '"') sb.Append('\"');
                    else if (p[i] == '\\') sb.Append('\\');
                    else if (p[i] == '0') sb.Append('\0');
                    else if (p[i] == 'a') sb.Append('\a');
                    else if (p[i] == 'b') sb.Append('\b');
                    else if (p[i] == 'f') sb.Append('\f');
                    else if (p[i] == 'n') sb.Append('\n');
                    else if (p[i] == 'r') sb.Append('\r');
                    else if (p[i] == 't') sb.Append('\t');
                    else if (p[i] == 'v') sb.Append('\v');
                    else if (p[i] == 'u' || p[i] == 'U')
                    {
                        // x와 같은 개념으로 했다. 원래는 \u0000 식으로 4자리가 있어야 한다.
                        int k = 0;
                        for (int j = i + 1; j <= total_size && k < 4; j++, k++)
                        {
                            if ((p[j] >= '0' && p[j] <= '9') || (p[j] >= 'a' && p[j] <= 'f') || (p[j] >= 'A' && p[j] <= 'F'))
                            {
                                continue;
                            }

                            break;
                        }

                        string buf = p.Substring(i + 1, k);
                        sb.Append((char)ConvertTool.ToUint64FromHexBuf(buf));

                        i = i + k;                                                                                                   
                    }
                    else if (p[i] == 'x')
                    {
                        int k = 0;
                        for (int j = i + 1; j <= total_size && k < 4; j++, k++)
                        {
                            if ((p[j] >= '0' && p[j] <= '9') || (p[j] >= 'a' && p[j] <= 'f') || (p[j] >= 'A' && p[j] <= 'F'))
                            {
                                continue;
                            }

                            break;
                        }

                        string buf = p.Substring(i + 1, k);
                        sb.Append((char)ConvertTool.ToUint64FromHexBuf(buf));

                        i = i+k;
                    }
                    else sb.Append(p[i]);

                    flag_slash = false;
                    continue;
                }
                else if (p[i] == '\\')
                {
                    flag_slash = true;
                }
                else
                {
                    sb.Append(p[i]);
                }
            }

            return sb.ToString();
        }

        class SeekedChar
        {
            public char ch;
            public int pos;
            public int col_pos;
            public int row_pos;
        }

        //------------------------------------------------------------------------------
        //	미리 정의된 define은 이것을 사용한다.
        //  이전 스크립트의 호환성을 위해서 미리 정의된 define을 지원하기로 한다.
        //------------------------------------------------------------------------------

        [NonSerialized]
        static string[] pre_define = { "ON", "OFF", "TRUE", "FALSE", "MB_OK", "MB_YESNO", "MB_YESNOCANCEL", "IDYES", "IDNO", "IDCANCEL", "IDOK" };
        [NonSerialized]
        //static int[] pre_value = { 1, 0, 1, 0, (int)MessageBoxButtons.OK, (int)MessageBoxButtons.YesNo, (int)MessageBoxButtons.YesNoCancel, (int)DialogResult.Yes, (int)DialogResult.No, (int)DialogResult.Cancel, (int)DialogResult.OK };
        static int[] pre_value = { 1, 0, 1, 0, (int)0, (int)4, (int)3, (int)6, (int)7, (int)2, (int)1 };

        bool IsPreDefinedValue(string buf, out int val)
        {
            int i;

            for (i = 0; i < pre_define.Length; i++)
            {
                if(buf == pre_define[i]) {
                    val = pre_value[i];
                    return true;
                }
            }

            val = 0;

            return false;
        }

        bool SeekPosCloseParenthesis(EditScriptLibMain main, string body, ref int i, int end_pos)
        {
            int open = 1;

            char ch;

            for (; i <= end_pos; i++)
            {
                ch = body[i];

                if (ch == ')')
                {
                    open--;

                    if(open == 0)
                        return true;
                }
                else if (ch == '(')
                {
                    open++;
                }
            }

            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, ") count mismatched.");

            return false;
        }

        /// <summary>
        /// 값을 + - * / | & ^ % 로 분리하여 블럭으로 나눈다음 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="file"></param>
        /// <param name="body"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string body, int col_pos, int row_pos)
        {
            body = ScriptLibTools.TrimWithCursorPos(body, ref col_pos, ref row_pos);

            sSourceString = body;
            nColumn = col_pos;
            nRow = row_pos;

            if (body.Length == 0)
            {
                eCalcMethod = EnumCalcMethod.LastValue;
                eValueType = EnumLastValueType.None;
                return true;
            }

            // {} 로 쌓여 있는 값은 Initial 값이다.
            // 초기화만 있고 new int[] 같은 문장이 없는 경우  = { 3, 4 }; 와 같은 경우
            if (body[0] == '{' && body[body.Length-1] == '}')
            {
                pInitial = new EditInitialValue(parentMethod, parentBlock);
                int start = 0;
                if (!((EditInitialValue)pInitial).Split(main, file, body, ref start, col_pos, row_pos)) return false;

                eCalcMethod = EnumCalcMethod.LastValue;
                eValueType = EnumLastValueType.InitArray;
                return true;
            }

            EditRecursiveValue lv = new EditRecursiveValue(parentMethod, parentBlock);
            EditRecursiveValue rv = new EditRecursiveValue(parentMethod, parentBlock);

            leftVal = lv;
            rightVal = rv;

            int size = body.Length;

            int open1 = 0;  // ( )
            int open2 = 0;  // [ ]
            bool open3 = false;  // " "
            int count_curlybracket = 0; // {}

            List<SeekedChar> arrayChar = new List<SeekedChar>();

            SplitedString splitDemension = new SplitedString();
            SplitedString splitCurlyBracket = new SplitedString();

            int dimension_start_pos = -1;   // [ 가 시작하는 위치를 찾아 놓는다.
            int dimension_end_pos = -1;

            int curlybracket_start_pos = -1;
            int curlybracket_end_pos = -1;
            int start_col_pos = col_pos;
            int start_row_pos = row_pos;

            // 문장을 기호로 분해한다.
            for (int i = 0; i < size; i++)
            {
                if (open3)
                {
                    if (body[i] == '"')
                    {
                        open3 = false;
                    }
                }
                else if (body[i] == '"')
                {
                    open3 = true;
                }
                else if (body[i] == '(')
                {
                    open1++;
                }
                else if (body[i] == ')')
                {
                    open1--;
                }
                else if (body[i] == '[')
                {
                    open2++;

                    if (open1 == 0 && count_curlybracket == 0 && open2 == 1 && dimension_start_pos == -1)
                    {
                        dimension_start_pos = i;
                        splitDemension.col_pos = col_pos;
                        splitDemension.row_pos = row_pos;
                    }
                }
                else if (count_curlybracket == 0 && body[i] == ']')
                {
                    open2--;
                    if (open2 == 0)
                        dimension_end_pos = i;
                }
                else if (body[i] == '{')
                {
                    count_curlybracket++;
                    if (count_curlybracket == 1 && curlybracket_start_pos == -1)
                    {
                        curlybracket_start_pos = i;
                        splitCurlyBracket.col_pos = col_pos;
                        splitCurlyBracket.row_pos = row_pos;
                    }
                }
                else if (body[i] == '}')
                {
                    count_curlybracket--;
                    if (count_curlybracket == 0)
                        curlybracket_end_pos = i;
                }
                else if (open1 == 0 && open2 == 0 && count_curlybracket==0)
                {
                    if (body[i] == '+' && i+1 < size && body[i + 1] == '+')     // ++연산자
                    {
                        // ++a + b  는 (++a) + (b) 와 같이 연산한다.
                        // a++ + b  는 (a++) + (b) 와 같이 연산한다.

                        main.CalcCursorPos(body[i], ref col_pos, ref row_pos);  
                        i++;
                    }
                    else if (body[i] == '-' && i + 1 < size && body[i + 1] == '-') // --연산자
                    {
                        // ++a + b  는 (++a) + (b) 와 같이 연산한다.
                        // a++ + b  는 (a++) + (b) 와 같이 연산한다.
                        main.CalcCursorPos(body[i], ref col_pos, ref row_pos);
                        i++;
                    }
                    else if (body[i] == '<' && i + 1 < size && body[i + 1] == '<') // << 연산자
                    {
                        SeekedChar sc = new SeekedChar();
                        sc.ch = body[i];
                        sc.pos = i;
                        sc.col_pos = col_pos;
                        sc.row_pos = row_pos;
                        arrayChar.Add(sc);

                        main.CalcCursorPos(body[i], ref col_pos, ref row_pos);
                        i++;
                    }
                    else if (body[i] == '>' && i + 1 < size && body[i + 1] == '>') // >> 연산자
                    {
                        SeekedChar sc = new SeekedChar();
                        sc.ch = body[i];
                        sc.pos = i;
                        sc.col_pos = col_pos;
                        sc.row_pos = row_pos;
                        arrayChar.Add(sc);

                        main.CalcCursorPos(body[i], ref col_pos, ref row_pos);
                        i++;
                    }
                    else if (body[i] == '+' || body[i] == '-' || body[i] == '*' || body[i] == '/' ||
                        body[i] == '|' || body[i] == '&' || body[i] == '^' || body[i] == '%')
                    {
                        SeekedChar sc = new SeekedChar();
                        sc.ch = body[i];
                        sc.pos = i;
                        sc.col_pos = col_pos;
                        sc.row_pos = row_pos;
                        arrayChar.Add(sc);
                    }
                }

                main.CalcCursorPos(body[i], ref col_pos, ref row_pos);
            }

            // 우선순위가 낮은것 부터 뒤에서 부터 처리한다.
            for (int i = arrayChar.Count - 1; i >= 0; i--)
            {
                if (arrayChar[i].ch == '+')
                {
                    if (arrayChar[i].pos == 0)  // a = +b 와 같이 +앞에 아무것도 없는 경우
                    {
                        if (!lv.Split(main, file, body.Substring(1, body.Length - 1), col_pos, row_pos)) return false;
                        eCalcMethod = EnumCalcMethod.UnaryPlus;
                        rightVal = null;
                        return true;
                    }

                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 1), arrayChar[i].col_pos+1, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.Plus;
                    return true;
                }
                else if (arrayChar[i].ch == '-')
                {
                    if (arrayChar[i].pos == 0)  // a = -b 와 같이 +앞에 아무것도 없는 경우
                    {
                        if (!lv.Split(main, file, body.Substring(1, body.Length - 1), col_pos, row_pos)) return false;
                        eCalcMethod = EnumCalcMethod.UnaryMinus;
                        rightVal = null;
                        return true;
                    }

                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 1), arrayChar[i].col_pos + 1, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.Minus;
                    return true;
                }
                else if (arrayChar[i].ch == '%')
                {
                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 1), arrayChar[i].col_pos + 1, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.Remainder;
                    return true;
                }
                else if (arrayChar[i].ch == '|')
                {
                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 1), arrayChar[i].col_pos + 1, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.Or;
                    return true;
                }
                else if (arrayChar[i].ch == '&')
                {
                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 1), arrayChar[i].col_pos + 1, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.And;
                    return true;
                }
                else if (arrayChar[i].ch == '^')
                {
                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 1), arrayChar[i].col_pos + 1, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.Xor;
                    return true;
                }
                else if (arrayChar[i].ch == '<')    // <<
                {
                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 2), arrayChar[i].col_pos + 2, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.LeftShift;
                    return true;
                }
                else if (arrayChar[i].ch == '>')    // >>
                {
                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 2), arrayChar[i].col_pos + 2, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.RightShift;
                    return true;
                }
            }
            // 
            for (int i = arrayChar.Count - 1; i >= 0; i--)
            {
                if (arrayChar[i].ch == '*')
                {
                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 1), arrayChar[i].col_pos + 1, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.Multiply;
                    return true;
                }
                else if (arrayChar[i].ch == '/')
                {
                    if (!lv.Split(main, file, body.Substring(0, arrayChar[i].pos), start_col_pos, start_row_pos)) return false;
                    if (!rv.Split(main, file, body.Substring(arrayChar[i].pos + 1), arrayChar[i].col_pos + 1, arrayChar[i].row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.Divide;
                    return true;
                }
            }

            if (body.Length >= 2 && body[0] == '(')
            {
                int index = 1;
                if (!SeekPosCloseParenthesis(main, body, ref index, body.Length - 1))
                {
                    return false;
                }

                // 맨 앞뒤가 (abc) 와 같이 ()로 묶여진 값이다.
                if (index == body.Length - 1)
                {
                    if (!lv.Split(main, file, body.Substring(1, body.Length - 2), col_pos + 1, row_pos)) return false;
                    eCalcMethod = EnumCalcMethod.Parenthesis;

                    rightVal = null;    // 오른쪽 값은 쓰지 않는다.

                    return true;
                }
                // ()가 맨앞뒤가 아니면 (abc)abc 와 같은 CAST 형식이다.
                else
                {
                    sLastValue = body.Substring(1, index - 1);
                    
                    if (!lv.Split(main, file, body.Substring(index+1), col_pos + index+1, row_pos)) return false;

                    eCalcMethod = EnumCalcMethod.Cast;

                    rightVal = null;    // 오른쪽 값은 쓰지 않는다.

                    return true;
                }
            }

            if (body.Length >= 2 && body[0] == '+' && body[1] == '+')
            {
                if (!lv.Split(main, file, body.Substring(2, body.Length - 2), col_pos + 2, row_pos)) return false;
                eCalcMethod = EnumCalcMethod.PrefixIncrement;
                rightVal = null;
                return true;
            }
            if (body.Length >= 2 && body[0] == '-' && body[1] == '-')
            {
                if (!lv.Split(main, file, body.Substring(2, body.Length - 2), col_pos + 2, row_pos)) return false;
                eCalcMethod = EnumCalcMethod.PrefixDecrement;
                rightVal = null;
                return true;
            }
            if (body.Length >= 2 && body[body.Length - 2] == '+' && body[body.Length - 1] == '+')
            {
                if (!lv.Split(main, file, body.Substring(0, body.Length - 2), col_pos, row_pos)) return false;
                eCalcMethod = EnumCalcMethod.PostfixIncrement;
                rightVal = null;
                return true;
            }
            if (body.Length >= 2 && body[body.Length - 2] == '-' && body[body.Length - 1] == '-')
            {
                if (!lv.Split(main, file, body.Substring(0, body.Length - 2), col_pos, row_pos)) return false;
                eCalcMethod = EnumCalcMethod.PostfixDecrement;
                rightVal = null;
                return true;
            }

            if (body.Length >= 2 && body[0] == '!')
            {
                if (!lv.Split(main, file, body.Substring(1, body.Length - 1), col_pos, row_pos)) return false;
                eCalcMethod = EnumCalcMethod.UnaryNegation;
                rightVal = null;
                return true;
            }
            if (body.Length >= 2 && body[0] == '~')
            {
                if (!lv.Split(main, file, body.Substring(1, body.Length - 1), col_pos, row_pos)) return false;
                eCalcMethod = EnumCalcMethod.UnaryBitwiseComplement;
                rightVal = null;
                return true;
            }

            if (curlybracket_start_pos != -1)   // 초기화가 있는 배열 선언이다.
            {
                splitCurlyBracket.block = body.Substring(curlybracket_start_pos, curlybracket_end_pos - curlybracket_start_pos + 1);
                pInitial = new EditInitialValue(parentMethod, parentBlock);
                int start = 0;
                if (!((EditInitialValue)pInitial).Split(main, file, splitCurlyBracket.block, ref start, splitCurlyBracket.col_pos, splitCurlyBracket.row_pos)) return false;
            }

            if (dimension_start_pos != -1)
            {
                splitDemension.block = body.Substring(dimension_start_pos, dimension_end_pos - dimension_start_pos + 1);
                pJaggedArrays = new EditJaggedArrays(parentMethod, parentBlock);
                if (!((EditJaggedArrays)pJaggedArrays).Split(main, file, splitDemension.block, splitDemension.col_pos, splitDemension.row_pos)) return false;
            }

            eCalcMethod = EnumCalcMethod.LastValue;

            if (pJaggedArrays != null)
            {
                body = body.Substring(0, dimension_start_pos);
            }
            else if (pInitial != null)
            {
                body = body.Substring(0, curlybracket_start_pos);
            }

            // 마지막 값일 때는 오른쪽 왼쪽 값은 없다.
            leftVal = null;     // 왼쪽 값은 쓰지 않는다.
            rightVal = null;    // 오른쪽 값은 쓰지 않는다.

            sLastValue = body;

            int pre_defined_value;
            
            // new 는 클래스를 할당하는 경우이다.
            // new Class()  new int[30]

            if (String.Compare(body, 0, "new ", 0, 4) == 0)
            {
                col_pos += 4;
                body = body.Substring(4);
                body = ScriptLibTools.TrimWithCursorPos(body, ref col_pos, ref row_pos);

                sLastValue = body;

                if (pJaggedArrays != null || pInitial != null)
                {
                    eValueType = EnumLastValueType.NewArray;
                }
                else
                {
                    eValueType = EnumLastValueType.NewClass;
                    eConstantType = EnumVarType.TypeClass;

                    pMethod = new EditCommandMethod((EditScriptLibMethod)parentMethod, parentBlock);
                    ((EditCommandMethod)pMethod).Split(main, file, body, col_pos, row_pos);
                }
                return true;
            }
            else if (body.Length >= 2 && body[0] == '"' && body[body.Length - 1] == '"')   // 이 체크가 아래의 함수형 체크보다 우선해야 한다. "()" 처럼 ""속에 괄호가 있을 수 있다.
            {
                eValueType = EnumLastValueType.Constant;
                eConstantType = EnumVarType.TypeString;
                sLastValue = ConvertStringFromSourceString(sLastValue);
                return true;
            }
            else if (body.Length >= 2 && body[0] == '\'' && body[body.Length - 1] == '\'')   
            {
                eValueType = EnumLastValueType.Constant;
                eConstantType = EnumVarType.TypeChar;

                if (body.Length == 2)
                {
                    main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "Empty character literal");
                    return false;
                }
                else if (body.Length == 3)
                {
                    sLastValue = body[1].ToString();
                }
                else // '\r' '\u0080', 같은 esc 문자이다.
                {
                    // escape 문자는 ', ", \, 0, a, b, f, n, r, t, u, U, x, v  의 14개가 있다.

                    if (body[1] != '\\')
                    {
                        main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "Too many characters in character literal");
                        return false;
                    }
                    if (body[2] == '\'' && body.Length == 4)
                    {
                        sLastValue = ((int)'\'').ToString();
                    }
                    else if (body[2] == '"' && body.Length == 4)
                    {
                        sLastValue = ((int)'"').ToString();
                    }
                    else if (body[2] == '\\' && body.Length == 4)
                    {
                        sLastValue = ((int)'\\').ToString();
                    }
                    else if (body[2] == '0' && body.Length == 4)
                    {
                        sLastValue = ((int)'\0').ToString();
                    }
                    else if (body[2] == 'a' && body.Length == 4)
                    {
                        sLastValue = ((int)'\a').ToString();
                    }
                    else if (body[2] == 'b' && body.Length == 4)
                    {
                        sLastValue = ((int)'\b').ToString();
                    }
                    else if (body[2] == 'f' && body.Length == 4)
                    {
                        sLastValue = ((int)'\f').ToString();
                    }
                    else if (body[2] == 'n' && body.Length == 4)
                    {
                        sLastValue = ((int)'\n').ToString();
                    }
                    else if (body[2] == 'r' && body.Length == 4)
                    {
                        sLastValue = ((int)'\r').ToString();
                    }
                    else if (body[2] == 't' && body.Length == 4)
                    {
                        sLastValue = ((int)'\t').ToString();
                    }
                    else if ((body[2] == 'u' || body[2] == 'U'))    // \u0000~\uFFFF 값이어야 한다.
                    {
                        if (body.Length != 8)
                        {
                            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "\\{0} escape sequence must be \\{0}0000~\\{0}FFFF", body[2]);
                            return false;
                        }
                        sLastValue = ConvertTool.ToUint64FromHexBuf(body.Substring(3)).ToString();
                    }
                    else if (body[2] == 'x' && body.Length > 4)                         // \x0~\xFFFF까지의 값이어야 한다.
                    {
                        sLastValue = ConvertTool.ToUint64FromHexBuf(body.Substring(3)).ToString();
                    }
                    else if (body[2] == 'v' && body.Length == 4)
                    {
                        sLastValue = ((int)'\v').ToString();
                    }
                    else
                    {
                        main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "Unrecognized escape sequence");
                        return false;
                    }
                }
                                
                return true;
            }
            else if (body.IndexOf('(') != -1)  // 함수형이다.
            {
                eValueType = EnumLastValueType.Method;
                pMethod = new EditCommandMethod((EditScriptLibMethod)parentMethod, parentBlock);
                return ((EditCommandMethod)pMethod).Split(main, file, body, col_pos, row_pos);
            }
            else if (IsPreDefinedValue(sLastValue, out pre_defined_value))
            {
                eValueType = EnumLastValueType.Constant;
                eConstantType = EnumVarType.TypeInt;
                sLastValue = pre_defined_value.ToString();
                return true;
            }
            else if (sLastValue == "true")
            {
                eValueType = EnumLastValueType.Constant;
                eConstantType = EnumVarType.TypeBool;
                sLastValue = "1";
                return true;
            }
            else if (sLastValue == "false")
            {
                eValueType = EnumLastValueType.Constant;
                eConstantType = EnumVarType.TypeBool;
                sLastValue = "0";
                return true;
            }
            else if (sLastValue == "null")
            {
                eValueType = EnumLastValueType.Constant;
                eConstantType = EnumVarType.TypeNull;
                sLastValue = "null";
                return true;
            }
            else
            {

            }

            // 16진수 인가를 검사한다.
            if (sLastValue.Length > 2)
            {
                // 16진수
                if (sLastValue[0] == '0' && (sLastValue[1] == 'x' || sLastValue[1] == 'X'))
                {
                    for (int i = 2; i < sLastValue.Length; i++)
                    {
                        if (sLastValue[i] >= '0' && sLastValue[i] <= '9')
                        {

                        }
                        else if (sLastValue[i] >= 'a' && sLastValue[i] <= 'f')
                        {

                        }
                        else if (sLastValue[i] >= 'A' && sLastValue[i] <= 'F')
                        {

                        }
                        else
                        {
                            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "{0} is not a Hexadecimal value", sLastValue);
                            return false;
                        }
                    }
                    eValueType = EnumLastValueType.Constant;
                    eConstantType = EnumVarType.TypeInt;
                    sLastValue = ConvertTool.ToUint64FromHexBuf(sLastValue.Substring(2)).ToString();
                    return true;
                }
            }

            bool flag_dot = false;

            // 숫자형인가를 검사한다.
            for (int i = 0; i < sLastValue.Length; i++)
            {
                if (sLastValue[i] >= '0' && sLastValue[i] <= '9')
                {

                }
                else if (sLastValue[i] == '.')
                {
                    flag_dot = true;
                }
                else
                {
                    goto not_numeric;
                }
            }

            eValueType = EnumLastValueType.Constant;

            if (flag_dot)
                eConstantType = EnumVarType.TypeDouble;
            else
                eConstantType = EnumVarType.TypeInt;
            return true;

        not_numeric:
            
            eValueType = EnumLastValueType.Variable;
            sLastValue = body;

            return true;
        }

        ScriptLibMemberPublic pCompileMemberPublic = null;      // 컴파일 시 찾은 포인터를 기억해 둔다.

        public void Compile(ScriptLibMain main)
        {
            if (eCalcMethod == EnumCalcMethod.LastValue)
            {
                if (pJaggedArrays != null)
                {
                    ((EditJaggedArrays)pJaggedArrays).Compile(main);
                }
                if (pInitial != null)
                {
                    ((EditInitialValue)pInitial).Compile(main);
                }

                if (eValueType == EnumLastValueType.Method)
                {
                    pMethod.Compile(main);
                }
                else if (eValueType == EnumLastValueType.None)
                {
                    // 값이 없는 경우
                }
                else if (eValueType == EnumLastValueType.Constant)
                {

                }
                else if (eValueType == EnumLastValueType.NewClass)
                {
                    string name_namespace, name_class;
                    ScriptLibTools.SplitNamespaceClass(pMethod.sMethodName, out name_namespace, out name_class);

                    ScriptLibClass slc;

                    EditScriptLibNamespace esln = null;

                    if (parentMethod != null)
                        esln = (EditScriptLibNamespace)(parentMethod.parentClass.parentNamespace);

                    if (!((EditScriptLibMain)main).SeekClassWithError(esln, ((EditScriptLibClass)parentMethod.parentClass).sourceFile, ref name_namespace, name_class, out slc, nColumn, nRow))
                    {
                        return;
                    }

                    // 생성자 Method는 Class명과 동일하다. 
                    pMethod.sMethodName = String.Format("{0}.{1}.{2}", name_namespace, name_class, name_class);
                    pMethod.Compile(main);
                }
                else if (eValueType == EnumLastValueType.NewArray)
                {
                    if (pJaggedArrays != null)
                        ((EditJaggedArrays)pJaggedArrays).Compile(main);

                    if(pInitial != null)
                        ((EditInitialValue)pInitial).Compile(main);
                }
                else if (eValueType == EnumLastValueType.InitArray)
                {
                    ((EditInitialValue)pInitial).Compile(main);
                }
                else
                {
                    // 변수는 숫자로 시작하면 안된다.
                    if (sLastValue[0] >= '0' && sLastValue[0] <= '9')
                    {
                        main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "Variable name must start with alphabet name='{0}'", sLastValue);
                        return;
                    }

                    // 외부 스크립트의 변수인가를 검사한다.
                    if (main.scriptExternal != null)
                    {
                        int retn = main.scriptExternal.IsExistVariable(sLastValue);
                        if (retn == 1)
                        {
                            eValueType = EnumLastValueType.ExternalVariable;
                            return;
                        }
                        else if (retn == 2)
                        {
                            eValueType = EnumLastValueType.ExternalVariable;
                            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, main.scriptExternal.ErrorType, main.scriptExternal.ErrorMessage);
                            return;
                        }
                    }

                    // namespace, class, method 를 분리한다.
                    ScriptLibTools.SplitNamespaceClassMethod(sLastValue, out sCompiledNamespace, out sCompiledClass, out sCompiledMethod);

                    // NameSpace.Class.Variable 형이 아닌 Variable형만 있는 경우 동적 선언된 변수인가를 검사한다.
                    if (sCompiledClass == null)
                    {
                        Variable var;
                        object finded_pos;
                        // parentBlock이 null인 경우는 class에서 선언된 변수형이므로 parent블럭이 없다.
                        if (parentBlock != null && ((EditCommandBlock)parentBlock).IsExistVariableOnCompile(sLastValue, out var, out finded_pos))
                        {
                            // class 에서 찾은 변수일 경우
                            if (finded_pos.GetType() == typeof(EditScriptLibClassVariable))
                            {

                                // static Method에서 non-static 변수를 호출할 수는 없다.
                                if (parentMethod.bStatic && !((ScriptLibMemberVariable)finded_pos).bStatic)
                                {
                                    main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "An object reference is required for th non-static field, method, or property '{0}'", sLastValue);
                                    return;
                                }
                                // 찾은 변수가 자신의 클래스의 static 변수인 경우
                                else if (((ScriptLibMemberVariable)finded_pos).bStatic || ((ScriptLibMemberVariable)finded_pos).pVar.bConst)
                                {
                                    sCompiledClass = parentMethod.parentClass.sNameClass;
                                    sCompiledNamespace = ((EditScriptLibClass)parentMethod.parentClass).parentNamespace.sNameNamespace;
                                    eValueType = EnumLastValueType.StaticVariable;

                                    pCompileMemberPublic = (ScriptLibMemberPublic)finded_pos;   // 컴파일 시 찾은 포인터를 기억해 둔다.
                                    return;
                                }
                            }

                            eValueType = EnumLastValueType.Variable;
                            return;
                        }

                        main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The name '{0}' does not exist in the current context", sLastValue);

                        return;
                    }

                    ScriptLibMemberMethod slm;

                    // namespace가 없고 class명이 있으면 할당된 클래스의 변수이거나 Property 일 수 있다. ex.Year 같은 형식
                    if (sCompiledNamespace == null && sCompiledClass != null)
                    {
                        Variable var;
                        object finded_pos;
                        if (((EditCommandBlock)parentBlock).IsExistVariableOnCompile(sCompiledClass, out var, out finded_pos))
                        {
                            // 선언된 형식은 배열인데 배열을 사용하지 않았으면 Array 속성만을 사용할 수 있다.
                            if (var.nDimensionals != null && pJaggedArrays == null)
                            {
                                if (sCompiledMethod == "Length")
                                {
                                    eValueType = EnumLastValueType.Array_Length;
                                    return;
                                }
                            }

                            // 선언된 변수가 있으면 선언된 형식의 클래스를 찾아서 속성이 존재하는가를 체크한다.
                            for (int l = 0; l < main.arrayLibrary.Count; l++)
                            {
                                for (int i = 0; i < main.arrayLibrary[l].arrayNamespace.Count; i++)
                                {
                                    ScriptLibNamespace sln = main.arrayLibrary[l].arrayNamespace[i];

                                    if (sln.sNameNamespace != var.sCompiledNamespace) continue;

                                    for (int j = 0; j < sln.arrayMember.Count; j++)
                                    {
                                        if (sln.arrayMember[j].GetName() == var.sCompiledClass)
                                        {
                                            if (sln.arrayMember[j].eMember == EnumScriptLibMember.Class)
                                            {
                                                ScriptLibClass slc = (ScriptLibClass)sln.arrayMember[j];

                                                slm = slc.GetPropertyPointer(sCompiledMethod);

                                                // 클래스안에서 함수로 발견된다면 Property 이다.
                                                if (slm != null)
                                                {
                                                    if (slm.eAccessLevel != EnumAccessLevel.access_public)
                                                    {
                                                        main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "'{0}.{1}' is inaccessible due to its protection level", var.sCompiledClass, sCompiledMethod);
                                                        return;
                                                    }
                                                    pCompileMemberPublic = slm;
                                                    eValueType = EnumLastValueType.AllocProperty;
                                                    pMethod = new EditCommandMethod((EditScriptLibMethod)parentMethod, parentBlock);
                                                    pMethod.sMethodName = sLastValue;
                                                    pMethod.Compile(main);
                                                    return;
                                                }

                                                ScriptLibMemberVariable slcv = slc.GetVariablePointer(main, sCompiledMethod);

                                                // 클래스안에서 변수로 발견된다면 
                                                if (slcv != null)
                                                {
                                                    if (slcv.eAccessLevel != EnumAccessLevel.access_public)
                                                    {
                                                        main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "'{0}.{1}' is inaccessible due to its protection level", var.sCompiledClass, sCompiledMethod);
                                                        return;
                                                    }

                                                    pCompileMemberPublic = slcv;
                                                    eValueType = EnumLastValueType.AllocVariable;
                                                    return;
                                                }
                                                else
                                                {
                                                    main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The variable or perperty '{0}' does not exist in the class '{1}'", sCompiledMethod, sCompiledClass);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "클래스의 변수나 메소드만 변수의 인자로 사용할 수 있습니다. '{0}'", sCompiledMethod);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }

                            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The variable or property '{0}' does not exist in the variable '{1}'", sCompiledMethod, var.sVarName);

                            return;
                        } // (EditCommandBlock)parentBlock).IsExistVariableOnCompile
                    }

                    EditScriptLibClass eslc = (EditScriptLibClass)parentMethod.parentClass;

                    // 할당된 클래스의 메소드가 아니면 static형 메소드이다.
                    ScriptLibMemberPublic slmp;
                    if (((EditScriptLibMain)main).SeekStaticPropertyVariableWithError(eslc, eslc.sourceFile, ref sCompiledNamespace, ref sCompiledClass, sCompiledMethod, out slmp, nColumn, nRow))
                    {
                        pCompileMemberPublic = slmp;

                        if (slmp.eMember == EnumScriptLibMember.Method)
                            eValueType = EnumLastValueType.StaticProperty;
                        else if (slmp.eMember == EnumScriptLibMember.EnumItem)
                        {
                            eValueType = EnumLastValueType.StaticVariable;  // StaticVariable을 함께 사용한다.
                        }
                        else
                            eValueType = EnumLastValueType.StaticVariable;

                        return;
                    }

                    return;
                }
            }
            else if(eCalcMethod == EnumCalcMethod.Parenthesis)
            {
                if (leftVal != null) ((EditRecursiveValue)leftVal).Compile(main);
            }
            else if (eCalcMethod == EnumCalcMethod.Cast)
            {
                if (leftVal != null) ((EditRecursiveValue)leftVal).Compile(main);
            }
            else
            {
                if (leftVal != null) ((EditRecursiveValue)leftVal).Compile(main);
                if (rightVal != null) ((EditRecursiveValue)rightVal).Compile(main);
            }
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            writer.WriteColRow(nColumn, nRow);

            // 한번에 하는 것이 파일 크기를 줄일 수 있다.
            writer.WriteLastValueProperty(eCalcMethod, eValueType, eConstantType);

            if (eCalcMethod == EnumCalcMethod.LastValue)
            {
                writer.WriteString(EnumBlockType.sLastValue, sLastValue);
            }
            else if (eCalcMethod == EnumCalcMethod.Parenthesis)
            {
                writer.WriteString(EnumBlockType.SourceString, sSourceString);
                ((EditRecursiveValue)leftVal).SaveToStream(writer, tab_depth + 1);
            }
            else if (eCalcMethod == EnumCalcMethod.Cast)
            {
                writer.WriteString(EnumBlockType.SourceString, sSourceString);
                writer.WriteString(EnumBlockType.sLastValue, sLastValue);           // (char) 에서 char부분만 저장된다.
                // ( type )  이 추가 되어야 한다.
                ((EditRecursiveValue)leftVal).SaveToStream(writer, tab_depth + 1);
            }
            else
            {
                writer.WriteString(EnumBlockType.SourceString, sSourceString);
                ((EditRecursiveValue)leftVal).SaveToStream(writer, tab_depth + 1);
                if(rightVal != null)
                    ((EditRecursiveValue)rightVal).SaveToStream(writer, tab_depth + 1);
            }

            writer.WriteNameSplit(sCompiledNamespace, sCompiledClass, sCompiledMethod);

            if (pMethod != null)
                pMethod.SaveToStream(writer, tab_depth + 1);

            if (pJaggedArrays != null)
                ((EditJaggedArrays)pJaggedArrays).SaveToStream(writer, tab_depth + 1);
            if (pInitial != null)
                ((EditInitialValue)pInitial).SaveToStream(writer, tab_depth + 1);

            parent_writer.WriteBlock(EnumBlockType.RecursiveValue, writer);
        }

        public void CheckSetable(ScriptLibMain main)
        {
            if (eCalcMethod != EnumCalcMethod.LastValue)
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "Only variable or property is setable '{0}'", sLastValue);
                return;
            }

            if (eValueType == EnumLastValueType.AllocProperty)
            {
                if (pCompileMemberPublic != null)
                {
                    if (pCompileMemberPublic.eMember == EnumScriptLibMember.Method)
                    {
                        EditScriptLibMethod eslm = (EditScriptLibMethod)pCompileMemberPublic;
                        if (!eslm.bPropertySet)
                        {
                            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "Property or indexer '{0}' cannot be assigned to -- it is read only", sLastValue);
                            return;
                        }
                    }
                }
            }
            else if (eValueType == EnumLastValueType.AllocVariable)
            {

            }
            else if (eValueType == EnumLastValueType.ExternalVariable)
            {

            }
            else if (eValueType == EnumLastValueType.StaticProperty)
            {

            }
            else if (eValueType == EnumLastValueType.StaticVariable)
            {
                if (pCompileMemberPublic != null)
                {
                    if (pCompileMemberPublic.eMember == EnumScriptLibMember.EnumItem)
                    {
                        main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "Enum member '{0}' is a read only", sLastValue);
                        return;
                    }
                    else if (pCompileMemberPublic.eMember == EnumScriptLibMember.Variable)
                    {
                        ScriptLibMemberVariable slmv = (ScriptLibMemberVariable)pCompileMemberPublic;
                        if (slmv.pVar.bConst)
                        {
                            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The '{0}' is a read only (const)", sLastValue);
                        }
                        else if (slmv.pVar.bReadonly)
                        {
                            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The '{0}' is a 'readonly' variable", sLastValue);
                        }
                        return;
                    }
                }
            }
            else if (eValueType == EnumLastValueType.Variable)
            {
                Variable var;
                object finded_pos;
                // parentBlock이 null인 경우는 class에서 선언된 변수형이므로 parent블럭이 없다.
                if (parentBlock != null && ((EditCommandBlock)parentBlock).IsExistVariableOnCompile(sLastValue, out var, out finded_pos))
                {
                    if (var.bConst)
                    {
                        main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The '{0}' is a read only (const)", sLastValue);
                    }
                    else if (var.bReadonly)
                    {
                        // readonly는 생성자가 아닌곳에서는 Set을 하면 안된다.
                        if (parentMethod.sNameMethod != parentMethod.parentClass.sNameClass)
                            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The '{0}' is a 'readonly' variable", sLastValue);
                    }
                }
            }
            else
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "Only variable or property is setable '{0}'", sLastValue);
                return;
            }
        }

        /// <summary>
        /// 컴파일이 끝난 후 대입하는데 문제가 없는가를 체크한다.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="var"></param>
        /// <returns></returns>
        public void CheckSubstitution(ScriptLibMain main, EditVariable var, int depth_dimension)
        {
            if (var.nDimensionals != null)
            {
                if (eCalcMethod == EnumCalcMethod.LastValue)
                {
                    if (eValueType == EnumLastValueType.InitArray)
                    {
                        /*
                        if (depth_dimension != 0)
                        {
                            if (var.nDimensionals.Length - depth_dimension > 1)
                            {
                                main.SetError(parentMethod.parentClass.sSourceFilename, nColumn, nRow, "다중 배열의 초기값은 new를 사용해야 합니다.");
                                return;
                            }
                        }*/

                        ((EditInitialValue)pInitial).CheckSubstitution(main, var, depth_dimension);
                    }
                    else if (eValueType == EnumLastValueType.NewArray)
                    {
                        if(pInitial != null)
                            ((EditInitialValue)pInitial).CheckSubstitution(main, var, depth_dimension);
                    }
                }
            }
        }
    }
}
