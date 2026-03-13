using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditInitialValue : InitialValue
    {
        public EditInitialValue(ScriptLibMemberMethod parent, CommandPublic parent_block)
            : base(parent, parent_block)
        {
            arrayValues = new List<RecursiveValue>();
        }

        bool Add(EditScriptLibMain main, EditScriptLibFile file, string body, int col_pos, int row_pos)
        {
            EditRecursiveValue erv = new EditRecursiveValue(parentMethod, parentBlock);

            if (!erv.Split(main, file, body, col_pos, row_pos)) return false;

            arrayValues.Add(erv);
            return true;
        }

        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string body, ref int i, int col_pos, int row_pos)
        {
            i++;    // {} 블럭을 시작해서 { 다음 칸 부터 해석한다.
            col_pos++;

            int count_squrebracket = 0; // []
            int count_parenthesis = 0; // ()
            int count_curlybracket = 0;

            int word_start_pos = -1;
            int word_start_col = 0;
            int word_start_row = 0;

            char ch;

            for (; i < body.Length; i++)
            {
                ch = body[i];

                if (ch == '{')  //
                {
                    if (count_curlybracket == 0 && word_start_pos == -1)
                    {
                        word_start_pos = i;
                        word_start_col = col_pos;
                        word_start_row = row_pos;
                    }

                    count_curlybracket++;
                }
                else if (ch == '}')  //
                {
                    count_curlybracket--;

                    // -1은 블럭의 끝이다.
                    if (count_curlybracket == 0)
                    {
                        if (word_start_pos != -1)
                        {
                            string buf = body.Substring(word_start_pos, i - word_start_pos+1);  // } 까지 포함한다.

                            if (!Add(main, file, buf, word_start_col, word_start_row)) return false;

                            word_start_pos = -1;
                        }
                    }
                    else if (count_curlybracket == -1)
                    {
                        if (word_start_pos != -1)
                        {
                            string buf = body.Substring(word_start_pos, i - word_start_pos);    // } 는 포함하지 않는다.

                            if (!Add(main, file, buf, word_start_col, word_start_row)) return false;

                            word_start_pos = -1;
                        }
                    }
                }
                else if (ch == '[')
                {
                    count_squrebracket++;
                }
                else if (ch == ']')
                {
                    count_squrebracket--;
                }
                else if (ch == ',')
                {
                    if (count_parenthesis == 0 && count_squrebracket == 0 && count_curlybracket == 0 && word_start_pos != -1)
                    {
                        string buf = body.Substring(word_start_pos, i - word_start_pos);

                        if (!Add(main, file, buf, word_start_col, word_start_row)) return false;
                        
                        word_start_pos = -1;
                    }
                }
                else if (ch == '(')
                {
                    count_parenthesis++;
                }
                else if (ch == ')')
                {
                    count_parenthesis--;
                }
                else
                {
                    if (word_start_pos == -1)
                    {
                        if (ch == ' ' || ch == '\t' || ch == '\n')
                        {

                        }
                        else
                        {
                            word_start_pos = i;
                            word_start_col = col_pos;
                            word_start_row = row_pos;
                        }
                    }
                }

                main.CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            return true;
        }

        public void Compile(ScriptLibMain main)
        {
            for (int i = 0; i < arrayValues.Count; i++)
            {
                ((EditRecursiveValue)arrayValues[i]).Compile(main);
            }
        }

        public void CheckSubstitution(ScriptLibMain main, EditVariable var, int depth_dimension)
        {
                for (int i = 0; i < arrayValues.Count; i++)
                {
                    int need_array_depth = var.nDimensionals.Length - depth_dimension-1;
                    if (need_array_depth > 0)
                    {
                        if (arrayValues[i].eValueType != RecursiveValue.EnumLastValueType.NewArray)
                        {
                            main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, arrayValues[i].nColumn, arrayValues[i].nRow, EnumScriptErrorType.Else, "다중 배열의 초기값은 new를 사용해야 합니다.");
                            return;
                        }
                        else
                        {
                            if (arrayValues[i].sLastValue != var.sVarType)
                            {
                                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, arrayValues[i].nColumn, arrayValues[i].nRow, EnumScriptErrorType.Else, "new 한 데이터 형식이 선언한 형식과 다릅니다.");
                            }
                            else if (arrayValues[i].pJaggedArrays == null)
                            {
                                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, arrayValues[i].nColumn, arrayValues[i].nRow, EnumScriptErrorType.Else, "new {0}[] 형식을 사용해야 합니다.", var.sVarType);
                            }
                            else if (arrayValues[i].pJaggedArrays.arrayDimensional.Count != need_array_depth)
                            {
                                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, arrayValues[i].nColumn, arrayValues[i].nRow, EnumScriptErrorType.Else, "[] 를 {0}개 사용해야 합니다.", need_array_depth);    
                            }
                            
                        }
                    }

                    ((EditRecursiveValue)arrayValues[i]).CheckSubstitution(main, var, depth_dimension+1);
                }
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            for (int i = 0; i < arrayValues.Count; i++)
            {
                ((EditRecursiveValue)arrayValues[i]).SaveToStream(writer, tab_depth + 1);
            }

            parent_writer.WriteBlock(EnumBlockType.InitialValue, writer);
        }
    }
}
