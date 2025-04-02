using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class ApplicationsByPostIdSpecification : BaseSpecifications<Application>
    {
		public ApplicationsByPostIdSpecification(int postId)
		   : base(a => a.PostId == postId)
		{
			AddInclude(a => a.Applicant);
			AddInclude(a => a.Agency);
		}
	}
}
