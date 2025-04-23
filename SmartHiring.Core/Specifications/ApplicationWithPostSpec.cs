using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationWithPostSpec : BaseSpec<Application>
    {
        public ApplicationWithPostSpec(int applicationId)
            : base(a => a.Id == applicationId)
        {
            AddInclude(a => a.Post);
        }
    }
}
