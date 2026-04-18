namespace JumpStarter.Models;

public class CommandEntry
{
    public string Name { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;
    public bool Shell { get; set; } = false;
}
