using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class JobsWithApplicationsSpec : BaseSpec<Post>
    {
        public JobsWithApplicationsSpec(int companyId, DateTime fromDate, DateTime toDate)
            : base(j => j.Company.Id == companyId && j.PostDate >= fromDate && j.PostDate <= toDate)
        {
            AddInclude(j => j.Applications);
        }
    }
}
