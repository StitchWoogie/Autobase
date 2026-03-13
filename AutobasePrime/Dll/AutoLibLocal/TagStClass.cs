using System;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for TagStClass.
	/// </summary>
	/// 
	[Serializable]
	public class TagStClass : TagPublicClass
	{
		// 파일에 저장.
		//public sbyte	act;


		public ushort 	address;
		public byte		read_size;				// 몇개를 읽을 것이냐 ? 1~255
		public byte		read_method;			// 어떻게 읽어올 것이냐 ?
		public sbyte	cMemoryType;			// 어떤 메모리에서 읽어올 것이냐?

        public string stReserved1;
        public string stReserved2;

		public string 	curr="";				// null 이면 스크립트에서 object를 검사못한다.

		public TagStClass()
		{
			//
			// TODO: Add constructor logic here
			//
			this.enumTagType = EnumTagType.ST;
		}

        public override object GetCurr()
        {
            return curr;
        }
	}
}
