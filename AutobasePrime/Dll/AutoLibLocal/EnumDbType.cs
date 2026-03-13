using System;

namespace AutoLibLocal
{
	/// <summary>
	/// 
	/// </summary>
	public enum EnumDbType
	{
		Normal=0,
		SQLServer=1,
		Oracle=2,
		MDB=3,
		MySQL=4,            // 2023-5-4 체크해본결과 mongodb도 이 문법을 사용하는듯하다.
        DB2=5,
        SQLServerCE=6,
        Tibero=7,
	}
}
