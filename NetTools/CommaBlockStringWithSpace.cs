using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetTools
{
    public class CommaBlockStringWithSpace : CommaBlockString
    {
        public override void GetString(ref string s)
        {
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                if (scanBufPos >= scanBufHap)
                {
                    s = sb.ToString();
                    return;
                }

                if (scanBuf[scanBufPos] == ' ' || scanBuf[scanBufPos] == '\t' || scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r' || scanBuf[scanBufPos] == cBlockCode)
                {
                    if (sb.Length != 0)
                    {
                        scanBufPos++;
                        s = sb.ToString();
                        return;
                    }
                }
                else
                {
                    sb.Append(scanBuf[scanBufPos]);
                }

                scanBufPos++;
            }

            /*
            s = "";

            while (true)
            {
                if (scanBufPos >= scanBufHap)
                {
                    return;
                }

                if (scanBuf[scanBufPos] == ' ' || scanBuf[scanBufPos] == '\t' || scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r' || scanBuf[scanBufPos] == cBlockCode)
                {
                    if (s.Length != 0)
                    {
                        scanBufPos++;
                        return;
                    }
                }
                else
                {
                    s += scanBuf[scanBufPos];
                }

                scanBufPos++;
            }*/
        }
    }
}
