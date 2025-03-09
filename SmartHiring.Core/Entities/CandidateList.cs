using SmartHiring.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class CandidateList : BaseEntity
	{
		public DateTime GeneratedDate { get; set; }
		public string Status { get; set; }

		public string ManagerId { get; set; }
		public AppUser Manager { get; set; }
		public int PostId { get; set; }
		public Post Post { get; set; }

		public ICollection<CandidateListApplicant> CandidateListApplicants { get; set; } = new HashSet<CandidateListApplicant>();
	}
}
