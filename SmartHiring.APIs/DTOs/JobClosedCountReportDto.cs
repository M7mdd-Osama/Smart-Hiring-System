namespace SmartHiring.APIs.DTOs
{
    public class JobClosedCountReportDto
    {
        public int TotalJobs { get; set; }  // Total number of jobs
        public List<ClosedJobDto> JobsClosed { get; set; }  // List of closed jobs
        public int TotalClosedJobs { get; set; }  // Total number of closed jobs
    }
}
