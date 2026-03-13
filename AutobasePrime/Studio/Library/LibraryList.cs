using System;
using System.Collections;
using GraphicModule;
using System.IO;
using AutoLibLocal;
using NetTools;

namespace Studio
{
	public class ListLibraryGroup
	{
		public string root;
		public string name;
		public ArrayList block = new ArrayList();
	}
    
	/// <summary>
	/// Summary description for LibraryList.
	/// </summary>
	public class LibraryList
	{
		public LibraryList()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		protected ArrayList tempItem;
		protected string tempName;
		protected int tempNo = 0;
		protected string tempRoot;
		protected string tempDir;

		protected virtual void Call(ObjectGroup group) {}

		protected void LoadItems(System.Windows.Forms.Form form, string root, string directory, ArrayList array)
		{
            DirectoryInfo info = new DirectoryInfo(directory);
            byte[] buffer;

            foreach (FileInfo fi in info.GetFiles("*.*"))
            {
                buffer = File.ReadAllBytes(fi.FullName);
                WebLibraryUtil.HashBuffer(buffer);

                if (buffer == null) return;

                MemoryStream stream = new MemoryStream(buffer);
                if (stream == null) return;

                ObjectCommonProperty ocp = new ObjectCommonProperty();
                ocp.bLoadByZipStream = true;
                ocp.streamZip = stream;

                MemoryStream smod = ObjectAnimation.RestoreFromZipStream(stream, "group.modx");
                if (smod == null)
                {
                    stream.Close();
                    return;
                }

                ObjectGroup group = new ObjectGroup(null, null, null, null, null);

                if (!group.LoadModX(ocp, new System.Windows.Forms.Form(), smod)) continue;

                smod.Close();

                stream.Close();

                tempItem = array;
                tempName = fi.Name;
                tempRoot = root;
                tempDir = fi.FullName;
                tempNo = 0;

                Call(group);
            }

            /*
			DirectoryInfo info = new DirectoryInfo(directory);
			string filename;
			foreach(DirectoryInfo di in info.GetDirectories("*.*")) 
			{
				filename = String.Format("{0}\\Group.modx", di.FullName);
				if(!File.Exists(filename)) 
				{
					filename = String.Format("{0}\\Group.mod", di.FullName);
				}
				if(!File.Exists(filename))	continue;

				ObjectCommonProperty ocp = new ObjectCommonProperty();
				ocp.sModuleName = filename;
				ObjectGroup group = new ObjectGroup(null,null,null,null,null);
				if(!group.Load(ocp, form, filename))	continue;

				tempItem = array;
				tempName = di.Name;
				tempRoot = root;
				tempDir = di.FullName;
				tempNo = 0;

				Call(group);
			}*/
		}

		void LoadOneThema(System.Windows.Forms.Form form, string thema_name, string root, string directory)
		{
			DirectoryInfo info = new DirectoryInfo(directory);
			foreach(DirectoryInfo di in info.GetDirectories("*.*")) 
			{
				ListLibraryGroup group = new ListLibraryGroup();
				group.name = thema_name+"."+di.Name;
				LoadItems(form, root, di.FullName, group.block);
				if(group.block.Count > 0) 
				{
					group.root = root;
					arrayLib.Add(group);
				}
			}
		}

		protected void MakeListAll(System.Windows.Forms.Form form, string filename)
		{
			string lib_directory;

            lib_directory = TotalConfig.GetLibraryFolderBasic();
            MakeList(form, filename, lib_directory);
            lib_directory = TotalConfig.GetLibraryFolderUser();
            MakeList(form, filename, lib_directory);
		}

		protected virtual void WriteOneLine(TextWriter writer, object obj) {}

		void MakeList(System.Windows.Forms.Form form, string file, string root)
		{
			string directory = root+"\\ModCut";

			this.arrayLib.Clear();

			if(!Directory.Exists(directory))	return;
			DirectoryInfo info = new DirectoryInfo(directory);
			foreach(DirectoryInfo di in info.GetDirectories("*.*")) 
			{
				LoadOneThema(form, di.Name, root, di.FullName);
			}

			string filename = String.Format("{0}\\{1}", root, file);
			TextWriter writer = new StreamWriter(filename);
			if(writer == null)	return;

			ListLibraryGroup group;

			for(int i = 0; i < this.arrayLib.Count; i++) 
			{
				group = (ListLibraryGroup)arrayLib[i];

				writer.WriteLine("Group,{0},", group.name);

				for(int j = 0; j < group.block.Count; j++) 
				{
					WriteOneLine(writer, group.block[j]);
				}
			}	
			
			writer.Close();
		}

		protected virtual object ReadOneLine(CommaBlockString comma) { return null;}

		void LoadList(string file, string root)
		{
			string filename = String.Format("{0}\\{1}", root, file);
			if(!File.Exists(filename))	return;
			TextReader reader = new StreamReader(filename);
			if(reader == null)	return;

			ListLibraryGroup group = null;
			object item = null;
			CommaBlockString comma = new CommaBlockString();
			string buf;
			string command = "";

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				comma.Set(buf);
				comma.GetString(ref command);
				if(command == "Group") 
				{
					group = new ListLibraryGroup();
					this.arrayLib.Add(group);
					comma.GetString(ref group.name);
					group.root = root;
				}
				else if(command == "Item") 
				{
					item = ReadOneLine(comma);
					if(group != null)
						group.block.Add(item);
				}
				else {}
			}
			
			reader.Close();
		}

		protected ArrayList arrayLib = new ArrayList();

		protected void LoadListAll(string filename, ArrayList array)
		{
			arrayLib = array;
			this.arrayLib.Clear();

			string directory;

            directory = TotalConfig.GetLibraryFolderBasic();
			LoadList(filename, directory);
            directory = TotalConfig.GetLibraryFolderUser();
			LoadList(filename, directory);
		}
	}
}
