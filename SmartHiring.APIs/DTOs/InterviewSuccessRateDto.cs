namespace SmartHiring.APIs.DTOs
{
    public class InterviewSuccessRateDto
    {
        public int TotalInterviews { get; set; }
        public int HiredCount { get; set; }
        public int FailedCount => TotalInterviews - HiredCount;
        public double SuccessRatePercentage { get; set; }

        public List<string> SuccessfulApplicants { get; set; } = new();
        public List<string> FailedApplicants { get; set; } = new();
    }
}
