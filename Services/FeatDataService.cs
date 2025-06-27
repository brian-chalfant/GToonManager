using System.IO;
using System.Text.Json;
using GToonManager.Models;

namespace GToonManager.Services;

public class FeatDataService
{
    private Dictionary<string, Feat>? _originFeats;
    private Dictionary<string, Feat>? _generalFeats;
    private readonly string _dataPath;

    public FeatDataService()
    {
        _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "feats");
    }

    public async Task LoadFeatsAsync()
    {
        try
        {
            await LoadOriginFeatsAsync();
            // Load other feat types as needed
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading feats: {ex.Message}");
            // Initialize empty dictionaries to prevent null reference exceptions
            _originFeats ??= new Dictionary<string, Feat>();
            _generalFeats ??= new Dictionary<string, Feat>();
        }
    }

    private async Task LoadOriginFeatsAsync()
    {
        var filePath = Path.Combine(_dataPath, "origin_feats.json");
        if (!File.Exists(filePath))
        {
            System.Diagnostics.Debug.WriteLine($"Origin feats file not found: {filePath}");
            _originFeats = new Dictionary<string, Feat>();
            return;
        }

        var jsonContent = await File.ReadAllTextAsync(filePath);
        var document = JsonDocument.Parse(jsonContent);
        
        _originFeats = new Dictionary<string, Feat>();

        if (document.RootElement.TryGetProperty("feats", out var featsElement))
        {
            foreach (var featProperty in featsElement.EnumerateObject())
            {
                var featName = featProperty.Name;
                var featData = featProperty.Value;

                var feat = new Feat
                {
                    Name = featName,
                    Description = featData.GetProperty("description").GetString() ?? "",
                    Source = featData.TryGetProperty("source", out var sourceElement) ? sourceElement.GetString() ?? "" : "",
                    Prerequisites = featData.TryGetProperty("prerequisites", out var prereqElement) && prereqElement.ValueKind != JsonValueKind.Null 
                        ? prereqElement.GetString() ?? "" 
                        : "",
                    IsOriginFeat = featData.TryGetProperty("origin_feat", out var originElement) && originElement.GetBoolean()
                };

                // Parse benefits
                if (featData.TryGetProperty("benefits", out var benefitsElement) && benefitsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var benefit in benefitsElement.EnumerateArray())
                    {
                        feat.Benefits.Add(benefit.GetString() ?? "");
                    }
                }

                _originFeats[featName] = feat;
            }
        }

        System.Diagnostics.Debug.WriteLine($"Loaded {_originFeats.Count} origin feats");
    }

    public List<Feat> GetOriginFeats()
    {
        return _originFeats?.Values.ToList() ?? new List<Feat>();
    }

    public List<Feat> GetGeneralFeats()
    {
        return _generalFeats?.Values.ToList() ?? new List<Feat>();
    }

    public List<Feat> GetAllFeats()
    {
        var allFeats = new List<Feat>();
        if (_originFeats != null) allFeats.AddRange(_originFeats.Values);
        if (_generalFeats != null) allFeats.AddRange(_generalFeats.Values);
        return allFeats;
    }

    public Feat? GetFeatByName(string name)
    {
        if (_originFeats?.TryGetValue(name, out var originFeat) == true)
            return originFeat;
        
        if (_generalFeats?.TryGetValue(name, out var generalFeat) == true)
            return generalFeat;

        return null;
    }
} 