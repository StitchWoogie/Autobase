using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Opc;
using Opc.Da;
using Opc.Cpx;
using System.Resources;
using OpcClient.plcScanComm;
using NetTools;

namespace OpcClient
{
	/// <summary>
	/// Summary description for opcItemSearchDlgForm.
	/// </summary>
	public class opcItemSearchDlgForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_Done;
		private System.Windows.Forms.Button button_Register;
		private System.Windows.Forms.TreeView treeView_Item;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Opc.Da.Server	m_server;
		private TreeNode		itemNode = null;
		//private BrowseFilters	m_filters = null;
		public	int				nServer, nGroup, nItem;
		public	bool			bAdd, bModify;
		public	System.Windows.Forms.TextBox textBox_ItemName;
		private System.Windows.Forms.CheckBox checkBox_UseDirectInput;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.TextBox textBox_accessName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Panel panel1;
		private ArrayList		arrItemName = new ArrayList();
		private System.Windows.Forms.ListView listView_item;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		public  bool			bUseDirectInput = false;

		public opcItemSearchDlgForm(Opc.Da.Server server)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			m_server = server;
			ConnectCheckAndTreeMake();
			this.treeView_Item.ImageList = Resources.Instance.ImageList;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(opcItemSearchDlgForm));
            this.button_Done = new System.Windows.Forms.Button();
            this.button_Register = new System.Windows.Forms.Button();
            this.treeView_Item = new System.Windows.Forms.TreeView();
            this.textBox_ItemName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_UseDirectInput = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.listView_item = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.textBox_accessName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Done
            // 
            this.button_Done.AccessibleDescription = null;
            this.button_Done.AccessibleName = null;
            resources.ApplyResources(this.button_Done, "button_Done");
            this.button_Done.BackgroundImage = null;
            this.button_Done.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Done.Font = null;
            this.button_Done.Name = "button_Done";
            // 
            // button_Register
            // 
            this.button_Register.AccessibleDescription = null;
            this.button_Register.AccessibleName = null;
            resources.ApplyResources(this.button_Register, "button_Register");
            this.button_Register.BackgroundImage = null;
            this.button_Register.Font = null;
            this.button_Register.Name = "button_Register";
            this.button_Register.Click += new System.EventHandler(this.button_Register_Click);
            // 
            // treeView_Item
            // 
            this.treeView_Item.AccessibleDescription = null;
            this.treeView_Item.AccessibleName = null;
            resources.ApplyResources(this.treeView_Item, "treeView_Item");
            this.treeView_Item.BackgroundImage = null;
            this.treeView_Item.Font = null;
            this.treeView_Item.FullRowSelect = true;
            this.treeView_Item.HideSelection = false;
            this.treeView_Item.ItemHeight = 14;
            this.treeView_Item.Name = "treeView_Item";
            this.treeView_Item.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Item_AfterSelect);
            this.treeView_Item.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView_Item_AfterExpand);
            // 
            // textBox_ItemName
            // 
            this.textBox_ItemName.AccessibleDescription = null;
            this.textBox_ItemName.AccessibleName = null;
            resources.ApplyResources(this.textBox_ItemName, "textBox_ItemName");
            this.textBox_ItemName.BackgroundImage = null;
            this.textBox_ItemName.Font = null;
            this.textBox_ItemName.Name = "textBox_ItemName";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // checkBox_UseDirectInput
            // 
            this.checkBox_UseDirectInput.AccessibleDescription = null;
            this.checkBox_UseDirectInput.AccessibleName = null;
            resources.ApplyResources(this.checkBox_UseDirectInput, "checkBox_UseDirectInput");
            this.checkBox_UseDirectInput.BackgroundImage = null;
            this.checkBox_UseDirectInput.Font = null;
            this.checkBox_UseDirectInput.Name = "checkBox_UseDirectInput";
            this.checkBox_UseDirectInput.Click += new System.EventHandler(this.checkBox_UseDirectInput_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.listView_item);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Controls.Add(this.treeView_Item);
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // splitter1
            // 
            this.splitter1.AccessibleDescription = null;
            this.splitter1.AccessibleName = null;
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.BackgroundImage = null;
            this.splitter1.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitter1.Font = null;
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // listView_item
            // 
            this.listView_item.AccessibleDescription = null;
            this.listView_item.AccessibleName = null;
            resources.ApplyResources(this.listView_item, "listView_item");
            this.listView_item.BackgroundImage = null;
            this.listView_item.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView_item.Font = null;
            this.listView_item.FullRowSelect = true;
            this.listView_item.Name = "listView_item";
            this.listView_item.UseCompatibleStateImageBehavior = false;
            this.listView_item.View = System.Windows.Forms.View.Details;
            this.listView_item.DoubleClick += new System.EventHandler(this.listView_item_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // textBox_accessName
            // 
            this.textBox_accessName.AccessibleDescription = null;
            this.textBox_accessName.AccessibleName = null;
            resources.ApplyResources(this.textBox_accessName, "textBox_accessName");
            this.textBox_accessName.BackgroundImage = null;
            this.textBox_accessName.Font = null;
            this.textBox_accessName.Name = "textBox_accessName";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.checkBox_UseDirectInput);
            this.groupBox2.Controls.Add(this.textBox_accessName);
            this.groupBox2.Controls.Add(this.textBox_ItemName);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // opcItemSearchDlgForm
            // 
            this.AcceptButton = this.button_Register;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.button_Done;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Register);
            this.Controls.Add(this.button_Done);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "opcItemSearchDlgForm";
            this.ShowInTaskbar = false;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.opcItemSearchDlgForm_KeyDown);
            this.Load += new System.EventHandler(this.opcItemSearchDlgForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

	
		private void ConnectCheckAndTreeMake()
		{
			if(m_server == null) return;
			if(m_server.IsConnected == false) return;
			itemNode = new TreeNode(m_server.Name);			
			itemNode.Tag = m_server.Duplicate();

			TreeFill(itemNode);
			this.treeView_Item.Nodes.Clear();
			this.treeView_Item.Nodes.Add(itemNode);			
		}

		private void TreeFill(TreeNode node)
		{
			try
			{
				if (!IsServerNode(node)) return;

				// get the server for the current node.
				Opc.Da.Server server = (Opc.Da.Server)node.Tag;
				// connect to server if not already connected.
				if (!server.IsConnected)
				{
					server.Connect(FindConnectData(node));
				}

				// browse for top level elements.
				TreeOrListFillBrowse(node, false);
				TreeOrListFillBrowse(node, true);				
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private ConnectData FindConnectData(TreeNode node)
		{
			if (node != null)
			{
				if (node.Tag != null && node.Tag.GetType() == typeof(ConnectData))
				{
					return (ConnectData)node.Tag;
				}

				return FindConnectData(node.Parent);
			}
			return null;
		}

		private void AddBrowseElement(TreeNode parent, BrowseElement element)
		{
			if (element.IsItem && element.HasChildren == false) return;			
			// create the new node.
			TreeNode node = new TreeNode(element.Name);
			node.Tag = element;
			
			node.ImageIndex = node.SelectedImageIndex = Resources.IMAGE_BROWSE_NODE;

			// add a dummy node to force display of '+' symbol.		//if(element.HasChildren) node.Nodes.Add(new TreeNode());
			if (isNextNodeHasBranch(parent, element)) node.Nodes.Add(new TreeNode());
			// add properties
			if (element.Properties != null)
			{
				foreach (ItemProperty property in element.Properties) AddItemProperty(node, property);				
			}
			// add to parent.
			parent.Nodes.Add(node);
		}

		private void AddItemProperty(TreeNode parent, ItemProperty property)
		{
			if (property.ResultID.Succeeded())
			{
				// create the new node.
				TreeNode node = new TreeNode(property.Description);
			
				// select the icon.
				if (property.ItemName != null && property.ItemName != "")
				{
					node.ImageIndex = node.SelectedImageIndex = Resources.IMAGE_BROWSE_ITEM;
				}
				else
				{
					node.ImageIndex = node.SelectedImageIndex = Resources.IMAGE_ITEM_PROPERTY;
				}

				node.Tag = property;

				if (property.Value != null)
				{
					TreeNode child = new TreeNode(Opc.Convert.ToString(property.Value));
					child.ImageIndex = child.SelectedImageIndex = Resources.IMAGE_REQUEST_LIST;
					child.Tag = property.Value;
					node.Nodes.Add(child);

					if (property.Value.GetType().IsArray)
					{
						foreach (object element in (Array)property.Value)
						{
							TreeNode arrayChild = new TreeNode(Opc.Convert.ToString(element));
							arrayChild.ImageIndex = arrayChild.SelectedImageIndex = Resources.IMAGE_REQUEST_LIST;
							arrayChild.Tag = element;
							child.Nodes.Add(arrayChild);
						}
					}
				}
	
				// add to parent.
				parent.Nodes.Add(node);
			}
		}

		private bool IsServerNode(TreeNode node)
		{
			if (node == null ||node.Tag == null) return false;
			return typeof(Opc.Da.Server).IsInstanceOfType(node.Tag);
		}

		private Opc.Da.Server FindServer(TreeNode node)
		{
			if (node != null)
			{
				if (IsServerNode(node))
				{
					return (Opc.Da.Server)node.Tag;
				}

				return FindServer(node.Parent);
			}

			return null;
		}

		
		string getCurrentNodeString(TreeNode node)
		{
			string			nodeString = "";
			TreeNode		curr = node;
			BrowseElement	ex = null;
			int				pos = 0;
			
			while(true) 
			{
				if(curr == null || curr.Tag == null ||
					curr.Tag.GetType() != typeof(BrowseElement)) return nodeString;
			
				ex = (BrowseElement)curr.Tag;
				if(pos == 0) nodeString = ex.ItemName;
				else nodeString = ex.ItemName + "/" + nodeString;
				curr = curr.Parent;
				pos++;
				if(pos >= 200) return nodeString;
			}
		}

		private bool isNextNodeHasBranch(TreeNode parentNode, BrowseElement element)
		{
			try
			{
				if(element == null) return false;
				ItemIdentifier itemID = null;
				itemID = new ItemIdentifier(element.ItemPath, element.ItemName);					
				itemID.prevItemName = getCurrentNodeString(parentNode);
				
				BrowsePosition position = null;	// begin a browse.
				BrowseFilters  m_filters = new BrowseFilters();
				m_filters.BrowseFilter = Opc.Da.browseFilter.branch;				
				BrowseElement[] elements = m_server.Browse(itemID, m_filters, out position);

				if (elements == null || elements.Length <= 0) return false;
				return true;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
			return false;
		}

		private void TreeOrListFillBrowse(TreeNode node, bool bItem)
		{
			try
			{
				BrowseElement  parent = null;
				ItemIdentifier itemID = null;

				if (node.Tag != null && node.Tag.GetType() == typeof(BrowseElement))
				{
					parent = (BrowseElement)node.Tag;
					itemID = new ItemIdentifier(parent.ItemPath, parent.ItemName);					
					itemID.prevItemName = getCurrentNodeString(node.Parent);					
				}

				if(bItem == false) 
				{
					node.Nodes.Clear();	// add properties
					if (parent != null && parent.Properties != null)
					{
						foreach (ItemProperty property in parent.Properties)
						{
							AddItemProperty(node, property);
						}
					}
				}

				// begin a browse.
				BrowsePosition position = null;
				BrowseFilters  m_filters = new BrowseFilters();
				if(bItem) m_filters.BrowseFilter = Opc.Da.browseFilter.item;
				else	  m_filters.BrowseFilter = Opc.Da.browseFilter.branch;
				BrowseElement[] elements = m_server.Browse(itemID, m_filters, out position);

				// add children.
				if(bItem) 
				{
					listView_item.Items.Clear();
					if (elements == null) return;
					foreach (BrowseElement element in elements) 
					{
						if(element.IsItem)
						{
							ListViewItem	item = new ListViewItem();
							item.Text = element.Name;
							item.SubItems.Add(element.ItemName);
							this.listView_item.Items.Add(item);							
						}
					}
				}
				else 
				{
					if (elements != null)
					{
						foreach (BrowseElement element in elements)
						{
							AddBrowseElement(node, element);
						}					
						node.Expand();
					}
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private bool IsBrowseElementNode(TreeNode node)
		{
			if (node == null || node.Tag == null) return false;
			return (node.Tag.GetType() == typeof(BrowseElement));
		}

		private void treeView_Item_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			TreeNode node = e.Node;
		
			if(IsBrowseElementNode(node))
			{			// browse for children if not already fetched.
				if (node.Nodes.Count >= 1 && node.Nodes[0].Text == "")
				{
					//TreeFillBrowse(node);
					TreeOrListFillBrowse(node, false);
				}
				return;
			}
		}

		private void treeView_Item_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			TreeNode node = e.Node;		
			//if(IsBrowseElementNode(node) == false) return;
			TreeOrListFillBrowse(node, true);			
		}

		string getFullPathFromItemName()
		{		
			string		path;

			if(this.treeView_Item.SelectedNode == null) return null;
			path = this.treeView_Item.SelectedNode.FullPath;
			if(path == null || path.Length <= 0) return null;
			
			CommaBlockString	comma = new CommaBlockString();
			string				buf = "";
			
			comma.Set(path);
			comma.SetBlockCode('\\');
			comma.GetString(ref buf);
			if(buf == null || buf.Length <= 0) return null;
			if(path.Length <= buf.Length) return null;		// item name 이 없다
			return path.Substring(buf.Length + 1);
		}

		string convertItemNameSeperateCode(string itemName)
		{
			CommaBlockString	comma = new CommaBlockString();
			string				imsi = "", buf = "";
			int					pos = 0;

			comma.Set(itemName);
			comma.SetBlockCode('\\');

			while(true) 
			{
				comma.GetString(ref imsi);				
				if(pos == 0) buf = imsi;
				else		 buf += "/" + imsi;
				if(comma.IsEOS()) break;
				if(pos >= 50) break;
				pos ++;
			}
			return buf;			
		}

		bool checkEqualAccessName()
		{
			if(this.textBox_accessName.Text.Length <= 0) return false;

			if(opcBasic.arrOpcServer.Count <= nServer) return false;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			if(opcServer.arrGroup.Count <= nGroup) return false;
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			opcItemReadWriteClass opcItem;

			for(int i = 0; i < opcGroup.arrItem.Count; i++) 
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				if(opcItem.accessName == textBox_accessName.Text) 
				{
					if(bModify && i == nItem) continue;				// 수정이면서 동일한 이름은 상관없다
					return true;
				}
			}
			return false;
			
		}

		
		bool checkItemNameAndEqualAccessname(string itemName, bool bMessage)
		{
			if(itemName == null || itemName.Length <= 0)
			{
				if(bMessage) 
				{
					if(NetTools.Tools.IsLangKorean()) 
						MessageBox.Show("아이템 이름을 입력하지 않았습니다. 알맞은 이름을 입력해 주세요.");
					else						 
						MessageBox.Show("Input the item name.", "Name error");
				}
				return false;
			}			
			if(checkEqualAccessName())
			{
				if(bMessage) 
				{
					if(NetTools.Tools.IsLangKorean()) 
						MessageBox.Show("동일한 엑세스 이름이 존재합니다. 다른 이름을 입력해 주세요.");
					else						 
						MessageBox.Show("Same Access Name is already exists.");
				}
				return false;
			}
			return true;
		}

		void addInsertOneSelectedItem(opcServerReadWriteClass opcServer, opcGroupReadWriteClass opcGroup, string itemName, string accessName)
		{
			opcItemReadWriteClass opcItem = new opcItemReadWriteClass();
			opcItem.itemName = itemName;		// 동일한 아이템도 추가가능하므로 비교안함
			opcItem.accessName = accessName;
			opcItem.bFlag = false;
			opcItem.readData = 0.0;

			if(bAdd || opcGroup.arrItem.Count <= 0) 
			{
				opcGroup.arrItem.Add(opcItem);
				opcGroupItemReadRegisterForm.formThis.insertAddOpcTreeItem(nServer, nGroup, nItem, opcItem, true);
			}
			else 
			{
				if(nItem < 0 || opcGroup.arrItem.Count <= nItem) 
				{
					opcGroup.arrItem.Add(opcItem);
					opcGroupItemReadRegisterForm.formThis.insertAddOpcTreeItem(nServer, nGroup, nItem, opcItem, true);
				}
				else 
				{
					opcGroup.arrItem.Insert(nItem, opcItem);
					opcGroupItemReadRegisterForm.formThis.insertAddOpcTreeItem(nServer, nGroup, nItem, opcItem, false);
					nItem++;
				}
			}
			/*opcItem = new opcItemReadWriteClass();
				opcItem.itemName = itemName;		// 동일한 아이템도 추가가능하므로 비교안함
				opcItem.accessName = textBox_accessName.Text;
				opcItem.bFlag = false;
				opcItem.readData = 0.0;
				if(bAdd || opcGroup.arrItem.Count <= 0) 
				{
					opcGroup.arrItem.Add(opcItem);
					opcGroupItemReadRegisterForm.formThis.insertAddOpcTreeItem(nServer, nGroup, nItem, opcItem, true);
				}
				else 
				{
					if(nItem < 0 || opcGroup.arrItem.Count <= nItem) 
					{
						opcGroup.arrItem.Add(opcItem);
						opcGroupItemReadRegisterForm.formThis.insertAddOpcTreeItem(nServer, nGroup, nItem, opcItem, true);
					}
					else 
					{
						opcGroup.arrItem.Insert(nItem, opcItem);
						opcGroupItemReadRegisterForm.formThis.insertAddOpcTreeItem(nServer, nGroup, nItem, opcItem, false);
						nItem++;
					}
				}*/
			if(opcGroup.bAsync)	opcBasic.addOneItemAsync(opcServer, opcGroup, opcItem);		// OPC Item 하나를 추가 등록
            if (comShareClass.bShareDllExist) comShareClass.comOpcShareOneRegisterItem((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, itemName, opcItem.readData);	// COM 에 하나의 아이템 추가, 새로 추가되므로 값은 null
		}

		void setSelectedItemRegister()
		{
			if(opcBasic.arrOpcServer.Count <= nServer) return;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			if(opcServer.arrGroup.Count <= nGroup) return;
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			
			ListViewItem	item;
			plcScanComm.plcScanCommMainClass.plcScanThreadPause(true);	// thread 를 일시중지
			if(listView_item.SelectedItems.Count == 1)					// 하나의 아이템을 등록
			{
				item = listView_item.SelectedItems[0];
				if(checkItemNameAndEqualAccessname(item.SubItems[1].Text, false) == false) return;
				addInsertOneSelectedItem(opcServer, opcGroup, item.SubItems[1].Text, textBox_accessName.Text);
			}
			else														// 여러 개의 아이템을 등록
			{
				for(int i = 0; i < listView_item.SelectedItems.Count; i++) 
				{
					item = listView_item.SelectedItems[i];
					if(item.SubItems[1].Text.Length <= 0) continue;				
					addInsertOneSelectedItem(opcServer, opcGroup, item.SubItems[1].Text, "");// 여러 아이템일 경우 access name 는 항상 빈칸
				}
			}
			plcScanComm.plcScanCommMainClass.plcScanThreadPause(false);	// Thread를 재 시작			
		}
		

		bool setOneItemModifyOrRegister(string itemName, bool bMessageDisplay)
		{
			if(opcBasic.arrOpcServer.Count <= nServer) return false;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			if(opcServer.arrGroup.Count <= nGroup) return false;
			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			
			if(this.bModify)
			{
				if(opcGroup.arrItem.Count <= nItem) return false;
				opcItemReadWriteClass opcItem = (opcItemReadWriteClass)opcGroup.arrItem[nItem];
				if(opcItem.itemName == itemName && opcItem.accessName == textBox_accessName.Text) return true; // 수정된 내용이 없다
				if(checkItemNameAndEqualAccessname(itemName, bMessageDisplay) == false) return false;

				plcScanComm.plcScanCommMainClass.plcScanThreadPause(true);	// thread 를 일시중지
				if(comShareClass.bShareDllExist) comShareClass.comOpcShareOneDeleteItem(opcServer.serverName, opcGroup.groupName, opcItem.itemName);	// COM 에서 하나의 아이템을 삭제
				if(opcGroup.bAsync)	opcBasic.deleteOneItemAsync(opcServer, opcGroup, nItem);// 이전의 아이템을 삭제
				opcItem.itemName = itemName;
				opcItem.accessName = textBox_accessName.Text;
				if(opcGroup.bAsync)	opcBasic.addOneItemAsync(opcServer, opcGroup, opcItem);// OPC Item 하나를 추가 등록

                if (comShareClass.bShareDllExist) comShareClass.comOpcShareOneRegisterItem((opcGroup.pServer != null) ? opcGroup.pServer.accessName : "", opcGroup.groupName, opcItem.itemName, opcItem.readData);	// COM 에 하나의 아이템 추가
				plcScanComm.plcScanCommMainClass.plcScanThreadPause(false);	// Thread를 재 시작
			}
			else 
			{
				if(checkItemNameAndEqualAccessname(itemName, bMessageDisplay) == false) return false;			
				plcScanComm.plcScanCommMainClass.plcScanThreadPause(true);	// thread 를 일시중지
				addInsertOneSelectedItem(opcServer, opcGroup, itemName, textBox_accessName.Text);// 함수내에서 COM에 OPC Item 등록
				plcScanComm.plcScanCommMainClass.plcScanThreadPause(false);	// Thread를 재 시작
			}
			return true;
		}

		void CloseDialog()
		{
			this.DialogResult = DialogResult.OK;
			Close();
		}

		void newItemNameRegister()
		{
			if(this.bUseDirectInput) 
			{
				if(!setOneItemModifyOrRegister(this.textBox_ItemName.Text, true)) return;
				if(this.bModify) CloseDialog();
				return;
			}
			
			if(this.listView_item.SelectedItems.Count <= 0) 
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("등록할 아이템을 선택하지 않았습니다. 아이템을 선택해 주세요.", "선택 오류");
				else
					MessageBox.Show("Select a item to regist.", "Selection error");
				return;
			}			
			if(this.bModify) 
			{
				ListViewItem	item = this.listView_item.SelectedItems[0];
				if(!setOneItemModifyOrRegister(item.SubItems[1].Text, false)) return;
				CloseDialog();				
			}
			else
			{
				setSelectedItemRegister();
			}
		}


		private void button_Register_Click(object sender, System.EventArgs e)
		{
			newItemNameRegister();
		}

		private void opcItemSearchDlgForm_Load(object sender, System.EventArgs e)
		{
			this.checkBox_UseDirectInput.Checked = this.bModify;
			if(this.bModify) 
			{
				if(NetTools.Tools.IsLangKorean())
					this.button_Register.Text = "수정";
				else
					this.button_Register.Text = "Modify";
			}
			else 
			{
				if(NetTools.Tools.IsLangKorean())
					this.button_Register.Text = "등록";
				else
					this.button_Register.Text = "Register";
			}
			setEnableDisableObject();
		}

		void setEnableDisableObject()
		{
			if(bUseDirectInput) 
			{
				this.textBox_ItemName.Enabled = true;
				this.treeView_Item.Enabled = false;
			}
			else 
			{
				this.textBox_ItemName.Enabled = false;
				this.treeView_Item.Enabled = true;
			}
		}

		private void checkBox_UseDirectInput_Click(object sender, System.EventArgs e)
		{
			bUseDirectInput = this.checkBox_UseDirectInput.Checked;
			setEnableDisableObject();
		}
		

		private void listView_item_DoubleClick(object sender, System.EventArgs e)
		{
			if(this.listView_item.SelectedItems.Count <= 0) return;
			ListViewItem	item = this.listView_item.SelectedItems[0];
			if(!setOneItemModifyOrRegister(item.SubItems[1].Text, true)) return;
			if(this.bModify) CloseDialog();
		}

		private void opcItemSearchDlgForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter) newItemNameRegister();
		}
		
	
	
	}

	public class opcRealItemNameClass
	{
		public	string			name;
		public	string			realName;
	}
}
