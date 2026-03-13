using System;
using System.Collections;
using AutoLibLocal;
using System.IO;
using NetTools;
using AutoLib;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for SqlBindList.
	/// </summary>
	public class SqlBindList
	{
		public static ArrayList blockAll;

		static SqlBindList()
		{
			//
			// TODO: Add constructor logic here
			//
			blockAll = AutoBaseSqlBindListLoad();
		}

		static ArrayList AutoBaseSqlBindListLoad()
		{
			ArrayList block = new ArrayList();

			if(!ConfigVarTotal.bLocalFlag)	return block;	// 웹모드에서는 필요할때마다 하나씩 가져온다.

			ALL_BIND_STRUCT all = new ALL_BIND_STRUCT();

			string path = String.Format("{0}\\SQL\\BINDLIST", TotalConfig.sDirWorkProject);
			
			if(!Directory.Exists(path))	return block;
			DirectoryInfo di = new DirectoryInfo(path);

			foreach(FileInfo fi in di.GetFiles("*.*")) 
			{
				all = new ALL_BIND_STRUCT();
				BindListLoad(fi.FullName, all.block);
				all.name = fi.Name;
				block.Add(all);
			}

			return block;
		}

		public static void BindListLoad(string filename, ArrayList block)
		{
			SQL_BIND_LIST bind;
			CommaBlockString comma = new CommaBlockString() ;
			string buf;

			if(!File.Exists(filename))	return;

			TextReader reader;
			if(String.Compare(Path.GetExtension(filename), ".lstx", true) == 0) 
				reader = new StreamReader(filename);
			else
				reader = new StreamReader(filename, System.Text.Encoding.Default);

			if(reader == null)	return;
			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				bind = new SQL_BIND_LIST();

				comma.Set(buf);
				comma.GetString(ref bind.tag);
				comma.GetString(ref bind.field);
				comma.GetString(ref buf);
				//bind.data_type = BindListChangeStringToType(buf);
				comma.GetInt(ref bind.data_size);
				comma.GetChar(ref bind.bUseOnRead);
				comma.GetChar(ref bind.bUseOnWrite);

				TagLib.GetTagTypeAndPos(bind.tag, ref bind.tag_type, ref bind.tag_pos);

				if(bind.data_size < 2)	bind.data_size = 50;
		
				block.Add(bind);
			}
			reader.Close();
		}

		public static bool SeekListFile(ref ALL_BIND_STRUCT all, string list_file)
		{
			if(ConfigVarTotal.bLocalFlag)	// 지역감시 모드일때
			{
				int l;

				for(l = 0; l < SqlBindList.blockAll.Count; l++) 
				{
					all = (ALL_BIND_STRUCT)SqlBindList.blockAll[l];
					if(String.Compare(list_file, all.name, true) == 0) 
					{
						return true;
					}
				}

				return false;
			}
			else	// WebMode 일 때
			{
				int l;

				for(l = 0; l < SqlBindList.blockAll.Count; l++) 
				{
					all = (ALL_BIND_STRUCT)SqlBindList.blockAll[l];
					if(String.Compare(list_file, all.name, true) == 0) 
					{
						if(all.bFileExist == false)	return false;

						return true;
					}
				}		

				
				string path = MakeFilePath.Project("SQL\\BindList", list_file);

				all = new ALL_BIND_STRUCT();
				BindListLoad(path, all.block);
				all.name = list_file;
				blockAll.Add(all);	

				if(!File.Exists(path)) 
				{
					all.bFileExist = false;
	
					return false;
				}

				return true;
			}
		}
	}

	public class SQL_BIND_LIST 
	{
		public string  tag;
		public EnumTagType   tag_type;
		public int[] tag_pos = new int[1];
		public string  field;
		public int   data_type;
		public int   data_size;
		public sbyte  bUseOnRead;		// Read에 사용
		public sbyte  bUseOnWrite;		// Write에 사용.
		//public ArrayList blockValue = new ArrayList();
		
	}

	public class ALL_BIND_STRUCT 
	{
		public string name;
		public ArrayList block = new ArrayList();
		public bool	 bFileExist = true;	// 웹모드일때 사용한다. 파일이 없어도 리스트를 만들고 flag를 false로한다.
	}
}

