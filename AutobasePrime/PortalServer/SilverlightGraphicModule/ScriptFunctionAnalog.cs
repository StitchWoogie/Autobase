using System;
using AutoLibLocal;
using AutoLib;

namespace SilverlightGraphicModule
{
	/// <summary>
	/// Summary description for ScriptFunctionAnalog.
	/// </summary>
	public class ScriptFunctionAnalog
	{
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

		static int Function_AiSetCurr(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string tag;
			int[] pos = new int[1];
			double tag_val;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out tag))	return -1;

			if(!scriptClass.GetAnalogInputPos(tag, ref pos))	return -1;

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out tag_val))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(scriptClass.bHandOperation && !scriptClass.HaveRightsHandOperationAndMsgAtScript(tag, tag))
					return 1;
				TagAiClass ai = TagLib.GetStructAI(tag, ref pos);
				TagWrite.WriteCurrAI(tag, ai, tag_val, scriptClass.bHandOperation);
			}

			return 1;
		}

		public static int Function_Analog(ScriptClass scriptClass, string command, string argument, out object val)
		{
			if(command == "AiCurr") 
			{
				return Function_AiCurr(scriptClass, command, argument, out val);
			}
			else if(command == "AiSumTotal") 
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
		}
	}
}
