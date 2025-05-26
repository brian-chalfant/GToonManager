using System.Collections.Generic;

namespace GToonManager.Services;

public static class DndCharacterSheetMapping
{
    // Based on the field discovery, we know the PDF has fields like:
    // text_1imkp, text_2qgox, text_3bfkv, etc.
    // This mapping is an educated guess that should be refined through testing
    
    public static Dictionary<string, string> GetFieldMapping()
    {
        return new Dictionary<string, string>
        {
            // Based on actual PDF field discovery report
            // Basic Character Information
            ["CharacterName"] = "text_1imkp",           // Character Name ✅
            ["Background"] = "text_2qgox",              // Background, Class ✅
            ["Race"] = "text_3bfkv",                    // Species, Subclass ✅
            ["ClassLevel"] = "text_4deth",              // Level (Total Level of all Classes) ✅
            ["ExperiencePoints"] = "text_5mocb",        // XP ✅
            
            // Ability Scores
            ["Strength"] = "text_23ewgq",               // Strength Score ✅
            ["Dexterity"] = "text_26ccgh",              // Dexterity Score ✅
            ["Constitution"] = "text_28owjh",           // Constitution Score ✅
            ["Intelligence"] = "text_24rqar",           // Intelligence Score ✅
            ["Wisdom"] = "text_25blbj",                 // Wisdom Score ✅
            ["Charisma"] = "text_27jhio",               // Charisma Score ✅
            
            // Ability Modifiers
            ["StrengthModifier"] = "text_17vpmg",       // Strength Modifier ✅
            ["DexterityModifier"] = "text_18ruyf",      // Dexterity Modifier ✅
            ["ConstitutionModifier"] = "text_21kabi",   // Constitution Modifier ✅
            ["IntelligenceModifier"] = "text_19lqwv",   // Intelligence Modifier ✅
            ["WisdomModifier"] = "text_20zbar",         // Wisdom Modifier ✅
            ["CharismaModifier"] = "text_22bxjy",       // Charisma Modifier ✅
            
            // Combat Stats
            ["ArmorClass"] = "text_6agjh",              // Armor Class ✅
            ["Initiative"] = "text_13wrft",             // Initiative ✅
            ["Speed"] = "text_14lvnq",                  // Speed ✅
            ["HitPointMaximum"] = "text_9efgi",         // Max Hit Points ✅
            ["CurrentHitPoints"] = "text_8bjml",        // Current Hit Points ✅
            ["TemporaryHitPoints"] = "text_10mexx",     // Temp Hit Points ✅
            ["HitDice"] = "text_12kvai",                // Hit Dice Max ✅
            ["ProficiencyBonus"] = "text_427aebp",      // Proficiency Bonus ✅
            
            // Additional Stats
            ["Size"] = "text_15cqja",                   // Size ✅
            ["PassivePerception"] = "text_16wgea",      // Passive Perception ✅
            ["Alignment"] = "text_275cexd",             // Alignment ✅
            
            // Saving Throws
            ["StrengthSave"] = "text_60zpuk",           // Strength Saving Throw ✅
            ["DexteritySave"] = "text_68bipd",          // Dexterity Saving Throw ✅
            ["ConstitutionSave"] = "text_72gnbq",       // Constitution Saving Throw ✅
            ["IntelligenceSave"] = "text_54bfgy",       // Intelligence Saving Throw ✅
            ["WisdomSave"] = "text_62yaan",             // Wisdom Saving Throw ✅
            ["CharismaSave"] = "text_73cwxt",           // Charisma Saving Throw ✅
            
            // Skills
            ["Acrobatics"] = "text_69srmm",             // Acrobatics Skill ✅
            ["AnimalHandling"] = "text_63uhiv",         // Animal Handling Skill ✅
            ["Arcana"] = "text_55nptn",                 // Arcana Skill ✅
            ["Athletics"] = "text_61knsn",              // Athletics Skill ✅
            ["Deception"] = "text_74rkfi",              // Deception Skill ✅
            ["History"] = "text_56ksru",                // History Skill ✅
            ["Insight"] = "text_64odvk",                // Insight Skill ✅
            ["Intimidation"] = "text_75pauh",           // Intimidation Skill ✅
            ["Investigation"] = "text_57bjob",          // Investigation Skill ✅
            ["Medicine"] = "text_65hnhb",               // Medicine Skill ✅
            ["Nature"] = "text_58zoel",                 // Nature Skill ✅
            ["Perception"] = "text_66djlf",             // Perception Skill ✅
            ["Performance"] = "text_76vfsc",            // Performance Skill ✅
            ["Persuasion"] = "text_77nads",             // Persuasion Skill ✅
            ["Religion"] = "text_59mfqs",               // Religion Skill ✅
            ["SleightOfHand"] = "text_70obrk",          // Sleight of Hand Skill ✅
            ["Stealth"] = "text_71pflk",                // Stealth Skill ✅
            ["Survival"] = "text_67cr",                 // Survival Skill ✅
            
            // Spellcasting
            ["SpellcastingModifier"] = "text_270axdp",  // Spellcasting Modifier ✅
            ["SpellSaveDC"] = "text_271dmmw",           // Spell Save DC ✅
            ["SpellAttackBonus"] = "text_272rsuk",      // Spell Attack Bonus ✅
            
            // Large Text Areas
            ["ClassFeatures"] = "textarea_93szqe",      // Class Features Column 1 ✅
            ["ClassFeatures2"] = "textarea_94ci",       // Class Features Column 2 ✅
            ["SpeciesTraits"] = "textarea_91jawi",      // Species(Race) Traits ✅
            ["Feats"] = "textarea_92aaks",              // Feats ✅
            ["WeaponsProficiencies"] = "textarea_89gsxf", // Weapons Proficiencies ✅
            ["ToolsProficiencies"] = "textarea_90mfnr", // Tool Proficiencies ✅
            ["Appearance"] = "textarea_273pluw",        // Appearance ✅
            ["BackstoryPersonality"] = "textarea_274seuz", // Backstory and Personality ✅
            ["Languages"] = "textarea_259fnwq",         // Languages ✅
            ["Equipment"] = "textarea_260nudr"          // Equipment ✅
        };
    }
    
    // Get field mapping with fallback for missing fields
    public static string? GetFieldName(string propertyName)
    {
        var mapping = GetFieldMapping();
        return mapping.TryGetValue(propertyName, out var fieldName) ? fieldName : null;
    }
    
    // Validate if a field exists in our mapping
    public static bool HasMapping(string propertyName)
    {
        return GetFieldMapping().ContainsKey(propertyName);
    }
} 