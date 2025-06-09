using GToonManager.Models;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using PdfSharp.Fonts;
using System.IO;

namespace GToonManager.Services;

public class PdfSharpExportService
{
    private readonly string _templatePath;

    public PdfSharpExportService()
    {
        _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "2024-character-sheet.pdf");
        
        // Initialize font resolver if not already set
        if (GlobalFontSettings.FontResolver == null)
        {
            GlobalFontSettings.FontResolver = new PdfSharpFontResolver();
        }
    }

    public Task<bool> ExportCharacterToPdfAsync(Character character, string outputPath)
    {
        try
        {
            if (!File.Exists(_templatePath))
            {
                System.Diagnostics.Debug.WriteLine($"Template PDF not found at: {_templatePath}");
                return Task.FromResult(false);
            }

            // Validate output path
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            System.Diagnostics.Debug.WriteLine($"Loading PDF template: {_templatePath}");
            
            // Open the PDF template
            using var document = PdfReader.Open(_templatePath, PdfDocumentOpenMode.Modify);
            
            System.Diagnostics.Debug.WriteLine($"PDF loaded successfully. Pages: {document.PageCount}");

            // Get the AcroForm (form fields)
            var form = document.AcroForm;
            if (form == null)
            {
                System.Diagnostics.Debug.WriteLine("No AcroForm found in PDF");
                return Task.FromResult(false);
            }

            System.Diagnostics.Debug.WriteLine($"Found AcroForm with {form.Fields.Count} fields");

            // Get field mappings
            var fieldMappings = DndCharacterSheetMapping.GetFieldMapping();
            
            int fieldsFound = 0;
            int fieldsAttempted = 0;

            // Fill the form fields
            foreach (var mapping in fieldMappings)
            {
                fieldsAttempted++;
                var value = GetCharacterValue(character, mapping.Key);
                
                if (TryFillField(form, mapping.Value, value))
                {
                    fieldsFound++;
                    System.Diagnostics.Debug.WriteLine($"Successfully filled field '{mapping.Value}' with value '{value}'");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Successfully filled {fieldsFound} out of {fieldsAttempted} attempted fields");

            // Set NeedAppearances to make text visible
            if (form.Elements.ContainsKey("/NeedAppearances"))
            {
                form.Elements["/NeedAppearances"] = new PdfSharp.Pdf.PdfBoolean(true);
            }
            else
            {
                form.Elements.Add("/NeedAppearances", new PdfSharp.Pdf.PdfBoolean(true));
            }

            // Save the document
            document.Save(outputPath);
            System.Diagnostics.Debug.WriteLine($"PDF saved successfully to: {outputPath}");

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error exporting PDF with PdfSharp: {ex.GetType().Name}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return Task.FromResult(false);
        }
    }

    private bool TryFillField(PdfAcroForm form, string fieldName, string value)
    {
        if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(value))
        {
            return false;
        }

        try
        {
            var field = form.Fields[fieldName];
            if (field != null)
            {
                if (field is PdfTextField textField)
                {
                    textField.Value = new PdfString(value);
                    return true;
                }
                else if (field is PdfCheckBoxField checkBox)
                {
                    // Handle checkbox fields - try multiple approaches
                    var isChecked = value.ToLower() == "true" || value == "1";
                    
                    try
                    {
                        // Based on analysis: /DV: /Off means unchecked value is the PDF name /Off
                        // We need to use PdfName instead of PdfString for checkbox values
                        
                        if (isChecked)
                        {
                            // Try the most common checked values as PDF names
                            // Based on our analysis, we'll try /Yes first, then /On
                            
                            // Approach 1: Try /Yes as PDF Name (most common)
                            var yesName = new PdfSharp.Pdf.PdfName("/Yes");
                            checkBox.Value = yesName;
                            if (checkBox.Elements.ContainsKey("/V"))
                            {
                                checkBox.Elements["/V"] = yesName;
                            }
                            if (checkBox.Elements.ContainsKey("/AS"))
                            {
                                checkBox.Elements["/AS"] = yesName;
                            }
                            else
                            {
                                checkBox.Elements.Add("/AS", yesName);
                            }
                        }
                        else
                        {
                            // Unchecked state: use /Off as PDF Name (from analysis)
                            var offName = new PdfSharp.Pdf.PdfName("/Off");
                            checkBox.Value = offName;
                            if (checkBox.Elements.ContainsKey("/V"))
                            {
                                checkBox.Elements["/V"] = offName;
                            }
                            if (checkBox.Elements.ContainsKey("/AS"))
                            {
                                checkBox.Elements["/AS"] = offName;
                            }
                            else
                            {
                                checkBox.Elements.Add("/AS", offName);
                            }
                        }
                        
                        // Set the Checked property after setting the values
                        checkBox.Checked = isChecked;
                        
                        // Debug: Show what we set for key checkboxes
                        // if (fieldName == "checkbox_249voxf" || fieldName == "checkbox_128cefr" || fieldName == "checkbox_126dqaq")
                        // {
                        //     System.Diagnostics.Debug.WriteLine($"=== AFTER SETTING {fieldName} ===");
                        //     System.Diagnostics.Debug.WriteLine($"Checked property: {checkBox.Checked}");
                        //     System.Diagnostics.Debug.WriteLine($"Value: {checkBox.Value}");
                        //     if (checkBox.Elements.ContainsKey("/V"))
                        //     {
                        //         System.Diagnostics.Debug.WriteLine($"Element /V: {checkBox.Elements["/V"]}");
                        //     }
                        //     if (checkBox.Elements.ContainsKey("/AS"))
                        //     {
                        //         System.Diagnostics.Debug.WriteLine($"Element /AS: {checkBox.Elements["/AS"]}");
                        //     }
                        //     System.Diagnostics.Debug.WriteLine($"=====================================");
                        // }
                    }
                    catch (Exception checkboxEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error setting checkbox {fieldName}: {checkboxEx.Message}");
                        // Final fallback: try just setting as text field
                        try
                        {
                            field.Value = new PdfString(isChecked ? "Yes" : "Off");
                        }
                        catch
                        {
                            // If all else fails, ignore this field
                        }
                    }
                    
                    return true;
                }
                else
                {
                    // Try generic field value setting
                    field.Value = new PdfString(value);
                    return true;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Field not found: {fieldName}");
                return false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting field '{fieldName}' to '{value}': {ex.Message}");
            return false;
        }
    }

    public List<string> GetPdfFieldNames()
    {
        var fieldNames = new List<string>();
        
        try
        {
            if (!File.Exists(_templatePath))
            {
                return fieldNames;
            }

            using var document = PdfReader.Open(_templatePath, PdfDocumentOpenMode.ReadOnly);
            var form = document.AcroForm;
            
            if (form != null)
            {
                var names = form.Fields.Names;
                foreach (var name in names)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        fieldNames.Add(name);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error reading PDF fields with PdfSharp: {ex.Message}");
        }

        return fieldNames;
    }

    public void AnalyzeCheckboxFields(PdfDocument document)
    {
        // Analyze checkbox fields to understand their structure
        // AnalyzeCheckboxFields(document);
    }

    private string GetCharacterValue(Character character, string propertyName)
    {
        return propertyName switch
        {
            // Basic Character Info
            "CharacterName" => character.Name,
            "PlayerName" => character.PlayerName,
            
            // NEW: Separate fields for the new PDF format
            "Race" => character.Race?.Name ?? "",
            "Subclass" => GetSubclassText(character),
            "Background" => character.Background?.Name ?? "",
            "Class" => GetClassText(character),
            
            "ClassLevel" => character.Level.ToString(),
            "ExperiencePoints" => character.ExperiencePoints.ToString(),
            
            // Ability Scores
            "Strength" => character.StrengthTotal?.ToString() ?? "",
            "Dexterity" => character.DexterityTotal?.ToString() ?? "",
            "Constitution" => character.ConstitutionTotal?.ToString() ?? "",
            "Intelligence" => character.IntelligenceTotal?.ToString() ?? "",
            "Wisdom" => character.WisdomTotal?.ToString() ?? "",
            "Charisma" => character.CharismaTotal?.ToString() ?? "",
            
            // Ability Modifiers (calculated from total scores)
            "StrengthModifier" => character.StrengthModifier.HasValue ? FormatModifier(character.StrengthModifier.Value) : "",
            "DexterityModifier" => character.DexterityModifier.HasValue ? FormatModifier(character.DexterityModifier.Value) : "",
            "ConstitutionModifier" => character.ConstitutionModifier.HasValue ? FormatModifier(character.ConstitutionModifier.Value) : "",
            "IntelligenceModifier" => character.IntelligenceModifier.HasValue ? FormatModifier(character.IntelligenceModifier.Value) : "",
            "WisdomModifier" => character.WisdomModifier.HasValue ? FormatModifier(character.WisdomModifier.Value) : "",
            "CharismaModifier" => character.CharismaModifier.HasValue ? FormatModifier(character.CharismaModifier.Value) : "",
            
            // Combat Stats
            "ArmorClass" => character.ArmorClass.ToString(),
            "Initiative" => character.DexterityModifier.HasValue ? FormatModifier(character.DexterityModifier.Value) : "",
            "Speed" => character.Speed.ToString(),
            "HitPointMaximum" => character.MaxHitPoints.ToString(),
            "CurrentHitPoints" => character.HitPoints.ToString(),
            "TemporaryHitPoints" => "0",
            "HitDice" => $"1d{character.ClassLevels.FirstOrDefault()?.CharacterClass?.HitDie ?? 8}",
            "ProficiencyBonus" => FormatModifier(character.ProficiencyBonus),
            
            // Saving Throws
            "StrengthSave" => FormatModifier(GetSavingThrowModifier(character, "strength")),
            "DexteritySave" => FormatModifier(GetSavingThrowModifier(character, "dexterity")),
            "ConstitutionSave" => FormatModifier(GetSavingThrowModifier(character, "constitution")),
            "IntelligenceSave" => FormatModifier(GetSavingThrowModifier(character, "intelligence")),
            "WisdomSave" => FormatModifier(GetSavingThrowModifier(character, "wisdom")),
            "CharismaSave" => FormatModifier(GetSavingThrowModifier(character, "charisma")),
            
            // Saving Throw Proficiencies (checkboxes)
            "StrengthSaveProficiency" => HasSavingThrowProficiency(character, "strength") ? "true" : "false",
            "DexteritySaveProficiency" => HasSavingThrowProficiency(character, "dexterity") ? "true" : "false",
            "ConstitutionSaveProficiency" => HasSavingThrowProficiency(character, "constitution") ? "true" : "false",
            "IntelligenceSaveProficiency" => HasSavingThrowProficiency(character, "intelligence") ? "true" : "false",
            "WisdomSaveProficiency" => HasSavingThrowProficiency(character, "wisdom") ? "true" : "false",
            "CharismaSaveProficiency" => HasSavingThrowProficiency(character, "charisma") ? "true" : "false",
            
            // Skills
            "Acrobatics" => FormatModifier(GetSkillModifier(character, "acrobatics")),
            "AnimalHandling" => FormatModifier(GetSkillModifier(character, "animal handling")),
            "Arcana" => FormatModifier(GetSkillModifier(character, "arcana")),
            "Athletics" => FormatModifier(GetSkillModifier(character, "athletics")),
            "Deception" => FormatModifier(GetSkillModifier(character, "deception")),
            "History" => FormatModifier(GetSkillModifier(character, "history")),
            "Insight" => FormatModifier(GetSkillModifier(character, "insight")),
            "Intimidation" => FormatModifier(GetSkillModifier(character, "intimidation")),
            "Investigation" => FormatModifier(GetSkillModifier(character, "investigation")),
            "Medicine" => FormatModifier(GetSkillModifier(character, "medicine")),
            "Nature" => FormatModifier(GetSkillModifier(character, "nature")),
            "Perception" => FormatModifier(GetSkillModifier(character, "perception")),
            "Performance" => FormatModifier(GetSkillModifier(character, "performance")),
            "Persuasion" => FormatModifier(GetSkillModifier(character, "persuasion")),
            "Religion" => FormatModifier(GetSkillModifier(character, "religion")),
            "SleightOfHand" => FormatModifier(GetSkillModifier(character, "sleight of hand")),
            "Stealth" => FormatModifier(GetSkillModifier(character, "stealth")),
            "Survival" => FormatModifier(GetSkillModifier(character, "survival")),
            
            // Skill Proficiencies (checkboxes)
            "AcrobaticsProficiency" => HasSkillProficiency(character, "acrobatics") ? "true" : "false",
            "AnimalHandlingProficiency" => HasSkillProficiency(character, "animal handling") ? "true" : "false",
            "ArcanaProficiency" => HasSkillProficiency(character, "arcana") ? "true" : "false",
            "AthleticsProficiency" => HasSkillProficiency(character, "athletics") ? "true" : "false",
            "DeceptionProficiency" => HasSkillProficiency(character, "deception") ? "true" : "false",
            "HistoryProficiency" => HasSkillProficiency(character, "history") ? "true" : "false",
            "InsightProficiency" => HasSkillProficiency(character, "insight") ? "true" : "false",
            "IntimidationProficiency" => HasSkillProficiency(character, "intimidation") ? "true" : "false",
            "InvestigationProficiency" => HasSkillProficiency(character, "investigation") ? "true" : "false",
            "MedicineProficiency" => HasSkillProficiency(character, "medicine") ? "true" : "false",
            "NatureProficiency" => HasSkillProficiency(character, "nature") ? "true" : "false",
            "PerceptionProficiency" => HasSkillProficiency(character, "perception") ? "true" : "false",
            "PerformanceProficiency" => HasSkillProficiency(character, "performance") ? "true" : "false",
            "PersuasionProficiency" => HasSkillProficiency(character, "persuasion") ? "true" : "false",
            "ReligionProficiency" => HasSkillProficiency(character, "religion") ? "true" : "false",
            "SleightOfHandProficiency" => HasSkillProficiency(character, "sleight of hand") ? "true" : "false",
            "StealthProficiency" => HasSkillProficiency(character, "stealth") ? "true" : "false",
            "SurvivalProficiency" => HasSkillProficiency(character, "survival") ? "true" : "false",
            
            // Armor Proficiencies (checkboxes)
            "LightArmorProficiency" => HasArmorProficiency(character, "light armor") ? "true" : "false",
            "MediumArmorProficiency" => HasArmorProficiency(character, "medium armor") ? "true" : "false",
            "HeavyArmorProficiency" => HasArmorProficiency(character, "heavy armor") ? "true" : "false",
            "ShieldProficiency" => HasArmorProficiency(character, "shields") ? "true" : "false",
            
            // Additional Stats
            "Size" => "Medium", // Default size
            "PassivePerception" => (10 + GetSkillModifier(character, "perception")).ToString(),
            "Alignment" => "",
            
            // Spellcasting (if applicable)
            "SpellcastingModifier" => "",
            "SpellSaveDC" => "",
            "SpellAttackBonus" => "",
            
            // Large Text Areas
            //TODO: Add class features to the character sheet
            "ClassFeatures" => GetClassFeatures(character),
            "ClassFeatures2" => "", // Additional class features if needed
            "SpeciesTraits" => GetRaceTraits(character),
            "Feats" => "",
            "WeaponsProficiencies" => "",
            "ToolsProficiencies" => "",
            "Appearance" => "",
            "BackstoryPersonality" => "",
            "Languages" => "",
            "Equipment" => "",
            
            _ => ""
        };
    }

    private static string FormatModifier(int modifier)
    {
        return modifier >= 0 ? $"+{modifier}" : modifier.ToString();
    }

    private static string GetClassFeatures(Character character)
    {
        if (character.ClassLevels.Count == 0)
            return "";
        
        var features = new List<string>();
        foreach (var classLevel in character.ClassLevels)
        {
            features.Add($"{classLevel.ClassName} (Level {classLevel.Level})");
            if (!string.IsNullOrEmpty(classLevel.CharacterClass?.Description))
            {
                features.Add($"- {classLevel.CharacterClass.Description}");
            }
        }
        return string.Join("\n", features);
    }

    private static string GetRaceTraits(Character character)
    {
        if (character.Race == null)
            return "";
        
        var traits = new List<string> { character.Race.Name };
        if (!string.IsNullOrEmpty(character.Race.Description))
        {
            traits.Add($"- {character.Race.Description}");
        }
        return string.Join("\n", traits);
    }

    private static string GetBackgroundClassText(Character character)
    {
        var parts = new List<string>();
        
        // Add background if available
        if (character.Background != null && !string.IsNullOrEmpty(character.Background.Name))
        {
            parts.Add(character.Background.Name);
        }
        
        // Add class(es) if available
        if (character.ClassLevels.Count > 0)
        {
            var classNames = character.ClassLevels.Select(cl => cl.ClassName).ToList();
            parts.Add(string.Join("/", classNames));
        }
        
        return string.Join(", ", parts);
    }

    private static string GetRaceSubclassText(Character character)
    {
        var parts = new List<string>();
        
        // Add race if available
        if (character.Race != null && !string.IsNullOrEmpty(character.Race.Name))
        {
            parts.Add(character.Race.Name);
        }
        
        // Add subrace if available
        if (character.Subrace != null && !string.IsNullOrEmpty(character.Subrace.Name))
        {
            parts.Add(character.Subrace.Name);
        }
        
        return string.Join(", ", parts);
    }

    private static string GetClassText(Character character)
    {
        if (character.ClassLevels.Count == 0)
            return "";
        
        var classNames = character.ClassLevels.Select(cl => cl.ClassName).ToList();
        return string.Join("/", classNames);
    }

    private static string GetSubclassText(Character character)
    {
        if (character.Subrace != null && !string.IsNullOrEmpty(character.Subrace.Name))
        {
            return character.Subrace.Name;
        }
        
        return "";
    }

    private static bool HasSavingThrowProficiency(Character character, string ability)
    {
        return character.ClassLevels.Any(cl => 
            cl.CharacterClass?.SavingThrowProficiencies?.Contains(ability, StringComparer.OrdinalIgnoreCase) == true);
    }

    private static bool HasSkillProficiency(Character character, string skill)
    {
        // Check background proficiencies first
        var backgroundSkills = character.Background?.ProficiencyGrants?.Skills ?? new List<string>();
        var hasBackgroundProficiency = backgroundSkills.Any(s => 
            string.Equals(s, skill, StringComparison.OrdinalIgnoreCase));
        
        // Check racial skill proficiencies
        var hasRacialProficiency = character.RacialSkillProficiencies
            .Any(s => string.Equals(s, skill, StringComparison.OrdinalIgnoreCase));
        
        // Check chosen class skill proficiencies
        var hasClassProficiency = character.ClassLevels
            .SelectMany(cl => cl.ChosenSkillProficiencies)
            .Any(s => string.Equals(s, skill, StringComparison.OrdinalIgnoreCase));
        
        return hasBackgroundProficiency || hasRacialProficiency || hasClassProficiency;
    }

    private static bool HasArmorProficiency(Character character, string armorType)
    {
        return character.ClassLevels.Any(cl => 
            cl.CharacterClass?.ArmorProficiencies?.Contains(armorType, StringComparer.OrdinalIgnoreCase) == true ||
            cl.CharacterClass?.ArmorProficiencies?.Contains("all armor", StringComparer.OrdinalIgnoreCase) == true);
    }

    private static int GetSavingThrowModifier(Character character, string ability)
    {
        var baseModifier = ability.ToLower() switch
        {
            "strength" => character.StrengthModifier ?? 0,
            "dexterity" => character.DexterityModifier ?? 0,
            "constitution" => character.ConstitutionModifier ?? 0,
            "intelligence" => character.IntelligenceModifier ?? 0,
            "wisdom" => character.WisdomModifier ?? 0,
            "charisma" => character.CharismaModifier ?? 0,
            _ => 0
        };

        var proficiencyBonus = HasSavingThrowProficiency(character, ability) ? character.ProficiencyBonus : 0;
        return baseModifier + proficiencyBonus;
    }

    private static int GetSkillModifier(Character character, string skill)
    {
        // Map skills to their associated abilities
        var baseModifier = skill.ToLower() switch
        {
            "acrobatics" => character.DexterityModifier ?? 0,
            "animal handling" => character.WisdomModifier ?? 0,
            "arcana" => character.IntelligenceModifier ?? 0,
            "athletics" => character.StrengthModifier ?? 0,
            "deception" => character.CharismaModifier ?? 0,
            "history" => character.IntelligenceModifier ?? 0,
            "insight" => character.WisdomModifier ?? 0,
            "intimidation" => character.CharismaModifier ?? 0,
            "investigation" => character.IntelligenceModifier ?? 0,
            "medicine" => character.WisdomModifier ?? 0,
            "nature" => character.IntelligenceModifier ?? 0,
            "perception" => character.WisdomModifier ?? 0,
            "performance" => character.CharismaModifier ?? 0,
            "persuasion" => character.CharismaModifier ?? 0,
            "religion" => character.IntelligenceModifier ?? 0,
            "sleight of hand" => character.DexterityModifier ?? 0,
            "stealth" => character.DexterityModifier ?? 0,
            "survival" => character.WisdomModifier ?? 0,
            _ => 0
        };

        var proficiencyBonus = HasSkillProficiency(character, skill) ? character.ProficiencyBonus : 0;
        return baseModifier + proficiencyBonus;
    }

    private static int CalculateModifier(int score)
    {
        return (score - 10) / 2;
    }
} 