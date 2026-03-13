/*
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing;
using AutoLibLocal;
using AutoLib;
using DialogControl;
using NetTools;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using BasicScreen.kdymain;
using ScriptLibRun;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ScriptFunctionElse.
	/// </summary>
	public class ScriptFunctionElse
	{
        
		public ScriptFunctionElse()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static int Function_ControlBox(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string tag;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out tag))	return -1;

			int[] pos= new int[1];

			TagPublicClass tp = TagLib.GetStructPublic(tag, ref pos);

			if(tp.enumTagType == EnumTagType.AI) 
			{

			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{

			}
			else 
			{
				scriptClass.ErrorMessage(String.Format("@ControlBox()는 DI나 AI Tag를 사용해야 합니다.({0})", tag));
				return -1;
			}

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(scriptClass.bHandOperation) 
				{
					if(tp.enumTagType == EnumTagType.DI) 
					{
						ControlBoxDigitalInputGo dialog = new ControlBoxDigitalInputGo();

						dialog.Go(scriptClass.formParent, tag);
					}
					else if(tp.enumTagType == EnumTagType.AI) 
					{
						ControlBoxAnalogInputGo dialog = new ControlBoxAnalogInputGo();

						dialog.Go(scriptClass.formParent, tag);
					}
					else {}
				}
			}

			return 1;
		}

		
        
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
			if(!scriptClass.GetArgumentString(buf, out msg1))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out msg2))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out msg_type))			return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				val = (double)MessageBox.Show(msg1, msg2, (MessageBoxButtons)msg_type); 
			}

			return 1;
		}

        
		static int Function_RGB(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			int value_r, value_g, value_b;
			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out value_r))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out value_g))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out value_b))	return -1;

			val = 0xFF000000+value_r*0x10000+value_g*0x100+value_b;

			return 1;
		}

		static int Function_ARGB(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			int value_a, value_r, value_g, value_b;
			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out value_a))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out value_r))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out value_g))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out value_b))	return -1;

			val = value_a*0x1000000+value_r*0x10000+value_g*0x100+value_b;

			return 1;
		}

		


        static int Run_rand(ScriptClass scriptClass, out object val)
        {
            val = random.Next(0x7FFF);	// C++에서 RAND_MAX함수가 7FFF이다. (오토베이스 8 버전과 호환성을 유지하기위해서)

            return 1;
        }

		static int Function_rand(ScriptClass scriptClass, string command, string argument, out object val)
		{
            return Run_rand(scriptClass, out val);
		}

		

		

		

		

		

        
		static int Function_DiCurr(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string tag;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out tag))	return -1;

			double curr;
			if(!GetDigital(scriptClass, tag, out curr))	return -1;
			val = curr;

			return 1;
		}

        
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
		}

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        
        static int Run_PlaySound(ScriptClass scriptClass, out object val, string filename)
        {
            val = 0;

            string path;

            if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				path = MakeFilePath.Sound(filename);

				if(!File.Exists(path)) 
				{
					if(Tools.IsLangKorean()) 
					{
						scriptClass.ErrorMessage(String.Format("PlaySound에서 [{0}] 파일을 찾을 수 없습니다.", path));
					}
					else 
					{
						scriptClass.ErrorMessage(String.Format("Can't found [{0}] file at PlaySound function", path));
					}
					return -1;
				}
				else 
				{
					Win32Function.PlaySound(null, 0 , EnumPlaySound.SND_ASYNC);	// 1 = SND_ASYNC
					Win32Function.PlaySound(path, 0 , EnumPlaySound.SND_ASYNC);	// 1 = SND_ASYNC
				}
			}

			return 1;
        }

		static int Function_PlaySound(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string filename;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out filename))	return -1;

            return Run_PlaySound(scriptClass, out val, filename);
		}

		static int Function_WinExec(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string filename;
			string args="";

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out filename))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				uint retn = Win32Function.WinExec(filename, (uint)EnumShowWindow.SW_SHOW); 

				if(retn < 32)
				{
					if(Tools.IsLangKorean())
						scriptClass.ErrorMessage(String.Format("WinExec 오류\nfilename={0}", filename, args));
					else
						scriptClass.ErrorMessage(String.Format("WinExec Error\nfilename={0}", filename, args));

					return -1;
				}
			}

			

			return 1;
		}

        
		static int Function_ON_OFF(ScriptClass scriptClass, string command, string argument, out object val, byte flag)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string tag;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out tag))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				int[] pos = new int[1];

				if(GetDigitalOutputPos(scriptClass, tag, ref pos)) 
				{	
					TagDoClass dout = TagLib.GetStructDO(tag, ref pos);

					if(scriptClass.bHandOperation && !SharedData.userInfo.HaveRightsHandOperationAndMsg(dout)) 
					{

					}
					else 
					{
						TagWrite.WriteCurrDO(tag, dout, (sbyte)flag, scriptClass.bHandOperation);
					}
				}
			}
			return 1;
		}

        
        static int Run_DiSet(ScriptClass scriptClass, out object retn, string tag, double tag_val)
        {
            retn = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                int[] pos = new int[1];

                if (!GetDigitalInputPos(scriptClass, tag, ref pos)) return -1;      // 태그가 있는지는 실행시에 검사한다. 2012-11-22 sprintf를 사용해서 태그를 만들었을때는 컴파일시 태그가 없다는 오류가 난다.

                TagDiClass di = TagLib.GetStructDI(tag, ref pos);
                if (scriptClass.bHandOperation && !SharedData.userInfo.HaveRightsHandOperationAndMsg(di))
                {

                }
                else
                {
                    TagWrite.WriteCurrDI(tag, di, (sbyte)tag_val, scriptClass.bHandOperation);
                }
            }

            return 1;
        }

		static int Function_DiSet(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string tag;
			double flag;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out tag))	return -1;

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out flag))	return -1;

            return Run_DiSet(scriptClass, out val, tag, flag);
		}

		static int Function_DoSetGroup(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			string tag;
			double flag;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out tag))	return -1;

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out flag))	return -1;

			int[] pos=new int[1];

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				TagPublicClass tp = TagLib.GetStructPublic(tag, ref pos);

				if(tp.enumTagType != EnumTagType.GDO) 
				{
					scriptClass.ErrorMessage(String.Format("{0} tag is not Do Group tag.", tag));
					return -1;
				}

				TagWrite.WriteCurrDoGroup((TagDoGroupClass)tp, (sbyte)flag, scriptClass.bHandOperation);
			}

			return 1;
		}

		[NonSerialized] public static ArrayList arrayMultiRegister=new ArrayList();

		static int Function_MultiRegister(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf="";
			
			int v;
			multiTrendTagStruct block = new multiTrendTagStruct();
			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out block.tag))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out v))	return -1;
			block.color = Tools.ConvertToColor(v);
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out v))	return -1;
			block.edge = (eTrendEdgeType)v;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				int[] pos = new int[1];
				if(!scriptClass.GetAnalogInputPos(block.tag, ref pos))
				{
					return -1;
				}
			}

			if(arrayMultiRegister.Count < 256) 
			{
				arrayMultiRegister.Add(block);
			}

			return 1;
		}

		static int Function_MultiClear(ScriptClass scriptClass, string command, string argument, out object val)
		{
			val = 0;
			arrayMultiRegister.Clear();

			return 1;
		}

		static int Function_MultiData(ScriptClass scriptClass, string command, string argument, out object val)
		{
			val = 0;
			if(arrayMultiRegister.Count == 0)	return 1;

			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			double v;
			eDataTime time_size;
			int graph_type;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out v))	return -1;
			time_size = (eDataTime)v;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out v))	return -1;
			graph_type = (int)v;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				ViewAnalogInputDataMain.ringViewAnalogInputDataMain.CreateMdi(TotalConfig.formMain, new ViewAnalogInputDataMain(time_size, arrayMultiRegister, ScriptFunctionSet.colorMultiTrendBack, graph_type), ConfigViewMain.nMdiCountOnBasicScreen);
			}

			arrayMultiRegister.Clear();

			return 1;
		}

		
        
		static int Function_ViewAlarmList(ScriptClass scriptClass, string command, string argument, out object val)
		{
			val = 0;

			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			int year, month, day;
			int msg_type;
			
			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out year))		return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out month))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out day))		return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out msg_type))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				BasicScreen.kdymain.ViewListAlarmMain.ViewAlarmList(year, month, day);
			}

			return 1;
		}

        

		public static int Function_Else(ScriptClass scriptClass, string command, string argument, out object val)
		{
			if(command == "ControlBox") 
			{
				return Function_ControlBox(scriptClass, command, argument, out val);
			}
            
			else if(command == "MessageBox") 
			{
				return Function_MessageBox(scriptClass, command, argument, out val);
			}
			else if(command == "RGB") 
			{
				return Function_RGB(scriptClass, command, argument, out val);
			}
			else if(command == "ARGB") 
			{
				return Function_ARGB(scriptClass, command, argument, out val);
			}
            
			else if(command == "rand") 
			{
				return Function_rand(scriptClass, command, argument, out val);
			}
			else if(command == "srand") 
			{
				return Function_srand(scriptClass, command, argument, out val);
			}
			
			else if(command == "DiCurr") 
			{
				return Function_DiCurr(scriptClass, command, argument, out val);
			}
            
			else if(command == "Message")
			{
				return Function_Message(scriptClass, command, argument, out val);
			}
			else if(command == "PlaySound")
			{
				return Function_PlaySound(scriptClass, command, argument, out val);
			}
			else if(command == "WinExec")
			{
				return Function_WinExec(scriptClass, command, argument, out val);
			}
			else if(command == "ON")
			{
				return Function_ON_OFF(scriptClass, command, argument, out val, 1);
			}
			else if(command == "OFF")
			{
				return Function_ON_OFF(scriptClass, command, argument, out val, 0);
			}
            
			else if(command == "DiSet")
			{
				return Function_DiSet(scriptClass, command, argument, out val);
			}
			else if(command == "DoSetGroup")
			{
				return Function_DoSetGroup(scriptClass, command, argument, out val);
			}
			else if(command == "MultiRegister")
			{
				return Function_MultiRegister(scriptClass, command, argument, out val);
			}
			else if(command == "MultiClear")
			{
				return Function_MultiClear(scriptClass, command, argument, out val);
			}
			else if(command == "MultiData")
			{
				return Function_MultiData(scriptClass, command, argument, out val);
			}
             
			else if(command == "ViewAlarmList")
			{
				return Function_ViewAlarmList(scriptClass, command, argument, out val);
			}

			else {}

			val = 0;

			return 0;
		}

        

	}
}
 */
