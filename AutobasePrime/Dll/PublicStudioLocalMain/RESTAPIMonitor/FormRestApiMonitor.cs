using AutoLib;
using AutoLibLocal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutobaseRESTAPIMonitor
{
    public partial class FormRestApiMonitor : Form
    {
        private List<ApiInfo> apiList = new List<ApiInfo>();
        private BindingSource bindingSource = new BindingSource();

        private readonly Action<string, string, string> _statusUpdateHandler;

        public FormRestApiMonitor()
        {
            InitializeComponent();
            GlobalSettings.Initialize();
            InitializeContextMenu();

            bindingSource.DataSource = apiList;
            dataGridViewApiList.DataSource = bindingSource;

            // 스크롤바 관련 설정
            dataGridViewApiList.ScrollBars = ScrollBars.Both;
            dataGridViewApiList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridViewApiList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridViewApiList.ReadOnly = true;
            dataGridViewApiList.AllowUserToAddRows = false;
            dataGridViewApiList.AllowUserToOrderColumns = false;
            dataGridViewApiList.AutoGenerateColumns = false;
            dataGridViewApiList.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewApiList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewApiList.HorizontalScrollingOffset = 0;

            int scrollWidth = SystemInformation.VerticalScrollBarWidth;
            dataGridViewApiList.Padding = new Padding(0, 0, scrollWidth, 0);

            int totalWidth = dataGridViewApiList.Width - SystemInformation.VerticalScrollBarWidth;
            dataGridViewApiList.Columns["Title"].Width = (int)(totalWidth * 0.15);
            dataGridViewApiList.Columns["Url"].Width = (int)(totalWidth * 0.40);
            dataGridViewApiList.Columns["IntervalSeconds"].Width = (int)(totalWidth * 0.11);
            dataGridViewApiList.Columns["Status"].Width = (int)(totalWidth * 0.14);
            dataGridViewApiList.Columns["LastUpdate"].Width = (int)(totalWidth * 0.20);

            dataGridViewApiList.RowHeadersWidth = 30;

            if (GlobalSettings.IsKorean)
            {
                dataGridViewApiList.Columns["Title"].HeaderText = "제목";
                dataGridViewApiList.Columns["Url"].HeaderText = "URL";
                dataGridViewApiList.Columns["IntervalSeconds"].HeaderText = "주기(초)";
                dataGridViewApiList.Columns["Status"].HeaderText = "상태";
                dataGridViewApiList.Columns["LastUpdate"].HeaderText = "마지막 업데이트";
            }
            else
            {
                dataGridViewApiList.Columns["Title"].HeaderText = "Title";
                dataGridViewApiList.Columns["Url"].HeaderText = "URL";
                dataGridViewApiList.Columns["IntervalSeconds"].HeaderText = "Interval(sec)";
                dataGridViewApiList.Columns["Status"].HeaderText = "Status";
                dataGridViewApiList.Columns["LastUpdate"].HeaderText = "Last Update";
            }

            this.Resize += (sender, e) =>
            {
                try
                {
                    int newTotalWidth = dataGridViewApiList.Width - SystemInformation.VerticalScrollBarWidth;
                    dataGridViewApiList.Columns["Title"].Width = (int)(newTotalWidth * 0.15);
                    dataGridViewApiList.Columns["Url"].Width = (int)(newTotalWidth * 0.40);
                    dataGridViewApiList.Columns["IntervalSeconds"].Width = (int)(newTotalWidth * 0.11);
                    dataGridViewApiList.Columns["Status"].Width = (int)(newTotalWidth * 0.14);
                    dataGridViewApiList.Columns["LastUpdate"].Width = (int)(newTotalWidth * 0.20);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[FormRestApiMonitor] Resize error: {ex.Message}");
                }
            };

            dataGridViewApiList.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.F3)
                {
                    e.Handled = true;
                }
            };

            _statusUpdateHandler = async (title, lastUpdate, status) =>
            {
                await UpdateApiStatusAsync(title, lastUpdate, status);
            };
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                await InitializeFormAsync();

                // 상태 업데이트 이벤트 구독
                if (RestApiMonitorService.IsExecutionMode)
                {
                    RestApiMonitorService.ClientStatusUpdate += _statusUpdateHandler;
                }
            }
            catch (Exception ex)
            {
                if (GlobalSettings.IsKorean)
                    MessageBox.Show($"폼 초기화 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show($"Error initializing form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task InitializeFormAsync()
        {
            await LoadApiListAsync();
        }

        private async Task LoadApiListAsync()
        {
            await Task.Run(async () =>
            {
                var configs = RestApiMonitorService.LoadAllConfigs();
                await this.InvokeAsync(new Action(() =>
                {
                    bindingSource.SuspendBinding();
                    try
                    {
                        apiList.Clear();
                        foreach (var config in configs)
                        {
                            bool running = RestApiMonitorService.IsClientRunning?.Invoke(config.Title) ?? false;
                            apiList.Add(new ApiInfo
                            {
                                Title = config.Title,
                                Url = config.Url,
                                IntervalSeconds = config.IntervalSeconds,
                                Status = running ? (GlobalSettings.IsKorean ? "실행" : "Running") : (GlobalSettings.IsKorean ? "중지" : "Stopped"),
                                LastUpdate = "-"
                            });
                        }
                        bindingSource.ResetBindings(false);
                    }
                    finally
                    {
                        bindingSource.ResumeBinding();
                    }
                }));
            });
        }

        private async Task UpdateSpecificApi()
        {
            try
            {
                var configs = RestApiMonitorService.LoadAllConfigs();
                var newConfig = configs.LastOrDefault();
                if (newConfig != null)
                {
                    await this.InvokeAsync(new Action(() =>
                    {
                        bool running = RestApiMonitorService.IsClientRunning?.Invoke(newConfig.Title) ?? false;
                        var newApi = new ApiInfo
                        {
                            Title = newConfig.Title,
                            Url = newConfig.Url,
                            IntervalSeconds = newConfig.IntervalSeconds,
                            Status = running ? (GlobalSettings.IsKorean ? "실행" : "Running") : (GlobalSettings.IsKorean ? "중지" : "Stopped"),
                            LastUpdate = "-"
                        };

                        dataGridViewApiList.SuspendLayout();
                        try
                        {
                            bindingSource.Add(newApi);

                            if (dataGridViewApiList.Rows.Count > 0)
                            {
                                dataGridViewApiList.FirstDisplayedScrollingRowIndex = dataGridViewApiList.Rows.Count - 1;
                            }
                            if (newConfig.IsRunning)
                            {
                                RestApiMonitorService.StartClientTask?.Invoke(newConfig);
                            }
                        }
                        finally
                        {
                            dataGridViewApiList.ResumeLayout();
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                await this.InvokeAsync(new Action(() =>
                {
                    if (GlobalSettings.IsKorean)
                        MessageBox.Show($"API 추가 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show($"Error adding API: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private async Task UpdateSpecificApiAsync(string oldTitle, string newTitle)
        {
            try
            {
                var configs = await Task.Run(() => RestApiMonitorService.LoadAllConfigs());

                await this.InvokeAsync(new Action(() =>
                {
                    var existingApi = apiList.FirstOrDefault(x => x.Title == oldTitle);
                    if (existingApi != null)
                    {
                        int index = apiList.IndexOf(existingApi);
                        apiList.RemoveAt(index);

                        var currentConfig = configs.FirstOrDefault(x => x.Title == newTitle);
                        if (currentConfig != null)
                        {
                            bool running = RestApiMonitorService.IsClientRunning?.Invoke(currentConfig.Title) ?? false;
                            var newApi = new ApiInfo
                            {
                                Title = currentConfig.Title,
                                Url = currentConfig.Url,
                                IntervalSeconds = currentConfig.IntervalSeconds,
                                Status = running ? (GlobalSettings.IsKorean ? "실행" : "Running") : (GlobalSettings.IsKorean ? "중지" : "Stopped"),
                                LastUpdate = existingApi.LastUpdate
                            };

                            dataGridViewApiList.SuspendLayout();
                            try
                            {
                                bindingSource.Insert(index, newApi);

                                if (dataGridViewApiList.Rows.Count > 0)
                                {
                                    dataGridViewApiList.FirstDisplayedScrollingRowIndex = index;
                                    dataGridViewApiList.Rows[index].Selected = true;
                                }
                                if (currentConfig.IsRunning)
                                {
                                    RestApiMonitorService.StartClientTask?.Invoke(currentConfig);
                                }
                            }
                            finally
                            {
                                dataGridViewApiList.ResumeLayout();
                            }
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                await this.InvokeAsync(new Action(() =>
                {
                    if (GlobalSettings.IsKorean)
                        MessageBox.Show($"API 업데이트 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show($"Error updating API: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private volatile bool _closing;

        private async Task UpdateApiStatusAsync(string title, string lastUpdate, string status)
        {
            if (_closing) return;
            if (IsDisposed || Disposing) return;
            if (!this.IsHandleCreated) return;
            if (dataGridViewApiList == null || dataGridViewApiList.IsDisposed) return;

            try
            {
                await this.InvokeAsync(new Action(() =>
                {
                    if (_closing) return;
                    if (dataGridViewApiList.IsDisposed) return;
                    if (dataGridViewApiList.RowCount <= 0) return;

                    var api = apiList.FirstOrDefault(x => x.Title == title);
                    if (api == null) return;

                    if (!string.IsNullOrEmpty(lastUpdate))
                        api.LastUpdate = lastUpdate;

                    if (!string.IsNullOrEmpty(status))
                        api.Status = status;

                    int index = apiList.IndexOf(api);
                    if (index < 0) return;

                    bindingSource.ResetItem(index);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormRestApiMonitor] UpdateApiStatusAsync Error: {ex.Message}");
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _closing = true;
        }

        private async void button_Add_Click(object sender, EventArgs e)
        {
            try
            {
                FormApi formApi = new FormApi(null, null, 1);
                formApi.Owner = this;
                if (formApi.ShowDialog() == DialogResult.OK)
                {
                    await UpdateSpecificApi();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormRestApiMonitor] button_Add_Click error: {ex.Message}");
            }
        }

        private async void dataGridViewApiList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < apiList.Count)
                {
                    var selectedApi = apiList[e.RowIndex];
                    FormApi formApi = new FormApi(selectedApi.Title, selectedApi.Url, selectedApi.IntervalSeconds);
                    formApi.Owner = this;
                    if (formApi.ShowDialog() == DialogResult.OK)
                    {
                        await UpdateSpecificApiAsync(selectedApi.Title, formApi.ModifiedTitle);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormRestApiMonitor] CellDoubleClick error: {ex.Message}");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormRestApiMonitor] exitToolStripMenuItem_Click error: {ex.Message}");
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (RestApiMonitorService.IsExecutionMode)
            {
                RestApiMonitorService.ClientStatusUpdate -= _statusUpdateHandler;
            }
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            if (dataGridViewApiList.CurrentRow == null)
            {
                if (GlobalSettings.IsKorean)
                    MessageBox.Show("삭제할 항목을 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Please select the item to delete", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selectedIndex = dataGridViewApiList.CurrentRow.Index;
            if (selectedIndex < 0 || selectedIndex >= apiList.Count)
            {
                if (GlobalSettings.IsKorean)
                    MessageBox.Show("선택된 항목이 유효하지 않습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Invalid selected item.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var selectedApi = apiList[dataGridViewApiList.CurrentRow.Index];

                DialogResult result = MessageBox.Show(
                    GlobalSettings.IsKorean ? $"'{selectedApi.Title}'를 삭제하시겠습니까?" : $"'Are you sure you want to delete {selectedApi.Title}?",
                    GlobalSettings.IsKorean ? "삭제 확인" : "Delete Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    RestApiMonitorService.StopClient?.Invoke(selectedApi.Title);

                    var configs = RestApiMonitorService.LoadAllConfigs();
                    configs.RemoveAll(x => x.Title == selectedApi.Title);
                    RestApiMonitorService.SaveAllConfigs(configs);

                    bindingSource.RemoveAt(selectedIndex);

                    if (GlobalSettings.IsKorean) MessageBox.Show("삭제되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("Deleted complete.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                if (GlobalSettings.IsKorean)
                    MessageBox.Show($"삭제 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show($"Error occurred during deletion: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_Modify_Click(object sender, EventArgs e)
        {
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var helpForm = FormHelp.GetInstance(0);
                if (!helpForm.Visible)
                {
                    helpForm.Show(this);
                }
                helpForm.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormRestApiMonitor] aboutToolStripMenuItem_Click error: {ex.Message}");
            }
        }

        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            dataGridViewApiList.ContextMenuStrip = contextMenu;

            ToolStripMenuItem modifyItem = new ToolStripMenuItem(GlobalSettings.IsKorean ? "수정" : "Modfiy");
            ToolStripMenuItem deleteItem = new ToolStripMenuItem(GlobalSettings.IsKorean ? "삭제" : "Delete");
            ToolStripMenuItem useItem = new ToolStripMenuItem(GlobalSettings.IsKorean ? "사용" : "Use");

            contextMenu.Items.AddRange(new ToolStripItem[] { modifyItem, deleteItem, useItem });

            // 실행 모드가 아닌 경우 사용/중지 메뉴 비활성화
            useItem.Visible = RestApiMonitorService.IsExecutionMode;

            contextMenu.Opening += (sender, e) =>
            {
                try
                {
                    var hit = dataGridViewApiList.HitTest(
                        dataGridViewApiList.PointToClient(Control.MousePosition).X,
                        dataGridViewApiList.PointToClient(Control.MousePosition).Y);

                    if (hit.RowIndex < 0 || hit.RowIndex >= apiList.Count)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _ = UpdateContextMenuAsync(hit.RowIndex, useItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[FormRestApiMonitor] ContextMenu.Opening error: {ex.Message}");
                    e.Cancel = true;
                }
            };

            modifyItem.Click += async (sender, e) =>
            {
                try
                {
                    if (dataGridViewApiList.SelectedRows.Count > 0)
                    {
                        var selectedApi = apiList[dataGridViewApiList.SelectedRows[0].Index];
                        FormApi formApi = new FormApi(selectedApi.Title, selectedApi.Url, selectedApi.IntervalSeconds);
                        if (formApi.ShowDialog() == DialogResult.OK)
                        {
                            await UpdateSpecificApiAsync(selectedApi.Title, formApi.ModifiedTitle);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"[FormRestApiMonitor] modifyItem.Click error: {ex.Message}");
                }
            };

            deleteItem.Click += async (sender, e) =>
            {
                if (dataGridViewApiList.SelectedRows.Count > 0)
                {
                    int selectedIndex = dataGridViewApiList.SelectedRows[0].Index;
                    var selectedApi = apiList[dataGridViewApiList.SelectedRows[0].Index];
                    DialogResult result = MessageBox.Show(
                        GlobalSettings.IsKorean ? $"'{selectedApi.Title}'를 삭제하시겠습니까?" : $"'Are you sure you want to delete {selectedApi.Title}?",
                        GlobalSettings.IsKorean ? "삭제 확인" : "Delete Confirm",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                RestApiMonitorService.StopClient?.Invoke(selectedApi.Title);

                                var configs = RestApiMonitorService.LoadAllConfigs();
                                configs.RemoveAll(x => x.Title == selectedApi.Title);
                                RestApiMonitorService.SaveAllConfigs(configs);
                            });

                            await this.InvokeAsync(new Action(() =>
                            {
                                bindingSource.RemoveAt(selectedIndex);

                                if (GlobalSettings.IsKorean)
                                    MessageBox.Show("삭제되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                else
                                    MessageBox.Show("Deleted complete.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }));
                        }
                        catch (Exception ex)
                        {
                            await this.InvokeAsync(new Action(() =>
                            {
                                if (GlobalSettings.IsKorean)
                                    MessageBox.Show($"삭제 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    MessageBox.Show($"Error during deletion: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }
                    }
                }
            };

            useItem.Click += async (sender, e) =>
            {
                if (!RestApiMonitorService.IsExecutionMode) return;

                if (dataGridViewApiList.SelectedRows.Count > 0)
                {
                    try
                    {
                        var selectedApi = apiList[dataGridViewApiList.SelectedRows[0].Index];
                        var configs = await Task.Run(() => RestApiMonitorService.LoadAllConfigs());
                        var config = configs.FirstOrDefault(x => x.Title == selectedApi.Title);

                        if (config != null)
                        {
                            config.IsRunning = !config.IsRunning;
                            int selectedIndex = dataGridViewApiList.SelectedRows[0].Index;

                            await Task.Run(() =>
                            {
                                if (config.IsRunning)
                                {
                                    RestApiMonitorService.StartClientTask?.Invoke(config);
                                }
                                else
                                {
                                    RestApiMonitorService.StopClient?.Invoke(selectedApi.Title);
                                }

                                RestApiMonitorService.SaveApiClient(config);
                            });

                            await this.InvokeAsync(() =>
                            {
                                if (selectedIndex >= 0 && selectedIndex < apiList.Count)
                                {
                                    apiList[selectedIndex].Status = config.IsRunning ?
                                        (GlobalSettings.IsKorean ? "시작" : "Start") :
                                        (GlobalSettings.IsKorean ? "중지" : "Stop");
                                    bindingSource.ResetItem(selectedIndex);

                                    dataGridViewApiList.ClearSelection();
                                    dataGridViewApiList.Rows[selectedIndex].Selected = true;
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        await this.InvokeAsync(new Action(() =>
                        {
                            if (GlobalSettings.IsKorean)
                                MessageBox.Show($"작업 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else
                                MessageBox.Show($"Error during operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                }
            };
        }

        private async Task UpdateContextMenuAsync(int rowIndex, ToolStripMenuItem useItem)
        {
            try
            {
                await this.InvokeAsync(new Action(() =>
                {
                    foreach (DataGridViewRow row in dataGridViewApiList.Rows)
                    {
                        row.Selected = false;
                    }
                    dataGridViewApiList.Rows[rowIndex].Selected = true;
                    dataGridViewApiList.CurrentCell = dataGridViewApiList.Rows[rowIndex].Cells[0];
                }));

                if (RestApiMonitorService.IsExecutionMode)
                {
                    var configs = await Task.Run(() => RestApiMonitorService.LoadAllConfigs());
                    var selectedApi = apiList[rowIndex];
                    var config = configs.FirstOrDefault(x => x.Title == selectedApi.Title);

                    if (config != null)
                    {
                        await this.InvokeAsync(new Action(() =>
                        {
                            useItem.Text = config.IsRunning ?
                                (GlobalSettings.IsKorean ? "중지" : "Stop") :
                                (GlobalSettings.IsKorean ? "시작" : "Start");
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                await this.InvokeAsync(() =>
                {
                    MessageBox.Show($"[FormRestApiMonitor] Context menu update error: {ex.Message}");
                });
            }
        }
    }
}
