using System;
using AutoLibLocal;
using AutoLib;
using System.Runtime.InteropServices;
using NetTools;
using GraphicModule;

namespace LocalMain
{
	/// <summary>
	/// Summary description for C_DdeTag.
	/// </summary>
	public class C_DdeTag
	{
        public delegate bool DelegateDdeLibLinkItemAdvise(String service, String topic, String item, out uint s, out uint t, out uint i);
        public delegate bool DelegateDdeLibRequestItem(uint service, uint topic, uint item, System.Text.StringBuilder data, int length);
        public delegate bool DelegateDdeLibChangedItem(uint service, uint topic, uint item, System.Text.StringBuilder data, int length);
        public delegate bool DelegateDdeLibTransaction(String topic, uint pos_s, uint pos_t, uint pos_i, String buf, System.Text.StringBuilder data);
        public delegate void DelegateDdeLibClientInit();
        public delegate void DelegateDdeLibClientUnInit();
        public delegate void DelegateDdeLibConnectTry();

        public static DelegateDdeLibLinkItemAdvise DdeLibLinkItemAdvise = null;
        public static DelegateDdeLibRequestItem DdeLibRequestItem = null;
        public static DelegateDdeLibChangedItem DdeLibChangedItem = null;
        public static DelegateDdeLibTransaction DdeLibTransaction = null;
        static DelegateDdeLibClientInit DdeLibClientInit = null;
        public static DelegateDdeLibClientUnInit DdeLibClientUnInit = null;
        public static DelegateDdeLibConnectTry DdeLibConnectTry = null;

        static C_DdeTag()
        {
            //
            // TODO: Add constructor logic here
            //
            if (IntPtr.Size == 8)
            {  // 64bit
                DdeLibLinkItemAdvise = C_DdeTag64.DdeLibLinkItemAdvise;
                DdeLibRequestItem = C_DdeTag64.DdeLibRequestItem;
                DdeLibChangedItem = C_DdeTag64.DdeLibChangedItem;
                DdeLibTransaction = C_DdeTag64.DdeLibTransaction;
                DdeLibClientInit = C_DdeTag64.DdeLibClientInit;
                DdeLibClientUnInit = C_DdeTag64.DdeLibClientUnInit;
                DdeLibConnectTry = C_DdeTag64.DdeLibConnectTry;
            }
            else
            {
                DdeLibLinkItemAdvise = C_DdeTag32.DdeLibLinkItemAdvise;
                DdeLibRequestItem = C_DdeTag32.DdeLibRequestItem;
                DdeLibChangedItem = C_DdeTag32.DdeLibChangedItem;
                DdeLibTransaction = C_DdeTag32.DdeLibTransaction;
                DdeLibClientInit = C_DdeTag32.DdeLibClientInit;
                DdeLibClientUnInit = C_DdeTag32.DdeLibClientUnInit;
                DdeLibConnectTry = C_DdeTag32.DdeLibConnectTry;
            }

            ScriptFunctionDde.procDdeTagLinkOne = new ScriptFunctionDde.DelegateDdeTagLinkOne(DdeTagLinkOne);
        }

		public static void DdeTagLinkAll()
		{
            if (SharedLocalMain.bTestMode == true) return;

			int i;
			int plc_scan_count = 0;
			TagPublicClass tp;

            if (TagLib.tagListAll == null) return;

			for(i = 0; i < TagLib.tagListAll.Length; i++) 
			{
				tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);
				
				DdeTagLinkOne(tp);

				if(tp.act == 1 && tp.cTagLinkType == 0)	plc_scan_count ++;
			}

			if(plc_scan_count > 0) 
			{	// plc_scan 을 사용하는 태그가 있으면 plc_scan 프로그램을 실행.

                FormLocalMain.RunPlcScan(true);

                /*
				string path;
				path = String.Format("{0}\\plc_scan.exe", System.Windows.Forms.Application.StartupPath);
				System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(path);
				info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

                try
                {
                    System.Diagnostics.Process.Start(info);
                }
                catch(Exception exception)
                {
                    if (Tools.IsLangKorean())
                    {
                        System.Windows.Forms.MessageBox.Show(exception.Message, "PLC_SCAN.exe 실행 오류");
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(exception.Message, "PLC_SCAN Execute Error");
                    }
                }*/
			}
		}

		public static void ClientInit()
		{
			DdeLibClientInit();
			//CallBack myCallBack = new CallBack(MyCallBackItemChanged);
			//DdeLibSetCallBack(myCallBack);
		}

		public static void ConnectTry()
		{
			DdeLibConnectTry();
		}

		public static void DdeTagLinkOne(TagPublicClass tp)
		{
            if (SharedLocalMain.bTestMode == true) return;

			if(tp.act == 0)				return;
			if(tp.cTagLinkType != 1)	return;
			if(String.Compare(tp.sDdeService, "AUTOBASE", true) == 0)	return;	// 자신을 연결할 수는 없다.
	
			string buf;
			buf = String.Format("Linking DDE Tag Service:{0}, Topic:{1}, Item:{2}",
				tp.sDdeService, tp.sDdeTopic, tp.sDdeItem);
			MessageDisplay.Show(buf);

			if(DdeLibLinkItemAdvise(tp.sDdeService, tp.sDdeTopic, tp.sDdeItem,
				out tp.dwDdeService, out tp.dwDdeTopic, out tp.dwDdeItem)) 
			{
				tp.bDdeLinkFlag = true;
			}
			else 
			{
				tp.bDdeLinkFlag = false;
			}
		}


	}
}

