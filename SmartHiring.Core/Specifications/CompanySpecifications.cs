using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
	public class CompanySpecifications : BaseSpecifications<Company>
	{

		public CompanySpecifications() : base()
		{
			Includes.Add(p => p.CompanyPhones);
			Includes.Add(p => p.HR);
			Includes.Add(p => p.Manager);
		}

		public CompanySpecifications(int companyId) : base(c => c.Id == companyId) { }
	}
}
