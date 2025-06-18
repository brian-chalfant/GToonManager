using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using GToonManager.Models;
using GToonManager.Services;
using GToonManager.Views;
using Microsoft.Win32;
using System.Text;

namespace GToonManager.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private Character _currentCharacter;
    private string _statusMessage = "Ready";
    private CharacterClass? _selectedClassToAdd;
    private int _selectedLevelToAdd = 1;
    private Background? _selectedBackground;
    private ImprovementOption? _selectedBackgroundImprovementOption;
    private readonly CharacterFileService _characterFileService;
    private readonly SpeciesDataService _speciesDataService;
    private readonly ClassDataService _classDataService;
    private readonly BackgroundDataService _backgroundDataService;
    private string? _currentFilePath;
    private Settings _settings;
    private StandardArrayViewModel? _standardArrayViewModel;
    private PointBuyViewModel? _pointBuyViewModel;
    private RollingViewModel? _rollingViewModel;
    private FreeEntryViewModel? _freeEntryViewModel;
    private FeaturesViewModel? _featuresViewModel;

    // --- Features & Traits Tab (for PDF export only) ---
    private ObservableCollection<FeatureTraitViewModel> _featuresAndTraits = new();
    public ObservableCollection<FeatureTraitViewModel> FeaturesAndTraits
    {
        get => _featuresAndTraits;
        set { _featuresAndTraits = value; OnPropertyChanged(nameof(FeaturesAndTraits)); }
    }

    private bool _featuresTabEnabled;
    public bool FeaturesTabEnabled
    {
        get => _featuresTabEnabled;
        set { _featuresTabEnabled = value; OnPropertyChanged(nameof(FeaturesTabEnabled)); }
    }

    public MainViewModel()
    {
        _characterFileService = new CharacterFileService();
        _speciesDataService = new SpeciesDataService();
        _classDataService = new ClassDataService();
        _backgroundDataService = new BackgroundDataService();
        
        // Load settings from file or create defaults
        _settings = SettingsService.LoadSettings();
        
        _currentCharacter = new Character
        {
            Name = "New Character",
            PlayerName = ""
        };
        
        // Set settings reference for calculations
        _currentCharacter.SetSettings(_settings);
        
        // Apply initial theme from loaded settings
        ThemeService.ApplyTheme(_settings.Theme);
        
        // Subscribe to character property changes
        _currentCharacter.PropertyChanged += CurrentCharacter_PropertyChanged;
        // Subscribe to class level changes for tab enablement
        _currentCharacter.ClassLevels.CollectionChanged += CurrentCharacter_ClassLevelsChanged;

        // Ensure UI updates when BackgroundAbilityScoreSelections changes
        BackgroundAbilityScoreSelections.CollectionChanged += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"BackgroundAbilityScoreSelections.CollectionChanged fired: Action={e.Action}");
            OnPropertyChanged(nameof(BackgroundAbilityScoreSelections));
            System.Diagnostics.Debug.WriteLine("Raised PropertyChanged for BackgroundAbilityScoreSelections");
        };

        InitializeData();
        InitializeCommands();
        
        // Create the initial ViewModel for the default method
        CreateViewModelForCurrentMethod();
        
        // Initialize Features ViewModel
        InitializeFeaturesViewModel();
    }

    public Character CurrentCharacter
    {
        get => _currentCharacter;
        set
        {
            // Unsubscribe from old character
            if (_currentCharacter != null)
            {
                _currentCharacter.PropertyChanged -= CurrentCharacter_PropertyChanged;
                _currentCharacter.ClassLevels.CollectionChanged -= CurrentCharacter_ClassLevelsChanged;
            }
            
            _currentCharacter = value;
            OnPropertyChanged(nameof(CurrentCharacter));
            
            // Subscribe to new character's property changes and set settings
            if (_currentCharacter != null)
            {
                _currentCharacter.SetSettings(_settings);
                _currentCharacter.PropertyChanged += CurrentCharacter_PropertyChanged;
                _currentCharacter.ClassLevels.CollectionChanged += CurrentCharacter_ClassLevelsChanged;
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    public ObservableCollection<Species> Species { get; } = new();
    public ObservableCollection<CharacterClass> Classes { get; } = new();
    public ObservableCollection<Background> Backgrounds { get; } = new();

    public Settings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            OnPropertyChanged(nameof(Settings));
            
            // Update current character with new settings
            if (_currentCharacter != null)
            {
                _currentCharacter.SetSettings(_settings);
            }
        }
    }

    public StandardArrayViewModel? StandardArrayViewModel
    {
        get => _standardArrayViewModel;
        set
        {
            _standardArrayViewModel = value;
            OnPropertyChanged(nameof(StandardArrayViewModel));
        }
    }

    public PointBuyViewModel? PointBuyViewModel
    {
        get => _pointBuyViewModel;
        set
        {
            _pointBuyViewModel = value;
            OnPropertyChanged(nameof(PointBuyViewModel));
        }
    }

    public RollingViewModel? RollingViewModel
    {
        get => _rollingViewModel;
        set
        {
            // Unsubscribe from old rolling view model
            if (_rollingViewModel != null)
            {
                _rollingViewModel.ScoresApplied -= OnRollingScoresApplied;
            }
            
            _rollingViewModel = value;
            
            // Subscribe to new rolling view model
            if (_rollingViewModel != null)
            {
                _rollingViewModel.ScoresApplied += OnRollingScoresApplied;
            }
            
            OnPropertyChanged(nameof(RollingViewModel));
        }
    }

    public FreeEntryViewModel? FreeEntryViewModel
    {
        get => _freeEntryViewModel;
        set
        {
            _freeEntryViewModel = value;
            OnPropertyChanged(nameof(FreeEntryViewModel));
        }
    }

    public FeaturesViewModel? FeaturesViewModel
    {
        get => _featuresViewModel;
        set
        {
            _featuresViewModel = value;
            OnPropertyChanged(nameof(FeaturesViewModel));
        }
    }

    public bool IsStandardArrayMode => Settings.AbilityScoreMethod == AbilityScoreGenerationMethod.StandardArray;
    public bool IsPointBuyMode => Settings.AbilityScoreMethod == AbilityScoreGenerationMethod.PointBuy;
    public bool IsRollingMode => Settings.AbilityScoreMethod == AbilityScoreGenerationMethod.FourD6DropLowest;
    public bool IsFreeEntryMode => Settings.AbilityScoreMethod == AbilityScoreGenerationMethod.FreeEntry;

    public CharacterClass? SelectedClassToAdd
    {
        get => _selectedClassToAdd;
        set
        {
            _selectedClassToAdd = value;
            OnPropertyChanged(nameof(SelectedClassToAdd));
            OnPropertyChanged(nameof(HasSkillChoices));
            OnPropertyChanged(nameof(AvailableSkillChoices));
            OnPropertyChanged(nameof(SkillChoicePrompt));
            
            // Clear previous selections when class changes
            SelectedSkills.Clear();
        }
    }

    public int SelectedLevelToAdd
    {
        get => _selectedLevelToAdd;
        set
        {
            _selectedLevelToAdd = Math.Max(1, Math.Min(20, value));
            OnPropertyChanged(nameof(SelectedLevelToAdd));
        }
    }

    // Skill selection properties
    public bool HasSkillChoices => SelectedClassToAdd?.SkillChoices?.HasChoices == true;
    
    public List<string> AvailableSkillChoices => SelectedClassToAdd?.SkillChoices?.AvailableSkills ?? new List<string>();
    
    public string SkillChoicePrompt => SelectedClassToAdd?.SkillChoices != null 
        ? $"Choose {SelectedClassToAdd.SkillChoices.ChooseCount} skills from the list below:"
        : "";
    
    public ObservableCollection<SkillSelectionItem> SelectedSkills { get; } = new();

    public int MaxSkillSelections => SelectedClassToAdd?.SkillChoices?.ChooseCount ?? 0;
    
    public bool CanAddMoreSkills => SelectedSkills.Count < MaxSkillSelections;
    
    public bool HasRequiredSkillSelections => !HasSkillChoices || SelectedSkills.Count == MaxSkillSelections;

    // Background ability score selection properties
    public Background? SelectedBackground
    {
        get => _selectedBackground;
        set
        {
            _selectedBackground = value;
            OnPropertyChanged(nameof(SelectedBackground));
            OnPropertyChanged(nameof(HasBackgroundAbilityScoreOptions));
            OnPropertyChanged(nameof(BackgroundAbilityScoreOptions));
            OnPropertyChanged(nameof(AvailableAbilityScoresForBackground));
            
            // Clear previous selections when background changes
            SelectedBackgroundImprovementOption = null;
            BackgroundAbilityScoreSelections.Clear();
        }
    }

    public ImprovementOption? SelectedBackgroundImprovementOption
    {
        get => _selectedBackgroundImprovementOption;
        set
        {
            _selectedBackgroundImprovementOption = value;
            OnPropertyChanged(nameof(SelectedBackgroundImprovementOption));
            OnPropertyChanged(nameof(HasBackgroundImprovementOption));
            OnPropertyChanged(nameof(BackgroundImprovementOptionDescription));
            OnPropertyChanged(nameof(CanApplyBackgroundSelection));
            OnPropertyChanged(nameof(IsUniformDistribution));
            
            // Clear selections when option changes
            BackgroundAbilityScoreSelections.Clear();
            
            // Auto-apply uniform distribution if selected
            if (IsUniformDistribution)
            {
                AutoApplyUniformDistribution();
            }
        }
    }

    public bool HasBackgroundAbilityScoreOptions => SelectedBackground?.AbilityScoreImprovement?.ImprovementOptions?.Count > 0;
    
    public List<ImprovementOption> BackgroundAbilityScoreOptions => SelectedBackground?.AbilityScoreImprovement?.ImprovementOptions ?? new List<ImprovementOption>();
    
    public List<string> AvailableAbilityScoresForBackground => SelectedBackground?.AbilityScoreImprovement?.AbilityScores ?? new List<string>();
    
    public bool HasBackgroundImprovementOption => SelectedBackgroundImprovementOption != null;
    
    public string BackgroundImprovementOptionDescription => SelectedBackgroundImprovementOption?.Description ?? "";
    
    public ObservableCollection<BackgroundAbilityScoreSelection> BackgroundAbilityScoreSelections { get; } = new();
    
    public bool CanApplyBackgroundSelection => HasBackgroundImprovementOption && 
                                               (ValidateBackgroundAbilityScoreSelections() || 
                                                (BackgroundAbilityScoreSelections.Count == 0 && IsUniformDistribution));

    public bool IsUniformDistribution => SelectedBackgroundImprovementOption?.Distributions?.Count == 1 && 
                                        SelectedBackgroundImprovementOption.Distributions[0].Count > 1;

    // Property to check if background abilities have been applied
    public bool HasAppliedBackgroundAbilities => CurrentCharacter.BackgroundAbilityScoreChoice != null;

    // Commands
    public ICommand NewCharacterCommand { get; private set; } = null!;
    public ICommand LoadCharacterCommand { get; private set; } = null!;
    public ICommand SaveCharacterCommand { get; private set; } = null!;
    public ICommand SaveCharacterAsCommand { get; private set; } = null!;
    public ICommand ExportToHtmlCommand { get; private set; } = null!;
    public ICommand ExportToPdfCommand { get; private set; } = null!;
    public ICommand DiscoverPdfFieldsCommand { get; private set; } = null!;
    public ICommand CreateTestPdfCommand { get; private set; } = null!;
    public ICommand AnalyzePdfCommand { get; private set; } = null!;
    public ICommand ShowSettingsCommand { get; private set; } = null!;
    public ICommand ExitApplicationCommand { get; private set; } = null!;
    public ICommand ShowAboutCommand { get; private set; } = null!;
    public ICommand OpenStandardArrayCommand { get; private set; } = null!;
    public ICommand OpenPointBuyCommand { get; private set; } = null!;
    public ICommand OpenRollingCommand { get; private set; } = null!;
    public ICommand OpenFreeEntryCommand { get; private set; } = null!;
    
    // Multiclass commands
    public ICommand AddClassCommand { get; private set; } = null!;
    public ICommand RemoveClassCommand { get; private set; } = null!;
    public ICommand AddSkillCommand { get; private set; } = null!;
    public ICommand RemoveSkillCommand { get; private set; } = null!;
    public ICommand ChooseSubclassCommand { get; private set; } = null!;
    
    // Background commands
    public ICommand ApplyBackgroundCommand { get; private set; } = null!;
    public ICommand AddBackgroundAbilityScoreCommand { get; private set; } = null!;
    public ICommand RemoveBackgroundAbilityScoreCommand { get; private set; } = null!;
    public ICommand ManageASICommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        NewCharacterCommand = new RelayCommand(NewCharacter);
        LoadCharacterCommand = new RelayCommand(LoadCharacter);
        SaveCharacterCommand = new RelayCommand(SaveCharacter);
        SaveCharacterAsCommand = new RelayCommand(SaveCharacterAs);
        ExportToHtmlCommand = new RelayCommand(ExportToHtml);
        ExportToPdfCommand = new RelayCommand(ExportToPdf);
        DiscoverPdfFieldsCommand = new RelayCommand(DiscoverPdfFields);
        CreateTestPdfCommand = new RelayCommand(CreateTestPdf);
        AnalyzePdfCommand = new RelayCommand(AnalyzePdf);
        ShowSettingsCommand = new RelayCommand(ShowSettings);
        ExitApplicationCommand = new RelayCommand(ExitApplication);
        ShowAboutCommand = new RelayCommand(ShowAbout);
        OpenStandardArrayCommand = new RelayCommand(OpenStandardArray);
        OpenPointBuyCommand = new RelayCommand(OpenPointBuy);
        OpenRollingCommand = new RelayCommand(OpenRolling);
        OpenFreeEntryCommand = new RelayCommand(OpenFreeEntry);
        
        // Multiclass commands
        AddClassCommand = new RelayCommand(AddClass);
        RemoveClassCommand = new RelayCommand(RemoveClass, CanRemoveClass);
        AddSkillCommand = new RelayCommand(AddSkill);
        RemoveSkillCommand = new RelayCommand(RemoveSkill);
        ChooseSubclassCommand = new RelayCommand(ChooseSubclass);
        
        // Background commands
        ApplyBackgroundCommand = new RelayCommand(ApplyBackground);
        AddBackgroundAbilityScoreCommand = new RelayCommand(AddBackgroundAbilityScore);
        RemoveBackgroundAbilityScoreCommand = new RelayCommand(RemoveBackgroundAbilityScore);
        
        // ASI commands
        ManageASICommand = new RelayCommand(ManageASI, CanManageASI);
    }

    private async void InitializeData()
    {
                    await LoadSpeciesAsync();
        await LoadClassesAsync();
        await LoadBackgroundsAsync();
    }

    private async Task LoadSpeciesAsync()
    {
        try
        {
            var species = await _speciesDataService.LoadAllSpeciesAsync();
            Species.Clear();
            foreach (var speciesEntry in species)
            {
                Species.Add(speciesEntry);
            }
            StatusMessage = $"Loaded {species.Count} species from data files";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading species: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error loading species: {ex}");
            
            // Fallback to basic species if loading fails
            Species.Add(new Species { Name = "Human", Description = "Versatile and ambitious" });
            Species.Add(new Species { Name = "Elf", Description = "Graceful and long-lived" });
            Species.Add(new Species { Name = "Dwarf", Description = "Hardy and traditional" });
            Species.Add(new Species { Name = "Halfling", Description = "Small and brave" });
        }
    }

    private async Task LoadClassesAsync()
    {
        try
        {
            var classes = await _classDataService.LoadAllClassesAsync();
            Classes.Clear();
            foreach (var characterClass in classes)
            {
                Classes.Add(characterClass);
            }
            StatusMessage = $"Loaded {classes.Count} classes from data files";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading classes: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error loading classes: {ex}");
            
            // Fallback to basic classes if loading fails
            Classes.Add(new CharacterClass { Name = "Fighter", Description = "Master of weapons and armor", HitDie = 10, PrimaryAbility = "Strength or Dexterity" });
            Classes.Add(new CharacterClass { Name = "Wizard", Description = "Master of arcane magic", HitDie = 6, PrimaryAbility = "Intelligence" });
            Classes.Add(new CharacterClass { Name = "Rogue", Description = "Master of stealth and skill", HitDie = 8, PrimaryAbility = "Dexterity" });
            Classes.Add(new CharacterClass { Name = "Cleric", Description = "Divine spellcaster", HitDie = 8, PrimaryAbility = "Wisdom" });
            Classes.Add(new CharacterClass { Name = "Ranger", Description = "Guardian of the wilderness", HitDie = 10, PrimaryAbility = "Dexterity and Wisdom" });
            Classes.Add(new CharacterClass { Name = "Paladin", Description = "Holy warrior", HitDie = 10, PrimaryAbility = "Strength and Charisma" });
            Classes.Add(new CharacterClass { Name = "Barbarian", Description = "Fierce warrior", HitDie = 12, PrimaryAbility = "Strength" });
            Classes.Add(new CharacterClass { Name = "Bard", Description = "Master of performance and magic", HitDie = 8, PrimaryAbility = "Charisma" });
            Classes.Add(new CharacterClass { Name = "Druid", Description = "Guardian of nature", HitDie = 8, PrimaryAbility = "Wisdom" });
            Classes.Add(new CharacterClass { Name = "Monk", Description = "Master of martial arts", HitDie = 8, PrimaryAbility = "Dexterity and Wisdom" });
            Classes.Add(new CharacterClass { Name = "Sorcerer", Description = "Innate magical power", HitDie = 6, PrimaryAbility = "Charisma" });
            Classes.Add(new CharacterClass { Name = "Warlock", Description = "Pact magic user", HitDie = 8, PrimaryAbility = "Charisma" });
        }
        
        // After classes are loaded, update any existing StandardArrayViewModel
        UpdateStandardArrayViewModelClasses();
    }
    
    private void UpdateStandardArrayViewModelClasses()
    {
        if (StandardArrayViewModel != null)
        {
            UpdateStandardArrayViewModelClasses(StandardArrayViewModel);
        }
    }
    
    private void UpdateStandardArrayViewModelClasses(StandardArrayViewModel viewModel)
    {
        viewModel.Classes.Clear();
        foreach (var characterClass in Classes)
        {
            viewModel.Classes.Add(characterClass);
        }
    }

    private async Task LoadBackgroundsAsync()
    {
        try
        {
            var backgrounds = await _backgroundDataService.LoadAllBackgroundsAsync();
            Backgrounds.Clear();
            foreach (var background in backgrounds)
            {
                Backgrounds.Add(background);
            }
            StatusMessage = $"Loaded {backgrounds.Count} backgrounds from data files";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading backgrounds: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error loading backgrounds: {ex}");
            
            // Fallback to basic backgrounds if loading fails
            Backgrounds.Add(new Background { Name = "Acolyte", Description = "Religious servant" });
            Backgrounds.Add(new Background { Name = "Criminal", Description = "Life of crime" });
            Backgrounds.Add(new Background { Name = "Folk Hero", Description = "Champion of the people" });
            Backgrounds.Add(new Background { Name = "Noble", Description = "Upper class upbringing" });
            Backgrounds.Add(new Background { Name = "Sage", Description = "Scholar and researcher" });
            Backgrounds.Add(new Background { Name = "Soldier", Description = "Military service" });
            Backgrounds.Add(new Background { Name = "Charlatan", Description = "Master of deception" });
            Backgrounds.Add(new Background { Name = "Entertainer", Description = "Performer" });
            Backgrounds.Add(new Background { Name = "Guild Artisan", Description = "Skilled craftsperson" });
            Backgrounds.Add(new Background { Name = "Hermit", Description = "Secluded contemplative" });
            Backgrounds.Add(new Background { Name = "Outlander", Description = "Wilderness dweller" });
            Backgrounds.Add(new Background { Name = "Sailor", Description = "Seafaring adventurer" });
        }
    }

    private void NewCharacter()
    {
        CurrentCharacter = new Character
        {
            Name = "New Character",
            PlayerName = ""
        };
        StatusMessage = "New character created";
    }

    private async void LoadCharacter()
    {
        try
        {
            StatusMessage = "Loading character...";
            
            var (character, filePath) = await _characterFileService.LoadCharacterAsync();
            if (character != null)
            {
                CurrentCharacter = character;
                _currentFilePath = filePath;
                StatusMessage = $"Loaded character: {character.Name}";
            }
            else
            {
                StatusMessage = "Load operation cancelled";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading character: {ex.Message}";
            MessageBox.Show($"Failed to load character:\n{ex.Message}", "Load Error", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void SaveCharacter()
    {
        try
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                // No current file path, use Save As
                SaveCharacterAs();
                return;
            }

            StatusMessage = "Saving character...";
            var success = await _characterFileService.SaveCharacterToFileAsync(CurrentCharacter, _currentFilePath);
            
            if (success)
            {
                StatusMessage = $"Saved character: {CurrentCharacter.Name}";
            }
            else
            {
                StatusMessage = "Failed to save character";
                MessageBox.Show("Failed to save character. Please try Save As instead.", "Save Error", 
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving character: {ex.Message}";
            MessageBox.Show($"Failed to save character:\n{ex.Message}", "Save Error", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void SaveCharacterAs()
    {
        try
        {
            StatusMessage = "Saving character...";
            
            var (success, filePath) = await _characterFileService.SaveCharacterAsAsync(CurrentCharacter);
            if (success)
            {
                _currentFilePath = filePath;
                StatusMessage = $"Saved character: {CurrentCharacter.Name}";
            }
            else
            {
                StatusMessage = "Save operation cancelled";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving character: {ex.Message}";
            MessageBox.Show($"Failed to save character:\n{ex.Message}", "Save Error", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportToHtml()
    {
        StatusMessage = "Export to HTML functionality not yet implemented";
    }

    private async void ExportToPdf()
    {
        LoadingWindow? loadingWindow = null;
        try
        {
            // First, show the Features and Traits review window
            var featuresAndTraitsWindow = new Views.FeaturesAndTraitsWindow(FeaturesAndTraits);
            
            // Set the owner safely
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null && mainWindow != featuresAndTraitsWindow && mainWindow.IsVisible)
            {
                featuresAndTraitsWindow.Owner = mainWindow;
            }
            
            var reviewResult = featuresAndTraitsWindow.ShowDialog();
            if (reviewResult != true)
            {
                StatusMessage = "PDF export cancelled by user";
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                DefaultExt = "pdf",
                FileName = $"{CurrentCharacter.Name}_CharacterSheet.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // Show loading screen immediately with initial message
                loadingWindow = LoadingWindow.ShowPdfExportProgress();
                StatusMessage = "Exporting to PDF...";
                
                // Give the UI a moment to render the loading screen
                await Task.Delay(50);
                
                // Update progress step by step with immediate feedback
                loadingWindow.UpdateProgress(10, "📄 Initializing PDF export...");
                await Task.Delay(100);
                
                loadingWindow.UpdateProgress(20, "🖋️ Gathering character data...");
                await Task.Delay(100);
                
                var pdfService = new PdfSharpExportService();
                
                loadingWindow.UpdateProgress(40, "🎨 Applying character sheet formatting...");
                await Task.Delay(100);
                
                loadingWindow.UpdateProgress(60, "📊 Processing ability scores and skills...");
                await Task.Delay(100);
                
                loadingWindow.UpdateProgress(80, "⚔️ Adding equipment and features...");
                
                // Perform the actual PDF export
                var success = await pdfService.ExportCharacterToPdfAsync(CurrentCharacter, saveFileDialog.FileName, this);
                
                loadingWindow.UpdateProgress(100, "✅ Export complete!");
                await Task.Delay(500); // Brief pause to show completion
                
                // Close loading window
                if (loadingWindow != null && loadingWindow.IsVisible)
                {
                    loadingWindow.Close();
                    loadingWindow = null; // Prevent finally block from trying to close again
                }
                
                if (success)
                {
                    StatusMessage = $"Character sheet exported to {saveFileDialog.FileName}";
                    
                    // Optionally open the file
                    var result = System.Windows.MessageBox.Show(
                        "PDF exported successfully! Would you like to open it now?",
                        "Export Complete",
                        System.Windows.MessageBoxButton.YesNo,
                        System.Windows.MessageBoxImage.Question);
                    
                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveFileDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
                else
                {
                    StatusMessage = "Failed to export PDF";
                    System.Windows.MessageBox.Show(
                        "Failed to export the character sheet to PDF. Please check the debug output for more details.",
                        "Export Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Error during PDF export";
            System.Windows.MessageBox.Show(
                $"An error occurred while exporting to PDF: {ex.Message}",
                "Export Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            // Ensure loading window is closed
            if (loadingWindow != null && loadingWindow.IsVisible)
            {
                loadingWindow.Close();
            }
        }
    }

    private void ShowSettings()
    {
        try
        {
            var settingsWindow = new Views.SettingsWindow(Settings);
            
            // Safely set the owner - ensure main window is shown and not the same instance
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null && mainWindow != settingsWindow && mainWindow.IsVisible)
            {
                settingsWindow.Owner = mainWindow;
            }
            
            var result = settingsWindow.ShowDialog();
            if (result == true)
            {
                // Settings were applied - they're already updated in the Settings object
                // Save the updated settings to file
                var saveSuccess = SettingsService.SaveSettings(Settings);
                if (saveSuccess)
                {
                    StatusMessage = "Settings have been updated and saved";
                }
                else
                {
                    StatusMessage = "Settings have been updated (but failed to save to file)";
                }
                
                // Apply the current theme (this ensures consistency even if theme wasn't changed)
                ThemeService.ApplyTheme(Settings.Theme);
                
                // Update current character with new settings to refresh calculations
                if (CurrentCharacter != null)
                {
                    CurrentCharacter.SetSettings(Settings);
                }
                
                // Notify that settings changed to update any dependent properties
                OnPropertyChanged(nameof(Settings));
                OnPropertyChanged(nameof(IsStandardArrayMode));
                OnPropertyChanged(nameof(IsPointBuyMode));
                OnPropertyChanged(nameof(IsRollingMode));
                OnPropertyChanged(nameof(IsFreeEntryMode));
                
                // Auto-create the appropriate ViewModel based on current method
                CreateViewModelForCurrentMethod();
            }
            else
            {
                StatusMessage = "Settings unchanged";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error opening settings: {ex.Message}";
            MessageBox.Show($"Failed to open settings:\n{ex.Message}", "Settings Error", 
                           MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExitApplication()
    {
        // Save settings on exit
        SettingsService.SaveSettings(Settings);
        
        System.Windows.Application.Current.Shutdown();
    }

    private async void DiscoverPdfFields()
    {
        // This method discovers the fields in the PDF template and writes them to a text file
        // it is useful for debugging and understanding the field names available in the PDF.
        // May no longer need this if we have a good mapping already.
        try
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "2024-character-sheet.pdf");
            
            if (!File.Exists(templatePath))
            {
                System.Windows.MessageBox.Show(
                    $"PDF template not found at: {templatePath}",
                    "Template Not Found",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "txt",
                FileName = "PDF_Fields_Discovery.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                StatusMessage = "Discovering PDF fields...";
                await PdfSharpFieldDiscoveryService.WriteFieldsToFileAsync(templatePath, saveFileDialog.FileName);
                StatusMessage = $"PDF field discovery report saved to {saveFileDialog.FileName}";
                
                // Optionally open the file
                var result = System.Windows.MessageBox.Show(
                    "Field discovery complete! Would you like to open the report now?",
                    "Discovery Complete",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);
                
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveFileDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Error discovering PDF fields";
            System.Windows.MessageBox.Show(
                $"An error occurred while discovering PDF fields: {ex.Message}",
                "Discovery Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    private async void CreateTestPdf()
    {
        // This method creates a test PDF with field names filled in, useful for debugging and understanding the field positions.
        // It uses the PdfFieldMappingTool to generate a PDF that shows each field name in its actual position on the character sheet.
        // May no longer need this if we have a good mapping already.
        try
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "2024-character-sheet.pdf");
            
            if (!File.Exists(templatePath))
            {
                System.Windows.MessageBox.Show(
                    $"PDF template not found at: {templatePath}",
                    "Template Not Found",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                DefaultExt = "pdf",
                FileName = "Field_Test_PDF.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                StatusMessage = "Creating field mapping PDF...";
                var success = await PdfFieldMappingTool.CreateFieldMappingPdfAsync(templatePath, saveFileDialog.FileName);
                
                if (success)
                {
                    StatusMessage = $"Test PDF created at {saveFileDialog.FileName}";
                    
                    var result = System.Windows.MessageBox.Show(
                        "Field mapping PDF created! This PDF shows each field name in its actual position on the character sheet. Use this to determine the correct field mappings.",
                        "Field Mapping PDF Created",
                        System.Windows.MessageBoxButton.YesNo,
                        System.Windows.MessageBoxImage.Information);
                    
                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveFileDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
                else
                {
                    StatusMessage = "Failed to create test PDF";
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Error creating test PDF";
            System.Windows.MessageBox.Show(
                $"An error occurred while creating test PDF: {ex.Message}",
                "Test PDF Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    private async void AnalyzePdf()
    {
        // This method analyzes the PDF structure and provides a report on the fields found, including text fields and checkboxes.
        // It uses the PdfSharpExportService to read the PDF and extract field information.
        // May no longer need this if we have a good mapping already.
        try
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "2024-character-sheet.pdf");
            
            if (!File.Exists(templatePath))
            {
                System.Windows.MessageBox.Show(
                    $"PDF template not found at: {templatePath}",
                    "Template Not Found",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            StatusMessage = "Analyzing PDF structure...";
            var analysis = await AnalyzePdfStructureAsync(templatePath);
            
            if (analysis.IsValid)
            {
                var message = $"PDF Analysis Results:\n\n" +
                            $"Total Fields: {analysis.TotalFields}\n" +
                            $"Text Fields: {analysis.TextFields}\n" +
                            $"Checkbox Fields: {analysis.CheckboxFields}\n" +
                            $"Encrypted: {analysis.IsEncrypted}\n\n" +
                            $"First few text fields:\n{string.Join("\n", analysis.FieldNames?.Take(10) ?? new List<string>())}";
                
                System.Windows.MessageBox.Show(message, "PDF Analysis Complete", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                
                StatusMessage = $"PDF analyzed: {analysis.TextFields} text fields, {analysis.CheckboxFields} checkboxes";
            }
            else
            {
                System.Windows.MessageBox.Show(
                    $"PDF Analysis Failed:\n{analysis.ErrorMessage}",
                    "Analysis Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                
                StatusMessage = $"PDF analysis failed: {analysis.ErrorMessage}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Error analyzing PDF";
            System.Windows.MessageBox.Show(
                $"An error occurred while analyzing PDF: {ex.Message}",
                "Analysis Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    private void ShowAbout()
    {
        var settingsPath = SettingsService.GetSettingsFilePath();
        var aboutMessage = $"GToon Manager v1.0\nD&D 5E Character Creator\n\nCreated with ❤️ for adventurers everywhere!\n\n" +
                          $"Settings are automatically saved to:\n{settingsPath}";
        
        MessageBox.Show(aboutMessage, "About GToon Manager", MessageBoxButton.OK, MessageBoxImage.Information);
        StatusMessage = "About dialog shown";
    }

    private void AddClass()
    {
        if (SelectedClassToAdd != null)
        {
            // Check if we need skill selections and they haven't been made
            if (HasSkillChoices && !HasRequiredSkillSelections)
            {
                StatusMessage = $"Please select {MaxSkillSelections} skills for the {SelectedClassToAdd.Name} class";
                return;
            }

            // Create the class level with chosen skills
            var classLevel = new CharacterClassLevel 
            { 
                CharacterClass = SelectedClassToAdd, 
                Level = SelectedLevelToAdd 
            };
            
            // Add chosen skills
            foreach (var skillItem in SelectedSkills)
            {
                classLevel.ChosenSkillProficiencies.Add(skillItem.SkillName);
            }
            
            // Add the class level directly to the character
            CurrentCharacter.ClassLevels.Add(classLevel);
            
            StatusMessage = $"Added {SelectedLevelToAdd} level(s) of {SelectedClassToAdd.Name}";
            
            // Clear selections
            SelectedClassToAdd = null;
            SelectedLevelToAdd = 1;
            SelectedSkills.Clear();
        }
        else
        {
            StatusMessage = "Please select a class to add";
        }
    }

    private void RemoveClass(object? parameter)
    {
        if (parameter is CharacterClassLevel classLevel)
        {
            CurrentCharacter.ClassLevels.Remove(classLevel);
            StatusMessage = $"Removed {classLevel.ClassName}";
        }
    }

    private bool CanRemoveClass(object? parameter)
    {
        return CurrentCharacter.ClassLevels.Count > 0;
    }

    private void AddSkill(object? parameter)
    {
        if (parameter is string skillName && CanAddMoreSkills)
        {
            if (!SelectedSkills.Any(s => s.SkillName == skillName))
            {
                SelectedSkills.Add(new SkillSelectionItem { SkillName = skillName });
                OnPropertyChanged(nameof(CanAddMoreSkills));
                OnPropertyChanged(nameof(HasRequiredSkillSelections));
                StatusMessage = $"Added {skillName} skill proficiency";
            }
        }
    }

    private void RemoveSkill(object? parameter)
    {
        if (parameter is SkillSelectionItem skillItem)
        {
            SelectedSkills.Remove(skillItem);
            OnPropertyChanged(nameof(CanAddMoreSkills));
            OnPropertyChanged(nameof(HasRequiredSkillSelections));
            StatusMessage = $"Removed {skillItem.SkillName} skill proficiency";
        }
    }

    private void ChooseSubclass(object? parameter)
    {
        if (parameter is CharacterClassLevel characterClassLevel && 
            characterClassLevel.CharacterClass != null && 
            characterClassLevel.CanChooseSubclass)
        {
            var window = new Views.SubclassSelectionWindow();
            var viewModel = new SubclassSelectionViewModel(
                characterClassLevel,
                subclass => {
                    ApplySubclass(characterClassLevel, subclass);
                    window.Close(); // Close the window after successful application
                },
                () => window.Close() // Cancel action - close window
            );

            window.DataContext = viewModel;
            
            // Safely set the owner - avoid setting window as its own owner
            if (Application.Current.MainWindow != null && Application.Current.MainWindow != window)
            {
                window.Owner = Application.Current.MainWindow;
            }
            
            window.ShowDialog();
        }
    }

    private void ApplySubclass(CharacterClassLevel characterClassLevel, Subclass selectedSubclass)
    {
        characterClassLevel.ChosenSubclass = selectedSubclass;
        StatusMessage = $"Applied {selectedSubclass.Name} subclass to {characterClassLevel.ClassName}";
    }

    private async Task<bool> CreateTestPdfWithFieldNamesAsync(string templatePath, string outputPath)
    {
        // This method creates a test PDF with field names filled in, useful for debugging and understanding the field positions.
        // It uses the PdfSharpExportService to generate a PDF that shows each field name in its actual position on the character sheet.
        // May no longer need this if we have a good mapping already.
        try
        {
            var pdfService = new PdfSharpExportService();
            var fieldNames = pdfService.GetPdfFieldNames();
            
            if (fieldNames.Count == 0)
            {
                return false;
            }

            // Create a simple test character
            var testCharacter = new Character
            {
                Name = "TEST_CHARACTER",
                PlayerName = "TEST_PLAYER"
            };

            // Try to export with test data
            return await pdfService.ExportCharacterToPdfAsync(testCharacter, outputPath);
        }
        catch
        {
            return false;
        }
    }

    private Task<PdfAnalysisResult> AnalyzePdfStructureAsync(string templatePath)
    // This method analyzes the PDF structure and provides a report on the fields found, including text fields and checkboxes.
    // It uses the PdfSharpExportService to read the PDF and extract field information.
    // May no longer need this if we have a good mapping already.
    {
        try
        {
            var pdfService = new PdfSharpExportService();
            var fieldNames = pdfService.GetPdfFieldNames();
            
            return Task.FromResult(new PdfAnalysisResult
            {
                IsValid = true,
                TotalFields = fieldNames.Count,
                TextFields = fieldNames.Count, // Simplified - assume all are text fields
                CheckboxFields = 0,
                IsEncrypted = false,
                FieldNames = fieldNames,
                ErrorMessage = null
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new PdfAnalysisResult
            {
                IsValid = false,
                ErrorMessage = ex.Message
            });
        }
    }

    private void OpenStandardArray()
    {
        try
        {
            var viewModel = new StandardArrayViewModel(CurrentCharacter);
            UpdateStandardArrayViewModelClasses(viewModel);
            
            var control = new Controls.StandardArrayControl();
            var window = new Views.AbilityScoreGenerationWindow();
            
            // Safely set the owner - ensure main window is shown and not the same instance
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null && mainWindow != window && mainWindow.IsVisible)
            {
                window.Owner = mainWindow;
            }
            
            window.SetContent("Standard Array Assignment", control, viewModel);
            
            var result = window.ShowDialog();
            if (result == true)
            {
                StatusMessage = "Standard Array applied to character";
            }
            else
            {
                StatusMessage = "Standard Array assignment cancelled";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error opening Standard Array: {ex.Message}";
        }
    }

    private void OpenPointBuy()
    {
        try
        {
            var viewModel = new PointBuyViewModel(CurrentCharacter, Settings.PointBuyPoints);
            
            // Create a control for point buy - we'll need to make one
            var control = new Views.PointBuyControl();
            var window = new Views.AbilityScoreGenerationWindow();
            
            // Safely set the owner - ensure main window is shown and not the same instance
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null && mainWindow != window && mainWindow.IsVisible)
            {
                window.Owner = mainWindow;
            }
            
            window.SetContent("Point Buy Assignment", control, viewModel);
            
            var result = window.ShowDialog();
            if (result == true)
            {
                StatusMessage = "Point Buy applied to character";
            }
            else
            {
                StatusMessage = "Point Buy assignment cancelled";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error opening Point Buy: {ex.Message}";
        }
    }

    private void OpenRolling()
    {
        try
        {
            var viewModel = new RollingViewModel(CurrentCharacter, Settings.AbilityScoreMethod, Settings.RerollLimit);
            
            var control = new Controls.RollingControl();
            var window = new Views.AbilityScoreGenerationWindow();
            
            // Safely set the owner - ensure main window is shown and not the same instance
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null && mainWindow != window && mainWindow.IsVisible)
            {
                window.Owner = mainWindow;
            }
            
            window.SetContent("Dice Rolling", control, viewModel);
            
            var result = window.ShowDialog();
            if (result == true)
            {
                StatusMessage = "Rolled scores applied to character";
            }
            else
            {
                StatusMessage = "Dice rolling cancelled";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error opening Rolling: {ex.Message}";
        }
    }

    private void OpenFreeEntry()
    {
        try
        {
            var viewModel = new FreeEntryViewModel(CurrentCharacter);
            
            var control = new Controls.FreeEntryControl();
            var window = new Views.AbilityScoreGenerationWindow();
            
            // Safely set the owner - ensure main window is shown and not the same instance
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null && mainWindow != window && mainWindow.IsVisible)
            {
                window.Owner = mainWindow;
            }
            
            window.SetContent("Manual Entry", control, viewModel);
            
            var result = window.ShowDialog();
            if (result == true)
            {
                StatusMessage = "Manual scores applied to character";
            }
            else
            {
                StatusMessage = "Manual entry cancelled";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error opening Free Entry: {ex.Message}";
        }
    }

    private void CreateViewModelForCurrentMethod()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"CreateViewModelForCurrentMethod() called - Method: {Settings.AbilityScoreMethod}");
            
            switch (Settings.AbilityScoreMethod)
            {
                case AbilityScoreGenerationMethod.StandardArray:
                    if (StandardArrayViewModel == null)
                    {
                        StandardArrayViewModel = new StandardArrayViewModel(CurrentCharacter);
                        UpdateStandardArrayViewModelClasses();
                        System.Diagnostics.Debug.WriteLine("StandardArrayViewModel auto-created");
                    }
                    break;
                    
                case AbilityScoreGenerationMethod.PointBuy:
                    PointBuyViewModel = new PointBuyViewModel(CurrentCharacter, Settings.PointBuyPoints);
                    System.Diagnostics.Debug.WriteLine("PointBuyViewModel auto-created");
                    break;
                    
                case AbilityScoreGenerationMethod.FourD6DropLowest:
                    if (RollingViewModel == null || RollingViewModel.IsCompleted)
                    {
                        System.Diagnostics.Debug.WriteLine($"Creating RollingViewModel with Settings.RerollLimit: {Settings.RerollLimit}");
                        RollingViewModel = new RollingViewModel(CurrentCharacter, Settings.AbilityScoreMethod, Settings.RerollLimit);
                        System.Diagnostics.Debug.WriteLine("RollingViewModel auto-created");
                    }
                    break;
                    
                case AbilityScoreGenerationMethod.FreeEntry:
                    FreeEntryViewModel = new FreeEntryViewModel(CurrentCharacter);
                    System.Diagnostics.Debug.WriteLine("FreeEntryViewModel auto-created");
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in CreateViewModelForCurrentMethod: {ex}");
            StatusMessage = $"Error creating ability score interface: {ex.Message}";
        }
    }

    private void OnRollingScoresApplied(object? sender, EventArgs e)
    {
        // Close the rolling interface by clearing the view model
        RollingViewModel = null;
        StatusMessage = "Ability scores applied successfully";
    }

    private void ApplyBackground()
    {
        if (SelectedBackground != null && HasBackgroundImprovementOption)
        {
            // Auto-apply uniform distributions if no manual selections have been made
            if (BackgroundAbilityScoreSelections.Count == 0 && IsUniformDistribution)
            {
                AutoApplyUniformDistribution();
            }
            
            // Check if we can apply after auto-application
            if (CanApplyBackgroundSelection)
            {
                // Apply the background to the character
                CurrentCharacter.Background = SelectedBackground;
                
                // Create and apply the ability score choice
                var backgroundChoice = new BackgroundAbilityScoreChoice
                {
                    Background = SelectedBackground,
                    SelectedOption = SelectedBackgroundImprovementOption!,
                    AbilityScoreImprovements = new Dictionary<string, int>()
                };
                
                // Convert selections to improvements dictionary
                foreach (var selection in BackgroundAbilityScoreSelections)
                {
                    if (backgroundChoice.AbilityScoreImprovements.ContainsKey(selection.AbilityScore))
                    {
                        backgroundChoice.AbilityScoreImprovements[selection.AbilityScore] += selection.Improvement;
                    }
                    else
                    {
                        backgroundChoice.AbilityScoreImprovements[selection.AbilityScore] = selection.Improvement;
                    }
                }
                
                CurrentCharacter.BackgroundAbilityScoreChoice = backgroundChoice;
                
                StatusMessage = $"Applied background: {SelectedBackground.Name}";
                
                // Clear selections
                SelectedBackground = null;
                SelectedBackgroundImprovementOption = null;
                BackgroundAbilityScoreSelections.Clear();
            }
            else
            {
                StatusMessage = "Please complete your background ability score selections";
            }
        }
        else
        {
            StatusMessage = "Please select a background and improvement option";
        }
    }

    private void AddBackgroundAbilityScore(object? parameter)
    {
        if (parameter is string abilityScore && SelectedBackgroundImprovementOption != null)
        {
            System.Diagnostics.Debug.WriteLine($"AddBackgroundAbilityScore called with: '{abilityScore}'");
            
            // If already selected, remove it (toggle off)
            var existing = BackgroundAbilityScoreSelections.FirstOrDefault(
                s => string.Equals(s.AbilityScore, abilityScore, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                System.Diagnostics.Debug.WriteLine($"Removing existing selection: '{existing.AbilityScore}' +{existing.Improvement}");
                BackgroundAbilityScoreSelections.Remove(existing);
                OnPropertyChanged(nameof(CanApplyBackgroundSelection));
                StatusMessage = $"Removed selection for {abilityScore}";
                return;
            }

            // Otherwise, add as before
            var availableImprovements = GetAvailableImprovementsForBackground();
            System.Diagnostics.Debug.WriteLine($"Available improvements: [{string.Join(", ", availableImprovements)}]");
            if (availableImprovements.Count > 0)
            {
                var improvement = availableImprovements.First();
                System.Diagnostics.Debug.WriteLine($"Adding new selection: '{abilityScore}' +{improvement}");
                BackgroundAbilityScoreSelections.Add(new BackgroundAbilityScoreSelection
                {
                    AbilityScore = abilityScore,
                    Improvement = improvement
                });

                OnPropertyChanged(nameof(CanApplyBackgroundSelection));
                StatusMessage = $"Added +{improvement} to {abilityScore}";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No available improvements!");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"AddBackgroundAbilityScore invalid parameters: abilityScore='{parameter}', hasOption={SelectedBackgroundImprovementOption != null}");
        }
    }

    private void RemoveBackgroundAbilityScore(object? parameter)
    {
        if (parameter is BackgroundAbilityScoreSelection selection)
        {
            BackgroundAbilityScoreSelections.Remove(selection);
            OnPropertyChanged(nameof(CanApplyBackgroundSelection));
            StatusMessage = $"Removed +{selection.Improvement} from {selection.AbilityScore}";
        }
    }

    private void AutoApplyUniformDistribution()
    {
        if (SelectedBackgroundImprovementOption?.Distributions == null || !IsUniformDistribution) return;

        var distribution = SelectedBackgroundImprovementOption.Distributions[0];
        var improvementAmount = distribution.Amount;
        var requiredCount = distribution.Count;
        var availableAbilities = AvailableAbilityScoresForBackground;

        // Clear any existing selections first
        BackgroundAbilityScoreSelections.Clear();

        // Auto-apply the improvements to the available abilities
        var abilitiesToImprove = availableAbilities.Take(requiredCount).ToList();
        
        foreach (var ability in abilitiesToImprove)
        {
            BackgroundAbilityScoreSelections.Add(new BackgroundAbilityScoreSelection
            {
                AbilityScore = ability,
                Improvement = improvementAmount
            });
        }

        OnPropertyChanged(nameof(CanApplyBackgroundSelection));
        StatusMessage = $"Auto-applied +{improvementAmount} to {string.Join(", ", abilitiesToImprove)}";
    }

    private bool ValidateBackgroundAbilityScoreSelections()
    {
        if (SelectedBackgroundImprovementOption?.Distributions == null) return false;
        
        var expectedTotals = new Dictionary<int, int>(); // improvement amount -> count
        foreach (var dist in SelectedBackgroundImprovementOption.Distributions)
        {
            expectedTotals[dist.Amount] = dist.Count;
        }
        
        var actualTotals = new Dictionary<int, int>();
        foreach (var selection in BackgroundAbilityScoreSelections)
        {
            if (actualTotals.ContainsKey(selection.Improvement))
                actualTotals[selection.Improvement]++;
            else
                actualTotals[selection.Improvement] = 1;
        }
        
        // Check if actual matches expected
        foreach (var expected in expectedTotals)
        {
            if (!actualTotals.TryGetValue(expected.Key, out var actual) || actual != expected.Value)
                return false;
        }
        
        // Check for no extra selections
        foreach (var actual in actualTotals)
        {
            if (!expectedTotals.ContainsKey(actual.Key))
                return false;
        }
        
        // Verify all selections are valid ability scores for this background
        var validAbilities = AvailableAbilityScoresForBackground;
        return BackgroundAbilityScoreSelections.All(s => validAbilities.Contains(s.AbilityScore, StringComparer.OrdinalIgnoreCase));
    }

    private List<int> GetAvailableImprovementsForBackground()
    {
        if (SelectedBackgroundImprovementOption?.Distributions == null) return new List<int>();
        
        var needed = new List<int>();
        foreach (var dist in SelectedBackgroundImprovementOption.Distributions)
        {
            for (int i = 0; i < dist.Count; i++)
            {
                needed.Add(dist.Amount);
            }
        }
        
        // Remove already selected improvements
        var used = BackgroundAbilityScoreSelections.Select(s => s.Improvement).ToList();
        foreach (var improvement in used)
        {
            needed.Remove(improvement);
        }
        
        return needed.OrderByDescending(x => x).ToList();
    }

    private void ManageASI(object? parameter)
    {
        var asiViewModel = new AbilityScoreImprovementViewModel(CurrentCharacter, OnCharacterUpdated);
        var asiWindow = new Views.AbilityScoreImprovementWindow(asiViewModel);
        asiWindow.ShowDialog();
    }

    private bool CanManageASI(object? parameter)
    {
        return CurrentCharacter.ClassLevels.Count > 0;
    }

    private void OnCharacterUpdated(Character character)
    {
        // Refresh UI properties that might have changed
        OnPropertyChanged(nameof(CurrentCharacter));
        UpdateFeaturesAndTraits();
        
        // Force refresh of all ability score related properties
        character.RefreshAbilityScoreModifiers();
    }

    private void CurrentCharacter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Character.Background))
        {
            // When a background is selected from dropdown, set it as the selected background for configuration
            if (CurrentCharacter.Background != null)
            {
                // Only trigger if this isn't the same as what we're already configuring
                if (SelectedBackground?.Name != CurrentCharacter.Background.Name)
                {
                    SelectedBackground = CurrentCharacter.Background;
                    // Clear the current character's background temporarily so user can configure ability scores
                    CurrentCharacter.Background = null;
                }
            }
        }
        else if (e.PropertyName == nameof(Character.BackgroundAbilityScoreChoice))
        {
            // When background ability score choice changes, update the applied status
            OnPropertyChanged(nameof(HasAppliedBackgroundAbilities));
        }
        if (e.PropertyName == nameof(Character.Species) ||
            e.PropertyName == nameof(Character.Subspecies) ||
            e.PropertyName == nameof(Character.Background) ||
            e.PropertyName == nameof(Character.ClassLevels))
        {
            UpdateFeaturesAndTraits();
        }
    }

    private void CurrentCharacter_ClassLevelsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateFeaturesAndTraits();
        
        // Enable/disable the Features tab based on whether there are class levels
        FeaturesTabEnabled = CurrentCharacter.ClassLevels.Count > 0;
        
        // Refresh Features ViewModel when classes change
        if (FeaturesViewModel != null)
        {
            InitializeFeaturesViewModel();
        }
    }

    // --- Ability Score Selection Helpers for XAML ---
    public int? GetAbilityScoreBonus(string abilityScore)
    {
        System.Diagnostics.Debug.WriteLine($"GetAbilityScoreBonus called with: '{abilityScore}'");
        System.Diagnostics.Debug.WriteLine($"Current selections count: {BackgroundAbilityScoreSelections.Count}");
        foreach (var selection in BackgroundAbilityScoreSelections)
        {
            System.Diagnostics.Debug.WriteLine($"  Selection: '{selection.AbilityScore}' -> +{selection.Improvement}");
        }
        
        var sel = BackgroundAbilityScoreSelections.FirstOrDefault(s => string.Equals(s.AbilityScore, abilityScore, StringComparison.OrdinalIgnoreCase));
        var result = sel?.Improvement;
        System.Diagnostics.Debug.WriteLine($"GetAbilityScoreBonus result: {result}");
        return result;
    }

    public bool IsAbilityScoreSelected(string abilityScore)
    {
        System.Diagnostics.Debug.WriteLine($"IsAbilityScoreSelected called with: '{abilityScore}'");
        var result = BackgroundAbilityScoreSelections.Any(s => string.Equals(s.AbilityScore, abilityScore, StringComparison.OrdinalIgnoreCase));
        System.Diagnostics.Debug.WriteLine($"IsAbilityScoreSelected result: {result}");
        return result;
    }

    // --- Features & Traits Tab ---
    private void UpdateFeaturesAndTraits()
    {
        var list = new List<FeatureTraitViewModel>();
        var character = CurrentCharacter;
        // Species traits
        if (character.Species != null)
        {
            foreach (var trait in character.Species.Traits)
            {
                list.Add(new FeatureTraitViewModel
                {
                    Name = trait.Name,
                    Description = trait.Description,
                    Source = $"Species: {character.Species.Name}",
                    IsSelected = true
                });
            }
        }
        // Subspecies traits
        if (character.Subspecies != null)
        {
            foreach (var trait in character.Subspecies.Traits)
            {
                list.Add(new FeatureTraitViewModel
                {
                    Name = trait.Name,
                    Description = trait.Description,
                    Source = $"Subspecies: {character.Subspecies.Name}",
                    IsSelected = true
                });
            }
        }
        // Background feature
        if (character.Background != null && character.Background.Feature != null)
        {
            list.Add(new FeatureTraitViewModel
            {
                Name = character.Background.Feature.Name,
                Description = character.Background.Feature.Description,
                Source = $"Feat: {character.Background.Feature.Name} (from Background: {character.Background.Name})",
                IsSelected = true
            });
        }
        // Class features (from all class levels)
        foreach (var classLevel in character.ClassLevels)
        {
            if (classLevel.CharacterClass != null)
            {
                // Do NOT add the class itself as a feature (description)
                // Add class features up to this level
                foreach (var kvp in classLevel.CharacterClass.Features)
                {
                    int featureLevel = kvp.Key;
                    if (featureLevel <= classLevel.Level)
                    {
                        foreach (var feature in kvp.Value)
                        {
                            // Skip Ability Score Improvement features - they will be handled separately
                            if (feature.Name == "Ability Score Improvement")
                                continue;
                                
                            list.Add(new FeatureTraitViewModel
                            {
                                Name = feature.Name,
                                Description = feature.Description,
                                Source = $"Class Feature: {classLevel.ClassName} (Level {featureLevel})",
                                IsSelected = true
                            });
                        }
                    }
                }
                // Subclass features
                if (classLevel.ChosenSubclass != null)
                {
                    var features = classLevel.ChosenSubclass.GetFeaturesUpToLevel(classLevel.Level);
                    foreach (var feature in features)
                    {
                        list.Add(new FeatureTraitViewModel
                        {
                            Name = feature.Name,
                            Description = feature.Description,
                            Source = $"Subclass: {classLevel.ChosenSubclass.Name} (Level {classLevel.Level})",
                            IsSelected = true
                        });
                    }
                }
            }
        }
        FeaturesAndTraits = new ObservableCollection<FeatureTraitViewModel>(list);
        FeaturesTabEnabled = character.Species != null && character.Background != null && character.ClassLevels.Count > 0;
    }

    private void InitializeFeaturesViewModel()
    {
        var choiceDataService = new ChoiceDataService();
        FeaturesViewModel = new FeaturesViewModel(CurrentCharacter, _classDataService, choiceDataService);
    }

    // Returns a string for PDF export of all selected features/traits, grouped by source
    public string GetSelectedFeaturesAndTraitsText()
    {
        var grouped = FeaturesAndTraits
            .Where(f => f.IsSelected)
            .GroupBy(f => f.Source)
            .OrderBy(g => g.Key);
        var sb = new StringBuilder();
        foreach (var group in grouped)
        {
            sb.AppendLine($"{group.Key}:");
            foreach (var item in group)
            {
                sb.AppendLine($"  {item.Name}: {item.Description}");
            }
            sb.AppendLine();
        }
        return sb.ToString().Trim();
    }

    public string GetSelectedClassFeaturesText()
    {
        var sb = new StringBuilder();
        foreach (var item in FeaturesAndTraits.Where(f => f.IsSelected && f.Source.StartsWith("Class Feature") && f.Name != "Ability Score Improvement"))
            sb.AppendLine($"- {item.Name}: {item.Description}");
        foreach (var item in FeaturesAndTraits.Where(f => f.IsSelected && f.Source.StartsWith("Subclass")))
            sb.AppendLine($"- {item.Name}: {item.Description}");
        return sb.ToString().Trim();
    }

    public string GetSelectedSpeciesTraitsText()
    {
        var sb = new StringBuilder();
        foreach (var item in FeaturesAndTraits.Where(f => f.IsSelected && (f.Source.StartsWith("Species") || f.Source.StartsWith("Subspecies"))))
            sb.AppendLine($"- {item.Name}: {item.Description}");
        return sb.ToString().Trim();
    }

    public string GetSelectedFeatsText()
    {
        var sb = new StringBuilder();
        foreach (var item in FeaturesAndTraits.Where(f => f.IsSelected && f.Source.StartsWith("Feat")))
            sb.AppendLine($"- {item.Name}: {item.Description}");
        return sb.ToString().Trim();
    }

    // Get available Ability Score Improvement features for the current character
    // This can be used for future ASI management UI
    public List<(int Level, string ClassName, ClassFeature Feature)> GetAvailableAbilityScoreImprovements()
    {
        var asiFeatures = new List<(int Level, string ClassName, ClassFeature Feature)>();
        var character = CurrentCharacter;
        
        foreach (var classLevel in character.ClassLevels)
        {
            if (classLevel.CharacterClass != null)
            {
                foreach (var kvp in classLevel.CharacterClass.Features)
                {
                    int featureLevel = kvp.Key;
                    if (featureLevel <= classLevel.Level)
                    {
                        foreach (var feature in kvp.Value)
                        {
                            if (feature.Name == "Ability Score Improvement")
                            {
                                asiFeatures.Add((featureLevel, classLevel.ClassName, feature));
                            }
                        }
                    }
                }
            }
        }
        
        return asiFeatures.OrderBy(asi => asi.Level).ToList();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class SkillSelectionItem
{
    public string SkillName { get; set; } = string.Empty;
}

public class BackgroundAbilityScoreSelection
{
    public string AbilityScore { get; set; } = string.Empty;
    public int Improvement { get; set; }
}

// Helper class for the tab
public class FeatureTraitViewModel : INotifyPropertyChanged
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 