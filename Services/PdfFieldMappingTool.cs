using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using PdfSharp.Fonts;
using System.IO;

namespace GToonManager.Services;

public class PdfFieldMappingTool
{
    public static Task<bool> CreateFieldMappingPdfAsync(string templatePath, string outputPath)
    {
        try
        {
            // Initialize font resolver if not already set
            if (GlobalFontSettings.FontResolver == null)
            {
                GlobalFontSettings.FontResolver = new PdfSharpFontResolver();
            }

            if (!File.Exists(templatePath))
            {
                System.Diagnostics.Debug.WriteLine($"Template PDF not found at: {templatePath}");
                return Task.FromResult(false);
            }

            // Validate output path
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            System.Diagnostics.Debug.WriteLine($"Creating field mapping PDF from: {templatePath}");
            
            // Open the PDF template
            using var document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
            
            // Get the AcroForm (form fields)
            var form = document.AcroForm;
            if (form == null)
            {
                System.Diagnostics.Debug.WriteLine("No AcroForm found in PDF");
                return Task.FromResult(false);
            }

            System.Diagnostics.Debug.WriteLine($"Found AcroForm with {form.Fields.Count} fields");

            int fieldsProcessed = 0;
            var names = form.Fields.Names;
            
            // Fill each field with its own name (shortened for readability)
            foreach (var fieldName in names)
            {
                if (string.IsNullOrEmpty(fieldName))
                    continue;

                try
                {
                    var field = form.Fields[fieldName];
                    if (field != null)
                    {
                        if (field is PdfTextField textField)
                        {
                            // Put a shortened version of the field name in the field
                            var shortName = fieldName.Length > 12 ? fieldName.Substring(0, 12) : fieldName;
                            textField.Value = new PdfString(shortName);
                            fieldsProcessed++;
                        }
                        else if (field is PdfCheckBoxField checkBox)
                        {
                            // Check some checkboxes to show where they are
                            if (fieldsProcessed % 5 == 0) // Check every 5th checkbox
                            {
                                checkBox.Checked = true;
                            }
                            fieldsProcessed++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error setting field '{fieldName}': {ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Processed {fieldsProcessed} fields");

            // Set NeedAppearances to make text visible
            if (form.Elements.ContainsKey("/NeedAppearances"))
            {
                form.Elements["/NeedAppearances"] = new PdfBoolean(true);
            }
            else
            {
                form.Elements.Add("/NeedAppearances", new PdfBoolean(true));
            }

            // Save the document
            document.Save(outputPath);
            System.Diagnostics.Debug.WriteLine($"Field mapping PDF saved to: {outputPath}");

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating field mapping PDF: {ex.GetType().Name}: {ex.Message}");
            return Task.FromResult(false);
        }
    }
} 