using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MdbLib
{
	/// <summary>
	/// Summary description for MdbTool.
	/// </summary>
	public class MdbTool
	{
        /*
		static MdbTool()
		{
			//
			// TODO: Add constructor logic here
			//
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MdbLib.db1.mdb");

            sampleMdb = new byte[stream.Length];
            stream.Read(sampleMdb, 0, (int)stream.Length);
		}

        static byte[] sampleMdb = null;
        // 파일을 미리 만들어 놓는 방법은 좋기는 하나 영문윈도우에서는 
        // selected collating sequence not supported by the operating system 오류를 발생시킨다.
        public static void MdbCreate(string filename)
        {
            if (File.Exists(filename)) return;	// already create

            BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create));
            writer.Write(sampleMdb);
            writer.Close();
        }*/

        public static void MdbCreate(string filename)
		{
			if(File.Exists(filename))	return;	// already create
		
			ADOX.CatalogClass catalog = new ADOX.CatalogClass();

			string dsn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", filename);
			catalog.Create(dsn);

            //catalog = null;
            //System.Threading.Thread.Sleep(1);

            /*
            if (catalog.ActiveConnection != null)
            {
                ADODB.Connection connection = catalog.ActiveConnection as ADODB.Connection;
                connection.Close();
            }
            catalog.ActiveConnection = null;
            catalog = null;*/

            //cat.ActiveConnection = null;
            //cat = null;
            //GC.Collect();
            //Marshal.ReleaseComObject(cat);
            //cat = null;
            //GC.Collect();
		}

	}
}
