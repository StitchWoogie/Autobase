using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoLibLocal
{
    #region Recipe CRUD

    public partial class DataPostgres
    {
        /// <summary>
        /// 레시피 목록 조회
        /// </summary>
        public async Task<ArrayList> GetRecipeListAsync()
        {
            ArrayList list = new ArrayList();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"SELECT recipe_id, recipe_name, description, created_at, updated_at,
                        recipe_guid, recipe_code, version, is_active,
                        status, approved_by, approved_at, recipe_mode
                        FROM system.recipe
                        WHERE is_active = true OR is_active IS NULL
                        ORDER BY recipe_name, version DESC";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var info = new RecipeInfo();
                            info.recipe_id = reader.GetInt32(0);
                            info.recipe_name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            info.description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            info.created_at = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3);
                            info.updated_at = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4);
                            info.recipe_guid = reader.IsDBNull(5) ? "" : reader.GetString(5);
                            info.recipe_code = reader.IsDBNull(6) ? "" : reader.GetString(6);
                            info.version = reader.IsDBNull(7) ? 1 : reader.GetInt32(7);
                            info.is_active = reader.IsDBNull(8) ? true : reader.GetBoolean(8);
                            info.status = reader.IsDBNull(9) ? "draft" : reader.GetString(9);
                            info.approved_by = reader.IsDBNull(10) ? "" : reader.GetString(10);
                            info.approved_at = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11);
                            info.recipe_mode = reader.IsDBNull(12) ? "standard" : reader.GetString(12);
                            list.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"레시피 목록 조회 오류: {ex.Message}");
            }

            return list;
        }

        /// <summary>
        /// 레시피 상세 조회 (Steps + Items 포함)
        /// </summary>
        public async Task<RecipeData> GetRecipeAsync(int recipeId)
        {
            RecipeData recipe = null;

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // 1. 레시피 마스터
                    string recipeQuery = @"SELECT recipe_id, recipe_name, description, created_at, updated_at,
                        recipe_guid, recipe_code, version, is_active,
                        status, approved_by, approved_at, recipe_mode
                        FROM system.recipe WHERE recipe_id = @id";
                    using (var cmd = new NpgsqlCommand(recipeQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", recipeId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                recipe = new RecipeData();
                                recipe.recipe_id = reader.GetInt32(0);
                                recipe.recipe_name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                recipe.description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                recipe.created_at = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3);
                                recipe.updated_at = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4);
                                recipe.recipe_guid = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                recipe.recipe_code = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                recipe.version = reader.IsDBNull(7) ? 1 : reader.GetInt32(7);
                                recipe.is_active = reader.IsDBNull(8) ? true : reader.GetBoolean(8);
                                recipe.status = reader.IsDBNull(9) ? "draft" : reader.GetString(9);
                                recipe.approved_by = reader.IsDBNull(10) ? "" : reader.GetString(10);
                                recipe.approved_at = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11);
                                recipe.recipe_mode = reader.IsDBNull(12) ? "standard" : reader.GetString(12);
                            }
                        }
                    }

                    if (recipe == null) return null;

                    // 2. Unit 계층 조회
                    Hashtable unitMap = new Hashtable(); // unit_id → RecipeUnitData
                    string unitQuery = @"SELECT unit_id, unit_name, unit_order, description
                        FROM system.recipe_unit WHERE recipe_id = @id ORDER BY unit_order";
                    using (var cmd = new NpgsqlCommand(unitQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", recipeId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var unit = new RecipeUnitData();
                                unit.unit_id = reader.GetInt32(0);
                                unit.recipe_id = recipeId;
                                unit.unit_name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                unit.unit_order = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                                unit.description = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                recipe.units.Add(unit);
                                unitMap[unit.unit_id] = unit;
                            }
                        }
                    }

                    // 3. 레시피 단계 (unit_id + entry_condition + expression + 확장 모델 포함)
                    string stepQuery = @"SELECT step_id, step_order, step_name, wait_time_ms, timeout_ms,
                        condition_tag, condition_value, condition_type, unit_id,
                        entry_condition_tag, entry_condition_value, entry_condition_type, entry_timeout_ms,
                        entry_expression, exit_expression,
                        running_expression, exit_actions_json, abort_actions_json,
                        transitions_json
                        FROM system.recipe_step WHERE recipe_id = @id ORDER BY step_order";
                    using (var cmd = new NpgsqlCommand(stepQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", recipeId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var step = new RecipeStepData();
                                step.step_id = reader.GetInt32(0);
                                step.step_order = reader.GetInt32(1);
                                step.step_name = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                step.wait_time_ms = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                step.timeout_ms = reader.IsDBNull(4) ? 30000 : reader.GetInt32(4);
                                step.condition_tag = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                step.condition_value = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                step.condition_type = reader.IsDBNull(7) ? "none" : reader.GetString(7);
                                step.unit_id = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                                step.entry_condition_tag = reader.IsDBNull(9) ? "" : reader.GetString(9);
                                step.entry_condition_value = reader.IsDBNull(10) ? "" : reader.GetString(10);
                                step.entry_condition_type = reader.IsDBNull(11) ? "none" : reader.GetString(11);
                                step.entry_timeout_ms = reader.IsDBNull(12) ? 0 : reader.GetInt32(12);
                                step.entry_expression = reader.IsDBNull(13) ? "" : reader.GetString(13);
                                step.exit_expression = reader.IsDBNull(14) ? "" : reader.GetString(14);
                                step.running_expression = reader.IsDBNull(15) ? "" : reader.GetString(15);
                                step.exit_actions_json = reader.IsDBNull(16) ? "" : reader.GetString(16);
                                step.abort_actions_json = reader.IsDBNull(17) ? "" : reader.GetString(17);
                                step.transitions_json = reader.IsDBNull(18) ? "" : reader.GetString(18);

                                // unit_id가 있으면 해당 Unit의 steps에 배치, 없으면 recipe 직속
                                if (step.unit_id > 0 && unitMap.ContainsKey(step.unit_id))
                                    ((RecipeUnitData)unitMap[step.unit_id]).steps.Add(step);
                                else
                                    recipe.steps.Add(step);
                            }
                        }
                    }

                    // 4. 모든 step의 items 조회 (recipe 직속 + unit 소속)
                    ArrayList allSteps = new ArrayList();
                    for (int i = 0; i < recipe.steps.Count; i++)
                        allSteps.Add(recipe.steps[i]);
                    for (int u = 0; u < recipe.units.Count; u++)
                    {
                        var unit = (RecipeUnitData)recipe.units[u];
                        for (int s = 0; s < unit.steps.Count; s++)
                            allSteps.Add(unit.steps[s]);
                    }

                    for (int i = 0; i < allSteps.Count; i++)
                    {
                        var step = (RecipeStepData)allSteps[i];

                        string itemQuery = @"SELECT item_id, tag_name, set_value, value_type, item_order
                            FROM system.recipe_step_item WHERE step_id = @stepId ORDER BY item_order";
                        using (var cmd = new NpgsqlCommand(itemQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@stepId", step.step_id);
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var item = new RecipeItemData();
                                    item.item_id = reader.GetInt32(0);
                                    item.tag_name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                    item.set_value = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                    item.value_type = reader.IsDBNull(3) ? "double" : reader.GetString(3);
                                    item.item_order = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                    step.items.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"레시피 상세 조회 오류: {ex.Message}");
            }

            return recipe;
        }

        /// <summary>
        /// 레시피 이름으로 조회
        /// </summary>
        public async Task<RecipeData> GetRecipeByNameAsync(string recipeName)
        {
            int recipeId = -1;

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT recipe_id FROM system.recipe WHERE recipe_name = @name AND (is_active = true OR is_active IS NULL)";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", recipeName);
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            recipeId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"레시피 이름 조회 오류: {ex.Message}");
            }

            if (recipeId < 0) return null;
            return await GetRecipeAsync(recipeId);
        }

        /// <summary>
        /// 레시피 저장 (신규 INSERT / 기존 UPDATE 통합, 트랜잭션)
        /// </summary>
        public async Task<(int recipeId, string error)> SaveRecipeAsync(RecipeData recipe)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            int recipeId;

                            // GUID 자동 생성 (하위호환)
                            if (string.IsNullOrEmpty(recipe.recipe_guid))
                                recipe.recipe_guid = Guid.NewGuid().ToString("N");
                            if (recipe.version <= 0)
                                recipe.version = 1;

                            if (recipe.recipe_id > 0)
                            {
                                // 기존 레시피 UPDATE
                                string updateQuery = @"UPDATE system.recipe SET recipe_name = @name, description = @desc,
                                    recipe_guid = @guid, recipe_code = @code, version = @ver,
                                    status = @status, approved_by = @approvedBy, approved_at = @approvedAt,
                                    recipe_mode = @mode
                                    WHERE recipe_id = @id";
                                using (var cmd = new NpgsqlCommand(updateQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@id", recipe.recipe_id);
                                    cmd.Parameters.AddWithValue("@name", recipe.recipe_name);
                                    cmd.Parameters.AddWithValue("@desc", (object)recipe.description ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@guid", (object)recipe.recipe_guid ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@code", (object)recipe.recipe_code ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@ver", recipe.version);
                                    cmd.Parameters.AddWithValue("@status", (object)recipe.status ?? "draft");
                                    cmd.Parameters.AddWithValue("@approvedBy", (object)recipe.approved_by ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@approvedAt", recipe.approved_at.HasValue ? (object)recipe.approved_at.Value : DBNull.Value);
                                    cmd.Parameters.AddWithValue("@mode", (object)recipe.recipe_mode ?? "standard");
                                    await cmd.ExecuteNonQueryAsync();
                                }
                                recipeId = recipe.recipe_id;

                                // 기존 units 삭제 (CASCADE로 unit 소속 steps/items 함께 삭제)
                                string deleteUnits = "DELETE FROM system.recipe_unit WHERE recipe_id = @id";
                                using (var cmd = new NpgsqlCommand(deleteUnits, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@id", recipeId);
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                // 기존 recipe 직속 steps/items 삭제 (unit_id IS NULL인 것만)
                                string deleteSteps = "DELETE FROM system.recipe_step WHERE recipe_id = @id AND unit_id IS NULL";
                                using (var cmd = new NpgsqlCommand(deleteSteps, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@id", recipeId);
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                            else
                            {
                                // 신규 레시피 INSERT
                                string insertQuery = @"INSERT INTO system.recipe
                                    (recipe_name, description, recipe_guid, recipe_code, version,
                                     status, approved_by, approved_at, recipe_mode)
                                    VALUES (@name, @desc, @guid, @code, @ver,
                                            @status, @approvedBy, @approvedAt, @mode) RETURNING recipe_id";
                                using (var cmd = new NpgsqlCommand(insertQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@name", recipe.recipe_name);
                                    cmd.Parameters.AddWithValue("@desc", (object)recipe.description ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@guid", (object)recipe.recipe_guid ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@code", (object)recipe.recipe_code ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@ver", recipe.version);
                                    cmd.Parameters.AddWithValue("@status", (object)recipe.status ?? "draft");
                                    cmd.Parameters.AddWithValue("@approvedBy", (object)recipe.approved_by ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@approvedAt", recipe.approved_at.HasValue ? (object)recipe.approved_at.Value : DBNull.Value);
                                    cmd.Parameters.AddWithValue("@mode", (object)recipe.recipe_mode ?? "standard");
                                    recipeId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                                }
                            }

                            // Units INSERT
                            for (int u = 0; u < recipe.units.Count; u++)
                            {
                                var unit = (RecipeUnitData)recipe.units[u];

                                string unitInsert = @"INSERT INTO system.recipe_unit
                                    (recipe_id, unit_name, unit_order, description)
                                    VALUES (@recipeId, @name, @order, @desc)
                                    RETURNING unit_id";

                                int unitId;
                                using (var cmd = new NpgsqlCommand(unitInsert, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@recipeId", recipeId);
                                    cmd.Parameters.AddWithValue("@name", unit.unit_name);
                                    cmd.Parameters.AddWithValue("@order", unit.unit_order);
                                    cmd.Parameters.AddWithValue("@desc", (object)unit.description ?? DBNull.Value);
                                    unitId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                                }

                                // Unit 소속 Steps INSERT
                                await InsertStepsAsync(conn, transaction, recipeId, unitId, unit.steps);
                            }

                            // Recipe 직속 Steps INSERT (unit_id = NULL)
                            await InsertStepsAsync(conn, transaction, recipeId, 0, recipe.steps);

                            transaction.Commit();
                            return (recipeId, null);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Debug.WriteLine($"레시피 저장 트랜잭션 실패: {ex.Message}");
                            return (-1, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"레시피 저장 오류: {ex.Message}");
                return (-1, ex.Message);
            }
        }

        /// <summary>
        /// 레시피 삭제 (FK CASCADE로 연쇄 삭제)
        /// </summary>
        public async Task<string> DeleteRecipeAsync(int recipeId)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM system.recipe WHERE recipe_id = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", recipeId);
                        int affected = await cmd.ExecuteNonQueryAsync();
                        if (affected == 0)
                            return "레시피를 찾을 수 없습니다.";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"레시피 삭제 오류: {ex.Message}");
                return ex.Message;
            }

            return null; // 성공
        }

        /// <summary>
        /// Steps + Items INSERT 헬퍼 (Unit 소속 또는 Recipe 직속)
        /// </summary>
        private async Task InsertStepsAsync(NpgsqlConnection conn, NpgsqlTransaction transaction,
            int recipeId, int unitId, ArrayList steps)
        {
            for (int s = 0; s < steps.Count; s++)
            {
                var step = (RecipeStepData)steps[s];

                string stepInsert = @"INSERT INTO system.recipe_step
                    (recipe_id, unit_id, step_order, step_name, wait_time_ms, timeout_ms,
                     condition_tag, condition_value, condition_type,
                     entry_condition_tag, entry_condition_value, entry_condition_type, entry_timeout_ms,
                     entry_expression, exit_expression,
                     running_expression, exit_actions_json, abort_actions_json,
                     transitions_json)
                    VALUES (@recipeId, @unitId, @order, @name, @waitMs, @timeoutMs,
                            @condTag, @condVal, @condType,
                            @entryTag, @entryVal, @entryType, @entryTimeoutMs,
                            @entryExpr, @exitExpr,
                            @runExpr, @exitActions, @abortActions,
                            @transJson)
                    RETURNING step_id";

                int stepId;
                using (var cmd = new NpgsqlCommand(stepInsert, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@recipeId", recipeId);
                    cmd.Parameters.AddWithValue("@unitId", unitId > 0 ? (object)unitId : DBNull.Value);
                    cmd.Parameters.AddWithValue("@order", step.step_order);
                    cmd.Parameters.AddWithValue("@name", (object)step.step_name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@waitMs", step.wait_time_ms);
                    cmd.Parameters.AddWithValue("@timeoutMs", step.timeout_ms);
                    cmd.Parameters.AddWithValue("@condTag", (object)step.condition_tag ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@condVal", (object)step.condition_value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@condType", (object)step.condition_type ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@entryTag", (object)step.entry_condition_tag ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@entryVal", (object)step.entry_condition_value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@entryType", (object)step.entry_condition_type ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@entryTimeoutMs", step.entry_timeout_ms);
                    cmd.Parameters.AddWithValue("@entryExpr", (object)step.entry_expression ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@exitExpr", (object)step.exit_expression ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@runExpr", (object)step.running_expression ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@exitActions", (object)step.exit_actions_json ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@abortActions", (object)step.abort_actions_json ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@transJson", (object)step.transitions_json ?? DBNull.Value);
                    stepId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                // Items INSERT
                for (int i = 0; i < step.items.Count; i++)
                {
                    var item = (RecipeItemData)step.items[i];

                    string itemInsert = @"INSERT INTO system.recipe_step_item
                        (step_id, tag_name, set_value, value_type, item_order)
                        VALUES (@stepId, @tag, @val, @valType, @order)";

                    using (var cmd = new NpgsqlCommand(itemInsert, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@stepId", stepId);
                        cmd.Parameters.AddWithValue("@tag", item.tag_name);
                        cmd.Parameters.AddWithValue("@val", item.set_value);
                        cmd.Parameters.AddWithValue("@valType", (object)item.value_type ?? "double");
                        cmd.Parameters.AddWithValue("@order", item.item_order);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        /// <summary>
        /// GUID로 활성 레시피 정보 조회 (Import 비교용)
        /// </summary>
        public async Task<RecipeInfo> GetRecipeInfoByGuidAsync(string recipeGuid)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"SELECT recipe_id, recipe_name, description, created_at, updated_at,
                        recipe_guid, recipe_code, version, is_active,
                        status, approved_by, approved_at, recipe_mode
                        FROM system.recipe
                        WHERE recipe_guid = @guid AND (is_active = true OR is_active IS NULL)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@guid", recipeGuid);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var info = new RecipeInfo();
                                info.recipe_id = reader.GetInt32(0);
                                info.recipe_name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                info.description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                info.created_at = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3);
                                info.updated_at = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4);
                                info.recipe_guid = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                info.recipe_code = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                info.version = reader.IsDBNull(7) ? 1 : reader.GetInt32(7);
                                info.is_active = reader.IsDBNull(8) ? true : reader.GetBoolean(8);
                                info.status = reader.IsDBNull(9) ? "draft" : reader.GetString(9);
                                info.approved_by = reader.IsDBNull(10) ? "" : reader.GetString(10);
                                info.approved_at = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11);
                                info.recipe_mode = reader.IsDBNull(12) ? "standard" : reader.GetString(12);
                                return info;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GUID 기반 레시피 조회 오류: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Import 배치 처리: 단일 트랜잭션으로 여러 레시피를 일괄 Import
        /// - softArchiveExisting=true이면 기존 활성 레시피를 is_active=false로 비활성화
        /// - 새 레시피를 INSERT (Units/Steps/Items 포함)
        /// - 감사 로그 기록
        /// </summary>
        public async Task<(int imported, string error)> ImportRecipeBatchAsync(
            List<(RecipeData recipe, bool softArchiveExisting)> importItems,
            string username, string machineName)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            int imported = 0;

                            for (int i = 0; i < importItems.Count; i++)
                            {
                                var recipe = importItems[i].recipe;
                                bool softArchive = importItems[i].softArchiveExisting;

                                // 1. 기존 활성 레시피 비활성화 (소프트 삭제)
                                if (softArchive && !string.IsNullOrEmpty(recipe.recipe_guid))
                                {
                                    string archiveSql = @"UPDATE system.recipe SET is_active = false
                                        WHERE recipe_guid = @guid AND (is_active = true OR is_active IS NULL)";
                                    using (var cmd = new NpgsqlCommand(archiveSql, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@guid", recipe.recipe_guid);
                                        await cmd.ExecuteNonQueryAsync();
                                    }
                                }

                                // 2. 새 레시피 INSERT
                                string insertSql = @"INSERT INTO system.recipe
                                    (recipe_name, description, recipe_guid, recipe_code, version, is_active,
                                     status, approved_by, approved_at, recipe_mode)
                                    VALUES (@name, @desc, @guid, @code, @ver, true,
                                            @status, @approvedBy, @approvedAt, @mode)
                                    RETURNING recipe_id";

                                int newRecipeId;
                                using (var cmd = new NpgsqlCommand(insertSql, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@name", recipe.recipe_name);
                                    cmd.Parameters.AddWithValue("@desc", (object)recipe.description ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@guid", (object)recipe.recipe_guid ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@code", (object)recipe.recipe_code ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@ver", recipe.version);
                                    cmd.Parameters.AddWithValue("@status", (object)recipe.status ?? "draft");
                                    cmd.Parameters.AddWithValue("@approvedBy", (object)recipe.approved_by ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@approvedAt", recipe.approved_at.HasValue ? (object)recipe.approved_at.Value : DBNull.Value);
                                    cmd.Parameters.AddWithValue("@mode", (object)recipe.recipe_mode ?? "standard");
                                    newRecipeId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                                }

                                // 3. Units INSERT
                                for (int u = 0; u < recipe.units.Count; u++)
                                {
                                    var unit = (RecipeUnitData)recipe.units[u];

                                    string unitInsert = @"INSERT INTO system.recipe_unit
                                        (recipe_id, unit_name, unit_order, description)
                                        VALUES (@recipeId, @name, @order, @desc)
                                        RETURNING unit_id";

                                    int unitId;
                                    using (var cmd = new NpgsqlCommand(unitInsert, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@recipeId", newRecipeId);
                                        cmd.Parameters.AddWithValue("@name", unit.unit_name);
                                        cmd.Parameters.AddWithValue("@order", unit.unit_order);
                                        cmd.Parameters.AddWithValue("@desc", (object)unit.description ?? DBNull.Value);
                                        unitId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                                    }

                                    // Unit 소속 Steps INSERT
                                    await InsertStepsAsync(conn, transaction, newRecipeId, unitId, unit.steps);
                                }

                                // 4. Recipe 직속 Steps INSERT (unit_id = NULL)
                                await InsertStepsAsync(conn, transaction, newRecipeId, 0, recipe.steps);

                                // 5. 감사 로그
                                string auditSql = @"INSERT INTO history.recipe_audit_log
                                    (recipe_id, recipe_name, action, target_type, target_name,
                                     old_value, new_value, username, machine_name)
                                    VALUES (@rid, @rname, 'IMPORT', 'RECIPE', @tname,
                                            @oldVal, @newVal, @user, @machine)";
                                using (var cmd = new NpgsqlCommand(auditSql, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@rid", newRecipeId);
                                    cmd.Parameters.AddWithValue("@rname", recipe.recipe_name);
                                    cmd.Parameters.AddWithValue("@tname", recipe.recipe_name);
                                    cmd.Parameters.AddWithValue("@oldVal",
                                        softArchive ? (object)$"{{\"action\":\"archive\",\"guid\":\"{recipe.recipe_guid}\"}}" : DBNull.Value);
                                    cmd.Parameters.AddWithValue("@newVal",
                                        (object)$"{{\"guid\":\"{recipe.recipe_guid}\",\"version\":{recipe.version}}}");
                                    cmd.Parameters.AddWithValue("@user", (object)username ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@machine", (object)machineName ?? DBNull.Value);
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                imported++;
                            }

                            transaction.Commit();
                            return (imported, null);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Debug.WriteLine($"Import 트랜잭션 실패: {ex.Message}");
                            return (0, ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Import 배치 오류: {ex.Message}");
                return (0, ex.Message);
            }
        }

        /// <summary>
        /// 레시피 실행 로그 INSERT (INSERT 전용 테이블)
        /// </summary>
        public async Task<long> InsertRecipeExecutionLogAsync(
            int recipeId, string recipeName, string unitName,
            string action, string status, int stepIndex, int totalSteps,
            string errorMessage, string username, string machineName,
            DateTime? executionStart, DateTime? executionEnd)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"INSERT INTO history.recipe_execution_log
                        (recipe_id, recipe_name, unit_name, action, status, step_index, total_steps,
                         error_message, username, machine_name, execution_start, execution_end)
                        VALUES (@rid, @rname, @uname, @action, @status, @sidx, @total,
                                @err, @user, @machine, @start, @end)
                        RETURNING log_id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@rid", recipeId > 0 ? (object)recipeId : DBNull.Value);
                        cmd.Parameters.AddWithValue("@rname", (object)recipeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@uname", (object)unitName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@sidx", stepIndex);
                        cmd.Parameters.AddWithValue("@total", totalSteps);
                        cmd.Parameters.AddWithValue("@err", (object)errorMessage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@user", (object)username ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@machine", (object)machineName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@start", executionStart.HasValue ? (object)executionStart.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@end", executionEnd.HasValue ? (object)executionEnd.Value : DBNull.Value);

                        var result = await cmd.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt64(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"실행 로그 INSERT 오류: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 실행 로그 배치 INSERT — 여러 로그를 한 번의 INSERT로 기록
        /// </summary>
        public async Task InsertRecipeExecutionLogBatchAsync(
            List<(int recipeId, string recipeName, string unitName, string action, string status,
                int stepIndex, int totalSteps, string errorMessage,
                string username, string machineName,
                DateTime? executionStart, DateTime? executionEnd)> entries)
        {
            if (entries == null || entries.Count == 0) return;

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var sb = new System.Text.StringBuilder();
                    sb.Append(@"INSERT INTO history.recipe_execution_log
                        (recipe_id, recipe_name, unit_name, action, status, step_index, total_steps,
                         error_message, username, machine_name, execution_start, execution_end)
                        VALUES ");

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;

                        for (int i = 0; i < entries.Count; i++)
                        {
                            var e = entries[i];
                            if (i > 0) sb.Append(", ");
                            sb.AppendFormat("(@rid{0}, @rname{0}, @uname{0}, @action{0}, @status{0}, @sidx{0}, @total{0}, @err{0}, @user{0}, @machine{0}, @start{0}, @end{0})", i);

                            cmd.Parameters.AddWithValue($"@rid{i}", e.recipeId > 0 ? (object)e.recipeId : DBNull.Value);
                            cmd.Parameters.AddWithValue($"@rname{i}", (object)e.recipeName ?? DBNull.Value);
                            cmd.Parameters.AddWithValue($"@uname{i}", (object)e.unitName ?? DBNull.Value);
                            cmd.Parameters.AddWithValue($"@action{i}", e.action);
                            cmd.Parameters.AddWithValue($"@status{i}", e.status);
                            cmd.Parameters.AddWithValue($"@sidx{i}", e.stepIndex);
                            cmd.Parameters.AddWithValue($"@total{i}", e.totalSteps);
                            cmd.Parameters.AddWithValue($"@err{i}", (object)e.errorMessage ?? DBNull.Value);
                            cmd.Parameters.AddWithValue($"@user{i}", (object)e.username ?? DBNull.Value);
                            cmd.Parameters.AddWithValue($"@machine{i}", (object)e.machineName ?? DBNull.Value);
                            cmd.Parameters.AddWithValue($"@start{i}", e.executionStart.HasValue ? (object)e.executionStart.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue($"@end{i}", e.executionEnd.HasValue ? (object)e.executionEnd.Value : DBNull.Value);
                        }

                        cmd.CommandText = sb.ToString();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"실행 로그 배치 INSERT 오류: {ex.Message}");
            }
        }

        #region ISA-88 Phase 3: Transition Execution Log

        /// <summary>
        /// 전이 실행 감사 로그 INSERT (INSERT 전용 — 변경/삭제 불가)
        /// </summary>
        public async Task<long> InsertTransitionLogAsync(
            string batchId, int stepOrder, string stepName,
            int transitionIndex, string transitionType, string expression,
            bool evaluatedResult, string actionTaken, string unitName,
            string errorMessage, string username, string machineName)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    string sql = @"INSERT INTO history.transition_execution_log
                        (batch_id, step_order, step_name, transition_index, transition_type,
                         expression, evaluated_result, action_taken, unit_name,
                         error_message, username, machine_name)
                        VALUES (@bid, @sorder, @sname, @tidx, @ttype,
                                @expr, @result, @action, @uname,
                                @err, @user, @machine)
                        RETURNING log_id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@bid", (object)batchId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@sorder", stepOrder);
                        cmd.Parameters.AddWithValue("@sname", (object)stepName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@tidx", transitionIndex);
                        cmd.Parameters.AddWithValue("@ttype", transitionType);
                        cmd.Parameters.AddWithValue("@expr", (object)expression ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@result", evaluatedResult);
                        cmd.Parameters.AddWithValue("@action", (object)actionTaken ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@uname", (object)unitName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@err", (object)errorMessage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@user", (object)username ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@machine", (object)machineName ?? DBNull.Value);

                        var result = await cmd.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt64(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"전이 로그 INSERT 오류: {ex.Message}");
                return -1;
            }
        }

        #endregion

        #region ISA-88 Phase 2: Control Recipe + Batch Execution CRUD

        /// <summary>
        /// Control Recipe 생성 — Master 레시피를 JSON 스냅샷으로 동결하여 저장
        /// </summary>
        public async Task<int> CreateControlRecipeAsync(int masterRecipeId, int masterVersion,
            string batchId, string snapshotJson, string username)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"INSERT INTO operational.control_recipe
                        (master_recipe_id, master_version, batch_id, recipe_snapshot, status, created_by)
                        VALUES (@masterId, @ver, @batchId, @snapshot::jsonb, 'pending', @user)
                        RETURNING control_recipe_id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@masterId", masterRecipeId);
                        cmd.Parameters.AddWithValue("@ver", masterVersion);
                        cmd.Parameters.AddWithValue("@batchId", batchId);
                        cmd.Parameters.AddWithValue("@snapshot", snapshotJson);
                        cmd.Parameters.AddWithValue("@user", (object)username ?? DBNull.Value);

                        var result = await cmd.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Control Recipe 생성 오류: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Control Recipe 조회 — 스냅샷 JSON 포함
        /// </summary>
        public async Task<(ControlRecipeInfo info, string snapshotJson)> GetControlRecipeAsync(int controlRecipeId)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"SELECT control_recipe_id, master_recipe_id, master_version,
                        batch_id, recipe_snapshot::text, status, created_at, created_by
                        FROM operational.control_recipe WHERE control_recipe_id = @id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", controlRecipeId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var info = new ControlRecipeInfo();
                                info.control_recipe_id = reader.GetInt32(0);
                                info.master_recipe_id = reader.GetInt32(1);
                                info.master_version = reader.GetInt32(2);
                                info.batch_id = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                string snapshot = reader.IsDBNull(4) ? "{}" : reader.GetString(4);
                                info.status = reader.IsDBNull(5) ? "pending" : reader.GetString(5);
                                info.created_at = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6);
                                info.created_by = reader.IsDBNull(7) ? "" : reader.GetString(7);
                                return (info, snapshot);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Control Recipe 조회 오류: {ex.Message}");
            }
            return (null, null);
        }

        /// <summary>
        /// Control Recipe 상태 업데이트 (pending → running → completed/aborted)
        /// </summary>
        public async Task<bool> UpdateControlRecipeStatusAsync(int controlRecipeId, string newStatus)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"UPDATE operational.control_recipe SET status = @status
                        WHERE control_recipe_id = @id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", newStatus);
                        cmd.Parameters.AddWithValue("@id", controlRecipeId);
                        int rows = await cmd.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Control Recipe 상태 업데이트 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Batch Execution 레코드 생성 — 배치 시작 시 호출
        /// </summary>
        public async Task<bool> CreateBatchExecutionAsync(BatchExecutionRecord record)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"INSERT INTO history.batch_execution
                        (batch_id, control_recipe_id, master_recipe_id, master_recipe_name,
                         master_version, operator_id, start_time, status)
                        VALUES (@batchId, @ctrlId, @masterId, @masterName,
                                @ver, @operator, @startTime, @status)";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@batchId", record.batch_id);
                        cmd.Parameters.AddWithValue("@ctrlId", record.control_recipe_id);
                        cmd.Parameters.AddWithValue("@masterId", record.master_recipe_id);
                        cmd.Parameters.AddWithValue("@masterName", (object)record.master_recipe_name ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ver", record.master_version);
                        cmd.Parameters.AddWithValue("@operator", (object)record.operator_id ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@startTime", record.start_time.HasValue ? (object)record.start_time.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@status", (object)record.status ?? "idle");
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Batch Execution 생성 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Batch Execution 상태/결과 업데이트 (실행 중 → 완료/실패/중단)
        /// </summary>
        public async Task<bool> UpdateBatchStatusAsync(string batchId, string status, string result, DateTime? endTime)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"UPDATE history.batch_execution
                        SET status = @status, result = @result, end_time = @endTime
                        WHERE batch_id = @batchId";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", (object)status ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@result", (object)result ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@endTime", endTime.HasValue ? (object)endTime.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@batchId", batchId);
                        int rows = await cmd.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Batch 상태 업데이트 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Batch Execution 목록 조회 (특정 Master Recipe 기준 또는 전체)
        /// </summary>
        public async Task<List<BatchExecutionRecord>> GetBatchExecutionListAsync(int? masterRecipeId = null, int limit = 100)
        {
            var list = new List<BatchExecutionRecord>();
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"SELECT batch_id, control_recipe_id, master_recipe_id,
                        master_recipe_name, master_version, operator_id,
                        start_time, end_time, result, status, created_at
                        FROM history.batch_execution";
                    if (masterRecipeId.HasValue)
                        sql += " WHERE master_recipe_id = @masterId";
                    sql += " ORDER BY created_at DESC LIMIT @limit";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        if (masterRecipeId.HasValue)
                            cmd.Parameters.AddWithValue("@masterId", masterRecipeId.Value);
                        cmd.Parameters.AddWithValue("@limit", limit);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var rec = new BatchExecutionRecord();
                                rec.batch_id = reader.IsDBNull(0) ? "" : reader.GetString(0);
                                rec.control_recipe_id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                rec.master_recipe_id = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                                rec.master_recipe_name = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                rec.master_version = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                rec.operator_id = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                rec.start_time = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
                                rec.end_time = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
                                rec.result = reader.IsDBNull(8) ? "" : reader.GetString(8);
                                rec.status = reader.IsDBNull(9) ? "idle" : reader.GetString(9);
                                rec.created_at = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10);
                                list.Add(rec);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Batch Execution 목록 조회 오류: {ex.Message}");
            }
            return list;
        }

        /// <summary>
        /// 배치 실행 이력 조회 — 날짜 범위 필터
        /// </summary>
        public async Task<List<BatchExecutionRecord>> GetBatchExecutionListAsync(
            int? masterRecipeId, DateTime dateFrom, DateTime dateTo, int limit = 500)
        {
            var list = new List<BatchExecutionRecord>();
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"SELECT batch_id, control_recipe_id, master_recipe_id,
                        master_recipe_name, master_version, operator_id,
                        start_time, end_time, result, status, created_at
                        FROM history.batch_execution
                        WHERE start_time >= @from AND start_time < @to";
                    if (masterRecipeId.HasValue)
                        sql += " AND master_recipe_id = @masterId";
                    sql += " ORDER BY created_at DESC LIMIT @limit";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@from", dateFrom);
                        cmd.Parameters.AddWithValue("@to", dateTo);
                        if (masterRecipeId.HasValue)
                            cmd.Parameters.AddWithValue("@masterId", masterRecipeId.Value);
                        cmd.Parameters.AddWithValue("@limit", limit);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var rec = new BatchExecutionRecord();
                                rec.batch_id = reader.IsDBNull(0) ? "" : reader.GetString(0);
                                rec.control_recipe_id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                rec.master_recipe_id = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                                rec.master_recipe_name = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                rec.master_version = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                rec.operator_id = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                rec.start_time = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
                                rec.end_time = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
                                rec.result = reader.IsDBNull(8) ? "" : reader.GetString(8);
                                rec.status = reader.IsDBNull(9) ? "idle" : reader.GetString(9);
                                rec.created_at = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10);
                                list.Add(rec);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Batch Execution(날짜) 조회 오류: {ex.Message}");
            }
            return list;
        }

        /// <summary>
        /// 최근 실행 로그 조회 (recipe_execution_log)
        /// </summary>
        public async Task<ArrayList> GetRecentExecutionLogsAsync(string recipeName, int limit = 50)
        {
            var list = new ArrayList();
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"SELECT log_id, recipe_id, recipe_name, unit_name, action, status,
                        step_index, total_steps, error_message, username,
                        execution_start, execution_end, created_at
                        FROM history.recipe_execution_log
                        WHERE recipe_name = @name
                        ORDER BY created_at DESC LIMIT @limit";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", recipeName);
                        cmd.Parameters.AddWithValue("@limit", limit);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var entry = new RecipeExecutionLogEntry();
                                entry.log_id = reader.GetInt64(0);
                                entry.recipe_id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                entry.recipe_name = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                entry.unit_name = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                entry.action = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                entry.status = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                entry.step_index = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                                entry.total_steps = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                                entry.error_message = reader.IsDBNull(8) ? "" : reader.GetString(8);
                                entry.username = reader.IsDBNull(9) ? "" : reader.GetString(9);
                                entry.execution_start = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10);
                                entry.execution_end = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11);
                                entry.created_at = reader.IsDBNull(12) ? DateTime.MinValue : reader.GetDateTime(12);
                                list.Add(entry);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"실행 로그 조회 오류: {ex.Message}");
            }
            return list;
        }

        /// <summary>
        /// 실행 로그 조회 — 날짜 범위 필터
        /// </summary>
        public async Task<ArrayList> GetRecentExecutionLogsAsync(
            string recipeName, DateTime dateFrom, DateTime dateTo, int limit = 500)
        {
            var list = new ArrayList();
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"SELECT log_id, recipe_id, recipe_name, unit_name, action, status,
                        step_index, total_steps, error_message, username,
                        execution_start, execution_end, created_at
                        FROM history.recipe_execution_log
                        WHERE recipe_name = @name AND created_at >= @from AND created_at < @to
                        ORDER BY created_at DESC LIMIT @limit";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", recipeName);
                        cmd.Parameters.AddWithValue("@from", dateFrom);
                        cmd.Parameters.AddWithValue("@to", dateTo);
                        cmd.Parameters.AddWithValue("@limit", limit);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var entry = new RecipeExecutionLogEntry();
                                entry.log_id = reader.GetInt64(0);
                                entry.recipe_id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                entry.recipe_name = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                entry.unit_name = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                entry.action = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                entry.status = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                entry.step_index = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                                entry.total_steps = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                                entry.error_message = reader.IsDBNull(8) ? "" : reader.GetString(8);
                                entry.username = reader.IsDBNull(9) ? "" : reader.GetString(9);
                                entry.execution_start = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10);
                                entry.execution_end = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11);
                                entry.created_at = reader.IsDBNull(12) ? DateTime.MinValue : reader.GetDateTime(12);
                                list.Add(entry);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"실행 로그(날짜) 조회 오류: {ex.Message}");
            }
            return list;
        }

        #endregion

        /// <summary>
        /// 레시피 감사 로그 INSERT (INSERT 전용 테이블)
        /// </summary>
        public async Task<long> InsertRecipeAuditLogAsync(
            int recipeId, string recipeName, string action,
            string targetType, string targetName,
            string oldValue, string newValue,
            string username, string machineName)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"INSERT INTO history.recipe_audit_log
                        (recipe_id, recipe_name, action, target_type, target_name,
                         old_value, new_value, username, machine_name)
                        VALUES (@rid, @rname, @action, @ttype, @tname,
                                @oldval, @newval, @user, @machine)
                        RETURNING log_id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@rid", recipeId > 0 ? (object)recipeId : DBNull.Value);
                        cmd.Parameters.AddWithValue("@rname", (object)recipeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@ttype", (object)targetType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@tname", (object)targetName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@oldval", (object)oldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@newval", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@user", (object)username ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@machine", (object)machineName ?? DBNull.Value);

                        var result = await cmd.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt64(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"감사 로그 INSERT 오류: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 레시피 감사 로그 INSERT (전자서명 포함)
        /// </summary>
        public async Task<long> InsertRecipeAuditLogAsync(
            int recipeId, string recipeName, string action,
            string targetType, string targetName,
            string oldValue, string newValue,
            string username, string machineName,
            string signature, string reason)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"INSERT INTO history.recipe_audit_log
                        (recipe_id, recipe_name, action, target_type, target_name,
                         old_value, new_value, username, machine_name, signature, reason)
                        VALUES (@rid, @rname, @action, @ttype, @tname,
                                @oldval, @newval, @user, @machine, @sig, @reason)
                        RETURNING log_id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@rid", recipeId > 0 ? (object)recipeId : DBNull.Value);
                        cmd.Parameters.AddWithValue("@rname", (object)recipeName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@ttype", (object)targetType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@tname", (object)targetName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@oldval", (object)oldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@newval", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@user", (object)username ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@machine", (object)machineName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@sig", (object)signature ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@reason", (object)reason ?? DBNull.Value);

                        var result = await cmd.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt64(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"감사 로그(서명) INSERT 오류: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// operational.audit_log 에 감사 로그 삽입 (전자서명 이력 포함)
        /// </summary>
        /// <param name="userName">사용자명</param>
        /// <param name="action">액션명 (BATCH_START, APPROVE, OBSOLETE 등)</param>
        /// <param name="objectType">대상 객체 유형 (RECIPE, BATCH 등)</param>
        /// <param name="objectName">대상 객체 이름</param>
        /// <param name="objectVersion">대상 객체 버전</param>
        /// <param name="reason">사유</param>
        /// <param name="result">결과 (success, fail 등)</param>
        /// <param name="source">출처 (서명 문자열 등)</param>
        /// <param name="clientIp">클라이언트 IP / 머신명</param>
        /// <param name="extraJson">추가 JSONB 데이터 (null 가능)</param>
        public async Task<long> InsertOperationalAuditLogAsync(
            string userName, string action,
            string objectType, string objectName, int objectVersion,
            string reason, string result,
            string source, string clientIp,
            string extraJson = null)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"INSERT INTO operational.audit_log
                        (event_time, user_name, action, object_type, object_name,
                         object_version, reason, result, source, client_ip, extra)
                        VALUES (CURRENT_TIMESTAMP, @user, @action, @otype, @oname,
                                @over, @reason, @result, @source, @cip,
                                @extra::jsonb)
                        RETURNING id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", (object)userName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@otype", (object)objectType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@oname", (object)objectName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@over", objectVersion);
                        cmd.Parameters.AddWithValue("@reason", (object)reason ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@result", (object)result ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@source", (object)source ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@cip", (object)clientIp ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@extra", (object)extraJson ?? DBNull.Value);

                        var id = await cmd.ExecuteScalarAsync();
                        return id != null ? Convert.ToInt64(id) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"operational.audit_log INSERT 오류: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 레시피 승인 상태 업데이트 (status, approved_by, approved_at)
        /// </summary>
        public async Task<bool> UpdateRecipeStatusAsync(int recipeId, string newStatus, string approvedBy)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 대상 레시피 상태 업데이트
                            string sql = @"UPDATE system.recipe
                                SET status = @status, approved_by = @approvedBy, approved_at = @approvedAt
                                WHERE recipe_id = @rid";

                            using (var cmd = new NpgsqlCommand(sql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@rid", recipeId);
                                cmd.Parameters.AddWithValue("@status", newStatus);
                                cmd.Parameters.AddWithValue("@approvedBy", (object)approvedBy ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@approvedAt",
                                    (newStatus == "approved") ? (object)DateTime.Now : DBNull.Value);

                                int rows = await cmd.ExecuteNonQueryAsync();
                                if (rows == 0)
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                            }

                            // ISA-88: approve 시 같은 recipe_guid의 이전 approved 버전 → obsolete
                            if (newStatus == "approved")
                            {
                                string obsoleteSql = @"UPDATE system.recipe
                                    SET status = 'obsolete'
                                    WHERE recipe_guid = (SELECT recipe_guid FROM system.recipe WHERE recipe_id = @rid)
                                      AND recipe_id != @rid
                                      AND status = 'approved'
                                      AND (is_active = true OR is_active IS NULL)";

                                using (var cmd = new NpgsqlCommand(obsoleteSql, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@rid", recipeId);
                                    int obsoleted = await cmd.ExecuteNonQueryAsync();
                                    if (obsoleted > 0)
                                        Debug.WriteLine($"ISA-88: {obsoleted} previous approved version(s) set to obsolete for recipe_id={recipeId}");
                                }
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"레시피 상태 업데이트 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 레시피 감사 로그 조회 (Revision History)
        /// </summary>
        public async Task<ArrayList> GetRecipeAuditLogsAsync(int recipeId, int maxRows = 100)
        {
            var list = new ArrayList();
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"SELECT log_id, recipe_id, recipe_name, action,
                            target_type, target_name, old_value, new_value,
                            username, machine_name, created_at,
                            COALESCE(signature, '') AS signature,
                            COALESCE(reason, '') AS reason
                        FROM history.recipe_audit_log
                        WHERE recipe_id = @rid
                        ORDER BY created_at DESC
                        LIMIT @lim";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@rid", recipeId);
                        cmd.Parameters.AddWithValue("@lim", maxRows);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var entry = new RecipeAuditLogEntry();
                                entry.log_id = reader.GetInt64(0);
                                entry.recipe_id = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                entry.recipe_name = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                entry.action = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                entry.target_type = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                entry.target_name = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                entry.old_value = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                entry.new_value = reader.IsDBNull(7) ? "" : reader.GetString(7);
                                entry.username = reader.IsDBNull(8) ? "" : reader.GetString(8);
                                entry.machine_name = reader.IsDBNull(9) ? "" : reader.GetString(9);
                                entry.created_at = reader.IsDBNull(10) ? DateTime.MinValue : reader.GetDateTime(10);
                                entry.signature = reader.IsDBNull(11) ? "" : reader.GetString(11);
                                entry.reason = reader.IsDBNull(12) ? "" : reader.GetString(12);
                                list.Add(entry);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"감사 로그 조회 오류: {ex.Message}");
            }
            return list;
        }
    }

    /// <summary>
    /// 레시피 감사 로그 엔트리
    /// </summary>
    public class RecipeAuditLogEntry
    {
        public long log_id;
        public int recipe_id;
        public string recipe_name = "";
        public string action = "";
        public string target_type = "";
        public string target_name = "";
        public string old_value = "";
        public string new_value = "";
        public string username = "";
        public string machine_name = "";
        public DateTime created_at;
        public string signature = "";
        public string reason = "";
    }

    #endregion
}
