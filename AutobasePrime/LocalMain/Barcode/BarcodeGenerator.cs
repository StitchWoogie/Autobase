using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AutoLibLocal;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZxBarcodeFormat = ZXing.BarcodeFormat;

namespace LocalMain.Barcode
{
	/// <summary>
	/// 바코드/QR 코드 생성기.
	/// 태그 바인딩 템플릿을 해석하여 동적 바코드 이미지를 생성한다.
	/// ZXing.Net 라이브러리(NuGet: ZXing.Net)를 사용한다.
	/// </summary>
	public class BarcodeGenerator : IDisposable
	{
		private GeneratorConfig _config;
		private Bitmap _cachedImage;
		private string _lastText = "";
		private readonly object _lock = new object();
		private bool _disposed;

		// Regex 컴파일 캐싱
		static readonly Regex RegexTagTemplate =
			new Regex(@"\{Tag\.([^}]+)\}", RegexOptions.Compiled);

		// BarcodeWriter 재사용
		private BarcodeWriter _writer;

		/// <summary>이미지가 갱신되었을 때 발생</summary>
		public event EventHandler ImageUpdated;

		public BarcodeGenerator(GeneratorConfig config)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
		}

		/// <summary>설정 변경</summary>
		public void UpdateConfig(GeneratorConfig config)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_lastText = "";  // 설정 변경 시 강제 재생성
			Regenerate();
		}

		/// <summary>
		/// 태그 바인딩 템플릿을 해석하여 텍스트를 생성한다.
		/// 예: "LOT={Tag.LOT};LINE={Tag.LINE}" → "LOT=230501;LINE=3"
		/// </summary>
		public static string ResolveTemplate(string template)
		{
			if (string.IsNullOrEmpty(template))
				return "";

			return RegexTagTemplate.Replace(template, match =>
			{
				string tagName = match.Groups[1].Value;
				try
				{
					int[] pos = null;
					TagPublicClass tp = TagLib.GetStructPublic(tagName, ref pos);
					if (tp != null)
					{
						object val = tp.GetCurr();
						return val?.ToString() ?? "";
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"[BarcodeGenerator] Tag resolve error '{tagName}': {ex.Message}");
				}
				return match.Value;
			});
		}

		/// <summary>
		/// 현재 설정에 따라 바코드 텍스트를 결정한다.
		/// TagBindingTemplate이 있으면 태그에서 해석, 없으면 TextSource 사용.
		/// </summary>
		public string GetCurrentText()
		{
			if (!string.IsNullOrEmpty(_config.TagBindingTemplate))
				return ResolveTemplate(_config.TagBindingTemplate);

			return _config.TextSource ?? "";
		}

		/// <summary>
		/// 현재 설정으로 바코드 이미지를 다시 생성한다.
		/// </summary>
		public Bitmap Regenerate()
		{
			string text = GetCurrentText();
			return GenerateImage(text);
		}

		private void EnsureWriter()
		{
			if (_writer != null) return;
			_writer = new BarcodeWriter();
		}

		/// <summary>
		/// 지정된 텍스트로 바코드/QR 이미지를 생성한다.
		/// ZXing.Net이 Bitmap을 직접 반환하므로 별도의 포맷 변환은 하지 않는다.
		/// </summary>
		public Bitmap GenerateImage(string text)
		{
			if (string.IsNullOrEmpty(text))
				return null;

			bool generated = false;

			lock (_lock)
			{
				// 동일 텍스트면 캐시 반환 (불필요한 regenerate 방지)
				if (text == _lastText && _cachedImage != null)
					return _cachedImage;

				try
				{
					int width = _config.Width > 0 ? _config.Width : 200;
					int height = _config.Height > 0 ? _config.Height : 200;

					// PDF417은 가로가 세로보다 넓어야 함 (최소 비율 보장)
					if (_config.Format == BarcodeFormat.PDF417)
					{
						if (width < 300) width = 300;
						if (height > width) height = width / 3;
					}

					int margin = Is2DFormat(_config.Format) ? 2 : 10;

					EnsureWriter();
					_writer.Format = ToZXingFormat(_config.Format);
					_writer.Options = new EncodingOptions
					{
						Width = width,
						Height = height,
						Margin = margin,
						PureBarcode = !_config.ShowHumanReadableText
					};

					if (_config.Format == BarcodeFormat.QRCode)
					{
						_writer.Options.Hints[EncodeHintType.ERROR_CORRECTION] =
							ToZXingErrorCorrection(_config.ErrorCorrectionLevel);
					}

					// ZXing이 직접 Bitmap 반환 → 별도 변환 불필요
					Bitmap bmp = _writer.Write(text);

					if (bmp != null)
					{
						_cachedImage?.Dispose();
						_cachedImage = bmp;
						_lastText = text;
						generated = true;
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"[BarcodeGenerator] Generate error: {ex.Message}");
					return null;
				}
			}

			// lock 외부에서 이벤트 발생 (deadlock 방지)
			if (generated)
			{
				try { ImageUpdated?.Invoke(this, EventArgs.Empty); }
				catch { }
			}

			return _cachedImage;
		}

		/// <summary>현재 캐시된 이미지를 반환한다.</summary>
		public Bitmap GetCachedImage()
		{
			lock (_lock)
			{
				return _cachedImage;
			}
		}

		/// <summary>
		/// 이미지를 파일로 내보낸다.
		/// </summary>
		public bool ExportToFile(string filePath, BarcodeImageFormat format = BarcodeImageFormat.PNG)
		{
			lock (_lock)
			{
				if (_cachedImage == null)
					return false;

				try
				{
					ImageFormat imgFormat = format == BarcodeImageFormat.BMP
						? ImageFormat.Bmp
						: ImageFormat.Png;

					_cachedImage.Save(filePath, imgFormat);
					return true;
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"[BarcodeGenerator] Export error: {ex.Message}");
					return false;
				}
			}
		}

		/// <summary>
		/// 2D 바코드 포맷 여부를 판별한다.
		/// </summary>
		private static bool Is2DFormat(BarcodeFormat format)
		{
			return format == BarcodeFormat.QRCode
				|| format == BarcodeFormat.DataMatrix
				|| format == BarcodeFormat.PDF417;
		}

		private static ZxBarcodeFormat ToZXingFormat(BarcodeFormat format)
		{
			switch (format)
			{
				case BarcodeFormat.QRCode: return ZxBarcodeFormat.QR_CODE;
				case BarcodeFormat.DataMatrix: return ZxBarcodeFormat.DATA_MATRIX;
				case BarcodeFormat.Code128: return ZxBarcodeFormat.CODE_128;
				case BarcodeFormat.Code39: return ZxBarcodeFormat.CODE_39;
				case BarcodeFormat.EAN13: return ZxBarcodeFormat.EAN_13;
				case BarcodeFormat.EAN8: return ZxBarcodeFormat.EAN_8;
				case BarcodeFormat.UPCA: return ZxBarcodeFormat.UPC_A;
				case BarcodeFormat.ITF14: return ZxBarcodeFormat.ITF;
				case BarcodeFormat.PDF417: return ZxBarcodeFormat.PDF_417;
				default: return ZxBarcodeFormat.QR_CODE;
			}
		}

		private static ErrorCorrectionLevel ToZXingErrorCorrection(QRErrorCorrectionLevel level)
		{
			switch (level)
			{
				case QRErrorCorrectionLevel.L: return ErrorCorrectionLevel.L;
				case QRErrorCorrectionLevel.M: return ErrorCorrectionLevel.M;
				case QRErrorCorrectionLevel.Q: return ErrorCorrectionLevel.Q;
				case QRErrorCorrectionLevel.H: return ErrorCorrectionLevel.H;
				default: return ErrorCorrectionLevel.Q;
			}
		}

		public void Dispose()
		{
			if (_disposed) return;
			_disposed = true;

			lock (_lock)
			{
				_cachedImage?.Dispose();
				_cachedImage = null;
			}
		}
	}
}
