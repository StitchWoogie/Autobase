using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using NetTools.OldDefine;
using System.Threading;
using System.IO;
using System.Diagnostics;
using ReportBasicLib;
using ScriptLibRun;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace GraphicModule
{
    /// <summary>
    /// Summary description for ScriptFunctionComboBox.
    /// </summary>
    public class ScriptFunctionExcel
    {
        /*
        public ScriptFunctionExcel()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static bool ExcelReportRun(ref string err)
        {
            string excelpath;
            excelpath = TotalConfig.LoadRegAutoBaseConfig("ExcelReport", null, "ExcelPath", "C:\\Program Files\\Microsoft Office\\Office\\EXCEL.EXE");

            if (!File.Exists(excelpath))
            {
                if (Tools.IsLangKorean())
                    err = String.Format("엑셀 실행 파일을 찾을 수 없습니다.\nPath={0}", excelpath);
                else
                    err = String.Format("Cannot find a Excel Program.\nPath={0}", excelpath);

                return false;
            }

            Process.Start(excelpath);       // 엑셀파일은 직접 호출하지 않고 엑셀 스크립에서 불러온다.

            return true;
        }

        static int Run_ExcelReportRun(ScriptClass scriptClass, out object val)
        {
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string err_msg = "";

                if (!ExcelReportRun(ref err_msg))
                {
                    scriptClass.ErrorMessage(err_msg);
                    return -1;
                }
            }

            return 1;
        }

        static int Function_ExcelReportRun(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();

            return Run_ExcelReportRun(scriptClass, out val);
        }

        static int nCountTempFile = 0;

        public static bool ExcelReportPrepare(ref string err, string path, string sheetname, int bPrint, int bClose, int year, int month, int day, int hour, int bSave)
        {
            string excelpath;
            excelpath = TotalConfig.LoadRegAutoBaseConfig("ExcelReport", null, "ExcelPath", "C:\\Program Files\\Microsoft Office\\Office\\EXCEL.EXE");

            if (!File.Exists(excelpath))
            {
                if (Tools.IsLangKorean())
                    err = String.Format("엑셀 실행 파일을 찾을 수 없습니다.\nPath={0}", excelpath);
                else
                    err = String.Format("Cannot find a Excel Program.\nPath={0}", excelpath);

                return false;
            }

            int first_count = nCountTempFile;
            string tempfile;

            Directory.CreateDirectory("C:\\ExcelReportTemp");

            while (true)
            {
                tempfile = String.Format("C:\\ExcelReportTemp\\AutoPrn{0:0000}.TXT", nCountTempFile);

                if (!File.Exists(tempfile))
                {
                    break;
                }
                else
                {
                    nCountTempFile++;
                    nCountTempFile %= 10000;
                    if (first_count == nCountTempFile) break;
                }
            }

            tempfile = String.Format("C:\\ExcelReportTemp\\AutoPrn{0:0000}.TXT", nCountTempFile);

            nCountTempFile++;
            nCountTempFile %= 10000;

            TextWriter writer = new StreamWriter(tempfile, false, System.Text.Encoding.Default);
            if (writer == null)
            {
                if (Tools.IsLangKorean())
                    err = String.Format("Temp 파일을 쓸수 없습니다.\nfilename={0}", tempfile);
                else
                    err = String.Format("Cannot write to Temp file.\nfilename={0}", tempfile);

                return false;
            }
            writer.Write("{0},", path);
            writer.Write("{0},", sheetname);
            writer.Write("{0},", (int)bPrint);
            writer.Write("{0},", (int)bClose);
            writer.Write("{0},", (int)year);
            writer.Write("{0},", (int)month);
            writer.Write("{0},", (int)day);
            writer.Write("{0},", (int)hour);
            writer.Write("{0},", (int)bSave);

            writer.Close();

            return true;
        }

        static int Run_ExcelReportPrepare(ScriptClass scriptClass, out object val, string path, string sheetname, int bPrint, int bClose, int year, int month, int day, int hour, int bSave)
        {
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string err_msg = "";

                if (!ExcelReportPrepare(ref err_msg, path, sheetname, (int)bPrint, (int)bClose, (int)year, (int)month, (int)day, (int)hour, bSave))
                {
                    scriptClass.ErrorMessage(err_msg);
                    return -1;
                }
            }

            return 1;
        }

        static int Function_ExcelReportPrepare(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string path;
            string sheetname;
            int bPrint;
            int bClose;
            int bSave;
            int year;
            int mon;
            int day;
            int hour;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out path)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out sheetname)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out bPrint)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out bClose)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out year)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out mon)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out day)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out hour)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out bSave)) return -1;

            return Run_ExcelReportPrepare(scriptClass, out val, path, sheetname, (int)bPrint, (int)bClose, (int)year, (int)mon, (int)day, (int)hour, bSave);
        }

        static int Function_ExcelReportRunDirect(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string path;
            string sheetname;
            int bPrint;
            int bClose;
            int bSave;
            int year;
            int mon;
            int day;
            int hour;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out path)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out sheetname)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out bPrint)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out bClose)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out year)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out mon)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out day)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out hour)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out bSave)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                DateTime t;

                try
                {
                    t = new DateTime(year, mon, day, hour, 0, 0);
                }
                catch
                {
                    t = DateTimeServer.Now;
                }

                if (AutoLibLocal.ConfigVarTotal.bLocalFlag)
                {
                    string err_msg = "";
                    string filename;

                    if (Path.IsPathRooted(path))
                        filename = path;
                    else
                        filename = String.Format("{0}\\Report\\{1}", TotalConfig.sDirWorkProject, path);

                    if (!ExcelReportPrepare(ref err_msg, filename, "", bPrint, bClose, year, mon, day, hour, bSave))
                    {
                        scriptClass.ErrorMessage(err_msg);
                        return -1;
                    }

                    if (!ExcelReportRun(ref err_msg))
                    {
                        scriptClass.ErrorMessage(err_msg);
                        return -1;
                    }
                }
                else
                {
                    string source_file;
                    string target_file;

                    localhost.ServiceExcelReport.ServiceExcelReport service = new GraphicModule.localhost.ServiceExcelReport.ServiceExcelReport();
                    // 10초가 너무적어서 30초로 고정 2009.5.28 다시 30초도 적어서 가변으로 바꿈 2009.6.10
                    service.Timeout = ConfigViewMain.nTimeoutOfExcelReportRunDirect * 1000;
                    service.CookieContainer = ConfigVarTotal.cookieContainer;
                    service.Url = AutoLibLocal.ConfigVarTotal.GetServicePath("ServiceExcelReport.asmx");

                    source_file = path;

                    try
                    {
                        target_file = service.GetReportStruct(source_file, t, t, ReportConfig.GetMinListTimeFr(), ReportConfig.GetMinListTimeTo(), 0);
                    }
                    catch (Exception exception)
                    {
                        scriptClass.ErrorMessage(exception.Message);
                        return -1;
                    }

                    string url = target_file;
                    System.Diagnostics.Process.Start("IExplore", url);
                }
            }

            return 1;
        }

        public static int Function_Excel(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (String.Compare(command, "ExcelReportRun") == 0)
            {
                return Function_ExcelReportRun(scriptClass, command, argument, out val);
            }
            else if (String.Compare(command, "ExcelReportPrepare") == 0)
            {
                return Function_ExcelReportPrepare(scriptClass, command, argument, out val);
            }
            if (String.Compare(command, "ExcelReportRunDirect") == 0)
            {
                return Function_ExcelReportRunDirect(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 Excel 함수입니다.\n({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 Excel 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined Excel function.\n({0})", command));
                }
                val = 0;
                return -1;
            }
        }

        
        public static int RunMethodExcel(ScriptClass script, string name, out object val, object[] args)
        {
            if (name == "ExcelReportRun")
            {
                return Run_ExcelReportRun(script, out val);
            }
            else if (name == "ExcelReportPrepare")
            {
                string path = ObjectValue.ToString(args[0]);
                string sheetname = ObjectValue.ToString(args[1]);
                int bPrint = ObjectValue.ToInt(args[2]);
                int bClose = ObjectValue.ToInt(args[3]);
                int year = ObjectValue.ToInt(args[4]);
                int mon = ObjectValue.ToInt(args[5]);
                int day = ObjectValue.ToInt(args[6]);
                int hour = ObjectValue.ToInt(args[7]);
                int bSave = ObjectValue.ToInt(args[8]);

                return Run_ExcelReportPrepare(script, out val, path, sheetname, (int)bPrint, (int)bClose, (int)year, (int)mon, (int)day, (int)hour, bSave);
            }
            else
            {
                script.ErrorMessage(String.Format("User Excution Engine {0} is not maked in the RunMethodExcel()", name));
                val = 0;
                return -1;
            }
        }
         */

        static int nCountTempFile = 0;

        public static bool ExcelReportPrepare(ref string err, string path, string sheetname, int bPrint, int bClose, int year, int month, int day, int hour, int bSave)
        {
            string excelpath;
            excelpath = TotalConfig.LoadRegAutoBaseConfig("ExcelReport", null, "ExcelPath", "C:\\Program Files\\Microsoft Office\\Office\\EXCEL.EXE");

            if (!File.Exists(excelpath))
            {
                if (Tools.IsLangKorean())
                    err = String.Format("엑셀 실행 파일을 찾을 수 없습니다.\nPath={0}", excelpath);
                else
                    err = String.Format("Cannot find a Excel Program.\nPath={0}", excelpath);

                return false;
            }

            int first_count = nCountTempFile;
            string tempfile;

            Directory.CreateDirectory("C:\\ExcelReportTemp");

            while (true)
            {
                tempfile = String.Format("C:\\ExcelReportTemp\\AutoPrn{0:0000}.TXT", nCountTempFile);

                if (!File.Exists(tempfile))
                {
                    break;
                }
                else
                {
                    nCountTempFile++;
                    nCountTempFile %= 10000;
                    if (first_count == nCountTempFile) break;
                }
            }

            tempfile = String.Format("C:\\ExcelReportTemp\\AutoPrn{0:0000}.TXT", nCountTempFile);

            nCountTempFile++;
            nCountTempFile %= 10000;

            TextWriter writer = new StreamWriter(tempfile, false, System.Text.Encoding.Default);
            if (writer == null)
            {
                if (Tools.IsLangKorean())
                    err = String.Format("Temp 파일을 쓸수 없습니다.\nfilename={0}", tempfile);
                else
                    err = String.Format("Cannot write to Temp file.\nfilename={0}", tempfile);

                return false;
            }
            writer.Write("{0},", path);
            writer.Write("{0},", sheetname);
            writer.Write("{0},", (int)bPrint);
            writer.Write("{0},", (int)bClose);
            writer.Write("{0},", (int)year);
            writer.Write("{0},", (int)month);
            writer.Write("{0},", (int)day);
            writer.Write("{0},", (int)hour);
            writer.Write("{0},", (int)bSave);

            writer.Close();

            return true;
        }

        static int Run_ExcelReportPrepare(ScriptClass scriptClass, out object val, string path, string sheetname, int bPrint, int bClose, int year, int month, int day, int hour, int bSave)
        {
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string err_msg = "";

                if (!ExcelReportPrepare(ref err_msg, path, sheetname, (int)bPrint, (int)bClose, (int)year, (int)month, (int)day, (int)hour, bSave))
                {
                    scriptClass.ErrorMessage(err_msg);
                    return -1;
                }
            }

            return 1;
        }

        static int Run_ExcelReportPrepare(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string path = (string)args[0];
            string sheetname = (string)args[1];
            int bPrint = (int)args[2];
            int bClose = (int)args[3];
            
            int year = (int)args[4];
            int mon = (int)args[5];
            int day = (int)args[6];
            int hour = (int)args[7];

            int bSave = (int)args[8];

            val = 0;

            return Run_ExcelReportPrepare(scriptClass, out val, path, sheetname, (int)bPrint, (int)bClose, (int)year, (int)mon, (int)day, (int)hour, bSave);
        }

        public static bool ExcelReportRun(ref string err)
        {
            string excelpath;
            excelpath = TotalConfig.LoadRegAutoBaseConfig("ExcelReport", null, "ExcelPath", "C:\\Program Files\\Microsoft Office\\Office\\EXCEL.EXE");

            if (!File.Exists(excelpath))
            {
                if (Tools.IsLangKorean())
                    err = String.Format("엑셀 실행 파일을 찾을 수 없습니다.\nPath={0}", excelpath);
                else
                    err = String.Format("Cannot find a Excel Program.\nPath={0}", excelpath);

                return false;
            }

            Process.Start(excelpath);       // 엑셀파일은 직접 호출하지 않고 엑셀 스크립에서 불러온다.

            return true;
        }

        static int Run_ExcelReportRun(ScriptClass scriptClass, out object val)
        {
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string err_msg = "";

                if (!ExcelReportRun(ref err_msg))
                {
                    scriptClass.ErrorMessage(err_msg);
                    return -1;
                }
            }

            return 1;
        }

        static int Run_ExcelReportRun(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            ScriptArgumentString arg = new ScriptArgumentString();

            return Run_ExcelReportRun(scriptClass, out val);
        }

        static int Run_ExcelReportRunDirect(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string path = (string)args[0];
            string sheetname = (string)args[1];
            int bPrint = (int)args[2];
            int bClose = (int)args[3];
            
            int year = (int)args[4];
            int mon = (int)args[5];
            int day = (int)args[6];
            int hour = (int)args[7];

            int bSave = (int)args[8];

            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                DateTime t;

                try
                {
                    t = new DateTime(year, mon, day, hour, 0, 0);
                }
                catch
                {
                    t = DateTimeServer.Now;
                }

                if (AutoLibLocal.ConfigVarTotal.bLocalFlag)
                {
                    string err_msg = "";
                    string filename;

                    if (Path.IsPathRooted(path))
                        filename = path;
                    else
                        filename = String.Format("{0}\\Report\\{1}", TotalConfig.sDirWorkProject, path);

                    if (!ExcelReportPrepare(ref err_msg, filename, "", bPrint, bClose, year, mon, day, hour, bSave))
                    {
                        scriptClass.ErrorMessage(err_msg);
                        return -1;
                    }

                    if (!ExcelReportRun(ref err_msg))
                    {
                        scriptClass.ErrorMessage(err_msg);
                        return -1;
                    }
                }
                else
                {
                    string source_file;
                    string target_file;

                    string[] keys;
                    string[] values;

                    ServiceLib.DicToString(out keys, out values);

                    if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 3, 2, 1))
                    {
                        source_file = path;

                        try
                        {
                            using (ServiceExcelWorkerClient service = new ServiceExcelWorkerClient(ConfigViewMain.nTimeoutOfExcelReportRunDirect))
                            {
                                ExcelWorkerJobInfo job = service.EnqueueWithDic(
                                    ServiceLib.StringToExcelReportString(keys),
                                    ServiceLib.StringToExcelReportString(values),
                                    source_file, t, t, ReportConfig.GetMinListTimeFr(), ReportConfig.GetMinListTimeTo(), 0);

                                if (job == null || String.IsNullOrEmpty(job.JobId))
                                {
                                    scriptClass.ErrorMessage("Excel Worker 작업 등록에 실패했습니다.");
                                    return -1;
                                }

                                ExcelWorkerJobInfo completed = service.WaitForCompleted(job.JobId, ConfigViewMain.nTimeoutOfExcelReportRunDirect, 2000);
                                target_file = completed.ResultUrl;
                            }
                        }
                        catch (Exception exception)
                        {
                            scriptClass.ErrorMessage(exception.Message);
                            return -1;
                        }
                    }
                    else
                    {
                        ServiceReferenceExcelReport.ServiceExcelReportClient service = new GraphicModule.ServiceReferenceExcelReport.ServiceExcelReportClient();
                        // 10초가 너무적어서 30초로 고정 2009.5.28 다시 30초도 적어서 가변으로 바꿈 2009.6.10
                        // service.Timeout = ConfigViewMain.nTimeoutOfExcelReportRunDirect * 1000;
                        // service.CookieContainer = ConfigVarTotal.cookieContainer;
                        // service.Url = AutoLibLocal.ConfigVarTotal.GetServicePath("ServiceExcelReport.asmx");
                        var binding = (BasicHttpBinding)service.Endpoint.Binding;
                        binding.SendTimeout = TimeSpan.FromMilliseconds( ConfigViewMain.nTimeoutOfExcelReportRunDirect * 1000);

                        source_file = path;

                        try
                        {
                            target_file = service.GetReportStruct(source_file, t, t, ReportConfig.GetMinListTimeFr(), ReportConfig.GetMinListTimeTo(), 0);
                        }
                        catch (Exception exception)
                        {
                            scriptClass.ErrorMessage(exception.Message);
                            return -1;
                        }
                    }

                    string url = target_file;
                    System.Diagnostics.Process.Start(url); // 2022-4-12 변경. Edge와 IExplore가 두개가 뜬다는 이야기가 있어서... //System.Diagnostics.Process.Start("IExplore", url);
                }
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            if (!TotalConfig.GetOemTypeRight(EnumOemTypeRight.ExcelReport)) return;  //20250113 PSU
            string prename = "Excel";

            prepare.AddMethod(prename, "ExcelReportPrepare", "void", new ScriptExternalRun.DeleMethod(Run_ExcelReportPrepare), "in:string:filename", "in:string:sheetname", "in:int:print", "in:int:close", "in:int:year", "in:int:month", "in:int:day", "in:int:hour", "in:int:save");
            prepare.AddMethod(prename, "ExcelReportRun", "void", new ScriptExternalRun.DeleMethod(Run_ExcelReportRun));
            prepare.AddMethod(prename, "ExcelReportRunDirect", "int", new ScriptExternalRun.DeleMethod(Run_ExcelReportRunDirect), "in:string:filename", "in:string:sheetname", "in:int:print", "in:int:close", "in:int:year", "in:int:month", "in:int:day", "in:int:hour", "in:int:save");
        }
    }
}

