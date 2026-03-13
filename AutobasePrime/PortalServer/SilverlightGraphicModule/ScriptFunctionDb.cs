using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using AutoLibLocal;
using AutoLib;
using NetTools;
using SilverlightAutoLibLocal;

namespace SilverlightGraphicModule
{
    public class ScriptFunctionDb
    {
        static List<DataSet> arrayDataSet = new List<DataSet>();
        static int nCountNewDataSet = 1;

        static int GetNewId()
        {
            string id_string;
            DataSet ds;
            int i;

            while (true)
            {
                nCountNewDataSet++;
                nCountNewDataSet %= 100;
                for (i = 0; i < arrayDataSet.Count; i++)
                {
                    ds = (DataSet)arrayDataSet[i];
                    id_string = String.Format("DataSet{0}", nCountNewDataSet);

                    if (ds.DataSetName == id_string)
                    {
                        goto next;
                    }
                }
                return nCountNewDataSet;
            next: ;

            }
        }

        static int Function_DbDsOpen(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string dsn;
            string text;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out dsn)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out text)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (arrayDataSet.Count >= 100)
                {
                    if (Tools.IsLangKorean())
                        scriptClass.ErrorMessage("DataSet은 동시에 100개 까지만 열 수 있습니다.");
                    else
                        scriptClass.ErrorMessage("Too many opened DataSet. Limit=100.");

                    val = -1;
                    return -1;
                }

                DataGate gate = new DataGate();
                string error;
                DataSet ds = gate.GetDataSetFromDsn(dsn, text, out error);

                if (ds == null)
                {
                    scriptClass.ErrorMessage(String.Format("Error:{0}", error));
                    val = -1;
                    return -1;
                }
                int new_id = GetNewId();
                ds.DataSetName = String.Format("DataSet{0}", new_id);
                arrayDataSet.Add(ds);
                val = new_id;
            }

            return 1;
        }

        static int Function_DbDsClose(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int id;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out id)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                DataSet ds;
                string id_string = String.Format("DataSet{0}", id);
                for (int i = 0; i < arrayDataSet.Count; i++)
                {
                    ds = (DataSet)arrayDataSet[i];

                    id_string = String.Format("DataSet{0}", id);

                    if (ds.DataSetName == id_string)
                    {
                        ds.Dispose();
                        arrayDataSet.RemoveAt(i);
                        break;
                    }
                }
            }

            return 1;
        }

        static DataSet SearchDataSet(int id)
        {
            DataSet ds;

            string id_string = String.Format("DataSet{0}", id);
            for (int i = 0; i < arrayDataSet.Count; i++)
            {
                ds = (DataSet)arrayDataSet[i];

                id_string = String.Format("DataSet{0}", id);

                if (ds.DataSetName == id_string)
                {
                    return ds;
                }
            }

            return null;
        }

        static int Function_DbDsGetRowData(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int id;
            string column;
            int row_pos;

            val = "";

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out id)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out column)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out row_pos)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                DataSet ds = SearchDataSet(id);
                if (ds == null)
                {
                    val = "";
                }
                else
                {
                    if (row_pos < 0 || row_pos >= ds.Tables[0].Rows.Count)
                        val = "";
                    else
                    {
                        DataRow row = ds.Tables[0].Rows[row_pos];
                        try
                        {
                            val = row[column].ToString();
                        }
                        catch (Exception exception)
                        {
                            if (Tools.IsLangKorean())
                                scriptClass.ErrorMessage(String.Format("DataSet GetValue에서 오류\nColumn:{0}\n오류내용:{1}", column, exception.Message));
                            else
                                scriptClass.ErrorMessage(String.Format("Error on DataSet GetValue method\nColumn:{0}\nError Message:{1}", column, exception.Message));

                            return -1;
                        }
                    }
                }

                return 1;
            }

            return 1;
        }

        static int Function_DbDsGetRowCount(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int id;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out id)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                DataSet ds = SearchDataSet(id);
                if (ds == null)
                {
                    val = 0;
                }
                else
                {
                    if (ds.Tables.Count > 0) val = ds.Tables[0].Rows.Count;
                    else val = 0;
                }

                return 1;
            }

            return 1;
        }

        static int Function_DbCommand(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string dsn;
            string text;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out dsn)) return -1;
            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out text)) return -1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                DataGate gate = new DataGate();
                string error;
                if (!gate.DataSetCommand(dsn, text, out error))
                {
                    scriptClass.ErrorMessage(String.Format("Error @DbCommand:{0}", error));
                    val = -1;
                    return -1;
                }
                val = 1;
            }

            return 1;
        }

        public static int Function_Db(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "DbDsOpen")
            {
                return Function_DbDsOpen(scriptClass, command, argument, out val);
            }
            else if (command == "DbDsClose")
            {
                return Function_DbDsClose(scriptClass, command, argument, out val);
            }
            else if (command == "DbDsGetRowData")
            {
                return Function_DbDsGetRowData(scriptClass, command, argument, out val);
            }
            else if (command == "DbDsGetRowCount")
            {
                return Function_DbDsGetRowCount(scriptClass, command, argument, out val);
            }
            else if (command == "DbCommand")
            {
                return Function_DbCommand(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 Db 함수입니다.\n({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 Db 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined Db??? function.\n({0})", command));
                }
                val = 0;
                return -1;
            }
        }
    }
}
