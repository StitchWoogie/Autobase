using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using AutoLib;
using NetTools.OldDefine;
using NetTools;
using AutoLibLocal;

namespace GraphicModule
{
	/// <summary>
	/// CE 호환 Bitmap 병합 오브젝트.
	/// .modx에서 ObjectBitmap으로 저장되어 CE에서 일반 비트맵으로 로드 가능.
	/// Desktop Studio에서는 원본 오브젝트 데이터를 이용해 병합 해제 가능.
	/// </summary>
	[Serializable]
	public class ObjectMergedBitmapSimple : ObjectExpand
	{
		ObjectArgsBitmap objArgs;

		// ObjectBitmap과 동일한 이미지 로딩
		AnimationClass animation = new AnimationClass();

		// 원본 오브젝트들의 직렬화된 데이터 (BinaryFormatter) - 해제용
		byte[] serializedOriginalObjects;
		int originalObjectCount;

		[NonSerialized]
		System.Windows.Forms.Form formParent;

		[NonSerialized]
		private string _currentLoadedPath;

		public ObjectMergedBitmapSimple(ObjectCommonProperty ocp, System.Windows.Forms.Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general,
			ObjectArgsBitmap args, byte[] serializedData, int objectCount)
			: base(ocp, rect, eid, null, general)
		{
			enumObjectType = EnumObjectType.MergedBitmapSimple;
			objArgs = args;
			serializedOriginalObjects = serializedData;
			originalObjectCount = objectCount;
			formParent = form;

			_ = SetFileName(args.sBitmapFile);
		}

		/// <summary>
		/// .modx 저장 시 ObjectBitmap으로 기록되도록 ToString 오버라이드.
		/// CE 호환성을 위해 ObjectBitmap 타입으로 저장한다.
		/// </summary>
		public override string ToString()
		{
			return "GraphicModule.ObjectBitmap";
		}

		async System.Threading.Tasks.Task<bool> SetFileName(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return false;

			string path = ObjectAnimation.MakeFilePathGraphicOnRunOrEdit(objCommonProperty, filename);

			if (string.Equals(_currentLoadedPath, path, StringComparison.OrdinalIgnoreCase))
				return false;

			try
			{
				if (objCommonProperty.bLoadByZipStream)
				{
					using (MemoryStream stream = ObjectAnimation.RestoreFromZipStream(objCommonProperty.streamZip, filename))
					{
						animation.LoadImageFromStream(stream, path);
					}
				}
				else
				{
					await animation.LoadImage(path);
				}

				_currentLoadedPath = path;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if (x1 > x2) Tools.Temp(ref x1, ref x2);
			if (y1 > y2) Tools.Temp(ref y1, ref y2);

			int swidth = x2 - x1 + 1;
			int sheight = y2 - y1 + 1;

			animation.Putimage(g, x1, y1, swidth, sheight, 0);
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			// ObjectBitmap과 동일한 필드 저장 (CE 호환)
			SaveObjectItem.FileName(writer, objArgs.sBitmapFile);
			SaveObjectItem.OverlayMethod(writer, objArgs.nOverlayMethod, objArgs.nRotateFlip);

			// 원본 오브젝트 데이터 저장 (CE에서는 LoadObjectFromModX.run()이 무시)
			writer.WriteLine("MergedOriginalCount,{0},", originalObjectCount);

			if (serializedOriginalObjects != null && serializedOriginalObjects.Length > 0)
			{
				string base64 = Convert.ToBase64String(serializedOriginalObjects);
				writer.WriteLine("MergedOriginalData,{0},", base64);
			}
		}

		/// <summary>
		/// 원본 오브젝트들의 직렬화 데이터를 반환 (Studio 해제용)
		/// </summary>
		public byte[] GetSerializedOriginalObjects()
		{
			return serializedOriginalObjects;
		}

		/// <summary>
		/// 원본 오브젝트 수를 반환
		/// </summary>
		public int GetOriginalObjectCount()
		{
			return originalObjectCount;
		}

		/// <summary>
		/// 직렬화 데이터에서 원본 오브젝트들을 역직렬화하여 반환
		/// </summary>
		public ArrayList DeserializeOriginalObjects()
		{
			if (serializedOriginalObjects == null || serializedOriginalObjects.Length == 0)
				return new ArrayList();

			try
			{
				BinaryFormatter formatter = new BinaryFormatter();
				using (MemoryStream stream = new MemoryStream(serializedOriginalObjects))
				{
					return (ArrayList)formatter.Deserialize(stream);
				}
			}
			catch (Exception)
			{
				return new ArrayList();
			}
		}

		/// <summary>
		/// Merged 데이터 로딩 (.modx에서 MergedOriginalCount/MergedOriginalData 를 이미 파싱한 후 호출)
		/// </summary>
		public void SetMergedData(byte[] serializedData, int objectCount)
		{
			serializedOriginalObjects = serializedData;
			originalObjectCount = objectCount;
		}

		public override void Close()
		{
		}

		public override void Dispose()
		{
			base.Dispose();
			this.animation.Dispose();
		}

		public override string GetObjectMainTitle()
		{
			if (Tools.IsLangKorean())
				return string.Format("CE 병합 Bitmap ({0}개)", originalObjectCount);
			else
				return string.Format("CE Merged Bitmap ({0} objects)", originalObjectCount);
		}

		/// <summary>
		/// 선택된 오브젝트들을 렌더링하여 PNG 파일로 저장
		/// </summary>
		public static string RenderAndSavePng(ArrayList objects, int bx1, int by1, int bx2, int by2, string graphicDir)
		{
			int width = bx2 - bx1 + 1;
			int height = by2 - by1 + 1;

			if (width <= 0 || height <= 0)
				return null;

			// 고유 파일명 생성
			string filename = string.Format("_merged_{0}.png", DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"));
			string fullPath = Path.Combine(graphicDir, filename);

			using (Bitmap bitmap = new Bitmap(width, height))
			{
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.CompositingMode = CompositingMode.SourceOver;
					g.Clear(Color.Transparent);

					Rectangle rcPaint = new Rectangle(0, 0, width, height);

					for (int i = 0; i < objects.Count; i++)
					{
						ObjectExpand obj = (ObjectExpand)objects[i];
						int ox1 = 0, oy1 = 0, ox2 = 0, oy2 = 0;
						obj.GetZone(ref ox1, ref oy1, ref ox2, ref oy2);

						obj.DisplayObject(g,
							ox1 - bx1,
							oy1 - by1,
							ox2 - bx1,
							oy2 - by1,
							obj.GetBorderThick());
					}
				}

				bitmap.Save(fullPath, ImageFormat.Png);
			}

			return filename;
		}
	}
}
