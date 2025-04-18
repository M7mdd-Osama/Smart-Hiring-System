using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class HiredInterviewsWithHRSpec : BaseSpec<Interview>
{
    public HiredInterviewsWithHRSpec(DateTime fromDate, DateTime toDate, int? companyId = null)
        : base(i =>
            i.InterviewStatus == InterviewStatus.Hired &&
            i.Date >= fromDate &&
            i.Date <= toDate &&
            (companyId == null || i.Post.Company.Id == companyId))
    {
        AddInclude(i => i.HR);
        AddInclude(i => i.Post);
        AddInclude(i => i.Post.Company);
    }
}

}
