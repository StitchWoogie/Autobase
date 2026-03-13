using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using AutoLibLocal;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// Summary description for ServiceDataSet
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ServiceDataSet : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet GetDataSetFromMdb(string filename, string command, out string error)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.GetDataSetFromMdb(filename, command, out error);
        }

        [WebMethod]
        public DataSet GetDataSetFromDsn(string dsn, string command, out string error)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.GetDataSetFromDsn(dsn, command, out error);
        }

        [WebMethod]
        public bool DataSetCommand(string dsn, string command, out string error)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.DataSetCommand(dsn, command, out error);
        }

        [WebMethod]
        public bool GetConnectionStringDbType(string dsn, out int dbtype)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            TotalConfig.sDirWorkProject = work_dir;
            EnumDbType type;
            bool retn = DbTool.GetConnectionStringDbType(dsn, out type);
            dbtype = (int)type;
            return retn;
        }
    }
}
