namespace SmartHiring.APIs.DTOs
{
    public class AgencyApplicationsBreakdownReportDto
    {
        public int TotalApplications { get; set; }
        public List<AgencyApplicationsBreakdownDto> Breakdown { get; set; }
    }
}
