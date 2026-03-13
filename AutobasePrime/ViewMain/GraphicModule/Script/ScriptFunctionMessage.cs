using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;
using System.Windows.Forms;

namespace GraphicModule
{
    class ScriptFunctionMessage
    {
        /*
        static int Function_MessageBox(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string msg1;
            string msg2;
            int msg_type;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out msg1)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out msg2)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out msg_type)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                val = (double)MessageBox.Show(msg1, msg2, (MessageBoxButtons)msg_type);
            }

            return 1;
        }*/

        /*
		static int Function_Message(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string msg;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out msg))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				MessageDisplay.Show(msg);
			}

			return 1;
		}*/

        static int Run_Message(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                // 상시스크립트가 스레드가 추가되면서 안전하게 스레드 호출을 사용한다. 2015-11-4
                MessageDisplay.ShowInThread((string)args[0]);
                //MessageDisplay.ShowInThread((string)args[0]);
            }

            retn_value = 0;
            return 1;
        }

        static int Run_MessageBox(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        {
            retn_value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                retn_value = (int)MessageBox.Show((string)args[0], (string)args[1], (MessageBoxButtons)(int)args[2]);
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Message";

            prepare.AddMethod(prename, "Message", "void", new ScriptExternalRun.DeleMethod(Run_Message), "in:string:text");
            prepare.AddMethod(prename, "MessageBox", "int", new ScriptExternalRun.DeleMethod(Run_MessageBox), "in:string:text", "in:string:title", "in:int:button_type");
        }
    }
}
