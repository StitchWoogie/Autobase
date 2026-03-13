//============================================================================
// TITLE: IFactory.cs
//
// CONTENTS:
// 
// A interface and a class used to instantiate server objects.
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
// 2003/08/18 RSA   Initial implementation.

using System;
using System.Xml;
using System.Net;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using OpcRcw.Comn;

namespace OpcCom
{
	/// <summary>
	/// The default class used to instantiate server objects.
	/// </summary>
	[Serializable]
	public class Factory : Opc.Factory
	{
		//======================================================================
		// Construction
		
		/// <summary>
		/// Initializes an instance for use for in process objects.
		/// </summary>
		public Factory() : base(null, false)
		{
			// do nothing.
		}

		/// <summary>
		/// Initializes an instance for use with .NET remoting.
		/// </summary>
		public Factory(bool useRemoting) : base(null, useRemoting)
		{
			// do nothing.
		}

		//======================================================================
		// ISerializable

		/// <summary>
		/// Contructs a server by de-serializing its URL from the stream.
		/// </summary>
		protected Factory(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// do nothing.
		}

		//======================================================================
		// IFactory

		/// <summary>
		/// Creates a new instance of the server.
		/// </summary>
		public override Opc.IServer CreateInstance(Opc.URL url, Opc.ConnectData connectData)
		{
			object comServer = Factory.Connect(url, connectData);
			
			if (comServer == null)
			{
				return null;
			}

			OpcCom.Server server = null; 

			try
			{
				// DA
				if (url.Scheme == Opc.UrlScheme.DA)
				{
					// Verify that it is a DA server.
					if (!typeof(OpcRcw.Da.IOPCServer).IsInstanceOfType(comServer))
					{
						throw new NotSupportedException(typeof(OpcRcw.Da.IOPCServer).FullName);
					}

					// DA 3.00
					if (typeof(OpcRcw.Da.IOPCBrowse).IsInstanceOfType(comServer))
					{
						server = new OpcCom.Da.Server(url, comServer);
					}
							
					// DA 2.XX
					else if (typeof(OpcRcw.Da.IOPCItemProperties).IsInstanceOfType(comServer))
					{
						server = new OpcCom.Da20.Server(url, comServer);
					}

					// DA 1.0A
					else
					{					
						server = new OpcCom.Da20.Server(url, comServer);
						//throw new NotSupportedException("Data Access 1.0a");
					}	
				}

				// HDA
				else if (url.Scheme == Opc.UrlScheme.HDA)
				{
					// Verify that it is a HDA server.
					if (!typeof(OpcRcw.Hda.IOPCHDA_Server).IsInstanceOfType(comServer))
					{
						throw new NotSupportedException(typeof(OpcRcw.Hda.IOPCHDA_Server).FullName);
					}
					
					server = new OpcCom.Hda.Server(url, comServer);
				}

				// All other specifications not supported yet.
				else
				{
					throw new NotSupportedException(url.Scheme);
				}
			}
			catch (Exception e)
			{
				if (server != null) { Marshal.ReleaseComObject(server); }
				throw e;
			}

			// initialize the wrapper object.
			if (server != null)
			{
				server.Initialize(url, connectData);
			}

			return server;
		}

		/// <summary>
		/// Connects to the specified COM server server.
		/// </summary>
		public static object Connect(Opc.URL url, Opc.ConnectData connectData)
		{
			// parse path to find prog id and clsid.
			string progID = url.Path;
			string clsid  = null;

			int index = url.Path.IndexOf('/');

			if (index >= 0)
			{
				progID = url.Path.Substring(0, index);
				clsid  = url.Path.Substring(index+1);
			}

			// look up prog id if clsid not specified in the url.
			Guid guid;

			if (clsid == null)
			{
				// use OpcEnum to lookup the prog id.
				guid  = new ServerEnumerator().CLSIDFromProgID(progID, url.HostName, connectData);

				// check if prog id is actually a clsid string.
				if (guid == Guid.Empty)
				{
					guid = new Guid(progID);
				}
			}
				
			// convert clsid string to a guid.
			else
			{
				guid = new Guid(clsid);
			}

			// instantiate the COM server.
			try
			{
				return OpcCom.Interop.CreateInstance(guid, url.HostName, (connectData != null)?connectData.GetCredential(null, null):null);
			}
			catch (Exception e)
			{
				throw new Opc.ConnectFailedException(e);
			}				
		}
	}
}
