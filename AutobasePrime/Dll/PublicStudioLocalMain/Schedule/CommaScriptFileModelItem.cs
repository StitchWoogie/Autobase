using System;
using PublicStudioLocalMain.Schedule;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for CommaScriptFileModelItem.
	/// </summary>
	public class CommaScriptFileModelItem : CommaScriptFileClass
	{
		public SCHEDULE_MODEL_ITEM_STRUCT item = new SCHEDULE_MODEL_ITEM_STRUCT();

		public CommaScriptFileModelItem()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		protected override void OnCommand(string command, NetTools.CommaBlockString comma)
		{
			base.OnCommand (command, comma);

			if(String.Compare(command, "Time", true) == 0) 
			{
				comma.GetInt(ref item.hour);
				comma.GetInt(ref item.minute);
			}
            else if (String.Compare(command, "TimeType", true) == 0)
            {
                comma.GetInt(ref item.nTimeType);
            }
            else if (String.Compare(command, "SunControl", true) == 0)
            {
                comma.GetInt(ref item.nSunBeforeAfterMinutes);
                comma.GetString(ref item.sLocation);
            }
			else if(String.Compare(command, "Script", true) == 0) 
			{
				CommaScriptFileModelScript scr = new CommaScriptFileModelScript();
				scr.Execute(in_pointer, command, comma);
				item.script = scr.str;
			}
			else if(String.Compare(command, "TagValue", true) == 0) 
			{
				if(item.blockTag == null) 
				{
					item.blockTag = new System.Collections.ArrayList();
				}
		
				string buf = "";
				SCHEDULE_TAG_VALUE_STRUCT tag;

				tag = new SCHEDULE_TAG_VALUE_STRUCT();

				comma.GetString(ref buf);
				tag.tag = buf;
				comma.GetString(ref buf);
				tag.val = buf;

				item.blockTag.Add(tag);
			}
			else {}
		}

	}
}
