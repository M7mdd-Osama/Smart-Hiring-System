using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PaidJobsByCompanySpec : BaseSpec<Post>
    {
        public PaidJobsByCompanySpec(int companyId)
            : base(p => p.CompanyId == companyId && p.PaymentStatus == "Paid")
        {
        }

        public PaidJobsByCompanySpec()
            : base(p => p.PaymentStatus == "Paid")
        {
        }
    }
}
