namespace PublicStudioLocalMain.Recipe
{
	partial class FormElectronicSignature
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
				components.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panelHeader = new System.Windows.Forms.Panel();
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblObjectInfo = new System.Windows.Forms.Label();
			this.lblUsername = new System.Windows.Forms.Label();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.lblPasswordError = new System.Windows.Forms.Label();
			this.lblReason = new System.Windows.Forms.Label();
			this.txtReason = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.panelHeader.SuspendLayout();
			this.SuspendLayout();
			//
			// panelHeader
			//
			this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
			this.panelHeader.Controls.Add(this.lblTitle);
			this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelHeader.Location = new System.Drawing.Point(0, 0);
			this.panelHeader.Name = "panelHeader";
			this.panelHeader.Size = new System.Drawing.Size(404, 40);
			this.panelHeader.TabIndex = 0;
			//
			// lblTitle
			//
			this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
			this.lblTitle.ForeColor = System.Drawing.Color.White;
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(404, 40);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Electronic Signature";
			this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			// lblObjectInfo
			//
			this.lblObjectInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.lblObjectInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
			this.lblObjectInfo.Location = new System.Drawing.Point(20, 48);
			this.lblObjectInfo.Name = "lblObjectInfo";
			this.lblObjectInfo.Size = new System.Drawing.Size(360, 20);
			this.lblObjectInfo.TabIndex = 1;
			this.lblObjectInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			// lblUsername
			//
			this.lblUsername.Location = new System.Drawing.Point(20, 78);
			this.lblUsername.Name = "lblUsername";
			this.lblUsername.Size = new System.Drawing.Size(80, 23);
			this.lblUsername.TabIndex = 2;
			this.lblUsername.Text = "User:";
			this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			//
			// txtUsername
			//
			this.txtUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
			this.txtUsername.Location = new System.Drawing.Point(110, 78);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.ReadOnly = true;
			this.txtUsername.Size = new System.Drawing.Size(270, 23);
			this.txtUsername.TabIndex = 3;
			//
			// lblPassword
			//
			this.lblPassword.Location = new System.Drawing.Point(20, 113);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(80, 23);
			this.lblPassword.TabIndex = 4;
			this.lblPassword.Text = "Password:";
			this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			//
			// txtPassword
			//
			this.txtPassword.Location = new System.Drawing.Point(110, 113);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.Size = new System.Drawing.Size(270, 23);
			this.txtPassword.TabIndex = 5;
			this.txtPassword.UseSystemPasswordChar = true;
			//
			// lblPasswordError
			//
			this.lblPasswordError.Font = new System.Drawing.Font("Segoe UI", 8F);
			this.lblPasswordError.ForeColor = System.Drawing.Color.Red;
			this.lblPasswordError.Location = new System.Drawing.Point(110, 138);
			this.lblPasswordError.Name = "lblPasswordError";
			this.lblPasswordError.Size = new System.Drawing.Size(270, 18);
			this.lblPasswordError.TabIndex = 6;
			//
			// lblReason
			//
			this.lblReason.Location = new System.Drawing.Point(20, 163);
			this.lblReason.Name = "lblReason";
			this.lblReason.Size = new System.Drawing.Size(80, 23);
			this.lblReason.TabIndex = 7;
			this.lblReason.Text = "Reason:";
			this.lblReason.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			//
			// txtReason
			//
			this.txtReason.Location = new System.Drawing.Point(110, 163);
			this.txtReason.Multiline = true;
			this.txtReason.Name = "txtReason";
			this.txtReason.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtReason.Size = new System.Drawing.Size(270, 80);
			this.txtReason.TabIndex = 8;
			//
			// btnOK
			//
			this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOK.ForeColor = System.Drawing.Color.White;
			this.btnOK.Location = new System.Drawing.Point(170, 261);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 32);
			this.btnOK.TabIndex = 9;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
			//
			// btnCancel
			//
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCancel.Location = new System.Drawing.Point(280, 261);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 32);
			this.btnCancel.TabIndex = 10;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
			//
			// FormElectronicSignature
			//
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(404, 311);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtReason);
			this.Controls.Add(this.lblReason);
			this.Controls.Add(this.lblPasswordError);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.txtUsername);
			this.Controls.Add(this.lblUsername);
			this.Controls.Add(this.lblObjectInfo);
			this.Controls.Add(this.panelHeader);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(380, 330);
			this.Name = "FormElectronicSignature";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Electronic Signature";
			this.panelHeader.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Panel panelHeader;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblObjectInfo;
		private System.Windows.Forms.Label lblUsername;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label lblPasswordError;
		private System.Windows.Forms.Label lblReason;
		private System.Windows.Forms.TextBox txtReason;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}
