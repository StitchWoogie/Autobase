using System;

namespace NetTools
{
    /// <summary>
    /// Summary description for ObjectValue.
    /// </summary>
    public class ObjectValue
    {
        public ObjectValue()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        // typeof 는 컴파일 타임에 분석된다.
        static Type TypeBool = typeof(bool);//Type.GetType("System.Char");
        static Type TypeChar = typeof(char);//Type.GetType("System.Char");
        static Type TypeSByte = typeof(sbyte);//Type.GetType("System.SByte");
        static Type TypeByte = typeof(byte);//Type.GetType("System.Byte");
        static Type TypeInt16 = typeof(short);//Type.GetType("System.Int16");
        static Type TypeUInt16 = typeof(ushort);//Type.GetType("System.UInt16");
        static Type TypeInt32 = typeof(int);//Type.GetType("System.Int32");
        static Type TypeUInt32 = typeof(uint);//Type.GetType("System.UInt32");
        static Type TypeInt64 = typeof(long);//Type.GetType("System.Int64");
        static Type TypeUInt64 = typeof(ulong);//Type.GetType("System.UInt64");
        static Type TypeDouble = typeof(double);//Type.GetType("System.Double");
        static Type TypeSingle = typeof(float);//Type.GetType("System.Single");
        static Type TypeString = typeof(string);//Type.GetType("System.String");

        public static float ToFloat(object obj)
        {
            return (float)ToDouble(obj);
        }

        public static double ToDouble(object obj)
        {
            if (obj == null) return 0;

            Type type = obj.GetType();

            if (type == TypeSByte)
                return (sbyte)obj;
            else if (type == TypeByte)
                return (byte)obj;
            else if (type == TypeChar)
                return (char)obj;
            else if (type == TypeInt16)
                return (short)obj;
            else if (type == TypeUInt16)
                return (ushort)obj;
            else if (type == TypeInt32)
                return (int)obj;
            else if (type == TypeUInt32)
                return (uint)obj;
            else if (type == TypeInt64)
                return (long)obj;
            else if (type == TypeUInt64)
                return (ulong)obj;
            else if (type == TypeDouble)
                return (double)obj;
            else if (type == TypeSingle)
                return (float)obj;
            else if (type == TypeString)
                return ConvertTool.ToDouble((string)obj);
            else if (type == TypeBool)
                return (bool)obj ? 1 : 0;

            return 0;//(double)obj;	이 외의 경우는 0을 반환한다.
        }

        public static sbyte ToSbyte(object obj)
        {
            return (sbyte)ToLong(obj);
        }

        public static byte ToByte(object obj)
        {
            return (byte)ToLong(obj);
        }

        public static char ToChar(object obj)
        {
            return (char)ToLong(obj);
        }

        public static short ToShort(object obj)
        {
            return (short)ToLong(obj);
        }

        public static ushort ToUshort(object obj)
        {
            return (ushort)ToLong(obj);
        }

        public static int ToInt(object obj)
        {
            return (int)ToLong(obj);
        }

        public static uint ToUint(object obj)
        {
            return (uint)ToLong(obj);
        }

        public static long ToLong(object obj)
        {
            if (obj == null) return 0;

            Type type = obj.GetType();

            if (type == TypeSByte)
                return (sbyte)obj;
            else if (type == TypeByte)
                return (byte)obj;
            else if (type == TypeChar)
                return (char)obj;
            else if (type == TypeInt16)
                return (short)obj;
            else if (type == TypeUInt16)
                return (ushort)obj;
            else if (type == TypeInt32)
                return (int)obj;
            else if (type == TypeUInt32)
                return ((uint)obj);
            else if (type == TypeInt64)
                return ((long)obj);
            else if (type == TypeUInt64)
                return (long)((ulong)obj);
            else if (type == TypeDouble)
                return (long)((double)obj);
            else if (type == TypeSingle)
                return (long)((float)obj);
            else if (type == TypeString)
                return ConvertTool.ToInt64((string)obj);
            else if (type == TypeBool)
                return (bool)obj ? 1 : 0;

            return 0;//(double)obj;	이 외의 경우는 0을 반환한다.
        }

        public static ulong ToUlong(object obj)
        {
            return (ulong)ToLong(obj);
        }

        public static string ToString(object obj)
        {
            if (obj == null) return "";

            Type type = obj.GetType();

            if (type == TypeString)
                return (string)obj;
            else if (type == TypeInt16)
                return ((short)obj).ToString();
            else if (type == TypeUInt16)
                return ((ushort)obj).ToString();
            else if (type == TypeInt32)
                return ((int)obj).ToString();
            else if (type == TypeUInt32)
                return ((uint)obj).ToString();
            else if (type == TypeInt64)
                return ((long)obj).ToString();
            else if (type == TypeUInt64)
                return ((ulong)obj).ToString();
            else if (type == TypeDouble)
                return ((double)obj).ToString();
            else if (type == TypeSingle)
                return ((float)obj).ToString();
            else
                return obj.ToString();	// 실제 이부분만 있어도 된다.
        }

        public static byte[] ToByteArray(object obj)
        {
            if (obj == null) return null;

            Type type = obj.GetType();

            if (type == typeof(object[]))
            {
                object[] src = (object[])obj;
                byte[] tar = new byte[src.Length];

                for (int i = 0; i < src.Length; i++)
                {
                    tar[i] = ToByte(src[i]);
                }

                return tar;
            }
            else if (type == typeof(byte[]))
            {
                return (byte[])obj;
            }
            else
            {
                return null;
            }
        }
    }
}
