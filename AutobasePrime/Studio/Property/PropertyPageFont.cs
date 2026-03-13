using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools.OldDefine;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageFont.
	/// </summary>
	public class PropertyPageFont : System.Windows.Forms.Form
	{
		public System.Windows.Forms.ComboBox comboBoxFamily;
		public System.Windows.Forms.ComboBox comboBoxSize;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.CheckBox checkBoxItalic;
		public System.Windows.Forms.CheckBox checkBoxBold;
		public System.Windows.Forms.CheckBox checkBoxUnderline;
        private System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.CheckBox checkBoxStrikeout;
        private Panel panelPreview;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageFont()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

            // 이부분이 속도가 좀 지연된다 그래서 static으로 잡았음 2007.10.30
            if (font_names == null)
            {
                font_names = new string[FontFamily.Families.Length];
                FontFamily family;
                for (int i = 0; i < FontFamily.Families.Length; i++)
                {
                    family = FontFamily.Families[i];
                    font_names[i] = family.Name;
                    
                }
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageFont));
            this.comboBoxFamily = new System.Windows.Forms.ComboBox();
            this.comboBoxSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxItalic = new System.Windows.Forms.CheckBox();
            this.checkBoxBold = new System.Windows.Forms.CheckBox();
            this.checkBoxUnderline = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.checkBoxStrikeout = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxFamily
            // 
            this.comboBoxFamily.AccessibleDescription = null;
            this.comboBoxFamily.AccessibleName = null;
            resources.ApplyResources(this.comboBoxFamily, "comboBoxFamily");
            this.comboBoxFamily.BackgroundImage = null;
            this.comboBoxFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBoxFamily.Font = null;
            this.comboBoxFamily.Name = "comboBoxFamily";
            this.comboBoxFamily.SelectedIndexChanged += new System.EventHandler(this.comboBoxFamily_SelectedIndexChanged);
            this.comboBoxFamily.TextChanged += new System.EventHandler(this.comboBoxFamily_TextChanged);
            // 
            // comboBoxSize
            // 
            this.comboBoxSize.AccessibleDescription = null;
            this.comboBoxSize.AccessibleName = null;
            resources.ApplyResources(this.comboBoxSize, "comboBoxSize");
            this.comboBoxSize.BackgroundImage = null;
            this.comboBoxSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBoxSize.Font = null;
            this.comboBoxSize.Name = "comboBoxSize";
            this.comboBoxSize.TextChanged += new System.EventHandler(this.comboBoxSize_TextChanged);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // checkBoxItalic
            // 
            this.checkBoxItalic.AccessibleDescription = null;
            this.checkBoxItalic.AccessibleName = null;
            resources.ApplyResources(this.checkBoxItalic, "checkBoxItalic");
            this.checkBoxItalic.BackgroundImage = null;
            this.checkBoxItalic.Font = null;
            this.checkBoxItalic.Name = "checkBoxItalic";
            this.checkBoxItalic.CheckedChanged += new System.EventHandler(this.checkBoxItalic_CheckedChanged);
            // 
            // checkBoxBold
            // 
            this.checkBoxBold.AccessibleDescription = null;
            this.checkBoxBold.AccessibleName = null;
            resources.ApplyResources(this.checkBoxBold, "checkBoxBold");
            this.checkBoxBold.BackgroundImage = null;
            this.checkBoxBold.Font = null;
            this.checkBoxBold.Name = "checkBoxBold";
            this.checkBoxBold.CheckedChanged += new System.EventHandler(this.checkBoxBold_CheckedChanged);
            // 
            // checkBoxUnderline
            // 
            this.checkBoxUnderline.AccessibleDescription = null;
            this.checkBoxUnderline.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUnderline, "checkBoxUnderline");
            this.checkBoxUnderline.BackgroundImage = null;
            this.checkBoxUnderline.Font = null;
            this.checkBoxUnderline.Name = "checkBoxUnderline";
            this.checkBoxUnderline.CheckedChanged += new System.EventHandler(this.checkBoxUnderline_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.panelPreview);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panelPreview
            // 
            this.panelPreview.AccessibleDescription = null;
            this.panelPreview.AccessibleName = null;
            resources.ApplyResources(this.panelPreview, "panelPreview");
            this.panelPreview.BackgroundImage = null;
            this.panelPreview.Font = null;
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPreview_Paint);
            // 
            // checkBoxStrikeout
            // 
            this.checkBoxStrikeout.AccessibleDescription = null;
            this.checkBoxStrikeout.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStrikeout, "checkBoxStrikeout");
            this.checkBoxStrikeout.BackgroundImage = null;
            this.checkBoxStrikeout.Font = null;
            this.checkBoxStrikeout.Name = "checkBoxStrikeout";
            this.checkBoxStrikeout.CheckedChanged += new System.EventHandler(this.checkBoxStrikeout_CheckedChanged);
            // 
            // PropertyPageFont
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.checkBoxStrikeout);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxUnderline);
            this.Controls.Add(this.checkBoxBold);
            this.Controls.Add(this.checkBoxItalic);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxSize);
            this.Controls.Add(this.comboBoxFamily);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyPageFont";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PropertyPageFont_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		bool fill_flag = false;
		void FillList()
		{
			if(fill_flag)	return;	

			for(int i = 0; i < font_names.Length; i++) 
			{
				comboBoxFamily.Items.Add(font_names[i]);
			}

			for(int i = 2; i < 100; i++) 
			{
				comboBoxSize.Items.Add(i.ToString());
			}

			fill_flag = true;
		}

		static string[] font_names = null;

		private void PropertyPageFont_Load(object sender, System.EventArgs e)
		{
			FillList();
		}

		bool IsFontExist(string name)
		{
			if(font_names == null)	return false;
			for(int i = 0; i < font_names.Length; i++) 
			{
				if(String.Compare(font_names[i], name, true) == 0)	return true;				
			}
			
			return false;
		}

		void PreviewFontChange()
		{
			int size;
			
			size = ConvertTool.ToInt32(comboBoxSize.Text);
			if(size == 0)	return;

			FontStyle style = 0;
			if(checkBoxItalic.Checked)		style |= FontStyle.Italic;
			if(checkBoxBold.Checked)		style |= FontStyle.Bold;
			if(checkBoxUnderline.Checked)	style |= FontStyle.Underline;
			if(checkBoxStrikeout.Checked)	style |= FontStyle.Strikeout;

			Font font = null;
			string font_name = this.comboBoxFamily.Text;

			if(!this.IsFontExist(font_name)) 
			{
				font_name = "Arial";
			}

			try 
			{
				font = new Font(font_name, size, style);
				panelPreview.BackColor = Color.White;
			}
			catch	// 폰트가 해당 스타일을 지원하지 않을 때 발생한다. 
			{
				font = new Font("Arial", size, style);
                panelPreview.BackColor = Color.Red;
			}

            panelPreview.Font = font;
		}

		private void checkBoxItalic_CheckedChanged(object sender, System.EventArgs e)
		{
			PreviewFontChange();
		}

		private void checkBoxBold_CheckedChanged(object sender, System.EventArgs e)
		{
			PreviewFontChange();
		}

		private void checkBoxUnderline_CheckedChanged(object sender, System.EventArgs e)
		{
			PreviewFontChange();
        }

		private void checkBoxStrikeout_CheckedChanged(object sender, System.EventArgs e)
		{
			PreviewFontChange();
		}

		private void comboBoxSize_TextChanged(object sender, System.EventArgs e)
		{
			PreviewFontChange();
		}

		void ActiveStyle()
		{
			string font_name = this.comboBoxFamily.Text;
			FontFamily family;
			
			if(!this.IsFontExist(font_name)) 
			{
				font_name = "Arial";
			}

			family = new FontFamily(font_name);
			
			bool flag = family.IsStyleAvailable(FontStyle.Italic);
			this.checkBoxItalic.Enabled = flag;
			if(!flag) this.checkBoxItalic.Checked = false;

			flag = family.IsStyleAvailable(FontStyle.Bold);
			this.checkBoxBold.Enabled = flag;
			if(!flag) this.checkBoxBold.Checked = false;
		}

		private void comboBoxFamily_TextChanged(object sender, System.EventArgs e)
		{
			ActiveStyle();
			PreviewFontChange();
		}

		public void SetLogFont(LOGFONT lf, bool first)
		{
			FillList();
			
			if(first) 
			{
				comboBoxFamily.Text = lf.lfFaceName;
				comboBoxSize.Text = (lf.lfHeight).ToString();

				//comboBoxFamily.SelectedText = lf.lfFaceName;
				//comboBoxSize.SelectedText = (lf.lfHeight).ToString();

				checkBoxItalic.Checked = ((lf.style & FontStyle.Italic) == FontStyle.Italic);
				checkBoxBold.Checked = ((lf.style & FontStyle.Bold) == FontStyle.Bold);
				checkBoxUnderline.Checked = ((lf.style & FontStyle.Underline) == FontStyle.Underline);
				checkBoxStrikeout.Checked = ((lf.style & FontStyle.Strikeout) == FontStyle.Strikeout);

			}
			else 
			{
				if(comboBoxFamily.Text != lf.lfFaceName) 
				{
					comboBoxFamily.Text = "";
				}
				if(comboBoxSize.Text != (lf.lfHeight).ToString()) 
				{
					comboBoxSize.Text = "";
				}

				bool check;

				check = ((lf.style & FontStyle.Italic) == FontStyle.Italic);

				if(checkBoxItalic.Checked != check) 
					checkBoxItalic.CheckState = CheckState.Indeterminate;

				check = ((lf.style & FontStyle.Bold) == FontStyle.Bold);

				if(checkBoxBold.Checked != check) 
					checkBoxBold.CheckState = CheckState.Indeterminate;

				check = ((lf.style & FontStyle.Underline) == FontStyle.Underline);

				if(checkBoxUnderline.Checked != check) 
					checkBoxUnderline.CheckState = CheckState.Indeterminate;

				check = ((lf.style & FontStyle.Strikeout) == FontStyle.Strikeout);

				if(checkBoxStrikeout.Checked != check) 
					checkBoxStrikeout.CheckState = CheckState.Indeterminate;
			}
		}

		private void labelPreview_Click(object sender, System.EventArgs e)
		{
		
		}

		private void comboBoxFamily_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

        private void panelPreview_Paint(object sender, PaintEventArgs e)
        {
                if(Tools.IsLangKorean())
                    SafeException.SafeDrawString(e.Graphics, "글꼴abcABC123", panelPreview.Font, Brushes.Black, 0, 0);
                else
                    SafeException.SafeDrawString(e.Graphics, "abcABC123", panelPreview.Font, Brushes.Black, 0, 0);
        }
	}
}
