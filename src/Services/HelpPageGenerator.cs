namespace JumpStarter.Services;

public static class HelpPageGenerator
{
    public static string Generate(string configPath)
    {
        string templatePath = Path.Combine(AppContext.BaseDirectory, "assets", "help.html");
        string template = File.ReadAllText(templatePath);
        string fileUrl = new Uri(configPath).AbsoluteUri;
        string html = template.Replace("{{CONFIG_URL}}", fileUrl);

        string outputPath = Path.Combine(Path.GetTempPath(), "JumpStarter_help.html");
        File.WriteAllText(outputPath, html);
        return outputPath;
    }
}
