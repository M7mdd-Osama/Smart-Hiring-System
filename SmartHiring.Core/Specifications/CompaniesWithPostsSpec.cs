using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{

    public class CompaniesWithPostsSpec : BaseSpec<Company>
    {
        public CompaniesWithPostsSpec(int? companyId)
            : base(c => companyId == null || c.Id == companyId)
        {
            AddInclude(c => c.Posts);
        }
    }
}
