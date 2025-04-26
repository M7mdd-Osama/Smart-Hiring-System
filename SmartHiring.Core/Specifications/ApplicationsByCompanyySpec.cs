using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsByCompanyySpec : BaseSpec<Application>
    {
        public ApplicationsByCompanyySpec(int companyId, DateTime fromDate, DateTime toDate)
            : base(a =>
                a.Post.CompanyId == companyId &&
                a.ApplicationDate >= fromDate.Date &&
                a.ApplicationDate < toDate.Date.AddDays(1))
        {
            AddInclude(a => a.Post);
            AddInclude(a => a.Post.Company);
        }
    }
}
