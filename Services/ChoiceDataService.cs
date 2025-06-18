using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GToonManager.Services;

public class ChoiceDataService
{
    private JObject? _choiceData;
    private readonly string _choiceDataPath;

    public ChoiceDataService()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _choiceDataPath = Path.Combine(baseDirectory, "Data", "choice_data.json");
    }

    public async Task LoadChoiceDataAsync()
    {
        try
        {
            if (File.Exists(_choiceDataPath))
            {
                var jsonContent = await File.ReadAllTextAsync(_choiceDataPath);
                _choiceData = JObject.Parse(jsonContent);
            }
            else
            {
                throw new FileNotFoundException($"Choice data file not found at: {_choiceDataPath}");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load choice data: {ex.Message}", ex);
        }
    }

    public List<ChoiceOption> GetChoiceOptions(string choiceType)
    {
        if (_choiceData == null)
        {
            throw new InvalidOperationException("Choice data not loaded. Call LoadChoiceDataAsync() first.");
        }

        var options = new List<ChoiceOption>();

        if (_choiceData[choiceType] is JObject choiceSection)
        {
            var description = choiceSection["description"]?.ToString() ?? "";
            var optionsArray = choiceSection["options"] as JArray;

            if (optionsArray != null)
            {
                foreach (var option in optionsArray)
                {
                    var choiceOption = new ChoiceOption
                    {
                        Name = option["name"]?.ToString() ?? "",
                        Description = option["description"]?.ToString() ?? "",
                        Prerequisites = option["prerequisites"]?.ToString() ?? "",
                        Source = option["source"]?.ToString() ?? "",
                        Cost = option["cost"]?.ToString() ?? "",
                        Examples = option["examples"]?.ToString() ?? "",
                        Benefits = option["benefits"]?.ToString() ?? "",
                        Special = option["special"]?.ToString() ?? ""
                    };
                    options.Add(choiceOption);
                }
            }
        }

        return options;
    }

    public string GetChoiceDescription(string choiceType)
    {
        if (_choiceData == null)
        {
            throw new InvalidOperationException("Choice data not loaded. Call LoadChoiceDataAsync() first.");
        }

        return _choiceData[choiceType]?["description"]?.ToString() ?? "";
    }

    public bool HasChoiceData(string choiceType)
    {
        return _choiceData?.ContainsKey(choiceType) == true;
    }

    public List<string> GetAvailableChoiceTypes()
    {
        if (_choiceData == null)
        {
            return new List<string>();
        }

        var choiceTypes = new List<string>();
        foreach (var property in _choiceData.Properties())
        {
            choiceTypes.Add(property.Name);
        }
        return choiceTypes;
    }

    // Specific helper methods for common choice types
    public List<ChoiceOption> GetBattleMasterManeuvers()
    {
        return GetChoiceOptions("battle_master_maneuvers");
    }

    public List<ChoiceOption> GetArtisanTools()
    {
        return GetChoiceOptions("artisan_tools");
    }

    public List<ChoiceOption> GetMetamagicOptions()
    {
        return GetChoiceOptions("metamagic");
    }

    public List<ChoiceOption> GetEldritchInvocations()
    {
        return GetChoiceOptions("eldritch_invocations");
    }

    public List<ChoiceOption> GetCreatureTypes()
    {
        return GetChoiceOptions("creature_types");
    }

    public List<ChoiceOption> GetTerrainTypes()
    {
        return GetChoiceOptions("terrain_types");
    }

    public List<ChoiceOption> GetPactBoons()
    {
        return GetChoiceOptions("pact_boons");
    }

    // Method to get filtered options based on prerequisites
    public List<ChoiceOption> GetFilteredChoiceOptions(string choiceType, int characterLevel = 1, List<string>? knownFeatures = null)
    {
        var allOptions = GetChoiceOptions(choiceType);
        var filteredOptions = new List<ChoiceOption>();

        foreach (var option in allOptions)
        {
            if (MeetsPrerequisites(option, characterLevel, knownFeatures))
            {
                filteredOptions.Add(option);
            }
        }

        return filteredOptions;
    }

    private bool MeetsPrerequisites(ChoiceOption option, int characterLevel, List<string>? knownFeatures)
    {
        if (string.IsNullOrEmpty(option.Prerequisites))
        {
            return true;
        }

        var prerequisites = option.Prerequisites.ToLower();

        // Check level requirements
        if (prerequisites.Contains("level"))
        {
            var levelMatch = System.Text.RegularExpressions.Regex.Match(prerequisites, @"(\d+).*level");
            if (levelMatch.Success && int.TryParse(levelMatch.Groups[1].Value, out int requiredLevel))
            {
                if (characterLevel < requiredLevel)
                {
                    return false;
                }
            }
        }

        // Check feature requirements
        if (knownFeatures != null && prerequisites.Contains("pact of"))
        {
            var pactMatch = System.Text.RegularExpressions.Regex.Match(prerequisites, @"pact of the (\w+)");
            if (pactMatch.Success)
            {
                var requiredPact = $"Pact of the {char.ToUpper(pactMatch.Groups[1].Value[0])}{pactMatch.Groups[1].Value.Substring(1)}";
                if (!knownFeatures.Contains(requiredPact))
                {
                    return false;
                }
            }
        }

        // Check cantrip requirements
        if (knownFeatures != null && prerequisites.Contains("cantrip"))
        {
            if (prerequisites.Contains("eldritch blast") && !knownFeatures.Contains("Eldritch Blast"))
            {
                return false;
            }
        }

        return true;
    }
}

public class ChoiceOption
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Prerequisites { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Cost { get; set; } = string.Empty;
    public string Examples { get; set; } = string.Empty;
    public string Benefits { get; set; } = string.Empty;
    public string Special { get; set; } = string.Empty;
} 