using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class CommandFor : CommandPublic
    {
        public CommandBlock pCommandBefore;            // loop 시작하기전 명령
        public RecursiveCondition pCondition;
        public CommandBlock pCommandAfter;             // loop 돌고난 후 명령

        public CommandBlock blockCommand;

        public CommandFor(ScriptLibMemberMethod parent, CommandPublic parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_for;
        }

        //public bool Run(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack, out EnumReturnType retntype, out object retnvalue)
        //{
        //    ClassDataStack data_stack = new ClassDataStack(parent_stack);   // 초기화 부분에 선언이 있으므로 스택을 따로 만들 필요가 있다.

        //    // 초기화 실행
        //    // false = 현재 스택에 초기화 된 스택을 가지고 와야 하므로 변수 할당 시 여기서 할당한 스택에 넣도록 한다.
        //    if (!pCommandBefore.Run(src, class_stack, data_stack, false, out retntype, out retnvalue)) return false;

        //    bool condition;

        //    while (true)
        //    {
        //        // 조건 검사
        //        if (!pCondition.CheckCondition(src, class_stack, data_stack, out condition)) return false;
        //        if (!condition) break;

        //        // 블럭 실행
        //        if (!blockCommand.Run(src, class_stack, data_stack, true, out retntype, out retnvalue)) return false;

        //        if (retntype == EnumReturnType.TypeBreak)   break;
        //        if (retntype == EnumReturnType.TypeReturn)  return true;
        //        if (retntype == EnumReturnType.TypeGoto) return true;

        //        if (retntype == EnumReturnType.TypeContinue) { }    // continue 문은 순서대로 진행하면 된다.     

        //        // 블럭 실행 후 count 실행
        //        if (!pCommandAfter.Run(src, class_stack, data_stack, true, out retntype, out retnvalue)) return false;
        //    }

        //    retntype = EnumReturnType.TypeNormal;
        //    return true;
        //}

        public async Task<(bool success, EnumReturnType retntype, object retnvalue)> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack)
        {
            EnumReturnType retntype = EnumReturnType.TypeNormal;
            object retnvalue = null;

            ClassDataStack data_stack = new ClassDataStack(parent_stack);   // 초기화 부분에 선언이 있으므로 스택을 따로 만들 필요가 있다.

            // 초기화 실행
            // false = 현재 스택에 초기화 된 스택을 가지고 와야 하므로 변수 할당 시 여기서 할당한 스택에 넣도록 한다.
            var (beforeSuccess, beforeRetntype, beforeRetnvalue) = await pCommandBefore.RunAsync(src, class_stack, data_stack, false).ConfigureAwait(false);
            if (!beforeSuccess) return (false, EnumReturnType.TypeNormal, null);

            retntype = beforeRetntype;
            retnvalue = beforeRetnvalue;

            while (true)
            {
                // 조건 검사
                var (condSuccess, condition) = await pCondition.CheckConditionAsync(src, class_stack, data_stack).ConfigureAwait(false);
                if (!condSuccess) return (false, EnumReturnType.TypeNormal, null);

                if (!condition) break;


                // 블럭 실행
                var (blockSuccess, blockRetntype, blockRetnvalue) = await blockCommand.RunAsync(src, class_stack, data_stack, true).ConfigureAwait(false);
                if (!blockSuccess) return (false, EnumReturnType.TypeNormal, null);

                if (retntype == EnumReturnType.TypeBreak) break;
                if (retntype == EnumReturnType.TypeReturn) return (true, retntype, retnvalue);
                if (retntype == EnumReturnType.TypeGoto) return (true, retntype, retnvalue);

                if (retntype == EnumReturnType.TypeContinue) { }    // continue 문은 순서대로 진행하면 된다.     

                // 블럭 실행 후 count 실행
                var (afterSuccess, afterRetntype, afterRetnvalue) = await pCommandAfter.RunAsync(src, class_stack, data_stack, true).ConfigureAwait(false);
                if (!afterSuccess) return (false, EnumReturnType.TypeNormal, null);

                // after 명령의 결과도 확인 (return/break/goto가 있을 수 있음)
                if (afterRetntype == EnumReturnType.TypeBreak) break;
                if (afterRetntype == EnumReturnType.TypeReturn) return (true, afterRetntype, afterRetnvalue);
                if (afterRetntype == EnumReturnType.TypeGoto) return (true, afterRetntype, afterRetnvalue);
            }

            retntype = EnumReturnType.TypeNormal;
            return (true, retntype, retnvalue);
        }


        /*
        public void Load(TextReader reader, ref int line)
        {
            string one_line = "";
            string command = "";
            string second_command = "";
            CommaTextReader comma = new CommaTextReader();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                line++;

                comma.Set(one_line);
                comma.GetString(ref command);
                comma.GetString(ref second_command);

                if (IsPublicLoadItem(command, second_command, comma))
                {

                }
                else if (command == "CommandFor")
                {
                    if (String.Compare(second_command, "END", true) == 0)
                    {
                        return;
                    }
                }
                else if (command == "CommandBefore")
                {
                    if (String.Compare(second_command, "BEGIN", true) == 0)
                    {
                        pCommandBefore = new CommandBlock(parentMethod, parentBlock);
                        pCommandBefore.Load(reader, command, ref line);
                    }
                }
                else if (command == "Condition")
                {
                    if (String.Compare(second_command, "BEGIN", true) == 0)
                    {
                        pCondition = new RecursiveCondition(parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                        pCondition.Load(reader, ref line);
                    }
                }
                else if (command == "CommandAfter")
                {
                    if (String.Compare(second_command, "BEGIN", true) == 0)
                    {
                        pCommandAfter = new CommandBlock(parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                        pCommandAfter.Load(reader, command, ref line);
                    }
                }
                else if (command == "CommandBlock")
                {
                    if (String.Compare(second_command, "BEGIN", true) == 0)
                    {
                        blockCommand = new CommandBlock(parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                        blockCommand.Load(reader, null, ref line);
                    }
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(reader, this.ToString(), command, second_command, line);
                    return;
                }

                
            }
        }*/

        public void Load(byte[] buffer, ref int line)
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
                else if (block.type == EnumBlockType.CommandBlock)
                {
                    blockCommand = new CommandBlock(parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                    blockCommand.Load(block.block_data, null, ref line);
                }
                else if (block.type == EnumBlockType.CommandBlock2)
                {
                    pCommandBefore = new CommandBlock(parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                    pCommandBefore.Load(block.block_data, null, ref line);
                }
                else if (block.type == EnumBlockType.CommandBlock3)
                {
                    pCommandAfter = new CommandBlock(parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                    pCommandAfter.Load(block.block_data, null, ref line);
                }
                else if (block.type == EnumBlockType.Condition)
                {
                    pCondition = new RecursiveCondition(parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                    pCondition.Load(block.block_data, ref line);
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

            sb.Append("for(");
            sb.Append(pCommandBefore.MakeDecompiledFileOneLine(depth));
            sb.Append("; ");
            sb.Append(pCondition.MakeDecompiledFile(depth));
            sb.Append("; ");
            sb.Append(pCommandAfter.MakeDecompiledFileOneLine(depth));
            sb.AppendLine(")");

            ScriptLibClass.AppendLineWithTab(sb, depth, "{{");
            sb.Append(blockCommand.MakeDecompiledFile(depth+1, true));
            ScriptLibClass.AppendWithTab(sb, depth, "}}"); 

            return sb.ToString();
        }
    }
}
