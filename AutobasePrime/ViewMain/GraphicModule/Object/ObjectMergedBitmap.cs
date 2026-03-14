using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Threading.Tasks;
using NetTools.OldDefine;
using NetTools;
using AutoLib;
using AutoLibLocal;

namespace GraphicModule
{
	/// <summary>
	/// 정적 오브젝트들을 Bitmap으로 병합한 오브젝트.
	/// Studio에서 병합/해제가 가능하며, 런타임에서는 캐시된 Bitmap으로 고속 렌더링한다.
	/// 확대/축소 시에는 원본 오브젝트 데이터로 재렌더링하여 화질을 유지한다.
	/// </summary>
	[Serializable]
	public class ObjectMergedBitmap : ObjectExpand
	{
		// 원본 오브젝트들의 직렬화된 데이터 (BinaryFormatter)
		byte[] serializedOriginalObjects;

		// 원본 오브젝트 수
		int originalObjectCount;

		// 원본 오브젝트들의 원래 좌표 (병합 시의 bounding rect 기준)
		int nOriginalWidth;
		int nOriginalHeight;

		// 런타임 캐시
		[NonSerialized] Bitmap cachedBitmap;
		[NonSerialized] int cachedScaleKeyX;
		[NonSerialized] int cachedScaleKeyY;
		[NonSerialized] int cachedOpticRate;

		public ObjectMergedBitmap(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general,
			byte[] serializedData, int objectCount, int origWidth, int origHeight)
			: base(ocp, rect, eid, null, general)
		{
			enumObjectType = EnumObjectType.MergedBitmap;
			serializedOriginalObjects = serializedData;
			originalObjectCount = objectCount;
			nOriginalWidth = origWidth;
			nOriginalHeight = origHeight;
		}

		public override void Close()
		{
			DisposeCachedBitmap();
		}

		public override void Dispose()
		{
			DisposeCachedBitmap();
		}

		void DisposeCachedBitmap()
		{
			if (cachedBitmap != null)
			{
				cachedBitmap.Dispose();
				cachedBitmap = null;
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
		/// 현재 배율에 맞는 Bitmap을 생성하거나 캐시에서 반환
		/// </summary>
		Bitmap GetOrCreateCachedBitmap(int displayWidth, int displayHeight)
		{
			if (displayWidth <= 0 || displayHeight <= 0)
				return null;

			// 캐시 키 비교: 화면 크기와 배율이 같으면 기존 캐시 사용
			int scaleKeyX = nScreenSizeX;
			int scaleKeyY = nScreenSizeY;
			int opticRate = wOpticRate;

			if (cachedBitmap != null &&
				cachedScaleKeyX == scaleKeyX &&
				cachedScaleKeyY == scaleKeyY &&
				cachedOpticRate == opticRate)
			{
				return cachedBitmap;
			}

			// 캐시 무효화 → 원본 오브젝트들로 재렌더링
			DisposeCachedBitmap();

			try
			{
				cachedBitmap = RenderFromOriginalObjects(displayWidth, displayHeight);
				cachedScaleKeyX = scaleKeyX;
				cachedScaleKeyY = scaleKeyY;
				cachedOpticRate = opticRate;
			}
			catch (Exception)
			{
				cachedBitmap = null;
			}

			return cachedBitmap;
		}

		/// <summary>
		/// 원본 오브젝트들을 역직렬화하여 지정 크기의 Bitmap에 렌더링
		/// </summary>
		Bitmap RenderFromOriginalObjects(int width, int height)
		{
			ArrayList objects = DeserializeOriginalObjects();
			if (objects == null || objects.Count == 0)
				return null;

			Bitmap bitmap = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.CompositingMode = CompositingMode.SourceOver;
				g.Clear(Color.Transparent);

				// 각 원본 오브젝트를 현재 크기에 맞게 렌더링
				float scaleX = (float)width / nOriginalWidth;
				float scaleY = (float)height / nOriginalHeight;

				g.ScaleTransform(scaleX, scaleY);

				Rectangle rcPaint = new Rectangle(0, 0, nOriginalWidth, nOriginalHeight);

				for (int i = 0; i < objects.Count; i++)
				{
					ObjectExpand obj = (ObjectExpand)objects[i];

					// 원본 좌표 기준으로 그리기 (병합 시 bounding rect의 left,top을 원점으로)
					obj.DisplayObject(g,
						obj.nLeft - nLeft,
						obj.nTop - nTop,
						obj.nRight - nLeft,
						obj.nBottom - nTop,
						obj.GetBorderThick());
				}
			}

			return bitmap;
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if (x1 > x2) Tools.Temp(ref x1, ref x2);
			if (y1 > y2) Tools.Temp(ref y1, ref y2);

			int displayWidth = x2 - x1 + 1;
			int displayHeight = y2 - y1 + 1;

			Bitmap bmp = GetOrCreateCachedBitmap(displayWidth, displayHeight);
			if (bmp != null)
			{
				g.DrawImage(bmp, x1, y1, displayWidth, displayHeight);
			}
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			writer.WriteLine("OriginalObjectCount,{0},", originalObjectCount);
			writer.WriteLine("OriginalSize,{0},{1},", nOriginalWidth, nOriginalHeight);

			if (serializedOriginalObjects != null && serializedOriginalObjects.Length > 0)
			{
				string base64 = Convert.ToBase64String(serializedOriginalObjects);
				writer.WriteLine("OriginalData,{0},", base64);
			}
		}

		/// <summary>
		/// .modx 파일에서 ObjectMergedBitmap 고유 데이터를 로딩
		/// </summary>
		public void LoadMergedData(TextReader reader, string command)
		{
			CommaTextReader comma = new CommaTextReader();
			string buf = "";

			while (true)
			{
				buf = reader.ReadLine();
				if (buf == null) break;

				comma.Set(buf);
				comma.GetString(ref buf);

				if (buf == "OriginalObjectCount")
				{
					comma.GetInt(ref originalObjectCount);
				}
				else if (buf == "OriginalSize")
				{
					comma.GetInt(ref nOriginalWidth);
					comma.GetInt(ref nOriginalHeight);
				}
				else if (buf == "OriginalData")
				{
					string base64 = "";
					comma.GetString(ref base64);
					if (!string.IsNullOrEmpty(base64))
					{
						serializedOriginalObjects = Convert.FromBase64String(base64);
					}
				}
				else if (buf == command)
				{
					break;
				}
			}
		}

		public override string GetObjectMainTitle()
		{
			if (Tools.IsLangKorean())
				return string.Format("병합 Bitmap ({0}개 오브젝트)", originalObjectCount);
			else
				return string.Format("Merged Bitmap ({0} objects)", originalObjectCount);
		}
	}
}
