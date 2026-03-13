using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;
using System.Globalization;
using System.Threading;
using AutoLib;
using System.ComponentModel;
using System.Diagnostics;
using NetTools;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ViewMain
{

    public class HeartbeatService
    {
        private static System.Threading.Timer _heartbeatTimer;
        private static readonly int _intervalSeconds = 30;
        private static bool _isRunning = false;
        private static int _consecutiveFailures = 0;
        private static readonly int _maxConsecutiveFailures = 3;

        static HeartbeatService()
        {

        }

        public static void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _consecutiveFailures = 0;

            // 30초마다 heartbeat 전송
            _heartbeatTimer = new System.Threading.Timer(
                SendHeartbeatCallback,
                null,
                TimeSpan.FromSeconds(_intervalSeconds), // 첫 실행 지연
                TimeSpan.FromSeconds(_intervalSeconds)  // 반복 간격
            );

            string clientGuid = DataGate.GetClientGuid();
            Debug.WriteLine(string.Format("Heartbeat 서비스 시작됨 - ClientGuid: {0}", clientGuid));
        }

        private static async void SendHeartbeatCallback(object state)
        {
            if (!_isRunning) return;

           await SendHeartbeat();
        }

        public static bool bEnableHeartbeat = true; //웹서버 동시접속자 수 제한 시 필요. 

        private static async Task SendHeartbeat()
        {
            if (!bEnableHeartbeat) return;

            try
            {
                string errorMsg;
                // async/await 대신 동기 방식으로 처리
                bool success;
                 (success, errorMsg) = await DataGate.UpdateHeartbeat();

                if (!success)
                {
                    _consecutiveFailures++;
                    string clientGuid = DataGate.GetClientGuid();

                    Debug.WriteLine(string.Format("Heartbeat 실패 #{0} - ClientGuid: {1}, Error: {2}",
                        _consecutiveFailures, clientGuid, errorMsg));

                    // 연속 실패 횟수가 임계값을 넘으면 세션 만료 처리
                    if (_consecutiveFailures >= _maxConsecutiveFailures)
                    {
                        Stop();
                        HandleSessionExpired(errorMsg);
                        Debug.WriteLine(string.Format("세션 만료 (연속 실패 {0}회): {1}",
                            _consecutiveFailures, errorMsg));
                    }
                }
                else
                {
                    // 성공 시 실패 카운터 리셋
                    _consecutiveFailures = 0;
                    string clientGuid = DataGate.GetClientGuid();
                    Debug.WriteLine(string.Format("Heartbeat 성공 - {0} - ClientGuid: {1}",
                        DateTime.Now.ToString("HH:mm:ss"), clientGuid));
                }
            }
            catch (Exception ex)
            {
                _consecutiveFailures++;
                string clientGuid = DataGate.GetClientGuid();
                string errorMsg = string.Format("Heartbeat 네트워크 오류 #{0}: {1}",
                    _consecutiveFailures, ex.Message);

                Debug.WriteLine(string.Format("{0} - ClientGuid: {1}", errorMsg, clientGuid));

                // 연속 실패 횟수가 임계값을 넘으면 세션 만료 처리
                if (_consecutiveFailures >= _maxConsecutiveFailures)
                {
                    Stop();
                    HandleSessionExpired(errorMsg);
                }
            }
        }

        private static void HandleSessionExpired(string message)
        {
            string clientGuid = DataGate.GetClientGuid();

            // 세션 만료 시 처리 로직
            Debug.WriteLine(string.Format("세션 만료 처리 시작 - ClientGuid: {0}, Message: {1}",
                clientGuid, message));

            WebCommInfo.Reset();
            string msg = Tools.IsLangKorean()
                              ? "서버와의 연결이 끊어졌습니다"
                               : "The connection to the server has been lost";

            TotalConfig.formMain.Invoke(new MethodInvoker(delegate ()
            {
                GraphicModule.ScriptFunctionWeb.procWebClientChangeSite("local");
                MessageBox.Show(msg);
            }));

            Debug.WriteLine(string.Format("Connection Error: {0} - ClientGuid: {1}", message, clientGuid));
        }

        public static void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _consecutiveFailures = 0;

            if (_heartbeatTimer != null)
            {
                _heartbeatTimer.Dispose();
                _heartbeatTimer = null;
            }

            string clientGuid = DataGate.GetClientGuid();
            Debug.WriteLine(string.Format("Heartbeat 서비스 중지됨 - ClientGuid: {0}", clientGuid));
        }
    }
}
