using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace WebTools
{
    public class Language
    {
        public static bool IsKorean(string ui_culture)
        {
            if (String.Compare(ui_culture, 0, "ko", 0, 2, true) == 0)
                return true;

            else if (String.Compare(ui_culture, 0, "한국어", 0, 3, true) == 0)  // 실제 페이지에서는 한국어 (대한민국) 이라고 나온다.
                return true;

            return false;
        }

        public static bool IsKorean(Page page)
        {
            return IsKorean(page.UICulture);
        }

        public static bool ChangeSelectedLanguage(Page page)
        {
            string lan;
            if (page.Session["LanguageUserSelected"] != null)
            {
                lan = page.Session["LanguageUserSelected"].ToString();

                if (lan == "Korean")
                {
                    page.UICulture = "ko-KR";
                    //page.Culture = "ko-KR";
                }
                else
                {
                    page.UICulture = "en-US";
                    //page.Culture = "en-US";
                }

                return true;
            }

            return false;
        }
    }
}
