using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class InterviewWithCandidateSpecifications : BaseSpecifications<Interview>
    {
        public InterviewWithCandidateSpecifications()
            : base()
        {
            Includes.Add(i => i.Applicant);
            Includes.Add(i => i.HR);
            Includes.Add(i => i.Post);
        }

        public InterviewWithCandidateSpecifications(int interviewId)
            : base(i => i.Id == interviewId)
        {
            Includes.Add(i => i.Applicant);
            Includes.Add(i => i.HR);
            Includes.Add(i => i.Post);
        }

        public InterviewWithCandidateSpecifications(int? applicantId, string? hrId)
            : base(i =>
                (!applicantId.HasValue || i.ApplicantId == applicantId.Value) &&
                (string.IsNullOrEmpty(hrId) || i.HRId == hrId)) // ✅ الحل الصحيح هنا
        {
            Includes.Add(i => i.Applicant);
            Includes.Add(i => i.HR);
            Includes.Add(i => i.Post);
        }
    }
}
