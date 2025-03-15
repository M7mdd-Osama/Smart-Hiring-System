using SmartHiring.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

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

		public string AgencyId { get; set; }
		[ForeignKey("AgencyId")]
		public AppUser Agency { get; set; }
	}
}
