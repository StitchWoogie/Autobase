using System;
using Reporting.Engine.Model;

namespace Reporting.Engine.Formula
{
    /// <summary>
    /// 수식 엔진 (Phase 2 구현 예정)
    /// 셀 수식 파싱, 의존성 분석, 계산을 담당한다.
    /// 엑셀 호환 함수(SUM, AVERAGE 등) + 레거시 리포트 커맨드(AiAve, DiOnTime 등)를 처리.
    /// </summary>
    public class FormulaEngine
    {
        /// <summary>
        /// 시트의 모든 수식 셀을 계산
        /// </summary>
        public void Calculate(SheetModel sheet)
        {
            throw new NotImplementedException("수식 엔진은 Phase 2에서 구현 예정입니다.");
        }

        /// <summary>
        /// 특정 셀의 수식을 계산하여 값 반환
        /// </summary>
        public object Evaluate(SheetModel sheet, int row, int col)
        {
            throw new NotImplementedException("수식 엔진은 Phase 2에서 구현 예정입니다.");
        }
    }
}
