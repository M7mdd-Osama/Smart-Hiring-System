﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public class Post : BaseEntity
	{
		public string JobTitle { get; set; }
		public string Description { get; set; }
		public string Requirements { get; set; }
		public DateTime PostDate { get; set; }
		public DateTime Deadline { get; set; }
		public decimal JobSalary { get; set; }
		public string JobStatus { get; set; }

		public int HRId { get; set; }
		public HR HR { get; set; }

		public ICollection<Application> Applications { get; set; } = new HashSet<Application>();
		public ICollection<CandidateList> CandidateLists { get; set; } = new HashSet<CandidateList>();


	}
}
