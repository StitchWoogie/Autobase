using System;
using System.Drawing;
using System.Windows.Forms;
using NetTools.OldDefine;
using GraphicModule;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditObjectCircle.
	/// </summary>
	public class ClassEditObjectText : ClassMainTool
	{
		public ClassEditObjectText()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static Point pMouseStart = new Point();

		public TextBox textBox = null;
		FormEditGraphic formParent;

		public override void MouseDownLeft(FormEditGraphic form, MouseEventArgs e)
		{
			TextBoxRemove();
			
			form.ConvertMousePointByGuideLine(form.workThis, ref pMouseStart, e.X, e.Y);
			
			formParent = form;
			textBox = new TextBox();
			textBox.Left = pMouseStart.X;
			textBox.Top = pMouseStart.Y;
			form.Controls.Add(textBox);

			textBox.Select();

			textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Event_KeyDown);
            textBox.LostFocus += new EventHandler(textBox_LostFocus);

            form.SelectListClear(form.workThis);
            form.DisplayAfterSelectedChanged();
            form.Invalidate();	// 바로 밑에서 return이 되면 무효화가 안된다.
		}

        void textBox_LostFocus(object sender, EventArgs e)
        {
            // MakeObject();  속성상자에서 다운되는 경우가 있다.
        }

		void TextBoxRemove()
		{
			if(textBox != null) 
			{
				formParent.Controls.Remove(textBox);
				//textBox.Dispose();
				textBox = null;
			}
		}

        void MakeObject()
        {
            if (textBox.Text.Length == 0)
            {
                TextBoxRemove();
                return;
            }

            WORK_MODULE_STRUCT work = formParent.workThis;
            RECT rect = new RECT();

            rect.left = formParent.GetPicturePosX(pMouseStart.X);
            rect.top = formParent.GetPicturePosY(pMouseStart.Y);
            rect.right = rect.left + 200;
            rect.bottom = rect.top + 50;

            ObjectArgsSingleText args = new ObjectArgsSingleText();
            args.text = textBox.Text;
            args.textColor = Color.Black;

            ObjectSingleText obj = new ObjectSingleText(work.obj.objCommonProperty, formParent, rect, null, new ObjectGeneral("SingleText1"), ClassEditInsert.GetNewFont(), args);

            Graphics g = formParent.CreateGraphics();
            obj.RecalcRectSize(g);                      // 크기를 글자에 맞추어서 다시 계산한다.

            ClassEditInsert.InsertPublic(formParent, obj, "Insert Text");
            
            TextBoxRemove();
        }

		private void Event_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyData == Keys.Enter) 
			{
                e.SuppressKeyPress = true;  // 이것을 하지 않으면 엔터입력시 소리가 난다. 2010-7-7
                MakeObject();
			}
			else if(e.KeyData == Keys.Escape) 
			{
				TextBoxRemove();
				return;				
			}

		}

		public override void MouseMove(FormEditGraphic form, MouseEventArgs e)
		{
			form.Cursor = Cursors.IBeam;
		}

		public override void MouseUpLeft(FormEditGraphic form, MouseEventArgs e)
		{
		}

		public override void Paint(FormEditGraphic form, Graphics g)
		{
		}

		public override void OnBeforeMainToolChanged()
		{
			TextBoxRemove();			
		}
	}
}
