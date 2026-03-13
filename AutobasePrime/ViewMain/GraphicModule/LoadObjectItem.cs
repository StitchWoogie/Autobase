using System;
using System.Collections.Generic;
using System.Text;
using NetTools;

namespace GraphicModule
{
    /// <summary>
    /// SaveObjectItem처럼 여러곳에서 Item을 불러야 할 필요가 있을 때
    /// </summary>
    class LoadObjectItem
    {
        // 그룹과 일반오브젝트에서 동시에 사용한다.
        public static void LoadClassName(CommaTextReader comma, ObjectGeneral oj, ObjectCommonProperty ocp)
        {
            string buf = "";
            
            comma.GetString(ref oj.sClassName);
            comma.GetChar(ref oj.bUseToolTip);
            comma.GetString(ref oj.sObjectDescription);
            sbyte flag = 0;
            comma.GetChar(ref flag);
            oj.bResponseOnVisible = (flag == 1);
            comma.GetString(ref oj.sOnStudioTitle);

            comma.GetChar(ref flag);
            if (AutoLib.ConfigStudio.bSaveLayerLockStatus)  // 
            {
                oj.bOnStudioLocked = (flag == 1);
            }

            comma.GetString(ref buf);
            if (AutoLib.ConfigStudio.bSaveLayerShowStatus && !ocp.bLoadOnLibraryView)
            {
                if (buf.Length != 0)    // 0 인 경우는 이전 버전의 파일이므로 true를 유지하도록 한다.
                {
                    oj.bOnStudioVisible = (buf == "1");
                }
            }
        }

        public static void LoadRotation(CommaTextReader comma, ObjectGeneral oj)
        {
            comma.GetFloat(ref oj.fRotateAngle);
        }

    }
}
