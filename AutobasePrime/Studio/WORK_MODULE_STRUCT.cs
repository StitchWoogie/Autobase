using System;
using GraphicModule;
using AutoLib;

namespace Studio
{
	/// <summary>
	/// Summary description for WORK_MODULE_STRUCT.
	/// </summary>
	public class WORK_MODULE_STRUCT
	{
		public WORK_MODULE_STRUCT()
		{
			//
			// TODO: Add constructor logic here
			//
			obj = new ObjectRoot();
		}

		public ObjectRoot       obj;
		public bool		        bChangeFlag;	// 화면이 갱신 되었는가를 나타낸다.

        public int nSelectCount
        {
            get
            {
                if (selectList == null) return 0;
                return selectList.Length;
            }
        }
		
        public SELECT_LIST[]    selectList;	    // 현재 선택되어 있는 object의 list
		public SELECT_LIST[]    selectListOld;	// Object를 Move하기전의 포인트 값들 

        public SELECT_LIST[] selectListOnlyParent;
        public SELECT_LIST[] selectListOnlyChild;

        public int nSelectCountOnlyParent
        {
            get
            {
                if (selectListOnlyParent == null) return 0;
                return selectListOnlyParent.Length;
            }
        }

        public int nSelectCountOnlyChild
        {
            get
            {
                if (selectListOnlyChild == null) return 0;
                return selectListOnlyChild.Length;
            }
        }
        
		public bool			bScrollHor;
		public bool			bScrollVer;
		public int			nScrollHorPos;
		public int			nScrollHorHap;
		public int			nScrollVerPos;
		public int			nScrollVerHap;
		public string		filename;

        public object parentSelectList;
	}
}
