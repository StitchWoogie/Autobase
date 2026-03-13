using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetTools;
using System.Collections;
using ScriptLibRun.Debugger;

namespace ScriptLibRun
{
    public class ScriptLibMain
    {
        public const int nTabSpace = 4;  // 탭 하나가 차지하는 공간

        public bool bErrorFlag = false;
        public List<ErrorMessageItem> arrayError = new List<ErrorMessageItem>();
        public ScriptExternalClass scriptExternal = null;

        public List<ScriptLibLibrary> arrayLibrary = new List<ScriptLibLibrary>();

        public static bool bDebugMode = false;

        //public static ScriptLibMain scriptMain = null;   // edit 모드나 run모드에서 ScriptLibMain을 할당하고 이 포인트를 연결하여 스크립트 전체에서 사용할 수 있도록 한다.

        public ScriptLibMain()
        {
            // 2022-2-23 10.3.5.21 쓰레드에서 사용하면 main을 static으로 선언할 때 arrayError가 공유되어서 오류가 난다. scriptLibMain을 static으로 선언하고 static 선언시에 Prepare()를 호출하고 
            // 다른 프로세서에서는 scriptExternal과 arrayLibrary를 참조하여 사용한다. 각 스크립트에서 계속호출하면 초기화 시간이 많이 걸린다.

            //PrepareLibrary.Prepare(this);

            //DebuggerRunService.procSetBreakPoint = new DebuggerRunService.DelegateSetBreakPoint(CallbackSetBreakPoint);
        }

        public void Prepare()
        {
            PrepareLibrary.Prepare(this);

            DebuggerRunService.procSetBreakPoint = new DebuggerRunService.DelegateSetBreakPoint(CallbackSetBreakPoint);
        }

        void CallbackSetBreakPoint(string source_file, int y)
        {
            List<int> array = new List<int>();
            array.Add(y);

            for (int i = 0; i < arrayLibrary.Count; i++)
            {
                arrayLibrary[i].SetBreakPoints(source_file, array);
            }
        }

        void AddMessage(bool berror, string projectname, string filename, int col_pos, int row_pos, EnumScriptErrorType et, string format, params object[] args)
        {
            ErrorMessageItem error = new ErrorMessageItem();
            error.bError = berror;
            error.col = col_pos;
            error.row = row_pos;
            error.sourcefile = filename;
            error.projectname = projectname;
            error.et = et;

            if (args.Length == 0)
                error.message = format;
            else
                error.message = String.Format(format, args);

            arrayError.Add(error);
        }

        public void SetError(string projectname, string filename, int col_pos, int row_pos, EnumScriptErrorType et, string format, params object[] args)
        {
            bErrorFlag = true;
            AddMessage(true, projectname, filename, col_pos, row_pos, et, format, args);
        }

        public void ClearError()
        {
            bErrorFlag = false;
            arrayError.Clear();
        }

        public void SetWarning(string projectname, string filename, int col_pos, int row_pos, EnumScriptErrorType et, string format, params object[] args)
        {
            AddMessage(false, projectname, filename, col_pos, row_pos, et, format, args);
        }

        public void LoadAllLibrary(string dir)
        {
            if (!Directory.Exists(dir)) return;

            DirectoryInfo di = new DirectoryInfo(dir);

            foreach (FileInfo fi in di.GetFiles("*.objx"))
            {
                LoadOneLibrary(fi.FullName);
            }

            PrepareBreakPoints(dir);
        }

        public static List<int> LoadBreakPoints(string sourcefile)
        {
            List<int> array = new List<int>();

            string filename = Path.ChangeExtension(sourcefile, "BreakPoints");

            if (!File.Exists(filename)) return array;
            TextReader reader = new StreamReader(filename);

            string one_line;
            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                array.Add(ConvertTool.ToInt32(one_line));
            }

            reader.Close();

            return array;
        }

        void PrepareBreakPoints(string dir)
        {
            if (!bDebugMode) return;            // Debug 모드일때만 BreakPoint를 준비한다.

            List<int> array = new List<int>();

            string filename = String.Format("{0}\\MyProject.BreakPoints", dir);

            if (!File.Exists(filename)) return;
            TextReader reader = new StreamReader(filename);

            string one_line;
            CommaBlockString comma = new CommaBlockString();
            string source_file;

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                comma.Set(one_line);
                source_file = comma.GetString();

                array = new List<int>();

                while (true)
                {
                    if (comma.IsEOS()) break;
                    array.Add(comma.GetInt());
                }

                for (int i = 0; i < arrayLibrary.Count; i++)
                {
                    arrayLibrary[i].SetBreakPoints(source_file, array);
                }
            }

            reader.Close();
        }

        public ScriptLibLibrary LoadOneLibrary(string filename)
        {
            ScriptLibLibrary sll = new ScriptLibLibrary();
            sll.sNameLibrary = Path.GetFileNameWithoutExtension(filename);
            arrayLibrary.Add(sll);

            int line = 1;

            byte[] buffer = File.ReadAllBytes(filename);

            ScriptReader reader = new ScriptReader(buffer);

            while (true)
            {
                var block = reader.ReadBlock();

                if (block == null) break;

                if (block.type == EnumBlockType.LibraryBlock)
                {
                    sll.Load(filename, block.block_data, ref line);
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                    //return;
                }
            }

            reader.Close();

            return sll;
        }
        
        public ScriptLibClass GetClassPointer(string name_namespace, string name_class)
        {
            ScriptLibLibrary sll;
            ScriptLibNamespace sln;
            ScriptLibClass slc;

            for (int l = 0; l < arrayLibrary.Count; l++)
            {
                sll = arrayLibrary[l];

                for (int i = 0; i < sll.arrayNamespace.Count; i++)
                {
                    sln = sll.arrayNamespace[i];
                    if (sln.sNameNamespace == name_namespace)
                    {
                        for (int j = 0; j < sln.arrayMember.Count; j++)
                        {
                            if (sln.arrayMember[j].eMember == EnumScriptLibMember.Class)
                            {
                                slc = (ScriptLibClass)sln.arrayMember[j];
                                if (slc.sNameClass == name_class)
                                {
                                    return slc;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        // 명확한 경로가 있을 때 사용
        public ScriptLibMemberMethod GetMethodPointer(ScriptLibMain slmain, string project_name, string source_file, int col_pos, int row_pos, string name_namespace, string name_class, string name_method, List<CommandMethodArg> args)
        {
            ScriptLibLibrary sll;
            ScriptLibNamespace sln;
            ScriptLibClass slc;
            ScriptLibMemberMethod slm;

            for (int l = 0; l < arrayLibrary.Count; l++)
            {
                sll = arrayLibrary[l];
                for (int i = 0; i < sll.arrayNamespace.Count; i++)
                {
                    sln = sll.arrayNamespace[i];
                    if (sln.sNameNamespace == name_namespace)
                    {
                        for (int j = 0; j < sln.arrayMember.Count; j++)
                        {
                            if (sln.arrayMember[j].eMember == EnumScriptLibMember.Class)
                            {
                                slc = (ScriptLibClass)sln.arrayMember[j];
                                if (slc.sNameClass == name_class)
                                {
                                    slm = slc.GetMethodPointer(slmain, project_name, source_file, col_pos, row_pos, name_method, args);

                                    if (slm != null) return slm;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        // 명확한 경로가 있을 때 사용
        public ScriptLibMemberMethod GetPropertyPointer(string name_namespace, string name_class, string name_method)
        {
            ScriptLibLibrary sll;
            ScriptLibNamespace sln;
            ScriptLibClass slc;
            ScriptLibMemberMethod slm;

            for (int l = 0; l < arrayLibrary.Count; l++)
            {
                sll = arrayLibrary[l];
                for (int i = 0; i < sll.arrayNamespace.Count; i++)
                {
                    sln = sll.arrayNamespace[i];
                    if (sln.sNameNamespace == name_namespace)
                    {
                        for (int j = 0; j < sln.arrayMember.Count; j++)
                        {
                            if (sln.arrayMember[j].eMember == EnumScriptLibMember.Class)
                            {
                                slc = (ScriptLibClass)sln.arrayMember[j];
                                if (slc.sNameClass == name_class)
                                {
                                    slm = slc.GetPropertyPointer(name_method);

                                    if (slm != null) return slm;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 클래스의 static 변수나 Enum의 멤버 포인트를 가져온다.
        /// </summary>
        /// <param name="name_namespace"></param>
        /// <param name="name_class"></param>
        /// <param name="name_method"></param>
        /// <returns></returns>
        public ScriptLibMemberPublic GetStaticVariablePointer(ScriptLibMain slmain, string name_namespace, string name_class, string name_method)
        {
            ScriptLibLibrary sll;
            ScriptLibNamespace sln;

            for (int l = 0; l < arrayLibrary.Count; l++)
            {
                sll = arrayLibrary[l];
                for (int i = 0; i < sll.arrayNamespace.Count; i++)
                {
                    sln = sll.arrayNamespace[i];
                    if (sln.sNameNamespace == name_namespace)
                    {
                        for (int j = 0; j < sln.arrayMember.Count; j++)
                        {
                            if (sln.arrayMember[j].eMember == EnumScriptLibMember.Class)
                            {
                                ScriptLibClass slc;
                                slc = (ScriptLibClass)sln.arrayMember[j];
                                if (slc.sNameClass == name_class)
                                {
                                    return slc.GetVariablePointer(slmain, name_method);
                                }
                            }
                            else // Enum
                            {
                                ScriptLibEnum slb;
                                slb = (ScriptLibEnum)sln.arrayMember[j];
                                if (slb.sNameEnum == name_class)
                                {
                                    return slb.GetItemPointer(name_method);
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /*
        public bool RunWithError(string name_namespace, string name_class, string name_method, ClassDataStack class_stack, out object retnvalue, object[] args)
        {
            ScriptLibClass slc = GetClassPointer(name_namespace, name_class);

            if (slc != null)
            {
                ScriptRunConfiguration src = new ScriptRunConfiguration();
                //src.main = this;
                return slc.Run(src, class_stack, name_method, out retnvalue, args);
            }

            SetError("project", "filename", 0, 0, "{0}.{1}.{2} Method를 찾을 수 없습니다.", name_namespace, name_class, name_method);

            retnvalue = 0;

            return false;
        }*/
    }
}
