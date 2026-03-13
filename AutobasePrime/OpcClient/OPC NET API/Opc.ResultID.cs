//============================================================================
// TITLE: Opc.Result.cs
//
// CONTENTS:
// 
// Defines static information for well known error/success codes.
//
// (c) Copyright 2002-2004 The OPC Foundation
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
using System.Xml;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace Opc
{
	/// <summary>
	/// Contains a unique identifier for a result code.
	/// </summary>
	[Serializable]
	public struct ResultID
	{
		/// <summary>
		/// Used for result codes identified by a qualified name.
		/// </summary>
		public XmlQualifiedName Name 
		{
			get{ return m_name; }
		}

		/// <summary>
		/// Used for result codes identified by a integer.
		/// </summary>
		public int Code
		{
			get{ return m_code; }
		}
		
		/// <summary>
		/// Returns true if the objects are equal.
		/// </summary>
		public static bool operator==(ResultID a, ResultID b) 
		{
			return a.Equals(b);
		}

		/// <summary>
		/// Returns true if the objects are not equal.
		/// </summary>
		public static bool operator!=(ResultID a, ResultID b) 
		{
			return !a.Equals(b);
		}

		/// <summary>
		/// Checks for the 'S_' prefix that indicates a success condition.
		/// </summary>
		public bool Succeeded()
		{
			if (Code != -1)   return (Code >= 0);
			if (Name != null) return Name.Name.StartsWith("S_");
			return false;
		}

		/// <summary>
		/// Checks for the 'E_' prefix that indicates an error condition.
		/// </summary>
		public bool Failed()
		{
			if (Code != -1)   return (Code < 0);
			if (Name != null) return Name.Name.StartsWith("E_");
			return false;
		}

		#region Constructors
		/// <summary>
		/// Initializes a result code identified by a qualified name.
		/// </summary>
		public ResultID(XmlQualifiedName name) 
		{ 
			m_name = name; 
			m_code = -1; 
		}

		/// <summary>
		/// Initializes a result code identified by an integer.
		/// </summary>
		public ResultID(long code) 
		{ 
			m_name = null; 
			
			if (code > Int32.MaxValue)
			{
				code = -(((long)UInt32.MaxValue)+1-code);
			}

			m_code = (int)code;
		}

		/// <summary>
		/// Initializes a result code identified by a qualified name.
		/// </summary>
		public ResultID(string name, string ns) 
		{ 
			m_name = new XmlQualifiedName(name, ns); 
			m_code = -1;
		}

		/// <summary>
		/// Initializes a result code with a general result code and a specific result code.
		/// </summary>
		public ResultID(ResultID resultID, long code) 
		{ 
			m_name = resultID.Name; 

			if (code > Int32.MaxValue)
			{
				code = -(((long)UInt32.MaxValue)+1-code);
			}

			m_code = (int)code;
		}
		#endregion

		#region Object Method Overrides
		/// <summary>
		/// Returns true if the target object is equal to the object.
		/// </summary>
		public override bool Equals(object target)
		{
			if (target != null && target.GetType() == typeof(ResultID))
			{
				ResultID resultID = (ResultID)target;

				// compare by integer if both specify valid integers.
				if (resultID.Code != -1 && Code != -1)
				{
					return (resultID.Code == Code); 
				}

				// compare by name if both specify valid names.
				if (resultID.Name != null && Name != null)
				{
					return (resultID.Name == Name);
				}
			}

			return false;
		}

		/// <summary>
		/// Formats the result identifier as a string.
		/// </summary>
		public override string ToString()
		{
			if (Name != null) return Name.Name;
			return String.Format("0x{0,0:X}", Code);
		}

		/// <summary>
		/// Returns a useful hash code for the object.
		/// </summary>
		public override int GetHashCode()
		{
			if (Code != -1) return Code.GetHashCode();
			if (Name != null) return Name.GetHashCode();
			return base.GetHashCode();
		}
		#endregion

		#region Private Members
		private XmlQualifiedName m_name;
		private int m_code;
		#endregion

		/// <remarks/>
		public static readonly ResultID S_OK                       = new ResultID("S_OK",                       Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_FALSE                    = new ResultID("S_FALSE",                    Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_DATAQUEUEOVERFLOW        = new ResultID("S_DATAQUEUEOVERFLOW",        Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_UNSUPPORTEDRATE          = new ResultID("S_UNSUPPORTEDRATE",          Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_CLAMP                    = new ResultID("S_CLAMP",                    Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_FAIL                     = new ResultID("E_FAIL",                     Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_TIMEDOUT                 = new ResultID("E_TIMEDOUT",                 Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_OUTOFMEMORY              = new ResultID("E_OUTOFMEMORY",              Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_NETWORK_ERROR            = new ResultID("E_NETWORK_ERROR",            Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_ACCESS_DENIED            = new ResultID("E_ACCESS_DENIED",            Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_INVALIDHANDLE            = new ResultID("E_INVALIDHANDLE",            Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_UNKNOWN_ITEM_NAME        = new ResultID("E_UNKNOWN_ITEM_NAME",        Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_INVALID_ITEM_NAME        = new ResultID("E_INVALID_ITEM_NAME",        Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_UNKNOWN_ITEM_PATH        = new ResultID("E_UNKNOWN_ITEM_PATH",        Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_INVALID_ITEM_PATH        = new ResultID("E_INVALID_ITEM_PATH",        Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_INVALID_PID              = new ResultID("E_INVALID_PID",              Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_READONLY                 = new ResultID("E_READONLY",                 Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_WRITEONLY                = new ResultID("E_WRITEONLY",                Namespace.OPC_DATA_ACCESS);
		/// <remarks/> 
		public static readonly ResultID E_BADTYPE                  = new ResultID("E_BADTYPE",                  Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_RANGE                    = new ResultID("E_RANGE",                    Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_INVALID_FILTER           = new ResultID("E_INVALID_FILTER",           Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_INVALIDCONTINUATIONPOINT = new ResultID("E_INVALIDCONTINUATIONPOINT", Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_NO_WRITEQT               = new ResultID("E_NO_WRITEQT",               Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_NO_ITEM_DEADBAND         = new ResultID("E_NO_ITEM_DEADBAND",         Namespace.OPC_DATA_ACCESS);
		/// <remarks/> 
		public static readonly ResultID E_NO_ITEM_SAMPLING         = new ResultID("E_NO_ITEM_SAMPLING",         Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_NO_ITEM_BUFFERING        = new ResultID("E_NO_ITEM_BUFFERING",        Namespace.OPC_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_TYPE_CHANGED             = new ResultID("E_TYPE_CHANGED",             Namespace.OPC_COMPLEX_DATA);
		/// <remarks/>
		public static readonly ResultID E_FILTER_DUPLICATE         = new ResultID("E_FILTER_DUPLICATE",         Namespace.OPC_COMPLEX_DATA);
		/// <remarks/>
		public static readonly ResultID E_FILTER_INVALID           = new ResultID("E_FILTER_INVALID",           Namespace.OPC_COMPLEX_DATA);
		/// <remarks/>
		public static readonly ResultID E_FILTER_ERROR             = new ResultID("E_FILTER_ERROR ",            Namespace.OPC_COMPLEX_DATA);
		/// <remarks/>
		public static readonly ResultID S_FILTER_NO_DATA           = new ResultID("S_FILTER_NO_DATA ",          Namespace.OPC_COMPLEX_DATA);

		/// <remarks/>
		public static readonly ResultID E_MAXEXCEEDED      = new ResultID("E_MAXEXCEEDED",          Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_NODATA           = new ResultID("S_NODATA",          Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_MOREDATA         = new ResultID("S_MOREDATA",          Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_INVALIDAGGREGATE = new ResultID("E_INVALIDAGGREGATE",  Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_CURRENTVALUE     = new ResultID("S_CURRENTVALUE",      Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_EXTRADATA        = new ResultID("S_EXTRADATA",         Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID W_NOFILTER         = new ResultID("W_NOFILTER",          Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_UNKNOWNATTRID    = new ResultID("E_UNKNOWNATTRID",     Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_NOT_AVAIL        = new ResultID("E_NOT_AVAIL",         Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_INVALIDDATATYPE  = new ResultID("E_INVALIDDATATYPE",   Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_DATAEXISTS       = new ResultID("E_DATAEXISTS",        Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_INVALIDATTRID    = new ResultID("E_INVALIDATTRID",     Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID E_NODATAEXISTS     = new ResultID("E_NODATAEXISTS",      Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_INSERTED         = new ResultID("S_INSERTED",          Namespace.OPC_HISTORICAL_DATA_ACCESS);
		/// <remarks/>
		public static readonly ResultID S_REPLACED         = new ResultID("S_REPLACED",          Namespace.OPC_HISTORICAL_DATA_ACCESS);
	}

	/// <summary>
	/// Used to raise an exception with associated with a specified result code.
	/// </summary>
	[Serializable]
	public class ResultIDException : ApplicationException
	{	/// <remarks/>
		public ResultID Result {get{ return m_result; }}
	
		/// <remarks/>
		public ResultIDException(ResultID result) : base(result.ToString()) { m_result = result; } 
		/// <remarks/>
		public ResultIDException(ResultID result, string message) : base(result.ToString() + "\r\n" + message) { m_result = result; } 
		/// <remarks/>
		public ResultIDException(ResultID result, string message, Exception e) : base(result.ToString() + "\r\n" + message, e) { m_result = result; } 
		/// <remarks/>
		protected ResultIDException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		
		/// <remarks/>
		private ResultID m_result = ResultID.E_FAIL;
	}
}
