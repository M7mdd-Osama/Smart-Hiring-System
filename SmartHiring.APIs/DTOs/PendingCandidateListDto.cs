namespace SmartHiring.APIs.DTOs
{
    public class PendingCandidateListDto
    {
        public int CandidateListId { get; set; }
        public ICollection<PendingCandidateListApplicantDto> Applicants { get; set; }
    }
}
