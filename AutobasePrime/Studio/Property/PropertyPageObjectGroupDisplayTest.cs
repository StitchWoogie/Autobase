using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using System.Drawing.Imaging;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectGroupDisplayTest.
	/// </summary>
	public class PropertyPageObjectGroupDisplayTest : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonTest;
		private System.Windows.Forms.ListView listViewResult;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectGroupDisplayTest()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectGroupDisplayTest));
            this.buttonTest = new System.Windows.Forms.Button();
            this.listViewResult = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonTest
            // 
            this.buttonTest.AccessibleDescription = null;
            this.buttonTest.AccessibleName = null;
            resources.ApplyResources(this.buttonTest, "buttonTest");
            this.buttonTest.BackgroundImage = null;
            this.buttonTest.Font = null;
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // listViewResult
            // 
            this.listViewResult.AccessibleDescription = null;
            this.listViewResult.AccessibleName = null;
            resources.ApplyResources(this.listViewResult, "listViewResult");
            this.listViewResult.BackgroundImage = null;
            this.listViewResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewResult.Font = null;
            this.listViewResult.Name = "listViewResult";
            this.listViewResult.UseCompatibleStateImageBehavior = false;
            this.listViewResult.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // PropertyPageObjectGroupDisplayTest
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewResult);
            this.Controls.Add(this.buttonTest);
            this.Icon = null;
            this.Name = "PropertyPageObjectGroupDisplayTest";
            this.ResumeLayout(false);

		}
		#endregion

		private void label1_Click(object sender, System.EventArgs e)
		{
		
		}

		ObjectGroup objGroup;

		public void SetGroup(ObjectGroup group)
		{
			objGroup = group;
		}

		bool AddResult(string title, int ticks, int loop)
		{
			if(ticks == 0)	return false;
			ListViewItem lvi = new ListViewItem(title);
			double result = ((double)ticks/loop)/1000.0;
			lvi.SubItems.Add(result.ToString());
			this.listViewResult.Items.Add(lvi);
			return true;
		}

		private void buttonTest_Click(object sender, System.EventArgs e)
		{
			ObjectGroup obj = (ObjectGroup)Tools.CopyObject(objGroup);

			int width = Screen.PrimaryScreen.Bounds.Width;
			int height = Screen.PrimaryScreen.Bounds.Height;

			Bitmap bitmapscreen = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			Graphics gscreen = Graphics.FromImage(bitmapscreen);

			Rectangle rect = new Rectangle(0, 0, width, height);

			int i;
			int loop = 1;
			int ticks;
			
		retry:
			loop *= 2;
			this.listViewResult.Items.Clear();

			ticks = Environment.TickCount;
			for(i = 0; i < loop; i++) 
			{
				obj.Display(gscreen, rect, 0, 0);
			}
			ticks = Environment.TickCount-ticks;
			if(!AddResult("100%", ticks, loop))	goto retry;

			int x1=0, y1=0, x2=0, y2=0;
			obj.GetZone(ref x1, ref y1, ref x2, ref y2);
			width = x2-x1+1;
			height = y2-y1+1;
            obj.UpdateZone(ClassEditProperty.formEditor, 0, 0, width - 1, height - 1);
			Bitmap bitmapf = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics gf = Graphics.FromImage(bitmapf);
			rect = new Rectangle(0, 0, width, height);
			
			obj.Display(gf, rect, 0, 0);

			ticks = Environment.TickCount;
			for(i = 0; i < loop; i++) 
			{
				gscreen.DrawImageUnscaled(bitmapf, 0, 0);
			}
			ticks = Environment.TickCount-ticks;

			if(!AddResult("100% Bitmap", ticks, loop))	goto retry;

			ticks = Environment.TickCount;
			ImageAttributes imageAttr = new ImageAttributes();
			Color overlay_color = bitmapf.GetPixel(0, 0);
			for(i = 0; i < loop; i++) 
			{
				gscreen.DrawImage(bitmapf, rect, 0, 0, width, height, GraphicsUnit.Pixel, imageAttr);
			}
			ticks = Environment.TickCount-ticks;
			if(!AddResult("100% Bitmap Overlay", ticks, loop))	goto retry;
		}
	}
}
