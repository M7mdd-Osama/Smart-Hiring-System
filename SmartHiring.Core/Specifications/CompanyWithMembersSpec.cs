using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class CompanyWithMembersSpec : BaseSpec<Company>
    {
        public CompanyWithMembersSpec(int companyId)
            : base(c => c.Id == companyId)
        {
            AddInclude(c => c.HR);
            AddInclude(c => c.Manager);
        }
    }
}
