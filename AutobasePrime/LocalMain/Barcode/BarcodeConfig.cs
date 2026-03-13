using System;
using System.Collections.Generic;

namespace LocalMain.Barcode
{
	/// <summary>
	/// 바코드 서브시스템 전체 설정
	/// </summary>
	[Serializable]
	public class BarcodeConfig
	{
		/// <summary>바코드 서브시스템 활성화 여부</summary>
		public bool Enabled { get; set; } = false;

		/// <summary>스캐너 설정 목록</summary>
		public List<ScannerConfig> Scanners { get; set; } = new List<ScannerConfig>();

		/// <summary>바코드 생성기 설정 목록</summary>
		public List<GeneratorConfig> Generators { get; set; } = new List<GeneratorConfig>();

		/// <summary>스캔 로그 활성화</summary>
		public bool EnableScanLog { get; set; } = true;

		/// <summary>스캔 로그 최대 보관 일수 (0=무제한)</summary>
		public int ScanLogRetentionDays { get; set; } = 90;
	}

	/// <summary>
	/// 개별 스캐너 설정
	/// </summary>
	[Serializable]
	public class ScannerConfig
	{
		/// <summary>스캐너 고유 이름</summary>
		public string Name { get; set; } = "";

		/// <summary>활성화 여부</summary>
		public bool Enabled { get; set; } = true;

		/// <summary>입력 유형</summary>
		public ScannerInputType InputType { get; set; } = ScannerInputType.KeyboardWedge;

		// ── 키보드 웨지 설정 ──

		/// <summary>스캔 시작 Prefix 문자 (예: STX, 0x02)</summary>
		public string Prefix { get; set; } = "";

		/// <summary>스캔 종료 Suffix 문자 (예: CR, LF, ETX)</summary>
		public string Suffix { get; set; } = "\r";

		/// <summary>키 입력 간 타임아웃 (밀리초). 이 시간 내 입력이 없으면 스캔 완료로 간주</summary>
		public int KeyTimeoutMs { get; set; } = 50;

		/// <summary>최소 입력 길이 (이 미만은 일반 키보드 입력으로 간주)</summary>
		public int MinLength { get; set; } = 3;

		// ── 시리얼 포트 설정 ──

		/// <summary>COM 포트 이름 (예: COM3)</summary>
		public string PortName { get; set; } = "COM1";

		/// <summary>보드레이트</summary>
		public int BaudRate { get; set; } = 9600;

		/// <summary>데이터 비트</summary>
		public int DataBits { get; set; } = 8;

		/// <summary>스톱 비트 (1, 1.5, 2)</summary>
		public float StopBits { get; set; } = 1;

		/// <summary>패리티 (None, Odd, Even)</summary>
		public string Parity { get; set; } = "None";

		/// <summary>시리얼 수신 종료 문자</summary>
		public string SerialTerminator { get; set; } = "\r\n";

		/// <summary>시리얼 수신 타임아웃 (밀리초)</summary>
		public int SerialTimeoutMs { get; set; } = 200;

		// ── 결과 매핑 ──

		/// <summary>스캔 결과를 저장할 태그 이름</summary>
		public string ResultTagName { get; set; } = "";

		// ── 유효성 검사 ──

		/// <summary>유효성 검사 규칙</summary>
		public ValidationConfig Validation { get; set; } = new ValidationConfig();
	}

	/// <summary>
	/// 바코드 유효성 검사 설정
	/// </summary>
	[Serializable]
	public class ValidationConfig
	{
		/// <summary>유효성 검사 활성화</summary>
		public bool Enabled { get; set; } = false;

		/// <summary>최소 길이 (0=검사 안 함)</summary>
		public int MinLength { get; set; } = 0;

		/// <summary>최대 길이 (0=검사 안 함)</summary>
		public int MaxLength { get; set; } = 0;

		/// <summary>필수 Prefix (빈 문자열=검사 안 함)</summary>
		public string RequiredPrefix { get; set; } = "";

		/// <summary>정규식 패턴 (빈 문자열=검사 안 함)</summary>
		public string RegexPattern { get; set; } = "";
	}

	/// <summary>
	/// 바코드 생성기 설정
	/// </summary>
	[Serializable]
	public class GeneratorConfig
	{
		/// <summary>생성기 고유 이름</summary>
		public string Name { get; set; } = "";

		/// <summary>바코드 형식</summary>
		public BarcodeFormat Format { get; set; } = BarcodeFormat.QRCode;

		/// <summary>고정 텍스트 소스 (TagBinding이 없을 때 사용)</summary>
		public string TextSource { get; set; } = "";

		/// <summary>태그 바인딩 템플릿 (예: "LOT={Tag.LOT};LINE={Tag.LINE}")</summary>
		public string TagBindingTemplate { get; set; } = "";

		/// <summary>태그 변경 시 자동 업데이트</summary>
		public bool AutoUpdate { get; set; } = true;

		/// <summary>이미지 너비 (픽셀)</summary>
		public int Width { get; set; } = 200;

		/// <summary>이미지 높이 (픽셀)</summary>
		public int Height { get; set; } = 200;

		/// <summary>QR 오류 정정 수준</summary>
		public QRErrorCorrectionLevel ErrorCorrectionLevel { get; set; } = QRErrorCorrectionLevel.Q;

		/// <summary>QR 데이터 형식</summary>
		public QRDataFormat DataFormat { get; set; } = QRDataFormat.KeyValue;

		/// <summary>모듈(셀) 크기 (픽셀, 0=자동)</summary>
		public int ModuleSize { get; set; } = 0;

		/// <summary>1D 바코드 아래에 텍스트 표시 여부 (Code128, EAN13, EAN8, UPC-A 등)</summary>
		public bool ShowHumanReadableText { get; set; } = true;
	}
}
