using System;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for FIELD_VIEW_STRUCT.
	/// </summary>
	[Serializable]
	public class FIELD_VIEW_STRUCT 
	{
		public string FieldName;
		public string DispName;
		public int		width;
		public sbyte	active;
		public sbyte	cAlign;
		public int	  nFieldPosOnDs;
	} 
}
