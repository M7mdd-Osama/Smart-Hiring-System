using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CompaniesWithDetailsSpecification : BaseSpecifications<Company>
    {
        public CompaniesWithDetailsSpecification(string? search) : base(c =>
            string.IsNullOrEmpty(search) || c.Name.ToLower().Contains(search.ToLower()))
        {
            AddInclude(c => c.HR);
            AddInclude(c => c.Manager);
            AddOrderBy(c => c.Name);

        }
        public CompaniesWithDetailsSpecification(int companyId) : base(c => c.Id == companyId)
        {
            AddInclude(c => c.HR);
            AddInclude(c => c.Manager);
        }
    }
}
