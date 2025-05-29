using System.ComponentModel;
using System.Windows.Input;
using GToonManager.Models;

namespace GToonManager.ViewModels;

public class StandardArrayViewModel : INotifyPropertyChanged
{
    private StandardArray _standardArray;
    private Character _character;
    private bool _isApplied = false;

    public StandardArrayViewModel(Character character)
    {
        _character = character;
        _standardArray = new StandardArray();
        
        // Load current ability scores if they match standard array values
        _standardArray.LoadFromAbilityScores(character.AbilityScores);
        
        // Subscribe to standard array changes
        _standardArray.PropertyChanged += StandardArray_PropertyChanged;
        
        InitializeCommands();
    }

    public StandardArray StandardArray
    {
        get => _standardArray;
        set
        {
            if (_standardArray != null)
                _standardArray.PropertyChanged -= StandardArray_PropertyChanged;
                
            _standardArray = value;
            
            if (_standardArray != null)
                _standardArray.PropertyChanged += StandardArray_PropertyChanged;
                
            OnPropertyChanged(nameof(StandardArray));
        }
    }

    public Character Character
    {
        get => _character;
        set
        {
            _character = value;
            OnPropertyChanged(nameof(Character));
        }
    }

    public bool IsApplied
    {
        get => _isApplied;
        set
        {
            _isApplied = value;
            OnPropertyChanged(nameof(IsApplied));
            OnPropertyChanged(nameof(IsExpanded));
        }
    }

    public bool IsExpanded => !IsApplied;

    // Commands
    public ICommand ApplyStandardArrayCommand { get; private set; } = null!;
    public ICommand ResetStandardArrayCommand { get; private set; } = null!;
    public ICommand ClearAssignmentCommand { get; private set; } = null!;

    // Separate commands for each ability
    public ICommand AssignStrengthCommand { get; private set; } = null!;
    public ICommand AssignDexterityCommand { get; private set; } = null!;
    public ICommand AssignConstitutionCommand { get; private set; } = null!;
    public ICommand AssignIntelligenceCommand { get; private set; } = null!;
    public ICommand AssignWisdomCommand { get; private set; } = null!;
    public ICommand AssignCharismaCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        ApplyStandardArrayCommand = new RelayCommand(ApplyStandardArray, CanApplyStandardArray);
        ResetStandardArrayCommand = new RelayCommand(ResetStandardArray);
        ClearAssignmentCommand = new RelayCommand(ClearAssignment);
        
        // Individual ability assignment commands
        AssignStrengthCommand = new RelayCommand(parameter => AssignValueToAbility("Strength", parameter));
        AssignDexterityCommand = new RelayCommand(parameter => AssignValueToAbility("Dexterity", parameter));
        AssignConstitutionCommand = new RelayCommand(parameter => AssignValueToAbility("Constitution", parameter));
        AssignIntelligenceCommand = new RelayCommand(parameter => AssignValueToAbility("Intelligence", parameter));
        AssignWisdomCommand = new RelayCommand(parameter => AssignValueToAbility("Wisdom", parameter));
        AssignCharismaCommand = new RelayCommand(parameter => AssignValueToAbility("Charisma", parameter));
    }

    private void ApplyStandardArray()
    {
        if (StandardArray.IsComplete)
        {
            StandardArray.ApplyToAbilityScores(Character.AbilityScores);
            IsApplied = true;
        }
    }

    private bool CanApplyStandardArray()
    {
        return StandardArray.IsComplete;
    }

    private void ResetStandardArray()
    {
        StandardArray = new StandardArray();
        IsApplied = false;
    }

    private void AssignValueToAbility(string ability, object? parameter)
    {
        if (parameter is int value)
        {
            try
            {
                StandardArray.AssignValue(ability, value);
            }
            catch (ArgumentException)
            {
                // Value is not available for assignment - ignore
            }
        }
    }

    private void ClearAssignment(object? parameter)
    {
        if (parameter is string ability)
        {
            StandardArray.ClearAssignment(ability);
        }
    }

    private void StandardArray_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Refresh command states when standard array changes
        if (e.PropertyName == nameof(StandardArray.IsComplete))
        {
            OnPropertyChanged(nameof(CanApplyStandardArray));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 