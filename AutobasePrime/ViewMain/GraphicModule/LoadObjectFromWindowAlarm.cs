using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NetTools;
using System.Drawing;
using System.IO;

namespace GraphicModule
{
    /// <summary>
    /// 각 오브젝트마다 고유한 멤버가 있으므로 클래스를 만들어서 사용하면 속도가 더 빨라지고 정리가 잘 될 듯
    /// </summary>
    class LoadObjectFromWindowAlarm : LoadObjectFromModX
    {
        public List<AlarmEventColumn> arrayColumns;

        public LoadObjectFromWindowAlarm(ObjectCommonProperty ocp)
            : base(ocp)
        {

        }

        protected override bool ChildCheck(TextReader reader, string command, CommaTextReader comma)
        {
            if (String.Compare(command, "AlarmEventColumn") == 0)
            {
                if (arrayColumns == null)
                    arrayColumns = new List<AlarmEventColumn>();

                AlarmEventColumn aec = new AlarmEventColumn();

                aec.name = comma.GetString();
                comma.GetBool(ref aec.visible);
                aec.title = comma.GetString();

                arrayColumns.Add(aec);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
