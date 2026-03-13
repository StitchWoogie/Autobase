using System;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using NetTools;

namespace OpcClient
{
	/// <summary>
	/// Summary description for opcReadWriteFileAccessClass.
	/// </summary>
	public class opcReadWriteFileAccessClass
	{
		enum eSavedDataType { SERVER = 0, GROUP, ITEM, ETC };

		public opcReadWriteFileAccessClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static eSavedDataType getSavedDataType(string data)
		{
			if(string.Compare(data, "server", false) == 0) return eSavedDataType.SERVER;
			if(string.Compare(data, "group", false) == 0) return eSavedDataType.GROUP;
			if(string.Compare(data, "item", false) == 0) return eSavedDataType.ITEM;
			return eSavedDataType.ETC;
		}

		static void getServerData(string one_line)
		{
			CommaBlockString		comma = new CommaBlockString();
			string					imsi = "";			

			comma.Set(one_line);
			comma.GetString(ref imsi);			// type
			comma.GetString(ref imsi);
			if(imsi.Length <= 0) return;		// null name

			opcServerReadWriteClass opcServer = new opcServerReadWriteClass();
			opcServer.serverName = imsi;
            opcServer.hostName = "";            // 2009-01-22 add
            if (comma.IsEOS()) opcServer.accessName = opcServer.serverName;// 2009-01-22 modify "" -> opcServer.serverName, 엑세스이름이 없으면 OPC 서버 이름으로 설정하도록 변경
            else
            {
                comma.GetString(ref imsi);
                opcServer.accessName = imsi;
                opcServer.accessName = opcServer.accessName.Trim();// 2009-01-22 add
                if (opcServer.accessName.Length <= 0) opcServer.accessName = opcServer.serverName;// 2009-01-22 modify "" -> opcServer.serverName, 엑세스이름이 없으면 OPC 서버 이름으로 설정하도록 변경
                if (comma.IsEOS() == false)     // 2009-01-22 add
                {
                    comma.GetString(ref imsi);
                    opcServer.hostName = imsi;
                }
            }
			opcServer.m_server = null;
			opcServer.bTryConnect = false;
			opcServer.arrGroup = new ArrayList();
			opcBasic.arrOpcServer.Add(opcServer);			
		}


		static void getGroupData(string one_line)
		{
			if(opcBasic.arrOpcServer.Count <= 0) return;	// 등록된 서버가 없다

			CommaBlockString		comma = new CommaBlockString();
			string					imsi = "";
			int						val = 0, nServer = opcBasic.arrOpcServer.Count-1;// 항상마지막 서버에 쓴다

			comma.Set(one_line);
			comma.GetString(ref imsi);			// type
			comma.GetString(ref imsi);
			if(imsi.Length <= 0) return;		// null name

			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			opcGroupReadWriteClass opcGroup = new opcGroupReadWriteClass(opcServer);
			opcGroup.groupName = imsi;
			opcGroup.serverName = opcServer.serverName;			// 서버이름을 보관, COM에 전송하기 위해서는 서버이름이 필요하다, ASYNC Read 에서
			comma.GetInt(ref val);
			opcGroup.uPeriod = (uint)val;
			opcGroup.milli = 0;						// 시간을 0으로 설정			
			opcGroup.bActive = true;				// 읽기를 한다
			opcGroup.bAsync = true;
			opcGroup.bRegister = false;				// 등록되지 않았다.
			if(comma.IsEOS() == false)				// 데이터가 읽을 경우
			{
				comma.GetInt(ref val);
				opcGroup.bActive = (val == 0) ? false : true;
			}
			if(comma.IsEOS() == false)				// 데이터가 읽을 경우
			{
				comma.GetInt(ref val);
				opcGroup.bAsync = (val == 0) ? false : true;
			}
			opcGroup.arrItem = new ArrayList();
			opcServer.arrGroup.Add(opcGroup);			
		}

		static void getItemData(string one_line)
		{
			if(opcBasic.arrOpcServer.Count <= 0) return;	// 등록된 서버가 없다

			CommaBlockString		comma = new CommaBlockString();
			string					imsi = "";
			int						nServer = opcBasic.arrOpcServer.Count-1;// 항상마지막 서버에 쓴다
			double					val = 0.0;

			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			if(opcServer.arrGroup.Count <= 0) return;		// 등록된 그룹이 없다
			int						nGroup = opcServer.arrGroup.Count-1;

			comma.Set(one_line);
			comma.GetString(ref imsi);			// type
			comma.GetString(ref imsi);
			if(imsi.Length <= 0) return;		// null name

			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			opcItemReadWriteClass opcItem = new opcItemReadWriteClass();

			opcItem.itemName = imsi;
			comma.GetString(ref imsi);			// access name
			if(imsi.Length <= 0) imsi = "";
			opcItem.accessName = imsi;

			comma.GetDouble(ref val);//.GetInt(ref val);				// read Data
			opcItem.readData = (object)val;
			opcItem.bFlag = false;
			opcItem.bNewRead = false;
			opcGroup.arrItem.Add(opcItem);
		}

		static void loadOpcSerGroupItemData()
		{
			if(opcBasic.arrOpcServer == null) opcBasic.arrOpcServer = new ArrayList();
			
			string filename = AutoLibLocal.TotalConfig.sDirWorkProject;
			filename += "\\OpcData\\Server.ini";
			if(!File.Exists(filename))	return;
			FileStream fs = File.OpenRead(filename);			
			if(fs == null)	return;

			TextReader				reader = new StreamReader(fs);
			CommaBlockString		comma = new CommaBlockString();
			string					one_line, imsi = "";
			int						pos = 0;			
			
			while(true)
			{
				one_line = reader.ReadLine();
				if(one_line == null) break;
				comma.Set(one_line);
				comma.GetString(ref imsi);
				switch(getSavedDataType(imsi)) 
				{
					case eSavedDataType.SERVER : getServerData(one_line); break;
					case eSavedDataType.GROUP :	getGroupData(one_line);	break;
					case eSavedDataType.ITEM : getItemData(one_line); break;
					default : break;
				}
				pos++;
				if(pos > 300000) break;
			}
			reader.Close();
		}

		static void loadConfigData()
		{
			if(opcBasic.opcClientConfig == null) opcBasic.opcClientConfig = new opcClientConfigClass();			
			opcBasic.opcClientConfig.shareName = "SharedName";

            string filename = AutoLibLocal.TotalConfig.sDirWorkProject;
			filename += "\\OpcData\\opcConfig.ini";			
			if(!File.Exists(filename))	return;
			FileStream fs = File.OpenRead(filename);			
			if(fs == null)	return;

			TextReader				reader = new StreamReader(fs);
			CommaBlockString		comma = new CommaBlockString();
			string					one_line, imsi = "";
			int						i = 0;
			
			one_line = reader.ReadLine();
			if(one_line == null) 
			{
				reader.Close();
				return;
			}			
			comma.Set(one_line);
			comma.GetString(ref imsi);
			if(imsi.Length > 0) opcBasic.opcClientConfig.shareName = imsi;
			comma.GetInt(ref i);
			if(imsi.Length > 0) opcBasic.opcClientConfig.bRetryConnect = (i == 1) ? true : false;
			comma.GetInt(ref i);
			if(imsi.Length > 0) opcBasic.opcClientConfig.nRetryMin = (i >= 1 && i <= 5000) ? i : 60;
			comma.GetInt(ref i);
			if(imsi.Length > 0) opcBasic.opcClientConfig.nItemDisplayPeriod = (i >= 50 && i <= 30000) ? i : 1000;

            comma.GetInt(ref i);
            opcBasic.opcClientConfig.nWritingCycle = i;

            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.bUseReWriteCheck = (i == 1) ? true : false;// add 2008-10-28 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.nReWriteCheckCount = (i >= 1 && i <= 10) ? i : 1;// add 2008-10-28 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.nReWriteCheckTime = (i >= 2 && i <= 3600) ? i : 5;// add 2008-10-28 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.bUsePeriodicRead = (i == 1) ? true : false;// add 2008-10-29 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.nPeriodicReadTime = (i >= 100 && i <= 60000) ? i : 5000;// add 2008-10-29 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.bUseDeviceReadMode = (i == 1) ? true : false;// add 2008-10-30 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.bUseDevicePeriodicReadMode = (i == 1) ? true : false;// add 2008-10-31 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent = (i == 1) ? true : false;// add 2008-11-06 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.nWriteDelayCount = (i >= 1 && i <= 1000) ? i : 50;// add 2008-11-06 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.nWriteDelayCheckTime = (i >= 1 && i <= 60) ? i : 20;// add 2008-11-06 엠넥스텍 요구에의해
            comma.GetInt(ref i);
            if (imsi.Length > 0) opcBasic.opcClientConfig.nWriteDelayingTime = (i >= 100 && i <= 60000) ? i : 2000;// add 2008-11-06 엠넥스텍 요구에의해

            comma.GetBool(ref opcBasic.opcClientConfig.bManualControlToFirst);  // 2008-11-14 추가 수동제어일 경우 우선권 부여 기능

			reader.Close();
		}
		

		static void saveOpcSerGroupItemData()
		{
            string filename = AutoLibLocal.TotalConfig.sDirWorkProject + "\\OpcData";
			if(!Directory.Exists(filename)) 
			{
				Directory.CreateDirectory(filename);
			}
			filename += "\\Server.ini";

			Stream fs = File.Open(filename, FileMode.Create);
			if(fs == null)	return;

			TextWriter				writer = new StreamWriter(fs);
			if(opcBasic.arrOpcServer.Count <= 0) 
			{
				writer.WriteLine(" ");
				writer.Close();
				return;
			}

			opcServerReadWriteClass opcServer;
			opcGroupReadWriteClass	opcGroup;
			opcItemReadWriteClass	opcItem;			
			int						i, j, k;

			for(i = 0; i < opcBasic.arrOpcServer.Count; i++) 
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];
                writer.WriteLine("server,{0},{1},{2},", opcServer.serverName, opcServer.accessName, opcServer.hostName);    // opcServer.hostName, 2009-01-22 add

				for(j = 0; j < opcServer.arrGroup.Count; j++) 
				{
					opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];
					writer.WriteLine("group,{0},{1},{2},{3},", opcGroup.groupName, opcGroup.uPeriod, (opcGroup.bActive) ? 1 : 0, (opcGroup.bAsync) ? 1 : 0);

					for(k = 0; k < opcGroup.arrItem.Count; k++) 
					{
						opcItem = (opcItemReadWriteClass)opcGroup.arrItem[k];
						writer.WriteLine("item,{0},{1},{2},", opcItem.itemName, opcItem.accessName, opcItem.readData);

					}					
				}				
			}
			writer.Close();
		}

		static void saveConfigData()
		{
            string filename = AutoLibLocal.TotalConfig.sDirWorkProject + "\\OpcData";
			if(!Directory.Exists(filename)) 
			{
				Directory.CreateDirectory(filename);
			}
			filename += "\\opcConfig.ini";
			
			Stream fs = File.Open(filename, FileMode.Create);
			if(fs == null)	return;

			TextWriter				writer = new StreamWriter(fs);			
			writer.Write("{0},{1},{2},{3},", opcBasic.opcClientConfig.shareName, (opcBasic.opcClientConfig.bRetryConnect) ? 1 : 0, opcBasic.opcClientConfig.nRetryMin, opcBasic.opcClientConfig.nItemDisplayPeriod);
            writer.Write("{0},", opcBasic.opcClientConfig.nWritingCycle);
            writer.Write("{0},{1},{2},{3},{4},", (opcBasic.opcClientConfig.bUseReWriteCheck) ? 1 : 0, opcBasic.opcClientConfig.nReWriteCheckCount, opcBasic.opcClientConfig.nReWriteCheckTime, (opcBasic.opcClientConfig.bUsePeriodicRead) ? 1 : 0, opcBasic.opcClientConfig.nPeriodicReadTime);   // add 2008-10-28 엠넥스텍 요구에의해
            writer.Write("{0},{1},", (opcBasic.opcClientConfig.bUseDeviceReadMode) ? 1 : 0, (opcBasic.opcClientConfig.bUseDevicePeriodicReadMode) ? 1 : 0);   // add 2008-10-31 엠넥스텍 요구에의해
            writer.Write("{0},{1},{2},{3},", (opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent) ? 1 : 0, opcBasic.opcClientConfig.nWriteDelayCount, opcBasic.opcClientConfig.nWriteDelayCheckTime, opcBasic.opcClientConfig.nWriteDelayingTime);   // add 2008-11-06 엠넥스텍 요구에의해
            writer.Write("{0},", opcBasic.opcClientConfig.bManualControlToFirst);   // add 2008-11-14 수동출력 우선권 부여 기능
            writer.WriteLine();
			writer.Close();
		}

		static public void loadOpcReadWriteConfigData()
		{
			loadOpcSerGroupItemData();
			loadConfigData();
		}

		static public void saveOpcReadWriteConfigData()
		{
			saveOpcSerGroupItemData();
			saveConfigData();
		}



	}
}
