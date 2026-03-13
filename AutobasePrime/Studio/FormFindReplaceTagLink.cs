using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Collections;
using GraphicModule;
using System.IO;
using AutoLibLocal;
using AutoLib;
using NetTools;
using Studio.Script;
using System.Threading.Tasks;

namespace Studio
{
    public partial class FormFindReplaceTagLink : Form
    {
        Form formParent = null;

        static FormFindReplaceTagLink dialogFind = null;

        public static void Run()
        {
            if (dialogFind == null)
            {
                dialogFind = new FormFindReplaceTagLink();
                dialogFind.Owner = SharedStudio.formMain;

                dialogFind.Show(); //250828 PSU SharedStudio.formMain 제거
            }
        }

        public FormFindReplaceTagLink()
        {
            InitializeComponent();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        ArrayList arrayTags;

        void AddTagListProgrammStartEndScript(string used_position, string when)
        {
            ScriptClass script = new ScriptClass();

            string fullpath = TotalConfig.FileOldNew("control\\StartEnd", when + ".CTL", when + ".CTLX");

            if (!File.Exists(fullpath)) return;

            script.LoadFromFile(fullpath);
            script.GetMultiSelectTagList(arrayTags, fullpath, EnumTagUsedType.String, "ProgramScript", used_position);
        }

        //void AddTagListGraphicModules()
        //{
        //    // Graphic Module
        //    DirectoryInfo di = new DirectoryInfo(TotalConfig.sDirWorkProject + "\\graphic");

        //    foreach (FileInfo fi in di.GetFiles("*.modx"))
        //    {
        //        ObjectRoot root = new ObjectRoot();
        //        Form form = new Form();
        //        root.Load(form, fi.FullName);

        //        root.GetMultiSelectTagList(arrayTags, fi.FullName);
        //    }
        //}

        private static Dictionary<string, (DateTime lastModified, ArrayList tags)> moduleTagCache
    = new Dictionary<string, (DateTime, ArrayList)>();

        void AddTagListGraphicModules()
        {
            DirectoryInfo di = new DirectoryInfo(TotalConfig.sDirWorkProject + "\\graphic");
            FileInfo[] modxFiles = di.GetFiles("*.modx");

            if (modxFiles.Length == 0) return;

            int processedCount = 0;

            foreach (FileInfo fi in modxFiles)
            {
                // 매 파일마다 취소 확인 (오버헤드 무시할 수준)
                if (tagLoadWorker?.CancellationPending == true)
                    break;

                try
                {
                    ArrayList cachedTags = GetCachedModuleTags(fi);
                    if (cachedTags != null)
                    {
                        // 캐시에서 가져온 태그 추가
                        arrayTags.AddRange(cachedTags);
                    }
                    else
                    {
                        // 캐시에 없으면 새로 로드
                        ArrayList newTags = LoadModuleTags(fi);
                        arrayTags.AddRange(newTags);

                        // 캐시에 저장
                        moduleTagCache[fi.FullName] = (fi.LastWriteTime, newTags);
                    }

                    processedCount++;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"모듈 로드 오류 ({fi.Name}): {ex.Message}");
                }
            }
        }

        private ArrayList GetCachedModuleTags(FileInfo fileInfo)
        {
            if (moduleTagCache.ContainsKey(fileInfo.FullName))
            {
                var cached = moduleTagCache[fileInfo.FullName];

                // 파일이 변경되지 않았으면 캐시된 데이터 반환
                if (cached.lastModified == fileInfo.LastWriteTime)
                {
                    return (ArrayList)cached.tags.Clone(); // 복사본 반환
                }
                else
                {
                    // 파일이 변경되었으면 캐시에서 제거
                    moduleTagCache.Remove(fileInfo.FullName);
                }
            }

            return null;
        }

        private ArrayList LoadModuleTags(FileInfo fileInfo)
        {
            ArrayList tags = new ArrayList();

            using (Form form = new Form())
            {
                ObjectRoot root = new ObjectRoot();
                root.Load(form, fileInfo.FullName);
                root.GetMultiSelectTagList(tags, fileInfo.FullName);
            }

            return tags;
        }

        void AddTagListAlwaysScript()
        {
            string filename;

            filename = String.Format("{0}\\control\\always", TotalConfig.sDirWorkProject);

            if (!Directory.Exists(filename)) return;

            DirectoryInfo info = new DirectoryInfo(filename);

            foreach (FileInfo fi in info.GetFiles("*.ctl?"))
            {
                ScriptClass script = new ScriptClass();
                script.LoadFromFile(fi.FullName);
                script.GetMultiSelectTagList(arrayTags, fi.FullName, EnumTagUsedType.String, "ProgramScript", "OnProgramAlive");
            }
        }

        void AddTagListKeyScript(string when)
        {
            string filename;

            filename = String.Format("{0}\\control\\{1}", TotalConfig.sDirWorkProject, when);

            if (!Directory.Exists(filename)) return;

            DirectoryInfo info = new DirectoryInfo(filename);

            foreach (FileInfo fi in info.GetFiles("*.ctl?"))
            {
                ScriptClass script = new ScriptClass();
                script.LoadFromFile(fi.FullName);
                script.GetMultiSelectTagList(arrayTags, fi.FullName, EnumTagUsedType.String, when, Path.GetFileNameWithoutExtension(fi.FullName));
            }
        }

        void AddTagListMenuScript()
        {
            string filename;

            filename = String.Format("{0}\\control\\menu", TotalConfig.sDirWorkProject);

            if (!Directory.Exists(filename)) return;

            DirectoryInfo info = new DirectoryInfo(filename);

            foreach (FileInfo fi in info.GetFiles("*.ctl?"))
            {
                ScriptClass script = new ScriptClass();
                script.LoadFromFile(fi.FullName);
                script.GetMultiSelectTagList(arrayTags, fi.FullName, EnumTagUsedType.String, "MenuScript", Path.GetFileNameWithoutExtension(fi.FullName));
            }
        }

        void AddTagListTagEventScript()   // AddTagListTagEventScript(); 추가  2023-12-14 박상욱
        {
            string filename;

            filename = String.Format("{0}\\control\\TagEvent", TotalConfig.sDirWorkProject);

            if (!Directory.Exists(filename)) return;

            DirectoryInfo info = new DirectoryInfo(filename);

            foreach (FileInfo fi in info.GetFiles("*.ctl?"))
            {
                ScriptClass script = new ScriptClass();
                script.LoadFromFile(fi.FullName);
                script.GetMultiSelectTagList(arrayTags, fi.FullName, EnumTagUsedType.String, "TagEventScript", Path.GetFileNameWithoutExtension(fi.FullName));
            }
        }

        void AddTagListLogInOutScript(string used_position, string when)
        {
            ScriptClass script = new ScriptClass();

            string fullpath = TotalConfig.FileOldNew("control\\LogInOut", when + ".CTL", when + ".CTLX");

            if (!File.Exists(fullpath)) return;

            script.LoadFromFile(fullpath);
            script.GetMultiSelectTagList(arrayTags, fullpath, EnumTagUsedType.String, "LogScript", used_position);
        }

        void AddTagListDemandControl()
        {
            ArrayList array = new ArrayList();

            AutoLib.DemandControl.FunctionBlockDemandControlLoad(array);

            int l;
            FUNCTION_BLOCK_DEMAND_CONTROL item;

            for (l = 0; l < array.Count; l++)
            {
                item = (FUNCTION_BLOCK_DEMAND_CONTROL)array[l];

                TagUtil.AddTagList(arrayTags, item.tagCurr.tag, item.tagCurr.tag_type, "DemandControl", EnumTagUsedType.String, item.title, "TagCurr");
                TagUtil.AddTagList(arrayTags, item.tagEcho.tag, item.tagEcho.tag_type, "DemandControl", EnumTagUsedType.String, item.title, "TagEcho");
                TagUtil.AddTagList(arrayTags, item.tagEOI.tag, item.tagEOI.tag_type, "DemandControl", EnumTagUsedType.String, item.title, "tagEOI");
                TagUtil.AddTagList(arrayTags, item.tagOut.tag, item.tagOut.tag_type, "DemandControl", EnumTagUsedType.String, item.title, "tagOut");
                TagUtil.AddTagList(arrayTags, item.tagOut2.tag, item.tagOut2.tag_type, "DemandControl", EnumTagUsedType.String, item.title, "tagOut2");
                TagUtil.AddTagList(arrayTags, item.tagPredictionDisplay.tag, item.tagPredictionDisplay.tag_type, "DemandControl", EnumTagUsedType.String, item.title, "tagPredictionDisplay");
                TagUtil.AddTagList(arrayTags, item.tagPredictionInput.tag, item.tagPredictionInput.tag_type, "DemandControl", EnumTagUsedType.String, item.title, "tagPredictionInput");
                TagUtil.AddTagList(arrayTags, item.tagStartTarget.tag, item.tagStartTarget.tag_type, "DemandControl", EnumTagUsedType.String, item.title, "tagStartTarget");
                TagUtil.AddTagList(arrayTags, item.tagTarget.tag, item.tagTarget.tag_type, "DemandControl", EnumTagUsedType.String, item.title, "tagTarget");
            }
        }

        void AddTagListMilliData()
        {
            ArrayList array = new ArrayList();
            MilliData.LoadMilliData(array);

            MILLI_DATA_STRUCT item;
            int l;

            for (l = 0; l < array.Count; l++)
            {
                item = (MILLI_DATA_STRUCT)array[l];

                TagUtil.AddTagList(arrayTags, item.tagCheckDI, EnumTagType.DI, "MilliData", EnumTagUsedType.String, item.title, "tagCheckDI");

                MILLI_DATA_TAG tag;
                for (int j = 0; j < item.blockTag.Count; j++)
                {
                    tag = (MILLI_DATA_TAG)item.blockTag[j];

                    TagUtil.AddTagList(arrayTags, tag.tag, tag.tag_type, "MilliData", EnumTagUsedType.String, item.title, "Member");
                }
            }
        }

        void AddTagListOnOffList()
        {
            OnOffList.DigitalOnOffListLoad();
            ArrayList array = (ArrayList)Tools.CopyObject(OnOffList.blockOnOffList);

            ON_OFF_LIST item;
            int l;

            for (l = 0; l < array.Count; l++)
            {
                item = (ON_OFF_LIST)array[l];

                TagUtil.AddTagList(arrayTags, item.tag, EnumTagType.DI, "OnOffList", EnumTagUsedType.String, item.tag, "Check DI");

                ON_OFF_LIST_MEMBER tag;
                for (int j = 0; j < item.blockList.Count; j++)
                {
                    tag = (ON_OFF_LIST_MEMBER)item.blockList[j];

                    TagUtil.AddTagList(arrayTags, tag.tag, EnumTagType.none, "OnOffList", EnumTagUsedType.String, tag.tag, "Member");
                }
            }
        }

        void AddTagListReportModules()
        {
            string path = TotalConfig.sDirWorkProject + "\\Report";

            if (!Directory.Exists(path)) return;

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo fi in di.GetFiles("*.rptx"))
            {
                ReportModule.ReportTagInfo info = new ReportModule.ReportTagInfo();

                info.GetMultiSelectTagList(fi.FullName, arrayTags);
            }
        }

        //void ReloadTagListFromEntireProject()
        //{
        //    arrayTags = new ArrayList();

        //    AddTagListGraphicModules();

        //    AddTagListProgrammStartEndScript("OnProgramStart", "Start");    // 프로그램 시작시 스크립트
        //    AddTagListProgrammStartEndScript("OnProgramStart", "End");      // 프로그램 종료시 스크립트
        //    AddTagListAlwaysScript();

        //    AddTagListKeyScript("KeyDown");
        //    AddTagListKeyScript("KeyUp");

        //    AddTagListLogInOutScript("OnLogIn", "LogIn");
        //    AddTagListLogInOutScript("OnLogOut", "LogOut");
        //    AddTagListLogInOutScript("OnAfterLogOut", "AfterLogOut");

        //    AddTagListDemandControl();
        //    AddTagListMilliData();
        //    AddTagListOnOffList();

        //    AddTagListMenuScript();     //  MenuScript(); TagEventScript(); 추가  2023-12-14 박상욱
        //    AddTagListTagEventScript();

        //    AddTagListReportModules();

        //    MULTI_SELECT_TAG_STRUCT list;
        //    ListViewItem lvi;

        //    for (int i = 0; i < arrayTags.Count; i++)
        //    {
        //        list = (MULTI_SELECT_TAG_STRUCT)arrayTags[i];

        //        lvi = new ListViewItem(list.tagSource);
        //        lvi.SubItems.Add("");   // 값을 대입하지 않도록 한다.
        //        lvi.SubItems.Add(list.tag_type.ToString());

        //        listViewTag.Items.Add(lvi);
        //    }
        //}

        //void ReloadTagList()
        //{
        //    Cursor cursorsave = this.Cursor;
        //    Cursor = Cursors.WaitCursor;

        //    formParent = SharedStudio.formMain.ActiveMdiChild;

        //    this.listViewTag.Items.Clear();

        //    if (this.comboBoxLookin.SelectedIndex == 1)
        //    {
        //        if (formParent.GetType() == typeof(FormEditGraphicFrame))
        //        {
        //            FormEditGraphic child = ((FormEditGraphicFrame)formParent).formChild;

        //            arrayTags = new ArrayList();

        //            child.workThis.obj.groupRoot.GetMultiSelectTagList(arrayTags);

        //            MULTI_SELECT_TAG_STRUCT list;
        //            ListViewItem lvi;

        //            for (int i = 0; i < arrayTags.Count; i++)
        //            {
        //                list = (MULTI_SELECT_TAG_STRUCT)arrayTags[i];

        //                lvi = new ListViewItem(list.tagSource);
        //                lvi.SubItems.Add("");   // 값을 대입하지 않도록 한다.
        //                lvi.SubItems.Add(list.tag_type.ToString());

        //                listViewTag.Items.Add(lvi);
        //            }
        //        }
        //    }
        //    else if (this.comboBoxLookin.SelectedIndex == 2)    // Entire project
        //    {
        //        ReloadTagListFromEntireProject();
        //    }
        //    else
        //    {
        //        if (formParent.GetType() == typeof(FormEditGraphicFrame))
        //        {
        //            FormEditGraphic child = ((FormEditGraphicFrame)formParent).formChild;

        //            arrayTags = new ArrayList();

        //            for (int i = 0; i < child.workThis.nSelectCount; i++)
        //            {
        //                ((ObjectExpand)child.workThis.selectList[i].obj).GetMultiSelectTagList(arrayTags);
        //            }

        //            MULTI_SELECT_TAG_STRUCT list;
        //            ListViewItem lvi;

        //            for (int i = 0; i < arrayTags.Count; i++)
        //            {
        //                list = (MULTI_SELECT_TAG_STRUCT)arrayTags[i];

        //                lvi = new ListViewItem(list.tagSource);
        //                lvi.SubItems.Add("");   // 값을 대입하지 않도록 한다.
        //                lvi.SubItems.Add(list.tag_type.ToString());

        //                listViewTag.Items.Add(lvi);
        //            }
        //        }
        //    }

        //    Cursor = cursorsave;
        //}


        void ReloadTagListFromEntireProject()
        {
            // ListView 업데이트 일시 중단
            this.listViewTag.BeginUpdate();

            try
            {
                arrayTags = new ArrayList();

                AddTagListGraphicModules();

                AddTagListProgrammStartEndScript("OnProgramStart", "Start");    // 프로그램 시작시 스크립트
                AddTagListProgrammStartEndScript("OnProgramStart", "End");      // 프로그램 종료시 스크립트
                AddTagListAlwaysScript();

                AddTagListKeyScript("KeyDown");
                AddTagListKeyScript("KeyUp");

                AddTagListLogInOutScript("OnLogIn", "LogIn");
                AddTagListLogInOutScript("OnLogOut", "LogOut");
                AddTagListLogInOutScript("OnAfterLogOut", "AfterLogOut");

                AddTagListDemandControl();
                AddTagListMilliData();
                AddTagListOnOffList();

                AddTagListMenuScript();     //  MenuScript(); TagEventScript(); 추가  2023-12-14 박상욱
                AddTagListTagEventScript();

                AddTagListReportModules();

                // ListView 아이템 생성 및 한 번에 추가
                if (arrayTags.Count > 0)
                {
                    ListViewItem[] items = new ListViewItem[arrayTags.Count];

                    for (int i = 0; i < arrayTags.Count; i++)
                    {
                        MULTI_SELECT_TAG_STRUCT list = (MULTI_SELECT_TAG_STRUCT)arrayTags[i];

                        // SubItems를 한 번에 설정
                        items[i] = new ListViewItem(new string[]
                        {
                    list.tagSource,
                    "",  // 빈 값
                    list.tag_type.ToString()
                        });
                    }

                    // 한 번에 모든 아이템 추가
                    this.listViewTag.Items.AddRange(items);
                }
            }
            finally
            {
                // ListView 업데이트 재개
                this.listViewTag.EndUpdate();
            }
        }


        private BackgroundWorker tagLoadWorker;
        private volatile bool isLoading = false;


        void ReloadTagList()
        {
            if (isLoading) return;

            if (tagLoadWorker?.IsBusy == true)
            {
                CancelTagLoading();
                return;
            }

            if (this.comboBoxLookin.SelectedIndex == 2) // Entire Project
            {
                // 한국어/영어에 따른 메시지 표시
                string message, title;
                if (Tools.IsLangKorean())
                {
                    message = "전체 프로젝트 읽기는 저장된 모듈만 정상적으로 불러옵니다.\n진행하시겠습니까?";
                    title = "전체 프로젝트 읽기";
                }
                else
                {
                    message = "The entire project read only successfully imports the stored module..\nDo you want to proceed with loading?";
                    title = "Entire Project Loading";
                }

                // UI 스레드에서 메시지박스 표시
                DialogResult result = DialogResult.Cancel;
                this.Invoke(new Action(() =>
                {
                    result = MessageBox.Show(message, title,
                                           MessageBoxButtons.OKCancel,
                                           MessageBoxIcon.Question);
                }));

                // 사용자가 취소를 선택한 경우
                if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            StartTagLoading();
        }

        private void StartTagLoading()
        {
            isLoading = true;
            Cursor = Cursors.WaitCursor;

            // 취소 버튼 활성화
            if (cancelButton != null)
            {
                cancelButton.Enabled = true;
                if (Tools.IsLangKorean()) cancelButton.Text = "취소";
            }

            this.comboBoxLookin.Enabled = false;

            if (progressBar1 != null) progressBar1.Visible = true;

            // UI 초기화는 메인 스레드에서
            this.listViewTag.Items.Clear();
            formParent = SharedStudio.formMain.ActiveMdiChild;

            tagLoadWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            tagLoadWorker.DoWork += TagLoadWorker_DoWork;
            tagLoadWorker.ProgressChanged += TagLoadWorker_ProgressChanged;
            tagLoadWorker.RunWorkerCompleted += TagLoadWorker_RunWorkerCompleted;

            // 현재 선택된 옵션을 전달
            tagLoadWorker.RunWorkerAsync(this.comboBoxLookin.SelectedIndex);
        }

        private void CancelTagLoading()
        {
            if (tagLoadWorker?.IsBusy == true)
            {
                tagLoadWorker.CancelAsync();

                if (cancelButton != null)
                {
                    cancelButton.Enabled = false;
                }
            }
        }

        private void TagLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int selectedIndex = (int)e.Argument;

            try
            {
                arrayTags = new ArrayList();

                if (selectedIndex == 1) // Selected Frame
                {
                    worker.ReportProgress(0, "선택된 프레임의 태그 로딩...");
                    LoadSelectedFrameTagsInBackground(worker, e);
                }
                else if (selectedIndex == 2) // Entire Project
                {
                    worker.ReportProgress(0, "전체 프로젝트 태그 로딩...");
                    LoadEntireProjectTagsInBackground(worker, e);
                }
                else // Selected Objects
                {
                    worker.ReportProgress(0, "선택된 객체의 태그 로딩...");
                    LoadSelectedObjectsTagsInBackground(worker, e);
                }

                if (!worker.CancellationPending)
                {
                    worker.ReportProgress(100, "태그 목록 생성 완료");
                }
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void LoadSelectedFrameTagsInBackground(BackgroundWorker worker, DoWorkEventArgs e)
        {
            // UI 스레드에서 가져온 정보를 사용해야 하므로 Invoke 필요
            FormEditGraphic child = null;

            // UI 스레드에서 정보 가져오기
            this.Invoke(new Action(() =>
            {
                if (formParent?.GetType() == typeof(FormEditGraphicFrame))
                {
                    child = ((FormEditGraphicFrame)formParent).formChild;
                }
            }));

            if (child == null) return;

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            worker.ReportProgress(50, "프레임 태그 수집 중...");

            // 백그라운드에서 태그 수집
            child.workThis.obj.groupRoot.GetMultiSelectTagList(arrayTags);

            worker.ReportProgress(90, "프레임 태그 수집 완료");
        }

        private void LoadSelectedObjectsTagsInBackground(BackgroundWorker worker, DoWorkEventArgs e)
        {
            FormEditGraphic child = null;
            int selectCount = 0;

            // UI 스레드에서 정보 가져오기
            this.Invoke(new Action(() =>
            {
                if (formParent?.GetType() == typeof(FormEditGraphicFrame))
                {
                    child = ((FormEditGraphicFrame)formParent).formChild;
                    selectCount = child.workThis.nSelectCount;
                }
            }));

            if (child == null) return;

            for (int i = 0; i < selectCount; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                worker.ReportProgress((i * 80) / selectCount, $"선택된 객체 {i + 1}/{selectCount} 처리 중...");

                ((ObjectExpand)child.workThis.selectList[i].obj).GetMultiSelectTagList(arrayTags);

                // 각 객체 처리 후 잠시 대기 (취소 응답성 향상)
                System.Threading.Thread.Sleep(10);
            }
        }

        private void LoadEntireProjectTagsInBackground(BackgroundWorker worker, DoWorkEventArgs e)
        {
            var loadSteps = new (Action action, string description)[]
            {
        (() => AddTagListGraphicModules(), "그래픽 모듈 로딩..."),
        (() => AddTagListProgrammStartEndScript("OnProgramStart", "Start"), "시작 스크립트 로딩..."),
        (() => AddTagListProgrammStartEndScript("OnProgramStart", "End"), "종료 스크립트 로딩..."),
        (() => AddTagListAlwaysScript(), "상시 스크립트 로딩..."),
        (() => AddTagListKeyScript("KeyDown"), "키다운 스크립트 로딩..."),
        (() => AddTagListKeyScript("KeyUp"), "키업 스크립트 로딩..."),
        (() => AddTagListLogInOutScript("OnLogIn", "LogIn"), "로그인 스크립트 로딩..."),
        (() => AddTagListLogInOutScript("OnLogOut", "LogOut"), "로그아웃 스크립트 로딩..."),
        (() => AddTagListLogInOutScript("OnAfterLogOut", "AfterLogOut"), "로그아웃 후 스크립트 로딩..."),
        (() => AddTagListDemandControl(), "디맨드 컨트롤 로딩..."),
        (() => AddTagListMilliData(), "밀리 데이터 로딩..."),
        (() => AddTagListOnOffList(), "On/Off 리스트 로딩..."),
        (() => AddTagListMenuScript(), "메뉴 스크립트 로딩..."),
        (() => AddTagListTagEventScript(), "태그 이벤트 스크립트 로딩..."),
        (() => AddTagListReportModules(), "리포트 모듈 로딩...")
            };

            for (int i = 0; i < loadSteps.Length; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                int progressPercent = (i * 90) / loadSteps.Length;
                worker.ReportProgress(progressPercent, loadSteps[i].description);

                loadSteps[i].action();

                // 각 단계 후 잠시 대기
                System.Threading.Thread.Sleep(10);
            }
        }

        private void TagLoadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 진행률 업데이트
            if (progressBar1 != null)
            {
                progressBar1.Value = e.ProgressPercentage;
            }
        }

        private volatile bool isFormClosing = false;

        private void TagLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // 폼이 닫히는 중이면 메시지박스 표시하지 않음
                if (isFormClosing) return;

                if (e.Error != null)
                {
                    if (Tools.IsLangKorean())
                    {
                        MessageBox.Show($"태그 로딩 중 오류: {e.Error.Message}", "오류",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Lodaing Tags Error: {e.Error.Message}", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                if (e.Cancelled)
                {
                    if (Tools.IsLangKorean())
                    {
                        MessageBox.Show("태그 로딩이 취소되었습니다.", "취소됨",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Tag loading cancelled: {e.Error.Message}", "Canceled",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return;
                }

                // 성공적으로 완료된 경우 ListView 업데이트
                UpdateListViewWithResults();

                if (Tools.IsLangKorean())
                {
                    MessageBox.Show($"태그 로딩 완료: {arrayTags?.Count ?? 0}개 항목", "완료",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Tag loading completed: {arrayTags?.Count ?? 0} items", "Completed",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                // UI 상태 복원
                RestoreUIState();
            }
        }


        private void UpdateListViewWithResults()
        {
            if (arrayTags == null || arrayTags.Count == 0) return;

            this.listViewTag.BeginUpdate();

            try
            {
                ListViewItem[] items = new ListViewItem[arrayTags.Count];

                for (int i = 0; i < arrayTags.Count; i++)
                {
                    MULTI_SELECT_TAG_STRUCT list = (MULTI_SELECT_TAG_STRUCT)arrayTags[i];
                    items[i] = new ListViewItem(new string[]
                    {
                list.tagSource,
                string.Empty,
                list.tag_type.ToString()
                    });

                    // IsTagExist가 false이면 빨간색으로 설정
                    if (!AutoLibLocal.TagLib.IsTagExist(list.tagSource))
                    {
                        //items[i].BackColor = Color.Red;
                         items[i].ForeColor = Color.Red;
                    }
                }

                this.listViewTag.Items.AddRange(items);
            }
            finally
            {
                this.listViewTag.EndUpdate();
            }
        }

        private void RestoreUIState()
        {
            // 폼이 닫히는 중이면 UI 복원 작업 건너뛰기
            if (isFormClosing || this.IsDisposed) return;

            Cursor = Cursors.Default;
            isLoading = false;

            this.comboBoxLookin.Enabled = true;

            // 취소 버튼 비활성화
            if (cancelButton != null)
            {
                cancelButton.Enabled = false;
                cancelButton.Text = "취소";
            }

            // 진행률 바 숨기기
            if (progressBar1 != null)
            {
                progressBar1.Visible = false;
                progressBar1.Value = 0;
            }

            // 리소스 정리
            tagLoadWorker?.Dispose();
            tagLoadWorker = null;
        }



        private void FormFindReplaceTagLink_Load(object sender, EventArgs e)
        {
            if (Tools.IsLangKorean())
            {
                this.comboBoxLookin.Items.Add("선택한 요소");
                this.comboBoxLookin.Items.Add("현재 모듈");
                this.comboBoxLookin.Items.Add("전체 프로젝트");
            }
            else
            {
                this.comboBoxLookin.Items.Add("Selected objects");
                this.comboBoxLookin.Items.Add("Current Module");
                this.comboBoxLookin.Items.Add("Entire Project");
            }

            this.comboBoxLookin.SelectedIndex = 1;

            EnableDisableButton();
        }

        // Sort가 되어 있는 경우가 있으므로 포인터를 찾는다.
        MULTI_SELECT_TAG_STRUCT GetMultiSelectTagStruct(string tag)
        {
            MULTI_SELECT_TAG_STRUCT list;

            for (int i = 0; i < arrayTags.Count; i++)
            {
                list = (MULTI_SELECT_TAG_STRUCT)arrayTags[i];

                if (list.tagSource == tag) return list;
            }

            return null;
        }

        ArrayList arrayUsed;

        void UpdateTagUsedInfo()
        {
            listViewUsed.Items.Clear();
            arrayUsed = new ArrayList();

            if (listViewTag.SelectedItems.Count == 0) return;
            MULTI_SELECT_TAG_STRUCT list;
            TAG_USED_INFOMATION used;
            string buf;

            for (int i = 0; i < listViewTag.SelectedItems.Count; i++)
            {
                ListViewItem lvi = listViewTag.SelectedItems[i];
                list = GetMultiSelectTagStruct(lvi.SubItems[0].Text);

                for (int j = 0; j < list.arrayUsed.Count; j++)
                {
                    used = (TAG_USED_INFOMATION)list.arrayUsed[j];

                    arrayUsed.Add(used);    // 목록을 보관해 놓는다.

                    ListViewItem l = new ListViewItem(used.zone);

                    if (used.type == EnumTagUsedType.GraphicObject)
                    {
                        if (used.obj == null)
                            buf = "null";
                        else
                            buf = ((ObjectExpand)used.obj).objGeneral.sOnStudioTitle.Length == 0 ? ((ObjectExpand)used.obj).enumObjectType.ToString() : ((ObjectExpand)used.obj).objGeneral.sOnStudioTitle;
                    }
                    else if (used.type == EnumTagUsedType.String)
                    {
                        buf = (string)used.obj;
                    }
                    else
                    {
                        buf = used.obj.ToString();
                    } 
                    l.SubItems.Add(buf);
                    l.SubItems.Add(used.position);

                    this.listViewUsed.Items.Add(l);
                }
            }
        }

        private void listViewTag_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTagUsedInfo();
        }

        private void listViewTag_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewTag.SelectedItems.Count == 0) return;

            int index = listViewTag.SelectedItems[0].Index;

            string tag;
            string des;
            if (DialogTag.SelectTag.SelectAll(out tag, out des) == DialogResult.OK)
            {
                for (int i = 0; i < listViewTag.SelectedItems.Count; i++)
                {
                    ListViewItem lvi = listViewTag.SelectedItems[i];
                    lvi.SubItems[1].Text = tag;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        StringComparison GetComparison()
        {
            return checkBoxFindOptionMatchCase.Checked ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            string find = this.textBoxFind.Text.Trim();

            if (find.Length == 0) return;

            ListViewItem lvi;
            int pos;

            if (listViewTag.SelectedItems.Count > 0)
            {
                pos = listViewTag.SelectedItems[listViewTag.SelectedItems.Count - 1].Index+1;
            }
            else
            {
                pos = 0;
            }

            // 전체를 선택 취소한다.
            for (int i = 0; i < listViewTag.Items.Count; i++)
            {
                lvi = listViewTag.Items[i];

                lvi.Selected = false;
            }

            for (int i = 0; i < listViewTag.Items.Count; i++, pos++)
            {
                pos %= listViewTag.Items.Count;

                lvi = listViewTag.Items[pos];

                int index = lvi.SubItems[0].Text.IndexOf(find, GetComparison());

                if (index != -1)
                {
                    lvi.Selected = true;
                    lvi.EnsureVisible();
                    return;
                }
            }

            if (Tools.IsLangKorean())
            {
                MessageBox.Show(find, "텍스트를 찾을 수 없습니다.");
            }
            else
            {
                MessageBox.Show(find, "Can't find the text");
            }
        }

        void EnableDisableButton()
        {
            string find_text = this.textBoxFind.Text.Trim();
            string replace_text = this.textBoxReplace.Text.Trim();
            
            bool flag_find = (find_text.Length != 0);
            bool flag_replace = (find_text.Length != 0);

            this.buttonFindNext.Enabled = flag_find;

            if (this.comboBoxLookin.SelectedIndex == 2)
            {
                this.textBoxReplace.Enabled = false;
                this.buttonTag.Enabled = false;
                this.buttonReplace.Enabled = false;
            }
            else
            {
                this.textBoxReplace.Enabled = true;
                this.buttonTag.Enabled = true;
                this.buttonReplace.Enabled = flag_replace;
            }
        }

        private void textBoxFind_TextChanged(object sender, EventArgs e)
        {
            EnableDisableButton();
        }

        private void textBoxReplace_TextChanged(object sender, EventArgs e)
        {
            EnableDisableButton();
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            string find_text = this.textBoxFind.Text.Trim();
            string replace_text = this.textBoxReplace.Text.Trim();

            if (find_text.Length == 0) return;

            ListViewItem lvi;

            for (int i = 0; i < listViewTag.SelectedItems.Count; i++)
            {
                lvi = listViewTag.SelectedItems[i];

                string converted_text;

                if (checkBoxReplaceOptionWholeWord.Checked)
                {
                    int index = lvi.SubItems[0].Text.IndexOf(find_text, GetComparison());

                    if (index != -1)
                    {
                        converted_text = replace_text;
                    }
                    else
                    {
                        converted_text = find_text;
                    }
                }
                else
                {
                    if (checkBoxFindOptionMatchCase.Checked)
                        converted_text = lvi.SubItems[0].Text.Replace(find_text, replace_text);
                    else
                        converted_text = ReplaceEx(lvi.SubItems[0].Text, find_text, replace_text);
                }

                if(converted_text != lvi.SubItems[0].Text) {
                    lvi.SubItems[1].Text = converted_text;
                    lvi.EnsureVisible();
                }
            }
        }

        private static string ReplaceEx(string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern,
                                              position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        private void buttonTag_Click(object sender, EventArgs e)
        {
            string tag;
            string des;
            if (DialogTag.SelectTag.SelectAll(this, out tag, out des) == DialogResult.OK)
            {
                this.textBoxReplace.Text = tag;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (comboBoxLookin.SelectedIndex == 2)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("[전체 프로젝트] 검색범위에서는 바꾸기 기능을 지원하지 않습니다.", "지원 안됨");
                }
                else
                {
                    MessageBox.Show("Replace function not supported in [Entire Project]", "Not Supported");
                }
                return;
            }

            if (Tools.IsLangKorean())
            {
                if (MessageBox.Show("바뀐 태그를 프로젝트에 적용할까요?", "적용확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
            }
            else
            {
                if (MessageBox.Show("Apply the replaced Tags to project?", "Apply", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
            }

            if (formParent != SharedStudio.formMain.ActiveMdiChild)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("찾은 영역과 바꾸기할 영역의 파일이 일치하지 않습니다.");
                }
                else
                {
                    MessageBox.Show("File mismatched at Find Zone And Replace Zone.");
                }
                return;
            }

            ListViewItem lvi;

            FormEditGraphic child = ((FormEditGraphicFrame)formParent).formChild;

            MULTI_SELECT_TAG_STRUCT list;
            int changed_count = 0;

            for (int i = 0; i < listViewTag.Items.Count; i++)
            {
                lvi = listViewTag.Items[i];

                list = GetMultiSelectTagStruct(lvi.SubItems[0].Text);
                string change_text = lvi.SubItems[1].Text;

                if (change_text.Length > 0) // 뭔가 바뀐경우에만 
                {
                    changed_count++;
                    if (!AutoLibLocal.TagLib.IsTagExist(change_text))
                    {
                        if (Tools.IsLangKorean())
                        {
                            if (MessageBox.Show("태그가 존재하지 않습니다.\n계속할까요?", "TagName:" + change_text, MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (MessageBox.Show("Tag not exists.\nContinue?", "TagName:" + change_text, MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                return;
                            }
                        }
                    }

                    list.tagTarget = change_text;
                }
                else
                {
                    list.tagTarget = list.tagSource;
                }
            }

            if (comboBoxLookin.SelectedIndex == 1)
            {
                child.workThis.obj.SetMultiSelectTagList(arrayTags);
            }
            else
            {
                for (int i = 0; i < child.workThis.nSelectCount; i++)
                {
                    ((ObjectExpand)child.workThis.selectList[i].obj).SetMultiSelectTagList(arrayTags);
                }
            }

            if (changed_count > 0)
            {
                child.SetChangeFlag();
            }

            if (Tools.IsLangKorean())
            {
                string msg = String.Format("{0} 개의 태그가 변경되었습니다.", changed_count);
                MessageBox.Show(msg, "변경완료");
            }
            else
            {
                string msg = String.Format("{0} Tags are replaced.", changed_count);
                MessageBox.Show(msg, "Replace OK");
            }
        }

        private void comboBoxLookin_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDisableButton();
            ReloadTagList();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            ReloadTagList();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormFindReplaceTagLink_FormClosed(object sender, FormClosedEventArgs e)
        {
            dialogFind = null;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewTag.Items.Count; i++)
            {
                listViewTag.Items[i].Selected = true;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewTag.SelectedItems.Count; i++)
            {
                listViewTag.SelectedItems[i].SubItems[1].Text = "";
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) //240409 태그이름 복사추가 hsjeong
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < listViewTag.SelectedItems.Count; i++)
            {
                sb.Append(listViewTag.SelectedItems[i].SubItems[0].Text.ToString() );
                sb.AppendLine();
            }
            sb.AppendLine();


            Clipboard.SetDataObject(sb.ToString().Trim());
        }

        private void listViewUsed_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewUsed.SelectedItems.Count == 0) return;

            string path = listViewUsed.SelectedItems[0].SubItems[0].Text;

            string ext = Path.GetExtension(path);

            string filename = String.Format("{0}\\{1}", TotalConfig.sDirWorkProject, path);

            if (!File.Exists(filename)) return;

            if (String.Compare(ext, ".modx", true) == 0)
            {
                SharedStudio.formMain.OpenModule(filename);
            }
            else if (String.Compare(ext, ".rptx", true) == 0)
            {
                SharedStudio.formMain.OpenReport(filename);
            }
            else if (String.Compare(ext, ".ctlx", true) == 0)
            {
                if (FormScriptAlways.IsExternalScriptEditorKey())
                {
                    FormScriptAlways.RunExternalScriptEditor(path);
                    return;
                }

                FormScriptEditor dialog = new FormScriptEditor();

                dialog.SetScript(filename);
                dialog.ShowDialog();
            }
        }

        private void listViewUsed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLookin.SelectedIndex != 1) return;    // Module Lookin
            if (formParent != SharedStudio.formMain.ActiveMdiChild) return;

            if (formParent.GetType() != typeof(FormEditGraphicFrame)) return;

            if (listViewUsed.SelectedItems.Count == 0) return;

            FormEditGraphicFrame form = (FormEditGraphicFrame)formParent;

            form.formChild.SelectListClear(form.formChild.workThis);

            TAG_USED_INFOMATION used = null;

            for (int i = 0; i < listViewUsed.SelectedItems.Count; i++)
            {
                used = (TAG_USED_INFOMATION)arrayUsed[listViewUsed.SelectedItems[i].Index];
                ((ObjectExpand)used.obj).bOnStudioSelected = true;
            }
            
            form.formChild.SelectListReMake();

            form.formChild.DisplayAfterSelectedChanged();
            form.formChild.SelectNodeClear();

            form.formChild.ScrollToObject((ObjectExpand)used?.obj); //250731 PSU 현재모듈 요소 위치로 스크롤 이동.
            form.formChild.Invalidate();
        }

        private void FormFindReplaceTagLink_SizeChanged(object sender, EventArgs e)
        {
            listViewUsed.Width = this.ClientRectangle.Width - 30;
        }

        private void FormFindReplaceTagLink_FormClosing(object sender, FormClosingEventArgs e)
        {
            isFormClosing = true;  // 폼 종료 상태 설정

            if (isLoading)
            {
                CancelTagLoading();

                // 작업이 완전히 취소될 때까지 잠시 대기
                int waitCount = 0;
                while (isLoading && waitCount < 50) // 최대 5초 대기
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(100);
                    waitCount++;
                }
            }
        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            CancelTagLoading();
        }
    }

}
