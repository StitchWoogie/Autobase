using System.Collections.Generic;
using System.Linq;

namespace Reporting.Engine.Model
{
    /// <summary>
    /// 워크시트 모델
    /// </summary>
    public sealed class SheetModel
    {
        /// <summary>시트 이름</summary>
        public string Name { get; set; }

        /// <summary>셀 데이터 (희소 구조)</summary>
        public SortedDictionary<CellAddress, CellModel> Cells { get; }

        /// <summary>열 너비 (1-based 인덱스, 단위: 문자폭 기준)</summary>
        public Dictionary<int, double> ColumnWidths { get; }

        /// <summary>행 높이 (1-based 인덱스, 단위: 포인트)</summary>
        public Dictionary<int, double> RowHeights { get; }

        /// <summary>병합 셀 범위 목록</summary>
        public List<MergedRange> MergedRanges { get; }

        /// <summary>페이지 설정</summary>
        public PageSetupModel PageSetup { get; set; }

        /// <summary>기본 열 너비</summary>
        public double DefaultColumnWidth { get; set; } = 8.43;

        /// <summary>기본 행 높이</summary>
        public double DefaultRowHeight { get; set; } = 15.0;

        public SheetModel()
        {
            Name = "Sheet1";
            Cells = new SortedDictionary<CellAddress, CellModel>();
            ColumnWidths = new Dictionary<int, double>();
            RowHeights = new Dictionary<int, double>();
            MergedRanges = new List<MergedRange>();
            PageSetup = new PageSetupModel();
        }

        public SheetModel(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// 셀 가져오기. 없으면 null 반환.
        /// </summary>
        public CellModel GetCell(int row, int col)
        {
            var addr = new CellAddress(row, col);
            return Cells.TryGetValue(addr, out var cell) ? cell : null;
        }

        /// <summary>
        /// 셀 가져오기. 없으면 새로 생성하여 반환.
        /// </summary>
        public CellModel GetOrCreateCell(int row, int col)
        {
            var addr = new CellAddress(row, col);
            if (!Cells.TryGetValue(addr, out var cell))
            {
                cell = new CellModel();
                Cells[addr] = cell;
            }
            return cell;
        }

        /// <summary>
        /// 셀 설정
        /// </summary>
        public void SetCell(int row, int col, CellModel cell)
        {
            Cells[new CellAddress(row, col)] = cell;
        }

        /// <summary>
        /// 셀 삭제
        /// </summary>
        public void RemoveCell(int row, int col)
        {
            Cells.Remove(new CellAddress(row, col));
        }

        /// <summary>
        /// 열 너비 반환 (없으면 기본값)
        /// </summary>
        public double GetColumnWidth(int col)
        {
            return ColumnWidths.TryGetValue(col, out var w) ? w : DefaultColumnWidth;
        }

        /// <summary>
        /// 행 높이 반환 (없으면 기본값)
        /// </summary>
        public double GetRowHeight(int row)
        {
            return RowHeights.TryGetValue(row, out var h) ? h : DefaultRowHeight;
        }

        /// <summary>
        /// 사용 중인 행 범위 (최대 행 번호). 셀이 없으면 0.
        /// </summary>
        public int UsedRowCount
        {
            get
            {
                if (Cells.Count == 0) return 0;
                return Cells.Keys.Max(a => a.Row);
            }
        }

        /// <summary>
        /// 사용 중인 열 범위 (최대 열 번호). 셀이 없으면 0.
        /// </summary>
        public int UsedColumnCount
        {
            get
            {
                if (Cells.Count == 0) return 0;
                return Cells.Keys.Max(a => a.Col);
            }
        }

        /// <summary>
        /// 지정한 셀이 병합 범위에 포함되는지 확인. 포함되면 해당 MergedRange 반환.
        /// </summary>
        public MergedRange FindMergedRange(int row, int col)
        {
            for (int i = 0; i < MergedRanges.Count; i++)
            {
                if (MergedRanges[i].Contains(row, col))
                    return MergedRanges[i];
            }
            return null;
        }

        public SheetModel Clone()
        {
            var sheet = new SheetModel(Name)
            {
                DefaultColumnWidth = DefaultColumnWidth,
                DefaultRowHeight = DefaultRowHeight,
                PageSetup = PageSetup?.Clone()
            };

            foreach (var kv in Cells)
                sheet.Cells[kv.Key] = kv.Value.Clone();

            foreach (var kv in ColumnWidths)
                sheet.ColumnWidths[kv.Key] = kv.Value;

            foreach (var kv in RowHeights)
                sheet.RowHeights[kv.Key] = kv.Value;

            foreach (var mr in MergedRanges)
                sheet.MergedRanges.Add(mr.Clone());

            return sheet;
        }
    }
}
