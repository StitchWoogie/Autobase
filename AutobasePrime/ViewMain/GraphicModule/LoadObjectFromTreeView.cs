using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NetTools;
using System.IO;

namespace GraphicModule
{
    /// <summary>
    /// 각 오브젝트마다 고유한 멤버가 있으므로 클래스를 만들어서 사용하면 속도가 더 빨라지고 정리가 잘 될 듯
    /// </summary>
    class LoadObjectFromTreeView : LoadObjectFromModX
    {
        public ScriptClass scriptEventDoubleClick;

        public LoadObjectFromTreeView(ObjectCommonProperty ocp)
            : base(ocp)
        {

        }

        protected override bool ChildCheck(TextReader reader, string command, CommaTextReader comma)
        {
            if (String.Compare(command, "ScriptEventDoubleClick") == 0)
            {
                scriptEventDoubleClick = LoadOneScript(reader, command);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
