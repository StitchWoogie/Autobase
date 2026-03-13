namespace Studio.Script
{
    partial class FormMessagePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessagePanel));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listViewErrorList = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.listViewLocals = new System.Windows.Forms.ListView();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(555, 201);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listViewErrorList);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(547, 175);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Error List";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listViewErrorList
            // 
            this.listViewErrorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader8});
            this.listViewErrorList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewErrorList.FullRowSelect = true;
            this.listViewErrorList.HideSelection = false;
            this.listViewErrorList.Location = new System.Drawing.Point(3, 3);
            this.listViewErrorList.MultiSelect = false;
            this.listViewErrorList.Name = "listViewErrorList";
            this.listViewErrorList.Size = new System.Drawing.Size(541, 169);
            this.listViewErrorList.SmallImageList = this.imageList1;
            this.listViewErrorList.TabIndex = 0;
            this.listViewErrorList.UseCompatibleStateImageBehavior = false;
            this.listViewErrorList.View = System.Windows.Forms.View.Details;
            this.listViewErrorList.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listViewErrorList_ColumnWidthChanged);
            this.listViewErrorList.SelectedIndexChanged += new System.EventHandler(this.listViewErrorList_SelectedIndexChanged);
            this.listViewErrorList.DoubleClick += new System.EventHandler(this.listViewErrorList_DoubleClick);
            this.listViewErrorList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewErrorList_ColumnClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "";
            this.columnHeader5.Width = 20;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Description";
            this.columnHeader1.Width = 255;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "File";
            this.columnHeader2.Width = 152;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Line";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Column";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Project";
            this.columnHeader8.Width = 150;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "error.PNG");
            this.imageList1.Images.SetKeyName(1, "warn.png");
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBoxOutput);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(547, 175);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Output";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxOutput.Location = new System.Drawing.Point(3, 3);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOutput.Size = new System.Drawing.Size(541, 169);
            this.textBoxOutput.TabIndex = 0;
            this.textBoxOutput.TextChanged += new System.EventHandler(this.textBoxOutput_TextChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.listViewLocals);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(547, 175);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Locals";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // listViewLocals
            // 
            this.listViewLocals.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7});
            this.listViewLocals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewLocals.Location = new System.Drawing.Point(0, 0);
            this.listViewLocals.Name = "listViewLocals";
            this.listViewLocals.Size = new System.Drawing.Size(547, 175);
            this.listViewLocals.TabIndex = 0;
            this.listViewLocals.UseCompatibleStateImageBehavior = false;
            this.listViewLocals.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Name";
            this.columnHeader6.Width = 151;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Value";
            this.columnHeader7.Width = 365;
            // 
            // FormMessagePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 201);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMessagePanel";
            this.Text = "FormMessagePanel";
            this.Load += new System.EventHandler(this.FormMessagePanel_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMessagePanel_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listViewErrorList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListView listViewLocals;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.ColumnHeader columnHeader8;
    }
}