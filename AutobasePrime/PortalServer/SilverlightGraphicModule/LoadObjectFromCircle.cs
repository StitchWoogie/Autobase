using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NetTools;

namespace SilverlightGraphicModule
{
    /// <summary>
    /// 각 오브젝트마다 고유한 멤버가 있으므로 클래스를 만들어서 사용하면 속도가 더 빨라지고 정리가 잘 될 듯
    /// </summary>
    class LoadObjectFromCircle : LoadObjectFromModX
    {
        public ObjectArgsCircle objArgs = new ObjectArgsCircle();

        /*
        public LoadObjectFromCircle(ObjectCommonProperty ocp)
            : base(ocp)
        {

        }*/

        protected override bool ChildCheck(string command, CommaTextReader comma)
        {
            if (String.Compare(command, "CircleType") == 0)
            {
                comma.GetInt(ref objArgs.type);
                comma.GetFloat(ref objArgs.fStartAngle);
                comma.GetFloat(ref objArgs.fSweepAngle);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
