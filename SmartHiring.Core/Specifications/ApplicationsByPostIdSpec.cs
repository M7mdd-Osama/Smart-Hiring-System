using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsByPostIdSpec : BaseSpec<Application>
    {
		public ApplicationsByPostIdSpec(int postId)
		   : base(a => a.PostId == postId)
		{
			AddInclude(a => a.Applicant);
			AddInclude(a => a.Agency);
		}
	}
}
