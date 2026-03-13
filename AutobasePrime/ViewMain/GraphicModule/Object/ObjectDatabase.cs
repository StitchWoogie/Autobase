﻿using System;
using AutoLibLocal;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using AutoLib;
using NetTools.OldDefine;
using NetTools;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GraphicModule
{

	[Serializable]
	public class ObjectArgsDatabase
	{
		public string dsn;
		public string filename;
		public string table;
		public sbyte bUseNo;
		public Color lColorText;
		public BrushPublic lColorBack = new BrushPublic();
		public int  nConnectionType;
		public sbyte bAutoUpdate;
		public bool  bUseGrid;
		public sbyte bUseFullCursor;
		public int   nUpdateTime;
        public bool bReverseNo; 
		public string sSqlTextWhere = "";
		public string sSqlTextOrderBy = "";
		public int nRecordLimit;
		public bool bUseAlternateRowColor;
		public bool bUsePagination;
		public int nPageSize = 100;
	}

	/// <summary>
	/// Summary description for ObjectDatabase.
	/// </summary>
	
	[Serializable]
	public class ObjectDatabase : ObjectExpand
	{
		ObjectArgsDatabase objArgs;

		[NonSerialized]
		WndDatabase wndChild;// = new WndDatabase();

        [NonSerialized]
        Form formParent; // 20250312 PSU

		[NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        public ObjectArgsDatabase ObjectArgs 
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

		public ObjectDatabase(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsDatabase args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.Database;
            bSupportObjectOnCE = false;
			objArgs = args;

            formParent = form; //20250312 PSU

			if(objArgs.nUpdateTime < 2)	objArgs.nUpdateTime = 2;	// 최소 2초

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				wndChild = new WndDatabase();
				wndChild.SetClassName(general.sClassName);
				wndChild.sFileName = objArgs.filename;
				wndChild.sTableName = objArgs.table;
				wndChild.sDsnName = objArgs.dsn;
				wndChild.m_bUseNo = (objArgs.bUseNo == 1);
				wndChild.bMdbOrString = objArgs.nConnectionType;
				wndChild.bAutoUpdate = (objArgs.bAutoUpdate == 1);
				wndChild.bUseGrid = objArgs.bUseGrid;
				wndChild.bUseFullCursor = (objArgs.bUseFullCursor == 1);
				wndChild.nUpdateTime = objArgs.nUpdateTime;
                wndChild.bReverseNo = objArgs.bReverseNo;
				wndChild.nRecordLimit = objArgs.nRecordLimit;
				wndChild.bUseAlternateRowColor = objArgs.bUseAlternateRowColor;
				wndChild.bUsePagination = objArgs.bUsePagination;
				wndChild.nPageSize = objArgs.nPageSize > 0 ? objArgs.nPageSize : 100;
				wndChild.SetSqlText(objArgs.sSqlTextWhere ?? "", objArgs.sSqlTextOrderBy ?? "");

				wndChild.TopLevel = false;
				wndChild.FormBorderStyle = FormBorderStyle.None;

                wndChild.Show();                // form.Coltrols.Add 후에 Show하면 초기 크기가 맞지 않는다.
				form.Controls.Add(wndChild);
	
                
				int x1=0, y1=0, x2=0, y2=0;
				GetViewZone(ref x1, ref y1, ref x2, ref y2);
				wndChild.Left = x1;
				wndChild.Top = y1;
				wndChild.Width = x2-x1;
				wndChild.Height = y2-y1;

                wndChild.SetTextColor(objArgs.lColorText);
                wndChild.SetBackColor(objArgs.lColorBack.basic_color);
                
				//wndChild.Show();

				wndChild.Font = MakeFont();

				arrayClassList.Add(this);

				wndChild.procSelChange = new GraphicModule.WndDatabase.DelegateSelChange(this.OnEventSelChange);

				_ = base.SetToolTipOnChildWindow(wndChild.m_list);
                OnVisible(ExpandCalcVisible());

                /*
                int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                GetViewZone(ref x1, ref y1, ref x2, ref y2);
                OnMove(x1, y1, x2, y2);*/
            }
			
			TextColor = objArgs.lColorText;
			BackColor = objArgs.lColorBack;
		}

		public override void Close()
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				wndChild.SaveClassConfig();
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

                Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);
				DrawClass.PopBox2(g, x1, y1, x2, y2, brushback);

				string buf;
				int i, y;

				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Near;
				format.LineAlignment = StringAlignment.Near;
				format.FormatFlags |= StringFormatFlags.NoWrap;
				Brush brush = new SolidBrush(RunColorText);
				Color altColor = Color.FromArgb(245, 248, 252);

				int drawY2 = y2;
				int paginationBarHeight = 0;
				if(objArgs.bUsePagination)
				{
					paginationBarHeight = cyChar + 4;
					drawY2 = y2 - paginationBarHeight;
				}

				for(i = 0, y = y1; y + cyChar <= drawY2; y+=cyChar, i++) 
				{
					// Alternating row color
					if(objArgs.bUseAlternateRowColor && i % 2 == 1)
					{
						using(Brush altBrush = new SolidBrush(altColor))
							g.FillRectangle(altBrush, x1, y, x2 - x1, cyChar);
					}

					r.left = x1;
					r.top = y;
					r.right = x2;
					r.bottom = y + cyChar;

					if(objArgs.bUseNo == 1) 
					{
                        if (objArgs.bReverseNo)
                            buf = String.Format("{0:000}", 100 - i);
                        else
                            buf = String.Format("{0:000}", i + 1);

						DrawClass.DrawText(g, buf, font, brush, r, format);
						r.left += cxChar*5;
					}

					if(r.left >= r.right)	continue;

					buf = String.Format("2026-02-26");
					DrawClass.DrawText(g, buf, font, brush, r, format);
					r.left += cxChar*13;

					if(r.left >= r.right)	continue;

					buf = String.Format("15:50:00");
					DrawClass.DrawText(g, buf, font, brush, r, format);
					r.left += cxChar*10;

					if(r.left >= r.right)	continue;

					buf = String.Format("Column1_Row{0:000}", i);
					DrawClass.DrawText(g, buf, font, brush, r, format);
					r.left += cxChar*19;

					if(r.left >= r.right)	continue;

					buf = String.Format("Column2_Row{0:000}", i);
					DrawClass.DrawText(g, buf, font, brush, r, format);
				}

				// Draw pagination bar
				if(objArgs.bUsePagination && paginationBarHeight > 0)
				{
					int barY = y2 - paginationBarHeight;
					using(Brush barBrush = new SolidBrush(Color.FromArgb(240, 240, 240)))
						g.FillRectangle(barBrush, x1, barY, x2 - x1, paginationBarHeight);
					using(Pen borderPen = new Pen(Color.FromArgb(200, 200, 200)))
						g.DrawLine(borderPen, x1, barY, x2, barY);

					int pageCount = objArgs.nPageSize > 0 ? (100 / objArgs.nPageSize + 1) : 1;
					string pageText = String.Format("<  1/{0}  >", pageCount);
					StringFormat sfPage = new StringFormat();
					sfPage.Alignment = StringAlignment.Far;
					sfPage.LineAlignment = StringAlignment.Center;
					RECT rPage = new RECT();
					rPage.left = x1;
					rPage.top = barY;
					rPage.right = x2 - 4;
					rPage.bottom = y2;
					DrawClass.DrawText(g, pageText, font, brush, rPage, sfPage);
				}
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

                    // 컨트롤 업데이트 일시 중지 //20250312 PSU
                    wndChild.SuspendLayout();

					Font font = MakeFont();
					wndChild.Font = font;

					wndChild.Left = x1;
					wndChild.Top = y1;
					wndChild.Width = Math.Abs(x2-x1);
					wndChild.Height = Math.Abs(y2-y1);

                    // 컨트롤 업데이트 재개
                    wndChild.ResumeLayout(false);

                    wndChild.PerformLayout(); // 레이아웃 강제 수행, objectdatabase는 필요.

				}
			}
		}

		public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
            if (command == "DatabaseSetFilter") 
			{
				wndChild.SetSqlText((string)args[1], (string)args[2]);
				await wndChild.ReLoad();
				return 1;
			}
			else if(command == "DatabaseReLoad") 
			{
				await wndChild.ReLoad();
				return 1;
			}
			else if(command == "DatabaseGetCurSel") 
			{
				return wndChild.GetCurSel();
			}
			else if(command == "DatabaseSetCurSel") 
			{
				wndChild.SetCurSel((int)args[1]);
				return 1;
			}
			else if(command == "DatabaseSetConnection") 
			{
				await wndChild.SetConnection((string)args[1], (string)args[2]);
				return 1;
			}
            else if (command == "DatabaseSetSelect")
            {
                await wndChild.SetSelect((string)args[1]);
                return 1;
            }
			else if(command == "DatabaseSetTable") 
			{
				await wndChild.SetTable((string)args[1]);
				return 1;
			}
            else if (command == "DatabaseSetRecordLimit")
			{
				await wndChild.SetRecordLimit((int)args[1]);
				return 1;
			}
            else if (command == "DatabaseGetValue")
            {
                args[3] = wndChild.GetValue((int)args[1], (string)args[2]);
                return 1;
            }

			return 0;
		}


		public override void ObjectSave(CommaTextWriter writer)
		{
			ObjectSaveFont(writer);

			SaveObjectItem.FileName(writer, objArgs.filename);
			SaveObjectItem.BackColor(writer, GetBackColor());
			SaveObjectItem.TextColor(writer, GetTextColor());
			
			writer.Write("\tStringOption,");
			writer.Write("{0},", objArgs.table);
			writer.Write("{0},", objArgs.bUseNo);
			writer.Write("{0},", objArgs.nConnectionType);
			writer.Write("{0},", objArgs.bAutoUpdate);
			writer.Write("{0},", objArgs.bUseGrid);
			writer.Write("{0},", objArgs.bUseFullCursor);
			writer.Write("{0},", objArgs.nUpdateTime);
			writer.Write("{0},", objArgs.dsn);
            writer.Write("{0},", objArgs.bReverseNo);
			writer.Write("{0},", objArgs.sSqlTextWhere ?? "");
			writer.Write("{0},", objArgs.sSqlTextOrderBy ?? "");
			writer.Write("{0},", objArgs.nRecordLimit);
			writer.Write("{0},", objArgs.bUseAlternateRowColor);
			writer.Write("{0},", objArgs.bUsePagination);
			writer.Write("{0},", objArgs.nPageSize);
			writer.WriteLine();
		}

        protected override void TextColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            wndChild.SetTextColor(this.RunColorText);
        }

        protected override void BackColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            wndChild.SetBackColor(this.RunColorBack.basic_color);
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

