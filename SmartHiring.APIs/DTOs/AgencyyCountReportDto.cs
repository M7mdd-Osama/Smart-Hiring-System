namespace SmartHiring.APIs.DTOs
{
    public class AgencyyCountReportDto
    {
        public int CompanyId { get; set; }
        public int TotalAgencies { get; set; }
        public DateTime FromDate { get; set; }  // إضافة FromDate
        public DateTime ToDate { get; set; }    // إضافة ToDate
        public List<AgencyyApplicationStatsDto> Agencies { get; set; } = new();
    }
}
