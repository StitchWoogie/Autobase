using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using AutoLib;
using AutoLibLocal;
using PortalServerWeb.Library;

namespace PortalServerWeb.AutoWeb.Service
{
	/// <summary>
	/// 레시피 WCF 서비스
	/// </summary>
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
	public class WcfServiceRecipe : IServiceRecipe
	{
		/// <summary>
		/// 레시피 목록 조회
		/// </summary>
		public DataSet GetRecipeList(out string error)
		{
			error = null;

			try
			{
				string work_dir = ProjectLib.GetWorkDir(System.Web.HttpContext.Current.Request);
				TotalConfig.sDirWorkProject = work_dir;

				var db = DataPostgres.Instance;
				if (db == null)
				{
					error = "DB 연결이 없습니다.";
					return null;
				}

				// Task.Run으로 별도 스레드풀에서 실행하여 ASP.NET SynchronizationContext 데드락 방지
				ArrayList list = Task.Run(() => db.GetRecipeListAsync()).GetAwaiter().GetResult();

				DataSet ds = new DataSet("RecipeListDataSet");
				DataTable dt = new DataTable("RecipeList");
				dt.Columns.Add("recipe_id", typeof(int));
				dt.Columns.Add("recipe_name", typeof(string));
				dt.Columns.Add("description", typeof(string));
				dt.Columns.Add("created_at", typeof(DateTime));
				dt.Columns.Add("updated_at", typeof(DateTime));

				for (int i = 0; i < list.Count; i++)
				{
					RecipeInfo info = (RecipeInfo)list[i];
					dt.Rows.Add(info.recipe_id, info.recipe_name, info.description,
						info.created_at, info.updated_at);
				}

				ds.Tables.Add(dt);
				return ds;
			}
			catch (Exception ex)
			{
				error = "GetRecipeList Exception: " + ex.Message;
				Debug.WriteLine("WcfServiceRecipe.GetRecipeList 오류: " + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// 레시피 상세 조회 (Steps + Items 포함)
		/// </summary>
		public DataSet GetRecipe(int recipeId, out string error)
		{
			error = null;

			try
			{
				string work_dir = ProjectLib.GetWorkDir(System.Web.HttpContext.Current.Request);
				TotalConfig.sDirWorkProject = work_dir;

				var db = DataPostgres.Instance;
				if (db == null)
				{
					error = "DB 연결이 없습니다.";
					return null;
				}

				// Task.Run으로 별도 스레드풀에서 실행하여 ASP.NET SynchronizationContext 데드락 방지
				RecipeData recipe = Task.Run(() => db.GetRecipeAsync(recipeId)).GetAwaiter().GetResult();
				if (recipe == null)
				{
					error = "레시피를 찾을 수 없습니다.";
					return null;
				}

				return RecipeToDataSet(recipe);
			}
			catch (Exception ex)
			{
				error = "GetRecipe Exception: " + ex.Message;
				Debug.WriteLine("WcfServiceRecipe.GetRecipe 오류: " + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// 레시피 Download (단일 경유점: LocalMain의 ServiceDataGateServer 경유)
		/// </summary>
		public bool RecipeDownload(string recipeName, string clientGuid, out string error)
		{
			error = null;
			try
			{
				if (!GuidHeartbeatManager.IsClientConnected(clientGuid))
				{ error = "Client not connected or heartbeat timeout."; return false; }

				InitRecipeProxy();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				int retn = sldg.Command("V2_RecipeDownload", recipeName);
				if (retn != 1)
				{ error = sldg.sErrorMessage ?? "RecipeDownload 실패"; return false; }
				return true;
			}
			catch (Exception ex)
			{ error = "RecipeDownload Exception: " + ex.Message; return false; }
		}

		/// <summary>
		/// 레시피 Upload (단일 경유점: LocalMain의 ServiceDataGateServer 경유)
		/// </summary>
		public bool RecipeUpload(string recipeName, string clientGuid, out string error)
		{
			error = null;
			try
			{
				if (!GuidHeartbeatManager.IsClientConnected(clientGuid))
				{ error = "Client not connected or heartbeat timeout."; return false; }

				InitRecipeProxy();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				int retn = sldg.Command("V2_RecipeUpload", recipeName);
				if (retn != 1)
				{ error = sldg.sErrorMessage ?? "RecipeUpload 실패"; return false; }
				return true;
			}
			catch (Exception ex)
			{ error = "RecipeUpload Exception: " + ex.Message; return false; }
		}

		/// <summary>
		/// 레시피 Download Unit (단일 경유점: LocalMain 경유)
		/// </summary>
		public bool RecipeDownloadUnit(string recipeName, string unitName, string clientGuid, out string error)
		{
			error = null;
			try
			{
				if (!GuidHeartbeatManager.IsClientConnected(clientGuid))
				{ error = "Client not connected or heartbeat timeout."; return false; }

				InitRecipeProxy();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				int retn = sldg.Command("V2_RecipeDownload", recipeName + "," + unitName);
				if (retn != 1)
				{ error = sldg.sErrorMessage ?? "RecipeDownloadUnit 실패"; return false; }
				return true;
			}
			catch (Exception ex)
			{ error = "RecipeDownloadUnit Exception: " + ex.Message; return false; }
		}

		/// <summary>
		/// 레시피 Upload Unit (단일 경유점: LocalMain 경유)
		/// </summary>
		public bool RecipeUploadUnit(string recipeName, string unitName, string clientGuid, out string error)
		{
			error = null;
			try
			{
				if (!GuidHeartbeatManager.IsClientConnected(clientGuid))
				{ error = "Client not connected or heartbeat timeout."; return false; }

				InitRecipeProxy();
				ServiceLibSvcDataGate sldg = new ServiceLibSvcDataGate();
				int retn = sldg.Command("V2_RecipeUpload", recipeName + "," + unitName);
				if (retn != 1)
				{ error = sldg.sErrorMessage ?? "RecipeUploadUnit 실패"; return false; }
				return true;
			}
			catch (Exception ex)
			{ error = "RecipeUploadUnit Exception: " + ex.Message; return false; }
		}

		void InitRecipeProxy()
		{
			ConfigVarTotal.sSiteRootName = ConfigWeb.GetLocalIP();
			ConfigVarTotal.eServiceType = EnumServiceType.WcfService;
			ConfigVarTotal.eBindType = EnumDataGateBindingType.NetTcp;
			ConfigVarTotal.nServicePort = 8732;
		}

		/// <summary>
		/// RecipeData → DataSet 변환 (직렬화, Unit 지원)
		/// </summary>
		static DataSet RecipeToDataSet(RecipeData recipe)
		{
			DataSet ds = new DataSet("RecipeDataSet");

			// Recipe 테이블
			DataTable dtRecipe = new DataTable("Recipe");
			dtRecipe.Columns.Add("recipe_id", typeof(int));
			dtRecipe.Columns.Add("recipe_name", typeof(string));
			dtRecipe.Columns.Add("description", typeof(string));
			dtRecipe.Columns.Add("created_at", typeof(DateTime));
			dtRecipe.Columns.Add("updated_at", typeof(DateTime));
			dtRecipe.Rows.Add(recipe.recipe_id, recipe.recipe_name, recipe.description,
				recipe.created_at, recipe.updated_at);
			ds.Tables.Add(dtRecipe);

			// Unit 테이블 (ISA-88 Lite)
			DataTable dtUnit = new DataTable("Unit");
			dtUnit.Columns.Add("unit_id", typeof(int));
			dtUnit.Columns.Add("unit_name", typeof(string));
			dtUnit.Columns.Add("unit_order", typeof(int));
			dtUnit.Columns.Add("description", typeof(string));

			for (int u = 0; u < recipe.units.Count; u++)
			{
				RecipeUnitData unit = (RecipeUnitData)recipe.units[u];
				dtUnit.Rows.Add(unit.unit_id, unit.unit_name, unit.unit_order, unit.description);
			}
			ds.Tables.Add(dtUnit);

			// Step 테이블 (unit_id 컬럼 추가)
			DataTable dtStep = new DataTable("Step");
			dtStep.Columns.Add("step_id", typeof(int));
			dtStep.Columns.Add("unit_id", typeof(int));
			dtStep.Columns.Add("step_order", typeof(int));
			dtStep.Columns.Add("step_name", typeof(string));
			dtStep.Columns.Add("wait_time_ms", typeof(int));
			dtStep.Columns.Add("timeout_ms", typeof(int));
			dtStep.Columns.Add("condition_tag", typeof(string));
			dtStep.Columns.Add("condition_value", typeof(string));
			dtStep.Columns.Add("condition_type", typeof(string));

			// Item 테이블
			DataTable dtItem = new DataTable("Item");
			dtItem.Columns.Add("item_id", typeof(int));
			dtItem.Columns.Add("step_id", typeof(int));
			dtItem.Columns.Add("tag_name", typeof(string));
			dtItem.Columns.Add("set_value", typeof(string));
			dtItem.Columns.Add("value_type", typeof(string));
			dtItem.Columns.Add("item_order", typeof(int));

			// Recipe 직속 step (unit_id = 0)
			for (int s = 0; s < recipe.steps.Count; s++)
			{
				RecipeStepData step = (RecipeStepData)recipe.steps[s];
				dtStep.Rows.Add(step.step_id, 0, step.step_order, step.step_name,
					step.wait_time_ms, step.timeout_ms,
					step.condition_tag, step.condition_value, step.condition_type);

				for (int i = 0; i < step.items.Count; i++)
				{
					RecipeItemData item = (RecipeItemData)step.items[i];
					dtItem.Rows.Add(item.item_id, step.step_id, item.tag_name,
						item.set_value, item.value_type, item.item_order);
				}
			}

			// Unit 소속 step
			for (int u = 0; u < recipe.units.Count; u++)
			{
				RecipeUnitData unit = (RecipeUnitData)recipe.units[u];
				for (int s = 0; s < unit.steps.Count; s++)
				{
					RecipeStepData step = (RecipeStepData)unit.steps[s];
					dtStep.Rows.Add(step.step_id, unit.unit_id, step.step_order, step.step_name,
						step.wait_time_ms, step.timeout_ms,
						step.condition_tag, step.condition_value, step.condition_type);

					for (int i = 0; i < step.items.Count; i++)
					{
						RecipeItemData item = (RecipeItemData)step.items[i];
						dtItem.Rows.Add(item.item_id, step.step_id, item.tag_name,
							item.set_value, item.value_type, item.item_order);
					}
				}
			}

			ds.Tables.Add(dtStep);
			ds.Tables.Add(dtItem);

			return ds;
		}
	}
}
