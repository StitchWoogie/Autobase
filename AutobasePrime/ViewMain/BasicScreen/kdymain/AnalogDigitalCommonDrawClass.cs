using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using NetTools;
using System.Data;
using AutoLibLocal;
using DialogControl;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;

namespace BasicScreen.kdymain
{

	

	/// <summary>
	/// Summary description for AnalogDigitalCommonDrawClass.
	/// </summary>
	public class AnalogDigitalCommonDrawClass : System.Windows.Forms.Form
	{
		public int MAX_AD_COLUMN = 10;	//define		
		public WorkStruct work;
		//TagListStruct[] tagList;
        protected Form parentForm = null;

        public AnalogDigitalCommonDrawClass()
		{
			//
			// TODO: Add constructor logic here
			//			
            //parentForm = parent;
			work = new WorkStruct();
			work.f = new Font(ConfigViewMain.fontMain.Name, ConfigViewMain.fontMain.Size);

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
		}

		public int getCurrentColumnStart(int pos)
		{
			int				i;
			float			xHap;

			if(work.xNum.maxColumn > MAX_AD_COLUMN) work.xNum.maxColumn = MAX_AD_COLUMN;
			if(pos < 0 || pos > work.xNum.maxColumn) return 0;

			for(i = 0, xHap = 0; i < pos; i++) xHap += work.xNum.xNum[i];

			return work.Ix+(int)(work.fontX*xHap);
		}


		public int getCurrentColumnWidth(int pos)
		{	
			if(work.xNum.maxColumn > MAX_AD_COLUMN) work.xNum.maxColumn = MAX_AD_COLUMN;
			if(pos < 0 || pos >= work.xNum.maxColumn) return 0;	
			return (int)(work.fontX*work.xNum.xNum[pos]);
		}

		public int getTotalColumnWidthHap()
		{
			int				i;
			float			xHap;

			if(work.xNum.maxColumn > MAX_AD_COLUMN) work.xNum.maxColumn = MAX_AD_COLUMN;
	
			for(i = 0, xHap = 0; i < work.xNum.maxColumn; i++) xHap += work.xNum.xNum[i];

			return (int)(xHap)+1;		// 전체 합 + 1
		}

		public bool checkPointDisplayReSize(int x)
		{
			int				i, px;

			if(work.xNum.maxColumn > MAX_AD_COLUMN) work.xNum.maxColumn = MAX_AD_COLUMN;

			for(i = 1; i <= work.xNum.maxColumn; i++) 
			{
				if(work.xNum.xNumReSizeFlag[i-1] == false) continue;
				px = getCurrentColumnStart(i);
				if(x >= px-(int)work.fontX && x <=px+(int)work.fontX) 
				{
					work.nCapturePos = i-1;
					work.nCaptureX = x;
					work.nCaptureMouseX = x;
					return true;
				}
			}
			return false;
		}

		public bool checkPointIsReSizeArea(int x)
		{
			int				i, px;

			if(work.xNum.maxColumn > MAX_AD_COLUMN) work.xNum.maxColumn = MAX_AD_COLUMN;

			for(i = 1; i <= work.xNum.maxColumn; i++) 
			{
				if(work.xNum.xNumReSizeFlag[i-1] == false) continue;
				px = getCurrentColumnStart(i);
				if(x >= px-work.fontX && x <=px+work.fontX) return true;		
			}
			return false;
		}

		public void AdMainFirstScreen(Graphics g)
		{
			//if(work.y_num <= 0) return;			
			DrawClass.gcls(g, getCurrentColumnStart(1), 0, work.width, work.height, SharedData.colorTotal.BACK);
		}

		public void AlarmDetailFirstScreen(Graphics g)
		{
			DrawClass.gcls(g, getCurrentColumnStart(1), 0, work.width, work.height, SharedData.alarmClass.colorAlarmBack);
		} 

		/* 
		public void AnalogDescriptionDraw(Graphics g, int mode)
		{  // mode 0 : ai, 1 : ao
			int 				x, y, width, gap;
			StringFormat		format = new StringFormat();
			
			AdMainDescriptionDraw(g);

			format.Alignment = StringAlignment.Center;
			gap = (int)(work.fontX*0.25);
			x = getCurrentColumnStart(3)+gap; 
			y = (int)(work.fontY*0.3);

			if(mode == 0)	//ANALOG_INPUT
				width = getCurrentColumnWidth(3)+getCurrentColumnWidth(4)-(int)(work.fontX*0.5);
			else
				width = getCurrentColumnWidth(3)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			if(Tools.IsLangKorean()) 
			{
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "현 재 값", Color.Blue, work.f, format);
			}
			else if(Tools.IsLangChinese()) 
			{
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "现在值", Color.Blue, work.f, format);
			}
			else 
			{
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "Current Value", Color.Blue, work.f, format);
			}
	
			if(mode == 0) 
			{		//ANALOG_INPUT
				x = getCurrentColumnStart(5)+gap;
				width = getCurrentColumnWidth(5)-(int)(work.fontX*0.5);
			}
			else 
			{
				x = getCurrentColumnStart(4)+gap;
				width = getCurrentColumnWidth(4)-(int)(work.fontX*0.5);
			}	
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean()) 
			{
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "단위", work.grayColor, work.f, format);
			}
			else 
			{
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "Unit", work.grayColor, work.f, format);
			}

			if(mode != 0) return;

			x = getCurrentColumnStart(6)+gap;
			width = getCurrentColumnWidth(6)-(int)(work.fontX*0.5);	
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			if(Tools.IsLangKorean()) 
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "자료", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "Data", work.grayColor, work.f, format);


			x = getCurrentColumnStart(7)+gap;
			width = getCurrentColumnWidth(7)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			if(Tools.IsLangKorean()) 
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "경보", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "Alarm", work.grayColor, work.f, format);
		}
		*/

		/*public void StringDescriptionDraw(Graphics g)
		{
			int 				x, width;
			StringFormat		format = new StringFormat();			
			
			AdMainDescriptionDraw(g);
			format.Alignment = StringAlignment.Center;			
			x = getCurrentColumnStart(3)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(3)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			if(Tools.IsLangKorean()) 
				DrawClass.GrayDrawText(g, x+(int)(work.fontX*0.5), (int)(work.fontY*0.3), width-(int)(work.fontX*0.25)*2, (int)work.fontY, "문 자 열", Color.Blue, work.f, format);
			else
				DrawClass.GrayDrawText(g, x+(int)(work.fontX*0.5), (int)(work.fontY*0.3), width-(int)(work.fontX*0.25)*2, (int)work.fontY, "String", Color.Blue, work.f, format);
		}*/

		/*
		public void ListAlarmDescriptionDraw(Graphics g, int mode)
		{
			int 				x, y, width;
			StringFormat		format = new StringFormat();
			
			format.Alignment = StringAlignment.Center;

			DrawClass.PopBox2(g, 0, 0, work.width-1, (int)(work.fontY*1.5), work.grayColor);

			if(work.y_num <= 0) return;
			y = (int)(work.fontY*0.3);
			x = getCurrentColumnStart(0)+(int)(work.fontX*0.25);	
			width = getCurrentColumnWidth(0)-(int)(work.fontX*0.5);

			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean()) 
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "순서", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "No", work.grayColor, work.f, format);				

			x = getCurrentColumnStart(1)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(1)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean()) 
			{
				if(mode == 0) DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "경 보 날 짜", work.grayColor, work.f, format);
				else		  DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "레벨", work.grayColor, work.f, format);
			}
			else 
			{
				if(mode == 0) DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Alarm Date", work.grayColor, work.f, format);
				else		  DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Level", work.grayColor, work.f, format);
			}
			x = getCurrentColumnStart(2)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(2)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			if(Tools.IsLangKorean()) 
			{
				if(mode == 0) DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "발생횟수", work.grayColor, work.f, format);
				else		  DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "경보날짜", work.grayColor, work.f, format);
			}
			else 
			{
				if(mode == 0) DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Count", work.grayColor, work.f, format);
				else		  DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Date", work.grayColor, work.f, format);
			}
			if(mode == 0) return;

			x = getCurrentColumnStart(3)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(3)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean()) 
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "경보시간", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Time", work.grayColor, work.f, format);
			
			x = getCurrentColumnStart(4)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(4)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean())
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "태그", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Tag", work.grayColor, work.f, format);
			
			x = getCurrentColumnStart(5)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(5)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			if(Tools.IsLangKorean())
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "태그설명", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Description", work.grayColor, work.f, format);

			x = getCurrentColumnStart(6)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(6)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			if(Tools.IsLangKorean())
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "경보내용", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Content", work.grayColor, work.f, format);
		}
		*/

		/*
		public void LogDescriptionDraw(Graphics g, int mode)
		{
			int 				x, y, width;
			StringFormat		format = new StringFormat();
			
			format.Alignment = StringAlignment.Center;

			DrawClass.PopBox2(g, 0, 0, work.width-1, (int)(work.fontY*1.5), work.grayColor);

			if(work.y_num <= 0) return;
			y = (int)(work.fontY*0.3);
			x = getCurrentColumnStart(0)+(int)(work.fontX*0.25);	
			width = getCurrentColumnWidth(0)-(int)(work.fontX*0.5);

			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean())
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "순서", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "No", work.grayColor, work.f, format);

			x = getCurrentColumnStart(1)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(1)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			if(Tools.IsLangKorean()) 
			{
				if(mode == 0) DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "로 그 날 짜", work.grayColor, work.f, format);
				else		  DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "시간", work.grayColor, work.f, format);
			}
			else 
			{
				if(mode == 0) DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Date", work.grayColor, work.f, format);
				else		  DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Time", work.grayColor, work.f, format);
			}

			x = getCurrentColumnStart(2)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(2)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean()) 
			{
				if(mode == 0) DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "파일명", work.grayColor, work.f, format);
				else		  DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "로 그 내 용", work.grayColor, work.f, format);
			}
			else 
			{
				if(mode == 0) DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Filename", work.grayColor, work.f, format);
				else		  DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Content", work.grayColor, work.f, format);
			}
		}
		*/
		
		/*
		public void AdMainDescriptionDraw(Graphics g)
		{
			int 				x, y, width;
			StringFormat		format = new StringFormat();
			
			format.Alignment = StringAlignment.Center;

			DrawClass.PopBox2(g, 0, 0, work.width-1, (int)(work.fontY*1.5), work.grayColor);

			if(work.y_num <= 0) return;
			x = getCurrentColumnStart(0)+(int)(work.fontX*0.25);	
			y = (int)(work.fontY*0.3);
			width = getCurrentColumnWidth(0)-(int)(work.fontX*0.5);

			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean()) 
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "순서", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "No", work.grayColor, work.f, format);

			x = getCurrentColumnStart(1)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(1)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean()) 
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "태  그", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Tag", work.grayColor, work.f, format);

			x = getCurrentColumnStart(2)+(int)(work.fontX*0.25);
			width = getCurrentColumnWidth(2)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean())
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "설  명", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x, y, width, (int)work.fontY, "Description", work.grayColor, work.f, format);
		}
		*/


		public void AnalogMainDescriptionDrawChild(Graphics g, int mode)
		{ // mode 0 : Ai,1 :Ao
			int 				x, gap;
			
			if(work.y_num <= 0) return;
			gap = (int)(work.fontX*0.25);
			x = getCurrentColumnStart(0);
			DrawClass.gcls(g, x, 0, x+getCurrentColumnWidth(0), work.height, work.grayColor);
			x = getCurrentColumnStart(1);
			DrawClass.PopBox2(g, x-gap, 0, x+gap, work.height, work.grayColor);
			x = getCurrentColumnStart(3);
			DrawClass.PopBox2(g, x-gap, 0, x+gap, work.height, work.grayColor);			

			if(mode == 0) x = getCurrentColumnStart(5);		//ANALOG_INPUT
			else		  x = getCurrentColumnStart(4);			
			DrawClass.PopBox2(g, x-gap, 0, x+gap, work.height, work.grayColor);			
		}

		public void DigitalMainDescriptionDrawChild(Graphics g, int mode)
		{ // mode 0 : di, 1 : do
			int 				x, gap;
			
			if(work.y_num <= 0) return;
			gap = (int)(work.fontX*0.25);
			x = getCurrentColumnStart(0);
			DrawClass.gcls(g, x, 0, x+getCurrentColumnWidth(0), work.height, work.grayColor);
			x = getCurrentColumnStart(1);
			DrawClass.PopBox2(g, x-gap, 0, x+gap, work.height, work.grayColor);
			x = getCurrentColumnStart(3);
			DrawClass.PopBox2(g, x-gap, 0, x+gap, work.height, work.grayColor);
			if(mode != 0) return;
			x = getCurrentColumnStart(4);			
			DrawClass.PopBox2(g, x-gap, 0, x+gap, work.height, work.grayColor);
		}

		/*
		public void GroupDetailDescriptionDraw(Graphics g)
		{
			AnalogDescriptionDraw(g, 0);

			int 				x, y, width, gap;
			StringFormat		format = new StringFormat();

			gap = (int)(work.fontX*0.25);
			x = getCurrentColumnStart(8)+gap;
			y = (int)(work.fontY*0.3);
			width = getCurrentColumnWidth(8)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			format.Alignment = StringAlignment.Center;
			if(Tools.IsLangKorean()) 
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "종류", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, (int)work.fontY, "Type", work.grayColor, work.f, format);			
		}
		*/


		public void AdMainDrawCurrentPos(Graphics g)
		{
			int 					x, y, width, gap;
			
			if(work.y_num <= 0) return;
			
			gap = (int)(work.fontX*0.5);
			x = getCurrentColumnStart(0)+gap;
			y = work.Iy + work.pos*(int)(work.fontY);
			width = getCurrentColumnWidth(0)-(int)(work.fontX);
			DrawClass.gnot(g, x, y, x+width, y+(int)work.fontY, Color.Black, 50);

			//x = getCurrentColumnStart(2)+gap;
			//width = getCurrentColumnWidth(2)-(int)(work.fontX);
			//DrawClass.gnot(g, x, y, x+width, y+(int)work.fontY, Color.Black, 50);		
		}

		public bool GetAdMainDrawCurrentPos(ref Rectangle rect1, ref Rectangle rect2)
		{
			int 					x, y, width, gap;

			if(work.y_num <= 0) return false;

			gap = (int)(work.fontX*0.5);
			x = getCurrentColumnStart(0)+gap;
			y = work.Iy + work.pos*(int)work.fontY;
			width = getCurrentColumnWidth(0)-(int)(work.fontX);
			rect1.X = x;
			rect1.Y = y;
			rect1.Width = width+1;
			rect1.Height = (int)work.fontY+1;

			x = getCurrentColumnStart(2)+gap;
			width = getCurrentColumnWidth(2)-(int)(work.fontX);
			rect2.X = x;
			rect2.Y = y;
			rect2.Width = width+1;
			rect2.Height = (int)work.fontY+1;
			return true;			
		}

		public void AdDisplayCaptureLineDraw(Graphics g)
		{
			int		y2;
			
			y2 = (int)(work.fontY*1.5);
			DrawClass.gline(g, work.nCaptureMouseX, 1, work.nCaptureMouseX, y2, Color.Black);
			//DrawClass.gnot(g, x1, y1, x2, y2, Color.Black, 25);
		}

		public void AdDisplayCaptureLineInvalidate()
		{
			Rectangle	r = new Rectangle(work.nCaptureMouseX, 1, 1, (int)(work.fontY*1.5));
			
			this.Invalidate(r);			
		}

		public bool AdWmLButtonUp(int x)
		{
			float					num;
					
			if(!work.bMouseCapture) return false;
	
			Cursor = Cursors.Arrow;
			Graphics g = CreateGraphics();

			AdDisplayCaptureLineInvalidate();
			//AdDisplayCaptureLineDraw(g);
			work.bMouseCapture = false;
			//ReleaseCapture();
			
			if(x == work.nCaptureX) return false;
	
			work.bConfigChange = true;
			if(x > work.nCaptureX) 
			{
				work.xNum.xNum[work.nCapturePos%10] += (x-work.nCaptureX)/work.fontX;
				if(work.xNum.xNum[work.nCapturePos%10] > 200.0) work.xNum.xNum[work.nCapturePos%10] = 200;
				return true;
			}
	
			num = work.xNum.xNum[work.nCapturePos%10]-(work.nCaptureX-x)/work.fontX;
			if(num <= 4) 
			{		// 4글자 이상은 되어야한다.
				work.xNum.xNum[work.nCapturePos%10] = 4;	
				return true;
			}
			work.xNum.xNum[work.nCapturePos%10] = num;	
			return true;	
		}

		public void AdDetailFirstScreen(Graphics g)
		{
			//DrawClass.gcls(g, getCurrentColumnStart(1), 0, work.width, work.height, SharedData.colorTotal.BACK);
			DrawClass.gcls(g, getCurrentColumnStart(1), 0, this.ClientRectangle.Width, this.ClientRectangle.Height, SharedData.colorTotal.BACK);

		}

		public void AdWmMouseMove(int x, int y)
		{
			if(work.bMouseCapture == false && y < (int)(work.fontY*1.5)) 
			{
				if(checkPointIsReSizeArea(x)) Cursor = Cursors.SizeWE;
				else Cursor = Cursors.Arrow;
				return;
			}
			if(!work.bMouseCapture) 
			{
				Cursor = Cursors.Arrow;				
				return;
			}
			if(work.nCaptureMouseX == x) return;
		
			Cursor = Cursors.SizeWE;
			
			AdDisplayCaptureLineInvalidate();
			work.nCaptureMouseX = x;
			AdDisplayCaptureLineInvalidate();			
		}

		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalogDigitalCommonDrawClass));
            this.SuspendLayout();
            // 
            // AnalogDigitalCommonDrawClass
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Icon = null;
            this.Name = "AnalogDigitalCommonDrawClass";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

		}	

		public void AiUpLinePosDraw(Graphics g, int fontX, int fontY, int x, int y, int height, double max, double min, string sFormat, float fFormat, char cFormat)
		{
			int					i;
			double				val;
			String				buf;
			StringFormat		format = new StringFormat();

			DrawClass.gline(g, x, y, x, y+height+1, Color.White);
			for(i = 0; i < 20; i+=2)
			{
				DrawClass.gline(g, x-fontX*2, y+i*height/20, x,  y+i*height/20, Color.White);
				DrawClass.gline(g, x-(int)(fontX*1.5), y+(i+1)*height/20, x, y+(i+1)*height/20, Color.White);
			}
			DrawClass.gline(g, x-fontX*2, y+height, x, y+height, Color.White);

			format.Alignment = StringAlignment.Far;
			for(i = 0; i < 6; i++)
			{
				val = GetDisplayValue(max-(i*(max-min)/5));

				/*
				buf = String.Format("{0,3:f2}", val);
				if(buf.Length >= 9) 
				{
					buf = String.Format("{0,9:E3}", val);
					DrawClass.GrayDrawText(g, x-(int)(fontX*12.3), y-fontY/2+i*height/5, (int)(fontX*10.5), fontY, buf, work.grayColor, work.f, format);
				}
				else DrawClass.GrayDrawText(g, x-(int)(fontX*12), y-fontY/2+i*height/5, (int)(fontX*9.5), fontY, buf, work.grayColor, work.f, format);
				*/
				buf = TagUtil.AiValueToStringOnlyPoint(sFormat, fFormat, cFormat, val);
				DrawClass.GrayDrawText(g, x-(int)(fontX*12), y-fontY/2+i*height/5, (int)(fontX*9.5), fontY, buf, work.grayColor, work.f, format);
			}
		}

		public void DownLinePosDraw(Graphics g, int fontX, int fontY, int x, int y, int width, int count)
		{
			int					i;
			
			DrawClass.gline(g, x, y, x+width, y, Color.DarkGray);
			for(i = 0; i < count+1; i++) 
			{
				if((i % 4) == 0) 
				{
					DrawClass.gline(g, (int)(x+(long)i*width/count), y, (int)(x+(long)i*width/count), y+(int)(fontY*0.6), Color.DarkGray);
					continue;
				}
				if(( i % 2) == 0) 
				{
					DrawClass.gline(g, (int)(x+(long)i*width/count), y, (int)(x+(long)i*width/count), y+(int)(fontY*0.4), Color.DarkGray);
					continue;
				}
				DrawClass.gline(g, (int)(x+(long)i*width/count), y, (int)(x+(long)i*width/count), y+(int)(fontY*0.3), Color.DarkGray);
			}
		}		

		public void getCurrentFontSize()
		{
			work.fontY = (int)work.f.GetHeight();
			if(work.fontY < 2) work.fontY = 2;
			work.fontX = work.fontY/2;
		}

		public void TagDescEtcDraw(Graphics g, int x, int y, int width, String tag, String buf, Color tColor, Color bColor)
		{
			StringFormat				format = new StringFormat();

			format.Alignment = StringAlignment.Far;
			DrawClass.GrayDrawText(g, x, y+1, work.fontX*7, work.fontY, tag,  tColor, work.f, format);
			DrawClass.PushBox2(g, x+(int)(work.fontX*7.5), y, x+width+(int)(work.fontX*8.5), y+work.fontY, bColor);
			format.Alignment = StringAlignment.Near;
			DrawClass.WinDrawText(g, x+(int)(work.fontX*8), y+1, width, work.fontY, buf, tColor, bColor, work.f, format);
		}


		public void DrawTrendEdgePointDisplay(Graphics g, int fontX, int x1, int y1, eTrendEdgeType mode, Color color)
		{
			Point[]				points;
			int					size = fontX*2/7;
			Brush				br = new SolidBrush(color);
			
			switch(mode) 
			{
				case eTrendEdgeType.RECT :   // rectangle
					size = fontX*4/15;
					DrawClass.gcls(g, x1-size, y1-size, x1+size, y1+size, color);
					break;
				case eTrendEdgeType.CIRCLE :   // fill circle
					//size = fontX*3/8;
					Pen				pen = new Pen(br);		//정원이 안되서 fill 후 draw
					g.FillEllipse(br, x1-size, y1-size, size*2, size*2);
					g.DrawEllipse(pen, x1-size, y1-size, size*2, size*2);
					break;
				case eTrendEdgeType.TRI :   // 삼각형
					size = fontX*3/7;
					points = new Point[3];
					points[0].X = x1-size;
					points[0].Y = y1+size;
					points[1].X = x1+size;
					points[1].Y = y1+size;
					points[2].X = x1;  // ? +1
					points[2].Y = y1-size;					
					g.FillPolygon(br, points);
					break;
				case eTrendEdgeType.DIAMOND :   // 마름모
					size = fontX*3/7;
					points = new Point[4];
					points[0].X = x1-size;
					points[0].Y = y1;
					points[1].X = x1;
					points[1].Y = y1-size;
					points[2].X = x1+size;
					points[2].Y = y1;
					points[3].X = x1;
					points[3].Y = y1+size;
					g.FillPolygon(br, points);
					break;
				case eTrendEdgeType.X_CHAR :   // x
					DrawClass.gline(g, x1-size, y1-size, x1+size, y1+size, color);
					DrawClass.gline(g, x1-size, y1+size, x1+size, y1-size, color);
					break;
				case eTrendEdgeType.STAR :   // *
					DrawClass.gline(g, x1-size, y1-size, x1+size, y1+size, color);
					DrawClass.gline(g, x1-size, y1+size, x1+size, y1-size, color);
					DrawClass.gline(g, x1,   y1+size, x1,   y1-size, color);
					break;				
			}
		}

		public void AiTrendGraphStatusLineDraw(Graphics g, TagAiClass ai, int y, double full, double _base, double yzone)
		{
			double					f, imsi;
            double gab = (full - _base);

            if (gab <= 0) return;
			
			imsi = TagUtil.GetDisplayValue(ai, ai.hihi);
			if(imsi <= full && imsi >= _base) 
			{
				if(full == 0)
					f = 0;
				else
                    f = (imsi - _base) / gab * yzone;// work.fontY * 17;
				DrawClass.gline(g, work.Ix+work.fontX*16, y-(int)f, work.Ix+(int)(work.fontX*76.1), y-(int)f, SharedData.colorTotal.HIHI);
			}
            imsi = TagUtil.GetDisplayValue(ai, ai.high);
			if(imsi <= full && imsi >= _base) 
			{
				if(full == 0)
					f = 0;
				else
                    f = (imsi - _base) / gab * yzone;//work.fontY*17;				
				DrawClass.gline(g, work.Ix+work.fontX*16, y-(int)f, work.Ix+(int)(work.fontX*76.1), y-(int)f, SharedData.colorTotal.HIGH);
			}
            imsi = TagUtil.GetDisplayValue(ai, ai.low);
			if(imsi <= full && imsi >= _base) 
			{
				if(full == 0)
					f = 0;
				else
                    f = (imsi - _base) / gab * yzone;//work.fontY*17;
				DrawClass.gline(g, work.Ix+work.fontX*16, y-(int)f, work.Ix+(int)(work.fontX*76.1), y-(int)f, SharedData.colorTotal.LOW);
			}
            imsi = TagUtil.GetDisplayValue(ai, ai.lolo);
			if(imsi <= full && imsi >= _base) 
			{
				if(full == 0)
					f = 0;
				else
                    f = (imsi - _base) / gab * yzone;//work.fontY*17;				
				DrawClass.gline(g, work.Ix+work.fontX*16, y-(int)f, work.Ix+(int)(work.fontX*76.1), y-(int)f, SharedData.colorTotal.LOLO);
			}
		}


		/*
		public void DigitalDescriptionDraw(Graphics g, int mode)
		{  // mode 0 : di, 1 :do, 10 : string
			int 				x, y, width, gap;
			StringFormat		format = new StringFormat();

			if(work.y_num <= 0) return;
			gap = (int)(work.fontX*0.25);
			y = (int)(work.fontY*0.3);

			AdMainDescriptionDraw(g);

			x = getCurrentColumnStart(3)+gap;	
			width = getCurrentColumnWidth(3)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);

			format.Alignment = StringAlignment.Center;
			switch(mode) 
			{
				case 0 :
				case 1 :
					if(Tools.IsLangKorean())
						DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, work.fontY, "현 재 값", Color.Blue, work.f, format);
					else
						DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, work.fontY, "Value", Color.Blue, work.f, format);
					break;
				case 10 :
					if(Tools.IsLangKorean())
						DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, work.fontY, "문 자 열", Color.Blue, work.f, format);
					else
						DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, work.fontY, "String", Color.Blue, work.f, format);
					break;
			}
			if(mode != 0) return;

			x = getCurrentColumnStart(4)+gap;
			width = getCurrentColumnWidth(4)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x+gap, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean())
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, work.fontY, "자료", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, work.fontY, "Data", work.grayColor, work.f, format);

			x = getCurrentColumnStart(5)+gap;
			width = getCurrentColumnWidth(5)-(int)(work.fontX*0.5);
			DrawClass.PushBox2(g, x, (int)(work.fontY*0.15), x+width, (int)(work.fontY*1.35), work.grayColor);
			if(Tools.IsLangKorean())
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, work.fontY, "경보", work.grayColor, work.f, format);
			else
				DrawClass.GrayDrawText(g, x+gap, y, width-gap*2, work.fontY, "Alarm", work.grayColor, work.f, format);
		}
		*/

		public void setCurrentScrollPos(int height)
		{
			//work.hpos = this.AutoScrollPosition.X;
			work.Iy = this.AutoScrollPosition.Y;
			
			work.spos = Math.Abs(work.Iy)/height;
			if(work.spos >= work.TagHap) work.spos = (work.TagHap-1 > 0) ? work.TagHap-1 : 0;
			work.Ix = this.AutoScrollPosition.X;//work.hpos;
		}		

		public void setMainWindowScrollPosChange(int x, int y)
		{
			Point		p = this.AutoScrollPosition;

			p.X = Math.Abs(p.X)+x*work.fontX;
			p.Y = Math.Abs(p.Y)+y*work.fontY;
			this.AutoScrollPosition = p;
			setCurrentScrollPos(work.fontY);
		}

		public void checkAndSetMainStartPos()
		{
			if(work.pos >= work.TagHap) work.pos = 0;
			if(work.pos >= work.y_num) setMainWindowScrollPosChange(0, work.pos);				
			work.bStartFlag = false;
		}

		public void setScrollPosFitFontY(int height)
		{
			Point		p = this.AutoScrollPosition;
			int			gap;
			
			gap = Math.Abs(p.Y) % height;
			if(gap == 0) return;

			if(gap < height/2) gap = -gap;
			else			   gap = height-gap;
			p.X = Math.Abs(p.X);
			p.Y = Math.Abs(p.Y) + gap;
			this.AutoScrollPosition = p;
			setCurrentScrollPos(height);
		}

		public void setScrollPosY()
		{
			Point		p = this.AutoScrollPosition;
			
			p.X = Math.Abs(p.X);
			p.Y = Math.Abs(work.pos*work.fontY);
			this.AutoScrollPosition = p;			
		}
		

		public bool mainArrowKeyOperation(Keys key)
		{
			if(work.y_num <= 0 || work.TagHap <= 0) return true;

			int				hap;
			
			switch(key)
			{
				case Keys.Up :
					hap = work.spos+work.y_num;
					setScrollPosFitFontY(work.fontY);
					if(work.pos < work.spos || work.pos >= hap) 
					{
						if(hap >= work.TagHap) hap = work.TagHap;
						work.pos = hap-1;
						Invalidate();
						return true;
					}
					if(work.pos > work.TagHap || work.pos <= 0) return true;
					work.pos--;
					if(work.pos == work.spos-1) setScrollPosY();
					Invalidate();
					return true;
				case Keys.Down :
					hap = work.spos+work.y_num;
					setScrollPosFitFontY(work.fontY);
					if(work.pos < work.spos || work.pos >= hap) 
					{
						work.pos = work.spos;
						Invalidate();
						return true;
					}
					if(work.pos >= work.TagHap-1 || work.pos < 0) return true;
					work.pos++;
					if(work.pos == work.spos+work.y_num) setScrollPosY();
					Invalidate();
					return true;
				case Keys.PageUp :
					hap = work.spos+work.y_num;
					setScrollPosFitFontY(work.fontY);
					if(work.pos < work.spos || work.pos >= hap)	// 현재위치가 화면표시밖에 있다
					{
						if(hap >= work.TagHap) work.pos = work.TagHap-1;
						else work.pos = hap-1;
						Invalidate();
						return true;
					}

					if(work.pos > work.TagHap || work.pos <= 0) return true;
					if(work.pos < work.y_num)					// 0번위치로 현재위치를 보낸다.
					{
						work.pos = 0;
						if(work.spos > 0) setScrollPosY();
						Invalidate();
						return true;
					}
					work.pos -= work.y_num;
					setScrollPosY();
					Invalidate();
					return true;
				case Keys.PageDown :
					hap = work.spos+work.y_num;
					setScrollPosFitFontY(work.fontY);
					if(work.pos < work.spos || work.pos >= hap)	 // 현재위치가 화면표시밖에 있다 
					{
						work.pos = work.spos;
						Invalidate();
						return true;
					}
					if(work.pos >= work.TagHap-1 || work.pos < 0) return true;
					hap = work.pos;
					if(work.pos+work.y_num >= work.TagHap-1) work.pos = work.TagHap-1;// 마지막 태그로 현재위치를 보낸다.
					else work.pos += work.y_num;					
					setScrollPosY();
					Invalidate();
					return true;
				case Keys.Left :
					setMainWindowScrollPosChange(-1, 0);					
					return true;
				case Keys.Right :
					setMainWindowScrollPosChange(1, 0);					
					return true;
			}
			return false;
		}

		public void setCurrentScrollPosDetail()
		{			
			work.Ix = this.AutoScrollPosition.X;
			//work.hpos = Math.Abs(work.Ix);
			work.Iy = this.AutoScrollPosition.Y;
			//work.vpos = Math.Abs(work.Iy);
		}

		public bool getValidPrevTagPos()
		{
			if(work.TagHap <= 0 || work.trend_tag_hap > 1) return false;	// 태그가 없거나 멀티 트랜드이다.
			int						pos = work.pos;
			TagPublicClass			tag;

			while(true)
			{
				if(pos <= 0) pos = work.TagHap-1;
				else pos--;
				if(pos == work.pos) return false;		// 동일한번호 = 모든 태그를 검색했으나 Active된 태그가 없다
				tag = TagLib.GetStructPublic(work.tagList[pos]);
				if(tag == null || tag.act == 0) continue;
				work.pos = pos;
				return true;
			}		
		}

		public bool getValidNextTagPos()
		{
			if(work.TagHap <= 0) return false;
			int						pos = work.pos;
			TagPublicClass			tag;

			while(true)
			{
				if(pos >= work.TagHap-1) pos = 0;
				else pos++;
				if(pos == work.pos) return false;		// 동일한번호 = 모든 태그를 검색했으나 Active된 태그가 없다
				tag = TagLib.GetStructPublic(work.tagList[pos]);
				if(tag == null || tag.act == 0) continue;
				work.pos = pos;
				return true;
			}
		}

		
		public bool detailArrowKeyOperation(Keys key)
		{
			switch(key)
			{
				case Keys.Up :
					setDetailWindowScrollPosChange(0, -1);
					return true;
				case Keys.Down :
					setDetailWindowScrollPosChange(0, 1);
					return true;
				case Keys.PageUp :
					if(getValidPrevTagPos() == false) return true;	// 유효한 태그가 없다
					work.bTagChanged = true;						// 태그가 바뀌었다
					Invalidate();
					return true;
				case Keys.PageDown :
					if(getValidNextTagPos() == false) return true;	// 유효한 태그가 없다
					work.bTagChanged = true;						// 태그가 바뀌었다
					Invalidate();
					return true;
				case Keys.Left :
					setDetailWindowScrollPosChange(-1, 0);
					return true;
				case Keys.Right :
					setDetailWindowScrollPosChange(1, 0);
					return true;
			}
			return false;
		}

		public void setDetailWindowScrollPosChange(int x, int y)
		{
			Point		p = this.AutoScrollPosition;

			p.X = Math.Abs(p.X)+x*work.fontX;
			p.Y = Math.Abs(p.Y)+y*work.fontY;
			this.AutoScrollPosition = p;
			setCurrentScrollPosDetail();
		}

		public void getMatchFontSize(float num, int hap)
		{
			int		i, val, fSize = 12;

			work.f = new Font(ConfigViewMain.fontMain.Name, fSize);
			val = (int)(work.f.Height*num);
			if(val > hap) 
			{
				for(i = 0; i < 100; i++) 
				{
					fSize--;
					if(fSize <= 0) 
					{
						fSize = 1;
						work.f = new Font(ConfigViewMain.fontMain.Name, fSize);
						break;
					}
					work.f = new Font(ConfigViewMain.fontMain.Name, fSize);
					val = (int)(work.f.Height*num);
					if(val <= hap) break;
				}
			}
			else 
			{
				for(i = 0; i < 100; i++) 
				{
					fSize++;
					if(fSize > 100) 
					{
						work.f = new Font(ConfigViewMain.fontMain.Name, fSize);
						break;
					}
					work.f = new Font(ConfigViewMain.fontMain.Name, fSize);
					val = (int)(work.f.Height*num);
					if(val > hap) 
					{
						fSize--;
						work.f = new Font(ConfigViewMain.fontMain.Name, fSize);
						break;
					}
				}
			}
			
		}

		public void DetailWindowSetSize(int xhap, int yhap)
		{			
			getClientSize();
			if(work.bDisplayFitWindowSize == true) 
			{				
				this.AutoScroll = false;
				if((float)(work.width*2/xhap) >= (float)(work.height/yhap)) getMatchFontSize(yhap, work.height);				
				else									  getMatchFontSize(xhap/2, work.width);				
			}
			else this.AutoScroll = true;

			getCurrentFontSize();			
			Size	size = new Size(xhap*work.fontX, yhap*work.fontY);
			this.AutoScrollMargin = size;
			setCurrentScrollPosDetail();
		}

		public int AdLbuttonDown(MouseEventArgs e)
		{
			int			pos;
			Rectangle	rect1 = new Rectangle();
			Rectangle	rect2 = new Rectangle();

			if(work.fontY <= 0) return 1;
			pos = (Math.Abs(work.Iy) + e.Y)/work.fontY;
			if(work.pos == pos || pos < 0 || pos >= work.TagHap) return 1;
			
			//AdMainDrawCurrentPos(g);
			if(GetAdMainDrawCurrentPos(ref rect1, ref rect2)) 
			{
				Invalidate(rect1);
				Invalidate(rect2);
			}
			work.pos = pos;
			if(GetAdMainDrawCurrentPos(ref rect1, ref rect2)) 
			{
				Invalidate(rect1);
				Invalidate(rect2);
			}
			//AdMainDrawCurrentPos(g);				
			return 1;			
		}


		public void AdInitDisplayCurrentColumnWidthAI()
		{
			int			i;

			work.bConfigChange = false;
			work.xNum.maxColumn = 9;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 7;		// no
			work.xNum.xNum[1] = 20;		// tag
			work.xNum.xNum[2] = 40;		// dec
			work.xNum.xNum[3] = 19;		// curr	원래는 33 하나
			work.xNum.xNum[4] = 14;		// curr
			work.xNum.xNum[5] = 6.5f;		// unit
			work.xNum.xNum[6] = 6;		// data
			work.xNum.xNum[7] = 6;		// alarm
			work.xNum.xNum[8] = 1;		// protect 1

			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;

			DisplayColumnWidthLoad("AI_SIZE");
		}

		void DisplayColumnWidthLoad(string name)
		{
			string path = ConfigVarTotal.sDirConfigUser+"\\BasicScreen";
			string filename = path+"\\"+name;
			
			if(File.Exists(filename)) 
			{
				Stream s = File.OpenRead(filename);
				//BinaryFormatter format = new BinaryFormatter();
				IFormatter format = new SoapFormatter();

				try 
				{
					work.xNum = (MAIN_DRAW_POS)format.Deserialize(s);
				}
				catch 
				{

				}
				s.Close();
			}
		}

		public void DisplayColumnWidthSave(string name)
		{
			string path = ConfigVarTotal.sDirConfigUser+"\\BasicScreen";
			Directory.CreateDirectory(path);

			string filename = path+"\\"+name;
			
			Stream s = File.OpenWrite(filename);
			s.Flush();
			SoapFormatter format = new SoapFormatter();
			format.Serialize(s, work.xNum);
			s.Close();
		}

		public void AdInitDisplayCurrentColumnWidthAO()
		{
			int			i;
			
			work.bConfigChange = false;
			work.xNum.maxColumn = 5;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 7;		// no
			work.xNum.xNum[1] = 20;		// tag
			work.xNum.xNum[2] = 40;		// dec
			work.xNum.xNum[3] = 33;		// curr	원래는 33 하나
			work.xNum.xNum[4] = 6.5f;	// unit
			
			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;

			DisplayColumnWidthLoad("AO_SIZE");
		}

		public void AdInitDisplayCurrentColumnWidthDI()
		{
			int			i;

			work.bConfigChange = false;
			work.xNum.maxColumn = 7;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 7;		// no
			work.xNum.xNum[1] = 20;		// tag
			work.xNum.xNum[2] = 40;		// dec
			work.xNum.xNum[3] = 33;		// curr
			work.xNum.xNum[4] = 6;		// data
			work.xNum.xNum[5] = 6;		// alarm
			work.xNum.xNum[6] = 1;		// protect 1

			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;
			
			DisplayColumnWidthLoad("DI_SIZE");
		}

		public void AdInitDisplayCurrentColumnWidthDO()
		{
			int			i;

			work.bConfigChange = false;
			work.xNum.maxColumn = 4;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 7;		// no
			work.xNum.xNum[1] = 20;		// tag
			work.xNum.xNum[2] = 40;		// dec
			work.xNum.xNum[3] = 33;		// curr
			
			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;
			
			DisplayColumnWidthLoad("DO_SIZE");
		}

		public void AdInitDisplayCurrentColumnWidthST()
		{
			int			i;

			work.bConfigChange = false;
			work.xNum.maxColumn = 4;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 7;		// no
			work.xNum.xNum[1] = 20;		// tag
			work.xNum.xNum[2] = 40;		// dec
			work.xNum.xNum[3] = 40;		// curr
			
			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;
			
			DisplayColumnWidthLoad("ST_SIZE");
		}

		public void AdInitDisplayCurrentColumnWidthAlarm()
		{
			int			i;

			work.bConfigChange = false;
			work.xNum.maxColumn = 3;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 7;		// no
			work.xNum.xNum[1] = 50;		// Alarm Date
			work.xNum.xNum[2] = 15;		// Alarm Count	
			
			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;
			
			DisplayColumnWidthLoad("ALARM_MAIN_SIZE");
		}

		public void AdInitDisplayCurrentColumnWidthAlarmDetail()
		{
			int			i;

			work.bConfigChange = false;
			work.xNum.maxColumn = 7;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 7;		// no
			work.xNum.xNum[1] = 6;		// level
			work.xNum.xNum[2] = 12;		// date
			work.xNum.xNum[3] = 12;		// Time
			work.xNum.xNum[4] = 20;		// Tag
			work.xNum.xNum[5] = 30;		// Desc
			work.xNum.xNum[6] = 30;		// Content
			
			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;
			
			DisplayColumnWidthLoad("ALARM_DETAIL_SIZE");
		}

		public void AdInitDisplayCurrentColumnWidthLog()
		{
			int			i;

			work.bConfigChange = false;
			work.xNum.maxColumn = 3;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 7;		// no
			work.xNum.xNum[1] = 30;		// Log Date
			work.xNum.xNum[2] = 20;		// Etc
			
			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;
			
			DisplayColumnWidthLoad("LOG_MAIN_SIZE");
		}

		public void AdInitDisplayCurrentColumnWidthLogDetail()
		{
			int			i;

			work.bConfigChange = false;
			work.xNum.maxColumn = 3;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 7;		// no
			work.xNum.xNum[1] = 10;		// Time
			work.xNum.xNum[2] = 55;		// 내용
			
			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;
			
			DisplayColumnWidthLoad("LOG_DETAIL_SIZE");
		}


		public void AdInitDisplayCurrentColumnWidthGroupDetail()
		{
			int			i;

			work.bConfigChange = false;
			work.xNum.maxColumn = 9;
			work.xNum.xNum = new float[MAX_AD_COLUMN];
			work.xNum.xNumReSizeFlag = new bool[MAX_AD_COLUMN];
			work.xNum.xNum[0] = 6;		// no
			work.xNum.xNum[1] = 20;		// tag
			work.xNum.xNum[2] = 30;		// dec
			work.xNum.xNum[3] = 15;		// curr	원래는 33 하나
			work.xNum.xNum[4] = 12;		// curr
			work.xNum.xNum[5] = 6.5f;	// unit
			work.xNum.xNum[6] = 6;		// data
			work.xNum.xNum[7] = 6;		// alarm
			work.xNum.xNum[8] = 8;		// protect 1

			for(i = 0; i < work.xNum.maxColumn; i++) work.xNum.xNumReSizeFlag[i] = true;
			for( ;i < MAX_AD_COLUMN; i++) work.xNum.xNumReSizeFlag[i] = false;

			DisplayColumnWidthLoad("GS_SIZE");
		}
		
		public void GetMainYnumSize(bool flag)
		{
			int 		i;

			getCurrentFontSize();
			getClientSize();
			if(flag == false) work.width = this.Width;
			if(work.fontY <= 0) return;			
			i = work.height;
			if(i < 0) i = 0;
			work.y_num = i / work.fontY;
			if((i % work.fontY) > work.fontY*3/4) work.y_num++;
		}

		
		public void getClientSize()
		{
			work.width = ClientSize.Width;
			work.height = ClientSize.Height;
		}

		public double GetDisplayValue(double val)
		{
			return val;
		}

		public int getplusday(int year, int mon, int day, int day_hap)
		{
			int 			i;

			if(day_hap <= 0) return day;
			for(i = 0; i < day_hap; i++)
				TimeUtil.plusOneDay(ref year, ref mon, ref day);
			return day;
		}

		void getSelectedAiTagName()
		{
			TagListStruct[] list = TagLib.GetTagList(EnumTagType.AI);
			if(list == null) work.tag = "";
			else			 work.tag = list[work.pos%work.TagHap].tag;			
		}

		void getSelectedDiTagName()
		{
			TagListStruct[] list = TagLib.GetTagList(EnumTagType.DI);
			if(list == null) work.tag = "";
			else			 work.tag = list[work.pos%work.TagHap].tag;			
		}

		public void tagActiveEnableDisable()
		{
			TagPublicClass tag = TagLib.GetStructPublic(work.tagList[work.pos]);
			tag.act = ((int)tag.act == 1) ? (sbyte)0 : (sbyte)1;
			this.Invalidate();
		}

		public void CallAiSettingDialog()
		{
			if(work.TagHap <= 0) return;
			TagAiClass	ai = TagLib.GetStructAI(work.tagList[work.pos]);
			DialogControl.ControlBoxAnalogInputGo dialog = new DialogControl.ControlBoxAnalogInputGo();
			dialog.Go(this, ai.tag);
		}

		public void CallTagPropertyWindows()
		{
			if(work.TagHap <= 0) return;
			TagPublicClass tag = TagLib.GetStructPublic(work.tagList[work.pos]);
			CallTagPropertyWindows(tag);
		}

		public void CallTagPropertyWindows(TagPublicClass tp)
		{
			if(DialogTag.TagEditor.Editor.ByViewMain(tp))
				this.Invalidate();
		}

		/*public void CallAoTagPropertyWindows()
		{
			if(work.TagHap <= 0) return;
			TagAoClass			ao = TagLib.GetStructAO(work.tagList[work.pos].tag, ref work.tagList[work.pos].tag_pos);
			if(DialogTag.TagEditor.Editor.ByViewMain(ao))
				this.Invalidate();
		}*/

		public void CallAnalogOutputDialog()
		{
			if(work.TagHap <= 0) return;

			DialogControl.ControlBoxAnalogOutputGo dialog = new DialogControl.ControlBoxAnalogOutputGo();
			dialog.Go(this, work.tagList[work.pos].tag);
		}

		/*public void CallDiTagPropertyWindows()
		{
			if(work.TagHap <= 0) return;
			TagDiClass di = TagLib.GetStructDI(work.tagList[work.pos].tag, ref work.tagList[work.pos].tag_pos);
			if(DialogTag.TagEditor.Editor.ByViewMain(di))
				this.Invalidate();
		}

		public void CallDoTagPropertyWindows()
		{
			if(work.TagHap <= 0) return;
			TagDoClass dout = TagLib.GetStructDO(work.tagList[work.pos].tag, ref work.tagList[work.pos].tag_pos);
			if(DialogTag.TagEditor.Editor.ByViewMain(dout))
				this.Invalidate();
		}*/

		public void callAiHandInputDialog()
		{
			if(work.TagHap <= 0) return;

			TagAiClass ai = TagLib.GetStructAI(work.tagList[work.pos]);
			ViewAnalogInputHandInputDlg dialog = new ViewAnalogInputHandInputDlg(ai);
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
			if(dialog.bElementChanged) this.Invalidate();		// 값이 변경되었다.
		}

		public void callDiHandInputDialog()
		{
			if(work.TagHap <= 0) return;

			TagDiClass di = TagLib.GetStructDI(work.tagList[work.pos]);
			ViewDigitalInputHandInputDlg dialog = new ViewDigitalInputHandInputDlg(di);
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
			if(dialog.bElementChanged) this.Invalidate();		// 값이 변경되었다.
		}

		public void CallDigitalOutputDialog()
		{
			if(work.TagHap <= 0) return;
			
			ControlBoxDigitalInputGo dialog = new ControlBoxDigitalInputGo();
			dialog.Go(this, work.tagList[work.pos].tag);			
		}

		public void CallDoOutputDialog()
		{
			if(work.TagHap <= 0) return;			
			
			DialogControl.ControlBoxDigitalOutputGo dialog = new DialogControl.ControlBoxDigitalOutputGo();
			dialog.Go(this, work.tagList[work.pos].tag);
		}
		
		public void callAiTrendWindow(int hour)
		{
			if(work.TagHap <= 0) return;
			
			ArrayList arr = new ArrayList();
			multiTrendTagStruct multi = new multiTrendTagStruct();

			getSelectedAiTagName();
			multi.tag = work.tag;
			multi.color = Color.Black;
			multi.edge = 0;
			arr.Add(multi);

			ViewAnalogInputTrendMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogInputTrendMain(hour, arr, Color.White, 14), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public void callDiTrendWindow(int hour)
		{
			if(work.TagHap <= 0) return;

			ArrayList arr = new ArrayList();
			multiTrendTagStruct multi = new multiTrendTagStruct();

			getSelectedDiTagName();
			multi.tag = work.tag;
			multi.color = Color.Black;
			multi.edge = 0;
			arr.Add(multi);

			ViewDigitalInputTrendMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputTrendMain(hour, arr, Color.White), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public void CallAiDataWindow(eDataTime dataTime)
		{
			if(work.TagHap <= 0) return;

			ArrayList arr = new ArrayList();
			multiTrendTagStruct da = new multiTrendTagStruct();
			getSelectedAiTagName();

			da.tag = work.tag;
			da.color = Color.Black;
			da.edge = 0;
			arr.Add(da);
			
			//Cursor = Cursors.WaitCursor;
			ViewAnalogInputDataMain.ringViewAnalogInputDataMain.CreateMdi(TotalConfig.formMain, new ViewAnalogInputDataMain(dataTime, arr, Color.White, (int)eDataDispType.DECIMAL), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public void CallDiDataWindow(eDataTime dataTime)
		{
			if(work.TagHap <= 0) return;

			ArrayList arr = new ArrayList();
			multiTrendTagStruct da = new multiTrendTagStruct();

			getSelectedDiTagName();
			da.tag = work.tag;
			da.color = Color.Black;
			da.edge = 0;
			arr.Add(da);
			
			ViewDigitalInputDataMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputDataMain(dataTime, arr, SharedData.colorTotal.BACK, (int)eDataDispType.DECIMAL), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		private void getStartDataTime()
		{
            work.dt = DateTimeServer.Now;
			if(work.trend_hour > 1)
				work.dt = work.dt.AddHours(-(work.trend_hour-1));
		}

        // 시간이 달라져도 데이터 간격을 지정한 값으로 한다. 2017-12-19 추가
        protected static bool bUseSamePeriod = false;
        protected static int nUseSamePeriod = 1;

		public void setInitTrendWidth()
		{
			int		hour = work.trend_hour;

			getStartDataTime();

            if (bUseSamePeriod)
            {
                work.trend_width = nUseSamePeriod;
            }
            else
            {
                if (hour <= 1) work.trend_width = 1;
                else if (hour <= 8) work.trend_width = 4;
                else if (hour <= 24) work.trend_width = 6;
                else if (hour <= 48) work.trend_width = 10;
                else if (hour <= 72) work.trend_width = 15;
                else work.trend_width = 60;
            }
		}
			

		public void setInitTrendDataHapAndPos()
		{
			DateTime		dt = new DateTime();

			if(work.trend_width <= 0) 
			{
				work.trend_width = 1;				
			}
			work.trend_hap = work.trend_hour*60/work.trend_width + 1;// hap + 마지막 시간			
            dt = DateTimeServer.Now;
			work.trend_pos = ((work.trend_hour-1)*60+dt.Minute)/work.trend_width;//+work.curr_min/width;
		}

		public void TrendCurrentPosDraw(Graphics g)
		{
			int					x;
			
			if(work.bCurrPosLine == false) return;
			if(work.trend_hap <= 1) return;
			
			x = work.Ix+work.fontX*16+(int)((long)work.trend_pos*work.fontX*60/(work.trend_hap-1));
			DrawClass.gline(g, x, work.Iy+(int)(work.fontY*6), x, work.Iy+(int)(work.fontY*23.6), Color.Black);
		}


		public void TrendCurrentPosInvalidate(int oldPos)
		{
			int					x;
			Rectangle			rect;
			
			if(work.bCurrPosLine == false) return;
			if(work.trend_hap <= 1 || oldPos < 0) return;

			x = work.Ix+work.fontX*16+(int)(oldPos*work.fontX*60/(work.trend_hap-1));
			rect = new Rectangle(x, work.Iy+(int)(work.fontY*6), 1, work.fontY*18);
			this.Invalidate(rect);

			x = work.Ix+work.fontX*16+(int)((long)work.trend_pos*work.fontX*60/(work.trend_hap-1));
			rect = new Rectangle(x, work.Iy+(int)(work.fontY*6), 1, work.fontY*18);
			this.Invalidate(rect);
		}

		public int getCurrentValueDrawPos(int pos)
		{
			int			x;

			x = work.Ix+(int)(work.fontX*5.5)+(int)(pos*work.fontX*60/work.trend_hap);
			if(x < work.Ix+work.fontX*16) return work.Ix+work.fontX*16;
			if(x > work.Ix+work.fontX*56) return work.Ix+work.fontX*56;
			return x;
		}

		public void TrendStartEndTimeDraw(Graphics g, ButtonCheck2 hansolButton, ref string startBuf, ref string endBuf)
		{
			DateTime				dt;
			
			dt = new DateTime(work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, 0);
			if(Tools.IsLangKorean())
				startBuf = String.Format("{0}월{1}일{2,2:d2}시00분~", dt.Month, dt.Day, dt.Hour);//work.trend_start_hour);//val[0].mon, val[0].day, work.trend_start_hour);
			else
				startBuf = String.Format("{0}/{1} {2,2:d2}:00 ~", dt.Month, dt.Day, dt.Hour);
			hansolButton.ButtonCheckDraw2(g, work.Ix+(int)(work.fontX*1.5), work.Iy+(int)(work.fontY*25.75), work.Ix+work.fontX*20, work.Iy+(int)(work.fontY*27.25), work.f, startBuf, StringAlignment.Center);
			
			dt = dt.AddHours(work.trend_hour);
			if(Tools.IsLangKorean())
				endBuf = String.Format("{0}월{1}일{2,2:d2}시00분", dt.Month, dt.Day, dt.Hour);
			else
				endBuf = String.Format("{0}/{1} {2}:00", dt.Month, dt.Day, dt.Hour);
			hansolButton.ButtonCheckDraw2(g, work.Ix+(int)(work.fontX*60.0), work.Iy+(int)(work.fontY*25.75), work.Ix+(int)(work.fontX*78.5), work.Iy+(int)(work.fontY*27.25), work.f, endBuf, StringAlignment.Center);
		}


		/*
		public void DisplayAiCurrentValue(Graphics g, TagAiClass ai, int y, int tagPos)
		{
			Color				tcolor;
			int					gap, x, width;			
			StringFormat		format = new StringFormat();
			
			format.Alignment = StringAlignment.Near;			
			tcolor = (ai.act == 1) ? SharedData.colorTotal.TAG : SharedData.colorTotal.INACTIVE;
			gap = (int)(work.fontX*0.5);
			x = getCurrentColumnStart(1)+gap;
			width = getCurrentColumnWidth(1)-(int)(work.fontX);
			DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, ai.tag, tcolor, SharedData.colorTotal.BACK, work.f, format);	
			
			x = getCurrentColumnStart(2)+gap;
			width = getCurrentColumnWidth(2)-(int)(work.fontX);
			if(tagPos == work.pos) tcolor = SharedData.colorTotal.BACK;
			else				   tcolor = (ai.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
			if(tagPos == work.pos) DrawClass.gcls(g, x, y, x+width, y+work.fontY, SharedData.colorTotal.DESCRIPTION);
			DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, ai.description, tcolor, SharedData.colorTotal.BACK, work.f, format);
	
			tcolor = (ai.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;
			x = getCurrentColumnStart(5)+gap;
			width = getCurrentColumnWidth(5)-(int)(work.fontX);
			format.Alignment = StringAlignment.Center;
			DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, ai.unit, tcolor, SharedData.colorTotal.BACK, work.f, format);

			tcolor = (ai.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;			
			x = getCurrentColumnStart(6)+gap;
			width = getCurrentColumnWidth(6)-(int)(work.fontX);
			if(ai.bFileSave == 1) 
			{
				if(Tools.IsLangKorean())
					DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, "자료", tcolor, Color.Red, work.f, format);
				else
					DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, "Data", tcolor, Color.Red, work.f, format);
			}

			x = getCurrentColumnStart(7)+gap;
			width = getCurrentColumnWidth(7)-(int)(work.fontX);
			if(ai.alarm == 1) 
			{
				if(Tools.IsLangKorean())
					DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, "경보", tcolor, Color.Red, work.f, format);
				else
					DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, "Alarm", tcolor, Color.Red, work.f, format);
			}
			x = getCurrentColumnStart(8)+gap;
			if((ai.wProtectFlags & EnumProtectFlag.SCAN) != 0) DrawClass.PopBox2(g, x, y+(int)(work.fontX*0.3), x+(int)work.fontX, y+(int)(work.fontY*0.7), Color.Blue);			
			ShowCurrentAiValueBar(g, ai, y);
		}
		*/

		private void ShowCurrentAiValueBar(Graphics g, TagAiClass ai, int y)
		{
			double			imsi, width;
			int				currValue, lolo, low, high, hihi;
			int				i, j, x, nWidth, gap;
			Color			tcolor = SharedData.colorTotal.TEXT, bcolor = SharedData.colorTotal.BACK;
			StringFormat	format = new StringFormat();
			
			format.Alignment = StringAlignment.Far;

			tcolor = (ai.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;
			gap = (int)(work.fontX*0.5);
			x = getCurrentColumnStart(4)+gap;
			nWidth = getCurrentColumnWidth(4)-(int)(work.fontX);
			DrawClass.gcls(g, x, y, x+nWidth, y+(int)work.fontY, bcolor);

			//			//buf = TagUtil.AiValueToStringOnlyPoint(ai, ai.curr);
			DrawClass.WinDrawText(g, x, y, nWidth, (int)work.fontY, TagUtil.AiValueToStringOnlyPoint(ai, ai.curr), tcolor, bcolor, work.f, format);

			if(ai.act == 0) return;

			x = getCurrentColumnStart(3)+gap;
			width = (float)(getCurrentColumnWidth(3)-work.fontX);
			DrawClass.gcls(g, x, y+(int)(work.fontX*0.2), x+(int)width, y+(int)(work.fontY*0.8), bcolor);

			if(ai.fFull == 0.0) return;    // avoid divide by zero

			imsi = (float)(ai.curr - ai.fBase);
			currValue = (int)(imsi * width / ai.fFull);
			if(currValue <= 1) currValue = 1;
			else if(currValue > width) currValue = (int)width;

			imsi = ai.lolo - ai.fBase;
			lolo = (int)(imsi * (width) / ai.fFull);
			if(lolo <= 1) lolo = 1;
			else if(lolo > width) lolo = (int)width;

			imsi = ai.low - ai.fBase;
			low = (int)(imsi * width / ai.fFull);
			if(low <= 1) low = 1;
			else if(low > width) low = (int)width;

			imsi = ai.high - ai.fBase;
			high = (int)(imsi * width / ai.fFull);
			if(high <= 1) high = 1;
			else if(high > width) high = (int)width;

			imsi = ai.hihi - ai.fBase;
			hihi = (int)(imsi * width / ai.fFull);
			if(hihi <= 1) hihi = 1;
			else if(hihi > width) hihi = (int)width;

			if(currValue > hihi) 
			{
				DrawClass.gcls(g, x+1, y+(int)(work.fontY*0.2), x+lolo, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOLO);
				DrawClass.gcls(g, x+lolo, y+(int)(work.fontY*0.2), x+low, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOW);
				DrawClass.gcls(g, x+low, y+(int)(work.fontY*0.2), x+high, y+(int)(work.fontY*0.8), SharedData.colorTotal.HIGH);
				DrawClass.gcls(g, x+high, y+(int)(work.fontY*0.2), x+currValue, y+(int)(work.fontY*0.8), SharedData.colorTotal.HIHI);
			}
			else if(currValue > high) 
			{
				DrawClass.gcls(g, x+1, y+(int)(work.fontY*0.2), x+lolo, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOLO);
				DrawClass.gcls(g, x+lolo, y+(int)(work.fontY*0.2), x+low, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOW);
				DrawClass.gcls(g, x+low, y+(int)(work.fontY*0.2), x+currValue, y+(int)(work.fontY*0.8), SharedData.colorTotal.HIHI);
			}
			else if(currValue > low) 
			{
				DrawClass.gcls(g, x+1, y+(int)(work.fontY*0.2), x+lolo, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOLO);
				DrawClass.gcls(g, x+lolo, y+(int)(work.fontY*0.2), x+currValue, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOW);
			}
			else
				DrawClass.gcls(g, x+1, y+(int)(work.fontY*0.2), x+currValue, y+(int)(work.fontY*0.8), SharedData.colorTotal.LOLO);


			j = (int)(work.fontY/3);
			if(j < 2) j = 2;
			for(i = j/2+2; i < width-2; i+=j)
				DrawClass.gcls(g, x+i, y+(int)(work.fontY*0.2), x+i+j/2, y+(int)(work.fontY*0.8), Color.Black);

			DrawClass.PopRectangle2(g, x, y+(int)(work.fontY*0.2), x+(int)width, y+(int)(work.fontY*0.8));
			DrawClass.PopRectangle2(g, x-1, y+(int)(work.fontY*0.2)-1, x+(int)width+1, y+(int)(work.fontY*0.8)+1);
		}

		public void DisplayAoCurrentValue(Graphics g, TagAoClass ao, int y, int pos, bool bGroupShow)
		{
			Color				tcolor;
			int					gap, x, width;			
			StringFormat		format = new StringFormat();
			
			format.Alignment = StringAlignment.Near;
			tcolor = (ao.act == 1) ? SharedData.colorTotal.TAG : SharedData.colorTotal.INACTIVE;
			gap = (int)(work.fontX*0.5);
			x = getCurrentColumnStart(1)+gap;
			width = getCurrentColumnWidth(1)-(int)(work.fontX);
			DrawClass.WinDrawText(g, x, y, width, work.fontY, ao.tag, tcolor, SharedData.colorTotal.BACK, work.f, format);
	
			x = getCurrentColumnStart(2)+gap;
			width = getCurrentColumnWidth(2)-(work.fontX);
			if(pos == work.pos) tcolor = SharedData.colorTotal.BACK;
			else				tcolor = (ao.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
			if(pos == work.pos) DrawClass.gcls(g, x, y, x+width, y+work.fontY, SharedData.colorTotal.DESCRIPTION);
			DrawClass.WinDrawText(g, x, y, width, work.fontY, ao.description, tcolor, SharedData.colorTotal.BACK, work.f, format);
	
			format.Alignment = StringAlignment.Center;
			if(bGroupShow) 
			{
				x = getCurrentColumnStart(4)+gap;
				width = getCurrentColumnWidth(4)-(work.fontX);
			}
			else 
			{
				x = getCurrentColumnStart(5)+gap;
				width = getCurrentColumnWidth(5)-(work.fontX);
			}
			if(ao.act == 1) tcolor = SharedData.colorTotal.TEXT;
			DrawClass.WinDrawText(g, x, y, width, work.fontY, ao.unit, tcolor, SharedData.colorTotal.BACK, work.f, format);
	
			ShowAoCurrentValueBar(g, ao, y, bGroupShow);
		}


		void ShowAoCurrentValueBar(Graphics g, TagAoClass ao, int y, bool bGroupShow)
		{
			int				x, width;
			string			buf;
			Color			tcolor = SharedData.colorTotal.TEXT;
			StringFormat	format = new StringFormat();
			
			format.Alignment = StringAlignment.Far;			
			x = getCurrentColumnStart(3)+(int)(work.fontX*0.5);
			if(bGroupShow) width = getCurrentColumnWidth(3)+getCurrentColumnWidth(4)-(int)(work.fontX);
			else		   width = getCurrentColumnWidth(3)-(int)(work.fontX);
			tcolor = (ao.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;

			DrawClass.gcls(g, x, y, x+width, y+work.fontY, SharedData.colorTotal.BACK);
			buf = String.Format("{0,4:f2}", ao.curr);
			DrawClass.WinDrawText(g, x, y, width, work.fontY, buf, tcolor, SharedData.colorTotal.BACK, work.f, format);
		}

		/*
		public void DisplayDiCurrentValue(Graphics g, TagDiClass di, int y, int tagPos, bool bGroupShow)
		{
			Color				tcolor;
			int					x, pos, width;
			StringFormat		format = new StringFormat();
	
			format.Alignment = StringAlignment.Near;
			if(di.act == 0) tcolor = SharedData.colorTotal.INACTIVE;
			else		    tcolor = SharedData.colorTotal.TAG;
			
			x = getCurrentColumnStart(1)+(int)(work.fontX*0.5);
			width = getCurrentColumnWidth(1)-(int)(work.fontX);
			DrawClass.WinDrawText(g, x, y, width, work.fontY, di.tag, tcolor, SharedData.colorTotal.BACK, work.f, format);

			x = getCurrentColumnStart(2)+(int)(work.fontX*0.5);
			width = getCurrentColumnWidth(2)-(int)(work.fontX);
			if(tagPos == work.pos) tcolor = SharedData.colorTotal.BACK;
			else				   tcolor = (di.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
			if(tagPos == work.pos) DrawClass.gcls(g, x, y, x+width, y+work.fontY, SharedData.colorTotal.DESCRIPTION);
			DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, di.description, tcolor, SharedData.colorTotal.BACK, work.f, format);
			
			x = getCurrentColumnStart(3)+(int)(work.fontX*0.5);
			if(bGroupShow) 
			{
				width = getCurrentColumnWidth(3)+getCurrentColumnWidth(4)-(int)(work.fontX);
				pos = 6;
			}
			else 
			{
				width = getCurrentColumnWidth(3)-(int)(work.fontX);
				pos = 4;
			}
			if(di.sSubOutDigital1 != null && di.sSubOutDigital1.Length > 1) 
			{
				if(width/3 > (int)(work.fontX*3.5))
					DrawClass.PushBox2(g, x+width*2/3+work.fontX*3, y+(int)(work.fontY*0.25), x+width*2/3+(int)(work.fontX*3.5), y+(int)(work.fontY*0.45), work.grayColor);
			}
			if(di.sSubOutDigital2 != null && di.sSubOutDigital2.Length > 1) 
			{
				if(width/3 > (int)(work.fontX*3.5))
					DrawClass.PushBox2(g, x+width*2/3+work.fontX*3, y+(int)(work.fontY*0.6), x+width*2/3+(int)(work.fontX*3.5), y+(int)(work.fontY*0.8), work.grayColor);
			}

			if(di.act == 1) tcolor = SharedData.colorTotal.TEXT;
			x = getCurrentColumnStart(pos)+(int)(work.fontX*0.5);
			width = getCurrentColumnWidth(pos)-(int)(work.fontX);
			pos++;
			
			format.Alignment = StringAlignment.Center;
			if(di.bFileSave == 1) 
			{
				if(Tools.IsLangKorean())
					DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, "자료", tcolor, Color.Red, work.f, format);
				else
					DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, "Data", tcolor, Color.Red, work.f, format);
			}

			x = getCurrentColumnStart(pos)+(int)(work.fontX*0.5);
			width = getCurrentColumnWidth(pos)-(int)(work.fontX);
			pos++;

			if(di.alarm != 0) 
			{
				if(Tools.IsLangKorean())
					DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, "경보", tcolor, Color.Red, work.f, format);
				else
					DrawClass.WinDrawText(g, x, y, width, (int)work.fontY, "Alarm", tcolor, Color.Red, work.f, format);
			}

			x = getCurrentColumnStart(pos)+(int)(work.fontX*0.5);
			
			if((di.wProtectFlags & EnumProtectFlag.SCAN) != 0) 
				DrawClass.PopBox2(g, x, y+(int)(work.fontX*0.3), x+(int)work.fontX, y+(int)(work.fontY*0.7), Color.Blue);

			if(di.act == 0)   return;
			DisplayDiCurrentOnOffValue(g, di, y, bGroupShow);
		}
		*/


		void DisplayDiCurrentOnOffValue(Graphics g, TagDiClass di, int y, bool bGroupShow)
		{
			int					x, width;
			StringFormat		format = new StringFormat();
				
			if(di.act == 0)   return;

			format.Alignment = StringAlignment.Center;
			x = getCurrentColumnStart(3)+(int)(work.fontX*0.5);
			if(bGroupShow) width = getCurrentColumnWidth(3)+getCurrentColumnWidth(4)-(int)(work.fontX);
			else 		   width = getCurrentColumnWidth(3)-(int)(work.fontX);
			
			DrawClass.gcls(g, x, y, x+width*2/3, y+work.fontY-1, SharedData.colorTotal.BACK);
			if(di.curr == 1) 
			{
				DrawClass.WinDrawText(g, x, y, width*2/3, work.fontY, di.desON, SharedData.colorTotal.ON, SharedData.colorTotal.BACK, work.f, format);
				if(width/3 > 2*work.fontX)
					DrawClass.PopBox2(g, x+width-work.fontX*3, y+work.fontX/3, x+width-work.fontX, y+work.fontY-work.fontX/3, SharedData.colorTotal.ON);
			}
			else  
			{
				DrawClass.WinDrawText(g, x, y, width*2/3, work.fontY, di.desOFF, SharedData.colorTotal.OFF, SharedData.colorTotal.BACK, work.f, format);
				if(width/3 > 2*work.fontX)					
					DrawClass.PopBox2(g, x+width-work.fontX*3, y+work.fontX/3, x+width-work.fontX, y+work.fontY-work.fontX/3, SharedData.colorTotal.OFF);
			}
		}

		public void DisplayDoCurrentValue(Graphics g, TagDoClass dout, int y, int tagPos, bool bGroupShow)
		{
			Color				tcolor;
			int					x, width;			
			StringFormat		format = new StringFormat();
	
			format.Alignment = StringAlignment.Near;
			x = getCurrentColumnStart(1)+(int)(work.fontX*0.5);
			width = getCurrentColumnWidth(1)-(work.fontX);
			if(dout.act == 0) tcolor = SharedData.colorTotal.INACTIVE;
			else			  tcolor = SharedData.colorTotal.TAG;
			DrawClass.WinDrawText(g, x, y, width, work.fontY, dout.tag, tcolor, SharedData.colorTotal.BACK, work.f, format);
	
			x = getCurrentColumnStart(2)+(int)(work.fontX*0.5);
			width = getCurrentColumnWidth(2)-(work.fontX);			
			if(tagPos == work.pos) tcolor = SharedData.colorTotal.BACK;
			else				   tcolor = (dout.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
			if(tagPos == work.pos) DrawClass.gcls(g, x, y, x+width, y+work.fontY, SharedData.colorTotal.DESCRIPTION);
			DrawClass.WinDrawText(g, x, y, width, work.fontY, dout.description, tcolor, SharedData.colorTotal.BACK, work.f, format);
	
			if(dout.act == 0) return;
			DisplayDoCurrentOnOffValue(g, dout, y, bGroupShow);
		}


		void DisplayDoCurrentOnOffValue(Graphics g, TagDoClass dout, int y, bool bGroupShow)
		{
			int					x, width;
			StringFormat		format = new StringFormat();
				
			if(dout.act == 0)   return;

			format.Alignment = StringAlignment.Center;
			x = getCurrentColumnStart(3)+(int)(work.fontX*0.5);
			if(bGroupShow) width = getCurrentColumnWidth(3)+getCurrentColumnWidth(4)-(int)(work.fontX);
			else 		   width = getCurrentColumnWidth(3)-(int)(work.fontX);
				
			DrawClass.gcls(g, x, y, x+width*2/3, y+work.fontY, SharedData.colorTotal.BACK);
			if(dout.curr == 1) 
			{
				DrawClass.WinDrawText(g, x, y, width*2/3, work.fontY, dout.desON, SharedData.colorTotal.ON, SharedData.colorTotal.BACK, work.f, format);
				if(width/3 > 2*work.fontX)
					DrawClass.PopBox2(g, x+width-work.fontX*3, y+work.fontX/3, x+width-work.fontX, y+work.fontY-work.fontX/3, SharedData.colorTotal.ON);
			}
			else if(dout.curr == 0) 
			{
				DrawClass.WinDrawText(g, x, y, width*2/3, work.fontY, dout.desOFF, SharedData.colorTotal.OFF, SharedData.colorTotal.BACK, work.f, format);
				if(width/3 > 2*work.fontX)
					DrawClass.PopBox2(g, x+width-work.fontX*3, y+work.fontX/3, x+width-work.fontX, y+work.fontY-work.fontX/3, SharedData.colorTotal.OFF);
			}
		}

		public void DisplayStCurrentValue(Graphics g, TagStClass st, int y, int tagPos, bool bGroupShow)
		{
			Color				tcolor;
			int					x, width;			
			StringFormat		format = new StringFormat();
	
			format.Alignment = StringAlignment.Near;
			x = getCurrentColumnStart(1)+(int)(work.fontX*0.5);
			width = getCurrentColumnWidth(1)-(work.fontX);
			if(st.act == 0) tcolor = SharedData.colorTotal.INACTIVE;
			else		    tcolor = SharedData.colorTotal.TAG;			
			DrawClass.WinDrawText(g, x, y, width, work.fontY, st.tag, tcolor, SharedData.colorTotal.BACK, work.f, format);
	
			x = getCurrentColumnStart(2)+(int)(work.fontX*0.5);
			width = getCurrentColumnWidth(2)-(work.fontX);
			if(tagPos == work.pos) tcolor = SharedData.colorTotal.BACK;
			else				   tcolor = (st.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
			if(tagPos == work.pos) DrawClass.gcls(g, x, y, x+width, y+work.fontY, SharedData.colorTotal.DESCRIPTION);
			DrawClass.WinDrawText(g, x, y, width, work.fontY, st.description, tcolor, SharedData.colorTotal.BACK, work.f, format);
	
			if(st.act == 0) return;
			DisplayStCurrentOnOffValue(g, st, y, bGroupShow);
		}


		void DisplayStCurrentOnOffValue(Graphics g, TagStClass st, int y, bool bGroupShow)
		{
			if(st.act == 0) return;

			int					x, width;
			StringFormat		format = new StringFormat();

			x = getCurrentColumnStart(3)+(int)(work.fontX*0.5);
			if(bGroupShow) width = getCurrentColumnWidth(3)+getCurrentColumnWidth(4)-(int)(work.fontX);				
			else 		   width = getCurrentColumnWidth(3)-(int)(work.fontX);
			DrawClass.gcls(g, x, y, x+width, y+work.fontY, SharedData.colorTotal.BACK);
	
			format.Alignment = StringAlignment.Near;			
			DrawClass.WinDrawText(g, x, y, width, work.fontY, st.curr, SharedData.colorTotal.DESCRIPTION, SharedData.colorTotal.BACK, work.f, format);
		}


		public bool isActiveTag()
		{
			TagPublicClass	tp = TagLib.GetStructPublic(work.tagList[work.pos]);
			return isActiveTag(tp);
		}

		public bool isActiveTag(TagPublicClass tp)
		{
			if(tp == null || tp.act == 0) 
			{
				if(Tools.IsLangKorean()) MessageBox.Show("이 태그는 비활성 태그입니다.", "비활성 태그");
				else MessageBox.Show("Selected Tag is Inactive.", "Inactive Tag");
				return false;
			}

			return true;
		}
		

		public void callTimeSettingDialog()
		{
			DateTime		dt = new DateTime(work.dt.Year, work.dt.Month, work.dt.Day, work.dt.Hour, 0, 0);
			ViewAnalogInputTrendDialogTime dlg = new ViewAnalogInputTrendDialogTime();
			
			dlg.numericUpDown_year.Value = work.dt.Year;
			dlg.numericUpDown_month.Value = work.dt.Month;
			dlg.numericUpDown_day.Value = work.dt.Day;			
			dlg.numericUpDown_hour.Value = work.dt.Hour;
            dlg.StartPosition = FormStartPosition.CenterParent;

			if(dlg.ShowDialog(this) == DialogResult.OK) 
			{
				work.dt = new DateTime((int)dlg.numericUpDown_year.Value, (int)dlg.numericUpDown_month.Value, (int)dlg.numericUpDown_day.Value, (int)dlg.numericUpDown_hour.Value, 0, 0);
				if(dt != work.dt) work.bDataReadFlag = false;
			}
			this.Invalidate();
		}

		public void callDataHourMinusFunc()
		{
			work.bDataReadFlag = false;
			work.dt = work.dt.AddHours(-work.trend_hour);
			this.Invalidate();
		}

		public void callDataHourPlusFunc()
		{
			work.bDataReadFlag = false;
			work.dt = work.dt.AddHours(work.trend_hour);
			this.Invalidate();
		
		}

		public int getTrendWidthPos()
		{
			switch(work.trend_width)
			{
				case 1 : return 0;
				case 2 : return 1;
				case 3 : return 2;
				case 4 : return 3;
				case 5 : return 4;
				case 6 : return 5;
				case 10 : return 6;
				case 12 : return 7;
				case 15 : return 8;
				case 20 : return 9;
				case 30 : return 10;
				case 60 : return 11;
				default : return 0;
			}
		}

		public int getPosToTrendWidth(int pos)
		{
			switch(pos)
			{
				case 0 : return 1;
				case 1 : return 2;
				case 2 : return 3;
				case 3 : return 4;
				case 4 : return 5;
				case 5 : return 6;
				case 6 : return 10;
				case 7 : return 12;
				case 8 : return 15;
				case 9 : return 20;
				case 10 : return 30;
				case 11 : return 60;
				default : return 1;
			}
		}

		public void callShowTimeChangeFunc()
		{
			if(work.dataTime == eDataTime.MONTH) work.dataTime = eDataTime.MIN;
			else work.dataTime++;
			work.bDataReadFlag = false;
			this.Invalidate();		
		}
		
		public void changeDataTimePlusMinus(int add)
		{
			work.bDataReadFlag = false;
			switch(work.dataTime)
			{
				case eDataTime.MIN : work.dt = work.dt.AddHours(add); break;
				case eDataTime.HOUR : work.dt = work.dt.AddDays(add); break;
				case eDataTime.DAY : work.dt = work.dt.AddMonths(add); break;				
				default : work.dt = work.dt.AddYears(add); break;
			}			
			this.Invalidate();
		}

		public void TagSearchNameOrPos()
		{
			if(work.TagHap <= 0) return;

			ViewTagNamePosSearchDlg dialog = new ViewTagNamePosSearchDlg(work.pos, work.TagHap);
			dialog.tagList = work.tagList;
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
			if(dialog.bTagChanged)								// 값이 변경되었다.
			{
				work.pos = dialog.currPos % work.TagHap;
				if(work.y_num < work.TagHap) 				
				{
					setScrollPosFitFontY(work.fontY);
					int		hap = work.pos-work.spos;
					setScrollPosY();
				}
				this.Invalidate();
			}
		}

		

		

		

		
		

		
		
	}

	public enum eDataTime { MIN = 0, HOUR, DAY, WEEK, MONTH };
	public enum eDataSort { AVERAGE = 0, MIN, MAX, SUM, ALL_DATA };
	public enum eDataDispType { DECIMAL = 0, BAR, LINE };
	public enum eTrendEdgeType { NONE = 0, RECT, CIRCLE, TRI, DIAMOND, X_CHAR, STAR };
	
	[Serializable]
	public struct MAIN_DRAW_POS
	{
		//public bool		bConfigChange;
		public int		maxColumn;
		public float[]	xNum;// = new float[10];
		public bool[]	xNumReSizeFlag;// = new bool[10];
	};
    
	public class WorkStruct 
	{
		public DateTime dt;
		
		public int pos;
		public bool bStartFlag;
		public string tag;
		public int spos;
		public int width;
		public int height;
		public int TagHap;
		public int fontX;
		public int fontY;
		public int Ix;
		public int Iy;
		public int y_num;
		public bool bConfigChange;
		public bool bMouseCapture;
		public int  nCaptureMouseX;
		public int	nCapturePos;
		public int	nCaptureX;
		public MAIN_DRAW_POS	xNum;
		//public Color	whiteGray = Color.FromArgb(236, 233, 216);
		public Color	grayColor = Color.FromArgb(236, 233, 216);
		public Color	backColor = Color.FromArgb(236, 233, 216);		
		//public Color    textColor = Color.FromArgb(0, 255, 255);
		//public Color	inActiveColor = Color.FromArgb(127, 127, 127);
		//public Color	tagColor = Color.FromArgb(255, 255, 0);
		//public Color	descColor = Color.FromArgb(0, 127, 255);
		//public Color	loloColor = Color.FromArgb(127, 127, 255);
		//public Color	lowColor = Color.FromArgb(127, 127, 0);
		//public Color	highColor = Color.FromArgb(0, 0, 255);
		//public Color	hihiColor = Color.FromArgb(0, 127, 0);
		//public Color	onColor = Color.FromArgb(255, 0, 0);
		//public Color	offColor = Color.FromArgb(0, 0, 255);
		public Font f;

		public bool bDisplayFitWindowSize;

		public int trend_min;			//ai detail
		public int trend_hap;
		//public float iValue;			// imsi value
		public double iOldValue;

		public int trend_hour;			// ai trend		
		public int mainTrendPos;		// 
		public bool bDataReadFlag;
		public bool bGuideLine;
		public int trend_width;
		public int trend_start_hour;
		public bool bCurrPosLine;		// 현재 선택된 라인 draw flag
		public bool bGuideAlarm;		// hihi, high, low, lolo 라인 draw flag
		public int trend_pos;
		public int trend_tag_hap;
		public int nMultiTrendShowTagSize;	// 멀티 트랜드에 표시될 태그의 크기

		public eDataSort dataSort;		// ai trend, data	평균, 최대, 최소, 적산
		public eDataTime dataTime;		// 	분, 시간, 일, 월, 년
		public eDataDispType dataDispType;//숫자, 바그래프, 선그래프
		public bool bChangeMinFlag;
		//public bool bReadFlag;
        //public bool bPrintFlag;       ConfigViewMain.bBasciScreenDataViewWhiteBackground 로 바뀜
		public long	remain_mili_sec, old_mili_sec;	// 상세보기의 타이머, 남아있는/이전 시간
		public bool	bTagChanged;					// 이전/다음 태그로 보기가 바뀌었냐? , Page Up/Down
		public TagListStruct[]	tagList;			// 아날로그/디지털/문자열 등의 태그를 저장할 스트럭쳐
		public EnumTagType		eTagType;			// tagList에 저장된 태그 타입
	}

	public struct multiTrendTagStruct
	{		
		public string		tag;//	pos;						// 태그이름
		public Color		color;
		public eTrendEdgeType	edge;		
	};
}
