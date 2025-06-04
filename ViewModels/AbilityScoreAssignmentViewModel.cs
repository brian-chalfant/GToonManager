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
        set => SetAssignment("Strength", value);
    }

    public int? DexterityAssignment
    {
        get => _assignments.GetValueOrDefault("Dexterity");
        set => SetAssignment("Dexterity", value);
    }

    public int? ConstitutionAssignment
    {
        get => _assignments.GetValueOrDefault("Constitution");
        set => SetAssignment("Constitution", value);
    }

    public int? IntelligenceAssignment
    {
        get => _assignments.GetValueOrDefault("Intelligence");
        set => SetAssignment("Intelligence", value);
    }

    public int? WisdomAssignment
    {
        get => _assignments.GetValueOrDefault("Wisdom");
        set => SetAssignment("Wisdom", value);
    }

    public int? CharismaAssignment
    {
        get => _assignments.GetValueOrDefault("Charisma");
        set => SetAssignment("Charisma", value);
    }

    public List<int> AvailableScores
    {
        get
        {
            // Create a list that accounts for duplicates
            var available = new List<int>(GeneratedScores);
            
            // Remove assigned scores one by one (handles duplicates correctly)
            foreach (var assignment in _assignments.Values)
            {
                if (assignment.HasValue)
                {
                    available.Remove(assignment.Value);
                }
            }
            
            return available.OrderByDescending(x => x).ToList();
        }
    }

    // Properties to provide available scores for each ability (including currently assigned value)
    public List<int?> StrengthAvailableScores
    {
        get
        {
            var scores = new List<int?> { null }; // Add null option to clear assignment
            scores.AddRange(AvailableScores.Cast<int?>());
            if (StrengthAssignment.HasValue && !scores.Contains(StrengthAssignment.Value))
            {
                scores.Add(StrengthAssignment.Value);
                scores = scores.OrderByDescending(x => x).ToList();
            }
            return scores;
        }
    }

    public List<int?> DexterityAvailableScores
    {
        get
        {
            var scores = new List<int?> { null };
            scores.AddRange(AvailableScores.Cast<int?>());
            if (DexterityAssignment.HasValue && !scores.Contains(DexterityAssignment.Value))
            {
                scores.Add(DexterityAssignment.Value);
                scores = scores.OrderByDescending(x => x).ToList();
            }
            return scores;
        }
    }

    public List<int?> ConstitutionAvailableScores
    {
        get
        {
            var scores = new List<int?> { null };
            scores.AddRange(AvailableScores.Cast<int?>());
            if (ConstitutionAssignment.HasValue && !scores.Contains(ConstitutionAssignment.Value))
            {
                scores.Add(ConstitutionAssignment.Value);
                scores = scores.OrderByDescending(x => x).ToList();
            }
            return scores;
        }
    }

    public List<int?> IntelligenceAvailableScores
    {
        get
        {
            var scores = new List<int?> { null };
            scores.AddRange(AvailableScores.Cast<int?>());
            if (IntelligenceAssignment.HasValue && !scores.Contains(IntelligenceAssignment.Value))
            {
                scores.Add(IntelligenceAssignment.Value);
                scores = scores.OrderByDescending(x => x).ToList();
            }
            return scores;
        }
    }

    public List<int?> WisdomAvailableScores
    {
        get
        {
            var scores = new List<int?> { null };
            scores.AddRange(AvailableScores.Cast<int?>());
            if (WisdomAssignment.HasValue && !scores.Contains(WisdomAssignment.Value))
            {
                scores.Add(WisdomAssignment.Value);
                scores = scores.OrderByDescending(x => x).ToList();
            }
            return scores;
        }
    }

    public List<int?> CharismaAvailableScores
    {
        get
        {
            var scores = new List<int?> { null };
            scores.AddRange(AvailableScores.Cast<int?>());
            if (CharismaAssignment.HasValue && !scores.Contains(CharismaAssignment.Value))
            {
                scores.Add(CharismaAssignment.Value);
                scores = scores.OrderByDescending(x => x).ToList();
            }
            return scores;
        }
    }

    public bool CanApplyAssignments => _assignments.Values.All(v => v.HasValue) && _assignments.Values.Count == 6;

    // Event for when scores are applied to character
    public event EventHandler? ScoresApplied;

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
        // Check if we can assign this value (do we have it available?)
        if (value.HasValue)
        {
            var available = new List<int>(GeneratedScores);
            
            // Remove all currently assigned scores except for the one we're potentially replacing
            foreach (var kvp in _assignments)
            {
                if (kvp.Key != ability && kvp.Value.HasValue)
                {
                    available.Remove(kvp.Value.Value);
                }
            }
            
            // Check if the value we want to assign is still available
            if (!available.Contains(value.Value))
            {
                // Value is not available, don't assign it
                return;
            }
        }
        
        // Only update if the value actually changed
        if (_assignments[ability] != value)
        {
            _assignments[ability] = value;
            
            // Notify the specific assignment property that changed
            OnPropertyChanged($"{ability}Assignment");
            
            // Notify dependent properties
            OnPropertyChanged(nameof(AvailableScores));
            OnPropertyChanged(nameof(CanApplyAssignments));
            
            // Notify all available scores properties have changed
            OnPropertyChanged(nameof(StrengthAvailableScores));
            OnPropertyChanged(nameof(DexterityAvailableScores));
            OnPropertyChanged(nameof(ConstitutionAvailableScores));
            OnPropertyChanged(nameof(IntelligenceAvailableScores));
            OnPropertyChanged(nameof(WisdomAvailableScores));
            OnPropertyChanged(nameof(CharismaAvailableScores));
        }
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
        
        // Notify all available scores properties have changed
        OnPropertyChanged(nameof(StrengthAvailableScores));
        OnPropertyChanged(nameof(DexterityAvailableScores));
        OnPropertyChanged(nameof(ConstitutionAvailableScores));
        OnPropertyChanged(nameof(IntelligenceAvailableScores));
        OnPropertyChanged(nameof(WisdomAvailableScores));
        OnPropertyChanged(nameof(CharismaAvailableScores));
    }

    public void ApplyAssignments()
    {
        if (!CanApplyAssignments) return;

        _character.AbilityScores.Strength = StrengthAssignment ?? 10;
        _character.AbilityScores.Dexterity = DexterityAssignment ?? 10;
        _character.AbilityScores.Constitution = ConstitutionAssignment ?? 10;
        _character.AbilityScores.Intelligence = IntelligenceAssignment ?? 10;
        _character.AbilityScores.Wisdom = WisdomAssignment ?? 10;
        _character.AbilityScores.Charisma = CharismaAssignment ?? 10;

        // Raise the event to notify that scores have been applied
        ScoresApplied?.Invoke(this, EventArgs.Empty);
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
        
        // Notify all available scores properties have changed
        OnPropertyChanged(nameof(StrengthAvailableScores));
        OnPropertyChanged(nameof(DexterityAvailableScores));
        OnPropertyChanged(nameof(ConstitutionAvailableScores));
        OnPropertyChanged(nameof(IntelligenceAvailableScores));
        OnPropertyChanged(nameof(WisdomAvailableScores));
        OnPropertyChanged(nameof(CharismaAvailableScores));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 