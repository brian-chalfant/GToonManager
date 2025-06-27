using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GToonManager.Models;
using GToonManager.Services;
using Newtonsoft.Json.Linq;

namespace GToonManager.ViewModels;

public class FeaturesViewModel : INotifyPropertyChanged
{
    private Character _character;
    private readonly ClassDataService _classDataService;
    private readonly ChoiceDataService _choiceDataService;
    private readonly FeatDataService _featDataService;

    public FeaturesViewModel(Character character, ClassDataService classDataService, ChoiceDataService choiceDataService, FeatDataService featDataService)
    {
        System.Diagnostics.Debug.WriteLine("[FeaturesViewModel] Constructor called");
        _character = character;
        _classDataService = classDataService;
        _choiceDataService = choiceDataService;
        _featDataService = featDataService;
        
        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Character: {character?.Name ?? "null"}");
        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Character class levels: {character?.ClassLevels?.Count ?? 0}");
        
        InitializeCommands();
        LoadChoiceDataAndChoices();
        
        // Subscribe to character changes
        _character.PropertyChanged += OnCharacterPropertyChanged;
        _character.ClassLevels.CollectionChanged += OnClassLevelsChanged;
    }

    // Collections for the 8 choice mechanics
    public ObservableCollection<SubclassChoice> SubclassChoices { get; } = new();
    public ObservableCollection<AbilityScoreImprovementChoice> AbilityScoreImprovementChoices { get; } = new();
    public ObservableCollection<CombatStyleChoice> CombatStyleChoices { get; } = new();
    public ObservableCollection<SkillsToolsChoice> SkillsToolsChoices { get; } = new();
    public ObservableCollection<CreatureTypeChoice> CreatureTypeChoices { get; } = new();
    public ObservableCollection<TerrainTypeChoice> TerrainTypeChoices { get; } = new();
    public ObservableCollection<SpellChoice> SpellChoices { get; } = new();
    public ObservableCollection<MagicalFeatureChoice> MagicalFeatureChoices { get; } = new();

    // Summary properties
    public int TotalChoices => SubclassChoices.Count + AbilityScoreImprovementChoices.Count + 
                              CombatStyleChoices.Count + SkillsToolsChoices.Count + 
                              CreatureTypeChoices.Count + TerrainTypeChoices.Count + 
                              SpellChoices.Count + MagicalFeatureChoices.Count;

    public int CompletedChoices => SubclassChoices.Count(c => c.IsCompleted) + 
                                  AbilityScoreImprovementChoices.Count(c => c.IsCompleted) + 
                                  CombatStyleChoices.Count(c => c.IsCompleted) + 
                                  SkillsToolsChoices.Count(c => c.IsCompleted) + 
                                  CreatureTypeChoices.Count(c => c.IsCompleted) + 
                                  TerrainTypeChoices.Count(c => c.IsCompleted) + 
                                  SpellChoices.Count(c => c.IsCompleted) + 
                                  MagicalFeatureChoices.Count(c => c.IsCompleted);

    public int RequiredChoices => SubclassChoices.Count(c => c.IsRequired) + 
                                 AbilityScoreImprovementChoices.Count(c => c.IsRequired) + 
                                 CombatStyleChoices.Count(c => c.IsRequired) + 
                                 SkillsToolsChoices.Count(c => c.IsRequired) + 
                                 CreatureTypeChoices.Count(c => c.IsRequired) + 
                                 TerrainTypeChoices.Count(c => c.IsRequired) + 
                                 SpellChoices.Count(c => c.IsRequired) + 
                                 MagicalFeatureChoices.Count(c => c.IsRequired);

    public int CompletedRequiredChoices => SubclassChoices.Count(c => c.IsRequired && c.IsCompleted) + 
                                          AbilityScoreImprovementChoices.Count(c => c.IsRequired && c.IsCompleted) + 
                                          CombatStyleChoices.Count(c => c.IsRequired && c.IsCompleted) + 
                                          SkillsToolsChoices.Count(c => c.IsRequired && c.IsCompleted) + 
                                          CreatureTypeChoices.Count(c => c.IsRequired && c.IsCompleted) + 
                                          TerrainTypeChoices.Count(c => c.IsRequired && c.IsCompleted) + 
                                          SpellChoices.Count(c => c.IsRequired && c.IsCompleted) + 
                                          MagicalFeatureChoices.Count(c => c.IsRequired && c.IsCompleted);

    public bool AllRequiredChoicesCompleted => CompletedRequiredChoices == RequiredChoices;

    public string ProgressSummary => $"{CompletedChoices}/{TotalChoices} choices completed ({CompletedRequiredChoices}/{RequiredChoices} required)";

    // Commands
    public ICommand RefreshChoicesCommand { get; private set; } = null!;
    public ICommand ResetAllChoicesCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        RefreshChoicesCommand = new RelayCommand(RefreshChoices);
        ResetAllChoicesCommand = new RelayCommand(ResetAllChoices);
    }

    private async void LoadChoiceDataAndChoices()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Starting to load choice data...");
            await _choiceDataService.LoadChoiceDataAsync();
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Choice data loaded successfully");
            
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Starting to load feat data...");
            await _featDataService.LoadFeatsAsync();
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Feat data loaded successfully");
            
            var availableTypes = _choiceDataService.GetAvailableChoiceTypes();
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Available choice types: {string.Join(", ", availableTypes)}");
            
            LoadAvailableChoices();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Error loading choice data: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Stack trace: {ex.StackTrace}");
            // Still try to load choices without detailed data
            LoadAvailableChoices();
        }
    }

    private void LoadAvailableChoices()
    {
        ClearAllChoices();

        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Loading choices for character with {_character.ClassLevels.Count} class levels");
        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Character name: {_character.Name}");

        // Load choices based on character's classes and levels
        foreach (var classLevel in _character.ClassLevels)
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Processing {classLevel.ClassName} level {classLevel.Level}");
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] CharacterClass is null: {classLevel.CharacterClass == null}");
            if (classLevel.CharacterClass != null)
            {
                System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] CharacterClass name: {classLevel.CharacterClass.Name}");
                System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Features count: {classLevel.CharacterClass.Features?.Count ?? 0}");
            }
            LoadChoicesForClassLevel(classLevel);
        }

        // Load choices from species and background
        LoadSpeciesChoices();
        LoadBackgroundChoices();

        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Total choices loaded: {TotalChoices}");
        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Subclass choices: {SubclassChoices.Count}");
        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] ASI choices: {AbilityScoreImprovementChoices.Count}");
        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Combat style choices: {CombatStyleChoices.Count}");

        OnPropertyChanged(nameof(TotalChoices));
        OnPropertyChanged(nameof(CompletedChoices));
        OnPropertyChanged(nameof(RequiredChoices));
        OnPropertyChanged(nameof(CompletedRequiredChoices));
        OnPropertyChanged(nameof(AllRequiredChoicesCompleted));
        OnPropertyChanged(nameof(ProgressSummary));
        
        // Apply any existing ASI choices to character
        ApplyASIChoicesToCharacter();
    }

    private void LoadChoicesForClassLevel(CharacterClassLevel classLevel)
    {
        var characterClass = classLevel.CharacterClass;
        if (characterClass == null) return;
        
        // Check for starting class benefits skill choices (level 1 only and only for first class)
        if (classLevel.Level >= 1 && _character.ClassLevels.FirstOrDefault() == classLevel)
        {
            LoadStartingClassSkillChoices(characterClass);
        }
        
        // Subclass selection is handled in Classes & Multiclassing tab, not here
        
        // Iterate through each level of this class
        for (int level = 1; level <= classLevel.Level; level++)
        {
            LoadChoicesForSpecificLevel(characterClass, level);
        }
    }

    private void LoadChoicesForSpecificLevel(CharacterClass characterClass, int level)
    {
        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Checking {characterClass.Name} level {level} for features");
        
        // Check class features data for choices at this level
        if (characterClass.Features?.ContainsKey(level) == true)
        {
            var features = characterClass.Features[level];
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found {features.Count} features at level {level}");
            
            foreach (var feature in features)
            {
                System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Processing feature: {feature.Name}");
                if (feature.Name == "Ability Score Improvement")
                {
                    System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] *** FOUND ASI FEATURE *** at level {level}");
                }
                ProcessFeatureForChoices(feature, characterClass.Name, level);
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] No features found at level {level} for {characterClass.Name}");
            if (characterClass.Features == null)
                System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Features dictionary is null for {characterClass.Name}");
            else
            {
                System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Available feature levels: {string.Join(", ", characterClass.Features.Keys)}");
            }
        }
    }

    private void ProcessFeatureForChoices(ClassFeature feature, string className, int level)
    {
        // 1. Subclass Choices
        if (IsSubclassChoice(feature))
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found subclass choice: {feature.Name}");
            var subclassChoice = CreateSubclassChoice(feature, className, level);
            SubclassChoices.Add(subclassChoice);
        }

        // 2. Ability Score Improvement
        if (IsAbilityScoreImprovement(feature))
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found ASI choice: {feature.Name}");
            var asiChoice = CreateAbilityScoreImprovementChoice(feature, className, level);
            AbilityScoreImprovementChoices.Add(asiChoice);
        }

        // 3. Combat Style (Fighting Style)
        if (IsCombatStyleChoice(feature))
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found combat style choice: {feature.Name}");
            var combatStyleChoice = CreateCombatStyleChoice(feature, className, level);
            CombatStyleChoices.Add(combatStyleChoice);
        }

        // 4. Skills/Tools (Expertise, etc.)
        if (IsSkillsToolsChoice(feature))
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found skills/tools choice: {feature.Name}");
            var skillsToolsChoice = CreateSkillsToolsChoice(feature, className, level);
            SkillsToolsChoices.Add(skillsToolsChoice);
        }

        // 5. Creature Type (Favored Enemy)
        if (IsCreatureTypeChoice(feature))
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found creature type choice: {feature.Name}");
            var creatureTypeChoice = CreateCreatureTypeChoice(feature, className, level);
            CreatureTypeChoices.Add(creatureTypeChoice);
        }

        // 6. Terrain Type (Natural Explorer)
        if (IsTerrainTypeChoice(feature))
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found terrain type choice: {feature.Name}");
            var terrainTypeChoice = CreateTerrainTypeChoice(feature, className, level);
            TerrainTypeChoices.Add(terrainTypeChoice);
        }

        // 7. Spell Choices
        if (IsSpellChoice(feature))
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found spell choice: {feature.Name}");
            var spellChoice = CreateSpellChoice(feature, className, level);
            SpellChoices.Add(spellChoice);
        }

        // 8. Magical Feature Choices
        if (IsMagicalFeatureChoice(feature))
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found magical feature choice: {feature.Name}");
            var magicalFeatureChoice = CreateMagicalFeatureChoice(feature, className, level);
            MagicalFeatureChoices.Add(magicalFeatureChoice);
        }
    }

    // Helper methods to identify feature types
    private bool IsSubclassChoice(ClassFeature feature)
    {
        return feature.Name.Contains("Archetype") || 
               feature.Name.Contains("College") || 
               feature.Name.Contains("Domain") || 
               feature.Name.Contains("Circle") || 
               feature.Name.Contains("Path") || 
               feature.Name.Contains("Tradition") || 
               feature.Name.Contains("Oath") || 
               feature.Name.Contains("Patron") || 
               feature.Name.Contains("Origin") || 
               feature.Name.Contains("Conclave");
    }

    private bool IsAbilityScoreImprovement(ClassFeature feature)
    {
        return feature.Name == "Ability Score Improvement";
    }

    private bool IsCombatStyleChoice(ClassFeature feature)
    {
        var isCombatStyle = feature.Name == "Fighting Style" || 
                           feature.Name == "Additional Fighting Style";
        
        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] IsCombatStyleChoice({feature.Name}): {isCombatStyle}");
        return isCombatStyle;
    }

    private bool IsSkillsToolsChoice(ClassFeature feature)
    {
        return feature.Name == "Expertise" || 
               feature.Name == "Student of War" ||
               feature.Description.Contains("choose") && 
               (feature.Description.Contains("skill") || feature.Description.Contains("tool"));
    }

    private bool IsCreatureTypeChoice(ClassFeature feature)
    {
        return feature.Name == "Favored Enemy" || 
               feature.Name == "Additional Favored Enemy";
    }

    private bool IsTerrainTypeChoice(ClassFeature feature)
    {
        return feature.Name == "Natural Explorer" || 
               feature.Name == "Additional Favored Terrain";
    }

    private bool IsSpellChoice(ClassFeature feature)
    {
        return feature.Name == "Spell Mastery" || 
               feature.Name == "Signature Spells" ||
               feature.Name.Contains("Mystic Arcanum") ||
               (feature.Description.Contains("choose") && feature.Description.Contains("spell"));
    }

    private bool IsMagicalFeatureChoice(ClassFeature feature)
    {
        return feature.Name == "Metamagic" || 
               feature.Name == "Eldritch Invocations" || 
               feature.Name == "Pact Boon" ||
               feature.Name == "Maneuvers";
    }

    // Create choice methods (simplified for now - would need full implementation)
    private SubclassChoice CreateSubclassChoice(ClassFeature feature, string className, int level)
    {
        return new SubclassChoice
        {
            Id = $"{className}_{level}_{feature.Name}",
            Name = feature.Name,
            Description = feature.Description,
            Level = level,
            ClassName = className,
            IsRequired = true
        };
    }

    private AbilityScoreImprovementChoice CreateAbilityScoreImprovementChoice(ClassFeature feature, string className, int level)
    {
        var choice = new AbilityScoreImprovementChoice
        {
            Id = $"{className}_{level}_{feature.Name}",
            Name = feature.Name,
            Description = feature.Description,
            Level = level,
            ClassName = className,
            IsRequired = true,
            CanChooseFeat = true,
            Character = _character
        };

        // Subscribe to completion status changes to update summary
        choice.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(AbilityScoreImprovementChoice.IsCompleted))
            {
                OnPropertyChanged(nameof(CompletedChoices));
                OnPropertyChanged(nameof(CompletedRequiredChoices));
                OnPropertyChanged(nameof(AllRequiredChoicesCompleted));
                OnPropertyChanged(nameof(ProgressSummary));
                
                // Apply ASI changes to character whenever completion status changes
                ApplyASIChoicesToCharacter();
            }
        };

        // Load available feats
        PopulateAvailableFeats(choice);
        
        // Initialize ability score selections for all six abilities
        InitializeAbilityScoreSelections(choice);

        return choice;
    }

    private void PopulateAvailableFeats(AbilityScoreImprovementChoice choice)
    {
        try
        {
            // Get all available feats (origin feats + general feats)
            var allFeats = _featDataService.GetAllFeats();
            
            // For now, include all feats - in the future we could filter by prerequisites
            foreach (var feat in allFeats)
            {
                choice.AvailableFeats.Add(feat);
            }
            
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Populated {choice.AvailableFeats.Count} feats for ASI choice");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Error populating feats: {ex.Message}");
        }
    }

    private void InitializeAbilityScoreSelections(AbilityScoreImprovementChoice choice)
    {
        // Create ability score selections for all abilities
        foreach (var ability in choice.AvailableAbilities)
        {
            var selection = new AbilityScoreSelection
            {
                AbilityScore = ability,
                Improvement = 0
            };
            
            // Set the parent choice for validation
            selection.SetParentChoice(choice);
            
            // Subscribe to property changes to update completion status
            selection.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(AbilityScoreSelection.Improvement))
                {
                    choice.UpdateCompletionStatus();
                    // Apply ASI changes to character whenever improvement values change
                    ApplyASIChoicesToCharacter();
                }
            };
            
            choice.AbilityScoreSelections.Add(selection);
        }
    }

    private CombatStyleChoice CreateCombatStyleChoice(ClassFeature feature, string className, int level)
    {
        var choice = new CombatStyleChoice
        {
            Id = $"{className}_{level}_{feature.Name}",
            Name = feature.Name,
            Description = feature.Description,
            Level = level,
            ClassName = className,
            IsRequired = true
        };

        PopulateCombatStyleOptions(choice, feature);
        return choice;
    }

    private SkillsToolsChoice CreateSkillsToolsChoice(ClassFeature feature, string className, int level)
    {
        var choice = new SkillsToolsChoice
        {
            Id = $"{className}_{level}_{feature.Name}",
            Name = feature.Name,
            Description = feature.Description,
            Level = level,
            ClassName = className,
            IsRequired = true,
            NumberToChoose = 2 // Default - would parse from feature
        };

        PopulateSkillsToolsOptions(choice, feature);
        return choice;
    }

    private CreatureTypeChoice CreateCreatureTypeChoice(ClassFeature feature, string className, int level)
    {
        var choice = new CreatureTypeChoice
        {
            Id = $"{className}_{level}_{feature.Name}",
            Name = feature.Name,
            Description = feature.Description,
            Level = level,
            ClassName = className,
            IsRequired = true
        };

        PopulateCreatureTypeOptions(choice);
        return choice;
    }

    private TerrainTypeChoice CreateTerrainTypeChoice(ClassFeature feature, string className, int level)
    {
        var choice = new TerrainTypeChoice
        {
            Id = $"{className}_{level}_{feature.Name}",
            Name = feature.Name,
            Description = feature.Description,
            Level = level,
            ClassName = className,
            IsRequired = true
        };

        PopulateTerrainTypeOptions(choice);
        return choice;
    }

    private SpellChoice CreateSpellChoice(ClassFeature feature, string className, int level)
    {
        return new SpellChoice
        {
            Id = $"{className}_{level}_{feature.Name}",
            Name = feature.Name,
            Description = feature.Description,
            Level = level,
            ClassName = className,
            IsRequired = true,
            NumberToChoose = 1 // Default - would parse from feature
        };
    }

    private MagicalFeatureChoice CreateMagicalFeatureChoice(ClassFeature feature, string className, int level)
    {
        var choice = new MagicalFeatureChoice
        {
            Id = $"{className}_{level}_{feature.Name}",
            Name = feature.Name,
            Description = feature.Description,
            Level = level,
            ClassName = className,
            IsRequired = true,
            NumberToChoose = GetNumberToChooseForMagicalFeature(feature),
            FeatureType = GetMagicalFeatureType(feature)
        };

        // Populate available features from choice data
        PopulateMagicalFeatureOptions(choice, feature);

        return choice;
    }

    private int GetNumberToChooseForMagicalFeature(ClassFeature feature)
    {
        if (feature.Name == "Metamagic") return 2;
        if (feature.Name == "Eldritch Invocations") return 2;
        if (feature.Name == "Pact Boon") return 1;
        if (feature.Name == "Maneuvers") return 3;
        return 1;
    }

    private string GetMagicalFeatureType(ClassFeature feature)
    {
        if (feature.Name == "Metamagic") return "Metamagic";
        if (feature.Name == "Eldritch Invocations") return "Eldritch Invocation";
        if (feature.Name == "Pact Boon") return "Pact Boon";
        if (feature.Name == "Maneuvers") return "Battle Master Maneuver";
        return "Magical Feature";
    }

    private void PopulateMagicalFeatureOptions(MagicalFeatureChoice choice, ClassFeature feature)
    {
        try
        {
            List<ChoiceOption> options = new();

            if (feature.Name == "Metamagic" && _choiceDataService.HasChoiceData("metamagic"))
            {
                options = _choiceDataService.GetMetamagicOptions();
            }
            else if (feature.Name.Contains("Eldritch") && _choiceDataService.HasChoiceData("eldritch_invocations"))
            {
                var characterLevel = _character.ClassLevels.Sum(cl => cl.Level);
                var knownFeatures = GetKnownFeatures();
                options = _choiceDataService.GetFilteredChoiceOptions("eldritch_invocations", characterLevel, knownFeatures);
            }
            else if (feature.Name.Contains("Pact") && _choiceDataService.HasChoiceData("pact_boons"))
            {
                options = _choiceDataService.GetPactBoons();
            }
            else if (feature.Name == "Maneuvers" && _choiceDataService.HasChoiceData("battle_master_maneuvers"))
            {
                options = _choiceDataService.GetBattleMasterManeuvers();
            }

            // Convert ChoiceOptions to MagicalFeatureOptions
            foreach (var option in options)
            {
                choice.AvailableFeatures.Add(new MagicalFeatureOption
                {
                    Name = option.Name,
                    Description = option.Description,
                    Prerequisites = option.Prerequisites,
                    Source = option.Source,
                    Cost = option.Cost
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Error populating magical feature options: {ex.Message}");
        }
    }

    private void PopulateCombatStyleOptions(CombatStyleChoice choice, ClassFeature feature)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] PopulateCombatStyleOptions called for feature: {feature.Name}");
            
            // Extract fighting styles from the feature mechanics if available
            if (feature.Mechanics is JObject mechanicsObj)
            {
                System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Feature has JObject mechanics");
                var type = mechanicsObj["type"]?.ToString();
                System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Mechanics type: {type}");
                
                if (type == "choice" && mechanicsObj["options"] is JArray optionsArray)
                {
                    System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found {optionsArray.Count} combat style options");
                    foreach (var option in optionsArray)
                    {
                        var name = option["name"]?.ToString() ?? "";
                        var description = option["description"]?.ToString() ?? "";
                        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Adding combat style: {name}");
                        
                        choice.AvailableStyles.Add(new CombatStyleOption
                        {
                            Name = name,
                            Description = description,
                            Effect = description // Could be enhanced with specific effects parsing
                        });
                    }
                    System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Total combat styles added: {choice.AvailableStyles.Count}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Not a choice type or no options array found");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Feature mechanics is not JObject: {feature.Mechanics?.GetType()}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Error populating combat style options: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Stack trace: {ex.StackTrace}");
        }
    }

    private void PopulateCreatureTypeOptions(CreatureTypeChoice choice)
    {
        try
        {
            if (_choiceDataService.HasChoiceData("creature_types"))
            {
                var options = _choiceDataService.GetCreatureTypes();
                foreach (var option in options)
                {
                    choice.AvailableCreatureTypes.Add(new CreatureTypeOption
                    {
                        Name = option.Name,
                        Description = option.Description,
                        IsHumanoidChoice = option.Name == "Humanoids"
                    });
                }

                choice.CanSelectTwoHumanoidRaces = true; // For humanoid creature type
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Error populating creature type options: {ex.Message}");
        }
    }

    private void PopulateTerrainTypeOptions(TerrainTypeChoice choice)
    {
        try
        {
            if (_choiceDataService.HasChoiceData("terrain_types"))
            {
                var options = _choiceDataService.GetTerrainTypes();
                foreach (var option in options)
                {
                    choice.AvailableTerrainTypes.Add(new TerrainTypeOption
                    {
                        Name = option.Name,
                        Description = option.Description,
                        Benefits = option.Benefits
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Error populating terrain type options: {ex.Message}");
        }
    }

    private void PopulateSkillsToolsOptions(SkillsToolsChoice choice, ClassFeature feature)
    {
        try
        {
            // Handle artisan tools choice
            if (feature.Name.Contains("Student of War") && _choiceDataService.HasChoiceData("artisan_tools"))
            {
                var options = _choiceDataService.GetArtisanTools();
                foreach (var option in options)
                {
                    choice.AvailableTools.Add(option.Name);
                }
            }
            
            // Add standard skills if this is a skill choice
            if (feature.Mechanics is JObject mechanicsObj)
            {
                var type = mechanicsObj["type"]?.ToString();
                if (type == "choice" && feature.Name.ToLower().Contains("skill"))
                {
                    // Add standard D&D skills
                    var skills = new[] { "Acrobatics", "Animal Handling", "Arcana", "Athletics", "Deception", 
                                       "History", "Insight", "Intimidation", "Investigation", "Medicine", 
                                       "Nature", "Perception", "Performance", "Persuasion", "Religion", 
                                       "Sleight of Hand", "Stealth", "Survival" };
                    
                    foreach (var skill in skills)
                    {
                        choice.AvailableSkills.Add(skill);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Error populating skills/tools options: {ex.Message}");
        }
    }

    private List<string> GetKnownFeatures()
    {
        var knownFeatures = new List<string>();
        
        // Add features from all class levels
        foreach (var classLevel in _character.ClassLevels)
        {
            var characterClass = classLevel.CharacterClass;
            if (characterClass?.Features != null)
            {
                for (int level = 1; level <= classLevel.Level; level++)
                {
                    if (characterClass.Features.ContainsKey(level))
                    {
                        foreach (var feature in characterClass.Features[level])
                        {
                            knownFeatures.Add(feature.Name);
                        }
                    }
                }
            }
        }

        return knownFeatures;
    }

    private void LoadSpeciesChoices()
    {
        // Load any choices from species (less common, but possible)
    }

    private void LoadStartingClassSkillChoices(CharacterClass characterClass)
    {
        // Check if the class has starting skill proficiency choices
        if (characterClass.StartingClassBenefits?.SkillProficiencies?.HasChoices == true)
        {
            var skillProficiencies = characterClass.StartingClassBenefits.SkillProficiencies;
            System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Found starting class skill choice for {characterClass.Name}: Choose {skillProficiencies.ChooseCount} from {skillProficiencies.AvailableSkills.Count} skills");
            
            var skillChoice = new SkillsToolsChoice
            {
                Id = $"{characterClass.Name}_Starting_Skills",
                Name = "Starting Skill Proficiencies",
                Description = $"Choose {skillProficiencies.ChooseCount} skill proficiencies for your {characterClass.Name} class",
                Level = 1,
                ClassName = characterClass.Name,
                IsRequired = true,
                NumberToChoose = skillProficiencies.ChooseCount
            };
            
            // Populate available skills using the new SkillOption system
            foreach (var skill in skillProficiencies.AvailableSkills)
            {
                var skillOption = new SkillOption { Name = skill };
                skillOption.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(SkillOption.IsSelected))
                    {
                        skillChoice.UpdateCompletionStatus();
                    }
                };
                skillChoice.SkillOptions.Add(skillOption);
            }
            
            SkillsToolsChoices.Add(skillChoice);
        }
    }

    private void LoadBackgroundChoices()
    {
        // Load any choices from background (less common, but possible)
    }

    private void ClearAllChoices()
    {
        SubclassChoices.Clear();
        AbilityScoreImprovementChoices.Clear();
        CombatStyleChoices.Clear();
        SkillsToolsChoices.Clear();
        CreatureTypeChoices.Clear();
        TerrainTypeChoices.Clear();
        SpellChoices.Clear();
        MagicalFeatureChoices.Clear();
    }

    private void RefreshChoices()
    {
        LoadAvailableChoices();
    }

    private void ResetAllChoices()
    {
        // Reset all choices to their initial state
        foreach (var choice in SubclassChoices)
        {
            choice.SelectedSubclass = null;
            choice.LegacySelectedSubclass = null;
        }
        
        foreach (var choice in AbilityScoreImprovementChoices)
        {
            choice.ChoiceType = ASIChoiceType.AbilityScoreIncrease;
            choice.SelectedFeat = null;
            foreach (var selection in choice.AbilityScoreSelections)
            {
                selection.Improvement = 0;
            }
        }
        
        foreach (var choice in CombatStyleChoices)
        {
            choice.SelectedStyle = null;
        }
        
        foreach (var choice in SkillsToolsChoices)
        {
            choice.SelectedSkills.Clear();
            choice.SelectedTools.Clear();
        }
        
        foreach (var choice in CreatureTypeChoices)
        {
            choice.SelectedCreatureType = null;
        }
        
        foreach (var choice in TerrainTypeChoices)
        {
            choice.SelectedTerrainType = null;
        }
        
        foreach (var choice in SpellChoices)
        {
            choice.SelectedSpells.Clear();
        }
        
        foreach (var choice in MagicalFeatureChoices)
        {
            choice.SelectedFeatures.Clear();
        }
    }

    // Apply completed ASI choices to the character's ASI properties
    private void ApplyASIChoicesToCharacter()
    {
        // Reset character ASI values
        _character.StrengthASI = 0;
        _character.DexterityASI = 0;
        _character.ConstitutionASI = 0;
        _character.IntelligenceASI = 0;
        _character.WisdomASI = 0;
        _character.CharismaASI = 0;

        // Apply improvements from all completed ASI choices
        foreach (var asiChoice in AbilityScoreImprovementChoices.Where(c => c.IsCompleted && c.IsChoosingAbilityScores))
        {
            foreach (var selection in asiChoice.AbilityScoreSelections.Where(s => s.Improvement > 0))
            {
                switch (selection.AbilityScore.ToLowerInvariant())
                {
                    case "strength":
                        _character.StrengthASI = (_character.StrengthASI ?? 0) + selection.Improvement;
                        break;
                    case "dexterity":
                        _character.DexterityASI = (_character.DexterityASI ?? 0) + selection.Improvement;
                        break;
                    case "constitution":
                        _character.ConstitutionASI = (_character.ConstitutionASI ?? 0) + selection.Improvement;
                        break;
                    case "intelligence":
                        _character.IntelligenceASI = (_character.IntelligenceASI ?? 0) + selection.Improvement;
                        break;
                    case "wisdom":
                        _character.WisdomASI = (_character.WisdomASI ?? 0) + selection.Improvement;
                        break;
                    case "charisma":
                        _character.CharismaASI = (_character.CharismaASI ?? 0) + selection.Improvement;
                        break;
                }
            }
        }

        System.Diagnostics.Debug.WriteLine($"[FeaturesViewModel] Applied ASI to character - STR: {_character.StrengthASI}, DEX: {_character.DexterityASI}, CON: {_character.ConstitutionASI}, INT: {_character.IntelligenceASI}, WIS: {_character.WisdomASI}, CHA: {_character.CharismaASI}");
    }

    private void OnCharacterPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Character.Species) || 
            e.PropertyName == nameof(Character.Background))
        {
            LoadAvailableChoices();
        }
    }

    private void OnClassLevelsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        LoadAvailableChoices();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 