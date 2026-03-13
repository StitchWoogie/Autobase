using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;

namespace ScriptLibEdit
{
    public class EditCommandBlock : CommandBlock
    {
        public EditCommandBlock(EditScriptLibMethod parent, CommandPublic parent_block) 
            : base(parent, parent_block)
        {
        }

        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos, bool method_main_block)
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

            if (parentBlock == null && method_main_block)
            {
                // Method의 마지막에 LastLine을 넣어서 디버그하기 편하게 한다.
                AddLastLine(main, file, col_pos, row_pos);
            }

            return true;
        }

        public override void Compile(ScriptLibMain main)
        {
            for (int i = 0; i < arrayCommand.Count; i++)
            {
                arrayCommand[i].Compile(main);
            }
        }

        bool AddLastLine(EditScriptLibMain main, EditScriptLibFile file, int col_pos, int row_pos)
        {
            EditCommandLastLine command = new EditCommandLastLine((EditScriptLibMethod)parentMethod, this);
            command.SetColRow(col_pos, row_pos);
            arrayCommand.Add(command);

            return true;
        }
        
        // 선언과 대입이 같이 있는 경우의 문장 int a = 10; 과 같은 문장일 경우 선언문을 추가하고 대입문을 추가한다.
        bool AddDeclaration(EditScriptLibMain main, EditScriptLibFile file, SplitedString first_word, SplitedString array_word, SplitedString second_word, string sBody, ref int pos, int col_pos, int row_pos, bool bConst)
        {
            if (IsExistVariableOnSplit(second_word.block))
            {
                if (Tools.IsLangKorean())
                    main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "같은 이름의 변수가 이미 정의되어 있습니다. '{0}'", second_word.block);
                else
                    main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "A local variable named '{0}' is already defined in this scope", second_word.block);
                return false;
            }

            EditCommandDeclaration command = new EditCommandDeclaration((EditScriptLibMethod)parentMethod, this);

            string value_string = null;

            if (sBody[pos] == '=')  // 대입문이 있다.
            {
                if(sBody[sBody.Length-1] == ';' || sBody[sBody.Length-1] == ',')
                    value_string = sBody.Substring(pos + 1, sBody.Length - pos - 2);   // 맨끝에 붙어있는 ; 를 제거하고 보관한다.
                else
                    value_string = sBody.Substring(pos + 1);   // {} 인 경우는 블럭을 그대로 준다.

                //
                //if (String.Compare(value_string, 0, " new byte[", 0, 10) == 0)
                //{
                //    int kkk = 100;
                //}
            }

            if(!command.Split(main, file, first_word, array_word, second_word, value_string, col_pos+1, row_pos, bConst)) return false;

            arrayCommand.Add(command);

            return true;
        }

        bool AddSubstitution(EditScriptLibMain main, EditScriptLibFile file, SplitedString first_word, SplitedString array_word, string sBody, ref int pos, int col_pos, int row_pos)
        {
            EditCommandSubstitution command = new EditCommandSubstitution((EditScriptLibMethod)parentMethod, this);

            command.SetColRow(first_word.col_pos, first_word.row_pos);

            if (array_word != null)
            {
                first_word.block += array_word.block;
            }
            ((EditRecursiveValue)command.valueTarget).Split(main, file, first_word.block, first_word.col_pos, first_word.row_pos);

            string value = sBody.Substring(pos + 1, sBody.Length - pos - 2);   // 맨끝에 붙어있는 ; 를 제거하고 보관한다.
            if (!((EditRecursiveValue)command.valueSource).Split(main, file, value, col_pos+1, row_pos)) return false;

            arrayCommand.Add(command);

            return true;
        }

        // x++, ++x, x--, y-- 같이 RecursiveValue도 되면서 Method의 기능을 하는데 실행을 하였으나 반환값이 필요없는 경우
        // CommandMethod 도 여기에 같이 사용할 수 있으나 이미 RecursiveValue와 BlockCommand 에 구분되어 있으므로 그냥 두었다.
        bool AddCommandVoidValue(EditScriptLibMain main, EditScriptLibFile file, SplitedString block)
        {
            EditCommandVoidValue command = new EditCommandVoidValue((EditScriptLibMethod)parentMethod, this);

            command.SetColRow(block.col_pos, block.row_pos);
            command.pValue = new EditRecursiveValue(parentMethod, this);

            string body = block.block;
            if (body[body.Length - 1] == ';' || body[body.Length - 1] == ',')   // 
            {
                body = body.Substring(0, body.Length - 1);  // 끝에 붙은 ; 는 제거한다.
            }

            ((EditRecursiveValue)command.pValue).Split(main, file, body, block.col_pos, block.row_pos);

            arrayCommand.Add(command);

            return true;
        }

        // +=, -=, *- ... 같은 식을 추가할 때 사용
        bool AddSubstitutionPlusEqual(EditScriptLibMain main, EditScriptLibFile file, SplitedString first_word, char add_char, string sBody, ref int pos, int col_pos, int row_pos)
        {
            EditCommandSubstitution command = new EditCommandSubstitution((EditScriptLibMethod)parentMethod, this);

            command.SetColRow(first_word.col_pos, first_word.row_pos);

            ((EditRecursiveValue)command.valueTarget).Split(main, file, first_word.block, col_pos, row_pos);

            string value = sBody.Substring(pos + 1, sBody.Length - pos - 2);   // 맨끝에 붙어있는 ; 를 제거하고 보관한다.

            value = first_word.block + add_char + "("+ value +")";   // i+=1 같은 형식을 i=i+(1) 같은 형식으로 바꿔준다

            col_pos -= first_word.block.Length; 
            
            if (!((EditRecursiveValue)command.valueSource).Split(main, file, value, col_pos, row_pos)) return false;

            arrayCommand.Add(command);

            return true;
        }

        bool AddMethod(EditScriptLibMain main, EditScriptLibFile file, string first_word, string sBody, ref int col_pos, ref int row_pos)
        {
            EditCommandMethod command = new EditCommandMethod((EditScriptLibMethod)parentMethod, this);

            if(!command.Split(main, file, sBody, col_pos, row_pos))    return false;

            arrayCommand.Add(command);

            return true;
        }

        // 이전의 command가 if 인가를 검사한다.
        bool IsPreCommandIsIf()
        {
            if (arrayCommand.Count == 0) return false;
            if (arrayCommand[arrayCommand.Count - 1].eCommandType == EnumCommandType.type_if)
            {
                return true;
            }

            return false;
        }

        bool AddIf(EditScriptLibMain main, EditScriptLibFile file, string sBody, int i, int col_pos, int row_pos, bool else_flag)
        {
            if (else_flag && !IsPreCommandIsIf())
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "if문이 없고 else if가 먼저 선언되었습니다");
                return false;
            }

            // ( 다음부터 계산해야 한다.
            main.CalcCursorPos(sBody[i], ref col_pos, ref row_pos);
            i++;

            int start_col_pos;
            int start_row_pos;
            int block_start_pos;
            int block_end_pos;

            start_col_pos = col_pos;
            start_row_pos = row_pos;

            if (GetSentenceToCharOrRightParenthesis(main, file, sBody, ref i, sBody.Length - 1, ref col_pos, ref row_pos, out block_start_pos, out block_end_pos, ')') == 0)
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "if 문장의 끝이 ) 로 끝나야 합니다.");
                return false;
            }

            EditCommandIf command = new EditCommandIf((EditScriptLibMethod)parentMethod, this);
            command.SetColRow(start_col_pos, start_row_pos);

            string condition = sBody.Substring(block_start_pos, block_end_pos - block_start_pos);
            ((EditRecursiveCondition)command.pCondition).Split(main, file, condition, start_col_pos, start_row_pos);
            command.bElseIf = else_flag;

            // ) 다음에 있는 모든 문장이 if문일 때 실행할 문장이다.
            string blocks = sBody.Substring(block_end_pos+1);

            if(!((EditCommandBlock)command.blockCommand).Split(main, file, blocks, col_pos + 1, row_pos, false))   return false; 

            arrayCommand.Add(command);

            return true;
        }

        bool AddElse(EditScriptLibMain main, EditScriptLibFile file, string sBody, ref int i, ref int col_pos, ref int row_pos)
        {
            if (!IsPreCommandIsIf())
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "if문이 없고 else가 먼저 선언 되었습니다");
                return false;
            }

            EditCommandElse command = new EditCommandElse((EditScriptLibMethod)parentMethod, this);

            // ) 다음에 있는 모든 문장이 else문일 때 실행할 문장이다.
            string blocks = sBody.Substring(i);

            if (!((EditCommandBlock)command.blockCommand).Split(main, file, blocks, col_pos, row_pos, false)) return false;

            arrayCommand.Add(command);

            return true;
        }

        bool AddWhile(EditScriptLibMain main, EditScriptLibFile file, string sBody, int i, int col_pos, int row_pos)
        {
            // ( 다음부터 계산해야 한다.
            main.CalcCursorPos(sBody[i], ref col_pos, ref row_pos);
            i++;

            int start_col_pos;
            int start_row_pos;
            int block_start_pos;
            int block_end_pos;

            start_col_pos = col_pos;
            start_row_pos = row_pos;
            if (GetSentenceToCharOrRightParenthesis(main, file, sBody, ref i, sBody.Length - 1, ref col_pos, ref row_pos, out block_start_pos, out block_end_pos, ')') == 0)
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "while 문장의 끝이 ) 로 끝나야 합니다.");
                return false;
            }

            EditCommandWhile command = new EditCommandWhile((EditScriptLibMethod)parentMethod, this);

            string condition = sBody.Substring(block_start_pos, block_end_pos - block_start_pos);
            ((EditRecursiveCondition)command.pCondition).Split(main, file, condition, col_pos, row_pos);

            // ) 다음에 있는 모든 문장이 while문일 때 실행할 문장이다.
            string blocks = sBody.Substring(block_end_pos + 1);

            command.blockCommand = new EditCommandBlock((EditScriptLibMethod)parentMethod, command);
            if (!((EditCommandBlock)command.blockCommand).Split(main, file, blocks, col_pos + 1, row_pos, false)) return false;

            command.SetColRow(start_col_pos, start_row_pos);
            arrayCommand.Add(command);

            return true;
        }

        bool AddFor(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos)
        {
            EditCommandFor command = new EditCommandFor((EditScriptLibMethod)parentMethod, this);

            if (!command.Split(main, file, sBody, col_pos, row_pos)) return false;

            arrayCommand.Add(command);

            return true;
        }

        bool AddReturn(EditScriptLibMain main, EditScriptLibFile file, string sBody, SplitedString second_word, int i, int col_pos, int row_pos)
        {
            string value_block;

            if (sBody[i] == ';')    // 문장이 끝났다.
            {
                if (second_word != null) 
                    value_block = second_word.block;

                else value_block = "";
            }
            else
            {
                value_block = sBody.Substring(i+1, (sBody.Length - i-2));
            }

            EditCommandReturn command = new EditCommandReturn((EditScriptLibMethod)parentMethod, this);
            if (value_block.Length > 0)
            {
                command.pValue = new EditRecursiveValue(parentMethod, this);
                ((EditRecursiveValue)command.pValue).Split(main, file, value_block, col_pos, row_pos);
            }

            command.SetColRow(col_pos, row_pos);    // 컬럼정보를 준다. 디버거 시 필요하다.
            arrayCommand.Add(command);

            return true;
        }

        // 부모의 명령어가 Loop (for, while, foreach) 문인가를 검사한다.
        bool IsParentLoopCommand(CommandPublic parent)
        {
            if (parent.eCommandType == EnumCommandType.type_for) return true;
            if (parent.eCommandType == EnumCommandType.type_foreach) return true;
            if (parent.eCommandType == EnumCommandType.type_while) return true;

            if (parent.eCommandType == EnumCommandType.type_block)
                return IsParentLoopCommand(((EditCommandBlock)parent).parentBlock);

            return false;
        }

        bool AddBreak(EditScriptLibMain main, EditScriptLibFile file, string sBody, SplitedString second_word, int i, int col_pos, int row_pos)
        {
            if (second_word != null)
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "break 뒤는 단어가 올 수 없고 ; 로 끝나야 합니다.");
                return false;
            }

            if (!IsParentLoopCommand(parentBlock))
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "break 문은 Loop 문(for/foreach/while ...) 에서만 사용할 수 있습니다.");
                return false;
            }
                        
            EditCommandBreak command = new EditCommandBreak((EditScriptLibMethod)parentMethod, this);

            command.SetColRow(col_pos, row_pos);    
            arrayCommand.Add(command);

            return true;
        }

        bool AddContinue(EditScriptLibMain main, EditScriptLibFile file, string sBody, SplitedString second_word, int i, int col_pos, int row_pos)
        {
            if (second_word != null)
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "continue 뒤는 단어가 올 수 없고 ; 로 끝나야 합니다.");
                return false;
            }

            if (!IsParentLoopCommand(parentBlock))
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "continue 문은 Loop 문(for/foreach/while ...) 에서만 사용할 수 있습니다.");
                return false;
            }

            EditCommandContinue command = new EditCommandContinue((EditScriptLibMethod)parentMethod, this);

            command.SetColRow(col_pos, row_pos);    
            arrayCommand.Add(command);

            return true;
        }

        bool AddGoto(EditScriptLibMain main, EditScriptLibFile file, string sBody, SplitedString second_word, int i, int col_pos, int row_pos)
        {
            if (second_word == null)
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "goto 문의 label 이름이 없습니다.");
                return false;
            }

            EditCommandGoto command = new EditCommandGoto((EditScriptLibMethod)parentMethod, this);
            command.sNameLabel = second_word.block;
            command.SetColRow(col_pos, row_pos);
            arrayCommand.Add(command);

            return true;
        }

        // 부모의 명령어가 Loop (for, while, foreach) 문인가를 검사한다.
        public override bool IsExistLabel(string name)
        {
            for (int i = 0; i < arrayCommand.Count; i++)
            {
                if (arrayCommand[i].eCommandType == EnumCommandType.type_label)
                {
                    CommandLabel ce = (CommandLabel)arrayCommand[i];
                    if (ce.sNameLabel == name) return true;
                }
                    
            }

            if (parentBlock != null)
            {
                return parentBlock.IsExistLabel(name);
            }

            return false;
        }
        
        bool AddLabel(EditScriptLibMain main, EditScriptLibFile file, string sBody, string first_word, SplitedString second_word, int i, int col_pos, int row_pos)
        {
            if (second_word != null)
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "label 문의 이름은 한단어로 구성되어야 합니다.");
                return false;
            }

            if (IsExistLabel(first_word))
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "같은 이름의 label 이 이미 존재합니다.");
                return false;
            }

            EditCommandLabel command = new EditCommandLabel((EditScriptLibMethod)parentMethod, this);
            command.sNameLabel = first_word;
            command.SetColRow(col_pos, row_pos);
            arrayCommand.Add(command);

            return true;
        }

        /// <summary>
        /// int a=0,b; => int a=0,   와 b; 로 분리된다.
        /// a++, b++;  => a++,       와 b++;로 분리된다.
        /// label: a=b; => label:    와 a=b; 로 분리된다.
        /// 
        /// 처럼 , 로 구분되어 있는 블럭을 분해한다. 최소한 1개가 있으며 ; 를 찾을 때 까지나 끝날 때 까지 계속한다.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="file"></param>
        /// <param name="sBody"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        /// <returns></returns>
        public static bool SplitBlockByCommaAndColon(EditScriptLibMain main, EditScriptLibFile file, string sBody, ref int col_pos, ref int row_pos, List<SplitedString> array_names)
        {
            int word_start_pos = 0;
            bool word_start = false;
            int start_col_pos = 0;
            int start_row_pos = 0;
            char ch;

            int count_parenthesis = 0;      // ()
            int count_curlybracket = 0;
            int count_squrebracket = 0; // []

            SplitedString cs;

            for (int i = 0; i < sBody.Length; i++)
            {
                ch = sBody[i];

                if (ch == '"')
                {
                    if (!main.SkipQuotationMark(file, sBody, ref i, sBody.Length-1, ref col_pos, ref row_pos)) return false;
                }
                else if (ch == '\'')
                {
                    if (!main.SkipApostrophe(file, sBody, ref i, sBody.Length - 1, ref col_pos, ref row_pos)) return false;
                }
                else if (ch == ';')
                {
                    if (count_parenthesis == 0 && count_curlybracket == 0) // () 안에 있는 , 는 인자의 구분이다. 
                    {
                        if (word_start)
                        {
                            cs = new SplitedString();
                            cs.block = sBody.Substring(word_start_pos, i - word_start_pos + 1);   // ; 도 포함한다.
                            cs.col_pos = start_col_pos;
                            cs.row_pos = start_row_pos;

                            array_names.Add(cs);

                            word_start = false;
                        }
                    }
                }
                else if (ch == ':')     // a = b ? c : d;       이런 형식은 아직 처리 안함
                {
                    if (count_parenthesis == 0 && count_curlybracket == 0) // 
                    {
                        if (word_start)
                        {
                            cs = new SplitedString();
                            cs.block = sBody.Substring(word_start_pos, i - word_start_pos + 1);   // : 도 포함한다.
                            cs.col_pos = start_col_pos;
                            cs.row_pos = start_row_pos;

                            array_names.Add(cs);

                            word_start = false;
                        }
                    }
                }
                else if (ch == '(')
                {
                    count_parenthesis++;
                }
                else if (ch == ')')
                {
                    count_parenthesis--;
                }
                else if (ch == '{')
                {
                    count_curlybracket++;
                }
                else if (ch == '}')
                {
                    count_curlybracket--;
                }
                else if (ch == '[')
                {
                    count_squrebracket++;
                }
                else if (ch == ']')
                {
                    count_squrebracket--;
                }
                else if (ch == ',')
                {
                    if (count_parenthesis == 0 && count_curlybracket == 0 && count_squrebracket == 0) // () 안에 있는 , 는 인자의 구분이다. 
                    {
                        if (word_start)
                        {
                            cs = new SplitedString();
                            cs.block = sBody.Substring(word_start_pos, i - word_start_pos + 1);  // , 도 포함한다.
                            cs.col_pos = start_col_pos;
                            cs.row_pos = start_row_pos;

                            array_names.Add(cs);

                            word_start = false;
                        }
                    }
                }
                else if (!word_start)
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
                    /*
                    else if (ch == '/')
                    {
                        bool? retn_description = main.CheckDescription(sBody, ref i, ref col_pos, ref row_pos);
                        if (retn_description == null) return false;  // 오류가 발생했다.
                        if (retn_description == true) continue;
                    }*/
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

            if (word_start)
            {
                cs = new SplitedString();
                cs.block = sBody.Substring(word_start_pos);
                cs.col_pos = start_col_pos;
                cs.row_pos = start_row_pos;

                array_names.Add(cs);

                word_start = false;
            }

            return true;
        }

        bool GetWord(EditScriptLibMain main, EditScriptLibFile file, string sBody, ref int i, ref int word_start_pos, ref SplitedString first_word, ref SplitedString second_word, int word_col_pos, int word_row_pos)
        {
            if (word_start_pos != -1)
            {
                if (first_word == null)
                {
                    first_word = new SplitedString();
                    first_word.block = sBody.Substring(word_start_pos, i - word_start_pos);
                    first_word.col_pos = word_col_pos;
                    first_word.row_pos = word_row_pos;
                }
                else if (second_word == null)
                {
                    second_word = new SplitedString();
                    second_word.block = sBody.Substring(word_start_pos, i - word_start_pos);
                    second_word.col_pos = word_col_pos;
                    second_word.row_pos = word_row_pos;
                }
                else
                {
                    main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, word_col_pos, word_row_pos, EnumScriptErrorType.Else, "단어가 연속해서 3개가 나열될 수 없습니다. FirstWord={0},SecondWord={1}", first_word.block, second_word.block);
                    return false;
                }

                word_start_pos = -1;
            }

            return true;
        }

        bool CheckNextIf(string sBody, int pos)
        {
            StringBuilder s = new StringBuilder();
            for (int i = pos; i < sBody.Length; i++)
            {
                if (sBody[i] == ' ' ||
                    sBody[i] == '\t' ||
                    sBody[i] == '\n')
                {

                }
                else
                {
                    s.Append(sBody[i]);
                }
            }

            string buf = s.ToString();

            if (String.Compare(buf, 0, "if(", 0, 3) == 0)
            {
                return true;
            }

            return false;
        }

        // Method 안에서 가능한 형태
        // 선언문 : int a = 1;  int a=1,b,c;  int[];  int [,];  int [][,][,,]
        // for 문 : for(int i = 0; i < 10; i++) { }
        // if  문 : if(c == d) ;
        // else문 : else { }    else;
        // 호출문 : function();
        // 대입문 : a = b;  a++; a = a+=1; b-=1; b--; c*=3; c/=5;
        // goto문 : goto abc;
        // label문: label:
        // 

        bool SplitOneBlock(EditScriptLibMain main, EditScriptLibFile file, string source, int col_pos, int row_pos)
        {
            // 단순히 {}로 쌓여있는 블럭이다.
            if (source.Length >= 2 && source[0] == '{' && source[source.Length - 1] == '}')
            {
                // 이미 블럭이기 때문에 또 블럭으로 쌓을 필요는 없다.
                main.CalcCursorPos(source[0], ref col_pos, ref row_pos);
                string buf = source.Substring(1, source.Length - 2);

                return Split(main, file, buf, col_pos, row_pos, false);
            }

            int word_start_pos = -1;
            int word_col_pos = 0;
            int word_row_pos = 0;
            char ch;

            SplitedString first_word = null;
            SplitedString second_word = null;
            SplitedString array_word = null;       // [] 와 같은 문자

            List<SplitedString> array_blocks = new List<SplitedString>();

            if (!SplitBlockByCommaAndColon(main, file, source, ref col_pos, ref row_pos, array_blocks)) return false;

            if (array_blocks.Count > 1)
            {
                System.Diagnostics.Debug.WriteLine("array_block_count > 1");
            }

            string sBody;
            SplitedString pre_datatype = null;
            bool bConst = false;

            for (int l = 0; l < array_blocks.Count; l++)
            {
                sBody = array_blocks[l].block;
                col_pos = array_blocks[l].col_pos;
                row_pos = array_blocks[l].row_pos;

                word_start_pos = -1;
                first_word = null;
                second_word = null;
                array_word = null;

                for (int i = 0; i < sBody.Length; i++)
                {
                    ch = sBody[i];

                    if (ch == '[')
                    {
                        if (!GetWord(main, file, sBody, ref i, ref word_start_pos, ref first_word, ref second_word, word_col_pos, word_row_pos)) return false;

                        string buf;

                        if (!main.SkipSqureBracket(file, sBody, ref i, ref col_pos, ref row_pos, out buf)) return false;

                        if (array_word == null)
                        {
                            array_word = new SplitedString();
                            array_word.block = buf;
                            array_word.col_pos = col_pos;
                            array_word.row_pos = row_pos;
                        }
                        else
                        {
                            array_word.block += buf;
                        }
                    }
                    else if (ch == '=' || ch == '(' || ch == '{' || ch == ':' ||
                        ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '%' ||
                        ch == '^' || ch == '&' || ch == '|' ||
                        ch == ';' || ch == ',')
                    {
                        if (!GetWord(main, file, sBody, ref i, ref word_start_pos, ref first_word, ref second_word, word_col_pos, word_row_pos)) return false;

                        if (ch == '=')  // 대입문이거나 선언문인 경우이다.
                        {
                            if (second_word != null)        // 선언문 int a = 30 과 같이 연속해서 두개의 단어가 있다.
                            {
                                if (pre_datatype != null)
                                {
                                    main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, ", 앞에서 이미 데이터 형이 선언되었습니다..");
                                    return false;
                                }

                                if (!AddDeclaration(main, file, first_word, array_word, second_word, sBody, ref i, col_pos, row_pos, bConst)) return false;
                                pre_datatype = first_word;  // int a, b; 와 같이 먼저 선언된 데이터 형을 기억한다.
                            }
                            else if (first_word != null)    // 단어가 하나밖에 없으면 대입문이거나 두번째로 나오는 선언문이다.
                            {
                                if (pre_datatype != null)
                                {
                                    if (!AddDeclaration(main, file, pre_datatype, array_word, first_word, sBody, ref i, col_pos, row_pos, bConst)) return false;
                                }
                                else
                                {
                                    if (!AddSubstitution(main, file, first_word, array_word, sBody, ref i, col_pos, row_pos)) return false;
                                }
                            }
                            else
                            {
                                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "= 앞에 대입할 변수가 있어야 합니다.");
                                return false;
                            }

                            goto next_block;  // 다음 문장 실행
                        }
                        else if (ch == '(') // for/if/while/switch/method 문인 경우이다.
                        {
                            if (second_word != null)    // ( 가 있는 경우는 앞에 단어가 1개만 있어야 한다.
                            {
                                if (first_word.block == "else" && second_word.block == "if")  // ( 로 시작하고 두 단어인경우는 else if밖에 없다.
                                {
                                    if (!AddIf(main, file, sBody, i, col_pos, row_pos, true)) return false;
                                    return true;
                                }
                                else
                                {
                                    main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "( 앞에 연속해서 2개의 단어가 있습니다.");
                                    return false;
                                }
                            }
                            if (first_word == null)
                            {
                                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "( 앞에 단어가 없습니다.");
                                return false;
                            }

                            if (first_word.block == "for")   // for 문
                            {
                                if (!AddFor(main, file, sBody, col_pos, row_pos)) return false;
                            }
                            else if (first_word.block == "if")   // if문
                            {
                                if (!AddIf(main, file, sBody, i, col_pos, row_pos, false)) return false;
                            }
                            else if (first_word.block == "elseif")   // 오토베이스 10까지 사용한 elseif문
                            {
                                if (!AddIf(main, file, sBody, i, col_pos, row_pos, true)) return false;
                            }
                            else if (first_word.block == "while")   // while
                            {
                                if (!AddWhile(main, file, sBody, i, col_pos, row_pos)) return false;
                            }
                            else if (first_word.block == "switch" || first_word.block == "foreach")
                            {
                                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "{0} 문은 아직 지원하지 않습니다.", first_word.block);
                                return false;
                            }
                            else // method
                            {
                                if (!AddMethod(main, file, first_word.block, sBody, ref col_pos, ref row_pos)) return false;
                            }

                            goto next_block;  // 다음 문장 실행
                        }
                        else if (ch == '{') // else
                        {
                            if (first_word.block == "else")
                            {
                                if (!AddElse(main, file, sBody, ref i, ref col_pos, ref row_pos)) return false;
                            }
                        }
                        else if (ch == ':') // label문
                        {
                            if (!AddLabel(main, file, sBody, first_word.block, second_word, i, col_pos, row_pos)) return false;

                            // label 문 다음에도 문장은 있다. 계속해석해야 한다.
                            //sBody = sBody.Substring(i + 1);
                            //i = -1;

                            first_word = null;
                            second_word = null;

                            continue;
                        }
                        else if (ch == '+' || ch == '-') // ++ += -- -=
                        {
                            if (i < sBody.Length - 1 && sBody[i + 1] == ch) // ++ 이거나 -- 일때
                            {
                                if (!AddCommandVoidValue(main, file, array_blocks[l])) return false;
                            }
                            else if (i < sBody.Length - 1 && sBody[i + 1] == '=')
                            {
                                i++;
                                col_pos++;
                                if (!AddSubstitutionPlusEqual(main, file, first_word, ch, sBody, ref i, col_pos, row_pos)) return false;
                            }
                        }
                        else if (ch == '*' || ch == '/' || ch == '%' || ch == '^' || ch == '&' || ch == '|') // *= /= %=
                        {
                            if (i < sBody.Length - 1 && sBody[i + 1] == '=')
                            {
                                i++;
                                col_pos++;
                                if (!AddSubstitutionPlusEqual(main, file, first_word, ch, sBody, ref i, col_pos, row_pos)) return false;
                            }
                        }
                        else if (ch == ';' || ch == ',') // return ; 과 같은식으로 return형이 없는 경우
                        {
                            if (first_word.block == "return")
                            {
                                if (!AddReturn(main, file, sBody, second_word, i, col_pos, row_pos)) return false;
                            }
                            else if (first_word.block == "break")
                            {
                                if (!AddBreak(main, file, sBody, second_word, i, col_pos, row_pos)) return false;
                            }
                            else if (first_word.block == "continue")
                            {
                                if (!AddContinue(main, file, sBody, second_word, i, col_pos, row_pos)) return false;
                            }
                            else if (first_word.block == "goto")
                            {
                                if (!AddGoto(main, file, sBody, second_word, i, col_pos, row_pos)) return false;
                            }
                            else if (second_word != null)
                            {
                                if (!AddDeclaration(main, file, first_word, array_word, second_word, sBody, ref i, col_pos, row_pos, bConst)) return false;
                                pre_datatype = first_word;
                            }
                            else
                            {
                                if (pre_datatype != null)
                                {
                                    if (!AddDeclaration(main, file, pre_datatype, array_word, first_word, sBody, ref i, col_pos, row_pos, bConst)) return false;
                                }
                                else
                                {
                                    main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "'{0}' is unknown command", sBody);
                                }
                            }
                        }

                        goto next_block;  // 다음 문장 실행

                    }
                    else if (word_start_pos != -1)
                    {
                        if (ch == ' ' || ch == '\t' || ch == '\n')
                        {
                            if (first_word == null)
                            {
                                first_word = new SplitedString();
                                first_word.block = sBody.Substring(word_start_pos, i - word_start_pos);
                                first_word.col_pos = word_col_pos;
                                first_word.row_pos = word_row_pos;

                                if (first_word.block == "return")
                                {
                                    if (!AddReturn(main, file, sBody, second_word, i, col_pos, row_pos)) return false;
                                    return true;
                                }
                                else if (first_word.block == "else")
                                {
                                    // else return 100; 과 같은 문장은 세개의 단어 연속이므로 
                                    // else if() 문이 아니면 다음 문장부터 블럭으로 계산한다.
                                    if (!CheckNextIf(sBody, i+1))    // 다음 문장이 if 인가를 검사한다.
                                    {
                                        if (!AddElse(main, file, sBody, ref i, ref col_pos, ref row_pos)) return false;
                                        return true;
                                    }
                                    
                                    // 다음문장이 if 문장이면 계속해서 해석하면 된다.
                                }
                                else if (first_word.block == "const")
                                {
                                    if (bConst)
                                    {
                                        main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, word_col_pos, word_row_pos, EnumScriptErrorType.Else, "'const' already defined.");
                                        return false;
                                    }
                                    bConst = true;
                                    first_word = null;
                                }
                            }
                            else if (second_word == null)
                            {
                                second_word = new SplitedString();
                                second_word.block = sBody.Substring(word_start_pos, i - word_start_pos);
                                second_word.col_pos = word_col_pos;
                                second_word.row_pos = word_row_pos;
                            }
                            else
                            {
                                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "단어가 연속해서 3개가 나열될 수 없습니다. FirstWord={0},SecondWord={1}", first_word.block, second_word.block);
                                return false;
                            }

                            word_start_pos = -1;
                        }
                    }
                    else // word가 시작되지 않았을 때
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
                            //if (!main.SkipDescription(sBody, ref i, ref col_pos, ref row_pos)) return false;
                            bool? retn_description = main.CheckDescription(file, sBody, ref i, ref col_pos, ref row_pos);
                            if (retn_description == null) return false;  // 오류가 발생했다.
                            if (retn_description == true) continue;
                        }
                        else
                        {
                            word_start_pos = i;
                            word_col_pos = col_pos;
                            word_row_pos = row_pos;
                        }
                    }

                    main.CalcCursorPos(ch, ref col_pos, ref row_pos);
                }
            next_block: ;
            }

            return true;
        }

        // for loop 속의 ; 사이에 있는 문장이나 if() 속의 조건문 Method(a, b, c) 속의 argument 등을 찿을 때 사용한다.
        // 0는 찾을 수 없는 경우 1은 문자를 찾은 경우 2는 )를 찾은 경우
        public static int GetSentenceToCharOrRightParenthesis(EditScriptLibMain main, EditScriptLibFile file, string body, ref int i, int string_end_pos, ref int col_pos, ref int row_pos, out int block_start_pos, out int block_end_pos, char end_char)
        {
            block_start_pos = 0;
            block_end_pos = 0;

            char ch;
            int count_parenthesis = 0;      // ()

            block_start_pos = i;

            for (; i <= string_end_pos; i++)
            {
                ch = body[i];
                
                if (ch == end_char && count_parenthesis == 0)
                {
                    block_end_pos = i;
                    main.CalcCursorPos(ch, ref col_pos, ref row_pos);
                    i++;
                    return 1;
                }
                else if (ch == '"')
                {
                    if (!main.SkipQuotationMark(file, body, ref i, string_end_pos, ref col_pos, ref row_pos)) return 0;
                }
                else if (ch == '\'')
                {
                    if (!main.SkipApostrophe(file, body, ref i, string_end_pos, ref col_pos, ref row_pos)) return 0;
                }
                else if (ch == '(')
                {
                    count_parenthesis++;
                }
                else if (ch == ')')
                {
                    if (count_parenthesis == 0)
                    {
                        block_end_pos = i;
                        main.CalcCursorPos(ch, ref col_pos, ref row_pos);
                        i++;
                        return 2;
                    }
                    count_parenthesis--;
                }
                else if (ch == '/')
                {
                    bool? retn_description = main.CheckDescription(file, body, ref i, ref col_pos, ref row_pos);
                    if (retn_description == null) return 0;  // 오류가 발생했다.
                    if (retn_description == true) continue;
                }
                else
                {

                }

                main.CalcCursorPos(ch, ref col_pos, ref row_pos);
            }

            main.SetError(file.sProjectName, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "{0}나 ) 문자를 찾을 수 없습니다.", end_char);
            return 0;
        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            SaveToStreamUserCommand(parent_writer, EnumBlockType.CommandBlock, tab_depth);
        }

        public void SaveToStreamUserCommand(ScriptWriter parent_writer, EnumBlockType type, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();
            SavePublic(writer, tab_depth + 1);
            for (int i = 0; i < arrayCommand.Count; i++)
            {
                arrayCommand[i].SaveToStream(writer, tab_depth + 1);
            }
            parent_writer.WriteBlock(type, writer);
        }

        /// <summary>
        /// Split시 삽입하기 전에 존재하는 가를 검사한다.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="var"></param>
        /// <returns></returns>
        public override bool IsExistVariableOnSplit(string varname)
        {
            for (int i = 0; i < arrayCommand.Count; i++)
            {
                if (arrayCommand[i].eCommandType == EnumCommandType.type_declaration)
                {
                    CommandDeclaration cd = (CommandDeclaration)arrayCommand[i];
                    if (cd.pVar.sVarName == varname)
                    {
                        return true;
                    }
                }
            }

            if (parentBlock != null)
            {
                return parentBlock.IsExistVariableOnSplit(varname);
            }
            else
            {
                return ((EditScriptLibMethod)parentMethod).IsExistVariableOnSplit(varname);
            }
        }

        /// <summary>
        /// Compile 시 존재하는 가를 검사한다.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="var"></param>
        /// <returns></returns>
        public override bool IsExistVariableOnCompile(string varname, out Variable var, out object finded_pos)
        {
            for (int i = 0; i < arrayCommand.Count; i++)
            {
                if (arrayCommand[i].eCommandType == EnumCommandType.type_declaration)
                {
                    CommandDeclaration cd = (CommandDeclaration)arrayCommand[i];
                    if (cd.pVar.sVarName == varname)
                    {
                        finded_pos = this;
                        var = cd.pVar;
                        return true;
                    }
                }
            }

            if (parentBlock != null)
            {
                return parentBlock.IsExistVariableOnCompile(varname, out var, out finded_pos);
            }
            else
            {
                return ((EditScriptLibMethod)parentMethod).IsExistVariableOnCompile(varname, out var, out finded_pos);
            }
        }
        
    }
}
