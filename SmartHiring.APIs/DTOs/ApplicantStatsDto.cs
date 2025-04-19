namespace SmartHiring.APIs.DTOs
{
    public class ApplicantStatsDto
    {
        public int TotalApplicants { get; set; }
        public List<ApplicantInfoDto> ApplicantData { get; set; }
    }
}
