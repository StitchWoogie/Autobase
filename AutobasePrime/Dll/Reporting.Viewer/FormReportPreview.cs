using System;
using System.Windows.Forms;
using Reporting.Engine.Model;

namespace Reporting.Viewer
{
    /// <summary>
    /// 인쇄 미리보기 폼 (Phase 2 구현 예정)
    /// SheetRenderer를 사용하여 페이지별 렌더링 결과를 미리보기로 표시한다.
    /// </summary>
    public partial class FormReportPreview : Form
    {
        private WorkbookModel _workbook;

        public FormReportPreview()
        {
            InitializeComponent();
        }

        public FormReportPreview(WorkbookModel workbook) : this()
        {
            _workbook = workbook;
            // Phase 2에서 SheetRenderer를 이용한 미리보기 구현
        }
    }
}
