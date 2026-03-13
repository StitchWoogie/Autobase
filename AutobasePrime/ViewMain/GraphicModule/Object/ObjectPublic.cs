using System;
using NetTools.OldDefine;
using System.Drawing;
using NetTools;
using AutoLibLocal;
using System.IO;

namespace GraphicModule
{
	[Serializable]
	public class ObjectCommonProperty
	{
        [NonSerialized]
        public System.Windows.Forms.Form form;  // 이것은 9.5.2 RunTime시 ExecuteClassName에서 this를 구분할 수 있게 지원하였다. (다른곳에서 사용 시 주의가 필요하다.)

		public string sModuleName;

        // 로딩시에만 필요하므로 복사할 필요 없다.
        [NonSerialized] 
        public bool bLoadByZipStream = false;   // Zip 파일 스트림에서 로딩한다.
        [NonSerialized] 
        public MemoryStream streamZip;
        [NonSerialized]
        public bool bLoadOnLibraryView;             // Library View 에서의 로딩이다.
	}

	/// <summary>
	/// Summary description for ObjectPublic.
	/// </summary>
	[Serializable]
	public class ObjectPublic : ObjectType
	{
		bool	bUseScriptLocationX;
		bool	bUseScriptLocationY;
		
		POINT pBasePoint = new POINT();

        protected int nScreenSizeX;				// 사용자 화면 사이즈
        protected int nScreenSizeY;
        protected int nModuleSizeX;				// 그래픽 모듈 전체의 그림 사이즈
        protected int nModuleSizeY;
		protected int  cObjectShowMethod;			// 보여주는 option  Font에서도 참조한다.
		int  wOpticRate;
		
		SIZE sizeGroup = new SIZE();
		protected RECT rGroupZone = new RECT();
		public ObjectCommonProperty objCommonProperty;
        
		public ObjectPublic(ObjectCommonProperty cp)
		{
			//
			// TODO: Add constructor logic here
			//
			objCommonProperty = cp;
			pBasePoint.x = 0;
			pBasePoint.y = 0;

			nScreenSizeX = 0;
			nScreenSizeY = 0;
			nModuleSizeX = 0;
			nModuleSizeY = 0;
			cObjectShowMethod = 0;
			wOpticRate = 100;

			bUseScriptLocationX = false;	// 확장 스크립트를 사용하지 않는다.
			bUseScriptLocationY = false;
		}

		public virtual void SetBasePoint(int x, int y)
        {
			pBasePoint.x = x;
			pBasePoint.y = y;
		}

        public int GetViewPosXOn100Percent(int x)
        {
            // Object가 100%일 때 화면상에서 위치할 좌표를 찾는다.
            if (sizeGroup.cx == Math.Abs(rGroupZone.right - rGroupZone.left + 1))
            {
                x += rGroupZone.left;
            }
            else
            {
                int sizex = rGroupZone.right - rGroupZone.left + 1;

                if (sizeGroup.cx != 0) x = x * sizex / sizeGroup.cx;

                x += rGroupZone.left;
            }

            //x += pBasePoint.x;
            return x;
        }

        public int GetViewPosYOn100Percent(int y)
        {
            // Object가 100%일 때 화면상에서 위치할 좌표를 찾는다.
            if (sizeGroup.cy == Math.Abs(rGroupZone.bottom - rGroupZone.top + 1))
            {
                y += rGroupZone.top;
            }
            else
            {
                int sizey = rGroupZone.bottom - rGroupZone.top + 1;

                if (sizeGroup.cy != 0) y = y * sizey / sizeGroup.cy;

                y += rGroupZone.top;
            }

            //y += pBasePoint.y;
            return y;
        }

        /*
        public int GetGroupPosXOn100Percent(int x)
        {
            // Object가 100%일 때 그림상에서 위치할 좌표를 찾는다.
            if (sizeGroup.cx == Math.Abs(rGroupZone.right - rGroupZone.left + 1))
            {
                x += rGroupZone.left;
            }
            else
            {
                int sizex = rGroupZone.right - rGroupZone.left + 1;

                if (sizeGroup.cx != 0) x = x * sizex / sizeGroup.cx;

                x += rGroupZone.left;
            }

            x += pBasePoint.x;
            return x;
        }

        public int GetGroupPosYOn100Percent(int y)
        {
            // Object가 100%일 때 그림상에서 위치할 좌표를 찾는다.
            if (sizeGroup.cy == Math.Abs(rGroupZone.bottom - rGroupZone.top + 1))
            {
                y += rGroupZone.top;
            }
            else
            {
                int sizey = rGroupZone.bottom - rGroupZone.top + 1;

                if (sizeGroup.cy != 0) y = y * sizey / sizeGroup.cy;

                y += rGroupZone.top;
            }

            y += pBasePoint.y;
            return y;
        }*/

		public int GetViewPosX(int x)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
				// 확장 스크립트 사용하지 않으면 그룹 좌표를 찾는다.
				// 먼저 Object가 100%일 때 그림상에서 위치할 좌표를 찾는다.
				if(sizeGroup.cx == Math.Abs(rGroupZone.right-rGroupZone.left+1)) 
				{
					x += rGroupZone.left;
				}
				else 
				{
					int sizex = rGroupZone.right-rGroupZone.left+1;

					if(sizeGroup.cx != 0)	x = x*sizex/sizeGroup.cx;

					x += rGroupZone.left;
				}

				if(wOpticRate == 100)	x += pBasePoint.x;
				else 
				{
					x = x*wOpticRate/100;
					x += pBasePoint.x;
				}
				return x;
			}
			else 
			{
				if(bUseScriptLocationX == false) 
				{	// 확장 스크립트 사용하지 않으면 그룹 좌표를 찾는다.
					// 먼저 Object가 100%일 때 그림상에서 위치할 좌표를 찾는다.
					if(sizeGroup.cx == Math.Abs(rGroupZone.right-rGroupZone.left+1)) 
					{
						x += rGroupZone.left;
					}
					else 
					{
						int sizex = rGroupZone.right-rGroupZone.left+1;

						if(sizeGroup.cx != 0)	x = x*sizex/sizeGroup.cx;

						x += rGroupZone.left;
					}
				}

				if(cObjectShowMethod == 0) 
				{
					if(wOpticRate == 100)	x += pBasePoint.x;
					else 
					{
						x = x*wOpticRate/100;
						x += pBasePoint.x;
					}
					return x;
				}

				if(nModuleSizeX == 0) 
				{
					;
				}
				else 
				{
					x = (int)(((long)nScreenSizeX*x)/nModuleSizeX);
					x += pBasePoint.x;
				}

				return x;
			}
		}

		public int GetViewPosY(int y)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
				// 확장 스크립트 사용하지 않으면 그룹좌표를 찾는다.
				// 먼저 Object가 100%일 때 그림상에서 위치할 좌표를 찾는다.
				if(sizeGroup.cy == Math.Abs(rGroupZone.bottom-rGroupZone.top+1)) 
				{
					y += rGroupZone.top;
				}
				else 
				{
					int sizey = rGroupZone.bottom-rGroupZone.top+1;

					if(sizeGroup.cy != 0)	y = y*sizey/sizeGroup.cy;

					y += rGroupZone.top;
				}

				if(wOpticRate == 100)	y += pBasePoint.y;
				else 
				{
					y = y*wOpticRate/100;
					y += pBasePoint.y;
				}
				return y;
			}
			else 
			{
				if(bUseScriptLocationY == false) 
				{	// 확장 스크립트 사용하지 않으면 그룹좌표를 찾는다.
					// 먼저 Object가 100%일 때 그림상에서 위치할 좌표를 찾는다.
					if(sizeGroup.cy == Math.Abs(rGroupZone.bottom-rGroupZone.top+1)) 
					{
						y += rGroupZone.top;
					}
					else 
					{
						int sizey = rGroupZone.bottom-rGroupZone.top+1;

						if(sizeGroup.cy != 0)	y = y*sizey/sizeGroup.cy;

						y += rGroupZone.top;
					}
				}

				if(cObjectShowMethod == 0) 
				{
					if(wOpticRate == 100)	y += pBasePoint.y;
					else 
					{
						y = y*wOpticRate/100;
						y += pBasePoint.y;
					}
					return y;
				}

				if(nModuleSizeY == 0) 
				{

				}
				else 
				{
					y = (int)(((long)nScreenSizeY*y)/nModuleSizeY);
					y += pBasePoint.y;
				}

				return y;
			}
		}

		public int GetViewSize(int size)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
				if(sizeGroup.cx == Math.Abs(rGroupZone.right-rGroupZone.left+1)) 
				{
					// += rGroupZone.left;
				}
				else 
				{
					int sizex = rGroupZone.right-rGroupZone.left+1;

					if(sizeGroup.cx != 0)	size = size*sizex/sizeGroup.cx;
				}

				if(wOpticRate == 100)	return size;
				else 
				{
					size = size*wOpticRate/100;
					return size;
				}
			}
			else 
			{
				//if(bUseScriptLocationX == OFF) {	// 확장 스크립트 사용하지 않으면 그룹좌표를 찾는다.
				// 먼저 Object가 100%일 때 그림상에서 위치할 좌표를 찾는다.
				if(sizeGroup.cx == Math.Abs(rGroupZone.right-rGroupZone.left+1)) 
				{
					// += rGroupZone.left;
				}
				else 
				{
					int sizex = rGroupZone.right-rGroupZone.left+1;

					if(sizeGroup.cx != 0)	size = size*sizex/sizeGroup.cx;
				}
				//}

				if(cObjectShowMethod == 0) 
				{
					if(wOpticRate == 100)	return size;
					else 
					{
						size = size*wOpticRate/100;
						return size;
					}
				}

				if(nModuleSizeX == 0) 
				{

				}
				else 
				{
					size = (int)(((long)nScreenSizeX*size)/nModuleSizeX);
				}

				return size;
			}
		}

		protected int GetViewSizeX(int size)
		{
			// 먼저 Object가 100%일 때 그림상에서 위치할 좌표를 찾는다.
			if(sizeGroup.cx == Math.Abs(rGroupZone.right-rGroupZone.left+1)) 
			{
				// += rGroupZone.left;
			}
			else 
			{
				int sizex = rGroupZone.right-rGroupZone.left+1;

				if(sizeGroup.cx != 0)	size = size*sizex/sizeGroup.cx;
			}

			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
				

				if(wOpticRate == 100)	return size;
				else 
				{
					size = size*wOpticRate/100;
					return size;
				}
			}
			else 
			{
				if(cObjectShowMethod == 0) 
				{
					if(wOpticRate == 100)	return size;
					else 
					{
						size = size*wOpticRate/100;
						return size;
					}
				}

				if(nModuleSizeX == 0) 
				{

				}
				else 
				{
					size = (int)(((long)nScreenSizeX*size)/nModuleSizeX);
				}

				return size;
			}
		}

		protected int GetViewSizeY(int size)
		{
			// 먼저 Object가 100%일 때 그림상에서 위치할 좌표를 찾는다.
			if(sizeGroup.cy == Math.Abs(rGroupZone.bottom-rGroupZone.top+1)) 
			{
				// += rGroupZone.left;
			}
			else 
			{
				int sizey = rGroupZone.bottom-rGroupZone.top+1;

				if(sizeGroup.cy != 0)	size = size*sizey/sizeGroup.cy;
			}

			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
			
				if(wOpticRate == 100)	return size;
				else 
				{
					size = size*wOpticRate/100;
					return size;
				}
			}
			else 
			{
				

				if(cObjectShowMethod == 0) 
				{
					if(wOpticRate == 100)	return size;
					else 
					{
						size = size*wOpticRate/100;
						return size;
					}
				}

				if(nModuleSizeY == 0) 
				{

				}
				else 
				{
					size = (int)(((long)nScreenSizeY*size)/nModuleSizeY);
				}

				return size;
			}
		}

		public virtual void SetScreenSize(int x, int y)
		{
			if(x == 0)	x = 1024;
			if(y == 0)	y = 768;

			nScreenSizeX = x;
			nScreenSizeY = y;

			// window에 비례하게 맞추어야 하므로 현재 그림에 맞는 window사이즈를  결정한다.
			if(cObjectShowMethod == 1) 
			{
				if(nModuleSizeX == 0) 
					nScreenSizeY = nModuleSizeY;
				else 
					nScreenSizeY = (int)((long)nModuleSizeY*x/nModuleSizeX);

				if(nScreenSizeY > y) 
				{
					if(nModuleSizeY == 0) 
						nScreenSizeX = nModuleSizeX;
					else
						nScreenSizeX = (int)((long)nModuleSizeX*y/nModuleSizeY);

					nScreenSizeY = y;
				}
				else 
				{
					nScreenSizeX = x;
				}
			}
				// window에 비례하게 맞추지 않는다.
			else if(cObjectShowMethod == 2) 
			{
		
			}
		}

        public virtual void SetZoneAtPercent100Expand(SIZE size, RECT r)
		{
			sizeGroup.cx = size.cx;
			sizeGroup.cy = size.cy;
			rGroupZone.left = r.left;
			rGroupZone.top = r.top;
			rGroupZone.right = r.right;
			rGroupZone.bottom = r.bottom;
		}

		public virtual void SetOpticRate(int rate) 
		{ 
			wOpticRate = rate; 
		}

        public void GetOpticRate(out double x, out double y)
        {
			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
                x = wOpticRate/100.0;
                y = x;
                return;
			}
			else 
			{
				if(cObjectShowMethod == 0) 
				{
                    x = wOpticRate/100.0;
                    y = x;
                    return;
				}

				if(nModuleSizeX == 0 || nModuleSizeY == 0) 
				{
                    x = 1;
                    y = 1;
                    return;
				}
				else 
				{
                    x = (double)nScreenSizeX / nModuleSizeX;
                    y = (double)nScreenSizeY / nModuleSizeY;
                    return;
				}
			}
        }

        /*
        public int GetOpticRate()
        {
            return wOpticRate;
        }*/

		public virtual void SetObjectOpticMethod(int method) 
		{ 
			cObjectShowMethod = method; 
		}

		public virtual void SetModuleSize(int x, int y) 
		{ 
			nModuleSizeX = x;
			nModuleSizeY = y; 
		}

		public void SetUseScriptLocationX(bool flag) 
		{ 
			bUseScriptLocationX = flag; 
		}

		public void SetUseScriptLocationY(bool flag) 
		{ 
			bUseScriptLocationY = flag; 
		}

		public virtual void SetCommonProperty(ObjectCommonProperty cp)
		{
			objCommonProperty = cp;
		}
	}
}


