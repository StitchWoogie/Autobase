using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using AutoLib;
using AutoLibLocal;
using System.IO;

namespace Studio
{
	/// <summary>
	/// Summary description for AnimationPlay.
	/// </summary>
	public class AnimationPlay : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

		public AnimationPlay()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnimationPlay));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // AnimationPlay
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Icon = null;
            this.Name = "AnimationPlay";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AnimationPlay_Paint);
            this.Load += new System.EventHandler(this.AnimationPlay_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void AnimationPlay_Load(object sender, System.EventArgs e)
		{
			timer1.Enabled = true;
		}

		AnimationClass ani = null;


        public void SetFileName(string filename, MemoryStream ms)
        {
            ani = new AnimationClass();
            ani.LoadImageFromStream(ms, filename);
            ChangeAnimationStruct();
            
            Invalidate();
        }

		public void SetFileName(string filename)
		{
			string path;

			if(filename != null && filename.Length != 0) 
			{
				if(Path.GetPathRoot(filename).Length == 0) 
				{
					path = MakeFilePath.Graphic(filename);
				}
				else 
				{
					path = filename;
				}
			}

			else 
			{
				path = "";
			}

            if (File.Exists(path))
            {
                ani = new AnimationClass();
                ani.LoadImage(path);
                ChangeAnimationStruct();
            }
            else
            {
                ani = null;
            }

			Invalidate();
		}

		int nCurrFrame;
		int nMaxClock;
		int nMaxFrame;
		int nWidth;
		int nHeight;

		void ChangeAnimationStruct()
		{
			nCurrFrame = 0;
			nMaxFrame = ani.GetMaxFrame();
			nMaxClock = ani.GetRPM();
			nWidth =  ani.Width();
			nHeight = ani.Height();
			//bCurrentFlag = 0;
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
            if (ani == null) return;

			if(nMaxFrame <= 1)	return;		// animation file 이 한프레임 밖에 없다.

			int speed;

			speed = nMaxClock;

			if(speed == 0)	return;

			DateTime dt = DateTime.Now;

			int frame = (int)((long)speed*nMaxFrame*(dt.Second*1000L+dt.Millisecond)/60000);

			frame = frame%nMaxFrame;

			if(nCurrFrame != frame) 
			{
				nCurrFrame  = frame;
				Invalidate();
			}
		}

		private void AnimationPlay_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            if (ani == null)
            {
                //NetTools.DrawClass.gcls(e.Graphics, 0, 0, nWidth, nHeight, Color.Red);
            }
            else
            {
                ani.Putimage(e.Graphics, 0, 0, nWidth - 1, nHeight - 1, nCurrFrame);
            }
		}
	}
}
