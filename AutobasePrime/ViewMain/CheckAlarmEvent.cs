using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLib;
using NetTools.Hash;
using System.Data;
using NetTools;
using System.Xml;
using System.IO;
using GraphicModule;
using AutoLibLocal;

namespace ViewMain
{
    class CheckAlarmEvent
    {
        public static void Check()
        {
            if (!CheckServerEvent.bAlarmEventChanged) return;

            CheckServerEvent.bAlarmEventChanged = false;

            string data = null;

            if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
            {
                AutoLib.WcfReferenceDataGateServer.ServiceDataGateServerClient service = ServiceLibSvcDataGate.GetServiceDataGate();

                try
                {
                    MakeHashCrc ht = new MakeHashCrc();
                    string hash = ht.ComputeHash("GetAlarmEvents" + ServiceLibSvcDataGate.nConnectionID.ToString());

                    data = service.GetAlarmEvents(ServiceLibSvcDataGate.nConnectionID, hash);
                }
                catch
                {
                    data = null;
                }
            }
            else
            {
                if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 2, 7, 6))
                {
                    AutoLib.ServiceReferenceDataGateServer.WcfServiceDataGateServerProxyClient service = ServiceLib.GetServiceDataGateServer();

                    try
                    {
                        MakeHashCrc ht = new MakeHashCrc();
                        ServiceLibSvcDataGate.nConnectionID = -2;
                        string hash = ht.ComputeHash("GetAlarmEvents" + ServiceLibSvcDataGate.nConnectionID.ToString());

                        data = service.GetAlarmEvents(ServiceLibSvcDataGate.nConnectionID, hash);
                    }
                    catch 
                    {
                        data = null;
                    }
                }
            }

            if (string.IsNullOrEmpty(data)) return;
                        
            DataSet ds = new DataSet();
            ds.ReadXml(new XmlTextReader(new StringReader(data)));
            
            DataRow row;

            //blockAlarmConfirmNot.Clear();
            FormAlarmEvent.blockAlarmConfirmNot.Clear();

            // 테이블이 없는 경우는 경보가 하나도 없는 경우이므로 Update는 해주어야 한다.
            if (ds.Tables.Count > 0)
            {
                ALARM_CONFIRMATION_STRUCT item;

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    row = ds.Tables[0].Rows[i];

                    item = new ALARM_CONFIRMATION_STRUCT();

                    item.msg_type = ConvertTool.ToUInt16(row["alarm_type"].ToString());
                    item.t.Set(ConvertTool.ToDateTime(row["alarm_datetime"].ToString()));
                    item.tag = row["Tag"].ToString();
                    item.description = row["Description"].ToString();
                    item.message = row["Message"].ToString();
                    item.bAlarm = ConvertTool.ToSByte(row["bAlarm"].ToString());
                    item.bConfirmMethod = ConvertTool.ToSByte(row["bConfirmMethod"].ToString());
                    item.tReturn.Set(ConvertTool.ToDateTime(row["tReturn"].ToString()));
                    item.id = ConvertTool.ToInt32(row["ID"].ToString());

                    //blockAlarmConfirmNot.Add(item);
                    FormAlarmEvent.blockAlarmConfirmNot.Add(item);
                }
            }

            FormAlarmEvent.AlarmConfirmCountChanged();
        }
    }
}
