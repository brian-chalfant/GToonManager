using System.ComponentModel;

namespace GToonManager.Models;

public class Race : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _source = string.Empty;
    private int _sourcePage = 0;
    private Dictionary<string, int> _abilityScores = new();
    private List<RaceTrait> _traits = new();
    private List<Subrace> _subraces = new();
    private RaceSize _size = new();
    private RaceSpeed _speed = new();
    private RaceLanguages _languages = new();
    private RaceAge _age = new();

    public string Name
    {
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

    public Dictionary<string, int> AbilityScores
    {
        get => _abilityScores;
        set
        {
            _abilityScores = value;
            OnPropertyChanged(nameof(AbilityScores));
        }
    }

    public List<RaceTrait> Traits
    {
        get => _traits;
        set
        {
            _traits = value;
            OnPropertyChanged(nameof(Traits));
        }
    }

    public List<Subrace> Subraces
    {
        get => _subraces;
        set
        {
            _subraces = value;
            OnPropertyChanged(nameof(Subraces));
        }
    }

    public RaceSize Size
    {
        get => _size;
        set
        {
            _size = value;
            OnPropertyChanged(nameof(Size));
        }
    }

    public RaceSpeed Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            OnPropertyChanged(nameof(Speed));
        }
    }

    public RaceLanguages Languages
    {
        get => _languages;
        set
        {
            _languages = value;
            OnPropertyChanged(nameof(Languages));
        }
    }

    public RaceAge Age
    {
        get => _age;
        set
        {
            _age = value;
            OnPropertyChanged(nameof(Age));
        }
    }

    public bool HasSubraces => Subraces?.Count > 0;

    public override string ToString() => Name;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class Subrace : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _source = string.Empty;
    private Dictionary<string, int> _abilityScores = new();
    private List<RaceTrait> _traits = new();
    private RaceLanguages? _languages;

    public string Name
    {
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

    public string Source
    {
        get => _source;
        set
        {
            _source = value;
            OnPropertyChanged(nameof(Source));
        }
    }

    public Dictionary<string, int> AbilityScores
    {
        get => _abilityScores;
        set
        {
            _abilityScores = value;
            OnPropertyChanged(nameof(AbilityScores));
        }
    }

    public List<RaceTrait> Traits
    {
        get => _traits;
        set
        {
            _traits = value;
            OnPropertyChanged(nameof(Traits));
        }
    }

    public RaceLanguages? Languages
    {
        get => _languages;
        set
        {
            _languages = value;
            OnPropertyChanged(nameof(Languages));
        }
    }

    public override string ToString() => Name;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RaceTrait
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Range { get; set; } = 0;
    public TraitGrants? Grants { get; set; }
    public TraitModifies? Modifies { get; set; }
    public TraitSpellcasting? Spellcasting { get; set; }
    public TraitUses? Uses { get; set; }
}

public class TraitGrants
{
    public List<string> SkillProficiencies { get; set; } = new();
    public List<string> WeaponProficiencies { get; set; } = new();
    public List<string> ArmorProficiencies { get; set; } = new();
    public List<string> ToolProficiencies { get; set; } = new();
    public List<string> DamageResistances { get; set; } = new();
    public List<string> SavingThrowAdvantages { get; set; } = new();
    public Dictionary<string, object> Speed { get; set; } = new();
}

public class TraitModifies
{
    public Dictionary<string, object> Properties { get; set; } = new();
}

public class TraitSpellcasting
{
    public string Ability { get; set; } = string.Empty;
    public Dictionary<string, object> Innate { get; set; } = new();
    public Dictionary<string, object> Spells { get; set; } = new();
}

public class TraitUses
{
    public string Per { get; set; } = string.Empty;
    public int Count { get; set; } = 1;
}

public class RaceSize
{
    public string Category { get; set; } = "Medium";
    public string Height { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
}

public class RaceSpeed
{
    public int Walk { get; set; } = 30;
    public int Swim { get; set; } = 0;
    public int Fly { get; set; } = 0;
    public int Climb { get; set; } = 0;
    public Dictionary<string, string> Modifiers { get; set; } = new();
}

public class RaceLanguages
{
    public List<string> Standard { get; set; } = new();
    public LanguageBonus? Bonus { get; set; }
}

public class LanguageBonus
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; } = 0;
    public List<string> From { get; set; } = new();
}

public class RaceAge
{
    public int Maturity { get; set; } = 18;
    public string Lifespan { get; set; } = string.Empty;
} 