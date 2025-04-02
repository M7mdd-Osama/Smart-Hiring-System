using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class ApplicantSpecification : BaseSpecifications<CandidateListApplicant>
	{
		public ApplicantSpecification(int candidateListId, int applicantId)
			: base(cl => cl.CandidateListId == candidateListId && cl.ApplicantId == applicantId)
		{
			AddInclude(cl => cl.Applicant);
			AddInclude(cl => cl.CandidateList);
			AddInclude(cl => cl.CandidateList.Post);
			AddInclude(cl => cl.CandidateList.Post.Company);
			AddInclude(cl => cl.Applicant.Applications);
			AddIncludeString("Applicant.Applications.Agency");
		}
	}
}
