using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AutoLibLocal;
using NetTools;
using AutoLib;
using System.Windows.Controls.Primitives;

namespace SilverlightDialogControl
{
    public class ControlBoxDigitalInputGo
    {
        public void Go(UserControl owner, string tag, sbyte bUseUserControl, string modulename, string dialog_title, string dialog_description)
        {
            TagDiClass di;
            int[] pos = new int[1];

            di = TagLib.GetStructDI(tag, ref pos);

            if ((di.wProtectFlags & EnumProtectFlag.CONTROL) > 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("이 태그는 제어 금지가 설정되어 있습니다.", di.tag, MessageBoxButton.OK);
                else if (Tools.IsLangChinese())
                    MessageBox.Show("这个标记已设置为禁止控制。", di.tag, MessageBoxButton.OK);
                else
                    MessageBox.Show("This tag protect to control.", di.tag, MessageBoxButton.OK);

                return;
            }

            di = TagLib.GetDirectTag(di);

            /*
            if (!SharedData.userInfo.IsHaveTagRight(di.tag))
            {
                string msg;
                if (NetTools.Tools.IsLangKorean())
                {
                    msg = String.Format("이 태그를 수동 작동할 권한이 없습니다.\n값을 변경하려면 권한이 있는 사용자 이름으로\nLOGIN 하시기 바랍니다.\n\n태그={0}", di.tag);
                    MessageBox.Show(msg, "작동 권한 없음");
                }
                else if (NetTools.Tools.IsLangChinese())
                {
                    msg = String.Format("没有权限把这个标记以非自动方式启动。\n想要更改值，请以有权限的用户名登录。\n\n标记={0}", di.tag);
                    MessageBox.Show(msg, "没有启动权限");
                }
                else
                {
                    msg = String.Format("Access to the TAG is denied.\nLOGIN another username to control the TAG\n\nTAG name={0}", di.tag);
                    MessageBox.Show(msg, "Access denied");
                }

                return;
            }*/

            if (di.cTagLinkType == 0 || di.cTagLinkType == 1)
            {
                // 이함수를 호출해 주어야 최초에 실행될때 태그가 없을 경우 오류를 찾을 수 있다.
                TagLib.GetStructDI(di.sSubOutDigital1, ref di.nSubOutDigital1);

                if (di.bUseAsOutput == 0 && di.nSubOutDigital1[0] == TagLib.TAG_NOT_FOUND)
                {
                    if (NetTools.Tools.IsLangKorean())
                        MessageBox.Show("설정된 디지털 출력태그(OUT1 또는 OUT2)가 없습니다.");
                    else
                        MessageBox.Show("OUT1 or OUT2 Tag is not defined.");

                    return;
                }
            }

            MyDialogDigitalInput dialog = new MyDialogDigitalInput(tag);

            dialog.Show(DialogStyle.Modal);
        }

    }

    class MyDialogDigitalInput : Dialog
    {
        string sTag;

        public MyDialogDigitalInput(string tag)
        {
            sTag = tag;
        }

        Button buttonON;
        Button buttonOFF;

        Grid childDigital;
        Grid gridBackground;

        protected override FrameworkElement GetContent()
        {
            /*
            ControlBoxAnalogInput dialog = new ControlBoxAnalogInput();

            Grid grid = new Grid() { Width = 500, Height = 500, };

            grid.Children.Add(dialog);

            return grid;*/
         
            
            // You could just use XamlReader to do everything except the event hookup.

            Grid gridback = new Grid() { Background=new SolidColorBrush(Color.FromArgb(50, 0, 0, 128)) };
            Grid grid = new Grid() { Width = 300, Height = 300 };

            gridback.Children.Add(grid);

            Border border = new Border() { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(6), Background = new SolidColorBrush(Color.FromArgb(150, 0, 0, 128)) };

            LinearGradientBrush lb = new LinearGradientBrush();
            lb.StartPoint = new Point(0, 0);
            lb.EndPoint = new Point(1, 0);
            GradientStop gs = new GradientStop();
            gs.Color = Colors.Yellow;
            gs.Offset = 0.0;
            lb.GradientStops.Add(gs);
            gs = new GradientStop();
            gs.Color = Colors.LightGray;
            gs.Offset = 1.0;
            lb.GradientStops.Add(gs);

            border.Background = lb;

            grid.Children.Add(border);

            grid.Children.Add(new TextBlock() { Text = "Dialog", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(8) });

            Button buttonCancel = new Button() { Width = 50, Height = 24, Content = "Cancel", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(8) };
            grid.Children.Add(buttonCancel);
            buttonCancel.Click += new RoutedEventHandler(button_Click);

            buttonOFF = new Button() { Width = 50, Height = 24, Content = "OFF", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(0, 0, 100, 40) };
            grid.Children.Add(buttonOFF);
            buttonOFF.Click += new RoutedEventHandler(buttonOFF_Click);

            buttonON = new Button() { Width = 50, Height = 24, Content = "ON", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(0, 0, 100, 80) };
            grid.Children.Add(buttonON);
            buttonON.Click += new RoutedEventHandler(buttonON_Click);

            Init();

            gridback.SizeChanged += new SizeChangedEventHandler(gridback_SizeChanged);

            childDigital = grid;
            gridBackground = gridback;

            orgx = childDigital.Width;
            orgy = childDigital.Height;

            return gridback;
        }

        double orgx;
        double orgy;

        void gridback_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //throw new NotImplementedException();

            Grid g = gridBackground;

            if (orgx > g.ActualWidth || orgy > g.ActualHeight)
            {
                ScaleTransform transform = new ScaleTransform();

                if (orgx > g.ActualWidth)
                {
                    transform.ScaleX = g.ActualWidth / orgx;
                    //childDigital.Width = g.ActualWidth;
                }
                else
                    transform.ScaleX = 1;

                if (orgy > g.ActualHeight)
                {
                    transform.ScaleY = g.ActualHeight / orgy;
                    //childDigital.Height = g.ActualHeight;
                }
                else
                    transform.ScaleY = 1;

                //childDigital.Width = orgx * transform.ScaleX;
                //childDigital.Height = orgy * transform.ScaleY;

                childDigital.RenderTransform = transform;
            }
            else
            {
                childDigital.Width = orgx;
                childDigital.Height = orgy;    
                childDigital.RenderTransform = null;
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClickOutside()
        {
            // Close();
        }

        int[] nTagPos;
        int[] nTagPosDO;

        void Init()
        {
            TagDiClass di = TagLib.GetStructDI(sTag, ref nTagPos);
            di = TagLib.GetDirectTag(di);

            //labelInputTag.Text = di.tag;
            //labelInputDes.Text = di.description;
            //labelOutputTag.Text = di.tag;
            //labelOutputDes.Text = di.description;

            if (di.cTagLinkType == 2)
            {

            }
            else
            {
                if (di.bUseAsOutput == 0 && di.sSubOutDigital1.Length > 0)
                {
                    TagDoClass dout = TagLib.GetStructDO(di.sSubOutDigital1, ref nTagPosDO);
                    dout = TagLib.GetDirectTag(dout);
                    //labelOutputTag.Text = dout.tag;
                    //labelOutputDes.Text = dout.description;
                }
            }

            if (di.curr == 0) buttonON.Focus();
            else buttonOFF.Focus();
        }

        protected override void OnLoad()
        {
            //this.textBoxValue.Select(0, textBoxValue.Text.Length);
            //this.textBoxValue.Focus();
        }

        void buttonON_Click(object sender, RoutedEventArgs e)
        {
            TagDiClass di = TagLib.GetStructDI(sTag, ref nTagPos);

            di = TagLib.GetDirectTag(di);

            if (!ConfirmMessageDI(di, 1))
            {
                Close();
                return;
            }

            SetValueAlarmInfo(di, 1);

            TagWrite.WriteCurrDI(di.tag, di, 1, true);
            if (di.cTagLinkType == 2) // 메모리 태그
            {
                //LibComNetServer.SendCommandTagValueChanged(di.tag, 1);
            }
            Close();
        }

        void buttonOFF_Click(object sender, RoutedEventArgs e)
        {
            TagDiClass di = TagLib.GetStructDI(sTag, ref nTagPos);

            di = TagLib.GetDirectTag(di);

            if (!ConfirmMessageDI(di, 1))
            {
                Close();
                return;
            }

            SetValueAlarmInfo(di, 0);

            TagWrite.WriteCurrDI(di.tag, di, 0, true);
            if (di.cTagLinkType == 2) // 메모리 태그
            {
                //LibComNetServer.SendCommandTagValueChanged(di.tag, 0);
            }
            Close();
        }

        bool ConfirmMessageDI(TagDiClass di, sbyte flag)
        {/*
            if (di.cConfirmCount <= 0) return true;

            string msg, title, buf;
            int i;

            for (i = 0; i < di.cConfirmCount && i < 5; i++)
            {
                if (Tools.IsLangKorean())
                {
                    if (flag == 1) buf = String.Format("{0} (운전)", di.desON);
                    else buf = String.Format("{0} (정지)", di.desOFF);
                    title = String.Format("출력 확인 {0}", i + 1);
                    msg = String.Format("     [{0}]]태그값을 {1} 할까요 ?     ", di.tag, buf);
                }
                else
                {
                    if (flag == 1) buf = String.Format("{0} (RUN)", di.desON);
                    else buf = String.Format("{0} (Stop)", di.desOFF);
                    title = String.Format("Confirm {0}", i + 1);
                    msg = String.Format("     Change [{0}] Tag status to {1}.     ", di.tag, buf);
                }
                if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo) != DialogResult.Yes) return false;
            }*/

            return true;
        }

        void SetValueAlarmInfo(TagDiClass di, sbyte flag)
        {/*
            string msg;

            if (di.cTagLinkType == 2)	// 메모리 태그
            {
                if (Tools.IsLangKorean())
                {
                    if (flag == 1) msg = "수동작동(메모리태그) ON";
                    else msg = "수동작동(메모리태그) OFF";
                }
                else
                {
                    if (flag == 1) msg = "Manual Operation(Memory Tag) ON";
                    else msg = "Manual Operation(Memory Tag) OFF";
                }

                AlarmUtil.AlarmDataSave(di, msg, EnumAlarmType.HAND_OPERATION);
            }
            else if (di.cTagLinkType == 3)
            {
                if (Tools.IsLangKorean())
                {
                    if (flag == 1) msg = "수동작동(간접태그) ON";
                    else msg = "수동작동(간접태그) OFF";
                }
                else
                {
                    if (flag == 1) msg = "Manual Operation(Indirect Tag) ON";
                    else msg = "Manual Operation(Indirect Tag) OFF";
                }

                AlarmUtil.AlarmDataSave(di, msg, EnumAlarmType.HAND_OPERATION);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    if (flag == 1) msg = "수동작동 ON";
                    else msg = "수동작동 OFF";
                }
                else
                {
                    if (flag == 1) msg = "Manual Operation ON";
                    else msg = "Manual Operation OFF";
                }

                AlarmUtil.AlarmDataSave(di, msg, EnumAlarmType.HAND_OPERATION);
            }
            */
        }

        /*
        public ControlBoxDigitalInput(string tag)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			sTag = tag;

			int[]				tagPos = new int[1];
			TagDiClass			di;
			
			di = TagLib.GetStructDI(tag, ref tagPos);
			di = TagLib.GetDirectTag(di);

			if(tagPos[0] != TagLib.TAG_NOT_FOUND) 
			{
				if(di.desON.Length > 0)	 buttonON.Text = di.desON;
				if(di.desOFF.Length > 0) buttonOFF.Text = di.desOFF;
			}			
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlBoxDigitalInput));
            this.buttonON = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelInputDes = new System.Windows.Forms.Label();
            this.labelInputTag = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.labelOutputTag = new System.Windows.Forms.Label();
            this.labelOutputDes = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonOFF = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonON
            // 
            this.buttonON.AccessibleDescription = null;
            this.buttonON.AccessibleName = null;
            resources.ApplyResources(this.buttonON, "buttonON");
            this.buttonON.BackgroundImage = null;
            this.buttonON.Font = null;
            this.buttonON.Name = "buttonON";
            this.buttonON.Click += new System.EventHandler(this.buttonON_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.labelInputDes);
            this.groupBox1.Controls.Add(this.labelInputTag);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // labelInputDes
            // 
            this.labelInputDes.AccessibleDescription = null;
            this.labelInputDes.AccessibleName = null;
            resources.ApplyResources(this.labelInputDes, "labelInputDes");
            this.labelInputDes.Font = null;
            this.labelInputDes.Name = "labelInputDes";
            // 
            // labelInputTag
            // 
            this.labelInputTag.AccessibleDescription = null;
            this.labelInputTag.AccessibleName = null;
            resources.ApplyResources(this.labelInputTag, "labelInputTag");
            this.labelInputTag.Font = null;
            this.labelInputTag.Name = "labelInputTag";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.labelOutputTag);
            this.groupBox2.Controls.Add(this.labelOutputDes);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // labelOutputTag
            // 
            this.labelOutputTag.AccessibleDescription = null;
            this.labelOutputTag.AccessibleName = null;
            resources.ApplyResources(this.labelOutputTag, "labelOutputTag");
            this.labelOutputTag.Font = null;
            this.labelOutputTag.Name = "labelOutputTag";
            // 
            // labelOutputDes
            // 
            this.labelOutputDes.AccessibleDescription = null;
            this.labelOutputDes.AccessibleName = null;
            resources.ApplyResources(this.labelOutputDes, "labelOutputDes");
            this.labelOutputDes.Font = null;
            this.labelOutputDes.Name = "labelOutputDes";
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // buttonOFF
            // 
            this.buttonOFF.AccessibleDescription = null;
            this.buttonOFF.AccessibleName = null;
            resources.ApplyResources(this.buttonOFF, "buttonOFF");
            this.buttonOFF.BackgroundImage = null;
            this.buttonOFF.Font = null;
            this.buttonOFF.Name = "buttonOFF";
            this.buttonOFF.Click += new System.EventHandler(this.buttonOFF_Click);
            // 
            // ControlBoxDigitalInput
            // 
            this.AcceptButton = this.buttonOFF;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonOFF);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonON);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControlBoxDigitalInput";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ControlBoxAnalogInput_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		int[] nTagPos= new int[1];
        int[] nTagPosDO;

		private void ControlBoxAnalogInput_Load(object sender, System.EventArgs e)
		{
			TagDiClass di = TagLib.GetStructDI(sTag, ref nTagPos);
			di = TagLib.GetDirectTag(di);

			labelInputTag.Text = di.tag;
			labelInputDes.Text = di.description;
			labelOutputTag.Text = di.tag;
			labelOutputDes.Text = di.description;

			if(di.cTagLinkType == 2) 
			{

			}
			else 
			{
                if (di.bUseAsOutput == 0 && di.sSubOutDigital1.Length > 0) 
				{
                    TagDoClass dout = TagLib.GetStructDO(di.sSubOutDigital1, ref nTagPosDO);
                    dout = TagLib.GetDirectTag(dout);
                    labelOutputTag.Text = dout.tag;
                    labelOutputDes.Text = dout.description;
				}
			}

			if(di.curr == 0)	buttonON.Select();
			else				buttonOFF.Select();
			
		}

		bool ConfirmMessageDI(TagDiClass di, sbyte flag) 
		{
			if(di.cConfirmCount <= 0)		return true;

			string msg, title, buf;
			int  i;

			for(i = 0; i < di.cConfirmCount && i < 5; i++) 
			{
				if(Tools.IsLangKorean()) 
				{
					if(flag == 1)	buf = String.Format("{0} (운전)", di.desON); 
					else			buf = String.Format("{0} (정지)", di.desOFF);		
					title = String.Format("출력 확인 {0}", i+1);
					msg = String.Format("     [{0}]]태그값을 {1} 할까요 ?     ", di.tag, buf);
				}
				else 
				{
					if(flag == 1) buf = String.Format("{0} (RUN)", di.desON);
					else           buf = String.Format("{0} (Stop)", di.desOFF);
					title = String.Format("Confirm {0}", i+1);	
					msg = String.Format("     Change [{0}] Tag status to {1}.     ", di.tag, buf);	
				}
				if(MessageBox.Show(msg, title, MessageBoxButtons.YesNo) != DialogResult.Yes)	return false;
			}

			return true;
		}

		void SetValueAlarmInfo(TagDiClass di, sbyte flag)
		{
			string msg;

			if(di.cTagLinkType == 2)	// 메모리 태그
			{
				if(Tools.IsLangKorean()) 
				{
					if(flag == 1)	msg = "수동작동(메모리태그) ON";
					else			msg = "수동작동(메모리태그) OFF";
				}
				else 
				{
					if(flag == 1)	msg = "Manual Operation(Memory Tag) ON";
                    else msg = "Manual Operation(Memory Tag) OFF";
				}

				AlarmUtil.AlarmDataSave(di, msg, EnumAlarmType.HAND_OPERATION);
			}
			else if(di.cTagLinkType == 3) 
			{
				if(Tools.IsLangKorean()) 
				{
					if(flag == 1)	msg = "수동작동(간접태그) ON";
					else			msg = "수동작동(간접태그) OFF";
				}
				else 
				{
                    if (flag == 1) msg = "Manual Operation(Indirect Tag) ON";
                    else msg = "Manual Operation(Indirect Tag) OFF";
				}

				AlarmUtil.AlarmDataSave(di, msg, EnumAlarmType.HAND_OPERATION);	
			}
			else 
			{
				if(Tools.IsLangKorean()) 
				{
					if(flag == 1)	msg = "수동작동 ON";
					else			msg = "수동작동 OFF";
				}
				else 
				{
                    if (flag == 1) msg = "Manual Operation ON";
                    else msg = "Manual Operation OFF";
				}

				AlarmUtil.AlarmDataSave(di, msg, EnumAlarmType.HAND_OPERATION);
			}
		}

		private void buttonON_Click(object sender, System.EventArgs e)
		{
			TagDiClass di = TagLib.GetStructDI(sTag, ref nTagPos);

			di = TagLib.GetDirectTag(di);

			if(!ConfirmMessageDI(di, 1)) 
			{
				Close();
				return;
			}

			SetValueAlarmInfo(di, 1);

			TagWrite.WriteCurrDI(di.tag, di, 1, true);
			if(di.cTagLinkType == 2) // 메모리 태그
			{
				LibComNetServer.SendCommandTagValueChanged(di.tag, 1);
			}
			Close();
		}

		private void buttonOFF_Click(object sender, System.EventArgs e)
		{
			TagDiClass di = TagLib.GetStructDI(sTag, ref nTagPos);

			di = TagLib.GetDirectTag(di);

			if(!ConfirmMessageDI(di, 1)) 
			{
				Close();
				return;
			}

			SetValueAlarmInfo(di, 0);

			TagWrite.WriteCurrDI(di.tag, di, 0, true);
			if(di.cTagLinkType == 2) // 메모리 태그
			{
				LibComNetServer.SendCommandTagValueChanged(di.tag, 0);
			}
			Close();
		}
        */
    }


}
