using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using AutoLibLocal;

namespace AutoLibLocal
{
    /// <summary>
    /// JSON 레시피 직렬화/역직렬화 (Studio ↔ JSON 파일)
    /// DB ID 필드 제외, GUID/Code/Version 포함
    /// </summary>
    public static class RecipeJsonHelper
    {
        #region DTO 클래스 (JSON 직렬화 전용)

        public class RecipeJsonDto
        {
            [JsonProperty("recipe_guid")]
            public string recipe_guid;
            [JsonProperty("recipe_code", NullValueHandling = NullValueHandling.Ignore)]
            public string recipe_code;  // 역직렬화(구 JSON 호환)만 유지, 직렬화 시 제외됨
            [JsonProperty("version")]
            public int version;
            [JsonProperty("recipe_name")]
            public string recipe_name;
            [JsonProperty("description")]
            public string description;
            [JsonProperty("recipe_mode", NullValueHandling = NullValueHandling.Ignore)]
            public string recipe_mode;
            [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
            public string status;
            [JsonProperty("units")]
            public List<RecipeUnitJsonDto> units = new List<RecipeUnitJsonDto>();
            [JsonProperty("steps")]
            public List<RecipeStepJsonDto> steps = new List<RecipeStepJsonDto>();
        }

        public class RecipeUnitJsonDto
        {
            [JsonProperty("unit_name")]
            public string unit_name;
            [JsonProperty("unit_order")]
            public int unit_order;
            [JsonProperty("description")]
            public string description;
            [JsonProperty("steps")]
            public List<RecipeStepJsonDto> steps = new List<RecipeStepJsonDto>();
        }

        public class RecipeStepJsonDto
        {
            [JsonProperty("step_order")]
            public int step_order;
            [JsonProperty("step_name")]
            public string step_name;
            [JsonProperty("wait_time_ms")]
            public int wait_time_ms;
            [JsonProperty("timeout_ms")]
            public int timeout_ms = 30000;
            [JsonProperty("condition_tag")]
            public string condition_tag;
            [JsonProperty("condition_value")]
            public string condition_value;
            [JsonProperty("condition_type")]
            public string condition_type = "none";

            // ISA-88 Entry Condition (시작 조건)
            [JsonProperty("entry_condition_tag", NullValueHandling = NullValueHandling.Ignore)]
            public string entry_condition_tag;
            [JsonProperty("entry_condition_value", NullValueHandling = NullValueHandling.Ignore)]
            public string entry_condition_value;
            [JsonProperty("entry_condition_type", NullValueHandling = NullValueHandling.Ignore)]
            public string entry_condition_type;
            [JsonProperty("entry_timeout_ms", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public int entry_timeout_ms;

            // 표현식 기반 조건 (B-3: Item 5)
            [JsonProperty("entry_expression", NullValueHandling = NullValueHandling.Ignore)]
            public string entry_expression;
            [JsonProperty("exit_expression", NullValueHandling = NullValueHandling.Ignore)]
            public string exit_expression;

            // 확장 Step 실행 모델 (C-5: Item 11)
            [JsonProperty("running_expression", NullValueHandling = NullValueHandling.Ignore)]
            public string running_expression;
            [JsonProperty("exit_actions_json", NullValueHandling = NullValueHandling.Ignore)]
            public string exit_actions_json;
            [JsonProperty("abort_actions_json", NullValueHandling = NullValueHandling.Ignore)]
            public string abort_actions_json;

            // ISA-88 Full Transition Model
            [JsonProperty("transitions_json", NullValueHandling = NullValueHandling.Ignore)]
            public string transitions_json;

            [JsonProperty("items")]
            public List<RecipeItemJsonDto> items = new List<RecipeItemJsonDto>();
        }

        public class RecipeItemJsonDto
        {
            [JsonProperty("tag_name")]
            public string tag_name;
            [JsonProperty("set_value")]
            public string set_value;
            [JsonProperty("value_type")]
            public string value_type = "double";
            [JsonProperty("item_order")]
            public int item_order;
        }

        #endregion

        #region 파일 I/O

        /// <summary>
        /// RecipeData 리스트를 JSON 파일로 저장
        /// </summary>
        public static bool SaveToJsonFile(List<RecipeData> recipes, string filePath, out string error)
        {
            error = null;
            try
            {
                // 디렉토리 생성
                string dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // RecipeData → DTO 변환
                var dtos = new List<RecipeJsonDto>();
                for (int i = 0; i < recipes.Count; i++)
                    dtos.Add(ToDto(recipes[i]));

                // JSON 직렬화 (들여쓰기 포함, UTF-8 BOM)
                string json = JsonConvert.SerializeObject(dtos, Formatting.Indented);
                File.WriteAllText(filePath, json, new UTF8Encoding(true));
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// JSON 파일에서 RecipeData 리스트 로드
        /// </summary>
        public static List<RecipeData> LoadFromJsonFile(string filePath, out string error)
        {
            error = null;
            try
            {
                if (!File.Exists(filePath))
                {
                    error = "파일이 존재하지 않습니다: " + filePath;
                    return null;
                }

                string json = File.ReadAllText(filePath, Encoding.UTF8);
                var dtos = JsonConvert.DeserializeObject<List<RecipeJsonDto>>(json);
                if (dtos == null)
                {
                    error = "JSON 파싱 결과가 null입니다.";
                    return null;
                }

                // DTO → RecipeData 변환
                var recipes = new List<RecipeData>();
                for (int i = 0; i < dtos.Count; i++)
                    recipes.Add(FromDto(dtos[i]));

                // 구조 검증
                string validationError = ValidateRecipes(recipes);
                if (validationError != null)
                {
                    error = validationError;
                    return null;
                }

                return recipes;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        #endregion

        #region 변환 메서드

        /// <summary>
        /// RecipeData → JSON 문자열 (Control Recipe 스냅샷 생성용)
        /// </summary>
        public static string ToJsonString(RecipeData recipe)
        {
            var dto = ToDto(recipe);
            return JsonConvert.SerializeObject(dto, Formatting.None);
        }

        /// <summary>
        /// JSON 문자열 → RecipeData (Control Recipe 스냅샷 복원용)
        /// </summary>
        public static RecipeData FromJsonString(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            var dto = JsonConvert.DeserializeObject<RecipeJsonDto>(json);
            if (dto == null) return null;
            return FromDto(dto);
        }

        /// <summary>
        /// RecipeData → JSON DTO (DB ID 제거)
        /// </summary>
        static RecipeJsonDto ToDto(RecipeData recipe)
        {
            var dto = new RecipeJsonDto();
            dto.recipe_guid = recipe.recipe_guid;
            // recipe_code 제거됨 — 직렬화 시 null → JSON에 포함 안됨
            dto.version = recipe.version;
            dto.recipe_name = recipe.recipe_name;
            dto.description = recipe.description;
            dto.recipe_mode = recipe.recipe_mode;
            dto.status = recipe.status;

            // Recipe 직속 Steps
            for (int s = 0; s < recipe.steps.Count; s++)
                dto.steps.Add(StepToDto((RecipeStepData)recipe.steps[s]));

            // Units
            for (int u = 0; u < recipe.units.Count; u++)
            {
                var unit = (RecipeUnitData)recipe.units[u];
                var unitDto = new RecipeUnitJsonDto();
                unitDto.unit_name = unit.unit_name;
                unitDto.unit_order = unit.unit_order;
                unitDto.description = unit.description;
                for (int s = 0; s < unit.steps.Count; s++)
                    unitDto.steps.Add(StepToDto((RecipeStepData)unit.steps[s]));
                dto.units.Add(unitDto);
            }

            return dto;
        }

        static RecipeStepJsonDto StepToDto(RecipeStepData step)
        {
            var dto = new RecipeStepJsonDto();
            dto.step_order = step.step_order;
            dto.step_name = step.step_name;
            dto.wait_time_ms = step.wait_time_ms;
            dto.timeout_ms = step.timeout_ms;
            dto.condition_tag = step.condition_tag;
            dto.condition_value = step.condition_value;
            dto.condition_type = step.condition_type;

            // Entry Condition (빈 값은 NullValueHandling.Ignore로 JSON에서 생략됨)
            if (!string.IsNullOrEmpty(step.entry_condition_tag) || step.entry_condition_type != "none")
            {
                dto.entry_condition_tag = step.entry_condition_tag;
                dto.entry_condition_value = step.entry_condition_value;
                dto.entry_condition_type = step.entry_condition_type;
                dto.entry_timeout_ms = step.entry_timeout_ms;
            }

            // 표현식 기반 조건 + 확장 모델 (빈 문자열은 null로 변환 → JSON 생략)
            dto.entry_expression = string.IsNullOrEmpty(step.entry_expression) ? null : step.entry_expression;
            dto.exit_expression = string.IsNullOrEmpty(step.exit_expression) ? null : step.exit_expression;
            dto.running_expression = string.IsNullOrEmpty(step.running_expression) ? null : step.running_expression;
            dto.exit_actions_json = string.IsNullOrEmpty(step.exit_actions_json) ? null : step.exit_actions_json;
            dto.abort_actions_json = string.IsNullOrEmpty(step.abort_actions_json) ? null : step.abort_actions_json;
            dto.transitions_json = string.IsNullOrEmpty(step.transitions_json) ? null : step.transitions_json;

            for (int i = 0; i < step.items.Count; i++)
            {
                var item = (RecipeItemData)step.items[i];
                var itemDto = new RecipeItemJsonDto();
                itemDto.tag_name = item.tag_name;
                itemDto.set_value = item.set_value;
                itemDto.value_type = item.value_type;
                itemDto.item_order = item.item_order;
                dto.items.Add(itemDto);
            }
            return dto;
        }

        /// <summary>
        /// JSON DTO → RecipeData (DB ID = 0)
        /// </summary>
        static RecipeData FromDto(RecipeJsonDto dto)
        {
            var recipe = new RecipeData();
            recipe.recipe_id = 0;
            recipe.recipe_guid = dto.recipe_guid ?? "";
            recipe.recipe_code = dto.recipe_code ?? "";
            recipe.version = dto.version;
            recipe.recipe_name = dto.recipe_name ?? "";
            recipe.description = dto.description ?? "";
            recipe.recipe_mode = !string.IsNullOrEmpty(dto.recipe_mode) ? dto.recipe_mode : "standard";
            recipe.status = !string.IsNullOrEmpty(dto.status) ? dto.status : "draft";
            recipe.is_active = true;

            // Recipe 직속 Steps
            if (dto.steps != null)
            {
                for (int s = 0; s < dto.steps.Count; s++)
                    recipe.steps.Add(StepFromDto(dto.steps[s]));
            }

            // Units
            if (dto.units != null)
            {
                for (int u = 0; u < dto.units.Count; u++)
                {
                    var unitDto = dto.units[u];
                    var unit = new RecipeUnitData();
                    unit.unit_id = 0;
                    unit.unit_name = unitDto.unit_name ?? "";
                    unit.unit_order = unitDto.unit_order;
                    unit.description = unitDto.description ?? "";
                    if (unitDto.steps != null)
                    {
                        for (int s = 0; s < unitDto.steps.Count; s++)
                            unit.steps.Add(StepFromDto(unitDto.steps[s]));
                    }
                    recipe.units.Add(unit);
                }
            }

            return recipe;
        }

        static RecipeStepData StepFromDto(RecipeStepJsonDto dto)
        {
            var step = new RecipeStepData();
            step.step_id = 0;
            step.step_order = dto.step_order;
            step.step_name = dto.step_name ?? "";
            step.wait_time_ms = dto.wait_time_ms;
            step.timeout_ms = dto.timeout_ms;
            step.condition_tag = dto.condition_tag ?? "";
            step.condition_value = dto.condition_value ?? "";
            step.condition_type = dto.condition_type ?? "none";

            // Entry Condition (구 JSON 호환: 없으면 기본값)
            step.entry_condition_tag = dto.entry_condition_tag ?? "";
            step.entry_condition_value = dto.entry_condition_value ?? "";
            step.entry_condition_type = dto.entry_condition_type ?? "none";
            step.entry_timeout_ms = dto.entry_timeout_ms;

            // 표현식 기반 조건 + 확장 모델 (구 JSON 호환: null → 빈 문자열)
            step.entry_expression = dto.entry_expression ?? "";
            step.exit_expression = dto.exit_expression ?? "";
            step.running_expression = dto.running_expression ?? "";
            step.exit_actions_json = dto.exit_actions_json ?? "";
            step.abort_actions_json = dto.abort_actions_json ?? "";
            step.transitions_json = dto.transitions_json ?? "";

            if (dto.items != null)
            {
                for (int i = 0; i < dto.items.Count; i++)
                {
                    var itemDto = dto.items[i];
                    var item = new RecipeItemData();
                    item.item_id = 0;
                    item.tag_name = itemDto.tag_name ?? "";
                    item.set_value = itemDto.set_value ?? "";
                    item.value_type = itemDto.value_type ?? "double";
                    item.item_order = itemDto.item_order;
                    step.items.Add(item);
                }
            }
            return step;
        }

        #endregion

        #region 검증

        /// <summary>
        /// 레시피 리스트 구조 검증
        /// </summary>
        /// <returns>null이면 유효, 그 외 오류 메시지</returns>
        public static string ValidateRecipes(List<RecipeData> recipes)
        {
            if (recipes == null || recipes.Count == 0) return null; // 빈 리스트는 허용

            var guidSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int r = 0; r < recipes.Count; r++)
            {
                var recipe = recipes[r];

                if (string.IsNullOrEmpty(recipe.recipe_name))
                    return string.Format("레시피 #{0}: 이름이 비어있습니다.", r + 1);

                if (string.IsNullOrEmpty(recipe.recipe_guid))
                    return string.Format("레시피 '{0}': GUID가 비어있습니다.", recipe.recipe_name);

                if (!guidSet.Add(recipe.recipe_guid))
                    return string.Format("레시피 '{0}': GUID가 중복됩니다 ({1}).", recipe.recipe_name, recipe.recipe_guid);

                // Unit 이름 중복 검사
                var unitNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int u = 0; u < recipe.units.Count; u++)
                {
                    var unit = (RecipeUnitData)recipe.units[u];
                    if (!string.IsNullOrEmpty(unit.unit_name) && !unitNames.Add(unit.unit_name))
                        return string.Format("레시피 '{0}': Unit 이름 '{1}'이(가) 중복됩니다.", recipe.recipe_name, unit.unit_name);
                }

                // 각 Step의 태그 검사
                ArrayList allSteps = new ArrayList();
                for (int s = 0; s < recipe.steps.Count; s++) allSteps.Add(recipe.steps[s]);
                for (int u = 0; u < recipe.units.Count; u++)
                {
                    var unit = (RecipeUnitData)recipe.units[u];
                    for (int s = 0; s < unit.steps.Count; s++) allSteps.Add(unit.steps[s]);
                }

                for (int s = 0; s < allSteps.Count; s++)
                {
                    var step = (RecipeStepData)allSteps[s];
                    var tagSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < step.items.Count; i++)
                    {
                        var item = (RecipeItemData)step.items[i];
                        if (string.IsNullOrEmpty(item.tag_name))
                            return string.Format("레시피 '{0}', Step '{1}': 태그명이 비어있습니다.",
                                recipe.recipe_name, step.step_name);
                        if (!tagSet.Add(item.tag_name))
                            return string.Format("레시피 '{0}', Step '{1}': 태그 '{2}'이(가) 중복됩니다.",
                                recipe.recipe_name, step.step_name, item.tag_name);
                    }
                }
            }

            return null; // 유효
        }

        #endregion

        #region 내용 비교

        /// <summary>
        /// 두 레시피의 내용이 동일한지 비교 (DB ID, timestamp 제외)
        /// </summary>
        public static bool IsContentEqual(RecipeData a, RecipeData b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;

            // 기본 필드
            if (a.recipe_name != b.recipe_name) return false;
            if (a.description != b.description) return false;

            // 직속 Steps
            if (a.steps.Count != b.steps.Count) return false;
            for (int s = 0; s < a.steps.Count; s++)
            {
                if (!IsStepEqual((RecipeStepData)a.steps[s], (RecipeStepData)b.steps[s]))
                    return false;
            }

            // Units
            if (a.units.Count != b.units.Count) return false;
            for (int u = 0; u < a.units.Count; u++)
            {
                var ua = (RecipeUnitData)a.units[u];
                var ub = (RecipeUnitData)b.units[u];
                if (ua.unit_name != ub.unit_name) return false;
                if (ua.unit_order != ub.unit_order) return false;
                if (ua.description != ub.description) return false;
                if (ua.steps.Count != ub.steps.Count) return false;
                for (int s = 0; s < ua.steps.Count; s++)
                {
                    if (!IsStepEqual((RecipeStepData)ua.steps[s], (RecipeStepData)ub.steps[s]))
                        return false;
                }
            }

            return true;
        }

        static bool IsStepEqual(RecipeStepData a, RecipeStepData b)
        {
            if (a.step_order != b.step_order) return false;
            if (a.step_name != b.step_name) return false;
            if (a.wait_time_ms != b.wait_time_ms) return false;
            if (a.timeout_ms != b.timeout_ms) return false;
            if (a.condition_tag != b.condition_tag) return false;
            if (a.condition_value != b.condition_value) return false;
            if (a.condition_type != b.condition_type) return false;
            // Entry Condition 비교
            if (a.entry_condition_tag != b.entry_condition_tag) return false;
            if (a.entry_condition_value != b.entry_condition_value) return false;
            if (a.entry_condition_type != b.entry_condition_type) return false;
            if (a.entry_timeout_ms != b.entry_timeout_ms) return false;
            // 표현식 + 확장 모델 비교
            if (a.entry_expression != b.entry_expression) return false;
            if (a.exit_expression != b.exit_expression) return false;
            if (a.running_expression != b.running_expression) return false;
            if (a.exit_actions_json != b.exit_actions_json) return false;
            if (a.abort_actions_json != b.abort_actions_json) return false;
            if (a.transitions_json != b.transitions_json) return false;
            if (a.items.Count != b.items.Count) return false;
            for (int i = 0; i < a.items.Count; i++)
            {
                var ia = (RecipeItemData)a.items[i];
                var ib = (RecipeItemData)b.items[i];
                if (ia.tag_name != ib.tag_name) return false;
                if (ia.set_value != ib.set_value) return false;
                if (ia.value_type != ib.value_type) return false;
                if (ia.item_order != ib.item_order) return false;
            }
            return true;
        }

        #endregion
    }
}
