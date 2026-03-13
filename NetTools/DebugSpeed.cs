using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetTools
{
	/// <summary>
	/// Summary description for FormMessageDisplay.
	/// </summary>
	class FormDebugSpeed : System.Windows.Forms.Form
	{
        private System.ComponentModel.IContainer components=null;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;

        public FormDebugSpeed()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(516, 271);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Title";
            this.columnHeader1.Width = 215;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Elapsed";
            this.columnHeader2.Width = 135;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Average";
            this.columnHeader3.Width = 132;
            // 
            // DebugSpeed
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(516, 271);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DebugSpeed";
            this.ShowInTaskbar = false;
            this.Text = "Speed Check";
            this.Load += new System.EventHandler(this.FormMessageDisplay_Load);
            this.Closed += new System.EventHandler(this.FormMessageDisplay_Closed);
            this.ResumeLayout(false);

		}
		#endregion

        void FillList()
        {

        }

		private void FormMessageDisplay_Load(object sender, System.EventArgs e)
		{
            formThis = this;

            ReLoadList();
		}

		private void FormMessageDisplay_Closed(object sender, System.EventArgs e)
		{
            formThis = null;
		}

        void ReLoadList()
        {
            this.listView1.Items.Clear();

            ListViewItem lvi;
            for (int i = 0; i < arrayItem.Count; i++)
            {
                lvi = new ListViewItem(arrayItem[i].title);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                this.listView1.Items.Add(lvi);
            }
        }

        void CalcItem(SpeedItem item, ListViewItem lvi, TimeSpan t)
        {
            if (t == item.t) return;

            item.t = t;

            if (item.count >= 100)
            {
                item.sum -= (item.sum / item.count);
                item.sum += t.Ticks;
            }
            else
            {
                item.sum += t.Ticks;
                item.count++;
            }

            lvi.SubItems[1].Text = t.ToString();
            lvi.SubItems[2].Text = TimeSpan.FromTicks(item.sum / item.count).ToString();

        }

        public void CheckSpeed(string title, TimeSpan t)
        {
            for (int i = 0; i < arrayItem.Count; i++)
            {
                if (arrayItem[i].title == title)
                {
                    CalcItem(arrayItem[i], this.listView1.Items[i], t);
                    return;
                }
            }

            SpeedItem item = new SpeedItem();
            item.title = title;

            arrayItem.Add(item);

            ReLoadList();

            CalcItem(item, this.listView1.Items[this.listView1.Items.Count-1], t);
        }

        static FormDebugSpeed formThis = null;
        static List<SpeedItem> arrayItem = new List<SpeedItem>();

        public static void Show(string title, TimeSpan t)
        {
            if (formThis == null)
            {
                FormDebugSpeed form = new FormDebugSpeed();
                //form.TopMost = true;
                form.Owner = Application.OpenForms[0];
                form.Show();

                form.CheckSpeed(title, t);
            }
            else
            {
                formThis.CheckSpeed(title, t);
            }
        }
	}

    public class DebugSpeed
    {
        Stopwatch stopwatch = new Stopwatch();

        public void Start()
        {
            if (!DeveloperVersion.bDebugSpeed) return;

            stopwatch.Start();
        }

        public void Stop(string title)
        {
            if (!DeveloperVersion.bDebugSpeed) return;

            stopwatch.Stop();

            FormDebugSpeed.Show(title, stopwatch.Elapsed);
        }
    }
    
    class SpeedItem
    {
        public string title;
        public TimeSpan t;
        public int count;
        public long sum;
    }
	
}
