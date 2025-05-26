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
        _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "2024-dnd-character-sheet-fillable.pdf");
        
        // Initialize font resolver if not already set
        if (GlobalFontSettings.FontResolver == null)
        {
            GlobalFontSettings.FontResolver = new PdfSharpFontResolver();
        }
    }

    public async Task<bool> ExportCharacterToPdfAsync(Character character, string outputPath)
    {
        try
        {
            if (!File.Exists(_templatePath))
            {
                System.Diagnostics.Debug.WriteLine($"Template PDF not found at: {_templatePath}");
                return false;
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
                return false;
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

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error exporting PDF with PdfSharp: {ex.GetType().Name}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
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
                    // Handle checkbox fields if needed
                    checkBox.Checked = value.ToLower() == "true" || value == "1";
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

    private string GetCharacterValue(Character character, string propertyName)
    {
        return propertyName switch
        {
            // Basic Character Info
            "CharacterName" => character.Name,
            "PlayerName" => character.PlayerName,
            "Race" => character.Race?.Name ?? "",
            "Background" => character.Background?.Name ?? "",
            "ClassLevel" => character.ClassSummary,
            "ExperiencePoints" => character.ExperiencePoints.ToString(),
            
            // Ability Scores
            "Strength" => character.AbilityScores.Strength.ToString(),
            "Dexterity" => character.AbilityScores.Dexterity.ToString(),
            "Constitution" => character.AbilityScores.Constitution.ToString(),
            "Intelligence" => character.AbilityScores.Intelligence.ToString(),
            "Wisdom" => character.AbilityScores.Wisdom.ToString(),
            "Charisma" => character.AbilityScores.Charisma.ToString(),
            
            // Ability Modifiers
            "StrengthModifier" => FormatModifier(character.StrengthModifier),
            "DexterityModifier" => FormatModifier(character.DexterityModifier),
            "ConstitutionModifier" => FormatModifier(character.ConstitutionModifier),
            "IntelligenceModifier" => FormatModifier(character.IntelligenceModifier),
            "WisdomModifier" => FormatModifier(character.WisdomModifier),
            "CharismaModifier" => FormatModifier(character.CharismaModifier),
            
            // Combat Stats
            "ArmorClass" => character.ArmorClass.ToString(),
            "Initiative" => FormatModifier(character.DexterityModifier),
            "Speed" => character.Speed.ToString(),
            "HitPointMaximum" => character.MaxHitPoints.ToString(),
            "CurrentHitPoints" => character.HitPoints.ToString(),
            "TemporaryHitPoints" => "0",
            "HitDice" => $"1d{character.ClassLevels.FirstOrDefault()?.CharacterClass?.HitDie ?? 8}",
            "ProficiencyBonus" => FormatModifier(character.ProficiencyBonus),
            
            // Saving Throws
            "StrengthSave" => FormatModifier(character.StrengthModifier),
            "DexteritySave" => FormatModifier(character.DexterityModifier),
            "ConstitutionSave" => FormatModifier(character.ConstitutionModifier),
            "IntelligenceSave" => FormatModifier(character.IntelligenceModifier),
            "WisdomSave" => FormatModifier(character.WisdomModifier),
            "CharismaSave" => FormatModifier(character.CharismaModifier),
            
            // Skills
            "Acrobatics" => FormatModifier(character.DexterityModifier),
            "AnimalHandling" => FormatModifier(character.WisdomModifier),
            "Arcana" => FormatModifier(character.IntelligenceModifier),
            "Athletics" => FormatModifier(character.StrengthModifier),
            "Deception" => FormatModifier(character.CharismaModifier),
            "History" => FormatModifier(character.IntelligenceModifier),
            "Insight" => FormatModifier(character.WisdomModifier),
            "Intimidation" => FormatModifier(character.CharismaModifier),
            "Investigation" => FormatModifier(character.IntelligenceModifier),
            "Medicine" => FormatModifier(character.WisdomModifier),
            "Nature" => FormatModifier(character.IntelligenceModifier),
            "Perception" => FormatModifier(character.WisdomModifier),
            "Performance" => FormatModifier(character.CharismaModifier),
            "Persuasion" => FormatModifier(character.CharismaModifier),
            "Religion" => FormatModifier(character.IntelligenceModifier),
            "SleightOfHand" => FormatModifier(character.DexterityModifier),
            "Stealth" => FormatModifier(character.DexterityModifier),
            "Survival" => FormatModifier(character.WisdomModifier),
            
            // Additional Stats
            "Size" => "Medium", // Default size
            "PassivePerception" => (10 + character.WisdomModifier).ToString(),
            "Alignment" => "",
            
            // Spellcasting (if applicable)
            "SpellcastingModifier" => "",
            "SpellSaveDC" => "",
            "SpellAttackBonus" => "",
            
            // Large Text Areas
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
} 