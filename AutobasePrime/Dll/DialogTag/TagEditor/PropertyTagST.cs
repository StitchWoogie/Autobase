using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using DialogTag;
using NetTools;
using System.IO;

namespace DialogTag.TagEditor
{
	/// <summary>
	/// Summary description for FormTagProperty.
	/// </summary>
	public class PropertyTagST : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox33;
		private System.Windows.Forms.Label label68;
		private System.Windows.Forms.Label label69;
		private System.Windows.Forms.GroupBox groupBox35;
		private System.Windows.Forms.Label label70;
		private System.Windows.Forms.TextBox textBoxStAddress;
		private System.Windows.Forms.TextBox textBoxStPort;
		private MyNumericUpDown numericUpDownStReadSize;
		private System.Windows.Forms.RadioButton radioButtonStMemoryType0;
		private System.Windows.Forms.RadioButton radioButtonStMemoryType1;
		private System.Windows.Forms.GroupBox groupBox26;
		private System.Windows.Forms.RadioButton radioButtonStReadMethod1;
		private System.Windows.Forms.RadioButton radioButtonStReadMethod0;
		private System.Windows.Forms.RadioButton radioButtonStReadMethod2;
		private System.Windows.Forms.RadioButton radioButtonStReadMethod3;

		MultiSelectTextBox multiStPort = new MultiSelectTextBox();
		MultiSelectTextBox multiStAddress = new MultiSelectTextBox();
		MultiSelectRadioButton multiStMemoryType = new MultiSelectRadioButton();
		MultiSelectRadioButton multiStReadMethod = new MultiSelectRadioButton();
		MultiSelectNumericUpDown multiStDataSize = new MultiSelectNumericUpDown();
		MultiSelectCheckBox multiStUseAsBothInputOutput = new MultiSelectCheckBox();
        private RadioButton radioButtonStReadMethod4;

		private System.Windows.Forms.CheckBox checkBoxStUseAsBothInputOutput;

		public PropertyTagST()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// ST
			multiStPort.Add(this.textBoxStPort);
			multiStAddress.Add(this.textBoxStAddress);
			multiStMemoryType.Add(this.radioButtonStMemoryType0, radioButtonStMemoryType1);
            multiStReadMethod.Add(this.radioButtonStReadMethod0, radioButtonStReadMethod1, radioButtonStReadMethod2, radioButtonStReadMethod3, radioButtonStReadMethod4);
			multiStDataSize.Add(this.numericUpDownStReadSize);
			multiStUseAsBothInputOutput.Add(this.checkBoxStUseAsBothInputOutput);

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyTagST));
            this.checkBoxStUseAsBothInputOutput = new System.Windows.Forms.CheckBox();
            this.groupBox33 = new System.Windows.Forms.GroupBox();
            this.textBoxStAddress = new System.Windows.Forms.TextBox();
            this.label68 = new System.Windows.Forms.Label();
            this.textBoxStPort = new System.Windows.Forms.TextBox();
            this.label69 = new System.Windows.Forms.Label();
            this.groupBox35 = new System.Windows.Forms.GroupBox();
            this.radioButtonStMemoryType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonStMemoryType0 = new System.Windows.Forms.RadioButton();
            this.label70 = new System.Windows.Forms.Label();
            this.numericUpDownStReadSize = new MyNumericUpDown();
            this.groupBox26 = new System.Windows.Forms.GroupBox();
            this.radioButtonStReadMethod3 = new System.Windows.Forms.RadioButton();
            this.radioButtonStReadMethod2 = new System.Windows.Forms.RadioButton();
            this.radioButtonStReadMethod1 = new System.Windows.Forms.RadioButton();
            this.radioButtonStReadMethod0 = new System.Windows.Forms.RadioButton();
            this.radioButtonStReadMethod4 = new System.Windows.Forms.RadioButton();
            this.groupBox33.SuspendLayout();
            this.groupBox35.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStReadSize)).BeginInit();
            this.groupBox26.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxStUseAsBothInputOutput
            // 
            resources.ApplyResources(this.checkBoxStUseAsBothInputOutput, "checkBoxStUseAsBothInputOutput");
            this.checkBoxStUseAsBothInputOutput.Name = "checkBoxStUseAsBothInputOutput";
            // 
            // groupBox33
            // 
            this.groupBox33.Controls.Add(this.textBoxStAddress);
            this.groupBox33.Controls.Add(this.label68);
            this.groupBox33.Controls.Add(this.textBoxStPort);
            this.groupBox33.Controls.Add(this.label69);
            this.groupBox33.Controls.Add(this.groupBox35);
            this.groupBox33.Controls.Add(this.label70);
            this.groupBox33.Controls.Add(this.numericUpDownStReadSize);
            this.groupBox33.Controls.Add(this.groupBox26);
            resources.ApplyResources(this.groupBox33, "groupBox33");
            this.groupBox33.Name = "groupBox33";
            this.groupBox33.TabStop = false;
            // 
            // textBoxStAddress
            // 
            resources.ApplyResources(this.textBoxStAddress, "textBoxStAddress");
            this.textBoxStAddress.Name = "textBoxStAddress";
            // 
            // label68
            // 
            resources.ApplyResources(this.label68, "label68");
            this.label68.Name = "label68";
            // 
            // textBoxStPort
            // 
            resources.ApplyResources(this.textBoxStPort, "textBoxStPort");
            this.textBoxStPort.Name = "textBoxStPort";
            // 
            // label69
            // 
            resources.ApplyResources(this.label69, "label69");
            this.label69.Name = "label69";
            // 
            // groupBox35
            // 
            this.groupBox35.Controls.Add(this.radioButtonStMemoryType1);
            this.groupBox35.Controls.Add(this.radioButtonStMemoryType0);
            resources.ApplyResources(this.groupBox35, "groupBox35");
            this.groupBox35.Name = "groupBox35";
            this.groupBox35.TabStop = false;
            // 
            // radioButtonStMemoryType1
            // 
            resources.ApplyResources(this.radioButtonStMemoryType1, "radioButtonStMemoryType1");
            this.radioButtonStMemoryType1.Name = "radioButtonStMemoryType1";
            // 
            // radioButtonStMemoryType0
            // 
            resources.ApplyResources(this.radioButtonStMemoryType0, "radioButtonStMemoryType0");
            this.radioButtonStMemoryType0.Name = "radioButtonStMemoryType0";
            // 
            // label70
            // 
            resources.ApplyResources(this.label70, "label70");
            this.label70.Name = "label70";
            // 
            // numericUpDownStReadSize
            // 
            resources.ApplyResources(this.numericUpDownStReadSize, "numericUpDownStReadSize");
            this.numericUpDownStReadSize.Name = "numericUpDownStReadSize";
            // 
            // groupBox26
            // 
            this.groupBox26.Controls.Add(this.radioButtonStReadMethod4);
            this.groupBox26.Controls.Add(this.radioButtonStReadMethod3);
            this.groupBox26.Controls.Add(this.radioButtonStReadMethod2);
            this.groupBox26.Controls.Add(this.radioButtonStReadMethod1);
            this.groupBox26.Controls.Add(this.radioButtonStReadMethod0);
            resources.ApplyResources(this.groupBox26, "groupBox26");
            this.groupBox26.Name = "groupBox26";
            this.groupBox26.TabStop = false;
            // 
            // radioButtonStReadMethod3
            // 
            resources.ApplyResources(this.radioButtonStReadMethod3, "radioButtonStReadMethod3");
            this.radioButtonStReadMethod3.Name = "radioButtonStReadMethod3";
            // 
            // radioButtonStReadMethod2
            // 
            resources.ApplyResources(this.radioButtonStReadMethod2, "radioButtonStReadMethod2");
            this.radioButtonStReadMethod2.Name = "radioButtonStReadMethod2";
            // 
            // radioButtonStReadMethod1
            // 
            resources.ApplyResources(this.radioButtonStReadMethod1, "radioButtonStReadMethod1");
            this.radioButtonStReadMethod1.Name = "radioButtonStReadMethod1";
            // 
            // radioButtonStReadMethod0
            // 
            resources.ApplyResources(this.radioButtonStReadMethod0, "radioButtonStReadMethod0");
            this.radioButtonStReadMethod0.Name = "radioButtonStReadMethod0";
            // 
            // radioButtonStReadMethod4
            // 
            resources.ApplyResources(this.radioButtonStReadMethod4, "radioButtonStReadMethod4");
            this.radioButtonStReadMethod4.Name = "radioButtonStReadMethod4";
            // 
            // PropertyTagST
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.checkBoxStUseAsBothInputOutput);
            this.Controls.Add(this.groupBox33);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyTagST";
            this.ShowInTaskbar = false;
            this.groupBox33.ResumeLayout(false);
            this.groupBox33.PerformLayout();
            this.groupBox35.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStReadSize)).EndInit();
            this.groupBox26.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetTagST(TagStClass st)
		{
			multiStPort.Set(st.port);
			multiStAddress.Set(st.address);
			multiStMemoryType.Set(st.cMemoryType);
			multiStReadMethod.Set(st.read_method);
			multiStDataSize.Set(st.read_size);

			multiStUseAsBothInputOutput.Set(st.bUseAsOutput);
		}

		public void GetTagST(TagStClass st)
		{
			multiStPort.Get(ref st.port);
			multiStAddress.Get(ref st.address);
			multiStMemoryType.Get(ref st.cMemoryType);
			multiStReadMethod.Get(ref st.read_method);
			multiStDataSize.Get(ref st.read_size);


			multiStUseAsBothInputOutput.Get(ref st.bUseAsOutput);
		}

		public void EnableDisableConnectionType(int type)
		{

		}


	}
}
