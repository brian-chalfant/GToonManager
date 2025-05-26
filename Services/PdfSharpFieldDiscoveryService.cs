using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Fonts;
using System.IO;

namespace GToonManager.Services;

public class PdfSharpFieldDiscoveryService
{
    public static Dictionary<string, string> DiscoverFields(string pdfPath)
    {
        var fieldInfo = new Dictionary<string, string>();
        
        try
        {
            // Initialize font resolver if not already set
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new PdfSharpFontResolver();
            }
            
            if (!File.Exists(pdfPath))
            {
                return fieldInfo;
            }

            using var document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.ReadOnly);
            var form = document.AcroForm;
            
            if (form == null)
            {
                return fieldInfo;
            }

            var names = form.Fields.Names;
            foreach (var fieldName in names)
            {
                if (string.IsNullOrEmpty(fieldName))
                    continue;

                try
                {
                    var field = form.Fields[fieldName];
                    var fieldType = field?.GetType().Name ?? "Unknown";
                    var currentValue = "";
                    
                    try
                    {
                        currentValue = field?.Value?.ToString() ?? "";
                    }
                    catch
                    {
                        currentValue = "Could not read value";
                    }
                    
                    fieldInfo[fieldName] = $"Type: {fieldType}, Current Value: '{currentValue}'";
                }
                catch (Exception ex)
                {
                    fieldInfo[fieldName] = $"Error accessing field: {ex.Message}";
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error discovering PDF fields with PdfSharp: {ex.Message}");
        }

        return fieldInfo;
    }

    public static async Task WriteFieldsToFileAsync(string pdfPath, string outputPath)
    {
        var fields = DiscoverFields(pdfPath);
        
        var lines = new List<string>
        {
            "PDF Form Fields Discovery Report (PdfSharp)",
            "===========================================",
            $"Template: {Path.GetFileName(pdfPath)}",
            $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            "",
            "Field Name | Type | Current Value",
            "-----------|------|---------------"
        };

        foreach (var field in fields.OrderBy(f => f.Key))
        {
            lines.Add($"{field.Key} | {field.Value}");
        }

        await File.WriteAllLinesAsync(outputPath, lines);
    }
} 