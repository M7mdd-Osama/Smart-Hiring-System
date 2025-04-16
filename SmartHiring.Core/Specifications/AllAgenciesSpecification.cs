using SmartHiring.Core.Entities.Identity;

namespace SmartHiring.Core.Specifications
{
    public class AllAgenciesSpecification : BaseSpecifications<AppUser>
    {
        public AllAgenciesSpecification()
           : base(a => a.AgencyName != null) // نفترض أن الـ Agencies عندهم AgencyName
        {
        }
    }
}
