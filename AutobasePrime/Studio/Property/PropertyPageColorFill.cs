using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageColor.
	/// </summary>
	public class PropertyPageColorFill : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panelColor;
		PropertyPageColorPublic formChild = new PropertyPageColorPublic();

		bool bMultiSelectOption = false;
		private System.Windows.Forms.CheckBox checkBoxUseFill;
		int  nFillOption;

		public void SetSelectedColor(Color val, bool first_flag)
		{
			formChild.SetSelectedColor(val, first_flag);
		}

		public Color GetSelectedColor()
		{
			return formChild.GetSelectedColor();
		}

		public int GetSelectedOption()
		{
			nFillOption = this.checkBoxUseFill.Checked ? 1 : 0;
			return nFillOption;
		}

		public bool IsMultiSelectedColor()
		{
			return formChild.IsMultiSelected();
		}

		public bool IsMultiSelectedOption()
		{
			return bMultiSelectOption;
		}

		public PropertyPageColorFill()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageColorFill));
            this.panelColor = new System.Windows.Forms.Panel();
            this.checkBoxUseFill = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // panelColor
            // 
            this.panelColor.AccessibleDescription = null;
            this.panelColor.AccessibleName = null;
            resources.ApplyResources(this.panelColor, "panelColor");
            this.panelColor.BackgroundImage = null;
            this.panelColor.Font = null;
            this.panelColor.Name = "panelColor";
            // 
            // checkBoxUseFill
            // 
            this.checkBoxUseFill.AccessibleDescription = null;
            this.checkBoxUseFill.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseFill, "checkBoxUseFill");
            this.checkBoxUseFill.BackgroundImage = null;
            this.checkBoxUseFill.Font = null;
            this.checkBoxUseFill.Name = "checkBoxUseFill";
            // 
            // PropertyPageColorFill
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.checkBoxUseFill);
            this.Controls.Add(this.panelColor);
            this.Icon = null;
            this.Name = "PropertyPageColorFill";
            this.Load += new System.EventHandler(this.PropertyPageColor_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageColor_Load(object sender, System.EventArgs e)
		{
			formChild.TopLevel = false;

			panelColor.Controls.Add(formChild);
			formChild.Show();
		}

		public void SetSelectedOption(int val, bool first_flag)
		{
			//formChild.SetSelectedColor(val, first_flag);
			if(first_flag) 
			{
				nFillOption = val;
				this.checkBoxUseFill.Checked = (val == 1);
			}
			else 
			{
				if(nFillOption != val) 
				{
					bMultiSelectOption = true;

					this.checkBoxUseFill.CheckState = CheckState.Indeterminate;
				}
			}
		}
	}
}
