//============================================================================
// TITLE: NetworkCredentialsDlg.cs
//
// CONTENTS:
// 
// A dialog used specify the username/password required to access a OPC server.
//
// (c) Copyright 2003 The OPC Foundation
// ALL RIGHTS RESERVED.
//
// DISCLAIMER:
//  This code is provided by the OPC Foundation solely to assist in 
//  understanding and use of the appropriate OPC Specification(s) and may be 
//  used as set forth in the License Grant section of the OPC Specification.
//  This code is provided as-is and without warranty or support of any sort
//  and is subject to the Warranty and Liability Disclaimers which appear
//  in the printed OPC Specification.
//
// MODIFICATION LOG:
//
// Date       By    Notes
// ---------- ---   -----
// 2003/06/11 RSA   Initial implementation.

using System;
using System.Net;
using System.Xml;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpcClient
{	
	/// <summary>
	/// A dialog used specify the username/password required to access a OPC server.
	/// </summary>
	public class NetworkCredentialsDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button CancelBTN;
		private System.Windows.Forms.Button OkBTN;
		private System.Windows.Forms.Panel ButtonsPN;
		private System.Windows.Forms.Panel TopPN;
		private System.Windows.Forms.TextBox DomainTB;
		private System.Windows.Forms.Label DomainLB;
		private System.Windows.Forms.Label UserNameLB;
		private System.Windows.Forms.Label PasswordLB;
		private System.Windows.Forms.TextBox UserNameTB;
		private System.Windows.Forms.TextBox PasswordTB;
		private System.ComponentModel.IContainer components = null;

		public NetworkCredentialsDlg()
		{
			// Required for Windows Form Designer support
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkCredentialsDlg));
            this.OkBTN = new System.Windows.Forms.Button();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.ButtonsPN = new System.Windows.Forms.Panel();
            this.DomainTB = new System.Windows.Forms.TextBox();
            this.DomainLB = new System.Windows.Forms.Label();
            this.UserNameLB = new System.Windows.Forms.Label();
            this.PasswordLB = new System.Windows.Forms.Label();
            this.TopPN = new System.Windows.Forms.Panel();
            this.PasswordTB = new System.Windows.Forms.TextBox();
            this.UserNameTB = new System.Windows.Forms.TextBox();
            this.ButtonsPN.SuspendLayout();
            this.TopPN.SuspendLayout();
            this.SuspendLayout();
            // 
            // OkBTN
            // 
            this.OkBTN.AccessibleDescription = null;
            this.OkBTN.AccessibleName = null;
            resources.ApplyResources(this.OkBTN, "OkBTN");
            this.OkBTN.BackgroundImage = null;
            this.OkBTN.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBTN.Font = null;
            this.OkBTN.Name = "OkBTN";
            // 
            // CancelBTN
            // 
            this.CancelBTN.AccessibleDescription = null;
            this.CancelBTN.AccessibleName = null;
            resources.ApplyResources(this.CancelBTN, "CancelBTN");
            this.CancelBTN.BackgroundImage = null;
            this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBTN.Font = null;
            this.CancelBTN.Name = "CancelBTN";
            // 
            // ButtonsPN
            // 
            this.ButtonsPN.AccessibleDescription = null;
            this.ButtonsPN.AccessibleName = null;
            resources.ApplyResources(this.ButtonsPN, "ButtonsPN");
            this.ButtonsPN.BackgroundImage = null;
            this.ButtonsPN.Controls.Add(this.CancelBTN);
            this.ButtonsPN.Controls.Add(this.OkBTN);
            this.ButtonsPN.Font = null;
            this.ButtonsPN.Name = "ButtonsPN";
            // 
            // DomainTB
            // 
            this.DomainTB.AccessibleDescription = null;
            this.DomainTB.AccessibleName = null;
            resources.ApplyResources(this.DomainTB, "DomainTB");
            this.DomainTB.BackgroundImage = null;
            this.DomainTB.Font = null;
            this.DomainTB.Name = "DomainTB";
            // 
            // DomainLB
            // 
            this.DomainLB.AccessibleDescription = null;
            this.DomainLB.AccessibleName = null;
            resources.ApplyResources(this.DomainLB, "DomainLB");
            this.DomainLB.Font = null;
            this.DomainLB.Name = "DomainLB";
            // 
            // UserNameLB
            // 
            this.UserNameLB.AccessibleDescription = null;
            this.UserNameLB.AccessibleName = null;
            resources.ApplyResources(this.UserNameLB, "UserNameLB");
            this.UserNameLB.Font = null;
            this.UserNameLB.Name = "UserNameLB";
            // 
            // PasswordLB
            // 
            this.PasswordLB.AccessibleDescription = null;
            this.PasswordLB.AccessibleName = null;
            resources.ApplyResources(this.PasswordLB, "PasswordLB");
            this.PasswordLB.Font = null;
            this.PasswordLB.Name = "PasswordLB";
            // 
            // TopPN
            // 
            this.TopPN.AccessibleDescription = null;
            this.TopPN.AccessibleName = null;
            resources.ApplyResources(this.TopPN, "TopPN");
            this.TopPN.BackgroundImage = null;
            this.TopPN.Controls.Add(this.PasswordTB);
            this.TopPN.Controls.Add(this.UserNameTB);
            this.TopPN.Controls.Add(this.DomainTB);
            this.TopPN.Controls.Add(this.UserNameLB);
            this.TopPN.Controls.Add(this.PasswordLB);
            this.TopPN.Controls.Add(this.DomainLB);
            this.TopPN.Font = null;
            this.TopPN.Name = "TopPN";
            // 
            // PasswordTB
            // 
            this.PasswordTB.AccessibleDescription = null;
            this.PasswordTB.AccessibleName = null;
            resources.ApplyResources(this.PasswordTB, "PasswordTB");
            this.PasswordTB.BackgroundImage = null;
            this.PasswordTB.Font = null;
            this.PasswordTB.Name = "PasswordTB";
            // 
            // UserNameTB
            // 
            this.UserNameTB.AccessibleDescription = null;
            this.UserNameTB.AccessibleName = null;
            resources.ApplyResources(this.UserNameTB, "UserNameTB");
            this.UserNameTB.BackgroundImage = null;
            this.UserNameTB.Font = null;
            this.UserNameTB.Name = "UserNameTB";
            // 
            // NetworkCredentialsDlg
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.CancelBTN;
            this.Controls.Add(this.ButtonsPN);
            this.Controls.Add(this.TopPN);
            this.Icon = null;
            this.Name = "NetworkCredentialsDlg";
            this.ButtonsPN.ResumeLayout(false);
            this.TopPN.ResumeLayout(false);
            this.TopPN.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Displays the network credentials in a model dialog.
		/// </summary>
		public NetworkCredential ShowDialog(NetworkCredential credentials)
		{
			if (credentials != null)
			{
				UserNameTB.Text = credentials.UserName;
				PasswordTB.Text = credentials.Password;
				DomainTB.Text   = credentials.Domain;
			}

            if (ShowDialog(Form.ActiveForm) != DialogResult.OK)
			{
				return null;
			}

			if (DomainTB.Text == null || DomainTB.Text == "")
			{
				return new NetworkCredential(UserNameTB.Text, PasswordTB.Text);
			}

			return new NetworkCredential(UserNameTB.Text, PasswordTB.Text, DomainTB.Text);
		}
	}
}
