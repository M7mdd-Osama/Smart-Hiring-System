using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PostsByCompanySpec : BaseSpec<Post>
    {
        public PostsByCompanySpec(int companyId):base(p => p.CompanyId == companyId)
        {
        }
    }
}
