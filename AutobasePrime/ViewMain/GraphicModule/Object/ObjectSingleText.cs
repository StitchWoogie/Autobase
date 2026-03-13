using System;
using AutoLib;
using NetTools.OldDefine;
using System.Drawing;
using NetTools;
using System.Windows.Forms;
using AutoLibLocal;
using static AutoLibLocal.LanguageManager;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsSingleText
	{
		public string	text;
		public Color	textColor;
	}

	/// <summary>
	/// Summary description for ObjectSingleText.
	/// </summary>
	[Serializable]
	public class ObjectSingleText : ObjectExpand
	{
		ObjectArgsSingleText objArgs;

        // 다국어 적용된 텍스트 캐시
        private string translatedText = string.Empty;

        public ObjectArgsSingleText ObjectArgs 
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

		public ObjectSingleText(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsSingleText args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.SingleText;
			objArgs = args;
			TextColor = args.textColor;
            //SetText(args.text);
            LanguageManager.EventLanguageChanged += OnLanguageChanged;
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

        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			Font font = MakeFont();
			StringFormat format = new StringFormat();
			Brush brush = new SolidBrush(RunColorText);

			format.Alignment = StringAlignment.Near;
			//format.LineAlignment = StringAlignment.Center;
            
            SafeException.SafeDrawString(g, GetDisplayText(), font, brush, x1, y1, format);
		}

		// Text의 값을 바꾸었거나 폰트를 바꾸었을때는 사각형의 오른쪽 아래의 크기만을 다시 계산하여야 한다.
		public void RecalcRectSize(Graphics g)
		{
			Font font = MakeFont100();
			SizeF size = g.MeasureString(GetDisplayText(), font); 
			nRight = (int)(nLeft+size.Width-1);
			nBottom = (int)(nTop+size.Height-1);
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			SaveObjectItem.TextColor(writer, GetTextColor());
			SaveObjectItem.String(writer, objArgs.text);
			ObjectSaveFont(writer);
		}

		public override void UpdateZone(Form form, int x1, int y1, int x2, int y2)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);
			
			if(Math.Abs(x2-x1) == Math.Abs(nRight-nLeft)) 
			{
                base.UpdateZone(form, x1, y1, x2, y2);
				return;
			}

			float size = logFont.lfHeight;

			float gab = 1.0f;
			Font  font = new Font(logFont.lfFaceName, size, logFont.style);

			Graphics g = form.CreateGraphics();

			if(Math.Abs(x2-x1) < Math.Abs(nRight-nLeft)) // 작아졌다.
			{
				while(true) 
				{
					size -= gab;
					if(size < 1) 
					{
						size += gab;
						break;	
					}

					font = new Font(logFont.lfFaceName, size, logFont.style);
					SizeF sizef = g.MeasureString(GetDisplayText(), font);

					if(sizef.Width <= Math.Abs(x2-x1))	break;
				}
			}
			else	// 커졌다.
			{
				while(true) 
				{
					size += gab;
					if(size > 1000) 
					{
						size -= gab;
						break;	
					}

					font = new Font(logFont.lfFaceName, size, logFont.style);
					SizeF sizef = g.MeasureString(GetDisplayText(), font);

					if(sizef.Width >= Math.Abs(x2-x1))	break;
				}
			}

			logFont.lfHeight = size;

			nLeft = x1;
			nTop = y1;

			RecalcRectSize(g);
		}

        protected override void OnObjectSetText(string text)
        {
            objArgs.text = text;

            objCommonProperty.form.Invalidate();    // 텍스트의 크기가 얼마인지 계산하는 것보다는 간단하게 모두 무효화 하는것이 편하다. 2013-5-15 추가
        }

        public override string GetObjectMainTitle()
        {
            return objArgs.text;
        }

        public override void EditRotateRight(int nx1, int ny1, int nx2, int ny2)
        {
            if (nx1 > nx2) Tools.Temp(ref nx1, ref nx2);
            if (ny1 > ny2) Tools.Temp(ref ny1, ref ny2);

            nLeft = nx1;
            nTop = ny1;

            Graphics g = objCommonProperty.form.CreateGraphics();
            RecalcRectSize(g); 

            //base.EditRotateRight(nx1, ny1, nx2, ny2);
        }

        public override void EditRotateLeft(int nx1, int ny1, int nx2, int ny2)
        {
            if (nx1 > nx2) Tools.Temp(ref nx1, ref nx2);
            if (ny1 > ny2) Tools.Temp(ref ny1, ref ny2);

            nLeft = nx1;
            nTop = ny1;

            Graphics g = objCommonProperty.form.CreateGraphics();
            RecalcRectSize(g); 

            //base.EditRotateLeft(nx1, ny1, nx2, ny2);
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

            Brush brush = new SolidBrush(RunColorText);

            SafeException.SafeDrawString(g, GetDisplayText(), font, brush, 0, 0);
        }

        public override void Dispose()
        {
            LanguageManager.EventLanguageChanged -= OnLanguageChanged; //251031 PSU 추가

            base.Dispose();
        }
    }
}

