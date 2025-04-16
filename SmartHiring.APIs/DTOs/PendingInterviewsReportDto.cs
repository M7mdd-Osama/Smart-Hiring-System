namespace SmartHiring.APIs.DTOs
{
    public class PendingInterviewsReportDto
    {
        public int TotalPendingInterviews { get; set; }
        public List<PendingInterviewCandidateDto> Candidates { get; set; }
    }
}
