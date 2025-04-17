using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class CompanySpec : BaseSpec<Company>
	{
		public CompanySpec() : base()
		{
			AddInclude(p => p.HR);
			AddInclude(p => p.Manager);
		}
		public CompanySpec(int companyId) : base(c => c.Id == companyId)
		{
			AddInclude(p => p.HR);
			AddInclude(p => p.Manager);
		}
	}
}
