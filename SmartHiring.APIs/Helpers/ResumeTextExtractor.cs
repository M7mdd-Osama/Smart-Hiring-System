using System.Text;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;

namespace SmartHiring.APIs.Helpers
{
    public class ResumeTextExtractor
    {
        public async Task<string> ExtractTextAsync(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".pdf" => await ExtractFromPdf(filePath),
                ".docx" => await ExtractFromDocx(filePath),
                ".txt" => await ExtractFromTxt(filePath),
                _ => throw new NotSupportedException("Unsupported file format")
            };
        }
        private async Task<string> ExtractFromPdf(string path)
        {
            var sb = new StringBuilder();
            using var document = PdfDocument.Open(path);
            foreach (var page in document.GetPages())
                sb.Append(page.Text);
            return sb.ToString();
        }
        private async Task<string> ExtractFromDocx(string path)
        {
            using var doc = WordprocessingDocument.Open(path, false);
            return doc.MainDocumentPart.Document.Body.InnerText;
        }
        private async Task<string> ExtractFromTxt(string path)
        {
            return await File.ReadAllTextAsync(path);
        }
    }
}