using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace PublicStudioLocalMain
{
    public class ClassPublic
    {
        public static void CallRemoteAS(string keylock_number, string keylock_version, string keylock_tag)
        {
            //string autobase_version = Request["AbVer"];
            //string keylock_number = Request["KeyNo"];
            //string keylock_version = Request["KeyVer"];
            //string keylock_tag = Request["KeyTag"];
            //string dotnet_version = Request["NetVer"];

            string autobase_version = Application.ProductVersion;;
            string dotnet_version = Environment.Version.ToString();

            string url = String.Format("http://www.autobase.biz/Korean/AS/RemoteAS.aspx");
            url += String.Format("?AbVer={0}", autobase_version);
            url += String.Format("&KeyNo={0}", keylock_number);
            url += String.Format("&KeyVer={0}", keylock_version);
            url += String.Format("&KeyTag={0}", keylock_tag);
            url += String.Format("&NetVer={0}", dotnet_version);

            System.Diagnostics.Process.Start(url); // 2022-4-12 변경.Edge와 IExplore가 두개가 뜬다는 이야기가 있어서... //System.Diagnostics.Process.Start("IExplore", url);

            /*
            FormRemoteAS form = new FormRemoteAS();

            form.Set(url);
            form.Show();
             */
        }
    }
}
