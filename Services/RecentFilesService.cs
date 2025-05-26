using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GToonManager.Services;

public class RecentFilesService
{
    private const int MaxRecentFiles = 10;
    private readonly string _recentFilesPath;
    private List<RecentFileInfo> _recentFiles = new();

    public RecentFilesService()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GToonManager");
        Directory.CreateDirectory(appDataPath);
        _recentFilesPath = Path.Combine(appDataPath, "recentfiles.json");
        LoadRecentFiles();
    }

    public class RecentFileInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public string CharacterName { get; set; } = string.Empty;
        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
        public string DisplayName => $"{CharacterName} - {Path.GetFileName(FilePath)}";
    }

    public IReadOnlyList<RecentFileInfo> RecentFiles => _recentFiles.AsReadOnly();

    public void AddRecentFile(string filePath, string characterName)
    {
        try
        {
            // Remove existing entry if it exists
            _recentFiles.RemoveAll(rf => rf.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

            // Add to the beginning of the list
            _recentFiles.Insert(0, new RecentFileInfo
            {
                FilePath = filePath,
                CharacterName = characterName,
                LastAccessed = DateTime.UtcNow
            });

            // Keep only the most recent files
            if (_recentFiles.Count > MaxRecentFiles)
            {
                _recentFiles = _recentFiles.Take(MaxRecentFiles).ToList();
            }

            // Remove files that no longer exist
            _recentFiles.RemoveAll(rf => !File.Exists(rf.FilePath));

            SaveRecentFiles();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding recent file: {ex.Message}");
        }
    }

    public void RemoveRecentFile(string filePath)
    {
        try
        {
            _recentFiles.RemoveAll(rf => rf.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));
            SaveRecentFiles();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error removing recent file: {ex.Message}");
        }
    }

    public void ClearRecentFiles()
    {
        try
        {
            _recentFiles.Clear();
            SaveRecentFiles();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error clearing recent files: {ex.Message}");
        }
    }

    private void LoadRecentFiles()
    {
        try
        {
            if (File.Exists(_recentFilesPath))
            {
                var json = File.ReadAllText(_recentFilesPath);
                var files = JsonSerializer.Deserialize<List<RecentFileInfo>>(json);
                _recentFiles = files?.Where(rf => File.Exists(rf.FilePath)).ToList() ?? new List<RecentFileInfo>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading recent files: {ex.Message}");
            _recentFiles = new List<RecentFileInfo>();
        }
    }

    private void SaveRecentFiles()
    {
        try
        {
            var json = JsonSerializer.Serialize(_recentFiles, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_recentFilesPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving recent files: {ex.Message}");
        }
    }
} 