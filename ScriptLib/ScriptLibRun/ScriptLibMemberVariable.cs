using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class ScriptLibMemberVariable : ScriptLibMemberPublic
    {
        public EnumAccessLevel eAccessLevel = EnumAccessLevel.access_private;
        public bool bStatic = false;    // 클래스 변수나 메소드에 사용할 수 있다. Method 내부에서는 사용 불가하다.
        //public bool bConst = false;     // const는 자체가 static이므로 static 과 함께 사용될 수 없고 항상 초기화를 가지고 Compile시 값이 결정된다.
                                        // 컴파일시에 DEFINE되는 구조이므로 숫자나 문자열 변수만 const가 가능하다. const는 Method안에서도 가능하다.
        //public bool bReadonly = false;  // 초기화하지 않아도 되고 생성자에서는 초기화가 가능하다.  Method 내부에서는 사용 불가하다.

        public Variable pVar = new Variable();
        public RecursiveValue pValue = null;

        object valueOnStatic;        // 이것은 변수가 static/const 일 때만 사용한다. 데이타 스택에 보관되지 않고 자체에 값을 할당하여 전역으로 사용할 수 있도록 한다.
        bool bStaticInited = false;  // 최초 한번 Init을 시켜준다.

        public ScriptLibMemberVariable()
        {
            eMember = EnumScriptLibMember.Variable;
        }

        public override string GetName()
        {
            return pVar.sVarName;
        }

        public override async Task<(bool, object)> GetStaticValueAsync(ScriptRunConfiguration src)
        {
            object retnvalue;

            if (!bStaticInited)
            {
                // static 변수는 최초 호출될 때 초기화 한다.
                if (pValue != null)
                {
                    var (success, value) = await pValue.RunAsync(src, null, null);
                    if (!success) return (false, value);
                    valueOnStatic = value;
                }
                bStaticInited = true;
            }
            retnvalue = valueOnStatic;

            return (true, retnvalue);
        }

        public override bool SetStaticValue(ScriptRunConfiguration src, object value)
        {
            if (!bStaticInited)
            {
                bStaticInited = true;
            }

            valueOnStatic = value;

            return true;
        }

        public void Load(byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (block.type == EnumBlockType.Variable)
                {
                    pVar.Load(block.block_data, ref line);
                }
                else if (block.type == EnumBlockType.RecursiveValue)
                {
                    pValue = new RecursiveValue(null, null);
                    pValue.Load(block.block_data, ref line);
                }
                else if (block.type == EnumBlockType.eAccessLevel)
                {
                    eAccessLevel = (EnumAccessLevel)block.ReadByte();
                }
                /*
                else if (block.type == EnumBlockType.bConst)
                {
                    bConst = block.ReadBool();
                }*/
                else if (block.type == EnumBlockType.bStatic)
                {
                    bStatic = block.ReadBool();
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                }
            }
        }

        public override string MakeDecompiledFile(int depth)
        {
            StringBuilder sb = new StringBuilder();

            ScriptLibClass.AppendWithTab(sb, depth, "");

            if (eAccessLevel == EnumAccessLevel.access_public)
            {
                sb.Append("public ");
            }
            else if (eAccessLevel == EnumAccessLevel.access_internal)
            {
                sb.Append("internal ");
            }
            else if (eAccessLevel == EnumAccessLevel.access_protected)
            {
                sb.Append("protected ");
            }
            else
            {
                // private 는 선언하지 않아도 기본값이다.
            }

            /*
            if (bConst)
            {
                sb.Append("const ");
            }*/

            if (bStatic)
            {
                sb.Append("static ");
            }

            sb.Append(pVar.MakeDecompiledFile(depth));
            if (pValue != null)
            {
                sb.Append(" = ");
                sb.Append(pValue.MakeDecompiledFile(depth));
            }
            sb.Append(";");

            /*
            if (eAccessLevel == EnumAccessLevel.access_public)
                ScriptLibClass.AppendWithTab(sb, depth, "public ");
            else
                ScriptLibClass.AppendWithTab(sb, depth, "");

            if (bStatic)
                sb.Append("static ");

            // return 타입이 없거나 생성자가 아닐 때
            if (sReturnDataType != null && sNameMethod != parentClass.sNameClass)
            {
                sb.Append(sReturnDataType + " ");
            }

            sb.Append(sNameMethod);

            if (!bProperty)
            {
                sb.Append('(');
                sb.Append(')');
            }

            sb.AppendLine();

            ScriptLibClass.AppendLineWithTab(sb, depth, "{{");

            if (bProperty)
            {
                if (bPropertyGet)
                {
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "get");
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "{{");
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "}}");
                }
                if (bPropertySet)
                {
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "set");
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "{{");
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "}}");
                }
            }
            //for (int i = 0; i < arrayMethod.Count; i++)
            //{
            //    sb.AppendLine(arrayMethod[i].MakeDecompiledFile(depth + 1));
            //}

            ScriptLibClass.AppendWithTab(sb, depth, "}}");*/

            return sb.ToString();
        }
    }
}
