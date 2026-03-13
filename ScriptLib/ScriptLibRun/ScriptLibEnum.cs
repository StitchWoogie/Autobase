using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace ScriptLibRun
{
    public class ScriptLibEnum : ScriptLibMemberPublic
    {
        public bool bPublic = false;    // class는 public 이거나 internal 둘 중에 하나만 가능하다.
        public string sSourceFilename;  // 소스 파일명
        public string sNameEnum;        // Enum
        public EnumVarType eVarType = EnumVarType.TypeInt; // sbyte/byte/short/ushort/int/uint/long/ulong 8개의 type을 가질 수 있다. 기본값은 int    

        protected List<ScriptLibEnumItem> arrayEnumItem = new List<ScriptLibEnumItem>();
        public ScriptLibNamespace parentNamespace;

        public ScriptLibEnum(ScriptLibNamespace parent_namespace)
        {
            eMember = EnumScriptLibMember.Enum;
            parentNamespace = parent_namespace;
        }

        public override string GetName()
        {
            return sNameEnum;
        }

        public ScriptLibEnumItem GetItemPointer(string name)
        {
            for (int i = 0; i < arrayEnumItem.Count; i++)
            {
                if (name == arrayEnumItem[i].sName)
                    return arrayEnumItem[i];
            }

            return null;
        }
        
        public void Load(byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (block.type == EnumBlockType.Name)
                {
                    sNameEnum = block.ReadString();
                }
                else if (block.type == EnumBlockType.eVarType)
                {
                    eVarType = (EnumVarType)block.ReadByte();
                }
                else if (block.type == EnumBlockType.SourceFilename)
                {
                    sSourceFilename = block.ReadString();
                }    
                else if (block.type == EnumBlockType.EnumMember)
                {
                    ScriptLibEnumItem lei = new ScriptLibEnumItem();
                    lei.Load(block.block_data, ref line);
                    arrayEnumItem.Add(lei);
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                    //return;
                }
            }
        }

        public override string MakeDecompiledFile(int depth)
        {
            StringBuilder sb = new StringBuilder();

            ScriptLibClass.AppendWithTab(sb, depth, "");
            sb.Append("enum ");
            sb.Append(sNameEnum);
            if (eVarType != EnumVarType.TypeInt)
            {
                sb.Append(" : ");
                sb.Append(Variable.ConvertBasicVarTypeToString(eVarType));
            }
            sb.AppendLine();
            ScriptLibClass.AppendLineWithTab(sb, depth, "{{");
            for (int i = 0; i < arrayEnumItem.Count; i++)
            {
                ScriptLibClass.AppendLineWithTab(sb, depth+1, "{0},", arrayEnumItem[i].MakeDecompiledFile(depth));
            }
            ScriptLibClass.AppendWithTab(sb, depth, "}}");
            
            return sb.ToString();
        }

        
    }
}
