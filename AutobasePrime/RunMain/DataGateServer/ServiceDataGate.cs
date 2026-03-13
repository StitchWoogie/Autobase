using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using System.IO;
using NetTools;
using System.Security.Cryptography;
using NetTools.Hash;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PortalServerWeb.AutoWeb.Service
{
    // 서비스 타입 구현
    public class ServiceDataGate : IServiceDataGate
    {
        public string GetServerVersion()
        {
            return Application.ProductVersion;
        }

        public int Connect()
        {
            return DataGateClient.Connect();
        }

        public void DisConnect(int id)
        {
            DataGateClient.DisConnect(id);
        }

        bool LocalDefaultUserCheck(out string username, out string err_msg, int need_version)
        {
            err_msg = "";

            DataLocal local = new DataLocal();

            bool retn = local.DefaultUserCheck(out err_msg, out username);

            if (retn)
            {
                //if (!PlusUserCount(out err_msg, need_version)) return false;
            }

            return retn;
        }

        bool CheckHashAndID(MethodBase mb, int id, string source, string hash, out string err_msg)
        {
            MakeHashCrc mhc = new MakeHashCrc();
            string h = mhc.ComputeHash(mb.Name+id.ToString()+source);

            if (h != hash)
            {
                err_msg = String.Format("Hash mismatched at {0} method", mb.Name);
                return false;
            }

            if (!DataGateClient.CheckID(id))
            {
                err_msg = String.Format("Connection id {0} is not exists.", id);
                return false;
            }

            err_msg = "";
            return true;
        }

        public bool DefaultUserCheck(int id, string hash, out string username, out string err_msg)
        {
            username = "";
            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, "", hash, out err_msg)) return false;

            return LocalDefaultUserCheck(out username, out err_msg, 0);
        }

        bool LocalCheckUserName(string username, string password, out string err_msg, int need_version)
        {
            err_msg = "";

            DataLocal local = new DataLocal();

            bool retn = local.CheckUserName(out err_msg, username, password);
            if (retn == true)
            {
                //if (!PlusUserCount(out err_msg, need_version)) return false;
            }
            return retn;
        }

        const int NEED_VERSION = 8; // 2008년 이상 키만 사용가능

        public bool CheckUserNameWithVersion(int id, string username, string password, string hash, out string err_msg)
        {
            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, username + password, hash, out err_msg)) return false;

            return LocalCheckUserName(username, password, out err_msg, NEED_VERSION);
        }
        
        void RecurseProjectFilesInfo(string root_dir, string dir, List<ProjectFilesInfo2> array)
        {
            DirectoryInfo infodir = new DirectoryInfo(dir);

            foreach (DirectoryInfo di in infodir.GetDirectories())
            {
                RecurseProjectFilesInfo(root_dir, di.FullName, array);
            }

            foreach (FileInfo fi in infodir.GetFiles("*.*"))
            {
                string ext = fi.Extension;
                //if (String.Compare(ext, ".ANI", true) == 0) continue;
                if (String.Compare(ext, ".PCX", true) == 0) continue;
                //if (String.Compare(ext, ".GIF", true) == 0) continue;
                //if (String.Compare(ext, ".BMP", true) == 0) continue;
                //if (String.Compare(ext, ".TIF", true) == 0) continue;
                //if (String.Compare(ext, ".JPG", true) == 0) continue;
                //if (String.Compare(ext, ".WMF", true) == 0) continue;
                //if (String.Compare(ext, ".EMF", true) == 0) continue;

                ProjectFilesInfo2 file = new ProjectFilesInfo2();
                file.filename = fi.FullName.Substring(root_dir.Length + 1);
                file.filesize = fi.Length;
                file.filetime = fi.LastWriteTimeUtc;
                array.Add(file);
            }
        }

        string GetWebProjectFolder()
        {
            string project_dir = TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebCopyTarget", "C:\\WebRoot");

            project_dir = String.Format("{0}\\AutoWeb\\Project", project_dir);

            return project_dir;
        }

        public List<ProjectFilesInfo2> GetProjectFilesInfo(int id, string hash)
        {
            string err_msg;

            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, "", hash, out err_msg)) return null;

            string project_dir = GetWebProjectFolder();

            List<ProjectFilesInfo2> array = new List<ProjectFilesInfo2>();

            RecurseProjectFilesInfo(project_dir, project_dir, array);

            return array;
        }

        public byte[] DownLoadWithData(int id, string filename, string hash)
        {
            string err_msg;
            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, filename, hash, out err_msg)) return null;

            string path;
            //DateTime dt;

            string project_dir = GetWebProjectFolder();

            path = String.Format("{0}\\{1}", project_dir, filename);
            if (!File.Exists(path))
            {
                //result = (int)ResultType.notfound;
                return null;
            }

            FileInfo info = new FileInfo(path);

            if (info.Length == 0)	// file size 0
            {
                //result = (int)ResultType.download;
                return null;
            }

            byte[] data = File.ReadAllBytes(path);

            return data;
        }

        public string[] GetTagValues(int id, string[] tags, string hash)
        {
            string err_msg;
            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, "", hash, out err_msg)) return null;

            string[] values = new string[tags.Length];

            for (int i = 0; i < tags.Length; i++)
            {
                SharedTag.GetCurr(tags[i], ref values[i]);
            }

            return values;
        }

        public void WriteCurr(int id, string username, string computer, string tag, string val, string hash)
        {
            string err_msg;
            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, username + computer + tag + val, hash, out err_msg)) return;

            string ip = "?.?.?.?";
            SharedTag.SetCurr(tag, val, username, ip, computer);
        }

        public List<ClassDataGateAlarmFileInfo> GetAlarmLists(int id, string hash)
        {
            string err_msg;

            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, "", hash, out err_msg)) return null;

            //string project_dir = GetWebProjectFolder();

            List<ClassDataGateAlarmFileInfo> array = new List<ClassDataGateAlarmFileInfo>();

            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

			string path = String.Format("{0}\\ALARM", data_dir);

			if(!Directory.Exists(path))	return null;
			
			DirectoryInfo info = new DirectoryInfo(path);

            FileInfo[] fis;
            
            /*
            fis = info.GetFiles("*.AL3");
            Array.Sort(fis, new FileInfoCompare());

			foreach(FileInfo fi in fis) 
			{
				row = dt.NewRow();
				row[0] = fi.Name;
				row[1] = fi.Length/ALARM_FILE_STRUCT.struct_size;
				dt.Rows.Add(row);
			}*/

            fis = info.GetFiles("*.ALMX");
            //Array.Sort(fis, new FileInfoCompare());
			foreach(FileInfo fi in fis) 
			{
                ClassDataGateAlarmFileInfo item = new ClassDataGateAlarmFileInfo();
                item.filename = fi.Name;
                item.alarm_count = Tools.GetLineHap(fi.FullName);
                array.Add(item);
			}

			//ds.Tables.Add(dt);

			return array;
        }

        public string GetAlarmFile(int id, string filename, string hash)
        {
            string err_msg;

            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, filename, hash, out err_msg)) return null;

            DataLocal local = new DataLocal();
            return local.GetAlarmFile(filename).GetXml();
        }

        public string GetLogLists(int id, string hash)
        {
            string err_msg;

            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, "", hash, out err_msg)) return null;

            DataLocal local = new DataLocal();
            return local.GetLogLists().GetXml();
        }

        public string GetLogFile(int id, string filename, string hash)
        {
            string err_msg;

            if (!CheckHashAndID(MethodBase.GetCurrentMethod(), id, filename, hash, out err_msg)) return null;

            DataLocal local = new DataLocal();
            return local.GetLogFile(filename).GetXml();
        }
    }

    public class DataGateClient
    {
        public static List<DataGateClient> arrayClient = new List<DataGateClient>();
        public static Random rand = new Random();

        public int id;
        public string ip;
        public int nFrameSend;
        public int nFrameRecv;
        public object syncLock = new object();  // 다른 thread에서 사용하면 충돌이 나기 때문에 변경시 lock을 해서 사용한다.

        public static int Connect()
        {
            while (true)
            {
            seek_next:
                int id = rand.Next(1, int.MaxValue);
                for (int i = 0; i < arrayClient.Count; i++)
                {
                    if (arrayClient[i].id == id)
                    {
                        goto seek_next;    
                    }
                }

                DataGateClient client = new DataGateClient();
                client.id = id;
                OperationContext context = OperationContext.Current;
                MessageProperties prop = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                client.ip = endpoint.Address;
                
                arrayClient.Add(client);

                return id;
            }
        }

        public static void DisConnect(int id)
        {
            for (int i = 0; i < arrayClient.Count; i++)
            {
                if (arrayClient[i].id == id)
                {
                    //using (arrayClient)
                    //{
                    //arrayClient.RemoveAt(i);
                    //}
                    arrayClient[i].id = -1;
                    return;
                }
            }
        }

        public static bool CheckID(int id)
        {
            for (int i = 0; i < arrayClient.Count; i++)
            {
                if (arrayClient[i].id == id)
                {
                    arrayClient[i].nFrameRecv++;
                    return true;
                }
            }

            return false;
        }

    }
}
