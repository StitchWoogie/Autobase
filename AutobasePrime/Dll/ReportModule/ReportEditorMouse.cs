using System;
using NetTools;
using System.Windows.Forms;
using System.Drawing;
using ReportBasicLib;

namespace ReportModule
{
	public class WORK_VIEW_STRUCT 
	{ 
		public EnumViewMode nViewMode = EnumViewMode.EDIT;
	}	

	public enum EnumViewMode 
	{
		EDIT,
		RUN,
		COMMAND,
		TAG,
		TIME,
	}

	enum EnumMousePosition
	{
		NO,
		RECORD,
		FIELD,
		CELL,
		TABLE,
		SIZE_CELL_X,
		SIZE_CELL_Y,
		SIZE_TABLE_X,
		SIZE_TABLE_Y,
	};

	/// <summary>
	/// Summary description for ReportEditorMouse.
	/// </summary>
	public class ReportEditorMouse
	{
		static bool bMouseCaptureFlag = false;
		static EnumMousePosition mouseCaptureMethod;
		static int mouseCaptureTableNo;
		static int mouseCaptureCellX;
		static int mouseCaptureCellY;
		static int nOldMX, nOldMY;
		static int nStartMX, nStartMY;
		static bool bDrawNotVert;
		static bool bDrawNotHorz;
		
		public ReportEditorMouse()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void MouseDown(FormReportChild form, System.Windows.Forms.MouseEventArgs e)
		{
			int table_no=0;
			int posx=0;
			int posy=0;
			REPORT_STRUCT report = form.GetReportStruct();
			TABLE_STRUCT table;
			WORK_VIEW_STRUCT work = form.workView;

			EnumMousePosition retn = CalcMousePosition(form, e, ref table_no, ref posx, ref posy);

			nStartMX = nOldMX = e.X;
			nStartMY = nOldMY = e.Y;

			if(work.nViewMode == EnumViewMode.RUN) 
			{
				bMouseCaptureFlag = true;
				form.Capture = true;
				return;
			}

			bDrawNotVert = false;
			bDrawNotHorz = false;
	
			if(retn == EnumMousePosition.CELL) 
			{
				form.SetCursorPosition(report, table_no, posx, posy);
				form.Invalidate();
				if(e.Button == MouseButtons.Left && e.Clicks >= 2)
				{
					OnLButtonDblClk(form);
					return;
				}
				mouseCaptureMethod = EnumMousePosition.CELL;
				mouseCaptureTableNo = table_no;
				mouseCaptureCellX = posx;
				mouseCaptureCellY = posy;
				bMouseCaptureFlag = true;
				form.Capture = true;
				return;
			}
			else if(retn == EnumMousePosition.RECORD) 
			{
				table = (TABLE_STRUCT)report.tableBuf[table_no];
				form.SetCursorPosition(report, table_no, 0, posy, table.cell_x-1, posy);
				form.Invalidate();
				if(e.Button == MouseButtons.Left && e.Clicks >= 2)
				{
					OnLButtonDblClk(form);
					return;
				}
				mouseCaptureMethod = EnumMousePosition.RECORD;
				mouseCaptureTableNo = table_no;
				mouseCaptureCellX = posx;
				mouseCaptureCellY = posy;
				bMouseCaptureFlag = true;
				form.Capture = true;
				return;
			}
			else if(retn == EnumMousePosition.FIELD) 
			{
				table = (TABLE_STRUCT)report.tableBuf[table_no];
				form.SetCursorPosition(report, table_no, posx, 0, posx, table.cell_y-1);
				form.Invalidate();
				if(e.Button == MouseButtons.Left && e.Clicks >= 2)
				{
					OnLButtonDblClk(form);
					return;
				}
				mouseCaptureMethod = EnumMousePosition.FIELD;
				mouseCaptureTableNo = table_no;
				mouseCaptureCellX = posx;
				mouseCaptureCellY = posy;
				bMouseCaptureFlag = true;
				form.Capture = true;
				return;
			}
			else if(retn == EnumMousePosition.TABLE) 
			{
				table = (TABLE_STRUCT)report.tableBuf[table_no];
				form.SetCursorPosition(report, table_no, 0, 0, table.cell_x-1, table.cell_y-1);
				form.Invalidate();
				if(e.Button == MouseButtons.Left && e.Clicks >= 2)
				{
					OnLButtonDblClk(form);
				}
				return;
			}
			else if(retn == EnumMousePosition.SIZE_TABLE_X ||
				retn == EnumMousePosition.SIZE_CELL_X) 
			{
				table = (TABLE_STRUCT)report.tableBuf[table_no];
				mouseCaptureMethod = retn;
				mouseCaptureTableNo = table_no;
				mouseCaptureCellX = posx;
				mouseCaptureCellY = posy;
				bMouseCaptureFlag = true;
				form.Capture = true;
				nStartMX = nOldMX = e.X;
				nStartMY = nOldMY = e.Y;
				//CClientDC dc(this);
				//DrawNotVertLine(&dc);
				bDrawNotVert = true;
				bDrawNotHorz = false;
				form.Invalidate();
				return;
			}
			else if(retn == EnumMousePosition.SIZE_TABLE_Y ||
				retn == EnumMousePosition.SIZE_CELL_Y) 
			{
				table = (TABLE_STRUCT)report.tableBuf[table_no];
				mouseCaptureMethod = retn;
				mouseCaptureTableNo = table_no;
				mouseCaptureCellX = posx;
				mouseCaptureCellY = posy;
				bMouseCaptureFlag = true;
				form.Capture = true;
				nStartMX = nOldMX = e.X;
				nStartMY = nOldMY = e.Y;
				//CClientDC dc(this);
				//DrawNotHorzLine(&dc);
				bDrawNotHorz = true;
				bDrawNotVert = false;
				form.Invalidate();
				return;
			}
		}

		static EnumMousePosition CalcMousePosition(FormReportChild form, System.Windows.Forms.MouseEventArgs e, ref int sel_table_no, ref int sel_cell_x, ref int sel_cell_y) 
		{
			REPORT_STRUCT report = form.GetReportStruct();

			if(report == null)	return EnumMousePosition.NO;

			int x;
			int posx, posy;
			CELL_STRUCT cell;
			int cell_pos;
			int x2, y2;
			int current_y = -form.nVerScrollPos + form.GetCanvasOriginY();
			TABLE_STRUCT table;
			int table_no;

			for(table_no = 0; table_no < report.TableCount; table_no++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[table_no];

				x2 = form.GetRealView(table.gab_left) - form.nHorScrollPos + form.GetCanvasOriginX();
				y2 = current_y+form.GetRealView(table.gab_top);

				if(e.X <= x2 && e.Y <= y2) 
				{
					sel_table_no = table_no;
					sel_cell_x = 0;
					sel_cell_y = 0;
					return EnumMousePosition.TABLE;
				}

				for(posx = 0, x = form.GetCanvasOriginX() + form.GetRealView(table.gab_left) - form.nHorScrollPos; posx < table.cell_x; posx++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[posx];

					x2 = x+form.GetRealView(cell.width);

					if(e.X >= x && e.X <= x2 && e.Y >= y2-1 && e.Y <= y2+1) 
					{
						sel_table_no = table_no;
						return EnumMousePosition.SIZE_TABLE_Y;
					}
		
					if(e.X >= x && e.X <= x2 && e.Y <= y2) 
					{
						sel_table_no = table_no;
						sel_cell_x = posx;
						sel_cell_y = 0;//posy;
						return EnumMousePosition.FIELD;
					}

					x+=form.GetRealView(cell.width);
				} 
		
				cell_pos = 0;
				current_y += form.GetRealView(table.gab_top);

				for(posy = 0; posy < table.cell_y; posy++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[cell_pos];
					x2 = form.GetCanvasOriginX() + form.GetRealView(table.gab_left) - form.nHorScrollPos;
					y2 = current_y+form.GetRealView(cell.height);

					if(e.Y >= current_y && e.X >= x2-1 && e.Y <= y2 && e.X <= x2+1) 
					{
						sel_table_no = table_no;
						return EnumMousePosition.SIZE_TABLE_X;
					}

					if(e.Y >= current_y && e.X <= x2 && e.Y <= y2) 
					{
						sel_table_no = table_no;
						sel_cell_x = 0;
						sel_cell_y = posy;
						return EnumMousePosition.RECORD;
					}

					for(posx = 0, x = form.GetCanvasOriginX() + form.GetRealView(table.gab_left) - form.nHorScrollPos; posx < table.cell_x; posx++, cell_pos++) 
					{
						cell = (CELL_STRUCT)table.cellBuf[cell_pos];
						x2 = x+form.GetRealView(cell.width);
						y2 = current_y+form.GetRealView(cell.height);

						if(e.X >= x2-1 && e.Y >= current_y && e.X <= x2+1 && e.Y <= y2) 
						{
							sel_table_no = table_no;
							sel_cell_x = posx;
							sel_cell_y = posy;
							return EnumMousePosition.SIZE_CELL_X;
						}

						if(e.X >= x && e.X <= x2 && e.Y >= y2-1 && e.Y <= y2+1) 
						{
							sel_table_no = table_no;
							sel_cell_x = posx;
							sel_cell_y = posy;
							return EnumMousePosition.SIZE_CELL_Y;
						}

						if(e.X >= x && e.Y >= current_y && e.X <= x2 && e.Y <= y2) 
						{
							sel_table_no = table_no;
							sel_cell_x = posx;
							sel_cell_y = posy;	
							return EnumMousePosition.CELL;
						}

						x+=form.GetRealView(cell.width);
					}
					current_y+=form.GetRealView(cell.height);
				}
			}

			return EnumMousePosition.NO;
		}

		public static void MouseMove(FormReportChild form, System.Windows.Forms.MouseEventArgs e)
		{
			WORK_VIEW_STRUCT work = form.workView;

			if(bMouseCaptureFlag == false) 
			{	// cursor 모양을 바꾼다.
				if(work.nViewMode == EnumViewMode.RUN)
					form.Cursor = Cursors.SizeAll;
				else
					ChangeMouseCursor(form, e);
				return;
			}

			if(work.nViewMode == EnumViewMode.RUN) 
			{
				int mx = form.nHorScrollPos-(e.X-nOldMX);
				int my = form.nVerScrollPos-(e.Y-nOldMY);

				//if(mx < 0)	mx = 0;
				//if(mx > form.nHorScrollHap)	mx = form.nHorScrollHap;
				//if(my < 0)	my = 0;
				//if(my > form.nVerScrollHap)	my = form.nVerScrollHap;

				if(mx != form.nHorScrollPos || my != form.nVerScrollPos) 
				{
					form.nHorScrollPos = mx;
					form.nVerScrollPos = my;
					form.AutoScrollPosition = new Point(mx, my);
					form.Invalidate();
				}

				nOldMX = e.X;
				nOldMY = e.Y;

				return;
			}

			int posx=0, posy=0;
			REPORT_STRUCT report = form.GetReportStruct();

			if(mouseCaptureMethod == EnumMousePosition.CELL) 
			{
				CalcMousePositionOnTable(form, e, mouseCaptureTableNo, ref posx, ref posy);
				if(posx != report.cursor_x1 || posy != report.cursor_y1) 
				{
					form.SetCursorPosition(report, mouseCaptureTableNo, posx, posy, report.cursor_x2, report.cursor_y2);
					form.Invalidate(); 
				}
			}
			else if(mouseCaptureMethod == EnumMousePosition.RECORD) 
			{
				CalcMousePositionOnRecord(form, e, mouseCaptureTableNo, ref posy);
				if(posy != report.cursor_y1) 
				{
					form.SetCursorPosition(report, mouseCaptureTableNo, report.cursor_x1, posy, report.cursor_x2, report.cursor_y2);
					form.Invalidate();
				}
			}
			else if(mouseCaptureMethod == EnumMousePosition.FIELD) 
			{
				CalcMousePositionOnField(form, e, mouseCaptureTableNo, ref posx);
				if(posx != report.cursor_x1) 
				{
					form.SetCursorPosition(report, mouseCaptureTableNo, posx, report.cursor_y1, report.cursor_x2, report.cursor_y2);
					form.Invalidate();
				}
			}
			else if(mouseCaptureMethod == EnumMousePosition.SIZE_TABLE_X ||
				mouseCaptureMethod == EnumMousePosition.SIZE_CELL_X) 
			{
				//CClientDC dc(this);
				//DrawNotVertLine(&dc);
				nOldMX = e.X;
				nOldMY = e.Y;
				form.Invalidate();
				//DrawNotVertLine(&dc);
			}
			else if(mouseCaptureMethod == EnumMousePosition.SIZE_TABLE_Y ||
				mouseCaptureMethod == EnumMousePosition.SIZE_CELL_Y) 
			{
				//CClientDC dc(this);
				//DrawNotHorzLine(&dc);
				nOldMX = e.X;
				nOldMY = e.Y;
				form.Invalidate();
				//DrawNotHorzLine(&dc);
			}
		}

		public static void MouseUp(FormReportChild form, System.Windows.Forms.MouseEventArgs e)
		{
			WORK_VIEW_STRUCT work = form.workView;

			if(bMouseCaptureFlag == false)	return;

			bMouseCaptureFlag = false;
			form.Capture = false;

			if(work.nViewMode == EnumViewMode.RUN)		return;

			REPORT_STRUCT report = form.GetReportStruct();
			TABLE_STRUCT table;

			if(mouseCaptureMethod == EnumMousePosition.SIZE_TABLE_X) 
			{
				int move = (nOldMX-nStartMX)*100/report.wOpticRate;
				if(move != 0) 
				{
					if(Tools.IsLangKorean()) 
					{
						ReportEditorUndo.UndoSave(form, "표의 가로(X) 위치 이동");
					}
					else if(Tools.IsLangChinese()) 
					{
						ReportEditorUndo.UndoSave(form, "表格的水平(X)位置移动");
					}
					else 
					{
						ReportEditorUndo.UndoSave(form, "Move Table X Position");
					}
					form.SetChangeFlag();
					table = (TABLE_STRUCT)report.tableBuf[mouseCaptureTableNo];
					table.gab_left += move;
					if(table.gab_left < 0)	table.gab_left = 0;
					form.ScrollUpdate();
					form.Invalidate();
				}
				else 
				{
					//CClientDC dc(this);
					//DrawNotVertLine(&dc);
					form.Invalidate();
				}
			}
			else if(mouseCaptureMethod == EnumMousePosition.SIZE_TABLE_Y) 
			{
				int move = (nOldMY-nStartMY)*100/report.wOpticRate;
				if(move != 0) 
				{
					if(Tools.IsLangKorean()) 
					{
						ReportEditorUndo.UndoSave(form, "표의 세로(Y) 위치 이동");
					}
					else if(Tools.IsLangChinese()) 
					{
						ReportEditorUndo.UndoSave(form, "表格的垂直(Y)位置移动");
					}
					else 
					{
						ReportEditorUndo.UndoSave(form, "Move Table Y Position");
					}
					form.SetChangeFlag();
					table = (TABLE_STRUCT)report.tableBuf[mouseCaptureTableNo];
					table.gab_top += move;
					if(table.gab_top < 2)	table.gab_top = 2;
					form.ScrollUpdate();
					form.Invalidate();
				}
				else 
				{
					//CClientDC dc(this);
					//DrawNotHorzLine(&dc);
					form.Invalidate();
				}
			}
			else if(mouseCaptureMethod == EnumMousePosition.SIZE_CELL_X) 
			{
				int move = (nOldMX-nStartMX)*100/report.wOpticRate;
				if(move != 0) 
				{
					if(Tools.IsLangKorean()) 
					{
						ReportEditorUndo.UndoSave(form, "셀의 가로 크기 변경");
					}
					else if(Tools.IsLangChinese()) 
					{
						ReportEditorUndo.UndoSave(form, "更改单元格水平大小");
					}
					else 
					{
						ReportEditorUndo.UndoSave(form, "Resize Cell X");
					}
					form.SetChangeFlag();
					table = (TABLE_STRUCT)report.tableBuf[mouseCaptureTableNo];
					ChangeWidth(table, mouseCaptureCellX, move);
					form.ScrollUpdate();
					form.Invalidate();
				}
				else 
				{
					//CClientDC dc(this);
					//DrawNotVertLine(&dc);
					form.Invalidate();
				}
			}
			else if(mouseCaptureMethod == EnumMousePosition.SIZE_CELL_Y) 
			{
				int move = (nOldMY-nStartMY)*100/report.wOpticRate;
				if(move != 0) 
				{
					if(Tools.IsLangKorean()) 
					{
						ReportEditorUndo.UndoSave(form, "셀의 세로(Y) 크기 변경");
					}
					else if(Tools.IsLangChinese()) 
					{
						ReportEditorUndo.UndoSave(form, "更改单元格垂直大小"); 
					}
					else 
					{
						ReportEditorUndo.UndoSave(form, "Resize Cell Y");
					}
					form.SetChangeFlag();
					table = (TABLE_STRUCT)report.tableBuf[mouseCaptureTableNo];
					ChangeHeight(table, mouseCaptureCellY, move);
					form.ScrollUpdate();
					form.Invalidate();
				}
				else 
				{
					//CClientDC dc(this);
					//DrawNotHorzLine(&dc);
					form.Invalidate();
				}
			}
		}

		static void ChangeMouseCursor(FormReportChild form, System.Windows.Forms.MouseEventArgs e)
		{
			int table_no = 0;
			int posx = 0, posy = 0;

			EnumMousePosition retn = CalcMousePosition(form, e, ref table_no, ref posx, ref posy);

			if(retn == EnumMousePosition.RECORD) 
			{
				form.Cursor = form.cursorRight;
			}
			else if(retn == EnumMousePosition.FIELD) 
			{
				form.Cursor = form.cursorDown;
			}
			else if(retn == EnumMousePosition.TABLE) 
			{
				form.Cursor = form.cursorRightDown;
			}
			else if(retn == EnumMousePosition.SIZE_CELL_X) 
			{
				form.Cursor = Cursors.SizeWE;
			}
			else if(retn == EnumMousePosition.SIZE_CELL_Y) 
			{
				form.Cursor = Cursors.SizeNS;
			}
			else if(retn == EnumMousePosition.SIZE_TABLE_X) 
			{
				form.Cursor = Cursors.SizeWE;
			}
			else if(retn == EnumMousePosition.SIZE_TABLE_Y) 
			{
				form.Cursor = Cursors.SizeNS;
			}
			else 
			{
				form.Cursor = Cursors.Arrow;
			}
		}

		static void CalcMousePositionOnTable(FormReportChild form, System.Windows.Forms.MouseEventArgs e, int curr_table, ref int sel_cell_x, ref int sel_cell_y) 
		{
			REPORT_STRUCT report = form.GetReportStruct();

			int x;
			int posx, posy;
			CELL_STRUCT cell;
			int cell_pos;
			int x2, y2;
			int current_y = -form.nVerScrollPos + form.GetCanvasOriginY();
			TABLE_STRUCT table;
			int table_no;

			table = (TABLE_STRUCT)report.tableBuf[curr_table];

			x = form.GetCanvasOriginX() + form.GetRealView(table.gab_left) - form.nHorScrollPos;
			if(e.X < x) 
			{
				sel_cell_x = 0;
				goto ok_seek_x;
			}
			for(posx = 0; posx < table.cell_x; posx++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[posx];

				x2 = x+form.GetRealView(cell.width);
	
				if(e.X >= x && e.X <= x2) 
				{
					sel_cell_x = posx;
					goto ok_seek_x;
				}

				x+=form.GetRealView(cell.width);
			}
			// 오른쪽 끝으로 갈 때까지 찾지 못하면 마지막 cell
			sel_cell_x = table.cell_x-1;

			ok_seek_x:	// x 를 찾았다.

				//----------------------------
				// y를 찾는다.
				// 먼저 테이블까지 찾아간다.
				//----------------------------

				for(table_no = 0; table_no < curr_table; table_no++) 
				{
					table = (TABLE_STRUCT)report.tableBuf[table_no];

					cell_pos = 0;
					current_y += form.GetRealView(table.gab_top);

					for(posy = 0; posy < table.cell_y; posy++, cell_pos += table.cell_x) 
					{
						cell = (CELL_STRUCT)table.cellBuf[cell_pos];
						current_y+=form.GetRealView(cell.height);
					}
				}

			table = (TABLE_STRUCT)report.tableBuf[curr_table];

			y2 = current_y+form.GetRealView(table.gab_top);

			if(e.Y < y2) 
			{
				sel_cell_y = 0;
				return;
			}
	
			cell_pos = 0;
			current_y += form.GetRealView(table.gab_top);

			for(posy = 0; posy < table.cell_y; posy++, cell_pos += table.cell_x) 
			{
				cell = (CELL_STRUCT)table.cellBuf[cell_pos];
				y2 = current_y+form.GetRealView(cell.height);
				if(e.Y >= current_y && e.Y <= y2) 
				{
					sel_cell_y = posy;
					return;
				}
		
				current_y+=form.GetRealView(cell.height);
			}

			sel_cell_y = table.cell_y-1;
		}

		static void CalcMousePositionOnRecord(FormReportChild form, System.Windows.Forms.MouseEventArgs e, int curr_table, ref int sel_cell_y) 
		{
			REPORT_STRUCT report = form.GetReportStruct();

			int posy;
			CELL_STRUCT cell;
			int cell_pos;
			int y2;
			int current_y = -form.nVerScrollPos + form.GetCanvasOriginY();
			TABLE_STRUCT table;
			int table_no;

			//----------------------------
			// y를 찾는다.
			// 먼저 테이블까지 찾아간다.
			//----------------------------

			for(table_no = 0; table_no < curr_table; table_no++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[table_no];

				cell_pos = 0;
				current_y += form.GetRealView(table.gab_top);

				for(posy = 0; posy < table.cell_y; posy++, cell_pos += table.cell_x) 
				{
					cell = (CELL_STRUCT)table.cellBuf[cell_pos];
					current_y+=form.GetRealView(cell.height);
				}
			}

			table = (TABLE_STRUCT)report.tableBuf[curr_table];

			y2 = current_y+form.GetRealView(table.gab_top);

			if(e.Y < y2) 
			{
				sel_cell_y = 0;
				return;
			}
	
			cell_pos = 0;
			current_y += form.GetRealView(table.gab_top);

			for(posy = 0; posy < table.cell_y; posy++, cell_pos+=table.cell_x) 
			{
				cell = (CELL_STRUCT)table.cellBuf[cell_pos];
				y2 = current_y+form.GetRealView(cell.height);
				if(e.Y >= current_y && e.Y <= y2) 
				{
					sel_cell_y = posy;
					return;
				}
		
				current_y+=form.GetRealView(cell.height);
			}

			sel_cell_y = table.cell_y-1;
		}

		static void CalcMousePositionOnField(FormReportChild form, System.Windows.Forms.MouseEventArgs e, int curr_table, ref int sel_cell_x) 
		{
			REPORT_STRUCT report = form.GetReportStruct();

			int x;
			int posx;
			CELL_STRUCT cell;
			int x2;
			TABLE_STRUCT table;

			table = (TABLE_STRUCT)report.tableBuf[curr_table];

			x = form.GetCanvasOriginX() + form.GetRealView(table.gab_left) - form.nHorScrollPos;
			if(e.X < x) 
			{
				sel_cell_x = 0;
				return;
			}
			for(posx = 0; posx < table.cell_x; posx++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[posx];

				x2 = x+form.GetRealView(cell.width);
	
				if(e.X >= x && e.X <= x2) 
				{
					sel_cell_x = posx;
					return;
				}

				x+=form.GetRealView(cell.width);
			}
			// 오른쪽 끝으로 갈 때까지 찾지 못하면 마지막 cell
			sel_cell_x = table.cell_x-1;
		}

		static void ChangeWidth(TABLE_STRUCT table, int cell_x, int move)
		{
			int i;
			CELL_STRUCT cell;
	
			for(i = 0; i < table.cell_y; i++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[table.cell_x*i+cell_x];
				cell.width += move;
				if(cell.width < 2)	cell.width = 2;
			}
		}

		static void ChangeHeight(TABLE_STRUCT table, int cell_y, int move)
		{
			int i;
			CELL_STRUCT cell;
	
			for(i = 0; i < table.cell_x; i++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[table.cell_x*cell_y+i];
				cell.height += move;
				if(cell.height < 2)	cell.height = 2;
			}
		}

		public static void OnLButtonDblClk(FormReportChild form)
		{
			WORK_VIEW_STRUCT work = form.workView;
				
			if(work.nViewMode == EnumViewMode.RUN)		return;	
			ReportEditorProperty.ChangeProperty(form);
		}

		static void DrawNotVertLine(FormReportChild form, Graphics g)
		{
			Rectangle r;

			r = form.ClientRectangle;
			
			r.X = nOldMX-1;
			r.Width = 3;

			g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 255)), r);
			//pDC->InvertRect(&r);
		}

		static void DrawNotHorzLine(FormReportChild form, Graphics g)
		{
			Rectangle r;

			r = form.ClientRectangle;

			r.Y = nOldMY-1;
			r.Height = 3;

			g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 255)), r);

			//pDC->InvertRect(&r);
		}

		public static void Paint(FormReportChild form, Graphics g)
		{
			if(!bMouseCaptureFlag)	return;
 
			if(bDrawNotHorz) 
			{
				DrawNotHorzLine(form, g);
			}
			if(bDrawNotVert) 
			{
				DrawNotVertLine(form, g);
			}
		}
	}
}

