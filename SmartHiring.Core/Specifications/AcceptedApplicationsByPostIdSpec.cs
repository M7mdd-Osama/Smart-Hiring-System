using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class AcceptedApplicationsByPostIdSpec : BaseSpec<Application>
	{
		public AcceptedApplicationsByPostIdSpec(int postId)
			: base(app => app.PostId == postId && app.IsShortlisted)
		{
			AddInclude(app => app.Applicant);
		}
	}
}
