using System;

namespace ExportTest
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	/// 
	public class ClassExportGate2 : ExportLib.ClassExportLib2 
	{
		public override string ProtocolGetName()
		{
			return "Test";
		}

		public override void ProtocolInit(string option)
		{

		}

		public override void ProtocolUnInit()
		{
						
		}

		int test;

		public override void ProtocolRecvBytes(byte[] buf, int size)
		{
			int a;
			for(int i = 0; i < size; i++)
			{
				a = buf[i];
			}

			byte[] imsi = new byte[100];

			imsi[0] = (byte)(test/256);
			imsi[1] = (byte)(test%256);						

			SendBytes(imsi, 2);

			test++;
		}

		public override int ProtocolOption(ref string option)
		{
			return 0;
		}
	}
}
