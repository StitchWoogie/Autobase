using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools;
using System.Drawing;

namespace ScriptLibEdit.Editor
{
    public partial class TextArea : Control
    {
        void IntelliSenseGo()
        {
           
            if (FormIntelliSense.formThis != null)
            {
                return;
            }

            FormIntelliSense form = new FormIntelliSense();

            int vx = GetViewCursorX(CursorX, CursorY);

            int x = (vx - nPageX) * nCharX + nStartTextX;
            int y = (CursorY - nPageY) * nCharY;

            x -= 10;
            y += nCharY+2;

            form.Owner = EditScriptConfig.formMainEditor;

            // 아래 세줄을 한번 하고 위치를 변경해야 원하는 위치로 간다.
            form.StartPosition = FormStartPosition.Manual;
            form.Left = 100;	// 작은 숫자를 주면 정확한 윈도우의 크기가 안나온다.(원인:모름)
            form.Top = 100;
            //form.TopMost = true;
            
            Point p = new Point(x, y);
            p = PointToScreen(p);
            
            form.StartPosition = FormStartPosition.Manual;
            form.Left = p.X;
            form.Top = p.Y;

            form.Show();

            EditScriptConfig.formMainEditor.Select();
        }
    }
}
