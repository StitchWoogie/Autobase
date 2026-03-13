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
    //[Serializable]
    public class ObjectArgsDigitalAnimation
    {
        public string sFileOff;
        public string sFileOn;
        public int nOverlayMethod;
        public int nRotateFlip;
    }

    /// <summary>
    /// Summary description for ObjectAnalogString.
    /// </summary>
    //[Serializable]
    public class ObjectDigitalAnimation : ObjectTag
    {
        public ObjectArgsDigitalAnimation objArgs;
        int nCurrFrame;
        //int nMaxClock;
        //int nMaxFrame;
        //int nWidth;
        //int nHeight;
        AnimationClass aniOn = new AnimationClass();
        AnimationClass aniOff = new AnimationClass();
        AnimationClass animation;
        sbyte bCurrentFlag;

        public ObjectDigitalAnimation(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsDigitalAnimation args)
            : base(ocp, rect, eid, general, null, tag, mouse_response, rMouse)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.DigitalAnimation;
            objArgs = args;

            nCurrFrame = 0;

            Image child = new Image();

            child.Stretch = Stretch.Fill;
            parent_canvas.Children.Add(child);
            SetShapeOriginal(child);
            MoveShape();

            SetFileNameOn(args.sFileOn);    // 속에 ChangeAnimationStruct()가 포함되어 있다.
            SetFileNameOff(args.sFileOff);  // 속에 ChangeAnimationStruct()가 포함되어 있다.

            //ChangeAnimationStruct();

            //Image child = new Image();

            //child.Source = animation.GetImageSource(0);

            //ocp.rootCanvas.Children.Add(child);

            //SetShapeOriginal(child);
            //MoveShape();
        }

        public void SetObjectArgs(ObjectArgsDigitalAnimation args)
        {
            objArgs.nOverlayMethod = args.nOverlayMethod;
            objArgs.sFileOff = args.sFileOff;
            objArgs.sFileOn = args.sFileOn;

            SetFileNameOn(args.sFileOn);
            SetFileNameOff(args.sFileOff);
        }

        TagDiClass GetStructDI(string tag, ref int[] pos)
        {
            TagDiClass di;
            if (this.bPreviewMode)
            {
                di = new TagDiClass();
                DateTime t = DateTime.Now;
                if ((t.Second % 3) == 0)
                    di.curr = 0;
                else
                    di.curr = 1;
                return di;
            }
            else
            {
                return TagLib.GetStructDI(tag, ref pos);
            }
        }

        void ChangeAnimationStruct()
        {
            TagDiClass di = this.GetStructDI(sTagName, ref nTagPos);

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {	// DI
                nCurrFrame = 0;
                if (di.curr == 0)
                {
                    animation = aniOff;
                    //nMaxFrame = aniOff.GetMaxFrame();
                    //nMaxClock = aniOff.GetRPM();
                    //nWidth = aniOff.Width();
                    //nHeight = aniOff.Height();
                }
                else
                {
                    animation = aniOn;
                    //nMaxFrame = aniOn.GetMaxFrame();
                    //nMaxClock = aniOn.GetRPM();
                    //nWidth = aniOn.Width();
                    //nHeight = aniOn.Height();
                }
                bCurrentFlag = di.curr;
            }
            else
            {
                nCurrFrame = 0;
                //nMaxFrame = aniOff.GetMaxFrame();
                //nMaxClock = aniOff.GetRPM();
                //nWidth = aniOff.Width();
                //nHeight = aniOff.Height();
                animation = aniOff;
                bCurrentFlag = 0;
            }

            Image child = (Image)GetShapeOriginal();
            child.Source = animation.GetImageSource(nCurrFrame);

            /*
            TagDiClass di = this.GetStructDI(sTagName, ref nTagPos);

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {	// DI
                nCurrFrame = 0;
                if (di.curr == 0)
                {
                    nMaxFrame = aniOff.GetMaxFrame();
                    nMaxClock = aniOff.GetRPM();
                    nWidth = aniOff.Width();
                    nHeight = aniOff.Height();
                }
                else
                {
                    nMaxFrame = aniOn.GetMaxFrame();
                    nMaxClock = aniOn.GetRPM();
                    nWidth = aniOn.Width();
                    nHeight = aniOn.Height();
                }
                bCurrentFlag = di.curr;
            }
            else
            {
                nCurrFrame = 0;
                nMaxFrame = aniOff.GetMaxFrame();
                nMaxClock = aniOff.GetRPM();
                nWidth = aniOff.Width();
                nHeight = aniOff.Height();
                bCurrentFlag = 0;
            }*/
        }

        void SetFileNameOff(string filename)
        {
            string path;

            path = ObjectAnimation.MakeFilePathGraphicOnRunOrEdit(objCommonProperty, filename);

            aniOff.LoadImage(path, objArgs.nOverlayMethod == 1, objArgs.nRotateFlip);
            ChangeAnimationStruct();
        }

        void SetFileNameOn(string filename)
        {
            string path;

            path = ObjectAnimation.MakeFilePathGraphicOnRunOrEdit(objCommonProperty, filename);

            aniOn.LoadImage(path, objArgs.nOverlayMethod == 1, objArgs.nRotateFlip);
            ChangeAnimationStruct();
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            TagDiClass di = this.GetStructDI(sTagName, ref nTagPos);

            if (nTagPos[0] != TagLib.TAG_NOT_FOUND)
            {
                if (di.curr == 0)
                {
                    if (objArgs.nOverlayMethod == 1)
                    {
                        aniOff.PutimageOverlay(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, nCurrFrame);
                    }
                    else
                    {
                        aniOff.Putimage(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, nCurrFrame);
                    }
                }
                else
                {
                    if (objArgs.nOverlayMethod == 1)
                    {
                        aniOn.PutimageOverlay(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, nCurrFrame);
                    }
                    else
                    {
                        aniOn.Putimage(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, nCurrFrame);
                    }
                }
            }
            else
            {
                // 태그 없습니다.
                if (objArgs.nOverlayMethod == 1)
                {
                    aniOff.PutimageOverlay(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, nCurrFrame);
                }
                else
                {
                    aniOff.Putimage(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, nCurrFrame);
                }
            }
        }

        sbyte bOldFlag = 0;

        public override void EventTimerObjectOnPreview(System.Windows.Forms.Form form)
        {
            TagDiClass di = this.GetStructDI(sTagName, ref nTagPos);
            if (bOldFlag != di.curr)
            {
                bOldFlag = di.curr;
                ChangeAnimationStruct();
            }
            EventTimerObject(form);
        }
        */

        public override void EventTimerObject(UserControl form)
        {
            base.EventTimerObject(form);

            TagDiClass di = this.GetStructDI(sTagName, ref nTagPos);
            if (bCurrentFlag != di.curr)
            {
                bCurrentFlag = di.curr;
                ChangeAnimationStruct();
            }

            int nMaxFrame = animation.GetMaxFrame();

            if (nMaxFrame <= 1) return;		// animation file 이 한프레임 밖에 없다.

            int speed;

            if (ExpandIsUseAnimationSpeed())
            {	// animation rpm 방식을 사용한다.
                speed = ExpandGetAnimationSpeed();
            }
            else
            {
                speed = animation.GetRPM();
            }

            if (speed == 0) return;

            DateTime dt = DateTime.Now;

            int frame;

            if (speed < 0)
            {
                frame = (int)(Math.Abs(speed) * nMaxFrame * (dt.Second * 1000 + dt.Millisecond) / 60000);
                frame = frame % nMaxFrame;
                frame = nMaxFrame - 1 - frame;
            }
            else
            {
                frame = (int)(speed * nMaxFrame * (dt.Second * 1000 + dt.Millisecond) / 60000);
                frame = frame % nMaxFrame;
            }

            if (nCurrFrame != frame)
            {
                nCurrFrame = frame;
                //InvalidateObject(form);
                Image child = (Image)GetShapeOriginal();
                child.Source = animation.GetImageSource(nCurrFrame);
            }
        }
        /*
        public override void EventTag(System.Windows.Forms.Form form, COMM_EVENT_STRUCT tagevent)
        {
            if (tagevent.tag_type != EnumTagType.DI) return;
            if (String.Compare(tagevent.tag, sTagName, true) != 0) return;

            ChangeAnimationStruct();
            InvalidateObject(form);
        }

        public void SetOriginalSize()
        {
            nRight = nLeft + nWidth - 1;
            nBottom = nTop + nHeight - 1;
        }

        public override void ObjectSave(CommaTextWriter writer)
        {
            ObjectSaveTag(writer);
            writer.WriteLine("\tFileNameOn,{0}", objArgs.sFileOn);
            writer.WriteLine("\tFileNameOff,{0}", objArgs.sFileOff);
            SaveObjectItem.OverlayMethod(writer, objArgs.nOverlayMethod);
        }

        public override void GetFamilyFile(ArrayList block)
        {
            FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();

            family.filename = objArgs.sFileOn;
            block.Add(family);

            family = new FAMILY_FILE_STRUCT();

            family.filename = objArgs.sFileOff;
            block.Add(family);
        }

        public override void ChangeFamilyFile(ArrayList block)
        {
            FAMILY_FILE_STRUCT family;

            family = ObjectGroup.GetMatchFamilyFile(objArgs.sFileOn, block);
            if (family != null)
            {
                objArgs.sFileOn = family.change;
                return;
            }

            family = ObjectGroup.GetMatchFamilyFile(objArgs.sFileOff, block);
            if (family != null)
            {
                objArgs.sFileOff = family.change;
            }
        }

        public override void AddObjectInfo(System.Windows.Forms.TreeNode list)
        {
            string info = Path.GetExtension(this.ToString()).Substring(1);
            info += String.Format(", {0}", this.sTagName);
            info += String.Format(", {0}", this.objArgs.sFileOff);
            info += String.Format(", {0}", this.objArgs.sFileOn);

            System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode(info);

            list.Nodes.Add(info);
        }*/
    }
}
