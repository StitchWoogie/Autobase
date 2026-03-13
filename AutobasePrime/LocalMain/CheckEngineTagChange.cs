using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using GraphicModule;
using System.Threading.Tasks;
using Opc.Ua;
using OPCUA.Client.Core;
using LocalMain.OPCUA;
using OpcUaClient.Ipc.Contracts;
using OpcUaClient.Ipc.Client;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LocalMain
{
	/// <summary>
	/// Summary description for EngineTagCheck.
	/// </summary>
	public class CheckEngineTagChange
	{
        public static GraphicModule.ScriptClass scriptCalc = new GraphicModule.ScriptClass();

		static CheckEngineTagChange()
		{
			//
			// TODO: Add constructor logic here
			//
			DateTime st = DateTime.Now;
			old_milli_sec = st.Second*1000+st.Millisecond;

			tagList = TagLib.MakeTagList();

            scriptCalc.AddOneVarFromModX("double", "value", 1, "");
		}

		public static void InitSharedTag()
		{
            FutureSharedTag.InitializeServer(maxTags: tagList.Length);  //MMF + Pipe 시작
			FutureSharedTag.InitialSyncFromEngine();  // MMF 태그값 초기 동기화

            if (ConfigRunMain.bShareTagValueBySharedMemory)
            {
                CheckEngineTagChangeBySharedMemory.Init();
            }
		}

		public static void UnInitSharedTag()
		{
            if (ConfigRunMain.bShareTagValueBySharedMemory)
            {
                CheckEngineTagChangeBySharedMemory.UnInit();
            }

			FutureSharedTag.ShutdownServer();

		}

		static int order = 0;
		static int old_milli_sec;
		static TagListStruct[] tagList = null;
		static NetTools.TimeOutMiliSecClass timeoutDebugMode = new TimeOutMiliSecClass();

		public static async Task CheckSignalChange()
		{
            if (SharedLocalMain.bTestMode)
			{
				if(timeoutDebugMode.IsTimeOut(200)) 
				{
					timeoutDebugMode.Minus(200);
					TagPublicClass tp = TagLib.GetStructPublic(tagList[order]);
					switch(tp.enumTagType) 
					{
						case EnumTagType.AI:	await CheckSignalChangeDebugAI((TagAiClass)tp).ConfigureAwait(false); 	break;
						case EnumTagType.AO:	CheckSignalChangeDebugAO((TagAoClass)tp);	break;
						case EnumTagType.DI:	await CheckSignalChangeDebugDI((TagDiClass)tp).ConfigureAwait(false);	break;
						case EnumTagType.DO:	break;	// DO tag는 밑에서 통채로 한다.
					}
					order++;
					order %= tagList.Length;
					
					CheckSignalChangeDebugDO();
				}

				await CheckWriteFromSharedTagOnDebug();
			}
			else 
			{
				await CheckSignalChangeAll();
			}	
		}

		//-----------------------------------------------------------------------------
		//	디버거 모드일때 AI 전체 스트럭쳐를 검사해서 연관된 간접 태그를 변화시킨다.
		//-----------------------------------------------------------------------------

		static async Task UpdateIndirectTagDebugAI()
		{
			TagAiClass ai;
			TagAiClass assign_ai;
			int i;

			for(i = 0; i < tagList.Length; i++) 
			{
				ai = TagLib.GetStructAI(tagList[i]);
				if(ai.act == 0)									continue;
				if(ai.cTagLinkType != 3)						continue;	// 간접 태그
				if(ai.assign == null)							continue;	// 아직 Assign에 된적이 없다.
				if(ai.assign.pos[0] == TagLib.TAG_NOT_FOUND)	continue;	// 연관태그가 없다.
				if(ai.assign.pos == tagList[i].tag_pos_save)	continue;	// 자신의 태그가 연결되어 있다.

				ai = TagLib.GetStructAI(tagList[i]);
				assign_ai = TagLib.GetStructAI(ai.assign.tag, ref ai.assign.pos);

				await CheckSignalChangeOneAI(ai, assign_ai.curr);
			}
		}

		//------------------------------------------------------------------------------
		//	디버거 모드일때 AI 스트럭쳐를 순서적으로 조금씩 증가 시킨다.
		//------------------------------------------------------------------------------

		static async Task CheckSignalChangeDebugAI(TagAiClass ai)
		{
			if(SharedLocalMain.bScanPauseFlag == true)				return;

			if(ai.act == 0)			    return;
			if(ai.cTagLinkType == 2)	return;		        // 메모리태그
			if(ai.cTagLinkType == 3)	return;		        // 간접태그
			if((ai.wProtectFlags & EnumProtectFlag.SCAN) == EnumProtectFlag.SCAN)		return;		// 수동 기입중

			ai.old = ai.curr;

			double imsi = ai.curr+(float)1.0;
			if(imsi > ai.fFull || imsi < ai.fBase) 
			{
				imsi = ai.fBase;
			}

			ai.curr = imsi;

			await WorkOnTagChangedAI(ai);

			await UpdateIndirectTagDebugAI();
		}

		//-----------------------------------------------------------------------------
		//	디버거 모드일때 AO 스트럭쳐를 순서적으로 조금씩 증가 시킨다.
		//-----------------------------------------------------------------------------

		static void CheckSignalChangeDebugAO(TagAoClass ao)
		{
	
		}

		//-----------------------------------------------------------------------------
		//	디버거 모드일때 DI 스트럭쳐를 순서적으로 조금씩 증가 시킨다음 연관된 간접태그를 변화시킨다.
		//-----------------------------------------------------------------------------

		static async Task UpdateIndirectTagDebugDI()
		{
			TagDiClass di;
			TagDiClass assign_di;
			int i;

			for(i = 0; i < tagList.Length; i++) 
			{
				di = TagLib.GetStructDI(tagList[i]);
				if(di.act == 0)					    continue;
				if(di.cTagLinkType != 3)				continue;				// 간접 태그
				if(di.assign == null)				continue;					// 아직 Assign이 된적이 없다.
				if(di.assign.pos[0] == TagLib.TAG_NOT_FOUND)	continue;		// 연관태그가 없다.
				if(di.assign.pos == tagList[i].tag_pos_save)			continue;		// 자신의 태그가 연결되어 있다.

				assign_di = TagLib.GetStructDI(di.assign.tag, ref di.assign.pos);

				await CheckSignalChangeOneDI(di, assign_di.curr);		
			}
		}

		//-----------------------------------------------------------------------------
		//	디버거 모드일때 DI 스트럭쳐를 순서적으로 조금씩 증가 시킨다.
		//-----------------------------------------------------------------------------

		static async Task CheckSignalChangeDebugDI(TagDiClass di)
		{
			if(SharedLocalMain.bScanPauseFlag == true)		return;

			sbyte mask;

			if(di.act == 0)			return;
			if(di.cTagLinkType == 2)	return;		// 메모리 태그
			if(di.cTagLinkType == 3)	return;		// 간접 태그
			if((di.wProtectFlags & EnumProtectFlag.SCAN) == EnumProtectFlag.SCAN)		return;		// 수동 기입중

			if(di.curr == 1)		mask = 0;
			else					mask = 1;

			await CheckSignalChangeOneDI(di, mask).ConfigureAwait(false);

			await UpdateIndirectTagDebugDI();
		}

		//-----------------------------------------------------------------------------
		// 디버거 모드일 때 DO 스트럭쳐를 순서적으로 조금씩 증가 시킨다.
		//-----------------------------------------------------------------------------
		
		static void CheckSignalChangeDebugDO()
		{
			int i;
	
			TagPublicClass tp;

			//int sec_hap = 0;
			//DateTime t = DateTime.Now;

			//if(t.Second != nOldSecAtDO) 
			//{
			//	if(t.Second > nOldSecAtDO) 
			//	{
			//		sec_hap = t.Second-nOldSecAtDO;
			//	}
			//	else 
			//	{
			//		sec_hap = (t.Second+60)-nOldSecAtDO;
			//	}
			//	nOldSecAtDO = t.Second;
			//}

			for(i = 0; i < tagList.Length; i++) 
			{
				tp = TagLib.GetStructPublic(tagList[i]);

				if(tp.enumTagType == EnumTagType.DO) 
				{
                    //CheckSignalChangeDO((TagDoClass)tp, sec_hap);
                    CheckSignalChangeDO((TagDoClass)tp);
                }
				else {}
			}
		}

		// 값이 바뀌었을때 해야할 일 실제/디버거 동일
		static async Task WorkOnTagChangedAI(TagAiClass ai)
		{
			// 경보 상황 시 최고/최소 값을 기억해 둔다.
			if(ai.IsPowerFactorTag())	// 역률일때
			{
				if(ai.curr < 0 && ai.curr > ai.fAlarmMinValue)	ai.fAlarmMinValue = ai.curr;
				if(ai.curr >= 0 && ai.curr < ai.fAlarmMaxValue)	ai.fAlarmMaxValue = ai.curr;
			}
			else 
			{
				if(ai.curr < ai.fAlarmMinValue)	ai.fAlarmMinValue = ai.curr;
				if(ai.curr > ai.fAlarmMaxValue)	ai.fAlarmMaxValue = ai.curr;
			}

			await SubOutAnalogCheck(ai);
			await AnalogSubCheckHiHiLoLo(ai);
			AnalogAlarmLevelCheck(ai);	                // 아날로그 값의 레벨을 체크하고 경보와 메세지를 발생 시킨다.
			SendEventToChild.SendEventAIToChild(ai);
			CheckRateOfChangeLimit(ai);					// 범위 변화 제한값을 넘었는지를 체크한다.
			SharedDatabase.EventAI(ai);

            FutureSharedTag.SetCurr(ai, ai.curr);
			
            await Script.TagEventScript.Run(ai);
		}

		static int nTagPos;
		//static int nOldSecAtDO = 0;

        static async Task CheckSignalChangeAll()
		{
			int i;
	
			TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();
			TagPublicClass tp;

            //int sec_hap = 0;
            //DateTime t = DateTime.Now;

            //if(t.Second != nOldSecAtDO) 
            //{
            //	if(t.Second > nOldSecAtDO)
            //	{
            //		sec_hap = t.Second-nOldSecAtDO;
            //	}
            //	else 
            //	{
            //		sec_hap = (t.Second+60)-nOldSecAtDO;
            //	}
            //	nOldSecAtDO = t.Second;
            //}

            for (i = 0; i < tagList.Length; i++) 
			{
				if(timeout.IsTimeOut(100)) 
				{
					Thread.Sleep(1);
					break;
				}

				nTagPos++;
				nTagPos %= tagList.Length;

                tp = TagLib.GetStructPublic(tagList[nTagPos]);

                if (tp.act == 0)
                {
                    tp.bWriteWait = false;  // 비활성화 시 외부의 출력 명령은 무시한다.  10.1.1 부터 수정
                    continue;
                }

                if (tp.bNeedSendEventToChild)
                {
                    SharedViewMain.EventGoTagChanged(tp);
                    tp.bNeedSendEventToChild = false;
                }

				if(tp.bWriteWait) 
				{
					await PlcScan.SetTagValue(tp, tp.sWriteWaitValue, tp.fWriteWaitValue, false);
					tp.bWriteWait = false;

                    if (ConfigAlarm.bSaveRemoteControlResult)   // 데이터가 너무 많아서 저장 조건일 때만 저장하도록 수정하였다. 2016-1-15
                    {
                        string msg;
                        string value;

                        if (tp.enumTagType == EnumTagType.ST)
                            value = tp.sWriteWaitValue;
                        else
                            value = tp.fWriteWaitValue.ToString();

                        if (Tools.IsLangKorean())
                            msg = String.Format("원격 수동 제어 (Value={0})", value);
                        else
                            msg = String.Format("Remote Manual Control (Value={0})", value);

                        AlarmUtil.AlarmDataSave(tp, msg, EnumAlarmType.HAND_OPERATION, tp.sWriteUser, tp.sWriteIP, tp.sWriteComputer);
                    }
				}

				if(tp.enumTagType == EnumTagType.AI) 
				{
					await CheckSignalChangeAI((TagAiClass)tp);
                    if (OPCUAServerMain.bEnabled && OPCUAServerMain.bLoad) 
						OPCUAServerMain._server.NodeManager.UpdateItem(nTagPos, Convert.ToDouble(tp.GetCurr()), tp.tag); // OPC UA Server 테스트용

                }
                else if(tp.enumTagType == EnumTagType.DI) 
				{
					await CheckSignalChangeDI((TagDiClass)tp);
                    await CheckSignalChangeDIO((TagDiClass)tp);   // 출력 겸용으로 사용 시 체크해야 한다.
                    if (OPCUAServerMain.bEnabled && OPCUAServerMain.bLoad) 
						OPCUAServerMain._server.NodeManager.UpdateItem(nTagPos, Convert.ToSByte(tp.GetCurr()), tp.tag); // OPC UA Server 테스트용

                }
                else if(tp.enumTagType == EnumTagType.DO) 
				{
					CheckSignalChangeDO((TagDoClass)tp);
                    if (OPCUAServerMain.bEnabled && OPCUAServerMain.bLoad) 
						OPCUAServerMain._server.NodeManager.UpdateItem(nTagPos, Convert.ToSByte(tp.GetCurr()), tp.tag); // OPC UA Server 테스트용

                }
                else if(tp.enumTagType == EnumTagType.ST) 
				{
					await CheckSignalChangeST((TagStClass)tp);
                    if (OPCUAServerMain.bEnabled && OPCUAServerMain.bLoad) 
						OPCUAServerMain._server.NodeManager.UpdateItem(nTagPos, (string)(tp.GetCurr()), tp.tag); // OPC UA Server 테스트용

                }
                else if (tp.enumTagType == EnumTagType.AO) // OPC UA Server 테스트용 추가
                {

                    if (OPCUAServerMain.bEnabled && OPCUAServerMain.bLoad) 
						OPCUAServerMain._server.NodeManager.UpdateItem(nTagPos, Convert.ToDouble(tp.GetCurr()), tp.tag); // OPC UA Server 테스트용
                }
                else if (tp.enumTagType == EnumTagType.GDO) // OPC UA Server 테스트용 추가
                {

                    if (OPCUAServerMain.bEnabled && OPCUAServerMain.bLoad) 
						OPCUAServerMain._server.NodeManager.UpdateItem(nTagPos, Convert.ToSByte(tp.GetCurr()), tp.tag); // OPC UA Server 테스트용
                }
                else {}
			}
		}

        static Random rand = new Random();


        static bool GetOpcData(TagPublicClass tp, out string data)
        {
            const string sNoData = "?|_&%";

            // 레지스트리의 그룹안에 아이템이 많으면 1000개 이상 넘으면 (실제 오토 하이테크에서는 대우조선해양에 4500개 정도를 한 Opc Group에 적용했다.)
            // LoadReg 속도가 현저하게 떨어진다. (아마 파일 폴더와 비슷한 구조라서 파일이 많으면 속도가 떨어지는 현상과 비슷하다.)
            // 그래서 Opc/Goup/Sum  식으로 구분했다. 2016-4-27  확실히 속도가 엄청 개선된것을 확인했다.  256 폴더로 분리되었다.
            // 속도가 떨어진 현장은 HKEY_CURRENT_USER AutoBase/OpcData 를 삭제해 주고 다시 시작해 준다.
            //data = TotalConfig.LoadRegAutoBaseConfig("OpcData", tp.sOpcServer + "\\" + tp.sOpcGroup, tp.sOpcItem, sNoData);

            byte sum = SharedTag.GetSum8(tp.sOpcItem);
            //data = TotalConfig.LoadRegAutoBaseConfig("OpcData", tp.sOpcServer + "\\" + tp.sOpcGroup + "\\" + sum.ToString(), tp.sOpcItem, sNoData);

            if (tp.flagOpcUAClient == 1) // OPC UA 24-09-02 추가 hsjeong
            {
                //data = TotalConfig.LoadRegAutoBaseConfig("OpcUAData", tp.sOpcServer + "\\" + tp.sOpcGroup + "\\" + sum.ToString(), tp.sOpcItem, sNoData);

                return GetOpcUaData(tp, out data);
            }
            else
            {
                data = TotalConfig.LoadRegAutoBaseConfig("OpcData", tp.sOpcServer + "\\" + tp.sOpcGroup + "\\" + sum.ToString(), tp.sOpcItem, sNoData);
            }


            if (data == sNoData)
				return false;


            CommaTextReader reader = new CommaTextReader();

            reader.Set(data);

            int quality = 0;

            if (tp.flagOpcUAClient == 1) // OPC UA 24-09-02 추가 hsjeong
            {
                if (reader.GetString() == "Good")
                {
                    quality = 192;
                }
            }
            else reader.GetInt(ref quality);

            for (int i = 0; i < tp.nOpcItemPos; i++)
            {
                reader.Skip();
            }

            if (tp.flagOpcUAClient == 1) // OPC UA 24-09-02 추가 hsjeong
            {
                reader.GetString(ref data);
                // Good, 1, 1, "hello,world", index = Value.IndexOf(',');
            }
            else
                reader.GetStringTotalRemain(ref data);//reader.GetString(ref data); UA에서GetString array 위해else로분리
            
            

            int q1, q2, q3;
            q1 = (quality >> 6) & 0x03; // 6,7 bit
            q2 = (quality >> 2) & 0x0F; // 2,3,4,5 bit
            q3 = (quality >> 0) & 0x03; // 0,1 bit

            tp.DeviceQuality = (EnumDeviceQuality)q1;
            tp.DeviceSubStatus = (byte)q2;
            tp.DeviceLimit = (EnumDeviceLimit)q3;

            // Quality 상태가 바뀌면 화면을 갱신해 준다.
            if (tp.bDevideStatusChanged)
            {
                tp.bDevideStatusChanged = false;

                if (tp.enumTagType == EnumTagType.AI)
                {
                    SendEventToChild.SendEventAIToChild((TagAiClass)tp);
                }
                else if (tp.enumTagType == EnumTagType.DI)
                {
                    SendEventToChild.SendEventDIToChild((TagDiClass)tp);
                }
                else if (tp.enumTagType == EnumTagType.ST)
                {
                    SendEventToChild.SendEventSTToChild((TagStClass)tp);
                }
            }

            return true;
        }

		static bool GetOpcUaData(TagPublicClass tp, out string data)
		{
            data = null;

            //Invalid OPC UA tag → skip
            if (tp == null ||
			  string.IsNullOrWhiteSpace(tp.sOpcServer) ||
			  string.IsNullOrWhiteSpace(tp.sOpcGroup) ||
			  string.IsNullOrWhiteSpace(tp.sOpcItem))
            {
                return false;
            }

            string fullName = $"{ tp.sOpcServer}.{tp.sOpcGroup}.{ tp.sOpcItem}";

            // =====================================================
            // 1. Cache hit (Hot path)
            // =====================================================
            if (OpcUaClientCache.TryGet(fullName, out var c))
            {
                // 배열 index 적용
                if (!c.TryGetValueString(tp.nOpcItemPos, out data))
                    data = "";

                tp.DeviceQuality = c.DeviceQuality;
                tp.DeviceSubStatus = c.DeviceSubStatus;
                tp.DeviceLimit = c.DeviceLimit;

				RaiseDeviceStatusChanged(tp);

                return true;
            }

            // =====================================================
            // 2. Cache miss → Warmup(ReadOnce) 요청
            // =====================================================
            if (OpcUaClientCache.TryRequestWarmup(fullName))
            {
                OpcUaIpcManager.Client?.UpdateItemAsync(
                    tp.sOpcServer,
                    tp.sOpcGroup,
                    tp.sOpcItem);
            }

            // 2️ Cold Tag만 기존 Pull, Cache miss → 비동기 워밍업만 요청
            // LegacyGetOpcUaData(tp, out data);
            data = null;
            return false;
        }


        static void RaiseDeviceStatusChanged(TagPublicClass tp)
        {
			// Quality 상태가 바뀌면 화면을 갱신해 준다.
			if (tp.bDevideStatusChanged)
			{
				tp.bDevideStatusChanged = false;

				switch (tp.enumTagType)
				{
					case EnumTagType.AI:
						SendEventToChild.SendEventAIToChild((TagAiClass)tp);
						break;
					case EnumTagType.DI:
						SendEventToChild.SendEventDIToChild((TagDiClass)tp);
						break;
					case EnumTagType.ST:
						SendEventToChild.SendEventSTToChild((TagStClass)tp);
						break;
				}
			}
        }

    //    static bool LegacyGetOpcUaData(TagPublicClass tp, out string data)
    //    {
    //        data = null;

    //        //  UI 스레드 방어 (중요)
    //        if (SynchronizationContext.Current != null)
    //            return false;

    //        ReadItemResponse res = OpcUaIpcManager.Client?.ReadItem(
				//tp.sOpcServer,
				//tp.sOpcGroup,
				//tp.sOpcItem);

    //        if (res == null || res.ResultCode != IpcResultCode.Ok)
    //            return false;

    //        try
    //        {
    //            object val = res.Value;

    //            if (val == null)
    //            {
    //                data = "";
    //            }
    //            // 🔹 Array → index 적용
    //            else if (val is Array arr)
    //            {
    //                int idx = tp.nOpcItemPos;
    //                if (idx >= 0 && idx < arr.Length)
    //                {
    //                    object v = arr.GetValue(idx);
    //                    data = v != null ? v.ToString() : "";
    //                }
    //                else
    //                {
    //                    data = "";
    //                }
    //            }
    //            // 🔹 Matrix → LocalMain에서는 무시
    //            else if (val is Matrix)
    //            {
    //                data = "";
    //            }
    //            // 🔹 Scalar
    //            else
    //            {
    //                data = val.ToString();
    //            }
    //        }
    //        catch
    //        {
    //            data = "";
    //        }

    //        // ===== Quality 처리 (기존 그대로) =====
    //        int quality = res.Quality;

    //        int q1 = (quality >> 6) & 0x03;
    //        int q2 = (quality >> 2) & 0x0F;
    //        int q3 = (quality >> 0) & 0x03;

    //        tp.DeviceQuality = (EnumDeviceQuality)q1;
    //        tp.DeviceSubStatus = (byte)q2;
    //        tp.DeviceLimit = (EnumDeviceLimit)q3;

    //        RaiseDeviceStatusChanged(tp);

    //        return true;
    //    }



        //------------------------------------------------------------------------------
        //	수시로 검사하는 부분으로 값의 변화를 검사한다.
        //------------------------------------------------------------------------------

        static async Task CheckSignalChangeAI(TagAiClass ai)
		{
			if(SharedLocalMain.bScanPauseFlag == true)		return;

			double value_double = 0;
			double value_float = 0;
			ulong  real = 0;

			if(ai.cTagLinkType == 3) 
			{	// 간접 태그
				if(ai.assign == null)	return;
				if(ai.assign.pos[0] == TagLib.TAG_NOT_FOUND)	return;
				TagAiClass assign_ai = TagLib.GetStructAI(ai.assign.tag, ref ai.assign.pos);
				await CheckSignalChangeOneAI(ai, assign_ai.curr);
				return;
			}

			if((ai.wProtectFlags & EnumProtectFlag.SCAN) == EnumProtectFlag.SCAN)	return;		// 스캔 금지 중이다.

			int milli_sec;
			DateTime st;
		
			int milli_gab; 

			st = DateTime.Now;

			milli_sec = st.Second*1000+st.Millisecond;

			if(milli_sec < ai.nScanTimeOldMiliSec) 
			{
				milli_gab = (milli_sec+60000)-ai.nScanTimeOldMiliSec;
			}
			else 
			{
				milli_gab = milli_sec-ai.nScanTimeOldMiliSec;
			}

			ai.nScanTimeOldMiliSec = milli_sec;

			// scantime이 되지 않으면 scan을 하지 않는다.
			ai.wScanTimeCurr += (ushort)milli_gab;

			if(ai.wScanTimeCurr < ai.wScanTime)	return;
			ai.wScanTimeCurr = 0;

			if(ai.cTagLinkType == 0) 
			{
                if (!PlcScan.GetAiValueAtScanBuf(ai, ref real, ref value_float))
                {
                    CancelPreviewOutputResult(ai);
                    return;
                }

                if (ai.fn == 0)
                {   // WORD
                    if (ai.nMemoryTypeSub == 1) value_double = (int)(uint)real;
                    else if (ai.nMemoryTypeSub == 2) value_double = (int)(uint)real;
                    else if (ai.nMemoryTypeSub == 3) value_double = (uint)real;
                    else if (ai.nMemoryTypeSub == 4) value_double = (uint)real;
                    else if (ai.nMemoryTypeSub == 5) value_double = value_float;
                    else if (ai.nMemoryTypeSub == 6) value_double = value_float;
                    else value_double = (ushort)real;
                }
				else if(ai.fn == 1)	
				{
					value_double = PlcScan.CalcBcdValue(ai, Win32Function.HIBYTE((ushort)real));
				}
				else if(ai.fn == 2)	
				{
					value_double = PlcScan.CalcBcdValue(ai, Win32Function.LOBYTE((ushort)real));
				}
                else if (ai.fn == 3)
                {
                    value_double = value_float;
                }
				else if(ai.fn == 4)		value_double = (double)real;			// DWORD
				else if(ai.fn == 5)		value_double = (short)real;				//
				else if(ai.fn == 6)		value_double = (double)((int)real);	    //
				else if(ai.fn == 7)		value_double = (double)real;			// HIWORD&LOWORD
				else if(ai.fn == 8)		value_double = (double)real;			// LOWORD&HIWORD
				else if(ai.fn == 9)		value_double = (double)real;			// LODWORD&HIDWORD
                else if (ai.fn == 10)   value_double = (double)real;			// LODWORD&HIDWORD
                else if (ai.fn == 11)   value_double = value_float;			    // double
                else if (ai.fn == 12)   // int64
                {
                    if(ai.nMemoryTypeSub == 1)          value_double = (double)(ulong)real;			// uint64
                    else
                    {
                        value_double = (double)(long)real;  // int64
                    }
                }
                else
                {
                    value_double = (ushort)real;
                }
			}
			else if(ai.cTagLinkType == 1)	// DDE
			{
				StringBuilder data = new StringBuilder(256);

				if(C_DdeTag.DdeLibChangedItem(ai.dwDdeService, ai.dwDdeTopic, ai.dwDdeItem, data, 256))
				{
					
				}
				else 
				{
                    if (ai.bDdeRequest == 0)
                    {
                        CancelPreviewOutputResult(ai);
                        return;
                    }

                    if (!C_DdeTag.DdeLibRequestItem(ai.dwDdeService, ai.dwDdeTopic, ai.dwDdeItem, data, 256))
                    {
                        CancelPreviewOutputResult(ai);
                        return;
                    }
				}

				// 끝에 \r\n 문자를 걸러내기 위해서 CommaBlock을 사용한다.
				CommaBlockString comma = new CommaBlockString();
				comma.Set(data.ToString());

				comma.GetDouble(ref value_double);
				//real = (ulong)value_double;
			}
            else if (ai.cTagLinkType == 2)	// Memory
            {
                if (ai.nCalculateFilter == 5)
                {
                    value_double = 0;
                }
                else
                {
                    if (ai.bTagChangeFlag)  // 만약 태그 속성이 바뀌었으면 메모리 태그도 경보등을 처리한다. 2011-12-6
                        value_double = ai.curr;
                    else
                    {
                        // 메모리 태그인 경우 과변화 경보 조건이 되면 계속 경보를 호출해야 한다. 2017-9-20 추가
                        if (ai.nAlarmProtectOnBigChangeCount > 0)
                        {
                            AnalogAlarmLevelCheck(ai);
                        }

                        return;
                    }
                }
            }
            else if (ai.cTagLinkType == 5)	// OPC
            {
                string data;
                bool retn;
                retn = GetOpcData(ai, out data);

                if (retn == false)
                {
                    CancelPreviewOutputResult(ai);
                    return;
                }
                if (data == null)
                {
                    CancelPreviewOutputResult(ai);
                    return;	// 데이터를 아직 읽지 못했다.
                }

                if (String.Compare(data, "True", true) == 0)
                {
                    value_double = 1;
                }
                else if (String.Compare(data, "False", true) == 0)
                {
                    value_double = 0;
                }
                else
                {
                    value_double = ConvertTool.ToDouble(data);
                }

                //real = (ulong)value_double;
            }
            else
            {
                return;
            }

			await CheckSignalChangeOneAI(ai, value_double);

		}

        // WritePreDisplay 넣은 후 2015-9-3 이후
        // 이것이 잘되면 ai.sosu_old 변수는 클래스에서 삭제해도 된다.
        public static async Task CheckSignalChangeOneAI(TagAiClass ai, double val)
        {
            if (ai.act == 0) return;

            //ai.real_curr = val;
            //ai.old = ai.curr;
            //ai.sosu_old = ai.sosu_curr;

            double new_curr;

            if (ai.cTagLinkType == 2)
            {	// 메모리 태그
                if (ai.nCalculateFilter == 5)
                {
                    scriptCalc.SetVarValue(0, val, 0);
                    //scriptCalc.GetValueRecurse(ai.sCalcScript, out new_curr);
                    var (sucess, resultValue) = await scriptCalc.GetValueRecurseAsDouble(ai.sCalcScript).ConfigureAwait(false);
                    new_curr = resultValue;
                }
                else
                {
                    new_curr = val;
                }
            }
            else if (ai.cTagLinkType == 3)
            {	// 간접태그
                new_curr = val;
            }
            else
            { // plc_scan 태그이거나, DDE, OPC 태그일 때
                if (ai.nCalculateFilter == 0)
                {	// 비례 수식으로 계산한다.
                    double ratio = ai.fPlcFull - ai.fPlcBase;
                    if (ratio == 0.0)
                        new_curr = 0.0;
                    else
                    {
                        new_curr = ((double)val - ai.fPlcBase) * (ai.fFull - ai.fBase) / ratio + ai.fBase;
                    }
                }
                else if (ai.nCalculateFilter == 1)
                {	// 특별수식 |Cos(-90)|~|Cos(90)| @는 -90에서 90까지
                    double ratio = ai.fPlcFull - ai.fPlcBase;
                    if (ratio == 0.0)
                        new_curr = 0.0;
                    else
                    {
                        double degree = (val - ai.fPlcBase) * (180) / ratio + (-90);
                        if (degree < -90) degree = (-90.0);
                        if (degree > 90) degree = 90.0;

                        new_curr = Math.Cos(Math.Abs(MathLib.MathDegreeToRadian(degree)));
                        new_curr = new_curr * (ai.fFull - ai.fBase) + ai.fBase;
                        if (degree < 0) new_curr *= (-1);
                    }
                }
                else if (ai.nCalculateFilter == 3)
                {	// 비례 역률 계산.
                    double ratio = ai.fPlcFull - ai.fPlcBase;
                    if (ratio == 0.0)
                        new_curr = 0.0;
                    else
                    {
                        double mid = ratio / 2 + ai.fPlcBase;
                        if (val < mid)
                        {	// 비례 증가
                            new_curr = ((val - ai.fPlcBase) * 2) * (ai.fFull - ai.fBase) / ratio + ai.fBase;
                            new_curr *= -1;
                        }
                        else
                        {						// 비례 감소
                            val = val - ratio;
                            new_curr = ((val - ai.fPlcBase) * 2) * (ai.fFull - ai.fBase) / ratio + ai.fBase;
                            new_curr = ai.fFull - new_curr;
                        }
                    }
                }
                else if (ai.nCalculateFilter == 4)
                {	// 계측 역률 계산.
                    new_curr = val;
                }
                else if (ai.nCalculateFilter == 5)
                {	// 계산 스크립트
                    // new_curr = val;
                    scriptCalc.SetVarValue(0, val, 0);
                    //scriptCalc.GetValueRecurse(ai.sCalcScript, out new_curr);
                    var (sucess, resultValue) = await scriptCalc.GetValueRecurseAsDouble(ai.sCalcScript);
                    new_curr = resultValue;
                }
                else
                {	// 보통은 이 PLC값을 그대로 대입한다.
                    new_curr = val;
                }
            }

            double new_sosu_curr = new_curr;

            if (ai.nCalcDelay > 0)
            {	// 지난값과 평균을 내어서 계측치로 사용한다.
                if (ai.nCalcDelay > 1000) ai.nCalcDelay = 1000;
                new_curr = (float)((((double)ai.sosu_curr * (double)ai.nCalcDelay) + new_sosu_curr) / (ai.nCalcDelay + 1.0));
            }

            if (ConfigRunMain.bAiCalcSameFormat)
            {	// 표시형식과 계산값을 일치시킨다.
                string imsi = TagUtil.AiValueToString(ai, new_curr);
                new_curr = ConvertTool.ToDouble(imsi);
            }

            if (ai.bCutOverValue == 1)
            {	// RANGE 가 초과할 때는 잘라버리는 옵션
                if (new_curr < ai.fBase) new_curr = ai.fBase;
                if (new_curr > ai.fFull) new_curr = ai.fFull;
            }
            else if (ai.bCutOverValue == 2) // 값 취소  2013-2-18 AI에서 이부분이 빠져서 추가했다.
            {
                if (new_curr < ai.fBase || new_curr > ai.fFull)
                {
					return; //260224 PSU 추가. return 이 누락되었음.
                }
            }

            new_sosu_curr = new_curr;
            ai.sosu_curr = new_sosu_curr;

            if (ai.previewOutputResult != null)
            {
                if (new_curr != ai.previewOutputResult.new_value)
                {
                    if (ai.previewOutputResult.timeout.IsTimeOut(ConfigRunMain.nPreviewOutputResultSeconds))
                    {
                        ai.real_curr = val;
                        ai.old = ai.curr;
                        ai.curr = new_curr;     // 시간안에 값이 원하는 값으로 오지 않으면 현재 측정된 값으로 한다.
                        ai.previewOutputResult = null;
                        // 원래값으로 돌렸기 때문에 화면을 갱신하도록 이벤트를 보내준다.
                        SendEventToChild.SendEventAIToChild(ai);
                    }
                    return;
                }
            }
            else
            {
                if (ai.curr == new_curr && ai.bTagChangeFlag == false)
                {		// 값이 변동이 없다.

                    // 값이 바뀌지 않았지만 과변화 경보 금지가 되어 있으면 경보 호출을 해주어야 한다.
                    // 이 것은 과변화가 생기고 난 후 아날로그 값이 변동이 없으면 경보함수가 호출이 되지 않기 때문에 아래 부분의 조건이 추가되었다. 
                    // 과변화 금지시간이 0이 될때까지는 체크해 주어야 한다.
                    if (ai.cAlarmProtectOnBigChangePercent != 0 && ai.nAlarmProtectOnBigChangeSecond != 0 &&
                        ai.nAlarmProtectOnBigChangeCount != 0)
                    {
                        AnalogAlarmLevelCheck(ai);		// 아날로그 값의 레벨을 체크하고 경보와 메세지를 발생 시킨다.
                    }
                    return;
                }
            }

            ai.real_curr = val;
            ai.old = ai.curr;
            ai.curr = new_curr;
            ai.previewOutputResult = null;

            await WorkOnTagChangedAI(ai);

            ai.bTagChangeFlag = false;	// 값이 바뀌지 않는 한은 다시 표시할 필요가 없다.
        }


		//------------------------------------------------------------------------------------
		//	디지털 입력에 변화가 있는지 수시로 검사한다.
		//------------------------------------------------------------------------------------

		public static async Task CheckSignalChangeOneDI(TagDiClass di, sbyte flag)
		{
			if(di.act  == 0)		return;

            if (di.previewOutputResult != null)
            {
                if (flag != di.previewOutputResult.new_value)
                {
                    if (di.previewOutputResult.timeout.IsTimeOut(ConfigRunMain.nPreviewOutputResultSeconds))
                    {
                        di.curr = flag;
                        di.previewOutputResult = null;
                        // 원래값으로 돌렸기 때문에 화면을 갱신하도록 이벤트를 보내준다.
                        SendEventToChild.SendEventDIToChild(di);
                    }
                    return;
                }

                di.curr = flag;
                di.previewOutputResult = null;
            }
            else
            {
                if (di.curr == flag) return;

                di.curr = flag;
            }

			await WorkOnTagChangedDI(di);
		}

		// 값이 바뀌었을때 해야할 일 실제/디버거 동일
		static async Task WorkOnTagChangedDI(TagDiClass di)
		{
			//di.count_on_off++;

			DateTime t = DateTime.Now;

			if(di.curr == 1) 
			{
				di.startOnSec = (sbyte)t.Second;
                di.prevTime = t;   // 20241219 PSU 추가.

                di.count_on_off++; // 20241219 PSU ON시만 카운팅.
			}
			else 
			{
				//di.cOnTime += (sbyte)(t.Second-di.startOnSec+1);

                //1분 이상 차이나거나 분이 다를 때(계속 ON or 재시작) , 현재시간 - 0 + 1;  1 ~ 60 사이.  20241219 PSU 수정.
                if ((t - di.prevTime).TotalMinutes > 1 || t.Minute != di.prevTime.Minute)
                {
                     di.cOnTime += (ushort)((t.Second - di.startOnSec) * 1000 + t.Millisecond);
                }
                else //1분 사이에 ON/OFF 변경 시
                {
                    ushort elapsedMilliseconds = (ushort)(t - di.prevTime).TotalMilliseconds; 

                    // 밀리초 단위로 지속 시간 계산 후 자료저장 시 초로 변경.
                    di.cOnTime += elapsedMilliseconds;
                }
			}

			await DISubDigitalOutOnTagCheck(di).ConfigureAwait(false);	// 출력할 디지털 출력 태그가 있는가를 검사한다.
			await DISubDigitalOutOffTagCheck(di);	// 출력할 디지털 출력 태그가 있는가를 검사한다.

			DigitalAlarmCheck(di);

			SendEventToChild.SendEventDIToChild(di);

			CheckEngineOnOffList.Save(di, false);

			SharedDatabase.EventDI(di);

            FutureSharedTag.SetCurr(di, di.curr);

            await Script.TagEventScript.Run(di);

			di.tEvent = DateTime.Now;
		}

        // 통신이 되지 않아서 값을 읽을 수 없거나 시간초과 되었을 때 원래값으로 돌려준다.
        static void CancelPreviewOutputResult(TagDiClass di)
        {
            if (di.previewOutputResult == null) return;

            di.curr = di.previewOutputResult.old_value;
            di.previewOutputResult = null;
            // 원래값으로 돌렸기 때문에 화면을 갱신하도록 이벤트를 보내준다.
            SendEventToChild.SendEventDIToChild(di);            
        }

        static void CancelPreviewOutputResult(TagAiClass ai)
        {
            if (ai.previewOutputResult == null) return;

            ai.curr = ai.previewOutputResult.old_value;
            ai.previewOutputResult = null;
            // 원래값으로 돌렸기 때문에 화면을 갱신하도록 이벤트를 보내준다.
            SendEventToChild.SendEventAIToChild(ai);
        }

		//------------------------------------------------------------------------------------
		//	디지털 입력에 변화가 있는지 수시로 검사한다.
		//------------------------------------------------------------------------------------

		static async Task CheckSignalChangeDI(TagDiClass di)
		{
			if(SharedLocalMain.bScanPauseFlag == true)		return;

			int wordpos;
			sbyte mask = 0;
			ushort word_value = 0;

			if(di.cTagLinkType == 3) 
			{	// 간접 태그
				if(di.assign == null)	return;
				if(di.assign.pos[0] == TagLib.TAG_NOT_FOUND)			return;
				TagDiClass assign_di = TagLib.GetStructDI(di.assign.tag, ref di.assign.pos);
				await CheckSignalChangeOneDI(di, assign_di.curr);
				return;
			}

			if((di.wProtectFlags & EnumProtectFlag.SCAN) == EnumProtectFlag.SCAN)		return;		// 수동기입 중이다.

			int milli_sec;
			DateTime st;
		
			int milli_gab; 

			st = DateTime.Now;

			milli_sec = st.Second*1000+st.Millisecond;

			if(milli_sec < di.nScanTimeOldMiliSec) 
			{
				milli_gab = (milli_sec+60000)-di.nScanTimeOldMiliSec;
			}
			else 
			{
				milli_gab = milli_sec-di.nScanTimeOldMiliSec;
			}

			di.nScanTimeOldMiliSec = milli_sec;

			// scantime이 되지 않으면 scan을 하지 않는다.
			di.wScanTimeCurr += (ushort)milli_gab;

			if(di.wScanTimeCurr < di.wScanTime)	return;
			di.wScanTimeCurr = 0;

			if(di.cTagLinkType == 0) 
			{	// PLC_SCAN 태그
				wordpos = (int)di.address_word;
                if (!PlcScan.GetDiValueAtScanBuf(di, di.port, di.station, wordpos, ref word_value))
                {
                    CancelPreviewOutputResult(di);
                    return;// 0 = from WORD buf
                }

                // 주소가 0이하인 경우가 있다. 201-10-17
				if(di.address_bit >= 0 && di.address_bit < 16) 
				{
					if((word_value & Tools.WORD_MASK[di.address_bit%16]) > 0) 	mask = 1;
					else													mask = 0;
				}
				else 
				{
					mask = 0;
				}

				if(di.bReverse == 1) 
				{	// 반전한다.
					mask = (mask == 1) ? (sbyte)0 : (sbyte)1;
				}
			}
			else if(di.cTagLinkType == 1) 
			{	// DDE tag
				StringBuilder data = new StringBuilder(256);

				if(C_DdeTag.DdeLibChangedItem(di.dwDdeService, di.dwDdeTopic, di.dwDdeItem, data, 256))
				{

				}
				else 
				{
                    if (di.bDdeRequest == 0)
                    {
                        CancelPreviewOutputResult(di);
                        return;
                    }


                    if (!C_DdeTag.DdeLibRequestItem(di.dwDdeService, di.dwDdeTopic, di.dwDdeItem, data, 256))
                    {
                        CancelPreviewOutputResult(di);
                        return;
                    }
				}

				// 끝에 \r\n 문자를 걸러내기 위해서 CommaBlock을 사용한다.
				CommaBlockString comma = new CommaBlockString();
				comma.Set(data.ToString());

				comma.GetChar(ref mask);
			}
			else if(di.cTagLinkType == 4) 
			{	// 시스템 태그
				mask = (sbyte)SystemStatusMemory.GetDI((int)(di.address_word*16+di.address_bit));
			}
			else if(di.cTagLinkType == 5)	// OPC
			{
                string data;
                bool retn;
                double value_double;

                retn = GetOpcData(di, out data);

                if (retn == false)
                {
                    CancelPreviewOutputResult(di);
                    return;
                }
                if (data == null)
                {
                    CancelPreviewOutputResult(di);
                    return;	// 데이터를 아직 읽지 못했다.
                }

                if (String.Compare(data, "True", true) == 0)
                {
                    value_double = 1;
                }
                else if (String.Compare(data, "False", true) == 0)
                {
                    value_double = 0;
                }
                else
                {
                    value_double = ConvertTool.ToDouble(data);
                }

				mask = (value_double == 0) ? (sbyte)0 : (sbyte)1;
			}
			else 
			{
				mask = di.curr;	
			}


            /* CheckSignalChangeOneDI 속에 이부분을 검사한다. 2015-9-2 제거
            if(di.curr == mask) 
            {
                return;
            }*/

			await CheckSignalChangeOneDI(di, mask);
		}

		static void CheckSignalChangeDO(TagDoClass dout)
		{
            //-----------------------------------
            // 지정 시간 후에 출력을 ON/OFF한다.
            //-----------------------------------

            // 지연 동작 없음
            if (dout.cDelayOutputMethod == 0)
                return;

            // 실행 시각 없음
            if (!dout.DelayExecuteAtUtc.HasValue)
                return;

            // 아직 시간이 안 됨
            if (DateTime.UtcNow < dout.DelayExecuteAtUtc.Value)
                return;

            // ---- 여기까지 왔으면 "실행 시점 도달" ----

            if (dout.cDelayOutputMethod == 1)
            {
                PlcScan.CommWriteLocalDigitalOutput(dout, 1, false);
            }
            else if (dout.cDelayOutputMethod == 2)
            {
                PlcScan.CommWriteLocalDigitalOutput(dout, 0, false);
            }

            // 상태 초기화 (재진입 방지)
            dout.cDelayOutputMethod = 0;
            dout.DelayExecuteAtUtc = null;

            //if(dout.cDelayOutputMethod == 1) 
            //{			// 지정시간 뒤에 출력을 ON 한다.
            //	dout.nDelayOutputSec -= (short)sec_hap;
            //	if(dout.nDelayOutputSec < 0) 	dout.nDelayOutputSec = 0;

            //	if(dout.nDelayOutputSec <= 0) 
            //	{
            //		if( PlcScan.CommWriteLocalDigitalOutput(dout, 1, false)) 
            //		{
            //			dout.cDelayOutputMethod = 0;
            //		}
            //	}
            //}
            //else if(dout.cDelayOutputMethod == 2) 
            //{	// 지정시간 뒤에 출력을 OFF 한다.

            //	dout.nDelayOutputSec -= (short)sec_hap;
            //	if(dout.nDelayOutputSec < 0) 	dout.nDelayOutputSec = 0;
            //	if(dout.nDelayOutputSec <= 0) 
            //	{
            //		if( PlcScan.CommWriteLocalDigitalOutput(dout, 0, false)) 
            //		{
            //			dout.cDelayOutputMethod = 0;
            //		}
            //	}
            //}
            //else 
            //{	// 지정 시간 뒤에 출력은 없다.

            //}
        }

        static async Task CheckSignalChangeDIO(TagDiClass di)
        {
            if (di.bUseAsOutput == 0) return;   // 출력 겸용이 아니다.

            //-----------------------------------
            // 지정 시간 후에 출력을 ON/OFF한다.
            //-----------------------------------

            // 지연 동작 없음
            if (di.writeDo.cDelayOutputMethod == 0)
                return;

            // 실행 시각 없음
            if (!di.writeDo.DelayExecuteAtUtc.HasValue)
                return;

            // 아직 시간이 안 됨
            if (DateTime.UtcNow < di.writeDo.DelayExecuteAtUtc.Value)
                return;

            if (di.writeDo.cDelayOutputMethod == 1)
            {
                await PlcScan.CommWriteToLocalDIO(di, 1, false);
            }
            else if (di.writeDo.cDelayOutputMethod == 2)
            {
                await PlcScan.CommWriteToLocalDIO(di, 0, false);
            }

            // 상태 초기화 (재진입 방지)
            di.writeDo.cDelayOutputMethod = 0;
            di.writeDo.DelayExecuteAtUtc = null;

            //if (di.writeDo.cDelayOutputMethod == 1)
            //{			// 지정시간 뒤에 출력을 ON 한다.
            //    di.writeDo.nDelayOutputSec -= (short)sec_hap;
            //    if (di.writeDo.nDelayOutputSec < 0) di.writeDo.nDelayOutputSec = 0;

            //    if (di.writeDo.nDelayOutputSec <= 0)
            //    {
            //        if (await PlcScan.CommWriteToLocalDIO(di, 1, false))
            //        {
            //            di.writeDo.cDelayOutputMethod = 0;
            //        }
            //    }
            //}
            //else if (di.writeDo.cDelayOutputMethod == 2)
            //{	// 지정시간 뒤에 출력을 OFF 한다.

            //    di.writeDo.nDelayOutputSec -= (short)sec_hap;
            //    if (di.writeDo.nDelayOutputSec < 0) di.writeDo.nDelayOutputSec = 0;
            //    if (di.writeDo.nDelayOutputSec <= 0)
            //    {
            //        if (await PlcScan.CommWriteToLocalDIO(di, 0, false))
            //        {
            //            di.writeDo.cDelayOutputMethod = 0;
            //        }
            //    }
            //}
            //else
            //{	// 지정 시간 뒤에 출력은 없다.

            //}
        }

		static async Task CheckSignalChangeST(TagStClass st)
		{
            if (st.cTagLinkType == 3)
            {	// 간접 태그
                if (st.assign == null) return;
                if (st.assign.pos[0] == TagLib.TAG_NOT_FOUND) return;
                TagStClass assign_st = TagLib.GetStructST(st.assign.tag, ref st.assign.pos);
                await ChangeStringTagValue(st, assign_st.curr);
                return;
            }

			if(SharedLocalMain.bScanPauseFlag == true)		return;

			string val = "";

			if(st.cTagLinkType == 0) 
			{
				if(!PlcScan.GetStValueAtScanBuf(st, out val))	return;

				await ChangeStringTagValue(st, val);
			}
			else if(st.cTagLinkType == 1) 
			{
				StringBuilder data = new StringBuilder(256);

				if(C_DdeTag.DdeLibChangedItem(st.dwDdeService, st.dwDdeTopic, st.dwDdeItem, data, 256))
				{

				}
				else 
				{
					if(st.bDdeRequest == 0)	return;

					if(!C_DdeTag.DdeLibRequestItem(st.dwDdeService, st.dwDdeTopic, st.dwDdeItem, data, 256))
						return;
				}

				CommaBlockString comma = new CommaBlockString();
				comma.SetBlockCode((char)EnumAsciiCode.CR);
				comma.Set(data.ToString());

				comma.GetString(ref val);

				await ChangeStringTagValue(st, val);
			}
			else if(st.cTagLinkType == 5)	// OPC
			{
                string data;
                bool retn;
                retn = GetOpcData(st, out data);

                if (retn == false) return;
                if (data == null) return;	// 데이터를 아직 읽지 못했다.

				await ChangeStringTagValue(st, data);
			}
		}

		public static async Task ChangeStringTagValue(TagStClass st, string val)
		{
			if(st.act == 0)	return;

			if(st.curr == val)	return;	// same

			st.curr = val;

			if(st.cTagLinkType == 2)	// 메모리 태그
			{ 
				//void SendCommandToNetWorkTagMemberChangedString(char *tag, WORD member, char *string);
				//SendCommandToNetWorkTagMemberChangedString(st.tag, TAG_MEMBER_curr, st.curr);
			}
            else if (st.cTagLinkType == 3)  // 간접 태그
            {

            }

			SendEventToChild.SendEventSTToChild(st);
			SharedDatabase.EventST(st);

            FutureSharedTag.SetCurr(st, st.curr);

            await Script.TagEventScript.Run(st);
		}

		//----------------------------------------------------------------------------
		//	아나로그 입력이 바뀌었을 때 이부분을 부르게 되는데,
		//	동시에 아나로그 출력을 행하는 부분이다.
		//----------------------------------------------------------------------------

		static async Task SubOutAnalogCheck(TagAiClass ai)
		{
			if(ai.nSubOutAnalog[0] == TagLib.TAG_NOT_FOUND)			return;	// 설정된 아날로그 출력 태그 없슴.

			TagAoClass ao = TagLib.GetStructAO(ai.sSubOutAnalog, ref ai.nSubOutAnalog);

			if(ai.nSubOutAnalog[0] == TagLib.TAG_NOT_FOUND)			return;	// 설정된 아날로그 출력 태그 없슴.

			await PlcScan.CommWriteAnalogOutput(ao, ai.curr, false);
		}

		// Debug 모드일때 사용한다.
		static int nPosCheckWriteFromSharedTag = 0;
		static async Task CheckWriteFromSharedTagOnDebug()
		{
			TagPublicClass tp;
			int i;
			//double ival;
			//string sval;
			TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();

			for(i = 0; i < tagList.Length; i++, nPosCheckWriteFromSharedTag++) 
			{
				//Thread.Sleep(1);
				if(timeout.IsTimeOut(50))	break;

				nPosCheckWriteFromSharedTag %= tagList.Length;

				tp = TagLib.GetStructPublic(tagList[nPosCheckWriteFromSharedTag]);

                if (tp.bNeedSendEventToChild)
                {
                    SharedViewMain.EventGoTagChanged(tp);
                    tp.bNeedSendEventToChild = false;
                }

                if (tp.bWriteWait)
                {
                    await PlcScan.SetTagValue(tp, tp.sWriteWaitValue, tp.fWriteWaitValue, false);
                    tp.bWriteWait = false;

                    if (ConfigAlarm.bSaveRemoteControlResult)   // 데이터가 너무 많아서 저장 조건일 때만 저장하도록 수정하였다. 2016-1-15
                    {
                        string msg;
                        string value;

                        if (tp.enumTagType == EnumTagType.ST)
                            value = tp.sWriteWaitValue;
                        else
                            value = tp.fWriteWaitValue.ToString();

                        if (Tools.IsLangKorean())
                            msg = String.Format("원격 수동 제어 (Value={0})", value);
                        else
                            msg = String.Format("Remote Manual Control (Value={0})", value);

                       AlarmUtil.AlarmDataSave(tp, msg, EnumAlarmType.HAND_OPERATION, tp.sWriteUser, tp.sWriteIP, tp.sWriteComputer);
                    }
                }

			}
		}
		
		//-------------------------------------------------------------------------------------
		//	아날로그 레벨을 체크하여 HiHi DO와 LoLo DO의 출력을 행한다.
		//-------------------------------------------------------------------------------------

		static async Task AnalogSubCheckHiHiLoLo(TagAiClass ai)
		{
			if(ai.curr >= ai.hihi) 
			{
				if(ai.cSubCheckLevelStatus != EnumAnalogLevel.HIHI)
				{
					await AISubDigitalOutHiHi(ai, 1);
					await AISubDigitalOutLoLo(ai, 0);
					ai.cSubCheckLevelStatus = EnumAnalogLevel.HIHI;
				}
			}
			else if(ai.curr <= ai.lolo) 
			{
				if(ai.cSubCheckLevelStatus != EnumAnalogLevel.LOLO) 
				{
					await AISubDigitalOutHiHi(ai, 0);
					await AISubDigitalOutLoLo(ai, 1);
					ai.cSubCheckLevelStatus = EnumAnalogLevel.LOLO;
				}
			}
			else 
			{
				if(ConfigRunMain.bMatchAlarmReturnGabAndHiHiLoLoDO) 
				{
					if(ai.cSubCheckLevelStatus == EnumAnalogLevel.HIHI) 
					{
						if(ai.curr < ai.hihi-ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							await AISubDigitalOutHiHi(ai, 0);
							await AISubDigitalOutLoLo(ai, 0);
							ai.cSubCheckLevelStatus = EnumAnalogLevel.NORMAL;
						}
					}
					else if(ai.cSubCheckLevelStatus == EnumAnalogLevel.LOLO) 
					{
						if(ai.curr > ai.lolo+ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							await AISubDigitalOutHiHi(ai, 0);
							await AISubDigitalOutLoLo(ai, 0);
							ai.cSubCheckLevelStatus = EnumAnalogLevel.NORMAL;
						}
					}
					else {}
				}
				else 
				{
					if(ai.cSubCheckLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						await AISubDigitalOutHiHi(ai, 0);
						await AISubDigitalOutLoLo(ai, 0);
						ai.cSubCheckLevelStatus = EnumAnalogLevel.NORMAL;
					}
				}
			}
		}

		//---------------------------------------------------------------------------
		// 아나로그 태그에 설정된 HiHi 일때 출력할 DO를 ON/OFF 한다.
		//---------------------------------------------------------------------------

		static async Task AISubDigitalOutHiHi(TagAiClass ai, sbyte flag)
		{
			if(ai.nSubOutDigitalHiHi[0] == TagLib.TAG_NOT_FOUND)	return;	// 설정된 출력 태그 없습니다.

			TagPublicClass tp = TagLib.GetStructPublic(ai.sSubOutDigitalHiHi, ref ai.nSubOutDigitalHiHi);

			if(ai.nSubOutDigitalHiHi[0] == TagLib.TAG_NOT_FOUND)	return;	// 한번 더 검사한다.

			await PlcScan.SetTagValue(tp, "", flag, false);
		}

		//------------------------------------------------------------------------------
		// 아나로그 태그에 설정된 LoLo 일때 출력할 DO를 ON/OFF 한다.
		//------------------------------------------------------------------------------

		static async Task AISubDigitalOutLoLo(TagAiClass ai, sbyte flag)
		{
			if(ai.nSubOutDigitalLoLo[0] == TagLib.TAG_NOT_FOUND)	return;	// 설정된 출력 태그 없습니다.

			TagPublicClass tp = TagLib.GetStructPublic(ai.sSubOutDigitalLoLo, ref ai.nSubOutDigitalLoLo);

			if(ai.nSubOutDigitalLoLo[0] == TagLib.TAG_NOT_FOUND)	return;	// 한번 더 검사한다.

			await PlcScan.SetTagValue(tp, "", flag, false);
		}

		//------------------------------------------------------------------------------
		//	디지털 입력이 ON 일때 ON, OFF일때 OFF하는 디지털 출력 태그를 검사한다.
		//------------------------------------------------------------------------------

		static async Task DISubDigitalOutOnTagCheck(TagDiClass di)
		{
			if(di.nSubOutDigitalOnTag[0] == TagLib.TAG_NOT_FOUND)	return;	// 설정된 출력 태그 없습니다.
			TagPublicClass tp = TagLib.GetStructPublic(di.sSubOutDigitalOnTag, ref di.nSubOutDigitalOnTag);
			if(di.nSubOutDigitalOnTag[0] == TagLib.TAG_NOT_FOUND)	return;	// 한번 더 검사한다.

			await PlcScan.SetTagValue(tp, "", di.curr, false).ConfigureAwait(false);
		}

		//------------------------------------------------------------------------------
		//	디지털 입력이 ON 일때 OFF, OFF일때 ON하는 디지털 출력 태그를 검사한다.
		//------------------------------------------------------------------------------

		static async Task DISubDigitalOutOffTagCheck(TagDiClass di)
		{
			if(di.nSubOutDigitalOffTag[0] == TagLib.TAG_NOT_FOUND)	return;	// 설정된 출력 태그 없습니다.

			TagPublicClass tp = TagLib.GetStructPublic(di.sSubOutDigitalOffTag, ref di.nSubOutDigitalOffTag);

			if(di.nSubOutDigitalOffTag[0] == TagLib.TAG_NOT_FOUND)	return;	// 한번 더 검사한다.

			if(di.curr == 1)
				await PlcScan.SetTagValue(tp, "", 0, false).ConfigureAwait(false);
			else
				await PlcScan.SetTagValue(tp, "", 1, false).ConfigureAwait(false);
		}

		static void CheckAlarmOnPowerFactor(TagAiClass ai)
		{
			if(ai.cAlarmType == 0) 
			{	// HiHi 이상 LoLo이하일때 경보검사

				// 양수일때는 hihi보다 작은 값이 hihi경보이다.
				if(ai.curr >= 0 && ai.curr <= ai.hihi) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIHI) 
					{
						AnalogAlarmSet_HIHI(ai);
					}
				}
				// 음수일때는 lolo보다 큰 값이 lolo경보이다.
				else if(ai.curr < 0 && ai.curr >= ai.lolo) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOLO) 
					{
						AnalogAlarmSet_LOLO(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIHI) 
						{
							if(ai.curr > ai.hihi+ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, true);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
						else 
						{	// EnumAnalogLevel.LOLO
							if(ai.curr < ai.lolo-ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, false);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
					}
				}
			}
			else if(ai.cAlarmType == 1) 
			{	// HiHi 이상
				if(ai.curr >= 0 && ai.curr <= ai.hihi) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIHI) 
					{
						AnalogAlarmSet_HIHI(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr > ai.hihi+ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, true);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 2) 
			{	// LoLo이하일때 경보검사
				if(ai.curr < 0 && ai.curr >= ai.lolo) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOLO) 
					{
						AnalogAlarmSet_LOLO(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr < ai.lolo-ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, false);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 3) 
			{	// High 이상 Low이하일때 경보검사
				if(ai.curr >= 0 && ai.curr <= ai.high) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIGH) 
					{
						AnalogAlarmSet_HIGH(ai);
					}
				}
				else if(ai.curr < 0 && ai.curr >= ai.low) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOW) 
					{
                        AnalogAlarmSet_LOW(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIGH) 
						{
							if(ai.curr > ai.high+ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, true);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
						else 
						{
							if(ai.curr < ai.low-ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, false);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
				
					}
				}
			}
			else if(ai.cAlarmType == 4) 
			{	// High 이상 일때 경보검사
				if(ai.curr >= 0 && ai.curr <= ai.high) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIGH) 
					{
						AnalogAlarmSet_HIGH(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr > ai.high+ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, true);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 5) 
			{	// Low이하일때 경보검사
				if(ai.curr < 0 && ai.curr >= ai.low) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOW) 
					{
                        AnalogAlarmSet_LOW(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr < ai.low-ai.fAlarmReturnGab) 
						{ // 경보 범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, false);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 6) 
			{	// HiHi High, Low, LoLo이하
				if(ai.curr >= 0 && ai.curr <= ai.hihi) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIHI) 
					{
						AnalogAlarmSet_HIHI(ai);
					}
				}
				else if(ai.curr < 0 && ai.curr >= ai.lolo) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOLO) 
					{
						AnalogAlarmSet_LOLO(ai);
					}
				}
				else if(ai.curr >= 0 && ai.curr <= ai.high) 
				{
					if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIHI) 
					{	// HiHi에서 복귀
						AnalogAlarmSet_ReturnHIHI_HIGH(ai);
					}
					else if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIGH) 
					{	// 경보 발생.
						AnalogAlarmSet_HIGH(ai);
					}
				}
				else if(ai.curr < 0 && ai.curr >= ai.low) 
				{
					if(ai.cAlarmLevelStatus == EnumAnalogLevel.LOLO) 
					{
						AnalogAlarmSet_ReturnLOLO_LOW(ai);
					}
					else if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOW) 
					{
                        AnalogAlarmSet_LOW(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIHI || ai.cAlarmLevelStatus == EnumAnalogLevel.HIGH) 
						{
							if(ai.curr > ai.high+ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, true);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
						else 
						{	// EnumAnalogLevel.LOLO or EnumAnalogLevel.LOW
							if(ai.curr < ai.low-ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, false);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
					}
				}
			}
			else if(ai.cAlarmType == 7) 
			{	// HiHi High
				if(ai.curr >= 0 && ai.curr <= ai.hihi) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIHI) 
					{
						AnalogAlarmSet_HIHI(ai);
					}
				}
				else if(ai.curr >= 0 && ai.curr <= ai.high) 
				{
					if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIHI) 
					{	// HiHi에서 복귀
						AnalogAlarmSet_ReturnHIHI_HIGH(ai);
					}
					else if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIGH) 
					{	// 경보 발생.
						AnalogAlarmSet_HIGH(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr > ai.high+ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, true);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 8) 
			{	// Low, LoLo이하
				if(ai.curr < 0 && ai.curr >= ai.lolo) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOLO) 
					{
						AnalogAlarmSet_LOLO(ai);
					}
				}
				else if(ai.curr < 0 && ai.curr >= ai.low) 
				{
					if(ai.cAlarmLevelStatus == EnumAnalogLevel.LOLO) 
					{
                        AnalogAlarmSet_ReturnLOLO_LOW(ai);
					}
					else if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOW) 
					{
                        AnalogAlarmSet_LOW(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr < ai.low-ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, false);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
            
			else {}			
		}

		//----------------------------------------------------------------------------
		// 읽어온 아나로그 값의 레벨을 체크하고 이상이 있을시에 알람과 메세지를
		// 디스프레이 한다.
		//----------------------------------------------------------------------------

		static void AnalogAlarmLevelCheck(TagAiClass ai)
		{
			if(CheckAlarmProtectOnBigChange(ai))	return;

			if(ai.IsPowerFactorTag()) 
			{
				CheckAlarmOnPowerFactor(ai);
				return;
			}

			if(ai.cAlarmType == 0) 
			{	// HiHi 이상 LoLo이하일때 경보검사
				if(ai.curr >= ai.hihi) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIHI) 
					{
						AnalogAlarmSet_HIHI(ai);
					}
				}
				else if(ai.curr <= ai.lolo) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOLO) 
					{
						AnalogAlarmSet_LOLO(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIHI) 
						{
							if(ai.curr < ai.hihi-ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, true);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
						else 
						{	// EnumAnalogLevel.LOLO
							if(ai.curr > ai.lolo+ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, false);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
					}
				}
			}
			else if(ai.cAlarmType == 1) 
			{	// HiHi 이상
				if(ai.curr >= ai.hihi) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIHI) 
					{
						AnalogAlarmSet_HIHI(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr < ai.hihi-ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, true);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 2) 
			{	// LoLo이하일때 경보검사
				if(ai.curr <= ai.lolo) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOLO) 
					{	
						AnalogAlarmSet_LOLO(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr > ai.lolo+ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, false);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 3) 
			{	// High 이상 Low이하일때 경보검사
				if(ai.curr >= ai.high) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIGH) 
					{
						AnalogAlarmSet_HIGH(ai);
					}
				}
				else if(ai.curr <= ai.low) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOW) 
					{
                        AnalogAlarmSet_LOW(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIGH) 
						{
							if(ai.curr < ai.high-ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, true);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
						else 
						{
							if(ai.curr > ai.low+ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, false);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
				
					}
				}
			}
			else if(ai.cAlarmType == 4) 
			{	// High 이상 일때 경보검사
				if(ai.curr >= ai.high) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIGH) 
					{
						AnalogAlarmSet_HIGH(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr < ai.high-ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, true);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 5) 
			{	// Low이하일때 경보검사
				if(ai.curr <= ai.low) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOW) 
					{
                        AnalogAlarmSet_LOW(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr > ai.low+ai.fAlarmReturnGab) 
						{ // 경보 범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, false);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 6) 
			{	// HiHi High, Low, LoLo이하
				if(ai.curr >= ai.hihi) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIHI) 
					{
						AnalogAlarmSet_HIHI(ai);
					}
				}
				else if(ai.curr <= ai.lolo) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOLO) 
					{
						AnalogAlarmSet_LOLO(ai);
					}
				}
				else if(ai.curr >= ai.high) 
				{
					if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIHI) 
					{	// HiHi에서 복귀
                        if (ai.curr < ai.hihi - ai.fAlarmReturnGab)
                        { // HiHi에서 High로 내려올 때도 경보복귀차이값을 사용한다. 9.5.3 부터
                          AnalogAlarmSet_ReturnHIHI_HIGH(ai);
                        }
					}
					else if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIGH) 
					{	// 경보 발생.
						AnalogAlarmSet_HIGH(ai);
					}
				}
				else if(ai.curr <= ai.low) 
				{
					if(ai.cAlarmLevelStatus == EnumAnalogLevel.LOLO) 
					{
                        if (ai.curr > ai.lolo + ai.fAlarmReturnGab) // LoLo에서 Low로 복귀 될때도 경보복귀차이값을 사용한다. 9.5.3 부터
                        {
                            AnalogAlarmSet_ReturnLOLO_LOW(ai);
                        }
					}
					else if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOW) 
					{
                        AnalogAlarmSet_LOW(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIHI || ai.cAlarmLevelStatus == EnumAnalogLevel.HIGH)
						{
							if(ai.curr < ai.high-ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, true);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
						else 
						{	// EnumAnalogLevel.LOLO or EnumAnalogLevel.LOW
							if(ai.curr > ai.low+ai.fAlarmReturnGab) 
							{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
								AnalogAlarmCheck_MinMax(ai, false);
								AnalogAlarmSet_NORMAL(ai);
							}
						}
					}
				}
			}
			else if(ai.cAlarmType == 7) 
			{	// HiHi High
				if(ai.curr >= ai.hihi) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIHI) 
					{
						AnalogAlarmSet_HIHI(ai);
					}
				}
				else if(ai.curr >= ai.high) 
				{
					if(ai.cAlarmLevelStatus == EnumAnalogLevel.HIHI) 
					{	// HiHi에서 복귀
                        if (ai.curr < ai.hihi - ai.fAlarmReturnGab) // HiHi에서 High로 내려올 때도 경보복귀차이값을 사용한다. 9.5.3 부터
                        {
							AnalogAlarmSet_ReturnHIHI_HIGH(ai);
                        }
					}
					else if(ai.cAlarmLevelStatus != EnumAnalogLevel.HIGH)
					{	// 경보 발생.
						AnalogAlarmSet_HIGH(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr < ai.high-ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, true);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
			else if(ai.cAlarmType == 8) 
			{	// Low, LoLo이하
				if(ai.curr <= ai.lolo) 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOLO) 
					{
						AnalogAlarmSet_LOLO(ai);
					}
				}
				else if(ai.curr <= ai.low) 
				{
					if(ai.cAlarmLevelStatus == EnumAnalogLevel.LOLO) 
					{
                        if (ai.curr > ai.lolo + ai.fAlarmReturnGab) // LoLo에서 Low로 복귀 될때도 경보복귀차이값을 사용한다. 9.5.3 부터
                        {
                            AnalogAlarmSet_ReturnLOLO_LOW(ai);
                        }
					}
					else if(ai.cAlarmLevelStatus != EnumAnalogLevel.LOW) 
					{
                        AnalogAlarmSet_LOW(ai);
					}
				}
				else 
				{
					if(ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL) 
					{
						if(ai.curr > ai.low+ai.fAlarmReturnGab) 
						{ // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
							AnalogAlarmCheck_MinMax(ai, false);
							AnalogAlarmSet_NORMAL(ai);
						}
					}
				}
			}
            else if (ai.cAlarmType == 9)
            {	// Low <= curr <= High 2014-2-10 추가 경보는 Mid 경보이지만 색상은 대체할 것이 없어서 High 경보로 표시된다.
                if (ai.low <= ai.curr && ai.curr <= ai.high)
                {
                    if (ai.cAlarmLevelStatus != EnumAnalogLevel.MIDDLE)
                    {
                       AnalogAlarmSet_MIDDLE(ai);
                    }
                }
                else
                {
                    if (ai.cAlarmLevelStatus != EnumAnalogLevel.NORMAL)
                    {
                        AnalogAlarmSet_NORMAL(ai);
                        /*
                        if (ai.curr > ai.low + ai.fAlarmReturnGab)
                        { // 경보범위를 벗어나도 gab만큼은 경보를 유지한다.
                            //AnalogAlarmCheck_MinMax(ai, false);
                            AnalogAlarmSet_NORMAL(ai);
                        }*/
                    }
                }
            }
			else {}
		}


        //--------------------------------------------------------------------------------------------
        // AI 경보를 발생시키기 전에 과 변화된 값은 시간이 어느정도 지난후에 경보를 발생 시키도록 한다.
        //--------------------------------------------------------------------------------------------

        static bool CheckAlarmProtectOnBigChange(TagAiClass ai)
        {
            if (ai.cAlarmProtectOnBigChangePercent == 0) return false;	// 과변화 금지를 사용하지 않는다.
            if (ai.nAlarmProtectOnBigChangeSecond == 0) return false;	// 과변화 금지를 사용하지 않는다.

            // 과변화 지연경보 엔진이 이미 시작되었으므로 더이상 아래의 비율을 계산할 필요가 없다. 2017-9-20
            if (ai.nAlarmProtectOnBigChangeCount > 0)
            {
                DateTime st = DateTime.Now;
                if (ai.cAlarmProtectOnBigChangeOldSec == st.Second) return true;	// 진행 중 

                if (ai.cAlarmProtectOnBigChangeOldSec > st.Second)
                {
                    ai.nAlarmProtectOnBigChangeCount -= (sbyte)(st.Second + 60 - ai.cAlarmProtectOnBigChangeOldSec);
                }
                else
                {
                    ai.nAlarmProtectOnBigChangeCount -= (sbyte)(st.Second - ai.cAlarmProtectOnBigChangeOldSec);
                }
                if (ai.nAlarmProtectOnBigChangeCount < 0) ai.nAlarmProtectOnBigChangeCount = 0;
                if (ai.nAlarmProtectOnBigChangeCount == 0)
                    return false;

                ai.cAlarmProtectOnBigChangeOldSec = (sbyte)st.Second;

                return true;
            }

            ushort rate;

            if (ai.fFull - ai.fBase == 0)
                rate = 0;
            else
                rate = (ushort)(Math.Abs(ai.curr - ai.old) * 100 / (ai.fFull - ai.fBase));

            if (rate >= ai.cAlarmProtectOnBigChangePercent)
            {	// 과 변화 되었다.
                ai.nAlarmProtectOnBigChangeCount = ai.nAlarmProtectOnBigChangeSecond;
                DateTime st = DateTime.Now;
                ai.cAlarmProtectOnBigChangeOldSec = (sbyte)st.Second;
                return true;
            }

            return false;
        }

        /*
		//--------------------------------------------------------------------------------------------
		// AI 경보를 발생시키기 전에 과 변화된 값은 시간이 어느정도 지난후에 경보를 발생 시키도록 한다.
		//--------------------------------------------------------------------------------------------

		static bool CheckAlarmProtectOnBigChange(TagAiClass ai)	
		{
			if(ai.cAlarmProtectOnBigChangePercent == 0)	return false;	// 과변화 금지를 사용하지 않는다.
			if(ai.nAlarmProtectOnBigChangeSecond  == 0)	return false;	// 과변화 금지를 사용하지 않는다.

			ushort rate;
	
			if(ai.fFull-ai.fBase == 0) 
				rate = 0;
			else		
				rate = (ushort)(Math.Abs(ai.curr-ai.old)*100/(ai.fFull-ai.fBase));

			if(rate >= ai.cAlarmProtectOnBigChangePercent) 
			{	// 과 변화 되었다.
				ai.nAlarmProtectOnBigChangeCount = ai.nAlarmProtectOnBigChangeSecond;
				DateTime st = DateTime.Now;
				ai.cAlarmProtectOnBigChangeOldSec = (sbyte)st.Second;
				return true;
			}
			if(ai.nAlarmProtectOnBigChangeCount > 0) 
			{
				DateTime st = DateTime.Now;
				if(ai.cAlarmProtectOnBigChangeOldSec == st.Second)	return true;	// 진행 중 

				if(ai.cAlarmProtectOnBigChangeOldSec > st.Second) 
				{
					ai.nAlarmProtectOnBigChangeCount -= (sbyte)(st.Second+60-ai.cAlarmProtectOnBigChangeOldSec);
				}
				else 
				{
					ai.nAlarmProtectOnBigChangeCount -= (sbyte)(st.Second-ai.cAlarmProtectOnBigChangeOldSec);
				}
				if(ai.nAlarmProtectOnBigChangeCount < 0)	ai.nAlarmProtectOnBigChangeCount = 0;
				if(ai.nAlarmProtectOnBigChangeCount == 0)
					return false;

				ai.cAlarmProtectOnBigChangeOldSec = (sbyte)st.Second;

				return true;
			}

			return false;
		}*/


		static string GetDefinedAlarmMsgNEW()
		{
			if(ConfigRunMain.sAlarmMsgNEW.Length == 0)	return "NEW";
			return ConfigRunMain.sAlarmMsgNEW;
		}

		static string GetDefinedAlarmMsgCNF()
		{
			if(ConfigRunMain.sAlarmMsgCNF.Length == 0)	return "CNF";
			return ConfigRunMain.sAlarmMsgCNF;
		}

		static string GetDefinedAlarmMsgRET()
		{
			if(ConfigRunMain.sAlarmMsgRET.Length == 0)	return "RET";
			return ConfigRunMain.sAlarmMsgRET;
		}

		//----------------------------------------------------------------------------
		//	아날로그가 HIHI가 되었을 때 해야할 일을 체크한다.
		//----------------------------------------------------------------------------

		static void AnalogAlarmSet_HIHI(TagAiClass ai)
		{
			if(ai.alarm == 0)	return;

			string message;
			if(Tools.IsLangKorean()) 
			{
				message = String.Format("{0}  {1:F2} {2} -HiHi상태", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else if(Tools.IsLangJapanese()) 
			{
				message = String.Format("{0}  {1:F2} {2} -HiHi 状態", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else if(Tools.IsLangChinese()) 
			{
				message = String.Format("{0}  {1:F2} {2} -HiHi 状态", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else 
			{
				message = String.Format("{0}  {1:F2} {2} -HiHi Status", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
           AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.HIHI, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);

			ai.cAlarmLevelStatus = EnumAnalogLevel.HIHI;
			ai.fAlarmMaxValue = ai.curr;
		}

		//----------------------------------------------------------------------------
		//	아날로그가 LOLO가 되었을 때 해야할일을 체크한다.
		//----------------------------------------------------------------------------

		static void AnalogAlarmSet_LOLO(TagAiClass ai)
		{
			if(ai.alarm == 0)	return;

			string message;
			if(Tools.IsLangKorean()) 
			{
				message = String.Format("{0}  {1:F2} {2} -LoLo상태", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else if(Tools.IsLangJapanese()) 
			{
				message = String.Format("{0}  {1:F2} {2} -LoLo 状態", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else if(Tools.IsLangChinese()) 
			{
				message = String.Format("{0}  {1:F2} {2} -LoLo 状态", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else 
			{
				message = String.Format("{0}  {1:F2} {2} -LoLo Status", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
            AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.LOLO, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);

			ai.cAlarmLevelStatus = EnumAnalogLevel.LOLO;
			ai.fAlarmMinValue = ai.curr;
		}

		//--------------------------------------------------------------------------------------
		//	아날로그 값이 정상으로 복귀 되었을 때 알람이 최고/최소 얼마까지 진행했는지를 보여준다.
		//	flag 0 - min value, 1 - max value
		//--------------------------------------------------------------------------------------

		static void AnalogAlarmCheck_MinMax(TagAiClass ai, bool flag)
		{
			if(ai.alarm == 0)	return;

			string message;
			if(flag == false) 
			{
				if(Tools.IsLangKorean()) 
				{
					message = String.Format("{0}  {1:F2} {2}-최소값", GetDefinedAlarmMsgCNF(), ai.fAlarmMinValue, ai.unit);
				}
				else if(Tools.IsLangJapanese()) 
				{
					message = String.Format("{0}  {1:F2} {2}-最小値", GetDefinedAlarmMsgCNF(), ai.fAlarmMinValue, ai.unit);
				}
				else if(Tools.IsLangChinese()) 
				{
					message = String.Format("{0}  {1:F2} {2}-最小值", GetDefinedAlarmMsgCNF(), ai.fAlarmMinValue, ai.unit);
				}
				else 
				{
					message = String.Format("{0}  {1:F2} {2}-Min value", GetDefinedAlarmMsgCNF(), ai.fAlarmMinValue, ai.unit);
				}
                AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.CNF, false, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
			else 
			{
				if(Tools.IsLangKorean()) 
				{
					message = String.Format("{0}  {1:F2} {2}-최고값", GetDefinedAlarmMsgCNF(), ai.fAlarmMaxValue, ai.unit);
				}
				else if(Tools.IsLangJapanese()) 
				{
					message = String.Format("{0}  {1:F2} {2}-最大値", GetDefinedAlarmMsgCNF(), ai.fAlarmMaxValue, ai.unit);
				}
				else if(Tools.IsLangChinese()) 
				{
					message = String.Format("{0}  {1:F2} {2}-最大值", GetDefinedAlarmMsgCNF(), ai.fAlarmMaxValue, ai.unit);
				}
				else 
				{
					message = String.Format("{0}  {1:F2} {2}-Max value", GetDefinedAlarmMsgCNF(), ai.fAlarmMaxValue, ai.unit);
				}
                AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.CNF, false, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
		}

		//----------------------------------------------------------------------------
		//	아날로그값이 정상으로 복귀 되었을 때 해야 할일을 체크한다.
		//----------------------------------------------------------------------------

		static void AnalogAlarmSet_NORMAL(TagAiClass ai)
		{
			if(ai.alarm == 0)	return;

			string message;
			if(Tools.IsLangKorean()) 
			{
				message = String.Format("{0}  {1:F2} {2} -정상복귀", GetDefinedAlarmMsgRET(), ai.curr, ai.unit);
			}
			else if(Tools.IsLangChinese()) 
			{
				message = String.Format("{0}  {1:F2} {2} -正常回复", GetDefinedAlarmMsgRET(), ai.curr, ai.unit);
			}
			else 
			{
				message = String.Format("{0}  {1:F2} {2} -return normal", GetDefinedAlarmMsgRET(), ai.curr, ai.unit);
			}
            AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.RETURN, false, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);

			ai.cAlarmLevelStatus = EnumAnalogLevel.NORMAL;
		}

		//----------------------------------------------------------------------------
		//	아날로그가 HIGH이상이 되었을 때 해야 할일을 체크한다.
		//----------------------------------------------------------------------------

		static void AnalogAlarmSet_HIGH(TagAiClass ai)
		{
			if(ai.alarm == 0)	return;

			string message;
			if(Tools.IsLangKorean()) 
			{
				message = String.Format("{0}  {1:F2} {2} -High상태", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else if(Tools.IsLangJapanese()) 
			{
				message = String.Format("{0}  {1:F2} {2} -High 状態", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else if(Tools.IsLangChinese()) 
			{
				message = String.Format("{0}  {1:F2} {2} -High 状态", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else 
			{
				message = String.Format("{0}  {1:F2} {2} -High Status", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}

            AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.HIGH, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);

			ai.cAlarmLevelStatus = EnumAnalogLevel.HIGH;
			ai.fAlarmMaxValue = ai.curr;
		}

		//----------------------------------------------------------------------------
		//	아날로그가 LOW이하가 되었을 때 해야 할일을 체크한다.
		//----------------------------------------------------------------------------

		static void AnalogAlarmSet_LOW(TagAiClass ai)
		{
			if(ai.alarm == 0)	return;

			string message;
			if(Tools.IsLangKorean()) 
			{
				message = String.Format("{0}  {1:F2} {2} -Low상태", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else if(Tools.IsLangJapanese()) 
			{
				message = String.Format("{0}  {1:F2} {2} -Low 状態", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else if(Tools.IsLangChinese()) 
			{
				message = String.Format("{0}  {1:F2} {2} -Low 状态", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
			else 
			{
				message = String.Format("{0}  {1:F2} {2} -Low Status", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
			}
            AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.LOW, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);

			ai.cAlarmLevelStatus = EnumAnalogLevel.LOW;
            ai.fAlarmMinValue = ai.curr;
		}

        //----------------------------------------------------------------------------
        //	아날로그가 MIDDLE값이 되었을 때 해야 할일을 체크한다.
        //----------------------------------------------------------------------------

        static void AnalogAlarmSet_MIDDLE(TagAiClass ai)
        {
            if (ai.alarm == 0) return;

            string message;
            if (Tools.IsLangKorean())
            {
                message = String.Format("{0}  {1:F2} {2} -Mid 상태", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
            }
            else if (Tools.IsLangJapanese())
            {
                message = String.Format("{0}  {1:F2} {2} -Mid 状態", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
            }
            else if (Tools.IsLangChinese())
            {
                message = String.Format("{0}  {1:F2} {2} -Mid 状态", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
            }
            else
            {
                message = String.Format("{0}  {1:F2} {2} -Mid Status", GetDefinedAlarmMsgNEW(), ai.curr, ai.unit);
            }

            AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.HIGH, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);

            ai.cAlarmLevelStatus = EnumAnalogLevel.MIDDLE;
        }

		//----------------------------------------------------------------------------
		//	아날로그가 HIHI에서 HIGH이상이 되었을 때 해야 할일을 체크한다.
		//----------------------------------------------------------------------------

		static void AnalogAlarmSet_ReturnHIHI_HIGH(TagAiClass ai)
		{
			if(ai.alarm == 0)	return;

			string message;
			message = String.Format("{0}  {1:F2} {2} -HiHi -> High", GetDefinedAlarmMsgRET(), ai.curr, ai.unit);

            AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.HIGH, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);

			ai.cAlarmLevelStatus = EnumAnalogLevel.HIGH;
			ai.fAlarmMaxValue = ai.curr;
		}

		//----------------------------------------------------------------------------
		//	아날로그가 HIHI에서 HIGH이상이 되었을 때 해야 할일을 체크한다.
		//----------------------------------------------------------------------------

		static void AnalogAlarmSet_ReturnLOLO_LOW(TagAiClass ai)
		{
			if(ai.alarm == 0)	return;

			string message;
			message = String.Format("{0}  {1:F2} {2} -LoLo -> Low", GetDefinedAlarmMsgRET(), ai.curr, ai.unit);

            AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.LOW, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);

			ai.cAlarmLevelStatus = EnumAnalogLevel.LOW;
            ai.fAlarmMinValue = ai.curr;
		}

		//-----------------------------------------------------------------------------------
		// 범위 변화 제한 값을 넘었는지를 체크한다.
		//-----------------------------------------------------------------------------------

		static void CheckRateOfChangeLimit(TagAiClass ai)	
		{
			if(ai.wRateOfChangeLimit == 0)	return;	// 변화를 검사하지 않는다.

			ushort rate;
	
			if(ai.fFull-ai.fBase == 0) 
				rate = 0;
			else		
				rate = (ushort)(Math.Abs(ai.curr-ai.old)*100/(ai.fFull-ai.fBase));

			if(rate >= ai.wRateOfChangeLimit) 
			{	// 변화율 범위를 넘었다.
				string message;
				if(Tools.IsLangKorean()) 
				{
					message = String.Format("값 급변 {0:F2} -> {1:F2} {2}", ai.old, ai.curr, ai.unit);
				}
				else if(Tools.IsLangJapanese()) 
				{
					message = String.Format("過変化 {0:F2} -> {1:F2} {2}", ai.old, ai.curr, ai.unit);
				}
				else if(Tools.IsLangChinese()) 
				{
					message = String.Format("过度变化 {0:F2} -> {1:F2} {2}", ai.old, ai.curr, ai.unit);
				}
				else 
				{
					message = String.Format("Change Limit Over {0:F2} -> {1:F2} {2}", ai.old, ai.curr, ai.unit);
				}
               AlarmDisplay.AlarmDisplayAI(ai, message, EnumAlarmType.OVER_RATE_OF_CHANGE_LIMIT, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
		}

		//------------------------------------------------------------------------------
		//	디지털 입력에 변화가 있을때 경보 상태에 있는지 검사한다.
		// 디지털 입력값이 바뀌었을 때만 이 함수를 부르게 되어 있다.
		//------------------------------------------------------------------------------

		static void DigitalAlarmCheck(TagDiClass di)
		{
			if(di.alarm == 0)	return;		// 경보 Option이 Disable 상태.

			string message;

			if(di.cAlarmType == 0) 
			{		// ON일때 경보
				if(di.curr == 0) 
				{			// 정상으로 복귀되었다.
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0}  {1} 상태", GetDefinedAlarmMsgRET(), di.desOFF);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0}  {1} 状態", GetDefinedAlarmMsgRET(), di.desOFF);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0}  {1} 状态", GetDefinedAlarmMsgRET(), di.desOFF);
					}
					else 
					{
						message = String.Format( "{0}  {1} status", GetDefinedAlarmMsgRET(), di.desOFF);
					}
                    AlarmDisplay.AlarmDisplayDI(di, message, EnumAlarmType.RETURN, false, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
					return;
				}
				else 
				{
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0}  {1} 상태", GetDefinedAlarmMsgNEW(), di.desON);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0}  {1} 状態", GetDefinedAlarmMsgNEW(), di.desON);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0}  {1} 状态", GetDefinedAlarmMsgNEW(), di.desON);
					}
					else 
					{
						message = String.Format( "{0}  {1} status", GetDefinedAlarmMsgNEW(), di.desON);
					}
                   AlarmDisplay.AlarmDisplayDI(di, message, EnumAlarmType.DI_ON, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
				}
			}
			else if(di.cAlarmType == 1) 
			{	// OFF 일때 경보 발생
				if(di.curr == 1) 
				{
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0}  {1} 상태", GetDefinedAlarmMsgRET(), di.desON);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0}  {1} 状態", GetDefinedAlarmMsgRET(), di.desON);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0}  {1} 状态", GetDefinedAlarmMsgRET(), di.desON);
					}
					else 
					{
						message = String.Format( "{0}  {1} status", GetDefinedAlarmMsgRET(), di.desON);
					}
                    AlarmDisplay.AlarmDisplayDI(di, message, EnumAlarmType.RETURN, false, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
					return;
				}
				else 
				{
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0}  {1} 상태", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0}  {1} 状態", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0}  {1} 状态", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
					else 
					{
						message = String.Format( "{0}  {1} status", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
                    AlarmDisplay.AlarmDisplayDI(di, message, EnumAlarmType.DI_OFF, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
				}
			}
			else if(di.cAlarmType == 2)		// OFF->ON->OFF일 때 이것은 OFF될때만 경보 그렇지 않으면 정상
			{	
				if(di.curr == 0) 
				{
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0}  {1} 상태", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0}  {1} 状態", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0}  {1} 状态", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
					else 
					{
						message = String.Format( "{0}  {1} status", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
                    AlarmDisplay.AlarmDisplayDI(di, message, EnumAlarmType.DI_OFF, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
				}
			}
			else if(di.cAlarmType == 3)		// ON->OFF->ON일 때 이것은 ON될때만 경보 그렇지 않으면 정상
			{	
				if(di.curr == 1) 
				{
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0}  {1} 상태", GetDefinedAlarmMsgNEW(), di.desON);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0}  {1} 状態", GetDefinedAlarmMsgNEW(), di.desON);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0}  {1} 状态", GetDefinedAlarmMsgNEW(), di.desON);
					}
					else 
					{
						message = String.Format( "{0}  {1} status", GetDefinedAlarmMsgNEW(), di.desON);
					}
                   AlarmDisplay.AlarmDisplayDI(di, message, EnumAlarmType.DI_ON, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
				}
			}
			else if(di.cAlarmType == 4) 
			{	// ON/OFF 아무 신호나 바뀌었을 때 경보 발생
				if(di.curr == 1) 
				{
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0} {1} 상태", GetDefinedAlarmMsgNEW(), di.desON);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0} {1} 状態", GetDefinedAlarmMsgNEW(), di.desON);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0} {1} 状态", GetDefinedAlarmMsgNEW(), di.desON);
					}
					else 
					{
						message = String.Format( "{0} {1} status", GetDefinedAlarmMsgNEW(), di.desON);
					}
				}
				else 
				{
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0} {1} 상태", GetDefinedAlarmMsgRET(), di.desOFF);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0} {1} 状態", GetDefinedAlarmMsgRET(), di.desOFF);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0} {1} 状态", GetDefinedAlarmMsgRET(), di.desOFF);
					}
					else 
					{
						message = String.Format( "{0} {1} status", GetDefinedAlarmMsgRET(), di.desOFF);
					}
				}
                AlarmDisplay.AlarmDisplayDI(di, message, EnumAlarmType.DI_ON, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
			else if(di.cAlarmType == 5) 
			{	// OFF/ON 아무 신호나 바뀌었을 때 경보 발생
				if(di.curr == 0) 
				{
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0} {1} 상태", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0} {1} 状態", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0} {1} 状态", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
					else 
					{
						message = String.Format( "{0} {1} status", GetDefinedAlarmMsgNEW(), di.desOFF);
					}
				}
				else 
				{
					if(Tools.IsLangKorean()) 
					{
						message = String.Format( "{0} {1} 상태", GetDefinedAlarmMsgRET(), di.desON);
					}
					else if(Tools.IsLangJapanese()) 
					{
						message = String.Format( "{0} {1} 状態", GetDefinedAlarmMsgRET(), di.desON);
					}
					else if(Tools.IsLangChinese()) 
					{
						message = String.Format( "{0} {1} 状态", GetDefinedAlarmMsgRET(), di.desON);
					}
					else 
					{
						message = String.Format( "{0} {1} status", GetDefinedAlarmMsgRET(), di.desON);
					}
				}
                AlarmDisplay.AlarmDisplayDI(di, message, EnumAlarmType.DI_OFF, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
			else {}
		}

		//-----------------------------------------------------------------------------------------
		//	프로그램 시작 시 바로 시작하면 스캔이 안되어 있을 수 있으므로 사용자가 지정한 시간뒤에 스캔을 시작한다.
		//-----------------------------------------------------------------------------------------

		static TimeOutClass timeoutScanGo = new TimeOutClass();

		public static bool IsScanStart()
		{
			if(timeoutScanGo == null)	return true;

			if(timeoutScanGo.IsTimeOut(ConfigRunMain.nScanStartTime))
			{
				timeoutScanGo = null;
				return true;
			}

			return false;
		}

		//---------------------------------------------------------------------------------------------
		//	이 함수는 프로그램 시작 시 Di On/Off tag를 새로 정리하지 위한 부분 
		//	프로그램 시작시 바로 시작하면 통신에서 읽지 못했을수가 있으므로 
		//	일정 시간이 지난뒤에 On-Off Tag를 출력한다.
		//	한번 출력하고 나면 이 함수는 다시 사용하지 않는다.
		//---------------------------------------------------------------------------------------------

		static TimeOutClass timeoutAiDiSubTag = new TimeOutClass();

		public static async Task CheckAiDiSubTag()
		{
			if(timeoutAiDiSubTag == null)	return;	// 모든 일을 다했다.

			if(timeoutAiDiSubTag.IsTimeOut(10)) 
			{
				timeoutAiDiSubTag = null;
				int i;
				
				if(ConfigRunMain.bWriteDiSubTagWhenStart) 
				{
					TagPublicClass tp;
					TagDiClass di;

					for(i = 0; i < TagLib.tagListAll.Length; i++) 
					{
						tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);
						if(tp.act == 0)	continue;
						if(tp.enumTagType != EnumTagType.DI)	continue;
						di = (TagDiClass)tp;
						await DISubDigitalOutOnTagCheck(di);		// 출력할 디지털 출력태그가 있는가를 검사한다.
						await DISubDigitalOutOffTagCheck(di);		// 출력할 디지털 출력태그가 있는가를 검사한다.
					}
				}
				if(ConfigRunMain.bWriteAiSubTagWhenStart) 
				{
					TagPublicClass tp;
					TagAiClass ai;
					
					for(i = 0; i < TagLib.tagListAll.Length; i++) 
					{
						tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);
						if(tp.act == 0)	continue;
						if(tp.enumTagType != EnumTagType.AI)	continue;
						ai = (TagAiClass)tp;
						await SubOutAnalogCheck(ai);			// 출력할 아날로그 태그가 있는가를 검사한다.
					}
				}
			}
		}
	}


}

