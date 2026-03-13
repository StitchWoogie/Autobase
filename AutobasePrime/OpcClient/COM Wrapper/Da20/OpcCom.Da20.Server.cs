//============================================================================
// TITLE: Server.cs
//
// CONTENTS:
// 
// An in-process wrapper for a remote OPC Data Access 2.0X server.
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
using System.Runtime.InteropServices;
using Opc;
using Opc.Da;
using OpcCom;
using OpcCom.Da;
using OpcRcw.Da;
using OpcRcw.Comn;
using NetTools;

namespace OpcCom.Da20
{
	/// <summary>
	/// An in-process wrapper for a remote OPC Data Access 2.0X server.
	/// </summary>
	public class  Server : OpcCom.Da.Server
	{	
		//======================================================================
		// Construction

		/// <summary>
		/// The default constructor for the object.
		/// </summary>
		internal Server() {}

		/// <summary>
		/// Initializes the object with the specifed COM server.
		/// </summary>
		internal Server(URL url, object server) : base(url, server) {}

		//======================================================================
		// IDisposable

		/// <summary>
		/// This must be called explicitly by clients to ensure the COM server is released.
		/// </summary>
		public override void Dispose() 
		{
			lock (this)
			{
				if (m_group != null)
				{
					Marshal.ReleaseComObject(m_group);

					try   { ((IOPCServer)m_server).RemoveGroup(m_groupHandle, 0); }
					catch {}

					m_group       = null;
					m_groupHandle = 0;

					base.Dispose();
                }
			}
		}
		
		//======================================================================
		// Connection Management

		/// <summary>
		/// Connects to the server with the specified URL and credentials.
		/// </summary>
		public override void Initialize(URL url, ConnectData connectData)
		{
			lock (this)
			{
				// connect to server.
				base.Initialize(url, connectData);

				// create a global group required for various item level operations.
				try
				{
					// get the default locale for the server.
					int localeID = 0;
					((IOPCCommon)m_server).GetLocaleID(out localeID);

					// add the group.
					int revisedUpdateRate = 0;
					Guid iid = typeof(IOPCItemMgt).GUID;

					((IOPCServer)m_server).AddGroup(
						"",
						1,
						0,
						0,
						IntPtr.Zero,
						IntPtr.Zero,
						localeID,
						out m_groupHandle,
						out revisedUpdateRate,
						ref iid,
						out m_group);
				}
				catch (Exception e)
				{
					Uninitialize();
					throw OpcCom.Interop.CreateException("IOPCServer.AddGroup", e);
				}
			}
		}

		//======================================================================
		// Private Members

		/// <summary>
		/// A global group used for various item level operations. 
		/// </summary>
		private object m_group = null;

		/// <summary>
		/// The server handle for the global group.
		/// </summary>
		private int m_groupHandle = 0;

		//======================================================================
		// Read

		/// <summary>
		/// Reads the values for the specified items.
		/// </summary>
		public override ItemValueResult[] Read(Item[] items)
		{
			if (items == null) throw new ArgumentNullException("items");

			// check if nothing to do.
			if (items.Length == 0)
			{
				return new ItemValueResult[0];
			}

			lock (this)
			{
				if (m_server == null) throw new NotConnectedException();

				// create temporary items.
				IdentifiedResult[] temporaryItems = AddItems(items);
				ItemValueResult[]  results        = new ItemValueResult[items.Length];

				try
				{
					// construct return values.
					ArrayList cacheReads  = new ArrayList(items.Length);
					ArrayList deviceReads = new ArrayList(items.Length);

					for (int ii = 0; ii < items.Length; ii++)
					{
						results[ii] = new ItemValueResult(temporaryItems[ii]);

						if (temporaryItems[ii].ResultID.Failed())
						{
							results[ii].ResultID       = temporaryItems[ii].ResultID;
							results[ii].DiagnosticInfo = temporaryItems[ii].DiagnosticInfo;
							continue;
						}

						if (items[ii].MaxAgeSpecified && items[ii].MaxAge < 0)
						{
							cacheReads.Add(results[ii]);
						}
						else
						{
							deviceReads.Add(results[ii]);
						}
					}

					// read values from the cache.
					if (cacheReads.Count > 0)
					{
						// items must be active for cache reads.
						try
						{
							// create list of server handles.
							int[] serverHandles = new int[cacheReads.Count];

							for (int ii = 0; ii < cacheReads.Count; ii++)
							{
								serverHandles[ii] = (int)((ItemValueResult)cacheReads[ii]).ServerHandle;
							}

							IntPtr pErrors = IntPtr.Zero;

							((IOPCItemMgt)m_group).SetActiveState(
								cacheReads.Count,
								serverHandles,
								1,
								out pErrors);

							// free error array.
							Marshal.FreeCoTaskMem(pErrors);
						}
						catch (Exception e)
						{
							throw OpcCom.Interop.CreateException("IOPCItemMgt.SetActiveState", e);
						}

						// read the values.
						ReadValues((ItemValueResult[])cacheReads.ToArray(typeof(ItemValueResult)), true);
					}

					// read values from the device.
					if (deviceReads.Count > 0)
					{
						ReadValues((ItemValueResult[])deviceReads.ToArray(typeof(ItemValueResult)), false);
					}
				}

				// remove temporary items after read.
				finally
				{
					RemoveItems(temporaryItems);
				}

				// return results.
				return results;
			}
		}

		//======================================================================
		// Write

		/// <summary>
		/// Write the values for the specified items.
		/// </summary>
		public override IdentifiedResult[] Write(ItemValue[] items)
		{
			if (items == null) throw new ArgumentNullException("items");

			// check if nothing to do.
			if (items.Length == 0)
			{
				return new IdentifiedResult[0];
			}

			lock (this)
			{
				if (m_server == null) throw new NotConnectedException();

				// create item objects to add temporary items.
				Item[] groupItems = new Item[items.Length];

				for (int ii = 0; ii < items.Length; ii++)
				{
					groupItems[ii] = new Item(items[ii]);
				}

				// create temporary items.
				IdentifiedResult[] results = AddItems(groupItems);

				try
				{
					// construct list of valid items to write.
					ArrayList writeItems  = new ArrayList(items.Length);
					ArrayList writeValues = new ArrayList(items.Length);

					for (int ii = 0; ii < items.Length; ii++)
					{
						if (results[ii].ResultID.Failed())
						{
							continue;
						}

						if (items[ii].QualitySpecified || items[ii].TimestampSpecified)
						{
							results[ii].ResultID       = ResultID.E_NO_WRITEQT;
							results[ii].DiagnosticInfo = null;
							continue;
						}

						writeItems.Add(results[ii]);
						writeValues.Add(items[ii]);
					}

					// read values from the cache.
					if (writeItems.Count > 0)
					{
						// initialize input parameters.
						int[]    serverHandles = new int[writeItems.Count];
						object[] values        = new object[writeItems.Count];

						for (int ii = 0; ii < serverHandles.Length; ii++)
						{
							serverHandles[ii] = (int)((IdentifiedResult)writeItems[ii]).ServerHandle;
							values[ii]        = OpcCom.Interop.GetVARIANT(((ItemValue)writeValues[ii]).Value);
						}
						
						IntPtr pErrors = IntPtr.Zero;

						// write item values.
						try
						{
							((IOPCSyncIO)m_group).Write(
								writeItems.Count,
								serverHandles,
								values,
								out pErrors);
						}
						catch (Exception e)
						{
							throw OpcCom.Interop.CreateException("IOPCSyncIO.Write", e);
						}

						// unmarshal results.
						int[] errors = OpcCom.Interop.GetInt32s(ref pErrors, writeItems.Count, true);

						for (int ii = 0; ii < writeItems.Count; ii++)
						{
							IdentifiedResult result = (IdentifiedResult)writeItems[ii];

							result.ResultID       = OpcCom.Interop.GetResultID(errors[ii]);
							result.DiagnosticInfo = null;
							
							// convert COM code to unified DA code.
							if (errors[ii] == ResultIDs.E_BADRIGHTS) { results[ii].ResultID = new ResultID(ResultID.E_READONLY, ResultIDs.E_BADRIGHTS); }
						}
					}
				}

				// remove temporary items
				finally
				{
					RemoveItems(results);
				}

				// return results.
				return results;
			}
		}
	
		//======================================================================
		// Browse

		/// <summary>
		/// Fetches child elements of the specified branch which match the filter criteria.
		/// </summary>
		public override BrowseElement[] Browse(
			ItemIdentifier            itemID,
			BrowseFilters             filters, 
			out Opc.Da.BrowsePosition position)
		{		
			if (filters == null) throw new ArgumentNullException("filters");	

			position = null;

			lock (this)
			{
				if (m_server == null) throw new NotConnectedException();

				OpcCom.Da20.BrowsePosition pos = null;

				ArrayList elements = new ArrayList();

				// search for child branches.
				if (filters.BrowseFilter != browseFilter.item)
				{
					BrowseElement[] branches = GetElements(itemID, filters, true, ref pos);
					
					if (branches != null)
					{
						elements.AddRange(branches);
					}

					position = pos;

					// return current set if browse halted.
					if (position != null)
					{
						return (BrowseElement[])elements.ToArray(typeof(BrowseElement));
					}
				}

				// search for child items.
				if (filters.BrowseFilter != browseFilter.branch)
				{
					BrowseElement[] items = GetElements(itemID, filters, false, ref pos);

					if (items != null)
					{
						elements.AddRange(items);
					}
					
					position = pos;
				}
				
				// return the elements.
				return (BrowseElement[])elements.ToArray(typeof(BrowseElement));
			}
		}



		//======================================================================
		// BrowseNext

		/// <summary>
		/// Continues a browse operation with previously specified search criteria.
		/// </summary>
		public override BrowseElement[] BrowseNext(ref Opc.Da.BrowsePosition position)
		{	
			lock (this)
			{
				if (m_server == null) throw new NotConnectedException();
				
				// check for valid browse position object.
				if (position == null && position.GetType() != typeof(OpcCom.Da20.BrowsePosition)) 
				{
					throw new BrowseCannotContinueException();
				}
				
				OpcCom.Da20.BrowsePosition pos = (OpcCom.Da20.BrowsePosition)position;

				ItemIdentifier itemID  = pos.ItemID;
				BrowseFilters  filters = pos.Filters; 

				ArrayList elements = new ArrayList();

				// search for child branches.
				if (pos.IsBranch)
				{
					BrowseElement[] branches = GetElements(itemID, filters, true, ref pos);
					
					if (branches != null)
					{
						elements.AddRange(branches);
					}

					position = pos;

					// return current set if browse halted.
					if (position != null)
					{
						return (BrowseElement[])elements.ToArray(typeof(BrowseElement));
					}
				}

				// search for child items.
				if (filters.BrowseFilter != browseFilter.branch)
				{
					BrowseElement[] items = GetElements(itemID, filters, false, ref pos);

					if (items != null)
					{
						elements.AddRange(items);
					}

					position = pos;
				}
				
				// return the elements.
				return (BrowseElement[])elements.ToArray(typeof(BrowseElement));
			}
		}

		//======================================================================
		// GetProperties

		/// <summary>
		/// Returns the specified properties for a set of items.
		/// </summary>
		public override ItemPropertyCollection[] GetProperties(
			ItemIdentifier[] itemIDs,
			PropertyID[]     propertyIDs,
			bool             returnValues)
		{		
			if (itemIDs == null) throw new ArgumentNullException("itemIDs");

			// check for trival case.
			if (itemIDs.Length == 0)
			{
				return new ItemPropertyCollection[0];
			}

			lock (this)
			{
				if (m_server == null) throw new NotConnectedException();

				// initialize list of property lists.
				ItemPropertyCollection[] propertyLists = new ItemPropertyCollection[itemIDs.Length];

				for (int ii = 0; ii < itemIDs.Length; ii++)
				{
					propertyLists[ii] = new ItemPropertyCollection();

					propertyLists[ii].ItemName = itemIDs[ii].ItemName;
					propertyLists[ii].ItemPath = itemIDs[ii].ItemPath;

					// fetch properties for item.
					try
					{
						ItemProperty[] properties = GetProperties(itemIDs[ii].ItemName, propertyIDs, returnValues);

						if (properties != null)
						{
							propertyLists[ii].AddRange(properties);
						}

						propertyLists[ii].ResultID = ResultID.S_OK;
					}
					catch (Exception e)
					{
						propertyLists[ii].ResultID = new ResultID(Marshal.GetHRForException(e));
					}
				}

				// return property lists.
				return propertyLists;
			}
		}
		
		//======================================================================
		// Private Methods

		/// <summary>
		/// Adds a set of temporary items used for a read/write operation.
		/// </summary>
		private IdentifiedResult[] AddItems(Item[] items)
		{
			// add items to group.
			int count = items.Length;

			OPCITEMDEF[] definitions = OpcCom.Da.Interop.GetOPCITEMDEFs(items);

			// ensure all items are created as inactive.
			for (int ii = 0; ii < definitions.Length; ii++)
			{
				definitions[ii].bActive = 0;
			}
				
			// initialize output parameters.
			IntPtr pResults = IntPtr.Zero;
			IntPtr pErrors  = IntPtr.Zero;

			try
			{
				((IOPCItemMgt)m_group).AddItems(
					count,
					definitions,
					out pResults,
					out pErrors);
			}
			catch (Exception e)
			{
				throw OpcCom.Interop.CreateException("IOPCItemMgt.AddItems", e);
			}

			// unmarshal output parameters.
			int[] serverHandles = OpcCom.Da.Interop.GetItemResults(ref pResults, count, true);
			int[] errors        = OpcCom.Interop.GetInt32s(ref pErrors,  count, true);
			
			// create results list.
			IdentifiedResult[] results = new IdentifiedResult[count];

			for (int ii = 0; ii < count; ii++)
			{
				results[ii] = new IdentifiedResult(items[ii]);

				results[ii].ServerHandle   = null;
				results[ii].ResultID       = OpcCom.Interop.GetResultID(errors[ii]);
				results[ii].DiagnosticInfo = null;

				if (results[ii].ResultID.Succeeded())
				{
					results[ii].ServerHandle = serverHandles[ii];
				}
			}
				
			// return results.
			return results;
		}

		/// <summary>
		/// Removes a set of temporary items used for a read/write operation.
		/// </summary>
		private void RemoveItems(IdentifiedResult[] items)
		{
			try
			{
				// contruct array of valid server handles.
				ArrayList handles = new ArrayList(items.Length);

				foreach (IdentifiedResult item in items)
				{
					if (item.ResultID.Succeeded() && item.ServerHandle.GetType() == typeof(int))
					{
						handles.Add((int)item.ServerHandle);
					}
				}

				// check if nothing to do.
				if (handles.Count == 0)
				{
					return;
				}

				// remove items from server.
				IntPtr pErrors = IntPtr.Zero;

				((IOPCItemMgt)m_group).RemoveItems(
					handles.Count,
					(int[])handles.ToArray(typeof(int)),
					out pErrors);

				// free returned error array.
				OpcCom.Interop.GetInt32s(ref pErrors, handles.Count, true);
			}
			catch
			{
				// ignore errors.
			}
		}

		/// <summary>
		/// Reads a set of values.
		/// </summary>
		private void ReadValues(ItemValueResult[] items, bool cache)
		{
			if (items.Length == 0) return;

			// marshal input parameters.
			int[] serverHandles = new int[items.Length];

			for (int ii = 0; ii < items.Length; ii++) 
			{	
				serverHandles[ii] = System.Convert.ToInt32(items[ii].ServerHandle);
			}

			// initialize output parameters.
			IntPtr pValues = IntPtr.Zero;
			IntPtr pErrors = IntPtr.Zero;

			try
			{
				((IOPCSyncIO)m_group).Read(
					(cache)?OPCDATASOURCE.OPC_DS_CACHE:OPCDATASOURCE.OPC_DS_DEVICE,
					items.Length,
					serverHandles,
					out pValues,
					out pErrors);	
			}
			catch (Exception e)
			{					
				throw OpcCom.Interop.CreateException("IOPCSyncIO.Read", e);
			}

			// unmarshal output parameters.
			ItemValue[] values = OpcCom.Da.Interop.GetItemValues(ref pValues, items.Length, true);
			int[]       errors = OpcCom.Interop.GetInt32s(ref pErrors, items.Length, true);

			// construct results list.
			for (int ii = 0; ii < items.Length; ii++)
			{
				items[ii].ResultID       = OpcCom.Interop.GetResultID(errors[ii]);
				items[ii].DiagnosticInfo = null;

				// convert COM code to unified DA code.
				if (errors[ii] == ResultIDs.E_BADRIGHTS) { items[ii].ResultID = new ResultID(ResultID.E_WRITEONLY, ResultIDs.E_BADRIGHTS); }

				if (items[ii].ResultID.Succeeded())
				{
					items[ii].Value              = values[ii].Value;
					items[ii].Quality            = values[ii].Quality;
					items[ii].QualitySpecified   = values[ii].QualitySpecified;
					items[ii].Timestamp          = values[ii].Timestamp;
					items[ii].TimestampSpecified = values[ii].TimestampSpecified;
				}
			}
		}

		/// <summary>
		/// Returns the set of available properties for the item.
		/// </summary>
		private ItemProperty[] GetAvailableProperties(string itemID)
		{
			// query for available properties.
			int count = 0;

			IntPtr pPropertyIDs  = IntPtr.Zero;
			IntPtr pDescriptions = IntPtr.Zero;
			IntPtr pDataTypes    = IntPtr.Zero;

			((IOPCItemProperties)m_server).QueryAvailableProperties(
				itemID,
				out count,
				out pPropertyIDs,
				out pDescriptions,
				out pDataTypes);

			// unmarshal results.
			int[]    propertyIDs  = OpcCom.Interop.GetInt32s(ref pPropertyIDs, count, true);
			string[] descriptions = OpcCom.Interop.GetUnicodeStrings(ref pDescriptions, count, true);

			Marshal.FreeCoTaskMem(pDataTypes);

			// check for error condition.
			if (count == 0)
			{
				return null;
			}

			// initialize property objects.
			ItemProperty[] properties = new ItemProperty[count];

			for (int ii = 0; ii < count; ii++)
			{
				properties[ii] = new ItemProperty();

				properties[ii].ID          = new PropertyID(propertyIDs[ii]);
				properties[ii].Description = descriptions[ii];
				properties[ii].ItemName    = null;
				properties[ii].ItemPath    = null;
				properties[ii].ResultID    = ResultID.S_OK;
				properties[ii].Value       = null;
			}

			// return property list.
			return properties;
		}

		/// <summary>
		/// Fetches the property item id for the specified set of properties.
		/// </summary>
		private void GetItemIDs(string itemID, ItemProperty[] properties)
		{
			try
			{
				// create input arguments;
				int[] propertyIDs = new int[properties.Length];

				for (int ii = 0; ii < properties.Length; ii++)
				{
					propertyIDs[ii] = properties[ii].ID.Code;
				}

				// lookup item ids.
				IntPtr pItemIDs = IntPtr.Zero;
				IntPtr pErrors  = IntPtr.Zero;

				((IOPCItemProperties)m_server).LookupItemIDs(
					itemID,
					properties.Length,
					propertyIDs,
					out pItemIDs,
					out pErrors);

				// unmarshal results.
				string[] itemIDs = OpcCom.Interop.GetUnicodeStrings(ref pItemIDs, properties.Length, true);
				int[]    errors  = OpcCom.Interop.GetInt32s(ref pErrors, properties.Length, true);

				// update property objects.
				for (int ii = 0; ii < properties.Length; ii++)
				{
					properties[ii].ItemName = null;
					properties[ii].ItemPath = null;

					if (errors[ii] >= 0)
					{
						properties[ii].ItemName = itemIDs[ii];
					}
				}
			}
			catch
			{
				// set item ids to null for alll properties.
				foreach (ItemProperty property in properties)
				{
					property.ItemName = null;
					property.ItemPath = null;
				}
			}
		}

		/// <summary>
		/// Fetches the property values for the specified set of properties.
		/// </summary>
		private void GetValues(string itemID, ItemProperty[] properties)
		{
			try
			{
				// create input arguments;
				int[] propertyIDs = new int[properties.Length];

				for (int ii = 0; ii < properties.Length; ii++)
				{
					propertyIDs[ii] = properties[ii].ID.Code;
				}

				// lookup item ids.
				IntPtr pValues = IntPtr.Zero;
				IntPtr pErrors = IntPtr.Zero;

				((IOPCItemProperties)m_server).GetItemProperties(
					itemID,
					properties.Length,
					propertyIDs,
					out pValues,
					out pErrors);

				// unmarshal results.
				object[] values = OpcCom.Interop.GetVARIANTs(ref pValues, properties.Length, true);
				int[]    errors = OpcCom.Interop.GetInt32s(ref pErrors, properties.Length, true);

				// update property objects.
				for (int ii = 0; ii < properties.Length; ii++)
				{
					properties[ii].Value    = null;
					properties[ii].ResultID = OpcCom.Interop.GetResultID(errors[ii]); 

					// substitute property reult code.
					if (errors[ii] == ResultIDs.E_BADRIGHTS) { properties[ii].ResultID = new ResultID(ResultID.E_WRITEONLY, ResultIDs.E_BADRIGHTS); }

					if (properties[ii].ResultID.Succeeded())
					{
						properties[ii].Value = OpcCom.Da.Interop.GetPropertyValue(properties[ii].ID, values[ii]);
					}
				}
			}
			catch (Exception e)
			{
				// set general error code as the result for each property.
				ResultID result = new ResultID(Marshal.GetHRForException(e));

				foreach (ItemProperty property in properties)
				{
					property.Value    = null;
					property.ResultID = result;
				}
			}
		}

		/// <summary>
		/// Gets the specified properties for the specified item.
		/// </summary>
		private ItemProperty[] GetProperties(string itemID, PropertyID[] propertyIDs, bool returnValues)
		{
			ItemProperty[] properties = null;

			// return all available properties.
			if (propertyIDs == null)
			{
				properties = GetAvailableProperties(itemID);
			}

			// return on the selected properties.
			else
			{
				// get available properties.
				ItemProperty[] availableProperties = GetAvailableProperties(itemID);

				// initialize result list.
				properties = new ItemProperty[propertyIDs.Length];

				for (int ii = 0; ii < propertyIDs.Length; ii++)
				{
					// search available property list for specified property.
					foreach (ItemProperty property in availableProperties)
					{
						if (property.ID == propertyIDs[ii])
						{
							properties[ii]    = (ItemProperty)property.Clone();
							properties[ii].ID = propertyIDs[ii];
							break;
						}
					}

					// property not valid for the item.
					if (properties[ii] == null)
					{
						properties[ii] = new ItemProperty();

						properties[ii].ID       = propertyIDs[ii];
						properties[ii].ResultID = ResultID.E_INVALID_PID;
					}
				}
			}

			// fill in missing fields in property objects.
			if (properties != null)
			{
				GetItemIDs(itemID, properties);

				if (returnValues)
				{
					GetValues(itemID, properties);
				}
			}

			// return property list.
			return properties;
		}

		private void SetItemBrowserPos(string path)
		{
			IOPCBrowseServerAddressSpace browser = (IOPCBrowseServerAddressSpace)m_server;
			OPCNAMESPACETYPE namespaceType = OPCNAMESPACETYPE.OPC_NS_HIERARCHIAL;
			
			try
			{
				browser.QueryOrganization(out namespaceType);
			}
			catch (Exception e)
			{
				throw OpcCom.Interop.CreateException("IOPCBrowseServerAddressSpace.QueryOrganization", e);
			}
			SetOneItemBrowserPos("", OPCBROWSEDIRECTION.OPC_BROWSE_UP);		// root 로 이동
			//if(path == null) return;
			
			if (namespaceType == OPCNAMESPACETYPE.OPC_NS_HIERARCHIAL)
			{
				if(SetOneItemBrowserPos(path, OPCBROWSEDIRECTION.OPC_BROWSE_TO)) return;	// full name 로 이동 성공 시 return
				CommaBlockString	comma = new CommaBlockString();	//full name으로 이동실패 시 하나의 branch씩 이동

				comma.Set(path);
				comma.SetBlockCode('/');
				string		name = "";
				while(true) 
				{
					comma.GetString(ref name);
					if(SetOneItemBrowserPos(name, OPCBROWSEDIRECTION.OPC_BROWSE_DOWN) == false) break;// 이동 실패시 return
					if(comma.IsEOS()) break;
				}
			}
			// generate error for any value other that the 'root' item for flat address spaces.
			else return;
		}

		/*bool isGuidName(string name)
		{
			if(name == null) return false;
			for(int i = 0; i < name.Length; i++) 
			{
				if(name[i] == '{') return true;				
			}
			return false;
		}

		string getOneGuidName(string name, bool bFirst)
		{
			//bool		bFlag = false;
			for(int i = 0; i < name.Length; i++) 
			{
				if(name[i] == '{') 
				{
					//return name.Substring(i);
					//if(bFlag) 
					//{
						if(bFirst) return name.Substring(0, i);
						else return name.Substring(i, name.Length-i);
					//}
					//bFlag = true;
					//continue;
				}
			}
			return name;
		}

		string excludeGuid(string name)
		{
			string		ex = "";
			int			pos = 0, length = 0;
			for(int i = 0; i < name.Length; i++) 
			{
				if(name[i] == '{' || name[i] == '}') 
				{
					if(i == 0 || length == 0) continue;
					ex += name.Substring(pos, length);
					pos = i+1;
					length = 0;
					continue;
				}
				length++;
			}
			if(length == 0) return ex;
			ex += name.Substring(pos, length);
			return ex;
		}*/


		private bool SetOneItemBrowserPos(string itemName, OPCBROWSEDIRECTION dir)
		{
			IOPCBrowseServerAddressSpace browser = (IOPCBrowseServerAddressSpace)m_server;
			
			try 
			{
				if(dir == OPCBROWSEDIRECTION.OPC_BROWSE_UP || itemName == null || itemName.Length <= 0) 
				{
					int count = 0;
					while(true) 
					{
						browser.ChangeBrowsePosition(dir, "");
						count++;
						if(count > 50) break;
					}
					return true;
				}
				else 
				{					
					browser.ChangeBrowsePosition(dir, (itemName != null)?itemName:"");
					return true;
				}				
			}
			catch 
			{
				//throw OpcCom.Interop.CreateException("IOPCBrowseServerAddressSpace.ChangeBrowsePosition", e);
				return false;
			}			
			
			/*try
			{
				browser.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_TO, (itemName != null)?itemName:"");
				return true;
			}
			catch (Exception e)
			{
				// attempt to browse down if browse to fails (needed to support some non-compliant servers). 
				if(bFullPath) return false;
				try
				{
					browser.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_DOWN, (itemName != null)?itemName:"");
					return true;
				}
				catch
				{
					if(bFullPath) return false;
					throw OpcCom.Interop.CreateException("IOPCBrowseServerAddressSpace.ChangeBrowsePosition", e);					
				}
			}	*/		
		}

		/// <summary>
		/// Returns an enumerator for the children of the specified branch.
		/// </summary>
		private EnumString GetEnumerator(string itemID, BrowseFilters filters, bool branches)
		{
			IOPCBrowseServerAddressSpace browser = (IOPCBrowseServerAddressSpace)m_server;

			// check the server address space type.
			/*OPCNAMESPACETYPE namespaceType = OPCNAMESPACETYPE.OPC_NS_HIERARCHIAL;

			try
			{
				browser.QueryOrganization(out namespaceType);
			}
			catch (Exception e)
			{
				throw OpcCom.Interop.CreateException("IOPCBrowseServerAddressSpace.QueryOrganization", e);
			}

			// move to the specified branch for hierarchial address spaces.
			if (namespaceType == OPCNAMESPACETYPE.OPC_NS_HIERARCHIAL)
			{
				try
				{
					browser.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_TO, (itemID != null)?itemID:"");
				}
				catch (Exception e)
				{
					// attempt to browse down if browse to fails (needed to support some non-compliant servers). 
					try
					{
						browser.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_DOWN, (itemID != null)?itemID:"");
					}
					catch
					{
						throw OpcCom.Interop.CreateException("IOPCBrowseServerAddressSpace.ChangeBrowsePosition", e);
					}
				}				
			}

				// generate error for any value other that the 'root' item for flat address spaces.
			else
			{
				if (itemID != null && itemID != "")
				{
					throw new ExternalException("Invalid item id for branch.", OpcCom.Da.ResultIDs.E_FAIL);
				}
			}*/

			try
			{
				// create the enumerator.
				OpcRcw.Da.IEnumString enumerator = null;

				browser.BrowseOPCItemIDs(
					(branches)?OPCBROWSETYPE.OPC_BRANCH:OPCBROWSETYPE.OPC_LEAF,
					(filters.ElementNameFilter != null)?filters.ElementNameFilter:"",
					(short)VarEnum.VT_EMPTY,
					0,
					out enumerator);

				// return the enumerator.
				return new EnumString(enumerator);
			}
			catch (Exception e)
			{
				throw OpcCom.Interop.CreateException("IOPCBrowseServerAddressSpace.BrowseOPCItemIDs", e);
			}
		}

		/// <summary>
		/// Reads a single value from the enumerator and returns a browse element.
		/// </summary>
		private BrowseElement GetElement(BrowseFilters filters, EnumString enumerator, bool isBranch)
		{
			// fetch next element name.
			string[] names = enumerator.Next(1);

			if (names == null || names.Length == 0)
			{
				return null;
			}

			BrowseElement element = new BrowseElement();

			element.Name        = names[0];
			element.HasChildren = isBranch;
			element.ItemPath    = null;

			try
			{
				// get item id.
				string itemName = null;
				//if(element.
				if(isBranch == false) 
				{
					//if(String.Compare(element.Name, "NOT_USED", false) == 0) 
					//{
					//	element.ItemName = "{"+Guid.Empty.ToString()+"}" + element.Name.Substring(8);
					//}/
					//else {
						((IOPCBrowseServerAddressSpace)m_server).GetItemID(element.Name, out itemName);
						element.ItemName = itemName;
					//}
				}
				else
				//if(itemName == null || itemName.Length == 0)	// add by kdy
					element.ItemName = element.Name;
				//else
				//	element.ItemName = itemName;

				// check if element is an actual item or just a branch.
				OPCITEMDEF definition = new OPCITEMDEF();

				definition.szItemID            = element.ItemName; 
				definition.szAccessPath        = null;
				definition.hClient             = 0;
				definition.bActive             = 0;
				definition.vtRequestedDataType = (short)VarEnum.VT_EMPTY;
				definition.dwBlobSize          = 0;
				definition.pBlob               = IntPtr.Zero;

				IntPtr pResults = IntPtr.Zero;
				IntPtr pErrors  = IntPtr.Zero;

				// validate item.
				((IOPCItemMgt)m_group).ValidateItems(
					1,
					new OPCITEMDEF[] { definition },
					0,
					out pResults,
					out pErrors);

				// free results.
				OpcCom.Da.Interop.GetItemResults(ref pResults, 1, true);

				int[] errors = OpcCom.Interop.GetInt32s(ref pErrors, 1, true);

				// can only be an item if validation succeeded.
				//element.IsItem = (errors[0] >= 0);//
				element.IsItem = !isBranch;		// modify by kdy 2004-03-08
			}

			// this is an error that should not occur - setting the item name to null is a graceful failure.
			catch
			{
				element.ItemName = null;
			}

			// fetch item properties.
			try
			{
				if (filters.ReturnAllProperties)
				{
					element.Properties = GetProperties(element.ItemName, null, filters.ReturnPropertyValues);
				}
				else if (filters.PropertyIDs != null)
				{
					element.Properties = GetProperties(element.ItemName, filters.PropertyIDs, filters.ReturnPropertyValues);
				}
			}

			// return no properties if an error fetching properties occurred.
			catch
			{
				element.Properties = null;
			}

			// return new element.
			return element;
		}

		bool checkItemNameIsPath(string itemName)
		{
			if(itemName == null || itemName.Length == 0) return false;
			for(int i = 0; i < itemName.Length; i++)
			{
				if(itemName[i] == '/') return true;
			}
			return false;
		}

		/// <summary>
		/// Returns a list of child elements that meet the filter criteria.
		/// </summary>
		private BrowseElement[] GetElements(
			ItemIdentifier                 itemID, 
			BrowseFilters                  filters, 
			bool                           branches, 
			ref OpcCom.Da20.BrowsePosition position)
		{
			// get the enumerator.
			EnumString enumerator = null;
			
			if (position == null)
			{
				//SetItemBrowserPos(itemID);
				string	name = "";
				if(itemID != null && itemID.ItemName != null) 
				{
					if(checkItemNameIsPath(itemID.ItemName)) 
					{						
						name = itemID.ItemName;
					}
					else 
					{
						if(itemID.prevItemName == null || itemID.prevItemName.Length == 0)
							name = itemID.ItemName;
						else
							name = itemID.prevItemName + "/" + itemID.ItemName;
					}
				}
				SetItemBrowserPos(name);
				enumerator = GetEnumerator((name != null)?name:null, filters, branches);
				//enumerator = GetEnumerator((itemID != null)?itemID.ItemName:null, filters, branches);
			}
			else
			{
				enumerator = position.Enumerator;
				position = null;
			}

			ArrayList elements = new ArrayList();

			// read elements one at a time.
			BrowseElement element = null;

			do
			{
				// get next element.
				element = GetElement(filters, enumerator, branches);
						
				if (element != null)
				{
					elements.Add(element);

					// check if max returned elements is exceeded.
					if (filters.MaxElementsReturned != 0 && filters.MaxElementsReturned == elements.Count)
					{
						position = new OpcCom.Da20.BrowsePosition(itemID, filters, enumerator, branches);
						break;
					}
				}
			}
			while (element != null);

			// free enumerator.
			if (position == null)
			{
				enumerator.Dispose();
			}

			// return list of elements.
			return (BrowseElement[])elements.ToArray(typeof(BrowseElement));
		}

		//======================================================================
		// Private Methods

		/// <summary>
		/// Creates a new instance of a subscription.
		/// </summary>
		protected override OpcCom.Da.Subscription CreateSubscription(
			object            group, 
			SubscriptionState state, 
			int               filters)
		{
			return new OpcCom.Da20.Subscription(group, state, filters);
		}
	}

	/// <summary>
	/// Implements an object that handles multi-step browse operations for DA2.05 servers.
	/// </summary>
	[Serializable]
	internal class BrowsePosition : Opc.Da.BrowsePosition
	{
		/// <summary>
		/// The enumerator for a browse operation.
		/// </summary>
		internal EnumString Enumerator = null;

		/// <summary>
		/// Whether the current enumerator returns branches or leaves.
		/// </summary>
		internal bool IsBranch = true;

		/// <summary>
		/// Initializes a browse position 
		/// </summary>
		internal BrowsePosition(
			ItemIdentifier itemID, 
			BrowseFilters  filters, 
			EnumString     enumerator, 
			bool           isBranch) 
			: 
			base(itemID, filters)
		{
			Enumerator = enumerator;
			IsBranch   = isBranch;
		}

		/// <summary>
		/// Releases unmanaged resources held by the object.
		/// </summary>
		public override void Dispose() 
		{
			if (Enumerator != null)
			{
				Enumerator.Dispose();
				Enumerator = null;
			}
		} 

		/// <summary>
		/// Creates a deep copy of the object.
		/// </summary>
		public override object Clone() 
		{ 
			BrowsePosition clone = (BrowsePosition)MemberwiseClone();
			clone.Enumerator = Enumerator.Clone();
			return clone;
		}
	}	
}
