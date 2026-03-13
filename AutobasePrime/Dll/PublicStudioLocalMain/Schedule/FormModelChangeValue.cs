using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormModelChangeValue.
	/// </summary>
	public class FormModelChangeValue : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.TextBox textBoxTag;
		public System.Windows.Forms.TextBox textBoxValue;
		private System.Windows.Forms.Label label2;
        public TextBox textBoxDescription;
        private Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormModelChangeValue()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModelChangeValue));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTag = new System.Windows.Forms.TextBox();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxTag
            // 
            resources.ApplyResources(this.textBoxTag, "textBoxTag");
            this.textBoxTag.Name = "textBoxTag";
            this.textBoxTag.ReadOnly = true;
            // 
            // textBoxValue
            // 
            resources.ApplyResources(this.textBoxValue, "textBoxValue");
            this.textBoxValue.Name = "textBoxValue";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // FormModelChangeValue
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTag);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormModelChangeValue";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormModelChangeValue_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxValue, TextBoxLimit.MAX_StringValue)) return;
            
            int[] tag_pos = new int[1];

            TagPublicClass tp = TagLib.GetStructPublic(this.textBoxTag.Text, ref tag_pos);

            if (tp.enumTagType == EnumTagType.DI || tp.enumTagType == EnumTagType.DO || tp.enumTagType == EnumTagType.GDO)
            {
                string value = this.textBoxValue.Text.Trim();

                if (value != "0" && value != "1")
                {
                    MessageBox.Show("디지털 태그는 값이 0 또는 1 이어야 합니다.", "값 입력 오류");
                    return;
                }
            }

			DialogResult = DialogResult.OK;
			Close();
		}

        private void FormModelChangeValue_Load(object sender, EventArgs e)
        {
            
        }
	}
}
