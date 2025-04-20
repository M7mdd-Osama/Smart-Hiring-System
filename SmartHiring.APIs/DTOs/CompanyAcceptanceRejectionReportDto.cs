namespace SmartHiring.APIs.DTOs
{
    public class CompanyAcceptanceRejectionReportDto
    {
        public int TotalApplications { get; set; }
        public int Accepted { get; set; }
        public int Rejected { get; set; }
        public List<CompanyAcceptanceRejectionDto> CompanyReports { get; set; }
    }
}
