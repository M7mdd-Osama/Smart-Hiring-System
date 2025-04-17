using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class JobWithCompanySpec : BaseSpec<Post>
    {
        public JobWithCompanySpec(int jobId)
            : base(p => p.Id == jobId)
        {
            AddInclude(p => p.Company);
        }
    }
}
