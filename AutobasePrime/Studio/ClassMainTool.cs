using System;
using System.Windows.Forms;
using System.Drawing;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassMainTool.
	/// </summary>
	public class ClassMainTool
	{
		
		public ClassMainTool()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public virtual void MouseDownLeft(FormEditGraphic form, MouseEventArgs e)
		{

		}

		public virtual void MouseDownRight(FormEditGraphic form, MouseEventArgs e)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			form.contextMenuStripMove.Show(form, new Point(e.X, e.Y));
		}

		public virtual void MouseMove(FormEditGraphic form, MouseEventArgs e)
		{
			
		}

		public virtual void MouseUpLeft(FormEditGraphic form, MouseEventArgs e)
		{

		}

		public virtual void MouseUpRight(FormEditGraphic form, MouseEventArgs e)
		{

		}

		public virtual void Paint(FormEditGraphic form, Graphics g)
		{

		}

		public virtual void DoubleClick(FormEditGraphic form)
		{

		}

		public virtual void OnBeforeMainToolChanged()
		{

		}
	}
}
