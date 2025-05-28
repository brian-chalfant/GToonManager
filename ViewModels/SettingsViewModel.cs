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

    public SettingsViewModel(Settings originalSettings)
    {
        _originalSettings = originalSettings;
        _currentSettings = new Settings();
        CopySettings(_originalSettings, _currentSettings);
        
        InitializeCommands();
    }

    public Settings CurrentSettings
    {
        get => _currentSettings;
        set
        {
            _currentSettings = value;
            OnPropertyChanged();
        }
    }

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
        // Copy current settings back to original
        CopySettings(_currentSettings, _originalSettings);
        DialogResult = true;
    }

    private void Cancel()
    {
        DialogResult = false;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 