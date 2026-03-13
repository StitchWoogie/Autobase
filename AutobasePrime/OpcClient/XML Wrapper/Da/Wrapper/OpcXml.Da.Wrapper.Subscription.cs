//============================================================================
// TITLE: Server.cs
//
// CONTENTS:
// 
// An in-process wrapper for an XML-DA server. 
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
// 2003/03/26 RSA   Initial implementation.

using System;
using System.Xml;
using System.Net;
using System.Threading;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Reflection;
using Opc;
using Opc.Da;

namespace OpcXml.Da.Wrapper
{
	/// <summary>
	/// Manages an item value cache for a subscription.
	/// </summary>
	internal class Subscription : IDisposable
	{
		//======================================================================
		// Construction
		
		/// <summary>
		/// Initializes the object.
		/// </summary>
		public Subscription() {}
			
		//======================================================================
		// IDisposable

		/// <summary>
		/// Disposes of the subscription and disposes all contained cache objects.
		/// </summary>
		public void Dispose()
		{
			lock (this)
			{
				if (CacheList != null)
				{
					foreach (Cache cache in CacheList)
					{
						cache.Dispose();
					}

					CacheList.Clear();
					CacheList = null;
				}
			}
		}

		//======================================================================
		// Public Properties

		/// <summary>
		/// The unique handle assigned to the subscription.
		/// </summary>
		public string ID;

		/// <summary>
		/// The maximum lifetime of the subscription.
		/// </summary>
		public TimeSpan PingTime;

		/// <summary>
		/// The UTC time of the last poll from the client.
		/// </summary>
		public DateTime LastPollTime;

		/// <summary>
		/// The set of caches for the subscription.
		/// </summary>
		public ArrayList CacheList = new ArrayList();
	}

	/// <summary>
	/// Manages an item value cache for a subscription.
	/// </summary>
	internal class Cache : IDisposable
	{
		//======================================================================
		// Construction
		
		/// <summary>
		/// Initializes the cache with the specified subacription.
		/// </summary>
		public Cache(Opc.Da.Subscription subscription)
		{
			if (subscription == null) throw new ArgumentNullException("subscription");
			m_subscription = subscription;
		}

		//======================================================================
		// IDisposable
		
		/// <summary>
		/// Disposes of the subscription and clears buffers.
		/// </summary>
		public void Dispose()
		{
			lock (this)
			{
				if (m_subscription != null)
				{
					m_items.Clear();
					m_handles.Clear();
					m_cache.Clear();
					m_buffer.Clear();

					try
					{
						Opc.Da.IServer server = m_subscription.Server;
						server.CancelSubscription(m_subscription);
					}
					catch
					{
						// ignore any errors.
					}

					m_subscription.Dispose();
					m_subscription = null;
				}
			}
		}

		//======================================================================
		// Private Members

		/// <summary>
		/// The subscription associated with the cache.
		/// </summary>
		private Opc.Da.Subscription m_subscription = null;

		/// <summary>
		/// The items indexed by the internal client handle.
		/// </summary>
		private Hashtable m_items = new Hashtable();

		/// <summary>
		/// The external client handles indexed by the internal client handle.
		/// </summary>
		private Hashtable m_handles = new Hashtable();

		/// <summary>
		/// The a cache of the lastest value for each item.
		/// </summary>
		private Hashtable m_cache = new Hashtable();

		/// <summary>
		/// The buffer of values returned from the server.
		/// </summary>
		private ArrayList m_buffer = new ArrayList();

		//======================================================================
		// Public Methods

		/// <summary>
		/// Initializes the cache with the set items.
		/// </summary>
		public SubscribeItemValueResult[] Initialize(Item[] items, bool returnValues)
		{
			if (items == null || items.Length == 0) throw new ArgumentNullException("items");

			lock (this)
			{
				// generate client handle that is unique in this context.
				foreach (Item item in items)
				{
					string clientHandle = Guid.NewGuid().ToString();
					if (item.ClientHandle != null) { m_handles[clientHandle] = item.ClientHandle; }
					item.ClientHandle = clientHandle;
				}

				// add items to subscription.
				ItemResult[] resultItems = m_subscription.AddItems(items);

				if (resultItems == null || resultItems.Length == 0)
				{
					throw new InvalidResponseException("ISubscription.AddItems");
				}

				// read initial values if requested.
				ItemValueResult[] initialValues = null;
				
				if (returnValues)
				{
					Item[] readItems = new Item[resultItems.Length];

					for (int ii = 0; ii < resultItems.Length; ii++)
					{
						// initialize an item to read value from cache.
						if (resultItems[ii].ResultID.Succeeded())
						{
							readItems[ii] = new Item(resultItems[ii]);
							readItems[ii].MaxAge = Int32.MaxValue;
							readItems[ii].MaxAgeSpecified = true;
						}
					
						// initialize a dummy item as a placeholder.
						else
						{
							readItems[ii] = new Item();
						}
					}

					try   { initialValues = m_subscription.Read(readItems); }
					catch { initialValues = null; }
				}

				// index subscription items by client handle.
				foreach (Item item in m_subscription.Items)
				{
					m_items[item.ClientHandle] = item;
				}

				// establish data update callback.
				m_subscription.DataChanged += new DataChangedEventHandler(OnDataChanged);

				// create subscribe item results.
				SubscribeItemValueResult[] results = new SubscribeItemValueResult[items.Length];

				for (int ii = 0; ii < items.Length; ii++)
				{
					// fill in subscribe item value result.
					results[ii] = new SubscribeItemValueResult(items[ii]);

					results[ii].ServerHandle          = resultItems[ii].ServerHandle;
					results[ii].ClientHandle          = m_handles[items[ii].ClientHandle];
					results[ii].ResultID              = resultItems[ii].ResultID;
					results[ii].DiagnosticInfo        = resultItems[ii].DiagnosticInfo;
					results[ii].Value                 = null;
					results[ii].Quality               = Quality.Bad;
					results[ii].QualitySpecified      = false;
					results[ii].Timestamp             = DateTime.MinValue;
					results[ii].TimestampSpecified    = false;
					results[ii].SamplingRate          = resultItems[ii].SamplingRate;
					results[ii].SamplingRateSpecified = resultItems[ii].SamplingRateSpecified;

					// set initial value.
					if (initialValues != null)
					{
						if (initialValues[ii].ResultID.Succeeded())
						{
							results[ii].Value              = initialValues[ii].Value;
							results[ii].Quality            = initialValues[ii].Quality;
							results[ii].QualitySpecified   = initialValues[ii].QualitySpecified;
							results[ii].Timestamp          = initialValues[ii].Timestamp;
							results[ii].TimestampSpecified = initialValues[ii].TimestampSpecified;
						}
					}

					// check for changed sampling rate.
					if (results[ii].ResultID.Succeeded())
					{
						if (items[ii].SamplingRateSpecified)
						{
							if (results[ii].SamplingRate == items[ii].SamplingRate)
							{
								results[ii].SamplingRate = 0;
								results[ii].SamplingRateSpecified = false;
							}
							else
							{
								results[ii].ResultID = ResultID.S_UNSUPPORTEDRATE;
							}
						}
					}
				}

				// return results.
				return results;
			}
		}
		
		/// <summary>
		/// Returns the current set of item values.
		/// </summary>
		public ItemValueResultList GetItems(bool returnAllItems)
		{
			lock (this)
			{
				// initialize changed values list.
				ItemValueResultList changedItems = new ItemValueResultList();
				changedItems.ClientHandle = m_subscription.ClientHandle;

				// return only changed items.
				if (!returnAllItems)
				{
					if (m_buffer.Count == 0) return null;

					foreach (ItemValueResult item in m_buffer)
					{
						ItemValueResult clone = (ItemValueResult)item.Clone();
						clone.ClientHandle = m_handles[item.ClientHandle];
						changedItems.Add(clone);
					}
				}

				// return contents of buffer plus all items.
				else
				{
					// add buffered items that are not in the cache.
					foreach (ItemValueResult item in m_buffer)
					{
						if (item != m_cache[item.ClientHandle])
						{
							ItemValueResult clone = (ItemValueResult)item.Clone();
							clone.ClientHandle = m_handles[item.ClientHandle];
							changedItems.Add(clone);
						}
					}

					// add all items in the cache.
					foreach (ItemValueResult item in m_cache.Values)
					{
						ItemValueResult clone = (ItemValueResult)item.Clone();
						clone.ClientHandle = m_handles[item.ClientHandle];
						changedItems.Add(clone);
					}
				}

				// clear the buffer.
				m_buffer.Clear();			

				return changedItems;
			}
		}

		/// <summary>
		/// Called when data updates are received from the server.
		/// </summary>
		private void OnDataChanged(object subscription, ItemValueResult[] values)
		{
			lock (this)
			{
				if (values == null || m_subscription == null) return;

				// add each item into the buffer.
				foreach (ItemValueResult value in values)
				{
					Item item = (Item)m_items[value.ClientHandle];

					// check for unrecognized item - should never happen.
					if (item == null) { continue; }

					// replace existing item if no buffering enabled.
					bool added = false;

					if (!item.EnableBuffering)
					{
						for (int ii = 0; ii < m_buffer.Count; ii++)
						{
							ItemValueResult previous = (ItemValueResult)m_buffer[ii];

							if (previous.ClientHandle.Equals(value.ClientHandle))
							{
								added = true;
								m_buffer[ii] = value;
								break;
							}
						}
					}

					// update the cache.
					m_cache[value.ClientHandle] = value;

					// append to buffer if not found or buffering enabled.
					if (!added) m_buffer.Add(value);
				}
			}
		}
	}
}
