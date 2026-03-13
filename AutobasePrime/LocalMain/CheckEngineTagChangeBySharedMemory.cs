using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using AutoLibLocal;
using NetTools;
using System.IO;

namespace LocalMain
{
    // 공유 메모리를 통한 태그값 공유
    class CheckEngineTagChangeBySharedMemory
    {
        [DllImport("Win32ShareMainUD.DLL", EntryPoint = "SharedTagServerInit", CallingConvention = CallingConvention.Cdecl)]
        static extern void SharedTagServerInit(int tag_total_count, int string_size);

        [DllImport("Win32ShareMainUD.DLL", EntryPoint = "SharedTagServerUnInit", CallingConvention = CallingConvention.Cdecl)]
        static extern void SharedTagServerUnInit();

        [DllImport("Win32ShareMainUD.DLL", EntryPoint = "SharedTagServerInitTag", CallingConvention = CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        static extern void SharedTagServerInitTag(int index, String tag, byte tag_type);

        [DllImport("Win32ShareMainUD.DLL", EntryPoint = "SharedTagServerInitSTRING", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void SharedTagServerInitSTRING();

        [DllImport("Win32ShareMainUD.DLL", EntryPoint = "SharedTagServerSetDOUBLE", CallingConvention = CallingConvention.Cdecl)]
        static extern void SharedTagServerSetDOUBLE(int index, double value);

        [DllImport("Win32ShareMainUD.DLL", EntryPoint = "SharedTagServerSetSTRING", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern void SharedTagServerSetSTRING(int index, string value);

        [DllImport("Win32ShareMainUD.DLL", EntryPoint = "SharedTagServerGetWriteItem", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern int SharedTagServerGetWriteItem([In, Out] byte[] array, int max);

        // Init과 UnInit중에 동시에 읽지 않도록 Lock해서 사용한다.
        static object objLock = new object();

        public static void Init()
        {
            // 대략 10만개 기준으로 0.2초 정도 걸린다.
            DebugSpeed elapsed = new DebugSpeed();

            elapsed.Start();

            lock (objLock)
            {
                SharedTagServerInit(TagLib.tagListAll.Length, 256); // 256 = 문자열 값을 가변적으로 사용할 수 있다.

                for (int i = 0; i < TagLib.tagListAll.Length; i++)
                {
                    SharedTagServerInitTag(i, TagLib.tagListAll[i].tag, (byte)TagLib.tagListAll[i].type);
                }

                // 태그를 모두 초기화한 후 문자열 버퍼를 준비한다.
                SharedTagServerInitSTRING();
            }

            TagPublicClass tp;

            for (int i = 0; i < TagLib.tagListAll.Length; i++)
            {
                tp = TagLib.tagListAll[i].ptr;

                if (tp.enumTagType == EnumTagType.AI)
                {
                    SetValue(tp, ((TagAiClass)tp).curr);
                }
                else if (tp.enumTagType == EnumTagType.DI)
                {
                    SetValue(tp, ((TagDiClass)tp).curr);
                }
                else if (tp.enumTagType == EnumTagType.ST)
                {
                    SetValue(tp, ((TagStClass)tp).curr);
                }
                else if (tp.enumTagType == EnumTagType.AO)
                {
                    SetValue(tp, ((TagAoClass)tp).curr);
                }
                else if (tp.enumTagType == EnumTagType.DO)
                {
                    SetValue(tp, ((TagDoClass)tp).curr);
                }
            }
            elapsed.Stop("SharedTagServerInit");
        }

        public static void SetValue(TagPublicClass tp, double value)
        {
            lock (objLock)
            {
                SharedTagServerSetDOUBLE(tp.nIndexOfStruct, value);
            }
        }

        public static void SetValue(TagPublicClass tp, string value)
        {
            lock (objLock)
            {
                SharedTagServerSetSTRING(tp.nIndexOfStruct, value);
            }
        }

        public static void UnInit()
        {
            lock (objLock)
            {
                SharedTagServerUnInit();
            }
        }

        /*
        // struct size = 9704
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
        public class WRITE_TAG
        {
            byte bReadyWrite;			// 쓰기가 준비 되었나?
            byte value_type;
            byte reserved1_3;			// 4의 배수에 맞춘다.
            byte reserved1_4;			// 4의 배수에 맞춘다.    
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public String tag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8192)]
            public byte[] value;
            int ip;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public String user_name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
            public String computer_name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            byte[] extra;		// 호환성을 휘해 여분을 남겨둔다. 
        }*/

        // 문자열을 null까지 읽고 주어진 크기까지 읽는다. structure 이므로 주어진 크기까지 읽어야 한다.
        static string ReadString(BinaryReader reader, int size)
        {
            StringBuilder s = new StringBuilder();
            int ch;
            bool bEnd = false;

            for (int i = 0; i < size; i++)
            {
                ch = reader.ReadInt16();

                if (bEnd) continue;

                if (ch == 0)
                {
                    bEnd = true;
                    continue;
                }

                s.Append((char)ch);
            }

            return s.ToString();
        }

        public static void GetWriteItem()
        {
            //WRITE_TAG wt = new WRITE_TAG();
            byte[] buffer = new byte[10000];

            int retn;

            lock (objLock)
            {
                retn = SharedTagServerGetWriteItem(buffer, buffer.Length);
            }
            
            if (retn == 1)
            {
                MemoryStream ms = new MemoryStream(buffer);
                BinaryReader reader = new BinaryReader(ms);
                
                reader.ReadByte();      // BYTE bReadyWrite;			// 쓰기가 준비 되었나?
                byte value_type = reader.ReadByte();      // BYTE value_type;
                reader.ReadByte();
                reader.ReadByte();

                string tag = ReadString(reader, 256);

                double value_f = 0; 
                string value_s = "";

                if (value_type == 2)
                {
                    value_f = reader.ReadDouble();
                    reader.ReadBytes(8192 - 8);
                    value_s = value_f.ToString();
                }
                else if (value_type == 9)
                {
                    value_s = ReadString(reader, 4096);
                }

                int ip = reader.ReadInt32();

                string username = ReadString(reader, 80);
                string computername = ReadString(reader, 80);

                ms.Close();
                
                int[] tag_pos = new int[1];
                TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);
                tp.sWriteWaitValue = value_s;
                tp.fWriteWaitValue = value_f;
                tp.sWriteUser = username;
                tp.sWriteIP = String.Format("{0}.{1}.{2}.{3}", (ip >> 0) & 0xFF, (ip >> 8) & 0xFF, (ip >> 16) & 0xFF, (ip >> 24) & 0xFF);
                tp.sWriteComputer = computername;
                tp.bWriteWait = true;
            }
        }
        
    }
}
