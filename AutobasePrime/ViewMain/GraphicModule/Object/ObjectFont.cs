using AutoLibLocal;
using NetTools;
using NetTools.OldDefine;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;


namespace GraphicModule
{
    /// <summary>
    /// Summary description for ObjectFont.
    /// 기존의 Font 객체는 GDI 누수 문제가 있을 수 있어 캐싱 방식 도입. 20260220 PSU
    /// 매 프레임 Font 생성 -> 조건 변경 시만 생성 (성능 향상)
    /// 초당 수십 개 생성, GC 의존 -> 상태 변경 시에만 재생성 (GDI 핸들/장시간 실행 안정성 확보)
    /// </summary>
    /// 
    [Serializable]
	public class ObjectFont : ObjectPublic
	{
		protected LOGFONT  logFont;

        // 폰트 캐싱 관련 필드들
        [NonSerialized]
        private Font cachedFont = null;
        [NonSerialized]
        private float lastCalculatedFontSize = -1;
        [NonSerialized]
        private string lastFontName = null;
        [NonSerialized]
        private FontStyle lastFontStyle = FontStyle.Regular;
        [NonSerialized]
        private int lastScreenSizeX = -1;
        [NonSerialized]
        private int lastScreenSizeY = -1;
        [NonSerialized]
        private int lastModuleSizeX = -1;
        [NonSerialized]
        private int lastModuleSizeY = -1;
        [NonSerialized]
        private int lastObjectShowMethod = -1;
        [NonSerialized]
        private EnumDefineMode lastDefineMode = (EnumDefineMode)(-1);

        public ObjectFont(ObjectCommonProperty ocp, LOGFONT font) : base(ocp)
		{
			//
			// TODO: Add constructor logic here
			//
			if(font == null) 
			{	
				logFont = new LOGFONT();

				if(NetTools.Tools.IsLangKorean()) 
				{
                    if(Environment.OSVersion.Version.Major >= 6)
                        logFont.lfFaceName = "Malgun Gothic";
                    else
					    logFont.lfFaceName = "Gulim";

					logFont.lfHeight = 9;
				}
				else if(NetTools.Tools.IsLangJapanese()) 
				{
					logFont.lfFaceName = "MS UI Gothic";
					logFont.lfHeight = 9;
				}
				else if(NetTools.Tools.IsLangChinese()) 
				{
					logFont.lfFaceName = "SimSun";
					logFont.lfHeight = 9;
				}
				else 
				{
					logFont.lfFaceName = "Tahoma";
					logFont.lfHeight = 9;  
				}
			}
			else 
			{
				logFont = font;

				if(font.lfHeight <= 0)		font.lfHeight = 9;
				if(font.lfFaceName == null) 
				{
                    if (NetTools.Tools.IsLangKorean())
                    {
                        if (Environment.OSVersion.Version.Major >= 6)
                            font.lfFaceName = "Malgun Gothic";
                        else
                            font.lfFaceName = "Gulim";
                    }
                    else if (NetTools.Tools.IsLangJapanese())
                        font.lfFaceName = "MS UI Gothic";
                    else if (NetTools.Tools.IsLangChinese())
                        font.lfFaceName = "SimSun";
                    else
                        font.lfFaceName = "Tahoma";
				}
			}
		}

        /// <summary>
        /// 캐시를 무효화합니다. LOGFONT가 변경되었을 때 호출해야 합니다.
        /// </summary>
        protected void InvalidateFontCache()
        {
            // Dispose하지 않음 - Control 등에서 참조 중일 수 있음. GC에 위임.
            cachedFont = null;
        }


        public void SetLogFont(LOGFONT lf)
		{
			if(lf == null)	return;

			logFont = lf;
            InvalidateFontCache(); // 폰트 변경시 캐시 무효화
        }

        public LOGFONT GetLogFont()
        {
            LOGFONT lf = new LOGFONT();
            lf.lfFaceName = logFont.lfFaceName;
            lf.style = logFont.style;
            lf.lfHeight = logFont.lfHeight;

            return lf;
        }

        //public Font MakeFont()
        //{
        //	Font font;
        //	float height;

        //          // 윈도우에 비례하지 않게 확대/축소할 때에는 폰트는 X/Y 중 작은 확대 비율에 따라간다. 2020-9-15 수정
        //          if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && cObjectShowMethod == 2)
        //          {
        //              double x = (double)nScreenSizeX / (double)nModuleSizeX;
        //              double y = (double)nScreenSizeY / (double)nModuleSizeY;

        //              if (x < y)
        //                  height = GetViewSizeX((int)logFont.lfHeight);
        //              else
        //                  height = GetViewSizeY((int)logFont.lfHeight);
        //          }
        //          else
        //          {
        //              height = GetViewSize((int)logFont.lfHeight);
        //          }

        //	if(height == 0)
        //		height = 1;

        //	try 
        //	{
        //		font = new Font(logFont.lfFaceName, height, logFont.style);
        //	}
        //	catch 
        //	{
        //		font = new Font("Arial", height, logFont.style);
        //	}

        //	return font;
        //}

        /// <summary>
        /// 일반 렌더링용 폰트 반환. 내부 캐시 반환
        /// ⚠ Control에 사용 시 IsHandleCreated 사용하여 타이밍 문제 해결.
        /// </summary>
        /// <returns></returns>
        public Font MakeFont()
        {
            if (!IsFontCacheValid())
            {
                UpdateFontCache();
            }
            return cachedFont;
        }

        /// <summary>
        /// 컨트롤 렌더링용 폰트 반환 (Clone).
        /// cachedFont와 독립적이므로 캐시 갱신/Dispose 시에도 컨트롤 Font 안전.
        /// 호출 측에서 이전 Font를 반드시 Dispose 해야 GDI 누수 방지.
        /// </summary>
        /// <returns></returns>
        public Font MakeControlFont()
        {
            if (!IsFontCacheValid())
            {
                UpdateFontCache();
            }
            // 컨트롤에 줄 때는 복사본을 준다 (원본 cachedFont는 내부에서만 관리) 260220 PSU
            return (Font)cachedFont.Clone();
        }

        /// <summary>
        /// 100%일때의 폰트를 만든다.
        /// </summary>
        /// <returns></returns>
        public Font MakeFont100()
		{
			Font font;
			float height = logFont.lfHeight;

			if(height == 0)
				height = 1;
			font = new Font(logFont.lfFaceName, height, logFont.style);

			return font;
		}


		public void ObjectSaveFont(CommaTextWriter writer)
		{
			writer.Write("\tLogFont,");
			writer.Write("{0},", logFont.lfFaceName);
			writer.Write("{0},", logFont.lfHeight);
			writer.Write("{0},", (logFont.style & FontStyle.Bold) > 0 ? 1 : 0);
			writer.Write("{0},", (logFont.style & FontStyle.Italic) > 0 ? 1 : 0);
			writer.Write("{0},", (logFont.style & FontStyle.Strikeout) > 0 ? 1 : 0);
			writer.Write("{0},", (logFont.style & FontStyle.Underline) > 0 ? 1 : 0);
			writer.WriteLine();
		}


        private float CalculateCurrentFontSize()
        {
            float height;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && cObjectShowMethod == 2)
            {
                double x = (double)nScreenSizeX / (double)nModuleSizeX;
                double y = (double)nScreenSizeY / (double)nModuleSizeY;
                if (x < y)
                    height = GetViewSizeX((int)logFont.lfHeight);
                else
                    height = GetViewSizeY((int)logFont.lfHeight);
            }
            else
            {
                height = GetViewSize((int)logFont.lfHeight);
            }

            // NaN, Infinity, 0 이하 방어
            if (float.IsNaN(height) || float.IsInfinity(height) || height <= 0)
                height = 1;

            return height;
        }

        private bool IsFontCacheValid()
        {
            if (cachedFont == null) 
                return false;

            float currentSize = CalculateCurrentFontSize();

            return lastCalculatedFontSize == currentSize &&
                   lastFontName == logFont.lfFaceName &&
                   lastFontStyle == logFont.style &&
                   lastScreenSizeX == nScreenSizeX &&
                   lastScreenSizeY == nScreenSizeY &&
                   lastModuleSizeX == nModuleSizeX &&
                   lastModuleSizeY == nModuleSizeY &&
                   lastObjectShowMethod == cObjectShowMethod &&
                   lastDefineMode == TotalConfig.defineMode;
        }

        private void UpdateFontCache()
        {
            float currentSize = CalculateCurrentFontSize();

            // 이전 cachedFont는 Dispose하지 않음.
            // Control.Font 등에서 참조 중일 수 있어 명시적 Dispose는 위험.
            // 참조가 끊기면 GC + Font Finalizer가 GDI 핸들을 해제함.

            // 새 폰트 생성
            try
            {
                cachedFont = new Font(logFont.lfFaceName, currentSize, logFont.style);
            }
            catch
            {
                cachedFont = new Font("Arial", currentSize, logFont.style);
            }

            // 캐시 상태 업데이트
            lastCalculatedFontSize = currentSize;
            lastFontName = logFont.lfFaceName;
            lastFontStyle = logFont.style;
            lastScreenSizeX = nScreenSizeX;
            lastScreenSizeY = nScreenSizeY;
            lastModuleSizeX = nModuleSizeX;
            lastModuleSizeY = nModuleSizeY;
            lastObjectShowMethod = cObjectShowMethod;
            lastDefineMode = TotalConfig.defineMode;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                cachedFont?.Dispose();
                cachedFont = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
        }
        ~ObjectFont()
        {
            Dispose(false);
        }

    }
}

