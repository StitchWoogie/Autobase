using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScriptLibEdit.Editor;
using AutoLibLocal;
using System.IO;
using NetTools;
using ScriptLibEdit;
using ScriptLibRun;
using Studio.Solution;
using GraphicModule;

namespace Studio.Script
{
    public partial class FormNewScriptEditor : Form
    {
        StatusBar statusBarMain;
        //public string sFilename;
        public UserControlScriptEditor formChild;

        public static string MakeSampleCode(string filename)
        {
            const string sSampleCode = "class {0}\n{{\n\tpublic string HelloWorld()\n\t{{\n\t\treturn \"Hello World!\";\n\t}}\n}}";

            string s = String.Format(sSampleCode, Path.GetFileNameWithoutExtension(filename));

            return s;
        }

        public FormNewScriptEditor(string filename, int new_flag, StatusBar status_bar, CallBackOnMdiActivated callbackmdi)
        {
            InitializeComponent();

            string path;

            if (Path.IsPathRooted(filename)) path = filename;
            else path = String.Format("{0}\\Script\\{1}", TotalConfig.sDirWorkProject, filename);

            statusBarMain = status_bar;

            lpfnCallOnMdiActivated = callbackmdi;

            formChild = new UserControlScriptEditor();

            formChild.procCallBackOnEvent = new UserControlScriptEditor.CallBackOnEvent(CallBackOnEvent);

            //formChild.FormBorderStyle = FormBorderStyle.None;
            //formChild.TopLevel = false;
            formChild.Dock = DockStyle.Fill;
            this.Controls.Add(formChild);
            formChild.Show();

            if (new_flag == 1)
            {
                formChild.panelEditor.LoadFromString(MakeSampleCode(filename));
            }
            else
            {
                formChild.panelEditor.LoadFromFile(path);
            }
        }

        void CallBackOnEvent(EnumEditorEventType type, params object[] param)
        {
            if (type == EnumEditorEventType.CursorPositionChanged)
            {
                statusBarPanelColNum.Text = "Col " + (formChild.panelEditor.ViewCursorX + 1).ToString();
                statusBarPanelRowNum.Text = "Ln " + (formChild.panelEditor.ViewCursorY + 1).ToString();
            }
            else if (type == EnumEditorEventType.InsertModeChanged)
            {
                statusBarPanelInsertMode.Text = formChild.panelEditor.bInsertMode ? "INS" : "OVR";
            }
            else if (type == EnumEditorEventType.SourceModified)
            {
                SetTitle(); // 타이틀에 있는 * 을 표시/비표시 한다.
            }
        }

        public void SetTitle()
        {
            string title;

            string dir = TotalConfig.sDirWorkProject + "\\Script";
            // 작업폴더 밑에 있는 파일이다.
            if (String.Compare(formChild.panelEditor.sFilename, 0, dir, 0, dir.Length, true) == 0)
                title = formChild.panelEditor.sFilename.Substring(dir.Length + 1);
            else
                title = formChild.panelEditor.sFilename;

            if (formChild.panelEditor.bChangeFlag)
            {
                title += " *";
            }

            //title += String.Format(", {0}%", workThis.obj.nOpticRate);

            this.Text = title;
        }

        private void FormNewScriptEditor_Load(object sender, EventArgs e)
        {
            SetTitle();
        }

        public delegate void CallBackOnMdiActivated(Form form, string filename, object obj);
        public CallBackOnMdiActivated lpfnCallOnMdiActivated;

        StatusBarPanel statusBarPanelMessage = new StatusBarPanel();
        StatusBarPanel statusBarPanelInsertMode = new StatusBarPanel();
        StatusBarPanel statusBarPanelRowNum = new StatusBarPanel();
        StatusBarPanel statusBarPanelColNum = new StatusBarPanel();

        private void FormNewScriptEditor_Activated(object sender, EventArgs e)
        {
            statusBarMain.Panels.Clear();

            statusBarPanelMessage.AutoSize = StatusBarPanelAutoSize.Spring;
            this.statusBarMain.Panels.Add(statusBarPanelMessage);

            this.statusBarMain.Panels.Add(statusBarPanelRowNum);

            this.statusBarMain.Panels.Add(statusBarPanelColNum);

            statusBarPanelInsertMode.Width = 50;
            this.statusBarMain.Panels.Add(statusBarPanelInsertMode);

            this.lpfnCallOnMdiActivated(this, formChild.panelEditor.sFilename, null);

            SharedStudio.formMain.ShowMessagePanel(true); 
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSave();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileSaveAs(formChild.panelEditor.sFilename);
        }

        bool FileSave(string filename)
        {
            // 먼저 폴더가 존재하는지 검사한다.
            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            BackUp.BackUpFile(filename);

            if (!formChild.panelEditor.Save(filename))
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("파일을 저장할 수 없습니다.", filename);
                else
                    MessageBox.Show("Can't save the file.", filename);

                return false;
            }
            else
            {
                formChild.panelEditor.bChangeFlag = false;
                formChild.panelEditor.sFilename = filename;
                SetTitle(); // title을 다시 그린다.
            }

            return true;
        }

        public bool FileSave()
        {
            string name = Path.GetFileNameWithoutExtension(formChild.panelEditor.sFilename);

            if (String.Compare(name, 0, "NonameClass", 0, 11, true) == 0)
            {
                return FileSaveAs(formChild.panelEditor.sFilename);
            }
            else
            {
                return FileSave(formChild.panelEditor.sFilename);
            }
        }

        private void menuItemFileSave_Click(object sender, System.EventArgs e)
        {
            FileSave();
        }

        bool FileSaveAs(string init_file)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Script files (*.cs)|*.cs";
            string init = TotalConfig.sDirWorkProject + "\\Script";

            if (!Directory.Exists(init))
            {
                try
                {
                    Directory.CreateDirectory(init);
                }
                catch
                {
                    return false;
                }
            }
            dialog.InitialDirectory = init;

            if (init_file != null)
            {
                dialog.FileName = init_file;
            }

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                bool retn = FileSave(dialog.FileName);
                if (retn)
                {
                    SharedStudio.formSolution.ReLoad();
                }
                return retn;
            }

            return false;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formChild.panelEditor.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formChild.panelEditor.Redo();
        }

        private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            this.undoToolStripMenuItem.Enabled = formChild.panelEditor.IsPosibleUndo();
            this.redoToolStripMenuItem.Enabled = formChild.panelEditor.IsPosibleRedo();
            this.copyToolStripMenuItem.Enabled = formChild.panelEditor.IsPosibleCopy();
            this.pasteToolStripMenuItem.Enabled = formChild.panelEditor.IsPosiblePaste();
        }

        void RecurseEnableAllSubMenuItems(ToolStripMenuItem item)
        {
            ToolStripItem tsi;
            for (int i = 0; i < item.DropDownItems.Count; i++)
            {
                tsi = item.DropDownItems[i];
                if (tsi.GetType() == typeof(ToolStripMenuItem))
                {
                    tsi.Enabled = true;
                    RecurseEnableAllSubMenuItems((ToolStripMenuItem)tsi);
                }
            }
        }

        private void editToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            RecurseEnableAllSubMenuItems(this.editToolStripMenuItem);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formChild.panelEditor.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formChild.panelEditor.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formChild.panelEditor.KeyDownDelete();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formChild.panelEditor.SelectAll();
        }

        private void toolStripMenuItemEditCut_Click(object sender, EventArgs e)
        {
            formChild.panelEditor.EditCut();
        }

        private void FormNewScriptEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.formChild.panelEditor.bChangeFlag)
            {
                DialogResult result;

                if (Tools.IsLangKorean())
                    result = MessageBox.Show(Path.GetFileName(formChild.panelEditor.sFilename) + " 파일을 저장하지 않았습니다.\n파일을 저장할까요?", formChild.panelEditor.sFilename, MessageBoxButtons.YesNoCancel);
                else if (Tools.IsLangChinese())
                    result = MessageBox.Show(Path.GetFileName(formChild.panelEditor.sFilename) + " 文件还没保存。\n想保存文件吗？", formChild.panelEditor.sFilename, MessageBoxButtons.YesNoCancel);
                else
                    result = MessageBox.Show(Path.GetFileName(formChild.panelEditor.sFilename) + " File not saved.\nSave to file?", formChild.panelEditor.sFilename, MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.No) return;
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (!FileSave())
                {
                    e.Cancel = true;
                    return;
                }

            }
        }

        private void startDebuggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!formChild.panelEditor.IsDebugStarted())
            {
                if (!Build()) return;

                if (SharedStudio.formMain.StartLocalMain("", 1))
                {
                    //TextArea.DebugStart();
                    UserControlScriptEditor.DebugStart(); //20260303 PSU 수정
                    //formChild.panelEditor.DebugStart();
                }
            }
            else
            {
                formChild.panelEditor.DebugStepContinue();
            }
        }

        private void stopDebuggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StudioMain.ShutDownProcess("LocalMain");
            formChild.panelEditor.DebugStop();
        }

        private void stepIntoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!formChild.panelEditor.IsDebugStarted())
            {
                if (!Build()) return;

                if (SharedStudio.formMain.StartLocalMain("", 2))
                {
                    //TextArea.DebugStart();
                    UserControlScriptEditor.DebugStart(); //20260303 PSU 수정
                    //formChild.panelEditor.DebugStart();
                }
            }
            else
            {
                formChild.panelEditor.DebugStepInto();
            }
        }

        private void stepOverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!formChild.panelEditor.IsDebugStarted())
            {
                if (!Build()) return;

                if (SharedStudio.formMain.StartLocalMain("", 2))
                {
                    //TextArea.DebugStart();
                    UserControlScriptEditor.DebugStart(); //20260303 PSU 수정
                    //formChild.panelEditor.DebugStart();
                }
            }
            else
            {
                formChild.panelEditor.DebugStepOver();
            }
        }

        private void toggleBreakpointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formChild.panelEditor.SetDebugBreakPointByCursor();
        }

        void CopyAllAssemblyToTarget(string source_dir, string target_dir)
        {
            Directory.CreateDirectory(target_dir);

            DirectoryInfo di = new DirectoryInfo(source_dir);

            string target_file;

            foreach(FileInfo fi in di.GetFiles("*.objx")) {
                target_file = String.Format("{0}\\{1}", target_dir, fi.Name);
                File.Copy(fi.FullName, target_file, true);
            }
        }

        bool CompileProject(EditScriptLibMain script, ScriptProject project)
        {
            string project_name = Path.GetFileNameWithoutExtension(project.sProjectFilename);
            string project_dir = Path.GetDirectoryName(project.sProjectFilename);

            ReferenceClass reference;

            // 참조되어 있는 라이브러리를 먼저 로드한다.

            for (int i = 0; i < project.arrayReference.Count; i++)
            {
                reference = project.arrayReference[i];

                if (reference.type == EnumReferenceType.Project)
                {
                    ScriptProject project2 = ScriptSolution.SeekProjectByName(reference.sReferenceName);
                    if (!CompileProject(script, project2)) return false;

                    string project2_path = Path.GetDirectoryName(project2.sProjectFilename);

                    string project2_bin_dir = String.Format("{0}\\Bin", project2_path);
                    string project_bin_dir = String.Format("{0}\\Bin", project_dir);

                    CopyAllAssemblyToTarget(project2_bin_dir, project_bin_dir);
                }
            }

            string library_file = String.Format("{0}\\bin\\{1}.objx", Path.GetDirectoryName(project.sProjectFilename), Path.GetFileNameWithoutExtension(project.sProjectFilename));

            // 한번 컴파일이 된 경우는 로딩만 한다. 
            if (project.bCompiled)
            {
                script.LoadOneLibrary(library_file);

                return true;
            }

            FormMessagePanel.formThis.MessageOutput("--- Build Started: Project : {0} ---", project_name);

            string filename;
            
            for (int i = 0; i < project.arraySource.Count; i++)
            {
                filename = String.Format("{0}\\{1}", project_dir, project.arraySource[i]);

                script.Split(project_name, project.sDefaultNamespace, filename);

                if (script.bErrorFlag)
                {
                    FormMessagePanel.formThis.MessageOutput("Build Failed: Project : {0}", project_name);
                    return false;
                }
            }

            // 해당 라이브러리의 포인터를 찾는다.
            EditScriptLibLibrary esll = (EditScriptLibLibrary)script.AddLibrary(project_name);

            esll.PrepareBeforeCompile();
            esll.Compile(script);

            if (script.bErrorFlag)
            {
                FormMessagePanel.formThis.MessageOutput("Build Failed: Project : {0}", project_name);
                return false;
            }

            FormMessagePanel.formThis.MessageOutput("Build Succeded: Project : {0}", project_name);

            string target_dir = String.Format("{0}\\Bin", project_dir);
            esll.Save(target_dir);

            project.bCompiled = true;   // 컴파일이 성공적으로 완료되었고 Assembly가 만들어 졌다.

            // 원본을 디컴파일 해보고 컴파일된 파일을 로딩해서 디컴파일 해봐서 다르면 오류를 발생한다.
            // 개발이 끝나면 나중에는 필요없을 듯 하다.
            string decompile1 = esll.MakeDecompiledString();

            ScriptLibMain slm = new ScriptLibMain();
            slm.Prepare();
            ScriptLibLibrary sll = slm.LoadOneLibrary(library_file);
            string decompile2 = sll.MakeDecompiledString();

            if (decompile1 != decompile2)
            {
                FormErrorDecompileCompare dialog = new FormErrorDecompileCompare();

                dialog.Set(decompile1, decompile2);
                dialog.Show();

                FormMessagePanel.formThis.MessageOutput("Build Failed: 디컴파일 오류 : {0}", project_name);

                return false;
            }

            return true;
        }

        bool Build()
        {
            EditScriptLibMain scriptLibMain = new EditScriptLibMain();
            scriptLibMain.Prepare();

            scriptLibMain.scriptExternal = new ScriptExternalRun();
            //ScriptLibMain.scriptMain = scriptLibMain;

            if (!SharedStudio.formMain.SaveAllDocuments()) return false;

            scriptLibMain.arrayError.Clear();
            FormMessagePanel.formThis.FillErrorListView(scriptLibMain);

            statusBarPanelMessage.Text = "Build Solution Started...";
            FormMessagePanel.formThis.MessageOutput(statusBarPanelMessage.Text);

            ScriptProject project;

            // Build All이므로 모든 프로젝트를 다시 컴파일 해주어야 한다.
            for (int i = 0; i < ScriptSolution.arrayProjects.Count; i++)
            {
                ScriptSolution.arrayProjects[i].bCompiled = false;
            }

            for (int i = 0; i < ScriptSolution.arrayProjects.Count; i++)
            {
                scriptLibMain.RemoveAllUserClass(); // 각 프로젝트마다 라이브러리를 깨끗이 비우고 다시 채워야 한다.

                project = ScriptSolution.arrayProjects[i];

                if (!CompileProject(scriptLibMain, project))
                {
                    FormMessagePanel.formThis.FillErrorListView(scriptLibMain);
                    statusBarPanelMessage.Text = "Build Failed";
                    return false;
                }
            }

            statusBarPanelMessage.Text = "Build Succeded";

            string target_bin_dir = String.Format("{0}\\Bin", TotalConfig.sDirWorkProject);

            Directory.CreateDirectory(target_bin_dir);

            CopyBreakPoints(target_bin_dir);
            CopyAllProjectsAssembly(target_bin_dir);

            // 아래는 개발시에만 사용하고 개발이 끝나면 필요가 없을 듯 하다.
            // 컴파일이 잘되고 저장이 잘되었는지 다시 읽어서 소스파일을 Bin폴더에 만들어 본다.
            ScriptLibMain slm = new ScriptLibMain();
            slm.Prepare();
            slm.LoadAllLibrary(target_bin_dir);

            return true;
        }

        // 컴파일된 모든 어셈블리를 오토베이스의 Bin폴더에 복사해 준다. 스크립트 프로젝트는 오토베이스 프로젝트와는 틀릴 수 있으므로
        void CopyAllProjectsAssembly(string target_bin_folder)
        {
            ScriptProject project;
            string project_bin_folder;

            for (int i = 0; i < ScriptSolution.arrayProjects.Count; i++)
            {
                project = ScriptSolution.arrayProjects[i];

                project_bin_folder = Path.GetDirectoryName(project.sProjectFilename)+"\\Bin";

                CopyAllAssemblyToTarget(project_bin_folder, target_bin_folder);
            }
        }

        void CopyBreakPoints(string target_dir)
        {
            ScriptProject project;
            string target_file = String.Format("{0}\\MyProject.BreakPoints", target_dir);
            TextWriter writer = new StreamWriter(target_file);
            string project_folder;

            List<int> array;

            for (int i = 0; i < ScriptSolution.arrayProjects.Count; i++)
            {
                project = ScriptSolution.arrayProjects[i];

                project_folder = Path.GetDirectoryName(project.sProjectFilename);

                for (int j = 0; j < project.arraySource.Count; j++)
                {
                    string sourcepath = String.Format("{0}\\{1}", project_folder, project.arraySource[j]);
                    string breakpath = Path.ChangeExtension(sourcepath, "BreakPoints");

                    array = ScriptLibMain.LoadBreakPoints(breakpath);

                    writer.Write("{0},", sourcepath);

                    for (int k = 0; k < array.Count; k++)
                    {
                        writer.Write("{0},", array[k]);
                    }

                    writer.WriteLine();
                }
            }

            writer.Close();
        }

        private void toolStripMenuItemBuildSolution_Click(object sender, EventArgs e)
        {
            // 메인 프로젝트가 하나밖에 없으므로 Build Project는 의미가 없다. 모든 프로젝트를 컴파일(Build Solution)하는 것은 당연하다.
            Build();
        }

        private void toolStripMenuItemBuild_DropDownOpened(object sender, EventArgs e)
        {
            bool debugging = formChild.panelEditor.IsDebugStarted();
            
            toolStripMenuItemBuildSolution.Enabled = !debugging;
            this.stopDebuggingToolStripMenuItem.Enabled = debugging;
        }
        
    }
}
