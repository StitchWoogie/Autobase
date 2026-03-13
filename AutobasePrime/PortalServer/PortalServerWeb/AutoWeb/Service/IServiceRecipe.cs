using System.Data;
using System.ServiceModel;

namespace PortalServerWeb.AutoWeb.Service
{
	[ServiceContract]
	public interface IServiceRecipe
	{
		[OperationContract]
		DataSet GetRecipeList(out string error);

		[OperationContract]
		DataSet GetRecipe(int recipeId, out string error);

		[OperationContract]
		bool RecipeDownload(string recipeName, string clientGuid, out string error);

		[OperationContract]
		bool RecipeUpload(string recipeName, string clientGuid, out string error);

		[OperationContract]
		bool RecipeDownloadUnit(string recipeName, string unitName, string clientGuid, out string error);

		[OperationContract]
		bool RecipeUploadUnit(string recipeName, string unitName, string clientGuid, out string error);
	}
}
