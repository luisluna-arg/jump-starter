using System.Diagnostics;
using Serilog;

namespace JumpStarter.Services;

public static class StartupRegistrar
{
    private const string TaskName = "JumpStarter";

    public static bool IsRegistered()
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = $"/Query /TN \"{TaskName}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            process!.WaitForExit();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to query startup task");
            return false;
        }
    }

    public static void Register(string exePath)
    {
        string args = $"/Create /TN \"{TaskName}\" /TR \"\\\"{exePath}\\\"\" /SC ONLOGON /RL HIGHEST /F";
        RunElevated("schtasks.exe", args);
    }

    public static void Unregister()
    {
        RunElevated("schtasks.exe", $"/Delete /TN \"{TaskName}\" /F");
    }

    private static void RunElevated(string fileName, string arguments)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = true,
            Verb = "runas"
        });
        process!.WaitForExit();
        if (process.ExitCode != 0)
            Log.Warning("{FileName} exited with code {ExitCode}", fileName, process.ExitCode);
    }
}
