using System;
using GraphicModule;
using System.Drawing;
using System.Windows.Forms;
using NetTools.OldDefine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using NetTools;
using Studio.Property;
using AutoLibLocal;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditProperty.
	/// </summary>
	public class ClassEditProperty
	{
		public ClassEditProperty()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static PropertySheetPublic propertySheet = null;
		public static FormEditGraphic formEditor;

		// 사용자가 오브젝트 특성메뉴를 선택했을 때
		public static void OnUserPropertyClick(FormEditGraphic form)
		{
			if(propertySheet != null) 
			{
				// 이미 화면이 떠있다.
				return;
			}

			Property(form);
		}

		static void Property(FormEditGraphic form)
		{
			if(propertySheet == null) 
			{
				propertySheet = new PropertySheetPublic();	
			}
			else	// 무조건 껏다가 켠다. (오류가 많이 나서)
			{
				propertySheet.Close();
				propertySheet = new PropertySheetPublic();	
			}

			WORK_MODULE_STRUCT work = form.workThis;

			formEditor = form;

			//if(work.nSelectCount != 1)	return;
			propertySheet.Prepare(form);

			bool multi_select = false;

			if(work.nSelectCount > 1)	multi_select = true;

			for(int i = 0; i < work.nSelectCount; i++) 
			{
				//EnumObjectType type = work.obj.groupRoot.GetObjectType(work.selectList[i].pos);
				//object obj = work.obj.groupRoot.GetPoint(work.selectList[i].pos);
                EnumObjectType type = ((ObjectType)work.selectList[i].obj).enumObjectType;
                object obj = work.selectList[i].obj;

				switch(type) 
				{
					case EnumObjectType.ButtonModule3D:
						PropertyButtonModule3D(obj, multi_select);
						break; 
					case EnumObjectType.ButtonModuleHide:
						PropertyButtonModuleHide(obj, multi_select);
						break;
					case EnumObjectType.ButtonProgramm:
						PropertyButtonProgram(obj, multi_select);
						break;
					case EnumObjectType.ButtonDigitalOut:
						PropertyButtonDigitalOut(obj, multi_select);
						break;

					case EnumObjectType.DigitalAnimation:
						PropertyDigitalAnimation(obj, multi_select);
						break;
					case EnumObjectType.DigitalCircle:
						PropertyDigitalCircle(obj, multi_select);
						break;
					case EnumObjectType.DigitalRectangle:
						PropertyDigitalRectangle(obj, multi_select);
						break;
					case EnumObjectType.DigitalString:
						PropertyDigitalString(obj, multi_select);
						break;

					case EnumObjectType.AnalogMeter:
						PropertyAnalogMeter(obj, multi_select);
						break;
					case EnumObjectType.AnalogRectangle:
						PropertyAnalogRectangle(obj, multi_select);
						break;
					case EnumObjectType.AnalogRotate:
						PropertyAnalogRotate(obj, multi_select);
						break;
					case EnumObjectType.AnalogStatus:
						PropertyAnalogStatus(obj, multi_select);
						break;
					case EnumObjectType.AnalogString:
						PropertyAnalogString(obj, multi_select);
						break;
                    case EnumObjectType.AnalogGauge:
                        PropertyAnalogGauge(obj, multi_select);
                        break;

                    //AnalogGauge 20250122 hsjeong

					case EnumObjectType.StringString:
						PropertyStringString(obj, multi_select);
						break;

					case EnumObjectType.Module:
						PropertyModule(obj, multi_select);
						break;

					case EnumObjectType.ControlCheckBox:
						PropertyControlCheckBox(obj, multi_select);
						break;
					case EnumObjectType.ControlComboBox:
						PropertyControlComboBox(obj, multi_select);
						break;
					case EnumObjectType.ControlEditBox:
						PropertyControlEditBox(obj, multi_select);
						break;
					case EnumObjectType.ControlListBox:
						PropertyControlListBox(obj, multi_select);
						break;
					case EnumObjectType.ControlRadioButton:
						PropertyControlRadioButton(obj, multi_select);
						break;
                    case EnumObjectType.ControlDatePicker:
                        PropertyControlDatePicker(obj, multi_select);
                        break;
                    case EnumObjectType.ControlTabControl:
                        PropertyControlTabControl(obj, multi_select);
                        break;

                    case EnumObjectType.ControlTreeView:
                        PropertyControlTreeView(obj, multi_select);
                        break;

					case EnumObjectType.Bitmap:
						PropertyBitmap(obj, multi_select);
						break;
					case EnumObjectType.Animation:
						PropertyAnimation(obj, multi_select);
						break;
					case EnumObjectType.SingleText:
						PropertySingleText(obj, multi_select);
						break;

					case EnumObjectType.Rectangle:
						PropertyRectangle(obj, multi_select);
						break;
					case EnumObjectType.Circle:
						PropertyCircle(obj, multi_select);
						break;
					case EnumObjectType.Line:
						PropertyLine(obj, multi_select);
						break;
					case EnumObjectType.Text:
						PropertyText(obj, multi_select);
						break;
					case EnumObjectType.Date:
						PropertyDate(obj, multi_select);
						break;
					case EnumObjectType.Clock:
						PropertyClock(obj, multi_select);
						break;


					case EnumObjectType.DatabaseTrend:
						PropertyDatabaseTrend(obj, multi_select);
						break;

					case EnumObjectType.Poly:
						PropertyPoly(obj, multi_select);
						break;

					case EnumObjectType.Curve:
						PropertyCurve(obj, multi_select);
						break;

					case EnumObjectType.RoundRectangle:
						PropertyRoundRectangle(obj, multi_select);
						break;

					case EnumObjectType.MultiGraph:
						PropertyMultiGraph(obj, multi_select);
						break;
					case EnumObjectType.MultiTrend:
						PropertyMultiTrend(obj, multi_select);
						break;
				
					case EnumObjectType.CustomChart:
						PropertyChartCustom(obj, multi_select);
						break;
					case EnumObjectType.Database:
						PropertyDatabase(obj, multi_select);
						break;
					case EnumObjectType.WindowAlarm:
						PropertyWindowAlarm(obj, multi_select);
						break;
					case EnumObjectType.DemandWindow:
						PropertyDemandWindow(obj, multi_select);
						break;
                    case EnumObjectType.DemandChart:
                        PropertyDemandChart(obj, multi_select);
                        break;
                    case EnumObjectType.MilliDataWindow:
						PropertyMilliData(obj, multi_select);
						break;
                    case EnumObjectType.MilliDataTrend:
                        PropertyMilliDataTrend(obj, multi_select);
                        break;
					case EnumObjectType.RealTimeTestGraph:
						PropertyRealTimeTestGraph(obj, multi_select);
						break;
					case EnumObjectType.XYGraph:
						PropertyXYGraph(obj, multi_select);
						break;

					case EnumObjectType.ChangeValueDisplay:
						PropertyChangeValueDisplay(obj, multi_select);
						break;

					case EnumObjectType.Group:
						PropertyGroup(obj, multi_select);
						break;

                    case EnumObjectType.WebBrowser:
                        PropertyWebBrowser(obj, multi_select);
                        break;
                        //webview 20240627 PSU
                    case EnumObjectType.WebView:
                        PropertyWebView(obj, multi_select);
                        break;
                        //vlc 20240627 PSU
                    case EnumObjectType.VLCAx:
                        PropertyVLCAx(obj, multi_select);
                        break;
                    case EnumObjectType.TagAnimation:
                        PropertyTagAnimation(obj, multi_select);
                        break;

                    case EnumObjectType.DataGridView:
                        PropertyDataGridView(obj, multi_select);
                        break;
                    case EnumObjectType.SVG:
                        PropertySVG(obj, multi_select);
                        break;
                        //SVG 20241024 PSU

                    case EnumObjectType.DonutChart:
                        PropertyDonutChart(obj, multi_select);
                        break;
                    //DonutChart 25-02-04 hsjeong

                    case EnumObjectType.BarcodeDisplay:
                        PropertyBarcodeDisplay(obj, multi_select);
                        break;
                    case EnumObjectType.BarcodeScanner:
                        PropertyBarcodeScanner(obj, multi_select);
                        break;

				}
			}

			if(work.nSelectCount == 0) 
			{
				propertySheet.Run();	// 오브젝트가 선택되지 않았다는 메시지는 보여야 한다.
			}

			//form.Select();
            SharedStudio.formMain.Select();
		}

        static bool PropertyRecv_Font(object obj, object prop)
        {
            Form form = (Form)prop;

            if (form.Name != "PropertyPageFont") return false;

            PropertyPageFont font = (PropertyPageFont)prop;

            LOGFONT lf = ((ObjectFont)obj).GetLogFont();

            if (font.comboBoxFamily.Text.Length != 0)
                lf.lfFaceName = font.comboBoxFamily.Text;

            if (font.comboBoxSize.Text.Length != 0)
                lf.lfHeight = ConvertTool.ToInt32(font.comboBoxSize.Text);

            if (font.checkBoxBold.CheckState != CheckState.Indeterminate)
            {
                if (font.checkBoxBold.CheckState == CheckState.Checked)
                    lf.style |= FontStyle.Bold;
                else
                    lf.style &= FontStyle.Italic | FontStyle.Strikeout | FontStyle.Underline;
            }

            if (font.checkBoxItalic.CheckState != CheckState.Indeterminate)
            {
                if (font.checkBoxItalic.CheckState == CheckState.Checked)
                    lf.style |= FontStyle.Italic;
                else
                    lf.style &= FontStyle.Bold | FontStyle.Strikeout | FontStyle.Underline;
            }

            if (font.checkBoxUnderline.CheckState != CheckState.Indeterminate)
            {
                if (font.checkBoxUnderline.CheckState == CheckState.Checked)
                    lf.style |= FontStyle.Underline;
                else
                    lf.style &= FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout;
            }

            if (font.checkBoxStrikeout.CheckState != CheckState.Indeterminate)
            {
                if (font.checkBoxStrikeout.CheckState == CheckState.Checked)
                    lf.style |= FontStyle.Strikeout;
                else
                    lf.style &= FontStyle.Bold | FontStyle.Italic | FontStyle.Underline;
            }


            ((ObjectFont)obj).SetLogFont(lf);

            return true;
        }

		static bool PropertyRecv_ClassName(object obj, object prop)
		{
			Form form = (Form)prop;

			if(form.Name != "PropertyPageClassName")		return false;
			
			PropertyPageClassName classname = (PropertyPageClassName)prop;

			if(classname.textBoxClassName.ReadOnly == false) 
			{
				((ObjectExpand)obj).objGeneral.SetClassName(classname.textBoxClassName.Text);
			}

			classname.Get(formEditor, (ObjectExpand)obj);
            
			return true;
		}

		static bool PropertyRecv_LineColor(object obj, object prop)
		{
			Form form = (Form)prop;

			if(form.Name != "PropertyPageColorLine")		return false;
			
			PropertyPageColorLine color = (PropertyPageColorLine)prop;

			if(!color.IsMultiSelectedColor())
				((ObjectExpand)obj).SetLineColor(color.GetSelectedColor());
			if(!color.IsMultiSelectedThick())
				((ObjectExpand)obj).SetBorderThick(color.GetSelectedThick());
			if(!color.IsMultiSelectedOption())
				((ObjectExpand)obj).nLineOption = color.GetSelectedOption();
            
			return true;
		}

        static bool PropertyRecv_ButtonThick(object obj, object prop)
        {
            Form form = (Form)prop;

            if (form.Name != "PropertyPageButtonThick") return false;

            PropertyPageButtonThick color = (PropertyPageButtonThick)prop;

            //if (!color.IsMultiSelectedColor())
                //((ObjectExpand)obj).SetLineColor(color.GetSelectedColor());
            if (!color.IsMultiSelectedThick())
                ((ObjectExpand)obj).SetBorderThick(color.GetSelectedThick());
            if (!color.IsMultiSelectedOption())
                ((ObjectExpand)obj).nLineOption = color.GetSelectedOption();
            if (!color.IsMultiSelectedLineColor())
                ((ObjectExpand)obj).SetLineColor(color.GetSelectedLineColor());

            if (obj is GraphicModule.ObjectButtonPublic)
            {
                if (!color.IsMultiSelectedDesignType())
                    ((GraphicModule.ObjectButtonPublic)obj).DesignType = (ButtonDesignType)color.GetSelectedDesignType();
                if (!color.IsMultiSelectedRadius())
                    ((GraphicModule.ObjectButtonPublic)obj).Radius = color.GetSelectedRadius();
            }

            return true;
        }

		static bool PropertyRecv_FillColor(object obj, object prop)
		{
			Form form = (Form)prop;

            if ((string)form.Tag != "FillColor") return false;

            PropertyPageBrush color = (PropertyPageBrush)prop;

            //if (!color.IsMultiSelected())
            //{
                GraphicModule.BrushPublic bp = ((ObjectExpand)obj).GetFillColor();
                color.GetSelectedBrush(ref bp);

                ((ObjectExpand)obj).SetFillColor(bp);
            //}
            
			return true;
		}

		static bool PropertyRecv_BackColor(object obj, object prop)
		{
			Form form = (Form)prop;

			if((string)form.Tag != "BackColor")		return false;
			
			PropertyPageBrush color = (PropertyPageBrush)prop;

            //if (!color.IsMultiSelected())
            //{
                GraphicModule.BrushPublic bp = ((ObjectExpand)obj).GetBackColor();
                color.GetSelectedBrush(ref bp);
                ((ObjectExpand)obj).SetBackColor(bp);
            //}
            
			return true;
		}

		static bool PropertyRecv_TextColor(object obj, object prop)
		{
			Form form = (Form)prop;

			if((string)form.Tag != "TextColor")		return false;
			
			PropertyPageColor color = (PropertyPageColor)prop;

			if(!color.IsMultiSelected())
				((ObjectExpand)obj).SetTextColor(color.GetSelectedColor());
            
			return true;
		}

		static bool PropertyRecv_Tag(object obj, object prop)
		{
			Form form = (Form)prop;

			if((string)form.Name != "PropertyPageTag")		return false;
			
			PropertyPageTag tag = (PropertyPageTag)prop;

			tag.GetTag(ref ((ObjectTag)obj).sTagName);
            ((ObjectTag)obj).SetTagName(((ObjectTag)obj).sTagName);
			/*
			if(tag.textBoxTag.Text.Length != 0) 
			{
				((ObjectTag)obj).sTagName = tag.textBoxTag.Text;
			}
			*/

			return true;
		}

		static bool PropertyRecv_MouseResponseDigital(object obj, object prop)
		{
			Form form = (Form)prop;

			if((string)form.Name != "PropertyPageMouseResponseDigital")		return false;
			
			PropertyPageMouseResponseDigital response = (PropertyPageMouseResponseDigital)prop;

            ((ObjectTag)obj).MouseResponse = response.GetMouseResponse(((ObjectTag)obj).MouseResponse);
		
			return true;
		}

		static bool PropertyRecv_MouseResponseAnalog(object obj, object prop)
		{
			Form form = (Form)prop;

			if((string)form.Name != "PropertyPageMouseResponseAnalog")		return false;
			
			PropertyPageMouseResponseAnalog response = (PropertyPageMouseResponseAnalog)prop;

            ((ObjectTag)obj).MouseResponse = response.GetMouseResponse(((ObjectTag)obj).MouseResponse);
		
			return true;
		}

		static bool PropertyRecv_MouseResponseString(object obj, object prop)
		{
			Form form = (Form)prop;

			if((string)form.Name != "PropertyPageMouseResponseString")		return false;
			
			PropertyPageMouseResponseString response = (PropertyPageMouseResponseString)prop;

            ((ObjectTag)obj).MouseResponse = response.GetMouseResponse(((ObjectTag)obj).MouseResponse);
		
			return true;
		}

        static bool PropertyRecv_MouseResponse(object obj, object prop)
        {
            Form form = (Form)prop;

            if ((string)form.Name != "PropertyPageMouseResponse") return false;

            PropertyPageMouseResponse response = (PropertyPageMouseResponse)prop;

            ((ObjectTag)obj).MouseResponse = response.GetMouseResponse(((ObjectTag)obj).MouseResponse);

            return true;
        }

        static bool PropertyRecv_ControlBox(object obj, object prop)
        {
            Form form = (Form)prop;

            if ((string)form.Name != "PropertyPageControlBox") return false;

            PropertyPageControlBox response = (PropertyPageControlBox)prop;

            MOUSE_RESPONSE_STRUCT r = ((ObjectTag)obj).MouseResponse;

            response.GetProp(r);
            ((ObjectTag)obj).MouseResponse = r;

            return true;
        }

		static bool PropertyRecv_ExpandOption(object obj, object prop)
		{
			Form form = (Form)prop;

			if((string)form.Name != "PropertyPageExpandOption")		return false;
			
			PropertyPageExpandOption expand = (PropertyPageExpandOption)prop;

			expand.DialogToObject((ObjectExpand)obj);

			return true;
		}

		// 이 Method는 ObjectTag에서 상속하지 않을 때 사용한다.
		static bool PropertyRecv_TagLocal(object obj, object prop, ref string tagname)
		{
			Form form = (Form)prop;

			if((string)form.Name != "PropertyPageTag")		return false;
			 
			PropertyPageTag tag = (PropertyPageTag)prop;

			tag.GetTag(ref tagname);
			/*
			if(tag.textBoxTag.Text.Length != 0) 
			{
				tagname = tag.textBoxTag.Text;
			}
			*/

			return true;
		}

		// 이 Method는 ObjectTag에서 상속하지 않을 때 사용한다.
		static bool PropertyRecv_ListData(object obj, object prop, ref ArrayList array)
		{
			Form form = (Form)prop;

			if((string)form.Name != "PropertyPageListData")		return false;
			 
			PropertyPageListData tag = (PropertyPageListData)prop;

			array = tag.ListData;
			/*
			if(tag.textBoxTag.Text.Length != 0) 
			{
				tagname = tag.textBoxTag.Text;
			}
			*/

			return true;
		}

		public static void PropertyButtonModule3D(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyButtonModule3D_Recv));

			PropertyPageObjectButtonModule3D local = new PropertyPageObjectButtonModule3D();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			local = (PropertyPageObjectButtonModule3D)propertySheet.AddPage(local, true, multi_select); 
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageButtonThick(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectButtonModule3D obj2 = (ObjectButtonModule3D)obj;

			local.SetObjectArgs(obj2);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyButtonModule3D_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_TextColor(obj, prop))	return;
			if(PropertyRecv_BackColor(obj, prop))	return;
            if (PropertyRecv_ButtonThick(obj, prop)) return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectButtonModule3D") 
			{
				PropertyPageObjectButtonModule3D local = (PropertyPageObjectButtonModule3D)prop;
				ObjectButtonModule3D obj2 = (ObjectButtonModule3D)obj;

				local.GetObjectArgs(obj2);
			}
		}

		public static void PropertyButtonModuleHide(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyButtonModuleHide_Recv));

			PropertyPageObjectButtonModuleHide local = new PropertyPageObjectButtonModuleHide();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			local = (PropertyPageObjectButtonModuleHide)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectButtonModuleHide obj2 = (ObjectButtonModuleHide)obj;

			local.Set(obj2.ModuleName);
			//local.textBoxModule.Text = obj2.ModuleName;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;
			
			expand.bUseZoneDisplay = false;
			expand.bUseVisible = false;
			expand.bUseBlinking = false;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorLine = false;
			expand.bUseColorFill = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyButtonModuleHide_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;

			if(((Form)prop).Name == "PropertyPageObjectButtonModuleHide") 
			{
				PropertyPageObjectButtonModuleHide local = (PropertyPageObjectButtonModuleHide)prop;
				ObjectButtonModuleHide obj2 = (ObjectButtonModuleHide)obj;

				string imsi = obj2.ModuleName;
				local.Get(ref imsi);
				obj2.ModuleName = imsi;
				//obj2.ModuleName = local.textBoxModule.Text;
			}
		}

		public static void PropertyButtonProgram(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyButtonProgram_Recv));

			PropertyPageObjectButtonProgram local = new PropertyPageObjectButtonProgram();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj); 
			propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageButtonThick(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectButtonProgram obj2 = (ObjectButtonProgram)obj;

			local.textBoxText.Text = obj2.Text;
			if(obj2.scriptButton == null)
				local.scriptTemp = new ScriptClass();
			else
				local.scriptTemp = (ScriptClass)Tools.CopyObject(obj2.scriptButton);

			local.ScriptType = obj2.nScriptType;
			local.textBoxFilename.Text = obj2.sFileName;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyButtonProgram_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_TextColor(obj, prop))	return;
			if(PropertyRecv_BackColor(obj, prop))	return;
            if (PropertyRecv_ButtonThick(obj, prop)) return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectButtonProgram")
			{
				PropertyPageObjectButtonProgram local = (PropertyPageObjectButtonProgram)prop;
				ObjectButtonProgram obj2 = (ObjectButtonProgram)obj;

				obj2.Text = local.textBoxText.Text;
				obj2.scriptButton = local.scriptTemp;
				obj2.nScriptType = local.ScriptType;
				obj2.sFileName = local.textBoxFilename.Text;
			}
		}


		public static void PropertyButtonDigitalOut(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyButtonDigitalOut_Recv));

			PropertyPageObjectButtonDigitalOut local = new PropertyPageObjectButtonDigitalOut();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj); 
			propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageButtonThick(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectButtonDigitalOut obj2 = (ObjectButtonDigitalOut)obj;

			local.textBoxText.Text = obj2.Text;
			local.DelaySec = obj2.DoDelayTime;
			local.Member = obj2.Member;
			local.DoMothod = obj2.DoMethod;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyButtonDigitalOut_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_TextColor(obj, prop))	return;
			if(PropertyRecv_BackColor(obj, prop))	return;
            if (PropertyRecv_ButtonThick(obj, prop)) return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectButtonDigitalOut") 
			{
				PropertyPageObjectButtonDigitalOut local = (PropertyPageObjectButtonDigitalOut)prop;
				ObjectButtonDigitalOut obj2 = (ObjectButtonDigitalOut)obj;

				obj2.Text = local.textBoxText.Text;
				obj2.Text = local.textBoxText.Text;
				obj2.DoDelayTime = local.DelaySec;
				obj2.Member = local.Member;
				obj2.DoMethod = local.DoMothod;
			}
		}

		public static void PropertyDigitalAnimation(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyDigitalAnimation_Recv));

			PropertyPageObjectDigitalAnimation local = new PropertyPageObjectDigitalAnimation();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			local = (PropertyPageObjectDigitalAnimation)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageTagDI(obj);
			propertySheet.AddPageMouseResponseDigital(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectDigitalAnimation obj2 = (ObjectDigitalAnimation)obj;

			local.SetObjectArgs(obj2.objArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyDigitalAnimation_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Tag(obj, prop))	return;
			if(PropertyRecv_ExpandOption(obj, prop))return;
			if(PropertyRecv_MouseResponseDigital(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;

			if(((Form)prop).Name == "PropertyPageObjectDigitalAnimation") 
			{
				PropertyPageObjectDigitalAnimation local = (PropertyPageObjectDigitalAnimation)prop;
				ObjectDigitalAnimation obj2 = (ObjectDigitalAnimation)obj;

				_ = obj2.SetObjectArgs(local.GetObjectArgs(obj2.objArgs));

				if(local.checkBoxRestore.Checked)
				{
					obj2.SetOriginalSize();
					formEditor.SelectListUpdate();
				}

                obj2.bSetOriginalSizeOnStudio = local.checkBoxRestore.Checked;
			}
		}

		public static void PropertyDigitalCircle(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyDigitalCircle_Recv));

			PropertyPageObjectDigitalCircle local = new PropertyPageObjectDigitalCircle();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			local = (PropertyPageObjectDigitalCircle)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageTagDI(obj);
			propertySheet.AddPageMouseResponseDigital(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectDigitalCircle obj2 = (ObjectDigitalCircle)obj;

			local.SetObjectArgs(obj2.objArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorLine = false;
			expand.bUseColorFill = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyDigitalCircle_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Tag(obj, prop))	return;
			//if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))return;
			if(PropertyRecv_MouseResponseDigital(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;

			if(((Form)prop).Name == "PropertyPageObjectDigitalCircle") 
			{
				PropertyPageObjectDigitalCircle local = (PropertyPageObjectDigitalCircle)prop;
				ObjectDigitalCircle obj2 = (ObjectDigitalCircle)obj;

				ObjectArgsDigitalCircle args = local.GetObjectArgs(obj2.objArgs);

				obj2.objArgs.colorOn = args.colorOn;
				obj2.objArgs.colorOff = args.colorOff;
			}
		}

		public static void PropertyDigitalRectangle(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyDigitalRectangle_Recv));

			PropertyPageObjectDigitalRectangle local = new PropertyPageObjectDigitalRectangle();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			local = (PropertyPageObjectDigitalRectangle)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageTagDI(obj);
			propertySheet.AddPageMouseResponseDigital(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectDigitalRectangle obj2 = (ObjectDigitalRectangle)obj;

			local.SetObjectArgs(obj2.objArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorLine = false;
			expand.bUseColorFill = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyDigitalRectangle_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Tag(obj, prop))	return;
			//if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))return;
			if(PropertyRecv_MouseResponseDigital(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;

			if(((Form)prop).Name == "PropertyPageObjectDigitalRectangle") 
			{
				PropertyPageObjectDigitalRectangle local = (PropertyPageObjectDigitalRectangle)prop;
				ObjectDigitalRectangle obj2 = (ObjectDigitalRectangle)obj;

				ObjectArgsDigitalRectangle args = local.GetObjectArgs(obj2.objArgs);

				obj2.objArgs.colorOn = args.colorOn;
				obj2.objArgs.colorOff = args.colorOff;
				obj2.objArgs.nDisplayMethod = args.nDisplayMethod;
			}
		}

		public static void PropertyDigitalString(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyDigitalString_Recv));

			PropertyPageObjectDigitalString local = new PropertyPageObjectDigitalString();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			local = (PropertyPageObjectDigitalString)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageTagDI(obj);
			propertySheet.AddPageMouseResponseDigital(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectDigitalString obj2 = (ObjectDigitalString)obj;

			local.SetObjectArgs(obj2.objArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorLine = false;
			expand.bUseColorFill = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyDigitalString_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Tag(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))return;
			if(PropertyRecv_MouseResponseDigital(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;

			if(((Form)prop).Name == "PropertyPageObjectDigitalString") 
			{
				PropertyPageObjectDigitalString local = (PropertyPageObjectDigitalString)prop;
				ObjectDigitalString obj2 = (ObjectDigitalString)obj;

				ObjectArgsDigitalString args = local.GetObjectArgs(obj2.objArgs);

				obj2.objArgs.colorOn = args.colorOn;
				obj2.objArgs.colorOff = args.colorOff;
				obj2.objArgs.colorBack = args.colorBack;
			}
		}

		static string String눈금색() 
		{
			if(Tools.IsLangKorean())
				return "눈금색";
			else if(Tools.IsLangJapanese())
				return "目盛り色";
			else if(Tools.IsLangChinese())
				return "网格颜色";
			else 
				return "GuideColor";
		}

        static string String커서색()
        {
            if (Tools.IsLangKorean())
                return "커서색";
            else
                return "CursorColor";
        }

		public static void PropertyAnalogMeter(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyAnalogMeter_Recv));

			PropertyPageObjectAnalogMeter local = new PropertyPageObjectAnalogMeter();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색());

			ObjectAnalogMeter obj2 = (ObjectAnalogMeter)obj;

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageTagAI(obj);
			propertySheet.AddPageMouseResponseAnalog(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj); 
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageLineColor(obj);
			propertySheet.AddPageColor(gcolor, obj2.GuideColor);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			//local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorLine = true;
			expand.bUseColorFill = true;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = true;

			propertySheet.Run();
		}

		private static void PropertyAnalogMeter_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Tag(obj, prop))			return;
			if(PropertyRecv_TextColor(obj, prop))	return;
			if(PropertyRecv_BackColor(obj, prop))	return;
			if(PropertyRecv_LineColor(obj, prop))	return;
			if(PropertyRecv_FillColor(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))return;
			if(PropertyRecv_MouseResponseAnalog(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;

			if(((Form)prop).Name == "PropertyPageObjectAnalogMeter") 
			{
				PropertyPageObjectAnalogMeter local = (PropertyPageObjectAnalogMeter)prop;
				ObjectAnalogMeter obj2 = (ObjectAnalogMeter)obj;

				//obj2.ObjectArgs = local.ObjectArgs;
				return;
			}
            if ((string)prop.Tag == "GuideLineColor") 
			{
				PropertyPageColor local = (PropertyPageColor)prop;
				ObjectAnalogMeter obj2 = (ObjectAnalogMeter)obj;

				if(!local.IsMultiSelected()) 
					obj2.GuideColor = local.GetSelectedColor();

				return;
			}
		}

		public static void PropertyAnalogRectangle(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyAnalogRectangle_Recv));

			PropertyPageObjectAnalogRectangle local = new PropertyPageObjectAnalogRectangle();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			//PropertyPageColor gcolor = new PropertyPageColor("MeterGuideLine", "GuideColor");

			ObjectAnalogRectangle obj2 = (ObjectAnalogRectangle)obj;

			local = (PropertyPageObjectAnalogRectangle)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageTagAI(obj);
            propertySheet.AddPageMouseResponseAnalog(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageFillColor(obj); 
			propertySheet.AddPageBackColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);
			local.SetViewRange(obj2.ViewRange);
			local.SetGuideLine(obj2.GuideLine);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorLine = false;
			expand.bUseColorFill = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		
		}

		private static void PropertyAnalogRectangle_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Tag(obj, prop))			return;
			if(PropertyRecv_MouseResponseAnalog(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;
			if(PropertyRecv_FillColor(obj, prop))	return;
			if(PropertyRecv_BackColor(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectAnalogRectangle") 
			{
				PropertyPageObjectAnalogRectangle local = (PropertyPageObjectAnalogRectangle)prop;
				ObjectAnalogRectangle obj2 = (ObjectAnalogRectangle)obj;

				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
				obj2.ViewRange = local.GetViewRange(obj2.ViewRange);
				obj2.GuideLine = local.GetGuideLine(obj2.GuideLine);
				return;
			}
		}

		public static void PropertyAnalogRotate(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyAnalogRotate_Recv));

			PropertyPageObjectAnalogRotate local = new PropertyPageObjectAnalogRotate();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			ObjectAnalogRotate obj2 = (ObjectAnalogRotate)obj;

			local = (PropertyPageObjectAnalogRotate)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageTagAI(obj);
            propertySheet.AddPageMouseResponseAnalog(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorLine = false;
			expand.bUseColorFill = false;
			expand.bUseColorText = false;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyAnalogRotate_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Tag(obj, prop))			return;
			if(PropertyRecv_MouseResponseAnalog(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;
			if(PropertyRecv_BackColor(obj, prop))	return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectAnalogRotate") 
			{
				PropertyPageObjectAnalogRotate local = (PropertyPageObjectAnalogRotate)prop;
				ObjectAnalogRotate obj2 = (ObjectAnalogRotate)obj;

				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
				return;
			}
		}

		public static void PropertyAnalogStatus(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyAnalogStatus_Recv));

			PropertyPageObjectAnalogStatus local = new PropertyPageObjectAnalogStatus();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			ObjectAnalogStatus obj2 = (ObjectAnalogStatus)obj;

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageTagAI(obj);
            propertySheet.AddPageMouseResponseAnalog(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.Member = obj2.Member;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorLine = false;
			expand.bUseColorFill = false;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyAnalogStatus_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Tag(obj, prop))			return;
			if(PropertyRecv_MouseResponseAnalog(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectAnalogStatus") 
			{
				PropertyPageObjectAnalogStatus local = (PropertyPageObjectAnalogStatus)prop;
				ObjectAnalogStatus obj2 = (ObjectAnalogStatus)obj;

				obj2.Member = local.Member;

				return;
			}
		}

		public static void PropertyAnalogString(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyAnalogString_Recv));

			PropertyPageObjectAnalogString local = new PropertyPageObjectAnalogString();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			local = (PropertyPageObjectAnalogString)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageTagAI(obj);
            propertySheet.AddPageMouseResponseAnalog(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectAnalogString obj2 = (ObjectAnalogString)obj;

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyAnalogString_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	 return;
			if(PropertyRecv_Tag(obj, prop))			 return;
			if(PropertyRecv_TextColor(obj, prop))	 return;
			if(PropertyRecv_BackColor(obj, prop))	 return;
			if(PropertyRecv_Font(obj, prop))		 return;
			if(PropertyRecv_ExpandOption(obj, prop)) return;
			if(PropertyRecv_MouseResponseAnalog(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;

			if(((Form)prop).Name == "PropertyPageObjectAnalogString") 
			{
				PropertyPageObjectAnalogString local = (PropertyPageObjectAnalogString)prop;
				ObjectAnalogString obj2 = (ObjectAnalogString)obj;

				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);

			}
		}

        public static void PropertyAnalogGauge(object obj, bool multi_select) //아날로그 게이지 추가 25-02-04 hsjeong
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyAnalogGauge_Recv));

            PropertyPageObjectAnalogGauge local = new PropertyPageObjectAnalogGauge();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            //PropertyPageColor gcolor = new PropertyPageColor("MeterGuideLine", "GuideColor");

            ObjectAnalogGauge obj2 = (ObjectAnalogGauge)obj;

            local = (PropertyPageObjectAnalogGauge)propertySheet.AddPage(local, true, multi_select);
            propertySheet.AddPageTagAI(obj);
            propertySheet.AddPageMouseResponseAnalog(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
            propertySheet.AddPageFont(obj);
            //propertySheet.AddPageLineColor2(obj);

            propertySheet.AddPageFillColor(obj);
            propertySheet.AddPageBackColor(obj);

            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            local.AddComboBoxItems();
            local.SetObjectArgs(obj2.ObjectArgs);
            local.SetViewRange(obj2.ViewRange2);
            local.SetGuideLine(obj2.GuideLine2);
            local.SetPointMember(obj2.Pointmember);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorLine = false;
            expand.bUseColorFill = true;
            expand.bUseColorText = false;
            expand.bUseColorBack = true;
            expand.bUseThickLine = false;

            propertySheet.Run();

        }

        private static void PropertyAnalogGauge_Recv(object obj, Form prop) //아날로그 게이지 추가 hsjeong 25-02-04
        {
            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_Tag(obj, prop)) return;
            if (PropertyRecv_MouseResponseAnalog(obj, prop)) return;
            if (PropertyRecv_ControlBox(obj, prop)) return;
            if (PropertyRecv_FillColor(obj, prop)) return;
            if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_Font(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectAnalogGauge")
            {
                PropertyPageObjectAnalogGauge local = (PropertyPageObjectAnalogGauge)prop;
                ObjectAnalogGauge obj2 = (ObjectAnalogGauge)obj;

                obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
                obj2.ViewRange2 = local.GetViewRange(obj2.ViewRange2);
                obj2.GuideLine2 = local.GetGuideLine(obj2.GuideLine2);
                obj2.Pointmember = local.GetPointMember(obj2.Pointmember);
                return;
            }
        }

		public static void PropertyStringString(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyStringString_Recv));

			PropertyPageObjectStringString local = new PropertyPageObjectStringString();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			local = (PropertyPageObjectStringString)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageTagST(obj);
			propertySheet.AddPageMouseResponseString(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectStringString obj2 = (ObjectStringString)obj;

			local.SetObjectArgs(obj2.ObjectArgs);
			local.SetTextAlign(obj2.TextAlign);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;
            
			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyStringString_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	 return;
			if(PropertyRecv_Tag(obj, prop))			 return;
			if(PropertyRecv_TextColor(obj, prop))	 return;
			if(PropertyRecv_BackColor(obj, prop))	 return;
			if(PropertyRecv_Font(obj, prop))		 return;
			if(PropertyRecv_ExpandOption(obj, prop)) return;
			if(PropertyRecv_MouseResponseString(obj, prop))return;
            if (PropertyRecv_ControlBox(obj, prop)) return;

			if(((Form)prop).Name == "PropertyPageObjectStringString") 
			{
				PropertyPageObjectStringString local = (PropertyPageObjectStringString)prop;
				ObjectStringString obj2 = (ObjectStringString)obj;

				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
				obj2.TextAlign = local.GetTextAlign(obj2.TextAlign);

			}
		}


		public static void PropertyModule(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyModule_Recv));

			PropertyPageObjectModule local = new PropertyPageObjectModule();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectModule obj2 = (ObjectModule)obj;

			local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
			expand.bUseZoneDisplay = false;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyModule_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	 return;
			if(PropertyRecv_ExpandOption(obj, prop)) return;

			if(((Form)prop).Name == "PropertyPageObjectModule") 
			{
				PropertyPageObjectModule local = (PropertyPageObjectModule)prop;
				ObjectModule obj2 = (ObjectModule)obj;

				obj2.ObjectArgs = local.ObjectArgs;
			}
		}

		public static void PropertyControlCheckBox(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyControlCheckBox_Recv));

			PropertyPageObjectControlCheckBox local = new PropertyPageObjectControlCheckBox();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectControlCheckBox obj2 = (ObjectControlCheckBox)obj;

			local = (PropertyPageObjectControlCheckBox)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageTagLocalAll(obj, obj2.ObjectArgs.sTag);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = true;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyControlCheckBox_Recv(object obj, Form prop)
		{
			ObjectControlCheckBox obj2 = (ObjectControlCheckBox)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TagLocal(obj, prop, ref obj2.ObjectArgs.sTag))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectControlCheckBox") 
			{
				PropertyPageObjectControlCheckBox local = (PropertyPageObjectControlCheckBox)prop;

				// obj2.ObjectArgs = local.ObjectArgs 식으로 하면 안됨
				// 윗부분에서 값을 변경했으므로 필요한 변수만 바꿀 것
				ObjectArgsControlCheckBox args = local.GetObjectArgs(obj2.ObjectArgs);

				obj2.ObjectArgs.sTitle = args.sTitle;
			}
		}

		public static void PropertyControlComboBox(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyControlComboBox_Recv));

			PropertyPageObjectControlComboBox local = new PropertyPageObjectControlComboBox();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectControlComboBox obj2 = (ObjectControlComboBox)obj; 

			local = (PropertyPageObjectControlComboBox)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageListData(obj, obj2.ObjectArgs.arrayListData, multi_select);
			propertySheet.AddPageTagLocalAll(obj, obj2.ObjectArgs.sTag);
            propertySheet.AddPageFont(obj);
            propertySheet.AddPageTextColor(obj);
            propertySheet.AddPageBackColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = true;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
			expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
            expand.bUseColorText = true;
            expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyControlComboBox_Recv(object obj, Form prop)
		{
			ObjectControlComboBox obj2 = (ObjectControlComboBox)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TagLocal(obj, prop, ref obj2.ObjectArgs.sTag))	return;
			if(PropertyRecv_ListData(obj, prop, ref obj2.ObjectArgs.arrayListData))	return;
            if (PropertyRecv_TextColor(obj, prop)) return;
            if (PropertyRecv_BackColor(obj, prop)) return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectControlComboBox") 
			{
				PropertyPageObjectControlComboBox local = (PropertyPageObjectControlComboBox)prop;
				
				// obj2.ObjectArgs = local.ObjectArgs 식으로 하면 안됨
				// 윗부분에서 값을 변경했으므로 필요한 변수만 바꿀 것
				ObjectArgsControlComboBox args = local.GetObjectArgs(obj2.ObjectArgs);

				obj2.ObjectArgs.dwWindowStyle = args.dwWindowStyle;
				obj2.ObjectArgs.nValueConvert = args.nValueConvert;
			}
		}

		public static void PropertyControlEditBox(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyControlEditBox_Recv));

			PropertyPageObjectControlEditBox local = new PropertyPageObjectControlEditBox();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectControlEditBox obj2 = (ObjectControlEditBox)obj; 

			local = (PropertyPageObjectControlEditBox)propertySheet.AddPage(local, true, multi_select);
			//propertySheet.AddPageListData(obj, obj2.ObjectArgs.arrayListData, multi_select);
			propertySheet.AddPageTagLocalAll(obj, obj2.ObjectArgs.sTag);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
            propertySheet.AddPageBackColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = true;
			expand.bUseEventSelChange = false;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyControlEditBox_Recv(object obj, Form prop)
		{
			ObjectControlEditBox obj2 = (ObjectControlEditBox)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TagLocal(obj, prop, ref obj2.ObjectArgs.sTag))	return;
			//if(PropertyRecv_ListData(obj, prop, ref obj2.ObjectArgs.arrayListData))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
            if(PropertyRecv_BackColor(obj, prop)) return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectControlEditBox") 
			{
				PropertyPageObjectControlEditBox local = (PropertyPageObjectControlEditBox)prop;
				
				// obj2.ObjectArgs = local.ObjectArgs 식으로 하면 안됨
				// 윗부분에서 값을 변경했으므로 필요한 변수만 바꿀 것
				ObjectArgsControlEditBox args = local.GetObjectArgs(obj2.ObjectArgs);

				obj2.ObjectArgs.dwWindowStyle = args.dwWindowStyle;
                obj2.ObjectArgs.nHorzAlign = args.nHorzAlign;
			}
		}

		public static void PropertyControlListBox(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyControlListBox_Recv));

			PropertyPageObjectControlListBox local = new PropertyPageObjectControlListBox();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectControlListBox obj2 = (ObjectControlListBox)obj; 

			local = (PropertyPageObjectControlListBox)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageListData(obj, obj2.ObjectArgs.arrayListData, multi_select);
			propertySheet.AddPageTagLocalAll(obj, obj2.ObjectArgs.sTag);
            propertySheet.AddPageFont(obj);
            propertySheet.AddPageTextColor(obj);
            propertySheet.AddPageBackColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = true;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
            expand.bUseColorText = true;
            expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyControlListBox_Recv(object obj, Form prop)
		{
			ObjectControlListBox obj2 = (ObjectControlListBox)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TagLocal(obj, prop, ref obj2.ObjectArgs.sTag))	return;
			if(PropertyRecv_ListData(obj, prop, ref obj2.ObjectArgs.arrayListData))	return;
            if (PropertyRecv_TextColor(obj, prop)) return;
            if (PropertyRecv_BackColor(obj, prop)) return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectControlListBox") 
			{
				PropertyPageObjectControlListBox local = (PropertyPageObjectControlListBox)prop;
				
				// obj2.ObjectArgs = local.ObjectArgs 식으로 하면 안됨
				// 윗부분에서 값을 변경했으므로 필요한 변수만 바꿀 것
				ObjectArgsControlListBox args = local.GetObjectArgs(obj2.ObjectArgs);

				obj2.ObjectArgs.dwWindowStyle = args.dwWindowStyle;
				obj2.ObjectArgs.nValueConvert = args.nValueConvert;
                obj2.ObjectArgs.nSelectionMode = args.nSelectionMode;
			}
		}

		public static void PropertyControlRadioButton(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyControlRadioButton_Recv));

			//PropertyPageObjectControlRadioButton local = new PropertyPageObjectControlRadioButton();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectControlRadioButton obj2 = (ObjectControlRadioButton)obj; 

			//propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageListData(obj, obj2.ObjectArgs.arrayListData, multi_select);
			propertySheet.AddPageTagLocalAll(obj, obj2.ObjectArgs.sTag);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			//local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = true;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyControlRadioButton_Recv(object obj, Form prop)
		{
			ObjectControlRadioButton obj2 = (ObjectControlRadioButton)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TagLocal(obj, prop, ref obj2.ObjectArgs.sTag))	return;
			if(PropertyRecv_ListData(obj, prop, ref obj2.ObjectArgs.arrayListData))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectControlRadioButton") 
			{
				PropertyPageObjectControlRadioButton local = (PropertyPageObjectControlRadioButton)prop;
				
				// obj2.ObjectArgs = local.ObjectArgs 식으로 하면 안됨
				// 윗부분에서 값을 변경했으므로 필요한 변수만 바꿀 것
				//obj2.ObjectArgs.dwWindowStyle = local.ObjectArgs.dwWindowStyle;
				//obj2.ObjectArgs.nValueConvert = local.ObjectArgs.nValueConvert;
			}
		}

        public static void PropertyControlDatePicker(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyControlDatePicker_Recv));

            PropertyPageObjectControlDatePicker local = new PropertyPageObjectControlDatePicker();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            ObjectControlDatePicker obj2 = (ObjectControlDatePicker)obj;

            local = (PropertyPageObjectControlDatePicker)propertySheet.AddPage(local, true, multi_select);
            //propertySheet.AddPageListData(obj, obj2.ObjectArgs.arrayListData, multi_select);
            //propertySheet.AddPageTagLocalAll(obj, obj2.ObjectArgs.sTag);
            propertySheet.AddPageFont(obj);
            //propertySheet.AddPageTextColor(obj);
            //propertySheet.AddPageBackColor(obj);

            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            local.SetObjectArgs(obj2.ObjectArgs);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            //expand.bUseMouseDown = false;
            //expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyControlDatePicker_Recv(object obj, Form prop)
        {
            ObjectControlDatePicker obj2 = (ObjectControlDatePicker)obj;

            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;
            //if (PropertyRecv_TagLocal(obj, prop, ref obj2.ObjectArgs.sTag)) return;
            //if(PropertyRecv_ListData(obj, prop, ref obj2.ObjectArgs.arrayListData))	return;
            //if (PropertyRecv_TextColor(obj, prop)) return;
            //if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_Font(obj, prop)) return;

            if (prop.GetType() == typeof(PropertyPageObjectControlDatePicker))
            {
                PropertyPageObjectControlDatePicker local = (PropertyPageObjectControlDatePicker)prop;

                // obj2.ObjectArgs = local.ObjectArgs 식으로 하면 안됨
                // 윗부분에서 값을 변경했으므로 필요한 변수만 바꿀 것
                ObjectArgsControlDatePicker args = local.GetObjectArgs(obj2.ObjectArgs);

                //obj2.ObjectArgs.dwWindowStyle = args.dwWindowStyle;
                obj2.ObjectArgs.sFormat = args.sFormat;
            }
        }

        public static void PropertyControlTabControl(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyControlTabControl_Recv));

            PropertyPageObjectControlTabControl local = new PropertyPageObjectControlTabControl();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            ObjectControlTabControl obj2 = (ObjectControlTabControl)obj;

            local = (PropertyPageObjectControlTabControl)propertySheet.AddPage(local, true, multi_select);
            //propertySheet.AddPageListData(obj, obj2.ObjectArgs.arrayListData, multi_select);
            //propertySheet.AddPageTagLocalAll(obj, obj2.ObjectArgs.sTag);
            propertySheet.AddPageFont(obj);
            //propertySheet.AddPageTextColor(obj);
            //propertySheet.AddPageBackColor(obj);

            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            local.SetObjectArgs(obj2.ObjectArgs);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            //expand.bUseMouseDown = false;
            //expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyControlTabControl_Recv(object obj, Form prop)
        {
            ObjectControlTabControl obj2 = (ObjectControlTabControl)obj;

            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;
            //if (PropertyRecv_TagLocal(obj, prop, ref obj2.ObjectArgs.sTag)) return;
            //if(PropertyRecv_ListData(obj, prop, ref obj2.ObjectArgs.arrayListData))	return;
            //if (PropertyRecv_TextColor(obj, prop)) return;
            //if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_Font(obj, prop)) return;

            if (prop.GetType() == typeof(PropertyPageObjectControlTabControl))
            {
                PropertyPageObjectControlTabControl local = (PropertyPageObjectControlTabControl)prop;

                // obj2.ObjectArgs = local.ObjectArgs 식으로 하면 안됨
                // 윗부분에서 값을 변경했으므로 필요한 변수만 바꿀 것
                ObjectArgsControlTabControl args = local.GetObjectArgs(obj2.ObjectArgs);

                //obj2.ObjectArgs.dwWindowStyle = args.dwWindowStyle;
                obj2.ObjectArgs.sFormat = args.sFormat;
            }
        }


        public static void PropertyControlTreeView(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyControlTreeView_Recv));

            PropertyPageObjectControlTreeView local = new PropertyPageObjectControlTreeView();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            ObjectControlTreeView obj2 = (ObjectControlTreeView)obj;

            local = (PropertyPageObjectControlTreeView)propertySheet.AddPage(local, true, multi_select);
            //propertySheet.AddPageListData(obj, obj2.ObjectArgs.arrayListData, multi_select);
            //propertySheet.AddPageTagLocalAll(obj, obj2.ObjectArgs.sTag);
            propertySheet.AddPageFont(obj);
            propertySheet.AddPageTextColor(obj);
            propertySheet.AddPageBackColor(obj);

            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            local.SetObjectArgs(obj2.ObjectArgs);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = true;
            expand.bUseEventSelChange = true;
            //expand.bUseMouseDown = false;
            //expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyControlTreeView_Recv(object obj, Form prop)
        {
            

            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;
            //if (PropertyRecv_TagLocal(obj, prop, ref obj2.ObjectArgs.sTag)) return;
            //if(PropertyRecv_ListData(obj, prop, ref obj2.ObjectArgs.arrayListData))	return;
            if (PropertyRecv_TextColor(obj, prop)) return;
            if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_Font(obj, prop)) return;

            if (prop.GetType() == typeof(PropertyPageObjectControlTreeView))
            {
                PropertyPageObjectControlTreeView local = (PropertyPageObjectControlTreeView)prop;
                ObjectControlTreeView obj2 = (ObjectControlTreeView)obj;

                obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
            }
        }

		public static void PropertyBitmap(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyBitmap_Recv));

			PropertyPageObjectBitmap local = new PropertyPageObjectBitmap();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectBitmap obj2 = (ObjectBitmap)obj; 

			local = (PropertyPageObjectBitmap)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyBitmap_Recv(object obj, Form prop)
		{
			ObjectBitmap obj2 = (ObjectBitmap)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;

			if(((Form)prop).Name == "PropertyPageObjectBitmap") 
			{
				PropertyPageObjectBitmap local = (PropertyPageObjectBitmap)prop;
				
				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
				if(local.checkBoxRestoreToOrizinalSize.Checked)	 
				{
					obj2.SetOriginalSize();
					formEditor.SelectListUpdate();
				}

                obj2.bSetOriginalSizeOnStudio = local.checkBoxRestoreToOrizinalSize.Checked;
			}
		}

		public static void PropertyAnimation(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyAnimation_Recv));

			PropertyPageObjectAnimation local = new PropertyPageObjectAnimation();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectAnimation obj2 = (ObjectAnimation)obj; 

			local = (PropertyPageObjectAnimation)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = true;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyAnimation_Recv(object obj, Form prop)
		{
			ObjectAnimation obj2 = (ObjectAnimation)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;

			if(((Form)prop).Name == "PropertyPageObjectAnimation")
			{
				PropertyPageObjectAnimation local = (PropertyPageObjectAnimation)prop;
				
				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
				if(local.checkBoxRestoreToOrizinalSize.Checked)	 
				{
					obj2.SetOriginalSize();
					formEditor.SelectListUpdate();
				}

                obj2.bSetOriginalSizeOnStudio = local.checkBoxRestoreToOrizinalSize.Checked;
			}
		}

		public static void PropertySingleText(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertySingleText_Recv));

			PropertyPageObjectSingleText local = new PropertyPageObjectSingleText();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectSingleText obj2 = (ObjectSingleText)obj; 

			local = (PropertyPageObjectSingleText)propertySheet.AddPage(local, true, multi_select);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = true;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertySingleText_Recv(object obj, Form prop)
		{
			ObjectSingleText obj2 = (ObjectSingleText)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop)) 
			{
				obj2.RecalcRectSize(formEditor.CreateGraphics());
				formEditor.SelectListUpdate();
				return;
			}

			if(((Form)prop).Name == "PropertyPageObjectSingleText") 
			{
				PropertyPageObjectSingleText local = (PropertyPageObjectSingleText)prop;
				
				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
				obj2.RecalcRectSize(formEditor.CreateGraphics());
				formEditor.SelectListUpdate();
			}
		}

		public static void PropertyRectangle(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyRectangle_Recv));

			PropertyPageObjectRectangle local = new PropertyPageObjectRectangle();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectRectangle obj2 = (ObjectRectangle)obj; 

			local = (PropertyPageObjectRectangle)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageLineColor(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseColorLine = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = true;

			propertySheet.Run();
		}

		private static void PropertyRectangle_Recv(object obj, Form prop)
		{
			ObjectRectangle obj2 = (ObjectRectangle)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_LineColor(obj, prop))		return;
			if(PropertyRecv_FillColor(obj, prop))		return;

			if(((Form)prop).Name == "PropertyPageObjectRectangle") 
			{
				PropertyPageObjectRectangle local = (PropertyPageObjectRectangle)prop;
				
				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
			}
		}

		public static void PropertyRoundRectangle(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyRoundRectangle_Recv));

			PropertyPageObjectRoundRectangle local = new PropertyPageObjectRoundRectangle();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectRoundRectangle obj2 = (ObjectRoundRectangle)obj; 

			local = (PropertyPageObjectRoundRectangle)propertySheet.AddPage(local, true, multi_select);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageLineColor(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseColorLine = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = true;

			propertySheet.Run();
		}

		private static void PropertyRoundRectangle_Recv(object obj, Form prop)
		{
			ObjectRoundRectangle obj2 = (ObjectRoundRectangle)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_FillColor(obj, prop))		return;
			if(PropertyRecv_LineColor(obj, prop))		return;
			

			if(((Form)prop).Name == "PropertyPageObjectRoundRectangle") 
			{
				PropertyPageObjectRoundRectangle local = (PropertyPageObjectRoundRectangle)prop;

				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
			}
		}

		public static void PropertyCircle(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyCircle_Recv));

			PropertyPageObjectCircle local = new PropertyPageObjectCircle();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectCircle obj2 = (ObjectCircle)obj; 

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageLineColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.objArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseColorLine = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = true;

			propertySheet.Run();
		}

		private static void PropertyCircle_Recv(object obj, Form prop)
		{
			ObjectCircle obj2 = (ObjectCircle)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_LineColor(obj, prop))		return;
			if(PropertyRecv_FillColor(obj, prop))		return;

			if(((Form)prop).Name == "PropertyPageObjectCircle") 
			{
				PropertyPageObjectCircle local = (PropertyPageObjectCircle)prop;
				
				obj2.objArgs = local.GetObjectArgs(obj2.objArgs);
			}
		}

		public static void PropertyLine(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyLine_Recv));

			PropertyPageObjectLine local = new PropertyPageObjectLine();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectLine obj2 = (ObjectLine)obj; 

			//propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageLineColor(obj);

			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			//local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseColorLine = true;
			//expand.bUseColorFill = true;
			expand.bUseThickLine = true;

			propertySheet.Run();
		}

		private static void PropertyLine_Recv(object obj, Form prop)
		{
			ObjectLine obj2 = (ObjectLine)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_LineColor(obj, prop))		return;

			if(((Form)prop).Name == "PropertyPageObjectLine") 
			{
				PropertyPageObjectLine local = (PropertyPageObjectLine)prop;
				
				// obj2.ObjectArgs = local.ObjectArgs;
				// obj2.RecalcRectSize(formEditor.CreateGraphics());
				// formEditor.SelectListUpdate();
			}
		}

		public static void PropertyText(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyText_Recv));

			PropertyPageObjectText local = new PropertyPageObjectText();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectText obj2 = (ObjectText)obj; 

			local = (PropertyPageObjectText)propertySheet.AddPage(local, true, multi_select);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = true;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyText_Recv(object obj, Form prop)
		{
			ObjectText obj2 = (ObjectText)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectText") 
			{
				PropertyPageObjectText local = (PropertyPageObjectText)prop;
				
				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
			}
		}

		public static void PropertyDate(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyDate_Recv));

			PropertyPageObjectDate local = new PropertyPageObjectDate();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectDate obj2 = (ObjectDate)obj; 

			propertySheet.AddPage(local, false, multi_select);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyDate_Recv(object obj, Form prop)
		{
			ObjectDate obj2 = (ObjectDate)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_BackColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectDate") 
			{
				PropertyPageObjectDate local = (PropertyPageObjectDate)prop;
				
				obj2.ObjectArgs = local.ObjectArgs;
			}
		}


		public static void PropertyChangeValueDisplay(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyChangeValueDisplay_Recv));

			PropertyPageObjectChangeValueDisplay local = new PropertyPageObjectChangeValueDisplay();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectChangeValueDisplay obj2 = (ObjectChangeValueDisplay)obj; 

			local = (PropertyPageObjectChangeValueDisplay)propertySheet.AddPage(local, true, multi_select);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyChangeValueDisplay_Recv(object obj, Form prop)
		{
			ObjectChangeValueDisplay obj2 = (ObjectChangeValueDisplay)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_BackColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectChangeValueDisplay") 
			{
				PropertyPageObjectChangeValueDisplay local = (PropertyPageObjectChangeValueDisplay)prop;

				ObjectArgsChangeValueDisplay args = local.GetObjectArgs(obj2.ObjectArgs);
				
				obj2.ObjectArgs.nListCount = args.nListCount;
			}
		}

		public static void PropertyClock(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyClock_Recv));

			PropertyPageObjectClock local = new PropertyPageObjectClock();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectClock obj2 = (ObjectClock)obj; 

			local = (PropertyPageObjectClock)propertySheet.AddPage(local, true, multi_select);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.SetObjectArgs(obj2.ObjectArgs);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyClock_Recv(object obj, Form prop)
		{
			ObjectClock obj2 = (ObjectClock)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_BackColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectClock") 
			{
				PropertyPageObjectClock local = (PropertyPageObjectClock)prop;
				
				obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);

                //obj2.ReMakePreviewBitmap();
			}
		}


		public static void PropertyDatabaseTrend(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyDatabaseTrend_Recv));

			PropertyPageObjectDatabaseTrend local = new PropertyPageObjectDatabaseTrend();
			PropertyPageObjectDatabaseTrendMember member = new PropertyPageObjectDatabaseTrendMember();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색());
            PropertyPageColor ccolor = new PropertyPageColor("CursorColor", String커서색());
			ObjectDatabaseTrend obj2 = (ObjectDatabaseTrend)obj; 

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPage(member, false, multi_select);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageColor(gcolor, obj2.ObjectArgs.lColorGuideLine);
            propertySheet.AddPageColor(ccolor, obj2.ObjectArgs.lColorCursor);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.ObjectArgs = obj2.ObjectArgs;

			member.GraphMember = obj2.GraphMember;
			member.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            //expand.bUseMouseLeftDown = true;
            //expand.bUseMouseLeftUp = true;
            //expand.bUseMouseRightDown = true;
            //expand.bUseMouseRightUp = true;
			//expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyDatabaseTrend_Recv(object obj, Form prop)
		{
			ObjectDatabaseTrend obj2 = (ObjectDatabaseTrend)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_BackColor(obj, prop))		return;
			if(PropertyRecv_FillColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectDatabaseTrend") 
			{
				PropertyPageObjectDatabaseTrend local = (PropertyPageObjectDatabaseTrend)prop;
				
				obj2.ObjectArgs.wTimeSelectOption = local.ObjectArgs.wTimeSelectOption;
				obj2.ObjectArgs.wShowUnit = local.ObjectArgs.wShowUnit;
				obj2.ObjectArgs.wTimeDevide = local.ObjectArgs.wTimeDevide;
				obj2.ObjectArgs.wLevelDevide = local.ObjectArgs.wLevelDevide;
				obj2.ObjectArgs.nDataCycle = local.ObjectArgs.nDataCycle;
				obj2.ObjectArgs.sDsn = local.ObjectArgs.sDsn;
				obj2.ObjectArgs.sTable = local.ObjectArgs.sTable;
				obj2.ObjectArgs.sColumnTime = local.ObjectArgs.sColumnTime;
				obj2.ObjectArgs.nDateColumnType = local.ObjectArgs.nDateColumnType;
				obj2.ObjectArgs.sColumnMilli = local.ObjectArgs.sColumnMilli;
				obj2.ObjectArgs.nBasicSpaceLeft = local.ObjectArgs.nBasicSpaceLeft;
				obj2.ObjectArgs.nBasicSpaceRight = local.ObjectArgs.nBasicSpaceRight;

                obj2.ObjectArgs.logarithmicScale = local.ObjectArgs.logarithmicScale;

                obj2.ObjectArgs.bUseToolBar = local.ObjectArgs.bUseToolBar; //20250306 PSU 추가
                obj2.ObjectArgs.nToolBarPos = local.ObjectArgs.nToolBarPos;
                obj2.ObjectArgs.nTooolBarBtnColor = local.ObjectArgs.nTooolBarBtnColor;
                obj2.ObjectArgs.bDontUseConfigDialog = local.ObjectArgs.bDontUseConfigDialog;
                obj2.ObjectArgs.bHideLabelDataRange = local.ObjectArgs.bHideLabelDataRange;

                obj2.ObjectArgs.nToolBarButtonSize = local.ObjectArgs.nToolBarButtonSize; //20250317 PSU 추가
                obj2.ObjectArgs.nToolBarTextSize = local.ObjectArgs.nToolBarTextSize;
				return;
			}
			if(((Form)prop).Name == "PropertyPageObjectDatabaseTrendMember") 
			{
				PropertyPageObjectDatabaseTrendMember local = (PropertyPageObjectDatabaseTrendMember)prop;
				
				obj2.GraphMember = local.GraphMember;
                obj2.ObjectArgs.pub = local.ObjectArgs.pub;
                //obj2.ObjectArgs.pub.wDisplayFlags = local.ObjectArgs.pub.wDisplayFlags;
                //obj2.ObjectArgs.pub.nLevelDisplaySize = local.ObjectArgs.pub.nLevelDisplaySize;
                //obj2.ObjectArgs.pub.wPointSize = local.ObjectArgs.pub.wPointSize;
				return;
			}
			if((string)prop.Tag == "GuideLineColor") 
			{
				PropertyPageColor local = (PropertyPageColor)prop;
				
				if(!local.IsMultiSelected())
					obj2.ObjectArgs.lColorGuideLine = local.GetSelectedColor();
				
				return;
			}
            if ((string)prop.Tag == "CursorColor")
            {
                PropertyPageColor local = (PropertyPageColor)prop;

                if (!local.IsMultiSelected())
                    obj2.ObjectArgs.lColorCursor = local.GetSelectedColor();

                return;
            }
		}

		public static void PropertyPoly(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyPoly_Recv));

			PropertyPageObjectPoly local = new PropertyPageObjectPoly();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectPoly obj2 = (ObjectPoly)obj; 

			//propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageLineColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			//local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseColorLine = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = true;

			propertySheet.Run();
		}

		private static void PropertyPoly_Recv(object obj, Form prop)
		{
			ObjectPoly obj2 = (ObjectPoly)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_LineColor(obj, prop))		return;
			if(PropertyRecv_FillColor(obj, prop))		return;

			if(((Form)prop).Name == "PropertyPageObjectPoly") 
			{
				PropertyPageObjectPoly local = (PropertyPageObjectPoly)prop;
				
				// obj2.ObjectArgs = local.ObjectArgs;
				// obj2.RecalcRectSize(formEditor.CreateGraphics());
				// formEditor.SelectListUpdate();
			}
		}


		public static void PropertyCurve(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyCurve_Recv));

			//PropertyPageObjectPoly local = new PropertyPageObjectPoly();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			ObjectCurve obj2 = (ObjectCurve)obj; 

			//propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageLineColor(obj);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			//local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseColorLine = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = true;

			propertySheet.Run();
		}

		private static void PropertyCurve_Recv(object obj, Form prop)
		{
			ObjectCurve obj2 = (ObjectCurve)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_LineColor(obj, prop))		return;
			if(PropertyRecv_FillColor(obj, prop))		return;

			/*
			if(((Form)prop).Name == "PropertyPageObjectPoly") 
			{
				PropertyPageObjectPoly local = (PropertyPageObjectPoly)prop;
				
				// obj2.ObjectArgs = local.ObjectArgs;
				// obj2.RecalcRectSize(formEditor.CreateGraphics());
				// formEditor.SelectListUpdate();
			}
			*/
		}


		public static void PropertyMultiGraph(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyMultiGraph_Recv));

			PropertyPageObjectMultiGraph local = new PropertyPageObjectMultiGraph();
			PropertyPageGraphMember member = new PropertyPageGraphMember();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색());
			ObjectMultiGraph obj2 = (ObjectMultiGraph)obj; 

			member.bUseDataType = false;	// 평균,최대,최소,순시 등을 사용하지 않는다.

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPage(member, false, multi_select);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageColor(gcolor, obj2.ObjectArgs.lColorGuideLine);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.ObjectArgs = obj2.ObjectArgs;

			member.GraphMember = obj2.GraphMember;
			member.ObjectArgs = obj2.ObjectArgs.pub;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
			expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
			//expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyMultiGraph_Recv(object obj, Form prop)
		{
			ObjectMultiGraph obj2 = (ObjectMultiGraph)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_BackColor(obj, prop))		return;
			if(PropertyRecv_FillColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectMultiGraph") 
			{
				PropertyPageObjectMultiGraph local = (PropertyPageObjectMultiGraph)prop;
				
				//obj2.ObjectArgs.wTimeSelectOption = local.ObjectArgs.wTimeSelectOption;
				obj2.ObjectArgs.wShowUnit = local.ObjectArgs.wShowUnit;
				obj2.ObjectArgs.wTimeDevide = local.ObjectArgs.wTimeDevide;
				obj2.ObjectArgs.wLevelDevide = local.ObjectArgs.wLevelDevide;
				//obj2.ObjectArgs.nDataCycle = local.ObjectArgs.nDataCycle;
				//obj2.ObjectArgs.sDsn = local.ObjectArgs.sDsn;
				//obj2.ObjectArgs.sTable = local.ObjectArgs.sTable;
				//obj2.ObjectArgs.sColumnTime = local.ObjectArgs.sColumnTime;
				//obj2.ObjectArgs.nDateColumnType = local.ObjectArgs.nDateColumnType;
				//obj2.ObjectArgs.sColumnMilli = local.ObjectArgs.sColumnMilli;
				//obj2.ObjectArgs.nBasicSpaceLeft = local.ObjectArgs.nBasicSpaceLeft;
				//obj2.ObjectArgs.nBasicSpaceRight = local.ObjectArgs.nBasicSpaceRight;
				obj2.ObjectArgs.bDisplayByTime = local.ObjectArgs.bDisplayByTime;
				obj2.ObjectArgs.bTimeDirToLeft = local.ObjectArgs.bTimeDirToLeft;
				obj2.ObjectArgs.nDataTime = local.ObjectArgs.nDataTime;

                obj2.ObjectArgs.logarithmicScale = local.ObjectArgs.logarithmicScale;
				return;
			}
			if(((Form)prop).Name == "PropertyPageGraphMember") 
			{
				PropertyPageGraphMember local = (PropertyPageGraphMember)prop;
				
				obj2.GraphMember = local.GraphMember;
                obj2.ObjectArgs.pub = local.ObjectArgs;
				//obj2.ObjectArgs.pub.wDisplayFlags = local.ObjectArgs.wDisplayFlags;
				//obj2.ObjectArgs.pub.nLevelDisplaySize = local.ObjectArgs.nLevelDisplaySize;
				//obj2.ObjectArgs.pub.wPointSize = local.ObjectArgs.wPointSize;
				return;
			}
			if((string)prop.Tag == "GuideLineColor") 
			{
				PropertyPageColor local = (PropertyPageColor)prop;
				
				if(!local.IsMultiSelected())
					obj2.ObjectArgs.lColorGuideLine = local.GetSelectedColor();
				
				return;
			}
		}


		public static void PropertyMultiTrend(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyMultiTrend_Recv));

			PropertyPageObjectMultiTrend local = new PropertyPageObjectMultiTrend();
			PropertyPageGraphMember member = new PropertyPageGraphMember();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색());
			ObjectMultiTrend obj2 = (ObjectMultiTrend)obj; 

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPage(member, false, multi_select);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageColor(gcolor, obj2.ObjectArgs.lColorGuideLine);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.ObjectArgs = obj2.ObjectArgs;

			member.GraphMember = obj2.GraphMember;
			member.ObjectArgs = obj2.ObjectArgs.pub;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
			//expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyMultiTrend_Recv(object obj, Form prop)
		{
			ObjectMultiTrend obj2 = (ObjectMultiTrend)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_BackColor(obj, prop))		return;
			if(PropertyRecv_FillColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectMultiTrend") 
			{
				PropertyPageObjectMultiTrend local = (PropertyPageObjectMultiTrend)prop;
				
				obj2.ObjectArgs.wTimeSelectOption = local.ObjectArgs.wTimeSelectOption;
				obj2.ObjectArgs.wShowUnit = local.ObjectArgs.wShowUnit;
				obj2.ObjectArgs.wTimeDevide = local.ObjectArgs.wTimeDevide;
				obj2.ObjectArgs.wLevelDevide = local.ObjectArgs.wLevelDevide;

                obj2.ObjectArgs.scriptEventAfterSettings = local.ObjectArgs.scriptEventAfterSettings;

                obj2.ObjectArgs.bUseMouseButtonAsZoom = local.ObjectArgs.bUseMouseButtonAsZoom;
                obj2.ObjectArgs.bUseMouseButtonAsZoomWithY = local.ObjectArgs.bUseMouseButtonAsZoomWithY;
                obj2.ObjectArgs.bDontUseConfigDialog = local.ObjectArgs.bDontUseConfigDialog;
                obj2.ObjectArgs.nDataCycle = local.ObjectArgs.nDataCycle;
                obj2.ObjectArgs.bDontUseConfigAutoRange = local.ObjectArgs.bDontUseConfigAutoRange;
				//obj2.ObjectArgs.nDataCycle = local.ObjectArgs.nDataCycle;
				//obj2.ObjectArgs.sDsn = local.ObjectArgs.sDsn;
				//obj2.ObjectArgs.sTable = local.ObjectArgs.sTable;
				//obj2.ObjectArgs.sColumnTime = local.ObjectArgs.sColumnTime;
				//obj2.ObjectArgs.nDateColumnType = local.ObjectArgs.nDateColumnType;
				//obj2.ObjectArgs.sColumnMilli = local.ObjectArgs.sColumnMilli;
				//obj2.ObjectArgs.nBasicSpaceLeft = local.ObjectArgs.nBasicSpaceLeft;
				//obj2.ObjectArgs.nBasicSpaceRight = local.ObjectArgs.nBasicSpaceRight;

				//obj2.ObjectArgs.bDisplayByTime = local.ObjectArgs.bDisplayByTime;
				//obj2.ObjectArgs.bTimeDirToLeft = local.ObjectArgs.bTimeDirToLeft;
				//obj2.ObjectArgs.nDataTime = local.ObjectArgs.nDataTime;
                obj2.ObjectArgs.logarithmicScale = local.ObjectArgs.logarithmicScale;

                obj2.ObjectArgs.bUseToolBar = local.ObjectArgs.bUseToolBar; //20250306 PSU 추가
                obj2.ObjectArgs.nToolBarPos = local.ObjectArgs.nToolBarPos;
                obj2.ObjectArgs.nTooolBarBtnColor = local.ObjectArgs.nTooolBarBtnColor;
                obj2.ObjectArgs.bHideLabelDataRange = local.ObjectArgs.bHideLabelDataRange;

                obj2.ObjectArgs.nToolBarButtonSize = local.ObjectArgs.nToolBarButtonSize; //20250317 PSU 추가
                obj2.ObjectArgs.nToolBarTextSize = local.ObjectArgs.nToolBarTextSize;
				return;
			}
			if(((Form)prop).Name == "PropertyPageGraphMember") 
			{
				PropertyPageGraphMember local = (PropertyPageGraphMember)prop;
				
				obj2.GraphMember = local.GraphMember;

                obj2.ObjectArgs.pub = local.ObjectArgs;

				//obj2.ObjectArgs.pub.wDisplayFlags = local.ObjectArgs.wDisplayFlags;
				//obj2.ObjectArgs.pub.nLevelDisplaySize = local.ObjectArgs.nLevelDisplaySize;
				//obj2.ObjectArgs.pub.wPointSize = local.ObjectArgs.wPointSize;
				return;
			}
			if((string)prop.Tag == "GuideLineColor") 
			{
				PropertyPageColor local = (PropertyPageColor)prop;
				
				if(!local.IsMultiSelected())
					obj2.ObjectArgs.lColorGuideLine = local.GetSelectedColor();
				
				return;
			}
		}


		//=== Chart property pages (25-02-24) ===

		public static void PropertyChartCustom(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyChartCustom_Recv));
			PropertyPageObjectChart local = new PropertyPageObjectChart(3);
			PropertyPageGraphMember member = new PropertyPageGraphMember();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색());
			CustomChart obj2 = (CustomChart)obj;
			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPage(member, false, multi_select);
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageColor(gcolor, obj2.objArgs.lColorGuideLine);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);
			local.ArgsCustom = obj2.objArgs;
			member.GraphMember = obj2.ChartMemberAsAnalog;
			member.ObjectArgs = obj2.objArgs.pub;
			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseColorFill = true;
			propertySheet.Run();
		}

		private static void PropertyChartCustom_Recv(object obj, Form prop)
		{
			CustomChart obj2 = (CustomChart)obj;
			if(PropertyRecv_ClassName(obj, prop)) return;
			if(PropertyRecv_ExpandOption(obj, prop)) return;
			if(PropertyRecv_TextColor(obj, prop))
			{
				obj2.ApplyColorToChart();
				return;
			}
			if(PropertyRecv_BackColor(obj, prop))
			{
				obj2.ApplyColorToChart();
				return;
			}
			if(PropertyRecv_FillColor(obj, prop))
			{
				obj2.ApplyColorToChart();
				return;
			}
			if(PropertyRecv_Font(obj, prop)) return;
			if((string)prop.Tag == "GuideLineColor")
			{
				PropertyPageColor local = (PropertyPageColor)prop;
				if(!local.IsMultiSelected())
					obj2.objArgs.lColorGuideLine = local.GetSelectedColor();
				obj2.ApplyColorToChart();
				return;
			}
			if(((Form)prop).Name == "PropertyPageObjectChart")
			{
				PropertyPageObjectChart local = (PropertyPageObjectChart)prop;
				obj2.ObjectArgs = local.ArgsCustom;
				return;
			}
			if(((Form)prop).Name == "PropertyPageGraphMember")
			{
				PropertyPageGraphMember local = (PropertyPageGraphMember)prop;
				obj2.ChartMember = local.GraphMember;
				obj2.objArgs.pub = local.ObjectArgs;
				return;
			}
		}



		public static void PropertyRealTimeTestGraph(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyRealTimeTestGraph_Recv));

			PropertyPageObjectRealTimeTestGraph local = new PropertyPageObjectRealTimeTestGraph();
			PropertyPageGraphMember member = new PropertyPageGraphMember();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색());

			ObjectRealTimeTestGraph obj2 = (ObjectRealTimeTestGraph)obj;

            member.bUseTimeShift = true;

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPage(member, false, multi_select);
            propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageColor(gcolor, obj2.ObjectArgs.lColorGuideLine);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.GraphMember = obj2.GraphTwoTagMemberList;
			local.ObjectArgs = obj2.ObjectArgs;
			
			member.GraphMember = obj2.GraphMember;
			member.ObjectArgs = obj2.ObjectArgs.pub;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
			//expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyRealTimeTestGraph_Recv(object obj, Form prop)
		{
			ObjectRealTimeTestGraph obj2 = (ObjectRealTimeTestGraph)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_BackColor(obj, prop))		return;
			if(PropertyRecv_FillColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectRealTimeTestGraph") 
			{
				PropertyPageObjectRealTimeTestGraph local = (PropertyPageObjectRealTimeTestGraph)prop;
				
				//obj2.ObjectArgs.wTimeSelectOption = local.ObjectArgs.wTimeSelectOption;
				obj2.ObjectArgs.wShowUnit = local.ObjectArgs.wShowUnit;
				obj2.ObjectArgs.wTimeDevide = local.ObjectArgs.wTimeDevide;
				obj2.ObjectArgs.wLevelDevide = local.ObjectArgs.wLevelDevide;
				obj2.ObjectArgs.nDataTime = local.ObjectArgs.nDataTime;
				obj2.ObjectArgs.sTagStart = local.ObjectArgs.sTagStart;
				obj2.ObjectArgs.bDisableCursor = local.ObjectArgs.bDisableCursor;
				obj2.ObjectArgs.nTimeDisplayType = local.ObjectArgs.nTimeDisplayType;
				obj2.ObjectArgs.sTagRun = local.ObjectArgs.sTagRun;
				obj2.GraphTwoTagMemberList = local.GraphMember;
                obj2.ObjectArgs.bUseSavedData = local.ObjectArgs.bUseSavedData;
                obj2.ObjectArgs.sSavedData = local.ObjectArgs.sSavedData;
                obj2.ObjectArgs.bGraphDisplayWhileRunning = local.ObjectArgs.bGraphDisplayWhileRunning;
				
				return;
			}
			if(((Form)prop).Name == "PropertyPageGraphMember") 
			{
				PropertyPageGraphMember local = (PropertyPageGraphMember)prop;
				
				obj2.GraphMember = local.GraphMember;

                obj2.ObjectArgs.pub = local.ObjectArgs;
				//obj2.ObjectArgs.pub.wDisplayFlags = local.ObjectArgs.wDisplayFlags;
				//obj2.ObjectArgs.pub.nLevelDisplaySize = local.ObjectArgs.nLevelDisplaySize;
				//obj2.ObjectArgs.pub.wPointSize = local.ObjectArgs.wPointSize;
				return;
			}
			if((string)prop.Tag == "GuideLineColor") 
			{
				PropertyPageColor local = (PropertyPageColor)prop;
				
				if(!local.IsMultiSelected())
					obj2.ObjectArgs.lColorGuideLine = local.GetSelectedColor();
				
				return;
			}
		}

		public static void PropertyXYGraph(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyXYGraph_Recv));

			PropertyPageObjectXYGraph local = new PropertyPageObjectXYGraph();
			PropertyPageXYGraphMember member = new PropertyPageXYGraphMember();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
			PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색());
			ObjectXYGraph obj2 = (ObjectXYGraph)obj; 

			propertySheet.AddPage(local, false, multi_select);
			propertySheet.AddPage(member, false, multi_select);
            propertySheet.AddPageFont(obj);
			//propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageFillColor(obj);
			propertySheet.AddPageColor(gcolor, obj2.ObjectArgs.gcolor);
			
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			local.ObjectArgs = obj2.ObjectArgs;

			member.GraphMember = obj2.GraphMember;
			member.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;
			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
			expand.bUseColorText = false;
			expand.bUseColorBack = true;
			expand.bUseColorFill = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyXYGraph_Recv(object obj, Form prop)
		{
			ObjectXYGraph obj2 = (ObjectXYGraph)obj;

			if(PropertyRecv_ClassName(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))	return;
			if(PropertyRecv_TextColor(obj, prop))		return;
			if(PropertyRecv_BackColor(obj, prop))		return;
			if(PropertyRecv_FillColor(obj, prop))		return;
			if(PropertyRecv_Font(obj, prop))			return;

			if(((Form)prop).Name == "PropertyPageObjectXYGraph") 
			{
				PropertyPageObjectXYGraph local = (PropertyPageObjectXYGraph)prop;
				
				//obj2.ObjectArgs..wTimeSelectOption = local.ObjectArgs.wTimeSelectOption;
				obj2.ObjectArgs.showunit = local.ObjectArgs.showunit;
				obj2.ObjectArgs.wTimeDevide = local.ObjectArgs.wTimeDevide;
				obj2.ObjectArgs.leveldevide = local.ObjectArgs.leveldevide;
				obj2.ObjectArgs.nDataTime = local.ObjectArgs.nDataTime;
				return;
			}
			if(((Form)prop).Name == "PropertyPageXYGraphMember") 
			{
				PropertyPageXYGraphMember local = (PropertyPageXYGraphMember)prop;
				
				obj2.GraphMember = local.GraphMember;
				obj2.ObjectArgs.wDisplayFlags = local.ObjectArgs.wDisplayFlags;
				//obj2.ObjectArgs.n.pub.nLevelDisplaySize = local.ObjectArgs.nLevelDisplaySize;
				obj2.ObjectArgs.point_size = local.ObjectArgs.point_size;
				return;
			}
			if((string)prop.Tag == "GuideLineColor")
			{
				PropertyPageColor local = (PropertyPageColor)prop;
				
				if(!local.IsMultiSelected())
					obj2.ObjectArgs.gcolor = local.GetSelectedColor();
				
				return;
			}
		}

		public static void PropertyDatabase(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyDatabase_Recv));

			PropertyPageObjectDatabase local = new PropertyPageObjectDatabase();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			propertySheet.AddPage(local, false, multi_select); 
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectDatabase obj2 = (ObjectDatabase)obj;

			local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = true;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyDatabase_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_TextColor(obj, prop))	return;
			if(PropertyRecv_BackColor(obj, prop))	return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectDatabase") 
			{
				PropertyPageObjectDatabase local = (PropertyPageObjectDatabase)prop;
				ObjectDatabase obj2 = (ObjectDatabase)obj;

				obj2.ObjectArgs.bAutoUpdate = local.ObjectArgs.bAutoUpdate;
				obj2.ObjectArgs.bUseGrid = local.ObjectArgs.bUseGrid;
				obj2.ObjectArgs.bUseFullCursor = local.ObjectArgs.bUseFullCursor;
				obj2.ObjectArgs.bUseNo = local.ObjectArgs.bUseNo;
				obj2.ObjectArgs.dsn = local.ObjectArgs.dsn;
				obj2.ObjectArgs.nUpdateTime = local.ObjectArgs.nUpdateTime;
				obj2.ObjectArgs.table = local.ObjectArgs.table;
				obj2.ObjectArgs.nConnectionType = local.ObjectArgs.nConnectionType;
				obj2.ObjectArgs.filename = local.ObjectArgs.filename;
                obj2.ObjectArgs.bReverseNo = local.ObjectArgs.bReverseNo;
				obj2.ObjectArgs.sSqlTextOrderBy = local.ObjectArgs.sSqlTextOrderBy; //20260226 PSU add
				obj2.ObjectArgs.sSqlTextWhere = local.ObjectArgs.sSqlTextWhere;
				obj2.ObjectArgs.nRecordLimit = local.ObjectArgs.nRecordLimit;
				obj2.ObjectArgs.bUseAlternateRowColor = local.ObjectArgs.bUseAlternateRowColor;
				obj2.ObjectArgs.bUsePagination = local.ObjectArgs.bUsePagination;
				obj2.ObjectArgs.nPageSize = local.ObjectArgs.nPageSize;


            }
		}

        public static void PropertyWebBrowser(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyWebBrowser_Recv));

            PropertyPageObjectWebBrowser local = new PropertyPageObjectWebBrowser();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

            // multiselect를 사용하므로 반환된 값으로 local을 바꾸어야 한다.
            local = (PropertyPageObjectWebBrowser)propertySheet.AddPage(local, true, multi_select);
            //propertySheet.AddPageFont(obj);
            //propertySheet.AddPageTextColor(obj);
            //propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            ObjectWebBrowser obj2 = (ObjectWebBrowser)obj;

            local.Set(obj2.ObjectArgs);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            //expand.bUseMouseDown = false;
            //expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyWebBrowser_Recv(object obj, Form prop)
        {
            if (PropertyRecv_ClassName(obj, prop)) return;
            //if (PropertyRecv_Font(obj, prop)) return;
            //if (PropertyRecv_TextColor(obj, prop)) return;
            //if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectWebBrowser")
            {
                PropertyPageObjectWebBrowser local = (PropertyPageObjectWebBrowser)prop;
                ObjectWebBrowser obj2 = (ObjectWebBrowser)obj;

                local.Get(obj2.ObjectArgs);
            }
        }

        public static void PropertyWebView(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyWebView_Recv));

            PropertyPageObjectWebView local = new PropertyPageObjectWebView();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

            // multiselect를 사용하므로 반환된 값으로 local을 바꾸어야 한다.
            local = (PropertyPageObjectWebView)propertySheet.AddPage(local, true, multi_select);
            //propertySheet.AddPageFont(obj);
            //propertySheet.AddPageTextColor(obj);
            //propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            ObjectWebView obj2 = (ObjectWebView)obj;

            local.Set(obj2.ObjectArgs);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            //expand.bUseMouseDown = false;
            //expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyWebView_Recv(object obj, Form prop)
        {
            if (PropertyRecv_ClassName(obj, prop)) return;
            //if (PropertyRecv_Font(obj, prop)) return;
            //if (PropertyRecv_TextColor(obj, prop)) return;
            //if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectWebView")
            {
                PropertyPageObjectWebView local = (PropertyPageObjectWebView)prop;
                ObjectWebView obj2 = (ObjectWebView)obj;

                local.Get(obj2.ObjectArgs);
            }
        }

        public static void PropertyVLCAx(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyVLCAx_Recv));

            PropertyPageObjectVLCAx local = new PropertyPageObjectVLCAx();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

            // multiselect를 사용하므로 반환된 값으로 local을 바꾸어야 한다.
            local = (PropertyPageObjectVLCAx)propertySheet.AddPage(local, true, multi_select);
            //propertySheet.AddPageFont(obj);
            //propertySheet.AddPageTextColor(obj);
            //propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            ObjectVLCAx obj2 = (ObjectVLCAx)obj;

            local.Set(obj2.ObjectArgs);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            //expand.bUseMouseDown = false;
            //expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyVLCAx_Recv(object obj, Form prop)
        {
            if (PropertyRecv_ClassName(obj, prop)) return;
            //if (PropertyRecv_Font(obj, prop)) return;
            //if (PropertyRecv_TextColor(obj, prop)) return;
            //if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectVLCAx")
            {
                PropertyPageObjectVLCAx local = (PropertyPageObjectVLCAx)prop;
                ObjectVLCAx obj2 = (ObjectVLCAx)obj;

                local.Get(obj2.ObjectArgs);
            }
        }

        public static void PropertySVG(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertySvg_Recv));

            PropertyPageObjectSVG local = new PropertyPageObjectSVG();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            ObjectSVG obj2 = (ObjectSVG)obj;

            local = (PropertyPageObjectSVG)propertySheet.AddPage(local, true, multi_select);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            local.SetObjectArgs(obj2.ObjectArgs);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = true;
            expand.bUseSliderVert = true;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertySvg_Recv(object obj, Form prop)
        {
            ObjectSVG obj2 = (ObjectSVG)obj;

            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectSVG")
            {
                PropertyPageObjectSVG local = (PropertyPageObjectSVG)prop;

                obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
                if (local.checkBoxRestoreToOrizinalSize.Checked)
                {
                    obj2.SetOriginalSize();
                    formEditor.SelectListUpdate();
                }

                obj2.bSetOriginalSizeOnStudio = local.checkBoxRestoreToOrizinalSize.Checked;
            }
        }



		public static void PropertyWindowAlarm(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyWindowAlarm_Recv));

			PropertyPageObjectWindowAlarm local = new PropertyPageObjectWindowAlarm();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			propertySheet.AddPage(local, false, multi_select); 
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectWindowAlarm obj2 = (ObjectWindowAlarm)obj;

			local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyWindowAlarm_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectWindowAlarm") 
			{
				PropertyPageObjectWindowAlarm local = (PropertyPageObjectWindowAlarm)prop;
				ObjectWindowAlarm obj2 = (ObjectWindowAlarm)obj;

				obj2.ObjectArgs.bFlagUseColumnHeader = local.ObjectArgs.bFlagUseColumnHeader;
				obj2.ObjectArgs.bFlagWindowCaption = local.ObjectArgs.bFlagWindowCaption;
				obj2.ObjectArgs.cIncludeMethod = local.ObjectArgs.cIncludeMethod;
                obj2.ObjectArgs.bFlagUseScrollHorz = local.ObjectArgs.bFlagUseScrollHorz;
                obj2.ObjectArgs.bFlagUseScrollVert = local.ObjectArgs.bFlagUseScrollVert;
                obj2.ObjectArgs.arrayColumns = local.ObjectArgs.arrayColumns;
			}
		}

		public static void PropertyDemandWindow(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyDemandWindow_Recv));

			PropertyPageObjectDemandWindow local = new PropertyPageObjectDemandWindow();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색()); //20250227 PSU 추가

            ObjectDemandWindow obj2 = (ObjectDemandWindow)obj;

			propertySheet.AddPage(local, false, multi_select); 
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageFillColor(obj);   //20250227 PSU 추가
            propertySheet.AddPageColor(gcolor, obj2.ObjectArgs.lColorGuideLine);  //20250227 PSU 추가
			propertySheet.AddPageExpand(expand, obj, multi_select);


			local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
            expand.bUseColorFill = true;  //20250227 PSU 추가
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyDemandWindow_Recv(object obj, Form prop)
		{
            ObjectDemandWindow obj2 = (ObjectDemandWindow)obj;

			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_TextColor(obj, prop))	return;
			if(PropertyRecv_BackColor(obj, prop))	return;
            if (PropertyRecv_FillColor(obj, prop)) return; //20250227 PSU 추가
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectDemandWindow") 
			{
				PropertyPageObjectDemandWindow local = (PropertyPageObjectDemandWindow)prop;
				

                //obj2.ObjectArgs.bFlagWindowCaption = local.ObjectArgs.bFlagWindowCaption;
                //obj2.ObjectArgs.cIncludeMethod = local.ObjectArgs.cIncludeMethod;
                //obj2.ObjectArgs.demand_name = local.ObjectArgs.demand_name;
                //obj2.ObjectArgs.thick_target = local.ObjectArgs.thick_target;
                obj2.ObjectArgs = local.ObjectArgs; //20250227 PSU 수정
			}
            if ((string)prop.Tag == "GuideLineColor")  //20250227 PSU 추가
            {
                PropertyPageColor local = (PropertyPageColor)prop;

                if (!local.IsMultiSelected())
                    obj2.ObjectArgs.lColorGuideLine = local.GetSelectedColor();
                return;
            }
		}

        public static void PropertyDemandChart(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyDemandChart_Recv));

            PropertyPageObjectDemandChart local = new PropertyPageObjectDemandChart();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색());

            ObjectDemandChart obj2 = (ObjectDemandChart)obj;

            local.Text = "DemandChart";
            propertySheet.AddPage(local, false, multi_select);
            propertySheet.AddPageFont(obj);
            propertySheet.AddPageTextColor(obj);
            propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageFillColor(obj);
            propertySheet.AddPageColor(gcolor, obj2.ObjectArgs != null ? obj2.ObjectArgs.lColorGuideLine : System.Drawing.Color.LightGray);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            local.ObjectArgs = obj2.ObjectArgs;

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = true;
            expand.bUseColorBack = true;
            expand.bUseColorFill = true;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyDemandChart_Recv(object obj, Form prop)
        {
            ObjectDemandChart obj2 = (ObjectDemandChart)obj;

            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_Font(obj, prop)) return;
            if (PropertyRecv_TextColor(obj, prop)) return;
            if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_FillColor(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectDemandChart")
            {
                PropertyPageObjectDemandChart local = (PropertyPageObjectDemandChart)prop;
                obj2.ObjectArgs = local.ObjectArgs;
            }
            if ((string)prop.Tag == "GuideLineColor")
            {
                PropertyPageColor local = (PropertyPageColor)prop;
                if (!local.IsMultiSelected())
                    obj2.ObjectArgs.lColorGuideLine = local.GetSelectedColor();
                return;
            }
        }

        public static void PropertyMilliData(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyMilliData_Recv));

			PropertyPageObjectMilliData local = new PropertyPageObjectMilliData();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			propertySheet.AddPage(local, false, multi_select); 
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
			propertySheet.AddPageClassName(obj, multi_select);
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectMilliData obj2 = (ObjectMilliData)obj;

			local.ObjectArgs = obj2.ObjectArgs;

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
			//expand.bUseMouseDown = false;
			//expand.bUseMouseUp = false;
			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = false;
			expand.bUseSliderVert = false;
			expand.bUseColorText = true;
			expand.bUseColorBack = true;
			expand.bUseThickLine = false;

			propertySheet.Run();
		}

		private static void PropertyMilliData_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			if(PropertyRecv_Font(obj, prop))		return;
			if(PropertyRecv_TextColor(obj, prop))	return;
			if(PropertyRecv_BackColor(obj, prop))	return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectMilliData")
			{
				PropertyPageObjectMilliData local = (PropertyPageObjectMilliData)prop;
				ObjectMilliData obj2 = (ObjectMilliData)obj;

				obj2.ObjectArgs.sTitle = local.ObjectArgs.sTitle;
			}
		}

        public static void PropertyMilliDataTrend(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyMilliDataTrend_Recv));

            PropertyPageObjectMilliDataTrend local = new PropertyPageObjectMilliDataTrend();
            PropertyPageObjectMilliDataTrendMember member = new PropertyPageObjectMilliDataTrendMember();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            PropertyPageColor gcolor = new PropertyPageColor("GuideLineColor", String눈금색());
            ObjectMilliDataTrend obj2 = (ObjectMilliDataTrend)obj;

            propertySheet.AddPage(local, false, multi_select);
            propertySheet.AddPage(member, false, multi_select);
            propertySheet.AddPageTextColor(obj);
            propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageFillColor(obj);
            propertySheet.AddPageColor(gcolor, obj2.ObjectArgs.lColorGuideLine);
            propertySheet.AddPageFont(obj);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            local.ObjectArgs = obj2.ObjectArgs;

            member.GraphMember = obj2.GraphMember;
            member.ObjectArgs = obj2.ObjectArgs;

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            //expand.bUseMouseLeftDown = true;
            //expand.bUseMouseLeftUp = true;
            //expand.bUseMouseRightDown = true;
            //expand.bUseMouseRightUp = true;
            //expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = true;
            expand.bUseColorBack = true;
            expand.bUseColorFill = true;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyMilliDataTrend_Recv(object obj, Form prop)
        {
            ObjectMilliDataTrend obj2 = (ObjectMilliDataTrend)obj;

            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;
            if (PropertyRecv_TextColor(obj, prop)) return;
            if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_FillColor(obj, prop)) return;
            if (PropertyRecv_Font(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectMilliDataTrend")
            {
                PropertyPageObjectMilliDataTrend local = (PropertyPageObjectMilliDataTrend)prop;

                obj2.ObjectArgs.wTimeSelectOption = local.ObjectArgs.wTimeSelectOption;
                obj2.ObjectArgs.wShowUnit = local.ObjectArgs.wShowUnit;
                obj2.ObjectArgs.wTimeDevide = local.ObjectArgs.wTimeDevide;
                obj2.ObjectArgs.wLevelDevide = local.ObjectArgs.wLevelDevide;
                obj2.ObjectArgs.nDataCycle = local.ObjectArgs.nDataCycle;
                obj2.ObjectArgs.sDsn = local.ObjectArgs.sDsn;
                //obj2.ObjectArgs.sTable = local.ObjectArgs.sTable;
                //obj2.ObjectArgs.sColumnTime = local.ObjectArgs.sColumnTime;
                //obj2.ObjectArgs.nDateColumnType = local.ObjectArgs.nDateColumnType;
                //obj2.ObjectArgs.sColumnMilli = local.ObjectArgs.sColumnMilli;
                obj2.ObjectArgs.nBasicSpaceLeft = local.ObjectArgs.nBasicSpaceLeft;
                obj2.ObjectArgs.nBasicSpaceRight = local.ObjectArgs.nBasicSpaceRight;
                obj2.ObjectArgs.bAutoUpdate = local.ObjectArgs.bAutoUpdate;

                obj2.ObjectArgs.logarithmicScale = local.ObjectArgs.logarithmicScale;

                obj2.ObjectArgs.bDontUseConfigDialog = local.ObjectArgs.bDontUseConfigDialog;
                obj2.ObjectArgs.bUseToolBar = local.ObjectArgs.bUseToolBar;    //20250306 PSU 추가.
                obj2.ObjectArgs.nToolBarPos = local.ObjectArgs.nToolBarPos;
                obj2.ObjectArgs.nTooolBarBtnColor = local.ObjectArgs.nTooolBarBtnColor;
                obj2.ObjectArgs.bHideLabelDataRange = local.ObjectArgs.bHideLabelDataRange;
                obj2.ObjectArgs.nToolBarButtonSize = local.ObjectArgs.nToolBarButtonSize; //20250317 PSU 추가
                obj2.ObjectArgs.nToolBarTextSize = local.ObjectArgs.nToolBarTextSize;
                return;
            }
            if (prop.GetType() == typeof(PropertyPageObjectMilliDataTrendMember))
            {
                PropertyPageObjectMilliDataTrendMember local = (PropertyPageObjectMilliDataTrendMember)prop;

                obj2.GraphMember = local.GraphMember;

                obj2.ObjectArgs.pub = local.ObjectArgs.pub;
                //obj2.ObjectArgs.pub.wDisplayFlags = local.ObjectArgs.pub.wDisplayFlags;
                //obj2.ObjectArgs.pub.nLevelDisplaySize = local.ObjectArgs.pub.nLevelDisplaySize;
                //obj2.ObjectArgs.pub.wPointSize = local.ObjectArgs.pub.wPointSize;
                return;
            }
            if ((string)prop.Tag == "GuideLineColor")
            {
                PropertyPageColor local = (PropertyPageColor)prop;

                if (!local.IsMultiSelected())
                    obj2.ObjectArgs.lColorGuideLine = local.GetSelectedColor();

                return;
            }
        }

		public static void PropertyGroup(object obj, bool multi_select)
		{
			propertySheet.AddObject(obj, new Property_Recv(PropertyGroup_Recv));

			PropertyPageObjectGroup local = new PropertyPageObjectGroup();
			//PropertyPageObjectGroupDisplayTest test = new PropertyPageObjectGroupDisplayTest();
			PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

			propertySheet.AddPage(local, false, multi_select); 
			//propertySheet.AddPage(test, false, multi_select); 

			/*
			propertySheet.AddPageFont(obj);
			propertySheet.AddPageTextColor(obj);
			propertySheet.AddPageBackColor(obj);
             */ 
			propertySheet.AddPageClassName(obj, multi_select);
			
			propertySheet.AddPageExpand(expand, obj, multi_select);

			ObjectGroup obj2 = (ObjectGroup)obj;

			local.ObjectToList(obj2);

			//test.SetGroup(obj2);

			expand.bUseSizeWidth = true;
			expand.bUseSizeHeight = true;
			expand.bUseLocationX = true;
			expand.bUseLocationY = true;
			expand.bUseEventKeyDown = false;
			expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;
            expand.bUseMouseLeftUp = true;
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

			expand.bUseZoneDisplay = true;
			expand.bUseVisible = true;
			expand.bUseBlinking = true;
			expand.bUseAnimationSpeed = false;
			expand.bUseSliderHorz = true;
			expand.bUseSliderVert = true;
			expand.bUseColorText = false;
			expand.bUseColorBack = false;
			expand.bUseThickLine = false;

            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                expand.bUseRotation = false;
            }

			propertySheet.Run();
		}

		private static void PropertyGroup_Recv(object obj, Form prop)
		{
			if(PropertyRecv_ClassName(obj, prop))	return;
			//if(PropertyRecv_Font(obj, prop))		return;
			//if(PropertyRecv_TextColor(obj, prop))	return;
			//if(PropertyRecv_BackColor(obj, prop))	return;
			if(PropertyRecv_ExpandOption(obj, prop))return;

			if(((Form)prop).Name == "PropertyPageObjectGroup") 
			{
				PropertyPageObjectGroup local = (PropertyPageObjectGroup)prop;
				ObjectGroup obj2 = (ObjectGroup)obj;

				local.ListToObject(obj2);

				if(local.checkBoxRestoreToOriginalSize.Checked)	 
				{
					int width=0, height=0;
					RECT rect = new RECT();
					obj2.GetGroupRealSize(ref width, ref height);
					obj2.GetZone(ref rect);
					rect.right = rect.left+width-1;
					rect.bottom = rect.top+height-1;
					obj2.UpdateZone(formEditor, rect.left, rect.top, rect.right, rect.bottom);
					obj2.SetZoneAtPercent100(obj2.sizeGroup, rect);

					formEditor.SelectListUpdate();
				}

                obj2.bSetOriginalSizeOnStudio = local.checkBoxRestoreToOriginalSize.Checked;

				/*
				obj2.ObjectArgs.bAutoUpdate = local.ObjectArgs.bAutoUpdate;
				obj2.ObjectArgs.bUseGrid = local.ObjectArgs.bUseGrid;
				obj2.ObjectArgs.bUseFullCursor = local.ObjectArgs.bUseFullCursor;
				obj2.ObjectArgs.bUseNo = local.ObjectArgs.bUseNo;
				obj2.ObjectArgs.dsn = local.ObjectArgs.dsn;
				obj2.ObjectArgs.nUpdateTime = local.ObjectArgs.nUpdateTime;
				obj2.ObjectArgs.table = local.ObjectArgs.table;
				obj2.ObjectArgs.nConnectionType = local.ObjectArgs.nConnectionType;
				obj2.ObjectArgs.filename = local.ObjectArgs.filename;
				*/
			}
		}

        public static void PropertyTagAnimation(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyTagAnimation_Recv));

            PropertyPageObjectTagAnimation local = new PropertyPageObjectTagAnimation();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            ObjectTagAnimation obj2 = (ObjectTagAnimation)obj;

            local = (PropertyPageObjectTagAnimation)propertySheet.AddPage(local, true, multi_select);
            propertySheet.AddPageTagLocalAll(obj, obj2.sTagName);
            propertySheet.AddPageMouseResponse(obj, multi_select);
            propertySheet.AddPageControlBox(obj);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            local.SetObjectArgs(obj2.objArgs, multi_select);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyTagAnimation_Recv(object obj, Form prop)
        {
            ObjectTagAnimation obj2 = (ObjectTagAnimation)obj;

            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_TagLocal(obj, prop, ref obj2.sTagName)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;
            if (PropertyRecv_MouseResponse(obj, prop)) return;
            if (PropertyRecv_ControlBox(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectTagAnimation")
            {
                PropertyPageObjectTagAnimation local = (PropertyPageObjectTagAnimation)prop;

                obj2.SetObjectArgs(local.GetObjectArgs(obj2.objArgs));

                if (local.checkBoxRestore.Checked)
                {
                    obj2.SetOriginalSize();
                    formEditor.SelectListUpdate();
                }

                obj2.bSetOriginalSizeOnStudio = local.checkBoxRestore.Checked;
            }
        }

        public static void PropertyDataGridView(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyDataGridView_Recv));

            PropertyPageObjectDataGridView local = new PropertyPageObjectDataGridView();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

            propertySheet.AddPage(local, false, multi_select);
            propertySheet.AddPageFont(obj);
            propertySheet.AddPageTextColor(obj);
            propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            ObjectDataGridView obj2 = (ObjectDataGridView)obj;

            local.SetObjectArgs(obj2.ObjectArgs);

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = true;
            //expand.bUseMouseDown = false;
            //expand.bUseMouseUp = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = true;
            expand.bUseColorBack = true;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyDataGridView_Recv(object obj, Form prop)
        {
            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_Font(obj, prop)) return;
            if (PropertyRecv_TextColor(obj, prop)) return;
            if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectDataGridView")
            {
                PropertyPageObjectDataGridView local = (PropertyPageObjectDataGridView)prop;
                ObjectDataGridView obj2 = (ObjectDataGridView)obj;

                obj2.ObjectArgs = local.GetObjectArgs(obj2.ObjectArgs);
            }
        }


        // 도넛차트 추가 hsjeong 25-02-04
        public static void PropertyDonutChart(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyDonutChart_Recv));

            PropertyPageObjectDonutChart local = new PropertyPageObjectDonutChart();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);
            //PropertyPageColor gcolor = new PropertyPageColor("MeterGuideLine", "GuideColor");

            ObjectDonutChart obj2 = (ObjectDonutChart)obj;

            local = (PropertyPageObjectDonutChart)propertySheet.AddPage(local, false, multi_select);

            propertySheet.AddPageFont(obj);



            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);


            local.ObjectArgs = obj2.ObjectArgs;
            local.ChartMember = obj2.ChartMember;

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            expand.bUseMouseLeftDown = true;            // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseLeftUp = true;              // 10.3.2.3 부터 확장스크립트 왼쪽마우스 클릭 지원
            expand.bUseMouseRightDown = true;
            expand.bUseMouseRightUp = true;

            expand.bUseMouseEnter = true;
            expand.bUseMouseLeave = true;
            expand.bUseMouseMove = true;

            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = true;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorLine = false;
            expand.bUseColorFill = false;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();

        }

        // 도넛차트 추가 hsjeong 25-02-04
        private static void PropertyDonutChart_Recv(object obj, Form prop)
        {
            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_ControlBox(obj, prop)) return;
            if (PropertyRecv_Font(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectDonutChart")
            {
                PropertyPageObjectDonutChart local = (PropertyPageObjectDonutChart)prop;
                ObjectDonutChart obj2 = (ObjectDonutChart)obj;

                obj2.ChartMember = local.ChartMember;

                obj2.ObjectArgs = local.ObjectArgs;
                return;
            }
        }

        #region BarcodeDisplay / BarcodeScanner Property

        public static void PropertyBarcodeDisplay(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyBarcodeDisplay_Recv));

            PropertyPageObjectBarcodeDisplay local = new PropertyPageObjectBarcodeDisplay();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

            local = (PropertyPageObjectBarcodeDisplay)propertySheet.AddPage(local, true, multi_select);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            ObjectBarcodeDisplay obj2 = (ObjectBarcodeDisplay)obj;
            local.ObjectArgs = obj2.ObjectArgs;

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = false;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = false;
            expand.bUseColorBack = false;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyBarcodeDisplay_Recv(object obj, Form prop)
        {
            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectBarcodeDisplay")
            {
                PropertyPageObjectBarcodeDisplay local = (PropertyPageObjectBarcodeDisplay)prop;
                ObjectBarcodeDisplay obj2 = (ObjectBarcodeDisplay)obj;
                obj2.ObjectArgs = local.ObjectArgs;
            }
        }

        public static void PropertyBarcodeScanner(object obj, bool multi_select)
        {
            propertySheet.AddObject(obj, new Property_Recv(PropertyBarcodeScanner_Recv));

            PropertyPageObjectBarcodeScanner local = new PropertyPageObjectBarcodeScanner();
            PropertyPageExpandOption expand = new PropertyPageExpandOption((ObjectExpand)obj);

            local = (PropertyPageObjectBarcodeScanner)propertySheet.AddPage(local, true, multi_select);
            propertySheet.AddPageTextColor(obj);
            propertySheet.AddPageBackColor(obj);
            propertySheet.AddPageClassName(obj, multi_select);
            propertySheet.AddPageExpand(expand, obj, multi_select);

            ObjectBarcodeScanner obj2 = (ObjectBarcodeScanner)obj;
            local.ObjectArgs = obj2.ObjectArgs;

            expand.bUseSizeWidth = true;
            expand.bUseSizeHeight = true;
            expand.bUseLocationX = true;
            expand.bUseLocationY = true;
            expand.bUseEventKeyDown = false;
            expand.bUseEventSelChange = false;
            expand.bUseZoneDisplay = true;
            expand.bUseVisible = true;
            expand.bUseBlinking = false;
            expand.bUseAnimationSpeed = false;
            expand.bUseSliderHorz = false;
            expand.bUseSliderVert = false;
            expand.bUseColorText = true;
            expand.bUseColorBack = true;
            expand.bUseThickLine = false;

            propertySheet.Run();
        }

        private static void PropertyBarcodeScanner_Recv(object obj, Form prop)
        {
            if (PropertyRecv_ClassName(obj, prop)) return;
            if (PropertyRecv_TextColor(obj, prop)) return;
            if (PropertyRecv_BackColor(obj, prop)) return;
            if (PropertyRecv_ExpandOption(obj, prop)) return;

            if (((Form)prop).Name == "PropertyPageObjectBarcodeScanner")
            {
                PropertyPageObjectBarcodeScanner local = (PropertyPageObjectBarcodeScanner)prop;
                ObjectBarcodeScanner obj2 = (ObjectBarcodeScanner)obj;
                obj2.ObjectArgs = local.ObjectArgs;
            }
        }

        #endregion

		public static void SelectChanged(FormEditGraphic form)
		{
			if(propertySheet == null)	return;
			
			// 아래는 다른 오브젝트를 선택했을 때 이전 오브젝트를 저장할 때 사용한다.
			// propertySheet.SendPropertyToRecv();
			// form.Invalidate();
			// form.SetChangeFlag();

			Property(form);
		}


        /// <summary>
        /// 오브젝트의 크기나 위치가 변경되면 ClassName에 있는 위치/크기 속성을 바꿔준다.
        /// </summary>
        /// <param name="form"></param>
        public static void ObjectPosSizeChanged(FormEditGraphic form)
        {
            if (propertySheet == null) return;

            propertySheet.ObjectPosSizeChanged();
        }
	}
}

