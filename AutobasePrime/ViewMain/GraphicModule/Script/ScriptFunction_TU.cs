using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Net.Sockets;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;

//20241111 PSU 텔레그램 봇 API 메시지 전송.
/// <summary>
/// 텔레그램 메시지 전송을 위한 큐 관리 클래스
/// 채널 : 참여자수 무제한, 관리자만 메시지 전송 가능, 정보전달 및 공지 형태
///        채널에 참여한 사용자에게 1대1 수신 개념.
///        채널에 메시지를 개시할 경우, 참여자수 상관없이 1개의 메시지 전송으로 처리됨.
/// 그룹 : 참여자수 최대200,000명, 모든 사용자간 소통 가능, 
/// </summary>
/// 
namespace GraphicModule
{
    class ScriptFunction_TU
    {
        public const SslProtocols _Tls11_12 = (SslProtocols)0x00000F00;//(SslProtocols)0x00000C00=tls12 0x300 = tls11 0x3000=tls13;    // tls 13은 윈도우즈 7에서 안된다. tls12만하면 7에서는 되는데 10에서는 안된다.
        public const SecurityProtocolType Tls11_12 = (SecurityProtocolType)_Tls11_12;

        // 메시지를 저장할 큐
        private static Queue<TelegramMessage> messageQueue = new Queue<TelegramMessage>();
        // 큐 접근을 위한 동기화 객체
        private static readonly object queueLock = new object();
        // 새 메시지 도착을 알리는 이벤트
        private static ManualResetEvent messageEvent = new ManualResetEvent(false);
        // 프로그램 종료 시 사용할 이벤트
        private static ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        // 처리 스레드 실행 상태
        private static bool isProcessing = false;
        // 큐의 최대 크기
        private const int MAX_QUEUE_SIZE = 1000;
        // 현재 메시지 처리 상태를 추적하기 위한 필드 추가
        private static volatile bool isMessageBeingProcessed = false;
        private static readonly object processingLock = new object();

        private static readonly Dictionary<string, DateTime> lastMessageTimeByBot = new Dictionary<string, DateTime>();
        private static readonly object botTimeLock = new object();
        private const int MIN_DELAY_SAME_BOT = 2000; // 동일 봇 최소 간격 (2초)

        /// <summary>
        /// 텔레그램 메시지 정보를 담는 클래스
        /// </summary>
        public class TelegramMessage
        {
            private string botToken;
            private string chatId;
            private string message;

            public string BotToken
            {
                get { return botToken; }
                set { botToken = value; }
            }
            public string ChatId
            {
                get { return chatId; }
                set { chatId = value; }
            }
            public string Message
            {
                get { return message; }
                set { message = value; }
            }
        }

        public class TelegramFileMessage : TelegramMessage
        {
            private string filePath;
            private string caption;

            public string FilePath
            {
                get { return filePath; }
                set { filePath = value; }
            }

            public string Caption
            {
                get { return caption; }
                set { caption = value; }
            }
        }

        /// <summary>
        /// 메시지 처리 시스템을 초기화합니다.
        /// 처리 스레드가 시작되지 않은 경우에만 새로운 스레드를 시작합니다.
        /// </summary>
        public static void Initialize()
        {
            if (!isProcessing)
            {
                isProcessing = true;
                // 단일 백그라운드 스레드 시작
                ThreadPool.QueueUserWorkItem(ProcessMessageQueueCallback);
            }
        }


        private static void ProcessMessageQueueCallback(object state)
        {
            while (!shutdownEvent.WaitOne(0))
            {
                TelegramMessage message = null;

                while (isMessageBeingProcessed)
                {
                    Thread.Sleep(100);
                    if (shutdownEvent.WaitOne(0)) return;
                }

                lock (queueLock)
                {
                    if (messageQueue.Count > 0)
                    {
                        message = messageQueue.Dequeue();
                    }
                }

                if (message != null)
                {
                    lock (processingLock)
                    {
                        isMessageBeingProcessed = true;
                    }

                    // 현재 봇의 대기 시간 계산
                    int delayRequired = CalculateRequiredDelay(message.BotToken);
                    if (delayRequired > 0)
                    {
                        Thread.Sleep(delayRequired);
                    }

                    // 메시지 전송
                    if (message is TelegramFileMessage)
                    {
                        SendTelegramFile((TelegramFileMessage)message);
                    }
                    else
                    {
                        SendTelegramMessage(message);
                    }

                    // 봇의 마지막 전송 시간 업데이트
                    UpdateLastMessageTime(message.BotToken);

                    lock (processingLock)
                    {
                        isMessageBeingProcessed = false;
                    }
                }
                else
                {
                    WaitHandle.WaitAny(new WaitHandle[] { messageEvent, shutdownEvent }, 1000);
                    messageEvent.Reset();
                }
            }
        }

        private static int CalculateRequiredDelay(string botToken)
        {
            lock (botTimeLock)
            {
                // 현재 봇의 마지막 전송 시간이 없으면 딜레이 없음
                if (!lastMessageTimeByBot.ContainsKey(botToken))
                {
                    return 0;
                }

                DateTime now = DateTime.Now;
                DateTime lastTime = lastMessageTimeByBot[botToken];

                // 동일 봇의 경우 2초 딜레이 계산
                return (int)Math.Max(0, MIN_DELAY_SAME_BOT - (now - lastTime).TotalMilliseconds);
            }
        }

        private static void UpdateLastMessageTime(string botToken)
        {
            lock (botTimeLock)
            {
                lastMessageTimeByBot[botToken] = DateTime.Now;
            }
        }

        /// <summary>
        /// 텔레그램 API를 통해 실제 메시지를 전송합니다.
        /// </summary>
        /// <param name="message">전송할 메시지 정보</param>
        /// <returns>전송 성공 여부</returns>
        private static bool SendTelegramMessage(TelegramMessage message)
        {
            SecurityProtocolType save = ServicePointManager.SecurityProtocol;
            try
            {
                try
                {
                    ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | Tls11_12;
                }
                catch
                {
                    // TLS 설정 실패 시 무시
                }

                using (WebClient wc = new WebClient())
                {
                    string targetAddress = string.Format("https://api.telegram.org/bot{0}/sendMessage", message.BotToken);
                    NameValueCollection parameters = new NameValueCollection();
                    parameters["chat_id"] = message.ChatId;
                    parameters["text"] = message.Message;
                    parameters["parse_mode"] = "";
                    wc.UploadValues(targetAddress, parameters);
                    return true;
                }
            }
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("Telegram send error: " + ex.Message);
            //    return false;
            //}

            catch (WebException ex)
            {
                string errorMessage = "";

                if (ex.Response != null)
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            string result = reader.ReadToEnd();
                            int errorCode = 0;

                            // 에러 코드 파싱
                            int startIndex = result.IndexOf("\"error_code\":");
                            if (startIndex > -1)
                            {
                                startIndex += 13;
                                int endIndex = result.IndexOf(",", startIndex);
                                if (endIndex > -1)
                                {
                                    string errorCodeStr = result.Substring(startIndex, endIndex - startIndex);
                                    errorCode = int.Parse(errorCodeStr);
                                }
                            }

                            // 에러 설명 파싱
                            string errorDesc = "";

                            startIndex = result.IndexOf("\"description\":\"");
                            if (startIndex > -1)
                            {
                                startIndex += 15;  // "description":" 길이
                                int endIndex = result.IndexOf("\"", startIndex);


                                if (endIndex > startIndex)
                                {
                                    errorDesc = result.Substring(startIndex, endIndex - startIndex);
                                    errorDesc = errorDesc.Replace("\"", "");
                                }
                            }

                            errorMessage = errorCode + ", " + errorDesc;

                        }
                    }
                    catch
                    {
                        // 응답 파싱 실패 시 기본 WebException 메시지 사용
                        errorMessage = ex.Message;
                    }
                }
                else
                {
                    errorMessage = ex.Message;
                }

                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(String.Format("Script Error: @TelegramMsg()\nErrorCode={0}", errorMessage));
                }
                return false;
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(String.Format("Script Error: @TelegramMsg()\n{0}", ex.Message));
                }
                return false;
            }

            finally
            {
                ServicePointManager.SecurityProtocol = save;
            }
        }

        public static int Run_TelegramMsg(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            if (!isProcessing)
            {
                Initialize(); // 첫 사용 시 초기화
            }

            string botToken = (string)args[0];
            string chatId = (string)args[1];
            string message = (string)args[2];
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                try
                {
                    lock (queueLock)
                    {
                        if (messageQueue.Count >= MAX_QUEUE_SIZE)
                        {
                            if (Tools.IsLangKorean())
                                scriptClass.ErrorMessage(String.Format("@TelegramMsg 오류\n메시지큐 최대개수 초과, {0}", MAX_QUEUE_SIZE));
                            else
                                scriptClass.ErrorMessage(String.Format("@TelegramMsg Error\nMaximum number of message queues exceeded, {0}", MAX_QUEUE_SIZE));
                            return -1;
                        }

                        TelegramMessage msg = new TelegramMessage();
                        msg.BotToken = botToken;
                        msg.ChatId = chatId;
                        msg.Message = message;
                        // msg.EnqueueTime = DateTime.Now;
                        // msg.RetryCount = 0;

                        messageQueue.Enqueue(msg);
                    }

                    // 처리 스레드에 새 메시지 알림
                    messageEvent.Set();
                    return 1;
                }
                catch (Exception ex)
                {
                    if (Tools.IsLangKorean())
                        scriptClass.ErrorMessage(String.Format("@TelegramMsg 오류\n{0}", ex.Message));
                    else
                        scriptClass.ErrorMessage(String.Format("@TelegramMsg Error\n{0}", ex.Message));

                    return -1;
                }
            }
            return 1;
        }

        /// <summary>
        /// 텔레그램 API를 통해 이미지를 전송합니다.
        /// </summary>
        /// <param name="message">전송할 이미지 메시지 정보</param>
        /// <returns>전송 성공 여부</returns>
        private static bool SendTelegramFile(TelegramFileMessage message)
        {
            SecurityProtocolType save = ServicePointManager.SecurityProtocol;
            try
            {
                try
                {
                    ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | Tls11_12;
                }
                catch
                {
                    // TLS 설정 실패 시 무시
                }

                if (!File.Exists(message.FilePath))
                {
                    if (ConfigViewMain.bScriptErrorMessageShow)
                    {
                        MessageDisplay.Show("Script Error: @TelegramFile()\nFile not found: " + message.FilePath);
                    }
                    return false;
                }

                // 파일 크기 체크 (텔레그램 봇 API 제한: 50MB)
                var fileInfo = new FileInfo(message.FilePath);
                if (fileInfo.Length > 50 * 1024 * 1024)
                {
                    if (ConfigViewMain.bScriptErrorMessageShow)
                    {
                        MessageDisplay.Show("Script Error: @TelegramFile()\nFile size exceeds 50MB limit");
                    }
                    return false;
                }

                string fileExtension = Path.GetExtension(message.FilePath).ToLower();
                string endPoint;
                string contentType;
                string paramName;

                // 파일 형식에 따라 엔드포인트와 컨텐츠 타입 설정
                switch (fileExtension)
                {
                    // 이미지 형식
                    case ".gif":
                        endPoint = "sendAnimation";
                        contentType = "image/gif";
                        paramName = "animation";
                        break;
                    case ".png":
                        endPoint = "sendPhoto";
                        contentType = "image/png";
                        paramName = "photo";
                        break;
                    case ".jpg":
                    case ".jpeg":
                        endPoint = "sendPhoto";
                        contentType = "image/jpeg";
                        paramName = "photo";
                        break;

                    // 문서 형식
                    case ".pdf":
                        endPoint = "sendDocument";
                        contentType = "application/pdf";
                        paramName = "document";
                        break;
                    case ".svg":
                        endPoint = "sendDocument";
                        contentType = "image/svg+xml";
                        paramName = "document";
                        break;
                    case ".txt":
                        endPoint = "sendDocument";
                        contentType = "text/plain";
                        paramName = "document";
                        break;
                    case ".csv":
                        endPoint = "sendDocument";
                        contentType = "text/csv";
                        paramName = "document";
                        break;
                    case ".doc":
                        endPoint = "sendDocument";
                        contentType = "application/msword";
                        paramName = "document";
                        break;
                    case ".docx":
                        endPoint = "sendDocument";
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        paramName = "document";
                        break;
                    case ".xls":
                        endPoint = "sendDocument";
                        contentType = "application/vnd.ms-excel";
                        paramName = "document";
                        break;
                    case ".xlsx":
                        endPoint = "sendDocument";
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        paramName = "document";
                        break;
                    case ".zip":
                        endPoint = "sendDocument";
                        contentType = "application/zip";
                        paramName = "document";
                        break;

                    // 기타 파일
                    default:
                        endPoint = "sendDocument";
                        contentType = "application/octet-stream";
                        paramName = "document";
                        break;
                }

                string targetAddress = string.Format("https://api.telegram.org/bot{0}/{1}", message.BotToken, endPoint);

                using (WebClient wc = new WebClient())
                {
                    // multipart/form-data 헤더 생성
                    var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                    wc.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);

                    var formData = new StringBuilder(1024);
                    formData.AppendLine("--" + boundary);
                    formData.AppendLine("Content-Disposition: form-data; name=\"chat_id\"");
                    formData.AppendLine();
                    formData.AppendLine(message.ChatId);

                    if (!string.IsNullOrEmpty(message.Caption))
                    {
                        formData.AppendLine("--" + boundary);
                        formData.AppendLine("Content-Disposition: form-data; name=\"caption\"");
                        formData.AppendLine();
                        formData.AppendLine(message.Caption);
                    }

                    formData.AppendLine("--" + boundary);
                    string fileName = Path.GetFileName(message.FilePath);
                    formData.AppendFormat("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n",
                        paramName, fileName);
                    formData.AppendFormat("Content-Type: {0}\r\n", contentType);
                    formData.AppendLine();

                    var headerBytes = Encoding.UTF8.GetBytes(formData.ToString());
                    var footerBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                    // 버퍼 기반의 스트림 처리
                    using (var requestStream = new MemoryStream())
                    {
                        // 헤더 쓰기
                        requestStream.Write(headerBytes, 0, headerBytes.Length);

                        // 파일 데이터를 버퍼를 사용해 청크 단위로 쓰기
                        using (var fileStream = File.OpenRead(message.FilePath))
                        {
                            byte[] buffer = new byte[8192]; // 8KB 버퍼
                            int bytesRead;
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                requestStream.Write(buffer, 0, bytesRead);
                            }
                        }

                        // 푸터 쓰기
                        requestStream.Write(footerBytes, 0, footerBytes.Length);

                        // 전송
                        byte[] result = wc.UploadData(targetAddress, requestStream.ToArray());
                        return true;
                    }
                }
            }
            catch (WebException ex)
            {
                string errorMessage = "";

                if (ex.Response != null)
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            string result = reader.ReadToEnd();
                            int errorCode = 0;

                            int startIndex = result.IndexOf("\"error_code\":");
                            if (startIndex > -1)
                            {
                                startIndex += 13;
                                int endIndex = result.IndexOf(",", startIndex);
                                if (endIndex > -1)
                                {
                                    string errorCodeStr = result.Substring(startIndex, endIndex - startIndex);
                                    errorCode = int.Parse(errorCodeStr);
                                }
                            }

                            string errorDesc = "";
                            startIndex = result.IndexOf("\"description\":\"");
                            if (startIndex > -1)
                            {
                                startIndex += 15;
                                int endIndex = result.IndexOf("\"", startIndex);
                                if (endIndex > startIndex)
                                {
                                    errorDesc = result.Substring(startIndex, endIndex - startIndex);
                                    errorDesc = errorDesc.Replace("\"", "");
                                }
                            }

                            errorMessage = errorCode + ", " + errorDesc;
                        }
                    }
                    catch
                    {
                        errorMessage = ex.Message;
                    }
                }
                else
                {
                    errorMessage = ex.Message;
                }

                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(String.Format("Script Error: @TelegramFile()\nErrorCode={0}", errorMessage));
                }
                return false;
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show(String.Format("Script Error: @TelegramFile()\n{0}", ex.Message));
                }
                return false;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = save;
            }
        }

        public static int Run_TelegramFile(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            if (!isProcessing)
            {
                Initialize();
            }

            string botToken = (string)args[0];
            string chatId = (string)args[1];
            string filePath = (string)args[2];
            string caption = (string)args[3];
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        if (Tools.IsLangKorean())
                            scriptClass.ErrorMessage(String.Format("@TelegramFile 오류\n파일을 찾을 수 없습니다: {0}", filePath));
                        else
                            scriptClass.ErrorMessage(String.Format("@TelegramFile Error\nFile not found: {0}", filePath));
                        return -1;
                    }

                    if (caption != null && caption.Length > 1024)
                    {
                        if (Tools.IsLangKorean())
                            scriptClass.ErrorMessage("@TelegramFile 오류\n캡션은 1024자를 초과할 수 없습니다.");
                        else
                            scriptClass.ErrorMessage("@TelegramFile Error\nCaption cannot exceed 1024 characters.");
                        return -1;
                    }

                    lock (queueLock)
                    {
                        if (messageQueue.Count >= MAX_QUEUE_SIZE)
                        {
                            if (Tools.IsLangKorean())
                                scriptClass.ErrorMessage(String.Format("@TelegramFile 오류\n메시지큐 최대개수 초과, {0}", MAX_QUEUE_SIZE));
                            else
                                scriptClass.ErrorMessage(String.Format("@TelegramFile Error\nMaximum number of message queues exceeded, {0}", MAX_QUEUE_SIZE));
                            return -1;
                        }

                        TelegramFileMessage msg = new TelegramFileMessage();
                        msg.BotToken = botToken;
                        msg.ChatId = chatId;
                        msg.FilePath = filePath;
                        msg.Caption = caption;

                        messageQueue.Enqueue(msg);
                    }

                    messageEvent.Set();
                    return 1;
                }
                catch (Exception ex)
                {
                    if (Tools.IsLangKorean())
                        scriptClass.ErrorMessage(String.Format("@TelegramFile 오류\n{0}", ex.Message));
                    else
                        scriptClass.ErrorMessage(String.Format("@TelegramFile Error\n{0}", ex.Message));

                    return -1;
                }
            }
            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "T";
            prepare.AddMethod(prename, "TelegramMsg", "int", new ScriptExternalRun.DeleMethod(Run_TelegramMsg), "in:string:botToken", "in:string:chatId", "in:string:msg");
            prepare.AddMethod(prename, "TelegramFile", "int", new ScriptExternalRun.DeleMethod(Run_TelegramFile), "in:string:botToken", "in:string:chatId", "in:string:filePath", "in:string:caption");
        }
    }
}
