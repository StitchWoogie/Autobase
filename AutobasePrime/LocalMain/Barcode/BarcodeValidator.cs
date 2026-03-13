using System;
using System.Text.RegularExpressions;

namespace LocalMain.Barcode
{
	/// <summary>
	/// 바코드 유효성 검사기.
	/// 길이, Prefix, 정규식 패턴을 기반으로 스캔된 바코드를 검증한다.
	/// 산업 현장에서는 잘못된 바코드로 인한 오작동을 방지하기 위해
	/// 반드시 프로세스 동작 전에 유효성 검사를 수행해야 한다.
	/// </summary>
	public static class BarcodeValidator
	{
		/// <summary>
		/// 주어진 검증 규칙에 따라 바코드 값을 검증한다.
		/// </summary>
		/// <param name="code">검증할 바코드 문자열</param>
		/// <param name="config">검증 규칙 설정</param>
		/// <returns>검증 결과</returns>
		public static ScanValidationResult Validate(string code, ValidationConfig config)
		{
			if (config == null || !config.Enabled)
				return ScanValidationResult.Valid;

			if (string.IsNullOrEmpty(code))
				return ScanValidationResult.Empty;

			// 1. 길이 검사
			if (config.MinLength > 0 && code.Length < config.MinLength)
				return ScanValidationResult.InvalidLength;

			if (config.MaxLength > 0 && code.Length > config.MaxLength)
				return ScanValidationResult.InvalidLength;

			// 2. Prefix 검사
			if (!string.IsNullOrEmpty(config.RequiredPrefix))
			{
				if (!code.StartsWith(config.RequiredPrefix, StringComparison.Ordinal))
					return ScanValidationResult.InvalidPrefix;
			}

			// 3. 정규식 패턴 검사
			if (!string.IsNullOrEmpty(config.RegexPattern))
			{
				try
				{
					if (!Regex.IsMatch(code, config.RegexPattern))
						return ScanValidationResult.InvalidFormat;
				}
				catch (ArgumentException)
				{
					// 잘못된 정규식 → 검증 통과 처리 (설정 오류이므로 운영을 차단하지 않음)
					System.Diagnostics.Debug.WriteLine(
						$"[BarcodeValidator] Invalid regex pattern: {config.RegexPattern}");
				}
			}

			return ScanValidationResult.Valid;
		}

		/// <summary>
		/// 바코드에서 Prefix/Suffix 문자를 제거한 정제된 문자열을 반환한다.
		/// </summary>
		public static string CleanCode(string rawCode, string prefix, string suffix)
		{
			if (string.IsNullOrEmpty(rawCode))
				return "";

			string result = rawCode;

			// Prefix 제거
			if (!string.IsNullOrEmpty(prefix) &&
				result.StartsWith(prefix, StringComparison.Ordinal))
			{
				result = result.Substring(prefix.Length);
			}

			// Suffix 제거
			if (!string.IsNullOrEmpty(suffix) &&
				result.EndsWith(suffix, StringComparison.Ordinal))
			{
				result = result.Substring(0, result.Length - suffix.Length);
			}

			return result.Trim();
		}
	}
}
