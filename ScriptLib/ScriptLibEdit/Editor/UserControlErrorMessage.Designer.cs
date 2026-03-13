namespace ScriptLibEdit.Editor
{
    partial class UserControlErrorMessage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlErrorMessage));
            this.listViewErrorList = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderFile = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderProject = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listViewErrorList
            // 
            this.listViewErrorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeaderFile,
            this.columnHeaderProject});
            this.listViewErrorList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewErrorList.FullRowSelect = true;
            this.listViewErrorList.HideSelection = false;
            this.listViewErrorList.Location = new System.Drawing.Point(0, 0);
            this.listViewErrorList.MultiSelect = false;
            this.listViewErrorList.Name = "listViewErrorList";
            this.listViewErrorList.Size = new System.Drawing.Size(762, 201);
            this.listViewErrorList.SmallImageList = this.imageList1;
            this.listViewErrorList.TabIndex = 1;
            this.listViewErrorList.UseCompatibleStateImageBehavior = false;
            this.listViewErrorList.View = System.Windows.Forms.View.Details;
            this.listViewErrorList.DoubleClick += new System.EventHandler(this.listViewErrorList_DoubleClick);
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
            // columnHeaderFile
            // 
            this.columnHeaderFile.Text = "File";
            this.columnHeaderFile.Width = 152;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Line";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Column";
            // 
            // columnHeaderProject
            // 
            this.columnHeaderProject.Text = "Project";
            this.columnHeaderProject.Width = 150;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "error.PNG");
            this.imageList1.Images.SetKeyName(1, "warn.png");
            // 
            // UserControlErrorMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewErrorList);
            this.Name = "UserControlErrorMessage";
            this.Size = new System.Drawing.Size(762, 201);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewErrorList;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeaderFile;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeaderProject;
        private System.Windows.Forms.ImageList imageList1;
    }
}
