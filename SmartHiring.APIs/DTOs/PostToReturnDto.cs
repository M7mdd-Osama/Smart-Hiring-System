using SmartHiring.Core.Entities;

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
		public decimal JobSalary { get; set; }
		public string JobStatus { get; set; }
		public string CompanyName { get; set; }
	}
}
