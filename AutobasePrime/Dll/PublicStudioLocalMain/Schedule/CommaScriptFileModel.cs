using System;
using PublicStudioLocalMain.Schedule;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for CommaScriptFileModel.
	/// </summary>
	public class CommaScriptFileModel : CommaScriptFileClass
	{
		public SCHEDULE_MODEL_STRUCT model = new SCHEDULE_MODEL_STRUCT();

		public CommaScriptFileModel()
		{
			//
			// TODO: Add constructor logic here
			//
			model.blockItem = new System.Collections.ArrayList();
		}

		protected override void OnCommand(string command, NetTools.CommaBlockString comma)
		{
			base.OnCommand (command, comma);

			if(String.Compare(command, "Title", true) == 0) 
			{
				comma.GetString(ref model.title);
			}
			else if(String.Compare(command, "Description", true) == 0) 
			{
				comma.GetString(ref model.description);
			}
			else if(String.Compare(command, "Member", true) == 0) 
			{
				CommaScriptFileModelItem scr = new CommaScriptFileModelItem();
				scr.Execute(in_pointer, command, comma);
				model.blockItem.Add(scr.item);
			}
			else {}
		}

	}
}
