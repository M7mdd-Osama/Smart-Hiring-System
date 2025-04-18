namespace SmartHiring.APIs.DTOs
{
    public class AgencyApplicationsBreakdownDto
    {
        public string AgencyIdString { get; set; }
        public string AgencyName { get; set; }
        public int ApplicationsCount { get; set; }
        public List<ApplicationDetailDto> Applications { get; set; }
    }

}
