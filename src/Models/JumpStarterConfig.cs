namespace JumpStarter.Models;

public class JumpStarterConfig
{
    public int MaxConcurrentTasks { get; set; } = 5;
    public List<CommandEntry> Commands { get; set; } = new();
}
