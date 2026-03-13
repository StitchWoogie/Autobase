using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using System.IO;
using NetTools;
using System.Windows.Forms;

namespace ScriptLibEdit
{
    public class EditScriptLibMain : ScriptLibMain
    {
        // 1. 먼저 파일을 분해해서 적당한 형태로 만든다. 문법상의 오류만 해당되고 각 클래스의 존재 여부는 생각하지 않는다. 
        public void Split(string library_name, string default_namespace, string filename)
        {
            if (!File.Exists(filename))
            {
                SetError(library_name, filename, 0, 0, EnumScriptErrorType.Else, "{0} file not found.", filename);
                return;
            }

            TextReader reader = new StreamReader(filename);

            string sBody;

            sBody = reader.ReadToEnd();
            reader.Close();

            SplitByCode(library_name, default_namespace, filename, sBody);
        }

        /*
        // Compile전에 준비해야 하는 것들
        public void PrepareBeforeCompile()
        {
            for (int i = 0; i < arrayLibrary.Count; i++)
            {
                if (arrayLibrary[i].GetType() != typeof(EditScriptLibLibrary)) continue;

                ((EditScriptLibLibrary)arrayLibrary[i]).PrepareBeforeCompile();
            }
        }

        /// <summary>
        /// 2. Split가 끝난 후 각 변수 및 함수의 존재여부를 검사한다.
        /// 오류가 여러줄 발생할 수 있으므로 리턴을 반환하지 않고 error flag로 오류를 처리한다.
        /// </summary>
        public void Compile()
        {
            for (int i = 0; i < arrayLibrary.Count; i++)
            {
                arrayLibrary[i].Compile(this);
            }
        }*/

        // 보통 \r\n이 개행문자로 사용되는데 \r문자를 제거하여 \n문자 하나로 만들어서 소스가 복잡하지 않게 만든다.
        string ClearCarriageReturn(string source)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] != '\r')
                {
                    s.Append(source[i]);
                }
            }

            return s.ToString();
        }

        /// <summary>
        /// " 문자가 시작되면 " 이 끝날 때 까지 계속한다. 오류가 발생하면 false를 반환한다.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="i"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        public bool SkipQuotationMark(EditScriptLibFile file, string body, ref int i, int string_end_pos, ref int col_pos, ref int row_pos)
        {
            int start_col_pos = col_pos;
            int start_row_pos = row_pos;

            i++;
            col_pos++;

            bool reverse_slash = false;
            char ch;

            for (; i <= string_end_pos; i++)
            {
                ch = body[i];

                if (ch == '\\')
                {
                    reverse_slash = !reverse_slash;
                }
                else if (ch == '"')
                {
                    if (reverse_slash)
                    {
                        // " " 문장 안에 있는 \" 문장이다
                        reverse_slash = false;
                    }
                    else
                    {
                        // 이전 함수가 i++ 와 CalcCursorPos(ch, ref col_pos, ref row_pos); 를 행하므로 할 필요가 없다.
                        return true;
                    }
                }
                else
                {
                    // 그외 문자일 경우 \ 다음에 있는 문자가 나오면 reverse_slash 는 해제된다.
                    if (reverse_slash)
                    {
                        reverse_slash = false;
                    }
                }

                CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            SetError(file.sProjectName, file.sSourceFilename, start_col_pos, start_row_pos, EnumScriptErrorType.Else, "\"문자 끝에 닫는 \" 문자가 없습니다.");

            return false;
        }

        /// <summary>
        /// ' 문자가 시작되면 ' 이 끝날 때 까지 계속한다. 오류가 발생하면 false를 반환한다.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="i"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        public bool SkipApostrophe(EditScriptLibFile file, string body, ref int i, int string_end_pos, ref int col_pos, ref int row_pos)
        {
            int start_col_pos = col_pos;
            int start_row_pos = row_pos;

            i++;
            col_pos++;

            bool reverse_slash = false;
            char ch;

            for (; i <= string_end_pos; i++)
            {
                ch = body[i];

                if (ch == '\\')
                {
                    reverse_slash = !reverse_slash;
                }
                else if (ch == '\'')
                {
                    if (reverse_slash)
                    {
                        // " " 문장 안에 있는 \" 문장이다
                        reverse_slash = false;
                    }
                    else
                    {
                        // 이전 함수가 i++ 와 CalcCursorPos(ch, ref col_pos, ref row_pos); 를 행하므로 할 필요가 없다.
                        return true;
                    }
                }
                else if (ch == '\n')
                {
                    // 개행 문자가 나오도록 ' 가 없으면 안된다. 'a' 형식은 그줄에 끝나야 한다.
                    SetError(file.sProjectName, file.sSourceFilename, start_col_pos, start_row_pos, EnumScriptErrorType.Else, "Cannot find closing \' character.");
                    return false;
                }
                else
                {
                    // 그외 문자일 경우 \ 다음에 있는 문자가 나오면 reverse_slash 는 해제된다.
                    if (reverse_slash)
                    {
                        reverse_slash = false;
                    }
                }

                CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            SetError(file.sProjectName, file.sSourceFilename, start_col_pos, start_row_pos, EnumScriptErrorType.Else, "Cannot find closing \' character.");

            return false;
        }

        /// <summary>
        /// [ 문자가 시작되면 ] 이 끝날 때 까지 계속한다. 오류가 발생하면 false를 반환한다.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="i"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
                
        public bool SkipSqureBracket(EditScriptLibFile file, string body, ref int i, ref int col_pos, ref int row_pos, out string block)
        {
            int start_col_pos = col_pos;
            int start_row_pos = row_pos;
            int start_i = i;

            i++;
            col_pos++;

            char ch;

            for (; i < body.Length; i++)
            {
                ch = body[i];

                if (ch == ']')
                {
                    // 이전 함수가 i++ 와 CalcCursorPos(ch, ref col_pos, ref row_pos); 를 행하므로 할 필요가 없다.
                    block = body.Substring(start_i, i - start_i+1);
                    return true;
                }
                else
                {

                }

                CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            SetError(file.sProjectName, file.sSourceFilename, start_col_pos, start_row_pos, EnumScriptErrorType.Else, "'['문자 끝에 닫는 ']' 문자가 없습니다.");

            block = "";
            return false;
        }

        /// <summary>
        /// OneBlock이라 함은 ; 로 끝나거나 {}로 닫혀있는 구간하나를 가져온다. ,는 해당되지 않는다.
        /// 이것은 SubString을 하지말고 block_pos를 가지고 해야한다.
        /// 모든 소스는 이 두가지 중 하나의 블럭으로 구성되어 있으므로 블럭별로 해석하면 된다.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="body"></param>
        /// <param name="i"></param>
        /// <param name="string_end_pos"></param>
        /// <param name="start_col_pos"></param>
        /// <param name="start_row_pos"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <param name="block_start_pos"></param>
        /// <param name="block_end_pos"></param>
        /// <returns></returns>

        public bool GetOneBlock(EditScriptLibFile file, string body, ref int i, int string_end_pos, out int start_col_pos, out int start_row_pos, ref int col_pos, ref int row_pos, out int block_start_pos, out int block_end_pos)
        {
            block_start_pos = 0;
            block_end_pos = 0;
            start_col_pos = 0;
            start_row_pos = 0;

            char ch;
            int count_curlybracket = 0;     // {}
            int count_parenthesis = 0;      // ()
            //int count_squarebracket = 0;    // [] 
            bool block_start = false;

            // 먼저 공간을 제외한 첫번째로 시작하는 글자를 찾는다.
            for (; i <= string_end_pos; i++)
            {
                ch = body[i];
                if (ch == ' ')
                {

                }
                else if (ch == '\t')
                {

                }
                else if (ch == '\n')
                {

                }
                else if (ch == ';') // ; 으로 시작된다는 것은 실행할 문장이 하나도 없는것이므로 빈칸 취급한다.
                {

                }
                else if (ch == '/')
                {
                    bool? retn_description = CheckDescription(file, body, ref i, ref col_pos, ref row_pos);
                    if (retn_description == null) return false;  // 오류가 발생했다.
                    if (retn_description == true) continue;

                    // / 로 시작하는 문장이 설명문이 아니면 그 글자고 시작한다.
                    block_start = true;
                    block_start_pos = i;
                    start_col_pos = col_pos;
                    start_row_pos = row_pos;
                    break;
                }
                else
                {
                    block_start = true;
                    block_start_pos = i;
                    start_col_pos = col_pos;
                    start_row_pos = row_pos;

                    break;
                }

                CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            for (; i <= string_end_pos; i++)
            {
                ch = body[i];
                
                if (ch == '}')
                {
                    count_curlybracket--;
                    if (count_curlybracket == 0)
                    {
                        block_end_pos = i;
                        i++;
                        return true;
                    }
                }
                else if (ch == ';')
                {
                    // ;이 for loop의 (;;;) 속에 있을 수도 있다
                    if (count_curlybracket == 0 && count_parenthesis == 0)
                    {
                        block_end_pos = i;
                        i++;
                        return true;
                    }
                }

                else if (ch == '"')
                {
                    if (!SkipQuotationMark(file, body, ref i, string_end_pos, ref col_pos, ref row_pos)) return false;
                }
                else if (ch == '\'')
                {
                    if (!SkipApostrophe(file, body, ref i, string_end_pos, ref col_pos, ref row_pos)) return false;
                }
                else if (ch == '{')
                {
                    count_curlybracket++;
                }
                else if (ch == '(')
                {
                    count_parenthesis++;
                }
                else if (ch == ')')
                {
                    count_parenthesis--;
                }
                else if (ch == '/')
                {
                    bool? retn_description = CheckDescription(file, body, ref i, ref col_pos, ref row_pos);
                    if (retn_description == null) return false;  // 오류가 발생했다.
                    if (retn_description == true)   continue;
                }
                else
                {

                }

                CalcCursorPos(ch, ref col_pos, ref row_pos);    
            }

            if (block_start)
            {
                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "블럭이나 문장의 끝은 ; 나 } 로 끝나야 합니다.");
                return false;
            }

            block_start_pos = i;
            block_end_pos = i;
            return true;
        }

        public ScriptLibLibrary AddLibrary(string name)
        {
            for (int i = 0; i < arrayLibrary.Count; i++)
            {
                if (arrayLibrary[i].sNameLibrary == name) return arrayLibrary[i];
            }

            EditScriptLibLibrary esll = new EditScriptLibLibrary();
            esll.sNameLibrary = name;

            arrayLibrary.Add(esll);

            return esll;
        }

        //string sDefaultLibrary = "MyProject";    // library가 선언되지 않았을 때 지정할 기본이름

        /// <summary>
        /// 소스 코드를 가지고 분해한다.
        /// </summary>
        /// <param name="library_name"></param>
        /// <param name="default_namespace"></param>
        /// <param name="filename"></param>
        /// <param name="source"></param>
        public void SplitByCode(string library_name, string default_namespace, string filename, string source)
        {
            EditScriptLibFile file = new EditScriptLibFile();

            file.sSourceFilename = filename;
            file.sProjectName = library_name;
            file.sDefaultNamespace = default_namespace;

            string sBody;

            sBody = ClearCarriageReturn(source);

            // 모든 파일을 기본 namespace로 싸서 하면 namaspce에서 main과 namespace 두군데서 하는 해석을 하나로 줄일 수 있다.
            ScriptLibLibrary esln = AddLibrary(library_name);

            esln.Split(this, file, sBody);
        }

        // 커서의 위치를 계산한다. 따로 계산하는 것이 소스를 복잡하게 하지 않을 것 같다.
        public void CalcCursorPos(char ch, ref int col_pos, ref int row_pos)
        {
            if (ch == '\t')
            {
                int remain = col_pos % nTabSpace;

                col_pos += (nTabSpace - remain);
            }
            else if (ch == '\n')
            {
                col_pos = 0;
                row_pos++;
            }
            else
            {
                col_pos++;
            }
        }

        /// <summary>
        /// / 문자인 경우 설명문인가를 검사한다. 오류가 발생하면 null을 반납하고 설명문이면 true를 반납하고 설명문이 아니면 false를 반납한다.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="i"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        public bool? CheckDescription(EditScriptLibFile file, string body, ref int i, ref int col_pos, ref int row_pos)
        {
            if (i >= body.Length - 1)
            {
                return false;   // 맨마지막에 / 가 있으므로 설명문이 아니다.
            }

            bool line_description = false;
            bool block_description = false;

            if (body[i + 1] == '/')
            {
                line_description = true;
            }
            else if (body[i + 1] == '*')
            {
                block_description = true;
            }
            else
            {
                return false;   // 설명문이 아니다. / 만 있다.
            }

            i += 2;
            col_pos += 2;
            
            char ch;
            bool star_flag = false;

            for (; i < body.Length; i++)
            {
                ch = body[i];

                CalcCursorPos(ch, ref col_pos, ref row_pos);

                if (line_description)   // 한줄 설명일 때는 개행할 때 까지 계속한다.
                {
                    if (ch == '\n')
                    {
                        return true;
                    }
                }
                else if (block_description) // 블럭 설명일 때는 * / 이 나올 때 까지 계속한다.
                {
                    if (!star_flag)
                    {
                        if (ch == '*')
                        {
                            star_flag = true;
                        }
                    }
                    else
                    {
                        if (ch == '/')
                        {
                            return true;
                        }
                        else
                        {
                            star_flag = false;
                        }
                    }
                }
            }

            if (line_description) return true;  // 한줄 설명문일 때는 소스가 끝이면 정상적이다.

            if (block_description)
            {
                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "*/ 로 끝나는 블럭 종료 문자가 없습니다.");
                return null;
            }

            return false;
        }
       
        /// <summary>
        /// { 까지의 위치를 찾으면서 사이에 들어 있는 단어를 가져온다.
        /// class, struct, enum, get, set 체크할때 이 함수를 사용한다.
        /// class 같은 경우는 class a : b,c,d,e 처럼 여러 클래스에서 상속 받을 수 있다.
        /// enum의 경우는 enum a : byte 와 같이 데이터 형식을 받을 수 있다.
        /// struct는 struct a { 와 같이 고유한 이름만 있고 상속은 받을 수 없다.
        /// </summary>
        /// <param name="source_filename"></param>
        /// <param name="body"></param>
        /// <param name="word"></param>
        /// <param name="i"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        public bool GetWordToLeftCurlyBracket(EditScriptLibFile file, string body, out string name, ref int i, ref int col_pos, ref int row_pos, out List<ScriptBaseClass> base_class_names)
        {
            i++;
            col_pos++;

            name = "";
            char ch;
            bool word_start = false;
            int word_start_pos = 0;
            bool colon_start = false;
            base_class_names = new List<ScriptBaseClass>();
            string buf;
            ScriptBaseClass sbc;

            for (; i < body.Length; i++)
            {
                ch = body[i];

                if (ch == '{')
                {
                    if (colon_start)
                    {
                        if (word_start)
                        {
                            buf  = body.Substring(word_start_pos, i - word_start_pos);
                            sbc = new ScriptBaseClass();
                            sbc.name = buf;
                            sbc.nColumn = col_pos;
                            sbc.nRow = row_pos;
                            base_class_names.Add(sbc);
                        }
                    }
                    else
                    {
                        if (word_start)
                        {
                            buf = body.Substring(word_start_pos, i - word_start_pos);
                            name = buf;
                        }
                    }

                    if(name.Length == 0) {
                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Name does not exist before {");
                        return false;
                    }
                    if (colon_start && base_class_names.Count == 0)
                    {
                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "base type not exist after :");
                        return false;
                    }

                    return true;
                }
                else if (ch == ':')
                {
                    if (colon_start)
                    {
                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Colon is duplicated.");
                        return false;
                    }

                    if (word_start)
                    {
                        name = body.Substring(word_start_pos, i - word_start_pos);
                        word_start = false;
                    }

                    if (name.Length == 0)
                    {
                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Name does not exist before :");
                        return false;
                    }

                    colon_start = true;
                }
                else if (ch == ',') // class인 경우 다중 상속일 경우만 존재한다.
                {
                    if (!colon_start)   // , 는 세미콜론 다음에만 있다.
                    {
                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "Invalid comma");
                        return false;
                    }

                    if (word_start)
                    {
                        buf = body.Substring(word_start_pos, i - word_start_pos);
                        sbc = new ScriptBaseClass();
                        sbc.name = buf;
                        sbc.nColumn = col_pos;
                        sbc.nRow = row_pos;
                        base_class_names.Add(sbc);

                        word_start = false;
                    }
                }
                else if (ch == ' ' || ch == '\t' || ch == '\n')
                {
                    if (colon_start)
                    {
                        if (word_start)
                        {
                            buf = body.Substring(word_start_pos, i - word_start_pos);
                            sbc = new ScriptBaseClass();
                            sbc.name = buf;
                            sbc.nColumn = col_pos;
                            sbc.nRow = row_pos;
                            base_class_names.Add(sbc);
                            
                        }
                    }
                    else
                    {
                        if (word_start)
                        {
                            buf = body.Substring(word_start_pos, i - word_start_pos);
                            name = buf;
                        }
                    }

                    word_start = false;
                }
                else
                {
                    if (ch == '/')
                    {
                        //if (!SkipDescription(body, ref i, ref col_pos, ref row_pos)) return false;
                        bool? retn_description = CheckDescription(file, body, ref i, ref col_pos, ref row_pos);
                        if (retn_description == null) return false;  // 오류가 발생했다.
                        if (retn_description == true) continue;
                    }
                    else
                    {
                        if (!word_start)
                        {
                            word_start = true;
                            word_start_pos = i;
                        }
                    }
                }

                CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "{ 을 찾을 수 없습니다.");
            return false;
        }

        /// <summary>
        /// 사용자가 만든 클래스를 모두 삭제한다. 컴파일을 새로 하기전에 이미 등록된 사용자 클래스를 삭제해야 중복되지 않는다.
        /// </summary>
        public void RemoveAllUserClass()
        {
            for (int i = arrayLibrary.Count-1; i >= 0; i--)
            {
                ScriptLibLibrary sll = arrayLibrary[i];

                if(!sll.bInternalLibrary)
                    arrayLibrary.RemoveAt(i);
            }

            bErrorFlag = false; 
            arrayError.Clear();
        }

        public bool SeekClassWithError(EditScriptLibNamespace own_namespace, EditScriptLibFile file, ref string name_namespace, string name_class, out ScriptLibClass retn_slc, int col_pos, int row_pos)
        {
            ScriptLibClass slc;
            ScriptLibNamespace sln;
            retn_slc = null;
            List<string> array_namespace = new List<string>();

            if (name_namespace == null)
            {
                // namespace명이 없으면 먼저 자신의 namespace에서 찾아 본다.
                for (int j = 0; j < own_namespace.arrayMember.Count; j++)
                {
                    if (own_namespace.arrayMember[j].eMember != EnumScriptLibMember.Class)
                    {
                        continue;
                    }
                    slc = (ScriptLibClass) own_namespace.arrayMember[j];
                    if (slc.sNameClass == name_class)
                    {
                        name_namespace = own_namespace.sNameNamespace;
                        retn_slc = slc;
                        return true;
                    }
                }

                for (int l = 0; l < arrayLibrary.Count; l++)
                {
                    for (int i = 0; i < arrayLibrary[l].arrayNamespace.Count; i++)
                    {
                        sln = arrayLibrary[l].arrayNamespace[i];

                        for (int j = 0; j < file.arrayUsing.Count; j++)
                        {
                            if (file.arrayUsing[j] == sln.sNameNamespace) goto ok_using_exist;
                        }
                        continue;

                    ok_using_exist:

                        for (int j = 0; j < sln.arrayMember.Count; j++)
                        {
                            if (sln.arrayMember[j].eMember != EnumScriptLibMember.Class)
                            {
                                continue;
                            }

                            slc = (ScriptLibClass)sln.arrayMember[j];
                            if (slc.sNameClass == name_class)
                            {
                                name_namespace = sln.sNameNamespace;
                                retn_slc = slc;
                                array_namespace.Add(sln.sNameNamespace);
                            }
                        }
                    }
                }
            }
            else // 네임스페이스명이 있는 경우는 해당 위치에서 찾는다.
            {
                for (int l = 0; l < arrayLibrary.Count; l++)
                {
                    for (int i = 0; i < arrayLibrary[l].arrayNamespace.Count; i++)
                    {
                        sln = arrayLibrary[l].arrayNamespace[i];

                        if (sln.sNameNamespace == name_namespace)
                        {
                            for (int j = 0; j < sln.arrayMember.Count; j++)
                            {
                                if (sln.arrayMember[j].eMember != EnumScriptLibMember.Class)
                                {
                                    continue;
                                }

                                slc = (ScriptLibClass)sln.arrayMember[j];
                                if (slc.sNameClass == name_class)
                                {
                                    retn_slc = slc;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            if (array_namespace.Count == 0)
            {
                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The class '{0}' could not be found", name_class);
                return false;
            }
            else if (array_namespace.Count > 1)
            {
                string msg = String.Format("The class '{0}' is found in the multi namespace ", name_class);
                for (int i = 0; i < array_namespace.Count; i++)
                {
                    msg += array_namespace[i] + ",";
                }

                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, msg);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 주어인 메소드가 static 이면 true를 반환하고 아니면 오류를 표시하고 false를 반환한다.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="slm"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        bool IsStaticMethodWithError(EditScriptLibFile file, ScriptLibClass eslc, ScriptLibMemberMethod slm, int col_pos, int row_pos)
        {
            if (eslc.sNameClass == slm.sNameMethod)
            {
                // 생성자는 static일 필요가 없다.  DateTime t = new DateTime(); 에서 호출된 경우의 Method이다.
            }
            else
            {
                if (!slm.bStatic)
                {
                    SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The Method/Property '{0}' is must be static", slm.sNameMethod);
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// 주어인 메소드나 변수가 static 이면 true를 반환하고 아니면 오류를 표시하고 false를 반환한다.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="slm"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        bool IsStaticMethodVariableWithError(EditScriptLibFile file, ScriptLibClass eslc, ScriptLibMemberPublic slmp, int col_pos, int row_pos, bool check_public)
        {
            if (slmp.eMember == EnumScriptLibMember.Method)
            {
                ScriptLibMemberMethod slm = (ScriptLibMemberMethod)slmp;
                if (eslc.sNameClass == slm.sNameMethod)
                {
                    // 생성자는 static일 필요가 없다.  DateTime t = new DateTime(); 에서 호출된 경우의 Method이다.
                }
                else
                {
                    if (!slm.bStatic)
                    {
                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The Method/Property '{0}' is must be static", slm.sNameMethod);
                        return false;
                    }

                    if (check_public && slm.eAccessLevel != EnumAccessLevel.access_public)
                    {
                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The Method/Property '{0}.{1}'' is inaccessible due to its protection level.", eslc.sNameClass, slm.sNameMethod);
                        return false;
                    }
                }
            }
            else
            {
                ScriptLibMemberVariable slmv = (ScriptLibMemberVariable)slmp;
                if (eslc.sNameClass == slmv.pVar.sVarName)
                {
                    // 생성자는 static일 필요가 없다.  DateTime t = new DateTime(); 에서 호출된 경우의 Method이다.
                }
                else
                {
                    if (!slmv.bStatic && !slmv.pVar.bConst)  // 변수는 static이나 const 모두 다이렉트로 호출할 수 있다.
                    {
                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The Variable '{0}' is must be static or const", slmv.pVar.sVarName);
                        return false;
                    }

                    if (check_public && slmv.eAccessLevel != EnumAccessLevel.access_public)
                    {
                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The Variable '{0}.{1}'' is inaccessible due to its protection level.", eslc.sNameClass, slmv.pVar.sVarName);
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// static 변수나 static Property를 찾는다.
        /// </summary>
        /// <param name="own_class"></param>
        /// <param name="file"></param>
        /// <param name="name_namespace"></param>
        /// <param name="name_class"></param>
        /// <param name="name_method"></param>
        /// <param name="retn_slm"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        public bool SeekStaticPropertyVariableWithError(EditScriptLibClass own_class, EditScriptLibFile file, ref string name_namespace, ref string name_class, string name_method, out ScriptLibMemberPublic retn_slm, int col_pos, int row_pos)
        {
            ScriptLibClass slc;
            ScriptLibNamespace sln;
            ScriptLibClass seeked_slc = null;

            retn_slm = null;

            // 클래스명이 지정되지 않았으므로 자신의 클래스에서 찾는다.
            if (name_class == null)
            {
                retn_slm = own_class.GetMethodVariablePointer(name_method);

                if (retn_slm != null)
                {
                    // 자신의 클래스에서는 static이 public일 필요는 없음
                    if (!IsStaticMethodVariableWithError(file, own_class, retn_slm, col_pos, row_pos, false)) return false;

                    name_namespace = own_class.sNameClass;
                    name_class = own_class.sNameClass;

                    return true;
                }

                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The method '{0}' could not be found in the own_class", name_method);
                return false;
            }

            List<string> array_namespace = new List<string>();

            EditScriptLibNamespace own_namespace = (EditScriptLibNamespace)own_class.parentNamespace;
            ScriptLibMemberPublic slmp;

            if (name_namespace == null)
            {
                // namespace명이 없으면 먼저 자신의 namespace에서 찾아 본다.
                for (int i = 0; i < own_namespace.arrayMember.Count; i++)
                {
                    slmp = own_namespace.arrayMember[i];
                    if (slmp.GetName() == name_class)
                    {
                        if (slmp.eMember == EnumScriptLibMember.Class)
                        {
                            slc = (ScriptLibClass)slmp;
                            retn_slm = slc.GetMethodVariablePointer(name_method);

                            if (retn_slm != null)
                            {
                                // 자신의 클래스에서는 static이 public을 체크할 필요는 없음
                                bool check_public = name_class == own_class.sNameClass ? false : true;
                                if (!IsStaticMethodVariableWithError(file, slc, retn_slm, col_pos, row_pos, check_public)) return false;

                                name_namespace = own_namespace.sNameNamespace;
                                return true;
                            }

                            SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The method '{0}.{1}' could not be found in the class", name_class, name_method);
                            return false;
                        }
                        else
                        {
                            ScriptLibEnum sle = (ScriptLibEnum)slmp;
                            retn_slm = sle.GetItemPointer(name_method);

                            if (retn_slm != null)
                            {
                                name_namespace = own_namespace.sNameNamespace;
                                return true;
                            }

                            SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The name '{0}.{1}' could not be found in the Enum", name_class, name_method);
                            return false;
                        }
                    }
                }

                // 자신의 namespace에서 없으면 다른 네임스페이스에서 찾아본다.
                for (int l = 0; l < arrayLibrary.Count; l++)
                {
                    for (int i = 0; i < arrayLibrary[l].arrayNamespace.Count; i++)
                    {
                        sln = arrayLibrary[l].arrayNamespace[i];

                        for (int j = 0; j < file.arrayUsing.Count; j++)
                        {
                            if (file.arrayUsing[j] == sln.sNameNamespace) goto ok_using_exist;
                        }
                        continue;

                    ok_using_exist:

                        for (int j = 0; j < sln.arrayMember.Count; j++)
                        {
                            slmp = sln.arrayMember[j];

                            if (slmp.GetName() == name_class)
                            {
                                if (slmp.eMember == EnumScriptLibMember.Class)
                                {
                                    slc = (ScriptLibClass)slmp;
                                    retn_slm = slc.GetPropertyPointer(name_method);

                                    if (!IsStaticMethodVariableWithError(file, slc, retn_slm, col_pos, row_pos, true)) return false;

                                    name_namespace = sln.sNameNamespace;
                                    seeked_slc = slc;
                                    array_namespace.Add(sln.sNameNamespace);
                                }
                                else
                                {
                                    /*
                                    ScriptLibEnum sle = (ScriptLibEnum)slmp;
                                    retn_slm = sle.GetItemPointer(name_method);

                                    if (retn_slm != null)
                                    {
                                        name_namespace = own_namespace.sNameNamespace;
                                        return true;
                                    }

                                    SetError(file.sSourceFilename, col_pos, row_pos, "The name '{0}.{1}' could not be found in the Enum", name_class, name_method);
                                    return false;*/
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int l = 0; l < arrayLibrary.Count; l++)
                {
                    for (int i = 0; i < arrayLibrary[l].arrayNamespace.Count; i++)
                    {
                        sln = arrayLibrary[l].arrayNamespace[i];

                        if (sln.sNameNamespace == name_namespace)
                        {
                            for (int j = 0; j < sln.arrayMember.Count; j++)
                            {
                                slmp = sln.arrayMember[i];
                                if (slmp.GetName() == name_class)
                                {
                                    if (slmp.eMember == EnumScriptLibMember.Class)
                                    {
                                        slc = (ScriptLibClass)slmp;
                                        retn_slm = slc.GetPropertyPointer(name_method);

                                        if (retn_slm != null)
                                        {
                                            if (!IsStaticMethodVariableWithError(file, slc, retn_slm, col_pos, row_pos, true)) return false;

                                            return true;
                                        }

                                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The method '{0}' could not be found in the project", name_method);

                                        return false;
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (array_namespace.Count == 0)
            {
                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The class '{0}' could not be found", name_class);
                return false;
            }
            else if (array_namespace.Count > 1)
            {
                string msg = String.Format("The class '{0}' is found in the multi namespace ", name_class);
                for (int i = 0; i < array_namespace.Count; i++)
                {
                    msg += array_namespace[i] + ",";
                }

                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, msg);
                return false;
            }

            retn_slm = seeked_slc.GetPropertyPointer(name_method);

            if (retn_slm != null)
            {
                if (!IsStaticMethodVariableWithError(file, seeked_slc, retn_slm, col_pos, row_pos, true)) return false;

                return true;
            }

            SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The method '{0}' could not be found in the project", name_method);

            return true;
        }

        /// <summary>
        /// static Method나 static Property를 찾는다.
        /// </summary>
        /// <param name="own_class"></param>
        /// <param name="file"></param>
        /// <param name="name_namespace"></param>
        /// <param name="name_class"></param>
        /// <param name="name_method"></param>
        /// <param name="retn_slm"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        public bool SeekStaticMethodWithError(EditScriptLibClass own_class, EditScriptLibFile file, ref string name_namespace, ref string name_class, string name_method, out ScriptLibMemberMethod retn_slm, int col_pos, int row_pos, List<CommandMethodArg> args)
        {
            ScriptLibClass slc;
            ScriptLibNamespace sln;
            ScriptLibClass seeked_slc = null;

            retn_slm = null;

            // 클래스명이 지정되지 않았으므로 자신의 클래스에서 찾는다.
            if (name_class == null)
            {
                retn_slm = own_class.GetMethodPointer(this, file.sProjectName, file.sSourceFilename, col_pos, row_pos, name_method, args);

                if (retn_slm != null)
                {
                    if (!IsStaticMethodWithError(file, own_class, retn_slm, col_pos, row_pos)) return false;

                    name_namespace = own_class.sNameClass;
                    name_class = own_class.sNameClass;

                    return true;
                }

                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The method '{0}' could not be found in the own_class", name_method);
                return false;
            }

            List<string> array_namespace = new List<string>();

            EditScriptLibNamespace own_namespace = (EditScriptLibNamespace)own_class.parentNamespace;

            if (name_namespace == null)
            {
                // namespace명이 없으면 먼저 자신의 namespace에서 찾아 본다.
                for (int i = 0; i < own_namespace.arrayMember.Count; i++)
                {
                    if (own_namespace.arrayMember[i].eMember != EnumScriptLibMember.Class)
                    {
                        continue;
                    }

                    slc = (EditScriptLibClass)own_namespace.arrayMember[i];
                    if (slc.sNameClass == name_class)
                    {
                        retn_slm = slc.GetMethodPointer(this, file.sProjectName, file.sSourceFilename, col_pos, row_pos, name_method, args);

                        if (retn_slm != null)
                        {
                            if (!IsStaticMethodWithError(file, slc, retn_slm, col_pos, row_pos)) return false;

                            name_namespace = own_namespace.sNameNamespace;
                            return true;
                        }

                        SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The method '{0}.{1}' could not be found in the own_namespace", name_class, name_method);
                        return false;
                    }
                }

                for (int l = 0; l < arrayLibrary.Count; l++)
                {
                    for (int i = 0; i < arrayLibrary[l].arrayNamespace.Count; i++)
                    {
                        sln = arrayLibrary[l].arrayNamespace[i];

                        for (int j = 0; j < file.arrayUsing.Count; j++)
                        {
                            if (file.arrayUsing[j] == sln.sNameNamespace) goto ok_using_exist;
                        }
                        continue;

                    ok_using_exist:

                        for (int j = 0; j < sln.arrayMember.Count; j++)
                        {
                            if (sln.arrayMember[j].eMember != EnumScriptLibMember.Class)
                            {
                                continue;
                            }

                            slc = (ScriptLibClass)sln.arrayMember[j];
                            if (slc.sNameClass == name_class)
                            {
                                retn_slm = slc.GetMethodPointer(this, file.sProjectName, file.sSourceFilename, col_pos, row_pos, name_method, args);

                                if (retn_slm == null)
                                {
                                    // 메시지는 slc.GetMethodPointer 에서 보여준다.
                                    //SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, "The method '{0}' could not be found in the '{1}' class", name_method, name_class);
                                    return false;
                                }

                                if (!IsStaticMethodWithError(file, slc, retn_slm, col_pos, row_pos)) return false;

                                name_namespace = sln.sNameNamespace;
                                seeked_slc = slc;
                                array_namespace.Add(sln.sNameNamespace);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int l = 0; l < arrayLibrary.Count; l++)
                {
                    for (int i = 0; i < arrayLibrary[l].arrayNamespace.Count; i++)
                    {
                        sln = arrayLibrary[l].arrayNamespace[i];

                        if (sln.sNameNamespace == name_namespace)
                        {
                            for (int j = 0; j < sln.arrayMember.Count; j++)
                            {
                                if (sln.arrayMember[j].eMember != EnumScriptLibMember.Class)
                                {
                                    continue;
                                }

                                slc = (ScriptLibClass)sln.arrayMember[j];
                                if (slc.sNameClass == name_class)
                                {
                                    retn_slm = slc.GetMethodPointer(this, file.sProjectName, file.sSourceFilename, col_pos, row_pos, name_method, args);

                                    if (retn_slm != null)
                                    {
                                        if (!IsStaticMethodWithError(file, slc, retn_slm, col_pos, row_pos)) return false;

                                        return true;
                                    }

                                    //SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, "The method '{0}' could not be found in the project", name_method);

                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            if (array_namespace.Count == 0)
            {
                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The class '{0}' could not be found", name_class);
                return false;
            }
            else if (array_namespace.Count > 1)
            {
                string msg = String.Format("The class '{0}' is found in the multi namespace ", name_class);
                for (int i = 0; i < array_namespace.Count; i++)
                {
                    msg += array_namespace[i] + ",";
                }

                SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, msg);
                return false;
            }

            retn_slm = seeked_slc.GetMethodPointer(this, file.sProjectName, file.sSourceFilename, col_pos, row_pos, name_method, args);

            if (retn_slm != null)
            {
                if (!IsStaticMethodWithError(file, seeked_slc, retn_slm, col_pos, row_pos)) return false;

                return true;
            }

            SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The method '{0}' could not be found in the project", name_method);

            return true;
        }
    }


}
