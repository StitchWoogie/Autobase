using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ScriptLibEdit.Debugger;
using ScriptLibRun.Debugger;

namespace ScriptLibEdit.Editor
{
    // Debug에 관련된 부분만 모아 놓았다.
    partial class TextArea : Control
    {
        void SaveBreakPoint(string sourcefile)
        {
            string filename = Path.ChangeExtension(sourcefile, "BreakPoints");

            if (arrayBreakPoints.Count <= 0)    // 파일이 너무 난잡하므로 BreakPoint가 없을 때는 삭제하는 것이 좋다.
            {
                if(File.Exists(filename))
                    File.Delete(filename);
            }
            else
            {
                TextWriter writer = new StreamWriter(filename);

                for (int i = 0; i < arrayBreakPoints.Count; i++)
                {
                    writer.WriteLine("{0}", arrayBreakPoints[i]);
                }

                writer.Close();
            }
        }

        bool bBreaking = false;
        int nBreakPos;

        public void GotoBreakPoint(int x, int y)
        {
            GotoViewCursor(x, y);
            bBreaking = true;
            nBreakPos = y;
            Invalidate();
        }

        static bool bDebugStarted = false;  // 디버그 모드는 전체 소스에서 사용하므로 static으로 잡았다.

        public bool IsDebugStarted()
        {
            return bDebugStarted;
        }

        public static void DebugStart()
        {
            bDebugStarted = true;
        }

        public void DebugStop()
        {
            bDebugStarted = false;

            if (bBreaking)
            {
                bBreaking = false;
                Invalidate();
            }
        }

        public void DebugStepInto()
        {
            if (bBreaking)
            {
                bBreaking = false;
                Invalidate();
                DebuggerEditMain.SendNextCommand(EnumDebugStep.StepInto);
            }
        }

        public void DebugStepOver()
        {
            if (bBreaking)
            {
                bBreaking = false;
                Invalidate();
                DebuggerEditMain.SendNextCommand(EnumDebugStep.StepOver);
            }
        }

        public void DebugStepContinue()
        {
            if (bBreaking)
            {
                bBreaking = false;
                Invalidate();
                DebuggerEditMain.SendNextCommand(EnumDebugStep.StepContinue);
            }
        }

        List<int> arrayBreakPoints = new List<int>();

        void SetDebugBreakPoint(int y)
        {
            //bool toggle = false;

            for (int i = 0; i < arrayBreakPoints.Count; i++)
            {
                if (arrayBreakPoints[i] == y)
                {
                    arrayBreakPoints.RemoveAt(i);
                    Invalidate();
                    //toggle = false;
                    goto ok;
                }
            }

            arrayBreakPoints.Add(y);
            Invalidate();
        //toggle = true;

            ok:
            if (bDebugStarted)
            {
                DebuggerEditMain.SetBreakPoint(sFilename, y);
            }
        }

        public void SetDebugBreakPointByCursor()
        {
            SetDebugBreakPoint(nCursorY);
        }
    }
}
