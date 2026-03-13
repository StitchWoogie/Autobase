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
    class LoadObjectFromTagAnimation : LoadObjectFromModX
    {
        public ArrayList blockTagAnimation = new ArrayList();

        public LoadObjectFromTagAnimation(ObjectCommonProperty ocp) : base(ocp)
        {

        }

        protected override bool ChildCheck(TextReader reader, string command, CommaTextReader comma)
        {
            if (String.Compare(command, "TagAnimationMember") == 0)
            {
                TagAnimationMember member = new TagAnimationMember();
                byte flag = 0;

                comma.GetBYTE(ref flag);
                member.active = (flag == 1);
                comma.GetBYTE(ref flag);
                member.bDefault = (flag == 1);
                comma.GetString(ref member.tag);
                comma.GetInt(ref member.condition);
                comma.GetString(ref member.value);
                comma.GetString(ref member.filename);

                blockTagAnimation.Add(member);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
