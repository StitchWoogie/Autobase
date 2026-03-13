using System;

namespace Reporting.Engine.Model
{
    /// <summary>
    /// 셀 주소 (행, 열). 1-based 인덱스.
    /// </summary>
    public struct CellAddress : IComparable<CellAddress>, IEquatable<CellAddress>
    {
        public int Row { get; }
        public int Col { get; }

        public CellAddress(int row, int col)
        {
            Row = row;
            Col = col;
        }

        /// <summary>
        /// 엑셀식 주소 문자열 반환 (예: "A1", "B2", "AA10")
        /// </summary>
        public override string ToString()
        {
            return ColumnToLetter(Col) + Row.ToString();
        }

        /// <summary>
        /// 엑셀식 주소 문자열로부터 CellAddress 생성 (예: "A1" -> (1,1))
        /// </summary>
        public static CellAddress Parse(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("주소 문자열이 비어있습니다.");

            int i = 0;
            // '$' 기호 건너뛰기
            if (i < address.Length && address[i] == '$') i++;

            int col = 0;
            while (i < address.Length && char.IsLetter(address[i]))
            {
                col = col * 26 + (char.ToUpper(address[i]) - 'A' + 1);
                i++;
            }

            if (i < address.Length && address[i] == '$') i++;

            int row = 0;
            while (i < address.Length && char.IsDigit(address[i]))
            {
                row = row * 10 + (address[i] - '0');
                i++;
            }

            if (row == 0 || col == 0)
                throw new ArgumentException($"잘못된 셀 주소: {address}");

            return new CellAddress(row, col);
        }

        public static bool TryParse(string address, out CellAddress result)
        {
            result = default;
            try
            {
                result = Parse(address);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 열 번호를 알파벳 문자로 변환 (1=A, 2=B, ..., 27=AA)
        /// </summary>
        public static string ColumnToLetter(int col)
        {
            string result = "";
            while (col > 0)
            {
                col--;
                result = (char)('A' + col % 26) + result;
                col /= 26;
            }
            return result;
        }

        /// <summary>
        /// 알파벳 문자를 열 번호로 변환 (A=1, B=2, ..., AA=27)
        /// </summary>
        public static int LetterToColumn(string letter)
        {
            int col = 0;
            for (int i = 0; i < letter.Length; i++)
            {
                col = col * 26 + (char.ToUpper(letter[i]) - 'A' + 1);
            }
            return col;
        }

        public int CompareTo(CellAddress other)
        {
            int cmp = Row.CompareTo(other.Row);
            return cmp != 0 ? cmp : Col.CompareTo(other.Col);
        }

        public bool Equals(CellAddress other) => Row == other.Row && Col == other.Col;
        public override bool Equals(object obj) => obj is CellAddress a && Equals(a);
        public override int GetHashCode() => (Row * 397) ^ Col;

        public static bool operator ==(CellAddress a, CellAddress b) => a.Equals(b);
        public static bool operator !=(CellAddress a, CellAddress b) => !a.Equals(b);
        public static bool operator <(CellAddress a, CellAddress b) => a.CompareTo(b) < 0;
        public static bool operator >(CellAddress a, CellAddress b) => a.CompareTo(b) > 0;
    }
}
