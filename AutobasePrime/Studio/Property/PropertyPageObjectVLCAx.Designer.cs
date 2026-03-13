namespace Studio
{
    partial class PropertyPageObjectVLCAx
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectVLCAx));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_mrl = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_options = new System.Windows.Forms.TextBox();
            this.checkBox_autoplay = new System.Windows.Forms.CheckBox();
            this.checkBox_autoloop = new System.Windows.Forms.CheckBox();
            this.numericUpDown_volume = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_volume)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_mrl);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBox_mrl
            // 
            resources.ApplyResources(this.textBox_mrl, "textBox_mrl");
            this.textBox_mrl.Name = "textBox_mrl";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_options);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBox_options
            // 
            resources.ApplyResources(this.textBox_options, "textBox_options");
            this.textBox_options.Name = "textBox_options";
            // 
            // checkBox_autoplay
            // 
            resources.ApplyResources(this.checkBox_autoplay, "checkBox_autoplay");
            this.checkBox_autoplay.Name = "checkBox_autoplay";
            this.checkBox_autoplay.UseVisualStyleBackColor = true;
            // 
            // checkBox_autoloop
            // 
            resources.ApplyResources(this.checkBox_autoloop, "checkBox_autoloop");
            this.checkBox_autoloop.Name = "checkBox_autoloop";
            this.checkBox_autoloop.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_volume
            // 
            this.numericUpDown_volume.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.numericUpDown_volume, "numericUpDown_volume");
            this.numericUpDown_volume.Name = "numericUpDown_volume";
            this.numericUpDown_volume.Tag = "";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // PropertyPageObjectVLCAx
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown_volume);
            this.Controls.Add(this.checkBox_autoloop);
            this.Controls.Add(this.checkBox_autoplay);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PropertyPageObjectVLCAx";
            this.Load += new System.EventHandler(this.PropertyPageObjectVLCAx_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_volume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_mrl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_options;
        private System.Windows.Forms.CheckBox checkBox_autoplay;
        private System.Windows.Forms.CheckBox checkBox_autoloop;
        private System.Windows.Forms.NumericUpDown numericUpDown_volume;
        private System.Windows.Forms.Label label1;
    }
}