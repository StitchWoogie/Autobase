using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetTools
{
    public class FileTool
    {
        public static void FileDeleteSafety(string filename)
        {
            if (!File.Exists(filename)) return;
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(filename, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }

        public static void FolderDeleteSafety(string dirname)
        {
            if (!Directory.Exists(dirname)) return;
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(dirname, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }
    }
}
