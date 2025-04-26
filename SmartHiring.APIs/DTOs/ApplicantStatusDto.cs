namespace SmartHiring.APIs.DTOs
{
    public class ApplicantStatusDto
    {
        public int TotalApplicants { get; set; }
        public List<ApplicantInfoDto> ApplicantData { get; set; }
    }
}
