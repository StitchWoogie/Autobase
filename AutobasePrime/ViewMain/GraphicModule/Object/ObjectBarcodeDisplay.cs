using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;
using NetTools;
using NetTools.OldDefine;
using AutoLib;
using AutoLibLocal;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZxBarcodeFormat = ZXing.BarcodeFormat;

namespace GraphicModule
{
	/// <summary>
	/// 바코드/QR 표시 오브젝트 설정 클래스.
	/// </summary>
	[Serializable]
	public class ObjectArgsBarcodeDisplay
	{
		/// <summary>바코드 형식 (0=QRCode, 1=DataMatrix, 2=Code128, 3=Code39, 4=EAN13, 5=EAN8, 6=UPC-A, 7=ITF14, 8=PDF417)</summary>
		public int nBarcodeFormat = 0;

		/// <summary>고정 텍스트 내용</summary>
		public string sTextSource = "";

		/// <summary>태그 바인딩 템플릿 (예: LOT={Tag.LOT};LINE={Tag.LINE})</summary>
		public string sTagBindingTemplate = "";

		/// <summary>태그 변경 시 자동 업데이트</summary>
		public bool bAutoUpdate = false;

		/// <summary>QR 오류 정정 수준 (0=L, 1=M, 2=Q, 3=H)</summary>
		public int nErrorCorrectionLevel = 2;

		/// <summary>이미지 자동 저장 경로 (비어 있으면 자동 저장 안함)</summary>
		public string sSavePath = "";

		/// <summary>생성 시 자동 저장 여부</summary>
		public bool bAutoSave = false;

		/// <summary>1D 바코드 하단 텍스트 표시 여부</summary>
		public bool bShowHumanReadableText = true;
	}

	/// <summary>
	/// HMI 화면에 바코드/QR 이미지를 표시하는 오브젝트.
	/// Autobase 스크린 에디터에서 바코드를 생성하고 표시할 수 있다.
	///
	/// 구조:
	///   GenerateBarcodeImage() - 바코드 바(bar)만 비트맵으로 생성
	///   DisplayObject()       - 바코드 스케일링 + 텍스트를 최종 해상도에서 직접 렌더링
	///   → 텍스트는 비트맵에 포함하지 않으므로 스케일링해도 항상 선명
	///
	/// 스크립트 사용:
	///   @ObjectCommand("BarcodeDisplay1", "BarcodeSetText", "LOT=230501;LINE=3")
	///   @ObjectCommand("BarcodeDisplay1", "BarcodeSetFormat", "QRCode")
	///   @ObjectCommand("BarcodeDisplay1", "BarcodeRefresh")
	/// </summary>
	[Serializable]
	public class ObjectBarcodeDisplay : ObjectExpand
	{
		ObjectArgsBarcodeDisplay objArgs;

		[NonSerialized]
		static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

		[NonSerialized]
		System.Windows.Forms.Form formParent;

		/// <summary>바코드 바(bar)만 포함하는 비트맵 (텍스트 없음)</summary>
		[NonSerialized]
		private Bitmap _generatedImage;

		[NonSerialized]
		private string _lastGeneratedText;

		[NonSerialized]
		private string _lastError;

		/// <summary>소스 비트맵에서 측정된 바 영역 좌측 X 좌표</summary>
		[NonSerialized]
		private int _barLeft;

		/// <summary>소스 비트맵에서 측정된 바 영역 우측 X 좌표</summary>
		[NonSerialized]
		private int _barRight;

		// Regex 컴파일 캐싱
		static readonly Regex RegexTag =
			new Regex(@"\{Tag\.([^}]+)\}", RegexOptions.Compiled);

		static readonly Regex RegexSimpleTag =
			new Regex(@"\{([^}]+)\}", RegexOptions.Compiled);

		// BarcodeWriter 재사용
		[NonSerialized]
		private BarcodeWriter _writer;

		/// <summary>현재 생성된 이미지를 외부에서 읽기 위한 속성 (저장/인쇄용)</summary>
		public Bitmap GeneratedImage { get { return _generatedImage; } }

		public ObjectArgsBarcodeDisplay ObjectArgs
		{
			set { objArgs = value; }
			get { return objArgs; }
		}

		public ObjectBarcodeDisplay(ObjectCommonProperty ocp, System.Windows.Forms.Form form, RECT rect,
			EXPAND_ID_STRUCT eid, ObjectGeneral general, ObjectArgsBarcodeDisplay args)
			: base(ocp, rect, eid, null, general)
		{
			enumObjectType = EnumObjectType.BarcodeDisplay;
			objArgs = args;
			formParent = form;

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				arrayClassList.Add(this);
				UpdateBarcodeImage();
			}
		}

		public override void Close()
		{
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				arrayClassList.Remove(this);
			}

			_generatedImage?.Dispose();
			_generatedImage = null;
		}

		/// <summary>
		/// 바코드 이미지를 생성/갱신한다.
		/// </summary>
		private void UpdateBarcodeImage()
		{
			try
			{
				string text = GetCurrentText();
				if (string.IsNullOrEmpty(text))
				{
					_lastError = null;
					return;
				}
				if (text == _lastGeneratedText && _generatedImage != null) return;

				int width = Math.Abs(nRight - nLeft) + 1;
				int height = Math.Abs(nBottom - nTop) + 1;
				if (width < 10) width = 200;
				if (height < 10) height = 200;

				var newImage = GenerateBarcodeImage(text, width, height);
				if (newImage != null)
				{
					_generatedImage?.Dispose();
					_generatedImage = newImage;
					_lastGeneratedText = text;
					_lastError = null;

					if (objArgs.bAutoSave && !string.IsNullOrEmpty(objArgs.sSavePath))
					{
						try { SaveImage(objArgs.sSavePath); } catch { }
					}
				}
			}
			catch (Exception ex)
			{
				_lastError = ex.Message;
				System.Diagnostics.Debug.WriteLine($"BarcodeDisplay 이미지 생성 오류: {ex.Message}");
			}
		}

		/// <summary>
		/// 현재 표시할 텍스트를 결정한다.
		/// </summary>
		private string GetCurrentText()
		{
			if (!string.IsNullOrEmpty(objArgs.sTagBindingTemplate))
			{
				return ResolveTagTemplate(objArgs.sTagBindingTemplate);
			}
			return objArgs.sTextSource ?? "";
		}

		/// <summary>
		/// 태그 템플릿 문자열에서 {Tag.xxx} 또는 {xxx}를 실제 태그 값으로 치환한다.
		/// 컴파일된 Regex를 사용하여 성능을 최적화한다.
		/// </summary>
		private string ResolveTagTemplate(string template)
		{
			if (string.IsNullOrEmpty(template)) return template;

			string result = RegexTag.Replace(template, match =>
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
				catch { }
				return match.Value;
			});

			result = RegexSimpleTag.Replace(result, match =>
			{
				string tagName = match.Groups[1].Value;
				if (tagName.StartsWith("Tag.")) return match.Value;
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
				catch { }
				return match.Value;
			});

			return result;
		}

		/// <summary>
		/// BarcodeWriter 인스턴스를 생성(또는 재사용)한다.
		/// </summary>
		private void EnsureWriter()
		{
			if (_writer != null) return;
			_writer = new BarcodeWriter();
		}

		/// <summary>
		/// ZXing.Net으로 바코드 바(bar)만 비트맵으로 생성한다.
		/// 텍스트는 포함하지 않으며, DisplayObject()에서 최종 해상도로 직접 렌더링한다.
		/// 바 영역 경계(_barLeft, _barRight)를 픽셀 스캔으로 측정하여 캐싱한다.
		/// </summary>
		private Bitmap GenerateBarcodeImage(string text, int width, int height)
		{
			try
			{
				string validationError = GetValidationError(text, objArgs.nBarcodeFormat);
				if (validationError != null)
				{
					_lastError = validationError;
					return null;
				}

				bool is2D = Is2DFormat(objArgs.nBarcodeFormat);
				int margin = GetMarginForFormat(objArgs.nBarcodeFormat);

				// PDF417은 가로가 세로보다 넓어야 함
				if (objArgs.nBarcodeFormat == 8)
				{
					if (width < 300) width = 300;
					if (height > width) height = width / 3;
				}

				// 1D + 텍스트 표시: 바코드 높이에서 텍스트 영역분을 제외
				bool needText = !is2D && objArgs.bShowHumanReadableText;
				int barcodeHeight = needText
					? Math.Max(20, height - Math.Max(16, height / 5))
					: height;

				EnsureWriter();
				_writer.Format = GetZXingFormat();
				_writer.Options = new EncodingOptions
				{
					Width = width,
					Height = barcodeHeight,
					Margin = margin,
					PureBarcode = true
				};

				if (objArgs.nBarcodeFormat == 0)
				{
					_writer.Options.Hints[EncodeHintType.ERROR_CORRECTION] =
						GetErrorCorrectionLevel();
				}

				using (Bitmap raw = _writer.Write(text))
				{
					// 바 영역 경계 측정 (raw가 dispose되기 전에)
					MeasureActualBarArea(raw, out _barLeft, out _barRight);

					// 바코드 바만 포함하는 비트맵 복사
					var bmp = new Bitmap(raw.Width, raw.Height, PixelFormat.Format32bppArgb);
					using (var g = Graphics.FromImage(bmp))
					{
						g.Clear(Color.White);
						g.SmoothingMode = SmoothingMode.None;
						g.InterpolationMode = InterpolationMode.NearestNeighbor;
						g.PixelOffsetMode = PixelOffsetMode.Half;
						g.DrawImageUnscaled(raw, 0, 0);
					}
					return bmp;
				}
			}
			catch (Exception ex)
			{
				_lastError = ex.Message;
				System.Diagnostics.Debug.WriteLine($"[BarcodeDisplay] ZXing error: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// 화면에 바코드를 표시한다.
		/// 바코드 이미지는 NearestNeighbor로 스케일링하고,
		/// 1D 바코드 텍스트는 최종 디스플레이 해상도에서 직접 렌더링하여 선명도를 보장한다.
		/// </summary>
		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if (x1 > x2) NetTools.Tools.Temp(ref x1, ref x2);
			if (y1 > y2) NetTools.Tools.Temp(ref y1, ref y2);

			int swidth = x2 - x1 + 1;
			int sheight = y2 - y1 + 1;

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && objArgs.bAutoUpdate)
			{
				UpdateBarcodeImage();
			}

			if (_generatedImage != null)
			{
				bool is2D = Is2DFormat(objArgs.nBarcodeFormat);
				bool drawText = !is2D && objArgs.bShowHumanReadableText;

				// 디스플레이 영역 분할: 상단 바코드 + 간격 + 하단 텍스트
				int textAreaH = drawText ? Math.Max(12, sheight / 5) : 0;
				int gap = drawText ? Math.Max(2, sheight / 30) : 0;
				int barcodeH = sheight - textAreaH - gap;

				// 바코드 이미지 (NearestNeighbor로 선명하게)
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				g.PixelOffsetMode = PixelOffsetMode.Half;
				g.DrawImage(_generatedImage, x1, y1, swidth, barcodeH);

				// 1D 바코드 하단 텍스트: 최종 해상도에서 직접 렌더링
				if (drawText && !string.IsNullOrEmpty(_lastGeneratedText))
				{
					// 간격 + 텍스트 배경을 흰색으로 채움
					using (var bgBrush = new SolidBrush(Color.White))
					{
						g.FillRectangle(bgBrush, x1, y1 + barcodeH, swidth, gap + textAreaH);
					}

					// 바 영역을 소스→디스플레이 좌표로 변환
					float scaleX = (float)swidth / _generatedImage.Width;
					float dispBarLeft = x1 + _barLeft * scaleX;
					float dispBarWidth = (_barRight - _barLeft + 1) * scaleX;

					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

					float fontSize = Math.Min(textAreaH * 0.8f, 18f);
					fontSize = Math.Max(fontSize, 8f);

					// 텍스트 폭이 바 영역을 초과하지 않도록 폰트 크기 축소
					using (var testFont = new Font("Consolas", fontSize))
					{
						SizeF measured = g.MeasureString(_lastGeneratedText, testFont);
						if (measured.Width > dispBarWidth)
						{
							fontSize = fontSize * dispBarWidth / measured.Width;
						}
					}
					fontSize = Math.Max(7f, fontSize);

					using (var font = new Font("Consolas", fontSize))
					using (var brush = new SolidBrush(Color.Black))
					{
						var sf = new StringFormat
						{
							Alignment = StringAlignment.Center,
							LineAlignment = StringAlignment.Center,
							Trimming = StringTrimming.EllipsisCharacter,
							FormatFlags = StringFormatFlags.NoWrap
						};
						// 텍스트를 실제 바 영역 위에 정렬 (gap만큼 아래로)
						var textRect = new RectangleF(
							dispBarLeft,
							y1 + barcodeH + gap,
							dispBarWidth,
							textAreaH);
						g.DrawString(_lastGeneratedText, font, brush, textRect, sf);
					}
				}
			}
			else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && _lastError != null)
			{
				// 런타임 오류
				using (var pen = new Pen(Color.Red, 1))
				{
					g.DrawRectangle(pen, x1, y1, swidth - 1, sheight - 1);
				}

				string errorMsg = _lastError;
				if (errorMsg.Length > 60) errorMsg = errorMsg.Substring(0, 57) + "...";

				float fontSize = Math.Max(7f, Math.Min(9f, swidth / 20f));
				using (var font = new Font("Arial", fontSize))
				using (var brush = new SolidBrush(Color.Red))
				{
					var sf = new StringFormat
					{
						Alignment = StringAlignment.Center,
						LineAlignment = StringAlignment.Center,
						Trimming = StringTrimming.EllipsisCharacter
					};
					g.DrawString(errorMsg, font, brush,
						new RectangleF(x1 + 2, y1 + 2, swidth - 4, sheight - 4), sf);
				}
			}
			else
			{
				// 디자인 모드 또는 이미지 없음 - 플레이스홀더
				using (var pen = new Pen(Color.Gray, 1))
				{
					g.DrawRectangle(pen, x1, y1, swidth - 1, sheight - 1);
				}

				string label = GetFormatLabel(objArgs.nBarcodeFormat);
				using (var font = new Font("Arial", 9f))
				using (var brush = new SolidBrush(Color.DarkGray))
				{
					var sf = new StringFormat
					{
						Alignment = StringAlignment.Center,
						LineAlignment = StringAlignment.Center
					};
					g.DrawString(label, font, brush,
						new RectangleF(x1, y1, swidth, sheight), sf);
				}
			}
		}

		// ─── 유틸리티 메서드 ─────────────────────────────────

		private static bool Is2DFormat(int formatIndex)
		{
			return formatIndex == 0 || formatIndex == 1 || formatIndex == 8;
		}

		private static int GetMarginForFormat(int formatIndex)
		{
			switch (formatIndex)
			{
				case 0: case 1: case 8: return 2;
				case 4: case 5: case 6: return 4;
				default: return 10;
			}
		}

		/// <summary>
		/// 렌더링된 바코드 비트맵의 실제 바 영역을 픽셀 스캔으로 측정한다.
		/// </summary>
		private static void MeasureActualBarArea(Bitmap barcodeImage, out int barLeft, out int barRight)
		{
			barLeft = 0;
			barRight = barcodeImage.Width - 1;

			if (barcodeImage == null || barcodeImage.Width < 2 || barcodeImage.Height < 2)
				return;

			int scanY = barcodeImage.Height / 2;
			int foundLeft = barcodeImage.Width;
			int foundRight = 0;

			for (int x = 0; x < barcodeImage.Width; x++)
			{
				Color pixel = barcodeImage.GetPixel(x, scanY);
				if (pixel.R < 128)
				{
					if (x < foundLeft) foundLeft = x;
					foundRight = x;
				}
			}

			if (foundRight > foundLeft)
			{
				barLeft = foundLeft;
				barRight = foundRight;
			}
		}

		private static string GetValidationError(string text, int formatIndex)
		{
			if (string.IsNullOrEmpty(text)) return "Empty input";

			switch (formatIndex)
			{
				case 4:
					if (!Regex.IsMatch(text, @"^\d{12,13}$"))
						return "EAN-13: 12~13 digits required";
					return null;
				case 5:
					if (!Regex.IsMatch(text, @"^\d{7,8}$"))
						return "EAN-8: 7~8 digits required";
					return null;
				case 6:
					if (!Regex.IsMatch(text, @"^\d{11,12}$"))
						return "UPC-A: 11~12 digits required";
					return null;
				case 7:
					if (!Regex.IsMatch(text, @"^\d+$"))
						return "ITF: digits only";
					if (text.Length % 2 != 0)
						return "ITF: even number of digits required";
					return null;
				default:
					return null;
			}
		}

		private ErrorCorrectionLevel GetErrorCorrectionLevel()
		{
			switch (objArgs.nErrorCorrectionLevel)
			{
				case 0: return ErrorCorrectionLevel.L;
				case 1: return ErrorCorrectionLevel.M;
				case 2: return ErrorCorrectionLevel.Q;
				case 3: return ErrorCorrectionLevel.H;
				default: return ErrorCorrectionLevel.Q;
			}
		}

		private ZxBarcodeFormat GetZXingFormat()
		{
			switch (objArgs.nBarcodeFormat)
			{
				case 0: return ZxBarcodeFormat.QR_CODE;
				case 1: return ZxBarcodeFormat.DATA_MATRIX;
				case 2: return ZxBarcodeFormat.CODE_128;
				case 3: return ZxBarcodeFormat.CODE_39;
				case 4: return ZxBarcodeFormat.EAN_13;
				case 5: return ZxBarcodeFormat.EAN_8;
				case 6: return ZxBarcodeFormat.UPC_A;
				case 7: return ZxBarcodeFormat.ITF;
				case 8: return ZxBarcodeFormat.PDF_417;
				default: return ZxBarcodeFormat.QR_CODE;
			}
		}

		private static string GetFormatLabel(int formatIndex)
		{
			switch (formatIndex)
			{
				case 0: return "QR Code";
				case 1: return "DataMatrix";
				case 2: return "Code128";
				case 3: return "Code39";
				case 4: return "EAN-13";
				case 5: return "EAN-8";
				case 6: return "UPC-A";
				case 7: return "ITF-14";
				case 8: return "PDF417";
				default: return "Barcode";
			}
		}

		// ─── 커맨드 / 직렬화 ─────────────────────────────────

		public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
			if (command == "BarcodeSetText")
			{
				objArgs.sTextSource = (string)args[1];
				objArgs.sTagBindingTemplate = "";
				_lastGeneratedText = null;
				UpdateBarcodeImage();
				InvalidateObject(formParent);
				return 1;
			}
			else if (command == "BarcodeSetTemplate")
			{
				objArgs.sTagBindingTemplate = (string)args[1];
				_lastGeneratedText = null;
				UpdateBarcodeImage();
				InvalidateObject(formParent);
				return 1;
			}
			else if (command == "BarcodeSetFormat")
			{
				string formatName = (string)args[1];
				int? idx = FormatNameToIndex(formatName);
				if (idx.HasValue)
					objArgs.nBarcodeFormat = idx.Value;
				_lastGeneratedText = null;
				UpdateBarcodeImage();
				InvalidateObject(formParent);
				return 1;
			}
			else if (command == "BarcodeRefresh")
			{
				_lastGeneratedText = null;
				UpdateBarcodeImage();
				InvalidateObject(formParent);
				return 1;
			}
			else if (command == "BarcodeSaveImage")
			{
				string filePath = (string)args[1];
				return SaveImage(filePath) ? 1 : 0;
			}
			else if (command == "BarcodePrint")
			{
				PrintImage();
				return 1;
			}

			return 0;
		}

		private static int? FormatNameToIndex(string formatName)
		{
			switch (formatName?.ToUpperInvariant())
			{
				case "QRCODE": case "QR": return 0;
				case "DATAMATRIX": case "DM": return 1;
				case "CODE128": return 2;
				case "CODE39": return 3;
				case "EAN13": case "EAN-13": return 4;
				case "EAN8": case "EAN-8": return 5;
				case "UPCA": case "UPC-A": return 6;
				case "ITF14": case "ITF-14": case "ITF": return 7;
				case "PDF417": return 8;
				default: return null;
			}
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			writer.Write("\tStringOption,");
			writer.Write("{0},", objArgs.nBarcodeFormat);
			writer.Write("{0},", objArgs.sTextSource ?? "");
			writer.Write("{0},", objArgs.sTagBindingTemplate ?? "");
			writer.Write("{0},", objArgs.bAutoUpdate ? 1 : 0);
			writer.Write("{0},", objArgs.nErrorCorrectionLevel);
			writer.Write("{0},", objArgs.sSavePath ?? "");
			writer.Write("{0},", objArgs.bAutoSave ? 1 : 0);
			writer.WriteLine("{0},", objArgs.bShowHumanReadableText ? 1 : 0);
		}

		public override void AddObjectInfo(TreeNode parent)
		{
			string info = Path.GetExtension(this.ToString()).Substring(1);
			string formatName = GetFormatLabel(objArgs.nBarcodeFormat);

			info += String.Format(", {0}", formatName);
			if (!string.IsNullOrEmpty(objArgs.sTagBindingTemplate))
				info += String.Format(", {0}", objArgs.sTagBindingTemplate);
			else if (!string.IsNullOrEmpty(objArgs.sTextSource))
				info += String.Format(", {0}", objArgs.sTextSource);

			TreeNode node = new TreeNode(info);
			parent.Nodes.Add(node);
		}

		public override string GetObjectMainTitle()
		{
			if (!string.IsNullOrEmpty(objArgs.sTagBindingTemplate))
				return objArgs.sTagBindingTemplate;
			return objArgs.sTextSource ?? "BarcodeDisplay";
		}

		public override void Dispose()
		{
			arrayClassList.Remove(this);
			base.Dispose();

			_generatedImage?.Dispose();
			_generatedImage = null;
		}

		// ─── 파일 저장 / 인쇄 ─────────────────────────────────

		public bool SaveImage(string filePath)
		{
			try
			{
				if (_generatedImage == null)
					UpdateBarcodeImage();

				if (_generatedImage == null) return false;

				string ext = Path.GetExtension(filePath).ToLowerInvariant();
				ImageFormat fmt;
				switch (ext)
				{
					case ".jpg": case ".jpeg": fmt = ImageFormat.Jpeg; break;
					case ".bmp": fmt = ImageFormat.Bmp; break;
					case ".gif": fmt = ImageFormat.Gif; break;
					default: fmt = ImageFormat.Png; break;
				}

				string dir = Path.GetDirectoryName(filePath);
				if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
					Directory.CreateDirectory(dir);

				_generatedImage.Save(filePath, fmt);
				return true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[BarcodeDisplay] SaveImage error: {ex.Message}");
				return false;
			}
		}

		public void PrintImage()
		{
			try
			{
				if (_generatedImage == null)
					UpdateBarcodeImage();

				if (_generatedImage == null) return;

				var pd = new System.Drawing.Printing.PrintDocument();
				Bitmap printBmp = (Bitmap)_generatedImage.Clone();

				pd.PrintPage += (s, e) =>
				{
					var area = e.MarginBounds;
					float scale = Math.Min((float)area.Width / printBmp.Width,
					                       (float)area.Height / printBmp.Height);
					int w = (int)(printBmp.Width * scale);
					int h = (int)(printBmp.Height * scale);
					int x = area.Left + (area.Width - w) / 2;
					int y = area.Top + (area.Height - h) / 2;
					e.Graphics.DrawImage(printBmp, x, y, w, h);
					printBmp.Dispose();
				};

				var dlg = new System.Windows.Forms.PrintDialog();
				dlg.Document = pd;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					pd.Print();
				}
				else
				{
					printBmp.Dispose();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[BarcodeDisplay] PrintImage error: {ex.Message}");
			}
		}

		// ─── 외부 호출 (스크립트/검색) ─────────────────────────

		public static ObjectBarcodeDisplay FindByClassName(string className)
		{
			for (int i = 0; i < arrayClassList.Count; i++)
			{
				var obj = arrayClassList[i] as ObjectBarcodeDisplay;
				if (obj == null || obj.objGeneral == null) continue;
				if (String.Compare(obj.objGeneral.sClassName, className, true) == 0)
					return obj;
			}
			return null;
		}

		public static void NotifyGenerateByName(string className, string text, int formatIndex)
		{
			for (int i = 0; i < arrayClassList.Count; i++)
			{
				var obj = arrayClassList[i] as ObjectBarcodeDisplay;
				if (obj == null || obj.objGeneral == null) continue;

				if (String.Compare(obj.objGeneral.sClassName, className, true) == 0)
				{
					obj.objArgs.sTextSource = text;
					obj.objArgs.nBarcodeFormat = formatIndex;
					obj._lastGeneratedText = null;
					obj.UpdateBarcodeImage();
					obj.InvalidateObject(obj.formParent);
				}
			}
		}
	}
}
