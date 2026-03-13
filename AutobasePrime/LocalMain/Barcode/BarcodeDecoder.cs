using System;
using System.Drawing;
using System.Diagnostics;
using ZXing;

namespace LocalMain.Barcode
{
	/// <summary>
	/// 바코드/QR 코드 이미지 디코더.
	/// 이미지 파일이나 Bitmap에서 바코드를 읽는다.
	///
	/// 주의: 산업 현장의 전용 스캐너는 자체적으로 디코딩을 수행하므로
	/// 이 클래스는 카메라 프레임이나 스크린 캡처 등 보조 용도로 사용한다.
	/// ZXing.Net 라이브러리가 필요하다.
	/// </summary>
	public static class BarcodeDecoder
	{
		/// <summary>
		/// 이미지 파일에서 바코드/QR 코드를 디코딩한다.
		/// </summary>
		/// <param name="imagePath">이미지 파일 경로</param>
		/// <param name="detectedFormat">감지된 바코드 형식</param>
		/// <returns>디코딩된 텍스트 (실패 시 null)</returns>
		public static string DecodeFromFile(string imagePath, out string detectedFormat)
		{
			detectedFormat = null;
			try
			{
				using (var bmp = new Bitmap(imagePath))
				{
					return DecodeFromBitmap(bmp, out detectedFormat);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BarcodeDecoder] File decode error: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// Bitmap 객체에서 바코드/QR 코드를 디코딩한다.
		/// </summary>
		/// <param name="bitmap">디코딩할 비트맵</param>
		/// <param name="detectedFormat">감지된 바코드 형식</param>
		/// <returns>디코딩된 텍스트 (실패 시 null)</returns>
		public static string DecodeFromBitmap(Bitmap bitmap, out string detectedFormat)
		{
			detectedFormat = null;
			if (bitmap == null) return null;

			try
			{
				var reader = new BarcodeReader();
				reader.AutoRotate = true;
				reader.Options.TryInverted = true;

				var result = reader.Decode(bitmap);
				if (result != null)
				{
					detectedFormat = result.BarcodeFormat.ToString();
					return result.Text;
				}

				return null;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[BarcodeDecoder] Decode error: {ex.Message}");
				return null;
			}
		}
	}
}
