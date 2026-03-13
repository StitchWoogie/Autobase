using System;

namespace Reporting.Engine.Formula
{
    /// <summary>
    /// 레거시 리포트 커맨드 어댑터 (Phase 2 구현 예정)
    /// 자체리포트의 39종 커맨드(AI_CURR, AI_AVE, DI_ONTIME 등)를
    /// 새 수식 엔진에서 실행할 수 있도록 브릿지 역할을 한다.
    ///
    /// 수식 형식: =AiAve(tagName, params...)
    /// 내부적으로 ReportBasicLib의 MakeRunReport 로직을 호출하거나 직접 구현.
    /// </summary>
    public class LegacyCommandAdapter
    {
        /// <summary>
        /// 레거시 커맨드 문자열을 파싱하여 실행
        /// </summary>
        /// <param name="command">커맨드 문자열 (예: "AiAve,tagName,param1,param2")</param>
        /// <returns>계산 결과</returns>
        public object Execute(string command)
        {
            throw new NotImplementedException("레거시 커맨드 어댑터는 Phase 2에서 구현 예정입니다.");
        }
    }
}
