using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Entities
{
	public enum InterviewStatus
	{
		Pending,
		Accepted,
		Rejected
	}
	public class Interview : BaseEntity
	{
		public DateTime Date { get; set; }
		public TimeSpan Time { get; set; }
		public string Location { get; set; }
		public InterviewStatus InterviewStatus { get; set; }
		public double Score { get; set; }


		public int HRId { get; set; }
		public HR HR { get; set; }

		public int PostId { get; set; }
		public Post Post { get; set; }

		public int ApplicantId { get; set; }
		public Applicant Applicant { get; set; }
	}
}
