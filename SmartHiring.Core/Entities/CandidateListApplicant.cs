using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class CandidateListApplicant
	{
		public int ApplicantId { get; set; }
		public Applicant Applicant { get; set; }

		public int CandidateListId { get; set; }
		public CandidateList CandidateList { get; set; }


	}
}
