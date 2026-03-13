namespace Reporting.Engine.Model
{
    /// <summary>
    /// 셀 데이터 타입
    /// </summary>
    public enum CellDataType
    {
        String = 0,
        Number,
        DateTime,
        Boolean,
        Formula,
        Error
    }

    /// <summary>
    /// 셀 데이터 모델
    /// </summary>
    public sealed class CellModel
    {
        /// <summary>
        /// 셀 값 (string, double, DateTime, bool, null)
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 수식 문자열 ("=SUM(A1:A10)" 또는 "=AiAve,tag,..." 등)
        /// null이면 수식 없음
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// 셀 서식. null이면 기본 서식 사용
        /// </summary>
        public StyleModel Style { get; set; }

        /// <summary>
        /// 셀 데이터 타입
        /// </summary>
        public CellDataType DataType { get; set; } = CellDataType.String;

        /// <summary>
        /// 수식이 있는지 여부
        /// </summary>
        public bool HasFormula => !string.IsNullOrEmpty(Formula);

        /// <summary>
        /// 표시용 텍스트 반환
        /// </summary>
        public string GetDisplayText()
        {
            if (Value == null) return string.Empty;
            if (Style?.NumberFormat != null && Style.NumberFormat != "General")
            {
                return FormatValue(Value, Style.NumberFormat);
            }
            return Value.ToString();
        }

        private static string FormatValue(object value, string format)
        {
            try
            {
                if (value is double d)
                    return d.ToString(format);
                if (value is System.DateTime dt)
                    return dt.ToString(format);
                return value.ToString();
            }
            catch
            {
                return value.ToString();
            }
        }

        public CellModel Clone()
        {
            return new CellModel
            {
                Value = Value,
                Formula = Formula,
                Style = Style?.Clone(),
                DataType = DataType
            };
        }
    }
}
