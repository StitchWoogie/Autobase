using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NetTools;
using System.Threading;

namespace HelpLib
{
    public partial class FormInstallHelp : Form
    {
        public FormInstallHelp()
        {
            InitializeComponent();
        }

        string sLanguage = ClassHelp.GetFitLanguageName();

        private void FormInstallHelp_Load(object sender, EventArgs e)
        {
            this.Text = String.Format("{0} Help Updating...", sLanguage);

            if (thread != null)
            {
                bEnd = true;
                thread.Join(5000);
            }

            this.timer1.Enabled = true;
            bDone = false;

            thread = new Thread(new ThreadStart(ThreadLoop));
            thread.Start();
        }

        public HelpLib.ServiceReferenceHelp.ArrayOfString arrayToUpdate;

        static ServiceReferenceHelp.WebServiceHelpSoapClient GetService()
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.ReaderQuotas.MaxArrayLength = 2147483647;

            System.ServiceModel.EndpointAddress endpoint = new System.ServiceModel.EndpointAddress("http://file.autobase.biz/Service/WebServiceHelp.asmx");

            ServiceReferenceHelp.WebServiceHelpSoapClient service = new HelpLib.ServiceReferenceHelp.WebServiceHelpSoapClient(binding, endpoint);

            return service;
        }

        bool bDone = false;

        Thread thread = null;

        bool bEnd = false;

        void ThreadLoop()
        {
            bEnd = false;
            bDone = false;

            CommaBlockString comma = new CommaBlockString();
            string filename = "";
            long filesize = 0;
            DateTime filetime;

            for (int i = 0; i < arrayToUpdate.Count; i++)
            {
                if (bEnd) break;

                comma.Set(arrayToUpdate[i]);
                comma.GetString(ref filename);
                comma.GetLong(ref filesize);
                filetime = comma.GetDateTime();

                sCurrentFile = filename;
                DownLoadOneFile(filename, filetime, filesize);
                nPercentTotal = (i+1)*100/arrayToUpdate.Count;
            }

            bDone = true;
        }

        void DownLoadOneFile(string file, DateTime filetime, long filesize)
        {
            string dir = ClassHelp.GetHelpFolder(sLanguage);
            string tempfile = String.Format("{0}\\{1}.temp", dir, file);
            string filename = String.Format("{0}\\{1}", dir, file);

            if (File.Exists(filename))
            {
                FileInfo fi = new FileInfo(filename);

                if (filesize == fi.Length && filetime == fi.LastWriteTimeUtc)
                {
                    return;
                }
            }
            
            long pos = 0;
            ServiceReferenceHelp.WebServiceHelpSoapClient service = GetService();
            string err_msg;
            int size = 0;

            Directory.CreateDirectory(dir);
            FileStream writer = File.OpenWrite(tempfile);
            byte[] buffer;
            nPercentFile = 0;

            while (true)
            {
                if (bEnd) break;
                
                Thread.Sleep(1);

                size = 500000; // 1M단위로 한다. 500K로 바꾸었다. 5M 단위로 해도 500K와 시간이 같다. 1M도 마찬가지. 그래서 %갱신이 빠르도록 500K로 했다.

                if (pos + size > filesize)
                {
                    size = (int)(filesize - pos);
                }

                if (size == 0) break;  // 다 읽었다.

                buffer = service.DownLoadFile(sLanguage, file, pos, size, out err_msg);
                if (buffer == null)
                {
                    writer.Close();
                    return;
                }

                if (buffer.Length != size)
                {
                    writer.Close();
                    return;
                }

                writer.Write(buffer, 0, size);

                pos += size;

                nPercentFile = (int)(pos * 100 / filesize);
            }

            writer.Close();

            if (bEnd) return;   // 중간에 종료하는 경우는 그냥 돌아간다.

            try
            {
                if (File.Exists(filename)) File.Delete(filename);
                File.Move(tempfile, filename);
                File.SetLastWriteTimeUtc(filename, filetime);
            }
            catch
            {

            }
        }

        int nPercentFile = 0;
        int nPercentTotal = 0;
        string sCurrentFile = "";

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (nPercentFile != progressBarFile.Value)
            {
                this.labelFile.Text = String.Format("{0}/{1} : {2}%", sLanguage, sCurrentFile, nPercentFile);
                progressBarFile.Value = nPercentFile;
            }
            if (nPercentTotal != progressBarTotal.Value)
            {
                this.labelTotal.Text = String.Format("Total : {0}%", nPercentTotal);
                progressBarTotal.Value = nPercentTotal;
            }

            if(bDone) {
                this.timer1.Enabled = false;
                if(Tools.IsLangKorean())
                    MessageBox.Show("업데이트를 완료했습니다.", "업데이트 완료");
                else
                    MessageBox.Show("O.K Update completed.", "Update completed.");

                DialogResult = DialogResult.OK;
                Close();
                return;
            }
        }

        private void FormInstallHelp_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;

            if (thread != null)
            {
                bEnd = true;
                thread.Join(5000);
                thread = null;
            }
        }

        public static bool GoUpdate()
        {
            FormInstallHelp dialog = new FormInstallHelp();

            string sLanguage = ClassHelp.GetFitLanguageName();

            ServiceReferenceHelp.WebServiceHelpSoapClient service = GetService();

            HelpLib.ServiceReferenceHelp.ArrayOfString files = new HelpLib.ServiceReferenceHelp.ArrayOfString();

            try
            {
                string dir = ClassHelp.GetHelpFolder(sLanguage);

                if (Directory.Exists(dir))
                {
                    DirectoryInfo di = new DirectoryInfo(dir);
                    foreach (FileInfo fi in di.GetFiles("*.chm"))
                    {
                        string buf = String.Format("{0},{1},{2}-{3}-{4} {5}:{6}:{7}", fi.Name, fi.Length,
                                    fi.LastWriteTimeUtc.Year, fi.LastWriteTimeUtc.Month, fi.LastWriteTimeUtc.Day,
                                    fi.LastWriteTimeUtc.Hour, fi.LastWriteTimeUtc.Minute, fi.LastWriteTimeUtc.Second);

                        files.Add(buf);
                    }
                }

                dialog.arrayToUpdate = service.CheckHelpLists(sLanguage, files);
            }
            catch (Exception exception)
            {
                string msg;
                if(Tools.IsLangKorean())
                    msg = String.Format("인터넷이 연결되지 않았거나 사이트에 오류가 있습니다.\nMessage={0}", exception.Message);
                else
                    msg = String.Format("The Internet is disconnected or Help Site errors.\nMessage={0}", exception.Message);

                MessageBox.Show(msg, "Error");
                
                return false;
            }

            if (dialog.arrayToUpdate.Count == 0)
            {
                string msg;

                if (Tools.IsLangKorean())
                {
                    msg = String.Format("업데이트할 도움말이 없습니다.\n현재 도움말이 최신 버전입니다.");
                }
                else
                {
                    msg = String.Format("There is no Help file to update.\nCurrent file is latest version.");
                }
                MessageBox.Show(msg, "O.K ("+sLanguage+")");
                return true;
            }

            if (dialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) return true;

            return false;
        }
    }
}

