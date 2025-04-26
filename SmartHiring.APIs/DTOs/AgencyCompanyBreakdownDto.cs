namespace SmartHiring.APIs.DTOs
{
    public class AgencyCompanyBreakdownDto
    {
        public string CompanyName { get; set; }
        public int ApplicationsCount { get; set; }
        public List<ApplicationDetailDto> Applications { get; set; }
    }
}
