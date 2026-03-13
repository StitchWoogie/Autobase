//============================================================================
// TITLE: Opc.ConnectData.cs
//
// CONTENTS:
// 
// Defines class to contain protocol dependent connection information.
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
using System.Net;
using System.Runtime.Serialization;

namespace Opc
{
	/// <summary>
	/// Contains protocol dependent connection and authenication information.
	/// </summary>
	[Serializable]
	public class ConnectData : ISerializable, ICredentials
	{	
		//======================================================================
		// Construction

		/// <summary>
		/// Initializes an instance with the specified credentials.
		/// </summary>
		public ConnectData(NetworkCredential credentials)
		{
			m_credentials = credentials;
			m_proxy       = null;
		}

		/// <summary>
		/// Initializes an instance with the specified credentials and web proxy.
		/// </summary>
		public ConnectData(NetworkCredential credentials, WebProxy proxy)
		{
			m_credentials = credentials;
			m_proxy       = proxy;
		}

		//======================================================================
		// ISerializable

		/// <summary>
		/// A set of names for fields used in serialization.
		/// </summary>
		private class Names
		{
			internal const string USER_NAME = "UserName";
			internal const string PASSWORD  = "Password";
			internal const string DOMAIN    = "Domain";
			internal const string PROXY_URI = "ProxyUri";
		}

		/// <summary>
		/// Contructs teh object by de-serializing from the stream.
		/// </summary>
		protected ConnectData(SerializationInfo info, StreamingContext context)
		{
			string username = info.GetString(Names.USER_NAME);
			string password = info.GetString(Names.PASSWORD);
			string domain   = info.GetString(Names.DOMAIN);
			string proxyUri = info.GetString(Names.PROXY_URI);

			if (domain != null)
			{
				m_credentials = new NetworkCredential(username, password, domain);
			}
			else
			{
				m_credentials = new NetworkCredential(username, password);
			}

			if (proxyUri != null)
			{
				m_proxy = new WebProxy(proxyUri);
			}
			else
			{
				m_proxy = null;
			}
		}

		/// <summary>
		/// Serializes a server into a stream.
		/// </summary>
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (m_credentials != null)
			{
				info.AddValue(Names.USER_NAME, m_credentials.UserName);
				info.AddValue(Names.PASSWORD,  m_credentials.Password);
				info.AddValue(Names.DOMAIN,    m_credentials.Domain);
			}
			else
			{
				info.AddValue(Names.USER_NAME, null);
				info.AddValue(Names.PASSWORD,  null);
				info.AddValue(Names.DOMAIN,    null);
			}

			if (m_proxy != null)
			{
				info.AddValue(Names.PROXY_URI, m_proxy.Address);
			}
			else
			{
				info.AddValue(Names.PROXY_URI, null);
			}
		}	

		/// <summary>
		/// The credentials to submit to the proxy server for authentication.
		/// </summary>
		public NetworkCredential Credentials
		{
			get { return m_credentials;  }
			set	{ m_credentials = value; }
		}


		/// <summary>
		/// Returns a NetworkCredential object that is associated with the specified URI, and authentication type.
		/// </summary>
		public NetworkCredential GetCredential(Uri uri, string authenticationType)
		{
			if (m_credentials != null)
			{
				return new NetworkCredential(m_credentials.UserName, m_credentials.Password, m_credentials.Domain);
			}

			return null;
		}

		/// <summary>
		/// Returns the web proxy object to use when connecting to the server.
		/// </summary>
		public IWebProxy GetProxy()
		{
			if (m_proxy != null)
			{
				return m_proxy;
			}
			else
			{
				//return WebProxy.GetDefaultProxy(); .NET 2.0부터는 사용못함
                return WebRequest.DefaultWebProxy;
			}
		}

		/// <summary>
		/// Sets the web proxy object.
		/// </summary>
		public void SetProxy(WebProxy proxy)
		{
			m_proxy = proxy;
		}

		/// <summary>
		/// The credentials used to autheniticate the client with the server.
		/// </summary>
		private NetworkCredential m_credentials = null;

		/// <summary>
		/// The web proxy used to access the server.
		/// </summary>
		private WebProxy m_proxy = null;
	}
}
