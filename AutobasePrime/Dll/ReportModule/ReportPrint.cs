using System;
using System.Drawing;
using NetTools.OldDefine;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Collections;
using NetTools;
using AutoLibLocal;
using System.IO;
using ReportBasicLib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportModule
{
	class PRINT_PAGE_LIST 
	{
		public int table;
		public int cell_y;
	}

	class REPORT_EXECUTE_INFO
	{
		public int cur_page;
		public int total_page;
		public string filename;
	}

	/// <summary>
	/// Summary description for ReportPrint.
	/// </summary>
	public class ReportPrint
	{
		REPORT_STRUCT reportPrint;
		//REPORT_STRUCT reportSource;
		int nCurrentPrintPage = 0;
		string sFileName;
		//EnumHandAuto bHandAuto = EnumHandAuto.HAND_MODE;

		//FormReportChild form;

		public ReportPrint()
		{
			//
			// TODO: Add constructor logic here
			//
			
		}

		//static PrinterSettings tempPrinter = new PrinterSettings();
		//static PageSettings tempPage = new PageSettings();

		public void PreviewGo(REPORT_STRUCT source, string filename)
		{
			sFileName = filename;
			PrintDocument pd = new PrintDocument();

			//pd.PrinterSettings     = prnSetting;
			//pd.DefaultPageSettings = pageSetting;

			//pd.BeginPrint += new PrintEventHandler(pd_BeginPrint);
			pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
			//pd.EndPrint += new PrintEventHandler(pd_EndPrint);

			nCurrentPrintPage = 0;

			PrinterSettings tempPrinter = new PrinterSettings();
			PageSettings tempPage = new PageSettings();

			reportPrint = source;
			REPORT_STRUCT report = source;
			tempPage.Landscape = report.bOrientation == 1 ? true : false;

			pd.PrinterSettings     = tempPrinter;
			pd.DefaultPageSettings = tempPage;

			PrintPreviewDialog dialog = new PrintPreviewDialog();

			dialog.Document = pd;

            try
            {
                dialog.ShowDialog(Form.ActiveForm);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Print Preview Error");
            }
		}
		
		public void PrintGo(REPORT_STRUCT source, string filename)
		{
			sFileName = filename;

			PrintDialog dialog = new PrintDialog();

			PrintDocument pd = new PrintDocument();

			reportPrint = source;
			REPORT_STRUCT report = reportPrint;
			PrinterSettings tempPrinter = new PrinterSettings();
			PageSettings tempPage = new PageSettings();

			tempPage.Landscape = report.bOrientation == 1 ? true : false;
			pd.DefaultPageSettings = tempPage;
			dialog.Document = pd;
			dialog.PrinterSettings = tempPrinter;
			//pd.DefaultPageSettings.Landscape = report.bOrientation == 1 ? true : false;

            if (dialog.ShowDialog(Form.ActiveForm) != DialogResult.OK) return;

			tempPage.Landscape = report.bOrientation == 1 ? true : false;

			pd.PrinterSettings     = dialog.PrinterSettings;
			pd.DefaultPageSettings = tempPage;

			pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);

			nCurrentPrintPage = 0;

			pd.Print();
		}

		//-----------------------------------------------------------------
		//	주어진 리포트에서 가로크기가 가장 큰 TABLE의 size를 얻는다.
		//-----------------------------------------------------------------

		public static int GetMaxTableWidth(REPORT_STRUCT report)
		{
			//if(report.nTableCount == 0)	return 0;

			int size = 0;
			int max = 100;
			int i, j;
			TABLE_STRUCT table;
			CELL_STRUCT cell;

			for(i = 0; i < report.TableCount; i++) 
			{
		
				table = (TABLE_STRUCT)report.tableBuf[i];
				size = table.gab_left;
				for(j = 0; j < table.cell_x; j++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[j];
					size += cell.width;
				}
				if(i == 0)	max = size;
				else 
				{
					if(size > max)	max = size;
				}
			}

			return max;
		}

		void CalcBeginPrinting(Graphics g, PageSettings ps, REPORT_STRUCT report)
		{
			int size = ReportPrint.GetMaxTableWidth(report);

			// int res = pDC->GetDeviceCaps(HORZRES);
			int physical_width = (int)ps.Bounds.Width;

			int res = physical_width*(100-report.margin_left-report.margin_right)/100;

			report.wOpticRate = res*100/size;
		}

		void CalcPaperRect(PageSettings ps, REPORT_STRUCT report, RECT rect)
		{
            // 관계없는 초기화라 뺐다 2013-4-3
			//rect.left = 0;
			//rect.top  = 0;
			//rect.right  = ps.PrinterResolution.X;	// HRES
			//rect.bottom = ps.PrinterResolution.Y;	// VRES
	
			int physical_width  = ps.Bounds.Width;
			int physical_height = ps.Bounds.Height;//pDC->GetDeviceCaps(PHYSICALHEIGHT);

			rect.left = physical_width*report.margin_left/100;//-pDC->GetDeviceCaps(PHYSICALOFFSETX);
			rect.top  = physical_height*report.margin_top/100;//-pDC->GetDeviceCaps(PHYSICALOFFSETY);
			rect.right  = rect.left+physical_width-physical_width*(report.margin_left+report.margin_right)/100;
			rect.bottom  = rect.top+physical_height-physical_height*(report.margin_top+report.margin_bottom)/100;
		}

		ArrayList blockPageList = new ArrayList();

		private void pd_BeginPrint(PageSettings ps, Graphics g) 
		{
			CalcBeginPrinting(g, ps, reportPrint);
			RECT rect = new RECT();
			CalcPaperRect(ps, reportPrint, rect);
			CalcPrintPageList(g, reportPrint, rect, blockPageList);
		}

		int GetCellMaxHeight(REPORT_STRUCT report, TABLE_STRUCT table, int posy)
		{
			int cell_pos = table.cell_x*posy;
			int posx;
			int max = 0;
			int height;
			CELL_STRUCT cell;
	
			for(posx = 0; posx < table.cell_x; posx++, cell_pos++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[cell_pos];
				height = FormReportChild.GetCellSizeHeight(report, table, cell);
				if(height > max)	max = height;
			}

			return max;
		}

        int[] BuildRowMaxHeightCache(REPORT_STRUCT report, TABLE_STRUCT table)
        {
            int[] rowMaxHeightCache = new int[table.cell_y];
            FormReportChild.BuildCellViewSizeCache(report, table, null, null, rowMaxHeightCache);
            return rowMaxHeightCache;
        }

		void CalcPrintPageList(Graphics g, REPORT_STRUCT report, RECT rectPaper, ArrayList block)
		{
			PRINT_PAGE_LIST list;
			TABLE_STRUCT table;
			CELL_STRUCT cell;
			int i;
			int current_y = rectPaper.top;
			int cell_pos;
			int posy;
			int y2;

			block.Clear();

			list = new PRINT_PAGE_LIST();

			list.table = 0;
			list.cell_y = 0;
			block.Add(list);

			for(i = 0; i < report.TableCount; i++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[i];
                int[] rowMaxHeightCache = BuildRowMaxHeightCache(report, table);
				cell_pos = 0;

				//current_y += GetRealView(report, table.gab_top);

				if(i == 0) 
				{	// 첫 페이지의 첫Table은 gab을 적용
					current_y += FormReportChild.GetRealView(report, table.gab_top);
				}
				else if(i != 0 && current_y != rectPaper.top) 
				{ // 이외 Table은 
					current_y += FormReportChild.GetRealView(report, table.gab_top);
				}
				else {}

				for(posy = 0; posy < table.cell_y; posy++, cell_pos += table.cell_x) 
				{
					cell = (CELL_STRUCT)table.cellBuf[cell_pos];
					y2 = current_y+rowMaxHeightCache[posy];
					if(y2 >= rectPaper.bottom) 
					{
						list = new PRINT_PAGE_LIST();
						list.table = i;
						list.cell_y = posy;
						block.Add(list);
						current_y = rectPaper.top;
					}
					current_y+=FormReportChild.GetRealView(report, cell.height);
				}
			}
		}

		void ReportPrintOnePage(REPORT_STRUCT report, Graphics g, PageSettings ps, ArrayList block, REPORT_EXECUTE_INFO info, EnumViewMode viewmode)
		{
			TABLE_STRUCT table;
			int l;
			int current_y;
			RECT rect = new RECT(), pr = new RECT();
			PRINT_PAGE_LIST list;
			int current_cell_y = 0;
			bool first_flag = false;

			if(info.cur_page >= block.Count) 
			{
				return;		
			}
			list = (PRINT_PAGE_LIST)block[info.cur_page];

			pr.left = 0;
			pr.top  = 0;
			pr.right  = ps.Bounds.Width;//.PrinterResolution.X;//pDC->GetDeviceCaps(HORZRES);
			pr.bottom = ps.Bounds.Height;//.PrinterResolution.Y;//pDC->GetDeviceCaps(VERTRES);
	
			DrawClass.gcls(g, pr.left, pr.top, pr.right, pr.bottom, report.lColorPaper);

			CalcPaperRect(ps, report, rect);
	
			current_y = rect.top;
			current_cell_y = list.cell_y;

			for(l = list.table; l < report.TableCount; l++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[l];

				if(info.cur_page == 0 && l == 0) 
				{	// 첫 페이지의 첫Table은 gab을 적용
					current_y += FormReportChild.GetRealView(report, table.gab_top);
				}
				else if(l != 0 && current_cell_y == 0 && first_flag == true) 
				{ // 이외 Table은 
					current_y += FormReportChild.GetRealView(report, table.gab_top);
				}
				else {}

				first_flag = true;

				if(PrintDrawOneTable(g, rect, report, table, ref current_y, ref current_cell_y, viewmode))	break;	// 한 페이지를 모두 그렸다.
				current_cell_y = 0;
			}

			int head_start_x=0, head_start_y=0;
			int foot_start_x=0, foot_start_y=0;

			CalcHeadStart(g, ps, report, ref head_start_x, ref head_start_y);
			PrintHeadFoot(g, report, report.header, head_start_x, head_start_y, info);
			CalcFootStart(g, ps, report, ref foot_start_x, ref foot_start_y);
			PrintHeadFoot(g, report, report.footer, foot_start_x, foot_start_y, info);
		}

		void PrintHeadFoot(Graphics g, REPORT_STRUCT report, HEAD_FOOT_STRUCT head, int start_x, int start_y, REPORT_EXECUTE_INFO info)
		{
			int l;
			DRAW_ONE_OBJECT one;

			for(l = 0; l < head.blockObject.Count; l++) 
			{
				one = (DRAW_ONE_OBJECT)head.blockObject[l];
				if(one.type == EnumDrawType.LINE) 
				{
					DRAW_OBJECT_LINE obj = (DRAW_OBJECT_LINE)one.p;
					DrawLine(g, report, obj, start_x, start_y);
				}
				else if(one.type == EnumDrawType.TEXT) 
				{
					DRAW_OBJECT_TEXT obj = (DRAW_OBJECT_TEXT)one.p;
					DrawText(g, report, obj, start_x, start_y, info);
				}
			}
		}

		void CalcHeadStart(Graphics g, PageSettings ps, REPORT_STRUCT report, ref int startx, ref int starty)
		{
			int physical_width  = ps.Bounds.Width;//pDC->GetDeviceCaps(PHYSICALWIDTH);
			int physical_height = ps.Bounds.Height;//pDC->GetDeviceCaps(PHYSICALHEIGHT);

			startx = physical_width*report.margin_left/100;//-pDC->GetDeviceCaps(PHYSICALOFFSETX);
			starty = physical_height*report.margin_header/100-FormReportChild.GetRealView(report, report.header.height);//-pDC->GetDeviceCaps(PHYSICALOFFSETY)
		}

		void CalcFootStart(Graphics g, PageSettings ps, REPORT_STRUCT report, ref int startx, ref int starty)
		{
			int physical_width  = ps.Bounds.Width;//pDC->GetDeviceCaps(PHYSICALWIDTH);
			int physical_height = ps.Bounds.Height;//pDC->GetDeviceCaps(PHYSICALHEIGHT);

			startx = physical_width*report.margin_left/100;//-pDC->GetDeviceCaps(PHYSICALOFFSETX);
			starty = physical_height-physical_height*report.margin_footer/100;//-pDC->GetDeviceCaps(PHYSICALOFFSETY);
		}

		bool PrintDrawOneTable(Graphics g, RECT rectPaper, REPORT_STRUCT report, TABLE_STRUCT table, ref int current_y, ref int posy, EnumViewMode viewmode)
		{
			int x;
			int posx;
			CELL_STRUCT cell;
			int cell_pos;
			int x2, y2;
            int[] widthCache = new int[table.cellBuf.Count];
            int[] heightCache = new int[table.cellBuf.Count];
            int[] rowMaxHeightCache = new int[table.cell_y];

            FormReportChild.BuildCellViewSizeCache(report, table, widthCache, heightCache, rowMaxHeightCache);

			cell_pos = table.cell_x*posy;

			for(; posy < table.cell_y; posy++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[cell_pos];
				y2 = current_y+rowMaxHeightCache[posy];
				if(y2 >= rectPaper.bottom) 
				{
					return true;
				}
		
				for(posx = 0, x = FormReportChild.GetRealView(report, table.gab_left)+rectPaper.left; posx < table.cell_x; posx++, cell_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[cell_pos];
					x2 = x+widthCache[cell_pos];
					y2 = current_y+heightCache[cell_pos];

					if(cell.cGroup != 2) 
					{
						FormReportChild.DrawOneCell(g, report, x, current_y, x2, y2, cell, viewmode, false);
					}

					x+=FormReportChild.GetRealView(report, cell.width);
				}
				current_y+=FormReportChild.GetRealView(report, cell.height);
			}

			return false;
		}

		private void pd_PrintPage(object sender, PrintPageEventArgs ev) 
		{
			if(nCurrentPrintPage == 0) 
			{
				pd_BeginPrint(ev.PageSettings, ev.Graphics);
			}

			REPORT_EXECUTE_INFO info = new REPORT_EXECUTE_INFO();

			info.filename = sFileName;
			info.total_page = this.blockPageList.Count;
			info.cur_page = nCurrentPrintPage;

			ReportPrintOnePage(reportPrint, ev.Graphics, ev.PageSettings, blockPageList, info, EnumViewMode.RUN);

			nCurrentPrintPage++;
			if(nCurrentPrintPage >= blockPageList.Count) 
			{
				ev.HasMorePages = false;
				nCurrentPrintPage = 0;
			}
			else
				ev.HasMorePages = true;
		}

		private void pd_EndPrint(object sender, PrintEventArgs ev) 
		{

		}

		void DrawLine(Graphics g, REPORT_STRUCT report, DRAW_OBJECT_LINE obj, int start_x, int start_y)
		{
			int x1, y1, x2, y2;
	
			Pen pen = new Pen(obj.color, obj.thick);
			//pen.DashStyle = obj.style;

			x1 = start_x+FormReportChild.GetRealView(report, obj.x1);
			y1 = start_y+FormReportChild.GetRealView(report, obj.y1);
			x2 = start_x+FormReportChild.GetRealView(report, obj.x2);
			y2 = start_y+FormReportChild.GetRealView(report, obj.y2);

			g.DrawLine(pen, x1, y1, x2, y2);
		}

		void ConvertHeadFootCommand(out string buf, int limit, string source, REPORT_EXECUTE_INFO info)
		{
			string command = "";
			bool command_mode = false;
			string imsi;
			int  i;

			buf = "";

			for(i = 0; i < source.Length; i++) 
			{
				if(command_mode == true) 
				{
					command += source[i];
					
					if(source[i] == ']') 
					{	// command close
						command_mode = false;
						if(String.Compare(command, "[Page]") == 0) 
						{
							imsi = String.Format("{0}", info.cur_page+1);
							buf += imsi;
						}
						else if(String.Compare(command, "[TotalPage]") == 0) 
						{
							imsi = String.Format("{0}", info.total_page);
							buf += imsi;
						}
						else if(String.Compare(command, "[FileName]") == 0) 
						{
							imsi = String.Format("{0}", info.filename);
							buf += imsi;
						}
						else if(String.Compare(command, "[Date]") == 0) 
						{
                            DateTime d = DateTimeServer.Now;

							imsi = String.Format("{0}/{1:00}/{2:00}", d.Year, d.Month, d.Day);
							buf += imsi;
						}
						else if(String.Compare(command, "[Time]") == 0) 
						{
                            DateTime t = DateTimeServer.Now;

							imsi = String.Format("{0:00}:{1:00}:{2:00}", t.Hour, t.Minute, t.Second);
							buf += imsi;
						}
					}
				}
				else 
				{
					if(source[i] == '&') 
					{	// command
						command_mode = true;
						command = "";
					}
					else 
					{
						buf += source[i];
					}
				}
			}
		}

		void DrawText(Graphics g, REPORT_STRUCT report, DRAW_OBJECT_TEXT obj, int start_x, int start_y, REPORT_EXECUTE_INFO info)
		{
			string buf;
			Rectangle r = new Rectangle();
			StringFormat format = new StringFormat();
			Font font;

			double height = FormReportChild.GetRealView(report, obj.fontSize);
			font = new Font(obj.fontName, (float)height, obj.fontStyle);
	
			r.X = start_x+FormReportChild.GetRealView(report, obj.x1);
			r.Y = start_y+FormReportChild.GetRealView(report, obj.y1);
			r.Width = Math.Abs(r.Left-(start_x+FormReportChild.GetRealView(report, obj.x2)));
			r.Height = Math.Abs(r.Top-(start_y+FormReportChild.GetRealView(report, obj.y2)));

			ConvertHeadFootCommand(out buf, 1000, obj.text, info);

			
			if(obj.align == 0)			format.Alignment = StringAlignment.Near;
			else if(obj.align == 1)		format.Alignment = StringAlignment.Center;
			else						format.Alignment = StringAlignment.Far;

            SafeException.SafeDrawString(g, buf, font, new SolidBrush(obj.color), r, format); 
		}

		public async Task PrintReportFile(string msg_title, string rpt_name, EnumHandAuto hand_auto, PrintDocument pd)
		{
			string filename;
	
			//bHandAuto = hand_auto;
			
			filename = String.Format("{0}\\report\\{1}", TotalConfig.sDirWorkProject, rpt_name);

			sFileName = filename;

			REPORT_STRUCT source = null;

			if(ConfigVarTotal.bLocalFlag) 
			{
				if(!File.Exists(filename)) 
				{
					/*
		#if	defined (COMPILE_HANGUL)
				MessageScreen(msg_title, "리포트 파일인 {0}파일을 찾을 수 없습니다.", filename);
		#else
						MessageScreen(msg_title, "Can't find {0} report file.", filename);
		#endif
		*/
					return;
				}

				source = ReportFile.ReportLoad(filename, false);
				if(source == null)	return;
			}

			reportPrint = await FormReportChild.MakeRunReportPublic(source, hand_auto, rpt_name);

			if(pd.DefaultPageSettings != null) 
			{
				pd.DefaultPageSettings.Landscape = source.bOrientation == 1 ? true : false;
			}
			else 
			{
				PageSettings tempPage = new PageSettings();
				tempPage.Landscape = source.bOrientation == 1 ? true : false;
				pd.DefaultPageSettings = tempPage;
			}

			pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);

			nCurrentPrintPage = 0;

			pd.Print();

			// 이부분이 없으니까 여러날 동시 인쇄할때 뒤에 인쇄하는 페이지가 길어지면 뒷부분만 나온다.
			// 아마 같은 pd를 계속헤서 사용해서 그런것 같다.
			pd.PrintPage -= new PrintPageEventHandler(this.pd_PrintPage);
		}

        public List<byte[]> MakeBitmapPages(REPORT_STRUCT report, string fileName)
        {
            if (report == null)
            {
                return null;
            }

            reportPrint = report;
            sFileName = fileName;
            nCurrentPrintPage = 0;

            List<byte[]> arrayBitmap = new List<byte[]>();
            PageSettings ps = new PageSettings();

            while (true)
            {
                using (Bitmap bitmap = new Bitmap(ps.Bounds.Width, ps.Bounds.Height))
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    if (nCurrentPrintPage == 0)
                    {
                        pd_BeginPrint(ps, g);
                    }

                    REPORT_EXECUTE_INFO info = new REPORT_EXECUTE_INFO();

                    info.filename = sFileName;
                    info.total_page = this.blockPageList.Count;
                    info.cur_page = nCurrentPrintPage;

                    ReportPrintOnePage(reportPrint, g, ps, blockPageList, info, EnumViewMode.RUN);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        arrayBitmap.Add(stream.ToArray());
                    }
                }

                nCurrentPrintPage++;

                if (nCurrentPrintPage >= blockPageList.Count)
                {
                    nCurrentPrintPage = 0;
                    break;
                }
            }

            return arrayBitmap;
        }

        public async Task<List<byte[]>> MakeBitmapReportResults(string msg_title, string rpt_name, EnumHandAuto hand_auto)
        {
            string filename;

            filename = String.Format("{0}\\report\\{1}", TotalConfig.sDirWorkProject, rpt_name);

            sFileName = filename;

            REPORT_STRUCT source = null;

            if (ConfigVarTotal.bLocalFlag)
            {
                if (!File.Exists(filename))
                {
                    return null;
                }

                source = ReportFile.ReportLoad(filename, false);
                if (source == null) return null;
            }

            reportPrint = await FormReportChild.MakeRunReportPublic(source, hand_auto, rpt_name);
            return MakeBitmapPages(reportPrint, sFileName);
        }
	}
}

