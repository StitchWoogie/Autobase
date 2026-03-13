using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace LocalMain.Barcode
{
	/// <summary>
	/// 스캐너 중앙 관리자.
	///
	/// 모든 스캐너(키보드 웨지, 시리얼)의 입력을 통합 관리하여
	/// 각 화면에서 개별적으로 스캐너를 처리하지 않도록 한다.
	///
	/// 구조:
	/// ScannerManager
	///  ├─ KeyboardWedgeScanner[] (키보드 웨지)
	///  └─ SerialPortScanner[]    (RS232/USB 시리얼)
	///
	/// 스캔 결과는 중앙 이벤트(OnBarcodeScanned)로 디스패치되며,
	/// BarcodeManager가 태그 업데이트 및 스크립트 실행을 처리한다.
	/// </summary>
	public class ScannerManager : IDisposable
	{
		private readonly List<KeyboardWedgeScanner> _keyboardScanners = new List<KeyboardWedgeScanner>();
		private readonly List<SerialPortScanner> _serialScanners = new List<SerialPortScanner>();
		private readonly object _lock = new object();
		private bool _disposed;

		/// <summary>바코드 스캔 완료 통합 이벤트</summary>
		public event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;

		/// <summary>초기화 여부</summary>
		public bool IsInitialized { get; private set; }

		/// <summary>
		/// 설정을 기반으로 스캐너를 초기화한다.
		/// </summary>
		public void Initialize(List<ScannerConfig> configs)
		{
			lock (_lock)
			{
				Cleanup();

				if (configs == null || configs.Count == 0)
				{
					Debug.WriteLine("[ScannerManager] No scanners configured");
					IsInitialized = true;
					return;
				}

				foreach (var config in configs)
				{
					if (!config.Enabled) continue;

					try
					{
						switch (config.InputType)
						{
							case ScannerInputType.KeyboardWedge:
								var kbScanner = new KeyboardWedgeScanner(config);
								kbScanner.BarcodeScanned += OnChildScannerScanned;
								_keyboardScanners.Add(kbScanner);
								Debug.WriteLine($"[ScannerManager] Keyboard wedge scanner added: {config.Name}");
								break;

							case ScannerInputType.SerialPort:
								var serialScanner = new SerialPortScanner(config);
								serialScanner.BarcodeScanned += OnChildScannerScanned;
								if (serialScanner.Open())
								{
									_serialScanners.Add(serialScanner);
									Debug.WriteLine($"[ScannerManager] Serial scanner opened: {config.Name} on {config.PortName}");
								}
								else
								{
									Debug.WriteLine($"[ScannerManager] Failed to open serial scanner: {config.Name} on {config.PortName}");
									serialScanner.Dispose();
								}
								break;
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"[ScannerManager] Scanner init error '{config.Name}': {ex.Message}");
					}
				}

				IsInitialized = true;
				Debug.WriteLine($"[ScannerManager] Initialized: {_keyboardScanners.Count} keyboard, {_serialScanners.Count} serial");
			}
		}

		/// <summary>
		/// 키보드 KeyPress 이벤트를 모든 키보드 웨지 스캐너에 전달한다.
		/// FormLocalMain.KeyPreview = true 상태에서 호출한다.
		/// </summary>
		/// <returns>바코드 입력으로 소비된 경우 true</returns>
		public bool ProcessKeyPress(KeyPressEventArgs e)
		{
			lock (_lock)
			{
				foreach (var scanner in _keyboardScanners)
				{
					if (scanner.ProcessKeyPress(e))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 키보드 KeyDown 이벤트를 모든 키보드 웨지 스캐너에 전달한다.
		/// </summary>
		public bool ProcessKeyDown(KeyEventArgs e)
		{
			lock (_lock)
			{
				foreach (var scanner in _keyboardScanners)
				{
					if (scanner.ProcessKeyDown(e))
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 특정 스캐너의 활성화 상태를 변경한다.
		/// </summary>
		public void SetScannerEnabled(string scannerName, bool enabled)
		{
			lock (_lock)
			{
				foreach (var s in _keyboardScanners)
				{
					if (s.Name == scannerName) s.Enabled = enabled;
				}
				foreach (var s in _serialScanners)
				{
					if (s.Name == scannerName) s.Enabled = enabled;
				}
			}
		}

		/// <summary>
		/// 모든 스캐너의 활성화 상태를 변경한다.
		/// </summary>
		public void SetAllEnabled(bool enabled)
		{
			lock (_lock)
			{
				foreach (var s in _keyboardScanners) s.Enabled = enabled;
				foreach (var s in _serialScanners) s.Enabled = enabled;
			}
		}

		/// <summary>
		/// 등록된 스캐너 이름 목록을 반환한다.
		/// </summary>
		public List<string> GetScannerNames()
		{
			var names = new List<string>();
			lock (_lock)
			{
				foreach (var s in _keyboardScanners) names.Add(s.Name);
				foreach (var s in _serialScanners) names.Add(s.Name);
			}
			return names;
		}

		/// <summary>
		/// 자식 스캐너의 이벤트를 통합 이벤트로 전달
		/// </summary>
		private void OnChildScannerScanned(object sender, BarcodeScannedEventArgs e)
		{
			try
			{
				BarcodeScanned?.Invoke(this, e);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[ScannerManager] Event dispatch error: {ex.Message}");
			}
		}

		private void Cleanup()
		{
			foreach (var s in _keyboardScanners)
			{
				try { s.BarcodeScanned -= OnChildScannerScanned; s.Dispose(); } catch { }
			}
			_keyboardScanners.Clear();

			foreach (var s in _serialScanners)
			{
				try { s.BarcodeScanned -= OnChildScannerScanned; s.Dispose(); } catch { }
			}
			_serialScanners.Clear();
		}

		public void Dispose()
		{
			if (_disposed) return;
			_disposed = true;
			lock (_lock)
			{
				Cleanup();
			}
		}
	}
}
