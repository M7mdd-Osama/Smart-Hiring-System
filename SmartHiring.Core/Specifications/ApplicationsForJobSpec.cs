using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsForJobSpec : BaseSpec<Application>
    {
        public ApplicationsForJobSpec(int jobId)
            : base(app => app.PostId == jobId)
        {
            AddInclude(app => app.Post);
        }
    }
}
