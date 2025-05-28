using System.ComponentModel;

namespace GToonManager.Models;

public enum AbilityScoreGenerationMethod
{
    StandardArray,
    PointBuy,
    FourD6DropLowest,
    ThreeD6Straight,
    Custom
}

public enum HitPointCalculationMethod
{
    Average,
    Rolled,
    Maximum
}

public enum StartingEquipmentMethod
{
    ByClassAndBackground,
    StartingGold
}

public enum DataValidationLevel
{
    StrictOfficial,
    AllowHomebrew
}

public enum CharacterSheetStyle
{
    OfficialDnD,
    Custom
}

public enum Theme
{
    Light,
    Dark
}

public class Settings : INotifyPropertyChanged
{
    // Ability Score Generation
    private AbilityScoreGenerationMethod _abilityScoreMethod = AbilityScoreGenerationMethod.StandardArray;
    private int _pointBuyPoints = 27;

    // Default Character Options
    private int _defaultStartingLevel = 1;
    private HitPointCalculationMethod _hitPointCalculation = HitPointCalculationMethod.Average;
    private StartingEquipmentMethod _startingEquipment = StartingEquipmentMethod.ByClassAndBackground;
    private bool _useVariantHuman = false;
    private bool _useOptionalClassFeatures = false;

    // Data Sources & Content
    private bool _enableCoreBooks = true;
    private bool _enableExpansionBooks = true;
    private bool _enableHomebrewContent = false;
    private string _homebrewContentPath = "";
    private DataValidationLevel _dataValidationLevel = DataValidationLevel.StrictOfficial;

    // Export & PDF Settings
    private CharacterSheetStyle _characterSheetStyle = CharacterSheetStyle.OfficialDnD;
    private bool _includeSpellLists = true;
    private bool _includeEquipmentDetails = true;
    private bool _includeBackstory = true;
    private int _pdfCompressionLevel = 5;

    // User Interface
    private Theme _theme = Theme.Light;
    private int _fontSize = 12;
    private int _autoSaveInterval = 5; // minutes
    private string _backupLocation = "";

    // Calculations & Automation
    private bool _autoCalculateModifiers = true;
    private bool _autoApplyRacialBonuses = true;
    private bool _autoUpdateProficiencyBonus = true;
    private bool _enableSpellSlotTracking = true;

    // Ability Score Generation
    public AbilityScoreGenerationMethod AbilityScoreMethod
    {
        get => _abilityScoreMethod;
        set { _abilityScoreMethod = value; OnPropertyChanged(); }
    }

    public int PointBuyPoints
    {
        get => _pointBuyPoints;
        set { _pointBuyPoints = Math.Max(15, Math.Min(35, value)); OnPropertyChanged(); }
    }

    // Default Character Options
    public int DefaultStartingLevel
    {
        get => _defaultStartingLevel;
        set { _defaultStartingLevel = Math.Max(1, Math.Min(20, value)); OnPropertyChanged(); }
    }

    public HitPointCalculationMethod HitPointCalculation
    {
        get => _hitPointCalculation;
        set { _hitPointCalculation = value; OnPropertyChanged(); }
    }

    public StartingEquipmentMethod StartingEquipment
    {
        get => _startingEquipment;
        set { _startingEquipment = value; OnPropertyChanged(); }
    }

    public bool UseVariantHuman
    {
        get => _useVariantHuman;
        set { _useVariantHuman = value; OnPropertyChanged(); }
    }

    public bool UseOptionalClassFeatures
    {
        get => _useOptionalClassFeatures;
        set { _useOptionalClassFeatures = value; OnPropertyChanged(); }
    }

    // Data Sources & Content
    public bool EnableCoreBooks
    {
        get => _enableCoreBooks;
        set { _enableCoreBooks = value; OnPropertyChanged(); }
    }

    public bool EnableExpansionBooks
    {
        get => _enableExpansionBooks;
        set { _enableExpansionBooks = value; OnPropertyChanged(); }
    }

    public bool EnableHomebrewContent
    {
        get => _enableHomebrewContent;
        set { _enableHomebrewContent = value; OnPropertyChanged(); }
    }

    public string HomebrewContentPath
    {
        get => _homebrewContentPath;
        set { _homebrewContentPath = value ?? ""; OnPropertyChanged(); }
    }

    public DataValidationLevel DataValidationLevel
    {
        get => _dataValidationLevel;
        set { _dataValidationLevel = value; OnPropertyChanged(); }
    }

    // Export & PDF Settings
    public CharacterSheetStyle CharacterSheetStyle
    {
        get => _characterSheetStyle;
        set { _characterSheetStyle = value; OnPropertyChanged(); }
    }

    public bool IncludeSpellLists
    {
        get => _includeSpellLists;
        set { _includeSpellLists = value; OnPropertyChanged(); }
    }

    public bool IncludeEquipmentDetails
    {
        get => _includeEquipmentDetails;
        set { _includeEquipmentDetails = value; OnPropertyChanged(); }
    }

    public bool IncludeBackstory
    {
        get => _includeBackstory;
        set { _includeBackstory = value; OnPropertyChanged(); }
    }

    public int PdfCompressionLevel
    {
        get => _pdfCompressionLevel;
        set { _pdfCompressionLevel = Math.Max(1, Math.Min(9, value)); OnPropertyChanged(); }
    }

    // User Interface
    public Theme Theme
    {
        get => _theme;
        set { _theme = value; OnPropertyChanged(); }
    }

    public int FontSize
    {
        get => _fontSize;
        set { _fontSize = Math.Max(8, Math.Min(24, value)); OnPropertyChanged(); }
    }

    public int AutoSaveInterval
    {
        get => _autoSaveInterval;
        set { _autoSaveInterval = Math.Max(1, Math.Min(60, value)); OnPropertyChanged(); }
    }

    public string BackupLocation
    {
        get => _backupLocation;
        set { _backupLocation = value ?? ""; OnPropertyChanged(); }
    }

    // Calculations & Automation
    public bool AutoCalculateModifiers
    {
        get => _autoCalculateModifiers;
        set { _autoCalculateModifiers = value; OnPropertyChanged(); }
    }

    public bool AutoApplyRacialBonuses
    {
        get => _autoApplyRacialBonuses;
        set { _autoApplyRacialBonuses = value; OnPropertyChanged(); }
    }

    public bool AutoUpdateProficiencyBonus
    {
        get => _autoUpdateProficiencyBonus;
        set { _autoUpdateProficiencyBonus = value; OnPropertyChanged(); }
    }

    public bool EnableSpellSlotTracking
    {
        get => _enableSpellSlotTracking;
        set { _enableSpellSlotTracking = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 