using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GToonManager.Models;

public class Character : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _playerName = string.Empty;
    private Race? _race;
    private Subrace? _subrace;
    private CharacterClass? _class; // Kept for backward compatibility
    private Background? _background;
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

    public Race? Race
    {
        get => _race;
        set
        {
            _race = value;
            // Clear subrace when race changes
            if (_subrace != null && (_race == null || !_race.Subraces.Contains(_subrace)))
            {
                Subrace = null;
            }
            OnPropertyChanged(nameof(Race));
            OnPropertyChanged(nameof(AvailableSubraces));
            OnPropertyChanged(nameof(HasSubraces));
            RefreshAbilityScoreModifiers();
        }
    }

    public Subrace? Subrace
    {
        get => _subrace;
        set
        {
            _subrace = value;
            OnPropertyChanged(nameof(Subrace));
            RefreshAbilityScoreModifiers();
        }
    }

    public List<Subrace> AvailableSubraces => Race?.Subraces ?? new List<Subrace>();
    public bool HasSubraces => Race?.HasSubraces ?? false;

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
    public int StrengthModifier => CalculateModifier(AbilityScores.Strength);
    public int DexterityModifier => CalculateModifier(AbilityScores.Dexterity);
    public int ConstitutionModifier => CalculateModifier(AbilityScores.Constitution);
    public int IntelligenceModifier => CalculateModifier(AbilityScores.Intelligence);
    public int WisdomModifier => CalculateModifier(AbilityScores.Wisdom);
    public int CharismaModifier => CalculateModifier(AbilityScores.Charisma);

    public int ProficiencyBonus => (Level - 1) / 4 + 2;

    // Racial bonuses
    public int StrengthRacialBonus => GetRacialBonus("strength");
    public int DexterityRacialBonus => GetRacialBonus("dexterity");
    public int ConstitutionRacialBonus => GetRacialBonus("constitution");
    public int IntelligenceRacialBonus => GetRacialBonus("intelligence");
    public int WisdomRacialBonus => GetRacialBonus("wisdom");
    public int CharismaRacialBonus => GetRacialBonus("charisma");

    // Total ability scores including racial bonuses
    public int StrengthTotal => AbilityScores.Strength + StrengthRacialBonus;
    public int DexterityTotal => AbilityScores.Dexterity + DexterityRacialBonus;
    public int ConstitutionTotal => AbilityScores.Constitution + ConstitutionRacialBonus;
    public int IntelligenceTotal => AbilityScores.Intelligence + IntelligenceRacialBonus;
    public int WisdomTotal => AbilityScores.Wisdom + WisdomRacialBonus;
    public int CharismaTotal => AbilityScores.Charisma + CharismaRacialBonus;

    // Skill proficiencies from race
    public List<string> RacialSkillProficiencies => GetRacialSkillProficiencies();

    private static int CalculateModifier(int score)
    {
        return (score - 10) / 2;
    }

    private int GetRacialBonus(string abilityName)
    {
        int bonus = 0;

        // Base race bonuses
        if (Race?.AbilityScores?.TryGetValue(abilityName, out var raceBonus) == true)
        {
            bonus += raceBonus;
        }

        // Subrace bonuses
        if (Subrace?.AbilityScores?.TryGetValue(abilityName, out var subraceBonus) == true)
        {
            bonus += subraceBonus;
        }

        return bonus;
    }

    private List<string> GetRacialSkillProficiencies()
    {
        var skills = new List<string>();

        // Add skills from race traits
        if (Race?.Traits != null)
        {
            foreach (var trait in Race.Traits)
            {
                if (trait.Grants?.SkillProficiencies != null)
                {
                    skills.AddRange(trait.Grants.SkillProficiencies);
                }
            }
        }

        // Add skills from subrace traits
        if (Subrace?.Traits != null)
        {
            foreach (var trait in Subrace.Traits)
            {
                if (trait.Grants?.SkillProficiencies != null)
                {
                    skills.AddRange(trait.Grants.SkillProficiencies);
                }
            }
        }

        return skills.Distinct().ToList();
    }

    private void RefreshAbilityScoreModifiers()
    {
        OnPropertyChanged(nameof(StrengthRacialBonus));
        OnPropertyChanged(nameof(DexterityRacialBonus));
        OnPropertyChanged(nameof(ConstitutionRacialBonus));
        OnPropertyChanged(nameof(IntelligenceRacialBonus));
        OnPropertyChanged(nameof(WisdomRacialBonus));
        OnPropertyChanged(nameof(CharismaRacialBonus));
        OnPropertyChanged(nameof(StrengthTotal));
        OnPropertyChanged(nameof(DexterityTotal));
        OnPropertyChanged(nameof(ConstitutionTotal));
        OnPropertyChanged(nameof(IntelligenceTotal));
        OnPropertyChanged(nameof(WisdomTotal));
        OnPropertyChanged(nameof(CharismaTotal));
        OnPropertyChanged(nameof(RacialSkillProficiencies));
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
                break;
            case nameof(AbilityScores.Constitution):
                OnPropertyChanged(nameof(ConstitutionModifier));
                OnPropertyChanged(nameof(ConstitutionTotal));
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
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 