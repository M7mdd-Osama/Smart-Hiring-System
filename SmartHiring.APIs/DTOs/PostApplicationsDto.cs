namespace SmartHiring.APIs.DTOs
{
    public class PostApplicationsDto
    {
        public int TotalApplications { get; set; }
        public int TotalJobs { get; set; }
        public List<JobApplicationDataDto> Jobs { get; set; } = new();
    }
}
