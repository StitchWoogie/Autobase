namespace Reporting.Engine.Model
{
    /// <summary>
    /// 용지 방향
    /// </summary>
    public enum PageOrientation
    {
        Portrait = 0,
        Landscape
    }

    /// <summary>
    /// 용지 크기
    /// </summary>
    public enum PaperSize
    {
        A4 = 0,
        A3,
        Letter,
        Legal,
        B4,
        B5
    }

    /// <summary>
    /// 페이지 설정 모델
    /// </summary>
    public sealed class PageSetupModel
    {
        public PageOrientation Orientation { get; set; } = PageOrientation.Portrait;
        public PaperSize PaperSize { get; set; } = PaperSize.A4;

        /// <summary>여백 (단위: mm)</summary>
        public double MarginLeft { get; set; } = 20;
        public double MarginRight { get; set; } = 20;
        public double MarginTop { get; set; } = 25;
        public double MarginBottom { get; set; } = 25;

        /// <summary>머리글/바닥글</summary>
        public string HeaderLeft { get; set; }
        public string HeaderCenter { get; set; }
        public string HeaderRight { get; set; }
        public string FooterLeft { get; set; }
        public string FooterCenter { get; set; }
        public string FooterRight { get; set; }

        /// <summary>인쇄 배율 (퍼센트, 기본 100)</summary>
        public int ScalePercent { get; set; } = 100;

        /// <summary>격자선 인쇄 여부</summary>
        public bool PrintGridLines { get; set; }

        public PageSetupModel Clone()
        {
            return new PageSetupModel
            {
                Orientation = Orientation,
                PaperSize = PaperSize,
                MarginLeft = MarginLeft,
                MarginRight = MarginRight,
                MarginTop = MarginTop,
                MarginBottom = MarginBottom,
                HeaderLeft = HeaderLeft,
                HeaderCenter = HeaderCenter,
                HeaderRight = HeaderRight,
                FooterLeft = FooterLeft,
                FooterCenter = FooterCenter,
                FooterRight = FooterRight,
                ScalePercent = ScalePercent,
                PrintGridLines = PrintGridLines
            };
        }
    }
}
