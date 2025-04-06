namespace SmartHiring.Core.Entities
{
	public class ApplicantAddress
	{
		public int ApplicantId { get; set; }
		public string Country { get; set; }
		public Applicant Applicant { get; set; }

		public string City { get; set; }
		public string ZIP { get; set; }
	}
}
