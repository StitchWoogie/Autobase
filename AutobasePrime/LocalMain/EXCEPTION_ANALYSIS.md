# LocalMain Unhandled Exception Analysis

## 1. Global Exception Handlers DISABLED (CRITICAL)

**`Program.cs:1`** - `#define USE_EXCEPTION_REPORT` is **commented out**, disabling all 3 global handlers:

| Handler | Line | Purpose | Status |
|---|---|---|---|
| `Application.ThreadException` | 62 | UI thread exceptions | **DISABLED** |
| `AppDomain.UnhandledException` | 66 | Background thread exceptions | **DISABLED** |
| `TaskScheduler.UnobservedTaskException` | 69 | Unobserved Task exceptions | **DISABLED** |

All unhandled exceptions below will cause an **immediate process crash**.

---

## 2. async void Methods Without try-catch (CRITICAL)

`async void` exceptions cannot be caught by callers. With global handlers disabled, they crash the process.

| File | Method | Line |
|---|---|---|
| `FormLocalMain.cs` | `FilelogInToolStripMenuItem_Click` | 152 |
| `FormLocalMain.cs` | `FilelogOutToolStripMenuItem_Click` | 157 |
| `FormLocalMain.cs` | `FormLocalMain_FormClosed` | 2185 |
| `Recipe/FormRecipeImport.cs` | `LoadAndCompare()` | 186 |
| `Recipe/RecipeManager.cs` | `ReLoad()` | 26 |
| `DataSave/TrendSaveManager.cs` | `OnDbStateChanged()` | 581 |

**Total async void methods in project: 41** (some have try-catch protection).

---

## 3. Thread Entry Points Without try-catch (HIGH)

| File | Method | Line | Risk |
|---|---|---|---|
| `CheckEngineTagChangeThread.cs` | `ThreadLoopSharedTag()` | 161 | NullRef possible at line 184-186 |
| `Alarm/AlarmMail.cs` | `ThreadLoop()` | 40 | `SendMail` exception kills thread |

---

## 4. Main Timer Single Point of Failure (HIGH)

`timerMain_Tick` (line 1331) has try-catch, but on exception sets `_excetpionFlag = true`, permanently stopping the timer and **all monitoring/control functions**:

```
timerMain_Tick -> PlanToolStatusLocal -> all engine calls:
  CheckEngineTagChange.CheckSignalChange()
  CheckEngineTimeChange.CheckTimeChange()
  CheckEngineAlwaysScript.PclProgrammStatus()
  FormConfigFlowView.FlowViewStatus()
  AlarmConfirm.AlarmConfirmationStatus()
  C_DdeTag.ConnectTry()
  CheckEngineTagChange.CheckAiDiSubTag()
  CheckEngineNetworkToViewMain.Check()
  AlarmToDigitalOut.CheckAlarmStatusToDigitalOutList()
  CheckEngineDemandControl.FunctionBlockDemandControlStatus()
  CheckEngineDemandNew.TickAsync()
  CheckEngineMinuteChanged.StatusRemainTrendSave()
  CheckEngineMilliData.CheckMilliData()
  CheckEngineSchedule.CheckScheduleStatus()
  ReportAutoPrint.AutoPrintCheck()
  PlcScanEvent.Check()
```

Any single exception in any engine -> **all monitoring stops**.

---

## 5. Silent Exception Swallowing (HIGH)

| File | Line | Context |
|---|---|---|
| `PythonAi/FormPythonAiDashboard.cs` | 183, 372, 420, 473 | `catch { }` |
| `PythonAi/PythonAiTcpConnection.cs` | 110, 136, 143 | `catch { }` |
| `OPCUA/OPCUAServerMain.cs` | 156 | `catch { }` |

---

## 6. Null Reference Risks (MEDIUM)

| File | Line | Code |
|---|---|---|
| `CheckEngineTagChangeThread.cs` | 184-186 | `tp = TagLib.GetStructPublic(...)` -> `tp.bNeedSharedTagUpdate` (no null check) |
| `AlarmMail.cs` | 67 | `alarm = arrayAlarm[0]` (Count check outside lock) |
| `FormLocalMain.cs` | 2275 | `CultureInfo.DefaultThreadCurrentUICulture.Name` (can be null) |

---

## 7. Unprotected Task.Run (MEDIUM)

| File | Line |
|---|---|
| `SmLog.cs` | 66 |
| `PythonAi/PythonAiManager.cs` | 84 |
| `CheckEngineAutoDeleteThread.cs` | 69 |
| `DemandNew/DemandCsvExporter.cs` | 171, 191, 211 |

---

## Summary

| Category | Count | Severity |
|---|---|---|
| Global handlers disabled | 3 handlers | **CRITICAL** |
| async void (unprotected) | 6+ | **CRITICAL** |
| Thread (no try-catch) | 2 | **HIGH** |
| Main timer SPOF | 1 | **HIGH** |
| Silent catch blocks | 6+ | **HIGH** |
| Null reference risks | 5+ | **MEDIUM** |
| Unprotected Task.Run | 6+ | **MEDIUM** |

## Recommended Fixes (Priority Order)

1. **Uncomment `#define USE_EXCEPTION_REPORT`** in Program.cs to enable global handlers
2. **Wrap individual engine calls** in `PlanToolStatusLocal` with separate try-catch to isolate failures
3. **Add try-catch** to `ThreadLoopSharedTag()` and `AlarmMail.ThreadLoop()`
4. **Add try-catch** to all unprotected `async void` event handlers
5. **Replace empty `catch {}`** blocks with exception logging
