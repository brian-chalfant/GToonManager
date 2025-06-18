using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GToonManager.Models;

namespace GToonManager.ViewModels;

public class AbilityScoreImprovementViewModel : INotifyPropertyChanged
{
    private readonly Character _character;
    private readonly Action<Character> _onCharacterUpdated;

    public AbilityScoreImprovementViewModel(Character character, Action<Character> onCharacterUpdated)
    {
        _character = character;
        _onCharacterUpdated = onCharacterUpdated;
        
        InitializeCommands();
        LoadAvailableASIs();
    }

    public string CharacterName => _character.Name;
    public int CharacterLevel => _character.Level;
    
    public int CurrentStrength => _character.StrengthTotal ?? 0;
    public int CurrentDexterity => _character.DexterityTotal ?? 0;
    public int CurrentConstitution => _character.ConstitutionTotal ?? 0;
    public int CurrentIntelligence => _character.IntelligenceTotal ?? 0;
    public int CurrentWisdom => _character.WisdomTotal ?? 0;
    public int CurrentCharisma => _character.CharismaTotal ?? 0;

    public ObservableCollection<ASIFeatureViewModel> AvailableASIs { get; } = new();
    
    public bool HasAvailableASIs => AvailableASIs.Any();

    public ICommand ConfigureASICommand { get; private set; } = null!;
    public ICommand CloseCommand { get; private set; } = null!;

    public event EventHandler? CloseRequested;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void InitializeCommands()
    {
        ConfigureASICommand = new RelayCommand<ASIFeatureViewModel>(asiFeature => ConfigureASI(asiFeature), asiFeature => CanConfigureASI(asiFeature));
        CloseCommand = new RelayCommand(() => Close());
    }

    private void LoadAvailableASIs()
    {
        AvailableASIs.Clear();
        
        // Initialize ASI tracking if not already done
        if (_character.AppliedASIs == null)
        {
            _character.AppliedASIs = new List<AppliedASI>();
        }

        foreach (var classLevel in _character.ClassLevels)
        {
            if (classLevel.CharacterClass != null)
            {
                foreach (var kvp in classLevel.CharacterClass.Features)
                {
                    int featureLevel = kvp.Key;
                    if (featureLevel <= classLevel.Level)
                    {
                        foreach (var feature in kvp.Value)
                        {
                            if (feature.Name == "Ability Score Improvement")
                            {
                                var asiId = $"{classLevel.ClassName}-{featureLevel}";
                                var existingASI = _character.AppliedASIs.FirstOrDefault(a => a.Id == asiId);
                                
                                AvailableASIs.Add(new ASIFeatureViewModel
                                {
                                    Id = asiId,
                                    Level = featureLevel,
                                    ClassName = classLevel.ClassName,
                                    Feature = feature,
                                    IsApplied = existingASI != null,
                                    AppliedASI = existingASI,
                                    DisplayText = $"Level {featureLevel} - {classLevel.ClassName}",
                                    StatusText = existingASI != null ? 
                                        $"Applied: {existingASI.GetDescription()}" : 
                                        "Not yet applied",
                                    ButtonText = existingASI != null ? "Modify" : "Configure",
                                    CanConfigure = true
                                });
                            }
                        }
                    }
                }
            }
        }
        
        OnPropertyChanged(nameof(HasAvailableASIs));
    }

    private void ConfigureASI(ASIFeatureViewModel? asiFeature)
    {
        if (asiFeature == null) return;

        var configViewModel = new ASIConfigurationViewModel(asiFeature, _character);
        var configWindow = new Views.ASIConfigurationWindow(configViewModel);
        
        if (configWindow.ShowDialog() == true)
        {
            // ASI was applied, refresh the list
            LoadAvailableASIs();
            RefreshAbilityScores();
            _onCharacterUpdated?.Invoke(_character);
        }
    }

    private bool CanConfigureASI(ASIFeatureViewModel? asiFeature)
    {
        return asiFeature?.CanConfigure == true;
    }

    private void RefreshAbilityScores()
    {
        // Force refresh of the character's ability score calculations
        _character.RefreshAbilityScoreModifiers();
        
        // Update the displayed values in this view model
        OnPropertyChanged(nameof(CurrentStrength));
        OnPropertyChanged(nameof(CurrentDexterity));
        OnPropertyChanged(nameof(CurrentConstitution));
        OnPropertyChanged(nameof(CurrentIntelligence));
        OnPropertyChanged(nameof(CurrentWisdom));
        OnPropertyChanged(nameof(CurrentCharisma));
    }

    private void Close()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ASIFeatureViewModel
{
    public string Id { get; set; } = string.Empty;
    public int Level { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public ClassFeature Feature { get; set; } = null!;
    public bool IsApplied { get; set; }
    public AppliedASI? AppliedASI { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public bool CanConfigure { get; set; }
} 