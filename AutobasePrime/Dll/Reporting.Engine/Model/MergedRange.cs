namespace Reporting.Engine.Model
{
    /// <summary>
    /// 병합 셀 범위
    /// </summary>
    public sealed class MergedRange
    {
        public int FirstRow { get; set; }
        public int FirstCol { get; set; }
        public int LastRow { get; set; }
        public int LastCol { get; set; }

        public MergedRange() { }

        public MergedRange(int firstRow, int firstCol, int lastRow, int lastCol)
        {
            FirstRow = firstRow;
            FirstCol = firstCol;
            LastRow = lastRow;
            LastCol = lastCol;
        }

        /// <summary>
        /// 지정한 셀이 이 병합 범위에 포함되는지 확인
        /// </summary>
        public bool Contains(int row, int col)
        {
            return row >= FirstRow && row <= LastRow && col >= FirstCol && col <= LastCol;
        }

        /// <summary>
        /// 지정한 셀이 이 병합 범위의 시작 셀(좌상단)인지 확인
        /// </summary>
        public bool IsOrigin(int row, int col)
        {
            return row == FirstRow && col == FirstCol;
        }

        /// <summary>
        /// 행 스팬 (병합 행 수)
        /// </summary>
        public int RowSpan => LastRow - FirstRow + 1;

        /// <summary>
        /// 열 스팬 (병합 열 수)
        /// </summary>
        public int ColSpan => LastCol - FirstCol + 1;

        public override string ToString()
        {
            return $"{CellAddress.ColumnToLetter(FirstCol)}{FirstRow}:{CellAddress.ColumnToLetter(LastCol)}{LastRow}";
        }

        public MergedRange Clone()
        {
            return new MergedRange(FirstRow, FirstCol, LastRow, LastCol);
        }
    }
}
