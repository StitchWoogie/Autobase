using System;
using System.Threading;
using System.Data;
using AutoLib;
using System.Collections;
using NetTools;
using AutoLibLocal;
using NetTools.Hash;
using System.Threading.Tasks;

namespace ViewMain
{
	/// <summary>
	/// Summary description for ThreadDataChange.
	/// </summary>
	public class ThreadDataChange
	{
		static public bool bThreadContinue = true;
		static public bool bThreadEnded = false;
		static TimeOutClass timeout = new TimeOutClass();
        static int MAX_READ_BLOCK = 20;               // 한번에 20개씩만 읽는다.

        /* Async 방식
        static void GetCurrentValue(ArrayList array)
        {
            if (array.Count == 0) return;

            if (ConfigVarTotal.bLocalFlag)
            {
                return;
            }
            else
            {
                AutoLib.localhost.ServiceDataTag.ServiceDataTag service = new AutoLib.localhost.ServiceDataTag.ServiceDataTag();
                service.Url = AutoLibLocal.ConfigVarTotal.GetServicePath("ServiceDataTag.asmx");
                service.Timeout = 5000;
                service.GetTagValueListCompleted += new AutoLib.localhost.ServiceDataTag.GetTagValueListCompletedEventHandler(service_GetTagValueListCompleted);

                nCurrentReadingNo++;
                service.GetTagValueListAsync(array.ToArray(), nCurrentReadingNo);

                timeoutCurrentValue.Reset();
                bCurrentReading = true;
            }
        }

        static int nCurrentPos = 0;
        static TimeOutClass timeoutCurrentValue = new TimeOutClass();
        static bool bCurrentReading = false;        // 현재 읽기를 진행중이다.
        static int nCurrentReadingNo = 0;           // 

        static void service_GetTagValueListCompleted(object sender, AutoLib.localhost.ServiceDataTag.GetTagValueListCompletedEventArgs e)
        {
            if ((int)e.UserState == nCurrentReadingNo)
            {
                bCurrentReading = false;
            }

            if (e.Error != null || e.Cancelled == true) return;

            if (e.Result == null)
            {
                // System.Windows.MessageBox.Show("service_GetTagValueListsCompleted 의 e.Result가 null");
                return;
            }

            DataSet ds = e.Result;

            if (ds == null) return;

            string tag;
            int[] nTagPos = new int[1];
            TagAiClass ai;
            TagAoClass ao;
            TagDiClass di;
            TagDoClass dout;
            TagStClass st;
            TagPublicClass tp;

            DataRow row;
            string sval;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                row = ds.Tables[0].Rows[i];

                tag = row["tag"].ToString();

                tp = TagLib.GetStructPublic(tag, ref nTagPos);
                sval = row["curr"].ToString();

                if (tp.assign != null)  // 간접 태그일 경우는 연결된 태그의 값을 대입해 준다.
                {
                    TagPublicClass tp2 = TagLib.GetStructPublic(tp.assign.tag, ref tp.assign.pos);

                    sval = tp2.GetCurr().ToString();
                }

                if (tp.enumTagType == EnumTagType.AI)
                {
                    double val;
                    try
                    {
                        val = ConvertTool.ToDouble(sval);
                    }
                    catch
                    {
                        val = 0;
                    }
                    ai = (TagAiClass)tp;
                    if (val != ai.curr)
                    {
                        ai.curr = val;
                        ai.bChangedDataCurr = true;
                    }
                }
                else if (tp.enumTagType == EnumTagType.AO)
                {
                    double val;
                    try
                    {
                        val = ConvertTool.ToDouble(sval);
                    }
                    catch
                    {
                        val = 0;
                    }
                    ao = (TagAoClass)tp;
                    if (val != ao.curr)
                    {
                        ao.curr = val;
                        ao.bChangedDataCurr = true;
                    }
                }
                else if (tp.enumTagType == EnumTagType.DI)
                {
                    sbyte val;
                    try
                    {
                        val = ConvertTool.ToSByte(sval);
                    }
                    catch
                    {
                        val = 0;
                    }
                    di = (TagDiClass)tp;
                    if (val != di.curr)
                    {
                        di.curr = val;
                        di.bChangedDataCurr = true;
                    }
                }
                else if (tp.enumTagType == EnumTagType.DO)
                {
                    sbyte val;
                    try
                    {
                        val = ConvertTool.ToSByte(sval);
                    }
                    catch
                    {
                        val = 0;
                    }
                    dout = (TagDoClass)tp;
                    if (val != dout.curr)
                    {
                        dout.curr = val;
                        dout.bChangedDataCurr = true;
                    }
                }
                else if (tp.enumTagType == EnumTagType.ST)
                {
                    string val = sval;
                    st = (TagStClass)tp;
                    if (val != st.curr)
                    {
                        st.curr = val;
                        st.bChangedDataCurr = true;
                    }
                }
            }
        }

        static void CheckCurrentValue()
        {
            if (bCurrentReading)
            {
                if (timeoutCurrentValue.IsTimeOut(5))
                {
                    bCurrentReading = false;
                }
                else// 아직 읽기 중이다.
                {
                    return;
                }
            }

            if (nCurrentPos >= TagLib.tagListAll.Length)
            {
                // 한바퀴 돌았다.
                if (timeout.IsTimeOut(SharedData.userInfo.nWebValueUpdateTime))
                {
                    timeout.Reset();
                    nCurrentPos = 0;
                }
            }

            TagPublicClass tp;
            ArrayList array = new ArrayList();

            int i;

            for (i = nCurrentPos; i < TagLib.tagListAll.Length; i++)
            {
                tp = TagLib.GetStructPublic(TagLib.tagListAll[i].tag, ref TagLib.tagListAll[i].tag_pos);

                // 데이터 요구가 있고 전역 태그일 때 스크롤바 기능중 마우스 누른상태에서는 값을 읽지 않는다.
                if (tp.NeedDataCurr && tp.bLocalTag == 0)
                {
                    array.Add(tp.tag);
                    tp.NeedDataCurr = false;

                    if (array.Count >= MAX_READ_BLOCK)
                    {
                        break;
                    }
                }
            }

            nCurrentPos = i;

            if (array.Count > 0)
            {
                GetCurrentValue(array);
                array.Clear();
                return;
            }
        } */

        
        // async 로 변경. 251226 PSU
		static async Task GetCurrentValue(ArrayList array)
		{
            if (array.Count == 0) return;

            DataSet ds;
            string tag;
            int[] nTagPos = new int[1];
            TagAiClass ai;
            TagAoClass ao;
            TagDiClass di;
            TagDoClass dout;
            TagStClass st;
            TagPublicClass tp;

            DataGate gate = new DataGate();
            ds = await gate.GetTagValueListAsync(array);

            if (ds == null)
            {
                //에러 문구 추가 250826 PSU
                string msgNodata;
                if (Tools.IsLangKorean()) msgNodata = "[Check RunMain]\n서버로부터 태그 데이터를 읽어오지 못했습니다.";
                else msgNodata = "[Check RunMain]\nFailed to read Tag Data from server.";
                MessageDisplay.Show(msgNodata);

                return;
            }

            DataRow row;
            string sval;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                row = ds.Tables[0].Rows[i];

                tag = row["tag"].ToString();

                tp = TagLib.GetStructPublic(tag, ref nTagPos);
                sval = row["curr"].ToString();

                if (tp.assign != null)  // 간접 태그일 경우는 연결된 태그의 값을 대입해 준다.
                {
                    TagPublicClass tp2 = TagLib.GetStructPublic(tp.assign.tag, ref tp.assign.pos);

                    sval = tp2.GetCurr().ToString();
                }

                if (tp.enumTagType == EnumTagType.AI)
                {
                    double val;
                    try
                    {
                        val = ConvertTool.ToDouble(sval);
                    }
                    catch
                    {
                        val = 0;
                    }
                    ai = (TagAiClass)tp;
                    if (val != ai.curr)
                    {
                        ai.curr = val;
                        ai.bChangedDataCurr = true;
                    }
                }
                else if (tp.enumTagType == EnumTagType.AO)
                {
                    double val;
                    try
                    {
                        val = ConvertTool.ToDouble(sval);
                    }
                    catch
                    {
                        val = 0;
                    }
                    ao = (TagAoClass)tp;
                    if (val != ao.curr)
                    {
                        ao.curr = val;
                        ao.bChangedDataCurr = true;
                    }
                }
                else if (tp.enumTagType == EnumTagType.DI)
                {
                    sbyte val;
                    try
                    {
                        val = ConvertTool.ToSByte(sval);
                    }
                    catch
                    {
                        val = 0;
                    }
                    di = (TagDiClass)tp;
                    if (val != di.curr)
                    {
                        di.curr = val;
                        di.bChangedDataCurr = true;
                    }
                }
                else if (tp.enumTagType == EnumTagType.DO)
                {
                    sbyte val;
                    try
                    {
                        val = ConvertTool.ToSByte(sval);
                    }
                    catch
                    {
                        val = 0;
                    }
                    dout = (TagDoClass)tp;
                    if (val != dout.curr)
                    {
                        dout.curr = val;
                        dout.bChangedDataCurr = true;
                    }
                }
                else if (tp.enumTagType == EnumTagType.ST)
                {
                    string val = sval;
                    st = (TagStClass)tp;
                    if (val != st.curr)
                    {
                        st.curr = val;
                        st.bChangedDataCurr = true;
                    }
                }
            }
		}

		static async Task RecurseCheckCurrentValue(TagGrClass gr, ArrayList array)
		{
			TagPublicClass tp;
			
			for(int i = 0; i < gr.arrayTag.Count; i++)
			{
				tp = (TagPublicClass)gr.arrayTag[i];
				
				if(tp.enumTagType == EnumTagType.GR) 
				{
					await RecurseCheckCurrentValue((TagGrClass)tp, array);
				}
				else 
				{
					// 데이터 요구가 있고 전역 태그일 때
					if(tp.NeedDataCurr && tp.bLocalTag == 0) 
					{
						array.Add(tp.tag);
						tp.NeedDataCurr = false;

                        if (array.Count >= MAX_READ_BLOCK) 
						{
							await GetCurrentValue(array);
							array.Clear();
						}
					}
				}
				
			}
		}

		static async Task CheckCurrentValue()
		{
			if(!timeout.IsTimeOut(SharedData.userInfo.nWebValueUpdateTime))	return;
			timeout.Reset();

			ArrayList array = new ArrayList();

			array.Clear();

			await RecurseCheckCurrentValue(TagLib.GetRootGroup(), array);

			if(array.Count > 0) 
			{
				await GetCurrentValue(array);
				array.Clear();
			}
			// debug
			// sharedData.formMain.Text = "Request Count="+array.Count.ToString();
		}

		static void RecurseCheckCurrentValueLocal(TagGrClass gr)
		{
			TagPublicClass tp;
			string share_val = "";

			for(int i = 0; i < gr.arrayTag.Count; i++)
			{
				tp = (TagPublicClass)gr.arrayTag[i];

				if(tp.enumTagType == EnumTagType.GR) 
				{
					RecurseCheckCurrentValueLocal((TagGrClass)tp);
				}
				else 
				{
					if(!SharedTag.GetCurrAfterWrite(tp.tag, ref share_val))	 continue;

					if(tp.enumTagType == EnumTagType.AI) 
					{
						double val = ConvertTool.ToDouble(share_val);
						TagAiClass ai = (TagAiClass)tp;
						if(val != ai.curr) 
						{
							ai.curr = val;
							ai.bChangedDataCurr = true;
						}
					}
					else if(tp.enumTagType == EnumTagType.AO) 
					{
						double val = ConvertTool.ToDouble(share_val);
						TagAoClass ao = (TagAoClass)tp;
						if(val != ao.curr) 
						{
							ao.curr = val;
							ao.bChangedDataCurr = true;
						}
					}
					else if(tp.enumTagType == EnumTagType.DI) 
					{
						sbyte val = ConvertTool.ToSByte(share_val);
						TagDiClass di = (TagDiClass)tp;
						if(val != di.curr) 
						{
							di.curr = val;
							di.bChangedDataCurr = true;
						}
					}
					else if(tp.enumTagType == EnumTagType.DO) 
					{
						sbyte val = ConvertTool.ToSByte(share_val);
						TagDoClass dout = (TagDoClass)tp;
						if(val != dout.curr) 
						{
							dout.curr = val;
							dout.bChangedDataCurr = true;
						}
					}
					else if(tp.enumTagType == EnumTagType.ST) 
					{
						string val = share_val;
						TagStClass st = (TagStClass)tp;
						if(val != st.curr) 
						{
							st.curr = val;
							st.bChangedDataCurr = true;
						}
					}
				}
			}	
		}

		static void CheckCurrentValueLocal()
		{
			RecurseCheckCurrentValueLocal(TagLib.GetRootGroup());
		}

		static async Task CheckElseValue()
		{

            await CheckServerEvent.CheckEventAsync();
		}

		static public async Task ThreadMainAsync(CancellationToken token)
		{
            // 중간에 사이트를 변경할 수 있으므로 기본값으로 설정해야 한다.  2013-7-4 추가
            bThreadContinue = true;
		    bThreadEnded = false;

			while(bThreadContinue && !token.IsCancellationRequested)
            {
				//Thread.Sleep(1);
                await Task.Delay(100, token); // 1 >> 100 으로 변경

                if (ConfigVarTotal.bLocalFlag) 
				{
					CheckCurrentValueLocal();
				}

				else // Web Check
				{
					if(WebCommInfo.nFailCount > 0) // Web 서비스와 통신 중 오류 발생
					{
						await DataGate.CheckServiceAlive();
					}
					else 
					{
						await CheckCurrentValue();
						await CheckElseValue();
					}
				}
			}

			bThreadEnded = true;
		}


        /*
		static int nCurrentPos = 0;

		static void CheckCurrentValueByTimer()
		{
			if(nCurrentPos >= TagLib.tagListAll.Length) 
			{
				// 한바퀴 돌았다.
				if(timeout.IsTimeOut(SharedData.userInfo.nWebValueUpdateTime)) 
				{
					timeout.Reset();
					nCurrentPos = 0;
				}
			}

			TagPublicClass tp;
			ArrayList array = new ArrayList();

			int i;

			for(i = nCurrentPos; i < TagLib.tagListAll.Length; i++)
			{
				tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);
				
				// 데이터 요구가 있고 전역 태그일 때
				if(tp.NeedDataCurr && tp.bLocalTag == 0) 
				{
					array.Add(tp.tag);
					tp.NeedDataCurr = false;

                    if (array.Count >= MAX_READ_BLOCK) 
					{
						break;
					}
				}
			}

			nCurrentPos = i;

			if(array.Count > 0) 
			{
				GetCurrentValue(array);
				array.Clear();
				return;
			}
		}

		static public void TimerMain()
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				CheckCurrentValueLocal();
			}
			else // Web Check
			{
				if(WebCommInfo.nFailCount > 0) // Web 서비스와 통신 중 오류 발생
				{
					DataGate.CheckServiceAlive();
				}
				else 
				{
					CheckCurrentValueByTimer();
					CheckElseValue();
				}
			}
		}*/
	}

    class CheckServerEvent
    {
        public static bool bAlarmEventChanged = false;
        static int nAlarmEventTrans = -1;
        static int nAlarmEventCount = 0;

        public static async Task CheckEventAsync()
        {
            if (!ConfigViewMain.bUseAlarmServer) return;

            if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
            {
                AutoLib.WcfReferenceDataGateServer.ServiceDataGateServerClient service = ServiceLibSvcDataGate.GetServiceDataGate();

                string event_result = "";

                try
                {
                    MakeHashCrc ht = new MakeHashCrc();
                    string hash = ht.ComputeHash("CheckServerEvent" + ServiceLibSvcDataGate.nConnectionID.ToString());

                    event_result =await service.CheckServerEventAsync(ServiceLibSvcDataGate.nConnectionID, hash);
                }
                catch
                {
                    event_result = null;
                }

                if (event_result != null)
                {
                    CommaBlockString comma = new CommaBlockString();
                    comma.Set(event_result);

                    int trans = comma.GetInt();
                    if (trans != nAlarmEventTrans)
                    {
                        bAlarmEventChanged = true;
                        nAlarmEventTrans = trans;
                    }

                    nAlarmEventCount = comma.GetInt();
                }

            }
            else
            {
                if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 2, 7, 6))
                {
                    AutoLib.ServiceReferenceDataGateServer.WcfServiceDataGateServerProxyClient service = ServiceLib.GetServiceDataGateServer();

                    string event_result = "";

                    try
                    {
                        MakeHashCrc ht = new MakeHashCrc();
                        ServiceLibSvcDataGate.nConnectionID = -2;
                        string hash = ht.ComputeHash("CheckServerEvent" + ServiceLibSvcDataGate.nConnectionID.ToString());

                        event_result = await service.CheckServerEventAsync(ServiceLibSvcDataGate.nConnectionID, hash);
                    }
                    catch (Exception exception)
                    {
                        if(Tools.IsLangKorean())
                            MessageDisplay.ShowInThread("CheckServerEvent() 오류:{0}\n\nLocalMain에서 '경보 웹 서버 사용' 옵션을 확인하세요.", exception.Message);
                        else
                            MessageDisplay.ShowInThread("CheckServerEvent() Exception:{0}\n\nEnable the option 'Use Alarm Web Server' in LocalMain", exception.Message);
                        
                        event_result = null;
                    }

                    if (event_result != null)
                    {
                        CommaBlockString comma = new CommaBlockString();
                        comma.Set(event_result);

                        int trans = comma.GetInt();
                        if (trans != nAlarmEventTrans)
                        {
                            bAlarmEventChanged = true;
                            nAlarmEventTrans = trans;
                        }

                        nAlarmEventCount = comma.GetInt();
                    }
                }
            }
        }
    }
}
