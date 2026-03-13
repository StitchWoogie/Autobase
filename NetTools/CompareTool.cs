using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetTools
{
    public class CompareTool
    {
        public static bool CompareBytes(byte[] comp1, int offset1, byte[] comp2, int offset2, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (comp1[offset1 + i] != comp2[offset2 + i]) return false;
            }

            return true;
        }
    }
}
