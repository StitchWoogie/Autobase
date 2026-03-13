using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;

namespace ScriptLibEdit
{
    public class EditScriptLibClassVariable : ScriptLibMemberVariable
    {
        EditScriptLibClass parentClass;

        public EditScriptLibClassVariable(EditScriptLibClass parent)
        {
            parentClass = parent;
            pVar = new EditVariable();
        }

        ColumnRowInfo columnRowInfo = new ColumnRowInfo();
        /*
        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos)
        {
            return ((EditRecursiveValue)pValue).Split(main, file, sBody, col_pos, row_pos);
        }*/

        public int nColumn, nRow;

        public void Compile(ScriptLibMain main)
        {
            // 2020-3-23 Compile() 포함시킴. 이전에는 제외되어 있었다.  이것이 없으니 변수의 종류가 계산되지 않아서 int 변수에 소수점을 대입하면 소수점이 살아있는 현상이 발생한다.
            // 왜 빠져있었는지는 알 수 없음.
            ((EditVariable)pVar).Compile(main, null, nColumn, nRow);    

            if (pValue != null)
            {
                ((EditRecursiveValue)pValue).Compile(main);
            }
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            /*
            if (bConst)
            {
                writer.WriteBool(EnumBlockType.bConst, bConst);
            }*/

            if (bStatic)
            {
                writer.WriteBool(EnumBlockType.bStatic, bStatic);
            }

            if (eAccessLevel != EnumAccessLevel.access_private)
            {
                writer.WriteAccessLevel(eAccessLevel);
            }

            ((EditVariable)pVar).SaveToStream(writer, tab_depth + 1);

            if (pValue != null)
            {
                ((EditRecursiveValue)pValue).SaveToStream(writer, tab_depth + 1);
            }

            parent_writer.WriteBlock(EnumBlockType.ClassVariable, writer);
        }

        
    }
}
