using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Opc;
using Opc.Da;
using OpcCom.Da;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using Opc.Cpx;
using OpcClient.plcScanComm;
using NetTools;

namespace OpcClient
{
	/// <summary>
	/// Summary description for opcReadWriteGroupLoopClass.
	/// </summary>
	public class opcReadWriteGroupLoopClass
	{


		public opcReadWriteGroupLoopClass()
		{
			//
			// TODO: Add constructor logic here
			//			
		}

		static public bool opcServerConnection(opcServerReadWriteClass opcServer)
		{
			if(opcServer == null) return false;
			if(opcServer.m_server == null) 
			{
				opcServer.tryConnectTime = DateTime.Now;
                string hostName = (opcServer.hostName.Length > 0) ? opcServer.hostName + "." : "";// 2009-01-22 add

				try 
				{
					URL url = new Opc.URL(opcServer.serverName);
                    if (opcServer.hostName.Length > 0) url.HostName = opcServer.hostName;
					url.Scheme = Opc.UrlScheme.DA;
					OpcCom.Factory factory = new OpcCom.Factory();
					opcServer.m_server = new Opc.Da.Server(factory, url);
					if(opcServer.m_server == null) return false;
					opcConnection.OnConnectOpc(opcServer.m_server);
					opcServer.bTryConnect = true;
					opcGroupReadWriteClass opcGroup;
					for(int i = 0; i < opcServer.arrGroup.Count; i++) 
					{
						opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];
						opcGroup.bRegister = false;			// 등록하지 않았다.
					}
					
					if(opcGroupItemReadRegisterForm.formThis != null) 
					{
						opcGroupItemReadRegisterForm.formThis.displayServerStatusList();                        
						if(NetTools.Tools.IsLangKorean())
                            opcGroupItemReadRegisterForm.formThis.displayEventOrErrorMessage("서버에 연결 : " + hostName + opcServer.serverName, opcServer.tryConnectTime);        //+ hostName 2009-01-22
						else
                            opcGroupItemReadRegisterForm.formThis.displayEventOrErrorMessage("Connected To Server : " + hostName + opcServer.serverName, opcServer.tryConnectTime);//+ hostName 2009-01-22
					}
					return true;
				}				
				catch
				{
					opcServer.bTryConnect = true;
					opcServer.m_server = null;
					if(opcGroupItemReadRegisterForm.formThis != null) 
					{
						opcGroupItemReadRegisterForm.formThis.displayServerStatusList();
						if(NetTools.Tools.IsLangKorean())
                            opcGroupItemReadRegisterForm.formThis.displayEventOrErrorMessage("서버에 연결실패 : " + hostName + opcServer.serverName, opcServer.tryConnectTime);//+ hostName 2009-01-22
						else
                            opcGroupItemReadRegisterForm.formThis.displayEventOrErrorMessage("Connection Failure : " + hostName + opcServer.serverName, opcServer.tryConnectTime);//+ hostName 2009-01-22
					}
				}
			}			
			return false;
		}
		
        static public bool opcServerDisConnection(opcServerReadWriteClass opcServer, bool bByUser)// add 2009-10-28, bool bByUser
        {
            try
            {
                opcServer.m_server.Disconnect();
            }
            catch { }
            opcServer.m_server = null;
            opcServer.tryConnectTime = DateTime.Now;	// 종료한 시간부터 계산한다.
            opcGroupItemReadRegisterForm.formThis.displayServerStatusList();

            string hostName = (opcServer.hostName.Length > 0) ? opcServer.hostName + "." : "";// 2009-01-22 add
            if (bByUser)
            {
                if (NetTools.Tools.IsLangKorean())
                    opcGroupItemReadRegisterForm.formThis.displayEventOrErrorMessage("사용자에 의한 연결종료 : " + hostName + opcServer.serverName, opcServer.tryConnectTime);//+ hostName 2009-01-22
                else
                    opcGroupItemReadRegisterForm.formThis.displayEventOrErrorMessage("DisConnected by User : " + hostName + opcServer.serverName, opcServer.tryConnectTime);//+ hostName 2009-01-22
            }
            else// add 2009-10-28
            {
                if (NetTools.Tools.IsLangKorean())
                    opcGroupItemReadRegisterForm.formThis.displayEventOrErrorMessage("서버의 종료에 따른 연결종료 : " + hostName + opcServer.serverName, opcServer.tryConnectTime);
                else
                    opcGroupItemReadRegisterForm.formThis.displayEventOrErrorMessage("DisConnected by Opc Server : " + hostName + opcServer.serverName, opcServer.tryConnectTime);
            }
            return true;
        }

		static void checkRetryConnectionTime(opcServerReadWriteClass opcServer)
		{
			if(opcBasic.opcClientConfig.bRetryConnect == false) return;		// 사용하지 않으면 return
			if(opcServer.m_server != null) return;							//연결되었으면
			if(opcServer.bTryConnect == false) return;						// 바로 연결시도 할 것이다.

			DateTime curr = DateTime.Now;

			if(TimeUtil.GetMinHap(curr)-TimeUtil.GetMinHap(opcServer.tryConnectTime) >= opcBasic.opcClientConfig.nRetryMin)
				opcServer.bTryConnect = false;
		}
		

		static public	void readGroup(int nArrPos)
		{
			opcServerReadWriteClass opcServer;

			opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nArrPos];
			if(opcServer.bTryConnect) checkRetryConnectionTime(opcServer);				
			if(opcServer.bTryConnect == false) opcServerConnection(opcServer);	// 연결을 시도한다
			if(opcServer.m_server == null) return;
			try 
			{
				if(opcServer.m_server.IsConnected)
				{
					readGroupItemData(opcServer);
				}
			}
			catch
			{
				//opcServerDisConnection(opcServer, false);// deleted 2009-10-28
                checkOpcServerStatusAndSetItemStatus();// add 2009-10-28
			}
		}

        static public void checkOpcServerStatusAndSetItemStatus()// add 2009-10-28 서버가 강제로 종료되었을 경우 연결 종료를 표시하기 위해, try , catch에 걸림
        {
            opcServerReadWriteClass opcServer;
            for (int i = 0; i < opcBasic.arrOpcServer.Count; i++)
            {
                opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];
                if (opcServer.m_server == null) continue;
                try
                {
                    if (opcServer.m_server.IsConnected)
                    {
                        opcServer.m_server.GetStatus();// add 2009-10-28 서버가 강제로 종료되었을 경우 연결 종료를 표시하기 위해, try , catch에 걸림
                    }
                }
                catch
                {
                    try
                    {
                        opcGroupReadWriteClass opcGroup;
                        opcItemReadWriteClass opcItem;
                        int j, k;
                        DateTime dt = DateTime.Now;

                        for (j = 0; j < opcServer.arrGroup.Count; j++)
                        {
                            opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];

                            for (k = 0; k < opcGroup.arrItem.Count; k++)
                            {
                                opcItem = (opcItemReadWriteClass)opcGroup.arrItem[k];
                                opcItem.bNewRead = true;
                                opcItem.reuslt = Opc.ResultID.E_FAIL;
                                opcItem.timeStamp = dt;
                                opcItem.quality = Opc.Da.Quality.Bad;
                                if (comShareClass.bShareDllExist) comShareClass.comOpcShareDataChanged(opcServer.accessName, opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);
                                opcItem.bFlag = false;
                            }
                        }
                    }
                    catch { }
                    opcServerDisConnection(opcServer, false);
                }
            }
        }
		
		
		static void readGroupItemData(opcServerReadWriteClass opcServer)
		{
			opcGroupReadWriteClass opcGroup;
			
			for(int i = 0; i < opcServer.arrGroup.Count;i++) 
			{
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];				
				if(opcGroup.bAsync) 
				{
					if(opcGroup.bRegister == true) continue;		// 이미 등록되어 있다
					opcBasic.registerOneGroupAsync(opcServer, opcGroup);
				}
				else readSyncGroupItemData(opcServer, opcGroup);
			}
		}
		
		
				
		static void readSyncGroupItemData(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup)
		{
			DateTime	curr = DateTime.Now;
			uint		milli_curr = basicTool.GetMinSecMilliHap(curr.Minute, curr.Second, curr.Millisecond);

			if(opcGroup.bActive == false || opcGroup.arrItem.Count <= 0) return;
			if(milli_curr < opcGroup.milli ||
				milli_curr >= opcGroup.milli + opcGroup.uPeriod) 
			{
				opcGroup.milli = milli_curr;
				readItemData(opcServer, opcGroup);
			}
		}

		
		static void readItemData(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup)
		{
			int						i;
			opcItemReadWriteClass	opcItem;			
			ItemValueResult[] results  = null;

			results = opcServer.m_server.Read(opcBasic.oneSyncGroupDataToArray(opcGroup));
            if (results == null) return;
			
			for(i = 0; i < opcGroup.arrItem.Count; i++) 
			{
				if(i >= results.Length) return;							// 읽은 개수가 아이템 수를 초과 2005-1-13 추가
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				opcItem.bNewRead = true;
				opcItem.reuslt = results[i].ResultID;
                opcItem.timeStamp = results[i].Timestamp;               // 2008-10-30 위쪽으로 이동, 밑에는 삭제
				if(results[i].Value == null) 
				{
                    if (opcItem.quality != results[i].Quality) // 2008-10-30 추가, quality 가 바뀌었으면
                    {
                        opcItem.quality = results[i].Quality;
                        if (comShareClass.bShareDllExist) comShareClass.comOpcShareDataChanged((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);
                    }
					opcItem.bFlag = false;
					continue;
				}
				//opcItem.timeStamp = results[i].Timestamp;
				if(opcItem.readData != (object)results[i].Value || opcItem.quality != results[i].Quality) // 값이 바뀌었으면
				{
					opcItem.readData = (object)results[i].Value;
                    opcItem.quality = results[i].Quality;
                    if (comShareClass.bShareDllExist) comShareClass.comOpcShareDataChanged((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);
				}
				opcItem.bFlag = true;
			}
		}

		static public ItemValueResult[] readOneItemData(opcServerReadWriteClass opcServer, string itemName, bool bDevice)
		{
            if (opcServer == null) return null;

			ArrayList				arrItems;
			Opc.Da.Item				oneItem;
			Opc.Da.Item[]			items;
            
			arrItems = new ArrayList();
			oneItem = new Item();
            if (bDevice == false)// 2008-10-30 add, cache read(default)
            {
                oneItem.MaxAgeSpecified = true;        
                oneItem.MaxAge = -1;
            }
            else// 2008-10-30 add,  device read
            {
                oneItem.MaxAgeSpecified = false;       
                oneItem.MaxAge = 0;
            }
			oneItem.ItemName = itemName;
			arrItems.Add(oneItem);
				
			items = (Item[])arrItems.ToArray(typeof(Item));
            try
            {
                return opcServer.m_server.Read(items);
            }
            catch
            {
                return null;
            }

		}


		/*static private void OnDataChange(object subscriptionHandle, ItemValueResult[] values)
		{
			opcServerReadWriteClass	opcServer;
			opcGroupReadWriteClass	opcGroup;
			int						i;
			
			for(i = 0; i < opcBasic.arrOpcServer.Count;i++) 
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];
				for(int j = 0; j < opcServer.arrGroup.Count; j++) 
				{
					opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];
					//if(opcGroup.bAsync == false) continue;
					if(opcGroup.sClientHandle == (string)subscriptionHandle) 
					{
						goto read_proc;
					}
				}			
			}
			return;
			read_proc:

			opcGroup.bNewRead = true;
			for(i = 0; i < values.Length; i++) checkAndSetReadValue(opcGroup, values[i]);
		}

		static void checkAndSetReadValue(opcGroupReadWriteClass	opcGroup, ItemValueResult val)
		{
			opcItemReadWriteClass	opcItem;
			for(int i = 0; i < opcGroup.arrItem.Count; i++) 
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				if(val.ItemName != opcItem.itemName) continue;
				if(val == null) 
				{
					opcItem.bFlag = false;
					continue;
				}
				opcItem.timeStamp = val.Timestamp;
				opcItem.readData = (object)val.Value;
				opcItem.bFlag = true;
			}
		}*/

		static public Opc.IdentifiedResult[] writeOneItem(opcServerReadWriteClass opcServer, string itemName, object val)
		{
			if(opcServer == null) return null;

			ArrayList				arrItems;
			Opc.Da.ItemValue		oneItem = new ItemValue();
			Opc.Da.ItemValue[]		items;			
			
			arrItems = new ArrayList();
			oneItem.ItemName = itemName;
			oneItem.Value = val;
			arrItems.Add(oneItem);
			
			items = (Opc.Da.ItemValue[])arrItems.ToArray(typeof(ItemValue));
			return opcServer.m_server.Write(items);
		}






	}
}
