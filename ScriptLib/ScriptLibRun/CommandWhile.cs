using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class CommandWhile : CommandPublic
    {
        public RecursiveCondition pCondition;
        public CommandBlock blockCommand;

        public CommandWhile(ScriptLibMemberMethod parent, CommandBlock parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_while;
        }

        public async Task<(bool success, EnumReturnType retntype, object retnvalue)> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack)
        {
            EnumReturnType retntype = EnumReturnType.TypeNormal;
            object retnvalue = null;

            while (true)
            {
                // 조건 검사
                var (conSuccess, condition) = await pCondition.CheckConditionAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!conSuccess) break;


                // 블럭 실행
                var (blockSuccess, blockRetntype, blockRetnvalue) = await blockCommand.RunAsync(src, class_stack, parent_stack, true).ConfigureAwait(false);

                retntype = blockRetntype;
                retnvalue = blockRetnvalue;

                if (retntype == EnumReturnType.TypeBreak) break;
                if (retntype == EnumReturnType.TypeReturn) return (true, retntype, retnvalue);
                if (retntype == EnumReturnType.TypeGoto) return (true, retntype, retnvalue);

                if (retntype == EnumReturnType.TypeContinue) { }    // continue 문은 순서대로 진행하면 된다.     
            }

            retntype = EnumReturnType.TypeNormal;   // loop 가 정상적으로 잘 끝났다.

            return (true, retntype, retnvalue); ;
        }

        //public bool Run(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack, out EnumReturnType retntype, out object retnvalue)
        //{
        //    retntype = EnumReturnType.TypeNormal;
        //    retnvalue = 0;

        //    bool condition;

        //    while (true)
        //    {
        //        // 조건 검사
        //        if (!pCondition.CheckCondition(src, class_stack, parent_stack, out condition)) return false;
        //        if (!condition) break;

        //        // 블럭 실행
        //        if (!blockCommand.Run(src, class_stack, parent_stack, true, out retntype, out retnvalue)) return false;

        //        if (retntype == EnumReturnType.TypeBreak)  break;   
        //        if (retntype == EnumReturnType.TypeReturn) return true;
        //        if (retntype == EnumReturnType.TypeGoto) return true;

        //        if (retntype == EnumReturnType.TypeContinue) { }    // continue 문은 순서대로 진행하면 된다.     
        //    }

        //    retntype = EnumReturnType.TypeNormal;   // loop 가 정상적으로 잘 끝났다.

        //    return true;
        //}

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
                else if (command == "CommandWhile")
                {
                    if (String.Compare(second_command, "END", true) == 0)
                    {
                        return;
                    }
                }
                else if (command == "Condition")
                {
                    if (String.Compare(second_command, "BEGIN", true) == 0)
                    {
                        pCondition = new RecursiveCondition(parentMethod, parentBlock);
                        pCondition.Load(reader, ref line);
                    }
                }
                else if (command == "CommandBlock")
                {
                    if (String.Compare(second_command, "BEGIN", true) == 0)
                    {
                        blockCommand = new CommandBlock(parentMethod, parentBlock);
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
                    blockCommand = new CommandBlock(parentMethod, parentBlock); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                    blockCommand.Load(block.block_data, null, ref line);
                }
                else if (block.type == EnumBlockType.Condition)
                {
                    pCondition = new RecursiveCondition(parentMethod, parentBlock); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
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

            sb.Append("while(");
            sb.Append(pCondition.MakeDecompiledFile(depth));
            sb.AppendLine(")");

            ScriptLibClass.AppendLineWithTab(sb, depth, "{{");
            sb.Append(blockCommand.MakeDecompiledFile(depth + 1, true));
            ScriptLibClass.AppendWithTab(sb, depth, "}}");

            return sb.ToString();
        }
    }
}
