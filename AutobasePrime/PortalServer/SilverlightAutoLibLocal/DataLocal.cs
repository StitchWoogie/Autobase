using System;
using System.Collections;
using NetTools;
using System.IO;

namespace AutoLibLocal
{
	[Flags]
	public enum EnumDataType
	{	// 아날로그 자료의 형태.
		AVE    = 0x0001,
		MIN    = 0x0002,
		MAX    = 0x0004,
		SUM    = 0x0008,
		SUB    = 0x0010,
		CURR   = 0x0020,
		MOMENT = 0x0040,		// 순시값

		ONTIME = 0x0080,	// On된 시간.
		OFFTIME= 0x0100,	// Off된 시간.
		COUNT  = 0x0200,	// on/off count

		AveMinMaxSum = AVE | MIN | MAX | SUM,
		CountOntime = COUNT | ONTIME,
	}

	public enum EnumDataTime : int
	{	// 아날로그 자료의 형태.
		Minute = 0,	// 분자료
		Hour   = 1,	// 분자료
		Day    = 2,	// 분자료
		Month  = 3,	// 분자료
		Week   = 4,	// 주자료
	}

	public class TREND_AI_STRUCT
	{
		public float		fSumMin;	// 1분 동안에 흘렀을 유량 (실제 적산치)
		public float		fAverage;	// 1분 동안에 계측된 값의 평균 (적산 평균 아님)
		public float		fMin;		// 1분 동안의 최소값
		public float		fMax;		// 1분 동안의 최소값
		public float		fCurr;		// 저장 당시의 현재값.

		public static	readonly int struct_size = 20;
	}

	public class FILE_TREND_AI_STRUCT
	{
		public 	byte		day;			// 일
		public 	byte		hour;			// 월
		public 	byte		min;			// 분
		public 	TREND_AI_STRUCT data = new TREND_AI_STRUCT();
		public 	ushort		crc;			// data 의 crc

		byte[] SingleToBytes(float val)
		{
			byte[] b = new byte[4];

			MemoryStream ms = new MemoryStream(b);
			BinaryWriter br = new BinaryWriter(ms);
			br.Write(val);
			br.Close();	
			return b;
		}

		public ushort CalcCRC()
		{
			ushort c;
			c = 0;

			c += day;
			c += hour;
			c += min;

			int i;
            byte[] b;
			b = SingleToBytes(data.fAverage);
			for(i = 0; i < 4; i++)	c += b[i];
			b = SingleToBytes(data.fCurr);
			for(i = 0; i < 4; i++)	c += b[i];
			b = SingleToBytes(data.fMax);
			for(i = 0; i < 4; i++)	c += b[i];
			b = SingleToBytes(data.fMin);
			for(i = 0; i < 4; i++)	c += b[i];
			b = SingleToBytes(data.fSumMin);
			for(i = 0; i < 4; i++)	c += b[i];

			return c;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(day);
			writer.Write(hour);
			writer.Write(min);
			writer.Write(data.fSumMin);
			writer.Write(data.fAverage);
			writer.Write(data.fMin);
			writer.Write(data.fMax);
			writer.Write(data.fCurr);
			writer.Write(crc);
		}

		public void Read(BinaryReader reader)
		{
			day = reader.ReadByte();
			hour = reader.ReadByte();
			min  = reader.ReadByte();
			data.fSumMin = reader.ReadSingle();
			data.fAverage = reader.ReadSingle();
			data.fMin = reader.ReadSingle();
			data.fMax = reader.ReadSingle();
			data.fCurr = reader.ReadSingle();
			crc = reader.ReadUInt16();
		}

		public static readonly int struct_size = TREND_AI_STRUCT.struct_size+5;
	}

	public class HOUR_DATA_ANALOG_STRUCT
	{
		public float		fSumHour;		// 한 시간 동안에 흘렀을 적산치
		public float		fAveHour;		// 한 시간 동안의 계측 평균치 (적산평균치 아님)
		public float		fMinHour;		// 한 시간 동안의 최소치
		public float		fMaxHour;		// 한 시간 동안의 최고치
		public float		fCurrSumMeter;	// 시간대 마지막에 계측된 적산 계량기 눈금
		public byte			flag;			// OFF이면 파일 초기화만 되어 있고 저장되지는 않았다.
		public ushort		crc;

		public static readonly int struct_size = 23;

		byte[] SingleToBytes(float val)
		{
			byte[] b = new byte[4];

			MemoryStream ms = new MemoryStream(b);
			BinaryWriter br = new BinaryWriter(ms);
			br.Write(val);
			br.Close();	
			return b;
		}

		public ushort CalcCRC()
		{
			ushort c = 0;

			int i;
			byte[] b;
			b = SingleToBytes(fSumHour);
			for(i = 0; i < 4; i++)	c += b[i];
			b = SingleToBytes(fAveHour);
			for(i = 0; i < 4; i++)	c += b[i];
			b = SingleToBytes(fMinHour);
			for(i = 0; i < 4; i++)	c += b[i];
			b = SingleToBytes(fMaxHour);
			for(i = 0; i < 4; i++)	c += b[i];
			b = SingleToBytes(fCurrSumMeter);
			for(i = 0; i < 4; i++)	c += b[i];
			c += flag;

			return c;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(fSumHour);
			writer.Write(fAveHour);
			writer.Write(fMinHour);
			writer.Write(fMaxHour);
			writer.Write(fCurrSumMeter);
			writer.Write(flag);
			writer.Write(crc);
		}
	}

	public class HOUR_DATA_DIGITAL_STRUCT
	{
		public ushort	wCountOnOff;	// 한 시간동안 ON/OFF 된 횟수.
		public uint		dwOnTime;		// 접점이 ON 된 시간 (단위 sec)
		public byte		flag;			// OFF이면 파일 초기화만 되어 있고 저장되지는 않았다.
		public ushort	crc;

		public static readonly int struct_size = 9;

		public ushort CalcCRC()
		{
			ushort c = 0;

			c += (ushort)((wCountOnOff >> 8) & 0xFF);
			c += (ushort)((wCountOnOff >> 0) & 0xFF);
			c += (ushort)((dwOnTime >> 24) & 0xFF);
			c += (ushort)((dwOnTime >> 16) & 0xFF);
			c += (ushort)((dwOnTime >> 8) & 0xFF);
			c += (ushort)((dwOnTime >> 0) & 0xFF);
			c += flag;

			return c;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(wCountOnOff);
			writer.Write(dwOnTime);
			writer.Write(flag);
			writer.Write(crc);
		}
	}


	public class HOUR_DATA_HEAD 
	{
		public byte[]	id = new byte[5];			// HOUR
		public short	version;		// 1
		public byte[]  extra = new byte[11];		// 이전에는 태그이름으로 사용했으나 40자로 증가되면서 무의미한 배열이 됨.

		public static readonly int struct_size = 18;

		public void Write(BinaryWriter writer)
		{
			writer.Write(id);
			writer.Write(version);
			writer.Write(extra);
		}
	}

	public class TREND_DI_STRUCT 
	{
		public short	nCountOnOff;	// 1분동안 접점이 ON/OFF 된 횟수.
		public byte		bOnOff;			// 자료 저장 시의 ON/OFF 상태
		public byte		cOnTime;		// 1분동안 접점이 ON 된 시간(단위 sec)

		public static	readonly int struct_size = 4;
	}

	public class FILE_TREND_DI_STRUCT 
	{
		public byte		day;		// 일
		public byte		hour;		// 월
		public byte		min;		// 분
		public TREND_DI_STRUCT data = new TREND_DI_STRUCT();
		public ushort	crc;		// data 의 crc

		public static	readonly int struct_size = TREND_DI_STRUCT.struct_size+5;

		public void Write(BinaryWriter writer)
		{
			writer.Write(day);
			writer.Write(hour);
			writer.Write(min);
			writer.Write(data.nCountOnOff);
			writer.Write(data.bOnOff);
			writer.Write(data.cOnTime);
			writer.Write(crc);
		}

		public void Read(BinaryReader reader)
		{
			day = reader.ReadByte();
			hour = reader.ReadByte();
			min = reader.ReadByte();
			data.nCountOnOff = reader.ReadInt16();
			data.bOnOff = reader.ReadByte();
			data.cOnTime = reader.ReadByte();
			crc = reader.ReadUInt16();
		}

		public ushort CalcCRC()
		{
			ushort c = 0;

			c += day;
			c += hour;
			c += min;

			c += (ushort)((data.nCountOnOff)/256);
			c += (ushort)((data.nCountOnOff)%256);
			c += data.bOnOff;
			c += data.cOnTime;

			return c;
		}
	}

    /*
	/// <summary>
	/// Summary description for DataLocal.
	/// </summary>
	public class DataLocal
	{
		public DataLocal()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public DataSet GetTagValueList(ArrayList array)
		{
			DataSet ds = new DataSet("TAG");

			string tag;

			DataTable dt = new DataTable("TAG");
			DataColumn dc;

			dc = new DataColumn("tag", Type.GetType("System.String"));
			dc.MaxLength = 80;
			dt.Columns.Add(dc);
			dc = new DataColumn("curr", Type.GetType("System.String"));
			dc.MaxLength = 256;
			dt.Columns.Add(dc);

			DataRow row;

			string curr="";
			bool retn;

			for(int i = 0; i < array.Count; i++) 
			{
				tag = (string)array[i];

				row = dt.NewRow();
				row[0] = tag;
				curr = "";
				retn = SharedTag.GetCurr(tag, ref curr);
				row[1] = curr;
				dt.Rows.Add(row);
			}

			ds.Tables.Add(dt);

			return ds;
		}

		public void WriteCurrAI(string tag, double val)
		{
			SharedTag.SetCurr(tag, val);
		}

		public void WriteCurrST(string tag, string val)
		{
			SharedTag.SetCurr(tag, val);
		}

		public void WriteCurr(string tag, object val, int delay_sec)
		{
			SharedTag.SetCurrDelaySec(tag, val, delay_sec);
		}

		bool CatDataGetAi(string tag, ref double val, EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min)
		{
			bool retn;

			if(data_time == EnumDataTime.Minute)
				retn = CatDataGetAiMin(0, tag, ref val, year, mon, day, hour, min, data_type);
			else if(data_time == EnumDataTime.Hour)
				retn = CatDataGetAiHour(0, tag, ref val, year, mon, day, hour, min, data_type);
			else if(data_time == EnumDataTime.Day)
				retn = CatDataGetAiDay(0, tag, ref val, year, mon, day, hour, min, data_type);
			else if(data_time == EnumDataTime.Week)
				retn = CatDataGetAiWeek(0, tag, ref val, year, mon, data_type);
			else if(data_time == EnumDataTime.Month)
				retn = CatDataGetAiMonth(0, tag, ref val, year, mon, day, hour, min, data_type);
			else
				retn = false;

			if(!retn)	val = 0;

			return retn;
		}

		public DataSet GetDataAi(string tag, EnumDataType value_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
		{
			DataSet ds = new DataSet();
			DataTable dt;

			dt = new DataTable(tag);
			
			DataColumn dc;
			DataRow row;
			double val = 0;
			bool retn;

			dc = new DataColumn("Flag", Type.GetType("System.SByte"));
			dt.Columns.Add(dc);

			if((value_type & EnumDataType.AVE) == EnumDataType.AVE) 
			{
				dc = new DataColumn("AVE");
				dt.Columns.Add(dc);
			}

			if((value_type & EnumDataType.MAX) == EnumDataType.MAX) 
			{
				dc = new DataColumn("MAX");
				dt.Columns.Add(dc);
			}

			if((value_type & EnumDataType.MIN) == EnumDataType.MIN) 
			{
				dc = new DataColumn("MIN");
				dt.Columns.Add(dc);
			}

			if((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT) 
			{
				dc = new DataColumn("MOMENT");
				dt.Columns.Add(dc);
			}

			if((value_type & EnumDataType.SUB) == EnumDataType.SUB) 
			{
				dc = new DataColumn("SUB");
				dt.Columns.Add(dc);
			}

			if((value_type & EnumDataType.SUM) == EnumDataType.SUM) 
			{
				dc = new DataColumn("SUM");
				dt.Columns.Add(dc);
			}

			for(int i = 0; i < data_count; i++)
			{
				row = dt.NewRow();

				retn = false;

				if((value_type & EnumDataType.AVE) == EnumDataType.AVE) 
				{
					retn = CatDataGetAi(tag, ref val, EnumDataType.AVE, data_time, year, mon, day, hour, min);
					row["AVE"]  = val.ToString();
				}
				if((value_type & EnumDataType.MAX) == EnumDataType.MAX) 
				{
					retn = CatDataGetAi(tag, ref val, EnumDataType.MAX, data_time, year, mon, day, hour, min);
					row["MAX"]  = val.ToString();
				}
				if((value_type & EnumDataType.MIN) == EnumDataType.MIN) 
				{
					retn = CatDataGetAi(tag, ref val, EnumDataType.MIN, data_time, year, mon, day, hour, min);
					row["MIN"]  = val.ToString();
				}
				if((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT) 
				{
					retn = CatDataGetAi(tag, ref val, EnumDataType.MOMENT, data_time, year, mon, day, hour, min);
					row["MOMENT"]  = val.ToString();
				}
				if((value_type & EnumDataType.SUB) == EnumDataType.SUB) 
				{
					retn = CatDataGetAi(tag, ref val, EnumDataType.SUB, data_time, year, mon, day, hour, min);
					row["SUB"]  = val.ToString();
				}
				if((value_type & EnumDataType.SUM) == EnumDataType.SUM) 
				{
					retn = CatDataGetAi(tag, ref val, EnumDataType.SUM, data_time, year, mon, day, hour, min);
					row["SUM"]  = val.ToString();
				}

				if(retn)
				{
					row["Flag"] = 1;
				}
				else 
				{
					row["Flag"] = 0;
				}

				dt.Rows.Add(row);

				for(int t = 0; t < data_gab; t++) 
				{ 
					if(data_time == EnumDataTime.Minute)
						TimeUtil.PlusMin(ref year, ref mon, ref day, ref hour, ref min);
					else if(data_time == EnumDataTime.Hour)
						TimeUtil.PlusHour(ref year, ref mon, ref day, ref hour);
					else if(data_time == EnumDataTime.Day)
						TimeUtil.PlusDay(ref year, ref mon, ref day);
					else if(data_time == EnumDataTime.Week)
						mon++;
					else if(data_time == EnumDataTime.Month)
						TimeUtil.PlusMonth(ref year, ref mon);
				}
			}

			ds.Tables.Add(dt);

			return ds;
		}

		public bool LoadMinDataStructAI(string tag, int year, int mon, int day, int hour, int min, TREND_AI_STRUCT data)
		{
			if(day < 1 || day > 31)		return false;
			if(hour < 0 || hour > 23)	return false;
			if(min < 0 || min > 59)		return false;

			string filename;
			string tag_file;
			string data_dir;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
			filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, mon, tag_file);

			FileStream fs;

			FILE_TREND_AI_STRUCT trend = new FILE_TREND_AI_STRUCT();

			if(!File.Exists(filename))	return false;
			fs = File.OpenRead(filename);
			if(fs == null) 	return false;	// 자료 없음

			BinaryReader br = new BinaryReader(fs);

			long start = 25;
			start = start*((day-1)*1440+hour*60L+min);

			fs.Seek(start, SeekOrigin.Begin);

			trend.day = br.ReadByte();
			trend.hour = br.ReadByte();
			trend.min = br.ReadByte();
			data.fSumMin = br.ReadSingle();
			data.fAverage = br.ReadSingle();
			data.fMin = br.ReadSingle();
			data.fMax = br.ReadSingle();
			data.fCurr = br.ReadSingle();
			trend.crc = br.ReadUInt16();
			
			fs.Close();

			if(trend.day != day)						return false;
			if(trend.hour != hour)						return false;
			if(trend.min != min)						return false;

			return true;
		}

		public bool LoadMinDataStructAI(string tag, DateTime t, TREND_AI_STRUCT data)
		{
			return LoadMinDataStructAI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data);
		}

		

		public bool CatDataGetAiMin(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
		{
			TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

			val = 0.0;

			if(!LoadMinDataStructAI(tag, year, month, day, hour, min, trend)) 
			{
				return false;
			}

			if(data_type == EnumDataType.AVE)
			{
				val = trend.fAverage;
			}
			else if(data_type == EnumDataType.SUM) 
			{
				val = trend.fSumMin;
			}
			else if(data_type == EnumDataType.MIN) 
			{
				val = trend.fMin;
			}
			else if(data_type == EnumDataType.MAX) 
			{
				val = trend.fMax;
			}
			else if(data_type == EnumDataType.MOMENT) 
			{
				val = trend.fCurr;
			}
			else if(data_type == EnumDataType.SUB) 
			{
				double value1;

				value1 = trend.fMax;

				TimeUtil.MinusMin(ref year, ref month, ref day, ref hour, ref min);
				if(!LoadMinDataStructAI(tag, year, month, day, hour, min, trend)) 
				{
					return false;
				}

				if(value1 < trend.fMax)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - trend.fMax;
				else
					val = value1-trend.fMax;
			}
			else 
			{
				return false;
			}

			return true;
		}

		bool CatDataGetDi(string tag, ref uint val, EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min)
		{
			bool retn;

			if(data_time == EnumDataTime.Minute)
				retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, data_type);
			else if(data_time == EnumDataTime.Hour)
				retn = CatDataGetDiHour(0, tag, ref val, year, mon, day, hour, data_type);
			else if(data_time == EnumDataTime.Day)
				retn = CatDataGetDiDay(0, tag, ref val, year, mon, day, data_type);
			else if(data_time == EnumDataTime.Week)
				retn = CatDataGetDiWeek(0, tag, ref val, year, mon, data_type);
			else if(data_time == EnumDataTime.Month)
				retn = CatDataGetDiMonth(0, tag, ref val, year, mon, data_type);
			else
				retn = false;

			if(!retn)	val = 0;

			return retn;
		}

		public DataSet GetDataDi(string tag, EnumDataType value_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
		{
			DataSet ds = new DataSet();
			DataTable dt;

			dt = new DataTable(tag);
			
			DataColumn dc;
			DataRow row;
			uint val=0;
			bool retn;

			dc = new DataColumn("Flag", Type.GetType("System.SByte"));
			dt.Columns.Add(dc);

			if((value_type & EnumDataType.COUNT) == EnumDataType.COUNT) 
			{
				dc = new DataColumn("COUNT");
				dt.Columns.Add(dc);
			}
			if((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT) 
			{
				dc = new DataColumn("MOMENT");
				dt.Columns.Add(dc);
			}
			if((value_type & EnumDataType.OFFTIME) == EnumDataType.OFFTIME) 
			{
				dc = new DataColumn("OFFTIME");
				dt.Columns.Add(dc);
			}
			if((value_type & EnumDataType.ONTIME) == EnumDataType.ONTIME) 
			{
				dc = new DataColumn("ONTIME");
				dt.Columns.Add(dc);
			}

			for(int i = 0; i < data_count; i++)
			{
				row = dt.NewRow();

				retn = false;

				if((value_type & EnumDataType.COUNT) == EnumDataType.COUNT) 
				{
					retn = CatDataGetDi(tag, ref val, EnumDataType.COUNT, data_time, year, mon, day, hour, min);
					row["COUNT"]  = val.ToString();
				}
				if((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT) 
				{
					if(data_time == EnumDataTime.Minute)
						retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, EnumDataType.MOMENT);
					else if(data_time == EnumDataTime.Hour)
						retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, EnumDataType.MOMENT);
					else if(data_time == EnumDataTime.Day)
						retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, EnumDataType.MOMENT);
					else if(data_time == EnumDataTime.Month)
						retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, EnumDataType.MOMENT);
					else
						retn = false;

					if(!retn)	val = 0;
					row["MOMENT"]  = val.ToString();
				}
				if((value_type & EnumDataType.OFFTIME) == EnumDataType.OFFTIME)
				{
					retn = CatDataGetDi(tag, ref val, EnumDataType.OFFTIME, data_time, year, mon, day, hour, min);
					row["OFFTIME"]  = val.ToString();
				}
				if((value_type & EnumDataType.ONTIME) == EnumDataType.ONTIME) 
				{
					retn = CatDataGetDi(tag, ref val, EnumDataType.ONTIME, data_time, year, mon, day, hour, min);
					row["ONTIME"]  = val.ToString();
				}

				if(retn)
				{
					row["Flag"] = 1;
				}
				else 
				{
					row["Flag"] = 0;
				}

				dt.Rows.Add(row);

				for(int t = 0; t < data_gab; t++) 
				{ 
					if(data_time == EnumDataTime.Minute)
						TimeUtil.PlusMin(ref year, ref mon, ref day, ref hour, ref min);
					else if(data_time == EnumDataTime.Hour)
						TimeUtil.PlusHour(ref year, ref mon, ref day, ref hour);
					else if(data_time == EnumDataTime.Day)
						TimeUtil.PlusDay(ref year, ref mon, ref day);
					else if(data_time == EnumDataTime.Week)
						mon++;
					else if(data_time == EnumDataTime.Month)
						TimeUtil.PlusMonth(ref year, ref mon);
				}
			}

			ds.Tables.Add(dt);

			return ds;
		}

		public bool CatDataGetDiMin(int terminal, string tag, ref uint val, int year, int month, int day, int hour, int min, EnumDataType data_type)
		{
			TREND_DI_STRUCT trend = new TREND_DI_STRUCT();

			val = 0;

			if(!LoadMinDataStructDI(tag, year, month, day, hour, min, trend)) 
			{
				return false;
			}

			if(data_type == EnumDataType.ONTIME) 
			{
				val = trend.cOnTime;
			}
			else if(data_type == EnumDataType.OFFTIME) 
			{
				val = (uint)(60-trend.cOnTime);
			}
			else if(data_type == EnumDataType.COUNT) 
			{
				val = (uint)trend.nCountOnOff;
			}
			else if(data_type == EnumDataType.MOMENT) 
			{
				val = (uint)trend.bOnOff;
			}
			else 
			{
				return false;
			}

			return true;
		}

		public bool LoadMinDataStructDI(string tag, int year, int mon, int day, int hour, int min, TREND_DI_STRUCT data)
		{
			if(day < 1 || day > 31)		return false;
			if(hour < 0 || hour > 23)	return false;
			if(min < 0 || min > 59)		return false;

			string filename;
			string tag_file;
			string data_dir;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
			filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, mon, tag_file);

			FileStream fs;

			FILE_TREND_DI_STRUCT trend = new FILE_TREND_DI_STRUCT();

			if(!File.Exists(filename))	return false;
			fs = File.OpenRead(filename);
			if(fs == null) 	return false;	// 자료 없음

			BinaryReader br = new BinaryReader(fs);

			long start = 9;
			start = start*((day-1)*1440+hour*60L+min);

			fs.Seek(start, SeekOrigin.Begin);

			trend.day = br.ReadByte();
			trend.hour = br.ReadByte();
			trend.min = br.ReadByte();
			data.nCountOnOff = br.ReadInt16();
			data.bOnOff = br.ReadByte();
			data.cOnTime = br.ReadByte();
			trend.crc = br.ReadUInt16();
			
			fs.Close();

			if(trend.day != day)						return false;
			if(trend.hour != hour)						return false;
			if(trend.min != min)						return false;


			return true;
		}

		public bool LoadMinDataStructDI(string tag, DateTime t, TREND_DI_STRUCT data)
		{
			return LoadMinDataStructDI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data);
		}

		public bool LoadHourDataStructAI(string tag, int year, int month, int day, int hour, HOUR_DATA_ANALOG_STRUCT data)
		{
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;

			string tag_file;
			string data_dir;
			string filename;


			FileStream fs;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
			filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, month, tag_file);

			if(!File.Exists(filename))	return false;
			fs = File.OpenRead(filename);
			if(fs == null)			return false;

			if(fs.Length != 18+HOUR_DATA_ANALOG_STRUCT.struct_size*24*31) 
			{
				fs.Close();
				return false;
			}

			BinaryReader br = new BinaryReader(fs);

			fs.Seek(18+((day-1)*24L+hour)*23, SeekOrigin.Begin);

			data.fSumHour = br.ReadSingle();
			data.fAveHour = br.ReadSingle();
			data.fMinHour = br.ReadSingle();
			data.fMaxHour = br.ReadSingle();
			data.fCurrSumMeter = br.ReadSingle();
			data.flag = br.ReadByte();
			data.crc = br.ReadUInt16();

			fs.Close();

			if(data.flag == 0)
			{
				return false;	// 자료는 있으나 초기화 되어있는 값이다.
			}

			return true;
		}

		public bool LoadHourDataStructAI(string tag, DateTime t, HOUR_DATA_ANALOG_STRUCT data)
		{
			return LoadHourDataStructAI(tag, t.Year, t.Month, t.Day, t.Hour, data);
		}

		//------------------------------------------------------------------------------
		//	아날로그 시간 데이터를 읽어온다.
		//------------------------------------------------------------------------------

		bool DataGetAiHourElse(int terminal, string tag, ref double val, int year, int month, int day, int hour, EnumDataType data_type)
		{
			// HOUR_DATA_HEAD head; size = 18
			HOUR_DATA_ANALOG_STRUCT data = new HOUR_DATA_ANALOG_STRUCT();

			val = 0.0;

			if(!LoadHourDataStructAI(tag, year, month, day, hour, data))	return false; 


			if(data_type == EnumDataType.AVE) 
			{
				val = data.fAveHour;
			}
			else if(data_type == EnumDataType.SUM) 
			{
				val = data.fSumHour;
			}
			else if(data_type == EnumDataType.MIN) 
			{
				val = data.fMinHour;
			}
			else if(data_type == EnumDataType.MAX) 
			{
				val = data.fMaxHour;
			}
			else 
			{
				return false;
			}

			return true;
		}

		public bool CatDataGetAiHour(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
		{
			if(data_type == EnumDataType.SUB) 
			{
				double value1 = 0;
				double value2 = 0;

				if(!DataGetAiHourElse(terminal, tag, ref value1, year, month, day, hour, EnumDataType.MAX))
					return false;

				TimeUtil.MinusHour(ref year, ref month, ref day, ref hour);

				if(!DataGetAiHourElse(terminal, tag, ref value2, year, month, day, hour, EnumDataType.MAX))
					return false;

				if(value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
					val = TagLib.GetTagMemberFull(tag)+value1-value2;
				else
					val = value1-value2;

				return true;
			}
			else if(data_type == EnumDataType.MOMENT) 
			{
				TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

				val = 0.0;

				if(!LoadMinDataStructAI(tag, year, month, day, hour, min, trend)) 
					return false;
				val = trend.fCurr;
				return true;
			}
			else 
			{
				return DataGetAiHourElse(terminal, tag, ref val, year, month, day, hour, data_type);
			}
		}

		bool DataGetAiDayElse(int terminal, string tag, ref double val, int year, int month, int day, EnumDataType data_type)
		{
			if(day < 1 || day > 31)	return false;

			string tag_file;
			string data_dir;
			string filename;


			HOUR_DATA_ANALOG_STRUCT data = new HOUR_DATA_ANALOG_STRUCT();
			FileStream fs;

			val = 0.0;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
			filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, month, tag_file);

			if(!File.Exists(filename))	return false;
			fs = File.OpenRead(filename);
			if(fs == null)			return false;

			if(fs.Length != 18+HOUR_DATA_ANALOG_STRUCT.struct_size*24*31) 
			{
				fs.Close();
				return false;
			}

			BinaryReader br = new BinaryReader(fs);

			bool read_flag = false;
			int  read_count = 0;
			int  i;

			//fs.Seek(18+((day-1)*24L+hour)*23, SeekOrigin.Begin);

			fs.Seek(18+((day-1)*24L)*23, SeekOrigin.Begin);

			for(i = 0; i < 24; i++) 
			{
				data.fSumHour = br.ReadSingle();
				data.fAveHour = br.ReadSingle();
				data.fMinHour = br.ReadSingle();
				data.fMaxHour = br.ReadSingle();
				data.fCurrSumMeter = br.ReadSingle();
				data.flag = br.ReadByte();
				data.crc = br.ReadUInt16();
				
				if(data.flag == 1) 
				{
					read_flag = true;
					read_count ++;

					if(data_type == EnumDataType.AVE) 
					{
						val += data.fAveHour;
					}
					else if(data_type == EnumDataType.SUM) 
					{
						val += data.fSumHour;
					}
					else if(data_type == EnumDataType.MIN) 
					{
						if(read_count == 1) 
						{	// 처음으로 읽을때
							val = data.fMinHour;
						}
						else 
						{
							if(data.fMinHour < val)	val = data.fMinHour;
						}
					}
					else if(data_type == EnumDataType.MAX) 
					{
						if(read_count == 1) 
						{	// 처음으로 읽을때
							val = data.fMaxHour;
						}
						else 
						{
							if(data.fMaxHour > val)	val = data.fMaxHour;
						}
					}
					else 
					{
						fs.Close();
						return false;
					}
				}
			}

			fs.Close();

			if(read_flag == false) 
			{
				return false;	// 읽은 데이터가 없다.
			}

			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			return true;
		}

		public bool CatDataGetAiDay(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
		{
			val = 0.0;

			if(data_type == EnumDataType.SUB) 
			{
				double value1=0;
				double value2=0;

				if(!DataGetAiDayElse(terminal, tag, ref value1, year, month, day, EnumDataType.MAX))
					return false;
				TimeUtil.MinusDay(ref year, ref month, ref day);
				if(!DataGetAiDayElse(terminal, tag, ref value2, year, month, day, EnumDataType.MAX))
					return false;

				if(value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - value2;
				else
					val = value1-value2;

				return true;
			}
			else if(data_type == EnumDataType.MOMENT) 
			{
				TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

				val = 0.0;

				if(!LoadMinDataStructAI(tag, year, month, day, hour, min, trend)) 
					return false;
				val = trend.fCurr;
				return true;
			}
			else 
			{
				return DataGetAiDayElse(terminal, tag, ref val, year, month, day, data_type);
			}
		}

		bool CatDataGetAiWeek(int terminal, string tag, ref double val, int year, int week, EnumDataType data_type)
		{
			bool read_flag = false;
			int  read_count = 0;

			double day_value = 0;
			int fr_year;
			int fr_mon;
			int fr_day;
			int i;

			GetDayFromWeek(year, week, out fr_year, out fr_mon, out fr_day);
            
			val = 0.0;

			for(i = 0; i < 7; i++) 
			{	// 한달의 데이터를 모두 읽는다.
				if(DataGetAiDayElse(terminal, tag, ref day_value, fr_year, fr_mon, fr_day, data_type)) 
				{
					read_flag = true;
					read_count ++;

					if(data_type == EnumDataType.AVE) 
					{
						val += day_value;
					}
					else if(data_type == EnumDataType.SUM) 
					{
						val += day_value;
					}
					else if(data_type == EnumDataType.MIN) 
					{
						if(read_count == 1) 
						{	// 처음으로 읽을때
							val = day_value;
						}
						else 
						{
							if(day_value < val)	val = day_value;
						}
					}
					else if(data_type == EnumDataType.MAX) 
					{
						if(read_count == 1) 
						{	// 처음으로 읽을때
							val = day_value;
						}
						else 
						{
							if(day_value > val)	val = day_value;
						}
					}
					else 
					{
						return false;
					}
				}

				TimeUtil.PlusDay(ref fr_year, ref fr_mon, ref fr_day);
			}

			if(read_flag == false) 
			{
				return false;	// 읽은 데이터가 없다.
			}

			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			return true;
		}

		bool DataGetAiMonthElse(int terminal, string tag, ref double val, int year, int month, EnumDataType data_type)
		{
			bool read_flag = false;
			int  read_count = 0;
			int  day;
			//float min = 0;
			//float max = 0;
			double day_value = 0;

			val = 0.0;

			for(day = 1; day <= 31; day++) 
			{	// 한달의 데이터를 모두 읽는다.
				if(!DataGetAiDayElse(terminal, tag, ref day_value, year, month, day, data_type))	continue;

				read_flag = true;
				read_count ++;

				if(data_type == EnumDataType.AVE) 
				{
					val += day_value;
				}
				else if(data_type == EnumDataType.SUM) 
				{
					val += day_value;
				}
				else if(data_type == EnumDataType.MIN) 
				{
					if(read_count == 1) 
					{	// 처음으로 읽을때
						val = day_value;
					}
					else 
					{
						if(day_value < val)	val = day_value;
					}
				}
				else if(data_type == EnumDataType.MAX) 
				{
					if(read_count == 1) 
					{	// 처음으로 읽을때
						val = day_value;
					}
					else 
					{
						if(day_value > val)	val = day_value;
					}
				}
				else 
				{
					return false;
				}
			}

			if(read_flag == false) 
			{
				return false;	// 읽은 데이터가 없다.
			}

			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			return true;
		}

		public bool CatDataGetAiMonth(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
		{
			val = 0.0;

			if(data_type == EnumDataType.SUB) 
			{
				double value1 = 0;
				double value2 = 0;

				if(!DataGetAiMonthElse(terminal, tag, ref value1, year, month, EnumDataType.MAX))
					return false;
				TimeUtil.MinusMonth(ref year, ref month);
				if(!DataGetAiMonthElse(terminal, tag, ref value2, year, month, EnumDataType.MAX))
					return false;

				if(value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - value2;
				else
					val = value1-value2;

				return true;
			}
			else if(data_type == EnumDataType.MOMENT) 
			{
				TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

				val = 0.0;

				if(!LoadMinDataStructAI(tag, year, month, day, hour, min, trend)) 
					return false;
				val = trend.fCurr;
				return true;
			}
			else 
			{
				return DataGetAiMonthElse(terminal, tag, ref val, year, month, data_type);
			}
		}


		bool DataGetAiYearElse(int terminal, string tag, ref double val, int year, EnumDataType data_type)
		{
			bool read_flag = false;
			int  read_count = 0;
			int  month;

			double day_value = 0;

			val = 0.0;

			for(month = 1; month <= 12; month++) 
			{	// 한달의 데이터를 모두 읽는다.
				if(!DataGetAiMonthElse(terminal, tag, ref day_value, year, month, data_type))	continue;

				read_flag = true;
				read_count ++;

				if(data_type == EnumDataType.AVE) 
				{
					val += day_value;
				}
				else if(data_type == EnumDataType.SUM) 
				{
					val += day_value;
				}
				else if(data_type == EnumDataType.MIN) 
				{
					if(read_count == 1) 
					{	// 처음으로 읽을때
						val = day_value;
					}
					else 
					{
						if(day_value < val)	val = day_value;
					}
				}
				else if(data_type == EnumDataType.MAX) 
				{
					if(read_count == 1) 
					{	// 처음으로 읽을때
						val = day_value;
					}
					else 
					{
						if(day_value > val)	val = day_value;
					}
				}
				else 
				{
					return false;
				}
			}

			if(read_flag == false) 
			{
				return false;	// 읽은 데이터가 없다.
			}

			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			return true;
		}

		public bool CatDataGetAiYear(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
		{
			val = 0.0;

			if(data_type == EnumDataType.SUB) 
			{
				double value1 = 0;
				double value2 = 0;

				if(!DataGetAiYearElse(terminal, tag, ref value1, year, EnumDataType.MAX))
					return false;
				TimeUtil.MinusYear(ref year);
				if(!DataGetAiYearElse(terminal, tag, ref value2, year, EnumDataType.MAX))
					return false;

				if(value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - value2;
				else
					val = value1-value2;

				return true;
			}
			else if(data_type == EnumDataType.MOMENT) 
			{
				TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

				val = 0.0;

				if(!LoadMinDataStructAI(tag, year, month, day, hour, min, trend)) 
					return false;
				val = trend.fCurr;
				return true;
			}
			else 
			{
				return DataGetAiYearElse(terminal, tag, ref val, year, data_type);
			}
		}

		public bool LoadHourDataStructDI(string tag, int year, int month, int day, int hour, HOUR_DATA_DIGITAL_STRUCT data)
		{
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;
            
            string tag_file;
			string data_dir;
			string filename;
			FileStream fs;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
			filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, month, tag_file);

			if(!File.Exists(filename))	return false;
			fs = File.OpenRead(filename);
			if(fs == null)			return false;

			if(fs.Length != 18+HOUR_DATA_DIGITAL_STRUCT.struct_size*24*31) 
			{
				fs.Close();
				return false;
			}

			BinaryReader br = new BinaryReader(fs);

			fs.Seek(18+((day-1)*24L+hour)*9, SeekOrigin.Begin);

			data.wCountOnOff = br.ReadUInt16();
			data.dwOnTime = br.ReadUInt32();
			data.flag = br.ReadByte();
			data.crc = br.ReadUInt16();

			fs.Close();

			if(data.flag == 0)	
			{
				return false;	// 자료는 있으나 초기화 되어있는 값이다.
			}

			return true;
		}

		public bool LoadHourDataStructDI(string tag, DateTime t, HOUR_DATA_DIGITAL_STRUCT data)
		{
			return LoadHourDataStructDI(tag, t.Year, t.Month, t.Day, t.Hour, data);
		}

		//------------------------------------------------------------------------------
		//	아날로그 시간 데이터를 읽어온다.
		//------------------------------------------------------------------------------

		public bool CatDataGetDiHour(int terminal, string tag, ref uint val, int year, int month, int day, int hour, EnumDataType data_type)
		{
			HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();

			val = 0;

			if(!LoadHourDataStructDI(tag, year, month, day, hour, data))	return false;


			if(data_type == EnumDataType.ONTIME) 
			{
				val = data.dwOnTime;
			}
			else if(data_type == EnumDataType.OFFTIME) 
			{
				val = 3600-data.dwOnTime;
			}
			else if(data_type == EnumDataType.COUNT) 
			{
				val = data.wCountOnOff;
			}
			else 
			{
				return false;
			}

			return true;
		}

		public bool CatDataGetDiDay(int terminal, string tag, ref uint val, int year, int month, int day, EnumDataType data_type)
		{
			if(day < 1 || day > 31)	return false;
			
			string tag_file;
			string data_dir;
			string filename;

			HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();
			bool read_flag = false;
			int  read_count = 0;
			int  i;
			FileStream fs;

			val = 0;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
			filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, month, tag_file);

			if(!File.Exists(filename))	return false;
			fs = File.OpenRead(filename);
			if(fs == null)			return false;

			if(fs.Length != 18+HOUR_DATA_DIGITAL_STRUCT.struct_size*24*31) 
			{
				fs.Close();
				return false;
			}

			BinaryReader br = new BinaryReader(fs);

			fs.Seek(18+((day-1)*24L)*9, SeekOrigin.Begin);

			for(i = 0; i < 24; i++) 
			{
				data.wCountOnOff = br.ReadUInt16();
				data.dwOnTime = br.ReadUInt32();
				data.flag = br.ReadByte();
				data.crc = br.ReadUInt16();

				if(data.flag == 1) 
				{
					read_flag = true;
					read_count ++;

					if(data_type == EnumDataType.ONTIME) 
					{
						val += data.dwOnTime;
					}
					else if(data_type == EnumDataType.OFFTIME) 
					{
						val += data.dwOnTime;
					}
					else if(data_type == EnumDataType.COUNT) 
					{
						val += data.wCountOnOff;
					}
					else 
					{
						fs.Close();
						return false;
					}
				}
			}

			fs.Close();

			if(read_flag == false) 
			{
				return false;	// 읽은 데이터가 없다.
			}

			if(data_type == EnumDataType.OFFTIME) 
			{
				val = (uint)(read_count*3600-val);
			}

			return true;
		}

		void GetDayFromWeek(int syear, int week, out int fr_year, out int fr_mon, out int fr_day)
		{
			DateTime dt = new DateTime(syear, 1, 1);

			int weekday = (int)dt.DayOfWeek;

			DateTime dtFr = dt.AddDays(-weekday);

			if(week > 1) 
			{
				dtFr = dtFr.AddDays(7*(week-1));
			}

			fr_year = dtFr.Year;
			fr_mon  = dtFr.Month;
			fr_day  = dtFr.Day;
		}

		bool CatDataGetDiWeek(int terminal, string tag, ref uint val, int year, int week, EnumDataType data_type)
		{
			int fr_year;
			int fr_mon;
			int fr_day;

			GetDayFromWeek(year, week, out fr_year, out fr_mon, out fr_day);

			val = 0;

			bool read_flag = false;
			int  read_count = 0;
		    uint imsi_val = 0;

			for(int i = 0; i < 7; i++) 
			{
				if(CatDataGetDiDay(0, tag, ref imsi_val, fr_year, fr_mon, fr_day, data_type))
				{
					read_flag = true;
					read_count++;
					val += imsi_val;
				}
				TimeUtil.PlusDay(ref fr_year, ref fr_mon, ref fr_day);
			}

			if(read_flag == false) 
			{
				return false;	// 읽은 데이터가 없다.
			}

			return true;
		}

		public bool CatDataGetDiMonth(int terminal, string tag, ref uint val, int year, int month, EnumDataType data_type)
		{
			string tag_file;
			string data_dir;
			string filename;

			HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();
			bool read_flag = false;
			int  read_count = 0;
			int  i;
			FileStream fs;

			val = 0;

			tag_file = TagUtil.ConvertTagToFile(tag);
			data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
			filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, month, tag_file);

			if(!File.Exists(filename))	return false;

			fs = File.OpenRead(filename);
			if(fs == null)			return false;
			if(fs.Length != 18+HOUR_DATA_DIGITAL_STRUCT.struct_size*24*31) 
			{
				fs.Close();
				return false;
			}

			BinaryReader br = new BinaryReader(fs);

			fs.Seek(18, SeekOrigin.Begin);	// head

			for(i = 0; i < 24*31; i++) 
			{
				data.wCountOnOff = br.ReadUInt16();
				data.dwOnTime = br.ReadUInt32();
				data.flag = br.ReadByte();
				data.crc = br.ReadUInt16();

				if(data.flag == 1) 
				{
					read_flag = true;
					read_count ++;

					if(data_type == EnumDataType.ONTIME) 
					{
						val += data.dwOnTime;
					}
					else if(data_type == EnumDataType.OFFTIME) 
					{
						val += data.dwOnTime;
					}
					else if(data_type == EnumDataType.COUNT) 
					{
						val += data.wCountOnOff;
					}
					else 
					{
						fs.Close();
						return false;
					}
				}
			}

			fs.Close();

			if(read_flag == false) 
			{
				return false;	// 읽은 데이터가 없다.
			}

			if(data_type == EnumDataType.OFFTIME) 
			{
				val = (uint)(read_count*3600-val);
			}

			return true;
		}

		public bool CatDataGetDiYear(int terminal, string tag, ref uint val, int year, EnumDataType data_type)
		{
			int month;
			bool read_flag = false;
			uint imsi=0;

			val = 0;

			for(month = 1; month <= 12; month++) 
			{
				if(CatDataGetDiMonth(terminal, tag, ref imsi, year, month, data_type)) 
				{
					read_flag = true;
					val += imsi;
				}
			}

			return read_flag;
		}

		public bool CheckUserName(out string err_msg, string username, string passcode)
		{
			string userfile = String.Format("{0}\\Users\\{1}.user", TotalConfig.sDirWorkProject, username);

			UserInfoStruct info = new UserInfoStruct();
			
			if(!info.LoadFile(out err_msg, userfile))	return false;
			if(info.sPassCode != passcode) 
			{
				err_msg = "Invalid username or password.";
				return false;
			}

			return true;
		}

		public bool DefaultUserCheck(out string err_msg, out string username)
		{
			username = TotalConfigW.GetDefaultUser(TotalConfig.sDirWorkProject);

			string userfile = String.Format("{0}\\Users\\{1}.user", TotalConfig.sDirWorkProject, username);

			UserInfoStruct info = new UserInfoStruct();
			
			bool retn = info.LoadFile(out err_msg, userfile);

			return retn;
		}

        class FileInfoCompare : IComparer
        {
            int IComparer.Compare(object o1, object o2)
            {
                FileInfo f1 = (FileInfo)o1;
                FileInfo f2 = (FileInfo)o2;

                return String.Compare(f1.Name, f2.Name);
            }
        }

		public DataSet GetLogLists()
		{
			string data_dir = TotalConfig.GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);

			string path = String.Format("{0}\\Log", data_dir);

			if(!Directory.Exists(path))	return null;
			
			DirectoryInfo info = new DirectoryInfo(path);

			DataSet ds = new DataSet();
			DataTable dt = new DataTable("LOG");
            DataColumn dc = new DataColumn("FileName", Type.GetType("System.String"));
			dt.Columns.Add(dc);

			DataRow row;

            FileInfo[] fis;

            fis = info.GetFiles("*.LOG");
            Array.Sort(fis, new FileInfoCompare());
			foreach(FileInfo fi in fis) 
			{
				row = dt.NewRow();
				row[0] = fi.Name;
				dt.Rows.Add(row);
			}

			ds.Tables.Add(dt);

			return ds;
		}

		public DataSet GetLogFile(string log_name)
		{
			string data_dir = TotalConfig.GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);

			string path = String.Format("{0}\\LOG\\{1}", data_dir, log_name);

			if(!File.Exists(path))	return null;

			DataSet ds = new DataSet();
			DataTable dt = new DataTable("LOG");
			DataColumn dc = new DataColumn("Description", Type.GetType("System.String"));
			dc.MaxLength = 256;
			dt.Columns.Add(dc);

			DataRow row;
			string one_line="";

			TextReader reader;

			string ext = Path.GetExtension(path);

			if(String.Compare(ext, ".logx", true) == 0)
				reader = new StreamReader(path);
			else
				reader = new StreamReader(path, System.Text.Encoding.Default);

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;

				row = dt.NewRow();
				row[0] = one_line;
				dt.Rows.Add(row);
			}

			reader.Close();
			ds.Tables.Add(dt);

			return ds;
		}

		public DataSet GetAlarmLists()
		{
			string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

			string path = String.Format("{0}\\ALARM", data_dir);

			if(!Directory.Exists(path))	return null;
			
			DirectoryInfo info = new DirectoryInfo(path);

			DataSet ds = new DataSet();
			DataTable dt = new DataTable("ALARM");
			DataColumn dc = new DataColumn("FileName", Type.GetType("System.String"));
			dt.Columns.Add(dc);
			dc = new DataColumn("AlarmCount", Type.GetType("System.Int32"));
			dt.Columns.Add(dc);

			DataRow row;

            FileInfo[] fis;
            
            fis = info.GetFiles("*.AL3");
            Array.Sort(fis, new FileInfoCompare());

			foreach(FileInfo fi in fis) 
			{
				row = dt.NewRow();
				row[0] = fi.Name;
				row[1] = fi.Length/ALARM_FILE_STRUCT.struct_size;
				dt.Rows.Add(row);
			}

            fis = info.GetFiles("*.ALMX");
            Array.Sort(fis, new FileInfoCompare());
			foreach(FileInfo fi in fis) 
			{
				row = dt.NewRow();
				row[0] = fi.Name;
				row[1] = Tools.GetLineHap(fi.FullName);
				dt.Rows.Add(row);
			}

			ds.Tables.Add(dt);

			return ds;
		}

		void MakeAlarmHeader(DataTable dt)
		{
			DataColumn dc;
			dc = new DataColumn("alarm_datetime", Type.GetType("System.DateTime"));
			dt.Columns.Add(dc);
			dc = new DataColumn("tag", Type.GetType("System.String"));
			dc.MaxLength = 40;
			dt.Columns.Add(dc);
			dc = new DataColumn("description", Type.GetType("System.String"));
			dc.MaxLength = 80;
			dt.Columns.Add(dc);
			dc = new DataColumn("message", Type.GetType("System.String"));
			dc.MaxLength = 80;
			dt.Columns.Add(dc);
			dc = new DataColumn("alarm_type", Type.GetType("System.UInt16"));
			dt.Columns.Add(dc);
			dc = new DataColumn("priority", Type.GetType("System.UInt16"));
			dt.Columns.Add(dc);
			dc = new DataColumn("port", Type.GetType("System.UInt16"));
			dt.Columns.Add(dc);
			dc = new DataColumn("station", Type.GetType("System.UInt16"));
			dt.Columns.Add(dc);
			dc = new DataColumn("address", Type.GetType("System.UInt32"));
			dt.Columns.Add(dc);
			dc = new DataColumn("type", Type.GetType("System.UInt16"));
			dt.Columns.Add(dc);	
		}

		void AddAlarmFile(DataTable dt, string path)
		{
			DataRow row;

			string ext = Path.GetExtension(path);

			if(String.Compare(ext, ".almx", true) == 0)	// 텍스트 방식의 파일
			{
				TextReader reader = new StreamReader(path);
				CommaBlockString comma = new CommaBlockString();
				string one_line;
				string imsi = "";
				ushort   uval16 = 0;
				uint   uval32 = 0;

				while(true) 
				{
					one_line = reader.ReadLine();
					if(one_line == null)	break;

					comma.Set(one_line);

					row = dt.NewRow();

					row[0] = comma.GetDateTime();
					comma.GetString(ref imsi);	// tag
					row[1] = imsi;
					comma.GetString(ref imsi);	// description
					row[2] = imsi;
					comma.GetString(ref imsi);	// msg
					row[3] = imsi;	
					comma.GetWORD(ref uval16);	// alarm_type
					row[4] = uval16;
					comma.GetWORD(ref uval16);	// priority
					row[5] = uval16;
					comma.GetWORD(ref uval16);	// port
					row[6] = uval16;
					comma.GetWORD(ref uval16);	// station
					row[7] = uval16;
					comma.GetDWORD(ref uval32);	// address
					row[8] = uval32;
					comma.GetWORD(ref uval16);	// type
					row[9] = uval16;

					dt.Rows.Add(row);
				}

				reader.Close();
			}
			else 
			{
				ALARM_FILE_STRUCT alarm = new ALARM_FILE_STRUCT();
				FileStream fs = File.OpenRead(path);
				BinaryReader reader = new BinaryReader(fs);

				int alarm_count = (int)(fs.Length/ALARM_FILE_STRUCT.struct_size);

				for(int i = 0; i < alarm_count; i++) 
				{
					//if(!Tools.TextGetOneLine(fs, ref one_line))	break;
					alarm.LoadFromFile(reader);

					row = dt.NewRow();
					//row[0] = one_line;
					row[0] = alarm.t.ToDateTime();
					row[1] = alarm.tag;
					row[2] = alarm.description;
					row[3] = alarm.msg;
					row[4] = alarm.alarm_type;
					row[5] = alarm.priority;
					row[6] = alarm.port;
					row[7] = alarm.station;
					row[8] = alarm.address;
					row[9] = alarm.type;
					dt.Rows.Add(row);
				}

				fs.Close();
			}
		}

		public DataSet GetAlarmFile(string alarm_file)
		{
			string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

			string path = String.Format("{0}\\ALARM\\{1}", data_dir, alarm_file);

            // 2007.10.29 추가. 주어진 파일이 없으면 다음 버전이나 이전버전을 읽는다.
            if (!File.Exists(path))
            {
                string ext = Path.GetExtension(alarm_file);

                if (String.Compare(ext, ".ALMX", true) == 0)
                {
                    path = String.Format("{0}\\ALARM\\{1}.AL3", data_dir, Path.GetFileNameWithoutExtension(alarm_file));
                }
                else
                {
                    path = String.Format("{0}\\ALARM\\{1}.ALMX", data_dir, Path.GetFileNameWithoutExtension(alarm_file));
                }

                if (!File.Exists(path)) return null;
            }

			DataSet ds = new DataSet();
			DataTable dt = new DataTable("ALARM");

			MakeAlarmHeader(dt);
			AddAlarmFile(dt, path);

			ds.Tables.Add(dt);

			return ds;
		}

		bool IsFileInclude(string filename, int year, int month, int day)
		{
			string name = Path.GetFileNameWithoutExtension(filename);
			if(name.Length != 8)	return false;// 날짜형 이름이 아니다.

			int y = ConvertTool.ToInt32(name.Substring(0, 4));
			int m = ConvertTool.ToInt32(name.Substring(4, 2));
			int d = ConvertTool.ToInt32(name.Substring(6, 2));

			if(year != 0 && year != y)		return false;
			if(month != 0 && month != m)	return false;
			if(day != 0 && day != d)		return false;

			return true;
		}

		public DataSet GetAlarmFileByScript(int year, int month, int day, int type)
		{
			
			string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

			string path = String.Format("{0}\\ALARM", data_dir);

			if(!Directory.Exists(path))	return null;

			DataSet ds = new DataSet();
			DataTable dt = new DataTable("ALARM");

			MakeAlarmHeader(dt);
			
			DirectoryInfo info = new DirectoryInfo(path);

			foreach(FileInfo fi in info.GetFiles("*.AL3")) 
			{
				if(IsFileInclude(fi.Name, year, month, day))
					AddAlarmFile(dt, fi.FullName);
			}

			foreach(FileInfo fi in info.GetFiles("*.ALMX")) 
			{
				if(IsFileInclude(fi.Name, year, month, day))
					AddAlarmFile(dt, fi.FullName);
			}

			ds.Tables.Add(dt);

			return ds;
		}

		public DataSet GetOnOffList(int year, int mon)
		{
			TextReader reader = null;

			string path = String.Format("{0}\\database\\{1:0000}{2:00}.datx", TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject), year, mon);
			if(File.Exists(path)) 
			{
				reader = new StreamReader(path);
			}
			else 
			{
				path = String.Format("{0}\\database\\{1:0000}{2:00}.dat", TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject), year, mon);
				if(File.Exists(path)) 
				{
					reader = new StreamReader(path, System.Text.Encoding.Default);	
				}
			}

			if(reader == null)	return null;

			DataSet ds = new DataSet();
			DataTable dt = new DataTable("OnOffList");
			DataColumn dc;

			dc = new DataColumn("tag", Type.GetType("System.String"));
			dc.MaxLength = 40;
			dt.Columns.Add(dc);

			dc = new DataColumn("start_time", Type.GetType("System.String"));
			dc.MaxLength = 11;
			dt.Columns.Add(dc);
			dc = new DataColumn("start_date", Type.GetType("System.String"));
			dc.MaxLength = 11;
			dt.Columns.Add(dc);

			dc = new DataColumn("end_time", Type.GetType("System.String"));
			dc.MaxLength = 11;
			dt.Columns.Add(dc);
			dc = new DataColumn("end_date", Type.GetType("System.String"));
			dc.MaxLength = 11;
			dt.Columns.Add(dc);

			dc = new DataColumn("oper_time", Type.GetType("System.String"));
			dc.MaxLength = 15;
			dt.Columns.Add(dc);
            
			DataRow row;
			
			string one_line;

			ArrayList array = new ArrayList();
			CommaBlockString comma = new CommaBlockString();
			string imsi="";

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;

				array.Clear();
				comma.Set(one_line);
				while(true) 
				{
					if(comma.IsEOS())	break;
					comma.GetString(ref imsi);
					array.Add(imsi);
				}

				if(array.Count == 0)	continue;

				for(int c = dt.Columns.Count; c < array.Count; c++) 
				{
					dc = new DataColumn("Item"+c, Type.GetType("System.String"));
					dc.MaxLength = 256;
					dt.Columns.Add(dc);
				}

				row = dt.NewRow();
				
				for(int i = 0; i < array.Count; i++) 
				{
					row[i] = array[i];
				}
				dt.Rows.Add(row);
			}

			reader.Close();

			ds.Tables.Add(dt);

			return ds;
		}

		public DataSet GetDataSetFromMdb(string filename, string command, out string error)
		{
			if(!File.Exists(filename)) 
			{
				error = "파일을 찾을 수 없습니다.";
				return null;
			}

			OleDbConnection conn = new OleDbConnection();

			conn.ConnectionString = String.Format("Provider=Microsoft.JET.OLEDB.4.0;Data Source={0};", filename);

			try 
			{
				conn.Open();
			}
			catch (Exception exception)
			{
				error = String.Format("GetDataSetFromMdb Connection.Open() error\n{0}", exception.Message);
				return null;
			}

			OleDbDataAdapter adapter = new OleDbDataAdapter(command, conn);

			DataSet ds = new DataSet();

			try 
			{
				adapter.Fill(ds, "TableTest");
			}
			catch 
			{
			}
			finally 
			{
				conn.Close();
			}

			error = "";
			return ds;
		}

		public DataSet GetDataSetFromDsn(string dsn, string command, out string error)
		{
			error = "";
			ConnectionString con_str;

			con_str = DbTool.GetConnectionString(dsn);

			if(con_str == null) 
			{
				if(Tools.IsLangKorean())
					error = String.Format("{0} 목록을 찾을 수 없습니다.", dsn);
				else 
					error = String.Format("Can't find {0} list at connection string", dsn);
				return null;
			}

			CommonDbConnection conn = new CommonDbConnection(con_str.dbConnectionType);

			conn.ConnectionString = con_str.dsn;
			try 
			{
				conn.Open();
			}
			catch (Exception exception)
			{
				error = String.Format("Connection.Open() error\n{0}", exception.Message);
				return null;
			}
			CommonDbDataAdapter adapter = new CommonDbDataAdapter(command, conn);

			DataSet ds = new DataSet();

			try 
			{
				adapter.Fill(ds, "TableTest");
			}
			catch (Exception exception)
			{
				error = String.Format("adapter.Fill Error\nCommand={0}\nError Message={1}", command, exception.Message);
				ds = null;
			}
			finally 
			{
				conn.Close();
			}
			
			return ds;
		}

		public bool DataSetCommand(string dsn, string command, out string error)
		{
			ConnectionString con_str;

			con_str = DbTool.GetConnectionString(dsn);

			if(con_str == null) 
			{
				if(Tools.IsLangKorean())
					error = String.Format("{0} 목록을 찾을 수 없습니다.", dsn);
				else  
					error = String.Format("Can't find {0} list at connection string", dsn);

				return false;
			}

			CommonDbConnection conn = new CommonDbConnection(con_str.dbConnectionType);

			conn.ConnectionString = con_str.dsn;

			try 
			{
				conn.Open();
			}
			catch (Exception exception)
			{
				error = exception.Message;
				return false; 
			}

			CommonDbCommand dbcom = new CommonDbCommand(command, conn);

			try 
			{
				dbcom.ExecuteNonQuery();
			}
			catch (Exception exception) 
			{
				conn.Close();
				error = exception.Message;
				return false;
			}

			conn.Close();

			error = "";
			return true;
		}

		public DataSet GetDataSetFromOdbc(string dsn, string command, out string error)
		{
			OdbcConnection conn = new OdbcConnection();

			conn.ConnectionString = dsn;

			try 
			{
				conn.Open();
			}
			catch (Exception exception)
			{
				if(Tools.IsLangKorean()) 
					error = String.Format("{0} 에 연결할 수 없습니다.\nError={1}", dsn, exception.Message);
				else
					error = String.Format("Can't connect to {0}\nError={1}", dsn, exception.Message);

				return null;
			}

			OdbcDataAdapter adapter = new OdbcDataAdapter(command, conn);

			DataSet ds = new DataSet();

			try 
			{
				adapter.Fill(ds, "TableTest");
			}
			catch (Exception exception)
			{
				conn.Close();
				error = String.Format("Query Error={0}", exception.Message);
				return null;
			}

			conn.Close();

			error = "";
			return ds;
		}

		public bool DataSetOdbcCommand(string dsn, string command, out string error)
		{
			OdbcConnection conn = new OdbcConnection();

			conn.ConnectionString = dsn;

			try 
			{
				conn.Open();
			}
			catch (Exception exception)
			{
				if(Tools.IsLangKorean()) 
					error = String.Format("{0} 에 연결할 수 없습니다.\nError={1}", dsn, exception.Message);
				else
					error = String.Format("Can't connect to {0}\nError={1}", dsn, exception.Message);

				return false;
			}

			OdbcCommand dbcom = new OdbcCommand(command, conn);

			try 
			{
				dbcom.ExecuteNonQuery();
			}
			catch (Exception exception) 
			{
				error = exception.Message;
				conn.Close();
				return false;
			}

			conn.Close();

			error = "";
			return true;
		}
	}*/
}

