namespace SmartHiring.APIs.DTOs
{
    public class CandidateListApplicantDto
    {
        public int ApplicantId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CV_Link { get; set; }
        public string InterviewStatus { get; set; }
        public DateTime InterviewDate { get; set; }
        public TimeSpan InterviewTime { get; set; }
        public string Location { get; set; }
    }
}
