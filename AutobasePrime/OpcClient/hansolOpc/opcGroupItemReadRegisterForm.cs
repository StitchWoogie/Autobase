using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Opc;
using Opc.Da;
using OpcClient.plcScanComm;
using NetTools;

namespace OpcClient
{
	/// <summary>
	/// Summary description for opcGroupItemReadRegisterForm.
	/// </summary>
	public class opcGroupItemReadRegisterForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView treeView_Opc;
		private System.Windows.Forms.Timer timer_Display;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenu contextMenu_Server;
		private System.Windows.Forms.MenuItem menuItem_AddServer;
		private System.Windows.Forms.MenuItem menuItem_DeleteServer;
		private System.Windows.Forms.MenuItem menuItem_ModifyServer;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem_AddGroup;
		private System.Windows.Forms.ContextMenu contextMenu_Group;
		private System.Windows.Forms.MenuItem menuItem_AddGroup2;
		private System.Windows.Forms.MenuItem menuItem_DeleteGroup;
		private System.Windows.Forms.MenuItem menuItem_ModifyGroup;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem_AddItem;
		private System.Windows.Forms.ContextMenu contextMenu_Item;
		private System.Windows.Forms.MenuItem menuItem_AddItem2;
		private System.Windows.Forms.MenuItem menuItem_DeleteItem;
		private System.Windows.Forms.MenuItem menuItem_ModifyItem;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem_Connect;
		private System.Windows.Forms.MenuItem menuItem_InsertServer;
		private System.Windows.Forms.ContextMenu contextMenu_Write;
		private System.Windows.Forms.MenuItem menuItem_itemWriteValue;
		private System.Windows.Forms.MenuItem menuItem_InsertGroup;
		private System.Windows.Forms.MenuItem menuItem_insertItem;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private int nDisplayServer = 0, nCurrDisplayServer = -1;
		private int nDisplayGroup = 0, nCurrDisplayGroup = -1;
		private	bool bServerDisplay = true;
		private	enum eTreePosType { SERVER = 0, GROUP, ITEM, ETC };

		long		old_mili_sec, remain_mili_sec = 0;
		
		int[]					headWidth = new int[5];
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Label label_status;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.ListView listView_errorOrEventMessage;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		NetTools.ControlListView list = new NetTools.ControlListView();

		public opcGroupItemReadRegisterForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//			
			this.panel4.Controls.Add(list);
			headWidth[0] = 250;			//그룹.아이템
			headWidth[1] = 200;			// 현재값
			headWidth[2] = 120;			// 자료형식
			headWidth[3] = 100;			// 읽기 상태
			headWidth[4] = 170;			// Time Stamp
			listBasicElementSetting();
			list.currPos = 0;
			
			this.timer_Display.Enabled = true;
			this.treeView_Opc.ImageList = Resources.Instance.ImageList;
		}

		void listBasicElementSetting()
		{
			list.font = new Font("굴림", 9);
			list.backColor = Color.White;
			list.textColor = Color.Black;
			list.contextMenu = this.contextMenu_Write;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(opcGroupItemReadRegisterForm));
            this.treeView_Opc = new System.Windows.Forms.TreeView();
            this.timer_Display = new System.Windows.Forms.Timer(this.components);
            this.contextMenu_Server = new System.Windows.Forms.ContextMenu();
            this.menuItem_AddServer = new System.Windows.Forms.MenuItem();
            this.menuItem_InsertServer = new System.Windows.Forms.MenuItem();
            this.menuItem_DeleteServer = new System.Windows.Forms.MenuItem();
            this.menuItem_ModifyServer = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem_AddGroup = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_Connect = new System.Windows.Forms.MenuItem();
            this.contextMenu_Group = new System.Windows.Forms.ContextMenu();
            this.menuItem_AddGroup2 = new System.Windows.Forms.MenuItem();
            this.menuItem_InsertGroup = new System.Windows.Forms.MenuItem();
            this.menuItem_DeleteGroup = new System.Windows.Forms.MenuItem();
            this.menuItem_ModifyGroup = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem_AddItem = new System.Windows.Forms.MenuItem();
            this.contextMenu_Item = new System.Windows.Forms.ContextMenu();
            this.menuItem_AddItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_insertItem = new System.Windows.Forms.MenuItem();
            this.menuItem_DeleteItem = new System.Windows.Forms.MenuItem();
            this.menuItem_ModifyItem = new System.Windows.Forms.MenuItem();
            this.contextMenu_Write = new System.Windows.Forms.ContextMenu();
            this.menuItem_itemWriteValue = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.listView_errorOrEventMessage = new System.Windows.Forms.ListView();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label_status = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView_Opc
            // 
            this.treeView_Opc.AccessibleDescription = null;
            this.treeView_Opc.AccessibleName = null;
            resources.ApplyResources(this.treeView_Opc, "treeView_Opc");
            this.treeView_Opc.BackgroundImage = null;
            this.treeView_Opc.Font = null;
            this.treeView_Opc.FullRowSelect = true;
            this.treeView_Opc.HideSelection = false;
            this.treeView_Opc.ItemHeight = 14;
            this.treeView_Opc.Name = "treeView_Opc";
            this.treeView_Opc.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Opc_AfterSelect);
            this.treeView_Opc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_Opc_MouseDown);
            // 
            // timer_Display
            // 
            this.timer_Display.Enabled = true;
            this.timer_Display.Interval = 50;
            this.timer_Display.Tick += new System.EventHandler(this.timer_Display_Tick);
            // 
            // contextMenu_Server
            // 
            this.contextMenu_Server.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_AddServer,
            this.menuItem_InsertServer,
            this.menuItem_DeleteServer,
            this.menuItem_ModifyServer,
            this.menuItem1,
            this.menuItem_AddGroup,
            this.menuItem2,
            this.menuItem_Connect});
            resources.ApplyResources(this.contextMenu_Server, "contextMenu_Server");
            // 
            // menuItem_AddServer
            // 
            resources.ApplyResources(this.menuItem_AddServer, "menuItem_AddServer");
            this.menuItem_AddServer.Index = 0;
            this.menuItem_AddServer.Click += new System.EventHandler(this.menuItem_AddServer_Click);
            // 
            // menuItem_InsertServer
            // 
            resources.ApplyResources(this.menuItem_InsertServer, "menuItem_InsertServer");
            this.menuItem_InsertServer.Index = 1;
            this.menuItem_InsertServer.Click += new System.EventHandler(this.menuItem_InsertServer_Click);
            // 
            // menuItem_DeleteServer
            // 
            resources.ApplyResources(this.menuItem_DeleteServer, "menuItem_DeleteServer");
            this.menuItem_DeleteServer.Index = 2;
            this.menuItem_DeleteServer.Click += new System.EventHandler(this.menuItem_DeleteServer_Click);
            // 
            // menuItem_ModifyServer
            // 
            resources.ApplyResources(this.menuItem_ModifyServer, "menuItem_ModifyServer");
            this.menuItem_ModifyServer.Index = 3;
            this.menuItem_ModifyServer.Click += new System.EventHandler(this.menuItem_ModifyServer_Click);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 4;
            // 
            // menuItem_AddGroup
            // 
            resources.ApplyResources(this.menuItem_AddGroup, "menuItem_AddGroup");
            this.menuItem_AddGroup.Index = 5;
            this.menuItem_AddGroup.Click += new System.EventHandler(this.menuItem_AddGroup_Click);
            // 
            // menuItem2
            // 
            resources.ApplyResources(this.menuItem2, "menuItem2");
            this.menuItem2.Index = 6;
            // 
            // menuItem_Connect
            // 
            resources.ApplyResources(this.menuItem_Connect, "menuItem_Connect");
            this.menuItem_Connect.Index = 7;
            this.menuItem_Connect.Click += new System.EventHandler(this.menuItem_Connect_Click);
            // 
            // contextMenu_Group
            // 
            this.contextMenu_Group.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_AddGroup2,
            this.menuItem_InsertGroup,
            this.menuItem_DeleteGroup,
            this.menuItem_ModifyGroup,
            this.menuItem5,
            this.menuItem_AddItem});
            resources.ApplyResources(this.contextMenu_Group, "contextMenu_Group");
            // 
            // menuItem_AddGroup2
            // 
            resources.ApplyResources(this.menuItem_AddGroup2, "menuItem_AddGroup2");
            this.menuItem_AddGroup2.Index = 0;
            this.menuItem_AddGroup2.Click += new System.EventHandler(this.menuItem_AddGroup2_Click);
            // 
            // menuItem_InsertGroup
            // 
            resources.ApplyResources(this.menuItem_InsertGroup, "menuItem_InsertGroup");
            this.menuItem_InsertGroup.Index = 1;
            this.menuItem_InsertGroup.Click += new System.EventHandler(this.menuItem_InsertGroup_Click);
            // 
            // menuItem_DeleteGroup
            // 
            resources.ApplyResources(this.menuItem_DeleteGroup, "menuItem_DeleteGroup");
            this.menuItem_DeleteGroup.Index = 2;
            this.menuItem_DeleteGroup.Click += new System.EventHandler(this.menuItem_DeleteGroup_Click);
            // 
            // menuItem_ModifyGroup
            // 
            resources.ApplyResources(this.menuItem_ModifyGroup, "menuItem_ModifyGroup");
            this.menuItem_ModifyGroup.Index = 3;
            this.menuItem_ModifyGroup.Click += new System.EventHandler(this.menuItem_ModifyGroup_Click);
            // 
            // menuItem5
            // 
            resources.ApplyResources(this.menuItem5, "menuItem5");
            this.menuItem5.Index = 4;
            // 
            // menuItem_AddItem
            // 
            resources.ApplyResources(this.menuItem_AddItem, "menuItem_AddItem");
            this.menuItem_AddItem.Index = 5;
            this.menuItem_AddItem.Click += new System.EventHandler(this.menuItem_AddItem_Click);
            // 
            // contextMenu_Item
            // 
            this.contextMenu_Item.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_AddItem2,
            this.menuItem_insertItem,
            this.menuItem_DeleteItem,
            this.menuItem_ModifyItem});
            resources.ApplyResources(this.contextMenu_Item, "contextMenu_Item");
            // 
            // menuItem_AddItem2
            // 
            resources.ApplyResources(this.menuItem_AddItem2, "menuItem_AddItem2");
            this.menuItem_AddItem2.Index = 0;
            this.menuItem_AddItem2.Click += new System.EventHandler(this.menuItem_AddItem2_Click);
            // 
            // menuItem_insertItem
            // 
            resources.ApplyResources(this.menuItem_insertItem, "menuItem_insertItem");
            this.menuItem_insertItem.Index = 1;
            this.menuItem_insertItem.Click += new System.EventHandler(this.menuItem_insertItem_Click);
            // 
            // menuItem_DeleteItem
            // 
            resources.ApplyResources(this.menuItem_DeleteItem, "menuItem_DeleteItem");
            this.menuItem_DeleteItem.Index = 2;
            this.menuItem_DeleteItem.Click += new System.EventHandler(this.menuItem_DeleteItem_Click);
            // 
            // menuItem_ModifyItem
            // 
            resources.ApplyResources(this.menuItem_ModifyItem, "menuItem_ModifyItem");
            this.menuItem_ModifyItem.Index = 3;
            this.menuItem_ModifyItem.Click += new System.EventHandler(this.menuItem_ModifyItem_Click);
            // 
            // contextMenu_Write
            // 
            this.contextMenu_Write.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_itemWriteValue});
            resources.ApplyResources(this.contextMenu_Write, "contextMenu_Write");
            // 
            // menuItem_itemWriteValue
            // 
            resources.ApplyResources(this.menuItem_itemWriteValue, "menuItem_itemWriteValue");
            this.menuItem_itemWriteValue.Index = 0;
            this.menuItem_itemWriteValue.Click += new System.EventHandler(this.menuItem_itemWriteValue_Click);
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.treeView_Opc);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // splitter1
            // 
            this.splitter1.AccessibleDescription = null;
            this.splitter1.AccessibleName = null;
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.BackgroundImage = null;
            this.splitter1.Font = null;
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.splitter2);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // panel4
            // 
            this.panel4.AccessibleDescription = null;
            this.panel4.AccessibleName = null;
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.BackgroundImage = null;
            this.panel4.Font = null;
            this.panel4.Name = "panel4";
            // 
            // splitter2
            // 
            this.splitter2.AccessibleDescription = null;
            this.splitter2.AccessibleName = null;
            resources.ApplyResources(this.splitter2, "splitter2");
            this.splitter2.BackgroundImage = null;
            this.splitter2.Font = null;
            this.splitter2.Name = "splitter2";
            this.splitter2.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.AccessibleDescription = null;
            this.panel3.AccessibleName = null;
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.BackgroundImage = null;
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Font = null;
            this.panel3.Name = "panel3";
            // 
            // panel6
            // 
            this.panel6.AccessibleDescription = null;
            this.panel6.AccessibleName = null;
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.BackgroundImage = null;
            this.panel6.Controls.Add(this.listView_errorOrEventMessage);
            this.panel6.Font = null;
            this.panel6.Name = "panel6";
            // 
            // listView_errorOrEventMessage
            // 
            this.listView_errorOrEventMessage.AccessibleDescription = null;
            this.listView_errorOrEventMessage.AccessibleName = null;
            resources.ApplyResources(this.listView_errorOrEventMessage, "listView_errorOrEventMessage");
            this.listView_errorOrEventMessage.BackgroundImage = null;
            this.listView_errorOrEventMessage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8});
            this.listView_errorOrEventMessage.Font = null;
            this.listView_errorOrEventMessage.FullRowSelect = true;
            this.listView_errorOrEventMessage.HideSelection = false;
            this.listView_errorOrEventMessage.Name = "listView_errorOrEventMessage";
            this.listView_errorOrEventMessage.UseCompatibleStateImageBehavior = false;
            this.listView_errorOrEventMessage.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            // 
            // columnHeader8
            // 
            resources.ApplyResources(this.columnHeader8, "columnHeader8");
            // 
            // panel5
            // 
            this.panel5.AccessibleDescription = null;
            this.panel5.AccessibleName = null;
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.BackgroundImage = null;
            this.panel5.Controls.Add(this.label_status);
            this.panel5.Font = null;
            this.panel5.Name = "panel5";
            // 
            // label_status
            // 
            this.label_status.AccessibleDescription = null;
            this.label_status.AccessibleName = null;
            resources.ApplyResources(this.label_status, "label_status");
            this.label_status.Font = null;
            this.label_status.Name = "label_status";
            // 
            // opcGroupItemReadRegisterForm
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Name = "opcGroupItemReadRegisterForm";
            this.ShowInTaskbar = false;
            this.Closed += new System.EventHandler(this.opcGroupItemReadRegisterForm_Closed);
            this.Load += new System.EventHandler(this.opcGroupItemReadRegisterForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		
		void regisgerServerName(bool bAdd, int pos, string hostName, string name, string access)// hostNae 2009-01-22 add
		{
			opcServerReadWriteClass opcServer = new opcServerReadWriteClass();
		
			opcServer = new opcServerReadWriteClass();
            opcServer.hostName = hostName;
			opcServer.serverName = name;
			opcServer.accessName = access;
			opcServer.m_server = null;
			opcServer.arrGroup = new ArrayList();

			TreeNode node = makeNewServerNode(opcServer);
			if(bAdd || opcBasic.arrOpcServer.Count <= 0) 
			{
				opcBasic.arrOpcServer.Add(opcServer);
				this.treeView_Opc.Nodes.Add(node);
			}
			else 
			{
				if(pos < 0 || opcBasic.arrOpcServer.Count <= pos) 
				{
					opcBasic.arrOpcServer.Add(opcServer);
					this.treeView_Opc.Nodes.Add(node);
				}
				else 
				{
					opcBasic.arrOpcServer.Insert(pos, opcServer);
					this.treeView_Opc.Nodes.Insert(pos, node);
				}
			}
		}

		/*int getTreeSelectedNodeDepth()
		{
			if(this.treeView_Opc.SelectedNode == null) return 0;
			return opcBasic.getSelectedTreeNodeDepth(this.treeView_Opc.SelectedNode.FullPath);			
		}*/

		int getTreeNodeDepth()
		{
			if(this.treeView_Opc.SelectedNode == null) return -1;

			TreeNode		parent = treeView_Opc.SelectedNode;			
			for(int i = 0; i < 3; i++) 
			{
				parent = parent.Parent;
				if(parent == null) return i;
			}			
			return -1;
		}

		int getTreeNodeDepthIndex(int depth)
		{
			switch(depth) 
			{
				case 2 : return treeView_Opc.SelectedNode.Parent.Parent.Index;
				case 1 : return treeView_Opc.SelectedNode.Parent.Index;
				case 0 : return treeView_Opc.SelectedNode.Index;
				default: return -1;
			}
		}

		int getServerGroupItemIndex(eTreePosType type)
		{
			int depth = getTreeNodeDepth();
			switch(type) 
			{
				case eTreePosType.SERVER : 
					if(depth < 0) return -1;
					return getTreeNodeDepthIndex(depth);
				case eTreePosType.GROUP : 
					if(depth < 1) return -1;
					return getTreeNodeDepthIndex(depth-1);
				case eTreePosType.ITEM : 
					if(depth < 2) return -1;
					return getTreeNodeDepthIndex(depth-2); 
				default: return -1;
			}
		}

		public bool getTreeFromServerNamePos(ref int nServer)
		{
			int pos = getServerGroupItemIndex(eTreePosType.SERVER);
			if(pos != -1) 
			{
				nServer = pos;
				return true;
			}
			//if(opcBasic.getTreePathFromServerNamePos(ref nServer, treeView_Opc.SelectedNode.FullPath)) return true;			
            //MessageBox.Show("Server Selection error");				
			return false;			
		}
		

		public bool getTreefromGroupNamePos(ref int nGroup, bool bMessage)
		{
			int pos = getServerGroupItemIndex(eTreePosType.GROUP);
			if(pos != -1) 
			{
				nGroup = pos;
				return true;
			}				
			//if(opcBasic.getTreePathFromGroupNamePos(nServer, ref nGroup, treeView_Opc.SelectedNode.FullPath)) return true;				
			if(bMessage) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("그룹을 선택하세요.");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择组。");
				else
					MessageBox.Show("Select the Group.");
			}
			return false;			
		}

		public bool getTreeFromItemNamePos(ref int nItem, bool bMessage)
		{
			int pos = getServerGroupItemIndex(eTreePosType.ITEM);
			if(pos != -1) 
			{
				nItem = pos;
				return true;
			}
			//if(opcBasic.getTreePathFromItemNamePos(nServer, nGroup, ref nItem, treeView_Opc.SelectedNode.FullPath)) return true;				
			
			if(bMessage) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("아이템을 선택하세요.");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择项。");
				else
					MessageBox.Show("Select the Item.");
			}
			return false;			
		}		
		
		void regisgerGroupName(bool bAdd, int nServer, int nGroup, string name, decimal nPeriod, bool bActive, bool bAsync)
		{
			opcServerReadWriteClass opcServer;
			opcGroupReadWriteClass opcGroup;// = new opcGroupReadWriteClass();

			try 
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			}
			catch
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("메모리 열기 실패. ArrayList Open Error.");
				else
					MessageBox.Show("ArrayList Open Error.");
				return;
			}
		
			opcGroup = new opcGroupReadWriteClass(opcServer);
			opcGroup.groupName = name;
			opcGroup.uPeriod = (uint)nPeriod;
			opcGroup.bActive = bActive;
			opcGroup.bAsync = bAsync;
			opcGroup.bRegister = false;
			opcGroup.milli = 0;
			opcGroup.serverName = opcServer.serverName;			// 서버이름을 보관, COM에 전송하기 위해서는 서버이름이 필요하다, ASYNC Read 에서
			opcGroup.arrItem = new ArrayList();

			TreeNode node = makeNewGroupNode(name);
			
			if(bAdd || opcServer.arrGroup.Count <= 0) 
			{
				opcServer.arrGroup.Add(opcGroup);
				treeView_Opc.Nodes[nServer].Nodes.Add(node);
			}
			else 
			{
				if(nGroup < 0 || opcServer.arrGroup.Count <= nGroup) 
				{
					opcServer.arrGroup.Add(opcGroup);
					treeView_Opc.Nodes[nServer].Nodes.Add(node);
				}
				else
				{
					opcServer.arrGroup.Insert(nGroup, opcGroup);
					treeView_Opc.Nodes[nServer].Nodes.Insert(nGroup, node);
				}
			}
			treeView_Opc.Nodes[nServer].Expand();
			fillViewListItem();
		}

		void regisgerItemName(int nServer, int nGroup, string name)
		{
			opcServerReadWriteClass opcServer;
			opcGroupReadWriteClass opcGroup;

			try 
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			}
			catch
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("메모리 열기 실패. ArrayList Open Error.");
				else
					MessageBox.Show("ArrayList Open Error.");
				return;
			}
		
			opcItemReadWriteClass opcItem = new opcItemReadWriteClass();
			opcItem.itemName = name;
			opcItem.bFlag = false;
			opcItem.readData = 0;
			opcGroup.arrItem.Add(opcItem);
		}

		TreeNode makeNewItemNode(string name)
		{
			TreeNode node = new TreeNode(name);
			node.ImageIndex = Resources.IMAGE_BROWSE_ITEM;
			node.SelectedImageIndex = Resources.IMAGE_BROWSE_ITEM;
			return node;
		}

		void FillItem(TreeNode root_node, opcGroupReadWriteClass opcGroup)
		{
			if(opcGroup.arrItem.Count <= 0) return;
			opcItemReadWriteClass opcItem;

			for(int i = 0; i < opcGroup.arrItem.Count; i++) 
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				TreeNode node = makeNewItemNode(opcBasic.getTreeViewItemName(opcItem));		
				root_node.Nodes.Add(node);
			}
		}

		TreeNode makeNewGroupNode(string name)
		{
			TreeNode node = new TreeNode(name);
			node.ImageIndex = Resources.IMAGE_BROWSE_NODE;
			node.SelectedImageIndex = Resources.IMAGE_BROWSE_NODE;
			return node;
		}

		void FillGroup(TreeNode root_node, opcServerReadWriteClass opcServer)
		{
			if(opcServer.arrGroup.Count <= 0) return;
			opcGroupReadWriteClass opcGroup;

			for(int i = 0; i < opcServer.arrGroup.Count; i++) 
			{
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];
				TreeNode node = makeNewGroupNode(opcGroup.groupName);
				root_node.Nodes.Add(node);
				FillItem(node, opcGroup);
			}
		}

		TreeNode makeNewServerNode(opcServerReadWriteClass opcServer)
		{
			TreeNode node = new TreeNode(opcBasic.getTreeViewServerName(opcServer));
			node.ImageIndex = Resources.IMAGE_LOCAL_SERVER;
			node.SelectedImageIndex = Resources.IMAGE_LOCAL_SERVER;
			return node;
		}

		public void fillServerNode()
		{
			this.treeView_Opc.Nodes.Clear();
			if(opcBasic.arrOpcServer.Count <= 0) return;
			opcServerReadWriteClass opcServer;			

			for(int i = 0;i < opcBasic.arrOpcServer.Count; i++) 
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];
				TreeNode node = makeNewServerNode(opcServer);
				FillGroup(node, opcServer);
				this.treeView_Opc.Nodes.Add(node);				
			}
			this.treeView_Opc.ExpandAll();
			fillViewListItem();
		}

		void makeOneGroupListView(opcGroupReadWriteClass opcGroup)
		{
			opcItemReadWriteClass	opcItem;
			ListViewItem			item;

			for(int j = 0; j < opcGroup.arrItem.Count; j++) 
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[j];
				item = new ListViewItem();
				item.Text = opcGroup.groupName + "." + opcItem.itemName;
				item.SubItems.Add("");
				item.SubItems.Add("");
				item.SubItems.Add("");
				item.SubItems.Add("");
				//list.l
				//this.listView_Display.Items.Add(item);
			}
		}

		void fillViewListItem()
		{
			nDisplayServer = 0;
			nDisplayGroup = 0;
			getAndcheckServerPos(ref nDisplayServer);						// 현재 디스플레이 서버를 찾는다.
			getAndCheckGroupPos(nDisplayServer, ref nDisplayGroup, false);	// 현재 디스플레이 그룹를 찾는다.			
			int depth = getTreeNodeDepth();

			if(nDisplayServer >= opcBasic.arrOpcServer.Count) {
				list.listHap = 0;
				list.listItemChanged();
				//listView_Display.Items.Clear();
				label_status.Text = "";
				return;
			}
			opcServerReadWriteClass	opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nDisplayServer];
			int						i;			
			opcGroupReadWriteClass	opcGroup;			
			
			//this.listView_Display.Items.Clear();
			if(depth == 0 || depth == -1)
			{
				bServerDisplay = true;
				int			count = 0;
				for(i = 0; i < opcServer.arrGroup.Count; i++) 
				{
					opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];
					count += opcGroup.arrItem.Count;
					//makeOneGroupListView(opcGroup);
				}
				list.listHap = count;
				list.listItemChanged();
			}
			else 
			{
				bServerDisplay = false;
				if(nDisplayGroup >= opcServer.arrGroup.Count) 
				{
					list.listHap = 0;
					list.listItemChanged();
					return;
				}
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nDisplayGroup];
				list.listHap = opcGroup.arrItem.Count;
				list.listItemChanged();
				//makeOneGroupListView(opcGroup);
			}
			displayServerStatusList();				// 연결상태를 표시
			nCurrDisplayServer = -1;				// 새로 그리기 위해서 -1로 설정
			nCurrDisplayGroup = -1;					// 새로 그리기 위해서 -1로 설정
			//oneServerListDisplay();			
		}

		public static opcGroupItemReadRegisterForm formThis;

		
		private void opcGroupItemReadRegisterForm_Load(object sender, System.EventArgs e)
		{
			list.paintMessage += new NetTools.ControlListView.OnEventPaintMessage(onPaintMessage);

			AutoLibLocal.TotalConfig.AutoBaseListCtrlConfigLoad(ref headWidth, "OpcClient", "GroupItemDisplay");
			this.panel1.Width = 300;
			formThis = this;
			list.bOwnerDraw = true;
			listHeaderFill();
			list.Show();
			fillListDataAll();
			fillServerNode();

			DateTime		dt = DateTime.Now;
			old_mili_sec = dt.Second*1000+dt.Millisecond;
		}

		void listHeaderFill()
		{
			ControlListViewHeader	head;
			string[]				text = new string[5];
						
			if(Tools.IsLangKorean()) 
			{
				text[0] = "그룹/아이템 이름";
				text[1] = "현재 값";
				text[2] = "자료 형식";
				text[3] = "읽기 상태";
				text[4] = "읽기 시간";
			}
			else if(Tools.IsLangJapanese()) 
			{
				text[0] = "グループ/アイテム名";
				text[1] = "現在値";
				text[2] = "データ形式";
				text[3] = "読み状態";
				text[4] = "読み時間"; 
			}
			else if(Tools.IsLangChinese()) 
			{
				text[0] = "组名/项名";
				text[1] = "现在值";
				text[2] = "资料格式";
				text[3] = "阅读状态";
				text[4] = "阅读时间"; 
			}
			else 
			{
				text[0] = "Group/ItemName";
				text[1] = "Value";
				text[2] = "Data Type";
				text[3] = "Read Status";
				text[4] = "Time Stamp";
			}			

			for(int i = 0; i < 5; i++) 
			{
				head = new ControlListViewHeader();
				head.width = headWidth[i];
				head.text = text[i];
				head.format = new StringFormat();
				if(i == 0) head.format.Alignment = StringAlignment.Near;
				else	   head.format.Alignment = StringAlignment.Center;
				head.color = Color.Black;
				list.header.Add(head);
			}
		}

		void fillListDataAll()
		{
			//if(tagList == null) list.listHap = 0;
			//else list.listHap = tagList.Length;
			list.listItemChanged();			
			//setDetailButtonEnableDisable();
		}

		string getReadItemValueString(object val)
		{
			int			i;
			string		data = "";

			if(opcBasic.isValueObjectIsSingleType(val))
			{
				data = val.ToString();
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_INT) 
			{
				int[] iVal = (int[])val;
				for(i = 0; i < iVal.Length; i++) 
				{
					if(i == 0) data = iVal[i].ToString();
					else	   data += "," + iVal[i].ToString();					
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_DOUBLE)
			{
				double[] dVal = (double[])val;
				for(i = 0; i < dVal.Length; i++) 
				{
					if(i == 0) data = dVal[i].ToString();
					else	   data += "," + dVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_BINARY) 
			{
				byte[] byteVal = (byte[])val;
				for(i = 0; i < byteVal.Length; i++) 
				{
					if(i == 0) data = byteVal[i].ToString();
					else	   data += "," + byteVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_SBINARY) 
			{
				sbyte[] sbyteVal = (sbyte[])val;
				for(i = 0; i < sbyteVal.Length; i++) 
				{
					if(i == 0) data = sbyteVal[i].ToString();
					else	   data += "," + sbyteVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_FLOAT) 
			{
				float[] fVal = (float[])val;
				for(i = 0; i < fVal.Length; i++) 
				{
					if(i == 0) data = fVal[i].ToString();
					else	   data += "," + fVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_STRING) 
			{
				string[] sVal = (string[])val;
				for(i = 0; i < sVal.Length; i++) 
				{
					if(i == 0) data = sVal[i].ToString();
					else	   data += "," + sVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_SHORT) 
			{
				short[] shortVal = (short[])val;
				for(i = 0; i < shortVal.Length; i++) 
				{
					if(i == 0) data = shortVal[i].ToString();
					else	   data += "," + shortVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_USHORT) 
			{
				ushort[] ushortVal = (ushort[])val;
				for(i = 0; i < ushortVal.Length; i++) 
				{
					if(i == 0) data = ushortVal[i].ToString();
					else	   data += "," + ushortVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_UINT) 
			{
				uint[] uintVal = (uint[])val;
				for(i = 0; i < uintVal.Length; i++) 
				{
					if(i == 0) data = uintVal[i].ToString();
					else	   data += "," + uintVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_LONG) 
			{
				long[] longVal = (long[])val;
				for(i = 0; i < longVal.Length; i++) 
				{
					if(i == 0) data = longVal[i].ToString();
					else	   data += "," + longVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_ULONG) 
			{
				ulong[] ulongVal = (ulong[])val;
				for(i = 0; i < ulongVal.Length; i++) 
				{
					if(i == 0) data = ulongVal[i].ToString();
					else	   data += "," + ulongVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_DECIMAL) 
			{
				decimal[] decimalVal = (decimal[])val;
				for(i = 0; i < decimalVal.Length; i++) 
				{
					if(i == 0) data = decimalVal[i].ToString();
					else	   data += "," + decimalVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_BOOLEAN) 
			{
				bool[] boolVal = (bool[])val;
				for(i = 0; i < boolVal.Length; i++) 
				{
					if(i == 0) data = boolVal[i].ToString();
					else	   data += "," + boolVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_DATETIME) 
			{
				DateTime[] DateTimeVal = (DateTime[])val;
				for(i = 0; i < DateTimeVal.Length; i++) 
				{
					if(i == 0) data = DateTimeVal[i].ToString();
					else	   data += "," + DateTimeVal[i].ToString();
				}
				return data;
			}
			if(val.GetType() == Opc.Type.ARRAY_ANY_TYPE) 
			{
				object[] objectVal = (object[])val;
				for(i = 0; i < objectVal.Length; i++) 
				{					
					if(i == 0) data = getReadItemValueString(objectVal[i]);
					else	   data += "," + getReadItemValueString(objectVal[i]);
				}
				return data;
			}
			return "Unknown Data Type";
		}

		/*void oneGroupListDisplay(ref int pos, bool bEqual, opcGroupReadWriteClass opcGroup)
		{
			opcItemReadWriteClass	opcItem;
			string					imsi = "";
			ListViewItem			item;

			for(int j = 0; j < opcGroup.arrItem.Count; j++, pos++)
			{
				if(pos >= listView_Display.Items.Count) continue;
				item = listView_Display.Items[pos];
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[j];
				if(bEqual && opcItem.bNewRead == false) continue;
				imsi = getReadItemValueString(opcItem.readData);
				item.SubItems[1].Text = imsi;
				if(opcGroup.bActive == false) item.SubItems[2].Text = "";
				else if(opcItem.bFlag && opcItem.readData != null) item.SubItems[2].Text = opcItem.readData.GetType().ToString();
				//else if(opcItem.reuslt == Opc.ResultID.E_WRITEONLY && opcItem.readData != null) item.SubItems[2].Text = opcItem.readData.GetType().ToString();
				else item.SubItems[2].Text = "";//"Unknown";
				if(opcGroup.bActive == false) item.SubItems[3].Text = "InActive";					
				else if(opcItem.bFlag == false) 
				{
					//if(bEqual == false) item.SubItems[3].Text = "";
					//else item.SubItems[3].Text = "Read Error";
					item.SubItems[3].Text = opcItem.reuslt.ToString();
				}
				else item.SubItems[3].Text = "Read OK";
				if(opcItem.bFlag == false || opcGroup.bActive == false) item.SubItems[4].Text = "";
				else item.SubItems[4].Text = opcItem.timeStamp.ToString();
				opcItem.bNewRead = false;
			}
		}*/

		/*void oneServerListDisplay()
		{
			if(nDisplayServer >= opcBasic.arrOpcServer.Count) return;
			opcServerReadWriteClass	opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nDisplayServer];

			int						i, pos = 0;			
			opcGroupReadWriteClass	opcGroup;
			bool					bEqual = (nCurrDisplayServer == nDisplayServer) ? true : false;

			if(bServerDisplay) 
			{
				for(i = 0; i < opcServer.arrGroup.Count; i++) 
				{
					opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];
					oneGroupListDisplay(ref pos, bEqual, opcGroup);
				}
			}
			else 
			{
				if(nDisplayGroup >= opcServer.arrGroup.Count) return;
				if(bEqual) bEqual = (nCurrDisplayGroup == nDisplayGroup) ? true : false;// 동일한 서버일 때, 동일한 그룹인가를 검사
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nDisplayGroup];
				oneGroupListDisplay(ref pos, bEqual, opcGroup);
			}
			nCurrDisplayServer = nDisplayServer;
			nCurrDisplayGroup = nDisplayGroup;
		}*/

		void checkServerListDisplay()
		{
			if(nDisplayServer >= opcBasic.arrOpcServer.Count) return;			
			opcServerReadWriteClass	opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nDisplayServer];
			opcGroupReadWriteClass	opcGroup;
			opcItemReadWriteClass	opcItem;
			bool					bChange = false;			
			
			if(bServerDisplay) 
			{
				for(int i = 0; i < opcServer.arrGroup.Count; i++) 
				{
					opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];					
					for(int j = 0; j < opcGroup.arrItem.Count; j++)
					{
						opcItem = (opcItemReadWriteClass)opcGroup.arrItem[j];
						if(opcItem.bNewRead == false) continue;
						bChange = true;
						opcItem.bNewRead = false;
					}
				}
			}
			else 
			{
				if(nDisplayGroup >= opcServer.arrGroup.Count) return;
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nDisplayGroup];
				for(int j = 0; j < opcGroup.arrItem.Count; j++)
				{
					opcItem = (opcItemReadWriteClass)opcGroup.arrItem[j];
					if(opcItem.bNewRead == false) continue;
					bChange = true;
					opcItem.bNewRead = false;
				}
			}
			if(bChange) list.Invalidate();
		}

		bool bTimerTick = false;

		private void timer_Display_Tick(object sender, System.EventArgs e)
		{
			if(opcBasic.arrOpcServer.Count <= 0) return;
			bTimerTick = !bTimerTick;
			if(bTimerTick == false) return;				// timer를 유연하게 하기 위해

			DateTime					dt = DateTime.Now;
			long						miliSec, hap;

			miliSec = dt.Second*1000+dt.Millisecond;
			hap = (miliSec >= old_mili_sec) ? miliSec-old_mili_sec : 60000+miliSec-old_mili_sec;
			old_mili_sec = miliSec;
			remain_mili_sec += hap;
			if(remain_mili_sec < opcBasic.opcClientConfig.nItemDisplayPeriod) return;
			//remain_mili_sec -= (long)opcBasic.opcClientConfig.nItemDisplayPeriod;	// 정확한 시간계산
			remain_mili_sec = 0;

			timer_Display.Enabled = false;			

			//oneServerListDisplay();
			checkServerListDisplay();
			timer_Display.Enabled = true;	
		}

		public bool isSelectedServerConnected()
		{
			int		nServer = 0;
			if(getAndcheckServerPos(ref nServer)) 
			{
				opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
				if(opcServer.m_server != null && opcServer.m_server.IsConnected == true) return true;					
				else return false;					
			}
			return false;
		}

		private void treeView_Opc_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Right) return;

			Point pos = new Point(e.X, e.Y);
			switch(getTreeNodeDepth())
			{
				case 0 :
				case -1 :		// 선택된 노드가 없을 경우
					if(isSelectedServerConnected()) 
					{
						if(NetTools.Tools.IsLangKorean())
							this.menuItem_Connect.Text = "연결끊기(&C)";
						else if(Tools.IsLangChinese())
							this.menuItem_Connect.Text = "中断连接(&C)";
						else
							this.menuItem_Connect.Text = "Dis&Connect";
					}
					else 
					{
						if(NetTools.Tools.IsLangKorean())
							this.menuItem_Connect.Text = "연결(&C)";
						else if(NetTools.Tools.IsLangChinese())
							this.menuItem_Connect.Text = "连接(&C)";
						else
							this.menuItem_Connect.Text = "&Connect";
					}
					this.contextMenu_Server.Show(this.treeView_Opc, pos);	// Server
					break;
				case 1 :
					this.contextMenu_Group.Show(this.treeView_Opc, pos);	// Group
					break;
				case 2 :
					this.contextMenu_Item.Show(this.treeView_Opc, pos);	// Item
					break;
			}
		}


		public void addInsetServerName(bool bAdd)
		{
			opcServerReadRegisterForm dialog = new opcServerReadRegisterForm();
			dialog.bAdd = bAdd;
			dialog.bModify = false;
			int		nServer = 0;
			if(bAdd == false) 
			{
				if(getTreeFromServerNamePos(ref nServer) == false) bAdd = true;		// 선택된 서버가 없을 때는 삽입				
			}
            dialog.textBox_HostName.Text = "";      // 2009-01-22 add
			dialog.textBox_ServerName.Text = "";
			dialog.textBox_accessName.Text = "";
			dialog.ShowDialog(this);
			if(dialog.DialogResult == DialogResult.OK) 
			{
                regisgerServerName(bAdd, nServer, dialog.textBox_HostName.Text, dialog.textBox_ServerName.Text, dialog.textBox_accessName.Text);//dialog.textBox_HostName.Text, 2009-01-22 add
				nServer = 0;
				getAndcheckServerPos(ref nServer);										// 현재 디스플레이 서버를 찾는다.
				nDisplayServer = nServer;
			}
		}


		private void menuItem_AddServer_Click(object sender, System.EventArgs e)
		{
			addInsetServerName(true);
		}

		private void menuItem_InsertServer_Click(object sender, System.EventArgs e)
		{
			addInsetServerName(false);			
		}


		bool getAndcheckServerPos(ref int nServer)
		{
			if(getTreeFromServerNamePos(ref nServer) == false) return false;
			if(nServer >= opcBasic.arrOpcServer.Count) return false;
			return true;
		}

		public bool getAndCheckGroupPos(int nServer, ref int nGroup, bool bMessage)
		{
			if(getTreefromGroupNamePos(ref nGroup, bMessage) == false) return false;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			if(nGroup >= opcServer.arrGroup.Count) return false;
			return true;
		}

		public bool getAndCheckItemPos(int nGroup, ref int nItem, opcServerReadWriteClass opcServer)
		{
			if(getTreeFromItemNamePos(ref nItem, true) == false) return false;
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			if(nItem >= opcGroup.arrItem.Count) return false;
			return true;
		}

		public void deleteOneSelectedServer()
		{
			int			nServer = 0;
			if(!getAndcheckServerPos(ref nServer)) return;
			
			DialogResult result;
			if(NetTools.Tools.IsLangKorean())
				result = MessageBox.Show(this, "선택한 서버를 삭제 할까요?", "삭제확인", MessageBoxButtons.YesNo,	MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			else if(NetTools.Tools.IsLangChinese())
				result = MessageBox.Show(this, "要删除选择的服务器吗？", "删除确认", MessageBoxButtons.YesNo,	MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			else
				result = MessageBox.Show(this, "Are you sure you want to delete the selected server?", "Delete Server", MessageBoxButtons.YesNo,	MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

			if(result == DialogResult.Yes)
			{
				opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];

				if(opcServer.m_server != null && opcServer.m_server.IsConnected)// 연결되어 있을경우
					opcReadWriteGroupLoopClass.opcServerDisConnection(opcServer, true);

				plcScanComm.plcScanCommMainClass.plcScanThreadPause(true);	// thread 를 일시중지
				if(comShareClass.bShareDllExist) comShareClass.comOpcShareOneDeleteServer(opcServer);		// COM에서 하나의 서버를 삭제
				opcBasic.arrOpcServer.RemoveAt(nServer);
				plcScanComm.plcScanCommMainClass.plcScanThreadPause(false);	// thread 를 재 시작

				this.treeView_Opc.Nodes.RemoveAt(nServer);
				fillViewListItem();
			}
		}

		private void menuItem_DeleteServer_Click(object sender, System.EventArgs e)
		{
			deleteOneSelectedServer();
		}


		void setChangedServerNameToGroup(opcServerReadWriteClass opcServer) 
		{
			opcGroupReadWriteClass opcGroup;
			for(int i = 0; i < opcServer.arrGroup.Count; i++) 
			{
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];
				opcGroup.serverName = opcServer.serverName;
			}
		}


		public void modifyOneSelectedServer()
		{
			int			nServer = 0;
			if(!getAndcheckServerPos(ref nServer)) return;
			
			opcServerReadRegisterForm dialog = new opcServerReadRegisterForm();
			dialog.bAdd = false;
			dialog.bModify = true;
			dialog.nServerPos = nServer;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
            dialog.textBox_HostName.Text = opcServer.hostName;          // 2009-01-22 add
			dialog.textBox_ServerName.Text = opcServer.serverName;
			dialog.textBox_accessName.Text = opcServer.accessName;
			bool		bNameChanged = false;
			dialog.ShowDialog(this);
			if(dialog.DialogResult == DialogResult.OK) 
			{
				if(opcServer.hostName == dialog.textBox_HostName.Text && opcServer.serverName == dialog.textBox_ServerName.Text && opcServer.accessName == dialog.textBox_accessName.Text) return; // 수정된 내용이 없다

				if(opcServer.hostName != dialog.textBox_HostName.Text || opcServer.serverName != dialog.textBox_ServerName.Text) bNameChanged = true;
				if(opcServer.m_server != null && opcServer.m_server.IsConnected && bNameChanged)// 서버이름이 변경되었을 경우
					opcReadWriteGroupLoopClass.opcServerDisConnection(opcServer, true);
				if(bNameChanged) 
				{
					if(comShareClass.bShareDllExist) comShareClass.comOpcShareOneDeleteServer(opcServer);// COM에서 하나의 서버를 삭제
				}
                opcServer.hostName = dialog.textBox_HostName.Text;
				opcServer.serverName = dialog.textBox_ServerName.Text;
				opcServer.accessName = dialog.textBox_accessName.Text;

				TreeNode node = this.treeView_Opc.Nodes[nServer];
				node.Text = opcBasic.getTreeViewServerName(opcServer);
				
				opcReadWriteGroupLoopClass.opcServerConnection(opcServer);
				if(bNameChanged) 
				{
					if(comShareClass.bShareDllExist) comShareClass.comOpcShareOneRegisterServer(opcServer);// COM에 하나의 서버를 추가
					setChangedServerNameToGroup(opcServer);				  // 변경된 서버이름을 그룹에 있는 서버이름에도 변경 
				}
			}
		}

		private void menuItem_ModifyServer_Click(object sender, System.EventArgs e)
		{
			modifyOneSelectedServer();
		}

		public void insertAddGroupName(bool bAdd)
		{
			int			nServer = 0, nGroup = 0;
			if(!getAndcheckServerPos(ref nServer)) return;
			if(bAdd == false) 
			{
				if(!getAndCheckGroupPos(nServer, ref nGroup, true)) bAdd = true;		// 선택된 그룹이 없을 때는 항상 삽입				
			}
						
			opcGroupReadRegisgerForm2 dialog = new opcGroupReadRegisgerForm2(nServer);
			dialog.bAdd = bAdd;
			dialog.bModify = false;
			dialog.textBox_GroupName.Text = "";
			dialog.checkBox_Active.Checked = true;
			dialog.checkBox_UseAsyncRead.Checked = true;
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
			if(dialog.DialogResult == DialogResult.OK) 
			{
				regisgerGroupName(bAdd, nServer, nGroup, dialog.textBox_GroupName.Text, dialog.numericUpDown_Period.Value, dialog.checkBox_Active.Checked, dialog.checkBox_UseAsyncRead.Checked);
			}
		}

		private void menuItem_AddGroup_Click(object sender, System.EventArgs e)
		{
			insertAddGroupName(true);
		}

		private void menuItem_InsertGroup_Click(object sender, System.EventArgs e)
		{
			insertAddGroupName(false);
		}

		private void menuItem_AddGroup2_Click(object sender, System.EventArgs e)
		{
			menuItem_AddGroup_Click(sender, e);		
		}

		public void deleteSelectedGrouopName()
		{
			int			nServer = 0, nGroup = 0;
			if(!getAndcheckServerPos(ref nServer)) return;
			if(!getAndCheckGroupPos(nServer, ref nGroup, true)) return;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			
			DialogResult result;
			if(NetTools.Tools.IsLangKorean())
				result = MessageBox.Show(this, "선택한 그룹을 삭제 할까요?", "삭제확인", MessageBoxButtons.YesNo,	MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            else if (NetTools.Tools.IsLangJapanese())
                result = MessageBox.Show(this, "選択したグループを削除しますか?", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			else
				result = MessageBox.Show(this, "Are you sure you want to delete the seleted group?", "Delete Group", MessageBoxButtons.YesNo,	MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

			if(result == DialogResult.Yes)
			{
				opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
				if(opcGroup.bAsync) opcBasic.deleteOneGroupAsync(opcServer, opcGroup);// Subscription 삭제
				plcScanComm.plcScanCommMainClass.plcScanThreadPause(true);	// thread 를 일시중지
				if(comShareClass.bShareDllExist) comShareClass.comOpcShareOneDeleteGroup(opcServer, opcGroup);// COM 에서 하나의 그룹을 삭제
				opcServer.arrGroup.RemoveAt(nGroup);
				plcScanComm.plcScanCommMainClass.plcScanThreadPause(false);	// thread 를 재 시작

				this.treeView_Opc.Nodes[nServer].Nodes[nGroup].Remove();
				fillViewListItem();
			}
		}

		
		private void menuItem_DeleteGroup_Click(object sender, System.EventArgs e)
		{
			deleteSelectedGrouopName();
		}

		bool checkGroupEditChange(opcGroupReadWriteClass opcGroup, opcGroupReadRegisgerForm2 dialog)
		{
			if(opcGroup.groupName != dialog.textBox_GroupName.Text) return true;
			if(opcGroup.uPeriod != (uint)dialog.numericUpDown_Period.Value) return true;
			if(opcGroup.bActive != dialog.checkBox_Active.Checked) return true;
			if(opcGroup.bAsync != dialog.checkBox_UseAsyncRead.Checked) return true;
			return false;
		}

		public void modifySelectedGroupName()
		{
			int			nServer = 0, nGroup = 0;
			if(!getAndcheckServerPos(ref nServer)) return;
			if(!getAndCheckGroupPos(nServer, ref nGroup, true)) return;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			
			opcGroupReadRegisgerForm2 dialog = new opcGroupReadRegisgerForm2(nServer);
			dialog.bAdd = false;
			dialog.bModify = true;
			dialog.nGroupPos = nGroup;
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			dialog.textBox_GroupName.Text = opcGroup.groupName;
			dialog.numericUpDown_Period.Value = (decimal)opcGroup.uPeriod;
			dialog.checkBox_Active.Checked = opcGroup.bActive;
			dialog.checkBox_UseAsyncRead.Checked = opcGroup.bAsync;
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
			if(dialog.DialogResult == DialogResult.OK && checkGroupEditChange(opcGroup, dialog)) 
			{
				opcGroup.groupName = dialog.textBox_GroupName.Text;
				opcGroup.uPeriod = (uint)dialog.numericUpDown_Period.Value;
				if(comShareClass.bShareDllExist) comShareClass.comOpcShareOneDeleteGroup(opcServer, opcGroup);	// COM 에서 하나의 아이템을 삭제
				if(opcGroup.bAsync && opcGroup.bActive)					// 이전상태 : 연결
				{
					opcGroup.bActive = dialog.checkBox_Active.Checked;
					opcGroup.bAsync = dialog.checkBox_UseAsyncRead.Checked;					
					if(opcGroup.bAsync && opcGroup.bActive) 			// 계속 Async 상태
						opcBasic.modifyGroupAsyncProperties(opcServer, opcGroup);
					if(!opcGroup.bAsync || !opcGroup.bActive)			// 연결종료조건
						opcBasic.deleteOneGroupAsync(opcServer, opcGroup);// Subscription 삭제
				}
				else													// 이전상태 : 연결안됨
				{
					opcGroup.bActive = dialog.checkBox_Active.Checked;
					opcGroup.bAsync = dialog.checkBox_UseAsyncRead.Checked;
					if(opcGroup.bAsync && opcGroup.bActive) 				// Async 상태
						opcBasic.registerOneGroupAsync(opcServer, opcGroup);// Subscription 등록
				}
				if(comShareClass.bShareDllExist) comShareClass.comOpcShareOneRegisterGroup(opcServer, opcGroup);	// COM 에서 하나의 아이템을 삭제
				TreeNode node = this.treeView_Opc.Nodes[nServer].Nodes[nGroup];
				node.Text = opcGroup.groupName;
				fillViewListItem();
			}
		}

		private void menuItem_ModifyGroup_Click(object sender, System.EventArgs e)
		{
			modifySelectedGroupName();
		}

		public void insertAddOpcTreeItem(int nServer, int nGroup, int nItem, opcItemReadWriteClass opcItem, bool bAdd)
		{
			TreeNode node = makeNewItemNode(opcBasic.getTreeViewItemName(opcItem));

			if(bAdd)
				this.treeView_Opc.Nodes[nServer].Nodes[nGroup].Nodes.Add(node);
			else
				this.treeView_Opc.Nodes[nServer].Nodes[nGroup].Nodes.Insert(nItem, node);
			treeView_Opc.Nodes[nServer].Nodes[nGroup].Expand();
			fillViewListItem();
		}

		public void insertAddItem(bool bAdd)
		{
			int		nServer = 0, nGroup = 0, nItem = 0;

			if(!getAndcheckServerPos(ref nServer)) return;
			if(!getAndCheckGroupPos(nServer, ref nGroup, true)) return;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			if(bAdd == false) 
			{
				if(!getAndCheckItemPos(nGroup, ref nItem, opcServer)) bAdd = true;		// 등록된 아이템이 없을 때는 추가
			}
						
			opcItemSearchDlgForm dialog = new opcItemSearchDlgForm(opcServer.m_server);
			dialog.bAdd = bAdd;
			dialog.nServer = nServer;
			dialog.nGroup = nGroup;
			dialog.nItem = nItem;
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
		}

		private void menuItem_AddItem2_Click(object sender, System.EventArgs e)
		{		
			menuItem_AddItem_Click(sender, e);
		}

		private void menuItem_AddItem_Click(object sender, System.EventArgs e)
		{
			insertAddItem(true);
		}

		private void menuItem_insertItem_Click(object sender, System.EventArgs e)
		{
			insertAddItem(false);
		}

		public void deleteSelectedItem()
		{
			int			nServer = 0, nGroup = 0, nItem = 0;
			if(!getAndcheckServerPos(ref nServer)) return;
			if(!getAndCheckGroupPos(nServer, ref nGroup, true)) return;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			if(!getAndCheckItemPos(nGroup, ref nItem, opcServer)) return;
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			
			plcScanComm.plcScanCommMainClass.plcScanThreadPause(true);	// thread 를 일시중지
			if(comShareClass.bShareDllExist) comShareClass.comOpcShareOneDeleteItem(opcServer, opcGroup, nItem);	// COM 에서 하나의 아이템을 삭제
			if(opcGroup.bAsync) opcBasic.deleteOneItemAsync(opcServer, opcGroup, nItem);							// Subscription내의 하나의 아이템등록 삭제, 어레이를 지우기전에...
			opcGroup.arrItem.RemoveAt(nItem);			
			plcScanComm.plcScanCommMainClass.plcScanThreadPause(false);	// thread 를 재 시작

			this.treeView_Opc.Nodes[nServer].Nodes[nGroup].Nodes[nItem].Remove();
			fillViewListItem();
		}

		private void menuItem_DeleteItem_Click(object sender, System.EventArgs e)
		{
			deleteSelectedItem();
		}

		public void modifySelectedItem()
		{
			int		nServer = 0, nGroup = 0, nItem = 0;

			if(!getAndcheckServerPos(ref nServer)) return;
			if(!getAndCheckGroupPos(nServer, ref nGroup, true)) return;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			if(!getAndCheckItemPos(nGroup, ref nItem, opcServer)) return;
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			
			opcItemReadWriteClass opcItem = (opcItemReadWriteClass)opcGroup.arrItem[nItem];

			opcItemSearchDlgForm dialog = new opcItemSearchDlgForm(opcServer.m_server);
			dialog.bAdd = false;
			dialog.bUseDirectInput = true;
			dialog.bModify = true;
			dialog.textBox_ItemName.Text = opcItem.itemName;
			dialog.textBox_accessName.Text = opcItem.accessName;
			dialog.nServer = nServer;
			dialog.nGroup = nGroup;
			dialog.nItem = nItem;
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
			if(dialog.DialogResult == DialogResult.OK)
			{
				TreeNode node = this.treeView_Opc.Nodes[nServer].Nodes[nGroup].Nodes[nItem];
				node.Text = opcBasic.getTreeViewItemName(opcItem);
				fillViewListItem();
			}
		}

		private void menuItem_ModifyItem_Click(object sender, System.EventArgs e)
		{
			modifySelectedItem();
		}

		public void displayEventOrErrorMessage(string message, DateTime t)
		{
			ListViewItem	item = new ListViewItem();
			item.Text = message;
			item.SubItems.Add(t.ToString());
			this.listView_errorOrEventMessage.Items.Insert(0, item);
		}


		public void displayServerStatusList()
		{
			if(nDisplayServer >= opcBasic.arrOpcServer.Count) 
			{
				this.label_status.Text = "";
				return;
			}
			opcServerReadWriteClass	opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nDisplayServer];

            string hostName = (opcServer.hostName.Length > 0) ? opcServer.hostName + "." : "";// 2009-01-22 add
            this.label_status.Text = hostName + opcServer.serverName + " = ";//hostName + , 2009-01-22 add
			if(opcServer.m_server == null || opcServer.m_server.IsConnected == false) 
			{
				if(NetTools.Tools.IsLangKorean()) 
					label_status.Text += "연결종료 ( 연결 종료 시간 : ";
				else if(NetTools.Tools.IsLangJapanese()) 
					label_status.Text += "連結終了 ( 終了時刻 : ";
				else if(NetTools.Tools.IsLangChinese()) 
					label_status.Text += "连接结束 ( 连接结束时间 : ";
				else					     
					label_status.Text += "DisConnected ( Try Time : ";
			}
			else 
			{
				if(NetTools.Tools.IsLangKorean()) 
					label_status.Text += "연결 중 ( 연결 시작 시간 : ";
				else if(NetTools.Tools.IsLangJapanese()) 
					label_status.Text += "連結中 ( 連結時刻 : ";
				else if(NetTools.Tools.IsLangChinese()) 
					label_status.Text += "正在连接 ( 连接开始时间 : ";
				else						 
					label_status.Text += "Connected ( Try Time : ";
			}
			this.label_status.Text += opcServer.tryConnectTime.ToString() + " )";			
			
			if(bServerDisplay) 
			{
				if(NetTools.Tools.IsLangKorean()) 
					label_status.Text += "\n그룹 개수 = " + opcServer.arrGroup.Count;
				else						 
					label_status.Text += "\nGroup Count = " + opcServer.arrGroup.Count;
			}
			else
			{
				//if(opcServer.m_server == null || opcServer.m_server.IsConnected == false) return;
				if(nDisplayGroup >= opcServer.arrGroup.Count) return;
				opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nDisplayGroup];
				if(NetTools.Tools.IsLangKorean()) 
					label_status.Text += "\n그룹이름 = " + opcGroup.groupName;
				else						 
					label_status.Text += "\nGroup Name = " + opcGroup.groupName;

				label_status.Text += "         Status = ";
				if(opcGroup.bActive) label_status.Text += "Active         ";
				else                 label_status.Text += "InActive       ";
				this.label_status.Text += "Read Type = ";
				if(opcGroup.bAsync) label_status.Text += "Async       ";
				else                label_status.Text += "Sync        ";
				if(NetTools.Tools.IsLangKorean()) 
					label_status.Text += "읽기간격 = " + opcGroup.uPeriod.ToString() + " mSec";
				else						 
					label_status.Text += "Read Period = " + opcGroup.uPeriod.ToString() + " mSec";
			}			
		}

		private void treeView_Opc_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			int		nServer = -1, nGroup = -1;
			getAndcheckServerPos(ref nServer);						// 현재 디스플레이 서버를 찾는다.
			getAndCheckGroupPos(nServer, ref nGroup, false);		// 현재 디스플레이 그룹를 찾는다.			
			int depth = getTreeNodeDepth();

			if(depth == 0 || depth == -1)							// 서버단위 display
			{
				if(bServerDisplay && nServer == nDisplayServer) return;
			}
			else													// 그룹단위 display
			{
				if(bServerDisplay == false && nServer == nDisplayServer && nGroup == nDisplayGroup) return;
			}
			fillViewListItem();
		}

		public void connectDisConnectSelectedServer()
		{
			int		nServer = 0;
			if(!getAndcheckServerPos(ref nServer)) return;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			
			if(opcServer.m_server != null && opcServer.m_server.IsConnected == true)
			{
				opcReadWriteGroupLoopClass.opcServerDisConnection(opcServer, true);				
			}
			else
				opcReadWriteGroupLoopClass.opcServerConnection(opcServer);
		}

		private void menuItem_Connect_Click(object sender, System.EventArgs e)
		{
			connectDisConnectSelectedServer();
		}

		private void listView_Display_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			/*if(e.Button == MouseButtons.Right) 
			{
				if(this.listView_Display.SelectedItems.Count <= 0) return;
				Point pos = new Point(e.X, e.Y);
				this.contextMenu_Write.Show(this.listView_Display, pos);	// Write Test
			}
			if(e.Button == MouseButtons.Left && e.Clicks == 2) 
			{
				//oneItemTestWrite();											// direct Write Test
			}*/
		}

		int selectedItemNameToPos(string name)
		{
			//for(int i = 0; i < listView_Display.Items.Count; i++) 
			//{
			//	if(listView_Display.Items[i].Text == name) return i;
			//}
			return -1;
		}

		void messageGroupSelectionError()
		{
			if(NetTools.Tools.IsLangKorean())
				MessageBox.Show("그룹/아이템 선택이 잘못되었습니다.");
			else
				MessageBox.Show("Group/Item Selection error.");			
		}

		void oneItemTestWrite()
		{
			if(list.listHap <= list.currPos || this.nDisplayServer >= opcBasic.arrOpcServer.Count) return;

			int				nGroup = 0, nItem = 0, pos = list.currPos;
			//ListViewItem	item;

			//if(listView_Display.SelectedItems.Count <= 0) item = listView_Display.Items[0];				
			//else item = listView_Display.SelectedItems[0];

			//int pos = selectedItemNameToPos(item.Text);
			//if(pos == -1) 
			//{
			//	messageGroupSelectionError();
			//	return;
			//}

			if(this.bServerDisplay) 
			{
				if(!opcBasic.getTotalItemPosFromGroupPos(nDisplayServer, ref nGroup, ref nItem, pos))
				{
					messageGroupSelectionError();
					return;
				}
			}
			else 
			{
				nGroup = nDisplayGroup;
				nItem = pos;				
			}
			
			opcServerReadWriteClass opcServer;
			opcGroupReadWriteClass opcGroup;
			opcItemReadWriteClass opcItem;			
			try
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nDisplayServer];			
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];			
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[nItem];
			}
			catch
			{
				messageGroupSelectionError();
				return;
			}
			//if(opcItem.reuslt != ResultID.E_WRITEONLY && (opcItem.bFlag == false || opcItem.readData == null || opcBasic.isValueObjectSupportedType(opcItem.readData) == false))
			/*if((opcItem.bFlag == false || opcItem.readData == null || opcBasic.isValueObjectSupportedType(opcItem.readData) == false))
			{
				if(basicTool.IsLangKorean())
					MessageBox.Show("선택한 아이템은 쓸 수 없는 아이템 입니다.");
				else
					MessageBox.Show("Selected Item is Write Unable.");
				return;
			}*/
			
			opcWriteItemTestForm dialog = new opcWriteItemTestForm();
            dialog.hostName = opcServer.hostName;
			dialog.serverName = opcServer.serverName;
			dialog.groupName = opcGroup.groupName;
			dialog.itemName = opcItem.itemName;
			dialog.nServer = nDisplayServer;
			dialog.nGroup = nGroup;
			dialog.nItem = nItem;
			dialog.opcServer = opcServer;
			dialog.opcItem = opcItem;
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
		}
		
		void oneLineDraw(Graphics g, opcGroupReadWriteClass opcGroup, opcItemReadWriteClass opcItem, int i, int x, int y, int xGap)
		{
			ControlListViewHeader	head = (ControlListViewHeader)list.header[0];
			string					imsi;
			
			//if(bEqual && opcItem.bNewRead == false) continue;
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)list.fontY, opcGroup.groupName + "." + opcItem.itemName, Color.Black, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[1];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, getReadItemValueString(opcItem.readData), Color.Black, list.backColor, list.font, head.format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[2];
			if(opcGroup.bActive == false) imsi = "";
			//else if(opcItem.readData != null) imsi = opcItem.readData.GetType().ToString();// 2008-10-30 changed, 밑에있는 것 빼고
            else if (opcItem.bFlag && opcItem.readData != null) imsi = opcItem.readData.GetType().ToString();
			else imsi = "";//"Unknown";
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, imsi, Color.Black, list.backColor, list.font, head.format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[3];

			if(opcGroup.bActive == false) imsi = "InActive";					
			else if(opcItem.bFlag == false) imsi = opcItem.reuslt.ToString();
			else imsi = "Read OK";

            imsi += String.Format("({0})", opcItem.quality.ToString());

			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, imsi, Color.Black, list.backColor, list.font, head.format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[4];
            if (opcGroup.bActive == false) imsi = "";       // 2008-10-30 changed, 아래 것을 빼고
			//if(opcItem.bFlag == false || opcGroup.bActive == false) imsi = "";
			else imsi = opcItem.timeStamp.ToString();			
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, imsi, Color.Black, list.backColor, list.font, head.format);
			opcItem.bNewRead = false;
		}

		bool getDisplayOpcItem(out opcGroupReadWriteClass opcGroup, out opcItemReadWriteClass opcItem, int pos)
		{
			opcGroup = null;
			opcItem = null;
			if(nDisplayServer >= opcBasic.arrOpcServer.Count) return false;

			opcServerReadWriteClass	opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nDisplayServer];
			//bool					bEqual = (nCurrDisplayServer == nDisplayServer) ? true : false;

			if(bServerDisplay) 
			{
				int		curr = pos;
				for(int i = 0; i < opcServer.arrGroup.Count; i++) 
				{
					opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[i];					
					if(curr < opcGroup.arrItem.Count)			// 현재의 그룹 내에 존재
					{
						opcItem = (opcItemReadWriteClass)opcGroup.arrItem[curr];
						return true;
					}
					curr -= opcGroup.arrItem.Count;					
					//oneGroupListDisplay(ref pos, bEqual, opcGroup);
				}
				return false;
			}
			else 
			{
				if(nDisplayGroup >= opcServer.arrGroup.Count) return false;
				//if(bEqual) bEqual = (nCurrDisplayGroup == nDisplayGroup) ? true : false;// 동일한 서버일 때, 동일한 그룹인가를 검사
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nDisplayGroup];
				if(pos >= opcGroup.arrItem.Count) return false;
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[pos];
				return true;
				//oneGroupListDisplay(ref pos, bEqual, opcGroup);
			}
		}
		

		void DrawData(Graphics g, Rectangle r)
		{
			if(r.Bottom < list.headHeight) return;
			if(r.Top >= list.headHeight) DrawClass.gcls(g, r, list.backColor);
			else						 DrawClass.gcls(g, 0, list.headHeight, Width, r.Bottom, list.backColor);
			if(list.pageLineCount <= 0 || list.fontY <= 0 || list.listHap <= 0) return;
			
			int						pos, x, y = list.headHeight, xGap = (int)(list.fontX*0.25), endPos;
			opcGroupReadWriteClass	opcGroup;
			opcItemReadWriteClass	opcItem;

			endPos = list.pageLineCount+list.startPos+1;		// 1줄 더 그린다
			if(endPos > list.listHap) endPos = list.listHap;
			
			for(pos = list.startPos; pos < endPos; pos++, y += list.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+list.fontY < r.Top) continue;

				if(getDisplayOpcItem(out opcGroup, out opcItem, pos) == false) continue;
				x = list.startX;
				oneLineDraw(g, opcGroup, opcItem, pos+1, x, y, xGap);
			}
			nCurrDisplayServer = nDisplayServer;
			nCurrDisplayGroup = nDisplayGroup;
		}

		void onPaintMessage(Graphics g, Rectangle r)
		{
			DrawData(g, r);
		}

		private void menuItem_itemWriteValue_Click(object sender, System.EventArgs e)
		{
			oneItemTestWrite();
		}

		private void opcGroupItemReadRegisterForm_Closed(object sender, System.EventArgs e)
		{
			// 프로그램 전체가 없어지면서 이 펑션이 불러지지 않으므로 메인 폼에서 부른다.
		}

		public void opcGroupItemFormClosedWork() // 메인 폼에서 부르기 위한 함수
		{
			list.paintMessage -= new NetTools.ControlListView.OnEventPaintMessage(onPaintMessage);

			ControlListViewHeader head;
			bool					bChange = false;
			for(int i = 0; i < list.header.Count; i++) 
			{
				head = (ControlListViewHeader)list.header[i];
				if(headWidth[i] == head.width) continue;
				headWidth[i] = head.width;
				bChange = true;
			}
			if(bChange) AutoLibLocal.TotalConfig.AutoBaseListCtrlConfigSave(ref headWidth, "OpcClient", "GroupItemDisplay");
		}
		
		//private void panel3_SizeChanged(object sender, System.EventArgs e)
		//{
			//this.listView_errorOrEventMessage.Height = this.panel3.Height-20;
		//}
		
		
		
		
		
		

		

		
	}
}
