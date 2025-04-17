using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class InterviewWithCandidateSpec : BaseSpec<Interview>
    {
        public InterviewWithCandidateSpec(DateTime fromDate, DateTime toDate, int companyId)
            : base(i =>
                i.Date >= fromDate &&
                i.Date <= toDate &&
                i.Post.CompanyId == companyId)
        {
            AddInclude(i => i.Applicant);
            AddInclude(i => i.HR);
            AddInclude(i => i.Post);
        }
    }
}
