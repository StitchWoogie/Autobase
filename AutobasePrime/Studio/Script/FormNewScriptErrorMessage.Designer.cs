namespace Studio.Script
{
    partial class FormNewScriptErrorMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewScriptErrorMessage));
            this.listViewErrorList = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewErrorList
            // 
            this.listViewErrorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewErrorList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewErrorList.FullRowSelect = true;
            this.listViewErrorList.HideSelection = false;
            this.listViewErrorList.Location = new System.Drawing.Point(0, 0);
            this.listViewErrorList.Name = "listViewErrorList";
            this.listViewErrorList.Size = new System.Drawing.Size(788, 222);
            this.listViewErrorList.SmallImageList = this.imageList1;
            this.listViewErrorList.TabIndex = 1;
            this.listViewErrorList.UseCompatibleStateImageBehavior = false;
            this.listViewErrorList.View = System.Windows.Forms.View.Details;
            this.listViewErrorList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewErrorList_MouseDoubleClick);
            this.listViewErrorList.SelectedIndexChanged += new System.EventHandler(this.listViewErrorList_SelectedIndexChanged);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "";
            this.columnHeader4.Width = 20;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Message";
            this.columnHeader1.Width = 367;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Line";
            this.columnHeader2.Width = 66;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Column";
            this.columnHeader3.Width = 70;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "error.PNG");
            this.imageList1.Images.SetKeyName(1, "warn.png");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 222);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(788, 43);
            this.panel1.TabIndex = 2;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(362, 9);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // FormNewScriptErrorMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(788, 265);
            this.Controls.Add(this.listViewErrorList);
            this.Controls.Add(this.panel1);
            this.Name = "FormNewScriptErrorMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Script Error Message";
            this.Load += new System.EventHandler(this.FormNewScriptErrorMessage_Load);
            this.SizeChanged += new System.EventHandler(this.FormNewScriptErrorMessage_SizeChanged);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormNewScriptErrorMessage_FormClosed);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewErrorList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonClose;
    }
}