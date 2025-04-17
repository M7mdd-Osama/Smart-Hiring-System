using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class InterviewsWithApplicantsSpec : BaseSpec<Interview>
    {
        public InterviewsWithApplicantsSpec(DateTime fromDate, DateTime toDate, int companyId)
        : base(i =>
            i.Post.CompanyId == companyId &&
            i.Date >= fromDate &&
            i.Date <= toDate)
        {
            AddInclude(i => i.Applicant);
            AddInclude(i => i.Post);
        }
    }
}
