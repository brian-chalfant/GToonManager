using System.ComponentModel;

namespace GToonManager.Models;

public class AbilityScores : INotifyPropertyChanged
{
    private int? _strength = null;
    private int? _dexterity = null;
    private int? _constitution = null;
    private int? _intelligence = null;
    private int? _wisdom = null;
    private int? _charisma = null;

    public int? Strength
    {
        get => _strength;
        set
        {
            _strength = value;
            OnPropertyChanged(nameof(Strength));
        }
    }

    public int? Dexterity
    {
        get => _dexterity;
        set
        {
            _dexterity = value;
            OnPropertyChanged(nameof(Dexterity));
        }
    }

    public int? Constitution
    {
        get => _constitution;
        set
        {
            _constitution = value;
            OnPropertyChanged(nameof(Constitution));
        }
    }

    public int? Intelligence
    {
        get => _intelligence;
        set
        {
            _intelligence = value;
            OnPropertyChanged(nameof(Intelligence));
        }
    }

    public int? Wisdom
    {
        get => _wisdom;
        set
        {
            _wisdom = value;
            OnPropertyChanged(nameof(Wisdom));
        }
    }

    public int? Charisma
    {
        get => _charisma;
        set
        {
            _charisma = value;
            OnPropertyChanged(nameof(Charisma));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 