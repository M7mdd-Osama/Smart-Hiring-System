using SmartHiring.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class AgencyApplicant : BaseEntity
	{
		public string AgencyId { get; set; }
		public AppUser Agency { get; set; }

		public int ApplicantId { get; set; }
		public Applicant Applicant { get; set; }
	}
}
