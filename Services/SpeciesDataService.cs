using System.IO;
using System.Text.Json;
using GToonManager.Models;

namespace GToonManager.Services;

public class SpeciesDataService
{
    private readonly string _speciesDataPath;

    public SpeciesDataService()
    {
        _speciesDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "species");
    }

    public async Task<List<Species>> LoadAllSpeciesAsync()
    {
        var species = new List<Species>();

        if (!Directory.Exists(_speciesDataPath))
        {
            System.Diagnostics.Debug.WriteLine($"Species data directory not found: {_speciesDataPath}");
            return species;
        }

        var jsonFiles = Directory.GetFiles(_speciesDataPath, "*.json");
        
        foreach (var file in jsonFiles)
        {
            try
            {
                var speciesEntry = await LoadSpeciesFromFileAsync(file);
                if (speciesEntry != null)
                {
                    species.Add(speciesEntry);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading species from {file}: {ex.Message}");
            }
        }

        return species.OrderBy(s => s.Name).ToList();
    }

    public async Task<Species?> LoadSpeciesFromFileAsync(string filePath)
    {
        try
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var speciesData = JsonSerializer.Deserialize<JsonElement>(jsonContent, jsonOptions);
            return ParseSpeciesFromJson(speciesData);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading species from file {filePath}: {ex.Message}");
            return null;
        }
    }

    private Species ParseSpeciesFromJson(JsonElement speciesData)
    {
        var species = new Species();

        if (speciesData.TryGetProperty("name", out var nameElement))
            species.Name = nameElement.GetString() ?? string.Empty;

        if (speciesData.TryGetProperty("description", out var descElement))
            species.Description = descElement.GetString() ?? string.Empty;

        if (speciesData.TryGetProperty("source", out var sourceElement))
            species.Source = sourceElement.GetString() ?? string.Empty;

        if (speciesData.TryGetProperty("source_page", out var pageElement))
            species.SourcePage = pageElement.GetInt32();

        // Parse ability scores
        if (speciesData.TryGetProperty("ability_scores", out var abilityElement))
        {
            species.AbilityScores = ParseAbilityScores(abilityElement);
        }

        // Parse traits
        if (speciesData.TryGetProperty("traits", out var traitsElement) && traitsElement.ValueKind == JsonValueKind.Array)
        {
            species.Traits = ParseTraits(traitsElement);
        }

        // Parse subraces
        if (speciesData.TryGetProperty("subraces", out var subracesElement) && subracesElement.ValueKind == JsonValueKind.Array)
        {
            species.Subraces = ParseSubspecies(subracesElement);
        }

        // Parse size
        if (speciesData.TryGetProperty("size", out var sizeElement))
        {
            species.Size = ParseSize(sizeElement);
        }

        // Parse speed
        if (speciesData.TryGetProperty("speed", out var speedElement))
        {
            species.Speed = ParseSpeed(speedElement);
        }

        // Parse languages
        if (speciesData.TryGetProperty("languages", out var languagesElement))
        {
            species.Languages = ParseLanguages(languagesElement);
        }

        // Parse age
        if (speciesData.TryGetProperty("age", out var ageElement))
        {
            species.Age = ParseAge(ageElement);
        }

        return species;
    }

    private Dictionary<string, int> ParseAbilityScores(JsonElement abilityElement)
    {
        var abilities = new Dictionary<string, int>();
        
        foreach (var property in abilityElement.EnumerateObject())
        {
            // Check if this is a simple integer value or a complex "choose" structure
            if (property.Value.ValueKind == JsonValueKind.Number)
            {
                abilities[property.Name] = property.Value.GetInt32();
            }
            else if (property.Name == "choose" && property.Value.ValueKind == JsonValueKind.Object)
            {
                // For now, we'll skip the "choose" mechanism as it requires user interaction
                // This is typically handled by character creation UI where user selects which abilities to boost
                // We could implement this later with a more sophisticated system
                //TODO: Implement the choose mechanism
                continue;
            }
        }

        return abilities;
    }

    private List<SpeciesTrait> ParseTraits(JsonElement traitsElement)
    {
        var traits = new List<SpeciesTrait>();

        foreach (var traitElement in traitsElement.EnumerateArray())
        {
            var trait = new SpeciesTrait();

            if (traitElement.TryGetProperty("name", out var nameElement))
                trait.Name = nameElement.GetString() ?? string.Empty;

            if (traitElement.TryGetProperty("description", out var descElement))
                trait.Description = descElement.GetString() ?? string.Empty;

            if (traitElement.TryGetProperty("range", out var rangeElement))
                trait.Range = rangeElement.GetInt32();

            if (traitElement.TryGetProperty("grants", out var grantsElement))
                trait.Grants = ParseTraitGrants(grantsElement);

            traits.Add(trait);
        }

        return traits;
    }

    private TraitGrants ParseTraitGrants(JsonElement grantsElement)
    {
        var grants = new TraitGrants();

        // Check for skill_proficiencies (from original JSON files) OR skillProficiencies (from saved character JSON)
        if (grantsElement.TryGetProperty("skill_proficiencies", out var skillsElement) ||
            grantsElement.TryGetProperty("skillProficiencies", out skillsElement))
        {
            if (skillsElement.ValueKind == JsonValueKind.Array)
            {
                // Handle simple array of skill names
                foreach (var skill in skillsElement.EnumerateArray())
                {
                    grants.SkillProficiencies.Add(skill.GetString() ?? string.Empty);
                }
            }
            else if (skillsElement.ValueKind == JsonValueKind.Object && skillsElement.TryGetProperty("choose", out var chooseElement))
            {
                // Handle "choose" structure - for now we'll skip this as it requires user interaction
                // This should be handled by character creation UI where user selects specific skills
                // We could add a note or placeholder to indicate this requires choice
                //TODO: Implement the choose mechanism
            }
        }

        if (grantsElement.TryGetProperty("weapon_proficiencies", out var weaponsElement) && weaponsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var weapon in weaponsElement.EnumerateArray())
            {
                grants.WeaponProficiencies.Add(weapon.GetString() ?? string.Empty);
            }
        }

        if (grantsElement.TryGetProperty("armor_proficiencies", out var armorElement) && armorElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var armor in armorElement.EnumerateArray())
            {
                grants.ArmorProficiencies.Add(armor.GetString() ?? string.Empty);
            }
        }

        if (grantsElement.TryGetProperty("tool_proficiencies", out var toolsElement) && toolsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var tool in toolsElement.EnumerateArray())
            {
                grants.ToolProficiencies.Add(tool.GetString() ?? string.Empty);
            }
        }

        if (grantsElement.TryGetProperty("damage_resistances", out var resistancesElement) && resistancesElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var resistance in resistancesElement.EnumerateArray())
            {
                grants.DamageResistances.Add(resistance.GetString() ?? string.Empty);
            }
        }

        if (grantsElement.TryGetProperty("saving_throw_advantages", out var savingThrowsElement) && savingThrowsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var savingThrow in savingThrowsElement.EnumerateArray())
            {
                grants.SavingThrowAdvantages.Add(savingThrow.GetString() ?? string.Empty);
            }
        }

        return grants;
    }

    private List<Subspecies> ParseSubspecies(JsonElement subracesElement)
    {
        var subspecies = new List<Subspecies>();

        foreach (var subraceElement in subracesElement.EnumerateArray())
        {
            var subSpecies = new Subspecies();

            if (subraceElement.TryGetProperty("name", out var nameElement))
                subSpecies.Name = nameElement.GetString() ?? string.Empty;

            if (subraceElement.TryGetProperty("description", out var descElement))
                subSpecies.Description = descElement.GetString() ?? string.Empty;

            if (subraceElement.TryGetProperty("source", out var sourceElement))
                subSpecies.Source = sourceElement.GetString() ?? string.Empty;

            if (subraceElement.TryGetProperty("ability_scores", out var abilityElement))
            {
                subSpecies.AbilityScores = ParseAbilityScores(abilityElement);
            }

            if (subraceElement.TryGetProperty("traits", out var traitsElement) && traitsElement.ValueKind == JsonValueKind.Array)
            {
                subSpecies.Traits = ParseTraits(traitsElement);
            }

            subspecies.Add(subSpecies);
        }

        return subspecies;
    }

    private SpeciesSize ParseSize(JsonElement sizeElement)
    {
        var size = new SpeciesSize();

        if (sizeElement.TryGetProperty("category", out var categoryElement))
            size.Category = categoryElement.GetString() ?? "Medium";

        if (sizeElement.TryGetProperty("height", out var heightElement))
            size.Height = heightElement.GetString() ?? string.Empty;

        if (sizeElement.TryGetProperty("weight", out var weightElement))
            size.Weight = weightElement.GetString() ?? string.Empty;

        return size;
    }

    private SpeciesSpeed ParseSpeed(JsonElement speedElement)
    {
        var speed = new SpeciesSpeed();

        if (speedElement.TryGetProperty("walk", out var walkElement))
            speed.Walk = walkElement.GetInt32();

        if (speedElement.TryGetProperty("swim", out var swimElement))
            speed.Swim = swimElement.GetInt32();

        if (speedElement.TryGetProperty("fly", out var flyElement))
            speed.Fly = flyElement.GetInt32();

        if (speedElement.TryGetProperty("climb", out var climbElement))
            speed.Climb = climbElement.GetInt32();

        return speed;
    }

    private SpeciesLanguages ParseLanguages(JsonElement languagesElement)
    {
        var languages = new SpeciesLanguages();

        if (languagesElement.TryGetProperty("standard", out var standardElement) && standardElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var lang in standardElement.EnumerateArray())
            {
                languages.Standard.Add(lang.GetString() ?? string.Empty);
            }
        }

        return languages;
    }

    private SpeciesAge ParseAge(JsonElement ageElement)
    {
        var age = new SpeciesAge();

        if (ageElement.TryGetProperty("maturity", out var maturityElement))
            age.Maturity = maturityElement.GetInt32();

        if (ageElement.TryGetProperty("lifespan", out var lifespanElement))
            age.Lifespan = lifespanElement.GetString() ?? string.Empty;

        return age;
    }
} 