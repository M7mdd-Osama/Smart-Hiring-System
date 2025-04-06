namespace SmartHiring.APIs.DTOs
{
	public class CompanyDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string BusinessEmail { get; set; }
        public string Phone { get; set; }
        public string LogoUrl { get; set; }
		public string HRId { get; set; }
		public string HRName { get; set; }
		public string HREmail { get; set; }
        public string PhoneNumberHR { get; set; }

        public string ManagerId { get; set; }
		public string ManagerName { get; set; }
		public string ManagerEmail { get; set; }
        public string PhoneNumberManager { get; set; }

    }
}