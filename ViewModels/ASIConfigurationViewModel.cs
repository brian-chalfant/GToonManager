using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GToonManager.Models;

namespace GToonManager.ViewModels;

public class ASIConfigurationViewModel : INotifyPropertyChanged
{
    private readonly ASIFeatureViewModel _asiFeature;
    private readonly Character _character;
    private bool _isSingleImprovement = true;
    private string? _selectedFirstAbility;
    private string? _selectedSecondAbility;

    public ASIConfigurationViewModel(ASIFeatureViewModel asiFeature, Character character)
    {
        _asiFeature = asiFeature;
        _character = character;
        
        InitializeCommands();
        LoadAbilities();
        
        // If modifying existing ASI, load current values
        if (asiFeature.AppliedASI != null)
        {
            LoadExistingASI(asiFeature.AppliedASI);
        }
    }

    public string Title => $"⚔️ Configure ASI - Level {_asiFeature.Level} ({_asiFeature.ClassName})";

    public bool IsSingleImprovement
    {
        get => _isSingleImprovement;
        set
        {
            _isSingleImprovement = value;
            OnPropertyChanged(nameof(IsSingleImprovement));
            OnPropertyChanged(nameof(IsDoubleImprovement));
            OnPropertyChanged(nameof(CanApply));
            ResetSelections();
        }
    }

    public bool IsDoubleImprovement
    {
        get => !_isSingleImprovement;
        set
        {
            _isSingleImprovement = !value;
            OnPropertyChanged(nameof(IsSingleImprovement));
            OnPropertyChanged(nameof(IsDoubleImprovement));
            OnPropertyChanged(nameof(CanApply));
            ResetSelections();
        }
    }

    public ObservableCollection<string> AvailableAbilities { get; } = new();
    public ObservableCollection<string> AvailableSecondAbilities { get; } = new();

    public string? SelectedFirstAbility
    {
        get => _selectedFirstAbility;
        set
        {
            _selectedFirstAbility = value;
            OnPropertyChanged(nameof(SelectedFirstAbility));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(SingleImprovementPreview));
            OnPropertyChanged(nameof(HasSingleImprovementPreview));
            OnPropertyChanged(nameof(DoubleImprovementPreview));
            OnPropertyChanged(nameof(HasDoubleImprovementPreview));
            UpdateSecondAbilityOptions();
        }
    }

    public string? SelectedSecondAbility
    {
        get => _selectedSecondAbility;
        set
        {
            _selectedSecondAbility = value;
            OnPropertyChanged(nameof(SelectedSecondAbility));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(DoubleImprovementPreview));
            OnPropertyChanged(nameof(HasDoubleImprovementPreview));
        }
    }

    public bool CanApply
    {
        get
        {
            if (IsSingleImprovement)
            {
                return !string.IsNullOrEmpty(SelectedFirstAbility) && 
                       CanIncreaseAbility(SelectedFirstAbility, 2);
            }
            else
            {
                return !string.IsNullOrEmpty(SelectedFirstAbility) && 
                       !string.IsNullOrEmpty(SelectedSecondAbility) &&
                       SelectedFirstAbility != SelectedSecondAbility &&
                       CanIncreaseAbility(SelectedFirstAbility, 1) &&
                       CanIncreaseAbility(SelectedSecondAbility, 1);
            }
        }
    }

    public string SingleImprovementPreview
    {
        get
        {
            if (string.IsNullOrEmpty(SelectedFirstAbility)) return string.Empty;
            
            var currentValue = GetCurrentAbilityScore(SelectedFirstAbility);
            var newValue = currentValue + 2;
            
            if (newValue > 20)
            {
                return $"⚠️ {SelectedFirstAbility}: {currentValue} → 20 (capped at 20)";
            }
            
            return $"✅ {SelectedFirstAbility}: {currentValue} → {newValue}";
        }
    }

    public bool HasSingleImprovementPreview => IsSingleImprovement && !string.IsNullOrEmpty(SelectedFirstAbility);

    public string DoubleImprovementPreview
    {
        get
        {
            if (string.IsNullOrEmpty(SelectedFirstAbility) || string.IsNullOrEmpty(SelectedSecondAbility))
                return string.Empty;
            
            var firstCurrent = GetCurrentAbilityScore(SelectedFirstAbility);
            var firstNew = Math.Min(firstCurrent + 1, 20);
            
            var secondCurrent = GetCurrentAbilityScore(SelectedSecondAbility);
            var secondNew = Math.Min(secondCurrent + 1, 20);
            
            var warning = (firstNew == 20 && firstCurrent < 20) || (secondNew == 20 && secondCurrent < 20) ? "⚠️ " : "✅ ";
            
            return $"{warning}{SelectedFirstAbility}: {firstCurrent} → {firstNew}, {SelectedSecondAbility}: {secondCurrent} → {secondNew}";
        }
    }

    public bool HasDoubleImprovementPreview => IsDoubleImprovement && 
                                             !string.IsNullOrEmpty(SelectedFirstAbility) && 
                                             !string.IsNullOrEmpty(SelectedSecondAbility);

    public ICommand ApplyCommand { get; private set; } = null!;
    public ICommand CancelCommand { get; private set; } = null!;

    public event EventHandler? CloseRequested;
    public event EventHandler? ApplyRequested;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void InitializeCommands()
    {
        ApplyCommand = new RelayCommand(() => Apply(), () => CanApply);
        CancelCommand = new RelayCommand(() => Cancel());
    }

    private void LoadAbilities()
    {
        var abilities = new[] { "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" };
        
        foreach (var ability in abilities)
        {
            AvailableAbilities.Add(ability);
        }
    }

    private void LoadExistingASI(AppliedASI appliedASI)
    {
        if (appliedASI.ImprovementType == ASIImprovementType.Single)
        {
            IsSingleImprovement = true;
            SelectedFirstAbility = appliedASI.FirstAbility;
        }
        else
        {
            IsDoubleImprovement = true;
            SelectedFirstAbility = appliedASI.FirstAbility;
            SelectedSecondAbility = appliedASI.SecondAbility;
        }
    }

    private void ResetSelections()
    {
        SelectedFirstAbility = null;
        SelectedSecondAbility = null;
    }

    private void UpdateSecondAbilityOptions()
    {
        AvailableSecondAbilities.Clear();
        
        foreach (var ability in AvailableAbilities)
        {
            if (ability != SelectedFirstAbility)
            {
                AvailableSecondAbilities.Add(ability);
            }
        }
    }

    private int GetCurrentAbilityScore(string ability)
    {
        return ability switch
        {
            "Strength" => _character.StrengthTotal ?? 0,
            "Dexterity" => _character.DexterityTotal ?? 0,
            "Constitution" => _character.ConstitutionTotal ?? 0,
            "Intelligence" => _character.IntelligenceTotal ?? 0,
            "Wisdom" => _character.WisdomTotal ?? 0,
            "Charisma" => _character.CharismaTotal ?? 0,
            _ => 0
        };
    }

    private bool CanIncreaseAbility(string ability, int amount)
    {
        var currentScore = GetCurrentAbilityScore(ability);
        return currentScore + amount <= 20;
    }

    private void Apply()
    {
        try
        {
            // Remove existing ASI if modifying
            if (_asiFeature.AppliedASI != null)
            {
                RemoveExistingASIFromCharacter(_asiFeature.AppliedASI);
                _character.AppliedASIs?.Remove(_asiFeature.AppliedASI);
            }

            // Create new ASI
            var newASI = new AppliedASI
            {
                Id = _asiFeature.Id,
                Level = _asiFeature.Level,
                ClassName = _asiFeature.ClassName,
                ImprovementType = IsSingleImprovement ? ASIImprovementType.Single : ASIImprovementType.Double,
                FirstAbility = SelectedFirstAbility!,
                SecondAbility = IsDoubleImprovement ? SelectedSecondAbility : null
            };

            // Initialize AppliedASIs if null
            if (_character.AppliedASIs == null)
            {
                _character.AppliedASIs = new List<AppliedASI>();
            }

            _character.AppliedASIs.Add(newASI);

            // Apply the improvements to the character
            ApplyImprovementToCharacter(newASI);

            ApplyRequested?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error applying ASI: {ex.Message}", "Error", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private void ApplyImprovementToCharacter(AppliedASI asi)
    {
        // Apply first ability improvement
        ApplyAbilityIncrease(asi.FirstAbility, asi.ImprovementType == ASIImprovementType.Single ? 2 : 1);
        
        // Apply second ability improvement if double improvement
        if (asi.ImprovementType == ASIImprovementType.Double && !string.IsNullOrEmpty(asi.SecondAbility))
        {
            ApplyAbilityIncrease(asi.SecondAbility, 1);
        }
    }

    private void RemoveExistingASIFromCharacter(AppliedASI existingASI)
    {
        // Remove first ability improvement
        ApplyAbilityIncrease(existingASI.FirstAbility, existingASI.ImprovementType == ASIImprovementType.Single ? -2 : -1);
        
        // Remove second ability improvement if double improvement
        if (existingASI.ImprovementType == ASIImprovementType.Double && !string.IsNullOrEmpty(existingASI.SecondAbility))
        {
            ApplyAbilityIncrease(existingASI.SecondAbility, -1);
        }
    }

    private void ApplyAbilityIncrease(string ability, int amount)
    {
        switch (ability)
        {
            case "Strength":
                _character.StrengthASI = (_character.StrengthASI ?? 0) + amount;
                break;
            case "Dexterity":
                _character.DexterityASI = (_character.DexterityASI ?? 0) + amount;
                break;
            case "Constitution":
                _character.ConstitutionASI = (_character.ConstitutionASI ?? 0) + amount;
                break;
            case "Intelligence":
                _character.IntelligenceASI = (_character.IntelligenceASI ?? 0) + amount;
                break;
            case "Wisdom":
                _character.WisdomASI = (_character.WisdomASI ?? 0) + amount;
                break;
            case "Charisma":
                _character.CharismaASI = (_character.CharismaASI ?? 0) + amount;
                break;
        }
    }

    private void Cancel()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 