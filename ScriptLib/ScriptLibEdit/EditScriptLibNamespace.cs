using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;

namespace ScriptLibEdit
{
    public class EditScriptLibNamespace : ScriptLibNamespace
    {
        public EditScriptLibNamespace(ScriptLibMain parent, ScriptLibLibrary parent_library)
            : base(parent_library)
        {

        }

        EditScriptLibClass AddClassPointer(ScriptLibMain main, EditScriptLibFile file, string name, bool bPartial, int col_pos, int row_pos, bool bStruct, List<ScriptBaseClass> base_class_names)
        {
            EditScriptLibClass slc;

            for (int i = 0; i < arrayMember.Count; i++)
            {
                if (arrayMember[i].GetName() == name)
                {
                    if (arrayMember[i].eMember == EnumScriptLibMember.Class)
                    {
                        slc = (EditScriptLibClass)arrayMember[i];
                        if (slc.bPartial && bPartial)
                            return (EditScriptLibClass)arrayMember[i];

                        main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The namespace '{0}' already contains a class '{1}'", sNameNamespace, name);
                    }
                    else
                    {
                        main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The namespace '{0}' already contains a enum '{1}'", sNameNamespace, name);
                    }

                    return null;
                }
            }

            slc = new EditScriptLibClass(this, file);
            slc.sNameClass = name;
            slc.bPartial = bPartial;
            slc.bStruct = bStruct;
            slc.SetBaseClass(base_class_names);

            arrayMember.Add(slc);

            return slc;
        }

        EditScriptLibEnum AddEnumPointer(ScriptLibMain main, EditScriptLibFile file, string name, int col_pos, int row_pos, List<ScriptBaseClass> base_class_names)
        {
            for (int i = 0; i < arrayMember.Count; i++)
            {
                if (arrayMember[i].GetName() == name)
                {
                    if(arrayMember[i].eMember == EnumScriptLibMember.Class)
                        main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The namespace '{0}' already contains a class '{1}'", sNameNamespace, name);
                    else
                        main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The namespace '{0}' already contains a enum '{1}'", sNameNamespace, name);

                    return null;
                }
            }

            EnumVarType evt = EnumVarType.TypeInt;

            if (base_class_names.Count > 1)
            {
                main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The enum can have only one type.");
            }
            else if (base_class_names.Count == 1)
            {
                evt = EditVariable.ConvertBasicVarStringToType(base_class_names[0].name);

                switch (evt)
                {
                    case EnumVarType.TypeSbyte:
                    case EnumVarType.TypeByte:
                    case EnumVarType.TypeShort:
                    case EnumVarType.TypeUShort:
                    case EnumVarType.TypeInt:
                    case EnumVarType.TypeUint:
                    case EnumVarType.TypeLong:
                    case EnumVarType.TypeULong:
                        break;
                    default:
                        main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The enum type sbyte, byte, short, ushort, int, uint, long, or ulong expected.");
                        break;
                }
            }

            EditScriptLibEnum esle = new EditScriptLibEnum(this, file);
            esle.sNameEnum = name;
            esle.eVarType = evt;

            arrayMember.Add(esle);

            return esle;
        }

        public override bool Split(ScriptLibMain main, object file, string sBody, int col_pos, int row_pos)
        {
            int start_col_pos = 0;
            int start_row_pos = 0;

            int block_start_pos;
            int block_end_pos;

            int start_pos = 0;

            while (true)
            {
                if (!((EditScriptLibMain)main).GetOneBlock((EditScriptLibFile)file, sBody, ref start_pos, sBody.Length - 1, out start_col_pos, out start_row_pos, ref col_pos, ref row_pos, out block_start_pos, out block_end_pos))
                {
                    return false;
                }

                if (block_start_pos == block_end_pos)   // 더 이상 해석할 문장이 없다.
                {
                    break;
                }

                string one_block_data = sBody.Substring(block_start_pos, block_end_pos - block_start_pos + 1);

                if (!SplitOneBlock((EditScriptLibMain)main, (EditScriptLibFile)file, one_block_data, start_col_pos, start_row_pos)) return false;
            }

            return true; 
        }

        bool CheckBlockUsing(EditScriptLibMain main, EditScriptLibFile file, string sBody, int i, int col_pos, int row_pos)
        {
            if (sBody[sBody.Length - 1] != ';')
            {
                main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "using 문은 ; 로 끝나야 합니다.");
                return false;
            }

            string data = sBody.Substring(i + 1, sBody.Length - i - 2);
            col_pos++;

            data = ScriptLibTools.TrimWithCursorPos(data, ref col_pos, ref row_pos);

            file.AddUsing(main, data, col_pos, row_pos);

            return true;
        }

        bool CheckBlockNamespace(EditScriptLibMain main, EditScriptLibFile file, string sBody, int i, int col_pos, int row_pos)
        {
            if (sBody[sBody.Length - 1] != '}')
            {
                main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Namespace 문은 } 로 끝나야 합니다.");
                return false;
            }

            string word;
            List<ScriptBaseClass> base_class_names;

            if (!main.GetWordToLeftCurlyBracket(file, sBody, out word, ref i, ref col_pos, ref row_pos, out base_class_names)) return false;

            ScriptLibNamespace esln = ((EditScriptLibLibrary)parentLibrary).AddNamespaceClass(main, word);

            int save_col_pos = col_pos;
            int save_row_pos = row_pos;
            int start_pos = i + 1;
            int end_pos = sBody.Length - 2;

            string one_block_data = sBody.Substring(start_pos, end_pos - start_pos + 1);

            if (!esln.Split(main, file, one_block_data, save_col_pos, save_row_pos)) return false;

            return true;
        }

        bool CheckBlockClass(EditScriptLibMain main, EditScriptLibFile file, EditScriptLibNamespace namespaceclass, string sBody, int i, int col_pos, int row_pos, EnumAccessLevel accesslevel, bool bPartial, bool bStruct)
        {
            if (sBody[sBody.Length - 1] != '}')
            {
                main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "class,struct 문은 } 로 끝나야 합니다.");
                return false;
            }

            string name;
            List<ScriptBaseClass> base_class_names;

            if (!main.GetWordToLeftCurlyBracket(file, sBody, out name, ref i, ref col_pos, ref row_pos, out base_class_names)) return false;

            int save_col_pos = col_pos;
            int save_row_pos = row_pos;
            int start_pos = i + 1;
            int end_pos = sBody.Length - 2;

            string one_block_data = sBody.Substring(start_pos, end_pos - start_pos + 1);

            EditScriptLibClass eslc = namespaceclass.AddClassPointer(main, file, name, bPartial, col_pos, row_pos, bStruct, base_class_names);

            if (eslc == null) return false;

            if (!eslc.Split(main, file, one_block_data, save_col_pos, save_row_pos)) return false;

            return true;
        }

        bool CheckBlockEnum(EditScriptLibMain main, EditScriptLibFile file, EditScriptLibNamespace namespaceclass, string sBody, int i, int col_pos, int row_pos, EnumAccessLevel accesslevel)
        {
            if (sBody[sBody.Length - 1] != '}')
            {
                main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "enum 문은 } 로 끝나야 합니다.");
                return false;
            }

            string word;
            List<ScriptBaseClass> base_class_names;

            if (!main.GetWordToLeftCurlyBracket(file, sBody, out word, ref i, ref col_pos, ref row_pos, out base_class_names)) return false;

            int save_col_pos = col_pos;
            int save_row_pos = row_pos;
            int start_pos = i + 1;
            int end_pos = sBody.Length - 2;

            string one_block_data = sBody.Substring(start_pos, end_pos - start_pos + 1);

            EditScriptLibEnum esle = namespaceclass.AddEnumPointer(main, file, word, col_pos, row_pos, base_class_names);

            if (esle == null) return false;

            if (!esle.Split(main, file, one_block_data, save_col_pos, save_row_pos)) return false;

            return true;
        }

        // 소스파일 하나에는 Block 종류가 using/namespace/class/enum 이 올 수 있다. class와 enum 은 public 의 access관리자가 올 수 있다.

        bool SplitOneBlock(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos)
        {
            int command_start_pos = 0;
            bool command_start = false;
            char ch;

            bool access_start = false;
            EnumAccessLevel eAccess = EnumAccessLevel.access_internal;
            bool bPartial = false;

            for (int i = 0; i < sBody.Length; i++)
            {
                ch = sBody[i];

                if (command_start) // Command문이 시작되었다.
                {
                    if (ch == ' ' || ch == '\t' || ch == '\n')
                    {
                        if (String.Compare(sBody, command_start_pos, "using", 0, 5) == 0)   // using 문
                        {
                            if (!CheckBlockUsing(main, file, sBody, i, col_pos, row_pos)) return false;

                            return true;
                        }
                        else if (String.Compare(sBody, command_start_pos, "namespace", 0, 9) == 0)   // namespace
                        {
                            if (!CheckBlockNamespace(main, file, sBody, i, col_pos, row_pos)) return false;

                            return true;
                        }
                        else if (String.Compare(sBody, command_start_pos, "public", 0, 6) == 0)   // public 문
                        {
                            if (access_start)
                            {
                                main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "access level(public/internal) 이 이미 선언 되었습니다.");
                                return false;
                            }
                            command_start = false;
                            eAccess = EnumAccessLevel.access_public;
                            access_start = true;
                        }
                        else if (String.Compare(sBody, command_start_pos, "internal", 0, 8) == 0)   // 
                        {
                            if (access_start)
                            {
                                main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "access level(public/internal) 이 이미 선언 되었습니다.");
                                return false;
                            }
                            command_start = false;
                            eAccess = EnumAccessLevel.access_internal;
                            access_start = true;
                        }
                        else if (String.Compare(sBody, command_start_pos, "protected", 0, 9) == 0 ||
                            String.Compare(sBody, command_start_pos, "private", 0, 7) == 0)   // 
                        {
                            main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Elements defined in a namespace cannot be explicitly declared as private, protected, or protected internal");
                            return false;
                        }
                        else if (String.Compare(sBody, command_start_pos, "partial", 0, 7) == 0)   // 
                        {
                            if (bPartial)
                            {
                                main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "partial 은 이미 선언 되었습니다.");
                                return false;
                            }
                            command_start = false;
                            bPartial = true;
                        }
                        else if (String.Compare(sBody, command_start_pos, "class", 0, 5) == 0)   // class 문
                        {
                            if (!CheckBlockClass(main, file, this, sBody, i, col_pos, row_pos, eAccess, bPartial, false)) return false;

                            return true;
                        }
                        else if (String.Compare(sBody, command_start_pos, "struct", 0, 6) == 0)   // struct
                        {
                            if (!CheckBlockClass(main, file, this, sBody, i, col_pos, row_pos, eAccess, bPartial, true)) return false;

                            return true;
                        }
                        else if (String.Compare(sBody, command_start_pos, "enum", 0, 4) == 0)   // enum 
                        {
                            if (!CheckBlockEnum(main, file, this, sBody, i, col_pos, row_pos, eAccess)) return false;
                            
                            return true;
                        }
                        else
                        {
                            string name = sBody.Substring(command_start_pos, i - command_start_pos);
                            main.SetError(parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "알 수 없는 단어입니다. ({0})", name);
                            return false;
                        }
                    }
                }
                else
                {
                    if (ch == ' ')
                    {
                    }
                    else if (ch == '\t')
                    {
                    }
                    else if (ch == '\n')
                    {
                    }
                    else if (ch == '/')
                    {
                        //if (!main.SkipDescription(sBody, ref i, ref col_pos, ref row_pos)) return false;
                        bool? retn_description = main.CheckDescription(file, sBody, ref i, ref col_pos, ref row_pos);
                        if (retn_description == null) return false;  // 오류가 발생했다.
                        if (retn_description == true) continue;
                    }
                    else
                    {
                        command_start = true;
                        command_start_pos = i;
                    }
                }

                main.CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            return true;
        }

        // Compile전에 준비해야 하는 것들
        public void PrepareBeforeCompile()
        {
            for (int i = 0; i < arrayMember.Count; i++)
            {
                if (arrayMember[i].eMember == EnumScriptLibMember.Class)
                {
                    ((EditScriptLibClass)arrayMember[i]).PrepareBeforeCompile();
                }
            }
        }

        public override void Compile(ScriptLibMain main)
        {
            for (int i = 0; i < arrayMember.Count; i++)
            {
                if (arrayMember[i].eMember == EnumScriptLibMember.Class)
                {
                    ((EditScriptLibClass)arrayMember[i]).Compile(main);
                }
            }
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            writer.WriteName(this.sNameNamespace);
            for (int i = 0; i < arrayMember.Count; i++)
            {
                if (arrayMember[i].eMember == EnumScriptLibMember.Class)
                    ((EditScriptLibClass)arrayMember[i]).SaveToStream(writer, tab_depth + 1, false);
                else 
                    ((EditScriptLibEnum)arrayMember[i]).SaveToStream(writer, tab_depth + 1);
            }
            parent_writer.WriteBlock(EnumBlockType.NamespaceBlock, writer);
        }
    }
}
