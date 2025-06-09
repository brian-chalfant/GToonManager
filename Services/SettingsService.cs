using System;
using System.IO;
using System.Text.Json;
using GToonManager.Models;

namespace GToonManager.Services;

/// <summary>
/// Service for persisting and loading application settings to/from JSON file
/// </summary>
public static class SettingsService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
        "GToonManager");
    
    private static readonly string SettingsFilePath = Path.Combine(SettingsDirectory, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Loads settings from the JSON file. If file doesn't exist or is corrupted, returns default settings.
    /// </summary>
    /// <returns>The loaded settings or default settings if loading fails</returns>
    public static Settings LoadSettings()
    {
        try
        {
            // Ensure settings directory exists
            if (!Directory.Exists(SettingsDirectory))
            {
                Directory.CreateDirectory(SettingsDirectory);
            }

            // Check if settings file exists
            if (!File.Exists(SettingsFilePath))
            {
                // Create default settings file
                var defaultSettings = new Settings();
                SaveSettings(defaultSettings);
                return defaultSettings;
            }

            // Read and deserialize settings file
            var jsonContent = File.ReadAllText(SettingsFilePath);
            var settings = JsonSerializer.Deserialize<Settings>(jsonContent, JsonOptions);
            
            return settings ?? new Settings();
        }
        catch (Exception ex)
        {
            // Log error (could implement logging service later)
            System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            
            // Return default settings if loading fails
            return new Settings();
        }
    }

    /// <summary>
    /// Saves the provided settings to the JSON file
    /// </summary>
    /// <param name="settings">The settings to save</param>
    /// <returns>True if save was successful, false otherwise</returns>
    public static bool SaveSettings(Settings settings)
    {
        try
        {
            // Ensure settings directory exists
            if (!Directory.Exists(SettingsDirectory))
            {
                Directory.CreateDirectory(SettingsDirectory);
            }

            // Serialize and write settings to file
            var jsonContent = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(SettingsFilePath, jsonContent);
            
            return true;
        }
        catch (Exception ex)
        {
            // Log error (could implement logging service later)
            System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets the full path to the settings file
    /// </summary>
    /// <returns>The settings file path</returns>
    public static string GetSettingsFilePath()
    {
        return SettingsFilePath;
    }

    /// <summary>
    /// Checks if the settings file exists
    /// </summary>
    /// <returns>True if settings file exists, false otherwise</returns>
    public static bool SettingsFileExists()
    {
        return File.Exists(SettingsFilePath);
    }

    /// <summary>
    /// Creates a backup of the current settings file
    /// </summary>
    /// <returns>True if backup was successful, false otherwise</returns>
    public static bool BackupSettings()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
                return false;

            var backupPath = SettingsFilePath + $".backup.{DateTime.Now:yyyyMMdd_HHmmss}";
            File.Copy(SettingsFilePath, backupPath);
            
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error backing up settings: {ex.Message}");
            return false;
        }
    }
} 