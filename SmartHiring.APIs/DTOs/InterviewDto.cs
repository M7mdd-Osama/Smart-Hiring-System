namespace SmartHiring.APIs.DTOs
{
    public class InterviewDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Location { get; set; }
        public string InterviewStatus { get; set; } = "Pending";

		public double Score { get; set; }
        public string HRId { get; set; }
        public int PostId { get; set; }
        public int ApplicantId { get; set; }
    }
}
