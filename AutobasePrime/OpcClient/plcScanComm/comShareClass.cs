using System;
using System.IO;
using System.Windows.Forms;
using Opc.Da;
using Microsoft.Win32;
using AutoLibLocal;
using System.Collections;

namespace OpcClient.plcScanComm
{
	/// <summary>
	/// Summary description for comShareClass.
	/// </summary>
	public class comShareClass
	{
		public static bool		bShareDllExist = false;


		static comShareClass()
		{
			//
			// TODO: Add constructor logic here
			//
			//
            //bShareDllExist = File.Exists(Application.StartupPath + "\\ComOpcShare_9_2_0.dll");
            bShareDllExist = true;
		}

        static RingSharedMemory smWriteMemory = new RingSharedMemory();

        public static void Init()
        {
            smWriteMemory.Create("NetOpcWriteSharedMemory", 10, 1000);
        }

        public static void UnInit()
        {
            smWriteMemory.Close();
        }

        static void DeleteOpcDataRegistry()
        {
            string main_key_path;

            main_key_path = String.Format("Software\\AutoBase\\OpcData");

            try
            {
                AutoLibLocal.TotalConfig.GetRootRegistryKey().DeleteSubKeyTree(main_key_path);
            }
            catch   // Sub키 Tree가 없을 때는 오류가 발생해서 Catch를 추가 2007.1.22
            {

            }
        }

		public static void comOpcShareAllRegisterItem()
		{
            DeleteOpcDataRegistry();    // 이전에 사용된 읽기에 사용한 레지스트리를 지운다.

			//using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			//{
				//share.ClearItem();								// 처음 등록하면서 이전에 등록된 모든 아이템을 지운다.
				//share.ClearWrite();								// 처음 등록하면서 이전에 쓰기명령을 모두지운다.
                

				opcServerReadWriteClass opcServer;
				opcGroupReadWriteClass	opcGroup;
				opcItemReadWriteClass	opcItem;
				for(int i = 0; i < opcBasic.arrOpcServer.Count; i++) 
				{
					opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];
					for(int j = 0; j < opcServer.arrGroup.Count; j++) 
					{
						opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];
						for(int k = 0; k < opcGroup.arrItem.Count; k++) 
						{
							opcItem = (opcItemReadWriteClass)opcGroup.arrItem[k];
							//share.AddItem(opcServer.serverName, opcGroup.groupName, opcItem.itemName, opcItem.readData);
                            DataToRegistry(opcServer.accessName, opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);
						}
					}
				}
			//}
		}

		public static void comOpcShareOneRegisterItem(string serverAccessName, string groupName, string itemName, object val)
		{
            /*
			using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			{
				share.AddItem(serverName, groupName, itemName, val);
			}*/

            DataToRegistry(serverAccessName, groupName, itemName, val, Quality.Good);
		}

		public static void comOpcShareOneRegisterGroup(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup)
		{
			opcItemReadWriteClass	opcItem;
			//using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			{
				for(int k = 0; k < opcGroup.arrItem.Count; k++) 
				{
					opcItem = (opcItemReadWriteClass)opcGroup.arrItem[k];
					//share.AddItem(opcServer.serverName, opcGroup.groupName, opcItem.itemName, opcItem.readData);
                    DataToRegistry(opcServer.accessName, opcGroup.groupName, opcItem.itemName, opcItem.readData, Quality.Good);
				}
			}
		}

		public static void comOpcShareOneRegisterServer(opcServerReadWriteClass opcServer)
		{
			opcGroupReadWriteClass opcGroup;
			opcItemReadWriteClass	opcItem;
			//using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			{
				for(int j = 0; j < opcServer.arrGroup.Count; j++) 
				{
					opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];
					for(int k = 0; k < opcGroup.arrItem.Count; k++) 
					{
						opcItem = (opcItemReadWriteClass)opcGroup.arrItem[k];
						//share.AddItem(opcServer.serverName, opcGroup.groupName, opcItem.itemName, opcItem.readData);
                        DataToRegistry(opcServer.accessName, opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);
					}
				}
			}
		}

		public static void comOpcShareOneDeleteItem(string serverName, string groupName, string itemName)
		{
			//if(bShareDllExist == false) return;		// using를 선언만해도 다운되기 때문에 이 코드는 안된다

            /*
			using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			{
				share.DeleteItem(serverName, groupName, itemName);				
			}*/
		}

		
		public static void comOpcShareOneDeleteItem(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup, int nItem)
		{
			if(nItem >= opcGroup.arrItem.Count) return;
			//using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			{
				opcItemReadWriteClass	opcItem = (opcItemReadWriteClass)opcGroup.arrItem[nItem];
				//share.DeleteItem(opcServer.serverName, opcGroup.groupName, opcItem.itemName);
			}
		}

		public static void comOpcShareOneDeleteGroup(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup)
		{
			opcItemReadWriteClass	opcItem;
			//using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			{
				for(int k = 0; k < opcGroup.arrItem.Count; k++) 
				{
					opcItem = (opcItemReadWriteClass)opcGroup.arrItem[k];
					//share.DeleteItem(opcServer.serverName, opcGroup.groupName, opcItem.itemName);
				}
			}
		}

		public static void comOpcShareOneDeleteServer(opcServerReadWriteClass opcServer)
		{
			opcGroupReadWriteClass opcGroup;
			opcItemReadWriteClass	opcItem;
			//using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			{
				for(int j = 0; j < opcServer.arrGroup.Count; j++) 
				{
					opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];
					for(int k = 0; k < opcGroup.arrItem.Count; k++) 
					{
						opcItem = (opcItemReadWriteClass)opcGroup.arrItem[k];
						//share.DeleteItem(opcServer.serverName, opcGroup.groupName, opcItem.itemName);
					}
				}
			}
		}

        //static void DataToRegistry(string hostName, string serverName, string groupName, string itemName, object obj, Quality quality)
        static void DataToRegistry(string serverAccessName, string groupName, string itemName, object obj, Quality quality)
        {
            if (serverAccessName == null || serverAccessName.Length <= 0) return;
            //if (hostName.Length > 0) serverName = hostName + "." + serverName;
            string data="";

            data = quality.GetCode().ToString()+',';

            if(obj.GetType() == Opc.Type.ARRAY_BINARY) 
			{
				byte[] o = (byte[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString()+',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_BOOLEAN)
			{
				bool[] o = (bool[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_DATETIME)
			{
				DateTime[] o = (DateTime[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_DECIMAL)
			{
				decimal[] o = (decimal[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_DOUBLE)	
			{
				double[] o = (double[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_FLOAT)	
			{
				float[] o = (float[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_INT)	
			{
				int[] o = (int[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_LONG)	
			{
				long[] o = (long[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_SBINARY)
			{
				sbyte[] o = (sbyte[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_SHORT)	
			{
				short[] o = (short[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_UINT)	
			{
				uint[] o = (uint[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_ULONG)	
			{
				ulong[] o = (ulong[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
			else if(obj.GetType() == Opc.Type.ARRAY_USHORT)	
			{
				ushort[] o = (ushort[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += o[i].ToString() + ',';
                }
			}
            else if (obj.GetType() == Opc.Type.ARRAY_STRING)
            {
                string[] o = (string[])obj;
                for (int i = 0; i < o.Length; i++)
                {
                    data += NetTools.CommaTextWriter.MakeString(o[i]) + ',';
                }
            }
            else if (obj.GetType() == Opc.Type.ARRAY_STRING)
            {
                string o = (string)obj;
                data += NetTools.CommaTextWriter.MakeString(o);
            }
            else
            {
                data += obj.ToString();
            }

            // 같은 그룹속에 아이템이 너무 많으면 속도가 떨어져서 sum 폴더로 256개 구분했다. 2016-4-27
            //AutoLibLocal.TotalConfig.SaveRegAutoBaseConfig("OpcData", serverAccessName+"\\"+groupName, itemName, data);

            byte sum = SharedTag.GetSum8(itemName);
            AutoLibLocal.TotalConfig.SaveRegAutoBaseConfig("OpcData", serverAccessName + "\\" + groupName + "\\" + sum.ToString(), itemName, data);
        }

		public static void comOpcShareDataChanged(string serverAccessName, string groupName, string itemName, object val, Quality quality)
		{
            DataToRegistry(serverAccessName, groupName, itemName, val, quality);

            /*
            using(ComOpcShare.Share share = new ComOpcShare.Share()) 
			{
				share.DataChanged(serverName, groupName, itemName, val);
			}
            */
		}

        static ArrayList arrayWaitWrite = new ArrayList();
        static NetTools.TimeOutMiliSecClass timeoutWrite = new NetTools.TimeOutMiliSecClass();

                
        public static bool checkItemIsWriteArray(string serverName, string groupName, string itemName)//2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해
        {
            OpcWriteItem item;
            for (int i = 0; i < arrayWaitWrite.Count; i++)
            {
                item = (OpcWriteItem)arrayWaitWrite[i];
                if (item.itemname == itemName && item.groupname == groupName && item.servername == serverName) return true;
            }
            return false;
        }

        public static void checkAndAddArrayReWrite()//2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해
        {
            if (opcBasic.opcClientConfig.bUseReWriteCheck == false || arrayWaitWrite.Count >= 10000) return;

            int i, j, k;
            opcServerReadWriteClass opcServer;
            opcGroupReadWriteClass opcGroup;
            opcItemReadWriteClass opcItem;
            OpcWriteItem item;
            DateTime dt = DateTime.Now;         //2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해
            int currTime = dt.Minute * 60 + dt.Second, imsiTime = 0;

            for (i = 0; i < opcBasic.arrOpcServer.Count; i++)
            {
                opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];
                if (opcServer.m_server == null || opcServer.m_server.IsConnected == false) continue;

                for (j = 0; j < opcServer.arrGroup.Count; j++)
                {
                    opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];
                    if (opcGroup.bActive == false) continue;
                    for (k = 0; k < opcGroup.arrItem.Count; k++)
                    {
                        if (arrayWaitWrite.Count >= 10000) 
                        {
                            return;
                        }

                        opcItem = (opcItemReadWriteClass)opcGroup.arrItem[k];
                        if (opcItem.nCurrWriteCount == 0) continue;             // 출력하지 않았다.
                        imsiTime = currTime;
                        if (imsiTime < opcItem.writeStartTime) imsiTime += 3600;// 1시간을 더해준다.
                        imsiTime = imsiTime - opcItem.writeStartTime;
                        if (imsiTime < opcBasic.opcClientConfig.nReWriteCheckTime) continue;// 지정한 시간이 지나지 않았다
                        opcItem.writeStartTime = currTime;      // 현재시간으로 재 설정
                        if (checkItemIsWriteArray(opcServer.serverName, opcGroup.groupName, opcItem.itemName))
                        {
                            opcItem.nCurrWriteCount = 0;        // 출력명령이 어레이에 대기중이다.
                            continue;
                        }
                        if (opcItem.writeVal == opcItem.readData || opcItem.nCurrWriteCount > opcBasic.opcClientConfig.nReWriteCheckCount)
                        {
                            opcItem.nCurrWriteCount = 0;        // 출력이 정상적으로 완료되거나 reWrite 횟수를 초과
                            continue;
                        }
                        opcItem.nCurrWriteCount++;              // 출력 횟수를 증가
                        item = new OpcWriteItem();
                        item.data = opcItem.writeVal;
                        item.groupname = opcGroup.groupName;
                        item.itemname = opcItem.itemName;
                        item.pos = opcItem.itemPos;
                        item.servername = opcServer.serverName;
                        item.writeSource = 2;               // re write
                        if (arrayWaitWrite.Count < 10000) // 10000개 까지만 받는다.
                        {
                            arrayWaitWrite.Add(item);
                        }                        
                    }
                }
            }            
            
        }

		public static void comOpcShareCheckAndWriteItem()
		{
            OpcWriteItem item;

            // 모아 놓는다.
            for (int i = 0; i < 5; i++)
            {
                item = (OpcWriteItem)smWriteMemory.GetItem();
                if (item == null) break;

                if (arrayWaitWrite.Count < 10000) // 10000개 까지만 받는다.
                {
                    if (opcBasic.opcClientConfig.bManualControlToFirst && item.bManualOperation)
                    {
                        arrayWaitWrite.Insert(0, item);
                    }
                    else
                    {
                        arrayWaitWrite.Add(item);
                    }
                }

                if (arrayWaitWrite.Count >= 1000)   // 1000개 이상은 메시지만 보여준다.
                {
                    string msg = String.Format("OpcClientMain에서 너무 많은 쓰기 아이템이 대기중입니다. (쓰기 대기수={0})", arrayWaitWrite.Count);
                    MessageDisplay.Show(msg);
                }
            }

            try
            {
                checkAndAddArrayReWrite();//2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해
            }
            catch { }


            if (arrayWaitWrite.Count <= 0) return;  // 더 이상 쓰기 목록이 없다.

            if (!timeoutWrite.IsTimeOut(opcBasic.opcClientConfig.nWritingCycle))    return;
            
            timeoutWrite.Reset();
                        
            item = (OpcWriteItem)arrayWaitWrite[0];
            //arrayWaitWrite.RemoveAt(0);       // delete this line 2008-11-06, 아래에서 처리

            string serverName, groupName, itemName;
            object val;
            int pos;

            serverName = item.servername;
            groupName = item.groupname;
            itemName = item.itemname;
            pos = item.pos;
            val = item.data;


            if (serverName.Length <= 0 || groupName.Length <= 0 || itemName.Length <= 0 || val == null)
            {
                arrayWaitWrite.RemoveAt(0);   // 2008-11-06 위에 있던 것을 여기에 복사
                return;
            }

			int		nServer = 0;
            if (opcBasic.getServerStringFromServerNamePos(ref nServer, serverName) == false)
            {
                arrayWaitWrite.RemoveAt(0);   // 2008-11-06 위에 있던 것을 여기에 복사
                return;
            }
			int		nGroup = 0;
            if (opcBasic.getGroupStringFromGroupNamePos(nServer, ref nGroup, groupName) == false)
            {
                arrayWaitWrite.RemoveAt(0);   // 2008-11-06 위에 있던 것을 여기에 복사
                return;
            }
			int		nItem = 0;
            if (opcBasic.getItemStringFromItemNamePos(nServer, nGroup, ref nItem, itemName) == false)
            {
                arrayWaitWrite.RemoveAt(0);   // 2008-11-06 위에 있던 것을 여기에 복사
                return;
            }
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
            if (checkWriteDelayTimeoutAsyncEvent(opcServer) == false) return;   // 2008-11-06 add

            arrayWaitWrite.RemoveAt(0);   // 2008-11-06 위에 있던 것을 여기에 복사
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			opcItemReadWriteClass opcItem = (opcItemReadWriteClass)opcGroup.arrItem[nItem];

			plcScanCommMainClass.eRWDataTypeCode		flag;
			try 
			{
				if(val.GetType() == Opc.Type.STRING || val.GetType() == Opc.Type.ARRAY_STRING) 
				{
					flag = plcScanCommMainClass.eRWDataTypeCode.STRING_DATA;
					plcScanCommMainClass.writeString = (string)val;
				}
				else if(val.GetType() == Opc.Type.DATETIME || val.GetType() == Opc.Type.ARRAY_DATETIME) 
				{
					flag = plcScanCommMainClass.eRWDataTypeCode.DATETIME_DATA;
					plcScanCommMainClass.writeString = Convert.ToString(val);
				}				
				else 
				{
					flag = plcScanCommMainClass.eRWDataTypeCode.DOUBLE_DATA;
					plcScanCommMainClass.dWriteVal = Convert.ToDouble(val);
				}
				
			}
			catch 
			{
				return;
			}
            if (item.writeSource == 2)
            {
                reWriteItem(opcServer, opcGroup, opcItem); // 2008-10-28 add : item.writeSource
            }
            else
            {
                writeItem(opcServer, opcGroup, opcItem, itemName, flag, pos);// opcGroup : 2008-10-31 add
            }
            insertWriteDelayMemory(opcServer, opcGroup.groupName, opcItem.itemName);// 2008-11-06 add

		}

        public static void insertWriteDelayMemory(opcServerReadWriteClass opcServer, string groupName, string itemName) // 2008-11-06 add
        {
            try
            {
                if (opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent == false) return;

                if (opcServer.arrWriteDelay == null) opcServer.arrWriteDelay = new ArrayList();
                if (opcServer.arrWriteDelay == null) return;

                if (opcServer.arrWriteDelay.Count >= 1 && opcServer.arrWriteDelay.Count >= opcBasic.opcClientConfig.nWriteDelayCount)
                {
                    opcServer.arrWriteDelay.RemoveAt(opcServer.arrWriteDelay.Count-1);
                }

                DateTime dt = DateTime.Now;
                opcClientWriteDelayWhenAsyncEvnetClass writeDelay = new opcClientWriteDelayWhenAsyncEvnetClass();
                writeDelay.groupName = groupName;
                writeDelay.itemName = itemName;
                writeDelay.writeSec = dt.Minute * 60 + dt.Second;
                opcServer.arrWriteDelay.Insert(0, writeDelay);
                if (opcServer.bWriteDelayWait == false) opcServer.writeDelayTimeout.Reset();
            }
            catch { }
        }

        public static void checkAndRemoveWriteDelayMemory(opcServerReadWriteClass opcServer) // 2008-11-06 add
        {
            try
            {             
                if (opcServer.arrWriteDelay == null) return;
                if (opcServer.arrWriteDelay.Count <= 0) return;
                
                DateTime dt = DateTime.Now;
                int curr, sec = dt.Minute * 60 + dt.Second;
                opcClientWriteDelayWhenAsyncEvnetClass writeDelay;
                for (int i = opcServer.arrWriteDelay.Count-1; i >= 0; i--)
                {
                    writeDelay = (opcClientWriteDelayWhenAsyncEvnetClass)opcServer.arrWriteDelay[i];
                    if (writeDelay.writeSec > sec) curr = sec + 3600 - writeDelay.writeSec;
                    else curr = sec - writeDelay.writeSec;
                    if (curr >= opcBasic.opcClientConfig.nWriteDelayCheckTime)
                    {
                        opcServer.arrWriteDelay.RemoveAt(i);
                    }
                    else return;        // 시간이 경과되지 않았으면, 앞쪽도 경과되지 않았다.
                }
            }
            catch { }
        }

        public static bool checkWriteDelayTimeoutAsyncEvent(opcServerReadWriteClass opcServer) // 2008-11-06 add
        {
            checkAndRemoveWriteDelayMemory(opcServer);
            if (opcServer.bWriteDelayWait == false) return true;
            if (opcServer.arrWriteDelay == null || opcServer.arrWriteDelay.Count <= 0)
            {
                opcServer.bWriteDelayWait = false;
                opcServer.writeDelayTimeout.Reset();
                return true;
            }
            if (opcServer.writeDelayTimeout.IsTimeOut(opcBasic.opcClientConfig.nWriteDelayingTime))
            {
                opcServer.bWriteDelayWait = false;
                opcServer.writeDelayTimeout.Reset();
                return true;
            }
            return false;
        }


		/*static void writeItem(opcServerReadWriteClass opcServer, opcItemReadWriteClass opcItem, string itemName, plcScanCommMainClass.eRWDataTypeCode flag, int itemPos)
		{
			ItemValueResult[] readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, itemName);
			if (readResults == null || (readResults[0].ResultID != Opc.ResultID.E_WRITEONLY && readResults[0].Value == null)) return;
			
			object		val;
			int			writePos = 0;

			if(readResults[0].ResultID != Opc.ResultID.S_OK)// == Opc.ResultID.E_WRITEONLY)  modify 2004-07-23
			{
				ItemValue item = new ItemValue();
				item.ItemName = itemName;
				opcBasic.GetDefaultValues(opcServer.m_server, new ItemValue[] { item });
				if(item.Value == null) readResults[0].Value = 0;			// Null 일 때는 기본으로 Int32로 설정
				else				   readResults[0].Value = item.Value;				
			}
			
			if(opcBasic.isValueObjectIsSingleType(readResults[0].Value)) itemPos = 0;		// 싱글 타입일 경우는 항상 0, 출력할 번지가 없다
			if(plcScanCommMainClass.writeItemValueToObject(flag, readResults[0].Value, itemPos, ref writePos, out val) != 1) return;
			
			Opc.IdentifiedResult[] writeResults = opcReadWriteGroupLoopClass.writeOneItem(opcServer, itemName, val);
			
			if (writeResults == null) return;

			if(readResults[0].ResultID != Opc.ResultID.S_OK)// == Opc.ResultID.E_WRITEONLY)  modify 2004-07-23, 쓴 값을 setting
			{
				if(opcItem == null) return;
				opcItem.bNewRead = true;
				opcItem.readData = val;
			}
		}*/

        static void writeItem(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup, opcItemReadWriteClass opcItem, string itemName, plcScanCommMainClass.eRWDataTypeCode flag, int itemPos)// 2007-12-12, 새로 수정 엠넥스텍 안되는 것 때문에, opcGroupReadWriteClass opcGroup : 2008-10-31 add
        {
            if (opcItem == null) return;
            object readVal;
            ItemValueResult[] readResults;

            if (opcItem.reuslt != Opc.ResultID.S_OK)// S_OK 가 아닐 때만 새로 읽도록 수정, 2007-12-12 엠넥스텍에서 출력이 잘 안된다고해서 ...
            {                                       // 윈래는 위의 if 문이 없었음
                // readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, itemName, false);     // 처음에는 cache로 읽고
                readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, itemName, opcBasic.opcClientConfig.bUseDevicePeriodicReadMode);
                // 이전에는 false 이었으나 중앙제어에서 아직 아이템이 읽지 않아서 활성화가 안되었을 때 출력이 안나간다고 해서 수정함.(9에서는 잘되었다고 함) 2015-2-23 

                // 중앙제어에서 아이템이 S_FALSE상태일 때 OpcClient에서 직접 출력하면 되나 감시에서 출력할 때는 출력이 안나간다고 해서 검사해보니
                // readResults[0].Value 가 null이라서 출력이 안나갔음 그래서 이 부분을 제외시켰음. 2015-3-19 수정
                // if (readResults == null || (readResults[0].ResultID != Opc.ResultID.E_WRITEONLY && readResults[0].Value == null)) return;
                if (readResults == null) return;

                if (readResults[0].ResultID != Opc.ResultID.S_OK)// == Opc.ResultID.E_WRITEONLY)  modify 2004-07-23
                {
                    ItemValue item = new ItemValue();
                    item.ItemName = itemName;
                    opcBasic.GetDefaultValues(opcServer.m_server, new ItemValue[] { item });
                    if (item.Value == null) readResults[0].Value = 0;			// Null 일 때는 기본으로 Int32로 설정
                    else readResults[0].Value = item.Value;
                }
                readVal = readResults[0].Value;
            }
            else
            {                       // 윈래는 else 부분이 없었음                
                readVal = opcItem.readData;
            }

            object val;
            int writePos = 0;

            if (opcBasic.isValueObjectIsSingleType(readVal)) itemPos = 0;		// 싱글 타입일 경우는 항상 0, 출력할 번지가 없다
            if (plcScanCommMainClass.writeItemValueToObject(flag, readVal, itemPos, ref writePos, out val) != 1) return;

            Opc.IdentifiedResult[] writeResults = opcReadWriteGroupLoopClass.writeOneItem(opcServer, itemName, val);
            opcItem.nCurrWriteCount = 1;        //2008-10-27 add 엠넥스텍에서 요구한 여러번 출력을 위해
            opcItem.writeVal = val;             //2008-10-27 add 엠넥스텍에서 요구한 여러번 출력을 위해
            opcItem.itemPos = itemPos;          //2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해            
            DateTime dt = DateTime.Now;         //2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해            
            opcItem.writeStartTime = dt.Minute * 60 + dt.Second; //2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해, 초 단위로 저장

            if (opcBasic.opcClientConfig.bUseDeviceReadMode)
            {
                readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, itemName, true);     // device로 읽고
                if (readResults == null) return;
                opcItem.bNewRead = true;
                opcItem.reuslt = readResults[0].ResultID;
                opcItem.timeStamp = readResults[0].Timestamp;
                if (readResults[0].Value == null)
                {
                    if (opcItem.quality != readResults[0].Quality) // quality 가 바뀌었으면
                    {
                        opcItem.quality = readResults[0].Quality;
                        if (comShareClass.bShareDllExist) comShareClass.comOpcShareDataChanged((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);
                    }
                    opcItem.bFlag = false;
                }
                else
                {
                    if (opcItem.readData != (object)readResults[0].Value || opcItem.quality != readResults[0].Quality) // 값이 바뀌었으면
                    {
                        opcItem.readData = (object)readResults[0].Value;
                        opcItem.quality = readResults[0].Quality;
                        if (comShareClass.bShareDllExist) comShareClass.comOpcShareDataChanged((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);
                    }
                    opcItem.bFlag = true;
                }
            }
            
            if (writeResults == null) return;
            //if (writeResults[0].ResultID == Opc.ResultID.S_OK)// 쓴 값을 setting
            //{
            //  if (opcItem == null) return;
            //  opcItem.bNewRead = true;
            //  opcItem.readData = val;
            //}
        }

        static void reWriteItem(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup, opcItemReadWriteClass opcItem)//2008-10-27 add 엠넥스텍에서 요구한 여러번 출력을 위해
        {
            if (opcServer == null || opcItem == null) return;
            Opc.IdentifiedResult[] writeResults = opcReadWriteGroupLoopClass.writeOneItem(opcServer, opcItem.itemName, opcItem.writeVal);

            if (opcBasic.opcClientConfig.bUseDeviceReadMode)
            {
                ItemValueResult[] readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, opcItem.itemName, true);     // device로 읽고
                if (readResults == null) return;
                opcItem.bNewRead = true;
                opcItem.reuslt = readResults[0].ResultID;
                opcItem.timeStamp = readResults[0].Timestamp;
                if (readResults[0].Value == null)
                {
                    if (opcItem.quality != readResults[0].Quality) // quality 가 바뀌었으면
                    {
                        opcItem.quality = readResults[0].Quality;
                        if (comShareClass.bShareDllExist) comShareClass.comOpcShareDataChanged((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);
                    }
                    opcItem.bFlag = false;
                }
                else
                {
                    if (opcItem.readData != (object)readResults[0].Value || opcItem.quality != readResults[0].Quality) // 값이 바뀌었으면
                    {
                        opcItem.readData = (object)readResults[0].Value;
                        opcItem.quality = readResults[0].Quality;
                        if (comShareClass.bShareDllExist) comShareClass.comOpcShareDataChanged((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, opcItem.itemName, opcItem.readData, opcItem.quality);
                    }
                    opcItem.bFlag = true;
                }
            }
            if (writeResults == null) return;            
        }        



	}
}
