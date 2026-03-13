using System;
using System.IO;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for BackUp.
	/// </summary>
	public class BackUp
	{
		public BackUp()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static string GetAutoBackupFolder()
        {
            string folder_default = TotalConfig.MakePathByWindowsDisk(TotalConfig.sOemRootFolder + "\\Back files");

            string folder = TotalConfig.LoadRegAutoBaseConfig("Config", "Folders", "AutoBackupFolder", folder_default);

            return folder;
        }

        public static void SetAutoBackupFolder(string folder)
        {
            TotalConfig.SaveRegAutoBaseConfig("Config", "Folders", "AutoBackupFolder", folder);
        }

		static string GetBackupName(string filename)
		{
			int root_length = Path.GetPathRoot(filename).Length;

			//return String.Format("{0}\\{1}", TotalConfig.MakePathByWindowsDisk(TotalConfig.sOemRootFolder+"\\Back files"), filename.Substring(root_length));
            return String.Format("{0}\\{1}", GetAutoBackupFolder(), filename.Substring(root_length));
		}

		public static string GetBackupDirectory(string filename)
		{
			return Path.GetDirectoryName(GetBackupName(filename));
		}

		public static void BackUpFile(string filename)
		{
			if(!File.Exists(filename))	return;	// 새이름으로 저장할 때는 원본 파일이 없을때도 있다.

			string target = GetBackupName(filename);

			string path = Path.GetDirectoryName(target);
			if(!Directory.Exists(path))	Directory.CreateDirectory(path);

			if(File.Exists(target)) 
			{
				File.SetAttributes(target, FileAttributes.Archive);
			}
			File.Copy(filename, target, true);
		}
	}
}
