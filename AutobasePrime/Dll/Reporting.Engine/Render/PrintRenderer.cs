using System;
using System.Drawing.Printing;
using Reporting.Engine.Model;

namespace Reporting.Engine.Render
{
    /// <summary>
    /// 인쇄 렌더러 (Phase 2 구현 예정)
    /// PrintDocument 이벤트에서 SheetRenderer를 활용하여 페이지를 그린다.
    /// 페이지 분할, 머리글/바닥글 처리 포함.
    /// </summary>
    public class PrintRenderer
    {
        /// <summary>
        /// 인쇄 시작
        /// </summary>
        /// <param name="workbook">인쇄할 워크북</param>
        public void Print(WorkbookModel workbook)
        {
            throw new NotImplementedException("인쇄 렌더러는 Phase 2에서 구현 예정입니다.");
        }

        /// <summary>
        /// 인쇄 미리보기용 PrintDocument 생성
        /// </summary>
        /// <param name="workbook">워크북</param>
        /// <returns>PrintDocument</returns>
        public PrintDocument CreatePrintDocument(WorkbookModel workbook)
        {
            throw new NotImplementedException("인쇄 렌더러는 Phase 2에서 구현 예정입니다.");
        }
    }
}
