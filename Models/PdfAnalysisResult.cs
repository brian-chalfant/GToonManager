namespace GToonManager.Models;

public class PdfAnalysisResult
{
    public bool IsValid { get; set; }
    public int TotalFields { get; set; }
    public int TextFields { get; set; }
    public int CheckboxFields { get; set; }
    public bool IsEncrypted { get; set; }
    public List<string>? FieldNames { get; set; }
    public string? ErrorMessage { get; set; }
} 