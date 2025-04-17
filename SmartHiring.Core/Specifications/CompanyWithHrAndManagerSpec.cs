using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
	public class CompanyWithHrAndManagerSpec : BaseSpec<Company>
	{
		public CompanyWithHrAndManagerSpec() : base()
		{
			AddInclude(c => c.Manager);
			AddInclude(c => c.HR);
		}
		public CompanyWithHrAndManagerSpec(int id) : base(c => c.Id == id)
		{
			AddInclude(c => c.Manager);
			AddInclude(c => c.HR);
		}

	}
}
