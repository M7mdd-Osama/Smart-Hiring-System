namespace SmartHiring.APIs.DTOs
{
    public class PendingInterviewDto
    {
        public string ApplicantName { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
