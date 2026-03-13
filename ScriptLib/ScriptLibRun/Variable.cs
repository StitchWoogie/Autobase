using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace ScriptLibRun
{
    public enum EnumVarType : byte
    {
        TypeNone=0,   // 생성자는 형태가 없다. public Class() 같은 경우
        TypeVoid=1,   // 함수의 선언에서 void 형태
        TypeClass=2,
        TypeObject=3,

        TypeBool=11,

        TypeSbyte=12,
        TypeByte=13,

        TypeChar=14,

        TypeShort=15,
        TypeUShort=16,

        TypeInt=17,
        TypeUint=18,

        TypeLong=19,
        TypeULong=20,

        TypeFloat=21,
        TypeDouble=22,

        TypeString=23,

        TypeNull = 24,  // null 값
    }

    [Serializable]
    public class Variable
    {
        public EnumVarType eVarType;
        public string sVarType;
        public string sVarName;

        // const와 readonly는 실제로는 컴파일 시에만 사용해도 문제는 없다. decompile할때 정확하게 하기 위해서 runtime에도 들어있지만 경우에 따라서는 없어도 된다. 
        public bool bConst=false;             // Method안에 선언되는 변수도 const를 가질 수 있다.
        public bool bReadonly = false;        // readonly는 클래스 변수만 속성을 가질수 있다.  
        
        public string sCompiledNamespace;
        public string sCompiledClass;

        public int[] nDimensionals = null;      // [][][] = 1,1,1 이고 [,][][,,] = 2,1,3 이 된다. 숫자는 각 배열의 차원수를 말한다.
                                                // null이면 배열이 아니다.

        public void CopyVar(Variable v)
        {
            sVarType = v.sVarType;
            sVarName = v.sVarName;
            sCompiledNamespace = v.sCompiledNamespace;
            sCompiledClass = v.sCompiledClass;
            eVarType = v.eVarType;
            if (v.nDimensionals != null)
            {
                nDimensionals = new int[v.nDimensionals.Length];
                for (int i = 0; i < v.nDimensionals.Length; i++)
                {
                    nDimensionals[i] = v.nDimensionals[i];
                }
            }
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
                    sVarName = block.ReadString();
                }
                else if (block.type == EnumBlockType.sVarType)
                {
                    sVarType = block.ReadString();
                }
                else if (block.type == EnumBlockType.eVarType)
                {
                    eVarType = (EnumVarType)block.ReadByte();
                }
                else if (block.type == EnumBlockType.NameSplitNamespace)
                {
                    sCompiledNamespace = block.ReadString();
                }
                else if (block.type == EnumBlockType.NameSplitClass)
                {
                    sCompiledClass = block.ReadString();
                }
                else if (block.type == EnumBlockType.VarDimension)
                {
                    nDimensionals = block.ReadArray();
                }
                else if (block.type == EnumBlockType.bConst)
                {
                    bConst = block.ReadBool();
                }
                else if (block.type == EnumBlockType.bReadonly)
                {
                    bReadonly = block.ReadBool();
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                }
            }
        }

        public string MakeDecompiledFile(int depth)
        {
            StringBuilder sb = new StringBuilder();

            if (bConst)
                sb.Append("const ");

            if (bReadonly)
                sb.Append("readonly ");

            sb.Append(sVarType);
            if (nDimensionals != null)
            {
                for (int i = 0; i < nDimensionals.Length; i++)
                {
                    sb.Append("[");
                    for (int j = 0; j < nDimensionals[i]-1; j++)
                    {
                        sb.Append(',');
                    }
                    sb.Append("]");
                }
            }
            sb.Append(" ");
            sb.Append(sVarName);
            
            return sb.ToString();
        }

        /// <summary>
        /// C#언어에서 가장 기본이 되는 데이터 Type 형태이다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ConvertBasicVarTypeToString(EnumVarType type)
        {
            if (type == EnumVarType.TypeObject)
                return "object";
            else if (type == EnumVarType.TypeBool)
                return "bool";
            else if (type == EnumVarType.TypeSbyte)
                return "sbyte";
            else if (type == EnumVarType.TypeByte)
                return "byte";
            else if (type == EnumVarType.TypeChar)
                return "char";
            else if (type == EnumVarType.TypeShort)
                return "short";
            else if (type == EnumVarType.TypeUShort)
                return "ushort";
            else if (type == EnumVarType.TypeInt)
                return "int";
            else if (type == EnumVarType.TypeUint)
                return "uint";
            else if (type == EnumVarType.TypeLong)
                return "long";
            else if (type == EnumVarType.TypeULong)
                return "ulong";
            else if (type == EnumVarType.TypeFloat)
                return "float";
            else if (type == EnumVarType.TypeDouble)
                return "double";
            else if (type == EnumVarType.TypeString)
                return "string";
            else
                return "";
        }
    }
}
