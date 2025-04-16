namespace SmartHiring.APIs.DTOs
{
    public class RankedApplicationsReportDto
    {
        public int TotalApplications { get; set; }
        public List<ApplicationRankedDto> TopApplications { get; set; }
    }
}
