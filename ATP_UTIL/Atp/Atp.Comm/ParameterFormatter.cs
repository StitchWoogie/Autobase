using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Drawing;
using NetTools;

namespace Ats.Comm
{
    public class ParameterFormatter
    {
        class StreamToolItem
        {
            public string name;
            public object value;
        }

        List<StreamToolItem> arrayItems = new List<StreamToolItem>();

        public ParameterFormatter()
        {
            
        }

        public ParameterFormatter(byte[] data)
        {
            Decode(data);
        }

        public void Add(string name, object value)
        {
            StreamToolItem item = new StreamToolItem();
            item.name = name;
            item.value = value;
            arrayItems.Add(item);
        }

        public object Get(string name)
        {
            for (int i = 0; i < arrayItems.Count; i++)
            {
                if (String.Compare(name, arrayItems[i].name, true) == 0)
                    return arrayItems[i].value;
            }

            return null;
        }

        /*
        public object Get(int index)
        {
            if(index < 0 || index >= arrayItems.Count)  return null;

            return arrayItems[index].value;
        }*/

        void Encode_int(MemoryStream ms, int data)
        {
            ms.WriteByte((byte)((data >> 0) & 0xFF));
            ms.WriteByte((byte)((data >> 8) & 0xFF));
            ms.WriteByte((byte)((data >> 16) & 0xFF));
            ms.WriteByte((byte)((data >> 24) & 0xFF));
        }

        void Encode_float(MemoryStream ms, float data)
        {
            uint u = FloatingTool.FloatToUint(data);

            ms.WriteByte((byte)((u >> 0) & 0xFF));
            ms.WriteByte((byte)((u >> 8) & 0xFF));
            ms.WriteByte((byte)((u >> 16) & 0xFF));
            ms.WriteByte((byte)((u >> 24) & 0xFF));
        }

        public byte[] Encode()
        {
            MemoryStream ms = new MemoryStream();
            object obj;
            StreamToolItem item;

            for (int i = 0; i < arrayItems.Count; i++)
            {
                item = arrayItems[i];

                // name을 쓴다.
                ms.WriteByte((byte)((item.name.Length >> 0) & 0xFF));
                ms.WriteByte((byte)((item.name.Length >> 8) & 0xFF));
                for (int j = 0; j < item.name.Length; j++)
                {
                    ms.WriteByte((byte)((item.name[j] >> 0) & 0xFF));
                    ms.WriteByte((byte)((item.name[j] >> 8) & 0xFF));
                }

                obj = item.value;

                if (obj.GetType() == typeof(sbyte))
                {
                    sbyte data = (sbyte)obj;
                    ms.WriteByte(1);
                    ms.WriteByte((byte)data);
                }
                else if (obj.GetType() == typeof(byte))
                {
                    byte data = (byte)obj;
                    ms.WriteByte(2);
                    ms.WriteByte((byte)data);
                }
                else if (obj.GetType() == typeof(short))
                {
                    short data = (short)obj;
                    ms.WriteByte(3);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                }
                else if (obj.GetType() == typeof(ushort))
                {
                    ushort data = (ushort)obj;
                    ms.WriteByte(4);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                }
                else if (obj.GetType() == typeof(int))
                {
                    int data = (int)obj;
                    ms.WriteByte(5);
                    Encode_int(ms, data);
                }
                else if (obj.GetType() == typeof(uint))
                {
                    uint data = (uint)obj;
                    ms.WriteByte(6);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                    ms.WriteByte((byte)((data >> 16) & 0xFF));
                    ms.WriteByte((byte)((data >> 24) & 0xFF));
                }
                else if (obj.GetType() == typeof(long))
                {
                    long data = (long)obj;
                    ms.WriteByte(7);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                    ms.WriteByte((byte)((data >> 16) & 0xFF));
                    ms.WriteByte((byte)((data >> 24) & 0xFF));
                    ms.WriteByte((byte)((data >> 32) & 0xFF));
                    ms.WriteByte((byte)((data >> 40) & 0xFF));
                    ms.WriteByte((byte)((data >> 48) & 0xFF));
                    ms.WriteByte((byte)((data >> 56) & 0xFF));
                }
                else if (obj.GetType() == typeof(ulong))
                {
                    ulong data = (ulong)obj;
                    ms.WriteByte(8);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                    ms.WriteByte((byte)((data >> 16) & 0xFF));
                    ms.WriteByte((byte)((data >> 24) & 0xFF));
                    ms.WriteByte((byte)((data >> 32) & 0xFF));
                    ms.WriteByte((byte)((data >> 40) & 0xFF));
                    ms.WriteByte((byte)((data >> 48) & 0xFF));
                    ms.WriteByte((byte)((data >> 56) & 0xFF));
                }
                else if (obj.GetType() == typeof(float))
                {
                    float data = (float)obj;

                    ms.WriteByte(9);

                    Encode_float(ms, data);
                }
                else if (obj.GetType() == typeof(double))
                {
                    double data = (double)obj;
                    ulong u = FloatingTool.DoubleToUlong(data);

                    ms.WriteByte(10);
                    ms.WriteByte((byte)((u >> 0) & 0xFF));
                    ms.WriteByte((byte)((u >> 8) & 0xFF));
                    ms.WriteByte((byte)((u >> 16) & 0xFF));
                    ms.WriteByte((byte)((u >> 24) & 0xFF));
                    ms.WriteByte((byte)((u >> 32) & 0xFF));
                    ms.WriteByte((byte)((u >> 40) & 0xFF));
                    ms.WriteByte((byte)((u >> 48) & 0xFF));
                    ms.WriteByte((byte)((u >> 56) & 0xFF));
                }
                else if (obj.GetType() == typeof(string))
                {
                    string data = (string)obj;
                    ms.WriteByte(11);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));
                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(bool))
                {
                    bool data = (bool)obj;
                    ms.WriteByte(12);
                    ms.WriteByte(data ? (byte)1 : (byte)0);
                }
                else if (obj.GetType() == typeof(sbyte[]))
                {
                    sbyte[] data = (sbyte[])obj;

                    ms.WriteByte(21);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)data[j]);
                    }
                }
                else if (obj.GetType() == typeof(byte[]))
                {
                    byte[] data = (byte[])obj;

                    ms.WriteByte(22);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)data[j]);
                    }
                }
                else if (obj.GetType() == typeof(short[]))
                {
                    short[] data = (short[])obj;

                    ms.WriteByte(23);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(ushort[]))
                {
                    ushort[] data = (ushort[])obj;

                    ms.WriteByte(24);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(int[]))
                {
                    int[] data = (int[])obj;

                    ms.WriteByte(25);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        Encode_int(ms, data[j]);
                    }
                }
                else if (obj.GetType() == typeof(uint[]))
                {
                    uint[] data = (uint[])obj;

                    ms.WriteByte(26);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 16) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 24) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(long[]))
                {
                    long[] data = (long[])obj;

                    ms.WriteByte(27);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 16) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 24) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 32) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 40) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 48) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 56) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(ulong[]))
                {
                    ulong[] data = (ulong[])obj;

                    ms.WriteByte(28);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 16) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 24) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 32) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 40) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 48) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 56) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(float[]))
                {
                    float[] data = (float[])obj;

                    ms.WriteByte(29);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        Encode_float(ms, data[j]);
                    }
                }
                else if (obj.GetType() == typeof(double[]))
                {
                    double[] data = (double[])obj;

                    ms.WriteByte(30);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        ulong u = FloatingTool.DoubleToUlong(data[j]);
                        ms.WriteByte((byte)((u >> 0) & 0xFF));
                        ms.WriteByte((byte)((u >> 8) & 0xFF));
                        ms.WriteByte((byte)((u >> 16) & 0xFF));
                        ms.WriteByte((byte)((u >> 24) & 0xFF));
                        ms.WriteByte((byte)((u >> 32) & 0xFF));
                        ms.WriteByte((byte)((u >> 40) & 0xFF));
                        ms.WriteByte((byte)((u >> 48) & 0xFF));
                        ms.WriteByte((byte)((u >> 56) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(string[]))
                {
                    string[] data = (string[])obj;
                    ms.WriteByte(31);
                    Encode_int(ms, data.Length);
                    for (int j = 0; j < data.Length; j++)
                    {
                        string s = data[j];
                        ms.WriteByte((byte)((s.Length >> 0) & 0xFF));
                        ms.WriteByte((byte)((s.Length >> 8) & 0xFF));
                        ms.WriteByte((byte)((s.Length >> 16) & 0xFF));
                        ms.WriteByte((byte)((s.Length >> 24) & 0xFF));

                        for (int k = 0; k < s.Length; k++)
                        {
                            ms.WriteByte((byte)((s[k] >> 0) & 0xFF));
                            ms.WriteByte((byte)((s[k] >> 8) & 0xFF));
                        }
                    }
                }
                else if (obj.GetType() == typeof(bool[]))
                {
                    bool[] data = (bool[])obj;

                    ms.WriteByte(32);
                    Encode_int(ms, data.Length);

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte(data[j] ? (byte)1 : (byte)0);
                    }
                }
                else if (obj.GetType() == typeof(Rectangle))
                {
                    Rectangle data = (Rectangle)obj;
                    ms.WriteByte(41);

                    Encode_int(ms, data.X);
                    Encode_int(ms, data.Y);
                    Encode_int(ms, data.Width);
                    Encode_int(ms, data.Height);
                }
                else if (obj.GetType() == typeof(RectangleF))
                {
                    RectangleF data = (RectangleF)obj;
                    ms.WriteByte(42);

                    Encode_float(ms, data.X);
                    Encode_float(ms, data.Y);
                    Encode_float(ms, data.Width);
                    Encode_float(ms, data.Height);
                }

                else
                {
                    ms.WriteByte(0);
                }
            }

            return ms.ToArray();
        }

        bool Decode_int(byte[] buf, ref int i, out int value)
        {
            value = 0;
            int data_size = 4;

            if (i + data_size > buf.Length)
                return false;

            int data = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
            i += data_size;

            value = data;

            return true;
        }

        bool Decode_float(byte[] buf, ref int i, out float value)
        {
            value = 0;

            int data_size = 4;
            if (i + data_size > buf.Length)
                return false;

            uint data = (uint)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
            i += data_size;

            value = FloatingTool.UintToFloat(data);

            return true;
        }

        bool Decode(byte[] buf)
        {
            int data_size = 0;
            byte data_type;
            StreamToolItem item;

            int i = 0;
            int name_size;
            StringBuilder name;

            while (true)
            {
                if (i >= buf.Length) break;

                name_size = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8));

                i += 2;

                if (i + name_size * 2 > buf.Length)
                    return false;

                name = new StringBuilder();

                for (int j = 0; j < name_size; j++)
                {
                    name.Append((char)(buf[i + 0] + buf[i + 1] * 256));

                    i += 2;
                }

                item = new StreamToolItem();
                item.name = name.ToString();

                data_type = buf[i];
                i++;

                if (data_type == 1)
                {
                    data_size = 1;
                    if (i + data_size > buf.Length)
                        return false;

                    sbyte data = (sbyte)buf[i];

                    i += data_size;

                    item.value = data;
                }
                else if (data_type == 2)
                {
                    data_size = 1;
                    if (i + data_size > buf.Length)
                        return false;

                    byte data = buf[i];
                    i += data_size;

                    item.value = data;
                }
                else if (data_type == 3)
                {
                    data_size = 2;
                    if (i + data_size > buf.Length)
                        return false;

                    short data = (short)(buf[i + 0] + buf[i + 1] * 256);
                    i += data_size;

                    item.value = data;
                }
                else if (data_type == 4)
                {
                    data_size = 2;
                    if (i + data_size > buf.Length)
                        return false;

                    ushort data = (ushort)(buf[i + 0] + buf[i + 1] * 256);
                    i += data_size;

                    item.value = data;
                }
                else if (data_type == 5)
                {
                    int data;
                    if (!Decode_int(buf, ref i, out data)) return false;
                    item.value = data;
                }
                else if (data_type == 6)
                {
                    data_size = 4;
                    if (i + data_size > buf.Length)
                        return false;

                    uint data = (uint)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                    i += data_size;

                    item.value = data;
                }
                else if (data_type == 7)
                {
                    data_size = 8;
                    if (i + data_size > buf.Length)
                        return false;

                    long data = (long)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                    i += data_size;

                    item.value = data;
                }
                else if (data_type == 8)
                {
                    data_size = 8;
                    if (i + data_size > buf.Length)
                        return false;

                    ulong data = (ulong)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                    i += data_size;

                    item.value = data;
                }
                else if (data_type == 9)
                {
                    float data;
                    if (!Decode_float(buf, ref i, out data)) return false;
                    item.value = data;
                }
                else if (data_type == 10)
                {
                    data_size = 8;
                    if (i + data_size > buf.Length)
                        return false;

                    ulong data = (ulong)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                    i += data_size;

                    double f = FloatingTool.UlongToDouble(data);

                    item.value = f;
                }
                else if (data_type == 11)
                {
                    data_size = 2;

                    if (i + 4 > buf.Length)
                        return false;

                    int str_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + str_len * data_size > buf.Length)
                        return false;

                    StringBuilder s = new StringBuilder();

                    for (int j = 0; j < str_len; j++)
                    {
                        s.Append((char)(buf[i + 0] + buf[i + 1] * 256));

                        i += data_size;
                    }

                    item.value = s.ToString();
                }
                else if (data_type == 12)
                {
                    data_size = 1;
                    if (i + data_size > buf.Length)
                        return false;

                    bool data = (buf[i] == 1);
                    i += data_size;

                    item.value = data;
                }
                else if (data_type == 21)
                {
                    data_size = 1;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    sbyte[] data = new sbyte[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (sbyte)buf[i];
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 22)
                {
                    data_size = 1;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    byte[] data = new byte[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (byte)buf[i];
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 23)
                {
                    data_size = 2;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    short[] data = new short[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (short)(buf[i + 0] + buf[i + 1] * 256);
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 24)
                {
                    data_size = 2;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    ushort[] data = new ushort[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (ushort)(buf[i + 0] + buf[i + 1] * 256);
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 25)
                {
                    data_size = 4;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    int[] data = new int[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 26)
                {
                    data_size = 4;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    uint[] data = new uint[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (uint)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 27)
                {
                    data_size = 8;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    long[] data = new long[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (long)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 28)
                {
                    data_size = 8;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    ulong[] data = new ulong[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (ulong)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 29)
                {
                    data_size = 4;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    float[] data = new float[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        uint u = (uint)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                        data[j] = FloatingTool.UintToFloat(u);
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 30)
                {
                    data_size = 8;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    double[] data = new double[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        ulong u = (ulong)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                        data[j] = FloatingTool.UlongToDouble(u);
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 31)
                {
                    data_size = 2;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                    string[] data = new string[array_len];

                    i += 4;

                    for (int k = 0; k < array_len; k++)
                    {
                        int str_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                        i += 4;

                        if (i + str_len * data_size > buf.Length)
                            return false;

                        StringBuilder s = new StringBuilder();

                        for (int j = 0; j < str_len; j++)
                        {
                            s.Append((char)(buf[i + 0] + buf[i + 1] * 256));
                            i += data_size;
                        }

                        data[k] = s.ToString();
                    }

                    item.value = data;
                }
                else if (data_type == 32)
                {
                    data_size = 1;

                    if (i + 4 > buf.Length)
                        return false;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return false;

                    bool[] data = new bool[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (buf[i] == 1);
                        i += data_size;
                    }

                    item.value = data;
                }
                else if (data_type == 41)
                {
                    int x, y, w, h;

                    if (!Decode_int(buf, ref i, out x)) return false;
                    if (!Decode_int(buf, ref i, out y)) return false;
                    if (!Decode_int(buf, ref i, out w)) return false;
                    if (!Decode_int(buf, ref i, out h)) return false;

                    item.value = new Rectangle(x, y, w, h);
                }
                else if (data_type == 42)
                {
                    float x, y, w, h;

                    if (!Decode_float(buf, ref i, out x)) return false;
                    if (!Decode_float(buf, ref i, out y)) return false;
                    if (!Decode_float(buf, ref i, out w)) return false;
                    if (!Decode_float(buf, ref i, out h)) return false;

                    item.value = new RectangleF(x, y, w, h);
                }
                else
                {
                    break;
                }

                arrayItems.Add(item);
            }

            return true;
        }

        /*
        public static byte[] ConvertArrayListToBytes(ArrayList array)
        {
            MemoryStream ms = new MemoryStream();
            object obj;

            for (int i = 0; i < array.Count; i++)
            {
                obj = array[i];
                if (obj.GetType() == typeof(sbyte))
                {
                    sbyte data = (sbyte)obj;
                    ms.WriteByte(1);
                    ms.WriteByte((byte)data);
                }
                else if (obj.GetType() == typeof(byte))
                {
                    byte data = (byte)obj;
                    ms.WriteByte(2);
                    ms.WriteByte((byte)data);
                }
                else if (obj.GetType() == typeof(short))
                {
                    short data = (short)obj;
                    ms.WriteByte(3);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                }
                else if (obj.GetType() == typeof(ushort))
                {
                    ushort data = (ushort)obj;
                    ms.WriteByte(4);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                }
                else if (obj.GetType() == typeof(int))
                {
                    int data = (int)obj;
                    ms.WriteByte(5);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                    ms.WriteByte((byte)((data >> 16) & 0xFF));
                    ms.WriteByte((byte)((data >> 24) & 0xFF));
                }
                else if (obj.GetType() == typeof(uint))
                {
                    uint data = (uint)obj;
                    ms.WriteByte(6);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                    ms.WriteByte((byte)((data >> 16) & 0xFF));
                    ms.WriteByte((byte)((data >> 24) & 0xFF));
                }
                else if (obj.GetType() == typeof(long))
                {
                    long data = (long)obj;
                    ms.WriteByte(7);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                    ms.WriteByte((byte)((data >> 16) & 0xFF));
                    ms.WriteByte((byte)((data >> 24) & 0xFF));
                    ms.WriteByte((byte)((data >> 32) & 0xFF));
                    ms.WriteByte((byte)((data >> 40) & 0xFF));
                    ms.WriteByte((byte)((data >> 48) & 0xFF));
                    ms.WriteByte((byte)((data >> 56) & 0xFF));
                }
                else if (obj.GetType() == typeof(ulong))
                {
                    ulong data = (ulong)obj;
                    ms.WriteByte(8);
                    ms.WriteByte((byte)((data >> 0) & 0xFF));
                    ms.WriteByte((byte)((data >> 8) & 0xFF));
                    ms.WriteByte((byte)((data >> 16) & 0xFF));
                    ms.WriteByte((byte)((data >> 24) & 0xFF));
                    ms.WriteByte((byte)((data >> 32) & 0xFF));
                    ms.WriteByte((byte)((data >> 40) & 0xFF));
                    ms.WriteByte((byte)((data >> 48) & 0xFF));
                    ms.WriteByte((byte)((data >> 56) & 0xFF));
                }
                else if (obj.GetType() == typeof(float))
                {
                    float data = (float)obj;

                    uint u = FloatingTool.FloatToUint(data);

                    ms.WriteByte(9);
                    ms.WriteByte((byte)((u >> 0) & 0xFF));
                    ms.WriteByte((byte)((u >> 8) & 0xFF));
                    ms.WriteByte((byte)((u >> 16) & 0xFF));
                    ms.WriteByte((byte)((u >> 24) & 0xFF));
                }
                else if (obj.GetType() == typeof(double))
                {
                    double data = (double)obj;
                    ulong u = FloatingTool.DoubleToUlong(data);

                    ms.WriteByte(10);
                    ms.WriteByte((byte)((u >> 0) & 0xFF));
                    ms.WriteByte((byte)((u >> 8) & 0xFF));
                    ms.WriteByte((byte)((u >> 16) & 0xFF));
                    ms.WriteByte((byte)((u >> 24) & 0xFF));
                    ms.WriteByte((byte)((u >> 32) & 0xFF));
                    ms.WriteByte((byte)((u >> 40) & 0xFF));
                    ms.WriteByte((byte)((u >> 48) & 0xFF));
                    ms.WriteByte((byte)((u >> 56) & 0xFF));
                }
                else if (obj.GetType() == typeof(string))
                {
                    string data = (string)obj;
                    ms.WriteByte(11);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));
                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(bool))
                {
                    bool data = (bool)obj;
                    ms.WriteByte(12);
                    ms.WriteByte(data ? (byte)1 : (byte)0);
                }
                else if (obj.GetType() == typeof(sbyte[]))
                {
                    sbyte[] data = (sbyte[])obj;

                    ms.WriteByte(21);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)data[j]);
                    }
                }
                else if (obj.GetType() == typeof(byte[]))
                {
                    byte[] data = (byte[])obj;

                    ms.WriteByte(22);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)data[j]);
                    }
                }
                else if (obj.GetType() == typeof(short[]))
                {
                    short[] data = (short[])obj;

                    ms.WriteByte(23);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(ushort[]))
                {
                    ushort[] data = (ushort[])obj;

                    ms.WriteByte(24);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(int[]))
                {
                    int[] data = (int[])obj;

                    ms.WriteByte(25);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 16) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 24) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(uint[]))
                {
                    uint[] data = (uint[])obj;

                    ms.WriteByte(26);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 16) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 24) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(long[]))
                {
                    long[] data = (long[])obj;

                    ms.WriteByte(27);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 16) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 24) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 32) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 40) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 48) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 56) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(ulong[]))
                {
                    ulong[] data = (ulong[])obj;

                    ms.WriteByte(28);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte((byte)((data[j] >> 0) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 8) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 16) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 24) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 32) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 40) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 48) & 0xFF));
                        ms.WriteByte((byte)((data[j] >> 56) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(float[]))
                {
                    float[] data = (float[])obj;

                    ms.WriteByte(29);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        uint u = FloatingTool.FloatToUint(data[j]);
                        ms.WriteByte((byte)((u >> 0) & 0xFF));
                        ms.WriteByte((byte)((u >> 8) & 0xFF));
                        ms.WriteByte((byte)((u >> 16) & 0xFF));
                        ms.WriteByte((byte)((u >> 24) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(double[]))
                {
                    double[] data = (double[])obj;

                    ms.WriteByte(30);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ulong u = FloatingTool.DoubleToUlong(data[j]);
                        ms.WriteByte((byte)((u >> 0) & 0xFF));
                        ms.WriteByte((byte)((u >> 8) & 0xFF));
                        ms.WriteByte((byte)((u >> 16) & 0xFF));
                        ms.WriteByte((byte)((u >> 24) & 0xFF));
                        ms.WriteByte((byte)((u >> 32) & 0xFF));
                        ms.WriteByte((byte)((u >> 40) & 0xFF));
                        ms.WriteByte((byte)((u >> 48) & 0xFF));
                        ms.WriteByte((byte)((u >> 56) & 0xFF));
                    }
                }
                else if (obj.GetType() == typeof(string[]))
                {
                    string[] data = (string[])obj;
                    ms.WriteByte(31);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));
                    for (int j = 0; j < data.Length; j++)
                    {
                        string s = data[j];
                        ms.WriteByte((byte)((s.Length >> 0) & 0xFF));
                        ms.WriteByte((byte)((s.Length >> 8) & 0xFF));
                        ms.WriteByte((byte)((s.Length >> 16) & 0xFF));
                        ms.WriteByte((byte)((s.Length >> 24) & 0xFF));

                        for (int k = 0; k < s.Length; k++)
                        {
                            ms.WriteByte((byte)((s[k] >> 0) & 0xFF));
                            ms.WriteByte((byte)((s[k] >> 8) & 0xFF));
                        }
                    }
                }
                else if (obj.GetType() == typeof(bool[]))
                {
                    bool[] data = (bool[])obj;

                    ms.WriteByte(32);
                    ms.WriteByte((byte)((data.Length >> 0) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 8) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 16) & 0xFF));
                    ms.WriteByte((byte)((data.Length >> 24) & 0xFF));

                    for (int j = 0; j < data.Length; j++)
                    {
                        ms.WriteByte(data[j] ? (byte)1 : (byte)0);
                    }
                }
                else
                {
                    ms.WriteByte(0);
                }
            }

            return ms.ToArray();
        }

        public static ArrayList ConvertBytesToArrayList(byte[] buf)
        {
            ArrayList array = new ArrayList();
            int data_size = 0;
            byte data_type;

            int i = 0;
            while (true)
            {
                if (i >= buf.Length) break;

                data_type = buf[i];
                i++;

                if (data_type == 1)
                {
                    data_size = 1;
                    if (i + data_size > buf.Length)
                        return null;

                    sbyte data = (sbyte)buf[i];

                    i += data_size;

                    array.Add(data);
                }
                else if (data_type == 2)
                {
                    data_size = 1;
                    if (i + data_size > buf.Length)
                        return null;

                    byte data = buf[i];
                    i += data_size;

                    array.Add(data);
                }
                else if (data_type == 3)
                {
                    data_size = 2;
                    if (i + data_size > buf.Length)
                        return null;

                    short data = (short)(buf[i + 0] + buf[i + 1] * 256);
                    i += data_size;

                    array.Add(data);
                }
                else if (data_type == 4)
                {
                    data_size = 2;
                    if (i + data_size > buf.Length)
                        return null;

                    ushort data = (ushort)(buf[i + 0] + buf[i + 1] * 256);
                    i += data_size;

                    array.Add(data);
                }
                else if (data_type == 5)
                {
                    data_size = 4;
                    if (i + data_size > buf.Length)
                        return null;

                    int data = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                    i += data_size;

                    array.Add(data);
                }
                else if (data_type == 6)
                {
                    data_size = 4;
                    if (i + data_size > buf.Length)
                        return null;

                    uint data = (uint)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                    i += data_size;

                    array.Add(data);
                }
                else if (data_type == 7)
                {
                    data_size = 8;
                    if (i + data_size > buf.Length)
                        return null;

                    long data = (long)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                    i += data_size;

                    array.Add(data);
                }
                else if (data_type == 8)
                {
                    data_size = 8;
                    if (i + data_size > buf.Length)
                        return null;

                    ulong data = (ulong)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                    i += data_size;

                    array.Add(data);
                }
                else if (data_type == 9)
                {
                    data_size = 4;
                    if (i + data_size > buf.Length)
                        return null;

                    uint data = (uint)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                    i += data_size;

                    float f = FloatingTool.UintToFloat(data);

                    array.Add(f);
                }
                else if (data_type == 10)
                {
                    data_size = 8;
                    if (i + data_size > buf.Length)
                        return null;

                    ulong data = (ulong)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                    i += data_size;

                    double f = FloatingTool.UlongToDouble(data);

                    array.Add(f);
                }
                else if (data_type == 11)
                {
                    data_size = 2;

                    if (i + 4 > buf.Length)
                        return null;

                    int str_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + str_len * data_size > buf.Length)
                        return null;

                    StringBuilder s = new StringBuilder();

                    for (int j = 0; j < str_len; j++)
                    {
                        s.Append((char)(buf[i + 0] + buf[i + 1] * 256));

                        i += data_size;
                    }

                    array.Add(s.ToString());
                }
                else if (data_type == 12)
                {
                    data_size = 1;
                    if (i + data_size > buf.Length)
                        return null;

                    bool data = (buf[i] == 1);
                    i += data_size;

                    array.Add(data);
                }
                else if (data_type == 21)
                {
                    data_size = 1;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    sbyte[] data = new sbyte[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (sbyte)buf[i];
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 22)
                {
                    data_size = 1;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    byte[] data = new byte[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (byte)buf[i];
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 23)
                {
                    data_size = 2;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    short[] data = new short[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (short)(buf[i + 0] + buf[i + 1] * 256);
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 24)
                {
                    data_size = 2;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    ushort[] data = new ushort[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (ushort)(buf[i + 0] + buf[i + 1] * 256);
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 25)
                {
                    data_size = 4;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    int[] data = new int[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 26)
                {
                    data_size = 4;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    uint[] data = new uint[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (uint)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 27)
                {
                    data_size = 8;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    long[] data = new long[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (long)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 28)
                {
                    data_size = 8;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    ulong[] data = new ulong[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (ulong)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 29)
                {
                    data_size = 4;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    float[] data = new float[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        uint u = (uint)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                        data[j] = FloatingTool.UintToFloat(u);
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 30)
                {
                    data_size = 8;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    double[] data = new double[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        ulong u = (ulong)(((long)buf[i + 0] << 0) | ((long)buf[i + 1] << 8) | ((long)buf[i + 2] << 16) | ((long)buf[i + 3] << 24) | ((long)buf[i + 4] << 32) | ((long)buf[i + 5] << 40) | ((long)buf[i + 6] << 48) | ((long)buf[i + 7] << 56));
                        data[j] = FloatingTool.UlongToDouble(u);
                        i += data_size;
                    }

                    array.Add(data);
                }
                else if (data_type == 31)
                {
                    data_size = 2;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));
                    string[] data = new string[array_len];

                    i += 4;

                    for (int k = 0; k < array_len; k++)
                    {
                        int str_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                        i += 4;

                        if (i + str_len * data_size > buf.Length)
                            return null;

                        StringBuilder s = new StringBuilder();

                        for (int j = 0; j < str_len; j++)
                        {
                            s.Append((char)(buf[i + 0] + buf[i + 1] * 256));
                            i += data_size;
                        }

                        data[k] = s.ToString();
                    }

                    array.Add(data);
                }
                else if (data_type == 32)
                {
                    data_size = 1;

                    if (i + 4 > buf.Length)
                        return null;

                    int array_len = (int)((buf[i + 0] << 0) | (buf[i + 1] << 8) | (buf[i + 2] << 16) | (buf[i + 3] << 24));

                    i += 4;

                    if (i + array_len * data_size > buf.Length)
                        return null;

                    bool[] data = new bool[array_len];

                    for (int j = 0; j < array_len; j++)
                    {
                        data[j] = (buf[i] == 1);
                        i += data_size;
                    }

                    array.Add(data);
                }
                else
                {
                    break;
                }
            }

            return array;
        }*/
    }
}
