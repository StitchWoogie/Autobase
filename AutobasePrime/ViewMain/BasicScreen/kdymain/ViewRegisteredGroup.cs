using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;
using AutoLib;
using NetTools;
using System.Data;
using DialogControl;
using AutoLibLocal;
using System.Drawing.Drawing2D;
using System.IO;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewRegisteredGroup.
	/// </summary>
	public class ViewRegisteredGroup : AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ContextMenu contextMenuRegisteredGroup;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.ComponentModel.IContainer components;

		public int				xnum = 125;
		public int				groupHap, gWidth, gHeight, gRealWidth, gRealHeight;
		bool					bDataChange;
		ArrayList				memberArr = new ArrayList();
		groupShowTagGroupMember	gr = new groupShowTagGroupMember();
		groupShowTagMember		item = new groupShowTagMember();
		
		private System.Windows.Forms.MenuItem menuItem_GroupDetail;
		private System.Windows.Forms.MenuItem menuItem_InsertGroup;
		private System.Windows.Forms.MenuItem menuItem_AddGroup;
		private System.Windows.Forms.MenuItem menuItem_DeleteGroup;
		private System.Windows.Forms.MenuItem menuItem_ModifyGroup;	
		

		public ViewRegisteredGroup(Form parent)
            
		{
            parentForm = parent;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.label1.Left = -100;
			work.bDisplayFitWindowSize = true;	// 항상 bFitToWindow = true
			
			RegisteredGroupInitValueSetting();
			registeredGroupSetSize();
		}
		

		void readGroupShowData()
		{
			string					filename;
			bool					bNewVersion = true, bUniCode = true;
			
			filename = TotalConfig.sDirWorkProject + "\\TAG\\REGISTERED_GROUP2.TAG";	// tag position 을 저장한 버전
			if(!File.Exists(filename)) 
			{
				bNewVersion = false;
				filename = TotalConfig.sDirWorkProject + "\\TAG\\REGISTERED_GROUP.TAG";	// uni code 버전
				if(!File.Exists(filename)) 
				{
					filename = TotalConfig.sDirWorkProject + "\\TAG\\GROUPSHOW.TAG";	// 이전 local 버전
					if(!File.Exists(filename)) return;
					bUniCode = false;
				}
				bDataChange = true;														// 이전버전이므로 저장해야 한다.
			}
			
			TextReader reader;
			if(memberArr == null) memberArr = new ArrayList();
			if(bUniCode)
				reader = new StreamReader(filename);
			else
				reader = new StreamReader(filename, System.Text.Encoding.Default);
			if(reader == null) return;

			CommaBlockString comma = new CommaBlockString();

			string					imsi = "", one_line;
			int						i, pos = 0;
			
			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null || one_line.Length == 0) break;

				comma.Set(one_line);
				gr = new groupShowTagGroupMember();
				comma.GetString(ref imsi);
				if(imsi == null || imsi.Length <= 0) continue;
				gr.name = imsi;
				comma.GetInt(ref gr.tagCount);
				if(gr.tagCount > 10000) gr.tagCount = 10000;

				gr.tag = new ArrayList();
				for(i = 0; i < gr.tagCount; i++)
				{	
					if(comma.IsEOS()) break;
					comma.GetString(ref imsi);
					if(imsi == null || imsi.Length <= 0) break;		// 파일이 손상되었다고 판단
					
					item = new groupShowTagMember();
					item.tag = imsi;
					item.tagType = AutoLibLocal.EnumTagType.none;
					if(bNewVersion) 
					{
						comma.GetString(ref imsi);
						if(imsi == null || imsi.Length <= 0) break;	// 파일이 손상되었다고 판단
						item.tagPos = BasicScreenTool.StringToTagPos(imsi);
					}
					TagLib.GetTagTypeAndPos(item.tag, ref item.tagType, ref item.tagPos);					
					gr.tag.Add(item);
				}
				gr.tagCount = gr.tag.Count;
				memberArr.Add(gr);
				pos++;
				if(pos >= 32000) break;
			}
			reader.Close();
			groupHap = memberArr.Count;
			work.TagHap = (groupHap + 2) / 3;
		}

		void saveGroupShowData()
		{
			string					filename;
			
			filename = TotalConfig.sDirWorkProject + "\\TAG\\REGISTERED_GROUP2.TAG";
			
			int				i, j, count = memberArr.Count;
			TextWriter		writer = new StreamWriter(filename);

			if(writer == null) return;
			
			for(i = 0; i < count; i++) 
			{		
				gr = (groupShowTagGroupMember)memberArr[i];
				writer.Write("{0},{1},", gr.name, gr.tag.Count);
				if(gr.tag != null) 
				{
					for(j = 0; j < gr.tag.Count; j++) 
					{
						item = (groupShowTagMember)gr.tag[j];
						if(item == null) continue;
						writer.Write("{0},{1},", item.tag, BasicScreenTool.TagPosToString(item.tagPos));
					}
				}
				writer.WriteLine("");
			}
			writer.Close();
		}

		private void RegisteredGroupInitValueSetting()
		{
			bDataChange = false;			// 그룹 보기 내용이 바뀌었는가 ?
			work.bStartFlag = true;
			work.spos = 0;
			work.pos = 0;
			work.Ix = 0;
			work.Iy = 0;			
			readGroupShowData();			
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewRegisteredGroup));
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuRegisteredGroup = new System.Windows.Forms.ContextMenu();
            this.menuItem_GroupDetail = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_InsertGroup = new System.Windows.Forms.MenuItem();
            this.menuItem_AddGroup = new System.Windows.Forms.MenuItem();
            this.menuItem_ModifyGroup = new System.Windows.Forms.MenuItem();
            this.menuItem_DeleteGroup = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuRegisteredGroup
            // 
            this.contextMenuRegisteredGroup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_GroupDetail,
            this.menuItem2,
            this.menuItem_InsertGroup,
            this.menuItem_AddGroup,
            this.menuItem_ModifyGroup,
            this.menuItem_DeleteGroup,
            this.menuItem7,
            this.menuItem8});
            // 
            // menuItem_GroupDetail
            // 
            this.menuItem_GroupDetail.Index = 0;
            resources.ApplyResources(this.menuItem_GroupDetail, "menuItem_GroupDetail");
            this.menuItem_GroupDetail.Click += new System.EventHandler(this.menuItem_GroupDetail_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItem_InsertGroup
            // 
            this.menuItem_InsertGroup.Index = 2;
            resources.ApplyResources(this.menuItem_InsertGroup, "menuItem_InsertGroup");
            this.menuItem_InsertGroup.Click += new System.EventHandler(this.menuItem_InsertGroup_Click);
            // 
            // menuItem_AddGroup
            // 
            this.menuItem_AddGroup.Index = 3;
            resources.ApplyResources(this.menuItem_AddGroup, "menuItem_AddGroup");
            this.menuItem_AddGroup.Click += new System.EventHandler(this.menuItem_AddGroup_Click);
            // 
            // menuItem_ModifyGroup
            // 
            this.menuItem_ModifyGroup.Index = 4;
            resources.ApplyResources(this.menuItem_ModifyGroup, "menuItem_ModifyGroup");
            this.menuItem_ModifyGroup.Click += new System.EventHandler(this.menuItem_ModifyGroup_Click);
            // 
            // menuItem_DeleteGroup
            // 
            this.menuItem_DeleteGroup.Index = 5;
            resources.ApplyResources(this.menuItem_DeleteGroup, "menuItem_DeleteGroup");
            this.menuItem_DeleteGroup.Click += new System.EventHandler(this.menuItem_DeleteGroup_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 6;
            resources.ApplyResources(this.menuItem7, "menuItem7");
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 7;
            resources.ApplyResources(this.menuItem8, "menuItem8");
            this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
            // 
            // ViewRegisteredGroup
            // 
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "ViewRegisteredGroup";
            this.Load += new System.EventHandler(this.ViewRegisteredGroup_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewRegisteredGroup_Paint);
            this.Closed += new System.EventHandler(this.ViewRegisteredGroup_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewRegisteredGroup_MouseDown);
            this.ResumeLayout(false);

		}
		#endregion

		void GetGroupMainYnumSize(bool flag)
		{
			int 		i;

			getCurrentFontSize();
			getClientSize();
			if(flag == false) work.width = this.Width;
			if(work.fontY <= 0) return;			
			i = work.height;// + work.fontY*10;
			if(i < 0) i = 0;
			work.y_num = i / (work.fontY*11);
			if(work.y_num <= 0) work.y_num = 1;
		}

		void setCurrentGroupScrollPos()
		{
			work.Iy = this.AutoScrollPosition.Y;
			
			work.spos = Math.Abs(work.Iy)/gHeight;
			work.Ix = this.AutoScrollPosition.X;
		}

		private void registeredGroupSetSize()
		{			
			GetGroupMainYnumSize(false);
			if(work.bDisplayFitWindowSize == true) 
			{
				getMatchFontSize(xnum/2, work.width);
				GetGroupMainYnumSize(false);
			}
			setDefaultGroupWindowsSize();
			
			Size	size = new Size(xnum*work.fontX, (work.TagHap+1)*work.fontY*11);	// height + 1 : 위치이동이 잘 안 되어서

			
			this.AutoScrollMargin = size;
			setCurrentGroupScrollPos();
			this.AutoScroll = true;
		}

		//Bitmap bitmapClient = null;

		private void ViewRegisteredGroup_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics	gScreen = e.Graphics;

			registeredGroupSetSize();			
			if(work.bStartFlag) checkAndSetMainStartPos();
			this.Parent.Invalidate();

			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;	// 높이, 넓이가 0보가 작으면 그리지 않는다.

			//if(bitmapClient == null || ClientRectangle.Width != bitmapClient.Width || ClientRectangle.Height != bitmapClient.Height)
			//	bitmapClient = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, gScreen);
			//Graphics g = Graphics.FromImage(bitmapClient);
            Graphics g = gScreen;// OnPaintBitmap.CreateGraphics(gScreen, this.ClientRectangle.Width, this.ClientRectangle.Height);

			g.SmoothingMode = SmoothingMode.HighSpeed;
			g.CompositingMode = CompositingMode.SourceOver;
			
			AdDetailFirstScreen(g);
			if(memberArr.Count > 0) 
			{
				GroupShowOnePageDraw(g);
				GroupShowCurrentPosDraw(g);
			}
			//OnPaintBitmap.DrawImageUnscaled(gScreen, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
		}

		void GroupShowOnePageDraw(Graphics g)
		{	
			if(work.fontY <= 0) return;

			int						j, x, y, gPos, endPos;
			StringFormat			format = new StringFormat();			
			
			gPos = work.spos * 3;
			endPos = (work.y_num+work.spos+2)*3;	// 시작위치를 알 수 없으므로 2줄 더 그린다

			format.Alignment = StringAlignment.Center;

			for(; gPos < endPos; gPos++) 
			{
				if(gPos >= groupHap) break;
				x = getGroupShowStartDrawX(gPos);
				y = work.Iy + (int)(work.fontY * 0.75) + gHeight * (gPos/3);
				DrawClass.PopBox2(g, x, y, x+gRealWidth, y+gRealHeight, Color.LightGray);
		
				gr = (groupShowTagGroupMember)memberArr[gPos];
				if(gr == null) continue;
				DrawClass.WinDrawText(g, x+work.fontX, y+(int)(work.fontY*0.5), gRealWidth, work.fontY, gr.name, Color.Blue, work.backColor, work.f, format);
				for(j = 0; j < gr.tagCount; j++, y += work.fontY) 
				{
					item = (groupShowTagMember)gr.tag[j];
					if(item == null) break;
					if(j >= 8) break;
					switch(item.tagType) 
					{
						case AutoLibLocal.EnumTagType.AI : AiGroupShowCurrentValueDraw(g, item.tag, item.tagPos, x+work.fontX, y+(int)(work.fontY*1.75)); break;
						case AutoLibLocal.EnumTagType.AO : AoGroupShowCurrentValueDraw(g, item.tag, item.tagPos, x+work.fontX, y+(int)(work.fontY*1.75)); break;
						case AutoLibLocal.EnumTagType.DI : DiGroupShowCurrentValueDraw(g, item.tag, item.tagPos, x+work.fontX, y+(int)(work.fontY*1.75)); break;
						case AutoLibLocal.EnumTagType.DO : DoGroupShowCurrentValueDraw(g, item.tag, item.tagPos, x+work.fontX, y+(int)(work.fontY*1.75)); break;
						case AutoLibLocal.EnumTagType.ST : StGroupShowCurrentValueDraw(g, item.tag, item.tagPos, x+work.fontX, y+(int)(work.fontY*1.75)); break;
						default : UnKnownTagValueDraw(g, item.tag, x+work.fontX, y+(int)(work.fontY*1.75)); break;// UNKNOWN_TAG
					}
				}
			}
		}

		void tagNameDraw(Graphics g, int x, int y, string tag, Color tcolor)
		{
			StringFormat			format = new StringFormat();

			format.Alignment = StringAlignment.Near;
			DrawClass.WinDrawText(g, x, y, work.fontX*11, work.fontY, tag, tcolor, work.backColor, work.f, format);

		}

		void AiGroupShowCurrentValueDraw(Graphics g, string tagName, int[] tagPos, int x, int y)
		{
			string					buf;
			double					_base, full, imsi, width;
			int						i, j, curr, lolo, low, high, hihi;
			Color					tcolor = Color.Black;
			TagAiClass				ai;
			StringFormat			format = new StringFormat();
			
			ai = TagLib.GetStructAI(tagName, ref tagPos);
			if(ai == null) return;

			if(ai.act != 1) tcolor = SharedData.colorTotal.INACTIVE;
			tagNameDraw(g, x, y, ai.tag, tcolor);

			width = (float)(work.fontX * 12);
			x += (int)width*2;	

			DrawClass.gcls(g, x, y, x+(int)width, y+work.fontY, Color.LightGray);
			buf = TagUtil.AiValueToStringOnlyPoint(ai, ai.curr);
			format.Alignment = StringAlignment.Far;
			DrawClass.WinDrawText(g, x, y, (int)width, work.fontY, buf, tcolor, Color.LightGray, work.f, format);
			if(ai.act != 1) return;

			x -= (int)width;
			DrawClass.gcls(g, x, y+(int)(work.fontX*0.2), x+(int)width, y+(int)(work.fontY*0.8), Color.LightGray);
			_base = ai.fBase;
			full = ai.fFull - _base;
			if(full == 0.0) return;    // avoid divide by zero

			imsi = (float)(ai.curr - _base);
			curr = (int)(imsi * width / full);
			if(curr <= 1) curr = 1;
			else if(curr > width) curr = (int)width;

			imsi = ai.lolo - _base;
			lolo = (int)(imsi * (width) / full);
			if(lolo <= 1) lolo = 1;
			else if(lolo > width) lolo = (int)width;

			imsi = ai.low - _base;
			low = (int)(imsi * width / full);
			if(low <= 1) low = 1;
			else if(low > width) low = (int)width;

			imsi = ai.high - _base;
			high = (int)(imsi * width / full);
			if(high <= 1) high = 1;
			else if(high > width) high = (int)width;

			imsi = ai.hihi - _base;
			hihi = (int)(imsi * width / full);
			if(hihi <= 1) hihi = 1;
			else if(hihi > width) hihi = (int)width;

			if(curr > hihi) 
			{
				DrawClass.gcls(g, x+1, y+(int)(work.fontY*0.2), x+lolo, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOLO);
				DrawClass.gcls(g, x+lolo, y+(int)(work.fontY*0.2), x+low, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOW);
				DrawClass.gcls(g, x+low, y+(int)(work.fontY*0.2), x+high, y+(int)(work.fontY*0.8), SharedData.colorTotal.HIGH);
				DrawClass.gcls(g, x+high, y+(int)(work.fontY*0.2), x+curr, y+(int)(work.fontY*0.8), SharedData.colorTotal.HIHI);
			}
			else if(curr > high) 
			{
				DrawClass.gcls(g, x+1, y+(int)(work.fontY*0.2), x+lolo, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOLO);
				DrawClass.gcls(g, x+lolo, y+(int)(work.fontY*0.2), x+low, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOW);
				DrawClass.gcls(g, x+low, y+(int)(work.fontY*0.2), x+curr, y+(int)(work.fontY*0.8), SharedData.colorTotal.HIGH);
			}
			else if(curr > low) 
			{
				DrawClass.gcls(g, x+1, y+(int)(work.fontY*0.2), x+lolo, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOLO);
				DrawClass.gcls(g, x+lolo, y+(int)(work.fontY*0.2), x+curr, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOW);
			}
			else
				DrawClass.gcls(g, x+1, y+(int)(work.fontY*0.2), x+curr, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOLO);


			j = work.fontY/3;
			if(j < 2) j = 2;
			for(i = j/2+2; i < width-2; i+=j)
				DrawClass.gcls(g, x+i, y+(int)(work.fontY*0.2), x+i, y+(int)(work.fontY*0.8), Color.DarkGray);

			DrawClass.PushRectangle2(g, x, y+(int)(work.fontY*0.2), x+(int)width, y+(int)(work.fontY*0.8));
			DrawClass.PushRectangle2(g, x-1, y+(int)(work.fontY*0.2)-1, x+(int)width+1, y+(int)(work.fontY*0.8)+1);
		}

		void AoGroupShowCurrentValueDraw(Graphics g, string tagName, int[] tagPos, int x, int y)
		{
			string					buf;
			Color					tcolor = Color.Black;//CAT_COLOR_TEXT;
			TagAoClass				ao;
			
			ao = TagLib.GetStructAO(tagName, ref tagPos);
			if(ao == null) return;
			if(ao.act != 1) tcolor = SharedData.colorTotal.INACTIVE;
			tagNameDraw(g, x, y, ao.tag, tcolor);

			x += work.fontX*18;
			DrawClass.gcls(g, x, y, x+work.fontX*18, y+work.fontY, Color.LightGray);

			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Far;
			buf = string.Format("{0,7:f2}", ao.curr);
			DrawClass.WinDrawText(g, x, y, work.fontX*18, work.fontY, buf, tcolor, Color.LightGray, work.f, format);
		}

		void DiGroupShowCurrentValueDraw(Graphics g, string tagName, int[] tagPos, int x, int y)
		{
			Color					tcolor = Color.Black;//CAT_COLOR_TEXT;
			TagDiClass				di;
			
			di = TagLib.GetStructDI(tagName, ref tagPos);
			if(di == null) return;
			if(di.act != 1) tcolor = SharedData.colorTotal.INACTIVE;
			tagNameDraw(g, x, y, di.tag, tcolor);

			x += work.fontX * 22;
			if(di.curr == 1) tcolor = SharedData.colorTotal.ON;
			else			 tcolor = SharedData.colorTotal.OFF;
			DrawClass.PushBox2(g, x, y+work.fontX/3, x+work.fontX*2, y+work.fontY-work.fontX/3, tcolor);

			x += work.fontX*6;
			DrawClass.gcls(g, x, y, x+work.fontX*8, y+work.fontY, Color.LightGray);
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Far;
			if(di.curr == 1) DrawClass.WinDrawText(g, x, y, work.fontX*8, work.fontY, di.desON, tcolor, Color.LightGray, work.f, format);
			else			 DrawClass.WinDrawText(g, x, y, work.fontX*8, work.fontY, di.desOFF, tcolor, Color.LightGray, work.f, format);	
		}

		void DoGroupShowCurrentValueDraw(Graphics g, string tagName, int[] tagPos, int x, int y)
		{
			Color					tcolor = Color.Black;//CAT_COLOR_TEXT;
			TagDoClass				dout;
			
			dout = TagLib.GetStructDO(tagName, ref tagPos);
			if(dout == null) return;
			if(dout.act != 1) tcolor = SharedData.colorTotal.INACTIVE;			
			tagNameDraw(g, x, y, dout.tag, tcolor);

			x += work.fontX * 22;
			if(dout.curr == 1) tcolor = SharedData.colorTotal.ON;
			else			   tcolor = SharedData.colorTotal.OFF;
			DrawClass.PushBox2(g, x, y+work.fontX/3, x+work.fontX*2, y+work.fontY-work.fontX/3, tcolor);

			x += work.fontX*6;
			DrawClass.gcls(g, x, y, x+work.fontX*8, y+work.fontY, Color.LightGray);
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Far;
			if(dout.curr == 1) DrawClass.WinDrawText(g, x, y, work.fontX*8, work.fontY, dout.desON, tcolor, Color.LightGray, work.f, format);
			else			   DrawClass.WinDrawText(g, x, y, work.fontX*8, work.fontY, dout.desOFF, tcolor, Color.LightGray, work.f, format);
		}

		void StGroupShowCurrentValueDraw(Graphics g, string tagName, int[] tagPos, int x, int y)
		{
			Color					tcolor = Color.Black;
			TagStClass				st;

			st = TagLib.GetStructST(tagName, ref tagPos);
			if(st == null) return;
			if(st.act != 1) tcolor = SharedData.colorTotal.INACTIVE;			
			tagNameDraw(g, x, y, st.tag, tcolor);			
			
			x += work.fontX*18;
			DrawClass.gcls(g, x, y, x+work.fontX*18, y+work.fontY, Color.LightGray);
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Far;
			DrawClass.WinDrawText(g, x, y, work.fontX*18, work.fontY, st.curr, tcolor, Color.LightGray, work.f, format);
	
		}

		void UnKnownTagValueDraw(Graphics g, string tagName, int x, int y)
		{	
			tagNameDraw(g, x, y, tagName, Color.DarkRed);

			StringFormat			format = new StringFormat();

			format.Alignment = StringAlignment.Near;
			if(Tools.IsLangKorean())
				DrawClass.WinDrawText(g, x+work.fontX*11, y, work.fontX*15, work.fontY, "- 태그 없음", Color.Black, work.backColor, work.f, format);
			else if(Tools.IsLangJapanese())
				DrawClass.WinDrawText(g, x+work.fontX*11, y, work.fontX*15, work.fontY, "- タグない", Color.Black, work.backColor, work.f, format);
			else if(Tools.IsLangChinese())
				DrawClass.WinDrawText(g, x+work.fontX*11, y, work.fontX*15, work.fontY, "- 没有标记", Color.Black, work.backColor, work.f, format);
			else
				DrawClass.WinDrawText(g, x+work.fontX*11, y, work.fontX*15, work.fontY, "- Not Exist", Color.Black, work.backColor, work.f, format);
		}

		int getGroupShowStartDrawX(int pos)
		{
			int		x, sub = work.width - gWidth*3;			
	
			x = (sub > 0) ? work.Ix + sub/2 : work.Ix + work.fontX;
			return x + (pos % 3) * gWidth;
		}

		void GroupShowCurrentPosDraw(Graphics g)
		{
			int						x, y, width;

			if(work.pos < work.spos*3 || work.pos >= (work.spos + work.y_num + 2) * 3 || memberArr.Count <= 0) return;

			width = (int)(work.fontX*0.3);
			x = getGroupShowStartDrawX(work.pos%3) - width;
			y = work.Iy + (int)(work.fontY * 0.75) + gHeight * (int)(work.pos/3) - width;
			
			DrawClass.gcls(g, x-width, y-width, x+work.fontX*38+width*3, y, Color.LightGray);
			DrawClass.gcls(g, x-width, y+work.fontY*10+width*2, x+work.fontX*38+width*3, y+work.fontY*10+width*3, Color.LightGray);
			DrawClass.gcls(g, x-width, y, x, y+work.fontY*10+width*2, Color.LightGray);
			DrawClass.gcls(g, x+work.fontX*38+width*2, y, x+work.fontX*38+width*3, y+work.fontY*10+width*2, Color.LightGray);

			DrawClass.grectangle(g, x-width, y-width, work.fontX*38+width*4, work.fontY*10+width*4, Color.DarkGray);
			DrawClass.grectangle(g, x, y, work.fontX*38+width*2, work.fontY*10+width*2, Color.DarkGray);
		}		

		int getMousePointToPos(int x, int y)
		{	
			int		ix, iy, pos, endPos;

			pos = work.spos * 3;
			endPos = (work.y_num+work.spos+2)*3;
			
			for(; pos < endPos; pos++) 
			{
				if(pos >= groupHap) break;
				ix = getGroupShowStartDrawX(pos);
				iy = work.Iy + (int)(work.fontY * 0.75) + gHeight * (pos/3);
				if(x >= ix && x <= ix+gRealWidth && y >= iy && y <= iy+gRealHeight) return pos;//work.spos*3 + i;
			}
			return -1;
		}

		bool GetGroupMainDrawCurrentPos(ref Rectangle rect1)
		{
			int 					x, y, width;

			if(work.pos < work.spos*3 || work.pos >= (work.spos + work.y_num + 2) * 3 || memberArr.Count <= 0) return false;

			width = (int)(work.fontX*0.3);
			x = getGroupShowStartDrawX(work.pos%3) - width;
			y = work.Iy + (int)(work.fontY * 0.75) + gHeight * (int)(work.pos/3) - width;

			rect1.X = x-width;
			rect1.Y = y-width;
			rect1.Width = work.fontX*38+width*4+1;
			rect1.Height = work.fontY*10+width*4+1;
			return true;
		}

		int GroupLbuttonDownDraw(MouseEventArgs e)
		{
			int			pos;

			pos = getMousePointToPos(e.X, e.Y);
			if(work.pos == pos) return 1;			
			if(pos < 0 || pos >= memberArr.Count) return 1;

			Rectangle	rect1 = new Rectangle();
			
			if(GetGroupMainDrawCurrentPos(ref rect1)) Invalidate(rect1);
			work.pos = pos;
			if(GetGroupMainDrawCurrentPos(ref rect1)) Invalidate(rect1);				
			return 1;			
		}

		private void ViewRegisteredGroup_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right) 
			{				
				GroupLbuttonDownDraw(e);

				Point pos = new Point(e.X, e.Y);
				Control control = new Control(this, "Text");
				this.contextMenuRegisteredGroup.Show(control, pos);
				return;
			}			
			if(e.Button != MouseButtons.Left) return;
			GroupLbuttonDownDraw(e);
			if(e.Clicks >= 2) CallRegisteredGroupDetail();
		}

		void setScrollPosAddSub(bool bAddFlag, int height)
		{
			Point		p = this.AutoScrollPosition;
			int			gap;
			
			if(bAddFlag) gap = height;
			else		 gap = -height;

			p.X = Math.Abs(p.X);
			p.Y = Math.Abs(p.Y) + gap;
			this.AutoScrollPosition = p;
			//setCurrentScrollPos(height);		// delete 2004-6-29 , 버그인것 같은데...
		}
		

		public bool groupMainArrowKeyOperation(Keys key)
		{
			int				hap;
			
			switch(key)
			{
				case Keys.Up :
					if(groupHap <= 0) return true;
					hap = (work.spos+work.y_num) * 3;
					setScrollPosFitFontY(gHeight);
					if(work.pos < work.spos * 3 || work.pos >= hap) 
					{
						if(hap >= groupHap) hap = groupHap;
						work.pos = hap-1;
						Invalidate();
						return true;
					}
					if(work.pos > groupHap || work.pos <= 0) return true;
					work.pos -= 3;
					if(work.pos < 0) work.pos = 0;
					if(work.pos/3 == work.spos-1) setScrollPosAddSub(false, gHeight);
					Invalidate();
					return true;
				case Keys.Down :
					if(groupHap <= 0) return true;
					hap = (work.spos+work.y_num) * 3;
					setScrollPosFitFontY(gHeight);
					if(work.pos < work.spos*3 || work.pos >= hap) 
					{
						work.pos = work.spos*3;
						Invalidate();
						return true;
					}
					if(work.pos >= groupHap-1 || work.pos < 0) return true;
					work.pos += 3;
					if(work.pos >= groupHap) work.pos = groupHap-1;
					if(work.pos/3 == work.spos+work.y_num) setScrollPosAddSub(true, gHeight);
					Invalidate();
					return true;
				case Keys.Left :
					if(groupHap <= 0) return true;
					hap = (work.spos+work.y_num) * 3;
					setScrollPosFitFontY(gHeight);
					if(work.pos < work.spos * 3 || work.pos >= hap) 
					{
						if(hap >= groupHap) hap = groupHap;
						work.pos = hap-1;
						Invalidate();
						return true;
					}
					if(work.pos > groupHap || work.pos <= 0) return true;
					work.pos --;					
					if(work.pos/3 == work.spos-1) setScrollPosAddSub(false, gHeight);
					Invalidate();
					return true;
				case Keys.Right :
					if(groupHap <= 0) return true;
					hap = (work.spos+work.y_num) * 3;
					setScrollPosFitFontY(gHeight);
					if(work.pos < work.spos*3 || work.pos >= hap) 
					{
						work.pos = work.spos*3;
						Invalidate();
						return true;
					}
					if(work.pos >= groupHap-1 || work.pos < 0) return true;
					work.pos ++;
					if(work.pos/3 == work.spos+work.y_num) setScrollPosAddSub(true, gHeight);
					Invalidate();					
					return true;
			}
			return false;
		}

        private void OnEventTagChanged(TagPublicClass tagevent)
		{	
			if(work.fontY <= 0) return;

			TagAiClass		ai;
			TagAoClass		ao;
			TagDiClass		di;
			TagDoClass		dout;
			TagStClass		st;
			int				i, j, x, y;
			Rectangle		r;

			for(i = work.spos*3; i < (work.y_num+work.spos+2)*3; i++) // 시작위치를 알 수 없으므로 2줄 더 그린다

			{
				if(i >= groupHap) break;
				gr = (groupShowTagGroupMember)memberArr[i];
				if(gr == null) continue;

				x = getGroupShowStartDrawX(i) + work.fontX;
				y = work.Iy + (int)(work.fontY * 0.75) + gHeight * (i/3) + (int)(work.fontY*1.75);
				
				//DrawClass.PopBox2(g, x, y, x+gRealWidth, y+gRealHeight, Color.LightGray);

				for(j = 0; j < gr.tagCount; j++, y += work.fontY) 
				{
					item = (groupShowTagMember)gr.tag[j];
					if(item == null) break;
					if(j >= 8) break;					
					switch(item.tagType) 
					{
						case AutoLibLocal.EnumTagType.AI : 
							ai = TagLib.GetStructAI(item.tag, ref item.tagPos);
							if(tagevent.tag == ai.tag)
							{
								r = new Rectangle(x, y, this.gRealWidth-work.fontX, work.fontY+1);
								this.Invalidate(r);								
							}
							break;
						case AutoLibLocal.EnumTagType.AO : 
							ao = TagLib.GetStructAO(item.tag, ref item.tagPos);
							if(tagevent.tag == ao.tag)
							{
								r = new Rectangle(x, y, this.gRealWidth-work.fontX, work.fontY+1);
								this.Invalidate(r);								
							}
							break;
						case AutoLibLocal.EnumTagType.DI : 
							di = TagLib.GetStructDI(item.tag, ref item.tagPos);
							if(tagevent.tag == di.tag)
							{
								r = new Rectangle(x, y, this.gRealWidth-work.fontX, work.fontY+1);
								this.Invalidate(r);								
							}
							break;
						case AutoLibLocal.EnumTagType.DO : 
							dout = TagLib.GetStructDO(item.tag, ref item.tagPos);
							if(tagevent.tag == dout.tag)
							{
								r = new Rectangle(x, y, this.gRealWidth-work.fontX, work.fontY+1);
								this.Invalidate(r);								
							}
							break;
						case AutoLibLocal.EnumTagType.ST :
							st = TagLib.GetStructST(item.tag, ref item.tagPos);
							if(tagevent.tag == st.tag)
							{
								r = new Rectangle(x, y, this.gRealWidth-work.fontX, work.fontY+1);
								this.Invalidate(r);								
							}
							break;
						default :
							break;
					}
				}
			}
		}


		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(work.fontY <= 0) return;

			TagAiClass		ai;
			TagAoClass		ao;
			TagDiClass		di;
			TagDoClass		dout;
			TagStClass		st;
			int				i, j;
			
			for(i = work.spos*3; i < (work.y_num+work.spos+2)*3; i++) // 시작위치를 알 수 없으므로 2줄 더 그린다

			{
				if(i >= groupHap) break;
				gr = (groupShowTagGroupMember)memberArr[i];
				if(gr == null) continue;
				for(j = 0; j < gr.tagCount; j++) 
				{
					item = (groupShowTagMember)gr.tag[j];
					if(item == null) break;
					if(j >= 8) break;
					switch(item.tagType) 
					{
						case AutoLibLocal.EnumTagType.AI : 
							ai = TagLib.GetStructAI(item.tag, ref item.tagPos);
							ai.NeedDataCurr = true;
							break;
						case AutoLibLocal.EnumTagType.AO : 
							ao = TagLib.GetStructAO(item.tag, ref item.tagPos);
							ao.NeedDataCurr = true;
							break;
						case AutoLibLocal.EnumTagType.DI : 
							di = TagLib.GetStructDI(item.tag, ref item.tagPos);
							di.NeedDataCurr = true;
							break;
						case AutoLibLocal.EnumTagType.DO : 
							dout = TagLib.GetStructDO(item.tag, ref item.tagPos);
							dout.NeedDataCurr = true;
							break;
						case AutoLibLocal.EnumTagType.ST :
							st = TagLib.GetStructST(item.tag, ref item.tagPos);
							st.NeedDataCurr = true;
							break;
						default :
							break;
					}
				}
			}
		}
		

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}
		

		void setDefaultGroupWindowsSize()
		{			
			gWidth = work.fontX*40;
			gHeight = work.fontY*11;
			gRealWidth = work.fontX*38;
			gRealHeight = work.fontY*10;		
		}

		private void ViewRegisteredGroup_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListTagChanged += new SharedViewMain.OnEventTagChanged(OnEventTagChanged);
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
		}

		private void ViewRegisteredGroup_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			if(bDataChange) saveGroupShowData();
		}		

		private void OnMainFontChanged()
		{
			work.f = ConfigViewMain.fontMain;
			work.bDisplayFitWindowSize = ConfigViewMain.bFitToWindow;
			this.Invalidate();
		}

		public void CallRegisteredGroupDetail()
		{
			if(work.pos < 0 || work.pos >= groupHap) return;
			gr = (groupShowTagGroupMember)memberArr[work.pos];
			if(gr == null) return;

			ViewRegisterGroupDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewRegisterGroupDetailMain(gr), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		private void menuItem_GroupDetail_Click(object sender, System.EventArgs e)
		{
			CallRegisteredGroupDetail();
		}

		public void InsertRegisteredGroupTag(bool bInsert)
		{
			ViewRegisteredGroupDataInputDlg dialog = new ViewRegisteredGroupDataInputDlg(null);

			if(Tools.IsLangKorean()) 
			{
				if(bInsert) dialog.Text = "그룹 삽입";
				else		dialog.Text = "그룹 추가";
			}
			else if(Tools.IsLangJapanese()) 
			{
				if(bInsert) dialog.Text = "グループ挿入";
				else		dialog.Text = "グループ追加";
			}
			else if(Tools.IsLangChinese()) 
			{
				if(bInsert) dialog.Text = "插入组";
				else		dialog.Text = "添加组";
			}
			else 
			{
				if(bInsert) dialog.Text = "Add Group";
				else		dialog.Text = "Insert Group";
			}
			
			try 
			{
				dialog.textBox_GroupName.Text = string.Format("GROUP{0,3:d03}", groupHap+1);
				dialog.comboBox_TagType.SelectedIndex = 0;		// AI
				dialog.comboBox_Element.SelectedIndex = 1;		// Tag
			}
			catch{}

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				if(bInsert)	
				{
					memberArr.Insert(work.pos, dialog.grBuf);
					if(groupHap <= 0) work.pos = 0;
					else			  work.pos++;
				}
				else 
				{
					memberArr.Add(dialog.grBuf);
				}
				bDataChange = true;
				groupHap = memberArr.Count;
				work.TagHap = (groupHap + 2) / 3;
				this.Invalidate();
			}
		}

		private void menuItem_InsertGroup_Click(object sender, System.EventArgs e)
		{
			InsertRegisteredGroupTag(true);
		}

		private void menuItem_AddGroup_Click(object sender, System.EventArgs e)
		{
			InsertRegisteredGroupTag(false);
		}

		public void ModifyRegisteredGroupTag()
		{
			if(work.pos < 0 || work.pos >= groupHap) return;

			gr = (groupShowTagGroupMember)memberArr[work.pos];
			if(gr == null) return;

			ViewRegisteredGroupDataInputDlg dialog = new ViewRegisteredGroupDataInputDlg(gr);

			if(Tools.IsLangKorean()) dialog.Text = "그룹 수정";
			else if(Tools.IsLangJapanese()) dialog.Text = "グループ修正";
			else if(Tools.IsLangChinese()) dialog.Text = "修改组";
			else 				     dialog.Text = "Modify Group";
			
			try 
			{
				dialog.textBox_GroupName.Text = gr.name;
				dialog.comboBox_TagType.SelectedIndex = 0;		// AI
				dialog.comboBox_Element.SelectedIndex = 1;		// Tag
			}
			catch{}

            dialog.StartPosition = FormStartPosition.CenterParent;
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				memberArr[work.pos] = dialog.grBuf;
				bDataChange = true;
				this.Invalidate();
			}
		}

		private void menuItem_ModifyGroup_Click(object sender, System.EventArgs e)
		{
			ModifyRegisteredGroupTag();		
		}

		public void DeleteRegisteredGroupTag()
		{
			if(work.pos < 0 || work.pos >= groupHap) return;

			if(Tools.IsLangKorean()) 
			{
				if(MessageBox.Show("선택한 그룹을 삭제 할까요?", "그룹 삭제 확인", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
			}
            else if (Tools.IsLangJapanese())
            {
                if (MessageBox.Show("選択したグループを削除しますか。", "削除確認", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
            }
            else if (Tools.IsLangChinese())
            {
                if (MessageBox.Show("要删除选择的组马?", "删除组", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to delete the selected group?", "Delete Group", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
            }
			memberArr.RemoveAt(work.pos);
			groupHap = memberArr.Count;
			work.TagHap = (groupHap + 2) / 3;
			if(work.pos >= groupHap) work.pos = groupHap-1;	
			if(groupHap  <= 0) work.pos = 0;
			bDataChange = true;
			this.Invalidate();
		}

		private void menuItem_DeleteGroup_Click(object sender, System.EventArgs e)
		{
			DeleteRegisteredGroupTag();
		}

        private void menuItem8_Click(object sender, EventArgs e)
        {
            parentForm.Close();
        }
		



	}	

	[Serializable]
	public class groupShowTagMember	
	{
		public string					tag;
		public AutoLibLocal.EnumTagType	tagType;		// AI, AO, DI, DO, ST 0 ~ 4
		public int[]					tagPos = new int[1];
	};

	[Serializable]
	public class groupShowTagGroupMember 
	{
		public int						tagCount;
		public string					name;
		public ArrayList				tag;		
	};
}
