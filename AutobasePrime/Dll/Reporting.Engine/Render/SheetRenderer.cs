using System;
using System.Drawing;
using Reporting.Engine.Model;

namespace Reporting.Engine.Render
{
    /// <summary>
    /// 시트 렌더러 (Phase 2 구현 예정)
    /// SheetModel을 GDI+ Graphics 위에 렌더링한다.
    /// 인쇄 미리보기, PNG 출력, 뷰어 화면에 사용된다.
    /// </summary>
    public class SheetRenderer
    {
        /// <summary>
        /// 시트를 지정된 Graphics 컨텍스트에 렌더링
        /// </summary>
        /// <param name="g">GDI+ Graphics</param>
        /// <param name="sheet">렌더링할 시트</param>
        /// <param name="bounds">렌더링 영역</param>
        public void Render(Graphics g, SheetModel sheet, Rectangle bounds)
        {
            throw new NotImplementedException("시트 렌더러는 Phase 2에서 구현 예정입니다.");
        }

        /// <summary>
        /// 시트를 비트맵으로 렌더링
        /// </summary>
        /// <param name="sheet">렌더링할 시트</param>
        /// <param name="width">비트맵 너비</param>
        /// <param name="height">비트맵 높이</param>
        /// <returns>렌더링된 Bitmap</returns>
        public Bitmap RenderToBitmap(SheetModel sheet, int width, int height)
        {
            throw new NotImplementedException("시트 렌더러는 Phase 2에서 구현 예정입니다.");
        }
    }
}
