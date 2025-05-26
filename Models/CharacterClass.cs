namespace GToonManager.Models;

public class CharacterClass
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int HitDie { get; set; } = 8;
    public string PrimaryAbility { get; set; } = string.Empty;
    public string[] SavingThrowProficiencies { get; set; } = Array.Empty<string>();
    public string[] SkillProficiencies { get; set; } = Array.Empty<string>();
    public string[] ArmorProficiencies { get; set; } = Array.Empty<string>();
    public string[] WeaponProficiencies { get; set; } = Array.Empty<string>();
    public string[] ToolProficiencies { get; set; } = Array.Empty<string>();
    public string[] StartingEquipment { get; set; } = Array.Empty<string>();

    public override string ToString() => Name;
} 