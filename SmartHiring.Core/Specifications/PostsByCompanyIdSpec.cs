using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PostsByCompanyIdSpec :BaseSpec<Post>
    {
        public PostsByCompanyIdSpec(int companyId)
        : base(p => p.CompanyId == companyId)
        {
        }
    }
}
