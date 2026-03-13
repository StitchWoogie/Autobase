using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using DatabaseConnection;
using NetTools;
using NetTools.OldDefine;
using AutoLibLocal;
using AutoLib;
using System.Threading.Tasks;
using System.Threading;
using Npgsql;
using System.Runtime.InteropServices;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for WndDatabase.
	/// </summary>
	public class WndDatabase : System.Windows.Forms.Form
	{
		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
		private const int WM_SETREDRAW = 0x000B;
		private System.ComponentModel.IContainer components;

		//Form objectParent;
		int nUpdateTimeCurrent;
		int nOldSec;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItemOption;
		public System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.Panel panelStatus;
		private System.Windows.Forms.Panel panelPagination;
		private System.Windows.Forms.Button buttonPrevPage;
		private System.Windows.Forms.Label labelPageInfo;
		private System.Windows.Forms.Button buttonNextPage;
		private System.Windows.Forms.ImageList imageListRowHeight;

		public WndDatabase()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			sClassName = "DefaultClassHistoryWindow";

			nRecordCount = 0;
			m_bUseNo = true;
			sTableName = "";

			sSqlTextWhere = "";
			sSqlTextOrderBy = "";
			SetSqlText("", "");

			bUseAlternateRowColor = false;
			bUsePagination = false;
			nPageSize = 100;

			//objectParent = null;

			nUpdateTimeCurrent = 0;
	
			DateTime t = DateTime.Now;
			nOldSec = t.Second;

			UpdateRowHeight();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WndDatabase));
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItemOption = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.m_list = new System.Windows.Forms.ListView();
            this.labelStatus = new System.Windows.Forms.Label();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.panelPagination = new System.Windows.Forms.Panel();
            this.buttonPrevPage = new System.Windows.Forms.Button();
            this.labelPageInfo = new System.Windows.Forms.Label();
            this.buttonNextPage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemOption,
            this.menuItem1,
            this.menuItem2});
            resources.ApplyResources(this.contextMenu1, "contextMenu1");
            // 
            // menuItemOption
            // 
            resources.ApplyResources(this.menuItemOption, "menuItemOption");
            this.menuItemOption.Index = 0;
            this.menuItemOption.Click += new System.EventHandler(this.menuItemOption_Click);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 1;
            // 
            // menuItem2
            // 
            resources.ApplyResources(this.menuItem2, "menuItem2");
            this.menuItem2.Index = 2;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // m_list
            // 
            this.m_list.AccessibleDescription = null;
            this.m_list.AccessibleName = null;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.BackgroundImage = null;
            this.m_list.ContextMenu = this.contextMenu1;
            this.m_list.Font = null;
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
            this.m_list.OwnerDraw = true;
            this.m_list.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.m_list_DrawColumnHeader);
            this.m_list.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.m_list_DrawItem);
            this.m_list.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.m_list_DrawSubItem);
            //
            // imageListRowHeight (controls ListView row height)
            //
            this.imageListRowHeight = new System.Windows.Forms.ImageList(this.components);
            this.imageListRowHeight.ImageSize = new System.Drawing.Size(1, 20);
            this.m_list.SmallImageList = this.imageListRowHeight;
            //
            // labelStatus
            //
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelStatus.Text = "";
            this.labelStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 8F);
            //
            // buttonPrevPage
            //
            this.buttonPrevPage.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonPrevPage.Width = 30;
            this.buttonPrevPage.Text = "<";
            this.buttonPrevPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPrevPage.FlatAppearance.BorderSize = 0;
            this.buttonPrevPage.Name = "buttonPrevPage";
            this.buttonPrevPage.Click += new System.EventHandler(this.buttonPrevPage_Click);
            //
            // labelPageInfo
            //
            this.labelPageInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPageInfo.Text = "1/1";
            this.labelPageInfo.Name = "labelPageInfo";
            //
            // buttonNextPage
            //
            this.buttonNextPage.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonNextPage.Width = 30;
            this.buttonNextPage.Text = ">";
            this.buttonNextPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNextPage.FlatAppearance.BorderSize = 0;
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Click += new System.EventHandler(this.buttonNextPage_Click);
            //
            // panelPagination
            //
            this.panelPagination.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelPagination.Width = 120;
            this.panelPagination.Name = "panelPagination";
            this.panelPagination.Visible = false;
            this.panelPagination.Controls.Add(this.labelPageInfo);
            this.panelPagination.Controls.Add(this.buttonPrevPage);
            this.panelPagination.Controls.Add(this.buttonNextPage);
            //
            // panelStatus
            //
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Height = 24;
            this.panelStatus.BackColor = System.Drawing.SystemColors.Control;
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Controls.Add(this.labelStatus);
            this.panelStatus.Controls.Add(this.panelPagination);
            //
            // WndDatabase
            //
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.m_list);
            this.Controls.Add(this.panelStatus);
            this.Icon = null;
            this.Name = "WndDatabase";
            this.Load += new System.EventHandler(this.WndDatabase_Load);
            this.ResumeLayout(false);

		}
		#endregion

		ArrayList blockField = new ArrayList();
		public bool m_bUseNo;
		DataSet rstHistory = new DataSet();
		int nRecordCount;
		public bool bAutoUpdate;
		public bool bUseGrid;
		public bool bUseFullCursor;
		public int  nUpdateTime;
        public bool bReverseNo;     // reversed row numbering
		public int nRecordLimit;    // record limit (0=unlimited)
		public bool bUseAlternateRowColor;
		public bool bUsePagination;
		public int nPageSize = 100;
		int nCurrentPage = 1;
		int nTotalPages = 1;

		static readonly Color AlternateRowColor = Color.FromArgb(245, 248, 252);

		void UpdateRowHeight()
		{
			Font f = m_list.Font ?? this.Font;
			if(f != null && imageListRowHeight != null)
			{
				int h = (int)(f.GetHeight() + 6);
				if(h < 18) h = 18;
				imageListRowHeight.ImageSize = new Size(1, h);
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			UpdateRowHeight();
		}

		async Task DataSetToListViewAsync()
		{
			FIELD_VIEW_STRUCT view;
			HorizontalAlignment align;

			// Save scroll position before clearing
			int savedTopIndex = -1;
			if(m_list.TopItem != null)
				savedTopIndex = m_list.TopItem.Index;

			// Suppress all painting to prevent flicker
			SendMessage(m_list.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
			m_list.BeginUpdate();
			m_list.Columns.Clear();
			m_list.Items.Clear();

			for(int i = 0; i < blockField.Count; i++)
			{
				view = (FIELD_VIEW_STRUCT)blockField[i];
				if(view.active == 0)	continue;

				if(view.cAlign == 0)		align = HorizontalAlignment.Left;
				else if(view.cAlign == 1)	align = HorizontalAlignment.Center;
				else						align = HorizontalAlignment.Right;

				m_list.Columns.Add(view.DispName, view.width, align);
				if(rstHistory.Tables.Count > 0)
					view.nFieldPosOnDs = rstHistory.Tables[0].Columns.IndexOf(view.FieldName);
				else
					view.nFieldPosOnDs = -1;
			}

			if(rstHistory.Tables.Count > 0)
			{
				int totalRows = rstHistory.Tables[0].Rows.Count;

				int startRow = 0;
				int endRow = totalRows;

				int effectivePageSize = nPageSize > 0 ? nPageSize : 100;
				if(bUsePagination && totalRows > 0)
				{
					nTotalPages = (totalRows + effectivePageSize - 1) / effectivePageSize;
					if(nCurrentPage > nTotalPages) nCurrentPage = nTotalPages;
					if(nCurrentPage < 1) nCurrentPage = 1;
					startRow = (nCurrentPage - 1) * effectivePageSize;
					endRow = Math.Min(startRow + effectivePageSize, totalRows);
				}
				else
				{
					nTotalPages = 1;
					nCurrentPage = 1;
				}

				// Build ListViewItems on background thread to avoid UI hang
				var localBlockField = blockField;
				var localRstHistory = rstHistory;
				bool localReverseNo = bReverseNo;
				bool localAltColor = bUseAlternateRowColor;
				int localStartRow = startRow;
				int localEndRow = endRow;
				int localTotalRows = totalRows;

				ListViewItem[] items = await Task.Run(() =>
				{
					var result = new ListViewItem[localEndRow - localStartRow];
					for(int i = localStartRow; i < localEndRow; i++)
					{
						DataRow row = localRstHistory.Tables[0].Rows[i];
						ListViewItem item = new ListViewItem();
						int count = 0;

						for(int j = 0; j < localBlockField.Count; j++)
						{
							FIELD_VIEW_STRUCT v = (FIELD_VIEW_STRUCT)localBlockField[j];
							if(v.active == 0) continue;

							string data;
							if(v.FieldName == "_local_number_")
							{
								if(localReverseNo)
									data = (localTotalRows - i).ToString();
								else
									data = (i + 1).ToString();
							}
							else
							{
								data = v.nFieldPosOnDs == -1 ? "" : row[v.nFieldPosOnDs].ToString();
							}

							if(count == 0) item.Text = data;
							else item.SubItems.Add(data);
							count++;
						}

						if(localAltColor && (i - localStartRow) % 2 == 1)
							item.BackColor = AlternateRowColor;

						result[i - localStartRow] = item;
					}
					return result;
				});

				m_list.Items.AddRange(items);
			}
			else
			{
				nTotalPages = 1;
				nCurrentPage = 1;
			}

			m_list.EndUpdate();
			// Re-enable painting and force a single
			SendMessage(m_list.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);

			// Restore scroll position after populating
			if(savedTopIndex >= 0 && savedTopIndex < m_list.Items.Count)
			{
				m_list.TopItem = m_list.Items[savedTopIndex];
			}

			m_list.Invalidate();
			UpdateStatusBar();
		}

		void UpdateStatusBar()
		{
			if(labelStatus != null)
			{
				string status = String.Format("Records: {0}", nRecordCount);
				if(nRecordLimit > 0)
					status += String.Format(" (Limit: {0})", nRecordLimit);
				if(bMdbOrString == 2)
					status += " [Default DB]";
				else if(bMdbOrString == 1)
					status += " [DSN]";
				else
					status += " [MDB]";
				labelStatus.Text = status;
			}

			if(panelPagination != null)
			{
				panelPagination.Visible = bUsePagination;
				if(bUsePagination)
				{
					labelPageInfo.Text = String.Format("{0}/{1}", nCurrentPage, nTotalPages);
					buttonPrevPage.Enabled = (nCurrentPage > 1);
					buttonNextPage.Enabled = (nCurrentPage < nTotalPages);
				}
			}
		}

		private SemaphoreSlim _openDataBaseSemaphore = new SemaphoreSlim(1, 1);	
        async Task OpenDataBase()
		{
			if(!await  _openDataBaseSemaphore.WaitAsync(0))
				return;

			try
			{
				blockField.Clear();
				FIELD_VIEW_STRUCT view;

				if (m_bUseNo)
				{
					view = new FIELD_VIEW_STRUCT();
					view.FieldName = "_local_number_";
					view.DispName = "NO";
					view.active = 1;
					view.width = 100;
					blockField.Add(view);
				}

				await Open();

				if (rstHistory.Tables.Count == 0)
				{
					nRecordCount = 0;
					for (int i = 0; i < 3; i++)
					{
						view = new FIELD_VIEW_STRUCT();
						view.FieldName = String.Format("Field{0}", i + 1);
						view.DispName = view.FieldName;
						view.active = 1;
						view.width = 100;
						blockField.Add(view);
					}
				}
				else
				{

					nRecordCount = rstHistory.Tables[0].Rows.Count;

					//ClassAdoTableDef dbTable(m_db);

					int count = rstHistory.Tables[0].Columns.Count;
					DataColumn col;

					for (int i = 0; i < count; i++)
					{
						view = new FIELD_VIEW_STRUCT();
						col = rstHistory.Tables[0].Columns[i];
						view.FieldName = col.ColumnName;
						view.DispName = view.FieldName;
						view.active = 1;
						view.width = 100;
						blockField.Add(view);
					}
				}

				LoadClassConfig();

				await DataSetToListViewAsync();
			}
            finally
            {
                _openDataBaseSemaphore.Release();
            }
        }

		public string sFileName;
		public string sTableName;
        public string sSelectText;
        bool bTableOrSelect = false;
		public string sDsnName;
		public int bMdbOrString = 0;
		string sSqlTextWhere;
		string sSqlTextOrderBy;

		string BuildQuery()
		{
			string query;

			if (bTableOrSelect == true)
			{
				query = sSelectText;
			}
			else
			{
				// MDB/Access uses TOP for record limit
				if (bMdbOrString == 0 && nRecordLimit > 0)
					query = String.Format("SELECT TOP {0} * FROM {1}", nRecordLimit, sTableName);
				else
					query = String.Format("SELECT * FROM {0}", sTableName);

				if (sSqlTextWhere != null && sSqlTextWhere.Length != 0)
				{
					query += String.Format(" WHERE {0}", sSqlTextWhere);
				}
				if (sSqlTextOrderBy != null && sSqlTextOrderBy.Length != 0)
				{
					query += String.Format(" ORDER BY {0}", sSqlTextOrderBy);
				}

				// PostgreSQL/DSN uses LIMIT
				if (bMdbOrString != 0 && nRecordLimit > 0)
				{
					query += String.Format(" LIMIT {0}", nRecordLimit);
				}
			}

			return query;
		}

		async Task Open()
		{
			rstHistory = new DataSet();

			string query = BuildQuery();
			string error = null;

			// DB I/O to ThreadPool
			await Task.Run(async () =>
			{
				if(bMdbOrString == 0)
				{
					DataGate gate = new DataGate();
					(rstHistory, error) = await gate.GetDataSetFromMdb(sFileName, query);
				}
				else if(bMdbOrString == 2)
				{
					// Default DB (PostgreSQL)
					if(ConfigVarTotal.bLocalFlag)
					{
						(rstHistory, error) = await OpenPostgresAsync(query);
					}
					else
					{
						// Web client: relay through DataGate server
						DataGate gate = new DataGate();
						(rstHistory, error) = await gate.GetDataSetFromDsn("__defaultdb__", query);
					}
				}
				else
				{
					DataGate gate = new DataGate();
					(rstHistory, error) = await gate.GetDataSetFromDsn(sDsnName, query);
				}
			});

			if(rstHistory == null)
			{
				if(bMdbOrString == 0)
					MessageDisplay.Show(String.Format("Filename={0}\n{1}", sFileName, error));
				else if(bMdbOrString == 2)
					MessageDisplay.Show(String.Format("DefaultDB Error\n{0}", error));
				else
					MessageDisplay.Show(String.Format("Dsn={0}\n{1}", sDsnName, error));
				rstHistory = new DataSet();
			}
		}

		async Task<(DataSet, string)> OpenPostgresAsync(string query)
		{
			string connStr = ConfigDataDB.sConnectionString;
			if (string.IsNullOrEmpty(connStr))
			{
				return (null, "Default DB connection string is not configured.");
			}

			// Web client: replace localhost with server address
			if (!ConfigVarTotal.bLocalFlag
				&& !string.IsNullOrEmpty(ConfigVarTotal.sSiteRootName)
				&& connStr.IndexOf("Host=localhost", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				connStr = System.Text.RegularExpressions.Regex.Replace(
					connStr, "Host=localhost", "Host=" + ConfigVarTotal.sSiteRootName,
					System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			}

			DataSet ds = new DataSet();
			try
			{
				using (var conn = new NpgsqlConnection(connStr))
				{
					await conn.OpenAsync();
					using (var adapter = new NpgsqlDataAdapter(query, conn))
					{
						adapter.Fill(ds, "TableTest");
					}
				}
			}
			catch (Exception ex)
			{
				return (null, String.Format("PostgreSQL Error\nQuery={0}\n{1}", query, ex.Message));
			}

			return (ds, "");
		}

		string sClassName;

		public void SetClassName(string class_name) 
		{ 
			sClassName = class_name; 
		}

		bool GetKeepCurrentConfig()
		{
			string filename = String.Format("{0}\\Config\\Database\\Member\\{1}.inix", TotalConfig.sDirWorkProject, sClassName);

			int val = Profile.GetPrivateProfileIntW("Config", "KeepCurrent", 0, filename);

			return (val == 1);
		}

		void SetKeepCurrentConfig(bool val)
		{
            string filename = String.Format("{0}\\Config\\Database\\Member\\{1}.inix", TotalConfig.sDirWorkProject, sClassName);

			Profile.WritePrivateProfileIntW("Config", "KeepCurrent", val ? 1 : 0, filename);
		}

        bool GetUseWebConfig()
        {
            string filename = String.Format("{0}\\Config\\Database\\Member\\{1}.inix", TotalConfig.sDirWorkProject, sClassName);

            int val = Profile.GetPrivateProfileIntW("Config", "UseWebConfig", 0, filename);

            return (val == 1);
        }

        void SetUseWebConfig(bool val)
        {
            string filename = String.Format("{0}\\Config\\Database\\Member\\{1}.inix", TotalConfig.sDirWorkProject, sClassName);

            Profile.WritePrivateProfileIntW("Config", "UseWebConfig", val ? 1 : 0, filename);
        }

		void SaveClassConfigColumn()
		{
			ListWidthToConfig();

            string path = TotalConfig.sDirWorkProject + "\\Config\\Database\\Member";

			Directory.CreateDirectory(path);

			string filename = path+"\\"+sClassName;

			FIELD_VIEW_STRUCT view;
			int i;

			FileStream s = File.Open(filename, FileMode.Create);
			TextWriter writer = new StreamWriter(s);

			for(i = 0; i < blockField.Count; i++) 
			{
				view = (FIELD_VIEW_STRUCT)blockField[i];
				writer.WriteLine("{0},{1},{2},{3},{4},", view.FieldName, view.DispName, view.active, view.width, view.cAlign);
			}
			writer.Close();
		}

		// �ܺο��� ���
		public void SaveClassConfig()
		{
			if(GetKeepCurrentConfig())	return;		// ������ ������ �����ؾ� �ϹǷ� �������� �ʴ´�.
			SaveClassConfigColumn();
		}

		void ListWidthToConfig()
		{
			FIELD_VIEW_STRUCT view;
			string column;
			for(int j = 0; j < m_list.Columns.Count; j++) 
			{
				column = m_list.Columns[j].Text;	

				for(int i = 0; i < blockField.Count; i++) 
				{
					view = (FIELD_VIEW_STRUCT)blockField[i];
					if(view.DispName != column)	continue; 
					view.width = m_list.Columns[j].Width;
					break;
				}
			}
		}

		void LoadClassConfig()
		{
            string filename;

            if (GetUseWebConfig())
            {
                string path = MakeFilePath.Project("Config\\Database\\Member", sClassName);
                filename = path;
            }
            else
            {
                string path = TotalConfig.sDirWorkProject + "\\Config\\Database\\Member";
                filename = path + "\\" + sClassName;
            }

			if(!File.Exists(filename))	return;

			string one_line;
			string column="";
			int i;
			FIELD_VIEW_STRUCT view;

			FileStream s = File.OpenRead(filename);
			TextReader reader = new StreamReader(s);
			CommaBlockString comma = new CommaBlockString();

			while(true)
			{
				one_line = reader.ReadLine();

				if(one_line == null)	break;
				comma.Set(one_line);
				comma.GetString(ref column);

				for(i = 0; i < blockField.Count; i++) 
				{
					view = (FIELD_VIEW_STRUCT)blockField[i];
					if(view.FieldName != column)	continue; 

					comma.GetString(ref view.DispName);
					comma.GetChar(ref view.active);
					comma.GetInt(ref view.width);
					comma.GetChar(ref view.cAlign);
					break;
				}

			}

			reader.Close();

			bool active_flag = false;

			for(i = 0; i < (int)blockField.Count; i++) 
			{
				view = (FIELD_VIEW_STRUCT)blockField[i];
				if(view.active == 1) 
				{
					active_flag = true;
					break;
				}
			}

			// ��� �ʵ尡 Ȱ��ȭ�� �ƴҶ��� ��� Ȱ��ȭ��Ų��.
			if(active_flag == false) 
			{
				for(i = 0; i < blockField.Count; i++) 
				{
					view = (FIELD_VIEW_STRUCT)blockField[i];
					view.active = 1;
				}
			}
		}

		public void SetSqlText(string where, string orderby)
		{
            bTableOrSelect = false;
			sSqlTextWhere = where;
			sSqlTextOrderBy = orderby;
		}

		private async void WndDatabase_Load(object sender, System.EventArgs e)
		{
			m_list.FullRowSelect = bUseFullCursor;
			m_list.GridLines = bUseGrid;
			m_list.Dock = DockStyle.Fill;

			// Enable double buffering to reduce flicker
			typeof(ListView).GetProperty("DoubleBuffered",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
				?.SetValue(m_list, true, null);

			await OpenDataBase();

			timer1.Enabled = true;
		}

		private void menuItemDeleteOne_Click(object sender, System.EventArgs e)
		{
		
		}

		private void menuItemDeleteAll_Click(object sender, System.EventArgs e)
		{
		
		}

		private async void menuItemOption_Click(object sender, System.EventArgs e)
		{
			FormDatabaseOption dialog = new FormDatabaseOption();

			this.ListWidthToConfig();

			dialog.blockTemp = (ArrayList)Tools.CopyObject(blockField);

			dialog.checkBoxKeepCurrentSet.Checked = this.GetKeepCurrentConfig();
            dialog.checkBoxUseWebConfig.Checked = this.GetUseWebConfig();
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				FIELD_VIEW_STRUCT source;
				FIELD_VIEW_STRUCT target;
				for(int i = 0; i < blockField.Count; i++)
				{
					target = (FIELD_VIEW_STRUCT)blockField[i];
					source = (FIELD_VIEW_STRUCT)dialog.blockTemp[i];

					target.active = source.active;
					target.cAlign = source.cAlign;
					target.DispName = source.DispName;
				}

				this.SetKeepCurrentConfig(dialog.checkBoxKeepCurrentSet.Checked);
                this.SetUseWebConfig(dialog.checkBoxUseWebConfig.Checked);
				await DataSetToListViewAsync();
				SaveClassConfigColumn();
			}
		}

		bool bWorkingFlag = false;

		bool _isReloading = false;

		private async void timer1_Tick(object sender, System.EventArgs e)
		{
			if(bWorkingFlag)	return;
			if(!bAutoUpdate)	return;
			if(_isReloading)	return;

			DateTime t = DateTime.Now;

			if(t.Second == nOldSec)	return;

			if(t.Second > nOldSec)
			{
				nUpdateTimeCurrent += (t.Second-nOldSec);
			}
			else
			{
				nUpdateTimeCurrent += (t.Second+60-nOldSec);
			}

			nOldSec = t.Second;

			if(nUpdateTimeCurrent < nUpdateTime)
			{
				return;
			}

			nUpdateTimeCurrent = 0;

			_isReloading = true;
			try
			{
				await ReLoad();
			}
			finally
			{
				_isReloading = false;
			}
		}

		public async Task ReLoad()
		{
			// TODO: Add your message handler code here and/or call default
			await OpenDataBase();
		}

		public int GetCurSel()
		{
			if(m_list.SelectedItems.Count == 0)	return -1;
			int listIndex = m_list.SelectedItems[0].Index;
			if(bUsePagination)
			{
				int effectivePageSize = nPageSize > 0 ? nPageSize : 100;
				return (nCurrentPage - 1) * effectivePageSize + listIndex;
			}
			return listIndex;
		}

		public void SetCurSel(int index)
		{
			int listIndex = index;
			if(bUsePagination)
			{
				int effectivePageSize = nPageSize > 0 ? nPageSize : 100;
				listIndex = index - (nCurrentPage - 1) * effectivePageSize;
				if(listIndex < 0 || listIndex >= m_list.Items.Count) return;
			}
			if(listIndex >= m_list.Items.Count) return;
			m_list.Items[listIndex].Selected = true;
		}

		public string GetValue(int index, string col_name)
		{
			if(index == -1) return "";
			if(index >= rstHistory.Tables[0].Rows.Count)	return "";

			DataRow row = rstHistory.Tables[0].Rows[index];

            string retn;

            try
            {
                retn = row[col_name].ToString();
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("DatabaseGetValue Error={0}", exception.Message);
                retn = "";
            }

            return retn;
		}

		public async Task SetConnection(string connection, string table)
		{
            sDsnName = connection;
			sFileName = connection;
			sTableName = table;
			await OpenDataBase();
		}

        public async Task  SetSelect(string select)
        {
            bTableOrSelect = true;
            sSelectText = select;
            await OpenDataBase();
        }

		public async Task SetTable(string table)
		{
            bTableOrSelect = false;
			sTableName = table;
			await OpenDataBase();
		}

		public async Task SetRecordLimit(int limit)
		{
			nRecordLimit = limit;
			await OpenDataBase();
		}

		public delegate Task DelegateSelChange(); // void �� Task�� ���� 250724 PSU
        public DelegateSelChange procSelChange = null;

		private async void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0)	return;

			if(procSelChange != null)
				await procSelChange();
		}

        public void SetTextColor(Color color)
        {
            this.m_list.ForeColor = color;
        }

        public void SetBackColor(Color color)
        {
            this.m_list.BackColor = color;
        }

		private async void buttonPrevPage_Click(object sender, EventArgs e)
		{
			if(nCurrentPage > 1)
			{
				nCurrentPage--;
				await DataSetToListViewAsync();
			}
		}

		private async void buttonNextPage_Click(object sender, EventArgs e)
		{
			if(nCurrentPage < nTotalPages)
			{
				nCurrentPage++;
				await DataSetToListViewAsync();
			}
		}

		// OwnerDraw event handlers for modern UI
		private void m_list_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			using(var headerBrush = new SolidBrush(Color.FromArgb(230, 236, 245)))
			{
				e.Graphics.FillRectangle(headerBrush, e.Bounds);
			}
			using(var textBrush = new SolidBrush(Color.FromArgb(50, 50, 50)))
			{
				var sf = new StringFormat();
				if(e.Header.TextAlign == HorizontalAlignment.Center)
					sf.Alignment = StringAlignment.Center;
				else if(e.Header.TextAlign == HorizontalAlignment.Right)
					sf.Alignment = StringAlignment.Far;
				else
					sf.Alignment = StringAlignment.Near;
				sf.LineAlignment = StringAlignment.Center;
				sf.FormatFlags = StringFormatFlags.NoWrap;

				var textRect = new RectangleF(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);
				e.Graphics.DrawString(e.Header.Text, m_list.Font, textBrush, textRect, sf);
			}
			// Bottom border
			using(var pen = new Pen(Color.FromArgb(200, 210, 225)))
			{
				e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
			}
		}

		private void m_list_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			e.DrawDefault = false;
		}

		private void m_list_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			// Background
			if(e.Item.Selected)
			{
				using(var brush = new SolidBrush(Color.FromArgb(51, 120, 210)))
					e.Graphics.FillRectangle(brush, e.Bounds);
			}
			else
			{
				using(var brush = new SolidBrush(e.Item.BackColor))
					e.Graphics.FillRectangle(brush, e.Bounds);
			}

			// Text
			Color textColor = e.Item.Selected ? Color.White : m_list.ForeColor;
			using(var textBrush = new SolidBrush(textColor))
			{
				var sf = new StringFormat();
				if(e.Header != null)
				{
					if(e.Header.TextAlign == HorizontalAlignment.Center)
						sf.Alignment = StringAlignment.Center;
					else if(e.Header.TextAlign == HorizontalAlignment.Right)
						sf.Alignment = StringAlignment.Far;
					else
						sf.Alignment = StringAlignment.Near;
				}
				sf.LineAlignment = StringAlignment.Center;
				sf.FormatFlags = StringFormatFlags.NoWrap;
				sf.Trimming = StringTrimming.EllipsisCharacter;

				var textRect = new RectangleF(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);
				e.Graphics.DrawString(e.SubItem.Text, m_list.Font, textBrush, textRect, sf);
			}

			// Grid line
			if(m_list.GridLines)
			{
				using(var pen = new Pen(Color.FromArgb(230, 230, 230)))
				{
					e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
					e.Graphics.DrawLine(pen, e.Bounds.Right - 1, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Bottom);
				}
			}
		}

	}
}

