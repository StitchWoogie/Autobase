//#define USE_HASP_AKSHASP_LIBRARY  //.NET용 라이브러리라서 편리하나 2.0이 아직 안나옴

using System;
using System.Runtime.InteropServices;

using System.Text;
using NetTools;
using System.Drawing;
using System.Windows.Forms;
using NetTools.OldDefine;
using System.IO;
using Microsoft.Win32;
using System.Management;
using System.Collections.Generic;
using Ats.Comm;
using System.Threading;

namespace AutoLibLocal.KeyLock
{
    public enum LOCK_TYPE
    {
        NO_LOCK,
        MEGA_LOCK,
        HASP_LOCK,
        SOFT_LOCK,
        AUTO_LOCK,
    }

	/// <summary>
	/// Summary description for KeyLock.
	/// </summary>
	public class KeyLock
	{
		public KeyLock()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static void ScrembleBuffer(int[] buf, int size, uint seed)
		{
			int temp;
			int i;
			int pos = 100;

			for(i = 0; i < size; i++) 
			{
				pos += (int)((seed >> (i%23)) & 0xFFFF);
				pos %= size;

				temp = buf[i];
				buf[i] = buf[pos];
				buf[pos] = temp;
			}
		}

		void KeyLockEncodeData(byte[] buf, int size, uint seed)
		{
			int i, j, k;
			byte c = 55;
			int pos;
			int[] order = new int[size];
			byte[] src = new byte[size];

			for(i = 0; i < size; i++) 
			{
				order[i] = i;
			}

			ScrembleBuffer(order, size, seed);

			for(j = 0; j < 11; j++) 
			{
				c = 55;
				for(k = 0; k < size; k++)
					src[k] = buf[k];

				for(i = 0; i < size; i++) 
				{
					pos = order[i];
					c ^= (byte)src[pos];
					c ^= (byte)((seed >> (i%23)) & 0xFF);
					buf[i] = c;
				}
			}
		}
		
		public static void KeyLockDecodeData(ref byte[] buf, int size, uint seed)
		{
			int i, j, k;
			byte c;
			int pos;
			int[] order = new int[size];
			byte[] src = new byte[size];

			for(i = 0; i < size; i++) 
			{
				order[i] = i;
			}

			ScrembleBuffer(order, size, seed);

			for(j = 0; j < 11; j++) 
			{
				for(k = 0; k < size; k++)
					src[k] = buf[k];
				for(i = 0; i < size; i++) 
				{
					pos = order[i];
					if(i == 0)	c = (byte)(src[i]^55);
					else		c = (byte)(src[i]^src[i-1]);
					c ^= (byte)((seed >> (i%23)) & 0xFF);
					buf[pos] = c;
				}
			}
		}


        /*
		void KeyLockGetOemString(byte val, out string s)
		{
			if(NetTools.Tools.IsLangKorean()) 
			{
				if(val == 0x20)		s = "";
				else if(val == 0x00)	s = "";
				else if(val == 0x01)	s = "학교 교육용";
				else if(val == 0x02)	s = "엠알 엔지니어링";
				else if(val == 0x03)	s = "중앙제어(주)";
				else if(val == 0x04)	s = "(주)코바이오텍";
				else if(val == 0x05)	s = "(주)원플러스";
				else if(val == 0x06)	s = "보정시엔아이(주)";
				else if(val == 0x07)	s = "동영정보통신";
				else if(val == 0x08)	s = "광양제철소 화재감시 시스템";
				else if(val == 0x09)	s = "명성하이콘 주차시스템";
				else if(val == 0x0A)	s = "창원 생활 폐기물 소각장";
				else if(val == 0x0B)	s = "마산대학";
				else if(val == 0x0C)	s = "(주)대청시스템즈";
				else if(val == 0x0D)	s = "(주)모던테크";
				else if(val == 0x0E)	s = "충인전장";
				else					s = "Bundle Version";
			}
			else 
			{
				if(val == 0x20)			s = "";
				else if(val == 0x00)	s = "";
				else if(val == 0x01)	s = "Academy Version";
				else if(val == 0x02)	s = "MR Engineering";
				else if(val == 0x03)	s = "JA Control";
				else if(val == 0x04)	s = "KOBIO Tech";
				else if(val == 0x05)	s = "WONPLUS co.,ltd.";
				else if(val == 0x06)	s = "Bo jung C&&I Engineering co.,ltd.";
				else if(val == 0x07)	s = "동영정보통신";
				else if(val == 0x08)	s = "광양제철소 화재감시 시스템";
				else if(val == 0x09)	s = "명성하이콘 주차시스템";
				else if(val == 0x0A)	s = "창원 생활 폐기물 소각장";
				else if(val == 0x0B)	s = "마산대학";
				else if(val == 0x0C)	s = "(주)대청시스템즈";
				else if(val == 0x0D)	s = "Modern Tech co.,ltd.";
				else if(val == 0x0E)	s = "충인전장";
				else					s = "Bundle Version";
			}
		}*/

		public static string sSerialNumber = "Not found";
        public static int nTagSize = 0;
		public static int nKeyLockVersion = -1;	// -1 = 키락이 없거나 모르는 버전
		// 0 = 흰색
		// 1 = 노랑
		// 2 = 연두색(초록)
		// 3 = 파랑 (Cyan)
		// 4 = 빨강 (or Magenta)

		public static int nKeyLockRunOrDev = 0;	// 0 = runtime
		// 1 = developer
		public static int nKeyLockOem = 0;

		// bool bKeyLockExist = true;
		public static LOCK_TYPE cLockType = LOCK_TYPE.NO_LOCK;

		public static bool bExistLocalKey = false;
		public static bool bExistWebKey = false;
		public static int  nWebUserCount = 1;

		/// <summary>
		/// 체크가 느리고 키가 없을 때는 속도가 너무 느려서 당분간 보류한다.
		/// </summary>
		/// <param name="seed"></param>
		/// <param name="array"></param>
		/// <returns></returns>
		
		[DllImport("_NetCheckA.DLL", EntryPoint="GetBuffer", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetBuffer(uint seed, [In, Out] byte[] array);

		bool CheckLockMegaUsbLock()
		{
			byte[] buffer = new Byte[200];								
			int type = 0;
			DateTime t = DateTime.Now;
			uint seed = (uint)(t.Millisecond+t.Second*1000+t.Minute*1000*60);

			try 
			{
				type = GetBuffer(seed, buffer);
			}
			catch// (Exception someerror)
			{
				//MessageBox.Show(someerror.Message, "Hasp Call",
				//	MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			if(type == 0)	return false;

			uint dwSerialNumber;

			dwSerialNumber = (uint)((buffer[99]<<24)+(buffer[98]<<16)+(buffer[97]<<8)+buffer[96]);
			sSerialNumber = String.Format("{0:X08}", dwSerialNumber);

			KeyLockDecodeData(ref buffer, 96, seed);

            if (buffer[2] != 'u') return false;
            if (buffer[4] != 'T') return false;
            if (buffer[6] != 'o') return false;
            if (buffer[8] != 'B') return false;
            if (buffer[10] != 'a') return false;
            if (buffer[12] != 'S') return false;
            if (buffer[14] != 'e') return false;

            if (TotalConfig.eOemType == EnumOemType.UYeG_GS || TotalConfig.eOemType == EnumOemType.UYeG_Normal)
            {
                if (buffer[0] != 'B') return false;
            }
            else
            {
                if (buffer[0] != 'A') return false;
            }

			nTagSize = buffer[1];

            nTagSize = (nTagSize * 32);

			int version = buffer[3];
			if(version >= 0x20)	nKeyLockVersion = version-0x20;
			else				nKeyLockVersion = version;

            if (version >= 10)  // 2010년 이상된 키만 32태그 이하 적용 2010-4-29 10.1.1 부터 32태그 이하 지원
            {
                if (nTagSize == 32)
                {
                    if (buffer[13] < 32 && buffer[13] > 0)
                    {
                        nTagSize = buffer[13] * buffer[1];
                    }
                }
            }

			version = buffer[5];
			if(version == 0x01)	nKeyLockRunOrDev = 1;
			else				nKeyLockRunOrDev = 0;

			nKeyLockOem = buffer[15];

			bExistLocalKey = true;

			if(buffer[7] == 'W') 
			{
				bExistWebKey = true;
                nWebUserCount = buffer[9] * 256 + buffer[11];   // 웹키가 존재할 때만 유저수를 체크한다.
			}

			cLockType = LOCK_TYPE.MEGA_LOCK;

			return true;
		}

        // 24-12-13 추가. LocalMain을 제외한 프로그램에서 Lite 키락 (SoftKey)이 설치되어 있는지를 체크하기위해서...
        public void CheckKeyLockOnlySoftLock()
        {
            if (CheckLockAutoLock())    // 
            {
                if (bExistLocalKey) return;
            }
        }

		public bool CheckKeyLockLocal()
		{
			bExistLocalKey = false;
			bExistWebKey   = false;

			TimeOutClass timeout = new TimeOutClass();

#if USE_HASP_AKSHASP_LIBRARY
            Hasp_Call_AKSHASP hasp = new Hasp_Call_AKSHASP();
#else
            Hasp_Call_NetCheckH hasp = new Hasp_Call_NetCheckH();
#endif

			retry:;

			if(hasp.CheckLockHaspLock()) 
			{
				if(bExistLocalKey)	return true;
			}
			if(CheckLockMegaUsbLock()) 
			{
				if(bExistLocalKey)	return true;
			}
			if(CheckLockSoftLock()) 
			{
				if(bExistLocalKey)	return true;
			}
            if (CheckLockAutoLock())    // 일단 Management에서 조금 늦은 감이 있으므로 제일 나중에 체크한다.
            {
                if (bExistLocalKey) return true;
            }
            /*
            if (CheckLockNetLock())    
            {
                if (bExistLocalKey) return true;
            }*/

			int wait = TotalConfig.AutoBaseIniGetKeyLockWait();

			if(wait < 0)	wait = 0;
			if(wait > 100)	wait = 100;
	
			if(wait > 0 && !timeout.IsTimeOut(wait)) 
			{
				System.Threading.Thread.Sleep(2000);
				goto retry;
			}

			return false;
		}

		public bool CheckKeyLockWeb()
		{
			bExistLocalKey = false;
			bExistWebKey   = false;

#if USE_HASP_AKSHASP_LIBRARY
            Hasp_Call_AKSHASP hasp = new Hasp_Call_AKSHASP();
#else
            Hasp_Call_NetCheckH hasp = new Hasp_Call_NetCheckH();
#endif
			if(hasp.CheckLockHaspLock()) 
			{
				if(bExistWebKey)  return true;
			}
            if (CheckLockAutoLock())
            {
                if (bExistWebKey) return true;
            }

			return false;
		}

		public static string KeyLockGetOemString()
		{
			int value = KeyLock.nKeyLockOem;
			string s;
			if(Tools.IsLangKorean()) 
			{
				if(value == 0x20)		s = "";
				else if(value == 0x00)	s = "";
                else if (value == 0x32) s = ""; // 50번 베트남 대리점용
				else if(value == 0x01)	s = "학교 교육용";
				else if(value == 0x02)	s = "엠알 엔지니어링";
				else if(value == 0x03)	s = "중앙제어(주)";
				else if(value == 0x04)	s = "(주)코바이오텍";
				else if(value == 0x05)	s = "(주)원플러스";
				else if(value == 0x06)	s = "보정시엔아이(주)";
				else if(value == 0x07)	s = "동영정보통신";
				else if(value == 0x08)	s = "광양제철소 화재감시 시스템";
				else if(value == 0x09)	s = "명성하이콘 주차시스템";
				else if(value == 0x0A)	s = "창원 생활 폐기물 소각장";
				else if(value == 0x0B)	s = "마산대학";
				else if(value == 0x0C)	s = "(주)대청시스템즈";
				else if(value == 0x0D)	s = "(주)모던테크";
				else if(value == 0x0E)	s = "충인전장";
					//else if(value == 0x0F)	strcpy(s, "대하제어");
                else if (value == 0x10) s = "교육 기관용(캐디언스)";
				else					s = "Bundle Version";
			}
			else 
			{
				if(value == 0x20)		s = "";
				else if(value == 0x00)	s = "";
                else if (value == 0x32) s = ""; // 50번 베트남 대리점용
				else if(value == 0x01)	s = "Academy Version";
				else if(value == 0x02)	s = "MR Engineering";
				else if(value == 0x03)	s = "JA Control";
				else if(value == 0x04)	s = "KOBIO Tech";
				else if(value == 0x05)	s = "WONPLUS co.,ltd.";
				else if(value == 0x06)	s = "Bo jung C&&I Engineering co.,ltd.";
				else if(value == 0x07)	s = "동영정보통신";
				else if(value == 0x08)	s = "광양제철소 화재감시 시스템";
				else if(value == 0x09)	s = "명성하이콘 주차시스템";
				else if(value == 0x0A)	s = "창원 생활 폐기물 소각장";
				else if(value == 0x0B)	s = "마산대학";
				else if(value == 0x0C)	s = "(주)대청시스템즈";
				else if(value == 0x0D)	s = "Modern Tech co.,ltd.";
				else if(value == 0x0E)	s = "충인전장";
					//else if(value == 0x0F)	strcpy(s, "대하제어");
                else if (value == 0x10) s = "교육 기관용(캐디언스)";
				else					s = "Bundle Version";
			}

			return s;
		}

		public static Color GetColor()
		{
			Color color = Color.FromArgb(0, 0, 0);

			if(nKeyLockVersion == 0)		color = Color.FromArgb(255, 255, 255);	// 흰색
			else if(nKeyLockVersion == 1)	color = Color.FromArgb(255, 255, 0);	// 노랑
			else if(nKeyLockVersion == 2)	color = Color.FromArgb(0, 255, 0);		// 녹색
			else if(nKeyLockVersion == 3)	color = Color.FromArgb(0, 255, 255);	// 하늘
			else if(nKeyLockVersion == 4)	color = Color.FromArgb(255, 0, 255);	// 분홍
			else if(nKeyLockVersion == 5)	color = Color.FromArgb(200, 200, 200);	// 회색
			else if(nKeyLockVersion == 6)	color = Color.FromArgb(255, 255, 255);	// 흰색 2005년 1월1일부터 출시
            else if (nKeyLockVersion == 7)  color = Color.FromArgb(255, 255, 255);	// 실제 2007년 6월 18일 부터 7 버전이 나갔지만 이때부터 버전과 년도를 일치 시키기로 함 6이하는 일치시키기 힘듬
            else if (nKeyLockVersion == 8)  color = Color.FromArgb(255, 255, 255);		// 
            else if (nKeyLockVersion == 9)  color = Color.FromArgb(255, 255, 255);		// 
            else color = Color.FromArgb(255, 255, 255);		// 

			return color;
		}

		public static int GetVersion()
		{
			return nKeyLockVersion;	
		}

		//------------------------------------------------------------------------------
		//	키락이 없고 지정 시간이 되면 메세지를 출력한다.
		//------------------------------------------------------------------------------

        //keylock 주기적 체크 추가. 20241024 PSU
        //구형PC(ATM)는 키락존재시 약 6초 소요. >> 스레드 적용.
        //Smlog 에 lock 적용.
        //FormLocalMain의 WndProc에 디바이스 변경확인 적용.
        //디바이스 변경시 10초마다 체크, 날짜변경시 체크.
        private static readonly object lockObject = new object();
        private static bool isRunning = false;
        private static Thread checkThread;
        private static DateTime lastCheckTime = DateTime.MinValue;
        private static DateTime _lastErrorLogTime = DateTime.MinValue;
        private static int _errorCount = 0;
        private static string _lastErrorMessage = string.Empty;
        private static bool _previousKeyLockStatus = false;
        private static int _deviceCheckFlag = 0;

        public static bool PreviousKeyLockStatus
        {
            get { return _previousKeyLockStatus; }
            set
            {
                if (_previousKeyLockStatus != value)
                {
                    _previousKeyLockStatus = value;
                }
            }
        }

        public static int DeviceCheckFlag
        {
            get
            {
                lock (lockObject)
                {
                    return _deviceCheckFlag;
                }
            }
            set
            {
                lock (lockObject)
                {
                    _deviceCheckFlag = value;
                }
            }
        }

        public static void StartKeyLockChecking()
        {
            if (!isRunning && (checkThread == null || !checkThread.IsAlive))
            {
                isRunning = true;
                lastCheckTime = DateTime.Now;
                checkThread = new Thread(KeyLockCheckingThread);
                checkThread.IsBackground = true;
                checkThread.Start();
            }
        }

        public static void StopKeyLockChecking()
        {
            isRunning = false;
            checkThread = null;
        }

        private static void KeyLockCheckingThread()
        {
            while (isRunning)
            {
                DateTime now = DateTime.Now;
                bool shouldCheck = false;

                // 디바이스 변경 체크 또는 날짜변경시 체크
                lock (lockObject)
                {
                    if (_deviceCheckFlag == 1 || now.Date != lastCheckTime.Date)
                    {
                        shouldCheck = true;
                    }
                }

                Thread.Sleep(2000); //장치 인식 후 2초 대기.

                //윈도우시간 과거로 변경하는 경우 처리를 위해 절대값 Duration 사용
                if (shouldCheck && (now - lastCheckTime).Duration() > TimeSpan.FromSeconds(10))
                {
                    try
                    {
                        KeyLock keyLock = new KeyLock();
                        bool newStatus = keyLock.CheckKeyLockLocalPeriod();
                        lastCheckTime = now;

                        if (PreviousKeyLockStatus != newStatus)
                        {
                            _previousKeyLockStatus = newStatus;

                            if (newStatus)
                            {
                                Log.Write(LogLevel.INFO, LogCategory.KEYLOCK,
                                    String.Format("Local KeyLock recognized. Serial Number: {0}", KeyLock.sSerialNumber));
                                MessageDisplay.Show(String.Format("Local KeyLock recognized. Serial Number: {0}", KeyLock.sSerialNumber));
                            }
                            else
                            {
                                Log.Write(LogLevel.WARNING, LogCategory.KEYLOCK, "Local KeyLock not recognized.");
                                MessageDisplay.Show("Local KeyLock not recognized");
                            }
                        }

                        // 디바이스 변경 체크였을 경우 플래그 리셋
                        lock (lockObject)
                        {
                            if (_deviceCheckFlag == 1)
                            {
                                _deviceCheckFlag = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteRateLimitedLog(String.Format("Error in KeyLock check: {0}", ex.Message));
                    }
                }


            }
        }

        private static void WriteRateLimitedLog(string message)
        {
            DateTime now = DateTime.Now;
            _errorCount++;

            // 처음이거나, 30분마다 기록.
            if (_lastErrorLogTime == DateTime.MinValue ||
                (now - _lastErrorLogTime).TotalMinutes >= 30)
            {
                string logMessage = (_errorCount > 1)
                    ? String.Format("{0} (Occurred {1} times)", message, _errorCount)
                    : message;

                Log.Write(LogLevel.ERROR, LogCategory.KEYLOCK, logMessage);
                _lastErrorLogTime = now;
            }
        }

        public bool CheckKeyLockLocalPeriod()
        {
            bExistLocalKey = false;
            bExistWebKey = false;

#if USE_HASP_AKSHASP_LIBRARY
    Hasp_Call_AKSHASP hasp = new Hasp_Call_AKSHASP();
#else
            Hasp_Call_NetCheckH hasp = new Hasp_Call_NetCheckH();
#endif

            if (hasp.CheckLockHaspLock() && KeyLock.bExistLocalKey) return true;
            if (CheckLockMegaUsbLock() && KeyLock.bExistLocalKey) return true;
            if (CheckLockSoftLock() && KeyLock.bExistLocalKey) return true;
            if (CheckLockAutoLock() && KeyLock.bExistLocalKey) return true;

            if (!bExistLocalKey && !bExistWebKey) { sSerialNumber = "Not found"; }
            return false;
        }
		static int nCurrSec = 0;
		static int nOldSec;
		static int nOldDay;

		public static bool CheckTimeOutLocalKey() //메인 타이머에서 호출.
		{
			DateTime t = DateTime.Now;
			if(bExistLocalKey == true) 
			{
				if(cLockType == LOCK_TYPE.SOFT_LOCK) 
				{
					if(nOldDay == 0) 
					{
						nOldDay = t.Day;
						return false;
					}
					else 
					{
						if(t.Day != nOldDay) 
						{
							nOldDay = t.Day;
							if(!CheckDayOfSoftLock(bSoftLockID)) 
							{
                                Log.Write(LogLevel.WARNING, LogCategory.KEYLOCK, "TestMode Start (Soft lock is expired)"); // 테스트모드 로그 추가 20241024 PSU
								if(Tools.IsLangKorean()) 
								{
									MessageBox.Show("소프트 락의 사용 기간이 만료되었습니다.\nTestMode로 변환되어 통신을 할 수 없습니다.", "SoftLock 만료");
								} 
								else 
								{
                                    MessageBox.Show("Soft lock is expired.\nIt will run by TestMode.", "KeyLock not exist");
								}
								return true;
							}
						}
					}

					return false;
				}
				else 
				{
                    nCurrSec = 0;  //키락인식되었을 경우, 초기화
					return false;
				}
			}

			if(t.Second != nOldSec) 
			{
				nOldSec = t.Second;
				//if(nCurrSec > 1800) 
                if (nCurrSec > 7200) //demo 2시간으로 연장 20241010 PSU
				{
					bExistLocalKey = true;
                    Log.Write(LogLevel.WARNING, LogCategory.SYSTEM, "TestMode Start"); // 테스트모드 로그 추가 20241024 PSU
					if(Tools.IsLangKorean()) 
					{
                        string msg = String.Format("{0} KeyLock이 없는 상태로 2시간을 사용했습니다.\nTestMode로 변환되어 통신을 할 수 없습니다.", TotalConfig.AutoBaseIniGetOemProgramName());
                        MessageBox.Show(msg, "KeyLock 없음");
					}
					else 
					{
                        string msg = String.Format("You have used this program for 2 hours without {0} KeyLock.\nIt will run by TestMode.", TotalConfig.AutoBaseIniGetOemProgramName());
                        MessageBox.Show(msg, "KeyLock not exist");
					}
					return true;
				}
				nCurrSec++;
			}
			return false;
		}

		public static string sSoftLockManagementCompany;
		public static string sSoftLockManagement;
		public static string sSoftLockInstallPlace;
		public static string sSoftLockSystem;
		public static string sSoftLockSystemUser;
		public static string sSoftLockSystemTelephone;
		public static SYSTEMTIME tSoftLockFrom = new SYSTEMTIME();
		public static SYSTEMTIME tSoftLockTo = new SYSTEMTIME();
		static byte[] bSoftLockID = new byte[16];

		static bool CheckDayOfSoftLock(byte[] guid)
		{
			string sGuid = "";
			string imsi;
			RegistryKey reg;
			string main_key_path;
			string item_end = "";
			byte crc_end   = 0x17;

			for(int i = 0; i < 16; i++) 
			{
				imsi = String.Format("{0:X02}", guid[i]);
				sGuid += imsi;

				crc_end += guid[i];
				imsi = String.Format("{0:X02}", crc_end);
				item_end += imsi;
			}

			main_key_path = String.Format("Software\\Microsoft\\Windows\\CurrentVersion\\Installer\\UpgradeCodes\\{0}", sGuid);

			int hap_fr;
			int hap_to;
			int hap_cr;
			SYSTEMTIME t = new SYSTEMTIME();

			t.GetLocalTime();

			hap_cr = (int)TimeUtil.GetDayHap(t.wYear, t.wMonth, t.wDay);
			hap_fr = (int)TimeUtil.GetDayHap(tSoftLockFrom.wYear, tSoftLockFrom.wMonth, tSoftLockFrom.wDay);
			hap_to = (int)TimeUtil.GetDayHap(tSoftLockTo.wYear, tSoftLockTo.wMonth, tSoftLockTo.wDay);

			RegistryKey key2;

			if(hap_cr < hap_fr || hap_cr > hap_to) 
			{
                reg = TotalConfig.GetRootRegistryKey();
				key2 = reg.CreateSubKey(main_key_path);
				if(key2 == null)	return false;
				key2.SetValue(item_end, "");
				key2.Close();
				reg.Close();
				return false;
			}

            reg = TotalConfig.GetRootRegistryKey();
			key2 = reg.OpenSubKey(main_key_path);
			if(key2 != null) 
			{
				key2.Close();
				reg.Close();
				return false;
			}

			reg.Close();

			return true;
		}

		string ReadStringFromBinary(byte[] buf, int pos)
		{
			int i;

			char ch;
			byte b1, b2;

			string retn = "";

			for(i = 0; i < 50; i++) 
			{
				b1 = buf[pos+i*2];
				b2 = buf[pos+i*2+1];
				ch = (char)(b1+b2*256);
				if(ch == 0)	break;
				retn += ch;
			}

			return retn;
		}

		bool CheckLockSoftLock()
		{	
			string filename;
			FileStream reader;

			filename = String.Format("{0}\\KeyLock.lic", Application.StartupPath);

			if(!File.Exists(filename))					return false;
			if(Tools.getfilesize(filename) != 10000)	return false;

			byte[] buf = new byte[10000];

			reader = File.Open(filename, FileMode.Open, FileAccess.Read);

			if(reader == null)	return false;
			reader.Read(buf, 0, 10000);
			reader.Close();

			KeyLockDecodeData(ref buf, 10000, 36);

			ushort crc_sum = 0;
			ushort crc_xor = 0;

			int i;
			const int MAX_BUFFER = 10000;
			for(i = 0; i < MAX_BUFFER-4; i++) 
			{
				crc_sum += buf[i];
				crc_xor += buf[i];
			}

			ushort read_sum = (ushort)(buf[MAX_BUFFER-4]+buf[MAX_BUFFER-3]*256);
			ushort read_xor = (ushort)(buf[MAX_BUFFER-2]+buf[MAX_BUFFER-1]*256);

			if(crc_sum != read_sum || crc_xor != read_xor) 
			{
				return false;
			}

			char[] wcs = new char[100];
	
			string imsi = ReadStringFromBinary(buf, 0);

			if(imsi != "AutoBaseSoftLock")	return false;

			for(i = 0; i < 16; i++)	bSoftLockID[i] = buf[50+i];

			sSoftLockManagementCompany = ReadStringFromBinary(buf, 100);

			sSoftLockManagement = ReadStringFromBinary(buf, 200);

			sSoftLockInstallPlace = ReadStringFromBinary(buf, 300);

			sSoftLockSystem = ReadStringFromBinary(buf, 400);

			sSoftLockSystemUser = ReadStringFromBinary(buf, 500);

			sSoftLockSystemTelephone = ReadStringFromBinary(buf, 600);
	
			tSoftLockFrom.wYear = (ushort)(buf[700]+buf[701]*256);
			tSoftLockFrom.wMonth = (ushort)(buf[702]+buf[703]*256);
			tSoftLockFrom.wDay = (ushort)(buf[704]+buf[705]*256);
			tSoftLockTo.wYear = (ushort)(buf[706]+buf[707]*256);
			tSoftLockTo.wMonth = (ushort)(buf[708]+buf[709]*256);
			tSoftLockTo.wDay = (ushort)(buf[710]+buf[711]*256);

			if(!CheckDayOfSoftLock(bSoftLockID)) 
			{
				return false;
			}

			nTagSize = 0;
			nKeyLockRunOrDev = 0;
			nKeyLockOem = 0;
			uint dwSerialNumber = (uint)((bSoftLockID[0]<<24) | (bSoftLockID[4]<<16) | (bSoftLockID[8]<<8) | (bSoftLockID[12]<<0));
			sSerialNumber = String.Format("{0:X08}", dwSerialNumber);
			cLockType = LOCK_TYPE.SOFT_LOCK;

			bExistLocalKey = true;

			return true;
		}

		public static bool IsSoftLock()
		{
			if(cLockType == LOCK_TYPE.SOFT_LOCK)	return true;
			else	return false;

		}

        string GetData(ManagementObject Source, string name)
        {
            object obj;

            try
            {
                obj = Source[name];
            }
            catch
            {
                obj = null;
            }

            string val;

            if (obj == null)
            {
                return "";
            }
            else if (obj.GetType() == typeof(uint))
            {
                val = String.Format("{0:X8}", obj);
            }
            else
            {
                val = obj.ToString();
            }

            return val;
        }

        public static string sBaseBoardProductName = "";

        /// <summary>
        ///  260121 PSU . vmware 종료 후 ManagementObject Source  에서 hang 현상 발견. try catch 로 예외처리 추가. 
        ///  다시 재현하기 어려워 try catch에 잡히는 지 모름. WMI hang 현상으로 판단됨.
        /// </summary>
        /// <param name="cpu"></param>
        /// <param name="mac"></param>
        /// <param name="harddisk"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        bool CompareSystemInfo(string cpu, string mac, string harddisk, string board)
        {
            string data = "";
            
            ManagementClass myManagementClass = new ManagementClass("Win32_processor");
            ManagementObjectCollection moc = myManagementClass.GetInstances();

            try
            {
                foreach (ManagementObject Source in moc)
                {
                    data = GetData(Source, "ProcessorID");
                    if (data.Length > 0) break;
                }
            }
            catch (Exception ex)
            {
                Log.Write(LogLevel.ERROR, LogCategory.KEYLOCK, $"SoftLock CompareSystemInfo Error : {ex.Message}"); // 20260121 PSU 추가
                MessageDisplay.Show($"SoftLock CompareSystemInfo Error : {ex.Message}");
                return false;
            }

            if (cpu != data) return false;
            
            myManagementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            moc = myManagementClass.GetInstances();

            bool mac_compare = false;

            CommaBlockString comma = new CommaBlockString();
            comma.Set(mac);
            List<string> macs = new List<string>();
            while (!comma.IsEOS())
            {
                comma.GetString(ref data);
                if (data.Length > 0)
                    macs.Add(data);
            }

            data = "";

            foreach (ManagementObject Source in moc)
            {
                /*
                string ip = GetData(Source, "IPEnabled");

                if (String.Compare(ip, "true", true) == 0)
                {
                    data = GetData(Source, "MACAddress");

                    for (int i = 0; i < macs.Count; i++)
                    {
                        if (data == macs[i])
                        {
                            mac_compare = true;
                            goto ok_mac;
                        }
                    }
                }*/

                // ATH 모델에서는 케이블을 연결하지 않으면 IP가 발생하지 않으므로 IPEnabled는 체크하지 않았다. 2012-2-27
                data = GetData(Source, "MACAddress");
                if (data.Length > 0)
                {
                    for (int i = 0; i < macs.Count; i++)
                    {
                        if (data == macs[i])
                        {
                            mac_compare = true;
                            goto ok_mac;
                        }
                    }
                }
            }
            ok_mac:
            if (!mac_compare) return false;

            /*  FDISK에 따라서 ID가 바뀔 수가 있으므로 보류한다. 2009.12.21
            myManagementClass = new ManagementClass("Win32_DiskDrive");
            moc = myManagementClass.GetInstances();

            data = "";

            foreach (ManagementObject Source in moc)
            {
                string deviceid = GetData(Source, "DeviceID");

                if (deviceid.IndexOf("PHYSICALDRIVE0") != -1)
                {
                    data = GetData(Source, "Signature") + "-" + GetData(Source, "Model");

                    break;
                }
            }

            if (harddisk != data) return false;*/

            myManagementClass = new ManagementClass("Win32_BaseBoard");
            moc = myManagementClass.GetInstances();

            data = "";

            foreach (ManagementObject Source in moc)
            {
                data = GetData(Source, "Product");
                if (data.Length > 0) break;
            }

            sBaseBoardProductName = data;

            if (board != data) return false;

            return true;
        }

        bool CheckLockAutoLockOneFile(string filename)
        {
            FileStream reader;

            if (!File.Exists(filename)) return false;
            if (Tools.getfilesize(filename) != 10000) return false;

            byte[] buf = new byte[10000];

            reader = File.Open(filename, FileMode.Open, FileAccess.Read);

            if (reader == null) return false;
            reader.Read(buf, 0, 10000);
            reader.Close();

            KeyLockDecodeData(ref buf, 10000, 0x7199);

            ushort crc_sum = 0;
            ushort crc_xor = 0;

            int i;
            const int MAX_BUFFER = 10000;
            for (i = 0; i < MAX_BUFFER - 4; i++)
            {
                crc_sum += buf[i];
                crc_xor += buf[i];
            }

            ushort read_sum = (ushort)(buf[MAX_BUFFER - 4] + buf[MAX_BUFFER - 3] * 256);
            ushort read_xor = (ushort)(buf[MAX_BUFFER - 2] + buf[MAX_BUFFER - 1] * 256);

            if (crc_sum != read_sum || crc_xor != read_xor)
            {
                return false;
            }

            char[] wcs = new char[100];

            string imsi = ReadStringFromBinary(buf, 1000);

            //if (imsi != "AutoBaseSoftLock") return false;

            EnumOemKeylockType oem = EnumOemKeylockType.Normal;
            if (imsi == "AutoBaseSoftLock") // 기존의 터치에 사용한 일반 버전의 소프트 키락
            {

            }
            else if (imsi == "AutoBaseLiteLock")    // SCADA-Lite에 사용하는 키락. 24-12-11 추가
            {
                oem = EnumOemKeylockType.ScadaLite;
            }

            string sOemSn = ReadStringFromBinary(buf, 400);

            sSerialNumber = "OEM-" + sOemSn + "-" + ReadStringFromBinary(buf, 1050);
            string sCpu = ReadStringFromBinary(buf, 0);

            string sMac = ReadStringFromBinary(buf, 100);

            string sHard = ReadStringFromBinary(buf, 200);

            string sMainBoard = ReadStringFromBinary(buf, 300);

            if (!CompareSystemInfo(sCpu, sMac, sHard, sMainBoard))
                return false;

            ushort bUseTime = (ushort)(buf[900] + buf[901] * 256);

            nKeyLockVersion = buf[1100] + buf[1101] * 256;

            bExistLocalKey = (buf[1102] == 1);
            nTagSize = ((buf[1103]) + (buf[1104] * 256) + (buf[1105] * 0x10000) + (buf[1106] * 0x1000000));
            bExistWebKey = (buf[1107] == 1);
            nWebUserCount = (buf[1108] + buf[1109] * 0x100);

            if (bUseTime == 1)
            {
                tSoftLockFrom.wYear = (ushort)(buf[902] + buf[903] * 256);
                tSoftLockFrom.wMonth = (ushort)(buf[904] + buf[905] * 256);
                tSoftLockFrom.wDay = (ushort)(buf[906] + buf[907] * 256);
                tSoftLockTo.wYear = (ushort)(buf[908] + buf[909] * 256);
                tSoftLockTo.wMonth = (ushort)(buf[910] + buf[911] * 256);
                tSoftLockTo.wDay = (ushort)(buf[912] + buf[913] * 256);

                sSerialNumber += String.Format("({0}-{1}-{2})", tSoftLockTo.wYear, tSoftLockTo.wMonth, tSoftLockTo.wDay);

                DateTime t = DateTime.Now;
                int days = (int)(TimeUtil.GetDayHap(t) - TimeUtil.GetDayHap(tSoftLockTo.wYear, tSoftLockTo.wMonth, tSoftLockTo.wDay));
                if (days > 0)
                {
                    bExistLocalKey = false;
                    bExistWebKey = false;
                    return false;
                }
            }

            cLockType = LOCK_TYPE.AUTO_LOCK;
            TotalConfig.eOemKeylockType = oem;

            return true;
        }

        bool CheckLockAutoLockOneFolder(string keylock_folder)
        {
            string filename;

            if (!Directory.Exists(keylock_folder)) return false;

            DirectoryInfo di = new DirectoryInfo(keylock_folder);
            FileInfo[] fis = di.GetFiles("SCADA-*.key");

            if (fis.Length == 0) return false;  // 1개인 경우에만 허용한다.

            for (int i = 0; i < fis.Length; i++)
            {
                filename = String.Format(fis[i].FullName);
                if (CheckLockAutoLockOneFile(filename)) return true;
            }

            return false;
        }

        // C:\AutoLock 폴더를 지우는 경우가 발생해서 C:\AutoBase\AutoLock 폴더에도 키락 파일을 보관하여 읽을 수 있도록 한다.  2013-6-21
        bool CheckLockAutoLock()
        {
            if (CheckLockAutoLockOneFolder("C:\\AutoLock")) return true;
            if (CheckLockAutoLockOneFolder("C:\\AutoBase\\AutoLock")) return true;

            return false;
        }

        /* C:\\AutoLock 폴더에 한개의 락만 허용한 경우 2013-6-21 이전
        bool CheckLockAutoLock()
        {
            string filename;
            FileStream reader;

            string keylock_folder = "C:\\AutoLock";
            if (!Directory.Exists(keylock_folder)) return false;

            DirectoryInfo di = new DirectoryInfo(keylock_folder);
            FileInfo[] fis = di.GetFiles("SCADA-*.key");

            if (fis.Length != 1) return false;  // 1개인 경우에만 허용한다.
            
            filename = String.Format(fis[0].FullName);

            if (!File.Exists(filename)) return false;
            if (Tools.getfilesize(filename) != 10000) return false;

            byte[] buf = new byte[10000];

            reader = File.Open(filename, FileMode.Open, FileAccess.Read);

            if (reader == null) return false;
            reader.Read(buf, 0, 10000);
            reader.Close();

            KeyLockDecodeData(ref buf, 10000, 0x7199);

            ushort crc_sum = 0;
            ushort crc_xor = 0;

            int i;
            const int MAX_BUFFER = 10000;
            for (i = 0; i < MAX_BUFFER - 4; i++)
            {
                crc_sum += buf[i];
                crc_xor += buf[i];
            }

            ushort read_sum = (ushort)(buf[MAX_BUFFER - 4] + buf[MAX_BUFFER - 3] * 256);
            ushort read_xor = (ushort)(buf[MAX_BUFFER - 2] + buf[MAX_BUFFER - 1] * 256);

            if (crc_sum != read_sum || crc_xor != read_xor)
            {
                return false;
            }

            char[] wcs = new char[100];

            string imsi = ReadStringFromBinary(buf, 1000);

            if (imsi != "AutoBaseSoftLock") return false;

            string sOemSn = ReadStringFromBinary(buf, 400);

            sSerialNumber = "OEM-" + sOemSn + "-" + ReadStringFromBinary(buf, 1050);
            string sCpu = ReadStringFromBinary(buf, 0);

            string sMac = ReadStringFromBinary(buf, 100);

            string sHard = ReadStringFromBinary(buf, 200);

            string sMainBoard = ReadStringFromBinary(buf, 300);

            if (!CompareSystemInfo(sCpu, sMac, sHard, sMainBoard))
                return false;

            ushort bUseTime = (ushort)(buf[900] + buf[901] * 256);

            nKeyLockVersion = buf[1100] + buf[1101] * 256;

            bExistLocalKey = (buf[1102] == 1);
            nTagSize = ((buf[1103]) + (buf[1104] *256) + (buf[1105] * 0x10000) + (buf[1106] *0x1000000));
            bExistWebKey = (buf[1107] == 1);
            nWebUserCount = (buf[1108] + buf[1109] * 0x100);

            if (bUseTime == 1)
            {
                tSoftLockFrom.wYear = (ushort)(buf[902] + buf[903] * 256);
                tSoftLockFrom.wMonth = (ushort)(buf[904] + buf[905] * 256);
                tSoftLockFrom.wDay = (ushort)(buf[906] + buf[907] * 256);
                tSoftLockTo.wYear = (ushort)(buf[908] + buf[909] * 256);
                tSoftLockTo.wMonth = (ushort)(buf[910] + buf[911] * 256);
                tSoftLockTo.wDay = (ushort)(buf[912] + buf[913] * 256);

                sSerialNumber += String.Format("({0}-{1}-{2})", tSoftLockTo.wYear, tSoftLockTo.wMonth, tSoftLockTo.wDay);

                DateTime t = DateTime.Now;
                int days = (int)(TimeUtil.GetDayHap(t) - TimeUtil.GetDayHap(tSoftLockTo.wYear, tSoftLockTo.wMonth, tSoftLockTo.wDay));
                if (days > 0)
                {
                    bExistLocalKey = false;
                    bExistWebKey = false;
                    return false;
                }
            }

            cLockType = LOCK_TYPE.AUTO_LOCK;

            return true;
        }*/

        /*
        ProtocolStream2Client myProtocol = new ProtocolStream2Client();
        DeviceTcp device = null;
        void Init()
        {
            if (device != null)
                UnInit();

            device = new DeviceTcp();

            if (!device.Init(false, myProtocol, "127.0.0.1", 7557))
            {
                MessageBox.Show("Check the KeyServer program or computer name.\n\n" + device.sMessage, "Connection error");
                device = null;
                return;
            }
        }

        void UnInit()
        {
            if (device == null) return;

            device.UnInit();
            device = null;
        }

        byte[] SendRecv(string text)
        {
            byte[] send;
            byte[] recv;

            send = Encoding.UTF8.GetBytes(text);

            bool retn = myProtocol.SendRecvBlock(send, out recv);

            return recv;
        }
        
        bool CheckLockNetLock()
        {
            Init();

            return false;
        }*/
	}
}
