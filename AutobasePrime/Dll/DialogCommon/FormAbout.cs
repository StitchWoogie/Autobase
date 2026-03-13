using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using AutoLibLocal;
using NetTools;

namespace DialogCommon
{
	/// <summary>
	/// Summary description for FormAbout.
	/// </summary>
	public class FormAbout : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxVersion;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelCompany;
		private System.Windows.Forms.TextBox textBoxProgram;
		private System.Windows.Forms.TextBox textBoxCompany;
		private System.Windows.Forms.PictureBox pictureBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormAbout()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxVersion = new System.Windows.Forms.TextBox();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.textBoxProgram = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCompany = new System.Windows.Forms.TextBox();
            this.labelCompany = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.AccessibleDescription = null;
            this.buttonClose.AccessibleName = null;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.BackgroundImage = null;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Font = null;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // textBoxVersion
            // 
            this.textBoxVersion.AccessibleDescription = null;
            this.textBoxVersion.AccessibleName = null;
            resources.ApplyResources(this.textBoxVersion, "textBoxVersion");
            this.textBoxVersion.BackgroundImage = null;
            this.textBoxVersion.Font = null;
            this.textBoxVersion.Name = "textBoxVersion";
            this.textBoxVersion.ReadOnly = true;
            // 
            // m_list
            // 
            this.m_list.AccessibleDescription = null;
            this.m_list.AccessibleName = null;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.BackgroundImage = null;
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.m_list.Font = null;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // textBoxProgram
            // 
            this.textBoxProgram.AccessibleDescription = null;
            this.textBoxProgram.AccessibleName = null;
            resources.ApplyResources(this.textBoxProgram, "textBoxProgram");
            this.textBoxProgram.BackgroundImage = null;
            this.textBoxProgram.Font = null;
            this.textBoxProgram.Name = "textBoxProgram";
            this.textBoxProgram.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // textBoxCompany
            // 
            this.textBoxCompany.AccessibleDescription = null;
            this.textBoxCompany.AccessibleName = null;
            resources.ApplyResources(this.textBoxCompany, "textBoxCompany");
            this.textBoxCompany.BackgroundImage = null;
            this.textBoxCompany.Font = null;
            this.textBoxCompany.Name = "textBoxCompany";
            this.textBoxCompany.ReadOnly = true;
            // 
            // labelCompany
            // 
            this.labelCompany.AccessibleDescription = null;
            this.labelCompany.AccessibleName = null;
            resources.ApplyResources(this.labelCompany, "labelCompany");
            this.labelCompany.Font = null;
            this.labelCompany.Name = "labelCompany";
            // 
            // pictureBox1
            // 
            this.pictureBox1.AccessibleDescription = null;
            this.pictureBox1.AccessibleName = null;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.BackgroundImage = null;
            this.pictureBox1.Font = null;
            this.pictureBox1.ImageLocation = null;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // FormAbout
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBoxCompany);
            this.Controls.Add(this.textBoxProgram);
            this.Controls.Add(this.textBoxVersion);
            this.Controls.Add(this.labelCompany);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_list);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        public string sFixTextProgram = "";     // 이 텍스트를 외부에서 사용하면 이 텍스트가 표기된다.
        public bool bDisplayCompany = true;     // 회사명 항목을 보여준다. (ViewMain의 경우 OEM 버전때문에 회사명을 보여주지 않는것이 좋다.)

		private void FormAbout_Load(object sender, System.EventArgs e)
		{
			this.CenterToScreen();

            if (sFixTextProgram.Length > 0)
                textBoxProgram.Text = sFixTextProgram;
            else
            {
                string oem = TotalConfig.AutoBaseIniGetOemProgramName();
                string product_name = Application.ProductName;

                if (TotalConfig.eOemType == EnumOemType.UYeG_GS ||
                    TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
                {
                    if (product_name == "LocalMain")
                        product_name = "감시";
                    else if (product_name == "Studio")
                        product_name = "스튜디오";
                }
                else if (TotalConfig.eOemType == EnumOemType.FiveTek)
                {
                    textBoxProgram.Multiline = true;
                    textBoxProgram.Top -= 6;
                    textBoxProgram.Height = 31;
                }
                else
                {

                }

                if (TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite && oem.IndexOf("Lite") == -1)
                {
                    oem += " Lite";
                }

                textBoxProgram.Text = oem + " " + product_name;
            }

            // Lite 문자열이 없고 SCADA Lite 버전일 때는 Lite를 붙여준다.
            if (TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite && textBoxProgram.Text.IndexOf("Lite") == -1)
            {
                textBoxProgram.Text += " Lite";
            }

            if(TotalConfig.bVersionCertificationSmallBusinessProducts)
            {
                textBoxVersion.Text = TotalConfig.AutoBaseIntGetVersionString();
            }
            else if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                textBoxVersion.Text = TotalConfig.AutoBaseIntGetVersionString();
            }
            else if (TotalConfig.eOemType == EnumOemType.FiveTek)
            {
                string value = "";
                Profile.GetPrivateProfileStringW("OEM", "Version", Application.ProductVersion, ref value, TotalConfig.GetProgramConfigFilename());

                textBoxVersion.Text = value;
            }
            else if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
                //textBoxVersion.Text = TotalConfig.AutoBaseIntGetVersionString();  //인증버전 10.3.0.8 버전은 Major.Minor만 표시했다.
                textBoxVersion.Text = Application.ProductVersion;
            }
            else if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                textBoxVersion.Text = TotalConfig.AutoBaseIntGetVersionString();
            }
            else if (TotalConfig.eOemType == EnumOemType.KobasAI)
            {
                textBoxVersion.Text = TotalConfig.AutoBaseIntGetVersionString();
            }
            else
            {
                textBoxVersion.Text = Application.ProductVersion;
            }

            if (bDisplayCompany)
            {
                textBoxCompany.Text = TotalConfig.AutoBaseIniGetOemCompanyName();
            }
            else
            {
                this.textBoxCompany.Visible = false;
                this.labelCompany.Visible = false;
            }

			Assembly[] myAssemblies = Thread.GetDomain().GetAssemblies();

			// Get the dynamic assembly named 'MyAssembly'. 
			Assembly asm = null;
			AssemblyName name;
			ListViewItem item;

			for(int i = 0; i < myAssemblies.Length; i++)
			{
				//if(String.Compare(myAssemblies[i].GetName().Name, "MyAssembly") == 0)
				asm = myAssemblies[i];
				name = asm.GetName();
				item = new ListViewItem(name.Name);
				item.SubItems.Add(name.Version.ToString());
				m_list.Items.Add(item);
			}

            if (tempIcon != null)
            {
                try
                {
                    this.pictureBox1.Image = tempIcon.ToBitmap();
                }
                catch
                {
                }
            }

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.pictureBox1.Visible = false;
                this.m_list.Visible = false;

                this.Width -= 86;
                this.Height -= 220;

                this.buttonClose.Location = new Point(this.ClientRectangle.Width/2-this.buttonClose.Size.Width/2, this.buttonClose.Location.Y - 220);
                if (Tools.IsLangKorean())
                    this.buttonClose.Text = "확인";
            }
            else if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
                this.m_list.Visible = false;

                this.Height -= 220;

                this.buttonClose.Location = new Point(this.ClientRectangle.Width / 2 - this.buttonClose.Size.Width / 2, this.buttonClose.Location.Y - 220);
            }
		}

		Icon tempIcon;

		public Icon ProgramIcon
		{
			set 
			{
                tempIcon = new Icon(value, 85, 85);
			}
		}

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			
		}
	}
}
