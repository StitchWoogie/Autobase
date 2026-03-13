using System;
using System.Collections;
using GraphicModule;
using System.IO;
using NetTools;

namespace Studio
{
	public class ListAnimation
	{
		public string name;
		public string path;
		public string ani_file;
	}

	/// <summary>
	/// Summary description for LibraryListDigitalAnimation.
	/// </summary>
	public class LibraryListAnimation : LibraryList
	{
		public LibraryListAnimation()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		void CallBackDigital(object obj2)
		{
			ObjectAnimation obj = (ObjectAnimation)obj2;

			ListAnimation ani = new ListAnimation();
			ani.ani_file = obj.ObjectArgs.sAnimationFile;

            tempName = Path.GetFileNameWithoutExtension(tempName);

			if(tempNo == 0)		ani.name = tempName;
			else				ani.name = tempName+tempNo.ToString();

            ani.path = tempDir.Substring(tempRoot.Length + 1);
			tempItem.Add(ani);

			tempNo++;
		}

		protected override void WriteOneLine(TextWriter writer, object obj)
		{
			ListAnimation item = (ListAnimation)obj;
			writer.WriteLine("Item,{0},{1},{2},", item.name, item.path, item.ani_file);
		}

		protected override object ReadOneLine(CommaBlockString comma)
		{
			ListAnimation item = new ListAnimation();
			comma.GetString(ref item.name);
			comma.GetString(ref item.path);
			comma.GetString(ref item.ani_file);
			return item;
		}
		
		protected override void Call(ObjectGroup group) 
		{
			group.CallBackObject("GraphicModule.ObjectAnimation", new ObjectExpand.DelegateCallBackObject(CallBackDigital));
		}

		public void Make(System.Windows.Forms.Form form)
		{
			MakeListAll(form, "Animation10.lstx");
		}

		public void Load(ArrayList array)
		{
			LoadListAll("Animation10.lstx", array);
		}
	}
}
