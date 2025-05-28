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
            
            // Saving Throws
            ["StrengthSave"] = "text_60zpuk",           // Strength Saving Throw ✅
            ["DexteritySave"] = "text_68bipd",          // Dexterity Saving Throw ✅
            ["ConstitutionSave"] = "text_72gnbq",       // Constitution Saving Throw ✅
            ["IntelligenceSave"] = "text_54bfgy",       // Intelligence Saving Throw ✅
            ["WisdomSave"] = "text_62yaan",             // Wisdom Saving Throw ✅
            ["CharismaSave"] = "text_73cwxt",           // Charisma Saving Throw ✅
            
            // Saving Throw Proficiencies
            ["StrengthSaveProficiency"] = "checkbox_125msvq",       // Strength Saving Throw Proficiency ✅
            ["DexteritySaveProficiency"] = "checkbox_127neck",      // Dexterity Saving Throw Proficiency ✅
            ["ConstitutionSaveProficiency"] = "checkbox_131jtbq",   // Constitution Saving Throw Proficiency ✅
            ["IntelligenceSaveProficiency"] = "checkbox_119phvx",   // Intelligence Saving Throw Proficiency ✅
            ["WisdomSaveProficiency"] = "checkbox_245mnu",          // Wisdom Saving Throw Proficiency ✅
            ["CharismaSaveProficiency"] = "checkbox_251dtie",       // Charisma Saving Throw Proficiency ✅
            
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
            
            // Skill Proficiencies
            ["AcrobaticsProficiency"] = "checkbox_128cefr",         // Acrobatics Skill Proficiency ✅
            ["AnimalHandlingProficiency"] = "checkbox_246hqns",     // Animal Handling Skill Proficiency ✅
            ["ArcanaProficiency"] = "checkbox_120drb",              // Arcana Skill Proficiency ✅
            ["AthleticsProficiency"] = "checkbox_126dqaq",          // Athletics Skill Proficiency ✅
            ["DeceptionProficiency"] = "checkbox_252naxc",          // Deception Skill Proficiency ✅
            ["HistoryProficiency"] = "checkbox_121xgrv",            // History Skill Proficiency ✅
            ["InsightProficiency"] = "checkbox_247lffe",            // Insight Skill Proficiency ✅
            ["IntimidationProficiency"] = "checkbox_253mbyq",       // Intimidation Skill Proficiency ✅
            ["InvestigationProficiency"] = "checkbox_122zffm",      // Investigation Skill Proficiency ✅
            ["MedicineProficiency"] = "checkbox_248scbg",           // Medicine Skill Proficiency ✅
            ["NatureProficiency"] = "checkbox_123smy",              // Nature Skill Proficiency ✅
            ["PerceptionProficiency"] = "checkbox_249voxf",         // Perception Skill Proficiency ✅
            ["PerformanceProficiency"] = "checkbox_254ypds",        // Performance Skill Proficiency ✅
            ["PersuasionProficiency"] = "checkbox_255ltdr",         // Persuasion Skill Proficiency ✅
            ["ReligionProficiency"] = "checkbox_124zscb",           // Religion Skill Proficiency ✅
            ["SleightOfHandProficiency"] = "checkbox_129tlov",      // Sleight of Hand Skill Proficiency ✅
            ["StealthProficiency"] = "checkbox_130ukqx",            // Stealth Skill Proficiency ✅
            ["SurvivalProficiency"] = "checkbox_250mjvi",           // Survival Skill Proficiency ✅
            
            // Armor Proficiencies and Equipment
            ["LightArmorProficiency"] = "checkbox_78ywrl",          // Light Armor Proficiency ✅
            ["MediumArmorProficiency"] = "checkbox_79gwxi",         // Medium Armor Proficiency ✅
            ["HeavyArmorProficiency"] = "checkbox_80xjpq",          // Heavy Armor Proficiency ✅
            ["ShieldProficiency"] = "checkbox_81xqwc",              // Shield Proficiency ✅
            ["ShieldEquipped"] = "checkbox_82wlwz",                 // Shield Equipped ✅
            
            // Death Saves
            ["DeathSaveSuccess1"] = "checkbox_83uays",              // Death Save Success 1 ✅
            ["DeathSaveSuccess2"] = "checkbox_84cpmm",              // Death Save Success 2 ✅
            ["DeathSaveSuccess3"] = "checkbox_85xlev",              // Death Save Success 3 ✅
            ["DeathSaveFailure1"] = "checkbox_86iksy",              // Death Save Failure 1 ✅
            ["DeathSaveFailure2"] = "checkbox_87keam",              // Death Save Failure 2 ✅
            ["DeathSaveFailure3"] = "checkbox_88iabv",              // Death Save Failure 3 ✅
            
            // Heroic Inspiration
            ["HeroicInspiration"] = "checkbox_132oesm",             // Heroic Inspiration ✅
            
            // Magic Item Attunement
            ["MagicItemAttunement1"] = "checkbox_256gnfj",          // Magic Item Attunement 1 ✅
            ["MagicItemAttunement2"] = "checkbox_257pzgq",          // Magic Item Attunement 2 ✅
            ["MagicItemAttunement3"] = "checkbox_258imyr",          // Magic Item Attunement 3 ✅
            ["MagicItemAttunementText1"] = "text_262lxdq",          // Magic Item Attunement Text 1 ✅
            ["MagicItemAttunementText2"] = "text_263nbcj",          // Magic Item Attunement Text 2 ✅
            ["MagicItemAttunementText3"] = "text_264exxd",          // Magic Item Attunement Text 3 ✅
            
            // Currency
            ["CopperPieces"] = "text_265qztg",                      // Copper Pieces ✅
            ["SilverPieces"] = "text_266ykte",                      // Silver Pieces ✅
            ["EmeraldPieces"] = "text_267yykj",                     // Emerald Pieces ✅
            ["GoldPieces"] = "text_268lycv",                        // Gold Pieces ✅
            ["PlatinumPieces"] = "text_269bzjf",                    // Platinum Pieces ✅
            
            // Weapons and Damage Cantrips
            ["WeaponCantripName1"] = "text_95mtme",                 // Weapons and Damage Cantrips Name Row 1 ✅
            ["WeaponCantripName2"] = "text_96nzoa",                 // Weapons and Damage Cantrips Name Row 2 ✅
            ["WeaponCantripName3"] = "text_97iydj",                 // Weapons and Damage Cantrips Name Row 3 ✅
            ["WeaponCantripName4"] = "text_98wnad",                 // Weapons and Damage Cantrips Name Row 4 ✅
            ["WeaponCantripName5"] = "text_99bdzl",                 // Weapons and Damage Cantrips Name Row 5 ✅
            ["WeaponCantripName6"] = "text_100zmbl",                // Weapons and Damage Cantrips Name Row 6 ✅
            ["WeaponCantripAttack1"] = "text_101ohoi",              // Weapons and Damage Cantrips Atk Bonus/DC Row 1 ✅
            ["WeaponCantripAttack2"] = "text_102yibj",              // Weapons and Damage Cantrips Atk Bonus/DC Row 2 ✅
            ["WeaponCantripAttack3"] = "text_103rlae",              // Weapons and Damage Cantrips Atk Bonus/DC Row 3 ✅
            ["WeaponCantripAttack4"] = "text_104qwkw",              // Weapons and Damage Cantrips Atk Bonus/DC Row 4 ✅
            ["WeaponCantripAttack5"] = "text_105gvuz",              // Weapons and Damage Cantrips Atk Bonus/DC Row 5 ✅
            ["WeaponCantripAttack6"] = "text_106vkdo",              // Weapons and Damage Cantrips Atk Bonus/DC Row 6 ✅
            ["WeaponCantripDamage1"] = "text_107qvwl",              // Weapons and Damage Cantrips Damage/Type Row 1 ✅
            ["WeaponCantripDamage2"] = "text_108aybq",              // Weapons and Damage Cantrips Damage/Type Row 2 ✅
            ["WeaponCantripDamage3"] = "text_109slbn",              // Weapons and Damage Cantrips Damage/Type Row 3 ✅
            ["WeaponCantripDamage4"] = "text_110lbdh",              // Weapons and Damage Cantrips Damage/Type Row 4 ✅
            ["WeaponCantripDamage5"] = "text_111ubex",              // Weapons and Damage Cantrips Damage/Type Row 5 ✅
            ["WeaponCantripDamage6"] = "text_112omg",               // Weapons and Damage Cantrips Damage/Type Row 6 ✅
            ["WeaponCantripNotes1"] = "text_113tpwa",               // Weapons and Damage Cantrips Notes Row 1 ✅
            ["WeaponCantripNotes2"] = "text_114lhuk",               // Weapons and Damage Cantrips Notes Row 2 ✅
            ["WeaponCantripNotes3"] = "text_115pcxn",               // Weapons and Damage Cantrips Notes Row 3 ✅
            ["WeaponCantripNotes4"] = "text_116sohv",               // Weapons and Damage Cantrips Notes Row 4 ✅
            ["WeaponCantripNotes5"] = "text_117fabc",               // Weapons and Damage Cantrips Notes Row 5 ✅
            ["WeaponCantripNotes6"] = "text_118cokl",               // Weapons and Damage Cantrips Notes Row 6 ✅
            
            // Spell Slots - Level 1
            ["SpellSlot1_1"] = "checkbox_133fsnv",                  // Level 1 Spellslot 1 ✅
            ["SpellSlot1_2"] = "checkbox_134uxes",                  // Level 1 Spellslot 2 ✅
            ["SpellSlot1_3"] = "checkbox_135smcx",                  // Level 1 Spellslot 3 ✅
            ["SpellSlot1_4"] = "checkbox_136eqsh",                  // Level 1 Spellslot 4 ✅
            
            // Spell Slots - Level 2
            ["SpellSlot2_1"] = "checkbox_137bkvv",                  // Level 2 Spellslot 1 ✅
            ["SpellSlot2_2"] = "checkbox_138hfaf",                  // Level 2 Spellslot 2 ✅
            ["SpellSlot2_3"] = "checkbox_139xewc",                  // Level 2 Spellslot 3 ✅
            
            // Spell Slots - Level 3
            ["SpellSlot3_1"] = "checkbox_140asxv",                  // Level 3 Spellslot 1 ✅
            ["SpellSlot3_2"] = "checkbox_141tzzj",                  // Level 3 Spellslot 2 ✅
            ["SpellSlot3_3"] = "checkbox_142llnt",                  // Level 3 Spellslot 3 ✅
            
            // Spell Slots - Level 4
            ["SpellSlot4_1"] = "checkbox_143wmsd",                  // Level 4 Spellslot 1 ✅
            ["SpellSlot4_2"] = "checkbox_144ypdo",                  // Level 4 Spellslot 2 ✅
            ["SpellSlot4_3"] = "checkbox_145rsbk",                  // Level 4 Spellslot 3 ✅
            
            // Spell Slots - Level 5
            ["SpellSlot5_1"] = "checkbox_146rcj",                   // Level 5 Spellslot 1 ✅
            ["SpellSlot5_2"] = "checkbox_147dpvm",                  // Level 5 Spellslot 2 ✅
            ["SpellSlot5_3"] = "checkbox_148pgfy",                  // Level 5 Spellslot 3 ✅
            
            // Spell Slots - Level 6
            ["SpellSlot6_1"] = "checkbox_149fdie",                  // Level 6 Spellslot 1 ✅
            ["SpellSlot6_2"] = "checkbox_150mpij",                  // Level 6 Spellslot 2 ✅
            
            // Spell Slots - Level 7
            ["SpellSlot7_1"] = "checkbox_151rogi",                  // Level 7 Spellslot 1 ✅
            ["SpellSlot7_2"] = "checkbox_152mibc",                  // Level 7 Spellslot 2 ✅
            
            // Spell Slots - Level 8
            ["SpellSlot8_1"] = "checkbox_153ylth",                  // Level 8 Spellslot 1 ✅
            
            // Spell Slots - Level 9
            ["SpellSlot9_1"] = "checkbox_154nskw",                  // Level 9 Spellslot 1 ✅
            
            // Cantrips and Prepared Spells - Row 1 (All Fields)
            ["SpellRow1_Concentration"] = "checkbox_155igkk",       // Row 1 Concentration ✅
            ["SpellRow1_Ritual"] = "checkbox_156dpdh",              // Row 1 Ritual ✅
            ["SpellRow1_Material"] = "checkbox_157mcmr",            // Row 1 Material ✅
            ["SpellRow1_Level"] = "text_276tnnb",                   // Row 1 Spell Level ✅
            ["SpellRow1_Name"] = "text_306noh",                     // Row 1 Spell Name ✅
            ["SpellRow1_CastingTime"] = "text_336ipws",             // Row 1 Casting Time ✅
            ["SpellRow1_Range"] = "text_366kvbn",                   // Row 1 Range ✅
            ["SpellRow1_Notes"] = "text_396fpbx",                   // Row 1 Notes ✅
            
            // Cantrips and Prepared Spells - Row 2 (All Fields)
            ["SpellRow2_Concentration"] = "checkbox_158tnpw",       // Row 2 Concentration ✅
            ["SpellRow2_Ritual"] = "checkbox_159ykcu",              // Row 2 Ritual ✅
            ["SpellRow2_Material"] = "checkbox_160leix",            // Row 2 Material ✅
            ["SpellRow2_Level"] = "text_277ozpu",                   // Row 2 Spell Level ✅
            ["SpellRow2_Name"] = "text_307kiz",                     // Row 2 Spell Name ✅
            ["SpellRow2_CastingTime"] = "text_337wzyu",             // Row 2 Casting Time ✅
            ["SpellRow2_Range"] = "text_367lrli",                   // Row 2 Range ✅
            ["SpellRow2_Notes"] = "text_397ytaj",                   // Row 2 Notes ✅
            
            // Cantrips and Prepared Spells - Row 3 (All Fields)
            ["SpellRow3_Concentration"] = "checkbox_161puyr",       // Row 3 Concentration ✅
            ["SpellRow3_Ritual"] = "checkbox_162tygu",              // Row 3 Ritual ✅
            ["SpellRow3_Material"] = "checkbox_163waps",            // Row 3 Material ✅
            ["SpellRow3_Level"] = "text_278naaw",                   // Row 3 Spell Level ✅
            ["SpellRow3_Name"] = "text_308oany",                    // Row 3 Spell Name ✅
            ["SpellRow3_CastingTime"] = "text_338ytza",             // Row 3 Casting Time ✅
            ["SpellRow3_Range"] = "text_368gjzj",                   // Row 3 Range ✅
            ["SpellRow3_Notes"] = "text_398joio",                   // Row 3 Notes ✅
            
            // Cantrips and Prepared Spells - Row 4 (All Fields)
            ["SpellRow4_Concentration"] = "checkbox_164vghx",       // Row 4 Concentration ✅
            ["SpellRow4_Ritual"] = "checkbox_165zsg",               // Row 4 Ritual ✅
            ["SpellRow4_Material"] = "checkbox_166rrhd",            // Row 4 Material ✅
            ["SpellRow4_Level"] = "text_279pbow",                   // Row 4 Spell Level ✅
            ["SpellRow4_Name"] = "text_309vlmr",                    // Row 4 Spell Name ✅
            ["SpellRow4_CastingTime"] = "text_339arfm",             // Row 4 Casting Time ✅
            ["SpellRow4_Range"] = "text_369ncpf",                   // Row 4 Range ✅
            ["SpellRow4_Notes"] = "text_399zzff",                   // Row 4 Notes ✅
            
            // Cantrips and Prepared Spells - Row 5 (All Fields)
            ["SpellRow5_Concentration"] = "checkbox_167ftoe",       // Row 5 Concentration ✅
            ["SpellRow5_Ritual"] = "checkbox_168wqfx",              // Row 5 Ritual ✅
            ["SpellRow5_Material"] = "checkbox_169vree",            // Row 5 Material ✅
            ["SpellRow5_Level"] = "text_280sjlk",                   // Row 5 Spell Level ✅
            ["SpellRow5_Name"] = "text_310iatm",                    // Row 5 Spell Name ✅
            ["SpellRow5_CastingTime"] = "text_340ccjc",             // Row 5 Casting Time ✅
            ["SpellRow5_Range"] = "text_370fitk",                   // Row 5 Range ✅
            ["SpellRow5_Notes"] = "text_400jtrv",                   // Row 5 Notes ✅
            
            // Cantrips and Prepared Spells - Row 6 (All Fields)
            ["SpellRow6_Concentration"] = "checkbox_170xhdq",       // Row 6 Concentration ✅
            ["SpellRow6_Ritual"] = "checkbox_171wuib",              // Row 6 Ritual ✅
            ["SpellRow6_Material"] = "checkbox_172yxmq",            // Row 6 Material ✅
            ["SpellRow6_Level"] = "text_281pirp",                   // Row 6 Spell Level ✅
            ["SpellRow6_Name"] = "text_311eska",                    // Row 6 Spell Name ✅
            ["SpellRow6_CastingTime"] = "text_341ifjx",             // Row 6 Casting Time ✅
            ["SpellRow6_Range"] = "text_371nwgj",                   // Row 6 Range ✅
            ["SpellRow6_Notes"] = "text_401evaj",                   // Row 6 Notes ✅
            
            // Cantrips and Prepared Spells - Row 7 (All Fields)
            ["SpellRow7_Concentration"] = "checkbox_173fcki",       // Row 7 Concentration ✅
            ["SpellRow7_Ritual"] = "checkbox_174ux",                // Row 7 Ritual ✅
            ["SpellRow7_Material"] = "checkbox_175jjkg",            // Row 7 Material ✅
            ["SpellRow7_Level"] = "text_282ncxo",                   // Row 7 Spell Level ✅
            ["SpellRow7_Name"] = "text_312trhs",                    // Row 7 Spell Name ✅
            ["SpellRow7_CastingTime"] = "text_342porh",             // Row 7 Casting Time ✅
            ["SpellRow7_Range"] = "text_372pdui",                   // Row 7 Range ✅
            ["SpellRow7_Notes"] = "text_402rmvx",                   // Row 7 Notes ✅
            
            // Cantrips and Prepared Spells - Row 8 (All Fields)
            ["SpellRow8_Concentration"] = "checkbox_176hwey",       // Row 8 Concentration ✅
            ["SpellRow8_Ritual"] = "checkbox_177gcrg",              // Row 8 Ritual ✅
            ["SpellRow8_Material"] = "checkbox_178mfym",            // Row 8 Material ✅
            ["SpellRow8_Level"] = "text_283dvfl",                   // Row 8 Spell Level ✅
            ["SpellRow8_Name"] = "text_313jros",                    // Row 8 Spell Name ✅
            ["SpellRow8_CastingTime"] = "text_343pgkv",             // Row 8 Casting Time ✅
            ["SpellRow8_Range"] = "text_373ldhy",                   // Row 8 Range ✅
            ["SpellRow8_Notes"] = "text_403alrh",                   // Row 8 Notes ✅
            
            // Cantrips and Prepared Spells - Row 9 (All Fields)
            ["SpellRow9_Concentration"] = "checkbox_179rogc",       // Row 9 Concentration ✅
            ["SpellRow9_Ritual"] = "checkbox_180wozz",              // Row 9 Ritual ✅
            ["SpellRow9_Material"] = "checkbox_181xloo",            // Row 9 Material ✅
            ["SpellRow9_Level"] = "text_284xpfb",                   // Row 9 Spell Level ✅
            ["SpellRow9_Name"] = "text_314gyue",                    // Row 9 Spell Name ✅
            ["SpellRow9_CastingTime"] = "text_344avnr",             // Row 9 Casting Time ✅
            ["SpellRow9_Range"] = "text_374zkkr",                   // Row 9 Range ✅
            ["SpellRow9_Notes"] = "text_404cd",                     // Row 9 Notes ✅
            
            // Cantrips and Prepared Spells - Row 10 (All Fields)
            ["SpellRow10_Concentration"] = "checkbox_182fzje",      // Row 10 Concentration ✅
            ["SpellRow10_Ritual"] = "checkbox_183jbbp",             // Row 10 Ritual ✅
            ["SpellRow10_Material"] = "checkbox_184cqbx",           // Row 10 Material ✅
            ["SpellRow10_Level"] = "text_285hhix",                  // Row 10 Spell Level ✅
            ["SpellRow10_Name"] = "text_315plkl",                   // Row 10 Spell Name ✅
            ["SpellRow10_CastingTime"] = "text_345dqn",             // Row 10 Casting Time ✅
            ["SpellRow10_Range"] = "text_375zhpt",                  // Row 10 Range ✅
            ["SpellRow10_Notes"] = "text_405dpvo",                  // Row 10 Notes ✅
            
            // Cantrips and Prepared Spells - Row 11 (All Fields)
            ["SpellRow11_Concentration"] = "checkbox_185eyvk",      // Row 11 Concentration ✅
            ["SpellRow11_Ritual"] = "checkbox_186byvu",             // Row 11 Ritual ✅
            ["SpellRow11_Material"] = "checkbox_187vtmi",           // Row 11 Material ✅
            ["SpellRow11_Level"] = "text_286sxcn",                  // Row 11 Spell Level ✅
            ["SpellRow11_Name"] = "text_316ndrn",                   // Row 11 Spell Name ✅
            ["SpellRow11_CastingTime"] = "text_346kmra",            // Row 11 Casting Time ✅
            ["SpellRow11_Range"] = "text_376igdr",                  // Row 11 Range ✅
            ["SpellRow11_Notes"] = "text_406wdqh",                  // Row 11 Notes ✅
            
            // Cantrips and Prepared Spells - Row 12 (All Fields)
            ["SpellRow12_Concentration"] = "checkbox_188nykl",      // Row 12 Concentration ✅
            ["SpellRow12_Ritual"] = "checkbox_189xfur",             // Row 12 Ritual ✅
            ["SpellRow12_Material"] = "checkbox_190psyl",           // Row 12 Material ✅
            ["SpellRow12_Level"] = "text_287swf",                   // Row 12 Spell Level ✅
            ["SpellRow12_Name"] = "text_317gkbw",                   // Row 12 Spell Name ✅
            ["SpellRow12_CastingTime"] = "text_347zdgy",            // Row 12 Casting Time ✅
            ["SpellRow12_Range"] = "text_377akvu",                  // Row 12 Range ✅
            ["SpellRow12_Notes"] = "text_407rgml",                  // Row 12 Notes ✅
            
            // Cantrips and Prepared Spells - Row 13 (All Fields)
            ["SpellRow13_Concentration"] = "checkbox_191mrty",      // Row 13 Concentration ✅
            ["SpellRow13_Ritual"] = "checkbox_192mbn",              // Row 13 Ritual ✅
            ["SpellRow13_Material"] = "checkbox_193lbfy",           // Row 13 Material ✅
            ["SpellRow13_Level"] = "text_288woyc",                  // Row 13 Spell Level ✅
            ["SpellRow13_Name"] = "text_318ogf",                    // Row 13 Spell Name ✅
            ["SpellRow13_CastingTime"] = "text_348rbre",            // Row 13 Casting Time ✅
            ["SpellRow13_Range"] = "text_378ongo",                  // Row 13 Range ✅
            ["SpellRow13_Notes"] = "text_408ekxz",                  // Row 13 Notes ✅
            
            // Cantrips and Prepared Spells - Row 14 (All Fields)
            ["SpellRow14_Concentration"] = "checkbox_194dlof",      // Row 14 Concentration ✅
            ["SpellRow14_Ritual"] = "checkbox_195knvm",             // Row 14 Ritual ✅
            ["SpellRow14_Material"] = "checkbox_196zsjg",           // Row 14 Material ✅
            ["SpellRow14_Level"] = "text_289qjuu",                  // Row 14 Spell Level ✅
            ["SpellRow14_Name"] = "text_319huqd",                   // Row 14 Spell Name ✅
            ["SpellRow14_CastingTime"] = "text_349jhxa",            // Row 14 Casting Time ✅
            ["SpellRow14_Range"] = "text_379isil",                  // Row 14 Range ✅
            ["SpellRow14_Notes"] = "text_409bfis",                  // Row 14 Notes ✅
            
            // Cantrips and Prepared Spells - Row 15 (All Fields)
            ["SpellRow15_Concentration"] = "checkbox_197btgs",      // Row 15 Concentration ✅
            ["SpellRow15_Ritual"] = "checkbox_198ylpb",             // Row 15 Ritual ✅
            ["SpellRow15_Material"] = "checkbox_199bnrg",           // Row 15 Material ✅
            ["SpellRow15_Level"] = "text_290yetj",                  // Row 15 Spell Level ✅
            ["SpellRow15_Name"] = "text_320kzak",                   // Row 15 Spell Name ✅
            ["SpellRow15_CastingTime"] = "text_350bvre",            // Row 15 Casting Time ✅
            ["SpellRow15_Range"] = "text_380vqyg",                  // Row 15 Range ✅
            ["SpellRow15_Notes"] = "text_410ntrj",                  // Row 15 Notes ✅
            
            // Cantrips and Prepared Spells - Row 16 (All Fields)
            ["SpellRow16_Concentration"] = "checkbox_200yvql",      // Row 16 Concentration ✅
            ["SpellRow16_Ritual"] = "checkbox_201blxv",             // Row 16 Ritual ✅
            ["SpellRow16_Material"] = "checkbox_202etpy",           // Row 16 Material ✅
            ["SpellRow16_Level"] = "text_291kfeq",                  // Row 16 Spell Level ✅
            ["SpellRow16_Name"] = "text_321ucnv",                   // Row 16 Spell Name ✅
            ["SpellRow16_CastingTime"] = "text_351cwtu",            // Row 16 Casting Time ✅
            ["SpellRow16_Range"] = "text_381ozpu",                  // Row 16 Range ✅
            ["SpellRow16_Notes"] = "text_411ozsg",                  // Row 16 Notes ✅
            
            // Cantrips and Prepared Spells - Row 17 (All Fields)
            ["SpellRow17_Concentration"] = "checkbox_203kayy",      // Row 17 Concentration ✅
            ["SpellRow17_Ritual"] = "checkbox_204fvv",              // Row 17 Ritual ✅
            ["SpellRow17_Material"] = "checkbox_205vyoc",           // Row 17 Material ✅
            ["SpellRow17_Level"] = "text_292feom",                  // Row 17 Spell Level ✅
            ["SpellRow17_Name"] = "text_322wcdj",                   // Row 17 Spell Name ✅
            ["SpellRow17_CastingTime"] = "text_352kpni",            // Row 17 Casting Time ✅
            ["SpellRow17_Range"] = "text_382pvih",                  // Row 17 Range ✅
            ["SpellRow17_Notes"] = "text_412awsp",                  // Row 17 Notes ✅
            
            // Cantrips and Prepared Spells - Row 18 (All Fields)
            ["SpellRow18_Concentration"] = "checkbox_206bbas",      // Row 18 Concentration ✅
            ["SpellRow18_Ritual"] = "checkbox_207eqsq",             // Row 18 Ritual ✅
            ["SpellRow18_Material"] = "checkbox_208szzi",           // Row 18 Material ✅
            ["SpellRow18_Level"] = "text_293caxd",                  // Row 18 Spell Level ✅
            ["SpellRow18_Name"] = "text_323kmpj",                   // Row 18 Spell Name ✅
            ["SpellRow18_CastingTime"] = "text_353ealo",            // Row 18 Casting Time ✅
            ["SpellRow18_Range"] = "text_383fvoy",                  // Row 18 Range ✅
            ["SpellRow18_Notes"] = "text_413nmfu",                  // Row 18 Notes ✅
            
            // Cantrips and Prepared Spells - Row 19 (All Fields)
            ["SpellRow19_Concentration"] = "checkbox_209cyju",      // Row 19 Concentration ✅
            ["SpellRow19_Ritual"] = "checkbox_210kdhg",             // Row 19 Ritual ✅
            ["SpellRow19_Material"] = "checkbox_211ytvj",           // Row 19 Material ✅
            ["SpellRow19_Level"] = "text_294mbxc",                  // Row 19 Spell Level ✅
            ["SpellRow19_Name"] = "text_324jzvi",                   // Row 19 Spell Name ✅
            ["SpellRow19_CastingTime"] = "text_354rrdf",            // Row 19 Casting Time ✅
            ["SpellRow19_Range"] = "text_384nbpp",                  // Row 19 Range ✅
            ["SpellRow19_Notes"] = "text_414zrqc",                  // Row 19 Notes ✅
            
            // Cantrips and Prepared Spells - Row 20 (All Fields)
            ["SpellRow20_Concentration"] = "checkbox_212askh",      // Row 20 Concentration ✅
            ["SpellRow20_Ritual"] = "checkbox_213mjyz",             // Row 20 Ritual ✅
            ["SpellRow20_Material"] = "checkbox_214apke",           // Row 20 Material ✅
            ["SpellRow20_Level"] = "text_295mhkj",                  // Row 20 Spell Level ✅
            ["SpellRow20_Name"] = "text_325xpvz",                   // Row 20 Spell Name ✅
            ["SpellRow20_CastingTime"] = "text_355thdu",            // Row 20 Casting Time ✅
            ["SpellRow20_Range"] = "text_385otxt",                  // Row 20 Range ✅
            ["SpellRow20_Notes"] = "text_415wftz",                  // Row 20 Notes ✅
            
            // Cantrips and Prepared Spells - Row 21 (All Fields)
            ["SpellRow21_Concentration"] = "checkbox_215jxzh",      // Row 21 Concentration ✅
            ["SpellRow21_Ritual"] = "checkbox_216apsx",             // Row 21 Ritual ✅
            ["SpellRow21_Material"] = "checkbox_217iidt",           // Row 21 Material ✅
            ["SpellRow21_Level"] = "text_296nuzy",                  // Row 21 Spell Level ✅
            ["SpellRow21_Name"] = "text_326ydhp",                   // Row 21 Spell Name ✅
            ["SpellRow21_CastingTime"] = "text_356jgbg",            // Row 21 Casting Time ✅
            ["SpellRow21_Range"] = "text_386cqq",                   // Row 21 Range ✅
            ["SpellRow21_Notes"] = "text_416aaoo",                  // Row 21 Notes ✅
            
            // Cantrips and Prepared Spells - Row 22 (All Fields)
            ["SpellRow22_Concentration"] = "checkbox_218yxjn",      // Row 22 Concentration ✅
            ["SpellRow22_Ritual"] = "checkbox_219bduk",             // Row 22 Ritual ✅
            ["SpellRow22_Material"] = "checkbox_220gshh",           // Row 22 Material ✅
            ["SpellRow22_Level"] = "text_297ormh",                  // Row 22 Spell Level ✅
            ["SpellRow22_Name"] = "text_327kpws",                   // Row 22 Spell Name ✅
            ["SpellRow22_CastingTime"] = "text_357fywm",            // Row 22 Casting Time ✅
            ["SpellRow22_Range"] = "text_387rwzj",                  // Row 22 Range ✅
            ["SpellRow22_Notes"] = "text_417ssbh",                  // Row 22 Notes ✅
            
            // Cantrips and Prepared Spells - Row 23 (All Fields)
            ["SpellRow23_Concentration"] = "checkbox_221jhtt",      // Row 23 Concentration ✅
            ["SpellRow23_Ritual"] = "checkbox_222vhxa",             // Row 23 Ritual ✅
            ["SpellRow23_Material"] = "checkbox_223shy",            // Row 23 Material ✅
            ["SpellRow23_Level"] = "text_298dend",                  // Row 23 Spell Level ✅
            ["SpellRow23_Name"] = "text_328uom",                    // Row 23 Spell Name ✅
            ["SpellRow23_CastingTime"] = "text_358ttbz",            // Row 23 Casting Time ✅
            ["SpellRow23_Range"] = "text_388fzpy",                  // Row 23 Range ✅
            ["SpellRow23_Notes"] = "text_418ffqz",                  // Row 23 Notes ✅
            
            // Cantrips and Prepared Spells - Row 24 (All Fields)
            ["SpellRow24_Concentration"] = "checkbox_224zwyr",      // Row 24 Concentration ✅
            ["SpellRow24_Ritual"] = "checkbox_225xkfx",             // Row 24 Ritual ✅
            ["SpellRow24_Material"] = "checkbox_226hdqs",           // Row 24 Material ✅
            ["SpellRow24_Level"] = "text_299jfya",                  // Row 24 Spell Level ✅
            ["SpellRow24_Name"] = "text_329yurm",                   // Row 24 Spell Name ✅
            ["SpellRow24_CastingTime"] = "text_359sdzx",            // Row 24 Casting Time ✅
            ["SpellRow24_Range"] = "text_389njna",                  // Row 24 Range ✅
            ["SpellRow24_Notes"] = "text_419cgqy",                  // Row 24 Notes ✅
            
            // Cantrips and Prepared Spells - Row 25 (All Fields)
            ["SpellRow25_Concentration"] = "checkbox_227ktzl",      // Row 25 Concentration ✅
            ["SpellRow25_Ritual"] = "checkbox_228tcym",             // Row 25 Ritual ✅
            ["SpellRow25_Material"] = "checkbox_229denx",           // Row 25 Material ✅
            ["SpellRow25_Level"] = "text_300sbgt",                  // Row 25 Spell Level ✅
            ["SpellRow25_Name"] = "text_330jyte",                   // Row 25 Spell Name ✅
            ["SpellRow25_CastingTime"] = "text_360qkby",            // Row 25 Casting Time ✅
            ["SpellRow25_Range"] = "text_390xssq",                  // Row 25 Range ✅
            ["SpellRow25_Notes"] = "text_420inil",                  // Row 25 Notes ✅
            
            // Cantrips and Prepared Spells - Row 26 (All Fields)
            ["SpellRow26_Concentration"] = "checkbox_230hvot",      // Row 26 Concentration ✅
            ["SpellRow26_Ritual"] = "checkbox_231dimi",             // Row 26 Ritual ✅
            ["SpellRow26_Material"] = "checkbox_232djkv",           // Row 26 Material ✅
            ["SpellRow26_Level"] = "text_301jjge",                  // Row 26 Spell Level ✅
            ["SpellRow26_Name"] = "text_331jfme",                   // Row 26 Spell Name ✅
            ["SpellRow26_CastingTime"] = "text_361zakc",            // Row 26 Casting Time ✅
            ["SpellRow26_Range"] = "text_391ujd",                   // Row 26 Range ✅
            ["SpellRow26_Notes"] = "text_421kyai",                  // Row 26 Notes ✅
            
            // Cantrips and Prepared Spells - Row 27 (All Fields)
            ["SpellRow27_Concentration"] = "checkbox_233cbqw",      // Row 27 Concentration ✅
            ["SpellRow27_Ritual"] = "checkbox_234tauj",             // Row 27 Ritual ✅
            ["SpellRow27_Material"] = "checkbox_235ofts",           // Row 27 Material ✅
            ["SpellRow27_Level"] = "text_302sawd",                  // Row 27 Spell Level ✅
            ["SpellRow27_Name"] = "text_332xoqp",                   // Row 27 Spell Name ✅
            ["SpellRow27_CastingTime"] = "text_362lmcs",            // Row 27 Casting Time ✅
            ["SpellRow27_Range"] = "text_392jwxs",                  // Row 27 Range ✅
            ["SpellRow27_Notes"] = "text_422beva",                  // Row 27 Notes ✅
            
            // Cantrips and Prepared Spells - Row 28 (All Fields)
            ["SpellRow28_Concentration"] = "checkbox_236shat",      // Row 28 Concentration ✅
            ["SpellRow28_Ritual"] = "checkbox_237xylb",             // Row 28 Ritual ✅
            ["SpellRow28_Material"] = "checkbox_238izpl",           // Row 28 Material ✅
            ["SpellRow28_Level"] = "text_303lxiz",                  // Row 28 Spell Level ✅
            ["SpellRow28_Name"] = "text_333ekrb",                   // Row 28 Spell Name ✅
            ["SpellRow28_CastingTime"] = "text_363wmjv",            // Row 28 Casting Time ✅
            ["SpellRow28_Range"] = "text_393yigx",                  // Row 28 Range ✅
            ["SpellRow28_Notes"] = "text_423zfql",                  // Row 28 Notes ✅
            
            // Cantrips and Prepared Spells - Row 29 (All Fields)
            ["SpellRow29_Concentration"] = "checkbox_239oups",      // Row 29 Concentration ✅
            ["SpellRow29_Ritual"] = "checkbox_240szbj",             // Row 29 Ritual ✅
            ["SpellRow29_Material"] = "checkbox_241yngw",           // Row 29 Material ✅
            ["SpellRow29_Level"] = "text_304nwpr",                  // Row 29 Spell Level ✅
            ["SpellRow29_Name"] = "text_334zias",                   // Row 29 Spell Name ✅
            ["SpellRow29_CastingTime"] = "text_364cpss",            // Row 29 Casting Time ✅
            ["SpellRow29_Range"] = "text_394kcfb",                  // Row 29 Range ✅
            ["SpellRow29_Notes"] = "text_424reea",                  // Row 29 Notes ✅
            
            // Cantrips and Prepared Spells - Row 30 (All Fields)
            ["SpellRow30_Concentration"] = "checkbox_242wdhz",      // Row 30 Concentration ✅
            ["SpellRow30_Ritual"] = "checkbox_243yotb",             // Row 30 Ritual ✅
            ["SpellRow30_Material"] = "checkbox_244wrfp",           // Row 30 Material ✅
            ["SpellRow30_Level"] = "text_305gycb",                  // Row 30 Spell Level ✅
            ["SpellRow30_Name"] = "text_335znlw",                   // Row 30 Spell Name ✅
            ["SpellRow30_CastingTime"] = "text_365zoza",            // Row 30 Casting Time ✅
            ["SpellRow30_Range"] = "text_395sjli",                  // Row 30 Range ✅
            ["SpellRow30_Notes"] = "text_425sosz",                  // Row 30 Notes ✅
            

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