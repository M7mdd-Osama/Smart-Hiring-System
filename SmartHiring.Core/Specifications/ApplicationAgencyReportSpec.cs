

using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationAgencyReportSpec : BaseSpec<Application>
    {
        public ApplicationAgencyReportSpec()
            : base(a => a.AgencyId != null) // شرط مبدئي أو فارغ حسب الحاجة
        {
            AddInclude(a => a.Agency);
            AddInclude(a => a.Post);
        }
    }

}
