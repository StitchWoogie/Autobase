using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditJaggedArrays : JaggedArrays
    {
        public EditJaggedArrays(ScriptLibMemberMethod parent, CommandPublic parent_block)
            : base(parent, parent_block)
        {
            
        }

        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string body, int col_pos, int row_pos)
        {
            arrayDimensional = new List<DimensionalArray>();

            int count_squrebracket = 0; // []
            int count_parenthesis = 0; // ()

            int word_start_pos = -1;
            int word_start_col = 0;
            int word_start_row = 0;

            char ch;
            EditDimensionalArray da = null;

            for (int i = 0; i < body.Length; i++)
            {
                ch = body[i];

                if (ch == '[')
                {
                    count_squrebracket++;
                    if (count_squrebracket == 1)
                    {
                        da = new EditDimensionalArray(parentMethod, parentBlock);
                        word_start_col = col_pos;
                        word_start_row = row_pos;
                        word_start_pos = i;
                    }
                }
                else if (ch == ']')
                {
                    count_squrebracket--;

                    if (count_squrebracket == 0)    // 배열하나 찾았다.
                    {
                        EditRecursiveValue value = new EditRecursiveValue(parentMethod, parentBlock);
                        string buf = body.Substring(word_start_pos + 1, i - word_start_pos - 1);   // [] 를 제외한 문장을 취한다.
                        if (!value.Split(main, file, buf, word_start_col + 1, word_start_row)) return false;
                        da.arrayValue.Add(value);

                        word_start_pos = -1;

                        arrayDimensional.Add(da);
                    }
                }
                else if (ch == ',')
                {
                    if (count_parenthesis == 0 && count_squrebracket == 1)
                    {
                        // 배열속에 들어가는 차원값
                        EditRecursiveValue value = new EditRecursiveValue(parentMethod, parentBlock);
                        string buf = body.Substring(word_start_pos + 1, i - word_start_pos - 1);
                        if (!value.Split(main, file, buf, word_start_col + 1, word_start_row)) return false;
                        da.arrayValue.Add(value);

                        word_start_col = col_pos;
                        word_start_row = row_pos;
                        word_start_pos = i;
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

                }
            }

            if (word_start_pos != -1)
            {
                main.SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "[] 의 갯수가 맞지 않습니다.");
                return false;
            }

            return true;
        }

        public void Compile(ScriptLibMain main)
        {
            if (arrayDimensional != null)
            {
                for (int i = 0; i < arrayDimensional.Count; i++)
                {
                    ((EditDimensionalArray)arrayDimensional[i]).Compile(main);
                }
            }
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            for (int i = 0; i < arrayDimensional.Count; i++)
            {
                ((EditDimensionalArray)arrayDimensional[i]).SaveToStream(writer, tab_depth + 1);
            }

            parent_writer.WriteBlock(EnumBlockType.DimensionalPosition, writer);
        }
    }
}
