# JumpStarter — Agent Reference

## What This Project Does

JumpStarter is a Windows system tray application that executes a configurable list of commands automatically at user logon. It runs processes silently in the background (no console windows), tracks their status, and exposes controls via a tray icon context menu. It also integrates with Windows Task Scheduler to register itself as a high-privilege startup task so subsequent launches require no UAC prompt.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET 10) |
| UI | Windows Forms (tray-only, no main window) |
| Logging | Serilog → rolling file in `%AppData%\JumpStarter\logs\` |
| Config format | JSONC (JSON with comments + trailing commas) |
| Installer | Inno Setup 6+ |
| Build | .NET CLI |

---

## Project Layout

```
jump-starter/
├── src/
│   ├── JumpStarter.csproj
│   ├── Program.cs                        # Entry point; configures Serilog, loads tray icon
│   ├── TrayApplicationContext.cs         # Tray icon, context menu, tooltip, startup trigger
│   ├── Models/
│   │   ├── JumpStarterConfig.cs          # Root config POCO (MaxConcurrentTasks, RunOnStartup, Commands)
│   │   └── CommandEntry.cs              # Per-command POCO (Name, Command, Delay, Shell, Enabled)
│   ├── Services/
│   │   ├── CommandExecutor.cs            # Loads config, runs commands, tracks status
│   │   ├── StartupRegistrar.cs           # Registers/unregisters Task Scheduler entry
│   │   └── HelpPageGenerator.cs         # Renders help.html template → temp file → browser
│   └── assets/
│       ├── help.html                     # Help page template (placeholder: {{CONFIG_URL}})
│       └── *.png                         # Tray icon image
├── installer/
│   └── JumpStarter.iss                   # Inno Setup script; targets %LocalAppData%\JumpStarter
├── .vscode/
│   ├── launch.json                       # Debug config (coreclr)
│   └── tasks.json                        # build + publish tasks
├── .claude/
│   └── commands/                         # Claude Code skill files
└── appsettings.jsonc                     # Not in src/ root — copied to output dir by csproj
```

> `appsettings.jsonc` lives at `src/appsettings.jsonc` and is copied to the output directory at build time. It is loaded at runtime from the executable's directory, not from a fixed path.

---

## Architecture

Three logical layers:

1. **Presentation** — `TrayApplicationContext`: owns the tray icon, context menu items, and tooltip updates. Calls into services; never executes commands directly.
2. **Business logic** — `CommandExecutor`: parses JSONC config, manages a `SemaphoreSlim` for `MaxConcurrentTasks` concurrency, executes each command, and maintains a `ConcurrentDictionary<string, string>` of per-command status strings.
3. **Infrastructure** — `StartupRegistrar` (schtasks.exe integration) and `HelpPageGenerator` (HTML templating).

### Execution flow

```
App start
  → Serilog configured
  → appsettings.jsonc existence validated
  → Tray icon shown (main window hidden)
  → If RunOnStartup == true → ExecuteAllAsync()
User clicks "Ejecutar ahora"
  → ExecuteAllAsync()
      → Config reloaded from disk (dynamic — no restart needed)
      → Each enabled command acquired through semaphore
      → Shell commands: cmd.exe /c, hidden window
      → Direct commands: UseShellExecute=true
      → Status updated in ConcurrentDictionary → tray tooltip refreshed
      → Non-zero exits / exceptions → Serilog Warning/Error
```

---

## Build & Run

```bash
# Debug build
dotnet build src\JumpStarter.csproj

# Self-contained release (for distribution)
dotnet publish src\JumpStarter.csproj -c Release -r win-x64 --self-contained true

# Run in VS Code: F5 (uses .vscode/launch.json)
```

Output: `src/bin/Debug/net10.0-windows/` or `publish/` for release.

The installer is built separately with Inno Setup using `installer/JumpStarter.iss`. It installs to `%LocalAppData%\JumpStarter` and requires no admin privileges.

---

## Configuration

`appsettings.jsonc` supports JSON comments and trailing commas. Schema:

```jsonc
{
  "MaxConcurrentTasks": 5,       // max parallel commands
  "RunOnStartup": true,          // auto-execute on app launch
  "Commands": [
    {
      "Name": "My App",
      "Command": "C:\\path\\to\\app.exe",
      "Delay": "00:00:05",       // TimeSpan string; wait before executing
      "Shell": false,            // true = cmd.exe /c; false = direct launch
      "Enabled": true
    }
  ]
}
```

Config is re-read from disk on every `ExecuteAllAsync()` call — changes take effect without restarting the app.

---

## Key Conventions

- **No console window**: all `ProcessStartInfo` calls set `CreateNoWindow = true` and `WindowStyle = ProcessWindowStyle.Hidden` (or `UseShellExecute = true` for direct launches).
- **Spanish UI strings**: tray menu labels and help page text are in Spanish. Keep new UI strings consistent.
- **Logging level**: only `Warning` and above are written to file. Do not add `Information`-level noise to hot paths.
- **Nullable enabled**: the project uses `<Nullable>enable</Nullable>`. All new code must be null-safe.
- **Status strings**: command status values are plain strings (e.g. `"Running"`, `"Done"`, `"Failed"`). They are displayed directly in the tray tooltip.
- **JSONC parsing**: the custom parser in `CommandExecutor` strips `//` comments and trailing commas before `JsonSerializer.Deserialize`. Do not switch to a different parser without accounting for this.
