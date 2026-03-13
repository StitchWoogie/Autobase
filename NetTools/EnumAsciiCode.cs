using System;

namespace NetTools
{
	/// <summary>
	/// Summary description for AsciiCode.
	/// </summary>
	public class AsciiCode
	{
		public AsciiCode()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public enum EnumAsciiCode : byte
	{
		SOH = 0x01,
		STX = 0x02,		// start of text
		ETX = 0x03,		// end of text
		EOT = 0x04,   	// end of transmission
		ENQ = 0x05,   	// enquiry
		ACK = 0x06,   	// acknowledge
		LF = 0x0A,   	// line feed
		CR = 0x0D,		// carriage return
		DLE	= 0x10,		
		XON = 0x11,
		XOFF = 0x13,
		NAK	= 0x15,		// negative acknowledge
	}
}
