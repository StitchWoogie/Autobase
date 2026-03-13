using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditCommandFor : CommandFor
    {
        public EditCommandFor(EditScriptLibMethod parent, CommandPublic parent_block)
            : base(parent, parent_block)
        {
        }

        public bool Split(EditScriptLibMain main, EditScriptLibFile file, string sBody, int col_pos, int row_pos)
        {
            SetColRow(col_pos, row_pos);

            int i = sBody.IndexOf('(');

            int start_col_pos, start_row_pos;
            int block_start_pos, block_end_pos;
            int retn;

            i++;
            col_pos++;


            // 초기화 문장
            start_col_pos = col_pos;
            start_row_pos = row_pos;
            retn = EditCommandBlock.GetSentenceToCharOrRightParenthesis(main, file, sBody, ref i, sBody.Length - 1, ref col_pos, ref row_pos, out block_start_pos, out block_end_pos, ';');
           
            if (retn != 1) // ;을 찾을 수 없거나 ) 로 끝난 경우
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "for문에서 ; 로 끝나는 초기화 문장이 없습니다.");
                return false;
            }

            string block = sBody.Substring(block_start_pos, block_end_pos - block_start_pos+1); // ; 콜론까지 포함한다.
            pCommandBefore = new EditCommandBlock((EditScriptLibMethod)parentMethod, this);
            if (!((EditCommandBlock)pCommandBefore).Split(main, file, block, start_col_pos, start_row_pos, false)) return false;

            // 조건문
            start_col_pos = col_pos;
            start_row_pos = row_pos;
            retn = EditCommandBlock.GetSentenceToCharOrRightParenthesis(main, file, sBody, ref i, sBody.Length - 1, ref col_pos, ref row_pos, out block_start_pos, out block_end_pos, ';');

            if (retn != 1) // ;을 찾을 수 없거나 ) 로 끝난 경우
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "for문에서 ; 로 끝나는 조건문 문장이 없습니다.");
                return false;
            }

            block = sBody.Substring(block_start_pos, block_end_pos - block_start_pos);
            pCondition = new EditRecursiveCondition((EditScriptLibMethod)parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
            if (!((EditRecursiveCondition)pCondition).Split(main, file, block, start_col_pos, start_row_pos)) return false;

            // 후 실행문
            start_col_pos = col_pos;
            start_row_pos = row_pos;
            retn = EditCommandBlock.GetSentenceToCharOrRightParenthesis(main, file, sBody, ref i, sBody.Length - 1, ref col_pos, ref row_pos, out block_start_pos, out block_end_pos, ')');

            if (retn == 0) // ) 를 찾을 수 없는 경우
            {
                main.SetError(parentMethod.parentClass.parentNamespace.parentLibrary.sNameLibrary, file.sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "for문에서 ) 로 끝나야 합니다.");
                return false;
            }

            // 블럭 실행문
            block = sBody.Substring(block_start_pos, block_end_pos - block_start_pos) + ";";    // ) 로 끝났으므로 세미콜론을 붙여준다.
            pCommandAfter = new EditCommandBlock((EditScriptLibMethod)parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
            if (!((EditCommandBlock)pCommandAfter).Split(main, file, block, start_col_pos, start_row_pos, false)) return false;

            // ) 다음에 있는 모든 문장이 while문일 때 실행할 문장이다.
            string blocks = sBody.Substring(block_end_pos + 1);

            blockCommand = new EditCommandBlock((EditScriptLibMethod)parentMethod, pCommandBefore); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
            if (!((EditCommandBlock)blockCommand).Split(main, file, blocks, col_pos + 1, row_pos, false)) return false;

            return true;
        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            SavePublic(writer, tab_depth + 1);

            ((EditCommandBlock)pCommandBefore).SaveToStreamUserCommand(writer, EnumBlockType.CommandBlock2, tab_depth + 1); // CommandBefore 문은 다른 문장의 Parent로 사용되기 때문에 반드시 맨처음 저장해야 한다.

            ((EditRecursiveCondition)pCondition).SaveToStream(writer, tab_depth + 1);
            ((EditCommandBlock)pCommandAfter).SaveToStreamUserCommand(writer, EnumBlockType.CommandBlock3, tab_depth + 1);

            blockCommand.SaveToStream(writer, tab_depth + 1);

            parent_writer.WriteBlock(EnumBlockType.CommandFor, writer);
        }

        public override void Compile(ScriptLibMain main)
        {
            pCommandBefore.Compile(main);
            ((EditRecursiveCondition)pCondition).Compile(main);
            pCommandAfter.Compile(main);
            blockCommand.Compile(main);
        }

    }
}
