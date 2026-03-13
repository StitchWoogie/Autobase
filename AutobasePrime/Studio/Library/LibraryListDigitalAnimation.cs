using System;
using System.Collections;
using GraphicModule;
using System.IO;
using NetTools;

namespace Studio
{
	public class ListDigitalAnimation
	{
		public string name;
		public string path;
		public string on_file;
		public string off_file;
	}

	/// <summary>
	/// Summary description for LibraryListDigitalAnimation.
	/// </summary>
	public class LibraryListDigitalAnimation : LibraryList
	{
		public LibraryListDigitalAnimation()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		void CallBackDigital(object obj2)
		{
			ObjectDigitalAnimation obj = (ObjectDigitalAnimation)obj2;

			ListDigitalAnimation ani = new ListDigitalAnimation();
			ani.on_file = obj.objArgs.sFileOn;
			ani.off_file = obj.objArgs.sFileOff;

            tempName = Path.GetFileNameWithoutExtension(tempName);

			if(tempNo == 0)		ani.name = tempName;
			else				ani.name = tempName+tempNo.ToString();

			ani.path = tempDir.Substring(tempRoot.Length+1);
			tempItem.Add(ani);

			tempNo++;
		}

		protected override void WriteOneLine(TextWriter writer, object obj)
		{
			ListDigitalAnimation item = (ListDigitalAnimation)obj;
			writer.WriteLine("Item,{0},{1},{2},{3},", item.name, item.path, item.off_file, item.on_file);
		}

		protected override object ReadOneLine(CommaBlockString comma)
		{
			ListDigitalAnimation item = new ListDigitalAnimation();
			comma.GetString(ref item.name);
			comma.GetString(ref item.path);
			comma.GetString(ref item.off_file);
			comma.GetString(ref item.on_file);
			return item;
		}
		
		protected override void Call(ObjectGroup group) 
		{
			group.CallBackObject("GraphicModule.ObjectDigitalAnimation", new ObjectExpand.DelegateCallBackObject(CallBackDigital));
		}

		public void Make(System.Windows.Forms.Form form)
		{
			MakeListAll(form, "DigitalAnimation10.lstx");
		}

		public void Load(ArrayList array)
		{
			LoadListAll("DigitalAnimation10.lstx", array);
		}
	}
}
