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
using SilverlightScriptLibRun;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ScriptExternalRun
    {
        public static ScriptExternalRun scriptExternal = new ScriptExternalRun();

        public delegate int DeleMethod(ScriptClass script, string name, out object retn, object[] args);

        // 속도를 빨리하기 위해서 스크립트 그룹별로 나누었다.
        class LocalMethodGroup
        {
            public string sGroupName;
            public List<LocalMethod> arrayMethod = new List<LocalMethod>();
        }

        class LocalMethod
        {
            public string sMethodName;
            public string retn;
            public List<MethodArgument> args = null;
            public DeleMethod pMethod;
        }

        List<LocalMethodGroup> arrayGroup = new List<LocalMethodGroup>();

        public ScriptExternalRun()
        {
            //ScriptFunctionAnalog.PrepareMethod(this);
            //ScriptFunctionContextMenu.PrepareMethod(this);
            //ScriptFunctionDataGrid.PrepareMethod(this);
            ScriptFunctionGet.PrepareMethod(this);
            //ScriptFunctionMail.PrepareMethod(this);
            //ScriptFunctionMenu.PrepareMethod(this);
            //ScriptFunctionMessage.PrepareMethod(this);
            ScriptFunctionObject.PrepareMethod(this);
            //ScriptFunctionPrint.PrepareMethod(this);
            //ScriptFunctionSchedule.PrepareMethod(this);
            //ScriptFunctionScreen.PrepareMethod(this);
            //ScriptFunctionScript.PrepareMethod(this);
            //ScriptFunctionControlTabControl.PrepareMethod(this);

            ScriptFunctionDateTime.PrepareMethod(this);

            //ScriptFunctionElse.PrepareMethod(this);
        }

        LocalMethodGroup GetGroupPointer(string group_name)
        {
            for (int i = 0; i < arrayGroup.Count; i++)
            {
                if (group_name == arrayGroup[i].sGroupName) return arrayGroup[i];
            }

            LocalMethodGroup group = new LocalMethodGroup();
            group.sGroupName = group_name;
            arrayGroup.Add(group);

            return group;
        }

        public bool AddMethod(string group_name, string name, string retn, DeleMethod p, params string[] arg)
        {
            LocalMethodGroup group = GetGroupPointer(group_name);

            LocalMethod method = new LocalMethod();
            string buf;

            method.retn = retn;
            method.sMethodName = name;
            method.pMethod = p;

            CommaBlockString comma = new CommaBlockString();

            comma.SetBlockCode(':');

            if (arg.Length > 0)
            {
                method.args = new List<MethodArgument>();
                for (int i = 0; i < arg.Length; i++)
                {
                    MethodArgument ma = new MethodArgument();

                    comma.Set(arg[i]);
                    buf = comma.GetString();

                    if (buf == "in")
                        ma.eInOut = EnumInOut.In;
                    else if (buf == "out")
                        ma.eInOut = EnumInOut.Out;
                    else if (buf == "ref")
                        ma.eInOut = EnumInOut.Ref;
                    else
                    {
                        string msg = String.Format("inout type은 in/out/ref 중 하나이어야 합니다. MethodName={0}, arg={1}", name, arg[i]);
                        MessageBox.Show(msg, "ScriptExternalRun.AddMethod", MessageBoxButton.OK);
                        return false;
                    }

                    buf = comma.GetString();
                    ma.pVar.sVarType = buf;
                    if (buf == "int")
                        ma.pVar.eVarType = SilverlightScriptLibRun.EnumVarType.TypeInt;
                    else if (buf == "double")
                        ma.pVar.eVarType = SilverlightScriptLibRun.EnumVarType.TypeDouble;
                    else if (buf == "string")
                        ma.pVar.eVarType = SilverlightScriptLibRun.EnumVarType.TypeString;
                    else if (buf == "bool")
                        ma.pVar.eVarType = SilverlightScriptLibRun.EnumVarType.TypeBool;
                    else if (buf == "object")
                        ma.pVar.eVarType = SilverlightScriptLibRun.EnumVarType.TypeObject;
                    else
                    {
                        string msg = String.Format("알 수 없는 data type 입니다. MethodName={0}, arg={1}, data_type={2}", name, arg[i], buf);
                        MessageBox.Show(msg, "ScriptExternalRun.AddMethod", MessageBoxButton.OK);
                        return false;
                    }

                    if (comma.IsEOS())
                    {
                        string msg = String.Format("argument는 inout:datatype:name 으로 구성되어야 합니다. MethodName={0}, arg={1}", name, arg[i]);
                        MessageBox.Show(msg, "ScriptExternalRun.AddMethod", MessageBoxButton.OK);
                        return false;
                    }

                    ma.pVar.sVarName = comma.GetString();

                    method.args.Add(ma);
                }
            }

            group.arrayMethod.Add(method);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lm"></param>
        /// <returns></returns>
        string MakeMethodUsageAll(LocalMethod lm)
        {
            return lm.sMethodName + " " + MakeMethodUsageBody(lm);
        }

        string MakeMethodUsageBody(LocalMethod lm)
        {
            string buf;
            MethodArgument ma;

            buf = String.Format("{0}(", lm.sMethodName);

            if (lm.args != null)
            {
                for (int i = 0; i < lm.args.Count; i++)
                {
                    ma = lm.args[i];
                    buf += String.Format("{0} {1}", ma.pVar.sVarType, ma.pVar.sVarName);
                    if (i < lm.args.Count - 1)
                        buf += ", ";
                }
            }

            buf += ")";

            return buf;
        }

        /// <summary>
        /// 이전 스크립트에서 호출하는 함수
        /// </summary>
        /// <param name="script"></param>
        /// <param name="methodname"></param>
        /// <param name="retn"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        public int RunMethodFromOldScript(ScriptClass scriptClass, string methodname, out object retn_value, bool need_retn_value, string argument)
        {
            retn_value = 0;

            //string method = methodname.Substring(1);

            for (int i = 0; i < arrayGroup.Count; i++)
            {
                if (String.Compare(arrayGroup[i].sGroupName, 0, methodname, 0, arrayGroup[i].sGroupName.Length) == 0)
                {
                    LocalMethod lm;
                    for (int j = 0; j < arrayGroup[i].arrayMethod.Count; j++)
                    {
                        lm = arrayGroup[i].arrayMethod[j];

                        if (lm.sMethodName == methodname)
                        {
                            if (need_retn_value && lm.retn == "void")
                            {
                                string msg;
                                if (Tools.IsLangKorean())
                                {
                                    msg = String.Format("'{0}' Method는 return 값이 없습니다.", lm.sMethodName);
                                }
                                else
                                {
                                    msg = String.Format("{0} Method have not retrn value.", lm.sMethodName);
                                }
                                scriptClass.ErrorMessage(msg);
                                return -1;
                            }

                            object[] param = null;

                            if (lm.args != null && lm.args.Count > 0)
                            {
                                ScriptArgumentString arg = new ScriptArgumentString();
                                string buf;
                                object arg_value;
                                MethodArgument ma;

                                param = new object[lm.args.Count];

                                arg.Set(argument);

                                for (int k = 0; k < lm.args.Count; k++)
                                {
                                    ma = lm.args[k];

                                    arg.GetArgument(out buf);

                                    if (buf.Length == 0)
                                    {
                                        string msg = String.Format("{0}번째 인자'{2}'가 없습니다.\n{1}", k + 1, MakeMethodUsageAll(lm), ma.pVar.sVarName);
                                        scriptClass.ErrorMessage(msg);
                                        return -1;
                                    }

                                    if (ma.pVar.eVarType == SilverlightScriptLibRun.EnumVarType.TypeString)
                                    {
                                        string arg_value_string;
                                        if (!scriptClass.GetArgumentString(buf, out arg_value_string))
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return -1;
                                        }
                                        arg_value = arg_value_string;
                                    }
                                    else if (ma.pVar.eVarType == SilverlightScriptLibRun.EnumVarType.TypeBool)
                                    {
                                        int arg_value_int;
                                        if (!scriptClass.GetValueRecurse(buf, out arg_value_int))
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return -1;
                                        }

                                        arg_value = (arg_value_int == 1) ? true : false;
                                    }
                                    else if (ma.pVar.eVarType == SilverlightScriptLibRun.EnumVarType.TypeInt)
                                    {
                                        int arg_value_int;
                                        if (!scriptClass.GetValueRecurse(buf, out arg_value_int))
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return -1;
                                        }

                                        arg_value = arg_value_int;
                                    }
                                    else if (ma.pVar.eVarType == SilverlightScriptLibRun.EnumVarType.TypeDouble)
                                    {
                                        double arg_value_double;
                                        if (!scriptClass.GetValueRecurse(buf, out arg_value_double))
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return -1;
                                        }

                                        arg_value = arg_value_double;
                                    }
                                    else if (ma.pVar.eVarType == SilverlightScriptLibRun.EnumVarType.TypeObject)
                                    {
                                        object arg_value_obj;
                                        if (!scriptClass.GetValueRecurse(buf, out arg_value_obj))
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return -1;
                                        }

                                        arg_value = arg_value_obj;
                                    }
                                    else
                                    {
                                        scriptClass.ErrorMessage(String.Format("ScriptExternalRun.RunMethodFromOldScript()에서 지원되지 않는 데이터 형식입니다. DataType={0}", ma.pVar.eVarType));
                                        return -1;
                                    }

                                    param[k] = arg_value;
                                }

                                arg.GetArgument(out buf);
                                if (buf.Length > 0)
                                {
                                    string msg = String.Format("{0} Method 호출 시 너무 많은 인자를 사용했습니다.", MakeMethodUsageAll(lm));
                                    scriptClass.ErrorMessage(msg);
                                }
                            }

                            int retn = lm.pMethod(scriptClass, methodname, out retn_value, param);
                            if (retn == -1)
                            {
                                return -1;
                            }

                            // out 이나 ref 인자는 값을 바꾸어 준다.
                            if (lm.args != null && lm.args.Count > 0)
                            {
                                ScriptArgumentString arg = new ScriptArgumentString();
                                string buf;
                                MethodArgument ma;

                                arg.Set(argument);

                                for (int k = 0; k < lm.args.Count; k++)
                                {
                                    arg.GetArgument(out buf);

                                    ma = lm.args[k];

                                    if (ma.eInOut == EnumInOut.In) continue;    // 값을 변경할 필요가 없다.

                                    if (ma.pVar.eVarType == SilverlightScriptLibRun.EnumVarType.TypeString)
                                    {
                                        if (!scriptClass.ChangeStringVar(buf, scriptClass.GetValueString(param[k]))) return -1;
                                    }
                                    else
                                    {
                                        if (!scriptClass.ChangeNumberVar(buf, scriptClass.GetValueDouble(param[k]), buf)) return -1;
                                    }
                                }

                            }

                            return retn;
                        }
                    }


                    return 0;
                }
            }

            return 0;   // 아직 지원하지 않은 것이 있으므로 0으로 return하여 다음 함수를 검사하도록 한다.
        }

        /*
        public void FillMethodsToListView(ListView listview)
        {
            for (int i = 0; i < arrayGroup.Count; i++)
            {
                for (int j = 0; j < arrayGroup[i].arrayMethod.Count; j++)
                {
                    ListViewItem lvi = new ListViewItem(arrayGroup[i].arrayMethod[j].retn);
                    lvi.SubItems.Add(MakeMethodUsageBody(arrayGroup[i].arrayMethod[j]));
                    listview.Items.Add(lvi);
                }
            }
        }

        public override int IsExistVariable(string varname)
        {
            if (varname.Length == 0) return 0;
            if (varname[0] != '$') return 0;    // 해당없음

            string tag = varname.Substring(1);

            int[] tag_pos = null;
            EnumTagType tag_type = EnumTagType.none;

            if (TagLib.GetTagTypeAndPos(tag, ref tag_type, ref tag_pos))
            {
                return 1;
            }

            sErrorMessage = String.Format("{0} Tag not founded", varname);

            return 2;
        }

        public override int IsExistMethod(string methodname)
        {
            if (methodname.Length == 0) return 0;
            if (methodname[0] != '@') return 0;    // 해당 없음

            string method = methodname.Substring(1);

            for (int i = 0; i < arrayGroup.Count; i++)
            {
                for (int j = 0; j < arrayGroup[i].arrayMethod.Count; j++)
                {
                    if (arrayGroup[i].arrayMethod[j].sMethodName == method) return 1;
                }
            }

            sErrorMessage = String.Format("{0} Method not founded", methodname);

            return 2;
        }

        public override object RunVariable(ScriptRunConfiguration src, object pre_pointer, string varname, out object value)
        {
            if (pre_pointer != null)
            {
                TagPublicClass tp = (TagPublicClass)pre_pointer;

                value = tp.GetCurr();

                return pre_pointer;
            }
            else
            {
                if (varname == null || varname.Length == 0)
                {
                    sErrorMessage = "VarName is Empty";
                    value = null;
                    return null;
                }
                else if (varname[0] != '$')
                {
                    sErrorMessage = "VarName must start $ charactor";
                    value = null;
                    return null;
                }

                string tag = varname.Substring(1);

                int[] tag_pos = null;
                TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);

                value = tp.GetCurr();

                return tp;
            }
        }

        public override object RunMethod(ScriptRunConfiguration src, object pre_pointer, string methodname, out object retn, object[] param)
        {
            LocalMethod lm;
            string method = methodname.Substring(1);

            if (pre_pointer != null)
            {
                lm = (LocalMethod)pre_pointer;
            }
            else
            {


                for (int i = 0; i < arrayGroup.Count; i++)
                {
                    for (int j = 0; j < arrayGroup[i].arrayMethod.Count; j++)
                    {
                        lm = arrayGroup[i].arrayMethod[j];

                        if (lm.sMethodName == method)
                        {
                            goto ok_seeked;
                        }
                    }
                }

                retn = 0;

                sErrorMessage = String.Format("Method not exists {0}", methodname);

                return null;
            }

        ok_seeked: ;

            if (lm.args != null)
            {
                // 호출하는 변수값이 메소드의 원형과 같게 변수를 맞추어 준다.
                for (int k = 0; k < lm.args.Count; k++)
                {
                    if (lm.args[k].pVar.eVarType == ScriptLibRun.EnumVarType.TypeInt)
                        param[k] = ObjectValue.ToInt(param[k]);
                    else if (lm.args[k].pVar.eVarType == ScriptLibRun.EnumVarType.TypeDouble)
                        param[k] = ObjectValue.ToDouble(param[k]);
                    else if (lm.args[k].pVar.eVarType == ScriptLibRun.EnumVarType.TypeString)
                        param[k] = ObjectValue.ToString(param[k]);
                }
            }

            ScriptClass sc = (ScriptClass)src.externalConfig;

            int r = lm.pMethod(sc, method, out retn, param);

            if (r == -1)
            {
                sErrorMessage = sc.GetError();
                return null;
            }

            return lm;
        }

        public override object ChangeVariable(ScriptRunConfiguration src, object pre_pointer, string varname, object value)
        {
            if (pre_pointer != null)
            {
                TagPublicClass tp = (TagPublicClass)pre_pointer;
                ScriptClass sc = (ScriptClass)src.externalConfig;

                TagWrite.WriteCurr(tp, value, sc.bHandOperation, 0);

                return tp;
            }
            else
            {
                if (varname == null || varname.Length == 0)
                {
                    sErrorMessage = "VarName is Empty";
                    value = null;
                    return false;
                }
                else if (varname[0] != '$')
                {
                    sErrorMessage = "VarName must start $ charactor";
                    value = null;
                    return false;
                }

                string tag = varname.Substring(1);

                int[] tag_pos = null;
                TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);
                ScriptClass sc = (ScriptClass)src.externalConfig;

                TagWrite.WriteCurr(tp, value, sc.bHandOperation, 0);

                return tp;
            }
        }*/
    }
}
