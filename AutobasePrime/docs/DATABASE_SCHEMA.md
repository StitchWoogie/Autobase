# AutobasePrime PostgreSQL 데이터베이스 스키마 문서

## 목차

1. [개요](#1-개요)
2. [연결 정보](#2-연결-정보)
3. [스키마 구조](#3-스키마-구조)
4. [system 스키마](#4-system-스키마)
5. [operational 스키마](#5-operational-스키마)
6. [history 스키마](#6-history-스키마)
7. [인덱스](#7-인덱스)
8. [TimescaleDB 정책](#8-timescaledb-정책)
9. [Enum/코드 값 정의](#9-enum코드-값-정의)
10. [SQL 예제](#10-sql-예제)

---

## 1. 개요

| 항목 | 값 |
|------|-----|
| DBMS | PostgreSQL 14+ |
| 확장 | TimescaleDB (시계열 데이터 최적화) |
| 데이터베이스명 | `autobase_db` |
| 스키마 | `system`, `operational`, `history` |
| DB 저장 타임존 | **UTC** |
| 표시 타임존 | `Asia/Seoul` (Local) |

### 타임존 규칙

> **DB에는 UTC로 저장하고, 읽을 때 ToLocalTime()으로 변환하여 사용한다.**

```
[저장 흐름] LocalTime → .ToUniversalTime() → DB (UTC)
[조회 흐름] DB (UTC) → .ToLocalTime() → 화면 표시 (Asia/Seoul)
```

- `TIMESTAMPTZ` 컬럼은 PostgreSQL 내부에서 UTC로 저장됨
- 연결 문자열의 `Timezone=Asia/Seoul`은 Npgsql 연결 세션의 시간대 설정
- C# 코드에서 저장 시 `DateTime.ToUniversalTime()` 호출 후 전달
- C# 코드에서 조회 시 `DateTime.ToLocalTime()` 호출 후 사용
- SQL에서 로컬 시간 비교 시: `AT TIME ZONE 'Asia/Seoul'` 사용

---

## 2. 연결 정보

| 항목 | 기본값 |
|------|--------|
| Host | `localhost` |
| Port | `5432` |
| Username | `admin` |
| Password | `admin` |
| Database | `autobase_db` |
| Timezone | `Asia/Seoul` (연결 세션 시간대) |
| Connection Timeout | 30초 |
| Command Timeout | 30초 |

> ⚠️ 연결 문자열의 Timezone은 Npgsql 세션 설정이며, **DB 내부 저장은 항상 UTC**입니다.

**연결 문자열 형식:**
```
Host=localhost;Port=5432;Username=admin;Password=admin;Database=autobase_db;Timezone=Asia/Seoul;Timeout=30;CommandTimeout=30;
```

설정 파일 경로: `PortalServer/PortalServerWeb/AutoWeb/Project/Config/pgDB.inix`

---

## 3. 스키마 구조

```
autobase_db
├── system          # 시스템 설정/메타데이터
│   ├── tags                  # 태그 마스터
│   ├── users                 # 사용자
│   ├── config                # 시스템 설정
│   └── millidata_config      # 미세자료 수집 설정
│
├── operational     # 실시간 SCADA 운영 데이터
│   ├── minute_analog_data    # 분 아날로그 (hypertable)
│   ├── hour_analog_data      # 시간 아날로그 (hypertable)
│   ├── minute_digital_data   # 분 디지털 (hypertable)
│   ├── hour_digital_data     # 시간 디지털 (hypertable)
│   ├── alarms                # 경보 (hypertable)
│   ├── logs                  # 로그 (hypertable)
│   └── log_metadata          # 로그 메타데이터
│
└── history         # 미세자료 (대용량 이력)
    ├── millidata_metadata    # 미세자료 테이블 메타
    ├── millidata_tag_metadata # 미세자료 태그 메타
    └── millidata_<이름>      # 동적 생성 hypertable
```

---

## 4. system 스키마

### 4.1 system.tags — 태그 마스터

태그(센서/계측점) 정보를 관리하는 마스터 테이블.

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `tag_id` | SERIAL | PK | 태그 고유 ID (자동증가) |
| `tag_name` | VARCHAR(100) | NOT NULL, UNIQUE | 태그명 |
| `description` | VARCHAR(200) | | 설명 |
| `data_type` | SMALLINT | NOT NULL | 데이터 유형 코드 |
| `full_scale` | REAL | | 풀스케일 값 |
| `created_at` | TIMESTAMP | DEFAULT NOW | 생성일시 |
| `updated_at` | TIMESTAMP | DEFAULT NOW | 수정일시 |

```sql
CREATE TABLE IF NOT EXISTS system.tags (
    tag_id SERIAL PRIMARY KEY,
    tag_name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(200),
    data_type SMALLINT NOT NULL,
    full_scale REAL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### 4.2 system.users — 사용자

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `id` | SERIAL | PK | 사용자 ID |
| `username` | VARCHAR(50) | NOT NULL, UNIQUE | 사용자명 |
| `password_hash` | VARCHAR(256) | NOT NULL | 암호 해시 |
| `password_mismatched_count` | INTEGER | DEFAULT 0 | 암호 틀린 횟수 |
| `use_auto_lock` | BOOLEAN | DEFAULT false | 자동 잠금 사용 여부 |
| `created_at` | TIMESTAMP | DEFAULT NOW | 생성일시 |
| `updated_at` | TIMESTAMP | DEFAULT NOW | 수정일시 |

```sql
CREATE TABLE IF NOT EXISTS system.users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(256) NOT NULL,
    password_mismatched_count INTEGER DEFAULT 0,
    use_auto_lock BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### 4.3 system.config — 시스템 설정

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `id` | SERIAL | PK | ID |
| `config_group` | VARCHAR(50) | NOT NULL | 설정 그룹 |
| `config_key` | VARCHAR(100) | NOT NULL | 설정 키 |
| `config_value` | TEXT | | 설정 값 |
| `description` | VARCHAR(200) | | 설명 |
| `created_at` | TIMESTAMP | DEFAULT NOW | 생성일시 |
| `updated_at` | TIMESTAMP | DEFAULT NOW | 수정일시 |

**UNIQUE 제약:** `(config_group, config_key)`

```sql
CREATE TABLE IF NOT EXISTS system.config (
    id SERIAL PRIMARY KEY,
    config_group VARCHAR(50) NOT NULL,
    config_key VARCHAR(100) NOT NULL,
    config_value TEXT,
    description VARCHAR(200),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(config_group, config_key)
);
```

### 4.4 system.millidata_config — 미세자료 수집 설정

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `id` | SERIAL | PK | ID |
| `title` | VARCHAR(200) | NOT NULL | 수집 제목 |
| `time_interval` | INTEGER | NOT NULL | 수집 주기 (ms) |
| `cut_method` | INTEGER | NOT NULL | 절단 방법 |
| `size_cut` | INTEGER | NOT NULL | 절단 크기 |
| `auto_delete` | BOOLEAN | DEFAULT false | 자동 삭제 여부 |
| `retention_days` | INTEGER | | 보존 일수 |
| `created_at` | TIMESTAMP | DEFAULT NOW | 생성일시 |
| `updated_at` | TIMESTAMP | DEFAULT NOW | 수정일시 |

```sql
CREATE TABLE IF NOT EXISTS system.millidata_config (
    id SERIAL PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    time_interval INTEGER NOT NULL,
    cut_method INTEGER NOT NULL,
    size_cut INTEGER NOT NULL,
    auto_delete BOOLEAN DEFAULT false,
    retention_days INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

---

## 5. operational 스키마

> 모든 데이터 테이블은 **TimescaleDB hypertable**로 관리됩니다.

### 5.1 operational.minute_analog_data — 분 아날로그 데이터

매 분마다 수집된 아날로그(계측) 데이터의 통계값을 저장.

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `tag_id` | INTEGER | PK (복합) | 태그 ID (→ system.tags) |
| `data_time` | TIMESTAMPTZ | PK (복합) | 데이터 시각 (분 단위 정규화) |
| `sum_min` | REAL | NOT NULL, DEFAULT 0 | 1분간 합계 |
| `average` | REAL | NOT NULL, DEFAULT 0 | 1분간 평균 |
| `min_value` | REAL | NOT NULL, DEFAULT 0 | 1분간 최솟값 |
| `max_value` | REAL | NOT NULL, DEFAULT 0 | 1분간 최댓값 |
| `curr_value` | REAL | NOT NULL, DEFAULT 0 | 현재값 |
| `created_at` | TIMESTAMPTZ | DEFAULT NOW | 생성일시 |

**PK:** `(tag_id, data_time)` · **Chunk interval:** 1일

```sql
CREATE TABLE IF NOT EXISTS operational.minute_analog_data (
    tag_id INTEGER NOT NULL,
    data_time TIMESTAMPTZ NOT NULL,
    sum_min REAL NOT NULL DEFAULT 0,
    average REAL NOT NULL DEFAULT 0,
    min_value REAL NOT NULL DEFAULT 0,
    max_value REAL NOT NULL DEFAULT 0,
    curr_value REAL NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (tag_id, data_time)
);

SELECT create_hypertable('operational.minute_analog_data', 'data_time',
    chunk_time_interval => INTERVAL '1 day',
    if_not_exists => TRUE, migrate_data => TRUE);
```

### 5.2 operational.hour_analog_data — 시간 아날로그 데이터

시간 단위로 집계된 아날로그 데이터.

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `tag_id` | INTEGER | PK (복합) | 태그 ID |
| `data_time` | TIMESTAMPTZ | PK (복합) | 데이터 시각 (시간 단위) |
| `sum_hour` | REAL | NOT NULL, DEFAULT 0 | 1시간 합계 |
| `avg_hour` | REAL | NOT NULL, DEFAULT 0 | 1시간 평균 |
| `min_hour` | REAL | NOT NULL, DEFAULT 0 | 1시간 최솟값 |
| `max_hour` | REAL | NOT NULL, DEFAULT 0 | 1시간 최댓값 |
| `curr_sum_meter` | REAL | NOT NULL, DEFAULT 0 | 적산 미터 값 |
| `flag` | BOOLEAN | DEFAULT true | 유효 플래그 |
| `created_at` | TIMESTAMPTZ | DEFAULT NOW | 생성일시 |

**PK:** `(tag_id, data_time)` · **Chunk interval:** 7일

```sql
CREATE TABLE IF NOT EXISTS operational.hour_analog_data (
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
);

SELECT create_hypertable('operational.hour_analog_data', 'data_time',
    chunk_time_interval => INTERVAL '7 days',
    if_not_exists => TRUE, migrate_data => TRUE);
```

### 5.3 operational.minute_digital_data — 분 디지털 데이터

매 분마다 수집된 디지털(접점) 데이터.

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `tag_id` | INTEGER | PK (복합) | 태그 ID |
| `data_time` | TIMESTAMPTZ | PK (복합) | 데이터 시각 (분 단위 정규화) |
| `count_on_off` | SMALLINT | NOT NULL, DEFAULT 0 | ON/OFF 전환 횟수 |
| `on_off_state` | BOOLEAN | NOT NULL, DEFAULT false | 현재 ON/OFF 상태 |
| `on_time` | SMALLINT | NOT NULL, DEFAULT 0 | ON 시간 (초) |
| `created_at` | TIMESTAMPTZ | DEFAULT NOW | 생성일시 |

**PK:** `(tag_id, data_time)` · **Chunk interval:** 1일

```sql
CREATE TABLE IF NOT EXISTS operational.minute_digital_data (
    tag_id INTEGER NOT NULL,
    data_time TIMESTAMPTZ NOT NULL,
    count_on_off SMALLINT NOT NULL DEFAULT 0,
    on_off_state BOOLEAN NOT NULL DEFAULT false,
    on_time SMALLINT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (tag_id, data_time)
);

SELECT create_hypertable('operational.minute_digital_data', 'data_time',
    chunk_time_interval => INTERVAL '1 day',
    if_not_exists => TRUE, migrate_data => TRUE);
```

### 5.4 operational.hour_digital_data — 시간 디지털 데이터

시간 단위로 집계된 디지털 데이터.

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `tag_id` | INTEGER | PK (복합) | 태그 ID |
| `data_time` | TIMESTAMPTZ | PK (복합) | 데이터 시각 (시간 단위) |
| `count_on_off` | INTEGER | NOT NULL, DEFAULT 0 | ON/OFF 전환 횟수 |
| `on_time` | INTEGER | NOT NULL, DEFAULT 0 | ON 시간 (초) |
| `flag` | BOOLEAN | DEFAULT true | 유효 플래그 |
| `created_at` | TIMESTAMPTZ | DEFAULT NOW | 생성일시 |

**PK:** `(tag_id, data_time)` · **Chunk interval:** 7일

```sql
CREATE TABLE IF NOT EXISTS operational.hour_digital_data (
    tag_id INTEGER NOT NULL,
    data_time TIMESTAMPTZ NOT NULL,
    count_on_off INTEGER NOT NULL DEFAULT 0,
    on_time INTEGER NOT NULL DEFAULT 0,
    flag BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (tag_id, data_time)
);

SELECT create_hypertable('operational.hour_digital_data', 'data_time',
    chunk_time_interval => INTERVAL '7 days',
    if_not_exists => TRUE, migrate_data => TRUE);
```

### 5.5 operational.alarms — 경보

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `id` | BIGSERIAL | 자동증가 | 경보 ID |
| `alarm_datetime` | TIMESTAMPTZ | NOT NULL | 경보 발생 시각 |
| `tag_name` | VARCHAR(100) | NOT NULL | 태그명 |
| `description` | VARCHAR(200) | | 태그 설명 |
| `message` | VARCHAR(200) | | 경보 메시지 |
| `alarm_type` | INTEGER | NOT NULL, CHECK ≥ 0 | 경보 유형 (→ EnumAlarmType) |
| `priority` | INTEGER | NOT NULL | 우선순위 (0~999) |
| `port` | INTEGER | NOT NULL, CHECK ≥ 0 | 포트 번호 |
| `station` | INTEGER | NOT NULL, CHECK ≥ 0 | 스테이션 번호 |
| `address` | BIGINT | NOT NULL, CHECK ≥ 0 | 주소 |
| `sub_type` | INTEGER | NOT NULL, CHECK ≥ 0 | 부가 유형 |
| `username` | VARCHAR(50) | | 조작자 |
| `ip_address` | VARCHAR(50) | | IP 주소 |
| `computer_name` | VARCHAR(100) | | 컴퓨터명 |
| `created_at` | TIMESTAMPTZ | DEFAULT NOW | 생성일시 |

**UNIQUE:** `(alarm_datetime, tag_name, alarm_type)` · **Chunk interval:** 7일

```sql
CREATE TABLE IF NOT EXISTS operational.alarms (
    id BIGSERIAL,
    alarm_datetime TIMESTAMPTZ NOT NULL,
    tag_name VARCHAR(100) NOT NULL,
    description VARCHAR(200),
    message VARCHAR(200),
    alarm_type INTEGER NOT NULL,
    priority INTEGER NOT NULL,
    port INTEGER NOT NULL,
    station INTEGER NOT NULL,
    address BIGINT NOT NULL,
    sub_type INTEGER NOT NULL,
    username VARCHAR(50),
    ip_address VARCHAR(50),
    computer_name VARCHAR(100),
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uk_alarm_time_tag_type UNIQUE (alarm_datetime, tag_name, alarm_type),
    CONSTRAINT chk_alarm_type CHECK (alarm_type >= 0),
    CONSTRAINT chk_port CHECK (port >= 0),
    CONSTRAINT chk_station CHECK (station >= 0),
    CONSTRAINT chk_address CHECK (address >= 0),
    CONSTRAINT chk_sub_type CHECK (sub_type >= 0)
);

SELECT create_hypertable('operational.alarms', 'alarm_datetime',
    chunk_time_interval => INTERVAL '7 days',
    if_not_exists => TRUE, migrate_data => TRUE);
```

### 5.6 operational.logs — 시스템 로그

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `id` | BIGSERIAL | 자동증가 | 로그 ID |
| `log_datetime` | TIMESTAMPTZ | NOT NULL, DEFAULT NOW | 로그 시각 |
| `level` | SMALLINT | NOT NULL, DEFAULT 1, CHECK 0~5 | 로그 레벨 (→ LogLevel) |
| `category` | INTEGER | NOT NULL, DEFAULT 100 | 카테고리 코드 (→ LogCategory) |
| `message` | TEXT | NOT NULL | 메시지 |
| `username` | VARCHAR(50) | | 사용자명 |
| `ip_address` | VARCHAR(50) | | IP 주소 |
| `machine_name` | VARCHAR(100) | | 컴퓨터명 |
| `detail` | JSONB | | 상세 정보 (JSON) |
| `created_at` | TIMESTAMPTZ | DEFAULT NOW | 생성일시 |

**Chunk interval:** 7일

```sql
CREATE TABLE IF NOT EXISTS operational.logs (
    id BIGSERIAL,
    log_datetime TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    level SMALLINT NOT NULL DEFAULT 1,
    category INTEGER NOT NULL DEFAULT 100,
    message TEXT NOT NULL,
    username VARCHAR(50),
    ip_address VARCHAR(50),
    machine_name VARCHAR(100),
    detail JSONB,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_log_level CHECK (level BETWEEN 0 AND 5)
);

SELECT create_hypertable('operational.logs', 'log_datetime',
    chunk_time_interval => INTERVAL '7 days',
    if_not_exists => TRUE, migrate_data => TRUE);
```

### 5.7 operational.log_metadata — 로그 메타데이터

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `id` | SERIAL | PK | ID |
| `log_date` | DATE | NOT NULL, UNIQUE | 로그 날짜 |
| `record_count` | INTEGER | DEFAULT 0 | 레코드 수 |
| `file_size_kb` | INTEGER | DEFAULT 0 | 파일 크기 (KB) |
| `created_at` | TIMESTAMPTZ | DEFAULT NOW | 생성일시 |
| `updated_at` | TIMESTAMPTZ | DEFAULT NOW | 수정일시 |

---

## 6. history 스키마

### 6.1 history.millidata_metadata — 미세자료 테이블 메타

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `table_name` | VARCHAR(100) | PK | 테이블명 |
| `title` | VARCHAR(200) | | 수집 제목 |
| `time_interval` | INTEGER | | 수집 주기 (ms) |
| `start_time` | TIMESTAMPTZ | | 시작 시각 |
| `end_time` | TIMESTAMPTZ | | 종료 시각 |
| `tag_count` | INTEGER | DEFAULT 0 | 태그 수 |
| `row_count` | INTEGER | DEFAULT 0 | 행 수 |
| `auto_delete` | BOOLEAN | DEFAULT false | 자동 삭제 |
| `retention_days` | INTEGER | | 보존 일수 |
| `created_at` | TIMESTAMPTZ | DEFAULT NOW | 생성일시 |
| `updated_at` | TIMESTAMPTZ | DEFAULT NOW | 수정일시 |

### 6.2 history.millidata_tag_metadata — 미세자료 태그 메타

| 컬럼 | 타입 | 제약 | 설명 |
|------|------|------|------|
| `table_name` | VARCHAR(100) | PK (복합) | 테이블명 |
| `tag_name` | VARCHAR(100) | PK (복합) | 태그명 |
| `tag_type` | INTEGER | | 태그 타입 (1=INTEGER, 9=TEXT, 기타=REAL) |
| `full_scale` | REAL | DEFAULT 0 | 풀스케일 |
| `base_value` | REAL | DEFAULT 0 | 기저값 |
| `column_name` | VARCHAR(100) | | DB 컬럼명 |

### 6.3 history.millidata_\<이름\> — 동적 미세자료 테이블

수집 설정마다 동적으로 생성되는 hypertable. `tag_type`에 따라 컬럼 타입이 결정됨.

| 고정 컬럼 | 타입 | 설명 |
|-----------|------|------|
| `data_time` | TIMESTAMPTZ | PK (복합) — 데이터 시각 |
| `data_count` | INTEGER | PK (복합) — 데이터 순번 |
| `millisec` | INTEGER | 밀리초 |
| `interval_ms` | INTEGER | 수집 간격 (ms) |
| `start_time` | TIMESTAMPTZ | 수집 시작 시각 |
| `created_at` | TIMESTAMPTZ | 생성일시 |
| (동적 태그 컬럼) | REAL / INTEGER / TEXT | 태그별 데이터 |

**Chunk interval:** 1시간

---

## 7. 인덱스

### 데이터 테이블 인덱스

```sql
-- 분 아날로그
CREATE INDEX idx_minute_analog_tag_time ON operational.minute_analog_data(tag_id, data_time);
CREATE INDEX idx_minute_analog_tag_time_desc ON operational.minute_analog_data(tag_id, data_time DESC);

-- 시간 아날로그
CREATE INDEX idx_hour_analog_tag_time ON operational.hour_analog_data(tag_id, data_time);
CREATE INDEX idx_hour_analog_tag_time_desc ON operational.hour_analog_data(tag_id, data_time DESC);

-- 분 디지털
CREATE INDEX idx_minute_digital_tag_time ON operational.minute_digital_data(tag_id, data_time);
CREATE INDEX idx_minute_digital_tag_time_desc ON operational.minute_digital_data(tag_id, data_time DESC);

-- 시간 디지털
CREATE INDEX idx_hour_digital_tag_time ON operational.hour_digital_data(tag_id, data_time);
CREATE INDEX idx_hour_digital_tag_time_desc ON operational.hour_digital_data(tag_id, data_time DESC);
```

### 경보 인덱스

```sql
CREATE INDEX idx_alarms_datetime ON operational.alarms(alarm_datetime DESC);
CREATE INDEX idx_alarms_tag_datetime ON operational.alarms(tag_name, alarm_datetime DESC);
CREATE INDEX idx_alarms_priority_datetime ON operational.alarms(priority, alarm_datetime DESC);
CREATE INDEX idx_alarms_type_datetime ON operational.alarms(alarm_type, alarm_datetime DESC);
CREATE INDEX idx_alarms_id_datetime ON operational.alarms(id, alarm_datetime DESC);
```

### 로그 인덱스

```sql
CREATE INDEX idx_logs_datetime ON operational.logs(log_datetime DESC);
CREATE INDEX idx_logs_level ON operational.logs(level);
CREATE INDEX idx_logs_category ON operational.logs(category);
CREATE INDEX idx_logs_level_datetime ON operational.logs(level, log_datetime DESC);
CREATE INDEX idx_logs_category_datetime ON operational.logs(category, log_datetime DESC);
CREATE INDEX idx_logs_id_datetime ON operational.logs(id, log_datetime DESC);
```

### 시스템/메타 인덱스

```sql
CREATE INDEX idx_tags_name ON system.tags(tag_name);
CREATE INDEX idx_millidata_metadata_start_time ON history.millidata_metadata(start_time);
CREATE INDEX idx_millidata_metadata_table_name ON history.millidata_metadata(table_name);
CREATE INDEX idx_millidata_tag_metadata_table_tag ON history.millidata_tag_metadata(table_name, tag_name);
```

---

## 8. TimescaleDB 정책

### 8.1 압축 정책

| 테이블 | Segment By | Order By | 압축 시점 |
|--------|-----------|----------|-----------|
| minute_analog_data | `tag_id` | `data_time DESC` | 7일 후 |
| hour_analog_data | `tag_id` | `data_time DESC` | 7일 후 |
| minute_digital_data | `tag_id` | `data_time DESC` | 7일 후 |
| hour_digital_data | `tag_id` | `data_time DESC` | 7일 후 |
| alarms | `tag_name, alarm_type` | `alarm_datetime DESC, id DESC` | 30일 후 |
| logs | `category` | `log_datetime DESC, id DESC` | 30일 후 |

### 8.2 데이터 보존 (Chunk 삭제)

```sql
-- 지정 날짜 이전 데이터 삭제
SELECT drop_chunks('operational.minute_analog_data', older_than => '2024-01-01'::timestamptz);
SELECT drop_chunks('operational.hour_analog_data',   older_than => '2024-01-01'::timestamptz);
-- 동일 패턴으로 모든 hypertable 적용 가능
```

---

## 9. Enum/코드 값 정의

### 9.1 EnumAlarmType — 경보 유형 (alarm_type 컬럼)

| 값 | 이름 | 설명 |
|----|------|------|
| 0 | HIHI | 상상한 초과 |
| 1 | HIGH | 상한 초과 |
| 2 | LOW | 하한 미달 |
| 3 | LOLO | 하하한 미달 |
| 4 | DI_ON | 디지털 ON 경보 |
| 5 | DI_OFF | 디지털 OFF 경보 |
| 6 | RETURN | 정상 복귀 |
| 7 | HAND_OPERATION | 수동 조작 |
| 8 | OVER_RATE_OF_CHANGE_LIMIT | 변화율 초과 |
| 9 | HAND_INPUT | 수동 기입 |
| 10 | DEMAND_CONTROL | Demand Control |
| 11 | GENERAL | 일반 경보 |
| 12 | CNF | 최소/최대값 확인 |

### 9.2 EnumAlarmSubType — 경보 부가 유형 (sub_type 컬럼)

| 값 | 이름 | 설명 |
|----|------|------|
| 0 | General_LOGIN | 로그인 |
| 1 | General_LOGOUT | 로그아웃 |

### 9.3 LogLevel — 로그 레벨 (level 컬럼)

| 값 | 이름 | 설명 |
|----|------|------|
| 0 | DEBUG | 디버그 정보 |
| 1 | INFO | 일반 정보 |
| 2 | WARNING | 경고 (시스템 정상, 사용자 조치 필요) |
| 3 | ERROR | 오류 (기능 제한) |
| 4 | CRITICAL | 치명적 오류 |
| 5 | FATAL | 시스템 중단 |

### 9.4 LogCategory — 로그 카테고리 (category 컬럼)

100 단위 그룹으로 분류. `category % 100 == 0`이면 그룹 전체를 의미.

| 코드 | 이름 | 설명 |
|------|------|------|
| **100** | **SYSTEM** | **시스템 운영 (그룹)** |
| 101 | PROGRAM_START | 프로그램 시작 |
| 102 | PROGRAM_END | 프로그램 종료 |
| 103 | KEYLOCK | 키 잠금 |
| 104 | SYSTEM_START | 시스템 시작 |
| 105 | SYSTEM_STOP | 시스템 종료 |
| 106 | BACKUP | 백업 |
| 107 | MAINTENANCE | 유지보수 |
| 108 | PERFORMANCE | 성능 모니터링 |
| **200** | **SECURITY** | **보안 (그룹)** |
| 201 | SECURITY_LOGIN | 로그인 (성공/실패) |
| 202 | SECURITY_LOGOUT | 로그아웃 |
| 203 | SECURITY_USER_CREATE | 사용자 생성 |
| 204 | SECURITY_USER_MODIFY | 사용자 수정 |
| 205 | SECURITY_USER_DELETE | 사용자 삭제 |
| 206 | SECURITY_PASSWORD | 암호 변경/초기화 |
| 207 | SECURITY_ACCOUNT_LOCK | 계정 잠금/해제 |
| 208 | SECURITY_PERMISSION | 권한/역할 변경 |
| 209 | SECURITY_ACCESS_DENIED | 접근 거부 |
| 210 | SECURITY_UNAUTHORIZED | 비인가 시도 |
| 211 | SECURITY_SESSION | 세션 관리 |
| **400** | **DATA** | **데이터 (그룹)** |
| 401 | DATA_SAVE | 데이터 저장 |
| 402 | DATA_DELETE | 데이터 삭제 |
| 403 | DATA_EXPORT | 내보내기 |
| 404 | DATA_IMPORT | 가져오기 |
| **500** | **COMMUNICATION** | **통신/장비 (그룹)** |
| 501 | COMM_ERROR | 통신 오류 |
| 502 | COMM_RECOVER | 통신 복구 |
| 503 | DEVICE_ERROR | 장비 오류 |
| 504 | DEVICE_RECOVER | 장비 복구 |
| **700** | **TAG** | **태그/데이터 수집 (그룹)** |
| 701 | TAG_VALUE_CHANGE | 태그 값 변경 |
| 702 | TAG_QUALITY_BAD | 태그 품질 불량 |
| **800** | **ALARM** | **경보 (그룹)** |
| 801 | ALARM_OCCURRED | 경보 발생 |
| 802 | ALARM_CLEARED | 경보 해제 |
| 803 | ALARM_ACKNOWLEDGED | 경보 확인 |

---

## 10. SQL 예제

### 10.1 태그 관리

```sql
-- 태그 조회
SELECT tag_id, tag_name, description, data_type, full_scale
FROM system.tags
ORDER BY tag_name;

-- 태그명으로 ID 조회
SELECT tag_id FROM system.tags WHERE tag_name = 'AI_Temperature_01';

-- 태그 등록 (중복 시 ID 반환)
INSERT INTO system.tags (tag_name, description, data_type, full_scale)
VALUES ('AI_Temperature_01', '보일러 온도', 0, 100.0)
ON CONFLICT (tag_name) DO NOTHING
RETURNING tag_id;

-- 태그 설명 수정
UPDATE system.tags
SET description = '1호기 보일러 온도', updated_at = CURRENT_TIMESTAMP
WHERE tag_name = 'AI_Temperature_01';
```

### 10.2 분 아날로그 데이터

> ⚠️ **data_time은 UTC로 저장됨.** 로컬 시간(KST)으로 조건 지정 시 `+09` 오프셋을 붙이거나 UTC로 변환.

```sql
-- 특정 태그의 기간 조회 (로컬 시간 → UTC 자동 변환)
-- '+09' 오프셋 지정하면 PostgreSQL이 자동으로 UTC 비교
SELECT t.tag_name,
       d.data_time AT TIME ZONE 'Asia/Seoul' AS local_time,  -- UTC → KST 변환 표시
       d.average, d.min_value, d.max_value, d.curr_value
FROM operational.minute_analog_data d
JOIN system.tags t ON t.tag_id = d.tag_id
WHERE t.tag_name = 'AI_Temperature_01'
  AND d.data_time >= '2026-02-01 00:00:00+09'   -- KST 2026-02-01 00:00 = UTC 2026-01-31 15:00
  AND d.data_time <  '2026-02-02 00:00:00+09'
ORDER BY d.data_time;

-- 여러 태그 동시 조회
SELECT t.tag_name,
       d.data_time AT TIME ZONE 'Asia/Seoul' AS local_time,
       d.average
FROM operational.minute_analog_data d
JOIN system.tags t ON t.tag_id = d.tag_id
WHERE t.tag_name IN ('AI_Temperature_01', 'AI_Pressure_01')
  AND d.data_time >= NOW() - INTERVAL '1 hour'
ORDER BY d.data_time, t.tag_name;

-- 데이터 삽입 (UTC로 저장, 중복 시 갱신)
-- C# 에서: DateTime.ToUniversalTime() 후 전달
INSERT INTO operational.minute_analog_data
    (tag_id, data_time, sum_min, average, min_value, max_value, curr_value)
VALUES
    (1, '2026-02-26 01:30:00+00', 150.5, 25.08, 24.5, 25.9, 25.3)  -- UTC 기준
ON CONFLICT (tag_id, data_time) DO UPDATE SET
    sum_min = EXCLUDED.sum_min,
    average = EXCLUDED.average,
    min_value = EXCLUDED.min_value,
    max_value = EXCLUDED.max_value,
    curr_value = EXCLUDED.curr_value;

-- 일간 시간대별 평균 (KST 기준 그룹핑)
SELECT date_trunc('hour', data_time AT TIME ZONE 'Asia/Seoul') AS kst_hour,
       AVG(average) AS avg_val,
       MIN(min_value) AS min_val,
       MAX(max_value) AS max_val
FROM operational.minute_analog_data
WHERE tag_id = 1
  AND data_time >= '2026-02-26 00:00:00+09'
  AND data_time <  '2026-02-27 00:00:00+09'
GROUP BY date_trunc('hour', data_time AT TIME ZONE 'Asia/Seoul')
ORDER BY kst_hour;

-- 최근 N건 조회
SELECT data_time AT TIME ZONE 'Asia/Seoul' AS local_time,
       average, curr_value
FROM operational.minute_analog_data
WHERE tag_id = 1
ORDER BY data_time DESC
LIMIT 10;
```

### 10.3 시간 아날로그 데이터

```sql
-- 일간 시간별 집계 조회
SELECT t.tag_name,
       h.data_time AT TIME ZONE 'Asia/Seoul' AS local_time,
       h.avg_hour, h.min_hour, h.max_hour, h.curr_sum_meter
FROM operational.hour_analog_data h
JOIN system.tags t ON t.tag_id = h.tag_id
WHERE t.tag_name = 'AI_Temperature_01'
  AND h.data_time >= '2026-02-01 00:00:00+09'
  AND h.data_time <  '2026-03-01 00:00:00+09'
ORDER BY h.data_time;

-- 일별 집계 (KST 기준 날짜 그룹핑)
SELECT (data_time AT TIME ZONE 'Asia/Seoul')::date AS kst_day,
       AVG(avg_hour) AS daily_avg,
       MIN(min_hour) AS daily_min,
       MAX(max_hour) AS daily_max
FROM operational.hour_analog_data
WHERE tag_id = 1
  AND data_time >= '2026-02-01 00:00:00+09'
  AND data_time <  '2026-03-01 00:00:00+09'
GROUP BY (data_time AT TIME ZONE 'Asia/Seoul')::date
ORDER BY kst_day;
```

### 10.4 분 디지털 데이터

```sql
-- 디지털 태그 상태 조회
SELECT t.tag_name,
       d.data_time AT TIME ZONE 'Asia/Seoul' AS local_time,
       d.on_off_state, d.count_on_off, d.on_time
FROM operational.minute_digital_data d
JOIN system.tags t ON t.tag_id = d.tag_id
WHERE t.tag_name = 'DI_Pump_01'
  AND d.data_time >= NOW() - INTERVAL '1 hour'
ORDER BY d.data_time;

-- 하루 ON 시간 합계 (초 → 분, KST 기준)
SELECT (data_time AT TIME ZONE 'Asia/Seoul')::date AS kst_day,
       SUM(on_time) / 60.0 AS on_minutes,
       SUM(count_on_off) AS total_switches
FROM operational.minute_digital_data
WHERE tag_id = 5
  AND data_time >= '2026-02-26 00:00:00+09'
  AND data_time <  '2026-02-27 00:00:00+09'
GROUP BY (data_time AT TIME ZONE 'Asia/Seoul')::date;

-- 디지털 데이터 삽입 (UTC로 저장)
INSERT INTO operational.minute_digital_data
    (tag_id, data_time, count_on_off, on_off_state, on_time)
VALUES
    (5, '2026-02-26 01:30:00+00', 3, true, 45)  -- UTC 기준
ON CONFLICT (tag_id, data_time) DO UPDATE SET
    count_on_off = EXCLUDED.count_on_off,
    on_off_state = EXCLUDED.on_off_state,
    on_time = EXCLUDED.on_time;
```

### 10.5 경보 조회

```sql
-- 최근 경보 조회 (UTC → KST 변환 표시)
SELECT alarm_datetime AT TIME ZONE 'Asia/Seoul' AS local_time,
       tag_name, message, alarm_type, priority
FROM operational.alarms
ORDER BY alarm_datetime DESC
LIMIT 50;

-- 기간별 특정 태그 경보 (KST 시간대로 조건 지정)
SELECT alarm_datetime AT TIME ZONE 'Asia/Seoul' AS local_time,
       message, alarm_type, priority, username
FROM operational.alarms
WHERE tag_name = 'AI_Temperature_01'
  AND alarm_datetime >= '2026-02-01 00:00:00+09'
  AND alarm_datetime <  '2026-03-01 00:00:00+09'
ORDER BY alarm_datetime DESC;

-- 경보 유형별 통계
SELECT alarm_type,
       COUNT(*) AS cnt,
       MIN(alarm_datetime) AT TIME ZONE 'Asia/Seoul' AS first_alarm,
       MAX(alarm_datetime) AT TIME ZONE 'Asia/Seoul' AS last_alarm
FROM operational.alarms
WHERE alarm_datetime >= NOW() - INTERVAL '7 days'
GROUP BY alarm_type
ORDER BY cnt DESC;

-- 우선순위별 미확인 경보
SELECT alarm_datetime AT TIME ZONE 'Asia/Seoul' AS local_time,
       tag_name, message, alarm_type, priority
FROM operational.alarms
WHERE priority >= 500
  AND alarm_datetime >= NOW() - INTERVAL '24 hours'
ORDER BY priority DESC, alarm_datetime DESC;

-- 경보 삽입 (C#에서 UTC 변환 후 전달, 중복 무시)
INSERT INTO operational.alarms
    (alarm_datetime, tag_name, description, message, alarm_type, priority,
     port, station, address, sub_type, username, ip_address, computer_name)
VALUES
    ('2026-02-26 01:30:15+00', 'AI_Temperature_01', '보일러 온도', 'HIHI 초과 (98.5)',
     0, 900, 1, 0, 100, 0, 'admin', '192.168.1.10', 'SCADA-01')
ON CONFLICT (alarm_datetime, tag_name, alarm_type) DO NOTHING;
```

### 10.6 로그 조회

```sql
-- 최근 로그 (UTC → KST 변환)
SELECT log_datetime AT TIME ZONE 'Asia/Seoul' AS local_time,
       level, category, message, username
FROM operational.logs
ORDER BY log_datetime DESC
LIMIT 100;

-- 오류 이상 레벨만 조회 (ERROR=3, CRITICAL=4, FATAL=5)
SELECT log_datetime AT TIME ZONE 'Asia/Seoul' AS local_time,
       level, category, message, detail
FROM operational.logs
WHERE level >= 3
  AND log_datetime >= NOW() - INTERVAL '24 hours'
ORDER BY log_datetime DESC;

-- 특정 카테고리 그룹 조회 (보안 200번대 전체)
SELECT log_datetime AT TIME ZONE 'Asia/Seoul' AS local_time,
       level, category, message, username, ip_address
FROM operational.logs
WHERE category >= 200 AND category < 300
  AND log_datetime >= '2026-02-01 00:00:00+09'
ORDER BY log_datetime DESC;

-- 로그인 이력 조회
SELECT log_datetime AT TIME ZONE 'Asia/Seoul' AS local_time,
       message, username, ip_address, machine_name
FROM operational.logs
WHERE category = 201  -- SECURITY_LOGIN
  AND log_datetime >= NOW() - INTERVAL '7 days'
ORDER BY log_datetime DESC;

-- KST 기준 날짜별 로그 통계 (C#: DataDBLocal.cs 패턴)
SELECT TO_CHAR(log_datetime AT TIME ZONE 'Asia/Seoul', 'YYYY-MM-DD') AS log_date,
       COUNT(*) AS record_count
FROM operational.logs
GROUP BY TO_CHAR(log_datetime AT TIME ZONE 'Asia/Seoul', 'YYYY-MM-DD')
ORDER BY log_date DESC;

-- 로그 삽입 (UTC로 저장)
INSERT INTO operational.logs
    (log_datetime, level, category, message, username, ip_address, machine_name, detail)
VALUES
    (NOW() AT TIME ZONE 'UTC', 1, 201, '사용자 로그인 성공', 'admin', '192.168.1.10', 'SCADA-01',
     '{"action": "login", "result": "success"}'::jsonb);

-- JSONB 상세 조건 조회
SELECT log_datetime AT TIME ZONE 'Asia/Seoul' AS local_time,
       message, detail
FROM operational.logs
WHERE detail @> '{"action": "login"}'::jsonb
  AND log_datetime >= NOW() - INTERVAL '7 days';
```

### 10.7 시스템 설정

```sql
-- 설정 조회
SELECT config_group, config_key, config_value, description
FROM system.config
ORDER BY config_group, config_key;

-- 특정 그룹 설정
SELECT config_key, config_value
FROM system.config
WHERE config_group = 'database';

-- 설정 추가/수정 (UPSERT)
INSERT INTO system.config (config_group, config_key, config_value, description)
VALUES ('database', 'retention_days', '365', '데이터 보존 일수')
ON CONFLICT (config_group, config_key) DO UPDATE SET
    config_value = EXCLUDED.config_value,
    updated_at = CURRENT_TIMESTAMP;
```

### 10.8 사용자 관리

```sql
-- 사용자 목록
SELECT id, username, use_auto_lock, password_mismatched_count, created_at
FROM system.users
ORDER BY username;

-- 잠금된 계정 조회
SELECT username, password_mismatched_count, updated_at
FROM system.users
WHERE password_mismatched_count >= 5 AND use_auto_lock = true;

-- 로그인 실패 횟수 초기화
UPDATE system.users
SET password_mismatched_count = 0, updated_at = CURRENT_TIMESTAMP
WHERE username = 'operator1';
```

### 10.9 미세자료 (MilliData) 조회

```sql
-- 미세자료 테이블 목록
SELECT table_name, title, time_interval, start_time, end_time, tag_count, row_count
FROM history.millidata_metadata
ORDER BY start_time DESC;

-- 특정 미세자료 테이블의 태그 목록
SELECT tag_name, tag_type, full_scale, column_name
FROM history.millidata_tag_metadata
WHERE table_name = 'millidata_boiler_01';

-- 미세자료 데이터 조회 (동적 테이블명)
SELECT data_time, data_count, millisec, ai_temperature_01, ai_pressure_01
FROM history.millidata_boiler_01
WHERE data_time BETWEEN '2026-02-26 10:00:00+09' AND '2026-02-26 10:05:00+09'
ORDER BY data_time, data_count;
```

### 10.10 통계/모니터링

```sql
-- 테이블별 레코드 수 확인
SELECT 'minute_analog'  AS tbl, COUNT(*) FROM operational.minute_analog_data
UNION ALL
SELECT 'hour_analog',          COUNT(*) FROM operational.hour_analog_data
UNION ALL
SELECT 'minute_digital',       COUNT(*) FROM operational.minute_digital_data
UNION ALL
SELECT 'hour_digital',         COUNT(*) FROM operational.hour_digital_data
UNION ALL
SELECT 'alarms',               COUNT(*) FROM operational.alarms
UNION ALL
SELECT 'logs',                 COUNT(*) FROM operational.logs
UNION ALL
SELECT 'tags',                 COUNT(*) FROM system.tags;

-- hypertable 청크 정보
SELECT hypertable_name, chunk_name, range_start, range_end,
       pg_size_pretty(total_bytes) AS size
FROM timescaledb_information.chunks
WHERE hypertable_schema = 'operational'
ORDER BY range_start DESC
LIMIT 20;

-- 압축 상태 확인
SELECT hypertable_name,
       number_compressed_chunks,
       before_compression_total_bytes,
       after_compression_total_bytes
FROM timescaledb_information.compression_settings cs
JOIN timescaledb_information.hypertable_compression_stats hcs
  ON cs.hypertable_schema = hcs.hypertable_schema
 AND cs.hypertable_name = hcs.hypertable_name
WHERE cs.hypertable_schema = 'operational';

-- DB 버전 확인
SELECT version();

-- TimescaleDB 버전
SELECT extversion FROM pg_extension WHERE extname = 'timescaledb';
```

### 10.11 데이터 정리 (Chunk 삭제)

```sql
-- 90일 이전 분 데이터 삭제
SELECT drop_chunks('operational.minute_analog_data',
    older_than => NOW() - INTERVAL '90 days');

SELECT drop_chunks('operational.minute_digital_data',
    older_than => NOW() - INTERVAL '90 days');

-- 1년 이전 시간 데이터 삭제
SELECT drop_chunks('operational.hour_analog_data',
    older_than => NOW() - INTERVAL '1 year');

-- 30일 이전 경보 삭제
SELECT drop_chunks('operational.alarms',
    older_than => NOW() - INTERVAL '30 days');

-- 미세자료 테이블 삭제
DROP TABLE IF EXISTS history.millidata_boiler_01;
DELETE FROM history.millidata_metadata WHERE table_name = 'millidata_boiler_01';
DELETE FROM history.millidata_tag_metadata WHERE table_name = 'millidata_boiler_01';
```

---

## ER 다이어그램 (텍스트)

```
system.tags (tag_id PK)
  │
  ├──< operational.minute_analog_data  (tag_id, data_time) PK
  ├──< operational.hour_analog_data    (tag_id, data_time) PK
  ├──< operational.minute_digital_data (tag_id, data_time) PK
  └──< operational.hour_digital_data   (tag_id, data_time) PK

operational.alarms     (alarm_datetime, tag_name, alarm_type) UNIQUE
operational.logs       (log_datetime, id)
operational.log_metadata (log_date UNIQUE)

history.millidata_metadata     (table_name PK)
  │
  ├──< history.millidata_tag_metadata (table_name, tag_name) PK
  └──< history.millidata_<이름>       (data_time, data_count) PK  [동적]
```
