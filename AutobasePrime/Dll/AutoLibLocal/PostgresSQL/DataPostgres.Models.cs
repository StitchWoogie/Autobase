using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AutoLibLocal
{
    #region Recipe Data Classes

    /// <summary>
    /// 레시피 단계별 태그-값 쌍
    /// </summary>
    public class RecipeItemData
    {
        public int item_id;
        public string tag_name = "";
        public string set_value = "";
        public string value_type = "double"; // "double", "int", "string", "bool"
        public int item_order;

        public RecipeItemData Clone()
        {
            RecipeItemData item = new RecipeItemData();
            item.item_id = item_id;
            item.tag_name = tag_name;
            item.set_value = set_value;
            item.value_type = value_type;
            item.item_order = item_order;
            return item;
        }
    }

    /// <summary>
    /// ISA-88 Step Transition 타입
    /// </summary>
    public enum TransitionType
    {
        Complete = 0,    // 정상 완료 → 다음 Step
        Exception = 1,   // 예외 → abort_actions 실행 후 배치 실패
        Abort = 2,       // 즉시 중단
        Loop = 3,        // target_step_order로 점프 (max_loop_count 제한)
        End = 4          // 레시피 전체 종료 (나머지 Step 건너뛰기)
    }

    /// <summary>
    /// Step 전이 정의 (ISA-88 Transition-based Flow)
    /// </summary>
    public class StepTransition
    {
        /// <summary>평가 우선순위 (낮을수록 먼저 평가, 0 = 최고)</summary>
        public int priority;

        /// <summary>전이 조건 표현식 (RecipeExpressionEvaluator 문법)</summary>
        public string expression = "";

        /// <summary>전이 타입</summary>
        public TransitionType type = TransitionType.Complete;

        /// <summary>Loop 전이 시 점프 대상 step_order (-1 = 현재 Step 반복)</summary>
        public int target_step_order = -1;

        /// <summary>Loop 전이 시 최대 반복 횟수 (0 = 무제한)</summary>
        public int max_loop_count = 10;

        /// <summary>전이 설명 (UI 표시용)</summary>
        public string description = "";

        /// <summary>자동 전이 타임아웃 (ms). 0 = expression으로만 평가, >0 = step 시작 후 X ms에 자동 발동</summary>
        public int timeout_ms = 0;

        public StepTransition Clone()
        {
            return new StepTransition
            {
                priority = this.priority,
                expression = this.expression ?? "",
                type = this.type,
                target_step_order = this.target_step_order,
                max_loop_count = this.max_loop_count,
                description = this.description ?? "",
                timeout_ms = this.timeout_ms
            };
        }
    }

    /// <summary>
    /// 레시피 단계 (순차 실행 단위)
    /// </summary>
    public class RecipeStepData
    {
        public int step_id;
        public int unit_id;                 // 0 = recipe 직속 (Unit 없음)
        public int step_order;
        public string step_name = "";
        public int wait_time_ms;            // 조건 체크 전 최소 대기시간 (ms)
        public int timeout_ms = 30000;      // 종료 조건 대기 타임아웃 (기본 30초)
        public string condition_tag = "";    // 종료 조건 태그 (빈 문자열이면 조건 없음)
        public string condition_value = "";  // 종료 조건 비교값
        public string condition_type = "none"; // "none", "equal", "greater", "less"

        // ISA-88 Entry Condition (시작 조건)
        public string entry_condition_tag = "";      // 시작 조건 태그 (빈 문자열이면 조건 없음)
        public string entry_condition_value = "";    // 시작 조건 비교값
        public string entry_condition_type = "none"; // "none", "equal", "greater", "less"
        public int entry_timeout_ms = 0;             // 시작 조건 타임아웃 (0 = 무제한)

        // 표현식 기반 조건 (B-3: Item 5) — expression이 있으면 legacy 단일태그보다 우선
        public string entry_expression = "";         // "$AI_0000 > 10 && $DI_0001 == 1"
        public string exit_expression = "";          // "$TI_100 >= 80 || $DI_0005 == 1"

        // 확장 Step 실행 모델 (C-5: Item 11)
        public string running_expression = "";       // 실행 중 지속 체크; false → exception
        public string exit_actions_json = "";        // JSON: [{"tag_name":"..","set_value":"..","value_type":".."}]
        public string abort_actions_json = "";       // JSON: 동일 형식

        // ISA-88 Full Transition Model
        public string transitions_json = "";         // JSON: List<StepTransition> — 우선순위별 전이 목록

        public ArrayList items = new ArrayList(); // RecipeItemData 목록

        public RecipeStepData Clone()
        {
            RecipeStepData step = new RecipeStepData();
            step.step_id = step_id;
            step.unit_id = unit_id;
            step.step_order = step_order;
            step.step_name = step_name;
            step.wait_time_ms = wait_time_ms;
            step.timeout_ms = timeout_ms;
            step.condition_tag = condition_tag;
            step.condition_value = condition_value;
            step.condition_type = condition_type;
            step.entry_condition_tag = entry_condition_tag;
            step.entry_condition_value = entry_condition_value;
            step.entry_condition_type = entry_condition_type;
            step.entry_timeout_ms = entry_timeout_ms;
            step.entry_expression = entry_expression;
            step.exit_expression = exit_expression;
            step.running_expression = running_expression;
            step.exit_actions_json = exit_actions_json;
            step.abort_actions_json = abort_actions_json;
            step.transitions_json = transitions_json;
            for (int i = 0; i < items.Count; i++)
                step.items.Add(((RecipeItemData)items[i]).Clone());
            return step;
        }
    }

    /// <summary>
    /// ISA-88 Unit 계층 (Recipe → Unit → Step)
    /// </summary>
    public class RecipeUnitData
    {
        public int unit_id;
        public int recipe_id;
        public string unit_name = "";
        public int unit_order;
        public string description = "";
        public ArrayList steps = new ArrayList(); // RecipeStepData 목록

        public RecipeUnitData Clone()
        {
            RecipeUnitData unit = new RecipeUnitData();
            unit.unit_id = unit_id;
            unit.recipe_id = recipe_id;
            unit.unit_name = unit_name;
            unit.unit_order = unit_order;
            unit.description = description;
            for (int i = 0; i < steps.Count; i++)
                unit.steps.Add(((RecipeStepData)steps[i]).Clone());
            return unit;
        }
    }

    /// <summary>
    /// 레시피 목록 표시용 기본 정보
    /// </summary>
    public class RecipeInfo
    {
        public int recipe_id;
        public string recipe_name = "";
        public string description = "";
        public string recipe_guid = "";       // 불변 크로스시스템 식별자 (GUID 32자리 hex)
        public string recipe_code = "";       // 사람이 읽는 코드 — deprecated (항상 빈 문자열)
        public int version = 1;              // Studio 저장 시 자동 증가
        public bool is_active = true;        // 소프트 삭제 플래그
        public string status = "draft";      // "draft", "approved", "obsolete"
        public string approved_by = "";       // 승인자 사용자명
        public DateTime? approved_at;         // 승인 일시
        public string recipe_mode = "standard"; // "quick" or "standard"
        public DateTime created_at;
        public DateTime updated_at;
    }

    /// <summary>
    /// 레시피 전체 데이터 (Steps + Items 포함)
    /// </summary>
    public class RecipeData : RecipeInfo
    {
        public ArrayList steps = new ArrayList(); // RecipeStepData 목록 (recipe 직속, Unit 없는 step)
        public ArrayList units = new ArrayList(); // RecipeUnitData 목록 (ISA-88 Unit 계층)

        public RecipeData Clone()
        {
            RecipeData recipe = new RecipeData();
            recipe.recipe_id = recipe_id;
            recipe.recipe_name = recipe_name;
            recipe.description = description;
            recipe.recipe_guid = recipe_guid;
            recipe.recipe_code = recipe_code;
            recipe.version = version;
            recipe.is_active = is_active;
            recipe.status = status;
            recipe.approved_by = approved_by;
            recipe.approved_at = approved_at;
            recipe.recipe_mode = recipe_mode;
            recipe.created_at = created_at;
            recipe.updated_at = updated_at;
            for (int i = 0; i < steps.Count; i++)
                recipe.steps.Add(((RecipeStepData)steps[i]).Clone());
            for (int i = 0; i < units.Count; i++)
                recipe.units.Add(((RecipeUnitData)units[i]).Clone());
            return recipe;
        }
    }

    /// <summary>
    /// ISA-88 Control Recipe — 배치별 동결된 Master Recipe 스냅샷
    /// </summary>
    public class ControlRecipeInfo
    {
        public int control_recipe_id;
        public int master_recipe_id;
        public int master_version;
        public string batch_id = "";
        public string status = "pending";  // pending, running, completed, aborted
        public DateTime created_at;
        public string created_by = "";
    }

    /// <summary>
    /// ISA-88 Batch Execution Record — 배치 실행 이력 (감사/추적)
    /// </summary>
    public class BatchExecutionRecord
    {
        public string batch_id = "";
        public int control_recipe_id;
        public int master_recipe_id;
        public string master_recipe_name = "";
        public int master_version;
        public string operator_id = "";
        public DateTime? start_time;
        public DateTime? end_time;
        public string result = "";       // completed, aborted, failed
        public string status = "idle";   // ISA-88 state: idle, running, holding, held, restarting, aborting, aborted, complete
        public DateTime created_at;
    }

    /// <summary>
    /// 레시피 실행 로그 엔트리 (recipe_execution_log 조회용)
    /// </summary>
    public class RecipeExecutionLogEntry
    {
        public long log_id;
        public int recipe_id;
        public string recipe_name = "";
        public string unit_name = "";
        public string action = "";
        public string status = "";
        public int step_index;
        public int total_steps;
        public string error_message = "";
        public string username = "";
        public DateTime? execution_start;
        public DateTime? execution_end;
        public DateTime created_at;
    }

    #endregion

    /// <summary>
    /// 레시피 CSV 내보내기/가져오기 유틸리티
    /// </summary>
    public static class RecipeCsvHelper
    {
        public static bool ExportToCsv(RecipeData recipe, string filePath, out string error)
        {
            error = null;
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath, false, new System.Text.UTF8Encoding(true)))
                {
                    // [RECIPE] 섹션
                    writer.WriteLine("[RECIPE]");
                    writer.WriteLine("Name,Description");
                    writer.WriteLine("{0},{1}", CsvEscape(recipe.recipe_name), CsvEscape(recipe.description));
                    writer.WriteLine();

                    // [UNIT] 섹션 (Unit이 있는 경우에만)
                    if (recipe.units.Count > 0)
                    {
                        writer.WriteLine("[UNIT]");
                        writer.WriteLine("UnitOrder,UnitName,Description");
                        for (int u = 0; u < recipe.units.Count; u++)
                        {
                            var unit = (RecipeUnitData)recipe.units[u];
                            writer.WriteLine("{0},{1},{2}", unit.unit_order, CsvEscape(unit.unit_name), CsvEscape(unit.description));
                        }
                        writer.WriteLine();
                    }

                    // [STEP] 섹션 - UnitOrder 포함 (-1 = recipe 직속)
                    writer.WriteLine("[STEP]");
                    writer.WriteLine("UnitOrder,StepOrder,StepName,WaitTimeMs,TimeoutMs,ConditionTag,ConditionValue,ConditionType");

                    // Recipe 직속 step
                    for (int s = 0; s < recipe.steps.Count; s++)
                    {
                        var step = (RecipeStepData)recipe.steps[s];
                        writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}",
                            -1, step.step_order, CsvEscape(step.step_name), step.wait_time_ms, step.timeout_ms,
                            CsvEscape(step.condition_tag), CsvEscape(step.condition_value), CsvEscape(step.condition_type));
                    }

                    // Unit 소속 step
                    for (int u = 0; u < recipe.units.Count; u++)
                    {
                        var unit = (RecipeUnitData)recipe.units[u];
                        for (int s = 0; s < unit.steps.Count; s++)
                        {
                            var step = (RecipeStepData)unit.steps[s];
                            writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}",
                                unit.unit_order, step.step_order, CsvEscape(step.step_name), step.wait_time_ms, step.timeout_ms,
                                CsvEscape(step.condition_tag), CsvEscape(step.condition_value), CsvEscape(step.condition_type));
                        }
                    }
                    writer.WriteLine();

                    // [STEP_ITEM] 섹션
                    writer.WriteLine("[STEP_ITEM]");
                    writer.WriteLine("StepOrder,TagName,SetValue,ValueType,ItemOrder");

                    // Recipe 직속
                    for (int s = 0; s < recipe.steps.Count; s++)
                    {
                        var step = (RecipeStepData)recipe.steps[s];
                        WriteStepItems(writer, step);
                    }

                    // Unit 소속
                    for (int u = 0; u < recipe.units.Count; u++)
                    {
                        var unit = (RecipeUnitData)recipe.units[u];
                        for (int s = 0; s < unit.steps.Count; s++)
                        {
                            var step = (RecipeStepData)unit.steps[s];
                            WriteStepItems(writer, step);
                        }
                    }
                    writer.WriteLine();

                    // [STEP_TRANSITION] 섹션 — transitions_json 보유 Step만 출력
                    bool hasTransitions = false;
                    for (int s = 0; s < recipe.steps.Count; s++)
                    {
                        if (!string.IsNullOrEmpty(((RecipeStepData)recipe.steps[s]).transitions_json))
                        { hasTransitions = true; break; }
                    }
                    if (!hasTransitions)
                    {
                        for (int u = 0; u < recipe.units.Count && !hasTransitions; u++)
                        {
                            var unit = (RecipeUnitData)recipe.units[u];
                            for (int s = 0; s < unit.steps.Count; s++)
                            {
                                if (!string.IsNullOrEmpty(((RecipeStepData)unit.steps[s]).transitions_json))
                                { hasTransitions = true; break; }
                            }
                        }
                    }

                    if (hasTransitions)
                    {
                        writer.WriteLine("[STEP_TRANSITION]");
                        writer.WriteLine("StepOrder,Priority,Expression,Type,TargetStep,MaxLoop,TimeoutMs,Description");
                        WriteStepTransitions(writer, recipe.steps);
                        for (int u = 0; u < recipe.units.Count; u++)
                            WriteStepTransitions(writer, ((RecipeUnitData)recipe.units[u]).steps);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        static void WriteStepItems(System.IO.StreamWriter writer, RecipeStepData step)
        {
            for (int i = 0; i < step.items.Count; i++)
            {
                var item = (RecipeItemData)step.items[i];
                writer.WriteLine("{0},{1},{2},{3},{4}",
                    step.step_order, CsvEscape(item.tag_name), CsvEscape(item.set_value),
                    CsvEscape(item.value_type), item.item_order);
            }
        }

        static void WriteStepTransitions(System.IO.StreamWriter writer, System.Collections.ArrayList steps)
        {
            for (int s = 0; s < steps.Count; s++)
            {
                var step = (RecipeStepData)steps[s];
                if (string.IsNullOrEmpty(step.transitions_json)) continue;
                try
                {
                    var transitions = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<StepTransition>>(step.transitions_json);
                    if (transitions == null) continue;
                    for (int t = 0; t < transitions.Count; t++)
                    {
                        var tr = transitions[t];
                        writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7}",
                            step.step_order, tr.priority, CsvEscape(tr.expression),
                            tr.type.ToString(), tr.target_step_order, tr.max_loop_count,
                            tr.timeout_ms, CsvEscape(tr.description));
                    }
                }
                catch { }
            }
        }

        public static RecipeData ImportFromCsv(string filePath, out string error)
        {
            error = null;
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    error = "파일이 존재하지 않습니다.";
                    return null;
                }

                RecipeData recipe = new RecipeData();
                string currentSection = "";
                System.Collections.Hashtable stepOrderMap = new System.Collections.Hashtable(); // stepOrder → RecipeStepData
                System.Collections.Hashtable unitOrderMap = new System.Collections.Hashtable(); // unitOrder → RecipeUnitData
                System.Collections.Hashtable transitionsPerStep = new System.Collections.Hashtable(); // stepOrder → List<StepTransition>
                bool hasUnitSection = false;
                bool newStepFormat = false; // UnitOrder,StepOrder,...  (8 fields) vs StepOrder,... (7 fields)
                string[] lines = System.IO.File.ReadAllLines(filePath, System.Text.Encoding.UTF8);

                // 사전 스캔: [UNIT] 섹션 및 새 STEP 포맷 존재 여부
                for (int i = 0; i < lines.Length; i++)
                {
                    string l = lines[i].Trim();
                    if (l == "[UNIT]") hasUnitSection = true;
                    if (l.StartsWith("UnitOrder,StepOrder,")) newStepFormat = true;
                }

                for (int lineNum = 0; lineNum < lines.Length; lineNum++)
                {
                    string line = lines[lineNum].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        currentSection = line;
                        continue;
                    }

                    // 헤더 행 스킵
                    if (line.StartsWith("Name,") || line.StartsWith("StepOrder,") ||
                        line.StartsWith("UnitOrder,StepOrder,") || line.StartsWith("UnitOrder,UnitName,") ||
                        (currentSection == "[STEP_TRANSITION]" && line.StartsWith("StepOrder,Priority,")))
                        continue;

                    string[] fields = CsvParseLine(line);

                    if (currentSection == "[RECIPE]")
                    {
                        if (fields.Length < 2) { error = string.Format("Line {0}: RECIPE 필드 부족.", lineNum + 1); return null; }
                        recipe.recipe_name = fields[0];
                        recipe.description = fields[1];
                    }
                    else if (currentSection == "[UNIT]")
                    {
                        if (fields.Length < 3) { error = string.Format("Line {0}: UNIT 필드 부족.", lineNum + 1); return null; }
                        var unit = new RecipeUnitData();
                        int unitOrder;
                        if (!int.TryParse(fields[0], out unitOrder)) { error = string.Format("Line {0}: UnitOrder 무효.", lineNum + 1); return null; }
                        unit.unit_order = unitOrder;
                        unit.unit_name = fields[1];
                        unit.description = fields[2];
                        recipe.units.Add(unit);
                        unitOrderMap[unitOrder] = unit;
                    }
                    else if (currentSection == "[STEP]")
                    {
                        if (newStepFormat)
                        {
                            // 새 포맷: UnitOrder,StepOrder,StepName,...
                            if (fields.Length < 8) { error = string.Format("Line {0}: STEP 필드 부족.", lineNum + 1); return null; }
                            int unitOrder;
                            int.TryParse(fields[0], out unitOrder);
                            var step = new RecipeStepData();
                            int stepOrder;
                            if (!int.TryParse(fields[1], out stepOrder)) { error = string.Format("Line {0}: StepOrder 무효.", lineNum + 1); return null; }
                            step.step_order = stepOrder;
                            step.step_name = fields[2];
                            int.TryParse(fields[3], out step.wait_time_ms);
                            int.TryParse(fields[4], out step.timeout_ms);
                            if (step.timeout_ms <= 0) step.timeout_ms = 30000;
                            step.condition_tag = fields[5];
                            step.condition_value = fields[6];
                            step.condition_type = fields[7];
                            if (string.IsNullOrEmpty(step.condition_type)) step.condition_type = "none";

                            if (unitOrder >= 0 && unitOrderMap.ContainsKey(unitOrder))
                                ((RecipeUnitData)unitOrderMap[unitOrder]).steps.Add(step);
                            else
                                recipe.steps.Add(step);

                            stepOrderMap[stepOrder] = step;
                        }
                        else
                        {
                            // 기존 포맷 호환: StepOrder,StepName,...
                            if (fields.Length < 7) { error = string.Format("Line {0}: STEP 필드 부족.", lineNum + 1); return null; }
                            var step = new RecipeStepData();
                            int stepOrder;
                            if (!int.TryParse(fields[0], out stepOrder)) { error = string.Format("Line {0}: StepOrder 무효.", lineNum + 1); return null; }
                            if (stepOrderMap.ContainsKey(stepOrder)) { error = string.Format("Line {0}: StepOrder {1} 중복.", lineNum + 1, stepOrder); return null; }
                            step.step_order = stepOrder;
                            step.step_name = fields[1];
                            int.TryParse(fields[2], out step.wait_time_ms);
                            int.TryParse(fields[3], out step.timeout_ms);
                            if (step.timeout_ms <= 0) step.timeout_ms = 30000;
                            step.condition_tag = fields[4];
                            step.condition_value = fields[5];
                            step.condition_type = fields[6];
                            if (string.IsNullOrEmpty(step.condition_type)) step.condition_type = "none";
                            recipe.steps.Add(step);
                            stepOrderMap[stepOrder] = step;
                        }
                    }
                    else if (currentSection == "[STEP_ITEM]")
                    {
                        if (fields.Length < 5) { error = string.Format("Line {0}: STEP_ITEM 필드 부족.", lineNum + 1); return null; }
                        int stepOrder;
                        if (!int.TryParse(fields[0], out stepOrder)) { error = string.Format("Line {0}: StepOrder 무효.", lineNum + 1); return null; }
                        if (!stepOrderMap.ContainsKey(stepOrder)) { error = string.Format("Line {0}: StepOrder {1} STEP 없음.", lineNum + 1, stepOrder); return null; }
                        var step = (RecipeStepData)stepOrderMap[stepOrder];
                        var item = new RecipeItemData();
                        item.tag_name = fields[1];
                        item.set_value = fields[2];
                        item.value_type = fields[3];
                        int.TryParse(fields[4], out item.item_order);
                        if (string.IsNullOrEmpty(item.tag_name)) { error = string.Format("Line {0}: TagName 비어있음.", lineNum + 1); return null; }
                        step.items.Add(item);
                    }
                    else if (currentSection == "[STEP_TRANSITION]")
                    {
                        // StepOrder,Priority,Expression,Type,TargetStep,MaxLoop,TimeoutMs,Description
                        if (fields.Length < 7) continue;
                        int stepOrder;
                        if (!int.TryParse(fields[0], out stepOrder)) continue;
                        if (!stepOrderMap.ContainsKey(stepOrder)) continue;

                        var tr = new StepTransition();
                        int.TryParse(fields[1], out tr.priority);
                        tr.expression = fields[2];
                        TransitionType ttype;
                        if (System.Enum.TryParse(fields[3], true, out ttype))
                            tr.type = ttype;
                        int.TryParse(fields[4], out tr.target_step_order);
                        int.TryParse(fields[5], out tr.max_loop_count);
                        int.TryParse(fields[6], out tr.timeout_ms);
                        if (fields.Length >= 8) tr.description = fields[7];

                        // transitionsPerStep에 누적
                        if (!transitionsPerStep.ContainsKey(stepOrder))
                            transitionsPerStep[stepOrder] = new System.Collections.Generic.List<StepTransition>();
                        ((System.Collections.Generic.List<StepTransition>)transitionsPerStep[stepOrder]).Add(tr);
                    }
                }

                // [STEP_TRANSITION] 수집된 전이를 transitions_json으로 직렬화
                foreach (System.Collections.DictionaryEntry de in transitionsPerStep)
                {
                    int so = (int)de.Key;
                    if (stepOrderMap.ContainsKey(so))
                    {
                        var step = (RecipeStepData)stepOrderMap[so];
                        var trList = (System.Collections.Generic.List<StepTransition>)de.Value;
                        step.transitions_json = Newtonsoft.Json.JsonConvert.SerializeObject(trList);
                    }
                }

                if (string.IsNullOrEmpty(recipe.recipe_name)) { error = "레시피 이름이 없습니다."; return null; }
                return recipe;
            }
            catch (System.Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        static string CsvEscape(string val)
        {
            if (val == null) return "";
            if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
                return "\"" + val.Replace("\"", "\"\"") + "\"";
            return val;
        }

        static string[] CsvParseLine(string line)
        {
            System.Collections.ArrayList fields = new System.Collections.ArrayList();
            System.Text.StringBuilder current = new System.Text.StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (inQuotes)
                {
                    if (c == '"') { if (i + 1 < line.Length && line[i + 1] == '"') { current.Append('"'); i++; } else { inQuotes = false; } }
                    else { current.Append(c); }
                }
                else
                {
                    if (c == '"') { inQuotes = true; }
                    else if (c == ',') { fields.Add(current.ToString()); current.Clear(); }
                    else { current.Append(c); }
                }
            }
            fields.Add(current.ToString());
            string[] result = new string[fields.Count];
            for (int i = 0; i < fields.Count; i++) result[i] = (string)fields[i];
            return result;
        }
    }

    /// <summary>
    /// MilliData 태그 정보 클래스
    /// </summary>
    public class MilliDataTagInfo
    {
        public string TagName { get; set; }
        public int TagType { get; set; }  // 0: AI, 1: DI, 9: ST
        public float FullScale { get; set; }
        public float BaseValue { get; set; }
    }
}
