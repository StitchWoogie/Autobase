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
    public partial class PageControlString : UserControl
    {
        public PageControlString(string tag)
        {
            InitializeComponent();

            sTag = tag;

            Init();

            StoryboardBoxOpen.Begin();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonCancel_MouseEnter(object sender, MouseEventArgs e)
        {
            //StoryboardCancelEnter.Begin();
        }

        private void buttonCancel_MouseLeave(object sender, MouseEventArgs e)
        {
            //StoryboardCancelLeave.Begin();
        }

        void Init()
        {
            TagStClass st = TagLib.GetStructST(sTag, ref nTagPos);
            //st = TagLib.GetDirectTag(st);

            this.textboxValue.Text = st.curr;

            this.textboxTagname.Text = st.tag;
            this.textboxTagdescription.Text = st.description;


            
        }

        string sTag;

        int[] nTagPos;
        //int[] nTagPosAO;

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            TagStClass st = TagLib.GetStructST(sTag, ref nTagPos);
            //st = TagLib.GetDirectTag(ai);

            string  value = this.textboxValue.Text;

            /*
            if (!ConfirmMessageAI(st, value))
            {
                Close();
                return;
            }

            SetValueAlarmInfo(ai, ai.curr, value);*/

            TagWrite.WriteCurrST(st.tag, st, value, true);

            Close();
        }

        private void buttonOk_MouseEnter(object sender, MouseEventArgs e)
        {
            //storyboardButtonokEnter.Begin();
        }

        private void buttonOk_MouseLeave(object sender, MouseEventArgs e)
        {
            //storyboardbottonokLeave.Begin();
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

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close(); //안됨
        }
    }
}
