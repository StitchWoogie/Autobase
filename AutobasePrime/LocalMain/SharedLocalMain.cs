using System;
using System.Collections;
using System.IO;
using NetTools;
using AutoLibLocal;
using AutoLib;
using NetTools.OldDefine;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using DatabaseConnection;
using DatabaseSaveList;
using DialogTag;

namespace LocalMain
{
	/// <summary>
	/// Summary description for SharedRunMain.
	/// </summary>
	public class SharedLocalMain 
	{
		//public ConnectionStringList dsnList = new ConnectionStringList();
		
		public static bool bTestMode = TotalConfig.GetAutoBaseTestMode();
		public static bool bScanPauseFlag = false;

		public SharedLocalMain()
		{
			//
			// TODO: Add constructor logic here
			//
			//dsnList.ConnectionStringLoad();
		}


	}
}

