using System.ComponentModel;

namespace GToonManager.Models;

public class AbilityScores : INotifyPropertyChanged
{
    private int _strength = 10;
    private int _dexterity = 10;
    private int _constitution = 10;
    private int _intelligence = 10;
    private int _wisdom = 10;
    private int _charisma = 10;

    public int Strength
    {
        get => _strength;
        set
        {
            _strength = value;
            OnPropertyChanged(nameof(Strength));
        }
    }

    public int Dexterity
    {
        get => _dexterity;
        set
        {
            _dexterity = value;
            OnPropertyChanged(nameof(Dexterity));
        }
    }

    public int Constitution
    {
        get => _constitution;
        set
        {
            _constitution = value;
            OnPropertyChanged(nameof(Constitution));
        }
    }

    public int Intelligence
    {
        get => _intelligence;
        set
        {
            _intelligence = value;
            OnPropertyChanged(nameof(Intelligence));
        }
    }

    public int Wisdom
    {
        get => _wisdom;
        set
        {
            _wisdom = value;
            OnPropertyChanged(nameof(Wisdom));
        }
    }

    public int Charisma
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