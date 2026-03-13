using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using Opc;
using Opc.Da;
using Opc.Cpx;
using OpcClient.plcScanComm;
using NetTools;


namespace OpcClient
{
	/// <summary>
	/// Summary description for opcBasicReadWriteClass.
	/// </summary>
	public class opcBasic
	{
		static public	ArrayList arrOpcServer = new ArrayList();		
		static public opcClientConfigClass opcClientConfig = new opcClientConfigClass();

		public opcBasic()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/*static public int getSelectedTreeNodeDepth(string path)
		{
			if(path == null || path.Length <= 0) return 0;

			CommaBlockString	comma = new CommaBlockString();
			string				buf = "";
			int					i = 0;

			comma.Set(path);
			comma.SetBlockCode('\\');
			while(true) 
			{
				comma.GetString(ref buf);
				i++;
				if(comma.IsEOS()) return i;
				if(i >= 100) return 0;				
			}			
		}*/

		static public string getTreeViewServerName(opcServerReadWriteClass opcServer)
		{
			if(opcServer == null) return "";

            opcServer.hostName = opcServer.hostName.Trim();
            if (opcServer.hostName.Length > 0)// 2009-01-22 add
            {
                if (opcServer.accessName.Length <= 0) return opcServer.hostName + "." + opcServer.serverName;
                else return opcServer.hostName + "." + opcServer.serverName + "  =" + opcServer.accessName;
            }
            else
            {
                if (opcServer.accessName.Length <= 0) return opcServer.serverName;
                else return opcServer.serverName + "  =" + opcServer.accessName;
            }
		}

		static public string getTreeViewItemName(opcItemReadWriteClass opcItem)
		{
			if(opcItem == null) return "";

			if(opcItem.accessName.Length <= 0) return opcItem.itemName;
			else							   return opcItem.itemName + "  =" + opcItem.accessName;
		}

		/*static public bool getTreePathFromServerNamePos(ref int pos, string path)
		{
			CommaBlockString	comma = new CommaBlockString();
			string				buf = "";
			int					i;

			comma.Set(path);
			comma.SetBlockCode('\\');
			comma.GetString(ref buf);

			opcServerReadWriteClass opcServer;
			for(i = 0; i < opcBasic.arrOpcServer.Count; i++) 			
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];				
				if(getTreeViewServerName(opcServer) == buf) 
				{
					pos = i;
					return true;
				}				
			}			
			return false;
		}*/

		static public bool getServerStringFromServerNamePos(ref int pos, string serverName)
		{
			int					i;
			
			opcServerReadWriteClass opcServer;
			for(i = 0; i < opcBasic.arrOpcServer.Count; i++) 			
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];
				if(opcServer.serverName == serverName || opcServer.accessName == serverName)
				{
					pos = i;
					return true;
				}				
			}			
			return false;
		}
		

		/*static public bool getTreePathFromGroupNamePos(int nServer, ref int nGroup, string path)
		{
			CommaBlockString	comma = new CommaBlockString();
			string				buf = "";
			int					i;

			comma.Set(path);
			comma.SetBlockCode('\\');
			comma.GetString(ref buf);
			if(comma.IsEOS()) return false;
			comma.GetString(ref buf);

			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			opcGroupReadWriteClass opcGroup;
			for(i = 0; i < opcServer.arrGroup.Count; i++) 			
			{
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];
				if(opcGroup.groupName == buf) 
				{
					nGroup = i;
					return true;
				}
			}
			return false;
		}*/

		static public bool getGroupStringFromGroupNamePos(int nServer, ref int nGroup, string groupName)
		{
			int					i;
		
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			opcGroupReadWriteClass opcGroup;
			for(i = 0; i < opcServer.arrGroup.Count; i++) 			
			{
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];
				if(opcGroup.groupName == groupName) 
				{
					nGroup = i;
					return true;
				}
			}
			return false;
		}

		static public bool getTotalItemPosFromGroupPos(int nServer, ref int nGroup, ref int nItem, int nItemPos)
		{
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			
			
			for(int hap = 0, i = 0; i < opcServer.arrGroup.Count; i++) 
			{
				opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];
				if(opcGroup.arrItem.Count <= 0) continue;
				if(nItemPos >= hap && nItemPos < hap+opcGroup.arrItem.Count) 
				{
					nGroup = i;
					nItem = nItemPos-hap;
					return true;
				}
				hap += opcGroup.arrItem.Count;
			}
			return false;
		}
		

		/*static public bool getTreePathFromItemNamePos(int nServer, int nGroup, ref int nItem, string path)
		{
			CommaBlockString	comma = new CommaBlockString();
			string				buf = "";
			int					i;

			comma.Set(path);
			comma.SetBlockCode('\\');
			comma.GetString(ref buf);
			if(comma.IsEOS()) return false;
			comma.GetString(ref buf);
			if(comma.IsEOS()) return false;
			comma.GetString(ref buf);

			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			opcItemReadWriteClass opcItem;
			for(i = 0; i < opcGroup.arrItem.Count; i++) 			
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				if(getTreeViewItemName(opcItem) == buf) 
				{
					nItem = i;
					return true;
				}
			}
			return false;
		}*/

		static public bool getItemStringFromItemNamePos(int nServer, int nGroup, ref int nItem, string itemName)
		{
			int					i;

			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			opcItemReadWriteClass opcItem;
			for(i = 0; i < opcGroup.arrItem.Count; i++) 			
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				if(opcItem.itemName == itemName || opcItem.accessName == itemName) 
				{
					nItem = i;
					return true;
				}
			}
			return false;
		}		


		static public void objectItemCount(ref object val, ref int pos)
		{
			if(isValueObjectIsSingleType(val)) 
			{
				pos ++;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_STRING) 
			{
				string[] stringVal = (string[])val;
				pos += stringVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_DATETIME) 
			{
				DateTime[] dateTimeVal = (DateTime[])val;
				pos += dateTimeVal.Length;
				return;
			}				
			if(val.GetType() == Opc.Type.ARRAY_INT) 
			{
				int[] iVal = (int[])val;
				pos += iVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_SBINARY) 
			{
				sbyte[] sbyteVal = (sbyte[])val;
				pos += sbyteVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_FLOAT) 
			{
				float[] floatVal = (float[])val;
				pos += floatVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_DOUBLE) 
			{
				double[] doubleVal = (double[])val;
				pos += doubleVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_BINARY) 
			{
				byte[] byteVal = (byte[])val;
				pos += byteVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_SHORT) 
			{
				short[] shortVal = (short[])val;
				pos += shortVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_USHORT) 
			{
				ushort[] ushortVal = (ushort[])val;
				pos += ushortVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_UINT) 
			{
				uint[] uintVal = (uint[])val;
				pos += uintVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_LONG) 
			{
				long[] longVal = (long[])val;
				pos += longVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_ULONG) 
			{
				ulong[] ulongVal = (ulong[])val;
				pos += ulongVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_DECIMAL) 
			{
				decimal[] decimalVal = (decimal[])val;
				pos += decimalVal.Length;
				return;
			}
			if(val.GetType() == Opc.Type.ARRAY_BOOLEAN) 
			{
				bool[] boolVal = (bool[])val;
				pos += boolVal.Length;
				return;
			}
			//if(val.GetType() == Opc.Type.ARRAY_ANY_TYPE) 
			//{
			//	objectItemCount(ref val, ref pos);
			//}
			else return;			// pos error
		}


		static public bool isValueObjectSupportedType(object val)
		{
			if(isValueObjectIsSingleType(val)) return true;
			if(isValueObjectIsArrayType(val)) return true;			
			return false;
		}

		static public bool isValueObjectIsArrayType(object val)
		{
			if(val.GetType() == Opc.Type.ARRAY_INT) return true;
			if(val.GetType() == Opc.Type.ARRAY_DOUBLE) return true;
			if(val.GetType() == Opc.Type.ARRAY_BINARY) return true;//new
			if(val.GetType() == Opc.Type.ARRAY_SBINARY) return true;//new
			if(val.GetType() == Opc.Type.ARRAY_FLOAT) return true;
			if(val.GetType() == Opc.Type.ARRAY_STRING) return true;
			if(val.GetType() == Opc.Type.ARRAY_SHORT) return true;//new
			if(val.GetType() == Opc.Type.ARRAY_USHORT) return true;//new
			if(val.GetType() == Opc.Type.ARRAY_UINT) return true;//new
			if(val.GetType() == Opc.Type.ARRAY_LONG) return true;//new   10개
			if(val.GetType() == Opc.Type.ARRAY_ULONG) return true;//new
			if(val.GetType() == Opc.Type.ARRAY_DECIMAL) return true;//new
			if(val.GetType() == Opc.Type.ARRAY_BOOLEAN) return true;//new
			if(val.GetType() == Opc.Type.ARRAY_DATETIME) return true;//new*/
			if(val.GetType() == Opc.Type.ARRAY_ANY_TYPE) return true;//new*/ 15개
			
			return false;
		}

		static public bool isValueObjectIsSingleType(object val)
		{
			if(val.GetType() == Opc.Type.INT) return true;
			if(val.GetType() == Opc.Type.SBYTE) return true;
			if(val.GetType() == Opc.Type.FLOAT) return true;
			if(val.GetType() == Opc.Type.DOUBLE) return true;
			if(val.GetType() == Opc.Type.STRING) return true;
			if(val.GetType() == Opc.Type.BYTE) return true;//new
			if(val.GetType() == Opc.Type.SHORT) return true;//new
			if(val.GetType() == Opc.Type.USHORT) return true;//new
			if(val.GetType() == Opc.Type.UINT) return true;//new
			if(val.GetType() == Opc.Type.LONG) return true;//new    10개
			if(val.GetType() == Opc.Type.ULONG) return true;//new
			if(val.GetType() == Opc.Type.DECIMAL) return true;//new
			if(val.GetType() == Opc.Type.BOOLEAN) return true;//new
			if(val.GetType() == Opc.Type.DATETIME) return true;//new
			if(val.GetType() == Opc.Type.DURATION) return true;//new
			if(val.GetType() == Opc.Type.ANY_TYPE) return true;//new*/ 16개
			return false;
		}

		static public Opc.Da.Subscription getRegisterSubscription(opcServerReadWriteClass opcServer, string sClientHandle)
		{
			for(int i = 0; i < opcServer.m_server.Subscriptions.Count; i++) 
			{
				if(sClientHandle == opcServer.m_server.Subscriptions[i].State.ClientHandle.ToString()) 
					return opcServer.m_server.Subscriptions[i];
				
			}
			return null;
		}

		static public Opc.Da.SubscriptionState getGroupSubscriptionState(opcGroupReadWriteClass opcGroup)
		{
			Opc.Da.SubscriptionState	state;
			
			state = new SubscriptionState();
			state.Active = true;
			state.Deadband = 0.0F;
			state.Name = opcGroup.groupName;
			state.UpdateRate = (int)opcGroup.uPeriod;
			opcGroup.sClientHandle = Guid.NewGuid().ToString();
			state.ClientHandle = opcGroup.sClientHandle;
			return state;
		}

		static public bool isEqualItemExist(opcGroupReadWriteClass opcGroup, ref opcItemReadWriteClass opcItem, int hap, bool bSave)
		{										// 그룹내의 중복된 아이템이 있는지?			
			int						i;
			opcItemReadWriteClass	imsiItem;
			
			for(i = 0; i < hap; i++)			// 처음은 중복된 아이템이 없으므로 pos 까지 검사, 즉 pos 가 0 이면 return false
			{
				imsiItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				if(opcItem.itemName == imsiItem.itemName) 
				{
					if(bSave == false) return true;					// 핸들을 저장하지 않는 옵션
					opcItem.serverHandle = imsiItem.serverHandle;	// 동일한 이름은 하나만 등록... 동일한 핸들
					opcItem.clientHandle = imsiItem.clientHandle;
					opcItem.readData = imsiItem.readData;			// 동일한 읽은 값을 저장
					opcItem.timeStamp = imsiItem.timeStamp;
					opcItem.reuslt = imsiItem.reuslt;
					opcItem.bFlag = imsiItem.bFlag;
					return true;
				}
			}
			return false;
		}

		static public bool isMultipleItemExist(opcGroupReadWriteClass opcGroup, string name)
		{										// 그룹내의 중복된 아이템이 2개 이상 있는지?
			int						i, count = 0;
			opcItemReadWriteClass	opcItem;
			
			for(i = 0; i < opcGroup.arrItem.Count; i++)			// 처음은 중복된 아이템이 없으므로 pos 까지 검사, 즉 pos 가 0 이면 return false
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				if(name == opcItem.itemName) count ++;
				if(count >= 2) return true;
			}
			return false;
		}

		static public void saveServerClientHandle(opcGroupReadWriteClass opcGroup, ref opcItemReadWriteClass opcItem)
		{
			opcItemReadWriteClass	imsiItem;
			for(int i = 0; i < opcGroup.arrItem.Count; i++)
			{
				imsiItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				if(imsiItem.serverHandle == null) continue;
				if(opcItem.itemName == imsiItem.itemName) 
				{
					opcItem.serverHandle = imsiItem.serverHandle;	// 동일한 이름은 하나만 등록... 동일한 핸들
					opcItem.clientHandle = imsiItem.clientHandle;
					opcItem.readData = imsiItem.readData;			// 동일한 읽은 값을 저장
					opcItem.timeStamp = imsiItem.timeStamp;
					opcItem.reuslt = imsiItem.reuslt;
					opcItem.bFlag = imsiItem.bFlag;
					return;
				}
			}
		}

		static public Opc.Da.Item[] oneSyncGroupDataToArray(opcGroupReadWriteClass opcGroup)
		{
			int						i;
			opcItemReadWriteClass	opcItem;
			ArrayList				arrItems;
			Opc.Da.Item				oneItem;
			
			arrItems = new ArrayList();
			for(i = 0; i < opcGroup.arrItem.Count; i++) 
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				//if(isEqualItemExist(opcGroup, ref opcItem, i, false)) continue; // 동일한 아이템이 존재
				oneItem = new Item();
				oneItem.ItemName = opcItem.itemName;
				arrItems.Add(oneItem);
			}
			return (Item[])arrItems.ToArray(typeof(Item));
		}
		
		static public Opc.Da.Item[] newOneItemDataToArray(ref opcItemReadWriteClass opcItem)//, Opc.Da.Subscription group)
		{
			ArrayList				arrItems;
			Opc.Da.Item				oneItem;
			
			arrItems = new ArrayList();
			oneItem = new Item();
			oneItem.ItemName = opcItem.itemName;
			oneItem.ClientHandle = Guid.NewGuid().ToString();					// add 2005-01-08
			opcItem.clientHandle = oneItem.ClientHandle;
			arrItems.Add(oneItem);
			return (Item[])arrItems.ToArray(typeof(Item));
		}

		static public Opc.Da.Item[] oneItemDataToArray(opcItemReadWriteClass opcItem)//, Opc.Da.Subscription group)
		{
			ArrayList				arrItems;
			Opc.Da.Item				oneItem;
			
			arrItems = new ArrayList();
			oneItem = new Item();
			
			oneItem.ItemName = opcItem.itemName;
			oneItem.ActiveSpecified = true;
			oneItem.ClientHandle = opcItem.clientHandle;
			oneItem.ServerHandle = opcItem.serverHandle;
			oneItem.prevItemName = opcItem.itemName;			
			arrItems.Add(oneItem);
			return (Item[])arrItems.ToArray(typeof(Item));
		}

		static public void addOneOpcItemAndReadHandle(Opc.Da.Subscription group, opcItemReadWriteClass opcItem)
		{
			Opc.Da.Item[]		m_items = null;
			m_items = group.AddItems(newOneItemDataToArray(ref opcItem));
			if(m_items == null || m_items.Length <= 0) return;			// 추가 실패
			opcItem.serverHandle = m_items[0].ServerHandle;
		}
		
		static public void registerOneGroupAsync(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup)
		{
			if(opcServer.m_server == null) return;
			if(opcGroup.bActive == false) return;			// active 가 아니면 등록하지 않는다.

			try 
			{
				Opc.Da.Subscription group = getRegisterSubscription(opcServer, opcGroup.sClientHandle);
				if(group != null) // 이미 등록되어 있다.
				{
					opcServer.m_server.CancelSubscription(group);		// 삭제 후 재 등록	2005-1-13 add		
					//group.ModifyItems((int)StateMask.All, oneGroupDataToArray(opcGroup, true));	// 2005-1-13 delete
				}

				Opc.Da.SubscriptionState state = getGroupSubscriptionState(opcGroup);
				group = (Opc.Da.Subscription)opcServer.m_server.CreateSubscription(state);
				//m_items = group.AddItems(oneGroupDataToArray(opcGroup, true));	// 하나씩 등록하는 것으로 변경, 동일 아이템 때문에 2005-1-13					
				opcItemReadWriteClass	opcItem;
				for(int i = 0; i < opcGroup.arrItem.Count; i++) 
				{
					opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
					if(isEqualItemExist(opcGroup, ref opcItem, i, true)) continue; // Async 읽기이고, 동일한 아이템이 존재
					addOneOpcItemAndReadHandle(group, opcItem);
				}
				//opcServer.m_server.Subscriptions.Add(group);						// CreateSubscription 에서 추가
				group.DataChanged += new DataChangedEventHandler(opcGroup.OnDataChange);
			}
			catch{}
			opcGroup.bRegister = true;
		}

		static public void modifyGroupAsyncProperties(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup)
		{
			
			if(opcServer.m_server == null) return;
			try
			{
				Opc.Da.Subscription group = getRegisterSubscription(opcServer, opcGroup.sClientHandle);
				if(group == null) 
				{				
					registerOneGroupAsync(opcServer, opcGroup);	// 등록
					return;
				}
				group.ModifyState((int)StateMask.All, getGroupSubscriptionState(opcGroup));
			}
			catch{}
			opcGroup.bRegister = true;
		}

		static public void deleteOneGroupAsync(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup)
		{
			if(opcServer.m_server == null) return;
			try 
			{
				Opc.Da.Subscription group = getRegisterSubscription(opcServer, opcGroup.sClientHandle);
				if(group == null) return;							// 삭제할 내용이 없다
				opcServer.m_server.CancelSubscription(group);		// 삭제
			}
			catch{}
			opcGroup.bRegister = false;		
		}
		

		static public void addOneItemAsync(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup, opcItemReadWriteClass opcItem)
		{
			if(opcServer.m_server == null) return;
			if(isMultipleItemExist(opcGroup, opcItem.itemName))		// 동일한 아이템이 2개이상 존재
			{
				saveServerClientHandle(opcGroup, ref opcItem);	// 서버, 클라이언트 핸들을 저장
				return;
			}

			try 
			{
				Opc.Da.Subscription group = getRegisterSubscription(opcServer, opcGroup.sClientHandle);
				if(group == null) registerOneGroupAsync(opcServer, opcGroup);	// 등록
				else 
				{
					addOneOpcItemAndReadHandle(group, opcItem);
					//Opc.Da.Item[]		m_items = null;
					//m_items = group.AddItems(newOneItemDataToArray(ref opcItem));
					//if(m_items == null || m_items.Length <= 0) return;			// 추가 실패
					//opcItem.serverHandle = m_items[0].ServerHandle;
				}
			}
			catch{}
			opcGroup.bRegister = true;
		}

		static public void deleteOneItemAsync(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup, int nItem)
		{
			if(opcServer.m_server == null) return;
			try 
			{
				Opc.Da.Subscription group = getRegisterSubscription(opcServer, opcGroup.sClientHandle);
				if(group == null) registerOneGroupAsync(opcServer, opcGroup);	// 등록				
				else 
				{
					if(nItem < 0 || nItem >= opcGroup.arrItem.Count) return;
					opcItemReadWriteClass opcItem = (opcItemReadWriteClass)opcGroup.arrItem[nItem];
					if(isMultipleItemExist(opcGroup, opcItem.itemName)) return; // 동일한 아이템이 2개이상 존재
					group.RemoveItems(oneItemDataToArray(opcItem));
				}
			}
			catch{}
			opcGroup.bRegister = true;
		}

		static public void GetDefaultValues(Opc.Da.Server m_server, ItemValue[] items)
		{
			try
			{
				// get item value properties.
				ItemPropertyCollection[] propertyLists = m_server.GetProperties(
					items,
					new PropertyID[] { Property.DATATYPE },	true);

				// update item values.
				for (int ii = 0; ii < items.Length; ii++)
				{
					// ignore errors for failures for individual items.
					if (propertyLists[ii].ResultID.Failed())
					{
						continue;
					}

					object defaultValue = null;
					System.Type type = (System.Type)propertyLists[ii][0].Value;
					System.Type baseType = (type.IsArray)?type.GetElementType():type;

					if (baseType == typeof(string))   defaultValue = "";
					if (baseType == typeof(DateTime)) defaultValue = DateTime.Now;
					if (baseType == typeof(object))   defaultValue = "";
					defaultValue = Opc.Convert.ChangeType(defaultValue, baseType);

					// convert to a three element array.
					if (type.IsArray)
					{
						defaultValue = new object[] {defaultValue, defaultValue, defaultValue};
						defaultValue = Opc.Convert.ChangeType(defaultValue, type);
					}				

					// update the object.
					items[ii].Value     =  defaultValue;
					items[ii].QualitySpecified   = false;//!valuesOnly;
					items[ii].TimestampSpecified = false;//!valuesOnly;
				}
			}
			catch
			{
				// ignore errors.
			}
		}
	}

	public class opcClientConfigClass
	{
		public	string			shareName;					// plc_scan 프로그램과 통신을 위한 공유메모리 이름
		public	bool			bRetryConnect;				// 연결실패 시 재시도 사용여부
		public	decimal			nRetryMin;					// 연결실패 시 재시도 사용 minute
		public	decimal			nItemDisplayPeriod = 1000;	// 아이템 리스트 갱신 시간간격

        public int              nWritingCycle=100;          // 쓰기 주기
        public bool             bManualControlToFirst = false;  // 수동제어일 경우 우선권 부여

        public bool             bUseReWriteCheck = false;   // 2008-10-28 add, 엠넥스텍 요구에 의해
        public int              nReWriteCheckTime = 5;      // 2008-10-28 add, 엠넥스텍 요구에 의해, 5 = 5 초, 1초 단위
        public int              nReWriteCheckCount = 1;     // 2008-10-28 add, 엠넥스텍 요구에 의해, 기본 1번 다시 쓴다.
        public bool             bUsePeriodicRead = false;   // 2008-10-29 add, Async group의 각 아이템의 주기적인 읽기 사용여부
        public int              nPeriodicReadTime = 1000;   // 2008-10-29 add, Async group의 각 아이템의 주기적인 읽기 사용시간
        public bool             bUseDeviceReadMode = false; // 2008-10-30 add,, 엠넥스텍 요구에 의해, 읽기 시 디바이스 값을 직접읽는 모드 SYNC일 때만
        public bool             bUseDevicePeriodicReadMode = false; // 2008-10-31 add,, 엠넥스텍 요구에 의해, 주기적인 읽기 시 디바이스 값을 직접읽는 모드 SYNC일 때만
        public bool             bUseWriteDelayWhenAsyncEvent = false; // 2008-11-06 add,, 엠넥스텍 요구에 의해, 지정한 개수와 시간범위 내에 출력한 포인트 이외의 ASYNC EVENT 가 들어올 때 지정한 시간동안 출력을 정지하는 옵션
        public int              nWriteDelayCount = 50;      // 2008-11-06 add,, 엠넥스텍 요구에 의해, 지정한 개수 
        public int              nWriteDelayCheckTime = 20;  // 2008-11-06 add,, 엠넥스텍 요구에 의해, 지정한 시간
        public int              nWriteDelayingTime = 2000;  // 2008-11-06 add,, 엠넥스텍 요구에 의해, 출력정지 시간, 단위:mSec
	}

    public class opcClientWriteDelayWhenAsyncEvnetClass // 2008-11-06 add 엠넥스텍 요구에 의해
    {
        public string   groupName;					// 그룹이름
        public string   itemName; 					// 아이템이름
        //public object   writeVal;                   // 출력한 값
        public int      writeSec;                   // 출력한 시간의 분,초를 초단위로 저장
    }
	

	public class opcServerReadWriteClass
	{
        public  string           hostName;          // 2009-01-22 add 리모트 컴퓨터 연결을 위한 서버이름 또는 IP번지
		public	string			serverName;
		public	string			accessName;			// 서버이름이 길고 외우기 힘들때 다른이름으로 접속하기 위해, 물론 serverName으로도 접속가능
		public  Opc.Da.Server	m_server;
		public	ArrayList		arrGroup;
		public	bool			bDisplay;
		public	bool			bTryConnect;
		public	DateTime		tryConnectTime;		// 연결을 시도한 시간, 성공 또는 실패
        public int              nPeriodicReadGroup = 0;// 2008-10-29 add for 엠넥스텍
        public ArrayList        arrWriteDelay;      // 2008-11-06 add for 엠넥스텍
        public bool             bWriteDelayWait = false;// 2008-11-06 add for 엠넥스텍
        public TimeOutMiliSecClass writeDelayTimeout = new TimeOutMiliSecClass();// 2008-11-06 add for 엠넥스텍
	}

	public class opcGroupReadWriteClass
	{
		public	string			groupName;
		public	uint			uPeriod;
		public  uint			milli;
		public	ArrayList		arrItem;
		//public	bool			bReRead;
		public	bool			bActive;			// 읽기를 사용할 것인지 ?
		public	bool			bAsync;				// Async 읽기를 사용할 것인가?
		public	bool			bRegister;			// Async 읽기일 때 subscription을 등록했는가?
		public	string			sClientHandle;		// handle 을 문자열로 변환 : Async 읽기일 때
		public	string			serverName;			// COM에 서버이름을 입력해야 하므로.. 내그룹의 서버이름을 저장, 2004-06-14 추가
        public  int             nPeriodicReadItem = 0;// 2008-10-29 add for 엠넥스텍

        public opcServerReadWriteClass pServer;

        public opcGroupReadWriteClass(opcServerReadWriteClass server)
        {
            pServer = server;
        }

		public void OnDataChange(object subscriptionHandle, ItemValueResult[] values)
		{
			//opcGroupReadWriteClass	opcGroup = this.p;
			opcGroupReadWriteClass	opcGroup = this;
			//if(opcGroup.bAsync == false) return;
			if(opcGroup.sClientHandle != (string)subscriptionHandle) return;
			if(values == null) return;
			
			for(int i = 0; i < values.Length; i++) checkAndSetReadValue(opcGroup, values[i]);
		}

		void checkAndSetReadValue(opcGroupReadWriteClass opcGroup, ItemValueResult val)
		{
			opcItemReadWriteClass	opcItem;
			for(int i = 0; i < opcGroup.arrItem.Count; i++) 
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				if(val.ItemName != opcItem.itemName) continue;
				opcItem.bNewRead = true;
				opcItem.reuslt = val.ResultID;
                if (val == null)            // 2008-10-31 분리, 아래 if문에서 || 로 체크하던 것을
                {
                    opcItem.bFlag = false;                    
                    continue;
                }
				if(val.Value == null)
				{
                    opcItem.timeStamp = val.Timestamp;  // 2008-10-31 add
                    opcItem.quality = val.Quality;      // 2008-10-31 add
					opcItem.bFlag = false;
                    if (comShareClass.bShareDllExist) comShareClass.comOpcShareDataChanged((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);// 2008-10-31 add, (opcGroup.pServer != null) ? opcGroup.pServer.hostName : "", 2009-01-22 add
					//if(val. == Opc.Type.DOUBLE) opcItem.readData = 0;
					continue;
				}
                opcItem.timeStamp = val.Timestamp;

				if(opcItem.readData != (object)val.Value || opcItem.quality != val.Quality) // 값이 바뀌었으면
				{
					opcItem.readData = (object)val.Value;
                    opcItem.quality = val.Quality;
                    if (comShareClass.bShareDllExist) comShareClass.comOpcShareDataChanged((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);//(opcGroup.pServer != null) ? opcGroup.pServer.hostName : "" 2009-01-22 add
				}
				opcItem.bFlag = true;
                isWriteDelayAsyncEvent(opcItem.itemName);   // 2008-11-06 add
			}
		}

        void isWriteDelayAsyncEvent(string itemName) // 2008-11-06 add
        {
            if (opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent == false) return;
            if (pServer.arrWriteDelay == null || pServer.arrWriteDelay.Count <= 0)
            {
                pServer.bWriteDelayWait = false;
                return;
            }
            opcClientWriteDelayWhenAsyncEvnetClass writeDelay;
            for(int i = 0; i < pServer.arrWriteDelay.Count; i++) {
                writeDelay = (opcClientWriteDelayWhenAsyncEvnetClass)pServer.arrWriteDelay[i];
                if(writeDelay.groupName == groupName && writeDelay.itemName == itemName) return;
                if (string.Compare(groupName, 0, "_EventExceptionGroup_", 0, 21) == 0) return;  // 2008-11-13, 엠넥스텍에서 한솔테크 사무실에서 테스트한 후 필요에 따라 추가
            }
            //if (pServer.bWriteDelayWait == false) // deleted 2008-11-13
                pServer.writeDelayTimeout.Reset();
            pServer.bWriteDelayWait = true;
        }
	}



	public class opcItemReadWriteClass
	{
		public	string			itemName;
		public	string			accessName;			// 아이템이름이 길고 외우기 힘들때 다른이름으로 접속하기 위해, 물론 itemName으로도 접속가능
		public	bool			bFlag;
		public	object			readData;
		public	DateTime		timeStamp;
		public  bool			bNewRead;			// 새로 읽었다.
		public	ResultID		reuslt;
		public	object			clientHandle;		// Async Read 시 GUID 저장 변수, 2005-01-08 추가
		public	object			serverHandle;		// Async Read 시 Server Handle 저장변수, 2005-01-08 추가
	//	public	uint			dataCount;			// array 등을 위한 개수, 기본데이터는 1개
        public Quality          quality;
        public short            nCurrWriteCount;    // 2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해
        public object           writeVal;           // 2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해
        public int              writeStartTime;     // 2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해, 분*60 + 초 만을 저장, 초 단위를 저장
        public int              itemPos;            // 2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해, 감시/통신에서 보내온 item pos를 저장
	}
}
