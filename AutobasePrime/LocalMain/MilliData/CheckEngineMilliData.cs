using System;
using System.Collections;
using AutoLib;
using AutoLibLocal;
using NetTools.OldDefine;
using System.Data;
using System.IO;
using NetTools;
using GraphicModule;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NpgsqlTypes;

namespace LocalMain
{
    /// <summary>
    /// PostgreSQL TimescaleDB를 이용한 MilliData 처리
    ///     /// - data_time에 밀리초 포함
    /// - 파일 단위 분할 로직 제거
    /// </summary>
    public class CheckEngineMilliData
    {
        private static bool m_Done = false;
        private static int nThreadCount = 0;
        private static int nCheckPos = 0;
        private static DataPostgres _dataPostgres;

        // history 스키마 사용
        private const string HISTORY_SCHEMA = "history";

        static CheckEngineMilliData()
        {
            _dataPostgres = DataPostgres.Instance;
        }

        private static MilliDataBatchProcessor _batchProcessor;
        private static bool _useBatchMode = true; // 설정으로 제어

        public static void Init()
        {
            // 미세 자료 감시가 없으면 초기화 불필요 251028 PSU
            if (MilliData.blockMilliData.Count == 0) return;

            // TimescaleDB 초기화 확인
            if (_dataPostgres == null)
            {
                Debug.WriteLine("DataPostgres 인스턴스를 초기화할 수 없습니다.");
                return;
            }

            if (_useBatchMode)
            {
                _batchProcessor = new MilliDataBatchProcessor(
                    batchSize: 10,      // 10개씩 모아서 저장
                    flushIntervalMs: 1000 // 1초마다 강제 저장
                );
            }
        }

        public static void UnInit()
        {
            m_Done = true;
            TimeOutClass timeout = new TimeOutClass();
            while (nThreadCount > 0)
            {
                Thread.Sleep(1);
                if (timeout.IsTimeOut(3)) break;   // 너무 오랜시간이 지났다.
            }
            _batchProcessor?.Dispose(); // 자동으로 FlushAll() 호출
            SaveRemainMilliData();
        }

        static async Task CheckMilliDataOne(MILLI_DATA_STRUCT item)
        {
            //if (item.bThread)    return;            
            if (item.bErrorFlag) 
                return;			// 자료 저장에 문제가 있다.

            if (item.nCondition == 1) // DI ON동안만 수집
            {				
                await CheckMilliDataOneByDI(item).ConfigureAwait(false);
            }
            else if (item.nCondition == 2) 	// 계속 수집
            {		
                await CheckMilliDataOneByContinue(item).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// PlanToolStatusLocal 에서 호출.
        /// </summary>
        public static async Task CheckMilliData()
        {
            if (MilliData.blockMilliData.Count == 0) return;	// 미세 자료 감시가 없다.

            MILLI_DATA_STRUCT item;
            int l;
            TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();

            for (l = 0; l < MilliData.blockMilliData.Count; l++)
            {
                if (timeout.IsTimeOut(100))     // 감시 프로그램이 너무 부하가 걸리지 않게 한다. 2010-7-20 200->100으로 줄임
                {
                    break;
                }

                DebugSpeed speed = new DebugSpeed();
                speed.Start();
                nCheckPos %= MilliData.blockMilliData.Count;

                item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[nCheckPos];
                try
                {
                    await CheckMilliDataOne(item).ConfigureAwait(false);
                }
                catch(Exception ex)
                {

                    MessageDisplay.Show("MilliData Error:Title={0}\nError Message={1}", item.title, ex.Message);
                }
                nCheckPos++;
                speed.Stop("MilliData");
            }
        }

        static async Task CheckMilliDataOneByDI(MILLI_DATA_STRUCT item)
        {
            if (item.bErrorFlag) return;

            TagDiClass di = TagLib.GetStructDI(item.tagCheckDI, ref item.tag_pos);
            SYSTEMTIME time = new SYSTEMTIME();

            if (di.curr == 1 && item.old_di_curr == 0)
            {	// 발생.
                if (item.bSavingFlag == 0)
                {
                    time.GetLocalTime();
                    await MakeHeaderAndField(item, time).ConfigureAwait(false);
                }
            }
            else if (di.curr == 0 && item.old_di_curr == 1)
            {	// 복귀
                if (item.bSavingFlag == 1)
                {
                    item.bSavingFlag = 0;
                }
            }
            else if (di.curr == 1)
            {	// 진행 중.
                if (item.bSavingFlag == 1)
                {
                    time.GetLocalTime();
                    // CSV 파일 분할 체크 (DB는 분할하지 않음)
                    if (item.nSaveFileType == 1 || item.nSaveFileType == 2)
                    {
                        await CsvCuttingCheck(item, time).ConfigureAwait(false);
                    }
                    else
                    {
                        await RemainCheck(item, time).ConfigureAwait(false);
                    }
                }
            }

            item.old_di_curr = di.curr;
        }

        static async Task CheckMilliDataOneByContinue(MILLI_DATA_STRUCT item)
        {
            if (item.bErrorFlag) return;

            SYSTEMTIME time = new SYSTEMTIME();

            if (item.bSavingFlag == 0)
            {	// first routine
                time.GetLocalTime();
                await MakeHeaderAndField(item, time).ConfigureAwait(false);
            }
            else
            {
                time.GetLocalTime();
                // CSV 파일 분할 체크 (DB는 분할하지 않음)
                if (item.nSaveFileType == 1 || item.nSaveFileType == 2)
                {
                    await CsvCuttingCheck(item, time).ConfigureAwait(false);
                }
                else
                {
                    await RemainCheck(item, time).ConfigureAwait(false);
                }
            }
        }

        static string MakeTableName(MILLI_DATA_STRUCT item)
        {
            // 단순화된 테이블명: millidata_{title}
            string tableName = "millidata_" + item.title.Replace(" ", "_").Replace("-", "_");

            if (item.bUseFilenameAddition)
            {
                TagStClass st = TagLib.GetStructST(item.sUseFilenameAdditionTag, ref item.tag_posUseFilenameAddition);

                if (item.tag_posUseFilenameAddition[0] != TagLib.TAG_NOT_FOUND)
                {
                    string curr = st.curr.Trim();
                    if (curr.Length > 0)
                    {
                        curr = curr.Replace(" ", "_").Replace("-", "_").Replace(".", "_")
                                  .Replace("(", "_").Replace(")", "_").Replace("[", "_").Replace("]", "_")
                                  .Replace("/", "_").Replace("\\", "_").Replace(":", "_");
                        tableName = tableName + "_" + curr;
                    }
                }
            }

            return tableName.ToLower();
        }

        /// <summary>
        ///  시간을 사이클에 맞는 시간으로 맞춘다
        /// </summary>
        static void FitTimeWithCycle(int cycle, SYSTEMTIME st)
        {
            int day_millisec = st.wHour * 60 * 60 * 1000 + st.wMinute * 60 * 1000 + st.wSecond * 1000 + st.wMilliseconds;

            if (cycle != 0)
            {
                day_millisec = (day_millisec / cycle) * cycle;
                st.wHour = (ushort)(day_millisec / (60 * 60 * 1000));
                day_millisec %= (60 * 60 * 1000);
                st.wMinute = (ushort)(day_millisec / (60 * 1000));
                day_millisec %= (60 * 1000);
                st.wSecond = (ushort)(day_millisec / (1000));
                day_millisec %= 1000;
                st.wMilliseconds = (ushort)day_millisec;
            }
        }

        /// <summary>
        /// CSV 분할 시 시간 정규화: cutMethod가 클수록 더 많은 하위 필드를 초기화
        /// 0=초, 1=분, 2=시, 3=일, 4=주, 5=월, 6=년
        /// </summary>
        static void NormalizeTimeForCutMethod(SYSTEMTIME time, int cutMethod)
        {
            time.wMilliseconds = 0;
            if (cutMethod >= 1) time.wSecond = 0;
            if (cutMethod >= 2) time.wMinute = 0;
            if (cutMethod >= 3) time.wHour = 0;    // 3=일, 4=주 모두 시간 초기화
            if (cutMethod >= 5) time.wDay = 1;
            if (cutMethod >= 6) time.wMonth = 1;
        }

        static async Task MakeHeaderAndField(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            item.bSavingFlag = 1;
            item.dwRecordCount = 0;
            item.nRemainedTime = 0;
            item.bDbOpenError = false;  // PostgreSQL 오류 플래그로 재활용

            SYSTEMTIME.memcpy(item.old_time, time);
            FitTimeWithCycle(item.nGab, item.old_time);

            SYSTEMTIME.memcpy(item.stStart, item.old_time);
            SYSTEMTIME.memcpy(item.stRecord, item.old_time);

            try
            {
                // PostgreSQL에 테이블 생성
                await CreateMilliDataTable(item).ConfigureAwait(false);

                // CSV 백업 파일 헤더 생성
                if (item.nSaveFileType == 1 || item.nSaveFileType == 2)
                {
                    MakeHeaderAndFieldCsv(item, time);
                }
            }
            catch (Exception ex)
            {
                MessageDisplay.Show("MilliData PostgreSQL 초기화 오류:Title={0}\nError Message={1}", item.title, ex.Message);
                item.bErrorFlag = true;
                item.bDbOpenError = true;  // PostgreSQL 오류 표시
                item.sErrorMsg = ex.Message;
            }
        }

        /// <summary>
        /// CSV 파일 분할 체크 (PostgreSQL은 분할하지 않음)
        /// </summary>
        static async Task CsvCuttingCheck(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            // 시간이 거꾸로 흘렀다면 저장 중단
            if (SYSTEMTIME.CompareTime(time, item.old_time) < 0)
            {
                return;
            }

            bool needNewFile = false;

            if (item.nCutMethod == 0) // 초
            {
                if ((time.wSecond % item.nSizeCut) == 0)
                {
                    if (time.wSecond != item.stStart.wSecond || time.wMinute != item.stStart.wMinute)
                    {
                        needNewFile = true;
                    }
                }
            }
            else if (item.nCutMethod == 1) // 분
            {
                if ((time.wMinute % item.nSizeCut) == 0)
                {
                    if (time.wMinute != item.stStart.wMinute || time.wHour != item.stStart.wHour)
                    {
                        needNewFile = true;
                    }
                }
            }
            else if (item.nCutMethod == 2) // 시
            {
                if ((time.wHour % item.nSizeCut) == 0)
                {
                    if (time.wHour != item.stStart.wHour || time.wDay != item.stStart.wDay)
                    {
                        needNewFile = true;
                    }
                }
            }
            else if (item.nCutMethod == 3) // 일
            {
                if (((time.wDay - 1) % item.nSizeCut) == 0)
                {
                    if (time.wDay != item.stStart.wDay || time.wMonth != item.stStart.wMonth)
                    {
                        needNewFile = true;
                    }
                }
            }
            else if (item.nCutMethod == 4) // 주
            {
                if (time.wDayOfWeek == 0) // 일요일
                {
                    if (time.wDay != item.stStart.wDay || time.wDayOfWeek != item.stStart.wDayOfWeek)
                    {
                        needNewFile = true;
                    }
                }
            }
            else if (item.nCutMethod == 5) // 월
            {
                if (((time.wMonth - 1) % item.nSizeCut) == 0)
                {
                    if (time.wMonth != item.stStart.wMonth || time.wYear != item.stStart.wYear)
                    {
                        needNewFile = true;
                    }
                }
            }
            else if (item.nCutMethod == 6) // 년
            {
                if (((time.wYear - 1) % item.nSizeCut) == 0)
                {
                    if (time.wYear != item.stStart.wYear)
                    {
                        needNewFile = true;
                    }
                }
            }

            if (needNewFile)
            {
                // 새로운 CSV 파일 생성
                SYSTEMTIME newTime = new SYSTEMTIME();
                SYSTEMTIME.memcpy(newTime, time);

                // 시간 정규화 (cutMethod가 클수록 더 많은 필드를 초기화)
                NormalizeTimeForCutMethod(newTime, item.nCutMethod);

                SYSTEMTIME.memcpy(item.stStart, newTime);
                MakeHeaderAndFieldCsv(item, newTime);
            }

            await RemainCheck(item, time).ConfigureAwait(false);
        }



        static async Task CreateMilliDataTable(MILLI_DATA_STRUCT item)
        {
            item.tableName = MakeTableName(item);

            try
            {
                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    // 단순화된 테이블 구조: data_time에 밀리초 포함
                    string createTableSql = $@"
                        CREATE TABLE IF NOT EXISTS ""{HISTORY_SCHEMA}"".""{item.tableName}"" (
                            data_time TIMESTAMPTZ NOT NULL,
                            interval_ms INTEGER NOT NULL,
                            millidata_config_id INTEGER,";

                    // 태그별 컬럼 추가
                    MILLI_DATA_TAG member;
                    for (int l = 0; l < item.blockTag.Count; l++)
                    {
                        member = (MILLI_DATA_TAG)item.blockTag[l];
                        string columnName = ConvertToValidColumnName(member.tag);

                        if (member.tag_type == EnumTagType.DI)
                        {
                            createTableSql += $"\n                            \"{columnName}\" INTEGER DEFAULT 0,";
                        }
                        else if (member.tag_type == EnumTagType.ST)
                        {
                            createTableSql += $"\n                            \"{columnName}\" TEXT DEFAULT '',";
                        }
                        else // AI 태그
                        {
                            createTableSql += $"\n                            \"{columnName}\" REAL DEFAULT 0,";
                        }
                    }

                    createTableSql += @"
                            created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                            PRIMARY KEY (data_time)
                        );";


                    using (var command = new NpgsqlCommand(createTableSql, connection))
                    {
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }

                    // 기존 테이블에 새 태그 컬럼 동기화 (태그 추가 대응)
                    await SyncTableColumns(connection, item).ConfigureAwait(false);

                    // Hypertable 변환
                    try
                    {
                        string hypertableSql = $@"
                            SELECT create_hypertable('""{HISTORY_SCHEMA}"".""{item.tableName}""', 'data_time',
                                chunk_time_interval => INTERVAL '1 day',
                                if_not_exists => TRUE);";

                        using (var command = new NpgsqlCommand(hypertableSql, connection))
                        {
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        // 압축 정책 7일
                        string compressionSql = $@"
                            ALTER TABLE ""{HISTORY_SCHEMA}"".""{item.tableName}"" SET (
                                timescaledb.compress,
                                timescaledb.compress_orderby = 'data_time DESC'
                            );";

                        using (var command = new NpgsqlCommand(compressionSql, connection))
                        {
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }

                        string compressionPolicySql = $@"
                            SELECT add_compression_policy('""{HISTORY_SCHEMA}"".""{item.tableName}""',
                                INTERVAL '7 days', if_not_exists => TRUE);";

                        using (var command = new NpgsqlCommand(compressionPolicySql, connection))
                        {
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Hypertable 생성 실패: {ex.Message}");
                    }

                    // 인덱스 생성
                    try
                    {
                        string indexSql = $@"
                            CREATE INDEX IF NOT EXISTS ""idx_{item.tableName}_config_id""
                            ON ""{HISTORY_SCHEMA}"".""{item.tableName}""(millidata_config_id);

                            CREATE INDEX IF NOT EXISTS ""idx_{item.tableName}_time_desc""
                            ON ""{HISTORY_SCHEMA}"".""{item.tableName}""(data_time DESC);";

                        using (var command = new NpgsqlCommand(indexSql, connection))
                        {
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"인덱스 생성 실패: {ex.Message}");
                    }

                    // 메타데이터 저장
                    await SaveMilliDataMetadata(connection, item).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"테이블 생성 오류: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 기존 테이블에 새로 추가된 태그의 컬럼이 없으면 ALTER TABLE ADD COLUMN으로 추가한다.
        /// 삭제된 태그의 컬럼은 절대 삭제하지 않는다 (기존 데이터 보존).
        /// </summary>
        static async Task SyncTableColumns(NpgsqlConnection connection, MILLI_DATA_STRUCT item)
        {
            // 1. information_schema.columns에서 기존 컬럼 목록 조회
            var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string queryColumns = @"
                SELECT column_name FROM information_schema.columns
                WHERE table_schema = @schema AND table_name = @tableName";

            using (var cmd = new NpgsqlCommand(queryColumns, connection))
            {
                cmd.Parameters.AddWithValue("schema", HISTORY_SCHEMA);
                cmd.Parameters.AddWithValue("tableName", item.tableName);
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                        existingColumns.Add(reader.GetString(0));
                }
            }

            if (existingColumns.Count == 0) return; // 방금 CREATE TABLE로 생성된 경우 → 동기화 불필요

            // 2. blockTag 순회하여 누락 컬럼 ADD
            for (int l = 0; l < item.blockTag.Count; l++)
            {
                MILLI_DATA_TAG member = (MILLI_DATA_TAG)item.blockTag[l];
                string columnName = ConvertToValidColumnName(member.tag);

                if (!existingColumns.Contains(columnName))
                {
                    string colType, defVal;
                    if (member.tag_type == EnumTagType.DI) { colType = "INTEGER"; defVal = "0"; }
                    else if (member.tag_type == EnumTagType.ST) { colType = "TEXT"; defVal = "''"; }
                    else { colType = "REAL"; defVal = "0"; }

                    try
                    {
                        string alterSql = $@"ALTER TABLE ""{HISTORY_SCHEMA}"".""{item.tableName}""
                            ADD COLUMN ""{columnName}"" {colType} DEFAULT {defVal}";
                        using (var alterCmd = new NpgsqlCommand(alterSql, connection))
                            await alterCmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                        Debug.WriteLine($"[MilliData] Added column '{columnName}' to '{item.tableName}'");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[MilliData] Failed to add column '{columnName}': {ex.Message}");
                    }
                }
            }
        }

        static async Task SaveMilliDataMetadata(NpgsqlConnection connection, MILLI_DATA_STRUCT item)
        {
            // history 스키마에 메타데이터 테이블이 이미 SchemaManager에 의해 생성되어 있음

            // 1. system 스키마에 millidata_config 저장
            string insertSystemConfigSql = $@"
                INSERT INTO system.millidata_config 
                (title, time_interval, cut_method, size_cut, condition_type, save_file_type, auto_delete, retention_days)
                VALUES (@title, @interval, @cutMethod, @sizeCut, @conditionType, @saveFileType, @autoDelete, @retentionDays)
                ON CONFLICT (id) DO NOTHING
                RETURNING id;";

            int configId = 0;
            using (var command = new NpgsqlCommand(insertSystemConfigSql, connection))
            {
                command.Parameters.AddWithValue("title", item.title ?? "");
                command.Parameters.AddWithValue("interval", item.nGab);
                command.Parameters.AddWithValue("cutMethod", item.nCutMethod);
                command.Parameters.AddWithValue("sizeCut", item.nSizeCut);
                command.Parameters.AddWithValue("conditionType", item.nCondition);
                command.Parameters.AddWithValue("saveFileType", item.nSaveFileType);
                command.Parameters.AddWithValue("autoDelete", item.bAutoDelete);
                command.Parameters.AddWithValue("retentionDays", item.nDaysOfAutoDelete);

                var result = await command.ExecuteScalarAsync().ConfigureAwait(false);
                if (result != null)
                {
                    configId = (int)result;
                    item.nMilliDataConfigId = configId;
                }
            }

            // 2. history 스키마에 메타데이터 저장
            string insertMetaSql = $@"
              INSERT INTO ""{HISTORY_SCHEMA}"".millidata_metadata
                (table_name, title, time_interval, start_time, cut_method, size_cut, condition_type, save_file_type, record_count, file_size_kb)
                VALUES (@tableName, @title, @interval, @startTime, @cutMethod, @sizeCut, @conditionType, @saveFileType, 0, 0)
                ON CONFLICT (table_name) DO UPDATE SET
                    title = EXCLUDED.title,
                    time_interval = EXCLUDED.time_interval,
                    start_time = EXCLUDED.start_time,
                    cut_method = EXCLUDED.cut_method,
                    size_cut = EXCLUDED.size_cut,
                    condition_type = EXCLUDED.condition_type,
                    save_file_type = EXCLUDED.save_file_type,
                    updated_at = CURRENT_TIMESTAMP;";

            using (var command = new NpgsqlCommand(insertMetaSql, connection))
            {
                command.Parameters.AddWithValue("tableName", item.tableName);
                command.Parameters.AddWithValue("title", item.title ?? "");
                command.Parameters.AddWithValue("interval", item.nGab);
                command.Parameters.AddWithValue("startTime", SystemTimeToDateTime(item.stStart));
                command.Parameters.AddWithValue("cutMethod", item.nCutMethod);
                command.Parameters.AddWithValue("sizeCut", item.nSizeCut);
                command.Parameters.AddWithValue("conditionType", item.nCondition);
                command.Parameters.AddWithValue("saveFileType", item.nSaveFileType);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            // 태그 메타데이터 저장
            await SaveTagMetadata(connection, item).ConfigureAwait(false);
        }

        /// <summary>
        /// 태그 메타데이터를 DELETE + INSERT 방식으로 갱신한다.
        /// 삭제된 태그의 메타데이터 행은 자동 제거되어 뷰어에서 표시되지 않는다.
        /// (데이터 테이블의 컬럼은 SyncTableColumns에서 ADD만 하며 절대 삭제하지 않음)
        /// </summary>
        static async Task SaveTagMetadata(NpgsqlConnection connection, MILLI_DATA_STRUCT item)
        {
            // 1. 해당 테이블의 기존 태그 메타데이터 전부 삭제
            string deleteSql = $@"DELETE FROM ""{HISTORY_SCHEMA}"".millidata_tag_metadata
                                  WHERE table_name = @tableName";
            using (var deleteCmd = new NpgsqlCommand(deleteSql, connection))
            {
                deleteCmd.Parameters.AddWithValue("tableName", item.tableName);
                await deleteCmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            // 2. 현재 blockTag의 태그만 INSERT (활성 태그만 메타데이터에 존재)
            for (int l = 0; l < item.blockTag.Count; l++)
            {
                MILLI_DATA_TAG member = (MILLI_DATA_TAG)item.blockTag[l];
                string columnName = ConvertToValidColumnName(member.tag);

                double fullScale = 0;
                double baseValue = 0;

                if (member.tag_type == EnumTagType.AI)
                {
                    TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.tag_pos);
                    fullScale = ai.fFull;
                    baseValue = ai.fBase;
                }
                else if (member.tag_type == EnumTagType.DI)
                {
                    fullScale = 100;
                    baseValue = 0;
                }

                string insertTagMetaSql = $@"
                    INSERT INTO ""{HISTORY_SCHEMA}"".millidata_tag_metadata
                    (table_name, tag_name, tag_type, full_scale, base_value, column_name)
                    VALUES (@tableName, @tagName, @tagType, @fullScale, @baseValue, @columnName)";

                using (var command = new NpgsqlCommand(insertTagMetaSql, connection))
                {
                    command.Parameters.AddWithValue("tableName", item.tableName);
                    command.Parameters.AddWithValue("tagName", member.tag);
                    command.Parameters.AddWithValue("tagType", (int)member.tag_type);
                    command.Parameters.AddWithValue("fullScale", fullScale);
                    command.Parameters.AddWithValue("baseValue", baseValue);
                    command.Parameters.AddWithValue("columnName", columnName);

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
        }


        static void MakeHeaderAndFieldCsv(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            try //250730 PSU try 밖으로 빼기.
            {
                // 기존 CSV 폴더 생성 로직 사용
                string dir;
                if (item.bUseCsvDateFolder)
                {
                    dir = MilliData.GetDataFolderCsv(item, time);
                }
                else if (item.bUseTargetFolder)
                {
                    dir = item.sTargetFolder;
                }
                else
                {
                    dir = MilliData.GetDataFolder(item);
                }

                Directory.CreateDirectory(dir);

                // 기존 파일명 생성 로직 사용 (MakeFileName 메서드와 동일)
                string filename = MakeFileNameFromSystemTime(item);
                item.filename_csv = String.Format("{0}\\{1}.csv", dir, filename);

                bool existed = File.Exists(item.filename_csv);

                // 파일이 없으면 헤더를 만든다
                if (!existed)
                {
                    using (TextWriter writer = new StreamWriter(item.filename_csv, true, System.Text.Encoding.Default))
                    {
                        // CSV 헤더: DataTime,Tag1,Tag2,...
                        writer.Write("DataTime,");

                        for (int l = 0; l < item.blockTag.Count; l++)
                        {
                            MILLI_DATA_TAG member = (MILLI_DATA_TAG)item.blockTag[l];
                            writer.Write("{0},", member.tag);
                        }

                        writer.WriteLine();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageDisplay.Show("MilliDataCSV Make Header Error:Title={0}\nError Message={1}", item.title, exception.Message);
                //item.bMdbOpenError = true;
            }
        }

        static string MakeFileNameFromSystemTime(MILLI_DATA_STRUCT item)
        {
            int year = item.stStart.wYear;
            int month = item.stStart.wMonth;
            int day = item.stStart.wDay;
            int hour = item.stStart.wHour;
            int minute = item.stStart.wMinute;
            int second = item.stStart.wSecond;

            string filename = String.Format("{0:0000}{1:00}{2:00}_{3:00}{4:00}{5:00}",
                year, month, day, hour, minute, second);

            if (item.bUseFilenameAddition)
            {
                TagStClass st = TagLib.GetStructST(item.sUseFilenameAdditionTag, ref item.tag_posUseFilenameAddition);

                if (item.tag_posUseFilenameAddition != null && item.tag_posUseFilenameAddition[0] != TagLib.TAG_NOT_FOUND)
                {
                    string curr = st.curr.Trim();
                    if (curr.Length > 0)
                        filename = filename + curr;
                }
            }

            return filename;
        }

        internal static string ConvertToValidColumnName(string tagName)
        {
            // PostgreSQL 컬럼명 규칙에 맞게 변환
            string result = tagName.ToLower()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace(".", "_")
                .Replace("(", "_")
                .Replace(")", "_")
                .Replace("[", "_")
                .Replace("]", "_")
                .Replace("/", "_")
                .Replace("\\", "_");

            // 숫자로 시작하면 접두사 추가
            if (char.IsDigit(result[0]))
            {
                result = "tag_" + result;
            }

            return result;
        }
        internal static DateTime SystemTimeToDateTime(SYSTEMTIME st)
        {
            try
            {
                // 로컬 시간을 UTC로 변환
                DateTime localTime = new DateTime(st.wYear, st.wMonth, st.wDay,
             st.wHour, st.wMinute, st.wSecond, st.wMilliseconds, DateTimeKind.Local);
                return localTime.ToUniversalTime();
            }
            catch
            {
                return DateTime.Now;
            }
        }


        static async Task RemainCheck(MILLI_DATA_STRUCT item, SYSTEMTIME time)
        {
            long curr = time.GetMilliSecHap() - item.old_time.GetMilliSecHap();

            // 1분이 넘었다. 너무 많은 시간 (1시간) 이 흐르면 데이터를 저장하지 않는다. 2013.1.17
            if(curr < 0 || curr > 3600000) 
            {
                item.bSavingFlag = 0;
                //item.DbClose();
                return;
            }

            // ✅ 지연 감지
            if (curr > item.nGab * 2)
            {
                int missedCount = (int)(curr / item.nGab) - 1;
                Debug.WriteLine($"[MilliData] {item.title}: {missedCount}개 데이터 누락 가능 (지연: {curr}ms)");
                SmLog.LogWarning(LogCategory.DATA_SAVE, $"MilliData '{item.title}': {curr}ms 지연으로 {missedCount}개 데이터 수집 누락");
            }

            SYSTEMTIME.memcpy(item.old_time, time);

            item.nRemainedTime += (int)curr;

            if (item.nRemainedTime >= item.nGab)
            {
                while (item.nRemainedTime >= item.nGab)
                {
                    //item.dwRecordCount++;

                    // PostgreSQL + CSV 모두 배치 처리!
                    if (_batchProcessor != null)
                    {
                        await _batchProcessor.AddItem(item).ConfigureAwait(false);
                    }

                    item.nRemainedTime -= item.nGab;
                    TimeUtil.AddMilliSecond(item.stRecord, item.nGab);
                    item.dwRecordCount++;
                }
            }
        }


        static void SaveRemainMilliData()
        {
            if (MilliData.blockMilliData.Count == 0) return;	// 미세 자료 감시가 없다.

            MILLI_DATA_STRUCT item;
            int l;

            for (l = 0; l < MilliData.blockMilliData.Count; l++)
            {
                item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[l];
                if (item.bErrorFlag) continue;   // 자료 저장에 문제가 있다.
                if (item.bSavingFlag == 0) continue;   // 자료 저장중이 아니다.
                item.bSavingFlag = 0;
            }
        }

        public static async Task CheckAutoDelete()
        {
            DateTime cutoffDate = DateTime.Now;

            for (int l = 0; l < MilliData.blockMilliData.Count; l++)
            {
                MILLI_DATA_STRUCT item = (MILLI_DATA_STRUCT)MilliData.blockMilliData[l];
                if (item.bAutoDelete == false) continue;

                await DeleteOldPostgresData(item, item.nDaysOfAutoDelete).ConfigureAwait(false);
                DeleteOldCsvData(item, cutoffDate, item.nDaysOfAutoDelete);
            }
        }

        static async Task DeleteOldPostgresData(MILLI_DATA_STRUCT item, int limitDays)
        {
            try
            {
                DateTime cutoffDate = DateTime.Now.AddDays(-limitDays);

                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string fullTableName = $"\"{HISTORY_SCHEMA}\".\"{item.tableName}\"";
                    string dropChunksQuery = $"SELECT drop_chunks('{fullTableName}', older_than => @cutoffDate);";

                    using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                    {
                        command.Parameters.AddWithValue("cutoffDate", cutoffDate);
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                        if (Tools.IsLangKorean())
                        {
                            SmLog.LogInfo(LogCategory.DATA_DELETE, $"[미세자료] {item.title} 저장 기한이 지난 레코드 삭제.");
                        }
                        else
                        {
                            SmLog.LogInfo(LogCategory.DATA_DELETE, $"[MilliData] {item.title} records are deleted because save date expired.");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                SmLog.LogError(LogCategory.DATA_DELETE, "[MilliData] Error: Can't delete old PostgreSQL data. {0}", ex.Message);
            }
        }

        static void DeleteOldCsvData(MILLI_DATA_STRUCT item, DateTime cutoffDate, int limit_day)
        {
            try
            {
                string path;

                // 폴더 경로 설정 로직 (기존과 동일)
                if (item.bUseCsvDateFolder)
                {
                    SYSTEMTIME currentTime = new SYSTEMTIME();
                    currentTime.GetLocalTime();
                    path = MilliData.GetDataFolderCsv(item, currentTime);
                }
                else if (item.bUseTargetFolder)
                {
                    path = item.sTargetFolder;
                }
                else
                {
                    path = MilliData.GetDataFolder(item);
                }

                if (!Directory.Exists(path)) return;

                DirectoryInfo info = new DirectoryInfo(path);

                foreach (FileInfo fi in info.GetFiles("*.csv"))
                {
                    try
                    {
                        if (fi.CreationTime < cutoffDate.AddDays(-limit_day))
                        {
                            File.Delete(fi.FullName);
                            if(Tools.IsLangKorean())
                                 SmLog.LogInfo(LogCategory.DATA_DELETE, "저장 기한이 지난 {0} 파일을 삭제.", fi.FullName);
                            else
                                SmLog.LogInfo( LogCategory.DATA_DELETE, "{0} file is deleted because save date expired.", fi.FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        SmLog.LogError(LogCategory.DATA_DELETE, "Error: Can't delete {0}. {1}", fi.FullName, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CSV 파일 자동 삭제 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// MilliData 조회를 위한 헬퍼 메서드들
        /// </summary>
        public static async Task<DataTable> GetMilliDataTable(string tableName, DateTime startTime, DateTime endTime)
        {
            var dt = new DataTable();

            try
            {
                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    // history 스키마에서 조회
                    string query = $@"
                        SELECT * FROM ""{HISTORY_SCHEMA}"".""{tableName}""
                        WHERE data_time BETWEEN @startTime AND @endTime
                        ORDER BY data_time";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("startTime", startTime);
                        command.Parameters.AddWithValue("endTime", endTime);

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MilliData 조회 오류: {ex.Message}");
            }

            return dt;
        }


    }


    /// <summary>
    /// MilliData용 배치 처리 클래스
    /// </summary>
    public class MilliDataBatchProcessor
    {
        private const string HISTORY_SCHEMA = "history";

        private readonly List<MilliDataBatchItem> _batchItems = new List<MilliDataBatchItem>();
        private readonly CsvBatchWriter _csvWriter;
        private readonly int _batchSize;
        private readonly Timer _flushTimer;
        private volatile bool _isDisposed = false;
        private volatile bool _isFlushInProgress = false;

        public MilliDataBatchProcessor(int batchSize = 1000, int flushIntervalMs = 5000)
        {
            _batchSize = batchSize;
            _csvWriter = new CsvBatchWriter(batchSize, flushIntervalMs);
            _flushTimer = new Timer(FlushBatchCallback, null, flushIntervalMs, flushIntervalMs);
        }


        public async Task AddItem(MILLI_DATA_STRUCT item)
        {
            // 현재 시점의 stRecord를 복사해서 저장
            SYSTEMTIME recordTimeCopy = new SYSTEMTIME();
            SYSTEMTIME.memcpy(recordTimeCopy, item.stRecord);

            lock (_batchItems)
            {
                _batchItems.Add(new MilliDataBatchItem
                {
                    Item = item,
                    Timestamp = DateTime.Now,
                    RecordTime = recordTimeCopy
                });

                if (item.nSaveFileType == 1 || item.nSaveFileType == 2)
                {
                    string csvRow = BuildCsvRow(item, recordTimeCopy);
                    _csvWriter.AddRow(item.filename_csv, csvRow);
                }
            }

            if (_batchItems.Count >= _batchSize)
            {
                await ProcessPostgresBatchAsync().ConfigureAwait(false);
            }
        }

        private string BuildCsvRow(MILLI_DATA_STRUCT item, SYSTEMTIME recordTime)
        {
            var sb = new StringBuilder();

            sb.Append($"{recordTime.wYear}-{recordTime.wMonth:00}-{recordTime.wDay:00} {recordTime.wHour:00}:{recordTime.wMinute:00}:{recordTime.wSecond:00}.{recordTime.wMilliseconds:000},");

            for (int l = 0; l < item.blockTag.Count; l++)
            {
                MILLI_DATA_TAG member = (MILLI_DATA_TAG)item.blockTag[l];

                if (member.tag_type == EnumTagType.AI)
                {
                    TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.tag_pos);
                    sb.Append($"{ai.curr},");
                }
                else if (member.tag_type == EnumTagType.DI)
                {
                    TagDiClass di = TagLib.GetStructDI(member.tag, ref member.tag_pos);
                    sb.Append($"{di.curr},");
                }
                else if (member.tag_type == EnumTagType.ST)
                {
                    TagStClass st = TagLib.GetStructST(member.tag, ref member.tag_pos);
                    string curr = st.curr ?? "";

                    if (CommaTextWriter.IsExistBlockCode(curr))
                        curr = CommaTextWriter.MakeString(curr);

                    sb.Append($"{curr},");
                }
                else
                {
                    sb.Append("0,");
                }
            }

            if (sb.Length > 0 && sb[sb.Length - 1] == ',')
                sb.Length--;

            return sb.ToString();
        }


        private async void FlushBatchCallback(object state)
        {
            if (_isDisposed || _isFlushInProgress) return;

            try
            {
                _isFlushInProgress = true;
                await ProcessPostgresBatchAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"배치 자동 플러시 오류: {ex.Message}");
                // Timer에서 발생한 예외는 로깅만 하고 앱 크래시 방지
            }
            finally
            {
                _isFlushInProgress = false;
            }
        }

        private async Task ProcessPostgresBatchAsync()
        {
            if (_isDisposed) return;

            List<MilliDataBatchItem> itemsToProcess;

            lock (_batchItems)
            {
                if (_isDisposed || _batchItems.Count == 0) return;

                itemsToProcess = new List<MilliDataBatchItem>(_batchItems);
                _batchItems.Clear();
            }

            if (itemsToProcess.Count == 0) return;

            try
            {
                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    using (var transaction = connection.BeginTransaction())
                    {
                        // PostgreSQL 저장 대상만 필터링   //x null 처리 추가 260219 미세자료 저장 구조 전체 재설계 필요.
                        var postgresItems = itemsToProcess
                      .Where(x => x?.Item != null &&
                                  (x.Item.nSaveFileType == 0 || x.Item.nSaveFileType == 1))
                      .ToList();

                        if (postgresItems.Count > 0)
                        {
                            // Multi-row INSERT 방식으로 일괄 저장
                            await SaveMilliDataRecordsBatch(connection, transaction, postgresItems)
                                .ConfigureAwait(false);
                        }

                        await transaction.CommitAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"배치 처리 오류: {ex.Message}");

                // 실패한 항목들을 다시 큐에 추가 (선택적)
                lock (_batchItems)
                {
                    if (!_isDisposed)
                    {
                        _batchItems.InsertRange(0, itemsToProcess);
                    }
                }
            }
        }

        private async Task SaveMilliDataRecordsBatch(NpgsqlConnection connection,
    NpgsqlTransaction transaction, List<MilliDataBatchItem> items)
        {
            if (items.Count == 0) return;

            // 같은 테이블의 항목들끼리 그룹화
            var groupedByTable = items.GroupBy(x => x.Item.tableName);

            foreach (var group in groupedByTable)
            {
                var tableItems = group.ToList();
                var firstItem = tableItems[0].Item;

                if (firstItem.bDbOpenError) continue;

                // 컬럼 정의 생성 (한 번만)
                string columns = "data_time, interval_ms, millidata_config_id";
                var columnNames = new List<string>();

                for (int l = 0; l < firstItem.blockTag.Count; l++)
                {
                    MILLI_DATA_TAG member = (MILLI_DATA_TAG)firstItem.blockTag[l];
                    string columnName = CheckEngineMilliData.ConvertToValidColumnName(member.tag);
                    columnNames.Add(columnName);
                    columns += $", \"{columnName}\"";
                }

                // Multi-row VALUES 생성
                var valuesList = new List<string>();
                var parameters = new List<NpgsqlParameter>();
                int paramIndex = 0;

                foreach (var batchItem in tableItems)
                {
                    DateTime dataTime = CheckEngineMilliData.SystemTimeToDateTime(batchItem.RecordTime);

                    string rowValues = $"@p{paramIndex}, @p{paramIndex + 1}, @p{paramIndex + 2}";
                    parameters.Add(new NpgsqlParameter($"p{paramIndex}", dataTime));
                    parameters.Add(new NpgsqlParameter($"p{paramIndex + 1}", batchItem.Item.nGab));
                    parameters.Add(new NpgsqlParameter($"p{paramIndex + 2}", batchItem.Item.nMilliDataConfigId));
                    paramIndex += 3;

                    for (int l = 0; l < batchItem.Item.blockTag.Count; l++)
                    {
                        MILLI_DATA_TAG member = (MILLI_DATA_TAG)batchItem.Item.blockTag[l];

                        if (member.tag_type == EnumTagType.AI)
                        {
                            TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.tag_pos);
                            parameters.Add(new NpgsqlParameter($"p{paramIndex}", NpgsqlDbType.Real)
                            { Value = (double)ai.curr });
                        }
                        else if (member.tag_type == EnumTagType.DI)
                        {
                            TagDiClass di = TagLib.GetStructDI(member.tag, ref member.tag_pos);
                            parameters.Add(new NpgsqlParameter($"p{paramIndex}", NpgsqlDbType.Integer)
                            { Value = di.curr });
                        }
                        else if (member.tag_type == EnumTagType.ST)
                        {
                            TagStClass st = TagLib.GetStructST(member.tag, ref member.tag_pos);
                            parameters.Add(new NpgsqlParameter($"p{paramIndex}", NpgsqlDbType.Text)
                            { Value = st.curr ?? "" });
                        }
                        else
                        {
                            parameters.Add(new NpgsqlParameter($"p{paramIndex}", NpgsqlDbType.Integer)
                            { Value = 0 });
                        }

                        rowValues += $", @p{paramIndex}";
                        paramIndex++;
                    }

                    valuesList.Add($"({rowValues})");
                }

                // UPDATE SET 구문
                var updateSet = columnNames.Select(col => $"\"{col}\" = EXCLUDED.\"{col}\"").ToList();

                string insertSql = $@"
            INSERT INTO ""{HISTORY_SCHEMA}"".""{firstItem.tableName}"" ({columns})
            VALUES {string.Join(", ", valuesList)}
            ON CONFLICT (data_time)
            DO UPDATE SET
                interval_ms = EXCLUDED.interval_ms,
                millidata_config_id = EXCLUDED.millidata_config_id,
                {string.Join(", ", updateSet)}";

                try
                {
                    using (var command = new NpgsqlCommand(insertSql, connection, transaction))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Multi-row 배치 저장 오류: {ex.Message}");
                    throw;
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;

            try
            {
                _flushTimer?.Dispose();
           

                // 남은 PostgreSQL 데이터 모두 저장 (최대 10초 대기)
                var flushTask = ProcessPostgresBatchAsync();
                if (!flushTask.Wait(10000))
                {
                    Debug.WriteLine("PostgreSQL 배치 플러시 타임아웃 발생");
                }

                _csvWriter?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MilliData 배치 처리기 해제 오류: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 배치 처리용 데이터 항목
    /// </summary>
    public class MilliDataBatchItem
    {
        public MILLI_DATA_STRUCT Item { get; set; }
        public DateTime Timestamp { get; set; }
        public SYSTEMTIME RecordTime { get; set; }
    }

    /// <summary>
    /// CSV 배치 처리를 위한 버퍼 클래스
    /// </summary>
    public class CsvBatchWriter : IDisposable
    {
        private readonly Dictionary<string, List<string>> _csvBuffers = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, DateTime> _lastFlush = new Dictionary<string, DateTime>();
        private readonly int _batchSize;
        private readonly int _flushIntervalMs;
        private readonly Timer _flushTimer;
        private readonly object _lockObject = new object();

        public CsvBatchWriter(int batchSize = 100, int flushIntervalMs = 2000)
        {
            _batchSize = batchSize;
            _flushIntervalMs = flushIntervalMs;
            _flushTimer = new Timer(FlushAllBuffers, null, flushIntervalMs, flushIntervalMs);
        }

        public void AddRow(string filePath, string csvRow)
        {
            lock (_lockObject)
            {
                if (!_csvBuffers.ContainsKey(filePath))
                {
                    _csvBuffers[filePath] = new List<string>();
                    _lastFlush[filePath] = DateTime.Now;
                }

                _csvBuffers[filePath].Add(csvRow);

                // 배치 크기에 도달하면 즉시 저장
                if (_csvBuffers[filePath].Count >= _batchSize)
                {
                    FlushBuffer(filePath);
                }
            }
        }

        private void FlushAllBuffers(object state)
        {
            lock (_lockObject)
            {
                var filesToFlush = new List<string>();
                var now = DateTime.Now;

                foreach (var kvp in _lastFlush)
                {
                    if ((now - kvp.Value).TotalMilliseconds >= _flushIntervalMs)
                    {
                        filesToFlush.Add(kvp.Key);
                    }
                }

                foreach (string filePath in filesToFlush)
                {
                    FlushBuffer(filePath);
                }
            }
        }

        private void FlushBuffer(string filePath)
        {
            if (!_csvBuffers.ContainsKey(filePath) || _csvBuffers[filePath].Count == 0)
                return;

            try
            {
                // 한 번에 모든 줄을 파일에 추가
                using (var writer = new StreamWriter(filePath, true, System.Text.Encoding.Default))
                {
                    foreach (string row in _csvBuffers[filePath])
                    {
                        writer.WriteLine(row);
                    }
                }

                _csvBuffers[filePath].Clear();
                _lastFlush[filePath] = DateTime.Now;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CSV 배치 저장 오류: {ex.Message}");
            }
        }

        public void FlushAll()
        {
            lock (_lockObject)
            {
                foreach (string filePath in _csvBuffers.Keys.ToList())
                {
                    FlushBuffer(filePath);
                }
            }
        }

        public void Dispose()
        {
            _flushTimer?.Dispose();
            FlushAll();
        }
    }

}

