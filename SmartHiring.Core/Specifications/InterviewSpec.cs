using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class InterviewSpec : BaseSpec<Interview>
	{
		public InterviewSpec(int interviewId)
			: base(i => i.Id == interviewId)
		{
			AddInclude(i => i.Applicant);
			AddInclude(i => i.Post);
			AddInclude(i => i.Post.Company);
			AddInclude(i => i.Applicant.Applications);
			AddIncludeString("Applicant.Applications.Agency");
		}
	}
}