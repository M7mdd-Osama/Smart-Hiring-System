namespace SmartHiring.APIs.DTOs
{
	public class PostToReturnDto
	{
		public int Id { get; set; }
		public string JobTitle { get; set; }
		public string Description { get; set; }
		public string Requirements { get; set; }
		public DateTime PostDate { get; set; }
		public DateTime Deadline { get; set; }
		public string JobStatus { get; set; }
		public decimal MinSalary { get; set; }
		public decimal MaxSalary { get; set; }
		public string Currency { get; set; }
		public bool HideSalary { get; set; }
		public int MinExperience { get; set; }
		public int MaxExperience { get; set; }
		public string PaymentStatus { get; set; }
		public string Country { get; set; }
		public string City { get; set; }

		public int CompanyId { get; set; }
		public string CompanyName { get; set; }
		public string LogoUrl { get; set; }

		public string HRId { get; set; }
		public string HRName { get; set; }

		public int TotalApplications { get; set; }
		public int SelectedCandidates { get; set; }

		public ICollection<string> JobCategories { get; set; }
		public ICollection<string> JobTypes { get; set; }
		public ICollection<string> Workplaces { get; set; }
		public ICollection<string> Skills { get; set; }
		public ICollection<string> CareerLevels { get; set; }

		public bool IsSaved { get; set; }
	}
}
