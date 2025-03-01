namespace SmartHiring.APIs.DTOs
{
    public class ApplicationDto
    {
        public int Id { get; set; }
        public double RankScore { get; set; }
        public bool IsShortlisted { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string CV_Link { get; set; }
        public int ApplicantId { get; set; }
        public int PostId { get; set; }
        public string? AgencyId { get; set; }
    }
}
