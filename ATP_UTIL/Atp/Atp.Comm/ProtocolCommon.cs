using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NetTools;

namespace Ats.Comm
{
    public class ProtocolCommon
    {
        public bool bUseBlockReceive = false;
        
        /*
        protected enum EnumProtocolStatus
        {
            Idle,
            Waiting,
        }

        protected EnumProtocolStatus eProtocolStatus = EnumProtocolStatus.Idle;

        public bool IsIdle
        {
            get
            {
                return (eProtocolStatus == EnumProtocolStatus.Idle);
            }
        }

        protected void WaitIdle()
        {
            while (true)
            {
                if (IsIdle) break;
                Thread.Sleep(1);
            }
        }*/

        /*
        public EnumProtocolStatus ProtocolStatus
        {
            get
            {
                return eProtocolStatus;
            }
        }*/

        protected DeviceCommon pDevice = null;

        protected int nRecvCount = 0;
        protected byte[] recv_buffer = new byte[1000];

        /*
        public delegate void DelegateCallback(EnumProtocolReturnCode eReturnCode);
        protected DelegateCallback procCallback = null;

        public void SetProcCallback(DelegateCallback proc)
        {
            procCallback = proc;
        }

        public void GoCallback(EnumProtocolReturnCode eReturnCode)
        {
            eProtocolStatus = EnumProtocolStatus.Idle;

            if (procCallback != null)
                procCallback(eReturnCode);
        }*/

        public void SetDevice(DeviceCommon device)
        {
            pDevice = device;
        }

        protected void ClearRecvCount()
        {
            pDevice.Clear();
            nRecvCount = 0;
        }

        public virtual void ExecuteOneChar(int ch)
        {
            
        }

        public virtual void ExecuteOneBlock(byte[] buf, int count)
        {

        }

        protected bool bReceiveWaiting = false;
        protected EnumProtocolReturnCode eReturnCode = EnumProtocolReturnCode.OK;

        public string sErrorMessage;
        public int nTimeOutMiliSec = 2000;

        protected EnumProtocolReturnCode WaitReceiveFrame()
        {
            TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();

            bReceiveWaiting = true;

            while (bReceiveWaiting)
            {
                Thread.Sleep(1);

                // 1000은 Desktop에서 ATS로 처음에 접속할 때 시간초과가 생긴는 경우가 있다.
                if (timeout.IsTimeOut(nTimeOutMiliSec))
                {
                    sErrorMessage = "TimeOut";

                    //eProtocolStatus = EnumProtocolStatus.Idle;
                    return EnumProtocolReturnCode.ErrorTimeOut;
                }

                pDevice.OnThread();
            }

            //eProtocolStatus = EnumProtocolStatus.Idle;
            return eReturnCode;
        }

        protected object pReturnData = null;

        public object ReturnData
        {
            get
            {
                return pReturnData;
            }
        }

        // 디바이스를 오픈하고 통신을 준비한 다음 처음으로 호출
        public virtual void ProtocolInit()
        {

        }

        // 디바이스를 닫기전 호출
        public virtual void ProtocolUnInit()
        {

        }
    }
}
