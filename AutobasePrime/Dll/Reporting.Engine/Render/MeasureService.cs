using System;
using System.Drawing;
using Reporting.Engine.Model;

namespace Reporting.Engine.Render
{
    /// <summary>
    /// 셀/텍스트 측정 서비스 (Phase 2 구현 예정)
    /// 폰트 기반 텍스트 크기 측정, 열 너비 자동 맞춤, 행 높이 자동 맞춤 등에 사용.
    /// GDI+ MeasureString을 내부적으로 활용.
    /// </summary>
    public class MeasureService
    {
        /// <summary>
        /// 지정 폰트로 텍스트 크기를 측정
        /// </summary>
        /// <param name="text">텍스트</param>
        /// <param name="font">폰트 설정</param>
        /// <returns>텍스트 크기 (픽셀)</returns>
        public SizeF MeasureText(string text, FontStyleModel font)
        {
            throw new NotImplementedException("측정 서비스는 Phase 2에서 구현 예정입니다.");
        }

        /// <summary>
        /// 열 너비 자동 맞춤 계산
        /// </summary>
        /// <param name="sheet">시트</param>
        /// <param name="col">열 번호 (1-based)</param>
        /// <returns>최적 열 너비 (문자폭 기준)</returns>
        public double AutoFitColumnWidth(SheetModel sheet, int col)
        {
            throw new NotImplementedException("측정 서비스는 Phase 2에서 구현 예정입니다.");
        }
    }
}
