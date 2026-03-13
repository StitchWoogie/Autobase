using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;
using DialogTag;
using Newtonsoft.Json;

namespace PublicStudioLocalMain.Recipe
{
	/// <summary>
	/// 레시피 Step 편집 다이얼로그 (ISA-88 Full Transition Model)
	/// </summary>
	public class FormConfigRecipeStep : System.Windows.Forms.Form
	{
        public FormConfigRecipeStep(RecipeStepData stepData, string recipeMode = "standard")
        {
            InitializeComponent();
            PostInitializeComponent();
            _recipeMode = (recipeMode ?? "standard").ToLower();

            ApplyLocalization();
            StepData = stepData.Clone();
            LoadStepData();
            ApplyModeVisibility();
        }

        private System.ComponentModel.IContainer components = null;

		// 기본 정보
		private System.Windows.Forms.Label labelStepName;
		private System.Windows.Forms.TextBox textBoxStepName;
		private System.Windows.Forms.Label labelWaitTime;
		private System.Windows.Forms.NumericUpDown numericWaitTime;
		private System.Windows.Forms.Label labelTimeout;
		private System.Windows.Forms.NumericUpDown numericTimeout;

		// 시작 조건 (Entry Condition) — 조건식 전용
		private System.Windows.Forms.GroupBox groupBoxEntryCondition;
		private System.Windows.Forms.TextBox textBoxEntryExpression;
		private System.Windows.Forms.Button buttonEntryTagSelect;
		private System.Windows.Forms.Button buttonEntryExprValidate;
		private System.Windows.Forms.Label labelEntryExprResult;

		// Transitions (ISA-88 Full Transition Model)
		private System.Windows.Forms.GroupBox groupBoxTransitions;
		private System.Windows.Forms.DataGridView dataGridTransitions;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransPriority;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransExpression;
		private System.Windows.Forms.DataGridViewButtonColumn colTransSelectTag;
		private System.Windows.Forms.DataGridViewComboBoxColumn colTransType;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransTarget;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransMaxLoop;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransTimeout;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTransDescription;

		// Exit Actions (정상 종료 시 액션)
		private System.Windows.Forms.GroupBox groupBoxExitActions;
		private System.Windows.Forms.TextBox textBoxExitActions;

		// Abort Actions (중단 시 액션)
		private System.Windows.Forms.GroupBox groupBoxAbortActions;
		private System.Windows.Forms.TextBox textBoxAbortActions;

		// Tag-Value Items
		private System.Windows.Forms.DataGridView dataGridItems;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTagName;
		private System.Windows.Forms.DataGridViewButtonColumn colSelectTag;
		private System.Windows.Forms.DataGridViewTextBoxColumn colSetValue;
		private System.Windows.Forms.DataGridViewComboBoxColumn colValueType;
		private System.Windows.Forms.DataGridViewTextBoxColumn colItemOrder;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBoxItems;
        private DataGridViewTextBoxColumn Priority;
        private DataGridViewTextBoxColumn Expression;
        private DataGridViewButtonColumn TransSelectTag;
        private DataGridViewComboBoxColumn TransType;
        private DataGridViewTextBoxColumn TargetStep;
        private DataGridViewTextBoxColumn MaxLoop;
        private DataGridViewTextBoxColumn TimeoutMs;
        private DataGridViewTextBoxColumn Description;
        private DataGridViewTextBoxColumn TagName;
        private DataGridViewButtonColumn SelectTag;
        private DataGridViewTextBoxColumn SetValue;
        private DataGridViewComboBoxColumn ValueType;
        private DataGridViewTextBoxColumn ItemOrder;

        public RecipeStepData StepData { get; private set; }
		private string _recipeMode = "standard";



        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        static void SetPlaceholder(TextBox tb, string text)
        {
            SendMessage(tb.Handle, EM_SETCUEBANNER, (IntPtr)1, text);
        }


        /// <summary>
        /// 다국어 텍스트 적용 (InitializeComponent 이후 호출)
        /// </summary>
        void ApplyLocalization()
		{
			if (!NetTools.Tools.IsLangKorean()) return;

			// Form
			this.Text = "Step 편집";

			// Row 1: 기본 정보
			labelStepName.Text = "Step 이름:";
			labelWaitTime.Text = "대기(ms):";
			labelTimeout.Text = "타임아웃(ms):";

			// Entry Condition
			groupBoxEntryCondition.Text = "시작 조건 (Entry Condition)";
			buttonEntryExprValidate.Text = "검증";

			// Transitions
			groupBoxTransitions.Text = "전이 조건 (Transitions)";
			colTransPriority.HeaderText = "우선순위";
			colTransPriority.ToolTipText = "0이 가장 높은 우선순위";
			colTransExpression.HeaderText = "조건식";
			colTransType.HeaderText = "타입";
			colTransTarget.HeaderText = "대상Step";
			colTransMaxLoop.HeaderText = "최대반복";
			colTransTimeout.HeaderText = "타임아웃(ms)";
			colTransTimeout.ToolTipText = "0 = 조건식만, >0 = N ms 후 자동 발동";
			colTransDescription.HeaderText = "설명";

			// Exit / Abort Actions
			groupBoxExitActions.Text = "종료 액션 (Exit Actions)";
			groupBoxAbortActions.Text = "중단 액션 (Abort Actions)";
			SetPlaceholder(textBoxExitActions, "TAG=값;TAG=값  예: $AO_VALVE=0;$DO_PUMP=0");
			SetPlaceholder(textBoxAbortActions, "TAG=값;TAG=값  예: $AO_VALVE=0;$DO_PUMP=0");

			// Items
			groupBoxItems.Text = "태그-값 항목";
			colTagName.HeaderText = "태그 이름";
			colSetValue.HeaderText = "설정 값";
			colValueType.HeaderText = "값 타입";
			colItemOrder.HeaderText = "순서";

			// Buttons
			buttonOK.Text = "확인";
			buttonCancel.Text = "취소";
		}

		/// <summary>
		/// Quick 모드 시 조건 UI 숨김
		/// </summary>
		void ApplyModeVisibility()
		{
			if (_recipeMode == "quick")
			{
				groupBoxEntryCondition.Visible = false;
				groupBoxTransitions.Visible = false;
				groupBoxExitActions.Visible = false;
				groupBoxAbortActions.Visible = false;
				labelWaitTime.Visible = false;
				numericWaitTime.Visible = false;
				labelTimeout.Visible = false;
				numericTimeout.Visible = false;
				groupBoxItems.Location = new Point(15, 50);
				groupBoxItems.Size = new Size(this.ClientSize.Width - 30, this.ClientSize.Height - 100);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
            this.labelStepName = new System.Windows.Forms.Label();
            this.textBoxStepName = new System.Windows.Forms.TextBox();
            this.labelWaitTime = new System.Windows.Forms.Label();
            this.numericWaitTime = new System.Windows.Forms.NumericUpDown();
            this.labelTimeout = new System.Windows.Forms.Label();
            this.numericTimeout = new System.Windows.Forms.NumericUpDown();
            this.groupBoxEntryCondition = new System.Windows.Forms.GroupBox();
            this.textBoxEntryExpression = new System.Windows.Forms.TextBox();
            this.buttonEntryTagSelect = new System.Windows.Forms.Button();
            this.buttonEntryExprValidate = new System.Windows.Forms.Button();
            this.labelEntryExprResult = new System.Windows.Forms.Label();
            this.groupBoxTransitions = new System.Windows.Forms.GroupBox();
            this.dataGridTransitions = new System.Windows.Forms.DataGridView();
            this.Priority = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Expression = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransSelectTag = new System.Windows.Forms.DataGridViewButtonColumn();
            this.TransType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.TargetStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxLoop = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeoutMs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxExitActions = new System.Windows.Forms.GroupBox();
            this.textBoxExitActions = new System.Windows.Forms.TextBox();
            this.groupBoxAbortActions = new System.Windows.Forms.GroupBox();
            this.textBoxAbortActions = new System.Windows.Forms.TextBox();
            this.groupBoxItems = new System.Windows.Forms.GroupBox();
            this.dataGridItems = new System.Windows.Forms.DataGridView();
            this.TagName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SelectTag = new System.Windows.Forms.DataGridViewButtonColumn();
            this.SetValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ItemOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericWaitTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTimeout)).BeginInit();
            this.groupBoxEntryCondition.SuspendLayout();
            this.groupBoxTransitions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTransitions)).BeginInit();
            this.groupBoxExitActions.SuspendLayout();
            this.groupBoxAbortActions.SuspendLayout();
            this.groupBoxItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridItems)).BeginInit();
            this.SuspendLayout();
            // 
            // labelStepName
            // 
            this.labelStepName.AutoSize = true;
            this.labelStepName.Location = new System.Drawing.Point(15, 18);
            this.labelStepName.Name = "labelStepName";
            this.labelStepName.Size = new System.Drawing.Size(72, 12);
            this.labelStepName.TabIndex = 0;
            this.labelStepName.Text = "Step Name:";
            // 
            // textBoxStepName
            // 
            this.textBoxStepName.Location = new System.Drawing.Point(100, 15);
            this.textBoxStepName.Name = "textBoxStepName";
            this.textBoxStepName.Size = new System.Drawing.Size(200, 21);
            this.textBoxStepName.TabIndex = 1;
            // 
            // labelWaitTime
            // 
            this.labelWaitTime.AutoSize = true;
            this.labelWaitTime.Location = new System.Drawing.Point(320, 18);
            this.labelWaitTime.Name = "labelWaitTime";
            this.labelWaitTime.Size = new System.Drawing.Size(60, 12);
            this.labelWaitTime.TabIndex = 2;
            this.labelWaitTime.Text = "Wait(ms):";
            // 
            // numericWaitTime
            // 
            this.numericWaitTime.Location = new System.Drawing.Point(400, 15);
            this.numericWaitTime.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.numericWaitTime.Name = "numericWaitTime";
            this.numericWaitTime.Size = new System.Drawing.Size(90, 21);
            this.numericWaitTime.TabIndex = 3;
            // 
            // labelTimeout
            // 
            this.labelTimeout.AutoSize = true;
            this.labelTimeout.Location = new System.Drawing.Point(510, 18);
            this.labelTimeout.Name = "labelTimeout";
            this.labelTimeout.Size = new System.Drawing.Size(83, 12);
            this.labelTimeout.TabIndex = 4;
            this.labelTimeout.Text = "Timeout(ms):";
            // 
            // numericTimeout
            // 
            this.numericTimeout.Location = new System.Drawing.Point(610, 15);
            this.numericTimeout.Maximum = new decimal(new int[] {
            600000,
            0,
            0,
            0});
            this.numericTimeout.Name = "numericTimeout";
            this.numericTimeout.Size = new System.Drawing.Size(90, 21);
            this.numericTimeout.TabIndex = 5;
            this.numericTimeout.Value = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            // 
            // groupBoxEntryCondition
            // 
            this.groupBoxEntryCondition.Controls.Add(this.textBoxEntryExpression);
            this.groupBoxEntryCondition.Controls.Add(this.buttonEntryTagSelect);
            this.groupBoxEntryCondition.Controls.Add(this.buttonEntryExprValidate);
            this.groupBoxEntryCondition.Controls.Add(this.labelEntryExprResult);
            this.groupBoxEntryCondition.Location = new System.Drawing.Point(15, 50);
            this.groupBoxEntryCondition.Name = "groupBoxEntryCondition";
            this.groupBoxEntryCondition.Size = new System.Drawing.Size(710, 48);
            this.groupBoxEntryCondition.TabIndex = 6;
            this.groupBoxEntryCondition.TabStop = false;
            this.groupBoxEntryCondition.Text = "Entry Condition";
            // 
            // textBoxEntryExpression
            // 
            this.textBoxEntryExpression.Location = new System.Drawing.Point(10, 20);
            this.textBoxEntryExpression.Name = "textBoxEntryExpression";
            this.textBoxEntryExpression.Size = new System.Drawing.Size(510, 21);
            this.textBoxEntryExpression.TabIndex = 0;
            // 
            // buttonEntryTagSelect
            // 
            this.buttonEntryTagSelect.Location = new System.Drawing.Point(525, 19);
            this.buttonEntryTagSelect.Name = "buttonEntryTagSelect";
            this.buttonEntryTagSelect.Size = new System.Drawing.Size(30, 23);
            this.buttonEntryTagSelect.TabIndex = 1;
            this.buttonEntryTagSelect.Text = "...";
            this.buttonEntryTagSelect.Click += new System.EventHandler(this.buttonEntryTagSelect_Click);
            // 
            // buttonEntryExprValidate
            // 
            this.buttonEntryExprValidate.Location = new System.Drawing.Point(560, 19);
            this.buttonEntryExprValidate.Name = "buttonEntryExprValidate";
            this.buttonEntryExprValidate.Size = new System.Drawing.Size(60, 23);
            this.buttonEntryExprValidate.TabIndex = 2;
            this.buttonEntryExprValidate.Text = "Validate";
            this.buttonEntryExprValidate.Click += new System.EventHandler(this.buttonEntryExprValidate_Click);
            // 
            // labelEntryExprResult
            // 
            this.labelEntryExprResult.AutoSize = true;
            this.labelEntryExprResult.ForeColor = System.Drawing.Color.Green;
            this.labelEntryExprResult.Location = new System.Drawing.Point(625, 22);
            this.labelEntryExprResult.Name = "labelEntryExprResult";
            this.labelEntryExprResult.Size = new System.Drawing.Size(0, 12);
            this.labelEntryExprResult.TabIndex = 3;
            // 
            // groupBoxTransitions
            // 
            this.groupBoxTransitions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTransitions.Controls.Add(this.dataGridTransitions);
            this.groupBoxTransitions.Location = new System.Drawing.Point(15, 288);
            this.groupBoxTransitions.Name = "groupBoxTransitions";
            this.groupBoxTransitions.Size = new System.Drawing.Size(710, 180);
            this.groupBoxTransitions.TabIndex = 8;
            this.groupBoxTransitions.TabStop = false;
            this.groupBoxTransitions.Text = "Transitions";
            // 
            // dataGridTransitions
            // 
            this.dataGridTransitions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridTransitions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Priority,
            this.Expression,
            this.TransSelectTag,
            this.TransType,
            this.TargetStep,
            this.MaxLoop,
            this.TimeoutMs,
            this.Description});
            this.dataGridTransitions.Location = new System.Drawing.Point(10, 20);
            this.dataGridTransitions.Name = "dataGridTransitions";
            this.dataGridTransitions.Size = new System.Drawing.Size(690, 150);
            this.dataGridTransitions.TabIndex = 0;
            this.dataGridTransitions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridTransitions_CellClick);
            this.dataGridTransitions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridTransitions_CellValueChanged);
            this.dataGridTransitions.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGrid_DataError);
            // 
            // Priority
            // 
            this.Priority.HeaderText = "Priority";
            this.Priority.Name = "Priority";
            this.Priority.ToolTipText = "0 = highest priority";
            this.Priority.Width = 60;
            // 
            // Expression
            // 
            this.Expression.HeaderText = "Expression";
            this.Expression.Name = "Expression";
            this.Expression.Width = 220;
            // 
            // TransSelectTag
            // 
            this.TransSelectTag.HeaderText = "";
            this.TransSelectTag.Name = "TransSelectTag";
            this.TransSelectTag.Text = "...";
            this.TransSelectTag.UseColumnTextForButtonValue = true;
            this.TransSelectTag.Width = 30;
            // 
            // TransType
            // 
            this.TransType.HeaderText = "Type";
            this.TransType.Items.AddRange(new object[] {
            "Complete",
            "Exception",
            "Abort",
            "Loop",
            "End"});
            this.TransType.Name = "TransType";
            this.TransType.Width = 90;
            // 
            // TargetStep
            // 
            this.TargetStep.HeaderText = "Target";
            this.TargetStep.Name = "TargetStep";
            this.TargetStep.Width = 60;
            // 
            // MaxLoop
            // 
            this.MaxLoop.HeaderText = "MaxLoop";
            this.MaxLoop.Name = "MaxLoop";
            this.MaxLoop.Width = 55;
            // 
            // TimeoutMs
            // 
            this.TimeoutMs.HeaderText = "Timeout(ms)";
            this.TimeoutMs.Name = "TimeoutMs";
            this.TimeoutMs.ToolTipText = "0 = expression only, >0 = auto-trigger after N ms";
            this.TimeoutMs.Width = 75;
            // 
            // Description
            // 
            this.Description.HeaderText = "Desc";
            this.Description.Name = "Description";
            this.Description.Width = 120;
            // 
            // groupBoxExitActions
            // 
            this.groupBoxExitActions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxExitActions.Controls.Add(this.textBoxExitActions);
            this.groupBoxExitActions.Location = new System.Drawing.Point(15, 473);
            this.groupBoxExitActions.Name = "groupBoxExitActions";
            this.groupBoxExitActions.Size = new System.Drawing.Size(345, 48);
            this.groupBoxExitActions.TabIndex = 9;
            this.groupBoxExitActions.TabStop = false;
            this.groupBoxExitActions.Text = "Exit Actions";
            // 
            // textBoxExitActions
            // 
            this.textBoxExitActions.Location = new System.Drawing.Point(10, 20);
            this.textBoxExitActions.Name = "textBoxExitActions";
            this.textBoxExitActions.Size = new System.Drawing.Size(325, 21);
            this.textBoxExitActions.TabIndex = 0;
            // 
            // groupBoxAbortActions
            // 
            this.groupBoxAbortActions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxAbortActions.Controls.Add(this.textBoxAbortActions);
            this.groupBoxAbortActions.Location = new System.Drawing.Point(370, 473);
            this.groupBoxAbortActions.Name = "groupBoxAbortActions";
            this.groupBoxAbortActions.Size = new System.Drawing.Size(355, 48);
            this.groupBoxAbortActions.TabIndex = 10;
            this.groupBoxAbortActions.TabStop = false;
            this.groupBoxAbortActions.Text = "Abort Actions";
            // 
            // textBoxAbortActions
            // 
            this.textBoxAbortActions.Location = new System.Drawing.Point(10, 20);
            this.textBoxAbortActions.Name = "textBoxAbortActions";
            this.textBoxAbortActions.Size = new System.Drawing.Size(335, 21);
            this.textBoxAbortActions.TabIndex = 0;
            // 
            // groupBoxItems
            // 
            this.groupBoxItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxItems.Controls.Add(this.dataGridItems);
            this.groupBoxItems.Location = new System.Drawing.Point(15, 103);
            this.groupBoxItems.Name = "groupBoxItems";
            this.groupBoxItems.Size = new System.Drawing.Size(710, 180);
            this.groupBoxItems.TabIndex = 7;
            this.groupBoxItems.TabStop = false;
            this.groupBoxItems.Text = "Tag-Value Items";
            // 
            // dataGridItems
            // 
            this.dataGridItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TagName,
            this.SelectTag,
            this.SetValue,
            this.ValueType,
            this.ItemOrder});
            this.dataGridItems.Location = new System.Drawing.Point(10, 20);
            this.dataGridItems.Name = "dataGridItems";
            this.dataGridItems.Size = new System.Drawing.Size(690, 150);
            this.dataGridItems.TabIndex = 0;
            this.dataGridItems.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridItems_CellClick);
            this.dataGridItems.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGrid_DataError);
            // 
            // TagName
            // 
            this.TagName.HeaderText = "Tag Name";
            this.TagName.Name = "TagName";
            this.TagName.Width = 200;
            // 
            // SelectTag
            // 
            this.SelectTag.HeaderText = "";
            this.SelectTag.Name = "SelectTag";
            this.SelectTag.Text = "...";
            this.SelectTag.UseColumnTextForButtonValue = true;
            this.SelectTag.Width = 35;
            // 
            // SetValue
            // 
            this.SetValue.HeaderText = "Set Value";
            this.SetValue.Name = "SetValue";
            this.SetValue.Width = 150;
            // 
            // ValueType
            // 
            this.ValueType.HeaderText = "Value Type";
            this.ValueType.Items.AddRange(new object[] {
            "double",
            "int",
            "string",
            "bool"});
            this.ValueType.Name = "ValueType";
            // 
            // ItemOrder
            // 
            this.ItemOrder.HeaderText = "Order";
            this.ItemOrder.Name = "ItemOrder";
            this.ItemOrder.Width = 60;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(560, 531);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 28);
            this.buttonOK.TabIndex = 11;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(645, 531);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 28);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            // 
            // FormConfigRecipeStep
            // 
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(740, 571);
            this.Controls.Add(this.labelStepName);
            this.Controls.Add(this.textBoxStepName);
            this.Controls.Add(this.labelWaitTime);
            this.Controls.Add(this.numericWaitTime);
            this.Controls.Add(this.labelTimeout);
            this.Controls.Add(this.numericTimeout);
            this.Controls.Add(this.groupBoxEntryCondition);
            this.Controls.Add(this.groupBoxItems);
            this.Controls.Add(this.groupBoxTransitions);
            this.Controls.Add(this.groupBoxExitActions);
            this.Controls.Add(this.groupBoxAbortActions);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(756, 600);
            this.Name = "FormConfigRecipeStep";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Step";
            ((System.ComponentModel.ISupportInitialize)(this.numericWaitTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericTimeout)).EndInit();
            this.groupBoxEntryCondition.ResumeLayout(false);
            this.groupBoxEntryCondition.PerformLayout();
            this.groupBoxTransitions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTransitions)).EndInit();
            this.groupBoxExitActions.ResumeLayout(false);
            this.groupBoxExitActions.PerformLayout();
            this.groupBoxAbortActions.ResumeLayout(false);
            this.groupBoxAbortActions.PerformLayout();
            this.groupBoxItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		/// <summary>
		/// Designer에서 파싱 불가능한 코드 (P/Invoke, Lambda)
		/// </summary>
		private void PostInitializeComponent()
		{
			// Placeholder text (P/Invoke)
			SetPlaceholder(this.textBoxEntryExpression, "$AI_0000 > 10 && $DI_0001 == 1");
			SetPlaceholder(this.textBoxExitActions, "TAG=val;TAG=val  e.g. $AO_VALVE=0;$DO_PUMP=0");
			SetPlaceholder(this.textBoxAbortActions, "TAG=val;TAG=val  e.g. $AO_VALVE=0;$DO_PUMP=0");

			// Lambda (ComboBox 즉시 커밋)
			this.dataGridTransitions.CurrentCellDirtyStateChanged += (s, ev) =>
			{
				if (dataGridTransitions.IsCurrentCellDirty)
					dataGridTransitions.CommitEdit(DataGridViewDataErrorContexts.Commit);
			};
		}


		private void DataGrid_DataError(object sender, System.Windows.Forms.DataGridViewDataErrorEventArgs e)
		{
			e.ThrowException = false;
		}

		#endregion

		void dataGridTransitions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			if (e.ColumnIndex != dataGridTransitions.Columns["TransType"].Index) return;

			var row = dataGridTransitions.Rows[e.RowIndex];
			string typeStr = row.Cells["TransType"].Value?.ToString() ?? "";
			ApplyTransitionTypeUI(row, typeStr);
		}

		/// <summary>
		/// Non-Loop 전이 타입이면 TargetStep, MaxLoop 비활성화 (회색 배경)
		/// </summary>
		void ApplyTransitionTypeUI(DataGridViewRow row, string typeStr)
		{
			bool isLoop = typeStr.Equals("Loop", StringComparison.OrdinalIgnoreCase);

			row.Cells["TargetStep"].ReadOnly = !isLoop;
			row.Cells["MaxLoop"].ReadOnly = !isLoop;

			var disabledColor = System.Drawing.Color.FromArgb(240, 240, 240);
			var enabledColor = System.Drawing.Color.White;

			row.Cells["TargetStep"].Style.BackColor = isLoop ? enabledColor : disabledColor;
			row.Cells["MaxLoop"].Style.BackColor = isLoop ? enabledColor : disabledColor;
		}

		void LoadStepData()
		{
			if (StepData == null) return;

			textBoxStepName.Text = StepData.step_name;
			numericWaitTime.Value = Math.Min(StepData.wait_time_ms, (int)numericWaitTime.Maximum);
			numericTimeout.Value = Math.Min(StepData.timeout_ms > 0 ? StepData.timeout_ms : 30000, (int)numericTimeout.Maximum);

			// Entry Expression
			textBoxEntryExpression.Text = StepData.entry_expression ?? "";

			// 하위호환: 기존 entry tag/compare/value → 표현식 자동 변환
			if (string.IsNullOrEmpty(textBoxEntryExpression.Text))
			{
				string auto = BuildExprFromLegacy(StepData.entry_condition_tag, StepData.entry_condition_type, StepData.entry_condition_value);
				if (!string.IsNullOrEmpty(auto))
					textBoxEntryExpression.Text = auto;
			}

			// Actions (JSON → 간략 표기)
			textBoxExitActions.Text = ActionsJsonToSimple(StepData.exit_actions_json);
			textBoxAbortActions.Text = ActionsJsonToSimple(StepData.abort_actions_json);

			// Transitions 로드
			dataGridTransitions.Rows.Clear();
			if (!string.IsNullOrEmpty(StepData.transitions_json))
			{
				try
				{
					var transitions = JsonConvert.DeserializeObject<List<StepTransition>>(StepData.transitions_json);
					if (transitions != null)
					{
						for (int i = 0; i < transitions.Count; i++)
						{
							var tr = transitions[i];
							int rowIdx = dataGridTransitions.Rows.Add();
							var row = dataGridTransitions.Rows[rowIdx];
							row.Cells["Priority"].Value = tr.priority;
							row.Cells["Expression"].Value = tr.expression;
							row.Cells["TransType"].Value = tr.type.ToString();
							row.Cells["TargetStep"].Value = tr.target_step_order;
							row.Cells["MaxLoop"].Value = tr.max_loop_count;
							row.Cells["TimeoutMs"].Value = tr.timeout_ms;
							row.Cells["Description"].Value = tr.description;

							// Non-Loop 전이: target/maxloop 비활성화
							ApplyTransitionTypeUI(row, tr.type.ToString());
						}
					}
				}
				catch { }
			}
			else
			{
				// 하위호환: legacy 필드에서 자동 행 생성
				if (!string.IsNullOrEmpty(StepData.running_expression))
				{
					int rowIdx = dataGridTransitions.Rows.Add();
					var row = dataGridTransitions.Rows[rowIdx];
					row.Cells["Priority"].Value = 0;
					row.Cells["Expression"].Value = "!(" + StepData.running_expression + ")";
					row.Cells["TransType"].Value = "Exception";
					row.Cells["TargetStep"].Value = -1;
					row.Cells["MaxLoop"].Value = 10;
					row.Cells["Description"].Value = "Auto: Running";
				}
				if (!string.IsNullOrEmpty(StepData.exit_expression))
				{
					int rowIdx = dataGridTransitions.Rows.Add();
					var row = dataGridTransitions.Rows[rowIdx];
					row.Cells["Priority"].Value = 10;
					row.Cells["Expression"].Value = StepData.exit_expression;
					row.Cells["TransType"].Value = "Complete";
					row.Cells["TargetStep"].Value = -1;
					row.Cells["MaxLoop"].Value = 10;
					row.Cells["Description"].Value = "Auto: Exit";
				}
				else if (!string.IsNullOrEmpty(StepData.condition_tag) && StepData.condition_type != "none")
				{
					string auto = BuildExprFromLegacy(StepData.condition_tag, StepData.condition_type, StepData.condition_value);
					if (!string.IsNullOrEmpty(auto))
					{
						int rowIdx = dataGridTransitions.Rows.Add();
						var row = dataGridTransitions.Rows[rowIdx];
						row.Cells["Priority"].Value = 10;
						row.Cells["Expression"].Value = auto;
						row.Cells["TransType"].Value = "Complete";
						row.Cells["TargetStep"].Value = -1;
						row.Cells["MaxLoop"].Value = 10;
						row.Cells["Description"].Value = "Auto: Legacy";
					}
				}
			}

			// Items 로드
			dataGridItems.Rows.Clear();
			for (int i = 0; i < StepData.items.Count; i++)
			{
				var item = (RecipeItemData)StepData.items[i];
				int rowIdx = dataGridItems.Rows.Add();
				dataGridItems.Rows[rowIdx].Cells["TagName"].Value = item.tag_name;
				dataGridItems.Rows[rowIdx].Cells["SetValue"].Value = item.set_value;
				dataGridItems.Rows[rowIdx].Cells["ValueType"].Value = item.value_type;
				dataGridItems.Rows[rowIdx].Cells["ItemOrder"].Value = item.item_order;
			}
		}

		/// <summary>
		/// 기존 tag/compare/value → 표현식 자동 변환 (하위호환)
		/// </summary>
		static string BuildExprFromLegacy(string tag, string type, string value)
		{
			if (string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(type) || type == "none")
				return null;

			string op;
			switch (type.ToLower())
			{
				case "equal": op = "=="; break;
				case "greater": op = ">"; break;
				case "less": op = "<"; break;
				default: return null;
			}

			return string.Format("${0} {1} {2}", tag, op, value ?? "0");
		}

		/// <summary>
		/// Actions JSON → 간략 문자열: $TAG=val;$TAG=val
		/// </summary>
		static string ActionsJsonToSimple(string json)
		{
			if (string.IsNullOrEmpty(json)) return "";
			try
			{
				var arr = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
				if (arr == null || arr.Count == 0) return "";
				var parts = new List<string>();
				foreach (var item in arr)
				{
					string tag = "", val = "";
					if (item.ContainsKey("tag_name")) tag = item["tag_name"];
					if (item.ContainsKey("set_value")) val = item["set_value"];
					if (!string.IsNullOrEmpty(tag))
						parts.Add("$" + tag + "=" + val);
				}
				return string.Join(";", parts);
			}
			catch { return json; }
		}

		/// <summary>
		/// 간략 문자열 → Actions JSON: [{"tag_name":"..","set_value":"..","value_type":"double"}]
		/// </summary>
		static string SimpleToActionsJson(string simple)
		{
			if (string.IsNullOrEmpty(simple)) return "";
			var arr = new List<Dictionary<string, string>>();
			foreach (string part in simple.Split(';'))
			{
				string p = part.Trim();
				if (string.IsNullOrEmpty(p)) continue;

				// $TAG=value  또는 TAG=value
				if (p.StartsWith("$")) p = p.Substring(1);
				int eq = p.IndexOf('=');
				if (eq <= 0) continue;

				string tag = p.Substring(0, eq).Trim();
				string val = p.Substring(eq + 1).Trim();

				var d = new Dictionary<string, string>();
				d["tag_name"] = tag;
				d["set_value"] = val;
				d["value_type"] = "double";
				arr.Add(d);
			}
			if (arr.Count == 0) return "";
			return JsonConvert.SerializeObject(arr);
		}

		void buttonOK_Click(object sender, EventArgs e)
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			string stepName = textBoxStepName.Text.Trim();
			if (string.IsNullOrEmpty(stepName))
			{
				MessageBox.Show(
					isKor ? "Step 이름을 입력하세요." : "Please enter the step name.",
					isKor ? "입력 오류" : "Input Error",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				textBoxStepName.Focus();
				return;
			}

			StepData.step_name = stepName;
			StepData.wait_time_ms = (int)numericWaitTime.Value;
			StepData.timeout_ms = (int)numericTimeout.Value;

			// Entry Expression
			StepData.entry_expression = textBoxEntryExpression.Text.Trim();

			// Entry expression 검증
			if (!string.IsNullOrEmpty(StepData.entry_expression))
			{
				string err = RecipeExpressionEvaluator.Validate(StepData.entry_expression);
				if (err != null)
				{
					MessageBox.Show(
						(isKor ? "시작조건 표현식 오류: " : "Entry expression error: ") + err,
						isKor ? "검증 오류" : "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textBoxEntryExpression.Focus();
					return;
				}
			}

			// Actions
			StepData.exit_actions_json = SimpleToActionsJson(textBoxExitActions.Text.Trim());
			StepData.abort_actions_json = SimpleToActionsJson(textBoxAbortActions.Text.Trim());

			// Transitions 수집
			var transitionList = new List<StepTransition>();
			for (int i = 0; i < dataGridTransitions.Rows.Count; i++)
			{
				var row = dataGridTransitions.Rows[i];
				if (row.IsNewRow) continue;

				string expr = row.Cells["Expression"].Value != null ? row.Cells["Expression"].Value.ToString().Trim() : "";
				string typeStr = row.Cells["TransType"].Value != null ? row.Cells["TransType"].Value.ToString() : "Complete";

				// 빈 행 건너뛰기 (타입도 없고 표현식도 없으면)
				if (string.IsNullOrEmpty(expr) && string.IsNullOrEmpty(typeStr)) continue;

				// 표현식 검증
				if (!string.IsNullOrEmpty(expr))
				{
					string err = RecipeExpressionEvaluator.Validate(expr);
					if (err != null)
					{
						MessageBox.Show(
							(isKor ? "전이 조건식 오류 (행 " : "Transition expression error (row ") + (i + 1) + "): " + err,
							isKor ? "검증 오류" : "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						dataGridTransitions.CurrentCell = row.Cells["Expression"];
						return;
					}
				}

				TransitionType transType;
				if (!Enum.TryParse(typeStr, out transType))
					transType = TransitionType.Complete;

				int priority = 0;
				if (row.Cells["Priority"].Value != null)
					int.TryParse(row.Cells["Priority"].Value.ToString(), out priority);

				int target = -1;
				if (row.Cells["TargetStep"].Value != null)
					int.TryParse(row.Cells["TargetStep"].Value.ToString(), out target);

				int maxLoop = 10;
				if (row.Cells["MaxLoop"].Value != null)
					int.TryParse(row.Cells["MaxLoop"].Value.ToString(), out maxLoop);
				if (maxLoop <= 0) maxLoop = 10;

				int timeoutMs = 0;
				if (row.Cells["TimeoutMs"].Value != null)
					int.TryParse(row.Cells["TimeoutMs"].Value.ToString(), out timeoutMs);

				string desc = row.Cells["Description"].Value != null ? row.Cells["Description"].Value.ToString() : "";

				transitionList.Add(new StepTransition
				{
					priority = priority,
					expression = expr,
					type = transType,
					target_step_order = target,
					max_loop_count = maxLoop,
					timeout_ms = timeoutMs,
					description = desc
				});
			}

			// JSON 직렬화
			if (transitionList.Count > 0)
			{
				transitionList.Sort((a, b) => a.priority.CompareTo(b.priority));
				StepData.transitions_json = JsonConvert.SerializeObject(transitionList);
			}
			else
			{
				StepData.transitions_json = "";
			}

			// Legacy 필드 비우기 (전이 모델 사용)
			StepData.exit_expression = "";
			StepData.running_expression = "";
			StepData.condition_tag = "";
			StepData.condition_value = "";
			StepData.condition_type = "none";
			StepData.entry_condition_tag = "";
			StepData.entry_condition_value = "";
			StepData.entry_condition_type = "none";

			// Items 수집 + 중복 검증
			ArrayList tempItems = new ArrayList();
			HashSet<string> tagSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			int autoOrder = 0;

			for (int i = 0; i < dataGridItems.Rows.Count; i++)
			{
				var row = dataGridItems.Rows[i];
				if (row.IsNewRow) continue;

				string tagName = row.Cells["TagName"].Value != null ? row.Cells["TagName"].Value.ToString().Trim() : "";
				if (string.IsNullOrEmpty(tagName)) continue;

				if (!tagSet.Add(tagName))
				{
					MessageBox.Show(
						isKor ? string.Format("중복된 태그가 있습니다: {0}", tagName)
						      : string.Format("Duplicate tag found: {0}", tagName),
						isKor ? "중복 오류" : "Duplicate Error",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					dataGridItems.CurrentCell = row.Cells["TagName"];
					return;
				}

				var item = new RecipeItemData();
				item.tag_name = tagName;
				item.set_value = row.Cells["SetValue"].Value != null ? row.Cells["SetValue"].Value.ToString() : "";
				item.value_type = row.Cells["ValueType"].Value != null ? row.Cells["ValueType"].Value.ToString() : "double";

				int itemOrder;
				if (row.Cells["ItemOrder"].Value != null && int.TryParse(row.Cells["ItemOrder"].Value.ToString(), out itemOrder))
					item.item_order = itemOrder;
				else
					item.item_order = autoOrder;

				tempItems.Add(item);
				autoOrder++;
			}

			StepData.items = tempItems;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		#region Tag Select & Validate Handlers

		/// <summary>
		/// 조건식 텍스트박스에 $TAG_NAME 을 삽입하는 공통 태그 선택 핸들러
		/// </summary>
		void InsertTagIntoExpression(TextBox target)
		{
			FormSelectTag dialog = new FormSelectTag();
			dialog.bUseTagAI = true;
			dialog.bUseTagAO = true;
			dialog.bUseTagDI = true;
			dialog.bUseTagDO = true;
			dialog.bUseTagST = true;
			dialog.bUseTagGDO = true;

			if (dialog.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(dialog.sTag))
			{
				string insert = "$" + dialog.sTag;
				int pos = target.SelectionStart;
				target.Text = target.Text.Insert(pos, insert);
				target.SelectionStart = pos + insert.Length;
				target.Focus();
			}
		}

		void buttonEntryTagSelect_Click(object sender, EventArgs e)
		{
			InsertTagIntoExpression(textBoxEntryExpression);
		}

		void buttonEntryExprValidate_Click(object sender, EventArgs e)
		{
			ValidateExpression(textBoxEntryExpression, labelEntryExprResult);
		}

		void ValidateExpression(TextBox source, Label result)
		{
			string expr = source.Text.Trim();
			if (string.IsNullOrEmpty(expr))
			{
				result.Text = "";
				return;
			}
			string err = RecipeExpressionEvaluator.Validate(expr);
			if (err == null)
			{
				result.ForeColor = Color.Green;
				result.Text = "OK";
			}
			else
			{
				result.ForeColor = Color.Red;
				result.Text = err;
			}
		}

		/// <summary>
		/// Transitions DataGrid "..." 버튼 클릭 → FormSelectTag → expression에 $TAG 추가
		/// </summary>
		void dataGridTransitions_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			if (e.ColumnIndex != dataGridTransitions.Columns["SelectTag"].Index) return;

			var row = dataGridTransitions.Rows[e.RowIndex];
			string currentExpr = row.Cells["Expression"].Value != null ? row.Cells["Expression"].Value.ToString() : "";

			FormSelectTag dialog = new FormSelectTag();
			dialog.bUseTagAI = true;
			dialog.bUseTagAO = true;
			dialog.bUseTagDI = true;
			dialog.bUseTagDO = true;
			dialog.bUseTagST = true;
			dialog.bUseTagGDO = true;

			if (dialog.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(dialog.sTag))
			{
				row.Cells["Expression"].Value = currentExpr + "$" + dialog.sTag;
			}
		}

		/// <summary>
		/// Items DataGrid "..." 버튼 클릭 → FormSelectTag 열기
		/// </summary>
		void dataGridItems_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			if (e.ColumnIndex != dataGridItems.Columns["SelectTag"].Index) return;

			FormSelectTag dialog = new FormSelectTag();
			dialog.bUseTagAI = true;
			dialog.bUseTagAO = true;
			dialog.bUseTagDI = true;
			dialog.bUseTagDO = true;
			dialog.bUseTagST = true;
			dialog.bUseTagGDO = true;

			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				var row = dataGridItems.Rows[e.RowIndex];
				row.Cells["TagName"].Value = dialog.sTag;

				if (!string.IsNullOrEmpty(dialog.sTag) && TagLib.IsTagExist(dialog.sTag))
				{
					int[] tag_pos = new int[1];
					var tp = TagLib.GetStructPublic(dialog.sTag, ref tag_pos);
					if (tp.enumTagType == EnumTagType.DI || tp.enumTagType == EnumTagType.DO)
						row.Cells["ValueType"].Value = "bool";
					else if (tp.enumTagType == EnumTagType.ST)
						row.Cells["ValueType"].Value = "string";
					else
						row.Cells["ValueType"].Value = "double";
				}
			}
		}

		#endregion
	}
}
