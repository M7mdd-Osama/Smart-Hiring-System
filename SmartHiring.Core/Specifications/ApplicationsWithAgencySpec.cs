using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithAgencySpec : BaseSpec<Application>
    {
        public ApplicationsWithAgencySpec()
            : base(a => a.AgencyId != null)
        {
            AddInclude(a => a.Agency);
        }
        public ApplicationsWithAgencySpec(int companyId)
            : base(a => a.AgencyId != null && a.Post.CompanyId == companyId)
        {
            AddInclude(a => a.Agency);
            AddInclude(a => a.Post); 
        }
    }
}
