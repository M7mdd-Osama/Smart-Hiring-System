using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsByCompanyJobsSpec : BaseSpec<Application>
    {
        public ApplicationsByCompanyJobsSpec(int companyId)
        : base(a => a.Post.CompanyId == companyId)
        {
            AddInclude(a => a.Post);
        }
    }
}
