using System;
using System.Threading;

namespace LocalMain
{
	/// <summary>
	/// Summary description for PlcScanEvent.
	/// </summary>
	public class PlcScanEvent
	{
		public PlcScanEvent()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//static ManualResetEvent manualEvent;
		//static Thread threadListener;
		
		public static void Init()
		{
			//manualEvent = new ManualResetEvent(false);

			//threadListener = new Thread(new ThreadStart(Listener));
			//threadListener.Start();

			PlcScan.Init(PlcScan.nMaxPorts);
		}

		public static void UnInit()
		{
			//bEndThread = true;
			//manualEvent.Set();
			//threadListener.Join(5000);	// 끝나기를 기다린다.
			//manualEvent.Close();

			PlcScan.UnInit();
		}

		//static bool bEndThread = false;


		/*
		static void Listener()
		{
			while(!bEndThread) 
			{
				Thread.Sleep(1);
				manualEvent.WaitOne();
				manualEvent.Reset();
				bEventByPlcScan = true;
			}
		}
		*/

		//static bool bEventByPlcScan = false;

		public static void Check()
		{
			/*
			if(bEventByPlcScan) 
			{
				bEventByPlcScan = false;
				PlcScan.UnInit();
				PlcScan.Init();
			}
			*/
			if(AutoLibLocal.SystemStatusMemory.GetDI(AutoLibLocal.SSMDI.PlcScanEventToLocalMain) == 1) 
			{
				AutoLibLocal.SystemStatusMemory.SetDI(AutoLibLocal.SSMDI.PlcScanEventToLocalMain, 0);
				PlcScan.UnInit();
				PlcScan.Init(PlcScan.nMaxPorts);
			}
		}
	}
}
