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
    public class ControlBoxAnalogInputGo
    {
        public delegate void DelegateUserControl(string filename, string tag, string dialog_title, string dialog_description, string dialog_minvalue, string dialog_maxvalue);
        public static DelegateUserControl procUserControl = null;

        public void Go(UserControl owner, string tag)
        {
            Go(owner, tag, 0, "", "?", "?");
        }

        public void Go(UserControl owner, string tag, sbyte bUseUserControl, string modulename, string dialog_title, string dialog_description)
        {
            TagAiClass ai;
            int[] pos = new int[1];

            ai = TagLib.GetStructAI(tag, ref pos);

            if ((ai.wProtectFlags & EnumProtectFlag.CONTROL) > 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("이 태그는 제어 금지가 설정되어 있습니다.", ai.tag, MessageBoxButton.OK);
                else if (Tools.IsLangChinese())
                    MessageBox.Show("这个标记已设置为禁止控制。", ai.tag, MessageBoxButton.OK);
                else
                    MessageBox.Show("This tag protect to control.", ai.tag, MessageBoxButton.OK);

                return;
            }

            if (!SharedData.userInfo.IsHaveTagRight(ai.tag))
            {
                string msg;

                if (NetTools.Tools.IsLangKorean())
                {
                    msg = String.Format("이 태그를 수동 작동할 권한이 없습니다.\n값을 변경하려면 권한이 있는 사용자 이름으로\nLOGIN 하시기 바랍니다.\n\n태그={0}", ai.tag);
                    MessageBox.Show(msg, "작동 권한 없음", MessageBoxButton.OK);
                }
                else if (NetTools.Tools.IsLangChinese())
                {
                    msg = String.Format("没有权限把这个标记以非自动方式启动。\n想要更改值，请以有权限的用户名登录。\n\n标记={0}", ai.tag);
                    MessageBox.Show(msg, "没有启动权限", MessageBoxButton.OK);
                }
                else
                {
                    msg = String.Format("Access to the TAG is denied.\nLOGIN another username to control the TAG\n\nTAG name={0}", ai.tag);
                    MessageBox.Show(msg, "Access denied", MessageBoxButton.OK);
                }

                return;
            }

            if (ai.cTagLinkType == 0 || ai.cTagLinkType == 1)
            {
                // 이함수를 호출해 주어야 최초에 실행될때 태그가 없을 경우 오류를 찾을 수 있다.
                TagLib.GetStructAI(ai.sSubOutAnalogSP, ref ai.nSubOutAnalogSP);

                if (ai.bUseAsOutput == 0 && ai.nSubOutAnalogSP[0] == TagLib.TAG_NOT_FOUND)
                {
                    if (NetTools.Tools.IsLangKorean())
                        MessageBox.Show("AO-SV 부분에 태그를 설정하지 않았습니다.");
                    else if (NetTools.Tools.IsLangChinese())
                        MessageBox.Show("AO-SV 部分没有设置标记。");
                    else
                        MessageBox.Show("AO SetPoint Tag is not defined.");

                    return;
                }
            }

            if (bUseUserControl == 1 && procUserControl != null)
            {
                //string filename = MakeFilePath.Graphic(modulename);
                procUserControl(modulename, tag, dialog_title, dialog_description, ai.fBase.ToString(), ai.fFull.ToString());
            }
            else
            {
                if (ConfigViewMain.bUserControlBoxUseAnalogModule && procUserControl != null)
                {
                    //string filename = MakeFilePath.Graphic(ConfigViewMain.sUserControlBoxAnalogModule);
                    procUserControl(modulename, tag, "Tag : " + ai.tag, ai.description, ai.fBase.ToString(), ai.fFull.ToString());
                }
                else
                {
                    MyDialog dlg = new MyDialog(tag);

                    dlg.Show(DialogStyle.Modal);
                }
            }

        }
    }

    class MyDialog : Dialog
    {
        TextBox textBoxValue = new TextBox();
        PageControlAnalog childAnalog;
        Grid gridBackground;

        protected override FrameworkElement GetContent()
        {
            PageControlAnalog dialog = new PageControlAnalog(sTag);

            dialog.procClose = new PageControlAnalog.DelegateClose(Close);

            Grid grid = new Grid() { Background=new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),};

            grid.Children.Add(dialog);

            grid.SizeChanged += new SizeChangedEventHandler(grid_SizeChanged);
            grid.KeyDown += new KeyEventHandler(grid_KeyDown);

            childAnalog = dialog;
            gridBackground = grid;

            return grid; 
        }

        void grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid g = gridBackground;

            if (childAnalog.Width > g.ActualWidth || childAnalog.Height > g.ActualHeight)
            {
                ScaleTransform transform = new ScaleTransform();

                if (childAnalog.Width > g.ActualWidth)
                    transform.ScaleX = g.ActualWidth / childAnalog.Width;
                else
                    transform.ScaleX = 1;

                if (childAnalog.Height > g.ActualHeight)
                    transform.ScaleY = g.ActualHeight / childAnalog.Height;
                else
                    transform.ScaleY = 1;

                childAnalog.LayoutRoot.RenderTransform = transform;
            }
            else
            {
                childAnalog.LayoutRoot.RenderTransform = null;
            }
        }

        /*
        void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }*/

        string sTag;

        public MyDialog(string tag)
        {
            sTag = tag;
        }

        /*
        protected override void OnClickOutside()
        {
            // Close();
        }*/

        //int[] nTagPos;
        //int[] nTagPosAO;
        /*
        void Init()
        {
            TagAiClass ai = TagLib.GetStructAI(sTag, ref nTagPos);
            ai = TagLib.GetDirectTag(ai);

            float fFull = ai.fFull;
            float fBase = ai.fBase;

            if (fFull < fBase)
            {
                string msg = String.Format("{0} 태그의 Full 값이 Base 값보다 작습니다.", ai.tag);
                MessageBox.Show(msg, "Full/Base 범위", MessageBoxButton.OK);
                fFull = 100;
                fBase = 0;
            }

            textBoxValue.Text = TagUtil.AiValueToStringOnlyPoint(ai, ai.curr);

            if (ai.cTagLinkType == 2)
            {
            }
            else
            {
                if (ai.bUseAsOutput == 0 && ai.sSubOutAnalogSP.Length > 0)
                {
                    TagAoClass ao = TagLib.GetStructAO(ai.sSubOutAnalogSP, ref nTagPosAO);
                    ao = TagLib.GetDirectTag(ao);
                    //labelOutputTag.Text = ao.tag;
                    //labelOutputDes.Text = ao.description;
                }
            }

            
        }
        

        protected override void OnLoad()
        {
            this.textBoxValue.Select(0, textBoxValue.Text.Length);
            this.textBoxValue.Focus();
        }

        void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            TagAiClass ai = TagLib.GetStructAI(sTag, ref nTagPos);
            ai = TagLib.GetDirectTag(ai);

            double value = ConvertTool.ToDouble(textBoxValue.Text);

            if (!ConfirmMessageAI(ai, value))
            {
                Close();
                return;
            }

            SetValueAlarmInfo(ai, ai.curr, value);

            TagWrite.WriteCurrAI(ai.tag, ai, value, true);

            if (ai.cTagLinkType == 2) // 메모리 태그
            {
                //LibComNetServer.SendCommandTagValueChanged(ai.tag, value);
            }

            Close();
        }

        bool ConfirmMessageAI(TagAiClass ai, double value)
        {
            
            if (ai.cConfirmCount <= 0) return true;

            string msg, title;
            int i;

            for (i = 0; i < ai.cConfirmCount && i < 5; i++)
            {
                if (Tools.IsLangKorean())
                {
                    title = String.Format("출력 확인 {0}", i + 1);
                    msg = String.Format("[{0}]태그의 값을 [{1}]로 바꾸시겠습니까?", ai.tag, value);
                }
                else if (Tools.IsLangChinese())
                {
                    title = String.Format("输出确认 {0}", i + 1);
                    msg = String.Format("要把[{0}]标记值改为[{1}]吗？", ai.tag, value);
                }
                else
                {
                    title = String.Format("Confirm {0}", i + 1);
                    msg = String.Format("Change [{0}] Tag value to [{1}]?", ai.tag, value);
                }
                if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo) != DialogResult.Yes) return false;
            }

            return true;
        }

        void SetValueAlarmInfo(TagAiClass ai, double old_val, double new_val)
        {
            
            string msg;
            string s_old;
            string s_new;

            s_old = TagUtil.AiValueToStringOnlyPoint(ai, old_val);
            s_new = TagUtil.AiValueToStringOnlyPoint(ai, new_val);

            if (Tools.IsLangKorean())
                msg = String.Format("수동 설정치 변경 ({0}->{1})", s_old, s_new);
            else
                msg = String.Format("Set Value Changed ({0}->{1})", s_old, s_new);

            AlarmUtil.AlarmDataSave(ai, msg, EnumAlarmType.HAND_OPERATION);
        }
        
        
        int[] nTagPos;
		int[] nTagPosAO;

		private void ControlBoxAnalogInput_Load(object sender, System.EventArgs e)
		{
			TagAiClass ai = TagLib.GetStructAI(sTag, ref nTagPos);
			ai = TagLib.GetDirectTag(ai);

            float fFull = ai.fFull;
            float fBase = ai.fBase;

            if (fFull < fBase)
            {
                string msg = String.Format("{0} 태그의 Full 값이 Base 값보다 작습니다.", ai.tag);
                MessageBox.Show(msg, "Full/Base 범위");
                fFull = 100;
                fBase = 0;
            }

			textBoxValue.Text = TagUtil.AiValueToStringOnlyPoint(ai, ai.curr);
			textBoxMin.Text = TagUtil.AiValueToStringOnlyPoint(ai, fBase);
			textBoxMax.Text = TagUtil.AiValueToStringOnlyPoint(ai, fFull);

			labelInputTag.Text = ai.tag;
			labelInputDes.Text = ai.description;
			labelOutputTag.Text = ai.tag;
			labelOutputDes.Text = ai.description;

			if(ai.cTagLinkType == 2) 
			{
			}
			else 
			{
                if(ai.bUseAsOutput == 0 && ai.sSubOutAnalogSP.Length > 0) 
				{
					TagAoClass ao = TagLib.GetStructAO(ai.sSubOutAnalogSP, ref nTagPosAO);
					ao = TagLib.GetDirectTag(ao);
					labelOutputTag.Text = ao.tag;
					labelOutputDes.Text = ao.description;
				}
			}

			long max;
			long min;

			max = (long)(fFull/ai.fScrollUnit+hScrollBar.LargeChange-1);
			min = (long)(fBase/ai.fScrollUnit);

			if(min < int.MinValue)	min = int.MinValue;
			if(max > int.MaxValue)	max = int.MaxValue;

			if((max-min) > (int.MaxValue-3))	// 스크롤 min max 의 차이가 21억이 넘으면 오류
			{
				min = max-(int.MaxValue-3);	
			}

			if(hScrollBar.Enabled) 
			{
				hScrollBar.Maximum = (int)max;
				hScrollBar.Minimum = (int)min;

				int val = (int)(ai.curr/ai.fScrollUnit);



				if(val < min)	val = (int)min;
				if(val > max)	val = (int)max;

				hScrollBar.Value = val;
			}

			// 마우스 누를 때 이 대화상자를 호출하면 입력기의 역역을 선택해도 선택이 되지 않는다.
			// 이럴때는 마우스 뗄때까지 기다렸다가 해야 한다.
			if((Control.MouseButtons & MouseButtons.Left) > 0)  // 마우스가 눌러져 있으면 나중에 
			{
				this.timer1.Enabled = true;
			}
			else
			{
				// 마우스가 눌러져 있지 않으면 지금 값 영역을 선택한다.
				this.textBoxValue.Select();
			}
		}

		private void hScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			TagAiClass ai = TagLib.GetStructAI(sTag, ref nTagPos);
			ai = TagLib.GetDirectTag(ai);

			textBoxValue.Text = TagUtil.AiValueToStringOnlyPoint(ai, e.NewValue*ai.fScrollUnit);
		}

		bool ConfirmMessageAI(TagAiClass ai, double value) 
		{
			if(ai.cConfirmCount <= 0)		return true;

			string msg, title;
			int  i;

			for(i = 0; i < ai.cConfirmCount && i < 5; i++) 
			{
				if(Tools.IsLangKorean()) 
				{
					title = String.Format("출력 확인 {0}", i+1);	
					msg = String.Format("[{0}]태그의 값을 [{1}]로 바꾸시겠습니까?", ai.tag, value);
				}
				else if(Tools.IsLangChinese()) 
				{
					title = String.Format("输出确认 {0}", i+1);	
					msg = String.Format("要把[{0}]标记值改为[{1}]吗？", ai.tag, value);
				}
				else 
				{
					title = String.Format("Confirm {0}", i+1);	
					msg = String.Format("Change [{0}] Tag value to [{1}]?", ai.tag, value);
				}
				if(MessageBox.Show(msg, title, MessageBoxButtons.YesNo) != DialogResult.Yes)	return false;
			}

			return true;
		}

		void SetValueAlarmInfo(TagAiClass ai, double old_val, double new_val)
		{
			string msg;
			string s_old;
			string s_new;

			s_old = TagUtil.AiValueToStringOnlyPoint(ai, old_val);
			s_new = TagUtil.AiValueToStringOnlyPoint(ai, new_val);

			if(Tools.IsLangKorean()) 
				msg = String.Format("수동 설정치 변경 ({0}->{1})", s_old, s_new);
			else
				msg = String.Format("Set Value Changed ({0}->{1})", s_old, s_new);

			AlarmUtil.AlarmDataSave(ai, msg, EnumAlarmType.HAND_OPERATION);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			TagAiClass ai = TagLib.GetStructAI(sTag, ref nTagPos);
			ai = TagLib.GetDirectTag(ai);

			double value = ConvertTool.ToDouble(textBoxValue.Text);

			if(!ConfirmMessageAI(ai, value)) 
			{
				Close();
				return;
			}

			SetValueAlarmInfo(ai, ai.curr, value);

			TagWrite.WriteCurrAI(ai.tag, ai, value, true);

			if(ai.cTagLinkType == 2) // 메모리 태그
			{
				LibComNetServer.SendCommandTagValueChanged(ai.tag, value);
			}

			Close();
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if((Control.MouseButtons & MouseButtons.Left) == 0) 
			{
				this.timer1.Enabled = false;
				this.textBoxValue.Select();
			}
		}
        */
    }


}
