using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class AcceptedCandidateListsSpec : BaseSpec<CandidateList>
	{
		public AcceptedCandidateListsSpec(int companyId)
			: base(cl => cl.Status == "Accepted" && cl.Post.CompanyId == companyId)
		{
			AddInclude(cl => cl.Post);
			AddInclude(cl => cl.CandidateListApplicants);
			AddIncludeString("CandidateListApplicants.Applicant");
			AddInclude(cl => cl.Post.Interviews);
		}
	}
}