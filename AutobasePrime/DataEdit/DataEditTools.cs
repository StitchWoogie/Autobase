using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using AutoLibLocal;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using NetTools;
using System.IO;
using AutoLib;

namespace DataEdit
{
	/// <summary>
	/// Summary description for DataEditTools.
	/// </summary>
	public class DataEditTools
	{
		public DataEditTools()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public delegate void OnEventEditFontChanged();
		static public OnEventEditFontChanged editFontChanged = null;

		static public void EventGoOnEventEditFontChanged()
		{
			if(editFontChanged == null)	return;
			editFontChanged();
		}

		static public void callDataEditConfigDialog()
		{
			DialogDataEditConfig dialog = new DialogDataEditConfig();
			if(dialog.ShowDialog() == DialogResult.OK) 
			{
				if(dialog.bFontChang) EventGoOnEventEditFontChanged();
			}
		}

		static void readConfigData(TextReader reader)
		{
			CommaBlockString	comma = new CommaBlockString();
			string				one_line, imsi = "";
			
			one_line = reader.ReadLine();
			if(one_line == null || one_line.Length == 0) return;
			comma.Set(one_line);
			comma.GetBool(ref DataEditConfig.bSaveWriteReason);

			one_line = reader.ReadLine();
			if(one_line == null || one_line.Length == 0) return;
			comma.Set(one_line);
			DataEditConfig.dTime = comma.GetDateTime();

			one_line = reader.ReadLine();
			if(one_line == null || one_line.Length == 0) return;
			comma.Set(one_line);
			comma.GetString(ref imsi);			
			if(comma.IsEOS() == false) 
			{
				float		f = 0.0F;
				comma.GetFloat(ref f);
				try 
				{
					DataEditConfig.fEditFont = new Font(imsi, f);
				}
				catch {}
			}

			one_line = reader.ReadLine();
			if(one_line == null || one_line.Length == 0) return;
			comma.Set(one_line);
			comma.GetInt(ref DataEditConfig.nBasicEditMode);

			one_line = reader.ReadLine();
			if(one_line == null || one_line.Length == 0) return;
			comma.Set(one_line);
			comma.GetInt(ref DataEditConfig.nMaxTickCount);

			one_line = reader.ReadLine();
			if(one_line == null || one_line.Length == 0) return;
			comma.Set(one_line);
			comma.GetInt(ref DataEditConfig.nTextSaveLoadCount);

			one_line = reader.ReadLine();
			if(one_line == null || one_line.Length == 0) return;
			comma.Set(one_line);
			comma.GetInt(ref DataEditConfig.nTextSaveLoadType);

			one_line = reader.ReadLine();
			if(one_line == null || one_line.Length == 0) return;
			comma.Set(one_line);
			comma.GetString(ref DataEditConfig.sTextFilename);
		}

		static public void loadDataEditConfigData()
		{
			string filename = TotalConfig.sDirWorkProject+"\\"+"DataEdit.cfg";			
			if(!File.Exists(filename))	return;

			FileStream fs = File.OpenRead(filename);
			if(fs == null)	return;

			TextReader reader = new StreamReader(fs);
			if(reader == null) 
			{
				fs.Close();
				return;
			}
			readConfigData(reader);
			reader.Close();
			fs.Close();			// 2005-6-28 add
		}

		static public void saveDataEditConfigData()
		{
			string			path = TotalConfig.sDirWorkProject;
			
			if(!Directory.Exists(path)) 
			{
				Directory.CreateDirectory(path);
			}
			string filename = path+"\\"+"DataEdit.cfg";

			Stream fs = File.Open(filename, FileMode.Create);
			if(fs == null)	return;

			TextWriter writer = new StreamWriter(fs);
			writer.WriteLine("{0},", DataEditConfig.bSaveWriteReason);
			writer.WriteLine("{0},", DataEditConfig.dTime);
			writer.WriteLine("{0},{1},", DataEditConfig.fEditFont.FontFamily.Name, DataEditConfig.fEditFont.Size);
			writer.WriteLine("{0},", DataEditConfig.nBasicEditMode);
			writer.WriteLine("{0},", DataEditConfig.nMaxTickCount);
			writer.WriteLine("{0},", DataEditConfig.nTextSaveLoadCount);
			writer.WriteLine("{0},", DataEditConfig.nTextSaveLoadType);
			writer.WriteLine("{0},", DataEditConfig.sTextFilename);
			
			writer.Close();
			fs.Close();			// 2005-6-28 add
		}

		static public bool SaveHourDataStructAI(string tag, int year, int month, int day, int hour, int minute, HOUR_DATA_ANALOG_STRUCT data)	// AutoLibLocal 에 있는 함수이나 flag 때문에 복사한 후 ...
		{
			if(month < 1 || month > 12)		return false;	// overflow
			if(day < 1 || day > 31)			return false;	// overflow
			if(hour < 0 || hour > 23)		return false;	// overflow
			
			string filename;
			string tag_file;
			string data_dir;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

			filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\AI", data_dir, year, month);

			if(!Directory.Exists(filename)) 
			{
				Directory.CreateDirectory(filename);
			}

			filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, month, tag_file);

			FileStream fs;

			if(!File.Exists(filename)) 
			{
				fs = File.OpenWrite(filename);
				if(fs == null) 	return false;	// 자료 없음
				BinaryWriter bw = new BinaryWriter(fs);
				
				byte[] imsi = new byte[25];

				for(int i = 0; i < 23; i++)		imsi[i] = 0;

				bw.Write(imsi, 0, 18);	// header
				for(int i = 0; i < 24*31; i++) 
				{
					bw.Write(imsi, 0, 23);
				}
				bw.Close();
			}

			fs = File.OpenWrite(filename);
			if(fs == null) 	return false;	// 자료 없음

			BinaryWriter wr = new BinaryWriter(fs);

			long start = 18+((day-1)*24L+hour)*23;

			fs.Seek(start, SeekOrigin.Begin);
			//data.flag = 1;				// 삭제 by 김도열

			data.crc = data.CalcCRC();

			data.Write(wr);
			
			wr.Close();

			return true;
		}

		static public bool SaveHourDataStructAI(string tag, DateTime t, HOUR_DATA_ANALOG_STRUCT data)	// AutoLibLocal 에 있는 함수이나 flag 때문에 복사한 후 ...
		{
			return SaveHourDataStructAI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data);
		}

		static public bool SaveHourDataStructDI(string tag, int year, int month, int day, int hour, int minute, HOUR_DATA_DIGITAL_STRUCT data)	// AutoLibLocal 에 있는 함수이나 flag 때문에 복사한 후 ...
		{
			string filename;
			string tag_file;
			string data_dir;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

			filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\DI", data_dir, year, month);

			if(!Directory.Exists(filename)) 
			{
				Directory.CreateDirectory(filename);
			}

			filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, month, tag_file);

			FileStream fs;

			if(!File.Exists(filename)) 
			{
				fs = File.OpenWrite(filename);
				if(fs == null) 	return false;	// 자료 없음
				BinaryWriter bw = new BinaryWriter(fs);
				
				byte[] imsi = new byte[25];

				for(int i = 0; i < 23; i++)		imsi[i] = 0;

				bw.Write(imsi, 0, 18);	// header
				for(int i = 0; i < 24*31; i++) 
				{
					bw.Write(imsi, 0, 9);
				}
				bw.Close();
			}

			fs = File.OpenWrite(filename);
			if(fs == null) 	return false;	// 자료 없음

			BinaryWriter wr = new BinaryWriter(fs);

			long start = 18+((day-1)*24L+hour)*9;

			fs.Seek(start, SeekOrigin.Begin);
			//data.flag = 1;				// 삭제 by 김도열

			data.crc = data.CalcCRC();

			data.Write(wr);
			
			wr.Close();

			return true;
		}

		static public bool SaveHourDataStructDI(string tag, DateTime t, HOUR_DATA_DIGITAL_STRUCT data)	// AutoLibLocal 에 있는 함수이나 flag 때문에 복사한 후 ...
		{
			return SaveHourDataStructDI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data);
		}


	}
}
