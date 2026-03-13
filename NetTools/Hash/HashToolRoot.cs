using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetTools.Hash
{
    public class HashToolRoot
    {
        public uint nRootSeed;
        bool flag_try = false;
        bool flag_ok = false;

        public HashToolRoot()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);

            nRootSeed = (uint)rand.Next();
        }

        public void CheckParam(uint param1, uint param2, uint param3, uint param4, uint param5)
        {
            // 한번만 시도해야 한다. 또 시도하면 시간을 끌어준다.
            if (flag_try)
            {
                Thread.Sleep(1000);
                return;
            }

            flag_try = true;

            if (param1 != (nRootSeed + 1)) return;
            if (param2 != (nRootSeed - 2)) return;
            if (param3 != (nRootSeed * 3)) return;
            if (param4 != (nRootSeed / 4)) return;
            if (param5 != (nRootSeed ^ 5)) return;

            flag_ok = true;
        }

        protected bool IsNormalAccess()
        {
            if (!flag_ok)
            {
                Thread.Sleep(1000); // 틀렸을 경우에는 시간을 끌어준다.
                return false;        
            }

            return true;
        }
    }
}
