using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using GToonManager.Models;

namespace GToonManager.ViewModels;

public class AbilityScoreAssignmentViewModel : INotifyPropertyChanged
{
    private Character _character;
    private ObservableCollection<int> _generatedScores = new();
    private Dictionary<string, int?> _assignments = new();
    private CharacterClass? _selectedClass;

    public AbilityScoreAssignmentViewModel(Character character)
    {
        _character = character;
        InitializeAssignments();
        InitializeCommands();
    }

    public ObservableCollection<int> GeneratedScores
    {
        get => _generatedScores;
        set
        {
            _generatedScores = value;
            OnPropertyChanged();
        }
    }

    public CharacterClass? SelectedClass
    {
        get => _selectedClass;
        set
        {
            _selectedClass = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasClassRecommendation));
            OnPropertyChanged(nameof(ClassRecommendationText));
        }
    }

    public bool HasClassRecommendation => SelectedClass?.StandardArrayRecommendation != null;

    public string ClassRecommendationText => SelectedClass != null 
        ? $"Recommended for {SelectedClass.Name}" 
        : "";

    // Assignment properties for each ability
    public int? StrengthAssignment
    {
        get => _assignments.GetValueOrDefault("Strength");
        set
        {
            SetAssignment("Strength", value);
            OnPropertyChanged();
        }
    }

    public int? DexterityAssignment
    {
        get => _assignments.GetValueOrDefault("Dexterity");
        set
        {
            SetAssignment("Dexterity", value);
            OnPropertyChanged();
        }
    }

    public int? ConstitutionAssignment
    {
        get => _assignments.GetValueOrDefault("Constitution");
        set
        {
            SetAssignment("Constitution", value);
            OnPropertyChanged();
        }
    }

    public int? IntelligenceAssignment
    {
        get => _assignments.GetValueOrDefault("Intelligence");
        set
        {
            SetAssignment("Intelligence", value);
            OnPropertyChanged();
        }
    }

    public int? WisdomAssignment
    {
        get => _assignments.GetValueOrDefault("Wisdom");
        set
        {
            SetAssignment("Wisdom", value);
            OnPropertyChanged();
        }
    }

    public int? CharismaAssignment
    {
        get => _assignments.GetValueOrDefault("Charisma");
        set
        {
            SetAssignment("Charisma", value);
            OnPropertyChanged();
        }
    }

    public List<int> AvailableScores => GeneratedScores.Where(score => !_assignments.Values.Contains(score)).ToList();

    public bool CanApplyAssignments => _assignments.Values.All(v => v.HasValue) && _assignments.Values.Count == 6;

    // Commands
    public ICommand ApplyClassRecommendationCommand { get; private set; } = null!;
    public ICommand ClearAssignmentsCommand { get; private set; } = null!;
    public ICommand ApplyAssignmentsCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        ApplyClassRecommendationCommand = new RelayCommand(ApplyClassRecommendation, CanApplyClassRecommendation);
        ClearAssignmentsCommand = new RelayCommand(ClearAssignments);
        ApplyAssignmentsCommand = new RelayCommand(ApplyAssignments, () => CanApplyAssignments);
    }

    private void InitializeAssignments()
    {
        _assignments["Strength"] = null;
        _assignments["Dexterity"] = null;
        _assignments["Constitution"] = null;
        _assignments["Intelligence"] = null;
        _assignments["Wisdom"] = null;
        _assignments["Charisma"] = null;
    }

    private void SetAssignment(string ability, int? value)
    {
        // Clear any existing assignment of this value
        if (value.HasValue)
        {
            var existingAssignment = _assignments.FirstOrDefault(kvp => kvp.Value == value);
            if (existingAssignment.Key != null && existingAssignment.Key != ability)
            {
                _assignments[existingAssignment.Key] = null;
                OnPropertyChanged($"{existingAssignment.Key}Assignment");
            }
        }

        _assignments[ability] = value;
        OnPropertyChanged(nameof(AvailableScores));
        OnPropertyChanged(nameof(CanApplyAssignments));
    }

    private bool CanApplyClassRecommendation()
    {
        return HasClassRecommendation && GeneratedScores.Count == 6;
    }

    private void ApplyClassRecommendation()
    {
        if (SelectedClass?.StandardArrayRecommendation == null) return;

        var recommendation = SelectedClass.StandardArrayRecommendation;
        var sortedScores = GeneratedScores.OrderByDescending(x => x).ToList();
        
        System.Diagnostics.Debug.WriteLine($"ApplyClassRecommendation for {SelectedClass.Name}");
        System.Diagnostics.Debug.WriteLine($"Generated scores: [{string.Join(", ", sortedScores)}]");
        
        // Ensure we have exactly 6 scores
        if (sortedScores.Count != 6) 
        {
            System.Diagnostics.Debug.WriteLine($"Invalid score count: {sortedScores.Count}, expected 6");
            return;
        }

        // Create a complete list of abilities with their recommended values
        var allAbilities = new Dictionary<string, int>
        {
            ["strength"] = recommendation.GetValueOrDefault("strength", 10),
            ["dexterity"] = recommendation.GetValueOrDefault("dexterity", 10),
            ["constitution"] = recommendation.GetValueOrDefault("constitution", 10),
            ["intelligence"] = recommendation.GetValueOrDefault("intelligence", 10),
            ["wisdom"] = recommendation.GetValueOrDefault("wisdom", 10),
            ["charisma"] = recommendation.GetValueOrDefault("charisma", 10)
        };

        // Sort abilities by recommended value (highest first)
        var sortedRecommendation = allAbilities.OrderByDescending(kvp => kvp.Value).ToList();

        // Clear existing assignments
        ClearAssignments();

        // Assign highest scores to highest recommended values
        for (int i = 0; i < 6; i++)
        {
            var ability = sortedRecommendation[i].Key;
            var score = sortedScores[i];
            var recommendedValue = sortedRecommendation[i].Value;
            
            System.Diagnostics.Debug.WriteLine($"Assigning {score} to {ability} (recommended: {recommendedValue})");
            
            switch (ability.ToLower())
            {
                case "strength":
                    StrengthAssignment = score;
                    break;
                case "dexterity":
                    DexterityAssignment = score;
                    break;
                case "constitution":
                    ConstitutionAssignment = score;
                    break;
                case "intelligence":
                    IntelligenceAssignment = score;
                    break;
                case "wisdom":
                    WisdomAssignment = score;
                    break;
                case "charisma":
                    CharismaAssignment = score;
                    break;
            }
        }
        
        System.Diagnostics.Debug.WriteLine("Class recommendation applied successfully");
    }

    private void ClearAssignments()
    {
        StrengthAssignment = null;
        DexterityAssignment = null;
        ConstitutionAssignment = null;
        IntelligenceAssignment = null;
        WisdomAssignment = null;
        CharismaAssignment = null;
    }

    private void ApplyAssignments()
    {
        if (!CanApplyAssignments) return;

        _character.AbilityScores.Strength = StrengthAssignment ?? 10;
        _character.AbilityScores.Dexterity = DexterityAssignment ?? 10;
        _character.AbilityScores.Constitution = ConstitutionAssignment ?? 10;
        _character.AbilityScores.Intelligence = IntelligenceAssignment ?? 10;
        _character.AbilityScores.Wisdom = WisdomAssignment ?? 10;
        _character.AbilityScores.Charisma = CharismaAssignment ?? 10;
    }

    public void SetGeneratedScores(IEnumerable<int> scores)
    {
        GeneratedScores.Clear();
        foreach (var score in scores)
        {
            GeneratedScores.Add(score);
        }
        
        ClearAssignments();
        OnPropertyChanged(nameof(AvailableScores));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 