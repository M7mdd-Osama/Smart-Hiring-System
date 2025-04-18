namespace SmartHiring.APIs.DTOs
{
    public class PaidJobsCountReportDto
    {
        public int TotalPaidJobs { get; set; }
        public List<PaidJobInfoDto> Jobs { get; set; }
        public DateTime FromDate { get; set; } // إضافة التاريخ
        public DateTime ToDate { get; set; }   // إضافة التاريخ
    }
}
