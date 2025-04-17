using SmartHiring.Core.Entities.Identity;

namespace SmartHiring.Core.Entities
{
	public class Company : BaseEntity
	{
		public string Name { get; set; }
		public string BusinessEmail { get; set; }

		public string Password { get; set; }
		public string? LogoUrl { get; set; }

		public string? ConfirmationCode { get; set; }
		public DateTime? ConfirmationCodeExpires { get; set; }
		public bool EmailConfirmed { get; set; } = false;
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ManagerId { get; set; }
		public AppUser? Manager { get; set; }

		public string? HRId { get; set; }
		public AppUser? HR { get; set; }

		public ICollection<Post> Posts { get; set; } = new HashSet<Post>();

	}
}
