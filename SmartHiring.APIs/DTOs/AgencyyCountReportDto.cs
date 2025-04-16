namespace SmartHiring.APIs.DTOs
{
    public class AgencyyCountReportDto
    {
        public int CompanyId { get; set; }
        public int TotalAgencies { get; set; }
        public List<AgencyyApplicationStatsDto> Agencies { get; set; } = new();
    }
}
