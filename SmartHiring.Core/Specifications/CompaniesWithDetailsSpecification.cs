using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CompaniesWithDetailsSpecification : BaseSpecifications<Company>
    {
        public CompaniesWithDetailsSpecification()
        {
            Includes.Add(c => c.HR);
            Includes.Add(c => c.Manager);
        }
        public CompaniesWithDetailsSpecification(int companyId) : base(c => c.Id == companyId)
        {
            Includes.Add(c => c.HR);
            Includes.Add(c => c.Manager);
        }
    }
}
