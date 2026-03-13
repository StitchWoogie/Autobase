using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetTools
{
    public partial class FormSelectExcelPath : Form
    {
        /// <summary>
        /// OK 클릭 시 선택된 Excel.exe 경로
        /// </summary>
        public string SelectedExcelPath { get; private set; }
        public FormSelectExcelPath()
        {
            InitializeComponent();
            InitListView();
        }



        private void FormSelectExcelPath_Load(object sender, EventArgs e)
        {
            LoadExcelPaths();
        }

        private void InitListView()
        {
            listViewExcel.Columns.Clear();
            listViewExcel.Columns.Add("Excel.exe Path", 600);
        }

        private void LoadExcelPaths()
        {
            listViewExcel.Items.Clear();

            List<string> paths = ExcelPathFinder.FindExcelExePaths();

            foreach (string path in paths)
            {
                var item = new ListViewItem(path);
                item.Tag = path;   // 반환용
                listViewExcel.Items.Add(item);
            }

            if (listViewExcel.Items.Count > 0)
            {
                listViewExcel.Items[0].Selected = true;
            }
        }


        // =========================
        // OK 버튼
        // =========================
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listViewExcel.SelectedItems.Count == 0)
                return;

            SelectedExcelPath = listViewExcel.SelectedItems[0].Tag as string;

            if (string.IsNullOrEmpty(SelectedExcelPath))
                return;

            DialogResult = DialogResult.OK;
            Close();
        }


        // =========================
        // Cancel 버튼
        // =========================
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // =========================
        // Open 버튼 (폴더 열기)
        // =========================
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (listViewExcel.SelectedItems.Count == 0)
                return;

            string excelPath = listViewExcel.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(excelPath))
                return;

            string folder = Path.GetDirectoryName(excelPath);
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                return;

            // 탐색기에서 해당 폴더 열기
            Process.Start("explorer.exe", folder);
        }

        private void listViewExcel_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSel = listViewExcel.SelectedItems.Count > 0;
            btnOK.Enabled = hasSel;
            btnOpen.Enabled = hasSel;
        }
    }
}
