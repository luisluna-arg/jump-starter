using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using JumpStarter.Models;
using Serilog;

namespace JumpStarter.Services;

public enum CommandStatus { Pending, Running, Done, Failed }

public class CommandExecutor
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private readonly string _configPath;
    private readonly ConcurrentDictionary<string, CommandStatus> _statuses = new();
    private Action? _onStatusChanged;

    public IReadOnlyDictionary<string, CommandStatus> Statuses => _statuses;

    public CommandExecutor(string configPath)
    {
        _configPath = configPath;
    }

    public static JumpStarterConfig LoadConfig(string configPath) =>
        JsonSerializer.Deserialize<JumpStarterConfig>(File.ReadAllText(configPath), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        }) ?? new JumpStarterConfig();

    public void OnStatusChanged(Action callback) => _onStatusChanged = callback;

    public async Task ExecuteAllAsync(CancellationToken ct = default)
    {
        var config = JsonSerializer.Deserialize<JumpStarterConfig>(
            File.ReadAllText(_configPath), _jsonOptions) ?? new JumpStarterConfig();

        using var semaphore = new SemaphoreSlim(Math.Max(1, config.MaxConcurrentTasks));

        _statuses.Clear();
        foreach (var entry in config.Commands.Where(e => e.Enabled))
            _statuses[entry.Name] = CommandStatus.Pending;

        NotifyChanged();

        var tasks = new List<Task>();
        foreach (var entry in config.Commands.Where(e => e.Enabled))
        {
            if (ct.IsCancellationRequested) break;

            if (entry.Delay > TimeSpan.Zero)
                await Task.Delay(entry.Delay, ct);

            tasks.Add(RunCommandAsync(entry, semaphore, ct));
        }

        await Task.WhenAll(tasks);
    }

    private async Task RunCommandAsync(CommandEntry entry, SemaphoreSlim semaphore, CancellationToken ct)
    {
        await semaphore.WaitAsync(ct);
        try
        {
            _statuses[entry.Name] = CommandStatus.Running;
            NotifyChanged();

            var psi = BuildProcessStartInfo(entry);
            using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            process.Start();
            await process.WaitForExitAsync(ct);

            _statuses[entry.Name] = process.ExitCode == 0 ? CommandStatus.Done : CommandStatus.Failed;
            if (process.ExitCode != 0)
                Log.Warning("Command {Name} exited with code {ExitCode}", entry.Name, process.ExitCode);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _statuses[entry.Name] = CommandStatus.Failed;
            Log.Error(ex, "Command {Name} failed to execute", entry.Name);
        }
        finally
        {
            semaphore.Release();
            NotifyChanged();
        }
    }

    private static ProcessStartInfo BuildProcessStartInfo(CommandEntry entry)
    {
        if (entry.Shell)
        {
            return new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{entry.Command}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
        }

        var parts = SplitCommandLine(entry.Command);
        return new ProcessStartInfo
        {
            FileName = parts.exe,
            Arguments = parts.args,
            UseShellExecute = true
        };
    }

    private static (string exe, string args) SplitCommandLine(string command)
    {
        command = command.Trim();
        if (command.StartsWith('"'))
        {
            int end = command.IndexOf('"', 1);
            if (end > 0)
            {
                string exe = command[1..end];
                string args = command[(end + 1)..].Trim();
                return (exe, args);
            }
        }

        int space = command.IndexOf(' ');
        if (space < 0) return (command, string.Empty);
        return (command[..space], command[(space + 1)..]);
    }

    private void NotifyChanged() => _onStatusChanged?.Invoke();
}
