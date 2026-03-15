using System;
using AutoLib;
using NetTools.OldDefine;
using System.Drawing;
using NetTools;
using AutoLibLocal;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using static AutoLibLocal.LanguageManager;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsText
	{
		public string text;
		public Color textColor;
        public TEXT_ALIGN align = new TEXT_ALIGN();
        public bool formatFlagDirectionVertical;
        public bool formatFlagNoWrap;
	}
	/// <summary>
	/// Summary description for ObjectText.
	/// </summary>
	[Serializable]
	public class ObjectText : ObjectExpand
	{
		ObjectArgsText objArgs;

        [NonSerialized]
        private StringFormat cachedFormat = null;
        [NonSerialized]
        private SolidBrush cachedBrush = null;
        [NonSerialized]
        private Color cachedTextColor = Color.Empty;
        [NonSerialized]
        private TEXT_ALIGN cachedAlign = new TEXT_ALIGN();
        [NonSerialized]
        private bool cachedFormatFlags = false;

        // 다국어 지원을 위한 원본 텍스트 저장 (Serializable)
       // private string originalText = string.Empty;

        // 다국어 적용된 텍스트 캐시
        private string translatedText = string.Empty;

        public ObjectArgsText ObjectArgs 
		{
			set 
			{
				objArgs = value;
                UpdateTranslatedText();

            }
			get 
			{
				return objArgs;
			}
		}

		public ObjectText(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsText args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.Text;
			objArgs = args;
			TextColor = args.textColor;
			SetText(args.text);

            LanguageManager.EventLanguageChanged += OnLanguageChanged;
        }

		void SetText(string str)
		{
            if (str == null)
            {
                str = string.Empty;
            }

            // 원본 텍스트 저장
            //originalText = str;
            objArgs.text = str;

            // 다국어 텍스트로 변환
            UpdateTranslatedText();
        }

        /// <summary>
        /// 언어 변경 이벤트 핸들러
        /// </summary>
        /// <param name="newLanguageCode">새로운 언어 코드</param>
        private void OnLanguageChanged(string newLanguageCode)
        {
            // 언어가 변경되면 텍스트 업데이트
            UpdateTranslatedText();
        }

        /// <summary>
        /// 다국어 텍스트 업데이트
        /// </summary>
        private void UpdateTranslatedText()
        {
            try
            {
                var langManager = LanguageManager.Instance;
                if (langManager != null)
                {
                    // @{key} 형식을 파싱하여 다국어 텍스트로 변환
                    translatedText = langManager.ParseText(objArgs.text);
                }
                else
                {
                    // LanguageManager가 없으면 원본 텍스트 사용
                    translatedText = objArgs.text;
                }
            }
            catch
            {
                // 예외 발생 시 원본 텍스트 사용
                translatedText = objArgs.text;
            }
        }

        /// <summary>
        /// 현재 표시할 텍스트 가져오기 (다국어 적용됨)
        /// </summary>
        /// <returns>표시할 텍스트</returns>
        private string GetDisplayText()
        {
            // 변환된 텍스트 반환 (없으면 원본 반환)
            return string.IsNullOrEmpty(translatedText) ? objArgs.text : translatedText;
        }


        private void UpdateTextRenderingCache()
        {
            // Brush 캐시 업데이트
            if (cachedTextColor != RunColorText)
            {
                cachedBrush?.Dispose();
                cachedBrush = new SolidBrush(RunColorText);
                cachedTextColor = RunColorText;
            }

            // StringFormat 캐시 업데이트
            bool needFormatUpdate = cachedFormat == null ||
                                   cachedAlign.x != objArgs.align.x ||
                                   cachedAlign.y != objArgs.align.y ||
                                   cachedFormatFlags != (objArgs.formatFlagDirectionVertical || objArgs.formatFlagNoWrap);

            if (needFormatUpdate)
            {
                cachedFormat?.Dispose();
                cachedFormat = new StringFormat();

                // Alignment 설정
                if (objArgs.align.x == 1)
                    cachedFormat.Alignment = StringAlignment.Center;
                else if (objArgs.align.x == 2)
                    cachedFormat.Alignment = StringAlignment.Far;
                else
                    cachedFormat.Alignment = StringAlignment.Near;

                if (objArgs.align.y == 1)
                    cachedFormat.LineAlignment = StringAlignment.Center;
                else if (objArgs.align.y == 2)
                    cachedFormat.LineAlignment = StringAlignment.Far;
                else
                    cachedFormat.LineAlignment = StringAlignment.Near;

                // FormatFlags 설정 (기존 플래그 초기화 후 설정)
                cachedFormat.FormatFlags = StringFormatFlags.NoClip; // 기본값으로 초기화

                if (objArgs.formatFlagDirectionVertical)
                    cachedFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
                if (objArgs.formatFlagNoWrap)
                    cachedFormat.FormatFlags |= StringFormatFlags.NoWrap;

                cachedAlign = new TEXT_ALIGN { x = objArgs.align.x, y = objArgs.align.y };
                cachedFormatFlags = objArgs.formatFlagDirectionVertical || objArgs.formatFlagNoWrap;
            }
        }

        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && signage != null && signage.bStart)
            {
                signage.Paint(g);
                return;
            }

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            Font font = MakeFont();
            //StringFormat format = new StringFormat();
            //Brush brush = new SolidBrush(RunColorText);

            //if(objArgs.align.x == 1)        format.Alignment = StringAlignment.Center;
            //else if (objArgs.align.x == 2)  format.Alignment = StringAlignment.Far;
            //else                            format.Alignment = StringAlignment.Near;

            //if (objArgs.align.y == 1)       format.LineAlignment = StringAlignment.Center;
            //else if (objArgs.align.y == 2)  format.LineAlignment = StringAlignment.Far;
            //else                            format.LineAlignment = StringAlignment.Near;

            //if (objArgs.formatFlagDirectionVertical)
            //    format.FormatFlags |= StringFormatFlags.DirectionVertical;
            //if (objArgs.formatFlagNoWrap)
            //    format.FormatFlags |= StringFormatFlags.NoWrap;

            //Rectangle r = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
            //SafeException.SafeDrawString(g, objArgs.text, font, brush, r, format);

            // 텍스트 렌더링 관련 캐시 업데이트
            UpdateTextRenderingCache();

            Rectangle r = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
            SafeException.SafeDrawString(g, GetDisplayText(), font, cachedBrush, r, cachedFormat);

        }

        protected override void DisplayPreviewObject(Graphics g, int x1, int y1, int x2, int y2, int thick)
        {
            Font font;

            int height = 10;

            try
            {
                font = new Font(logFont.lfFaceName, height, logFont.style);
            }
            catch
            {
                font = new Font("Arial", height, logFont.style);
            }

            using (Brush brush = new SolidBrush(RunColorText))
                SafeException.SafeDrawString(g, GetDisplayText(), font, brush, 0, 0);

            font.Dispose();
         }

		public override void ObjectSave(CommaTextWriter writer)
		{
			SaveObjectItem.TextColor(writer, GetTextColor());

            // 여러 줄일 경우 나누어서 저장한다. 9.3.9 부터 지원
            System.IO.StringReader reader = new System.IO.StringReader(objArgs.text);
            string one_line;
            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                SaveObjectItem.String(writer, one_line);
            }

            SaveObjectItem.TextAlign(writer, objArgs.align);
            writer.WriteLine("\tStringOption,{0},{1},", objArgs.formatFlagDirectionVertical, objArgs.formatFlagNoWrap);

			ObjectSaveFont(writer);
		}

        protected override void OnObjectSetText(string text)
        {
            objArgs.text = text;

            UpdateTranslatedText(); //251031 PSU 추가

            if (signage != null)
            {
                if (signage.arrayCaption.Count == 0)
                {
                    signage.Add(GetDisplayText(), MakeFont(), RunColorText);
                }
                else
                {
                    signage.arrayCaption[0].sText = GetDisplayText();
                }
                signage.ClearBitmap();
            }

            InvalidateObject();
        }

        [NonSerialized]
        SignageClass signage = null;

        public override async Task EventTimerObject(System.Windows.Forms.Form form)
        {
            if (signage == null) return;

            signage.Timer(form, this);

            await Task.CompletedTask;
        }

        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            if (signage != null && signage.bStart)
            {
                signage.SetZone(x1, y1, x2, y2);
            }
        }

        void AllocSignage()
        {
            if (signage == null)
            {
                signage = new SignageClass();

                if (objArgs.align.y == 1) signage.LineAlignment = StringAlignment.Center;
                else if (objArgs.align.y == 2) signage.LineAlignment = StringAlignment.Far;
                else signage.LineAlignment = StringAlignment.Near;
            }
        }

        protected override bool ExecuteClassName_Signage(string command, out object retn_value, params object[] args)
        {
            if (command == "SignageStart")
            {
                AllocSignage();

                // 위치가 초기화 되지 않았으므로 초기화 해준다.
                int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                GetViewZone(ref x1, ref y1, ref x2, ref y2);
                signage.SetZone(x1, y1, x2, y2);

                if (signage.arrayCaption.Count == 0)
                {
                    signage.Add(GetDisplayText(), MakeFont(), RunColorText);
                }

                signage.bStart = true;
            }
            else if (command == "SignageStop")
            {
                AllocSignage();

                signage.bStart = false;
            }
            else if (command == "SignageSetSpeed")
            {
                AllocSignage();

                int speed = (int)args[1];

                signage.nPixelPerSec = speed;
            }
            else if (command == "SignageSetBackColor")
            {
                AllocSignage();

                int color = (int)args[1];

                signage.backColor = Color.FromArgb(color);
            }
            else if (command == "SignageListAdd")
            {
                AllocSignage();

                string text = (string)args[1];

                Font font = MakeFont();

                signage.Add(text, font, RunColorText);
            }
            else if (command == "SignageListClear")
            {
                AllocSignage();

                signage.Clear();
            }
            else
            {
                retn_value = 0;
                return false;
            }

            retn_value = 1;
            return false;

            // 함수형이 return값을 반환하는 형태만 return true이다.
        }

        public override void Dispose()
        {
            LanguageManager.EventLanguageChanged -= OnLanguageChanged; //251031 PSU 추가

            // 텍스트 렌더링 캐시 해제
            cachedFormat?.Dispose();
            cachedBrush?.Dispose();

            if (signage != null)
                signage.ClearBitmap();

            base.Dispose();
        }
	}

    class SignageClass
    {
        public bool bStart = false;
        public Bitmap bitmap = null;
        int nOldTickCount = System.Environment.TickCount;
        public int nPixelPerSec = 100;    // 초당 움직이는 픽셀
        double fMoveX = 0;
        public StringAlignment LineAlignment = StringAlignment.Center;

        Rectangle rZone = new Rectangle(0, 0, 100, 100);

        public List<CaptionItem> arrayCaption = new List<CaptionItem>();
        public Color backColor = Color.Transparent;

        public void Add(string text, Font font, Color ct)
        {
            CaptionItem ci = new CaptionItem();
            ci.sText = text;
            ci.font = font;
            ci.colorText = ct;

            arrayCaption.Add(ci);
        }

        public void Clear()
        {
            arrayCaption.Clear();
        }

        public void ClearBitmap()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }

        public void SetZone(int x1, int y1, int x2, int y2)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            rZone = new Rectangle(x1, y1, x2 - x1, y2-y1);
        }

        int nCaptionPos = 0;

        public void Timer(Form form, ObjectExpand obj)
        {
            if (!bStart) return;

            if (arrayCaption.Count == 0) return;

            if (bitmap == null)
            {
                if (arrayCaption.Count == 0) return;

                nCaptionPos %= arrayCaption.Count;

                CaptionItem ci = arrayCaption[nCaptionPos];

                SizeF size;
                using (Graphics gMeasure = form.CreateGraphics())
                    size = gMeasure.MeasureString(ci.sText, ci.font);

                bitmap = new Bitmap((int)size.Width + 10, (int)size.Height);

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    if (backColor.A != 0)
                    {
                        using (Brush bgBrush = new SolidBrush(backColor))
                            g.FillRectangle(bgBrush, 0, 0, size.Width+10, size.Height);
                    }

                    using (Brush brush = new SolidBrush(ci.colorText))
                        g.DrawString(ci.sText, ci.font, brush, 0+5, 0);
                }
            }

            int ticks = System.Environment.TickCount;
            int ms = (ticks - nOldTickCount);

            nOldTickCount = ticks;

            if (ms < 0) // 컴퓨터를 켠지 24.9일이 지났다.
            {

            }
            else
            {
                fMoveX += (double)nPixelPerSec / (double)1000 * ms;

                if (fMoveX > rZone.Width + bitmap.Width)
                {
                    fMoveX = 0;

                    nCaptionPos++;
                    nCaptionPos %= arrayCaption.Count;

                    ClearBitmap();

                    return;
                }

                obj.InvalidateObject();
            }
        }

        public void Paint(Graphics g)
        {
            if (bitmap != null)
            {
                Region save_clip = g.Clip;
                g.Clip = new Region(rZone);

                if (LineAlignment == StringAlignment.Center)
                {
                    int y = rZone.Y + rZone.Height/2 - bitmap.Height/2;
                    g.DrawImageUnscaled(bitmap, rZone.X + rZone.Width - (int)fMoveX, y);
                }
                else if (LineAlignment == StringAlignment.Far)
                {
                    int y = rZone.Y + rZone.Height - bitmap.Height;
                    g.DrawImageUnscaled(bitmap, rZone.X + rZone.Width - (int)fMoveX, y);
                }
                else // Near
                {
                    g.DrawImageUnscaled(bitmap, rZone.X + rZone.Width - (int)fMoveX, rZone.Y);
                }
                g.Clip = save_clip;
            }
        }
    }

    class CaptionItem
    {
        public string sText;
        public Font font;
        public Color colorText;
        //public int nPixelPerSec = 200;
        //public Color colorBack;
    }
}
