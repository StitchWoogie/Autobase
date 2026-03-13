namespace OpcClient
{
    partial class opcServerViewWriteDelayForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(opcServerViewWriteDelayForm));
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label_IsTimeDelay = new System.Windows.Forms.Label();
            this.label_WriteCount = new System.Windows.Forms.Label();
            this.listView_WriteWaitting = new System.Windows.Forms.ListView();
            this.No = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.button_OK = new System.Windows.Forms.Button();
            this.timerViewWrite = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label_IsTimeDelay);
            this.groupBox7.Controls.Add(this.label_WriteCount);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // label_IsTimeDelay
            // 
            resources.ApplyResources(this.label_IsTimeDelay, "label_IsTimeDelay");
            this.label_IsTimeDelay.Name = "label_IsTimeDelay";
            // 
            // label_WriteCount
            // 
            resources.ApplyResources(this.label_WriteCount, "label_WriteCount");
            this.label_WriteCount.Name = "label_WriteCount";
            // 
            // listView_WriteWaitting
            // 
            this.listView_WriteWaitting.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.No,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            resources.ApplyResources(this.listView_WriteWaitting, "listView_WriteWaitting");
            this.listView_WriteWaitting.FullRowSelect = true;
            this.listView_WriteWaitting.Name = "listView_WriteWaitting";
            this.listView_WriteWaitting.UseCompatibleStateImageBehavior = false;
            this.listView_WriteWaitting.View = System.Windows.Forms.View.Details;
            // 
            // No
            // 
            resources.ApplyResources(this.No, "No");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            // 
            // timerViewWrite
            // 
            this.timerViewWrite.Enabled = true;
            this.timerViewWrite.Interval = 1000;
            this.timerViewWrite.Tick += new System.EventHandler(this.timerViewWrite_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView_WriteWaitting);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // opcServerViewWriteDelayForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "opcServerViewWriteDelayForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.opcServerViewWriteDelayForm_Load);
            this.groupBox7.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label_IsTimeDelay;
        private System.Windows.Forms.Label label_WriteCount;
        private System.Windows.Forms.ListView listView_WriteWaitting;
        private System.Windows.Forms.ColumnHeader No;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Timer timerViewWrite;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}