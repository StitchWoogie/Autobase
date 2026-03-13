using System;

namespace LocalMain.Barcode
{
	/// <summary>
	/// 지원하는 바코드/QR 코드 유형
	/// </summary>
	public enum BarcodeFormat
	{
		QRCode = 0,
		DataMatrix = 1,
		Code128 = 2,
		Code39 = 3,
		EAN13 = 4,
		EAN8 = 5,
		UPCA = 6,
		ITF14 = 7,
		PDF417 = 8,
	}

	/// <summary>
	/// QR 코드 오류 정정 수준 (ISO/IEC 18004)
	/// </summary>
	public enum QRErrorCorrectionLevel
	{
		/// <summary>약 7% 복원 가능</summary>
		L = 0,
		/// <summary>약 15% 복원 가능</summary>
		M = 1,
		/// <summary>약 25% 복원 가능 (산업 환경 권장)</summary>
		Q = 2,
		/// <summary>약 30% 복원 가능</summary>
		H = 3,
	}

	/// <summary>
	/// 스캐너 입력 소스 유형
	/// </summary>
	public enum ScannerInputType
	{
		/// <summary>USB 키보드 웨지 스캐너</summary>
		KeyboardWedge = 0,
		/// <summary>RS232 시리얼 포트 스캐너</summary>
		SerialPort = 1,
	}

	/// <summary>
	/// 스캔 유효성 검사 결과
	/// </summary>
	public enum ScanValidationResult
	{
		Valid = 0,
		InvalidLength = 1,
		InvalidPrefix = 2,
		InvalidFormat = 3,
		InvalidChecksum = 4,
		Empty = 5,
	}

	/// <summary>
	/// 바코드 이미지 내보내기 형식
	/// </summary>
	public enum BarcodeImageFormat
	{
		PNG = 0,
		BMP = 1,
	}

	/// <summary>
	/// QR 데이터 인코딩 형식
	/// </summary>
	public enum QRDataFormat
	{
		/// <summary>일반 텍스트</summary>
		PlainText = 0,
		/// <summary>키=값 쌍 (LOT=230501;ITEM=VALVE;LINE=3)</summary>
		KeyValue = 1,
		/// <summary>JSON 형식</summary>
		JSON = 2,
	}

	/// <summary>
	/// 스캔 이벤트 데이터
	/// </summary>
	[Serializable]
	public class BarcodeScannedEventArgs : EventArgs
	{
		/// <summary>스캔된 바코드 원본 문자열</summary>
		public string RawCode { get; set; }

		/// <summary>Prefix/Suffix 제거 후 정제된 코드</summary>
		public string CleanCode { get; set; }

		/// <summary>스캔 시각</summary>
		public DateTime Timestamp { get; set; }

		/// <summary>스캐너 입력 유형</summary>
		public ScannerInputType SourceType { get; set; }

		/// <summary>스캐너 식별자 (포트 이름 또는 장치 ID)</summary>
		public string ScannerId { get; set; }

		/// <summary>유효성 검사 결과</summary>
		public ScanValidationResult ValidationResult { get; set; }

		/// <summary>유효성 검사 통과 여부</summary>
		public bool IsValid => ValidationResult == ScanValidationResult.Valid;
	}

	/// <summary>
	/// 스캔 로그 레코드 (감사 추적용)
	/// </summary>
	[Serializable]
	public class ScanLogRecord
	{
		public DateTime Timestamp { get; set; }
		public string Operator { get; set; }
		public string BarcodeValue { get; set; }
		public ScanValidationResult ValidationResult { get; set; }
		public string ScannerId { get; set; }
		public ScannerInputType SourceType { get; set; }
		public string ResultTagName { get; set; }
		public string ResultTagValue { get; set; }
		public string ScriptExecuted { get; set; }
	}
}
