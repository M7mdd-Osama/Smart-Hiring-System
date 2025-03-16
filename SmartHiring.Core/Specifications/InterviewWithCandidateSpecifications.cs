using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class InterviewWithCandidateSpecifications : BaseSpecifications<Interview>
    {
        public InterviewWithCandidateSpecifications()
            : base()
        {
			AddInclude(i => i.Applicant);
            AddInclude(i => i.HR);
            AddInclude(i => i.Post);
        }

        public InterviewWithCandidateSpecifications(int interviewId)
            : base(i => i.Id == interviewId)
        {
            AddInclude(i => i.Applicant);
            AddInclude(i => i.HR);
            AddInclude(i => i.Post);
        }

        public InterviewWithCandidateSpecifications(int? applicantId, string? hrId)
            : base(i =>
                (!applicantId.HasValue || i.ApplicantId == applicantId.Value) &&
                (string.IsNullOrEmpty(hrId) || i.HRId == hrId))
        {
			AddInclude(i => i.Applicant);
            AddInclude(i => i.HR);
            AddInclude(i => i.Post);
        }
    }
}
