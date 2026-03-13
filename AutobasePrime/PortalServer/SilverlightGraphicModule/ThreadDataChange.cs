using System;
using System.Threading;
using AutoLib;
using System.Collections.Generic;
using NetTools;
using AutoLibLocal;
using System.Linq;
using System.Windows.Data;
using System.Xml.Linq;
using SilverlightAutoLibLocal.ServiceReferenceDataTag2;

namespace SilverlightGraphicModule
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
                
		static void GetCurrentValue(ArrayOfString array)
		{
            if (array.Count == 0) return;

            WebServiceDataTag2SoapClient service = new WebServiceDataTag2SoapClient(new System.ServiceModel.BasicHttpBinding(), DataGate.GetEndPoint("WebServiceDataTag2.asmx"));

            service.GetTagValueListsCompleted += new EventHandler<GetTagValueListsCompletedEventArgs>(service_GetTagValueListsCompleted);

            nCurrentReadingNo++;
            
            service.GetTagValueListsAsync(array, nCurrentReadingNo);

            timeoutCurrentValue.Reset();
            bCurrentReading = true;
        }

        static void service_GetTagValueListsCompleted(object sender, GetTagValueListsCompletedEventArgs e)
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

            XDocument x = XDocument.Parse(e.Result);

			string tag;
			int[] nTagPos = new int[1];
			TagAiClass ai;
			TagAoClass ao;
			TagDiClass di;
			TagDoClass dout;
			TagStClass st;
            TagPublicClass tp;

			string sval;

            XElement root = x.Element("Root");

			foreach(XElement el in root.Elements()) 
			{
                tag = el.Element("Tag").Value;
				tp = TagLib.GetStructPublic(tag, ref nTagPos);

                sval = el.Element("Curr").Value;

				if(tp.enumTagType == EnumTagType.AI) 
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

					if(val != ai.curr) 
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
					if(val != ao.curr) 
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
					if(val != di.curr) 
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
					if(val != dout.curr) 
					{
						dout.curr = val;
						dout.bChangedDataCurr = true;
					}
				}
                else if (tp.enumTagType == EnumTagType.ST) 
				{
					string val = sval;
                    st = (TagStClass)tp;
					if(val != st.curr) 
					{
						st.curr = val;
						st.bChangedDataCurr = true;
					}
				}
			}
		}

        /*

        static void RecurseCheckCurrentValue(TagGrClass gr, ArrayOfString array)
		{
			TagPublicClass tp;
			
			for(int i = 0; i < gr.arrayTag.Count; i++)
			{
				tp = (TagPublicClass)gr.arrayTag[i];
				
				if(tp.enumTagType == EnumTagType.GR) 
				{
					RecurseCheckCurrentValue((TagGrClass)tp, array);
				}
				else 
				{
					// 데이터 요구가 있고 전역 태그일 때
					if(tp.bNeedDataCurr && tp.bLocalTag == 0) 
					{
						array.Add(tp.tag);
						tp.bNeedDataCurr = false;

                        if (array.Count >= MAX_READ_BLOCK) 
						{
							GetCurrentValue(array);
							array.Clear();
						}
					}
				}
			}
		}

		static void CheckCurrentValue()
		{
			//if(!timeout.IsTimeOut(SharedData.userInfo.nWebValueUpdateTime))	return;
			timeout.Reset();

            ArrayOfString array = new ArrayOfString();

			array.Clear();

			RecurseCheckCurrentValue(TagLib.GetRootGroup(), array);

			if(array.Count > 0) 
			{
				GetCurrentValue(array);
				array.Clear();
			}
		}

		static void CheckElseValue()
		{
			
		}

		static public void ThreadMain()
		{
			while(bThreadContinue) 
			{
				Thread.Sleep(1);

                
				if(ConfigVarTotal.bLocalFlag) 
				{
					// CheckCurrentValueLocal();
				}
				else // Web Check
				{
					//if(WebCommInfo.nFailCount > 0) // Web 서비스와 통신 중 오류 발생
					//{
					//	DataGate.CheckServiceAlive();
					//}
					//else 
					//{
						//CheckCurrentValue();
					//	CheckElseValue();
					//}
				}
			}

			bThreadEnded = true;
		}*/

		static int nCurrentPos = 0;
        static TimeOutClass timeoutCurrentValue = new TimeOutClass();
        static bool bCurrentReading = false;        // 현재 읽기를 진행중이다.
        static int nCurrentReadingNo = 0;           // 

		static void CheckCurrentValueByTimer()
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
            ArrayOfString array = new ArrayOfString();

			int i;

			for(i = nCurrentPos; i < TagLib.tagListAll.Length; i++)
			{
				tp = TagLib.GetStructPublic(TagLib.tagListAll[i].tag, ref TagLib.tagListAll[i].tag_pos);
				
				// 데이터 요구가 있고 전역 태그일 때 스크롤바 기능중 마우스 누른상태에서는 값을 읽지 않는다.
				if(tp.bNeedDataCurr && tp.bLocalTag == 0 && !tp.bScrollBarMoving) 
				{
					array.Add(tp.tag);
					tp.bNeedDataCurr = false;

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

        public static void TimerMain()
        {
            CheckCurrentValueByTimer();
        }
	}
}
