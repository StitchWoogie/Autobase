using System;
using System.Collections;
using NetTools;
using System.IO;
using AutoLibLocal;
using System.Windows.Forms;

namespace AutoLib
{
	/// <summary>
	/// Summary description for TerminalClass.
	/// </summary>
	public class TerminalClass
	{
		public TerminalClass()
		{
			
		}

		public static void Init()
		{
            TerminalClass terminalStruct = new TerminalClass();

            TagLib.groupRoot = new TagGrClass();    // 태그를 다시 읽을 경우 초기화가 필요하다.

            terminalStruct.TagLoadAll();
            TagLib.InitByTerminalClass();
		}

		bool TagLoadNewFormat()
		{
			string filename = MakeFilePath.Tag("Local.tagx");

			if(!File.Exists(filename))	return false;

			TagFile tagfile = new TagFile();

			if(tagfile.LoadTag(filename, TagLib.groupRoot, System.Text.Encoding.UTF8, false) == false)	return true;	// 파일은 존재하나 읽을 수 없을 때는 구버전을 읽지 않는다.

			return true;	
		}

		public void TagLoadAll()
		{
			if(!TagLoadNewFormat()) // 파일이 존재하지 않을때만 구버전을 읽는다.
			{
				TagFileOld load = new TagFileOld();
				string path;

				path = MakeFilePath.Tag("AI.TAG");
				load.TagLoadAI(path);
				path = MakeFilePath.Tag("AO.TAG");
				load.TagLoadAO(path);
				path = MakeFilePath.Tag("DI.TAG");
				load.TagLoadDI(path);
				path = MakeFilePath.Tag("DO.TAG");
				load.TagLoadDO(path);
				path = MakeFilePath.Tag("ST.TAG");
				load.TagLoadST(path);
				path = MakeFilePath.Tag("do-group.TAG");
				load.TagLoadDoGroup(path);
			}

			// 태그가 없을때는 프로그램의 원할한 구조를 위해 기본 태그를 만든다.
			if(TagLib.GetTagListCount() == 0) // TagLib.groupRoot.arrayTag.Count == 0 을 사용하면 비어있는 그룹 태그만 있을 때 프로그램이 전체적으로 다운된다.
			{
				TagLib.MakeDefaultTag();
            }
		}

		public static bool SaveTag(TagGrClass tagTemp)
		{
			string filename = String.Format("{0}\\TAG", TotalConfig.sDirWorkProject);

            try
            {
                Directory.CreateDirectory(filename);
            }
            catch
            {
                return false;
            }

			filename = String.Format("{0}\\TAG\\Local.tagx", TotalConfig.sDirWorkProject);

			TagFile tagfile = new TagFile();

			BackUp.BackUpFile(filename);

			bool retn = tagfile.SaveTag(filename, tagTemp, System.Text.Encoding.UTF8, false, false);
			
			if(retn) 
			{	
				TagLib.bChangedByLocalMain = false;
			}

			return retn;
		}

	}
}
