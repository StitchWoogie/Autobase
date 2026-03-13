using System;
using System.IO;
using NetTools;
using System.Windows.Forms;

namespace AutoLibLocal
{
	public struct ALARM_PRIORITY_STRUCT
	{
		public sbyte	bLinePrint;
		public sbyte	cScreen;
		public sbyte	cSound;
	}

	/// <summary>
	/// Summary description for AlarmPriority.
	/// </summary>
	public class AlarmPriority
	{
		const int MAX_ALARM_PRIORITY = 1000;
		public static ALARM_PRIORITY_STRUCT[] alarmPriority = new ALARM_PRIORITY_STRUCT[MAX_ALARM_PRIORITY];

		static AlarmPriority()
		{
			//
			// TODO: Add constructor logic here
			//
			Load();
		}

		public static sbyte AlarmPriorityGetOptionScreen(int priority)
		{
			if(priority < 0 || priority > 999)	return 1;

			return alarmPriority[priority].cScreen;
		}

		public static void Save()
		{
			string filename;
			TextWriter writer;
			int i;
	
			filename = String.Format("{0}\\Alarm", TotalConfig.sDirWorkProject);
			Directory.CreateDirectory(filename);
			filename = String.Format("{0}\\Alarm\\priority.lstx", TotalConfig.sDirWorkProject);
			writer = new StreamWriter(filename);
			if(writer == null) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("파일을 쓸 수 없습니다.\n읽기 전용이거나 파일을 만들 수 없습니다.", filename);
				}
				else 
				{
					MessageBox.Show("Can't write file.\nIt is Read only file or makefile.", filename);
				}
				return;
			}
			for(i = 0; i < MAX_ALARM_PRIORITY; i++) 
			{
				writer.Write("{0:000},", i);
				writer.Write("{0},", alarmPriority[i].bLinePrint);
				writer.Write("{0},", alarmPriority[i].cScreen);
				writer.Write("{0},", alarmPriority[i].cSound);
				writer.WriteLine();
			}
			writer.Close();	
		}

		public static void Load()
		{
			int i;
			TextReader reader;
			string buf;
			CommaBlockString comma = new CommaBlockString();
			int priority = 0;

			for(i = 0; i < 1000; i++) 
			{
				alarmPriority[i].bLinePrint = 1;
				alarmPriority[i].cScreen = 1;	// one message
				alarmPriority[i].cSound = 1;	// one sound
			}

			string filename;
			filename = String.Format("{0}\\Alarm\\priority.lstx", TotalConfig.sDirWorkProject);
			if(File.Exists(filename)) 
			{
				reader = new StreamReader(filename);
			}
			else 
			{
				filename = String.Format("{0}\\priority.lst", TotalConfig.sDirWorkProject);
				if(File.Exists(filename))
					reader = new StreamReader(filename);	
				else
					reader = null;
			}

			if(reader == null)	return;
			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				if(buf.Length == 0)	continue;
				comma.Set(buf);
				comma.GetInt(ref priority);
				if(priority < 0 || priority > 999)	continue;
				comma.GetChar(ref alarmPriority[priority].bLinePrint);
				comma.GetChar(ref alarmPriority[priority].cScreen);
				comma.GetChar(ref alarmPriority[priority].cSound);
			}
			reader.Close();
		}

		sbyte AlarmPriorityGetOptionPrint(int priority)
		{
			if(priority < 0 || priority > 999)	return 1;

			return alarmPriority[priority].bLinePrint;
		}

		public static sbyte AlarmPriorityGetOptionSound(int priority)
		{
			if(priority < 0 || priority > 999)	return 1;

			return alarmPriority[priority].cSound;
		}
	}
}

