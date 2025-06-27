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
    
    // Standard array recommendation for 2024 PHB (legacy support)
    public Dictionary<string, int>? StandardArrayRecommendation { get; set; }
    
    // Multiple standard array recommendations for 2024 PHB
    public List<AbilityScoreRecommendation>? StandardArrayRecommendations { get; set; }
    
    // Helper properties for recommendations
    public bool HasMultipleRecommendations 
    { 
        get 
        {
            // Check starting class benefits first
            if (StartingClassBenefits?.StandardArrayRecommendations?.Count > 1)
                return true;
            
            // Then check class level
            if (StandardArrayRecommendations?.Count > 1)
                return true;
                
            return false;
        }
    }
    
    public bool HasAnyRecommendations => StandardArrayRecommendation != null || StandardArrayRecommendations?.Count > 0 || StartingClassBenefits?.StandardArrayRecommendations?.Count > 0;
    
    // 2024 D&D benefits structure
    public StartingClassBenefits? StartingClassBenefits { get; set; }
    public MulticlassBenefits? MulticlassBenefits { get; set; }
    
    // Subclass information
    public int SubclassLevel { get; set; } = 3; // Level at which subclass is chosen
    public string SubclassType { get; set; } = string.Empty; // e.g., "archetype", "oath", "patron"
    public List<Subclass> Subclasses { get; set; } = new();
    
    public bool HasSubclasses => Subclasses.Any();
    public bool CanChooseSubclass(int characterLevel) => characterLevel >= SubclassLevel && HasSubclasses;

    public Dictionary<int, List<ClassFeature>> Features { get; set; } = new();

    public override string ToString() => Name;
}

public class AbilityScoreRecommendation
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, int> Array { get; set; } = new();
    
    public override string ToString() => Name;
}

public class StartingClassBenefits
{
    public string Description { get; set; } = string.Empty;
    public string[] PrimaryAbility { get; set; } = Array.Empty<string>();
    public Dictionary<string, int>? StandardArrayRecommendation { get; set; }
    public List<AbilityScoreRecommendation>? StandardArrayRecommendations { get; set; }
    public string[] SavingThrowProficiencies { get; set; } = Array.Empty<string>();
    public string[] ArmorProficiencies { get; set; } = Array.Empty<string>();
    public string[] WeaponProficiencies { get; set; } = Array.Empty<string>();
    public string[] ToolProficiencies { get; set; } = Array.Empty<string>();
    public SkillChoiceOptions? SkillProficiencies { get; set; }
    public object? EquipmentGrants { get; set; }
}

public class MulticlassBenefits
{
    public string Description { get; set; } = string.Empty;
    public MulticlassPrerequisites? Prerequisites { get; set; }
    public string[] WeaponProficiencies { get; set; } = Array.Empty<string>();
    public string[] ArmorProficiencies { get; set; } = Array.Empty<string>();
    public string[] ToolProficiencies { get; set; } = Array.Empty<string>();
}

public class MulticlassPrerequisites
{
    public Dictionary<string, int>? MinimumAbilityScores { get; set; }
}

public class SkillChoiceOptions
{
    public int ChooseCount { get; set; } = 0;
    public List<string> AvailableSkills { get; set; } = new();
    
    public bool HasChoices => ChooseCount > 0 && AvailableSkills.Count > 0;
}

public class ClassFeature
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object? Mechanics { get; set; }
} 