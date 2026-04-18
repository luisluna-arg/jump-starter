using System.Text.Json;
using JumpStarter.Models;
using Serilog;

namespace JumpStarter;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        string logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "JumpStarter", "logs", "jumps-.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.File(logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 90)
            .CreateLogger();

        try
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath))
            {
                Log.Fatal("appsettings.json not found at {Path}", configPath);
                return;
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var config = JsonSerializer.Deserialize<JumpStarterConfig>(
                File.ReadAllText(configPath), options) ?? new JumpStarterConfig();

            string iconPath = Path.Combine(AppContext.BaseDirectory,
                "assets", "Gemini_Generated_Image_1isa9x1isa9x1isa.png");

            ApplicationConfiguration.Initialize();
            Application.Run(new TrayApplicationContext(config, iconPath));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception during startup");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}