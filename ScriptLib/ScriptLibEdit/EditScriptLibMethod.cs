using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;

namespace ScriptLibEdit
{
    public class EditScriptLibMethod : ScriptLibMemberMethod
    {
        public EditScriptLibFile libFile;              // 클래스는 partial로 여러개의 파일로 구분되어 질 수 있으므로 각 메소드마다 파일 구조체를 따로 보관한다.

        public EditScriptLibMethod(ScriptLibClass parent)
            : base(parent)
        {
            blockCommand = new EditCommandBlock(this, null);
        }

        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos)
        {
            sSourceFilename = file.sSourceFilename;

            nColumn = col_pos;
            nRow = row_pos;

            libFile = file;

            bool retn = ((EditCommandBlock)blockCommand).Split(main, file, sBody, col_pos, row_pos, true);

            return retn;
        }

        public void Compile(ScriptLibMain main, EditScriptLibClass parent_class, int col_pos, int row_pos)
        {
            for (int i = 0; i < arrayParams.Count; i++)
            {
                ((EditMethodArgument)arrayParams[i]).Compile(main, parent_class, col_pos, row_pos);
            }
            
            if(blockCommand != null)
                blockCommand.Compile(main);
            if(blockCommand2 != null)
                blockCommand2.Compile(main);
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            writer.WriteName(sNameMethod);
            writer.WriteSourceFilename(sSourceFilename);
            writer.WriteStatic(bStatic);
            writer.WriteAccessLevel(eAccessLevel);
            writer.WriteString(EnumBlockType.ReturnDataType, sReturnDataType);

            if (bProperty)
            {
                writer.WriteProperty(bProperty, bPropertyGet, bPropertySet);
            }

            for (int i = 0; i < arrayParams.Count; i++)
            {
                ((EditMethodArgument)arrayParams[i]).SaveToStream(writer, tab_depth + 1);
            }

            if (blockCommand != null)
                blockCommand.SaveToStream(writer, tab_depth + 1);

            if (blockCommand2 != null)
                ((EditCommandBlock)blockCommand2).SaveToStreamUserCommand(writer, EnumBlockType.CommandBlock2, tab_depth + 1);

            parent_writer.WriteBlock(EnumBlockType.MethodBlock, writer);
        }

        public bool IsExistVariableOnSplit(string varname)
        {
            for (int i = 0; i < arrayParams.Count; i++)
            {
                if (arrayParams[i].pVar.sVarName == varname)
                {
                    return true;
                }
            }

            if (bPropertySet)
            {
                if (varname == "value")
                {
                    return true;    // Set Property가 존재할 때는 value를 사용할 수 있다.
                }
            }

            return ((EditScriptLibClass)parentClass).IsExistVariableOnSplit(varname);
        }

        public bool IsExistVariableOnCompile(string varname, out Variable var, out object finded_pos)
        {
            for (int i = 0; i < arrayParams.Count; i++)
            {
                if (arrayParams[i].pVar.sVarName == varname)
                {
                    finded_pos = this;
                    var = arrayParams[i].pVar;
                    return true;
                }
            }

            if (bPropertySet)
            {
                if (varname == "value")
                {
                    var = null;
                    finded_pos = this;
                    return true;    // Set Property가 존재할 때는 value를 사용할 수 있다.
                }
            }

            return ((EditScriptLibClass)parentClass).IsExistVariableOnCompile(varname, out var, out finded_pos);
        }

        /*
        public bool IsExistVariableWithError(string varname, out Variable var, int col_pos, int row_pos)
        {
            for (int i = 0; i < arrayParams.Count; i++)
            {
                if (arrayParams[i].var.sVarName == varname)
                {
                    var = arrayParams[i].var;
                    return true;
                }
            }

            if (bPropertySet)
            {
                if (varname == "value")
                {
                    var = null;
                    return true;    // Set Property가 존재할 때는 value를 사용할 수 있다.
                }
            }

            return ((EditScriptLibClass)parentClass).IsExistVariableWithError(varname, out var, col_pos, row_pos);
        }*/
    }
}
