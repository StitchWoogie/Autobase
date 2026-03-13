using System;
using NetTools.OldDefine;

namespace Studio
{
	/// <summary>
	/// Summary description for SELECT_LIST.
	/// </summary>
	public struct SELECT_LIST
	{
		public int x1;
		public int y1;
		public int x2;
		public int y2;
		//public int pos;
        //public GraphicModule.ObjectPublicGroupLayer parent;
        public int[] opos;
        public object obj;
	}

	public enum EnumSelect
	{
		SELECT_NONE,
		SELECT_MID,
		SELECT_LEFT_TOP,
		SELECT_RIGHT_TOP,
		SELECT_LEFT_BOTTOM,
		SELECT_RIGHT_BOTTOM,
		SELECT_MID_LEFT,
		SELECT_MID_TOP,
		SELECT_MID_RIGHT,
		SELECT_MID_BOTTOM,
		SELECT_POPUP_MENU,
	}



	
}
