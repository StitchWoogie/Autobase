using System;
using Reporting.Engine.Model;

namespace Reporting.Engine.IO
{
    /// <summary>
    /// WorkbookModel → PNG 이미지 출력 (Phase 2 구현 예정)
    /// SheetRenderer를 이용하여 비트맵으로 렌더링 후 PNG 저장
    /// </summary>
    public static class PngWriter
    {
        /// <summary>
        /// 워크북의 활성 시트를 PNG로 저장
        /// </summary>
        public static void Write(WorkbookModel workbook, string filePath)
        {
            throw new NotImplementedException("PNG 출력은 Phase 2에서 구현 예정입니다.");
        }

        /// <summary>
        /// 워크북의 활성 시트를 PNG로 저장 (크기 지정)
        /// </summary>
        public static void Write(WorkbookModel workbook, string filePath, int width, int height)
        {
            throw new NotImplementedException("PNG 출력은 Phase 2에서 구현 예정입니다.");
        }
    }
}
