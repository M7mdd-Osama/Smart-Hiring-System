using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class InterviewsForJobSpec : BaseSpec<Interview>
    {
        public InterviewsForJobSpec(int jobId)
            : base(i => i.PostId == jobId)
        {
            AddInclude(i => i.Post);
            AddInclude(i => i.Applicant);

        }
    }
}
