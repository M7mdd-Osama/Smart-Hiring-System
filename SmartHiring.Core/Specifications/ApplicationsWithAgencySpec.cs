using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithAgencySpec : BaseSpec<Application>
    {
        public ApplicationsWithAgencySpec()
            : base(a => a.AgencyId != null)
        {
        }

        public ApplicationsWithAgencySpec(int companyId)
            : base(a => a.AgencyId != null && a.Post.CompanyId == companyId)
        {
            AddInclude(a => a.Post); 
        }
    }
}
