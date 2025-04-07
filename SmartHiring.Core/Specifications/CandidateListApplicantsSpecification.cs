using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CandidateListApplicantsSpecification : BaseSpecifications<CandidateListApplicant>
    {
        public CandidateListApplicantsSpecification(int candidateListId, string userId)
             : base(cla =>
                 cla.CandidateListId == candidateListId &&
                (cla.CandidateList.Post.Company.HRId == userId ||
                 cla.CandidateList.Post.Company.ManagerId == userId))
        {
            AddInclude(cla => cla.Applicant);
            AddInclude(cla => cla.Applicant.Applications);
            AddInclude(cla => cla.CandidateList);
            AddInclude(cla => cla.CandidateList.Post.Company);
        }
    }
}
