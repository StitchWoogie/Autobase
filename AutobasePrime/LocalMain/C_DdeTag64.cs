using System;
using AutoLibLocal;
using AutoLib;
using System.Runtime.InteropServices;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// Summary description for C_DdeTag.
	/// </summary>
	public class C_DdeTag64
	{
        [DllImport("Win64Common.DLL", EntryPoint = "LinkItemAdvise")]
        public static extern bool DdeLibLinkItemAdvise(String service, String topic, String item, out uint s, out uint t, out uint i);

        [DllImport("Win64Common.DLL", EntryPoint = "RequestItem")]
        public static extern bool DdeLibRequestItem(uint service, uint topic, uint item, System.Text.StringBuilder data, int length);

        [DllImport("Win64Common.DLL", EntryPoint = "ChangedItem")]
        public static extern bool DdeLibChangedItem(uint service, uint topic, uint item, System.Text.StringBuilder data, int length);

        [DllImport("Win64Common.DLL", EntryPoint = "Transaction")]
        public static extern bool DdeLibTransaction(String topic, uint pos_s, uint pos_t, uint pos_i, String buf, System.Text.StringBuilder data);

        [DllImport("Win64Common.DLL", EntryPoint = "ClientInit")]
        public static extern void DdeLibClientInit();

        [DllImport("Win64Common.DLL", EntryPoint = "ClientUnInit")]
        public static extern void DdeLibClientUnInit();

        [DllImport("Win64Common.DLL", EntryPoint = "ConnectTry")]
        public static extern void DdeLibConnectTry();
	}
}
