using System.Drawing;
using System.Windows.Forms;

namespace Studio.Script
{
    partial class FormScriptFind
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
            this.textBoxFind = new System.Windows.Forms.TextBox();
            this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.buttonFindPrevious = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelFind = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxFind
            // 
            this.textBoxFind.Location = new System.Drawing.Point(78, 12);
            this.textBoxFind.Name = "textBoxFind";
            this.textBoxFind.Size = new System.Drawing.Size(200, 21);
            this.textBoxFind.TabIndex = 1;
            this.textBoxFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFind_KeyDown);
            // 
            // checkBoxMatchCase
            // 
            this.checkBoxMatchCase.Location = new System.Drawing.Point(78, 39);
            this.checkBoxMatchCase.Name = "checkBoxMatchCase";
            this.checkBoxMatchCase.Size = new System.Drawing.Size(125, 31);
            this.checkBoxMatchCase.TabIndex = 2;
            this.checkBoxMatchCase.Text = "대/소문자 구분";
            this.checkBoxMatchCase.UseVisualStyleBackColor = true;
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.Location = new System.Drawing.Point(288, 10);
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.Size = new System.Drawing.Size(75, 23);
            this.buttonFindNext.TabIndex = 3;
            this.buttonFindNext.Text = "다음 찾기";
            this.buttonFindNext.UseVisualStyleBackColor = true;
            this.buttonFindNext.Click += new System.EventHandler(this.ButtonFindNext_Click);
            // 
            // buttonFindPrevious
            // 
            this.buttonFindPrevious.Location = new System.Drawing.Point(288, 39);
            this.buttonFindPrevious.Name = "buttonFindPrevious";
            this.buttonFindPrevious.Size = new System.Drawing.Size(75, 23);
            this.buttonFindPrevious.TabIndex = 4;
            this.buttonFindPrevious.Text = "이전 찾기";
            this.buttonFindPrevious.UseVisualStyleBackColor = true;
            this.buttonFindPrevious.Click += new System.EventHandler(this.ButtonFindPrevious_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(288, 68);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "닫기";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // labelFind
            // 
            this.labelFind.Location = new System.Drawing.Point(6, 15);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(71, 22);
            this.labelFind.TabIndex = 0;
            this.labelFind.Text = "찾을 내용:";
            // 
            // FormScriptFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 105);
            this.Controls.Add(this.labelFind);
            this.Controls.Add(this.textBoxFind);
            this.Controls.Add(this.checkBoxMatchCase);
            this.Controls.Add(this.buttonFindNext);
            this.Controls.Add(this.buttonFindPrevious);
            this.Controls.Add(this.buttonClose);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormScriptFind";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "찾기";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
    }
}