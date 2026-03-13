using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using System.IO;
using NetTools;
using System.Diagnostics;

namespace AniEditLib
{
    public partial class FormAniEdit : Form
    {
        public string sFileName;

        int nMaxFrame;
        int nMaxClock;
        int nWidth;
        int nHeight;
        int nCurrFrame;
        AnimationClass animation = new AnimationClass();
        bool bRunning = false;

        bool bNewFlag = false;
        bool bChangeFlag = false;

        public FormAniEdit(string filename)
        {
            InitializeComponent();

            sFileName = filename;
        }

        public FormAniEdit(string filename, int width, int height, int frame, int rpm, int bitsperpixel)
        {
            InitializeComponent();

            bNewFlag = true;
            sFileName = filename;
            animation = new AnimationClass(width, height, frame, rpm, bitsperpixel);
        }

        public FormAniEdit(string filename, Bitmap bitmap)
        {
            InitializeComponent();

            bNewFlag = true;
            sFileName = filename;

            animation = new AnimationClass(bitmap.Width, bitmap.Height, 1, 60, bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? 24 : 32);

            animation.SetBitmap(0, bitmap);
        }

        void LoadFromGif(string filename)
        {
            GifImage gif = new GifImage(filename);

            int count = gif.GetCount();
            int rpm = gif.GetSimpleRPM(); //2024-12-11 PSU 추가

            Image image = gif.GetFrame(0);

            //animation.ChangeProperty(image.Width, image.Height, 60/count, 8);
            animation.ChangeProperty(image.Width, image.Height, rpm, 8);  //2024-12-11 PSU 수정

            for (int i = 0; i < count; i++)
            {
                image = gif.GetFrame(i);
                animation.Insert(i, (Bitmap)image);
            }
        }

        private async void FormAniEdit_Load(object sender, EventArgs e)
        {
            if (!bNewFlag)
            {
                if (String.Compare(Path.GetExtension(sFileName), ".gif", true) == 0)
                {
                    LoadFromGif(sFileName);
                }
                else // ani
                {
                    await animation.LoadImage(sFileName);
                }
            }

            nMaxFrame = animation.GetMaxFrame();
            nMaxClock = animation.GetRPM();
            nWidth = animation.Width();
            nHeight = animation.Height();
            nCurrFrame = 0;

            if (bRunning)
            {
                timer1.Enabled = true;
            }

            CalcSize();

            SetTitle();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (nMaxFrame <= 1) return;		// animation file 이 한프레임 밖에 없다.

            int speed;

            speed = nMaxClock;

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
                panel1.Invalidate();
            }
        }

        int nLimitX=0, nLimitY=0;

        void CalcSize()
        {
            nLimitX = panel1.ClientRectangle.Width / (nWidth + 10);
            if (nLimitX < 1) nLimitX = 1;
            nLimitY = (nMaxFrame+(nLimitX-1))/nLimitX;

            int ly = panel1.ClientRectangle.Height / (nHeight + 10);

            if (nLimitY > ly)
            {
                this.vScrollBar1.Enabled = true;
                this.vScrollBar1.Minimum = 0;
                this.vScrollBar1.Maximum = (nLimitY - ly)+this.vScrollBar1.LargeChange;
            }
            else
            {
                this.vScrollBar1.Enabled = false;
                this.vScrollBar1.Value = 0;
            }
        }

        int nCursor = 0;

        private void FormAniEdit_Paint(object sender, PaintEventArgs e)
        {
            
        }

        public void RunAnimation()
        {
            this.bRunning = !bRunning;

            if (bRunning)
            {
                this.timer1.Enabled = true;
            }
            else
            {
                this.timer1.Enabled = false;
            }

            panel1.Invalidate();
        }

        private void FormAniEdit_SizeChanged(object sender, EventArgs e)
        {
            CalcSize();
            panel1.Invalidate();
        }

        public void Insert()
        {
            Bitmap bitmap = animation.NewBitmap();
            animation.Insert(nCursor, bitmap);

            nMaxFrame = animation.GetMaxFrame();

            CalcSize();

            panel1.Invalidate();

            bChangeFlag = true;
            SetTitle();
        }

        public static string sFilterBitmap = "Bitmap(PNG,BMP,PCX,GIF,TIF,JPG,WMF,EMF)|*.png;*.bmp;*.pcx;*.gif;*.tif;*.jpg;*.wmf;*.emf";
        public static string sFilterAnimation = "Animation files(*.ANI)|*.ani";

        public static bool SelectBitmapFile(ref string filename)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = sFilterBitmap;
            dialog.InitialDirectory = TotalConfig.sDirWorkProject + "\\graphic";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filename = dialog.FileName; // 9.4.1
                return true;
            }
            return false;
        }

        public void InsertFromFile()
        {
            string filename = "";

            if (!SelectBitmapFile(ref filename)) return;

            PicTools.PictureToBitmap load = new PicTools.PictureToBitmap();

            Bitmap bitmap = load.Load(filename);
            
            animation.Insert(nCursor, bitmap);

            nMaxFrame = animation.GetMaxFrame();

            CalcSize();

            panel1.Invalidate();

            bChangeFlag = true;
            SetTitle();
        }

        public void Delete()
        {
            animation.Delete(nCursor);

            nMaxFrame = animation.GetMaxFrame();

            CalcSize();

            panel1.Invalidate();

            bChangeFlag = true;
            SetTitle();
        }

        public static Bitmap bitmapCopy = null;

        public void Copy()
        {
            Bitmap bitmap = animation.GetBitmap(nCursor);

            if (bitmap != null)
            {
                Clipboard.SetImage(bitmap);
                bitmapCopy = bitmap;
            }
        }

        public void Paste()
        {
            if (bitmapCopy == null) return;

            Bitmap bitmap = (Bitmap)Tools.CopyObject(bitmapCopy);

            animation.Insert(nCursor, bitmap);

            nMaxFrame = animation.GetMaxFrame();

            CalcSize();

            panel1.Invalidate();

            bChangeFlag = true;
            SetTitle();
        }

        public void PasteFromClipboard()
        {
            if (!Clipboard.ContainsImage()) return;

            Bitmap bitmap = (Bitmap)Clipboard.GetImage();

            animation.Insert(nCursor, bitmap);

            nMaxFrame = animation.GetMaxFrame();

            CalcSize();

            panel1.Invalidate();

            bChangeFlag = true;
            SetTitle();
        }

        public bool FileSave()
        {
            /*
            string ext = Path.GetExtension(sFileName);
            if (String.Compare(ext, ".ani", true) == 0)
            {
                return SaveAs(Path.ChangeExtension(sFileName, ".an3")); // 신형식
            }*/

            string name = Path.GetFileNameWithoutExtension(sFileName);
            if (String.Compare(name, 0, "noname", 0, 6, true) == 0)
            {
                return SaveAs(Path.ChangeExtension(sFileName, ".ani"));
            }

            if (String.Compare(Path.GetExtension(sFileName), ".ani", true) != 0)
            {
                return SaveAs(Path.ChangeExtension(sFileName, ".ani"));
            }
            
            return FileSave(sFileName);
        }

        public bool FileSave(string filename)
        {
            // 먼저 폴더가 존재하는지 검사한다.
            string dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            BackUp.BackUpFile(filename);

            if (!animation.Save(filename))
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("파일을 저장할 수 없습니다.", filename);
                else
                    MessageBox.Show("Can't save the file.", filename);

                return false;
            }
            else
            {
                bChangeFlag = false;
                sFileName = filename;
                SetTitle(); // title을 다시 그린다.
            }

            return true;
        }

        bool SaveAs(string init_file)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = sFilterAnimation;
            string init = TotalConfig.sDirWorkProject + "\\Graphic";

            if (!Directory.Exists(init))
            {
                Directory.CreateDirectory(init);
            }
            dialog.InitialDirectory = init;

            if (init_file != null)
            {
                dialog.FileName = init_file;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bool retn = FileSave(dialog.FileName);
                return retn;
            }

            return false;
        }

        public void FileSaveAs()
        {
            SaveAs(sFileName);
        }

        public void SetTitle()
        {
            string title;
            
            title = sFileName;

            if (bChangeFlag)
            {
                title += " *";
            }

            //title += String.Format(", {0}%", workThis.obj.nOpticRate);

            this.Text = title;
        }

        private void FormAniEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bChangeFlag)
            {
                DialogResult result;

                if (Tools.IsLangKorean())
                    result = MessageBox.Show(Path.GetFileName(sFileName) + " 파일을 저장하지 않았습니다.\n파일을 저장할까요?", sFileName, MessageBoxButtons.YesNoCancel);
                else if (Tools.IsLangChinese())
                    result = MessageBox.Show(Path.GetFileName(sFileName) + " 文件还没保存。\n想保存文件吗？", sFileName, MessageBoxButtons.YesNoCancel);
                else
                    result = MessageBox.Show(Path.GetFileName(sFileName) + " File not saved.\nSave to file?", sFileName, MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.No) return;
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

                if (!FileSave())
                {
                    e.Cancel = true;
                    return;
                }

            }
        }

        public void Property()
        {
            FormProperty dialog = new FormProperty();

            dialog.nWidth = nWidth;
            dialog.nHeight = nHeight;
            dialog.nFrame = nMaxFrame;
            dialog.nRpm = nMaxClock;
            dialog.nColor = animation.GetColor();

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                nMaxClock = dialog.nRpm;
                nWidth = dialog.nWidth;
                nHeight = dialog.nHeight;

                animation.ChangeProperty(dialog.nWidth, dialog.nHeight, dialog.nRpm, dialog.nColor);

                CalcSize();

                bChangeFlag = true;
                panel1.Invalidate();
                SetTitle();
            }
        }

        public static bool ExecuteBitmapEditor(string argument)
        {
            try
            {
                DateTime oldt = File.GetLastWriteTime(argument);
                Process p = Process.Start(Config.sBitmapEditor, argument);

                p.WaitForExit();

                DateTime t = File.GetLastWriteTime(argument);

                if (oldt != t)
                {
                    return true;
                }
            }
            catch (Exception exception)
            {
                string msg;
                if (Tools.IsLangKorean())
                {
                    msg = String.Format("그림 편집기를 실행할 수 없습니다.\n오류내용-{0}", exception.Message);
                    MessageBox.Show(msg, Config.sBitmapEditor);
                }
                else if (Tools.IsLangChinese())
                {
                    msg = String.Format("不能运行图片编辑器.\n错误内容-{0}", exception.Message);
                    MessageBox.Show(msg, Config.sBitmapEditor);
                }
                else
                {
                    msg = String.Format("Can't run bitmap editor.\nError Message-{0}", exception.Message);
                    MessageBox.Show(msg, Config.sBitmapEditor);
                }
            }

            return false;
        }

        public void EditBitmap()
        {
            string filename;

            filename = Path.GetTempFileName();
            filename = Path.ChangeExtension(filename, ".png");

            Bitmap bitmap = animation.GetBitmap(nCursor);
            bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Png);

            if (ExecuteBitmapEditor(filename))
            {
                bitmap = (Bitmap)Image.FromFile(filename);
                animation.SetBitmap(nCursor, bitmap);

                bChangeFlag = true;
                panel1.Invalidate();
                SetTitle();
            }
        }

        public void Add(Bitmap bitmap)
        {
            animation.Insert(nMaxFrame, bitmap);

            nMaxFrame = animation.GetMaxFrame();

            nCursor = nMaxFrame - 1;

            CalcSize();

            panel1.Invalidate();

            bChangeFlag = true;
            SetTitle();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (bRunning)   // run mode
            {
                animation.Putimage(e.Graphics, ClientRectangle.Width / 2 - nWidth / 2, ClientRectangle.Height / 2 - nHeight / 2, nWidth, nHeight, nCurrFrame);
            }
            else
            {
                int x, y;
                int frame = 0;

                frame = nLimitX * vScrollBar1.Value;
                y = 10;
                for (int i = 0; i < nLimitY; i++, y += nHeight + 10)
                {
                    x = 10;
                    for (int j = 0; j < nLimitX; j++, x += nWidth + 10, frame++)
                    {
                        if (frame >= nMaxFrame) goto ok_out;
                        if (nCursor == frame)
                        {
                            Pen pen = new Pen(Color.Blue, 3);
                            e.Graphics.DrawRectangle(pen, x - 2, y - 2, nWidth + 3, nHeight + 3);
                        }
                        else
                            NetTools.DrawClass.PopRectangle2(e.Graphics, x - 1, y - 1, x + nWidth, y + nHeight);
                        animation.Putimage(e.Graphics, x, y, nWidth, nHeight, frame);
                    }
                }
            ok_out: ;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (StopAnimation()) return;

            int x, y;
            int frame = 0;

            frame = nLimitX * vScrollBar1.Value;

            y = 10;
            for (int i = 0; i < nLimitY; i++, y += nHeight + 10)
            {
                x = 10;
                for (int j = 0; j < nLimitX; j++, x += nWidth + 10, frame++)
                {
                    if (frame >= nMaxFrame) goto ok_out;

                    if (e.X >= x && e.X < x + nWidth && e.Y >= y && e.Y < y + nHeight)
                    {
                        if (nCursor != frame)
                        {
                            nCursor = frame;
                            panel1.Invalidate();
                            return;
                        }
                    }
                }
            }
        ok_out: ;
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            this.panel1.Invalidate();
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditBitmap();
        }

        bool StopAnimation()
        {
            if (!bRunning) return false;

            this.bRunning = false;

            this.timer1.Enabled = false;

            panel1.Invalidate();

            return true;
        }

        private void FormAniEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
                StopAnimation();
        }
    }
}
