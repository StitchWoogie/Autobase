using System;
using System.IO;
using System.Threading.Tasks;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for DataSave.
	/// </summary>
	public class DataSave
	{
        public DataSave()
		{
            //
            // TODO: Add constructor logic here
            //
        }


        #region FileSystem
		/*
        static public bool SaveMinDataStructAI(string tag, int year, int month, int day, int hour, int minute, TREND_AI_STRUCT data, bool flag)
		{
			if(month < 1 || month > 12)		return false;	// overflow
			if(day < 1 || day > 31)			return false;	// overflow
			if(hour < 0 || hour > 23)		return false;	// overflow
			if(minute < 0 || minute > 59)	return false;	// overflow
			
			string filename;
			string tag_file;
			string data_dir;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

			filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\AI", data_dir, year, month);

			if(!Directory.Exists(filename)) 
			{
				Directory.CreateDirectory(filename);
			}

			filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, month, tag_file);

			FileStream fs;

			if(!File.Exists(filename)) 
			{
				fs = File.OpenWrite(filename);
				if(fs == null) 	return false;	// 자료 없음
				BinaryWriter bw = new BinaryWriter(fs);
				
				byte[] imsi = new byte[25];

				for(int i = 0; i < 25; i++)		imsi[i] = 0;

				for(int i = 0; i < 60*24*31; i++) 
				{
					bw.Write(imsi, 0, 25);
                }
				bw.Close();
			}

			fs = File.OpenWrite(filename);
			if(fs == null) 	return false;	// 자료 없음

			BinaryWriter wr = new BinaryWriter(fs);

			long start = 25;
			start = start*((day-1)*1440+hour*60L+minute);

			fs.Seek(start, SeekOrigin.Begin);

			FILE_TREND_AI_STRUCT trend = new FILE_TREND_AI_STRUCT();

			if(flag == false) trend.day = 0;		// 2005-6-28 add by kdy 자료삭제를 위해
			else			  trend.day = (byte)day;
			trend.hour = (byte)hour;
			trend.min = (byte)minute;
			trend.data.fAverage = data.fAverage;
			trend.data.fCurr = data.fCurr;
			trend.data.fMax = data.fMax;
			trend.data.fMin = data.fMin;
			trend.data.fSumMin = data.fSumMin;
			trend.crc = trend.CalcCRC();
            
			trend.Write(wr);
			
			wr.Close();

			return true;
		}

		static public bool SaveMinDataStructAI(string tag, DateTime t, TREND_AI_STRUCT data, bool flag)
		{
			return SaveMinDataStructAI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data, flag);
		}


		static public bool SaveMinDataStructDI(string tag, int year, int month, int day, int hour, int minute, TREND_DI_STRUCT data, bool flag)
		{
			string filename;
			string tag_file;
			string data_dir;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

			filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\DI", data_dir, year, month);

			if(!Directory.Exists(filename)) 
			{
				Directory.CreateDirectory(filename);
			}

			filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, month, tag_file);

			FileStream fs;

			if(!File.Exists(filename)) 
			{
				fs = File.OpenWrite(filename);
				if(fs == null) 	return false;	// 자료 없음
				BinaryWriter bw = new BinaryWriter(fs);
				
				byte[] imsi = new byte[25];

				for(int i = 0; i < 9; i++)		imsi[i] = 0;

				for(int i = 0; i < 60*24*31; i++) 
				{
					bw.Write(imsi, 0, 9);
				}
				bw.Close();
			}

			fs = File.OpenWrite(filename);
			if(fs == null) 	return false;	// 자료 없음

			BinaryWriter wr = new BinaryWriter(fs);

			long start = 9;
			start = start*((day-1)*1440+hour*60L+minute);

			fs.Seek(start, SeekOrigin.Begin);

			FILE_TREND_DI_STRUCT trend = new FILE_TREND_DI_STRUCT();

			if(flag == false) trend.day = 0;		// 2005-6-29 add by kdy 자료삭제를 위해
			else			  trend.day = (byte)day;
			trend.hour = (byte)hour;
			trend.min = (byte)minute;
			trend.data.bOnOff = data.bOnOff;
			trend.data.cOnTime = data.cOnTime;
			trend.data.nCountOnOff = data.nCountOnOff;
			trend.crc = trend.CalcCRC();
            
			trend.Write(wr);
			
			wr.Close();

			return true;
		}

		static public bool SaveMinDataStructDI(string tag, DateTime t, TREND_DI_STRUCT data, bool flag)
		{
			return SaveMinDataStructDI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data, flag);
		}

		static public bool SaveHourDataStructAI(string tag, int year, int month, int day, int hour, int minute, HOUR_DATA_ANALOG_STRUCT data)
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
			data.flag = 1;

			data.crc = data.CalcCRC();

			data.Write(wr);
			
			wr.Close();

			return true;
		}

		static public bool SaveHourDataStructAI(string tag, DateTime t, HOUR_DATA_ANALOG_STRUCT data)
		{
			return SaveHourDataStructAI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data);
		}


		static public bool SaveHourDataStructDI(string tag, int year, int month, int day, int hour, int minute, HOUR_DATA_DIGITAL_STRUCT data)
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
			data.flag = 1;

			data.crc = data.CalcCRC();

			data.Write(wr);
			
			wr.Close();

			return true;
		}

		static public bool SaveHourDataStructDI(string tag, DateTime t, HOUR_DATA_DIGITAL_STRUCT data)
		{
			return SaveHourDataStructDI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data);
		}
		*/
        #endregion FileSystem


        static public async Task<bool> SaveMinDataStructAI(string tag, DateTime t, TREND_AI_STRUCT data, bool flag)
        {
			if (flag == false) return true;

			return await DataPostgres.Instance.SaveMinDataAI(tag, t, data);
        }

        static public async Task< bool> SaveMinDataStructDI(string tag, DateTime t, TREND_DI_STRUCT data, bool flag)
        {
            if (flag == false) return true;

            return await DataPostgres.Instance.SaveMinDataDI(tag, t, data);
        }

        static public async Task<bool> SaveHourDataStructAI(string tag, DateTime t, HOUR_DATA_ANALOG_STRUCT data)
        {
			if(data.flag == 0) return true;

            return await DataPostgres.Instance.SaveHourDataAI(tag, t, data);
        }

        static public async Task<bool> SaveHourDataStructDI(string tag, DateTime t, HOUR_DATA_DIGITAL_STRUCT data)
        {
            if (data.flag == 0) return true;

            return await DataPostgres.Instance.SaveHourDataDI(tag, t, data);
        }


    }
}



