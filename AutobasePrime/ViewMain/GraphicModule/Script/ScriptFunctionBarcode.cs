using System;
using System.Collections.Generic;
using AutoLibLocal;

namespace GraphicModule
{
	/// <summary>
	/// Barcode / QR Code script functions.
	///
	/// Supported formats: QRCode, DataMatrix, Code128, Code39, EAN13, EAN8, UPCA, ITF14, PDF417
	///
	/// Script usage examples:
	///   result = @BarcodeScannerGetLastScan("BarcodeScanner1")
	///   @BarcodeScannerEnable("BarcodeScanner1", 1)
	///   @BarcodeScannerSetName("BarcodeScanner1", "MyScanner")
	///   @BarcodeScannerSetResult("BarcodeScanner1", "SCAN_DATA")
	///   value  = @BarcodeExtractValue(barcode, "LOT")
	///   @BarcodeGenerate("BarcodeDisplay1", "LOT=230501", "QRCode")
	///   @BarcodeGenerate("BarcodeDisplay1", "4901234567890", "EAN13")
	///   @BarcodeGenerate("BarcodeDisplay1", "12345678", "EAN8")
	///   @BarcodeGenerate("BarcodeDisplay1", "PDF417 data", "PDF417")
	///   @BarcodeSaveImage("BarcodeDisplay1", "C:\output\barcode.png")
	///   @BarcodePrint("BarcodeDisplay1")
	///   @BarcodeExportImage("Gen1", "C:\output\barcode.png")
	/// </summary>
	public class ScriptFunctionBarcode
	{
		// Delegates (LocalMain provides implementations)
		public delegate string DelegateGetLastScan(string scannerName);
		public delegate string DelegateExtractValue(string barcodeText, string key);
		public delegate void DelegateSetScannerEnabled(string scannerName, bool enabled);
		public delegate bool DelegateExportImage(string generatorName, string filePath);

		public static DelegateGetLastScan procGetLastScan = null;
		public static DelegateExtractValue procExtractValue = null;
		public static DelegateSetScannerEnabled procSetScannerEnabled = null;
		public static DelegateExportImage procExportImage = null;

		#region BarcodeScanner commands (className-based)

		/// <summary>
		/// @BarcodeScannerGetLastScan(className) : string
		/// </summary>
		static int Run_BarcodeScannerGetLastScan(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = "";
			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1;

			string className = (string)args[0];
			var obj = ObjectBarcodeScanner.FindByClassName(className);
			if (obj != null)
				val = obj.ExecuteClassName(false, "BarcodeScannerGetLastScan").Result ?? "";

			return 1;
		}

		/// <summary>
		/// @BarcodeScannerEnable(className, enabled) : void
		/// </summary>
		static int Run_BarcodeScannerEnable(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1;

			string className = (string)args[0];
			int enabled = Convert.ToInt32(args[1]);
			var obj = ObjectBarcodeScanner.FindByClassName(className);
			if (obj != null)
				val = obj.ExecuteClassName(false, "BarcodeScannerEnable", null, enabled).Result;

			return 1;
		}

		/// <summary>
		/// @BarcodeScannerSetName(className, scannerName) : void
		/// </summary>
		static int Run_BarcodeScannerSetName(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1;

			string className = (string)args[0];
			string scannerName = (string)args[1];
			var obj = ObjectBarcodeScanner.FindByClassName(className);
			if (obj != null)
				val = obj.ExecuteClassName(false, "BarcodeScannerSetName", null, scannerName).Result;

			return 1;
		}

		/// <summary>
		/// @BarcodeScannerSetResult(className, result) : void
		/// </summary>
		static int Run_BarcodeScannerSetResult(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1;

			string className = (string)args[0];
			string result = (string)args[1];
			var obj = ObjectBarcodeScanner.FindByClassName(className);
			if (obj != null)
				val = obj.ExecuteClassName(false, "BarcodeScannerSetResult", null, result).Result;

			return 1;
		}

		#endregion

		#region BarcodeDisplay commands (className-based)

		/// <summary>
		/// @BarcodeGenerate(className, text, format) : int
		/// </summary>
		static int Run_BarcodeGenerate(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string className = (string)args[0];
			string text = (string)args[1];
			string format = args.Length > 2 ? (string)args[2] : "QRCode";

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				int formatIndex = FormatStringToIndex(format);
				ObjectBarcodeDisplay.NotifyGenerateByName(className, text, formatIndex);
				val = 1;
			}

			return 1;
		}

		/// <summary>
		/// @BarcodeSaveImage(className, filePath) : int
		/// </summary>
		static int Run_BarcodeSaveImage(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1;

			string className = (string)args[0];
			string filePath = (string)args[1];
			var obj = ObjectBarcodeDisplay.FindByClassName(className);
			if (obj != null)
				val = obj.SaveImage(filePath) ? 1 : 0;

			return 1;
		}

		/// <summary>
		/// @BarcodePrint(className) : void
		/// </summary>
		static int Run_BarcodePrint(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1;

			string className = (string)args[0];
			var obj = ObjectBarcodeDisplay.FindByClassName(className);
			if (obj != null)
			{
				obj.PrintImage();
				val = 1;
			}

			return 1;
		}

		#endregion

		#region Utility functions

		/// <summary>
		/// @BarcodeExtractValue(barcodeText, key) : string
		/// </summary>
		static int Run_BarcodeExtractValue(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = "";
			string barcodeText = (string)args[0];
			string key = (string)args[1];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procExtractValue != null)
					val = procExtractValue(barcodeText, key) ?? "";
			}

			return 1;
		}

		/// <summary>
		/// @BarcodeExportImage(generatorName, filePath) : int
		/// </summary>
		static int Run_BarcodeExportImage(ScriptClass scriptClass, string method_name, out object val, object[] args)
		{
			val = 0;
			string generatorName = (string)args[0];
			string filePath = (string)args[1];

			if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
			{
				if (procExportImage != null)
					val = procExportImage(generatorName, filePath) ? 1 : 0;
			}

			return 1;
		}

		/// <summary>
		/// 포맷 문자열을 ObjectBarcodeDisplay의 인덱스로 변환.
		/// </summary>
		static int FormatStringToIndex(string format)
		{
			switch (format?.ToUpperInvariant())
			{
				case "DATAMATRIX": case "DM": return 1;
				case "CODE128": return 2;
				case "CODE39": return 3;
				case "EAN13": case "EAN-13": return 4;
				case "EAN8": case "EAN-8": return 5;
				case "UPCA": case "UPC-A": return 6;
				case "ITF14": case "ITF-14": case "ITF": return 7;
				case "PDF417": return 8;
				default: return 0; // QRCode
			}
		}

		#endregion

		/// <summary>
		/// Register all barcode script functions with the script engine.
		/// </summary>
		public static void PrepareMethod(ScriptExternalRun prepare)
		{
			string prename = "Barcode";

			// --- BarcodeScanner (className-based) ---
			prepare.AddMethod(prename, "BarcodeScannerGetLastScan", "string",
				new ScriptExternalRun.DeleMethod(Run_BarcodeScannerGetLastScan),
				"in:string:class_name");

			prepare.AddMethod(prename, "BarcodeScannerEnable", "void",
				new ScriptExternalRun.DeleMethod(Run_BarcodeScannerEnable),
				"in:string:class_name", "in:int:enabled");

			prepare.AddMethod(prename, "BarcodeScannerSetName", "void",
				new ScriptExternalRun.DeleMethod(Run_BarcodeScannerSetName),
				"in:string:class_name", "in:string:scanner_name");

			prepare.AddMethod(prename, "BarcodeScannerSetResult", "void",
				new ScriptExternalRun.DeleMethod(Run_BarcodeScannerSetResult),
				"in:string:class_name", "in:string:result");

			// --- BarcodeDisplay (className-based) ---
			prepare.AddMethod(prename, "BarcodeGenerate", "int",
				new ScriptExternalRun.DeleMethod(Run_BarcodeGenerate),
				"in:string:class_name", "in:string:text", "in:string:format");

			prepare.AddMethod(prename, "BarcodeSaveImage", "int",
				new ScriptExternalRun.DeleMethod(Run_BarcodeSaveImage),
				"in:string:class_name", "in:string:file_path");

			prepare.AddMethod(prename, "BarcodePrint", "void",
				new ScriptExternalRun.DeleMethod(Run_BarcodePrint),
				"in:string:class_name");

			// --- Utility ---
			prepare.AddMethod(prename, "BarcodeExtractValue", "string",
				new ScriptExternalRun.DeleMethod(Run_BarcodeExtractValue),
				"in:string:barcode_text", "in:string:key");

			prepare.AddMethod(prename, "BarcodeExportImage", "int",
				new ScriptExternalRun.DeleMethod(Run_BarcodeExportImage),
				"in:string:generator_name", "in:string:file_path");
		}
	}
}
