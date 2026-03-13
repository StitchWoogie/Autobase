using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class ScriptLibMemberMethod : ScriptLibMemberPublic
    {
        public string sNameMethod;

        public bool bProperty = false;      // Property 형태의 함수이다.
        public bool bPropertyGet = false;   // Property의 Get함수 지원
        public bool bPropertySet = false;   // Property의 Set함수 지원

        public EnumAccessLevel eAccessLevel;// Access
        public bool bStatic;                // static의 여부
        public string sReturnDataType;      // 리턴의 형태

        public List<MethodArgument> arrayParams = new List<MethodArgument>();

        public ScriptLibClass parentClass;

        public CommandBlock blockCommand = null;     // Property 형태일때는 Get으로 사용
        public CommandBlock blockCommand2 = null;    // Property 형태일때는 Set으로 사용

        public delegate bool DeleMethod(ClassDataStack cds, out object retn, object[] args);
        public DeleMethod procMethodByProgram = null;           // Internal 라이브러리 형태의 Proc이다

        public delegate Task<(bool success, object retn)> DeleMethodAsync(ClassDataStack cds, object[] args); //20250723 PSU 추가
        public DeleMethodAsync procMethodByProgramAsync = null;

        public string sSourceFilename;  // 소스 파일명

        public int nColumn, nRow;

        public ScriptLibMemberMethod(ScriptLibClass parent)
        {
            parentClass = parent;
            eMember = EnumScriptLibMember.Method;
        }

        public override string GetName()
        {
            return sNameMethod;
        }

        public static void GetMethodArgumentMinMax(List<MethodArgument> args, out int min_count, out int max_count)
        {
            // 허용 가능한 인자의 갯수를 찾는다.
            min_count = args.Count;
            max_count = args.Count;

            for (int i = 0; i < args.Count; i++)
            {
                if (args[i].eInOut == EnumInOut.Params)
                {
                    min_count = i;
                    max_count = 10000;
                    break;
                }
            }
        }

        int nMinArgument = -1;
        int nMaxArgument = -1;

        public async Task<(bool success, object retnValue)> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, object[] args)
        {
            int count_arg_call = 0;
            if (args != null)
                count_arg_call = args.Length;

            if (nMinArgument == -1)
            {
                GetMethodArgumentMinMax(arrayParams, out nMinArgument, out nMaxArgument);
            }

            if (count_arg_call < nMinArgument || count_arg_call > nMaxArgument)
            {
                src.slmain.SetError(parentClass.parentNamespace.parentLibrary.sNameLibrary, sSourceFilename, 0, 0,
            EnumScriptErrorType.Else, "{0}.{1} 메소드 호출시 인자({2})와 선언된 인자의 개수가 다릅니다. {3}",
            parentClass.sNameClass, sNameMethod, count_arg_call, MakeStringMethodUsing());
                return (false, 0);
            }

            object retnvalue;

            // 내부 라이브러리 형태의 Proc이다
            if (procMethodByProgramAsync != null)
            {
                var (success, returnValue) = await procMethodByProgramAsync(class_stack, args).ConfigureAwait(false);
                if (!success)
                    return (false, 0);
                retnvalue = returnValue;
            }
            else
            {
                // Method가 시작할 때는 data_stack을 새로 준비한다.
                ClassDataStack data_stack = new ClassDataStack(null);

                if (bPropertyGet)
                {
                    if (blockCommand == null)
                    {
                        src.slmain.SetError(parentClass.parentNamespace.parentLibrary.sNameLibrary, sSourceFilename, 0, 0,
                     EnumScriptErrorType.Else, "{0}.{1} Property는 get 을 가지고 있지 않습니다.",
                     parentClass.sNameClass, sNameMethod);
                        return (false, 0);
                    }
                    else
                    {
                        var (success, retntype, returnValue) = await blockCommand.RunAsync(src, class_stack, data_stack, true).ConfigureAwait(false);
                        return (success, returnValue);
                    }
                }

                for (int i = 0; i < arrayParams.Count; i++)
                {
                    if (i >= args.Length) break;    // params인 경우 이 인자를 사용하지 않으면 하나 적은 경우도 있다.

                    data_stack.AddItem(arrayParams[i].pVar, args[i]);
                }

                var (cmdSuccess, cmdRetntype, cmdReturnValue) = await blockCommand.RunAsync(src, class_stack, data_stack, true);
                if (!cmdSuccess)
                    return (false, 0);
                retnvalue = cmdReturnValue;


                // ref 이거나 out 인자는 중간에 new로 재할당 되었을 수 있으므로 포인터를 다시 연결해 준다. 2016-5-11
                for (int i = 0; i < arrayParams.Count; i++)
                {
                    if (arrayParams[i].eInOut == EnumInOut.Out ||
                        arrayParams[i].eInOut == EnumInOut.Ref)
                        args[i] = data_stack.arrayData[i].value;
                }
            }

            return (true, retnvalue);
        }

        /// <summary>
        /// 메소드의 원형을 문자열로 보여준다. void Method(int a) 처럼
        /// </summary>
        /// <returns></returns>
        public string MakeStringMethodUsing()
        {
            string buf;
            MethodArgument ma;

            if(this.sReturnDataType == null)
                buf = String.Format("{0}(", this.sNameMethod);
            else
                buf = String.Format("{1} {0}(", this.sNameMethod, this.sReturnDataType);

            for (int i = 0; i < this.arrayParams.Count; i++)
            {
                ma = this.arrayParams[i];
                buf += String.Format("{0} {1}", ma.pVar.sVarType, ma.pVar.sVarName);
                if (i < arrayParams.Count - 1)
                    buf += ", ";
            }

            buf += ")";

            return buf;
        }

        public override void SetBreakPoints(string filename, List<int> array)
        {
            if (String.Compare(sSourceFilename, filename, true) != 0) return;

            if(blockCommand != null)
                blockCommand.SetBreakPoints(filename, array);

            if (blockCommand2 != null)
                blockCommand2.SetBreakPoints(filename, array);
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
                    sNameMethod = block.ReadString();
                }
                else if (block.type == EnumBlockType.SourceFilename)
                {
                    sSourceFilename = block.ReadString();
                }
                else if (block.type == EnumBlockType.eAccessLevel)
                {
                    eAccessLevel = (EnumAccessLevel)block.ReadByte();
                }
                else if (block.type == EnumBlockType.bStatic)
                {
                    bStatic = block.ReadBool();
                }
                else if (block.type == EnumBlockType.ReturnDataType)
                {
                    sReturnDataType = block.ReadString();
                }
                else if (block.type == EnumBlockType.Param)
                {
                    MethodArgument ma = new MethodArgument();
                    ma.Load(block.block_data, ref line);
                    arrayParams.Add(ma);
                }
                else if (block.type == EnumBlockType.Property)
                {
                    block.ReadProperty(out bProperty, out bPropertyGet, out bPropertySet);
                }
                else if (block.type == EnumBlockType.CommandBlock)
                {
                    blockCommand = new CommandBlock(this, null);
                    blockCommand.Load(block.block_data, null, ref line);
                }
                else if (block.type == EnumBlockType.CommandBlock2)
                {
                    blockCommand2 = new CommandBlock(this, null);
                    blockCommand2.Load(block.block_data, null, ref line);
                }
                else if (block.type == EnumBlockType.ClassVariable)
                {

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

            if(eAccessLevel == EnumAccessLevel.access_public)
                ScriptLibClass.AppendWithTab(sb, depth, "public ");
            else
                ScriptLibClass.AppendWithTab(sb, depth, "");

            if(bStatic)
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
                for (int i = 0; i < arrayParams.Count; i++)
                {
                    sb.Append(arrayParams[i].MakeDecompiledFile(depth));
                    if (i < arrayParams.Count - 1)  // 마지막이 아니면 ,로 구분한다.
                        sb.Append(", ");
                }
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
                    sb.Append(blockCommand.MakeDecompiledFile(depth + 2, true));    
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "}}");
                }
                if (bPropertySet)
                {
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "set");
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "{{");
                    sb.Append(blockCommand2.MakeDecompiledFile(depth + 2, true));    
                    ScriptLibClass.AppendLineWithTab(sb, depth + 1, "}}");
                }
            }
            else
            {
                sb.Append(blockCommand.MakeDecompiledFile(depth + 1, true));    
            }

            ScriptLibClass.AppendWithTab(sb, depth, "}}");

            return sb.ToString();
        }
    }
}
