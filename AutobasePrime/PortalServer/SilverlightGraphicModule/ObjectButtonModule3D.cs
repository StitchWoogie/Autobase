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
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    public class ObjectButtonModule3D : ObjectButtonPublic
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

        public ObjectButtonModule3D(ObjectCommonProperty ocp, Canvas parent_canvas, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general, ObjectArgsButtonPublic args, string filename)
            : base(ocp, parent_canvas, rect, eid, lf, general, args)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.ButtonModule3D;
            sModuleName = filename;
        }

        void GoToPage(string filename)
        {
            /*
            //App app = (App)Application.Current;

            Page page = objCommonProperty.rootPage;
            page.LayoutRoot.Children.Clear();
            page.Add(control);*/
        }

        protected override void OnClicked()
        {
            /* string filename = MakeFilePath.Graphic(sModuleName);
            //Page page = new Page(sModuleName, -1);
            GoToPage(filename); */

            // objCommonProperty.rootPage.ReLoad(sModuleName); 같은 콘트롤에서 사용할 때
            
            //string url = String.Format("GraphicModulePage.aspx?filename={0}", sModuleName);
            //System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(url, UriKind.Relative));

            string url = MakeFilePath.SilverlightGraphicModule(sModuleName);
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(url, UriKind.Absolute));
        }

        /*
        public override bool WmLeftButtonUp(UserControl form, MouseEventArgs e)
        {
            bool retn = base.WmLeftButtonUp(form, e);

            if (retn == true)
            {


                string filename = MakeFilePath.Graphic(sModuleName);

                GraphicTool.RestoreGraphicWindow(filename, -1, 0, 0);
            }

            return retn;
        }

        
        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.TextColor(writer, GetTextColor());
            SaveObjectItem.BackColor(writer, GetBackColor());
            SaveObjectItem.FileName(writer, sModuleName);
            SaveObjectItem.String(writer, this.Text);
            ObjectSaveFont(writer);
        }*/

    }
}
