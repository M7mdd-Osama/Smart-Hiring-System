using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CompaniesWithDetailsSpec : BaseSpec<Company>
    {
        public CompaniesWithDetailsSpec(string? search) : base(c =>
            string.IsNullOrEmpty(search) || c.Name.ToLower().Contains(search.ToLower()))
        {
            AddInclude(c => c.HR);
            AddInclude(c => c.Manager);
            AddOrderBy(c => c.Name);

        }
    }
}
