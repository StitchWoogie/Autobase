using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NetTools.OldDefine;
using System.Drawing;
using NetTools;
using AutoLibLocal;
using System.Collections;
using System.Threading.Tasks;

namespace GraphicModule
{
    [Serializable]
    public class ObjectArgsWebBrowser
    {
        public string url;
        public bool styleNavigation = true;
        public bool styleScrollBar = true;
        public bool styleContextMenu = true;
    }

    [Serializable]
    public class ObjectWebBrowser : ObjectExpand
    {
        ObjectArgsWebBrowser objArgs;

        [NonSerialized]
        WebBrowser wndChild;

        [NonSerialized]
        Form formParent; // 20250312 PSU

        [NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        public ObjectArgsWebBrowser ObjectArgs
        {
            set
            {
                objArgs = value;
            }
            get
            {
                return objArgs;
            }
        }

        public ObjectWebBrowser(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsWebBrowser args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.WebBrowser;
			objArgs = args;

            formParent = form; //20250312 PSU

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				wndChild = new WebBrowser();
                
                wndChild.Url = new Uri(objArgs.url);
                wndChild.AllowNavigation = objArgs.styleNavigation;
                wndChild.ScrollBarsEnabled = objArgs.styleScrollBar;
                wndChild.IsWebBrowserContextMenuEnabled = objArgs.styleContextMenu;

				form.Controls.Add(wndChild);
	
				int x1=0, y1=0, x2=0, y2=0;
				GetViewZone(ref x1, ref y1, ref x2, ref y2);
                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);
				wndChild.Left = x1;
				wndChild.Top = y1;
				wndChild.Width = x2-x1;
				wndChild.Height = y2-y1;

				wndChild.Show();

				arrayClassList.Add(this);

                _ = base.SetToolTipOnChildWindow(wndChild);
                OnVisible(ExpandCalcVisible());
			}

            else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {

                UpdateControlState(wndChild,formParent);//20250312 PSU
            }
		}

		public override void Close()
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				arrayClassList.Remove(this);
			}
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				if(y1 > y2)	Tools.Temp(ref y1, ref y2);

				Font font = MakeFont();

				int cyChar = (int)font.GetHeight()+1;
				int cxChar = (int)(font.GetHeight()/2);

				RECT r = new RECT();

				DrawClass.PopBox2(g, x1, y1, x2, y2, Color.LightGray);

				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;
				//format.FormatFlags |= StringFormatFlags.NoWrap;
				Brush brush = Brushes.Black;

                r.left = x1;
                r.top = y1;
                r.right = x2;
                r.bottom = y2;
                DrawClass.DrawText(g, this.objArgs.url, font, brush, r, format);
			}
		}

		public override void OnMove(int x1, int y1, int x2, int y2)
		{
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(wndChild != null) 
				{
					Font font = MakeFont();
					wndChild.Font = font;

					wndChild.Left = x1;
					wndChild.Top = y1;
					wndChild.Width = x2-x1;
					wndChild.Height = y2-y1;
				}
			}
		}

		public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
            await Task.CompletedTask; // 경고 해결용

            if (command == "WebNavigate") 
			{
                wndChild.Navigate((string)args[1], (int)args[2] != 0);
				return 1;
			}
			else if(command == "WebGoBack") 
			{
                wndChild.GoBack();
				return 1;
			}
            else if (command == "WebGoForward")
            {
                wndChild.GoForward();
                return 1;
            }
            else if (command == "WebGoHome")
            {
                wndChild.GoHome();
                return 1;
            }
            else if (command == "WebGoSearch")
            {
                wndChild.GoSearch();
                return 1;
            }
            else if (command == "WebRefresh")
            {
                wndChild.Refresh();
                return 1;
            }

			return 0;
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			ObjectSaveFont(writer);

			writer.Write("\tStringOption,");
			writer.Write("{0},", objArgs.url);

            UInt32 style = 0;
            if (objArgs.styleContextMenu) style |= Tools.DWORD_MASK[0];
            if (objArgs.styleNavigation) style |= Tools.DWORD_MASK[1];
            if (objArgs.styleScrollBar) style |= Tools.DWORD_MASK[2];
            writer.Write("{0:X08},", style);
			writer.WriteLine();
		}

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                this.wndChild.Visible = flag;
            }

        }
    }
}
