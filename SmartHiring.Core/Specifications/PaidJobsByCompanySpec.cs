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
        public PaidJobsByCompanySpec(DateTime fromDate, DateTime toDate)
        : base(p => p.PaymentStatus == "Paid" && p.PostDate >= fromDate && p.PostDate <= toDate)
        {
        }

        public PaidJobsByCompanySpec(int companyId, DateTime fromDate, DateTime toDate)
            : base(p => p.CompanyId == companyId && p.PaymentStatus == "Paid" && p.PostDate >= fromDate && p.PostDate <= toDate)
        {
        }
    }
}
