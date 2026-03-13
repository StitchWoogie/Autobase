using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using AutoLibLocal;
using NetTools;

namespace GraphicModule
{
    /// <summary>
    /// Chart 오브젝트 로더 (LoadObjectFromMultiTrend 패턴 기반).
    /// ChartOption, ChartColors, ChartTitle, ChartMember, CustomPoint 파싱 지원.
    /// </summary>
    class LoadObjectFromChart : LoadObjectFromModX
    {
        public ScriptClass scriptEventAfterSettings;
        public LogarithmicScale logarithmicScale = new LogarithmicScale();

        // Chart 공통 설정
        public ObjectArgsChartCommon chartCommon = new ObjectArgsChartCommon();

        // Chart 전용 멤버 (CHART_TAG_MEMBER)
        public ArrayList chartMember = new ArrayList();

        // CustomTrend용 커스텀 포인트
        public ArrayList customPoints = new ArrayList();

        // CustomTrend 데이터 수집 간격 (ms)
        public int nDataTimeCustom = 1000;

        public LoadObjectFromChart(ObjectCommonProperty ocp)
            : base(ocp)
        {
        }

        protected override bool ChildCheck(TextReader reader, string command, CommaTextReader comma)
        {
            if (String.Compare(command, "ScriptEventAfterSettings") == 0)
            {
                scriptEventAfterSettings = LoadOneScript(reader, command);
            }
            else if (String.Compare(command, "LogarithmicScale") == 0)
            {
                logarithmicScale = new LogarithmicScale();
                logarithmicScale.bUse = comma.GetBool();
                logarithmicScale.fBase = comma.GetDouble();
            }
            else if (String.Compare(command, "ChartOption") == 0)
            {
                chartCommon.nDefaultSeriesType = comma.GetInt();
                chartCommon.bShowLegend = comma.GetBool();
                chartCommon.bShowGrid = comma.GetBool();
                chartCommon.bAntiAlias = comma.GetBool();
                chartCommon.b3DStyle = comma.GetBool();
                chartCommon.nTitleSize = comma.GetInt();
                chartCommon.nLegendPosition = comma.GetInt();
                chartCommon.nAxisXLabelAngle = comma.GetInt();
                chartCommon.bUseToolBar = comma.GetBool();
                chartCommon.nToolBarPos = comma.GetInt();
                chartCommon.nToolBarBtnColor = comma.GetInt();
                chartCommon.nToolBarButtonSize = comma.GetInt();
                chartCommon.nToolBarTextSize = comma.GetInt();
                chartCommon.bHideLabelDataRange = comma.GetBool();
                chartCommon.bDontUseConfigDialog = comma.GetBool();
                chartCommon.bUseMouseButtonAsZoom = comma.GetBool();
                chartCommon.bUseCrosshair = comma.GetBool();
                chartCommon.bUseTagDescription = comma.GetBool();
            }
            else if (String.Compare(command, "ChartColors") == 0)
            {
                chartCommon.colorChartBack = Color.FromArgb(comma.GetInt());
                chartCommon.colorPlotBack = Color.FromArgb(comma.GetInt());
                chartCommon.colorGrid = Color.FromArgb(comma.GetInt());
                chartCommon.colorTitle = Color.FromArgb(comma.GetInt());
                chartCommon.colorLegendText = Color.FromArgb(comma.GetInt());
                chartCommon.colorLegendBack = Color.FromArgb(comma.GetInt());
            }
            else if (String.Compare(command, "ChartTitle") == 0)
            {
                string title = "";
                comma.GetString(ref title);
                chartCommon.sTitle = title;
            }
            else if (String.Compare(command, "ChartMember") == 0)
            {
                CHART_TAG_MEMBER member = new CHART_TAG_MEMBER();

                string tag = "";
                comma.GetString(ref tag);
                member.tag = tag.Trim();
                member.nType = (EnumTagType)comma.GetInt();
                member.color = Color.FromArgb(comma.GetInt());
                member.nSeriesType = comma.GetInt();
                member.nLineThick = comma.GetInt();
                if (member.nLineThick < 1) member.nLineThick = 1;
                member.nPointType = comma.GetInt();
                member.nAxisPosition = comma.GetInt();
                member.nLevelFrom = comma.GetInt();
                member.nLevelTo = comma.GetInt();
                comma.GetChar(ref member.visible);
                comma.GetChar(ref member.bReverseY);
                member.nValueType = comma.GetInt();
                member.nTagDisplaySize = comma.GetInt();
                comma.GetHexWORD(ref member.wFlags);
                member.nTimeShift = comma.GetInt();

                string col = "";
                comma.GetString(ref col);
                member.column = col;

                chartMember.Add(member);
            }
            else if (String.Compare(command, "nDataTime") == 0)
            {
                // CustomChart 데이터 수집 간격 (ms)
                // 부모 LoadObjectFromModX 필드에 임시 저장하지 않고 chartCommon에 직접 저장 불가
                // → nShowUnit 패턴처럼 public 필드 사용
                nDataTimeCustom = comma.GetInt();
            }
            else if (String.Compare(command, "CustomPoint") == 0)
            {
                CHART_CUSTOM_POINT cp = new CHART_CUSTOM_POINT();

                string label = "";
                comma.GetString(ref label);
                cp.label = label;
                cp.value = comma.GetDouble();
                cp.color = Color.FromArgb(comma.GetInt());
                cp.visible = comma.GetBool();

                customPoints.Add(cp);
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
