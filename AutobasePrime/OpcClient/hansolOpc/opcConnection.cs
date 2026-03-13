using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Opc;
using Opc.Da;
using OpcCom.Da;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using Opc.Cpx;

namespace OpcClient
{
	/// <summary>
	/// Summary description for opcConnection.
	/// </summary>
	public class opcConnection
	{
		static private WebProxy m_proxy = null;
		public opcConnection()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static public void OnConnectOpc(Opc.Da.Server server)
		{
			//OnDisconnect();			
			// use the specified server object directly.
			ComplexTypeCache.Server = server;

			// connect with an empty configuration.
			//Cursor = Cursors.WaitCursor;
			
			NetworkCredential credentials = null;
			//do
			//{		

			server.Connect(new ConnectData(credentials, m_proxy));
			//break;				

			//	credentials = new NetworkCredentialsDlg().ShowDialog(credentials);
			//}
			//while (credentials != null);	
			//server.SetResultFilters((int)ResultFilter.All);
		}
	}
}
