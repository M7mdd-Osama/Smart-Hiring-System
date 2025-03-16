using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
	public class CompanyWithHrAndManagerSpacifications : BaseSpecifications<Company>
	{
		public CompanyWithHrAndManagerSpacifications() : base()
		{
			AddInclude(c => c.Manager);
			AddInclude(c => c.HR);
		}
		public CompanyWithHrAndManagerSpacifications(int id) : base(c => c.Id == id) // Where(c => c.Id == id)
		{
			AddInclude(c => c.Manager);
			AddInclude(c => c.HR);
		}

	}
}
