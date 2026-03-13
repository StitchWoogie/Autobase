using System;
using AutoLibLocal;
using AutoLib;
using ScriptLibRun;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ScriptFunctionAnalog.
	/// </summary>
	public class ScriptFunctionAnalog
	{
		/*
        public ScriptFunctionAnalog()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        
		static int Function_AiCurr(ScriptClass scriptClass, string command, string argument, out object val)
		{
			TagAiClass ai = scriptClass.FunctionAnalog_GetAnalogInputPoint(command, argument);

			val = 0;
	
			if(ai == null)	return -1;

			ai.NeedDataCurr = true;
			val = ai.curr;

			return 1;	
		}

		static int Function_AiSumTotal(ScriptClass scriptClass, string command, string argument, out object val)
		{
			TagAiClass ai = scriptClass.FunctionAnalog_GetAnalogInputPoint(command, argument);

			val = 0;
	
			if(ai == null)	return -1;

			val = ai.fSumTotal;	
			return 1;	
		}

		static int Function_AiHiHi(ScriptClass scriptClass, string command, string argument, out object val)
		{
			TagAiClass ai = scriptClass.FunctionAnalog_GetAnalogInputPoint(command, argument);

			val = 0;
	
			if(ai == null)	return -1;

			val = ai.hihi;	
			return 1;	
		}

		static int Function_AiHigh(ScriptClass scriptClass, string command, string argument, out object val)
		{
			TagAiClass ai = scriptClass.FunctionAnalog_GetAnalogInputPoint(command, argument);

			val = 0;
	
			if(ai == null)	return -1;

			val = ai.high;	
			return 1;	
		}

		static int Function_AiLow(ScriptClass scriptClass, string command, string argument, out object val)
		{
			TagAiClass ai = scriptClass.FunctionAnalog_GetAnalogInputPoint(command, argument);

			val = 0;
	
			if(ai == null)	return -1;

			val = ai.low;	
			return 1;	
		}

		static int Function_AiLoLo(ScriptClass scriptClass, string command, string argument, out object val)
		{
			TagAiClass ai = scriptClass.FunctionAnalog_GetAnalogInputPoint(command, argument);

			val = 0;
	
			if(ai == null)	return -1;

			val = ai.lolo;	
			return 1;	
		}

        
        static int Run_AiSetCurr(ScriptClass scriptClass, out object retn, string tag, double tag_val)
        {
            retn = 0;

            int[] pos = new int[1];

            if (!scriptClass.GetAnalogInputPos(tag, ref pos)) return -1;

            if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(scriptClass.bHandOperation && !scriptClass.HaveRightsHandOperationAndMsgAtScript(tag, tag))
					return 1;
				TagAiClass ai = TagLib.GetStructAI(tag, ref pos);
				TagWrite.WriteCurrAI(tag, ai, tag_val, scriptClass.bHandOperation);
			}

            return 1;
        }

		static int Function_AiSetCurr(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string tag;
			double tag_val;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out tag))	return -1;

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out tag_val))	return -1;

            return Run_AiSetCurr(scriptClass, out val, tag, tag_val);
		}

		public static int Function_Analog(ScriptClass scriptClass, string command, string argument, out object val)
		{
            
			if(command == "AiCurr") 
			{
				return Function_AiCurr(scriptClass, command, argument, out val);
			}
			if(command == "AiSumTotal") 
			{
				return Function_AiSumTotal(scriptClass, command, argument, out val);
			}
			else if(command == "AiHiHi") 
			{
				return Function_AiHiHi(scriptClass, command, argument, out val);
			}
			else if(command == "AiHigh") 
			{
				return Function_AiHigh(scriptClass, command, argument, out val);
			}
			else if(command == "AiLow") 
			{
				return Function_AiLow(scriptClass, command, argument, out val);
			}
			else if(command == "AiLoLo") 
			{
				return Function_AiLoLo(scriptClass, command, argument, out val);
			}
            
			else if(command == "AiSetCurr") 
			{
				return Function_AiSetCurr(scriptClass, command, argument, out val);
			}
			else {}

			val = 0;
			return 0;
		}*/

        public static int Run_AiCurr(ScriptClass script, string name, out object retn_value, object[] args)
        {
            string tag = (string)args[0];
            int[] pos = new int[1];

            retn_value = 0;

            TagAiClass ai = TagLib.GetStructAI(tag, ref pos);

            if (ai == null) return -1;

            ai.NeedDataCurr = true;

            retn_value = ai.curr;

            return 1;
        }

        public static int Run_AiSumTotal(ScriptClass script, string name, out object retn_value, object[] args)
        {
            string tag = (string)args[0];
            int[] pos = new int[1];

            retn_value = 0;

            TagAiClass ai = TagLib.GetStructAI(tag, ref pos);

            if (ai == null) return -1;

            retn_value = ai.fSumTotal;

            return 1;
        }

        public static int Run_AiHiHi(ScriptClass script, string name, out object retn_value, object[] args)
        {
            string tag = (string)args[0];
            int[] pos = new int[1];

            retn_value = 0;

            TagAiClass ai = TagLib.GetStructAI(tag, ref pos);

            if (ai == null) return -1;

            retn_value = ai.hihi;

            return 1;
        }

        public static int Run_AiHigh(ScriptClass script, string name, out object retn_value, object[] args)
        {
            string tag = (string)args[0];
            int[] pos = new int[1];

            retn_value = 0;

            TagAiClass ai = TagLib.GetStructAI(tag, ref pos);

            if (ai == null) return -1;

            retn_value = ai.high;

            return 1;
        }

        public static int Run_AiLow(ScriptClass script, string name, out object retn_value, object[] args)
        {
            string tag = (string)args[0];
            int[] pos = new int[1];

            retn_value = 0;

            TagAiClass ai = TagLib.GetStructAI(tag, ref pos);

            if (ai == null) return -1;

            retn_value = ai.low;

            return 1;
        }

        public static int Run_AiLoLo(ScriptClass script, string name, out object retn_value, object[] args)
        {
            string tag = (string)args[0];
            int[] pos = new int[1];

            retn_value = 0;

            TagAiClass ai = TagLib.GetStructAI(tag, ref pos);

            if (ai == null) return -1;

            retn_value = ai.lolo;

            return 1;
        }

        public static int Run_AiSetCurr(ScriptClass script, string name, out object retn_value, object[] args)
        {
            string tag = (string)args[0];
            double tag_val = (double)args[1];

            retn_value = 0;

            int[] pos = new int[1];

            if (!script.GetAnalogInputPos(tag, ref pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (script.bHandOperation && !script.HaveRightsHandOperationAndMsgAtScript(tag, tag))
                    return 1;
                TagAiClass ai = TagLib.GetStructAI(tag, ref pos);
                TagWrite.WriteCurrAI(tag, ai, tag_val, script.bHandOperation);
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Ai";

            prepare.AddMethod(prename, "AiCurr", "double", new ScriptExternalRun.DeleMethod(Run_AiCurr), "in:string:tag");
            prepare.AddMethod(prename, "AiSumTotal", "double", new ScriptExternalRun.DeleMethod(Run_AiSumTotal), "in:string:tag");
            prepare.AddMethod(prename, "AiHiHi", "double", new ScriptExternalRun.DeleMethod(Run_AiHiHi), "in:string:tag");
            prepare.AddMethod(prename, "AiHigh", "double", new ScriptExternalRun.DeleMethod(Run_AiHigh), "in:string:tag");
            prepare.AddMethod(prename, "AiLow", "double", new ScriptExternalRun.DeleMethod(Run_AiLow), "in:string:tag");
            prepare.AddMethod(prename, "AiLoLo", "double", new ScriptExternalRun.DeleMethod(Run_AiLoLo), "in:string:tag");

            prepare.AddMethod(prename, "AiSetCurr", "void", new ScriptExternalRun.DeleMethod(Run_AiSetCurr), "in:string:tag", "in:double:value");

            
        }
	}
}
