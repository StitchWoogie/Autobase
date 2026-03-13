using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;

namespace ScriptLibEdit
{
    public class EditVariable : Variable
    {
        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            if (nDimensionals != null)
            {
                writer.WriteArray(EnumBlockType.VarDimension, nDimensionals);
            }

            writer.WriteName(sVarName);
            writer.WriteString(EnumBlockType.sVarType, sVarType);
            writer.WriteByte(EnumBlockType.eVarType, (byte)eVarType);
            writer.WriteNameSplit(sCompiledNamespace, sCompiledClass);

            if (bConst)
                writer.WriteConst(bConst);
            if (bReadonly)
                writer.WriteReadonly(bReadonly);

            parent_writer.WriteBlock(EnumBlockType.Variable, writer);
        }

        public bool SplitArray(ScriptLibMain main, EditScriptLibFile file, string array_word, int col_pos, int row_pos)
        {
            if (array_word == null) return true;

            // 앞의 문장에서 []의 갯수는 정확히 맞아서 들어온다. 갯수를 계산할 필요는 없다.
            int array_count = 0;
            int dimensional = 1;

            for (int i = 0; i < array_word.Length; i++)
            {
                if (array_word[i] == '[')
                {
                    array_count++;
                }
            }

            nDimensionals = new int[array_count];
            int array_pos = 0;

            for (int i = 0; i < array_word.Length; i++)
            {
                if (array_word[i] == '[')
                {
                    dimensional = 1;
                }
                else if (array_word[i] == ',')
                {
                    dimensional++;
                }
                else if (array_word[i] == ']')
                {
                    nDimensionals[array_pos] = dimensional;
                    array_pos++;
                }
                else if (array_word[i] == ' ')  // 빈칸은 허용한다.
                {

                }
                else
                {
                    main.SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "배열 선언 시 '{0}' 문자는 허용하지 않습니다.", array_word[i]);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// C#언어에서 가장 기본이 되는 데이터 Type 형태이다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EnumVarType ConvertBasicVarStringToType(string type)
        {
            if (type == "object")
                return EnumVarType.TypeObject;
            else if (type == "bool")
                return EnumVarType.TypeBool;
            else if (type == "sbyte")
                return EnumVarType.TypeSbyte;
            else if (type == "byte")
                return EnumVarType.TypeByte;
            else if (type == "char")
                return EnumVarType.TypeChar;
            else if (type == "short")
                return EnumVarType.TypeShort;
            else if (type == "ushort")
                return EnumVarType.TypeUShort;
            else if (type == "int")
                return EnumVarType.TypeInt;
            else if (type == "uint")
                return EnumVarType.TypeUint;
            else if (type == "long")
                return EnumVarType.TypeLong;
            else if (type == "ulong")
                return EnumVarType.TypeULong;
            else if (type == "float")
                return EnumVarType.TypeFloat;
            else if (type == "double")
                return EnumVarType.TypeDouble;
            else if (type == "string")
                return EnumVarType.TypeString;
            else
                return EnumVarType.TypeNone;
        }

        public void Compile(ScriptLibMain main, EditScriptLibClass parent_class, int nColumn, int nRow)
        {
            ScriptLibTools.SplitNamespaceClass(sVarType, out sCompiledNamespace, out sCompiledClass);

            /*
            if (sCompiledClass == "string")
            {
                int kkk = 10;
            }*/

            // 항목이 하나만 있는 경우 a.b 형식이 아니고 b 같은 형식이다.
            if (sCompiledNamespace == null)
            {
                eVarType = ConvertBasicVarStringToType(sVarType);

                if (eVarType == EnumVarType.TypeString)
                {
                    // 별칭을 실제 이름으로 바꿔준다.
                    sCompiledNamespace = "System";
                    sCompiledClass = "String";
                }
                
                if (eVarType != EnumVarType.TypeNone)
                    return;
            }

            ScriptLibClass slc;

            EditScriptLibNamespace esln = (EditScriptLibNamespace)parent_class.parentNamespace;

            if (!((EditScriptLibMain)main).SeekClassWithError(esln, parent_class.sourceFile, ref sCompiledNamespace, sCompiledClass, out slc, nColumn, nRow))
            {
                return;
            }

            eVarType = EnumVarType.TypeClass;
        }
    }
}
