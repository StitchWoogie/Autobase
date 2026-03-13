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
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    public class ObjectArgsModule
    {
        public string filename;
    }
    /// <summary>
    /// Summary description for ObjectModule.
    /// </summary>
    public class ObjectModule : ObjectExpand
    {
        ObjectArgsModule objArgs;

        //Page formModule;

        bool bModuleSameFlag = false;

        public ObjectArgsModule ObjectArgs
        {
            set
            {
                objArgs = value;
            }
            get
            {
                return objArgs;
            }
        }

        public ObjectModule(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, string parent_module, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, ObjectArgsModule args)
            : base(ocp, rect, eid, null, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Module;
            objArgs = args;

            /*
            string filename = MakeFilePath.Graphic(args.filename);

            if (String.Compare(filename, parent_module, true) == 0)
            {
                bModuleSameFlag = true;
            }
            else
            {
                bModuleSameFlag = false;
            }*/

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (bModuleSameFlag == false)
                {
                    Page child = new Page();
                    
                    child.Load(args.filename, 2);

                    //child.ChangeModuleOpticMethod(2);

                    parent_canvas.Children.Add(child);

                    SetShapeOriginal(child);
                    MoveShape();

                    /*
                    //MakeFilePath make = new MakeFilePath();
                    //string filename = make.Graphic(args.filename);

                    formModule = new Page(args.filename);
                    //
                    formModule.FormBorderStyle = FormBorderStyle.None;
                    formModule.TopLevel = false;

                    formModule.WindowState = FormWindowState.Normal;
                    formModule.Size = new Size(0, 0);

                    form.Controls.Add(formModule);

                    formModule.Show();

                    int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                    GetViewZone(ref x1, ref y1, ref x2, ref y2);
                    formModule.Left = x1;
                    formModule.Top = y1;
                    formModule.Width = x2 - x1;
                    formModule.Height = y2 - y1;

                    formModule.ChangeModuleOpticMethod(2);*/
                }
            }
        }

        protected override bool IsNeedMouseHitTest()
        {
            return true;    // Module속에는 무엇이 들어올지 모르므로 마우스 응답은 항상 활성
        }

        /*
        public override void Close()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && formModule != null)
            {
                formModule.Close();
                formModule = null;
            }
        }

        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (formModule != null)
                {
                    formModule.Left = x1;
                    formModule.Top = y1;
                    formModule.Width = x2 - x1;
                    formModule.Height = y2 - y1;
                }
            }
        }

        
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            int width = x2 - x1 + 1;
            int height = y2 - y1 + 1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                DrawClass.gcls(g, x1, y1, x2, y2, Color.LightGray);

                Rectangle r = new Rectangle(x1, y1, width, height);

                Font font = new Font("굴림", 10);

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                g.DrawString(objArgs.filename, font, Brushes.Black, r, format);
            }
            else
            {
                if (bModuleSameFlag)
                {

                    DrawClass.gcls(g, x1, y1, x2, y2, Color.LightGray);

                    Rectangle r = new Rectangle(x1, y1, width, height);

                    Font font = new Font("굴림", 10);

                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    string msg;

                    if (Tools.IsLangKorean())
                    {
                        msg = String.Format("오류:같은 모듈을 사용 했습니다.:{0}", objArgs.filename);
                    }
                    else
                    {
                        msg = String.Format("Error:Parent & Child Module is same:{0}", objArgs.filename);
                    }

                    g.DrawString(msg, font, Brushes.Black, r, format);
                }
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.FileName(writer, objArgs.filename);
        }*/
    }
}
