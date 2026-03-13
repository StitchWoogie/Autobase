using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using AutoLib;
using NetTools.OldDefine;
using NetTools;
using System.Windows.Forms;
using AutoLibLocal;
using System.Threading.Tasks;

namespace GraphicModule
{
	public enum ButtonDesignType
	{
		Classic3D = 0,
		Modern = 1,
		Soft3D = 2
	}

	[Serializable]
	public class ObjectArgsButtonPublic
	{
		public string sText;
		public Color tcolor;
		public BrushPublic bcolor = new BrushPublic();
        public Color lcolor = Color.Black;  // 이것은 9.5.3 이전 버전에는 없는 기능이므로 디폴트 값(Black)을 사용하도록 했다. 생성시는 디폴트 값은 이것을 수정하지 말고 Insert시 디폴트 값을 변경하도록 한다.
        public int thick = 2;               // 이것은 9.5.3 이전 버전에는 없는 기능이므로 디폴트 값(2)을 사용하도록 했다. 생성시는 디폴트 값은  이것을 수정하지 말고 Insert시 디폴트 값을 변경하도록 한다.
		public ButtonDesignType designType = ButtonDesignType.Classic3D;
		public int radius = 0;
	}

	/// <summary>
	/// Summary description for ObjectButtonPublic.
	/// </summary>
	///
	[Serializable]
	public class ObjectButtonPublic : ObjectExpand
	{
		ObjectArgsButtonPublic objArgs;
		bool bCaptureFlag = false;	// 마우스가 눌러져있는가?
		bool bMouseInFlag = false;	// 마우스가 버턴영역 안에 있는가를 검사.

		static readonly StringFormat CenterFormat = new StringFormat
		{
			Alignment = StringAlignment.Center,
			LineAlignment = StringAlignment.Center,
			FormatFlags = StringFormatFlags.NoWrap
		};

		public string Text
		{
			set { objArgs.sText = value; }
			get { return objArgs.sText; }
		}

		public ButtonDesignType DesignType
		{
			set { objArgs.designType = value; }
			get { return objArgs.designType; }
		}

		public int Radius
		{
			set { objArgs.radius = value; }
			get { return objArgs.radius; }
		}

		public ObjectButtonPublic(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general, ObjectArgsButtonPublic args)
			: base(ocp, rect, eid, lf, general)
		{
			objArgs = args;

			SetTextColor(args.tcolor);
			SetBackColor(args.bcolor);
            SetLineColor(args.lcolor);
            SetBorderThick(args.thick);
		}

		#region Classic3D Rendering

		// 원래 DrawClass.PopRectangle 패턴: Fill → 3D Bevel → 외곽선(LAST)
		// 외곽선을 최후에 그려야 검정색 등 어두운 색에서도 gray bevel에 덮이지 않는다.

        void PopBox(Graphics g, int x1, int y1, int x2, int y2, Color color, int thick)
        {
            // 1. Fill 배경 (외곽선 안쪽 영역)
            DrawClass.gcls(g, x1, y1, x2 - 1, y2 - 1, color);

            // 2. 3D bevel (White=좌상 highlight, Gray=우하 shadow)
            using (Pen penGray = new Pen(Color.FromArgb(0x80, 0x80, 0x80)))
            {
                for (int i = 0; i < thick; i++)
                {
                    g.DrawLine(Pens.White, x1 + 1 + i, y2 - 1 - i, x1 + 1 + i, y1 + 1 + i);
                    g.DrawLine(Pens.White, x1 + 1 + i, y1 + 1 + i, x2 - 1 - i, y1 + 1 + i);

                    g.DrawLine(penGray, x2 - 1 - i, y1 + 1 + i, x2 - 1 - i, y2 - 1 - i);
                    g.DrawLine(penGray, x2 - 1 - i, y2 - 1 - i, x1 + 1 + i, y2 - 1 - i);
                }
            }

            // 3. 외곽선 LAST (thick > 0 일 때만)
            if (thick > 0)
            {
                using (Pen penLine = new Pen(RunColorLine))
                {
                    g.DrawLine(penLine, x1, y1, x2, y1);
                    g.DrawLine(penLine, x2, y1, x2, y2);
                    g.DrawLine(penLine, x2, y2, x1, y2);
                    g.DrawLine(penLine, x1, y2, x1, y1);
                }
            }
        }

        void PopBox(Graphics g, int x1, int y1, int x2, int y2, Brush brush, int thick)
        {
            // 1. Fill 배경 (외곽선 안쪽 영역)
            g.FillRectangle(brush, x1, y1, x2 - x1, y2 - y1);

            // 2. 3D bevel
            using (Pen penGray = new Pen(Color.FromArgb(0x80, 0x80, 0x80)))
            {
                for (int i = 0; i < thick; i++)
                {
                    g.DrawLine(Pens.White, x1 + 1 + i, y2 - 1 - i, x1 + 1 + i, y1 + 1 + i);
                    g.DrawLine(Pens.White, x1 + 1 + i, y1 + 1 + i, x2 - 1 - i, y1 + 1 + i);

                    g.DrawLine(penGray, x2 - 1 - i, y1 + 1 + i, x2 - 1 - i, y2 - 1 - i);
                    g.DrawLine(penGray, x2 - 1 - i, y2 - 1 - i, x1 + 1 + i, y2 - 1 - i);
                }
            }

            // 3. 외곽선 LAST (thick > 0 일 때만)
            if (thick > 0)
            {
                using (Pen penLine = new Pen(RunColorLine))
                {
                    g.DrawLine(penLine, x1, y1, x2, y1);
                    g.DrawLine(penLine, x2, y1, x2, y2);
                    g.DrawLine(penLine, x2, y2, x1, y2);
                    g.DrawLine(penLine, x1, y2, x1, y1);
                }
            }
        }

        void PushBox(Graphics g, int x1, int y1, int x2, int y2, Color color, int thick)
        {
            // 1. Fill
            DrawClass.gcls(g, x1, y1, x2 - 1, y2 - 1, color);

            // 2. 3D bevel (reversed: Gray=좌상, White=우하)
            using (Pen penGray = new Pen(Color.FromArgb(0x80, 0x80, 0x80)))
            {
                for (int i = 0; i < thick; i++)
                {
                    g.DrawLine(penGray, x1 + 1 + i, y2 - 1 - i, x1 + 1 + i, y1 + 1 + i);
                    g.DrawLine(penGray, x1 + 1 + i, y1 + 1 + i, x2 - 1 - i, y1 + 1 + i);

                    g.DrawLine(Pens.White, x2 - 1 - i, y1 + 1 + i, x2 - 1 - i, y2 - 1 - i);
                    g.DrawLine(Pens.White, x2 - 1 - i, y2 - 1 - i, x1 + 1 + i, y2 - 1 - i);
                }
            }

            // 3. 외곽선 LAST (thick > 0 일 때만)
            if (thick > 0)
            {
                using (Pen penLine = new Pen(RunColorLine))
                {
                    g.DrawLine(penLine, x1, y1, x2, y1);
                    g.DrawLine(penLine, x2, y1, x2, y2);
                    g.DrawLine(penLine, x2, y2, x1, y2);
                    g.DrawLine(penLine, x1, y2, x1, y1);
                }
            }
        }

        void PushBox(Graphics g, int x1, int y1, int x2, int y2, Brush brush, int thick)
        {
            // 1. Fill
            g.FillRectangle(brush, x1, y1, x2 - x1, y2 - y1);

            // 2. 3D bevel (reversed)
            using (Pen penGray = new Pen(Color.FromArgb(0x80, 0x80, 0x80)))
            {
                for (int i = 0; i < thick; i++)
                {
                    g.DrawLine(penGray, x1 + 1 + i, y2 - 1 - i, x1 + 1 + i, y1 + 1 + i);
                    g.DrawLine(penGray, x1 + 1 + i, y1 + 1 + i, x2 - 1 - i, y1 + 1 + i);

                    g.DrawLine(Pens.White, x2 - 1 - i, y1 + 1 + i, x2 - 1 - i, y2 - 1 - i);
                    g.DrawLine(Pens.White, x2 - 1 - i, y2 - 1 - i, x1 + 1 + i, y2 - 1 - i);
                }
            }

            // 3. 외곽선 LAST (thick > 0 일 때만)
            if (thick > 0)
            {
                using (Pen penLine = new Pen(RunColorLine))
                {
                    g.DrawLine(penLine, x1, y1, x2, y1);
                    g.DrawLine(penLine, x2, y1, x2, y2);
                    g.DrawLine(penLine, x2, y2, x1, y2);
                    g.DrawLine(penLine, x1, y2, x1, y1);
                }
            }
        }

		#endregion

		#region Safe Brush / Fill Helpers

		/// <summary>
		/// MakePublicBrush 안전 래퍼.
		/// brush_type 1(Solid) → 직접 SolidBrush 생성 (MakePublicBrush 우회).
		/// brush_type 2,3(Gradient) → MakePublicBrush 호출하되, 생성 실패 시 SolidBrush 폴백.
		/// </summary>
		static Brush MakeSafeBrush(BrushPublic pub, int x1, int y1, int x2, int y2)
		{
			if (pub.brush_type == 0)
				return new SolidBrush(Color.Transparent);  // 투명 배경 (Brushes.Transparent는 공유 인스턴스라 using Dispose시 파괴됨)

			if (pub.brush_type != 2 && pub.brush_type != 3)
				return new SolidBrush(pub.basic_color);

			if (x2 - x1 < 4 || y2 - y1 < 4)
				return new SolidBrush(pub.basic_color);

			try
			{
				return ObjectRectangle.MakePublicBrush(pub, x1, y1, x2, y2);
			}
			catch
			{
				return new SolidBrush(pub.basic_color);
			}
		}

        /// <summary>
        /// 배경 Brush를 path 영역에 안전하게 채운다.
        /// LinearGradientBrush는 GDI+ native 레벨에서 ArgumentException을 발생시킬 수 있으므로
        /// 실패 시 basic_color SolidBrush로 폴백 렌더링하여 버튼이 항상 표시되도록 한다.
        /// </summary>
        void FillBackgroundSafe(Graphics g, Brush brush, GraphicsPath path)
        {
            try
            {
                g.FillPath(brush, path);
            }
            catch (ArgumentException)
            {
                using (SolidBrush fb = new SolidBrush(RunColorBack.basic_color))
                    g.FillPath(fb, path);
            }
        }

        #endregion

        #region Rounded Rectangle Helper

        static GraphicsPath CreateRoundRectPath(Rectangle rect, int radius)
		{
			GraphicsPath path = new GraphicsPath();
			if (radius <= 0 || rect.Width < 2 || rect.Height < 2)
			{
				path.AddRectangle(rect);
				return path;
			}
			int d = radius * 2;
			if (d > rect.Width) d = rect.Width;
			if (d > rect.Height) d = rect.Height;
			if (d < 2)
			{
				path.AddRectangle(rect);
				return path;
			}
			path.AddArc(rect.X, rect.Y, d, d, 180, 90);
			path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
			path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
			path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
			path.CloseFigure();
			return path;
		}

		#endregion

		#region Modern / Soft3D Rendering

		void DrawModernButton(Graphics g, int x1, int y1, int x2, int y2, Font font, bool isPressed, bool isHover)
		{
			int w = x2 - x1;
			int h = y2 - y1;
			if (w < 2 || h < 2) return;

			Rectangle rect = new Rectangle(x1, y1, w, h);
			int rad = objArgs.radius;
			int thick = GetBorderThick();

			using (Brush brushback = MakeSafeBrush(RunColorBack, x1, y1, x2, y2))
			using (GraphicsPath path = CreateRoundRectPath(rect, rad))
			{
				SmoothingMode oldMode = g.SmoothingMode;
				g.SmoothingMode = SmoothingMode.AntiAlias;

                FillBackgroundSafe(g, brushback, path);

                if (isPressed)
				{
					using (SolidBrush overlay = new SolidBrush(Color.FromArgb(25, 0, 0, 0)))
						g.FillPath(overlay, path);
				}
				else if (isHover)
				{
					using (SolidBrush overlay = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
						g.FillPath(overlay, path);
				}

				if (thick > 0)
				{
					using (Pen pen = new Pen(RunColorLine, thick))
						g.DrawPath(pen, path);
				}

				g.SmoothingMode = oldMode;
			}

			Rectangle r = isPressed
				? new Rectangle(x1 + 1, y1 + 1, w, h)
				: new Rectangle(x1, y1, w, h);

			using (SolidBrush textBrush = new SolidBrush(RunColorText))
				DrawClass.DrawTextClip(g, objArgs.sText, font, textBrush, r, CenterFormat);
		}

		void DrawSoft3DButton(Graphics g, int x1, int y1, int x2, int y2, Font font, bool isPressed, bool isHover)
		{
			int w = x2 - x1;
			int h = y2 - y1;
			if (w < 2 || h < 2) return;

			int rad = objArgs.radius;
			int thick = GetBorderThick();
			int shadowOffset = Math.Max(2, h / 20);

            SmoothingMode oldMode = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			// Shadow (not drawn when pressed)
			if (!isPressed)
			{
				Rectangle shadowRect = new Rectangle(x1 + shadowOffset, y1 + shadowOffset, w, h);
                int alpha = Math.Min(80, 30 + h / 4);

                using (GraphicsPath shadowPath = CreateRoundRectPath(shadowRect, rad))
				using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
				{
					g.FillPath(shadowBrush, shadowPath);
				}
			}

			Rectangle rect = new Rectangle(x1, y1, w, h);

			using (Brush brushback = MakeSafeBrush(RunColorBack, x1, y1, x2, y2))
			using (GraphicsPath path = CreateRoundRectPath(rect, rad))
			{
                FillBackgroundSafe(g, brushback, path);

                if (isPressed)
				{
					using (SolidBrush overlay = new SolidBrush(Color.FromArgb(25, 0, 0, 0)))
						g.FillPath(overlay, path);
				}
				else if (isHover)
				{
					using (SolidBrush overlay = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
						g.FillPath(overlay, path);
				}

				if (thick > 0)
				{
					using (Pen pen = new Pen(RunColorLine, thick))
						g.DrawPath(pen, path);
				}
			}

			g.SmoothingMode = oldMode;

			Rectangle r = isPressed
				? new Rectangle(x1 + 1, y1 + 1, w, h)
				: new Rectangle(x1, y1, w, h);

			using (SolidBrush textBrush = new SolidBrush(RunColorText))
				DrawClass.DrawTextClip(g, objArgs.sText, font, textBrush, r, CenterFormat);
		}

		#endregion

        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (x2 - x1 < 2 || y2 - y1 < 2) return;  // 너무 작으면 렌더링 안함 (GraphicsPath/Brush에 최소 2px 필요)

            Font font = MakeFont();

			ButtonDesignType dt = objArgs.designType;

			switch (dt)
			{
				case ButtonDesignType.Modern:
				case ButtonDesignType.Soft3D:
				{
					bool isPressed = (TotalConfig.defineMode == EnumDefineMode.MODE_RUN) && bCaptureFlag && bMouseInFlag;
					bool isHover = (TotalConfig.defineMode == EnumDefineMode.MODE_RUN) && !bCaptureFlag && bMouseInFlag;

					if (dt == ButtonDesignType.Modern)
						DrawModernButton(g, x1, y1, x2, y2, font, isPressed, isHover);
					else
						DrawSoft3DButton(g, x1, y1, x2, y2, font, isPressed, isHover);
					break;
				}
				default:
					DrawClassic3D(g, x1, y1, x2, y2, font);
					break;
			}
        }

		void DrawClassic3D(Graphics g, int x1, int y1, int x2, int y2, Font font)
		{
			bool isPressed = (TotalConfig.defineMode == EnumDefineMode.MODE_RUN) && bCaptureFlag && bMouseInFlag;

			// PopBox/PushBox는 x2,y2를 inclusive로 취급 (DrawClass.PopBox/gcls 원래 패턴)
			using (Brush brushback = MakeSafeBrush(RunColorBack, x1, y1, x2, y2))
			{
				if (isPressed)
					PushBox(g, x1, y1, x2, y2, brushback, GetBorderThick());
				else
					PopBox(g, x1, y1, x2, y2, brushback, GetBorderThick());
			}

			Rectangle r = isPressed
				? new Rectangle(x1 + 1, y1 + 1, x2 - x1, y2 - y1)
				: new Rectangle(x1, y1, x2 - x1, y2 - y1);

			using (SolidBrush textBrush = new SolidBrush(RunColorText))
				DrawClass.DrawTextClip(g, objArgs.sText, font, textBrush, r, CenterFormat);
		}

        async Task RunMouseScript(Form form, ScriptClass script)
        {
            script.SetHandOperation();  // 수동으로 출력한다.
            await script.RunAsync(form, this);
            if (script.IsError())
            {
                string message;
                message = script.GetError();
                MessageDisplay.Show(message);
            }
        }

		public override async Task<bool> WmLeftButtonDown(System.Windows.Forms.Form form, MouseEventArgs e)
		{
            if (!CheckResponseOnVisible()) return false;

			int sx, sy;
			int x1=0, x2=0, y1=0, y2=0;

            GetMousePositionByRotate(e, out sx, out sy);

			GetViewZone(ref x1, ref y1, ref x2, ref y2);

			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			if(sx < x1 || sy < y1 || sx > x2 || sy > y2) 	return false;

            if (eID.active == 1 && expandScript.scriptMouseLeftDown != null)
            {
                await RunMouseScript(form, expandScript.scriptMouseLeftDown);
            }

			InvalidateObject(form);
			bCaptureFlag = true;
			bMouseInFlag = true;		// 마우스가 버턴속에 있다.
			form.Capture = true;

			return true;
		}

		public override async Task<bool> WmLeftButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)
		{
			if(bCaptureFlag == false)	return false;
			bCaptureFlag = false;
			form.Capture = false;

            if (eID.active == 1 && expandScript.scriptMouseLeftUp != null)
            {
                await RunMouseScript(form, expandScript.scriptMouseLeftUp);
                InvalidateObject(form);
            }

			if(bMouseInFlag == false)	return false;

			// bMouseInFlag 유지 → pressed에서 hover 상태로 자연 전환
			InvalidateObject(form);

			return true;
		}

		public override async Task<bool> WmMouseMove(System.Windows.Forms.Form form, MouseEventArgs e)
		{
			await base.WmMouseMove(form, e);	// Button 도 MouseZone Display를  사용한다.

			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return false;

			int sx, sy;
			int x1=0, x2=0, y1=0, y2=0;
			bool mousein;

            GetMousePositionByRotate(e, out sx, out sy);

			GetViewZone(ref x1, ref y1, ref x2, ref y2);

			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			if(sx >= x1 && sy >= y1 && sx <= x2 && sy <= y2) 	mousein = true;
			else												mousein = false;

			if(bMouseInFlag == mousein)
			{
				if (bCaptureFlag) return true;
				return false;
			}

			bMouseInFlag = mousein;
			InvalidateObject(form);

			if (bCaptureFlag) return true;
			return false;
		}

        public override string GetObjectMainTitle()
        {
            return objArgs.sText;
        }
	}
}
