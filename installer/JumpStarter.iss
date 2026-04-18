; JumpStarter Installer
; Requires Inno Setup 6+ (https://jrsoftware.org/isinfo.php)
; Before compiling: dotnet publish src\JumpStarter.csproj -c Release -r win-x64 --self-contained true -o publish\

#define AppName "JumpStarter"
#define AppVersion "1.0.0"
#define AppPublisher "JumpStarter"
#define AppExeName "JumpStarter.exe"
#define PublishDir "..\publish"

[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={localappdata}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputDir=installer_output
OutputBaseFilename=JumpStarter_Setup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=lowest
WizardStyle=modern
UninstallDisplayIcon={app}\{#AppExeName}

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "launchafterinstall"; Description: "Iniciar JumpStarter ahora"; GroupDescription: "Opciones:"; Flags: unchecked

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{group}\Desinstalar {#AppName}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Iniciar {#AppName}"; Flags: nowait postinstall skipifsilent; Tasks: launchafterinstall

[UninstallRun]
Filename: "schtasks.exe"; Parameters: "/Delete /TN ""JumpStarter"" /F"; Flags: runhidden; RunOnceId: "RemoveTask"
