namespace GToonManager.Models;

public class Background
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] SkillProficiencies { get; set; } = Array.Empty<string>();
    public string[] Languages { get; set; } = Array.Empty<string>();
    public string[] ToolProficiencies { get; set; } = Array.Empty<string>();
    public string[] Equipment { get; set; } = Array.Empty<string>();
    public string Feature { get; set; } = string.Empty;

    public override string ToString() => Name;
} 