using System;
using GraphicModule;
using System.IO; 
using System.Windows.Forms;
using NetTools.OldDefine;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditImport.
	/// </summary>
	public class ClassEditImportWMF
	{
		public ClassEditImportWMF()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		class METAHEADER 
		{
			public ushort mtType;
			public ushort mtHederSize;
			public ushort mtVersion;
			public uint   mtSize;
			public ushort mtNoObjects;
			public uint   mtMaxRecord;
			public ushort mtNoParameters;

			public void Read(BinaryReader reader)
			{
				mtType = reader.ReadUInt16();
				mtHederSize = reader.ReadUInt16();
				mtVersion = reader.ReadUInt16();
				mtSize = reader.ReadUInt32();
				mtNoObjects = reader.ReadUInt16();
				mtMaxRecord = reader.ReadUInt32();
				mtNoParameters = reader.ReadUInt16();
			}
		}

		class RECT16 
		{
			public short	left;
			public short	top;
			public short	right;
			public short	bottom;

			public void Read(BinaryReader reader)
			{
				left = reader.ReadInt16();
				top = reader.ReadInt16();
				right = reader.ReadInt16();
				bottom = reader.ReadInt16();
			}
		}

		class POINT16 
		{
			public short	x;
			public short	y;

			public void Read(BinaryReader reader)
			{
				x = reader.ReadInt16();
				y = reader.ReadInt16();
			}
		}

		class LOGBRUSH16	// 8byte
		{
			public ushort	lbStyle;
			public uint		lbColor;
			public short	lbHatch;

			public void Read(BinaryReader reader)
			{
				lbStyle = reader.ReadUInt16();
				lbColor = reader.ReadUInt32();
				lbHatch = reader.ReadInt16();
			}
		}

		class LOGPEN16	// 10 byte
		{
			public ushort lopnStyle;
			public POINT16 lopnWidth = new POINT16();
			public uint   lopnColor;

			public void Read(BinaryReader reader)
			{
				lopnStyle = reader.ReadUInt16();
				lopnWidth.Read(reader);
				lopnColor = reader.ReadUInt32();
			}
		}

		class LOGFONT16
		{
			public short   lfHeight;
			public short   lfWidth;
			public short   lfEscapement;
			public short   lfOrientation;
			public short   lfWeight;
			public byte    lfItalic;
			public byte    lfUnderline;
			public byte    lfStrikeOut;
			public byte    lfCharSet;
			public byte    lfOutPrecision;
			public byte    lfClipPrecision;
			public byte    lfQuality;
			public byte    lfPitchAndFamily;
			public byte[] lfFaceName = new byte[32];// [LF_FACESIZE];

			public void Read(BinaryReader reader) 
			{
				lfHeight = reader.ReadInt16();
				lfWidth = reader.ReadInt16();
				lfEscapement = reader.ReadInt16();
				lfOrientation = reader.ReadInt16();
				lfWeight = reader.ReadInt16();
				lfItalic  = reader.ReadByte();
				lfUnderline  = reader.ReadByte();
				lfStrikeOut  = reader.ReadByte();
				lfCharSet  = reader.ReadByte();
				lfOutPrecision  = reader.ReadByte();
				lfClipPrecision  = reader.ReadByte();
				lfQuality  = reader.ReadByte();
				lfPitchAndFamily  = reader.ReadByte();
				lfFaceName = reader.ReadBytes(32);// [LF_FACESIZE];

				string name = Tools.BytesToString(lfFaceName);
			}
		}

		class PLACEABLE_METAFILE_HEADER 
		{
			public uint     key;
			public ushort   hmf;
			public RECT16   bbox = new RECT16();
			public ushort   inch;
			public uint     reserved;
			public ushort   checksum;

			public void Read(BinaryReader reader)
			{
				key = reader.ReadUInt32();
				hmf = reader.ReadUInt16();
				bbox.Read(reader);
				inch = reader.ReadUInt16();
				reserved = reader.ReadUInt32();
				checksum = reader.ReadUInt16();
			}
		}

		class METARECORD_HEADER 
		{
			public uint rdSize;
			public ushort rdFunction;

			public void Read(BinaryReader reader)
			{
				rdSize = reader.ReadUInt32();
				rdFunction = reader.ReadUInt16();
			}
		}

		const uint PLACEABLE_METAFILE_KEY = 0x9AC6CDD7;

		static bool IsPlaceableMetaFile(BinaryReader reader)
		{
			uint dwPlaceableMetafileKey;
			long lPosition;

			// save the original position in the file
			lPosition = reader.BaseStream.Position;;

			// read the first DWORD of the file 
			dwPlaceableMetafileKey = reader.ReadUInt32();

			// seek back to the original position in the file 
			reader.BaseStream.Position = lPosition;

			return (dwPlaceableMetafileKey == PLACEABLE_METAFILE_KEY);
		}

		static bool IsBitmapFile(BinaryReader reader)
		{
			ushort bfType;
			long lPosition;

			// save the original position in the file
			lPosition = reader.BaseStream.Position;;

			// read the first DWORD of the file 
			bfType = reader.ReadUInt16();

			// seek back to the original position in the file 
			reader.BaseStream.Position = lPosition;

			return (bfType == 0x4D42);	// BM
		}

		static short nMapMode=0;

		static short CalcX(short x)
		{
			return x;
		}

		static short CalcY(short y)
		{
			if(nMapMode == 8)	return y;
			else				return (short)(-y);
		}

		static Color MakeColor(int color)
		{
			int r, g, b;
			r = (int)((color >>  0) & 0xFF);
			g = (int)((color >>  8) & 0xFF);
			b = (int)((color >> 16) & 0xFF);
			return Color.FromArgb(r, g, b);
		}

		static void AddToArray(ArrayList array, object obj)
		{
			for(int i = 0; i < array.Count; i++) 
			{
				if(array[i] == null) 
				{
					array[i] = obj;
					return;
				}
				
			}

			array.Add(obj);
		}

		// 라인이 PS_NULL일때는 안그리는것 보다는 채움색과 같은 칼라로 1라인 그려주는 것이 좋다.
		static void FitLineStyle(Color fillcolor, ref Color linecolor, ref int linethick)
		{
			if(linecolor.ToArgb() == 0) 
			{
				linecolor = fillcolor;
				linethick = 1;
			}
		}

		static ObjectGroup ProcessMetafileRecords(FormEditGraphic form, BinaryReader reader, string filename)
		{
			nMapMode = 0;		// 전역변수이므로 초기화를 해야한다.
			bool bDone;
			bool bNewRecord = false;

			bDone = false;

			POINT16 point16 = new POINT16();
			RECT16 rect16 = new RECT16();
			LOGPEN16 pen16;
			LOGBRUSH16 brush16;
			ArrayList blockPoly;
			ArrayList blockCurve;
			int count;
			Point point;
			Color colorLine = Color.Black;
			Color colorFill = Color.White;
			Color colorText = Color.Black;
			Color colorBack = Color.White;
			LOGFONT fontStruct = new LOGFONT();
			RECT rect = new RECT();
			CURVE_STRUCT curve;
			int nLineThick = 1;
			short val = 0;
			ArrayList arrayObject = new ArrayList();

            ObjectGroup group = new ObjectGroup(null, null, null, null, null);
			object obj;
			METARECORD_HEADER MetaRecordHeader = new METARECORD_HEADER();

			int x, y;
			int movex=0, movey=0;
			Color lcolor;
			int   lthick;
			int width, height;

			while (!bDone && reader.BaseStream.Position < reader.BaseStream.Length) 
			{
				long old_position = reader.BaseStream.Position;
				MetaRecordHeader.Read(reader);

				switch(MetaRecordHeader.rdFunction) 
				{
					case 0:
						bDone = true;
						break;
					case 0x01E:	// SAVEDC	인자없다.
						break;
					case 0x035:	// REALIZEPALETTE	인자없다.
						break;
					case 0x0F7:	// CREATEPALETTE	
						AddToArray(arrayObject, "Palette");	// 파레트도 리소스 할당
						break;
					case 0x102:	// "SETBKMODE", 2byte
						reader.ReadInt16();	// BK MODE
						break;
					case 0x103:	// "SETMAPMODE", 2byte
						nMapMode = reader.ReadInt16();	// MAP MODE
														// MM_ANISOTROPIC = 8
														// 
						break;
					case 0x104:	// "SETROP2", 2byte
						reader.ReadInt16();	// DRAW MODE
						break;
					case 0x105:	// "SETRELABS", 2byte
						reader.ReadInt16();	// 
						break;
					case 0x106:	// "SETPOLYFILLMODE",      
						val = reader.ReadInt16();	// FILL MODE
						break;
					case 0x107:	// "SETSTRETCHBLTMODE",      
						reader.ReadInt16();	//  STRETCHBLTMODE
						break;
					case 0x127:	// "RESTOREDC",
						reader.ReadInt16();	//
						break;
					case 0x12D:	// "SELECTOBJECT",
						val = reader.ReadInt16();	// HGDIOBJ

						if(val < arrayObject.Count) 
						{
							obj = arrayObject[val];
							string obj_name = obj.ToString();
							if(obj_name.IndexOf("LOGPEN16") != -1) 
							{
								pen16 = (LOGPEN16)obj;				
								if(pen16.lopnStyle == 5) // PS_NULL
								{
									colorLine = Color.FromArgb(0);
								}
								else 
								{
									// A가0이므로 그냥대입하면 안된다.
									colorLine = MakeColor((int)pen16.lopnColor);
								}

								if(pen16.lopnWidth.x == 0)
									nLineThick = 1;
								else
									nLineThick = pen16.lopnWidth.x;	// 	
							}
							else if(obj_name.IndexOf("LOGBRUSH16") != -1) 
							{
								brush16 = (LOGBRUSH16)obj;				

								if(brush16.lbStyle == 1) // BS_NULL
								{
									colorFill = Color.FromArgb(0);
								}
								else 
								{
									// A가0이므로 그냥대입하면 안된다.
									colorFill = MakeColor((int)brush16.lbColor);		
								}
							}
							else if(obj_name.IndexOf("LOGFONT16") != -1) 
							{
								LOGFONT16 lf = (LOGFONT16)obj;

								fontStruct.lfFaceName = Tools.BytesToString(lf.lfFaceName);
								fontStruct.lfHeight = (int)((-lf.lfHeight*(Double)72.0/96.0)+0.5);

								fontStruct.style = 0;

								if(lf.lfWeight == 700)
									fontStruct.style |= FontStyle.Bold;
								if(lf.lfItalic == 1)
									fontStruct.style |= FontStyle.Italic;
								if(lf.lfUnderline == 1)
									fontStruct.style |= FontStyle.Underline;
								if(lf.lfStrikeOut == 1)
									fontStruct.style |= FontStyle.Strikeout;

								fontStruct.lfHeight = Math.Abs(fontStruct.lfHeight);
							}
						}
						break;
					case 0x12E:	// "SETTEXTALIGN",
						reader.ReadInt16();	//
						break;

					case 0x1F0:	// "DELETEOBJECT",
						val = reader.ReadInt16();	//HGDIOBJ
						if(val < arrayObject.Count)
							arrayObject[val] = null;
						break;

					case 0x201:	// SETBKCOLOR
						colorBack = MakeColor(reader.ReadInt32());
						break;
					case 0x209:	// SETTEXTCOLOR
						colorText = MakeColor(reader.ReadInt32());
						break;
						
					case 0x20B:	// "SETWINDOWORG",
						point16.Read(reader);
						break;
					case 0x20C:	// "SETWINDOWEXT",
						point16.Read(reader);
						break;
					case 0x20A:	// "SETTEXTJUSTIFICATION",
						val = reader.ReadInt16();	// nBreakExtra 
						val = reader.ReadInt16();	// nBreakCount 
						break;
					case 0x213:	// LINETO
						y = CalcY(reader.ReadInt16());
						x = CalcX(reader.ReadInt16());
						
						rect.left = x;
						rect.top = y;
						rect.right = movex;
						rect.bottom = movey;

						group.AddObject(new ObjectLine(null, rect, null, new ObjectGeneral("Line1"), colorLine, new BrushSolid(colorFill), 1, nLineThick));

						movex = x;
						movey = y;
						break;
					case 0x214:	// MOVETO
						movey = CalcY(reader.ReadInt16());
						movex = CalcX(reader.ReadInt16());
						break;
					case 0x234:	// "SELECTPALETTE",
						val = reader.ReadInt16();
						break;
					case 0x2FA:	// "CREATEPENINDIRECT",    
						pen16 = new LOGPEN16();
						pen16.Read(reader);

						AddToArray(arrayObject, pen16);

						break;
					case 0x2FB:	// "CREATEFONTINDIRECT",
						LOGFONT16 lf16 = new LOGFONT16();
						lf16.Read(reader);

						AddToArray(arrayObject, lf16);

						break;
					case 0x2FC:	// "CREATEBRUSHINDIRECT",
						brush16 = new LOGBRUSH16();
						brush16.Read(reader);

						AddToArray(arrayObject, brush16);

						break;
					case 0x324:	// "POLYGON"
						count = reader.ReadInt16();
						blockPoly = new ArrayList();
						for(int i = 0; i < count; i++) 
						{
							point = new Point();
							point.X = CalcX(reader.ReadInt16());
							point.Y = CalcY(reader.ReadInt16());
							blockPoly.Add(point);
						}
						lcolor = colorLine;
						lthick = nLineThick;
						FitLineStyle(colorFill, ref lcolor, ref lthick);
						group.AddObject(new ObjectPoly(null, form, null, null, new ObjectGeneral("Poly1"), lcolor, new BrushSolid(colorFill), 1, lthick, blockPoly));
						break;
					case 0x325:	// "POLYLINE"	연결하지 않은 라인이다. 곡선 오브젝트를 사용하는 것이 낫다.
						count = reader.ReadInt16();
						blockCurve = new ArrayList();
						for(int i = 0; i < count; i++) 
						{
							curve = new CURVE_STRUCT();
							curve.x[0] = CalcX(reader.ReadInt16());
							curve.y[0] = CalcY(reader.ReadInt16());
							if(i == 0)
								curve.type = EnumCurveType.START;
							else
								curve.type = EnumCurveType.LINE;
				
							blockCurve.Add(curve);
						}
						group.AddObject(new ObjectCurve(null, form, null, null, new ObjectGeneral("Poly1"), colorLine, new BrushSolid(Color.Transparent), 1, nLineThick, blockCurve));
						break;
					case 0x416:	// INTERSECTCLIPRECT
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						break;

					case 0x418:	// "ELLIPSE",              
						rect.top = CalcY(reader.ReadInt16());
						rect.left = CalcX(reader.ReadInt16());
						rect.bottom = CalcY(reader.ReadInt16());
						rect.right = CalcX(reader.ReadInt16());
						lcolor = colorLine;
						lthick = nLineThick;
						FitLineStyle(colorFill, ref lcolor, ref lthick);
						group.AddObject(new ObjectCircle(null, rect, null, new ObjectGeneral("Circle1"), lcolor, new BrushSolid(colorFill), 1, lthick, null));
						break;
					case 0x41B:	// "RECTANGLE",              
						rect.top = CalcY(reader.ReadInt16());
						rect.left = CalcX(reader.ReadInt16());
						rect.bottom = CalcY(reader.ReadInt16());
						rect.right = CalcX(reader.ReadInt16());
						lcolor = colorLine;
						lthick = nLineThick;
						FitLineStyle(colorFill, ref lcolor, ref lthick);
						group.AddObject(new ObjectRectangle(null, rect, null, new ObjectGeneral("Rect1"), lcolor, new BrushSolid(colorFill), 1, lthick, new ObjectArgsRectangle()));
						break;
					case 0x538:	// POLYPOLYGON  다중POLY이다.
						int[] array_count;
						count = reader.ReadInt16();	// poly의 개수
						array_count = new int[count];	
						for(int i = 0; i < count; i++) 
						{
							array_count[i] = reader.ReadInt16();	// 각 Poly의 Point수
						}
						blockCurve = new ArrayList();
						for(int i = 0; i < count; i++) 
						{
							for(int j = 0; j < array_count[i]; j++) 
							{
								curve = new CURVE_STRUCT();
								curve.x[0] = CalcX(reader.ReadInt16());
								curve.y[0] = CalcY(reader.ReadInt16());
								if(j == 0)
									curve.type = EnumCurveType.START;
								else
									curve.type = EnumCurveType.LINE;

								if(j == array_count[i]-1)	// 마지막 점
								{
									curve.type |= EnumCurveType.CLOSE;
								}
				
								blockCurve.Add(curve);
							}
						}
						lcolor = colorLine;
						lthick = nLineThick;
						FitLineStyle(colorFill, ref lcolor, ref lthick);
						group.AddObject(new ObjectCurve(null, form, null, null, new ObjectGeneral("Poly1"), lcolor, new BrushSolid(colorFill), 1, lthick, blockCurve));
						break;
					case 0x61C:	// "ROUNDRECT",              
					{						
						ObjectArgsRoundRectangle args = new ObjectArgsRoundRectangle();

						args.round_y = reader.ReadInt16();
						args.round_x = reader.ReadInt16();

						rect.top = CalcY(reader.ReadInt16());
						rect.left = CalcX(reader.ReadInt16());
						rect.bottom = CalcY(reader.ReadInt16());
						rect.right = CalcX(reader.ReadInt16());
												
						lcolor = colorLine;
						lthick = nLineThick;
						FitLineStyle(colorFill, ref lcolor, ref lthick);
						group.AddObject(new ObjectRoundRectangle(null, rect, null, new ObjectGeneral("RoundRect1"), lcolor, new BrushSolid(colorFill), 1, lthick, args));
						break;
					}
					case 0x61D:	// PATBLT
						int rop = reader.ReadInt32();	// ROP 21=PATCOPY

						height = reader.ReadInt16();
						width  = reader.ReadInt16();
						rect.top = CalcY(reader.ReadInt16());
						rect.left = CalcX(reader.ReadInt16());
						
						rect.bottom = rect.top+height-1;
						rect.right = rect.left+width-1;

						lcolor = colorLine;
						lthick = nLineThick;
						FitLineStyle(colorFill, ref lcolor, ref lthick);
						group.AddObject(new ObjectRectangle(null, rect, null, new ObjectGeneral("Rect1"), lcolor, new BrushSolid(colorFill), 0, lthick, new ObjectArgsRectangle()));

						break;
					case 0x626:	// ESCAPE ??? (뭔지모르겠다)
						break;
					case 0x6FF:	// META_CREATEREGION  42byte의 데이타가 있다.
						AddToArray(arrayObject, "META_CREATEREGION");	// 파레트도 리소스 할당
						break;
					case 0x817:	// ARC
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						break;
					case 0x81A:	// PIE
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						val = reader.ReadInt16();
						break;
					case 0x940:	// DIBBITBLT
					{
						val = (short)reader.ReadUInt32();	// dwRop, 
						val = reader.ReadInt16();	// YSrc,
						val = reader.ReadInt16();	// XSrc,
						int nDestHeight = reader.ReadInt16();	// nDestHeight,
						int nDestWidth = reader.ReadInt16();	// nDestWidth,
						int yDest = reader.ReadInt16();	// YDest,
						int xDest = reader.ReadInt16();	// XDest,

						DIBitmap dib = new DIBitmap();
						Bitmap bitmap = dib.Read(reader);

						if(bitmap != null) 
						{
							string temp_file = String.Format("{0}\\EMF_{1}.bmp", Application.UserAppDataPath, Path.GetFileNameWithoutExtension(filename));
							bitmap.Save(temp_file);

							string maked_file;
							ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, temp_file, out maked_file);
							
							ObjectArgsBitmap args = new ObjectArgsBitmap();
							args.sBitmapFile = maked_file;
							args.nOverlayMethod = 0;

							rect.left = xDest;
							rect.top =  yDest;
							rect.right = rect.left+nDestWidth-1;
							rect.bottom = rect.top+nDestHeight-1;

							obj = new ObjectBitmap(form.workThis.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Bitmap1"), args);
							group.AddObject(obj);
						}
						
						break;
					}

					case 0xA32:	// META_EXTTEXTOUT
					{
						y = CalcY(reader.ReadInt16());	// iUsage, 
						x = CalcX(reader.ReadInt16());	// iUsage, 
						count = reader.ReadInt16();	// iUsage, 
						val = reader.ReadInt16();	// iUsage, 
						val = reader.ReadInt16();	// iUsage, 
						val = reader.ReadInt16();	// iUsage, 
						val = reader.ReadInt16();	// iUsage, 
						val = reader.ReadInt16();	// iUsage, 

						byte[] s = new byte[count];
						for(int i = 0; i < count; i++) 
						{
							s[i] = reader.ReadByte();
						}

						string s2 = Tools.BytesToString(s);

						ObjectArgsSingleText args = new ObjectArgsSingleText();

						args.text = s2;
						args.textColor = colorText;

						rect.left = x;
						rect.top =  y;
						rect.right = rect.left+10;
						rect.bottom = rect.top+10;

						obj = new ObjectSingleText(form.workThis.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Text1"), (LOGFONT)Tools.CopyObject(fontStruct), args);
						group.AddObject(obj);

						break;
					}
						
					case 0xF43:	// STRETCHDIB
					{
						val = (short)reader.ReadUInt32();	// dwRop, 
						val = reader.ReadInt16();	// iUsage, 
						val = reader.ReadInt16();	// nSrcHeight,
						val = reader.ReadInt16();	// nSrcWidth,
						val = reader.ReadInt16();	// YSrc,
						val = reader.ReadInt16();	// XSrc,
						int nDestHeight = reader.ReadInt16();	// nDestHeight,
						int nDestWidth = reader.ReadInt16();	// nDestWidth,
						int yDest = reader.ReadInt16();	// YDest,
						int xDest = reader.ReadInt16();	// XDest,

						DIBitmap dib = new DIBitmap();
						Bitmap bitmap = dib.Read(reader);

						if(bitmap != null) 
						{
							string temp_file = String.Format("{0}\\EMF_{1}.bmp", Application.UserAppDataPath, Path.GetFileNameWithoutExtension(filename));
							bitmap.Save(temp_file);

							string maked_file;
							ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, temp_file, out maked_file);
							
							ObjectArgsBitmap args = new ObjectArgsBitmap();
							args.sBitmapFile = maked_file;
							args.nOverlayMethod = 0;

							rect.left = xDest;
							rect.top =  yDest;
							rect.right = rect.left+nDestWidth-1;
							rect.bottom = rect.top+nDestHeight-1;

							obj = new ObjectBitmap(form.workThis.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Bitmap1"), args);
							group.AddObject(obj);
						}

						
						break;
					}
					default:
						if(!bNewRecord) 
						{	// 한번만 디스프레이
							string msg;
							msg = String.Format("MetaFile New Record Type (0x{0:X04})", MetaRecordHeader.rdFunction);
							MessageBox.Show(msg, "New Record Type founded.");
							bNewRecord = true;
						}
						break;
				}

				reader.BaseStream.Position = old_position;
				reader.BaseStream.Seek(MetaRecordHeader.rdSize*2, SeekOrigin.Current);
			}

			if(group.GetObjectHap() == 0) 
			{
				return null;
			}

			group.MakeGroupRect(form);

			return group;
		}

		static ObjectGroup ProcessFromBitmap(FormEditGraphic form, BinaryReader reader, string filename)
		{
			nMapMode = 0;		// 전역변수이므로 초기화를 해야한다.

			RECT rect = new RECT();

            ObjectGroup group = new ObjectGroup(null, null, null, null, null);

			Bitmap bitmap = new Bitmap(reader.BaseStream);

			if(bitmap != null) 
			{
				string temp_file = String.Format("{0}\\EMF_{1}.bmp", Application.UserAppDataPath, Path.GetFileNameWithoutExtension(filename));
				bitmap.Save(temp_file);

				string maked_file;
				ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, temp_file, out maked_file);
				
				ObjectArgsBitmap args = new ObjectArgsBitmap();
				args.sBitmapFile = maked_file;
				args.nOverlayMethod = 0;

				rect.left = 0;
				rect.top =  0;
				rect.right = rect.left+bitmap.Width-1;
				rect.bottom = rect.top+bitmap.Height-1;

				object obj = new ObjectBitmap(form.workThis.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Bitmap1"), args);
				group.AddObject(obj);
			}

			if(group.GetObjectHap() == 0) 
			{
				return null;
			}

			group.MakeGroupRect(form);

			return group;
		}

		public ObjectGroup Import(FormEditGraphic form, string filename)
		{
			FileStream fs;

			if(!File.Exists(filename))	return null;
			fs = File.OpenRead(filename);
			if(fs == null) 	return null;

			BinaryReader reader = new BinaryReader(fs);

			ObjectGroup group;

			if(IsPlaceableMetaFile(reader)) 
			{
				PLACEABLE_METAFILE_HEADER pheader = new PLACEABLE_METAFILE_HEADER();

				pheader.Read(reader);
			}

			else if(IsBitmapFile(reader))	// 확장자만 WMF이고 실제로는 BMP파일 
			{
				group = ProcessFromBitmap(form, reader, filename);
				reader.Close();
				fs.Close();

				return group;
			}

			METAHEADER header = new METAHEADER();

			header.Read(reader);

			group = ProcessMetafileRecords(form, reader, filename);

			reader.Close();
			fs.Close();

			return group;
		}
	}
}


