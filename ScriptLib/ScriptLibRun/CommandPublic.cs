using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using ScriptLibRun.Debugger;

namespace ScriptLibRun
{
    public class CommandPublic : ColumnRowInfo
    {
        public EnumCommandType eCommandType;
        protected ScriptLibMemberMethod parentMethod;
        protected CommandPublic parentBlock;

        public bool bBreakPoint = false;        // 이 명령에 BreakPoint가 걸려있다.
        //public bool bBreakPointImsi = false;     // StepInto나 StopOver인 경우 잠깐동안 걸리는 BreakPoint
        public static bool bBreakMustNext = false;

        public CommandPublic(ScriptLibMemberMethod parent, CommandPublic parent_block)
        {
            parentMethod = parent;
            parentBlock = parent_block;
        }

        // 선언은 되어 있지만 Edit 모드에서만 사용하는 함수
        public virtual void SaveToStream(ScriptWriter writer, int tab_depth)
        {

        }

        // 선언은 되어 있지만 Edit 모드에서만 사용하는 함수
        public virtual void Compile(ScriptLibMain main)
        {

        }

        // Edit 모드에서만 사용
        public virtual bool IsExistVariableOnSplit(string varname)
        {
            if (parentBlock != null)
            {
                return parentBlock.IsExistVariableOnSplit(varname);
            }
            
            return false;
        }

        // Edit 모드에서만 사용
        public virtual bool IsExistVariableOnCompile(string varname, out Variable var, out object finded_pos)
        {
            if (parentBlock != null)
            {
                return parentBlock.IsExistVariableOnCompile(varname, out var, out finded_pos);
            }

            var = null;
            finded_pos = null;
            return false;
        }

        // Edit 모드에서만 사용
        public virtual bool IsExistLabel(string name)
        {
            if (parentBlock != null)
            {
                return parentBlock.IsExistLabel(name);
            }

            return false;
        }

        public EnumDebugStep DebuggerGotoCursor(ClassDataStack class_stack, ClassDataStack data_stack, int column, int row)
        {
            if (!ScriptLibMain.bDebugMode) return EnumDebugStep.StopNone;  // Debug 모드가 아니다.

            if (!bBreakPoint && !bBreakMustNext) return EnumDebugStep.StopNone;

            bBreakMustNext = false;    // 임시로 Break된 부분은 Trace 후에 제거한다.

            return DebuggerMain.Goto(parentMethod.sSourceFilename, column, row, class_stack, data_stack);
        }

        public virtual string MakeDecompiledFile(int depth, bool bAddSemicolon)
        {
            return "";
        }

        // , 로 구분되는 command
        // int a=1,b=2;
        public virtual string MakeDecompiledFileCommaBlock(int depth, bool bFirstComma)
        {
            return MakeDecompiledFile(depth, false);
        }
    }
}
