using System;
using AutoLibLocal;
using AutoLib;
using System.Windows.Forms;
using NetTools;

namespace DialogTag.TagEditor
{
	/// <summary>
	/// Summary description for Editor.
	/// </summary>
	public class Editor
	{
		public Editor()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public delegate void DelegateTagPropertyChanged(TagPublicClass tp);
		public static DelegateTagPropertyChanged procTagPropertyChanged = null;

		public static bool ByViewMain(TagPublicClass tp)
		{
            if (checkTagFileChanged.IsChanged())
            {
                if(Tools.IsLangKorean())
                    MessageBox.Show("외부에서 태그 파일이 변경 되었습니다.\n감시 프로그램 재시작 후 태그를 변경할 수 있습니다.", "태그 파일 변경");
                else
                    MessageBox.Show("The tag file is changed by outside.\nIf you edit the tag then restart Local Main.", "Tag file changed");

                return false;
            }

			DialogTag.TagEditor.FormTagProperty dialog = new DialogTag.TagEditor.FormTagProperty(null);

			int index;
			index = tp.tag.IndexOf('.');
			if(index == -1)	dialog.sGroupName = "";
			else			dialog.sGroupName = tp.tag.Substring(0, tp.tag.Length-(tp.name.Length+1));

			dialog.SetTag(tp);
			dialog.bEditTagName = false;	// 태그명은 바꿀 수 없다.

            //20241010 PSU 창위치 변경
            dialog.StartPosition = FormStartPosition.CenterParent;
			if(dialog.ShowDialog(TotalConfig.formMain) == DialogResult.OK) 
			{
				TagPublicClass save = (TagPublicClass)NetTools.Tools.CopyObject(tp);

				dialog.GetTag(tp);
				if(tp.enumTagType == EnumTagType.AI) 
				{
					TagAiClass ai = (TagAiClass)tp;
					ai.bTagChangeFlag = true;

					ai.nSubOutAnalog = new int[1];
					ai.nSubOutAnalogSP = new int[1];
					ai.nSubOutDigitalHiHi = new int[1];
					ai.nSubOutDigitalLoLo = new int[1];
				}
				else if(tp.enumTagType == EnumTagType.DI) 
				{
					TagDiClass di = (TagDiClass)tp;
					di.nSubOutDigital1 = new int[1];
					di.nSubOutDigital2 = new int[1];
					di.nSubOutDigitalOffTag = new int[1];
					di.nSubOutDigitalOnTag = new int[1];
				}
				else {}

				TagLib.bChangedByLocalMain = true;

				SharedViewMain.EventGoTagPropertyChanged(tp);
				LibComNetServer.CheckChangedMember(tp, save);	// 태그 멤버 변경 검사
				if(procTagPropertyChanged != null) 
				{
					procTagPropertyChanged(tp);
				}

                TagLib.bNeedFileSaveList = true;   // 파일저장 목록을 새로 만들어야 한다.

				return true;
			}

			return false;
		}

        public static CheckFileChanged checkTagFileChanged = new CheckFileChanged();

	}
}
