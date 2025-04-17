using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithApplicantSpec : BaseSpec<Application>
    {
        public ApplicationsWithApplicantSpec(int jobId)
            : base(a => a.PostId == jobId)
        {
            AddInclude(a => a.Applicant);
            AddInclude(a => a.Post);
        }
    }
}
