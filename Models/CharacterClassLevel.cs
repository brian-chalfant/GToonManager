using System.ComponentModel;
using System.Collections.ObjectModel;

namespace GToonManager.Models;

public class CharacterClassLevel : INotifyPropertyChanged
{
    private CharacterClass? _characterClass;
    private int _level = 1;
    private ObservableCollection<string> _chosenSkillProficiencies = new();
    private Subclass? _chosenSubclass;

    public CharacterClass? CharacterClass
    {
        get => _characterClass;
        set
        {
            _characterClass = value;
            OnPropertyChanged(nameof(CharacterClass));
            OnPropertyChanged(nameof(ClassName));
        }
    }

    public int Level
    {
        get => _level;
        set
        {
            _level = Math.Max(1, Math.Min(20, value)); // Clamp between 1 and 20
            OnPropertyChanged(nameof(Level));
        }
    }

    public string ClassName => CharacterClass?.Name ?? "Unknown Class";

    public ObservableCollection<string> ChosenSkillProficiencies
    {
        get => _chosenSkillProficiencies;
        set
        {
            _chosenSkillProficiencies = value ?? new ObservableCollection<string>();
            OnPropertyChanged(nameof(ChosenSkillProficiencies));
        }
    }

    public Subclass? ChosenSubclass
    {
        get => _chosenSubclass;
        set
        {
            _chosenSubclass = value;
            OnPropertyChanged(nameof(ChosenSubclass));
            OnPropertyChanged(nameof(SubclassName));
            OnPropertyChanged(nameof(HasSubclass));
            OnPropertyChanged(nameof(CanChooseSubclass));
            OnPropertyChanged(nameof(ClassWithSubclass));
        }
    }

    public string SubclassName => ChosenSubclass?.Name ?? "No Subclass";
    public bool HasSubclass => ChosenSubclass != null;
    public bool CanChooseSubclass => CharacterClass?.CanChooseSubclass(Level) == true && !HasSubclass;
    
    // Property for display with subclass
    public string ClassWithSubclass => HasSubclass ? $"{ClassName} - {SubclassName}" : ClassName;

    public override string ToString() => HasSubclass ? $"{ClassName} - {SubclassName} {Level}" : $"{ClassName} {Level}";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 