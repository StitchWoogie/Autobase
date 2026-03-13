using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class CommandMethod : CommandPublic
    {
        protected enum EnumMethodType
        {
            StaticMethod,   // Static Method를 다이렉트로 Call
            AllocMethod,    // 변수로 할당된 메소드
            ExternalMethod, // 스크립트 외부의 메소드
            SelfClassMethod,  // 자신의 클래스에 속한 메소드
        }

        public string sMethodName;
        public List<CommandMethodArg> args = null;
        protected EnumMethodType eMethod = EnumMethodType.StaticMethod;

        // 컴파일 할 때 각 위치를 찾아준다.
        public string sCompiledNamespace;
        public string sCompiledClass;
        public string sCompiledMethod;

        public CommandMethod(ScriptLibMemberMethod parent, CommandPublic parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_method;
        }

        ScriptLibClass pRunClass = null;
        ScriptLibMemberMethod pRunMethod = null;
        object pExternalMethod = null;

        public async Task<(bool success, object val)> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack, bool new_class)
        {
            object val = 0;

            // 호출할 Argument를 준비한다.
            object[] param = null;

            if (args != null)
            {
                // 여기서는 object형식으로 값을 가져오는 것이 맞다. 각종 계산식이 인자로 호출될 수 있다.
                param = new object[args.Count];
                for (int i = 0; i < args.Count; i++)
                {
                    var (argSuccess, argValue) = await args[i].RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                    if (!argSuccess) return (false, argValue);
                    param[i] = argValue;
                }
            }

            if (eMethod == EnumMethodType.ExternalMethod)
            {
                object extVal;
                (pExternalMethod, extVal) = await src.slmain.scriptExternal.RunMethodAsync(src, pExternalMethod, sMethodName, param, args).ConfigureAwait(false);
                val = extVal;

                if (pExternalMethod == null)
                {
                    src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary,
                parentMethod.sSourceFilename, nColumn, nRow,
                src.slmain.scriptExternal.ErrorType, src.slmain.scriptExternal.ErrorMessage);
                    return (false, val);
                }
            }
            else if (eMethod == EnumMethodType.SelfClassMethod) // 자신의 클래스에 속한 메소드
            {
                if (pRunMethod == null)
                    pRunMethod = parentMethod.parentClass.GetMethodPointer(src.slmain, parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, sCompiledMethod, args);

                var (methodSuccess, methodVal) = await pRunMethod.RunAsync(src, class_stack, param).ConfigureAwait(false);
                if (!methodSuccess) return (false, methodVal);
                val = methodVal;
            }
            else if (eMethod == EnumMethodType.AllocMethod) // ex.Method 처럼 변수에 연결된 메소드이다.
            {
                ItemDataStack val2;
                if (!parent_stack.GetItemPointer(sCompiledClass, out val2))
                {
                    src.slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary,
                   parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.VarNotFound,
                   "'{0}' 변수를 찾을 수 없습니다.", sCompiledClass);
                    return (false, val);
                }

                if (pRunMethod == null)
                    pRunMethod = src.slmain.GetMethodPointer(src.slmain, parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, val2.pVar.sCompiledNamespace, val2.pVar.sCompiledClass, sCompiledMethod, args);

                if (val2.value.GetType() == typeof(ClassDataStack))
                {
                    var (allocSuccess, allocVal) = await pRunMethod.RunAsync(src, (ClassDataStack)(val2.value), param).ConfigureAwait(false);
                    if (!allocSuccess) return (false, allocVal);
                    val = allocVal;
                }
                else
                {
                    // System.String일 경우 값이 문자열이 직접 대입되므로 ClassData스택이 아니다.
                    // 이경우는 s.ToLower() 와 같은 확장 함수이므로 내부에서 값을 참고할 수 있도록 selfValue에 대입해준다.
                    ClassDataStack cds = new ClassDataStack(null);
                    cds.selfValue = val2.value;

                    var (extSuccess, extVal) = await pRunMethod.RunAsync(src, cds, param).ConfigureAwait(false);
                    if (!extSuccess) return (false, extVal);
                    val = extVal;
                }
            }
            else // Static Method
            {
                if (pRunMethod == null)
                    pRunMethod = src.slmain.GetMethodPointer(src.slmain, parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, sCompiledNamespace, sCompiledClass, sCompiledMethod, args);

                if (new_class)
                {
                    // new Class() 로 할당된 메소드이므로 class_datastack을 새로 만들고
                    ClassDataStack new_class_stack = new ClassDataStack(null); // new Class() 로 호출된 함수이다.

                    if (pRunClass == null)
                        pRunClass = src.slmain.GetClassPointer(sCompiledNamespace, sCompiledClass);

                    await pRunClass.PrepareDataStack(src, new_class_stack).ConfigureAwait(false);    // 클래스를 만들고 클래스의 스택을 준비한다.

                    var (newSuccess, newVal) = await pRunMethod.RunAsync(src, new_class_stack, param).ConfigureAwait(false);
                    if (!newSuccess) return (false, newVal);

                    val = new_class_stack;  // retn 값을 할당된 클래스 스택을 지정한다.
                }
                else
                {
                    var (staticSuccess, staticVal) = await pRunMethod.RunAsync(src, class_stack, param).ConfigureAwait(false);
                    if (!staticSuccess) return (false, staticVal);
                    val = staticVal;
                }
            }

            // Out/Ref 의 경우는 값을 갱신해 준다.
            if (args != null)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    if (args[i].eInOut == EnumInOut.In) continue;   // 계산식의 인자도 이것에 속한다.
                    if (args[i].eInOut == EnumInOut.Params) continue;
                    if (args[i].eInOut == EnumInOut.OutParams)
                    {
                    }

                    var changeSuccess = await args[i].value.ChangeVariableAsync(src, class_stack, parent_stack, param[i]).ConfigureAwait(false);
                    if (!changeSuccess) return (false, val);
                }
            }
            return (true, val);
        }

        public void Load(byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (IsPublicLoadItem(block))
                {

                }
                else if (block.type == EnumBlockType.Name)
                {
                    sMethodName = block.ReadString();
                }
                else if (block.type == EnumBlockType.MethodType)
                {
                    eMethod = (EnumMethodType)block.ReadByte();
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
                else if (block.type == EnumBlockType.CommandMethodArg)
                {
                    if (args == null)
                        args = new List<CommandMethodArg>();

                    CommandMethodArg val = new CommandMethodArg(parentMethod, parentBlock);
                    val.Load(block.block_data, ref line);
                    args.Add(val);
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                    //return;
                }
            }
        }

        public override string MakeDecompiledFile(int depth, bool bAddSemicolon)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(sMethodName);
            sb.Append("(");
            if (args != null)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    sb.Append(args[i].MakeDecompiledFile(depth));
                    if (i < args.Count - 1)
                        sb.Append(", ");
                }
            }
            sb.Append(")");
            if(bAddSemicolon)
                sb.Append(";");
            
            return sb.ToString();
        }
    }
}
