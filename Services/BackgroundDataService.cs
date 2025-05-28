using System.IO;
using System.Text.Json;
using GToonManager.Models;

namespace GToonManager.Services;

public class BackgroundDataService
{
    private readonly string _backgroundDataPath;

    public BackgroundDataService()
    {
        _backgroundDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "backgrounds");
    }

    public async Task<List<Background>> LoadAllBackgroundsAsync()
    {
        var backgrounds = new List<Background>();

        if (!Directory.Exists(_backgroundDataPath))
        {
            System.Diagnostics.Debug.WriteLine($"Background data directory not found: {_backgroundDataPath}");
            return backgrounds;
        }

        var jsonFiles = Directory.GetFiles(_backgroundDataPath, "*.json");
        
        foreach (var file in jsonFiles)
        {
            try
            {
                var background = await LoadBackgroundFromFileAsync(file);
                if (background != null)
                {
                    backgrounds.Add(background);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading background from {file}: {ex.Message}");
            }
        }

        return backgrounds.OrderBy(b => b.Name).ToList();
    }

    public async Task<Background?> LoadBackgroundFromFileAsync(string filePath)
    {
        try
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var backgroundData = JsonSerializer.Deserialize<JsonElement>(jsonContent, jsonOptions);
            return ParseBackgroundFromJson(backgroundData);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading background from file {filePath}: {ex.Message}");
            return null;
        }
    }

    private Background ParseBackgroundFromJson(JsonElement backgroundData)
    {
        var background = new Background();

        if (backgroundData.TryGetProperty("name", out var nameElement))
            background.Name = nameElement.GetString() ?? string.Empty;

        if (backgroundData.TryGetProperty("description", out var descElement))
            background.Description = descElement.GetString() ?? string.Empty;

        if (backgroundData.TryGetProperty("suggested_characteristics", out var suggestedElement))
            background.SuggestedCharacteristics = suggestedElement.GetString() ?? string.Empty;

        // Parse feature
        if (backgroundData.TryGetProperty("feature", out var featureElement))
        {
            background.Feature = ParseFeature(featureElement);
        }

        // Parse proficiency grants
        if (backgroundData.TryGetProperty("proficiency_grants", out var proficiencyElement))
        {
            background.ProficiencyGrants = ParseProficiencyGrants(proficiencyElement);
        }

        // Parse equipment grants
        if (backgroundData.TryGetProperty("equipment_grants", out var equipmentElement))
        {
            background.EquipmentGrants = ParseEquipmentGrants(equipmentElement);
        }

        // Parse personality traits
        if (backgroundData.TryGetProperty("personality", out var personalityElement))
        {
            background.Personality = ParsePersonalityTraits(personalityElement);
        }

        return background;
    }

    private BackgroundFeature ParseFeature(JsonElement featureElement)
    {
        var feature = new BackgroundFeature();

        if (featureElement.TryGetProperty("name", out var nameElement))
            feature.Name = nameElement.GetString() ?? string.Empty;

        if (featureElement.TryGetProperty("description", out var descElement))
            feature.Description = descElement.GetString() ?? string.Empty;

        return feature;
    }

    private ProficiencyGrants ParseProficiencyGrants(JsonElement proficiencyElement)
    {
        var grants = new ProficiencyGrants();

        // Parse skills
        if (proficiencyElement.TryGetProperty("skills", out var skillsElement) && skillsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var skill in skillsElement.EnumerateArray())
            {
                var skillName = skill.GetString();
                if (!string.IsNullOrEmpty(skillName))
                {
                    grants.Skills.Add(skillName);
                }
            }
        }

        // Parse languages
        if (proficiencyElement.TryGetProperty("languages", out var languagesElement))
        {
            grants.Languages = ParseLanguageGrant(languagesElement);
        }

        // Parse tools
        if (proficiencyElement.TryGetProperty("tools", out var toolsElement) && toolsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var tool in toolsElement.EnumerateArray())
            {
                var toolName = tool.GetString();
                if (!string.IsNullOrEmpty(toolName))
                {
                    grants.Tools.Add(toolName);
                }
            }
        }

        return grants;
    }

    private LanguageGrant ParseLanguageGrant(JsonElement languageElement)
    {
        var languageGrant = new LanguageGrant();

        if (languageElement.TryGetProperty("count", out var countElement))
            languageGrant.Count = countElement.GetInt32();

        if (languageElement.TryGetProperty("type", out var typeElement))
            languageGrant.Type = typeElement.GetString() ?? string.Empty;

        if (languageElement.TryGetProperty("description", out var descElement))
            languageGrant.Description = descElement.GetString() ?? string.Empty;

        // Handle known languages if they exist as an array
        if (languageElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var lang in languageElement.EnumerateArray())
            {
                var langName = lang.GetString();
                if (!string.IsNullOrEmpty(langName))
                {
                    languageGrant.Known.Add(langName);
                }
            }
        }

        return languageGrant;
    }

    private EquipmentGrants ParseEquipmentGrants(JsonElement equipmentElement)
    {
        var grants = new EquipmentGrants();

        // Parse fixed equipment
        if (equipmentElement.TryGetProperty("fixed", out var fixedElement) && fixedElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in fixedElement.EnumerateArray())
            {
                var equipmentItem = new EquipmentItem();

                if (item.TryGetProperty("item", out var itemElement))
                    equipmentItem.Item = itemElement.GetString() ?? string.Empty;

                if (item.TryGetProperty("quantity", out var quantityElement))
                    equipmentItem.Quantity = quantityElement.GetInt32();

                if (item.TryGetProperty("description", out var descElement))
                    equipmentItem.Description = descElement.GetString() ?? string.Empty;

                grants.Fixed.Add(equipmentItem);
            }
        }

        // Parse currency
        if (equipmentElement.TryGetProperty("currency", out var currencyElement))
        {
            grants.Currency = ParseCurrency(currencyElement);
        }

        return grants;
    }

    private Currency ParseCurrency(JsonElement currencyElement)
    {
        var currency = new Currency();

        if (currencyElement.TryGetProperty("gold", out var goldElement))
            currency.Gold = goldElement.GetInt32();

        if (currencyElement.TryGetProperty("silver", out var silverElement))
            currency.Silver = silverElement.GetInt32();

        if (currencyElement.TryGetProperty("copper", out var copperElement))
            currency.Copper = copperElement.GetInt32();

        return currency;
    }

    private PersonalityTraits ParsePersonalityTraits(JsonElement personalityElement)
    {
        var personality = new PersonalityTraits();

        if (personalityElement.TryGetProperty("personality_traits", out var traitsElement))
        {
            personality.PersonalityTraitsOptions = ParseTraitOptions(traitsElement, false);
        }

        if (personalityElement.TryGetProperty("ideals", out var idealsElement))
        {
            personality.Ideals = ParseTraitOptions(idealsElement, true);
        }

        if (personalityElement.TryGetProperty("bonds", out var bondsElement))
        {
            personality.Bonds = ParseTraitOptions(bondsElement, false);
        }

        if (personalityElement.TryGetProperty("flaws", out var flawsElement))
        {
            personality.Flaws = ParseTraitOptions(flawsElement, false);
        }

        return personality;
    }

    private TraitOptions ParseTraitOptions(JsonElement traitElement, bool isIdeals)
    {
        var options = new TraitOptions();

        if (traitElement.TryGetProperty("count", out var countElement))
            options.Count = countElement.GetInt32();

        if (traitElement.TryGetProperty("type", out var typeElement))
            options.Type = typeElement.GetString() ?? string.Empty;

        if (traitElement.TryGetProperty("suggestions", out var suggestionsElement) && suggestionsElement.ValueKind == JsonValueKind.Array)
        {
            if (isIdeals)
            {
                // Handle ideals with alignment information
                foreach (var suggestion in suggestionsElement.EnumerateArray())
                {
                    if (suggestion.ValueKind == JsonValueKind.Object)
                    {
                        var ideal = new IdealOption();
                        
                        if (suggestion.TryGetProperty("ideal", out var idealElement))
                            ideal.Ideal = idealElement.GetString() ?? string.Empty;

                        if (suggestion.TryGetProperty("alignment", out var alignmentElement))
                            ideal.Alignment = alignmentElement.GetString() ?? string.Empty;

                        options.IdealSuggestions.Add(ideal);
                    }
                }
            }
            else
            {
                // Handle regular string suggestions
                foreach (var suggestion in suggestionsElement.EnumerateArray())
                {
                    var suggestionText = suggestion.GetString();
                    if (!string.IsNullOrEmpty(suggestionText))
                    {
                        options.Suggestions.Add(suggestionText);
                    }
                }
            }
        }

        return options;
    }
} 