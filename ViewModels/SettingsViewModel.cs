using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using GToonManager.Models;
using Microsoft.Win32;

namespace GToonManager.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private Settings _originalSettings;
    private Settings _currentSettings;
    private bool _hasUnappliedChanges = false;
    private bool _hasAppliedChanges = false;

    public SettingsViewModel(Settings originalSettings)
    {
        _originalSettings = originalSettings;
        _currentSettings = new Settings();
        CopySettings(_originalSettings, _currentSettings);
        
        // Subscribe to changes in current settings
        _currentSettings.PropertyChanged += CurrentSettings_PropertyChanged;
        
        InitializeCommands();
    }

    public Settings CurrentSettings
    {
        get => _currentSettings;
        set
        {
            if (_currentSettings != null)
            {
                _currentSettings.PropertyChanged -= CurrentSettings_PropertyChanged;
            }
            
            _currentSettings = value;
            
            if (_currentSettings != null)
            {
                _currentSettings.PropertyChanged += CurrentSettings_PropertyChanged;
            }
            
            OnPropertyChanged();
        }
    }

    // Properties for UI state
    public bool HasUnappliedChanges
    {
        get => _hasUnappliedChanges;
        private set
        {
            _hasUnappliedChanges = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ApplyButtonText));
        }
    }

    public bool HasAppliedChanges
    {
        get => _hasAppliedChanges;
        private set
        {
            _hasAppliedChanges = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ApplyButtonText));
        }
    }

    public string ApplyButtonText => HasUnappliedChanges ? "Apply" : "Close";

    // Commands
    public ICommand ApplyCommand { get; private set; } = null!;
    public ICommand CancelCommand { get; private set; } = null!;
    public ICommand BrowseHomebrewPathCommand { get; private set; } = null!;
    public ICommand BrowseBackupLocationCommand { get; private set; } = null!;
    public ICommand ResetToDefaultsCommand { get; private set; } = null!;

    // Properties for dialog result
    public bool? DialogResult { get; set; }

    private void InitializeCommands()
    {
        ApplyCommand = new RelayCommand(Apply);
        CancelCommand = new RelayCommand(Cancel);
        BrowseHomebrewPathCommand = new RelayCommand(BrowseHomebrewPath);
        BrowseBackupLocationCommand = new RelayCommand(BrowseBackupLocation);
        ResetToDefaultsCommand = new RelayCommand(ResetToDefaults);
    }

    private void Apply()
    {
        if (HasUnappliedChanges)
        {
            // Copy current settings back to original
            CopySettings(_currentSettings, _originalSettings);
            HasUnappliedChanges = false;
            HasAppliedChanges = true;
        }
        else
        {
            // Acting as Close button
            DialogResult = true;
            OnPropertyChanged(nameof(DialogResult));
        }
    }

    private void Cancel()
    {
        if (HasUnappliedChanges)
        {
            // Revert current settings to original values
            _currentSettings.PropertyChanged -= CurrentSettings_PropertyChanged;
            CopySettings(_originalSettings, _currentSettings);
            _currentSettings.PropertyChanged += CurrentSettings_PropertyChanged;
            OnPropertyChanged(nameof(CurrentSettings));
            HasUnappliedChanges = false;
        }
        
        DialogResult = HasAppliedChanges;
        OnPropertyChanged(nameof(DialogResult));
    }

    private void CurrentSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // When any setting changes, mark as having unapplied changes
        HasUnappliedChanges = true;
        
        // Apply the setting immediately to the original for real-time UI updates
        // This creates the immediate visual feedback the user requested
        ApplySettingImmediately(e.PropertyName);
    }

    private void ApplySettingImmediately(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName)) return;
        
        // Apply specific high-impact settings immediately for real-time feedback
        // This affects the main UI while still tracking unapplied state
        switch (propertyName)
        {
            case nameof(Settings.HitPointCalculation):
                // Update the original settings object directly
                _originalSettings.HitPointCalculation = _currentSettings.HitPointCalculation;
                break;
            case nameof(Settings.AbilityScoreMethod):
                _originalSettings.AbilityScoreMethod = _currentSettings.AbilityScoreMethod;
                break;
            case nameof(Settings.RerollLimit):
                _originalSettings.RerollLimit = _currentSettings.RerollLimit;
                break;
            // Add other critical settings that need immediate feedback here
            // Theme changes might be too disruptive for immediate application
        }
    }

    private void BrowseHomebrewPath()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Homebrew Content Folder",
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = "Select Folder",
        };

        // This is a bit of a hack to allow folder selection with OpenFileDialog
        dialog.Filter = "Folders|*.folder";
        dialog.FilterIndex = 1;
        dialog.RestoreDirectory = true;

        if (dialog.ShowDialog() == true)
        {
            var selectedPath = System.IO.Path.GetDirectoryName(dialog.FileName);
            if (!string.IsNullOrEmpty(selectedPath))
            {
                CurrentSettings.HomebrewContentPath = selectedPath;
            }
        }
    }

    private void BrowseBackupLocation()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Backup Location",
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = "Select Folder",
        };

        // This is a bit of a hack to allow folder selection with OpenFileDialog
        dialog.Filter = "Folders|*.folder";
        dialog.FilterIndex = 1;
        dialog.RestoreDirectory = true;

        if (dialog.ShowDialog() == true)
        {
            var selectedPath = System.IO.Path.GetDirectoryName(dialog.FileName);
            if (!string.IsNullOrEmpty(selectedPath))
            {
                CurrentSettings.BackupLocation = selectedPath;
            }
        }
    }

    private void ResetToDefaults()
    {
        var result = MessageBox.Show(
            "Are you sure you want to reset all settings to their default values?",
            "Reset Settings",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            CurrentSettings = new Settings();
            OnPropertyChanged(nameof(CurrentSettings));
        }
    }

    private void CopySettings(Settings source, Settings target)
    {
        // Ability Score Generation
        target.AbilityScoreMethod = source.AbilityScoreMethod;
        target.PointBuyPoints = source.PointBuyPoints;
        target.RerollLimit = source.RerollLimit;

        // Default Character Options
        target.DefaultStartingLevel = source.DefaultStartingLevel;
        target.HitPointCalculation = source.HitPointCalculation;
        target.StartingEquipment = source.StartingEquipment;
        target.UseVariantHuman = source.UseVariantHuman;
        target.UseOptionalClassFeatures = source.UseOptionalClassFeatures;

        // Data Sources & Content
        target.EnableCoreBooks = source.EnableCoreBooks;
        target.EnableExpansionBooks = source.EnableExpansionBooks;
        target.EnableHomebrewContent = source.EnableHomebrewContent;
        target.HomebrewContentPath = source.HomebrewContentPath;
        target.DataValidationLevel = source.DataValidationLevel;

        // Export & PDF Settings
        target.CharacterSheetStyle = source.CharacterSheetStyle;
        target.IncludeSpellLists = source.IncludeSpellLists;
        target.IncludeEquipmentDetails = source.IncludeEquipmentDetails;
        target.IncludeBackstory = source.IncludeBackstory;
        target.PdfCompressionLevel = source.PdfCompressionLevel;

        // User Interface
        target.Theme = source.Theme;
        target.FontSize = source.FontSize;
        target.AutoSaveInterval = source.AutoSaveInterval;
        target.BackupLocation = source.BackupLocation;

        // Calculations & Automation
        target.AutoCalculateModifiers = source.AutoCalculateModifiers;
        target.AutoApplyRacialBonuses = source.AutoApplyRacialBonuses;
        target.AutoUpdateProficiencyBonus = source.AutoUpdateProficiencyBonus;
        target.EnableSpellSlotTracking = source.EnableSpellSlotTracking;
    }

    public void Dispose()
    {
        if (_currentSettings != null)
        {
            _currentSettings.PropertyChanged -= CurrentSettings_PropertyChanged;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 