using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditCommandDeclaration : CommandDeclaration
    {
        public EditCommandDeclaration(EditScriptLibMethod parent, CommandBlock parent_block)
            : base(parent, parent_block)
        {
            pVar = new EditVariable();
        }

        public bool Split(EditScriptLibMain main, EditScriptLibFile file, SplitedString first_word, SplitedString array_word, SplitedString second_word, string value_string, int col_pos, int row_pos, bool bConst)
        {
            SetColRow(first_word.col_pos, first_word.row_pos);  

            if (array_word != null)
            {
                if (!((EditVariable)pVar).SplitArray(main, file, array_word.block, array_word.col_pos, array_word.row_pos)) return false;
            }

            pVar.sVarType = first_word.block;
            pVar.sVarName = second_word.block;
            pVar.bConst = bConst;

            if(value_string != null)
            {
                value_string = ScriptLibTools.TrimWithCursorPos(value_string, ref col_pos, ref row_pos);

                if (value_string[value_string.Length - 1] == '}')   // 초기화가 들어 있는 루틴이다.
                {
                    // new 가 들어 있지 않고 { } 로 시작하는 경우는 처리할 필요가 있다.
                }
                
                initValue = new EditRecursiveValue(parentMethod, parentBlock);
                if (!((EditRecursiveValue)initValue).Split(main, file, value_string, col_pos, row_pos)) return false;
            }

            return true;
        }

        public override void Compile(ScriptLibMain main)
        {
            ((EditVariable)pVar).Compile(main, ((EditScriptLibClass)parentMethod.parentClass), nColumn, nRow);

            // = 초기 값이 있다.
            if (initValue != null)
            {
                ((EditRecursiveValue)initValue).Compile(main);

                ((EditRecursiveValue)initValue).CheckSubstitution(main, (EditVariable)pVar, 0);
            }
        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();
            SavePublic(writer, tab_depth + 1);
            ((EditVariable)pVar).SaveToStream(writer, tab_depth + 1);
            if (initValue != null)
            {
                ((EditRecursiveValue)initValue).SaveToStream(writer, tab_depth + 1);
            }
            parent_writer.WriteBlock(EnumBlockType.CommandDeclaration, writer);
        }
        
    }
}
