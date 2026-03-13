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
using NetTools;

namespace SilverlightGraphicModule
{
    /// <summary>
    /// SaveObjectItem처럼 여러곳에서 Item을 불러야 할 필요가 있을 때
    /// </summary>
    class LoadObjectItem
    {
        // 그룹과 일반오브젝트에서 동시에 사용한다.
        public static void LoadClassName(CommaTextReader comma, ObjectGeneral oj)
        {
            comma.GetString(ref oj.sClassName);
            comma.GetChar(ref oj.bUseToolTip);
            comma.GetString(ref oj.sObjectDescription);
            sbyte flag = 0;
            comma.GetChar(ref flag);
            oj.bResponseOnVisible = (flag == 1);
        }

        public static void LoadRotation(CommaTextReader comma, ObjectGeneral oj)
        {
            comma.GetFloat(ref oj.fRotateAngle);
        }

    }
}
