using System;
using ExportLib;
using System.IO;

namespace ExportInfoVisionSI
{
	/// <summary>
	/// Summary description for ClassExportGate2.
	/// </summary>
	public class ClassExportGate2 : ExportLib.ClassExportLib2
	{
		byte[]		commSendBuf = new byte[5000];
		
		public override string ProtocolGetName()
		{
			return "InfoVision SI";
		}

		void getInitOptionData(string option)
		{
			if(option.Length <= 0) return;

			CommaBlockString comma = new CommaBlockString();
			//byte			i = 0;

			comma.Set(option);				
			/*
			comma.GetBYTE(ref cStation);
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref i);
			bFloatData = (i == 1) ? true : false;
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref i);
			bReadValueFloat = (i == 1) ? true : false;			
			*/
		}
		
		public override void ProtocolInit(string option)
		{			
			getInitOptionData(option);			
		}

		public override void ProtocolUnInit()
		{
			
		}

		void ExecuteFunctionRead(byte[] commRecvBuf, string tag_type, int tag_pos, int tag_count)
		{
			int count;

			commSendBuf[0] = 0x02;
			commSendBuf[5] = commRecvBuf[5];
			commSendBuf[6] = commRecvBuf[6];
			commSendBuf[7] = commRecvBuf[7];
			commSendBuf[8] = commRecvBuf[8];
			commSendBuf[9] = commRecvBuf[9];
			commSendBuf[10] = commRecvBuf[10];
			commSendBuf[11] = commRecvBuf[11];
			commSendBuf[12] = commRecvBuf[12];
			commSendBuf[13] = commRecvBuf[13];
			commSendBuf[14] = commRecvBuf[14];
			commSendBuf[15] = commRecvBuf[15];
			commSendBuf[16] = commRecvBuf[16];
			count = 17;

			if(tag_type == "AI" || tag_type == "AO") 
			{
				MemoryStream stream = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(stream);

				for(int i = 0; i < tag_count; i++) 
				{
					writer.Write((float)GetTagValueDouble(tag_pos+i));
				}

				byte[] buf = stream.ToArray();

				writer.Close();
				
				for(int i = 0; i < buf.Length; i++) 
				{
					commSendBuf[count++] = buf[i];
				}
			}
			else if(tag_type == "DI" || tag_type == "DO") 
			{
				for(int i = 0; i < tag_count; i++) 
				{
					commSendBuf[count++] = (GetTagValueDouble(tag_pos+i) == 0) ? (byte)'0' : (byte)'1';
				}
			}
			else 
			{
				return;
			}

			commSendBuf[count++] = 0x03;

			string imsi = String.Format("{0:0000}", count+2);
				
			for(int i = 0; i < 4; i++) 
			{
				commSendBuf[i+1] = (byte)imsi[i];
			}

			byte crc = NetTools.GetCRC.SumBYTE(commSendBuf, 0, count);

			imsi = String.Format("{0:X02}", crc);
				
			commSendBuf[count++] = (byte)(imsi[0]);
			commSendBuf[count++] = (byte)(imsi[1]);
			
			SendBytes(commSendBuf, count);
		}

		void ExecuteFunctionWrite(byte[] commRecvBuf, bool ok)
		{
			int count;
			int i;

			for(i = 0; i < 41; i++) 
			{
				commSendBuf[i] = commRecvBuf[i];
			}

			count = 41;

			commSendBuf[count++] = ok ? (byte)'0' : (byte)'1';

			commSendBuf[count++] = 0x03;

			string imsi = String.Format("{0:0000}", count+2);
				
			for(i = 0; i < 4; i++) 
			{
				commSendBuf[i+1] = (byte)imsi[i];
			}

			byte crc = NetTools.GetCRC.SumBYTE(commSendBuf, 0, count);

			imsi = String.Format("{0:X02}", crc);
				
			commSendBuf[count++] = (byte)(imsi[0]);
			commSendBuf[count++] = (byte)(imsi[1]);
			
			SendBytes(commSendBuf, count);
		}

		float BytesToFloat(byte[] val, int pos)
		{
			MemoryStream stream = new MemoryStream(val, pos, 4);
			BinaryReader reader = new BinaryReader(stream);
			float fVal;

			try 
			{
				fVal = reader.ReadSingle();
			}
			catch 
			{
				fVal = 0;
			}
			reader.Close();			

			return fVal;
		}

		

		int GetInt(byte[] buf, int pos, int size)
		{
			string imsi = GetString(buf, pos, size);

			return ConvertTool.ToInt32(imsi);
		}

		string GetString(byte[] buf, int pos, int size)
		{
			string imsi = "";
			for(int i = 0; i < size; i++, pos++) 
			{
				imsi += (char)(buf[pos]);
			}
			return imsi;
		}

		public override void ProtocolRecvBytes(byte[] buf, int size)
		{			
			// 이 프로토콜은 TCP/IP 통신을 할 때만 가정하여 만들었다. 232로 사용하려면 하나씩 받는 부분이 필요하다.
			if(size == 20)	// Data Read
			{
				if(buf[0] != 0x02)				return;	
				if(buf[size-3] != 0x03)			return;
				if(GetInt(buf, 1, 4) != size)	return;

				byte crc_result = NetTools.GetCRC.SumBYTE(buf, 0, size-2);
				byte crc = NetTools.HexBuf.ToByte(buf, size-2);

				if(crc != crc_result)	return;

				string tag_type = GetString(buf, 7, 2);
				int tag_pos = GetInt(buf, 9, 4);
				int read_count = GetInt(buf, 13, 4);

				ExecuteFunctionRead(buf, tag_type, tag_pos, read_count);
			}
			else if(size == 45 || size == 48)		// Data Write (45 == DO, 48 = AO)
			{
				if(buf[0] != 0x02)		return;	
				if(buf[size-3] != 0x03)	return;
				if(GetInt(buf, 1, 4) != size)	return;
				
				byte crc_result = NetTools.GetCRC.SumBYTE(buf, 0, size-2);
				byte crc = NetTools.HexBuf.ToByte(buf, size-2);

				if(crc != crc_result)	return;

				string tag_type = GetString(buf, 7, 2);
				string tag_name = GetString(buf, 9, 32).Trim();

				object obj;
				if(tag_type == "AO") 
				{
					obj = (double)BytesToFloat(buf, 41);
				}
				else if(tag_type == "DO") 
				{
					obj = (buf[41] == '1') ? 1 : 0;
				}
				else 
				{
					ExecuteFunctionWrite(buf, false);
					return;
				}

				ExecuteFunctionWrite(buf, this.SetTagValueByName(tag_name, obj));
			}
		}

		public override int ProtocolOption(ref string option)
		{
			/*
			FormProtocolOption form = new FormProtocolOption();

			getInitOptionData(option);			// 옵션 데이터를 다시 읽는다, 실제 통신에는 영향이 없다.

			//form.numericUpDownStation.Value = cStation;
			//form.checkBoxReadFloat.Checked = bFloatData;
			//form.checkBoxWriteFloat.Checked = bReadValueFloat;

			if(form.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
			{

				return 1;
			}
			*/

			return 0;
		}
	}
}
