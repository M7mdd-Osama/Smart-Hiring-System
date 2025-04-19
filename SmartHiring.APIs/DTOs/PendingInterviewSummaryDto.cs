namespace SmartHiring.APIs.DTOs
{
    public class PendingInterviewSummaryDto
    {

        public int TotalInterviews { get; set; }
        public int TotalPendingInterviews { get; set; }
        public List<PendingInterviewDto> PendingInterviews { get; set; }

    }
}
