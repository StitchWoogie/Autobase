using System;
using Reporting.Engine.Model;

namespace Reporting.Engine.IO
{
    /// <summary>
    /// WorkbookModel → PDF 출력 (Phase 2 구현 예정)
    /// PdfSharp 사용 예정
    /// </summary>
    public static class PdfWriter
    {
        /// <summary>
        /// 워크북을 PDF로 저장
        /// </summary>
        public static void Write(WorkbookModel workbook, string filePath)
        {
            throw new NotImplementedException("PDF 출력은 Phase 2에서 구현 예정입니다.");
        }
    }
}
