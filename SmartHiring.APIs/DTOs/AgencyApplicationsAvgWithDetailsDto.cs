namespace SmartHiring.APIs.DTOs
{
    public class AgencyApplicationsAvgWithDetailsDto
    {
        public double AverageApplicationsPerAgency { get; set; }
        public List<AgencyApplicationsBreakdownDto> Breakdown { get; set; }
    }
}
