namespace SmartHiring.APIs.DTOs
{
    public class AgencyCountReportDto
    {
        public int TotalAgencies { get; set; }
        public List<AgencyInfoDto> AgenciesData { get; set; } 
        public Dictionary<int, int> AgenciesPerYear { get; set; }
    }
}
