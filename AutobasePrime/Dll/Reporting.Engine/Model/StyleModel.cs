using System.Drawing;

namespace Reporting.Engine.Model
{
    /// <summary>
    /// 셀 서식 모델
    /// </summary>
    public sealed class StyleModel
    {
        public FontStyleModel Font { get; set; }
        public BorderStyleModel Border { get; set; }
        public FillStyleModel Fill { get; set; }
        public AlignmentStyleModel Alignment { get; set; }
        public string NumberFormat { get; set; }

        public StyleModel()
        {
            Font = new FontStyleModel();
            Border = new BorderStyleModel();
            Fill = new FillStyleModel();
            Alignment = new AlignmentStyleModel();
            NumberFormat = "General";
        }

        public StyleModel Clone()
        {
            return new StyleModel
            {
                Font = Font?.Clone(),
                Border = Border?.Clone(),
                Fill = Fill?.Clone(),
                Alignment = Alignment?.Clone(),
                NumberFormat = NumberFormat
            };
        }
    }

    /// <summary>
    /// 폰트 서식
    /// </summary>
    public sealed class FontStyleModel
    {
        public string Name { get; set; } = "맑은 고딕";
        public double Size { get; set; } = 11;
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underline { get; set; }
        public bool Strikethrough { get; set; }
        public Color Color { get; set; } = Color.Black;

        public FontStyleModel Clone()
        {
            return new FontStyleModel
            {
                Name = Name,
                Size = Size,
                Bold = Bold,
                Italic = Italic,
                Underline = Underline,
                Strikethrough = Strikethrough,
                Color = Color
            };
        }
    }

    /// <summary>
    /// 보더(테두리) 서식
    /// </summary>
    public sealed class BorderStyleModel
    {
        public BorderEdge Left { get; set; } = new BorderEdge();
        public BorderEdge Top { get; set; } = new BorderEdge();
        public BorderEdge Right { get; set; } = new BorderEdge();
        public BorderEdge Bottom { get; set; } = new BorderEdge();

        public bool HasAnyBorder =>
            Left.Style != BorderLineStyle.None ||
            Top.Style != BorderLineStyle.None ||
            Right.Style != BorderLineStyle.None ||
            Bottom.Style != BorderLineStyle.None;

        public BorderStyleModel Clone()
        {
            return new BorderStyleModel
            {
                Left = Left.Clone(),
                Top = Top.Clone(),
                Right = Right.Clone(),
                Bottom = Bottom.Clone()
            };
        }
    }

    /// <summary>
    /// 보더 한 변
    /// </summary>
    public sealed class BorderEdge
    {
        public BorderLineStyle Style { get; set; } = BorderLineStyle.None;
        public Color Color { get; set; } = Color.Black;

        public BorderEdge Clone()
        {
            return new BorderEdge { Style = Style, Color = Color };
        }
    }

    /// <summary>
    /// 보더 라인 스타일
    /// </summary>
    public enum BorderLineStyle
    {
        None = 0,
        Thin,
        Medium,
        Thick,
        Dashed,
        Dotted,
        Double
    }

    /// <summary>
    /// 채우기(배경) 서식
    /// </summary>
    public sealed class FillStyleModel
    {
        public Color BackgroundColor { get; set; } = Color.Empty;
        public FillPattern PatternType { get; set; } = FillPattern.None;

        public bool HasFill => PatternType != FillPattern.None && BackgroundColor != Color.Empty;

        public FillStyleModel Clone()
        {
            return new FillStyleModel
            {
                BackgroundColor = BackgroundColor,
                PatternType = PatternType
            };
        }
    }

    /// <summary>
    /// 채우기 패턴
    /// </summary>
    public enum FillPattern
    {
        None = 0,
        Solid
    }

    /// <summary>
    /// 정렬 서식
    /// </summary>
    public sealed class AlignmentStyleModel
    {
        public HorizontalAlign Horizontal { get; set; } = HorizontalAlign.General;
        public VerticalAlign Vertical { get; set; } = VerticalAlign.Bottom;
        public bool WrapText { get; set; }
        public int TextRotation { get; set; }

        public AlignmentStyleModel Clone()
        {
            return new AlignmentStyleModel
            {
                Horizontal = Horizontal,
                Vertical = Vertical,
                WrapText = WrapText,
                TextRotation = TextRotation
            };
        }
    }

    /// <summary>
    /// 수평 정렬
    /// </summary>
    public enum HorizontalAlign
    {
        General = 0,
        Left,
        Center,
        Right
    }

    /// <summary>
    /// 수직 정렬
    /// </summary>
    public enum VerticalAlign
    {
        Top = 0,
        Middle,
        Bottom
    }
}
