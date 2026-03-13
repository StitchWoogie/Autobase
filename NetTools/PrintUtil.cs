using System;
using System.Drawing.Printing;

namespace NetTools
{
	/// <summary>
	/// Summary description for PrintUtil.
	/// </summary>
	public class PrintUtil
	{
		public PrintUtil()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static bool IsPrinterExist(string name)
		{
			foreach(String printer in PrinterSettings.InstalledPrinters) 
			{
				if(String.Compare(printer, name, true) == 0) 
				{
					return true;
				}
			}

			return false;
		}
	}
}
