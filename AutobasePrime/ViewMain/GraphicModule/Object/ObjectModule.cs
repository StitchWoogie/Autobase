using System;
using AutoLib;
using NetTools.OldDefine;
using System.Windows.Forms;
using System.Drawing;
using AutoLibLocal;
using NetTools;
using System.IO;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsModule
	{
		public string filename;
	}
	/// <summary>
	/// Summary description for ObjectModule.
	/// </summary>
	[Serializable]
	public class ObjectModule : ObjectExpand
	{
		ObjectArgsModule objArgs;

		[NonSerialized]
		FormGraphicChild formModule;

		bool bModuleSameFlag = false;

		public ObjectArgsModule ObjectArgs 
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

		public ObjectModule(ObjectCommonProperty ocp, System.Windows.Forms.Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, ObjectArgsModule args)
			: base(ocp, rect, eid, null, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.Module;
			objArgs = args;

			
			string filename = MakeFilePath.Graphic(args.filename);

			if(String.Compare(filename, ocp.sModuleName, true) == 0) 
			{
				bModuleSameFlag = true;
			}
			else 
			{
				bModuleSameFlag = false;
			}

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(bModuleSameFlag == false)
				{
					//MakeFilePath make = new MakeFilePath();
					//string filename = make.Graphic(args.filename);

					formModule = new FormGraphicChild(filename);
					//
					formModule.FormBorderStyle = FormBorderStyle.None;
					formModule.TopLevel = false;
					
					formModule.WindowState = FormWindowState.Normal;
					formModule.Size = new Size(0, 0);

					form.Controls.Add(formModule);
				
					formModule.Show();

					int x1=0, y1=0, x2=0, y2=0;
					GetViewZone(ref x1, ref y1, ref x2, ref y2);
					formModule.Left = x1;
					formModule.Top = y1;
					formModule.Width = x2-x1;
					formModule.Height = y2-y1;

					formModule.ChangeModuleOpticMethod(2);
				}
			}
		}

		public override void Close()
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN && formModule != null) 
			{
				formModule.Close();
				formModule = null;
			}
		}

		public override void OnMove(int x1, int y1, int x2, int y2)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(formModule != null) 
				{
					formModule.Left = x1;
					formModule.Top = y1;
					formModule.Width = x2-x1;
					formModule.Height = y2-y1;
				}
			}
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			int width = x2-x1+1;
			int height = y2-y1+1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
				DrawClass.gcls(g, x1, y1, x2, y2, Color.LightGray);

				Rectangle r = new Rectangle(x1, y1, width, height);

				Font font = new Font("Gulim", 10);
				
				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;

                SafeException.SafeDrawString(g, objArgs.filename, font, Brushes.Black, r, format);
			}
			else 
			{
				if(bModuleSameFlag) {

					DrawClass.gcls(g, x1, y1, x2, y2, Color.LightGray);

					Rectangle r = new Rectangle(x1, y1, width, height);

					Font font = new Font("Gulim", 10);
				
					StringFormat format = new StringFormat();
					format.Alignment = StringAlignment.Center;
					format.LineAlignment = StringAlignment.Center;

					string msg;
					
					if(Tools.IsLangKorean()) 
					{
						msg = String.Format("오류:같은 모듈을 사용 했습니다.:{0}", objArgs.filename);
					}
					else 
					{
						msg = String.Format("Error:Parent & Child Module is same:{0}", objArgs.filename);
					}

                    SafeException.SafeDrawString(g, msg, font, Brushes.Black, r, format);
				}
			}
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			SaveObjectItem.FileName(writer, objArgs.filename);
		} 
	}
}

/*
void SetScreenSize(int x, int y)
{
	ObjectPublic :: SetScreenSize(x, y);

	if(hwndObject == NULL)	return;

	int x1, y1, x2, y2;
	
	x1 = GetViewPosX(rectZone.left);
	y1 = GetViewPosY(rectZone.top);
	x2 = GetViewPosX(rectZone.right);
	y2 = GetViewPosY(rectZone.bottom);

	MoveWindow(hwndObject, x1, y1, x2-x1+1, y2-y1+1, FALSE);
	InvalidateRect(hwndObject, NULL, FALSE);
}

void ObjectModule :: SetBasePoint(int x, int y)
{
	ObjectPublic :: SetBasePoint(x, y);

	if(hwndObject == NULL)	return;

	int x1, y1, x2, y2;
	
	x1 = GetViewPosX(rectZone.left);
	y1 = GetViewPosY(rectZone.top);
	x2 = GetViewPosX(rectZone.right);
	y2 = GetViewPosY(rectZone.bottom);

	MoveWindow(hwndObject, x1, y1, x2-x1+1, y2-y1+1, FALSE);
	InvalidateRect(hwndObject, NULL, FALSE);
}
*/
