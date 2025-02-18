using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class Admin : BaseEntity
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }

		public ICollection<Company> Companies { get; set; } = new HashSet<Company>();

	}
}
