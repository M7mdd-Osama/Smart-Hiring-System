using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class CandidateListApplicantsSpecification : BaseSpecifications<CandidateListApplicant>
	{
		public CandidateListApplicantsSpecification(int candidateListId, string hrId)
			: base(cla =>
			cla.CandidateListId == candidateListId &&
			cla.CandidateList.Post.Company.HRId == hrId)
		{
			AddInclude(cla => cla.Applicant);
			AddInclude(cla => cla.Applicant.Applications);
			AddInclude(cla => cla.CandidateList);
			AddInclude(cla => cla.CandidateList.Post.Company);
		}
	}
}
