﻿namespace SmartHiring.APIs.DTOs
{
	public class CandidateListDto
	{
		public int Id { get; set; }
		public string JobTitle { get; set; }
		public int TotalApplicants { get; set; }
		public int Remaining { get; set; }
		public int Progress { get; set; }
	}
}
