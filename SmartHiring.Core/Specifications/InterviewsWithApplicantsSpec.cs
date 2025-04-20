using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class InterviewsWithApplicantsSpec : BaseSpec<Interview>
    {
        public InterviewsWithApplicantsSpec(DateTime fromDate, DateTime toDate, int companyId)
            : base(i =>
                i.Date >= fromDate.Date &&
                i.Date < toDate.Date.AddDays(1) &&
                i.Post.CompanyId == companyId)
        {
            AddInclude(i => i.Applicant);
            AddInclude(i => i.Post);
        }
    }

}
