using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using LocalMain;
using GraphicModule;

namespace AutoLibLocal
{
    /// <summary>
    /// 기존 레지스트리 방식과 새로운 MMF 방식을 병행 지원하는 래퍼
    /// </summary>
    public class FutureSharedTag
	{
        private static SharedTagMemoryManager _memoryManager;
        private static SharedTagPipeServer _pipeServer;
        private static readonly object _initLock = new object();

        /// <summary>
        /// 서버 초기화 (SCADA 메인 프로세스에서 호출)
        /// SharedTagMemoryManager 생성 >> 모든 태그 슬롯 초기화 
        /// </summary>
        public static void InitializeServer(int maxTags = 10000)
        {
            lock (_initLock)
            {
                if (_memoryManager == null)
                {
                    _memoryManager = new SharedTagMemoryManager(maxTags);
                    _pipeServer = new SharedTagPipeServer(_memoryManager);

                    // 콜백 설정 - 외부에서 쓰기 요청이 오면 엔진 태그값 업데이트
                    _pipeServer.SetTagWriteCallback(OnExternalTagWrite);

                    _pipeServer.Start();
                }
            }
        }

        /// <summary>
        /// 엔진 → MMF 최초 전체 동기화 (서버 시작 시 1회 호출)
        /// </summary>
        public static void InitialSyncFromEngine()
        {
            if (_memoryManager == null)
                return;

            foreach (var tagInfo in TagLib.tagListAll)
            {
                TagPublicClass tp = TagLib.GetStructPublic(tagInfo);
                if (tp == null)
                    continue;

                switch (tp.enumTagType)
                {
                    case EnumTagType.AI:
                    case EnumTagType.AO:
                        _memoryManager.UpdateTag(tp.tag, (double)tp.GetCurr());
                        break;
                    case EnumTagType.DI:
                    case EnumTagType.DO:
                        _memoryManager.UpdateTag(tp.tag, (sbyte)tp.GetCurr());
                        break;
                    case EnumTagType.ST:
                        _memoryManager.UpdateTag(tp.tag, tp.GetCurr().ToString());
                        break;
                }
            }
        }

        /// <summary>
        /// 외부 쓰기 요청 콜백 - 엔진 태그값 업데이트
        /// </summary>
        private static void OnExternalTagWrite(string tagName, string value, string user, string ip, string computer)
        {
            try
            {
                // TagLib에서 태그 찾기
                int[] tagPos = new int[1];
                TagPublicClass tp = TagLib.GetStructPublic(tagName, ref tagPos);

                if (tp == null)
                {
                    System.Diagnostics.Debug.WriteLine($"태그를 찾을 수 없음: {tagName}");
                    return;
                }

                // 엔진 태그값 업데이트 준비
                tp.sWriteWaitValue = value;

                // 숫자 변환 시도
                if (double.TryParse(value, out double dval))
                {
                    tp.fWriteWaitValue = dval;
                }
                else
                {
                    tp.fWriteWaitValue = 0;
                }

                // 쓰기 정보 저장
                tp.sWriteUser = user;
                tp.sWriteIP = ip;
                tp.sWriteComputer = computer;

                // 쓰기 대기 플래그 설정 - 엔진이 다음 사이클에서 처리
                tp.bWriteWait = true;

                System.Diagnostics.Debug.WriteLine(
                    $"엔진 태그 쓰기 준비: {tagName}={value}, User={user}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"엔진 태그 업데이트 실패: {tagName}, {ex.Message}");
            }
        }

        /// <summary>
        /// 서버 종료
        /// </summary>
        public static void ShutdownServer()
        {
            lock (_initLock)
            {
                _pipeServer?.Dispose();
                _memoryManager?.Dispose();
                _pipeServer = null;
                _memoryManager = null;
            }
        }



        public FutureSharedTag()
		{
			//
			// TODO: Add constructor logic here
			//
		}


        // CheckEngineTagChangeThread에서만 호출된다.
        public static bool SetCurrRegistryByThread(SharedRegistryClass share, double val)
        {
            share.Write("Value", val.ToString());
            return true;
        }

        // CheckEngineTagChangeThread에서만 호출된다.
        public static bool SetCurrRegistryByThread(SharedRegistryClass share, string val)
        {
            share.Write("Value", val);
            return true;
        }

        /// <summary>
        /// 태그 Quality 결정
        /// </summary>
        private static TagQuality DetermineQuality(TagPublicClass tp)
        {
            // TagPublicClass에 있을 수 있는 상태 플래그들
            // (실제 필드명은 프로젝트에 맞게 조정 필요)

            if (tp.GetType().GetProperty("bCommError")?.GetValue(tp) is bool commError && commError)
                return TagQuality.CommFail;

            if (tp.GetType().GetProperty("bTimeout")?.GetValue(tp) is bool timeout && timeout)
                return TagQuality.Timeout;

            if (tp.GetType().GetProperty("bDeviceError")?.GetValue(tp) is bool deviceError && deviceError)
                return TagQuality.DeviceError;

            return TagQuality.Good;
        }

        /// <summary>
        /// 태그 값 설정 (double) - 엔진 → MMF
        /// </summary>
        public static bool SetCurr(TagPublicClass tp, double val)
        {
            if (ConfigRunMain.bShareTagValueBySharedMemory) 
                CheckEngineTagChangeBySharedMemory.SetValue(tp, val);

            //if (ScriptFunctionTag.bTagShareUpdateOption)
            //{
            //    byte sum = SharedTag.GetSum8(tp.tag);
            //    TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tp.tag, "Value", val.ToString());
            //}
            //else
            //{
            //    tp.bNeedSharedTagUpdate = true;
            //}

            if (_memoryManager != null)
            {
                //TagQuality quality = DetermineQuality(tp);
                //return _memoryManager.UpdateTag(tp.tag, val, quality);
                return _memoryManager.UpdateTag(tp.tag, val);
            }

            return true;
        }

        public static bool SetCurr(TagPublicClass tp, string val)
        {
            if (ConfigRunMain.bShareTagValueBySharedMemory) 
                CheckEngineTagChangeBySharedMemory.SetValue(tp, val);

            //if (ScriptFunctionTag.bTagShareUpdateOption)
            //{
            //    byte sum = SharedTag.GetSum8(tp.tag);
            //    TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tp.tag, "Value", val);
            //}
            //else
            //{
            //    tp.bNeedSharedTagUpdate = true;
            //}

            if (_memoryManager != null)
            {
                return _memoryManager.UpdateTag(tp.tag, val);
            }

            return true;
        }

        /*
        public static bool SetCurr(string tag, double val)
		{
			byte sum = SharedTag.GetSum8(tag);
			TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString()+"\\"+tag, "Value", val.ToString());
			return true;
		}

		public static bool SetCurr(string tag, string val)
		{
			byte sum = SharedTag.GetSum8(tag);
			TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString()+"\\"+tag, "Value", val);
			return true;
		}*/


        public static bool GetWriteitem(string tag, out double ival, out string sval, out string user, out string ip, out string computer)
		{
			byte sum = SharedTag.GetSum8(tag);

            user = "";
            ip = "";
            computer = "";

			if(TotalConfig.LoadRegAutoBaseConfig("TagShare", sum.ToString()+"\\"+tag, "bWrite", 0) == 0) 
			{
				ival = 0;
				sval = "";
				return false;
			}

			string buf = TotalConfig.LoadRegAutoBaseConfig("TagShare", sum.ToString()+"\\"+tag, "Write", "");

			NetTools.CommaTextReader comma = new NetTools.CommaTextReader();
			comma.Set(buf);
			uint seed1 = (uint)TotalConfig.LoadRegAutoBaseConfig("TagShare", null, "Code", 0);
			long seed2 = 0;
			comma.GetLong(ref seed2);
			uint code = SharedTag.MakeCode(seed1, (uint)seed2);
			uint readcode = 0;
			comma.GetDWORD(ref readcode);

			sval = "";
            comma.GetStringTotalRemain(ref sval);//comma.GetString(ref sval);
			ival = NetTools.ConvertTool.ToDouble(sval);

            // 출력한 사람의 정보도 가져온다. 2011-4-15
            buf = TotalConfig.LoadRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tag, "WriteInfo", "");
            comma.Set(buf);
            comma.GetString(ref user);
            comma.GetString(ref ip);
            comma.GetString(ref computer);

            bool retn = false;
            string result_msg = "";

            // 10.3.5.0에서 출력이 안되는 부분이 있어서 시간이 다른것 같아서 시간은 무시해서 컴파일 한다. 중국 시간으로 설정해서 인지는 확실하지 않음. 2020-10-28

            if (readcode == code)
            {
                DateTime t2 = DateTime.Now;
                DateTime t1 = t2.AddMinutes(-30);
                DateTime t_seed = new DateTime(seed2);

                if (seed2 >= t1.Ticks && seed2 <= t2.Ticks)	// 30분전에 실행한 명령은 실행하지 않는다.
                    result_msg = "";//retn = true;
                else
                    result_msg = String.Format("time mismatched t1={0}, seedt={1}, t2={2}", t1, t_seed, t2);

                retn = true;
            }
            else
            {
                result_msg = String.Format("code mismatched readcode={0}, code={1}, seed1={2}, seed2={3}, value={4}", readcode, code, seed1, seed2, sval);
            }

            TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tag, "Write", result_msg);
            TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tag, "bWrite", 0);

			return retn;
		}

        public static bool GetWriteitem(SharedRegistryClass share, out double ival, out string sval, out string user, out string ip, out string computer)
        {
            user = "";
            ip = "";
            computer = "";

            if (share.Read("bWrite", 0) == 0)
            {
                ival = 0;
                sval = "";
                return false;
            }

            string buf = share.Read("Write", "");

            NetTools.CommaTextReader comma = new NetTools.CommaTextReader();
            comma.Set(buf);
            uint seed1 = (uint)TotalConfig.LoadRegAutoBaseConfig("TagShare", null, "Code", 0);
            long seed2 = 0;
            comma.GetLong(ref seed2);
            uint code = SharedTag.MakeCode(seed1, (uint)seed2);
            uint readcode = 0;
            comma.GetDWORD(ref readcode);

            sval = "";
            comma.GetStringTotalRemain(ref sval);//comma.GetString(ref sval);
            ival = NetTools.ConvertTool.ToDouble(sval);

            // 출력한 사람의 정보도 가져온다. 2011-4-15
            buf = share.Read("WriteInfo", "");
            comma.Set(buf);
            comma.GetString(ref user);
            comma.GetString(ref ip);
            comma.GetString(ref computer);

            bool retn = false;
            string result_msg = "";

            // 10.3.5.0에서 출력이 안되는 부분이 있어서 시간이 다른것 같아서 시간은 무시해서 컴파일 한다. 중국 시간으로 설정해서 인지는 확실하지 않음.

            if (readcode == code)
            {
                DateTime t2 = DateTime.Now;
                DateTime t1 = t2.AddMinutes(-30);
                DateTime t_seed = new DateTime(seed2);

                if (seed2 >= t1.Ticks && seed2 <= t2.Ticks)	// 30분전에 실행한 명령은 실행하지 않는다.
                    result_msg = "";//retn = true;
                else
                    result_msg = String.Format("time mismatched t1={0}, seedt={1}, t2={2}", t1, t_seed, t2);

                retn = true;
            }
            else
            {
                result_msg = String.Format("code mismatched readcode={0}, code={1}, seed1={2}, seed2={3}, value={4}", readcode, code, seed1, seed2, sval);
            }

            share.Write("Write", result_msg);
            share.Write("bWrite", 0);

            return retn;
        }
	}

    /*
     * 
     * ### 외부 프로그램 → 엔진 (쓰기)
     [외부 프로그램]
    ↓ SharedTagClient.WriteTag()
    [Named Pipe]
        ↓
    [SharedTagPipeServer.ProcessCommand()]
        ↓
    1. MMF 업데이트: _memoryManager.UpdateTag()
    2. 콜백 호출: _onTagWrite()
        ↓
    [FutureSharedTag.OnExternalTagWrite()]
        ↓
    TagPublicClass 업데이트:
      - tp.sWriteWaitValue = value
      - tp.fWriteWaitValue = numericValue  
      - tp.bWriteWait = true
        ↓
    [엔진 메인 루프]
        ↓
    tp.bWriteWait == true 확인
        ↓
    실제 태그값 적용 (curr 업데이트)

    ### 엔진 → 외부 프로그램 (읽기)
    [엔진 태그값 변경]
        ↓
    [ThreadLoopSharedTag]
        ↓
    FutureSharedTag.SetCurr()
        ↓
    _memoryManager.UpdateTag()
        ↓
    [MMF 업데이트]
        ↓
    [외부 프로그램]
    SharedTagClient.ReadTag()로 읽기
     * 
     */


}
