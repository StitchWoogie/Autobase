using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using WebTools.ServiceReferenceBing;
using System.Web;

namespace WebTools
{
    public class TranslateEngine
    {
        public bool TranslateByYahoo(string search, out string result)
        {
            result = "";

            string translationMode = "ko_en";
            string url = String.Format("http://kr.babelfish.yahoo.com/translate_txt?lp={0}&tt=urltext&intl=1&doit=done&urltext={1}", translationMode, HttpUtility.UrlEncode(search, Encoding.Default));
            WebClient webClient = new WebClient();

            webClient.Encoding = Encoding.Default;

            string body = webClient.DownloadString(url);

            //string start_code = "<input type=\"hidden\" name=\"p\" value=\"";
            string start_code = "<div id=\"result\"><div style=\"padding:0.6em;\">";
            int index = body.IndexOf(start_code);

            if (index != -1)
            {
                result = "";
                for (int i = index + start_code.Length; i < body.Length; i++)
                {
                    if (body[i] == '<')
                    {
                        return true;
                    }
                    else
                    {
                        result += body[i];
                    }
                }
            }

            return false;
        }

        public bool TranslateByGoogle(string search, out string result)
        {
            result = "";

            string translationMode = "ko|en";
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", HttpUtility.UrlEncode(search, Encoding.UTF8), translationMode);
            WebClient webClient = new WebClient();

            webClient.Encoding = Encoding.Default;

            string body = webClient.DownloadString(url);

            string start_code = "<span id=result_box class=\"short_text\">";
            int index = body.IndexOf(start_code);
            if (index == -1)
            {
                start_code = "<span id=result_box class=\"long_text\">";
                index = body.IndexOf(start_code);
            }

            bool tag_start = false;

            if (index != -1)
            {
                string imsi = "";
                char ch;
                int span_count = 1;
                result = "";
                for (int i = index + start_code.Length; i < body.Length; i++)
                {
                    ch = body[i];

                    if (tag_start)
                    {
                        if (ch == '>')
                        {
                            if (imsi == "/span")
                            {
                                span_count--;
                                if (span_count == 0) return true;
                            }
                            else if (String.Compare("span", 0, imsi, 0, 4) == 0)
                            {
                                span_count++;
                            }

                            imsi = "";
                            tag_start = false;
                        }
                        else
                            imsi += ch;
                    }
               
                    else
                    {
                        if (body[i] == '<')
                        {
                            tag_start = true;
                            result += imsi;
                            imsi = "";
                        }
                        else
                        {
                            imsi += ch;
                        }
                    }
                }
            }

            return false;
        }

        System.ServiceModel.EndpointAddress GetEndPoint()
        {
            string url;

            url = String.Format("http://api.bing.net/soap.asmx");
            
            return new System.ServiceModel.EndpointAddress(url);
        }

        // ServiceReferenceUsername.ServiceUsername2SoapClient service = new WebTools.ServiceReferenceUsername.ServiceUsername2SoapClient(); 식으로 하면 Endpoint를 찾지 못한다.
        // 어차피 Web.Config 에서 변경할 수 있는것이 아니므로 수동으로 EndPoint를 지정해 주었다.
        BingPortTypeClient GetServiceUsername()
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;

            BingPortTypeClient service = new BingPortTypeClient(binding, GetEndPoint());

            return service;
        }

        SearchRequest BuildRequest(string search, int lang_no)
        {
            // 서비스를 추가할 때는 http://api.bing.net/search.wsdl?AppID=9B21B081BDFA6A43B0FF4A95648CDAC1D8BEED15&Version=2.2  로 해야한다.
            string AppId = "9B21B081BDFA6A43B0FF4A95648CDAC1D8BEED15";
            SearchRequest request = new SearchRequest();

            // Common request fields (required)
            request.AppId = AppId;
            request.Query = search;
            request.Sources = new SourceType[] { SourceType.Translation };

            // SourceType-specific fields (required)
            request.Translation = new TranslationRequest();
            request.Translation.SourceLanguage = "Ko";

            if (lang_no == 2)
                request.Translation.TargetLanguage = "zh-CHS";
            else if (lang_no == 3)
                request.Translation.TargetLanguage = "Ja";
            else if (lang_no == 4)
                request.Translation.TargetLanguage = "Vi";
            else
                request.Translation.TargetLanguage = "En";

            // Common request fields (optional)
            request.Version = "2.2";

            return request;
        }

        public bool TranslateByBing(string search, out string result, int lang_no)
        {
            result = "";

            // BingService implements IDisposable.
            using (BingPortTypeClient service = GetServiceUsername())
            {
                try
                {
                    SearchRequest request = BuildRequest(search, lang_no);

                    // Send the request; display the response.
                    SearchResponse response = service.Search(request);

                    foreach (TranslationResult r in response.Translation.Results)
                    {
                        result += r.TranslatedTerm;
                    }

                    //DisplayResponse(response);
                }

                catch
                {
                    // An exception occurred while accessing the network.
                    //Console.WriteLine(ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
