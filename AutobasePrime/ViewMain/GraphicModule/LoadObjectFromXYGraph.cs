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
    class LoadObjectFromXYGraph : LoadObjectFromModX
    {
        public ArrayList graphMemberXY = new ArrayList();

        public LoadObjectFromXYGraph(ObjectCommonProperty ocp)
            : base(ocp)
        {

        }

        int r = 0, g = 0, b = 0;

        protected override bool ChildCheck(TextReader reader, string command, CommaTextReader comma)
        {
            if (String.Compare(command, "GraphMemberXY") == 0)
            {
                XY_GRAPH_MEMBER member = new XY_GRAPH_MEMBER();
                comma.GetString(ref member.tagX);
                comma.GetString(ref member.tagY);
                comma.GetInt(ref r);
                comma.GetInt(ref g);
                comma.GetInt(ref b);
                member.color = Color.FromArgb(r, g, b);
                comma.Skip();//comma.GetChar(ref member.n.cValueType);
                comma.GetChar(ref member.cPointType);
                comma.GetChar(ref member.cLineThick);
                if (member.cLineThick < 1) member.cLineThick = 1;

                comma.GetChar(ref member.cAxisPositionX);
                comma.GetInt(ref member.nLevelFromX);
                comma.GetInt(ref member.nLevelToX);
                comma.GetChar(ref member.cAxisPositionY);
                comma.GetInt(ref member.nLevelFromY);
                comma.GetInt(ref member.nLevelToY);
                comma.GetInt(ref member.nTagDisplaySize);
                comma.GetChar(ref member.bReverseX);
                comma.GetChar(ref member.bReverseY);
                comma.GetInt(ref member.nGraphType);

                graphMemberXY.Add(member);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
