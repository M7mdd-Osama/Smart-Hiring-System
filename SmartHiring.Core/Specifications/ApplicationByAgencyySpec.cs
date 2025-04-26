using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationByAgencyySpec : BaseSpec<Application>
    {
        public ApplicationByAgencyySpec(string agencyId, DateTime fromDate, DateTime toDate)
            : base(a =>
                a.AgencyId == agencyId &&
                a.ApplicationDate >= fromDate.Date &&
                a.ApplicationDate < toDate.Date.AddDays(1))
        {
            AddInclude(a => a.Agency);
        }
    }
}
