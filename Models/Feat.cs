using System.ComponentModel;

namespace GToonManager.Models;

public class Feat : INotifyPropertyChanged
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Prerequisites { get; set; } = string.Empty;
    public bool IsOriginFeat { get; set; }
    public List<string> Benefits { get; set; } = new();

    // For ability score improvements in feats
    public bool HasAbilityScoreImprovement { get; set; }
    public List<string> AbilityScoreOptions { get; set; } = new();
    public int AbilityScoreIncrease { get; set; } = 1;

    // Display properties
    public string DisplayName => Name;
    public string DisplayDescription => Description;
    public string BenefitsText => string.Join("\n• ", Benefits.Prepend(""));
    
    public string FullDescription
    {
        get
        {
            var result = Description;
            if (Benefits.Count > 0)
            {
                result += "\n\nBenefits:" + BenefitsText;
            }
            if (!string.IsNullOrEmpty(Prerequisites))
            {
                result = $"Prerequisites: {Prerequisites}\n\n" + result;
            }
            return result;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 