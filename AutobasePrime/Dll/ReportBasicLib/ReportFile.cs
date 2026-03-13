using System;
using System.IO;
using NetTools;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace ReportBasicLib
{
	/// <summary>
	/// Summary description for ReportFile.
	/// </summary>
	public class ReportFile
	{
		public ReportFile()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static REPORT_STRUCT ReportLoad(string filename, bool bBoxUse)
		{
			if(!File.Exists(filename))	return null;
			string ext = Path.GetExtension(filename);

			if(String.Compare(ext, ".rptx", true) == 0) 
			{
				TextReader reader = new StreamReader(filename);
				if(reader == null)	return null;
				REPORT_STRUCT report = ReportLoadFromRptx(filename, reader, bBoxUse);
				reader.Close();
				return report;
			}
			else {
				FileStream fs = File.OpenRead(filename);
				if(fs == null)	return null;
				BinaryReader reader = new BinaryReader(fs);
				REPORT_STRUCT report = ReportLoadFromRpt(filename, reader, bBoxUse);
				reader.Close();
				return report;
			}
		}

		class REPORT_FILE_HEADER
		{
			public byte[]  id = new byte[7];		// REPORT
			public float   version_major;
			public float   version_minor;
			public ushort  crc;			// head crc;
		}

		static void ReadHeadFootStruct(FileLoadCrcSum16 file, ref HEAD_FOOT_STRUCT head)
		{
			head.height = file.ReadInt32();
			head.count = file.ReadUInt32();
			file.ReadUInt32();	// Block *blockObject;
		}

		static void ReadBorderStruct(FileLoadCrcSum16 file, ref BORDER_STRUCT border)
		{
			border.color = file.ReadColor();
			border.type = file.ReadSByte();
			border.thick = file.ReadSByte();
		}

		static string ReadBytes(FileLoadCrcSum16 file, int size)
		{
			byte[] read = file.ReadBytes(size);
			return Tools.BytesToString(read);
		}

		static void ReadLogFont(FileLoadCrcSum16 file, ref string fontName, ref float fontSize, ref FontStyle fontStyle)
		{
			int imsi;
			imsi = file.ReadInt32();
			fontSize = (int)((-imsi*(Double)72.0/96.0)+0.5);
			imsi = file.ReadInt32();
			imsi = file.ReadInt32();
			imsi = file.ReadInt32();

			imsi = file.ReadInt32();
			if(imsi == 700)
				fontStyle |= FontStyle.Bold;

			imsi = file.ReadByte();
			if(imsi == 1)
				fontStyle |= FontStyle.Italic;

			imsi = file.ReadByte();
			if(imsi == 1)
				fontStyle |= FontStyle.Underline;

			imsi = file.ReadByte();
			if(imsi == 1)
				fontStyle |= FontStyle.Strikeout;

			imsi = file.ReadByte();
			imsi = file.ReadByte();
			imsi = file.ReadByte();
			imsi = file.ReadByte();
			imsi = file.ReadByte();

			fontName = ReadBytes(file, 32);
		}

		static REPORT_STRUCT ReportLoadFromRpt(string filename, BinaryReader file, bool bBoxUse)
		{
			REPORT_FILE_HEADER head = new REPORT_FILE_HEADER();
			string message;
			REPORT_STRUCT report;
			FileLoadCrcSum16 crc_check;
			ushort crc;

			//file header를 불러온다.

			crc_check = new FileLoadCrcSum16(file);
			head.id = crc_check.ReadBytes(7);
			head.version_major = crc_check.ReadSingle();
			head.version_minor = crc_check.ReadSingle();
			head.crc = file.ReadUInt16();
			
			report = new REPORT_STRUCT();

			crc_check = new FileLoadCrcSum16(file);

			report.margin_left = crc_check.ReadInt32();
			report.margin_top  = crc_check.ReadInt32();
			report.margin_right = crc_check.ReadInt32();
			report.margin_bottom = crc_check.ReadInt32();
			report.margin_header = crc_check.ReadInt32();
			report.margin_footer = crc_check.ReadInt32();
			report.bOrientation = crc_check.ReadSByte();
			report.wOpticRate = crc_check.ReadUInt16();
			report.cursor_table = crc_check.ReadInt32();
			report.cursor_x1 = crc_check.ReadInt32();
			report.cursor_y1 = crc_check.ReadInt32();
			report.cursor_x2 = crc_check.ReadInt32();
			report.cursor_y2 = crc_check.ReadInt32();
			int nTableCount = crc_check.ReadInt32();
			report.description = ReadBytes(crc_check, 80);
			report.lColorPaper = crc_check.ReadColor();
			crc_check.ReadUInt32();	// TABLE_STRUCT *tableBuf;
			ReadHeadFootStruct(crc_check, ref report.header);
			ReadHeadFootStruct(crc_check, ref report.footer);
			crc_check.ReadBytes(80);	// char extra[80];
			crc = file.ReadUInt16();	// WORD crc

			if(crc != crc_check.crc) 
			{
				message = String.Format("Report Struct crc mismatched\nnot report file or file destroyed.");
				OpenErrorMessage(filename, message, bBoxUse);
				return null;
			}

			report.tableBuf = null;
			report.header.blockObject = null;
			report.footer.blockObject = null;

			TABLE_STRUCT table;
			CELL_STRUCT cell;
			int i;

			report.tableBuf = new System.Collections.ArrayList();
			report.header.blockObject = new System.Collections.ArrayList();
			report.footer.blockObject = new System.Collections.ArrayList();

			for(i = 0; i < nTableCount; i++) 
			{
				table = new TABLE_STRUCT();

				crc_check = new FileLoadCrcSum16(file);

				table.no = crc_check.ReadInt32();
				table.cell_x = crc_check.ReadUInt16();
				table.cell_y = crc_check.ReadUInt16();
				table.gab_left = crc_check.ReadInt32();
				table.gab_top = crc_check.ReadInt32();
				crc_check.ReadUInt32();	// CELL_STRUCT	  *cellBuf;
				crc_check.ReadBytes(80);	// extra[80]
				crc = file.ReadUInt16();	// crc
				
				if(crc != crc_check.crc) 
				{
					message = String.Format("Table Struct crc mismatched\nnot report file or file destroyed.");
					OpenErrorMessage(filename, message, bBoxUse);
					return null;
				}

				for(int m = 0; m < table.cell_x*table.cell_y; m++) 
				{
					cell = new CELL_STRUCT();
					
					crc_check = new FileLoadCrcSum16(file);

					cell.x = crc_check.ReadUInt16();
					cell.y = crc_check.ReadUInt16();
					cell.tcolor = crc_check.ReadColor();
					cell.bcolor = crc_check.ReadColor();;
					for(int j = 0; j < 6; j++) 
					{
						ReadBorderStruct(crc_check, ref cell.border[j]);
					}
					cell.cAlignHorz = crc_check.ReadSByte();
					cell.cAlignVert = crc_check.ReadSByte();
					ReadLogFont(crc_check, ref cell.fontName, ref cell.fontSize, ref cell.fontStyle);
					cell.text = ReadBytes(crc_check, 80);
					cell.width = crc_check.ReadInt32();
					cell.height = crc_check.ReadInt32();
					cell.cGroup = crc_check.ReadSByte();
					cell.nGroupX = crc_check.ReadUInt16();
					cell.nGroupY = crc_check.ReadUInt16();
					cell.wOnRunY1 = crc_check.ReadUInt16();
					cell.wOnRunY2 = crc_check.ReadUInt16();
					cell.bCalced = crc_check.ReadSByte();
					cell.format.cType = crc_check.ReadSByte();
					cell.format.cUnderPoint = crc_check.ReadSByte();
					cell.format.bThousandComma = crc_check.ReadSByte();
					cell.format.cDateTime = crc_check.ReadSByte();
					cell.format.cTimeCount = crc_check.ReadSByte();
					cell.format.bWeekAdd = crc_check.ReadSByte();
					crc_check.ReadBytes(9);	// extra[9];

					crc_check.ReadBytes(33);// cell struct extra[33];

					crc = file.ReadUInt16();

					if(crc != crc_check.crc) 
					{
						message = String.Format("CELL{0} Struct crc mismatched\nnot report file or file destroyed.", m);
						OpenErrorMessage(filename, message, bBoxUse);
						return null;
					}
                                                            
					if((cell.x != (m%table.cell_x)) || (cell.y != (m/table.cell_x)) ) 
					{
						
						message = String.Format("Cell x, y information mismatched\nnot report file or file destroyed.");
						OpenErrorMessage(filename, message, bBoxUse);

						return null;
					}

					table.cellBuf.Add(cell);
				}
				report.tableBuf.Add(table);
			}

			if(!LoadHeadFoot(filename, file, report.header.blockObject, report.header.count, bBoxUse)) 
			{
				return null;
			}
			if(!LoadHeadFoot(filename, file, report.footer.blockObject, report.footer.count, bBoxUse)) 
			{
				return null;
			}

			if(report.header.height < 100)		report.header.height = 100;
			if(report.footer.height < 100)		report.footer.height = 100;

			return report;
		}

		static void OpenErrorMessage(string filename, string msg, bool bBoxUse)
		{
			//if(bBoxUse)
				MessageBox.Show(msg, filename);
			//else 
			//	MessageScreen(file->GetFileName(), msg);
		}

		static bool LoadHeadFoot(string filename, BinaryReader file, ArrayList block, uint count, bool bBoxUse)
		{
			int l;
			DRAW_ONE_OBJECT one;
			ushort crc;
			string message;
			FileLoadCrcSum16 crc_check;

			for(l = 0; l < count; l++) 
			{
				one = new DRAW_ONE_OBJECT();

				crc_check = new FileLoadCrcSum16(file);

				one.type = (EnumDrawType)file.ReadUInt16();
		
				if(one.type == EnumDrawType.LINE) 
				{
					one.p = new DRAW_OBJECT_LINE();
					DRAW_OBJECT_LINE obj = (DRAW_OBJECT_LINE)one.p;

					obj.x1 = crc_check.ReadInt32();
					obj.y1 = crc_check.ReadInt32();
					obj.x2 = crc_check.ReadInt32();
					obj.y2 = crc_check.ReadInt32();
					obj.thick = crc_check.ReadInt32();
					obj.style = crc_check.ReadInt32();
					obj.color = crc_check.ReadColor();
					crc_check.ReadBytes(16);	// extra[16];
					crc = file.ReadUInt16();	// crc

					if(crc != crc_check.crc) 
					{
						message = String.Format("DRAW_OBJECT_LINE Struct crc mismatched\nnot report file or file destroyed.");
						OpenErrorMessage(filename, message, bBoxUse);
						return false;
					}

					block.Add(one);
				}
				else if(one.type == EnumDrawType.TEXT) 
				{
					one.p = new DRAW_OBJECT_TEXT();
					DRAW_OBJECT_TEXT obj = (DRAW_OBJECT_TEXT)one.p;											

					obj.x1 = crc_check.ReadInt32();
					obj.y1 = crc_check.ReadInt32();
					obj.x2 = crc_check.ReadInt32();
					obj.y2 = crc_check.ReadInt32();
					obj.align = crc_check.ReadInt32();
					ReadLogFont(crc_check, ref obj.fontName, ref obj.fontSize, ref obj.fontStyle);
					obj.text = ReadBytes(crc_check, 256);
					obj.color = crc_check.ReadColor();
					crc_check.ReadBytes(16);	// extra[16];
					crc = file.ReadUInt16();	// crc

					if(crc != crc_check.crc) 
					{
						message = String.Format("DRAW_OBJECT_TEXT Struct crc mismatched\nnot report file or file destroyed.");
						OpenErrorMessage(filename, message, bBoxUse);
						return false;
					}

					block.Add(one);
				}
				else 
				{

				}
			}

			return true;
		}

		public static readonly float fVersionFileMajor = 8.3f;
		public static readonly float fVersionFileMinor = 8.3f;

		static void SaveHeaderFooter(CommaTextWriter writer, HEAD_FOOT_STRUCT hf, string field_name)
		{
			if(hf.blockObject.Count == 0)	return;

			DRAW_ONE_OBJECT one;

			writer.WriteLine("\t{0},Begin,", field_name);
			writer.WriteLine("\t\tHeight,{0},", hf.height);
			for(int i = 0; i < hf.blockObject.Count; i++) 
			{
				one = (DRAW_ONE_OBJECT)hf.blockObject[i];
				if(one.type == EnumDrawType.LINE) 
				{
					DRAW_OBJECT_LINE obj = (DRAW_OBJECT_LINE)one.p;
					writer.WriteLine("\t\tMember,Line,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},",
						obj.x1, obj.y1, obj.x2, obj.y2, obj.thick, obj.style,
						obj.color.A, obj.color.R, obj.color.G, obj.color.B);
				}
				else if(one.type == EnumDrawType.TEXT) 
				{
					DRAW_OBJECT_TEXT obj = (DRAW_OBJECT_TEXT)one.p;
					writer.WriteLine("\t\tMember,Text,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},",
						obj.x1, obj.y1, obj.x2, obj.y2, obj.align, 
						obj.fontName, obj.fontSize, (int)obj.fontStyle,
						obj.text,
						obj.color.A, obj.color.R, obj.color.G, obj.color.B);
				}
				else 
				{

				}
			}
			writer.WriteLine("\t{0},End,", field_name);
		}

		public static bool Save(REPORT_STRUCT report, string filename)
		{
			string dir = Path.GetDirectoryName(filename);
			if(!Directory.Exists(dir)) 
			{
				Directory.CreateDirectory(dir);
			}
            
			CommaTextWriter writer = new CommaTextWriter(filename);

			if(writer == null)	return false;

			writer.WriteLine("Report,Begin,");
			writer.WriteLine("\tVersionFileMajor,{0},", fVersionFileMajor);
			writer.WriteLine("\tVersionFileMinor,{0},", fVersionFileMinor);
			writer.WriteLine("\tVersionProgram,{0},", Application.ProductVersion);
			writer.WriteLine("\tMargin,{0},{1},{2},{3},{4},{5},", report.margin_left, report.margin_top, report.margin_right, report.margin_bottom, report.margin_header, report.margin_footer);
			writer.WriteLine("\tOrientation,{0},", report.bOrientation);
			writer.WriteLine("\tOpticRate,{0},", report.wOpticRate);
			writer.WriteLine("\tCursor,{0},{1},{2},{3},{4},", report.cursor_table, report.cursor_x1, report.cursor_y1, report.cursor_x2, report.cursor_y2);
			writer.WriteLine("\tDescription,{0}", report.description);
			writer.WriteLine("\tPaperColor,{0},{1},{2},{3},", report.lColorPaper.A, report.lColorPaper.R, report.lColorPaper.G, report.lColorPaper.B);

			TABLE_STRUCT table;
			CELL_STRUCT cell;
			for(int i = 0; i < report.tableBuf.Count; i++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[i];
				writer.WriteLine("\tTable,Begin,");
				writer.WriteLine("\t\tCellXY,{0},{1},", table.cell_x, table.cell_y);
				writer.WriteLine("\t\tGabXY,{0},{1},", table.gab_left, table.gab_top);
				for(int j = 0; j < table.cellBuf.Count; j++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[j];
					writer.WriteLine("\t\tCell,Begin,");
					writer.WriteLine("\t\t\tXY,{0},{1},", cell.x, cell.y);
					writer.WriteLine("\t\t\tTextColor,{0},{1},{2},{3},", cell.tcolor.A, cell.tcolor.R, cell.tcolor.G, cell.tcolor.B);
					writer.WriteLine("\t\t\tBackColor,{0},{1},{2},{3},", cell.bcolor.A, cell.bcolor.R, cell.bcolor.G, cell.bcolor.B);
					for(int k = 0; k < 6; k++) 
					{
						writer.WriteLine("\t\t\tBorder,{0},{1},{2},{3},{4},{5},{6},", 
							k, 
							cell.border[k].color.A, cell.border[k].color.R, cell.border[k].color.G, cell.border[k].color.B,
							cell.border[k].type, 
							cell.border[k].thick);
					}
					writer.WriteLine("\t\t\tAlign,{0},{1},", cell.cAlignHorz, cell.cAlignVert);
					writer.WriteLine("\t\t\tFont,{0},{1},{2},", cell.fontName, cell.fontSize, (int)cell.fontStyle);
					writer.WriteLine("\t\t\tText,{0}", cell.text);
					writer.WriteLine("\t\t\tSize,{0},{1},", cell.width, cell.height);
					writer.WriteLine("\t\t\tGroup,{0},{1},{2},", cell.cGroup, cell.nGroupX, cell.nGroupY);
                    writer.WriteLine("\t\t\tFormat,{0},{1},{2},{3},{4},{5},{6}", 
						cell.format.cType, 
						cell.format.cUnderPoint, 
						cell.format.bThousandComma, 
						cell.format.cDateTime,
						cell.format.cTimeCount,
						cell.format.bWeekAdd,
                        cell.format.sUserFormat);

					writer.WriteLine("\t\tCell,End,");
				}
				writer.WriteLine("\tTable,End,");
			}

			SaveHeaderFooter(writer, report.header, "Header");
			SaveHeaderFooter(writer, report.footer, "Footer");

			writer.WriteLine("Report,End,");

			writer.Close();
			
			return true;
		}

        public static bool SaveToCSV(REPORT_STRUCT report, string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            CommaTextWriter writer = new CommaTextWriter(filename, System.Text.Encoding.Default);

            if (writer == null) return false;

            /*
            writer.WriteLine("Report,Begin,");
            writer.WriteLine("\tVersionFileMajor,{0},", fVersionFileMajor);
            writer.WriteLine("\tVersionFileMinor,{0},", fVersionFileMinor);
            writer.WriteLine("\tVersionProgram,{0},", Application.ProductVersion);
            writer.WriteLine("\tMargin,{0},{1},{2},{3},{4},{5},", report.margin_left, report.margin_top, report.margin_right, report.margin_bottom, report.margin_header, report.margin_footer);
            writer.WriteLine("\tOrientation,{0},", report.bOrientation);
            writer.WriteLine("\tOpticRate,{0},", report.wOpticRate);
            writer.WriteLine("\tCursor,{0},{1},{2},{3},{4},", report.cursor_table, report.cursor_x1, report.cursor_y1, report.cursor_x2, report.cursor_y2);
            writer.WriteLine("\tDescription,{0}", report.description);
            writer.WriteLine("\tPaperColor,{0},{1},{2},{3},", report.lColorPaper.A, report.lColorPaper.R, report.lColorPaper.G, report.lColorPaper.B);
             */

            TABLE_STRUCT table;
            CELL_STRUCT cell;
            for (int i = 0; i < report.tableBuf.Count; i++)
            {
                table = (TABLE_STRUCT)report.tableBuf[i];
                //writer.WriteLine("\tTable,Begin,");
                //writer.WriteLine("\t\tCellXY,{0},{1},", table.cell_x, table.cell_y);
                //writer.WriteLine("\t\tGabXY,{0},{1},", table.gab_left, table.gab_top);

                for (int j = 0; j < table.cell_y; j++)
                {
                    for (int k = 0; k < table.cell_x; k++)
                    {
                        cell = (CELL_STRUCT)table.cellBuf[j*table.cell_x+k];
                        writer.Write("{0},", cell.text);
                        
                    }
                    writer.WriteLine();
                }

                writer.WriteLine();
            }

            writer.Close();

            return true;
        }

		static CELL_STRUCT LoadOneCell(string filename, TextReader reader, bool bBoxUse)
		{
			CELL_STRUCT cell = new CELL_STRUCT();
			string one_line;
			string command = "";
			CommaTextReader comma = new CommaTextReader();

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;

				comma.Set(one_line);
				comma.GetString(ref command);

				if(command == "XY") 
				{
					comma.GetInt(ref cell.x);
					comma.GetInt(ref cell.y);
				}
				else if(command == "TextColor") 
				{
					int r=0,g=0,b=0,a=0;
					comma.GetInt(ref a);
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					cell.tcolor = Color.FromArgb(a,r,g,b);
				}
				else if(command == "BackColor") 
				{
					int r=0,g=0,b=0,a=0;
					comma.GetInt(ref a);
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					cell.bcolor = Color.FromArgb(a,r,g,b);
				}
				else if(command == "Border") 
				{
					int pos = 0;

					comma.GetInt(ref pos);

					int r=0,g=0,b=0,a=0;
					comma.GetInt(ref a);
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					cell.border[pos].color = Color.FromArgb(a,r,g,b);

					comma.GetChar(ref cell.border[pos].type);
					comma.GetChar(ref cell.border[pos].thick);
				}
				else if(command == "Align") 
				{
					comma.GetChar(ref cell.cAlignHorz);
					comma.GetChar(ref cell.cAlignVert);
				}
				else if(command == "Font") 
				{
					comma.GetString(ref cell.fontName);
					comma.GetFloat(ref cell.fontSize);

					int style = 0;
					comma.GetInt(ref style);
					cell.fontStyle = (FontStyle)style;
				}
				else if(command == "Text") 
				{
					comma.GetString(ref cell.text);
				}
				else if(command == "Size") 
				{
					comma.GetInt(ref cell.width);
					comma.GetInt(ref cell.height);
				}
				else if(command == "Group") 
				{
					comma.GetChar(ref cell.cGroup);
					comma.GetInt(ref cell.nGroupX);
					comma.GetInt(ref cell.nGroupY);
				}
				else if(command == "Format") 
				{
					comma.GetChar(ref cell.format.cType);
					comma.GetChar(ref cell.format.cUnderPoint);
					comma.GetChar(ref cell.format.bThousandComma);
					comma.GetChar(ref cell.format.cDateTime);
					comma.GetChar(ref cell.format.cTimeCount);
					comma.GetChar(ref cell.format.bWeekAdd);
                    comma.GetString(ref cell.format.sUserFormat);
				}
				else if(command == "Cell") 
				{
					string end = "";
					comma.GetString(ref end);
					if(end == "End") 
					{
						return cell;
					}
					else 
					{
						string msg;
						msg = String.Format("Report file is destroyed. (not Cell,End)");
						OpenErrorMessage(filename, msg, bBoxUse); 
						return null;
					}
				}
				else 
				{
					string msg;
					msg = String.Format("Unknowned cell field. ({0})", command);
					OpenErrorMessage(filename, msg, bBoxUse);
				}
			}

			return cell;
		}

		static TABLE_STRUCT LoadOneTable(string filename, TextReader reader, bool bBoxUse, int table_no)
		{
			TABLE_STRUCT table = new TABLE_STRUCT();

			table.no = table_no;

			string one_line;
			string command = "";
			CommaTextReader comma = new CommaTextReader();
            
			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;

				comma.Set(one_line);
				comma.GetString(ref command);

				if(command == "CellXY") 
				{
					comma.GetInt(ref table.cell_x);
					comma.GetInt(ref table.cell_y);
				}
				else if(command == "GabXY") 
				{
					comma.GetInt(ref table.gab_left);
					comma.GetInt(ref table.gab_top);
				}
				else if(command == "Table") 
				{
					string end = "";
					comma.GetString(ref end);
					if(end == "End") 
					{
						return table;
					}
					else 
					{
						string msg;
						msg = String.Format("Report file is destroyed. (not Table,End)");
						OpenErrorMessage(filename, msg, bBoxUse); 
						return null;
					}
				}
				else if(command == "Cell") 
				{
					string begin = "";
					comma.GetString(ref begin);
					if(begin == "Begin") 
					{
						CELL_STRUCT cell = LoadOneCell(filename, reader, bBoxUse);
						if(cell == null)	return null;
						table.cellBuf.Add(cell);
					}
					else 
					{
						string msg;
						msg = String.Format("Report file is destroyed. (not Cell,Begin)");
						OpenErrorMessage(filename, msg, bBoxUse); 
						return null;
					}
				}
				else 
				{
					string msg;
					msg = String.Format("Unknowned table field. ({0})", command);
					OpenErrorMessage(filename, msg, bBoxUse);
				}

			}

			return table;
		}

		static HEAD_FOOT_STRUCT LoadHeaderFooter(string filename, TextReader reader, bool bBoxUse, string section)
		{
			HEAD_FOOT_STRUCT hf = new HEAD_FOOT_STRUCT();
			string one_line;
			string command = "";
			string name="";
			CommaTextReader comma = new CommaTextReader();

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;

				comma.Set(one_line);
				comma.GetString(ref command);

				if(command == "Height") 
				{
					comma.GetInt(ref hf.height);
				}
				else if(command == "Member") 
				{
					DRAW_ONE_OBJECT one = new DRAW_ONE_OBJECT();
					comma.GetString(ref name);
					if(name == "Line") 
					{
						one.type = EnumDrawType.LINE;
						DRAW_OBJECT_LINE obj = new DRAW_OBJECT_LINE();
						comma.GetInt(ref obj.x1);
						comma.GetInt(ref obj.y1);
						comma.GetInt(ref obj.x2);
						comma.GetInt(ref obj.y2);
						comma.GetInt(ref obj.thick);
						comma.GetInt(ref obj.style);
						int r=0,g=0,b=0,a=0;
						comma.GetInt(ref a);
						comma.GetInt(ref r);
						comma.GetInt(ref g);
						comma.GetInt(ref b);
						obj.color = Color.FromArgb(a,r,g,b);
						
						one.p = obj;

						hf.blockObject.Add(one);
					}
					else if(name == "Text") 
					{
						one.type = EnumDrawType.TEXT;
						DRAW_OBJECT_TEXT obj = new DRAW_OBJECT_TEXT();
						comma.GetInt(ref obj.x1);
						comma.GetInt(ref obj.y1);
						comma.GetInt(ref obj.x2);
						comma.GetInt(ref obj.y2);
						comma.GetInt(ref obj.align);
						comma.GetString(ref obj.fontName);
						comma.GetFloat(ref obj.fontSize);
						int style = 0;
						comma.GetInt(ref style);
						obj.fontStyle = (FontStyle)style;
						comma.GetString(ref obj.text);
						int r=0,g=0,b=0,a=0;
						comma.GetInt(ref a);
						comma.GetInt(ref r);
						comma.GetInt(ref g);
						comma.GetInt(ref b);
						obj.color = Color.FromArgb(a,r,g,b);
						
						one.p = obj;

						hf.blockObject.Add(one);
					}
					else 
					{
						string msg;
						msg = String.Format("Unknowned HEAD_FOOT Draw Object. ({0})", name);
						OpenErrorMessage(filename, msg, bBoxUse);
					}
				}
				else if(command == section) 
				{
					string end = "";
					comma.GetString(ref end);
					if(end == "End") 
					{
						return hf;
					}
					else 
					{
						string msg;
						msg = String.Format("Report file is destroyed. (not {0},End)", section);
						OpenErrorMessage(filename, msg, bBoxUse); 
						return null;
					}
				}
				else 
				{
					string msg;
					msg = String.Format("Unknowned HEAD_FOOT field. ({0})", command);
					OpenErrorMessage(filename, msg, bBoxUse);
				}

			}

			return hf;
		}

		static REPORT_STRUCT ReportLoadFromRptx(string filename, TextReader reader, bool bBoxUse)
		{
			REPORT_STRUCT report = ReportLib.MakeNewReport();	// new REPORT_STRUCT();
			CommaTextReader comma = new CommaTextReader();
			string one_line;
			string command = "";

			one_line = reader.ReadLine();
			if(one_line == null)	return null;
			if(one_line != "Report,Begin,") 
			{
				return null;
			}

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				if(one_line == "Report,End,")	break;
				comma.Set(one_line);
				comma.GetString(ref command);

				if(command == "VersionFileMajor") 
				{
					comma.GetFloat(ref report.fVersionFileMajor);
					if(report.fVersionFileMajor > fVersionFileMajor) 
					{
						string msg;
						if(Tools.IsLangKorean()) 
							msg = String.Format("리포터 Major 버전이 상위버전입니다.\n파일을 읽는 중 오류가 발생할 수 있습니다.\nFile Major Version = {0}\nCurrent Major Version={1}", report.fVersionFileMajor, fVersionFileMajor);
						else if(Tools.IsLangChinese()) 
							msg = String.Format("报表Major版本是上位版本。\n在打开文件时，会发生错误。\nFile Major Version = {0}\nCurrent Major Version={1}", report.fVersionFileMajor, fVersionFileMajor);
						else
							msg = String.Format("Report Major version is too high.\nFile Major Version = {0}\nCurrent Major Version={1}", report.fVersionFileMajor, fVersionFileMajor);

						MessageBox.Show(msg, "Version mismatched"); 
					}
				}
				else if(command == "VersionFileMinor") 
				{
					comma.GetFloat(ref report.fVersionFileMinor);
					if(report.fVersionFileMinor > fVersionFileMinor) 
					{
						string msg;
						if(Tools.IsLangKorean()) 
							msg = String.Format("리포터 Minor 버전이 상위 버전입니다.\n몇가지 특성을 읽지 못할 수 있습니다.\nFile Minor Version = {0}\nCurrent Minor Version={1}", report.fVersionFileMinor, fVersionFileMinor);
						else if(Tools.IsLangChinese()) 
							msg = String.Format("报表Minor版本是上位版本。\n几个属性会不能打开。\nFile Minor Version = {0}\nCurrent Minor Version={1}", report.fVersionFileMinor, fVersionFileMinor);
						else
							msg = String.Format("Report Minor version is too high.\nFile Minor Version = {0}\nCurrent Minor Version={1}", report.fVersionFileMinor, fVersionFileMinor);

						MessageBox.Show(msg, "High Version");
					}
				}
				else if(command == "VersionProgram") 
				{
					comma.GetString(ref report.sVersionProgram);
				}
				else if(command == "Margin") 
				{
					comma.GetInt(ref report.margin_left);
					comma.GetInt(ref report.margin_top);
					comma.GetInt(ref report.margin_right);
					comma.GetInt(ref report.margin_bottom);
					comma.GetInt(ref report.margin_header);
					comma.GetInt(ref report.margin_footer);
				}
				else if(command == "Orientation") 
				{
					comma.GetChar(ref report.bOrientation);
				}
				else if(command == "OpticRate") 
				{
					comma.GetInt(ref report.wOpticRate);
				}
				else if(command == "Cursor") 
				{
					comma.GetInt(ref report.cursor_table);
					comma.GetInt(ref report.cursor_x1);
					comma.GetInt(ref report.cursor_y1);
					comma.GetInt(ref report.cursor_x2);
					comma.GetInt(ref report.cursor_y2);
				}
				else if(command == "Description") 
				{
					comma.GetString(ref report.description);
				}
				else if(command == "PaperColor") 
				{
					int r=0,g=0,b=0,a=0;
					comma.GetInt(ref a);
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					report.lColorPaper = Color.FromArgb(a,r,g,b);
				}
				else if(command == "Table") 
				{
					string begin = "";
					comma.GetString(ref begin);
					if(begin == "Begin") 
					{
						TABLE_STRUCT table = LoadOneTable(filename, reader, bBoxUse, report.TableCount);
						if(table == null)	return null;
						report.tableBuf.Add(table);
					}
					else 
					{
						string msg;
						msg = String.Format("Report file is destroyed. (not Table,Begin)");
						OpenErrorMessage(filename, msg, bBoxUse); 
						return null;
					}
				}
				else if(command == "Header") 
				{
					string begin = "";
					comma.GetString(ref begin);
					if(begin == "Begin") 
					{
						HEAD_FOOT_STRUCT hf = LoadHeaderFooter(filename, reader, bBoxUse, command);
						if(hf == null)	return null;
						report.header = hf;
					}
					else 
					{
						string msg;
						msg = String.Format("Report file is destroyed. (not Header,Begin)");
						OpenErrorMessage(filename, msg, bBoxUse); 
						return null;
					}
				}
				else if(command == "Footer") 
				{
					string begin = "";
					comma.GetString(ref begin);
					if(begin == "Begin") 
					{
						HEAD_FOOT_STRUCT hf = LoadHeaderFooter(filename, reader, bBoxUse, command);
						if(hf == null)	return null;
						report.footer = hf;
					}
					else 
					{
						string msg;
						msg = String.Format("Report file is destroyed. (not Header,Begin)");
						OpenErrorMessage(filename, msg, bBoxUse); 
						return null;
					}
				}
				else 
				{
					string msg;
					msg = String.Format("Unknowned report field. ({0})", command);
					OpenErrorMessage(filename, msg, bBoxUse);
				}
			}

			return report;
		}

		public static string GetReportDescription(string filename)
		{
			if(!File.Exists(filename))	return null;
			string ext = Path.GetExtension(filename);

			if(String.Compare(ext, ".rptx", true) == 0) 
			{
				TextReader reader = new StreamReader(filename);
				if(reader == null)	return null;
				string des = GetReportDescriptionFromRptX(filename, reader);
				reader.Close();
				return des;
			}
			else 
			{
				FileStream fs = File.OpenRead(filename);
				if(fs == null)	return null;
				BinaryReader reader = new BinaryReader(fs);
				string des = GetReportDescriptionFromRpt(filename, reader);
				reader.Close();
				return des;
			}
		}

		static string GetReportDescriptionFromRpt(string filename, BinaryReader file)
		{
			REPORT_FILE_HEADER head = new REPORT_FILE_HEADER();
			string message;
			REPORT_STRUCT report;
			FileLoadCrcSum16 crc_check;
			ushort crc;

			//file header를 불러온다.

			crc_check = new FileLoadCrcSum16(file);
			head.id = crc_check.ReadBytes(7);
			head.version_major = crc_check.ReadSingle();
			head.version_minor = crc_check.ReadSingle();
			head.crc = file.ReadUInt16();
			
			report = new REPORT_STRUCT();

			crc_check = new FileLoadCrcSum16(file);

			report.margin_left = crc_check.ReadInt32();
			report.margin_top  = crc_check.ReadInt32();
			report.margin_right = crc_check.ReadInt32();
			report.margin_bottom = crc_check.ReadInt32();
			report.margin_header = crc_check.ReadInt32();
			report.margin_footer = crc_check.ReadInt32();
			report.bOrientation = crc_check.ReadSByte();
			report.wOpticRate = crc_check.ReadUInt16();
			report.cursor_table = crc_check.ReadInt32();
			report.cursor_x1 = crc_check.ReadInt32();
			report.cursor_y1 = crc_check.ReadInt32();
			report.cursor_x2 = crc_check.ReadInt32();
			report.cursor_y2 = crc_check.ReadInt32();
			int nTableCount = crc_check.ReadInt32();
			report.description = ReadBytes(crc_check, 80);
			report.lColorPaper = crc_check.ReadColor();
			crc_check.ReadUInt32();	// TABLE_STRUCT *tableBuf;
			ReadHeadFootStruct(crc_check, ref report.header);
			ReadHeadFootStruct(crc_check, ref report.footer);
			crc_check.ReadBytes(80);	// char extra[80];
			crc = file.ReadUInt16();	// WORD crc

			if(crc != crc_check.crc) 
			{
				message = String.Format("Report Struct crc mismatched\nnot report file or file destroyed.");
				OpenErrorMessage(filename, message, true);
				return null;
			}

			return report.description;
		}

		static string GetReportDescriptionFromRptX(string filename, TextReader reader)
		{
			REPORT_STRUCT report = new REPORT_STRUCT();
			CommaTextReader comma = new CommaTextReader();
			string one_line;
			string command = "";

			one_line = reader.ReadLine();
			if(one_line == null)	return null;
			if(one_line != "Report,Begin,") 
			{
				return null;
			}

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				if(one_line == "Report,End,")	break;
				comma.Set(one_line);
				comma.GetString(ref command);

				if(command == "Description") 
				{
					comma.GetString(ref report.description);
					return report.description;
				}
			}

			return null;
		}
	}
}


