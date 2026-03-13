using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditCommandMethod : CommandMethod
    {
        public EditCommandMethod(EditScriptLibMethod parent, CommandPublic parent_block)
            : base(parent, parent_block)
        {
            args = new List<CommandMethodArg>();
        }

        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos)
        {
            SetColRow(col_pos, row_pos);

            int i = sBody.IndexOf('(');

            sMethodName = sBody.Substring(0, i).Trim();     // test1 (); 와 같이 ( 앞에 스페이스가 있는 경우를 제거한다.

            int start_col_pos, start_row_pos;
            int block_start_pos, block_end_pos;
            int retn;
            string buf;
            EditCommandMethodArg value;

            i++;
            col_pos++;

            while (true)
            {
                start_col_pos = col_pos;
                start_row_pos = row_pos;
                retn = EditCommandBlock.GetSentenceToCharOrRightParenthesis(main, file, sBody, ref i, sBody.Length - 1, ref col_pos, ref row_pos, out block_start_pos, out block_end_pos, ',');
                if (retn == 0) return false;
                buf = sBody.Substring(block_start_pos, block_end_pos - block_start_pos);

                if (buf.Length > 0)  // method() 인경우는 첫번째 인자가 없다.
                {
                    buf = ScriptLibTools.TrimWithCursorPos(buf, ref start_col_pos, ref start_row_pos);

                    value = new EditCommandMethodArg((EditScriptLibMethod)parentMethod, parentBlock);

                    if (String.Compare(buf, 0, "out ", 0, 4) == 0)
                    {
                        value.eInOut = EnumInOut.Out;
                        buf = buf.Substring(4);
                    }
                    else if (String.Compare(buf, 0, "ref ", 0, 4) == 0)
                    {
                        value.eInOut = EnumInOut.Ref;
                        buf = buf.Substring(4); 
                    }

                    ((EditRecursiveValue)value.value).Split(main, file, buf, start_col_pos, start_row_pos);
                    args.Add(value);
                }

                if (retn == 2) break;
            }

            return true;
        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();
            SavePublic(writer, tab_depth + 1);
            writer.WriteName(sMethodName);
            writer.WriteByte(EnumBlockType.MethodType, (byte)eMethod);
            writer.WriteNameSplit(sCompiledNamespace, sCompiledClass, sCompiledMethod);
            for (int i = 0; i < args.Count; i++)
            {
                ((EditCommandMethodArg)args[i]).SaveToStream(writer, tab_depth + 1);
            }
            parent_writer.WriteBlock(EnumBlockType.CommandMethod, writer);
        }

        public override void Compile(ScriptLibMain slmain)
        {
            for (int i = 0; i < args.Count; i++)
            {
                ((EditCommandMethodArg)args[i]).Compile(slmain);
            }

            // 외부 선언 함수인가를 검사한다.
            if (slmain.scriptExternal != null)
            {
                int retn = slmain.scriptExternal.IsExistMethod(sMethodName, args);
                if (retn == 1)
                {
                    eMethod = EnumMethodType.ExternalMethod;
                    return;
                }
                else if (retn == 2)
                {
                    slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, slmain.scriptExternal.ErrorType, slmain.scriptExternal.ErrorMessage);
                    eMethod = EnumMethodType.ExternalMethod;
                    return;
                }
            }

            // namespace, class, method 를 분리한다.
            ScriptLibTools.SplitNamespaceClassMethod(sMethodName, out sCompiledNamespace, out sCompiledClass, out sCompiledMethod);

            EditScriptLibClass eslc = (EditScriptLibClass)parentMethod.parentClass;

            if (sCompiledClass == null) // Method명 밖에 없다. 이것은 내부 클래스를 호출하는 경우이다.
            {
                if (eslc.GetMethodPointer(slmain, parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, sCompiledMethod, args) == null)
                {
                    slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The method '{0}' does not exist in the class '{1}'", sCompiledMethod, eslc.sNameClass);
                }
                eMethod = EnumMethodType.SelfClassMethod;
                return;
            }

            // namespace가 없고 class명이 있으면 할당된 클래스일 수 있다.   ex.Method() 형식일 경우
            if (sCompiledNamespace == null && sCompiledClass != null)
            {
                Variable var;
                object finded_pos;
                if (((EditCommandBlock)parentBlock).IsExistVariableOnCompile(sCompiledClass, out var, out finded_pos))
                {
                    for (int l = 0; l < slmain.arrayLibrary.Count; l++)
                    {
                        for (int i = 0; i < slmain.arrayLibrary[l].arrayNamespace.Count; i++)
                        {
                            ScriptLibNamespace sln = slmain.arrayLibrary[l].arrayNamespace[i];

                            if (sln.sNameNamespace != var.sCompiledNamespace) continue;

                            for (int j = 0; j < sln.arrayMember.Count; j++)
                            {
                                if (sln.arrayMember[j].eMember == EnumScriptLibMember.Class)
                                {
                                    ScriptLibClass slc = (ScriptLibClass)sln.arrayMember[j];

                                    if (slc.sNameClass == var.sCompiledClass)
                                    {
                                        ScriptLibMemberMethod slmm = slc.GetMethodPointer(slmain, parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, sCompiledMethod, args);

                                        if (slmm != null)
                                        {
                                            if (slmm.eAccessLevel != EnumAccessLevel.access_public)
                                            {
                                                slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The method '{0}' is inaccessible due to its protection level", sCompiledMethod);
                                            }
                                            eMethod = EnumMethodType.AllocMethod;
                                            return;
                                        }
                                        else
                                        {
                                            slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The method '{0}' does not exist in the class '{1}'", sCompiledMethod, sCompiledClass);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    slmain.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, parentMethod.sSourceFilename, nColumn, nRow, EnumScriptErrorType.Else, "The method '{0}' does not exist in the variable '{1}'", sCompiledMethod, var.sVarName);
                                        
                    return;
                }
            }

            eslc = (EditScriptLibClass)parentMethod.parentClass;
            ScriptLibMemberMethod slm;

            // 할당된 클래스의 메소드가 아니면 static형 메소드이다.
            if (((EditScriptLibMain)slmain).SeekStaticMethodWithError(eslc, eslc.sourceFile, ref sCompiledNamespace, ref sCompiledClass, sCompiledMethod, out slm, nColumn, nRow, args))
            {

            }

            return;
            
        }
    }
}
