using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NetTools;
using System.IO;
using AutoLibLocal;

namespace GraphicModule
{
    /// <summary>
    /// 각 오브젝트마다 고유한 멤버가 있으므로 클래스를 만들어서 사용하면 속도가 더 빨라지고 정리가 잘 될 듯
    /// </summary>
    class LoadObjectFromButtonDigitalOut : LoadObjectFromModX
    {
        public ArrayList blockButtonDoutMember = new ArrayList();

        public LoadObjectFromButtonDigitalOut(ObjectCommonProperty ocp)
            : base(ocp)
        {

        }

        protected override bool ChildCheck(TextReader reader, string command, CommaTextReader comma)
        {
            if (base.ChildCheck(reader, command, comma)) return true;

            if (String.Compare(command, "ButtonDoutMember") == 0)
            {
                BUTTON_DOUT_STRUCT member = new BUTTON_DOUT_STRUCT();

                comma.GetString(ref member.tag);
                member.tag = member.tag.Trim();
                blockButtonDoutMember.Add(member);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
