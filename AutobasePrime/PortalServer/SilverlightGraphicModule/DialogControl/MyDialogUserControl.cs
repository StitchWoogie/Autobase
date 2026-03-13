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
using AutoLib;
using NetTools;
using System.Collections.Generic;
using SilverlightGraphicModule;

namespace SilverlightDialogControl
{
    public class MyDialogUserControl : Dialog
    {
        //TextBox textBoxValue = new TextBox();
        //SilverlightGraphicModule.Page childAnalog;
        Grid gridBackground;
        Canvas gridFrame;       // 이것을 그리드로 하면 Scale Transform이 적용될 때 오른쪽/아래 영역이 비례적으로 안보이는 현상이 나타난다.
        Grid gridIn;
        public string Text = "Dialog";
        Rectangle rectClose;
        Rectangle rectTitle;

        protected override FrameworkElement GetContent()
        {
            //dialog.procClose = new AnalogBox.DelegateClose(Close);

            Grid grid = new Grid() { Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)), };     // 배경으로 반투명 하게 그리는 부분

            grid.SizeChanged += new SizeChangedEventHandler(grid_SizeChanged);
            
            gridBackground = grid;

            Grid gridp = new Grid();

            grid.Children.Add(gridp);

            gridFrame = new Canvas() { Width=0,Height=0, Margin=new Thickness(0,0,0,0)};

            gridp.Children.Add(gridFrame);

            rectTitle = new Rectangle();
            rectTitle.Height = 20;
            rectTitle.Width = 0;
            rectTitle.Fill = new SolidColorBrush(Color.FromArgb(128, 0, 0, 128));
            gridFrame.Children.Add(rectTitle);

            TextBlock text = new TextBlock();
            text.Text = Text;
            text.Margin = new Thickness(4, 4, 0, 0);
            text.Foreground = new SolidColorBrush(Colors.White);
            gridFrame.Children.Add(text);

            Rectangle rect = new Rectangle();
            rectClose = rect;
            rect.Width = 16;
            rect.Height = 16;
            rect.HorizontalAlignment = HorizontalAlignment.Right;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.Margin = new Thickness(0, 2, 2, 0);
            rect.Fill = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            rect.Stroke = new SolidColorBrush(Colors.Black);
            rect.MouseLeftButtonDown += new MouseButtonEventHandler(rect_MouseLeftButtonDown);
            gridFrame.Children.Add(rect);

            gridIn = new Grid() { Margin = new Thickness(0, 20, 0, 0), };

            gridFrame.Children.Add(gridIn);

            pageThis.eventHandlerModuleClose += new EventHandler(pageThis_eventHandlerModuleClose);     // 페이지가 CloseModule에 의하여 Close되면 이 Popup을 닫아준다.
            pageThis.eventHandlerModuleLoadComplete += new EventHandler(pageThis_eventHandlerModuleLoadComplete);
            gridIn.Children.Add(pageThis);
            //childAnalog = pageThis;

            return grid;

            /*
            // You could just use XamlReader to do everything except the event hookup.

            Grid grid = new Grid() { Width = 300, Height = 300 };

            Border border = new Border() { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(6), Background = new SolidColorBrush(Color.FromArgb(150, 0, 0, 128)) };

            grid.Children.Add(border);

            grid.Children.Add(new TextBlock() { Text = "Dialog", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(8) });

            Button buttonCancel = new Button() { Width = 50, Height = 24, Content = "Cancel", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(8) };
            grid.Children.Add(buttonCancel);
            buttonCancel.Click += new RoutedEventHandler(button_Click);

            Button buttonOK = new Button() { Width = 50, Height = 24, Content = "OK", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(0, 0, 8, 40) };
            grid.Children.Add(buttonOK);
            buttonOK.Click += new RoutedEventHandler(buttonOK_Click);

            textBoxValue = new TextBox() { Width = 100, Height = 40, Text = "OK", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Margin = new Thickness(0, 0, 50, 80), FontSize=20};
            grid.Children.Add(textBoxValue);

            Init();

            return grid;*/
        }

        void rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            //throw new NotImplementedException();
        }

        double nOrgWidth=1, nOrgHeight=1;

        void pageThis_eventHandlerModuleLoadComplete(object sender, EventArgs e)
        {
            uint flags = pageThis.obj.GetModuleWindowStyleFlag();
            int title_size = 0;
            int border_thick = 0;

            if ((flags & (uint)EnumWindowStyleFlags.WS_CAPTION) == (uint)EnumWindowStyleFlags.WS_CAPTION ||
                (flags & (uint)EnumWindowStyleFlags.WS_SYSMENU) == (uint)EnumWindowStyleFlags.WS_SYSMENU)
            {
                title_size = 20;
            }
            else
            {
                title_size = 0;
                gridIn.Margin = new Thickness(0, 0, 0, 0);
                rectTitle.Fill = null;    
            }

            /*
            if ((flags & (uint)EnumWindowStyleFlags.WS_BORDER) == (uint)EnumWindowStyleFlags.WS_BORDER)
            {
                border_thick = 1;
            }*/

            ChangeTagName(pageUser, sTempUserControlBoxTag);
            this.gridFrame.Width = pageThis.obj.nModuleSizeX + border_thick * 2;
            this.gridFrame.Height = pageThis.obj.nModuleSizeY + title_size + border_thick * 2;
            //throw new NotImplementedException();

            nOrgWidth = this.gridFrame.Width;
            nOrgHeight = this.gridFrame.Height;

            Canvas.SetLeft(rectClose, gridFrame.Width - 18);

            rectTitle.Width = this.gridFrame.Width;

            ScrollUpdate();
        }

        void pageThis_eventHandlerModuleClose(object sender, EventArgs e)
        {
            Close();
        }

        void ScrollUpdate()
        {
            Grid g = gridBackground;

            if (nOrgWidth > g.ActualWidth || nOrgHeight > g.ActualHeight)
            {
                ScaleTransform transform = new ScaleTransform();

                if (nOrgWidth > g.ActualWidth)
                {
                    transform.ScaleX = g.ActualWidth / nOrgWidth;
                }
                else
                    transform.ScaleX = 1;

                if (nOrgHeight > g.ActualHeight)
                {
                    transform.ScaleY = g.ActualHeight / nOrgHeight;
                }
                else
                    transform.ScaleY = 1;
                
                gridFrame.RenderTransform = transform;
            }
            else
            {
                gridFrame.RenderTransform = null;
            }
        }

        void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollUpdate();
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        Page pageThis;

        public MyDialogUserControl(Page page)
        {
            //sTag = tag;
            pageThis = page;
        }

        protected override void OnClickOutside()
        {
            // Close();
        }

        //int[] nTagPos;
        //int[] nTagPosAO;

        protected override void OnLoad()
        {
            //this.textBoxValue.Select(0, textBoxValue.Text.Length);
            //this.textBoxValue.Focus();
        }

        void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            /*
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

            Close();*/
        }

        bool ConfirmMessageAI(TagAiClass ai, double value)
        {
            /*
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
            }*/

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

        /*
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

        
        static void ChangeTagName(Page ex, string tag)
        {
            List<object> array = new List<object>();

            ex.obj.groupRoot.GetMultiSelectTagList(array);

            MULTI_SELECT_TAG_STRUCT list;

            for (int i = 0; i < array.Count; i++)
            {
                list = (MULTI_SELECT_TAG_STRUCT)array[i];

                if (list.tagSource == "__ControlBox_Tag")
                {
                    list.tagTarget = tag;
                }
            }

            ex.obj.groupRoot.SetMultiSelectTagList(array);
        }

        /// <summary>
        /// 사용자가 정의한 제어 박스를 표시한다.
        /// </summary>
        /// <param name="filename"></param>
        public static void DisplayUserControlBox(string filename, string tag, string dialog_title, string dialog_description, string dialog_minvalue, string dialog_maxvalue)
        {
            /*
            int module_sizex = 0;
            int module_sizey = 0;
            EnumModuleWindowStyle module_style = 0;
            EnumWindowStyleFlags module_style_flag = 0;
            int module_location_method = 0;
            int module_posx = 0;
            int module_posy = 0;
            int opacity = 0;
            bool popup_dialog = false;*/
            TagPublicClass tp;
            int[] tag_pos = new int[1];

            tp = TagLib.GetStructPublic(tag, ref tag_pos);

            /*
            GetModuleProperty(filename, ref module_sizex, ref module_sizey, ref module_style, ref module_style_flag,
                ref module_location_method, ref module_posx, ref module_posy, ref popup_dialog, ref opacity);

            popup_dialog = true;*/

            sTempUserControlBoxTag = tag;

            if (dialog_title == "?")
            {
                sTempUserControlBoxTitle = "Tag : " + tag;
            }
            else
            {
                sTempUserControlBoxTitle = dialog_title;
            }

            if (dialog_description == "?")
            {
                sTempUserControlBoxDescription = tp.description;
            }
            else
            {
                sTempUserControlBoxDescription = dialog_description;
            }

            sTempUserControlBoxMaxValue = dialog_maxvalue;
            sTempUserControlBoxMinValue = dialog_minvalue;

            /*
            //if (pos_method == -1)
            //{
            FormGraphicFrame dialog = MakeGraphicPopupWindow(filename, module_sizex, module_sizey, module_style_flag,
                module_location_method, module_posx, module_posy, popup_dialog, opacity);
            */

            SilverlightGraphicModule.Page page = new SilverlightGraphicModule.Page();

            pageUser = page;
            //page.eventHandlerModuleLoadComplete += new EventHandler(page_EventListModuleLoadComplete);
            
            page.Load(filename, 0);

            
            //dialog.formChild.bUseUserDefinedTitle = true;
            
            /*
            if (popup_dialog) dialog.ShowDialog();
            else
            {
                dialog.Owner = TotalConfig.formMain;
                dialog.Show();
            }*/

            MyDialogUserControl dialog = new MyDialogUserControl(page);

            dialog.Text = sTempUserControlBoxTitle;

            dialog.Show(DialogStyle.Modal);
        }

        /*
        static void page_EventListModuleLoadComplete(object sender, EventArgs e)
        {
            ChangeTagName(pageUser, sTempUserControlBoxTag);

            //pageUser.LayoutRoot.Width = pageUser.obj.nModuleSizeX;
            //pageUser.LayoutRoot.Height = pageUser.obj.nModuleSizeY;
        }*/

        static Page pageUser;


        public static string sTempUserControlBoxTitle = "";
        public static string sTempUserControlBoxDescription = "";
        public static string sTempUserControlBoxMaxValue = "";
        public static string sTempUserControlBoxMinValue = "";
        public static string sTempUserControlBoxTag = "";
    }
}
