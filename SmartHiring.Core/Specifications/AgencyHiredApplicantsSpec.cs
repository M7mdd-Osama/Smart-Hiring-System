using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class AgencyHiredApplicantsSpec : BaseSpec<Interview>
    {
        public AgencyHiredApplicantsSpec(List<int> applicantIds)
            : base(i => applicantIds.Contains(i.ApplicantId) && i.InterviewStatus == InterviewStatus.Hired)
        {
            AddInclude(i => i.Applicant);
            AddInclude(i => i.HR);
            AddInclude(i => i.HR.HRCompany);
        }

    }
}