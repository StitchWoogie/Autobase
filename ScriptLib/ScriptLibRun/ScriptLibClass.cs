using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using ScriptLibRun.Debugger;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class ScriptBaseClass
    {
        public string name;
        public string sCompiledNamespace;
        public string sCompiledClass;
        public int nColumn;
        public int nRow;
        public ScriptLibClass pSlc;

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            writer.WriteNameSplit(sCompiledNamespace, sCompiledClass);

            parent_writer.WriteBlock(EnumBlockType.BaseClassBlock, writer);
        }

        public void Load(byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (block.type == EnumBlockType.NameSplitNamespace)
                {
                    sCompiledNamespace = block.ReadString();
                }
                else if (block.type == EnumBlockType.NameSplitClass)
                {
                    sCompiledClass = block.ReadString();
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                }
            }
        }
    }

    public class ScriptLibClass : ScriptLibMemberPublic
    {
        public bool bPublic = false;    // class는 public 이거나 internal 둘 중에 하나만 가능하다.
        public bool bPartial = false;   // 소스파일이 분리된 클래스
        public bool bStruct = false;    // true이면 struct이다
        //public string sSourceFilename;// 소스 파일명
        public string sNameClass;       // 클래스 명

        public List<ScriptLibMemberMethod> arrayMethod = new List<ScriptLibMemberMethod>();
        public List<ScriptLibMemberVariable> arrayVariable = new List<ScriptLibMemberVariable>();
        public ScriptLibNamespace parentNamespace;

        public List<ScriptBaseClass> arrayParentClass = null; // 상속받을 부모 클래스 struct 에서는 사용하지 않으므로 기본값을 null로 한다. 다중상속이 있으므로 List를 사용했다.
                
        public ScriptLibClass(ScriptLibNamespace parent_namespace)
        {
            eMember = EnumScriptLibMember.Class;
            parentNamespace = parent_namespace;
        }

        public override string GetName()
        {
            return sNameClass;
        }

        // 이코드는 Edit엔진에서만 필요한데 오토베이스 이전 스크립트와 호환을 위해서 만들었다. 11버전에서는 빼고 Edit에서만 만들것
        // 실제저장은 EditScriptLibClass에서만 한다.
        public virtual void SaveToStream(ScriptWriter writer, int tab_depth, bool write_version)
        {

        }

        public async Task PrepareDataStack(ScriptRunConfiguration src, ClassDataStack data)
        {
            ClassDataStack parent_stack = new ClassDataStack(null);

            for (int i = 0; i < arrayVariable.Count; i++)
            {
                ItemDataStack ids = new ItemDataStack();

                if (arrayVariable[i].bStatic) continue;     // static 변수는 데이타 스택에 준비할 필요가 없다.
                if (arrayVariable[i].pVar.bConst) continue; // const 변수는 데이타 스택에 준비할 필요가 없다.

                object value = null;
                bool success;

                if (arrayVariable[i].pValue != null)
                {
                    //if (!arrayVariable[i].pValue.Run(src, data, null, out value)) ;
                    //arrayVariable[i].pValue.Run(src, data, null, out value);
                    (success, value) = await arrayVariable[i].pValue.RunAsync(src, data, parent_stack).ConfigureAwait(false);    // Class variable은 parent_stack을 null을 주면 아래의 Run에서 null 오류가 발생할 수 있으므로 
                                                                                                           // 비어있는 parent_stack를 전송한다.
                }

                data.AddItem(arrayVariable[i].pVar, value);

                /*
                ids.pVar.sVarType = arrayVariable[i].pVar.sVarType;
                ids.pVar.sVarName = arrayVariable[i].pVar.sVarName;
                data.arrayData.Add(ids);*/
            }

            if (arrayParentClass != null)
            {
                for (int i = 0; i < arrayParentClass.Count; i++)
                {
                    if (arrayParentClass[i].pSlc == null)
                    {
                        arrayParentClass[i].pSlc = src.slmain.GetClassPointer(arrayParentClass[i].sCompiledNamespace, arrayParentClass[i].sCompiledClass);
                    }

                   await arrayParentClass[i].pSlc.PrepareDataStack(src, data).ConfigureAwait(false);
                } 
            }
        }

        public async Task<(bool success, object returnValue)> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, string start_method, object[] args)
        {
            ScriptLibMemberMethod slm;

            for (int i = 0; i < arrayMethod.Count; i++)
            {
                slm = arrayMethod[i];

                if (start_method == slm.sNameMethod)
                {
                    var (success, returnValue) = await slm.RunAsync(src, class_stack, args).ConfigureAwait(false);

                    if (!success)
                    {
                        return (false, null);
                    }

                    return (true, returnValue);
                }
            }
            return (true, 0);
        }

        public static void ShowErrorLoadUnknownCommand(string class_name, ScriptReaderBlock block, int line)
        {
            string msg;
            msg = String.Format("Line-{1} Unknown Command : {0} in the Class={2} ", block.type, line, class_name);

            System.Windows.Forms.MessageBox.Show(msg, "Load Error");
        }

        public ScriptLibMemberMethod GetPropertyPointer(string name)
        {
            for (int i = 0; i < arrayMethod.Count; i++)
            {
                if (name == arrayMethod[i].sNameMethod)
                    return arrayMethod[i];
            }

            return null;
        }

        public ScriptLibMemberPublic GetMethodVariablePointer(string name)
        {
            for (int i = 0; i < arrayMethod.Count; i++)
            {
                if (name == arrayMethod[i].sNameMethod)
                    return arrayMethod[i];
            }

            for (int i = 0; i < arrayVariable.Count; i++)
            {
                if (name == arrayVariable[i].pVar.sVarName)
                    return arrayVariable[i];
            }

            return null;
        }

        // 컴파일 시에도 이함수를 사용한다.
        public ScriptLibMemberVariable GetVariablePointer(ScriptLibMain slmain, string name)
        {
            for (int i = 0; i < arrayVariable.Count; i++)
            {
                if (name == arrayVariable[i].pVar.sVarName)
                    return arrayVariable[i];
            }

            if (arrayParentClass != null)
            {
                for (int i = 0; i < arrayParentClass.Count; i++)
                {
                    if (arrayParentClass[i].pSlc == null)
                    {
                        arrayParentClass[i].pSlc = slmain.GetClassPointer(arrayParentClass[i].sCompiledNamespace, arrayParentClass[i].sCompiledClass);
                    }

                    ScriptLibMemberVariable slmv = arrayParentClass[i].pSlc.GetVariablePointer(slmain, name);

                    if (slmv != null)
                    {
                        return slmv;
                    }
                }
            }

            return null;
        }

        public ScriptLibMemberMethod GetMethodPointer(ScriptLibMain slmain, string project_name, string source_file, int col_pos, int row_pos, string name, List<CommandMethodArg> args)
        {
            int arg_count = 0;
            if (args != null)
                arg_count = args.Count;

            ScriptLibMemberMethod slmm1 = null;

            for (int i = 0; i < arrayMethod.Count; i++)
            {
                if (name == arrayMethod[i].sNameMethod)
                {
                    int min, max;
                    ScriptLibMemberMethod.GetMethodArgumentMinMax(arrayMethod[i].arrayParams, out min, out max);

                    if (arg_count >= min && arg_count <= max)
                        return arrayMethod[i];

                    slmm1 = arrayMethod[i];
                }
            }

            if (arrayParentClass != null)
            {
                for (int i = 0; i < arrayParentClass.Count; i++)
                {
                    if (arrayParentClass[i].pSlc == null)
                    {
                        arrayParentClass[i].pSlc = slmain.GetClassPointer(arrayParentClass[i].sCompiledNamespace, arrayParentClass[i].sCompiledClass);
                    }

                    ScriptLibMemberMethod slmm = arrayParentClass[i].pSlc.GetMethodPointer(slmain, project_name, source_file, col_pos, row_pos, name, args);

                    if (slmm != null)
                    {
                        return slmm;
                    }
                }
            }

            if (slmm1 != null)
            {
                slmain.SetError(project_name, source_file, col_pos, row_pos, EnumScriptErrorType.Else, "Check argument count of method '{0}'", name);
                return null;
            }

            slmain.SetError(project_name, source_file, col_pos, row_pos, EnumScriptErrorType.Else, "The method '{0}' could not be found in the '{1}' class", name, sNameClass);

            return null;
        }

        /*
        public ScriptLibMemberMethod GetMethodPointerOnRuntime(string name, List<CommandMethodArg> args)
        {
            int arg_count = 0;
            if (args != null)
                arg_count = args.Count;

            for (int i = 0; i < arrayMethod.Count; i++)
            {
                if (name == arrayMethod[i].sNameMethod)
                {
                    int min, max;
                    ScriptLibMemberMethod.GetMethodArgumentMinMax(arrayMethod[i].arrayParams, out min, out max);

                    if (arg_count >= min && arg_count <= max)
                        return arrayMethod[i];
                }
            }

            
            if (arrayParentClass != null)
            {
                for (int i = 0; i < arrayParentClass.Count; i++)
                {
                    if (arrayParentClass[i].pSlc == null)
                    {
                        arrayParentClass[i].pSlc = src.main.GetClassPointer(arrayParentClass[i].sCompiledNamespace, arrayParentClass[i].sCompiledClass);
                    }
                    
                    ScriptLibMemberMethod slmm = arrayParentClass[i].pSlc.GetMethodPointerOnRuntime(src, name, args);

                    if (slmm != null)
                    {
                        return slmm;
                    }
                }
            }

            return null;
        }
        
        public ScriptLibMemberMethod GetMethodPointerOnCompile(ScriptLibMain main, string source_file, string name, List<CommandMethodArg> args, int col_pos, int row_pos)
        {
            int arg_count = 0;
            if (args != null)
                arg_count = args.Count;

            ScriptLibMemberMethod slmm = null;

            for (int i = 0; i < arrayMethod.Count; i++)
            {
                if (name == arrayMethod[i].sNameMethod)
                {
                    int min, max;
                    ScriptLibMemberMethod.GetMethodArgumentMinMax(arrayMethod[i].arrayParams, out min, out max);

                    if (arg_count >= min && arg_count <= max)
                        return arrayMethod[i];

                    slmm = arrayMethod[i];
                }
            }

            if (slmm != null)
            {
                main.SetError(parentNamespace.parentLibrary.sNameLibrary, source_file, col_pos, row_pos, "메소드 호출 시 인자의 갯수가 다릅니다. {0}", slmm.MakeStringMethodUsing());
                return slmm;
            }

            if (arrayParentClass != null)
            {
                for (int i = 0; i < arrayParentClass.Count; i++)
                {
                    slmm = arrayParentClass[i].pSlc.GetMethodPointerOnCompile(main, source_file, name, args, col_pos, row_pos);

                    if (slmm != null)
                    {
                        return slmm;
                    }
                }
            }

            return null;
        }*/

        public override void SetBreakPoints(string filename, List<int> array)
        {
            for (int i = 0; i < arrayMethod.Count; i++)
            {
                arrayMethod[i].SetBreakPoints(filename, array);
            }
        }

        /// <summary>
        /// 앞쪽에 탭을 넣어서 소스를 보기좋게 정렬한다.
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void AppendWithTab(StringBuilder sb, int tab, string format, params object[] args)
        {
            if (tab > 0)
            {
                for (int i = 0; i < tab; i++)
                {
                    sb.Append('\t');
                }
            }
            string s = String.Format(format, args);
            sb.Append(s);
        }

        /// <summary>
        /// 앞쪽에 탭을 넣어서 소스를 보기좋게 정렬한다.
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void AppendLineWithTab(StringBuilder sb, int tab, string format, params object[] args)
        {
            AppendWithTab(sb, tab, format, args);
            sb.AppendLine();
        }

        public void Load(byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (block.type == EnumBlockType.Name)
                {
                    sNameClass = block.ReadString();
                }
                else if (block.type == EnumBlockType.Version)
                {
                    // 오브젝트 스크립트는 클래스만 따로 저장되어 있기 때문에 버전 정보가 있다.
                    int major, minor, build, revision;
                    block.ReadVersion(out major, out minor, out build, out revision);
                }
                else if (block.type == EnumBlockType.bStruct)
                {
                    bStruct = block.ReadBool();
                }
                else if (block.type == EnumBlockType.MethodBlock)
                {
                    ScriptLibMemberMethod slm = new ScriptLibMemberMethod(this);
                    slm.Load(block.block_data, ref line);
                    arrayMethod.Add(slm);
                }
                else if (block.type == EnumBlockType.ClassVariable)
                {
                    ScriptLibMemberVariable slv = new ScriptLibMemberVariable();
                    slv.Load(block.block_data, ref line);
                    arrayVariable.Add(slv);
                }
                else if (block.type == EnumBlockType.BaseClassBlock)
                {
                    ScriptBaseClass sbc = new ScriptBaseClass();
                    sbc.Load(block.block_data, ref line);

                    if (arrayParentClass == null)
                        arrayParentClass = new List<ScriptBaseClass>();

                    arrayParentClass.Add(sbc);
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                }
            }
        }

        public override string MakeDecompiledFile(int depth)
        {
            StringBuilder sb = new StringBuilder();
            if(bStruct)
                ScriptLibClass.AppendLineWithTab(sb, depth, String.Format("struct {0}", sNameClass));
            else
                ScriptLibClass.AppendLineWithTab(sb, depth, String.Format("class {0}", sNameClass));

            ScriptLibClass.AppendLineWithTab(sb, depth, "{{");

            for (int i = 0; i < arrayVariable.Count; i++)
            {
                sb.AppendLine(arrayVariable[i].MakeDecompiledFile(depth + 1));
            }

            if (arrayVariable.Count > 0 && arrayMethod.Count > 0)
                sb.AppendLine(); // 변수와 함수가 있으면 두 부분을 분리해 준다.

            for (int i = 0; i < arrayMethod.Count; i++)
            {
                if (i > 0) sb.AppendLine(); // 두번째 부터는 한칸 띄워준다.

                sb.AppendLine(arrayMethod[i].MakeDecompiledFile(depth+1));
            }

            ScriptLibClass.AppendWithTab(sb, depth, "}}");

            return sb.ToString();
        }

        
    }

    
}
