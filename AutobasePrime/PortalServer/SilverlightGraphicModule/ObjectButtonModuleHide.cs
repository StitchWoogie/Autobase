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
using NetTools.OldDefine;
using AutoLib;

namespace SilverlightGraphicModule
{
    public class ObjectButtonModuleHide : ObjectExpand
    {
        string sModuleName;

        public string ModuleName
        {
            get
            {
                return sModuleName;
            }
            set
            {
                sModuleName = value;
            }
        }

        public ObjectButtonModuleHide(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, string filename)
            : base(ocp, rect, eid, null, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ButtonModuleHide;
            sModuleName = filename;
            
            AddMouseZone(0, 0, 0, 0);
            
            Canvas child = new Canvas();            // 아무것도 없으면 마우스가 반응할 수 없으므로 비어 있는 오브젝트를 사용한다. 2009.6.16(10.0.0.5)

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        /*
        void GoToPage(UserControl control)
        {
            App app = (App)Application.Current;

            objCommonProperty.rootCanvas.Children.Clear();
            objCommonProperty.rootCanvas.Children.Add(control);
        }*/

        public override bool WmLeftButtonDown(UserControl form, MouseEventArgs e)
        {
            if (!bOnMouseZone) return false;

            string url = MakeFilePath.SilverlightGraphicModule(sModuleName);
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(url, UriKind.Absolute));
            
            return true;
        }

        public override bool WmLeftButtonUp(UserControl form, MouseEventArgs e)
        {
            return false;
        }

        
    }
}
