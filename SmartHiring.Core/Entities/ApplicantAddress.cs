using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class ApplicantAddress
	{
		public int ApplicantId { get; set; }
		public string Country { get; set; }
		public Applicant Applicant { get; set; }

		public string City { get; set; }
		public string ZIP { get; set; }


	}
}
