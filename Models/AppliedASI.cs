namespace GToonManager.Models;

public class AppliedASI
{
    public string Id { get; set; } = string.Empty;
    public int Level { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public ASIImprovementType ImprovementType { get; set; }
    public string FirstAbility { get; set; } = string.Empty;
    public string? SecondAbility { get; set; }

    public string GetDescription()
    {
        if (ImprovementType == ASIImprovementType.Single)
        {
            return $"{FirstAbility} +2";
        }
        else
        {
            return $"{FirstAbility} +1, {SecondAbility} +1";
        }
    }

    public override string ToString()
    {
        return $"Level {Level} {ClassName}: {GetDescription()}";
    }
}

public enum ASIImprovementType
{
    Single,
    Double
} 