﻿using SmartHiring.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SmartHiring.Core.Entities
{
	public enum InterviewStatus
	{
		[EnumMember(Value = "Pending")]
		Pending,
		[EnumMember(Value = "Accepted")]
		Accepted,
		[EnumMember(Value = "Rejected")]
		Rejected
	}
	public class Interview : BaseEntity
	{
		public DateTime Date { get; set; }
		public TimeSpan Time { get; set; }
		public string Location { get; set; }

		public InterviewStatus InterviewStatus { get; set; }
		public double Score { get; set; }


		public string HRId { get; set; }
		[ForeignKey("HRId")]
		public AppUser HR { get; set; }

		public int PostId { get; set; }
		[ForeignKey("PostId")]
		public Post Post { get; set; }

		public int ApplicantId { get; set; }
		public Applicant Applicant { get; set; }
	}
}
