using System;
using AutoLib;
using AutoLibLocal;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;

namespace GraphicModule
{
    public class ScriptFunctionHttp
    {
        public static readonly byte[] Key = Encoding.UTF8.GetBytes("AutoBaseRESTAPI0"); // 16바이트 키
        public static readonly byte[] IV = Encoding.UTF8.GetBytes("01PATSEResaBotuA");  // 16바이트 IV

        public static string EncryptAES(string text)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = Key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor())
                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public const SslProtocols _Tls11_12 = (SslProtocols)0x00000F00;//(SslProtocols)0x00000C00=tls12 0x300 = tls11 0x3000=tls13;    // tls 13은 윈도우즈 7에서 안된다. tls12만하면 7에서는 되는데 10에서는 안된다.
        public const SecurityProtocolType Tls11_12 = (SecurityProtocolType)_Tls11_12;


        //private static Dictionary<string, WebHeaderCollection> headerCollections = new Dictionary<string, WebHeaderCollection>();

        [ThreadStatic]
        private static Dictionary<string, WebHeaderCollection> headerCollections;

        static int Run_HttpCreateHeader(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                if (headerCollections == null)
                {
                    headerCollections = new Dictionary<string, WebHeaderCollection>();
                }

                string headerId = args[0].ToString();
                headerCollections[headerId] = new WebHeaderCollection();
                val = 1;
                return 1;
            }
            catch (Exception ex)
            {
                val = 0;
                scriptClass.ErrorMessage(String.Format("@HttpCreateHeader Error: {0}", ex.Message));
                return -1; //20250618 PSU 0 -> -1
            }
        }

        // 제한된 헤더인지 확인하는 헬퍼 메서드
        private static bool IsRestrictedHeader(string headerName)
        {
            string[] restrictedHeaders = new string[] 
            {
                "accept",
                "connection",
                "content-length",
                "content-type",
                "date",
                "expect",
                "host",
                "if-modified-since",
                "range",
                "referer",
                "transfer-encoding",
                "user-agent",
                "proxy-connection"
            };

            return Array.IndexOf(restrictedHeaders, headerName.ToLower()) != -1;
        }

        static int Run_HttpAddHeader(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0; //20250618 PSU
            try
            {
                string headerId = args[0].ToString();
                string name = args[1].ToString();
                string value = args[2].ToString();
                //bool useEncryption = args.Length > 3 && args[3].ToString() == "1";
                bool useEncryption = false;   //20250618 PSU Null 체크
                if (args.Length >= 4 && args[3] != null)
                {
                    useEncryption = args[3].ToString() == "1";
                }

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                if (useEncryption)
                {
                    using (AesManaged aes = new AesManaged())
                    {
                        //Studio.FormConfigHttpHeader 와 동시 변경 필요.
                        aes.Key = Encoding.UTF8.GetBytes("AutoBaseRESTAPI0"); // 16바이트 키
                        aes.IV = Encoding.UTF8.GetBytes("01PATSEResaBotuA");

                        byte[] encryptedBytes = Convert.FromBase64String(value);

                        using (var decryptor = aes.CreateDecryptor())
                        using (var msDecrypt = new MemoryStream(encryptedBytes))
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            value = srDecrypt.ReadToEnd();
                        }
                    }
                }


                if (headerCollections.ContainsKey(headerId))
                {
                    // 헤더값을 그대로 사용  WebHeaderCollection이 내부적으로 적절한 인코딩을 처리합니다.
                    //headerCollections[headerId].Add(name, value);
                    headerCollections[headerId].Set(name, value);  // Set을 사용하면 기존 헤더가 있으면 덮어쓰고, 없으면 새로 추가합니다
                    val = 1;
                    return 1;
                }
                val = 0;
                return 1;
            }
            catch (Exception ex)
            {
                val = 0;
                scriptClass.ErrorMessage(String.Format("@HttpAddHeader Error: {0}", ex.Message));
                return -1; //20250618 PSU 0 -> -1
            }
        }

        static int Run_HttpRemoveHeader(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0; //20250618 PSU
            try
            {
                string headerId = args[0].ToString();

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                if (headerCollections.ContainsKey(headerId))
                {
                    headerCollections.Remove(headerId);
                    val = 1;
                    return 1;
                }
                val = 0;
                return 1;
            }
            catch (Exception ex)
            {
                val = 0;
                scriptClass.ErrorMessage(String.Format("@HttpRemoveHeader Error: {0}", ex.Message));
                return -1; //20250618 PSU 0 -> -1
            }
        }

        private static readonly HttpClient httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public static int Run_HttpLoadHeaderConfig(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0; //20250618 PSU
            try
            {
                string headerId = args[0].ToString();
                string headerFileName = args[1].ToString();

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                if (!headerCollections.ContainsKey(headerId))
                {
                    throw new ArgumentException("Header ID not found");
                }

               // string apiPath = Path.Combine(TotalConfig.sDirWorkProject, "API");
               // string filePath = Path.Combine(apiPath, headerFileName + ".header");

                string filePath = MakeFilePath.Project("API", headerFileName + ".header"); //20250624 PSU 웹클라이언트에서도 읽을 수 있게 변경.

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Header file not found");
                }

                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(new char[] { ',' }, 3);
                        if (parts.Length >= 2)
                        {
                            string name = parts[0];
                            string value = parts[1];
                            bool isEncrypted = parts.Length > 2 && parts[2] == "1";

                            if (isEncrypted)
                            {
                                using (AesManaged aes = new AesManaged())
                                {
                                    aes.Key = Key;
                                    aes.IV = IV;

                                    byte[] encryptedBytes = Convert.FromBase64String(value);
                                    using (var decryptor = aes.CreateDecryptor())
                                    using (var msDecrypt = new MemoryStream(encryptedBytes))
                                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                                    using (var srDecrypt = new StreamReader(csDecrypt))
                                    {
                                        value = srDecrypt.ReadToEnd();
                                    }
                                }
                            }

                            headerCollections[headerId].Set(name, value);
                        }
                    }
                }

                val = 1;
                return 1;
            }
            catch (Exception ex)
            {
                val = 0;
                scriptClass.ErrorMessage(String.Format("@HttpLoadHeaderConfig Error: {0}", ex.Message));
                return -1; //20250618 PSU 0 -> -1
            }
        }

        static async Task<(int, object val)> Run_HttpRequestAsync(ScriptClass scriptClass, string method_name, object[] args)
        {
            try
            {
                string httpMethod = args[0].ToString();
                string url = args[1].ToString();
                string data = httpMethod == "GET" || httpMethod == "DELETE" ? null : args[2].ToString();
                string headerId = args[3].ToString();

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                {
                    return (1, "");
                }

                // HttpRequestMessage 생성
                using (var request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri(url);
                    request.Method = new HttpMethod(httpMethod);

                    // 헤더 적용
                    if (!string.IsNullOrEmpty(headerId) && headerCollections.ContainsKey(headerId))
                    {
                        foreach (string headerName in headerCollections[headerId].AllKeys)
                        {
                            string headerValue = headerCollections[headerId][headerName];

                            try
                            {
                                // Content 헤더와 일반 헤더 구분하여 처리
                                switch (headerName.ToLower())
                                {
                                    case "content-type":
                                        // Content가 있을 때만 설정 (나중에 Content 설정 시 처리)
                                        break;
                                    case "accept":
                                        request.Headers.Accept.Clear();
                                        request.Headers.Accept.ParseAdd(headerValue);
                                        break;
                                    case "user-agent":
                                        request.Headers.UserAgent.Clear();
                                        request.Headers.UserAgent.ParseAdd(headerValue);
                                        break;
                                    case "authorization":
                                        if (System.Net.Http.Headers.AuthenticationHeaderValue.TryParse(headerValue, out var authHeader))
                                        {
                                            request.Headers.Authorization = authHeader;
                                        }
                                        else
                                        {
                                            // 파싱 실패 시 일반 헤더로 처리
                                            request.Headers.TryAddWithoutValidation(headerName, headerValue);
                                        }
                                        break;
                                    case "content-length":
                                    case "host":
                                        // 자동 설정되므로 무시
                                        break;
                                    default:
                                        // 일반 헤더 추가 시도
                                        if (!request.Headers.TryAddWithoutValidation(headerName, headerValue))
                                        {
                                            // Content 헤더일 가능성이 있으므로 나중에 처리
                                        }
                                        break;
                                }
                            }
                            catch (Exception headerEx)
                            {
                                scriptClass.ErrorMessage($"Header error ({headerName}): {headerEx.Message}");
                            }
                        }
                    }

                    // POST, PUT, PATCH 등 Body가 필요한 경우 Content 설정
                    if (!string.IsNullOrEmpty(data))
                    {
                        request.Content = new StringContent(data, Encoding.UTF8);

                        // Content-Type 헤더 처리
                        if (!string.IsNullOrEmpty(headerId) && headerCollections.ContainsKey(headerId))
                        {
                            string contentType = headerCollections[headerId]["content-type"];
                            if (!string.IsNullOrEmpty(contentType))
                            {
                                try
                                {
                                    request.Content.Headers.ContentType =
                                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
                                }
                                catch
                                {
                                    // 파싱 실패 시 기본값 사용
                                    request.Content.Headers.ContentType =
                                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                                }
                            }
                        }
                    }

                    // HTTP 요청 실행
                    using (var response = await httpClient.SendAsync(request))
                    {
                        string result = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            return (1, result);
                        }
                        else
                        {
                            scriptClass.ErrorMessage($"@HttpRequest HTTP Error: {response.StatusCode} - {result}");
                            return (-1, result);
                        }
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                scriptClass.ErrorMessage($"@HttpRequest HttpRequestException Error: {httpEx.Message}");
                return (-1, "");
            }
            catch (TaskCanceledException tcEx)
            {
                if (tcEx.CancellationToken.IsCancellationRequested)
                {
                    scriptClass.ErrorMessage("@HttpRequest Timeout Error: Request was cancelled due to timeout");
                }
                else
                {
                    scriptClass.ErrorMessage($"@HttpRequest Timeout Error: {tcEx.Message}");
                }
                return (-1, "");
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage($"@HttpRequest Error: {ex.Message}");
                return (-1, "");
            }
        }



        #region 동기 요청
        //static int Run_HttpRequest(ScriptClass scriptClass, string method_name, out object val, object[] args)
        //{
        //    HttpWebRequest request = null;
        //    SecurityProtocolType save = ServicePointManager.SecurityProtocol;
        //    val = ""; //20250618 PSU
        //    try
        //    {
        //        try
        //        {
        //            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | Tls11_12;
        //        }
        //        catch
        //        {
        //            // TLS 설정 실패 시 무시
        //        }

        //        string httpMethod = args[0].ToString();
        //        string url = args[1].ToString();
        //        string data = httpMethod == "GET" || httpMethod == "DELETE" ? null : args[2].ToString();
        //        string headerId = args[3].ToString();

        //        if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

        //        request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Method = httpMethod;  // GET, POST, PUT, PATCH, DELETE
        //        request.Timeout = 30000;          // 30초
        //        request.ReadWriteTimeout = 30000;   // 30초

        //        // 헤더 적용
        //        if (!string.IsNullOrEmpty(headerId) && headerCollections.ContainsKey(headerId))
        //        {
        //            foreach (string headerName in headerCollections[headerId].AllKeys)
        //            {
        //                string headerValue = headerCollections[headerId][headerName];

        //                // 헤더 이름에 따른 처리
        //                switch (headerName.ToLower())
        //                {
        //                    case "content-type":
        //                        request.ContentType = headerValue;
        //                        break;
        //                    case "accept":
        //                        request.Accept = headerValue;
        //                        break;
        //                    case "user-agent":
        //                        request.UserAgent = headerValue;
        //                        break;
        //                    case "content-length":
        //                        // Content-Length는 자동 계산되므로 무시
        //                        break;
        //                    case "host":
        //                        // Host는 자동 설정되므로 무시
        //                        break;
        //                    case "expect":
        //                        request.Expect = headerValue;
        //                        break;
        //                    case "connection":
        //                        request.KeepAlive = (headerValue.ToLower() == "keep-alive");
        //                        break;
        //                    case "if-modified-since":
        //                        DateTime modifiedSince;
        //                        if (DateTime.TryParse(headerValue, out modifiedSince))
        //                        {
        //                            request.IfModifiedSince = modifiedSince;
        //                        }
        //                        break;
        //                    default:
        //                        // 나머지 일반 헤더들은 그대로 추가
        //                        try
        //                        {
        //                            request.Headers[headerName] = headerValue;
        //                        }
        //                        catch (Exception headerEx)
        //                        {
        //                            scriptClass.ErrorMessage(String.Format("Header error ({0}): {1}", headerName, headerEx.Message));
        //                        }
        //                        break;
        //                }
        //            }
        //        }

        //        // GET, DELETE가 아닌 경우에만 데이터 전송   // POST, PUT, PATCH 데이터 처리
        //        if (!string.IsNullOrEmpty(data))
        //        {
        //            byte[] byteArray = Encoding.UTF8.GetBytes(data);
        //            request.ContentLength = byteArray.Length;

        //            using (Stream dataStream = request.GetRequestStream())
        //            {
        //                dataStream.Write(byteArray, 0, byteArray.Length);
        //            }
        //        }
        //        // 응답 처리
        //        using (WebResponse response = request.GetResponse())
        //        {
        //            using (Stream dataStream = response.GetResponseStream())
        //            {
        //                using (StreamReader reader = new StreamReader(dataStream))
        //                {
        //                    val = reader.ReadToEnd();
        //                    return 1;
        //                }
        //            }
        //        }
        //    }
        //    catch (WebException webEx)
        //    {
        //        // WebException에서 오류 응답 처리
        //        try
        //        {
        //            using (WebResponse response = webEx.Response)
        //            {
        //                using (Stream dataStream = response.GetResponseStream())
        //                {
        //                    using (StreamReader reader = new StreamReader(dataStream))
        //                    {
        //                        val = "";
        //                        scriptClass.ErrorMessage(String.Format("@HttpRequest WebException Error: {0}", webEx.Message));
        //                        return -1; //20250618 PSU 0 -> -1
        //                    }
        //                }
        //            }
        //        }
        //        catch
        //        {
        //            val = "";
        //            scriptClass.ErrorMessage(String.Format("@HttpRequest WebException Error: {0}", webEx.Message));
        //            return -1; //20250618 PSU 0 -> -1
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        val = "";
        //        scriptClass.ErrorMessage(String.Format("@HttpRequest Error: {0}", ex.Message));
        //        return -1; //20250618 PSU 0 -> -1
        //    }
        //    finally
        //    {
        //        ServicePointManager.SecurityProtocol = save;
        //    }

        //}
        #endregion

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            if (!TotalConfig.GetOemTypeRight(EnumOemTypeRight.Http)) return;  //20250113 PSU

            string prename = "Http";

            prepare.AddMethod(prename, "HttpCreateHeader", "int", new ScriptExternalRun.DeleMethod(Run_HttpCreateHeader), "in:string:headerId");
            prepare.AddMethod(prename, "HttpAddHeader", "int", new ScriptExternalRun.DeleMethod(Run_HttpAddHeader), "in:string:headerId", "in:string:name", "in:string:value", "params:object:useEncryption");
            prepare.AddMethod(prename, "HttpRemoveHeader", "int", new ScriptExternalRun.DeleMethod(Run_HttpRemoveHeader), "in:string:headerId");
            prepare.AddMethod(prename, "HttpLoadHeaderConfig", "int", new ScriptExternalRun.DeleMethod(Run_HttpLoadHeaderConfig), "in:string:headerId", "in:string:headerFileName");
            prepare.AddMethod(prename, "HttpRequest", "string", new ScriptExternalRun.AsyncDeleMethod(Run_HttpRequestAsync), "in:string:method", "in:string:url", "in:string:data", "in:string:headerId");
        }
    }
}
