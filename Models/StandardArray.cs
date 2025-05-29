using System.ComponentModel;
using System.Collections.ObjectModel;

namespace GToonManager.Models;

public class StandardArray : INotifyPropertyChanged
{
    // The standard array values for D&D 2024
    public static readonly int[] StandardArrayValues = { 15, 14, 13, 12, 10, 8 };
    
    private readonly Dictionary<string, int> _assignedValues = new();
    private readonly List<int> _availableValues = new(StandardArrayValues);

    public StandardArray()
    {
        // Initialize with default assignments (all unassigned)
        _assignedValues["Strength"] = 0;
        _assignedValues["Dexterity"] = 0;
        _assignedValues["Constitution"] = 0;
        _assignedValues["Intelligence"] = 0;
        _assignedValues["Wisdom"] = 0;
        _assignedValues["Charisma"] = 0;
    }

    public int GetAssignedValue(string ability)
    {
        return _assignedValues.TryGetValue(ability, out var value) ? value : 0;
    }

    public void AssignValue(string ability, int value)
    {
        if (!_assignedValues.ContainsKey(ability))
            throw new ArgumentException($"Invalid ability: {ability}");

        // Remove the old value from assigned and add it back to available
        var oldValue = _assignedValues[ability];
        if (oldValue > 0)
        {
            _availableValues.Add(oldValue);
        }

        // Remove the new value from available and assign it
        if (value > 0)
        {
            if (!_availableValues.Contains(value))
                throw new ArgumentException($"Value {value} is not available for assignment");
            
            _availableValues.Remove(value);
        }

        _assignedValues[ability] = value;
        OnPropertyChanged(nameof(AvailableValues));
        OnPropertyChanged($"{ability}Value");
        OnPropertyChanged(nameof(IsComplete));
    }

    public void ClearAssignment(string ability)
    {
        AssignValue(ability, 0);
    }

    public List<int> AvailableValues => _availableValues.OrderByDescending(x => x).ToList();

    public bool IsComplete => _assignedValues.Values.All(v => v > 0);

    // Properties for binding to UI
    public int StrengthValue
    {
        get => GetAssignedValue("Strength");
        set => AssignValue("Strength", value);
    }

    public int DexterityValue
    {
        get => GetAssignedValue("Dexterity");
        set => AssignValue("Dexterity", value);
    }

    public int ConstitutionValue
    {
        get => GetAssignedValue("Constitution");
        set => AssignValue("Constitution", value);
    }

    public int IntelligenceValue
    {
        get => GetAssignedValue("Intelligence");
        set => AssignValue("Intelligence", value);
    }

    public int WisdomValue
    {
        get => GetAssignedValue("Wisdom");
        set => AssignValue("Wisdom", value);
    }

    public int CharismaValue
    {
        get => GetAssignedValue("Charisma");
        set => AssignValue("Charisma", value);
    }

    public void ApplyToAbilityScores(AbilityScores abilityScores)
    {
        if (!IsComplete)
            throw new InvalidOperationException("Cannot apply incomplete standard array assignments");

        abilityScores.Strength = StrengthValue;
        abilityScores.Dexterity = DexterityValue;
        abilityScores.Constitution = ConstitutionValue;
        abilityScores.Intelligence = IntelligenceValue;
        abilityScores.Wisdom = WisdomValue;
        abilityScores.Charisma = CharismaValue;
    }

    public void LoadFromAbilityScores(AbilityScores abilityScores)
    {
        // Clear current assignments
        _availableValues.Clear();
        _availableValues.AddRange(StandardArrayValues);
        
        foreach (var ability in _assignedValues.Keys.ToList())
        {
            _assignedValues[ability] = 0;
        }

        // Try to assign values if they match standard array values
        var scores = new Dictionary<string, int>
        {
            ["Strength"] = abilityScores.Strength,
            ["Dexterity"] = abilityScores.Dexterity,
            ["Constitution"] = abilityScores.Constitution,
            ["Intelligence"] = abilityScores.Intelligence,
            ["Wisdom"] = abilityScores.Wisdom,
            ["Charisma"] = abilityScores.Charisma
        };

        foreach (var kvp in scores)
        {
            if (StandardArrayValues.Contains(kvp.Value) && _availableValues.Contains(kvp.Value))
            {
                AssignValue(kvp.Key, kvp.Value);
            }
        }

        OnPropertyChanged(nameof(AvailableValues));
        OnPropertyChanged(nameof(IsComplete));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 