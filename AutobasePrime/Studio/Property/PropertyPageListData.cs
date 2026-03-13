using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageListData.
	/// </summary>
	public class PropertyPageListData : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxItem;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageListData()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageListData));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxItem = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // textBoxItem
            // 
            this.textBoxItem.AcceptsReturn = true;
            this.textBoxItem.AccessibleDescription = null;
            this.textBoxItem.AccessibleName = null;
            resources.ApplyResources(this.textBoxItem, "textBoxItem");
            this.textBoxItem.BackgroundImage = null;
            this.textBoxItem.Font = null;
            this.textBoxItem.Name = "textBoxItem";
            // 
            // PropertyPageListData
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.textBoxItem);
            this.Controls.Add(this.label1);
            this.Icon = null;
            this.Name = "PropertyPageListData";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public ArrayList ListData 
		{
			set 
			{
				if(value == null) 
				{
					this.textBoxItem.Text = "";
					return;
				}
				string text = "";
				string item;

				for(int i = 0; i < value.Count; i++) 
				{
					item = (string)value[i];
					text += item;
					if(i != value.Count-1) text += "\r\n";
				}

				this.textBoxItem.Text = text;
			}
			get 
			{
				ArrayList array = new ArrayList();
				CommaBlockString comma = new CommaBlockString();
				comma.SetBlockCode('\r');
				comma.Set(this.textBoxItem.Text);

				string item = "";
				while(true) 
				{
					if(comma.IsEOS())	break;
					comma.GetString(ref item);
					if(item.Length > 0) 
					{
						array.Add(item);
					}
				}

				return array;
			}
		}
	}
}
