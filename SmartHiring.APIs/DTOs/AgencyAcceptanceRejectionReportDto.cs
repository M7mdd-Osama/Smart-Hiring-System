namespace SmartHiring.APIs.DTOs
{
    public class AgencyAcceptanceRejectionReportDto
    {
        public string AgencyName { get; set; }
        public int TotalApplications { get; set; }
        public int Accepted { get; set; }
        public int Rejected { get; set; }
        public double AcceptanceRate { get; set; }
        public double RejectionRate { get; set; }
    }
}
