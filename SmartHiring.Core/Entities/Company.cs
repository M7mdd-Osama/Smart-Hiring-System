using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class Company : BaseEntity
	{
        public int Id { get; set; }
        public string Name { get; set; }
		public string Location { get; set; }
		public string Industry { get; set; }
		public string BusinessEmail { get; set; }
 

        public int AdminId { get; set; }
		public Admin Admin { get; set; }

		public int ManagerId { get; set; }
		public Manager Manager { get; set; }

		public ICollection<HR> HRs { get; set; } = new HashSet<HR>();
		public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
		public ICollection<CompanyPhone> CompanyPhones { get; set; } = new HashSet<CompanyPhone>();

	}
}
