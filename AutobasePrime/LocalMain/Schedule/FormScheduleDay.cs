using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using NetTools.OldDefine;
using AutoLib;
using DialogHoliday;
using PublicStudioLocalMain.Schedule;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormScheduleDay.
	/// </summary>
	public class FormScheduleDay : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TreeView m_tree;
		private System.Windows.Forms.TreeView m_treeOrder; 
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;

		public FormScheduleDay()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScheduleDay));
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_tree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.m_treeOrder = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.m_tree);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // m_tree
            // 
            this.m_tree.AccessibleDescription = null;
            this.m_tree.AccessibleName = null;
            resources.ApplyResources(this.m_tree, "m_tree");
            this.m_tree.BackgroundImage = null;
            this.m_tree.Font = null;
            this.m_tree.ImageList = this.imageList1;
            this.m_tree.Name = "m_tree";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            this.imageList1.Images.SetKeyName(13, "");
            this.imageList1.Images.SetKeyName(14, "");
            this.imageList1.Images.SetKeyName(15, "");
            this.imageList1.Images.SetKeyName(16, "");
            this.imageList1.Images.SetKeyName(17, "");
            this.imageList1.Images.SetKeyName(18, "");
            this.imageList1.Images.SetKeyName(19, "");
            this.imageList1.Images.SetKeyName(20, "");
            this.imageList1.Images.SetKeyName(21, "");
            this.imageList1.Images.SetKeyName(22, "");
            this.imageList1.Images.SetKeyName(23, "");
            this.imageList1.Images.SetKeyName(24, "");
            this.imageList1.Images.SetKeyName(25, "");
            this.imageList1.Images.SetKeyName(26, "");
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Controls.Add(this.m_treeOrder);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // m_treeOrder
            // 
            this.m_treeOrder.AccessibleDescription = null;
            this.m_treeOrder.AccessibleName = null;
            resources.ApplyResources(this.m_treeOrder, "m_treeOrder");
            this.m_treeOrder.BackgroundImage = null;
            this.m_treeOrder.Font = null;
            this.m_treeOrder.ImageList = this.imageList1;
            this.m_treeOrder.Name = "m_treeOrder";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // FormScheduleDay
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = null;
            this.Name = "FormScheduleDay";
            this.Load += new System.EventHandler(this.FormScheduleDay_Load);
            this.SizeChanged += new System.EventHandler(this.FormScheduleDay_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		ArrayList blockGo = new ArrayList();
		DateTime tTime;

		private void FormScheduleDay_Load(object sender, System.EventArgs e)
		{
			tTime = DateTime.Now;
			UpdateLabel();
		}

		void UpdateLabel()
		{
			string buf;
			if(Tools.IsLangKorean()) 
			{
				buf = String.Format("{0}-{1:00}-{2:00} 스케쥴 제어 목록", tTime.Year, tTime.Month, tTime.Day);
			}
			else if(Tools.IsLangJapanese()) 
			{
				buf = String.Format("{0}-{1:00}-{2:00} スケジュール制御リスト", tTime.Year, tTime.Month, tTime.Day);
			}
			else if(Tools.IsLangChinese()) 
			{
				buf = String.Format("{0}-{1:00}-{2:00} 计划表控制列表", tTime.Year, tTime.Month, tTime.Day);
			}
            else if (Tools.IsLangVietnamese())
            {
                buf = String.Format("{0}-{1:00}-{2:00} Danh sách điều khiển lịch", tTime.Year, tTime.Month, tTime.Day);
            }
			else 
			{
				buf = String.Format("{0}-{1:00}-{2:00} Schedule control lists", tTime.Year, tTime.Month, tTime.Day);
			}

			this.label1.Text = buf;

			if(Tools.IsLangKorean()) 
			{
				buf = "시간별 제어 목록";
			}
			else if(Tools.IsLangJapanese()) 
			{
				buf = "時間別制御リスト";
			}
			else if(Tools.IsLangChinese()) 
			{
				buf = "按时间控制列表";
			}
            else if (Tools.IsLangVietnamese())
            {
                buf = "Phân loại theo giờ";
            }
			else 
			{
				buf = "Sort by hour";
			}

			this.label2.Text = buf;
		}

		void AddModelTree(TreeNode tree, SCHEDULE_MODEL_STRUCT model, DateTime t_target)
		{
			int m;
			SCHEDULE_MODEL_ITEM_STRUCT item;
			string buf;
			string str;
	
			for(m = 0; m < model.blockItem.Count; m++) 
			{
				item = (SCHEDULE_MODEL_ITEM_STRUCT)model.blockItem[m];

                Schedule.CalculateToSunControlTime(item, t_target); // Tree에 등록하기 전에 Sunrist/Sunset일 경우는 시간을 계산한다. 

				buf = String.Format("{0:00}:{1:00}   ", item.hour, item.minute);
				str = buf;

				Schedule.ModelMakeString(item, out buf);
				str += buf;

				TreeNode node = new TreeNode(str, item.hour%24, item.hour%24);
				tree.Nodes.Add(node);
			}
		}

		static int sort_function(object b1, object b2)
		{
			SCHEDULE_GO go1 = (SCHEDULE_GO)b1;
			SCHEDULE_GO go2 = (SCHEDULE_GO)b2;

			return (go2.hour*60+go2.min)-(go1.hour*60+go1.min);
		}

		void FillTreeOrder()
		{
			Tools.BlockSort(blockGo, new Tools.BlockSortFunction(sort_function));
	
			m_treeOrder.Nodes.Clear();

			SCHEDULE_GO go;
			string buf;
			int l;
	
			for(l = 0; l < blockGo.Count; l++) 
			{
				go = (SCHEDULE_GO)blockGo[l];

				if(Tools.IsLangKorean())
					buf = String.Format("{0:00}:{1:00} {2} (모델:{3})", go.hour, go.min, go.str, go.model);
				else if(Tools.IsLangJapanese())
					buf = String.Format("{0:00}:{1:00} {2} (モデル:{3})", go.hour, go.min, go.str, go.model);
				else if(Tools.IsLangChinese())
					buf = String.Format("{0:00}:{1:00} {2} (模型:{3})", go.hour, go.min, go.str, go.model);
				else 
					buf = String.Format("{0:00}:{1:00} {2} (Model:{3})", go.hour, go.min, go.str, go.model);

				TreeNode node = new TreeNode(buf, go.hour%24, go.hour%24);
				m_treeOrder.Nodes.Add(node);
			}
		}

		// 다시 불러준다.
		public void ReLoad()
		{
			SYSTEMTIME t = new SYSTEMTIME();
			t.Set(tTime);
			ChangeDay(t);
		}

		public void ChangeDay(SYSTEMTIME t)
		{
			tTime = t.ToDateTime();

			m_tree.Nodes.Clear();
			blockGo.Clear();

			int  week = TimeUtil.GetWeekDay(tTime.Year, tTime.Month, tTime.Day);
			string text_holiday;
			string text_special;
			SCHEDULE_STRUCT sc;
			string buf;
			SCHEDULE_MODEL_STRUCT model;
			int model_pos;
	
			bool bHoliday = Holiday.IsHoliday(tTime.Year, tTime.Month, tTime.Day, out text_holiday);
			bool bSpecial = Holiday.IsSpecialDay(tTime.Year, tTime.Month, tTime.Day, out text_special);

			bool retn = Schedule.ScheduleGetDayStructure(tTime.Year, tTime.Month, tTime.Day, out sc, week, bHoliday, bSpecial);  

			TreeNode hTree;
			int l;

			if(retn) 
			{
				NAME_STRUCT name;

				if(Tools.IsLangKorean()) 
				{
					buf = String.Format("고정 스케쥴 ({0})", sc.title);
				}
				else if(Tools.IsLangJapanese()) 
				{
					buf = String.Format("固定スケジュール ({0})", sc.title);
				}
				else if(Tools.IsLangChinese()) 
				{
					buf = String.Format("固定计划表 ({0})", sc.title);
				}
                else if (Tools.IsLangVietnamese())
                {
                    buf = String.Format("Lịch cố định ({0})", sc.title);
                }
				else 
				{
					buf = String.Format("Fixed Schedule ({0})", sc.title);
				}

				hTree = new TreeNode(buf, 26, 26);
				m_tree.Nodes.Add(hTree);
		
				for(l = 0; l < sc.blockName.Count; l++) 
				{
					name = (NAME_STRUCT)sc.blockName[l];
					if(Tools.IsLangKorean()) 
					{
						buf = String.Format("운전모델({0})", name.title);
					}
					else if(Tools.IsLangJapanese()) 
					{
						buf = String.Format("運転モデル({0})", name.title);
					}
					else if(Tools.IsLangChinese()) 
					{
                        buf = String.Format("操作模型({0})", name.title);
					}
					else 
					{
						buf = String.Format("Model({0})", name.title);
					}
			
					bool model_retn = Schedule.GetModelStructure(name.title, out model, out model_pos);

					if(model_retn)
						buf = String.Format("{0} ({1})", model.title, model.description);
					else 
					{
						if(Tools.IsLangKorean()) 
						{
							buf = String.Format("{0} (오류:없는모델)", name.title);
						}
						else if(Tools.IsLangChinese()) 
						{
							buf = String.Format("{0} (错误:此模型不存在)", name.title);
						}
						else 
						{
							buf = String.Format("{0} (Error:Model Not exist)", name.title);
						}
					}

					TreeNode hTree2 = new TreeNode(buf, model_retn?25:24, model_retn?25:24);
					hTree.Nodes.Add(hTree2);

					if(!model_retn)	continue;

					AddModelTree(hTree2, model, tTime);
					CheckEngineSchedule.AddModelToGoBlock(blockGo, model, model_pos, tTime);
				}

				hTree.Expand();
			}
			else 
			{
				if(Tools.IsLangKorean()) 
				{
					m_tree.Nodes.Add(new TreeNode("고정 스케쥴 (없음)", 26, 26));
				}
				else if(Tools.IsLangJapanese()) 
				{
					m_tree.Nodes.Add(new TreeNode("固定スケジュール (ない)", 26, 26));
				}
				else if(Tools.IsLangChinese()) 
				{
					m_tree.Nodes.Add(new TreeNode("固定计划表 (无)", 26, 26));
				}
                else if (Tools.IsLangVietnamese())
                {
                    m_tree.Nodes.Add(new TreeNode("Lịch cố định (None)", 26, 26));
                }
				else 
				{
					m_tree.Nodes.Add(new TreeNode("Fixed Schedule (None)", 26, 26));
				}
			}

			if(Tools.IsLangKorean()) 
			{
				hTree = new TreeNode("추가 스케쥴", 26, 26);
				m_tree.Nodes.Add(hTree);
			}
			else if(Tools.IsLangJapanese()) 
			{
				hTree = new TreeNode("追加スケジュール", 26, 26);
				m_tree.Nodes.Add(hTree);
			}
			else if(Tools.IsLangChinese()) 
			{
				hTree = new TreeNode("添加计划表", 26, 26);
				m_tree.Nodes.Add(hTree);
			}
            else if (Tools.IsLangVietnamese())
            {
                hTree = new TreeNode("Lịch bổ xung", 26, 26);
                m_tree.Nodes.Add(hTree);
            }
			else 
			{
				hTree = new TreeNode("Additional Schedule", 26, 26);
				m_tree.Nodes.Add(hTree);
			}

			SCHEDULE_ADDITIONAL add;
			string  string_add;
	
			for(l = 0; l < Schedule.blockScheduleAdditional.Count; l++) 
			{
				add = (SCHEDULE_ADDITIONAL)Schedule.blockScheduleAdditional[l];
				if(Schedule.IsDayInclude(tTime, add, week, bHoliday, bSpecial)) 
				{
					ScheduleLib.MakeStringScheduleAdditional(out string_add, add);

					if(Tools.IsLangKorean())
						buf = String.Format("{0} ({1}) 모델:{2}", add.title, string_add, add.model);
					else if(Tools.IsLangJapanese())
						buf = String.Format("{0} ({1}) モデル:{2}", add.title, string_add, add.model);
					else if(Tools.IsLangChinese())
						buf = String.Format("{0} ({1}) 模型:{2}", add.title, string_add, add.model);
					else
						buf = String.Format("{0} ({1}) Model:{2}", add.title, string_add, add.model);

					TreeNode hTree2 = new TreeNode(buf, 25, 25);
					hTree.Nodes.Add(hTree2);

					bool model_retn = Schedule.GetModelStructure(add.model, out model, out model_pos);

					if(!model_retn)	continue;

					AddModelTree(hTree2, model, tTime);
					CheckEngineSchedule.AddModelToGoBlock(blockGo, model, model_pos, tTime);
				}
			}

			hTree.Expand();

			FillTreeOrder();

			Invalidate();

			UpdateLabel();
		}

		private void FormScheduleDay_SizeChanged(object sender, System.EventArgs e)
		{
			this.panel1.Width = this.ClientRectangle.Width/2;
		}
	}
}

