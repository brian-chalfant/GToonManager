using System.IO;
using System.Text.Json;
using GToonManager.Models;

namespace GToonManager.Services;

public class RaceDataService
{
    private readonly string _raceDataPath;

    public RaceDataService()
    {
        _raceDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "races");
    }

    public async Task<List<Race>> LoadAllRacesAsync()
    {
        var races = new List<Race>();

        if (!Directory.Exists(_raceDataPath))
        {
            System.Diagnostics.Debug.WriteLine($"Race data directory not found: {_raceDataPath}");
            return races;
        }

        var jsonFiles = Directory.GetFiles(_raceDataPath, "*.json");
        
        foreach (var file in jsonFiles)
        {
            try
            {
                var race = await LoadRaceFromFileAsync(file);
                if (race != null)
                {
                    races.Add(race);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading race from {file}: {ex.Message}");
            }
        }

        return races.OrderBy(r => r.Name).ToList();
    }

    public async Task<Race?> LoadRaceFromFileAsync(string filePath)
    {
        try
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var raceData = JsonSerializer.Deserialize<JsonElement>(jsonContent, jsonOptions);
            return ParseRaceFromJson(raceData);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading race from file {filePath}: {ex.Message}");
            return null;
        }
    }

    private Race ParseRaceFromJson(JsonElement raceData)
    {
        var race = new Race();

        if (raceData.TryGetProperty("name", out var nameElement))
            race.Name = nameElement.GetString() ?? string.Empty;

        if (raceData.TryGetProperty("description", out var descElement))
            race.Description = descElement.GetString() ?? string.Empty;

        if (raceData.TryGetProperty("source", out var sourceElement))
            race.Source = sourceElement.GetString() ?? string.Empty;

        if (raceData.TryGetProperty("source_page", out var pageElement))
            race.SourcePage = pageElement.GetInt32();

        // Parse ability scores
        if (raceData.TryGetProperty("ability_scores", out var abilityElement))
        {
            race.AbilityScores = ParseAbilityScores(abilityElement);
        }

        // Parse traits
        if (raceData.TryGetProperty("traits", out var traitsElement) && traitsElement.ValueKind == JsonValueKind.Array)
        {
            race.Traits = ParseTraits(traitsElement);
        }

        // Parse subraces
        if (raceData.TryGetProperty("subraces", out var subracesElement) && subracesElement.ValueKind == JsonValueKind.Array)
        {
            race.Subraces = ParseSubraces(subracesElement);
        }

        // Parse size
        if (raceData.TryGetProperty("size", out var sizeElement))
        {
            race.Size = ParseSize(sizeElement);
        }

        // Parse speed
        if (raceData.TryGetProperty("speed", out var speedElement))
        {
            race.Speed = ParseSpeed(speedElement);
        }

        // Parse languages
        if (raceData.TryGetProperty("languages", out var languagesElement))
        {
            race.Languages = ParseLanguages(languagesElement);
        }

        // Parse age
        if (raceData.TryGetProperty("age", out var ageElement))
        {
            race.Age = ParseAge(ageElement);
        }

        return race;
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
                continue;
            }
        }

        return abilities;
    }

    private List<RaceTrait> ParseTraits(JsonElement traitsElement)
    {
        var traits = new List<RaceTrait>();

        foreach (var traitElement in traitsElement.EnumerateArray())
        {
            var trait = new RaceTrait();

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

        if (grantsElement.TryGetProperty("skill_proficiencies", out var skillsElement))
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

        if (grantsElement.TryGetProperty("damage_resistances", out var resistancesElement) && resistancesElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var resistance in resistancesElement.EnumerateArray())
            {
                grants.DamageResistances.Add(resistance.GetString() ?? string.Empty);
            }
        }

        // Handle feat grants (like variant human)
        if (grantsElement.TryGetProperty("feat", out var featElement))
        {
            // For now, we'll just acknowledge that this race grants a feat
            // The actual feat selection would be handled by character creation UI
            // We could add this to a separate property later if needed
        }

        return grants;
    }

    private List<Subrace> ParseSubraces(JsonElement subracesElement)
    {
        var subraces = new List<Subrace>();

        foreach (var subraceElement in subracesElement.EnumerateArray())
        {
            var subrace = new Subrace();

            if (subraceElement.TryGetProperty("name", out var nameElement))
                subrace.Name = nameElement.GetString() ?? string.Empty;

            if (subraceElement.TryGetProperty("description", out var descElement))
                subrace.Description = descElement.GetString() ?? string.Empty;

            if (subraceElement.TryGetProperty("source", out var sourceElement))
                subrace.Source = sourceElement.GetString() ?? string.Empty;

            if (subraceElement.TryGetProperty("ability_scores", out var abilityElement))
                subrace.AbilityScores = ParseAbilityScores(abilityElement);

            if (subraceElement.TryGetProperty("traits", out var traitsElement) && traitsElement.ValueKind == JsonValueKind.Array)
                subrace.Traits = ParseTraits(traitsElement);

            subraces.Add(subrace);
        }

        return subraces;
    }

    private RaceSize ParseSize(JsonElement sizeElement)
    {
        var size = new RaceSize();

        if (sizeElement.TryGetProperty("category", out var categoryElement))
            size.Category = categoryElement.GetString() ?? "Medium";

        return size;
    }

    private RaceSpeed ParseSpeed(JsonElement speedElement)
    {
        var speed = new RaceSpeed();

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

    private RaceLanguages ParseLanguages(JsonElement languagesElement)
    {
        var languages = new RaceLanguages();

        if (languagesElement.TryGetProperty("standard", out var standardElement) && standardElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var lang in standardElement.EnumerateArray())
            {
                languages.Standard.Add(lang.GetString() ?? string.Empty);
            }
        }

        return languages;
    }

    private RaceAge ParseAge(JsonElement ageElement)
    {
        var age = new RaceAge();

        if (ageElement.TryGetProperty("maturity", out var maturityElement))
            age.Maturity = maturityElement.GetInt32();

        if (ageElement.TryGetProperty("lifespan", out var lifespanElement))
            age.Lifespan = lifespanElement.GetString() ?? string.Empty;

        return age;
    }
} 