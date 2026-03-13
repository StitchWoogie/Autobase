using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using NetTools;
using System.IO;
using AutoLibLocal;

namespace Studio
{
    public class WebLibraryGate
    {
        public string sMainTable;

        public WebLibraryGate(string table)
        {
            sMainTable = table;
        }

        public static bool bLogIn = false;
        public const string sUrlLibrarySite = "http://www.library.autobase.biz";

        public static string GetUrlLibrarySiteTitle()
        {
            if (Tools.IsLangKorean())
            {
                return "라이브러리 사이트에 회원 가입";
            }
            else if (Tools.IsLangVietnamese())
            {
                return "Tạo tài khoản trên trang web thư viện.";
            }
            else
            {
                return "Create Account on Library Site";
            }
        }

        public static bool LogIn()
        {
            if (bLogIn) return true;

            bool use_url = true;

            if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
                use_url = false;

            // Username 사이트를 검사
            if (!UsernameLibrary.FormLogIn.LogInAuto(sUrlLibrarySite, GetUrlLibrarySiteTitle(), use_url)) return false;
            
            // Library 사이트를 검사
            if (!CheckLibrarySiteUser(UsernameLibrary.Config.sUserName))
            {
                return false;
            }

            bLogIn = true;

            return bLogIn;
        }

        public static void LogOut()
        {
            bLogIn = false;
        }

        public static string sUsername;
        static int nConnectionID;

        public static bool CheckLibrarySiteUser(string un)
        {
            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string passcode = Hash(un);

            int connid;

            try
            {
                connid = service.CheckUsername(un, passcode);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "로그인 오류");
                return false;
            }

            if (connid == -1)
            {
                MessageBox.Show("Username이나 Password가 틀립니다.", "로그인 오류");
                return false;
            }
            if (connid == -2)
            {
                MessageBox.Show("허용된 IP가 아닙니다.", "로그인 오류");
                return false;
            }
            if (connid == -3)
            {
                MessageBox.Show("Hash가 틀립니다.", "로그인 오류");
                return false;
            }

            sUsername = un;
            nConnectionID = connid;

            return true;

        }

        static byte[] StringToBytes(string buf)
        {
            byte[] b = new byte[buf.Length * 2];

            for (int i = 0; i < buf.Length; i++)
            {
                b[i * 2 + 0] = (byte)(buf[i] / 256);
                b[i * 2 + 1] = (byte)(buf[i] % 256);
            }

            return b;
        }

        static string BytesToString(byte[] buf)
        {
            string s = "";

            for (int i = 0; i < buf.Length; i++)
            {
                s += String.Format("{0:X02}", buf[i]);
            }

            return s;
        }

        static string Hash(string org)
        {
            byte[] b = StringToBytes(org + "AutoBase Library Hash 1");

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(b);

            return BytesToString(result);
        }

        static string Hash2(string org)
        {
            byte[] b = StringToBytes(org + "AutoBase Library Hash 2");

            SHA1 sha = new SHA1Managed();
            byte[] result = sha.ComputeHash(b);

            return BytesToString(result);
        }


        public string PersonalGetThemeList()
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString()+sMainTable);

            string result = service.PersonalGetThemeList(hash, sUsername, nConnectionID, sMainTable, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : GetPersonalThemeList");
                return null;
            }

            return result;
        }

        public string PersonalGetGroupList(string theme_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString()+sMainTable+theme_name);

            string result = service.PersonalGetGroupList(hash, sUsername, nConnectionID, sMainTable, theme_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : GetPersonalGroupList");
                return null;
            }

            return result;
        }

        public string PersonalGetItemList(string theme_name, string group_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable+theme_name+group_name);

            string result = service.PersonalGetItemList(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : GetPersonalItemList");
                return null;
            }

            return result;
        }

        public bool? PersonalIsItemExists(string theme_name, string group_name, string item_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name + group_name + item_name);

            bool? result = service.PersonalIsItemExists(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, item_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : IsItemExists");
                return null;
            }

            return result;
        }

        public bool? PersonalAddOneItem(string theme_name, string group_name, string item_name, byte[] buffer, string keyword, bool web_share, int price, string comment)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable+ theme_name+ group_name + item_name);

            bool? result;

            try
            {
                result = service.PersonalAddOneItem2(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, item_name, buffer, keyword, web_share, price, comment, out err_msg);
            }
            catch (Exception exception)
            {
                result = null;
                err_msg = exception.Message;
            }

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : AddOneItem");
                return null;
            }

            return result;
        }

        public byte[] PersonalGetOneItem(string theme_name, string group_name, string item_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + group_name);

            byte[] result = service.PersonalGetOneFile(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, item_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalGetOneItem");
                return null;
            }

            return result;
        }

        public byte[] PersonalGetOneItem2(string theme_name, string group_name, string item_name, out int price, out int download, out DateTime tCreate, out bool web_share)
        {
            price = 0;
            download = 0;
            web_share = false;
            tCreate = new DateTime();

            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + group_name);

            byte[] result = service.PersonalGetOneFile2(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, item_name, out price, out download, out tCreate, out web_share, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalGetOneItem2");
                return null;
            }

            return result;
        }

        public bool? PersonalCreateOneTheme(string theme_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name);

            bool? result = service.PersonalCreateOneTheme(hash, sUsername, nConnectionID, sMainTable, theme_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalCreateOneTheme");
                return null;
            }

            return result;
        }

        public bool? PersonalChangeOneTheme(string theme_name, string change_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name);

            bool? result = service.PersonalChangeOneTheme(hash, sUsername, nConnectionID, sMainTable, theme_name, change_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalChangeOneTheme");
                return null;
            }

            return result;
        }

        public bool? PersonalChangeOneGroup(string theme_name, string group_name, string change_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name+group_name);

            bool? result = service.PersonalChangeOneGroup(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, change_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalChangeOneGroup");
                return null;
            }

            return result;
        }

        public bool? PersonalChangeOneItem(string theme_name, string group_name, string item_name, string change_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name + group_name+item_name);

            bool? result = service.PersonalChangeOneItem(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, item_name, change_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalChangeOneItem");
                return null;
            }

            return result;
        }

        public bool PersonalGetOneItemProperty(string theme_name, string group_name, string item_name, out string keywords, out int price, out bool web_share, out string comment)
        {
            price = 0;
            web_share = false;
            comment = "";
            keywords = "";

            if (!LogIn()) return false;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name + group_name + item_name);

            bool? result = service.PersonalGetOneItemProperty2(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, item_name, out keywords, out price, out web_share, out comment, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalGetOneItemProperty");
                return false;
            }

            return true;
        }

        public bool PersonalSetOneItemProperty(string theme_name, string group_name, string item_name, string keywords, int price, bool web_share, string comment)
        {
            if (!LogIn()) return false;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name + group_name + item_name);

            bool? result = service.PersonalSetOneItemProperty2(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, item_name, keywords, price, web_share, comment, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalSetOneItemProperty");
                return false;
            }

            return true;
        }

        public bool? PersonalDeleteOneTheme(string theme_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name);

            bool? result = service.PersonalDeleteOneTheme(hash, sUsername, nConnectionID, sMainTable, theme_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalDeleteOneTheme");
                return null;
            }

            return result;
        }

        public bool? PersonalDeleteOneGroup(string theme_name, string group_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name+group_name);

            bool? result = service.PersonalDeleteOneGroup(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalDeleteOneGroup");
                return null;
            }

            return result;
        }

        public bool? PersonalDeleteOneItem(string theme_name, string group_name, string item_name)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name + group_name + item_name);

            bool? result = service.PersonalDeleteOneItem(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, item_name, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalDeleteOneItem");
                return null;
            }

            return result;
        }

        public bool? PersonalRenameKeywords(string theme_name, string group_name, string item_name, string keywords)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + sMainTable + theme_name + group_name + item_name+keywords);

            bool? result = service.PersonalRenameKeywords(hash, sUsername, nConnectionID, sMainTable, theme_name, group_name, item_name, keywords, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : PersonalRenameOneItem");
                return null;
            }

            return result;
        }

        public string SearchGetKeywordList()
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString());

            string result = service.SearchGetKeywordList(hash, sUsername, nConnectionID, sMainTable, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : SearchGetKeywordList");
                return null;
            }

            return result;
        }

        public string SearchGetItemIds(string keyword, int sorttype)
        {
            if (!LogIn()) return null;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString() + keyword);

            string result = service.SearchGetItemIdsWithUpdateTime(hash, sUsername, nConnectionID, sMainTable, keyword, sorttype, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : SearchGetItemIds");
                return null;
            }

            return result;
        }

        byte[] LoadCache(string name, DateTime tUpdate, out string info)
        {
            info = "";

            if (!FormConfigLibrary.bUseWebLibraryCache) return null;

            long sub_pos = ConvertTool.ToInt64(name)%100;  // 폴더를 나누면 속도가 개선될 듯  

            string temp_file = String.Format("{0}AutoBase\\LibCache\\{1}\\SUB{2}\\{3}", Path.GetTempPath(), sMainTable, sub_pos, name);

            if (!File.Exists(temp_file))
            {
                return null;
            }

            if (tUpdate != File.GetLastWriteTime(temp_file))
            {
                return null;
            }

            byte[] buffer;

            buffer = File.ReadAllBytes(temp_file);
            WebLibraryUtil.HashBuffer(buffer);

            ushort data_crc = NetTools.GetCRC.SumWORD(buffer, 0, buffer.Length - 2);
            ushort crc = (ushort)(buffer[buffer.Length - 2] + buffer[buffer.Length - 1] * 256);

            if (crc != data_crc) return null;   // crc mismatched

            MemoryStream ms = new MemoryStream(buffer);

            BinaryReader br = new BinaryReader(ms);
            int total_size = br.ReadInt32();

            if (total_size != buffer.Length)    // total size mismatched
            {
                br.Close();
                return null;
            }

            int info_size = br.ReadInt32();
            char[] cinfo = new char[info_size / 2];
            for (int i = 0; i < cinfo.Length; i++)
            {
                cinfo[i] = (char)br.ReadInt16();
            }
            int data_size = br.ReadInt32();
            byte[] data = br.ReadBytes(data_size);
            
            br.Close();

            info = new String(cinfo);

            WebLibraryUtil.HashBuffer(data);

            return data;
        }

        void SaveCache(string name, DateTime tUpdate, byte[] data, string info)
        {
            if (!FormConfigLibrary.bUseWebLibraryCache) return;

            long sub_pos = ConvertTool.ToInt64(name) % 100;    // 폴더를 나누면 속도가 개선될 듯  

            string temp_dir = String.Format("{0}AutoBase\\LibCache\\{1}\\SUB{2}", Path.GetTempPath(), sMainTable, sub_pos);
            string temp_file = String.Format("{0}AutoBase\\LibCache\\{1}\\SUB{2}\\{3}", Path.GetTempPath(), sMainTable, sub_pos, name);

            Directory.CreateDirectory(temp_dir);

            byte[] imsi = new byte[data.Length];

            data.CopyTo(imsi, 0);

            WebLibraryUtil.HashBuffer(imsi);

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            int info_size = info.Length*2;
            int data_size = data.Length;
            int total_size = 4+4+info_size+4+data_size+2;   // 2 = data만 SUM CRC

            bw.Write(total_size);
            bw.Write(info_size);

            char[] cinfo = info.ToCharArray();
            for (int i = 0; i < cinfo.Length; i++)
            {
                bw.Write((short)cinfo[i]);
            }

            bw.Write(data_size);
            bw.Write(imsi);
            bw.Write((ushort)0);    // 일단 CRC로 0을 저장

            bw.Flush();
            bw.Close();

            byte[] buffer = ms.ToArray();

            ushort crc = NetTools.GetCRC.SumWORD(buffer, 0, buffer.Length - 2);
            buffer[buffer.Length - 2] = (byte)(crc % 256);
            buffer[buffer.Length - 1] = (byte)(crc / 256);

            WebLibraryUtil.HashBuffer(buffer);

            File.WriteAllBytes(temp_file, buffer);
            File.SetLastWriteTime(temp_file, tUpdate);
        }

        public byte[] SearchGetOneItem(string id, DateTime tUpdate, out int price, out int download, out string username, out DateTime tCreate, out string itemname, out float score, out string comment)
        {
            price = 0;
            download = 0;
            username = "";
            tCreate = new DateTime();
            itemname = "";
            score = 0;
            comment = "";

            if (!LogIn()) return null;

            byte[] result;
            string info;

            result = LoadCache(id, tUpdate, out info);
            if (result == null)
            {
                biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

                string err_msg;
                string hash = Hash(sUsername + nConnectionID.ToString() + id);

                result = service.SearchGetOneFile(hash, sUsername, nConnectionID, sMainTable, id, out err_msg, out info);

                if (result == null)
                {
                    MessageBox.Show(err_msg, "WebLibraryGate Error : SearchGetOneItem");
                    return null;
                }

                SaveCache(id, tUpdate, result, info);
            }

            NetTools.CommaTextReader comma = new NetTools.CommaTextReader();
            comma.Set(info);
            string command = "";
            while (true)
            {
                if (comma.IsEOS()) break;
                comma.GetString(ref command);
                if (String.Compare(command, 0, "Price=", 0, 6, true) == 0)
                {
                    price = NetTools.ConvertTool.ToInt32(command.Substring(6));
                }
                else if (String.Compare(command, 0, "DownLoad=", 0, 9, true) == 0)
                {
                    download = NetTools.ConvertTool.ToInt32(command.Substring(9));
                }

                else if (String.Compare(command, 0, "Username=", 0, 9, true) == 0)
                {
                    //username = command.Substring(9);
                }
                else if (String.Compare(command, 0, "Nickname=", 0, 9, true) == 0)
                {
                    username = command.Substring(9);
                }
                else if (String.Compare(command, 0, "Create=", 0, 7, true) == 0)
                {
                    try
                    {
                        tCreate = NetTools.ConvertTool.ToDateTime(command.Substring(7));
                    }
                    catch
                    {
                        
                    }
                }
                else if (String.Compare(command, 0, "itemname=", 0, 9, true) == 0)
                {
                    itemname = command.Substring(9);
                }
                else if (String.Compare(command, 0, "Score=", 0, 6, true) == 0)
                {
                    score = ConvertTool.ToSingle(command.Substring(6));
                }
                else if (String.Compare(command, 0, "Comment=", 0, 8, true) == 0)
                {
                    comment = command.Substring(8);
                }
                else
                {
                }
            }

            return result;
        }

        public bool CheckCash(long itemid)
        {
            if (!LogIn()) return false;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString());

            bool? result = service.CheckCash(hash, sUsername, nConnectionID, sMainTable, itemid, out err_msg);

            if (result == null)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : CheckCash");
                return false;
            }

            if (result == false)
            {
                MessageBox.Show("포인트가 부족합니다.", "포인트 부족");
            }

            return (bool)result;
        }

        public double GetRemainCash()
        {
            if (!LogIn()) return 0;

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string err_msg;
            string hash = Hash(sUsername + nConnectionID.ToString());
            double cash;

            bool result = service.GetRemainCash(hash, sUsername, nConnectionID, out cash, out err_msg);

            if (result == false)
            {
                MessageBox.Show(err_msg, "WebLibraryGate Error : GetRemainCash");
                return 0;
            }

            return cash;
        }

        public bool? IsFreeItem(long itemid, out string err_msg)
        {
            err_msg = "";
            if (!LogIn())
            {
                err_msg = "로그인 하지 못했습니다.";
                return null;
            }

            biz.autobase.library.www.WebServiceLibrary service = new biz.autobase.library.www.WebServiceLibrary();

            string hash = Hash(sUsername + nConnectionID.ToString());

            bool? result = service.IsFreeItem(hash, sUsername, nConnectionID, sMainTable, itemid, out err_msg);

            return result;
        }

        public string GetHashItemId(string itemid)
        {
            return Hash2(sUsername+sMainTable+itemid);
        }
    }
}
