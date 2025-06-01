using System.ComponentModel;
using System.Windows.Input;
using GToonManager.Models;

namespace GToonManager.ViewModels;

public class RollingViewModel : INotifyPropertyChanged
{
    private Character _character;
    private Random _random;
    private List<int> _rolledScores = new();
    private bool _hasRolled = false;
    private AbilityScoreAssignmentViewModel? _assignmentViewModel;

    public RollingViewModel(Character character, AbilityScoreGenerationMethod method)
    {
        _character = character;
        _random = new Random();
        InitializeCommands();
        
        MethodDescription = "Roll 4d6, drop lowest die to generate 6 ability scores";
    }

    public string MethodDescription { get; }

    public List<int> RolledScores
    {
        get => _rolledScores;
        private set
        {
            _rolledScores = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasRolledScores));
            OnPropertyChanged(nameof(TotalPoints));
        }
    }

    public bool HasRolled
    {
        get => _hasRolled;
        private set
        {
            _hasRolled = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanRoll));
        }
    }

    public bool HasRolledScores => RolledScores.Count == 6;
    public bool CanRoll => !HasRolled;

    public AbilityScoreAssignmentViewModel? AssignmentViewModel
    {
        get => _assignmentViewModel;
        private set
        {
            _assignmentViewModel = value;
            OnPropertyChanged();
        }
    }

    public int TotalPoints => RolledScores.Sum();

    // Commands
    public ICommand GenerateScoresCommand { get; private set; } = null!;
    public ICommand RerollCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        GenerateScoresCommand = new RelayCommand(GenerateScores, () => CanRoll);
        RerollCommand = new RelayCommand(Reroll);
    }

    private void GenerateScores()
    {
        if (!CanRoll) return;

        var newScores = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            var (value, _) = Roll4d6DropLowest();
            newScores.Add(value);
        }

        RolledScores = newScores;
        HasRolled = true;

        // Create assignment ViewModel
        AssignmentViewModel = new AbilityScoreAssignmentViewModel(_character);
        AssignmentViewModel.SetGeneratedScores(RolledScores);
    }

    private void Reroll()
    {
        HasRolled = false;
        RolledScores = new List<int>();
        AssignmentViewModel = null;
    }

    private (int value, string description) Roll4d6DropLowest()
    {
        var rolls = new int[4];
        for (int i = 0; i < 4; i++)
        {
            rolls[i] = _random.Next(1, 7);
        }

        Array.Sort(rolls);
        var total = rolls[1] + rolls[2] + rolls[3]; // Drop the lowest (rolls[0])
        
        var description = $"[{string.Join(", ", rolls)}] → {total} (dropped {rolls[0]})";
        return (total, description);
    }



    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 