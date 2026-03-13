using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;

namespace ScriptLibEdit
{
    public class EditScriptLibEnum : ScriptLibEnum
    {
        public EditScriptLibFile sourceFile = null;
        
        public EditScriptLibEnum(EditScriptLibNamespace parent, EditScriptLibFile file)
            : base(parent)
        {
            sourceFile = file;
        }

        bool AddOneItem(EditScriptLibMain main, EditScriptLibFile file, string name, int val, int col_pos, int row_pos)
        {
            for (int i = 0; i < arrayEnumItem.Count; i++)
            {
                if (arrayEnumItem[i].sName == name)
                {
                    main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Same enum item '{0}' is already exists in '{1}'", name, this.sNameEnum);
                    return false;
                }
            }

            ScriptLibEnumItem lei = new ScriptLibEnumItem();
            lei.sName = name;
            lei.value = val;
            arrayEnumItem.Add(lei);

            return true;
        }

        bool SplitOneItem(EditScriptLibMain main, EditScriptLibFile file, string body, int col_pos, int row_pos, ref int enum_default_value)
        {
            body = ScriptLibTools.TrimWithCursorPos(body, ref col_pos, ref row_pos);

            char ch;
            int word_start_pos = -1;
            int word_start_col = 0;
            int word_start_row = 0;

            for (int i = 0; i < body.Length; i++)
            {
                ch = body[i];

                if (ch == '=')
                {
                    if (word_start_pos != -1)
                    {
                        string word = body.Substring(word_start_pos, i - word_start_pos).Trim(); // 오른쪽만 트림하면 된다.

                        int val = ConvertTool.ToInt32(body.Substring(i + 1));

                        if (!AddOneItem(main, file, word, val, word_start_col, word_start_row)) return false;

                        enum_default_value = val+1;

                        return true;
                    }
                }
                else
                {
                    if (word_start_pos == -1)
                    {
                        word_start_pos = i;
                        word_start_col = col_pos;
                        word_start_row = row_pos;
                    }
                }
            }

            if (word_start_pos != -1)
            {
                string word = body.Substring(word_start_pos);

                int val = enum_default_value;

                if (!AddOneItem(main, file, word, val, word_start_col, word_start_row)) return false;

                enum_default_value = val + 1;

                return true;
            }

            return true;
        }

        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos)
        {
            char ch;
            int word_start_pos = -1;
            int word_start_col=0;
            int word_start_row=0;
            int enum_default_value = 0; // = 을 지정하지 않는 경우에 사용 

            for (int i = 0; i < sBody.Length; i++)
            {
                ch = sBody[i];

                if (ch == ',')
                {
                    if (word_start_pos != -1)
                    {
                        string word = sBody.Substring(word_start_pos, i - word_start_pos);
                        if (!SplitOneItem(main, file, word, word_start_col, word_start_row, ref enum_default_value)) return false;
                        word_start_pos = -1;
                    }
                }
                else
                {
                    if (word_start_pos == -1)
                    {
                        word_start_pos = i;
                        word_start_col = col_pos;
                        word_start_row = row_pos;
                    }
                }
            }

            if (word_start_pos != -1)
            {
                string word = sBody.Substring(word_start_pos);
                if (!SplitOneItem(main, file, word, word_start_col, word_start_row, ref enum_default_value)) return false;
            }

            return true;
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();
            writer.WriteName(this.sNameEnum);

            // int 기본형이 아닐때만 저장한다.
            if(eVarType != EnumVarType.TypeInt)
                writer.WriteByte(EnumBlockType.eVarType, (byte)eVarType);

            writer.WriteSourceFilename(this.sourceFile.sSourceFilename);
            
            for (int i = 0; i < arrayEnumItem.Count; i++)
            {
                ScriptWriter wenum = new ScriptWriter();
                wenum.WriteName(arrayEnumItem[i].sName);
                wenum.WriteInt(EnumBlockType.EnumValue, arrayEnumItem[i].value);

                writer.WriteBlock(EnumBlockType.EnumMember, wenum);
            }

            parent_writer.WriteBlock(EnumBlockType.EnumBlock, writer);
        }
        
    }
}
