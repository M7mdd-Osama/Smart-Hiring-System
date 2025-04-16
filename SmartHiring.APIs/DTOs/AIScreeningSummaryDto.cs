namespace SmartHiring.APIs.DTOs
{
    public class AIScreeningSummaryDto
    {
        public List<ApplicantDto> AcceptedApplicants { get; set; }
        public List<ApplicantDto> RejectedApplicants { get; set; }
    }

}
