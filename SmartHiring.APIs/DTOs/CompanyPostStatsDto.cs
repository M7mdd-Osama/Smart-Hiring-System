namespace SmartHiring.APIs.DTOs
{
    public class CompanyPostStatsDto
    {
        public int TotalCompanies { get; set; }
        public int TotalPosts { get; set; }
        public List<CompanyPostCountDto> PostsPerCompany { get; set; }
    }
}
