using System;

namespace Reporting.Engine.Formula
{
    /// <summary>
    /// 태그 리졸버 (Phase 2 구현 예정)
    /// SCADA 태그명을 실제 데이터 값으로 해석하는 역할.
    /// 수식 엔진에서 태그 참조가 필요할 때 호출된다.
    /// </summary>
    public class TagResolver
    {
        /// <summary>
        /// 태그명으로 현재 값 조회
        /// </summary>
        /// <param name="tagName">태그명</param>
        /// <returns>태그의 현재 값</returns>
        public object ResolveTag(string tagName)
        {
            throw new NotImplementedException("태그 리졸버는 Phase 2에서 구현 예정입니다.");
        }

        /// <summary>
        /// 태그명으로 이력 데이터 조회
        /// </summary>
        /// <param name="tagName">태그명</param>
        /// <param name="from">시작 시간</param>
        /// <param name="to">종료 시간</param>
        /// <returns>이력 데이터 배열</returns>
        public double[] ResolveHistory(string tagName, DateTime from, DateTime to)
        {
            throw new NotImplementedException("태그 리졸버는 Phase 2에서 구현 예정입니다.");
        }
    }
}
