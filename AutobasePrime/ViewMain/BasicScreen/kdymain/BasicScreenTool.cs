using System;
using System.Drawing;
using AutoLib;
using NetTools;
using AutoLibLocal;
using System.Windows.Forms;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for BasicScreenTool.
	/// </summary>
	public class BasicScreenTool 
	{
		public BasicScreenTool()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//public static bool isActiveTag()
		//{//
		//	TagPublicClass	tp = TagLib.GetStructPublic(work.tagList[work.pos].tag, ref work.tagList[work.pos].tag_pos);
		//	return isActiveTag(tp);
		//}

		public static bool isActiveTag(TagPublicClass tp)
		{
			if(tp == null || tp.act == 0) 
			{
				if(Tools.IsLangKorean()) MessageBox.Show("이 태그는 비활성 태그입니다.", "비활성 태그");
				else MessageBox.Show("Selected Tag is Inactive.", "Inactive Tag");
				return false;
			}
			return true;
		}

		public static string GetDesON(TagDiClass di)
		{
			if(di.desON.Length == 0)	
			{
				return "ON";
			}
			else 
			{
				if(di.desON == di.desOFF) return di.desON + "(ON)";// ON/OFF 설명이 동일할 때
				return di.desON;
			}
		}

		public static string GetDesOFF(TagDiClass di)
		{
			if(di.desOFF.Length == 0)	
			{
				return "OFF";
			}
			else 
			{
				if(di.desON == di.desOFF) return di.desOFF + "(OFF)";// ON/OFF 설명이 동일할 때
				return di.desOFF;
			}
		}

		public static void callDetailWindow(TagPublicClass tp)
		{
			if(tp == null) return;			
			switch(tp.enumTagType) 
			{
				case EnumTagType.AI :
					if(isActiveTag(tp) == false) return;
					ViewAnalogInputDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogInputDetailMain(tp.tag), ConfigViewMain.nMdiCountOnBasicScreen);
					break;
				case EnumTagType.AO :
					if(isActiveTag(tp) == false) return;
					ViewAnalogOutputDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogOutputDetailMain(tp.tag), ConfigViewMain.nMdiCountOnBasicScreen);
					break;
				case EnumTagType.DI :
					if(isActiveTag(tp) == false) return;
					ViewDigitalInputDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputDetailMain(tp.tag), ConfigViewMain.nMdiCountOnBasicScreen);
					break;
				case EnumTagType.DO :
					if(isActiveTag(tp) == false) return;
					ViewDigitalOutputDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalOutputDetailMain(tp.tag), ConfigViewMain.nMdiCountOnBasicScreen);
					break;
				case EnumTagType.ST :
				case EnumTagType.GR :
					//CallSelectTagPropertyWindows();
					break;
			}
		}

        //20241010 PSU 출력창은 Form 속성에서 positon 수정. centerparent.
		public static bool callValueChangeWindows(Form owner, TagPublicClass tp)
		{
			if(tp == null) return false;
			switch(tp.enumTagType) 
			{
				case EnumTagType.AI :
					if(BasicScreenTool.isActiveTag(tp) == false) return false;
					DialogControl.ControlBoxAnalogInputGo dialogAi = new DialogControl.ControlBoxAnalogInputGo();
					dialogAi.Go(owner, tp.tag);
					break;
				case EnumTagType.AO :
					if(BasicScreenTool.isActiveTag(tp) == false) return false;
					DialogControl.ControlBoxAnalogOutputGo dialogAo = new DialogControl.ControlBoxAnalogOutputGo();
					dialogAo.Go(owner, tp.tag);
					break;
				case EnumTagType.DI :
					if(BasicScreenTool.isActiveTag(tp) == false) return false;
					DialogControl.ControlBoxDigitalInputGo dialogDi = new DialogControl.ControlBoxDigitalInputGo();
					dialogDi.Go(owner, tp.tag);
					break;
				case EnumTagType.DO :
					if(BasicScreenTool.isActiveTag(tp) == false) return false;
					DialogControl.ControlBoxDigitalOutputGo dialogDo = new DialogControl.ControlBoxDigitalOutputGo();
					dialogDo.Go(owner, tp.tag);
					break;
				case EnumTagType.ST :				
					if(BasicScreenTool.isActiveTag(tp) == false) return false;
					DialogControl.ControlBoxStringTagGo dialogSt = new DialogControl.ControlBoxStringTagGo();
					dialogSt.Go(owner, tp.tag);
					break;
				case EnumTagType.GDO:
					if(BasicScreenTool.isActiveTag(tp) == false) return false;
					ControlBoxDoGroup dialogDoGroup = new ControlBoxDoGroup(tp.tag);
                    dialogDoGroup.StartPosition = FormStartPosition.CenterParent;
					dialogDoGroup.ShowDialog(owner);
					break;
				default : return false;
			}
			return true;
		}

        // xGab = fontx*0.25 즉 폰트의 1/4 크기
		public static void AiCurrValAndProgBarDraw(Graphics g, TagAiClass ai, int x, int y, int width, int height, int xGap, Color tColor, Color bColor, Font font)
		{
            if (ai == null) return;

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;

            string value_string = TagUtil.AiValueToStringWithDeviceQuality(ai);

            SizeF sizef = g.MeasureString(value_string, font);

            int draw_width = width - xGap * 2;  // 그릴 수 있는 width

            //DrawClass.WinDrawText(g, x + xGap * 3 + width * 3 / 5, y + 1, width * 2 / 5 - xGap * 4, height, value_string, tColor, bColor, font, format);
            DrawClass.WinDrawText(g, x + xGap, y + 1, draw_width, height, value_string, tColor, bColor, font, format);
            if (ai.act != 1) return;    // 활성화가 아닐때는 숫자만 표시하고 return 한다.

            x = x + xGap;
            int graph_width = (draw_width) /2;                              // bar graph width
            int remain_width = (int)(draw_width - sizef.Width - xGap);      // 남은 width 1/4의 공간도 준다.

            if (remain_width < graph_width) // 남아있는 width가 그래프 그릴 공간이 안되면
            {
                width = remain_width;
            }
            else
            {
                width = graph_width;
            }

            if (width < 10) return; // 그릴 공간이 너무 적으면 그리지 않는다.

            double _base, full, imsi;
            int i, j, curr, lolo, low, high, hihi;

            _base = ai.fBase;
            full = ai.fFull - _base;
            if (full == 0.0) return;    // 최대값이 설정되어 있지 않으면 숫자만 표시하고 return한다. avoid divide by zero     

            imsi = (float)(ai.curr - _base);
            curr = (int)(imsi * width / full);
            if (curr <= 1) curr = 1;
            else if (curr > width) curr = (int)width;

            imsi = ai.lolo - _base;
            lolo = (int)(imsi * (width) / full);
            if (lolo <= 1) lolo = 1;
            else if (lolo > width) lolo = (int)width;

            imsi = ai.low - _base;
            low = (int)(imsi * width / full);
            if (low <= 1) low = 1;
            else if (low > width) low = (int)width;

            imsi = ai.high - _base;
            high = (int)(imsi * width / full);
            if (high <= 1) high = 1;
            else if (high > width) high = (int)width;

            imsi = ai.hihi - _base;
            hihi = (int)(imsi * width / full);
            if (hihi <= 1) hihi = 1;
            else if (hihi > width) hihi = (int)width;

            if (curr > hihi)
            {
                DrawClass.gcls(g, x + 1, y + (int)(height * 0.2), x + lolo, y + (int)(height * 0.8), SharedData.colorTotal.LOLO);
                DrawClass.gcls(g, x + lolo, y + (int)(height * 0.2), x + low, y + (int)(height * 0.8), SharedData.colorTotal.LOW);
                DrawClass.gcls(g, x + low, y + (int)(height * 0.2), x + high, y + (int)(height * 0.8), SharedData.colorTotal.HIGH);
                DrawClass.gcls(g, x + high, y + (int)(height * 0.2), x + curr, y + (int)(height * 0.8), SharedData.colorTotal.HIHI);
            }
            else if (curr > high)
            {
                DrawClass.gcls(g, x + 1, y + (int)(height * 0.2), x + lolo, y + (int)(height * 0.8), SharedData.colorTotal.LOLO);
                DrawClass.gcls(g, x + lolo, y + (int)(height * 0.2), x + low, y + (int)(height * 0.8), SharedData.colorTotal.LOW);
                DrawClass.gcls(g, x + low, y + (int)(height * 0.2), x + curr, y + (int)(height * 0.8), SharedData.colorTotal.HIGH);
            }
            else if (curr > low)
            {
                DrawClass.gcls(g, x + 1, y + (int)(height * 0.2), x + lolo, y + (int)(height * 0.8), SharedData.colorTotal.LOLO);
                DrawClass.gcls(g, x + lolo, y + (int)(height * 0.2), x + curr, y + (int)(height * 0.8), SharedData.colorTotal.LOW);
            }
            else
                DrawClass.gcls(g, x + 1, y + (int)(height * 0.2), x + curr, y + (int)(height * 0.8), SharedData.colorTotal.LOLO);


            j = height / 3;
            if (j < 2) j = 2;
            for (i = j / 2 + 2; i < width - 2; i += j)
                DrawClass.gcls(g, x + i, y + (int)(height * 0.2), x + i, y + (int)(height * 0.8), Color.DarkGray);

            DrawClass.PushRectangle2(g, x, y + (int)(height * 0.2), x + (int)width, y + (int)(height * 0.8));
            DrawClass.PushRectangle2(g, x - 1, y + (int)(height * 0.2) - 1, x + (int)width + 1, y + (int)(height * 0.8) + 1);

            /*
            // 바 그래프가 전체의 3/5를 차지하고 있어 중요한 숫자가 디스프레이 되지 않아서 숫자 위주로 디스프레이 할 필요가 있다.
			if(ai == null) return;

			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Far;

            string value_string = TagUtil.AiValueToStringWithDeviceQuality(ai);

            DrawClass.WinDrawText(g, x + xGap * 3 + width * 3 / 5, y + 1, width * 2 / 5 - xGap * 4, height, value_string, tColor, bColor, font, format);
			if(ai.act != 1) return;

			double					_base, full, imsi;
			int						i, j, curr, lolo, low, high, hihi;
			
			x = x+xGap;
			width = width*3/5-xGap*2;

			_base = ai.fBase;
			full = ai.fFull - _base;
			if(full == 0.0) return;    // avoid divide by zero

			imsi = (float)(ai.curr - _base);
			curr = (int)(imsi * width / full);
			if(curr <= 1) curr = 1;
			else if(curr > width) curr = (int)width;

			imsi = ai.lolo - _base;
			lolo = (int)(imsi * (width) / full);
			if(lolo <= 1) lolo = 1;
			else if(lolo > width) lolo = (int)width;

			imsi = ai.low - _base;
			low = (int)(imsi * width / full);
			if(low <= 1) low = 1;
			else if(low > width) low = (int)width;

			imsi = ai.high - _base;
			high = (int)(imsi * width / full);
			if(high <= 1) high = 1;
			else if(high > width) high = (int)width;

			imsi = ai.hihi - _base;
			hihi = (int)(imsi * width / full);
			if(hihi <= 1) hihi = 1;
			else if(hihi > width) hihi = (int)width;

			if(curr > hihi) 
			{
				DrawClass.gcls(g, x+1, y+(int)(height*0.2), x+lolo, y+(int)(height*0.8), SharedData.colorTotal.LOLO);
				DrawClass.gcls(g, x+lolo, y+(int)(height*0.2), x+low, y+(int)(height*0.8), SharedData.colorTotal.LOW);
				DrawClass.gcls(g, x+low, y+(int)(height*0.2), x+high, y+(int)(height*0.8), SharedData.colorTotal.HIGH);
				DrawClass.gcls(g, x+high, y+(int)(height*0.2), x+curr, y+(int)(height*0.8), SharedData.colorTotal.HIHI);
			}
			else if(curr > high) 
			{
				DrawClass.gcls(g, x+1, y+(int)(height*0.2), x+lolo, y+(int)(height*0.8), SharedData.colorTotal.LOLO);
				DrawClass.gcls(g, x+lolo, y+(int)(height*0.2), x+low, y+(int)(height*0.8), SharedData.colorTotal.LOW);
				DrawClass.gcls(g, x+low, y+(int)(height*0.2), x+curr, y+(int)(height*0.8), SharedData.colorTotal.HIGH);
			}
			else if(curr > low) 
			{
				DrawClass.gcls(g, x+1, y+(int)(height*0.2), x+lolo, y+(int)(height*0.8), SharedData.colorTotal.LOLO);
				DrawClass.gcls(g, x+lolo, y+(int)(height*0.2), x+curr, y+(int)(height*0.8), SharedData.colorTotal.LOW);
			}
			else
				DrawClass.gcls(g, x+1, y+(int)(height*0.2), x+curr, y+(int)(height*0.8), SharedData.colorTotal.LOLO);


			j = height/3;
			if(j < 2) j = 2;
			for(i = j/2+2; i < width-2; i+=j)
				DrawClass.gcls(g, x+i, y+(int)(height*0.2), x+i, y+(int)(height*0.8), Color.DarkGray);

			DrawClass.PushRectangle2(g, x, y+(int)(height*0.2), x+(int)width, y+(int)(height*0.8));
			DrawClass.PushRectangle2(g, x-1, y+(int)(height*0.2)-1, x+(int)width+1, y+(int)(height*0.8)+1);
             */
		}

		public static void DiCurrValAndStatusButtonDraw(Graphics g, TagDiClass di, int x, int y, int width, int height, Font font)
		{
			if(di == null) return;

			Color	tcolor = (di.curr == 1) ? SharedData.colorTotal.ON : SharedData.colorTotal.OFF;
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Center;

            string value_string;

            if (di.curr == 1) value_string = di.desON;
            else value_string = di.desOFF;

            value_string = TagUtil.ApplyDeviceQuality(di, value_string);

            DrawClass.WinDrawText(g, x, y, width / 2, height, value_string, tcolor, Color.LightGray, font, format);

			x += width/2;
			DrawClass.PushBox2(g, x+width/6-height/2, y+height/6, x+width/6-height/2+height, y+height-height/6, tcolor);
		}

		public static void DoCurrValAndStatusButtonDraw(Graphics g, TagDoClass dout, int x, int y, int width, int height, Font font)
		{
			if(dout == null) return;

			Color	tcolor = (dout.curr == 1) ? SharedData.colorTotal.ON : SharedData.colorTotal.OFF;
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Center;

			if(dout.curr == 1) DrawClass.WinDrawText(g, x, y, width/2, height, dout.desON, tcolor, Color.LightGray, font, format);
			else			   DrawClass.WinDrawText(g, x, y, width/2, height, dout.desOFF, tcolor, Color.LightGray, font, format);

			x += width/2;
			DrawClass.PushBox2(g, x+width/6-height/2, y+height/6, x+width/6-height/2+height, y+height-height/6, tcolor);
		}

		public static void DoGroupCurrValAndStatusButtonDraw(Graphics g, TagDoGroupClass doGroup, int x, int y, int width, int height, Font font)
		{
			if(doGroup == null) return;

			Color	tcolor = (doGroup.curr == 1) ? SharedData.colorTotal.ON : SharedData.colorTotal.OFF;
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Center;

			if(doGroup.curr == 1) DrawClass.WinDrawText(g, x, y, width/2, height, "ON", tcolor, Color.LightGray, font, format);
			else				  DrawClass.WinDrawText(g, x, y, width/2, height, "OFF", tcolor, Color.LightGray, font, format);

			x += width/2;
			DrawClass.PushBox2(g, x+width/6-height/2, y+height/6, x+width/6-height/2+height, y+height-height/6, tcolor);
		}


		public static string TagPosToString(int[] tag_pos)
		{
			string buf = "";
			for(int i = 0; i < tag_pos.Length; i++) 
			{
				if(i != 0) 
				{
					buf += '.';
				}
				buf += tag_pos[i];
			}

			return buf;
		}

		public static int[] StringToTagPos(string buf)
		{
			int[] tag_pos;

			int count = 0;
			int i;

			for(i = 0; i < buf.Length; i++) 
			{
				if(buf[i] == '.')	count++;
			}

			CommaBlockString comma = new CommaBlockString();
			comma.SetBlockCode('.');
			comma.Set(buf);

			count++;	// .보다 1개 많다.

			tag_pos = new int[count];

			for(i = 0; i < count; i++) 
			{
				comma.GetInt(ref tag_pos[i]);
			}

			return tag_pos;
		}
		




	}
}
