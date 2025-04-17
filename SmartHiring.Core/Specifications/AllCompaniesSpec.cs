using SmartHiring.Core.Entities;
namespace SmartHiring.Core.Specifications
{
    public class AllCompaniesSpec : BaseSpec<Company>
    {
        public AllCompaniesSpec(DateTime fromDate, DateTime toDate)
            : base(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate)
        {
        }
    }
}
