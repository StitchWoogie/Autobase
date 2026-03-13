using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class RecursiveValue
    {
        public enum EnumCalcMethod
        {
            LastValue = 0,
            Plus = 1,       // a + b
            Minus = 2,      // a - b
            Multiply = 3,   // a * b
            Divide = 4,     // a / b
            Remainder = 5,  // a % b
            Or = 6,         // a | b
            And = 7,        // a & b    
            Xor = 8,        // a ^ b    
            Parenthesis = 9,// () 로 쌓여진 값

            PrefixIncrement=10,     // ++x
            PostfixIncrement = 11,  // x++
            PrefixDecrement = 12,   // --x
            PostfixDecrement = 13,  // x--

            UnaryPlus = 15, // +x
            UnaryMinus = 16, // -x
            UnaryNegation = 17, // !x  부정으로 실제로는 true false에서만 사용되어야 한다.
            UnaryBitwiseComplement = 18,    // ~x  비트보수

            LeftShift = 19,     // a << b
            RightShift = 20,    // a >> b

            Cast = 21,          // (cast)value 와 같이 cast된 값
        }

        public enum EnumLastValueType
        {
            None,               // 값이 주어지지 않았다. 글자가 아예 없다는 뜻
            Variable,           // 자신의 클래스에 선언된 변수이거나, 인자로 넘어온 변수, Method내부에 선언된 변수를 말한다.
            Constant,
            Method,
            ExternalVariable,   // 외부 어플리케이션에서 승인한 변수
            NewClass,           // new Class() 같은 형식
            NewArray,           // new int[30] 와 같은 형식
            InitArray,          // new가 없이 = {3,4,5}만 있는 형식
            AllocVariable,      // 클래스로 할당된 변수의 Variable
            AllocProperty,      // 클래스로 할당된 변수의 Property
            StaticProperty,     // Static Property
            StaticVariable,     // Static Variable  1. Class 속의 static 변수, 2. Enum의 멤버  3. Class속의 const

            Array_Length,       // [] Array 속성중에서 a.Length
        }

        protected RecursiveValue leftVal = null;
        protected RecursiveValue rightVal = null;
        protected EnumCalcMethod eCalcMethod;       // 수식으로 되어 있는 형식
        public string sLastValue = null;
        protected string sSourceString;
        public EnumLastValueType eValueType;     // 최종값 형식
        protected CommandMethod pMethod = null;
        protected EnumVarType eConstantType;

        protected ScriptLibMemberMethod parentMethod;
        protected CommandPublic parentBlock;

        protected string sCompiledNamespace;
        protected string sCompiledClass;
        protected string sCompiledMethod;

        public JaggedArrays pJaggedArrays = null;
        protected InitialValue pInitial = null;

        public RecursiveValue(ScriptLibMemberMethod parent, CommandPublic parent_block)
        {
            parentMethod = parent;
            parentBlock = parent_block;
        }

        public int nColumn, nRow;

        //ScriptLibClass pRunClass = null;          // Run시만 사용하는 포인트
        ScriptLibMemberMethod pRunMethod = null;    // Run시만 사용하는 포인트
        ScriptLibMemberPublic pRunPublic = null;

        object pExternalVariable = null;

        bool IsFloatingPointValue(object val)
        {
            Type type = val.GetType();

            if (type == typeof(float)) return true;
            if (type == typeof(double)) return true;

            return false;
        }

        public async Task<(bool success, object result)> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack)
        {
            object val = 0;

            if (eCalcMethod == EnumCalcMethod.LastValue)
            {
                if (eValueType == EnumLastValueType.None)
                {
                    return (true, val);
                }

                List<int[]> dimensional_values = null;

                // 먼저 Dimension이 있으면 계산해 둔다. 아래에서 반드시 사용한다.
                if (pJaggedArrays != null)
                {
                    dimensional_values = new List<int[]>();
                    var jaggedResult = await pJaggedArrays.RunAsync(src, class_stack, parent_stack, dimensional_values).ConfigureAwait(false);
                    if (!jaggedResult) return (false, val);
                }

                if (eValueType == EnumLastValueType.Method)
                {
                    var (methodSuccess, methodVal) = await pMethod.RunAsync(src, class_stack, parent_stack, false).ConfigureAwait(false);
                    if (!methodSuccess) return (false, methodVal);
                    return (true, methodVal);
                }
                else if (eValueType == EnumLastValueType.StaticProperty)
                {
                    if (pRunMethod == null)
                    {
                        pRunMethod = src.slmain.GetPropertyPointer(sCompiledNamespace, sCompiledClass, sCompiledMethod);
                    }

                    var (propSuccess, propVal) = await pRunMethod.RunAsync(src, class_stack, null).ConfigureAwait(false);
                    if (!propSuccess) return (false, propVal);
                    return (true, propVal);
                }
                else if (eValueType == EnumLastValueType.StaticVariable)    // 
                {
                    if (pRunPublic == null)
                    {
                        pRunPublic = src.slmain.GetStaticVariablePointer(src.slmain, sCompiledNamespace, sCompiledClass, sCompiledMethod);
                    }

                    var (staticSuccess, staticVal) = await pRunPublic.GetStaticValueAsync(src).ConfigureAwait(false);
                    if (!staticSuccess) return (false, staticVal);
                    return (true, staticVal);
                }
                else if (eValueType == EnumLastValueType.NewClass)
                {
                    // new Class() 로 시작한 호출은 new_class 인자를 true로 설정한다.
                    var (newSuccess, newVal) = await pMethod.RunAsync(src, class_stack, parent_stack, true).ConfigureAwait(false);
                    if (!newSuccess) return (false, newVal);
                    return (true, newVal);
                }
                else if (eValueType == EnumLastValueType.InitArray)
                {
                    var (initSuccess, initVal) = await pInitial.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!initSuccess) return (false, initVal);
                    return (true, initVal);
                }
                else if (eValueType == EnumLastValueType.NewArray)
                {
                    if (pInitial != null)
                    {
                        var (initSuccess, initVal) = await pInitial.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                        if (!initSuccess) return (false, initVal);
                        val = initVal;
                    }
                    else
                    {
                        int[] dimentional_value = dimensional_values[0];

                        int hap = 0;

                        for (int i = 0; i < dimentional_value.Length; i++)
                        {
                            hap += dimentional_value[i];
                        }
                        val = new object[hap];
                    }

                    return (true, val);
                }
                else if (eValueType == EnumLastValueType.AllocVariable)
                {
                    ItemDataStack ids;
                    if (!parent_stack.GetItemPointer(sCompiledClass, out ids))
                    {
                        src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary,
                            parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.VarNotFound,
                            "{0}.{1}에서 선언된 {0} 변수를 찾을 수 없습니다.", sCompiledClass, sCompiledMethod);
                        return (false, 0);
                    }

                    ClassDataStack stack = (ClassDataStack)ids.value;
                    bool varSuccess = stack.GetValueInClassStack(sCompiledMethod, dimensional_values, out val);
                    if (!varSuccess) return (false, val);
                    return (true, val);
                }
                else if (eValueType == EnumLastValueType.AllocProperty)
                {
                    ItemDataStack ids;
                    if (!parent_stack.GetItemPointer(sCompiledClass, out ids))
                    {
                        src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary,
                            parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.VarNotFound,
                            "{0}.{1}에서 선언된 {0} 변수를 찾을 수 없습니다.", sCompiledClass, sCompiledMethod);
                        return (false, val);
                    }

                    ClassDataStack stack = (ClassDataStack)ids.value;
                    var (propSuccess, propVal) = await pMethod.RunAsync(src, class_stack, parent_stack, false).ConfigureAwait(false);
                    if (!propSuccess) return (false, propVal);
                    return (true, propVal);
                }
                else if (eValueType == EnumLastValueType.ExternalVariable)
                {
                    if (src.slmain.scriptExternal == null)
                    {
                        src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary,
                 parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else,
                 "You must alloc ScriptExternalClass.");
                        return (false, val);
                    }

                    var extResult = src.slmain.scriptExternal.RunVariable(src, pExternalVariable, sLastValue, out val);
                    if (extResult != null)
                    {
                        pExternalVariable = extResult;
                        return (true, val);
                    }
                    else
                    {
                        src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary,
                            parentMethod.sSourceFilename, nColumn, nRow,
                            src.slmain.scriptExternal.ErrorType, src.slmain.scriptExternal.ErrorMessage);
                        return (false, val);
                    }

                }
                else if (eValueType == EnumLastValueType.Constant)
                {
                    if (eConstantType == EnumVarType.TypeInt)
                        val = ConvertTool.ToInt32(sLastValue);
                    else if (eConstantType == EnumVarType.TypeDouble)
                        val = ConvertTool.ToDouble(sLastValue);
                    else if (eConstantType == EnumVarType.TypeChar)
                        val = sLastValue[0];
                    else if (eConstantType == EnumVarType.TypeNull)
                        val = null;        // 일단 null은 (int)0으로 한다. 2017-3-6 다시 null 으로 변경했다.
                    else
                        val = sLastValue;

                    return (true, val);
                }
                else if (eValueType == EnumLastValueType.Variable)
                {
                    if (parent_stack.GetValue(class_stack, sLastValue, dimensional_values, out val))
                        return (true, val);

                    if (sLastValue.Length > 0 && sLastValue[0] == '"' && sLastValue[sLastValue.Length - 1] == '"')
                    {
                        val = sLastValue.Substring(1, sLastValue.Length - 2);
                    }
                    else
                    {
                        val = sLastValue;
                    }
                    // return (true, val); //없었음
                }
                else if (eValueType == EnumLastValueType.Array_Length)
                {
                    ItemDataStack ids;
                    if (!parent_stack.GetItemPointer(sCompiledClass, out ids))
                    {
                        src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "{0}.{1}에서 선언된 {0} 변수를 찾을 수 없습니다.", sCompiledClass, sCompiledMethod);
                        return (false, val);
                    }

                    if (ids.value != null && ids.value.GetType().IsArray)
                        val = ((Array)(ids.value)).Length;
                    else
                        val = 0;
                    return (true, val); //없었음
                }

                else
                {
                    src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "프로그램 수정 요함 '{0}' is unknown ValueType in the Method RecursiveValue.Run()", eValueType);
                    return (false, val);
                }
            }
            else if (eCalcMethod == EnumCalcMethod.Plus)
            {

                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);


                if (val1.GetType() == typeof(string) && val2.GetType() == typeof(string))
                {
                    val = (string)val1 + (string)val2;
                }
                else if (IsFloatingPointValue(val1) || IsFloatingPointValue(val2))
                {
                    val = ObjectValue.ToDouble(val1) + ObjectValue.ToDouble(val2);
                }
                else
                {
                    val = ObjectValue.ToLong(val1) + ObjectValue.ToLong(val2);
                }
            }
            else if (eCalcMethod == EnumCalcMethod.Minus)
            {

                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);

                if (IsFloatingPointValue(val1) || IsFloatingPointValue(val2))
                    val = ObjectValue.ToDouble(val1) - ObjectValue.ToDouble(val2);
                else
                    val = ObjectValue.ToLong(val1) - ObjectValue.ToLong(val2);
            }
            else if (eCalcMethod == EnumCalcMethod.Multiply)
            {

                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);

                if (IsFloatingPointValue(val1) || IsFloatingPointValue(val2))
                    val = ObjectValue.ToDouble(val1) * ObjectValue.ToDouble(val2);
                else
                    val = ObjectValue.ToLong(val1) * ObjectValue.ToLong(val2);
            }
            else if (eCalcMethod == EnumCalcMethod.Divide)
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);

                if (IsFloatingPointValue(val1) || IsFloatingPointValue(val2))
                    val = ObjectValue.ToDouble(val1) / ObjectValue.ToDouble(val2);
                else
                    val = ObjectValue.ToLong(val1) / ObjectValue.ToLong(val2);
            }
            else if (eCalcMethod == EnumCalcMethod.Remainder)
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);

                if (IsFloatingPointValue(val1) || IsFloatingPointValue(val2))
                    val = ObjectValue.ToDouble(val1) % ObjectValue.ToDouble(val2);
                else
                    val = ObjectValue.ToLong(val1) % ObjectValue.ToLong(val2);
            }
            else if (eCalcMethod == EnumCalcMethod.And)
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);

                // C#에서 정수형 연산만 된다.
                val = (uint)ObjectValue.ToDouble(val1) & (uint)ObjectValue.ToDouble(val2);
            }
            else if (eCalcMethod == EnumCalcMethod.Or)
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);

                // C#에서 정수형 연산만 된다.
                val = (uint)ObjectValue.ToDouble(val1) | (uint)ObjectValue.ToDouble(val2);
            }
            else if (eCalcMethod == EnumCalcMethod.Xor)
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);

                // C#에서 정수형 연산만 된다.
                val = (uint)ObjectValue.ToDouble(val1) ^ (uint)ObjectValue.ToDouble(val2);
            }
            else if (eCalcMethod == EnumCalcMethod.LeftShift)
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);

                if (val1.GetType() == typeof(sbyte))
                    val = ObjectValue.ToSbyte(val1) << ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(byte))
                    val = ObjectValue.ToByte(val1) << ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(short))
                    val = ObjectValue.ToShort(val1) << ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(ushort))
                    val = ObjectValue.ToUshort(val1) << ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(int))
                    val = ObjectValue.ToInt(val1) << ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(uint))
                    val = ObjectValue.ToUint(val1) << ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(long))
                    val = ObjectValue.ToLong(val1) << ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(ulong))
                    val = ObjectValue.ToUlong(val1) << ObjectValue.ToInt(val2);
                else
                    val = ObjectValue.ToInt(val1) << ObjectValue.ToInt(val2);
            }
            else if (eCalcMethod == EnumCalcMethod.RightShift)
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val);

                var (success2, val2) = await rightVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success2) return (false, val);

                if (val1.GetType() == typeof(sbyte))
                    val = ObjectValue.ToSbyte(val1) >> ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(byte))
                    val = ObjectValue.ToByte(val1) >> ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(short))
                    val = ObjectValue.ToShort(val1) >> ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(ushort))
                    val = ObjectValue.ToUshort(val1) >> ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(int))
                    val = ObjectValue.ToInt(val1) >> ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(uint))
                    val = ObjectValue.ToUint(val1) >> ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(long))
                    val = ObjectValue.ToLong(val1) >> ObjectValue.ToInt(val2);
                else if (val1.GetType() == typeof(ulong))
                    val = ObjectValue.ToUlong(val1) >> ObjectValue.ToInt(val2);
                else
                    val = ObjectValue.ToInt(val1) >> ObjectValue.ToInt(val2);
            }
            else if (eCalcMethod == EnumCalcMethod.Parenthesis) // ()로 쌓여져 있는 경우
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);
                val = val1;
            }
            else if (eCalcMethod == EnumCalcMethod.Cast) // (cast)value 형태
            {
                // 만들어진 값을 cast 할 필요가 있다.
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);

                if (sLastValue == "sbyte")
                    val = ObjectValue.ToSbyte(val1);
                else if (sLastValue == "byte")
                    val = ObjectValue.ToByte(val1);
                else if (sLastValue == "char")
                    val = ObjectValue.ToChar(val1);
                else if (sLastValue == "short")
                    val = ObjectValue.ToShort(val1);
                else if (sLastValue == "ushort")
                    val = ObjectValue.ToUshort(val1);
                else if (sLastValue == "int")
                    val = ObjectValue.ToInt(val1);
                else if (sLastValue == "uint")
                    val = ObjectValue.ToUint(val1);
                else if (sLastValue == "long")
                    val = ObjectValue.ToLong(val1);
                else if (sLastValue == "ulong")
                    val = ObjectValue.ToUlong(val1);
                else if (sLastValue == "float")
                    val = ObjectValue.ToFloat(val1);
                else if (sLastValue == "double")
                    val = ObjectValue.ToDouble(val1);
                else if (sLastValue == "bool")
                    val = ObjectValue.ToDouble(val1) == 0 ? 0 : 1;     // 0이면 0  그 이외(음수포함)는 1
                else
                {

                }
            }
            else if (eCalcMethod == EnumCalcMethod.PrefixIncrement) // ++x
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);

                object d;
                if (IsFloatingPointValue(val1))
                    d = ObjectValue.ToDouble(val1) + 1;
                else
                    d = ObjectValue.ToLong(val1) + 1;

                var changeSuccess = await leftVal.ChangeVariableAsync(src, class_stack, parent_stack, d).ConfigureAwait(false);
                if (!changeSuccess) return (false, val);
                val = d;
            }
            else if (eCalcMethod == EnumCalcMethod.PrefixDecrement) // --x
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);

                object d;
                if (IsFloatingPointValue(val1))
                    d = ObjectValue.ToDouble(val1) - 1;
                else
                    d = ObjectValue.ToLong(val1) - 1;

                var changeSuccess = await leftVal.ChangeVariableAsync(src, class_stack, parent_stack, d).ConfigureAwait(false);
                if (!changeSuccess) return (false, val);
                val = d;
            }
            else if (eCalcMethod == EnumCalcMethod.PostfixIncrement) // x++
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);

                object d;
                if (IsFloatingPointValue(val1))
                    d = ObjectValue.ToDouble(val1) + 1;
                else
                    d = ObjectValue.ToLong(val1) + 1;

                var changeSuccess = await leftVal.ChangeVariableAsync(src, class_stack, parent_stack, d).ConfigureAwait(false);
                if (!changeSuccess) return (false, val);
                val = d;
            }
            else if (eCalcMethod == EnumCalcMethod.PostfixDecrement) // x--
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);

                object d;
                if (IsFloatingPointValue(val1))
                    d = ObjectValue.ToDouble(val1) - 1;
                else
                    d = ObjectValue.ToLong(val1) - 1;

                var changeSuccess = await leftVal.ChangeVariableAsync(src, class_stack, parent_stack, d).ConfigureAwait(false);
                if (!changeSuccess) return (false, val);
                val = d;
            }
            else if (eCalcMethod == EnumCalcMethod.UnaryPlus)   // +x
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);

                val = val1;
            }
            else if (eCalcMethod == EnumCalcMethod.UnaryMinus)  // -x
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);

                if (IsFloatingPointValue(val1))
                    val = -ObjectValue.ToDouble(val1);
                else
                    val = -ObjectValue.ToLong(val1);
            }
            else if (eCalcMethod == EnumCalcMethod.UnaryNegation)   // !x
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);

                val = !(ObjectValue.ToDouble(val1) == 1 ? true : false);
            }
            else if (eCalcMethod == EnumCalcMethod.UnaryBitwiseComplement)  // ~x
            {
                var (success1, val1) = await leftVal.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success1) return (false, val1);

                val = ~((uint)ObjectValue.ToDouble(val1));
            }

            return (true, val);
        }

        public async Task<bool> ChangeVariableAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack, object val)
        {
            List<int[]> dimensional_pos = null;

            // 먼저 Dimension이 있으면 계산해 둔다. 아래에서 반드시 사용한다.
            if (pJaggedArrays != null)
            {
                dimensional_pos = new List<int[]>();
                var pJaggedSuccess = await pJaggedArrays.RunAsync(src, class_stack, parent_stack, dimensional_pos).ConfigureAwait(false);
                if (!pJaggedSuccess) return false;
            }

            if (eValueType == EnumLastValueType.ExternalVariable)
            {
                if ((pExternalVariable = await  src.slmain.scriptExternal.ChangeVariable(src, pExternalVariable, sLastValue, val)) == null)
                {
                    src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, src.slmain.scriptExternal.ErrorType, src.slmain.scriptExternal.ErrorMessage);
                    return false;
                }
            }
            else if (eValueType == EnumLastValueType.Variable)
            {
                if (parent_stack.SetValue(class_stack, sLastValue, dimensional_pos, val))
                    return true;

                src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "Cannot find the variable '{0}' in ChangeVariable", sLastValue);
                return false;
            }
            else if (eValueType == EnumLastValueType.StaticVariable)
            {
                if (pRunPublic == null)
                {
                    pRunPublic = src.slmain.GetStaticVariablePointer(src.slmain, sCompiledNamespace, sCompiledClass, sCompiledMethod);
                }

                return pRunPublic.SetStaticValue(src, val);
            }
            else if (eValueType == EnumLastValueType.AllocVariable)
            {
                ItemDataStack ids;
                if (!parent_stack.GetItemPointer(sCompiledClass, out ids))
                {
                    src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "{0}.{1}에서 선언된 {0} 변수를 찾을 수 없습니다.", sCompiledClass, sCompiledMethod);
                    return false;
                }

                ClassDataStack stack = (ClassDataStack)ids.value;

                if (stack.SetValue(null, sCompiledMethod, null, val))
                    return true;

                src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "{0}.{1}에서 선언된 {1} 변수를 찾을 수 없습니다.", sCompiledClass, sCompiledMethod);
                return false;
            }
            else
            {
                src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The left-hand side of an assignment must be a variable, property or indexer {0}", sLastValue);
                return false;
            }

            return true;
        }

        public void Load(byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (block.type == EnumBlockType.ColRow)
                {
                    block.ReadColRow(out nColumn, out nRow);
                }
                else if (block.type == EnumBlockType.LastValueProperty)
                {
                    block.ReadLastValueProperty(out eCalcMethod, out eValueType, out eConstantType);
                }
                /*
                else if (block.type == EnumBlockType.eCalcMethod)
                {
                    eCalcMethod = (EnumCalcMethod)block.ReadByte();
                }
                else if (block.type == EnumBlockType.eValueType)
                {
                    eValueType = (EnumLastValueType)block.ReadByte();
                }
                else if (block.type == EnumBlockType.eVarType)
                {
                    eConstantType = (EnumVarType)block.ReadByte();
                }*/
                else if (block.type == EnumBlockType.sLastValue)
                {
                    sLastValue = block.ReadString();
                }
                else if (block.type == EnumBlockType.NameSplitNamespace)
                {
                    sCompiledNamespace = block.ReadString();
                }
                else if (block.type == EnumBlockType.NameSplitClass)
                {
                    sCompiledClass = block.ReadString();
                }
                else if (block.type == EnumBlockType.NameSplitMethod)
                {
                    sCompiledMethod = block.ReadString();
                }
                else if (block.type == EnumBlockType.CommandMethod)
                {
                    pMethod = new CommandMethod(parentMethod, parentBlock);
                    pMethod.Load(block.block_data, ref line);
                }
                else if (block.type == EnumBlockType.SourceString)
                {
                    sSourceString = block.ReadString();
                }
                else if (block.type == EnumBlockType.RecursiveValue)
                {
                    if (leftVal == null)
                    {
                        leftVal = new RecursiveValue(parentMethod, parentBlock);
                        leftVal.Load(block.block_data, ref line);
                    }
                    else if (rightVal == null)
                    {
                        rightVal = new RecursiveValue(parentMethod, parentBlock);
                        rightVal.Load(block.block_data, ref line);
                    }
                }
                else if (block.type == EnumBlockType.InitialValue)
                {
                    pInitial = new InitialValue(parentMethod, parentBlock);
                    pInitial.Load(block.block_data, ref line);
                }
                else if (block.type == EnumBlockType.DimensionalPosition)
                {
                    pJaggedArrays = new JaggedArrays(parentMethod, parentBlock);
                    pJaggedArrays.Load(block.block_data, ref line);
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                    //return;
                }
            }
        }

        string ConvertToEscapeString(string source)
        {
            StringBuilder s = new StringBuilder();

            // escape 문자는 ', ", \, 0, a, b, f, n, r, t, u, U, x, v  의 14개가 있다.

            for (int i = 0; i < source.Length; i++)
            {
                
                if(source[i] == '\"')
                    s.Append("\\\"");
                //else if(source[i] == '\'')        // 문자열속에서는 ' 문자는 Escape를 하지 않아도 된다.   (""속에는 "'" "\'" 둘다 사용가능하고 '' 안에서는 반드시 '\'' 와 같이 사용하여야 한다.)
                //    s.Append("\\\'");
                else if (source[i] == '\\')
                    s.Append("\\\\");
                else if (source[i] == '\0')
                    s.Append("\\0");
                else if (source[i] == '\a')
                    s.Append("\\a");
                else if (source[i] == '\b')
                    s.Append("\\b");
                else if (source[i] == '\f')
                    s.Append("\\f");
                else if (source[i] == '\n')
                    s.Append("\\n");
                else if (source[i] == '\r')
                    s.Append("\\r");
                else if (source[i] == '\t')
                    s.Append("\\t");
                else if (source[i] == '\v')
                    s.Append("\\v");
                else
                    s.Append(source[i]);
            }

            return s.ToString();
        }

        public string MakeDecompiledFile(int depth, string vartype)
        {
            StringBuilder sb = new StringBuilder();

            if (eCalcMethod == EnumCalcMethod.LastValue)
            {
                if (eValueType == EnumLastValueType.NewClass)
                {
                    sb.Append("new ");
                    sb.Append(pMethod.MakeDecompiledFile(depth, false));
                }
                else if (eValueType == EnumLastValueType.InitArray)
                {
                    sb.Append(pInitial.MakeDecompiledFile(depth));
                }
                else if (eValueType == EnumLastValueType.NewArray)
                {
                    //sb.Append("new "); new int[] 형식을 추가해야 한다.
                    sb.Append("new int"); // 일단 임으로 추가했다.
                    //sb.Append(vartype);

                    sb.Append(pJaggedArrays.MakeDecompiledFile(depth));

                    if (pInitial != null)
                        sb.Append(pInitial.MakeDecompiledFile(depth));
                }
                else if (eValueType == EnumLastValueType.Constant)
                {
                    if (eConstantType == EnumVarType.TypeString)
                    {
                        //sb.Append("\"" + sLastValue + "\"");
                        sb.Append("\"" + ConvertToEscapeString(sLastValue) + "\"");
                    }
                    else if (eConstantType == EnumVarType.TypeBool)
                    {
                        if (sLastValue == "1")
                            sb.Append("true");
                        else
                            sb.Append("false");
                    }
                    else
                    {
                        sb.Append(sLastValue);
                    }
                }
                else if (eValueType == EnumLastValueType.Variable)
                {
                    sb.Append(sLastValue);
                    if (pJaggedArrays != null)
                        sb.Append(pJaggedArrays.MakeDecompiledFile(depth));
                }
                else
                {
                    sb.Append(sLastValue);
                }
            }
            else if (eCalcMethod == EnumCalcMethod.Parenthesis)
            {
                sb.Append("(");
                sb.Append(leftVal.MakeDecompiledFile(depth));
                sb.Append(")");
            }
            else if (eCalcMethod == EnumCalcMethod.Cast)
            {
                sb.Append("(");
                sb.Append(sLastValue);
                sb.Append(")");
                sb.Append(leftVal.MakeDecompiledFile(depth));
            }
            else if (eCalcMethod == EnumCalcMethod.PrefixIncrement)
            {
                sb.Append("++");
                sb.Append(leftVal.MakeDecompiledFile(depth));
            }
            else if (eCalcMethod == EnumCalcMethod.PrefixDecrement)
            {
                sb.Append("--");
                sb.Append(leftVal.MakeDecompiledFile(depth));
            }
            else if (eCalcMethod == EnumCalcMethod.PostfixIncrement)
            {
                sb.Append(leftVal.MakeDecompiledFile(depth));
                sb.Append("++");
            }
            else if (eCalcMethod == EnumCalcMethod.PostfixDecrement)
            {
                sb.Append(leftVal.MakeDecompiledFile(depth));
                sb.Append("--");
            }
            else if (eCalcMethod == EnumCalcMethod.UnaryPlus)
            {
                sb.Append("+");
                sb.Append(leftVal.MakeDecompiledFile(depth));
            }
            else if (eCalcMethod == EnumCalcMethod.UnaryMinus)
            {
                sb.Append("-");
                sb.Append(leftVal.MakeDecompiledFile(depth));
            }
            else if (eCalcMethod == EnumCalcMethod.UnaryNegation)
            {
                sb.Append("!");
                sb.Append(leftVal.MakeDecompiledFile(depth));
            }
            else if (eCalcMethod == EnumCalcMethod.UnaryBitwiseComplement)
            {
                sb.Append("~");
                sb.Append(leftVal.MakeDecompiledFile(depth));
            }
            else
            {
                sb.Append(leftVal.MakeDecompiledFile(depth));
                sb.Append(" ");

                if (eCalcMethod == EnumCalcMethod.And)
                    sb.Append("&");
                else if (eCalcMethod == EnumCalcMethod.Divide)
                    sb.Append("/");
                else if (eCalcMethod == EnumCalcMethod.Minus)
                    sb.Append("-");
                else if (eCalcMethod == EnumCalcMethod.Multiply)
                    sb.Append("*");
                else if (eCalcMethod == EnumCalcMethod.Or)
                    sb.Append("|");
                else if (eCalcMethod == EnumCalcMethod.Plus)
                    sb.Append("+");
                else if (eCalcMethod == EnumCalcMethod.Remainder)
                    sb.Append("%");
                else if (eCalcMethod == EnumCalcMethod.Xor)
                    sb.Append("^");
                else if (eCalcMethod == EnumCalcMethod.LeftShift)
                    sb.Append("<<");
                else if (eCalcMethod == EnumCalcMethod.RightShift)
                    sb.Append(">>");
                else
                    sb.Append("????");

                sb.Append(" ");
                sb.Append(rightVal.MakeDecompiledFile(depth));
            }

            return sb.ToString();
        }

        public string MakeDecompiledFile(int depth)
        {
            return MakeDecompiledFile(depth, "");
        }
    }
}
