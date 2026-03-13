using AutoLibLocal;
using NetTools;
using PortalServerWeb.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "WcfServiceDataTag"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 WcfServiceDataTag.svc나 WcfServiceDataTag.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WcfServiceDataTag : IServiceDataTag
    {
        private HttpContext Ctx => HttpContext.Current;

        public int CheckServiceAlive(int val) => ~val;

        public DataSet GetTagValueList(ArrayList array)
        {
            return GetTagValueListInternal(array);
        }

        private DataSet GetTagValueListInternal(ArrayList array)
        {
            try
            {
                string buf = "";
                foreach (string s in array)
                    buf += s + ",";

                string recv;
                if (!ServiceDataTagStatic.SendAndGetData(Ctx, EnumMultiBlockCommand.TagValueList, buf, out recv))
                    return null;

                DataSet ds = new DataSet("TAG");
                DataTable dt = new DataTable("Table1");
                dt.Columns.Add("TAG", typeof(string));
                dt.Columns.Add("Curr", typeof(string));
                ds.Tables.Add(dt);

                CommaTextReader comma = new CommaTextReader();
                comma.Set(recv);

                for (int i = 0; i < array.Count; i++)
                {
                    string tag = (string)array[i];
                    string val = "";
                    comma.GetString(ref val);

                    var row = dt.NewRow();
                    row["TAG"] = tag;
                    row["Curr"] = val;
                    dt.Rows.Add(row);
                }

                return ds;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WcfServiceDataTag.GetTagValueList Exception: " + ex.Message);
                return null;
            }
        }

        public bool WriteCurrAI(string tag, double val, string guid)
        {
            try
            {
                string err;

                if(!GuidHeartbeatManager.IsClientConnected(guid))
                {
                    return false;
                }

                if (!ConfigWeb.CheckBelowSecurityLevel3(out err)) return false;

                string work = ProjectLib.GetWorkDir(Ctx.Request);
                TotalConfig.sDirWorkProject = work;

                string buf = $"{tag},{val},WebService,{Ctx.Request.UserHostAddress},WebServer";

                string recv;
                ServiceDataTagStatic.SendAndGetData(Ctx, EnumMultiBlockCommand.SetTagValue, buf, out recv);
                return true;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("WcfServiceDataTag.WriteCurrAI Exception: " + ex.Message);
                return false;
            }

        }

        public bool WriteCurrST(string tag, string val, string guid)
        {
            try
            {
                string err;
                if (!GuidHeartbeatManager.IsClientConnected(guid))
                {
                    return false;
                }
                if (!ConfigWeb.CheckBelowSecurityLevel3(out err)) return false;

                string work = ProjectLib.GetWorkDir(Ctx.Request);
                TotalConfig.sDirWorkProject = work;

                string buf = $"{tag},{val},WebService,{Ctx.Request.UserHostAddress},WebServer";

                string recv;
                ServiceDataTagStatic.SendAndGetData(Ctx, EnumMultiBlockCommand.SetTagValue, buf, out recv);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WcfServiceDataTag.WriteCurrST Exception: " + ex.Message);
                return false;
            }
        }

        public async Task< DataSet> GetDataAi(string tag, int value_type, int data_time,
            int year, int mon, int day, int hour, int min, int count, int gab)
            => await ServiceDataTagStatic.GetDataAi(Ctx, tag, value_type, data_time,
                                              year, mon, day, hour, min, count, gab);

        public async Task<DataSet> GetDataDi(string tag, int value_type, int data_time,
            int year, int mon, int day, int hour, int min, int count, int gab)
            => await ServiceDataTagStatic.GetDataDi(Ctx, tag, value_type, data_time,
                                              year, mon, day, hour, min, count, gab);

        public async Task<DataSet> GetLogLists()
        {
            try
            {
                string work = ProjectLib.GetWorkDir(Ctx.Request);
                ServiceDataTagStatic.TryLogIn(work);
                TotalConfig.sDirWorkProject = work;

                DataLocal local = new DataLocal();
                return await local.GetLogLists();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WcfServiceDataTag.GetLogLists Exception: " + ex.Message);
                return null;
            }
        }

        public async Task<DataSet> GetLogFile(string name)
        {
            try
            {
                string work = ProjectLib.GetWorkDir(Ctx.Request);
                ServiceDataTagStatic.TryLogIn(work);
                TotalConfig.sDirWorkProject = work;

                DataLocal local = new DataLocal();
                return await local.GetLogFile(name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WcfServiceDataTag.GetLogFile Exception: " + ex.Message);
                return null;
            }
        }

        public async Task< DataSet> GetAlarmLists()
        {
            try
            {
                string work = ProjectLib.GetWorkDir(Ctx.Request);
                ServiceDataTagStatic.TryLogIn(work);
                TotalConfig.sDirWorkProject = work;

                DataLocal local = new DataLocal();
                return await local.GetAlarmListsAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WcfServiceDataTag.GetAlarmLists Exception: " + ex.Message);
                return null;
            }
        }

        public async Task<DataSet> GetAlarmFile(string name)
        {
            try
            {
                string work = ProjectLib.GetWorkDir(Ctx.Request);
                ServiceDataTagStatic.TryLogIn(work);
                TotalConfig.sDirWorkProject = work;

                DataLocal local = new DataLocal();
                return await local.GetAlarmFileAsync(name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WcfServiceDataTag.GetAlarmFile Exception: " + ex.Message);
                return null;
            }
        }

        public async Task<DataSet> GetAlarmFileByScript(int year, int month, int day, int option)
        {
            try
            {
                string work = ProjectLib.GetWorkDir(Ctx.Request);
                ServiceDataTagStatic.TryLogIn(work);
                TotalConfig.sDirWorkProject = work;

                DataLocal local = new DataLocal();
                DateTime tFrom = new DateTime(year, month, day, 0, 0, 0);
                DateTime tTo = new DateTime(year, month, day, 23, 59, 59);
                return await local.GetAlarmFileByScript(tFrom, tTo, "", false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WcfServiceDataTag.GetAlarmFileByScript Exception: " + ex.Message);
                return null;
            }
        }

        public async Task<DataSet> GetAlarmFileByScript2(DateTime tFrom, DateTime tTo, string option)
        {
            try
            {
                string work = ProjectLib.GetWorkDir(Ctx.Request);
                ServiceDataTagStatic.TryLogIn(work);
                TotalConfig.sDirWorkProject = work;

                DataLocal local = new DataLocal();
                return await local.GetAlarmFileByScript(tFrom, tTo, option, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WcfServiceDataTag.GetAlarmFileByScript2 Exception: " + ex.Message);
                return null;
            }
        }
    }
}
