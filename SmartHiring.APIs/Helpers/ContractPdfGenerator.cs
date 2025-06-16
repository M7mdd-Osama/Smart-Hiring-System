using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SmartHiring.Core.Entities;

namespace SmartHiring.APIs.Helpers
{
    public static class ContractPdfGenerator
    {
        public static byte[] Generate(Interview interview)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.EnableDebugging = true;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Segoe UI"));

                    // Header
                    page.Header().MinHeight(80).Background(Colors.Blue.Darken2).Padding(20).Row(row =>
                    {
                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("EMPLOYMENT CONTRACT")
                                .FontSize(24).Bold().FontColor(Colors.White);
                            col.Item().Text("Professional Agreement")
                                .FontSize(12).FontColor(Colors.Grey.Lighten2);
                        });

                        row.ConstantColumn(120).AlignRight().Column(col =>
                        {
                            col.Item().Text("Contract Date")
                                .FontSize(10).FontColor(Colors.Grey.Lighten2);
                            col.Item().Text($"{DateTime.UtcNow:MMM dd, yyyy}")
                                .FontSize(14).Bold().FontColor(Colors.White);
                        });
                    });

                    // Content
                    page.Content().PaddingVertical(10).PaddingHorizontal(20).Column(col =>
                    {
                        // Company Information
                        col.Item().PaddingBottom(10).Column(section =>
                        {
                            section.Item().Background(Colors.Grey.Lighten4).Padding(15).Column(companyCol =>
                            {
                                companyCol.Item().Text("COMPANY INFORMATION")
                                    .FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                                companyCol.Item().PaddingTop(5).Text($"{interview.Post.Company.Name}")
                                    .FontSize(16).Bold().FontColor(Colors.Blue.Darken1);
                                companyCol.Item().Text($"Position: {interview.Post.JobTitle}")
                                    .FontSize(12).FontColor(Colors.Grey.Darken2);
                                companyCol.Item().Text($"Email: {interview.Post.Company.BusinessEmail ?? "N/A"}")
                                    .FontSize(12).FontColor(Colors.Grey.Darken2);
                                companyCol.Item().Text($"Address: {interview.Post.City ?? "N/A"}, {interview.Post.Country ?? "N/A"}")
                                    .FontSize(12).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        // Candidate Information
                        col.Item().PaddingBottom(10).Column(section =>
                        {
                            section.Item().Background(Colors.Grey.Lighten3).Padding(15).Column(candCol =>
                            {
                                candCol.Item().Text("CANDIDATE INFORMATION")
                                    .FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                                candCol.Item().PaddingTop(5).Text($"{interview.Applicant.FName} {interview.Applicant.LName}")
                                    .FontSize(16).Bold().FontColor(Colors.Blue.Darken1);
                                candCol.Item().Text($"Email: {interview.Applicant.Email ?? "N/A"}")
                                    .FontSize(12).FontColor(Colors.Grey.Darken2);
                                candCol.Item().Text($"Phone: {interview.Applicant.Phone ?? "N/A"}")
                                    .FontSize(12).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        // Contract Body
                        col.Item().PaddingBottom(15).Column(section =>
                        {
                            section.Item().Text("CONTRACT TERMS")
                                .FontSize(14).Bold().FontColor(Colors.Blue.Darken2);

                            section.Item().PaddingTop(5).Text(
                                "This employment contract confirms that the candidate has been selected for the position mentioned above. " +
                                "By signing this document, both parties agree to the terms and conditions of the employment, including duties, compensation, " +
                                "working hours, and confidentiality policies.")
                                .FontSize(11).FontColor(Colors.Grey.Darken3).LineHeight(1.5f);

                            // 👇 Add Interview Score here
                            section.Item().PaddingTop(10).Text($"Interview Score: {interview.Score}/100")
                                .FontSize(12).Bold().FontColor(Colors.Green.Darken2);
                        });

                        // Signature Section
                        col.Item().PaddingVertical(15).Row(row =>
                        {
                            // Candidate Signature (empty box)
                            row.RelativeColumn().Column(candidateCol =>
                            {
                                candidateCol.Item().PaddingBottom(5).Text("Candidate Signature")
                                    .FontSize(10).FontColor(Colors.Grey.Darken1);

                                candidateCol.Item()
                                    .Border(1).BorderColor(Colors.Grey.Medium).MinHeight(60).Padding(5);

                                candidateCol.Item().PaddingTop(5).Text("Date: ________________")
                                    .FontSize(10).FontColor(Colors.Grey.Darken1);
                            });

                            row.ConstantColumn(20); // spacing

                            // Company Representative Signature (name inside, date under)
                            row.RelativeColumn().Column(companyCol =>
                            {
                                var managerName = interview.Post.Company.Manager?.DisplayName ?? "Company Representative";

                                companyCol.Item().PaddingBottom(5).Text("Company Representative")
                                    .FontSize(10).FontColor(Colors.Grey.Darken1);

                                companyCol.Item().Border(1).BorderColor(Colors.Grey.Medium).MinHeight(60).Padding(5)
                                    .AlignMiddle().AlignCenter().Text(managerName)
                                    .FontSize(18).Bold().FontColor(Colors.Blue.Darken1);

                                companyCol.Item().PaddingTop(5).Text($"Date: {DateTime.UtcNow:MMM dd, yyyy}")
                                    .FontSize(10).FontColor(Colors.Grey.Darken1);
                            });
                        });
                        // Welcome Message
                        col.Item().PaddingTop(10).Background(Colors.Green.Lighten5).Padding(10).Column(welcomeCol =>
                        {
                            welcomeCol.Item().Text("Welcome to the Team!")
                                .FontSize(16).Bold().FontColor(Colors.Green.Darken2).AlignCenter();
                            welcomeCol.Item().PaddingTop(2).Text(
                                "We look forward to working with you and wish you success in your new role.")
                                .FontSize(11).FontColor(Colors.Green.Darken1).AlignCenter().Italic();
                        });
                    });

                    // Footer
                    page.Footer().MinHeight(30).Background(Colors.Grey.Darken3).Padding(5).Row(row =>
                    {
                        row.RelativeColumn().Text($"© {DateTime.UtcNow.Year} {interview.Post.Company.Name}")
                            .FontSize(9).FontColor(Colors.Grey.Lighten2);
                        row.RelativeColumn().Text($"Generated on {DateTime.UtcNow:MMM dd, yyyy} at {DateTime.UtcNow:HH:mm}")
                            .FontSize(9).FontColor(Colors.Grey.Lighten2).AlignRight();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
