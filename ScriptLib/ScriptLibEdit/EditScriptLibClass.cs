using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;

namespace ScriptLibEdit
{
    public class SplitedString
    {
        public int col_pos;
        public int row_pos;
        public string block;
    }

    public class EditScriptLibClass : ScriptLibClass
    {
        public EditScriptLibFile sourceFile = null;

        public EditScriptLibClass(ScriptLibNamespace parent, EditScriptLibFile file)
            : base(parent)
        {
            sourceFile = file;
        }
        
        bool AddMethod(EditScriptLibMain main, EditScriptLibFile file, EnumAccessLevel eAccess, bool bStatic, string retn_datatype, string method_name, SplitedString args_block, string body, SplitedString body_block)
        {
            if (body[body.Length - 1] == ';')
            {
                main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, args_block.col_pos, args_block.row_pos, EnumScriptErrorType.Else, "Must declare a body block of '{0}()' method", method_name);
                return false;
            }

            List<MethodArgument> args = new List<MethodArgument>();

            // 먼저 argument 갯수를 구한다.
            if (!CheckMethodArguments(main, file, args_block.block, 1, args_block.block.Length - 1, args_block.col_pos, args_block.row_pos, args)) return false;

            for (int i = 0; i < arrayMethod.Count; i++)
            {
                if (arrayMethod[i].sNameMethod == method_name)
                {
                    int min1, max1;
                    ScriptLibMemberMethod.GetMethodArgumentMinMax(arrayMethod[i].arrayParams, out min1, out max1);
                    
                    int min2, max2;
                    ScriptLibMemberMethod.GetMethodArgumentMinMax(args, out min2, out max2);

                    if (max2 < min1 || min2 > max1) break;   // 인자의 갯수가 다르면 다시 등록할 수 있다.
                                                             // 인자의 갯수가 같으면 Type이 다르면 찾는 계산이 너무 복잡해서
                                                             // 다음 기회로 미루었다.

                    main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, args_block.col_pos, args_block.row_pos, EnumScriptErrorType.Else, "같은 이름 '{0}' 의 메소드는 인자의 갯수가 달라야 합니다.", method_name);

                    return false;
                }
            }

            

            EditScriptLibMethod method = new EditScriptLibMethod(this);
            method.sNameMethod = method_name;
            method.arrayParams = args;
            method.eAccessLevel = eAccess;
            method.bStatic = bStatic;
            method.sReturnDataType = retn_datatype;

            /*
            int start_pos = stop_pos + 1;
            int end_pos = body.Length - 2;
            string one_block_data = body.Substring(start_pos, end_pos - start_pos + 1);*/

            if (!method.Split(main, file, body_block.block, body_block.col_pos, body_block.row_pos)) return false;

            arrayMethod.Add(method);

            return true;
        }

        bool AddProperty(EditScriptLibMain main, EditScriptLibFile file, EnumAccessLevel eAccess, bool bStatic, string var_type, string var_name, string body, SplitedString ss_body)
        {
            for (int i = 0; i < arrayMethod.Count; i++)
            {
                if (arrayMethod[i].sNameMethod == var_name)
                {
                    // 메소드는 같은 이름을 등록할 수 없다.
                    main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, ss_body.col_pos, ss_body.row_pos, EnumScriptErrorType.Else, "이미 같은 이름({0})의 메소드나 Property가 선언되었습니다.", var_name);
                    return false;
                }
            }

            if (IsExistVariableOnSplit(var_name))
            {
                main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, ss_body.col_pos, ss_body.row_pos, EnumScriptErrorType.Else, "이미 같은 이름이 ({0})의 변수로 선언 되었습니다.", var_name);
                return false;
            }

            string body_block = ss_body.block.Substring(1, ss_body.block.Length-2);

            ss_body.col_pos++;

            int start_col_pos = 0;
            int start_row_pos = 0;

            int block_start_pos;
            int block_end_pos;

            int start_pos = 0;
            bool bPropertyGet = false;
            bool bPropertySet = false;
            EditScriptLibMethod eslm = new EditScriptLibMethod(this);
            EditCommandBlock command1 = null;
            EditCommandBlock command2 = null;

            eslm.sSourceFilename = file.sSourceFilename;  // 소스파일을 지정해야 오류 발생 시 정확한 파일이름을 알 수 있다.
            
            while (true)
            {
                if (!main.GetOneBlock(file, body_block, ref start_pos, body_block.Length - 1, out start_col_pos, out start_row_pos, ref ss_body.col_pos, ref ss_body.row_pos, out block_start_pos, out block_end_pos))
                {
                    return false;
                }

                if (block_start_pos == block_end_pos)   // 더 이상 해석할 문장이 없다.
                {
                    break;
                }

                string one_block_data = body_block.Substring(block_start_pos, block_end_pos - block_start_pos + 1);

                if (String.Compare(one_block_data, 0, "get", 0, 3) == 0)
                {
                    if (bPropertyGet)
                    {
                        main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, ss_body.col_pos, ss_body.row_pos, EnumScriptErrorType.Else, "get property가 두개 이상 지정되었습니다.");
                        return false;
                    }

                    command1 = new EditCommandBlock(eslm, null);

                    string word;
                    int cp = start_col_pos;
                    int rp = start_row_pos;
                    int i = 0;
                    List<ScriptBaseClass> inheritances;

                    if (!main.GetWordToLeftCurlyBracket(file, one_block_data, out word, ref i, ref cp, ref rp, out inheritances)) return false;

                    string getset_block = one_block_data.Substring(i + 1, one_block_data.Length - i - 2);

                    if (!command1.Split(main, file, getset_block, cp, rp, false)) return false;

                    bPropertyGet = true;
                }
                else if (String.Compare(one_block_data, 0, "set", 0, 3) == 0)
                {
                    if (bPropertySet)
                    {
                        main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, ss_body.col_pos, ss_body.row_pos, EnumScriptErrorType.Else, "set property가 두개 이상 지정되었습니다.");
                        return false;
                    }

                    command2 = new EditCommandBlock(eslm, null);

                    string word;
                    int cp = start_col_pos;
                    int rp = start_row_pos;
                    int i = 0;
                    List<ScriptBaseClass> inheritances;

                    if (!main.GetWordToLeftCurlyBracket(file, one_block_data, out word, ref i, ref cp, ref rp, out inheritances)) return false;

                    string getset_block = one_block_data.Substring(i + 1, one_block_data.Length - i - 2);

                    if (!command2.Split(main, file, getset_block, cp, rp, false)) return false;

                    bPropertySet = true;
                }
                else
                {
                    main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, ss_body.col_pos, ss_body.row_pos, EnumScriptErrorType.Else, "get이나 set을 사용해야 합니다.");
                }
            }

            if (bPropertyGet == false && bPropertySet == false)
            {
                main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, ss_body.col_pos, ss_body.row_pos, EnumScriptErrorType.Else, "Property에서는 get이나 set 중 하나를 반드시 사용해야 합니다.");
                return false;
            }

            eslm.sNameMethod = var_name;
            eslm.bProperty = true;
            eslm.bPropertyGet = bPropertyGet;
            eslm.bPropertySet = bPropertySet;
            eslm.eAccessLevel = eAccess;
            eslm.bStatic = bStatic;
            eslm.sReturnDataType = var_type;
            eslm.blockCommand = command1;
            eslm.blockCommand2 = command2;
            
            arrayMethod.Add(eslm);
            
            return true;
        }

        bool AddVariable(EditScriptLibMain main, EditScriptLibFile file, EnumAccessLevel eAccess, bool bStatic, SplitedString var_type, SplitedString var_name, SplitedString name_value, List<SplitedString> array_blocks, bool bConst, bool bReadonly)
        {
            /*
            if (var_name.block == "test_var1")
            {
                int kkk = 10;
            }*/

            if (bConst && bStatic)
            {
                main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, name_value.col_pos, name_value.row_pos, EnumScriptErrorType.Else, "const 는 static과 함께 사용할 수 없습니다.", var_name.block);
                return false;
            }

            if (bConst && bReadonly)
            {
                main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, name_value.col_pos, name_value.row_pos, EnumScriptErrorType.Else, "const 는 readonly와 함께 사용할 수 없습니다.", var_name.block);
                return false;
            }

            if (IsExistVariableOnSplit(var_name.block))
            {
                main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, name_value.col_pos, name_value.row_pos, EnumScriptErrorType.Else, "이미 같은 이름({0})의 변수가 선언 되었습니다.", var_name.block);
                return false;
            }

            SplitedString array_word = null;

            int index = var_type.block.IndexOf('[');

            if (index != -1)
            {
                array_word = new SplitedString();
                array_word.block = var_type.block.Substring(index);
                array_word.col_pos = var_type.col_pos+index;
                array_word.row_pos = var_type.row_pos;
                var_type.block = var_type.block.Substring(0, index);
            }

            EditScriptLibClassVariable var = new EditScriptLibClassVariable(this);

            var.eAccessLevel = eAccess;
            var.bStatic = bStatic;

            if (array_word != null)
            {
                if (!((EditVariable)var.pVar).SplitArray(main, file, array_word.block, array_word.col_pos, array_word.row_pos)) 
                    return false;
            }
            var.pVar.sVarType = var_type.block;
            var.pVar.sVarName = var_name.block;
            var.pVar.bConst = bConst;
            var.pVar.bReadonly = bReadonly;
            var.nColumn = var_name.col_pos;
            var.nRow = var_name.row_pos;

            if (name_value == null)   // 대입 없이 끝난 선언문
            {
                if (bConst)
                {
                    main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, name_value.col_pos, name_value.row_pos, EnumScriptErrorType.Else, "const 변수는 초기화 값을 가지고 있어야 합니다.", var_name.block);
                    return false;
                }
            }
            else
            {
                // ; 을 떼어준다.
                if (name_value.block[name_value.block.Length - 1] == ';')
                {
                    name_value.block = name_value.block.Substring(0, name_value.block.Length - 1);
                }

                EditScriptLibMethod eslm = new EditScriptLibMethod(this);       // Split/Compile시 parentMethod가 null이면 오류가 발생하므로 정확한 메시지를 보여주기 위해 비어있는 method를 준다.

                // 이부분은 EditScriptLibMethod.Split에서 해야 하지만 여의치 않아서 여기서 한다. 2016-5-19
                eslm.sSourceFilename = file.sSourceFilename;
                eslm.nColumn = name_value.col_pos;
                eslm.nRow = name_value.row_pos;
                eslm.libFile = file;

                EditCommandBlock ecb = new EditCommandBlock(eslm, null);
                EditRecursiveValue value = new EditRecursiveValue(eslm, ecb);  // 원래 블럭은 여기서 시작이지만 비어있는 CommabdBlock을 parent_block으로 주는것이 더 좋을듯 2016-5-19

                if (!value.Split(main, file, name_value.block, name_value.col_pos, name_value.row_pos)) return false;

                var.pValue = value;
            }

            arrayVariable.Add(var);

            // , 다음에 있는 선언을 추가한다.
            for(int i = 1; i < array_blocks.Count; i++) {
                List<SplitedString> array_names = new List<SplitedString>();
                if (!SplitBySpaceOrEqual(main, file, array_blocks[i].block, array_blocks[i].col_pos, array_blocks[i].row_pos, ref array_names)) return false;

                var = new EditScriptLibClassVariable(this);

                var.eAccessLevel = eAccess;
                var.bStatic = bStatic;
                var.pVar.sVarType = var_type.block;
                var.pVar.sVarName = array_names[0].block;
                var.pVar.bConst = bConst;
                var.pVar.bReadonly = bReadonly;
                var.nColumn = array_names[0].col_pos;
                var.nRow = array_names[0].row_pos;

                if (array_names.Count == 1) // 초기화 없이 변수만 있는 경우  int a=1,b;  에서 b의 경우
                {

                }
                else if (array_names.Count == 3)
                {
                    if (array_names[1].block != "=")
                    {
                        main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, array_blocks[i].col_pos, array_blocks[i].row_pos, EnumScriptErrorType.Else, "추가 변수 선언 방법이 틀렸습니다. {0}", array_blocks[i].block);
                        return false;
                    }

                    EditScriptLibMethod eslm = new EditScriptLibMethod(this);   // Split/Compile시 parentMethod가 null이면 오류가 발생하므로 정확한 메시지를 보여주기 위해 비어있는 method를 준다.

                    // 이부분은 EditScriptLibMethod.Split에서 해야 하지만 여의치 않아서 여기서 한다. 2016-5-19
                    eslm.sSourceFilename = file.sSourceFilename;
                    eslm.nColumn = name_value.col_pos;
                    eslm.nRow = name_value.row_pos;
                    eslm.libFile = file;
                    
                    EditCommandBlock ecb = new EditCommandBlock(eslm, null);
                    EditRecursiveValue value = new EditRecursiveValue(eslm, ecb);  // 원래 블럭은 여기서 시작이지만 비어있는 CommabdBlock을 parent_block으로 주는것이 더 좋을듯 2016-5-19

                    if (!value.Split(main, file, array_names[2].block, array_names[2].col_pos, array_names[2].row_pos)) return false;

                    var.pValue = value;
                }
                else
                {
                    main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, array_blocks[i].col_pos, array_blocks[i].row_pos, EnumScriptErrorType.Else, "추가 변수 선언 방법이 틀렸습니다. {0}", array_blocks[i].block);
                    return false;
                }

                arrayVariable.Add(var);
            }

            return true;
        }

        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos)
        {
            int start_col_pos = 0;
            int start_row_pos = 0;

            int block_start_pos;
            int block_end_pos;

            int start_pos = 0;

            while (true)
            {
                if (!main.GetOneBlock(file, sBody, ref start_pos, sBody.Length - 1, out start_col_pos, out start_row_pos, ref col_pos, ref row_pos, out block_start_pos, out block_end_pos))
                {
                    return false;
                }

                if (block_start_pos == block_end_pos)   // 더 이상 해석할 문장이 없다.
                {
                    break;
                }

                string one_block_data = sBody.Substring(block_start_pos, block_end_pos - block_start_pos + 1);

                if (!SplitOneBlock(main, file, one_block_data, start_col_pos, start_row_pos)) return false;
            }

            return true;
        }

        // Compile전에 준비해야 하는 것들
        public void PrepareBeforeCompile()
        {
            // 생성자가 없는 경우 기본 생성자를 만들어 준다.
            EditScriptLibMethod eslm;

            for (int i = 0; i < arrayMethod.Count; i++)
            {
                eslm = (EditScriptLibMethod)arrayMethod[i];

                if (eslm.sNameMethod == sNameClass) return; // 생성자가 있다.
            }

            // 클래스에 생성자가 없는 경우 생성자를 기본적으로 만들어 준다.
            eslm = new EditScriptLibMethod(this);
            eslm.sNameMethod = sNameClass;
            arrayMethod.Add(eslm);
        }

        public void Compile(ScriptLibMain main)
        {
            if (arrayParentClass != null)
            {
                for (int i = 0; i < arrayParentClass.Count; i++)
                {
                    ScriptLibTools.SplitNamespaceClass(arrayParentClass[i].name, out arrayParentClass[i].sCompiledNamespace, out arrayParentClass[i].sCompiledClass);

                    ScriptLibClass slc;

                    EditScriptLibNamespace esln = (EditScriptLibNamespace)parentNamespace;

                    if (!((EditScriptLibMain)main).SeekClassWithError(esln, this.sourceFile, ref arrayParentClass[i].sCompiledNamespace, arrayParentClass[i].sCompiledClass, out slc, arrayParentClass[i].nColumn, arrayParentClass[i].nRow))
                    {
                        return;
                    }

                    arrayParentClass[i].pSlc = slc;
                }
            }

            EditScriptLibMethod eslm;

            for (int i = 0; i < arrayMethod.Count; i++)
            {
                eslm = (EditScriptLibMethod)arrayMethod[i];

                if (eslm.sNameMethod == this.sNameClass)
                {
                    if (eslm.bStatic && eslm.arrayParams.Count > 0)
                    {
                        main.SetError(parentNamespace.parentLibrary.sNameLibrary, sourceFile.sSourceFilename, eslm.nColumn, eslm.nRow, EnumScriptErrorType.Else, "'{0}' a static constructor Method must be parameterless", eslm.MakeStringMethodUsing());
                    }
                }

                eslm.Compile(main, this, eslm.nColumn, eslm.nRow);
            }

            for (int i = 0; i < arrayVariable.Count; i++)
            {
                ((EditScriptLibClassVariable)arrayVariable[i]).Compile(main);
            }
        }

        void ErrorMessageDoubleDeclareAccessLevel(EditScriptLibMain main, int col_pos, int row_pos)
        {
            main.SetError(parentNamespace.parentLibrary.sNameLibrary, sourceFile.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "More than one protection modifier");
        }

        void ErrorMessageDoubleDeclareStatic(EditScriptLibMain main, int col_pos, int row_pos)
        {
            main.SetError(parentNamespace.parentLibrary.sNameLibrary, sourceFile.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Duplicate 'static' modifier");
        }

        void ErrorMessageDoubleDeclareOverride(EditScriptLibMain main, int col_pos, int row_pos)
        {
            main.SetError(parentNamespace.parentLibrary.sNameLibrary, sourceFile.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Duplicate 'override' modifier");
        }

        void ErrorMessageDoubleDeclareReadOnly(EditScriptLibMain main, int col_pos, int row_pos)
        {
            main.SetError(parentNamespace.parentLibrary.sNameLibrary, sourceFile.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Duplicate 'readonly' modifier");
        }

        void ErrorMessageDoubleDeclareConst(EditScriptLibMain main, int col_pos, int row_pos)
        {
            main.SetError(parentNamespace.parentLibrary.sNameLibrary, sourceFile.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Duplicate 'const' modifier");
        }

        /*
        /// <summary>
        /// int a=0,b;
        /// 처럼 ,로 구분되어 있는 블럭을 분해한다. 최소한 1개가 있으며 { 나 ; 를 찾을 때 까지 계속한다.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="file"></param>
        /// <param name="sBody"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        bool SplitBlockByComma(EditScriptLibMain main, EditScriptLibFile file, string sBody, ref int col_pos, ref int row_pos, List<SplitedString> array_names, out int stop_pos)
        {
            int word_start_pos = 0;
            bool word_start = false;
            int start_col_pos = 0;
            int start_row_pos = 0;
            char ch;

            int count_parenthesis = 0;      // ()
            stop_pos = 0;

            SplitedString cs;

            for (int i = 0; i < sBody.Length; i++)
            {
                ch = sBody[i];

                if (ch == '{')
                {
                    if (word_start)
                    {
                        cs = new SplitedString();
                        cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                        cs.col_pos = start_col_pos;
                        cs.row_pos = start_row_pos;
                        
                        array_names.Add(cs);
                    }

                    stop_pos = i;
                    return true;
                }
                else if (ch == ';')
                {
                    if (word_start)
                    {
                        cs = new SplitedString();
                        cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                        cs.col_pos = start_col_pos;
                        cs.row_pos = start_row_pos;

                        array_names.Add(cs);
                    }
                    stop_pos = i;
                    return true;
                }
                else if (ch == '(')
                {
                    count_parenthesis++;
                }
                else if (ch == ')')
                {
                    count_parenthesis--;
                }
                else if (ch == ',')
                {
                    if (count_parenthesis == 0) // () 안에 있는 , 는 인자의 구분이다. 
                    {
                        if (word_start)
                        {
                            cs = new SplitedString();
                            cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                            cs.col_pos = start_col_pos;
                            cs.row_pos = start_row_pos;

                            array_names.Add(cs);

                            word_start = false;
                        }
                    }
                }
                else if(!word_start)
                {
                    if (ch == ' ')
                    {

                    }
                    else if (ch == '\t')
                    {
                    }
                    else if (ch == '\n')
                    {

                    }
                    
                    else
                    {
                        word_start = true;
                        word_start_pos = i;
                        start_col_pos = col_pos;
                        start_row_pos = row_pos;
                    }
                }

                main.CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, "문장은 ; 나 } 로 끝나야 합니다.");

            return false;
        }*/

        /// <summary>
        /// a=0*10   ->  a / = / 0*10 으로 분리
        /// public int a  -> public / int / a 로 분리
        /// a = Method(b) -> a / = / Method / (b)로 분리
        /// public int a() { ... }  ->  public / int / a / () / { ... }
        /// 
        /// 처럼되어 있는 문장을 ' ' '=' ':' 등의 구분에서 분리해 준다.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="file"></param>
        /// <param name="sBody"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        bool SplitBySpaceOrEqual(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos, ref List<SplitedString> array_names)
        {
            int word_start_pos = 0;
            bool word_start = false;
            int word_start_col_pos = 0;
            int word_start_row_pos = 0;
            char ch;
            int count_parenthesis = 0;      // ()

            SplitedString cs;

            int i = 0;

            for (; i < sBody.Length; i++)
            {
                ch = sBody[i];

                if (ch == '(')
                {
                    count_parenthesis++;

                    if (count_parenthesis == 1)
                    {
                        if (word_start)
                        {
                            cs = new SplitedString();
                            cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                            cs.col_pos = word_start_col_pos;
                            cs.row_pos = word_start_row_pos;

                            array_names.Add(cs);

                            word_start = false;
                        }

                        word_start = true;
                        word_start_pos = i;
                        word_start_col_pos = col_pos;
                        word_start_row_pos = row_pos;
                    }
                }
                else if (ch == ')')
                {
                    count_parenthesis--;

                    if (count_parenthesis == 0)
                    {
                        if (word_start)
                        {
                            cs = new SplitedString();
                            cs.block = sBody.Substring(word_start_pos, i - word_start_pos+1);
                            cs.col_pos = word_start_col_pos;
                            cs.row_pos = word_start_row_pos;

                            array_names.Add(cs);

                            /*
                            if (String.Compare(cs.block, 0, "(byte[]", 0, 7) == 0)
                            {
                                int kkkk = 10;
                            }*/

                            word_start = false;
                        }
                    }
                }
                else if (ch == '{')
                {
                    // { 가 있으면 프로퍼티이거나 함수의 형태이다.
                    if (word_start)
                    {
                        cs = new SplitedString();
                        cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                        cs.col_pos = word_start_col_pos;
                        cs.row_pos = word_start_row_pos;

                        array_names.Add(cs);
                    }

                    cs = new SplitedString();
                    cs.block = sBody.Substring(i);
                    cs.col_pos = col_pos;
                    cs.row_pos = row_pos;
                    array_names.Add(cs);

                    return true;
                }
                else if (ch == ';')
                {
                    // 앞의 분해에서 ; 가 포함되어 있다.
                    if (word_start)
                    {
                        cs = new SplitedString();
                        cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                        cs.col_pos = word_start_col_pos;
                        cs.row_pos = word_start_row_pos;

                        array_names.Add(cs);
                    }

                    return true;
                }
                else if (count_parenthesis == 0)
                {
                    if (ch == '=')
                    {
                        if (word_start)
                        {
                            cs = new SplitedString();
                            cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                            cs.col_pos = word_start_col_pos;
                            cs.row_pos = word_start_row_pos;

                            array_names.Add(cs);
                        }

                        cs = new SplitedString();
                        cs.block = "=";
                        cs.col_pos = col_pos;
                        cs.row_pos = row_pos;

                        array_names.Add(cs);

                        cs = new SplitedString();
                        cs.block = sBody.Substring(i + 1);
                        cs.col_pos = col_pos + 1;
                        cs.row_pos = row_pos;

                        array_names.Add(cs);

                        return true;
                    }
                    else if (ch == ':') // 생성자() : base () 형식
                    {
                        if (word_start)
                        {
                            cs = new SplitedString();
                            cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                            cs.col_pos = word_start_col_pos;
                            cs.row_pos = word_start_row_pos;

                            array_names.Add(cs);

                            cs = new SplitedString();
                            cs.block = ":";
                            cs.col_pos = col_pos;
                            cs.row_pos = row_pos;

                            array_names.Add(cs);
                        }
                    }
                    else if (ch == ' ' || ch == '\t' || ch == '\n')
                    {
                        if (count_parenthesis == 0)
                        {
                            if (word_start)
                            {
                                cs = new SplitedString();
                                cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                                cs.col_pos = word_start_col_pos;
                                cs.row_pos = word_start_row_pos;

                                array_names.Add(cs);

                                word_start = false;
                            }
                        }
                    }
                    else
                    {
                        if (count_parenthesis == 0)
                        {

                            if (!word_start)
                            {
                                word_start = true;
                                word_start_pos = i;
                                word_start_col_pos = col_pos;
                                word_start_row_pos = row_pos;
                            }
                        }
                    }
                }

                main.CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            if (word_start)
            {
                cs = new SplitedString();
                cs.block = sBody.Substring(word_start_pos, i - word_start_pos);
                cs.col_pos = word_start_col_pos;
                cs.row_pos = word_start_row_pos;

                array_names.Add(cs);
            }

            return true;
        }

        /// <summary>
        /// public static int Method() {}
        /// public static int Var;
        /// public static int i = 10, i = 20;
        /// public static int i, j;
        /// public static int i=Method(), j;
        /// public static int Property {}
        /// public static Contructor() {}
        /// public 
        /// int[] a = new int[3] { 10, 20, 30 };
        /// double s = Math.Abs(10.3);
        /// double s = Math.Abs(10.3), k = Math.Acos(2.3);
        /// static int kkk = 10;
        /// static int kkkk = kkk * 10;
        /// static int kkkkk = kkkk = kkk;
        /// public EditScriptLibClass(EditScriptLibNamespace parent, EditScriptLibFile file) : base()
        /// 
        /// 구조는 access static datatype name1,name2 식으로 온다.
        /// 유일하게 생성자는 access static name 식으로 온다. 이거만 주의 한다.
        /// { 이거나 ; 가 될 때까지 계속 해석 해야 한다.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="file"></param>
        /// <param name="sBody"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        bool SplitOneBlock(EditScriptLibMain main, EditScriptLibFile file, string source, int col_pos, int row_pos)
        {
            bool access_start = false;
            EnumAccessLevel eAccess = EnumAccessLevel.access_private;
            bool bStatic = false;
            bool bOverride = false;
            SplitedString datatype = null;
            SplitedString name = null;
            bool bConst = false;
            bool bReadonly = false;

            List<SplitedString> array_blocks = new List<SplitedString>();

            //int stop_pos = 0;
            //if (!SplitBlockByComma(main, file, source, ref col_pos, ref row_pos, array_blocks, out stop_pos)) return false;
            if (!EditCommandBlock.SplitBlockByCommaAndColon(main, file, source, ref col_pos, ref row_pos, array_blocks)) return false;
            //int stop_col = col_pos;
            //int stop_row = row_pos;

            string sBody = array_blocks[0].block;
            col_pos = array_blocks[0].col_pos;
            row_pos = array_blocks[0].row_pos;

            List<SplitedString> array_names = new List<SplitedString>();
            if (!SplitBySpaceOrEqual(main, file, sBody, col_pos, row_pos, ref array_names)) return false;

            for (int i = 0; i < array_names.Count; i++)
            {
                if (array_names[i].block == "public")
                {
                    if (access_start)
                    {
                        ErrorMessageDoubleDeclareAccessLevel(main, array_names[i].col_pos, array_names[i].row_pos);
                        return false;
                    }
                    access_start = true;
                    eAccess = EnumAccessLevel.access_public;
                }
                else if (array_names[i].block == "private")
                {
                    if (access_start)
                    {
                        ErrorMessageDoubleDeclareAccessLevel(main, array_names[i].col_pos, array_names[i].row_pos);
                        return false;
                    }
                    access_start = true;
                    eAccess = EnumAccessLevel.access_private;
                }
                else if (array_names[i].block == "protected")
                {
                    if (access_start)
                    {
                        ErrorMessageDoubleDeclareAccessLevel(main, array_names[i].col_pos, array_names[i].row_pos);
                        return false;
                    }
                    access_start = true;
                    eAccess = EnumAccessLevel.access_protected;
                }
                else if (array_names[i].block == "static")   // static 문
                {
                    if (bStatic)
                    {
                        ErrorMessageDoubleDeclareStatic(main, array_names[i].col_pos, array_names[i].row_pos);
                        return false;
                    }
                    bStatic = true;
                }
                else if (array_names[i].block == "override")   // 
                {
                    if (bOverride)
                    {
                        ErrorMessageDoubleDeclareOverride(main, array_names[i].col_pos, array_names[i].row_pos);
                        return false;
                    }
                    bOverride = true;
                }
                else if (array_names[i].block == "readonly")   // 
                {
                    if (bReadonly)
                    {
                        ErrorMessageDoubleDeclareReadOnly(main, array_names[i].col_pos, array_names[i].row_pos);
                        return false;
                    }
                    bReadonly = true;
                }
                else if (array_names[i].block == "const")   // 
                {
                    if (bConst)
                    {
                        ErrorMessageDoubleDeclareConst(main, array_names[i].col_pos, array_names[i].row_pos);
                        return false;
                    }
                    bConst = true;
                }
                else if (array_names[i].block == "=")   // 대입문
                {
                    if (datatype == null)
                    {
                        main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, array_names[i].col_pos, array_names[i].row_pos, EnumScriptErrorType.Else, "= 앞에 선언된 변수의 데이터형이 없습니다.");
                    }
                    if (name == null)
                    {
                        main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, array_names[i].col_pos, array_names[i].row_pos, EnumScriptErrorType.Else, "= 앞에 선언된 변수의 이름이 없습니다.");
                    }

                    if (!AddVariable(main, file, eAccess, bStatic, datatype, name, array_names[i + 1], array_blocks, bConst, bReadonly)) return false;

                    return true;
                }
                else if (array_names[i].block[0] == '(')   // 함수
                {
                    if (name == null)
                    {
                        // Data Type 없이 ( 가 시작되는 것은 클래스와 같은 이름을 가진 생성자만 가능하다.

                        if (datatype.block != sNameClass)
                        {
                            main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "( 앞에 Method명이 없습니다.");
                            return false;
                        }

                        // name = datatype; 이렇게 하면 포인터가 복사된다.
                        name = new SplitedString();
                        name.block = datatype.block;
                        name.col_pos = datatype.col_pos;
                        name.row_pos = datatype.row_pos;
                                                
                        datatype.block = "void";
                    }

                    if (datatype == null)
                    {
                        main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Method의 return 값 형태가 지정되지 않았습니다.");
                        return false;
                    }

                    if (!AddMethod(main, file, eAccess, bStatic, datatype.block, name.block, array_names[i], source, array_names[i + 1])) return false;

                    return true;
                }
                else if (array_names[i].block[0] == '{')   // property
                {
                    if (name == null)
                    {
                        main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "{ 앞에 Property 명이 없습니다.");
                        return false;
                    }

                    if (!AddProperty(main, file, eAccess, bStatic, datatype.block, name.block, source, array_names[i])) return false;

                    return true;
                }
                else
                {
                    if (datatype == null)
                    {
                        datatype = array_names[i];
                    }
                    else if (name == null)
                    {
                        name = array_names[i];
                    }
                    else
                    {
                        main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, array_names[i].col_pos, array_names[i].row_pos, EnumScriptErrorType.Else, "변수명이 두 개 이상이 지정되었습니다.");
                    }
                }
            }

            if (name == null)
            {
                if (Tools.IsLangKorean())
                    main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "; 앞에 선언된 변수의 이름이 없습니다.");
                else
                    main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Invalid token ';' in class, struct, or interface member declaration");

                return false;
            }

            if (!AddVariable(main, file, eAccess, bStatic, datatype, name, null, array_blocks, bConst, bReadonly)) return false;

            return true;

        }

        bool CheckMethodArguments(EditScriptLibMain main, EditScriptLibFile file, string body, int i, int end_pos, int col_pos, int row_pos, List<MethodArgument> args)
        {
            if (i == end_pos) return true;  // ()로 쌓여 있어서 argument가 없는 경우

            char ch;
            bool word_start = false;
            int word_start_pos = 0;
            EnumInOut inout = EnumInOut.In;
            string datatype = null;
            string name = null;
            int start_col_pos=col_pos, start_row_pos=row_pos;

            for (; i <= end_pos; i++)
            {
                ch = body[i];

                if (ch == ',' || ch == ')')
                {
                    if (name == null)
                    {
                        if (word_start)
                        {
                            name = body.Substring(word_start_pos, i - word_start_pos);
                        }
                        else
                        {
                            main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "{0} 앞에 변수명이 없습니다.", ch);
                            return false;
                        }
                    }
                    else
                    {
                        if (word_start)
                        {
                            main.SetError(parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "{0} 앞에 변수명이 두개가 지정되었습니다.", ch);
                            return false;
                        }
                    }

                    // void method(byte[] a) 와 같이 함수 선언에서 [] 부분인데 []의 갯수 같은 문법은 검사하지 않았다. 나중에 검사가 필요하다.
                    SplitedString ss_array = null;

                    int index = datatype.IndexOf('[');

                    if (index != -1)
                    {
                        ss_array = new SplitedString();
                        ss_array.block = datatype.Substring(index);
                        ss_array.col_pos = col_pos;
                        ss_array.row_pos = row_pos;

                        datatype = datatype.Substring(0, index);
                    }

                    EditMethodArgument arg = new EditMethodArgument();

                    if (ss_array != null)
                    {
                        if (!((EditVariable)arg.pVar).SplitArray(main, file, ss_array.block, ss_array.col_pos, ss_array.row_pos)) return false;
                    }

                    arg.pVar.sVarType = datatype;
                    arg.eInOut = inout;
                    arg.pVar.sVarName = name;
                    arg.nColumn = start_col_pos;
                    arg.nRow = start_row_pos;

                    args.Add(arg);

                    word_start = false;
                    name = null;
                    datatype = null;
                    inout = 0;
                }
                else if (word_start) // 단어가 시작되었다.
                {
                    if (ch == ' ' || ch == '\t' || ch == '\n')
                    {
                        if (String.Compare(body, word_start_pos, "ref", 0, 3) == 0)
                        {
                            inout = EnumInOut.Ref;
                        }
                        else if (String.Compare(body, word_start_pos, "out", 0, 3) == 0)   // 
                        {
                            inout = EnumInOut.Out;
                        }
                        else if (String.Compare(body, word_start_pos, "params", 0, 6) == 0)   // 
                        {
                            inout = EnumInOut.Params;
                        }
                        else
                        {
                            if (datatype == null)
                            {
                                datatype = body.Substring(word_start_pos, i - word_start_pos);
                            }
                            else
                            {
                                name = body.Substring(word_start_pos, i - word_start_pos);
                            }
                        }

                        word_start = false;
                    }
                }
                else
                {
                    if (ch == ' ')
                    {
                    }
                    else if (ch == '\t')
                    {
                    }
                    else if (ch == '\n')
                    {
                    }
                    else if (ch == '/')
                    {
                        //if (!main.SkipDescription(body, ref i, ref col_pos, ref row_pos)) return false;
                        bool? retn_description = main.CheckDescription(file, body, ref i, ref col_pos, ref row_pos);
                        if (retn_description == null) return false;  // 오류가 발생했다.
                        if (retn_description == true) continue;
                    }
                    else
                    {
                        word_start = true;
                        word_start_pos = i;
                        start_col_pos = col_pos;
                        start_row_pos = row_pos;
                    }
                }

                main.CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            return true;
        }

        // Method의 형 선언에서 ) 까지의 위치를 찾는다.
        bool SeekPosToRightParenthesis(EditScriptLibMain main, string body, ref int i, int end_pos, ref int col_pos, ref int row_pos)
        {
            i++;
            col_pos++;

            char ch;

            for (; i <= end_pos; i++)
            {
                ch = body[i];

                if (ch == ')')
                {
                    return true;
                }

                main.CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            main.SetError(parentNamespace.parentLibrary.sNameLibrary, sourceFile.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "메소드의 선언에서 ) 문자를 찾을 수 없습니다.");
            return false;
        }

        // Method의 형 선언에서 { 까지의 위치를 찾는다.
        bool SeekPosToLeftBracket(EditScriptLibMain main, string body, ref int i, int end_pos, ref int col_pos, ref int row_pos)
        {
            i++;
            col_pos++;

            char ch;

            for (; i <= end_pos; i++)
            {
                ch = body[i];

                if (ch == '{')
                {
                    return true;
                }
                else if (ch == ' ' || ch == '\t' || ch == '\n')
                {

                }
                else
                {
                    main.SetError(parentNamespace.parentLibrary.sNameLibrary, sourceFile.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "메소드의 시작은 { 문자로 시작해야 합니다.");
                }

                main.CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            main.SetError(parentNamespace.parentLibrary.sNameLibrary, sourceFile.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "메소드의 시작인 { 문자를 찾을 수 없습니다.");
            return false;
        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth, bool write_version)
        {
            ScriptWriter writer = new ScriptWriter();

            // 이전 스크립트에서는 클래스만 저장하므로 버전이 필요하다.
            if (write_version) 
                writer.WriteVersion();

            writer.WriteName(this.sNameClass);
            writer.WriteBool(EnumBlockType.bStruct, bStruct);
            
            for (int i = 0; i < arrayMethod.Count; i++)
            {
                ((EditScriptLibMethod)arrayMethod[i]).SaveToStream(writer, tab_depth + 1);
            }
            for (int i = 0; i < arrayVariable.Count; i++)
            {
                ((EditScriptLibClassVariable)arrayVariable[i]).SaveToStream(writer, tab_depth + 1);
            }

            if (arrayParentClass != null)
            {
                for (int i = 0; i < arrayParentClass.Count; i++)
                {
                    arrayParentClass[i].SaveToStream(writer, tab_depth + 1);
                }
            }

            parent_writer.WriteBlock(EnumBlockType.ClassBlock, writer);
        }

        public bool IsExistVariableOnSplit(string varname)
        {
            for (int i = 0; i < arrayVariable.Count; i++)
            {
                if (arrayVariable[i].pVar.sVarName == varname)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsExistVariableOnCompile(string varname, out Variable var, out object finded_pos)
        {
            for (int i = 0; i < arrayVariable.Count; i++)
            {
                if (arrayVariable[i].pVar.sVarName == varname)
                {
                    finded_pos = arrayVariable[i];
                    var = arrayVariable[i].pVar;
                    return true;
                }
            }

            if (arrayParentClass != null)
            {
                for (int i = 0; i < arrayParentClass.Count; i++)
                {
                    EditScriptLibClass eslc = (EditScriptLibClass)arrayParentClass[i].pSlc;

                    if (eslc.IsExistVariableOnCompile(varname, out var, out finded_pos))
                        return true;
                }
            }

            finded_pos = null;
            var = null;
            return false;
        }

        /*
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

        public void SetBaseClass(List<ScriptBaseClass> lists)
        {
            if (lists.Count == 0) return;

            arrayParentClass = lists;
            /*
            arrayParentClass = new List<ScriptBaseClass>();

            for (int i = 0; i < lists.Count; i++)
            {
                ScriptBaseClass sbc = new ScriptBaseClass();

                sbc.name = lists[i];

                arrayParentClass.Add(sbc);
            }*/
        }
    }
}
