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
using System.Collections.Generic;
using NetTools;

namespace SilverlightGraphicModule
{
    class LoadObjectFromTagAnimation : LoadObjectFromModX
    {
        public List<object> blockTagAnimation = new List<object>();

        protected override bool ChildCheck(string command, CommaTextReader comma)
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
