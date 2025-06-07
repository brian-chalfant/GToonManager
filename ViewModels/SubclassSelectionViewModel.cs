using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using GToonManager.Models;

namespace GToonManager.ViewModels;

public class SubclassSelectionViewModel : INotifyPropertyChanged
{
    private Subclass? _selectedSubclass;
    private readonly CharacterClassLevel _characterClassLevel;
    private readonly Action<Subclass> _onSubclassChosen;
    private readonly Action _onCancel;

    public SubclassSelectionViewModel(CharacterClassLevel characterClassLevel, Action<Subclass> onSubclassChosen, Action onCancel)
    {
        _characterClassLevel = characterClassLevel;
        _onSubclassChosen = onSubclassChosen;
        _onCancel = onCancel;

        AvailableSubclasses = new ObservableCollection<Subclass>(characterClassLevel.CharacterClass?.Subclasses ?? new List<Subclass>());
        
        // Set up commands
        ChooseSubclassCommand = new RelayCommand(ChooseSubclass, CanChoose);
        CancelCommand = new RelayCommand(Cancel);

        // Auto-select first subclass if available
        if (AvailableSubclasses.Any())
        {
            SelectedSubclass = AvailableSubclasses.First();
        }
    }

    public ObservableCollection<Subclass> AvailableSubclasses { get; }

    public string SubclassTypeName => _characterClassLevel.CharacterClass?.SubclassType.ToTitleCase() ?? "Subclass";

    public Subclass? SelectedSubclass
    {
        get => _selectedSubclass;
        set
        {
            _selectedSubclass = value;
            OnPropertyChanged(nameof(SelectedSubclass));
            OnPropertyChanged(nameof(CanChooseSubclass));
            OnPropertyChanged(nameof(SubclassFeaturesByLevel));
            (ChooseSubclassCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public bool CanChooseSubclass => SelectedSubclass != null;

    public ObservableCollection<SubclassFeatureLevelGroup> SubclassFeaturesByLevel
    {
        get
        {
            var groups = new ObservableCollection<SubclassFeatureLevelGroup>();
            
            if (SelectedSubclass != null)
            {
                foreach (var level in SelectedSubclass.FeatureLevels)
                {
                    var features = SelectedSubclass.GetFeaturesAtLevel(level);
                    if (features.Any())
                    {
                        groups.Add(new SubclassFeatureLevelGroup
                        {
                            Level = level,
                            Features = new ObservableCollection<SubclassFeature>(features)
                        });
                    }
                }
            }

            return groups;
        }
    }

    public ICommand ChooseSubclassCommand { get; }
    public ICommand CancelCommand { get; }

    private void ChooseSubclass(object? parameter)
    {
        if (SelectedSubclass != null)
        {
            // Show confirmation dialog
            var result = System.Windows.MessageBox.Show(
                $"Are you sure you want to choose {SelectedSubclass.Name} as your {SubclassTypeName}?\n\n" +
                "This choice will be permanent for this character.",
                "Confirm Subclass Choice",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _onSubclassChosen(SelectedSubclass);
            }
        }
    }

    private bool CanChoose(object? parameter)
    {
        return CanChooseSubclass;
    }

    private void Cancel(object? parameter)
    {
        _onCancel();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class SubclassFeatureLevelGroup
{
    public int Level { get; set; }
    public ObservableCollection<SubclassFeature> Features { get; set; } = new();
}

// Extension method to convert subclass type to title case
public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        
        return char.ToUpper(input[0]) + input[1..].ToLower();
    }
} 