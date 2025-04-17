using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PendingInterviewsSpec : BaseSpec<Interview>
    {
        public PendingInterviewsSpec(DateTime fromDate, DateTime toDate, int companyId)
        : base(i =>
            i.Date >= fromDate &&
            i.Date <= toDate &&
            i.InterviewStatus == InterviewStatus.Pending &&
            i.Post.CompanyId == companyId)
        {
            AddInclude(i => i.Post);
            AddInclude(i => i.Applicant);
        }
    }
}
