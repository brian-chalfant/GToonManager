using System.ComponentModel;
using System.Windows.Input;
using GToonManager.Models;

namespace GToonManager.ViewModels;

public class FreeEntryViewModel : INotifyPropertyChanged
{
    private Character _character;

    public FreeEntryViewModel(Character character)
    {
        _character = character;
        InitializeCommands();
    }

    public string Description => "Enter ability scores directly (1-30 recommended)";

    public int StrengthValue
    {
        get => _character.AbilityScores.Strength ?? 10;
        set
        {
            if (ValidateAbilityScore(value))
            {
                _character.AbilityScores.Strength = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPoints));
            }
        }
    }

    public int DexterityValue
    {
        get => _character.AbilityScores.Dexterity ?? 10;
        set
        {
            if (ValidateAbilityScore(value))
            {
                _character.AbilityScores.Dexterity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPoints));
            }
        }
    }

    public int ConstitutionValue
    {
        get => _character.AbilityScores.Constitution ?? 10;
        set
        {
            if (ValidateAbilityScore(value))
            {
                _character.AbilityScores.Constitution = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPoints));
            }
        }
    }

    public int IntelligenceValue
    {
        get => _character.AbilityScores.Intelligence ?? 10;
        set
        {
            if (ValidateAbilityScore(value))
            {
                _character.AbilityScores.Intelligence = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPoints));
            }
        }
    }

    public int WisdomValue
    {
        get => _character.AbilityScores.Wisdom ?? 10;
        set
        {
            if (ValidateAbilityScore(value))
            {
                _character.AbilityScores.Wisdom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPoints));
            }
        }
    }

    public int CharismaValue
    {
        get => _character.AbilityScores.Charisma ?? 10;
        set
        {
            if (ValidateAbilityScore(value))
            {
                _character.AbilityScores.Charisma = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPoints));
            }
        }
    }

    public int TotalPoints => StrengthValue + DexterityValue + ConstitutionValue + 
                             IntelligenceValue + WisdomValue + CharismaValue;

    // Commands
    public ICommand SetDefaultsCommand { get; private set; } = null!;
    public ICommand SetEliteArrayCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        SetDefaultsCommand = new RelayCommand(SetDefaults);
        SetEliteArrayCommand = new RelayCommand(SetEliteArray);
    }

    private bool ValidateAbilityScore(int value)
    {
        // Allow reasonable ranges for ability scores
        return value >= 1 && value <= 30;
    }

    private void SetDefaults()
    {
        StrengthValue = 10;
        DexterityValue = 10;
        ConstitutionValue = 10;
        IntelligenceValue = 10;
        WisdomValue = 10;
        CharismaValue = 10;
    }

    private void SetEliteArray()
    {
        // Elite array: 15, 14, 13, 12, 10, 8
        StrengthValue = 15;
        DexterityValue = 14;
        ConstitutionValue = 13;
        IntelligenceValue = 12;
        WisdomValue = 10;
        CharismaValue = 8;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 