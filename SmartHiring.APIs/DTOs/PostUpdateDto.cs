namespace SmartHiring.APIs.DTOs
{
	public class PostUpdateDto
	{
		public string? JobTitle { get; set; }
		public ICollection<string>? JobCategories { get; set; }
		public ICollection<int>? JobTypes { get; set; }
		public ICollection<int>? Workplaces { get; set; }
		public string? Country { get; set; }
		public string? City { get; set; }

		public string? Description { get; set; }
		public string? Requirements { get; set; }
		public DateTime? Deadline { get; set; }
		public decimal? MinSalary { get; set; }
		public decimal? MaxSalary { get; set; }
		public string? Currency { get; set; }
		public int? MinExperience { get; set; }
		public int? MaxExperience { get; set; }
		public ICollection<string>? Skills { get; set; }
		public ICollection<int>? CareerLevels { get; set; }
	}
}