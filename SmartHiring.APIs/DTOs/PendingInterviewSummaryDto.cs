namespace SmartHiring.APIs.DTOs
{
    public class PendingInterviewSummaryDto
    {

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalInterviews { get; set; }
        public int TotalPendingInterviews { get; set; }
        public List<PendingInterviewDto> PendingInterviews { get; set; }

    }
}
