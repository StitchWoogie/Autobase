using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NetTools;
using System.Collections;

using opcBasic = OPCUAMain.OPCUAClientMain;
using OPCUAMain;

namespace OPCUAMain
{
    public class opcReadWriteFileAccessClass
    {
        enum eSavedDataType { SERVER = 0, GROUP, ITEM, ETC };
        enum eSavedDataType2 
        {  connections = 1,
            pkiSetup = 2,
            settings = 3,
            others = 0,
        };

        public opcReadWriteFileAccessClass()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static eSavedDataType getSavedDataType(string data)
        {
            if (string.Compare(data, "server", false) == 0) return eSavedDataType.SERVER;
            if (string.Compare(data, "group", false) == 0) return eSavedDataType.GROUP;
            if (string.Compare(data, "item", false) == 0) return eSavedDataType.ITEM;

            
            return eSavedDataType.ETC;
        }

        static eSavedDataType2 getSavedDataType2(string data)
        {

            if (string.Compare(data, "Connection", false) == 0) return eSavedDataType2.connections;
            if (string.Compare(data, "PKISetup", false) == 0) return eSavedDataType2.pkiSetup;
            if (string.Compare(data, "Settings", false) == 0) return eSavedDataType2.settings;
            return eSavedDataType2.others;
        }

        static void getServerData(string one_line)
        {
            CommaBlockString comma = new CommaBlockString();
            string imsi = "";

            comma.Set(one_line);
            comma.GetString(ref imsi);          // server
            comma.GetString(ref imsi);          // Provier 
            comma.GetString(ref imsi);           // TCP addr
            if (imsi.Length <= 0) return;
            string buf_addr = imsi;

            comma.GetString(ref imsi);           // End Point
            if (imsi.Length <= 0) return;
            string buf_endpoint = imsi;

            comma.GetString(ref imsi);           // Index
            int index =  Convert.ToInt32( imsi );

            comma.GetString(ref imsi);           // Access Name
            if (imsi.Length <= 0) return;
            string buf_access = imsi;



            OPCUAMember_server _server = new OPCUAMember_server(buf_addr, buf_endpoint, buf_access, index);


            opcBasic.Servers.Add(_server);

        }


        static void getGroupData(string one_line)
        {
            if (opcBasic.Servers.Count <= 0) return;   // 등록된 서버가 없다

            CommaBlockString comma = new CommaBlockString();
            string imsi = "";
            int nServer = opcBasic.Servers.Count - 1;// 항상마지막 서버에 쓴다

            comma.Set(one_line);
            comma.GetString(ref imsi);  //group
            comma.GetString(ref imsi);  //groupname
            if (imsi.Length <= 0) return;

            string buf_group = imsi;

            comma.GetString(ref imsi);  //read gap
            if (imsi.Length <= 0) return;
            int buf_interval = Convert.ToInt32(imsi);

            comma.GetString(ref imsi);  //reserved

            comma.GetString(ref imsi);  //reserved

            comma.GetString(ref imsi);  //reserved

            comma.GetString(ref imsi);  //reserved

            OPCUAMember_group _group = new OPCUAMember_group(buf_group, buf_interval);

            OPCUAMember_server _server = (OPCUAMember_server)opcBasic.Servers[nServer];
            _server.arrGroup.Add(_group);


        }

        static void getItemData(string one_line)
        {
            if (opcBasic.Servers.Count <= 0) return;   // 등록된 서버가 없다

            CommaBlockString comma = new CommaBlockString();
            string imsi = "";
            int nServer = opcBasic.Servers.Count - 1;// 항상마지막 서버에 쓴다
            //double val = 0.0;

            OPCUAMember_server opcServer = (OPCUAMember_server)opcBasic.Servers[nServer];
            if (opcServer.arrGroup.Count <= 0) return;      // 등록된 그룹이 없다
            int nGroup = opcServer.arrGroup.Count - 1;

            OPCUAMember_group opcGroup = (OPCUAMember_group)opcServer.arrGroup[nGroup];

            comma.Set(one_line);
            comma.GetString(ref imsi);  //item
            comma.GetString(ref imsi);  //item node
            if (imsi.Length <= 0) return;

            string buf_node = imsi;

            comma.GetString(ref imsi);  //Alias
            if (imsi.Length <= 0) return;

            string buf_access = imsi;

            comma.GetString(ref imsi);  //reserved

            comma.GetString(ref imsi);  //reserved

            comma.GetString(ref imsi);  //value
            string buf_value = imsi;

            OPCUAMember_item _item = new OPCUAMember_item(buf_access, buf_node, buf_value);
            opcGroup.arrItem.Add(_item);

        }

        static void getOptionConnect(string one_line)
        {
            CommaBlockString comma = new CommaBlockString();
            string imsi = "";

            comma.Set(one_line);
            comma.GetString(ref imsi);         //Connection
            comma.GetString(ref imsi);          //bAuto
            comma.GetString(ref imsi);          //nAutoTimer
            comma.GetString(ref imsi);          //bReTry
            comma.GetString(ref imsi);          //nAReTry


            comma.GetString(ref imsi);           // nPort
            if (imsi.Length <= 0) return;
            try
            {
                int port_num = Convert.ToInt32(imsi);

                OPCUAServerMain.nPort = port_num;
            }
            catch (Exception ex)
            {

            }

        }

        static void getOpionPKISetup(string one_line)
        {
            CommaBlockString comma = new CommaBlockString();
            string imsi = "";

            comma.Set(one_line);
            comma.GetString(ref imsi);         //PKISetup
            comma.GetString(ref imsi);          //cert filename
            comma.GetString(ref imsi);          //key filename
            comma.GetString(ref imsi);          //nPKIEnable
            if (imsi.Length <= 0) return;
            try
            {
                int pki_enable = Convert.ToInt32(imsi);
                if ((pki_enable & 2) > 0)
                {
                    OPCUAServerMain.bSecurity = true;
                }
                else
                {
                    OPCUAServerMain.bSecurity = false;
                }

                if ((pki_enable & 1) > 0)
                {
                    OPCUAServerMain.bNone = true;
                }
                else
                {
                    OPCUAServerMain.bNone = false;
                }

            }
            catch (Exception ex)
            {

            }


            comma.GetString(ref imsi);          //encrypt password
            comma.GetString(ref imsi);           // reserved2
           

        }

        static void getOpionSettings(string one_line)
        {
            CommaBlockString comma = new CommaBlockString();
            string imsi = "";

            comma.Set(one_line);
            comma.GetString(ref imsi);         //Settings
            comma.GetString(ref imsi);          //UpdateTimer
            comma.GetString(ref imsi);          //Tag Count
            comma.GetString(ref imsi);          //Server Sleep

            comma.GetString(ref imsi);          //ereserved2
            comma.GetString(ref imsi);           // reserved3


        }



        static void loadOpcSerGroupItemData()
        {
            if (opcBasic.Servers == null) return;

            string filename = AutoLibLocal.TotalConfig.sDirWorkProject;
            filename += "\\OpcData\\UAServer.ini";

            //if(System.OperatingSystem.IsLinux() ) filename = FileNameControls.FindFileIgnoreCase(filename);

            if (!File.Exists(filename)) return;
            FileStream fs = File.OpenRead(filename);
            if (fs == null) return;

            TextReader reader = new StreamReader(fs);
            CommaBlockString comma = new CommaBlockString();
            string one_line, imsi = "";
            int pos = 0;

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);
                comma.GetString(ref imsi);
                switch (getSavedDataType(imsi))
                {
                    case eSavedDataType.SERVER: getServerData(one_line); break;
                    case eSavedDataType.GROUP: getGroupData(one_line); break;
                    case eSavedDataType.ITEM: getItemData(one_line); break;
                    default: break;
                }
                pos++;
                if (pos > 300000) break;
            }
            reader.Close();
        }

        static void loadConfigData()
        {
            //if (opcBasic.opcClientConfig == null) opcBasic.opcClientConfig = new opcClientConfigClass();
            //opcBasic.opcClientConfig.shareName = "SharedName";

            //string filename = AutoLibLocal.TotalConfig.sDirWorkProject;
            //filename += "\\OpcData\\opcConfig.ini";
            //if (!File.Exists(filename)) return;
            //FileStream fs = File.OpenRead(filename);
            //if (fs == null) return;

            //TextReader reader = new StreamReader(fs);
            //CommaBlockString comma = new CommaBlockString();
            //string one_line, imsi = "";
            //int i = 0;

            //one_line = reader.ReadLine();
            //if (one_line == null)
            //{
            //    reader.Close();
            //    return;
            //}
            //comma.Set(one_line);
            //comma.GetString(ref imsi);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.shareName = imsi;
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.bRetryConnect = (i == 1) ? true : false;
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.nRetryMin = (i >= 1 && i <= 5000) ? i : 60;
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.nItemDisplayPeriod = (i >= 50 && i <= 30000) ? i : 1000;

            //comma.GetInt(ref i);
            //opcBasic.opcClientConfig.nWritingCycle = i;

            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.bUseReWriteCheck = (i == 1) ? true : false;// add 2008-10-28 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.nReWriteCheckCount = (i >= 1 && i <= 10) ? i : 1;// add 2008-10-28 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.nReWriteCheckTime = (i >= 2 && i <= 3600) ? i : 5;// add 2008-10-28 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.bUsePeriodicRead = (i == 1) ? true : false;// add 2008-10-29 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.nPeriodicReadTime = (i >= 100 && i <= 60000) ? i : 5000;// add 2008-10-29 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.bUseDeviceReadMode = (i == 1) ? true : false;// add 2008-10-30 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.bUseDevicePeriodicReadMode = (i == 1) ? true : false;// add 2008-10-31 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent = (i == 1) ? true : false;// add 2008-11-06 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.nWriteDelayCount = (i >= 1 && i <= 1000) ? i : 50;// add 2008-11-06 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.nWriteDelayCheckTime = (i >= 1 && i <= 60) ? i : 20;// add 2008-11-06 엠넥스텍 요구에의해
            //comma.GetInt(ref i);
            //if (imsi.Length > 0) opcBasic.opcClientConfig.nWriteDelayingTime = (i >= 100 && i <= 60000) ? i : 2000;// add 2008-11-06 엠넥스텍 요구에의해

            //comma.GetBool(ref opcBasic.opcClientConfig.bManualControlToFirst);  // 2008-11-14 추가 수동제어일 경우 우선권 부여 기능

            //reader.Close();
        }

        static void loadOpcConifg()
        {

            string filename = AutoLibLocal.TotalConfig.sDirWorkProject;
            filename += "\\OpcData\\UAServerConfig.ini";

            //if(System.OperatingSystem.IsLinux() ) filename = FileNameControls.FindFileIgnoreCase(filename);

            if (!File.Exists(filename)) return;
            FileStream fs = File.OpenRead(filename);
            if (fs == null) return;

            TextReader reader = new StreamReader(fs);
            CommaBlockString comma = new CommaBlockString();
            string one_line, imsi = "";
            int pos = 0;

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);
                comma.GetString(ref imsi);
                switch (getSavedDataType2(imsi))
                {
                    case eSavedDataType2.connections: getOptionConnect(one_line); break;
                    case eSavedDataType2.pkiSetup: getOpionPKISetup(one_line); break;
                    case eSavedDataType2.settings: getOpionSettings(one_line); break;
                    default: break;
                }
                pos++;
                if (pos > 300000) break;
            }
            reader.Close();
        }


        static void saveOpcSerGroupItemData()
        {
            //string filename = AutoLibLocal.TotalConfig.sDirWorkProject + "\\OpcData";
            //if (!Directory.Exists(filename))
            //{
            //    Directory.CreateDirectory(filename);
            //}
            //filename += "\\Server.ini";

            //Stream fs = File.Open(filename, FileMode.Create);
            //if (fs == null) return;

            //TextWriter writer = new StreamWriter(fs);
            //if (opcBasic.arrOpcServer.Count <= 0)
            //{
            //    writer.WriteLine(" ");
            //    writer.Close();
            //    return;
            //}

            //opcServerReadWriteClass opcServer;
            //opcGroupReadWriteClass opcGroup;
            //opcItemReadWriteClass opcItem;
            //int i, j, k;

            //for (i = 0; i < opcBasic.arrOpcServer.Count; i++)
            //{
            //    opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];
            //    writer.WriteLine("server,{0},{1},{2},", opcServer.serverName, opcServer.accessName, opcServer.hostName);    // opcServer.hostName, 2009-01-22 add

            //    for (j = 0; j < opcServer.arrGroup.Count; j++)
            //    {
            //        opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];
            //        writer.WriteLine("group,{0},{1},{2},{3},", opcGroup.groupName, opcGroup.uPeriod, (opcGroup.bActive) ? 1 : 0, (opcGroup.bAsync) ? 1 : 0);

            //        for (k = 0; k < opcGroup.arrItem.Count; k++)
            //        {
            //            opcItem = (opcItemReadWriteClass)opcGroup.arrItem[k];
            //            writer.WriteLine("item,{0},{1},{2},", opcItem.itemName, opcItem.accessName, opcItem.readData);

            //        }
            //    }
            //}
            //writer.Close();
        }

        static void saveConfigData()
        {
            //string filename = AutoLibLocal.TotalConfig.sDirWorkProject + "\\OpcData";
            //if (!Directory.Exists(filename))
            //{
            //    Directory.CreateDirectory(filename);
            //}
            //filename += "\\opcConfig.ini";

            //Stream fs = File.Open(filename, FileMode.Create);
            //if (fs == null) return;

            //TextWriter writer = new StreamWriter(fs);
            //writer.Write("{0},{1},{2},{3},", opcBasic.opcClientConfig.shareName, (opcBasic.opcClientConfig.bRetryConnect) ? 1 : 0, opcBasic.opcClientConfig.nRetryMin, opcBasic.opcClientConfig.nItemDisplayPeriod);
            //writer.Write("{0},", opcBasic.opcClientConfig.nWritingCycle);
            //writer.Write("{0},{1},{2},{3},{4},", (opcBasic.opcClientConfig.bUseReWriteCheck) ? 1 : 0, opcBasic.opcClientConfig.nReWriteCheckCount, opcBasic.opcClientConfig.nReWriteCheckTime, (opcBasic.opcClientConfig.bUsePeriodicRead) ? 1 : 0, opcBasic.opcClientConfig.nPeriodicReadTime);   // add 2008-10-28 엠넥스텍 요구에의해
            //writer.Write("{0},{1},", (opcBasic.opcClientConfig.bUseDeviceReadMode) ? 1 : 0, (opcBasic.opcClientConfig.bUseDevicePeriodicReadMode) ? 1 : 0);   // add 2008-10-31 엠넥스텍 요구에의해
            //writer.Write("{0},{1},{2},{3},", (opcBasic.opcClientConfig.bUseWriteDelayWhenAsyncEvent) ? 1 : 0, opcBasic.opcClientConfig.nWriteDelayCount, opcBasic.opcClientConfig.nWriteDelayCheckTime, opcBasic.opcClientConfig.nWriteDelayingTime);   // add 2008-11-06 엠넥스텍 요구에의해
            //writer.Write("{0},", opcBasic.opcClientConfig.bManualControlToFirst);   // add 2008-11-14 수동출력 우선권 부여 기능
            //writer.WriteLine();
            //writer.Close();
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

        static public void loadOpcReadWriteConfigData2()
        {

            loadOpcConifg();
        }



    }
}
