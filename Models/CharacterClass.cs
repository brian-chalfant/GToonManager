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
    
    // New property for parsed skill choices
    public SkillChoiceOptions? SkillChoices { get; set; }
    
    // Standard array recommendation for 2024 PHB
    public Dictionary<string, int>? StandardArrayRecommendation { get; set; }
    
    // Subclass information
    public int SubclassLevel { get; set; } = 3; // Level at which subclass is chosen
    public string SubclassType { get; set; } = string.Empty; // e.g., "archetype", "oath", "patron"
    public List<Subclass> Subclasses { get; set; } = new();
    
    public bool HasSubclasses => Subclasses.Any();
    public bool CanChooseSubclass(int characterLevel) => characterLevel >= SubclassLevel && HasSubclasses;

    public override string ToString() => Name;
}

public class SkillChoiceOptions
{
    public int ChooseCount { get; set; } = 0;
    public List<string> AvailableSkills { get; set; } = new();
    
    public bool HasChoices => ChooseCount > 0 && AvailableSkills.Count > 0;
} 