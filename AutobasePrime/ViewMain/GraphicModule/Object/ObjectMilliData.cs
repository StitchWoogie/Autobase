using System;
using AutoLibLocal;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using AutoLib;
using NetTools.OldDefine;
using NetTools;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphicModule
{

	[Serializable]
	public class ObjectArgsMilliData
	{
		public string sTitle;
	}

	/// <summary>
	/// Summary description for ObjectDatabase.
	/// </summary>
	
	[Serializable]
	public class ObjectMilliData : ObjectExpand
	{
		ObjectArgsMilliData objArgs;

		[NonSerialized]
		MilliDataWnd wndChild;

        [NonSerialized]
        Form formParent; // 20250312 PSU

		[NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        public ObjectArgsMilliData ObjectArgs 
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

		public ObjectMilliData(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsMilliData args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.MilliDataWindow;
            bSupportObjectOnCE = false;
			objArgs = args;

            formParent = form; //20250312 PSU

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				wndChild = new MilliDataWnd();
				string last_file;

				if(GetLastFile(objArgs.sTitle, out last_file)) 
				{
                  //  if(ConfigVarTotal.bLocalFlag)
					   // wndChild.SetFileNameLocal(last_file);
                    //else
                    //    wndChild.SetFileNameWeb(objArgs.sTitle, last_file);
				}

				wndChild.TopLevel = false;
				wndChild.FormBorderStyle = FormBorderStyle.None;
				
				wndChild.Font = MakeFont();

                wndChild.Show();            // form.Coltrols.Add 후에 Show하면 초기 크기가 맞지 않는다. 2009.2.3
                form.Controls.Add(wndChild);    
	
				int x1=0, y1=0, x2=0, y2=0;
				GetViewZone(ref x1, ref y1, ref x2, ref y2);
				wndChild.Left = x1;
				wndChild.Top = y1;
				wndChild.Width = x2-x1;
				wndChild.Height = y2-y1;

				arrayClassList.Add(this);

				_ = base.SetToolTipOnChildWindow(wndChild);
                OnVisible(ExpandCalcVisible());
			}
		}

		static bool GetLastFile(string title, out string fullname)
		{
			fullname = "";
			if(title.Length == 0)	return false;

            if (!ConfigVarTotal.bLocalFlag)
            {
                // 미세자료는 10.3.2.6 부터 지원한다.  미세자료 트랜드는 10.3.2.5 부터 지원
                if (!ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 3, 2, 6)) 
                    return false;
                
                ServiceLibDataGate sldg = new ServiceLibDataGate();

                int retn = sldg.Command("MilliDataGetFileLists", title);

                if (retn == -1)
                {
                    MessageBox.Show(sldg.sErrorMessage);
                    return false;
                }

                List<string> array = sldg.GetResultListString(0);

                fullname = "";

                for (int i = 0; i < array.Count; i++)
                {
                    if (fullname.Length == 0)
                    {
                        fullname = array[i];
                    }
                    else
                    {
                        if (String.Compare(array[i], fullname, true) > 0)
                        {
                            fullname = array[i];
                        }
                    }
                }

                if (fullname.Length == 0) return false;

                return true;
            }
            else
            {
                string data_dir;
                data_dir = TotalConfig.GetProjectDataDirectory();

                string path;
                string filename;

                filename = "";

                path = String.Format("{0}\\MiliData\\{1}", data_dir, title);

                if (!Directory.Exists(path)) return false;

                DirectoryInfo info = new DirectoryInfo(path);

                foreach (FileInfo fi in info.GetFiles("*.mdb"))
                {
                    if (filename.Length == 0)
                    {
                        filename = fi.Name;
                        fullname = fi.FullName;
                    }
                    else
                    {
                        if (String.Compare(fi.Name, filename, true) > 0)
                        {
                            filename = fi.Name;
                            fullname = fi.FullName;
                        }
                    }
                }

                if (filename.Length == 0) return false;

                return true;
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

				//int cyChar = (int)font.GetHeight()+1;
				//int cxChar = (int)font.SizeInPoints;

				RECT r = new RECT();

				DrawClass.PopBox2(g, x1, y1, x2, y2, Color.LightGray);
				DrawClass.PushRectangle2(g, x1+3, y1+3, x2-3, y2-3);

				r.left = x1;
				r.top = y1;
				r.right = x2;
				r.bottom = y2;

				string str;

				str = objGeneral.sClassName;

				DrawClass.DrawText(g, str, font, Brushes.Black, r, DrawClass.StringFormatCenter);
			}

            else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {

                UpdateControlState(wndChild,formParent);//20250312 PSU
            }
		}


		public override void OnMove(int x1, int y1, int x2, int y2)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(wndChild != null) 
				{
                    // 컨트롤 업데이트 일시 중지
                    wndChild.SuspendLayout();

					Font font = MakeFont();
					wndChild.SetFont(font);

					wndChild.Left = x1;
					wndChild.Top = y1;
					wndChild.Width = x2-x1;
					wndChild.Height = y2-y1;

                    // 컨트롤 업데이트 재개
                    wndChild.ResumeLayout(false);
				}
			}
		}

		public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
			try
			{
				if (command == "MilliDataSetFile")
				{
					//if(!File.Exists((string)args[1]))	return 0;
					if (Tools.IsLangKorean())
					{
						MessageDisplay.Show("@MilliDataSetFile 지원하지 않는 함수입니다.");
					}
					else
					{
                        MessageDisplay.Show("@MilliDataSetFile function is not supported.");
                    }
					return 0;
				}
				else if (command == "MilliDataSetTimeType")
				{
					wndChild.SetTimeType((int)args[1]);
					return 1;
				}
				else if (command == "MilliDataSetTimeRange")
				{
					int startYear = (int)args[1];
					int startMon = (int)args[2];
					int startDay = (int)args[3];
					int startHour = (int)args[4];
					int startMin = (int)args[5];
					int startSec = (int)args[6];
					int endYear = (int)args[7];
					int endMon = (int)args[8];
					int endDay = (int)args[9];
					int endHour = (int)args[10];
					int endMin = (int)args[11];
					int endSec = (int)args[12];

					wndChild.SetTimeRange(startYear, startMon, startDay, startHour, startMin, startSec,
										 endYear, endMon, endDay, endHour, endMin, endSec);
                    return 1;
                }
				else if (command == "MilliDataSetTableName")
				{
					wndChild.SetTableName((string)args[1]);
					await wndChild.LoadDataAsync();
                    return 1;
                }
				else
				{
					return 0;
				}
			}
			catch(Exception ex)
			{
				MessageDisplay.Show(ex.Message, "Error");
                return 0;
            }
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			ObjectSaveFont(writer);
	
			writer.Write("\tStringOption,");
			writer.Write("{0},", objArgs.sTitle);
			writer.WriteLine();
        }

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                this.wndChild.Visible = flag;
            }

        }

        public override void UpdateZone(Form form, int x1, int y1, int x2, int y2)
        {
            if (TotalConfig.eOemType == EnumOemType.UYeG_GS || TotalConfig.eOemType == EnumOemType.UYeG_Normal)
            {
                // 텍스트가 겹쳐서 300*500 이하는 되지 않도록 수정

                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                if((x2-x1) < 299) {
                    x2 = x1 + 299;
                }
                if ((y2 - y1) < 499)
                {
                    y2 = y1 + 499;
                }
                base.UpdateZone(form, x1, y1, x2, y2);
            }
            else
            {
                base.UpdateZone(form, x1, y1, x2, y2);
            }
        }

    }
}

/*
 * 
 * 
 * 
void ObjectDatabase :: OnMove(int x1, int y1, int x2, int y2)
{
#if	defined (MODE_RUN)
	wndChild.MoveWindow(x1, y1, x2-x1, y2-y1, TRUE);

	if(hFontCtrl)	DeleteObject(hFontCtrl);
	hFontCtrl = MakeFont();
	wndChild.ChangeFont(hFontCtrl);
#endif
}







 

*/ 