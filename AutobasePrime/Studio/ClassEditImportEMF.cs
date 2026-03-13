using System;
using GraphicModule;
using System.IO; 
using System.Windows.Forms;
using NetTools.OldDefine;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using NetTools;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditImport.
	/// </summary>
	public class ClassEditImportEMF
	{
		public ClassEditImportEMF()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		class LOGBRUSH32	// 8byte
		{
			public uint	lbStyle;
			public uint	lbColor;
			public uint	lbHatch;

			public void Read(BinaryReader reader)
			{
				lbStyle = reader.ReadUInt32();
				lbColor = reader.ReadUInt32();
				lbHatch = reader.ReadUInt32();
			}
		}

		static Color MakeColor(int color)
		{
			int r, g, b;
			r = (int)((color >>  0) & 0xFF);
			g = (int)((color >>  8) & 0xFF);
			b = (int)((color >> 16) & 0xFF);
			return Color.FromArgb(r, g, b);
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

		[Serializable]
		class EMR
		{
			public uint iType;
			public uint nSize;

			public void Read(BinaryReader reader)
			{
				iType = reader.ReadUInt32();
				nSize = reader.ReadUInt32();
			}
		}

		class RECTL 
		{
			public int	left;
			public int	top;
			public int	right;
			public int	bottom;

			public void Read(BinaryReader reader)
			{
				left = reader.ReadInt32();
				top = reader.ReadInt32();
				right = reader.ReadInt32();
				bottom = reader.ReadInt32();
			}
		}

		class SIZEL
		{
			public int	cx;
			public int	cy;

			public void Read(BinaryReader reader)
			{
				cx = reader.ReadInt32();
				cy = reader.ReadInt32();
			}
		}

		struct POINTL
		{
			public int	x;
			public int	y;

			public void Read(BinaryReader reader)
			{
				x = reader.ReadInt32();
				y = reader.ReadInt32();
			}
		}

		class EMRSETMODE
		{
			public EMR	emr = new EMR();
			public uint	iMode;

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				iMode = reader.ReadUInt32();
			}
		}

		class EMROBJECT
		{
			public EMR	emr = new EMR();
			public uint	ihObject;

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				ihObject = reader.ReadUInt32();
			}
		}

		class EMRPOINTL
		{
			public EMR	emr = new EMR();
			public POINTL	ptlOrigin = new POINTL();

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				ptlOrigin.Read(reader);
			}	
		}

		class EMRRECTL
		{
			public EMR	emr = new EMR();
			public RECTL	rclBounds = new RECTL();

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				rclBounds.Read(reader);
			}	
		}

		class EMRCREATEBRUSHINDIRECT
		{
			public EMR        emr = new EMR();
			public uint      ihBrush;          // Brush handle index
			public LOGBRUSH32 lb = new LOGBRUSH32();               // The style must be BS_SOLID, BS_HOLLOW,
			// BS_NULL or BS_HATCHED.

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				ihBrush = reader.ReadUInt32();
				lb.Read(reader);
			}
		}

		class ENHMETAHEADER 
		{
			public uint   iType;              // Record type EMR_HEADER
			public uint   nSize;              // Record size in bytes.  This may be greater
			// than the sizeof(ENHMETAHEADER).
			public RECTL   rclBounds = new RECTL();          // Inclusive-inclusive bounds in device units
			public RECTL   rclFrame = new RECTL();           // Inclusive-inclusive Picture Frame of metafile in .01 mm units
			public uint   dSignature;         // Signature.  Must be ENHMETA_SIGNATURE.
			public uint   nVersion;           // Version number
			public uint   nBytes;             // Size of the metafile in bytes
			public uint   nRecords;           // Number of records in the metafile
			public ushort    nHandles;           // Number of handles in the handle table
			// Handle index zero is reserved.
			public ushort    sReserved;          // Reserved.  Must be zero.
			public uint   nDescription;       // Number of chars in the unicode description string
			// This is 0 if there is no description string
			public uint   offDescription;     // Offset to the metafile description record.
			// This is 0 if there is no description string
			public uint   nPalEntries;        // Number of entries in the metafile palette.
			public SIZEL   szlDevice = new SIZEL();          // Size of the reference device in pels
			public SIZEL   szlMillimeters= new SIZEL();     // Size of the reference device in millimeters
			//#if(WINVER >= 0x0400)
			public uint   cbPixelFormat;      // Size of PIXELFORMATDESCRIPTOR information
			// This is 0 if no pixel format is set
			public uint   offPixelFormat;     // Offset to PIXELFORMATDESCRIPTOR
			// This is 0 if no pixel format is set
			public uint   bOpenGL;            // TRUE if OpenGL commands are present in
			// the metafile, otherwise FALSE
			//#endif  WINVER >= 0x0400 
			//#if(WINVER >= 0x0500)
			public SIZEL   szlMicrometers = new SIZEL();     // Size of the reference device in micrometers
			//#endif // WINVER >= 0x0500 

			public void Read(BinaryReader reader)
			{
				iType = reader.ReadUInt32();              // Record type EMR_HEADER
				nSize = reader.ReadUInt32();              // Record size in bytes.  This may be greater
				// than the sizeof(ENHMETAHEADER).
				rclBounds.Read(reader);          // Inclusive-inclusive bounds in device units
				rclFrame.Read(reader);           // Inclusive-inclusive Picture Frame of metafile in .01 mm units
				dSignature = reader.ReadUInt32();         // Signature.  Must be ENHMETA_SIGNATURE.
				nVersion = reader.ReadUInt32();           // Version number
				nBytes = reader.ReadUInt32();             // Size of the metafile in bytes
				nRecords = reader.ReadUInt32();           // Number of records in the metafile
				nHandles = reader.ReadUInt16();           // Number of handles in the handle table
				// Handle index zero is reserved.
				sReserved = reader.ReadUInt16();          // Reserved.  Must be zero.
				nDescription = reader.ReadUInt32();       // Number of chars in the unicode description string
				// This is 0 if there is no description string
				offDescription = reader.ReadUInt32();     // Offset to the metafile description record.
				// This is 0 if there is no description string
				nPalEntries = reader.ReadUInt32();        // Number of entries in the metafile palette.
				szlDevice.Read(reader);          // Size of the reference device in pels
				szlMillimeters.Read(reader);     // Size of the reference device in millimeters
				//#if(WINVER >= 0x0400)
				cbPixelFormat = reader.ReadUInt32();      // Size of PIXELFORMATDESCRIPTOR information
				// This is 0 if no pixel format is set
				offPixelFormat = reader.ReadUInt32();     // Offset to PIXELFORMATDESCRIPTOR
				// This is 0 if no pixel format is set
				bOpenGL = reader.ReadUInt32();            // TRUE if OpenGL commands are present in
				// the metafile, otherwise FALSE
				//#endif  WINVER >= 0x0400 
				//#if(WINVER >= 0x0500)
				szlMicrometers.Read(reader);     // Size of the reference device in micrometers
				//#endif // WINVER >= 0x0500 
			}
		}

		class EMREXTCREATEPEN
		{
			public EMR     emr = new EMR();
			public uint   ihPen;              // Pen handle index
			public uint   offBmi;             // Offset to the BITMAPINFO structure if any
			public uint   cbBmi;              // Size of the BITMAPINFO structure if any
			// The bitmap info is followed by the bitmap
			// bits to form a packed DIB.
			public uint   offBits;            // Offset to the brush bitmap bits if any
			public uint   cbBits;             // Size of the brush bitmap bits if any
			public EXTLOGPEN elp = new EXTLOGPEN();              // The extended pen with the style array.

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				ihPen = reader.ReadUInt32();
				offBmi = reader.ReadUInt32();
				cbBmi = reader.ReadUInt32();
				offBits = reader.ReadUInt32();
				cbBits = reader.ReadUInt32();
				elp.Read(reader);
			}
		}

		class EXTLOGPEN 
		{
			public uint    elpPenStyle;
			public uint    elpWidth;
			public uint    elpBrushStyle;
			public uint    elpColor;
			public uint    elpHatch;     //Sundown: elpHatch could take a HANDLE
			public uint    elpNumEntries;
			public uint[]    elpStyleEntry = new uint[1];

			public void Read(BinaryReader reader)
			{
				elpPenStyle = reader.ReadUInt32();
				elpWidth = reader.ReadUInt32();
				elpBrushStyle = reader.ReadUInt32();
				elpColor = reader.ReadUInt32();
				elpHatch = reader.ReadUInt32();
				elpNumEntries = reader.ReadUInt32();
				elpStyleEntry[0] = reader.ReadUInt32();
			}
		}

		class EMRSETMITERLIMIT
		{
			public EMR     emr = new EMR();
			public float   eMiterLimit;

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				eMiterLimit = reader.ReadSingle();
			}
		}

		struct POINTS 
		{
			public short x;
			public short y;

			public void Read(BinaryReader reader)
			{
				x = reader.ReadInt16();
				y = reader.ReadInt16();
			}
		}

		struct EMRPOLY16
		{
			public EMR     emr;
			public RECTL   rclBounds;          // Inclusive-inclusive bounds in device units
			public uint   cpts;               // Total number of points in all polys
			public POINTS[]  apts;            // Array of points

			public void Read(BinaryReader reader)
			{
				emr = new EMR();
				rclBounds = new RECTL();

				emr.Read(reader);
				rclBounds.Read(reader);
				cpts = reader.ReadUInt32();
				apts = new POINTS[cpts];
				for(int i = 0; i < cpts; i++) 
				{
					apts[i].Read(reader);
				}
			}
		}

		struct EMRPOLY32
		{
			public EMR     emr;
			public RECTL   rclBounds;          // Inclusive-inclusive bounds in device units
			public uint   cpts;               // Total number of points in all polys
			public POINTL[]  apts;            // Array of points

			public void Read(BinaryReader reader)
			{
				emr = new EMR();
				rclBounds = new RECTL();

				emr.Read(reader);
				rclBounds.Read(reader);
				cpts = reader.ReadUInt32();
				apts = new POINTL[cpts];
				for(int i = 0; i < cpts; i++) 
				{
					apts[i].Read(reader);
				}
			}
		}

		struct EMRPOLYPOLYGON16
		{
			public EMR     emr;
			public RECTL   rclBounds;          // Inclusive-inclusive bounds in device units
			public uint   nPolys;             // Number of polys
			public uint   cpts;               // Total number of points in all polys
			public uint[]   aPolyCounts;     // Array of point counts for each poly
			public POINTS[]  apts;            // Array of points

			public void Read(BinaryReader reader)
			{
				emr = new EMR();
				rclBounds = new RECTL();

				emr.Read(reader);
				rclBounds.Read(reader);
				nPolys = reader.ReadUInt32();
				cpts = reader.ReadUInt32();

				aPolyCounts = new uint[nPolys];
				for(int i = 0; i < nPolys; i++) 
				{
					aPolyCounts[i] = reader.ReadUInt32();
				}
				apts = new POINTS[cpts];
				for(int i = 0; i < cpts; i++) 
				{
					apts[i].Read(reader);
				}
			}
		}

		void AddPolyPolygon16(Form form, CurrentDC dc, ObjectGroup group, EMRPOLYPOLYGON16 d, bool fill)
		{
			ArrayList blockCurve = new ArrayList();
			CURVE_STRUCT curve = new CURVE_STRUCT();
			int pos = 0;
			Color lcolor;
			int lthick;

			for(int i = 0; i < d.nPolys; i++) 
			{
				for(int j = 0; j < d.aPolyCounts[i]; j++, pos++) 
				{
					curve = new CURVE_STRUCT();
					curve.x[0] = d.apts[pos].x;
					curve.y[0] = d.apts[pos].y;
					//curve.x[0] = CalcX(reader.ReadInt16());
					//curve.y[0] = CalcY(reader.ReadInt16());
					if(j == 0)
						curve.type = EnumCurveType.START;
					else
						curve.type = EnumCurveType.LINE;

					if(fill && j == d.aPolyCounts[i]-1)	// 채움형(polygon)이고 마지막 점
					{
						curve.type |= EnumCurveType.CLOSE;
					}
				
					blockCurve.Add(curve);
				}
			}
			lcolor = dc.colorLine;
			lthick = dc.nLineThick;
			FitLineStyle(dc.colorFill, ref lcolor, ref lthick);

			group.AddObject(new ObjectCurve(null, form, null, null, new ObjectGeneral("Curve1"), lcolor, new BrushSolid(dc.colorFill), 1, lthick, blockCurve));
		}

        void AddPolygon16(Form form, CurrentDC dc, ObjectGroup group, EMRPOLY16 d, bool fill)
        {
            ArrayList blockCurve = new ArrayList();
            CURVE_STRUCT curve = new CURVE_STRUCT();
            int pos = 0;
            Color lcolor;
            int lthick;


            for (int j = 0; j < d.cpts; j++, pos++)
                {
                    curve = new CURVE_STRUCT();
                    curve.x[0] = d.apts[pos].x;
                    curve.y[0] = d.apts[pos].y;
                    //curve.x[0] = CalcX(reader.ReadInt16());
                    //curve.y[0] = CalcY(reader.ReadInt16());
                    if (j == 0)
                        curve.type = EnumCurveType.START;
                    else
                        curve.type = EnumCurveType.LINE;

                    if (fill && j == d.cpts - 1)	// 채움형(polygon)이고 마지막 점
                    {
                        curve.type |= EnumCurveType.CLOSE;
                    }

                    blockCurve.Add(curve);
                }

            lcolor = dc.colorLine;
            lthick = dc.nLineThick;
            FitLineStyle(dc.colorFill, ref lcolor, ref lthick);

            group.AddObject(new ObjectCurve(null, form, null, null, new ObjectGeneral("Curve1"), lcolor, new BrushSolid(dc.colorFill), 1, lthick, blockCurve));
        }

		void AddPolyBezierTo16(Form form, ObjectGroup group, EMRPOLY16 d, ArrayList blockCurve)
		{
			CURVE_STRUCT curve = new CURVE_STRUCT();

			for(int i = 0; i < d.cpts; i++) 
			{
				if((i%3) == 0) 
				{
					curve = new CURVE_STRUCT();	
					curve.type = EnumCurveType.BEZIER;
				}

				curve.x[i%3] = d.apts[i].x;
				curve.y[i%3] = d.apts[i].y;
				
				if((i%3) == 2) 
				{
					blockCurve.Add(curve);
				}
			}
		}

        void AddPolyLineTo16(Form form, ObjectGroup group, EMRPOLY16 d, ArrayList blockCurve)
        {
            CURVE_STRUCT curve = new CURVE_STRUCT();

            for (int i = 0; i < d.cpts; i++)
            {

                curve = new CURVE_STRUCT();
                curve.type = EnumCurveType.LINE;


                curve.x[0] = d.apts[i].x;
                curve.y[0] = d.apts[i].y;
                blockCurve.Add(curve);
            }
        }

		void AddPolyLineTo32(Form form, ObjectGroup group, EMRPOLY32 d, ArrayList blockCurve)
		{
			CURVE_STRUCT curve = new CURVE_STRUCT();

			for(int i = 0; i < d.cpts; i++) 
			{
				
				curve = new CURVE_STRUCT();	
				curve.type = EnumCurveType.LINE;
			

				curve.x[0] = d.apts[i].x;
				curve.y[0] = d.apts[i].y;
				blockCurve.Add(curve);
			}
		}

		void SelectObject(CurrentDC dc, EMROBJECT emrobject)
		{
			object obj;
			string obj_name;
			for(int i = 0; i < arrayObject.Count; i++) 
			{
				obj = arrayObject[i];

				obj_name = obj.ToString();

				if(obj_name.IndexOf("EMREXTCREATEPEN") != -1) 
				{
					EMREXTCREATEPEN d = (EMREXTCREATEPEN)obj;
					if(emrobject.ihObject == d.ihPen) 
					{
						if(d.elp.elpPenStyle == 5) // PS_NULL
						{
							dc.colorLine = Color.FromArgb(0);
						}
						else 
						{
							// A가0이므로 그냥대입하면 안된다.
							dc.colorLine = MakeColor((int)d.elp.elpColor);
						}

						if(d.elp.elpWidth == 0)
							dc.nLineThick = 1;
						else
							dc.nLineThick = (int)d.elp.elpWidth;	// 	
					}


				}
				else if(obj_name.IndexOf("EMRCREATEPEN") != -1) 
				{
					EMRCREATEPEN d = (EMRCREATEPEN)obj;
					if(emrobject.ihObject == d.ihPen) 
					{
						if(d.lopn.lopnStyle == 5) // PS_NULL
						{
							dc.colorLine = Color.FromArgb(0);
						}
						else 
						{
							// A가0이므로 그냥대입하면 안된다.
							dc.colorLine = MakeColor((int)d.lopn.lopnColor);
						}

						if(d.lopn.lopnWidth.x == 0)
							dc.nLineThick = 1;
						else
							dc.nLineThick = (int)d.lopn.lopnWidth.x;	// 	
					}


				}
				else if(obj_name.IndexOf("EMRCREATEBRUSHINDIRECT") != -1) 
				{
					EMRCREATEBRUSHINDIRECT d = (EMRCREATEBRUSHINDIRECT)obj;	
					if(emrobject.ihObject == d.ihBrush) 
					{
						if(d.lb.lbStyle == 1) // BS_NULL
						{
							dc.colorFill = Color.FromArgb(0);
						}
						else 
						{
							// A가0이므로 그냥대입하면 안된다.
							dc.colorFill = MakeColor((int)d.lb.lbColor);		
						}
					}
				}
			}
		}

		void DeleteObject(EMROBJECT emrobject)
		{
			object obj;
			string obj_name;
			for(int i = 0; i < arrayObject.Count; i++) 
			{
				obj = arrayObject[i];

				obj_name = obj.ToString();

				if(obj_name.IndexOf("EMREXTCREATEPEN") != -1) 
				{
					EMREXTCREATEPEN d = (EMREXTCREATEPEN)obj;
					if(emrobject.ihObject == d.ihPen) 
					{
						arrayObject.RemoveAt(i);
						return;
					}
				}
				else if(obj_name.IndexOf("EMRCREATEPEN") != -1) 
				{
					EMRCREATEPEN d = (EMRCREATEPEN)obj;
					if(emrobject.ihObject == d.ihPen) 
					{
						arrayObject.RemoveAt(i);
						return;
					}
				}
				else if(obj_name.IndexOf("EMRCREATEBRUSHINDIRECT") != -1) 
				{
					EMRCREATEBRUSHINDIRECT d = (EMRCREATEBRUSHINDIRECT)obj;	
					if(emrobject.ihObject == d.ihBrush) 
					{
						arrayObject.RemoveAt(i);
						return;
					}
				}
			}
		}

		class EMREOF
		{
			public EMR     emr = new EMR();
			public uint   nPalEntries;        // Number of palette entries
			public uint   offPalEntries;      // Offset to the palette entries
			public uint   nSizeLast;          // Same as nSize and must be the last DWORD
			// of the record.  The palette entries,
			// if exist, precede this field.
			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				nPalEntries = reader.ReadUInt32();
				offPalEntries = reader.ReadUInt32();
				nSizeLast = reader.ReadUInt32();
			}
		}

		ArrayList arrayObject = new ArrayList();
		

		[Serializable]
		class XFORM
		{
			public float   eM11;
			public float   eM12;
			public float   eM21;
			public float   eM22;
			public float   eDx;
			public float   eDy;

			public void Read(BinaryReader reader)
			{
				eM11 = reader.ReadSingle();
				eM12 = reader.ReadSingle();
				eM21 = reader.ReadSingle();
				eM22 = reader.ReadSingle();
				eDx = reader.ReadSingle();
				eDy = reader.ReadSingle();
			}
		}

		[Serializable]
		class EMRMODIFYWORLDTRANSFORM
		{
			public EMR     emr = new EMR();
			public XFORM   xform = new XFORM();
			public uint   iMode;

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				xform.Read(reader);
				iMode = reader.ReadUInt32();
			}
		}

		class EMRSETWORLDTRANSFORM
		{
			public EMR     emr = new EMR();
			public XFORM   xform = new XFORM();

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				xform.Read(reader);
			}
		}

		class EMRRESTOREDC
		{
			public EMR     emr = new EMR();
			public int	iRelative;          // Specifies a relative instance

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				iRelative = reader.ReadInt32();
			}
		}

		class EMRCOLOR
		{
			public EMR     emr = new EMR();
			public uint	   crColor;

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				crColor = reader.ReadUInt32();
			}
		}

		class LOGPEN
		{
			public uint    lopnStyle;
			public POINTL   lopnWidth = new POINTL();
			public uint    lopnColor;

			public void Read(BinaryReader reader)
			{
				lopnStyle = reader.ReadUInt32();
				lopnWidth.Read(reader);
				lopnColor = reader.ReadUInt32();
			}
		}

		class EMRCREATEPEN
		{
			public EMR     emr = new EMR();
			public uint	   ihPen;
			public LOGPEN  lopn = new LOGPEN();

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				ihPen = reader.ReadUInt32();
				lopn.Read(reader);
			}
		}

		class EMRSTRETCHDIBITS
		{
			public EMR     emr = new EMR();
			public RECTL   rclBounds= new RECTL();          // Inclusive-inclusive bounds in device units
			public int    xDest;
			public int    yDest;
			public int    xSrc;
			public int    ySrc;
			public int    cxSrc;
			public int    cySrc;
			public uint   offBmiSrc;          // Offset to the source BITMAPINFO structure
			public uint   cbBmiSrc;           // Size of the source BITMAPINFO structure
			public uint   offBitsSrc;         // Offset to the source bitmap bits
			public uint   cbBitsSrc;          // Size of the source bitmap bits
			public uint   iUsageSrc;          // Source bitmap info color table usage
			public uint   dwRop;
			public int    cxDest;
			public int    cyDest;

			public void Read(BinaryReader reader)
			{
				emr.Read(reader);
				rclBounds.Read(reader);
				xDest = reader.ReadInt32();
				yDest = reader.ReadInt32();
				xSrc = reader.ReadInt32();
				ySrc = reader.ReadInt32();
				cxSrc = reader.ReadInt32();
				cySrc = reader.ReadInt32();
				offBmiSrc = reader.ReadUInt32();
				cbBmiSrc = reader.ReadUInt32();
				offBitsSrc = reader.ReadUInt32();
				cbBitsSrc = reader.ReadUInt32();
				iUsageSrc = reader.ReadUInt32();
				dwRop = reader.ReadUInt32();
				cxDest = reader.ReadInt32();
				cyDest = reader.ReadInt32();

				DIBitmap dib = new DIBitmap();
				bitmap = dib.Read(reader);
			}

			public Bitmap bitmap = null;
		}

        /*
        class EMRGDICOMMENT
        {
            //public uint lopnStyle;
            //public POINTL lopnWidth = new POINTL();
            //public uint lopnColor;

            public void Read(BinaryReader reader)
            {
                //lopnStyle = reader.ReadUInt32();
                //lopnWidth.Read(reader);
                //lopnColor = reader.ReadUInt32();
            }
        }*/

		[Serializable]
		class CurrentDC 
		{
			public EMRMODIFYWORLDTRANSFORM xform = null;
			public Color colorLine = Color.Black;
			public Color colorFill = Color.White;
			public Color colorText = Color.Black;
			public Color colorBack = Color.White;
			public int nLineThick = 1;
            public uint icmMode = 1;
		}

		int ConvertX(CurrentDC dc, int x)
		{
			if(dc.xform == null)	return x;

			x = (int)(x*dc.xform.xform.eM11+dc.xform.xform.eDx);

			return x;
		}

		int ConvertY(CurrentDC dc, int y)
		{
			if(dc.xform == null)	return y;

			y = (int)(y*dc.xform.xform.eM22+dc.xform.xform.eDy);

			return y;
		}

		int ConvertWidth(CurrentDC dc, int w)
		{
			if(dc.xform == null)	return w;

			w = (int)(w*dc.xform.xform.eM11);

			return w;
		}

		int ConvertHeight(CurrentDC dc, int h)
		{
			if(dc.xform == null)	return h;

			h = (int)(h*dc.xform.xform.eM22);

			return h;
		}

		public ObjectGroup Import(FormEditGraphic form, string filename, out double scalex, out double scaley)
		{
			FileStream fs;
            scalex = 1;
            scaley = 1;

			if(!File.Exists(filename))	return null;
			fs = File.OpenRead(filename);
			if(fs == null) 	return null;

			BinaryReader reader = new BinaryReader(fs);

			EMR MetaRecord = new EMR();
            ObjectGroup group = new ObjectGroup(null, null, null, null, null);
			bool bNewRecord = false;
			long old;
			ENHMETAHEADER header;
			EMRSETMODE mode = new EMRSETMODE();
			EMRPOINTL point = new EMRPOINTL();

            EMRPOINTL pViewportOrg = new EMRPOINTL();
            EMRPOINTL pViewportExt = new EMRPOINTL();
            EMRPOINTL pWindowOrg = new EMRPOINTL();
            EMRPOINTL pWindowExt = new EMRPOINTL();

			EMROBJECT emrobject = new EMROBJECT();
			bool bBeginPath = false;
			ArrayList arrayPath = new ArrayList();
			CURVE_STRUCT curve = new CURVE_STRUCT();
			EMRPOLYPOLYGON16 emrpolypolygon16 = new EMRPOLYPOLYGON16();
			Color lcolor;
			int lthick;
			EMRRECTL rectl;
			EMRCOLOR emrcolor;
			ArrayList arrayDC = new ArrayList();
			CurrentDC dc = new CurrentDC();
            //EMRGDICOMMENT gdicomment = new EMRGDICOMMENT();
				
			while(fs.Position < fs.Length) 
			{
				old = fs.Position;
				MetaRecord.Read(reader);
				fs.Position = old;
				switch(MetaRecord.iType) 
				{
					case 1:		// EMR_HEADER
						header = new ENHMETAHEADER();
						header.Read(reader);
						break;
					case 6:		// EMR_POLYLINETO
						EMRPOLY32 emrpoly32 = new EMRPOLY32();
						emrpoly32.Read(reader);
						AddPolyLineTo32(form, group, emrpoly32, arrayPath);
						break;
					case 9:	// EMR_SETWINDOWEXTEX
						pWindowExt.Read(reader);
						break;
					case 10:	// EMR_SETWINDOWORGEX
                        pWindowOrg.Read(reader);
						break;
					case 11:	// EMR_SETVIEWPORTEXTEX
                        pViewportExt.Read(reader);
						break;
					case 12:	// EMR_SETVIEWPORTORGEX
                        pViewportOrg.Read(reader);
						break;
					case 14:	// EMR_EOF
						EMREOF emreof = new EMREOF();
						emreof.Read(reader);
						break;
					case 17:	// EMR_SETMAPMODE
						mode.Read(reader);
						break;
					case 18:	// EMR_SETBKMODE
						mode.Read(reader);
						break;
					case 19:	// EMR_SETPOLYFILLMODE
						mode.Read(reader);
						break;
					case 22:	// EMR_SETTEXTALIGN
						mode.Read(reader);
						break;
					case 24:	// EMR_SETTEXTCOLOR
						emrcolor = new EMRCOLOR();
						emrcolor.Read(reader);
						dc.colorText = MakeColor((int)emrcolor.crColor);
						break;
					case 25:	// EMR_SETBKCOLOR
						emrcolor = new EMRCOLOR();
						emrcolor.Read(reader);
						dc.colorBack = MakeColor((int)emrcolor.crColor);
						break;
					case 27:	// EMR_MOVETOEX
						point.Read(reader);
						if(bBeginPath) 
						{
							curve = new CURVE_STRUCT();
							curve.x[0] = point.ptlOrigin.x;
							curve.y[0] = point.ptlOrigin.y;
							curve.type = EnumCurveType.START;
			
							arrayPath.Add(curve);
						}
						break;
                    case 30: // EMR_INTERSECTCLIPRECT
                        rectl = new EMRRECTL();
                        rectl.Read(reader);
                        break;
					case 33:	// EMR_SAVEDC
						arrayDC.Add(Tools.CopyObject(dc));
						break;
					case 34:	// EMR_RESTOREDC
						EMRRESTOREDC emrrestoredc = new EMRRESTOREDC();
						emrrestoredc.Read(reader);

						if(arrayDC.Count > 0) 
						{
							dc = (CurrentDC)arrayDC[arrayDC.Count-1];
							arrayDC.RemoveAt(arrayDC.Count-1);
						}
						break;
					case 35:	// EMR_SETWORLDTRANSFORM
						EMRSETWORLDTRANSFORM emrsetworldtransform = new EMRSETWORLDTRANSFORM();
						emrsetworldtransform.Read(reader);
						break;
					case 36:	// EMR_MODIFYWORLDTRANSFORM
						dc.xform = new EMRMODIFYWORLDTRANSFORM();
						dc.xform.Read(reader);
						break;
					case 37:	// EMR_SELECTOBJECT
						emrobject.Read(reader);
						SelectObject(dc, emrobject);
						break;
					case 38:	// EMR_CREATEPEN
						EMRCREATEPEN emrcreatepen = new EMRCREATEPEN();
						emrcreatepen.Read(reader);
						arrayObject.Add(emrcreatepen);
						break;
                    case 39:	// EMR_CREATEBRUSHINDIRECT
                        EMRCREATEBRUSHINDIRECT brush = new EMRCREATEBRUSHINDIRECT();
                        brush.Read(reader);
                        arrayObject.Add(brush);
                        break;
					case 40:	// EMR_DELETEOBJECT
						emrobject.Read(reader);
						DeleteObject(emrobject);
						break;

                    case 54:	// EMR_LINETO
                        {
                            point.Read(reader);
                            curve = new CURVE_STRUCT();
                            curve.type = EnumCurveType.LINE;

                            curve.x[0] = point.ptlOrigin.x;
                            curve.y[0] = point.ptlOrigin.y;
                            arrayPath.Add(curve);
                        }
                        break;

					case 58:	// EMR_SETMITERLIMIT
						EMRSETMITERLIMIT emrsetmiterlimit = new EMRSETMITERLIMIT();
						emrsetmiterlimit.Read(reader);
						break;

					case 59:	// EMR_BEGINPATH
						bBeginPath = true;
						arrayPath = new ArrayList();	// 초기화
						break;
					case 60:	// EMR_ENDPATH
						bBeginPath = false;
						lcolor = dc.colorLine;
						lthick = dc.nLineThick;
						FitLineStyle(dc.colorFill, ref lcolor, ref lthick);
						group.AddObject(new ObjectCurve(null, form, null, null, new ObjectGeneral("Curve1"), lcolor, new BrushSolid(dc.colorFill), 1, lthick, arrayPath));
						break;
					case 61:	// EMR_CLOSEFIGURE
						if(arrayPath.Count > 0) 
						curve = (CURVE_STRUCT)arrayPath[arrayPath.Count-1];
						curve.type |= EnumCurveType.CLOSE;
						break;
					case 62:	// EMR_FILLPATH
						rectl = new EMRRECTL();
						rectl.Read(reader);
						break;
					case 63:	// EMR_STROKEANDFILLPATH
						rectl = new EMRRECTL();
						rectl.Read(reader);
						break;
					case 64:	// EMR_STROKEPATH
						rectl = new EMRRECTL();
						rectl.Read(reader);
						break;
					case 67:	// EMR_SELECTCLIPPATH
						mode.Read(reader);
						break;

                    case 70:	// EMR_GDICOMMENT
                        break;

                    case 75:    // EMR_EXTSELECTCLIPRGN
                        break;

                    case 76:	// EMR_BITBLT   그림이다 일단 빼놓았다.
                        break;

					case 81:	// EMR_STRETCHDIBITS
						EMRSTRETCHDIBITS emrstretchdibits = new EMRSTRETCHDIBITS();
						emrstretchdibits.Read(reader);
						if(emrstretchdibits.bitmap != null) 
						{
							string temp_file = String.Format("{0}\\EMF_{1}.bmp", Application.UserAppDataPath, Path.GetFileNameWithoutExtension(filename));
							emrstretchdibits.bitmap.Save(temp_file);

							string maked_file;
							ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, temp_file, out maked_file);
							
							ObjectArgsBitmap args = new ObjectArgsBitmap();
							args.sBitmapFile = maked_file;
							args.nOverlayMethod = 0;
							RECT rect = new RECT();

							rect.left = ConvertX(dc, emrstretchdibits.xDest);
							rect.top =  ConvertY(dc, emrstretchdibits.yDest);
							rect.right = rect.left+(ConvertWidth(dc, emrstretchdibits.cxDest)-1);
							rect.bottom = rect.top+(ConvertHeight(dc, emrstretchdibits.cyDest)-1);

							ObjectBitmap obj = new ObjectBitmap(form.workThis.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Bitmap1"), args);
							group.AddObject(obj);
						}
						break;

					case 82:	// EMR_EXTCREATEFONTINDIRECTW
						break;

					case 83:	// EMR_EXTTEXTOUTA
						break;

					case 84:	// EMR_EXTTEXTOUTW
						break;

                    case 86:    // EMR_POLYGON16
                        {
                            EMRPOLY16 emrpoly16 = new EMRPOLY16();
                            emrpoly16.Read(reader);
                            AddPolygon16(form, dc, group, emrpoly16, true);
                        }
                        break;

                    case 88:	// EMR_POLYBEZIERTO16
                        {
                            EMRPOLY16 emrpoly16 = new EMRPOLY16();
                            emrpoly16.Read(reader);
                            AddPolyBezierTo16(form, group, emrpoly16, arrayPath);
                        }
                        break;

                    case 89: //#define EMR_POLYLINETO16                89
                        {
                            EMRPOLY16 emrpoly16 = new EMRPOLY16();
                            emrpoly16.Read(reader);
                            AddPolyLineTo16(form, group, emrpoly16, arrayPath);
                        }
                        break;

					case 90:	// EMR_POLYPOLYLINE16
						emrpolypolygon16 = new EMRPOLYPOLYGON16();
						emrpolypolygon16.Read(reader);
						AddPolyPolygon16(form, dc, group, emrpolypolygon16, false);
						break;

					case 91:	// EMR_POLYPOLYGON16
						emrpolypolygon16 = new EMRPOLYPOLYGON16();
						emrpolypolygon16.Read(reader);
						AddPolyPolygon16(form, dc, group, emrpolypolygon16, true);
						break;

					case 95:	// EMR_EXTCREATEPEN
						EMREXTCREATEPEN pen = new EMREXTCREATEPEN();
						pen.Read(reader);
						arrayObject.Add(pen);
						break;
                        
                    case 98:	// EMR_SETICMMODE
                        mode.Read(reader);
                        dc.icmMode = mode.iMode;  // 1 = ICM_OFF, 2 = ICM_ON, 3 = ICM_QUERY, 4 = ICM_DONE_OUTSIDE
                        break;

					default:
						if(!bNewRecord) 
						{	// 한번만 디스프레이
							string msg;
							msg = String.Format("MetaFile New Record Type\n(iType={0}, nSize={1}, DataSize={2})", MetaRecord.iType, MetaRecord.nSize, MetaRecord.nSize-8);
							MessageBox.Show(msg, "New Record Type founded.");
							bNewRecord = true;
						}
						break;
				}

				fs.Position = old;
				fs.Seek(MetaRecord.nSize, SeekOrigin.Current);
			}

			reader.Close();
			fs.Close();

			group.MakeGroupRect(form);

            if (pWindowExt.ptlOrigin.x != 0 && pViewportExt.ptlOrigin.x != 0)
            {
                scalex = (double)pWindowExt.ptlOrigin.x / pViewportExt.ptlOrigin.x;
            }
            if (pWindowExt.ptlOrigin.y != 0 && pViewportExt.ptlOrigin.y != 0)
            {
                scaley = (double)pWindowExt.ptlOrigin.y / pViewportExt.ptlOrigin.y;
            }

            /*
            RECT r = new RECT();

            int width = 0, height = 0;
            group.GetGroupRealSize(ref width, ref height);
            //group.GetZone(ref r);

            r.left = r.left * pViewportExt.ptlOrigin.x / pWindowExt.ptlOrigin.x;
            r.top = r.top * pViewportExt.ptlOrigin.y / pWindowExt.ptlOrigin.y;
            r.right = r.right * pViewportExt.ptlOrigin.x / pWindowExt.ptlOrigin.x;
            r.bottom = r.bottom * pViewportExt.ptlOrigin.y / pWindowExt.ptlOrigin.y;

            group.UpdateZone(form, r.left, r.top, r.right, r.bottom);*/

			return group;
		}
	}
}

/*
 #define EMR_HEADER                      1
#define EMR_POLYBEZIER                  2
#define EMR_POLYGON                     3
#define EMR_POLYLINE                    4
#define EMR_POLYBEZIERTO                5
#define EMR_POLYLINETO                  6
#define EMR_POLYPOLYLINE                7
#define EMR_POLYPOLYGON                 8
#define EMR_SETWINDOWEXTEX              9
#define EMR_SETWINDOWORGEX              10
#define EMR_SETVIEWPORTEXTEX            11
#define EMR_SETVIEWPORTORGEX            12
#define EMR_SETBRUSHORGEX               13
#define EMR_EOF                         14
#define EMR_SETPIXELV                   15
#define EMR_SETMAPPERFLAGS              16
#define EMR_SETMAPMODE                  17
#define EMR_SETBKMODE                   18
#define EMR_SETPOLYFILLMODE             19
#define EMR_SETROP2                     20
#define EMR_SETSTRETCHBLTMODE           21
#define EMR_SETTEXTALIGN                22
#define EMR_SETCOLORADJUSTMENT          23
#define EMR_SETTEXTCOLOR                24
#define EMR_SETBKCOLOR                  25
#define EMR_OFFSETCLIPRGN               26
#define EMR_MOVETOEX                    27
#define EMR_SETMETARGN                  28
#define EMR_EXCLUDECLIPRECT             29
#define EMR_INTERSECTCLIPRECT           30
#define EMR_SCALEVIEWPORTEXTEX          31
#define EMR_SCALEWINDOWEXTEX            32
#define EMR_SAVEDC                      33
#define EMR_RESTOREDC                   34
#define EMR_SETWORLDTRANSFORM           35
#define EMR_MODIFYWORLDTRANSFORM        36
#define EMR_SELECTOBJECT                37
#define EMR_CREATEPEN                   38
#define EMR_CREATEBRUSHINDIRECT         39
#define EMR_DELETEOBJECT                40
#define EMR_ANGLEARC                    41
#define EMR_ELLIPSE                     42
#define EMR_RECTANGLE                   43
#define EMR_ROUNDRECT                   44
#define EMR_ARC                         45
#define EMR_CHORD                       46
#define EMR_PIE                         47
#define EMR_SELECTPALETTE               48
#define EMR_CREATEPALETTE               49
#define EMR_SETPALETTEENTRIES           50
#define EMR_RESIZEPALETTE               51
#define EMR_REALIZEPALETTE              52
#define EMR_EXTFLOODFILL                53
#define EMR_LINETO                      54
#define EMR_ARCTO                       55
#define EMR_POLYDRAW                    56
#define EMR_SETARCDIRECTION             57
#define EMR_SETMITERLIMIT               58
#define EMR_BEGINPATH                   59
#define EMR_ENDPATH                     60
#define EMR_CLOSEFIGURE                 61
#define EMR_FILLPATH                    62
#define EMR_STROKEANDFILLPATH           63
#define EMR_STROKEPATH                  64
#define EMR_FLATTENPATH                 65
#define EMR_WIDENPATH                   66
#define EMR_SELECTCLIPPATH              67
#define EMR_ABORTPATH                   68

#define EMR_GDICOMMENT                  70
#define EMR_FILLRGN                     71
#define EMR_FRAMERGN                    72
#define EMR_INVERTRGN                   73
#define EMR_PAINTRGN                    74
#define EMR_EXTSELECTCLIPRGN            75
#define EMR_BITBLT                      76
#define EMR_STRETCHBLT                  77
#define EMR_MASKBLT                     78
#define EMR_PLGBLT                      79
#define EMR_SETDIBITSTODEVICE           80
#define EMR_STRETCHDIBITS               81
#define EMR_EXTCREATEFONTINDIRECTW      82
#define EMR_EXTTEXTOUTA                 83
#define EMR_EXTTEXTOUTW                 84
#define EMR_POLYBEZIER16                85
#define EMR_POLYGON16                   86
#define EMR_POLYLINE16                  87
#define EMR_POLYBEZIERTO16              88
#define EMR_POLYLINETO16                89
#define EMR_POLYPOLYLINE16              90
#define EMR_POLYPOLYGON16               91
#define EMR_POLYDRAW16                  92
#define EMR_CREATEMONOBRUSH             93
#define EMR_CREATEDIBPATTERNBRUSHPT     94
#define EMR_EXTCREATEPEN                95
#define EMR_POLYTEXTOUTA                96
#define EMR_POLYTEXTOUTW                97

#if(WINVER >= 0x0400)
#define EMR_SETICMMODE                  98
#define EMR_CREATECOLORSPACE            99
#define EMR_SETCOLORSPACE              100
#define EMR_DELETECOLORSPACE           101
#define EMR_GLSRECORD                  102
#define EMR_GLSBOUNDEDRECORD           103
#define EMR_PIXELFORMAT                104
#endif // WINVER >= 0x0400 

#if(WINVER >= 0x0500)
#define EMR_RESERVED_105               105
#define EMR_RESERVED_106               106
#define EMR_RESERVED_107               107
#define EMR_RESERVED_108               108
#define EMR_RESERVED_109               109
#define EMR_RESERVED_110               110
#define EMR_COLORCORRECTPALETTE        111
#define EMR_SETICMPROFILEA             112
#define EMR_SETICMPROFILEW             113
#define EMR_ALPHABLEND                 114
#define EMR_SETLAYOUT                  115
#define EMR_TRANSPARENTBLT             116
#define EMR_RESERVED_117               117
#define EMR_GRADIENTFILL               118
#define EMR_RESERVED_119               119
#define EMR_RESERVED_120               120
#define EMR_COLORMATCHTOTARGETW        121
#define EMR_CREATECOLORSPACEW          122
#endif // WINVER >= 0x0500 

#define EMR_MIN                          1

#if (WINVER >= 0x0500)
#define EMR_MAX                        122
#elif (WINVER >= 0x0400)
#define EMR_MAX                        104
#else
#define EMR_MAX                         97
#endif 
*/