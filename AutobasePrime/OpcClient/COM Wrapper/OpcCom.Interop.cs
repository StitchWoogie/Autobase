//============================================================================
// TITLE: Common.cs
//
// CONTENTS:
// 
// Defines wrappers for basic WIN32/OLE32 API functions.
//
// (c) Copyright 2002-2003 The OPC Foundation
// ALL RIGHTS RESERVED.
//
// DISCLAIMER:
//  This code is provided by the OPC Foundation solely to assist in 
//  understanding and use of the appropriate OPC Specification(s) and may be 
//  used as set forth in the License Grant section of the OPC Specification.
//  This code is provided as-is and without warranty or support of any sort
//  and is subject to the Warranty and Liability Disclaimers which appear
//  in the printed OPC Specification.
//
// MODIFICATION LOG:
//
// Date       By    Notes
// ---------- ---   -----
// 2002/11/16 RSA   First release.
// 2003/09/29 RSA   Fixed a security bug when connecting to COM servers on Windows CE devices.

using System;
using System.Net;
using System.Globalization;
using System.Runtime.InteropServices;
using Opc;
//using System.Windows.Forms;

namespace OpcCom
{
	/// <summary>
	/// Exposes WIN32 and COM API functions.
	/// </summary>
	public class Interop
	{
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		private struct SERVER_INFO_100
		{
			public uint   sv100_platform_id;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string sv100_name;
		} 	

		private const uint LEVEL_SERVER_INFO_100 = 100;
		private const uint LEVEL_SERVER_INFO_101 = 101;

		private const int  MAX_PREFERRED_LENGTH  = -1;

		private const uint SV_TYPE_WORKSTATION   = 0x00000001;
		private const uint SV_TYPE_SERVER        = 0x00000002;

		[DllImport("Netapi32.dll")]
		private static extern int NetServerEnum(
			IntPtr     servername,
			uint       level,
			out IntPtr bufptr,
			int        prefmaxlen,
			out int    entriesread,
			out int    totalentries,
			uint       servertype,
			IntPtr     domain,
			IntPtr     resume_handle);

		[DllImport("Netapi32.dll")]	
		private static extern int NetApiBufferFree(IntPtr buffer);

		/// <summary>
		/// Enumerates computers on the local network.
		/// </summary>
		public static string[] EnumComputers()
		{
			IntPtr pInfo;

			int entriesRead = 0;
			int totalEntries = 0;

			int result = NetServerEnum(
				IntPtr.Zero,
				LEVEL_SERVER_INFO_100,
				out pInfo,
				MAX_PREFERRED_LENGTH,
				out entriesRead,
				out totalEntries,
				SV_TYPE_WORKSTATION | SV_TYPE_SERVER,
				IntPtr.Zero,
				IntPtr.Zero);		

			if (result != 0)
			{
				throw new ApplicationException("NetApi Error = " + String.Format("0x{0,0:X}", result));
			}

			string[] computers = new string[entriesRead];

			IntPtr pos = pInfo;

			for (int ii = 0; ii < entriesRead; ii++)
			{
				SERVER_INFO_100 info = (SERVER_INFO_100)Marshal.PtrToStructure(pos, typeof(SERVER_INFO_100));
				
				computers[ii] = info.sv100_name;

				pos = (IntPtr)(pos.ToInt32() + Marshal.SizeOf(typeof(SERVER_INFO_100)));
			}

			NetApiBufferFree(pInfo);

			return computers;
		}

		private const int MAX_MESSAGE_LENGTH = 1024;

		private const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
		private const uint FORMAT_MESSAGE_FROM_SYSTEM    = 0x00001000;

		[DllImport("Kernel32.dll")]
		private static extern int FormatMessageW(
			int    dwFlags,
			IntPtr lpSource,
			int    dwMessageId,
			int    dwLanguageId,
			IntPtr lpBuffer,
			int    nSize,
			IntPtr Arguments);

		/// <summary>
		/// Retrieves the system message text for the specified error.
		/// </summary>
		public static string GetSystemMessage(int error)
		{
			IntPtr buffer = Marshal.AllocCoTaskMem(MAX_MESSAGE_LENGTH);

			int result = FormatMessageW(
				(int)(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_FROM_SYSTEM),
				IntPtr.Zero,
				error,
				0,
				buffer,
				MAX_MESSAGE_LENGTH-1,
				IntPtr.Zero);

			string msg = Marshal.PtrToStringUni(buffer);
			Marshal.FreeCoTaskMem(buffer);

			if (msg != null && msg.Length > 0)
			{
				return msg;
			}

            return String.Format("0x{0,0:X}", error);
		}

		private const int MAX_COMPUTERNAME_LENGTH = 31;

		[DllImport("Kernel32.dll")]
		private static extern int GetComputerNameW(IntPtr lpBuffer, ref int lpnSize);
		
		/// <summary>
		/// Retrieves the name of the local computer.
		/// </summary>
		public static string GetComputerName()
		{
			string name = null;
			int size = MAX_COMPUTERNAME_LENGTH+1;

			IntPtr pName = Marshal.AllocCoTaskMem(size*2);
			
			if (GetComputerNameW(pName, ref size) != 0)
			{
				name = Marshal.PtrToStringUni(pName, size);
			}

			Marshal.FreeCoTaskMem(pName);

			return name;
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		private struct SOLE_AUTHENTICATION_SERVICE
		{
			public uint   dwAuthnSvc;
			public uint   dwAuthzSvc;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pPrincipalName;
			public int    hr;
		} 	
		
		private const uint RPC_C_AUTHN_NONE                = 0;
		private const uint RPC_C_AUTHN_DCE_PRIVATE         = 1;
		private const uint RPC_C_AUTHN_DCE_PUBLIC          = 2;
		private const uint RPC_C_AUTHN_DEC_PUBLIC          = 4;
		private const uint RPC_C_AUTHN_GSS_NEGOTIATE       = 9;
		private const uint RPC_C_AUTHN_WINNT               = 10;
		private const uint RPC_C_AUTHN_GSS_SCHANNEL        = 14;
		private const uint RPC_C_AUTHN_GSS_KERBEROS        = 16;
		private const uint RPC_C_AUTHN_DPA                 = 17;
		private const uint RPC_C_AUTHN_MSN                 = 18;
		private const uint RPC_C_AUTHN_DIGEST              = 21;
		private const uint RPC_C_AUTHN_MQ                  = 100;
		private const uint RPC_C_AUTHN_DEFAULT             = 0xFFFFFFFF;

		private const uint RPC_C_AUTHZ_NONE                = 0;
		private const uint RPC_C_AUTHZ_NAME                = 1;
		private const uint RPC_C_AUTHZ_DCE                 = 2;
		private const uint RPC_C_AUTHZ_DEFAULT             = 0xffffffff;

		private const uint RPC_C_AUTHN_LEVEL_DEFAULT       = 0;
		private const uint RPC_C_AUTHN_LEVEL_NONE          = 1;
		private const uint RPC_C_AUTHN_LEVEL_CONNECT       = 2;
		private const uint RPC_C_AUTHN_LEVEL_CALL          = 3;
		private const uint RPC_C_AUTHN_LEVEL_PKT           = 4;
		private const uint RPC_C_AUTHN_LEVEL_PKT_INTEGRITY = 5;
		private const uint RPC_C_AUTHN_LEVEL_PKT_PRIVACY   = 6;

		private const uint RPC_C_IMP_LEVEL_ANONYMOUS       = 1;
		private const uint RPC_C_IMP_LEVEL_IDENTIFY        = 2;
		private const uint RPC_C_IMP_LEVEL_IMPERSONATE     = 3;
		private const uint RPC_C_IMP_LEVEL_DELEGATE        = 4;
		
		private const uint EOAC_NONE	                   = 0x00;
		private const uint EOAC_MUTUAL_AUTH                = 0x01;
		private const uint EOAC_CLOAKING                   = 0x10;
		private const uint EOAC_SECURE_REFS	               = 0x02;
		private const uint EOAC_ACCESS_CONTROL             = 0x04;
		private const uint EOAC_APPID	                   = 0x08;

		[DllImport("ole32.dll")]
		private static extern int CoInitializeSecurity(
			IntPtr                        pSecDesc,
			int                           cAuthSvc,
			SOLE_AUTHENTICATION_SERVICE[] asAuthSvc,
			IntPtr                        pReserved1,
			uint                          dwAuthnLevel,
			uint                          dwImpLevel,
			IntPtr                        pAuthList,
			uint                          dwCapabilities,
			IntPtr                        pReserved3);
				
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		private struct COSERVERINFO
		{
			public uint         dwReserved1;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string       pwszName;
			public IntPtr       pAuthInfo;
			public uint         dwReserved2;
		};

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		private struct COAUTHINFO
		{
			public uint   dwAuthnSvc;
			public uint   dwAuthzSvc;
			public IntPtr pwszServerPrincName;
			public uint   dwAuthnLevel;
			public uint   dwImpersonationLevel;
			public IntPtr pAuthIdentityData;
			public uint   dwCapabilities;
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		private struct COAUTHIDENTITY
		{
			public IntPtr User;
			public uint   UserLength;
			public IntPtr Domain;
			public uint   DomainLength;
			public IntPtr Password;
			public uint   PasswordLength;
			public uint   Flags;
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
		private struct MULTI_QI
		{
			public IntPtr iid;
			[MarshalAs(UnmanagedType.IUnknown)]
			public object pItf;
			public uint   hr;
		}
		
		private const uint CLSCTX_INPROC_SERVER	= 0x1;
		private const uint CLSCTX_INPROC_HANDLER	= 0x2;
		private const uint CLSCTX_LOCAL_SERVER	= 0x4;
		private const uint CLSCTX_REMOTE_SERVER	= 0x10;

		private static readonly Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
		
		private const uint SEC_WINNT_AUTH_IDENTITY_ANSI    = 0x1;
		private const uint SEC_WINNT_AUTH_IDENTITY_UNICODE = 0x2;

		[DllImport("ole32.dll")]
		private static extern void CoCreateInstanceEx(
			ref Guid         clsid,
			[MarshalAs(UnmanagedType.IUnknown)]
			object           punkOuter,
			uint             dwClsCtx,
			[In]
			ref COSERVERINFO pServerInfo,
			uint             dwCount,
			[In, Out]
			MULTI_QI[]       pResults);

		/// <summary>
		/// Initializes COM security.
		/// </summary>
		public static void InitializeSecurity()
		{
			int error = CoInitializeSecurity(
				IntPtr.Zero,
				-1,
				null,
				IntPtr.Zero,
				RPC_C_AUTHN_LEVEL_NONE,
				RPC_C_IMP_LEVEL_IDENTIFY,
				IntPtr.Zero,
				EOAC_NONE,
				IntPtr.Zero);		

			if (error != 0)
			{
				throw new ExternalException("CoInitializeSecurity: " + GetSystemMessage(error), error);
			}
		}

		/// <summary>
		/// Creates an instance of a COM server.
		/// </summary>
		public static object CreateInstance(Guid clsid, string hostName, NetworkCredential credential)
		{
			string userName = null;
			string password = null;
			string domain   = null;

			if (credential != null)
			{
				userName = credential.UserName;
				password = credential.Password;
				domain   = credential.Domain; 
			}			

			GCHandle hUserName = GCHandle.Alloc(userName, GCHandleType.Pinned);
			GCHandle hPassword = GCHandle.Alloc(password, GCHandleType.Pinned);
			GCHandle hDomain   = GCHandle.Alloc(domain,   GCHandleType.Pinned);

			GCHandle hIdentity = new GCHandle();

			if (userName != null && userName != String.Empty)
			{
				COAUTHIDENTITY identity = new COAUTHIDENTITY();

				identity.User           = hUserName.AddrOfPinnedObject();
				identity.UserLength     = (uint)((userName != null)?userName.Length:0);
				identity.Password       = hPassword.AddrOfPinnedObject();
				identity.PasswordLength = (uint)((password != null)?password.Length:0);
				identity.Domain         = hDomain.AddrOfPinnedObject();
				identity.DomainLength   = (uint)((domain != null)?domain.Length:0);
				identity.Flags          = SEC_WINNT_AUTH_IDENTITY_UNICODE;

				hIdentity = GCHandle.Alloc(identity, GCHandleType.Pinned);
			}	

			COAUTHINFO authInfo           = new COAUTHINFO();
			authInfo.dwAuthnSvc           = RPC_C_AUTHN_WINNT;
			authInfo.dwAuthzSvc           = RPC_C_AUTHZ_NONE;
			authInfo.pwszServerPrincName  = IntPtr.Zero;
			authInfo.dwAuthnLevel         = RPC_C_AUTHN_LEVEL_CONNECT;
			authInfo.dwImpersonationLevel = RPC_C_IMP_LEVEL_IMPERSONATE;
			authInfo.pAuthIdentityData    = (hIdentity.IsAllocated)?hIdentity.AddrOfPinnedObject():IntPtr.Zero;
			authInfo.dwCapabilities       = EOAC_NONE;

			GCHandle hAuthInfo = GCHandle.Alloc(authInfo, GCHandleType.Pinned);

			COSERVERINFO serverInfo = new COSERVERINFO();
			serverInfo.pwszName     = hostName;
			serverInfo.pAuthInfo    = (credential != null)?hAuthInfo.AddrOfPinnedObject():IntPtr.Zero;
			serverInfo.dwReserved1  = 0;
			serverInfo.dwReserved2  = 0;

			GCHandle hClsid = GCHandle.Alloc(IID_IUnknown, GCHandleType.Pinned);

			MULTI_QI[] results = new MULTI_QI[1];

			results[0].iid  = hClsid.AddrOfPinnedObject();
			results[0].pItf = null;
			results[0].hr   = 0;

			try
			{
				CoCreateInstanceEx(
					ref clsid,
					null,
					CLSCTX_INPROC_HANDLER | CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
					ref serverInfo,
					1,
					results);		
			}
			catch (Exception e)
			{
				if (hUserName.IsAllocated) hUserName.Free();
				if (hPassword.IsAllocated) hPassword.Free();
				if (hDomain.IsAllocated)   hDomain.Free();
				if (hIdentity.IsAllocated) hIdentity.Free();
				if (hAuthInfo.IsAllocated) hAuthInfo.Free();
				if (hClsid.IsAllocated)    hClsid.Free();
				throw e;
			}

			if (hUserName.IsAllocated) hUserName.Free();
			if (hPassword.IsAllocated) hPassword.Free();
			if (hDomain.IsAllocated)   hDomain.Free();
			if (hIdentity.IsAllocated) hIdentity.Free();
			if (hAuthInfo.IsAllocated) hAuthInfo.Free();
			if (hClsid.IsAllocated)    hClsid.Free();
			  
			try 
			{
				if (results[0].hr != 0)
				{
					throw new ExternalException("CoCreateInstanceEx: " + GetSystemMessage((int)results[0].hr));				
				}				
			}
			catch 
			{
				//MessageBox.Show(this, "opcEnum Service does not registered.", "opcEnum Register Error");
				return null;			// opcEnum 이 서비스로 등록되지 않았다
			}			
			return results[0].pItf;
		}

		/// <summary>
		/// Unmarshals and frees an array of 32 bit integers.
		/// </summary>
		public static int[] GetInt32s(ref IntPtr pArray, int size, bool deallocate)
		{
			if (pArray == IntPtr.Zero || size <= 0)
			{
				return null;
			}

			int[] array = new int[size];
			Marshal.Copy(pArray, array, 0, size);

			if (deallocate)
			{
				Marshal.FreeCoTaskMem(pArray);
				pArray = IntPtr.Zero;
			}

			return array;
		}

		/// <summary>
		/// Unmarshals and frees a array of 16 bit integers.
		/// </summary>
		public static short[] GetInt16s(ref IntPtr pArray, int size, bool deallocate)
		{
			if (pArray == IntPtr.Zero || size <= 0)
			{
				return null;
			}

			short[] array = new short[size];
			Marshal.Copy(pArray, array, 0, size);

			if (deallocate)
			{
				Marshal.FreeCoTaskMem(pArray);
				pArray = IntPtr.Zero;
			}

			return array;
		}

		/// <summary>
		/// Marshals an array of strings into a unmanaged memory buffer
		/// </summary>
		/// <param name="values">The array of strings to marshal</param>
		/// <returns>The pointer to the unmanaged memory buffer</returns>
		public static IntPtr GetUnicodeStrings(string[] values)
		{
			int size = (values != null)?values.Length:0;

			if (size <= 0)
			{
				return IntPtr.Zero;
			}

			IntPtr pValues = IntPtr.Zero;

			int[] pointers = new int[size];
			
			for (int ii = 0; ii < size; ii++)
			{
				pointers[ii] = (int)Marshal.StringToCoTaskMemUni(values[ii]);
			}

			pValues = Marshal.AllocCoTaskMem(values.Length*Marshal.SizeOf(typeof(IntPtr)));
			Marshal.Copy(pointers, 0, pValues, size);

			return pValues;
		}

		/// <summary>
		/// Unmarshals and frees a array of unicode strings.
		/// </summary>
		public static string[] GetUnicodeStrings(ref IntPtr pArray, int size, bool deallocate)
		{
			if (pArray == IntPtr.Zero || size <= 0)
			{
				return null;
			}

			int[] pointers = new int[size];
			Marshal.Copy(pArray, pointers, 0, size);

			string[] strings = new string[size];

			for (int ii = 0; ii < size; ii++)
			{
				IntPtr pString = (IntPtr)pointers[ii];
				strings[ii] = Marshal.PtrToStringUni(pString);
				if (deallocate) Marshal.FreeCoTaskMem(pString);
			}

			if (deallocate)
			{
				Marshal.FreeCoTaskMem(pArray);
				pArray = IntPtr.Zero;
			}

			return strings;
		}
		
		/// <summary>
		/// Marshals a DateTime as a WIN32 FILETIME.
		/// </summary>
		/// <param name="datetime">The DateTime object to marshal</param>
		/// <returns>The WIN32 FILETIME</returns>
        public static System.Runtime.InteropServices.ComTypes.FILETIME GetFILETIME(DateTime datetime)
		{
            System.Runtime.InteropServices.ComTypes.FILETIME filetime;

			DateTime minValue = new DateTime(1601, 1, 1);

			if (datetime <= minValue)
			{
				filetime.dwHighDateTime = 0;
				filetime.dwLowDateTime  = 0;
				return filetime;
			}

			// adjust for WIN32 FILETIME base.
			long ticks = (datetime.ToUniversalTime().Subtract(new TimeSpan(minValue.Ticks))).Ticks;

			filetime.dwHighDateTime = (int)((ticks>>32) & 0xFFFFFFFF);
			filetime.dwLowDateTime  = (int)(ticks & 0xFFFFFFFF);

			return filetime;
		}

		/// <summary>
		/// Unmarshals a WIN32 FILETIME from a pointer.
		/// </summary>
		/// <param name="pFiletime">A pointer to a FILETIME structure.</param>
		/// <returns>A DateTime object.</returns>
		public static DateTime GetFILETIME(IntPtr pFiletime)
		{
			if (pFiletime == IntPtr.Zero)
			{
				return DateTime.MinValue;
			}

            return GetFILETIME((System.Runtime.InteropServices.ComTypes.FILETIME)Marshal.PtrToStructure(pFiletime, typeof(System.Runtime.InteropServices.ComTypes.FILETIME)));
		}

		/// <summary>
		/// Unmarshals a WIN32 FILETIME.
		/// </summary>
        public static DateTime GetFILETIME(System.Runtime.InteropServices.ComTypes.FILETIME filetime)
		{
			// convert FILETIME structure to a 64 bit integer.
			long buffer = (long)filetime.dwHighDateTime;

			if (buffer < 0)
			{
				buffer += ((long)UInt32.MaxValue+1);
			}

			long ticks = (buffer<<32);

			buffer = (long)filetime.dwLowDateTime;

			if (buffer < 0)
			{
				buffer += ((long)UInt32.MaxValue+1);
			}

			ticks += buffer;

			// check for invalid value.
			if (ticks == 0)
			{
				return DateTime.MinValue;
			}

			// adjust for WIN32 FILETIME base.
			return new DateTime(1601, 1, 1).Add(new TimeSpan(ticks)).ToLocalTime();
		}

		/// <summary>
		/// Marshals an array of DateTimes into an unmanaged array of FILETIMEs
		/// </summary>
		/// <param name="datetimes">The array of DateTimes to marshal</param>
		/// <returns>The IntPtr array of FILETIMEs</returns>
		public static IntPtr GetFILETIMEs(DateTime[] datetimes)
		{
			int count = (datetimes != null)?datetimes.Length:0;

			if (count <= 0)
			{
				return IntPtr.Zero;
			}

            IntPtr pFiletimes = Marshal.AllocCoTaskMem(count * Marshal.SizeOf(typeof(System.Runtime.InteropServices.ComTypes.FILETIME)));

			IntPtr pos = pFiletimes;

			for (int ii = 0; ii < count; ii++)
			{
				Marshal.StructureToPtr(OpcCom.Interop.GetFILETIME(datetimes[ii]), pos, false);
                pos = (IntPtr)(pos.ToInt32() + Marshal.SizeOf(typeof(System.Runtime.InteropServices.ComTypes.FILETIME)));
			}

			return pFiletimes;
		}	

		/// <summary>
		/// Unmarshals an array of WIN32 FILETIMEs as DateTimes.
		/// </summary>
		public static DateTime[] GetFILETIMEs(ref IntPtr pArray, int size, bool deallocate)
		{
			if (pArray == IntPtr.Zero || size <= 0)
			{
				return null;
			}

			DateTime[] datetimes = new DateTime[size];

			IntPtr pos = pArray;

			for (int ii = 0; ii < size; ii++)
			{
				datetimes[ii] = OpcCom.Interop.GetFILETIME(pos);
                pos = (IntPtr)(pos.ToInt32() + Marshal.SizeOf(typeof(System.Runtime.InteropServices.ComTypes.FILETIME)));
			}

			if (deallocate)
			{
				Marshal.FreeCoTaskMem(pArray);
				pArray = IntPtr.Zero;
			}

			return datetimes;
		}	

		/// <summary>
		/// The size, in bytes, of a VARIANT structure.
		/// </summary>
		private const int VARIANT_SIZE = 0x10;

		/// <summary>
		/// Frees all memory referenced by a VARIANT stored in unmanaged memory.
		/// </summary>
		[DllImport("oleaut32.dll")]
		public static extern void VariantClear(IntPtr pVariant);

		/// <summary>
		/// Converts an object into a value that can be marshalled to a VARIANT.
		/// </summary>
		/// <param name="source">The object to convert.</param>
		/// <returns>The converted object.</returns>
		public static object GetVARIANT(object source)
		{
			// check for invalid args.
			if (source == null || source.GetType() == null)
			{
				return null;
			}

			// convert a decimal array to an object array since decimal arrays can't be converted to a variant.
			if (source.GetType() == typeof(decimal[]))
			{
				decimal[] srcArray = (decimal[])source;
				object[]  dstArray = new object[srcArray.Length];

				for (int ii = 0; ii < srcArray.Length; ii++)
				{
					try
					{
						dstArray[ii] = (object)srcArray[ii];
					}
					catch (Exception)
					{
						dstArray[ii] = Double.NaN;
					}
				}

				return dstArray;
			}

			// no conversion required.
			return source;
		}

		/// <summary>
		/// Marshals an array objects into an unmanaged array of VARIANTs.
		/// </summary>
		/// <param name="values">An array of the objects to be marshalled</param>
		/// <returns>An pointer to the array in unmanaged memory</returns>
		public static IntPtr GetVARIANTs(object[] values)
		{
			int count = (values != null)?values.Length:0;

			if (count <= 0)
			{
				return IntPtr.Zero;
			}

			IntPtr pValues = Marshal.AllocCoTaskMem(count*OpcCom.Interop.VARIANT_SIZE);

			IntPtr pos = pValues;

			for (int ii = 0; ii < count; ii++)
			{
				Marshal.GetNativeVariantForObject(OpcCom.Interop.GetVARIANT(values[ii]), pos);
				pos = (IntPtr)(pos.ToInt32() + OpcCom.Interop.VARIANT_SIZE);
			}

			return pValues;
		}	

		/// <summary>
		/// Unmarshals an array of VARIANTs as objects.
		/// </summary>
		public static object[] GetVARIANTs(ref IntPtr pArray, int size, bool deallocate)
		{
			// this method unmarshals VARIANTs one at a time because a single bad value throws 
			// an exception with GetObjectsForNativeVariants(). This approach simply sets the 
			// offending value to null.

			if (pArray == IntPtr.Zero || size <= 0)
			{
				return null;
			}

			object[] values = new object[size];

			IntPtr pos = pArray;

			for (int ii = 0; ii < size; ii++)
			{
				try 
				{
					values[ii] = Marshal.GetObjectForNativeVariant(pos);
					if (deallocate) OpcCom.Interop.VariantClear(pos);
				}
				catch (Exception)
				{
					values[ii] = null;
				}

				pos = (IntPtr)(pos.ToInt32() + VARIANT_SIZE);
			}

			if (deallocate)
			{
				Marshal.FreeCoTaskMem(pArray);
				pArray = IntPtr.Zero;
			}

			return values;
		}	

		/// <summary>
		/// Converts a LCID to a Locale string.
		/// </summary>
		internal static string GetLocale(int input)
		{
			CultureInfo locale = null;

			try   { locale = new CultureInfo(input); }
			catch { locale = CultureInfo.InvariantCulture; }
		
			return locale.Name;
		}

		/// <summary>
		/// Converts a Locale string to a LCID.
		/// </summary>
		internal static int GetLocale(string input)
		{
			// check for the default culture.
			if (input == null || input == "")
			{
				return 0;
			}

			CultureInfo locale = null;

			try   { locale = new CultureInfo(input); }
			catch { locale = CultureInfo.CurrentCulture; }
		
			return locale.LCID;
		}

		/// <summary>
		/// Converts the VARTYPE to a system type.
		/// </summary>
		internal static System.Type GetType(VarEnum input)
		{				
			switch (input)
			{
				case VarEnum.VT_I1:                         return typeof(sbyte);
				case VarEnum.VT_UI1:                        return typeof(byte);
				case VarEnum.VT_I2:                         return typeof(short);
				case VarEnum.VT_UI2:                        return typeof(ushort);
				case VarEnum.VT_I4:                         return typeof(int);
				case VarEnum.VT_UI4:                        return typeof(uint);
				case VarEnum.VT_R4:                         return typeof(float);
				case VarEnum.VT_R8:                         return typeof(double);
				case VarEnum.VT_CY:                         return typeof(decimal);
				case VarEnum.VT_BOOL:                       return typeof(bool);
				case VarEnum.VT_DATE:                       return typeof(DateTime);
				case VarEnum.VT_BSTR:                       return typeof(string);
				case VarEnum.VT_ARRAY | VarEnum.VT_I1:      return typeof(sbyte[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI1:     return typeof(byte[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_I2:      return typeof(short[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI2:     return typeof(ushort[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_I4:      return typeof(int[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_UI4:     return typeof(uint[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_R4:      return typeof(float[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_R8:      return typeof(double[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_CY:      return typeof(decimal[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_BOOL:    return typeof(bool[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_DATE:    return typeof(DateTime[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_BSTR:    return typeof(string[]);
				case VarEnum.VT_ARRAY | VarEnum.VT_VARIANT: return typeof(object[]);
				default:                                    return typeof(object);
			}
		}

		/// <summary>
		/// Converts the system type to a VARTYPE.
		/// </summary>
		internal static VarEnum GetType(System.Type input)
		{				
			if (input == null)               return VarEnum.VT_EMPTY;
			if (input == typeof(sbyte))      return VarEnum.VT_I1;
			if (input == typeof(byte))       return VarEnum.VT_UI1;
			if (input == typeof(short))      return VarEnum.VT_I2;
			if (input == typeof(ushort))     return VarEnum.VT_UI2;
			if (input == typeof(int))        return VarEnum.VT_I4;
			if (input == typeof(uint))       return VarEnum.VT_UI4;
			if (input == typeof(long))       return VarEnum.VT_I4;
			if (input == typeof(ulong))      return VarEnum.VT_UI4;
			if (input == typeof(float))      return VarEnum.VT_R4;
			if (input == typeof(double))     return VarEnum.VT_R8;
			if (input == typeof(decimal))    return VarEnum.VT_CY;
			if (input == typeof(bool))       return VarEnum.VT_BOOL;
			if (input == typeof(DateTime))   return VarEnum.VT_DATE;
			if (input == typeof(string))     return VarEnum.VT_BSTR;
			if (input == typeof(object))     return VarEnum.VT_EMPTY;
			if (input == typeof(sbyte[]))    return VarEnum.VT_ARRAY | VarEnum.VT_I1;
			if (input == typeof(byte[]))     return VarEnum.VT_ARRAY | VarEnum.VT_UI1;
			if (input == typeof(short[]))    return VarEnum.VT_ARRAY | VarEnum.VT_I2;
			if (input == typeof(ushort[]))   return VarEnum.VT_ARRAY | VarEnum.VT_UI2;
			if (input == typeof(int[]))      return VarEnum.VT_ARRAY | VarEnum.VT_I4;
			if (input == typeof(uint[]))     return VarEnum.VT_ARRAY | VarEnum.VT_UI4;
			if (input == typeof(long[]))     return VarEnum.VT_ARRAY | VarEnum.VT_I4;
			if (input == typeof(ulong[]))    return VarEnum.VT_ARRAY | VarEnum.VT_UI4;
			if (input == typeof(float[]))    return VarEnum.VT_ARRAY | VarEnum.VT_R4;
			if (input == typeof(double[]))   return VarEnum.VT_ARRAY | VarEnum.VT_R8;
			if (input == typeof(decimal[]))  return VarEnum.VT_ARRAY | VarEnum.VT_CY;
			if (input == typeof(bool[]))     return VarEnum.VT_ARRAY | VarEnum.VT_BOOL;
			if (input == typeof(DateTime[])) return VarEnum.VT_ARRAY | VarEnum.VT_DATE;
			if (input == typeof(string[]))   return VarEnum.VT_ARRAY | VarEnum.VT_BSTR;
			if (input == typeof(object[]))   return VarEnum.VT_ARRAY | VarEnum.VT_VARIANT;
			
			return VarEnum.VT_EMPTY;
		}

		/// <summary>
		/// Converts the HRESULT to a system type.
		/// </summary>
		internal static ResultID GetResultID(int input)
		{				
			switch (input)
			{
				// data access.
				case Da.ResultIDs.S_OK:                       return new ResultID(ResultID.S_OK,                       input);
				case Da.ResultIDs.E_FAIL:                     return new ResultID(ResultID.E_FAIL,                     input);
				case Da.ResultIDs.DISP_E_TYPEMISMATCH:        return new ResultID(ResultID.E_BADTYPE,                  input);
				case Da.ResultIDs.DISP_E_OVERFLOW:            return new ResultID(ResultID.E_RANGE,                    input);
				case Da.ResultIDs.E_OUTOFMEMORY:              return new ResultID(ResultID.E_OUTOFMEMORY,              input);
				case Da.ResultIDs.E_INVALIDHANDLE:            return new ResultID(ResultID.E_INVALIDHANDLE,            input);
				case Da.ResultIDs.E_BADTYPE:                  return new ResultID(ResultID.E_BADTYPE,                  input);
				case Da.ResultIDs.E_UNKNOWNITEMID:            return new ResultID(ResultID.E_UNKNOWN_ITEM_NAME,        input);
				case Da.ResultIDs.E_INVALIDITEMID:            return new ResultID(ResultID.E_INVALID_ITEM_NAME,        input);
				case Da.ResultIDs.E_UNKNOWNPATH:              return new ResultID(ResultID.E_UNKNOWN_ITEM_PATH,        input);
				case Da.ResultIDs.E_INVALIDFILTER:            return new ResultID(ResultID.E_INVALID_FILTER,           input);
				case Da.ResultIDs.E_RANGE:                    return new ResultID(ResultID.E_RANGE,                    input);
				case Da.ResultIDs.S_UNSUPPORTEDRATE:          return new ResultID(ResultID.S_UNSUPPORTEDRATE,          input);
				case Da.ResultIDs.S_CLAMP:                    return new ResultID(ResultID.S_CLAMP,                    input);
				case Da.ResultIDs.E_INVALID_PID:              return new ResultID(ResultID.E_INVALID_PID,              input);
				case Da.ResultIDs.E_DEADBANDNOTSUPPORTED:     return new ResultID(ResultID.E_NO_ITEM_DEADBAND,         input);
				case Da.ResultIDs.E_NOBUFFERING:              return new ResultID(ResultID.E_NO_ITEM_BUFFERING,        input);
				case Da.ResultIDs.E_NOTSUPPORTED:             return new ResultID(ResultID.E_NO_WRITEQT,               input);
				case Da.ResultIDs.E_INVALIDCONTINUATIONPOINT: return new ResultID(ResultID.E_INVALIDCONTINUATIONPOINT, input);
				case Da.ResultIDs.S_DATAQUEUEOVERFLOW:        return new ResultID(ResultID.S_DATAQUEUEOVERFLOW,        input);

				// complex data.
				case Cpx.ResultIDs.E_TYPE_CHANGED:         return new ResultID(ResultID.E_TYPE_CHANGED,     input);
				case Cpx.ResultIDs.E_FILTER_DUPLICATE:     return new ResultID(ResultID.E_FILTER_DUPLICATE, input);
				case Cpx.ResultIDs.E_FILTER_INVALID:       return new ResultID(ResultID.E_FILTER_INVALID,   input);
				case Cpx.ResultIDs.E_FILTER_ERROR:         return new ResultID(ResultID.E_FILTER_ERROR,     input);
				case Cpx.ResultIDs.S_FILTER_NO_DATA:       return new ResultID(ResultID.S_FILTER_NO_DATA,   input);
				
				// historical data access.
				case Hda.ResultIDs.E_MAXEXCEEDED:      return new ResultID(ResultID.E_MAXEXCEEDED, input);
				case Hda.ResultIDs.S_NODATA:           return new ResultID(ResultID.S_NODATA, input);
				case Hda.ResultIDs.S_MOREDATA:         return new ResultID(ResultID.S_MOREDATA, input);
				case Hda.ResultIDs.E_INVALIDAGGREGATE: return new ResultID(ResultID.E_INVALIDAGGREGATE, input);
				case Hda.ResultIDs.S_CURRENTVALUE:     return new ResultID(ResultID.S_CURRENTVALUE, input);
				case Hda.ResultIDs.S_EXTRADATA:        return new ResultID(ResultID.S_EXTRADATA, input);
				case Hda.ResultIDs.W_NOFILTER:         return new ResultID(ResultID.W_NOFILTER, input);
				case Hda.ResultIDs.E_UNKNOWNATTRID:    return new ResultID(ResultID.E_UNKNOWNATTRID, input);
				case Hda.ResultIDs.E_NOT_AVAIL:        return new ResultID(ResultID.E_NOT_AVAIL, input);
				case Hda.ResultIDs.E_INVALIDDATATYPE:  return new ResultID(ResultID.E_INVALIDDATATYPE, input);
				case Hda.ResultIDs.E_DATAEXISTS:       return new ResultID(ResultID.E_DATAEXISTS, input);
				case Hda.ResultIDs.E_INVALIDATTRID:    return new ResultID(ResultID.E_INVALIDATTRID, input);
				case Hda.ResultIDs.E_NODATAEXISTS:     return new ResultID(ResultID.E_NODATAEXISTS, input);
				case Hda.ResultIDs.S_INSERTED:         return new ResultID(ResultID.S_INSERTED, input);
				case Hda.ResultIDs.S_REPLACED:         return new ResultID(ResultID.S_REPLACED, input);

				default:                            
				{
					// check for RPC error.
					if ((input & 0x7FFF0000) == 0x00010000)
					{
						return new ResultID(ResultID.E_NETWORK_ERROR, input);
					}
					
					// chekc for success code.
					if (input >= 0)	
					{
						return new ResultID(ResultID.S_FALSE, input);
					}

					// return generic error.
					return new ResultID(ResultID.E_FAIL, input);
				}
			}
		}
		
		/// <summary>
		/// Returns an exception after extracting HRESULT from the exception.
		/// </summary>
		public static Exception CreateException(string message, Exception e)
		{	
			return CreateException(message, Marshal.GetHRForException(e));
		}

		/// <summary>
		/// Returns an exception after extracting HRESULT from the exception.
		/// </summary>
		public static Exception CreateException(string message, int code)
		{	
			return new Opc.ResultIDException(OpcCom.Interop.GetResultID(code), message);
		}
	}
}
