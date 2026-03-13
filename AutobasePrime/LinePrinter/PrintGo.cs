using System;
using NetTools;
using System.IO.Ports;
using System.Runtime.InteropServices;
using AutoLibLocal;

namespace LinePrinter
{
	/// <summary>
	/// Summary description for PrintGo.
	/// </summary>
	public class PrintGo
	{
		public PrintGo()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static int PrintOneLineString(string String, string command)
        {
            int port_no;
            if (Config.port_type == 0)	// Printer
            {
                port_no = Config.lpt_no;
            }
            else
            {
                port_no = Config.com_no;
            }

            string imsiBuffer = "";

            if (command.Length != 0 && Config.cEscFlag)
            {
                imsiBuffer = command;
                imsiBuffer += String;
            }
            else if (String.Length != 0) imsiBuffer = String;
            else imsiBuffer = "      ";

            imsiBuffer += "\n\r";


            if (Config.port_type == 2)
            {
                byte[] imsibytes = Tools.StringToBytes(imsiBuffer);

                IntPtr pUnmanagedBytes = new IntPtr(0);

                pUnmanagedBytes = Marshal.AllocCoTaskMem(imsibytes.Length);
                // Copy the managed byte array into the unmanaged array.
                Marshal.Copy(imsibytes, 0, pUnmanagedBytes, imsibytes.Length);
                // Send the unmanaged bytes to the printer.
                try
                {
                    bool bSuccess = RawPrinterHelper.SendBytesToPrinter(Config.sPrinterName, pUnmanagedBytes, imsibytes.Length);
                }
                catch
                {

                }
                // Free the unmanaged memory that you allocated earlier.
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
            }
            else
            {
                DirectOutput.OpenWriteClose((sbyte)Config.port_type, port_no, Config.baud, (sbyte)Config.parity, (sbyte)Config.data, (sbyte)Config.stop, imsiBuffer, 0);
            }

            return 1;
        }
        

        /*
		public static int PrintOneLineString(string String, string command)
		{
			int port_no;
			if(Config.port_type == 0)	// Printer
			{
				port_no = Config.lpt_no;
			}
			else 
			{	
				port_no = Config.com_no;
			}

			string imsiBuffer = "";

			if(command.Length != 0 && Config.cEscFlag) 
			{
				imsiBuffer = command;
				imsiBuffer += String;
			}
			else if(String.Length != 0) imsiBuffer = String;	
			else						imsiBuffer = "      ";

			imsiBuffer += "\n\r";

			//byte[] buf = Tools.StringToBytes(imsiBuffer);

			DirectOutput.OpenWriteClose((sbyte)Config.port_type, port_no, Config.baud, (sbyte)Config.parity, (sbyte)Config.data, (sbyte)Config.stop, imsiBuffer, 0);

			return 1;
		}*/

		/*
		public static int PrintOneLineString(string String, string command)
		{
			SerialPort serial = new SerialPort();

			if(Config.port_type == 0)	// Printer
			{
				serial.PortName = "LPT"+Config.lpt_no.ToString();
				serial.BaudRate = Config.baud;
				serial.Parity = (Parity)Config.parity;
				serial.DataBits = Config.data;
				serial.StopBits = (StopBits)Config.stop;
			}
			else 
			{	
				serial.PortName = "COM"+Config.com_no.ToString();
				serial.BaudRate = Config.baud;
				serial.Parity = (Parity)Config.parity;
				serial.DataBits = Config.data;
				serial.StopBits = (StopBits)Config.stop;
			}

			serial.Open();

			string imsiBuffer = "";

			if(command.Length != 0 && Config.cEscFlag) 
			{
				imsiBuffer = command;
				imsiBuffer += String;
			}
			else if(String.Length != 0) imsiBuffer = String;	
			else						imsiBuffer = "      ";

			imsiBuffer += "\n\r";

			byte[] buf = Tools.StringToBytes(imsiBuffer);

			serial.Write(buf, 0, buf.Length);

			serial.Close();

			return 1;
		}
		*/
	}
}

