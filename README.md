# JumpStarter

A Windows system tray application that runs a configurable list of commands at user logon.

## Features

- Automatically runs commands at user logon
- Sequential execution with a configurable per-command delay
- Up to 5 concurrent tasks (configurable)
- No visible terminal window — GUI apps (Chrome, etc.) open normally; scripts run silently
- Tray tooltip shows real-time status of running, completed, and failed commands
- Registered via Task Scheduler with highest privileges — no UAC prompt on subsequent startups
- Rolling error log (90 days) at `%AppData%\JumpStarter\logs\`

## Configuration

Edit `appsettings.json` next to the executable:

```json
{
  "MaxConcurrentTasks": 5,
  "Commands": [
    {
      "Name": "Open Chrome",
      "Command": "chrome.exe https://www.google.com",
      "Delay": "00:00:00",
      "Shell": false
    },
    {
      "Name": "Maintenance script",
      "Command": "powershell.exe -NoProfile -File C:\\scripts\\startup.ps1",
      "Delay": "00:00:10",
      "Shell": true
    }
  ]
}
```

| Field | Type | Description |
|---|---|---|
| `Name` | string | Short descriptive label for the command |
| `Command` | string | Executable and arguments |
| `Delay` | `hh:mm:ss` | Wait time before executing. Default: `"00:00:00"` |
| `Shell` | bool | `false` = direct executable, no terminal · `true` = `cmd.exe /c`, hidden window. Default: `false` |

## Tray Menu

| Option | Description |
|---|---|
| Run now | Executes all commands immediately |
| Start with Windows | Registers/removes the Task Scheduler entry (UAC prompt once) |
| Exit | Closes the application |

## Installation

Download `JumpStarter_Setup.exe` from [Releases](../../releases) and run it. No administrator privileges required.

To build the installer yourself, see [docs/building-the-installer.md](docs/building-the-installer.md).

## Build from source

```powershell
git clone https://github.com/luisluna-arg/jump-starter.git
cd jump-starter
dotnet build src\JumpStarter.csproj -c Release
```

Requires: [.NET 10 SDK](https://dotnet.microsoft.com/download)

## License

MIT
