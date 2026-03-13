using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetTools
{
    public class CheckFileChanged
    {
        /*
        class WatcherFileClass
        {
            public string filename;
            public DateTime t;
        }

        // SystemFileWatcher이 이벤트가 두번 들어와서 같은 날짜면 처리하지 않는다.
        List<WatcherFileClass> arrayWatcherFile = new List<WatcherFileClass>();

        public bool IsFileChanged(string filename)
        {
            DateTime t = File.GetLastWriteTime(filename);

            for (int i = 0; i < arrayWatcherFile.Count; i++)
            {
                if (String.Compare(arrayWatcherFile[i].filename, filename, true) == 0)
                {
                    if (arrayWatcherFile[i].t != t)
                    {
                        arrayWatcherFile[i].t = t;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            WatcherFileClass item = new WatcherFileClass();

            item.filename = filename;
            item.t = t;

            arrayWatcherFile.Add(item);

            return true;
        }*/

        public bool IsChanged()
        {
            if (sFileName == null) return false;

            DateTime t= File.GetLastWriteTime(sFileName);

            return (tFile != t);
        }

        public void Reset()
        {
            if (sFileName == null) return;

            tFile = File.GetLastWriteTime(sFileName);
        }

        string sFileName;
        DateTime tFile;

        public void Register(string filename)
        {
            sFileName = filename;
            Reset();
        }
    }
}


