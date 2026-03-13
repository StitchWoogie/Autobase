using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    /// <summary>
    /// 배열에서 [a,b,c] 같은 각 차원의 위치를 말한다.
    /// 
    /// </summary>
    public class DimensionalArray
    {
        public List<RecursiveValue> arrayValue = new List<RecursiveValue>();  // 변수가 a[3,4,5] 이면 arrayValue에 각각 3,4,5 의 값이 들어 있다.

        protected ScriptLibMemberMethod parentMethod;
        protected CommandPublic parentBlock;

        public DimensionalArray(ScriptLibMemberMethod parent, CommandPublic parent_block)
        {
            parentMethod = parent;
            parentBlock = parent_block;
        }

        public async Task<bool> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack, List<int[]> dimensional_values)
        {
            int[] dp = new int[arrayValue.Count];

            for (int i = 0; i < arrayValue.Count; i++)
            {
                var (success, val) = await arrayValue[i].RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success) return false;

                dp[i] = ObjectValue.ToInt(val);
            }

            dimensional_values.Add(dp);

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

                if (block.type == EnumBlockType.RecursiveValue)
                {
                    RecursiveValue rv = new RecursiveValue(parentMethod, parentBlock);
                    rv.Load(block.block_data, ref line);
                    arrayValue.Add(rv);
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

            for (int i = 0; i < arrayValue.Count; i++)
            {
                sb.Append(arrayValue[i].MakeDecompiledFile(depth));
                if (i < arrayValue.Count - 1)
                    sb.Append(",");
            }
            
            return sb.ToString();
        }
    }
}
