using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
	public class InterviewWithCandidateSpecifications : BaseSpecifications<Interview>
	{
		public InterviewWithCandidateSpecifications() : base()
		{
			Includes.Add(i => i.Applicant);
			Includes.Add(i => i.HR);
		}
	}
}
