using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class JobsWithInterviewsSpec : BaseSpec<Post>
    {
        public JobsWithInterviewsSpec(int companyId)
            : base(j => j.Company.Id == companyId)
        {
            AddInclude(j => j.Interviews);
        }

    }
}
