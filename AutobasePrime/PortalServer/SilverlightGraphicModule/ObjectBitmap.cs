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
using System.Collections.Generic;
using NetTools;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    //[Serializable]
    public class ObjectArgsBitmap
    {
        public string sBitmapFile;
        public int nOverlayMethod;
        public int nRotateFlip;
    }
    /// <summary>
    /// Summary description for ObjectBitmap.
    /// </summary>
    //[Serializable]
    public class ObjectBitmap : ObjectExpand
    {
        ObjectArgsBitmap objArgs;
        int nWidth = 0;
        int nHeight = 0;

        // non serialize해도 결국 Object를 새로 만들어야 하므로 변함이 없다.
        AnimationClass animation = new AnimationClass();

        //[NonSerialized]
        static public List<object> arrayClassList = new List<object>();

        //[NonSerialized]
        UserControl formParent;

        /*
        public ObjectArgsBitmap ObjectArgs
        {
            set
            {
                objArgs = value;
                this.SetFileName(value.sBitmapFile);
            }
            get
            {
                return objArgs;
            }
        }*/

        public ObjectBitmap(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, ObjectArgsBitmap args)
            : base(ocp, rect, eid, null, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Bitmap;
            objArgs = args;
            formParent = form;

            SetFileName(args.sBitmapFile);

            if (nRight == -9999 && nBottom == -9999)
            {	// 이전 버전은 x2, y2 좌표가 없으므로 그림 크기로 맞춰준다.
                nRight = nLeft + nWidth - 1;
                nBottom = nTop + nHeight - 1;
            }

            //EnumRunChangedType change_type;
            //ExpandRun(null, out change_type);
            //ExpandCalcObjectRect();

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);
            }

            Image child = new Image();

            child.Source = animation.GetImageSource(0);
            child.Stretch = Stretch.Fill;

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        public override void Close()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Remove(this);
            }
        }

        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "BitmapSetFile")
            {
                SetFileName((string)args[0]);
                InvalidateObject(formParent);
                return 1;
            }

            return 0;
        }

        void SetFileName(string filename)
        {
            string path;

            path = ObjectAnimation.MakeFilePathGraphicOnRunOrEdit(objCommonProperty, filename);

            animation.LoadImage(path, objArgs.nOverlayMethod == 1, objArgs.nRotateFlip);

            nWidth = animation.Width();
            nHeight = animation.Height();
        }
        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            int swidth = x2 - x1 + 1;
            int sheight = y2 - y1 + 1;

            if (objArgs.nOverlayMethod == 0)
            {
                animation.Putimage(g, x1, y1, swidth, sheight, 0);
            }
            else
            {
                animation.PutimageOverlay(g, x1, y1, swidth, sheight, 0);
            }
        }

        public void SetOriginalSize()
        {
            if (nLeft > nRight) Tools.Temp(ref nLeft, ref nRight);
            if (nTop > nBottom) Tools.Temp(ref nTop, ref nBottom);

            nRight = nLeft + nWidth - 1;
            nBottom = nTop + nHeight - 1;

            ExpandCalcObjectRect();
        }
        
        public override void ObjectSave(CommaTextWriter writer)
        {
            SaveObjectItem.FileName(writer, objArgs.sBitmapFile);
            SaveObjectItem.OverlayMethod(writer, objArgs.nOverlayMethod);
        }

        public override void GetFamilyFile(ArrayList block)
        {
            FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();

            family.filename = objArgs.sBitmapFile;
            block.Add(family);
        }

        public override void ChangeFamilyFile(ArrayList block)
        {
            FAMILY_FILE_STRUCT family;
            family = ObjectGroup.GetMatchFamilyFile(objArgs.sBitmapFile, block);
            if (family == null) return;
            objArgs.sBitmapFile = family.change;
        }

        public override void AddObjectInfo(System.Windows.Forms.TreeNode parent)
        {
            string info = System.IO.Path.GetExtension(this.ToString()).Substring(1);
            info += String.Format(", {0}", this.objArgs.sBitmapFile);

            System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode(info);
            parent.Nodes.Add(node);
        }*/
    }
}
