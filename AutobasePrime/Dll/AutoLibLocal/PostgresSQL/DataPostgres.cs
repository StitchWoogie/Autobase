using AutoLibLocal.PostgresSQL;
using NetTools;
using NetTools.OldDefine;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace AutoLibLocal
{
    /*
     1. system 스키마 (시스템/설정)

        tags - 태그 마스터 정보 (tag_id, tag_name, description 등)
        users - 사용자 관리
        config - 시스템 설정
        millidata_config - MilliData 설정 정보

        2. operational 스키마 (실시간 SCADA 운영 데이터)

        minute_analog_data - 분별 아날로그 데이터 
        hour_analog_data - 시간별 아날로그 데이터 
        minute_digital_data - 분별 디지털 데이터 
        hour_digital_data - 시간별 디지털 데이터 
        alarms - 경보 데이터 
        logs - 로그 데이터 

        3. history 스키마 (대용량 이력 데이터)

        millidata_metadata - MilliData 테이블 메타정보
        millidata_tag_metadata - MilliData 태그 메타정보
        millidata_<millidata_Name> - 동적 생성되는 MilliData 테이블

        4.자동삭제 hypertable을 사용할 경우, delete가 아닌 chunk 단위로 삭제(drop_chucks) 해야한다.
     */


    #region ToDo
    /*
     
    - 프로젝트 생성 시 프로젝트 별 DB 생성 기능 추가. 프로젝트 생성 로직에 추가 필요.
       DB 생성 시 동일 DB명, 사용자 체크.
       
    - retention 자동삭제 처리 기간 설정 ConfigData 활용
    - 자동백업 기능. 주기적 백업. 원래는 자동삭제 시 zip파일로 백업 후 삭제 로직.

    - external DB 설정
    - postgresSQL 설치 방법 동시설치 / 별도설치

    - DB 이중화. 이중화 사용 시 처리. 현재 데이터 이중화 제대로 구현안되어있음. 이중화 시 데이터로드, 저장 등.
    */
    #endregion



    #region TagRepository DB 태그 id 캐싱
    public class TagRepository
    {
        private readonly string _connectionString;
        private readonly ConcurrentDictionary<string, int> _tagCache = new ConcurrentDictionary<string, int>();
        private readonly object _lock = new object();

        public TagRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> GetOrCreateTagIdAsync(string tagName, string description = "", int dataType = 0, float fullScale = 0)
        {
            // 1) 캐시에 있으면 바로 반환
            lock (_lock)
            {
                if (_tagCache.TryGetValue(tagName, out int cachedId))
                    return cachedId;
            }

            // 2) DB 조회 / 생성
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var selectCommand = new NpgsqlCommand(
                    "SELECT tag_id FROM system.tags WHERE tag_name = @tagName", connection))
                {
                    selectCommand.Parameters.AddWithValue("tagName", tagName);
                    object result = await selectCommand.ExecuteScalarAsync();
                    if (result != null)
                    {
                        int tagId = (int)result;
                        _tagCache[tagName] = tagId;
                        return tagId;
                    }
                }

                using (var insertCommand = new NpgsqlCommand(@"
                    INSERT INTO system.tags (tag_name, description, data_type, full_scale) 
                    VALUES (@tagName, @description, @dataType, @fullScale) 
                    RETURNING tag_id", connection))
                {
                    insertCommand.Parameters.AddWithValue("tagName", tagName);
                    insertCommand.Parameters.AddWithValue("description", description ?? "");
                    insertCommand.Parameters.AddWithValue("dataType", dataType);
                    insertCommand.Parameters.AddWithValue("fullScale", fullScale);

                    int newId = (int)await insertCommand.ExecuteScalarAsync();
                    lock (_lock) { _tagCache[tagName] = newId; }
                    return newId;
                }
            }
        }
    }
    #endregion


    /// <summary>
    /// 개선된 스키마 아키텍처 - 3개 스키마 분리
    /// </summary>
    public class SchemaArchitecture
    {
        // 1. 시스템/설정 스키마
        public const string SYSTEM_SCHEMA = "system";

        // 2. 운영 스키마 (실시간 SCADA 데이터)
        public const string OPERATIONAL_SCHEMA = "operational";

        // 3. 히스토리 스키마 (MilliData 등 대용량 이력)
        public const string HISTORY_SCHEMA = "history";
    }

    public class ConfigDataDB
    {
        public static string sPostgresHost;
        public static string sPostgresPort;
        public static string sPostgresUsername;
        public static string sPostgresPassword;
        public static string sPostgresDatabase;
        public static string sPostgresDatabasePassword;
       // public static string sOperationalDataRetentionDays; //자료유지기간은 자료설정 창에서 따로 처리.

        // 타임존 설정 추가
        public static string sPostgresTimezone;  //datetimez 는 utc 기준으로 저장, 데이터 READ 시 toLocalTime() 가 아닌 다른 시간대로 읽어오려면 필요할 수도 있어서 추가.

        // 단일 데이터베이스 연결 문자열
        public static string sConnectionString;

        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("AutoBaseDBpasswd"); // 16, 24, or 32 bytes
        private static readonly byte[] EncryptionIV = Encoding.UTF8.GetBytes("passwdDBAutoBase"); // 16 bytes

        /// <summary>
        /// .inix 파일에서 PostgreSQL 설정을 로드
        /// </summary>
        public static void LoadConfig()
        {
            // 현재 UI 스레드의 SynchronizationContext 저장
            SynchronizationContext originalContext = null;

            try
            {
                // --- Only WinForms: Save UI context ---
                if (AppEnvironment.IsWinForms())
                    originalContext = SynchronizationContext.Current;

                string dbConfigFile;

                if (AppEnvironment.IsAspNet())
                    dbConfigFile = String.Format("{0}\\Config\\pgDB.inix", TotalConfig.sDirWorkProject);// HttpContext.Current.Request.PhysicalApplicationPath + "AutoWeb\\Project");
                else dbConfigFile = String.Format("{0}\\Config\\pgDB.inix", TotalConfig.sDirWorkProject);

                // 기본값 설정
                SetDefaultValues();

                // 파일이 없으면 설정 폼 표시
                if (!File.Exists(dbConfigFile))
                {
                    HandleMissingConfig(dbConfigFile, originalContext);
                    BuildConnectionString();
                    return;
                }

                // 파일 읽기
                ReadConfigFile(dbConfigFile);

                // 연결 문자열 생성
                BuildConnectionString();
            }
            catch (Exception ex)
            {
                // 로그 기록 (TotalConfig.LogError 또는 다른 로깅 메커니즘 사용)
                Debug.WriteLine($"ConfigDataDB 로드 중 오류 발생: {ex.Message}");

                // 기본값으로 설정
                SetDefaultValues();
                BuildConnectionString();
            }
            finally
            {
                // --- Only WinForms: Restore UI context ---
                if (AppEnvironment.IsWinForms() && originalContext != null) { 
                    SynchronizationContext.SetSynchronizationContext(originalContext);
                }
            }
        }

        private static void HandleMissingConfig(string file, SynchronizationContext originalCtx)
        {
            // --- ASP.NET (IIS) ---
            if (AppEnvironment.IsAspNet())
            {
                CreateDefaultConfigFile(file);
                return;
            }

            // --- 콘솔/서비스 ---
            if (AppEnvironment.IsConsoleOrService())
            {
                CreateDefaultConfigFile(file);
                return;
            }

            // --- WinForms (유일하게 UI 호출 가능) ---
            if (AppEnvironment.IsWinForms())
            {
                bool ok = ShowConfigurationDialog();

                // WinForms에서 다이얼로그 실행 후 UI context 복원이 매우 중요
                // 다이얼로그 표시 후 SynchronizationContext 복원 >> static 클래스에서 UI 관련 호출 후 크로스스레드 오류 발생하여 추가.
                if (originalCtx != null)
                    SynchronizationContext.SetSynchronizationContext(originalCtx);

                if (!ok)
                {
                    // 사용자가 취소한 경우 기본값으로 진행
                    if (Tools.IsLangKorean())
                    {
                        MessageBox.Show(
                        "PostgreSQL 설정이 취소되었습니다.\n기본값으로 진행합니다.\n" +
                        "나중에 Config\\pgDB.inix 파일을 수정하거나 재설정할 수 있습니다.",
                        "설정 취소",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                        "PostgreSQL configuration was canceled.\nProceeding with default values.\n" +
                        "You can modify the Config\\pgDB.inix file or reconfigure later.",
                        "Configuration Canceled",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    }

                    CreateDefaultConfigFile(file);
                }
                else
                {
                    CreateDefaultConfigFile(file);
                }
            }
        }

        private static void ReadConfigFile(string configFile)
        {
            string[] lines = File.ReadAllLines(configFile, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) ||
                    line.StartsWith("[") ||
                    line.StartsWith("#"))
                    continue;

                var parts = line.Split(new char[] { '=' }, 2);
                if (parts.Length != 2) continue;

                string key = parts[0].Trim().ToLower();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "host": sPostgresHost = value; break;
                    case "port": sPostgresPort = value; break;
                    case "username": sPostgresUsername = value; break;
                    case "password": sPostgresPassword = DecryptPassword(value); break;
                    case "database": sPostgresDatabase = value; break;
                    case "timezone": sPostgresTimezone = value; break;
                }
            }
        }

        /// <summary>
        /// PostgreSQL 설정 다이얼로그 표시
        /// </summary>
        /// <returns>설정이 성공적으로 저장되었으면 true, 취소되었으면 false</returns>
        private static bool ShowConfigurationDialog()
        {
            try
            {
                using (PostgresConfigForm configForm = new PostgresConfigForm())
                {
                    DialogResult result = configForm.ShowDialog();

                    if (result == DialogResult.OK && configForm.ConfigSaved)
                    {
                        // 연결 문자열 재생성
                        BuildConnectionString();
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"설정 폼을 표시하는 중 오류가 발생했습니다:\n{ex.Message}",
                    "오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        /// <summary>
        /// 설정 다이얼로그를 수동으로 표시 (메뉴나 버튼에서 호출 가능)
        /// </summary>
        public static void ShowConfigurationUI()
        {
            try
            {
                // 기존 설정값을 로드하여 표시
                using (PostgresConfigForm configForm = new PostgresConfigForm(loadExisting: false))
                {
                    DialogResult result = configForm.ShowDialog();

                    if (result == DialogResult.OK && configForm.ConfigSaved)
                    {
                        // 연결 문자열 재생성
                        BuildConnectionString();

                        if (Tools.IsLangKorean())
                        {
                            MessageBox.Show(
                            "PostgreSQL 설정이 업데이트되었습니다.\n변경사항을 적용하려면 감시프로그램을 재시작하세요.",
                            "설정 완료",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(
                            "PostgreSQL configuration has been updated.\nPlease restart the LocalMain program to apply changes.",
                            "Configuration Updated",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show(
                    $"설정 변경 중 오류가 발생했습니다:\n{ex.Message}",
                    "오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(
                    $"An error occurred while changing the configuration:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 기본값 설정
        /// </summary>
        private static void SetDefaultValues()
        {
            sPostgresHost = "localhost";
            sPostgresPort = "5432";
            sPostgresUsername = "admin";
            sPostgresPassword = "admin";
            sPostgresDatabase = "autobase_db";
            sPostgresTimezone = "Asia/Seoul";  
            //sOperationalDataRetentionDays = "90";
        }

        /// <summary>
        /// 기본 설정 파일 생성
        /// </summary>
        private static void CreateDefaultConfigFile(string filePath)
        {
            try
            {
                string configDir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                // 기본 비밀번호 암호화
                string encryptedPassword = EncryptPassword(sPostgresPassword);

                StringBuilder configContent = new StringBuilder();
                configContent.AppendLine("[PostgreSQL Database Configuration]");
                configContent.AppendLine($"Host={sPostgresHost}");
                configContent.AppendLine($"Port={sPostgresPort}");
                configContent.AppendLine($"Username={sPostgresUsername}");
                configContent.AppendLine($"Password={encryptedPassword}");
                configContent.AppendLine($"Database={sPostgresDatabase}");
                configContent.AppendLine($"Timezone={sPostgresTimezone}");
                //configContent.AppendLine($"DataRetentionDays={sOperationalDataRetentionDays}");
                configContent.AppendLine($"# Created: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                File.WriteAllText(filePath, configContent.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"기본 설정 파일 생성 중 오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// 연결 문자열 생성
        /// </summary>
        private static void BuildConnectionString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"Host={sPostgresHost};");
            builder.Append($"Port={sPostgresPort};");
            builder.Append($"Username={sPostgresUsername};");
            builder.Append($"Password={sPostgresPassword};");
            builder.Append($"Database={sPostgresDatabase};");

            // 타임존이 설정된 경우만 추가
            if (!string.IsNullOrEmpty(sPostgresTimezone))
            {
                builder.Append($"Timezone={sPostgresTimezone};");
            }

            // 연결 타임아웃 설정 (옵션)
            builder.Append("Timeout=30;");
            builder.Append("CommandTimeout=30;");

           // builder.Append("Options=-c lc_messages=en_US.UTF8;");

            // lc_messages를 C로 설정 (영문 에러 메시지 사용)
            //builder.Append("Options=-c lc_messages=C;");

            sConnectionString = builder.ToString();
        }

        #region 암호화/복호화 메서드

        /// <summary>
        /// AES를 사용하여 비밀번호 암호화
        /// </summary>
        public static string EncryptPassword(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = EncryptionKey;
                    aes.IV = EncryptionIV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                            byte[] encrypted = msEncrypt.ToArray();
                            return Convert.ToBase64String(encrypted);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"비밀번호 암호화 중 오류 발생: {ex.Message}");
                return plainText; // 암호화 실패 시 원본 반환
            }
        }

        /// <summary>
        /// AES를 사용하여 비밀번호 복호화
        /// </summary>
        public static string DecryptPassword(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = EncryptionKey;
                    aes.IV = EncryptionIV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(buffer))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                // 복호화 실패 시 (암호화되지 않은 비밀번호일 수 있음)
                return cipherText;
            }
        }

        #endregion

    }
    public class DatabaseConfigManager
    {
        /// <summary>
        /// 애플리케이션 시작 시 호출
        /// </summary>
        public static async Task<bool> InitializeDatabase()
        {
            try
            {
                ConfigDataDB.LoadConfig();

                // 1. 데이터베이스 생성
                await EnsureDatabaseExists();

                // 2. 스키마 초기화
                var schemaManager = new SchemaManager();
                await schemaManager.InitializeAllSchemasAsync();
                Debug.WriteLine("모든 스키마 초기화 완료");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"데이터베이스 초기화 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 데이터베이스가 없으면 생성
        /// </summary>
        private static async Task EnsureDatabaseExists()
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder(ConfigDataDB.sConnectionString);
                string targetDatabase = builder.Database;
                builder.Database = "postgres"; // 기본 데이터베이스로 변경

                using (var connection = new NpgsqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (var checkCmd = new NpgsqlCommand(
                        $"SELECT 1 FROM pg_database WHERE datname = '{targetDatabase}'", connection))
                    {
                        var exists = await checkCmd.ExecuteScalarAsync();

                        if (exists == null)
                        {
                            Debug.WriteLine($"데이터베이스 '{targetDatabase}' 생성 중...");

                            using (var createCmd = new NpgsqlCommand(
                                $"CREATE DATABASE {targetDatabase}", connection))
                            {
                                await createCmd.ExecuteNonQueryAsync();                        
                            }

                            Debug.WriteLine($"데이터베이스 '{targetDatabase}' 생성 완료");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"데이터베이스 생성 오류: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 애플리케이션 종료 시 호출
        /// </summary>
        public static void ShutdownDatabase()
        {
            try
            {
                DataPostgres.Instance?.Dispose();
                Debug.WriteLine("데이터베이스 종료 완료");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"데이터베이스 종료 오류: {ex.Message}");
            }
        }
    }


    /// <summary>
    /// 스키마 관리자 클래스
    /// </summary>
    public class SchemaManager
    {
        private readonly string _connectionString;

        public SchemaManager()
        {
            _connectionString = ConfigDataDB.sConnectionString;
        }

        public async Task InitializeAllSchemasAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // 1. 스키마 생성
                await CreateSchemas(connection);
                Debug.WriteLine("스키마 생성 완료");

                // 2. TimescaleDB 확장 설치
                await SetupTimescaleDB(connection);
                Debug.WriteLine("TimescaleDB 설정 완료");

                // 3. 시스템 테이블 생성
                await CreateSystemTables(connection);
                Debug.WriteLine("시스템 테이블 생성 완료");

                // 4. 운영 테이블 생성
                await CreateOperationalTables(connection);
                Debug.WriteLine("운영 테이블 생성 완료");

                // 5. 히스토리 메타 테이블 생성
                await CreateHistoryMetaTables(connection);
                Debug.WriteLine("히스토리 메타 테이블 생성 완료");

                // 6. Hypertable 생성
                await CreateHypertables(connection);
                Debug.WriteLine("Hypertable 생성 완료");

                // 7. 인덱스 생성
                await CreateIndexes(connection);
                Debug.WriteLine("인덱스 생성 완료");

                // 8. 압축 및 보존 정책 설정
                await SetupPolicies(connection);
                Debug.WriteLine("정책 설정 완료");
            }
        }

        private async Task CreateSchemas(NpgsqlConnection connection)
        {
            string[] schemas = new string[]
            {
                $"CREATE SCHEMA IF NOT EXISTS {SchemaArchitecture.SYSTEM_SCHEMA};",
                $"CREATE SCHEMA IF NOT EXISTS {SchemaArchitecture.OPERATIONAL_SCHEMA};",
                $"CREATE SCHEMA IF NOT EXISTS {SchemaArchitecture.HISTORY_SCHEMA};"
            };

            foreach (string schemaScript in schemas)
            {
                using (var command = new NpgsqlCommand(schemaScript, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task SetupTimescaleDB(NpgsqlConnection connection)
        {
            try
            {
                using (var cmd = new NpgsqlCommand("CREATE EXTENSION IF NOT EXISTS timescaledb CASCADE;", connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TimescaleDB 확장 설치 실패: {ex.Message}");
            }
        }

        private async Task CreateSystemTables(NpgsqlConnection connection)
        {
            string[] systemTables = new string[]
            {
                // 태그 마스터 정보
                @"CREATE TABLE IF NOT EXISTS system.tags (
                    tag_id SERIAL PRIMARY KEY,
                    tag_name VARCHAR(100) NOT NULL UNIQUE,
                    description VARCHAR(200),
                    data_type SMALLINT NOT NULL,
                    full_scale REAL,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );",
                
                // 사용자 관리
                @"CREATE TABLE IF NOT EXISTS system.users (
                    id SERIAL PRIMARY KEY,
                    username VARCHAR(50) NOT NULL UNIQUE,
                    password_hash VARCHAR(256) NOT NULL,
                    password_mismatched_count INTEGER DEFAULT 0,
                    use_auto_lock BOOLEAN DEFAULT false,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );",
                
                // 시스템 설정
                @"CREATE TABLE IF NOT EXISTS system.config (
                    id SERIAL PRIMARY KEY,
                    config_group VARCHAR(50) NOT NULL,
                    config_key VARCHAR(100) NOT NULL,
                    config_value TEXT,
                    description VARCHAR(200),
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    UNIQUE(config_group, config_key)
                );",
                
                // MilliData 설정 정보
                @"CREATE TABLE IF NOT EXISTS system.millidata_config (
                    id SERIAL PRIMARY KEY,
                    title VARCHAR(200) NOT NULL,
                    time_interval INTEGER NOT NULL,
                    cut_method INTEGER NOT NULL,
                    size_cut INTEGER NOT NULL,
                    condition_type INTEGER NOT NULL,
                    save_file_type INTEGER NOT NULL,
                    auto_delete BOOLEAN DEFAULT false,
                    retention_days INTEGER DEFAULT 90,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );",

                // 레시피 마스터
                @"CREATE TABLE IF NOT EXISTS system.recipe (
                    recipe_id SERIAL PRIMARY KEY,
                    recipe_name VARCHAR(200) NOT NULL UNIQUE,
                    description VARCHAR(500),
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );",

                // ISA-88 Unit 계층 (Recipe → Unit → Step)
                @"CREATE TABLE IF NOT EXISTS system.recipe_unit (
                    unit_id SERIAL PRIMARY KEY,
                    recipe_id INT NOT NULL REFERENCES system.recipe(recipe_id) ON DELETE CASCADE,
                    unit_name VARCHAR(200) NOT NULL,
                    unit_order INT NOT NULL DEFAULT 0,
                    description VARCHAR(500),
                    UNIQUE(recipe_id, unit_name),
                    UNIQUE(recipe_id, unit_order)
                );",

                // 레시피 단계 (순차 실행)
                @"CREATE TABLE IF NOT EXISTS system.recipe_step (
                    step_id SERIAL PRIMARY KEY,
                    recipe_id INT NOT NULL REFERENCES system.recipe(recipe_id) ON DELETE CASCADE,
                    step_order INT NOT NULL,
                    step_name VARCHAR(200),
                    wait_time_ms INT DEFAULT 0,
                    timeout_ms INT DEFAULT 30000,
                    condition_tag VARCHAR(200),
                    condition_value VARCHAR(200),
                    condition_type VARCHAR(20) DEFAULT 'none',
                    UNIQUE(recipe_id, step_order)
                );",

                // 레시피 단계별 태그-값 쌍
                @"CREATE TABLE IF NOT EXISTS system.recipe_step_item (
                    item_id SERIAL PRIMARY KEY,
                    step_id INT NOT NULL REFERENCES system.recipe_step(step_id) ON DELETE CASCADE,
                    tag_name VARCHAR(200) NOT NULL,
                    set_value VARCHAR(200) NOT NULL,
                    value_type VARCHAR(20) DEFAULT 'double',
                    item_order INT DEFAULT 0,
                    UNIQUE(step_id, item_order)
                );",

                // 레시피 실행 로그 (INSERT 전용)
                @"CREATE TABLE IF NOT EXISTS history.recipe_execution_log (
                    log_id BIGSERIAL PRIMARY KEY,
                    recipe_id INT,
                    recipe_name VARCHAR(200),
                    unit_name VARCHAR(200),
                    action VARCHAR(20) NOT NULL,
                    status VARCHAR(20) NOT NULL,
                    step_index INT,
                    total_steps INT,
                    error_message TEXT,
                    username VARCHAR(50),
                    machine_name VARCHAR(100),
                    execution_start TIMESTAMPTZ,
                    execution_end TIMESTAMPTZ,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
                );",

                // 레시피 감사 로그 (INSERT 전용)
                @"CREATE TABLE IF NOT EXISTS history.recipe_audit_log (
                    log_id BIGSERIAL PRIMARY KEY,
                    recipe_id INT,
                    recipe_name VARCHAR(200),
                    action VARCHAR(20) NOT NULL,
                    target_type VARCHAR(30),
                    target_name VARCHAR(200),
                    old_value TEXT,
                    new_value TEXT,
                    username VARCHAR(50),
                    machine_name VARCHAR(100),
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
                );"
            };

            foreach (string tableScript in systemTables)
            {
                try
                {
                    using (var command = new NpgsqlCommand(tableScript, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"시스템 테이블 생성 오류: {ex.Message}");
                }
            }

            // updated_at 자동 갱신 트리거
            string[] recipeTriggers = new string[]
            {
                @"CREATE OR REPLACE FUNCTION system.update_recipe_timestamp()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW.updated_at = CURRENT_TIMESTAMP;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;",

                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'trg_recipe_updated_at') THEN
                        CREATE TRIGGER trg_recipe_updated_at BEFORE UPDATE ON system.recipe
                        FOR EACH ROW EXECUTE FUNCTION system.update_recipe_timestamp();
                    END IF;
                END $$;"
            };

            foreach (string triggerScript in recipeTriggers)
            {
                try
                {
                    using (var command = new NpgsqlCommand(triggerScript, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"레시피 트리거 생성 오류: {ex.Message}");
                }
            }

            // ISA-88 마이그레이션: 테이블/컬럼 생성 + 제약조건 추가 (개별 try-catch로 안전 실행)
            string[] recipeMigrations = new string[]
            {
                // 안전망: recipe_unit 테이블 (systemTables에서 누락되었을 경우 대비)
                @"CREATE TABLE IF NOT EXISTS system.recipe_unit (
                    unit_id SERIAL PRIMARY KEY,
                    recipe_id INT NOT NULL REFERENCES system.recipe(recipe_id) ON DELETE CASCADE,
                    unit_name VARCHAR(200) NOT NULL,
                    unit_order INT NOT NULL DEFAULT 0,
                    description VARCHAR(500),
                    UNIQUE(recipe_id, unit_name),
                    UNIQUE(recipe_id, unit_order)
                );",

                // 안전망: recipe_execution_log 테이블 (history 스키마)
                @"CREATE TABLE IF NOT EXISTS history.recipe_execution_log (
                    log_id BIGSERIAL PRIMARY KEY,
                    recipe_id INT,
                    recipe_name VARCHAR(200),
                    unit_name VARCHAR(200),
                    action VARCHAR(20) NOT NULL,
                    status VARCHAR(20) NOT NULL,
                    step_index INT,
                    total_steps INT,
                    error_message TEXT,
                    username VARCHAR(50),
                    machine_name VARCHAR(100),
                    execution_start TIMESTAMPTZ,
                    execution_end TIMESTAMPTZ,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
                );",

                // 안전망: recipe_audit_log 테이블 (history 스키마)
                @"CREATE TABLE IF NOT EXISTS history.recipe_audit_log (
                    log_id BIGSERIAL PRIMARY KEY,
                    recipe_id INT,
                    recipe_name VARCHAR(200),
                    action VARCHAR(20) NOT NULL,
                    target_type VARCHAR(30),
                    target_name VARCHAR(200),
                    old_value TEXT,
                    new_value TEXT,
                    username VARCHAR(50),
                    machine_name VARCHAR(100),
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
                );",

                // recipe_step에 unit_id FK 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='unit_id') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN unit_id INT
                            REFERENCES system.recipe_unit(unit_id) ON DELETE CASCADE;
                    END IF;
                END $$;",

                // recipe_step의 기존 UNIQUE(recipe_id, step_order) 제약조건 제거
                // → Unit별 step_order 중복 허용 필요 (서로 다른 Unit에 같은 step_order 가능)
                @"DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM pg_constraint
                        WHERE conname='recipe_step_recipe_id_step_order_key'
                        AND conrelid = 'system.recipe_step'::regclass) THEN
                        ALTER TABLE system.recipe_step
                            DROP CONSTRAINT recipe_step_recipe_id_step_order_key;
                    END IF;
                END $$;",

                // 새 UNIQUE 제약조건: (recipe_id, unit_id, step_order) — unit_id NULL 허용 (COALESCE 사용)
                @"CREATE UNIQUE INDEX IF NOT EXISTS uq_recipe_step_order
                    ON system.recipe_step (recipe_id, COALESCE(unit_id, 0), step_order);",

                // recipe_step_item에 중복 태그 방지 제약조건
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname='uq_step_item_tag') THEN
                        ALTER TABLE system.recipe_step_item
                            ADD CONSTRAINT uq_step_item_tag UNIQUE(step_id, tag_name);
                    END IF;
                END $$;",

                // 인덱스
                @"CREATE INDEX IF NOT EXISTS idx_recipe_unit_rid ON system.recipe_unit(recipe_id, unit_order);",
                @"CREATE INDEX IF NOT EXISTS idx_recipe_step_uid ON system.recipe_step(unit_id);",
                @"CREATE INDEX IF NOT EXISTS idx_exec_log_rid ON history.recipe_execution_log(recipe_id, created_at DESC);",
                @"CREATE INDEX IF NOT EXISTS idx_audit_log_rid ON history.recipe_audit_log(recipe_id, created_at DESC);",

                // 실행 로그: UPDATE/DELETE 금지 RULE
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE rulename='no_update_exec_log') THEN
                        CREATE RULE no_update_exec_log AS ON UPDATE TO history.recipe_execution_log DO INSTEAD NOTHING;
                    END IF;
                END $$;",
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE rulename='no_delete_exec_log') THEN
                        CREATE RULE no_delete_exec_log AS ON DELETE TO history.recipe_execution_log DO INSTEAD NOTHING;
                    END IF;
                END $$;",

                // 감사 로그: UPDATE/DELETE 금지 RULE
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE rulename='no_update_audit_log') THEN
                        CREATE RULE no_update_audit_log AS ON UPDATE TO history.recipe_audit_log DO INSTEAD NOTHING;
                    END IF;
                END $$;",
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE rulename='no_delete_audit_log') THEN
                        CREATE RULE no_delete_audit_log AS ON DELETE TO history.recipe_audit_log DO INSTEAD NOTHING;
                    END IF;
                END $$;",

                // ===== GUID 기반 레시피 형상관리 마이그레이션 =====

                // recipe_guid 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe' AND column_name='recipe_guid') THEN
                        ALTER TABLE system.recipe ADD COLUMN recipe_guid VARCHAR(32);
                    END IF;
                END $$;",

                // recipe_code 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe' AND column_name='recipe_code') THEN
                        ALTER TABLE system.recipe ADD COLUMN recipe_code VARCHAR(100);
                    END IF;
                END $$;",

                // version 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe' AND column_name='version') THEN
                        ALTER TABLE system.recipe ADD COLUMN version INT DEFAULT 1;
                    END IF;
                END $$;",

                // is_active 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe' AND column_name='is_active') THEN
                        ALTER TABLE system.recipe ADD COLUMN is_active BOOLEAN DEFAULT true;
                    END IF;
                END $$;",

                // recipe_name UNIQUE 제약조건을 활성 레코드 전용 partial index로 교체
                // (소프트삭제 시 동명 비활성 레코드 허용)
                @"DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM pg_constraint
                        WHERE conname='recipe_recipe_name_key'
                        AND conrelid = 'system.recipe'::regclass) THEN
                        ALTER TABLE system.recipe DROP CONSTRAINT recipe_recipe_name_key;
                    END IF;
                END $$;",
                // ISA-88 Revision: 같은 recipe_name + 다른 version 공존 허용
                @"DROP INDEX IF EXISTS system.uq_recipe_name_active;",
                @"CREATE UNIQUE INDEX IF NOT EXISTS uq_recipe_name_version_active
                    ON system.recipe(recipe_name, version)
                    WHERE is_active = true OR is_active IS NULL;",

                // ISA-88 Revision: 같은 recipe_guid + 다른 version 공존 허용
                @"DROP INDEX IF EXISTS system.uq_recipe_guid;",
                @"CREATE UNIQUE INDEX IF NOT EXISTS uq_recipe_guid_version
                    ON system.recipe(recipe_guid, version)
                    WHERE recipe_guid IS NOT NULL AND recipe_guid != ''
                    AND (is_active = true OR is_active IS NULL);",

                // 기존 레시피 GUID 자동 배정 (백필)
                @"UPDATE system.recipe SET recipe_guid = REPLACE(gen_random_uuid()::text, '-', '')
                    WHERE recipe_guid IS NULL OR recipe_guid = '';",

                // ===== ISA-88 Phase 1: 승인 상태 마이그레이션 =====

                // status 컬럼 추가 (draft / approved / obsolete)
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe' AND column_name='status') THEN
                        ALTER TABLE system.recipe ADD COLUMN status VARCHAR(20) DEFAULT 'draft';
                    END IF;
                END $$;",

                // approved_by 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe' AND column_name='approved_by') THEN
                        ALTER TABLE system.recipe ADD COLUMN approved_by VARCHAR(50);
                    END IF;
                END $$;",

                // approved_at 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe' AND column_name='approved_at') THEN
                        ALTER TABLE system.recipe ADD COLUMN approved_at TIMESTAMPTZ;
                    END IF;
                END $$;",

                // 기존 레시피 → 이미 사용 중이므로 'approved'로 백필
                @"UPDATE system.recipe SET status = 'approved'
                    WHERE (status IS NULL OR status = 'draft')
                    AND (is_active = true OR is_active IS NULL)
                    AND recipe_guid IS NOT NULL AND recipe_guid != '';",

                // ===== ISA-88 Phase 1: Step Entry Condition 마이그레이션 =====

                // entry_condition_tag 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='entry_condition_tag') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN entry_condition_tag VARCHAR(200);
                    END IF;
                END $$;",

                // entry_condition_value 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='entry_condition_value') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN entry_condition_value VARCHAR(200);
                    END IF;
                END $$;",

                // entry_condition_type 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='entry_condition_type') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN entry_condition_type VARCHAR(20) DEFAULT 'none';
                    END IF;
                END $$;",

                // entry_timeout_ms 컬럼 추가
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='entry_timeout_ms') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN entry_timeout_ms INT DEFAULT 0;
                    END IF;
                END $$;",

                // ===== ISA-88 Phase 2: Control Recipe + Batch Execution 마이그레이션 =====

                // Control Recipe 테이블 — Master 스냅샷 + 배치별 동결 레시피
                @"CREATE TABLE IF NOT EXISTS operational.control_recipe (
                    control_recipe_id SERIAL PRIMARY KEY,
                    master_recipe_id INT NOT NULL,
                    master_version INT NOT NULL,
                    batch_id VARCHAR(100) NOT NULL,
                    recipe_snapshot JSONB NOT NULL,
                    status VARCHAR(20) DEFAULT 'pending',
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    created_by VARCHAR(50)
                );",
                @"CREATE INDEX IF NOT EXISTS idx_ctrl_recipe_batch
                    ON operational.control_recipe(batch_id);",
                @"CREATE INDEX IF NOT EXISTS idx_ctrl_recipe_master
                    ON operational.control_recipe(master_recipe_id);",

                // Control Recipe DELETE 금지 RULE
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE rulename='no_delete_ctrl_recipe') THEN
                        CREATE RULE no_delete_ctrl_recipe AS ON DELETE TO operational.control_recipe DO INSTEAD NOTHING;
                    END IF;
                END $$;",

                // Batch Execution 테이블 — 배치 실행 기록 (감사/추적용)
                @"CREATE TABLE IF NOT EXISTS history.batch_execution (
                    batch_id VARCHAR(100) PRIMARY KEY,
                    control_recipe_id INT,
                    master_recipe_id INT,
                    master_recipe_name VARCHAR(200),
                    master_version INT,
                    operator_id VARCHAR(50),
                    start_time TIMESTAMPTZ,
                    end_time TIMESTAMPTZ,
                    result VARCHAR(20),
                    status VARCHAR(20) DEFAULT 'idle',
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
                );",

                // Batch Execution DELETE 금지 RULE
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE rulename='no_delete_batch_exec') THEN
                        CREATE RULE no_delete_batch_exec AS ON DELETE TO history.batch_execution DO INSTEAD NOTHING;
                    END IF;
                END $$;",

                // ===== ISA-88 Phase 4: Quick/Standard 모드 마이그레이션 =====

                // recipe_mode 컬럼 추가 (quick / standard)
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe' AND column_name='recipe_mode') THEN
                        ALTER TABLE system.recipe ADD COLUMN recipe_mode VARCHAR(20) DEFAULT 'standard';
                    END IF;
                END $$;",

                // 기존 레시피 → standard로 백필
                @"UPDATE system.recipe SET recipe_mode = 'standard' WHERE recipe_mode IS NULL;",

                // ===== ISA-88 Phase 6: audit_log 전자서명 컬럼 추가 =====

                // signature 컬럼 추가 (전자서명 문자열: "username|timestamp|action|reason")
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='history' AND table_name='recipe_audit_log' AND column_name='signature') THEN
                        ALTER TABLE history.recipe_audit_log ADD COLUMN signature VARCHAR(500);
                    END IF;
                END $$;",

                // reason 컬럼 추가 (전자서명 사유)
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='history' AND table_name='recipe_audit_log' AND column_name='reason') THEN
                        ALTER TABLE history.recipe_audit_log ADD COLUMN reason TEXT;
                    END IF;
                END $$;",

                // ===== Item 2: 스키마 분리 마이그레이션 =====
                // control_recipe → operational 스키마
                @"DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.tables
                        WHERE table_schema='system' AND table_name='control_recipe') THEN
                        ALTER TABLE system.control_recipe SET SCHEMA operational;
                    END IF;
                END $$;",

                // batch_execution → history 스키마
                @"DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.tables
                        WHERE table_schema='system' AND table_name='batch_execution') THEN
                        ALTER TABLE system.batch_execution SET SCHEMA history;
                    END IF;
                END $$;",

                // recipe_execution_log → history 스키마
                @"DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.tables
                        WHERE table_schema='system' AND table_name='recipe_execution_log') THEN
                        ALTER TABLE system.recipe_execution_log SET SCHEMA history;
                    END IF;
                END $$;",

                // recipe_audit_log → history 스키마
                @"DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM information_schema.tables
                        WHERE table_schema='system' AND table_name='recipe_audit_log') THEN
                        ALTER TABLE system.recipe_audit_log SET SCHEMA history;
                    END IF;
                END $$;",

                // ===== Item 5: 표현식 조건 컬럼 추가 =====
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='entry_expression') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN entry_expression TEXT DEFAULT '';
                    END IF;
                END $$;",
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='exit_expression') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN exit_expression TEXT DEFAULT '';
                    END IF;
                END $$;",

                // ===== Item 11: Step 실행 모델 확장 컬럼 추가 =====
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='running_expression') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN running_expression TEXT DEFAULT '';
                    END IF;
                END $$;",
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='exit_actions_json') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN exit_actions_json TEXT DEFAULT '';
                    END IF;
                END $$;",
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='abort_actions_json') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN abort_actions_json TEXT DEFAULT '';
                    END IF;
                END $$;",
                // ===== ISA-88 Full Transition Model: transitions_json 컬럼 추가 =====
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                        WHERE table_schema='system' AND table_name='recipe_step' AND column_name='transitions_json') THEN
                        ALTER TABLE system.recipe_step ADD COLUMN transitions_json TEXT DEFAULT '';
                    END IF;
                END $$;",

                // ===== ISA-88 Phase 3 개선: Transition 감사 로그 테이블 =====
                @"CREATE TABLE IF NOT EXISTS history.transition_execution_log (
                    log_id BIGSERIAL PRIMARY KEY,
                    batch_id VARCHAR(100),
                    step_order INT,
                    step_name VARCHAR(200),
                    transition_index INT,
                    transition_type VARCHAR(20) NOT NULL,
                    expression TEXT,
                    evaluated_result BOOLEAN NOT NULL,
                    action_taken VARCHAR(200),
                    unit_name VARCHAR(200),
                    error_message TEXT,
                    username VARCHAR(50),
                    machine_name VARCHAR(100),
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
                );",
                @"CREATE INDEX IF NOT EXISTS idx_transition_log_batch
                    ON history.transition_execution_log(batch_id, created_at DESC);",
                // Transition Log 변경/삭제 금지 RULE
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE rulename='no_update_transition_log') THEN
                        CREATE RULE no_update_transition_log AS ON UPDATE TO history.transition_execution_log DO INSTEAD NOTHING;
                    END IF;
                END $$;",
                @"DO $$ BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE rulename='no_delete_transition_log') THEN
                        CREATE RULE no_delete_transition_log AS ON DELETE TO history.transition_execution_log DO INSTEAD NOTHING;
                    END IF;
                END $$;"
            };

            foreach (string migrationScript in recipeMigrations)
            {
                try
                {
                    using (var command = new NpgsqlCommand(migrationScript, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"레시피 마이그레이션 오류: {ex.Message}");
                }
            }
        }

        private async Task CreateOperationalTables(NpgsqlConnection connection)
        {
            string[] operationalTables = new string[]
            {
                // 분별 아날로그 데이터
                @"CREATE TABLE IF NOT EXISTS operational.minute_analog_data (
                    tag_id INTEGER NOT NULL,
                    data_time TIMESTAMPTZ NOT NULL,
                    sum_min REAL NOT NULL DEFAULT 0,
                    average REAL NOT NULL DEFAULT 0,
                    min_value REAL NOT NULL DEFAULT 0,
                    max_value REAL NOT NULL DEFAULT 0,
                    curr_value REAL NOT NULL DEFAULT 0,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (tag_id, data_time)
                );",
                
                // 시간별 아날로그 데이터
                @"CREATE TABLE IF NOT EXISTS operational.hour_analog_data (
                    tag_id INTEGER NOT NULL,
                    data_time TIMESTAMPTZ NOT NULL,
                    sum_hour REAL NOT NULL DEFAULT 0,
                    avg_hour REAL NOT NULL DEFAULT 0,
                    min_hour REAL NOT NULL DEFAULT 0,
                    max_hour REAL NOT NULL DEFAULT 0,
                    curr_sum_meter REAL NOT NULL DEFAULT 0,
                    flag BOOLEAN DEFAULT true,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (tag_id, data_time)
                );",
                
                // 분별 디지털 데이터
                @"CREATE TABLE IF NOT EXISTS operational.minute_digital_data (
                    tag_id INTEGER NOT NULL,
                    data_time TIMESTAMPTZ NOT NULL,
                    count_on_off SMALLINT NOT NULL DEFAULT 0,
                    on_off_state BOOLEAN NOT NULL DEFAULT false,
                    on_time SMALLINT NOT NULL DEFAULT 0,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (tag_id, data_time)
                );",
                
                // 시간별 디지털 데이터
                @"CREATE TABLE IF NOT EXISTS operational.hour_digital_data (
                    tag_id INTEGER NOT NULL,
                    data_time TIMESTAMPTZ NOT NULL,
                    count_on_off INTEGER NOT NULL DEFAULT 0,
                    on_time INTEGER NOT NULL DEFAULT 0,
                    flag BOOLEAN DEFAULT true,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (tag_id, data_time)
                );",
                
               // 경보 데이터 (ID 기반으로 변경)
                   @"CREATE TABLE IF NOT EXISTS operational.alarms (
                    id BIGSERIAL,
                    alarm_datetime TIMESTAMPTZ NOT NULL,
                    tag_name VARCHAR(100) NOT NULL,
                    description VARCHAR(200),
                    message VARCHAR(200),
                    alarm_type INTEGER NOT NULL,           -- ushort → INTEGER
                    priority INTEGER NOT NULL,             -- ushort → INTEGER
                    port INTEGER NOT NULL,                 -- ushort → INTEGER
                    station INTEGER NOT NULL,              -- ushort → INTEGER
                    address BIGINT NOT NULL,               -- uint → BIGINT
                    sub_type INTEGER NOT NULL,             -- ushort → INTEGER
                    username VARCHAR(50),
                    ip_address VARCHAR(45),
                    computer_name VARCHAR(100),
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (id, alarm_datetime),
                    CONSTRAINT uk_alarm_time_tag_type UNIQUE (alarm_datetime, tag_name, alarm_type),
                    CONSTRAINT chk_alarm_type CHECK (alarm_type >= 0),
                    CONSTRAINT chk_priority CHECK (priority >= 0 AND priority <= 999),
                    CONSTRAINT chk_port CHECK (port >= 0),
                    CONSTRAINT chk_station CHECK (station >= 0),
                    CONSTRAINT chk_address CHECK (address >= 0),
                    CONSTRAINT chk_sub_type CHECK (sub_type >= 0)
                );",
                
                // 로그 데이터
                @"CREATE TABLE IF NOT EXISTS operational.logs (
                    id BIGSERIAL,
                    log_datetime TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    level SMALLINT NOT NULL DEFAULT 1,
                    category INTEGER NOT NULL DEFAULT 100,
                    message TEXT NOT NULL,
                    username VARCHAR(50),
                    ip_address VARCHAR(45),
                    machine_name VARCHAR(100),
                    detail JSONB,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (id, log_datetime),
                    CONSTRAINT chk_log_level CHECK (level BETWEEN 0 AND 5)
                );"
            };

            foreach (string tableScript in operationalTables)
            {
                using (var command = new NpgsqlCommand(tableScript, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }

            // 감사 로그 테이블 (전자서명 이력)
            string auditLogDdl = @"CREATE TABLE IF NOT EXISTS operational.audit_log (
                id BIGSERIAL PRIMARY KEY,
                event_time TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
                user_name TEXT,
                action TEXT NOT NULL,
                object_type TEXT,
                object_name TEXT,
                object_version INT DEFAULT 0,
                reason TEXT,
                result TEXT,
                source TEXT,
                client_ip TEXT,
                extra JSONB
            );";
            using (var cmd = new NpgsqlCommand(auditLogDdl, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 감사 로그 인덱스
            string auditLogIdx = @"CREATE INDEX IF NOT EXISTS idx_audit_log_event_time
                ON operational.audit_log(event_time DESC);";
            using (var cmd = new NpgsqlCommand(auditLogIdx, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // 감사 로그 수정/삭제 방지 (append-only)
            string noUpdateRule = @"DO $$ BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE tablename='audit_log' AND schemaname='operational' AND rulename='no_update_op_audit_log') THEN
                    CREATE RULE no_update_op_audit_log AS ON UPDATE TO operational.audit_log DO INSTEAD NOTHING;
                END IF;
            END $$;";
            using (var cmd = new NpgsqlCommand(noUpdateRule, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            string noDeleteRule = @"DO $$ BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_rules WHERE tablename='audit_log' AND schemaname='operational' AND rulename='no_delete_op_audit_log') THEN
                    CREATE RULE no_delete_op_audit_log AS ON DELETE TO operational.audit_log DO INSTEAD NOTHING;
                END IF;
            END $$;";
            using (var cmd = new NpgsqlCommand(noDeleteRule, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateHistoryMetaTables(NpgsqlConnection connection)
        {
            string[] metaTables = new string[]
            {
                // MilliData 테이블 메타정보
                @"CREATE TABLE IF NOT EXISTS history.millidata_metadata (
                    table_name VARCHAR(100) PRIMARY KEY,
                    title VARCHAR(200),
                    time_interval INTEGER,
                    start_time TIMESTAMPTZ,
                    end_time TIMESTAMPTZ,
                    cut_method INTEGER,
                    size_cut INTEGER,
                    condition_type INTEGER,
                    save_file_type INTEGER,
                    record_count BIGINT DEFAULT 0,
                    file_size_kb BIGINT DEFAULT 0,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
                );",
                
                // MilliData 태그 메타정보
                @"CREATE TABLE IF NOT EXISTS history.millidata_tag_metadata (
                    table_name VARCHAR(100),
                    tag_name VARCHAR(100),
                    tag_type INTEGER,
                    full_scale REAL DEFAULT 0,
                    base_value REAL DEFAULT 0,
                    column_name VARCHAR(100),
                    PRIMARY KEY (table_name, tag_name)
                );",

                //로그 메타데이터 테이블
                @"CREATE TABLE IF NOT EXISTS operational.log_metadata (
                    id SERIAL PRIMARY KEY,
                    log_date DATE NOT NULL UNIQUE,
                    record_count INTEGER DEFAULT 0,
                    file_size_kb INTEGER DEFAULT 0,
                    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
                );"
            };

            foreach (string tableScript in metaTables)
            {
                using (var command = new NpgsqlCommand(tableScript, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task CreateHypertables(NpgsqlConnection connection)
        {
            string[] hypertableQueries = new string[]
            {
                @"SELECT create_hypertable('operational.minute_analog_data', 'data_time', 
                    chunk_time_interval => INTERVAL '1 day', 
                    if_not_exists => TRUE,
                    migrate_data => TRUE);",

                @"SELECT create_hypertable('operational.hour_analog_data', 'data_time', 
                    chunk_time_interval => INTERVAL '7 days', 
                    if_not_exists => TRUE,
                    migrate_data => TRUE);",

                @"SELECT create_hypertable('operational.minute_digital_data', 'data_time', 
                    chunk_time_interval => INTERVAL '1 day', 
                    if_not_exists => TRUE,
                    migrate_data => TRUE);",

                @"SELECT create_hypertable('operational.hour_digital_data', 'data_time', 
                    chunk_time_interval => INTERVAL '7 days', 
                    if_not_exists => TRUE,
                    migrate_data => TRUE);",

                @"SELECT create_hypertable('operational.alarms', 'alarm_datetime', 
                    chunk_time_interval => INTERVAL '7 days', 
                    if_not_exists => TRUE,
                    migrate_data => TRUE);",

                @"SELECT create_hypertable('operational.logs', 'log_datetime', 
                    chunk_time_interval => INTERVAL '7 days', 
                    if_not_exists => TRUE,
                    migrate_data => TRUE);"
            };

            foreach (string query in hypertableQueries)
            {
                try
                {
                    // Hypertable인지 먼저 확인
                    string checkQuery = query.Contains("minute_analog_data") ?
                        "SELECT * FROM timescaledb_information.hypertables WHERE hypertable_name = 'minute_analog_data' AND hypertable_schema = 'operational';" :
                        query.Contains("hour_analog_data") ?
                        "SELECT * FROM timescaledb_information.hypertables WHERE hypertable_name = 'hour_analog_data' AND hypertable_schema = 'operational';" :
                        query.Contains("minute_digital_data") ?
                        "SELECT * FROM timescaledb_information.hypertables WHERE hypertable_name = 'minute_digital_data' AND hypertable_schema = 'operational';" :
                        query.Contains("hour_digital_data") ?
                        "SELECT * FROM timescaledb_information.hypertables WHERE hypertable_name = 'hour_digital_data' AND hypertable_schema = 'operational';" :
                        query.Contains("alarms") ?
                        "SELECT * FROM timescaledb_information.hypertables WHERE hypertable_name = 'alarms' AND hypertable_schema = 'operational';" :
                        "SELECT * FROM timescaledb_information.hypertables WHERE hypertable_name = 'logs' AND hypertable_schema = 'operational';";

                    bool isHypertable = false;
                    using (var checkCmd = new NpgsqlCommand(checkQuery, connection))
                    using (var reader = await checkCmd.ExecuteReaderAsync())
                    {
                        isHypertable = await reader.ReadAsync();
                    }

                    if (!isHypertable)
                    {
                        using (var command = new NpgsqlCommand(query, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                        Debug.WriteLine($"Hypertable 생성 성공: {query.Substring(0, Math.Min(50, query.Length))}");
                    }
                    else
                    {
                        Debug.WriteLine($"이미 Hypertable로 설정됨: {checkQuery.Split('\'')[1]}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Hypertable 생성 실패: {ex.Message}");
                    Debug.WriteLine($"쿼리: {query}");
                }
            }
        }

        private async Task CreateIndexes(NpgsqlConnection connection)
        {
            string[] indexes = new string[]
            {
                 // 데이터 테이블 인덱스
                "CREATE INDEX IF NOT EXISTS idx_minute_analog_tag_time ON operational.minute_analog_data(tag_id, data_time);",
                "CREATE INDEX IF NOT EXISTS idx_hour_analog_tag_time ON operational.hour_analog_data(tag_id, data_time);",
                "CREATE INDEX IF NOT EXISTS idx_minute_digital_tag_time ON operational.minute_digital_data(tag_id, data_time);",
                "CREATE INDEX IF NOT EXISTS idx_hour_digital_tag_time ON operational.hour_digital_data(tag_id, data_time);",

                 // 경보 인덱스 
                "CREATE INDEX IF NOT EXISTS idx_alarms_datetime ON operational.alarms(alarm_datetime DESC);",
                "CREATE INDEX IF NOT EXISTS idx_alarms_tag_datetime ON operational.alarms(tag_name, alarm_datetime DESC);",
                "CREATE INDEX IF NOT EXISTS idx_alarms_priority_datetime ON operational.alarms(priority, alarm_datetime DESC);",
                "CREATE INDEX IF NOT EXISTS idx_alarms_type_datetime ON operational.alarms(alarm_type, alarm_datetime DESC);",
                "CREATE INDEX IF NOT EXISTS idx_alarms_id_datetime ON operational.alarms(id, alarm_datetime DESC);",

                 // 로그 인덱스 
                "CREATE INDEX IF NOT EXISTS idx_logs_datetime ON operational.logs(log_datetime DESC);",
                "CREATE INDEX IF NOT EXISTS idx_logs_level ON operational.logs(level);",
                "CREATE INDEX IF NOT EXISTS idx_logs_category ON operational.logs(category);",
                "CREATE INDEX IF NOT EXISTS idx_logs_level_datetime ON operational.logs(level, log_datetime DESC);",
                "CREATE INDEX IF NOT EXISTS idx_logs_category_datetime ON operational.logs(category, log_datetime DESC);",
                "CREATE INDEX IF NOT EXISTS idx_logs_id_datetime ON operational.logs(id, log_datetime DESC);",

                  // 시스템 테이블 인덱스
                "CREATE INDEX IF NOT EXISTS idx_tags_name ON system.tags(tag_name);",

                 // 내림차순 인덱스
                "CREATE INDEX IF NOT EXISTS idx_minute_analog_tag_time_desc ON operational.minute_analog_data(tag_id, data_time DESC);",
                "CREATE INDEX IF NOT EXISTS idx_hour_analog_tag_time_desc ON operational.hour_analog_data(tag_id, data_time DESC);",
                "CREATE INDEX IF NOT EXISTS idx_minute_digital_tag_time_desc ON operational.minute_digital_data(tag_id, data_time DESC);",
                "CREATE INDEX IF NOT EXISTS idx_hour_digital_tag_time_desc ON operational.hour_digital_data(tag_id, data_time DESC);",
        
                // 추가: MilliData 메타데이터 인덱스
                "CREATE INDEX IF NOT EXISTS idx_millidata_metadata_start_time ON history.millidata_metadata(start_time);",
                "CREATE INDEX IF NOT EXISTS idx_millidata_metadata_table_name ON history.millidata_metadata(table_name);",
                "CREATE INDEX IF NOT EXISTS idx_millidata_tag_metadata_table_tag ON history.millidata_tag_metadata(table_name, tag_name);",

                // 레시피 인덱스
                "CREATE INDEX IF NOT EXISTS idx_recipe_step_recipe_id ON system.recipe_step(recipe_id, step_order);",
                "CREATE INDEX IF NOT EXISTS idx_recipe_step_item_step_id ON system.recipe_step_item(step_id, item_order);"
            };

            foreach (string indexScript in indexes)
            {
                try
                {
                    using (var command = new NpgsqlCommand(indexScript, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"인덱스 생성 오류: {ex.Message}");
                }
            }
        }

        private async Task SetupPolicies(NpgsqlConnection connection)
        {
            var compressionSettings = new[]
            {
                new { Table = "operational.minute_analog_data", SegmentBy = "tag_id", OrderBy = "data_time DESC", CompressAfter = "7 days" },
                new { Table = "operational.hour_analog_data", SegmentBy = "tag_id", OrderBy = "data_time DESC", CompressAfter = "7 days" },
                new { Table = "operational.minute_digital_data", SegmentBy = "tag_id", OrderBy = "data_time DESC", CompressAfter = "7 days" },
                new { Table = "operational.hour_digital_data", SegmentBy = "tag_id", OrderBy = "data_time DESC", CompressAfter = "7 days" },
                new { Table = "operational.alarms", SegmentBy = "tag_name, alarm_type", OrderBy = "alarm_datetime DESC, id DESC", CompressAfter = "30 days" },
                new { Table = "operational.logs", SegmentBy = "category", OrderBy = "log_datetime DESC, id DESC", CompressAfter = "30 days" }
            };

            foreach (var setting in compressionSettings)
            {
                try
                {
                    // 1. 먼저 Hypertable인지 확인
                    string checkHypertableQuery = $@"
                SELECT COUNT(*) 
                FROM timescaledb_information.hypertables 
                WHERE format('%I.%I', hypertable_schema, hypertable_name) = '{setting.Table}';";

                    bool isHypertable = false;
                    using (var checkCmd = new NpgsqlCommand(checkHypertableQuery, connection))
                    {
                        var result = await checkCmd.ExecuteScalarAsync();
                        isHypertable = Convert.ToInt32(result) > 0;
                    }

                    if (!isHypertable)
                    {
                        Debug.WriteLine($"압축 정책 스킵 (Hypertable 아님): {setting.Table}");
                        continue;
                    }

                    // 2. 기존 압축 설정 확인
                    string checkCompressionQuery = $@"
                SELECT COUNT(*) 
                FROM timescaledb_information.compression_settings 
                WHERE format('%I.%I', hypertable_schema, hypertable_name) = '{setting.Table}';";

                    bool hasCompression = false;
                    using (var checkCmd = new NpgsqlCommand(checkCompressionQuery, connection))
                    {
                        var result = await checkCmd.ExecuteScalarAsync();
                        hasCompression = Convert.ToInt32(result) > 0;
                    }

                    // 3. 압축 설정이 없으면 추가
                    if (!hasCompression)
                    {
                        string alterTableQuery = $@"
                    ALTER TABLE {setting.Table} SET (
                        timescaledb.compress,
                        timescaledb.compress_segmentby = '{setting.SegmentBy}',
                        timescaledb.compress_orderby = '{setting.OrderBy}'
                    );";

                        using (var command = new NpgsqlCommand(alterTableQuery, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                        Debug.WriteLine($"압축 설정 완료: {setting.Table}");
                    }
                    else
                    {
                        Debug.WriteLine($"이미 압축 설정됨: {setting.Table}");
                    }

                    // 4. 압축 정책 확인 및 추가
                    string checkPolicyQuery = $@"
                SELECT COUNT(*) 
                FROM timescaledb_information.jobs 
                WHERE hypertable_name = '{setting.Table.Split('.')[1]}' 
                AND proc_name = 'policy_compression';";

                    bool hasPolicy = false;
                    using (var checkCmd = new NpgsqlCommand(checkPolicyQuery, connection))
                    {
                        var result = await checkCmd.ExecuteScalarAsync();
                        hasPolicy = Convert.ToInt32(result) > 0;
                    }

                    if (!hasPolicy)
                    {
                        string addPolicyQuery = $@"
                    SELECT add_compression_policy('{setting.Table}', 
                        INTERVAL '{setting.CompressAfter}', 
                        if_not_exists => TRUE);";

                        using (var command = new NpgsqlCommand(addPolicyQuery, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                        Debug.WriteLine($"압축 정책 추가 완료: {setting.Table}");
                    }
                    else
                    {
                        Debug.WriteLine($"이미 압축 정책 존재: {setting.Table}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"압축 정책 설정 오류 ({setting.Table}): {ex.Message}");
                }
            }
        }

        // MilliData 테이블 동적 생성 (history 스키마)
        public async Task CreateMilliDataTable(string tableName, List<MilliDataTagInfo> tags)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = new StringBuilder();
                sql.AppendLine($"CREATE TABLE IF NOT EXISTS history.{tableName} (");
                sql.AppendLine("    data_time TIMESTAMPTZ NOT NULL,");
                sql.AppendLine("    data_count INTEGER NOT NULL,");
                sql.AppendLine("    millisec INTEGER NOT NULL,");
                sql.AppendLine("    interval_ms INTEGER NOT NULL,");
                sql.AppendLine("    start_time TIMESTAMPTZ NOT NULL,");

                foreach (var tag in tags)
                {
                    string columnName = ConvertToValidColumnName(tag.TagName);
                    string dataType = tag.TagType == 1 ? "INTEGER" : (tag.TagType == 9 ? "TEXT" : "REAL");
                    sql.AppendLine($"    {columnName} {dataType} DEFAULT {(dataType == "TEXT" ? "''" : "0")},");
                }

                sql.AppendLine("    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,");
                sql.AppendLine("    PRIMARY KEY (data_time, data_count)");
                sql.AppendLine(");");

                using (var command = new NpgsqlCommand(sql.ToString(), connection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                // Hypertable로 변환
                try
                {
                    string hypertableSql = $@"
                    SELECT create_hypertable('history.{tableName}', 'data_time', 
                        chunk_time_interval => INTERVAL '1 hour',
                        if_not_exists => TRUE);";

                    using (var command = new NpgsqlCommand(hypertableSql, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }


                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"MilliData Hypertable 생성 실패: {ex.Message}");
                }
            }
        }

        private string ConvertToValidColumnName(string tagName)
        {
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

            if (char.IsDigit(result[0]))
            {
                result = "tag_" + result;
            }

            return result;
        }
    }

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


    public partial class DataPostgres : IDisposable
    {
        #region Private Fields
        private string _connectionString;
        private readonly object _lockObject = new object();
        private static DataPostgres _instance;
        private static readonly object _instanceLock = new object();

        private Dictionary<string, NpgsqlConnection> _connectionPool = new Dictionary<string, NpgsqlConnection>();
        //private readonly int _maxPoolSize = 10;

        private System.Timers.Timer _autoDeleteTimer;
        private int _dataRetentionDays = 90;
        private readonly TagRepository _tagRepo;
        #endregion

        #region Constructor & Singleton
        private DataPostgres()
        {
            InitializeConnectionString();
            _tagRepo = new TagRepository(ConfigDataDB.sConnectionString);
           // StartAutoDeleteTimer();
        }

        public static DataPostgres Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                            _instance = new DataPostgres();
                    }
                }
                return _instance;
            }
        }
        #endregion


        #region Initialization
        private void InitializeConnectionString()
        {
            ConfigDataDB.LoadConfig();
            _connectionString = ConfigDataDB.sConnectionString;
        }

        #endregion

        #region Time Normalization Helpers

        /// <summary>
        /// 분 단위로 시간을 정규화 (초, 밀리초 제거)
        /// </summary>
        private DateTime NormalizeToMinute(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                               dateTime.Hour, dateTime.Minute, 0);
        }

        /// <summary>
        /// 시간 단위로 시간을 정규화 (분, 초, 밀리초 제거)
        /// </summary>
        private DateTime NormalizeToHour(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                               dateTime.Hour, 0, 0);
        }

        #endregion

        #region Tag Management
        public async Task<int> GetOrCreateTagId(string tagName, string description = "",
        int dataType = 0, float fullScale = 0)
        {
            // TagRepository를 통해 System 스키마에서 tag_id 조회/생성
            return await _tagRepo.GetOrCreateTagIdAsync(tagName, description, dataType, fullScale);
        }

        public async Task<DataSet> GetTagValueList(ArrayList tagList)
        {
            DataSet ds = new DataSet("TAG");
            DataTable dt = new DataTable("TAG");

            dt.Columns.Add("tag", typeof(string));
            dt.Columns.Add("curr", typeof(string));

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (string tag in tagList)
                {
                    DataRow row = dt.NewRow();
                    row["tag"] = tag;

                    // TagRepository로 tag_id 조회
                    int tagId = await _tagRepo.GetOrCreateTagIdAsync(tag);

                    using (var command = new NpgsqlCommand(@"
                    SELECT curr_value 
                    FROM operational.minute_analog_data
                    WHERE tag_id = @tagId 
                    ORDER BY data_time DESC 
                    LIMIT 1", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        object result = await command.ExecuteScalarAsync();

                        row["curr"] = result?.ToString() ?? "";
                        dt.Rows.Add(row);
                    }
                }
            }

            ds.Tables.Add(dt);
            return ds;
        }
        #endregion

        #region Analog Data Methods
        public async Task<bool> SaveMinDataAI(string tagName, DateTime dataTime, TREND_AI_STRUCT data)
        {
            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                // 시간을 분 단위로 정규화
                DateTime normalizedTime = NormalizeToMinute(dataTime);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                        INSERT INTO operational.minute_analog_data 
                        (tag_id, data_time, sum_min, average, min_value, max_value, curr_value)
                        VALUES (@tagId, @dataTime, @sumMin, @average, @minValue, @maxValue, @currValue)
                        ON CONFLICT (tag_id, data_time) DO UPDATE SET
                            sum_min = EXCLUDED.sum_min,
                            average = EXCLUDED.average,
                            min_value = EXCLUDED.min_value,
                            max_value = EXCLUDED.max_value,
                            curr_value = EXCLUDED.curr_value", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("dataTime", normalizedTime);
                        command.Parameters.AddWithValue("sumMin", data.fSumMin);
                        command.Parameters.AddWithValue("average", data.fAverage);
                        command.Parameters.AddWithValue("minValue", data.fMin);
                        command.Parameters.AddWithValue("maxValue", data.fMax);
                        command.Parameters.AddWithValue("currValue", data.fCurr);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveMinDataAI 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 특정 시간대의 모든 분 데이터를 한 번에 조회
        /// </summary>
        public async Task<List<TREND_AI_STRUCT>> LoadHourMinuteDataAI(string tagName, DateTime hourTime)
        {
            var result = new List<TREND_AI_STRUCT>();

            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // 해당 시간의 시작과 끝 계산
                    DateTime startTime = new DateTime(hourTime.Year, hourTime.Month, hourTime.Day, hourTime.Hour, 0, 0);
                    DateTime endTime = startTime.AddMinutes(59);

                    using (var command = new NpgsqlCommand(@"
                    SELECT data_time, sum_min, average, min_value, max_value, curr_value
                    FROM operational.minute_analog_data
                    WHERE tag_id = @tagId 
                      AND data_time BETWEEN @startTime AND @endTime
                    ORDER BY data_time", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("startTime", startTime);
                        command.Parameters.AddWithValue("endTime", endTime);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var trend = new TREND_AI_STRUCT();
                                trend.fSumMin = reader.GetFloat(reader.GetOrdinal("sum_min"));
                                trend.fAverage = reader.GetFloat(reader.GetOrdinal("average"));
                                trend.fMin = reader.GetFloat(reader.GetOrdinal("min_value"));
                                trend.fMax = reader.GetFloat(reader.GetOrdinal("max_value"));
                                trend.fCurr = reader.GetFloat(reader.GetOrdinal("curr_value"));

                                result.Add(trend);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadHourMinuteDataAI 오류: {ex.Message}");
            }

            return result;
        }


        public async Task<bool> LoadMinDataStructAI(string tagName, int year, int month,
        int day, int hour, int minute, TREND_AI_STRUCT data)
        {
            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    DateTime dataTime;
                    try
                    {
                        dataTime = new DateTime(year, month, day, hour, minute, 0);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Debug.WriteLine($"유효하지 않은 날짜입니다: {year}-{month:D2}-{day:D2} {hour:D2}:{minute:D2}:00");
                        return false;
                    }

                    using (var command = new NpgsqlCommand(@"
                    SELECT sum_min, average, min_value, max_value, curr_value
                    FROM operational.minute_analog_data
                    WHERE tag_id = @tagId AND data_time = @dataTime", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("dataTime", dataTime);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                data.fSumMin = reader.GetFloat(reader.GetOrdinal("sum_min"));
                                data.fAverage = reader.GetFloat(reader.GetOrdinal("average"));
                                data.fMin = reader.GetFloat(reader.GetOrdinal("min_value"));
                                data.fMax = reader.GetFloat(reader.GetOrdinal("max_value"));
                                data.fCurr = reader.GetFloat(reader.GetOrdinal("curr_value"));
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadMinDataStructAI 오류: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// 특정 시간대의 모든 분 디지털 데이터를 한 번에 조회
        /// </summary>
        public async Task<List<TREND_DI_STRUCT>> LoadHourMinuteDataDI(string tagName, DateTime hourTime)
        {
            var result = new List<TREND_DI_STRUCT>();

            try
            {

                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    DateTime startTime = new DateTime(hourTime.Year, hourTime.Month, hourTime.Day, hourTime.Hour, 0, 0);
                    DateTime endTime = startTime.AddMinutes(59);

                    using (var command = new NpgsqlCommand(@"
                    SELECT data_time, count_on_off, on_off_state, on_time
                    FROM operational.minute_digital_data
                    WHERE tag_id = @tagId 
                      AND data_time BETWEEN @startTime AND @endTime
                    ORDER BY data_time", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("startTime", startTime);
                        command.Parameters.AddWithValue("endTime", endTime);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var trend = new TREND_DI_STRUCT();
                                trend.nCountOnOff = reader.GetInt16(reader.GetOrdinal("count_on_off"));
                                trend.bOnOff = reader.GetBoolean(reader.GetOrdinal("on_off_state")) ? (byte)1 : (byte)0;
                                trend.cOnTime = (byte)reader.GetInt16(reader.GetOrdinal("on_time"));

                                result.Add(trend);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadHourMinuteDataDI 오류: {ex.Message}");
            }

            return result;
        }
        public async Task<bool> SaveHourDataAI(string tagName, DateTime dataTime, HOUR_DATA_ANALOG_STRUCT data)
        {
            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                // 시간을 시간 단위로 정규화
                DateTime normalizedTime = NormalizeToHour(dataTime);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                    INSERT INTO operational.hour_analog_data 
                    (tag_id, data_time, sum_hour, avg_hour, min_hour, max_hour, curr_sum_meter, flag)
                    VALUES (@tagId, @dataTime, @sumHour, @avgHour, @minHour, @maxHour, @currSumMeter, @flag)
                    ON CONFLICT (tag_id, data_time) DO UPDATE SET
                        sum_hour = EXCLUDED.sum_hour,
                        avg_hour = EXCLUDED.avg_hour,
                        min_hour = EXCLUDED.min_hour,
                        max_hour = EXCLUDED.max_hour,
                        curr_sum_meter = EXCLUDED.curr_sum_meter,
                        flag = EXCLUDED.flag", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("dataTime", normalizedTime);
                        command.Parameters.AddWithValue("sumHour", data.fSumHour);
                        command.Parameters.AddWithValue("avgHour", data.fAveHour);
                        command.Parameters.AddWithValue("minHour", data.fMinHour);
                        command.Parameters.AddWithValue("maxHour", data.fMaxHour);
                        command.Parameters.AddWithValue("currSumMeter", data.fCurrSumMeter);
                        command.Parameters.AddWithValue("flag", data.flag != 0);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveHourDataAI 오류: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Digital Data Methods
        public async Task<bool> SaveMinDataDI(string tagName, DateTime dataTime, TREND_DI_STRUCT data)
        {
            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName, "", 1);

                // 시간을 분 단위로 정규화
                DateTime normalizedTime = NormalizeToMinute(dataTime);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                    INSERT INTO operational.minute_digital_data 
                    (tag_id, data_time, count_on_off, on_off_state, on_time)
                    VALUES (@tagId, @dataTime, @countOnOff, @onOffState, @onTime)
                    ON CONFLICT (tag_id, data_time) DO UPDATE SET
                        count_on_off = EXCLUDED.count_on_off,
                        on_off_state = EXCLUDED.on_off_state,
                        on_time = EXCLUDED.on_time", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("dataTime", normalizedTime);
                        command.Parameters.AddWithValue("countOnOff", data.nCountOnOff);
                        command.Parameters.AddWithValue("onOffState", data.bOnOff != 0);
                        command.Parameters.AddWithValue("onTime", data.cOnTime);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("SaveMinDataDI 오류: {0}", ex.Message));
                return false;
            }
        }


        public async Task<bool> LoadMinDataStructDI(string tagName, int year, int month,
            int day, int hour, int minute, TREND_DI_STRUCT data)
        {
            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    DateTime dataTime;
                    try
                    {
                        dataTime = new DateTime(year, month, day, hour, minute, 0);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Debug.WriteLine($"유효하지 않은 날짜입니다: {year}-{month:D2}-{day:D2} {hour:D2}:{minute:D2}:00");
                        return false;
                    }

                    using (var command = new NpgsqlCommand(@"
                    SELECT count_on_off, on_off_state, on_time
                    FROM operational.minute_digital_data
                    WHERE tag_id = @tagId AND data_time = @dataTime", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("dataTime", dataTime);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                data.nCountOnOff = reader.GetInt16(reader.GetOrdinal("count_on_off"));
                                data.bOnOff = reader.GetBoolean(reader.GetOrdinal("on_off_state")) ? (byte)1 : (byte)0;
                                data.cOnTime = reader.GetByte(reader.GetOrdinal("on_time"));
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadMinDataStructDI 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 시간 디지털 데이터 저장
        /// </summary>
        public async Task<bool> SaveHourDataDI(string tagName, DateTime dataTime, HOUR_DATA_DIGITAL_STRUCT data)
        {
            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName, "", 1);

                // 시간을 시간 단위로 정규화
                DateTime normalizedTime = NormalizeToHour(dataTime);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                    INSERT INTO operational.hour_digital_data 
                    (tag_id, data_time, count_on_off, on_time, flag)
                    VALUES (@tagId, @dataTime, @countOnOff, @onTime, @flag)
                    ON CONFLICT (tag_id, data_time) DO UPDATE SET
                        count_on_off = EXCLUDED.count_on_off,
                        on_time = EXCLUDED.on_time,
                        flag = EXCLUDED.flag", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("dataTime", normalizedTime);
                        command.Parameters.AddWithValue("countOnOff", (int)data.wCountOnOff);
                        command.Parameters.AddWithValue("onTime", (int)data.dwOnTime);
                        command.Parameters.AddWithValue("flag", data.flag != 0);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("SaveHourDataDI 오류: {0}", ex.Message));
                return false;
            }
        }



        /// <summary>
        /// 시간 아날로그 데이터 로드
        /// </summary>
        public async Task<bool> LoadHourDataStructAI(string tagName, int year, int month,
            int day, int hour, HOUR_DATA_ANALOG_STRUCT data)
        {
            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    DateTime dataTime;
                    try
                    {
                        dataTime = new DateTime(year, month, day, hour, 0, 0);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Debug.WriteLine($"유효하지 않은 날짜입니다: {year}-{month:D2}-{day:D2} {hour:D2}:00:00");
                        return false;
                    }

                    using (var command = new NpgsqlCommand(@"
                    SELECT sum_hour, avg_hour, min_hour, max_hour, curr_sum_meter, flag
                    FROM operational.hour_analog_data
                    WHERE tag_id = @tagId AND data_time = @dataTime", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("dataTime", dataTime);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                data.fSumHour = reader.GetFloat(reader.GetOrdinal("sum_hour"));
                                data.fAveHour = reader.GetFloat(reader.GetOrdinal("avg_hour"));
                                data.fMinHour = reader.GetFloat(reader.GetOrdinal("min_hour"));
                                data.fMaxHour = reader.GetFloat(reader.GetOrdinal("max_hour"));
                                data.fCurrSumMeter = reader.GetFloat(reader.GetOrdinal("curr_sum_meter"));
                                data.flag = reader.GetBoolean(reader.GetOrdinal("flag")) ? (byte)1 : (byte)0;
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("LoadHourDataStructAI 오류: {0}", ex.Message));
                return false;
            }
        }


        /// <summary>
        /// 시간 디지털 데이터 로드
        /// </summary>
        public async Task<bool> LoadHourDataStructDI(string tagName, int year, int month,
            int day, int hour, HOUR_DATA_DIGITAL_STRUCT data)
        {
            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    DateTime dataTime;
                    try
                    {
                        dataTime = new DateTime(year, month, day, hour, 0, 0);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        return false;
                    }

                    using (var command = new NpgsqlCommand(@"
                    SELECT count_on_off, on_time, flag
                    FROM operational.hour_digital_data
                    WHERE tag_id = @tagId AND data_time = @dataTime", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("dataTime", dataTime);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                data.wCountOnOff = (ushort)reader.GetInt32(reader.GetOrdinal("count_on_off"));
                                data.dwOnTime = (uint)reader.GetInt32(reader.GetOrdinal("on_time"));
                                data.flag = reader.GetBoolean(reader.GetOrdinal("flag")) ? (byte)1 : (byte)0;
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("LoadHourDataStructDI 오류: {0}", ex.Message));
                return false;
            }
        }


        #endregion


        #region 월별 배치 조회 메서드 (파일시스템 방식과 동일한 성능)

        /// <summary>
        /// 월별 모든 분별 AI 데이터를 한 번에 조회 (TimescaleDB 최적화)
        /// 파일시스템에서 한 달 파일을 통으로 읽는 것과 동일한 효과
        /// </summary>
        public async Task<System.Data.DataTable> GetMonthMinuteDataAI(string tagName, int year, int month)
        {
            var dt = new System.Data.DataTable();

            try
            {
                // 캐시된 tag_id 조회 (System DB)
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // 월 시작/끝 시간 계산
                    DateTime monthStart = new DateTime(year, month, 1, 0, 0, 0);
                    DateTime monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

                    using (var command = new NpgsqlCommand(@"
                SELECT data_time, sum_min, average, min_value, max_value, curr_value
                    FROM operational.minute_analog_data 
                    WHERE tag_id = @tagId 
                      AND data_time >= @startTime 
                      AND data_time <= @endTime
                    ORDER BY data_time", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("startTime", monthStart);
                        command.Parameters.AddWithValue("endTime", monthEnd);

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetMonthMinuteDataAI 오류: {ex.Message}");
            }

            return dt;
        }

        /// <summary>
        /// 월별 모든 분별 DI 데이터를 한 번에 조회
        /// </summary>
        public async Task<System.Data.DataTable> GetMonthMinuteDataDI(string tagName, int year, int month)
        {
            var dt = new System.Data.DataTable();

            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    DateTime monthStart = new DateTime(year, month, 1, 0, 0, 0);
                    DateTime monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

                    using (var command = new NpgsqlCommand(@"
                    SELECT data_time, count_on_off, on_off_state, on_time
                    FROM operational.minute_digital_data
                    WHERE tag_id = @tagId 
                      AND data_time >= @startTime 
                      AND data_time <= @endTime
                    ORDER BY data_time", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("startTime", monthStart);
                        command.Parameters.AddWithValue("endTime", monthEnd);

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetMonthMinuteDataDI 오류: {ex.Message}");
            }

            return dt;
        }

        /// <summary>
        /// 월별 모든 시간별 AI 데이터를 한 번에 조회
        /// </summary>
        public async Task<System.Data.DataTable> GetMonthHourDataAI(string tagName, int year, int month)
        {
            var dt = new System.Data.DataTable();

            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    DateTime monthStart = new DateTime(year, month, 1, 0, 0, 0);
                    DateTime monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

                    using (var command = new NpgsqlCommand(@"
                    SELECT data_time, sum_hour, avg_hour, min_hour, max_hour, curr_sum_meter, flag
                    FROM operational.hour_analog_data
                    WHERE tag_id = @tagId 
                      AND data_time >= @startTime 
                      AND data_time <= @endTime
                    ORDER BY data_time", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("startTime", monthStart);
                        command.Parameters.AddWithValue("endTime", monthEnd);

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetMonthHourDataAI 오류: {ex.Message}");
            }

            return dt;
        }

        /// <summary>
        /// 월별 모든 시간별 DI 데이터를 한 번에 조회
        /// </summary>
        public async Task<System.Data.DataTable> GetMonthHourDataDI(string tagName, int year, int month)
        {
            var dt = new System.Data.DataTable();

            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    DateTime monthStart = new DateTime(year, month, 1, 0, 0, 0);
                    DateTime monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

                    using (var command = new NpgsqlCommand(@"
                    SELECT data_time, count_on_off, on_time, flag
                    FROM operational.hour_digital_data
                    WHERE tag_id = @tagId 
                      AND data_time >= @startTime 
                      AND data_time <= @endTime
                    ORDER BY data_time", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
                        command.Parameters.AddWithValue("startTime", monthStart);
                        command.Parameters.AddWithValue("endTime", monthEnd);

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetMonthHourDataDI 오류: {ex.Message}");
            }

            return dt;
        }

        #endregion


        #region Alarm Methods

        /// <summary>
        /// 경보 저장
        /// </summary>
        public async Task<long> SaveAlarm(
            DateTime alarmDateTime,
            string tagName,
            string description,
            string message,
            ushort alarmType,      // ushort 그대로
            ushort priority,       // ushort 그대로
            ushort port,           // ushort 그대로
            ushort station,        // ushort 그대로
            uint address,          // uint 그대로
            ushort subType,        // ushort 그대로
            string username = null,
            string ipAddress = null,
            string computerName = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                INSERT INTO operational.alarms 
                (alarm_datetime, tag_name, description, message, alarm_type, priority, 
                 port, station, address, sub_type, username, ip_address, computer_name)
                VALUES (@alarmDateTime, @tagName, @description, @message, @alarmType, @priority,
                        @port, @station, @address, @subType, @username, @ipAddress, @computerName)
                ON CONFLICT (alarm_datetime, tag_name, alarm_type) DO NOTHING
                RETURNING id", connection))
                    {
                        command.Parameters.AddWithValue("alarmDateTime", alarmDateTime);
                        command.Parameters.AddWithValue("tagName", tagName ?? "");
                        command.Parameters.AddWithValue("description", description ?? "");
                        command.Parameters.AddWithValue("message", message ?? "");
                        command.Parameters.AddWithValue("alarmType", (int)alarmType);      // INTEGER로 저장
                        command.Parameters.AddWithValue("priority", (int)priority);        // INTEGER로 저장
                        command.Parameters.AddWithValue("port", (int)port);                // INTEGER로 저장
                        command.Parameters.AddWithValue("station", (int)station);          // INTEGER로 저장
                        command.Parameters.AddWithValue("address", (long)address);         // BIGINT로 저장
                        command.Parameters.AddWithValue("subType", (int)subType);          // INTEGER로 저장
                        command.Parameters.AddWithValue("username", (object)username ?? DBNull.Value);
                        command.Parameters.AddWithValue("ipAddress", (object)ipAddress ?? DBNull.Value);
                        command.Parameters.AddWithValue("computerName", (object)computerName ?? DBNull.Value);

                        var result = await command.ExecuteScalarAsync();
                        return result != null ? Convert.ToInt64(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("SaveAlarm 오류: {0}", ex.Message));
                return -1;
            }
        }


        /// <summary>
        /// 경보 일괄 저장 (배치 처리)
        /// </summary>
        public async Task<int> SaveAlarmsBatch(List<AlarmData> alarms)
        {
            int savedCount = 0;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = new NpgsqlCommand(@"
                        INSERT INTO operational.alarms 
                        (alarm_datetime, tag_name, description, message, alarm_type, priority, 
                         port, station, address, sub_type, username, ip_address, computer_name)
                        VALUES (@alarmDateTime, @tagName, @description, @message, @alarmType, @priority,
                                @port, @station, @address, @subType, @username, @ipAddress, @computerName)
                        ON CONFLICT (alarm_datetime, tag_name, alarm_type) DO NOTHING", connection, transaction))
                            {
                                // 파라미터 준비
                                var pAlarmDateTime = command.Parameters.Add("alarmDateTime", NpgsqlTypes.NpgsqlDbType.TimestampTz);
                                var pTagName = command.Parameters.Add("tagName", NpgsqlTypes.NpgsqlDbType.Varchar);
                                var pDescription = command.Parameters.Add("description", NpgsqlTypes.NpgsqlDbType.Varchar);
                                var pMessage = command.Parameters.Add("message", NpgsqlTypes.NpgsqlDbType.Varchar);
                                var pAlarmType = command.Parameters.Add("alarmType", NpgsqlTypes.NpgsqlDbType.Smallint);
                                var pPriority = command.Parameters.Add("priority", NpgsqlTypes.NpgsqlDbType.Smallint);
                                var pPort = command.Parameters.Add("port", NpgsqlTypes.NpgsqlDbType.Smallint);
                                var pStation = command.Parameters.Add("station", NpgsqlTypes.NpgsqlDbType.Smallint);
                                var pAddress = command.Parameters.Add("address", NpgsqlTypes.NpgsqlDbType.Integer);
                                var pSubType = command.Parameters.Add("subType", NpgsqlTypes.NpgsqlDbType.Smallint);
                                var pUsername = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
                                var pIpAddress = command.Parameters.Add("ipAddress", NpgsqlTypes.NpgsqlDbType.Varchar);
                                var pComputerName = command.Parameters.Add("computerName", NpgsqlTypes.NpgsqlDbType.Varchar);

                                await command.PrepareAsync();

                                foreach (var item in alarms)
                                {
                                    // 디버깅 로그
                                    Debug.WriteLine($"DB 저장: {item.TagName}, UTC: {item.AlarmDateTime:yyyy-MM-dd HH:mm:ss.fff}");

                                    pAlarmDateTime.Value = item.AlarmDateTime;    // 로컬 시간 그대로 저장 (PostgreSQL이 자동으로 시간대 처리
                                    pTagName.Value = item.TagName;
                                    pDescription.Value = (object)item.Description ?? DBNull.Value;
                                    pMessage.Value = (object)item.Message ?? DBNull.Value;
                                    pAlarmType.Value = (int)item.AlarmType;      // ushort → int
                                    pPriority.Value = (int)item.Priority;        // ushort → int
                                    pPort.Value = (int)item.Port;                // ushort → int
                                    pStation.Value = (int)item.Station;          // ushort → int
                                    pAddress.Value = (long)item.Address;         // uint → long
                                    pSubType.Value = (int)item.SubType;          // ushort → int
                                    pUsername.Value = (object)item.Username ?? DBNull.Value;
                                    pIpAddress.Value = (object)item.IpAddress ?? DBNull.Value;
                                    pComputerName.Value = (object)item.ComputerName ?? DBNull.Value;

                                    savedCount += await command.ExecuteNonQueryAsync();
                                }

                                await transaction.CommitAsync();
                                if (savedCount > 0)
                                {
                                    Debug.WriteLine($"경보 배치 저장: {savedCount}개");
                                }
                            }
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("SaveAlarmsBatch 오류: {0}", ex.Message));
                return -1;
            }

            return savedCount;
        }

        /// <summary>
        /// 경보 조회 (기간별)
        /// </summary>
        public async Task<System.Data.DataTable> GetAlarms(DateTime startTime, DateTime endTime, string tagName = null)
        {
            var dt = new System.Data.DataTable();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                SELECT id, alarm_datetime, tag_name, description, message, 
                       alarm_type, priority, port, station, address, sub_type,
                       username, ip_address, computer_name, created_at
                FROM operational.alarms 
                WHERE alarm_datetime BETWEEN @startTime AND @endTime";

                    if (!string.IsNullOrEmpty(tagName))
                    {
                        query += " AND tag_name = @tagName";
                    }

                    query += " ORDER BY alarm_datetime DESC, id DESC";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("startTime", startTime);
                        command.Parameters.AddWithValue("endTime", endTime);

                        if (!string.IsNullOrEmpty(tagName))
                        {
                            command.Parameters.AddWithValue("tagName", tagName);
                        }

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("GetAlarms 오류: {0}", ex.Message));
            }

            return dt;
        }


        /// <summary>
        /// 경보 조회 (고급 필터) - AddAlarmFile 로직을 DB 조회로 변경
        /// </summary>
        /// <param name="startTime">시작 시간</param>
        /// <param name="endTime">종료 시간</param>
        /// <param name="filterTag">태그명 정규식 필터 (null이면 모든 태그)</param>
        /// <param name="filterType">경보 타입 필터 배열 (null이면 모든 타입)</param>
        /// <param name="filterPort">포트 필터 배열 (null이면 모든 포트)</param>
        /// <param name="priority">우선순위 필터 (null이면 모든 우선순위)</param>
        /// <param name="limit">최대 조회 건수</param>
        /// <returns>경보 데이터 테이블</returns>
        public async Task<System.Data.DataTable> GetAlarmsAdvanced(
            DateTime startTime,
            DateTime endTime,
            Regex filterTag = null,
            bool[] filterType = null,
            bool[] filterPort = null,
            short? priority = null,
              int? limit = null) // nullable로 변경
        {
            var dt = new System.Data.DataTable();

            // DataTable 컬럼 구조 정의 (GetAlarmFileAsync와 동일한 구조)
            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("alarm_datetime", typeof(DateTime));
            dt.Columns.Add("tag_name", typeof(string));
            dt.Columns.Add("description", typeof(string));
            dt.Columns.Add("message", typeof(string));
            dt.Columns.Add("alarm_type", typeof(int));
            dt.Columns.Add("priority", typeof(int));
            dt.Columns.Add("port", typeof(int));
            dt.Columns.Add("station", typeof(int));
            dt.Columns.Add("address", typeof(long));
            dt.Columns.Add("sub_type", typeof(int));
            dt.Columns.Add("username", typeof(string));
            dt.Columns.Add("ip_address", typeof(string));
            dt.Columns.Add("computer_name", typeof(string));

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // 기본 WHERE 조건
                    var whereConditions = new List<string>
            {
                "alarm_datetime BETWEEN @startTime AND @endTime"
            };

                    // 우선순위 필터 추가
                    if (priority.HasValue)
                    {
                        whereConditions.Add("priority = @priority");
                    }

                    // SQL에서 limit 조건 처리
                    string limitClause = limit.HasValue ? "LIMIT @limit" : "";
                    string query = string.Format(@"
                    SELECT id, alarm_datetime, tag_name, description, message, 
                           alarm_type, priority, port, station, address, sub_type,
                           username, ip_address, computer_name
                    FROM operational.alarms 
                    WHERE {0}
                    ORDER BY alarm_datetime DESC, id DESC
                    {1}", string.Join(" AND ", whereConditions), limitClause);

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // 파라미터 설정 (시간을 UTC로 변환)
                        DateTime startTimeUtc = DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime();
                        DateTime endTimeUtc = DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime();

                        command.Parameters.AddWithValue("startTime", startTimeUtc);
                        command.Parameters.AddWithValue("endTime", endTimeUtc);
                        // limit 파라미터는 값이 있을 때만 추가
                        if (limit.HasValue)
                        {
                            command.Parameters.AddWithValue("limit", limit.Value);
                        }

                        if (priority.HasValue)
                        {
                            command.Parameters.AddWithValue("priority", priority.Value);
                        }

                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                // DB에서 읽은 데이터 (인덱스 기반 접근)
                                long id = reader.GetInt64(0);
                                DateTime alarmDateTime = reader.GetDateTime(1);
                                string tagName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                string description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                string message = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                                int alarmType = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                                int priorityValue = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                                int port = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                                int station = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                                long address = reader.IsDBNull(9) ? 0L : reader.GetInt64(9);
                                int subType = reader.IsDBNull(10) ? 0 : reader.GetInt32(10);
                                string username = reader.IsDBNull(11) ? string.Empty : reader.GetString(11);
                                string ipAddress = reader.IsDBNull(12) ? string.Empty : reader.GetString(12);
                                string computerName = reader.IsDBNull(13) ? string.Empty : reader.GetString(13);

                                // AddAlarmFile과 동일한 필터링 로직 적용

                                // 태그 필터 검사
                                if (filterTag != null && !filterTag.IsMatch(tagName))
                                    continue;

                                // 경보 타입 필터 검사
                                if (filterType != null && !IsBoolFilterInclude(filterType, alarmType))
                                    continue;

                                // 포트 필터 검사
                                if (filterPort != null && !IsBoolFilterInclude(filterPort, port))
                                    continue;

                                // 필터를 통과한 데이터를 DataTable에 추가 (GetAlarmFileAsync와 동일한 구조)
                                DataRow row = dt.NewRow();

                                row["Id"] = id;
                                row["alarm_datetime"] = alarmDateTime.ToLocalTime(); // UTC에서 Local 시간으로 변환
                                row["tag_name"] = tagName;
                                row["description"] = description;
                                row["message"] = message;
                                row["alarm_type"] = alarmType;
                                row["priority"] = priorityValue;
                                row["port"] = port;
                                row["station"] = station;
                                row["address"] = address;
                                row["sub_type"] = subType;
                                row["username"] = username;
                                row["ip_address"] = ipAddress;
                                row["computer_name"] = computerName;

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("GetAlarmsAdvanced 오류: {0}", ex.Message));
            }

            return dt;
        }

        /// <summary>
        /// bool 배열 필터에 해당 값이 포함되는지 검사 (AddAlarmFile에서 사용하던 메서드)
        /// </summary>
        /// <param name="filterArray">필터 배열</param>
        /// <param name="value">검사할 값</param>
        /// <returns>포함 여부</returns>
        private bool IsBoolFilterInclude(bool[] filterArray, int value)
        {
            if (filterArray == null || value < 0 || value >= filterArray.Length)
                return false;

            return filterArray[value];
        }


        /// <summary>
        /// 경보 데이터 클래스
        /// </summary>
        public class AlarmData
        {
            public long Id { get; set; }
            public DateTime AlarmDateTime { get; set; }
            public string TagName { get; set; }
            public string Description { get; set; }
            public string Message { get; set; }
            public ushort AlarmType { get; set; }      // DB INTEGER → ushort
            public ushort Priority { get; set; }       // DB INTEGER → ushort
            public ushort Port { get; set; }           // DB INTEGER → ushort
            public ushort Station { get; set; }        // DB INTEGER → ushort
            public uint Address { get; set; }          // DB BIGINT → uint
            public ushort SubType { get; set; }        // DB INTEGER → ushort
            public string Username { get; set; }
            public string IpAddress { get; set; }
            public string ComputerName { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        #endregion


        #region Log Methods

        /// <summary>
        /// 로그 저장
        /// </summary>
        public async Task<bool> SaveLog(
            LogLevel level,
            int category,
            string message,
            string username = null,
            string ipAddress = null,
            string machineName = null,
            string detail = null,
            DateTime? logDateTime = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(@"
                INSERT INTO operational.logs 
                (log_datetime, level, category, message, username, ip_address, machine_name, detail)
                VALUES (@logDateTime, @level, @category, @message, @username, @ipAddress, @machineName, @detail::jsonb)",
                        connection))
                    {
                        command.Parameters.AddWithValue("logDateTime", logDateTime ?? DateTime.Now);
                        command.Parameters.AddWithValue("level", (short)level);
                        command.Parameters.AddWithValue("category", category);
                        command.Parameters.AddWithValue("message", message ?? "");
                        command.Parameters.AddWithValue("username", (object)username ?? DBNull.Value);
                        command.Parameters.AddWithValue("ipAddress", (object)ipAddress ?? DBNull.Value);
                        command.Parameters.AddWithValue("machineName", (object)machineName ?? DBNull.Value);
                        command.Parameters.AddWithValue("detail", (object)detail ?? DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveLog 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 로그 배치 저장
        /// </summary>
        public async Task<int> SaveLogsBatch(List<LogEntry> logs)
        {
            int savedCount = 0;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = new NpgsqlCommand(@"
                        INSERT INTO operational.logs 
                        (log_datetime, level, category, message, username, ip_address, machine_name, detail)
                        VALUES (@logDateTime, @level, @category, @message, @username, @ipAddress, @machineName, @detail::jsonb)",
                                connection, transaction))
                            {
                                var pLogDateTime = command.Parameters.Add("logDateTime", NpgsqlTypes.NpgsqlDbType.TimestampTz);
                                var pLevel = command.Parameters.Add("level", NpgsqlTypes.NpgsqlDbType.Smallint);
                                var pCategory = command.Parameters.Add("category", NpgsqlTypes.NpgsqlDbType.Integer);
                                var pMessage = command.Parameters.Add("message", NpgsqlTypes.NpgsqlDbType.Text);
                                var pUsername = command.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar);
                                var pIpAddress = command.Parameters.Add("ipAddress", NpgsqlTypes.NpgsqlDbType.Varchar);
                                var pMachineName = command.Parameters.Add("machineName", NpgsqlTypes.NpgsqlDbType.Varchar);
                                var pDetail = command.Parameters.Add("detail", NpgsqlTypes.NpgsqlDbType.Text);

                                await command.PrepareAsync();

                                foreach (var log in logs)
                                {
                                    // DateTime을 UTC로 변환
                                    var utcDateTime = log.LogDateTime.Kind == DateTimeKind.Utc
                                        ? log.LogDateTime
                                        : log.LogDateTime.ToUniversalTime();

                                    pLogDateTime.Value = utcDateTime;
                                    pLevel.Value = (short)log.Level;
                                    pCategory.Value = log.Category;
                                    pMessage.Value = log.Message ?? "";
                                    pUsername.Value = (object)log.Username ?? DBNull.Value;
                                    pIpAddress.Value = (object)log.IpAddress ?? DBNull.Value;
                                    pMachineName.Value = (object)log.MachineName ?? DBNull.Value;
                                    pDetail.Value = (object)log.Detail ?? DBNull.Value;

                                    savedCount += await command.ExecuteNonQueryAsync();
                                }
                            }

                            await transaction.CommitAsync();
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("SaveLogsBatch 오류: {0}", ex.Message));
                return -1;
            }

            return savedCount;
        }



        /// <summary>
        /// 로그 저장 (간편 메서드 - INFO 레벨)
        /// </summary>
        public async Task<bool> SaveLogInfo(int category, string message, string username = null)
        {
            return await SaveLog(LogLevel.INFO, category, message, username,
                GetClientIpAddress(), Environment.MachineName);
        }

        /// <summary>
        /// 로그 저장 (간편 메서드 - WARNING 레벨)
        /// </summary>
        public async Task<bool> SaveLogWarning(int category, string message, string username = null, string detail = null)
        {
            return await SaveLog(LogLevel.WARNING, category, message, username,
                GetClientIpAddress(), Environment.MachineName, detail);
        }

        /// <summary>
        /// 로그 저장 (간편 메서드 - ERROR 레벨)
        /// </summary>
        public async Task<bool> SaveLogError(int category, string message, Exception ex = null, string username = null)
        {
            string detail = ex != null ? $"{{\"error\":\"{ex.Message}\",\"stackTrace\":\"{ex.StackTrace}\"}}".Replace("\"", "\\\"") : null;
            return await SaveLog(LogLevel.ERROR, category, message, username,
                GetClientIpAddress(), Environment.MachineName, detail);
        }

        /// <summary>
        /// 로그 조회 (기간별, 필터 포함)
        /// </summary>
        public async Task<System.Data.DataTable> GetLogs(
            DateTime startTime,
            DateTime endTime,
            string searchText = null,
            LogLevel? minLevel = null,
            int? category = null,
            string username = null)
        {
            var dt = new System.Data.DataTable();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var queryBuilder = new System.Text.StringBuilder(@"
                SELECT 
                    id,
                    log_datetime,
                    level,
                    category,
                    message,
                    username,
                    ip_address,
                    machine_name,
                    detail,
                    created_at
                FROM operational.logs 
                WHERE log_datetime BETWEEN @startTime AND @endTime");

                    var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("startTime", startTime),
                new NpgsqlParameter("endTime", endTime)
            };

                    // 검색어 필터
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        queryBuilder.Append(" AND message ILIKE @searchText");
                        parameters.Add(new NpgsqlParameter("searchText", "%" + searchText + "%"));
                    }

                    // 레벨 필터 (해당 레벨 이상)
                    if (minLevel.HasValue)
                    {
                        queryBuilder.Append(" AND level >= @minLevel");
                        parameters.Add(new NpgsqlParameter("minLevel", (short)minLevel.Value));
                    }

                    // 카테고리 필터 (100단위 그룹 또는 정확한 카테고리)
                    if (category.HasValue)
                    {
                        // 100으로 나누어떨어지면 그룹 전체, 아니면 정확한 카테고리
                        if (category.Value % 100 == 0)
                        {
                            queryBuilder.Append(" AND category >= @categoryMin AND category < @categoryMax");
                            parameters.Add(new NpgsqlParameter("categoryMin", category.Value));
                            parameters.Add(new NpgsqlParameter("categoryMax", category.Value + 100));
                        }
                        else
                        {
                            queryBuilder.Append(" AND category = @category");
                            parameters.Add(new NpgsqlParameter("category", category.Value));
                        }
                    }

                    // 사용자 필터
                    if (!string.IsNullOrEmpty(username))
                    {
                        queryBuilder.Append(" AND username = @username");
                        parameters.Add(new NpgsqlParameter("username", username));
                    }

                    queryBuilder.Append(" ORDER BY log_datetime DESC, id DESC LIMIT 10000");

                    using (var command = new NpgsqlCommand(queryBuilder.ToString(), connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetLogs 오류: {ex.Message}");
            }
            return dt;
        }

        /// <summary>
        /// 로그 조회 (ID 기반 페이징)
        /// </summary>
        public async Task<System.Data.DataTable> GetLogsPaged(
            long startId,
            int pageSize = 100,
            LogLevel? minLevel = null,
            int? category = null)
        {
            var dt = new System.Data.DataTable();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var queryBuilder = new System.Text.StringBuilder(@"
                SELECT 
                    id,
                    log_datetime,
                    level,
                    category,
                    message,
                    username,
                    ip_address,
                    machine_name,
                    created_at
                FROM operational.logs 
                WHERE id > @startId");

                    var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("startId", startId),
                new NpgsqlParameter("pageSize", pageSize)
            };

                    if (minLevel.HasValue)
                    {
                        queryBuilder.Append(" AND level >= @minLevel");
                        parameters.Add(new NpgsqlParameter("minLevel", (short)minLevel.Value));
                    }

                    if (category.HasValue)
                    {
                        if (category.Value % 100 == 0)
                        {
                            queryBuilder.Append(" AND category >= @categoryMin AND category < @categoryMax");
                            parameters.Add(new NpgsqlParameter("categoryMin", category.Value));
                            parameters.Add(new NpgsqlParameter("categoryMax", category.Value + 100));
                        }
                        else
                        {
                            queryBuilder.Append(" AND category = @category");
                            parameters.Add(new NpgsqlParameter("category", category.Value));
                        }
                    }

                    queryBuilder.Append(" ORDER BY id LIMIT @pageSize");

                    using (var command = new NpgsqlCommand(queryBuilder.ToString(), connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        using (var adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetLogsPaged 오류: {ex.Message}");
            }
            return dt;
        }

        /// <summary>
        /// 특정 로그 상세 조회
        /// </summary>
        public async Task<LogEntry> GetLogById(long id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                SELECT 
                    id,
                    log_datetime,
                    level,
                    category,
                    message,
                    username,
                    ip_address,
                    machine_name,
                    detail,
                    created_at
                FROM operational.logs 
                WHERE id = @id",
                        connection))
                    {
                        command.Parameters.AddWithValue("id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new LogEntry
                                {
                                    Id = reader.GetInt64(0),
                                    LogDateTime = reader.GetDateTime(1),
                                    Level = (LogLevel)reader.GetInt16(2),
                                    Category = reader.GetInt32(3),
                                    Message = reader.GetString(4),
                                    Username = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    IpAddress = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    MachineName = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    Detail = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    CreatedAt = reader.GetDateTime(9)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetLogById 오류: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// 로그 통계 조회 (레벨별, 카테고리별)
        /// </summary>
        public async Task<System.Data.DataTable> GetLogStatistics(DateTime startTime, DateTime endTime)
        {
            var dt = new System.Data.DataTable();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                SELECT 
                    level,
                    (category / 100) * 100 as category_group,
                    COUNT(*) as log_count,
                    MIN(log_datetime) as first_log,
                    MAX(log_datetime) as last_log
                FROM operational.logs 
                WHERE log_datetime BETWEEN @startTime AND @endTime
                GROUP BY level, category_group
                ORDER BY level DESC, category_group",
                        connection))
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
                Debug.WriteLine($"GetLogStatistics 오류: {ex.Message}");
            }
            return dt;
        }

        /// <summary>
        /// 로그 개수 조회
        /// </summary>
        public async Task<int> GetLogCount(DateTime startTime, DateTime endTime, LogLevel? minLevel = null)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT COUNT(*) FROM operational.logs WHERE log_datetime BETWEEN @startTime AND @endTime";

                    if (minLevel.HasValue)
                    {
                        query += " AND level >= @minLevel";
                    }

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("startTime", startTime);
                        command.Parameters.AddWithValue("endTime", endTime);

                        if (minLevel.HasValue)
                        {
                            command.Parameters.AddWithValue("minLevel", (short)minLevel.Value);
                        }

                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetLogCount 오류: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 클라이언트 IP 주소 가져오기 (보조 메서드)
        /// </summary>
        private string GetClientIpAddress()
        {
            // 실제 환경에 맞게 구현
            // WinForms의 경우 로컬 IP를 반환
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch { }
            return "127.0.0.1";
        }

        #endregion

        #region Batch Operations

        /// <summary>
        /// 분 아날로그 데이터 배치 저장 (성능 최적화)
        /// </summary>
        public async Task<int> SaveMinDataAIBatch(List<(string tagName, DateTime dataTime, TREND_AI_STRUCT trend)> dataList)
        {
            if (dataList.Count == 0) return 0;

            int savedCount = 0;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Tag ID 일괄 조회 (최적화)
                    var uniqueTagNames = dataList.Select(d => d.tagName).Distinct().ToList();
                    var tagIdMap = new Dictionary<string, int>();

                    foreach (var tagName in uniqueTagNames)
                    {
                        tagIdMap[tagName] = await _tagRepo.GetOrCreateTagIdAsync(tagName);
                    }

                    // (tag_id, normalizedTime) 기준으로 중복 제거 - 마지막 항목 유지
                    // ON CONFLICT DO UPDATE는 같은 문장 내에서 동일 행을 두 번 갱신할 수 없으므로 사전에 중복 제거 필요
                    var deduped = new Dictionary<(int tagId, DateTime time), (string tagName, DateTime dataTime, TREND_AI_STRUCT trend)>();
                    foreach (var data in dataList)
                    {
                        int tagId = tagIdMap[data.tagName];
                        DateTime normalizedTime = NormalizeToMinute(data.dataTime);
                        deduped[(tagId, normalizedTime)] = data;
                    }

                    var dedupedList = deduped.ToList();

                    if (dedupedList.Count < dataList.Count)
                    {
                        Debug.WriteLine($"SaveMinDataAIBatch: 중복 제거 {dataList.Count} → {dedupedList.Count}건");
                    }

                    // VALUES 구문 생성
                    var valuesBuilder = new System.Text.StringBuilder();
                    var parameters = new List<NpgsqlParameter>();

                    for (int i = 0; i < dedupedList.Count; i++)
                    {
                        var key = dedupedList[i].Key;
                        var data = dedupedList[i].Value;

                        if (i > 0) valuesBuilder.Append(",");
                        valuesBuilder.Append($"(@tagId{i}, @dataTime{i}, @sumMin{i}, @avg{i}, @min{i}, @max{i}, @curr{i})");

                        parameters.Add(new NpgsqlParameter($"tagId{i}", key.tagId));
                        parameters.Add(new NpgsqlParameter($"dataTime{i}", key.time));
                        parameters.Add(new NpgsqlParameter($"sumMin{i}", data.trend.fSumMin));
                        parameters.Add(new NpgsqlParameter($"avg{i}", data.trend.fAverage));
                        parameters.Add(new NpgsqlParameter($"min{i}", data.trend.fMin));
                        parameters.Add(new NpgsqlParameter($"max{i}", data.trend.fMax));
                        parameters.Add(new NpgsqlParameter($"curr{i}", data.trend.fCurr));
                    }

                    string sql = $@"
                INSERT INTO operational.minute_analog_data
                (tag_id, data_time, sum_min, average, min_value, max_value, curr_value)
                VALUES {valuesBuilder}
                ON CONFLICT (tag_id, data_time) DO UPDATE SET
                    sum_min = EXCLUDED.sum_min,
                    average = EXCLUDED.average,
                    min_value = EXCLUDED.min_value,
                    max_value = EXCLUDED.max_value,
                    curr_value = EXCLUDED.curr_value";

                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        savedCount = await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveMinDataAIBatch 오류: {ex.Message}");
            }

            return savedCount;
        }

        /// <summary>
        /// 분 디지털 데이터 배치 저장 (성능 최적화)
        /// </summary>
        public async Task<int> SaveMinDataDIBatch(List<(string tagName, DateTime dataTime, TREND_DI_STRUCT trend)> dataList)
        {
            if (dataList.Count == 0) return 0;

            int savedCount = 0;

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Tag ID 일괄 조회 (최적화)
                    var uniqueTagNames = dataList.Select(d => d.tagName).Distinct().ToList();
                    var tagIdMap = new Dictionary<string, int>();

                    foreach (var tagName in uniqueTagNames)
                    {
                        tagIdMap[tagName] = await _tagRepo.GetOrCreateTagIdAsync(tagName, "", 1);
                    }

                    // (tag_id, normalizedTime) 기준으로 중복 제거 - 마지막 항목 유지
                    // ON CONFLICT DO UPDATE는 같은 문장 내에서 동일 행을 두 번 갱신할 수 없으므로 사전에 중복 제거 필요
                    var deduped = new Dictionary<(int tagId, DateTime time), (string tagName, DateTime dataTime, TREND_DI_STRUCT trend)>();
                    foreach (var data in dataList)
                    {
                        int tagId = tagIdMap[data.tagName];
                        DateTime normalizedTime = NormalizeToMinute(data.dataTime);
                        deduped[(tagId, normalizedTime)] = data;
                    }

                    var dedupedList = deduped.ToList();

                    if (dedupedList.Count < dataList.Count)
                    {
                        Debug.WriteLine($"SaveMinDataDIBatch: 중복 제거 {dataList.Count} → {dedupedList.Count}건");
                    }

                    // VALUES 구문 생성
                    var valuesBuilder = new System.Text.StringBuilder();
                    var parameters = new List<NpgsqlParameter>();

                    for (int i = 0; i < dedupedList.Count; i++)
                    {
                        var key = dedupedList[i].Key;
                        var data = dedupedList[i].Value;

                        if (i > 0) valuesBuilder.Append(",");
                        valuesBuilder.Append($"(@tagId{i}, @dataTime{i}, @countOnOff{i}, @onOffState{i}, @onTime{i})");

                        parameters.Add(new NpgsqlParameter($"tagId{i}", key.tagId));
                        parameters.Add(new NpgsqlParameter($"dataTime{i}", key.time));
                        parameters.Add(new NpgsqlParameter($"countOnOff{i}", data.trend.nCountOnOff));
                        parameters.Add(new NpgsqlParameter($"onOffState{i}", data.trend.bOnOff != 0));
                        parameters.Add(new NpgsqlParameter($"onTime{i}", data.trend.cOnTime));
                    }

                    string sql = $@"
                INSERT INTO operational.minute_digital_data
                (tag_id, data_time, count_on_off, on_off_state, on_time)
                VALUES {valuesBuilder}
                ON CONFLICT (tag_id, data_time) DO UPDATE SET
                    count_on_off = EXCLUDED.count_on_off,
                    on_off_state = EXCLUDED.on_off_state,
                    on_time = EXCLUDED.on_time";

                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        savedCount = await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveMinDataDIBatch 오류: {ex.Message}");
            }

            return savedCount;
        }

        #endregion

        #region Auto Delete & Maintenance

        /// <summary>
        /// 분 단위 데이터 자동삭제 (drop_chunks 사용)
        /// </summary>
        public async Task PerformAutoDeleteMinData(int nMonthDataSaveOfMin)
        {
            try
            {
                DateTime cutoffDate = DateTime.Now.AddMonths(-nMonthDataSaveOfMin);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        string[] deleteTables = new string[]
                        {
                        "operational.minute_analog_data",
                        "operational.minute_digital_data"
                        };

                        foreach (string table in deleteTables)
                        {
                            // TimescaleDB의 drop_chunks 함수 사용
                            string dropChunksQuery = $"SELECT drop_chunks('{table}', older_than => @cutoffDate);";

                            using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                            {
                                command.Parameters.AddWithValue("cutoffDate", cutoffDate);
                                await command.ExecuteNonQueryAsync();
                                Debug.WriteLine($"{table}에서 {cutoffDate} 이전 chunk 삭제 완료");
                            }
                        }

                        Debug.WriteLine($"분 데이터 자동 삭제 완료: {cutoffDate} 이전 데이터");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"분 데이터 자동 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"분 데이터 자동 삭제 프로세스 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 시간 단위 데이터 자동삭제 (drop_chunks 사용)
        /// </summary>
        public async Task PerformAutoDeleteHourData(int nMonthDataSave)
        {
            try
            {
                DateTime cutoffDate = DateTime.Now.AddMonths(-nMonthDataSave);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        string[] deleteTables = new string[]
                        {
                        "operational.hour_analog_data",
                        "operational.hour_digital_data"
                        };

                        foreach (string table in deleteTables)
                        {
                            string dropChunksQuery = $"SELECT drop_chunks('{table}', older_than => @cutoffDate);";

                            using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                            {
                                command.Parameters.AddWithValue("cutoffDate", cutoffDate);
                                await command.ExecuteNonQueryAsync();
                                Debug.WriteLine($"{table}에서 {cutoffDate} 이전 chunk 삭제 완료");
                            }
                        }

                        Debug.WriteLine($"시간 데이터 자동 삭제 완료: {cutoffDate} 이전 데이터");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"시간 데이터 자동 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"시간 데이터 자동 삭제 프로세스 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 알람 데이터 자동삭제 (drop_chunks 사용)
        /// </summary>
        public async Task PerformAutoDeleteAlarm(int nMonthDataSave)
        {
            try
            {
                DateTime cutoffDate = DateTime.Now.AddMonths(-nMonthDataSave);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        string dropChunksQuery = "SELECT drop_chunks('operational.alarms', older_than => @cutoffDate);";

                        using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                        {
                            command.Parameters.AddWithValue("cutoffDate", cutoffDate);
                            await command.ExecuteNonQueryAsync();
                            Debug.WriteLine($"operational.alarms에서 {cutoffDate} 이전 chunk 삭제 완료");
                        }

                        Debug.WriteLine($"알람 데이터 자동 삭제 완료: {cutoffDate} 이전 데이터");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"알람 데이터 자동 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"알람 데이터 자동 삭제 프로세스 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 로그 데이터 자동삭제 (drop_chunks 사용)
        /// </summary>
        public async Task PerformAutoDeleteLog(int nMonthDataSave)
        {
            try
            {
                DateTime cutoffDate = DateTime.Now.AddMonths(-nMonthDataSave);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        string dropChunksQuery = "SELECT drop_chunks('operational.logs', older_than => @cutoffDate);";

                        using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                        {
                            command.Parameters.AddWithValue("cutoffDate", cutoffDate);
                            await command.ExecuteNonQueryAsync();
                            Debug.WriteLine($"operational.logs에서 {cutoffDate} 이전 chunk 삭제 완료");
                        }

                        Debug.WriteLine($"로그 데이터 자동 삭제 완료: {cutoffDate} 이전 데이터");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"로그 데이터 자동 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"로그 데이터 자동 삭제 프로세스 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 밀리초 데이터 자동삭제 (디스크 용량 기반)
        /// </summary>
        public async Task PerformAutoDeleteMilliDataByUsedSize(string tableName, DateTime dateTime)
        {
            try
            {
                DateTime cutoffDate = dateTime;

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        string fullTableName = $"history.{tableName}";
                        string dropChunksQuery = $"SELECT drop_chunks('{fullTableName}', older_than => @cutoffDate);";

                        using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                        {
                            command.Parameters.AddWithValue("cutoffDate", cutoffDate);
                            await command.ExecuteNonQueryAsync();
                            Debug.WriteLine($"{fullTableName}에서 {cutoffDate} 이전 chunk 삭제 완료");
                        }

                        Debug.WriteLine($"밀리초 데이터 자동 삭제 완료: {cutoffDate} 이전 데이터");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"밀리초 데이터 자동 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"밀리초 데이터 자동 삭제 프로세스 오류: {ex.Message}");
            }
        }


        #endregion

        #region DateTime 기반 삭제 메서드

        /// <summary>
        /// 분 단위 데이터 삭제 (특정 날짜 기준)
        /// </summary>
        /// <param name="cutoffDateTime">이 날짜 이전 데이터 삭제</param>
        public async Task PerformAutoDeleteMinDataByDateTime(DateTime cutoffDateTime)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        string[] deleteTables = new string[]
                        {
                        "operational.minute_analog_data",
                        "operational.minute_digital_data"
                        };

                        foreach (string table in deleteTables)
                        {
                            string dropChunksQuery = $"SELECT drop_chunks('{table}', older_than => @cutoffDate);";

                            using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                            {
                                command.Parameters.AddWithValue("cutoffDate", cutoffDateTime);
                                await command.ExecuteNonQueryAsync();
                                Debug.WriteLine($"{table}에서 {cutoffDateTime} 이전 chunk 삭제 완료");
                            }
                        }

                        Debug.WriteLine($"분 데이터 삭제 완료: {cutoffDateTime} 이전 데이터");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"분 데이터 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"분 데이터 삭제 프로세스 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 시간 단위 데이터 삭제 (특정 날짜 기준)
        /// </summary>
        /// <param name="cutoffDateTime">이 날짜 이전 데이터 삭제</param>
        public async Task PerformAutoDeleteHourDataByDateTime(DateTime cutoffDateTime)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        string[] deleteTables = new string[]
                        {
                        "operational.hour_analog_data",
                        "operational.hour_digital_data"
                        };

                        foreach (string table in deleteTables)
                        {
                            string dropChunksQuery = $"SELECT drop_chunks('{table}', older_than => @cutoffDate);";

                            using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                            {
                                command.Parameters.AddWithValue("cutoffDate", cutoffDateTime);
                                await command.ExecuteNonQueryAsync();
                                Debug.WriteLine($"{table}에서 {cutoffDateTime} 이전 chunk 삭제 완료");
                            }
                        }

                        Debug.WriteLine($"시간 데이터 삭제 완료: {cutoffDateTime} 이전 데이터");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"시간 데이터 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"시간 데이터 삭제 프로세스 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 알람 데이터 삭제 (특정 날짜 기준)
        /// </summary>
        /// <param name="cutoffDateTime">이 날짜 이전 데이터 삭제</param>
        public async Task PerformAutoDeleteAlarmByDateTime(DateTime cutoffDateTime)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        string dropChunksQuery = "SELECT drop_chunks('operational.alarms', older_than => @cutoffDate);";

                        using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                        {
                            command.Parameters.AddWithValue("cutoffDate", cutoffDateTime);
                            await command.ExecuteNonQueryAsync();
                            Debug.WriteLine($"operational.alarms에서 {cutoffDateTime} 이전 chunk 삭제 완료");
                        }

                        Debug.WriteLine($"알람 데이터 삭제 완료: {cutoffDateTime} 이전 데이터");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"알람 데이터 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"알람 데이터 삭제 프로세스 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 로그 데이터 삭제 (특정 날짜 기준)
        /// </summary>
        /// <param name="cutoffDateTime">이 날짜 이전 데이터 삭제</param>
        public async Task PerformAutoDeleteLogByDateTime(DateTime cutoffDateTime)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        string dropChunksQuery = "SELECT drop_chunks('operational.logs', older_than => @cutoffDate);";

                        using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                        {
                            command.Parameters.AddWithValue("cutoffDate", cutoffDateTime);
                            await command.ExecuteNonQueryAsync();
                            Debug.WriteLine($"operational.logs에서 {cutoffDateTime} 이전 chunk 삭제 완료");
                        }

                        Debug.WriteLine($"로그 데이터 삭제 완료: {cutoffDateTime} 이전 데이터");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"로그 데이터 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"로그 데이터 삭제 프로세스 오류: {ex.Message}");
            }
        }

        #endregion

        #region 전체 테이블 일괄 삭제 메서드

        /// <summary>
        /// Operational 스키마의 모든 테이블 데이터 삭제 (특정 날짜 기준)
        /// </summary>
        /// <param name="cutoffDateTime">이 날짜 이전 데이터 삭제</param>
        public async Task PerformAutoDeleteOperationalTablesByDateTime(DateTime cutoffDateTime)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        Debug.WriteLine($"=== Operational 테이블 자동 삭제 시작: {cutoffDateTime} 이전 데이터 ===");

                        string[] operationalTables = new string[]
                        {
                        "operational.minute_analog_data",
                        "operational.minute_digital_data",
                        "operational.hour_analog_data",
                        "operational.hour_digital_data",
                        "operational.alarms",
                        "operational.logs"
                        };

                        int successCount = 0;
                        int failCount = 0;

                        foreach (string table in operationalTables)
                        {
                            try
                            {
                                string dropChunksQuery = $"SELECT drop_chunks('{table}', older_than => @cutoffDate);";

                                using (var command = new NpgsqlCommand(dropChunksQuery, connection))
                                {
                                    command.Parameters.AddWithValue("cutoffDate", cutoffDateTime);
                                    await command.ExecuteNonQueryAsync();
                                    Debug.WriteLine($"[성공] {table}에서 {cutoffDateTime} 이전 chunk 삭제 완료");
                                    successCount++;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"[실패] {table} 삭제 오류: {ex.Message}");
                                failCount++;
                            }
                        }

                        Debug.WriteLine($"=== Operational 테이블 자동 삭제 완료 ===");
                        Debug.WriteLine($"성공: {successCount}개, 실패: {failCount}개");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Operational 테이블 자동 삭제 오류: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Operational 테이블 자동 삭제 프로세스 오류: {ex.Message}");
            }
        }

        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (_autoDeleteTimer != null)
            {
                _autoDeleteTimer.Stop();
                _autoDeleteTimer.Dispose();
            }

            foreach (NpgsqlConnection connection in _connectionPool.Values)
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
            _connectionPool.Clear();
        }
        #endregion

        #region Connection Test & Monitoring
        /// <summary>
        /// 데이터베이스 연결 테스트
        /// </summary>
        public async Task<bool> TestConnection()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand("SELECT 1", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        return result != null && (int)result == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"연결 테스트 실패: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// 데이터베이스 버전 정보 조회
        /// </summary>
        public async Task<string> GetDatabaseVersion()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand("SELECT version()", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        return result?.ToString() ?? "Unknown";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"버전 조회 실패: {ex.Message}");
                return "Error: " + ex.Message;
            }
        }

        /// <summary>
        /// 데이터베이스 통계 조회
        /// </summary>
        public async Task<Dictionary<string, long>> GetDataStatistics()
        {
            var stats = new Dictionary<string, long>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var queries = new Dictionary<string, string>
                    {
                        ["분 아날로그 데이터"] = "SELECT COUNT(*) FROM operational.minute_analog_data",
                        ["시간 아날로그 데이터"] = "SELECT COUNT(*) FROM operational.hour_analog_data",
                        ["분 디지털 데이터"] = "SELECT COUNT(*) FROM operational.minute_digital_data",
                        ["시간 디지털 데이터"] = "SELECT COUNT(*) FROM operational.hour_digital_data",
                        ["경보 수"] = "SELECT COUNT(*) FROM operational.alarms",
                        ["로그 수"] = "SELECT COUNT(*) FROM operational.logs",
                        ["총 태그 수"] = "SELECT COUNT(*) FROM system.tags"
                    };

                    foreach (var query in queries)
                    {
                        try
                        {
                            using (var command = new NpgsqlCommand(query.Value, connection))
                            {
                                var result = await command.ExecuteScalarAsync();
                                stats[query.Key] = Convert.ToInt64(result);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"통계 조회 오류 ({query.Key}): {ex.Message}");
                            stats[query.Key] = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"데이터베이스 통계 조회 실패: {ex.Message}");
            }

            return stats;
        }

        /// <summary>
        /// 연결 문자열 유효성 검사
        /// </summary>
        public bool ValidateConnectionString()
        {
            try
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    Debug.WriteLine("연결 문자열이 비어있습니다.");
                    return false;
                }

                // 연결 문자열 파싱 테스트
                var builder = new NpgsqlConnectionStringBuilder(_connectionString);

                if (string.IsNullOrEmpty(builder.Host))
                {
                    Debug.WriteLine("호스트가 지정되지 않았습니다.");
                    return false;
                }

                if (string.IsNullOrEmpty(builder.Database))
                {
                    Debug.WriteLine("데이터베이스가 지정되지 않았습니다.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"연결 문자열 검증 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// PostgreSQL 서버 연결 테스트 (postgres 기본 DB 사용)
        /// </summary>
        public async Task<bool> TestServerConnection()
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder(_connectionString);
                builder.Database = "postgres"; // 기본 데이터베이스로 연결

                using (var connection = new NpgsqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand("SELECT 1", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        return result != null && (int)result == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"서버 연결 테스트 실패: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Query Optimization Methods

        /// <summary>
        /// 시간 범위별 데이터 조회 (TimescaleDB 최적화)
        /// </summary>
        public async Task<System.Data.DataTable> GetMinuteDataRange(
            string tagName, DateTime startTime, DateTime endTime)
        {
            var dt = new System.Data.DataTable();

            try
            {
                int tagId = await _tagRepo.GetOrCreateTagIdAsync(tagName);

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                    SELECT data_time, sum_min, average, min_value, max_value, curr_value
                    FROM operational.minute_analog_data
                    WHERE tag_id = @tagId 
                      AND data_time BETWEEN @startTime AND @endTime
                    ORDER BY data_time", connection))
                    {
                        command.Parameters.AddWithValue("tagId", tagId);
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
                Debug.WriteLine($"GetMinuteDataRange 오류: {ex.Message}");
            }

            return dt;
        }

        #endregion



        #region Schema & History Helper Methods

        /// <summary>
        /// 스키마별 테이블 목록 조회
        /// </summary>
        public async Task<List<string>> GetSchemaTableList(string schemaName)
        {
            var tables = new List<string>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                    SELECT table_name 
                    FROM information_schema.tables 
                    WHERE table_schema = @schemaName
                    ORDER BY table_name", connection))
                    {
                        command.Parameters.AddWithValue("schemaName", schemaName);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tables.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"스키마 테이블 목록 조회 오류: {ex.Message}");
            }

            return tables;
        }



        #endregion

        #region Health Check & Monitoring

        /// <summary>
        /// 스키마별 상태 확인
        /// </summary>
        public async Task<Dictionary<string, bool>> CheckSchemaHealth()
        {
            var health = new Dictionary<string, bool>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string[] schemas = {
                        SchemaArchitecture.SYSTEM_SCHEMA,
                        SchemaArchitecture.OPERATIONAL_SCHEMA,
                        SchemaArchitecture.HISTORY_SCHEMA
                    };

                    foreach (string schema in schemas)
                    {
                        try
                        {
                            using (var command = new NpgsqlCommand(
                                $"SELECT 1 FROM information_schema.schemata WHERE schema_name = '{schema}'",
                                connection))
                            {
                                var result = await command.ExecuteScalarAsync();
                                health[schema] = result != null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"스키마 {schema} 확인 오류: {ex.Message}");
                            health[schema] = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"스키마 상태 확인 실패: {ex.Message}");
            }

            return health;
        }

        /// <summary>
        /// TimescaleDB Hypertable 상태 확인
        /// </summary>
        public async Task<Dictionary<string, bool>> CheckHypertableStatus()
        {
            var status = new Dictionary<string, bool>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string[] tables = {
                        "operational.minute_analog_data",
                        "operational.hour_analog_data",
                        "operational.minute_digital_data",
                        "operational.hour_digital_data",
                        "operational.alarms",
                        "operational.logs"
                    };

                    foreach (string table in tables)
                    {
                        try
                        {
                            using (var command = new NpgsqlCommand(
                                $"SELECT 1 FROM timescaledb_information.hypertables WHERE hypertable_name = '{table.Split('.')[1]}'",
                                connection))
                            {
                                var result = await command.ExecuteScalarAsync();
                                status[table] = result != null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Hypertable {table} 확인 오류: {ex.Message}");
                            status[table] = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hypertable 상태 확인 실패: {ex.Message}");
            }

            return status;
        }

        #endregion
    }

    public static class SafeDateTimeConverter
    {
        /// <summary>
        /// DateTime의 Kind에 따라 적절히 로컬 시간으로 변환
        /// </summary>
        private static DateTime ConvertToLocalTime(DateTime dateTime)
        {
            switch (dateTime.Kind)
            {
                case DateTimeKind.Utc:
                    return dateTime.ToLocalTime();

                case DateTimeKind.Local:
                    return dateTime;

                case DateTimeKind.Unspecified:
                    // PostgreSQL에서 오는 대부분의 경우 UTC로 간주
                    return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();

                default:
                    return dateTime;
            }
        }
    }

    /// <summary>
    /// 로그 심각도 레벨
    /// </summary>
    public enum LogLevel
    {
        DEBUG = 0,      // 디버그 정보
        INFO = 1,       // 일반 정보
        WARNING = 2,    // 경고   , 시스템 정상, 사용자 조치 필요
        ERROR = 3,      // 오류 , 기능 제한
        CRITICAL = 4,   // 치명적
        FATAL = 5       // 시스템 중단
    }


    /// <summary>
    /// 로그 카테고리/타입
    /// </summary>
    public static class LogCategory
    {
        // 시스템 운영
        public const int SYSTEM = 100;
        public const int PROGRAM_START = 101;
        public const int PROGRAM_END = 102;

        public const int KEYLOCK = 103;

        public const int SYSTEM_START = 104;
        public const int SYSTEM_STOP = 105;
        public const int BACKUP = 106;
        public const int MAINTENANCE = 107;

        // 성능 모니터링 260226 PSU
        public const int PERFORMANCE = 108;

        // 보안/사용자 관련 - 모든 인증, 인가, 계정 관리
        public const int SECURITY = 200;
        public const int SECURITY_LOGIN = 201;           // 로그인 (성공/실패)
        public const int SECURITY_LOGOUT = 202;          // 로그아웃
        public const int SECURITY_USER_CREATE = 203;     // 사용자 생성
        public const int SECURITY_USER_MODIFY = 204;     // 사용자 수정
        public const int SECURITY_USER_DELETE = 205;     // 사용자 삭제
        public const int SECURITY_PASSWORD = 206;        // 암호 변경/초기화
        public const int SECURITY_ACCOUNT_LOCK = 207;    // 계정 잠금/해제
        public const int SECURITY_PERMISSION = 208;      // 권한/역할 변경
        public const int SECURITY_ACCESS_DENIED = 209;   // 접근 거부
        public const int SECURITY_UNAUTHORIZED = 210;    // 비인가 시도
        public const int SECURITY_SESSION = 211;         // 세션 관리


        // 데이터 관련
        public const int DATA = 400;
        public const int DATA_SAVE = 401;
        public const int DATA_DELETE = 402;
        public const int DATA_EXPORT = 403;
        public const int DATA_IMPORT = 404;

        // 통신/장비
        public const int COMMUNICATION = 500;
        public const int COMM_ERROR = 501;
        public const int COMM_RECOVER = 502;
        public const int DEVICE_ERROR = 503;
        public const int DEVICE_RECOVER = 504;


        // 태그/데이터 수집
        public const int TAG = 700;
        public const int TAG_VALUE_CHANGE = 701;
        public const int TAG_QUALITY_BAD = 702;



        // 경보
        public const int ALARM = 800;
        public const int ALARM_OCCURRED = 801;
        public const int ALARM_CLEARED = 802;
        public const int ALARM_ACKNOWLEDGED = 803;

        public static string GetCategoryName(int category)
        {
            // 100단위로 그룹 확인
            int group = (category / 100) * 100;

            switch (category)
            {
                // 시스템
                case PROGRAM_START: return "프로그램시작";
                case PROGRAM_END: return "프로그램종료";
                case SYSTEM_START: return "시스템시작";
                case SYSTEM_STOP: return "시스템종료";
                case BACKUP: return "백업";
                case MAINTENANCE: return "유지보수";

                // 보안 
                case SECURITY_LOGIN: return "로그인";
                case SECURITY_LOGOUT: return "로그아웃";
                case SECURITY_USER_CREATE: return "사용자생성";
                case SECURITY_USER_MODIFY: return "사용자수정";
                case SECURITY_USER_DELETE: return "사용자삭제";
                case SECURITY_PASSWORD: return "암호관리";
                case SECURITY_ACCOUNT_LOCK: return "계정잠금";
                case SECURITY_PERMISSION: return "권한관리";
                case SECURITY_ACCESS_DENIED: return "접근거부";
                case SECURITY_UNAUTHORIZED: return "비인가시도";
                case SECURITY_SESSION: return "세션관리";

                case DATA_SAVE: return "데이터저장";
                case DATA_DELETE: return "데이터삭제";
                case DATA_EXPORT: return "내보내기";
                case DATA_IMPORT: return "가져오기";

                case COMM_ERROR: return "통신오류";
                case COMM_RECOVER: return "통신복구";
                case DEVICE_ERROR: return "장비오류";
                case DEVICE_RECOVER: return "장비복구";

                case TAG_VALUE_CHANGE: return "태그값변경";
                case TAG_QUALITY_BAD: return "태그품질불량";

                case ALARM_OCCURRED: return "경보발생";
                case ALARM_CLEARED: return "경보해제";
                case ALARM_ACKNOWLEDGED: return "경보확인";

                default:
                    // 그룹명 반환
                    switch (group)
                    {
                        case SYSTEM: return "시스템";
                        case SECURITY: return "보안";
                        case DATA: return "데이터";
                        case COMMUNICATION: return "통신";
                        case TAG: return "태그";
                        case ALARM: return "경보";
                        default: return "알수없음";
                    }
            }
        }
    }


    /// <summary>
    /// 로그 엔트리
    /// </summary>
    public class LogEntry
    {
        public long Id { get; set; }
        public DateTime LogDateTime { get; set; }
        public LogLevel Level { get; set; }
        public int Category { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string IpAddress { get; set; }
        public string MachineName { get; set; }
        public string Detail { get; set; }  // JSON 등 상세정보
        public DateTime CreatedAt { get; set; }

        public string LevelName => Level.ToString();
        public string CategoryName => LogCategory.GetCategoryName(Category);
    }

    /// <summary>
    /// 로그 타입 정의
    /// </summary>
    public static class LogType
    {
        // 시스템 운영 관련
        public const int None = 0;
        public const int ProgramStart = 1;
        public const int ProgramEnd = 2;
        public const int WARNING = 3;
        public const int CRITICAL = 4;

        // 사용자 활동
        public const int USER_LOGIN = 10;
        public const int USER_LOGOUT = 11;
        public const int USER_ACTION = 12;

        // 시스템 이벤트
        public const int SYSTEM_START = 20;
        public const int SYSTEM_STOP = 21;
        public const int BACKUP = 22;
        public const int MAINTENANCE = 23;

        // 데이터 관련
        public const int DATA_SAVE = 30;
        public const int DATA_DELETE = 31;
        public const int DATA_EXPORT = 32;
        public const int DATA_IMPORT = 33;

        // 통신/장비
        public const int COMM_ERROR = 40;
        public const int COMM_RECOVER = 41;
        public const int DEVICE_ERROR = 42;
        public const int DEVICE_RECOVER = 43;

        // 보안
        public const int SECURITY_ALERT = 50;
        public const int ACCESS_DENIED = 51;
        public const int UNAUTHORIZED = 52;

        //오류
        public const int ERROR = 50000;
        public const int ErrorScript = 51000;

        /// <summary>
        /// 로그 타입 이름 반환
        /// </summary>
        public static string GetTypeName(int logType)
        {
            switch (logType)
            {
                case None: return "일반";
                case WARNING: return "경고";
                case ERROR: return "오류";
                case CRITICAL: return "치명적";
                case USER_LOGIN: return "로그인";
                case USER_LOGOUT: return "로그아웃";
                case USER_ACTION: return "사용자조작";
                case SYSTEM_START: return "시스템시작";
                case SYSTEM_STOP: return "시스템종료";
                case BACKUP: return "백업";
                case MAINTENANCE: return "유지보수";
                case DATA_SAVE: return "데이터저장";
                case DATA_DELETE: return "데이터삭제";
                case DATA_EXPORT: return "내보내기";
                case DATA_IMPORT: return "가져오기";
                case COMM_ERROR: return "통신오류";
                case COMM_RECOVER: return "통신복구";
                case DEVICE_ERROR: return "장비오류";
                case DEVICE_RECOVER: return "장비복구";
                case SECURITY_ALERT: return "보안경고";
                case ACCESS_DENIED: return "접근거부";
                case UNAUTHORIZED: return "비인가시도";
                default: return "알수없음";
            }
        }
    }

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