namespace SmartHiring.APIs.DTOs
{
    public class CompanyToReturnDto
    {
        public int Id { get; set; }
		public string Name { get; set; }
		public string BusinessEmail { get; set; }

		public string LogoUrl { get; set; }

		public bool EmailConfirmed { get; set; } = false;

		public string ManagerId { get; set; }
		public string Manager { get; set; }

		public string HRId { get; set; }
		public string HR { get; set; }

		public string Phone { get; set; }
	}
}
