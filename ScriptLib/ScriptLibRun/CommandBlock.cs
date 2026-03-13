using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using ScriptLibRun.Debugger;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public enum EnumReturnType
    {
        TypeNormal,     // 끝까지 실행해서 자연스럽게 끝났다.

        TypeReturn,     // return은 Method를 벗어난다.
        TypeContinue,   // loop를 계속한다.
        TypeGoto,       // Method안에 있는 goto 문으로 간다.
        TypeBreak,      // Loop를 벗어난다.
    }

    public class CommandBlock : CommandPublic
    {
        protected List<CommandPublic> arrayCommand = new List<CommandPublic>();

        public CommandBlock(ScriptLibMemberMethod parent, CommandPublic parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_block;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="class_stack"></param>
        /// <param name="parent_stack"></param>
        /// <param name="use_localstack">보통 Command블럭은 local_stack을 따로 사용하고 벗어나면 해제되지만 for loop의 초기화 같은 데에서는 상위 stack을 사용할 필요가 있다.</param>
        /// <param name="retntype"></param>
        /// <param name="retnvalue"></param>
        /// <returns></returns>
        public async Task<(bool success, EnumReturnType returntype, object returnValue)> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack, bool use_localstack)
        {
            EnumReturnType retntype;
            object retnvalue;

            retntype = EnumReturnType.TypeNormal;
            retnvalue = null;

            CommandPublic cp;

            ClassDataStack data_stack;

            if (use_localstack)
                data_stack = new ClassDataStack(parent_stack);
            else
                data_stack = parent_stack;

            bool bResultIf = false;

            EnumDebugStep retn_step;

            for (int i = 0; i < arrayCommand.Count; i++)
            {
                cp = arrayCommand[i];

                retn_step = cp.DebuggerGotoCursor(class_stack, data_stack, cp.nColumn, cp.nRow);

                if (retn_step == EnumDebugStep.StepInto)
                {
                    bBreakMustNext = true;
                }

                if (cp.eCommandType == EnumCommandType.type_declaration)
                {
                    CommandDeclaration cd = (CommandDeclaration)cp;

                    var declResult = await cd.RunAsync(src, class_stack, data_stack).ConfigureAwait(false);
                    if (!declResult) return (false, EnumReturnType.TypeNormal, null);
                }
                else if (cp.eCommandType == EnumCommandType.type_substitution)
                {
                    CommandSubstitution cs = (CommandSubstitution)cp;

                    var substResult = await cs.RunAsync(src, class_stack, data_stack).ConfigureAwait(false);
                    if (!substResult) return (false, EnumReturnType.TypeNormal, null);
                }
                else if (cp.eCommandType == EnumCommandType.type_if)
                {
                    CommandIf ci = (CommandIf)cp;

                    if (ci.bElseIf && bResultIf) continue;   // else if인 경우는 앞의 조건이 만족하면 실행하지 않는다.

                    var (condSuccess, condition) = await ci.CheckConditionAsync(src, class_stack, data_stack).ConfigureAwait(false);
                    if (!condSuccess)
                    {
                        return (false, EnumReturnType.TypeNormal, null);
                    }

                    if (condition)
                    {
                        var (blockSuccess, blockRetntype, blockRetnvalue) = await ci.blockCommand.RunAsync(src, class_stack, data_stack, true).ConfigureAwait(false);
                        if (!blockSuccess) return (false, EnumReturnType.TypeNormal, null);

                        retntype = blockRetntype;
                        retnvalue = blockRetnvalue;

                        if (retntype == EnumReturnType.TypeReturn) return (true, retntype, retnvalue);
                        if (retntype == EnumReturnType.TypeGoto)
                        {
                            if (!SeekLabelPosition((string)retnvalue, ref i))
                                return (true, retntype, retnvalue);
                        }
                        // 정상적인 진행이 아닌경우는 return한다. continue/break/return 문
                        else if (retntype != EnumReturnType.TypeNormal) return (true, retntype, retnvalue);
                    }

                    bResultIf = condition;
                }

                else if (cp.eCommandType == EnumCommandType.type_else)
                {
                    if (bResultIf)
                    {
                        bResultIf = false;
                        continue;    // else 인 경우는 앞의 조건이 만족하면 실행하지 않는다.
                    }

                    CommandElse ce = (CommandElse)cp;

                    var (blockSuccess, blockRetntype, blockRetnvalue) = await ce.blockCommand.RunAsync(src, class_stack, data_stack, true).ConfigureAwait(false);
                    if (!blockSuccess) return (false, EnumReturnType.TypeNormal, null);

                    //251113 PSU return 값 할당 누락으로 다시 추가.
                    retntype = blockRetntype;
                    retnvalue = blockRetnvalue;

                    if (retntype == EnumReturnType.TypeReturn)
                        return (true, retntype, retnvalue);
                    if (retntype == EnumReturnType.TypeGoto)
                    {
                        if (!SeekLabelPosition((string)retnvalue, ref i))
                            return (true, retntype, retnvalue);
                    }
                    // 정상적인 진행이 아닌경우는 return한다. continue/break/return 문
                    else if (retntype != EnumReturnType.TypeNormal)
                        return (true, retntype, retnvalue);
                }
                else if (cp.eCommandType == EnumCommandType.type_method)
                {
                    CommandMethod cm = (CommandMethod)cp;

                    var (methodSuccess, methodRetnvalue) = await cm.RunAsync(src, class_stack, data_stack, false).ConfigureAwait(false);
                    if (!methodSuccess) return (false, EnumReturnType.TypeNormal, null);
                    retnvalue = methodRetnvalue;
                }
                else if (cp.eCommandType == EnumCommandType.type_return)
                {
                    CommandReturn cr = (CommandReturn)cp;

                    // return 값이 있는 경우만 값을 받아온다.
                    if (cr.pValue != null)
                    {
                        var (valueSuccess, valueResult) = await cr.pValue.RunAsync(src, class_stack, data_stack).ConfigureAwait(false);
                        if (!valueSuccess) return (false, EnumReturnType.TypeNormal, null);
                        retnvalue = valueResult;
                    }

                    retntype = EnumReturnType.TypeReturn;

                    return (true, retntype, retnvalue);
                }
                else if (cp.eCommandType == EnumCommandType.type_for)
                {
                    CommandFor cf = (CommandFor)cp;

                    var (forSuccess, forRetntype, forRetnvalue) = await cf.RunAsync(src, class_stack, data_stack).ConfigureAwait(false);
                    if (!forSuccess) return (false, EnumReturnType.TypeNormal, null);

                    retntype = forRetntype;
                    retnvalue = forRetnvalue;
                    if (retntype == EnumReturnType.TypeReturn)
                        return (true, retntype, retnvalue);
                    if (retntype == EnumReturnType.TypeGoto)
                    {
                        if (!SeekLabelPosition((string)retnvalue, ref i))
                            return (true, retntype, retnvalue);
                    }

                }
                else if (cp.eCommandType == EnumCommandType.type_while)
                {
                    CommandWhile cw = (CommandWhile)cp;

                    var (whileSuccess, whileRetntype, whileRetnvalue) = await cw.RunAsync(src, class_stack, data_stack).ConfigureAwait(false);
                    if (!whileSuccess) return (false, EnumReturnType.TypeNormal, null);

                    retntype = whileRetntype;
                    retnvalue = whileRetnvalue;

                    if (retntype == EnumReturnType.TypeReturn)
                        return (true, retntype, retnvalue);
                    if (retntype == EnumReturnType.TypeGoto)
                    {
                        if (!SeekLabelPosition((string)retnvalue, ref i))
                            return (true, retntype, retnvalue);
                    }
                }
                else if (cp.eCommandType == EnumCommandType.type_break)
                {
                    retntype = EnumReturnType.TypeBreak;
                    return (true, retntype, retnvalue);
                }
                else if (cp.eCommandType == EnumCommandType.type_continue)
                {
                    retntype = EnumReturnType.TypeContinue;
                    return (true, retntype, retnvalue);
                }
                else if (cp.eCommandType == EnumCommandType.type_goto)
                {
                    CommandGoto cl = (CommandGoto)cp;

                    if (!SeekLabelPosition(cl.sNameLabel, ref i))
                    {
                        retntype = EnumReturnType.TypeGoto;
                        retnvalue = cl.sNameLabel;
                        return (true, retntype, retnvalue);
                    }
                    // 찾은 라벨 위치가 다음 진행될때 +1 되므로 다음행이 진행된다.
                    continue;
                }
                else if (cp.eCommandType == EnumCommandType.type_label)
                {
                    // label은 실행할 필요는 없다.
                }
                else if (cp.eCommandType == EnumCommandType.type_voidvalue)
                {
                    CommandVoidValue cr = (CommandVoidValue)cp;

                    var (valueSuccess, valueResult) = await cr.pValue.RunAsync(src, class_stack, data_stack).ConfigureAwait(false);
                    if (!valueSuccess) return (false, EnumReturnType.TypeNormal, null);
                    retnvalue = valueResult;
                }
                else if (cp.eCommandType == EnumCommandType.type_lastline)
                {
                    // 아무것도 하지 않는다. 디버그중 함수의 끝에서 스톱하기 위해서 존재하는 명령어
                }
                else
                {
                    src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary,
               parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else,
               "Known command (추가지원필요) : {0}", cp.ToString());
                    return (false, EnumReturnType.TypeNormal, null);
                }

                if (retn_step == EnumDebugStep.StepOver)
                {
                    bBreakMustNext = true;
                }
            }

            retntype = EnumReturnType.TypeNormal;   // 정상적으로 블럭이 끝났다.
            return (true, retntype, retnvalue);
        }

        bool SeekLabelPosition(string label, ref int seek_pos)
        {
            for (int i = 0; i < arrayCommand.Count; i++)
            {
                if (arrayCommand[i].eCommandType == EnumCommandType.type_label)
                {
                    CommandLabel cl = (CommandLabel)arrayCommand[i];

                    if (cl.sNameLabel == label)
                    {
                        seek_pos = i;
                        return true;
                    }
                }
            }

            return false;
        }

        
        public void SetBreakPoints(string filename, List<int> array)
        {
            CommandPublic cp;

            for (int y = 0; y < array.Count; y++)
            {
                for (int i = 0; i < arrayCommand.Count; i++)
                {
                    cp = arrayCommand[i];
                    if (cp.nRow == array[y])
                    {
                        cp.bBreakPoint = !cp.bBreakPoint;
                        break;
                    }
                }
            }
        }

        public void Load(byte[] buffer, string user_command, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (IsPublicLoadItem(block))
                {

                }
                else if (block.type == EnumBlockType.CommandSubstitution)
                {
                    CommandSubstitution cmd = new CommandSubstitution(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandDeclaration)
                {
                    CommandDeclaration cmd = new CommandDeclaration(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandMethod)
                {
                    CommandMethod cmd = new CommandMethod(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandReturn)
                {
                    CommandReturn cmd = new CommandReturn(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandFor)
                {
                    CommandFor cmd = new CommandFor(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandIf)
                {
                    CommandIf cmd = new CommandIf(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandElse)
                {
                    CommandElse cmd = new CommandElse(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandWhile)
                {
                    CommandWhile cmd = new CommandWhile(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandBreak)
                {
                    CommandBreak cmd = new CommandBreak(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandContinue)
                {
                    CommandContinue cmd = new CommandContinue(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandGoto)
                {
                    CommandGoto cmd = new CommandGoto(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandLabel)
                {
                    CommandLabel cmd = new CommandLabel(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandVoidValue)
                {
                    CommandVoidValue cmd = new CommandVoidValue(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else if (block.type == EnumBlockType.CommandLastLine)
                {
                    CommandLastLine cmd = new CommandLastLine(parentMethod, this);
                    cmd.Load(block.block_data, ref line);
                    arrayCommand.Add(cmd);
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                    //return;
                }
            }
        }

        public override string MakeDecompiledFile(int depth, bool bAddSemicolon)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < arrayCommand.Count; i++)
            {
                ScriptLibClass.AppendWithTab(sb, depth, "");
                string buffer = arrayCommand[i].MakeDecompiledFile(depth, bAddSemicolon);
                sb.AppendLine(buffer);
            }

            return sb.ToString();
        }

        // 각 커맨드는 , 로 구분한다. 
        // for(initializer; condition; iterator) 에서 initializer나 iterator들어가는 여러문장이 ,로 구분되어 들어간다.
        public string MakeDecompiledFileOneLine(int depth)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < arrayCommand.Count; i++)
            {
                string buffer = arrayCommand[i].MakeDecompiledFileCommaBlock(depth, i==0);
                sb.Append(buffer);
                if (i < arrayCommand.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }
    }
}
