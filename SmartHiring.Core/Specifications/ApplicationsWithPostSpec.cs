using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsWithPostSpec : BaseSpec<Application>
    {
        public ApplicationsWithPostSpec()
        {
            AddInclude(app => app.Post);
        }
    }
}
