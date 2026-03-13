using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;
using System.IO;
using DialogCommon;
using HelpLib;

namespace AniEditLib
{
    public partial class FormAniEditMain : Form
    {
        public FormAniEditMain()
        {
            InitializeComponent();
        }

        string sLastDirGraphicOpen = null;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Animation files (*.ani)|*.ani";

            if (sLastDirGraphicOpen != null)
                dialog.InitialDirectory = sLastDirGraphicOpen;
            else
                dialog.InitialDirectory = TotalConfig.sDirWorkProject + "\\graphic";

            if (Tools.IsLangKorean())
                dialog.Title = "그래픽 모듈 열기";
            else if (Tools.IsLangJapanese())
                dialog.Title = "グラフィック モジュール 開く";
            else if (Tools.IsLangChinese())
                dialog.Title = "打开图形模块";
            else
                dialog.Title = "Open Graphic Module";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                sLastDirGraphicOpen = Path.GetDirectoryName(dialog.FileName);

                OpenModule(dialog.FileName);
            }
        }

        public void OpenModule(string path)
        {
            //string filename = Path.GetFileName(path);
            string filename = path;
            FormAniEdit form;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                if (this.MdiChildren[i].Name == "FormAniEdit")
                {
                    form = (FormAniEdit)this.MdiChildren[i];
                    if (String.Compare(filename, form.sFileName, true) == 0)
                    {
                        //form.Activate();	// ?? form.Focus() 도 확실하지 않음
                        form.Select();

                        if (form.WindowState == FormWindowState.Minimized)
                            form.WindowState = FormWindowState.Normal;
                        return;
                    }
                }
            }

            bool first_flag = (this.MdiChildren.Length == 0);
            form = new FormAniEdit(filename);
            form.MdiParent = this;
            if (first_flag) form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        FormAniEdit GetActiveChild()
        {
            if (this.ActiveMdiChild == null) return null;

            if (this.ActiveMdiChild.GetType() != typeof(FormAniEdit)) return null;

            return (FormAniEdit)this.ActiveMdiChild;
        }

        private void runAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.RunAnimation();
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.Insert();
        }

        private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            bool flag = (GetActiveChild() != null);

            this.insertToolStripMenuItem.Enabled = flag;
            this.deleteToolStripMenuItem.Enabled = flag;
            this.insertFromFileToolStripMenuItem.Enabled = flag;
            this.copyToolStripMenuItem.Enabled = flag;
            this.pasteToolStripMenuItem.Enabled = (flag && FormAniEdit.bitmapCopy != null);
            this.pasteFromClipboardToolStripMenuItem.Enabled = (flag && Clipboard.ContainsImage());

            this.propertiesToolStripMenuItem.Enabled = flag;
            this.editBitmapToolStripMenuItem.Enabled = flag;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.Delete();
        }

        private void insertFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.InsertFromFile();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.Paste();
        }

        private void pasteFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.PasteFromClipboard();
        }

        static int nNewModule = 0;

        public void OnFileNew()
        {
            
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNewAnimation dialog = new FormNewAnimation();

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename;

                while (true)
                {
                    filename = String.Format("{0}\\graphic\\noname{1:00}.ani", TotalConfig.sDirWorkProject, nNewModule++);

                    if (!File.Exists(filename)) break;
                }

                bool first_flag = (this.MdiChildren.Length == 0);

                FormAniEdit form;

                form = new FormAniEdit(filename, dialog.nWidth, dialog.nHeight, dialog.nFrame, dialog.nRpm, dialog.nColor);

                form.MdiParent = this;
                if (first_flag) form.WindowState = FormWindowState.Maximized;
                form.Show();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.FileSave();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.FileSaveAs();
        }

        private void fileToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            bool flag = (GetActiveChild() != null);

            this.saveToolStripMenuItem.Enabled = flag;
            this.saveAsToolStripMenuItem.Enabled = flag;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.Property();
        }

        private void editBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null) return;

            child.EditBitmap();
        }

        private void viewToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            bool flag = (GetActiveChild() != null);

            this.runAnimationToolStripMenuItem.Enabled = flag;
        }

        private void bitmapEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigBitmapEditor dialog = new FormConfigBitmapEditor();

            dialog.ShowDialog(this);
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }


        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Close();
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form[] childForm = MdiChildren;
            //Make sure to ask for saving the doc before exiting the app 

            for (int i = 0; i < childForm.Length; i++)
                childForm[i].Close(); 
        }

        private void windowToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            bool flag = (GetActiveChild() != null);

            this.cascadeToolStripMenuItem.Enabled = flag;
            this.tileToolStripMenuItem.Enabled = flag;
            this.aToolStripMenuItem.Enabled = flag;
            this.closeToolStripMenuItem.Enabled = flag;
            this.closeAllToolStripMenuItem.Enabled = flag;
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClassHelp.ShowHelp(this, "AniEdit.chm", "AniEditMain.htm", false);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogCommon.FormAbout dialog = new DialogCommon.FormAbout();

            dialog.ProgramIcon = this.Icon;

            dialog.ShowDialog(this);
        }

        private void FormAniEditMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            AniEditOnStudio.formAniEditor = null;
        }

        public void Insert(Bitmap bitmap)
        {
            FormAniEdit child = GetActiveChild();

            if (child == null)
            {
                string filename;

                while (true)
                {
                    filename = String.Format("{0}\\graphic\\noname{1:00}.ani", TotalConfig.sDirWorkProject, nNewModule++);

                    if (!File.Exists(filename)) break;
                }

                bool first_flag = (this.MdiChildren.Length == 0);

                FormAniEdit form;

                form = new FormAniEdit(filename, bitmap);

                form.MdiParent = this;
                if (first_flag) form.WindowState = FormWindowState.Maximized;
                form.Show();
            }
            else
            {
                child.Add(bitmap);
            }

            /*
            FormNewAnimation dialog = new FormNewAnimation();

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename;

                while (true)
                {
                    filename = String.Format("{0}\\graphic\\noname{1:00}.ani", TotalConfig.sDirWorkProject, nNewModule++);

                    if (!File.Exists(filename)) break;
                }

                bool first_flag = (this.MdiChildren.Length == 0);

                FormAniEdit form;

                form = new FormAniEdit(filename, dialog.nWidth, dialog.nHeight, dialog.nFrame, dialog.nRpm, dialog.nColor);

                form.MdiParent = this;
                if (first_flag) form.WindowState = FormWindowState.Maximized;
                form.Show();
            }*/

        }

        private void toolStripMenuItemImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Animation files (*.gif)|*.gif";

            if (sLastDirGraphicOpen != null)
                dialog.InitialDirectory = sLastDirGraphicOpen;
            else
                dialog.InitialDirectory = TotalConfig.sDirWorkProject + "\\graphic";

            if (Tools.IsLangKorean())
                dialog.Title = "가져오기";
            else
                dialog.Title = "Import";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                sLastDirGraphicOpen = Path.GetDirectoryName(dialog.FileName);

                OpenModule(dialog.FileName);
            }
        }

        private void FormAniEditMain_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        
        
    }
}
