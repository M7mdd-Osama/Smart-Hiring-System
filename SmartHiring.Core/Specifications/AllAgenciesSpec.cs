using SmartHiring.Core.Entities.Identity;

namespace SmartHiring.Core.Specifications
{
    public class AllAgenciesSpec : BaseSpec<AppUser>
    {
        public AllAgenciesSpec()
           : base(a => a.AgencyName != null)
        {
        }
    }
}
