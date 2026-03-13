using AutoLibLocal;
using NetTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMain
{
    /// <summary>
    /// 프로그램 시작 시 초기화
    /// </summary>
    public class TrendInitializer
    {
        public static async Task<bool> InitializeAsync()
        {
            try
            {
                // 1. 설정 로드
                //ConfigDataDB.LoadConfig(); // DataPostgres instance 생성 시 이미 실행됨.

                // 2. DB 초기화
                bool dbOk = await DatabaseConfigManager.InitializeDatabase();
                if (!dbOk)
                {
                    if(Tools.IsLangKorean())
                        MessageDisplay.Show("데이터베이스 초기화 실패");
                    else
                        MessageDisplay.Show("Database initialization failed");
                   // return false;
                }

                // 3. 개선된 저장 매니저 시작
                TrendSaveManager.Instance.Start();

                //// 4. 백업 데이터 자동 복구 시도
                //if (!ConfigData.bSkipDataWhenSavingDelayed)
                //{
                //    var backupManager = new TrendBackupManager();
                //    int recovered = await backupManager.AutoRecoverFromBackupsAsync();

                //    if (recovered > 0)
                //    {
                //        if (Tools.IsLangKorean())
                //            MessageDisplay.Show(string.Format("백업 데이터 {0}개 복구 완료", recovered));
                //        else
                //            MessageDisplay.Show(string.Format("{0} backup data recovered", recovered));
                //    }
                //}

                if (Tools.IsLangKorean())
                    MessageDisplay.Show("트렌드 저장 시스템 초기화 완료");
                else
                    MessageDisplay.Show("Trend saving system initialization completed");

                return true;
            }
            catch (Exception ex)
            {
                if (Tools.IsLangKorean())
                {
                    MessageDisplay.Show(string.Format("초기화 실패: {0}", ex.Message));
                    SmLog.LogError(LogCategory.SYSTEM_START, "초기화 실패", ex);
                }
                else
                {
                    MessageDisplay.Show(string.Format("Initialization failed: {0}", ex.Message));
                    SmLog.LogError(LogCategory.SYSTEM_START, "Initialization failed", ex);
                }
                return false;
            }
        }

        /// <summary>
        /// 프로그램 종료 시 정리
        /// </summary>
        public static async Task ShutdownAsync()
        {
            try
            {
                // 1. 남은 데이터 저장
                await CheckEngineMinuteChanged.SaveRemainDataBeforeExit();

                // 2. DB 연결 종료
                DatabaseConfigManager.ShutdownDatabase();

                //if (Tools.IsLangKorean())
                //    MessageDisplay.Show("트렌드 저장 시스템 종료 완료");
                //else
                //    MessageDisplay.Show("Trend saving system shutdown completed");
                Debug.WriteLine("Trend saving system shutdown completed");
            }
            catch (Exception ex)
            {
                if (Tools.IsLangKorean())
                {
                    //MessageDisplay.Show(string.Format("종료 처리 오류: {0}", ex.Message));
                    SmLog.LogError(LogCategory.SYSTEM_STOP, "종료 처리 오류", ex);
                    Debug.WriteLine(string.Format("Shutdown processing error: {0}", ex.Message));
                }
                else
                {
                    //MessageDisplay.Show(string.Format("Shutdown processing error: {0}", ex.Message));
                    SmLog.LogError(LogCategory.SYSTEM_STOP, "Shutdown processing error", ex);
                    Debug.WriteLine(string.Format("Shutdown processing error: {0}", ex.Message));
                }
            }
        }
    }
}
