using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using AutoLibLocal;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectMilliData.
	/// </summary>
	public class PropertyPageObjectMilliData : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox m_combo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonSetupMilliData;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectMilliData()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectMilliData));
            this.m_combo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSetupMilliData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_combo
            // 
            this.m_combo.AccessibleDescription = null;
            this.m_combo.AccessibleName = null;
            resources.ApplyResources(this.m_combo, "m_combo");
            this.m_combo.BackgroundImage = null;
            this.m_combo.Font = null;
            this.m_combo.Name = "m_combo";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // buttonSetupMilliData
            // 
            this.buttonSetupMilliData.AccessibleDescription = null;
            this.buttonSetupMilliData.AccessibleName = null;
            resources.ApplyResources(this.buttonSetupMilliData, "buttonSetupMilliData");
            this.buttonSetupMilliData.BackgroundImage = null;
            this.buttonSetupMilliData.Font = null;
            this.buttonSetupMilliData.Name = "buttonSetupMilliData";
            this.buttonSetupMilliData.Click += new System.EventHandler(this.buttonSetupMilliData_Click);
            // 
            // PropertyPageObjectMilliData
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.buttonSetupMilliData);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_combo);
            this.Icon = null;
            this.Name = "PropertyPageObjectMilliData";
            this.Load += new System.EventHandler(this.PropertyPageObjectMilliData_Load);
            this.ResumeLayout(false);

		}
		#endregion

		void FillCombo()
		{
			ArrayList block = new ArrayList();
			MilliData.LoadMilliData(block);
			MILLI_DATA_STRUCT item;

			this.m_combo.Items.Clear();

			for(int i = 0; i < block.Count; i++) 
			{
				item = (MILLI_DATA_STRUCT)block[i];
				this.m_combo.Items.Add(item.title);
			}
		}

		private void PropertyPageObjectMilliData_Load(object sender, System.EventArgs e)
		{
			FillCombo();
		}

		private void buttonSetupMilliData_Click(object sender, System.EventArgs e)
		{
			if(FormConfigMilliData.ConfigMilliData() == true) 
			{
				FillCombo();
			}
		}

		public ObjectArgsMilliData ObjectArgs 
		{
			set 
			{
				this.m_combo.Text = value.sTitle;
			}
			get 
			{
				ObjectArgsMilliData args = new ObjectArgsMilliData();

				args.sTitle = this.m_combo.Text;

				return args;
			}
		}
	}
}
