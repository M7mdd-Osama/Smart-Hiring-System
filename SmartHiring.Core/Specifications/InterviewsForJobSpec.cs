using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class InterviewsForJobSpec : BaseSpec<Interview>
    {
        public InterviewsForJobSpec(int jobId, DateTime fromDate, DateTime toDate)
            : base(i =>
                i.PostId == jobId &&
                i.Date >= fromDate &&
                i.Date <= toDate)
        {
            AddInclude(i => i.Post);
            AddInclude(i => i.Applicant);

        }
    }
}
