using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AutoLibLocal;
using NetTools;
using AutoLib;

namespace SilverlightDialogControl
{
    public partial class PageControlAnalog : UserControl
    {
        public PageControlAnalog(string tag)
        {
            InitializeComponent();

            sTag = tag;

            Init();

            StoryboardBoxOpen.Begin();

            this.slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slider_ValueChanged);
        }

        void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TagAiClass ai = TagLib.GetStructAI(sTag, ref nTagPos);
            ai = TagLib.GetDirectTag(ai);

            this.textboxSet.Text = TagUtil.AiValueToStringOnlyPoint(ai, slider.Value);
            // throw new NotImplementedException();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonCancel_MouseEnter(object sender, MouseEventArgs e)
        {
            StoryboardCancelEnter.Begin();
        }

        private void buttonCancel_MouseLeave(object sender, MouseEventArgs e)
        {
            StoryboardCancelLeave.Begin();
        }

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

            
            this.textboxSet.Text = TagUtil.AiValueToStringOnlyPoint(ai, ai.curr);
            this.textblockMinNum.Text = TagUtil.AiValueToStringOnlyPoint(ai, fBase);
            this.textblockMaxNum.Text = TagUtil.AiValueToStringOnlyPoint(ai, fFull);

            this.textBlockTagname.Text = ai.tag;
            this.textboxTagdescription.Text = ai.description;

            if (ai.cTagLinkType == 2)
            {
            }
            else
            {
                if (ai.bUseAsOutput == 0 && ai.sSubOutAnalogSP.Length > 0)
                {
                    TagAoClass ao = TagLib.GetStructAO(ai.sSubOutAnalogSP, ref nTagPosAO);
                    ao = TagLib.GetDirectTag(ao);
                }
            }

            double max;
            double min;

            max = fFull;
            min = fBase;
            //max = (long)(fFull / ai.fScrollUnit + hScrollBar.LargeChange - 1);
            //min = (long)(fBase / ai.fScrollUnit);

            if (min < double.MinValue) min = int.MinValue;
            if (max > double.MaxValue) max = int.MaxValue;

            if ((max - min) > (double.MaxValue - 3))	// 스크롤 min max 의 차이가 21억이 넘으면 오류
            {
                min = max - (double.MaxValue - 3);
            }


            slider.Minimum = min;
            slider.Maximum = max;
                ///hScrollBar.Maximum = (int)max;
                //hScrollBar.Minimum = (int)min;

                double val = ai.curr;

                if (val < min) val = (int)min;
                if (val > max) val = (int)max;

                slider.Value = val;


            /*
            // 마우스 누를 때 이 대화상자를 호출하면 입력기의 역역을 선택해도 선택이 되지 않는다.
            // 이럴때는 마우스 뗄때까지 기다렸다가 해야 한다.
            if ((Control.MouseButtons & MouseButtons.Left) > 0)  // 마우스가 눌러져 있으면 나중에 
            {
                this.timer1.Enabled = true;
            }
            else
            {
                // 마우스가 눌러져 있지 않으면 지금 값 영역을 선택한다.
                this.textBoxValue.Select();
            }*/

            
        }

        string sTag;

        int[] nTagPos;
        int[] nTagPosAO;

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            
            TagAiClass ai = TagLib.GetStructAI(sTag, ref nTagPos);
            ai = TagLib.GetDirectTag(ai);

            double value = ConvertTool.ToDouble(this.textboxSet.Text);

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

        private void buttonOk_MouseEnter(object sender, MouseEventArgs e)
        {
            storyboardButtonokEnter.Begin();
        }

        private void buttonOk_MouseLeave(object sender, MouseEventArgs e)
        {
            storyboardbottonokLeave.Begin();
        }

        private void buttonCloseArea_MouseEnter(object sender, MouseEventArgs e)
        {
            StoryboardButtonclose.Begin();
        }

        private void buttonCloseArea_MouseLeave(object sender, MouseEventArgs e)
        {
            StoryboardButtonclose.Stop();
        }

        private void buttonCloseArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        public delegate void DelegateClose();
        public DelegateClose procClose = null;

        void Close()
        {
            if (procClose != null) procClose();
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
                if (MessageBox.Show(msg, title, MessageBoxButton.OKCancel) != MessageBoxResult.OK) return false;
            }

            return true;
        }

        void SetValueAlarmInfo(TagAiClass ai, double old_val, double new_val)
        {
            /*
            string msg;
            string s_old;
            string s_new;

            s_old = TagUtil.AiValueToStringOnlyPoint(ai, old_val);
            s_new = TagUtil.AiValueToStringOnlyPoint(ai, new_val);

            if (Tools.IsLangKorean())
                msg = String.Format("수동 설정치 변경 ({0}->{1})", s_old, s_new);
            else
                msg = String.Format("Set Value Changed ({0}->{1})", s_old, s_new);

            AlarmUtil.AlarmDataSave(ai, msg, EnumAlarmType.HAND_OPERATION);*/
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.textboxSet.Focus();
        }
    }
}
