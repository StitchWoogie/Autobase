using AutoLibLocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace GraphicModule
{
    /// <summary>
    /// Chart 오브젝트 스크립트 함수 지원.
    /// ScriptFunctionMultiTrend / ScriptFunctionMultiGraph 패턴 기반.
    /// Chart 타입 (Custom) 의 스크립트 API 제공.
    /// </summary>
    public class ScriptFunctionChart
    {
        //===================================================================
        // 공통 비동기 델리게이트 (ObjectMultiTrend 최신 패턴)
        //===================================================================


        /// <summary>
        /// CustomChart 스크립트 실행
        /// </summary>
        static async Task<(int, object)> Run_CustomChart(
            ScriptClass scriptClass, string method_name, object[] args)
        {
            if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                return (1, null);

            object ret = await scriptClass.ExecuteClassName(
                CustomChart.arrayClassList, (string)args[0], method_name, args);
            return (1, ret);
        }

        //===================================================================
        // PrepareMethod: 모든 스크립트 메서드 등록
        //===================================================================

        public static void PrepareMethod(ScriptExternalRun prepare)
        {

            //---------------------------------------------------------------
            // CustomChart 스크립트 메서드
            //---------------------------------------------------------------
            string preCu = "CustomChart";

            prepare.AddMethod(preCu, "CustomChartClear", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name");

            // 태그 기반 멤버 추가/삭제
            prepare.AddMethod(preCu, "CustomChartAddTag", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:string:tag_name", "in:int:color");

            prepare.AddMethod(preCu, "CustomChartDeleteTag", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:string:tag_name");

            // 커스텀 포인트 추가
            prepare.AddMethod(preCu, "CustomChartAddPoint", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:string:label",
                "in:double:value", "in:int:color");

            // 커스텀 포인트 삭제
            prepare.AddMethod(preCu, "CustomChartRemovePoint", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:string:label");

            // 커스텀 포인트 값 설정
            prepare.AddMethod(preCu, "CustomChartSetPointValue", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:string:label", "in:double:value");

            // 커스텀 포인트 값 조회
            prepare.AddMethod(preCu, "CustomChartGetPointValue", "double",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:string:label");

            // 커스텀 포인트 색상 설정
            prepare.AddMethod(preCu, "CustomChartSetPointColor", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:string:label", "in:int:color");

            // 커스텀 포인트 개수 조회
            prepare.AddMethod(preCu, "CustomChartGetPointCount", "int",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name");

            // 모든 커스텀 포인트 삭제
            prepare.AddMethod(preCu, "CustomChartClearPoints", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name");

            // 차트 타입 변경 (Pie=6, Doughnut=7, Radar=8 포함)
            prepare.AddMethod(preCu, "CustomChartSetSeriesType", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:int:series_type");

            // 가시성 설정
            prepare.AddMethod(preCu, "CustomChartSetVisible", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:string:tag_name", "in:int:visible");

            // 배경색 변경
            prepare.AddMethod(preCu, "CustomChartSetBackColor", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:int:color");

            // Y축 범위 설정
            prepare.AddMethod(preCu, "CustomChartSetBasicLevel", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:string:tag_name",
                "in:double:min_value", "in:double:max_value");

            // 로그 스케일 설정
            prepare.AddMethod(preCu, "CustomChartSetLogarithmicScale", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:int:use", "in:double:log_base");

            // 그리드라인 설정
            prepare.AddMethod(preCu, "CustomChartSetGridLine", "void",
                (ScriptExternalRun.AsyncDeleMethod)Run_CustomChart,
                "in:string:class_name", "in:int:show");
        }
    }
}
