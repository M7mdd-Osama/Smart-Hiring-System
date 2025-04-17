using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class InterviewByStatusSpec : BaseSpec<Interview>
    {
        public InterviewByStatusSpec(int companyId, InterviewStatus status)
        : base(i => i.Post.CompanyId == companyId && i.InterviewStatus == status)
        {
            AddInclude(i => i.Post);
        }
    }
}
