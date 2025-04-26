namespace SmartHiring.APIs.DTOs
{
    public class AgencyyCountReportDto
    {
        public int TotalAgencies { get; set; }
        public List<AgencyyApplicationStatusDto> Agencies { get; set; } = new();
    }
}
