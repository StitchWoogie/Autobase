using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Contracts
{
    /// <summary>
    /// IPC 요청 처리 결과 코드
    /// </summary>
    public enum IpcResultCode
    {
        /// <summary>정상 처리</summary>
        Ok = 0,

        /// <summary>요청 인자 오류 (null, empty, 형식 오류 등)</summary>
        InvalidArgument = 1,

        /// <summary>대상(Server / Group / Item 등) 없음</summary>
        NotFound = 2,

        /// <summary>대상은 있으나 현재 상태에서 수행 불가</summary>
        InvalidState = 3,

        /// <summary>값이 아직 준비되지 않음 (Subscription 미수신)</summary>
        NotReady = 4,

        /// <summary>요청이 모호함 (ItemName 중복 등)</summary>
        Ambiguous = 5,

        /// <summary>타임아웃</summary>
        Timeout = 6,

        /// <summary>내부 처리 오류</summary>
        InternalError = 9
    }
}
