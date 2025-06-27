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
    private int _rerollCount = 0;
    private int _rerollLimit = 1;
    private bool _isCompleted = false;


    public RollingViewModel(Character character, AbilityScoreGenerationMethod method)
    {
        _character = character;
        _random = new Random();
        InitializeCommands();
        
        MethodDescription = "Roll 4d6, drop lowest die to generate 6 ability scores";
    }

    public RollingViewModel(Character character, AbilityScoreGenerationMethod method, int rerollLimit)
    {
        _character = character;
        _random = new Random();
        _rerollLimit = rerollLimit;
        InitializeCommands();
        
        MethodDescription = "Roll 4d6, drop lowest die to generate 6 ability scores";
        
        // Debug output
        System.Diagnostics.Debug.WriteLine($"RollingViewModel created with rerollLimit: {rerollLimit}");
        System.Diagnostics.Debug.WriteLine($"Initial state: HasRolled={HasRolled}, RerollCount={RerollCount}, RerollLimit={RerollLimit}, CanReroll={CanReroll}");
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
            System.Diagnostics.Debug.WriteLine($"HasRolled changed to: {value}");
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanRoll));
            OnPropertyChanged(nameof(ShowRerollButton));
            RefreshCommands();
        }
    }

    public bool HasRolledScores => RolledScores?.Count == 6;
    public bool CanRoll => !HasRolled;

    public int RerollCount
    {
        get => _rerollCount;
        private set
        {
            _rerollCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanReroll));
            OnPropertyChanged(nameof(RerollsRemaining));
            OnPropertyChanged(nameof(RerollButtonText));
            OnPropertyChanged(nameof(ShowRerollButton));
            RefreshCommands();
        }
    }

    public int RerollsRemaining => Math.Max(0, _rerollLimit - _rerollCount);
    public bool CanReroll
    {
        get
        {
            var canReroll = HasRolled && _rerollCount < _rerollLimit;
            System.Diagnostics.Debug.WriteLine($"CanReroll calculated: {canReroll} (HasRolled={HasRolled}, RerollCount={_rerollCount}, RerollLimit={_rerollLimit})");
            return canReroll;
        }
    }

    public bool ShowRerollButton
    {
        get
        {
            var showButton = HasRolled && _rerollLimit > 0;
            System.Diagnostics.Debug.WriteLine($"ShowRerollButton calculated: {showButton} (HasRolled={HasRolled}, RerollLimit={_rerollLimit})");
            return showButton;
        }
    }

    public string RerollButtonText => $"Reroll ({RerollsRemaining} remaining)";

    // Add property to expose reroll limit for debugging
    public int RerollLimit => _rerollLimit;

    public bool IsCompleted
    {
        get => _isCompleted;
        private set
        {
            _isCompleted = value;
            OnPropertyChanged();
        }
    }

    public AbilityScoreAssignmentViewModel? AssignmentViewModel
    {
        get => _assignmentViewModel;
        private set
        {
            // Unsubscribe from old assignment view model
            if (_assignmentViewModel != null)
            {
                _assignmentViewModel.ScoresApplied -= OnScoresApplied;
            }
            
            _assignmentViewModel = value;
            
            // Subscribe to new assignment view model
            if (_assignmentViewModel != null)
            {
                _assignmentViewModel.ScoresApplied += OnScoresApplied;
            }
            
            OnPropertyChanged();
        }
    }

    public int TotalPoints => RolledScores?.Sum() ?? 0;

    // Event for when scores are applied to character
    public event EventHandler? ScoresApplied;

    // Commands
    public ICommand GenerateScoresCommand { get; private set; } = null!;
    public ICommand RerollCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        GenerateScoresCommand = new RelayCommand(GenerateScores, () => CanRoll);
        RerollCommand = new RelayCommand(Reroll, () => CanReroll);
    }

    // Method to refresh command states
    private void RefreshCommands()
    {
        System.Diagnostics.Debug.WriteLine("RefreshCommands called");
        System.Diagnostics.Debug.WriteLine($"Current state: HasRolled={HasRolled}, RerollCount={RerollCount}, RerollLimit={RerollLimit}, CanReroll={CanReroll}");
        
        // Force immediate command manager refresh
        System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }), System.Windows.Threading.DispatcherPriority.Background);
        
        (GenerateScoresCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (RerollCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private void GenerateScores()
    {
        if (!CanRoll) return;

        System.Diagnostics.Debug.WriteLine("GenerateScores called");
        var newScores = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            var (value, _) = Roll4d6DropLowest();
            newScores.Add(value);
        }

        RolledScores = newScores;
        HasRolled = true;

        System.Diagnostics.Debug.WriteLine($"After GenerateScores: HasRolled={HasRolled}, RerollCount={RerollCount}, RerollLimit={RerollLimit}, CanReroll={CanReroll}");

        // Create assignment ViewModel
        AssignmentViewModel = new AbilityScoreAssignmentViewModel(_character);
        AssignmentViewModel.SetGeneratedScores(RolledScores);
        
        // Note: Classes need to be populated by the calling view model (MainViewModel)
    }

    private void Reroll()
    {
        System.Diagnostics.Debug.WriteLine($"Reroll called - CanReroll={CanReroll}");
        if (!CanReroll) return;

        RerollCount++;
        System.Diagnostics.Debug.WriteLine($"RerollCount incremented to: {RerollCount}");
        
        // Clear current scores and assignment
        RolledScores = new List<int>();
        AssignmentViewModel = null;
        
        // Automatically generate new scores
        var newScores = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            var (value, _) = Roll4d6DropLowest();
            newScores.Add(value);
        }

        RolledScores = newScores;
        HasRolled = true;

        System.Diagnostics.Debug.WriteLine($"After Reroll: HasRolled={HasRolled}, RerollCount={RerollCount}, RerollLimit={RerollLimit}, CanReroll={CanReroll}");

        // Create assignment ViewModel
        AssignmentViewModel = new AbilityScoreAssignmentViewModel(_character);
        AssignmentViewModel.SetGeneratedScores(RolledScores);
    }

    private void OnScoresApplied(object? sender, EventArgs e)
    {
        // Mark the rolling process as completed
        IsCompleted = true;
        
        // Bubble up the event to notify the main view model
        ScoresApplied?.Invoke(this, EventArgs.Empty);
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