using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using NetTools.OldDefine;
using AutoLibLocal;
using AutoLib;
using System.Drawing.Printing;
using System.Data;
using System.Collections.Generic;
using NetTools.Hash;
using System.Threading.Tasks;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for FormAlarmEvent.
	/// </summary>
	public class FormAlarmEvent : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components; 

		public static ArrayList blockAlarmConfirmNot = new ArrayList();

        static int nUniqueIDPos = 0;

        public ThreadForm threadForm = new ThreadForm();

        public static int MakeUniqueID()
        {
            ALARM_CONFIRMATION_STRUCT item;

            lock (blockAlarmConfirmNot)
            {
                while (true)
                {
                next:
                    nUniqueIDPos++;

                    for (int i = 0; i < blockAlarmConfirmNot.Count; i++)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[i];
                        if (item.id == nUniqueIDPos) goto next;
                    }

                    return nUniqueIDPos;
                }
            }
        }

		private System.Windows.Forms.Panel panelMain;
		private System.Windows.Forms.MenuItem menuItemAlarmPrint;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItemConfirmAlarmSound;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItemConfirmOneAlarm;
        private MenuItem menuItemAlarmFont;
		public NetTools.ControlListView m_list = new ControlListView();
        public List<AlarmEventColumn> arrayColumns;

		public FormAlarmEvent()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.Load += async (sender, e) => await this.FormAlarmEvent_Load(sender, e);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAlarmEvent));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuAlarmConfirmAll = new System.Windows.Forms.ContextMenu();
            this.menuItemConfirmOneAlarm = new System.Windows.Forms.MenuItem();
            this.menuItemConfirmOnePage = new System.Windows.Forms.MenuItem();
            this.menuItemConfirmAllAlarm = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemConfirmAlarmSound = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItemDeleteAlarm = new System.Windows.Forms.MenuItem();
            this.menuItemDeleteAllAlarm = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItemDisableAlarm = new System.Windows.Forms.MenuItem();
            this.menuItemAlarmSound = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItemAlarmPrint = new System.Windows.Forms.MenuItem();
            this.menuItemAlarmFont = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemCancel = new System.Windows.Forms.MenuItem();
            this.panelMain = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuAlarmConfirmAll
            // 
            this.contextMenuAlarmConfirmAll.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemConfirmOneAlarm,
            this.menuItemConfirmOnePage,
            this.menuItemConfirmAllAlarm,
            this.menuItem4,
            this.menuItemConfirmAlarmSound,
            this.menuItem6,
            this.menuItemDeleteAlarm,
            this.menuItemDeleteAllAlarm,
            this.menuItem7,
            this.menuItemDisableAlarm,
            this.menuItemAlarmSound,
            this.menuItem11,
            this.menuItemAlarmPrint,
            this.menuItemAlarmFont,
            this.menuItem5,
            this.menuItemCancel});
            resources.ApplyResources(this.contextMenuAlarmConfirmAll, "contextMenuAlarmConfirmAll");
            this.contextMenuAlarmConfirmAll.Popup += new System.EventHandler(this.contextMenuAlarmConfirmAll_Popup);
            // 
            // menuItemConfirmOneAlarm
            // 
            resources.ApplyResources(this.menuItemConfirmOneAlarm, "menuItemConfirmOneAlarm");
            this.menuItemConfirmOneAlarm.Index = 0;
            this.menuItemConfirmOneAlarm.Click += new System.EventHandler(this.menuItemConfirmOneAlarm_Click);
            // 
            // menuItemConfirmOnePage
            // 
            resources.ApplyResources(this.menuItemConfirmOnePage, "menuItemConfirmOnePage");
            this.menuItemConfirmOnePage.Index = 1;
            this.menuItemConfirmOnePage.Click += new System.EventHandler(this.menuItemConfirmOnePage_Click);
            // 
            // menuItemConfirmAllAlarm
            // 
            resources.ApplyResources(this.menuItemConfirmAllAlarm, "menuItemConfirmAllAlarm");
            this.menuItemConfirmAllAlarm.Index = 2;
            this.menuItemConfirmAllAlarm.Click += new System.EventHandler(this.menuItemConfirmAllAlarm_Click);
            // 
            // menuItem4
            // 
            resources.ApplyResources(this.menuItem4, "menuItem4");
            this.menuItem4.Index = 3;
            // 
            // menuItemConfirmAlarmSound
            // 
            resources.ApplyResources(this.menuItemConfirmAlarmSound, "menuItemConfirmAlarmSound");
            this.menuItemConfirmAlarmSound.Index = 4;
            this.menuItemConfirmAlarmSound.Click += new System.EventHandler(this.menuItemConfirmAlarmSound_Click);
            // 
            // menuItem6
            // 
            resources.ApplyResources(this.menuItem6, "menuItem6");
            this.menuItem6.Index = 5;
            // 
            // menuItemDeleteAlarm
            // 
            resources.ApplyResources(this.menuItemDeleteAlarm, "menuItemDeleteAlarm");
            this.menuItemDeleteAlarm.Index = 6;
            this.menuItemDeleteAlarm.Click += new System.EventHandler(this.menuItemDeleteAlarm_Click);
            // 
            // menuItemDeleteAllAlarm
            // 
            resources.ApplyResources(this.menuItemDeleteAllAlarm, "menuItemDeleteAllAlarm");
            this.menuItemDeleteAllAlarm.Index = 7;
            this.menuItemDeleteAllAlarm.Click += new System.EventHandler(this.menuItemDeleteAllAlarm_Click);
            // 
            // menuItem7
            // 
            resources.ApplyResources(this.menuItem7, "menuItem7");
            this.menuItem7.Index = 8;
            // 
            // menuItemDisableAlarm
            // 
            resources.ApplyResources(this.menuItemDisableAlarm, "menuItemDisableAlarm");
            this.menuItemDisableAlarm.Index = 9;
            this.menuItemDisableAlarm.Click += new System.EventHandler(this.menuItemDisableAlarm_Click);
            // 
            // menuItemAlarmSound
            // 
            resources.ApplyResources(this.menuItemAlarmSound, "menuItemAlarmSound");
            this.menuItemAlarmSound.Index = 10;
            this.menuItemAlarmSound.Click += new System.EventHandler(this.menuItemAlarmSound_Click);
            // 
            // menuItem11
            // 
            resources.ApplyResources(this.menuItem11, "menuItem11");
            this.menuItem11.Index = 11;
            // 
            // menuItemAlarmPrint
            // 
            resources.ApplyResources(this.menuItemAlarmPrint, "menuItemAlarmPrint");
            this.menuItemAlarmPrint.Index = 12;
            this.menuItemAlarmPrint.Click += new System.EventHandler(this.menuItemAlarmPrint_Click);
            // 
            // menuItemAlarmFont
            // 
            resources.ApplyResources(this.menuItemAlarmFont, "menuItemAlarmFont");
            this.menuItemAlarmFont.Index = 13;
            this.menuItemAlarmFont.Click += new System.EventHandler(this.menuItemAlarmFont_Click);
            // 
            // menuItem5
            // 
            resources.ApplyResources(this.menuItem5, "menuItem5");
            this.menuItem5.Index = 14;
            // 
            // menuItemCancel
            // 
            resources.ApplyResources(this.menuItemCancel, "menuItemCancel");
            this.menuItemCancel.Index = 15;
            this.menuItemCancel.Click += new System.EventHandler(this.menuItemCancel_Click);
            // 
            // panelMain
            // 
            this.panelMain.AccessibleDescription = null;
            this.panelMain.AccessibleName = null;
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.BackgroundImage = null;
            this.panelMain.Font = null;
            this.panelMain.Name = "panelMain";
            // 
            // FormAlarmEvent
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = null;
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = null;
            this.Name = "FormAlarmEvent";
            // this.Load += new System.EventHandler(this.FormAlarmEvent_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormAlarmEvent_Paint);
            this.SizeChanged += new System.EventHandler(this.FormAlarmEvent_SizeChanged);
            this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.MenuItem menuItemConfirmOnePage;
		private System.Windows.Forms.MenuItem menuItemConfirmAllAlarm;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItemDeleteAlarm;
		private System.Windows.Forms.MenuItem menuItemDeleteAllAlarm;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItemDisableAlarm;
		private System.Windows.Forms.MenuItem menuItemAlarmSound;
		private System.Windows.Forms.MenuItem menuItemCancel;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.ContextMenu contextMenuAlarmConfirmAll;

		static ArrayList blockAlarmWnd = new ArrayList();

        public static int nEventAlarmTrans = 0; // 숫자가 바뀌면 이벤트가 발생했다는 것

		public static void AlarmConfirmCountChanged()
		{
            nEventAlarmTrans++;

			REGISTER_ALARM_STRUCT reg;
			int l;

			for(l = 0; l < blockAlarmWnd.Count; l++) 
			{
				reg = (REGISTER_ALARM_STRUCT)blockAlarmWnd[l];

				reg.wnd.ConfirmListChanged();

                reg.wnd.threadForm.Invalidate(reg.wnd, true);	// true=모든 차일드를 함께 무효화 시킨다.
			}

            SaveAlarmConfirmList(); // 상황이 바뀌면 이벤트 경보의 현재 상태를 저장한다.
		}

		WORK_ALARM_CURRENT_LIST workAlarm = new WORK_ALARM_CURRENT_LIST();

		public int Option1
		{
			set 
			{
				this.workAlarm.option1 = value;
			}
		}

        public int[] Option2
		{
			set 
			{
				this.workAlarm.option2 = value;
			}
		}

        bool bConfirmListChanged = false;

		public void ConfirmListChanged()
		{
			// AlarmFilterGo(workAlarm);    타이머에서 한다. 2016-3-31
            bConfirmListChanged = true;
		}

		ArrayList arrayIndex = new ArrayList();

        public DateTime tFilterAlarmFrom;  // 경보파일을 검색할 때 시작 시간
        public DateTime tFilterAlarmTo;    // 경보파일을 검색할 때 끝 시간

        ArrayList arrayFileAlarm = new ArrayList();

        // Option2 조건이 해당되면 인덱스에 추가한다. 
        void AddIndexIfInclude(WORK_ALARM_CURRENT_LIST work, int compare, int pos)
        {
            for (int j = 0; j < work.option2.Length; j++)
            {
                if (compare == work.option2[j])
                {
                    INDEX_STRUCT index = new INDEX_STRUCT();
                    index.pos = pos;
                    arrayIndex.Add(index);
                    break;  // 한번 포함되면 계속 검색할 필요가 없어서 빠져나간다. 2016-9-7. 같은 필터가 두번이상 등록되면 경보도 두번이상 추가되는 문제점이 있기도 하다.
                }
            }
        }

        bool IsOption2Include(WORK_ALARM_CURRENT_LIST work, int compare)
        {
            for (int j = 0; j < work.option2.Length; j++)
            {
                if (compare == work.option2[j])
                {
                    return true;
                }
            }

            return false;
        }
        
        // 이벤트 경보가 아닌 파일 경보를 말한다.
        async Task AlarmFilterGoFileAlarm(WORK_ALARM_CURRENT_LIST work)
        {
            this.m_list.listHap = 0;
            arrayIndex.Clear();
            arrayFileAlarm.Clear();

            DataGate gate = new DataGate();
            
            ALARM_CONFIRMATION_STRUCT item;
            DataRow row;
            int l;

            DataSet ds = await gate.GetAlarmFileByScript(tFilterAlarmFrom, tFilterAlarmTo, "", true);

            if (ds == null) return;

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                item = new ALARM_CONFIRMATION_STRUCT();
                row = ds.Tables[0].Rows[i];

                item.t.Set(ConvertTool.ToDateTime(row[0].ToString()));
                item.tag = row[1].ToString();
                item.description = row[2].ToString();
                item.message = row[3].ToString();
                if ((ushort)row[4] == 7 && ConfigViewMain.bShowUserManualControl)
                {
                    if (Tools.IsLangKorean()) 
                        item.message += String.Format(" 변경자=''{0}''", row[10].ToString());
                    else 
                        item.message += String.Format(" Changed by=''{0}''", row[10].ToString());
                }
                item.msg_type = ConvertTool.ToUInt16(row[4].ToString());
                item.priority = ConvertTool.ToUInt16(row[5].ToString());
                item.port = ConvertTool.ToInt32(row[6].ToString());
                // 7 station
                // 8 address
                item.msg_sub_type = ConvertTool.ToUInt16(row[9].ToString());
                item.bAlarm = 1;     // 0 이면 확인 시간 복귀시간이 표시된다.

                arrayFileAlarm.Add(item);
                //arrayIndex.Add(i);  // 의미없는 인덱스 이지만 필요해서 같은 위치를 추가해 준다.
            }

            if (ConfigAlarm.bAlarmConfirmSorting == true)
            {
                if (work.option1 == 1)
                {	// port별로 보기
                    for (l = arrayFileAlarm.Count - 1; l >= 0; l--)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];

                        AddIndexIfInclude(work, item.port, l);
                    }
                }
                else if (work.option1 == 2)
                {	// 우선 순위별로 보기
                    for (l = arrayFileAlarm.Count - 1; l >= 0; l--)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];

                        AddIndexIfInclude(work, item.priority, l);
                    }
                }
                else if (work.option1 == 3)
                {	// 메시지 종류별로 보기
                    for (l = arrayFileAlarm.Count - 1; l >= 0; l--)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];

                        AddIndexIfInclude(work, item.msg_type, l);
                    }
                }
                else if (work.option1 == 4)
                {	// 일반경보 중 메시지 종류별로 보기
                    for (l = arrayFileAlarm.Count - 1; l >= 0; l--)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];

                        if (item.msg_type != 11) continue;  // 일반경보가 아니면 해당없음

                        AddIndexIfInclude(work, item.msg_sub_type, l);
                    }
                }
                else if (work.option1 == 104)
                {	// 일반경보 중 메시지 종류별로 보기중 해당되지 않는 것만 
                    for (l = arrayFileAlarm.Count - 1; l >= 0; l--)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];

                        if (item.msg_type == 11 && IsOption2Include(work, item.msg_sub_type))
                        { // 일반경보이고 SUB경보가 일치하면 해당없음. 그외는 해당있음

                        }
                        else
                        {
                            INDEX_STRUCT index = new INDEX_STRUCT();
                            index.pos = l;
                            arrayIndex.Add(index);
                        }
                    }
                }
                else
                {	// 그 이외의 경우는 모두 다를 포함시킨다.
                    for (l = arrayFileAlarm.Count - 1; l >= 0; l--)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];
                        INDEX_STRUCT index = new INDEX_STRUCT();
                        index.pos = l;
                        arrayIndex.Add(index);
                    }
                }
            }
            else
            {
                if (work.option1 == 1)
                {	// port별로 보기
                    for (l = 0; l < arrayFileAlarm.Count; l++)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];

                        AddIndexIfInclude(work, item.port, l);
                    }
                }
                else if (work.option1 == 2)
                {	// 우선순위별로 보기
                    for (l = 0; l < arrayFileAlarm.Count; l++)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];
                        
                        AddIndexIfInclude(work, item.priority, l);
                    }
                }
                else if (work.option1 == 3)
                {	// 메시지 종류별로 보기
                    for (l = 0; l < arrayFileAlarm.Count; l++)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];

                        AddIndexIfInclude(work, item.msg_type, l);
                    }
                }
                else if (work.option1 == 4)
                {	// 일반경보 중 메시지 종류별로 보기
                    for (l = 0; l < arrayFileAlarm.Count; l++)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];

                        if (item.msg_type != 11) continue;  // 일반경보가 아니면 해당없음

                        AddIndexIfInclude(work, item.msg_sub_type, l);
                    }
                }
                else if (work.option1 == 104)
                {	// 일반경보 중 메시지 종류별로 보기중 해당되지 않는 것만 
                    for (l = 0; l < arrayFileAlarm.Count; l++)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];

                        if (item.msg_type == 11 && IsOption2Include(work, item.msg_sub_type))
                        { // 일반경보이고 SUB경보가 일치하면 해당없음. 그외는 해당있음

                        }
                        else
                        {
                            INDEX_STRUCT index = new INDEX_STRUCT();
                            index.pos = l;
                            arrayIndex.Add(index);
                        }
                    }
                }
                else
                {	// 그 이외의 경우는 모두 다를 포함시킨다.
                    for (l = 0; l < arrayFileAlarm.Count; l++)
                    {
                        item = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[l];
                        INDEX_STRUCT index = new INDEX_STRUCT();
                        index.pos = l;
                        arrayIndex.Add(index);
                    }
                }
            }

            this.m_list.listHap = arrayIndex.Count;
            this.m_list.listItemChanged();
        }

        bool CheckInclueMethod4(WORK_ALARM_CURRENT_LIST work, ALARM_CONFIRMATION_STRUCT list)
        {
            // 조건이 4일때 미확인 또는 발생중인 경보만 리스트한다.
            // true는 보여준다. false는 보여주지 않는다.
            if (work.cIncludeMethod != 4) return true;  
            if (list.bHandConfirm == 0) return true;    // 확인이 되지 않았으면 보여준다.

            if (list.bAlarmTag == 1) return true;
            return false;

            //EnumTagType type = 0;
            //int[] pos = new int[1];
            //bool bAlarm = false;

            //// int  GetTagTypeAndPos(int terminal, const char *tag, int &type, short &pos);
            //TagLib.GetTagTypeAndPos(list.tag, ref type, ref pos);
            //if (type == EnumTagType.AI)
            //{	// AI tag
            //    TagAiClass ai = TagLib.GetStructAI(list.tag, ref pos);
            //    bAlarm = ai.bCurrentAlarmStatus;
            //}
            //else if (type == EnumTagType.DI)
            //{	// DI tag
            //    TagDiClass di = TagLib.GetStructDI(list.tag, ref pos);
            //    bAlarm = di.bCurrentAlarmStatus;
            //}

            //// 확인이 된것중에서 경보중이면 보여주고 경보가 끝나면 보여주지 않는다.
            //if (bAlarm) return true;    

            //return false;
        }

        async Task AlarmFilterGo(WORK_ALARM_CURRENT_LIST work)
		{
            if (work.cIncludeMethod == 3)   // File Alarm
            {
                await AlarmFilterGoFileAlarm(work);
                return;
            }

			this.m_list.listHap = 0;
			arrayIndex.Clear();
            
			int l;
			ALARM_CONFIRMATION_STRUCT list;

			if(ConfigAlarm.bAlarmConfirmSorting == true) 
			{
				if(work.option1 == 1)	
				{	// port별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = blockAlarmConfirmNot.Count - 1; l >= 0; l--)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];
                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            AddIndexIfInclude(work, list.port, l);
                        }
                    }
				}
				else if(work.option1 == 2)	
				{	// 우선 순위별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = blockAlarmConfirmNot.Count - 1; l >= 0; l--)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];
                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            AddIndexIfInclude(work, list.priority, l);
                        }
                    }
				}
                else if (work.option1 == 3)
                {	// 메시지 종류별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = blockAlarmConfirmNot.Count - 1; l >= 0; l--)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];
                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            AddIndexIfInclude(work, list.msg_type, l);
                        }
                    }
                }
                else if (work.option1 == 4)
                {	// 일반경보 중 메시지 종류별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = blockAlarmConfirmNot.Count - 1; l >= 0; l--)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];

                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            if (list.msg_type != 11) continue;  // 일반경보가 아니면 해당없음

                            AddIndexIfInclude(work, list.msg_sub_type, l);
                        }
                    }
                }
                else if (work.option1 == 104)
                {	// 우선 순위별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = blockAlarmConfirmNot.Count - 1; l >= 0; l--)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];

                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            if (list.msg_type == 11 && IsOption2Include(work, list.msg_sub_type))
                            { // 일반경보이고 SUB경보가 일치하면 해당없음. 그외는 해당있음

                            }
                            else
                            {
                                INDEX_STRUCT index = new INDEX_STRUCT();
                                index.pos = l;
                                arrayIndex.Add(index);
                            }
                        }
                    }
                }
				else 
				{	// 그 이외의 경우는 모두 다를 포함시킨다.
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = blockAlarmConfirmNot.Count - 1; l >= 0; l--)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];
                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            INDEX_STRUCT index = new INDEX_STRUCT();
                            index.pos = l;
                            arrayIndex.Add(index);
                        }
                    }
				}
			}
			else 
			{
				if(work.option1 == 1)	
				{	// port별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = 0; l < blockAlarmConfirmNot.Count; l++)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];
                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            AddIndexIfInclude(work, list.port, l);
                        }
                    }
				}
				else if(work.option1 == 2)	
				{	// 우선순위별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = 0; l < blockAlarmConfirmNot.Count; l++)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];
                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            AddIndexIfInclude(work, list.priority, l);
                        }
                    }
				}
                else if (work.option1 == 3)
                {	// 메시지 종류별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = 0; l < blockAlarmConfirmNot.Count; l++)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];
                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            AddIndexIfInclude(work, list.msg_type, l);
                        }
                    }
                }
                else if (work.option1 == 4)
                {	// 일반경보 중 메시지 종류별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = 0; l < blockAlarmConfirmNot.Count; l++)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];

                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            if (list.msg_type != 11) continue;  // 일반경보가 아니면 해당없음

                            AddIndexIfInclude(work, list.msg_sub_type, l);
                        }
                    }
                }
                else if (work.option1 == 104)
                {	// 우선 순위별로 보기
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = 0; l < blockAlarmConfirmNot.Count; l++)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];

                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            if (list.msg_type == 11 && IsOption2Include(work, list.msg_sub_type))
                            { // 일반경보이고 SUB경보가 일치하면 해당없음. 그외는 해당있음

                            }
                            else
                            {
                                INDEX_STRUCT index = new INDEX_STRUCT();
                                index.pos = l;
                                arrayIndex.Add(index);
                            }
                        }
                    }
                }
				else 
				{	// 그 이외의 경우는 모두 다를 포함시킨다.
                    lock (blockAlarmConfirmNot)
                    {
                        for (l = 0; l < blockAlarmConfirmNot.Count; l++)
                        {
                            list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];
                            if (work.cIncludeMethod == 0 && list.bHandConfirm != 0) continue;
                            if (work.cIncludeMethod == 2 && list.bHandConfirm == 0) continue;
                            if (!CheckInclueMethod4(work, list)) continue;    // 미확인 또는 발생중인 경보만 리스트한다.

                            INDEX_STRUCT index = new INDEX_STRUCT();
                            index.pos = l;
                            arrayIndex.Add(index);
                        }
                    }
				}
			}

			this.m_list.listHap = arrayIndex.Count;
			this.m_list.listItemChanged();
		}

        List<AlarmEventColumn> pColumnPos;

		private async Task FormAlarmEvent_Load(object sender, System.EventArgs e)
		{
            if (arrayColumns == null) arrayColumns = AlarmEventColumn.Init();

            pColumnPos = AlarmEventColumn.SeekAlarmEventColumnPointer(arrayColumns);

			//this.panelMain.Dock = DockStyle.Fill;
			SizeChange();			

			m_list.bOwnerDraw = true;
			m_list.TopLevel = false;
			m_list.font = this.Font;

            if(ConfigAlarm.bEnableContextMenuOnAlarmEvent)
			    m_list.contextMenu = this.contextMenuAlarmConfirmAll;

            m_list.colorCursor = Color.FromArgb(40, Color.DarkGray);
            m_list.colorCursorFocus = Color.FromArgb(40, Color.LightSkyBlue);
            m_list.colorCursorBorder = Color.FromArgb(128, Color.LightBlue);

			m_list.HeaderStyle = this.bUseColumnHeader ? ColumnHeaderStyle.Nonclickable : ColumnHeaderStyle.None;
			
			//userList.doubleClick = new ControlListView.OnEventDoubleClick(OnDoubleClick);
			this.panelMain.Controls.Add(m_list);

			ControlListViewHeader header;

			StringFormat center = new StringFormat();
			center.Alignment = StringAlignment.Center;

            for (int i = 0; i < pColumnPos.Count; i++)
            {
                header = new ControlListViewHeader();

                header.text = pColumnPos[i].title;
                
                header.format = DrawClass.StringFormatLeft;
                header.width = 100;
                m_list.header.Add(header);
            }

            

			m_list.paintMessage = new ControlListView.OnEventPaintMessage(OnListPaint);

			m_list.Show();

			m_list.currPos = 0;

			TotalConfig.AutoBaseListCtrlConfigLoad(this.m_list, "AlarmEvent", this.Text);

			WORK_ALARM_CURRENT_LIST work = workAlarm;
	
			work.option1 = 0;
			work.option2 = new int[1];

			await AlarmFilterGo(work);	

			work.sUniName = this.Text;

			RegisterAlarmHWND(this, true);

			m_list.backColor = SharedData.alarmClass.colorAlarmBack;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.menuItemConfirmOnePage.Visible = false;
            }

            
		}

		public void SetFont(Font font)
		{
			this.Font = font;
			
			m_list.font = font;
			m_list.fontChanged();

            SizeChange();

			Invalidate(true);
		}

		protected void OnListPaint(System.Drawing.Graphics g, Rectangle r)
		{
			if(r.Bottom < m_list.headHeight) return;

			if(r.Top >= m_list.headHeight) 
				DrawClass.gcls(g, r, m_list.backColor);
			else				    
				DrawClass.gcls(g, 0, m_list.headHeight, Width, r.Bottom, m_list.backColor);

			if(m_list.pageLineCount <= 0 || m_list.fontY <= 0 || m_list.listHap <= 0) return;
			
			int pos, x, y = m_list.headHeight, xGap = (int)(m_list.fontX*0.25), endPos;

			endPos = m_list.pageLineCount+m_list.startPos+1;		// 1줄 더 그린다

			if(endPos > m_list.listHap) endPos = m_list.listHap;

			WORK_ALARM_CURRENT_LIST work = workAlarm;
			ALARM_CONFIRMATION_STRUCT list;
			
			for(pos = m_list.startPos; pos < endPos; pos++, y += m_list.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+m_list.fontY < r.Top) continue;
				
				WorkGetBlock(work, out list, pos);

				x = m_list.startX;
				oneLineDraw(g, list, x, y, xGap, false);
			}
		}

		void oneLineDraw(Graphics g, ALARM_CONFIRMATION_STRUCT list, int x, int y, int xGap, bool bPrint)
		{
			Color					color;
			ControlListViewHeader	head;
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			string buf;
			RECT r = new RECT();
			WORK_ALARM_CURRENT_LIST work = workAlarm;

			if(bPrint)	
				color = Color.Black;
			else 
			{
                color = SharedData.alarmClass.GetAlarmColor(list.msg_type, list.priority);
			}

			for(int i = 0; i < m_list.header.Count; i++) 
			{
				head = (ControlListViewHeader)m_list.header[i];
	
				if(pColumnPos[i].name == "Priority")		
                    buf = String.Format("{0:000}", list.priority);
                else if (pColumnPos[i].name == "AlarmDate") 
                    buf = String.Format("{0:0000}-{1:00}-{2:00}", list.t.wYear, list.t.wMonth, list.t.wDay);
                else if (pColumnPos[i].name == "AlarmTime") 
                    buf = String.Format("{0:00}:{1:00}:{2:00}", list.t.wHour, list.t.wMinute, list.t.wSecond);
                else if (pColumnPos[i].name == "Tag")
                {
                    buf = list.tag;
                }
                else if (pColumnPos[i].name == "Description")
                    buf = list.description;
                else if (pColumnPos[i].name == "Message")
                {
                    if (list.bAlarm == 1)
                        buf = list.message;
                    else
                    {
                        if (list.bConfirmMethod == 1)
                        {
                            if (Tools.IsLangKorean())
                            {
                                buf = String.Format("{0} 확인시간-{1:0000}/{2:00}/{3:00} {4:00}:{5:00}:{6:00}", list.message,
                                    list.tReturn.wYear, list.tReturn.wMonth, list.tReturn.wDay,
                                    list.tReturn.wHour, list.tReturn.wMinute, list.tReturn.wSecond);
                            }
                            else if (Tools.IsLangJapanese())
                            {
                                buf = String.Format("{0} 確認時間-{1:0000}/{2:00}/{3:00} {4:00}:{5:00}:{6:00}", list.message,
                                    list.tReturn.wYear, list.tReturn.wMonth, list.tReturn.wDay,
                                    list.tReturn.wHour, list.tReturn.wMinute, list.tReturn.wSecond);
                            }
                            else if (Tools.IsLangChinese())
                            {
                                buf = String.Format("{0} 检查时间-{1:0000}/{2:00}/{3:00} {4:00}:{5:00}:{6:00}", list.message,
                                    list.tReturn.wYear, list.tReturn.wMonth, list.tReturn.wDay,
                                    list.tReturn.wHour, list.tReturn.wMinute, list.tReturn.wSecond);
                            }
                            else
                            {
                                buf = String.Format("{0} Confirmation-{1:0000}/{2:00}/{3:00} {4:00}:{5:00}:{6:00}", list.message,
                                    list.tReturn.wYear, list.tReturn.wMonth, list.tReturn.wDay,
                                    list.tReturn.wHour, list.tReturn.wMinute, list.tReturn.wSecond);
                            }
                        }
                        else
                        {
                            if (Tools.IsLangKorean())
                            {
                                buf = String.Format("{0} 복귀시간-{1:0000}/{2:00}/{3:00} {4:00}:{5:00}:{6:00}", list.message,
                                    list.tReturn.wYear, list.tReturn.wMonth, list.tReturn.wDay,
                                    list.tReturn.wHour, list.tReturn.wMinute, list.tReturn.wSecond);
                            }
                            else if (Tools.IsLangJapanese())
                            {
                                buf = String.Format("{0} 復帰時間-{1:0000}/{2:00}/{3:00} {4:00}:{5:00}:{6:00}", list.message,
                                    list.tReturn.wYear, list.tReturn.wMonth, list.tReturn.wDay,
                                    list.tReturn.wHour, list.tReturn.wMinute, list.tReturn.wSecond);
                            }
                            else if (Tools.IsLangChinese())
                            {   // 回复时间 -> 恢复时间 으로 변경 - 2019-06-21
                                buf = String.Format("{0} 恢复时间-{1:0000}/{2:00}/{3:00} {4:00}:{5:00}:{6:00}", list.message,
                                    list.tReturn.wYear, list.tReturn.wMonth, list.tReturn.wDay,
                                    list.tReturn.wHour, list.tReturn.wMinute, list.tReturn.wSecond);
                            }
                            else
                            {
                                buf = String.Format("{0} Return Time-{1:0000}/{2:00}/{3:00} {4:00}:{5:00}:{6:00}", list.message,
                                    list.tReturn.wYear, list.tReturn.wMonth, list.tReturn.wDay,
                                    list.tReturn.wHour, list.tReturn.wMinute, list.tReturn.wSecond);
                            }
                        }
                    }
                }
                else buf = "Error";

				if(!bPrint) 
				{
					if(i == 0)  // 태그3부터 깜박이다가 사용자가 선택 가능한 컬럼으로 빠뀌면서 모두 깜박인다. 2012-10-23
					{
						if(list.bAlarm == 1 && work.cIncludeMethod != 3)    // 파일경보가 아닐때만 깜박임을 사용한다.
						{	// 현재 Alarm 진행중이다.
							if(work.bBlinkingStatus == 1) 
							{
								//SetTextColor(blb.hdc, color);
							}
							else 
							{
								int cr = color.R/2;
								int cg = color.G/2;
								int cb = color.B/2;

								//SetTextColor(blb.hdc, RGB(r, g, b));
								color = Color.FromArgb(cr,cg,cb);
							}
						}
						else 
						{

						}
					}
				}

				DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)m_list.fontY, buf, color, m_list.backColor, m_list.font, head.format);

				x += head.width;
			}
		}

		void RegisterAlarmHWND(FormAlarmEvent wnd, bool flag)
		{
			REGISTER_ALARM_STRUCT reg = new REGISTER_ALARM_STRUCT();

			reg.wnd = wnd;

			if(flag)	blockAlarmWnd.Add(reg);
			else 
			{
				int l;

				for(l = 0; l < blockAlarmWnd.Count; l++) 
				{
					reg = (REGISTER_ALARM_STRUCT)blockAlarmWnd[l];
					if(reg.wnd == wnd) 
					{
						blockAlarmWnd.RemoveAt(l);
						return;
					}
				}
			}
		}

        private bool isTimerProcessing = false;

        private async void timer1_Tick(object sender, System.EventArgs e)
        {
            // 중복실행 방지
            if (isTimerProcessing)
                return;

            try
            {
                isTimerProcessing = true;

                WORK_ALARM_CURRENT_LIST work = workAlarm;

                ALARM_CONFIRMATION_STRUCT list;

                work.bBlinkingStatus = work.bBlinkingStatus == 1 ? (sbyte)0 : (sbyte)1;

                int l;

                for (l = 0; l < this.arrayIndex.Count; l++)
                {
                    WorkGetBlock(work, out list, l);

                    if (list.bAlarm == 1)
                    {
                        //InvalidateItem(l, 3, 5);
                        this.m_list.Invalidate();
                    }
                }

                if (bConfirmListChanged)
                {
                    await AlarmFilterGo(workAlarm);    //타이머에서 한다.
                    bConfirmListChanged = false;
                    this.Invalidate();          // 갯수가 변경되었을 경우 Count 표시가 바뀔 수 있도록 갱신해준다.
                }

                threadForm.OnTimer();
            }

            catch (Exception ex)
            {
                // 로그 기록 또는 예외 처리
                System.Diagnostics.Debug.WriteLine($"Timer error: {ex.Message}");
            }
            finally
            {
                isTimerProcessing = false;
            }
        }

        void WorkGetBlock(WORK_ALARM_CURRENT_LIST work, out ALARM_CONFIRMATION_STRUCT list, int pos)
        {
            if (pos >= this.arrayIndex.Count)
            {
                list = new ALARM_CONFIRMATION_STRUCT();
                return;
            }

            if (work.cIncludeMethod == 3)   // File Alarm
            {
                INDEX_STRUCT index;

                index = (INDEX_STRUCT)arrayIndex[pos];
                list = (ALARM_CONFIRMATION_STRUCT)arrayFileAlarm[index.pos];
            }
            else
            {
                INDEX_STRUCT index;

                index = (INDEX_STRUCT)arrayIndex[pos];
                lock (blockAlarmConfirmNot)
                {
                    if (index.pos >= blockAlarmConfirmNot.Count)
                        list = new ALARM_CONFIRMATION_STRUCT();
                    else
                        list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[index.pos];
                }
            }
        }

		public void WorkConfirmOneBlock(int cursor)
		{
			WORK_ALARM_CURRENT_LIST work = this.workAlarm; 
			ALARM_CONFIRMATION_STRUCT list;
	
			if(cursor < 0 || cursor >= arrayIndex.Count)	return;	// range over

			WorkGetBlock(work, out list, cursor);
			ConfirmOne(list);
			AlarmConfirmCountChanged();
			AlarmConfirmToNetWork(list);
		}

        public static void ConfirmOneID(int id)
        {
            ALARM_CONFIRMATION_STRUCT list;

            lock (blockAlarmConfirmNot)
            {
                for (int i = 0; i < blockAlarmConfirmNot.Count; i++)
                {
                    list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[i];

                    if (list.id != id) continue;

                    ConfirmOne(list);
                    AlarmConfirmCountChanged();
                    AlarmConfirmToNetWork(list);

                    break;
                }
            }
        }

		static void ConfirmOne(ALARM_CONFIRMATION_STRUCT list)
		{
			if(list.bAlarm == 0) 
			{	// 이미 경보가 꺼졌다.
				list.bHandConfirm = 1;
				return;	
			}

			EnumTagType type=0;
			int[] pos = new int[1];

			// int  GetTagTypeAndPos(int terminal, const char *tag, int &type, short &pos);
			TagLib.GetTagTypeAndPos(list.tag, ref type, ref pos);
			if(type == EnumTagType.AI) 
			{	// AI tag
				TagAiClass ai = TagLib.GetStructAI(list.tag, ref pos);
				ai.bNeedAlarmConfirm = false;
			}
			else if(type == EnumTagType.DI) 
			{	// DI tag
				TagDiClass di = TagLib.GetStructDI(list.tag, ref pos);
				di.bNeedAlarmConfirm = false;
			}

			list.tReturn.GetLocalTime();

			list.bHandConfirm = 1;
			list.bAlarm = 0;
			list.bConfirmMethod = 1;	// 수동으로 확인 했다.
		}

		private void menuItemConfirmOneAlarm_Click(object sender, System.EventArgs e)
		{
			WorkConfirmOneBlock(this.m_list.currPos);
		}

		private void menuItemConfirmOnePage_Click(object sender, System.EventArgs e)
		{
			ALARM_CONFIRMATION_STRUCT list;
			int i, l;

			for(l = m_list.startPos, i = 0; l < this.arrayIndex.Count && i < m_list.pageLineCount; l++, i++) 
			{
				WorkGetBlock(this.workAlarm, out list, l);
				ConfirmOne(list);
				AlarmConfirmToNetWork(list);
			}
			TotalAlarmHwndInvalidateRect();
		}

		public void WorkConfirmAllBlock()
		{
			WORK_ALARM_CURRENT_LIST work = this.workAlarm;
			ALARM_CONFIRMATION_STRUCT list;
			int l;
	
			for(l = 0; l < arrayIndex.Count; l++) 
			{
				WorkGetBlock(work, out list, l);
				ConfirmOne(list);
				AlarmConfirmToNetWork(list);
			}
			AlarmConfirmCountChanged();
		}

		private void menuItemConfirmAllAlarm_Click(object sender, System.EventArgs e)
		{
			WorkConfirmAllBlock();
		}

        public static void DeleteOneID(int id)
        {
            ALARM_CONFIRMATION_STRUCT list;

            lock (blockAlarmConfirmNot)
            {
                for (int i = 0; i < blockAlarmConfirmNot.Count; i++)
                {
                    list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[i];

                    if (list.id != id) continue;

                    ConfirmOne(list);
                    blockAlarmConfirmNot.RemoveAt(i);
                    AlarmDeleteToNetWork(list);
                    AlarmConfirmCountChanged();

                    return;
                }
            }
        }

        public void WorkDeleteOneBlock(int cursor)
        {
            WORK_ALARM_CURRENT_LIST work = this.workAlarm;

            if (work.cIncludeMethod == 3) return;   // File Alarm

            ALARM_CONFIRMATION_STRUCT list = new ALARM_CONFIRMATION_STRUCT();

            if (cursor < 0 || cursor >= arrayIndex.Count)
            {
                return;
            }

            // 아래 두 줄은 삭제시 tag.bNeedAlarmConfirm 을 OFF 해주기 위해 필요하다.
            WorkGetBlock(work, out list, cursor);
            ConfirmOne(list);

            INDEX_STRUCT index;


            index = (INDEX_STRUCT)arrayIndex[cursor];
            lock (blockAlarmConfirmNot)
            {
                if(index.pos < blockAlarmConfirmNot.Count)
                    blockAlarmConfirmNot.RemoveAt(index.pos);   
            }


            AlarmDeleteToNetWork(list);
        }

		private void menuItemDeleteAlarm_Click(object sender, System.EventArgs e)
		{
			WorkDeleteOneBlock(m_list.currPos);
			AlarmConfirmCountChanged();
		}

		public void WorkDeleteAllBlock()
		{
			WORK_ALARM_CURRENT_LIST work = this.workAlarm;

            if (work.cIncludeMethod == 3) return;   // File Alarm

			int l, m;
			INDEX_STRUCT pos, pos2;
			ALARM_CONFIRMATION_STRUCT list;

			for(l = 0; l < arrayIndex.Count; l++) 
			{
				// 아래 두 줄은 삭제시 tag.bNeedAlarmConfirm 을 OFF 해 주기 위해 필요하다.
				WorkGetBlock(work, out list, l);
				ConfirmOne(list);
				AlarmDeleteToNetWork(list);
		
				pos = (INDEX_STRUCT)arrayIndex[l];

                lock (blockAlarmConfirmNot)
                {
                    if(pos.pos < blockAlarmConfirmNot.Count)
                        blockAlarmConfirmNot.RemoveAt(pos.pos); 
                }

				for(m = l+1; m < arrayIndex.Count; m++) 
				{
					pos2 = (INDEX_STRUCT)arrayIndex[m];	
					if(pos2.pos > pos.pos) 
					{
						pos2.pos--;
					}
				}
			}
		}

		private void menuItemDeleteAllAlarm_Click(object sender, System.EventArgs e)
		{
			WorkDeleteAllBlock();
			AlarmConfirmCountChanged();
		}

		private void menuItemDisableAlarm_Click(object sender, System.EventArgs e)
		{
			if(ConfigAlarm.bAlarmProtectAll)	ConfigAlarm.bAlarmProtectAll = false;
			else								ConfigAlarm.bAlarmProtectAll = true;
		}

		private void menuItemAlarmSound_Click(object sender, System.EventArgs e)
		{
			if(ConfigAlarm.bAlarmSoundFlag)	ConfigAlarm.bAlarmSoundFlag = false;
			else						    ConfigAlarm.bAlarmSoundFlag = true;
		}

		private void menuItemCancel_Click(object sender, System.EventArgs e)
		{
		
		}

		void TotalAlarmHwndInvalidateRect()
		{
			REGISTER_ALARM_STRUCT reg;
			int l;

			for(l = 0; l < blockAlarmWnd.Count; l++) 
			{
				reg = (REGISTER_ALARM_STRUCT)blockAlarmWnd[l];
				//reg.wnd.Invalidate();
                reg.wnd.threadForm.Invalidate(reg.wnd, false);	// true=모든 차일드를 함께 무효화 시킨다.
			}		
		}

		public static void TotalAlarmSetBackColor()
		{
			REGISTER_ALARM_STRUCT reg;
			int l;

			for(l = 0; l < blockAlarmWnd.Count; l++) 
			{
				reg = (REGISTER_ALARM_STRUCT)blockAlarmWnd[l];
				reg.wnd.m_list.backColor = SharedData.alarmClass.colorAlarmBack;
                
				//reg.wnd.Invalidate(true);
                reg.wnd.threadForm.Invalidate(reg.wnd, true);	// true=모든 차일드를 함께 무효화 시킨다.
			}		
		}

		static void AlarmDeleteToNetWork(ALARM_CONFIRMATION_STRUCT list)
		{
            if (ConfigVarTotal.bLocalFlag)
            {
                if (SystemStatusMemory.GetDI(SSMDI.bExchangeAlarmStatus) == 0) return;

                string buf;

                buf = String.Format("Tag={0},String={1},", list.tag, list.message);

                NetWorkProtocolSend send = new NetWorkProtocolSend();
                ushort trans = NetWorkProtocolSend.GetTransaction();

                send.MakeBlock(EnumNetworkCommand.ALARM_LIST_DELETE_ONE, trans, buf);
                LibComNetServer.SendEventProgramToNetwork(send.bufSend, send.nBufCount);
            }
            else
            {
                string command = "EventAlarmDelete";
                string argument = String.Format("{0},", list.id);

                DataGate.ExecuteCommand(command, argument);
            }
		}

		static void AlarmConfirmToNetWork(ALARM_CONFIRMATION_STRUCT list)
		{
            if (ConfigVarTotal.bLocalFlag)
            {
                if (SystemStatusMemory.GetDI(SSMDI.bExchangeAlarmStatus) == 0) return;

                string buf;

                buf = String.Format("Tag={0},String={1},", list.tag, list.message);

                NetWorkProtocolSend send = new NetWorkProtocolSend();
                ushort trans = NetWorkProtocolSend.GetTransaction();

                send.MakeBlock(EnumNetworkCommand.ALARM_LIST_CONFIRM_ONE, trans, buf);
                LibComNetServer.SendEventProgramToNetwork(send.bufSend, send.nBufCount);
            }
            else
            {
                string command = "EventAlarmConfirm";
                string argument = String.Format("{0},", list.id);

                DataGate.ExecuteCommand(command, argument);
            }
		}

		public void OnMyClosed()
		{
			this.timer1.Enabled = false;
			TotalConfig.AutoBaseListCtrlConfigSave(this.m_list, "AlarmEvent", this.Text);
			RegisterAlarmHWND(this, false);
		}

		private void contextMenuAlarmConfirmAll_Popup(object sender, System.EventArgs e)
		{
			this.menuItemDisableAlarm.Checked = ConfigAlarm.bAlarmProtectAll;
			this.menuItemAlarmSound.Checked = ConfigAlarm.bAlarmSoundFlag;

            bool flag;

            flag = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_CONFIG_ALARM);

			this.menuItemDisableAlarm.Enabled = flag;
			this.menuItemAlarmSound.Enabled = flag;

            flag = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_ALARM_CONFIRM);
			
			this.menuItemConfirmOneAlarm.Enabled = flag;
			this.menuItemConfirmOnePage.Enabled = flag;
			this.menuItemConfirmAllAlarm.Enabled = flag;

            flag = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_ALARM_EVENT_DELETE);
            
			this.menuItemDeleteAlarm.Enabled = flag;
			this.menuItemDeleteAllAlarm.Enabled = flag;
		}

		public int IncludeMethod
		{
			set
			{
				this.workAlarm.cIncludeMethod = value;
			}
		}

		public sbyte WindowCaption
		{
			set
			{
				this.workAlarm.bFlagWindowCaption = value;
			}
		}

		bool bUseColumnHeader = true;

		public sbyte UseColumnHeader
		{
			set
			{
				this.bUseColumnHeader = (value == 1);
			}
		}

		class INDEX_STRUCT
		{
			public int pos=0;
		}

		public static void AlarmListConfirmOneByNetWork(string tag, string message)
		{
			int l;
			bool change = false;
			ALARM_CONFIRMATION_STRUCT list;

            lock (blockAlarmConfirmNot)
            {
                for (l = 0; l < blockAlarmConfirmNot.Count; l++)
                {
                    list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];

                    if (String.Compare(tag, list.tag, true) == 0 && String.Compare(message, list.message, true) == 0)
                    {
                        ConfirmOne(list);
                        change = true;
                    }
                }
            }

			if(change) 
			{
				AlarmConfirmCountChanged();
			}
		}

		public static void AlarmListDeleteOneByNetWork(string tag, string message)
		{
			int l;
			bool change = false;
			ALARM_CONFIRMATION_STRUCT list;

            lock (blockAlarmConfirmNot)
            {
            next_go:

                for (l = 0; l < blockAlarmConfirmNot.Count; l++)
                {
                    list = (ALARM_CONFIRMATION_STRUCT)blockAlarmConfirmNot[l];

                    if (String.Compare(tag, list.tag, true) == 0 && String.Compare(message, list.message, true) == 0)
                    {
                        ConfirmOne(list);
                        blockAlarmConfirmNot.RemoveAt(l);
                        change = true;
                        goto next_go;
                    }
                }
            }

			if(change) 
			{
				AlarmConfirmCountChanged();
			}
		}

		public void SizeChange()
		{
			if(this.workAlarm.bFlagWindowCaption == 1) 
			{
                // int  height = this.Font.Height;
                int height = GetStableFontHeight(); //20250729 PSU 폰트계산 변경.

                this.panelMain.Left = 0;
				this.panelMain.Top = height;
				this.panelMain.Width = this.ClientRectangle.Width;
				this.panelMain.Height = this.ClientRectangle.Height-height;
			}
			else 
			{
				this.panelMain.Left = 0;
				this.panelMain.Top = 0;
				this.panelMain.Width = this.ClientRectangle.Width;
				this.panelMain.Height = this.ClientRectangle.Height;
			}
		}

		private void FormAlarmEvent_SizeChanged(object sender, System.EventArgs e)
		{
			SizeChange();		
		}

        private int GetStableFontHeight()
        {
            try
            {
                //  return (int)this.Font.GetHeight();
                if (m_list.font == null) return 0;
                return m_list.font.Height;  //251002 this.Font는 화면크기 조절 시 dispose 되어 캐시하거나 다른 객체의 font 사용.
            }
            catch
            {
                return 18;
            }

        }
        void DrawTitle(Graphics g)
		{
			if(this.workAlarm.bFlagWindowCaption != 1)	return;

			string buf;

            int height = GetStableFontHeight(); //this.Font.height;  20250729 PSU 폰트계산 변경. 닷넷4.8.1 에서 font.Height 가 오류가 나는 경우가 있음.

            DrawClass.gcls(g, 0, 0, this.ClientRectangle.Width, height, Color.FromArgb(0, 0, 0x80));

			if(Tools.IsLangKorean()) 
			{
				buf = String.Format("{0} [경보 개수-{1}]", this.workAlarm.sUniName, arrayIndex.Count);
			}
			else if(Tools.IsLangJapanese()) 
			{
                buf = String.Format("{0} [警報の個数-{1}]", this.workAlarm.sUniName, arrayIndex.Count);
			}
			else if(Tools.IsLangChinese()) 
			{
				buf = String.Format("{0} [警报数-{1}]", this.workAlarm.sUniName, arrayIndex.Count);
			}
			else 
			{
				buf = String.Format("{0} [Alarm Count-{1}]", this.workAlarm.sUniName, arrayIndex.Count);
			}
            //SafeException.SafeDrawString(g, buf, this.Font, Brushes.White, 10, 0);
            SafeException.SafeDrawString(g, buf, m_list.font, Brushes.White, 10, 0);
        }

		private void FormAlarmEvent_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawTitle(e.Graphics);
		}

		private void menuItemAlarmPrint_Click(object sender, System.EventArgs e)
		{
			currentListPrint();
		}

		void drawPrintPageHeader(PrintPageEventArgs ev, ref int y)
		{
			int			x = 10;
			
			y += m_list.font.Height + 6;
			Rectangle				r;
			ControlListViewHeader	head;

            for (int i = 0; i < m_list.header.Count; i++) 
			{
				head = (ControlListViewHeader)m_list.header[i];
				r = new Rectangle(x, y, head.width, m_list.font.Height);
                SafeException.SafeDrawString(ev.Graphics, head.text, m_list.font, Brushes.Black, r);
				x += head.width;
			}
			y += m_list.font.Height;
		}

		int		nPrintListPage, nPrintListPos;

		void drawPrintPage(object sender, PrintPageEventArgs ev)
		{
			if(this.m_list.listHap <= nPrintListPos) return;

			int						x, y = 40;
			//Rectangle				r;
			ALARM_CONFIRMATION_STRUCT data;
			//ControlListViewHeader	head;
			WORK_ALARM_CURRENT_LIST work = workAlarm;
			int xGap = (int)(m_list.fontX*0.25);

			drawPrintPageHeader(ev, ref y);
			for(int i = nPrintListPos; i < m_list.listHap; i++)
			{	
				WorkGetBlock(work, out data, i);

				if(data == null) continue;				

				x = 10;//m_list.startX;
				oneLineDraw(ev.Graphics, data, x, y, xGap, true);
				
				y += m_list.font.Height;
				if(y >= ev.Graphics.VisibleClipBounds.Height-m_list.font.Height)
				{
					nPrintListPos = i+1;
					nPrintListPage ++;
					ev.HasMorePages = true;
					return;
				}
			}
		}

		public void currentListPrint()
		{			
			PrinterSettings s = new PrinterSettings();
			PrintDialog dialog = new PrintDialog();
			dialog.PrinterSettings = s;
			PrintDocument pd = new PrintDocument();
			pd.PrintPage += new PrintPageEventHandler(drawPrintPage);
			dialog.Document = pd;
			if(dialog.ShowDialog(this) != DialogResult.OK) return;
			nPrintListPos = 0;			// 0 번 list 부터 인쇄
			nPrintListPage = 1;			// 1 페이지 부터

			dialog.Document.Print();
		}

		public delegate void DelegateConfirmAlarmSound();
		public static DelegateConfirmAlarmSound procConfirmAlarmSound = null;

		private void menuItemConfirmAlarmSound_Click(object sender, System.EventArgs e)
		{
			if(procConfirmAlarmSound != null)	procConfirmAlarmSound();
		}

        public delegate void DelegateSaveAlarmConfirmList();
        public static DelegateSaveAlarmConfirmList procSaveAlarmConfirmList = null;

        static void SaveAlarmConfirmList()
        {
            if (procSaveAlarmConfirmList != null) procSaveAlarmConfirmList();
        }

        public string GetValue(int row_pos, int column_pos)
        {
            ALARM_CONFIRMATION_STRUCT list;
            WORK_ALARM_CURRENT_LIST work = workAlarm;

            if (row_pos < 0 || row_pos >= this.arrayIndex.Count)    // Range over
            {
                return "";
            }

            WorkGetBlock(work, out list, row_pos);

            if (list == null) return "";

            if (column_pos < 0 || column_pos > 5) return "";

            string buf = "";

            if (column_pos == 0)        buf = String.Format("{0:000}", list.priority);
            else if (column_pos == 1)   buf = String.Format("{0:0000}-{1:00}-{2:00}", list.t.wYear, list.t.wMonth, list.t.wDay);
            else if (column_pos == 2)   buf = String.Format("{0:00}:{1:00}:{2:00}", list.t.wHour, list.t.wMinute, list.t.wSecond);
            else if (column_pos == 3)   buf = list.tag;
            else if (column_pos == 4)   buf = list.description;
            else if (column_pos == 5)
            {
                buf = list.message;
            }

            return buf;
        }

        private void menuItemAlarmFont_Click(object sender, EventArgs e)
        {
            FontDialog dialog = new FontDialog();

            dialog.Font = this.Font;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                SetFont(dialog.Font);

                // 기본으로 제공하는 윈도우일 경우는 폰트를 환경에 저장한다.
                if (this.Text == "DefaultAlarmEventWindow")
                {
                    ConfigAlarm.fontAlarmEvent = dialog.Font;
                    ConfigAlarm.SaveConfig();
                }
            }
        }
        
	}

	class REGISTER_ALARM_STRUCT
	{
		public FormAlarmEvent wnd = null;
	}

	class WORK_ALARM_CURRENT_LIST
	{
		public sbyte bBlinkingStatus=0;
		public int	 option1=0;
        public int[] option2 = new int[1];
		public string sUniName="";
		public int cIncludeMethod=0;	// 
		public sbyte bFlagWindowCaption=0;
	}

    [Serializable]
    public class AlarmEventColumn
    {
        public string name;
        public bool visible;
        public string title;

        static void AddOneColumn(List<AlarmEventColumn> arrayColumns, string name, bool visible, string title)
        {
            AlarmEventColumn item = new AlarmEventColumn();
            item.name = name;
            item.visible = visible;
            item.title = title;
            arrayColumns.Add(item);
        }

        public static List<AlarmEventColumn> Init()
        {
            List<AlarmEventColumn> arrayColumns = new List<AlarmEventColumn>();

            if (Tools.IsLangKorean())
            {
                AddOneColumn(arrayColumns, "Priority", true, "우선순위");
                AddOneColumn(arrayColumns, "AlarmDate", true, "경보날짜");
                AddOneColumn(arrayColumns, "AlarmTime", true, "경보시간");
                AddOneColumn(arrayColumns, "Tag", true, "태그");
                AddOneColumn(arrayColumns, "Description", true, "태그 설명");
                AddOneColumn(arrayColumns, "Message", true, "경보 내용");
            }
            else if (Tools.IsLangJapanese())
            {
                AddOneColumn(arrayColumns, "Priority", true, "レベル");
                AddOneColumn(arrayColumns, "AlarmDate", true, "警報日付");
                AddOneColumn(arrayColumns, "AlarmTime", true, "警報時刻");
                AddOneColumn(arrayColumns, "Tag", true, "タグ");
                AddOneColumn(arrayColumns, "Description", true, "タグ説明");
                AddOneColumn(arrayColumns, "Message", true, "警報內容");
            }
            else if (Tools.IsLangChinese())
            {
                AddOneColumn(arrayColumns, "Priority", true, "优先权");
                AddOneColumn(arrayColumns, "AlarmDate", true, "警报日期");
                AddOneColumn(arrayColumns, "AlarmTime", true, "警报时间");
                AddOneColumn(arrayColumns, "Tag", true, "标记");
                AddOneColumn(arrayColumns, "Description", true, "标记描述");
                AddOneColumn(arrayColumns, "Message", true, "警报内容");
            }
            else if (Tools.IsLangVietnamese())
            {
                AddOneColumn(arrayColumns, "Priority", true, "Mức");
                AddOneColumn(arrayColumns, "AlarmDate", true, "Ngày");
                AddOneColumn(arrayColumns, "AlarmTime", true, "Thời gian");
                AddOneColumn(arrayColumns, "Tag", true, "Tag");
                AddOneColumn(arrayColumns, "Description", true, "Mô tả");
                AddOneColumn(arrayColumns, "Message", true, "Message");
            }
            else
            {
                AddOneColumn(arrayColumns, "Priority", true, "Priority");
                AddOneColumn(arrayColumns, "AlarmDate", true, "Alarm Date");
                AddOneColumn(arrayColumns, "AlarmTime", true, "Alarm Time");
                AddOneColumn(arrayColumns, "Tag", true, "Tag");
                AddOneColumn(arrayColumns, "Description", true, "Description");
                AddOneColumn(arrayColumns, "Message", true, "Message");
            }

            return arrayColumns;
        }

        public static List<AlarmEventColumn> SeekAlarmEventColumnPointer(List<AlarmEventColumn> arrayColumns)
        {
            string[] column_name = { "Priority", "AlarmDate", "AlarmTime", "Tag", "Description", "Message" };
            List<AlarmEventColumn> column_pos = new List<AlarmEventColumn>();

            for (int i = 0; i < column_name.Length; i++)
            {
                for (int j = 0; j < arrayColumns.Count; j++)
                {
                    if (!arrayColumns[j].visible) continue;

                    if (column_name[i] == arrayColumns[j].name)
                    {
                        column_pos.Add(arrayColumns[j]);
                        break;
                    }
                }
            }

            return column_pos;
        }
    }

    public class ThreadForm
    {
        Form formInvalidate = null;
        bool bInvalidateChilderen = false;

        public void Invalidate(System.Windows.Forms.Form form, bool invalidateChilderen)
        {
            bInvalidateChilderen = invalidateChilderen;
            formInvalidate = form;
        }

        public void OnTimer()
        {
            if (formInvalidate != null)
            {
                formInvalidate.Invalidate(bInvalidateChilderen);
                formInvalidate = null;
            }
        }

        
    }
}

