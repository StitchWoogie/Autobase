using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using AutoLib;
using AutoLibLocal;
using PortalServerWeb.Library;

namespace PortalServerWeb.AutoWeb.Service
{
	/// <summary>
	/// 프리셋 WCF 서비스 (단일 경유점: LocalMain의 ClassDataGateServer를 경유)
	/// </summary>
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	public class WcfServicePreset : IServicePreset
	{
		void Init()
		{
			ConfigVarTotal.sSiteRootName = ConfigWeb.GetLocalIP();
			ConfigVarTotal.eServiceType = EnumServiceType.WcfService;
			ConfigVarTotal.eBindType = EnumDataGateBindingType.NetTcp;
			ConfigVarTotal.nServicePort = 8732;
		}

		public string GetPresetList(out string error)
		{
			error = null;
			try
			{
				Init();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				int retn = sldg.Command("V2_PresetGetList");
				if (retn != 1) { error = sldg.sErrorMessage; return null; }
				return sldg.GetResultString(0);
			}
			catch (Exception ex)
			{
				error = "GetPresetList: " + ex.Message;
				Debug.WriteLine("WcfServicePreset.GetPresetList error: " + ex.Message);
				return null;
			}
		}

		public string GetPreset(string presetName, out string error)
		{
			error = null;
			try
			{
				Init();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				int retn = sldg.Command("V2_PresetGet", presetName);
				if (retn != 1) { error = sldg.sErrorMessage; return null; }
				return sldg.GetResultString(0);
			}
			catch (Exception ex)
			{
				error = "GetPreset: " + ex.Message;
				Debug.WriteLine("WcfServicePreset.GetPreset error: " + ex.Message);
				return null;
			}
		}

		public bool SavePreset(string presetName, string json, out string error)
		{
			error = null;
			try
			{
				Init();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				sldg.PrepareArg1(json);
				int retn = sldg.Command("V2_PresetSave", presetName);
				if (retn != 1) { error = sldg.sErrorMessage; return false; }
				return true;
			}
			catch (Exception ex)
			{
				error = "SavePreset: " + ex.Message;
				Debug.WriteLine("WcfServicePreset.SavePreset error: " + ex.Message);
				return false;
			}
		}

		public bool DeletePreset(string presetName, out string error)
		{
			error = null;
			try
			{
				Init();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				int retn = sldg.Command("V2_PresetDelete", presetName);
				if (retn != 1) { error = sldg.sErrorMessage; return false; }
				return true;
			}
			catch (Exception ex)
			{
				error = "DeletePreset: " + ex.Message;
				Debug.WriteLine("WcfServicePreset.DeletePreset error: " + ex.Message);
				return false;
			}
		}

		public bool ApplyPreset(string presetName, string variantName, out string error)
		{
			error = null;
			try
			{
				Init();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				int retn = sldg.Command("V2_PresetApply", presetName + "," + variantName);
				if (retn != 1) { error = sldg.sErrorMessage; return false; }
				return true;
			}
			catch (Exception ex)
			{
				error = "ApplyPreset: " + ex.Message;
				Debug.WriteLine("WcfServicePreset.ApplyPreset error: " + ex.Message);
				return false;
			}
		}

		public bool CapturePreset(string templatePreset, string variantName, string saveName, out string error)
		{
			error = null;
			try
			{
				Init();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				int retn = sldg.Command("V2_PresetCapture", templatePreset + "," + variantName + "," + saveName);
				if (retn != 1) { error = sldg.sErrorMessage; return false; }
				return true;
			}
			catch (Exception ex)
			{
				error = "CapturePreset: " + ex.Message;
				Debug.WriteLine("WcfServicePreset.CapturePreset error: " + ex.Message);
				return false;
			}
		}
	}
}
