# Autobase SCADA - Barcode / QR Code Subsystem Technical Design Specification

## 1. PURPOSE OF BARCODE/QR IN AUTOBASE SCADA

### Why Barcode/QR is Needed

Industrial SCADA platforms require barcode and QR code capabilities for:

- **Paperless Operation**: Eliminate manual data entry errors at operator stations
- **Traceability**: Track materials, lots, and products through the production process
- **Compliance**: Meet FDA 21 CFR Part 11, ISO 9001, and GMP traceability requirements
- **Speed**: Reduce changeover time by scanning recipe/lot codes instead of typing

### Typical Industrial Workflows

| Workflow | Description | Flow |
|----------|-------------|------|
| **LOT Start** | Operator scans LOT barcode to start production | Scan → Tag.LOT = value → Script.Run("StartProduction") |
| **Material Verification** | Scan material barcode to verify correct input | Scan → Validate against recipe → Accept/Reject |
| **Operator Login** | Scan employee QR badge for login | Scan → Extract operator ID → C_protect.LogInByUsername() |
| **Recipe Selection** | Scan recipe QR to load and execute recipe | Scan → Parse recipe name → RecipeManager.BatchStart() |
| **Machine Identification** | Scan machine barcode for setup | Scan → Tag.MACHINE_ID = value → Load machine config |
| **Production Traceability** | Log every scan event with timestamp and operator | Scan → ScanEventLogger → CSV audit trail |

### Interaction with SCADA Tags and Scripts

```
Scanner Input → OS → ScannerManager → BarcodeComplete Event
    ↓
BarcodeManager:
    1. Validate (length, prefix, regex)
    2. Update Tag (Tag.ResultTagName = scanned value)
    3. Fire BarcodeScanned event → Script engine
    4. Log scan event to CSV
```

---

## 2. SUPPORTED CODE TYPES

### Required Formats

| Format | Type | Use Case |
|--------|------|----------|
| **QR Code** | 2D Matrix | Structured data (LOT+LINE+ITEM), large data capacity |
| **DataMatrix** | 2D Matrix | Small labels, high-density industrial marking |
| **Code128** | 1D Linear | General purpose, alphanumeric |
| **Code39** | 1D Linear | Legacy systems, simple alphanumeric |

### Optional Format

| Format | Type | Use Case |
|--------|------|----------|
| **EAN13** | 1D Linear | Product identification (retail/warehouse) |

### Why QR/DataMatrix is Recommended for Industrial Environments

- **Damage Tolerance**: 2D codes with error correction survive scratches, dirt, oil
- **Data Capacity**: QR can hold up to 4,296 alphanumeric characters
- **Small Footprint**: DataMatrix encodes data in very small areas (important for PCBs, small parts)
- **Structured Data**: Support key-value and JSON formats for multi-field encoding
- **Error Correction**: QR Level Q recovers 25% damage — suitable for factory floor conditions

---

## 3. BARCODE GENERATION SYSTEM

### BarcodeGenerator Component

**File**: `BarcodeGenerator.cs`

#### Capabilities

- Generate barcode/QR from static text
- Generate barcode/QR from tag values via template binding
- Template-based generation using multiple tags (e.g., `LOT={Tag.LOT};LINE={Tag.LINE}`)
- Automatic update when bound tags change (`AutoUpdate` flag)
- Configurable image size and module size
- QR error correction level selection (L/M/Q/H)
- Export to PNG and BMP formats
- ZXing.Net integration with fallback rendering

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `BarcodeType` | BarcodeFormat | QRCode, DataMatrix, Code128, Code39, EAN13 |
| `TextSource` | string | Static text content |
| `TagBindingTemplate` | string | Template with `{Tag.xxx}` placeholders |
| `AutoUpdate` | bool | Re-generate on tag change |
| `Width` | int | Image width in pixels |
| `Height` | int | Image height in pixels |
| `ErrorCorrectionLevel` | QRErrorCorrectionLevel | L(7%), M(15%), Q(25%), H(30%) |
| `ModuleSize` | int | Cell size in pixels (0=auto) |

#### Template Example

```
LOT={Tag.LOT};LINE={Tag.LINE};DATE={Tag.PROD_DATE}
```

Resolves to:
```
LOT=230501;LINE=3;DATE=2026-03-09
```

---

## 4. SCANNER INPUT SYSTEM

### Supported Scanner Types

| Type | Connection | Data Flow |
|------|-----------|-----------|
| **USB Keyboard Wedge** | USB HID | Scanner → OS keyboard buffer → KeyPress event → ScannerManager |
| **USB Virtual COM** | USB-Serial | Scanner → COM port → SerialPort.DataReceived → ScannerManager |
| **RS232 Serial** | DB9/DB25 | Scanner → COM port → SerialPort.DataReceived → ScannerManager |

### Data Flow

```
Physical Scanner
    ↓
OS Input Layer (HID Keyboard or COM Port)
    ↓
ScannerManager
    ├─ KeyboardWedgeScanner (monitors KeyPress events)
    └─ SerialPortScanner (monitors COM DataReceived)
    ↓
BarcodeComplete Event
    ↓
BarcodeManager → Validate → Tag Update → Script → Log
```

### Input Handling

| Feature | Keyboard Wedge | Serial Port |
|---------|---------------|-------------|
| Prefix Detection | STX, custom chars | N/A (uses terminator) |
| Suffix/Terminator | CR, LF, ETX | CR+LF, CR, custom |
| Input Buffering | StringBuilder with timeout | StringBuilder with terminator |
| Scan Timeout | 50ms key-to-key interval | 200ms receive timeout |
| Min Length Filter | Configurable (default: 3) | Configurable (default: 3) |

### Event

```csharp
OnBarcodeScanned(BarcodeScannedEventArgs args)
    .RawCode        // Original scanned string
    .CleanCode      // Prefix/suffix removed
    .Timestamp      // Scan time
    .SourceType     // KeyboardWedge or SerialPort
    .ScannerId      // Scanner configuration name
    .ValidationResult // Valid, InvalidLength, etc.
    .IsValid        // Convenience boolean
```

---

## 5. SCANNER MANAGER ARCHITECTURE

### Structure

```
ScannerManager
    ├─ KeyboardWedgeScanner[0..n]    (USB keyboard wedge)
    │     ├─ Input buffer (StringBuilder)
    │     ├─ Prefix/suffix detection
    │     ├─ Key timeout timer
    │     └─ BarcodeScanned event
    └─ SerialPortScanner[0..n]       (RS232/USB serial)
          ├─ SerialPort instance
          ├─ Receive buffer (StringBuilder)
          ├─ Terminator detection
          ├─ Receive timeout timer
          └─ BarcodeScanned event
```

### Responsibilities

| Responsibility | Description |
|---------------|-------------|
| **Input Buffering** | Accumulate characters until scan completion |
| **Prefix/Suffix Detection** | Identify scanner-specific delimiters |
| **Barcode Completion** | Detect end of scan (terminator or timeout) |
| **Event Dispatching** | Unified `BarcodeScanned` event to BarcodeManager |
| **Scanner Enable/Disable** | Per-scanner and global enable control |

### Why Centralized Processing

Scanner processing MUST be centralized (not per-screen) because:

1. **Keyboard wedge scanners send input to the focused window** — if handled per-screen, switching screens loses scans
2. **Serial scanners are system-wide resources** — only one process can open a COM port
3. **Audit logging requires a single point of capture** — decentralized would miss events
4. **Tag updates must be atomic** — centralized ensures no race conditions

---

## 6. HMI OBJECT DESIGN

### BarcodeDisplayObject (for Screen Editor)

**Purpose**: Render a barcode/QR image on SCADA HMI screens

| Property | Type | Description |
|----------|------|-------------|
| BarcodeType | BarcodeFormat | Code format to generate |
| TextSource | string | Static text |
| TagBinding | string | Template with `{Tag.xxx}` |
| AutoUpdate | bool | Re-render on tag change |
| Size | Size | Display size |
| ErrorCorrectionLevel | QRErrorCorrectionLevel | QR EC level |

### BarcodeScannerObject (for Screen Editor)

**Purpose**: Display scanner status and last scan result on HMI screens

| Property | Type | Description |
|----------|------|-------------|
| Enable | bool | Scanner active |
| Prefix | string | Expected prefix |
| Suffix | string | Expected suffix |
| ResultTag | string | Tag to store scan result |
| ValidationRule | ValidationConfig | Validation settings |

| Event | Description |
|-------|-------------|
| OnBarcodeScanned | Fires when valid barcode received |
| OnValidationFailed | Fires when validation fails |

---

## 7. TAG AND SCRIPT INTEGRATION

### Typical Workflow

```
SCAN "LOT230501;LINE=3"
    ↓
Parse → Tag.BARCODE_RAW = "LOT230501;LINE=3"
    ↓
Tag.LOT = "LOT230501" (via ExtractValue)
Tag.LINE = "3"
    ↓
Script.Run("OnBarcodeReceived")
    ↓
Process Control (start batch, verify material, etc.)
```

### Script Functions (available in Autobase script engine)

| Function | Return | Parameters | Description |
|----------|--------|------------|-------------|
| `@BarcodeGetLastScan` | string | scanner_name | Get last scan value |
| `@BarcodeExtractValue` | string | barcode_text, key | Extract key from structured data |
| `@BarcodeScannerEnable` | void | scanner_name, enabled | Enable/disable scanner |
| `@BarcodeGenerate` | int | text, format | Generate barcode image |
| `@BarcodeExportImage` | int | generator_name, file_path | Export to file |

### Script Example

```
// LOT 스캔 시 생산 시작 스크립트
lot = @BarcodeGetLastScan("Scanner1")
if lot <> "" then
    @SetTagValue("LOT_NUMBER", lot)
    line = @BarcodeExtractValue(lot, "LINE")
    @SetTagValue("LINE_NUMBER", line)
    @RecipeBatchStart("MainRecipe", lot)
end if
```

---

## 8. BARCODE VALIDATION

### Validation Mechanisms

| Type | Description | Example |
|------|-------------|---------|
| **Length** | Min/max character count | Min=6, Max=20 |
| **Prefix** | Required string prefix | "LOT" |
| **Regex** | Regular expression pattern | `LOT[0-9]{6}` |
| **Empty Check** | Reject empty/whitespace | Automatic |

### Configuration Example

```json
{
  "Enabled": true,
  "MinLength": 6,
  "MaxLength": 50,
  "RequiredPrefix": "LOT",
  "RegexPattern": "LOT[0-9]{5,}"
}
```

### Why Validation is Critical

- **Wrong material in wrong machine** — can cause product defects, equipment damage
- **Invalid lot codes** — break traceability chain for recalls
- **Stale/damaged barcodes** — partial reads must be rejected, not processed
- **Security** — prevent injection of malicious data through barcode content

---

## 9. QR DATA FORMAT DESIGN

### Key-Value Format (Recommended)

```
LOT=230501;ITEM=VALVE;LINE=3;OP=Kim
```

- Simple to parse
- Human-readable
- Compatible with legacy systems
- Compact encoding

### JSON Format

```json
{"lot":"230501","item":"VALVE","line":"3","op":"Kim"}
```

- Standard format for MES/ERP integration
- Supports nested data
- Wide tool support

### Advantages for Traceability and MES Integration

- **Structured data** enables automatic routing and verification
- **Multiple fields** in single scan reduces operator steps
- **Standard formats** enable direct MES API calls
- **Bidirectional**: SCADA can generate QR for downstream systems

---

## 10. OPTIONAL IMAGE DECODING

### Supported Sources

| Source | Method | Use Case |
|--------|--------|----------|
| Image File | `BarcodeDecoder.DecodeFromFile()` | QC verification of printed labels |
| Bitmap | `BarcodeDecoder.DecodeFromBitmap()` | Camera frame processing |
| Screen Capture | Via Bitmap capture | Testing and verification |

### Implementation

Uses **ZXing.Net** (dynamically loaded) for decoding. If ZXing.Net is not available, decoding functions return null with a debug message.

**Note**: Dedicated hardware scanners perform their own decoding internally and are the recommended approach for production use. Image decoding is supplementary.

---

## 11. SCAN EVENT LOGGING

### Log Fields

| Field | Type | Description |
|-------|------|-------------|
| Timestamp | DateTime | Scan time (ms precision) |
| Operator | string | Logged-in user name |
| BarcodeValue | string | Cleaned barcode content |
| ValidationResult | enum | Valid, InvalidLength, etc. |
| ScannerId | string | Scanner configuration name |
| SourceType | enum | KeyboardWedge, SerialPort |
| ResultTagName | string | Target tag name |
| ResultTagValue | string | Value written to tag |
| ScriptExecuted | string | Script triggered by scan |

### Log File Format

- **Location**: `{StartupPath}\BarcodeLog\ScanLog_YYYYMMDD.csv`
- **Format**: UTF-8 CSV with header
- **Rotation**: Daily file rotation
- **Retention**: Configurable (default: 90 days, auto-cleanup)

### Why Logging is Critical

- **Regulatory Compliance**: FDA, ISO require complete production records
- **Recall Support**: Trace all materials to affected products
- **Audit Trail**: Who scanned what, when, where
- **Quality Investigation**: Correlate defects with material lots
- **Non-Repudiation**: Timestamped operator actions

---

## 12. INDUSTRIAL BEST PRACTICES

| Practice | Rationale |
|----------|-----------|
| Use QR/DataMatrix over 1D codes | Higher data capacity, error correction, smaller footprint |
| Configure Prefix/Suffix per scanner | Distinguish scanner input from keyboard typing |
| Use dedicated SCADA terminals | Prevent scanner input going to wrong application |
| Validate before process actions | Never execute production steps on invalid scans |
| Centralize scanner management | Single point of control and audit |
| Log every scan event | Traceability, compliance, troubleshooting |
| Use Error Correction Level Q or H | Factory environments have dust, oil, vibration |
| Test scanner configuration offline | Verify prefix/suffix, validation rules before production |
| Keep barcode content structured | Key-value or JSON for machine processing |
| Monitor scanner health | Alert on scan failures, disconnections |

---

## 13. AUTOBASE IMPLEMENTATION GUIDELINES

### Integration Architecture

```
C_init.ViewProgrammStart()
    ↓
BarcodeManager.Init()
    ├─ Load BarcodeConfig.json
    ├─ ScannerManager.Initialize()
    │     ├─ KeyboardWedgeScanner[] (per config)
    │     └─ SerialPortScanner[] (per config, opens COM ports)
    ├─ ScanEventLogger.Start()
    └─ BarcodeGenerator[] (per config)

FormLocalMain KeyPreview
    ↓
BarcodeManager.ProcessKeyPress()  → keyboard wedge input
BarcodeManager.ProcessKeyDown()   → Enter key detection

ScriptExternalRun.PrepareAll()
    ↓
ScriptFunctionBarcode.PrepareMethod()
    → @BarcodeGetLastScan, @BarcodeExtractValue, etc.

FormLocalMain Menu
    ↓
FormBarcodeConfig (Settings dialog)
    → Scanner setup, Generator setup, Log viewer, Test tools

C_init.ViewProgrammExit()
    ↓
BarcodeManager.Shutdown()
    ├─ ScannerManager.Dispose() (close COM ports)
    ├─ ScanEventLogger.Dispose() (flush remaining logs)
    └─ BarcodeGenerator[].Dispose()
```

### File Structure

```
LocalMain/Barcode/
    ├─ BarcodeTypes.cs          # Enums, event args, log record
    ├─ BarcodeConfig.cs         # Configuration classes
    ├─ BarcodeValidator.cs      # Validation logic
    ├─ BarcodeGenerator.cs      # Image generation (ZXing.Net + fallback)
    ├─ BarcodeDecoder.cs        # Image decoding (optional, ZXing.Net)
    ├─ QRDataParser.cs          # Key-value and JSON parsing
    ├─ KeyboardWedgeScanner.cs  # Keyboard wedge input handler
    ├─ SerialPortScanner.cs     # Serial port input handler
    ├─ ScannerManager.cs        # Central scanner management
    ├─ ScanEventLogger.cs       # CSV audit logging
    ├─ BarcodeManager.cs        # Top-level orchestrator
    └─ FormBarcodeConfig.cs     # Settings UI

ViewMain/GraphicModule/Script/
    └─ ScriptFunctionBarcode.cs # Script engine integration
```

### NuGet Dependencies

| Package | Required | Purpose |
|---------|----------|---------|
| **ZXing.Net** | Optional | High-quality barcode generation and decoding |
| System.IO.Ports | Built-in (.NET 4.8) | Serial port communication |
| Newtonsoft.Json | Already in project | Configuration serialization |

### Configuration File

**Path**: `{StartupPath}\BarcodeConfig.json`

```json
{
  "Enabled": true,
  "EnableScanLog": true,
  "ScanLogRetentionDays": 90,
  "Scanners": [
    {
      "Name": "Scanner1",
      "Enabled": true,
      "InputType": 0,
      "Prefix": "",
      "Suffix": "\r",
      "KeyTimeoutMs": 50,
      "MinLength": 3,
      "ResultTagName": "BARCODE_SCAN",
      "Validation": {
        "Enabled": true,
        "MinLength": 6,
        "RequiredPrefix": "LOT",
        "RegexPattern": "LOT[0-9]{5,}"
      }
    }
  ],
  "Generators": [
    {
      "Name": "LotLabel",
      "Format": 0,
      "TagBindingTemplate": "LOT={Tag.LOT};LINE={Tag.LINE}",
      "AutoUpdate": true,
      "Width": 200,
      "Height": 200,
      "ErrorCorrectionLevel": 2
    }
  ]
}
```

---

## Design Principles

1. **Reliability**: Scan events are never lost — even on validation failure, they are logged
2. **Traceability**: Every scan is recorded with timestamp, operator, and result
3. **Maintainability**: Clean separation between scanner input, validation, tag update, and logging
4. **Non-Blocking**: Scanner processing does not block the UI thread or SCADA engine
5. **Graceful Degradation**: If ZXing.Net is not installed, fallback rendering works; if no scanners configured, system runs without impact
