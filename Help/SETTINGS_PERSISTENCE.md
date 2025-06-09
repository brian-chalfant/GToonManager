# Settings Persistence Implementation

## Overview
GToon Manager now automatically saves and loads user settings between application sessions using JSON file persistence.

## Implementation Details

### SettingsService.cs
- **Location**: `Services/SettingsService.cs`
- **Purpose**: Handles all settings persistence operations
- **Storage Location**: `%APPDATA%\GToonManager\settings.json`
- **Format**: JSON with camelCase property naming

### Key Features

#### Automatic Loading
- Settings are automatically loaded when the application starts
- If no settings file exists, default settings are created and saved
- If the settings file is corrupted, default settings are used as fallback

#### Automatic Saving
- Settings are saved when applying changes in the Settings dialog
- Settings are also saved when the application exits (as a backup)
- Failed saves are handled gracefully with appropriate user feedback

#### Error Handling
- Robust error handling for file I/O operations
- Graceful degradation if settings can't be saved/loaded
- Debug logging for troubleshooting

### Technical Implementation

#### JSON Serialization Options
```csharp
JsonOptions = new()
{
    WriteIndented = true,              // Pretty-printed JSON
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // camelCase properties
    PropertyNameCaseInsensitive = true // Flexible reading
}
```

#### Settings File Location
- **Windows**: `C:\Users\[Username]\AppData\Roaming\GToonManager\settings.json`
- Directory is created automatically if it doesn't exist

#### Backup Functionality
- `BackupSettings()` method creates timestamped backup files
- Format: `settings.json.backup.yyyyMMdd_HHmmss`

### Modified Files

1. **Services/SettingsService.cs** (New)
   - Static service class for settings persistence
   - Methods: `LoadSettings()`, `SaveSettings()`, `GetSettingsFilePath()`, etc.

2. **ViewModels/MainViewModel.cs** (Modified)
   - Constructor now loads settings on startup: `_settings = SettingsService.LoadSettings()`
   - `ShowSettings()` method saves settings after applying changes
   - `ExitApplication()` method saves settings on exit
   - `ShowAbout()` method displays settings file location

### User Experience

#### Startup
- Application loads with previously saved settings
- Theme, generation methods, and all preferences are preserved
- No user action required

#### Settings Changes
- Changes are immediately applied for real-time preview
- Settings are persisted when "Apply" is clicked
- Status bar shows confirmation: "Settings have been updated and saved"

#### About Dialog
- Now shows the location of the settings file for user reference
- Helpful for troubleshooting or manual backup

### Supported Settings

All settings from the Settings model are automatically persisted:

#### Ability Score Generation
- Generation method (Standard Array, Point Buy, Rolling, Manual)
- Point buy points limit
- Reroll limit for rolling

#### Default Character Options
- Starting level
- Hit point calculation method
- Starting equipment method
- Variant human usage
- Optional class features

#### Data Sources & Content
- Core books enabled/disabled
- Expansion books enabled/disabled
- Homebrew content enabled/disabled
- Homebrew content path
- Data validation level

#### Export & PDF Settings
- Character sheet style
- Include spell lists
- Include equipment details
- Include backstory
- PDF compression level

#### User Interface
- **Theme** (Light/Dark) - Applied immediately
- Font size
- Auto-save interval
- Backup location

#### Calculations & Automation
- Auto-calculate modifiers
- Auto-apply racial bonuses
- Auto-update proficiency bonus
- Enable spell slot tracking

### File Format Example

```json
{
  "abilityScoreMethod": 0,
  "pointBuyPoints": 27,
  "rerollLimit": 1,
  "defaultStartingLevel": 1,
  "hitPointCalculation": 1,
  "startingEquipment": 0,
  "useVariantHuman": false,
  "useOptionalClassFeatures": false,
  "enableCoreBooks": true,
  "enableExpansionBooks": true,
  "enableHomebrewContent": false,
  "homebrewContentPath": "",
  "dataValidationLevel": 0,
  "characterSheetStyle": 0,
  "includeSpellLists": true,
  "includeEquipmentDetails": true,
  "includeBackstory": true,
  "pdfCompressionLevel": 5,
  "theme": 0,
  "fontSize": 12,
  "autoSaveInterval": 5,
  "backupLocation": "",
  "autoCalculateModifiers": true,
  "autoApplyRacialBonuses": true,
  "autoUpdateProficiencyBonus": true,
  "enableSpellSlotTracking": true
}
```

### Benefits

1. **User Convenience**: No need to reconfigure settings every time
2. **Persistence**: All preferences are remembered between sessions
3. **Reliability**: Robust error handling ensures application stability
4. **Transparency**: Users can see where settings are stored
5. **Portability**: Settings file can be backed up or shared
6. **Performance**: Fast JSON serialization with minimal overhead

### Future Enhancements

Potential future improvements:
- Settings import/export functionality
- Multiple settings profiles
- Cloud synchronization support
- Settings validation and migration for version updates 