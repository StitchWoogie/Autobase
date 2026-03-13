using System;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for ShareProtocol.
	/// </summary>
	public class ShareProtocol
	{
		public ShareProtocol()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public class ShareByMemory : ShareProtocol
	{
		public byte[] Read()
		{
			return null;		
		}

		public void Send(byte[] buf)
		{
			
		}
	}
}
