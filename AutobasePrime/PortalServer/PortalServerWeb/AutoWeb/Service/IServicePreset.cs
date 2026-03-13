using System.ServiceModel;

namespace PortalServerWeb.AutoWeb.Service
{
	[ServiceContract]
	public interface IServicePreset
	{
		[OperationContract]
		string GetPresetList(out string error);

		[OperationContract]
		string GetPreset(string presetName, out string error);

		[OperationContract]
		bool SavePreset(string presetName, string json, out string error);

		[OperationContract]
		bool DeletePreset(string presetName, out string error);

		[OperationContract]
		bool ApplyPreset(string presetName, string variantName, out string error);

		[OperationContract]
		bool CapturePreset(string templatePreset, string variantName, string saveName, out string error);
	}
}
