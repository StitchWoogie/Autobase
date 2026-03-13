using System;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ObjectGeneral.
	/// </summary>
	[Serializable]
	public class ObjectGeneral
	{
		public string sClassName;

		public void GetClassName(out string name) { name = sClassName; }
		public string GetClassName() { return sClassName; }
		public void SetClassName(string name) { sClassName = name; }

		public sbyte bUseToolTip;				// 툴팁의 사용 여부
		public string sObjectDescription="";	// 오브젝트의 설명. 툴팁 활성 시에 사용한다.

        public bool bResponseOnVisible = false; // 오브젝트가 표시된 상태에서만 마우스 응답 기능 
                                                // 9.3.5 부터 추가 ObjectTag에만 있는 것을 전체 오브젝트에 기능을 지원하였다.

        public float fRotateAngle = 0;          // 10.0 부터 지원
        public string sOnStudioTitle = "";      // 스튜디에에서 사용하는 고유 이름
        public bool bOnStudioLocked = false;    // 스튜디오에서 편집 시에만 사용하는 기능
        public bool bOnStudioVisible = true;    // 스튜디오에서 편집 시에만 사용하는 기능
		
		public ObjectGeneral()
		{
			//
			// TODO: Add constructor logic here
			//
			sClassName = "NoName";
		}

		public ObjectGeneral(string class_name)
		{
			//
			// TODO: Add constructor logic here
			//
			sClassName = class_name;
		}
	}
}
