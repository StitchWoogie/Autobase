using PublicStudioLocalMain;
using Studio.OPCUA;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Studio
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

        //20241010 PSU Form owner �߰�
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

        public static bool SelectItemUA(Form owner, out string servername, out string groupname, out string itemname) // OPC UA 24-09-02 �߰� hsjeong
        {
            servername = "";
            groupname = "";
            itemname = "";

            IOpcBrowseProvider provider = CreateUaBrowseProvider();

            FormSelectItem dialog = new FormSelectItem(provider);
            dialog.bUseOpcUAClient = true;

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

        /// <summary>
        /// OPC UA Client 프로세스가 실행 중이면 IPC Provider,
        /// 아니면 UAServer.ini 오프라인 Provider를 반환한다.
        /// </summary>
        private static IOpcBrowseProvider CreateUaBrowseProvider()
        {
            // 1) IPC 연결이 이미 있으면 사용
            if (OpcUaIpcManager.Client != null)
                return new OpcUaIpcBrowseProvider();

            // 2) OPC UA Client 프로세스가 실행 중이면 IPC 직접 연결 시도
            if (IsOpcUaClientRunning())
            {
                bool connected = OpcUaIpcManager
                    .TryConnectOnceAsync(3000)
                    .GetAwaiter().GetResult();

                if (connected && OpcUaIpcManager.Client != null)
                    return new OpcUaIpcBrowseProvider();
            }

            // 3) 오프라인: UAServer.ini 에서 트리 정보 로드
            string iniPath = Path.Combine(
                AutoLibLocal.TotalConfig.sDirWorkProject,
                "OpcData", "UaClient", "UAServer.ini");

            return new OpcUaOfflineBrowseProvider(iniPath);
        }

        private static bool IsOpcUaClientRunning()
        {
            try
            {
                string clientPath = System.Windows.Forms.Application.StartupPath
                    + "\\Autobase_OPCUA_Client.exe";

                foreach (var p in Process.GetProcessesByName("Autobase_OPCUA_Client"))
                {
                    try
                    {
                        if (p.MainModule.FileName.Equals(
                            clientPath, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                    catch { }
                }
            }
            catch { }
            return false;
        }



	}
}
