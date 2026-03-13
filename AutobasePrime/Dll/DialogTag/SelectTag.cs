using System;
using AutoLib;
using AutoLibLocal;
using System.Windows.Forms;

namespace DialogTag
{
	/// <summary>
	/// Summary description for SelectTag.
	/// </summary>
	public class SelectTag
	{
		public string sTag;
		public string sDes;
		public EnumTagType	tagType = EnumTagType.none;

		public bool bUseTagAI = false;
		public bool bUseTagAO = false;
		public bool bUseTagDI = false;
		public bool bUseTagDO = false;
		public bool bUseTagST = false;
		public bool bUseTagGDO = false;

		public SelectTag()
		{
			//
			// TODO: Add constructor logic here
			//
			if(!TagLib.bInit) 
			{
				TerminalClass.Init();
			}
		}

        //public DialogResult Run()
        //{
        //    FormSelectTag dialog = new FormSelectTag();

        //    dialog.bUseTagAI = bUseTagAI;
        //    dialog.bUseTagAO = bUseTagAO;
        //    dialog.bUseTagDI = bUseTagDI;
        //    dialog.bUseTagDO = bUseTagDO;
        //    dialog.bUseTagST = bUseTagST;
        //    dialog.bUseTagGDO = bUseTagGDO;

        //    DialogResult result = dialog.ShowDialog();

        //    sTag = dialog.sTag;
        //    sDes = dialog.sDes;
        //    tagType = dialog.tagType;		// add by kdy 2004-08-19

        //    return result;
        //}


        //20241010 PSU IWin32Window owner 추가, Form 또는 Control을 매개변수로 받음.
        public DialogResult Run()
        {
            return Run(null);
        }

        public DialogResult Run(IWin32Window owner)
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagAI = bUseTagAI;
            dialog.bUseTagAO = bUseTagAO;
            dialog.bUseTagDI = bUseTagDI;
            dialog.bUseTagDO = bUseTagDO;
            dialog.bUseTagST = bUseTagST;
            dialog.bUseTagGDO = bUseTagGDO;

            DialogResult result;
            if (owner != null)
            {
                result = dialog.ShowDialog(owner);
            }
            else
            {
                result = dialog.ShowDialog();
            }

            sTag = dialog.sTag;
            sDes = dialog.sDes;
            tagType = dialog.tagType;  // add by kdy 2004-08-19

            return result;
        }

		public static DialogResult SelectDi(out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagDI = true;

			DialogResult result = dialog.ShowDialog();

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

		public static DialogResult SelectDi(Form form, out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagDI = true;

			DialogResult result = dialog.ShowDialog(form);

			tag = dialog.sTag;
			des = dialog.sDes;			

			return result;
		}

		public static DialogResult SelectDiDo(out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagDI = true;
			dialog.bUseTagDO = true;

			DialogResult result = dialog.ShowDialog();

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

        //20241010 PSU Form owner 추가
        public static DialogResult SelectDiDo(Form owner, out string tag, out string des)
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagDI = true;
            dialog.bUseTagDO = true;

            DialogResult result = dialog.ShowDialog(owner);

            tag = dialog.sTag;
            des = dialog.sDes;

            return result;
        }

		public static DialogResult SelectAi(out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagAI = true;

			DialogResult result = dialog.ShowDialog();

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

		public static DialogResult SelectAi(Form owner, out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagAI = true;
            dialog.StartPosition = FormStartPosition.CenterParent;

			DialogResult result = dialog.ShowDialog(owner);

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

		public static DialogResult SelectAll(out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagAI = true;
			dialog.bUseTagAO = true;
			dialog.bUseTagDI = true;
			dialog.bUseTagDO = true;
			dialog.bUseTagST = true;
			dialog.bUseTagGDO = true;

			DialogResult result = dialog.ShowDialog();

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

        //20241010 PSU Form owner 추가.
        public static DialogResult SelectAll(Form owner, out string tag, out string des)
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagAI = true;
            dialog.bUseTagAO = true;
            dialog.bUseTagDI = true;
            dialog.bUseTagDO = true;
            dialog.bUseTagST = true;
            dialog.bUseTagGDO = true;
            //dialog.StartPosition = FormStartPosition.CenterParent;

            DialogResult result = dialog.ShowDialog(owner);

            tag = dialog.sTag;
            des = dialog.sDes;

            return result;
        }


		public static DialogResult SelectAiDi(out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagAI = true;
			dialog.bUseTagDI = true;

			DialogResult result = dialog.ShowDialog();

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

        public static DialogResult SelectAiDi(Form owner, out string tag, out string des)
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagAI = true;
            dialog.bUseTagDI = true;

            DialogResult result = dialog.ShowDialog(owner);

            tag = dialog.sTag;
            des = dialog.sDes;

            return result;
        }

        public static DialogResult SelectAiDiSt(out string tag, out string des)
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagAI = true;
            dialog.bUseTagDI = true;
            dialog.bUseTagST = true;

            DialogResult result = dialog.ShowDialog();

            tag = dialog.sTag;
            des = dialog.sDes;

            return result;
        }

        
        public static DialogResult SelectAiDiSt(Form owner, out string tag, out string des)
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagAI = true;
            dialog.bUseTagDI = true;
            dialog.bUseTagST = true;

            DialogResult result = dialog.ShowDialog(owner);

            tag = dialog.sTag;
            des = dialog.sDes;

            return result;
        }

		public static DialogResult SelectDo(out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagDO = true;

			DialogResult result = dialog.ShowDialog();

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

		public static DialogResult SelectDo(Form owner, out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagDO = true;

			DialogResult result = dialog.ShowDialog(owner);

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

		public static DialogResult SelectDoGdo(Form owner, out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagDO = true;
			dialog.bUseTagGDO = true;

			DialogResult result = dialog.ShowDialog(owner);

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

		public static DialogResult SelectAo(out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagAO = true;

			DialogResult result = dialog.ShowDialog();

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

		public static DialogResult SelectAo(Form owner, out string tag, out string des)
		{
			FormSelectTag dialog = new FormSelectTag();

			dialog.bUseTagAO = true;

			DialogResult result = dialog.ShowDialog(owner);

			tag = dialog.sTag;
			des = dialog.sDes;
			
			return result;
		}

        public static DialogResult SelectSt(out string tag, out string des)
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagST = true;

            DialogResult result = dialog.ShowDialog();

            tag = dialog.sTag;
            des = dialog.sDes;

            return result;
        }

        //20241010 PSU Form owner 추가
        public static DialogResult SelectSt(Form owner, out string tag, out string des)
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagST = true;

            DialogResult result = dialog.ShowDialog(owner);

            tag = dialog.sTag;
            des = dialog.sDes;

            return result;
        }
	
	}
}

