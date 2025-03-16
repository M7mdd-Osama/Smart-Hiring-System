using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CompaniesWithDetailsSpecification : BaseSpecifications<Company>
    {
        public CompaniesWithDetailsSpecification()
        {
			AddInclude(c => c.HR);
            AddInclude(c => c.Manager);
        }
        public CompaniesWithDetailsSpecification(int companyId) : base(c => c.Id == companyId)
        {
            AddInclude(c => c.HR);
            AddInclude(c => c.Manager);
        }
    }
}
