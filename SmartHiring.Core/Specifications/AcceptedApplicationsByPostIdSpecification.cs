using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class AcceptedApplicationsByPostIdSpecification : BaseSpecifications<Application>
	{
		public AcceptedApplicationsByPostIdSpecification(int postId)
			: base(app => app.PostId == postId && app.IsShortlisted)
		{
			AddInclude(app => app.Applicant);
		}
	}
}
