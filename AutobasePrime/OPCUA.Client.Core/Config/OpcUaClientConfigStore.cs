using NetTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using opcBasic = OPCUA.Client.Core.OPCUAClientMain;

namespace OPCUA.Client.Core.Config
{
    public class OpcUaClientConfigStore
    {
        private readonly string _baseDir;
        enum eSavedDataType { SERVER = 0, GROUP, ITEM, ETC };

        public OpcUaClientConfigStore(string baseDir)
        {
            _baseDir = baseDir;
        }

        public OpcUaClientConfig Load()
        {
            var config = new OpcUaClientConfig();
            LoadClientServers();

            // - client 전용 옵션
            loadConfigData();

            return config;
        }

        public void Save(OpcUaClientConfig config)
        {
            // 기존 saveOpcSerGroupItemData()의
            // "Client 부분만" 여기로 이동
        }

        static void LoadClientServers()
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


        static eSavedDataType getSavedDataType(string data)
        {
            if (string.Compare(data, "server", false) == 0) return eSavedDataType.SERVER;
            if (string.Compare(data, "group", false) == 0) return eSavedDataType.GROUP;
            if (string.Compare(data, "item", false) == 0) return eSavedDataType.ITEM;


            return eSavedDataType.ETC;
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
            int index = Convert.ToInt32(imsi);

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

            OPCUAMember_server _server = (OPCUAMember_server)opcBasic.Servers[nServer];

            OPCUAMember_group _group = new OPCUAMember_group(_server, buf_group, buf_interval);
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
            opcGroup.AddItem(_item);

        }


        static void saveOpcSerGroupItemData()
        {
            string dir = AutoLibLocal.TotalConfig.sDirWorkProject + "\\OpcData";
            Directory.CreateDirectory(dir);

            string filename = Path.Combine(dir, "UAServer.ini");

            using (var writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                foreach (var srv in opcBasic.Servers)
                {
                    // server,<Provider>,<TCPAddr>,<Endpoint>,<Index>,<AccessName>
                    writer.WriteLine(
                        "server,{0},{1},{2},{3},{4}",
                        srv.serverName ?? "",
                        srv.hostName ?? "",
                        srv.hostName ?? "",
                        srv.endpointindex,
                        srv.accessName ?? ""
                    );

                    if (srv.arrGroup == null) continue;

                    foreach (var grp in srv.arrGroup)
                    {
                        // group,<GroupName>,<Interval>,0,0,0,0
                        writer.WriteLine(
                            "group,{0},{1},0,0,0,0",
                            grp.sName,
                            grp.nInterval
                        );

                        if (grp.arrItem == null) continue;

                        foreach (var item in grp.arrItem)
                        {
                            // item,<NodeId>,<Alias>,0,0,<Value>
                            writer.WriteLine(
                                "item,{0},{1},0,0,{2}",
                                item.sNode ?? "",
                                item.sName ?? "",
                                item.sValue ?? ""
                            );
                        }
                    }
                }
            }
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



    }
}
