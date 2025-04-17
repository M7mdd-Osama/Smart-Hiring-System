namespace SmartHiring.APIs.DTOs
{
    public class CompanyCountReportDto
    {
        public int TotalCompanies { get; set; }
        public List<CompanyDto> Companies { get; set; }
        public Dictionary<int, int> CompaniesPerYear { get; set; }
    }
}
