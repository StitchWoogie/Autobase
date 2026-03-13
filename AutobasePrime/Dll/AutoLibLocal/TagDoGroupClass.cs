using System;
using System.Collections;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for TagDoClass.
	/// </summary>
	/// 
	[Serializable]
	public class TagDoGroupClass : TagPublicClass
	{
		public ArrayList member = new ArrayList();

		public sbyte		curr;						// flag ON/OFF;

		public TagDoGroupClass()
		{
			//
			// TODO: Add constructor logic here
			//
			this.enumTagType = EnumTagType.GDO;
		}

        public override object CopyObjectOnStudio()
		{
			return NetTools.Tools.CopyObject(this);
			

		}
	}

	[Serializable]
	public class DoGroupMember
	{
		public string tag;

		[NonSerialized]
		public int[] pos = new int[1];
	}
}
