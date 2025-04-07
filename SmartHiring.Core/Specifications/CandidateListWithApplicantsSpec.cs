using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CandidateListWithApplicantsSpec : BaseSpecifications<CandidateList>
    {
        public CandidateListWithApplicantsSpec(int postId)
            : base(cl => cl.PostId == postId && cl.Status == "Pending")
        {
            AddInclude(cl => cl.CandidateListApplicants);
            AddIncludeString("CandidateListApplicants.Applicant");
            AddIncludeString("CandidateListApplicants.Applicant.Applications");
            AddInclude(cl => cl.Post);
        }
    }
}
