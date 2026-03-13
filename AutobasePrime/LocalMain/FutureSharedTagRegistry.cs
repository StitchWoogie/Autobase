using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using LocalMain;
using GraphicModule;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for SharedTag.
	/// </summary>
	public class FutureSharedTagRegistry
    {
		public FutureSharedTagRegistry()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static SharedRegistryClass PrepareShareTagClass(string tag)
        {
            SharedRegistryClass share = new SharedRegistryClass();

            byte sum = SharedTag.GetSum8(tag);
            share.Init("TagShare", sum.ToString() + "\\" + tag);

            return share;
        }

        // CheckEngineTagChangeThread에서만 호출된다.
        public static bool SetCurrRegistryByThread(SharedRegistryClass share, double val)
        {
            share.Write("Value", val.ToString());
            return true;
        }

        // CheckEngineTagChangeThread에서만 호출된다.
        public static bool SetCurrRegistryByThread(SharedRegistryClass share, string val)
        {
            share.Write("Value", val);
            return true;
        }

        public static bool SetCurr(TagPublicClass tp, double val)
        {
            if (ConfigRunMain.bShareTagValueBySharedMemory) 
                CheckEngineTagChangeBySharedMemory.SetValue(tp, val);

            if (ScriptFunctionTag.bTagShareUpdateOption)
            {
                byte sum = SharedTag.GetSum8(tp.tag);
                TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tp.tag, "Value", val.ToString());
            }
            else
            {
                tp.bNeedSharedTagUpdate = true;
            }

            return true;
        }

        public static bool SetCurr(TagPublicClass tp, string val)
        {
            if (ConfigRunMain.bShareTagValueBySharedMemory) 
                CheckEngineTagChangeBySharedMemory.SetValue(tp, val);

            if (ScriptFunctionTag.bTagShareUpdateOption)
            {
                byte sum = SharedTag.GetSum8(tp.tag);
                TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tp.tag, "Value", val);
            }
            else
            {
                tp.bNeedSharedTagUpdate = true;
            }

            return true;
        }

        /*
        public static bool SetCurr(string tag, double val)
		{
			byte sum = SharedTag.GetSum8(tag);
			TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString()+"\\"+tag, "Value", val.ToString());
			return true;
		}

		public static bool SetCurr(string tag, string val)
		{
			byte sum = SharedTag.GetSum8(tag);
			TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString()+"\\"+tag, "Value", val);
			return true;
		}*/

		public static bool GetWriteitem(string tag, out double ival, out string sval, out string user, out string ip, out string computer)
		{
			byte sum = SharedTag.GetSum8(tag);

            user = "";
            ip = "";
            computer = "";

			if(TotalConfig.LoadRegAutoBaseConfig("TagShare", sum.ToString()+"\\"+tag, "bWrite", 0) == 0) 
			{
				ival = 0;
				sval = "";
				return false;
			}

			string buf = TotalConfig.LoadRegAutoBaseConfig("TagShare", sum.ToString()+"\\"+tag, "Write", "");

			NetTools.CommaTextReader comma = new NetTools.CommaTextReader();
			comma.Set(buf);
			uint seed1 = (uint)TotalConfig.LoadRegAutoBaseConfig("TagShare", null, "Code", 0);
			long seed2 = 0;
			comma.GetLong(ref seed2);
			uint code = SharedTag.MakeCode(seed1, (uint)seed2);
			uint readcode = 0;
			comma.GetDWORD(ref readcode);

			sval = "";
            comma.GetStringTotalRemain(ref sval);//comma.GetString(ref sval);
			ival = NetTools.ConvertTool.ToDouble(sval);

            // 출력한 사람의 정보도 가져온다. 2011-4-15
            buf = TotalConfig.LoadRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tag, "WriteInfo", "");
            comma.Set(buf);
            comma.GetString(ref user);
            comma.GetString(ref ip);
            comma.GetString(ref computer);

            bool retn = false;
            string result_msg = "";

            // 10.3.5.0에서 출력이 안되는 부분이 있어서 시간이 다른것 같아서 시간은 무시해서 컴파일 한다. 중국 시간으로 설정해서 인지는 확실하지 않음. 2020-10-28

            if (readcode == code)
            {
                DateTime t2 = DateTime.Now;
                DateTime t1 = t2.AddMinutes(-30);
                DateTime t_seed = new DateTime(seed2);

                if (seed2 >= t1.Ticks && seed2 <= t2.Ticks)	// 30분전에 실행한 명령은 실행하지 않는다.
                    result_msg = "";//retn = true;
                else
                    result_msg = String.Format("time mismatched t1={0}, seedt={1}, t2={2}", t1, t_seed, t2);

                retn = true;
            }
            else
            {
                result_msg = String.Format("code mismatched readcode={0}, code={1}, seed1={2}, seed2={3}, value={4}", readcode, code, seed1, seed2, sval);
            }

            TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tag, "Write", result_msg);
            TotalConfig.SaveRegAutoBaseConfig("TagShare", sum.ToString() + "\\" + tag, "bWrite", 0);

			return retn;
		}

        public static bool GetWriteitem(SharedRegistryClass share, out double ival, out string sval, out string user, out string ip, out string computer)
        {
            user = "";
            ip = "";
            computer = "";

            if (share.Read("bWrite", 0) == 0)
            {
                ival = 0;
                sval = "";
                return false;
            }

            string buf = share.Read("Write", "");

            NetTools.CommaTextReader comma = new NetTools.CommaTextReader();
            comma.Set(buf);
            uint seed1 = (uint)TotalConfig.LoadRegAutoBaseConfig("TagShare", null, "Code", 0);
            long seed2 = 0;
            comma.GetLong(ref seed2);
            uint code = SharedTag.MakeCode(seed1, (uint)seed2);
            uint readcode = 0;
            comma.GetDWORD(ref readcode);

            sval = "";
            comma.GetStringTotalRemain(ref sval);//comma.GetString(ref sval);
            ival = NetTools.ConvertTool.ToDouble(sval);

            // 출력한 사람의 정보도 가져온다. 2011-4-15
            buf = share.Read("WriteInfo", "");
            comma.Set(buf);
            comma.GetString(ref user);
            comma.GetString(ref ip);
            comma.GetString(ref computer);

            bool retn = false;
            string result_msg = "";

            // 10.3.5.0에서 출력이 안되는 부분이 있어서 시간이 다른것 같아서 시간은 무시해서 컴파일 한다. 중국 시간으로 설정해서 인지는 확실하지 않음.

            if (readcode == code)
            {
                DateTime t2 = DateTime.Now;
                DateTime t1 = t2.AddMinutes(-30);
                DateTime t_seed = new DateTime(seed2);

                if (seed2 >= t1.Ticks && seed2 <= t2.Ticks)	// 30분전에 실행한 명령은 실행하지 않는다.
                    result_msg = "";//retn = true;
                else
                    result_msg = String.Format("time mismatched t1={0}, seedt={1}, t2={2}", t1, t_seed, t2);

                retn = true;
            }
            else
            {
                result_msg = String.Format("code mismatched readcode={0}, code={1}, seed1={2}, seed2={3}, value={4}", readcode, code, seed1, seed2, sval);
            }

            share.Write("Write", result_msg);
            share.Write("bWrite", 0);

            return retn;
        }
	}

    
}
