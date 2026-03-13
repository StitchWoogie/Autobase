# AutobaseOpcUaClient

## Project Overview
OPC UA Client application developed with .NET Framework 4.8

## Architecture
The solution consists of multiple projects organized by functionality:

### Abstractions Layer
- **OpcUa.Client.Abstractions**: Interfaces and abstractions for OPC UA client
- **OPCUAClient.Abstractions**: Additional abstraction layer

### Core Layer
- **OPCUA.Client.Core**: Core business logic and OPC UA communication implementation

### Host Layer
- **OpcUa.Client.Host**: Application hosting, configuration, and lifecycle management

### IPC (Inter-Process Communication) Layer
- **OpcUa.Client.Ipc.Server**: IPC server for cross-process communication
- **OpcUaClient.Ipc.Client**: IPC client for cross-process communication
- **OpcUaClient.Ipc.Contracts**: Shared contracts and interfaces for IPC

### UI Layer
- **OPCUA.Client.UI**: User interface layer

## Technology Stack
- .NET Framework 4.8
- OPC UA SDK
- C#
- IPC communication infrastructure

## Build & Run
```
# Build solution
msbuild AutobaseOpcUaClient.sln /p:Configuration=Release

# Or use Visual Studio 2019/2022
```

## Project Structure
```
AutobasePrime/
├── OpcUa.Client.Abstractions/      # Core interfaces and abstractions
├── OPCUAClient.Abstractions/       # Additional abstractions
├── OPCUA.Client.Core/              # Core business logic and OPC UA implementation
├── OpcUa.Client.Host/              # Application hosting and configuration
├── OpcUa.Client.Ipc.Server/        # IPC server
├── OpcUaClient.Ipc.Client/         # IPC client
├── OpcUaClient.Ipc.Contracts/      # IPC contracts and interfaces
├── OPCUA.Client.UI/                # User interface
└── CLAUDE.md                       # Project documentation
```

## Scope Restrictions
Only files within the above project folders should be accessed and modified.
