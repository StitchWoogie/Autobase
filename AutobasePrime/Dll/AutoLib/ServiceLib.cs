using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;
using System.ServiceModel;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;

namespace AutoLib
{
    

    public class ServiceLib
    {
        
        static bool bKeepConnection = false;        // Connection을 끊지않고 계속유지해도 통신이 잘되는 것은 확인했다. 하지만 호환성을 위해서 보류한다.
                                                    // 2016-11-15

        public static string sCookieName = null;

        public static Dictionary<string, string> dicVars = new Dictionary<string, string>();

        public static void DicToString(out string[] keys, out string[] values)
        {
            values = dicVars.Values.ToArray();
            keys = dicVars.Keys.ToArray();
        }

        public static string[] StringToExcelReportString(string[] source)
        {
            return source;
        }

        public static string[] StringToReportString(string[] source)
        {
            return source;
        }
        //public static AutoLib.ServiceReferenceExcelReport.ArrayOfString StringToExcelReportString(string[] source)
        //{
        //    AutoLib.ServiceReferenceExcelReport.ArrayOfString array = new AutoLib.ServiceReferenceExcelReport.ArrayOfString();

        //    for(int i = 0; i < source.Length; i++) {
        //        array.Add(source[i]);
        //    }

        //    return array;
        //}

        //public static AutoLib.ServiceReferenceReport.ArrayOfString StringToReportString(string[] source)
        //{
        //    AutoLib.ServiceReferenceReport.ArrayOfString array = new AutoLib.ServiceReferenceReport.ArrayOfString();

        //    for (int i = 0; i < source.Length; i++)
        //    {
        //        array.Add(source[i]);
        //    }

        //    return array;
        //}

        static System.ServiceModel.EndpointAddress GetEndPoint(string service_name)
        {
            string url = AutoLibLocal.ConfigVarTotal.GetServicePath(service_name);
            
            System.ServiceModel.EndpointAddress ep = new System.ServiceModel.EndpointAddress(url);

            return ep;
        }

        static BasicHttpBinding GetBinding()
        {
            /*
            if (bKeeyConnection)
            {
                if (saveBidning != null) return saveBidning;
            }*/

            System.ServiceModel.BasicHttpBinding binding;
            if(ConfigVarTotal.bSSL)
                binding = new System.ServiceModel.BasicHttpBinding(BasicHttpSecurityMode.Transport);
            else
                binding = new System.ServiceModel.BasicHttpBinding();

            //TimeSpan ts1 = binding.SendTimeout;     // Default 1분
            //TimeSpan ts2 = binding.ReceiveTimeout;  // Default 10분
            //TimeSpan ts3 = binding.OpenTimeout;     // Default 1분
            //TimeSpan ts4 = binding.CloseTimeout;    // Default 1분

            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;

            /*
            if (bKeeyConnection)
            {
                saveBidning = binding;
            }*/

            return binding;
        }

        static ServiceReferenceDataSet.ServiceDataSetClient saveServiceDataSet = null;
        static ServiceReferenceDataTag.ServiceDataTagClient saveServiceDataTag = null;
        static ServiceReferenceDataTag2.WebServiceDataTag2Client saveServiceDataTag2 = null;

        static ServiceReferenceDownLoadProject.ServiceDownLoadProjectClient saveServiceDownLoadProject = null;
        static ServiceReferenceOdbc.ServiceOdbcClient saveServiceOdbc = null;
        static ServiceReferenceUserProtect.ServiceUserProtectClient saveServiceUserProtect = null;
        static ServiceReferenceDataGateServer.WcfServiceDataGateServerProxyClient saveServiceDataGateServer = null;
        static ServiceReferenceExcelReport.ServiceExcelReportClient saveServiceExcelReport = null;
        static ServiceReferenceReport.ServiceReportClient saveServiceReport = null;

        static ServiceReferenceService3.WebService3Client saveService3 = null;

        static ServiceReferenceRecipe.WcfServiceRecipeClient saveServiceRecipe = null;
        static ServiceReferencePreset.WcfServicePresetClient saveServicePreset = null;

        // 사이트가 바뀔때마다 접속을 리셋해야 한다.
        public static void ClearConnection()
        {
            saveServiceDataSet = null;
            saveServiceDataTag = null;
            saveServiceDataTag2 = null;

            saveServiceDownLoadProject = null;
            saveServiceOdbc = null;
            saveServiceUserProtect = null;
            saveServiceDataGateServer = null;
            saveServiceExcelReport = null;
            saveServiceReport = null;

            saveService3 = null;

            saveServiceRecipe = null;
            saveServicePreset = null;
        }

        public static ServiceReferenceDataSet.ServiceDataSetClient GetServiceDataSet()
        {
            if (bKeepConnection)
            {
                if (saveServiceDataSet != null) return saveServiceDataSet;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            ServiceReferenceDataSet.ServiceDataSetClient service = new ServiceReferenceDataSet.ServiceDataSetClient(binding, GetEndPoint("ServiceDataSet.svc"));

            if (bKeepConnection)
            {
                saveServiceDataSet = service;
            }

            return service;
        }

        public static ServiceReferenceDataTag.ServiceDataTagClient GetServiceDataTag()
        {
            if (bKeepConnection)
            {
                if (saveServiceDataTag != null) return saveServiceDataTag;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            ServiceReferenceDataTag.ServiceDataTagClient service = new ServiceReferenceDataTag.ServiceDataTagClient(binding, GetEndPoint("ServiceDataTag.svc"));

            if (bKeepConnection)
            {
                saveServiceDataTag = service;
            }

            return service;
        }

        public static ServiceReferenceDataTag2.WebServiceDataTag2Client GetServiceDataTag2()
        {
            if (bKeepConnection)
            {
                if (saveServiceDataTag2 != null) return saveServiceDataTag2;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            ServiceReferenceDataTag2.WebServiceDataTag2Client service = new ServiceReferenceDataTag2.WebServiceDataTag2Client(binding, GetEndPoint("WebServiceDataTag2.svc"));

            if (bKeepConnection)
            {
                saveServiceDataTag2 = service;
            }

            return service;
        }

        public static ServiceReferenceDownLoadProject.ServiceDownLoadProjectClient GetServiceDownLoadProject()
        {
            if (bKeepConnection)
            {
                if (saveServiceDownLoadProject != null) return saveServiceDownLoadProject;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            ServiceReferenceDownLoadProject.ServiceDownLoadProjectClient service = new ServiceReferenceDownLoadProject.ServiceDownLoadProjectClient(binding, GetEndPoint("ServiceDownLoadProject.svc"));

            if (bKeepConnection)
            {
                saveServiceDownLoadProject = service;
            }
            
            return service;
        }

        public static ServiceReferenceOdbc.ServiceOdbcClient GetServiceOdbc()
        {
            if (bKeepConnection)
            {
                if (saveServiceOdbc != null) return saveServiceOdbc;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            ServiceReferenceOdbc.ServiceOdbcClient service = new ServiceReferenceOdbc.ServiceOdbcClient(binding, GetEndPoint("ServiceOdbc.svc"));

            if (bKeepConnection)
            {
                saveServiceOdbc = service;
            }

            return service;
        }

        public static ServiceReferenceUserProtect.ServiceUserProtectClient GetServiceUserProtect()
        {
            if (bKeepConnection)
            {
                if (saveServiceUserProtect != null) return saveServiceUserProtect;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            ServiceReferenceUserProtect.ServiceUserProtectClient service = new ServiceReferenceUserProtect.ServiceUserProtectClient(binding, GetEndPoint("ServiceUserProtect.svc"));

            if (bKeepConnection)
            {
                saveServiceUserProtect = service;
            }

            return service;
        }

        public static ServiceReferenceExcelReport.ServiceExcelReportClient GetServiceExcelReport()
        {
            if (bKeepConnection)
            {
                if (saveServiceExcelReport != null) return saveServiceExcelReport;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            // 2017-1-19 추가함. 1분을 넘음 (중국현장) 로컬상에서는 이상하게 오래 걸림. 3분이상. 1번시트를 깨끗하게 비우면 30초 정도에 끝남.
            binding.SendTimeout = new TimeSpan(0, ConfigViewMain.nTimeoutOfExcelReportRunDirect/60, ConfigViewMain.nTimeoutOfExcelReportRunDirect%60);

            ServiceReferenceExcelReport.ServiceExcelReportClient service = new ServiceReferenceExcelReport.ServiceExcelReportClient(binding, GetEndPoint("ServiceExcelReport.svc"));

            if (bKeepConnection)
            {
                saveServiceExcelReport = service;
            }

            return service;
        }

        public static ServiceReferenceReport.ServiceReportClient GetServiceReport()
        {
            if (bKeepConnection)
            {
                if (saveServiceReport != null) return saveServiceReport;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            ServiceReferenceReport.ServiceReportClient service = new ServiceReferenceReport.ServiceReportClient(binding, GetEndPoint("ServiceReport.svc"));

            if (bKeepConnection)
            {
                saveServiceReport = service;
            }

            return service;
        }

        public static async Task<byte[][]> GetReportBitmapPngsAsync(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            ServiceReferenceReport.ServiceReportClient service = GetServiceReport();
            return await service.GetReportBitmapPngsAsync(filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto)
                .ConfigureAwait(false);
        }

        public static async Task<byte[][]> GetReportBitmapPngsWithDicAsync(string filename, DateTime tHand, DateTime tAuto,
            DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            ServiceReferenceReport.ServiceReportClient service = GetServiceReport();

            string[] keys;
            string[] values;
            DicToString(out keys, out values);

            return await service.GetReportBitmapPngsWithDicAsync(
                StringToReportString(keys),
                StringToReportString(values),
                filename,
                tHand,
                tAuto,
                tMinListFr,
                tMinListTo,
                hand_auto).ConfigureAwait(false);
        }

        public static async Task<List<string>> SaveReportBitmapPngsAsync(string filename, string outputDirectory,
            DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            byte[][] pages = await GetReportBitmapPngsAsync(filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto)
                .ConfigureAwait(false);
            return SaveReportBitmapPngFiles(filename, outputDirectory, pages);
        }

        public static async Task<List<string>> SaveReportBitmapPngsWithDicAsync(string filename, string outputDirectory,
            DateTime tHand, DateTime tAuto, DateTime tMinListFr, DateTime tMinListTo, int hand_auto)
        {
            byte[][] pages = await GetReportBitmapPngsWithDicAsync(filename, tHand, tAuto, tMinListFr, tMinListTo, hand_auto)
                .ConfigureAwait(false);
            return SaveReportBitmapPngFiles(filename, outputDirectory, pages);
        }

        public static List<string> SaveReportBitmapPngFiles(string filename, string outputDirectory, byte[][] pages)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new ArgumentException("출력 폴더 경로가 없습니다.", nameof(outputDirectory));
            }

            if (pages == null || pages.Length == 0)
            {
                return new List<string>();
            }

            Directory.CreateDirectory(outputDirectory);

            string safePrefix = Path.GetFileNameWithoutExtension(filename);
            if (string.IsNullOrWhiteSpace(safePrefix))
            {
                safePrefix = "Report";
            }

            List<string> files = new List<string>();
            for (int i = 0; i < pages.Length; i++)
            {
                string fileName = string.Format("{0}_{1:000}.png", safePrefix, i + 1);
                string fullPath = Path.Combine(outputDirectory, fileName);
                File.WriteAllBytes(fullPath, pages[i]);
                files.Add(fullPath);
            }

            return files;
        }

        public static ServiceReferenceDataGateServer.WcfServiceDataGateServerProxyClient GetServiceDataGateServer()
        {
            if (bKeepConnection)
            {
                if (saveServiceDataGateServer != null) return saveServiceDataGateServer;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            int hello = binding.ReaderQuotas.MaxStringContentLength;
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;       // String이 8192가 넘으면 오류가 나서 이부분을 추가하였다. 2016-1-14. 기본값=8192
                                                                            
            ServiceReferenceDataGateServer.WcfServiceDataGateServerProxyClient service = new ServiceReferenceDataGateServer.WcfServiceDataGateServerProxyClient(binding, GetEndPoint("WcfServiceDataGateServerProxy.svc"));

            if (bKeepConnection)
            {
                saveServiceDataGateServer = service;
            }

            return service;
        }

        public static ServiceReferenceService3.WebService3Client GetService3()
        {
            if (bKeepConnection)
            {
                if (saveService3 != null) return saveService3;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            binding.ReaderQuotas.MaxStringContentLength = 2147483647;       // Database return String이 8192가 넘으면 오류가 나서 이부분을 추가하였다. 2016-10-17. 기본값=8192

            ServiceReferenceService3.WebService3Client service = new ServiceReferenceService3.WebService3Client(binding, GetEndPoint("WebService3.svc"));

            if (bKeepConnection)
            {
                saveService3 = service;
            }

            return service;
        }

        public static ServiceReferenceRecipe.WcfServiceRecipeClient GetServiceRecipe()
        {
            if (bKeepConnection)
            {
                if (saveServiceRecipe != null) return saveServiceRecipe;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            ServiceReferenceRecipe.WcfServiceRecipeClient service = new ServiceReferenceRecipe.WcfServiceRecipeClient(binding, GetEndPoint("ServiceRecipe.svc"));

            if (bKeepConnection)
            {
                saveServiceRecipe = service;
            }

            return service;
        }

        public static ServiceReferencePreset.WcfServicePresetClient GetServicePreset()
        {
            if (bKeepConnection)
            {
                if (saveServicePreset != null) return saveServicePreset;
            }

            System.ServiceModel.BasicHttpBinding binding = GetBinding();

            ServiceReferencePreset.WcfServicePresetClient service = new ServiceReferencePreset.WcfServicePresetClient(binding, GetEndPoint("ServicePreset.svc"));

            if (bKeepConnection)
            {
                saveServicePreset = service;
            }

            return service;
        }

        public static string GetKeyEnc()
        {
            long ticks = DateTime.UtcNow.Ticks + ConfigVarTotal.nTickGab;

            string key_enc = ServiceLib.Encrypt(ticks.ToString());

            return key_enc;
        }

        public static string Encrypt(string source)
        {
            string source_enc = AESEncrypt256(source, keyHash);
            return source_enc;
        }

        public static string Decrypt(string source_enc)
        {
            string source = AESDecrypt256(source_enc, keyHash);
            return source;
        }

        public static byte[] Hash(string source)
        {
            if (ConfigVarTotal.nWebServerSecurityLevel == 4) // 256 bit Hash
            {
                byte[] hash_s = Encoding.UTF8.GetBytes(source + "*&%6^0");
                // SHA256 sha256 = new SHA256CryptoServiceProvider(); XP SP3이상에서 지원이 된다고 하는데 안되어서 SHA256Managed를 사용한다. 2017-2-10. XP가 사라질때까지 SHA256Managed로 사용해야 할 듯.
                // SHA256CryptoServiceProvider는 OS레벨에서 지원해서 속도는 빠른것으로 보임
                SHA256 sha = new SHA256Managed();
                byte[] result = sha.ComputeHash(hash_s);

                return result;
            }
            else // 160 bit Hash
            {
                byte[] hash_s = Encoding.UTF8.GetBytes(source + "~!@#");
                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] result = sha.ComputeHash(hash_s);

                return result;
            }
        }

        static readonly byte[] keyHash = new byte[32] { 
            0xa0, 0x20, 0xA1, 0x58, 0x7f, 0xec, 0xaf, 0x5f, 0x1C, 0x34, 0xe5, 0xe7, 0xB7, 0xf6, 0xbB, 0xcc,
            0xec, 0x12, 0xe4, 0x71, 0xa0, 0x20, 0xA1, 0x58, 0x7f, 0xec, 0xaf, 0x5f, 0x1C, 0x34, 0xe5, 0xe7};

        //AES_256 암호화
        static String AESEncrypt256(String Input, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Convert.ToBase64String(xBuff);
            return Output;
        }


        //AES_256 복호화
        static String AESDecrypt256(String Input, byte[] key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Encoding.UTF8.GetString(xBuff);
            return Output;
        }
    }
}
