using System;
using System.Collections;
using AutoLibLocal;
using System.Net;
using NetTools;
using System.Xml.Linq;
using SilverlightAutoLibLocal.ServiceReferenceDataTag2;
using SilverlightAutoLibLocal;
using SilverlightAutoLibLocal.ServiceReferenceDataSet2;
using System.Security.Cryptography;

namespace AutoLib
{
	/// <summary>
	/// Summary description for DataGate.
	/// </summary>
	/// 

	public class WebCommInfo
	{
		public static int nFailCount = 0;
		public static string sErrorMessage="";

		public static void SetError(string err_string)
		{
			nFailCount++;
			sErrorMessage = err_string;
		}

		public static void Reset()
		{
			nFailCount = 0;
			sErrorMessage = "";
		}
	}
	
	public class DataGate
	{
		
		public DataGate()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        /*
		public static void CheckServiceAlive()
		{
			localhost.ServiceDataTag.ServiceDataTag data = new ServiceDataTag();
			data.Timeout = 5000;
			
			try 
			{
				int retn = data.CheckServiceAlive(500);
				WebCommInfo.Reset();
			}
			catch
			{
				
			}
		}

		public DataSet GetTagValueList(ArrayList array)
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				return null;
			}
			else 
			{
				ServiceDataTag data = new ServiceDataTag();
				data.Timeout = 5000;
				DataSet ds;

				try 
				{
					ds = data.GetTagValueList(array.ToArray());
				}
				catch (Exception ex)
				{
					
					ds = null;
					WebCommInfo.SetError("DataGate.cs GetTagValueList()\n"+ex.Message);
				}

				return ds;
			}
		}*/

        public static System.ServiceModel.EndpointAddress GetEndPoint(string service_name)
        {
            string url = AutoLibLocal.ConfigVarTotal.GetServicePath(service_name);
            return new System.ServiceModel.EndpointAddress(url);
        }

        byte[] StringToBytes(string buf)
        {
            byte[] b = new byte[buf.Length * 2];

            for (int i = 0; i < buf.Length; i++)
            {
                b[i * 2 + 0] = (byte)(buf[i] / 256);
                b[i * 2 + 1] = (byte)(buf[i] % 256);
            }

            return b;
        }

		// 수동 출력일 때만 원격으로 보내야 한다.
        public void WriteCurrDouble(string tag, TagPublicClass tp, double val, bool bHandOperation)
        {
            if (tp.bLocalTag == 1) return;	// local tag이므로 네트워크를 통해서 출력되지 않는다.
            if (!bHandOperation) return;	// 수동출력만 원격 출력 허용

            WebServiceDataTag2SoapClient service = new WebServiceDataTag2SoapClient(new System.ServiceModel.BasicHttpBinding(), GetEndPoint("WebServiceDataTag2.asmx"));

            string computername = "Silverlight";

            byte[] hash_s = StringToBytes("WriteCurr" + SharedData.userInfo.sUsername + computername + tag + val.ToString());

            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1Managed();

            byte[] result = sha.ComputeHash(hash_s);

            service.WriteCurrAsync(SharedData.userInfo.sUsername, computername, tag, val.ToString(), result);
            //service.WriteCurrAIAsync(tag, val);
        }

		// 수동 출력일 때만 원격으로 보내야 한다.
		public void WriteCurrString(string tag, TagPublicClass tp, string val, bool bHandOperation)
		{
            if (tp.bLocalTag == 1) return;	// local tag이므로 네트워크를 통해서 출력되지 않는다.
            if (!bHandOperation) return;	// 수동출력만 원격 출력 허용

            WebServiceDataTag2SoapClient service = new WebServiceDataTag2SoapClient(new System.ServiceModel.BasicHttpBinding(), GetEndPoint("WebServiceDataTag2.asmx"));

            string computername = "Silverlight";

            byte[] hash_s = StringToBytes("WriteCurr" + SharedData.userInfo.sUsername + computername + tag + val);

            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1Managed();

            byte[] result = sha.ComputeHash(hash_s);

            service.WriteCurrAsync(SharedData.userInfo.sUsername, computername, tag, val, result);

            //service.WriteCurrSTAsync(tag, val);
		}
        
        
		// 수동 출력일 때만 원격으로 보내야 한다. 숫자 문자열에 상관없이 출력하고 지연시간도 있다.
		public void WriteCurr(TagPublicClass tp, object val, bool bHandOperation, int delay_sec)
		{
            if (tp.bLocalTag == 1) return;	// local tag이므로 네트워크를 통해서 출력되지 않는다.
            if (!bHandOperation) return;	// 수동출력만 원격 출력 허용

                if (val.GetType() == typeof(string))
                    WriteCurrString(tp.tag, tp, (string)val, bHandOperation);
                else
                    WriteCurrDouble(tp.tag, tp, ObjectValue.ToDouble(val), bHandOperation);
		}
        /*
		public DataSet GetDataAi(string tag, EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				DataLocal data = new DataLocal();
				return data.GetDataAi(tag, data_type, data_time, year, mon, day, hour, min, data_count, data_gab);
			}
			else 
			{
				ServiceDataTag data = new ServiceDataTag();
				DataSet ds;
				try 
				{
					ds = data.GetDataAi(tag, (int)data_type, (int)data_time, year, mon, day, hour, min, data_count, data_gab);
				}
				catch (Exception ex)
				{
					ds = null;
					WebCommInfo.SetError(ex.Message);
				}
				return ds;
			}
		}

		public DataSet GetDataDi(string tag, EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				DataLocal data = new DataLocal();
				return data.GetDataDi(tag, data_type, data_time, year, mon, day, hour, min, data_count, data_gab);
			}
			else 
			{
				ServiceDataTag data = new ServiceDataTag();
				DataSet ds;

				try 
				{
					ds = data.GetDataDi(tag, (int)data_type, (int)data_time, year, mon, day, hour, min, data_count, data_gab);
				}
				catch (Exception ex)
				{
					ds = null;
					WebCommInfo.SetError(ex.Message);
				}

				return ds;

			}
		}  
        
		public bool CheckUserName(string username, string passcode, out string err_msg)
		{
			if(NetTools.Tools.IsLangKorean())
				err_msg = "사용자 이름이 존재하지 않거나 암호가 틀립니다.";
			else if(NetTools.Tools.IsLangChinese())
				err_msg = "用户名不存在或密码不正确。";
			else
				err_msg = "Invalid Username or Password.";

			if(ConfigVarTotal.bLocalFlag) 
			{
				string supervisor = TotalConfig.AutoBaseIniGetOemSupervisorName();
				if(String.Compare(supervisor, username, true) == 0) 
				{
					uint pass = TotalConfig.LoadRegAutoBaseConfig("Supervisor", null, "PassCode", AutoLibLocal.UserInfoStruct.ZipPassword(username, username));
					if(passcode == pass.ToString())
						return true;
					else 
					{
						return false;
					}
				}
				DataLocal data = new DataLocal();
				return data.CheckUserName(out err_msg, username, passcode);
			}
			else 
			{
				ServiceUserProtect data = new ServiceUserProtect();
				data.Timeout = 10000;	// 서버에서 키락 체크 시 시간이 많이 걸린다.
				bool retn;
				try 
				{
					retn = data.CheckUserName(username, passcode, out err_msg);
				}
				catch (Exception ex)
				{
					retn = false;
					WebCommInfo.SetError(ex.Message);
				}
				return retn;
			}
		}

		public int GetUserCount()
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				return 0;
			}
			else 
			{
				ServiceUserProtect data = new ServiceUserProtect();
				data.Timeout = 5000;
				int retn;
				try 
				{
					retn = data.GetUserCount();
				}
				catch (Exception ex)
				{
					retn = 0;
					WebCommInfo.SetError(ex.Message);
				}
				return retn;
			}
		}
 
		public bool DefaultUserCheck(out string err_msg, out string username)
		{
			username = "";

			if(ConfigVarTotal.bLocalFlag) 
			{
				DataLocal data = new DataLocal();
				return data.DefaultUserCheck(out err_msg, out username);
			}
			else 
			{
				ServiceUserProtect data = new ServiceUserProtect();
				data.Timeout = 10000;	// 서버에서 키락 체크시 시간이 많이 걸린다.

                if (String.Compare(data.Url, 0, "http://www.webtest.autobase.biz", 0, 31) == 0)
                {
                    err_msg = "Web References의 URL이 http://www.webtest.autobase.biz로 초기화 되었습니다.\nthis.Url = AutoLibLocal.ConfigVarTotal.GetServicePath(\"????.asmx\") 로 바꾸어 주어야 합니다.";
                    System.Windows.Forms.MessageBox.Show(err_msg);
                    return false; 
                }

				bool retn;
				try 
				{
					retn = data.DefaultUserCheck(out username, out err_msg);
				}
				catch (Exception ex)
				{
					retn = false;
					err_msg = ex.Message;
					WebCommInfo.SetError(ex.Message);
				}

				return retn;
			}
		}

		public void LogOut()
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				// local은 정보를 저장하지 않는다.
			}
			else 
			{
				ServiceUserProtect data = new ServiceUserProtect();
				data.Timeout = 5000;
				try 
				{
					data.LogOut();
				}
				catch (Exception ex)
				{
					WebCommInfo.SetError(ex.Message);
				}
			}
		}

		public DataSet GetLogLists()
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				DataLocal data = new DataLocal();
				return data.GetLogLists();
			}
			else 
			{
				ServiceDataTag data = new ServiceDataTag();
				data.Timeout = 10000;
				DataSet ds;
				try 
				{
					ds = data.GetLogLists();
				}
				catch (Exception ex)
				{
					ds = null;
					WebCommInfo.SetError(ex.Message);
				}

				return ds;
			}
		}

		public DataSet GetLogFile(string log_name)
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				DataLocal data = new DataLocal();
				return data.GetLogFile(log_name);
			}
			else 
			{
				ServiceDataTag data = new ServiceDataTag();
				DataSet ds;
				try 
				{
					ds = data.GetLogFile(log_name);
				}
				catch (Exception ex)
				{
					ds = null;
					WebCommInfo.SetError(ex.Message);
				}

				return ds;
			}
		}

		public DataSet GetAlarmLists()
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				DataLocal data = new DataLocal();
				return data.GetAlarmLists();
			}
			else 
			{
				ServiceDataTag data = new ServiceDataTag();
				data.Timeout = 10000;
				DataSet ds;
				try 
				{
					ds = data.GetAlarmLists();
				}
				catch (Exception ex)
				{
					ds = null;
					WebCommInfo.SetError(ex.Message);
				}

				return ds;
			}
		}

		public DataSet GetAlarmFile(string alarm_file)
		{
			if(ConfigVarTotal.bLocalFlag) 
			{
				DataLocal data = new DataLocal();
				return data.GetAlarmFile(alarm_file);
			}
			else 
			{
				ServiceDataTag data = new ServiceDataTag();
				DataSet ds;
				try 
				{
					return data.GetAlarmFile(alarm_file);
				}
				catch (Exception ex)
				{
					ds = null;
					WebCommInfo.SetError(ex.Message);
				}

				return ds;
			}
		}

        public DataSet GetAlarmFileByScript(DateTime tFrom, DateTime tTo, string option)
        {
            if (ConfigVarTotal.bLocalFlag)
            {
                DataLocal data = new DataLocal();
                return data.GetAlarmFileByScript(tFrom, tTo, option);
            }
            else
            {
                ServiceDataTag data = new ServiceDataTag();
                DataSet ds = null;

                try
                {
                    return data.GetAlarmFileByScript2(tFrom, tTo, option);  // 마지막 옵션은 사용하지 않는다. 9.5.2 부터 사용
                }
                catch (Exception ex)
                {
                    ds = null;
                    WebCommInfo.SetError(ex.Message);
                }


                return ds;
            }
        }

        public void GetDataSetFromMdb(string filename, string command, System.ComponentModel.AsyncCompletedEventArgs completedargs)
		{
            WebServiceDataSet2SoapClient service = new WebServiceDataSet2SoapClient(new System.ServiceModel.BasicHttpBinding(), GetEndPoint("WebServiceDataSet2.asmx"));

            service.GetDataSetFromMdbCompleted += (GetDataSetFromMdbCompletedEventArgs)completedargs;// new EventHandler<GetDataSetFromMdbCompletedEventArgs>(service_GetDataSetFromMdbCompleted);
            service.GetDataSetFromMdbAsync(filename, command); 
		}

        void service_GetDataSetFromMdbCompleted(object sender, GetDataSetFromMdbCompletedEventArgs e)
        {
            
        }*/

        public DataSet GetDataSetFromDsn(string dsn, string command, out string error)
		{
			error = "";
        
            SilverlightAutoLibLocal.ServiceReferenceDataSet2.WebServiceDataSet2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataSet2();

            dsResult = new DataSet();
            
            service.GetDataSetFromDsnCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetDataSetFromDsnCompletedEventArgs>(service_GetDataSetFromDsnCompleted);
            service.GetDataSetFromDsnAsync(dsn, command);

            return dsResult;
        }

        DataSet dsResult = null;

        void service_GetDataSetFromDsnCompleted(object sender, SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetDataSetFromDsnCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true)
            {
                return;
            }

            if (e.Result == null)
            {
                return;
            }

            DataSet ds = dsResult;

            ds.GetDataFromString(e.Result);
        }
        
		public bool DataSetCommand(string dsn, string command, out string error)
		{
			error = "";

            SilverlightAutoLibLocal.ServiceReferenceDataSet2.WebServiceDataSet2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataSet2();

            service.DataSetCommandAsync(dsn, command);

            return true;
		}

        /*
		public bool GetConnectionStringDbType(string dsn, out EnumDbType dbtype)
		{
			if(ConfigVarTotal.bLocalFlag) 
				return DbTool.GetConnectionStringDbType(dsn, out dbtype);
			else 
			{
				ServiceDataSet data = new ServiceDataSet();
				try 
				{
					int type;
					bool retn = data.GetConnectionStringDbType(dsn, out type);
					dbtype = (EnumDbType)type;
					return retn;
				}
				catch (Exception ex)
				{
					WebCommInfo.SetError(ex.Message);
					dbtype = EnumDbType.Normal;
					return false;
				}
			}
		}

		public DataSet GetDataSetFromOdbc(string dsn, string command, out string error)
		{
			error = "";
			if(ConfigVarTotal.bLocalFlag) 
			{
				DataLocal data = new DataLocal();
				return data.GetDataSetFromOdbc(dsn, command, out error);
			}
			else 
			{
				ServiceOdbc data = new ServiceOdbc();
				DataSet ds;
				try 
				{
					return data.GetDataSetFromOdbc(dsn, command, out error);
				}
				catch (Exception ex)
				{
					ds = null;
					WebCommInfo.SetError(ex.Message);
					error = ex.Message;
				}

				return ds;
			}
		}

		public bool DataSetOdbcCommand(string dsn, string command, out string error)
		{
			error = "";
			if(ConfigVarTotal.bLocalFlag) 
			{
				DataLocal data = new DataLocal();
				return data.DataSetOdbcCommand(dsn, command, out error);
			}
			else 
			{
				ServiceOdbc data = new ServiceOdbc();
				try 
				{
					return data.DataSetOdbcCommand(dsn, command, out error);
				}
				catch (Exception ex)
				{
					WebCommInfo.SetError(ex.Message);
					error = ex.Message;
				}

				return false;
			}
		}*/
	}
}
