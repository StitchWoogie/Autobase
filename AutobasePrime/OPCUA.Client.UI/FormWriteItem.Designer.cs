namespace OPCUA.Client.UI
{
    partial class FormWriteItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWriteItem));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbServer = new System.Windows.Forms.Label();
            this.lbGroup = new System.Windows.Forms.Label();
            this.lbItem = new System.Windows.Forms.Label();
            this.txtWrite = new System.Windows.Forms.TextBox();
            this.numArray = new System.Windows.Forms.NumericUpDown();
            this.lbType = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numArray)).BeginInit();
            this.lbType.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Group :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Item :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Write Value :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Array Position :";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(129, 244);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(279, 244);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbServer
            // 
            this.lbServer.AutoSize = true;
            this.lbServer.Location = new System.Drawing.Point(119, 22);
            this.lbServer.Name = "lbServer";
            this.lbServer.Size = new System.Drawing.Size(38, 12);
            this.lbServer.TabIndex = 2;
            this.lbServer.Text = "label6";
            // 
            // lbGroup
            // 
            this.lbGroup.AutoSize = true;
            this.lbGroup.Location = new System.Drawing.Point(119, 55);
            this.lbGroup.Name = "lbGroup";
            this.lbGroup.Size = new System.Drawing.Size(38, 12);
            this.lbGroup.TabIndex = 2;
            this.lbGroup.Text = "label6";
            // 
            // lbItem
            // 
            this.lbItem.AutoSize = true;
            this.lbItem.Location = new System.Drawing.Point(119, 88);
            this.lbItem.Name = "lbItem";
            this.lbItem.Size = new System.Drawing.Size(38, 12);
            this.lbItem.TabIndex = 2;
            this.lbItem.Text = "label6";
            // 
            // txtWrite
            // 
            this.txtWrite.Location = new System.Drawing.Point(121, 155);
            this.txtWrite.Name = "txtWrite";
            this.txtWrite.Size = new System.Drawing.Size(233, 21);
            this.txtWrite.TabIndex = 3;
            // 
            // numArray
            // 
            this.numArray.Location = new System.Drawing.Point(121, 194);
            this.numArray.Name = "numArray";
            this.numArray.Size = new System.Drawing.Size(120, 21);
            this.numArray.TabIndex = 4;
            // 
            // lbType
            // 
            this.lbType.Controls.Add(this.numArray);
            this.lbType.Controls.Add(this.txtWrite);
            this.lbType.Controls.Add(this.lbItem);
            this.lbType.Controls.Add(this.lbGroup);
            this.lbType.Controls.Add(this.lbServer);
            this.lbType.Controls.Add(this.btnCancel);
            this.lbType.Controls.Add(this.btnOk);
            this.lbType.Controls.Add(this.label5);
            this.lbType.Controls.Add(this.label4);
            this.lbType.Controls.Add(this.label7);
            this.lbType.Controls.Add(this.label6);
            this.lbType.Controls.Add(this.label3);
            this.lbType.Controls.Add(this.label2);
            this.lbType.Controls.Add(this.label1);
            this.lbType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbType.Location = new System.Drawing.Point(0, 0);
            this.lbType.Name = "lbType";
            this.lbType.Size = new System.Drawing.Size(413, 301);
            this.lbType.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(119, 121);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "type";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 121);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "Type :";
            // 
            // FormWriteItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 301);
            this.Controls.Add(this.lbType);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormWriteItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Write Item";
            ((System.ComponentModel.ISupportInitialize)(this.numArray)).EndInit();
            this.lbType.ResumeLayout(false);
            this.lbType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbServer;
        private System.Windows.Forms.Label lbGroup;
        private System.Windows.Forms.Label lbItem;
        private System.Windows.Forms.TextBox txtWrite;
        private System.Windows.Forms.NumericUpDown numArray;
        private System.Windows.Forms.Panel lbType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
    }
}