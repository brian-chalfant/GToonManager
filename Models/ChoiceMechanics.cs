using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GToonManager.Models;

// Base class for all choice mechanics
public abstract class ChoiceMechanic : INotifyPropertyChanged
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Level { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsCompleted { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// 1. Subclass Choice
public class SubclassChoice : ChoiceMechanic
{
    public ObservableCollection<SubclassOption> AvailableSubclasses { get; set; } = new();
    private SubclassOption? _selectedSubclass;
    public SubclassOption? SelectedSubclass
    {
        get => _selectedSubclass;
        set
        {
            _selectedSubclass = value;
            IsCompleted = value != null;
            OnPropertyChanged(nameof(SelectedSubclass));
            OnPropertyChanged(nameof(IsCompleted));
        }
    }
}

public class SubclassOption
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}

// 2. Ability Score Improvement Choice
public class AbilityScoreImprovementChoice : ChoiceMechanic
{
    public bool CanChooseFeat { get; set; }
    private bool _chooseFeat;
    public bool ChooseFeat
    {
        get => _chooseFeat;
        set
        {
            _chooseFeat = value;
            OnPropertyChanged(nameof(ChooseFeat));
            UpdateCompletionStatus();
        }
    }

    public ObservableCollection<AbilityScoreSelection> AbilityScoreSelections { get; set; } = new();
    public ObservableCollection<FeatOption> AvailableFeats { get; set; } = new();
    
    private FeatOption? _selectedFeat;
    public FeatOption? SelectedFeat
    {
        get => _selectedFeat;
        set
        {
            _selectedFeat = value;
            OnPropertyChanged(nameof(SelectedFeat));
            UpdateCompletionStatus();
        }
    }

    private void UpdateCompletionStatus()
    {
        if (ChooseFeat && CanChooseFeat)
        {
            IsCompleted = SelectedFeat != null;
        }
        else
        {
            IsCompleted = AbilityScoreSelections.Sum(s => s.Improvement) == 2;
        }
        OnPropertyChanged(nameof(IsCompleted));
    }
}

public class AbilityScoreSelection
{
    public string AbilityScore { get; set; } = string.Empty;
    public int Improvement { get; set; }
}

public class FeatOption
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Prerequisites { get; set; } = string.Empty;
}

// 3. Combat Style Choice
public class CombatStyleChoice : ChoiceMechanic
{
    public ObservableCollection<CombatStyleOption> AvailableStyles { get; set; } = new();
    private CombatStyleOption? _selectedStyle;
    public CombatStyleOption? SelectedStyle
    {
        get => _selectedStyle;
        set
        {
            _selectedStyle = value;
            IsCompleted = value != null;
            OnPropertyChanged(nameof(SelectedStyle));
            OnPropertyChanged(nameof(IsCompleted));
        }
    }
}

public class CombatStyleOption
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Effect { get; set; } = string.Empty;
}

// 4. Skills/Tools Choice
public class SkillsToolsChoice : ChoiceMechanic
{
    public int NumberToChoose { get; set; }
    public ObservableCollection<string> AvailableSkills { get; set; } = new();
    public ObservableCollection<string> AvailableTools { get; set; } = new();
    public ObservableCollection<string> SelectedSkills { get; set; } = new();
    public ObservableCollection<string> SelectedTools { get; set; } = new();

    public bool CanChooseSkills => AvailableSkills.Count > 0;
    public bool CanChooseTools => AvailableTools.Count > 0;

    public void UpdateCompletionStatus()
    {
        IsCompleted = (SelectedSkills.Count + SelectedTools.Count) == NumberToChoose;
        OnPropertyChanged(nameof(IsCompleted));
    }
}

// 5. Creature Type Choice
public class CreatureTypeChoice : ChoiceMechanic
{
    public ObservableCollection<CreatureTypeOption> AvailableCreatureTypes { get; set; } = new();
    private CreatureTypeOption? _selectedCreatureType;
    public CreatureTypeOption? SelectedCreatureType
    {
        get => _selectedCreatureType;
        set
        {
            _selectedCreatureType = value;
            IsCompleted = value != null;
            OnPropertyChanged(nameof(SelectedCreatureType));
            OnPropertyChanged(nameof(IsCompleted));
        }
    }

    public bool CanSelectTwoHumanoidRaces { get; set; }
    public ObservableCollection<string> SelectedHumanoidRaces { get; set; } = new();
}

public class CreatureTypeOption
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsHumanoidChoice { get; set; }
}

// 6. Terrain Type Choice
public class TerrainTypeChoice : ChoiceMechanic
{
    public ObservableCollection<TerrainTypeOption> AvailableTerrainTypes { get; set; } = new();
    private TerrainTypeOption? _selectedTerrainType;
    public TerrainTypeOption? SelectedTerrainType
    {
        get => _selectedTerrainType;
        set
        {
            _selectedTerrainType = value;
            IsCompleted = value != null;
            OnPropertyChanged(nameof(SelectedTerrainType));
            OnPropertyChanged(nameof(IsCompleted));
        }
    }
}

public class TerrainTypeOption
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Benefits { get; set; } = string.Empty;
}

// 7. Spell Choice
public class SpellChoice : ChoiceMechanic
{
    public int NumberToChoose { get; set; }
    public int SpellLevel { get; set; }
    public string SpellList { get; set; } = string.Empty; // "wizard", "any class", etc.
    public ObservableCollection<SpellOption> AvailableSpells { get; set; } = new();
    public ObservableCollection<SpellOption> SelectedSpells { get; set; } = new();

    public void UpdateCompletionStatus()
    {
        IsCompleted = SelectedSpells.Count == NumberToChoose;
        OnPropertyChanged(nameof(IsCompleted));
    }
}

public class SpellOption
{
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public string School { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Classes { get; set; } = string.Empty;
}

// 8. Magical Feature Choice
public class MagicalFeatureChoice : ChoiceMechanic
{
    public int NumberToChoose { get; set; }
    public string FeatureType { get; set; } = string.Empty; // "Metamagic", "Eldritch Invocation", "Pact Boon"
    public ObservableCollection<MagicalFeatureOption> AvailableFeatures { get; set; } = new();
    public ObservableCollection<MagicalFeatureOption> SelectedFeatures { get; set; } = new();

    public void UpdateCompletionStatus()
    {
        IsCompleted = SelectedFeatures.Count == NumberToChoose;
        OnPropertyChanged(nameof(IsCompleted));
    }
}

public class MagicalFeatureOption
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Prerequisites { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Cost { get; set; } = string.Empty; // For sorcery points, etc.
} 