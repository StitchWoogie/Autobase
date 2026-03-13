using System;
using AutoLibLocal;

namespace Studio
{
	public enum EnumMainTool
	{
		ARROW, 
		LINE,
		RECT,
		RECT_FILL,
		CIRCLE,
		CIRCLE_FILL,
		POLY,
		POLY_FILL,
		TEXT,
		ROUND_RECTANGLE,
		CURVE,
		POINT,
        SPUIT,  // 스포이트
	}

	/// <summary>
	/// Summary description for SharedStudio.
	/// </summary>
	public class SharedStudio
	{
		
		public static StudioMain formMain = null;
		public static FormSolution formSolution = null;
		
		public SharedStudio()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
