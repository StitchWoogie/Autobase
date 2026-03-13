using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace ChangeAssemblyVersion
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonChange;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.TextBox textBoxVersion;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListView listViewResult;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonChange = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.textBoxVersion = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listViewResult = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonChange
			// 
			this.buttonChange.Location = new System.Drawing.Point(712, 16);
			this.buttonChange.Name = "buttonChange";
			this.buttonChange.TabIndex = 0;
			this.buttonChange.Text = "Change";
			this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(712, 48);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// textBoxVersion
			// 
			this.textBoxVersion.Location = new System.Drawing.Point(8, 24);
			this.textBoxVersion.Name = "textBoxVersion";
			this.textBoxVersion.Size = new System.Drawing.Size(328, 21);
			this.textBoxVersion.TabIndex = 2;
			this.textBoxVersion.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBoxVersion);
			this.groupBox1.Location = new System.Drawing.Point(16, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(344, 56);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Last Version";
			// 
			// listViewResult
			// 
			this.listViewResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							 this.columnHeader1,
																							 this.columnHeader2,
																							 this.columnHeader3});
			this.listViewResult.Location = new System.Drawing.Point(16, 96);
			this.listViewResult.Name = "listViewResult";
			this.listViewResult.Size = new System.Drawing.Size(768, 384);
			this.listViewResult.TabIndex = 4;
			this.listViewResult.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Result";
			this.columnHeader1.Width = 516;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Old Version";
			this.columnHeader2.Width = 125;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "New version";
			this.columnHeader3.Width = 121;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 72);
			this.label1.Name = "label1";
			this.label1.TabIndex = 5;
			this.label1.Text = "Result";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Form1
			// 
			this.AcceptButton = this.buttonChange;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(800, 485);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listViewResult);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.buttonChange);
			this.Name = "Form1";
			this.Text = "Change Version (AUTOBASE,NetTools)";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		void DisplayResult(string filename, string old, string ch)
		{
			ListViewItem item = new ListViewItem(filename);
			item.SubItems.Add(old);
			item.SubItems.Add(ch);
			this.listViewResult.Items.Add(item);
		}

		void BackupFile(string filename)
		{
			string path = Path.GetDirectoryName(filename);
		}

		bool ChangeVersion(DateTime t, string filename)
		{
			TextReader reader = new StreamReader(filename);

			if(reader == null) 
			{
				DisplayResult(filename, "Error", "Can't open");
				return false;
			}

			string temp_file = Application.UserAppDataPath+"\\AssemblyInfo_Temp.cs";

			TextWriter writer = new StreamWriter(temp_file);

			if(writer == null) 
			{
				reader.Close();
				DisplayResult(temp_file, "Error", "Can't open temp file");
				return false;
			}

			CommaBlockString comma = new CommaBlockString();
			//[assembly: AssemblyVersion("1.0.*")]
			string one_line;
			bool bversion = false;
			
			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)		break;	// end of line
				if(one_line.Length == 0) 
				{
					writer.WriteLine(one_line);
					continue;
				}
				if(String.Compare(one_line, 0, "[assembly: AssemblyVersion(\"", 0, 28) == 0) 
				{
					if(bversion) 
					{
						DisplayResult(filename, "Error", "Version multi defined.");
						reader.Close();
						writer.Close();
						return false;
					}
					comma.Set(one_line.Substring(28));
					comma.SetBlockCode('\"');
					string old_ver="";
					comma.GetString(ref old_ver);
					DisplayResult(filename, old_ver, this.textBoxVersion.Text);
					bversion = true;
					writer.WriteLine("[assembly: AssemblyVersion(\"{0}\")]", this.textBoxVersion.Text);
				}
				else 
				{
					writer.WriteLine(one_line);
				}
			}

			reader.Close();
			writer.Close();

			if(!bversion) 
			{
				DisplayResult(filename, "Error", "Version field not found");
				return false;
			}

			string datetime = String.Format("{0:0000}-{1:00}-{2:00} {3:00}.{4:00}.{5:00}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
			string backpath = "C:\\Backup AssemblyInfo\\"+datetime+Path.GetDirectoryName(filename).Substring(2);
			if(!Directory.Exists(backpath)) 
			{
				Directory.CreateDirectory(backpath);
			}

			string backfile = backpath+"\\AssemblyInfo.cs";
			if(File.Exists(backfile)) 
			{
				File.SetAttributes(backfile, FileAttributes.Normal);
			}
			File.Copy(filename, backfile, true);
			
			string target = filename;
			
			if(File.Exists(target))
			{
				File.SetAttributes(target, FileAttributes.Normal);
			}
			FileAttributes attr = File.GetAttributes(target);
			File.Copy(temp_file, target, true);
			File.SetAttributes(target, attr);

			return true;
		}

		bool RecurseChange(DateTime t, string root_path)
		{
			if(!Directory.Exists(root_path)) 
			{
				DisplayResult(root_path, "Error", "Path not found");
				return false;
			}
			DirectoryInfo info = new DirectoryInfo(root_path);

			foreach(DirectoryInfo di in info.GetDirectories("*.*")) 
			{
				if(!RecurseChange(t, di.FullName))	return false;
			}

			foreach(FileInfo fi in info.GetFiles("AssemblyInfo.cs")) 
			{
				if(!ChangeVersion(t, fi.FullName))	return false;
			}			

			return true;
		}

		private void buttonChange_Click(object sender, System.EventArgs e)
		{
			if(this.textBoxVersion.Text.Length == 0) 
			{
				MessageBox.Show("You must input the version", "Error");
				return;
			}

			DateTime t = DateTime.Now;
			
			this.listViewResult.Items.Clear();
			if(!RecurseChange(t, "D:\\Autobase48\\AutoBasePrime")) 
			{
				MessageBox.Show("Can't change the version", "Error");
				return;
			}
			if(!RecurseChange(t, "D:\\Autobase48\\NetTools")) 
			{
				MessageBox.Show("Can't change the version", "Error");
				return;
			}

			MessageBox.Show("Version changing is completed.", "OK");

			SaveConfig();			

		}

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		void LoadConfig()
		{
			string config_file = Application.StartupPath+"\\VersionConfig.txt";

			if(!File.Exists(config_file))	return;

			TextReader reader = new StreamReader(config_file);

			if(reader == null)	return;
			this.textBoxVersion.Text = reader.ReadLine();
			reader.Close();
		}

		void SaveConfig()
		{
			string config_file = Application.StartupPath+"\\VersionConfig.txt";

			TextWriter writer = new StreamWriter(config_file);

			if(writer == null)	return;
			writer.WriteLine(this.textBoxVersion.Text);
			writer.Close();
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			LoadConfig();
		}
	}
}
