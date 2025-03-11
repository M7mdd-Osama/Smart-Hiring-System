namespace SmartHiring.APIs.DTOs
{
	public class PostPaymentDto
	{
		public int Id { get; set; }
		public string JobTitle { get; set; }
		public string PaymentStatus { get; set; }
		public string? PaymentIntentId { get; set; }
		public string? ClientSecret { get; set; }
	}
}
