using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace LocalMain.Barcode
{
	/// <summary>
	/// USB 키보드 웨지 스캐너 입력 처리기.
	///
	/// 키보드 웨지 스캐너는 바코드를 키보드 입력으로 전송한다.
	/// 일반 키보드 입력과 구분하기 위해 다음 조건을 사용한다:
	/// 1. 입력 속도가 매우 빠름 (KeyTimeoutMs 이내)
	/// 2. Prefix/Suffix 문자 감지
	/// 3. 최소 길이 이상의 연속 입력
	///
	/// 이 클래스는 FormLocalMain에 키보드 후크를 설치하여 동작한다.
	/// </summary>
	public class KeyboardWedgeScanner : IDisposable
	{
		private readonly ScannerConfig _config;
		private readonly StringBuilder _buffer = new StringBuilder();
		private readonly Timer _timer;
		private readonly object _lock = new object();
		private bool _disposed;
		private bool _prefixDetected;
		private DateTime _lastKeyTime;

		/// <summary>바코드 스캔 완료 이벤트</summary>
		public event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;

		/// <summary>스캐너 이름</summary>
		public string Name => _config.Name;

		/// <summary>활성화 여부</summary>
		public bool Enabled { get; set; }

		public KeyboardWedgeScanner(ScannerConfig config)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
			Enabled = config.Enabled;

			_timer = new Timer();
			_timer.Interval = Math.Max(config.KeyTimeoutMs, 20);
			_timer.Tick += OnTimerTick;
		}

		/// <summary>
		/// 키 입력을 처리한다. FormLocalMain의 KeyPreview 이벤트에서 호출한다.
		/// </summary>
		/// <param name="e">키 이벤트</param>
		/// <returns>바코드 입력으로 소비된 경우 true</returns>
		public bool ProcessKeyPress(KeyPressEventArgs e)
		{
			if (!Enabled) return false;

			char c = e.KeyChar;

			lock (_lock)
			{
				DateTime now = DateTime.Now;

				// Prefix 감지
				if (!string.IsNullOrEmpty(_config.Prefix) && !_prefixDetected)
				{
					if (_buffer.Length == 0 && _config.Prefix.Length == 1 && c == _config.Prefix[0])
					{
						_prefixDetected = true;
						_buffer.Clear();
						_lastKeyTime = now;
						_timer.Stop();
						_timer.Start();
						return true; // Prefix 문자를 소비
					}
				}

				// Suffix 감지 → 스캔 완료
				if (!string.IsNullOrEmpty(_config.Suffix) && _config.Suffix.Length == 1 && c == _config.Suffix[0])
				{
					if (_buffer.Length >= _config.MinLength)
					{
						CompleteScan();
						return true; // Suffix 문자를 소비
					}
					else
					{
						// 최소 길이 미달: 일반 키보드 입력으로 간주
						ResetBuffer();
						return false;
					}
				}

				// Enter 키를 기본 종료 문자로 처리 (Suffix가 \r인 경우)
				if (c == '\r' && _config.Suffix == "\r")
				{
					if (_buffer.Length >= _config.MinLength)
					{
						CompleteScan();
						return true;
					}
					else
					{
						ResetBuffer();
						return false;
					}
				}

				// 타임아웃 체크: 이전 키 입력으로부터 너무 오래 경과했으면 버퍼 초기화
				if (_buffer.Length > 0 && (now - _lastKeyTime).TotalMilliseconds > _config.KeyTimeoutMs * 3)
				{
					ResetBuffer();
				}

				// 제어 문자 무시 (탭, ESC 등)
				if (char.IsControl(c) && c != '\r' && c != '\n' && c != '\t')
					return false;

				// 버퍼에 추가
				_buffer.Append(c);
				_lastKeyTime = now;

				// 타이머 재시작
				_timer.Stop();
				_timer.Start();

				// Prefix가 설정되어 있고 감지된 상태면 키를 소비
				if (_prefixDetected)
					return true;
			}

			return false;
		}

		/// <summary>
		/// KeyDown 이벤트 처리 (Enter 키 감지용)
		/// </summary>
		public bool ProcessKeyDown(KeyEventArgs e)
		{
			if (!Enabled) return false;

			if (e.KeyCode == Keys.Enter)
			{
				lock (_lock)
				{
					if (_buffer.Length >= _config.MinLength)
					{
						CompleteScan();
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// 타임아웃 발생: Suffix 없이 입력이 중단됨
		/// </summary>
		private void OnTimerTick(object sender, EventArgs e)
		{
			_timer.Stop();

			lock (_lock)
			{
				if (_buffer.Length >= _config.MinLength)
				{
					// Suffix 없이도 최소 길이를 만족하면 스캔 완료로 처리
					CompleteScan();
				}
				else
				{
					ResetBuffer();
				}
			}
		}

		/// <summary>
		/// 스캔 완료 처리
		/// </summary>
		private void CompleteScan()
		{
			_timer.Stop();

			string rawCode = _buffer.ToString();
			ResetBuffer();

			if (string.IsNullOrEmpty(rawCode))
				return;

			string cleanCode = BarcodeValidator.CleanCode(rawCode, _config.Prefix, _config.Suffix);

			var args = new BarcodeScannedEventArgs
			{
				RawCode = rawCode,
				CleanCode = cleanCode,
				Timestamp = DateTime.Now,
				SourceType = ScannerInputType.KeyboardWedge,
				ScannerId = _config.Name,
				ValidationResult = BarcodeValidator.Validate(cleanCode, _config.Validation),
			};

			try
			{
				BarcodeScanned?.Invoke(this, args);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[KeyboardWedgeScanner] Event handler error: {ex.Message}");
			}
		}

		private void ResetBuffer()
		{
			_buffer.Clear();
			_prefixDetected = false;
			_timer.Stop();
		}

		public void Dispose()
		{
			if (_disposed) return;
			_disposed = true;
			_timer.Stop();
			_timer.Dispose();
		}
	}
}
