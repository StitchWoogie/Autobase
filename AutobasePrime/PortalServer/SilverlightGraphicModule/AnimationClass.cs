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
using System.Windows.Media.Imaging;
using System.IO;
using NetTools;

namespace SilverlightGraphicModule
{
    public class ANIMATION_STRUCT2
    {
        public byte[] id = new byte[9];			// "ANIMATION"
        public short version;					// 2
        public byte[] description = new byte[78];// description
        public short rpm;						// rpm
        public short frame;
        public short clock;						// one frame clock count
        public short width;						//
        public short height;					//
        public short bitsperpixel;				// bits per pixel
        public System.Int32 oneframesize;		// one frame size
    }

    /// <summary>
    /// Summary description for AnimationClass.
    /// </summary>
    /// 
    //[Serializable]
    public class AnimationClass
    {
        BitmapImage[] bitmapThis;

        int nMaxFrame = 0;
        int nRpm = 0;
        int nWidth = 100;
        int nHeight = 50;
        
        //[NonSerialized]
        string sErrorString;
        string sFileName;

        public bool bError = false;

        public AnimationClass()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        bool bOverlay = false;
        int nRotateFlip;

        public int LoadImage(string filename, bool overlay, int rotate_flip)
        {
            bOverlay = overlay;
            nRotateFlip = rotate_flip;
            bError = false;

            sFileName = filename;

            NetTools.UriSplit split = new NetTools.UriSplit();
            split.Split(filename);

            string dir = split.GetDirectoryName();
            string file = split.GetFileName();
            string ext = split.GetExtension();

            //file = System.Windows.Browser.HttpUtility.UrlEncode(file);  // SPACE는 +가 된다.
            file = System.Uri.EscapeDataString(file);  // SPACE는 %20이 된다.
            
            if (String.Compare(ext, ".ani", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                filename = String.Format("{0}/WebPublish/{1}.txt", dir, file);

                System.Net.WebClient client = new System.Net.WebClient();
                client.OpenReadCompleted += new System.Net.OpenReadCompletedEventHandler(client_OpenReadCompleted);
                client.OpenReadAsync(new Uri(filename, UriKind.Absolute));
            }
            else
            {
                if(overlay) 
                    filename = String.Format("{0}/WebPublish/{1}_00_R{2}_ovr.png", dir, file, nRotateFlip);
                else
                    filename = String.Format("{0}/WebPublish/{1}_00_R{2}.png", dir, file, nRotateFlip);

                bitmapThis = new BitmapImage[1];

                bitmapThis[0] = new BitmapImage();

                bitmapThis[0].UriSource = new Uri(filename, UriKind.Absolute);  // UriKind.Relative 는 잘 안됨

                nMaxFrame = 1;
            }

            return 1;
        }

        public event EventHandler eventHandlerOnAnimationInfoReaded;

        void client_OpenReadCompleted(object sender, System.Net.OpenReadCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true) return;

            if (e.Result == null) return;

            System.Windows.Resources.StreamResourceInfo resinfo = new System.Windows.Resources.StreamResourceInfo(e.Result, null);

            TextReader reader = new StreamReader(resinfo.Stream);

            if (reader == null) return;

            string one_line;
            CommaTextReader comma = new CommaTextReader();
            string command = "";
            int frame = 0;

            NetTools.UriSplit split = new NetTools.UriSplit();
            split.Split(sFileName);

            string dir = split.GetDirectoryName();
            string file = split.GetFileName();

            //file = System.Windows.Browser.HttpUtility.UrlEncode(file);
            file = System.Uri.EscapeDataString(file);  // SPACE는 %20이 된다.

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                if (one_line.Length == 0) continue;
                if (one_line[0] == ';') continue;

                comma.Set(one_line);
                comma.GetString(ref command);

                if (String.Compare(command, "Frame", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    comma.GetInt(ref this.nMaxFrame);
                    bitmapThis = new BitmapImage[nMaxFrame];
                }
                else if (String.Compare(command, "Rpm", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    comma.GetInt(ref this.nRpm);
                }
                else if (String.Compare(command, "Size", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    comma.GetInt(ref this.nWidth);
                    comma.GetInt(ref this.nHeight);
                }

                else if (String.Compare(command, "Member", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    string filename;
                    
                    if(bOverlay)
                        filename = String.Format("{0}/WebPublish/{1}_{2:00}_R{3}_ovr.png", dir, file, frame, nRotateFlip);
                    else
                        filename = String.Format("{0}/WebPublish/{1}_{2:00}_R{3}.png", dir, file, frame, nRotateFlip);

                    bitmapThis[frame] = new BitmapImage();
                    bitmapThis[frame].UriSource = new Uri(filename, UriKind.Absolute);  // UriKind.Relative 는 잘 안됨

                    frame++;
                }
                else { }
            }

            reader.Close();

            if (eventHandlerOnAnimationInfoReaded != null)
                eventHandlerOnAnimationInfoReaded(this, EventArgs.Empty);
        }

        public int Width()
        {
            return nWidth;
        }

        public int Height()
        {
            return nHeight;
        }

        public int GetMaxFrame()
        {
            return nMaxFrame;
        }

        public int GetRPM()
        {
            return nRpm;
        }

        BitmapImage WaitImage()
        {
            BitmapImage bitmap = new BitmapImage(new Uri(@"Resources/WaitImage.png", UriKind.Relative));

            return bitmap;
        }

        public BitmapImage GetImageSource(int frame)
        {
            //if (bitmapThis == null) return WaitImage();   화면이 번쩍해서 좋지않다.
            if (bitmapThis == null) return null;

            if (frame >= nMaxFrame)
                frame = 0;
            if (frame < 0)
                frame = 0;

            return bitmapThis[frame];
        }
        
        void SetErrorString(string err)
        {
            sErrorString = err;
        }
    }
}
