using System;

namespace NetTools
{
	/// <summary>
	/// Summary description for ConvertTool.
	/// </summary>
	public class ConvertTool
	{
		public ConvertTool()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static float ToSingle(string val)
		{
			return (float)ToDouble(val);
		}

		public static sbyte ToSByte(string val)
		{
			return (sbyte)ToInt64(val);
		}

		public static int ToInt32(string val)
		{
			return (int)ToInt64(val);
		}

		public static uint ToUInt32(string val)
		{
			return (uint)ToInt64(val);
		}

		public static ulong ToUInt64(string val)
		{
			return (ulong)ToInt64(val);
		}

		public static ushort ToUInt16(string val)
		{
			return (ushort)ToInt64(val);
		}

		public static byte ToByte(string val)
		{
			return (byte)ToInt64(val);
		}

		public static short ToInt16(string val)
		{
			return (short)ToInt64(val);
		}

		public static bool ToBoolean(string val)
		{
			return Convert.ToBoolean(val);
		}

		public static int ToInt32(decimal val)
		{
			return Convert.ToInt32(val);
		}

		public static int ToInt32(bool val)
		{
			return Convert.ToInt32(val);
		}

		public static sbyte ToSByte(decimal val)
		{
			return Convert.ToSByte(val);
		}

		public static DateTime ToDateTime(string val)
		{
			return Convert.ToDateTime(val);
		}

		public static DateTime ToDateTime(object val)
		{
			if(typeof(DateTime) == val.GetType())
				return Convert.ToDateTime(val);
			else
				return new DateTime(2000,1,1);
		}

		public static double ToDouble(object val)
		{
			return Convert.ToDouble(val);
		}

		public static decimal ToDecimal(string val)
		{
			return Convert.ToDecimal(val);
		}
		
		public static double ToDouble(string val)
		{
            string buf = "";
            bool point_flag = false;	// 소수점
            bool e_flag = false;		// 지수 시작 플래그
            int sign1 = 0;				// 1.23E21 에서 1.23부분의 부호
            int sign2 = 0;				// 1.23E21 에서 21부분의 부호

            for (int i = 0; i < val.Length; i++)
            {
                if (val[i] >= '0' && val[i] <= '9')	// 숫자일 때
                {
                    buf += val[i];
                }
                else if (val[i] == '-')
                {
                    if (e_flag)
                    {
                        if (sign2 != 0) break;	// 이미 부호는 결정되었다. 중복해서 부호가 나왔다.
                        sign2 = -1;
                    }
                    else
                    {
                        if (sign1 != 0) break;	// 이미 부호는 결정되었다. 중복해서 부호가 나왔다.
                        sign1 = -1;
                    }
                    buf += val[i];
                }
                else if (val[i] == '+')
                {
                    if (e_flag)
                    {
                        if (sign2 != 0) break;	// 이미 부호는 결정되었다. 중복해서 부호가 나왔다.
                        sign2 = 1;
                    }
                    else
                    {
                        if (sign1 != 0) break;	// 이미 부호는 결정되었다. 중복해서 부호가 나왔다.
                        sign1 = 1;
                    }
                    buf += val[i];
                }
                else if (val[i] == 'e' || val[i] == 'E')
                {
                    if (e_flag) break;	// 이미 지수표시가 나왔다.
                    e_flag = true;
                    buf += val[i];
                }
                else if (val[i] == '.')
                {
                    if (e_flag) break;		// 지수에는 소수점이 없다.
                    if (point_flag) break;	// 이미 소숫점이 나왔다.
                    point_flag = true;
                    buf += val[i];
                }
                else if (val[i] == ',')		// 1000단위 구분일 수 있으므로 무시
                {

                }
                else if (val[i] == ' ' || val[i] == '\t') // 빈 공간도 무시한다.
                {

                }
                else if (val[i] == 0x20a9 && i == 0)	// 처음이 \ 표시이면 그냥 간다.
                {
                    continue;
                }
                else if (val[i] == 0x0024 && i == 0)	// 처음이 $ 표시이면 그냥 간다.
                {
                    continue;
                }
                else if (val[i] == 0x00A5 && i == 0)	// 처음이 ￥ 표시이면 그냥 간다.
                {
                    continue;
                }
                else	// 이상한 문자가 나왔다.
                {
                    break;
                }

            }

            if (buf.Length == 0) return 0;

            try
            {
                return Convert.ToDouble(buf);
            }
            catch
            {
                return 0;
            }
		}

		public static long ToInt64(string val)
		{
			if(val.Length == 0)			return 0;
			
			long result = 0;			
			int sign = 0;				
			
			for(int i = 0; i < val.Length; i++) 
			{
				if(val[i] >= '0'&& val[i] <= '9')	// 숫자일 때
				{
					if(sign == 0)	sign = 1;	// 부호를 양으로 결정
					result = result*10+(val[i]-'0');
				}
				else if(val[i] == '-') 
				{
					if(sign != 0)	break;	// 이미 부호는 결정되었다. 중복해서 부호가 나왔다.
					sign = -1;
				}
				else if(val[i] == '+') 
				{
					if(sign != 0)	break;	// 이미 부호는 결정되었다. 중복해서 부호가 나왔다.
					sign = 1;
				}
				else if(val[i] == ',')		// 1000단위 구분일 수 있으므로 무시
				{
					
				}
				else if(val[i] == ' ' || val[i] == '\t') // 빈 공간도 무시한다.
				{

				}
				else	// 이상한 문자가 나왔다.
				{
					break;
				}
			}

			if(sign < 0)	result *= -1;

			return result;
		}
	}
}
