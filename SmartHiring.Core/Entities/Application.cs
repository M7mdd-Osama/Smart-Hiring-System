using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class Application : BaseEntity
	{
		public double RankScore { get; set; }
		public bool IsShortlisted { get; set; }
		public DateTime ApplicationDate { get; set; }
		public string CV_Link { get; set; }

		public int ApplicantId { get; set; }
		[ForeignKey("ApplicantId")]
		public Applicant Applicant { get; set; }

		public int PostId { get; set; }
		public Post Post { get; set; }

		public int? AgencyId { get; set; }
		public Agency? Agency { get; set; }
	}
}
