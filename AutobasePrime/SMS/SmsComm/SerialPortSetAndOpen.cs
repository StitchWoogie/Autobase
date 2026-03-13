using System;
using System.IO;
using System.Windows.Forms;
using SMS.SmsFunc;
using NetTools;
using System.IO.Ports;
using DialogCommon;

namespace SMS.SmsComm
{
	/// <summary>
	/// Summary description for SerilaPortSetAndOpen.
	/// </summary>
	public class SerialPortSetAndOpen
	{
		public static SerialPort port;
		public static bool		 bOpen;

        public SerialPortSetAndOpen()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        static public void setSerialPort()
		{
			port.BaudRate = SmsBasic.smsConfig.nBaud;
			port.Parity = (Parity)SmsBasic.smsConfig.nParity;
			port.DataBits = SmsBasic.smsConfig.nData;
			port.StopBits = (StopBits)SmsBasic.smsConfig.nStop;			
			port.PortName = "COM"+SmsBasic.smsConfig.nPort.ToString();

			port.RtsEnable = (SmsBasic.smsConfig.nRts == 1);
			port.DtrEnable = (SmsBasic.smsConfig.nDtr == 1);
		}

		static void openErrorMessageDisplay()
		{
			string		message = "";
			if(Tools.IsLangKorean()) 
			{
				message = string.Format("시리얼 포트({0})를 열 수 없습니다.", "COM"+SmsBasic.smsConfig.nPort.ToString());
				MessageBox.Show(message, "통신포트 열기 오류");
			}
			else 
			{
				message = string.Format("Serial Port Open Error. ({0})", "COM"+SmsBasic.smsConfig.nPort.ToString());
				MessageBox.Show(message, "Comm Port Open Error");
			}
		}

		static public void open()
		{
			if(SmsBasic.smsConfig.eConnectType != SendSMSData.eConectionType.SERIAL) return;
			port = new SerialPort();
			try 
			{
				setSerialPort();
				port.Open();
				if(port.IsOpen == false) openErrorMessageDisplay();
				else bOpen = true;
			}
			catch 
			{
				openErrorMessageDisplay();
				bOpen = false;
			}			
		}

		static public void close()
		{
			if(port == null || bOpen == false) return;
			if(port.IsOpen == false) return;
			try 
			{
				port.Close();
				TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();
				timeout.Reset();
				while(true)					// 다시 port를 open 하기 위해 일정한 시간동안 쉰다 ( 200 msec 정도 )
				{
					if(timeout.IsTimeOut(200)) break;					
				}
				bOpen = false;
			}
			catch {}
		}

		static public bool read(ref string data)
		{
			if(port.IsOpen == false) return false;

			byte[] buf = new byte[1];

			string		imsi = "";
			imsi = port.ReadExisting();
			if(imsi.Length <= 0) return false;
			data += imsi;
			SendSMSData.codeViewPush.DisplayRecvCode(imsi, imsi.Length);
			return true;

			//if(port.Read(buf, 0, 1) != 1) return -1;
			//return (int)buf[0];
		}

		static public void write(string data)
		{
			if(port.IsOpen == false) return;

			byte[]		buf = Tools.StringToBytes(data);
			
			int		len = buf.Length;

			// 호출하는 함수에서 제한할 것 (2006.5.18 제거)
			//if(len > SmsBasic.smsConfig.nMaxSendChar) len = SmsBasic.smsConfig.nMaxSendChar;

			port.Write(buf, 0, len);
			SendSMSData.codeViewPush.DisplaySendCode(buf, len);
			SendSMSData.codeViewPush.DisplaySendNextLine();
		}

		static public void writeByte(byte[] buf, int len)
		{
			if(port.IsOpen == false) return;

			port.Write(buf, 0, len);
			SendSMSData.codeViewPush.DisplaySendCode(buf, len);
			SendSMSData.codeViewPush.DisplaySendNextLine();
		}
	}
}
