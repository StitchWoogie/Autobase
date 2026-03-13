using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NetTools
{
    public class FloatingTool
    {
        public static void FloatDataToByte(ref byte[] data, float val)
        {
            //byte[] imsi = new Byte[4];

            MemoryStream stream = new MemoryStream(data);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(val);
            writer.Close();
        }

        public static void DoubleDataToByte(ref byte[] data, double val)
        {
            //byte[] imsi = new Byte[8];

            MemoryStream stream = new MemoryStream(data);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(val);
            writer.Close();
        }

        public static float ByteDataToFloat(byte[] data, int pos)
        {
            byte[] imsi = new byte[4];
            for (int i = 0; i < 4; i++) imsi[i] = data[pos + i];

            MemoryStream stream = new MemoryStream(imsi);
            BinaryReader reader = new BinaryReader(stream);
            float val = reader.ReadSingle();
            reader.Close();
            return val;
        }

        public static double ByteDataToDouble(byte[] data, int pos)
        {
            byte[] imsi = new byte[8];
            for (int i = 0; i < 8; i++) imsi[i] = data[pos + i];

            MemoryStream stream = new MemoryStream(imsi);
            BinaryReader reader = new BinaryReader(stream);
            double val = reader.ReadDouble();
            reader.Close();
            return val;
        }

        public static float UintToFloat(uint data)
        {
            byte[] imsi = new byte[4];
            imsi[0] = (byte)((data >> 0) & 0xFF);
            imsi[1] = (byte)((data >> 8) & 0xFF);
            imsi[2] = (byte)((data >> 16) & 0xFF);
            imsi[3] = (byte)((data >> 24) & 0xFF);

            return ByteDataToFloat(imsi, 0);
        }

        public static double UlongToDouble(ulong data)
        {
            byte[] imsi = new byte[8];
            imsi[0] = (byte)((data >> 0) & 0xFF);
            imsi[1] = (byte)((data >> 8) & 0xFF);
            imsi[2] = (byte)((data >> 16) & 0xFF);
            imsi[3] = (byte)((data >> 24) & 0xFF);
            imsi[4] = (byte)((data >> 32) & 0xFF);
            imsi[5] = (byte)((data >> 40) & 0xFF);
            imsi[6] = (byte)((data >> 48) & 0xFF);
            imsi[7] = (byte)((data >> 56) & 0xFF);

            return ByteDataToDouble(imsi, 0);
        }

        public static uint FloatToUint(float data)
        {
            byte[] result = new byte[4];

            FloatDataToByte(ref result, data);

            return (uint)(result[0] | (result[1] << 8) | (result[2] << 16) | (result[3] << 24));
        }

        public static ulong DoubleToUlong(double data)
        {
            byte[] result = new byte[8];

            DoubleDataToByte(ref result, data);

            return (ulong)((ulong)result[0] | ((ulong)result[1] << 8) | ((ulong)result[2] << 16) | ((ulong)result[3] << 24) | ((ulong)result[4] << 32) | ((ulong)result[5] << 40) | ((ulong)result[6] << 48) | ((ulong)result[7] << 56));
        }
    }
}
