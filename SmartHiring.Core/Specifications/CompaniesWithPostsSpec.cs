using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CompaniesWithPostsSpec : BaseSpec<Company>
    {
        public CompaniesWithPostsSpec()
            : base(c => true)
        {
            AddInclude(c => c.Posts);
        }
    }
}
