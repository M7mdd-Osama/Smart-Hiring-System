using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class JobClosedSpec : BaseSpec<Post>
    {
        public JobClosedSpec(int companyId)
        : base(p => p.CompanyId == companyId && p.JobStatus == "Closed") 
        {
        }
    }
}
