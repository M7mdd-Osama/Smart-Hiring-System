using System.Text;
using UglyToad.PdfPig;

namespace SmartHiring.APIs.Helpers
{
    public class PdfTextExtractor
    {
        public async Task<string> ExtractTextFromPdfAsync(string filePath)
        {
            var sb = new StringBuilder();

            using (PdfDocument document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    sb.Append(page.Text);
                }
            }
            return sb.ToString();
        }
    }
}

