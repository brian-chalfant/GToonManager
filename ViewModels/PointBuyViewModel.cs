using System.ComponentModel;
using System.Windows.Input;
using GToonManager.Models;

namespace GToonManager.ViewModels;

public class PointBuyViewModel : INotifyPropertyChanged
{
    private Character _character;
    private int _availablePoints;
    private int _strengthCost = 8;
    private int _dexterityCost = 8;
    private int _constitutionCost = 8;
    private int _intelligenceCost = 8;
    private int _wisdomCost = 8;
    private int _charismaCost = 8;

    public PointBuyViewModel(Character character, int totalPoints = 27)
    {
        _character = character;
        TotalPoints = totalPoints;
        
        // Initialize costs from current character ability scores (clamped to valid range)
        _strengthCost = Math.Max(8, Math.Min(15, character.AbilityScores.Strength));
        _dexterityCost = Math.Max(8, Math.Min(15, character.AbilityScores.Dexterity));
        _constitutionCost = Math.Max(8, Math.Min(15, character.AbilityScores.Constitution));
        _intelligenceCost = Math.Max(8, Math.Min(15, character.AbilityScores.Intelligence));
        _wisdomCost = Math.Max(8, Math.Min(15, character.AbilityScores.Wisdom));
        _charismaCost = Math.Max(8, Math.Min(15, character.AbilityScores.Charisma));
        
        InitializeCommands();
        CalculateAvailablePoints();
    }

    public int TotalPoints { get; private set; }

    public int AvailablePoints
    {
        get => _availablePoints;
        private set
        {
            _availablePoints = value;
            OnPropertyChanged();
        }
    }

    public int UsedPoints => TotalPoints - AvailablePoints;
    
    public bool CanApplyPointBuy => AvailablePoints >= 0;

    // Cost properties (8-15 scale)
    public int StrengthCost
    {
        get => _strengthCost;
        set
        {
            if (SetCost(ref _strengthCost, value))
            {
                _character.AbilityScores.Strength = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StrengthValue));
            }
        }
    }

    public int DexterityCost
    {
        get => _dexterityCost;
        set
        {
            if (SetCost(ref _dexterityCost, value))
            {
                _character.AbilityScores.Dexterity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DexterityValue));
            }
        }
    }

    public int ConstitutionCost
    {
        get => _constitutionCost;
        set
        {
            if (SetCost(ref _constitutionCost, value))
            {
                _character.AbilityScores.Constitution = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConstitutionValue));
            }
        }
    }

    public int IntelligenceCost
    {
        get => _intelligenceCost;
        set
        {
            if (SetCost(ref _intelligenceCost, value))
            {
                _character.AbilityScores.Intelligence = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IntelligenceValue));
            }
        }
    }

    public int WisdomCost
    {
        get => _wisdomCost;
        set
        {
            if (SetCost(ref _wisdomCost, value))
            {
                _character.AbilityScores.Wisdom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WisdomValue));
            }
        }
    }

    public int CharismaCost
    {
        get => _charismaCost;
        set
        {
            if (SetCost(ref _charismaCost, value))
            {
                _character.AbilityScores.Charisma = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CharismaValue));
            }
        }
    }

    // Display values
    public int StrengthValue => StrengthCost;
    public int DexterityValue => DexterityCost;
    public int ConstitutionValue => ConstitutionCost;
    public int IntelligenceValue => IntelligenceCost;
    public int WisdomValue => WisdomCost;
    public int CharismaValue => CharismaCost;

    // Commands
    public ICommand ResetPointBuyCommand { get; private set; } = null!;
    public ICommand ApplyPointBuyCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        ResetPointBuyCommand = new RelayCommand(ResetPointBuy);
        ApplyPointBuyCommand = new RelayCommand(ApplyPointBuy);
    }

    private bool SetCost(ref int field, int value)
    {
        // Validate point buy constraints
        if (value < 8 || value > 15) return false;

        var oldCost = GetPointCost(field);
        var newCost = GetPointCost(value);
        var costDiff = newCost - oldCost;

        if (AvailablePoints - costDiff < 0) return false;

        field = value;
        CalculateAvailablePoints();
        return true;
    }

    private static int GetPointCost(int abilityScore)
    {
        // Standard D&D 5e point buy costs
        return abilityScore switch
        {
            8 => 0,
            9 => 1,
            10 => 2,
            11 => 3,
            12 => 4,
            13 => 5,
            14 => 7,
            15 => 9,
            _ => 0
        };
    }

    private void CalculateAvailablePoints()
    {
        var usedPoints = GetPointCost(StrengthCost) +
                        GetPointCost(DexterityCost) +
                        GetPointCost(ConstitutionCost) +
                        GetPointCost(IntelligenceCost) +
                        GetPointCost(WisdomCost) +
                        GetPointCost(CharismaCost);
        
        AvailablePoints = TotalPoints - usedPoints;
        OnPropertyChanged(nameof(UsedPoints));
        OnPropertyChanged(nameof(CanApplyPointBuy));
    }

    private void ResetPointBuy()
    {
        StrengthCost = 8;
        DexterityCost = 8;
        ConstitutionCost = 8;
        IntelligenceCost = 8;
        WisdomCost = 8;
        CharismaCost = 8;
        CalculateAvailablePoints();
    }

    private void ApplyPointBuy()
    {
        // Values are already applied to character through property setters
        // This could trigger additional logic if needed
    }



    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 