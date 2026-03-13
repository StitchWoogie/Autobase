using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace LocalMain.Barcode
{
	/// <summary>
	/// RS232/USB 시리얼 포트 바코드 스캐너 입력 처리기.
	///
	/// COM 포트를 통해 바코드 스캐너로부터 데이터를 수신한다.
	/// 종료 문자(Terminator)를 기준으로 하나의 스캔을 완료 판정한다.
	/// </summary>
	public class SerialPortScanner : IDisposable
	{
		private readonly ScannerConfig _config;
		private SerialPort _port;
		private readonly StringBuilder _buffer = new StringBuilder();
		private readonly object _lock = new object();
		private System.Threading.Timer _timeoutTimer;
		private bool _disposed;

		/// <summary>바코드 스캔 완료 이벤트</summary>
		public event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;

		/// <summary>스캐너 이름</summary>
		public string Name => _config.Name;

		/// <summary>활성화 여부</summary>
		public bool Enabled { get; set; }

		/// <summary>포트 열림 상태</summary>
		public bool IsOpen => _port != null && _port.IsOpen;

		public SerialPortScanner(ScannerConfig config)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
			Enabled = config.Enabled;
		}

		/// <summary>
		/// 시리얼 포트를 열고 수신을 시작한다.
		/// </summary>
		public bool Open()
		{
			try
			{
				if (_port != null && _port.IsOpen)
					return true;

				_port = new SerialPort
				{
					PortName = _config.PortName,
					BaudRate = _config.BaudRate,
					DataBits = _config.DataBits,
					Parity = ParseParity(_config.Parity),
					StopBits = ParseStopBits(_config.StopBits),
					ReadTimeout = _config.SerialTimeoutMs > 0 ? _config.SerialTimeoutMs : 500,
					Encoding = Encoding.UTF8,
					ReceivedBytesThreshold = 1,
				};

				_port.DataReceived += OnDataReceived;
				_port.ErrorReceived += OnErrorReceived;
				_port.Open();

				Debug.WriteLine($"[SerialPortScanner] '{_config.Name}' opened on {_config.PortName}");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[SerialPortScanner] Open error '{_config.Name}': {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// 시리얼 포트를 닫는다.
		/// </summary>
		public void Close()
		{
			try
			{
				_timeoutTimer?.Dispose();
				_timeoutTimer = null;

				if (_port != null && _port.IsOpen)
				{
					_port.DataReceived -= OnDataReceived;
					_port.ErrorReceived -= OnErrorReceived;
					_port.Close();
				}

				_port?.Dispose();
				_port = null;

				Debug.WriteLine($"[SerialPortScanner] '{_config.Name}' closed");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[SerialPortScanner] Close error: {ex.Message}");
			}
		}

		/// <summary>
		/// 시리얼 데이터 수신 이벤트 핸들러
		/// </summary>
		private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			if (!Enabled) return;

			try
			{
				string data = _port.ReadExisting();
				if (string.IsNullOrEmpty(data))
					return;

				lock (_lock)
				{
					_buffer.Append(data);

					// 종료 문자 확인
					string terminator = _config.SerialTerminator;
					if (string.IsNullOrEmpty(terminator))
						terminator = "\r\n";

					string bufStr = _buffer.ToString();
					int termIdx = bufStr.IndexOf(terminator, StringComparison.Ordinal);

					if (termIdx >= 0)
					{
						// 종료 문자 발견 → 스캔 완료
						string rawCode = bufStr.Substring(0, termIdx);
						_buffer.Clear();

						// 종료 문자 이후의 데이터가 있으면 버퍼에 유지
						int remain = termIdx + terminator.Length;
						if (remain < bufStr.Length)
							_buffer.Append(bufStr.Substring(remain));

						CancelTimeout();
						ProcessCompleteScan(rawCode);
					}
					else
					{
						// 종료 문자 미발견 → 타임아웃 대기
						ResetTimeout();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[SerialPortScanner] DataReceived error: {ex.Message}");
			}
		}

		/// <summary>
		/// 타임아웃: 종료 문자 없이 데이터 수신이 중단됨
		/// </summary>
		private void OnTimeout(object state)
		{
			lock (_lock)
			{
				if (_buffer.Length >= _config.MinLength)
				{
					string rawCode = _buffer.ToString();
					_buffer.Clear();
					ProcessCompleteScan(rawCode);
				}
				else
				{
					_buffer.Clear();
				}
			}
		}

		private void ProcessCompleteScan(string rawCode)
		{
			if (string.IsNullOrEmpty(rawCode))
				return;

			string cleanCode = BarcodeValidator.CleanCode(rawCode, _config.Prefix, _config.Suffix);

			if (cleanCode.Length < _config.MinLength)
				return;

			var args = new BarcodeScannedEventArgs
			{
				RawCode = rawCode,
				CleanCode = cleanCode,
				Timestamp = DateTime.Now,
				SourceType = ScannerInputType.SerialPort,
				ScannerId = _config.Name,
				ValidationResult = BarcodeValidator.Validate(cleanCode, _config.Validation),
			};

			try
			{
				BarcodeScanned?.Invoke(this, args);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[SerialPortScanner] Event handler error: {ex.Message}");
			}
		}

		private void ResetTimeout()
		{
			int timeoutMs = _config.SerialTimeoutMs > 0 ? _config.SerialTimeoutMs : 200;

			if (_timeoutTimer == null)
				_timeoutTimer = new System.Threading.Timer(OnTimeout, null, timeoutMs, Timeout.Infinite);
			else
				_timeoutTimer.Change(timeoutMs, Timeout.Infinite);
		}

		private void CancelTimeout()
		{
			_timeoutTimer?.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			Debug.WriteLine($"[SerialPortScanner] Port error on '{_config.Name}': {e.EventType}");
		}

		/// <summary>
		/// 시스템에서 사용 가능한 COM 포트 목록을 반환한다.
		/// </summary>
		public static string[] GetAvailablePorts()
		{
			try
			{
				return SerialPort.GetPortNames();
			}
			catch
			{
				return new string[0];
			}
		}

		private static Parity ParseParity(string value)
		{
			if (string.IsNullOrEmpty(value)) return Parity.None;
			switch (value.ToUpperInvariant())
			{
				case "ODD": return Parity.Odd;
				case "EVEN": return Parity.Even;
				case "MARK": return Parity.Mark;
				case "SPACE": return Parity.Space;
				default: return Parity.None;
			}
		}

		private static StopBits ParseStopBits(float value)
		{
			if (value <= 1.0f) return StopBits.One;
			if (value <= 1.5f) return StopBits.OnePointFive;
			return StopBits.Two;
		}

		public void Dispose()
		{
			if (_disposed) return;
			_disposed = true;
			Close();
		}
	}
}
