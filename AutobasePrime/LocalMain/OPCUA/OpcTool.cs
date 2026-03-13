using LocalMain.OPCUA;
using PublicStudioLocalMain;
using System;
using System.Windows.Forms;

namespace LocalMain
{
	/// <summary>
	/// Summary description for OpcTool.
	/// </summary>
	public class OpcTool
	{
		public OpcTool()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        //20241010 PSU Form owner 추가
		public static bool SelectItem(Form owner, out string servername, out string groupname, out string itemname)
		{
			servername = "";
			groupname = "";
			itemname = "";


			FormSelectItem dialog = new FormSelectItem(new OpcDaRegistryBrowseProvider());

			//using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			//{
			//	dialog.arrayServer = share.GetServerList();
			//}
			dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(owner) == DialogResult.OK) 
			{
				servername = dialog.sServer;
				groupname = dialog.sGroup;
				itemname = dialog.sItem;
				return true;
			}

			return false;
		}

        public static bool SelectItemUA(Form owner, out string servername, out string groupname, out string itemname) // OPC UA 24-09-02 추가 hsjeong
        {
            servername = "";
            groupname = "";
            itemname = "";

			IOpcBrowseProvider provider = new OpcUaIpcBrowseProvider();
                FormSelectItem dialog = new FormSelectItem(provider);
            dialog.bUseOpcUAClient = true;

            //using(ComOpcShare.Share share = new ComOpcShare.Share()) 
            //{
            //	dialog.arrayServer = share.GetServerList();
            //}

            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(owner) == DialogResult.OK)
            {
                servername = dialog.sServer;
                groupname = dialog.sGroup;
                itemname = dialog.sItem;
                return true;
            }

            return false;
        }



	}
}
