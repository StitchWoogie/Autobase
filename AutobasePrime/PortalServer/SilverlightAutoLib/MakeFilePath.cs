using System;
using System.IO;
using System.Net;
using AutoLibLocal;
//using SilverlightAutoLib.ServiceReference1;

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

        /*
		static void DownLoadSystemFile(string dir, string filename, bool compare_flag, DateTime file_time, long file_size, string target)
		{
            ServiceDownLoadProject service = new ServiceDownLoadProject();
			service.Timeout = 4000;

			byte[] array = null;
			ResultType result = 0;

			try 
			{
				array = service.DownLoadWithData(dir, filename, compare_flag, ref file_time, file_size, ref result);
			}
			catch (WebException)
			{
				result = ResultType.failed;
			}

			if(result == ResultType.failed) // 어떤 이유로 성공못함.
			{
				service.Dispose();
				return;	
			}
			else if(result == ResultType.matched)	// 파일이 같음
			{
				FileAttributes attr = File.GetAttributes(target); 
				File.SetAttributes(target, attr & ~FileAttributes.Archive);
				service.Dispose();
				return;
			}
			else if(result == ResultType.notfound)	// 파일이 없음
			{
				if(File.Exists(target)) 
				{
					File.Delete(target);
				}
			}
			else 
			{
				string path;

		
				path = Path.GetDirectoryName(target); // file명이 ex2\ex.mod 처럼 sub폴더가 포함된 경우가 있으므로 파일명에서 폴더명을 얻는것이 정확하다.

				if(!Directory.Exists(path))
					Directory.CreateDirectory(path);

				FileStream fs = File.Open(target, FileMode.Create);//File.OpenWrite(target);
				fs.Flush();
				if(array != null)
					fs.Write(array, 0, array.Length);
				fs.Close();

				File.SetLastWriteTime(target, file_time);

				FileAttributes attr = File.GetAttributes(target);
				File.SetAttributes(target, attr & ~FileAttributes.Archive);
			}

			service.Dispose();
		}*/

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
				path = String.Format("{0}\\{1}\\{2}", "Test", dir, filename);
			}

            //if(ConfigVarTotal.bSSL)
            //    path = String.Format("https://{0}/AutoWeb/Project/{1}/{2}", ConfigVarTotal.sSiteRootName, dir, filename);
            //else
                path = String.Format("http://{0}/AutoWeb/Project/{1}/{2}", ConfigVarTotal.sSiteRootName, dir, filename);
            
            //path = String.Format("{0}/{1}/{2}", "http://localhost:62891/AutobaseWeb/AutoWeb/Project", dir, filename);

            return path;
		}

        public static string SilverlightGraphicModule(string filename)
        {
            string path;

            //if (ConfigVarTotal.bSSL)
            //    path = String.Format("https://{0}/AutoWeb/Silverlight/GraphicModulePage.aspx?filename={1}", ConfigVarTotal.sSiteRootName, filename);
            //else
                path = String.Format("http://{0}/AutoWeb/Silverlight/GraphicModulePage.aspx?filename={1}", ConfigVarTotal.sSiteRootName, filename);

            return path;
        }


		public static string Graphic(string filename)
		{
			return Project("graphic", filename);
		}

        /*
		public static string Graphic_ObjScr(string filename)
		{
			return Project("graphic\\objscr", filename);
		}*/

		public static string Tag(string filename)
		{
			return Project("TAG", filename);
		}

		public static string Users(string filename)
		{
			return Project("Users", filename+".user");
		}

		public static string Sound(string filename)
		{
			return Project("sound", filename);
		}

        public static string MakePublishTextPath(string filename)
        {
            NetTools.UriSplit split = new NetTools.UriSplit();
            split.Split(filename);

            string dir = split.GetDirectoryName();
            string file = split.GetFileName();
            string ext = split.GetExtension();

            //file = System.Windows.Browser.HttpUtility.UrlEncode(file);
            file = System.Uri.EscapeDataString(file);

            string target = String.Format("{0}/{1}.txt", dir, file);

            return target;
        }

        /*
		static void RecurseResetFlags(string root_path)
		{
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
		}*/
	}

    /*
    class DownLoadClass
    {
        bool bDownLoadEnd = false;
        string sTarget;

        public void DownLoadSystemFile(string dir, string filename, bool compare_flag, DateTime file_time, long file_size, string target)
        {
            bDownLoadEnd = false;

            ServiceDownLoadProjectSoapClient service = new ServiceDownLoadProjectSoapClient();
            service.DownLoadWithDataCompleted += new EventHandler<DownLoadWithDataCompletedEventArgs>(service_DownLoadWithDataCompleted);

            ResultType result = 0;
            sTarget = target;

            service.DownLoadWithDataAsync(dir, filename, compare_flag, file_time, file_size, result);

            while (true)
            {
                if (bDownLoadEnd) break;
                System.Threading.Thread.Sleep(1);
            }
        }

        void service_DownLoadWithDataCompleted(object sender, DownLoadWithDataCompletedEventArgs e)
        {
            //throw new NotImplementedException();

            ResultType result = e.result1;
            DateTime file_time = e.file_time;
            byte[] array = (byte[])e.Result;

            if (result == ResultType.failed) // 어떤 이유로 성공못함.
            {
                return;
            }
            else if (result == ResultType.matched)	// 파일이 같음
            {
                
                //FileAttributes attr = File.GetAttributes(target);
                //File.SetAttributes(target, attr & ~FileAttributes.Archive);
                
                return;
            }
            else if (result == ResultType.notfound)	// 파일이 없음
            {
                if (File.Exists(sTarget))
                {
                    File.Delete(sTarget);
                }
            }
            else
            {
                string path;

                path = Path.GetDirectoryName(sTarget); // file명이 ex2\ex.mod 처럼 sub폴더가 포함된 경우가 있으므로 파일명에서 폴더명을 얻는것이 정확하다.

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                FileStream fs = File.Open(sTarget, FileMode.Create);//File.OpenWrite(target);
                fs.Flush();
                if (array != null)
                    fs.Write(array, 0, array.Length);
                fs.Close();

                //File.SetLastWriteTime(sTarget, file_time);

                //FileAttributes attr = File.GetAttributes(sTarget);
                //File.SetAttributes(sTarget, attr & ~FileAttributes.Archive);
            }

            bDownLoadEnd = true;
        }
    }*/
}
