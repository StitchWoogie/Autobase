using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace NetTools
{
	/// <summary>
	/// Summary description for Tools.
	/// </summary>
	public class Tools 
	{
		public static ushort[]	WORD_MASK = {	0x0001, 0x0002, 0x0004, 0x0008,
												0x0010, 0x0020, 0x0040, 0x0080,
												0x0100, 0x0200, 0x0400, 0x0800,
												0x1000, 0x2000, 0x4000, 0x8000 };

		public static uint[] DWORD_MASK = {
		0x00000001, 0x00000002, 0x00000004, 0x00000008, 
		0x00000010, 0x00000020, 0x00000040, 0x00000080, 
		0x00000100, 0x00000200, 0x00000400, 0x00000800, 
		0x00001000, 0x00002000, 0x00004000, 0x00008000, 
		0x00010000, 0x00020000, 0x00040000, 0x00080000, 
		0x00100000, 0x00200000, 0x00400000, 0x00800000, 
		0x01000000, 0x02000000, 0x04000000, 0x08000000, 
		0x10000000, 0x20000000, 0x40000000, 0x80000000 };

		public Tools()
		{
			//
			// TODO: Add constructor logic here
			// 
		}

		public static void Temp(ref int temp1, ref int temp2)
		{
			int temp;
			temp = temp1;
			temp1 = temp2;
			temp2 = temp;
		}

        public static void Temp(ref double temp1, ref double temp2)
        {
            double temp;
            temp = temp1;
            temp1 = temp2;
            temp2 = temp;
        }

		public static string BytesToString(byte[] buf)
		{
			System.Text.Decoder d = System.Text.Encoding.Default.GetDecoder();
			char[] chars = new char[buf.Length];
			int retn = d.GetChars(buf, 0, buf.Length, chars, 0);

			string buf_t = "";
			for(int i = 0; i < retn; i++) 
			{
				if(chars[i] == 0)	break;	// 실제 문자열 길이와 배열의 길이는 다르다.
				buf_t += Char.ToString(chars[i]);
			}

			return buf_t;
		}

		public static byte[] StringToBytes(string buf)
		{
			System.Text.Encoder d = System.Text.Encoding.Default.GetEncoder();

			char[] chars = buf.ToCharArray(); 
			byte[] bytes = new byte[chars.Length*2];

			int retn = d.GetBytes(chars, 0, chars.Length, bytes, 0, true);

			byte[] result = new byte[retn];

			for(int i = 0; i < retn; i++) 
			{
				result[i] = bytes[i];
			}

			return result;
		}

		public static bool TextGetOneLine(FileStream fs, ref string buf_t)
		{
			int fc;
			int read_count = 0;
			System.Text.Decoder d = System.Text.Encoding.Default.GetDecoder();
			int retn;

			byte[] buf = new byte[10000];
			char[] chars = new char[10000];

			while( (fc = fs.ReadByte())  != -1) 
			{
				if(fc == 0x0d || fc == 0x0A) 
				{                     // 개행 문자
					if(fc == 0x0d)	fs.ReadByte();
					buf[read_count] = 0;

					// convert to unicode
					retn = d.GetChars(buf, 0, read_count, chars, 0);
					buf_t = "";
					for(int i = 0; i < retn; i++) 
					{
						buf_t += Char.ToString(chars[i]);
					}

					return true;
				}
				else 
				{   			//
					if(read_count < buf.Length-1)
						buf[read_count++] = (byte)fc;
				}
			}
			if(read_count == 0)	return false;

			buf[read_count] = 0;

			// convert to unicode
			retn = d.GetChars(buf, 0, read_count, chars, 0);
			buf_t = "";
			for(int i = 0; i < retn; i++) 
			{
				buf_t += Char.ToString(chars[i]);
			}

			return true;
		}

		public static void KillEndSpace(ref string buf)
		{
			int hap = buf.Length;
			int i;
			//string target="";
			int end_pos = buf.Length;

			for(i = hap-1; i >= 0; i--) 
			{
				if(buf[i] == 32 || buf[i] == '\t') 
				{
					end_pos = i;
					//buf[i] = 0;
					continue;
				}
				break;
			}

			buf = buf.Substring(0, end_pos);
		}
		

		/*
		static public void MakeDefaultLogFont(ref LOGFONT lf)
		{
			lf.lfHeight = -16;
			lf.lfFaceName = "굴림체";
			lf.style = System.Drawing.FontStyle.Regular;
		}
		*/

		static public int BmpWidthToByte(int width, int bits)
		{

			int imsi;

			if(bits == 1) 
			{
				imsi = (int)((((width+7)/8)+3)/4*4);
			}
			else if(bits == 4) 
			{
				imsi = (int)(( ((width+1)/2)+3)/4*4);
			}
			else if(bits == 8) 
			{
				imsi = ((width+3)/4)*4;
			}
			else if(bits == 16) 
			{
				imsi = ((width*2+3)/4)*4;
			}
			else if(bits == 32) 
			{
				imsi = width*4;
			}
			else 
			{
				imsi = (int)( (width*3+3)/4*4);
			}
			return imsi;
		}

		static public Color ConvertToColor(int org)
		{
			int r = (org >> 16) & 0xFF;
			int g = (org >> 8) & 0xFF;
			int b = (org >> 0) & 0xFF;

			return Color.FromArgb(r, g, b);
		}

		public static string GetWindowsRootDirectory()
		{
			string system = Environment.SystemDirectory;

			for(int i = system.Length-1; i > 0; i--) 
			{
				if(system[i] == '\\' || system[i] == '/') 
				{
					return system.Substring(0, i);
				}
			}

			return "C:\\WINNT";


		}

        /// <summary>
        /// .NET 4.5 이상에서는 CultureInfo.DefaultThreadCurrentCulture 사용.
		/// Thread.CurrentThread.CurrentUICulture 는 앱 시작 시 초기화되어 변경 불가.
        /// </summary>
        public static bool IsLangKorean()
		{
			//CultureInfo info = Thread.CurrentThread.CurrentUICulture;

			CultureInfo info = CultureInfo.DefaultThreadCurrentUICulture; //251103 PSU 수정 

			if(String.Compare(info.Name, "ko", true) == 0)
				return true;
			if(String.Compare(info.Name, "ko-kr", true) == 0)
				return true;

			return false;
		}

		public static bool IsLangJapanese()
		{
            //CultureInfo info = Thread.CurrentThread.CurrentUICulture;
            CultureInfo info = CultureInfo.DefaultThreadCurrentUICulture; //251103 PSU 수정 

            if (String.Compare(info.Name, "ja", true) == 0)
				return true;
			if(String.Compare(info.Name, "ja-jp", true) == 0)
				return true;

			return false;
		}

		public static bool IsLangChinese()
		{
            //CultureInfo info = Thread.CurrentThread.CurrentUICulture;
            CultureInfo info = CultureInfo.DefaultThreadCurrentUICulture; //251103 PSU 수정 

            if (String.Compare(info.Name, 0, "zh-", 0, 3, true) == 0)
				return true;

			return false;
		}

        public static bool IsLangVietnamese()
        {
            //CultureInfo info = Thread.CurrentThread.CurrentUICulture;
            CultureInfo info = CultureInfo.DefaultThreadCurrentUICulture; //251103 PSU 수정 

            if (String.Compare(info.Name, "vi", true) == 0)
                return true;
            if (String.Compare(info.Name, "vi-VN", true) == 0)
                return true;

            return false;
        }

        public static bool IsLangRussian()
        {
            //CultureInfo info = Thread.CurrentThread.CurrentUICulture;
            CultureInfo info = CultureInfo.DefaultThreadCurrentUICulture; //251103 PSU 수정 

            if (String.Compare(info.Name, "ru", true) == 0)
                return true;
            if (String.Compare(info.Name, "ru-RU", true) == 0)
                return true;

            return false;
        }

		public static bool CompareFile(string source, string target)
		{
			if(!File.Exists(source))	return false;
			if(!File.Exists(target))	return false;

			FileInfo fi1 = new FileInfo(source);
			FileInfo fi2 = new FileInfo(target);
			if(fi1.Length != fi2.Length)	return false;

			FileStream f1 = File.OpenRead(source);
			if(f1 == null)	return false;
			FileStream f2 = File.OpenRead(target);
			if(f2 == null)	
			{
				f1.Close();
				return false;
			}

			int ch1, ch2;

			while(true) 
			{
				ch1 = f1.ReadByte();
				ch2 = f2.ReadByte();

				if(ch1 == -1 || ch2 == -1)	break;
				if(ch1 != ch2) 
				{
					f1.Close();
					f2.Close();
					return false;
				}
			}

			f1.Close();
			f2.Close();
			return true;
		}

        public static bool CompareFile(byte[] source, string target)
        {
            //if (!File.Exists(source)) return false;
            if (!File.Exists(target)) return false;

            //FileInfo fi1 = new FileInfo(source);
            FileInfo fi2 = new FileInfo(target);
            if (source.Length != fi2.Length) return false;

            //FileStream f1 = File.OpenRead(source);
            //if (f1 == null) return false;

            FileStream f2 = File.OpenRead(target);
            if (f2 == null)
            {
                //f1.Close();
                return false;
            }

            int ch1, ch2;

            for (int i = 0; i < source.Length; i++)
            {
                ch1 = source[i];
                ch2 = f2.ReadByte();

                if (ch2 == -1) break;
                if (ch1 != ch2)
                {
                    //f1.Close();
                    f2.Close();
                    return false;
                }
            }

            //f1.Close();
            f2.Close();
            return true;
        }

		public static object CopyObject(object source)
		{
			MemoryStream s = new MemoryStream();

			BinaryFormatter fomat = new BinaryFormatter();
			fomat.Serialize(s, source);
			s.Seek(0, SeekOrigin.Begin);
			object target = fomat.Deserialize(s);

			return target;
		}

		public static long getfilesize(string filename)
		{
			if(!File.Exists(filename))	return 0;

			FileInfo info = new FileInfo(filename);
			return info.Length;
		}

		public static int DeleteDirAndFile(string root_path)
		{
			if(!Directory.Exists(root_path))	return 0;

			DirectoryInfo info = new DirectoryInfo(root_path);

			int count = 0;

			foreach(DirectoryInfo di in info.GetDirectories("*.*")) 
			{
				count += DeleteDirAndFile(di.FullName);
			}

			foreach(FileInfo fi in info.GetFiles("*.*")) 
			{
				//FileAttributes attr = fi.Attributes;
				//fi.Attributes = (attr | FileAttributes.Archive);
				File.Delete(fi.FullName);
				count++;
			}

			return count;
		}

		public delegate int BlockSortFunction(object a, object b);

		public static void BlockSort(ArrayList block, BlockSortFunction function)
		{
			// sort
			int retn;
			int pos;
			int l;
			int m;
			object obj1;
			object obj2;

			for(l = 0; l < block.Count; l++) 
			{
				obj1 = block[l];
				pos = l;
				for(m = l+1; m < block.Count; m++) 
				{
					obj2 = block[m];
					retn = function(obj1, obj2);
					if(retn < 0) 
					{
						pos = m;
						obj1 = obj2;
					}
				}

				if(pos != l) 
				{
					object temp;
					temp = block[l];
					block[l] = block[pos];
					block[pos] = temp;
				}
			}	
		}

		//------------------------------------------------------------------------------
		//	파일의 개수를 알아본다.
		//------------------------------------------------------------------------------

		public static int GetFileHap(string filename)
		{
			string directory;
			string name;

			directory = Path.GetDirectoryName(filename);
			name = Path.GetFileName(filename);

			if(!Directory.Exists(directory))	return 0;

			DirectoryInfo info = new DirectoryInfo(directory);

			int count = 0;

			count = info.GetFiles(name).Length;
			//foreach(FileInfo fi in info.GetFiles(name)) 
			//{
			//	count++;
			//}

			return count;
		}


		public static void SetNumericUpDownValue(System.Windows.Forms.NumericUpDown numeric, decimal value)
		{
			if(value < numeric.Minimum)			numeric.Value = numeric.Minimum;
			else if(value > numeric.Maximum)	numeric.Value = numeric.Maximum;
			else								numeric.Value = value;

			return;
		}


		/// <summary>
		/// 주어진 파일을 텍스트 파일이라고 가정하고 줄수를 얻는다.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static int GetLineHap(string filename)
		{
			if(!File.Exists(filename))	return 0;

			TextReader reader = new StreamReader(filename);
			if(reader == null)	return 0;
			int count = 0;
			while(true) 
			{
				if(reader.ReadLine() == null)	break;
				count++;
			}
			reader.Close();

			return count;
		}

		

		public static void bell()
		{
			//Win32Function.MessageBeep(0xFFFFFFFF);
			//Win32Function.MessageBeep(0);
			Win32Function.Beep(500, 100);
		}

		public static Process GetPreviousProcess()
		{
            // studio에서 실행하면 .vhost가 붙는다. 이 부분을 잘라내야 테스트가 가능하다.
            string process_name = Process.GetCurrentProcess().ProcessName;

            int index = process_name.IndexOf(".vshost");

            if(index != -1) {
                process_name = process_name.Substring(0, index);
            }

            string name;

			foreach(Process p in Process.GetProcesses())
			{
                if (p.Id == Process.GetCurrentProcess().Id) continue;

                name = p.ProcessName;
                index = name.IndexOf(".vshost");

                if(index != -1) {
                    name = name.Substring(0, index);
                }

				if(String.Compare(process_name, name, true) != 0)	continue; 

				return p;
			}

			return null;
		}

        static char[] filename_unable_char = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

        /// <summary>
        /// 파일명만 검사한다. 전체경로명이 아니다.
        /// </summary>
        /// <param name="name">파일명만 (전체경로명이 아니다.)</param>
        /// <returns></returns>
        public static bool IsAbleFilename(string name)
        {
            if (name.IndexOfAny(filename_unable_char) != -1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 파일명만 변경. 전체경로명이 아니다.
        /// </summary>
        /// <param name="name">파일명만 (전체 경로명이 아니다.)</param>
        /// <param name="change">변경할 문자</param>
        /// <returns></returns>
        public static string ConvertToAbleFilename(string name, char change)
        {
            char[] char_unable = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

            if (!IsAbleFilename(name))
            {
                string result = "";
                for (int i = 0; i < name.Length; i++)
                {
                    for (int j = 0; j < filename_unable_char.Length; j++)
                    {
                        if (name[i] == filename_unable_char[j])
                        {
                            result += change;
                            goto next;
                        }
                    }

                    result += name[i];

                next: ;

                }

                return result.Trim();
            }

            return name;
        }

	}
}

