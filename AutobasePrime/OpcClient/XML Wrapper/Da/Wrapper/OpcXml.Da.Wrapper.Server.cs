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
using System.Web.Services.Protocols;
using Opc;
using Opc.Da;
using OpcCom.Da;

namespace OpcXml.Da.Wrapper
{
	/// <summary>
	/// A XML-DA server implementation that wraps a COM-DA server.
	/// </summary>
	public class Server : IServer
	{	
		//======================================================================
		// Construction

		/// <summary>
		/// Initializes the XML-DA server.
		/// </summary>
		public Server() 
		{
			// initialize server status.
			m_status.VendorInfo     = "OPC XML Data Access 1.00 Sample Server";
			m_status.ProductVersion = "1.00.1.00";
			m_status.ServerState    = serverState.failed;
			m_status.StatusInfo     = null;
			m_status.CurrentTime    = DateTime.Now;
			m_status.StartTime      = DateTime.Now;
			m_status.LastUpdateTime = DateTime.MinValue;

			// set the supported locales.
			m_supportedLocales = new string[] { "en", "fr" };

			// create the reosurce manager.
			m_resourceManager  = new ResourceManager("OpcXml.Resources.Strings", Assembly.GetExecutingAssembly());
		}

		//======================================================================
		// IDisposable

		/// <summary>
		/// Releases any unmanaged resources used by the server.
		/// </summary>
		public void Dispose()
		{
			lock (this)
			{
				if (m_server != null)
				{
					m_server.Dispose();
				}
			}
		}
		
		//======================================================================
		// Public Properties

		/// <summary>
		/// The names of the locales supported by the server.
		/// </summary>
		public string[] SupportedLocales {get{lock (this){ return (m_supportedLocales != null)?(string[])m_supportedLocales.Clone():null; }}}

		//======================================================================
		// OpcXml.Da.IServer

		/// <summary>
		/// Connects to the server with the specified address.
		/// </summary>
		public void Connect(URL url, ConnectData connectData)
		{
			lock (this)
			{
				try
				{
					// initialize object used to communicate with the COM server.
					m_server = new Opc.Da.Server(new OpcCom.Factory(), url);

					// connect to the underlying COM server.
					m_server.Connect(connectData);

					// set default result filters.
					m_server.SetResultFilters((int)ResultFilter.All);

					// update server state.
					m_status.ServerState = serverState.running;
				}
				catch (Exception e)
				{
					m_server             = null;
					m_status.ServerState = serverState.commFault;
					m_status.StatusInfo  = e.Message;
					throw e;
				}
			}
		}

		/// <summary>
		/// Disconnects from the server and releases all network resources.
		/// </summary>
		public void Disconnect()
		{
			lock (this)
			{
				try
				{
					m_server.Disconnect();
					m_server.Dispose();
				}
				catch
				{
					m_server             = null;
					m_status.ServerState = serverState.failed;
				}
			}
		}
		
		/// <summary>
		/// Returns the current server status.
		/// </summary>
		public ReplyBase GetStatus(
			string           locale,
			string           clientRequestHandle,
			out ServerStatus status)
		{
			lock (this)
			{
				// initialize reply.
				ReplyBase reply = CreateReply(locale, clientRequestHandle);

				// copy the current status.
				status             = (ServerStatus)m_status.Clone();
				status.CurrentTime = DateTime.Now;
				
				// update reply and return.
				reply.ReplyTime = DateTime.Now;
				return reply;
			}
		}

		/// <summary>
		/// Reads a set of items.
		/// </summary>
		public ReplyBase Read(
			RequestOptions          options, 
			ItemList                requestList,
			out ItemValueResultList replyList,
			out Error[]             errors)
		{
			lock (this)
			{
				// initialize reply.
				ReplyBase reply = CreateReply(options.Locale, options.RequestHandle);

				// ensure server can process the request.
				CheckState(reply.RevisedLocaleID, true);

				// check deadline.
				replyList = CheckDeadline(reply.RevisedLocaleID, options.RequestDeadline, requestList);

				if (replyList != null)
				{
					errors = ApplyOptions(reply.RevisedLocaleID, options, replyList);
					reply.ReplyTime = DateTime.Now;
					return reply;
				}

				// create reply list.
				replyList = new ItemValueResultList();

				// apply list level parameters.
				ApplyItemListDefaults(requestList);

				ItemValueResult[] replyItems = null;

				try
				{
					replyItems = m_server.Read((Item[])requestList.ToArray(typeof(Item)));
				}
				catch (Exception e)
				{
					throw CreateException(reply.RevisedLocaleID, e);
				}

				// read items from server.
				replyList.AddRange(replyItems);

				// apply request options.
				errors = ApplyOptions(reply.RevisedLocaleID, options, replyList);

				// update reply and return.
				reply.ReplyTime = DateTime.Now;
				return reply;
			}
		}

		/// <summary>
		/// Writes a set of items and, if requested, returns the current values.
		/// </summary>
		public ReplyBase Write(
			RequestOptions          options, 
			ItemValueList           requestList, 
			bool                    returnValues,
			out ItemValueResultList replyList,
			out Error[]             errors)
		{
			lock (this)
			{
				// initialize reply.
				ReplyBase reply = CreateReply(options.Locale, options.RequestHandle);

				// ensure server can process the request.
				CheckState(reply.RevisedLocaleID, true);
				
				// check deadline.
				replyList = CheckDeadline(reply.RevisedLocaleID, options.RequestDeadline, requestList);

				if (replyList != null)
				{
					errors = ApplyOptions(reply.RevisedLocaleID, options, replyList);
					reply.ReplyTime = DateTime.Now;
					return reply;
				}

				// create reply list.
				replyList = new ItemValueResultList();

				// write items from server.
				IdentifiedResult[] results = null;
				
				try
				{
					results = m_server.Write((ItemValue[])requestList.ToArray(typeof(ItemValue)));			
				}
				catch (Exception e)
				{
					throw CreateException(reply.RevisedLocaleID, e);
				}

				// readback item values from server.
				ItemValueResult[] replyItems = null;

				if (returnValues)
				{
					Item[] readbackItems = new Item[results.Length];

					// skip readback for items where the write failed.
					for (int ii = 0; ii < results.Length; ii++)
					{
						readbackItems[ii] = (results[ii].ResultID.Succeeded())?new Item(results[ii]):new Item();
					}

					// check readback failures.
					try
					{
						replyItems = m_server.Read(readbackItems);
					}
					catch (Exception e)
					{
						throw CreateException(reply.RevisedLocaleID, e);
					}
				}

				// create item value result list if not created during read-back.
				if (replyItems == null)
				{
					replyItems = new ItemValueResult[results.Length];
				}

				// copy result information.
				for (int ii = 0; ii < results.Length; ii++)
				{
					// create item value result if not created with read-back.
					if (replyItems[ii] == null)
					{
						replyItems[ii] = new ItemValueResult(results[ii]);
					}

					// ensure any non-S_OK result returned during the write is returned.
					if (results[ii].ResultID != ResultID.S_OK)
					{
						replyItems[ii].ResultID       = results[ii].ResultID;
						replyItems[ii].DiagnosticInfo = results[ii].DiagnosticInfo;
					}
				}

				// add items to list.
				replyList.AddRange(replyItems);

				// apply request options.
				errors = ApplyOptions(reply.RevisedLocaleID, options, replyList);
				
				// update reply and return.
				reply.ReplyTime = DateTime.Now;
				return reply;
			}
		}

		/// <summary>
		/// Establishes a subscription for the set of items.
		/// </summary>
		public ReplyBase Subscribe(
			RequestOptions          options, 
			ItemList                requestList, 
			TimeSpan                pingTime,
			bool                    returnValues,
			out string              subscriptionID,
			out ItemValueResultList replyList,
			out Error[]             errors)
		{
			lock (this)
			{
				subscriptionID = null;

				// initialize reply.
				ReplyBase reply = CreateReply(options.Locale, options.RequestHandle);

				// ensure server can process the request.
				CheckState(reply.RevisedLocaleID, true);

				// check deadline.
				replyList = CheckDeadline(reply.RevisedLocaleID, options.RequestDeadline, requestList);

				if (replyList != null)
				{
					errors = ApplyOptions(reply.RevisedLocaleID, options, replyList);
					reply.ReplyTime = DateTime.Now;
					return reply;
				}

				Subscription subscription = new Subscription();

				// create reply list.
				replyList = new ItemValueResultList();

				// initialize the subscription state from the list level parameters.
				SubscriptionState state = new SubscriptionState();

				state.Name         = null;
				state.ClientHandle = requestList.ClientHandle;
				state.Active       = true;
				state.UpdateRate   = (requestList.SamplingRateSpecified)?requestList.SamplingRate:0;
				state.KeepAlive    = 0;
				state.Deadband     = (requestList.DeadbandSpecified)?requestList.Deadband:0;
				state.Locale       = reply.RevisedLocaleID;

				// create a new subscription.
				Opc.Da.Subscription comSubscription = null;
				
				try
				{
					comSubscription = (Opc.Da.Subscription)m_server.CreateSubscription(state);
				}
				catch (Exception e)
				{
					throw CreateException(reply.RevisedLocaleID, e);
				}

				// save the revised update rate.
				if (!requestList.SamplingRateSpecified || requestList.SamplingRate != comSubscription.State.UpdateRate)
				{
					replyList.SamplingRate          = comSubscription.State.UpdateRate;
					replyList.SamplingRateSpecified = true;
				}

				// create cache.
				Cache cache = new Cache(comSubscription);

				// add items to subscription.
				SubscribeItemValueResult[] replyItems = cache.Initialize(
					(Item[])requestList.ToArray(typeof(Item)), 
					returnValues);

				// add reply items to reply list.
				replyList.AddRange(replyItems);

				// add to subscription list (this list is indexed by the subscription handle).
				subscription.CacheList.Add(cache);

				// assign a unique subscription handle and save subscription list.
				subscriptionID = Guid.NewGuid().ToString();

				// index the new client subscription by the server handle.
				subscription.ID           = subscriptionID;
				subscription.PingTime     = pingTime;
				subscription.LastPollTime = DateTime.Now;

				m_subscriptions[subscriptionID] = subscription;

				// start the ping timer - checks for expired subscriptions once per second.
				if (m_pingTimer == null)
				{
					m_pingTimer = new Timer(
						new TimerCallback(CleanupSubscriptions), 
						null, 
						1000,
						1000);
				}

				// apply request options.
				errors = ApplyOptions(reply.RevisedLocaleID, options, replyList);
		
				// update reply and return.
				reply.ReplyTime = DateTime.Now;
				return reply;
			}
		}

		/// <summary>
		/// Polls the server for the any item changes for one or more subscriptions.
		/// </summary>
		public ReplyBase PolledRefresh(
			RequestOptions            options, 
			string[]                  subscriptionIDs,
			TimeSpan                  holdTime,
			TimeSpan                  waitTime,
			bool                      returnAllValues,
			out string[]              invalidSubscriptionIDs,
			out ItemValueResultList[] replyLists,
			out Error[]               errors,
			out bool                  dataBufferOverflow)
		{
			if (subscriptionIDs == null || subscriptionIDs.Length == 0) throw new ArgumentNullException("subscriptionIDs");

			// initialize reply.
			ReplyBase reply = CreateReply(options.Locale, options.RequestHandle);

			// ensure server can process the request.
			lock (this)	{ CheckState(reply.RevisedLocaleID, true); }

			// check deadline.
			if (options.RequestDeadline != DateTime.MinValue)
			{
				if (options.RequestDeadline < DateTime.Now)
				{
					throw CreateException(reply.RevisedLocaleID, Error.E_TIMEDOUT);
				}
			}

			// check for excessive hold times.
			if (holdTime.TotalSeconds > 60)
			{
				throw CreateException(reply.RevisedLocaleID, Error.E_INVALIDHOLDTIME);
			}

			// wait for the hold time to expire.
			if (holdTime.Ticks > 0) Thread.Sleep(holdTime);

			ArrayList invalidHandles = new ArrayList();
			ArrayList changedLists   = new ArrayList();

			DateTime waitUntil = DateTime.Now.Add(waitTime);
			
			// check for data changes until the wait time expires. 
			do
			{
				lock (this)
				{
					foreach (string subscriptionID in subscriptionIDs)
					{
						// lookup subscription handle.
						Subscription subscription = (Subscription)m_subscriptions[subscriptionID];

						if (subscription == null)
						{
							invalidHandles.Add(subscriptionID);
							continue;
						}

						// update the last poll time for the subscription.
						subscription.LastPollTime = DateTime.Now;

						// look for available data in each cache.
						foreach (Cache cache in subscription.CacheList)
						{
							ItemValueResultList changedItems = cache.GetItems(returnAllValues);

							if (changedItems != null)
							{
								changedItems.ServerHandle = subscriptionID;
								changedLists.Add(changedItems);
							}
						}
					}
				}

				// exit loop if changes found or all handles are invalid.
				if (changedLists.Count > 0 || invalidHandles.Count == subscriptionIDs.Length)
				{
					break;
				}

				// poll subcription caches for changes every 100ms until wait time exceeded.
				Thread.Sleep(100);
			}
			while (DateTime.Now < waitUntil);

			// initialize output parameters.
			invalidSubscriptionIDs = (invalidHandles.Count > 0)?(string[])invalidHandles.ToArray(typeof(string)):null;
			replyLists             = (ItemValueResultList[])changedLists.ToArray(typeof(ItemValueResultList));
			dataBufferOverflow     = false;
			
			// apply request options.
			errors = ApplyOptions(reply.RevisedLocaleID, options, replyLists);

			// update reply and return.
			reply.ReplyTime = DateTime.Now;
			return reply;
		}

		/// <summary>
		/// Terminates one or more subscriptions.
		/// </summary>
		public void Unsubscribe(string[] subscriptionIDs)
		{
			if (subscriptionIDs == null) throw new ArgumentNullException("subscriptionIDs");

			lock (this)
			{
				foreach (string subscriptionID in subscriptionIDs)
				{
					Subscription subscription = (Subscription)m_subscriptions[subscriptionID];
					
					if (subscription != null)
					{
						m_subscriptions.Remove(subscriptionID);
						subscription.Dispose();
					}
				}
			}
		}

		/// <summary>
		/// Returns a set of elements at the specified position and that meet the filter criteria.
		/// </summary>
		public ReplyBase Browse(
			string              locale,
			string              clientRequestHandle,
			bool                returnErrorText,
			ItemIdentifier      itemID,
			BrowseFilters       filters,
			ref string          continuationPoint,
			out bool            moreElements,
			out BrowseElement[] elements,
			out Error[]         errors)
		{
			moreElements = false;
			
			lock (this)
			{
				// initialize reply.
				ReplyBase reply = CreateReply(locale, clientRequestHandle);

				// ensure server can process the request.
				CheckState(reply.RevisedLocaleID, false);

				// lookup position used to continue browse.
				Opc.Da.BrowsePosition position = null;
				
				if (continuationPoint != null)
				{
					position = (Opc.Da.BrowsePosition)m_positions[continuationPoint];

					// cannot continue previous browse.
					if (position == null)
					{
						throw CreateException(reply.RevisedLocaleID, Error.E_INVALIDCONTINUATIONPOINT);
					}
				}

				// continue previous browse.
				if (position != null)
				{
					m_positions.Remove(continuationPoint);

					try
					{
						elements = m_server.BrowseNext(ref position);
					}
					catch (Exception e)
					{
						position.Dispose();
						throw CreateException(reply.RevisedLocaleID, e);
					}
				}

				// begin new browse.
				else
				{
					try
					{
						elements = m_server.Browse(itemID, filters, out position);
					}
					catch (Exception e)
					{
						throw CreateException(reply.RevisedLocaleID, e);
					}
				}

				// save new continuation point.
				continuationPoint = null;

				if (position != null)
				{
					// dispose of any previous browse positions.
					foreach (Opc.Da.BrowsePosition current in m_positions.Values) { current.Dispose(); }
					m_positions.Clear();

					moreElements = true;
					continuationPoint = Guid.NewGuid().ToString();
					m_positions[continuationPoint] = position;
				}

				errors = (returnErrorText)?GetErrors(reply.RevisedLocaleID, elements):null;
				
				// update reply and return.
				reply.ReplyTime = DateTime.Now;
				return reply;
			}
		}

		/// <summary>
		/// Returns the specified properties for a set of items.
		/// </summary>
		public ReplyBase GetProperties(
			string                 locale,
			string                 clientRequestHandle,
			bool                   returnErrorText,
			ItemIdentifier[]       itemIDs,
			PropertyID[]           propertyIDs,
			string                 itemPath,
			bool                   returnValues,
			out ItemPropertyCollection[] properties,
			out Error[]            errors)
		{
			lock (this)
			{
				// initialize reply.
				ReplyBase reply = CreateReply(locale, clientRequestHandle);

				// ensure server can process the request.
				CheckState(reply.RevisedLocaleID, false);

				// get the properties.
				try
				{
					properties = m_server.GetProperties(
						itemIDs, 
						propertyIDs,
						returnValues);
				}
				catch (Exception e)
				{
					throw CreateException(reply.RevisedLocaleID, e);
				}

				errors = (returnErrorText)?GetErrors(reply.RevisedLocaleID, properties):null;

				// update reply and return.
				reply.ReplyTime = DateTime.Now;
				return reply;
			}
		}
		
		//======================================================================
		// Private Members

		/// <summary>
		/// The COM server being wrapped by the XML-DA server.
		/// </summary>
		private Opc.Da.Server m_server = null;

		/// <summary>
		/// The current status of the XML-DA server (distinct from the status of the COM-DA server).
		/// </summary>
		private ServerStatus m_status = new ServerStatus();

		/// <summary>
		/// A table of subscription lists indexed by subscription handle.
		/// </summary>
		private Hashtable m_subscriptions = new Hashtable();

		/// <summary>
		/// A timer that clears out expired subscriptions.
		/// </summary>
		private Timer m_pingTimer = null;

		/// <summary>
		/// A table of last pool times indexed by subscription handle.
		/// </summary>
		private Hashtable m_subscriptionPollTimes = new Hashtable();

		/// <summary>
		/// The names of the locales supported by the server.
		/// </summary>
		private string[] m_supportedLocales = null;

		/// <summary>
		/// The resource manager used to access localized resources.
		/// </summary>
		protected ResourceManager m_resourceManager = null;

		/// <summary>
		/// Stores browse positions for incomplete browse operations.
		/// </summary>
		Hashtable m_positions = new Hashtable();

		//======================================================================
		// Private Methods

		/// <summary>
		/// Returns a localized string with the specified name.
		/// </summary>
		private string GetString(string name, string locale)
		{
			// create a culture object.
			CultureInfo culture = null;
			
			try   { culture = new CultureInfo(locale); }
			catch {	culture = new CultureInfo(""); }

			// lookup resource string.
			try   { return m_resourceManager.GetString(name, culture); }
			catch {	return null; }
		}

		/// <summary>
		/// Initializes a reply object.
		/// </summary>
		private ReplyBase CreateReply(string locale, string clientRequestHandle)
		{
			ReplyBase reply = new ReplyBase();

			reply.ClientRequestHandle = clientRequestHandle;
			reply.RcvTime             = DateTime.Now;
			reply.ReplyTime           = DateTime.MinValue;
			reply.ServerState         = m_status.ServerState;
			reply.RevisedLocaleID     = Opc.Da.Server.FindBestLocale(locale, m_supportedLocales);

			return reply;
		}

		/// <summary>
		/// Checks that the server is aply to process requests.
		/// </summary>
		private void CheckState(string locale, bool isDataRequest)
		{
			if (isDataRequest)
			{
				if (m_status.ServerState != serverState.running && m_status.ServerState != serverState.test)
				{
					throw CreateException(locale, Error.E_SERVERSTATE);
				}
			}
			else
			{
				if (m_status.ServerState == serverState.failed)
				{
					throw CreateException(locale, Error.E_SERVERSTATE);
				}
			}
		}

		/// <summary>
		/// Creates a SOAP exception for the specified error.
		/// </summary>
		private Exception CreateException(string locale, XmlQualifiedName error)
		{
			return new SoapException(GetString(error.Name, locale), error);
		}

		/// <summary>
		/// Creates a SOAP exception for the specified exception.
		/// </summary>
		private Exception CreateException(string locale, Exception e)
		{
			// map unified DA results onto XML-DA errors.
			ResultIDException re = null;

			try   { re = (ResultIDException)e; }
			catch {	return new SoapException(e.Message, Error.E_FAIL, e); }
				
			if (re.Result == ResultID.E_FAIL)                     return CreateException(locale, Error.E_FAIL);
			if (re.Result == ResultID.E_INVALID_ITEM_NAME)        return CreateException(locale, Error.E_INVALID_ITEM_NAME);
			if (re.Result == ResultID.E_INVALID_ITEM_PATH)        return CreateException(locale, Error.E_INVALID_ITEM_PATH);
			if (re.Result == ResultID.E_UNKNOWN_ITEM_NAME)        return CreateException(locale, Error.E_UNKNOWN_ITEM_NAME);
			if (re.Result == ResultID.E_UNKNOWN_ITEM_PATH)        return CreateException(locale, Error.E_UNKNOWN_ITEM_PATH);
			if (re.Result == ResultID.E_INVALID_FILTER)           return CreateException(locale, Error.E_INVALIDFILTER);
			if (re.Result == ResultID.E_INVALIDCONTINUATIONPOINT) return CreateException(locale, Error.E_INVALIDCONTINUATIONPOINT);
			if (re.Result == ResultID.E_TIMEDOUT)                 return CreateException(locale, Error.E_TIMEDOUT);
			if (re.Result == ResultID.E_OUTOFMEMORY)              return CreateException(locale, Error.E_OUTOFMEMORY);

			// throw a generic error exception.
			throw new SoapException(re.Result.ToString(), Error.E_FAIL, e);
		}

		/// <summary>
		/// Updates the item objects with values specified at the list level.
		/// </summary>
		private void ApplyItemListDefaults(ItemList list)
		{
			foreach (Item item in list)
			{
				if (item.ReqType == null)
				{
					item.ReqType = list.ReqType;
				}
				
				if (!item.MaxAgeSpecified)
				{
					item.MaxAge          = list.MaxAge;
					item.MaxAgeSpecified = list.MaxAgeSpecified;
				}

				if (!item.DeadbandSpecified)
				{
					item.Deadband          = list.Deadband;
					item.DeadbandSpecified = list.DeadbandSpecified;
				}

				if (!item.SamplingRateSpecified)
				{
					item.SamplingRate          = list.SamplingRate;
					item.SamplingRateSpecified = list.SamplingRateSpecified;
				}

				if (!item.EnableBufferingSpecified)
				{
					item.EnableBuffering = list.EnableBuffering;
					item.EnableBufferingSpecified = list.EnableBufferingSpecified;
				}
			}
		}

		/// <summary>
		/// Generates a list of error results if the deadline has already passed.
		/// </summary>
		private ItemValueResultList CheckDeadline(string locale, DateTime deadline, object requestList)
		{
			// check for trivial case.
			if (deadline == DateTime.MinValue || requestList == null)
			{
				return null;
			}

			// check if deadline has already passed.
			if (deadline < DateTime.Now)
			{
				throw CreateException(locale, Error.E_TIMEDOUT);
			}

			// check if there is enough time left to complete the request.
			// the value used here is arbitraty - picked to demonstrate how to use the deadline.
			if (((TimeSpan)(deadline - DateTime.Now)).TotalSeconds > 1)
			{
				return null;
			}

			// create result list.
			ItemValueResultList replyList = new ItemValueResultList();

			foreach (ItemIdentifier requestItem in (ICollection)requestList)
			{
				ItemValueResult replyItem = new ItemValueResult(requestItem);

				replyItem.Value              = null;
				replyItem.Quality            = Quality.Bad;
				replyItem.QualitySpecified   = false;
				replyItem.Timestamp          = DateTime.MinValue;
				replyItem.TimestampSpecified = false;
				replyItem.ResultID           = ResultID.E_TIMEDOUT;
				replyItem.DiagnosticInfo     = null;

				replyList.Add(replyItem);
			}

			return replyList;
		}

		/// <summary>
		/// Applies the request objects to the results.
		/// </summary>
		private Error[] ApplyOptions(string locale, RequestOptions options, object replyLists)
		{
			// check for null.
			if (replyLists == null)
			{
				return null;
			}
			
			// process single result list.
			if (replyLists.GetType() == typeof(ItemValueResultList))
			{
				foreach (ItemValueResult replyItem in (ItemValueResultList)replyLists)
				{
					// remove fields that are not requested by the client.
					if ((options.Filters & (int)ResultFilter.ItemName) == 0)       replyItem.ItemName       = null;
					if ((options.Filters & (int)ResultFilter.ItemPath) == 0)       replyItem.ItemPath       = null;
					if ((options.Filters & (int)ResultFilter.DiagnosticInfo) == 0) replyItem.DiagnosticInfo = null;

					if ((options.Filters & (int)ResultFilter.ItemTime) == 0) 
					{
						replyItem.Timestamp = DateTime.MinValue;
						replyItem.TimestampSpecified = false;
					}
				}
			}
			
			// process multiple result lists.
			else if (replyLists.GetType() == typeof(ItemValueResultList[]))
			{
				foreach (ItemValueResultList replyList in (ItemValueResultList[])replyLists)
				{
					// remove fields that are not requested by the client.
					foreach (ItemValueResult replyItem in replyList)
					{
						if ((options.Filters & (int)ResultFilter.ItemName) == 0)       replyItem.ItemName       = null;
						if ((options.Filters & (int)ResultFilter.ItemPath) == 0)       replyItem.ItemPath       = null;
						if ((options.Filters & (int)ResultFilter.DiagnosticInfo) == 0) replyItem.DiagnosticInfo = null;

						if ((options.Filters & (int)ResultFilter.ItemTime) == 0) 
						{
							replyItem.Timestamp = DateTime.MinValue;
							replyItem.TimestampSpecified = false;
						}
					}
				}
			}

			// return verbose error texts, if required.
			return ((options.Filters & (int)ResultFilter.ErrorText) != 0)?GetErrors(locale, replyLists):null;
		}

		/// <summary>
		/// Constructs an error object for a result identifier.
		/// </summary>
		private OpcXml.Da.Error GetError(string locale, ResultID resultID)
		{
			OpcXml.Da.Error error = new OpcXml.Da.Error();

			error.ID   = OpcXml.Da10.Request.GetResultID(resultID);
			error.Text = GetString(resultID.Name.Name, locale);

			if (error.Text == null || error.Text == "")
			{
				try   { error.Text = m_server.GetErrorText(locale, resultID);   }
				catch {	error.Text = String.Format("0x{0,8:X}", resultID.Code); }
			}
			
			return error;
		}

		/// <summary>
		/// Constructs an error object for a result identifier.
		/// </summary>
		private OpcXml.Da.Error[] GetErrors(string locale, object resultLists)
		{
			Hashtable resultIDs = new Hashtable();

			// check if there is nothing to do.
			if (resultLists == null) { return null; }

			// search item value result lists.
			if (resultLists.GetType() == typeof(ItemValueResultList))
			{
				foreach (ItemValueResult resultItem in (ItemValueResultList)resultLists)
				{
					if (resultItem.ResultID != ResultID.S_OK)
					{
						if (!resultIDs.Contains(resultItem.ResultID))
						{
							resultIDs[resultItem.ResultID] = GetError(locale, resultItem.ResultID);
						}
					}
				}
			}

            // search item value result lists.
			else if (resultLists.GetType() == typeof(ItemValueResultList[]))
			{
				foreach (ItemValueResultList resultList in (ItemValueResultList[])resultLists)
				{
					foreach (ItemValueResult resultItem in resultList)
					{
						if (resultItem.ResultID != ResultID.S_OK)
						{
							if (!resultIDs.Contains(resultItem.ResultID))
							{
								resultIDs[resultItem.ResultID] = GetError(locale, resultItem.ResultID);
							}
						}
					}
				}
			}

			// search browse elements.
			else if (resultLists.GetType().GetElementType() == typeof(BrowseElement))
			{
				foreach (BrowseElement element in (Array)resultLists)
				{
					if (element.Properties == null) { continue; }

					foreach (ItemProperty property in element.Properties)
					{
						if (property.ResultID != ResultID.S_OK)
						{
							if (!resultIDs.Contains(property.ResultID))
							{
								resultIDs[property.ResultID] = GetError(locale, property.ResultID);
							}
						}
					}
				}
			}

			// search item property lists.
			else if (resultLists.GetType().GetElementType() == typeof(ItemPropertyCollection))
			{
				foreach (ItemPropertyCollection propertyList in (Array)resultLists)
				{
					if (propertyList == null) { continue; }

					if (propertyList.ResultID != ResultID.S_OK)
					{
						if (!resultIDs.Contains(propertyList.ResultID))
						{
							resultIDs[propertyList.ResultID] = GetError(locale, propertyList.ResultID);
						}
					}

					foreach (ItemProperty property in propertyList)
					{
						if (property.ResultID != ResultID.S_OK)
						{
							if (!resultIDs.Contains(property.ResultID))
							{
								resultIDs[property.ResultID] = GetError(locale, property.ResultID);
							}
						}
					}
				}
			}

			// construct array of unique errors.
			ArrayList errors = new ArrayList();

			foreach (OpcXml.Da.Error error in resultIDs.Values)
			{
				errors.Add(error);
			}

			return (OpcXml.Da.Error[])errors.ToArray(typeof(OpcXml.Da.Error));
		}

		/// <summary>
		/// Cleans up any inactive subscriptions.
		/// </summary>
		private void CleanupSubscriptions(object state)
		{
			lock (this)
			{
				// collect list of expired subscriptions.
				ArrayList expiredSubscriptions = new ArrayList();

				foreach (Subscription subscription in m_subscriptions.Values)
				{
					if (DateTime.Now > subscription.LastPollTime.Add(subscription.PingTime))
					{
						expiredSubscriptions.Add(subscription);
					}
				}

				// remove expired subscriptions.
				foreach (Subscription subscription in expiredSubscriptions)
				{
					m_subscriptions.Remove(subscription.ID);
					subscription.Dispose();
				}

				// cancel timer if no more subscriptions.
				if (m_subscriptions.Count == 0)
				{
					if (m_pingTimer != null)
					{
						m_pingTimer.Dispose();
						m_pingTimer = null;
					}
				}
			}
		}
	}
}
