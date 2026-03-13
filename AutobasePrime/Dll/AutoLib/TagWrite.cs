using System;
using System.Threading.Tasks;
using AutoLibLocal;
using NetTools;

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

		public static async Task SetTagValue(string tag, string sValue, double fValue, bool bHandOperation)
		{
			int[] pos = new int[1];

			TagPublicClass tp = TagLib.GetStructPublic(tag, ref pos);
			
			if(tp.enumTagType == EnumTagType.AI) 
			{
				TagAiClass ai = (TagAiClass)tp;
				await WriteCurrAI(tag, ai, fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.AO) 
			{
				TagAoClass ao = (TagAoClass)tp;
				await WriteCurrAO(tag, ao, fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{
				TagDiClass di = (TagDiClass)tp;
				await WriteCurrDI(tag, di, (sbyte)fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.DO) 
			{
				TagDoClass dout = (TagDoClass)tp;
				await WriteCurrDO(tag, dout, (sbyte)fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.ST) 
			{
				TagStClass st = (TagStClass)tp;
				await WriteCurrST(tag, st, sValue, bHandOperation);
			}
			else 
			{

			}
		}

		public static async Task SetTagValue(string tag, string sValue, bool bHandOperation)
		{
			if(sValue == null)	sValue = "";

			int[] pos = new int[1];
            
			TagPublicClass tp = TagLib.GetStructPublic(tag, ref pos);

			if(tp.enumTagType == EnumTagType.AI) 
			{
				TagAiClass ai = (TagAiClass)tp;
				double fValue;
                fValue = ConvertTool.ToDouble(sValue);
				await WriteCurrAI(tag, ai, fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.AO) 
			{
                TagAoClass ao = (TagAoClass)tp;
                double fValue;
                fValue = ConvertTool.ToDouble(sValue);
                await WriteCurrAO(tag, ao, fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{
				double fValue;
                fValue = ConvertTool.ToDouble(sValue);
				TagDiClass di = (TagDiClass)tp;
				await WriteCurrDI(tag, di, (sbyte)fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.DO) 
			{
                double fValue;
                fValue = ConvertTool.ToDouble(sValue);
                TagDoClass dout = (TagDoClass)tp;
                await WriteCurrDO(tag, dout, (sbyte)fValue, bHandOperation);
			}
			else if(tp.enumTagType == EnumTagType.ST) 
			{
				TagStClass st = (TagStClass)tp;
				await WriteCurrST(tag, st, sValue, bHandOperation);
			}
			else 
			{

			}
		}

        public delegate Task LocalMainEngineSetTagValue(TagPublicClass tp, object val, bool bManualOperation);	// 값을 변경하는 LocalMain Engine
		public static LocalMainEngineSetTagValue localMainEngineSetTagValue = null;	

		public static async Task WriteCurrAI(string tag, TagAiClass ai, double val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				await localMainEngineSetTagValue(ai, val, bHandOperation);
				return;
			}
			
			DataGate gate = new DataGate();
			await gate.WriteCurrDouble(tag, ai, val, bHandOperation);
			
            // ViewMain에서 값 변경후 값을 바로 볼수 있게 값을 바로 바꾸어 주는 기능을 선택할 수 있게 수정. 2023-11-22 kks
			if(ConfigViewMain.bEnableSettingValuePreview)
                ai.curr = val;

			ai.bChangedDataCurr = true;
		}

		public static async Task WriteCurrAO(string tag, TagAoClass ao, double val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				await localMainEngineSetTagValue(ao, val, bHandOperation);
				return;
			}

			DataGate gate = new DataGate();
			await gate.WriteCurrDouble(tag, ao, val, bHandOperation);

            // ViewMain에서 값 변경후 값을 바로 볼수 있게 값을 바로 바꾸어 주는 기능을 선택할 수 있게 수정. 2023-11-22 kks
            if (ConfigViewMain.bEnableSettingValuePreview)
			    ao.curr = val;

			ao.bChangedDataCurr = true;
		}

		public static async Task WriteCurrDI(string tag, TagDiClass di, sbyte val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				await localMainEngineSetTagValue(di, val, bHandOperation);
				return;
			}

			DataGate gate = new DataGate();
			await gate.WriteCurrDouble(tag, di, val, bHandOperation);

            // ViewMain에서 값 변경후 값을 바로 볼수 있게 값을 바로 바꾸어 주는 기능을 선택할 수 있게 수정. 2023-11-22 kks
            if (ConfigViewMain.bEnableSettingValuePreview)
			    di.curr = val;

			di.bChangedDataCurr = true;
		}

		public static async Task WriteCurrDO(string tag, TagDoClass dout, sbyte val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				await localMainEngineSetTagValue(dout, val, bHandOperation);
				return;
			}

			DataGate gate = new DataGate();
			await gate.WriteCurrDouble(tag, dout, val, bHandOperation);

            // ViewMain에서 값 변경후 값을 바로 볼수 있게 값을 바로 바꾸어 주는 기능을 선택할 수 있게 수정. 2023-11-22 kks
            if (ConfigViewMain.bEnableSettingValuePreview)
			    dout.curr = val;

			dout.bChangedDataCurr = true;
		}

		public static async Task WriteCurrST(string tag, TagStClass st, string val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				await localMainEngineSetTagValue(st, val, bHandOperation);
				return;
			}

			DataGate gate = new DataGate();
			await gate.WriteCurrString(tag, st, val, bHandOperation);

            // ViewMain에서 값 변경후 값을 바로 볼수 있게 값을 바로 바꾸어 주는 기능을 선택할 수 있게 수정. 2023-11-22 kks
            if (ConfigViewMain.bEnableSettingValuePreview)
			    st.curr = val;

			st.bChangedDataCurr = true;
		}

		public static async Task WriteCurrDoGroup(TagDoGroupClass gdo, sbyte val, bool bHandOperation)
		{
			if(localMainEngineSetTagValue != null)
			{
				await localMainEngineSetTagValue(gdo, val, bHandOperation);
				return;
			}

			DataGate gate = new DataGate();
			await gate.WriteCurrDouble(gdo.tag, gdo, val, bHandOperation);

            // ViewMain에서 값 변경후 값을 바로 볼수 있게 값을 바로 바꾸어 주는 기능을 선택할 수 있게 수정. 2023-11-22 kks
            if (ConfigViewMain.bEnableSettingValuePreview)
			    gdo.curr = val;

			gdo.bChangedDataCurr = true;
		}

		public delegate Task LocalMainEngineSetTagValueDelaySec(TagPublicClass tp, object val, int delay_sec, bool bManualOperation);	// 값을 변경하는 LocalMain Engine
		public static LocalMainEngineSetTagValueDelaySec localMainEngineSetTagValueDelaySec = null;	

		public static async Task WriteCurr(TagPublicClass tp, object val, bool bHandOperation, int delay_sec)
		{
			if(localMainEngineSetTagValueDelaySec != null)
			{
				await localMainEngineSetTagValueDelaySec(tp, val, delay_sec, bHandOperation);
				return;
			}

			DataGate gate = new DataGate();
			await gate.WriteCurr(tp, val, bHandOperation, delay_sec);

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
				di.curr = ObjectValue.ToSbyte(val);
			}
			else if(tp.enumTagType == EnumTagType.DO) 
			{
				TagDoClass dout = (TagDoClass)tp;
				dout.curr = ObjectValue.ToSbyte(val);
			}
			else if(tp.enumTagType == EnumTagType.ST) 
			{
				TagStClass st = (TagStClass)tp;
				st.curr = ObjectValue.ToString(val);
			}
			else if(tp.enumTagType == EnumTagType.GDO) 
			{
				TagDoGroupClass gdo = (TagDoGroupClass)tp;
				gdo.curr = ObjectValue.ToSbyte(val);
			}
			else 
			{

			}

			tp.bChangedDataCurr = true;
		}
	}
}
