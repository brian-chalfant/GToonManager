using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using GToonManager.Models;

namespace GToonManager.ViewModels;

public class StandardArrayViewModel : INotifyPropertyChanged
{
    private StandardArray _standardArray;
    private Character _character;
    private bool _isApplied = false;
    private CharacterClass? _selectedClass;
    private ObservableCollection<CharacterClass> _classes;

    public StandardArrayViewModel(Character character)
    {
        _character = character;
        _standardArray = new StandardArray();
        _classes = new ObservableCollection<CharacterClass>();
        
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

    public ObservableCollection<CharacterClass> Classes
    {
        get => _classes;
        set
        {
            _classes = value;
            OnPropertyChanged(nameof(Classes));
        }
    }

    public CharacterClass? SelectedClass
    {
        get => _selectedClass;
        set
        {
            _selectedClass = value;
            OnPropertyChanged(nameof(SelectedClass));
            OnPropertyChanged(nameof(HasClassRecommendation));
            OnPropertyChanged(nameof(ClassRecommendationText));
        }
    }

    public bool HasClassRecommendation 
    { 
        get 
        {
            var hasRec = SelectedClass?.StandardArrayRecommendation != null;
            System.Diagnostics.Debug.WriteLine($"HasClassRecommendation: {hasRec}, SelectedClass: {SelectedClass?.Name}, StandardArrayRecommendation null: {SelectedClass?.StandardArrayRecommendation == null}");
            return hasRec;
        }
    }
    
    public string ClassRecommendationText
    {
        get
        {
            if (!HasClassRecommendation || SelectedClass?.StandardArrayRecommendation == null)
                return string.Empty;
                
            var rec = SelectedClass.StandardArrayRecommendation;
            return $"Recommended for {SelectedClass.Name}: " +
                   $"Str:{rec["strength"]}, Dex:{rec["dexterity"]}, Con:{rec["constitution"]}, " +
                   $"Int:{rec["intelligence"]}, Wis:{rec["wisdom"]}, Cha:{rec["charisma"]}";
        }
    }

    // Commands
    public ICommand ApplyStandardArrayCommand { get; private set; } = null!;
    public ICommand ResetStandardArrayCommand { get; private set; } = null!;
    public ICommand ClearAssignmentCommand { get; private set; } = null!;
    public ICommand ApplyClassRecommendationCommand { get; private set; } = null!;

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
        ApplyClassRecommendationCommand = new RelayCommand(ApplyClassRecommendation, CanApplyClassRecommendation);
        
        // Individual ability assignment commands
        AssignStrengthCommand = new RelayCommand(parameter => AssignValueToAbility("Strength", parameter));
        AssignDexterityCommand = new RelayCommand(parameter => AssignValueToAbility("Dexterity", parameter));
        AssignConstitutionCommand = new RelayCommand(parameter => AssignValueToAbility("Constitution", parameter));
        AssignIntelligenceCommand = new RelayCommand(parameter => AssignValueToAbility("Intelligence", parameter));
        AssignWisdomCommand = new RelayCommand(parameter => AssignValueToAbility("Wisdom", parameter));
        AssignCharismaCommand = new RelayCommand(parameter => AssignValueToAbility("Charisma", parameter));
    }

    public void ApplyStandardArray()
    {
        if (StandardArray.IsComplete)
        {
            StandardArray.ApplyToAbilityScores(Character.AbilityScores);
            IsApplied = true;
        }
    }

    public bool CanApplyStandardArray()
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

    private void ApplyClassRecommendation()
    {
        System.Diagnostics.Debug.WriteLine("ApplyClassRecommendation called");
        
        if (!HasClassRecommendation || SelectedClass?.StandardArrayRecommendation == null)
        {
            System.Diagnostics.Debug.WriteLine("No class recommendation available");
            return;
        }

        var rec = SelectedClass.StandardArrayRecommendation;
        
        // Clear current assignments
        ResetStandardArray();
        
        // Apply recommended assignments
        try
        {
            // The JSON uses lowercase keys, but StandardArray expects capitalized ability names
            StandardArray.AssignValue("Strength", rec["strength"]);
            StandardArray.AssignValue("Dexterity", rec["dexterity"]);
            StandardArray.AssignValue("Constitution", rec["constitution"]);
            StandardArray.AssignValue("Intelligence", rec["intelligence"]);
            StandardArray.AssignValue("Wisdom", rec["wisdom"]);
            StandardArray.AssignValue("Charisma", rec["charisma"]);
            
            System.Diagnostics.Debug.WriteLine($"Successfully applied class recommendation for {SelectedClass.Name}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to apply class recommendation: {ex.Message}");
            // If assignment fails, just reset and let user know
            ResetStandardArray();
        }
    }

    private bool CanApplyClassRecommendation()
    {
        return HasClassRecommendation;
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