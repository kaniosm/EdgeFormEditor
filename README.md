# EdgeFormEditor

`EdgeFormEditor` is a lightweight Windows Forms utility for viewing and managing Microsoft Edge autofill entries from the local `Web Data` SQLite database.

## Features

- Filter autofill records by `Name` and `Value`
- Sort grid columns by clicking headers
- Mark rows for deletion with red highlighting
- Commit pending deletions only when `Save` is clicked
- Startup warning with optional forced Edge shutdown to prevent lock/read-only errors

## Data Source

The app works with Edge’s local profile database at:

`%LOCALAPPDATA%\Microsoft\Edge\User Data\Default\Web Data`

## Tech Stack

- `.NET 10`
- `Windows Forms`
- `Entity Framework Core (SQLite)`

## Distribution

### Quick Build (Recommended - Works without Windows SDK)

Build a ready-to-distribute single executable:

```powershell
dotnet publish EdgeFormEditor\EdgeFormEditor.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .\dist\SelfContained
```

**Output:** `dist\SelfContained\EdgeFormEditor.exe` (~124 MB)

This single file:
- ? Includes .NET 10 runtime (no installation needed)
- ? Runs on any Windows x64 machine
- ? No installer required - just copy and run
- ? No dependencies

---

## MSIX Installer

### Option 1: Using MSIX Packaging Project (requires Windows SDK)

A packaging project is included at `EdgeFormEditor.Package/EdgeFormEditor.Package.wapproj`.

**Prerequisites:**
- Windows 10 SDK (10.0.19041.0 or later) installed via Visual Studio Installer

**Build from Visual Studio:**
1. Install Windows SDK via Visual Studio Installer ? Individual Components ? Windows 10 SDK
2. Open the solution in Visual Studio
3. Set `EdgeFormEditor.Package` as the startup project
4. Set configuration to `Release` and platform to `x64`
5. Build the packaging project
6. Output `.msix` is in `EdgeFormEditor.Package\AppPackages\`

**Build from command line:**
```powershell
dotnet restore EdgeFormEditor\EdgeFormEditor.csproj
msbuild EdgeFormEditor.Package\EdgeFormEditor.Package.wapproj /t:Build /p:Configuration=Release /p:Platform=x64 /p:UapAppxPackageBuildMode=SideloadOnly /p:AppxBundle=Never
```

### Option 2: Self-Contained Deployment (no installer, no SDK required)

Build a self-contained executable that can run without .NET runtime installed:

```powershell
dotnet publish EdgeFormEditor\EdgeFormEditor.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

Output will be in: `EdgeFormEditor\bin\Release\net10.0-windows\win-x64\publish\`

The single `.exe` file can be distributed directly (no installation needed).

### Option 3: Framework-Dependent Deployment (smallest size)

Build a framework-dependent executable (requires .NET 10 runtime on target machine):

```powershell
dotnet publish EdgeFormEditor\EdgeFormEditor.csproj -c Release -r win-x64 --self-contained false
```

Output will be in: `EdgeFormEditor\bin\Release\net10.0-windows\win-x64\publish\`

## Important Note

For reliable write operations, Microsoft Edge should be closed before saving changes, because running Edge processes can keep the database locked or effectively read-only.