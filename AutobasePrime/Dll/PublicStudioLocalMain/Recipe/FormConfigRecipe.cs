using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using Newtonsoft.Json;

namespace PublicStudioLocalMain.Recipe
{
	/// <summary>
	/// Studio용 레시피 설정 메인 폼 (ISA-88 Lite: Unit 계층 지원)
	/// JSON 파일 기반 저장 (DB 의존 제거)
	/// </summary>
	public partial class FormConfigRecipe : System.Windows.Forms.Form
	{
		private RecipeData _currentRecipe;
		private List<RecipeData> _allRecipes = new List<RecipeData>();
		private string _jsonFilePath = "";
		private const string NODE_TAG_DIRECT = "__DIRECT__";

		public FormConfigRecipe()
		{
			InitializeComponent();
			ApplyLocalization();
			InitJsonFilePath();
			ApplyPermissions();
			LoadRecipeList();
		}

		#region JSON 파일 경로

		void InitJsonFilePath()
		{
			string projectDir = TotalConfig.sDirWorkProject;
			if (string.IsNullOrEmpty(projectDir))
				projectDir = "";

			string recipeDir = Path.Combine(projectDir, "RECIPE");
			if (!Directory.Exists(recipeDir))
			{
				try { Directory.CreateDirectory(recipeDir); }
				catch { }
			}

			_jsonFilePath = Path.Combine(recipeDir, "recipes.json");
		}

		#endregion


		#region Localization (다국어 - InitializeComponent 외부)

		void ApplyLocalization()
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			this.Text = isKor ? "레시피 설정" : "Recipe Configuration";

			// ToolStrip buttons
			this.btnAddRecipe.Text = isKor ? "레시피 추가" : "Add Recipe";
			this.btnDeleteRecipe.Text = isKor ? "레시피 삭제" : "Delete Recipe";
			this.btnAddUnit.Text = isKor ? "Unit 추가" : "Add Unit";
			this.btnDeleteUnit.Text = isKor ? "Unit 삭제" : "Delete Unit";
			this.btnAddStep.Text = isKor ? "Step 추가" : "Add Step";
			this.btnEditStep.Text = isKor ? "Step 편집" : "Edit Step";
			this.btnDeleteStep.Text = isKor ? "Step 삭제" : "Delete Step";
			this.btnExportCsv.Text = isKor ? "CSV 내보내기" : "Export CSV";
			this.btnImportCsv.Text = isKor ? "CSV 가져오기" : "Import CSV";
			this.btnSave.Text = isKor ? "저장" : "Save";

			// ListView columns
			this.colName.Text = isKor ? "레시피 이름" : "Recipe Name";
			this.colVersion.Text = isKor ? "버전" : "Ver";
			this.colDescription.Text = isKor ? "설명" : "Description";

			// Labels
			this.labelRecipeName.Text = isKor ? "이름:" : "Name:";
			this.labelDescription.Text = isKor ? "설명:" : "Desc:";
			this.labelRecipeMode.Text = isKor ? "모드:" : "Mode:";
			this.labelStatus.Text = isKor ? "상태:" : "Status:";
		}

		#endregion

		#region Permission

		void ApplyPermissions()
		{
			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return;

			// MODIFY 권한 없으면 편집 버튼 비활성화
			if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_MODIFY))
			{
				btnAddRecipe.Enabled = false;
				btnDeleteRecipe.Enabled = false;
				btnAddUnit.Enabled = false;
				btnDeleteUnit.Enabled = false;
				btnAddStep.Enabled = false;
				btnEditStep.Enabled = false;
				btnDeleteStep.Enabled = false;
				btnSave.Enabled = false;
				btnImportCsv.Enabled = false;
			}

			// VIEW 권한 없으면 Export도 비활성화
			if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_VIEW))
			{
				btnExportCsv.Enabled = false;
			}

			}

		#endregion

		#region Recipe List (JSON 파일 기반)

		void LoadRecipeList()
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			listViewRecipes.Items.Clear();
			_currentRecipe = null;
			treeViewStructure.Nodes.Clear();
			textBoxRecipeName.Text = "";
			textBoxDescription.Text = "";

			try
			{
				// JSON 파일이 없으면 빈 목록
				if (!File.Exists(_jsonFilePath))
				{
					_allRecipes = new List<RecipeData>();
					return;
				}

				string error;
				var loaded = RecipeJsonHelper.LoadFromJsonFile(_jsonFilePath, out error);
				if (loaded == null)
				{
					_allRecipes = new List<RecipeData>();
					if (error != null)
					{
						MessageBox.Show(
							(isKor ? "레시피 파일 로드 실패: " : "Failed to load recipe file: ") + error,
							isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					return;
				}

				_allRecipes = loaded;

				for (int i = 0; i < _allRecipes.Count; i++)
				{
					var recipe = _allRecipes[i];
					ListViewItem lvi = new ListViewItem(recipe.recipe_name);
					lvi.SubItems.Add(recipe.version.ToString());
					lvi.SubItems.Add(recipe.description);
					lvi.Tag = i; // 인덱스를 Tag로 저장
					listViewRecipes.Items.Add(lvi);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					(isKor ? "레시피 목록 로드 실패: " : "Failed to load recipe list: ") + ex.Message,
					isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		void LoadRecipeDetail(int recipeIndex)
		{
			treeViewStructure.Nodes.Clear();
			textBoxRecipeName.Text = "";
			textBoxDescription.Text = "";

			if (recipeIndex < 0 || recipeIndex >= _allRecipes.Count)
			{
				_currentRecipe = null;
				return;
			}

			_currentRecipe = _allRecipes[recipeIndex];

			textBoxRecipeName.Text = _currentRecipe.recipe_name;
			textBoxDescription.Text = _currentRecipe.description;
			comboBoxRecipeMode.SelectedIndex = (_currentRecipe.recipe_mode == "quick") ? 1 : 0;

			UpdateStatusDisplay();
			ApplyModeUI();
			RefreshTree();
		}

		void listViewRecipes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewRecipes.SelectedItems.Count == 0) return;

			int recipeIndex = (int)listViewRecipes.SelectedItems[0].Tag;
			LoadRecipeDetail(recipeIndex);
		}

		#endregion

		#region JSON 저장

		bool SaveAllRecipesToJson()
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			// 구조 검증
			string validationError = RecipeJsonHelper.ValidateRecipes(_allRecipes);
			if (validationError != null)
			{
				MessageBox.Show(
					(isKor ? "검증 오류: " : "Validation error: ") + validationError,
					isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			string error;
			if (!RecipeJsonHelper.SaveToJsonFile(_allRecipes, _jsonFilePath, out error))
			{
				MessageBox.Show(
					(isKor ? "저장 실패: " : "Save failed: ") + error,
					isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			return true;
		}

		#endregion

		#region TreeView Refresh

		void RefreshTree()
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			bool isQuick = _currentRecipe != null && _currentRecipe.recipe_mode == "quick";
			treeViewStructure.Nodes.Clear();
			if (_currentRecipe == null) return;

			// "(Direct)" 노드 = recipe 직속 step
			string directLabel = isQuick
				? (isKor ? "(Steps - Quick)" : "(Steps - Quick)")
				: (isKor ? "(Direct - 직속)" : "(Direct)");
			TreeNode directNode = new TreeNode(directLabel);
			directNode.Tag = NODE_TAG_DIRECT;
			directNode.ImageIndex = 0;

			// recipe 직속 step 추가
			ArrayList directSteps = GetSortedSteps(_currentRecipe.steps);
			for (int i = 0; i < directSteps.Count; i++)
			{
				var step = (RecipeStepData)directSteps[i];
				string stepLabel = isQuick
					? string.Format("[{0}] {1}  ({2} tags)", step.step_order, step.step_name, step.items.Count)
					: string.Format("[{0}] {1} [{2}]", step.step_order, step.step_name, step.items.Count);
				TreeNode stepNode = new TreeNode(stepLabel);
				stepNode.Tag = step;
				directNode.Nodes.Add(stepNode);
			}
			treeViewStructure.Nodes.Add(directNode);

			// Quick 모드에서는 Unit 노드 표시하지 않음
			if (!isQuick)
			{
				ArrayList sortedUnits = GetSortedUnits(_currentRecipe.units);
				for (int u = 0; u < sortedUnits.Count; u++)
				{
					var unit = (RecipeUnitData)sortedUnits[u];
					string unitLabel = string.Format("Unit {0}: {1}", unit.unit_order, unit.unit_name);
					if (!string.IsNullOrEmpty(unit.description))
						unitLabel += " - " + unit.description;

					TreeNode unitNode = new TreeNode(unitLabel);
					unitNode.Tag = unit;

					ArrayList unitSteps = GetSortedSteps(unit.steps);
					for (int s = 0; s < unitSteps.Count; s++)
					{
						var step = (RecipeStepData)unitSteps[s];
						string stepLabel = string.Format("[{0}] {1} [{2}]", step.step_order, step.step_name, step.items.Count);
						TreeNode stepNode = new TreeNode(stepLabel);
						stepNode.Tag = step;
						unitNode.Nodes.Add(stepNode);
					}

					treeViewStructure.Nodes.Add(unitNode);
				}
			}

			treeViewStructure.ExpandAll();
		}

		ArrayList GetSortedSteps(ArrayList steps)
		{
			ArrayList sorted = new ArrayList(steps);
			sorted.Sort(new StepOrderComparer());
			return sorted;
		}

		ArrayList GetSortedUnits(ArrayList units)
		{
			ArrayList sorted = new ArrayList(units);
			sorted.Sort(new UnitOrderComparer());
			return sorted;
		}

		class StepOrderComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				return ((RecipeStepData)x).step_order.CompareTo(((RecipeStepData)y).step_order);
			}
		}

		class UnitOrderComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				return ((RecipeUnitData)x).unit_order.CompareTo(((RecipeUnitData)y).unit_order);
			}
		}

		#endregion

		#region Step Preview (B-2)

		void treeViewStructure_AfterSelect(object sender, TreeViewEventArgs e)
		{
			ShowStepPreview(e.Node);
		}

		void ShowStepPreview(TreeNode node)
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			bool isQuick = _currentRecipe != null && _currentRecipe.recipe_mode == "quick";

			if (node == null || !(node.Tag is RecipeStepData))
			{
				labelPreviewTitle.Text = isKor ? "(스텝을 선택하세요)" : "(Select a step)";
				labelPreviewWait.Text = "";
				labelPreviewEntry.Text = "";
				labelPreviewExit.Text = "";
				labelPreviewRunning.Text = "";
				listViewPreviewItems.Items.Clear();
				return;
			}

			var step = (RecipeStepData)node.Tag;

			labelPreviewTitle.Text = string.Format("Step: [{0}] {1}", step.step_order, step.step_name);

			if (isQuick)
			{
				// Quick 모드: Tag-Value 목록만 표시 (Entry/Transition/Exit 불필요)
				labelPreviewWait.Text = isKor
					? string.Format("Quick 모드 — {0}개 태그 즉시 쓰기", step.items.Count)
					: string.Format("Quick mode — {0} tag(s) instant write", step.items.Count);
				labelPreviewEntry.Text = "";
				labelPreviewExit.Text = "";
				labelPreviewRunning.Text = "";
			}
			else
			{
				// Standard 모드: 전체 ISA-88 정보 표시
				labelPreviewWait.Text = string.Format(
					isKor ? "대기: {0}ms  타임아웃: {1}ms" : "Wait: {0}ms  Timeout: {1}ms",
					step.wait_time_ms, step.timeout_ms);

				// Entry
				if (!string.IsNullOrEmpty(step.entry_expression))
					labelPreviewEntry.Text = (isKor ? "시작: " : "Entry: ") + step.entry_expression;
				else
					labelPreviewEntry.Text = isKor ? "시작: (없음)" : "Entry: (none)";

				// Transitions (ISA-88 Full Model)
				if (!string.IsNullOrEmpty(step.transitions_json))
				{
					try
					{
						var transitions = JsonConvert.DeserializeObject<List<StepTransition>>(step.transitions_json);
						if (transitions != null && transitions.Count > 0)
						{
							transitions.Sort((a, b) => a.priority.CompareTo(b.priority));
							labelPreviewExit.Text = string.Format(
								isKor ? "전이: {0}개 정의" : "Transitions: {0} defined", transitions.Count);
							// 최우선순위 전이 표시
							var top = transitions[0];
							string typeStr = top.type.ToString();
							string expr = top.expression.Length > 40
								? top.expression.Substring(0, 37) + "..."
								: top.expression;
							labelPreviewRunning.Text = string.Format("[P{0}] {1}: {2}",
								top.priority, typeStr, expr);
						}
						else
						{
							labelPreviewExit.Text = isKor ? "전이: (없음)" : "Transitions: (none)";
							labelPreviewRunning.Text = "";
						}
					}
					catch
					{
						labelPreviewExit.Text = isKor ? "전이: (파싱 오류)" : "Transitions: (parse error)";
						labelPreviewRunning.Text = "";
					}
				}
				else
				{
					// Legacy fallback — 전이 형식으로 통일 표시
					int transCount = 0;
					string topInfo = "";

					if (!string.IsNullOrEmpty(step.running_expression))
					{
						transCount++;
						string expr = step.running_expression.Length > 40
							? step.running_expression.Substring(0, 37) + "..." : step.running_expression;
						topInfo = "[P0] Exception: !(" + expr + ")";
					}
					if (!string.IsNullOrEmpty(step.exit_expression))
					{
						transCount++;
						if (string.IsNullOrEmpty(topInfo))
						{
							string expr = step.exit_expression.Length > 40
								? step.exit_expression.Substring(0, 37) + "..." : step.exit_expression;
							topInfo = "[P10] Complete: " + expr;
						}
					}

					labelPreviewExit.Text = transCount > 0
						? string.Format(isKor ? "전이: {0}개 (레거시)" : "Transitions: {0} (legacy)", transCount)
						: (isKor ? "전이: (없음)" : "Transitions: (none)");
					labelPreviewRunning.Text = topInfo;
				}
			}

			// Items list
			listViewPreviewItems.Items.Clear();
			for (int i = 0; i < step.items.Count; i++)
			{
				var item = (RecipeItemData)step.items[i];
				ListViewItem lvi = new ListViewItem(item.tag_name);
				lvi.SubItems.Add(item.set_value);
				lvi.SubItems.Add(item.value_type);
				listViewPreviewItems.Items.Add(lvi);
			}
		}

		#endregion

		#region Mode Visibility (B-1)

		void comboBoxRecipeMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_currentRecipe == null) return;

			// 즉시 모델에 반영 (저장 전에도 UI가 모드에 맞게 표시되도록)
			_currentRecipe.recipe_mode = comboBoxRecipeMode.SelectedIndex == 1 ? "quick" : "standard";

			ApplyModeUI();
			RefreshTree();

			// 현재 선택된 스텝 미리보기 갱신
			if (treeViewStructure.SelectedNode != null)
				ShowStepPreview(treeViewStructure.SelectedNode);
		}

		void ApplyModeUI()
		{
			bool isQuick = comboBoxRecipeMode.SelectedIndex == 1;

			// Quick 모드 시 Unit 버튼 숨김
			btnAddUnit.Visible = !isQuick;
			btnDeleteUnit.Visible = !isQuick;
			toolStripSeparatorUnit.Visible = !isQuick;
		}

		#endregion

		#region Recipe CRUD

		void btnAddRecipe_Click(object sender, EventArgs e)
		{
			_currentRecipe = new RecipeData();
			_currentRecipe.recipe_name = "NewRecipe";
			_currentRecipe.recipe_code = "";  // 하위호환 — 빈 문자열
			_currentRecipe.description = "";
			_currentRecipe.recipe_guid = Guid.NewGuid().ToString("N");
			_currentRecipe.version = 0;  // 첫 저장 시 version++ → 1이 됨
			_currentRecipe.status = "draft";
			_currentRecipe.is_active = true;

			textBoxRecipeName.Text = _currentRecipe.recipe_name;
			textBoxDescription.Text = "";
			comboBoxRecipeMode.SelectedIndex = 0; // Standard

			// _allRecipes에 추가
			_allRecipes.Add(_currentRecipe);

			// ListView 갱신
			bool isKor = NetTools.Tools.IsLangKorean();
			int idx = _allRecipes.Count - 1;
			ListViewItem lvi = new ListViewItem(_currentRecipe.recipe_name);
			lvi.SubItems.Add(_currentRecipe.version.ToString());
			lvi.SubItems.Add(_currentRecipe.description);
			lvi.Tag = idx;
			listViewRecipes.Items.Add(lvi);

			// 새 항목 선택
			lvi.Selected = true;
			lvi.EnsureVisible();

			RefreshTree();
		}

		void btnDeleteRecipe_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			if (_currentRecipe == null) return;

			// _allRecipes에 존재하는지 확인
			int recipeIndex = _allRecipes.IndexOf(_currentRecipe);
			if (recipeIndex < 0) return;

			if (MessageBox.Show(
				string.Format(isKor ? "레시피 '{0}'을(를) 삭제하시겠습니까?" : "Delete recipe '{0}'?", _currentRecipe.recipe_name),
				isKor ? "확인" : "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			_allRecipes.RemoveAt(recipeIndex);
			_currentRecipe = null;

			// JSON 저장
			if (!SaveAllRecipesToJson())
			{
				// 저장 실패 시 다시 로드
				LoadRecipeList();
				return;
			}

			LoadRecipeList();
		}

		void btnSave_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			if (_currentRecipe == null) return;

			if (string.IsNullOrEmpty(textBoxRecipeName.Text.Trim()))
			{
				MessageBox.Show(
					isKor ? "레시피 이름을 입력하세요." : "Please enter the recipe name.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string newName = textBoxRecipeName.Text.Trim();

			// 이름 중복 검사 (다른 레시피와)
			for (int i = 0; i < _allRecipes.Count; i++)
			{
				if (_allRecipes[i] != _currentRecipe &&
					string.Compare(_allRecipes[i].recipe_name, newName, true) == 0)
				{
					MessageBox.Show(
						isKor ? "같은 이름의 레시피가 이미 존재합니다." : "A recipe with the same name already exists.",
						isKor ? "중복 오류" : "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
			}

			_currentRecipe.recipe_name = newName;
			_currentRecipe.recipe_code = "";  // 하위호환 — 항상 빈 문자열
			_currentRecipe.description = textBoxDescription.Text.Trim();
			_currentRecipe.recipe_mode = comboBoxRecipeMode.SelectedIndex == 1 ? "quick" : "standard";

			// GUID 없으면 자동 생성 (CSV import 등으로 만들어진 경우)
			if (string.IsNullOrEmpty(_currentRecipe.recipe_guid))
				_currentRecipe.recipe_guid = Guid.NewGuid().ToString("N");

			// 저장 시 version 자동 증가
			_currentRecipe.version++;

			// JSON 저장
			if (!SaveAllRecipesToJson())
			{
				// 저장 실패 시 version 원복
				_currentRecipe.version--;
				return;
			}

			MessageBox.Show(
				string.Format(isKor ? "저장되었습니다. (Version: {0})" : "Saved successfully. (Version: {0})", _currentRecipe.version),
				isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);

			// ListView만 갱신 (선택 유지)
			RefreshListView();
		}

		/// <summary>
		/// ListView 항목만 갱신 (선택 상태 유지)
		/// </summary>
		void RefreshListView()
		{
			listViewRecipes.BeginUpdate();
			listViewRecipes.Items.Clear();

			for (int i = 0; i < _allRecipes.Count; i++)
			{
				var recipe = _allRecipes[i];
				ListViewItem lvi = new ListViewItem(recipe.recipe_name);
				lvi.SubItems.Add(recipe.version.ToString());
				lvi.SubItems.Add(recipe.description);
				lvi.Tag = i;

				if (_currentRecipe != null && recipe == _currentRecipe)
					lvi.Selected = true;

				listViewRecipes.Items.Add(lvi);
			}

			listViewRecipes.EndUpdate();
		}

		#endregion

		#region Unit CRUD

		void btnAddUnit_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			if (_currentRecipe == null) return;

			string unitName = ShowInputBox(
				isKor ? "Unit 이름을 입력하세요:" : "Enter unit name:",
				isKor ? "Unit 추가" : "Add Unit", "");
			if (string.IsNullOrEmpty(unitName)) return;

			// 중복 이름 검사
			for (int i = 0; i < _currentRecipe.units.Count; i++)
			{
				if (string.Compare(((RecipeUnitData)_currentRecipe.units[i]).unit_name, unitName, true) == 0)
				{
					MessageBox.Show(
						isKor ? "이미 존재하는 Unit 이름입니다." : "Unit name already exists.",
						isKor ? "중복 오류" : "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
			}

			RecipeUnitData newUnit = new RecipeUnitData();
			newUnit.unit_name = unitName;
			newUnit.unit_order = _currentRecipe.units.Count + 1;
			_currentRecipe.units.Add(newUnit);

			RefreshTree();
		}

		void btnDeleteUnit_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			if (_currentRecipe == null) return;

			TreeNode selNode = treeViewStructure.SelectedNode;
			if (selNode == null || !(selNode.Tag is RecipeUnitData)) return;

			var unit = (RecipeUnitData)selNode.Tag;

			if (MessageBox.Show(
				string.Format(isKor ? "Unit '{0}'을(를) 삭제하시겠습니까?\n하위 Step도 모두 삭제됩니다." : "Delete unit '{0}'?\nAll steps under this unit will also be deleted.", unit.unit_name),
				isKor ? "확인" : "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			_currentRecipe.units.Remove(unit);

			// unit_order 재정렬
			for (int i = 0; i < _currentRecipe.units.Count; i++)
			{
				((RecipeUnitData)_currentRecipe.units[i]).unit_order = i + 1;
			}

			RefreshTree();
		}

		#endregion

		#region Step CRUD

		void btnAddStep_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			if (_currentRecipe == null) return;

			// 트리에서 선택된 노드를 기반으로 대상 결정
			TreeNode selNode = treeViewStructure.SelectedNode;
			ArrayList targetSteps = null;
			RecipeUnitData targetUnit = null;

			if (selNode == null)
			{
				// 기본: recipe 직속
				targetSteps = _currentRecipe.steps;
			}
			else if (selNode.Tag is string && (string)selNode.Tag == NODE_TAG_DIRECT)
			{
				targetSteps = _currentRecipe.steps;
			}
			else if (selNode.Tag is RecipeUnitData)
			{
				targetUnit = (RecipeUnitData)selNode.Tag;
				targetSteps = targetUnit.steps;
			}
			else if (selNode.Tag is RecipeStepData)
			{
				// step 노드 선택 → 부모 확인
				TreeNode parentNode = selNode.Parent;
				if (parentNode != null && parentNode.Tag is RecipeUnitData)
				{
					targetUnit = (RecipeUnitData)parentNode.Tag;
					targetSteps = targetUnit.steps;
				}
				else
				{
					targetSteps = _currentRecipe.steps;
				}
			}
			else
			{
				targetSteps = _currentRecipe.steps;
			}

			RecipeStepData newStep = new RecipeStepData();
			newStep.step_order = targetSteps.Count + 1;
			newStep.step_name = "Step " + newStep.step_order;
			newStep.timeout_ms = 30000;
			if (targetUnit != null)
				newStep.unit_id = targetUnit.unit_id;

			string currentMode = _currentRecipe != null ? (_currentRecipe.recipe_mode ?? "standard") : "standard";
			FormConfigRecipeStep dialog = new FormConfigRecipeStep(newStep, currentMode);
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				targetSteps.Add(dialog.StepData);
				RefreshTree();
			}
		}

		void btnEditStep_Click(object sender, EventArgs e)
		{
			EditSelectedStep();
		}

		void treeViewStructure_DoubleClick(object sender, EventArgs e)
		{
			EditSelectedStep();
		}

		void EditSelectedStep()
		{
			if (_currentRecipe == null) return;

			TreeNode selNode = treeViewStructure.SelectedNode;
			if (selNode == null || !(selNode.Tag is RecipeStepData)) return;

			var step = (RecipeStepData)selNode.Tag;

			// 소속 step 리스트 찾기
			ArrayList ownerSteps = FindOwnerSteps(step);
			if (ownerSteps == null) return;

			string currentMode = _currentRecipe.recipe_mode ?? "standard";
			FormConfigRecipeStep dialog = new FormConfigRecipeStep(step, currentMode);
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				int idx = ownerSteps.IndexOf(step);
				if (idx >= 0)
				{
					ownerSteps[idx] = dialog.StepData;
				}
				RefreshTree();
			}
		}

		void btnDeleteStep_Click(object sender, EventArgs e)
		{
			if (_currentRecipe == null) return;

			TreeNode selNode = treeViewStructure.SelectedNode;
			if (selNode == null || !(selNode.Tag is RecipeStepData)) return;

			var step = (RecipeStepData)selNode.Tag;

			ArrayList ownerSteps = FindOwnerSteps(step);
			if (ownerSteps == null) return;

			ownerSteps.Remove(step);

			// step_order 재정렬
			for (int i = 0; i < ownerSteps.Count; i++)
			{
				((RecipeStepData)ownerSteps[i]).step_order = i + 1;
			}

			RefreshTree();
		}

		/// <summary>
		/// step이 속한 ArrayList 찾기 (recipe 직속 또는 unit 소속)
		/// </summary>
		ArrayList FindOwnerSteps(RecipeStepData step)
		{
			if (_currentRecipe == null) return null;

			// recipe 직속 검색
			if (_currentRecipe.steps.Contains(step))
				return _currentRecipe.steps;

			// unit 소속 검색
			for (int u = 0; u < _currentRecipe.units.Count; u++)
			{
				var unit = (RecipeUnitData)_currentRecipe.units[u];
				if (unit.steps.Contains(step))
					return unit.steps;
			}

			return null;
		}

		#endregion

		#region Status Display & Approve/Obsolete

		void UpdateStatusDisplay()
		{
			if (_currentRecipe == null)
			{
				labelStatusValue.Text = "";
				return;
			}

			string status = _currentRecipe.status ?? "draft";
			labelStatusValue.Text = status.ToUpper();

			switch (status.ToLower())
			{
				case "approved":
					labelStatusValue.ForeColor = Color.Green;
					break;
				case "obsolete":
					labelStatusValue.ForeColor = Color.Gray;
					break;
				default: // draft
					labelStatusValue.ForeColor = Color.OrangeRed;
					break;
			}

		}

		#endregion

		#region CSV Export/Import

		void btnExportCsv_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			if (_currentRecipe == null)
			{
				MessageBox.Show(
					isKor ? "내보낼 레시피를 선택하세요." : "Please select a recipe to export.",
					isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "CSV Files (*.csv)|*.csv";
			dlg.FileName = _currentRecipe.recipe_name + ".csv";

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string error;
				if (RecipeCsvHelper.ExportToCsv(_currentRecipe, dlg.FileName, out error))
				{
					MessageBox.Show(
						isKor ? "내보내기 완료." : "Export completed.",
						isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(
						(isKor ? "내보내기 실패: " : "Export failed: ") + error,
						isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		void btnImportCsv_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "CSV Files (*.csv)|*.csv";

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string error;
				RecipeData imported = RecipeCsvHelper.ImportFromCsv(dlg.FileName, out error);
				if (imported == null)
				{
					MessageBox.Show(
						(isKor ? "가져오기 실패: " : "Import failed: ") + error,
						isKor ? "오류" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// CSV에서 가져온 레시피에 GUID 자동 배정
				if (string.IsNullOrEmpty(imported.recipe_guid))
					imported.recipe_guid = Guid.NewGuid().ToString("N");
				if (imported.version <= 0)
					imported.version = 1;
				imported.is_active = true;

				// 이름 중복 검사
				bool nameExists = false;
				int existingIndex = -1;
				for (int i = 0; i < _allRecipes.Count; i++)
				{
					if (string.Compare(_allRecipes[i].recipe_name, imported.recipe_name, true) == 0)
					{
						nameExists = true;
						existingIndex = i;
						break;
					}
				}

				if (nameExists)
				{
					DialogResult dr = MessageBox.Show(
						string.Format(isKor
							? "같은 이름의 레시피 '{0}'이(가) 이미 존재합니다.\n덮어쓰시겠습니까?"
							: "Recipe '{0}' already exists.\nOverwrite?", imported.recipe_name),
						isKor ? "확인" : "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

					if (dr == DialogResult.Yes)
					{
						// 기존 GUID 유지, version 증가
						imported.recipe_guid = _allRecipes[existingIndex].recipe_guid;
						imported.version = _allRecipes[existingIndex].version + 1;
						_allRecipes[existingIndex] = imported;
						_currentRecipe = imported;
					}
					else
					{
						return;
					}
				}
				else
				{
					_allRecipes.Add(imported);
					_currentRecipe = imported;
				}

				textBoxRecipeName.Text = imported.recipe_name;
				textBoxDescription.Text = imported.description;

				// Import 후 자동 저장
				if (SaveAllRecipesToJson())
				{
					LoadRecipeList();
					// 가져온 레시피 자동 선택
					for (int i = 0; i < listViewRecipes.Items.Count; i++)
					{
						if (listViewRecipes.Items[i].Text == imported.recipe_name)
						{
							listViewRecipes.Items[i].Selected = true;
							break;
						}
					}
					MessageBox.Show(
						isKor ? "가져오기 및 저장 완료." : "Import and save completed.",
						isKor ? "알림" : "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					RefreshTree();
					MessageBox.Show(
						isKor ? "가져오기 완료. 저장에 실패했습니다. 수동으로 저장해 주세요." : "Import done but save failed. Please save manually.",
						isKor ? "경고" : "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		#endregion

		#region InputBox Helper

		static string ShowInputBox(string prompt, string title, string defaultValue)
		{
			Form inputForm = new Form();
			inputForm.Width = 350;
			inputForm.Height = 150;
			inputForm.Text = title;
			inputForm.StartPosition = FormStartPosition.CenterParent;
			inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
			inputForm.MaximizeBox = false;
			inputForm.MinimizeBox = false;

			Label lbl = new Label() { Left = 10, Top = 10, Text = prompt, AutoSize = true };
			TextBox txt = new TextBox() { Left = 10, Top = 35, Width = 310, Text = defaultValue };
			Button btnOk = new Button() { Text = "OK", Left = 160, Top = 70, Width = 75, DialogResult = DialogResult.OK };
			Button btnCancel = new Button() { Text = "Cancel", Left = 245, Top = 70, Width = 75, DialogResult = DialogResult.Cancel };

			inputForm.Controls.Add(lbl);
			inputForm.Controls.Add(txt);
			inputForm.Controls.Add(btnOk);
			inputForm.Controls.Add(btnCancel);
			inputForm.AcceptButton = btnOk;
			inputForm.CancelButton = btnCancel;

			if (inputForm.ShowDialog() == DialogResult.OK)
				return txt.Text.Trim();
			return null;
		}

		#endregion

		/// <summary>
		/// Studio에서 호출하는 정적 메서드 (Static method pattern)
		/// </summary>
		public static void ConfigRecipe()
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			// VIEW 권한 체크
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN &&
				!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_RECIPE_VIEW))
			{
				MessageBox.Show(
					isKor ? "레시피 설정 열람 권한이 없습니다." : "No permission to view recipe configuration.",
					isKor ? "권한 오류" : "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			FormConfigRecipe dialog = new FormConfigRecipe();
			dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog();
		}
	}
}
