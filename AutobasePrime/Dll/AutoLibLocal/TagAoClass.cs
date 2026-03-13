using System;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for TagAoClass.
	/// </summary>
	/// 
	[Serializable]
	public class TagAoClass : TagPublicClass
	{
		
		public uint 		address;
		public string 		sExtraAddr;	// PLC 기종에 따라 틀리다, FUJI(B, M, D...) MELSEC(X, Y, ...)
		public ushort		wExtraAddr;		// 어드레스 이외에 통신에 필요할 수 있는 값을 넣을수 있게 했다.
		public short 		fn;
		public string  		unit;
        public double fBase;
        public double fFull;
		public short		nCalculateFilter;		// 계산 방법, 0 - 계산하지 않고 그대로 출력
        public double plc_base;				// 출력 최소값
        public double plc_full;				// 출력 최대값

		public sbyte		cDdeDataFormat;			// DDE 출력시 데이터 형태
		public sbyte		bBcdValue;				// BCD로 출력할 것인가?
		public sbyte		bCutOverValue;			// 초과치는 자른다.

        public string       sCalcScript;            // 계산용 스크립트

		public double	curr;
		public double	old;
		public ushort	real_curr;

		public TagAoClass()
		{
			//
			// TODO: Add constructor logic here
			//
			this.enumTagType = EnumTagType.AO;
		}

        public override object GetCurr()
        {
            return curr;
        }
	}
}
