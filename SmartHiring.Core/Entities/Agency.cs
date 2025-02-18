using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class Agency : BaseEntity
	{
		public string Name { get; set; }
		public string ApplicantPool { get; set; }
		public string Address { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }

		public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
		public ICollection<AgencyApplicant> AgencyApplicants { get; set; } = new HashSet<AgencyApplicant>();



	}
}
