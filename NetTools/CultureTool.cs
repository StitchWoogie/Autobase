using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

namespace NetTools
{
    public class CultureTool
    {
        // 미국 = -123,456,789,00
        // 러시아 = -123 456 789.00

        //public static char cNumberDecimalSeparator = '.';   // 소수점 구분
        //public static char cNumberGroupSeparator = ',';     // 천단위 구분

        public static CultureInfo ciKR;    // Culture를 구분하기전에 대부분 ko-KR 형식으로 저장되었으므로 그 형식으로 읽고 쓰고 해야 된다.
                                    // 특히 , 같은 경우 CSV 파일 같은 것은 저장할 수 없으므로 국제적으로 사용해야 되는 경우 ciKR 형식으로 저장해야 호환성이 좋다. 
                                    // 레지스트리에 저장할 때도 ciKR 형식으로 저장하는 것이 좋다. 중간에 형식을 바꾸면 전체가 틀어지게 된다.

        public static CultureInfo ciCurrent;    // 현재의 CultureInfo;
        
        static CultureTool()
        {
            CultureInfo info = Thread.CurrentThread.CurrentCulture;

            ciCurrent = info;
            /*
            string s;
            
            s = info.NumberFormat.NumberDecimalSeparator;

            if (s.Length > 0)
                cNumberDecimalSeparator = s[0];

            s = info.NumberFormat.NumberGroupSeparator;
            if (s.Length > 0)
                cNumberGroupSeparator = s[0];*/

            ciKR = new CultureInfo("ko-KR");
            ciKR.NumberFormat.NumberDecimalSeparator = "."; // 사용자가 제어판에서 설정을 바꾸면 그쪽으로 따라가게 된다. 그래서 Default로 변경한다.
            ciKR.NumberFormat.NumberGroupSeparator = ",";   // 사용자가 제어판에서 설정을 바꾸면 그쪽으로 따라가게 된다. 그래서 Default로 변경한다.
        }

    }
}
