using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

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
    public CharacterClassLevel? ClassLevel { get; set; }
    public ObservableCollection<Subclass> AvailableSubclasses { get; set; } = new();
    private Subclass? _selectedSubclass;
    public Subclass? SelectedSubclass
    {
        get => _selectedSubclass;
        set
        {
            _selectedSubclass = value;
            IsCompleted = value != null;
            OnPropertyChanged(nameof(SelectedSubclass));
            OnPropertyChanged(nameof(IsCompleted));
            
            // Apply the subclass to the character level
            if (ClassLevel != null && value != null)
            {
                ClassLevel.ChosenSubclass = value;
            }
        }
    }
    
    // Legacy support for SubclassOption (if needed elsewhere)
    public ObservableCollection<SubclassOption> LegacyAvailableSubclasses { get; set; } = new();
    private SubclassOption? _legacySelectedSubclass;
    public SubclassOption? LegacySelectedSubclass
    {
        get => _legacySelectedSubclass;
        set
        {
            _legacySelectedSubclass = value;
            OnPropertyChanged(nameof(LegacySelectedSubclass));
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
    public bool CanChooseFeat { get; set; } = true; // In 2024, feats are always an option
    
    // Reference to the character for ability score validation
    public Character? Character { get; set; }

    private ASIChoiceType _choiceType = ASIChoiceType.AbilityScoreIncrease;
    public ASIChoiceType ChoiceType
    {
        get => _choiceType;
        set
        {
            _choiceType = value;
            OnPropertyChanged(nameof(ChoiceType));
            OnPropertyChanged(nameof(IsChoosingFeat));
            OnPropertyChanged(nameof(IsChoosingAbilityScores));
            UpdateCompletionStatus();
        }
    }

    public bool IsChoosingFeat => ChoiceType == ASIChoiceType.Feat;
    public bool IsChoosingAbilityScores => ChoiceType == ASIChoiceType.AbilityScoreIncrease;

    // Ability Score selections (can be +2 to one or +1 to two)
    public ObservableCollection<AbilityScoreSelection> AbilityScoreSelections { get; set; } = new();
    
    // Available abilities for selection
    public List<string> AvailableAbilities { get; } = new() 
    { 
        "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" 
    };
    
    // Feat selection
    public ObservableCollection<Feat> AvailableFeats { get; set; } = new();
    
    private Feat? _selectedFeat;
    public Feat? SelectedFeat
    {
        get => _selectedFeat;
        set
        {
            _selectedFeat = value;
            OnPropertyChanged(nameof(SelectedFeat));
            UpdateCompletionStatus();
        }
    }

    public int TotalPointsUsed => AbilityScoreSelections.Sum(s => s.Improvement);
    public int RemainingPoints => 2 - TotalPointsUsed;
    public bool CanAddMorePoints => RemainingPoints > 0;

    public void UpdateCompletionStatus()
    {
        if (IsChoosingFeat && CanChooseFeat)
        {
            IsCompleted = SelectedFeat != null;
        }
        else if (IsChoosingAbilityScores)
        {
            IsCompleted = TotalPointsUsed == 2;
        }
        else
        {
            IsCompleted = false;
        }
        OnPropertyChanged(nameof(IsCompleted));
        OnPropertyChanged(nameof(TotalPointsUsed));
        OnPropertyChanged(nameof(RemainingPoints));
        OnPropertyChanged(nameof(CanAddMorePoints));
    }
}

public enum ASIChoiceType
{
    AbilityScoreIncrease,
    Feat
}

public class AbilityScoreSelection : INotifyPropertyChanged
{
    public string AbilityScore { get; set; } = string.Empty;
    
    private int _improvement;
    private AbilityScoreImprovementChoice? _parentChoice;
    
    public int Improvement 
    { 
        get => _improvement;
        set
        {
            var newValue = Math.Max(0, Math.Min(2, value));
            
            // Check if this change would exceed the 2-point limit
            if (_parentChoice != null)
            {
                var currentTotal = _parentChoice.AbilityScoreSelections.Sum(s => s.AbilityScore == AbilityScore ? 0 : s.Improvement);
                if (currentTotal + newValue > 2)
                {
                    // Don't allow the change if it would exceed the limit
                    return;
                }
                
                // Check if this would make the ability score exceed 20
                var wouldExceedCap = WouldExceedAbilityScoreCap(newValue);
                if (wouldExceedCap)
                {
                    // Don't allow the change if it would exceed the 20 cap
                    return;
                }
            }
            
            _improvement = newValue;
            OnPropertyChanged(nameof(Improvement));
            
            // Notify parent choice to update completion status
            _parentChoice?.UpdateCompletionStatus();
        }
    }

    // Unique group name for radio buttons - combines parent choice ID with ability score
    public string UniqueGroupName => _parentChoice != null ? $"{_parentChoice.Id}_{AbilityScore}" : AbilityScore;

    public void SetParentChoice(AbilityScoreImprovementChoice parentChoice)
    {
        _parentChoice = parentChoice;
        OnPropertyChanged(nameof(UniqueGroupName)); // Notify that UniqueGroupName changed
    }

    // Check if applying the proposed improvement would exceed the 20 ability score cap
    private bool WouldExceedAbilityScoreCap(int proposedImprovement)
    {
        if (_parentChoice?.Character == null) return false;
        
        var character = _parentChoice.Character;
        
        // Get base score + species + background bonuses (without any ASI)
        var baseScore = AbilityScore.ToLowerInvariant() switch
        {
            "strength" => character.AbilityScores.Strength + character.StrengthSpeciesBonus + character.StrengthBackgroundBonus,
            "dexterity" => character.AbilityScores.Dexterity + character.DexteritySpeciesBonus + character.DexterityBackgroundBonus,
            "constitution" => character.AbilityScores.Constitution + character.ConstitutionSpeciesBonus + character.ConstitutionBackgroundBonus,
            "intelligence" => character.AbilityScores.Intelligence + character.IntelligenceSpeciesBonus + character.IntelligenceBackgroundBonus,
            "wisdom" => character.AbilityScores.Wisdom + character.WisdomSpeciesBonus + character.WisdomBackgroundBonus,
            "charisma" => character.AbilityScores.Charisma + character.CharismaSpeciesBonus + character.CharismaBackgroundBonus,
            _ => null
        };
        
        if (!baseScore.HasValue) return false;
        
        // Get current ASI total from character (this includes all applied ASI improvements)
        var currentASI = AbilityScore.ToLowerInvariant() switch
        {
            "strength" => character.StrengthASI ?? 0,
            "dexterity" => character.DexterityASI ?? 0,
            "constitution" => character.ConstitutionASI ?? 0,
            "intelligence" => character.IntelligenceASI ?? 0,
            "wisdom" => character.WisdomASI ?? 0,
            "charisma" => character.CharismaASI ?? 0,
            _ => 0
        };
        
        // Remove this selection's current contribution and add the proposed improvement
        var adjustedASI = currentASI - _improvement + proposedImprovement;
        
        // Check if total would exceed 20
        var totalScore = baseScore.Value + adjustedASI;
        return totalScore > 20;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
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
    
    // Enhanced skill options with selection tracking
    public ObservableCollection<SkillOption> SkillOptions { get; set; } = new();
    public ObservableCollection<ToolOption> ToolOptions { get; set; } = new();

    public bool CanChooseSkills => AvailableSkills.Count > 0 || SkillOptions.Count > 0;
    public bool CanChooseTools => AvailableTools.Count > 0 || ToolOptions.Count > 0;

    public int SelectedCount => SkillOptions.Count(s => s.IsSelected) + ToolOptions.Count(t => t.IsSelected);
    public bool CanSelectMore => SelectedCount < NumberToChoose;

    public void UpdateCompletionStatus()
    {
        IsCompleted = SelectedCount == NumberToChoose;
        OnPropertyChanged(nameof(IsCompleted));
        OnPropertyChanged(nameof(SelectedCount));
        OnPropertyChanged(nameof(CanSelectMore));
        
        // Update CanBeSelected for all options
        foreach (var skill in SkillOptions)
        {
            skill.UpdateCanBeSelected(CanSelectMore);
        }
        foreach (var tool in ToolOptions)
        {
            tool.UpdateCanBeSelected(CanSelectMore);
        }
    }
}

public class SkillOption : INotifyPropertyChanged
{
    public string Name { get; set; } = string.Empty;
    
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }
    
    private bool _canBeSelected = true;
    public bool CanBeSelected
    {
        get => _canBeSelected;
        set
        {
            _canBeSelected = value;
            OnPropertyChanged(nameof(CanBeSelected));
        }
    }

    public void UpdateCanBeSelected(bool canSelectMore)
    {
        CanBeSelected = IsSelected || canSelectMore;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ToolOption : INotifyPropertyChanged
{
    public string Name { get; set; } = string.Empty;
    
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }
    
    private bool _canBeSelected = true;
    public bool CanBeSelected
    {
        get => _canBeSelected;
        set
        {
            _canBeSelected = value;
            OnPropertyChanged(nameof(CanBeSelected));
        }
    }

    public void UpdateCanBeSelected(bool canSelectMore)
    {
        CanBeSelected = IsSelected || canSelectMore;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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