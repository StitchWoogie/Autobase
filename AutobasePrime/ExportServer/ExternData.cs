using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace ExportServer
{
	/// <summary>
	/// Summary description for ExternData.
	/// </summary>
	public class ExternData
	{
		public ExternData()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static ArrayList serverList = new ArrayList();
		public static ArrayList driverList = new ArrayList();
	}

	public class DriverItem
	{
		public string filename;
		public string name;
		public int version;
	}

	public class DriverList
	{
		public static void Load()
		{
			ExternData.driverList.Clear();
			
			string path = Application.StartupPath+"\\ExportDriver";

			if(!Directory.Exists(path))	return;
			DirectoryInfo di = new DirectoryInfo(path);

			foreach(FileInfo fi in di.GetFiles("*.dll")) 
			{
				Assembly asm;
				// You must supply a valid fully qualified assembly name here.            
				asm = Assembly.LoadFile(fi.FullName);
				Type[] Types = asm.GetTypes();

				foreach (Type oType in Types)
				{
					string s = oType.Name.ToString();
					if(s == "ClassExportGate2") 
					{
						DriverItem item = new DriverItem();
						item.filename = fi.FullName;

						ExportLib.ClassExportLib2 obj = (ExportLib.ClassExportLib2)asm.CreateInstance(oType.FullName);
						item.name = obj.ProtocolGetName();
						item.version = 2;
						ExternData.driverList.Add(item);
						break;
					}
				}

			}
		}

		public static ExportLib.ClassExportLib2 LoadDriver(string name)
		{
			DriverItem item;

			for(int i = 0; i < ExternData.driverList.Count; i++) 
			{
				item = (DriverItem)ExternData.driverList[i];
				if(name == item.name) 
				{
					Assembly asm;
					// You must supply a valid fully qualified assembly name here.            
					asm = Assembly.LoadFile(item.filename);

					Type[] Types = asm.GetTypes();

					foreach (Type oType in Types)
					{
						string s = oType.Name.ToString();
						if(s == "ClassExportGate2") 
						{
							object obj = asm.CreateInstance(oType.FullName);
							return (ExportLib.ClassExportLib2)obj;
						}
					}
				}
			}

			return null;
		}
	}
}
