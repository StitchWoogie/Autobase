using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal.KeyLock;
using NetTools;
using AutoLib;
using AutoLibLocal;
using System.Management;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography;
using LocalMain.KeyLockInfo;
using System.IO;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormInformationKeyLock. 
	/// </summary>
	public class FormInformationKeyLock : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labelSerialNumber;
		private System.Windows.Forms.Label labelTagSize;
		private System.Windows.Forms.Label labelTagUsed;
		private System.Windows.Forms.Label labelRuntime;
		private System.Windows.Forms.Label labelOemString;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Panel panelVersion;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Label labelWebUser;
        private Button buttonGetLicense;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormInformationKeyLock()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInformationKeyLock));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelWebUser = new System.Windows.Forms.Label();
            this.labelOemString = new System.Windows.Forms.Label();
            this.labelRuntime = new System.Windows.Forms.Label();
            this.labelTagUsed = new System.Windows.Forms.Label();
            this.labelTagSize = new System.Windows.Forms.Label();
            this.labelSerialNumber = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panelVersion = new System.Windows.Forms.Panel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonGetLicense = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.labelWebUser);
            this.groupBox1.Controls.Add(this.labelOemString);
            this.groupBox1.Controls.Add(this.labelRuntime);
            this.groupBox1.Controls.Add(this.labelTagUsed);
            this.groupBox1.Controls.Add(this.labelTagSize);
            this.groupBox1.Controls.Add(this.labelSerialNumber);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // labelWebUser
            // 
            this.labelWebUser.AccessibleDescription = null;
            this.labelWebUser.AccessibleName = null;
            resources.ApplyResources(this.labelWebUser, "labelWebUser");
            this.labelWebUser.Font = null;
            this.labelWebUser.Name = "labelWebUser";
            // 
            // labelOemString
            // 
            this.labelOemString.AccessibleDescription = null;
            this.labelOemString.AccessibleName = null;
            resources.ApplyResources(this.labelOemString, "labelOemString");
            this.labelOemString.Font = null;
            this.labelOemString.Name = "labelOemString";
            // 
            // labelRuntime
            // 
            this.labelRuntime.AccessibleDescription = null;
            this.labelRuntime.AccessibleName = null;
            resources.ApplyResources(this.labelRuntime, "labelRuntime");
            this.labelRuntime.Font = null;
            this.labelRuntime.Name = "labelRuntime";
            // 
            // labelTagUsed
            // 
            this.labelTagUsed.AccessibleDescription = null;
            this.labelTagUsed.AccessibleName = null;
            resources.ApplyResources(this.labelTagUsed, "labelTagUsed");
            this.labelTagUsed.Font = null;
            this.labelTagUsed.Name = "labelTagUsed";
            // 
            // labelTagSize
            // 
            this.labelTagSize.AccessibleDescription = null;
            this.labelTagSize.AccessibleName = null;
            resources.ApplyResources(this.labelTagSize, "labelTagSize");
            this.labelTagSize.Font = null;
            this.labelTagSize.Name = "labelTagSize";
            // 
            // labelSerialNumber
            // 
            this.labelSerialNumber.AccessibleDescription = null;
            this.labelSerialNumber.AccessibleName = null;
            resources.ApplyResources(this.labelSerialNumber, "labelSerialNumber");
            this.labelSerialNumber.Font = null;
            this.labelSerialNumber.Name = "labelSerialNumber";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.panelVersion);
            this.groupBox2.Controls.Add(this.labelVersion);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // panelVersion
            // 
            this.panelVersion.AccessibleDescription = null;
            this.panelVersion.AccessibleName = null;
            resources.ApplyResources(this.panelVersion, "panelVersion");
            this.panelVersion.BackgroundImage = null;
            this.panelVersion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelVersion.Font = null;
            this.panelVersion.Name = "panelVersion";
            // 
            // labelVersion
            // 
            this.labelVersion.AccessibleDescription = null;
            this.labelVersion.AccessibleName = null;
            resources.ApplyResources(this.labelVersion, "labelVersion");
            this.labelVersion.Font = null;
            this.labelVersion.Name = "labelVersion";
            // 
            // buttonClose
            // 
            this.buttonClose.AccessibleDescription = null;
            this.buttonClose.AccessibleName = null;
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.BackgroundImage = null;
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Font = null;
            this.buttonClose.Name = "buttonClose";
            // 
            // buttonGetLicense
            // 
            this.buttonGetLicense.AccessibleDescription = null;
            this.buttonGetLicense.AccessibleName = null;
            resources.ApplyResources(this.buttonGetLicense, "buttonGetLicense");
            this.buttonGetLicense.BackgroundImage = null;
            this.buttonGetLicense.Font = null;
            this.buttonGetLicense.Name = "buttonGetLicense";
            this.buttonGetLicense.UseVisualStyleBackColor = true;
            this.buttonGetLicense.Click += new System.EventHandler(this.buttonGetLicense_Click);
            // 
            // FormInformationKeyLock
            // 
            this.AcceptButton = this.buttonClose;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.buttonGetLicense);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormInformationKeyLock";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormInformationKeyLock_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion


        

		private void FormInformationKeyLock_Load(object sender, System.EventArgs e)
		{
			string buf;
			
			this.labelSerialNumber.Text = KeyLock.sSerialNumber;
			if(Tools.IsLangKorean())
				buf = String.Format("고유번호: {0}", KeyLock.sSerialNumber);
			else if(Tools.IsLangJapanese())
				buf = String.Format("製品番号: {0}", KeyLock.sSerialNumber);
			else if(Tools.IsLangChinese())
				buf = String.Format("固有号码: {0}", KeyLock.sSerialNumber);
			else
				buf = String.Format("Serial Number: {0}", KeyLock.sSerialNumber);
			this.labelSerialNumber.Text = buf;

            if (KeyLock.bExistLocalKey)
            {
                int tag_size = KeyLock.nTagSize;
                if (Tools.IsLangKorean())
                {
                    if (tag_size == 0)
                        buf = String.Format("태그수 : Unlimited");
                    else
                        buf = String.Format("태그수 : {0}", tag_size);
                }
                else if (Tools.IsLangJapanese())
                {
                    if (tag_size == 0)
                        buf = String.Format("タグ数 : Unlimited");
                    else
                        buf = String.Format("タグ数 : {0}", tag_size);
                }
                else if (Tools.IsLangChinese())
                {
                    if (tag_size == 0)
                        buf = String.Format("标记数 : Unlimited");
                    else
                        buf = String.Format("标记数 : {0}", tag_size);
                }
                else
                {
                    if (tag_size == 0)
                        buf = String.Format("Tag Size : Unlimited");
                    else
                        buf = String.Format("Tag Size : {0}", tag_size);
                }
            }
            else
            {
                if(Tools.IsLangKorean())
                    buf = String.Format("태그수 : 키락 없음");
                else
                    buf = String.Format("Tag Size : Keylock not found.");
            }

			this.labelTagSize.Text = buf;
            
			if(Tools.IsLangKorean())
				buf = String.Format("사용된 태그수 : {0}", KeyLockGetTotalTagUsed());
			else if(Tools.IsLangJapanese())
				buf = String.Format("使用タグ数 : {0}", KeyLockGetTotalTagUsed());
			else if(Tools.IsLangChinese())
				buf = String.Format("已使用的标记数 : {0}", KeyLockGetTotalTagUsed());
			else
				buf = String.Format("Tag Used : {0}", KeyLockGetTotalTagUsed());

			this.labelTagUsed.Text = buf;

			if(Tools.IsLangKorean())
				buf = String.Format("{0}", KeyLock.nKeyLockRunOrDev == 1 ? "개발용 버전" : "실행용 버전");
			else if(Tools.IsLangJapanese())
				buf = String.Format("{0}", KeyLock.nKeyLockRunOrDev == 1 ? "開発用バージョン" : "実行用バージョン");
			else if(Tools.IsLangChinese())
				buf = String.Format("{0}", KeyLock.nKeyLockRunOrDev == 1 ? "开发用版本" : "运行用版本");
			else
				buf = String.Format("{0}", KeyLock.nKeyLockRunOrDev == 1 ? "Developer Version" : "Runtime Version");

            if (TotalConfig.eOemType == EnumOemType.SBAS)
               buf = "";
            
			this.labelRuntime.Text = buf;

			string oem = KeyLock.KeyLockGetOemString();

			if(oem.Length > 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					buf = String.Format("사용범위 : {0}", oem);
				}
				else if(Tools.IsLangChinese()) 
				{
					buf = String.Format("使用范围 : {0}", oem);
				}
				else 
				{
					buf = String.Format("Bundle : {0}", oem);
				}
				this.labelOemString.Text = buf;
			}
			else 
			{
				this.labelOemString.Text = oem;
			}

			if(KeyLock.bExistWebKey) 
			{
				if(Tools.IsLangKorean()) 
					buf = String.Format("웹서버 : {0} 사용자용", KeyLock.nWebUserCount);
				else if(Tools.IsLangJapanese())
					buf = String.Format("WEB サーバー : {0} ユーザー用", KeyLock.nWebUserCount);
				else if(Tools.IsLangChinese())
					buf = String.Format("Web服务器 : {0} 用户用", KeyLock.nWebUserCount);
				else 
					buf = String.Format("WebServer : {0} User", KeyLock.nWebUserCount);
				this.labelWebUser.Text = buf;
			}
			else 
			{
				this.labelWebUser.Text = "";
			}

            int version = KeyLock.GetVersion();
            if(version >= 7)    version += 2000;

			if(Tools.IsLangKorean())
                this.labelVersion.Text = String.Format("키락 버전 : {0}", version);
			else if(Tools.IsLangJapanese())
                this.labelVersion.Text = String.Format("Keylock バージョン : {0}", version);
			else if(Tools.IsLangChinese())
                this.labelVersion.Text = String.Format("Keylock 版本 : {0}", version);
			else
                this.labelVersion.Text = String.Format("Key Version : {0}", version);

			this.panelVersion.BackColor = KeyLock.GetColor();

            EnableDisableLicenseButton();
		}

		public static int KeyLockGetTotalTagUsed()
		{
			int size = 0;
			int i;

			TagPublicClass tp;
			for(i = 0; i < TagLib.tagListAll.Length; i++) 
			{
				tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);
				if(tp.act == 0)	continue;
				size++;
			}
			return size;
		}

		public static void InformationKeylock()
		{
			if(KeyLock.IsSoftLock()) 
			{
				FormInformationSoftLock dialog = new FormInformationSoftLock();
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ShowDialog(TotalConfig.formMain);
			}
			else 
			{
				FormInformationKeyLock dialog = new FormInformationKeyLock();
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ShowDialog(TotalConfig.formMain);
			}
		}

        // 최대한 라이센스 버튼을 가리는 것이 좋겠다.
        void EnableDisableLicenseButton()
        {
            string cpu = GetCpu();
            string mainboard = GetMainBoard();

            // 김경수의 개발 컴퓨터일 때는 라이센스 버튼을 항상 표시해 준다.
            if (String.Compare(TotalConfig.sCurrentComputer, "dol-PC", true) == 0 && cpu == "BFEBFBFF0001067A" && mainboard == "P43 Neo-F (MS-7519)")    // dol-pc
            {
                return;
            }

            // 키락이 있으면 라이센스를 가져올 필요가 없다.
            if (KeyLock.bExistLocalKey)
            {
                this.buttonGetLicense.Visible = false;
                return;
            }

            if (cpu == "BFEBFBFF00030679" && mainboard == "MS15A-U4")          // ATB 모델
            {
            }
            else if (cpu == "BFEBFBFF00030678" && mainboard == "MVBAYAI-SI")        // ATM,ATP,ABP 모델
            {
            }
            else
            {
                this.buttonGetLicense.Visible = false;
                return;
            }
        }

        string GetData(ManagementObject Source, string name)
        {
            object obj;

            try
            {
                obj = Source[name];
            }
            catch
            {
                obj = null;
            }

            string val;

            if (obj == null)
            {
                return "";
            }
            else if (obj.GetType() == typeof(uint))
            {
                val = String.Format("{0:X8}", obj);
            }
            else if (obj.GetType() == typeof(String[]))
            {
                val = "";
                String[] s = (String[])obj;
                for (int i = 0; i < s.Length; i++)
                {
                    val += s[i];
                }
            }
            else
            {
                val = obj.ToString();
            }

            return val;
        }

        string GetCpu()
        {
            string data = "";
            ManagementClass myManagementClass;
            ManagementObjectCollection moc;

            myManagementClass = new ManagementClass("Win32_processor");
            moc = myManagementClass.GetInstances();

            try
            {
                foreach (ManagementObject Source in moc)
                {
                    data = GetData(Source, "ProcessorID");
                    if (data.Length > 0) break;
                }
            }
            catch
            {
                // 베트남에서 foreach (ManagementObject Source in moc) 에서 Exception이 걸리는 경우가 있다. 2020-2-13
            }

            return data;
        }

        string GetMainBoard()
        {
            string data = "";
            ManagementClass myManagementClass;
            ManagementObjectCollection moc;

            myManagementClass = new ManagementClass("Win32_BaseBoard");
            moc = myManagementClass.GetInstances();

            data = "";

            try
            {
                foreach (ManagementObject Source in moc)
                {
                    data = GetData(Source, "Product");
                    if (data.Length > 0) break;
                }
            }
            catch
            {
                // 여기서는 걸리지 않았으나 위의 GetCPU에서 걸렸으므로 여기도 같이 추가한다.
            }

            return data;
        }

        string GetMacAddress()
        {
            string data = "";
            ManagementClass myManagementClass;
            ManagementObjectCollection moc;

            myManagementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            moc = myManagementClass.GetInstances();

            data = "";

            List<string> mac_address = new List<string>();

            try
            {
                foreach (ManagementObject Source in moc)
                {
                    string imsi = GetData(Source, "MACAddress");

                    if (imsi.Length > 0)
                    {
                        string item_Caption = GetData(Source, "Caption");           // 네트워크의 설명이 나온다. 앞에 [00007] 같은 숫자가 붙어서 나온다.
                        string item_Description = GetData(Source, "Description");   // 네트워크의 설명이 나온다.
                        string item_IPEnabled = GetData(Source, "IPEnabled");       // 케이블이 끊어지면 False로 나온다.
                        string item_DNSHostName = GetData(Source, "DNSHostName");   // dol-PC와 같은 정보가 나온다.
                        string item_DNSDomain = GetData(Source, "DNSDomain");       // local이 나오나 케이블이 끊어지면 나오지 않는다.
                        string item_DNSDomainSuffixSearchOrder = GetData(Source, "DNSDomainSuffixSearchOrder"); // local이 나오나 케이블이 끊어지면 나오지 않는다.
                        string item_ServiceName = GetData(Source, "ServiceName");   // Service의 이름이 나온다.

                        if (String.Compare(item_Description, 0, "Realtek", 0, 7, true) == 0 ||  // ATP
                            String.Compare(item_Description, 0, "Intel(R)", 0, 8, true) == 0 || // ATH
                            String.Compare(item_Description, 0, "Microsoft Hyper-V", 0, 17, true) == 0)    // Hyper-V 속의 네트워크 드라이버
                        {
                            // 중복해서 올라오므로 같은 것은 빼준다.
                            for (int i = 0; i < mac_address.Count; i++)
                            {
                                if (imsi == mac_address[i]) goto same_mac_already_registerd;
                            }

                            mac_address.Add(imsi);

                            if (data.Length == 0)
                                data = imsi;
                            else
                                data = data + "," + imsi;
                        }
                    }

                same_mac_already_registerd: ;

                }
            }
            catch
            {
                // 여기서는 걸리지 않았으나 위의 GetCPU에서 걸렸으므로 여기도 같이 추가한다.
            }

            return data;

            /*
            // 전체 맥어드레스도 보여준다. 혹시 찾지 못하는 경우 수동으로 넣을 수 있도록 하기 위해서
            data = "";
            mac_address.Clear();
            foreach (ManagementObject Source in moc)
            {
                string imsi = GetData(Source, "MACAddress");

                if (imsi.Length > 0)
                {
                    // 중복해서 올라오므로 같은 것은 빼준다.
                    for (int i = 0; i < mac_address.Count; i++)
                    {
                        if (imsi == mac_address[i]) goto same_mac_already_registerd2;
                    }

                    mac_address.Add(imsi);

                    if (data.Length == 0)
                        data = imsi;
                    else
                        data = data + "," + imsi;

                }

            same_mac_already_registerd2: ;

            }

            this.textBoxMacAll.Text = data;*/
        }

        // WORD 로 되어있는 버퍼를 문자열로 바꿔준다.
        string ConvertHexSerialNumberToString(string sn)
        {
            // 32534853574e4841333237363033204b20202020 -> S2SHNWAH236730K   (B141F482-Samsung SSD 750 EVO 250GB ATA Device)
            // 2020202057202d44435752413057323038373033 -> WD-WCARW0027830   (B141F482-WDC WD3200AAKS-00VYA0 ATA Device) 

            if ((sn.Length % 4) != 0) return sn;

            StringBuilder s = new StringBuilder();

            for (int i = 0; i < sn.Length; i += 4)
            {
                // 유효한 문자인가를 검사
                for (int j = 0; j < 4; j++)
                {
                    if (sn[i + j] >= '0' && sn[i + j] <= '9') continue;
                    if (sn[i + j] >= 'a' && sn[i + j] <= 'f') continue;
                    if (sn[i + j] >= 'A' && sn[i + j] <= 'F') continue;

                    return sn;  // hex 문자열이 아니다.
                }

                ushort us = HexBuf.ToWORD(sn, i);
                s.Append((char)((us >> 0) & 0xFF));
                s.Append((char)((us >> 8) & 0xFF));
            }

            return s.ToString().Trim();
        }

        string GetDisk(out string sn)
        {
            string data = "";
            sn = "";

            ManagementClass myManagementClass;
            ManagementObjectCollection moc;

            myManagementClass = new ManagementClass("Win32_DiskDrive");
            moc = myManagementClass.GetInstances();

            data = "";

            try
            {
                foreach (ManagementObject Source in moc)
                {
                    string deviceid = GetData(Source, "DeviceID");

                    if (deviceid.IndexOf("PHYSICALDRIVE0") != -1)
                    {
                        string Signature = GetData(Source, "Signature");

                        data = Signature + "-" + GetData(Source, "Model");

                        sn = GetData(Source, "SerialNumber");

                        sn = ConvertHexSerialNumberToString(sn);

                        break;
                    }

                }
            }
            catch
            {
                // 여기서는 걸리지 않았으나 위의 GetCPU에서 걸렸으므로 여기도 같이 추가한다.
            }

            return data;
        }

        static System.ServiceModel.EndpointAddress GetEndPoint()
        {
            string url = "http://www.order.autobase.biz/License/WebServiceKeyLock.asmx";

            System.ServiceModel.EndpointAddress ep = new System.ServiceModel.EndpointAddress(url);

            return ep;
        }

        static BasicHttpBinding GetBinding()
        {
            System.ServiceModel.BasicHttpBinding binding;

            binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;

            return binding;
        }

        byte[] MakeHash(byte[] org)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(org);

            return result;
        }

        static byte[] KEY = { 0x1e, 0xc3, 0x1c, 0x2d, 0x1b, 0xc3, 0x10, 0x53, 0x17, 0x2b, 0x1e, 0x20, 0x11, 0x23, 0x32, 0x43 };
        static byte[] IV = { 0x15, 0x63, 0x72, 0x83, 0x19, 0xa3, 0x1b, 0xc3, 0xd2, 0x2e, 0xf2, 0x20, 0x12, 0x22, 0x13, 0x24 };

        private void buttonGetLicense_Click(object sender, EventArgs e)
        {
            FormInputOemSerialNumber dialog = new FormInputOemSerialNumber();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            string sOemSn = dialog.Get();

            string filename = String.Format("C:\\AutoLock\\SCADA-{0}.key", sOemSn);

            if (File.Exists(filename))
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("같은 파일이 이미 존재합니다.\n기존 파일을 백업하고 삭제한 후 새로 받을 수 있습니다.", filename);
                }
                else
                {
                    MessageBox.Show("Same filename already exists.\nBackup and delete exist file.", filename);
                }

                return;
            }
            
            string sCpu = GetCpu();
            string sMainboard = GetMainBoard();
            string sMac = GetMacAddress();
            string sDiskSn;
            string sDisk = GetDisk(out sDiskSn);

            byte[] item0 = NetTools.Cryptography.CryptoAES.Encrypt(Encoding.UTF8.GetBytes(sOemSn), KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            byte[] item1 = NetTools.Cryptography.CryptoAES.Encrypt(Encoding.UTF8.GetBytes(sCpu), KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            byte[] item2 = NetTools.Cryptography.CryptoAES.Encrypt(Encoding.UTF8.GetBytes(sMainboard), KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            byte[] item3 = NetTools.Cryptography.CryptoAES.Encrypt(Encoding.UTF8.GetBytes(sMac), KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            byte[] item4 = NetTools.Cryptography.CryptoAES.Encrypt(Encoding.UTF8.GetBytes(sDisk), KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            byte[] item5 = NetTools.Cryptography.CryptoAES.Encrypt(Encoding.UTF8.GetBytes(sDiskSn), KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
            byte[] item6 = NetTools.Cryptography.CryptoAES.Encrypt(Encoding.UTF8.GetBytes(TotalConfig.sCurrentComputer), KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);

            LocalMain.ServiceReferenceKeyLock.ArrayOfBase64Binary args = new LocalMain.ServiceReferenceKeyLock.ArrayOfBase64Binary();

            args.Add(item0);
            args.Add(item1);
            args.Add(item2);
            args.Add(item3);
            args.Add(item4);
            args.Add(item5);
            args.Add(item6);

            int nTotalCount = 0;

            for (int i = 0; i < args.Count; i++)
                nTotalCount += args[i].Length;
            nTotalCount += 5;

            byte[] total = new byte[nTotalCount];

            int pos = 0;
            int length;

            for (int i = 0; i < args.Count; i++)
            {
                length = args[i].Length;
                Array.Copy(args[i], 0, total, pos, length);
                pos += length;
            }

            total[pos++] = (byte)'a';
            total[pos++] = (byte)'b';
            total[pos++] = (byte)'c';
            total[pos++] = (byte)'d';
            total[pos++] = (byte)'e';

            byte[] total_hash = MakeHash(total);

            string err_msg;

            try
            {
                ServiceReferenceKeyLock.WebServiceKeyLockSoapClient service = new LocalMain.ServiceReferenceKeyLock.WebServiceKeyLockSoapClient(GetBinding(), GetEndPoint());

                byte[] bin_enc = service.DownLoadKeyFile(args, total_hash, out err_msg);

                if (bin_enc != null)
                {
                    byte[] buffer = NetTools.Cryptography.CryptoAES.Decrypt(bin_enc, KEY, IV, System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
                    
                    filename = String.Format("C:\\AutoLock");
                    Directory.CreateDirectory(filename);
                    filename = String.Format("C:\\AutoLock\\SCADA-{0}.key", sOemSn);

                    if (File.Exists(filename))
                    {
                        if (Tools.IsLangKorean())
                        {
                            if (MessageBox.Show("같은 파일이 이미 존재합니다.\n덮어 쓸까요?", filename, MessageBoxButtons.YesNo) != DialogResult.Yes)
                                return;
                        }
                        else
                        {
                            if (MessageBox.Show("Same filename already exists.\nOverwrite?", filename, MessageBoxButtons.YesNo) != DialogResult.Yes)
                                return;
                        }
                    }

                    File.WriteAllBytes(filename, buffer);

                    // 사용자가 삭제하는 것을 방지하기 위해서 C:\\AutoBase\\AutoLock에도 같은 파일을 복사한다.  2013-6-21
                    filename = String.Format("C:\\AutoBase\\AutoLock");
                    Directory.CreateDirectory(filename);
                    filename = String.Format("C:\\AutoBase\\AutoLock\\SCADA-{0}.key", sOemSn);

                    if (File.Exists(filename))
                    {
                        if (Tools.IsLangKorean())
                        {
                            if (MessageBox.Show("같은 파일이 이미 존재합니다.\n덮어 쓸까요?", filename, MessageBoxButtons.YesNo) != DialogResult.Yes)
                                return;
                        }
                        else
                        {
                            if (MessageBox.Show("Same filename already exists.\nOverwrite?", filename, MessageBoxButtons.YesNo) != DialogResult.Yes)
                                return;
                        }
                    }

                    File.WriteAllBytes(filename, buffer);

                    if(Tools.IsLangKorean())
                        MessageBox.Show("키 파일 받기를 완료했습니다.\n프로그램을 재시작해야 새로운 키락이 적용됩니다.\n\n받은 파일명 = " + filename, "받기 완료");
                    else
                        MessageBox.Show("License download completed.\nYou must restart the program to use new keylock.\n\nFilename = " + filename, "OK");
                }
                else
                {
                    if(Tools.IsLangKorean())
                        MessageBox.Show(err_msg, "키 파일 받기 실패");
                    else
                        MessageBox.Show(err_msg, "License download failed.");
                }
            }
            catch
            {
                // 오류 메시지가 보이면 웹 주소가 보여서 좋지 않음
                if(Tools.IsLangKorean())
                    MessageBox.Show("서비스 연결 오류", "키 파일 받기 실패(Fail)");
                else
                    MessageBox.Show("Service connection error.", "License download failed");
            }

        }
	}
}

