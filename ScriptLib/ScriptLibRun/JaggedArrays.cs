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
    /// 
    /// 배열에서  a[2,2][3][1,2,3][2] 이라면  arrayDemensional은 [][][][] = count가 4가된다. 각 배열의 차원은 DimensionalArray에 선언되어 있다.
    /// JaggedArray 가 4개이고 각각 2DArray 1DArray 3DArray 1DArray로 구성되어 있다.
    /// </summary>
    public class JaggedArrays
    {
        protected ScriptLibMemberMethod parentMethod;
        protected CommandPublic parentBlock;

        public List<DimensionalArray> arrayDimensional = null;

        public JaggedArrays(ScriptLibMemberMethod parent, CommandPublic parent_block)
        {
            parentMethod = parent;
            parentBlock = parent_block;
        }

        public async Task<bool> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack, List<int[]> dimensional_values)
        {
            for (int i = 0; i < arrayDimensional.Count; i++)
            {
                //if (!arrayDimensional[i].Run(src, class_stack, parent_stack, dimensional_values)) return false;
                var success = await arrayDimensional[i].RunAsync(src, class_stack, parent_stack, dimensional_values);
                if (!success) return false;
            }

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
                                    
                if (block.type == EnumBlockType.DimensinalArray)
                {
                    if (arrayDimensional == null)
                    {
                        arrayDimensional = new List<DimensionalArray>();
                    }
                    DimensionalArray da = new DimensionalArray(parentMethod, parentBlock);
                    da.Load(block.block_data, ref line);
                    arrayDimensional.Add(da);
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

            for (int i = 0; i < arrayDimensional.Count; i++)
            {
                sb.Append('[');
                sb.Append(arrayDimensional[i].MakeDecompiledFile(depth));
                sb.Append(']');
            }

            return sb.ToString();
        }
    }
}
