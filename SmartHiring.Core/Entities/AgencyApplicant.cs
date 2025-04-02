using SmartHiring.Core.Entities.Identity;

namespace SmartHiring.Core.Entities
{
	public class AgencyApplicant
	{
		public string AgencyId { get; set; }
		public AppUser Agency { get; set; }

		public int ApplicantId { get; set; }
		public Applicant Applicant { get; set; }
	}
}
