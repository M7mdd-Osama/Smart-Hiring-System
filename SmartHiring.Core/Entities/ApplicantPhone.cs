using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class ApplicantPhone
	{
		public int ApplicantId { get; set; }
		public Applicant Applicant { get; set; }

		public string Phone { get; set; }

	}
}
