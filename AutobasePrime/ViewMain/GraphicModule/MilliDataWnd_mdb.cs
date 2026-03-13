using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools.OldDefine;
using NetTools;
using System.Data;
using System.Data.OleDb;
using AutoLibLocal;
using System.IO;
using System.Windows.Documents;
using System.Collections.Generic;
using AutoLib;

namespace GraphicModule 
{
	/// <summary>
	/// Summary description for MilliDataWnd.
	/// </summary>
	public class MilliDataWnd_mdb : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		public static int nMilliDataBackColor = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nMilliDataBackColor", 0);
        public static bool bMilliDataTagDescription = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bMilliDataTagDescription", false);
                                                                                                        		
		public MilliDataWnd_mdb()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			//m_db = new ClassAdoConnection;
			//memset(&stStart, 0, sizeof(SYSTEMTIME));
			nTimeInterval = 0;
			nRecordCount = 0;
			nLimitX = 50;
			nScrollPosX = 0;
			nScrollMaxX = 0;
			nCursorPos = 0;
			bCursorFlag = false;
			nDesTag = 0;
			nSelectX1 = 0;
			nSelectX2 = 0;
			bBitmapFill = false;
			m_nTimeType = 0;
			for(int i = 0; i < MAX_VIEW_ACTIVE; i++)	bActive[i] = 1;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MilliDataWnd_mdb));
            this.scrollHorz = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // scrollHorz
            // 
            this.scrollHorz.AccessibleDescription = null;
            this.scrollHorz.AccessibleName = null;
            resources.ApplyResources(this.scrollHorz, "scrollHorz");
            this.scrollHorz.BackgroundImage = null;
            this.scrollHorz.Font = null;
            this.scrollHorz.Name = "scrollHorz";
            this.scrollHorz.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollHorz_Scroll);
            // 
            // MilliDataWnd
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.scrollHorz);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = null;
            this.Name = "MilliDataWnd";
            this.Load += new System.EventHandler(this.MilliDataWnd_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MilliDataWnd_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MilliDataWnd_Paint);
            this.SizeChanged += new System.EventHandler(this.MilliDataWnd_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MilliDataWnd_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MilliDataWnd_MouseMove);
            this.ResumeLayout(false);

		}
		#endregion

		int m_nTimeType;
		int cyChar;
		int cxChar;
		SYSTEMTIME  stStart = new SYSTEMTIME();
		int			nTimeInterval;
		DataTable dtData = new DataTable();
		DataTable dtMember = new DataTable();
		int gx1, gy1, gx2, gy2;
		int tx1, ty1, tx2, ty2;
		//HScrollBar scrollHorz = new HScrollBar();

		int	nLimitX;
		int nScrollPosX;
		int nScrollMaxX;
		int nCursorPos;
		bool bCursorFlag;
		int nDesTag;
		int nSelectX1, nSelectX2;
		Bitmap bmTotal;
		public bool bBitmapFill;
		const int MAX_VIEW_ACTIVE = 1000;
		sbyte[] bActive = new sbyte[MAX_VIEW_ACTIVE];
		int  nRecordCount;

		const int MAX_DEFINED_COLOR	= 6;
		private System.Windows.Forms.HScrollBar scrollHorz;

		Color[] definedColor = { 
										 Color.FromArgb(0, 255, 0),
										 Color.FromArgb(255, 255, 0),
										 Color.FromArgb(0, 255, 255),
										 Color.FromArgb(255, 0, 255),
										 Color.FromArgb(0, 0, 255),
										 Color.FromArgb(255, 0, 0),
		};

		Color GetMilliDataBackColor()
		{
			if(nMilliDataBackColor == 1)	return Color.White;
			else										return Color.Black;
		}

		Color GetDefinedColor(int no)
		{
			no %= MAX_DEFINED_COLOR;

			Color color = definedColor[no];

			if(nMilliDataBackColor == 1) 
			{
				int r, g, b;
				r = color.R;
				g = color.G;
				b = color.B;

				if(r > 0)	r = 0x80;
				if(g > 0)	g = 0x80;
				if(b > 0)	b = 0x80;

				return Color.FromArgb(r, g, b);
			}
			else 
			{
				return color;
			}
		}

		private void MilliDataWnd_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
	
			// TODO: Add your message handler code here
			RECT rect = new RECT();
			int  border_height = gy1-1;
			string buf;

			rect.Set(this.ClientRectangle);

			DrawClass.gcls(g, 0, 0, rect.right, rect.bottom, Color.LightGray);

			//dc.SetTextColor(DARK_COLOR);
			//dc.SetBkColor(WHITE_GRAY_COLOR);

			if(Tools.IsLangKorean()) 
			{
				buf = String.Format("시작시간:{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", stStart.wYear, stStart.wMonth, stStart.wDay,
					stStart.wHour, stStart.wMinute, stStart.wSecond);
			}
			else if(Tools.IsLangJapanese()) 
			{
				buf = String.Format("始作時間:{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", stStart.wYear, stStart.wMonth, stStart.wDay,
					stStart.wHour, stStart.wMinute, stStart.wSecond);
			}
			else if(Tools.IsLangChinese()) 
			{
				buf = String.Format("开始时间:{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", stStart.wYear, stStart.wMonth, stStart.wDay,
					stStart.wHour, stStart.wMinute, stStart.wSecond);
			}
            else if (Tools.IsLangVietnamese())
            {
                buf = String.Format("Thời gian bắt đầu:{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", stStart.wYear, stStart.wMonth, stStart.wDay,
                    stStart.wHour, stStart.wMinute, stStart.wSecond);
            }
			else 
			{
				buf = String.Format("Start Time:{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", stStart.wYear, stStart.wMonth, stStart.wDay,
					stStart.wHour, stStart.wMinute, stStart.wSecond);
			}

            SafeException.SafeDrawString(g, buf, this.Font, Brushes.Black, 10, 10);	

			if(Tools.IsLangKorean()) 
			{
				buf = String.Format("자료간격:{0} msec", nTimeInterval);
			}
			else if(Tools.IsLangJapanese()) 
			{
				buf = String.Format("データ間隔:{0} msec", nTimeInterval);
			}
			else if(Tools.IsLangChinese()) 
			{
				buf = String.Format("资料间隔:{0} msec", nTimeInterval);
			}
            else if (Tools.IsLangVietnamese())
            {
                buf = String.Format("Dữ liệu Gap:{0} msec", nTimeInterval);
            }
			else 
			{
				buf = String.Format("Data Gap:{0} msec", nTimeInterval);
			}

            SafeException.SafeDrawString(g, buf, this.Font, Brushes.Black, 10, 10 + cyChar);	

			if(Tools.IsLangKorean()) 
			{
				buf = String.Format("자료개수:{0}", nRecordCount);
			}
			else if(Tools.IsLangJapanese())  
			{
				buf = String.Format("データ個数:{0}", nRecordCount);
			}
			else if(Tools.IsLangChinese())  
			{
				buf = String.Format("资料数:{0}", nRecordCount);
			}
            else if (Tools.IsLangVietnamese())
            {
                buf = String.Format("Ghi số:{0}", nRecordCount);
            }
			else 
			{
				buf = String.Format("Record Count:{0}", nRecordCount);
			}

			SafeException.SafeDrawString(g, buf, this.Font, Brushes.Black, 10, 10+cyChar*2);

			DrawClass.gcls(g, gx1, gy1, gx2, gy2, GetMilliDataBackColor());

			DrawBarY(g);
			DrawBarX(g);

			if(bBitmapFill == false) 
			{
				Graphics gb = Graphics.FromImage(bmTotal);

				DrawClass.gcls(gb, 0, 0, tx2-tx1, ty2-ty1, GetMilliDataBackColor());
				DrawGraph(gb, 0, 0, tx2-tx1+1, ty2-ty1+1, 0, nRecordCount, false);
				bBitmapFill = true;
			}

			g.DrawImageUnscaled(bmTotal, tx1, ty1);

			DrawGraph(g, gx1, gy1, gx2, gy2, nScrollPosX, nLimitX, true);

			DrawTagBar(g);
			DrawSelectBar(g);
			DrawCursorPos(g);	
		}

		void ScrollUpdate()
		{
			if(nRecordCount > nLimitX) 
			{
				scrollHorz.Enabled = true;
				nScrollMaxX = nRecordCount-nLimitX;
				scrollHorz.Minimum = 0;
				scrollHorz.Maximum = nScrollMaxX+9;
				scrollHorz.Value = nScrollPosX;
			}
			else
				scrollHorz.Enabled = false;
		}

        public void SetFileNameWeb(string group_name, string file_name)
        {
            DataClose();

            Invalidate();

            ServiceLibDataGate sldg = new ServiceLibDataGate();

            int retn = sldg.Command("MilliDataGetOneFile", group_name, file_name);

            if (retn == -1)
            {
                MessageBox.Show(sldg.sErrorMessage, file_name);

                return;
            }

            DataSet ds = new DataSet();

            ds.ReadXml(new StringReader(sldg.GetResultString(0)));

            DataTable dt = ds.Tables["Header"];
            
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Record not exist on Header Table", file_name);
                return;
            }

            DataRow row = dt.Rows[0];

            DateTime t;

            object obj = row["StartTime"];
            if(obj.GetType() == typeof(DateTime))
                t = (DateTime)obj;
            else
                t = ConvertTool.ToDateTime(obj.ToString());

            stStart.Set(t);
            nTimeInterval = ConvertTool.ToInt32(row["TimeInterval"].ToString());

            dtMember = ds.Tables["Member"];
            dtData = ds.Tables["Data"];

            if(dtData == null)  // Row 가 없으면 테이블이 없다.
                nRecordCount = 0;
            else
                nRecordCount = dtData.Rows.Count;

            if (nRecordCount < 50) nLimitX = nRecordCount;
            else nLimitX = 50;

            nScrollPosX = 0;
            ScrollUpdate();

            SetSelectZone(nScrollPosX, nScrollPosX + nLimitX - 1);

            bBitmapFill = false;
            Invalidate();
        }

		public void SetFileNameLocal(string filename)
		{
			DataClose();

			Invalidate();

			string dsn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", filename);
			OleDbConnection db = new OleDbConnection(dsn);

            try
            {
                db.Open();
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("{0}\nfilename={1}", exception.Message, filename);
                return;
            }

			DataTable dt = new DataTable();
			string query = String.Format("SELECT * FROM Header");
			OleDbDataAdapter adapter = new OleDbDataAdapter(query, db);
			adapter.Fill(dt);

			if(dt.Rows.Count == 0) 
			{
				db.Close();
				MessageBox.Show("Record not exist on Header Table", filename);
				return;
			}

			DataRow row = dt.Rows[0];

			DateTime t = (DateTime)row["StartTime"];
			stStart.Set(t);
			nTimeInterval = ConvertTool.ToInt32(row["TimeInterval"].ToString());

			dtMember = new DataTable();
			query = String.Format("SELECT * FROM Member");
			adapter = new OleDbDataAdapter(query, db);
			adapter.Fill(dtMember);

			dtData = new DataTable();
			query = String.Format("SELECT * FROM Data ORDER BY [Data Count]");
			adapter = new OleDbDataAdapter(query, db);
			adapter.Fill(dtData);

			db.Close();

			nRecordCount = dtData.Rows.Count;

			if(nRecordCount < 50)	nLimitX = nRecordCount;
			else					nLimitX = 50;

			nScrollPosX = 0;
			ScrollUpdate();

			SetSelectZone(nScrollPosX, nScrollPosX+nLimitX-1);

			bBitmapFill = false;
			Invalidate();
		}

		private void MilliDataWnd_SizeChanged(object sender, System.EventArgs e)
		{
			int cx = this.ClientRectangle.Width;
			int cy = this.ClientRectangle.Height;

			ty1 = 20+cyChar*3;
			tx1 = 10;
			tx2 = cx-10;
			ty2 = ty1+(cy-ty1)/4;

			gy1 = ty2+cyChar+5;
			gx1 = cxChar*20;
			gx2 = cx-1;
			gy2 = cy-1-cyChar*5-5;

			scrollHorz.Left = gx1;
			scrollHorz.Top = gy2+1;
			scrollHorz.Width = gx2-gx1+1;
			scrollHorz.Height = cyChar;

			Graphics g = this.CreateGraphics();

			int width = tx2-tx1+1;
			int height = ty2-ty1+1;
			if(width < 1)	width = 1;
			if(height < 1)	height = 1;
			bmTotal = new Bitmap(width, height, g);
			bBitmapFill = false;	

			Invalidate();
		}

		void CalcCharSize()
		{
			cxChar = this.Font.Height/2;
			cyChar = this.Font.Height;
		}

		private void MilliDataWnd_Load(object sender, System.EventArgs e)
		{
			CalcCharSize();
		}

		void DrawGraph(Graphics g, int x1, int y1, int x2, int y2, int scroll_posx, int limitx, bool point_flag)
		{
			if(nRecordCount <= 1)	return;	// data not exist or data too small

			int i;

			bool move_flag = false;
			int x, y;

			int screen_width = x2-x1;
			int screen_height = y2-y1;
			string str;
			int f;
			int pos;
			double full=0, fBase=0;
			int nGab = 1;
			int varTagType;
			int di_pos;
			int di_height;
			sbyte active;
			Pen pen;
			int movex=0, movey=0;

			if(limitx > screen_width)
			{
				nGab = limitx/screen_width;
			}
			else 
			{
				nGab = 1;
			}

			DataRow row;

			for(f = 0; f < dtMember.Rows.Count; f++) 
			{
				row = dtMember.Rows[f];
				pen = new Pen(GetDefinedColor(f));

				str = row["Tag"].ToString();

				if(dtData.Columns.IndexOf(str) == -1)	continue;	// 컬럼이 존재하지 않는다.

				full = ConvertTool.ToDouble(row["Full"].ToString());
				fBase = ConvertTool.ToDouble(row["Base"].ToString());
				varTagType = ConvertTool.ToInt32(row["TagType"].ToString());

				if(f >= MAX_VIEW_ACTIVE)	active = 1;
				else						active = bActive[f];

				if(active == 0)	continue;	// 그리지 않는다.

				move_flag = false;

				if(dtMember.Rows.Count == 0)  
					di_height = screen_height;
				else
					di_height = screen_height/dtMember.Rows.Count;

				di_pos = y2-di_height*f;

				DataRow datarow;
				double var;

				for(i = 0, pos = scroll_posx; i < limitx && pos < nRecordCount; i+=nGab, pos+=nGab) 
				{
					datarow = dtData.Rows[pos];

					if(limitx <= 1) 
					{
						x = x1;
					}
					else 
					{
						x = x1+screen_width*i/(limitx-1);
					}

					var = ConvertTool.ToDouble(datarow[str].ToString());

					if(varTagType == 0) 
					{
						if(full-fBase == 0)
							y = y2;
						else
							y = (int)(y2-((var-fBase)*screen_height/(full-fBase)));

						if(y < y1)	y = y1;
						if(y > y2)	y = y2;
					}

					else 
					{	// DI tag
						if(var == 1) 
						{
							y = (int)(di_pos-di_height*0.9);
						}
						else 
						{
							y = (int)(di_pos-di_height*0.1);
						}
					}

					if(move_flag == false) 
					{
						move_flag = true;
						movex = x;
						movey = y;
					}
					else 
					{
						g.DrawLine(pen, movex, movey, x, y);
						movex = x;
						movey = y;
					}

					if(point_flag)
						DrawClass.gcls(g, x-1, y-1, x+1, y+1, GetDefinedColor(f));
				}
			}
		}

		void DrawBarX(Graphics g)
		{
			RECT r = new RECT();

			r.Set(this.ClientRectangle);

			Pen pen = new Pen(Color.DarkGray);
			int i, pos;
			int x;
			int screen_width = gx2-gx1;
			string buf;
			string date_buf;
			SizeF size;
			int old_textx = 0;
			int old_date_textx = 0;
			int gab = 1;
			int old_x = -100;

			//dc->SetTextColor(DARK_COLOR);
			//dc->SetBkColor(WHITE_GRAY_COLOR);

			for(i = 0, pos = nScrollPosX; i < nLimitX && pos < nRecordCount; i+=gab, pos+=gab) 
			{
				if(nLimitX <= 1) 
				{
					x = gx1;
				}
				else 
				{
					x = gx1+screen_width*i/(nLimitX-1);
				}

				if(x < old_x+15) 
				{	// 적어도 안내라인은 15칸 정도로 한다.
					continue;
				}
				old_x = x;

				g.DrawLine(pen, x, gy1, x, gy2);

				ChangePosToString(out buf, out date_buf, pos);

				size = g.MeasureString(buf, this.Font);

				r.left = (int)(x-size.Width/2);
				r.top = gy2+cyChar+1;
				r.right = (int)(r.left+size.Width);
				r.bottom = r.top+cyChar;

				if(r.left > old_textx+cxChar) 
				{
					DrawClass.DrawText(g, buf, this.Font, Brushes.Black, r, DrawClass.StringFormatCenter);
					old_textx = r.right;

					size = g.MeasureString(date_buf, this.Font);
					r.left = (int)(x-size.Width/2);
					r.top = gy2+cyChar*2+1;
					r.right = (int)(r.left+size.Width);
					r.bottom = r.top+cyChar;

					if(r.left > old_date_textx+cxChar) 
					{
						DrawClass.DrawText(g, date_buf, this.Font, Brushes.Black, r, DrawClass.StringFormatCenter);
						old_date_textx = r.right;
					}
				}
			}
		}

		void DrawBarY(Graphics g)
		{
			DrawClass.gcls(g, 10, gy1, gx1-10, gy2, GetMilliDataBackColor());

			//if(!m_db->IsOpen())		return;
			if(dtMember.Rows.Count == 0)	return;

			Pen pen = new Pen(Color.DarkGray);
			int screen_height = gy2-gy1;
			int i, y;
			int devide = 11;
			RECT r = new RECT();
			string buf;
			double		full = 100, fBase = 0;
			int			tag_type;

			if(nDesTag >= dtMember.Rows.Count)	nDesTag = 0;	// range over
			DataRow row = dtMember.Rows[nDesTag];

			full = ConvertTool.ToDouble(row["Full"].ToString());
			fBase = ConvertTool.ToDouble(row["Base"].ToString());
			tag_type = ConvertTool.ToInt32(row["TagType"].ToString());

			int di_height;
			if(dtMember.Rows.Count == 0)
				di_height = screen_height;
			else 
				di_height = screen_height/dtMember.Rows.Count;
			int di_pos = gy2-di_height*nDesTag;

			//pdc->SetTextColor(GetDefinedColor(nDesTag));
			//pdc->SetBkColor(GetMilliDataBackColor());
	
			if(tag_type == 0) 
			{	// AI
				for(i = 0; i < devide; i++) 
				{
					y = gy2-(i*screen_height/(devide-1));

					g.DrawLine(pen, gx1, y, gx2, y);

					buf = String.Format("{0:F2}", fBase+i*(full-fBase)/(devide-1));

					r.left = 10;
					r.top = y-cyChar/2;
					r.right = gx1-10-2;
					r.bottom = r.top+cyChar;

					if(r.bottom > gy2)	r.top = gy2-cyChar;
					if(r.top < gy1)		r.top = gy1;

					r.bottom = r.top+cyChar;

					DrawClass.DrawText(g, buf, this.Font, new SolidBrush(GetDefinedColor(nDesTag)), r, DrawClass.StringFormatRight);
				}
			}
			else 
			{
				r.left = 10;
				r.top = di_pos-cyChar;
				r.right = gx1-10-2;
				r.bottom = di_pos;

				DrawClass.DrawText(g, "OFF", this.Font, new SolidBrush(GetDefinedColor(nDesTag)), r, DrawClass.StringFormatRight);

				r.left = 10;
				r.top = di_pos-di_height;
				r.right = gx1-10-2;
				r.bottom = r.top+cyChar;

				DrawClass.DrawText(g, "ON", this.Font, new SolidBrush(GetDefinedColor(nDesTag)), r, DrawClass.StringFormatRight);
			}
		}

		void DrawTagBar(Graphics g)
		{
			//if(!m_db->IsOpen())		return;

			double var;
			int tag_type;

			int x, y;

			int screen_width = gx2-gx1;
			int screen_height = gy2-gy1;
			string str;
			int f;
			double full=0, fBase=0;
			Color color;
			RECT rect = new RECT();
			RECT r = new RECT();
			string buf;

			rect.Set(this.ClientRectangle);

			y = gy2+cyChar*2+2;

			//pdc->SetTextColor(DARK_COLOR);
			//pdc->SetBkColor(WHITE_GRAY_COLOR);

			if(nDesTag >= dtMember.Rows.Count)
				nDesTag = 0;

			DataRow row;

			for(x = 0, f = 0; f < dtMember.Rows.Count && x < rect.right; f++, x+=cxChar*15) 
			{
				color = GetDefinedColor(f);

				row = dtMember.Rows[f];

				str = row["Tag"].ToString();

				full = ConvertTool.ToDouble(row["Full"].ToString());
				fBase = ConvertTool.ToDouble(row["Base"].ToString());
				tag_type = ConvertTool.ToInt32(row["TagType"].ToString());

				DrawClass.PopBox2(g, x, y+cyChar, x+cxChar*15-1, y+cyChar*2, Color.LightGray);
				DrawClass.PushBox2(g, x+2, y+2+cyChar, x+6, y+cyChar*2-2, color);

				r.left = x+9;
				r.top = y+1+cyChar;
				r.right = x+cxChar*15-1;
				r.bottom = y+cyChar*2;
	
				Brush brush;
				if(f < MAX_VIEW_ACTIVE && bActive[f] == 0) 
				{
					brush = Brushes.DarkGray;
				}	
				else 
					if(nDesTag == f)	brush = Brushes.White;
				else					brush = Brushes.Black;

                if (MilliDataWnd.bMilliDataTagDescription) // 태그 설명으로 표시
                {
                    TagPublicClass tp;
                    int[] tag_pos = new int[1];
                    tp = TagLib.GetStructPublic(str, ref tag_pos);
                    DrawClass.DrawText(g, tp.description, this.Font, brush, r, DrawClass.StringFormatLeft);
                }
                else
                {
                    DrawClass.DrawText(g, str, this.Font, brush, r, DrawClass.StringFormatLeft);
                }

				DrawClass.PopBox2(g, x, y+cyChar*2+1, x+cxChar*15-1, y+cyChar*2+1+cyChar, Color.LightGray);

				if(bCursorFlag) 
				{
					DataRow datarow = dtData.Rows[nCursorPos];

					var = ConvertTool.ToDouble(datarow[str].ToString());

					if(tag_type == 0) 
					{
						buf = String.Format("{0:F2}", var);
					} 
					else 
					{
						if(var == 1)	buf = "ON";
						else			buf = "OFF";
					}

					r.left = x+9;
					r.top = y+cyChar*2+2;
					r.right = x+cxChar*15-1;
					r.bottom = y+cyChar+1+cyChar*2;
		
					DrawClass.DrawText(g, buf, this.Font, Brushes.Black, r, DrawClass.StringFormatLeft);
				}
			}
		}

		void DrawCursorPos(Graphics g)
		{
			if(bCursorFlag == false)					return;
			if(nCursorPos < nScrollPosX)			return;		// range over
			if(nCursorPos >= nScrollPosX+nLimitX)	return;

			int screen_width = gx2-gx1;
			int x;
	
			if(nLimitX <= 1) 
				x = gx1;
			else
				x = gx1+screen_width*(nCursorPos-nScrollPosX)/(nLimitX-1);

			RECT r = new RECT();
			RECT rect = new RECT();

			rect.Set(this.ClientRectangle);

			r.left = x-1;
			r.top = gy1;
			r.right = x+2;
			r.bottom = gy2;
			DrawClass.InvertRect(g, r);

			string time_buf;
			string date_buf;
			string buf;

			ChangePosToString(out time_buf, out date_buf, nCursorPos);

			if(m_nTimeType == 1) 
			{
				buf = date_buf;
				buf += "  ";
				buf += time_buf;
			}
			else 
			{
				buf = date_buf;
			}

			SizeF size = g.MeasureString(buf, this.Font);

			r.left = (int)(x-size.Width/2);
			r.top = gy1-cyChar-1;
			r.right = (int)(r.left+size.Width);
			r.bottom = r.top+cyChar;

			if(r.right > rect.right)	r.left = (int)(rect.right-size.Width);
			if(r.left < 0)				r.left = 0;

			r.right = (int)(r.left+size.Width);

			DrawClass.DrawText(g, buf, this.Font, Brushes.Black, r, DrawClass.StringFormatLeft);
		}

		bool CheckTagSelect(ref int pos, System.Windows.Forms.MouseEventArgs e)
		{
			int y = gy2+cyChar*2+2;	

			//if(!m_db->IsOpen())		return 0;

			int tag_hap;
			int x, f;
			RECT rect = new RECT();

			rect.Set(this.ClientRectangle);
	
			tag_hap = dtMember.Rows.Count;

			for(x = 0, f = 0; f < tag_hap && x < rect.right; f++, x+=cxChar*15) 
			{
				if(e.X >= x && e.X <= x+cxChar*15 && e.Y > y) 
				{
					pos = f;
					return true;
				}
			}

			return false;
		}

		static int nStartX, nEndX;
		static bool bMouseCapture;

		private void MilliDataWnd_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left) 
			{
				OnLButtonDown(e);
			}
			else if(e.Button == MouseButtons.Right) 
			{
				OnRButtonDown(e);
			}
		}

		void OnRButtonDown(System.Windows.Forms.MouseEventArgs e) 
		{
			// TODO: Add your message handler code here and/or call default
	
			int tag_pos = 0;

			if(!CheckTagSelect(ref tag_pos, e))	return;

			if(tag_pos >= MAX_VIEW_ACTIVE)	return;
	
			bActive[tag_pos] = bActive[tag_pos] == 1 ? (sbyte)0 : (sbyte)1;

			Invalidate();
		}

		void OnLButtonDown(System.Windows.Forms.MouseEventArgs e)
		{
			// TODO: Add your message handler code here and/or call default
	
			int tag_pos = 0;

			if(CheckTagSelect(ref tag_pos, e)) 
			{
				nDesTag = tag_pos;
				Invalidate();
				return;
			}

			int screen_width;

			if(e.X >= tx1 && e.X <= tx2 && e.Y >= ty1 && e.Y <= ty2) 
			{
				Graphics g = CreateGraphics();
		
				//DrawSelectBar(g);
				this.Capture = true;
				bMouseCapture = true;
				screen_width = tx2-tx1;
				nStartX = (int)((double)(e.X-tx1)*(nRecordCount-1)/(screen_width)+0.5);
				nEndX = nStartX;
				DrawSelectBar(g);
				return;
			}

			if(e.X < gx1)	return;
			if(e.X > gx2)	return;
			if(e.Y < gy1)	return;
			if(e.Y > gy2)	return;

			screen_width = gx2-gx1;
			int pos = (int)(nScrollPosX+(double)(e.X-gx1)*(nLimitX-1)/(screen_width)+0.5);

			if(pos < nScrollPosX)			return;
			if(pos >= nScrollPosX+nLimitX)	return;

			nCursorPos = pos;
			bCursorFlag = true;

			//RECT r;

			//GetClientRect(&r);
			//r.top = gy1-cyChar;
			//InvalidateRect(&r);
			this.Invalidate();
		}

		private void MilliDataWnd_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(bMouseCapture == false)	return;

			int screen_width = tx2-tx1;
			int pos = (int)((double)(e.X-tx1)*(nRecordCount-1)/(screen_width)+0.5);

			if(pos < 0)	pos = 0;
			if(pos >= nRecordCount)	pos = nRecordCount-1;
			if(pos == nEndX)	return;

			Graphics g = CreateGraphics();
			//DrawSelectBar(g);

			nEndX = pos;

			RECT r = new RECT();

			r.left = tx1;
			r.top = ty1;
			r.right = tx2+1;
			r.bottom = ty2+1;

			DrawSelectBar(g);	
		}

		private void MilliDataWnd_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(bMouseCapture == false)	return;

			bMouseCapture = false;
			this.Capture = false;;

			SetSelectZone(nStartX, nEndX);
			nScrollPosX = nSelectX1;
			nLimitX = nSelectX2-nSelectX1+1;

			ScrollUpdate();

			Invalidate();	
		}

		void ChangePosToString(out string string_time, out string string_date, int pos)
		{
			if(m_nTimeType == 0) 
			{
				int msec = pos*nTimeInterval;
				int hour = msec/(1000*3600);
				msec %= (1000*3600);
				int minute = msec/(1000*60);
				msec %= (1000*60);
				int sec = msec/1000;
				msec %= 1000;

				if(nTimeInterval < 1000) 
				{
					string_time = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", hour, minute, sec, msec);
				}
				else 
				{
					string_time = String.Format("{0:00}:{1:00}:{2:00}", hour, minute, sec);
				}
				string_date = "";
			}
			else 
			{
				DateTime tm = stStart.ToDateTime();

				int msec = pos*nTimeInterval;

				tm = tm.AddMilliseconds(msec);

				if(nTimeInterval < 1000) 
				{
					string_time = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", tm.Hour, tm.Minute, tm.Second, tm.Millisecond);
				}
				else 
				{
					string_time = String.Format("{0:00}:{1:00}:{2:00}", tm.Hour, tm.Minute, tm.Second);
				}

				string_date = String.Format("{0:0000}-{1:00}-{2:00}", tm.Year, tm.Month, tm.Day);
			}
		}

		void DataClose()
		{
			//if(m_db->IsOpen())	m_db->Close();
			nTimeInterval = 0;
			nRecordCount = 0;
			bCursorFlag = false;
			stStart = new SYSTEMTIME();
			scrollHorz.Enabled = false;;
		}

		void DrawSelectBar(Graphics g)
		{
			//if(!m_db->IsOpen())		return;
			g.DrawImageUnscaled(bmTotal, tx1, ty1);

			int dx1, dx2;

			if(bMouseCapture) 
			{
				dx1 = nStartX;
				dx2 = nEndX;
			}
			else 
			{
				dx1 = nSelectX1;
				dx2 = nSelectX2;
			}

			if(dx1 > dx2)	Tools.Temp(ref dx1, ref dx2);

			int screen_width = tx2-tx1;

			int x1;
			int x2;

			if(nRecordCount <= 1) 
			{
				x1 = tx1;
				x2 = tx1;
			}
			else 
			{
				x1 = tx1+screen_width*(dx1)/(nRecordCount-1);
				x2 = tx1+screen_width*(dx2)/(nRecordCount-1);
			}

			RECT r = new RECT();
			RECT rect = new RECT();

			rect.Set(this.ClientRectangle);

			r.left = x1;
			r.top =  ty1;
			r.right = x2+1;
			r.bottom = ty2;
			DrawClass.InvertRect(g, r);
		}

		void SetSelectZone(int x1, int x2)
		{
			nSelectX1 = x1;
			nSelectX2 = x2;

			if(nSelectX1 > nSelectX2)	Tools.Temp(ref nSelectX1, ref nSelectX2);

			if(nSelectX1 < 0)				nSelectX1 = 0;
			if(nSelectX2 >= nRecordCount)	nSelectX2 = nRecordCount-1;
		}

		public void SetTimeType(int type)
		{
			m_nTimeType = type;
			Invalidate();
		}

		public int GetTimeType()
		{
			return m_nTimeType;
		}

		private void scrollHorz_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if(nScrollPosX == e.NewValue)	return;
			
			nScrollPosX = e.NewValue;
			if(nScrollPosX < 0)				nScrollPosX = 0;
			if(nScrollPosX > nScrollMaxX)	nScrollPosX = nScrollMaxX;

			//scrollHorz.SetScrollPos(nScrollPosX);
			SetSelectZone(nScrollPosX, nScrollPosX+nLimitX-1);
			Invalidate();
		}

		public void SetFont(Font font)
		{
			this.Font = font;
			CalcCharSize();
			this.ScrollUpdate();
		}
	}

    public class MilliDataBasicReader
    {
        public List<string> LoadGroupList() 
        {
            List<string> array = new List<string>();

            string path;

            path = String.Format("{0}\\MiliData", TotalConfig.GetProjectDataDirectory());

            if (!Directory.Exists(path)) return array;

            DirectoryInfo info = new DirectoryInfo(path);

            foreach (DirectoryInfo di in info.GetDirectories())
            {
                array.Add(di.Name);
            }

            return array;
        }

        public List<string> LoadMemberList(string group_name)
        {
            List<string> array = new List<string>();

            string path;

            path = String.Format("{0}\\MiliData\\{1}", TotalConfig.GetProjectDataDirectory(), group_name);

            DirectoryInfo info = new DirectoryInfo(path);

            foreach (FileInfo fi in info.GetFiles("*.mdb"))
            {
                array.Add(fi.Name);
            }

            return array;
        }

        public DataSet GetFile(string group_name, string file_name, out string err_msg)
        {
            err_msg = "";
            string path;

            path = String.Format("{0}\\MiliData\\{1}\\{2}", TotalConfig.GetProjectDataDirectory(), group_name, file_name);

            string dsn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", path);
            OleDbConnection db = new OleDbConnection(dsn);

            DataSet ds = new DataSet();

            try
            {
                db.Open();
            }
            catch (Exception exception)
            {
                err_msg = exception.Message;
                return null;
            }

            DataTable dt = new DataTable("Header");
            string query = String.Format("SELECT * FROM Header");
            OleDbDataAdapter adapter = new OleDbDataAdapter(query, db);
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                db.Close();
                err_msg = String.Format("Row not exist on Header Table");
                return null;
            }

            ds.Tables.Add(dt);

            dt = new DataTable("Member");
            query = String.Format("SELECT * FROM Member");
            adapter = new OleDbDataAdapter(query, db);
            adapter.Fill(dt);

            ds.Tables.Add(dt);

            dt = new DataTable("Data");
            query = String.Format("SELECT * FROM Data ORDER BY [Data Count]");
            adapter = new OleDbDataAdapter(query, db);
            adapter.Fill(dt);

            db.Close();

            ds.Tables.Add(dt);

            return ds;
        }
    }
}

/*
void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar) 
{
	// TODO: Add your message handler code here and/or call default
	
	switch(nSBCode) {
		case SB_LINELEFT:
			if(nScrollPosX <= 0)	return;
			nScrollPosX--;
			scrollHorz.SetScrollPos(nScrollPosX);
			SetSelectZone(nScrollPosX, nScrollPosX+nLimitX-1);
			Invalidate();
			break;
		case SB_LINERIGHT:
			if(nScrollPosX >= nScrollMaxX)	return;
			nScrollPosX++;
			scrollHorz.SetScrollPos(nScrollPosX);
			SetSelectZone(nScrollPosX, nScrollPosX+nLimitX-1);
			Invalidate();
			break;
		case SB_PAGELEFT:
			if(nScrollPosX <= 0)	return;
			nScrollPosX-=nLimitX;
			if(nScrollPosX < 0)	nScrollPosX = 0;
			scrollHorz.SetScrollPos(nScrollPosX);
			SetSelectZone(nScrollPosX, nScrollPosX+nLimitX-1);
			Invalidate();
			break;
		case SB_PAGERIGHT:
			if(nScrollPosX >= nScrollMaxX)	return;
			nScrollPosX+=nLimitX;
			if(nScrollPosX > nScrollMaxX)	nScrollPosX = nScrollMaxX;
			scrollHorz.SetScrollPos(nScrollPosX);
			SetSelectZone(nScrollPosX, nScrollPosX+nLimitX-1);
			Invalidate();
			break;
		case SB_THUMBPOSITION:
			nScrollPosX = nPos;
			if(nScrollPosX < 0)				nScrollPosX = 0;
			if(nScrollPosX > nScrollMaxX)	nScrollPosX = nScrollMaxX;
			scrollHorz.SetScrollPos(nScrollPosX);
			SetSelectZone(nScrollPosX, nScrollPosX+nLimitX-1);
			Invalidate();
			break;
	}
}

*/ 
