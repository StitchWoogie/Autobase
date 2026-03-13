# Excel Worker Architecture Plan

## Goal
- Preserve existing Excel-template behavior.
- Preserve user-authored Excel forms, formulas, VBA, and Excel-specific output.
- Support web-triggered report generation without running Excel inside IIS.

## Why This Architecture
- Users want the same result as local Excel execution.
- Existing templates depend on real Excel behavior.
- `IIS + Excel DCOM` is still operationally fragile.
- A dedicated Excel Worker isolates risk while preserving compatibility.

## Recommended Architecture
```text
Web / Local / Scheduler
        ->
Report Request API
        ->
Queue / Job Store
        ->
Excel Worker Service
        ->
Interactive Windows Session + Excel
        ->
Result File Store
        ->
Download / View API
```

## Core Principles
- Never run Excel in IIS worker process.
- Never rely on DCOM automation inside the web application.
- Run Excel only inside a dedicated worker host on Windows.
- Keep Excel execution isolated per worker instance.
- Make job execution observable and restartable.

## Components
### 1. Report Request API
- Receives report generation requests.
- Validates template path, period, caller identity, and report options.
- Creates a job record and returns a job ID.

### 2. Job Store
- Stores:
  - job ID
  - requested template
  - parameters
  - status
  - worker assignment
  - result path
  - error details

### 3. Excel Worker Service
- Windows service or worker launcher.
- Polls queued jobs.
- Starts or attaches to a controlled Excel execution session.
- Executes legacy Excel report logic.
- Uploads or registers result files.

### 4. Worker Session Host
- Dedicated Windows user profile.
- Fixed Office version.
- Required add-ins and template dependencies preinstalled.
- Popup suppression and watchdog management applied.

### 5. Result Delivery
- File share, object store, or application-managed result folder.
- API returns download URL or streams the file.

## Execution Flow
1. Client requests a report.
2. API creates a queued job.
3. Worker claims the job.
4. Worker prepares runtime input.
5. Worker launches Excel in its controlled session.
6. Existing Excel report automation runs.
7. Worker waits for completion and validates output.
8. Worker stores file and updates job status.
9. Client polls or receives completion event.

## How Existing Code Should Evolve
### Current State
- `ScriptFunctionExcel` still models the legacy local/web execution split.
- The legacy web path already delegates report generation to a service boundary.

### Near-Term Direction
- Keep local Excel behavior unchanged.
- Replace the current web-side Excel generation implementation with an Excel Worker-backed job service.
- Web should submit work, not execute Excel.

## Integration Direction for Existing Projects
### LocalMain / ViewMain
- Keep current local Excel execution for compatibility.
- Optionally add a setting:
  - `Run locally with Excel`
  - `Submit to worker`

### Portal / Web
- Switch from direct service execution to:
  - enqueue job
  - query job status
  - fetch result

### ExcelReportData
- Keep only if local Excel add-in is still required by the worker execution path.
- Do not treat it as a web runtime dependency.

## Worker Contract
### Job Request
- report template path
- time range
- output options
- print/save/close behavior
- caller metadata
- string variable dictionary

### Job Result
- status
- result file path or URL
- start/end timestamps
- worker machine
- Excel version
- failure log

## Operational Controls
### Required
- per-job timeout
- Excel process watchdog
- orphan process cleanup
- worker health probe
- retry policy only for safe failures

### Recommended
- one job at a time per worker session
- worker pool instead of one shared Excel instance
- dedicated worker VM or RDS session per capacity unit

## Security Controls
- signed template locations only
- restricted macro trust policy
- least-privilege worker account
- isolated result directory per tenant/site if needed
- audit trail for who generated which report

## Migration Plan
### Step 1
- Restore current Excel report runtime to legacy Excel behavior locally.

### Step 2
- Define job table and worker API contract.

### Step 3
- Implement worker host project:
  - dequeue
  - execute
  - store result
  - update status

### Step 4
- Switch web flow from direct execution to job submission.

### Step 5
- Add monitoring, retries, timeout handling, and worker pool scaling.

## What Not To Do
- Do not run Excel directly in IIS.
- Do not mix OpenXML-based output with templates that require VBA parity.
- Do not expect server-side parallel scale from a single Excel instance.

## Decision
- Existing Excel reports should remain Excel-executed.
- Web-triggered Excel reports should move to a dedicated Excel Worker model.
- `OpenXML + ClosedXML` remains valuable, but for non-Excel-dependent report products and native-report exports.
