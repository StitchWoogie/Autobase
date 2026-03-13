using NetTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalMain.OPCUA
{
    enum eSavedDataType2
    {
        connections = 1,
        pkiSetup = 2,
        settings = 3,
        others = 0,
    };


    public class OpcUaServerConfig
    {
        public int Port { get; set; }
        public bool EnableSecurity { get; set; }
        public bool AllowNone { get; set; }
    }

    public static class OpcUaServerConfigStore
    {
        public static void Load()
        {
            loadOpcConifg();
        }

        public static void Save()
        {
            saveConfigData();
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


        static eSavedDataType2 getSavedDataType2(string data)
        {

            if (string.Compare(data, "Connection", false) == 0) return eSavedDataType2.connections;
            if (string.Compare(data, "PKISetup", false) == 0) return eSavedDataType2.pkiSetup;
            if (string.Compare(data, "Settings", false) == 0) return eSavedDataType2.settings;
            return eSavedDataType2.others;
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
        static void saveConfigData()
        {
            string dir = AutoLibLocal.TotalConfig.sDirWorkProject + "\\OpcData";
            Directory.CreateDirectory(dir);

            string filename = Path.Combine(dir, "UAServerConfig.ini");

            using (var writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                // Connection
                writer.WriteLine(
                    "Connection,{0},{1},{2},{3},{4}",
                    1,              // bAuto (임시)
                    30,             // nAutoTimer
                    1,              // bRetry
                    5,              // nRetry
                    OPCUAServerMain.nPort
                );

                // PKISetup
                int pkiFlag = 0;
                if (OPCUAServerMain.bSecurity) pkiFlag |= 2;
                if (OPCUAServerMain.bNone) pkiFlag |= 1;

                writer.WriteLine(
                    "PKISetup,{0},{1},{2},",
                    "client_cert.der",
                    "client_key.pem",
                    pkiFlag
                );

                // Settings (placeholder)
                writer.WriteLine(
                    "Settings,{0},{1},{2},0,0",
                    1000,   // UpdateTimer
                    0,      // TagCount
                    0       // ServerSleep
                );
            }
        }


    }
}
