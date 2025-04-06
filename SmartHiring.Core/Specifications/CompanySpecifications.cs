using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class CompanySpecifications : BaseSpecifications<Company>
	{

		public CompanySpecifications() : base()
		{
			AddInclude(p => p.HR);
			AddInclude(p => p.Manager);
		}

		public CompanySpecifications(int companyId) : base(c => c.Id == companyId)
		{
			AddInclude(p => p.HR);
			AddInclude(p => p.Manager);
		}
	}
}
