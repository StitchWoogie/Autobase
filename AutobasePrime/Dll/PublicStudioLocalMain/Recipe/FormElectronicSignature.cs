using System;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;

namespace PublicStudioLocalMain.Recipe
{
	/// <summary>
	/// 전자서명 다이얼로그 (ISA-88 Phase 6)
	/// 비밀번호 재확인 + 사유(Reason) 입력
	/// 반환값: "username|timestamp|action|reason"
	/// </summary>
	public partial class FormElectronicSignature : Form
	{
		#region Properties

		/// <summary>
		/// 서명 결과 문자열: "username|yyyy-MM-dd HH:mm:ss|action|reason"
		/// DialogResult.OK일 때만 유효
		/// </summary>
		public string SignatureResult { get; private set; } = "";

		/// <summary>
		/// 서명 사유 (reason 만 따로 가져올 때)
		/// </summary>
		public string Reason { get; private set; } = "";

		/// <summary>
		/// 서명 시각
		/// </summary>
		public DateTime SignedAt { get; private set; }

		#endregion

		private readonly string _actionName;
		private readonly string _username;
		private readonly string _objectType;
		private readonly string _objectName;
		private readonly int _objectVersion;
		private int _failCount = 0;
		private const int MaxFailCount = 5;
		private string _lastAuthError = "";

		/// <summary>
		/// 전자서명 다이얼로그 생성
		/// </summary>
		/// <param name="actionName">수행할 액션명 (예: "approve", "obsolete", "batch_start", "batch_abort")</param>
		/// <param name="username">현재 로그인 사용자명 (null이면 SharedData.userInfo.sUsername 사용)</param>
		/// <param name="objectType">대상 객체 유형 (예: "RECIPE", "BATCH")</param>
		/// <param name="objectName">대상 객체 이름</param>
		/// <param name="objectVersion">대상 객체 버전 (0이면 미표시)</param>
		public FormElectronicSignature(string actionName, string username = null,
			string objectType = "", string objectName = "", int objectVersion = 0)
		{
			_actionName = actionName ?? "";
			_username = username ?? SharedData.userInfo.sUsername;
			_objectType = objectType ?? "";
			_objectName = objectName ?? "";
			_objectVersion = objectVersion;

			InitializeComponent();
			SetObjectInfo();
			ApplyLocalization();

			txtUsername.Text = _username;
		}

		/// <summary>
		/// 대상 정보 문자열 구성 (동적 코드 — InitializeComponent 외부)
		/// </summary>
		private void SetObjectInfo()
		{
			if (!string.IsNullOrEmpty(_objectName))
			{
				string info = string.Format("[{0}] {1}", _actionName.ToUpper(), _objectName);
				if (_objectVersion > 0)
					info += string.Format(" v{0}", _objectVersion);
				if (!string.IsNullOrEmpty(_objectType))
					info = string.Format("{0}: {1}", _objectType, info);
				lblObjectInfo.Text = info;
			}
			else
			{
				lblObjectInfo.Text = _actionName.ToUpper();
			}
		}

		private void ApplyLocalization()
		{
			bool isKor = NetTools.Tools.IsLangKorean();

			if (isKor)
			{
				this.Text = "전자서명";
				lblTitle.Text = "전자서명";
				lblUsername.Text = "사용자:";
				lblPassword.Text = "비밀번호:";
				lblReason.Text = "사유:";
				btnOK.Text = "확인";
				btnCancel.Text = "취소";
			}
		}

		private async void BtnOK_Click(object sender, EventArgs e)
		{
			// 1. 비밀번호 필수 확인
			if (string.IsNullOrEmpty(txtPassword.Text))
			{
				bool isKor = NetTools.Tools.IsLangKorean();
				lblPasswordError.Text = isKor ? "비밀번호를 입력하세요." : "Please enter password.";
				txtPassword.Focus();
				return;
			}

			// 2. 사유 필수 확인
			if (string.IsNullOrWhiteSpace(txtReason.Text))
			{
				bool isKor = NetTools.Tools.IsLangKorean();
				lblPasswordError.Text = isKor ? "사유를 입력하세요." : "Please enter reason.";
				txtReason.Focus();
				return;
			}

			// 3. 비밀번호 검증 — FormLogin과 동일한 DataGate.CheckUserName 사용
			btnOK.Enabled = false;
			bool authSuccess = await VerifyPasswordAsync(_username, txtPassword.Text);
			btnOK.Enabled = true;

			if (!authSuccess)
			{
				bool isKor = NetTools.Tools.IsLangKorean();
				_failCount++;

				if (_failCount >= MaxFailCount)
				{
					string msg = isKor
						? string.Format("비밀번호 오류 {0}회 초과. 서명이 취소됩니다.", MaxFailCount)
						: string.Format("Password failed {0} times. Signature cancelled.", MaxFailCount);
					MessageBox.Show(msg, isKor ? "전자서명 오류" : "Signature Error",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					DialogResult = DialogResult.Cancel;
					Close();
					return;
				}

				// DataGate에서 반환된 오류 메시지 사용 (계정 잠금 등 포함)
				if (!string.IsNullOrEmpty(_lastAuthError))
					lblPasswordError.Text = _lastAuthError;
				else
					lblPasswordError.Text = isKor
						? string.Format("비밀번호가 일치하지 않습니다. ({0}/{1})", _failCount, MaxFailCount)
						: string.Format("Password mismatch. ({0}/{1})", _failCount, MaxFailCount);

				txtPassword.SelectAll();
				txtPassword.Focus();
				return;
			}

			// 4. 서명 생성
			SignedAt = DateTime.Now;
			Reason = txtReason.Text.Trim();

			// 서명 문자열: "username|timestamp|action|reason"
			// reason 내 파이프 문자 제거 (구분자 보호)
			string safeReason = Reason.Replace("|", " ").Replace("\r\n", " ").Replace("\n", " ");
			SignatureResult = string.Format("{0}|{1}|{2}|{3}",
				_username,
				SignedAt.ToString("yyyy-MM-dd HH:mm:ss"),
				_actionName,
				safeReason);

			DialogResult = DialogResult.OK;
			Close();
		}

		private void BtnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		/// <summary>
		/// 비밀번호 검증: FormLogin과 동일한 DataGate.CheckUserName 경로 사용
		/// - 파일에서 사용자 정보 재로딩
		/// - 계정 잠금(nPasswordMismatchedCount) 적용
		/// - 성공 시 실패 카운트 리셋
		/// </summary>
		private async System.Threading.Tasks.Task<bool> VerifyPasswordAsync(string username, string password)
		{
			_lastAuthError = "";
			try
			{
				// FormLogin과 동일: 해시 생성 → DataGate.CheckUserName
				string passCode = UserInfoStruct.ZipPassword(username, password);
				string passCode256 = UserInfoStruct.ZipPassword256(username, password);

				DataGate gate = new DataGate();
				var (success, errorMsg, trialMode) = await gate.CheckUserName(username, passCode, passCode256);

				if (!success)
					_lastAuthError = errorMsg ?? "";

				return success;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("VerifyPasswordAsync error: " + ex.Message);
				_lastAuthError = ex.Message;
				return false;
			}
		}

		/// <summary>
		/// 전자서명 팝업을 표시하고 결과를 반환하는 정적 헬퍼
		/// </summary>
		/// <param name="owner">부모 폼</param>
		/// <param name="actionName">액션명 (예: "approve", "obsolete", "batch_start", "batch_abort")</param>
		/// <param name="username">사용자명 (null이면 현재 로그인 사용자)</param>
		/// <param name="objectType">대상 객체 유형 (예: "RECIPE", "BATCH")</param>
		/// <param name="objectName">대상 객체 이름</param>
		/// <param name="objectVersion">대상 객체 버전 (0이면 미표시)</param>
		/// <returns>(success, signatureString, reason)</returns>
		public static (bool success, string signature, string reason) ShowSignature(
			IWin32Window owner, string actionName, string username = null,
			string objectType = "", string objectName = "", int objectVersion = 0)
		{
			using (var dlg = new FormElectronicSignature(actionName, username, objectType, objectName, objectVersion))
			{
				if (dlg.ShowDialog(owner) == DialogResult.OK)
				{
					return (true, dlg.SignatureResult, dlg.Reason);
				}
				return (false, "", "");
			}
		}
	}
}
