using System;
using System.Text;

namespace NetTools
{
	/// <summary>
	/// String을 읽을 때 ,를 포함한 문자열도 사용할 수 있도록 해 준다.
	/// 기본 함수들은 CommaBlockString과 같다.
	/// GetString부분만 틀리다
	/// </summary>
	/// 
	public class CommaTextReader : CommaBlockString
	{
		/*
		public override void GetString(ref string s)
		{
			s = "";
			bool open = false;
			bool flag = false;
			int  count = 0;
			char[] buf = new char[scanBufHap];

			while(true) 
			{
				if(scanBufPos >= scanBufHap) 
				{
					s = new string(buf, 0, count);
					return;
				}

				if(open) 
				{
					if(scanBuf[scanBufPos] == '"') 
					{
						if(flag) 
						{
							//s += '"';
							buf[count++] = '"';
							flag = false;
						}
						else 
						{
							flag = true;
						}
						scanBufPos++;
						continue;
					}
					else if(scanBuf[scanBufPos] == cBlockCode) 
					{
						if(flag) 
						{
							scanBufPos++;
							s = new string(buf, 0, count);
							return;
						}
						else 
						{
							buf[count++] = scanBuf[scanBufPos];
							//s += scanBuf[scanBufPos];
							scanBufPos++;
						}
					}
					else 
					{
						if(flag)	// 문장 중간에 ""로 되어야 하는데 하나가 빠진 "는 오류이지만 표시해준다.
						{
							s += '"';
							flag = false;
						}
						//s += scanBuf[scanBufPos];
						buf[count++] = scanBuf[scanBufPos];
						scanBufPos++;
						
					}
				}
				else 
				{
					if(s.Length == 0 && scanBuf[scanBufPos] == '"') 
					{
						open = true;
						scanBufPos++;
						continue;
					}
					if(scanBuf[scanBufPos] == cBlockCode) 
					{
						scanBufPos++;
						s = new string(buf, 0, count);
						return;
					}
				
					if(s.Length == 0 && (scanBuf[scanBufPos] == ' '  || scanBuf[scanBufPos] == '\t' ||
						scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r')) 
					{

					}
					else 
					{
						//s += scanBuf[scanBufPos];
						buf[count++] = scanBuf[scanBufPos];
					}

					scanBufPos++;
				}
			}
		}
		*/

        public override void GetString(ref string s)
        {
            s = GetStringLocal(false);
        }

        /// <summary>
        /// 왼쪽의 공간도 함께 읽어온다.
        /// </summary>
        public string GetStringWithSpace()
        {
            return GetStringLocal(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="use_left_space">왼쪽의 공간을 사용한다.</param>
        /// <returns></returns>
		string GetStringLocal(bool use_left_space)
		{
            // string += 속도가 너무 늦다. 2010.7-2 수정
            StringBuilder s = new StringBuilder("");
            bool open = false;
            bool flag = false;

            while (true)
            {
                if (scanBufPos >= scanBufHap)
                {
                    return s.ToString();
                }

                if (open)
                {
                    if (scanBuf[scanBufPos] == '"')
                    {
                        if (flag)
                        {
                            s.Append('"');
                            flag = false;
                        }
                        else
                        {
                            flag = true;
                        }
                        scanBufPos++;
                        continue;
                    }
                    else if (scanBuf[scanBufPos] == cBlockCode)
                    {
                        if (flag)
                        {
                            scanBufPos++;
                            return s.ToString();
                        }
                        else
                        {
                            s.Append(scanBuf[scanBufPos]);
                            scanBufPos++;
                        }
                    }
                    else
                    {
                        if (flag)	// 문장 중간에 ""로 되어야 하는데 하나가 빠진 "는 오류이지만 표시해준다.
                        {
                            s.Append('"');
                            flag = false;
                        }
                        s.Append(scanBuf[scanBufPos]);
                        scanBufPos++;

                    }
                }
                else
                {
                    if (s.Length == 0 && scanBuf[scanBufPos] == '"')
                    {
                        open = true;
                        scanBufPos++;
                        continue;
                    }
                    if (scanBuf[scanBufPos] == cBlockCode)
                    {
                        scanBufPos++;
                        return s.ToString();
                    }

                    if (use_left_space == false && s.Length == 0 && (scanBuf[scanBufPos] == ' ' || scanBuf[scanBufPos] == '\t' ||
                        scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r'))
                    {

                    }
                    else
                    {
                        s.Append(scanBuf[scanBufPos]);
                    }

                    scanBufPos++;
                }
            }

            /*
			string s = "";
			bool open = false;
			bool flag = false;

			while(true) 
			{
				if(scanBufPos >= scanBufHap) 
				{
					return s;
				}

				if(open) 
				{
					if(scanBuf[scanBufPos] == '"') 
					{
						if(flag) 
						{
							s += '"';
							flag = false;
						}
						else 
						{
							flag = true;
						}
						scanBufPos++;
						continue;
					}
					else if(scanBuf[scanBufPos] == cBlockCode) 
					{
						if(flag) 
						{
							scanBufPos++;
							return s;
						}
						else 
						{
							s += scanBuf[scanBufPos];
							scanBufPos++;
						}
					}
					else 
					{
						if(flag)	// 문장 중간에 ""로 되어야 하는데 하나가 빠진 "는 오류이지만 표시해준다.
						{
							s += '"';
							flag = false;
						}
						s += scanBuf[scanBufPos];
						scanBufPos++;
						
					}
				}
				else 
				{
					if(s.Length == 0 && scanBuf[scanBufPos] == '"') 
					{
						open = true;
						scanBufPos++;
						continue;
					}
					if(scanBuf[scanBufPos] == cBlockCode) 
					{
						scanBufPos++;
						return s;
					}

                    if (use_left_space == false && s.Length == 0 && (scanBuf[scanBufPos] == ' ' || scanBuf[scanBufPos] == '\t' ||
						scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r')) 
					{

					}
					else 
					{
						s += scanBuf[scanBufPos];
					}

					scanBufPos++;
				}
			}*/
		}

		
	}
}

