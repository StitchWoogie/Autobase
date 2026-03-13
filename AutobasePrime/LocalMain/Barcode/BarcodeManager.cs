using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using AutoLibLocal;
using GraphicModule;
using Newtonsoft.Json;

namespace LocalMain.Barcode
{
	/// <summary>
	/// 바코드/QR 서브시스템 중앙 관리자.
	///
	/// 아키텍처:
	/// BarcodeManager (이 클래스)
	///  ├─ ScannerManager   : 스캐너 입력 통합 관리
	///  ├─ ScanEventLogger  : 스캔 이벤트 로깅
	///  └─ BarcodeGenerator : 바코드/QR 이미지 생성 (복수)
	///
	/// 동작 흐름:
	/// 1. Scanner → ScannerManager → BarcodeScanned 이벤트
	/// 2. BarcodeManager → 유효성 검사 → 태그 업데이트 → 스크립트 실행
	/// 3. ScanEventLogger → CSV 로그 기록
	///
	/// 초기화: C_init.ViewProgrammStart()에서 BarcodeManager.Init() 호출
	/// </summary>
	public static class BarcodeManager
	{
		private static BarcodeConfig _config;
		private static ScannerManager _scannerManager;
		private static ScanEventLogger _scanEventLogger;
		private static readonly Dictionary<string, BarcodeGenerator> _generators
			= new Dictionary<string, BarcodeGenerator>();
		private static readonly object _lock = new object();
		private static bool _initialized;

		/// <summary>바코드 스캔 완료 이벤트 (태그 업데이트 후 발생)</summary>
		public static event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;

		/// <summary>유효성 검사 실패 이벤트</summary>
		public static event EventHandler<BarcodeScannedEventArgs> ValidationFailed;

		/// <summary>초기화 여부</summary>
		public static bool IsInitialized => _initialized;

		/// <summary>스캐너 매니저 접근</summary>
		public static ScannerManager Scanner => _scannerManager;

		#region 초기화 / 종료

		/// <summary>
		/// 바코드 서브시스템을 초기화한다.
		/// </summary>
		public static void Init()
		{
			if (_initialized) return;

			try
			{
				// 설정 로드
				_config = LoadConfig();

				if (!_config.Enabled)
				{
					Debug.WriteLine("[BarcodeManager] Barcode subsystem is disabled");
					_initialized = true;
					return;
				}

				// 스캐너 매니저 초기화
				_scannerManager = new ScannerManager();
				_scannerManager.BarcodeScanned += OnScannerBarcodeScanned;
				_scannerManager.Initialize(_config.Scanners);

				// 스캔 이벤트 로거 초기화
				if (_config.EnableScanLog)
				{
					string logDir = GetLogDirectory();
					_scanEventLogger = new ScanEventLogger(logDir, _config.ScanLogRetentionDays);
					_scanEventLogger.Start();
				}

				// 바코드 생성기 초기화
				foreach (var genConfig in _config.Generators)
				{
					if (!string.IsNullOrEmpty(genConfig.Name))
					{
						_generators[genConfig.Name] = new BarcodeGenerator(genConfig);
					}
				}

				_initialized = true;
				Debug.WriteLine($"[BarcodeManager] Initialized: {_config.Scanners.Count} scanners, {_config.Generators.Count} generators");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BarcodeManager] Init error: {ex.Message}");
				_initialized = true; // 에러가 발생해도 초기화 완료로 표시 (시스템 구동 차단 방지)
			}
		}

		/// <summary>
		/// 바코드 서브시스템을 종료한다.
		/// </summary>
		public static void Shutdown()
		{
			lock (_lock)
			{
				try
				{
					if (_scannerManager != null)
					{
						_scannerManager.BarcodeScanned -= OnScannerBarcodeScanned;
						_scannerManager.Dispose();
						_scannerManager = null;
					}

					if (_scanEventLogger != null)
					{
						_scanEventLogger.Dispose();
						_scanEventLogger = null;
					}

					foreach (var gen in _generators.Values)
					{
						try { gen.Dispose(); } catch { }
					}
					_generators.Clear();

					_initialized = false;
					Debug.WriteLine("[BarcodeManager] Shutdown complete");
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"[BarcodeManager] Shutdown error: {ex.Message}");
				}
			}
		}

		#endregion

		#region 스캔 이벤트 처리

		/// <summary>
		/// 스캐너에서 바코드가 스캔되었을 때의 처리 흐름
		/// </summary>
		private static void OnScannerBarcodeScanned(object sender, BarcodeScannedEventArgs e)
		{
			try
			{
				// 1. 유효성 검사 결과 확인
				if (!e.IsValid)
				{
					Debug.WriteLine($"[BarcodeManager] Validation failed: {e.CleanCode} → {e.ValidationResult}");

					// 유효성 검사 실패 이벤트
					try { ValidationFailed?.Invoke(null, e); } catch { }

					// HMI 스캐너 오브젝트에도 실패 결과 전달
					try
					{
						ObjectBarcodeScanner.NotifyAllScanners(
							e.ScannerId, e.CleanCode, false);
					}
					catch { }

					// 실패해도 로그는 기록
					LogScanEvent(e, null, null, null);
					return;
				}

				// 2. 태그 업데이트
				string resultTagName = null;
				string resultTagValue = null;
				UpdateResultTag(e, ref resultTagName, ref resultTagValue);

				// 3. 스캔 완료 이벤트 발생 (스크립트 실행용)
				try { BarcodeScanned?.Invoke(null, e); } catch { }

				// 3-1. HMI 스캐너 오브젝트 업데이트
				try
				{
					GraphicModule.ObjectBarcodeScanner.NotifyAllScanners(
						e.ScannerId, e.CleanCode, e.IsValid);
				}
				catch { }

				// 4. 스캔 로그 기록
				string scriptName = null; // 스크립트 실행은 이벤트 핸들러에서 수행
				LogScanEvent(e, resultTagName, resultTagValue, scriptName);

				Debug.WriteLine($"[BarcodeManager] Scan processed: {e.CleanCode} from {e.ScannerId}");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BarcodeManager] Scan processing error: {ex.Message}");
			}
		}

		/// <summary>
		/// 스캔 결과를 설정된 태그에 저장한다.
		/// </summary>
		private static void UpdateResultTag(BarcodeScannedEventArgs e,
			ref string resultTagName, ref string resultTagValue)
		{
			if (_config == null) return;

			// 스캐너에 연결된 결과 태그 찾기
			foreach (var scannerConfig in _config.Scanners)
			{
				if (scannerConfig.Name == e.ScannerId &&
					!string.IsNullOrEmpty(scannerConfig.ResultTagName))
				{
					resultTagName = scannerConfig.ResultTagName;
					resultTagValue = e.CleanCode;

					try
					{
						// 태그에 값 쓰기
						int[] pos = null;
						TagPublicClass tp = TagLib.GetStructPublic(resultTagName, ref pos);
						if (tp != null)
						{
							AutoLib.TagWrite.SetTagValue(resultTagName, resultTagValue, false);
							Debug.WriteLine($"[BarcodeManager] Tag updated: {resultTagName} = {resultTagValue}");
						}
						else
						{
							Debug.WriteLine($"[BarcodeManager] Tag not found: {resultTagName}");
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"[BarcodeManager] Tag update error: {ex.Message}");
					}
					break;
				}
			}
		}

		private static void LogScanEvent(BarcodeScannedEventArgs e,
			string resultTagName, string resultTagValue, string scriptName)
		{
			if (_scanEventLogger == null) return;

			string operatorName = "";
			try
			{
				operatorName = ConfigRunMain.sUserName ?? "";
			}
			catch { }

			_scanEventLogger.Log(e, operatorName, resultTagName, resultTagValue, scriptName);
		}

		#endregion

		#region 키보드 이벤트 전달

		/// <summary>
		/// FormLocalMain의 KeyPress 이벤트를 스캐너 매니저에 전달한다.
		/// </summary>
		public static bool ProcessKeyPress(KeyPressEventArgs e)
		{
			return _scannerManager?.ProcessKeyPress(e) ?? false;
		}

		/// <summary>
		/// FormLocalMain의 KeyDown 이벤트를 스캐너 매니저에 전달한다.
		/// </summary>
		public static bool ProcessKeyDown(KeyEventArgs e)
		{
			return _scannerManager?.ProcessKeyDown(e) ?? false;
		}

		#endregion

		#region 바코드 생성기 접근

		/// <summary>
		/// 이름으로 바코드 생성기를 가져온다.
		/// </summary>
		public static BarcodeGenerator GetGenerator(string name)
		{
			lock (_lock)
			{
				if (_generators.TryGetValue(name, out var gen))
					return gen;
				return null;
			}
		}

		/// <summary>
		/// 텍스트에서 바코드 이미지를 생성한다 (일회성).
		/// </summary>
		public static System.Drawing.Bitmap GenerateBarcode(string text,
			BarcodeFormat format = BarcodeFormat.QRCode,
			int width = 200, int height = 200,
			QRErrorCorrectionLevel ecLevel = QRErrorCorrectionLevel.Q,
			bool showHumanReadableText = true)
		{
			var config = new GeneratorConfig
			{
				Name = "_temp_",
				Format = format,
				TextSource = text,
				Width = width,
				Height = height,
				ErrorCorrectionLevel = ecLevel,
				ShowHumanReadableText = showHumanReadableText,
			};

			// using 사용 금지 - Dispose()가 반환된 비트맵(_cachedImage)을 파괴함
			var gen = new BarcodeGenerator(config);
			return gen.GenerateImage(text);
		}

		#endregion

		#region 설정 로드/저장

		/// <summary>설정 파일 경로</summary>
		private static string GetConfigFilePath()
		{
			string projectDir = Application.StartupPath;
			return Path.Combine(projectDir, "BarcodeConfig.json");
		}

		/// <summary>로그 디렉토리 경로</summary>
		private static string GetLogDirectory()
		{
			string projectDir = Application.StartupPath;
			return Path.Combine(projectDir, "BarcodeLog");
		}

		/// <summary>설정을 파일에서 로드한다.</summary>
		public static BarcodeConfig LoadConfig()
		{
			try
			{
				string path = GetConfigFilePath();
				if (File.Exists(path))
				{
					string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
					return JsonConvert.DeserializeObject<BarcodeConfig>(json) ?? new BarcodeConfig();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BarcodeManager] Config load error: {ex.Message}");
			}
			return new BarcodeConfig();
		}

		/// <summary>설정을 파일에 저장한다.</summary>
		public static bool SaveConfig(BarcodeConfig config)
		{
			try
			{
				string path = GetConfigFilePath();
				string json = JsonConvert.SerializeObject(config, Formatting.Indented);
				File.WriteAllText(path, json, System.Text.Encoding.UTF8);

				_config = config;
				Debug.WriteLine("[BarcodeManager] Config saved");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BarcodeManager] Config save error: {ex.Message}");
				return false;
			}
		}

		/// <summary>현재 설정을 반환한다.</summary>
		public static BarcodeConfig GetConfig()
		{
			return _config ?? new BarcodeConfig();
		}

		/// <summary>
		/// 설정을 변경하고 서브시스템을 재초기화한다.
		/// </summary>
		public static void ApplyConfig(BarcodeConfig config)
		{
			SaveConfig(config);
			Shutdown();
			_initialized = false;
			Init();
		}

		#endregion

		#region 스크립트 함수용 정적 메서드

		/// <summary>
		/// 마지막 스캔 결과를 반환한다 (스크립트에서 호출용).
		/// </summary>
		public static string GetLastScanValue(string scannerName)
		{
			if (_config == null) return "";

			foreach (var sc in _config.Scanners)
			{
				if (sc.Name == scannerName && !string.IsNullOrEmpty(sc.ResultTagName))
				{
					try
					{
						int[] pos = null;
						TagPublicClass tp = TagLib.GetStructPublic(sc.ResultTagName, ref pos);
						if (tp != null)
						{
							object val = tp.GetCurr();
							return val?.ToString() ?? "";
						}
					}
					catch { }
				}
			}
			return "";
		}

		/// <summary>
		/// 바코드 텍스트에서 특정 키의 값을 추출한다 (스크립트에서 호출용).
		/// </summary>
		public static string ExtractBarcodeValue(string barcodeText, string key)
		{
			return QRDataParser.ExtractValue(barcodeText, key);
		}

		/// <summary>
		/// 스캐너 활성화/비활성화 (스크립트에서 호출용).
		/// </summary>
		public static void SetScannerEnabled(string scannerName, bool enabled)
		{
			_scannerManager?.SetScannerEnabled(scannerName, enabled);
		}

		#endregion
	}
}
