using System;
using System.Collections.Generic;

using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.IO;
using NetTools;
using System.Collections;
using AutoLibLocal;
using GraphicModule;

namespace Studio
{
    public class WebLibraryUtil
    {
        public static void AddOneEntry(ZipOutputStream s, byte[] buffer, string name, DateTime t, FileAttributes attr)
        {
            //Crc32 crc = new Crc32();

            ZipEntry entry = new ZipEntry(name);

            entry.DateTime = t;
            entry.Size = buffer.Length;
            entry.ExternalFileAttributes = (int)attr;

            //crc.Reset();
            //crc.Update(buffer);

            //entry.Crc = crc.Value;

            s.PutNextEntry(entry);

            s.Write(buffer, 0, buffer.Length);
        }

        public static void SaveInformation(ZipOutputStream s, string keywords)
        {
            MemoryStream stream = new MemoryStream();
            CommaTextWriter writer = new CommaTextWriter(stream);

            writer.WriteLine("Keywords,{0}", keywords);

            writer.Close();

            byte[] buffer = stream.ToArray();

            string dir = "Info.txt";
            AddOneEntry(s, buffer, dir, DateTime.Now, FileAttributes.Archive);

            stream.Close();
        }

        public static void UpdateKeywords(string filename, string keywords)
        {
            byte[] s_buffer = File.ReadAllBytes(filename);
            HashBuffer(s_buffer);

            ZipInputStream z_s = new ZipInputStream(new MemoryStream(s_buffer));

            MemoryStream stream_t = new MemoryStream();
            ZipOutputStream z_t = new ZipOutputStream(stream_t);

            z_t.SetLevel(6);

            ZipEntry entry;

            while ((entry = z_s.GetNextEntry()) != null)
            {
                if (String.Compare(entry.Name, "info.txt", true) == 0) continue;

                byte[] buffer = new byte[entry.Size];
                byte[] data = new byte[2048];

                int size;

                int pos = 0;

                while (true)
                {
                    size = z_s.Read(data, 0, data.Length);  // 한번에 크기 읽으니까 뒷 부분이 읽혀지지 않는 현상 발생 그래서 2048로 잘라서 하니 잘됨
                    if (size == 0) break;
                    Array.Copy(data, 0, buffer, pos, size);
                    pos += size;
                }

                AddOneEntry(z_t, buffer, entry.Name, entry.DateTime, FileAttributes.Archive);// (FileAttributes)entry.ExternalFileAttributes);
            }

            SaveInformation(z_t, keywords);

            z_t.Finish();
            z_t.Close();

            z_s.Close();

            byte[] buffer_t = stream_t.ToArray();

            HashBuffer(buffer_t);
            File.WriteAllBytes(filename, buffer_t);
        }

        public static void SaveTagListPublic(ZipOutputStream s, ArrayList block)
        {
            TagGrClass gr = new TagGrClass();
            MULTI_SELECT_TAG_STRUCT list;
            TagPublicClass tp;
            int[] tagpos = new int[1];

            for (int i = 0; i < block.Count; i++)
            {
                list = (MULTI_SELECT_TAG_STRUCT)block[i];
                tp = (TagPublicClass)Tools.CopyObject(TagLib.GetStructPublic(list.tagSource, ref tagpos));
                tp.name = tp.tag;
                gr.AddTag(tp, true);
            }

            MemoryStream stream = new MemoryStream();
            TagFile file = new TagFile();
            file.SaveTag(stream, gr, false, false);

            byte[] buffer = stream.ToArray();

            string dir = "Group.tagx";
            WebLibraryUtil.AddOneEntry(s, buffer, dir, DateTime.Now, FileAttributes.Archive);

            stream.Close();
        }

        public static string GetKeywords(Stream stream)
        {
            MemoryStream sk = ObjectAnimation.RestoreFromZipStream(stream, "info.txt");

            if (sk == null) return "";

            TextReader reader = new StreamReader(sk);
            CommaTextReader comma = new CommaTextReader();
            string one_line;
            string command = "";
            string keywords = "";
            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                comma.Set(one_line);
                comma.GetString(ref command);

                if (command == "Keywords")
                {
                    comma.GetString(ref keywords);
                }
            }

            return keywords;
        }

        public static string GetKeywords(string filename)
        {
            byte[] buffer = File.ReadAllBytes(filename);
            HashBuffer(buffer);

            MemoryStream stream = new MemoryStream(buffer);
            //Stream stream = File.OpenRead(filename);

            string keywords = GetKeywords(stream);

            stream.Close();

            return keywords;
        }

        public static MemoryStream GetFile(string zipfile, string file)
        {
            byte[] buffer = File.ReadAllBytes(zipfile);
            HashBuffer(buffer);

            MemoryStream stream = new MemoryStream(buffer);
            MemoryStream ms = ObjectAnimation.RestoreFromZipStream(stream, file);
            stream.Close();

            return ms;
        }

        public static bool HashBuffer(byte[] buffer)
        {
            int[] rand_hash = new int[17] { 0xDB, 0x6D, 0x19, 0x54, 0x45, 0x36, 0x7B, 0x91, 0xF7, 0x19, 0xBC, 0xC2, 0x8A, 0x2E, 0xA3, 0x68, 0x7F };

            int size = buffer.Length;

            for (int i = 0; i < size; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ (i % 256) ^ rand_hash[i%17]);
            }

            return true;
        }

    }
}
