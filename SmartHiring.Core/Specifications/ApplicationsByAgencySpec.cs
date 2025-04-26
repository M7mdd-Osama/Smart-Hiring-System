using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsByAgencySpec : BaseSpec<Application>
    {
        public ApplicationsByAgencySpec(string agencyUserId)
        : base(a => a.AgencyId == agencyUserId)
        {
            AddInclude(a => a.Post);
            AddInclude(a => a.Post.Company);
        }
    }
}
