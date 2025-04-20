using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class JobsWithInterviewsSpec : BaseSpec<Post>
    {
        public JobsWithInterviewsSpec(int companyId, DateTime fromDate, DateTime toDate)
            : base(j => j.Company.Id == companyId &&
                        j.Interviews.Any(i => i.Date >= fromDate && i.Date <= toDate))
        {
            AddInclude(j => j.Interviews);
        }
    }
}
