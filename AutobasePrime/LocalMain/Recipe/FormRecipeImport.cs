using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using NetTools;

namespace LocalMain
{
	/// <summary>
	/// LocalMain JSON Import Tool
	/// JSON 파일에서 레시피를 DB로 Import (Transaction 기반)
	/// </summary>
	public class FormRecipeImport : Form
	{
		private DataGridView gridCompare;
		private DataGridViewCheckBoxColumn colSelect;
		private DataGridViewTextBoxColumn colStatus;
		private DataGridViewTextBoxColumn colName;
		private DataGridViewTextBoxColumn colJsonVer;
		private DataGridViewTextBoxColumn colDbVer;
		private DataGridViewTextBoxColumn colAction;

		private Button btnImport;
		private Button btnCancel;
		private Button btnSelectAll;
		private Button btnDeselectAll;
		private Label labelSummary;
		private Label labelFilePath;
		private ProgressBar progressBar;

		private List<ImportCompareItem> _compareItems = new List<ImportCompareItem>();
		private string _jsonFilePath;

		public FormRecipeImport()
		{
			InitializeComponent();
			ApplyLocalization();
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.gridCompare = new DataGridView();
			this.colSelect = new DataGridViewCheckBoxColumn();
			this.colStatus = new DataGridViewTextBoxColumn();
			this.colName = new DataGridViewTextBoxColumn();
			this.colJsonVer = new DataGridViewTextBoxColumn();
			this.colDbVer = new DataGridViewTextBoxColumn();
			this.colAction = new DataGridViewTextBoxColumn();
			this.btnImport = new Button();
			this.btnCancel = new Button();
			this.btnSelectAll = new Button();
			this.btnDeselectAll = new Button();
			this.labelSummary = new Label();
			this.labelFilePath = new Label();
			this.progressBar = new ProgressBar();

			((System.ComponentModel.ISupportInitialize)(this.gridCompare)).BeginInit();
			this.SuspendLayout();

			// labelFilePath
			this.labelFilePath.Location = new Point(10, 10);
			this.labelFilePath.Size = new Size(760, 20);
			this.labelFilePath.Text = "";

			// gridCompare
			this.colSelect.HeaderText = "Select";
			this.colSelect.Width = 45;
			this.colSelect.Name = "colSelect";

			this.colStatus.HeaderText = "Status";
			this.colStatus.Width = 80;
			this.colStatus.ReadOnly = true;
			this.colStatus.Name = "colStatus";

			this.colName.HeaderText = "Recipe Name";
			this.colName.Width = 200;
			this.colName.ReadOnly = true;
			this.colName.Name = "colName";

			this.colJsonVer.HeaderText = "JSON Ver";
			this.colJsonVer.Width = 70;
			this.colJsonVer.ReadOnly = true;
			this.colJsonVer.Name = "colJsonVer";

			this.colDbVer.HeaderText = "DB Ver";
			this.colDbVer.Width = 60;
			this.colDbVer.ReadOnly = true;
			this.colDbVer.Name = "colDbVer";

			this.colAction.HeaderText = "Action";
			this.colAction.Width = 200;
			this.colAction.ReadOnly = true;
			this.colAction.Name = "colAction";

			this.gridCompare.Columns.AddRange(new DataGridViewColumn[] {
				this.colSelect, this.colStatus, this.colName,
				this.colJsonVer, this.colDbVer, this.colAction
			});
			this.gridCompare.Location = new Point(10, 35);
			this.gridCompare.Size = new Size(760, 340);
			this.gridCompare.AllowUserToAddRows = false;
			this.gridCompare.AllowUserToDeleteRows = false;
			this.gridCompare.RowHeadersVisible = false;
			this.gridCompare.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.gridCompare.CellContentClick += new DataGridViewCellEventHandler(gridCompare_CellContentClick);

			// Buttons
			this.btnSelectAll.Text = "Select All";
			this.btnSelectAll.Location = new Point(10, 385);
			this.btnSelectAll.Size = new Size(90, 28);
			this.btnSelectAll.Click += new EventHandler(btnSelectAll_Click);

			this.btnDeselectAll.Text = "Deselect All";
			this.btnDeselectAll.Location = new Point(110, 385);
			this.btnDeselectAll.Size = new Size(90, 28);
			this.btnDeselectAll.Click += new EventHandler(btnDeselectAll_Click);

			this.labelSummary.Location = new Point(210, 390);
			this.labelSummary.Size = new Size(350, 20);
			this.labelSummary.Text = "";

			this.progressBar.Location = new Point(10, 420);
			this.progressBar.Size = new Size(560, 20);
			this.progressBar.Visible = false;

			this.btnImport.Text = "Execute Import";
			this.btnImport.Location = new Point(580, 415);
			this.btnImport.Size = new Size(95, 28);
			this.btnImport.Click += new EventHandler(btnImport_Click);

			this.btnCancel.Text = "Close";
			this.btnCancel.Location = new Point(680, 415);
			this.btnCancel.Size = new Size(90, 28);
			this.btnCancel.Click += new EventHandler(btnCancel_Click);

			// Form
			this.ClientSize = new Size(785, 455);
			this.Controls.AddRange(new Control[] {
				this.labelFilePath, this.gridCompare,
				this.btnSelectAll, this.btnDeselectAll, this.labelSummary,
				this.progressBar, this.btnImport, this.btnCancel
			});
			this.Text = "Recipe JSON Import";
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;

			((System.ComponentModel.ISupportInitialize)(this.gridCompare)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private void ApplyLocalization()
		{
			bool isKor = Tools.IsLangKorean();

			this.Text = isKor ? "레시피 JSON Import" : "Recipe JSON Import";

			// Grid columns
			this.colSelect.HeaderText = isKor ? "선택" : "Select";
			this.colStatus.HeaderText = isKor ? "상태" : "Status";
			this.colName.HeaderText = isKor ? "레시피명" : "Recipe Name";
			this.colAction.HeaderText = isKor ? "동작" : "Action";

			// Buttons
			this.btnSelectAll.Text = isKor ? "전체 선택" : "Select All";
			this.btnDeselectAll.Text = isKor ? "전체 해제" : "Deselect All";
			this.btnImport.Text = isKor ? "Import 실행" : "Execute Import";
			this.btnCancel.Text = isKor ? "닫기" : "Close";
		}

		#region Load & Compare

		/// <summary>
		/// JSON 파일을 선택하고 DB와 비교 수행
		/// </summary>
		public async void LoadAndCompare()
		{
			bool isKor = Tools.IsLangKorean();

			// 파일 선택
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
			dlg.Title = isKor ? "Import할 레시피 JSON 파일 선택" : "Select Recipe JSON File to Import";

			if (dlg.ShowDialog(this) != DialogResult.OK)
			{
				this.DialogResult = DialogResult.Cancel;
				this.Close();
				return;
			}

			_jsonFilePath = dlg.FileName;
			labelFilePath.Text = (isKor ? "파일: " : "File: ") + _jsonFilePath;

			// JSON 파싱 + 검증
			string error;
			List<RecipeData> jsonRecipes = RecipeJsonHelper.LoadFromJsonFile(_jsonFilePath, out error);
			if (jsonRecipes == null || jsonRecipes.Count == 0)
			{
				MessageBox.Show(
					(isKor ? "JSON 파일 로드 실패: " : "Failed to load JSON file: ") + (error ?? "레시피가 없습니다."),
					isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.DialogResult = DialogResult.Cancel;
				this.Close();
				return;
			}

			// DB와 비교
			await CompareWithDatabase(jsonRecipes);

			// 비교 결과 표시
			RefreshGrid();
			UpdateSummary();

			this.ShowDialog();
		}

		async Task CompareWithDatabase(List<RecipeData> jsonRecipes)
		{
			bool isKor = Tools.IsLangKorean();
			_compareItems.Clear();

			var db = DataPostgres.Instance;
			if (db == null)
			{
				MessageBox.Show(
					isKor ? "DB 연결이 없습니다." : "No DB connection.",
					isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			for (int i = 0; i < jsonRecipes.Count; i++)
			{
				var jsonRecipe = jsonRecipes[i];
				var item = new ImportCompareItem();
				item.JsonRecipe = jsonRecipe;

				try
				{
					// GUID로 DB 조회
					RecipeInfo dbInfo = null;
					if (!string.IsNullOrEmpty(jsonRecipe.recipe_guid))
					{
						dbInfo = await db.GetRecipeInfoByGuidAsync(jsonRecipe.recipe_guid);
					}

					item.DbInfo = dbInfo;

					if (dbInfo == null)
					{
						// DB에 없음 → New
						item.Status = ImportStatus.New;
						item.Selected = true;
						item.StatusText = isKor ? "신규" : "New";
					}
					else if (dbInfo.version < jsonRecipe.version)
					{
						// DB version < JSON version → Update
						item.Status = ImportStatus.Update;
						item.Selected = true;
						item.StatusText = isKor ? "업데이트" : "Update";
					}
					else if (dbInfo.version == jsonRecipe.version)
					{
						// 동일 version → 내용 비교
						RecipeData dbRecipe = await db.GetRecipeAsync(dbInfo.recipe_id);
						item.DbRecipe = dbRecipe;

						if (dbRecipe != null && RecipeJsonHelper.IsContentEqual(jsonRecipe, dbRecipe))
						{
							// 동일 내용 → Skip
							item.Status = ImportStatus.Skip;
							item.Selected = false;
							item.StatusText = isKor ? "동일" : "Same";
						}
						else
						{
							// 다른 내용 → Conflict
							item.Status = ImportStatus.Conflict;
							item.Selected = false;
							item.StatusText = isKor ? "충돌" : "Conflict";
						}
					}
					else
					{
						// DB version > JSON version → Warning
						item.Status = ImportStatus.Warning;
						item.Selected = false;
						item.StatusText = string.Format(isKor ? "경고 (DB v{0} > JSON v{1})" : "Warning (DB v{0} > JSON v{1})",
							dbInfo.version, jsonRecipe.version);
					}
				}
				catch (Exception ex)
				{
					item.Status = ImportStatus.Conflict;
					item.Selected = false;
					item.StatusText = "Error: " + ex.Message;
				}

				_compareItems.Add(item);
			}
		}

		#endregion

		#region Grid Display

		void RefreshGrid()
		{
			bool isKor = Tools.IsLangKorean();
			gridCompare.Rows.Clear();

			for (int i = 0; i < _compareItems.Count; i++)
			{
				var item = _compareItems[i];
				int rowIdx = gridCompare.Rows.Add();
				var row = gridCompare.Rows[rowIdx];

				row.Cells["colSelect"].Value = item.Selected;
				row.Cells["colStatus"].Value = item.StatusText;
				row.Cells["colName"].Value = item.JsonRecipe.recipe_name;
				row.Cells["colJsonVer"].Value = item.JsonRecipe.version;
				row.Cells["colDbVer"].Value = item.DbInfo != null ? item.DbInfo.version.ToString() : "-";

				// 동작 텍스트
				switch (item.Status)
				{
					case ImportStatus.New:
						row.Cells["colAction"].Value = isKor ? "DB에 새로 추가" : "Insert new to DB";
						row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220); // 연두
						break;
					case ImportStatus.Update:
						row.Cells["colAction"].Value = isKor
							? string.Format("v{0} → v{1} 업데이트", item.DbInfo.version, item.JsonRecipe.version)
							: string.Format("Update v{0} → v{1}", item.DbInfo.version, item.JsonRecipe.version);
						row.DefaultCellStyle.BackColor = Color.FromArgb(220, 230, 255); // 연파랑
						break;
					case ImportStatus.Skip:
						row.Cells["colAction"].Value = isKor ? "동일하므로 건너뛰기" : "Skip (identical)";
						row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240); // 회색
						break;
					case ImportStatus.Conflict:
						row.Cells["colAction"].Value = isKor ? "내용 다름 - 선택 시 덮어쓰기" : "Content differs - will overwrite if selected";
						row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220); // 연빨강
						break;
					case ImportStatus.Warning:
						row.Cells["colAction"].Value = isKor ? "DB가 더 높은 버전 - 선택 시 다운그레이드" : "DB has newer version - will downgrade if selected";
						row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 200); // 주황
						break;
				}

				row.Tag = i;
			}
		}

		void UpdateSummary()
		{
			bool isKor = Tools.IsLangKorean();

			int newCount = 0, updateCount = 0, skipCount = 0, conflictCount = 0, warningCount = 0, selectedCount = 0;

			for (int i = 0; i < _compareItems.Count; i++)
			{
				switch (_compareItems[i].Status)
				{
					case ImportStatus.New: newCount++; break;
					case ImportStatus.Update: updateCount++; break;
					case ImportStatus.Skip: skipCount++; break;
					case ImportStatus.Conflict: conflictCount++; break;
					case ImportStatus.Warning: warningCount++; break;
				}
				if (_compareItems[i].Selected) selectedCount++;
			}

			if (isKor)
			{
				labelSummary.Text = string.Format(
					"신규:{0}  업데이트:{1}  동일:{2}  충돌:{3}  경고:{4}  |  선택:{5}",
					newCount, updateCount, skipCount, conflictCount, warningCount, selectedCount);
			}
			else
			{
				labelSummary.Text = string.Format(
					"New:{0}  Update:{1}  Same:{2}  Conflict:{3}  Warn:{4}  |  Selected:{5}",
					newCount, updateCount, skipCount, conflictCount, warningCount, selectedCount);
			}
		}

		void gridCompare_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			if (e.ColumnIndex != gridCompare.Columns["colSelect"].Index) return;

			// Checkbox 토글 반영
			gridCompare.CommitEdit(DataGridViewDataErrorContexts.Commit);
			bool val = (bool)(gridCompare.Rows[e.RowIndex].Cells["colSelect"].Value ?? false);
			int itemIndex = (int)gridCompare.Rows[e.RowIndex].Tag;
			_compareItems[itemIndex].Selected = val;

			UpdateSummary();
		}

		void btnSelectAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < _compareItems.Count; i++)
				_compareItems[i].Selected = true;
			RefreshGrid();
			UpdateSummary();
		}

		void btnDeselectAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < _compareItems.Count; i++)
				_compareItems[i].Selected = false;
			RefreshGrid();
			UpdateSummary();
		}

		#endregion

		#region Import Execution

		async void btnImport_Click(object sender, EventArgs e)
		{
			bool isKor = Tools.IsLangKorean();

			// 권한 체크
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN &&
				!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_MODIFY))
			{
				MessageBox.Show(
					isKor ? "레시피 수정 권한이 없습니다." : "No recipe modify permission.",
					isKor ? "권한 오류" : "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			// 선택된 항목 수집
			var selectedItems = new List<ImportCompareItem>();
			for (int i = 0; i < _compareItems.Count; i++)
			{
				if (_compareItems[i].Selected)
					selectedItems.Add(_compareItems[i]);
			}

			if (selectedItems.Count == 0)
			{
				MessageBox.Show(
					isKor ? "Import할 레시피를 선택하세요." : "Please select recipes to import.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			// 실행 중인 레시피 차단 검사
			for (int i = 0; i < selectedItems.Count; i++)
			{
				string recipeName = selectedItems[i].JsonRecipe.recipe_name;
				if (CheckEngineRecipe.IsExecuting(recipeName, null))
				{
					MessageBox.Show(
						string.Format(isKor
							? "레시피 '{0}'이(가) 현재 실행 중입니다.\n실행 중인 레시피는 Import할 수 없습니다."
							: "Recipe '{0}' is currently executing.\nCannot import a running recipe.", recipeName),
						isKor ? "차단" : "Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
			}

			// Conflict/Warning 항목 확인
			int conflictWarnCount = 0;
			for (int i = 0; i < selectedItems.Count; i++)
			{
				if (selectedItems[i].Status == ImportStatus.Conflict ||
					selectedItems[i].Status == ImportStatus.Warning)
					conflictWarnCount++;
			}

			if (conflictWarnCount > 0)
			{
				if (MessageBox.Show(
					string.Format(isKor
						? "선택한 항목 중 {0}개가 충돌/경고 상태입니다.\n기존 레시피를 덮어씁니다. 계속하시겠습니까?"
						: "{0} selected items have conflict/warning status.\nExisting recipes will be overwritten. Continue?", conflictWarnCount),
					isKor ? "확인" : "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
					return;
			}

			// 최종 확인
			if (MessageBox.Show(
				string.Format(isKor
					? "{0}개의 레시피를 Import하시겠습니까?"
					: "Import {0} recipes?", selectedItems.Count),
				isKor ? "확인" : "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			// Import 실행
			await ExecuteImport(selectedItems);
		}

		async Task ExecuteImport(List<ImportCompareItem> selectedItems)
		{
			bool isKor = Tools.IsLangKorean();

			btnImport.Enabled = false;
			btnCancel.Enabled = false;
			btnSelectAll.Enabled = false;
			btnDeselectAll.Enabled = false;
			gridCompare.Enabled = false;
			progressBar.Visible = true;
			progressBar.Maximum = selectedItems.Count;
			progressBar.Value = 0;

			try
			{
				var db = DataPostgres.Instance;
				if (db == null)
				{
					MessageBox.Show(
						isKor ? "DB 연결이 없습니다." : "No DB connection.",
						isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// ImportRecipeBatchAsync용 데이터 구성
				var importItems = new List<(RecipeData recipe, bool softArchiveExisting)>();

				for (int i = 0; i < selectedItems.Count; i++)
				{
					var item = selectedItems[i];
					bool softArchive = (item.Status != ImportStatus.New); // New가 아니면 기존 아카이브
					importItems.Add((item.JsonRecipe, softArchive));
				}

				// 사용자 정보
				string username = "";
				try
				{
					if (SharedData.userInfo != null && !string.IsNullOrEmpty(SharedData.userInfo.sUsername))
						username = SharedData.userInfo.sUsername;
				}
				catch { }

				string machineName = Environment.MachineName;

				// 단일 트랜잭션으로 Import 실행
				var result = await db.ImportRecipeBatchAsync(importItems, username, machineName);

				progressBar.Value = progressBar.Maximum;

				if (result.error != null)
				{
					MessageBox.Show(
						(isKor ? "Import 실패 (전체 Rollback): " : "Import failed (full Rollback): ") + result.error,
						isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					// 성공: RecipeManager 캐시 갱신
					RecipeManager.ReLoad();

					MessageBox.Show(
						string.Format(isKor
							? "Import 완료: {0}개 레시피가 DB에 반영되었습니다."
							: "Import complete: {0} recipes applied to DB.", result.imported),
						isKor ? "완료" : "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

					this.DialogResult = DialogResult.OK;
					this.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					(isKor ? "Import 오류: " : "Import error: ") + ex.Message,
					isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				btnImport.Enabled = true;
				btnCancel.Enabled = true;
				btnSelectAll.Enabled = true;
				btnDeselectAll.Enabled = true;
				gridCompare.Enabled = true;
				progressBar.Visible = false;
			}
		}

		#endregion

		void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}

    /// <summary>
    /// Import 비교 상태
    /// </summary>
    public enum ImportStatus
    {
        New,       // DB에 없음 → 신규 INSERT
        Update,    // DB version < JSON version → 업데이트
        Skip,      // 동일 version, 동일 내용 → 건너뛰기
        Conflict,  // 동일 version, 다른 내용 → 충돌
        Warning    // DB version > JSON version → 경고 (다운그레이드)
    }

    /// <summary>
    /// Import 비교 항목
    /// </summary>
    public class ImportCompareItem
    {
        public RecipeData JsonRecipe;      // JSON에서 읽은 레시피
        public RecipeInfo DbInfo;          // DB의 레시피 정보 (없으면 null)
        public RecipeData DbRecipe;        // DB의 레시피 전체 (비교용, 필요 시 로드)
        public ImportStatus Status;
        public bool Selected;              // 사용자 선택 여부
        public string StatusText;          // 상태 설명 텍스트
    }

}
