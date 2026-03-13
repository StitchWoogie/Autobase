using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools.OldDefine;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Drawing.Drawing2D;

namespace GraphicModule
{
	

	/// <summary>
	/// Summary description for FormDemandControlChild. 
	/// </summary>
	public class FormDemandControlChild : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		//string sClassName = "DefaultClassDemandWindow";
        Font lfLogFont;
		int nLineThickTarget = 1;
		WORK_DEMAND_CHILD work = new WORK_DEMAND_CHILD();
		string sDemandName;

        //20250227 PSU 추가
        int nStatusBarPos = 0;
        BrushPublic ColorFill = new BrushPublic();
        BrushPublic ColorBack = new BrushPublic();
        Color ColorText = Color.Black;
        Color ColorGuideLine = Color.LightGray;
        Color ColorPredictionPower = Color.Blue;
        Color ColorExcessedPower = Color.Red;
        Color ColorTargetPower = Color.LightGreen;
        Color ColorStatusFill = Color.Black;
        Color ColorStatusBack = Color.LightGray;
        Color ColorStatusValue = Color.Yellow;
        Color ColorTargetValue = Color.LightGreen;
        Color ColorPreValue = Color.FromArgb(0, 255, 255);
        Color ColorExValue = Color.Red;

		public void SetControlNo(int no)
		{
			this.work.nControlNo = no;
		}

		public FormDemandControlChild()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            lfLogFont = ConfigViewMain.MakeDefaultFont();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDemandControlChild));
            this.SuspendLayout();
            // 
            // FormDemandControlChild
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = null;
            this.Name = "FormDemandControlChild";
            this.Load += new System.EventHandler(this.FormDemandControlChild_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormDemandControlChild_Paint);
            this.SizeChanged += new System.EventHandler(this.FormDemandControlChild_SizeChanged);
            this.Closed += new System.EventHandler(this.FormDemandControlChild_Closed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormDemandControlChild_MouseDown);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetLogFont(Font font)
		{
			lfLogFont = font;

		}

        public void SetBasicColor() //20250227 PSU 추가
        {
            this.ColorBack.basic_color = Color.White;
            this.ColorFill.basic_color = Color.White;
        }

        public void SetColors(Color predictionPower, Color excessedPower, Color targetPower,  //20250210
  Color guideLine, Color text, BrushPublic fill, BrushPublic back, Color statusBack, Color statusFill, Color statusValue, Color targetValue, Color preValue, Color exValue, int statusBarPos)
        {
            ColorPredictionPower = predictionPower;
            ColorExcessedPower = excessedPower;
            ColorTargetPower = targetPower;
            ColorGuideLine = guideLine;
            ColorText = text;
            ColorFill = fill;
            ColorBack = back;
            ColorStatusBack = statusBack;
            ColorStatusFill = statusFill;
            ColorStatusValue = statusValue;
            ColorTargetValue = targetValue;
            ColorPreValue = preValue;
            ColorExValue = exValue;
            nStatusBarPos = statusBarPos;
            Invalidate();
        }


        //private void FormDemandControlChild_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if(work.nControlNo >= (int)DemandControl.blockDemandControl.Count) 
        //    {
        //        return;
        //    }

        //    if(e.X >= work.rTarget.left && e.Y >= work.rTarget.top && 
        //        e.X <= work.rTarget.right && e.Y <= work.rTarget.bottom) 
        //    {

        //        FUNCTION_BLOCK_DEMAND_CONTROL item;

        //        item = (FUNCTION_BLOCK_DEMAND_CONTROL)DemandControl.blockDemandControl[work.nControlNo];

        //        TagAiClass ai = TagLib.GetStructAI(item.tagTarget.tag, ref item.tagTarget.tag_pos);

        //        if(ai.cTagLinkType == 2 ||	// 메모리 태그
        //            ai.cTagLinkType == 3) 
        //        {	// 간접 태그
        //            DialogControl.ControlBoxAnalogInputGo go = new DialogControl.ControlBoxAnalogInputGo();
        //            go.Go(this, item.tagTarget.tag);
        //        }
        //    }		
        //}

        private void FormDemandControlChild_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) //20250227 PSU 수정
        {
            if (work.nControlNo >= (int)DemandControl.blockDemandControl.Count)
            {
                return;
            }

            if (nStatusBarPos == 2) //상태바 숨김 251128 PSU
            {
                return;
            }

            // 세로 바 형태일 때의 목표전력 클릭 영역 계산
            if (nStatusBarPos == 1)
            {
                // 세로 바의 기본 너비
                int leftBarWidth = work.border_height * 3;

                // 목표전력 박스의 위치와 크기 계산
                int boxX = 8;
                int boxWidth = leftBarWidth - 16;
                int startY = 15;
                int boxHeight = (int)(work.cyChar * 1.5);
                int boxY = startY + work.cyChar + 4;

                // 새로운 목표전력 클릭 영역 확인
                if (e.X >= boxX && e.X <= (boxX + boxWidth) &&
                    e.Y >= boxY && e.Y <= (boxY + boxHeight))
                {
                    FUNCTION_BLOCK_DEMAND_CONTROL item;
                    item = (FUNCTION_BLOCK_DEMAND_CONTROL)DemandControl.blockDemandControl[work.nControlNo];
                    TagAiClass ai = TagLib.GetStructAI(item.tagTarget.tag, ref item.tagTarget.tag_pos);
                    if (ai.cTagLinkType == 2 || ai.cTagLinkType == 3)
                    {
                        DialogControl.ControlBoxAnalogInputGo go = new DialogControl.ControlBoxAnalogInputGo();
                        go.Go(this, item.tagTarget.tag);
                    }
                    return;
                }
            }
            // 기존의 가로 바 형태 클릭 처리
            else if (e.X >= work.rTarget.left && e.Y >= work.rTarget.top &&
                e.X <= work.rTarget.right && e.Y <= work.rTarget.bottom)
            {

                FUNCTION_BLOCK_DEMAND_CONTROL item;

                item = (FUNCTION_BLOCK_DEMAND_CONTROL)DemandControl.blockDemandControl[work.nControlNo];

                TagAiClass ai = TagLib.GetStructAI(item.tagTarget.tag, ref item.tagTarget.tag_pos);

                if (ai.cTagLinkType == 2 ||	// 메모리 태그
                    ai.cTagLinkType == 3)
                {	// 간접 태그
                    DialogControl.ControlBoxAnalogInputGo go = new DialogControl.ControlBoxAnalogInputGo();
                    go.Go(this, item.tagTarget.tag);
                }
            }
        }



		void CalcOnSize()
		{
			Rectangle rect = this.ClientRectangle;

			work.gx1 = 7+work.cxChar*10;
			work.gy1 = work.border_height+(work.cyChar/2)+1;
			work.gx2 = rect.Right-8;
			work.gy2 = rect.Bottom-8-work.cyChar;	
		}

		public void CalcVar()
		{
			work.cxChar = (int)lfLogFont.Height/2;
			work.cyChar = (int)lfLogFont.Height;

			work.border_height = work.cyChar*2+22;

			work.rTarget.left = 6+work.cxChar*1;
			work.rTarget.top = 9+work.cyChar;
			work.rTarget.right = 6+work.cxChar*15;
			work.rTarget.bottom = 11+work.cyChar*2;

		}

		private void FormDemandControlChild_Load(object sender, System.EventArgs e)
		{
			CalcVar();
			CalcOnSize();

			DemandControl.arrayClassList.Add(this);

            // 2007.6.14 OnPaintBitmap 대신 사용할 수 있다. 이 함수로 화면 떨림을 예방할 수 있다. OnPaintBitmap(Memory dc)을 사용하면 화면위에 투명한 윈도우가 오면 화면 떨림이 발생한다.
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
		}

		private void FormDemandControlChild_Closed(object sender, System.EventArgs e)
		{
			DemandControl.arrayClassList.Remove(this);
		}

		private void FormDemandControlChild_SizeChanged(object sender, System.EventArgs e)
		{
			CalcOnSize();

			this.Invalidate();
		}

        void DrawBorderText(Graphics g, int x1, int y1, int x2, int y2, string str, Color color) //각 인자를 color에 맞춰 박스안에 기입.
        {
            RECT r = new RECT();

            DrawClass.PushBox2(g, x1, y1, x2, y2, ColorStatusFill);
            r.left = x1 + 1;
            r.top = y1 + 1;
            r.right = x2 - 1;
            r.bottom = y2;

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            format.LineAlignment = StringAlignment.Center;
            DrawClass.DrawText(g, str, lfLogFont, new SolidBrush(color), r, format);
        }


        void DisplayGraphGuide(Graphics g)
        {
            if (work.nControlNo >= (int)DemandControl.blockDemandControl.Count) return;

            FUNCTION_BLOCK_DEMAND_CONTROL item;

            item = (FUNCTION_BLOCK_DEMAND_CONTROL)DemandControl.blockDemandControl[work.nControlNo];

            //Pen pen = new Pen(Color.LightGray);
            Pen pen = new Pen(ColorGuideLine);
            int hap_x = item.nControlTime * 60;
            double hap_y;
            int width = work.gx2 - work.gx1 + 1;
            int height = work.gy2 - work.gy1 + 1;
            int x, y;
            int i;
            RECT r = new RECT();
            string buf;
            int old_x = 0;
            int old_y = work.gy2 + work.cyChar;
            SizeF size;
            Brush brushText = new SolidBrush(ColorText);  //20250211

            hap_y = (int)(item.fTargetValue * item.nPercentY / 100);
            if (item.fValueForecast > hap_y) hap_y = item.fValueForecast;

            if (hap_y == 0) hap_y = 1;

            // guide line을 그린다.

            for (i = 0; i < hap_x; i++)
            {
                if ((i % 60) != 0) continue;

                x = work.gx1 + i * width / hap_x;

                if (i != 0) g.DrawLine(pen, x, work.gy1, x, work.gy2);

                buf = String.Format("{0}:{1:00}", i / 60, i % 60);

                size = g.MeasureString(buf, lfLogFont);

                r.left = (int)(x - size.Width / 2);
                r.top = work.gy2 + 2;
                r.right = (int)(r.left + size.Width);
                r.bottom = r.top + work.cyChar;

                if (r.left > old_x + work.cxChar)
                {
                    old_x = r.right;
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    //DrawClass.DrawText(g, buf, lfLogFont, Brushes.Black, r, format);  //x축 글자

                    DrawClass.DrawText(g, buf, lfLogFont, brushText, r, format);
                }
            }

            int fBase = (int)(item.fTargetValue / 20);

            if (fBase == 0) fBase = 1;

            if (fBase < 10) fBase = 10;
            else if (fBase < 50) fBase = 50;
            else if (fBase < 100) fBase = 100;
            else if (fBase < 500) fBase = 500;
            else if (fBase < 1000) fBase = 1000;
            else { }

            for (i = 0; i < hap_y; i++)
            {
                if (i == 0) continue;
                if ((i % fBase) != 0) continue;

                y = (int)(work.gy2 - i * height / hap_y);

                buf = i.ToString();

                r.left = 0;
                r.top = y - work.cyChar / 2;
                r.right = work.gx1 - 2;
                r.bottom = r.top + work.cyChar;

                if (r.bottom < old_y)
                {
                    old_y = r.top;

                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Center;
                    //DrawClass.DrawText(g, buf, lfLogFont, Brushes.Black, r, format);  //y축 글자

                    DrawClass.DrawText(g, buf, lfLogFont, brushText, r, format);
                    g.DrawLine(pen, work.gx1, y, work.gx2, y);
                }
            }
        }


        void DisplayGraph(Graphics g, int line_thick)
        {
            // 현재 그래픽 품질 설정 저장
           // SmoothingMode originalSmoothing = g.SmoothingMode;
           // g.SmoothingMode = SmoothingMode.AntiAlias;

            if (work.nControlNo >= (int)DemandControl.blockDemandControl.Count) return;

            FUNCTION_BLOCK_DEMAND_CONTROL item;

            item = (FUNCTION_BLOCK_DEMAND_CONTROL)DemandControl.blockDemandControl[work.nControlNo];

            // Color color = item.fValueForecast > item.fTargetValue ? Color.Red : Color.Blue;  //targetvalue보다 높으면 빨강, 낮으면 파랑
            Color color = item.fValueForecast > item.fTargetValue ? ColorExcessedPower : ColorPredictionPower;  //targetvalue보다 높으면 빨강, 낮으면 파랑

            Pen hPenReal = new Pen(color, 3);
            Pen hPenVirtual = new Pen(color, 1);
            //Pen hPenGuide = new Pen(Color.LightGreen, line_thick);
            Pen hPenGuide = new Pen(ColorTargetPower, line_thick);

            int hap_x = item.nControlTime * 60;
            double hap_y;
            int width = work.gx2 - work.gx1 + 1;
            int height = work.gy2 - work.gy1 + 1;
            int x, y;
            int i;
            int movex, movey;

            hap_y = (int)(item.fTargetValue * item.nPercentY / 100);

            if (item.fValueForecast > hap_y) hap_y = item.fValueForecast;

            if (hap_y == 0) hap_y = 1;

            // guide line을 그린다.
            x = work.gx1;
            if (item.tagStartTarget.tag_type == EnumTagType.AI)
            {
                TagAiClass ai = TagLib.GetStructAI(item.tagStartTarget.tag, ref item.tagStartTarget.tag_pos);
                y = (int)(work.gy2 - ai.curr * height / hap_y);
            }
            else
                y = work.gy2;

            if (y < work.gy1) y = work.gy1;
            if (y > work.gy2) y = work.gy2;

            movex = x;
            movey = y;

            x = work.gx2;
            y = (int)(work.gy2 - item.fTargetValue * height / hap_y);
            g.DrawLine(hPenGuide, movex, movey, x, y);

            //SelectObject(hdc, hPenReal);

            movex = work.gx1;
            movey = work.gy2; // 여기서 MoveTo를 하는 이유는 진행이 하나도 안되었을 때를 위해서

            for (i = 0; i < hap_x && i < item.nCurrentSec; i++)
            {
                x = work.gx1 + i * width / hap_x;
                y = (int)(work.gy2 - item.data[i] * height / hap_y);

                if (y < work.gy1) y = work.gy1;
                if (y > work.gy2) y = work.gy2;

                if (i == 0)
                {
                    //MoveToEx(hdc, x, y, NULL);
                }
                else
                {
                    g.DrawLine(hPenReal, movex, movey, x, y);
                }
                movex = x;
                movey = y;
            }

            //SelectObject(hdc, hPenVirtual);

            x = work.gx2;
            y = (int)(work.gy2 - item.fValueForecast * height / hap_y);

            if (y < work.gy1) y = work.gy1;
            if (y > work.gy2) y = work.gy2;

            g.DrawLine(hPenVirtual, movex, movey, x, y);

            //// 그래픽 품질 원래대로 복원
            //g.SmoothingMode = originalSmoothing;

            // 펜 리소스 해제
            hPenReal.Dispose();
            hPenVirtual.Dispose();
            hPenGuide.Dispose();
        }


        void DisplayAll(Graphics g)
        {
            // TODO: Add your message handler code here
            RECT rect = new RECT();
            string buf;
            FUNCTION_BLOCK_DEMAND_CONTROL item;

            rect.left = this.ClientRectangle.Left;
            rect.top = this.ClientRectangle.Top;
            rect.bottom = this.ClientRectangle.Bottom;
            rect.right = this.ClientRectangle.Right;

            if (work.nControlNo >= (int)DemandControl.blockDemandControl.Count)
            {
                DrawClass.PopBox2(g, 0, 0, rect.right - 1, rect.bottom - 1, Color.LightGray);
                buf = String.Format("[{0}] Demand not found", sDemandName);
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                DrawClass.DrawText(g, buf, lfLogFont, Brushes.Black, rect, format);
                return;
            }

            item = (FUNCTION_BLOCK_DEMAND_CONTROL)DemandControl.blockDemandControl[work.nControlNo];

            if (item.error_flag == 1)
            {
                DrawClass.PopBox2(g, 0, 0, rect.right - 1, rect.bottom - 1, Color.LightGray);

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                DrawClass.DrawText(g, item.sErrorMsg, lfLogFont, Brushes.Black, rect, format);
                return;
            }

            double value_remain = item.fTargetValue - item.fValueForecast;	// 남은 전력
            TagAiClass ai = TagLib.GetStructAI(item.tagTarget.tag, ref item.tagTarget.tag_pos);

            //SetTextColor(hdc, DARK_COLOR);
            //SetBkColor(hdc, WHITE_GRAY_COLOR);

            if (nStatusBarPos == 1)
            {
                // 왼쪽 바 형태로 표시
                int leftBarWidth = work.border_height * 3;

                // 배경 설정
                //DrawClass.gcls(g, 0, 0, rect.right - 1, rect.bottom - 1, Color.White);
                Brush brush = ObjectRectangle.MakePublicBrush(ColorBack, 0, 0, rect.right - 1, rect.bottom - 1);
                DrawClass.gcls(g, 0, 0-1, rect.right - 1, rect.bottom - 1, brush);

                // 전체 항목 개수와 간격 계산
                int itemCount = 6;
                int baseSpacing = work.border_height + (int)(work.cyChar * 0.5);
                int startY = 15;
                int totalHeight = startY + (baseSpacing * (itemCount + 1));

                // 왼쪽 바 배경 그리기
                // DrawClass.PopBox2(g, 0, 0, leftBarWidth, totalHeight, Color.LightGray);
                //DrawClass.PushRectangle2(g, 5, 5, leftBarWidth - 5, totalHeight - 5);
                //DrawClass.PopRectangle2(g, 6, 6, leftBarWidth - 6, totalHeight - 6);
                DrawClass.PopBox2(g, 0, 0, leftBarWidth, work.gy2 + work.cyChar, ColorStatusBack);
                DrawClass.PopBox2(g, 0, 0, leftBarWidth, work.gy2 + work.cyChar + 8, ColorStatusBack);
                DrawClass.PushRectangle2(g, 5, 5, leftBarWidth - 5, work.gy2 + work.cyChar);
                DrawClass.PopRectangle2(g, 6, 6, leftBarWidth - 6, work.gy2 + work.cyChar - 1);

                // 텍스트와 박스를 위한 기본 설정
                int textX = 8;
                int textY = startY;
                int boxX = 8;
                int boxWidth = leftBarWidth - 16;
                int boxY = textY + work.cyChar + 4;
                int boxHeight = (int)(work.cyChar * 1.5);

                // 목표전력
                DrawValueItem(g, "목표전력", String.Format("{0:F0} {1}", item.fTargetValue, ai.unit),
                  textX, ref textY, boxX, ref boxY, boxWidth, boxHeight, baseSpacing, ColorTargetValue);

                // 예측전력
                buf = String.Format("{0:F0} {1}", item.fValueForecast, ai.unit);
                DrawValueItem(g, "예측전력", buf, textX, ref textY, boxX, ref boxY, boxWidth, boxHeight,
                    baseSpacing, item.fValueForecast > item.fTargetValue ? ColorExValue : ColorPreValue);

                // 현재전력
                DrawValueItem(g, "현재전력", String.Format("{0:F0} {1}", item.fValueKwh, ai.unit),
                    textX, ref textY, boxX, ref boxY, boxWidth, boxHeight, baseSpacing, ColorStatusValue);

                // 경과시간
                DrawValueItem(g, "경과시간", String.Format("{0}:{1:00}", item.nCurrentSec / 60, item.nCurrentSec % 60),
                    textX, ref textY, boxX, ref boxY, boxWidth, boxHeight, baseSpacing, ColorStatusValue);

                // 부하상태
                string statusText = item.bUseIsolation == 0 ? "사용안함" :
                    TagUtil.GetDigitalStatusString(TagLib.GetStructDI(item.tagEcho.tag, ref item.tagEcho.tag_pos),
                    TagLib.GetStructDI(item.tagEcho.tag, ref item.tagEcho.tag_pos).curr);
                DrawValueItem(g, "부하상태", statusText, textX, ref textY, boxX, ref boxY, boxWidth, boxHeight,
                    baseSpacing, ColorStatusValue);

                // 여유/초과전력
                string powerText = value_remain > 0 ? "여유전력" : "초과전력";
                DrawValueItem(g, powerText, String.Format("{0:F0} {1}", Math.Abs(value_remain), ai.unit),
                    textX, ref textY, boxX, ref boxY, boxWidth, boxHeight, baseSpacing, ColorStatusValue);

                // 그래프 영역 조정
                work.gx1 = leftBarWidth + (7 + work.cxChar * 10);
                work.gy1 = 10;   //그래프 y 위치 올리기.

                brush = ObjectRectangle.MakePublicBrush(ColorFill, 0, 0, rect.right - 1, rect.bottom - 1);
                DrawClass.gcls(g, work.gx1 - 1, work.gy1 - 1, work.gx2 + 1, work.gy2 + 1, brush);

                DrawClass.grect(g, work.gx1 - 1, work.gy1 - 1, work.gx2 + 1, work.gy2 + 1, ColorText);
            }
            else if (nStatusBarPos == 2) //상태바 숨김  251128 PSU
            {
                work.border_height = 0;
                // 그래프 영역 조정
                work.gx1 = (7 + work.cxChar * 10);
                work.gy1 = 10;   //그래프 y 위치 올리기

                Brush brush = ObjectRectangle.MakePublicBrush(ColorBack, 0, work.border_height, rect.right - 1, rect.bottom - 1);
                DrawClass.gcls(g, 0 - 1, work.border_height - 1, rect.right - 1, rect.bottom - 1, brush);  //디맨드 배경 색상

                brush = ObjectRectangle.MakePublicBrush(ColorFill, 0, 0, rect.right - 1, rect.bottom - 1); //그래프 채움색상
                DrawClass.gcls(g, work.gx1 - 1, work.gy1 - 1, work.gx2 + 1, work.gy2 + 1, brush);
                DrawClass.grect(g, work.gx1 - 1, work.gy1 - 1, work.gx2 + 1, work.gy2 + 1, ColorText);  // 그래프영역 외곽선
            }
            else  //가로 툴바
            {
                Brush brush = ObjectRectangle.MakePublicBrush(ColorBack, 0, work.border_height, rect.right - 1, rect.bottom - 1);
                DrawClass.gcls(g, 0-1, work.border_height, rect.right - 1, rect.bottom - 1, brush);  //디맨드 배경 색상


                DrawClass.PopBox2(g, 0, 0, rect.right - 1, work.border_height, ColorStatusBack);  //상단 바 배경색상

                DrawClass.PushRectangle2(g, 5, 5, rect.right - 6, work.border_height - 5);  //상단 바 push
                DrawClass.PopRectangle2(g, 6, 6, rect.right - 7, work.border_height - 6);  // 상단 바 pop

                brush = ObjectRectangle.MakePublicBrush(ColorFill, 0, 0, rect.right - 1, rect.bottom - 1); //그래프 채움색상
                DrawClass.gcls(g, work.gx1 - 1, work.gy1 - 1, work.gx2 + 1, work.gy2 + 1, brush);
                DrawClass.grect(g, work.gx1 - 1, work.gy1 - 1, work.gx2 + 1, work.gy2 + 1, ColorText);  // 그래프영역 외곽선

                Brush brushText = new SolidBrush(ColorText);

                if (Tools.IsLangKorean())                            //여기서부터 상단바 값제목 색상
                    buf = "목표전력";
                else if (Tools.IsLangJapanese())
                    buf = "目標電力";
                else if (Tools.IsLangChinese())
                    buf = "目标电力";
                else if (Tools.IsLangVietnamese())
                    buf = "Chỉ tiêu Điện";
                else
                    buf = "Target power";
                SafeException.SafeDrawString(g, buf, lfLogFont, brushText, 6 + work.cxChar * 1, 8);

                if (Tools.IsLangKorean())
                    buf = "예측전력";
                else if (Tools.IsLangJapanese())
                    buf = "予測電力";
                else if (Tools.IsLangChinese())
                    buf = "预测电力";
                else if (Tools.IsLangVietnamese())
                    buf = "Dự đoán Điện";
                else
                    buf = "Prediction Power";

                SafeException.SafeDrawString(g, buf, lfLogFont, brushText, 6 + work.cxChar * 16, 8);

                if (Tools.IsLangKorean())
                    buf = "현재전력";
                else if (Tools.IsLangJapanese())
                    buf = "現在電力";
                else if (Tools.IsLangChinese())
                    buf = "现在电力";
                else if (Tools.IsLangVietnamese())
                    buf = "Điện hiện tại";
                else
                    buf = "Current Power";
                SafeException.SafeDrawString(g, buf, lfLogFont, brushText, 6 + work.cxChar * 31, 8);

                if (Tools.IsLangKorean())
                    buf = "경과시간";
                else if (Tools.IsLangJapanese())
                    buf = "経過時間";
                else if (Tools.IsLangChinese())
                    buf = "超时";
                else if (Tools.IsLangVietnamese())
                    buf = "Thời gian trôi qua";
                else
                    buf = "Elapsed time";
                SafeException.SafeDrawString(g, buf, lfLogFont, brushText, 6 + work.cxChar * 46, 8);

                if (Tools.IsLangKorean())
                    buf = "부하상태";
                else if (Tools.IsLangJapanese())
                    buf = "負荷状態";
                else if (Tools.IsLangChinese())
                    buf = "负载状态";
                else if (Tools.IsLangVietnamese())
                    buf = "Trạng thái Beaker";
                else
                    buf = "Breaker Status";
                SafeException.SafeDrawString(g, buf, lfLogFont, brushText, 6 + work.cxChar * 61, 8);

                if (Tools.IsLangKorean())
                {
                    if (value_remain > 0) buf = "여유전력";
                    else buf = "초과전력";
                }
                else if (Tools.IsLangJapanese())
                {
                    if (value_remain > 0) buf = "余裕電力";
                    else buf = "超過電力";
                }
                else if (Tools.IsLangChinese())
                {
                    if (value_remain > 0) buf = "余裕电力";
                    else buf = "超过电力";
                }
                else if (Tools.IsLangVietnamese())
                {
                    if (value_remain > 0) buf = "Lương điện còn lại";
                    else buf = "Excessed Power";
                }
                else
                {
                    if (value_remain > 0) buf = "Remain Power";
                    else buf = "excessed Power";
                }

                SafeException.SafeDrawString(g, buf, lfLogFont, brushText, 6 + work.cxChar * 76, 8);

                //TagAiClass ai = TagLib.GetStructAI(item.tagTarget.tag, ref item.tagTarget.tag_pos);

                //status 수치            
                buf = String.Format("{0:F0} {1}", item.fTargetValue, ai.unit);
                DrawBorderText(g, 6 + work.cxChar * 1, 9 + work.cyChar, 6 + work.cxChar * 15, 11 + work.cyChar * 2, buf, ColorTargetValue);

                buf = String.Format("{0:F0} {1}", item.fValueForecast, ai.unit);
                DrawBorderText(g, 6 + work.cxChar * 16, 9 + work.cyChar, 6 + work.cxChar * 30, 11 + work.cyChar * 2, buf, item.fValueForecast > item.fTargetValue ? ColorExValue : ColorPreValue);

                buf = String.Format("{0:F0} {1}", item.fValueKwh, ai.unit);
                DrawBorderText(g, 6 + work.cxChar * 31, 9 + work.cyChar, 6 + work.cxChar * 45, 11 + work.cyChar * 2, buf, ColorStatusValue);

                buf = String.Format("{0}:{1:00}", item.nCurrentSec / 60, item.nCurrentSec % 60);
                DrawBorderText(g, 6 + work.cxChar * 46, 9 + work.cyChar, 6 + work.cxChar * 60, 11 + work.cyChar * 2, buf, ColorStatusValue);

                if (item.bUseIsolation == 0)
                {
                    if (Tools.IsLangKorean())
                        buf = "사용안함";
                    else if (Tools.IsLangJapanese())
                        buf = "無効";
                    else if (Tools.IsLangChinese())
                        buf = "禁用";
                    else
                        buf = "Not used";
                }
                else
                {
                    TagDiClass di = TagLib.GetStructDI(item.tagEcho.tag, ref item.tagEcho.tag_pos);

                    buf = TagUtil.GetDigitalStatusString(di, di.curr);
                }

                DrawBorderText(g, 6 + work.cxChar * 61, 9 + work.cyChar, 6 + work.cxChar * 75, 11 + work.cyChar * 2, buf, ColorStatusValue);

                buf = String.Format("{0:F0} {1}", Math.Abs(value_remain), ai.unit);
                DrawBorderText(g, 6 + work.cxChar * 76, 9 + work.cyChar, 6 + work.cxChar * 90, 11 + work.cyChar * 2, buf, ColorStatusValue);
            }
            DisplayGraphGuide(g);
            DisplayGraph(g, nLineThickTarget);
        }

        private void DrawValueItem(Graphics g, string key, string value,
   int textX, ref int textY, int boxX, ref int boxY,
   int boxWidth, int boxHeight, int spacing, Color valueColor)
        {
            string label;
            // 다국어 처리
            if (key == "목표전력")
            {
                if (Tools.IsLangKorean())
                    label = "목표전력";
                else if (Tools.IsLangJapanese())
                    label = "目標電力";
                else if (Tools.IsLangChinese())
                    label = "目标电力";
                else if (Tools.IsLangVietnamese())
                    label = "Chỉ tiêu Điện";
                else
                    label = "Target power";
            }
            else if (key == "예측전력")
            {
                if (Tools.IsLangKorean())
                    label = "예측전력";
                else if (Tools.IsLangJapanese())
                    label = "予測電力";
                else if (Tools.IsLangChinese())
                    label = "预测电力";
                else if (Tools.IsLangVietnamese())
                    label = "Dự đoán Điện";
                else
                    label = "Prediction Power";
            }
            else if (key == "현재전력")
            {
                if (Tools.IsLangKorean())
                    label = "현재전력";
                else if (Tools.IsLangJapanese())
                    label = "現在電力";
                else if (Tools.IsLangChinese())
                    label = "现在电力";
                else if (Tools.IsLangVietnamese())
                    label = "Điện hiện tại";
                else
                    label = "Current Power";
            }
            else if (key == "경과시간")
            {
                if (Tools.IsLangKorean())
                    label = "경과시간";
                else if (Tools.IsLangJapanese())
                    label = "経過時間";
                else if (Tools.IsLangChinese())
                    label = "超时";
                else if (Tools.IsLangVietnamese())
                    label = "Thời gian trôi qua";
                else
                    label = "Elapsed time";
            }
            else if (key == "부하상태")
            {
                if (Tools.IsLangKorean())
                    label = "부하상태";
                else if (Tools.IsLangJapanese())
                    label = "負荷状態";
                else if (Tools.IsLangChinese())
                    label = "负载状态";
                else if (Tools.IsLangVietnamese())
                    label = "Trạng thái Beaker";
                else
                    label = "Breaker Status";
            }
            else if (key == "여유전력" || key == "초과전력")
            {
                if (Tools.IsLangKorean())
                {
                    label = key == "여유전력" ? "여유전력" : "초과전력";
                }
                else if (Tools.IsLangJapanese())
                {
                    label = key == "여유전력" ? "余裕電力" : "超過電力";
                }
                else if (Tools.IsLangChinese())
                {
                    label = key == "여유전력" ? "余裕电力" : "超过电力";
                }
                else if (Tools.IsLangVietnamese())
                {
                    label = key == "여유전력" ? "Lương điện còn lại" : "Excessed Power";
                }
                else
                {
                    label = key == "여유전력" ? "Remain Power" : "excessed Power";
                }
            }
            else
            {
                label = key;  // 기본값
            }
            Brush brushText = new SolidBrush(ColorText);  //20250211
            SafeException.SafeDrawString(g, label, lfLogFont, brushText, textX, textY);   //수치 이름.
            DrawBorderText(g, boxX, boxY, boxX + boxWidth, boxY + boxHeight, value, valueColor);  //수치 + 박스

            textY += spacing;
            boxY += spacing;
        }


		//Bitmap bitmapClient = null;

		private void FormDemandControlChild_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			//if(bitmapClient == null || ClientRectangle.Width != bitmapClient.Width || ClientRectangle.Height != bitmapClient.Height)
			//	bitmapClient = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height, e.Graphics);

            Graphics g = e.Graphics;// OnPaintBitmap.CreateGraphics(e.Graphics, this.ClientRectangle.Width, this.ClientRectangle.Height);

            
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingMode = CompositingMode.SourceOver;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

			DisplayAll(g);

			//OnPaintBitmap.DrawImageUnscaled(e.Graphics, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
		}

		public void SetDemandName(string demand_name)
		{
			int l;
			FUNCTION_BLOCK_DEMAND_CONTROL item;

			sDemandName = demand_name;

			for(l = 0; l < DemandControl.blockDemandControl.Count; l++) 
			{
				item = (FUNCTION_BLOCK_DEMAND_CONTROL)DemandControl.blockDemandControl[l];
				if(item.title == demand_name) 
				{
					work.nControlNo = l;
					return;
				}
			}	
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
		} 

		public void SetLineThickTarget(int thick) 
		{ 
			nLineThickTarget = thick; 
			if(nLineThickTarget < 1)	nLineThickTarget = 1;
		}

        public void InvalidateByNumber(int no)
        {
            if (work.nControlNo == no)
            {
                Invalidate();
            }
        }
	}

	class WORK_DEMAND_CHILD
	{
		public int	gx1=0;
		public int	gy1=0;
		public int	gx2=0;
		public int	gy2=0;
		public int	cxChar=0;
		public int	cyChar=0;
		public int   nControlNo = 0;
		public int   border_height=0;
		public RECT	rTarget = new RECT();
	} 
}

