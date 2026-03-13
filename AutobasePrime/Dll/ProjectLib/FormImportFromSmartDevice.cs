using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools;
using System.IO;
using ProjectSelect;

namespace ProjectLib
{
    public partial class FormImportFromSmartDevice : Form
    {
        public FormImportFromSmartDevice()
        {
            InitializeComponent();
        }

        byte[] GetConfigurationData(RemoteApi remote, string root_path)
        {
            string config_file = String.Format("{0}\\Project\\Config\\ProjectConfig.inix", this.textBoxFilename.Text);

            byte[] config_data = remote.ReadAllBytes(config_file);

            return config_data;
        }

        private void FormImportFromSmartDevice_Load(object sender, EventArgs e)
        {
            RemoteApi remote = new RemoteApi();

            if (!remote.Init(true))
            {
                Close();
                return;
            }

            byte[] config_data;

            this.textBoxFilename.Text = "\\Flash Disk\\SmartScada";
            config_data = GetConfigurationData(remote, this.textBoxFilename.Text);

            if (config_data == null)
            {
                this.textBoxFilename.Text = "\\NandFlash\\SmartScada";
                config_data = GetConfigurationData(remote, this.textBoxFilename.Text);

                if (config_data == null)
                {
                    this.textBoxFilename.Text = "\\Nand\\SmartScada";
                    config_data = GetConfigurationData(remote, this.textBoxFilename.Text);

                    if (config_data == null)
                    {
                        this.textBoxFilename.Text = "\\SmartScada";
                        config_data = GetConfigurationData(remote, this.textBoxFilename.Text);

                        if (config_data == null)
                        {
                            if (Tools.IsLangKorean())
                                MessageBox.Show("가져올 프로젝트가 장치에 존재하지 않습니다.", "프로젝트 없음");
                            else
                                MessageBox.Show("Project not exists to import in Smart Device.", "Project not exists");

                            this.buttonOK.Enabled = false;

                            return;
                        }
                    }
                }

            }

            if (config_data != null)
            {
                string ce_deployed_folder = "";
                string project_title = "";

                Profile.GetPrivateProfileString("Compact_Deployment", "Deployed Folder", "", ref ce_deployed_folder, config_data);
                Profile.GetPrivateProfileString("Information", "Title", "", ref project_title, config_data);

                if (ce_deployed_folder.Length == 0)
                    ce_deployed_folder = "C:\\AutoBase\\Project\\SmartProject";
                if (project_title.Length == 0)
                    project_title = "Smart Project";

                this.textBoxDirProject.Text = ce_deployed_folder;
                this.textBoxProjectTitle.Text = project_title;
            }

            remote.Uninit();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (Tools.IsLangKorean())
            {
                if (MessageBox.Show("가져오기를 시작할까요?", "가져오기", MessageBoxButtons.YesNo)
                    != DialogResult.Yes) return;
            }
            else
            {
                if (MessageBox.Show("Import Now?", "Import", MessageBoxButtons.YesNo)
                    != DialogResult.Yes) return;
            }

            if (Directory.Exists(this.textBoxDirProject.Text))
            {
                if (Tools.IsLangKorean())
                {
                    if (MessageBox.Show("프로젝트 폴더가 이미 존재합니다.\n기존 폴더에 덮어쓸까요?", "폴더 존재", MessageBoxButtons.YesNo)
                        != DialogResult.Yes) return;
                }
                else if (Tools.IsLangChinese())
                {
                    if (MessageBox.Show("项目文件夹已经存在。\n是否替换现有的?", "文件夹已存在", MessageBoxButtons.YesNo)
                        != DialogResult.Yes) return;
                }
                else
                {
                    if (MessageBox.Show("Project directory already exists.\nOverwrite to exist directory?", "Directory exists", MessageBoxButtons.YesNo)
                        != DialogResult.Yes) return;
                }
            }

            if (!CheckUsedTitleDir(this.textBoxProjectTitle.Text, this.textBoxDirProject.Text)) return;

            RemoteApi remote = new RemoteApi();
            if (!remote.Init(true)) return;

            string ce_project_folder = this.textBoxFilename.Text+"\\Project";

            List<FileInfoCE> array = remote.GetFilesList(ce_project_folder);

            // FileInfo fi;

            for (int i = 0; i < array.Count; i++)
            {
                this.labelStatus.Text = array[i].FullName;
                this.labelStatus.Update();

                string sub_fullname = array[i].FullName.Substring(ce_project_folder.Length);
                string sub_dir = Path.GetDirectoryName(sub_fullname);
                string filename = String.Format("{0}{1}", this.textBoxDirProject.Text, sub_dir);

                Directory.CreateDirectory(filename);

                filename = String.Format("{0}{1}", this.textBoxDirProject.Text, sub_fullname);

                byte[] data = remote.ReadAllBytes(array[i].FullName);
                File.WriteAllBytes(filename, data);
            }

            remote.Uninit();

            if (Tools.IsLangKorean())
                MessageBox.Show("가져오기가 완료 되었습니다.", "가져오기 완료");
            else
                MessageBox.Show("Import completed.", "Import O.K");

            this.labelStatus.Text = "Import O.K";

            DialogResult = DialogResult.OK;
            Close();
        }

        public ListView listParent;

        public bool bAddFlag = false;
        public PROJECT_STRUCT projectAdd;

        bool CheckUsedTitleDir(string title, string dir)
        {
            ListViewItem lvi;

            for (int i = 0; i < listParent.Items.Count; i++)
            {
                lvi = listParent.Items[i];

                // 같은 이름 같은 폴더는 그대로 사용
                if (String.Compare(lvi.SubItems[0].Text, title, true) == 0 &&
                    String.Compare(lvi.SubItems[1].Text, dir, true) == 0)
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                    bAddFlag = false;
                    return true;
                }
                else if (String.Compare(lvi.SubItems[0].Text, title, true) == 0)
                {
                    if (Tools.IsLangKorean())
                    {
                        MessageBox.Show("같은 제목의 프로젝트가 이미 존재합니다.", "제목중복");
                    }
                    else
                    {
                        MessageBox.Show("Same title is already exist.", "Title error");
                    }
                    return false;
                }
                else if (String.Compare(lvi.SubItems[1].Text, dir, true) == 0)
                {
                    if (Tools.IsLangKorean())
                    {
                        MessageBox.Show("지정한 폴더는 이미 다른 프로젝트에서 사용중입니다.", "폴더오류");
                    }
                    else if (Tools.IsLangChinese())
                    {
                        MessageBox.Show("您指定的文件夹在别的项目中正在使用。", "文件夹");
                    }
                    else
                    {
                        MessageBox.Show("Same folder is already used by another project.", "Folder error");
                    }
                    return false;
                }
            }

            bAddFlag = true;
            projectAdd = new PROJECT_STRUCT();
            projectAdd.title = this.textBoxProjectTitle.Text;
            projectAdd.directory = this.textBoxDirProject.Text;
            projectAdd.nPlatform = 1;// nPlatform;

            return true;
        }

        

        private void buttonDirProject_Click(object sender, EventArgs e)
        {
            string title;

            if (Tools.IsLangKorean())
            {
                title = "작업 폴더 선택";
            }
            else if (Tools.IsLangJapanese())
            {
                title = "プロジェクト フォルダ選択";
            }
            else if (Tools.IsLangChinese())
            {
                title = "选择项目文件夹";
            }
            else
            {
                title = "Select project directory";
            }

            FormDirectorySelect dialog = new FormDirectorySelect();
            dialog.StartPosition = FormStartPosition.CenterParent;

            dialog.FileName = this.textBoxDirProject.Text;
            dialog.Text = title;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxDirProject.Text = dialog.FileName;
            }
        }	
    }
}
