using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using AutoLibLocal;

namespace RunMain
{
    class LocalServiceMain
    {
        static ServiceHost hostService = null;
        static object syncObj = new object();
        static bool isRestarting = false;
        static bool isShuttingDown = false;

        // 재시작 관리를 위한 변수들
        private static int consecutiveFailures = 0;
        private const int RESTART_DELAY_MS = 60000; // 60초
        private const int MAX_CONSECUTIVE_FAILURES = 10; // 연속 실패 시 백오프 적용

        public static void Init()
        {
            if (!ConfigViewMain.bUseAlarmServer) return;

            StartServiceHost();
        }


        private static void StartServiceHost()
        {
            lock (syncObj)
            {
                try
                {
                    // 기존 서비스 정리
                    CleanupServiceHost();

                    hostService = new ServiceHost(typeof(PortalServerWeb.AutoWeb.Service.ServiceDataGateServer), new Uri("net.tcp://localhost:8732/AutoWeb/Service/ServiceDataGateServer.svc"));

                    // mexBinding을 추가하려면 Behavior가 있어야 한다.
                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    hostService.Description.Behaviors.Add(smb);

                    // Binding
                    NetTcpBinding ntb = new NetTcpBinding();
                    ntb.Security.Mode = SecurityMode.None;
                    ntb.MaxReceivedMessageSize = 2147483647;
                    ntb.MaxBufferSize = 2147483647;
                    ntb.ReaderQuotas.MaxArrayLength = 2147483647;

                    hostService.AddServiceEndpoint(
                            typeof(PortalServerWeb.AutoWeb.Service.IServiceDataGateServer),        // service contract
                            ntb,        // service binding
                            "");                // relative address

                    // Service Reference를 하려면 mexbinding이 필요하다.
                    hostService.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");


                    hostService.Faulted += hostService_Faulted;

                    hostService.Open();

                    // 성공 시 연속 실패 카운터 리셋
                    consecutiveFailures = 0;

                    if (NetTools.Tools.IsLangKorean())
                    {
                        Log.Write(LogLevel.INFO, LogCategory.SYSTEM, "WCF Host(경보 웹서버) 시작됨");
                    }
                    else
                    {
                        Log.Write(LogLevel.INFO, LogCategory.SYSTEM, "WCF Host(Alarm WebServer) started");
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
        }

        private static bool IsPortRelatedError(Exception ex)
        {
            // 포트 충돌 (AddressAlreadyInUseException 직접 체크)
            if (ex is System.ServiceModel.AddressAlreadyInUseException)
                return true;

            // 내부 예외가 SocketException(10048)인지 확인
            if (ex.InnerException is System.Net.Sockets.SocketException)
            {
                System.Net.Sockets.SocketException se = (System.Net.Sockets.SocketException)ex.InnerException;
                return se.ErrorCode == 10048 || se.ErrorCode == 10013; // 포트 충돌, 권한 문제
            }

            return false;
        }

        private static void HandlePortError()
        {
            if (NetTools.Tools.IsLangKorean())
            {
                MessageDisplay.Show("포트 8732가 이미 사용 중이거나 접근 권한이 없습니다.\n다른 프로그램이 해당 포트를 사용 중인지 확인해주세요.", "WCF Host(경보 웹서버) 시작 실패");
                Log.Write(LogLevel.ERROR, LogCategory.SYSTEM, "WCF Host(경보 웹서버) 포트 충돌 - 서비스 시작 안됨");
            }
            else
            {
                MessageDisplay.Show("Port 8732 is already in use or access is denied.\nPlease check if another program is using the port.", "WCF Host(Alarm WebServer) start failed");
                Log.Write(LogLevel.ERROR, LogCategory.SYSTEM, "WCF Host(Alarm WebServer) port conflict - Service not started");
            }
        }

        private static void HandleError(Exception ex)
        {
            consecutiveFailures++;

            // 포트 관련 오류인지 확인하여 적절한 메시지 표시
            bool isPortIssue = IsPortRelatedError(ex);

            string message;
            if (NetTools.Tools.IsLangKorean())
            {
                if (isPortIssue)
                    message = String.Format("WCF Host(경보 웹서버) 포트 문제 (연속 실패 {0}회): 포트 8732가 사용 중이거나 접근 권한 없음", consecutiveFailures);
                else
                    message = String.Format("WCF Host(경보 웹서버) StartService 예외 (연속 실패 {0}회): {1}", consecutiveFailures, ex.Message);

                MessageDisplay.Show(message);
                Log.Write(LogLevel.ERROR, LogCategory.SYSTEM, message);
            }
            else
            {
                if (isPortIssue)
                    message = String.Format("WCF Host(Alarm WebServer) port issue (consecutive failures {0}): Port 8732 in use or access denied", consecutiveFailures);
                else
                    message = String.Format("WCF Host(Alarm WebServer) StartService exception (consecutive failures {0}): {1}", consecutiveFailures, ex.Message);

                MessageDisplay.Show(message);
                Log.Write(LogLevel.ERROR, LogCategory.SYSTEM, message);
            }

            ScheduleRestart();
        }



        private static void ScheduleRestart()
        {
            lock (syncObj)
            {
                if (isShuttingDown) return;  // 종료 중이면 예약 금지
                if (isRestarting) return; // 이미 예약되어 있으면 중복 방지
                isRestarting = true;
            }

            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                // 백오프 전략: 연속 실패가 많을 때 대기 시간 증가 (최대 10분)
                int delayMs = RESTART_DELAY_MS;
                if (consecutiveFailures > MAX_CONSECUTIVE_FAILURES)
                {
                    int backoffMultiplier = Math.Min((consecutiveFailures - MAX_CONSECUTIVE_FAILURES) / 5 + 1, 20); // 최대 20배
                    delayMs = Math.Min(RESTART_DELAY_MS * backoffMultiplier, 600000); // 최대 10분
                }

                System.Threading.Thread.Sleep(delayMs);

                lock (syncObj)
                {
                    if (isShuttingDown) return;
                    isRestarting = false;
                }
                StartServiceHost();
            });
        }


        private static void hostService_Faulted(object sender, EventArgs e)
        {
            consecutiveFailures++;

            if (NetTools.Tools.IsLangKorean())
            {
                MessageDisplay.Show("WCF Host Faulted → 재시작 시도");
                Log.Write(LogLevel.ERROR, LogCategory.SYSTEM, "WCF Host Faulted → 재시작 시도");
            }
            else
            {
                MessageDisplay.Show("WCF Host Faulted → Attempting to restart");
                Log.Write(LogLevel.ERROR, LogCategory.SYSTEM, "WCF Host Faulted → Attempting to restart");
            }

            ScheduleRestart();
        }

        private static void CleanupServiceHost()
        {
            if (hostService != null)
            {
                try
                {
                    // 이벤트 핸들러 해제
                    hostService.Faulted -= hostService_Faulted;

                    // ServiceHost 상태에 따른 적절한 종료
                    switch (hostService.State)
                    {
                        case CommunicationState.Opened:
                            hostService.Close();
                            break;
                        case CommunicationState.Opening:
                        case CommunicationState.Closing:
                        case CommunicationState.Faulted:
                            hostService.Abort();
                            break;
                        case CommunicationState.Created:
                            // 아직 열리지 않은 상태면 그냥 null로 설정
                            break;
                    }
                }
                catch
                {
                    try { hostService.Abort(); }
                    catch { }
                }
                finally
                {
                    hostService = null;
                }
            }
        }

        public static void UnInit()
        {
            lock (syncObj)
            {
                isShuttingDown = true;
                CleanupServiceHost();
            }
        }
    }
}
