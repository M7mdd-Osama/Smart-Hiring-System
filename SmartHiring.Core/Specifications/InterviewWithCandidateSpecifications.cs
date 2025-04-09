using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class InterviewWithCandidateSpecifications : BaseSpecifications<Interview>
    {
        public InterviewWithCandidateSpecifications(DateTime fromDate, DateTime toDate, int companyId)
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
