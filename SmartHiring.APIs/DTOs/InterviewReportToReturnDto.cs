namespace SmartHiring.APIs.DTOs
{
	public class InterviewReportToReturnDto
	{
		public int TotalCandidates { get; set; }
		public int AcceptedCandidates { get; set; }
		public int RejectedCandidates { get; set; }
		public ICollection<CandidateReportToReturnDto> Candidates { get; set; } = new HashSet<CandidateReportToReturnDto>();

	}
}
