using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetTools
{
    public class CommaTextMaker
    {
        StringBuilder sb = new StringBuilder();

        public void Write(string format, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] != null && args[i].GetType() == Type.GetType("System.String"))
                {
                    if (CommaTextWriter.IsExistBlockCode((string)args[i]))
                        args[i] = CommaTextWriter.MakeString((string)args[i]);
                }
            }

            sb.AppendFormat(format, args);
        }

        public string GetResult()
        {
            return sb.ToString();
        }
    }
}
