namespace SmartHiring.Core.Entities
{
	public class Applicant : BaseEntity
	{
		public string FName { get; set; }
		public string LName { get; set; }
		public string Email { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Phone { get; set; }

		public ICollection<ApplicantSkill> ApplicantSkills { get; set; } = new HashSet<ApplicantSkill>();

		public ICollection<ApplicantAddress> ApplicantAddresses { get; set; } = new HashSet<ApplicantAddress>();
		public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
		public ICollection<CandidateListApplicant> CandidateListApplicants { get; set; } = new HashSet<CandidateListApplicant>();
	}
}
