using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using System.IO;
using NetTools;
using GraphicModule;

namespace Studio.Library
{
    public partial class FormSelectControlBoxFromLibrary : Form
    {
        FormLibraryGroupPreview formPreview;

        public FormSelectControlBoxFromLibrary()
        {
            InitializeComponent();
        }

        WebLibraryPublic weblibrary = new WebLibraryModFull();

        private void FormSelectControlBoxFromLibrary_Load(object sender, EventArgs e)
        {
            formPreview = new FormLibraryGroupPreview(this.weblibrary, this.Modal ? false : true);
            formPreview.TopLevel = false;
            formPreview.Dock = DockStyle.Fill;
            formPreview.PreviewPanelSize = 200;

            panel1.Controls.Add(formPreview);
            formPreview.Show();
            formPreview.procOnDoubleClick = new FormLibraryGroupPreview.DoubleClickCommand(OnDoubleClickOnPreview);

            ReLoadPreview();
        }

        void ReLoadPreview()
        {

            string directory = TotalConfig.GetLibraryFolderBasic();

            directory += "\\ControlBox";

            formPreview.SetGroupLocal(weblibrary, directory);
        }

        private void OnDoubleClickOnPreview()
        {
            InsertGo();
        }

        public string sFileName;

        string MakeNewName(string templete_name, byte[] temp)
        {
            string filename;

            int no = 1;

            filename = String.Format("{0}\\graphic\\{1}.modx", TotalConfig.sDirWorkProject, templete_name);

            while (true)
            {
                if (!File.Exists(filename))
                {
                    File.WriteAllBytes(filename, temp);
                    break;
                }

                if (Tools.CompareFile(temp, filename)) break;  // 같은 파일이다.

                filename = String.Format("{0}\\graphic\\{1}_{2:00}.modx", TotalConfig.sDirWorkProject, templete_name, no++);
            }

            return filename;
        }

        void InsertGo()
        {
            if (!formPreview.IsSelected())
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("삽입할 요소를 선택하세요.", "선택오류");
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请选择要插入的对象。", "选择错误");
                else
                    MessageBox.Show("Select object to insert", "Selection error");

                return;
            }

            string insert_name = formPreview.GetSelectedName();
            object obj = formPreview.InsertSelection(); // 이것을 실행해야 부가적인 파일들이 복사된다.

            string temp_filename = Path.GetTempFileName();
            MemoryStream stream = new MemoryStream();
            ((ObjectRoot)obj).ObjectSaveToStream(stream);

            sFileName = MakeNewName(insert_name, stream.ToArray());

            DialogResult = DialogResult.OK;
            Close();

            /*
            if (weblibrary.GetType() == typeof(WebLibraryModCut))
            {
                Form child = SharedStudio.formMain.ActiveMdiChild;
                FormEditGraphicFrame form = (FormEditGraphicFrame)child;

                form.InsertLibrary_FromInsertClick((ObjectGroup)obj);
            }
            else if (weblibrary.GetType() == typeof(WebLibraryModFull))
            {
                SharedStudio.formMain.FileNewGraphicByTemplete((ObjectRoot)obj, formPreview.GetSelectedName());
            }

            weblibrary.nTempInsertType = GetLibraryType();
            weblibrary.nTempInsertTheme = this.listBoxTheme.SelectedIndex;
            weblibrary.nTempInsertGroup = this.listBoxGroup.SelectedIndex;
            weblibrary.sTempInsertItem = formPreview.GetSelectedName();

            weblibrary.nTempPreviewSplitter = formPreview.PreviewPanelSize;

            if (this.Modal)
            {
                //weblibrary.nTempInsertType = GetLibraryType();
                //weblibrary.nTempInsertTheme = this.listBoxTheme.SelectedIndex;
                //weblibrary.nTempInsertGroup = this.listBoxGroup.SelectedIndex;
                //weblibrary.sTempInsertItem = formPreview.GetSelectedName();
                Close();
            }*/
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            InsertGo();
        }
    }
}
