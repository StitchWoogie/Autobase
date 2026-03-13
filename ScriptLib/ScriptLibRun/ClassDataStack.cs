using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;

namespace ScriptLibRun
{
    [Serializable]
    public class ClassDataStack
    {
        public ClassDataStack parentStack = null;

        public List<ItemDataStack> arrayData = new List<ItemDataStack>();

        public object selfValue = null;     // (string)System.String이나 (int)System.Int32 같은 클래스일 경우 자신의 값을 말한다. 

        public ClassDataStack(ClassDataStack parent_stack)
        {
            parentStack = parent_stack;
        }

        public bool GetValueInClassStack(string varname, List<int[]> dimensional_pos, out object value)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                if (arrayData[i].pVar.sVarName == varname)
                {
                    if (dimensional_pos == null)
                        value = arrayData[i].value;
                    else
                    {
                        value = GetValueByDimension(arrayData[i].value, dimensional_pos, 0);
                    }
                    //value = arrayData[i].value;
                    return true;
                }
            }

            if (parentStack != null)
                return parentStack.GetValueInClassStack(varname, dimensional_pos, out value);

            value = 0;

            return false;
        }

        //object GetValueByDimension(object source, List<int[]> dimensional_pos, int i)
        //{
        //    if (i >= dimensional_pos.Count) return null;

        //    Type type = source.GetType();

        //    // 문자열인경우 배열을 요구하면 문자를 반환해 준다.
        //    if (type == typeof(string))
        //    {
        //        string source1 = (string)source;

        //        int pos = dimensional_pos[i][0];

        //        // 이전스크립트에서 char[100] 변수는 string으로 변환되면서 실제로 100이 아니게 된다. 배열을 초과하는 경우를 막아야 한다. 2018-1-11
        //        if (pos < source1.Length)
        //            return source1[pos];
        //        else
        //            return 0;
        //    }

        //    if (!type.IsArray) return null;

        //    if (type == typeof(byte[]))
        //    {
        //        byte[] source1 = (byte[])source;

        //        int pos = dimensional_pos[i][0];

        //        if (i >= dimensional_pos.Count - 1)   // 마지막 
        //            return source1[pos];

        //        return GetValueByDimension(source1[pos], dimensional_pos, i + 1);
        //    }
        //    else 
        //    {
        //        object[] source1 = (object[])source;

        //        int pos = dimensional_pos[i][0];

        //        if (i >= dimensional_pos.Count - 1)   // 마지막 
        //            return source1[pos];

        //        return GetValueByDimension(source1[pos], dimensional_pos, i + 1);
        //    }

        //}

        object GetValueByDimension(object source, List<int[]> dimensional_pos, int i)
        {
            if (i >= dimensional_pos.Count) return null;

            Type type = source.GetType();
            string message;
            // 문자열인경우 배열을 요구하면 문자를 반환해 준다.
            if (type == typeof(string))
            {
                string source1 = (string)source;
                int pos = dimensional_pos[i][0];

                // 이전스크립트에서 char[100] 변수는 string으로 변환되면서 실제로 100이 아니게 된다. 배열을 초과하는 경우를 막아야 한다. 2018-1-11
                if (pos < source1.Length && pos >= 0)
                    return source1[pos];
                else
                {
                    if (Tools.IsLangKorean())
                    {
                        message = string.Format("GetValue, 배열 인덱스 {0}가(이) 길이 {1}를 초과했습니다", pos, source1.Length);
                    }
                    else message = string.Format("GetValue, Array index {0} is out of bounds for length {1}", pos, source1.Length);
                    throw new IndexOutOfRangeException(message);
                } // 범위를 벗어난 경우, 0으로 출력되던 점을 에러로 변경 20250107 PSU
            }

            if (!type.IsArray) return null;

            if (type == typeof(byte[]))
            {
                byte[] source1 = (byte[])source;
                int pos = dimensional_pos[i][0];

                if (pos < 0 || pos >= source1.Length)
                {
                    if (Tools.IsLangKorean())
                    {
                        message = string.Format("GetValue, 배열 인덱스 {0}가(이) 길이 {1}를 초과했습니다", pos, source1.Length);
                    }
                    else message = string.Format("GetValue, Array index {0} is out of bounds for length {1}", pos, source1.Length);
                    throw new IndexOutOfRangeException(message);
                } // 범위를 벗어난 경우, 인덱스 오류로 프로그램이 종료되어 에러메시지 추가 20250107 PSU

                if (i >= dimensional_pos.Count - 1)   // 마지막 
                    return source1[pos];

                return GetValueByDimension(source1[pos], dimensional_pos, i + 1);
            }
            else
            {
                object[] source1 = (object[])source;

                int pos = dimensional_pos[i][0];
                if (pos < 0 || pos >= source1.Length)
                {
                    if (Tools.IsLangKorean())
                    {
                        message = string.Format("GetValue, 배열 인덱스 {0}가 길이 {1}를 초과했습니다", pos, source1.Length);
                    }
                    else message = string.Format("GetValue, Array index {0} is out of bounds for length {1}", pos, source1.Length);
                    throw new IndexOutOfRangeException(message);
                } // 범위를 벗어난 경우, 인덱스 오류로 프로그램이 종료되어 에러메시지 추가 20250107 PSU

                if (i >= dimensional_pos.Count - 1)   // 마지막 
                    return source1[pos];

                return GetValueByDimension(source1[pos], dimensional_pos, i + 1);
            }
        }

        /// <summary>
        /// class_stack은 클래스가 만들어질 때 한번 생겨서 클래스에 포함된 변수를 저장한다.
        /// </summary>
        /// <param name="class_stack">class_stack은 클래스가 만들어질 때 한번 생겨서 클래스에 포함된 변수를 저장한다.</param>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GetValue(ClassDataStack class_stack, string varname, List<int[]> dimensional_pos, out object value)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                if (arrayData[i].pVar.sVarName == varname)
                {
                    if (dimensional_pos == null)
                        value = arrayData[i].value;
                    else
                    {
                        value = GetValueByDimension(arrayData[i].value, dimensional_pos, 0);
                    }
                    return true;
                }
            }

            if (parentStack != null)
                return parentStack.GetValue(class_stack, varname, dimensional_pos, out value);
            
            if(class_stack != null)
                return class_stack.GetValueInClassStack(varname, dimensional_pos, out value);

            value = 0;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetValueInClassStack(string varname, List<int[]> dimensional_pos, object value)
        {
            ItemDataStack ids;

            for (int i = 0; i < arrayData.Count; i++)
            {
                ids = arrayData[i];

                if (ids.pVar.sVarName == varname)
                {
                    if (dimensional_pos == null)
                    {
                        // 배열인데 배열이 없으면 포인터 대입으로 보임 a = new int[50]; // 이나 a = b 같은
                        if (ids.pVar.nDimensionals != null)
                        {
                            ids.value = value;
                        }
                        else 
                            ids.value = CastToFitVarType(ids.pVar.eVarType, value);
                    }
                    else
                    {
                        SetValueByDimension(ids.pVar.eVarType, ref ids.value, dimensional_pos, 0, value);
                    }

                    return true;
                }
            }

            if (parentStack != null)
                return parentStack.SetValueInClassStack(varname, dimensional_pos, value);

            return false;
        }

        //void SetValueByDimension(EnumVarType var_type, ref object target, List<int[]> dimensional_pos, int i, object value)
        //{
        //    if (i >= dimensional_pos.Count) return;
        //    if (!target.GetType().IsArray) return;

        //    object[] target1 = (object[])target;

        //    int pos = dimensional_pos[i][0];

        //    // 배열 범위 체크 추가
        //    if (pos >= target1.Length) return;  // 범위를 벗어난 경우 조용히 리턴 20250107 PSU

        //    if (i >= dimensional_pos.Count - 1)   // 마지막
        //        target1[pos] = CastToFitVarType(var_type, value);
        //    else
        //        SetValueByDimension(var_type, ref target1[pos], dimensional_pos, i + 1, value);
        //}

        void SetValueByDimension(EnumVarType var_type, ref object target, List<int[]> dimensional_pos, int i, object value)
        {
            if (i >= dimensional_pos.Count) return;
            if (!target.GetType().IsArray) return;

            object[] target1 = (object[])target;

            int pos = dimensional_pos[i][0];

            // 배열 범위 체크 추가
            if (pos < 0 || pos >= target1.Length)
            {
                string message = "";
                if (Tools.IsLangKorean())
                {
                    message = string.Format("SetValue, 배열 인덱스 {0}가(이) 길이 {1}를 초과했습니다", pos, target1.Length);
                }
                else message = string.Format("SetValue, Array index {0} is out of bounds for length {1}", pos, target1.Length);
                throw new IndexOutOfRangeException(message);
            } // 범위를 벗어난 경우, 인덱스 오류로 프로그램이 종료되어 에러메시지 추가 20250107 PSU

            if (i >= dimensional_pos.Count - 1)   // 마지막
                target1[pos] = CastToFitVarType(var_type, value);
            else
                SetValueByDimension(var_type, ref target1[pos], dimensional_pos, i + 1, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="class_stack">class_stack은 클래스가 만들어질 때 한번 생겨서 클래스에 포함된 변수를 저장한다.</param>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(ClassDataStack class_stack, string varname, List<int[]> dimensional_pos, object value)
        {
            ItemDataStack ids;

            for (int i = 0; i < arrayData.Count; i++)
            {
                ids = arrayData[i];

                if (ids.pVar.sVarName == varname)
                {
                    if (dimensional_pos == null)
                    {
                        // 배열인데 배열이 없으면 포인터 대입으로 보임 a = new int[50]; // 이나 a = b 같은
                        if (ids.pVar.nDimensionals != null)
                        {
                            ids.value = value;
                        }
                        else 
                            ids.value = CastToFitVarType(ids.pVar.eVarType, value);
                    }
                    else
                    {
                        SetValueByDimension(ids.pVar.eVarType, ref ids.value, dimensional_pos, 0, value);
                    }

                    return true;
                }
            }

            if (parentStack != null)
                return parentStack.SetValue(class_stack, varname, dimensional_pos, value);

            if(class_stack != null)
                return class_stack.SetValueInClassStack(varname, dimensional_pos, value);

            return false;
        }

        public bool GetItemPointer(string varname, out ItemDataStack item)
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                if (arrayData[i].pVar.sVarName == varname)
                {
                    item = arrayData[i];
                    return true;
                }
            }

            if (parentStack != null)
                return parentStack.GetItemPointer(varname, out item);

            item = null;

            return false;
        }

        // 초기화 후 자신이 가지고 있는 값을 변수의 형태에 맞추어 새로 대입한다.
        void RecurseFitVarTypeAfterInitial(EnumVarType var_type, ref object value)
        {
            if (value == null) return;

            if (!value.GetType().IsArray)
            {
                value = CastToFitVarType(var_type, value);
            }
            else
            {
                if (value.GetType() == typeof(object[]))
                {
                    object[] source1 = (object[])value;

                    for (int i = 0; i < source1.Length; i++)
                    {
                        RecurseFitVarTypeAfterInitial(var_type, ref source1[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Method내에서 변수가 선언되거나, Method가 시작될 때 인자 선언 시 이 함수가 호출된다. 
        /// </summary>
        /// <param name="var"></param>
        /// <param name="value">할당된 값이 넘어온다.</param>
        public void AddItem(Variable var, object value)
        {
            ItemDataStack ids = new ItemDataStack();
            ids.pVar.CopyVar(var);
            
            ids.value = value;
            RecurseFitVarTypeAfterInitial(var.eVarType, ref ids.value);

            arrayData.Add(ids);
        }

        // 변수에 값을 넣을 때 그 변수의 특성에 맞도록 (cast)해서 넣어야 한다. 배열이 아니고 배열의 원소일 때만 해당된다.
        object CastToFitVarType(EnumVarType var_type, object val)
        {
            object value;

            if (var_type == EnumVarType.TypeSbyte)
                value = (sbyte)ObjectValue.ToLong(val);
            else if (var_type == EnumVarType.TypeByte)
                value = (byte)ObjectValue.ToLong(val);
            else if (var_type == EnumVarType.TypeShort)
                value = (short)ObjectValue.ToLong(val);
            else if (var_type == EnumVarType.TypeUShort)
                value = (ushort)ObjectValue.ToLong(val);
            else if (var_type == EnumVarType.TypeInt)
                value = (int)ObjectValue.ToLong(val);
            else if (var_type == EnumVarType.TypeUint)
                value = (uint)ObjectValue.ToLong(val);
            else if (var_type == EnumVarType.TypeLong)
                value = (long)ObjectValue.ToLong(val);
            else if (var_type == EnumVarType.TypeULong)
                value = (ulong)ObjectValue.ToLong(val);

            else if (var_type == EnumVarType.TypeFloat)
                value = (float)ObjectValue.ToDouble(val);
            else if (var_type == EnumVarType.TypeDouble)
                value = (double)ObjectValue.ToDouble(val);

            else if (var_type == EnumVarType.TypeClass)
                value = val;
            else
                value = val;

            return value;
        }

        // 디버그시 Studio에서 데이터를 보려고 사용한다. value를 그냥 Serialize 하면 다운되는 오브젝트 타입이 있어서 valueToStudio로 변경해서 사용한다.
        public void ConvertToStudioValue()
        {
            for (int i = 0; i < arrayData.Count; i++)
            {
                arrayData[i].ConvertToStudioValue();
            }

            if (parentStack != null)
                parentStack.ConvertToStudioValue();
        }
        
    }

    [Serializable]
    public class ItemDataStack
    {
        public Variable pVar = new Variable();

        [NonSerialized]
        public object value;            // 실제 값이 object[]이면 Serializeble 이 되지 않고 다운된다.

        public object valueToStudio;    // 디버그시 Studio에서 데이터를 보려고 사용한다. value를 그냥 Serialize 하면 다운되는 오브젝트 타입이 있어서 valueToStudio로 변경해서 사용한다.

        string ObjectToString(object source)
        {
            if (source.GetType() == typeof(object[]))
            {
                object[] objs = (object[])source;

                string s = "{";

                for (int i = 0; i < objs.Length; i++)
                {
                    if (objs[i] == null)
                        s += "null";
                    else 
                        s += ObjectToString(objs[i]);

                    if (i < objs.Length - 1)
                        s += ",";
                }
                s += "}";
                return s;
            }
            else
            {
                return source.ToString();
            }
        }

        public void ConvertToStudioValue()
        {
            if (value == null)
            {
                valueToStudio = "null";
            }
            else if (value.GetType() == typeof(object[]))
            {
                valueToStudio = ObjectToString(value);
            }
            else
            {
                valueToStudio = value;
            }
        }
        /*
        // 변수에 값을 넣을 때 그 변수의 특성에 맞도록 (cast)해서 넣어야 한다.
        public void SetValue(object val)
        {
            if (pVar.nDimensionals != null)
            {
                int a = 10;
            }

            if (pVar.eVarType == EnumVarType.TypeSbyte)
                value = (sbyte)ObjectValue.ToLong(val);
            else if (pVar.eVarType == EnumVarType.TypeByte)
                value = (byte)ObjectValue.ToLong(val);
            else if (pVar.eVarType == EnumVarType.TypeShort)
                value = (short)ObjectValue.ToLong(val);
            else if (pVar.eVarType == EnumVarType.TypeUShort)
                value = (ushort)ObjectValue.ToLong(val);
            else if (pVar.eVarType == EnumVarType.TypeInt)
                value = (int)ObjectValue.ToLong(val);
            else if (pVar.eVarType == EnumVarType.TypeUint)
                value = (uint)ObjectValue.ToLong(val);
            else if (pVar.eVarType == EnumVarType.TypeLong)
                value = (long)ObjectValue.ToLong(val);
            else if (pVar.eVarType == EnumVarType.TypeULong)
                value = (ulong)ObjectValue.ToLong(val);

            else if (pVar.eVarType == EnumVarType.TypeFloat)
                value = (float)ObjectValue.ToDouble(val);
            else if (pVar.eVarType == EnumVarType.TypeDouble)
                value = (double)ObjectValue.ToDouble(val);

            //else if (pVar.eVarType == EnumVarType.TypeString)
            //    value = ObjectValue.ToString(val);

            else if (pVar.eVarType == EnumVarType.TypeClass)
                value = val;
            else
                value = val;
        }*/
    }
}
