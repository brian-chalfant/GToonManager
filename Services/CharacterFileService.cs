using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Win32;
using GToonManager.Models;

namespace GToonManager.Services;

public class CharacterFileService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly RecentFilesService _recentFilesService;

    public CharacterFileService()
    {
        _recentFilesService = new RecentFilesService();
    }

    public RecentFilesService RecentFiles => _recentFilesService;

    public class CharacterSaveData
    {
        public Character Character { get; set; } = new();
        public CharacterFileMetadata Metadata { get; set; } = new();
    }

    public class CharacterFileMetadata
    {
        public string Version { get; set; } = "1.0";
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
        public string AppVersion { get; set; } = "1.0.0";
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// Opens a file dialog to save a character to a .gtchar file
    /// </summary>
    public async Task<(bool Success, string? FilePath)> SaveCharacterAsAsync(Character character)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "GToon Character Files (*.gtchar)|*.gtchar|All Files (*.*)|*.*",
            DefaultExt = "gtchar",
            FileName = SanitizeFileName($"{character.Name}_{character.ClassSummary}".Replace(" / ", "_"))
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            var success = await SaveCharacterToFileAsync(character, saveFileDialog.FileName);
            return (success, success ? saveFileDialog.FileName : null);
        }

        return (false, null);
    }

    /// <summary>
    /// Saves a character to the specified file path
    /// </summary>
    public async Task<bool> SaveCharacterToFileAsync(Character character, string filePath)
    {
        try
        {
            var saveData = new CharacterSaveData
            {
                Character = character,
                Metadata = new CharacterFileMetadata
                {
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    Description = $"Character: {character.Name} ({character.ClassSummary})"
                }
            };

            var json = JsonSerializer.Serialize(saveData, JsonOptions);
            await File.WriteAllTextAsync(filePath, json);
            
            // Add to recent files
            _recentFilesService.AddRecentFile(filePath, character.Name);
            
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (you might want to add a logging service)
            System.Diagnostics.Debug.WriteLine($"Error saving character: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Opens a file dialog to load a character from a .gtchar file
    /// </summary>
    public async Task<(Character? Character, string? FilePath)> LoadCharacterAsync()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "GToon Character Files (*.gtchar)|*.gtchar|All Files (*.*)|*.*",
            DefaultExt = "gtchar"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var character = await LoadCharacterFromFileAsync(openFileDialog.FileName);
            return (character, character != null ? openFileDialog.FileName : null);
        }

        return (null, null);
    }

    /// <summary>
    /// Loads a character from the specified file path
    /// </summary>
    public async Task<Character?> LoadCharacterFromFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var saveData = JsonSerializer.Deserialize<CharacterSaveData>(json, JsonOptions);

            var character = saveData?.Character;
            
            // Add to recent files if successfully loaded
            if (character != null)
            {
                _recentFilesService.AddRecentFile(filePath, character.Name);
            }

            return character;
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error parsing character file: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading character: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets file metadata without loading the entire character
    /// </summary>
    public async Task<CharacterFileMetadata?> GetFileMetadataAsync(string filePath)
    {
        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var saveData = JsonSerializer.Deserialize<CharacterSaveData>(json, JsonOptions);
            return saveData?.Metadata;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error reading file metadata: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Validates if a file is a valid GToon character file
    /// </summary>
    public async Task<bool> IsValidCharacterFileAsync(string filePath)
    {
        try
        {
            var metadata = await GetFileMetadataAsync(filePath);
            return metadata != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a safe filename from character data
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var invalidChar in invalidChars)
        {
            fileName = fileName.Replace(invalidChar, '_');
        }
        return fileName.Trim();
    }

    /// <summary>
    /// Creates a quick save with timestamp
    /// </summary>
    public async Task<bool> QuickSaveCharacterAsync(Character character, string? basePath = null)
    {
        try
        {
            basePath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = $"{SanitizeFileName(character.Name)}_{DateTime.Now:yyyyMMdd_HHmmss}.gtchar";
            var filePath = Path.Combine(basePath, "GToon Characters", fileName);
            
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            
            return await SaveCharacterToFileAsync(character, filePath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in quick save: {ex.Message}");
            return false;
        }
    }
} 