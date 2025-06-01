using System.IO;
using System.Text.Json;
using GToonManager.Models;

namespace GToonManager.Services;

public class ClassDataService
{
    private readonly string _classDataPath;

    public ClassDataService()
    {
        _classDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "classes");
    }

    public async Task<List<CharacterClass>> LoadAllClassesAsync()
    {
        var classes = new List<CharacterClass>();

        if (!Directory.Exists(_classDataPath))
        {
            System.Diagnostics.Debug.WriteLine($"Class data directory not found: {_classDataPath}");
            return classes;
        }

        var jsonFiles = Directory.GetFiles(_classDataPath, "*.json")
            .Where(f => !Path.GetFileName(f).Equals("class_template.json", StringComparison.OrdinalIgnoreCase));
        
        foreach (var file in jsonFiles)
        {
            try
            {
                var characterClass = await LoadClassFromFileAsync(file);
                if (characterClass != null)
                {
                    classes.Add(characterClass);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading class from {file}: {ex.Message}");
            }
        }

        return classes.OrderBy(c => c.Name).ToList();
    }

    public async Task<CharacterClass?> LoadClassFromFileAsync(string filePath)
    {
        try
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var classData = JsonSerializer.Deserialize<JsonElement>(jsonContent, jsonOptions);
            return ParseClassFromJson(classData);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading class from file {filePath}: {ex.Message}");
            return null;
        }
    }

    private CharacterClass ParseClassFromJson(JsonElement classData)
    {
        var characterClass = new CharacterClass();

        if (classData.TryGetProperty("name", out var nameElement))
            characterClass.Name = nameElement.GetString() ?? string.Empty;

        if (classData.TryGetProperty("description", out var descElement))
            characterClass.Description = descElement.GetString() ?? string.Empty;

        // Parse hit_dice (e.g., "1d10" -> 10)
        if (classData.TryGetProperty("hit_dice", out var hitDiceElement))
        {
            var hitDiceStr = hitDiceElement.GetString() ?? "";
            if (hitDiceStr.StartsWith("1d") && int.TryParse(hitDiceStr.Substring(2), out var hitDie))
            {
                characterClass.HitDie = hitDie;
            }
        }

        // Parse primary ability
        if (classData.TryGetProperty("primary_ability", out var primaryAbilityElement))
        {
            characterClass.PrimaryAbility = ParsePrimaryAbility(primaryAbilityElement);
        }

        // Parse saving throw proficiencies
        if (classData.TryGetProperty("saving_throw_proficiencies", out var savingThrowsElement))
        {
            characterClass.SavingThrowProficiencies = ParseStringArray(savingThrowsElement);
        }

        // Parse skill proficiencies
        if (classData.TryGetProperty("skill_proficiencies", out var skillProfElement))
        {
            characterClass.SkillChoices = ParseSkillProficiencies(skillProfElement);
            
            // For backward compatibility, also populate the old SkillProficiencies array
            if (characterClass.SkillChoices?.HasChoices == true)
            {
                characterClass.SkillProficiencies = new[] { $"Choose {characterClass.SkillChoices.ChooseCount} from: {string.Join(", ", characterClass.SkillChoices.AvailableSkills)}" };
            }
            else
            {
                characterClass.SkillProficiencies = ParseSkillProficienciesLegacy(skillProfElement);
            }
        }

        // Parse armor proficiencies
        if (classData.TryGetProperty("armor_proficiencies", out var armorProfElement))
        {
            characterClass.ArmorProficiencies = ParseStringArray(armorProfElement);
        }

        // Parse weapon proficiencies
        if (classData.TryGetProperty("weapon_proficiencies", out var weaponProfElement))
        {
            characterClass.WeaponProficiencies = ParseStringArray(weaponProfElement);
        }

        // Parse tool proficiencies
        if (classData.TryGetProperty("tool_proficiencies", out var toolProfElement))
        {
            characterClass.ToolProficiencies = ParseStringArray(toolProfElement);
        }

        // Parse starting equipment (simplified)
        if (classData.TryGetProperty("starting_equipment", out var equipmentElement))
        {
            characterClass.StartingEquipment = ParseStartingEquipment(equipmentElement);
        }

        // Parse standard array recommendation for 2024 PHB
        if (classData.TryGetProperty("standard_array_recommendation", out var standardArrayElement))
        {
            characterClass.StandardArrayRecommendation = ParseStandardArrayRecommendation(standardArrayElement);
        }

        return characterClass;
    }

    private string ParsePrimaryAbility(JsonElement primaryAbilityElement)
    {
        if (primaryAbilityElement.ValueKind == JsonValueKind.Array)
        {
            var abilities = new List<string>();
            foreach (var abilityElement in primaryAbilityElement.EnumerateArray())
            {
                var ability = abilityElement.GetString();
                if (!string.IsNullOrEmpty(ability))
                {
                    abilities.Add(CapitalizeName(ability));
                }
            }
            return string.Join(" and ", abilities);
        }
        else if (primaryAbilityElement.ValueKind == JsonValueKind.String)
        {
            return CapitalizeName(primaryAbilityElement.GetString() ?? "");
        }

        return "";
    }

    private string[] ParseStringArray(JsonElement arrayElement)
    {
        if (arrayElement.ValueKind != JsonValueKind.Array)
            return Array.Empty<string>();

        var result = new List<string>();
        foreach (var element in arrayElement.EnumerateArray())
        {
            var value = element.GetString();
            if (!string.IsNullOrEmpty(value))
            {
                result.Add(value);
            }
        }

        return result.ToArray();
    }

    private SkillChoiceOptions? ParseSkillProficiencies(JsonElement skillElement)
    {
        if (skillElement.ValueKind == JsonValueKind.Object)
        {
            // Handle "choose X from [list]" structure
            var skills = new List<string>();
            var chooseCount = 0;
            
            if (skillElement.TryGetProperty("choose", out var chooseElement))
            {
                chooseCount = chooseElement.GetInt32();
            }
            
            if (skillElement.TryGetProperty("from", out var fromElement) && fromElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var skill in fromElement.EnumerateArray())
                {
                    var skillName = skill.GetString();
                    if (!string.IsNullOrEmpty(skillName))
                    {
                        skills.Add(skillName);
                    }
                }
            }

            if (chooseCount > 0 && skills.Count > 0)
            {
                return new SkillChoiceOptions
                {
                    ChooseCount = chooseCount,
                    AvailableSkills = skills
                };
            }
        }
        else if (skillElement.ValueKind == JsonValueKind.Array)
        {
            // Handle direct skill list (no choices) - return null to indicate no choices needed
            return null;
        }

        return null;
    }

    private string[] ParseSkillProficienciesLegacy(JsonElement skillElement)
    {
        if (skillElement.ValueKind == JsonValueKind.Array)
        {
            return ParseStringArray(skillElement);
        }
        else if (skillElement.ValueKind == JsonValueKind.Object)
        {
            // Handle "choose X from [list]" structure - return empty array for legacy compatibility
            return Array.Empty<string>();
        }

        return Array.Empty<string>();
    }

    private string[] ParseStartingEquipment(JsonElement equipmentElement)
    {
        var equipment = new List<string>();

        if (equipmentElement.TryGetProperty("choices", out var choicesElement) && choicesElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var choice in choicesElement.EnumerateArray())
            {
                if (choice.TryGetProperty("from", out var fromElement) && fromElement.ValueKind == JsonValueKind.Array)
                {
                    var options = new List<string>();
                    foreach (var option in fromElement.EnumerateArray())
                    {
                        if (option.ValueKind == JsonValueKind.String)
                        {
                            options.Add(option.GetString() ?? "");
                        }
                        else if (option.ValueKind == JsonValueKind.Array)
                        {
                            var items = new List<string>();
                            foreach (var item in option.EnumerateArray())
                            {
                                items.Add(item.GetString() ?? "");
                            }
                            options.Add(string.Join(", ", items));
                        }
                    }

                    if (options.Count > 0)
                    {
                        equipment.Add($"Choose: {string.Join(" OR ", options)}");
                    }
                }
            }
        }

        return equipment.ToArray();
    }

    private Dictionary<string, int>? ParseStandardArrayRecommendation(JsonElement standardArrayElement)
    {
        if (standardArrayElement.ValueKind != JsonValueKind.Object)
            return null;

        var recommendation = new Dictionary<string, int>();

        foreach (var property in standardArrayElement.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Number)
            {
                recommendation[property.Name] = property.Value.GetInt32();
            }
        }

        return recommendation.Count > 0 ? recommendation : null;
    }

    private string CapitalizeName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        return char.ToUpper(name[0]) + name.Substring(1).ToLower();
    }
} 