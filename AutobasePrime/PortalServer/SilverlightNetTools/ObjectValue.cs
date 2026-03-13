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

		static Type TypeChar   = typeof(char);//Type.GetType("System.Char");
		static Type TypeSByte  = typeof(sbyte);//Type.GetType("System.SByte");
		static Type TypeByte   = typeof(byte);//Type.GetType("System.Byte");
		static Type TypeInt16  = typeof(short);//Type.GetType("System.Int16");
		static Type TypeUInt16 = typeof(ushort);//Type.GetType("System.UInt16");
		static Type TypeInt32  = typeof(int);//Type.GetType("System.Int32");
		static Type TypeUInt32 = typeof(uint);//Type.GetType("System.UInt32");
		static Type TypeInt64  = typeof(long);//Type.GetType("System.Int64");
		static Type TypeUInt64 = typeof(ulong);//Type.GetType("System.UInt64");
		static Type TypeDouble = typeof(double);//Type.GetType("System.Double");
		static Type TypeSingle = typeof(float);//Type.GetType("System.Single");
		static Type TypeString = typeof(string);//Type.GetType("System.String");

		public static double ToDouble(object obj)
		{
			if(obj == null)	return 0;

			Type type = obj.GetType();

			if(type == TypeSByte)
				return (sbyte)obj;
			else if(type == TypeByte)
				return (byte)obj;
			else if(type == TypeChar)
				return (char)obj;
			else if(type == TypeInt16)
				return (short)obj;
			else if(type == TypeUInt16)
				return (ushort)obj;
			else if(type == TypeInt32)
				return (int)obj;
			else if(type == TypeUInt32)
				return (uint)obj;
			else if(type == TypeInt64)
				return (long)obj;
			else if(type == TypeUInt64)
				return (ulong)obj;
			else if(type == TypeDouble)
				return (double)obj;
			else if(type == TypeSingle)
				return (float)obj;
			else if(type == TypeString)
				return ConvertTool.ToDouble((string)obj);

			return 0;//(double)obj;	이 외의 경우는 0을 반환한다.
		}

		public static int ToInt(object obj)
		{
			if(obj == null)	return 0;

			Type type = obj.GetType();

			if(type == TypeSByte)
				return (sbyte)obj;
			else if(type == TypeByte)
				return (byte)obj;
			else if(type == TypeChar)
				return (char)obj;
			else if(type == TypeInt16)
				return (short)obj;
			else if(type == TypeUInt16)
				return (ushort)obj;
			else if(type == TypeInt32)
				return (int)obj;
			else if(type == TypeUInt32)
				return (int)((uint)obj);
			else if(type == TypeInt64)
				return (int)((long)obj);
			else if(type == TypeUInt64)
				return (int)((ulong)obj);
			else if(type == TypeDouble)
				return (int)((double)obj);
			else if(type == TypeSingle)
				return (int)((float)obj);
			else if(type == TypeString)
				return ConvertTool.ToInt32((string)obj);

			return 0;//(double)obj;	이 외의 경우는 0을 반환한다.
		}

		/*
		char GetValueChar(object obj)
		{
			return (char)GetValueDouble(obj);
		}
		*/

		public static sbyte ToSByte(object obj)
		{
			return (sbyte)ToDouble(obj);
		}

		/*
		byte GetValueByte(object obj)
		{
			return (byte)GetValueDouble(obj);
		}

		short GetValueShort(object obj)
		{
			return (short)GetValueDouble(obj);
		}

		ushort GetValueUshort(object obj)
		{
			return (ushort)GetValueDouble(obj);
		}

		uint GetValueUint(object obj)
		{
			return (uint)GetValueDouble(obj);
		}

		long GetValueLong(object obj)
		{
			return (long)GetValueDouble(obj);
		}

		ulong GetValueUlong(object obj)
		{
			return (ulong)GetValueDouble(obj);
		}

		float GetValueFloat(object obj)
		{
			return (float)GetValueDouble(obj);
		}
		*/

		public static string ToString(object obj)
		{
			if(obj == null)	return "";

			Type type = obj.GetType();

			if(type == TypeString) 
				return (string)obj;
			else if(type == TypeInt16)
				return ((short)obj).ToString();
			else if(type == TypeUInt16)
				return ((ushort)obj).ToString();
			else if(type == TypeInt32)
				return ((int)obj).ToString();
			else if(type == TypeUInt32)
				return ((uint)obj).ToString();
			else if(type == TypeInt64)
				return ((long)obj).ToString();
			else if(type == TypeUInt64)
				return ((ulong)obj).ToString();
			else if(type == TypeDouble)
				return ((double)obj).ToString();
			else if(type == TypeSingle)
				return ((float)obj).ToString();
			else
				return obj.ToString();	// 실제 이부분만 있어도 된다.
		}
	}
}
