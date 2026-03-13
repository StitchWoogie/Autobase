namespace Studio.Layer
{
    partial class FormLayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLayer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonAddLayer = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddSubLayer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonDeleteObject = new System.Windows.Forms.ToolStripButton();
            this.panelDraw = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemAddLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAddSubLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemLayerProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemLock = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemUnLock = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemShow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemHide = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemUnGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemObjectProerties = new System.Windows.Forms.ToolStripMenuItem();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.toolStripLabelCount = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddLayer,
            this.toolStripButtonAddSubLayer,
            this.toolStripSeparator3,
            this.toolStripButtonDeleteObject,
            this.toolStripLabelCount});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButtonAddLayer
            // 
            resources.ApplyResources(this.toolStripButtonAddLayer, "toolStripButtonAddLayer");
            this.toolStripButtonAddLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddLayer.Name = "toolStripButtonAddLayer";
            this.toolStripButtonAddLayer.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButtonAddSubLayer
            // 
            resources.ApplyResources(this.toolStripButtonAddSubLayer, "toolStripButtonAddSubLayer");
            this.toolStripButtonAddSubLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddSubLayer.Name = "toolStripButtonAddSubLayer";
            this.toolStripButtonAddSubLayer.Click += new System.EventHandler(this.toolStripButtonAddSubLayer_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // toolStripButtonDeleteObject
            // 
            resources.ApplyResources(this.toolStripButtonDeleteObject, "toolStripButtonDeleteObject");
            this.toolStripButtonDeleteObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeleteObject.Name = "toolStripButtonDeleteObject";
            this.toolStripButtonDeleteObject.Click += new System.EventHandler(this.toolStripButtonDeleteLayer_Click);
            // 
            // panelDraw
            // 
            resources.ApplyResources(this.panelDraw, "panelDraw");
            this.panelDraw.BackColor = System.Drawing.Color.White;
            this.panelDraw.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelDraw.ContextMenuStrip = this.contextMenuStrip1;
            this.panelDraw.Name = "panelDraw";
            this.panelDraw.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDraw_Paint);
            this.panelDraw.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelDraw_MouseDoubleClick);
            this.panelDraw.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelDraw_MouseDown);
            this.panelDraw.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelDraw_MouseMove);
            this.panelDraw.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelDraw_MouseUp);
            // 
            // contextMenuStrip1
            // 
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddLayer,
            this.toolStripMenuItemAddSubLayer,
            this.toolStripMenuItemLayerProperties,
            this.toolStripSeparator1,
            this.toolStripMenuItemLock,
            this.toolStripMenuItemUnLock,
            this.toolStripMenuItemShow,
            this.toolStripMenuItemHide,
            this.toolStripSeparator2,
            this.toolStripMenuItemGroup,
            this.toolStripMenuItemUnGroup,
            this.toolStripSeparator4,
            this.toolStripMenuItemObjectProerties});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // toolStripMenuItemAddLayer
            // 
            resources.ApplyResources(this.toolStripMenuItemAddLayer, "toolStripMenuItemAddLayer");
            this.toolStripMenuItemAddLayer.Name = "toolStripMenuItemAddLayer";
            this.toolStripMenuItemAddLayer.Click += new System.EventHandler(this.toolStripMenuItemAddLayer_Click);
            // 
            // toolStripMenuItemAddSubLayer
            // 
            resources.ApplyResources(this.toolStripMenuItemAddSubLayer, "toolStripMenuItemAddSubLayer");
            this.toolStripMenuItemAddSubLayer.Name = "toolStripMenuItemAddSubLayer";
            this.toolStripMenuItemAddSubLayer.Click += new System.EventHandler(this.toolStripMenuItemAddSubLayer_Click);
            // 
            // toolStripMenuItemLayerProperties
            // 
            resources.ApplyResources(this.toolStripMenuItemLayerProperties, "toolStripMenuItemLayerProperties");
            this.toolStripMenuItemLayerProperties.Name = "toolStripMenuItemLayerProperties";
            this.toolStripMenuItemLayerProperties.Click += new System.EventHandler(this.toolStripMenuItemLayerProperties_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // toolStripMenuItemLock
            // 
            resources.ApplyResources(this.toolStripMenuItemLock, "toolStripMenuItemLock");
            this.toolStripMenuItemLock.Name = "toolStripMenuItemLock";
            this.toolStripMenuItemLock.Click += new System.EventHandler(this.toolStripMenuItemLock_Click);
            // 
            // toolStripMenuItemUnLock
            // 
            resources.ApplyResources(this.toolStripMenuItemUnLock, "toolStripMenuItemUnLock");
            this.toolStripMenuItemUnLock.Name = "toolStripMenuItemUnLock";
            this.toolStripMenuItemUnLock.Click += new System.EventHandler(this.toolStripMenuItemUnLock_Click);
            // 
            // toolStripMenuItemShow
            // 
            resources.ApplyResources(this.toolStripMenuItemShow, "toolStripMenuItemShow");
            this.toolStripMenuItemShow.Name = "toolStripMenuItemShow";
            this.toolStripMenuItemShow.Click += new System.EventHandler(this.toolStripMenuItemShow_Click);
            // 
            // toolStripMenuItemHide
            // 
            resources.ApplyResources(this.toolStripMenuItemHide, "toolStripMenuItemHide");
            this.toolStripMenuItemHide.Name = "toolStripMenuItemHide";
            this.toolStripMenuItemHide.Click += new System.EventHandler(this.toolStripMenuItemHide_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripMenuItemGroup
            // 
            resources.ApplyResources(this.toolStripMenuItemGroup, "toolStripMenuItemGroup");
            this.toolStripMenuItemGroup.Name = "toolStripMenuItemGroup";
            this.toolStripMenuItemGroup.Click += new System.EventHandler(this.toolStripMenuItemGroup_Click);
            // 
            // toolStripMenuItemUnGroup
            // 
            resources.ApplyResources(this.toolStripMenuItemUnGroup, "toolStripMenuItemUnGroup");
            this.toolStripMenuItemUnGroup.Name = "toolStripMenuItemUnGroup";
            this.toolStripMenuItemUnGroup.Click += new System.EventHandler(this.toolStripMenuItemUnGroup_Click);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // toolStripMenuItemObjectProerties
            // 
            resources.ApplyResources(this.toolStripMenuItemObjectProerties, "toolStripMenuItemObjectProerties");
            this.toolStripMenuItemObjectProerties.Name = "toolStripMenuItemObjectProerties";
            this.toolStripMenuItemObjectProerties.Click += new System.EventHandler(this.toolStripMenuItemObjectProerties_Click);
            // 
            // vScrollBar1
            // 
            resources.ApplyResources(this.vScrollBar1, "vScrollBar1");
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "eye_off3.png");
            this.imageList1.Images.SetKeyName(1, "Eye_On2.png");
            this.imageList1.Images.SetKeyName(2, "Lock_off2.png");
            this.imageList1.Images.SetKeyName(3, "Lock_on2.png");
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "DirClose.bmp");
            this.imageList2.Images.SetKeyName(1, "DirOpen.bmp");
            this.imageList2.Images.SetKeyName(2, "Group.bmp");
            // 
            // toolStripLabelCount
            // 
            resources.ApplyResources(this.toolStripLabelCount, "toolStripLabelCount");
            this.toolStripLabelCount.Name = "toolStripLabelCount";
            // 
            // FormLayer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelDraw);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLayer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormLayer_FormClosed);
            this.Load += new System.EventHandler(this.FormLayer_Load);
            this.SizeChanged += new System.EventHandler(this.FormLayer_SizeChanged);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddLayer;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddSubLayer;
        private System.Windows.Forms.Panel panelDraw;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteObject;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddLayer;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddSubLayer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLayerProperties;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemObjectProerties;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemLock;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUnLock;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemShow;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHide;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGroup;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemUnGroup;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel toolStripLabelCount;
    }
}