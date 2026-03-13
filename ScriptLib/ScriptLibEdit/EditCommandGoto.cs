using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditCommandGoto : CommandGoto
    {
        public EditCommandGoto(EditScriptLibMethod parent, CommandBlock parent_block)
            : base(parent, parent_block)
        {

        }

        public override void Compile(ScriptLibMain main)
        {
            // goto 문의 label은 해당 메소드의 전체가 해석되어야 존재 여부를 알 수 있다.
            if (!IsExistLabel(sNameLabel))
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "'{0}' Label 문을 찾을 수 없습니다.", sNameLabel);
                return;
            }
        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();
            SavePublic(writer, tab_depth + 1);
            writer.WriteName(sNameLabel);
            parent_writer.WriteBlock(EnumBlockType.CommandGoto, writer);
        }

        

    }
}
