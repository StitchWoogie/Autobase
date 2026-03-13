using System;
using System.IO;
using System.Net;
using AutoLibLocal;
using NetTools.Hash;
using System.Threading;
using NetTools;
using AutoLib.ServiceReferenceDownLoadProject;

namespace AutoLib
{
	/// <summary>
	/// Summary description for MakeFilePath.
	/// </summary>
	public class MakeFilePath
	{
		public MakeFilePath()
		{
			//
			// TODO: Add constructor logic here
			//
		}


        // 클라이언트에서 태그 로딩 시 잘 안되는 문제가 있어서 WebReference를 ServiceReference로 변경해서 해보았다. 2010.10.8
        static void DownLoadSystemFile(string dir, string filename, bool compare_flag, DateTime file_time, long file_size, string target)
        {
            if(!ConfigVarTotal.bWebServiceAlive)    return; // 2017-4-28 추가함.

            if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
            {
                ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();

                int retn = sldg.Command("V2_DownLoadFileWithCompare", dir, filename, ConvertTool.ToDateTimeString(file_time), file_size);

                if (retn == (int)ServiceReferenceDownLoadProject.ResultType.matched)	// 파일이 같음
                {
                    FileAttributes attr = File.GetAttributes(target);
                    File.SetAttributes(target, attr & ~FileAttributes.Archive);
                    return;
                }
                else if (retn == (int)ServiceReferenceDownLoadProject.ResultType.notfound)	// 파일이 없음
                {
                    if (File.Exists(target))
                    {
                        File.Delete(target);
                    }
                }
                else if (retn == (int)ServiceReferenceDownLoadProject.ResultType.download)
                {
                    string path;

                    path = Path.GetDirectoryName(target); // file명이 ex2\ex.mod 처럼 sub폴더가 포함된 경우가 있으므로 파일명에서 폴더명을 얻는것이 정확하다.

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    if (sldg.GetResultBytes(0) == null)  // 파일 크기가 0인 경우에도 파일을 만들어 주는 것이 좋다.
                    {
                        FileStream fs = File.Open(target, FileMode.Create);//File.OpenWrite(target);
                        fs.Flush();
                        fs.Close();
                    }
                    else
                    {
                        File.WriteAllBytes(target, sldg.GetResultBytes(0));
                    }

                    file_time = ConvertTool.ToDateTime(sldg.GetResultString(1));
                    file_size = ConvertTool.ToInt64(sldg.GetResultString(2));

                    File.SetLastWriteTime(target, file_time);

                    FileAttributes attr = File.GetAttributes(target);
                    File.SetAttributes(target, attr & ~FileAttributes.Archive);
                }
                else
                {

                }
            }
            else
            {

                ServiceReferenceDownLoadProject.ServiceDownLoadProjectClient service = ServiceLib.GetServiceDownLoadProject();

                ServiceReferenceDownLoadProject.ResultType result = 0;

				WcfServiceDownLoadProjectDownloadResponse res;
                try
                {
                    res = service.Download(dir, filename, compare_flag, file_time, file_size);
                    result = res.Result;
                }
                catch (WebException)
                {
                    result = ServiceReferenceDownLoadProject.ResultType.failed;
                    return;
                }

                if (result == 0)//ResultType.failed) // 어떤 이유로 성공못함.
                {
                    return;
                }
                else if (result == ServiceReferenceDownLoadProject.ResultType.matched)	// 파일이 같음
                {
                    FileAttributes attr = File.GetAttributes(target);
                    File.SetAttributes(target, attr & ~FileAttributes.Archive);
                    return;
                }
                else if (result == ServiceReferenceDownLoadProject.ResultType.notfound)	// 파일이 없음
                {
                    if (File.Exists(target))
                    {
                        File.Delete(target);
                    }
                }
                else
                {
                    string path;

                    path = Path.GetDirectoryName(target); // file명이 ex2\ex.mod 처럼 sub폴더가 포함된 경우가 있으므로 파일명에서 폴더명을 얻는것이 정확하다.

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    if (res.Data == null) // 파일 크기가 0인 경우에도 파일을 만들어 주는 것이 좋다.
                    {
                        using (var fs = File.Open(target, FileMode.Create))
                        {
                            fs.Flush();
                        }
                    }
                    else
                    {
                        File.WriteAllBytes(target, res.Data);
                    }

                    // 파일 수정시간 맞추기
                    File.SetLastWriteTime(target, file_time);

                    // Archive 속성 제거
                    FileAttributes attr = File.GetAttributes(target);
                    File.SetAttributes(target, attr & ~FileAttributes.Archive);
                }
            }
            
        }

		public static string Project(string dir, string filename)
		{
			string path;

			if(ConfigVarTotal.bLocalFlag) 
			{
				path = String.Format("{0}\\{1}\\{2}", TotalConfig.sDirWorkProject, dir, filename);
				return path;
			}
			else 
			{
				path = String.Format("{0}\\{1}\\{2}", ConfigVarTotal.sDirConfigTempProject, dir, filename);
			}

			if(File.Exists(path)) 
			{
                if (!ConfigWebView.bCheckProjectEveryConnection) return path;   // 매번 검사할 필요가 없다.

				FileAttributes attr = File.GetAttributes(path);

				if((attr & FileAttributes.Archive) == 0) // file already compared
				{
					return path;
				}

				DateTime dt = File.GetLastWriteTime(path);
				FileInfo info = new FileInfo(path);
				DownLoadSystemFile(dir, filename, true, dt, info.Length, path);
			}
			else 
			{
				DateTime dt = DateTime.Now;
				DownLoadSystemFile(dir, filename, false, dt, 0, path);
			}

			return path;
		}

		public static string Graphic(string filename)
		{
			return Project("graphic", filename);
		}

		public static string Graphic_ObjScr(string filename)
		{
			return Project("graphic\\objscr", filename);
		}

		public static string Tag(string filename)
		{
			return Project("TAG", filename);
		}

		public static string Users(string filename)
		{
			return Project("Users", AutoLibLocal.UserInfoStruct.EncodeUserFilename(filename)+".user");
		}

		public static string Sound(string filename)
		{
			return Project("sound", filename);
		}

		static void RecurseResetFlags(string root_path)
		{
            if (!ConfigWebView.bCheckProjectEveryConnection) return;   // 매번 검사할 필요가 없다.

			if(!Directory.Exists(root_path))	return;
			DirectoryInfo info = new DirectoryInfo(root_path);

			foreach(DirectoryInfo di in info.GetDirectories("*.*")) 
			{
				RecurseResetFlags(di.FullName);
			}

			foreach(FileInfo fi in info.GetFiles("*.*")) 
			{
				FileAttributes attr = fi.Attributes;
				fi.Attributes = (attr | FileAttributes.Archive);
			}
		}

		public static void ResetAllFileFlags()
		{
			if(ConfigVarTotal.bLocalFlag)	return;

			RecurseResetFlags(ConfigVarTotal.sDirConfigTempProject);
		}

		public static string GetProjectDirectory()
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				return TotalConfig.sDirWorkProject;
			}
			else 
			{
				return ConfigVarTotal.sDirConfigTempProject;
			}
		}

		public static TextReader OpenOldNew(string sub_dir, string ascii_file, string unicode_file)
		{
			string filename;
			
			filename = MakeFilePath.Project(sub_dir, unicode_file);

			if(File.Exists(filename)) 
			{
				return new StreamReader(filename);
			}
			else 
			{
				filename = MakeFilePath.Project(sub_dir, ascii_file);
				if(File.Exists(filename)) 
				{
					return new StreamReader(filename, System.Text.Encoding.Default);
				}
			}

			return null;
		}
	}
    public class DownloadResponse
    {
        public ResultType Result { get; set; }
        public DateTime FileTime { get; set; }
        public long FileSize { get; set; }
        public byte[] Data { get; set; }
    }
}
