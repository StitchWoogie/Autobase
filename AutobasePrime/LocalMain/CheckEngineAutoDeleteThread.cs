using AutoLibLocal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalMain
{
    //class CheckEngineAutoDeleteThread
    //{
    //    static Thread threadWriteCheck;

    //    public static void Init()
    //    {
    //        threadWriteCheck = new Thread(new ThreadStart(ThreadLoopAutoDelete));
    //        threadWriteCheck.Start();
    //    }

    //    public static void UnInit()
    //    {
    //        bEnd = true;

    //        if (threadWriteCheck != null)   // 초기 로그인에서 실패시 걸림
    //        {
    //            threadWriteCheck.Join(5000);
    //        }
    //    }

    //    public static bool bEnd = false;

    //    public static bool bGoAutoDeleteMonthLimit = false;
    //    public static bool bGoAutoDeleteUsedSize = false;

    //    static void ThreadLoopAutoDelete()
    //    {
    //        while (!bEnd)
    //        {
    //            Thread.Sleep(100);

    //            if (bGoAutoDeleteMonthLimit)
    //            {
    //                CheckEngineAutoDeleteDayLimit.DeleteOldDataByMonthLimit();				// 하루가 지나면 자료 저장기간이 지난 파일은 지운다.
    //                bGoAutoDeleteMonthLimit = false;
    //            }
    //            if (bGoAutoDeleteUsedSize)
    //            {
    //                CheckEngineAutoDeleteUsedSize.DeleteOldDataByDiskUsed();
    //                bGoAutoDeleteUsedSize = false;
    //            }

    //        }
    //    }
    //}

    public static class CheckEngineAutoDeleteThread
    {
        private static CancellationTokenSource _cts;
        private static Task _workerTask;

        public static bool bGoAutoDeleteMonthLimit = false;
        public static bool bGoAutoDeleteUsedSize = false;

        public static void Init()
        {
            _cts = new CancellationTokenSource();
            _workerTask = Task.Run(() => ThreadLoopAutoDeleteAsync(_cts.Token));
        }

        public static void UnInit()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _workerTask?.Wait(5000);  // 최대 5초 기다림
            }
        }

        private static async Task ThreadLoopAutoDeleteAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(100, token);

                    if (bGoAutoDeleteMonthLimit)
                    {
                        await CheckEngineAutoDeleteDayLimit.DeleteOldDataByMonthLimitAsync(token);
                        bGoAutoDeleteMonthLimit = false;
                    }

                    if (bGoAutoDeleteUsedSize)
                    {
                        await CheckEngineAutoDeleteUsedSize.DeleteOldDataByDiskUsedAsync(token);
                        bGoAutoDeleteUsedSize = false;
                    }
                }
                catch (OperationCanceledException)
                {
                    // 종료 시 정상적으로 빠져나오기
                    break;
                }
                catch (Exception ex)
                {
                    SmLog.LogError(LogCategory.DATA_DELETE, $"AutoDelete Thread Error: {ex.Message}", ex);
                }
            }
        }
    }

}
