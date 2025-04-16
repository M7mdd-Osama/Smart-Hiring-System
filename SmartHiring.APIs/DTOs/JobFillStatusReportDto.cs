namespace SmartHiring.APIs.DTOs
{
    public class JobFillStatusReportDto
    {
        public int TotalPosts { get; set; }
        public int FilledPosts { get; set; }
        public int UnfilledPosts { get; set; }

        public List<string> FilledPostsData { get; set; } = new();
        public List<string> UnfilledPostsData { get; set; } = new();
    }

}
