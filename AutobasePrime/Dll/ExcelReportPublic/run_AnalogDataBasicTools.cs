using System;
using System.Data;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for run_AnalogDataBasicTools.
	/// </summary>
	public class run_AnalogDataBasicTools
	{
		public run_AnalogDataBasicTools()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static public string AiAve(memberConfigStruct member, DataSet ds, int hap)
		{
			bool		bExist = false;
			double		val = 0;
			int			i, count = 0;
			DataRow		row;

			for(i = 0; i < hap; i++)
			{				
				try 
				{
					row = ds.Tables[0].Rows[i];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
					val += ConvertTool.ToDouble(row["AVE"].ToString());	
					count ++;
					bExist = true;
				}
				catch
				{
				}
			}
			if(bExist) 
			{
				try
				{
					val /= count;
					return val.ToString();
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return member.noneDataString;
		}


		static public string AiMax(memberConfigStruct member, DataSet ds, int hap)
		{
			bool		bExist = false;
			double		val, max = 0;
			int			i;
			DataRow		row;

			for(i = 0; i < hap; i++)
			{				
				try 
				{
					row = ds.Tables[0].Rows[i];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
					val = ConvertTool.ToDouble(row["MAX"].ToString());	
					if(bExist) 
					{
						if(val > max) max = val;
					}
					else max = val;					
					bExist = true;
				}
				catch
				{
				}
			}
			if(bExist) 
			{
				try
				{
					return max.ToString();
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return member.noneDataString;
		}

		static public string AiMin(memberConfigStruct member, DataSet ds, int hap)
		{
			bool		bExist = false;
			double		val, min = 0;
			int			i;
			DataRow		row;

			for(i = 0; i < hap; i++)
			{
				try 
				{
					row = ds.Tables[0].Rows[i];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
					val = ConvertTool.ToDouble(row["MIN"].ToString());	
					if(bExist) 
					{
						if(val < min) min = val;
					}
					else min = val;
					bExist = true;
				}
				catch
				{

				}
			}

			if(bExist) 
			{
				try
				{
					return min.ToString();
					
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return member.noneDataString;
		}

        static public string AiMoment(memberConfigStruct member, DataSet ds, int hap)
        {
            double val;
            DataRow row;

            row = ds.Tables[0].Rows[0];
            if (ConvertTool.ToInt16(row[0].ToString()) == 1)
            {
                try
                {
                    val = ConvertTool.ToDouble(row["MOMENT"].ToString());
                }
                catch
                {
                    return member.noneDataString;
                }

                return val.ToString();
            }
            else {
                return member.noneDataString;
            }
        }

		static public string AiSumAndMaxSub(memberConfigStruct member, DataSet ds, int hap, string columnName)
		{
			bool		bExist = false;
			double		val = 0;
			int			i;
			DataRow		row;

			for(i = 0; i < hap; i++)
			{
				try 
				{
					row = ds.Tables[0].Rows[i];
					if(ConvertTool.ToInt16(row[0].ToString()) != 1) continue;
					val += ConvertTool.ToDouble(row[columnName].ToString());					
					bExist = true;
				}
				catch
				{
				}
			}
			if(bExist) 
			{
				try
				{
					return val.ToString();
				}
				catch 
				{
					return member.noneDataString;
				}
			}
			return member.noneDataString;
		}

	}
}
