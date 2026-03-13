using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;
using ScriptLibRun;
using NetTools;
using System.Windows.Forms;
using AutoLib;
using System.Threading.Tasks;

namespace GraphicModule
{
    public class ScriptExternalRun : ScriptLibRun.ScriptExternalClass
    {
        public static ScriptExternalRun scriptExternal = new ScriptExternalRun();

        public delegate int DeleMethod(ScriptClass script, string name, out object retn, object[] args);

        public delegate Task<(int, object)> AsyncDeleMethod(ScriptClass scriptClass, string method_name, object[] args); //20250723 PSU async 추가.

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
            public AsyncDeleMethod pAsyncMethod; // 20250723 PSU 새로 추가
        }

        List<LocalMethodGroup> arrayGroup = new List<LocalMethodGroup>();

        public ScriptExternalRun()
        {
            ScriptFunctionAnalog.PrepareMethod(this);
            ScriptFunctionContextMenu.PrepareMethod(this);
            ScriptFunctionDataGrid.PrepareMethod(this);
            ScriptFunctionGet.PrepareMethod(this);
            ScriptFunctionMail.PrepareMethod(this);
            ScriptFunctionMenu.PrepareMethod(this);
            ScriptFunctionMessage.PrepareMethod(this);
            ScriptFunctionObject.PrepareMethod(this);
            ScriptFunctionPrint.PrepareMethod(this);
            ScriptFunctionReg.PrepareMethod(this);
            ScriptFunctionSchedule.PrepareMethod(this);
            ScriptFunctionScreen.PrepareMethod(this);
            ScriptFunctionScript.PrepareMethod(this);
            ScriptFunctionString.PrepareMethod(this);
            ScriptFunctionControlTabControl.PrepareMethod(this);
            
            ScriptFunctionReport.PrepareMethod(this);
            ScriptFunctionTime.PrepareMethod(this);
            ScriptFunctionMultiGraph.PrepareMethod(this);
            ScriptFunctionMultiTrend.PrepareMethod(this);
            ScriptFunctionChart.PrepareMethod(this);    //25-02-24 Chart 컨트롤 스크립트 등록

            ScriptFunctionAlarm.PrepareMethod(this);
            ScriptFunctionFile.PrepareMethod(this);
            ScriptFunctionFolder.PrepareMethod(this);
            ScriptFunctionAts.PrepareMethod(this);
            ScriptFunctionDateTime.PrepareMethod(this);
            ScriptFunctionSignage.PrepareMethod(this);
            ScriptFunctionControlEditBox.PrepareMethod(this);
            ScriptFunctionSet.PrepareMethod(this);
            ScriptFunctionDbTrend.PrepareMethod(this);   //이것이 Db보다는 앞에 와야 할 듯
            ScriptFunctionDb.PrepareMethod(this);
            ScriptFunctionAnimation.PrepareMethod(this);
            ScriptFunctionBitmap.PrepareMethod(this);
            ScriptFunctionCircle.PrepareMethod(this);
            ScriptFunctionCommandLine.PrepareMethod(this);
            ScriptFunctionControlCheckBox.PrepareMethod(this);
            ScriptFunctionControlComboBox.PrepareMethod(this);
            ScriptFunctionControlDatePicker.PrepareMethod(this);
            ScriptFunctionControlListBox.PrepareMethod(this);
            ScriptFunctionControlRadioButton.PrepareMethod(this);
            ScriptFunctionControlTree.PrepareMethod(this);
            ScriptFunctionCsv.PrepareMethod(this);
            ScriptFunctionDatabase.PrepareMethod(this); // ScriptFunctionData 보다는 먼저와야 한다.
            ScriptFunctionDataTable.PrepareMethod(this); 
            ScriptFunctionData.PrepareMethod(this);
            //ScriptFunctionDbTrend.PrepareMethod(this);
            ScriptFunctionDde.PrepareMethod(this);
            ScriptFunctionDialog.PrepareMethod(this);
            ScriptFunctionExcel.PrepareMethod(this);
            ScriptFunctionGlobal.PrepareMethod(this);
            ScriptFunctionImage.PrepareMethod(this);
            ScriptFunctionKey.PrepareMethod(this);
            ScriptFunctionLang.PrepareMethod(this); // 251031 PSU 추가
            ScriptFunctionLog.PrepareMethod(this);
            ScriptFunctionMath.PrepareMethod(this);
            ScriptFunctionMdi.PrepareMethod(this);
            ScriptFunctionMilliData.PrepareMethod(this);
            ScriptFunctionMilliTrend.PrepareMethod(this);
            ScriptFunctionModule.PrepareMethod(this);
            ScriptFunctionMouse.PrepareMethod(this);
            ScriptFunctionPlcScan.PrepareMethod(this);
            ScriptFunctionProcess.PrepareMethod(this);
            ScriptFunctionRealTimeTestGraph.PrepareMethod(this);
            ScriptFunctionSms.PrepareMethod(this);
            ScriptFunctionSql.PrepareMethod(this);
            ScriptFunctionSystem.PrepareMethod(this);
            ScriptFunctionSVG.PrepareMethod(this);  //SVG 20241024 PSU
            ScriptFunctionTag.PrepareMethod(this);
            ScriptFunctionToolBar.PrepareMethod(this);
            
            ScriptFunctionWeb.PrepareMethod(this);
            ScriptFunctionVLC.PrepareMethod(this); //VLCAx 202406026
            ScriptFunctionHttp.PrepareMethod(this); //20250204 PSU 추가
            ScriptFunctionJson.PrepareMethod(this); //20250204 PSU 추가
            ScriptFunctionPythonAi.PrepareMethod(this); //20260309 PSU Python AI Engine


            ScriptFunctionXYGraph.PrepareMethod(this);

            ScriptFunctionProxy.PrepareMethod(this);
            ScriptFunctionRecipe.PrepareMethod(this);
            ScriptFunctionPreset.PrepareMethod(this);
            ScriptFunctionBarcode.PrepareMethod(this);

            ScriptFunction_aL.PrepareMethod(this);
            ScriptFunction_AU.PrepareMethod(this);
            ScriptFunction_cL.PrepareMethod(this);
            ScriptFunction_CU.PrepareMethod(this);
            ScriptFunction_DU.PrepareMethod(this);
            ScriptFunction_eL.PrepareMethod(this);
            ScriptFunction_IU.PrepareMethod(this);
            ScriptFunction_lL.PrepareMethod(this);
            ScriptFunction_LU.PrepareMethod(this);
            ScriptFunction_MU.PrepareMethod(this);
            ScriptFunction_OU.PrepareMethod(this);
            ScriptFunction_pL.PrepareMethod(this);
            ScriptFunction_PU.PrepareMethod(this);
            ScriptFunction_rL.PrepareMethod(this);
            ScriptFunction_RU.PrepareMethod(this);
            ScriptFunction_sL.PrepareMethod(this);
            ScriptFunction_SU.PrepareMethod(this);
            ScriptFunction_tL.PrepareMethod(this);
            ScriptFunction_TU.PrepareMethod(this);
            ScriptFunction_VU.PrepareMethod(this);
            ScriptFunction_WU.PrepareMethod(this);
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

        // async 메서드용 오버로드 추가
        public bool AddMethod(string group_name, string name, string retn, AsyncDeleMethod asyncP, params string[] arg)
        {
            return AddMethodInternal(group_name, name, retn, null, asyncP, arg);
        }


        public bool AddMethodInternal(string group_name, string name, string retn, DeleMethod p, AsyncDeleMethod asyncP, params string[] arg)
        {
            LocalMethodGroup group = GetGroupPointer(group_name);
            LocalMethod method = new LocalMethod();
            string buf;
            method.retn = retn;
            method.sMethodName = name;
            method.pMethod = p;
            method.pAsyncMethod = asyncP; // 새 필드 추가 필요

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
                    else if (buf == "params")
                        ma.eInOut = EnumInOut.Params;
                    else if (buf == "out params")           // GetVarValue(string var_name, string value), int GetVarValue(string var_name) 의 두가지가 있으므로 out 도 params로 정했다. 이 Type은 구형 스크립트에서만 있는 형식
                        ma.eInOut = EnumInOut.OutParams;
                    else
                    {
                        string msg = String.Format("inout type은 in/out/ref 중 하나이어야 합니다. MethodName={0}, arg={1}", name, arg[i]);
                        MessageBox.Show(msg, "ScriptExternalRun.AddMethod");
                        return false;
                    }

                    buf = comma.GetString();
                    ma.pVar.sVarType = buf;
                    if (buf == "int")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeInt;
                    else if (buf == "float")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeFloat;
                    else if (buf == "double")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeDouble;
                    else if (buf == "string")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeString;
                    else if (buf == "bool")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeBool;
                    else if (buf == "object")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeObject;
                    else if (buf == "byte[]")
                    {
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeByte;
                        ma.pVar.nDimensionals = new int[1];
                        ma.pVar.nDimensionals[0] = 1;   // []는 1 [,] = 2
                    }
                    else if (buf == "object[]")
                    {
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeObject;
                        ma.pVar.nDimensionals = new int[1];
                        ma.pVar.nDimensionals[0] = 1;   // []는 1 [,] = 2
                    }
                    else
                    {
                        string msg = String.Format("알 수 없는 data type 입니다. MethodName={0}, arg={1}, data_type={2}", name, arg[i], buf);
                        MessageBox.Show(msg, "ScriptExternalRun.AddMethod");
                        return false;
                    }

                    if (comma.IsEOS())
                    {
                        string msg = String.Format("argument는 inout:datatype:name 으로 구성되어야 합니다. MethodName={0}, arg={1}", name, arg[i]);
                        MessageBox.Show(msg, "ScriptExternalRun.AddMethod");
                        return false;
                    }
                    ma.pVar.sVarName = comma.GetString();
                    method.args.Add(ma);
                }
            }
            comma.Clear();
            comma = null;
            group.arrayMethod.Add(method);

            return true;
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
                    
                    if(buf == "in") 
                        ma.eInOut = EnumInOut.In;
                    else if (buf == "out")
                        ma.eInOut = EnumInOut.Out;
                    else if (buf == "ref")
                        ma.eInOut = EnumInOut.Ref;
                    else if (buf == "params")
                        ma.eInOut = EnumInOut.Params;
                    else if (buf == "out params")           // GetVarValue(string var_name, string value), int GetVarValue(string var_name) 의 두가지가 있으므로 out 도 params로 정했다. 이 Type은 구형 스크립트에서만 있는 형식
                        ma.eInOut = EnumInOut.OutParams;
                    else
                    {
                        string msg = String.Format("inout type은 in/out/ref 중 하나이어야 합니다. MethodName={0}, arg={1}", name, arg[i]);
                        MessageBox.Show(msg, "ScriptExternalRun.AddMethod");
                        return false;
                    }

                    buf = comma.GetString();
                    ma.pVar.sVarType = buf;
                    if (buf == "int")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeInt;
                    else if (buf == "float")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeFloat;
                    else if(buf == "double")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeDouble;
                    else if (buf == "string")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeString;
                    else if (buf == "bool")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeBool;
                    else if (buf == "object")
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeObject;
                    else if (buf == "byte[]")
                    {
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeByte;
                        ma.pVar.nDimensionals = new int[1];
                        ma.pVar.nDimensionals[0] = 1;   // []는 1 [,] = 2
                    }
                    else if (buf == "object[]")
                    {
                        ma.pVar.eVarType = ScriptLibRun.EnumVarType.TypeObject;
                        ma.pVar.nDimensionals = new int[1];
                        ma.pVar.nDimensionals[0] = 1;   // []는 1 [,] = 2
                    }
                    else
                    {
                        string msg = String.Format("알 수 없는 data type 입니다. MethodName={0}, arg={1}, data_type={2}", name, arg[i], buf);
                        MessageBox.Show(msg, "ScriptExternalRun.AddMethod");
                        return false;
                    }

                    if (comma.IsEOS())
                    {
                        string msg = String.Format("argument는 inout:datatype:name 으로 구성되어야 합니다. MethodName={0}, arg={1}", name, arg[i]);
                        MessageBox.Show(msg, "ScriptExternalRun.AddMethod");
                        return false;
                    }

                    ma.pVar.sVarName = comma.GetString();
                    
                    method.args.Add(ma);
                }
            }

            comma.Clear();
            comma = null;
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
            //return lm.sMethodName+" "+MakeMethodUsageBody(lm);
            return MakeMethodUsageBody(lm);
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
        public async Task<(int, object retn_value)> RunMethodFromOldScript(ScriptClass scriptClass, string methodname, bool need_retn_value, string argument)
        {
            object retn_value = 0;

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
                                return (-1, retn_value);
                            }

                            // 이것을 array로 변경해야 한다.
                            //object[] param = null;
                            List<object> array_param = new List<object>();

                            if (lm.args != null && lm.args.Count > 0)
                            {
                                ScriptArgumentString arg = new ScriptArgumentString();
                                string buf;
                                object arg_value;
                                MethodArgument ma;

                                // param = new object[lm.args.Count];

                                arg.Set(argument);

                                for (int k = 0; k < lm.args.Count; k++)
                                {
                                    ma = lm.args[k];

                                    arg.GetArgument(out buf);

                                    if (buf.Length == 0)
                                    {
                                        // params 인 경우는 더이상 없으면 해석을 끝내지만 오류는 아니다. 
                                        if (ma.eInOut == EnumInOut.Params)
                                        {
                                            break;
                                        }
                                        else if (ma.eInOut == EnumInOut.OutParams)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            string msg = String.Format("{0}번째 인자'{2}'가 없습니다.\n{1}", k + 1, MakeMethodUsageAll(lm), ma.pVar.sVarName);
                                            scriptClass.ErrorMessage(msg);
                                            return (-1, retn_value);
                                        }
                                    }

                                    if (ma.pVar.eVarType == ScriptLibRun.EnumVarType.TypeString)
                                    {
                                        (bool success, string arg_value_string) = await scriptClass.GetArgumentString(buf).ConfigureAwait(false);
                                        if (!success)
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return (-1, retn_value);
                                        }
                                        arg_value = arg_value_string;
                                    }
                                    else if (ma.pVar.eVarType == ScriptLibRun.EnumVarType.TypeBool)
                                    {
                                        (bool success, int arg_value_int) = await scriptClass.GetValueRecurseAsInt(buf).ConfigureAwait(false);
                                        if (!success)
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return (-1, retn_value);
                                        }
                                        
                                        arg_value = (arg_value_int==1) ? true : false;
                                    }
                                    else if (ma.pVar.eVarType == ScriptLibRun.EnumVarType.TypeInt)
                                    {
                                        (bool success, int arg_value_int) = await scriptClass.GetValueRecurseAsInt(buf).ConfigureAwait(false);
                                        if (!success)
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return (-1, retn_value);
                                        }

                                        arg_value = arg_value_int;
                                    }
                                    else if (ma.pVar.eVarType == ScriptLibRun.EnumVarType.TypeFloat)
                                    {
                                        (bool success, float arg_value_float) = await scriptClass.GetValueRecurseAsFloat(buf).ConfigureAwait(false);
                                        if (!success)
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return (-1, retn_value);
                                        }

                                        arg_value = arg_value_float;
                                    }
                                    else if (ma.pVar.eVarType == ScriptLibRun.EnumVarType.TypeDouble)
                                    {
                                        (bool success, double arg_value_double) = await scriptClass.GetValueRecurseAsDouble(buf).ConfigureAwait(false);
                                        if (!success)
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return (-1, retn_value);
                                        }

                                        arg_value = arg_value_double;
                                    }
                                    else if (ma.pVar.eVarType == ScriptLibRun.EnumVarType.TypeObject)
                                    {
                                        (bool success, object arg_value_obj) = await scriptClass.GetValueRecurse(buf).ConfigureAwait(false);
                                        if (!success)
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return (-1, retn_value);
                                        }

                                        arg_value = arg_value_obj;
                                    }
                                    else if (ma.pVar.eVarType == ScriptLibRun.EnumVarType.TypeByte)
                                    {
                                        (bool success, byte arg_value_byte) = await scriptClass.GetValueRecurseAsByte(buf).ConfigureAwait(false);
                                        if (!success)
                                        {
                                            string msg = String.Format("\n\nParameter{0} ({1})\n{2}", k + 1, ma.pVar.sVarName, MakeMethodUsageAll(lm));
                                            scriptClass.AddErrorMessage(msg);
                                            return (-1, retn_value);
                                        }

                                        arg_value = arg_value_byte;
                                    }
                                    else
                                    {
                                        scriptClass.ErrorMessage(String.Format("ScriptExternalRun.RunMethodFromOldScript()에서 지원되지 않는 데이터 형식입니다. DataType={0}", ma.pVar.eVarType));
                                        return (-1, retn_value);
                                    }

                                    array_param.Add(arg_value);
                                    //param[k] = arg_value;

                                    // params인경우는 계속한다. 더이상 버퍼가 없을 때 까지
                                    if (ma.eInOut == EnumInOut.Params)
                                        k--;
                                    else if (ma.eInOut == EnumInOut.OutParams)
                                        k--;
                                }

                                /* 없어도 될 듯
                                arg.GetArgument(out buf);
                                if (buf.Length > 0)
                                {
                                    string msg = String.Format("{0} Method 호출 시 너무 많은 인자를 사용했습니다.", MakeMethodUsageAll(lm));
                                    scriptClass.ErrorMessage(msg);
                                }*/
                            }

                            object[] param = null;

                            if (array_param.Count > 0)
                            {
                                param = new object[array_param.Count];
                                for (int k = 0; k < array_param.Count; k++)
                                {
                                    param[k] = array_param[k];
                                }
                            }

                            //int retn = lm.pMethod(scriptClass, methodname, out retn_value, param);
                            //if (retn == -1)
                            //{
                            //    return (-1, retn_value);
                            //}

                            // async 메서드 분기 
                            int retn;
                            if (lm.pAsyncMethod != null)
                            {
                                // async 메서드 호출
                                var result = await lm.pAsyncMethod(scriptClass, methodname, param).ConfigureAwait(false);
                                retn = result.Item1;
                                retn_value = result.Item2;
                            }
                            else if (lm.pMethod != null)
                            {
                                // 기존 sync 메서드 호출
                                retn = lm.pMethod(scriptClass, methodname, out retn_value, param);
                            }
                            else
                            {
                                // 메서드가 설정되지 않음
                                retn = -1;
                                retn_value = 0; //null;
                            }

                            if (retn == -1)
                            {
                                return (-1, retn_value);
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

                                    if (ma.eInOut == EnumInOut.In) continue;
                                    if (ma.eInOut == EnumInOut.Params) continue;

                                    if (ma.eInOut == EnumInOut.OutParams)
                                    {
                                        if (k >= param.Length)
                                        {
                                            break;
                                        }
                                    }

                                    if (ma.pVar.eVarType == ScriptLibRun.EnumVarType.TypeString)
                                    {
                                        if (!await scriptClass.ChangeStringVar(buf, scriptClass.GetValueString(param[k]))) return (-1, retn_value);
                                    }
                                    else
                                    {
                                        if(param[k].GetType() == typeof(byte[])) {
                                            if (!await scriptClass.ChangeArrayVar(buf, (byte[])param[k], buf)) return (-1, retn_value);
                                        }
                                        else {
                                            if (!await scriptClass.ChangeNumberVar(buf, scriptClass.GetValueDouble(param[k]), buf)) return (-1, retn_value);
                                        }
                                    }
                                }

                            }

                            return (retn, retn_value);
                        }
                    }

                    // 부분적으로 지원하는 경우가 있으므로 오류를 내면 안된다. 2013-2-18  모든 함수가 지원된 다음 아래를 체크해야 한다.
                    /*
                    if (Tools.IsLangKorean())
                    {
                        scriptClass.ErrorMessage(String.Format("지원되지 않는 {0}??? 함수입니다.\n{1}", arrayGroup[i].sGroupName, methodname));
                    }
                    else if (Tools.IsLangChinese())
                    {
                        scriptClass.ErrorMessage(String.Format("不支持的 {0}??? 函数。({1})", arrayGroup[i].sGroupName, methodname));
                    }
                    else
                    {
                        scriptClass.ErrorMessage(String.Format("Undefined {0}??? function.\n{1}", arrayGroup[i].sGroupName, methodname));
                    }
                    return -1; 
                     * 
                     */

                    return (0, retn_value);
                }
            }

            return (0, retn_value);  // 아직 지원하지 않은 것이 있으므로 0으로 return하여 다음 함수를 검사하도록 한다.
        }

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

        /// <summary>
        /// Monaco Editor 자동완성용 메서드 목록을 반환한다.
        /// 각 항목: [0]=name, [1]=returnType, [2]=signature(with args)
        /// </summary>
        public List<string[]> GetMethodCompletionList()
        {
            var list = new List<string[]>();
            for (int i = 0; i < arrayGroup.Count; i++)
            {
                for (int j = 0; j < arrayGroup[i].arrayMethod.Count; j++)
                {
                    var lm = arrayGroup[i].arrayMethod[j];
                    string name = lm.sMethodName;
                    string retn = lm.retn ?? "void";
                    string sig = MakeMethodUsageBody(lm);
                    list.Add(new string[] { name, retn, sig });
                }
            }
            return list;
        }

        public override int IsExistVariable(string varname)
        {
            if (varname.Length == 0) return 0;
            if (varname[0] != '$') return 0;    // 해당없음

            string tag = varname.Substring(1);

            ReadyTagMember rtm = new ReadyTagMember();
            int[] tag_pos = new int[1];

            if (!TagLib.GetTagTypePosMember(tag, out rtm.tag_name, out rtm.tag_type, ref tag_pos, out rtm.tag_member, out rtm.member_var, out rtm.tp))
            {
                if (Tools.IsLangKorean())
                {
                    SetError(EnumScriptErrorType.TagNotFound, String.Format("존재하지 않는 태그(${0})", tag));
                }
                else
                {
                    SetError(EnumScriptErrorType.TagNotFound, String.Format("(${0}) tag not found", tag));
                }

                return 2;
            }

            return 1;
        }

        public override int IsExistMethod(string methodname, List<CommandMethodArg> args)
        {
            if (methodname.Length == 0) return 0;
            if (methodname[0] != '@') return 0;    // 해당 없음

            string method = methodname.Substring(1);

            for (int i = 0; i < arrayGroup.Count; i++)
            {
                if (String.Compare(arrayGroup[i].sGroupName, 0, method, 0, arrayGroup[i].sGroupName.Length) != 0) continue;

                LocalMethod lm;

                for (int j = 0; j < arrayGroup[i].arrayMethod.Count; j++)
                {
                    lm = arrayGroup[i].arrayMethod[j];
                    if (lm.sMethodName == method)
                    {
                        // 선언에 아규먼트가 없으면 O.K
                        if (lm.args == null) return 1;
                        if (lm.args.Count == 0) return 1;

                        MethodArgument ma;
                        for (int k = 0; k < lm.args.Count; k++)
                        {
                            ma = lm.args[k];

                            // params 인 경우는 더이상 없으면 해석을 끝내지만 오류는 아니다. 
                            if (ma.eInOut == EnumInOut.Params)
                                return 1;
                            else if (ma.eInOut == EnumInOut.OutParams)
                            {
                                return 1;
                            }

                            if (k >= args.Count)
                            {
                                //sErrorMessage = String.Format("Argument {0} needed. check the argument count {1}", k + 1, MakeMethodUsageAll(lm), ma.pVar.sVarName);
                                SetError(EnumScriptErrorType.Else, String.Format("{0}번째 인자'{2}'가 없습니다. {1}", k + 1, MakeMethodUsageAll(lm), ma.pVar.sVarName));
                                return 2;
                            }
                        }

                        return 1;
                    }
                }
            }

            SetError(EnumScriptErrorType.Else, String.Format("The method name '{0}' does not exist in the current context", methodname));

            return 2;
        }

        // 태그를 멤버별로 분리한 다음 다음에 호출하기 쉽게 이 클래스를 저장한다.
        class ReadyTagMember
        {
            public string tag_name;	
            public EnumTagType tag_type;
            public TagPublicClass tp;
            public EnumTagMember tag_member;
            public EnumTagMemberVar member_var;
            public int[] tag_pos = new int[1];
        }

        public override object RunVariable(ScriptRunConfiguration src, object pre_pointer, string varname, out object value)
        {
            ScriptClass sc = (ScriptClass)src.externalConfig;
            ReadyTagMember rtm;

            if (pre_pointer != null)
            {
                rtm = (ReadyTagMember)pre_pointer;
            }
            else
            {
                if (varname == null || varname.Length == 0)
                {
                    SetError(EnumScriptErrorType.Else, "VarName is Empty");
                    value = null;
                    return null;
                }
                else if (varname[0] != '$')
                {
                    SetError(EnumScriptErrorType.Else, "VarName must start $ charactor");
                    value = null;
                    return null;
                }

                string tag = varname.Substring(1);

                rtm = new ReadyTagMember();

                if (!TagLib.GetTagTypePosMember(tag, out rtm.tag_name, out rtm.tag_type, ref rtm.tag_pos, out rtm.tag_member, out rtm.member_var, out rtm.tp))
                {
                    if (Tools.IsLangKorean())
                    {
                        SetError(EnumScriptErrorType.TagNotFound, String.Format("존재하지 않는 태그(${0}) RunVariable()", tag));
                    }
                    else
                    {
                        SetError(EnumScriptErrorType.TagNotFound, String.Format("(${0}) tag not found. RunVariable()", tag));
                    }
                    value = null;
                    return null;		// 대입문이 아니다.
                }
            }

            if (rtm.member_var == EnumTagMemberVar.MEMBER_VAR_string)
            {
                value = sc.GetTagMemberValue(rtm.tag_name, rtm.tag_type, rtm.tp, rtm.tag_member);
            }
            else
            {
                value = sc.GetTagMemberValue(rtm.tag_name, rtm.tag_type, rtm.tp, rtm.tag_member);
            }

            return rtm;
        }

        // ScriptClass parentScript = new ScriptClass();   // RunMethod에서 사용 시 오류 정보만 가져오기 때문에 속도를 위해 외부에 선언해서 함께 사용해도 관계없을 듯

        // 신 스크립트에서 @method 일 때 호출하는 함수
        public override async Task<(object, object retn)> RunMethodAsync(ScriptRunConfiguration src, object pre_pointer, string methodname, object[] param, List<CommandMethodArg> args)
        {
            LocalMethod lm;
            string method = methodname.Substring(1);
            object retn;

            if (pre_pointer != null)
            {
                lm = (LocalMethod)pre_pointer;
            }
            else
            {
                for (int i = 0; i < arrayGroup.Count; i++)
                {
                    if (String.Compare(arrayGroup[i].sGroupName, 0, method, 0, arrayGroup[i].sGroupName.Length) != 0) continue;

                    for (int j = 0; j < arrayGroup[i].arrayMethod.Count; j++)
                    {
                        lm = arrayGroup[i].arrayMethod[j];

                        if (lm.sMethodName == method)
                        {
                            // Version 10까지의 @로 시작하는 함수는 out 문법이 없기 때문에 실제 프로젝트에 out이 없다. 그래서 NewScript에서 이전 @함수를 호출할때는 out을 자동으로 붙여주는것이 좋을 듯하다.
                            // lm.args 가 null 인 경우도 있다. int rand()
                            for (int k = 0; lm.args != null && k < lm.args.Count; k++)
                            {
                                if (lm.args[k].eInOut == EnumInOut.Out)
                                    args[k].eInOut = EnumInOut.Out;
                                else if (lm.args[k].eInOut == EnumInOut.Ref)
                                    args[k].eInOut = EnumInOut.Ref;
                                else if (lm.args[k].eInOut == EnumInOut.OutParams)
                                {
                                    if(k < args.Count) 
                                        args[k].eInOut = EnumInOut.OutParams;
                                }
                            }
                            goto ok_seeked;
                        }
                    }
                }

                retn = 0;

                SetError(EnumScriptErrorType.Else, String.Format("Method not exists {0}", methodname));

                return (null, retn);
            }

        ok_seeked: ;

            if (lm.args != null)
            {
                
                // 호출하는 변수값이 메소드의 원형과 같게 변수를 맞추어 준다.
                for (int k = 0; k < lm.args.Count; k++)
                {


                    // Out 일경우는 인자값을 줄필요가 없다.
                    if (lm.args[k].eInOut == EnumInOut.Out) continue;
                    if (lm.args[k].eInOut == EnumInOut.OutParams) continue;

                    if (lm.args[k].pVar.eVarType == ScriptLibRun.EnumVarType.TypeInt)
                        param[k] = ObjectValue.ToInt(param[k]);
                    else if (lm.args[k].pVar.eVarType == ScriptLibRun.EnumVarType.TypeFloat)
                        param[k] = ObjectValue.ToFloat(param[k]);
                    else if (lm.args[k].pVar.eVarType == ScriptLibRun.EnumVarType.TypeDouble)
                        param[k] = ObjectValue.ToDouble(param[k]);
                    else if (lm.args[k].pVar.eVarType == ScriptLibRun.EnumVarType.TypeString)
                        param[k] = ObjectValue.ToString(param[k]);
                    else if (lm.args[k].pVar.eVarType == ScriptLibRun.EnumVarType.TypeByte)
                    {
                        param[k] = ObjectValue.ToString(param[k]);
                    }
                    else if (lm.args[k].pVar.eVarType == ScriptLibRun.EnumVarType.TypeObject)
                    {
                        //param[k] = ObjectValue.ToString(param[k]);
                    }
                    else
                    {
                        SetError(EnumScriptErrorType.Else, String.Format("lm.args[k].pVar.eVarType not defined. Type={0}", lm.args[k].pVar.eVarType.ToString()));
                        retn = 0;
                        return (null, retn);
                    }
                }
            }

            ScriptClass sc = (ScriptClass)src.externalConfig;

            //int r = lm.pMethod(sc, method, out retn, param);

            //if (r == -1)
            //{
            //    SetError(sc.GetErrorType(), sc.GetError());
            //    return (null, retn);
            //}

            int r;
            if (lm.pAsyncMethod != null)
            {
                // async 메서드 호출
                var result = await lm.pAsyncMethod(sc, method, param).ConfigureAwait(false);
                r = result.Item1;
                retn = result.Item2;
            }
            else if (lm.pMethod != null)
            {
                // 기존 sync 메서드 호출
                r = lm.pMethod(sc, method, out retn, param);
            }
            else
            {
                // 메서드가 설정되지 않음
                r = -1;
                retn = 0; //null;
            }

            if (r == -1)
            {
                SetError(sc.GetErrorType(), sc.GetError());
                return (null, retn);
            }

            return (lm, retn);
        }

        public override async Task<object> ChangeVariable(ScriptRunConfiguration src, object pre_pointer, string varname, object value)
        {
            ScriptClass sc = (ScriptClass)src.externalConfig;
            ReadyTagMember rtm;

            if (pre_pointer != null)
            {
                rtm = (ReadyTagMember)pre_pointer;
            }
            else
            {
                if (varname == null || varname.Length == 0)
                {
                    SetError(EnumScriptErrorType.Else, "VarName is Empty");
                    value = null;
                    return false;
                }
                else if (varname[0] != '$')
                {
                    SetError(EnumScriptErrorType.Else, "VarName must start $ charactor");
                    value = null;
                    return false;
                }

                string tag = varname.Substring(1);

                rtm = new ReadyTagMember();

                if (!TagLib.GetTagTypePosMember(tag, out rtm.tag_name, out rtm.tag_type, ref rtm.tag_pos, out rtm.tag_member, out rtm.member_var, out rtm.tp))
                {
                    if (Tools.IsLangKorean())
                    {
                        SetError(EnumScriptErrorType.TagNotFound, String.Format("존재하지 않는 태그(${0})", tag));
                    }
                    else
                    {
                        SetError(EnumScriptErrorType.TagNotFound, String.Format("(${0}) tag not found", tag));
                    }
                    value = null;
                    return null;		// 대입문이 아니다.
                }
            }

            if (rtm.member_var == EnumTagMemberVar.MEMBER_VAR_string)
            {
                string value_string;
                value_string = sc.GetValueString(value);
                rtm.tag_pos = await sc.SetTagMemberString(rtm.tag_name, rtm.tag_type, rtm.tag_pos, rtm.tag_member, value_string);
            }
            else
            {
                rtm.tag_pos = await sc.SetTagMemberValue(rtm.tag_name, rtm.tag_type, rtm.tag_pos, rtm.tag_member, sc.GetValueDouble(value));
            }

            return rtm;
        }

        
    }
}
