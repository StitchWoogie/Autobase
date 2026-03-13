using System;
using AutoLibLocal;
using System.Threading;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// 태그 체크를 쓰레드에서 하여 메인의 부하를 좀 더 줄일 필요가 있다.
	/// </summary>
	public class CheckEngineTagChangeThread
	{
		public CheckEngineTagChangeThread()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        static Thread threadSharedTag;
        static Thread threadWriteCheck;

		public static void Init()
		{

            //   threadWriteCheck = new Thread(new ThreadStart(ThreadLoopWriteCheck));
            // threadWriteCheck.Start();

            threadSharedTag = new Thread(new ThreadStart(ThreadLoopSharedTag));
            threadSharedTag.Start();
        }

		public static void UnInit()
		{
            bEnd = true;

            //if (threadWriteCheck != null)   // 초기 로그인에서 실패시 걸림
            //{
            //	threadWriteCheck.Join(5000);
            //}

            if (threadSharedTag != null)   // 초기 로그인에서 실패시 걸림
            {
                threadSharedTag.Join(5000);
            }


            //srRandomID.UnInit();

            //TagPublicClass tp;

            //for (int i = 0; i < TagLib.tagListAll.Length; i++)
            //{
            //	tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);

            //	tp.sharedRegistry.UnInit();
            //}
        }

        public static bool bEnd = false;


        //      /// <summary>
        //      /// 레지스트리 기반
        //      /// </summary>
        //static void ThreadLoopWriteCheck()
        //{
        //	double ival;
        //	string sval;
        //	TagPublicClass tp;
        //	int i;
        //	TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();
        //	int nTagPos = 0;
        //	int oldid = -1;
        //	int newid = -1;
        //          string user, ip, computer;

        //	while(!bEnd) 
        //	{
        //		Thread.Sleep(10);

        //              if (ConfigRunMain.bShareTagValueBySharedMemory)
        //              {
        //                  CheckEngineTagChangeBySharedMemory.GetWriteItem();
        //              }

        //		//newid = TotalConfig.LoadRegAutoBaseConfig("TagShare", null, "RandomID", (int)-1);
        //              newid = srRandomID.Read("RandomID", (int)-1);

        //		if(newid == oldid)  // 외부에서 출력이 들어오지 않았을 경우
        //		{
        //			nTagPos++;
        //			nTagPos %= TagLib.tagListAll.Length;

        //			//if(FutureSharedTag.GetWriteitem(TagLib.tagListAll[nTagPos].tag, out ival, out sval))
        //                  if (FutureSharedTag.GetWriteitem(TagLib.tagListAll[nTagPos].ptr.sharedRegistry, out ival, out sval, out user, out ip, out computer))
        //			{
        //				tp = TagLib.GetStructPublic(TagLib.tagListAll[nTagPos]);
        //				tp.sWriteWaitValue = sval;
        //				tp.fWriteWaitValue = ival;
        //                      tp.sWriteUser = user;
        //                      tp.sWriteIP = ip;
        //                      tp.sWriteComputer = computer;
        //				tp.bWriteWait = true;
        //			}
        //		}
        //		else                // 외부에서 출력이 들어왔을 경우
        //		{
        //                  // 수동으로 출력한 태그를 우선적으로 검사한다. 2010.4.6(10.1.0.2) 부터 지원
        //                  string write_tag = TotalConfig.LoadRegAutoBaseConfig("TagShare", null, "WriteTag", "");

        //                  if (write_tag.Length > 0)
        //                  {
        //                      if (FutureSharedTag.GetWriteitem(write_tag, out ival, out sval, out user, out ip, out computer))
        //                      {
        //                          int[] tag_pos = new int[1];
        //                          tp = TagLib.GetStructPublic(write_tag, ref tag_pos);
        //                          tp.sWriteWaitValue = sval;
        //                          tp.fWriteWaitValue = ival;
        //                          tp.sWriteUser = user;
        //                          tp.sWriteIP = ip;
        //                          tp.sWriteComputer = computer;
        //                          tp.bWriteWait = true;
        //                      }
        //                  }

        //                  timeout.Reset();

        //			for(i = 0; i < TagLib.tagListAll.Length; i++) 
        //			{
        //				if(bEnd)	break;

        //				if(timeout.IsTimeOut(100)) 
        //				{
        //					Thread.Sleep(100);
        //					timeout.Reset();
        //				}

        //				nTagPos++;
        //				nTagPos %= TagLib.tagListAll.Length;

        //                      if (FutureSharedTag.GetWriteitem(TagLib.tagListAll[nTagPos].ptr.sharedRegistry, out ival, out sval, out user, out ip, out computer))
        //				//if(FutureSharedTag.GetWriteitem(TagLib.tagListAll[nTagPos].tag, out ival, out sval))
        //				{
        //					tp = TagLib.GetStructPublic(TagLib.tagListAll[nTagPos]);
        //					tp.sWriteWaitValue = sval;
        //					tp.fWriteWaitValue = ival;
        //                          tp.sWriteUser = user;
        //                          tp.sWriteIP = ip;
        //                          tp.sWriteComputer = computer;
        //					tp.bWriteWait = true;
        //				}
        //			}

        //			oldid = newid;
        //                  Thread.Sleep(1);
        //		}
        //	}
        //}

        static void ThreadLoopSharedTag()
        {
            TagPublicClass tp;
            int i;
            TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();

            while (!bEnd)
            {
                //timeout.SleepRemain();  // 남아 있는 시간을 쉬어준다.
                Thread.Sleep(100);

                timeout.Reset();
                // 태그값이 변경되면 Shared태그에 데이터를 저장해 준다.
                for (i = 0; i < TagLib.tagListAll.Length; i++)
                {
                    if (bEnd) break;

                    if (timeout.IsTimeOut(100))
                    {
                        Thread.Sleep(100);
                        timeout.Reset();
                    }

                    tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);

                    if (tp.bNeedSharedTagUpdate)
                    {
                        tp.bNeedSharedTagUpdate = false;

                        if (tp.enumTagType == EnumTagType.AI)
                        {
                            FutureSharedTag.SetCurrRegistryByThread(tp.sharedRegistry, ((TagAiClass)tp).curr);
                        }
                        else if (tp.enumTagType == EnumTagType.DI)
                        {
                            FutureSharedTag.SetCurrRegistryByThread(tp.sharedRegistry, ((TagDiClass)tp).curr);
                        }
                        else if (tp.enumTagType == EnumTagType.ST)
                        {
                            FutureSharedTag.SetCurrRegistryByThread(tp.sharedRegistry, ((TagStClass)tp).curr);
                        }
                        else if (tp.enumTagType == EnumTagType.AO)
                        {
                            FutureSharedTag.SetCurrRegistryByThread(tp.sharedRegistry, ((TagAoClass)tp).curr);
                        }
                        else if (tp.enumTagType == EnumTagType.DO)
                        {
                            FutureSharedTag.SetCurrRegistryByThread(tp.sharedRegistry, ((TagDoClass)tp).curr);
                        }

                        //Thread.Sleep(1);
                    }
                }
            }
        }

    }
}



