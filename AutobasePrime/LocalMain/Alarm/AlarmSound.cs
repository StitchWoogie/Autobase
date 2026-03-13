using System;
using AutoLib;
using AutoLibLocal;
using System.IO;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// Summary description for AlarmSound.
	/// </summary>
	public class AlarmSound
	{
		public AlarmSound()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void Play(string alarm_wave, ushort prioriry)
		{
			if(ConfigAlarm.bAlarmSoundFlag == false)	return;

			sbyte method = AlarmPriority.AlarmPriorityGetOptionSound(prioriry);

			if(method == 0)	return;	// no sound

			string filename;

			switch(ConfigAlarm.cAlarmSoundType) 
			{
				case 0:	
					Tools.bell();
					break;
				case 1:
					if(alarm_wave.Length != 0) 
					{
						filename = String.Format("{0}\\sound\\{1}", TotalConfig.sDirWorkProject, alarm_wave);
						if(File.Exists(filename)) 
						{
							Win32Function.PlaySound(filename, IntPtr.Zero, EnumPlaySound.SND_ASYNC|EnumPlaySound.SND_NOSTOP); 
							break;
						}
					}
					Win32Function.PlaySound(ConfigAlarm.sAlarmWaveFile, IntPtr.Zero, EnumPlaySound.SND_ASYNC|EnumPlaySound.SND_NOSTOP);//SND_ASYNC | SND_NOSTOP
					break;
				default:	
					Tools.bell();
					break;
			}	
		}
	}
}
