using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class AgencyApplicant
	{
		public int AgencyId { get; set; }
		public Agency Agency { get; set; }

		public int ApplicantId { get; set; }
		public Applicant Applicant { get; set; }
	}
}
