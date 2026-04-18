using JumpStarter.Services;
using JumpStarter.Models;

namespace JumpStarter;

public class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly CommandExecutor _executor;
    private readonly ToolStripMenuItem _startupMenuItem;
    private readonly SynchronizationContext _syncContext;

    public TrayApplicationContext(JumpStarterConfig config, string iconPath)
    {
        _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
        _executor = new CommandExecutor(config);
        _executor.OnStatusChanged(UpdateTooltip);

        Icon trayIcon = File.Exists(iconPath)
            ? Icon.FromHandle(new System.Drawing.Bitmap(iconPath).GetHicon())
            : SystemIcons.Application;

        _startupMenuItem = new ToolStripMenuItem("Iniciar con Windows", null, OnToggleStartup)
        {
            Checked = StartupRegistrar.IsRegistered()
        };

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Ejecutar ahora", null, OnRunNow);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(_startupMenuItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("Salir", null, OnExit);

        _notifyIcon = new NotifyIcon
        {
            Icon = trayIcon,
            Text = "JumpStarter",
            ContextMenuStrip = contextMenu,
            Visible = true
        };

        UpdateTooltip();
        _ = RunOnStartupAsync();
    }

    private async Task RunOnStartupAsync()
    {
        await _executor.ExecuteAllAsync();
    }

    private void OnRunNow(object? sender, EventArgs e)
    {
        _ = _executor.ExecuteAllAsync();
    }

    private void OnToggleStartup(object? sender, EventArgs e)
    {
        try
        {
            if (StartupRegistrar.IsRegistered())
            {
                StartupRegistrar.Unregister();
                _startupMenuItem.Checked = false;
            }
            else
            {
                string exePath = Application.ExecutablePath;
                StartupRegistrar.Register(exePath);
                _startupMenuItem.Checked = StartupRegistrar.IsRegistered();
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Failed to toggle startup registration");
        }
    }

    private void OnExit(object? sender, EventArgs e)
    {
        _notifyIcon.Visible = false;
        Application.Exit();
    }

    private void UpdateTooltip()
    {
        _syncContext.Post(_ => RefreshTooltip(), null);
    }

    private void RefreshTooltip()
    {
        var statuses = _executor.Statuses;

        var running = statuses.Where(kv => kv.Value == CommandStatus.Running).Select(kv => kv.Key).ToList();
        var done    = statuses.Where(kv => kv.Value == CommandStatus.Done).Select(kv => kv.Key).ToList();
        var failed  = statuses.Where(kv => kv.Value == CommandStatus.Failed).Select(kv => kv.Key).ToList();

        var lines = new List<string> { "JumpStarter" };
        if (running.Count > 0) lines.Add("▶ " + string.Join(", ", running));
        if (done.Count > 0)    lines.Add("✓ " + string.Join(", ", done));
        if (failed.Count > 0)  lines.Add("✗ " + string.Join(", ", failed));

        string text = string.Join("\n", lines);
        if (text.Length > 127)
        {
            string summary = $"JumpStarter\n▶ {running.Count} corriendo  ✓ {done.Count} listos  ✗ {failed.Count} fallidos";
            text = summary.Length > 127 ? "JumpStarter" : summary;
        }

        _notifyIcon.Text = text;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }
        base.Dispose(disposing);
    }
}
