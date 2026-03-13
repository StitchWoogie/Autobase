using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;
using NetTools;
using NetTools.OldDefine;
using AutoLib;
using AutoLibLocal;

namespace GraphicModule
{
	/// <summary>
	/// 바코드 스캐너 상태 오브젝트 설정 클래스.
	/// </summary>
	[Serializable]
	public class ObjectArgsBarcodeScanner
	{
		/// <summary>연결된 스캐너 이름</summary>
		public string sScannerName = "";

		/// <summary>스캔 결과를 저장할 태그 이름</summary>
		public string sResultTagName = "";

		/// <summary>스캔 결과 표시 여부</summary>
		public bool bShowLastScan = true;

		/// <summary>스캐너 상태 표시 여부</summary>
		public bool bShowStatus = true;

		/// <summary>배경 색상</summary>
		public int nBackColor = Color.White.ToArgb();

		/// <summary>텍스트 색상</summary>
		public int nTextColor = Color.Black.ToArgb();

		/// <summary>유효 스캔 시 색상</summary>
		public int nValidColor = Color.Green.ToArgb();

		/// <summary>무효 스캔 시 색상</summary>
		public int nInvalidColor = Color.Red.ToArgb();
	}

	/// <summary>
	/// HMI 화면에 바코드 스캐너 상태와 마지막 스캔 결과를 표시하는 오브젝트.
	///
	/// 스크립트 사용:
	///   val = @BarcodeScannerGetLastScan("BarcodeScanner1")
	///   @BarcodeScannerEnable("BarcodeScanner1", 1)
	///   @BarcodeScannerSetName("BarcodeScanner1", "Scanner2")
	///   @BarcodeScannerSetResult("BarcodeScanner1", "SCAN_DATA")
	/// </summary>
	[Serializable]
	public class ObjectBarcodeScanner : ObjectExpand
	{
		ObjectArgsBarcodeScanner objArgs;

		[NonSerialized]
		static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

		[NonSerialized]
		System.Windows.Forms.Form formParent;

		/// <summary>마지막 스캔 값</summary>
		[NonSerialized]
		private string _lastScanValue = "";

		/// <summary>마지막 스캔 유효성</summary>
		[NonSerialized]
		private bool _lastScanValid = true;

		/// <summary>마지막 스캔 시간</summary>
		[NonSerialized]
		private DateTime _lastScanTime = DateTime.MinValue;

		/// <summary>스캐너 활성 상태</summary>
		[NonSerialized]
		private bool _scannerEnabled = true;

		public ObjectArgsBarcodeScanner ObjectArgs
		{
			set { objArgs = value; }
			get { return objArgs; }
		}

		public ObjectBarcodeScanner(ObjectCommonProperty ocp, System.Windows.Forms.Form form, RECT rect,
			EXPAND_ID_STRUCT eid, ObjectGeneral general, ObjectArgsBarcodeScanner args)
			: base(ocp, rect, eid, null, general)
		{
			enumObjectType = EnumObjectType.BarcodeScanner;
			objArgs = args;
			formParent = form;

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				arrayClassList.Add(this);
			}
		}

		public override void Close()
		{
			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				arrayClassList.Remove(this);
			}
		}

		/// <summary>
		/// 스캔 결과를 외부에서 업데이트한다.
		/// BarcodeManager의 스캔 이벤트에서 호출될 수 있다.
		/// </summary>
		public void UpdateScanResult(string value, bool isValid, DateTime scanTime)
		{
			_lastScanValue = value ?? "";
			_lastScanValid = isValid;
			_lastScanTime = scanTime;

			if (formParent != null)
			{
				InvalidateObject(formParent);
			}
		}

		/// <summary>
		/// 현재 스캐너의 마지막 스캔 결과를 BarcodeManager에서 가져온다.
		/// </summary>
		private void RefreshFromManager()
		{
			if (string.IsNullOrEmpty(objArgs.sScannerName)) return;

			try
			{
				// BarcodeManager를 리플렉션 없이 접근 (런타임에서 ScriptFunctionBarcode delegate를 통해)
				if (ScriptFunctionBarcode.procGetLastScan != null)
				{
					string val = ScriptFunctionBarcode.procGetLastScan(objArgs.sScannerName);
					if (!string.IsNullOrEmpty(val) && val != _lastScanValue)
					{
						_lastScanValue = val;
						_lastScanTime = DateTime.Now;
					}
				}
			}
			catch { }
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if (x1 > x2) NetTools.Tools.Temp(ref x1, ref x2);
			if (y1 > y2) NetTools.Tools.Temp(ref y1, ref y2);

			int swidth = x2 - x1 + 1;
			int sheight = y2 - y1 + 1;

			Color backColor = IsUseBackColor ? GetBackColor().basic_color : Color.FromArgb(objArgs.nBackColor);
			Color textColor = IsUseTextColor ? GetTextColor() : Color.FromArgb(objArgs.nTextColor);

			// 배경
			using (var brush = new SolidBrush(backColor))
			{
				g.FillRectangle(brush, x1, y1, swidth, sheight);
			}

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				RefreshFromManager();
				DrawRunModeContent(g, x1, y1, swidth, sheight, textColor);
			}
			else
			{
				DrawDesignModeContent(g, x1, y1, swidth, sheight, textColor);
			}

			// 테두리
			using (var pen = new Pen(Color.Gray, 1))
			{
				g.DrawRectangle(pen, x1, y1, swidth - 1, sheight - 1);
			}
		}

		private void DrawRunModeContent(Graphics g, int x, int y, int w, int h, Color textColor)
		{
			int padding = 4;
			int currentY = y + padding;

			// 스캐너 이름 및 상태
			if (objArgs.bShowStatus)
			{
				string statusText;
				Color statusColor;

				if (!string.IsNullOrEmpty(objArgs.sScannerName))
				{
					statusText = objArgs.sScannerName + (_scannerEnabled ? " [ON]" : " [OFF]");
					statusColor = _scannerEnabled ? Color.FromArgb(objArgs.nValidColor) : Color.Gray;
				}
				else
				{
					statusText = "Scanner [N/A]";
					statusColor = Color.Gray;
				}

				using (var font = new Font("Arial", 8f, FontStyle.Bold))
				using (var brush = new SolidBrush(statusColor))
				{
					g.DrawString(statusText, font, brush, x + padding, currentY);
					currentY += 16;
				}
			}

			// 마지막 스캔 결과
			if (objArgs.bShowLastScan && !string.IsNullOrEmpty(_lastScanValue))
			{
				Color scanColor = _lastScanValid
					? Color.FromArgb(objArgs.nValidColor)
					: Color.FromArgb(objArgs.nInvalidColor);

				using (var font = new Font("Consolas", 10f))
				using (var brush = new SolidBrush(scanColor))
				{
					var sf = new StringFormat
					{
						Trimming = StringTrimming.EllipsisCharacter
					};
					var rect = new RectangleF(x + padding, currentY, w - padding * 2, h - (currentY - y) - padding);
					g.DrawString(_lastScanValue, font, brush, rect, sf);
				}

				// 스캔 시간
				if (_lastScanTime > DateTime.MinValue)
				{
					string timeStr = _lastScanTime.ToString("HH:mm:ss");
					using (var font = new Font("Arial", 7f))
					using (var brush = new SolidBrush(Color.Gray))
					{
						var sf = new StringFormat { Alignment = StringAlignment.Far };
						g.DrawString(timeStr, font, brush, new RectangleF(x, y + h - 16, w - padding, 14), sf);
					}
				}
			}
		}

		private void DrawDesignModeContent(Graphics g, int x, int y, int w, int h, Color textColor)
		{
			string label = "Scanner";
			if (!string.IsNullOrEmpty(objArgs.sScannerName))
				label = objArgs.sScannerName;

			using (var font = new Font("Arial", 9f))
			using (var brush = new SolidBrush(Color.DarkGray))
			{
				var sf = new StringFormat
				{
					Alignment = StringAlignment.Center,
					LineAlignment = StringAlignment.Center
				};
				g.DrawString(label, font, brush, new RectangleF(x, y, w, h), sf);
			}

			// 바코드 아이콘 표시 (상단)
			int iconSize = Math.Min(w, h) / 3;
			if (iconSize > 30) iconSize = 30;
			if (iconSize < 10) iconSize = 10;
			int iconX = x + (w - iconSize) / 2;
			int iconY = y + 4;

			using (var pen = new Pen(Color.DarkGray, 1))
			{
				// 간단한 바코드 라인 아이콘
				int barX = iconX;
				int[] widths = { 2, 1, 3, 1, 2, 1, 3, 1, 2 };
				bool fill = true;
				foreach (int bw in widths)
				{
					if (fill)
					{
						using (var brush = new SolidBrush(Color.DarkGray))
						{
							g.FillRectangle(brush, barX, iconY, bw, iconSize);
						}
					}
					barX += bw;
					fill = !fill;
				}
			}
		}

		public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
			if (command == "BarcodeScannerGetLastScan")
			{
				return _lastScanValue ?? "";
			}
			else if (command == "BarcodeScannerEnable")
			{
				int enabled = Convert.ToInt32(args[1]);
				_scannerEnabled = (enabled != 0);

				// BarcodeManager에 전달
				if (ScriptFunctionBarcode.procSetScannerEnabled != null && !string.IsNullOrEmpty(objArgs.sScannerName))
				{
					ScriptFunctionBarcode.procSetScannerEnabled(objArgs.sScannerName, _scannerEnabled);
				}

				InvalidateObject(formParent);
				return 1;
			}
			else if (command == "BarcodeScannerSetName")
			{
				objArgs.sScannerName = (string)args[1];
				_lastScanValue = "";
				_lastScanTime = DateTime.MinValue;
				InvalidateObject(formParent);
				return 1;
			}
			else if (command == "BarcodeScannerSetResult")
			{
				// 외부에서 결과 강제 설정
				_lastScanValue = (string)args[1];
				_lastScanValid = true;
				_lastScanTime = DateTime.Now;

				// 태그에도 기록
				if (!string.IsNullOrEmpty(objArgs.sResultTagName))
				{
					try
					{
						_ = AutoLib.TagWrite.SetTagValue(objArgs.sResultTagName, _lastScanValue, false);
					}
					catch { }
				}

				InvalidateObject(formParent);
				return 1;
			}

			return 0;
		}

		/// <summary>
		/// className으로 ObjectBarcodeScanner를 찾아 반환한다.
		/// </summary>
		public static ObjectBarcodeScanner FindByClassName(string className)
		{
			for (int i = 0; i < arrayClassList.Count; i++)
			{
				var obj = arrayClassList[i] as ObjectBarcodeScanner;
				if (obj == null || obj.objGeneral == null) continue;
				if (String.Compare(obj.objGeneral.sClassName, className, true) == 0)
					return obj;
			}
			return null;
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			writer.Write("\tStringOption,");
			writer.Write("{0},", objArgs.sScannerName ?? "");
			writer.Write("{0},", objArgs.sResultTagName ?? "");
			writer.Write("{0},", objArgs.bShowLastScan ? 1 : 0);
			writer.Write("{0},", objArgs.bShowStatus ? 1 : 0);
			writer.Write("{0},", objArgs.nBackColor);
			writer.Write("{0},", objArgs.nTextColor);
			writer.Write("{0},", objArgs.nValidColor);
			writer.WriteLine("{0},", objArgs.nInvalidColor);
		}

		public override void AddObjectInfo(TreeNode parent)
		{
			string info = Path.GetExtension(this.ToString()).Substring(1);
			info += String.Format(", Scanner: {0}", objArgs.sScannerName ?? "N/A");

			if (!string.IsNullOrEmpty(objArgs.sResultTagName))
				info += String.Format(", Tag: {0}", objArgs.sResultTagName);

			TreeNode node = new TreeNode(info);
			parent.Nodes.Add(node);
		}

		public override string GetObjectMainTitle()
		{
			return objArgs.sScannerName ?? "BarcodeScanner";
		}

		/// <summary>
		/// 스캔 이벤트에서 이 스캐너 이름과 일치하는 모든 오브젝트를 업데이트한다.
		/// BarcodeManager에서 호출한다.
		/// </summary>
		public static void NotifyAllScanners(string scannerName, string value, bool isValid)
		{
			for (int i = 0; i < arrayClassList.Count; i++)
			{
				var obj = arrayClassList[i] as ObjectBarcodeScanner;
				if (obj != null && String.Compare(obj.objArgs.sScannerName, scannerName, true) == 0)
				{
					obj.UpdateScanResult(value, isValid, DateTime.Now);
				}
			}
		}
	}
}
