using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows;
using GToonManager.Models;
using GToonManager.Services;
using Microsoft.Win32;

namespace GToonManager.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private Character _currentCharacter;
    private string _statusMessage = "Ready";
    private CharacterClass? _selectedClassToAdd;
    private int _selectedLevelToAdd = 1;
    private readonly CharacterFileService _characterFileService;
    private readonly RaceDataService _raceDataService;
    private readonly ClassDataService _classDataService;
    private readonly BackgroundDataService _backgroundDataService;
    private string? _currentFilePath;
    private Settings _settings;

    public MainViewModel()
    {
        _characterFileService = new CharacterFileService();
        _raceDataService = new RaceDataService();
        _classDataService = new ClassDataService();
        _backgroundDataService = new BackgroundDataService();
        _settings = new Settings();
        _currentCharacter = new Character
        {
            Name = "New Character",
            PlayerName = ""
        };

        InitializeData();
        InitializeCommands();
    }

    public Character CurrentCharacter
    {
        get => _currentCharacter;
        set
        {
            _currentCharacter = value;
            OnPropertyChanged(nameof(CurrentCharacter));
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

    public ObservableCollection<Race> Races { get; } = new();
    public ObservableCollection<CharacterClass> Classes { get; } = new();
    public ObservableCollection<Background> Backgrounds { get; } = new();

    public Settings Settings
    {
        get => _settings;
        set
        {
            _settings = value;
            OnPropertyChanged(nameof(Settings));
        }
    }

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
    
    // Multiclass commands
    public ICommand AddClassCommand { get; private set; } = null!;
    public ICommand RemoveClassCommand { get; private set; } = null!;
    public ICommand AddSkillCommand { get; private set; } = null!;
    public ICommand RemoveSkillCommand { get; private set; } = null!;

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
        
        // Multiclass commands
        AddClassCommand = new RelayCommand(AddClass);
        RemoveClassCommand = new RelayCommand(RemoveClass, CanRemoveClass);
        AddSkillCommand = new RelayCommand(AddSkill);
        RemoveSkillCommand = new RelayCommand(RemoveSkill);
    }

    private async void InitializeData()
    {
        await LoadRacesAsync();
        await LoadClassesAsync();
        await LoadBackgroundsAsync();
    }

    private async Task LoadRacesAsync()
    {
        try
        {
            var races = await _raceDataService.LoadAllRacesAsync();
            Races.Clear();
            foreach (var race in races)
            {
                Races.Add(race);
            }
            StatusMessage = $"Loaded {races.Count} races from data files";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading races: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error loading races: {ex}");
            
            // Fallback to basic races if loading fails
            Races.Add(new Race { Name = "Human", Description = "Versatile and ambitious" });
            Races.Add(new Race { Name = "Elf", Description = "Graceful and long-lived" });
            Races.Add(new Race { Name = "Dwarf", Description = "Hardy and traditional" });
            Races.Add(new Race { Name = "Halfling", Description = "Small and brave" });
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
        try
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                DefaultExt = "pdf",
                FileName = $"{CurrentCharacter.Name}_CharacterSheet.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                StatusMessage = "Exporting to PDF...";
                
                var pdfService = new PdfSharpExportService();
                var success = await pdfService.ExportCharacterToPdfAsync(CurrentCharacter, saveFileDialog.FileName);
                
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
    }

    private void ShowSettings()
    {
        try
        {
            var settingsWindow = new Views.SettingsWindow(Settings);
            settingsWindow.Owner = Application.Current.MainWindow;
            
            var result = settingsWindow.ShowDialog();
            if (result == true)
            {
                // Settings were applied - they're already updated in the Settings object
                StatusMessage = "Settings have been updated";
                
                // Here you could add logic to apply settings immediately, like:
                // - Reload data if content sources changed
                // - Update UI theme if theme changed
                // - etc.
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
        System.Windows.Application.Current.Shutdown();
    }

    private async void DiscoverPdfFields()
    {
        // This method discovers the fields in the PDF template and writes them to a text file
        // it is useful for debugging and understanding the field names available in the PDF.
        // May no longer need this if we have a good mapping already.
        try
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "2024-dnd-character-sheet-fillable.pdf");
            
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
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "2024-dnd-character-sheet-fillable.pdf");
            
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
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "2024-dnd-character-sheet-fillable.pdf");
            
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
        //TODO: Implement about dialog functionality
        StatusMessage = "About dialog functionality not yet implemented";
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
            classLevel.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CurrentCharacter.Level));
                OnPropertyChanged(nameof(CurrentCharacter.ProficiencyBonus));
                OnPropertyChanged(nameof(CurrentCharacter.ClassSummary));
            };
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

    private async Task<PdfAnalysisResult> AnalyzePdfStructureAsync(string templatePath)
    // This method analyzes the PDF structure and provides a report on the fields found, including text fields and checkboxes.
    // It uses the PdfSharpExportService to read the PDF and extract field information.
    // May no longer need this if we have a good mapping already.
    {
        try
        {
            var pdfService = new PdfSharpExportService();
            var fieldNames = pdfService.GetPdfFieldNames();
            
            return new PdfAnalysisResult
            {
                IsValid = true,
                TotalFields = fieldNames.Count,
                TextFields = fieldNames.Count, // Simplified - assume all are text fields
                CheckboxFields = 0,
                IsEncrypted = false,
                FieldNames = fieldNames,
                ErrorMessage = null
            };
        }
        catch (Exception ex)
        {
            return new PdfAnalysisResult
            {
                IsValid = false,
                ErrorMessage = ex.Message
            };
        }
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