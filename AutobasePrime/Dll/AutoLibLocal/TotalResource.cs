using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for TotalResource.
	/// </summary>
	public class TotalResource : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		public System.Windows.Forms.ImageList imageListTagType;
		public static TotalResource res = new TotalResource();

		public TotalResource()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TotalResource));
			this.imageListTagType = new System.Windows.Forms.ImageList(this.components);
			// 
			// imageListTagType
			// 
			this.imageListTagType.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageListTagType.ImageSize = new System.Drawing.Size(32, 16);
			this.imageListTagType.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTagType.ImageStream")));
			this.imageListTagType.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// TotalResource
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(320, 245);
			this.Name = "TotalResource";
			this.Text = "TotalResource";

		}
		#endregion

		public enum ImageTagType
		{
			AI = 0,
			AO = 1,
			DI = 2,
			DO = 3,
			ST = 4,
			Unknown = 5,
			GR = 6,
			GDO = 7,
		}
	}
}
