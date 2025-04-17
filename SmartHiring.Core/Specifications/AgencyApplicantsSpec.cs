using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class AgencyApplicantsSpec : BaseSpec<AgencyApplicant>
    {
        public AgencyApplicantsSpec(string agencyId)
            : base(aa => aa.AgencyId == agencyId)
        {
            AddInclude(aa => aa.Applicant);
        }
    }
}
