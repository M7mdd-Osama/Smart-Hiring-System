using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CompanyWithMembersSpecifications : BaseSpecifications<Company>
    {
        public CompanyWithMembersSpecifications(int companyId)
            : base(c => c.Id == companyId)
        {
            AddInclude(c => c.HR);
            AddInclude(c => c.Manager);
        }
    }
}
