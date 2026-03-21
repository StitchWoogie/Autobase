using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Studio.RemoteProjectEditor
{
    /// <summary>
    /// HTTP client for communicating with RemoteProjectAgent on a remote PC.
    /// </summary>
    public class RemoteProjectClient
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string ApiKey { get; set; }
        public int TimeoutMs { get; set; } = 30000;

        private string BaseUrl => $"http://{Host}:{Port}";

        public RemoteProjectClient(string host, int port, string apiKey = "")
        {
            Host = host;
            Port = port;
            ApiKey = apiKey ?? "";
        }

        #region Connection

        public bool Ping()
        {
            try
            {
                var result = GetJson("/api/ping");
                return result != null && result.ContainsKey("status") && result["status"].ToString() == "ok";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tests authentication by calling an endpoint that requires API Key.
        /// Returns: "ok" on success, "unauthorized" on 401, or error message.
        /// </summary>
        public string TestAuthentication()
        {
            try
            {
                var result = GetLocalMainStatus();
                return "ok";
            }
            catch (WebException ex) when (ex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                return "unauthorized";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region Project Operations

        /// <summary>
        /// Downloads the remote project as a zip file and saves to localPath.
        /// </summary>
        public void DownloadProject(string localZipPath)
        {
            using (var client = CreateWebClient())
            {
                client.DownloadFile($"{BaseUrl}/api/project/download", localZipPath);
            }
        }

        /// <summary>
        /// Uploads a zip file to the remote PC and deploys it.
        /// </summary>
        public Dictionary<string, object> UploadProject(string localZipPath)
        {
            byte[] data = File.ReadAllBytes(localZipPath);
            return PostBinary("/api/project/upload", data, "application/zip");
        }

        /// <summary>
        /// Gets the list of files in the remote project.
        /// </summary>
        public Dictionary<string, object> GetProjectFiles()
        {
            return GetJson("/api/project/files");
        }

        #endregion

        #region LocalMain Control

        public Dictionary<string, object> GetLocalMainStatus()
        {
            return GetJson("/api/localmain/status");
        }

        public Dictionary<string, object> RestartLocalMain(string page = "")
        {
            var body = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(page))
                body["page"] = page;
            return PostJson("/api/localmain/restart", body);
        }

        public Dictionary<string, object> StopLocalMain()
        {
            return PostJson("/api/localmain/stop", new Dictionary<string, object>());
        }

        public Dictionary<string, object> StartLocalMain(string page = "")
        {
            var body = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(page))
                body["page"] = page;
            return PostJson("/api/localmain/start", body);
        }

        #endregion

        #region Screen Operations

        /// <summary>
        /// Captures a screenshot of the remote LocalMain/ViewMain window.
        /// Returns a Bitmap image.
        /// </summary>
        public Bitmap CaptureScreen()
        {
            var request = CreateRequest("/api/screen/capture", "GET");
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.ContentType.StartsWith("image/"))
                {
                    using (var stream = response.GetResponseStream())
                    {
                        return new Bitmap(stream);
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Navigates the remote LocalMain to a specific graphic page.
        /// </summary>
        public Dictionary<string, object> NavigateToPage(string pageName)
        {
            var body = new Dictionary<string, object> { { "page", pageName } };
            return PostJson("/api/screen/navigate", body);
        }

        /// <summary>
        /// Gets the list of available graphic pages on the remote project.
        /// </summary>
        public Dictionary<string, object> GetPages()
        {
            return GetJson("/api/screen/pages");
        }

        #endregion

        #region HTTP Helpers

        private WebClient CreateWebClient()
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            if (!string.IsNullOrEmpty(ApiKey))
                client.Headers.Add("X-API-Key", ApiKey);
            return client;
        }

        private HttpWebRequest CreateRequest(string path, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create($"{BaseUrl}{path}");
            request.Method = method;
            request.Timeout = TimeoutMs;
            request.ReadWriteTimeout = TimeoutMs;
            if (!string.IsNullOrEmpty(ApiKey))
                request.Headers.Add("X-API-Key", ApiKey);
            return request;
        }

        private Dictionary<string, object> GetJson(string path)
        {
            var request = CreateRequest(path, "GET");
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = int.MaxValue;
                return serializer.Deserialize<Dictionary<string, object>>(json);
            }
        }

        private Dictionary<string, object> PostJson(string path, Dictionary<string, object> body)
        {
            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(body);
            byte[] data = Encoding.UTF8.GetBytes(json);

            var request = CreateRequest(path, "POST");
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string responseJson = reader.ReadToEnd();
                return serializer.Deserialize<Dictionary<string, object>>(responseJson);
            }
        }

        private Dictionary<string, object> PostBinary(string path, byte[] data, string contentType)
        {
            var request = CreateRequest(path, "POST");
            request.ContentType = contentType;
            request.ContentLength = data.Length;
            request.Timeout = 300000; // 5 min for large uploads

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<Dictionary<string, object>>(json);
            }
        }

        #endregion
    }
}
