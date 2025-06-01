using System.ComponentModel;

namespace GToonManager.Models;

public class Background : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private BackgroundFeature _feature = new();
    private ProficiencyGrants _proficiencyGrants = new();
    private EquipmentGrants _equipmentGrants = new();
    private PersonalityTraits _personality = new();
    private string _suggestedCharacteristics = string.Empty;
    private string _source = string.Empty;
    private int _sourcePage = 0;
    private AbilityScoreImprovement _abilityScoreImprovement = new();

    public string Name
    {
        //TODO: Add source and source page to the backgrounds
        //TODO: Add Backgrounds from the PHB 2024 that are not in the PHB 2014
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    public BackgroundFeature Feature
    {
        get => _feature;
        set
        {
            _feature = value;
            OnPropertyChanged(nameof(Feature));
        }
    }

    public ProficiencyGrants ProficiencyGrants
    {
        get => _proficiencyGrants;
        set
        {
            _proficiencyGrants = value;
            OnPropertyChanged(nameof(ProficiencyGrants));
        }
    }

    public EquipmentGrants EquipmentGrants
    {
        get => _equipmentGrants;
        set
        {
            _equipmentGrants = value;
            OnPropertyChanged(nameof(EquipmentGrants));
        }
    }

    public PersonalityTraits Personality
    {
        get => _personality;
        set
        {
            _personality = value;
            OnPropertyChanged(nameof(Personality));
        }
    }

    public string SuggestedCharacteristics
    {
        get => _suggestedCharacteristics;
        set
        {
            _suggestedCharacteristics = value;
            OnPropertyChanged(nameof(SuggestedCharacteristics));
        }
    }

    public string Source
    {
        get => _source;
        set
        {
            _source = value;
            OnPropertyChanged(nameof(Source));
        }
    }

    public int SourcePage
    {
        get => _sourcePage;
        set
        {
            _sourcePage = value;
            OnPropertyChanged(nameof(SourcePage));
        }
    }

    public AbilityScoreImprovement AbilityScoreImprovement
    {
        get => _abilityScoreImprovement;
        set
        {
            _abilityScoreImprovement = value;
            OnPropertyChanged(nameof(AbilityScoreImprovement));
        }
    }

    // Legacy properties for backward compatibility
    public string[] SkillProficiencies => ProficiencyGrants.Skills.ToArray();
    public string[] Languages => ProficiencyGrants.Languages.Known.ToArray();
    public string[] ToolProficiencies => ProficiencyGrants.Tools.ToArray();
    public string[] Equipment => EquipmentGrants.Fixed.Select(e => $"{e.Quantity}x {e.Item}").ToArray();
    public string FeatureText => Feature.Description;

    public override string ToString() => Name;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class AbilityScoreImprovement
{
    public string Description { get; set; } = string.Empty;
    public List<string> AbilityScores { get; set; } = new();
    public List<ImprovementOption> ImprovementOptions { get; set; } = new();
    public int MaxScore { get; set; } = 20;
}

public class ImprovementOption
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Distribution> Distributions { get; set; } = new();
}

public class Distribution
{
    public int Count { get; set; }
    public int Amount { get; set; }
}

public class BackgroundFeature
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class ProficiencyGrants
{
    public List<string> Skills { get; set; } = new();
    public LanguageGrant Languages { get; set; } = new();
    public List<string> Tools { get; set; } = new();
}

public class LanguageGrant
{
    public int Count { get; set; } = 0;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Known { get; set; } = new();
}

public class EquipmentGrants
{
    public List<EquipmentItem> Fixed { get; set; } = new();
    public Currency Currency { get; set; } = new();
}

public class EquipmentItem
{
    public string Item { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public string Description { get; set; } = string.Empty;
}

public class Currency
{
    public int Gold { get; set; } = 0;
    public int Silver { get; set; } = 0;
    public int Copper { get; set; } = 0;
}

public class PersonalityTraits
{
    public TraitOptions PersonalityTraitsOptions { get; set; } = new();
    public TraitOptions Ideals { get; set; } = new();
    public TraitOptions Bonds { get; set; } = new();
    public TraitOptions Flaws { get; set; } = new();
}

public class TraitOptions
{
    public List<string> Suggestions { get; set; } = new();
    public List<IdealOption> IdealSuggestions { get; set; } = new();
    public int Count { get; set; } = 1;
    public string Type { get; set; } = "choice";
}

public class IdealOption
{
    public string Ideal { get; set; } = string.Empty;
    public string Alignment { get; set; } = string.Empty;
}

public class BackgroundAbilityScoreChoice
{
    public Background Background { get; set; } = null!;
    public ImprovementOption SelectedOption { get; set; } = null!;
    public Dictionary<string, int> AbilityScoreImprovements { get; set; } = new();
    
    public bool IsValid => AbilityScoreImprovements.Values.Sum() == GetExpectedTotal();
    
    private int GetExpectedTotal()
    {
        if (SelectedOption?.Distributions == null) return 0;
        return SelectedOption.Distributions.Sum(d => d.Count * d.Amount);
    }
} 