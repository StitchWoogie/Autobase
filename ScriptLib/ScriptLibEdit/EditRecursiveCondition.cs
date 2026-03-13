using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditRecursiveCondition : RecursiveCondition
    {
        public EditRecursiveCondition(ScriptLibMemberMethod parent, CommandPublic parent_block)
            : base(parent, parent_block)
        {
            
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

                    if (open == 0)
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
        
        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos)
        {
            sSourceString = sBody;

            string s;

            s = ScriptLibTools.TrimWithCursorPos(sBody, ref col_pos, ref row_pos);

            // 2017-2-8 추가
            // ( ) 로 쌓여있으면 그것을 먼저 제거한다. ((( ))) 처럼 계속 있는경우도 제거된다.
            while (true)
            {
                if (s.Length < 2) break;
                if (s[0] != '(')   break;
                if(s[s.Length - 1] != ')') break;

                int index = 1;
                if (!SeekPosCloseParenthesis(main, s, ref index, s.Length - 1))
                {
                    return false;
                }

                if (index != s.Length - 1) break;
                
                // 맨 앞뒤가 (abc) 와 같이 ()로 묶여진 값이다.
                s = s.Substring(1, s.Length - 2);

                col_pos++;

                s = ScriptLibTools.TrimWithCursorPos(s, ref col_pos, ref row_pos);
            }

            SetColRow(col_pos, row_pos);

            int size = s.Length;

            int open1 = 0;  // ( )
            int open2 = 0;  // [ ]
            bool open3 = false;  // " "

            // 먼저 || 나 &&가 있는가를 검사한다.
            for (int i = 0; i < size; i++)
            {
                if (open3)
                {
                    if (s[i] == '"')
                    {
                        open3 = false;
                    }
                }
                else if (s[i] == '"')
                {
                    open3 = true;
                }
                else if (s[i] == '(')
                {
                    open1++;
                }
                else if (s[i] == ')')
                {
                    open1--;
                }
                else if (s[i] == '[')
                {
                    open2++;
                }
                else if (s[i] == ']')
                {
                    open2--;
                }
                else if (open1 == 0 && open2 == 0)
                {
                    if (i < size - 1 && s[i] == '&' && s[i + 1] == '&')
                    {
                        EditRecursiveCondition lc = new EditRecursiveCondition(parentMethod, parentBlock);
                        EditRecursiveCondition rc = new EditRecursiveCondition(parentMethod, parentBlock);

                        leftCondition = lc;
                        rightCondition = rc;

                        if (!lc.Split(main, file, s.Substring(0, i), col_pos, row_pos)) return false;
                        if (!rc.Split(main, file, s.Substring(i + 2), col_pos, row_pos)) return false;

                        eMulti = EnumMultiCompare.And;
                        return true;
                        
                    }
                    else if (i < size - 1 && s[i] == '|' && s[i + 1] == '|')
                    {
                        EditRecursiveCondition lc = new EditRecursiveCondition(parentMethod, parentBlock);
                        EditRecursiveCondition rc = new EditRecursiveCondition(parentMethod, parentBlock);

                        leftCondition = lc;
                        rightCondition = rc;

                        if (!lc.Split(main, file, s.Substring(0, i), col_pos, row_pos)) return false;
                        if (!rc.Split(main, file, s.Substring(i + 2), col_pos, row_pos)) return false;
                        eMulti = EnumMultiCompare.Or;
                        return true;
                    }
                    else
                    {
                        // or and 연산이 있는 경우도 있으므로 오류를 발생하지 않는다.
                    }

                }
            }

            EditRecursiveValue lv;
            EditRecursiveValue rv;

            eMulti = EnumMultiCompare.One;
            eCompare = EnumCompareMethod.Unknown;

            open1 = 0;  // ( )
            open2 = 0;  // [ ]
            open3 = false;  // " "

            for (int i = 0; i < size; i++)
            {
                if (open3)
                {
                    if (s[i] == '"')
                    {
                        open3 = false;
                    }
                }
                else if (s[i] == '"')
                {
                    open3 = true;
                }
                else if (s[i] == '(')
                {
                    open1++;
                }
                else if (s[i] == ')')
                {
                    open1--;
                }
                else if (s[i] == '[')
                {
                    open2++;
                }
                else if (s[i] == ']')
                {
                    open2--;
                }
                else if (open1 == 0 && open2 == 0)
                {
                    int compare_size = 0;   
                    if (s[i] == '>' && s[i + 1] == '=')
                    {
                        eCompare = EnumCompareMethod.BigEqual;
                        compare_size = 2;
                    }
                    else if (s[i] == '<' && s[i + 1] == '=')
                    {
                        eCompare = EnumCompareMethod.SmallEqual;
                        compare_size = 2;
                    }
                    else if (s[i] == '=' && s[i + 1] == '=')
                    {
                        eCompare = EnumCompareMethod.Equal;
                        compare_size = 2;
                    }
                    else if (s[i] == '!' && s[i + 1] == '=')
                    {
                        eCompare = EnumCompareMethod.NotEqual;
                        compare_size = 2;
                    }
                    else if (s[i] == '<')
                    {
                        eCompare = EnumCompareMethod.Small;
                        compare_size = 1;
                    }
                    else if (s[i] == '>')
                    {
                        eCompare = EnumCompareMethod.Big;
                        compare_size = 1;
                    }
                    else {

                    }

                    if(compare_size > 0) {
                        lv = new EditRecursiveValue(parentMethod, parentBlock);
                        rv = new EditRecursiveValue(parentMethod, parentBlock);

                        leftVal = lv;
                        rightVal = rv;

                        if (!lv.Split(main, file, s.Substring(0, i), col_pos, row_pos)) return false;
                        if (!rv.Split(main, file, s.Substring(i + compare_size), col_pos, row_pos)) return false;

                        return true;
                    }

                }
            }

            lv = new EditRecursiveValue(parentMethod, parentBlock);

            leftVal = lv;

            if (s[0] == '!')
            {
                eCompare = EnumCompareMethod.False;
                if (!lv.Split(main, file, s.Substring(1), col_pos, row_pos)) return false;
            }
            else
            {
                eCompare = EnumCompareMethod.True;
                if (!lv.Split(main, file, s, col_pos, row_pos)) return false;
            }

            return true;
        }

        public void Compile(ScriptLibMain main)
        {
            if (eMulti == EnumMultiCompare.One)
            {
                ((EditRecursiveValue)leftVal).Compile(main);
                if (rightVal != null)
                {
                    ((EditRecursiveValue)rightVal).Compile(main);
                }
            }
            else {
                ((EditRecursiveCondition)leftCondition).Compile(main);
                ((EditRecursiveCondition)rightCondition).Compile(main);
            }
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            SavePublic(writer, tab_depth + 1);
            writer.WriteString(EnumBlockType.SourceString, sSourceString);
            writer.WriteByte(EnumBlockType.eCompareMulti, (byte)eMulti);
            if (eMulti == EnumMultiCompare.One)
            {
                writer.WriteByte(EnumBlockType.eCompare, (byte)eCompare);
                ((EditRecursiveValue)leftVal).SaveToStream(writer, tab_depth + 1);
                if (rightVal != null)
                    ((EditRecursiveValue)rightVal).SaveToStream(writer, tab_depth + 1);
            }
            else
            {
                ((EditRecursiveCondition)leftCondition).SaveToStream(writer, tab_depth + 1);
                ((EditRecursiveCondition)rightCondition).SaveToStream(writer, tab_depth + 1);
            }

            parent_writer.WriteBlock(EnumBlockType.Condition, writer);
        }

    }
}
