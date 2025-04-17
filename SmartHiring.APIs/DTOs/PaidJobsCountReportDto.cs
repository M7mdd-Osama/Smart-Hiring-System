namespace SmartHiring.APIs.DTOs
{
    public class PaidJobsCountReportDto
    {
        public int TotalPaidJobs { get; set; }
        public List<PaidJobInfoDto> Jobs { get; set; }
    }
}
