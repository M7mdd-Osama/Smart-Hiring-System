using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class Applicant : BaseEntity
	{
		public string FName { get; set; }
		public string LName { get; set; }
		public string Email { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Skills { get; set; }
		
		public ICollection<ApplicantPhone> ApplicantPhones { get; set; } = new HashSet<ApplicantPhone>();
		public ICollection<ApplicantAddress> ApplicantAddresses { get; set; } = new HashSet<ApplicantAddress>();
		public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
		public ICollection<CandidateListApplicant> CandidateListApplicants { get; set; } = new HashSet<CandidateListApplicant>();


	}
}
