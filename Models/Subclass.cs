using System.Collections.Generic;

namespace GToonManager.Models;

public class Subclass
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public Dictionary<int, List<SubclassFeature>> Features { get; set; } = new();
    
    public List<int> FeatureLevels => Features.Keys.OrderBy(k => k).ToList();
    
    public List<SubclassFeature> GetFeaturesAtLevel(int level)
    {
        return Features.TryGetValue(level, out var features) ? features : new List<SubclassFeature>();
    }
    
    public List<SubclassFeature> GetFeaturesUpToLevel(int level)
    {
        var allFeatures = new List<SubclassFeature>();
        foreach (var featureLevel in FeatureLevels.Where(l => l <= level))
        {
            allFeatures.AddRange(GetFeaturesAtLevel(featureLevel));
        }
        return allFeatures;
    }
    
    public override string ToString() => Name;
}

public class SubclassFeature
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object? Mechanics { get; set; } // For future game mechanics implementation
    
    public override string ToString() => Name;
} 