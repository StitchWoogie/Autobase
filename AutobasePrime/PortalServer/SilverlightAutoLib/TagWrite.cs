using System;
using AutoLibLocal;
using NetTools;
using AutoLib;

namespace AutoLib
{
	/// <summary>
	/// Summary description for TagWritePublic.
	/// </summary>
	public class TagWrite
	{
		public TagWrite()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void SetTagValue(string tag, string sValue, double fValue, bool bHandOperation)
		{
			int[] pos = new int[1];

			TagPublicClass tp = TagLib.GetStructPublic(tag, ref pos);
			
			if(tp.enumTagType == EnumTagType.AI) 
			{
				TagAiClass ai = (TagAiClass)tp;
				WriteCurrAI(tag, ai, fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.AO) 
			{
				TagAoClass ao = (TagAoClass)tp;
				WriteCurrAO(tag, ao, fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{
				TagDiClass di = (TagDiClass)tp;
				WriteCurrDI(tag, di, (sbyte)fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.DO) 
			{
				TagDoClass dout = (TagDoClass)tp;
				WriteCurrDO(tag, dout, (sbyte)fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.ST) 
			{
				TagStClass st = (TagStClass)tp;
				WriteCurrST(tag, st, sValue, bHandOperation);
			}
			else 
			{

			}
		}

		public static void SetTagValue(string tag, string sValue, bool bHandOperation)
		{
			if(sValue == null)	sValue = "";

			int[] pos = new int[1];
            
			TagPublicClass tp = TagLib.GetStructPublic(tag, ref pos);

			if(tp.enumTagType == EnumTagType.AI) 
			{
				TagAiClass ai = (TagAiClass)tp;
				double fValue;
				if(sValue.Length == 0) 
				{
					fValue = 0;
				}
				else 
				{
					try 
					{
						fValue = ConvertTool.ToDouble(sValue);
					}
					catch 
					{
						fValue = 0;
					}
				}
				WriteCurrAI(tag, ai, fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.AO) 
			{
				
			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{
				double fValue;
				if(sValue.Length == 0) 
				{
					fValue = 0;
				}
				else 
				{
					try 
					{
						fValue = ConvertTool.ToDouble(sValue);
					}
					catch 
					{
						fValue = 0;
					}
				}

				TagDiClass di = (TagDiClass)tp;
				WriteCurrDI(tag, di, (sbyte)fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.DO) 
			{
				
			}
			else if(tp.enumTagType == EnumTagType.ST) 
			{
				TagStClass st = (TagStClass)tp;
				WriteCurrST(tag, st, sValue, bHandOperation);
			}
			else 
			{

			}
		}

		public delegate void LocalMainEngineSetTagValue(TagPublicClass tp, object val);	// 값을 변경하는 LocalMain Engine
		public static LocalMainEngineSetTagValue localMainEngineSetTagValue = null;	

		public static void WriteCurrAI(string tag, TagAiClass ai, double val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				localMainEngineSetTagValue(ai, val);
				return;
			}
			
			DataGate gate = new DataGate();
			gate.WriteCurrDouble(tag, ai, val, bHandOperation);
			
			ai.curr = val;
			ai.bChangedDataCurr = true;
		}

		public static void WriteCurrAO(string tag, TagAoClass ao, double val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				localMainEngineSetTagValue(ao, val);
				return;
			}

			DataGate gate = new DataGate();
			gate.WriteCurrDouble(tag, ao, val, bHandOperation);

			ao.curr = val;
			ao.bChangedDataCurr = true;
		}

		public static void WriteCurrDI(string tag, TagDiClass di, sbyte val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				localMainEngineSetTagValue(di, val);
				return;
			}

			DataGate gate = new DataGate();
			gate.WriteCurrDouble(tag, di, val, bHandOperation);

			di.curr = val;
			di.bChangedDataCurr = true;
		}

		public static void WriteCurrDO(string tag, TagDoClass dout, sbyte val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				localMainEngineSetTagValue(dout, val);
				return;
			}

			DataGate gate = new DataGate();
			gate.WriteCurrDouble(tag, dout, val, bHandOperation);

			dout.curr = val;
			dout.bChangedDataCurr = true;
		}

		public static void WriteCurrST(string tag, TagStClass st, string val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				localMainEngineSetTagValue(st, val);
				return;
			}

			DataGate gate = new DataGate();
			gate.WriteCurrString(tag, st, val, bHandOperation);

			st.curr = val;
			st.bChangedDataCurr = true;
		}

		public static void WriteCurrDoGroup(TagDoGroupClass gdo, sbyte val, bool bHandOperation)
		{/*
			if(localMainEngineSetTagValue != null)
			{
				localMainEngineSetTagValue(gdo, val);
				return;
			}

			DataGate gate = new DataGate();
			gate.WriteCurrDouble(gdo.tag, gdo, val, bHandOperation);

			gdo.curr = val;
			gdo.bChangedDataCurr = true;*/
		}

		public delegate void LocalMainEngineSetTagValueDelaySec(TagPublicClass tp, object val, int delay_sec);	// 값을 변경하는 LocalMain Engine
		public static LocalMainEngineSetTagValueDelaySec localMainEngineSetTagValueDelaySec = null;	

		public static void WriteCurr(TagPublicClass tp, object val, bool bHandOperation, int delay_sec)
		{
			if(localMainEngineSetTagValueDelaySec != null)
			{
				localMainEngineSetTagValueDelaySec(tp, val, delay_sec);
				return;
			}

			DataGate gate = new DataGate();
			gate.WriteCurr(tp, val, bHandOperation, delay_sec);

			if(tp.enumTagType == EnumTagType.AI) 
			{
				TagAiClass ai = (TagAiClass)tp;
				ai.curr = ObjectValue.ToDouble(val);
			}
			else if(tp.enumTagType == EnumTagType.AO) 
			{
				TagAoClass ao = (TagAoClass)tp;
				ao.curr = ObjectValue.ToDouble(val);
			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{
				TagDiClass di = (TagDiClass)tp;
				di.curr = ObjectValue.ToSByte(val);
			}
			else if(tp.enumTagType == EnumTagType.DO) 
			{
				TagDoClass dout = (TagDoClass)tp;
				dout.curr = ObjectValue.ToSByte(val);
			}
			else if(tp.enumTagType == EnumTagType.ST) 
			{
				TagStClass st = (TagStClass)tp;
				st.curr = ObjectValue.ToString(val);
			}
			else if(tp.enumTagType == EnumTagType.GDO) 
			{
				TagDoGroupClass gdo = (TagDoGroupClass)tp;
				gdo.curr = ObjectValue.ToSByte(val);
			}
			else 
			{

			}

			tp.bChangedDataCurr = true;
		}
	}
}
