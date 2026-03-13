using System;
using System.Security.Cryptography;

namespace NetTools
{
	/// <summary>
	/// Summary description for Crypto.
	/// </summary>
	public class Crypto
	{
		public Crypto()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static byte[] Encode(string source, byte[] encryption_key)
		{
			byte[] br = System.Text.Encoding.Unicode.GetBytes(source);

			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
			MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            
			des.Key = hashmd5.ComputeHash(encryption_key);
			des.Mode = CipherMode.ECB;

			ICryptoTransform desdencrpt = des.CreateEncryptor();

			return desdencrpt.TransformFinalBlock(br, 0, br.Length);
		}

		public static byte[] Encode(string source, string encryption_key)
		{
			return Encode(source, System.Text.Encoding.Unicode.GetBytes(encryption_key));
		}

		public static string Decode(byte[] source, byte[] encryption_key)
		{
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
			MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

			des.Key = hashmd5.ComputeHash(encryption_key);
			des.Mode = CipherMode.ECB;

			ICryptoTransform desdencrypt = des.CreateDecryptor();

			byte[] result;
			
			try 
			{
				result = desdencrypt.TransformFinalBlock(source, 0, source.Length);
			}
			catch 
			{
				return "";
			}

			return System.Text.Encoding.Unicode.GetString(result);
		}

		public static string Decode(byte[] source, string encryption_key)
		{
			return Decode(source, System.Text.Encoding.Unicode.GetBytes(encryption_key));
		}
	}
}
