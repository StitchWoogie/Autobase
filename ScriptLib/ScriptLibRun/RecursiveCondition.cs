using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class RecursiveCondition : ColumnRowInfo
    {
        protected enum EnumCompareMethod
        {
            Unknown = 0,
            Equal = 1,
            NotEqual = 2,
            BigEqual = 3,
            SmallEqual = 4,
            Big = 5,
            Small = 6,
            True = 7,
            False = 8,
        }

        protected enum EnumMultiCompare
        {
            One,
            And,
            Or,
        }

        protected string sSourceString;
        protected RecursiveCondition leftCondition;
        protected RecursiveCondition rightCondition;
        protected RecursiveValue leftVal = null;
        protected RecursiveValue rightVal = null;
        protected EnumCompareMethod eCompare;
        protected EnumMultiCompare eMulti;

        protected ScriptLibMemberMethod parentMethod;
        protected CommandPublic parentBlock;

        public RecursiveCondition(ScriptLibMemberMethod parent, CommandPublic parent_block)
        {
            parentMethod = parent;
            parentBlock = parent_block;
        }

        public async Task<(bool success, bool condition_value)> CheckConditionAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack)
        {
            bool condition_value = false;

            if (eMulti == EnumMultiCompare.And)
            {
                var (success1, con1) = await leftCondition.CheckConditionAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, false);

                var (success2, con2) = await rightCondition.CheckConditionAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, false);

                condition_value = (con1 && con2);
            }
            else if (eMulti == EnumMultiCompare.Or)
            {
                var (success1, con1) = await leftCondition.CheckConditionAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, false);

                var (success2, con2) = await rightCondition.CheckConditionAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, false);

                condition_value = (con1 || con2);
            }
            else if (eMulti == EnumMultiCompare.One)
            {
                if (eCompare == EnumCompareMethod.Big)
                {
                    var (success1, value1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success1) return (false, false);

                    var (success2, value2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success2) return (false, false);

                    condition_value = (ObjectValue.ToDouble(value1) > ObjectValue.ToDouble(value2));
                    return (true, condition_value);
                }
                else if (eCompare == EnumCompareMethod.BigEqual)
                {
                    var (success1, value1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success1) return (false, false);

                    var (success2, value2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success2) return (false, false);

                    condition_value = (ObjectValue.ToDouble(value1) >= ObjectValue.ToDouble(value2));
                    return (true, condition_value);
                }
                else if (eCompare == EnumCompareMethod.Equal)
                {
                    var (success1, value1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success1) return (false, false);

                    var (success2, value2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success2) return (false, false);

                    // null 체크
                    if (value1 == null || value2 == null)
                    {
                        condition_value = (value1 == value2);
                    }
                    else if (value1.GetType() == typeof(string) && value2.GetType() == typeof(string))
                    {
                        condition_value = ((string)value1 == (string)value2);
                    }
                    else if (value1.GetType() == typeof(string) || value2.GetType() == typeof(string))
                    {
                        src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary,
                            parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else,
                            "문자열과 다른 종류를 비교할 수 없습니다.");
                        return (false, false);
                    }
                    else
                    {
                        condition_value = (ObjectValue.ToDouble(value1) == ObjectValue.ToDouble(value2));
                    }
                    return (true, condition_value);
                }
                else if (eCompare == EnumCompareMethod.NotEqual)
                {
                    var (success1, value1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success1) return (false, false);

                    var (success2, value2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success2) return (false, false);

                    // 2017-3-6 변수가 object인경우 null인 경우가 있다.
                    if (value1 == null || value2 == null)
                    {
                        condition_value = (value1 != value2);
                    }
                    else if (value1.GetType() == typeof(string) && value2.GetType() == typeof(string))
                    {
                        condition_value = ((string)value1 != (string)value2);
                    }
                    else if (value1.GetType() == typeof(string) || value2.GetType() == typeof(string))
                    {
                        src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "문자열과 다른 종류를 비교할 수 없습니다.");

                        return (false, false);
                    }
                    else
                    {
                        condition_value = (ObjectValue.ToDouble(value1) != ObjectValue.ToDouble(value2));
                    }

                    return (true, condition_value);
                }
                else if (eCompare == EnumCompareMethod.Small)
                {
                    var (success1, value1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success1) return (false, false);

                    var (success2, value2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success2) return (false, false);

                    condition_value = (ObjectValue.ToDouble(value1) < ObjectValue.ToDouble(value2));
                    return (true, condition_value);
                }
                else if (eCompare == EnumCompareMethod.SmallEqual)
                {
                    var (success1, value1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success1) return (false, false);

                    var (success2, value2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success2) return (false, false);

                    condition_value = (ObjectValue.ToDouble(value1) <= ObjectValue.ToDouble(value2));
                    return (true, condition_value);
                }
                else if (eCompare == EnumCompareMethod.True)
                {
                    var (success1, value1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success1) return (false, false);

                    condition_value = ObjectValue.ToDouble(value1) != 0;
                    return (true, condition_value);
                }
                else if (eCompare == EnumCompareMethod.False)
                {
                    var (success1, value1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!success1) return (false, false);

                    condition_value = ObjectValue.ToDouble(value1) == 0;
                    return (true, condition_value);
                }
            }

            return (true, condition_value);
        }


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
                else if (block.type == EnumBlockType.eCompare)
                {
                    eCompare = (EnumCompareMethod)block.ReadByte();
                }
                else if (block.type == EnumBlockType.eCompareMulti)
                {
                    eMulti = (EnumMultiCompare)block.ReadByte();
                }
                else if (block.type == EnumBlockType.SourceString)
                {
                    sSourceString = block.ReadString();
                }
                else if (block.type == EnumBlockType.RecursiveValue)
                {
                    if (leftVal == null)
                    {
                        leftVal = new RecursiveValue(parentMethod, parentBlock);
                        leftVal.Load(block.block_data, ref line);
                    }
                    else if (rightVal == null)
                    {
                        rightVal = new RecursiveValue(parentMethod, parentBlock);
                        rightVal.Load(block.block_data, ref line);
                    }
                }
                else if (block.type == EnumBlockType.Condition)
                {
                    if (leftCondition == null)
                    {
                        leftCondition = new RecursiveCondition(parentMethod, parentBlock);
                        leftCondition.Load(block.block_data, ref line);
                    }
                    else if (rightCondition == null)
                    {
                        rightCondition = new RecursiveCondition(parentMethod, parentBlock);
                        rightCondition.Load(block.block_data, ref line);
                    }
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                    //return;
                }
            }
        }

        public string MakeDecompiledFile(int depth)
        {
            StringBuilder sb = new StringBuilder();

            if (eMulti == EnumMultiCompare.One)
            {
                if (eCompare == EnumCompareMethod.True)
                {
                    sb.Append(leftVal.MakeDecompiledFile(depth));
                }
                else if (eCompare == EnumCompareMethod.False)
                {
                    sb.Append("!");
                    sb.Append(leftVal.MakeDecompiledFile(depth));
                }
                else if (eCompare == EnumCompareMethod.Big)
                {
                    sb.Append(leftVal.MakeDecompiledFile(depth));
                    sb.Append(" > ");
                    sb.Append(rightVal.MakeDecompiledFile(depth));
                }
                else if (eCompare == EnumCompareMethod.BigEqual)
                {
                    sb.Append(leftVal.MakeDecompiledFile(depth));
                    sb.Append(" >= ");
                    sb.Append(rightVal.MakeDecompiledFile(depth));
                }
                else if (eCompare == EnumCompareMethod.Equal)
                {
                    sb.Append(leftVal.MakeDecompiledFile(depth));
                    sb.Append(" == ");
                    sb.Append(rightVal.MakeDecompiledFile(depth));
                }
                else if (eCompare == EnumCompareMethod.NotEqual)
                {
                    sb.Append(leftVal.MakeDecompiledFile(depth));
                    sb.Append(" != ");
                    sb.Append(rightVal.MakeDecompiledFile(depth));
                }
                else if (eCompare == EnumCompareMethod.Small)
                {
                    sb.Append(leftVal.MakeDecompiledFile(depth));
                    sb.Append(" < ");
                    sb.Append(rightVal.MakeDecompiledFile(depth));
                }
                else if (eCompare == EnumCompareMethod.SmallEqual)
                {
                    sb.Append(leftVal.MakeDecompiledFile(depth));
                    sb.Append(" <= ");
                    sb.Append(rightVal.MakeDecompiledFile(depth));
                }
                
            }
            else if (eMulti == EnumMultiCompare.And)
            {
                // 두개이상의 비교가 있으면 () 가 원래 있었다.
                if (leftCondition.eMulti != EnumMultiCompare.One)
                    sb.Append("(");
                sb.Append(leftCondition.MakeDecompiledFile(depth));
                if (leftCondition.eMulti != EnumMultiCompare.One)
                    sb.Append(")");

                sb.Append(" && ");

                if (rightCondition.eMulti != EnumMultiCompare.One)
                    sb.Append("(");
                sb.Append(rightCondition.MakeDecompiledFile(depth));
                if (rightCondition.eMulti != EnumMultiCompare.One)
                    sb.Append(")");
            }
            else if (eMulti == EnumMultiCompare.Or)
            {
                // 두개이상의 비교가 있으면 () 가 원래 있었다.
                if (leftCondition.eMulti != EnumMultiCompare.One)
                    sb.Append("(");
                sb.Append(leftCondition.MakeDecompiledFile(depth));
                if (leftCondition.eMulti != EnumMultiCompare.One)
                    sb.Append(")");

                sb.Append(" || ");

                if (rightCondition.eMulti != EnumMultiCompare.One)
                    sb.Append("(");
                sb.Append(rightCondition.MakeDecompiledFile(depth));
                if (rightCondition.eMulti != EnumMultiCompare.One)
                    sb.Append(")");
            }
            /*
            if (eCalcMethod == EnumCalcMethod.LastValue)
            {
                if (eValueType == EnumLastValueType.NewClass)
                {
                    sb.Append("new ");
                    sb.Append(pMethod.MakeDecompiledFile(depth));
                }
                else
                {
                    sb.Append(sLastValue);
                }
            }
            else if (eCalcMethod == EnumCalcMethod.Parenthesis)
            {
                sb.Append("(");
                sb.Append(leftVal.MakeDecompiledFile(depth));
                sb.Append(")");
            }
            else
            {
                sb.Append(leftVal.MakeDecompiledFile(depth));
                sb.Append(" ");

                if (eCalcMethod == EnumCalcMethod.And)
                    sb.Append("&");
                else if (eCalcMethod == EnumCalcMethod.Divide)
                    sb.Append("/");
                else if (eCalcMethod == EnumCalcMethod.Minus)
                    sb.Append("-");
                else if (eCalcMethod == EnumCalcMethod.Multiply)
                    sb.Append("*");
                else if (eCalcMethod == EnumCalcMethod.Or)
                    sb.Append("|");
                else if (eCalcMethod == EnumCalcMethod.Plus)
                    sb.Append("+");
                else if (eCalcMethod == EnumCalcMethod.Remainder)
                    sb.Append("%");
                else if (eCalcMethod == EnumCalcMethod.Xor)
                    sb.Append("^");
                else
                    sb.Append("????");

                sb.Append(" ");
                sb.Append(rightVal.MakeDecompiledFile(depth));
            }*/

            return sb.ToString();
        }
    }
}
