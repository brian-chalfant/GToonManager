using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GToonManager.Models;

public class Character : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _playerName = string.Empty;
    private Species? _species;
    private Subspecies? _subspecies;
    private CharacterClass? _class; // Kept for backward compatibility
    private Background? _background;
    private BackgroundAbilityScoreChoice? _backgroundAbilityScoreChoice;
    private AbilityScores _abilityScores = new();
    private int _armorClass = 10;
    private int _hitPoints = 8;
    private int _maxHitPoints = 8;
    private int _speed = 30;
    private int _experiencePoints = 0;

    // Multiclass support
    private ObservableCollection<CharacterClassLevel> _classLevels = new();

    public Character()
    {
        _classLevels.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(ProficiencyBonus));
            OnPropertyChanged(nameof(ClassSummary));
            OnPropertyChanged(nameof(ClassSkillProficiencies));
            OnPropertyChanged(nameof(CalculatedMaxHitPoints));
            
            // Subscribe to new items and unsubscribe from old items
            if (e.NewItems != null)
            {
                foreach (CharacterClassLevel classLevel in e.NewItems)
                {
                    classLevel.PropertyChanged += ClassLevel_PropertyChanged;
                    classLevel.ChosenSkillProficiencies.CollectionChanged += ChosenSkillProficiencies_CollectionChanged;
                }
            }
            
            if (e.OldItems != null)
            {
                foreach (CharacterClassLevel classLevel in e.OldItems)
                {
                    classLevel.PropertyChanged -= ClassLevel_PropertyChanged;
                    classLevel.ChosenSkillProficiencies.CollectionChanged -= ChosenSkillProficiencies_CollectionChanged;
                }
            }
        };

        // Subscribe to ability score changes
        _abilityScores.PropertyChanged += AbilityScores_PropertyChanged;
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string PlayerName
    {
        get => _playerName;
        set
        {
            _playerName = value;
            OnPropertyChanged(nameof(PlayerName));
        }
    }

    public int Level => ClassLevels.Sum(cl => cl.Level);

    public Species? Species
    {
        get => _species;
        set
        {
            _species = value;
            // Clear subspecies when species changes
            if (_subspecies != null && (_species == null || !_species.Subraces.Contains(_subspecies)))
            {
                Subspecies = null;
            }
            OnPropertyChanged(nameof(Species));
            OnPropertyChanged(nameof(AvailableSubspecies));
            OnPropertyChanged(nameof(HasSubspecies));
            RefreshAbilityScoreModifiers();
        }
    }

    public Subspecies? Subspecies
    {
        get => _subspecies;
        set
        {
            _subspecies = value;
            OnPropertyChanged(nameof(Subspecies));
            RefreshAbilityScoreModifiers();
        }
    }

    public List<Subspecies> AvailableSubspecies => Species?.Subraces ?? new List<Subspecies>();
    public bool HasSubspecies => Species?.HasSubraces ?? false;

    public CharacterClass? Class
    {
        get => _class;
        set
        {
            _class = value;
            OnPropertyChanged(nameof(Class));
        }
    }

    public ObservableCollection<CharacterClassLevel> ClassLevels
    {
        get => _classLevels;
        set
        {
            _classLevels = value;
            OnPropertyChanged(nameof(ClassLevels));
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(ProficiencyBonus));
            OnPropertyChanged(nameof(ClassSummary));
        }
    }

    public string ClassSummary
    {
        get
        {
            if (ClassLevels.Count == 0) return "No Classes";
            if (ClassLevels.Count == 1) return ClassLevels[0].ToString();
            return string.Join(" / ", ClassLevels.Select(cl => cl.ToString()));
        }
    }

    public Background? Background
    {
        get => _background;
        set
        {
            _background = value;
            OnPropertyChanged(nameof(Background));
            // Clear background ability score choice when background changes
            BackgroundAbilityScoreChoice = null;
        }
    }

    public BackgroundAbilityScoreChoice? BackgroundAbilityScoreChoice
    {
        get => _backgroundAbilityScoreChoice;
        set
        {
            _backgroundAbilityScoreChoice = value;
            OnPropertyChanged(nameof(BackgroundAbilityScoreChoice));
            RefreshAbilityScoreModifiers();
        }
    }

    public AbilityScores AbilityScores
    {
        get => _abilityScores;
        set
        {
            // Unsubscribe from old instance
            if (_abilityScores != null)
            {
                _abilityScores.PropertyChanged -= AbilityScores_PropertyChanged;
            }

            _abilityScores = value;

            // Subscribe to new instance
            if (_abilityScores != null)
            {
                _abilityScores.PropertyChanged += AbilityScores_PropertyChanged;
            }

            OnPropertyChanged(nameof(AbilityScores));
            RefreshAbilityScoreModifiers();
        }
    }

    public int ArmorClass
    {
        get => _armorClass;
        set
        {
            _armorClass = value;
            OnPropertyChanged(nameof(ArmorClass));
        }
    }

    public int HitPoints
    {
        get => _hitPoints;
        set
        {
            _hitPoints = value;
            OnPropertyChanged(nameof(HitPoints));
        }
    }

    public int MaxHitPoints
    {
        get => _maxHitPoints;
        set
        {
            _maxHitPoints = value;
            OnPropertyChanged(nameof(MaxHitPoints));
        }
    }

    public int Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            OnPropertyChanged(nameof(Speed));
        }
    }

    public int ExperiencePoints
    {
        get => _experiencePoints;
        set
        {
            _experiencePoints = value;
            OnPropertyChanged(nameof(ExperiencePoints));
        }
    }

    // Calculated properties
    public int? StrengthModifier => AbilityScores.Strength.HasValue ? CalculateModifier(AbilityScores.Strength.Value) : (int?)null;
    public int? DexterityModifier => AbilityScores.Dexterity.HasValue ? CalculateModifier(AbilityScores.Dexterity.Value) : (int?)null;
    public int? ConstitutionModifier => AbilityScores.Constitution.HasValue ? CalculateModifier(AbilityScores.Constitution.Value) : (int?)null;
    public int? IntelligenceModifier => AbilityScores.Intelligence.HasValue ? CalculateModifier(AbilityScores.Intelligence.Value) : (int?)null;
    public int? WisdomModifier => AbilityScores.Wisdom.HasValue ? CalculateModifier(AbilityScores.Wisdom.Value) : (int?)null;
    public int? CharismaModifier => AbilityScores.Charisma.HasValue ? CalculateModifier(AbilityScores.Charisma.Value) : (int?)null;

    public int ProficiencyBonus => (Level - 1) / 4 + 2;

    // Species bonuses (deprecated in D&D 2024 - abilities come from backgrounds)
    public int StrengthSpeciesBonus => GetSpeciesBonus("strength");
    public int DexteritySpeciesBonus => GetSpeciesBonus("dexterity");
    public int ConstitutionSpeciesBonus => GetSpeciesBonus("constitution");
    public int IntelligenceSpeciesBonus => GetSpeciesBonus("intelligence");
    public int WisdomSpeciesBonus => GetSpeciesBonus("wisdom");
    public int CharismaSpeciesBonus => GetSpeciesBonus("charisma");

    // Background bonuses
    public int StrengthBackgroundBonus => GetBackgroundBonus("strength");
    public int DexterityBackgroundBonus => GetBackgroundBonus("dexterity");
    public int ConstitutionBackgroundBonus => GetBackgroundBonus("constitution");
    public int IntelligenceBackgroundBonus => GetBackgroundBonus("intelligence");
    public int WisdomBackgroundBonus => GetBackgroundBonus("wisdom");
    public int CharismaBackgroundBonus => GetBackgroundBonus("charisma");

    // Total ability scores including species and background bonuses
    public int? StrengthTotal => AbilityScores.Strength.HasValue ? AbilityScores.Strength.Value + StrengthSpeciesBonus + StrengthBackgroundBonus : (int?)null;
    public int? DexterityTotal => AbilityScores.Dexterity.HasValue ? AbilityScores.Dexterity.Value + DexteritySpeciesBonus + DexterityBackgroundBonus : (int?)null;
    public int? ConstitutionTotal => AbilityScores.Constitution.HasValue ? AbilityScores.Constitution.Value + ConstitutionSpeciesBonus + ConstitutionBackgroundBonus : (int?)null;
    public int? IntelligenceTotal => AbilityScores.Intelligence.HasValue ? AbilityScores.Intelligence.Value + IntelligenceSpeciesBonus + IntelligenceBackgroundBonus : (int?)null;
    public int? WisdomTotal => AbilityScores.Wisdom.HasValue ? AbilityScores.Wisdom.Value + WisdomSpeciesBonus + WisdomBackgroundBonus : (int?)null;
    public int? CharismaTotal => AbilityScores.Charisma.HasValue ? AbilityScores.Charisma.Value + CharismaSpeciesBonus + CharismaBackgroundBonus : (int?)null;

    // Skill proficiencies from species
    public List<string> SpeciesSkillProficiencies => GetSpeciesSkillProficiencies();
    
    // Skill proficiencies from background
    public List<string> BackgroundSkillProficiencies => GetBackgroundSkillProficiencies();
    
    // Skill proficiencies from classes
    public List<string> ClassSkillProficiencies => GetClassSkillProficiencies();

    // Calculated hit points based on settings
    public int CalculatedMaxHitPoints => CalculateMaxHitPoints();
    
    // Get the current hit point calculation method description
    public string HitPointCalculationMethodDescription
    {
        get
        {
            var method = _settings?.HitPointCalculation ?? HitPointCalculationMethod.Rolled;
            return method switch
            {
                HitPointCalculationMethod.Average => "Average method",
                HitPointCalculationMethod.Rolled => "Rolled method",
                HitPointCalculationMethod.Maximum => "Maximum method",
                _ => "Unknown method"
            };
        }
    }
    
    // Settings reference for calculations
    private Settings? _settings;
    
    // Method to set settings reference
    public void SetSettings(Settings settings)
    {
        _settings = settings;
        OnPropertyChanged(nameof(CalculatedMaxHitPoints));
        OnPropertyChanged(nameof(HitPointCalculationMethodDescription));
    }
    
    // Calculated armor class based on D&D 2024 rules
    public int CalculatedArmorClass => CalculateArmorClass();

    // Property to check if ability scores have been generated (any score is not null)
    public bool HasGeneratedAbilityScores => 
        AbilityScores.Strength.HasValue || AbilityScores.Dexterity.HasValue || 
        AbilityScores.Constitution.HasValue || AbilityScores.Intelligence.HasValue || 
        AbilityScores.Wisdom.HasValue || AbilityScores.Charisma.HasValue;

    private static int CalculateModifier(int score)
    {
        return (score - 10) / 2;
    }

    private int GetSpeciesBonus(string abilityName)
    {
        int bonus = 0;

        // Base species bonuses (deprecated in D&D 2024)
        if (Species?.AbilityScores?.TryGetValue(abilityName, out var speciesBonus) == true)
        {
            bonus += speciesBonus;
        }

        // Subspecies bonuses (deprecated in D&D 2024)
        if (Subspecies?.AbilityScores?.TryGetValue(abilityName, out var subspeciesBonus) == true)
        {
            bonus += subspeciesBonus;
        }

        return bonus;
    }

    private int GetBackgroundBonus(string abilityName)
    {
        if (BackgroundAbilityScoreChoice?.AbilityScoreImprovements != null)
        {
            // Use case-insensitive lookup to handle both lowercase and uppercase ability names
            var kvp = BackgroundAbilityScoreChoice.AbilityScoreImprovements
                .FirstOrDefault(x => string.Equals(x.Key, abilityName, StringComparison.OrdinalIgnoreCase));
            
            if (!kvp.Equals(default(KeyValuePair<string, int>)))
            {
                return kvp.Value;
            }
        }
        return 0;
    }

    private List<string> GetSpeciesSkillProficiencies()
    {
        var skills = new List<string>();

        // Add skills from species traits
        if (Species?.Traits != null)
        {
            foreach (var trait in Species.Traits)
            {
                if (trait.Grants?.SkillProficiencies != null)
                {
                    skills.AddRange(trait.Grants.SkillProficiencies);
                }
            }
        }

        // Add skills from subspecies traits
        if (Subspecies?.Traits != null)
        {
            foreach (var trait in Subspecies.Traits)
            {
                if (trait.Grants?.SkillProficiencies != null)
                {
                    skills.AddRange(trait.Grants.SkillProficiencies);
                }
            }
        }

        return skills.Distinct().ToList();
    }

    private List<string> GetBackgroundSkillProficiencies()
    {
        var skills = new List<string>();

        // Add skills from background
        if (Background?.ProficiencyGrants?.Skills != null)
        {
            skills.AddRange(Background.ProficiencyGrants.Skills);
        }

        return skills.Distinct().ToList();
    }

    private List<string> GetClassSkillProficiencies()
    {
        var skills = new List<string>();

        // Add skills from class levels (chosen skill proficiencies)
        foreach (var classLevel in ClassLevels)
        {
            if (classLevel.ChosenSkillProficiencies != null)
            {
                skills.AddRange(classLevel.ChosenSkillProficiencies);
            }
        }

        return skills.Distinct().ToList();
    }

    private int CalculateMaxHitPoints()
    {
        if (ClassLevels.Count == 0) return 8; // Default starting HP
        
        int totalHp = 0;
        bool isFirstLevel = true;
        var calculationMethod = _settings?.HitPointCalculation ?? HitPointCalculationMethod.Rolled;
        
        foreach (var classLevel in ClassLevels)
        {
            if (classLevel.CharacterClass == null) continue;
            
            var hitDie = classLevel.CharacterClass.HitDie;
            var classLevels = classLevel.Level;
            
            for (int level = 1; level <= classLevels; level++)
            {
                if (isFirstLevel)
                {
                    // First level: always max hit die + Constitution modifier
                    totalHp += hitDie + (ConstitutionModifier ?? 0);
                    isFirstLevel = false;
                }
                else
                {
                    // Subsequent levels: based on calculation method
                    int levelHp = calculationMethod switch
                    {
                        HitPointCalculationMethod.Average => (hitDie + 1) / 2, // Average, rounded up
                        HitPointCalculationMethod.Rolled => CalculateRolledHitPoints(hitDie), // Simulated good rolls
                        HitPointCalculationMethod.Maximum => hitDie, // Maximum possible
                        _ => (hitDie + 1) / 2 // Default to average
                    };
                    totalHp += levelHp + (ConstitutionModifier ?? 0);
                }
            }
        }
        
        return Math.Max(1, totalHp); // Minimum 1 HP
    }

    private static int CalculateRolledHitPoints(int hitDie)
    {
        // Simulate favorable rolls - typically slightly above average
        // This provides a consistent "rolled" feel without actual randomness
        return hitDie switch
        {
            6 => 4,   // d6: average 3.5, rolled 4
            8 => 5,   // d8: average 4.5, rolled 5  
            10 => 6,  // d10: average 5.5, rolled 6
            12 => 7,  // d12: average 6.5, rolled 7
            _ => (hitDie + 1) / 2 + 1 // For any other die, average + 1
        };
    }

    private int CalculateArmorClass()
    {
        // D&D 2024 base AC calculation
        // Base AC = 10 + Dex modifier (when not wearing armor)
        // TODO: When armor system is implemented, this will need to check for equipped armor
        
        return 10 + (DexterityModifier ?? 0);
    }

    private void RefreshAbilityScoreModifiers()
    {
        OnPropertyChanged(nameof(StrengthSpeciesBonus));
        OnPropertyChanged(nameof(DexteritySpeciesBonus));
        OnPropertyChanged(nameof(ConstitutionSpeciesBonus));
        OnPropertyChanged(nameof(IntelligenceSpeciesBonus));
        OnPropertyChanged(nameof(WisdomSpeciesBonus));
        OnPropertyChanged(nameof(CharismaSpeciesBonus));
        OnPropertyChanged(nameof(StrengthBackgroundBonus));
        OnPropertyChanged(nameof(DexterityBackgroundBonus));
        OnPropertyChanged(nameof(ConstitutionBackgroundBonus));
        OnPropertyChanged(nameof(IntelligenceBackgroundBonus));
        OnPropertyChanged(nameof(WisdomBackgroundBonus));
        OnPropertyChanged(nameof(CharismaBackgroundBonus));
        OnPropertyChanged(nameof(StrengthTotal));
        OnPropertyChanged(nameof(DexterityTotal));
        OnPropertyChanged(nameof(ConstitutionTotal));
        OnPropertyChanged(nameof(IntelligenceTotal));
        OnPropertyChanged(nameof(WisdomTotal));
        OnPropertyChanged(nameof(CharismaTotal));
        OnPropertyChanged(nameof(SpeciesSkillProficiencies));
        OnPropertyChanged(nameof(BackgroundSkillProficiencies));
        OnPropertyChanged(nameof(ClassSkillProficiencies));
        OnPropertyChanged(nameof(CalculatedMaxHitPoints));
        OnPropertyChanged(nameof(CalculatedArmorClass));
    }

    // Multiclass helper methods
    public void AddClass(CharacterClass characterClass, int level = 1)
    {
        var existingClass = ClassLevels.FirstOrDefault(cl => cl.CharacterClass?.Name == characterClass.Name);
        if (existingClass != null)
        {
            existingClass.Level += level;
        }
        else
        {
            var newClassLevel = new CharacterClassLevel { CharacterClass = characterClass, Level = level };
            newClassLevel.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Level));
                OnPropertyChanged(nameof(ProficiencyBonus));
                OnPropertyChanged(nameof(ClassSummary));
            };
            ClassLevels.Add(newClassLevel);
        }
    }

    public void RemoveClass(CharacterClass characterClass)
    {
        var classLevel = ClassLevels.FirstOrDefault(cl => cl.CharacterClass?.Name == characterClass.Name);
        if (classLevel != null)
        {
            ClassLevels.Remove(classLevel);
        }
    }

    public int GetClassLevel(CharacterClass characterClass)
    {
        return ClassLevels.FirstOrDefault(cl => cl.CharacterClass?.Name == characterClass.Name)?.Level ?? 0;
    }

    private void AbilityScores_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // When an ability score changes, notify all dependent properties
        switch (e.PropertyName)
        {
            case nameof(AbilityScores.Strength):
                OnPropertyChanged(nameof(StrengthModifier));
                OnPropertyChanged(nameof(StrengthTotal));
                break;
            case nameof(AbilityScores.Dexterity):
                OnPropertyChanged(nameof(DexterityModifier));
                OnPropertyChanged(nameof(DexterityTotal));
                OnPropertyChanged(nameof(CalculatedArmorClass));
                break;
            case nameof(AbilityScores.Constitution):
                OnPropertyChanged(nameof(ConstitutionModifier));
                OnPropertyChanged(nameof(ConstitutionTotal));
                OnPropertyChanged(nameof(CalculatedMaxHitPoints));
                break;
            case nameof(AbilityScores.Intelligence):
                OnPropertyChanged(nameof(IntelligenceModifier));
                OnPropertyChanged(nameof(IntelligenceTotal));
                break;
            case nameof(AbilityScores.Wisdom):
                OnPropertyChanged(nameof(WisdomModifier));
                OnPropertyChanged(nameof(WisdomTotal));
                break;
            case nameof(AbilityScores.Charisma):
                OnPropertyChanged(nameof(CharismaModifier));
                OnPropertyChanged(nameof(CharismaTotal));
                break;
        }
        
        // Always notify that the generated status may have changed
        OnPropertyChanged(nameof(HasGeneratedAbilityScores));
    }

    private void ClassLevel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // When a class level property changes, update relevant character properties
        if (e.PropertyName == nameof(CharacterClassLevel.Level))
        {
            OnPropertyChanged(nameof(Level));
            OnPropertyChanged(nameof(ProficiencyBonus));
            OnPropertyChanged(nameof(CalculatedMaxHitPoints));
        }
        else if (e.PropertyName == nameof(CharacterClassLevel.CharacterClass))
        {
            OnPropertyChanged(nameof(ClassSummary));
            OnPropertyChanged(nameof(ClassSkillProficiencies));
            OnPropertyChanged(nameof(CalculatedMaxHitPoints));
        }
    }

    private void ChosenSkillProficiencies_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // When skill proficiencies change in any class level, update the class skills display
        OnPropertyChanged(nameof(ClassSkillProficiencies));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 