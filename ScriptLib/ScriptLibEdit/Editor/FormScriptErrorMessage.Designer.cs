namespace ScriptLibEdit.Editor
{
    partial class FormScriptErrorMessage
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.userControlErrorMessage1 = new ScriptLibEdit.Editor.UserControlErrorMessage();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 179);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(679, 40);
            this.panel1.TabIndex = 0;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(313, 9);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // userControlErrorMessage1
            // 
            this.userControlErrorMessage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userControlErrorMessage1.Location = new System.Drawing.Point(0, 0);
            this.userControlErrorMessage1.Name = "userControlErrorMessage1";
            this.userControlErrorMessage1.Size = new System.Drawing.Size(679, 179);
            this.userControlErrorMessage1.TabIndex = 1;
            // 
            // FormScriptErrorMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 219);
            this.Controls.Add(this.userControlErrorMessage1);
            this.Controls.Add(this.panel1);
            this.Name = "FormScriptErrorMessage";
            this.Text = "FormErrorMessage";
            this.SizeChanged += new System.EventHandler(this.FormErrorMessage_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonClose;
        private UserControlErrorMessage userControlErrorMessage1;
    }
}