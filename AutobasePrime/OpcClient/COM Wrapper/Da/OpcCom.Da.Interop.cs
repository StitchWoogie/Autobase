//============================================================================
// TITLE: OpcCom.Da.Interop.cs
//
// CONTENTS:
// 
// An object to handle asynchronous requests.
//
// (c) Copyright 2003 The OPC Foundation
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
// 2003/04/03 RSA   Initial implementation.

using System;
using System.Threading;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Reflection;
using Opc;
using Opc.Da;

namespace OpcCom.Da
{
	/// <summary>
	/// Contains state information for a single asynchronous OpcCom.Da.Interop.
	/// </summary>
	public class Interop
	{		
		/// <summary>
		/// Converts a standard FILETIME to an OpcRcw.Da.FILETIME structure.
		/// </summary>
        internal static OpcRcw.Da.FILETIME Convert(System.Runtime.InteropServices.ComTypes.FILETIME input)
		{
			OpcRcw.Da.FILETIME output = new OpcRcw.Da.FILETIME();
			output.dwLowDateTime   = input.dwLowDateTime;
			output.dwHighDateTime  = input.dwHighDateTime;
			return output;
		}

		/// <summary>
		/// Converts an OpcRcw.Da.FILETIME to a standard FILETIME structure.
		/// </summary>
        internal static System.Runtime.InteropServices.ComTypes.FILETIME Convert(OpcRcw.Da.FILETIME input)
		{
            System.Runtime.InteropServices.ComTypes.FILETIME output = new System.Runtime.InteropServices.ComTypes.FILETIME();
			output.dwLowDateTime  = input.dwLowDateTime;
			output.dwHighDateTime = input.dwHighDateTime;
			return output;
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
		/// Unmarshals and deallocates a OPCSERVERSTATUS structure.
		/// </summary>
		internal static ServerStatus GetServerStatus(ref IntPtr pInput, bool deallocate)
		{
			ServerStatus output = null;
			
			if (pInput != IntPtr.Zero)
			{
				OpcRcw.Da.OPCSERVERSTATUS status = (OpcRcw.Da.OPCSERVERSTATUS)Marshal.PtrToStructure(pInput, typeof(OpcRcw.Da.OPCSERVERSTATUS));
		
				output = new ServerStatus();

				output.VendorInfo     = status.szVendorInfo;
				output.ProductVersion = String.Format("{0}.{1}.{2}", status.wMajorVersion, status.wMinorVersion, status.wBuildNumber);
				output.ServerState    = (serverState)status.dwServerState;
				output.StatusInfo     = null;
				output.StartTime      = OpcCom.Interop.GetFILETIME(Convert(status.ftStartTime));
				output.CurrentTime    = OpcCom.Interop.GetFILETIME(Convert(status.ftCurrentTime));
				output.LastUpdateTime = OpcCom.Interop.GetFILETIME(Convert(status.ftLastUpdateTime));

				if (deallocate)
				{
					Marshal.FreeCoTaskMem(pInput);
					pInput = IntPtr.Zero;
				}
			}

			return output;
		}

		/// <summary>
		/// Converts a browseFilter values to the COM equivalent.
		/// </summary>
		internal static OpcRcw.Da.OPCBROWSEFILTER GetBrowseFilter(browseFilter input)
		{			
			switch (input)
			{
				case browseFilter.all:    return OpcRcw.Da.OPCBROWSEFILTER.OPC_BROWSE_FILTER_ALL;
				case browseFilter.branch: return OpcRcw.Da.OPCBROWSEFILTER.OPC_BROWSE_FILTER_BRANCHES;
				case browseFilter.item:   return OpcRcw.Da.OPCBROWSEFILTER.OPC_BROWSE_FILTER_ITEMS;
			}

			return OpcRcw.Da.OPCBROWSEFILTER.OPC_BROWSE_FILTER_ALL;
		}

		/// <summary>
		/// Unmarshals and deallocates an array of OPCBROWSEELEMENT structures.
		/// </summary>
		internal static BrowseElement[] GetBrowseElements(ref IntPtr pInput, int count, bool deallocate)
		{
			BrowseElement[] output = null;
			
			if (pInput != IntPtr.Zero && count > 0)
			{
				output = new BrowseElement[count];

				IntPtr pos = pInput;

				for (int ii = 0; ii < count; ii++)
				{
					output[ii] = GetBrowseElement(pos, deallocate);
					pos = (IntPtr)(pos.ToInt32() + Marshal.SizeOf(typeof(OpcRcw.Da.OPCBROWSEELEMENT)));
				}

				if (deallocate)
				{
					Marshal.FreeCoTaskMem(pInput);
					pInput = IntPtr.Zero;
				}
			}

			return output;
		}

		/// <summary>
		/// Unmarshals and deallocates a OPCBROWSEELEMENT structures.
		/// </summary>
		internal static BrowseElement GetBrowseElement(IntPtr pInput, bool deallocate)
		{
			BrowseElement output = null;
			
			if (pInput != IntPtr.Zero)
			{
				OpcRcw.Da.OPCBROWSEELEMENT element = (OpcRcw.Da.OPCBROWSEELEMENT)Marshal.PtrToStructure(pInput, typeof(OpcRcw.Da.OPCBROWSEELEMENT));
		
				output = new BrowseElement();

				output.Name        = element.szName;
				output.ItemPath    = null;
				output.ItemName    = element.szItemID;
				output.IsItem      = ((element.dwFlagValue & OpcRcw.Da.Constants.OPC_BROWSE_ISITEM) != 0);
				output.HasChildren = ((element.dwFlagValue & OpcRcw.Da.Constants.OPC_BROWSE_HASCHILDREN) != 0);
				output.Properties  = GetItemProperties(ref element.ItemProperties, deallocate);
			}

			return output;
		}

		/// <summary>
		/// Creates an array of property codes.
		/// </summary>
		internal static int[] GetPropertyIDs(PropertyID[] propertyIDs)
		{
			ArrayList output = new ArrayList();

			if (propertyIDs != null)
			{
				foreach (PropertyID propertyID in propertyIDs)
				{
					output.Add(propertyID.Code);
				}
			}

			return (int[])output.ToArray(typeof(int));
		}

		/// <summary>
		/// Unmarshals and deallocates an array of OPCITEMPROPERTIES structures.
		/// </summary>
		internal static ItemPropertyCollection[] GetItemPropertyCollections(ref IntPtr pInput, int count, bool deallocate)
		{
			ItemPropertyCollection[] output = null;
			
			if (pInput != IntPtr.Zero && count > 0)
			{
				output = new ItemPropertyCollection[count];

				IntPtr pos = pInput;

				for (int ii = 0; ii < count; ii++)
				{
					OpcRcw.Da.OPCITEMPROPERTIES list = (OpcRcw.Da.OPCITEMPROPERTIES)Marshal.PtrToStructure(pos, typeof(OpcRcw.Da.OPCITEMPROPERTIES));

					output[ii]          = new ItemPropertyCollection();
					output[ii].ItemPath = null;
					output[ii].ItemName = null;
					output[ii].ResultID = OpcCom.Interop.GetResultID(list.hrErrorID);

					ItemProperty[] properties = GetItemProperties(ref list, deallocate);

					if (properties != null)
					{
						output[ii].AddRange(properties);
					}
					
					pos = (IntPtr)(pos.ToInt32() + Marshal.SizeOf(typeof(OpcRcw.Da.OPCITEMPROPERTIES)));
				}

				if (deallocate)
				{
					Marshal.FreeCoTaskMem(pInput);
					pInput = IntPtr.Zero;
				}
			}

			return output;
		}

		/// <summary>
		/// Unmarshals and deallocates a OPCITEMPROPERTIES structures.
		/// </summary>
		internal static ItemProperty[] GetItemProperties(ref OpcRcw.Da.OPCITEMPROPERTIES input, bool deallocate)
		{
			ItemProperty[] output = null;
			
			if (input.dwNumProperties > 0)
			{
				output = new ItemProperty[input.dwNumProperties];

				IntPtr pos = input.pItemProperties;

				for (int ii = 0; ii < output.Length; ii++)
				{
					output[ii] = GetItemProperty(pos, deallocate);
					pos = (IntPtr)(pos.ToInt32() + Marshal.SizeOf(typeof(OpcRcw.Da.OPCITEMPROPERTY)));
				}

				if (deallocate)
				{
					Marshal.FreeCoTaskMem(input.pItemProperties);
					input.pItemProperties = IntPtr.Zero;
				}
			}

			return output;
		}

		/// <summary>
		/// Unmarshals and deallocates a OPCITEMPROPERTY structures.
		/// </summary>
		internal static ItemProperty GetItemProperty(IntPtr pInput, bool deallocate)
		{
			ItemProperty output = null;
						
			if (pInput != IntPtr.Zero)
			{
				OpcRcw.Da.OPCITEMPROPERTY property = (OpcRcw.Da.OPCITEMPROPERTY)Marshal.PtrToStructure(pInput, typeof(OpcRcw.Da.OPCITEMPROPERTY));
		
				output = new ItemProperty();

				output.ID          = GetPropertyID(property.dwPropertyID);
				output.Description = property.szDescription;
				output.ItemPath    = null;
				output.ItemName    = property.szItemID;
				output.Value       = GetPropertyValue(output.ID, property.vValue);
				output.ResultID    = OpcCom.Interop.GetResultID(property.hrErrorID);

				// convert COM DA code to unified DA code.
				if (property.hrErrorID == ResultIDs.E_BADRIGHTS) output.ResultID = new ResultID(ResultID.E_WRITEONLY, ResultIDs.E_BADRIGHTS);
			}

			return output;
		}

		/// <remarks/>
		public static PropertyID GetPropertyID(int input)
		{
			FieldInfo[] fields = typeof(Opc.Da.Property).GetFields(BindingFlags.Static | BindingFlags.Public);

			foreach (FieldInfo field in fields)
			{
				PropertyID property = (PropertyID)field.GetValue(typeof(PropertyID));

				if (input == property.Code)
				{
					return property;
				}
			}

			return new PropertyID(input);
		}

		/// <summary>
		/// Converts the property value to a type supported by the unified interface.
		/// </summary>
		internal static object GetPropertyValue(PropertyID propertyID, object input)
		{	
			if (input == null) return null;
					
			try
			{
				if (propertyID == Property.DATATYPE)
				{
					return OpcCom.Interop.GetType((VarEnum)System.Convert.ToUInt16(input));
				}

				if (propertyID == Property.ACCESSRIGHTS)
				{
					switch ((int)input)
					{
						case OpcRcw.Da.Constants.OPC_READABLE:  return accessRights.readable;
						case OpcRcw.Da.Constants.OPC_WRITEABLE: return accessRights.writable;
						
						case OpcRcw.Da.Constants.OPC_READABLE | OpcRcw.Da.Constants.OPC_WRITEABLE: 
						{
							return accessRights.readWritable;
						}
					}

					return null;
				}

				if (propertyID == Property.EUTYPE)
				{
					switch ((OpcRcw.Da.OPCEUTYPE)input)
					{
						case OpcRcw.Da.OPCEUTYPE.OPC_NOENUM:     return euType.noEnum;
						case OpcRcw.Da.OPCEUTYPE.OPC_ANALOG:     return euType.analog;
						case OpcRcw.Da.OPCEUTYPE.OPC_ENUMERATED: return euType.enumerated;
					}

					return null;
				}

				if (propertyID == Property.QUALITY)
				{
					return new Opc.Da.Quality(System.Convert.ToInt16(input));
				}

				// convert UTC time in property to local time for the unified DA interface.
				if (propertyID == Property.TIMESTAMP)
				{
					if (input.GetType() == typeof(DateTime))
					{
						return ((DateTime)input).ToLocalTime();
					}
				}
			}
			catch {}

			return input;
		}
		
		/// <summary>
		/// Converts an array of item values to an array of OPCITEMVQT objects.
		/// </summary>
		internal static OpcRcw.Da.OPCITEMVQT[] GetOPCITEMVQTs(ItemValue[] input)
		{
			OpcRcw.Da.OPCITEMVQT[] output = null;

			if (input != null)
			{
				output = new OpcRcw.Da.OPCITEMVQT[input.Length];

				for (int ii = 0; ii < input.Length; ii++)
				{
					output[ii] = new OpcRcw.Da.OPCITEMVQT();

					DateTime timestamp = (input[ii].TimestampSpecified)?input[ii].Timestamp:DateTime.MinValue;	

					output[ii].vDataValue          = OpcCom.Interop.GetVARIANT(input[ii].Value);
					output[ii].bQualitySpecified   = (input[ii].QualitySpecified)?1:0;
					output[ii].wQuality            = (input[ii].QualitySpecified)?input[ii].Quality.GetCode():(short)0;
					output[ii].bTimeStampSpecified = (input[ii].TimestampSpecified)?1:0;
					output[ii].ftTimeStamp         = OpcCom.Da.Interop.Convert(OpcCom.Interop.GetFILETIME(timestamp));
				}

			}

			return output;
		}

		/// <summary>
		/// Converts an array of item objects to an array of GetOPCITEMDEF objects.
		/// </summary>
		internal static OpcRcw.Da.OPCITEMDEF[] GetOPCITEMDEFs(Item[] input)
		{
			OpcRcw.Da.OPCITEMDEF[] output = null;

			if (input != null)
			{
				output = new OpcRcw.Da.OPCITEMDEF[input.Length];

				for (int ii = 0; ii < input.Length; ii++)
				{
					output[ii] = new OpcRcw.Da.OPCITEMDEF();

					output[ii].szItemID            = input[ii].ItemName;
					output[ii].szAccessPath        = input[ii].ItemPath;
					output[ii].bActive             = (input[ii].ActiveSpecified)?((input[ii].Active)?1:0):1;
					output[ii].vtRequestedDataType = (short)OpcCom.Interop.GetType(input[ii].ReqType);
					output[ii].hClient             = 0;
					output[ii].dwBlobSize          = 0;
					output[ii].pBlob               = IntPtr.Zero;
				}
			}

			return output;
		}

		/// <summary>
		/// Unmarshals and deallocates a OPCITEMSTATE structures.
		/// </summary>
		internal static ItemValue[] GetItemValues(ref IntPtr pInput, int count, bool deallocate)
		{
			ItemValue[] output = null;
			
			if (pInput != IntPtr.Zero && count > 0)
			{
				output = new ItemValue[count];

				IntPtr pos = pInput;

				for (int ii = 0; ii < count; ii++)
				{
					OpcRcw.Da.OPCITEMSTATE result = (OpcRcw.Da.OPCITEMSTATE)Marshal.PtrToStructure(pos, typeof(OpcRcw.Da.OPCITEMSTATE));

					output[ii]                    = new ItemValue();
					output[ii].ClientHandle       = result.hClient;
					output[ii].Value              = result.vDataValue;
					output[ii].Quality            = new Opc.Da.Quality(result.wQuality);
					output[ii].QualitySpecified   = true;
					output[ii].Timestamp          = OpcCom.Interop.GetFILETIME(Convert(result.ftTimeStamp));
					output[ii].TimestampSpecified = output[ii].Timestamp != DateTime.MinValue;
					
					pos = (IntPtr)(pos.ToInt32() + Marshal.SizeOf(typeof(OpcRcw.Da.OPCITEMSTATE)));
				}

				if (deallocate)
				{
					Marshal.FreeCoTaskMem(pInput);
					pInput = IntPtr.Zero;
				}
			}

			return output;
		}

		/// <summary>
		/// Unmarshals and deallocates a OPCITEMRESULT structures.
		/// </summary>
		internal static int[] GetItemResults(ref IntPtr pInput, int count, bool deallocate)
		{
			int[] output = null;
			
			if (pInput != IntPtr.Zero && count > 0)
			{
				output = new int[count];

				IntPtr pos = pInput;

				for (int ii = 0; ii < count; ii++)
				{
					OpcRcw.Da.OPCITEMRESULT result = (OpcRcw.Da.OPCITEMRESULT)Marshal.PtrToStructure(pos, typeof(OpcRcw.Da.OPCITEMRESULT));

					output[ii] = result.hServer;

					if (deallocate)
					{
						Marshal.FreeCoTaskMem(result.pBlob);
						result.pBlob = IntPtr.Zero;
					}
					
					pos = (IntPtr)(pos.ToInt32() + Marshal.SizeOf(typeof(OpcRcw.Da.OPCITEMRESULT)));
				}

				if (deallocate)
				{
					Marshal.FreeCoTaskMem(pInput);
					pInput = IntPtr.Zero;
				}
			}

			return output;
		}
	}
}
