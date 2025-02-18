using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class HR : BaseEntity
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }

		public int CompanyId { get; set; }
		public Company Company { get; set; }

		public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
		public ICollection<Interview> Interviews { get; set; } = new HashSet<Interview>();
	}
}
