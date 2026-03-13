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
using NetTools.OldDefine;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace SilverlightGraphicModule
{
    //[Serializable]
    public class ObjectArgsAnimation
    {
        public string sAnimationFile;
        public int nOverlayMethod;
        public int nRotateFlip;
    }

    /// <summary>
    /// Summary description for ObjectAnimation.
    /// </summary>
    /// 
    //[Serializable]
    public class ObjectAnimation : ObjectExpand
    {
        ObjectArgsAnimation objArgs;

        int nWidth;
        int nHeight;

        int nCurrFrame;
        AnimationClass animation = new AnimationClass();

        static public List<object> arrayClassList = new List<object>();

        UserControl formParent;

        public ObjectArgsAnimation ObjectArgs
        {
            set
            {
                objArgs = value;
                this.SetFileName(value.sAnimationFile);
            }
            get
            {
                return objArgs;
            }
        }

        public ObjectAnimation(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, ObjectArgsAnimation args)
            : base(ocp, rect, eid, null, general)
        {
            //
            // TODO: Add constructor logic here
            //
            
            enumObjectType = EnumObjectType.Animation;
            objArgs = args;
            formParent = form;

            nCurrFrame = 0;

            animation.eventHandlerOnAnimationInfoReaded += new EventHandler(animation_eventHandlerOnAnimationInfoReaded);
            SetFileName(args.sAnimationFile);

            // 이전버전 Left Top 좌표만 있던 시절
            if (nRight == -9999 && nBottom == -9999)
            {
                UpdateZone(rect.left, rect.top, rect.left + nWidth - 1, rect.top + nHeight - 1);
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);
            }

            Image child = new Image();
            child.Stretch = Stretch.Fill;   // 영역에 맞추기 위해서는 Fill으로 해야한다.
            child.Source = animation.GetImageSource(nCurrFrame);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        void animation_eventHandlerOnAnimationInfoReaded(object sender, EventArgs e)
        {
            Image child = (Image)GetShapeOriginal();
            if (child == null) return;
            child.Source = animation.GetImageSource(nCurrFrame);
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
            if (command == "AnimationSetFile")
            {
                nCurrFrame = 0;
                SetFileName((string)args[0]);
                InvalidateObject(formParent);
                return 1;
            }

            return 0;
        }

        public static string MakeFilePathGraphicOnRunOrEdit(ObjectCommonProperty cp, string name)
        {
            string path;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                path = String.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(cp.sModuleName), name);
            }
            else
            {
                if (ConfigVarTotal.bLocalFlag)
                {
                    path = String.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(cp.sModuleName), name);
                }
                else
                {
                    string dir = System.IO.Path.GetDirectoryName(cp.sModuleName);

                    int index = dir.IndexOf("graphic", StringComparison.CurrentCultureIgnoreCase);
                    if (index != -1)
                    {
                        dir = dir.Substring(index);
                    }

                    path = MakeFilePath.Project(dir, name);
                }
            }

            return path;
        }

        
        void SetFileName(string filename)
        {
            string path = MakeFilePathGraphicOnRunOrEdit(objCommonProperty, filename);

            animation.LoadImage(path, objArgs.nOverlayMethod == 1, objArgs.nRotateFlip);

            nWidth = animation.Width();
            nHeight = animation.Height();
        }
        
        public override void EventTimerObject(UserControl form)
        {
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

                Image child = (Image)GetShapeOriginal();
                child.Source = animation.GetImageSource(nCurrFrame);
            }
        }

        public static MemoryStream RestoreFromZipStream(Stream sourcestream, string filename)
        {
            //if (sourcestream == null) return null;

            try
            {
                sourcestream.Seek(0, SeekOrigin.Begin); // 처음으로 포인터를 변경 stream을 닫지 않으므로 시작 시 초기화가 필요

                ZipInputStream s = new ZipInputStream(sourcestream);

                ZipEntry entry;

                while ((entry = s.GetNextEntry()) != null)
                {
                    if (String.Compare(entry.Name, filename, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        byte[] data = new byte[2048];

                        MemoryStream stream = new MemoryStream();

                        int size;

                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size == 0) break;

                            stream.Write(data, 0, size);
                        }

                        stream.Seek(0, SeekOrigin.Begin);   // stream이 뒤로 가서 앞으로 이동해야 한다.

                        return stream;
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

    }
}
