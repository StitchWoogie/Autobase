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
    class LoadObjectFromDataGrid : LoadObjectFromModX
    {
        public ScriptClass scriptEventCellClick;
        public ScriptClass scriptEventCellPainting;
        public ScriptClass scriptEventCellValueChanged;

        public LoadObjectFromDataGrid(ObjectCommonProperty ocp)
            : base(ocp)
        {

        }

        protected override bool ChildCheck(TextReader reader, string command, CommaTextReader comma)
        {
            if (String.Compare(command, "ScriptEventCellClick") == 0)
            {
                scriptEventCellClick = LoadOneScript(reader, command);
            }
            else if (String.Compare(command, "ScriptEventCellPainting") == 0)
            {
                scriptEventCellPainting = LoadOneScript(reader, command);
            }
            else if (String.Compare(command, "ScriptEventCellValueChanged") == 0)
            {
                scriptEventCellValueChanged = LoadOneScript(reader, command);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
