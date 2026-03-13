using System;
using System.Collections.Generic;

namespace Reporting.Engine.Model
{
    /// <summary>
    /// 워크북 모델 (다중 시트 지원)
    /// </summary>
    public sealed class WorkbookModel
    {
        /// <summary>시트 목록</summary>
        public IList<SheetModel> Sheets { get; }

        /// <summary>활성 시트 인덱스 (0-based)</summary>
        public int ActiveSheetIndex { get; set; }

        /// <summary>명명된 스타일</summary>
        public IDictionary<string, StyleModel> NamedStyles { get; }

        public WorkbookModel()
        {
            Sheets = new List<SheetModel>();
            NamedStyles = new Dictionary<string, StyleModel>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 기본 시트 1개로 워크북 생성
        /// </summary>
        public static WorkbookModel CreateDefault()
        {
            var wb = new WorkbookModel();
            wb.Sheets.Add(new SheetModel("Sheet1"));
            wb.ActiveSheetIndex = 0;
            return wb;
        }

        /// <summary>
        /// 활성 시트 반환
        /// </summary>
        public SheetModel ActiveSheet
        {
            get
            {
                if (Sheets.Count == 0) return null;
                if (ActiveSheetIndex < 0 || ActiveSheetIndex >= Sheets.Count)
                    ActiveSheetIndex = 0;
                return Sheets[ActiveSheetIndex];
            }
        }

        /// <summary>
        /// 시트 이름으로 검색
        /// </summary>
        public SheetModel GetSheet(string name)
        {
            for (int i = 0; i < Sheets.Count; i++)
            {
                if (string.Equals(Sheets[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    return Sheets[i];
            }
            return null;
        }

        /// <summary>
        /// 시트 추가
        /// </summary>
        public SheetModel AddSheet(string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = "Sheet" + (Sheets.Count + 1);

            var sheet = new SheetModel(name);
            Sheets.Add(sheet);
            return sheet;
        }

        /// <summary>
        /// 시트 삭제
        /// </summary>
        public bool RemoveSheet(int index)
        {
            if (index < 0 || index >= Sheets.Count || Sheets.Count <= 1)
                return false;

            Sheets.RemoveAt(index);
            if (ActiveSheetIndex >= Sheets.Count)
                ActiveSheetIndex = Sheets.Count - 1;
            return true;
        }

        public WorkbookModel Clone()
        {
            var wb = new WorkbookModel();
            wb.ActiveSheetIndex = ActiveSheetIndex;

            foreach (var sheet in Sheets)
                wb.Sheets.Add(sheet.Clone());

            foreach (var kv in NamedStyles)
                wb.NamedStyles[kv.Key] = kv.Value.Clone();

            return wb;
        }
    }
}
