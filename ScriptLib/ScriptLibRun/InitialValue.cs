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
    /// RecursiveValue의 변수 선언시 초기값이 있는 경우  int[] a = {2, 3}; 와 같은 경우
    /// EnumLastValueType.InitArray 이거나 EnumLastValueType.NewArray일 경우 이 클래스가 사용된다.
    /// </summary>
    public class InitialValue
    {
        protected ScriptLibMemberMethod parentMethod;
        protected CommandPublic parentBlock;

        protected string new_command = null;   
        protected List<RecursiveValue> arrayValues = null;

        public InitialValue(ScriptLibMemberMethod parent, CommandPublic parent_block)
        {
            parentMethod = parent;
            parentBlock = parent_block;
        }

        //public bool Run(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack, out object retn_val)
        //{
        //    retn_val = null;

        //    if (arrayValues != null)
        //    {
        //        object[] val = new object[arrayValues.Count];
        //        for (int i = 0; i < arrayValues.Count; i++)
        //        {
        //            if (!arrayValues[i].Run(src, class_stack, parent_stack, out val[i])) return false;
        //        }
        //        retn_val = val;
        //    }

        //    return true;
        //}

        public async Task<(bool success, object retn_val)> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack)
        {
            object retn_val = null;

            if (arrayValues != null)
            {
                object[] val = new object[arrayValues.Count];

                for (int i = 0; i < arrayValues.Count; i++)
                {
                    var (success, value) = await arrayValues[i].RunAsync(src, class_stack, parent_stack);
                    if (!success) return (false, null);

                    val[i] = value;
                }

                retn_val = val;
            }

            return (true, retn_val);
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
                    if (arrayValues == null)
                        arrayValues = new List<RecursiveValue>();

                    RecursiveValue rv = new RecursiveValue(parentMethod, parentBlock);
                    rv.Load(block.block_data, ref line);
                    arrayValues.Add(rv);
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

            sb.Append("{");
            for (int i = 0; i < arrayValues.Count; i++)
            {
                sb.Append(arrayValues[i].MakeDecompiledFile(depth));
                if(i < arrayValues.Count-1)
                    sb.Append(", ");
            }
            sb.Append("}");
            
            return sb.ToString();
        }
    }
}
