using System.Collections.Generic;

namespace GToonManager.Services;

public static class DndCharacterSheetMapping
{
    // OLD MAPPINGS FOR 2024-dnd-character-sheet-fillable.pdf - COMMENTED OUT
    /*
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
            ["ArmorClassWithShield"] = "text_7lehl",    // Armor Class with Shield ✅
            ["Initiative"] = "text_13wrft",             // Initiative ✅
            ["Speed"] = "text_14lvnq",                  // Speed ✅
            ["HitPointMaximum"] = "text_9efgi",         // Max Hit Points ✅
            ["CurrentHitPoints"] = "text_8bjml",        // Current Hit Points ✅
            ["TemporaryHitPoints"] = "text_10mexx",     // Temp Hit Points ✅
            ["HitDice"] = "text_12kvai",                // Hit Dice Max ✅
            ["HitDiceSpent"] = "text_11dcsb",           // Hit Dice Spent ✅
            ["ProficiencyBonus"] = "text_427aebp",      // Proficiency Bonus ✅
            
            // Additional Stats
            ["Size"] = "text_15cqja",                   // Size ✅
            ["PassivePerception"] = "text_16wgea",      // Passive Perception ✅
            ["Alignment"] = "text_275cexd",             // Alignment ✅
            
            // ... rest of old mappings commented out for brevity ...
        };
    }
    */
    
    // NEW MAPPINGS FOR 2024-character-sheet.pdf
    public static Dictionary<string, string> GetFieldMapping()
    {
        return new Dictionary<string, string>
        {
            // Basic Character Information
            ["CharacterName"] = "Text1",               // Character Name
            ["Background"] = "Text6",                  // Background
            ["ClassLevel"] = "Text11",                 // Character Level
            ["ExperiencePoints"] = "Text12",           // XP
            ["Class"] = "Text7",                       // Character Class
            ["Race"] = "Text8",                        // Species
            ["Subclass"] = "Text9",                    // Subclass
            ["Alignment"] = "Text100",                 // Alignment
            
            // Ability Scores
            ["Strength"] = "Text64",                   // Strength Score
            ["Dexterity"] = "Text66",                  // Dexterity Score
            ["Constitution"] = "Text67",               // Constitution Score
            ["Intelligence"] = "Text63",               // Intelligence Score
            ["Wisdom"] = "Text65",                     // Wisdom Score
            ["Charisma"] = "Text68",                   // Charisma Score
            
            // Ability Modifiers
            ["StrengthModifier"] = "Text21",           // Strength Modifier
            ["DexterityModifier"] = "Text22",          // Dexterity Modifier
            ["ConstitutionModifier"] = "Text24",       // Constitution Modifier
            ["IntelligenceModifier"] = "Text20",       // Intelligence Modifier
            ["WisdomModifier"] = "Text23",             // Wisdom Modifier
            ["CharismaModifier"] = "Text25",           // Charisma Modifier
            
            // Combat Stats
            ["ArmorClass"] = "Text13",                 // Armor Class
            ["CurrentHitPoints"] = "Text14",           // Current Hit Points
            ["TemporaryHitPoints"] = "Text15",         // Temp Hit Points
            ["HitPointMaximum"] = "Text16",            // Max Hit Points
            ["HitDice"] = "Text17",                    // Max Hit Dice
            ["HitDiceSpent"] = "Text18",               // Spent Hit Dice
            ["ProficiencyBonus"] = "Text19",           // Proficiency Bonus
            ["Initiative"] = "Text26",                 // Initiative
            ["Speed"] = "Text27",                      // Speed
            ["Size"] = "Text28",                       // Size
            ["PassivePerception"] = "Text29",          // Passive Perception
            
            // Saving Throws
            ["StrengthSave"] = "Text91",               // Strength Saving Throw Modifier
            ["DexteritySave"] = "Text87",              // Dexterity Saving Throw Modifier
            ["ConstitutionSave"] = "Text86",           // Constitution Saving Throw Modifier
            ["IntelligenceSave"] = "Text4",            // Intelligence Saving Throw Modifier (Check Box4 indicates proficiency)
            ["WisdomSave"] = "Text75",                 // Wisdom Saving Throw Modifier
            ["CharismaSave"] = "Text69",               // Charisma Saving Throw Modifier
            
            // Saving Throw Proficiencies
            ["StrengthSaveProficiency"] = "Check Box37",       // Strength Saving Throw Proficiency
            ["DexteritySaveProficiency"] = "Check Box33",      // Dexterity Saving Throw Proficiency
            ["ConstitutionSaveProficiency"] = "Check Box32",   // Constitution Saving Throw Proficiency
            ["IntelligenceSaveProficiency"] = "Check Box4",    // Intelligence Saving Throw Proficiency
            ["WisdomSaveProficiency"] = "Check Box21",         // Wisdom Saving Throw Proficiency
            ["CharismaSaveProficiency"] = "Check Box26",       // Charisma Saving Throw Proficiency
            
            // Skills
            ["Acrobatics"] = "Text88",                 // Acrobatics Skill Modifier
            ["AnimalHandling"] = "Text76",             // Animal Handling Skill Modifier
            ["Arcana"] = "Text70",                     // Arcana Skill Modifier
            ["Athletics"] = "Text92",                  // Athletics Skill Modifier
            ["Deception"] = "Text82",                  // Deception Skill Modifier
            ["History"] = "Text71",                    // History Skill Modifier
            ["Insight"] = "Text77",                    // Insight Skill Modifier
            ["Intimidation"] = "Text83",               // Intimidation Skill Modifier
            ["Investigation"] = "Text72",              // Investigation Skill Modifier
            ["Medicine"] = "Text78",                   // Medicine Skill Modifier
            ["Nature"] = "Text73",                     // Nature Skill Modifier
            ["Perception"] = "Text79",                 // Perception Skill Modifier
            ["Performance"] = "Text84",                // Performance Skill Modifier
            ["Persuasion"] = "Text85",                 // Persuasion Skill Modifier
            ["Religion"] = "Text74",                   // Religion Skill Modifier
            ["SleightOfHand"] = "Text89",              // Sleight of Hand Skill Modifier
            ["Stealth"] = "Text90",                    // Stealth Skill Modifier
            ["Survival"] = "Text80",                   // Survival Skill Modifier
            
            // Skill Proficiencies
            ["AcrobaticsProficiency"] = "Check Box34",         // Acrobatics Skill Proficiency
            ["AnimalHandlingProficiency"] = "Check Box22",     // Animal Handling Skill Proficiency
            ["ArcanaProficiency"] = "Check Box16",             // Arcana Skill Proficiency
            ["AthleticsProficiency"] = "Check Box38",          // Athletics Skill Proficiency
            ["DeceptionProficiency"] = "Check Box27",          // Deception Skill Proficiency
            ["HistoryProficiency"] = "Check Box17",            // History Skill Proficiency
            ["InsightProficiency"] = "Check Box23",            // Insight Skill Proficiency
            ["IntimidationProficiency"] = "Check Box28",       // Intimidation Skill Proficiency
            ["InvestigationProficiency"] = "Check Box19",      // Investigation Skill Proficiency
            ["MedicineProficiency"] = "Check Box25",           // Medicine Skill Proficiency
            ["NatureProficiency"] = "Check Box20",             // Nature Skill Proficiency
            ["PerceptionProficiency"] = "Check Box31",         // Perception Skill Proficiency
            ["PerformanceProficiency"] = "Check Box30",        // Performance Skill Proficiency
            ["PersuasionProficiency"] = "Check Box29",         // Persuasion Skill Proficiency
            ["ReligionProficiency"] = "Check Box18",           // Religion Skill Proficiency
            ["SleightOfHandProficiency"] = "Check Box35",      // Sleight of Hand Skill Proficiency
            ["StealthProficiency"] = "Check Box36",            // Stealth Skill Proficiency
            ["SurvivalProficiency"] = "Check Box24",           // Survival Skill Proficiency
            
            // Armor Proficiencies and Equipment
            ["LightArmorProficiency"] = "Check Box13",         // Light Armor Proficiency
            ["MediumArmorProficiency"] = "Check Box14",        // Medium Armor Proficiency
            ["HeavyArmorProficiency"] = "Check Box15",         // Heavy Armor Proficiency
            ["ShieldProficiency"] = "Check Box12",             // Shield Proficiency
            ["ShieldEquipped"] = "Check Box3",                 // Shield Equipped
            
            // Death Saves
            ["DeathSaveSuccess1"] = "Check Box5",              // Death Save Success 1
            ["DeathSaveSuccess2"] = "Check Box6",              // Death Save Success 2
            ["DeathSaveSuccess3"] = "Check Box7",              // Death Save Success 3
            ["DeathSaveFailure1"] = "Check Box8",              // Death Save Failure 1
            ["DeathSaveFailure2"] = "Check Box9",              // Death Save Failure 2
            ["DeathSaveFailure3"] = "Check Box10",             // Death Save Failure 3
            
            // Heroic Inspiration
            ["HeroicInspiration"] = "Check Box11",             // Heroic Inspiration
            
            // Magic Item Attunement
            ["MagicItemAttunement1"] = "Check Box249",         // Magic Item Attunement 1
            ["MagicItemAttunement2"] = "Check Box250",         // Magic Item Attunement 2
            ["MagicItemAttunement3"] = "Check Box251",         // Magic Item Attunement 3
            ["MagicItemAttunementText1"] = "Text101",          // Magic Item Attunement Text 1
            ["MagicItemAttunementText2"] = "Text102",          // Magic Item Attunement Text 2
            ["MagicItemAttunementText3"] = "Text103",          // Magic Item Attunement Text 3
            
            // Currency
            ["CopperPieces"] = "Text226",                      // Copper Pieces
            ["SilverPieces"] = "Text267",                      // Silver Pieces
            ["EmeraldPieces"] = "Text268",                     // Emerald Pieces
            ["GoldPieces"] = "Text269",                        // Gold Pieces
            ["PlatinumPieces"] = "Text270",                    // Platinum Pieces
            
            // Weapons and Damage Cantrips
            ["WeaponCantripName1"] = "Text30",                 // Weapons and Damage Cantrips Name Row 1
            ["WeaponCantripName2"] = "Text34",                 // Weapons and Damage Cantrips Name Row 2
            ["WeaponCantripName3"] = "Text38",                 // Weapons and Damage Cantrips Name Row 3
            ["WeaponCantripName4"] = "Text42",                 // Weapons and Damage Cantrips Name Row 4
            ["WeaponCantripName5"] = "Text46",                 // Weapons and Damage Cantrips Name Row 5
            ["WeaponCantripName6"] = "Text50",                 // Weapons and Damage Cantrips Name Row 6
            ["WeaponCantripAttack1"] = "Text31",               // Weapons and Damage Cantrips Atk Bonus/DC Row 1
            ["WeaponCantripAttack2"] = "Text35",               // Weapons and Damage Cantrips Atk Bonus/DC Row 2
            ["WeaponCantripAttack3"] = "Text39",               // Weapons and Damage Cantrips Atk Bonus/DC Row 3
            ["WeaponCantripAttack4"] = "Text43",               // Weapons and Damage Cantrips Atk Bonus/DC Row 4
            ["WeaponCantripAttack5"] = "Text47",               // Weapons and Damage Cantrips Atk Bonus/DC Row 5
            ["WeaponCantripAttack6"] = "Text51",               // Weapons and Damage Cantrips Atk Bonus/DC Row 6
            ["WeaponCantripDamage1"] = "Text32",               // Weapons and Damage Cantrips Damage/Type Row 1
            ["WeaponCantripDamage2"] = "Text36",               // Weapons and Damage Cantrips Damage/Type Row 2
            ["WeaponCantripDamage3"] = "Text40",               // Weapons and Damage Cantrips Damage/Type Row 3
            ["WeaponCantripDamage4"] = "Text44",               // Weapons and Damage Cantrips Damage/Type Row 4
            ["WeaponCantripDamage5"] = "Text48",               // Weapons and Damage Cantrips Damage/Type Row 5
            ["WeaponCantripDamage6"] = "Text52",               // Weapons and Damage Cantrips Damage/Type Row 6
            ["WeaponCantripNotes1"] = "Text33",                // Weapons and Damage Cantrips Notes Row 1
            ["WeaponCantripNotes2"] = "Text37",                // Weapons and Damage Cantrips Notes Row 2
            ["WeaponCantripNotes3"] = "Text41",                // Weapons and Damage Cantrips Notes Row 3
            ["WeaponCantripNotes4"] = "Text45",                // Weapons and Damage Cantrips Notes Row 4
            ["WeaponCantripNotes5"] = "Text49",                // Weapons and Damage Cantrips Notes Row 5
            ["WeaponCantripNotes6"] = "Text53",                // Weapons and Damage Cantrips Notes Row 6
            
            // Spell Slots - Level 1
            ["SpellSlot1_1"] = "Check Box227",                 // Level 1 Spellslot 1
            ["SpellSlot1_2"] = "Check Box228",                 // Level 1 Spellslot 2
            ["SpellSlot1_3"] = "Check Box229",                 // Level 1 Spellslot 3
            ["SpellSlot1_4"] = "Check Box230",                 // Level 1 Spellslot 4
            ["SpellSlot1Total"] = "Text112",                   // Total Level 1 Spellslots
            
            // Spell Slots - Level 2
            ["SpellSlot2_1"] = "Check Box231",                 // Level 2 Spellslot 1
            ["SpellSlot2_2"] = "Check Box232",                 // Level 2 Spellslot 2
            ["SpellSlot2_3"] = "Check Box233",                 // Level 2 Spellslot 3
            ["SpellSlot2Total"] = "Text113",                   // Total Level 2 Spellslots
            
            // Spell Slots - Level 3
            ["SpellSlot3_1"] = "Check Box234",                 // Level 3 Spellslot 1
            ["SpellSlot3_2"] = "Check Box235",                 // Level 3 Spellslot 2
            ["SpellSlot3_3"] = "Check Box236",                 // Level 3 Spellslot 3
            ["SpellSlot3Total"] = "Text114",                   // Total Level 3 Spellslots
            
            // Spell Slots - Level 4
            ["SpellSlot4_1"] = "Check Box237",                 // Level 4 Spellslot 1
            ["SpellSlot4_2"] = "Check Box238",                 // Level 4 Spellslot 2
            ["SpellSlot4_3"] = "Check Box239",                 // Level 4 Spellslot 3
            ["SpellSlot4Total"] = "Text115",                   // Total Level 4 SpellSlots
            
            // Spell Slots - Level 5
            ["SpellSlot5_1"] = "Check Box240",                 // Level 5 Spellslot 1
            ["SpellSlot5_2"] = "Check Box241",                 // Level 5 Spellslot 2
            ["SpellSlot5_3"] = "Check Box242",                 // Level 5 Spellslot 3
            ["SpellSlot5Total"] = "Text116",                   // Total Level 5 SpellSlots
            
            // Spell Slots - Level 6
            ["SpellSlot6_1"] = "Check Box243",                 // Level 6 Spellslot 1
            ["SpellSlot6_2"] = "Check Box244",                 // Level 6 Spellslot 2
            ["SpellSlot6Total"] = "Text117",                   // Total Level 6 SpellSlots
            
            // Spell Slots - Level 7
            ["SpellSlot7_1"] = "Check Box245",                 // Level 7 Spellslot 1
            ["SpellSlot7_2"] = "Check Box246",                 // Level 7 Spellslot 2
            ["SpellSlot7Total"] = "Text118",                   // Total Level 7 SpellSlots
            
            // Spell Slots - Level 8
            ["SpellSlot8_1"] = "Check Box247",                 // Level 8 Spellslot 1
            ["SpellSlot8Total"] = "Text119",                   // Total Level 8 SpellSlots
            
            // Spell Slots - Level 9
            ["SpellSlot9_1"] = "Check Box248",                 // Level 9 Spellslot 1
            ["SpellSlot9Total"] = "Text120",                   // Total Level 9 SpellSlots
            
            // Cantrips and Prepared Spells - Rows 1-7 (using Check Box252[0][0-6], Check Box253[0][0-6], Check Box254[0][0-6])
            ["SpellRow1_Concentration"] = "Check Box252[0][0]", // Row 1 Concentration
            ["SpellRow1_Ritual"] = "Check Box253[0][0]",        // Row 1 Ritual
            ["SpellRow1_Material"] = "Check Box254[0][0]",      // Row 1 Material
            ["SpellRow1_Level"] = "Text105",                    // Row 1 Spell Level
            ["SpellRow1_Name"] = "Text106",                     // Row 1 Spell Name
            ["SpellRow1_CastingTime"] = "Text107",              // Row 1 Casting Time
            ["SpellRow1_Range"] = "Text109",                    // Row 1 Range
            ["SpellRow1_Notes"] = "Text108",                    // Row 1 Notes
            
            ["SpellRow2_Concentration"] = "Check Box252[0][1]", // Row 2 Concentration
            ["SpellRow2_Ritual"] = "Check Box253[0][1]",        // Row 2 Ritual
            ["SpellRow2_Material"] = "Check Box254[0][1]",      // Row 2 Material
            ["SpellRow2_Level"] = "Text121",                    // Row 2 Spell Level
            ["SpellRow2_Name"] = "Text150",                     // Row 2 Spell Name
            ["SpellRow2_CastingTime"] = "Text188",              // Row 2 Casting Time
            ["SpellRow2_Range"] = "Text237",                    // Row 2 Range
            ["SpellRow2_Notes"] = "Text208",                    // Row 2 Notes
            
            ["SpellRow3_Concentration"] = "Check Box252[0][2]", // Row 3 Concentration
            ["SpellRow3_Ritual"] = "Check Box253[0][2]",        // Row 3 Ritual
            ["SpellRow3_Material"] = "Check Box254[0][2]",      // Row 3 Material
            ["SpellRow3_Level"] = "Text122",                    // Row 3 Spell Level
            ["SpellRow3_Name"] = "Text151",                     // Row 3 Spell Name
            ["SpellRow3_CastingTime"] = "Text179",              // Row 3 Casting Time
            ["SpellRow3_Range"] = "Text238",                    // Row 3 Range
            ["SpellRow3_Notes"] = "Text209",                    // Row 3 Notes
            
            ["SpellRow4_Concentration"] = "Check Box252[0][3]", // Row 4 Concentration
            ["SpellRow4_Ritual"] = "Check Box253[0][3]",        // Row 4 Ritual
            ["SpellRow4_Material"] = "Check Box254[0][3]",      // Row 4 Material
            ["SpellRow4_Level"] = "Text123",                    // Row 4 Spell Level
            ["SpellRow4_Name"] = "Text152",                     // Row 4 Spell Name
            ["SpellRow4_CastingTime"] = "Text180",              // Row 4 Casting Time
            ["SpellRow4_Range"] = "Text239",                    // Row 4 Range
            ["SpellRow4_Notes"] = "Text210",                    // Row 4 Notes
            
            ["SpellRow5_Concentration"] = "Check Box252[0][4]", // Row 5 Concentration
            ["SpellRow5_Ritual"] = "Check Box253[0][4]",        // Row 5 Ritual
            ["SpellRow5_Material"] = "Check Box254[0][4]",      // Row 5 Material
            ["SpellRow5_Level"] = "Text124",                    // Row 5 Spell Level
            ["SpellRow5_Name"] = "Text153",                     // Row 5 Spell Name
            ["SpellRow5_CastingTime"] = "Text181",              // Row 5 Casting Time
            ["SpellRow5_Range"] = "Text240",                    // Row 5 Range
            ["SpellRow5_Notes"] = "Text211",                    // Row 5 Notes
            
            ["SpellRow6_Concentration"] = "Check Box252[0][5]", // Row 6 Concentration
            ["SpellRow6_Ritual"] = "Check Box253[0][5]",        // Row 6 Ritual
            ["SpellRow6_Material"] = "Check Box254[0][5]",      // Row 6 Material
            ["SpellRow6_Level"] = "Text125",                    // Row 6 Spell Level
            ["SpellRow6_Name"] = "Text154",                     // Row 6 Spell Name
            ["SpellRow6_CastingTime"] = "Text182",              // Row 6 Casting Time
            ["SpellRow6_Range"] = "Text241",                    // Row 6 Range
            ["SpellRow6_Notes"] = "Text212",                    // Row 6 Notes
            
            ["SpellRow7_Concentration"] = "Check Box252[0][6]", // Row 7 Concentration
            ["SpellRow7_Ritual"] = "Check Box253[0][6]",        // Row 7 Ritual
            ["SpellRow7_Material"] = "Check Box254[0][6]",      // Row 7 Material
            ["SpellRow7_Level"] = "Text126",                    // Row 7 Spell Level
            ["SpellRow7_Name"] = "Text155",                     // Row 7 Spell Name
            ["SpellRow7_CastingTime"] = "Text183",              // Row 7 Casting Time
            ["SpellRow7_Range"] = "Text242",                    // Row 7 Range
            ["SpellRow7_Notes"] = "Text213",                    // Row 7 Notes
            
            // Rows 8-20 (using Check Box255[0-12], Check Box256[0-12], Check Box257[0-12])
            ["SpellRow8_Concentration"] = "Check Box255[0]",    // Row 8 Concentration
            ["SpellRow8_Ritual"] = "Check Box256[0]",           // Row 8 Ritual
            ["SpellRow8_Material"] = "Check Box257[0]",         // Row 8 Material
            ["SpellRow8_Level"] = "Text127",                    // Row 8 Spell Level
            ["SpellRow8_Name"] = "Text156",                     // Row 8 Spell Name
            ["SpellRow8_CastingTime"] = "Text184",              // Row 8 Casting Time
            ["SpellRow8_Range"] = "Text243",                    // Row 8 Range
            ["SpellRow8_Notes"] = "Text214",                    // Row 8 Notes
            
            // Continue pattern for rows 9-20...
            ["SpellRow9_Concentration"] = "Check Box255[1]",    // Row 9 Concentration
            ["SpellRow9_Ritual"] = "Check Box256[1]",           // Row 9 Ritual
            ["SpellRow9_Material"] = "Check Box257[1]",         // Row 9 Material
            ["SpellRow9_Level"] = "Text128",                    // Row 9 Spell Level
            ["SpellRow9_Name"] = "Text157",                     // Row 9 Spell Name
            ["SpellRow9_CastingTime"] = "Text185",              // Row 9 Casting Time
            ["SpellRow9_Range"] = "Text245",                    // Row 9 Range
            ["SpellRow9_Notes"] = "Text215",                    // Row 9 Notes
            
            ["SpellRow10_Concentration"] = "Check Box255[2]",   // Row 10 Concentration
            ["SpellRow10_Ritual"] = "Check Box256[2]",          // Row 10 Ritual
            ["SpellRow10_Material"] = "Check Box257[2]",        // Row 10 Material
            ["SpellRow10_Level"] = "Text129",                   // Row 10 Spell Level
            ["SpellRow10_Name"] = "Text158",                    // Row 10 Spell Name
            ["SpellRow10_CastingTime"] = "Text186",             // Row 10 Casting Time
            ["SpellRow10_Range"] = "Text246",                   // Row 10 Range
            ["SpellRow10_Notes"] = "Text216",                   // Row 10 Notes
            
            // Spellcasting
            ["SpellcastingModifier"] = "Text93",               // Spellcasting Modifier
            ["SpellSaveDC"] = "Text94",                         // Spell Save DC
            ["SpellAttackBonus"] = "Text95",                    // Spell Attack Bonus
            ["SpellcastingAbility"] = "Text111",               // Spellcasting Ability
            
            // Large Text Areas
            ["ClassFeatures"] = "Text54",                      // Class Features 1
            ["ClassFeatures2"] = "Text55",                     // Class Features 2
            ["SpeciesTraits"] = "Text57",                      // Species Traits
            ["Feats"] = "Text58",                              // Feats
            ["WeaponsProficiencies"] = "Text59",               // Weapon Proficiencies
            ["ToolsProficiencies"] = "Text60",                 // Tool Proficiencies
            ["Appearance"] = "Text96",                         // Appearance
            ["BackstoryPersonality"] = "Text97",               // Backstory and Personality
            ["Languages"] = "Text98",                          // Languages
            ["Equipment"] = "Text99"                           // Equipment
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