# Building the Installer

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Inno Setup 6+](https://jrsoftware.org/isdl.php)

## Steps

### 1. Publish the application

From the repository root:

```powershell
dotnet publish src\JumpStarter.csproj -c Release -r win-x64 --self-contained true -o publish\
```

This produces a self-contained executable in `publish\` — no .NET installation required on the target machine.

### 2. Compile the installer

Open `installer\JumpStarter.iss` with Inno Setup Compiler and press **Build → Compile** (or `Ctrl+F9`).

The installer is generated at:

```
installer\installer_output\JumpStarter_Setup.exe
```

### 3. Distribute

Share `JumpStarter_Setup.exe`. The installer:

- Does not require administrator privileges
- Installs to `%LocalAppData%\JumpStarter`
- Offers the option to launch the app after installation

## Notes

- If you change the executable name or publish path, update `PublishDir` and `AppExeName` in the `.iss` file.
- To change the version, edit `#define AppVersion` in `installer\JumpStarter.iss`.
- Uninstalling automatically removes the Task Scheduler entry if it was registered.
