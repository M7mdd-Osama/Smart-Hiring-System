using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class CompanyPhone
	{
		public int CompanyId { get; set; }
		public Company Company { get; set; }

		public string Phone { get; set; }

	}
}
