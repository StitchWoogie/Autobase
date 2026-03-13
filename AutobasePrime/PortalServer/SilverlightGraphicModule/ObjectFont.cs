using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NetTools.OldDefine;

namespace SilverlightGraphicModule
{
    public class ObjectFont : ObjectPublic
    {
        protected LOGFONT logFont;

        public ObjectFont(ObjectCommonProperty ocp, LOGFONT font)
            : base(ocp)
        {
            //
            // TODO: Add constructor logic here
            //
            
            if (font == null)
            {
                logFont = new LOGFONT();

                if (NetTools.Tools.IsLangKorean())
                {
                    logFont.lfFaceName = "굴림";
                    logFont.lfHeight = 9;
                }
                else if (NetTools.Tools.IsLangJapanese())
                {
                    logFont.lfFaceName = "MS UI Gothic";
                    logFont.lfHeight = 9;
                }
                else if (NetTools.Tools.IsLangChinese())
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

                if (font.lfHeight <= 0) font.lfHeight = 9;
                if (font.lfFaceName == null)
                {
                    if (NetTools.Tools.IsLangKorean())
                        font.lfFaceName = "굴림";
                    else if (NetTools.Tools.IsLangJapanese())
                        font.lfFaceName = "MS UI Gothic";
                    else if (NetTools.Tools.IsLangChinese())
                        font.lfFaceName = "SimSun";
                    else
                        font.lfFaceName = "Tahoma";
                }
            }
        }

        public void SetLogFont(LOGFONT lf)
        {
            if (lf == null) return;

            logFont = lf;
        }

        public LOGFONT GetLogFont()
        {
            LOGFONT lf = new LOGFONT();
            lf.lfFaceName = logFont.lfFaceName;
            //lf.style = logFont.style;
            
            lf.Bold = logFont.Bold;
            lf.Italic = logFont.Italic;
            lf.Underline = logFont.Underline;
            lf.Strikeout = logFont.Strikeout;

            lf.lfHeight = logFont.lfHeight;

            return lf;
        }

        FontFamily NewFontFamily(string source)
        {
            string target;

            if (source == "바탕")   target = "Batang";
            else if (source == "바탕체") target = "BatangChe";
            else if (source == "궁서") target = "Gungsuh";
            else if (source == "궁서체") target = "GungsuhChe";
            else if (source == "돋움") target = "Dotum";
            else if (source == "돋움체") target = "DotumChe";
            else if (source == "굴림") target = "Gulim";
            else if (source == "굴림체") target = "GulimChe";

            else if (source == "HY견명조") target = "HYMyeongJo-Extra";
            else if (source == "휴먼엑스포") target = "Expo M";
            else if (source == "휴먼모음T") target = "MoeumT R";
            else if (source == "휴먼옛체") target = "Yet R";
            else if (source == "휴먼편지체") target = "Pyunji R";
            else if (source == "휴먼아미체") target = "Ami R";
            else if (source == "휴먼매직체") target = "Magic R";
            else if (source == "휴먼둥근헤드라인") target = "Headline R";

            else if (source == "맑은 고딕") target = "Malgun Gothic";
            else if (source == "새굴림") target = "New Gulim";
            else target = source;

            return new FontFamily(target);
        }

        protected void MakeFont(TextBlock obj)
        {
            obj.FontFamily = NewFontFamily(logFont.lfFaceName);
            obj.FontSize = GetViewSize((int)logFont.lfHeight) *96 / 72;

            obj.FontStyle = logFont.Italic ? FontStyles.Italic : FontStyles.Normal;
            obj.FontWeight = logFont.Bold ? FontWeights.Bold : FontWeights.Normal;
        }

        protected void MakeFont(Button obj)
        {
            obj.FontFamily = NewFontFamily(logFont.lfFaceName);
            obj.FontSize = GetViewSize((int)logFont.lfHeight) *96 / 72;

            obj.FontStyle = logFont.Italic ? FontStyles.Italic : FontStyles.Normal;
            obj.FontWeight = logFont.Bold ? FontWeights.Bold : FontWeights.Normal;
        }

        protected void MakeFont(TextBox obj)
        {
            obj.FontFamily = NewFontFamily(logFont.lfFaceName);
            obj.FontSize = GetViewSize((int)logFont.lfHeight) *96 / 72;

            obj.FontStyle = logFont.Italic ? FontStyles.Italic : FontStyles.Normal;
            obj.FontWeight = logFont.Bold ? FontWeights.Bold : FontWeights.Normal;
        }

        protected void MakeFont(ListBox obj)
        {
            obj.FontFamily = NewFontFamily(logFont.lfFaceName);
            obj.FontSize = GetViewSize((int)logFont.lfHeight) *96 / 72;

            obj.FontStyle = logFont.Italic ? FontStyles.Italic : FontStyles.Normal;
            obj.FontWeight = logFont.Bold ? FontWeights.Bold : FontWeights.Normal;
        }

        protected void MakeFont(ComboBox obj)
        {
            obj.FontFamily = NewFontFamily(logFont.lfFaceName);
            obj.FontSize = GetViewSize((int)logFont.lfHeight) *96 / 72;

            obj.FontStyle = logFont.Italic ? FontStyles.Italic : FontStyles.Normal;
            obj.FontWeight = logFont.Bold ? FontWeights.Bold : FontWeights.Normal;
        }

    }
}
