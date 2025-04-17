namespace SmartHiring.APIs.DTOs
{
    public class AgencyApplicationsBreakdownDto
    {
        public string AgencyIdString { get; set; }  // ← بدل int
        public string AgencyName { get; set; }
        public int ApplicationsCount { get; set; }
    }

}
