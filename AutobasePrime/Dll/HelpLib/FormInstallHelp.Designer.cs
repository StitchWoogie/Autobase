namespace HelpLib
{
    partial class FormInstallHelp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInstallHelp));
            this.progressBarFile = new System.Windows.Forms.ProgressBar();
            this.labelFile = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            this.progressBarTotal = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBarFile
            // 
            this.progressBarFile.AccessibleDescription = null;
            this.progressBarFile.AccessibleName = null;
            resources.ApplyResources(this.progressBarFile, "progressBarFile");
            this.progressBarFile.BackgroundImage = null;
            this.progressBarFile.Font = null;
            this.progressBarFile.Name = "progressBarFile";
            // 
            // labelFile
            // 
            this.labelFile.AccessibleDescription = null;
            this.labelFile.AccessibleName = null;
            resources.ApplyResources(this.labelFile, "labelFile");
            this.labelFile.Font = null;
            this.labelFile.Name = "labelFile";
            // 
            // labelTotal
            // 
            this.labelTotal.AccessibleDescription = null;
            this.labelTotal.AccessibleName = null;
            resources.ApplyResources(this.labelTotal, "labelTotal");
            this.labelTotal.Font = null;
            this.labelTotal.Name = "labelTotal";
            // 
            // progressBarTotal
            // 
            this.progressBarTotal.AccessibleDescription = null;
            this.progressBarTotal.AccessibleName = null;
            resources.ApplyResources(this.progressBarTotal, "progressBarTotal");
            this.progressBarTotal.BackgroundImage = null;
            this.progressBarTotal.Font = null;
            this.progressBarTotal.Name = "progressBarTotal";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormInstallHelp
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.progressBarTotal);
            this.Controls.Add(this.labelFile);
            this.Controls.Add(this.progressBarFile);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormInstallHelp";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormInstallHelp_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormInstallHelp_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarFile;
        private System.Windows.Forms.Label labelFile;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.ProgressBar progressBarTotal;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonCancel;
    }
}