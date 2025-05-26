using PdfSharp.Fonts;
using System.IO;

namespace GToonManager.Services;

public class PdfSharpFontResolver : IFontResolver
{
    public string DefaultFontName => "Arial";

    public byte[]? GetFont(string faceName)
    {
        try
        {
            // Try to find the font in Windows system fonts
            string fontPath = GetSystemFontPath(faceName);
            
            if (!string.IsNullOrEmpty(fontPath) && File.Exists(fontPath))
            {
                return File.ReadAllBytes(fontPath);
            }

            // Fallback to Arial if requested font not found
            if (faceName != "Arial")
            {
                fontPath = GetSystemFontPath("Arial");
                if (!string.IsNullOrEmpty(fontPath) && File.Exists(fontPath))
                {
                    return File.ReadAllBytes(fontPath);
                }
            }

            // Last resort: try to find any available font
            var winFontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));
            var possibleFonts = new[] { "arial.ttf", "calibri.ttf", "times.ttf", "cour.ttf" };
            
            foreach (var font in possibleFonts)
            {
                var path = Path.Combine(winFontsDir, font);
                if (File.Exists(path))
                {
                    return File.ReadAllBytes(path);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Font resolver error: {ex.Message}");
        }

        return null;
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        try
        {
            // Normalize font family name
            string fontName = familyName.ToLower() switch
            {
                "courier new" or "courier" => "Courier New",
                "times new roman" or "times" => "Times New Roman",
                "arial" => "Arial",
                "calibri" => "Calibri",
                _ => familyName
            };

            // Create a font face name based on style
            string faceName = fontName;
            if (isBold && isItalic)
                faceName += " Bold Italic";
            else if (isBold)
                faceName += " Bold";
            else if (isItalic)
                faceName += " Italic";

            return new FontResolverInfo(faceName);
        }
        catch
        {
            // Fallback to Arial
            return new FontResolverInfo("Arial");
        }
    }

    private string GetSystemFontPath(string fontName)
    {
        var winFontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));
        
        // Common font file mappings
        var fontMappings = new Dictionary<string, string[]>
        {
            ["Courier New"] = new[] { "cour.ttf", "courbd.ttf", "couri.ttf", "courbi.ttf" },
            ["Arial"] = new[] { "arial.ttf", "arialbd.ttf", "ariali.ttf", "arialbi.ttf" },
            ["Times New Roman"] = new[] { "times.ttf", "timesbd.ttf", "timesi.ttf", "timesbi.ttf" },
            ["Calibri"] = new[] { "calibri.ttf", "calibrib.ttf", "calibrii.ttf", "calibriz.ttf" }
        };

        if (fontMappings.ContainsKey(fontName))
        {
            foreach (var fileName in fontMappings[fontName])
            {
                var path = Path.Combine(winFontsDir, fileName);
                if (File.Exists(path))
                {
                    return path;
                }
            }
        }

        // Try direct filename matching
        var possiblePaths = new[]
        {
            Path.Combine(winFontsDir, $"{fontName.ToLower().Replace(" ", "")}.ttf"),
            Path.Combine(winFontsDir, $"{fontName.ToLower().Replace(" ", "")}.otf"),
            Path.Combine(winFontsDir, $"{fontName}.ttf"),
            Path.Combine(winFontsDir, $"{fontName}.otf")
        };

        return possiblePaths.FirstOrDefault(File.Exists) ?? "";
    }
} 