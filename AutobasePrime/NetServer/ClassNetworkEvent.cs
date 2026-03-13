using System;
using AutoLibLocal;

namespace NetServer
{
	/// <summary>
	/// Summary description for ClassNetworkEvent.
	/// </summary>
	public class ClassNetworkEvent
	{
		public ClassNetworkEvent()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static RingSharedMemory smNetworkToViewMain = new RingSharedMemory();
        static RingSharedMemory smViewMainToNetwork = new RingSharedMemory();

        public static void Init()
        {
            smNetworkToViewMain.Create("NetShareNetworkToViewMain", 10, 1000);
            smViewMainToNetwork.Create("NetShareViewMainToNetwork", 10, 1000);
        }

        public static void UnInit()
        {
            smNetworkToViewMain.Close();
            smViewMainToNetwork.Close();
        }

		public static void Check()
		{
			byte[] data = null;

			data = (byte[])smViewMainToNetwork.GetItem();

			if(data != null)
			{
				NetWorkProtocolRecv recv = new NetWorkProtocolRecv();
	
				recv.Split(data, data.Length);
				NetCommon.NetLib.SendCodeToAllExceptMe((EnumNetworkCommand)recv.wCommand, recv.wTransaction, data, data.Length);
			}
		}
	}
}
