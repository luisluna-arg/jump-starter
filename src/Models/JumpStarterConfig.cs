namespace JumpStarter.Models;

public class JumpStarterConfig
{
    public int MaxConcurrentTasks { get; set; } = 5;
    public bool RunOnStartup { get; set; } = true;
    public List<CommandEntry> Commands { get; set; } = new();
}
